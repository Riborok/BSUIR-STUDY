# -*- coding: utf-8 -*-
"""
Вычисление точечных и интервальных оценок откликов ИМ
в опыте из 20 повторных прогонов при уровне значимости 0.05.

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
NUM_RUNS = 20  # Количество повторных прогонов
SIMULATION_TIME = 10 * 60 * 60  # 10 часов для длинных прогонов
ALPHA = 0.05  # Уровень значимости

# Создание директории для выходных файлов
OUTPUT_DIR = Path("lab3_2")
OUTPUT_DIR.mkdir(exist_ok=True)


def calculate_confidence_interval(data, alpha=0.05):
    """
    Вычисляет точечные и интервальные оценки для выборки.

    Использует t-распределение Стьюдента для построения
    доверительного интервала (так как σ неизвестна и n < 30).

    Args:
        data: массив наблюдений
        alpha: уровень значимости

    Returns:
        dict с точечными и интервальными оценками
    """
    n = len(data)

    # Точечные оценки
    mean = np.mean(data)  # Выборочное среднее (точечная оценка мат. ожидания)
    std = np.std(data, ddof=1)  # Выборочное СКО (несмещённая оценка)
    var = np.var(data, ddof=1)  # Выборочная дисперсия (несмещённая оценка)

    # Стандартная ошибка среднего
    se = std / np.sqrt(n)

    # Критическое значение t-распределения
    t_critical = stats.t.ppf(1 - alpha/2, df=n-1)

    # Доверительный интервал для среднего
    margin_of_error = t_critical * se
    ci_lower = mean - margin_of_error
    ci_upper = mean + margin_of_error

    # Доверительный интервал для дисперсии (хи-квадрат)
    chi2_lower = stats.chi2.ppf(alpha/2, df=n-1)
    chi2_upper = stats.chi2.ppf(1 - alpha/2, df=n-1)
    var_ci_lower = (n - 1) * var / chi2_upper
    var_ci_upper = (n - 1) * var / chi2_lower

    return {
        'n': n,
        'mean': mean,
        'std': std,
        'var': var,
        'se': se,
        't_critical': t_critical,
        'margin_of_error': margin_of_error,
        'ci_lower': ci_lower,
        'ci_upper': ci_upper,
        'ci_width': ci_upper - ci_lower,
        'var_ci_lower': var_ci_lower,
        'var_ci_upper': var_ci_upper,
        'confidence_level': 1 - alpha,
        'alpha': alpha
    }


def run_simulations(num_runs=NUM_RUNS, simulation_time=SIMULATION_TIME):
    """
    Выполняет повторные прогоны симуляции.

    Returns:
        DataFrame с результатами прогонов,
        списки значений двух откликов
    """
    results = []

    config = CONFIG.copy()
    config["simulation_time"] = simulation_time

    print(f"\n{'='*60}")
    print(f"Запуск {num_runs} симуляций (по {simulation_time/3600:.1f} часов каждая)")
    print(f"{'='*60}")

    avg_waits = []
    throughputs = []

    for i in range(num_runs):
        seed = 2000 + i  # Фиксированные seed для воспроизводимости
        print(f"Прогон {i+1}/{num_runs} (seed={seed})...", end=" ")

        # Подавляем вывод симулятора
        import io
        import sys
        old_stdout = sys.stdout
        sys.stdout = io.StringIO()

        try:
            summary = run_simulation(seed=seed, config=config, return_timeseries=False)
        finally:
            sys.stdout = old_stdout

        avg_wait = summary['avg_wait_to_payment_s']
        throughput = summary['throughput']

        avg_waits.append(avg_wait)
        throughputs.append(throughput)

        results.append({
            'run': i + 1,
            'seed': seed,
            'avg_wait_to_payment': avg_wait,
            'throughput': throughput
        })

        print(f"avg_wait={avg_wait:.2f}s, throughput={throughput}")

    results_df = pd.DataFrame(results)
    print(f"\nВсего выполнено прогонов: {num_runs}")

    return results_df, avg_waits, throughputs


def plot_confidence_intervals(responses_data, filename=None):
    """
    Визуализация доверительных интервалов.
    """
    if filename is None:
        filename = OUTPUT_DIR / 'confidence_intervals.png'

    fig, axes = plt.subplots(1, 2, figsize=(14, 6))

    for idx, (response_name, data, stats_result) in enumerate(responses_data):
        ax = axes[idx]
        data = np.array(data)

        # Scatter plot данных
        x = np.arange(1, len(data) + 1)
        ax.scatter(x, data, color='steelblue', s=60, alpha=0.7, label='Наблюдения', zorder=3)

        # Линия среднего
        ax.axhline(y=stats_result['mean'], color='red', linestyle='-', linewidth=2,
                   label=f'Среднее = {stats_result["mean"]:.2f}')

        # Доверительный интервал
        ax.axhline(y=stats_result['ci_lower'], color='green', linestyle='--', linewidth=1.5)
        ax.axhline(y=stats_result['ci_upper'], color='green', linestyle='--', linewidth=1.5)
        ax.fill_between([0, len(data) + 1], stats_result['ci_lower'], stats_result['ci_upper'],
                        color='green', alpha=0.15, label=f'95% ДИ: [{stats_result["ci_lower"]:.2f}, {stats_result["ci_upper"]:.2f}]')

        ax.set_xlabel('Номер прогона', fontsize=11)
        ax.set_ylabel(response_name, fontsize=11)
        ax.set_title(f'{response_name}\nТочечная оценка и 95% доверительный интервал', fontsize=11)
        ax.legend(loc='best', fontsize=9)
        ax.grid(True, alpha=0.3)
        ax.set_xlim(0, len(data) + 1)

        # Добавляем текст с информацией
        info_text = (f'n = {stats_result["n"]}\n'
                     f'x̄ = {stats_result["mean"]:.2f}\n'
                     f's = {stats_result["std"]:.2f}\n'
                     f'SE = {stats_result["se"]:.2f}\n'
                     f't_крит = {stats_result["t_critical"]:.3f}\n'
                     f'Δ = {stats_result["margin_of_error"]:.2f}')

        ax.text(0.98, 0.02, info_text, transform=ax.transAxes, fontsize=9,
                verticalalignment='bottom', horizontalalignment='right',
                bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.8))

    plt.suptitle(f'Точечные и интервальные оценки откликов\n(n={NUM_RUNS} прогонов, α={ALPHA})',
                 fontsize=13, fontweight='bold')
    plt.tight_layout()
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    plt.close()
    print(f"График сохранён: {filename}")


def main():
    """Основная функция."""

    print("\n" + "="*70)
    print("ТОЧЕЧНЫЕ И ИНТЕРВАЛЬНЫЕ ОЦЕНКИ ОТКЛИКОВ")
    print(f"Количество прогонов: n = {NUM_RUNS}")
    print(f"Уровень значимости: α = {ALPHA}")
    print(f"Доверительная вероятность: 1-α = {1-ALPHA}")
    print("="*70)

    # Запускаем симуляции
    results_df, avg_waits, throughputs = run_simulations()

    # Сохраняем результаты прогонов
    results_df.to_csv(OUTPUT_DIR / 'confidence_intervals_runs.csv', index=False)
    print(f"\nДанные прогонов сохранены: {OUTPUT_DIR / 'confidence_intervals_runs.csv'}")

    print("\n" + "="*70)
    print("РЕЗУЛЬТАТЫ ОЦЕНИВАНИЯ")
    print("="*70)

    # Список откликов
    responses = [
        ('Среднее время ожидания (сек)', avg_waits),
        ('Пропускная способность (машин)', throughputs)
    ]

    all_results = []
    responses_data = []

    for response_name, data in responses:
        print(f"\n{'-'*50}")
        print(f"Отклик: {response_name}")
        print(f"{'-'*50}")

        data = np.array(data)
        result = calculate_confidence_interval(data, alpha=ALPHA)

        print(f"\n  ТОЧЕЧНЫЕ ОЦЕНКИ:")
        print(f"    Выборочное среднее (x̄):     {result['mean']:.4f}")
        print(f"    Выборочное СКО (s):         {result['std']:.4f}")
        print(f"    Выборочная дисперсия (s²):  {result['var']:.4f}")

        print(f"\n  ИНТЕРВАЛЬНЫЕ ОЦЕНКИ (доверительный уровень {(1-ALPHA)*100:.0f}%):")
        print(f"    Стандартная ошибка (SE):    {result['se']:.4f}")
        print(f"    t-критическое (df={result['n']-1}):     {result['t_critical']:.4f}")
        print(f"    Погрешность (Δ):            {result['margin_of_error']:.4f}")
        print(f"\n    Доверительный интервал для среднего:")
        print(f"      [{result['ci_lower']:.4f}; {result['ci_upper']:.4f}]")
        print(f"      Ширина интервала: {result['ci_width']:.4f}")

        print(f"\n    Доверительный интервал для дисперсии:")
        print(f"      [{result['var_ci_lower']:.4f}; {result['var_ci_upper']:.4f}]")

        all_results.append({
            'Отклик': response_name,
            'n': result['n'],
            'Среднее (x̄)': result['mean'],
            'СКО (s)': result['std'],
            'Дисперсия (s²)': result['var'],
            'Станд. ошибка (SE)': result['se'],
            't-критическое': result['t_critical'],
            'Погрешность (Δ)': result['margin_of_error'],
            'ДИ нижняя граница': result['ci_lower'],
            'ДИ верхняя граница': result['ci_upper'],
            'Ширина ДИ': result['ci_width'],
            'ДИ дисперсии нижн.': result['var_ci_lower'],
            'ДИ дисперсии верхн.': result['var_ci_upper'],
            'α': ALPHA
        })

        responses_data.append((response_name, data, result))

    # Сохраняем результаты в CSV
    results_summary = pd.DataFrame(all_results)
    results_summary.to_csv(OUTPUT_DIR / 'confidence_intervals_results.csv', index=False, encoding='utf-8-sig')
    print(f"\n{'='*70}")
    print(f"Результаты сохранены: {OUTPUT_DIR / 'confidence_intervals_results.csv'}")

    # Выводим итоговую таблицу
    print("\n" + "="*70)
    print("ИТОГОВАЯ ТАБЛИЦА")
    print("="*70)

    # Компактный вывод
    summary_table = pd.DataFrame({
        'Отклик': [r['Отклик'] for r in all_results],
        'x̄': [f"{r['Среднее (x̄)']:.2f}" for r in all_results],
        's': [f"{r['СКО (s)']:.2f}" for r in all_results],
        '95% ДИ': [f"[{r['ДИ нижняя граница']:.2f}; {r['ДИ верхняя граница']:.2f}]" for r in all_results],
        'Ширина ДИ': [f"{r['Ширина ДИ']:.2f}" for r in all_results]
    })
    print(summary_table.to_string(index=False))

    # Строим графики
    plot_confidence_intervals(responses_data)

    return results_summary


if __name__ == "__main__":
    main()

