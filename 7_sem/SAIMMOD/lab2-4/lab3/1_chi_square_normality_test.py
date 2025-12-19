# -*- coding: utf-8 -*-
"""
Проверка гипотезы о нормальности распределения откликов
при «длинных» повторных прогонах с использованием критерия хи-квадрат.

Методология:
- Выполняется N длинных прогонов симуляции
- Для каждого прогона вычисляется среднее значение отклика
- Проверяется нормальность распределения этих средних значений

Откликами являются (выбраны 2 согласно заданию):
1. Среднее время ожидания до оплаты (avg_wait_to_payment)
2. Пропускная способность системы (throughput - количество обслуженных машин)
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
NUM_RUNS = 100  # Количество повторных прогонов (достаточно для критерия хи-квадрат)
SIMULATION_TIME = 10 * 60 * 60  # 10 часов для "длинных" прогонов
ALPHA = 0.05  # Уровень значимости

# Создание директории для выходных файлов
OUTPUT_DIR = Path("lab3_1")
OUTPUT_DIR.mkdir(exist_ok=True)


def chi_square_normality_test(data, num_bins=None, alpha=0.05):
    """
    Критерий хи-квадрат для проверки нормальности распределения.

    Args:
        data: массив наблюдений
        num_bins: количество интервалов (если None, вычисляется автоматически)
        alpha: уровень значимости

    Returns:
        dict с результатами теста
    """
    n = len(data)
    if n < 30:
        return {
            'valid': False,
            'message': f'Недостаточно данных (n={n}, требуется >= 30)'
        }

    # Оценка параметров нормального распределения по выборке
    mu = np.mean(data)
    sigma = np.std(data, ddof=1)

    # Определение количества интервалов по правилу Стёрджеса
    if num_bins is None:
        num_bins = int(1 + 3.322 * np.log10(n))
        num_bins = max(5, min(num_bins, 20))  # от 5 до 20 интервалов

    # Разбиение на интервалы с равными вероятностями (квантили)
    # Это обеспечивает примерно равные ожидаемые частоты
    quantiles = np.linspace(0, 1, num_bins + 1)
    bin_edges = stats.norm.ppf(quantiles, loc=mu, scale=sigma)
    bin_edges[0] = -np.inf
    bin_edges[-1] = np.inf

    # Наблюдаемые частоты
    observed, _ = np.histogram(data, bins=bin_edges)

    # Ожидаемые частоты (равные для квантильного разбиения)
    expected = np.full(num_bins, n / num_bins)

    # Объединение интервалов с малыми ожидаемыми частотами (< 5)
    while True:
        min_exp = expected.min()
        if min_exp >= 5 or len(expected) <= 3:
            break
        # Находим индекс минимальной частоты и объединяем с соседним
        min_idx = np.argmin(expected)
        if min_idx == 0:
            observed[1] += observed[0]
            expected[1] += expected[0]
            observed = observed[1:]
            expected = expected[1:]
            bin_edges = np.concatenate([[bin_edges[0]], bin_edges[2:]])
        elif min_idx == len(expected) - 1:
            observed[-2] += observed[-1]
            expected[-2] += expected[-1]
            observed = observed[:-1]
            expected = expected[:-1]
            bin_edges = np.concatenate([bin_edges[:-2], [bin_edges[-1]]])
        else:
            if expected[min_idx - 1] < expected[min_idx + 1]:
                observed[min_idx - 1] += observed[min_idx]
                expected[min_idx - 1] += expected[min_idx]
            else:
                observed[min_idx + 1] += observed[min_idx]
                expected[min_idx + 1] += expected[min_idx]
            observed = np.delete(observed, min_idx)
            expected = np.delete(expected, min_idx)
            bin_edges = np.delete(bin_edges, min_idx + 1)

    # Число степеней свободы: k - 1 - p, где p = 2 (оценены mu и sigma)
    k = len(observed)
    df = k - 1 - 2

    if df <= 0:
        return {
            'valid': False,
            'message': f'Недостаточно степеней свободы (df={df})'
        }

    # Вычисление статистики хи-квадрат
    chi2_stat = np.sum((observed - expected) ** 2 / expected)

    # Критическое значение и p-value
    chi2_critical = stats.chi2.ppf(1 - alpha, df)
    p_value = 1 - stats.chi2.cdf(chi2_stat, df)

    # Решение
    reject_h0 = chi2_stat > chi2_critical

    return {
        'valid': True,
        'n': n,
        'num_bins': k,
        'df': df,
        'mu': mu,
        'sigma': sigma,
        'chi2_stat': chi2_stat,
        'chi2_critical': chi2_critical,
        'p_value': p_value,
        'alpha': alpha,
        'reject_h0': reject_h0,
        'conclusion': 'Отвергаем H0 (распределение НЕ нормальное)' if reject_h0
                      else 'Не отвергаем H0 (распределение может быть нормальным)',
        'observed': observed,
        'expected': expected,
        'bin_edges': bin_edges
    }


def run_long_simulations(num_runs=NUM_RUNS, simulation_time=SIMULATION_TIME):
    """
    Выполняет длинные повторные прогоны симуляции.

    Для каждого прогона собирает СРЕДНИЕ значения откликов.
    Это соответствует методологии проверки нормальности при повторных прогонах.

    Returns:
        DataFrame с метриками откликов для каждого прогона,
        списки средних значений двух откликов
    """
    results = []

    config = CONFIG.copy()
    config["simulation_time"] = simulation_time

    print(f"\n{'='*60}")
    print(f"Запуск {num_runs} длинных симуляций (по {simulation_time/3600:.1f} часов каждая)")
    print(f"{'='*60}")

    # Списки средних значений откликов по прогонам
    avg_waits_per_run = []
    throughput_per_run = []

    for i in range(num_runs):
        seed = 1000 + i  # Фиксированные seed для воспроизводимости
        if (i + 1) % 10 == 0 or i == 0:
            print(f"\nПрогон {i+1}/{num_runs} (seed={seed})...")

        # Подавляем вывод симулятора
        import io
        import sys
        old_stdout = sys.stdout
        sys.stdout = io.StringIO()

        try:
            summary = run_simulation(seed=seed, config=config, return_timeseries=False)
        finally:
            sys.stdout = old_stdout

        # Собираем СРЕДНИЕ значения откликов для этого прогона
        avg_wait = summary['avg_wait_to_payment_s']
        throughput = summary['throughput']

        avg_waits_per_run.append(avg_wait)
        throughput_per_run.append(throughput)

        results.append({
            'run': i + 1,
            'seed': seed,
            'throughput': throughput,
            'avg_wait_to_payment': avg_wait,
        })

        if (i + 1) % 10 == 0 or i == 0:
            print(f"  avg_wait={avg_wait:.2f}s, throughput={throughput}")

    results_df = pd.DataFrame(results)
    print(f"\nВсего выполнено прогонов: {num_runs}")

    return results_df, avg_waits_per_run, throughput_per_run


def plot_chi_square_results(data, test_result, response_name, filename):
    """
    Визуализация результатов критерия хи-квадрат.
    """
    if not test_result['valid']:
        print(f"  Невозможно построить график: {test_result['message']}")
        return

    fig, axes = plt.subplots(1, 2, figsize=(14, 5))

    # 1. Гистограмма с теоретической нормальной кривой
    ax1 = axes[0]

    # Используем оптимальное число интервалов для нормального вида
    n_bins = int(np.sqrt(len(data)))  # Правило квадратного корня
    n_bins = max(8, min(n_bins, 12))  # Ограничиваем 8-12 интервалов

    counts, bins, patches = ax1.hist(data, bins=n_bins, density=True, alpha=0.7,
                                      color='steelblue', edgecolor='black', label='Эмпирическое')

    # Теоретическая нормальная кривая
    x = np.linspace(test_result['mu'] - 4*test_result['sigma'],
                    test_result['mu'] + 4*test_result['sigma'], 300)
    y = stats.norm.pdf(x, test_result['mu'], test_result['sigma'])
    ax1.plot(x, y, 'r-', lw=2.5, label=f'Теор. N(μ={test_result["mu"]:.1f}, σ={test_result["sigma"]:.1f})')

    # Ограничиваем ось X для лучшего вида
    ax1.set_xlim(test_result['mu'] - 3.5*test_result['sigma'],
                 test_result['mu'] + 3.5*test_result['sigma'])

    ax1.set_xlabel(response_name, fontsize=11)
    ax1.set_ylabel('Плотность вероятности', fontsize=11)
    ax1.set_title(f'Гистограмма распределения', fontsize=12)
    ax1.legend(loc='upper right')
    ax1.grid(True, alpha=0.3)

    # 2. Столбчатая диаграмма наблюдаемых vs ожидаемых частот
    ax2 = axes[1]
    x_pos = np.arange(len(test_result['observed']))
    width = 0.35
    bars1 = ax2.bar(x_pos - width/2, test_result['observed'], width,
                    label='Наблюдаемые', alpha=0.8, color='steelblue', edgecolor='black')
    bars2 = ax2.bar(x_pos + width/2, test_result['expected'], width,
                    label='Ожидаемые (теор.)', alpha=0.8, color='salmon', edgecolor='black')
    ax2.set_xlabel('Интервал', fontsize=11)
    ax2.set_ylabel('Частота', fontsize=11)
    ax2.set_title(f'Сравнение частот: χ²={test_result["chi2_stat"]:.2f}, p-value={test_result["p_value"]:.4f}', fontsize=12)
    ax2.legend(loc='upper right')
    ax2.set_xticks(x_pos)
    ax2.set_xticklabels([f'{i+1}' for i in x_pos])
    ax2.grid(True, alpha=0.3, axis='y')

    # Добавляем текстовую информацию
    info_text = (f'n = {test_result["n"]}\n'
                 f'μ = {test_result["mu"]:.2f}\n'
                 f'σ = {test_result["sigma"]:.2f}\n'
                 f'df = {test_result["df"]}\n'
                 f'χ² = {test_result["chi2_stat"]:.3f}\n'
                 f'χ²крит = {test_result["chi2_critical"]:.3f}\n'
                 f'p-value = {test_result["p_value"]:.4f}\n'
                 f'α = {test_result["alpha"]}')

    ax2.text(1.02, 0.5, info_text, transform=ax2.transAxes, fontsize=10,
             verticalalignment='center', bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.8))

    # Определяем цвет заголовка по результату теста
    conclusion_color = 'green' if not test_result['reject_h0'] else 'red'
    plt.suptitle(f'Проверка нормальности: {response_name}\n{test_result["conclusion"]}',
                 fontsize=12, fontweight='bold', color=conclusion_color)
    plt.tight_layout()
    plt.savefig(filename, dpi=150, bbox_inches='tight')
    plt.close()
    print(f"  График сохранён: {filename}")


def main():
    """Основная функция для запуска анализа."""

    print("\n" + "="*70)
    print("ПРОВЕРКА НОРМАЛЬНОСТИ РАСПРЕДЕЛЕНИЯ ОТКЛИКОВ")
    print("Метод: критерий хи-квадрат Пирсона")
    print("Методология: проверка средних значений по длинным прогонам")
    print("="*70)

    # Запускаем длинные симуляции
    results_df, avg_waits, throughputs = run_long_simulations()

    # Сохраняем сводные результаты прогонов
    results_df.to_csv(OUTPUT_DIR / 'chi_square_runs_summary.csv', index=False)
    print(f"\nСводка прогонов сохранена: {OUTPUT_DIR / 'chi_square_runs_summary.csv'}")

    print("\n" + "="*70)
    print("РЕЗУЛЬТАТЫ КРИТЕРИЯ ХИ-КВАДРАТ")
    print("="*70)

    # Список откликов для тестирования (2 отклика согласно заданию)
    responses = [
        ('Среднее время ожидания до оплаты (сек)', avg_waits, 'avg_wait_to_payment'),
        ('Пропускная способность (машин)', throughputs, 'throughput')
    ]

    # Результаты всех тестов
    test_results = []

    for response_name, data, file_prefix in responses:
        print(f"\n{'-'*50}")
        print(f"Отклик: {response_name}")
        print(f"{'-'*50}")

        data = np.array(data)
        print(f"  Количество прогонов (наблюдений): {len(data)}")

        if len(data) > 0:
            print(f"  Среднее: {np.mean(data):.2f}")
            print(f"  Стд. откл.: {np.std(data, ddof=1):.2f}")
            print(f"  Мин: {np.min(data):.2f}, Макс: {np.max(data):.2f}")

        # Выполняем тест хи-квадрат
        result = chi_square_normality_test(data, alpha=ALPHA)

        if result['valid']:
            print(f"\n  Результат критерия хи-квадрат:")
            print(f"    - Число интервалов: {result['num_bins']}")
            print(f"    - Степени свободы: {result['df']}")
            print(f"    - Chi^2 статистика: {result['chi2_stat']:.3f}")
            print(f"    - Chi^2 критическое (alpha={ALPHA}): {result['chi2_critical']:.3f}")
            print(f"    - p-value: {result['p_value']:.4f}")
            print(f"\n  ВЫВОД: {result['conclusion']}")

            # Строим графики
            plot_chi_square_results(data, result, response_name,
                                   OUTPUT_DIR / f'chi_square_{file_prefix}.png')

            test_results.append({
                'Отклик': response_name,
                'n': result['n'],
                'μ': result['mu'],
                'σ': result['sigma'],
                'Число интервалов': result['num_bins'],
                'df': result['df'],
                'χ² статистика': result['chi2_stat'],
                'χ² критическое': result['chi2_critical'],
                'p-value': result['p_value'],
                'α': ALPHA,
                'Гипотеза отвергнута': 'Да' if result['reject_h0'] else 'Нет',
                'Вывод': result['conclusion']
            })
        else:
            print(f"\n  ОШИБКА: {result['message']}")
            test_results.append({
                'Отклик': response_name,
                'Вывод': result['message']
            })

    # Сохраняем итоговую таблицу результатов
    test_results_df = pd.DataFrame(test_results)
    test_results_df.to_csv(OUTPUT_DIR / 'chi_square_test_results.csv', index=False, encoding='utf-8-sig')
    print(f"\n{'='*70}")
    print(f"Результаты тестов сохранены: {OUTPUT_DIR / 'chi_square_test_results.csv'}")

    # Выводим итоговую таблицу
    print("\n" + "="*70)
    print("ИТОГОВАЯ ТАБЛИЦА РЕЗУЛЬТАТОВ")
    print("="*70)
    print(test_results_df.to_string(index=False))

    # Создаем сводный график
    create_summary_plot(responses, test_results)

    return test_results_df


def create_summary_plot(responses, test_results):
    """Создаёт сводный график всех тестов."""

    fig, axes = plt.subplots(1, 2, figsize=(14, 5))

    for idx, (response_name, data, file_prefix) in enumerate(responses):
        if len(data) == 0:
            continue

        data = np.array(data)
        mu, sigma = np.mean(data), np.std(data, ddof=1)

        # Гистограмма
        ax = axes[idx]
        n_bins = int(np.sqrt(len(data)))
        n_bins = max(8, min(n_bins, 12))

        ax.hist(data, bins=n_bins, density=True, alpha=0.7, color='steelblue', edgecolor='black',
                label='Эмпирическое')

        x = np.linspace(mu - 4*sigma, mu + 4*sigma, 300)
        ax.plot(x, stats.norm.pdf(x, mu, sigma), 'r-', lw=2.5,
                label=f'Теор. N(μ={mu:.1f}, σ={sigma:.1f})')

        ax.set_xlim(mu - 3.5*sigma, mu + 3.5*sigma)
        ax.set_title(response_name, fontsize=11)
        ax.set_xlabel('Значение', fontsize=10)
        ax.set_ylabel('Плотность', fontsize=10)
        ax.legend(loc='upper right')
        ax.grid(True, alpha=0.3)


    plt.suptitle('Проверка нормальности откликов (критерий хи-квадрат)\nСредние значения по длинным прогонам',
                 fontsize=14, fontweight='bold')
    plt.tight_layout()
    plt.savefig(OUTPUT_DIR / 'chi_square_summary.png', dpi=150, bbox_inches='tight')
    plt.close()
    print(f"Сводный график сохранён: {OUTPUT_DIR / 'chi_square_summary.png'}")


if __name__ == "__main__":
    main()

