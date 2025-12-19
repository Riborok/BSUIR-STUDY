from typing import List, Tuple, Dict, Any
import matplotlib.pyplot as plt
import numpy as np

from LemerGenerator import build_optimal_generator, GENERATOR_CONFIGS


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


def ks_table_critical(n: int) -> Dict[float, float]:
    table = {
        0.99: (0.0709, 0.15),
        0.95: (0.1601, 0.14),
        0.75: (0.3793, 0.15),
        0.50: (0.5887, 0.15),
        0.25: (0.8326, 0.16),
        0.05: (1.2239, 0.17),
        0.01: (1.5174, 0.20),
    }
    crit = {}
    for p, (a, b) in table.items():
        crit[p] = a - b / np.sqrt(n)
    return crit


def kolmogorov_smirnov_stats(sample: List[float]) -> Dict[str, Any]:
    n = len(sample)
    if n == 0:
        raise ValueError("Пустая выборка для КС-теста")

    sorted_sample = np.sort(np.asarray(sample))
    Fn = np.arange(1, n + 1) / n
    F_theor = sorted_sample

    D_plus = np.max(Fn - F_theor)
    D_minus = np.max(F_theor - (np.arange(0, n) / n))
    D = max(D_plus, D_minus)

    Kn_plus = np.sqrt(n) * D_plus
    Kn_minus = np.sqrt(n) * D_minus
    Kn = np.sqrt(n) * D

    crit = ks_table_critical(n)

    p_interval = None
    for p in sorted(crit.keys(), reverse=True):
        if Kn < crit[p]:
            p_interval = p
            break
    if p_interval is None:
        p_interval = "<0.01"

    return {
        "n": n,
        "D_plus": float(D_plus),
        "D_minus": float(D_minus),
        "D": float(D),
        "Kn_plus": float(Kn_plus),
        "Kn_minus": float(Kn_minus),
        "Kn": float(Kn),
        "p_interval": p_interval,
        "critical_values": crit,
        "sorted_sample": sorted_sample,
        "Fn": Fn,
    }


def plot_empirical_vs_theoretical(sorted_sample: np.ndarray, Fn: np.ndarray, ks_res: Dict[str, Any], part_num: int):
    n = ks_res["n"]
    D_plus = ks_res["D_plus"]
    D_minus = ks_res["D_minus"]
    Kn = ks_res["Kn"]

    plt.figure(figsize=(9, 6))
    plt.step(sorted_sample, Fn, where="post", label="Эмпирическая Fn(x)", linewidth=1.5, color="blue")
    plt.plot(sorted_sample, sorted_sample, linestyle="--", label="Теоретическая F(x)=x", linewidth=1.2, color="black")

    idx_plus = int(np.argmax(Fn - sorted_sample))
    x_plus = sorted_sample[idx_plus]
    y_emp_plus = Fn[idx_plus]
    y_theor_plus = x_plus
    plt.vlines(x_plus, y_theor_plus, y_emp_plus, colors="green", linestyles="-", linewidth=2)
    plt.scatter([x_plus], [y_emp_plus], color="green", zorder=5, s=60, label=f"D+ = {D_plus:.4f}")
    plt.text(x_plus, (y_emp_plus + y_theor_plus) / 2, f"D+={D_plus:.4f}",
             color="green", fontsize=10, ha="left", va="center",
             bbox=dict(facecolor="white", alpha=0.6, edgecolor="green"))

    F_emp_prev = np.arange(0, n) / n
    idx_minus = int(np.argmax(sorted_sample - F_emp_prev))
    x_minus = sorted_sample[idx_minus]
    y_theor_minus = x_minus
    y_emp_minus = F_emp_prev[idx_minus]

    plt.vlines(x_minus, y_emp_minus, y_theor_minus, colors="red", linestyles="-", linewidth=2)
    plt.scatter([x_minus], [y_theor_minus], color="red", zorder=5, s=60, label=f"D- = {D_minus:.4f}")
    plt.text(x_minus, (y_emp_minus + y_theor_minus) / 2, f"D-={D_minus:.4f}",
             color="red", fontsize=10, ha="right", va="center",
             bbox=dict(facecolor="white", alpha=0.6, edgecolor="red"))

    plt.xlabel("x")
    plt.ylabel("F(x)")
    plt.title(f"КС-критерий — часть #{part_num} (n={n}) — Kn = {Kn:.4f}, p-отрезок: {ks_res['p_interval']}")
    plt.legend()
    plt.grid(True, alpha=0.3)
    plt.tight_layout()
    plt.show()


def run_kolmogorov_smirnov_tests(generator_type: str,
                                 x0: int,
                                 total_sample_size: int,
                                 num_parts: int = 3) -> None:
    print("=" * 70)
    print("КРИТЕРИЙ КОЛМОГОРОВА–СМИРНОВА (КС)")
    print("=" * 70)
    print(f"Генератор: {GENERATOR_CONFIGS[generator_type]['name']}")
    print(f"Общий размер последовательности: {total_sample_size:,}")
    print(f"Количество частей: {num_parts}")
    print(f"Начальное значение (x0): {x0}")
    print()

    print("Генерация полной последовательности...")
    _, normalized_sample = generate_full_sequence(generator_type, x0, total_sample_size)

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

        ks_res = kolmogorov_smirnov_stats(part_sample)

        print(f"K+ (D_plus) = {ks_res['D_plus']:.6f}")
        print(f"K- (D_minus) = {ks_res['D_minus']:.6f}")
        print(f"Нормированное K (Kn) = {ks_res['Kn']:.6f}")
        p_int = ks_res["p_interval"]
        if isinstance(p_int, float):
            crit_val = ks_res["critical_values"][p_int]
            print(f"Попадает в p-отрезок: p >= {p_int} (критическое Kn = {crit_val:.6f})")
        else:
            print(f"Попадает в p-отрезок: {p_int} (Kn выше всех табличных критических значений)")
        print()

        plot_empirical_vs_theoretical(ks_res["sorted_sample"], ks_res["Fn"], ks_res, part_num)
        results.append(ks_res)

    print("СВОДНЫЕ РЕЗУЛЬТАТЫ КС-ТЕСТА")
    print("=" * 50)
    kn_values = [r["Kn"] for r in results]
    print(f"Kn (по частям): {[f'{v:.6f}' for v in kn_values]}")
    print(f"Средний Kn: {np.mean(kn_values):.6f}")
    print()


def main():
    total_sample_size = 100000
    x0 = 42
    generator_type = "mix"
    num_parts = 3

    run_kolmogorov_smirnov_tests(generator_type, x0, total_sample_size * num_parts, num_parts)


main()