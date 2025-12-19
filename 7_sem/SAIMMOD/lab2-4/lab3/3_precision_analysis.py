# -*- coding: utf-8 -*-
"""
Оценка зависимости точности имитации от количества прогонов.
Определение минимального количества прогонов для достижения 5% погрешности отклика.

Относительная погрешность: δ = Δ / x̄ * 100%
где Δ - полуширина доверительного интервала, x̄ - среднее значение

Откликами являются:
1. Среднее время ожидания до оплаты (avg_wait_to_payment)
2. Пропускная способность системы (throughput)
"""

import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).parent.parent))

import numpy as np
import pandas as pd
from scipy import stats
import matplotlib.pyplot as plt
from gas_station_simulation import run_simulation, CONFIG

# Настройки
MAX_RUNS = 100  # Максимальное количество прогонов для анализа
SIMULATION_TIME = 10 * 60 * 60  # 10 часов
ALPHA = 0.05  # Уровень значимости
TARGET_PRECISION = 5.0  # Целевая погрешность в процентах

# Создание директории для выходных файлов
OUTPUT_DIR = Path("lab3_3")
OUTPUT_DIR.mkdir(exist_ok=True)


def calculate_precision(data, alpha=0.05):
    """
    Вычисляет точность (относительную погрешность) оценки.

    Args:
        data: массив наблюдений
        alpha: уровень значимости

    Returns:
        dict с оценками и погрешностью
    """
    n = len(data)
    if n < 2:
        return None

    mean = np.mean(data)
    std = np.std(data, ddof=1)
    se = std / np.sqrt(n)

    t_critical = stats.t.ppf(1 - alpha/2, df=n-1)
    margin_of_error = t_critical * se

    # Относительная погрешность в процентах
    relative_error = (margin_of_error / abs(mean)) * 100 if mean != 0 else float('inf')

    return {
        'n': n,
        'mean': mean,
        'std': std,
        'se': se,
        't_critical': t_critical,
        'margin_of_error': margin_of_error,
        'ci_lower': mean - margin_of_error,
        'ci_upper': mean + margin_of_error,
        'relative_error_percent': relative_error
    }


def run_all_simulations(max_runs=MAX_RUNS, simulation_time=SIMULATION_TIME):
    """
    Выполняет все симуляции и возвращает накопленные данные.
    """
    config = CONFIG.copy()
    config["simulation_time"] = simulation_time

    print(f"\n{'='*60}")
    print(f"Запуск {max_runs} симуляций для анализа точности")
    print(f"{'='*60}")

    avg_waits = []
    throughputs = []

    for i in range(max_runs):
        seed = 3000 + i
        print(f"Прогон {i+1}/{max_runs}...", end=" ")

        import io
        import sys
        old_stdout = sys.stdout
        sys.stdout = io.StringIO()

        try:
            summary = run_simulation(seed=seed, config=config, return_timeseries=False)
        finally:
            sys.stdout = old_stdout

        avg_waits.append(summary['avg_wait_to_payment_s'])
        throughputs.append(summary['throughput'])

        print(f"wait={summary['avg_wait_to_payment_s']:.1f}, thr={summary['throughput']}")

    return avg_waits, throughputs


def analyze_precision_vs_runs(data, response_name, alpha=ALPHA):
    """
    Анализирует зависимость точности от количества прогонов.
    """
    results = []

    for n in range(2, len(data) + 1):
        subset = data[:n]
        precision = calculate_precision(subset, alpha)
        if precision:
            results.append({
                'n': n,
                'mean': precision['mean'],
                'std': precision['std'],
                'margin_of_error': precision['margin_of_error'],
                'relative_error_percent': precision['relative_error_percent'],
                'ci_lower': precision['ci_lower'],
                'ci_upper': precision['ci_upper']
            })

    return pd.DataFrame(results)


