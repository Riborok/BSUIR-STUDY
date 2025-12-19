from typing import List, Tuple, Dict, Any
import matplotlib.pyplot as plt
import numpy as np
from scipy import stats

from LemerGenerator import build_optimal_generator, GENERATOR_CONFIGS


def chi_square_test(observed_freq: np.ndarray, sample_size: int, bins: int) -> Dict[str, Any]:
    min_freq = int(np.min(observed_freq))
    condition_satisfied = min_freq >= 5

    expected_freq = sample_size / bins

    V = float(np.sum((observed_freq - expected_freq) ** 2 / expected_freq))

    nu = bins - 1

    chi2_001 = stats.chi2.ppf(0.99, nu)
    chi2_005 = stats.chi2.ppf(0.95, nu)
    chi2_010 = stats.chi2.ppf(0.90, nu)
    chi2_990 = stats.chi2.ppf(0.01, nu)

    p_value = 1 - stats.chi2.cdf(V, nu)

    if V < chi2_990 or V > chi2_001:
        interpretation = "БРАКУЮТСЯ как недостаточно случайные"
        category = "Неприемлемо"
    elif (chi2_990 <= V < stats.chi2.ppf(0.05, nu)) or (stats.chi2.ppf(0.95, nu) < V <= chi2_001):
        interpretation = "ПОДОЗРИТЕЛЬНЫЕ"
        category = "Подозрительно"
    elif (stats.chi2.ppf(0.05, nu) <= V < chi2_010) or (stats.chi2.ppf(0.90, nu) < V <= stats.chi2.ppf(0.95, nu)):
        interpretation = "СЛЕГКА ПОДОЗРИТЕЛЬНЫЕ"
        category = "Слегка подозрительно"
    else:
        interpretation = "ПРИЕМЛЕМЫЕ"
        category = "Приемлемо"

    return {
        "V": V,
        "nu": nu,
        "p_value": p_value,
        "expected_freq": expected_freq,
        "min_observed_freq": min_freq,
        "condition_satisfied": condition_satisfied,
        "interpretation": interpretation,
        "category": category,
        "critical_values": {
            0.01: stats.chi2.ppf(0.99, nu),
            0.05: chi2_005,
            0.10: chi2_010,
            0.99: chi2_001,
        },
    }


def generate_full_sequence(generator_type: str, x0: int, total_size: int) -> Tuple[List[int], List[float]]:
    gen = build_optimal_generator(generator_type, x0)

    integer_sample = gen.sequence(total_size)
    normalized_sample = [x / gen.m for x in integer_sample]

    return integer_sample, normalized_sample


def split_sequence_into_parts(sequence: List[float], num_parts: int = 3) -> List[List[float]]:
    part_size = len(sequence) // num_parts
    parts = []

    for i in range(num_parts):
        start_idx = i * part_size
        if i == num_parts - 1:
            end_idx = len(sequence)
        else:
            end_idx = (i + 1) * part_size

        parts.append(sequence[start_idx:end_idx])

    return parts


def build_histogram_with_chi2(normalized_sample: List[float], generator_type: str,
                              part_num: int, start_idx: int, end_idx: int, bins: int = 25) -> Dict[str, Any]:
    sample_size = len(normalized_sample)
    config_name = GENERATOR_CONFIGS[generator_type]["name"]

    observed_freq, _ = np.histogram(normalized_sample, bins=bins, range=(0, 1))

    chi2_result = chi_square_test(observed_freq, sample_size, bins)

    plt.figure(figsize=(10, 6))

    counts, _, _ = plt.hist(normalized_sample, bins=bins, alpha=0.7,
                            color='skyblue', edgecolor='black', linewidth=0.5)

    expected_freq = sample_size / bins
    plt.axhline(y=expected_freq, color='red', linestyle='--', linewidth=2,
                label=f'Ожидаемая частота: {expected_freq:.0f}')

    plt.xlabel('Значения [0, 1)')
    plt.ylabel('Частота')
    plt.title(
        f'Часть #{part_num}: {config_name} (элементы {start_idx}-{end_idx})\nV = {chi2_result["V"]:.4f}, p = {chi2_result["p_value"]:.4f}')
    plt.legend()
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.show()

    return chi2_result


