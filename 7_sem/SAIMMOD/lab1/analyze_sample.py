from typing import List, Tuple
import matplotlib.pyplot as plt

from LemerGenerator import build_optimal_generator, GENERATOR_CONFIGS


def generate_sample_data(generator_type: str, x0: int, sample_size: int) -> Tuple[
    List[int], List[float]]:

    gen = build_optimal_generator(generator_type, x0)

    integer_sample = gen.sequence(sample_size)

    normalized_sample = [x / gen.m for x in integer_sample]

    return integer_sample, normalized_sample


def build_histogram(normalized_sample: List[float], generator_type: str, bins: int = 20) -> None:
    sample_size = len(normalized_sample)
    config_name = GENERATOR_CONFIGS[generator_type]["name"]

    plt.figure(figsize=(10, 6))

    counts, _, _ = plt.hist(normalized_sample, bins=bins, alpha=0.7,
                            color='skyblue', edgecolor='black', linewidth=0.5)

    expected_freq = sample_size / bins
    plt.axhline(y=expected_freq, color='red', linestyle='--', linewidth=2,
                label=f'Ожидаемая частота: {expected_freq:.0f}')

    plt.xlabel('Значения [0, 1)')
    plt.ylabel('Частота')
    plt.title(f'Гистограмма распределения: {config_name} ({bins} интервалов)')
    plt.legend()
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.show()


def print_sample_info(integer_sample: List[int], normalized_sample: List[float],
                      generator_type: str) -> None:
    sample_size = len(integer_sample)
    config_name = GENERATOR_CONFIGS[generator_type]["name"]

    print(f"Генератор: {config_name}")
    print(f"Размер выборки: {sample_size:,}")
    print(f"Первые 10 значений: {[f'{x:.4f}' for x in normalized_sample[:10]]}")


def main():
    sample_size = 50000
    bins = 25
    x0 = 42
    generator_type = "mix"

    print("=" * 50)
    print("ГЕНЕРАТОР ПСЕВДОСЛУЧАЙНЫХ ЧИСЕЛ")
    print("=" * 50)

    integer_data, normalized_data = generate_sample_data(generator_type, x0, sample_size)

    print_sample_info(integer_data, normalized_data, generator_type)

    build_histogram(normalized_data, generator_type, bins)

main()