def find_min_runs_for_precision(precision_df, target_precision=TARGET_PRECISION):
    """
    Находит минимальное количество прогонов для достижения целевой погрешности.
    """
    for _, row in precision_df.iterrows():
        if row['relative_error_percent'] <= target_precision:
            return int(row['n'])
    return None


def plot_precision_analysis(precision_data, filename=None):
    """
    Визуализация зависимости точности от количества прогонов.
    """
    if filename is None:
        filename = OUTPUT_DIR / 'precision_vs_runs.png'

    fig, axes = plt.subplots(2, 2, figsize=(14, 10))

    for idx, (response_name, df, min_runs) in enumerate(precision_data):
        # График 1: Относительная погрешность
        ax1 = axes[0, idx]
        ax1.plot(df['n'], df['relative_error_percent'], 'b-', linewidth=2, marker='o',
                 markersize=4, label='Относительная погрешность')
        ax1.axhline(y=TARGET_PRECISION, color='red', linestyle='--', linewidth=2,
                    label=f'Целевая погрешность ({TARGET_PRECISION}%)')

        if min_runs:
            ax1.axvline(x=min_runs, color='green', linestyle=':', linewidth=2,
                        label=f'Мин. прогонов: n={min_runs}')
            ax1.scatter([min_runs], [df[df['n']==min_runs]['relative_error_percent'].values[0]],
                        color='green', s=100, zorder=5)

        ax1.set_xlabel('Количество прогонов (n)', fontsize=11)
        ax1.set_ylabel('Относительная погрешность δ (%)', fontsize=11)
        ax1.set_title(f'{response_name}\nЗависимость погрешности от n', fontsize=11)
        ax1.legend(loc='upper right', fontsize=9)
        ax1.grid(True, alpha=0.3)
        ax1.set_xlim(0, len(df) + 2)
        ax1.set_ylim(0, max(df['relative_error_percent'].max() * 1.1, TARGET_PRECISION * 2))

        # График 2: Доверительный интервал
        ax2 = axes[1, idx]
        ax2.fill_between(df['n'], df['ci_lower'], df['ci_upper'],
                         alpha=0.3, color='steelblue', label='95% ДИ')
        ax2.plot(df['n'], df['mean'], 'b-', linewidth=2, label='Среднее')

        if min_runs:
            ax2.axvline(x=min_runs, color='green', linestyle=':', linewidth=2,
                        label=f'n={min_runs} (δ≤{TARGET_PRECISION}%)')

        ax2.set_xlabel('Количество прогонов (n)', fontsize=11)
        ax2.set_ylabel(response_name, fontsize=11)
        ax2.set_title(f'Сходимость доверительного интервала', fontsize=11)
        ax2.legend(loc='upper right', fontsize=9)
        ax2.grid(True, alpha=0.3)
        ax2.set_xlim(0, len(df) + 2)

    plt.suptitle(f'Зависимость точности имитации от количества прогонов\n'
                 f'(α={ALPHA}, целевая погрешность δ={TARGET_PRECISION}%)',
                 fontsize=13, fontweight='bold')
    plt.tight_layout()
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    plt.close()
    print(f"График сохранён: {filename}")