def run_chi_square_tests(generator_type: str, x0: int, total_sample_size: int, bins: int, num_parts: int = 3) -> None:
    print("=" * 70)
    print("КРИТЕРИЙ ХИ-КВАДРАТ")
    print("=" * 70)
    print(f"Генератор: {GENERATOR_CONFIGS[generator_type]['name']}")
    print(f"Общий размер последовательности: {total_sample_size:,}")
    print(f"Количество частей для анализа: {num_parts}")
    print(f"Количество интервалов: {bins}")
    print(f"Начальное значение: {x0}")
    print()

    print("Генерация полной последовательности...")
    integer_sample, normalized_sample = generate_full_sequence(generator_type, x0, total_sample_size)

    parts = split_sequence_into_parts(normalized_sample, num_parts)

    results = []
    for part_num, part_sample in enumerate(parts, start=1):
        print(f"АНАЛИЗ ЧАСТИ #{part_num}")
        print("-" * 40)

        part_size = len(part_sample)
        start_idx = (part_num - 1) * (total_sample_size // num_parts)
        if part_num == num_parts:
            end_idx = total_sample_size - 1
        else:
            end_idx = start_idx + part_size - 1

        print(f"Размер части: {part_size:,} элементов")
        print(f"Диапазон элементов: {start_idx} - {end_idx}")
        print()

        chi2_result = build_histogram_with_chi2(part_sample, generator_type, part_num, start_idx, end_idx, bins)
        results.append(chi2_result)

        print(f"Статистика V = {chi2_result['V']:.6f}")
        print(f"p-value = {chi2_result['p_value']:.6f}")
        print(f"Категория: {chi2_result['interpretation']}")

        is_acceptable = chi2_result["category"] in ["Приемлемо", "Слегка подозрительно"]
        print(f"Результат: {'ПРИЕМЛЕМАЯ' if is_acceptable else 'НЕ ПРИЕМЛЕМАЯ'}")
        print()

    print("СВОДНЫЙ АНАЛИЗ РЕЗУЛЬТАТОВ")
    print("=" * 50)

    v_statistics = [r["V"] for r in results]
    p_values = [r["p_value"] for r in results]
    accepted_count = sum(1 for r in results if r["category"] in ["Приемлемо", "Слегка подозрительно"])

    print(f"Статистики V: {[f'{v:.6f}' for v in v_statistics]}")
    print(f"p-values: {[f'{p:.6f}' for p in p_values]}")
    print(f"Средняя статистика V: {np.mean(v_statistics):.6f}")
    print(f"Средний p-value: {np.mean(p_values):.6f}")
    print()

    print("Результаты тестов по частям:")
    for i, r in enumerate(results, 1):
        status = "ПРИНЯТА" if r["category"] in ["Приемлемо", "Слегка подозрительно"] else "ОТКЛОНЕНА"
        print(f"  Часть #{i}: {status} ({r['interpretation']})")

    print()
    print(f"Итого принято: {accepted_count}/{num_parts}")

    if accepted_count >= num_parts - 1:
        final_conclusion = "ГЕНЕРАТОР КАЧЕСТВЕННЫЙ"
        print(f"ОКОНЧАТЕЛЬНЫЙ ВЫВОД: {final_conclusion}")
    elif accepted_count >= num_parts // 2:
        final_conclusion = "ГЕНЕРАТОР УДОВЛЕТВОРИТЕЛЬНЫЙ"
        print(f"ОКОНЧАТЕЛЬНЫЙ ВЫВОД: {final_conclusion}")
    else:
        final_conclusion = "ГЕНЕРАТОР НЕКАЧЕСТВЕННЫЙ"
        print(f"ОКОНЧАТЕЛЬНЫЙ ВЫВОД: {final_conclusion}")


def main():
    total_sample_size = 100
    bins = 20
    x0 = 2316823157
    generator_type = "mix"
    num_parts = 3

    run_chi_square_tests(generator_type, x0, total_sample_size * num_parts, bins, num_parts)


main()