def main():
    """Основная функция."""

    print("\n" + "="*70)
    print("АНАЛИЗ ЗАВИСИМОСТИ ТОЧНОСТИ ОТ КОЛИЧЕСТВА ПРОГОНОВ")
    print(f"Уровень значимости: α = {ALPHA}")
    print(f"Целевая относительная погрешность: δ = {TARGET_PRECISION}%")
    print("="*70)

    # Выполняем симуляции
    avg_waits, throughputs = run_all_simulations()

    # Анализируем зависимость точности
    responses = [
        ('Среднее время ожидания (сек)', avg_waits),
        ('Пропускная способность (машин)', throughputs)
    ]

    precision_data = []
    all_results = []

    print("\n" + "="*70)
    print("РЕЗУЛЬТАТЫ АНАЛИЗА")
    print("="*70)

    for response_name, data in responses:
        print(f"\n{'-'*50}")
        print(f"Отклик: {response_name}")
        print(f"{'-'*50}")

        # Анализ точности
        precision_df = analyze_precision_vs_runs(data, response_name)

        # Находим минимальное n для целевой погрешности
        min_runs = find_min_runs_for_precision(precision_df, TARGET_PRECISION)

        print(f"\n  Зависимость погрешности от количества прогонов:")
        print(f"  {'n':>4} | {'Среднее':>12} | {'δ (%)':>10} | {'95% ДИ':>25}")
        print(f"  {'-'*4}-+-{'-'*12}-+-{'-'*10}-+-{'-'*25}")

        # Выводим ключевые точки
        key_points = [2, 5, 10, 15, 20, 25, 30, 40, 50]
        for n in key_points:
            if n <= len(precision_df):
                row = precision_df[precision_df['n'] == n].iloc[0]
                ci_str = f"[{row['ci_lower']:.2f}; {row['ci_upper']:.2f}]"
                marker = " <--" if row['relative_error_percent'] <= TARGET_PRECISION and \
                         (n == min_runs or precision_df[precision_df['n'] == n-1]['relative_error_percent'].values[0] > TARGET_PRECISION if n > 2 else True) else ""
                print(f"  {n:>4} | {row['mean']:>12.2f} | {row['relative_error_percent']:>10.2f} | {ci_str:>25}{marker}")

        if min_runs:
            print(f"\n  >>> Минимальное количество прогонов для δ ≤ {TARGET_PRECISION}%: n = {min_runs}")
            final_row = precision_df[precision_df['n'] == min_runs].iloc[0]
            print(f"      При n={min_runs}: δ = {final_row['relative_error_percent']:.2f}%")
        else:
            print(f"\n  >>> Целевая погрешность {TARGET_PRECISION}% не достигнута при n={MAX_RUNS}")
            final_row = precision_df.iloc[-1]
            print(f"      При n={MAX_RUNS}: δ = {final_row['relative_error_percent']:.2f}%")

            # Оценка необходимого n
            # δ = t * s / (√n * x̄) * 100 => n = (t * s * 100 / (δ * x̄))²
            # Приближённо при большом n: t ≈ 1.96
            estimated_n = int(np.ceil((1.96 * final_row['std'] * 100 / (TARGET_PRECISION * final_row['mean']))**2))
            print(f"      Оценка необходимого n: ~{estimated_n}")

        precision_data.append((response_name, precision_df, min_runs))

        all_results.append({
            'Отклик': response_name,
            'Мин. n для δ≤5%': min_runs if min_runs else f'>{MAX_RUNS}',
            'δ при n=20 (%)': precision_df[precision_df['n']==20]['relative_error_percent'].values[0] if len(precision_df) >= 20 else None,
            'δ при n=max (%)': precision_df.iloc[-1]['relative_error_percent'],
            'Финальное среднее': precision_df.iloc[-1]['mean'],
            'Финальное СКО': precision_df.iloc[-1]['std']
        })

    # Сохраняем детальные результаты
    for response_name, precision_df, _ in precision_data:
        safe_name = response_name.replace(' ', '_').replace('(', '').replace(')', '')
        precision_df.to_csv(OUTPUT_DIR / f'precision_analysis_{safe_name[:20]}.csv', index=False)

    # Итоговая таблица
    summary_df = pd.DataFrame(all_results)
    summary_df.to_csv(OUTPUT_DIR / 'precision_analysis_summary.csv', index=False, encoding='utf-8-sig')

    print("\n" + "="*70)
    print("ИТОГОВАЯ ТАБЛИЦА")
    print("="*70)
    print(summary_df.to_string(index=False))

    # Строим графики
    plot_precision_analysis(precision_data)

    print(f"\nРезультаты сохранены в CSV файлы")

    return summary_df


if __name__ == "__main__":
    main()

