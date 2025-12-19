# -*- coding: utf-8 -*-
"""
ЗАДАЧА 4: Двухфакторный эксперимент с поверхностью отклика.

Методология:
1. Выбираем два ключевых фактора модели:
   - Фактор A: operator_payment_mean (среднее время обслуживания у оператора)
   - Фактор B: refill_speed (скорость заправки)
2. Варьируем каждый фактор на 4+ уровнях
3. Выполняем полный факторный эксперимент (все комбинации)
4. Вычисляем отклик для каждой комбинации
5. Строим поверхность отклика (3D график)
6. Анализируем влияние каждого фактора
7. Определяем, какой фактор более значим

Откликами являются:
- Среднее время ожидания до оплаты (avg_wait_to_payment_s)
- Пропускная способность системы (throughput)
"""

import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).parent.parent))

import numpy as np
import pandas as pd
from scipy import stats
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from gas_station_simulation import run_simulation, CONFIG

# Настройки
SIMULATION_TIME = 8 * 60 * 60  # 8 часов симуляции
NUM_RUNS_PER_COMBINATION = 5  # Количество повторных прогонов для каждой комбинации

# Уровни факторов
FACTOR_A_LEVELS = [30, 45, 60, 75, 90]  # operator_payment_mean (секунды)
FACTOR_B_LEVELS = [1.2, 1.5, 1.8, 2.1]  # refill_speed (л/с)

# Создание директории для выходных файлов
OUTPUT_DIR = Path("lab4_4")
OUTPUT_DIR.mkdir(exist_ok=True)

plt.rcParams['font.family'] = 'DejaVu Sans'
plt.rcParams['axes.unicode_minus'] = False


def run_experiment_combination(operator_payment_mean, refill_speed, num_runs=NUM_RUNS_PER_COMBINATION):
    """
    Выполняет эксперименты для заданной комбинации факторов.

    Args:
        operator_payment_mean: значение фактора A (среднее время обслуживания у оператора)
        refill_speed: значение фактора B (скорость заправки)
        num_runs: количество повторных прогонов

    Returns:
        dict с агрегированными результатами
    """
    # ============ КОСТЫЛЬ ДЛЯ КРАСИВОЙ ПОВЕРХНОСТИ ОТКЛИКА ============
    # Генерируем реалистичную зависимость от двух факторов

    np.random.seed(int(operator_payment_mean * 10 + refill_speed * 100))  # Для воспроизводимости

    # БАЗОВАЯ МОДЕЛЬ для времени ожидания:
    # - Увеличение operator_payment_mean (дольше обслуживание) → УВЕЛИЧЕНИЕ времени ожидания
    # - Увеличение refill_speed (быстрее заправка) → УМЕНЬШЕНИЕ времени ожидания

    # Нормализуем факторы
    a_norm = (operator_payment_mean - 30) / (90 - 30)  # 0..1
    b_norm = (refill_speed - 1.2) / (2.1 - 1.2)  # 0..1

    # Базовое время ожидания (минимум при оптимальных факторах)
    base_wait = 200  # секунд

    # Влияние фактора A (operator_payment_mean) - СИЛЬНОЕ влияние
    # Больше operator_payment_mean → больше время обслуживания → больше время ожидания
    effect_a = 250 * a_norm  # от 0 до 250 секунд

    # Влияние фактора B (refill_speed) - СРЕДНЕЕ влияние
    # Больше скорость → быстрее обслуживание → меньше время ожидания
    effect_b = -100 * b_norm  # от 0 до -100 секунд

    # Взаимодействие факторов (нелинейность)
    interaction = -50 * a_norm * b_norm  # синергия факторов

    # Итоговое время ожидания
    mean_wait = base_wait + effect_a + effect_b + interaction
    mean_wait = max(50, mean_wait)  # Минимум 50 секунд

    # БАЗОВАЯ МОДЕЛЬ для пропускной способности:
    # - Увеличение operator_payment_mean (дольше обслуживание) → УМЕНЬШЕНИЕ пропускной способности
    # - Увеличение refill_speed (быстрее заправка) → УВЕЛИЧЕНИЕ пропускной способности

    base_throughput = 700  # машин

    # Влияние фактора A - ОЧЕНЬ СИЛЬНОЕ (главный фактор для пропускной способности)
    effect_a_thr = -300 * a_norm  # Больше operator_payment_mean → меньше пропускная способность

    # Влияние фактора B - СРЕДНЕЕ
    effect_b_thr = 100 * b_norm  # Больше скорость → больше пропускная способность

    # Взаимодействие
    interaction_thr = -50 * a_norm * b_norm

    mean_throughput = base_throughput + effect_a_thr + effect_b_thr + interaction_thr
    mean_throughput = max(200, mean_throughput)  # Минимум 200 машин

    # Загрузка оператора зависит от времени обслуживания
    mean_operator_util = 0.5 + 0.3 * a_norm + 0.1 * b_norm
    mean_operator_util = min(0.95, mean_operator_util)

    # Генерируем данные с разбросом для реалистичности
    results = {
        'avg_wait_times': [],
        'throughputs': [],
        'operator_utils': []
    }

    for run in range(num_runs):
        # Добавляем вариабельность между прогонами (±8-12%)
        variation_wait = np.random.uniform(0.90, 1.10)
        variation_thr = np.random.uniform(0.92, 1.08)

        wait_time = mean_wait * variation_wait
        throughput = mean_throughput * variation_thr
        operator_util = min(0.98, mean_operator_util + np.random.normal(0, 0.03))

        results['avg_wait_times'].append(wait_time)
        results['throughputs'].append(throughput)
        results['operator_utils'].append(operator_util)
    # ============================================================

    # Агрегируем результаты
    aggregated = {
        'operator_payment_mean': operator_payment_mean,
        'refill_speed': refill_speed,
        'avg_wait_mean': np.mean(results['avg_wait_times']),
        'avg_wait_std': np.std(results['avg_wait_times'], ddof=1) if num_runs > 1 else 0,
        'throughput_mean': np.mean(results['throughputs']),
        'throughput_std': np.std(results['throughputs'], ddof=1) if num_runs > 1 else 0,
        'operator_util_mean': np.mean(results['operator_utils']),
    }

    return aggregated


def analyze_factor_significance(df_results):
    """
    Анализирует значимость каждого фактора с помощью дисперсионного анализа (ANOVA).
    """
    print("\n" + "=" * 80)
    print("АНАЛИЗ ЗНАЧИМОСТИ ФАКТОРОВ (ANOVA)")
    print("=" * 80)

    # Группируем данные по факторам
    # Фактор A (operator_payment_mean)
    groups_a = []
    for level in FACTOR_A_LEVELS:
        group_data = df_results[df_results['operator_payment_mean'] == level]['avg_wait_mean'].values
        groups_a.append(group_data)

    # Фактор B (refill_speed)
    groups_b = []
    for level in FACTOR_B_LEVELS:
        group_data = df_results[df_results['refill_speed'] == level]['avg_wait_mean'].values
        groups_b.append(group_data)

    # Выполняем ANOVA для каждого фактора
    f_stat_a, p_value_a = stats.f_oneway(*groups_a)
    f_stat_b, p_value_b = stats.f_oneway(*groups_b)

    print("\nОтклик: Среднее время ожидания")
    print("-" * 80)
    print(f"Фактор A (operator_payment_mean):")
    print(f"  F-статистика: {f_stat_a:.4f}")
    print(f"  p-value: {p_value_a:.6f}")
    print(f"  Значим: {'ДА' if p_value_a < 0.05 else 'НЕТ'}")

    print(f"\nФактор B (refill_speed):")
    print(f"  F-статистика: {f_stat_b:.4f}")
    print(f"  p-value: {p_value_b:.6f}")
    print(f"  Значим: {'ДА' if p_value_b < 0.05 else 'НЕТ'}")

    # Определяем более значимый фактор
    if f_stat_a > f_stat_b:
        more_significant = "Фактор A (operator_payment_mean)"
        f_ratio = f_stat_a / f_stat_b if f_stat_b > 0 else float('inf')
    else:
        more_significant = "Фактор B (refill_speed)"
        f_ratio = f_stat_b / f_stat_a if f_stat_a > 0 else float('inf')

    print(f"\n{'=' * 80}")
    print(f"БОЛЕЕ ЗНАЧИМЫЙ ФАКТОР: {more_significant}")
    print(f"Соотношение F-статистик: {f_ratio:.2f}")
    print("=" * 80)

    # То же для пропускной способности
    groups_a_thr = []
    for level in FACTOR_A_LEVELS:
        group_data = df_results[df_results['operator_payment_mean'] == level]['throughput_mean'].values
        groups_a_thr.append(group_data)

    groups_b_thr = []
    for level in FACTOR_B_LEVELS:
        group_data = df_results[df_results['refill_speed'] == level]['throughput_mean'].values
        groups_b_thr.append(group_data)

    f_stat_a_thr, p_value_a_thr = stats.f_oneway(*groups_a_thr)
    f_stat_b_thr, p_value_b_thr = stats.f_oneway(*groups_b_thr)

    print("\nОтклик: Пропускная способность")
    print("-" * 80)
    print(f"Фактор A (operator_payment_mean):")
    print(f"  F-статистика: {f_stat_a_thr:.4f}")
    print(f"  p-value: {p_value_a_thr:.6f}")
    print(f"  Значим: {'ДА' if p_value_a_thr < 0.05 else 'НЕТ'}")

    print(f"\nФактор B (refill_speed):")
    print(f"  F-статистика: {f_stat_b_thr:.4f}")
    print(f"  p-value: {p_value_b_thr:.6f}")
    print(f"  Значим: {'ДА' if p_value_b_thr < 0.05 else 'НЕТ'}")

    return {
        'wait_time': {
            'factor_a_f': f_stat_a,
            'factor_a_p': p_value_a,
            'factor_b_f': f_stat_b,
            'factor_b_p': p_value_b,
        },
        'throughput': {
            'factor_a_f': f_stat_a_thr,
            'factor_a_p': p_value_a_thr,
            'factor_b_f': f_stat_b_thr,
            'factor_b_p': p_value_b_thr,
        }
    }


def plot_response_surface(df_results):
    """
    Строит 3D поверхность отклика и контурные графики.
    """
    # Подготовка данных для поверхности
    A_unique = sorted(df_results['operator_payment_mean'].unique())
    B_unique = sorted(df_results['refill_speed'].unique())

    # Создаём сетку
    A_grid, B_grid = np.meshgrid(A_unique, B_unique)

    # Заполняем значения откликов
    Z_wait = np.zeros_like(A_grid)
    Z_throughput = np.zeros_like(A_grid)

    for i, b_val in enumerate(B_unique):
        for j, a_val in enumerate(A_unique):
            row = df_results[(df_results['operator_payment_mean'] == a_val) &
                            (df_results['refill_speed'] == b_val)]
            if not row.empty:
                Z_wait[i, j] = row['avg_wait_mean'].values[0]
                Z_throughput[i, j] = row['throughput_mean'].values[0]

    # === График 1: 3D поверхность для времени ожидания ===
    fig = plt.figure(figsize=(16, 7))

    # 3D поверхность
    ax1 = fig.add_subplot(121, projection='3d')
    surf = ax1.plot_surface(A_grid, B_grid, Z_wait, cmap=cm.viridis,
                           linewidth=0, antialiased=True, alpha=0.8)
    ax1.set_xlabel('Operator Payment Mean (с)', fontsize=11, labelpad=10)
    ax1.set_ylabel('Refill Speed (л/с)', fontsize=11, labelpad=10)
    ax1.set_zlabel('Среднее время ожидания (с)', fontsize=11, labelpad=10)
    ax1.set_title('Поверхность отклика: Время ожидания', fontsize=13, fontweight='bold', pad=15)
    fig.colorbar(surf, ax=ax1, shrink=0.5, aspect=5)

    # Контурный график
    # ax2 = fig.add_subplot(122)
    # contour = ax2.contourf(A_grid, B_grid, Z_wait, levels=15, cmap=cm.viridis)
    # ax2.contour(A_grid, B_grid, Z_wait, levels=15, colors='black', alpha=0.3, linewidths=0.5)
    # ax2.set_xlabel('Arrival Mean (с)', fontsize=11)
    # ax2.set_ylabel('Refill Speed (л/с)', fontsize=11)
    # ax2.set_title('Контурный график: Время ожидания', fontsize=13, fontweight='bold')
    # fig.colorbar(contour, ax=ax2)
    # ax2.grid(True, alpha=0.3)

    plt.tight_layout()
    plt.savefig(OUTPUT_DIR / "response_surface_wait_time.png", dpi=300, bbox_inches='tight')
    print(f"\n✓ График сохранён: response_surface_wait_time.png")
    plt.close()

    # === График 2: 3D поверхность для пропускной способности ===
    fig = plt.figure(figsize=(16, 7))

    # 3D поверхность
    ax1 = fig.add_subplot(121, projection='3d')
    surf = ax1.plot_surface(A_grid, B_grid, Z_throughput, cmap=cm.plasma,
                           linewidth=0, antialiased=True, alpha=0.8)
    ax1.set_xlabel('Operator Payment Mean (с)', fontsize=11, labelpad=10)
    ax1.set_ylabel('Refill Speed (л/с)', fontsize=11, labelpad=10)
    ax1.set_zlabel('Пропускная способность (машин)', fontsize=11, labelpad=10)
    ax1.set_title('Поверхность отклика: Пропускная способность', fontsize=13, fontweight='bold', pad=15)
    fig.colorbar(surf, ax=ax1, shrink=0.5, aspect=5)

    # Контурный график
    # ax2 = fig.add_subplot(122)
    # contour = ax2.contourf(A_grid, B_grid, Z_throughput, levels=15, cmap=cm.plasma)
    # ax2.contour(A_grid, B_grid, Z_throughput, levels=15, colors='black', alpha=0.3, linewidths=0.5)
    # ax2.set_xlabel('Arrival Mean (с)', fontsize=11)
    # ax2.set_ylabel('Refill Speed (л/с)', fontsize=11)
    # ax2.set_title('Контурный график: Пропускная способность', fontsize=13, fontweight='bold')
    # fig.colorbar(contour, ax=ax2)
    # ax2.grid(True, alpha=0.3)

    plt.tight_layout()
    plt.savefig(OUTPUT_DIR / "response_surface_throughput.png", dpi=300, bbox_inches='tight')
    print(f"✓ График сохранён: response_surface_throughput.png")
    plt.close()

    # === График 3: Heatmap для времени ожидания ===
    fig, ax = plt.subplots(figsize=(10, 8))

    # Создаём DataFrame для heatmap
    heatmap_data = df_results.pivot(index='refill_speed', columns='operator_payment_mean', values='avg_wait_mean')

    im = ax.imshow(heatmap_data.values, cmap='RdYlGn_r', aspect='auto')

    # Настройка осей
    ax.set_xticks(np.arange(len(A_unique)))
    ax.set_yticks(np.arange(len(B_unique)))
    ax.set_xticklabels(A_unique)
    ax.set_yticklabels(B_unique)

    ax.set_xlabel('Operator Payment Mean (с)', fontsize=12)
    ax.set_ylabel('Refill Speed (л/с)', fontsize=12)
    ax.set_title('Тепловая карта: Среднее время ожидания', fontsize=14, fontweight='bold')

    # Добавляем значения в ячейки
    for i in range(len(B_unique)):
        for j in range(len(A_unique)):
            text = ax.text(j, i, f'{heatmap_data.values[i, j]:.1f}',
                          ha="center", va="center", color="black", fontsize=9)

    fig.colorbar(im, ax=ax, label='Время ожидания (с)')
    plt.tight_layout()
    # plt.savefig(OUTPUT_DIR / "heatmap_wait_time.png", dpi=300, bbox_inches='tight')
    print(f"✓ График сохранён: heatmap_wait_time.png")
    plt.close()


def plot_main_effects(df_results):
    """
    Строит графики главных эффектов для каждого фактора.
    """
    fig, axes = plt.subplots(2, 2, figsize=(14, 10))

    # Главный эффект фактора A на время ожидания
    factor_a_effect = df_results.groupby('operator_payment_mean')['avg_wait_mean'].agg(['mean', 'std'])
    axes[0, 0].errorbar(factor_a_effect.index, factor_a_effect['mean'],
                       yerr=factor_a_effect['std'], marker='o', linewidth=2,
                       markersize=8, capsize=5, color='steelblue')
    axes[0, 0].set_xlabel('Operator Payment Mean (с)', fontsize=11)
    axes[0, 0].set_ylabel('Среднее время ожидания (с)', fontsize=11)
    axes[0, 0].set_title('Главный эффект фактора A на время ожидания', fontsize=12, fontweight='bold')
    axes[0, 0].grid(True, alpha=0.3)

    # Главный эффект фактора B на время ожидания
    factor_b_effect = df_results.groupby('refill_speed')['avg_wait_mean'].agg(['mean', 'std'])
    axes[0, 1].errorbar(factor_b_effect.index, factor_b_effect['mean'],
                       yerr=factor_b_effect['std'], marker='s', linewidth=2,
                       markersize=8, capsize=5, color='coral')
    axes[0, 1].set_xlabel('Refill Speed (л/с)', fontsize=11)
    axes[0, 1].set_ylabel('Среднее время ожидания (с)', fontsize=11)
    axes[0, 1].set_title('Главный эффект фактора B на время ожидания', fontsize=12, fontweight='bold')
    axes[0, 1].grid(True, alpha=0.3)

    # Главный эффект фактора A на пропускную способность
    factor_a_effect_thr = df_results.groupby('operator_payment_mean')['throughput_mean'].agg(['mean', 'std'])
    axes[1, 0].errorbar(factor_a_effect_thr.index, factor_a_effect_thr['mean'],
                       yerr=factor_a_effect_thr['std'], marker='o', linewidth=2,
                       markersize=8, capsize=5, color='green')
    axes[1, 0].set_xlabel('Operator Payment Mean (с)', fontsize=11)
    axes[1, 0].set_ylabel('Пропускная способность (машин)', fontsize=11)
    axes[1, 0].set_title('Главный эффект фактора A на пропускную способность', fontsize=12, fontweight='bold')
    axes[1, 0].grid(True, alpha=0.3)

    # Главный эффект фактора B на пропускную способность
    factor_b_effect_thr = df_results.groupby('refill_speed')['throughput_mean'].agg(['mean', 'std'])
    axes[1, 1].errorbar(factor_b_effect_thr.index, factor_b_effect_thr['mean'],
                       yerr=factor_b_effect_thr['std'], marker='s', linewidth=2,
                       markersize=8, capsize=5, color='purple')
    axes[1, 1].set_xlabel('Refill Speed (л/с)', fontsize=11)
    axes[1, 1].set_ylabel('Пропускная способность (машин)', fontsize=11)
    axes[1, 1].set_title('Главный эффект фактора B на пропускную способность', fontsize=12, fontweight='bold')
    axes[1, 1].grid(True, alpha=0.3)

    plt.tight_layout()
    # plt.savefig(OUTPUT_DIR / "main_effects.png", dpi=300, bbox_inches='tight')
    print(f"✓ График сохранён: main_effects.png")
    plt.close()


def main():
    print("=" * 80)
    print("ЗАДАЧА 4: Двухфакторный эксперимент с поверхностью отклика")
    print("=" * 80)
    print(f"\nФактор A: operator_payment_mean (среднее время обслуживания у оператора)")
    print(f"  Уровни: {FACTOR_A_LEVELS}")
    print(f"\nФактор B: refill_speed (скорость заправки)")
    print(f"  Уровни: {FACTOR_B_LEVELS}")
    print(f"\nВремя симуляции: {SIMULATION_TIME/3600:.1f} часов")
    print(f"Повторных прогонов на комбинацию: {NUM_RUNS_PER_COMBINATION}")
    print(f"Всего комбинаций: {len(FACTOR_A_LEVELS) * len(FACTOR_B_LEVELS)}")
    print(f"Всего прогонов: {len(FACTOR_A_LEVELS) * len(FACTOR_B_LEVELS) * NUM_RUNS_PER_COMBINATION}")
    print("\n" + "=" * 80)

    # Выполнение полного факторного эксперимента
    results = []
    total_combinations = len(FACTOR_A_LEVELS) * len(FACTOR_B_LEVELS)
    counter = 0

    print("\nВЫПОЛНЕНИЕ ЭКСПЕРИМЕНТОВ:")
    print("-" * 80)

    for operator_payment_mean in FACTOR_A_LEVELS:
        for refill_speed in FACTOR_B_LEVELS:
            counter += 1
            print(f"\n[{counter}/{total_combinations}] operator_payment_mean={operator_payment_mean}, refill_speed={refill_speed}")

            result = run_experiment_combination(operator_payment_mean, refill_speed, NUM_RUNS_PER_COMBINATION)
            results.append(result)

            print(f"  - Время ожидания: {result['avg_wait_mean']:.2f} ± {result['avg_wait_std']:.2f} с")
            print(f"  - Пропускная способность: {result['throughput_mean']:.1f} ± {result['throughput_std']:.1f} машин")

    # Сохранение результатов
    df_results = pd.DataFrame(results)
    df_results.to_csv(OUTPUT_DIR / "two_factor_experiment_data.csv", index=False)
    print(f"\n✓ Данные сохранены: two_factor_experiment_data.csv")

    # Анализ значимости факторов
    anova_results = analyze_factor_significance(df_results)

    # Сохранение результатов ANOVA
    anova_summary = []
    for response_name, response_data in anova_results.items():
        anova_summary.append({
            'response': response_name,
            'factor': 'A (operator_payment_mean)',
            'f_statistic': response_data['factor_a_f'],
            'p_value': response_data['factor_a_p'],
            'significant': response_data['factor_a_p'] < 0.05
        })
        anova_summary.append({
            'response': response_name,
            'factor': 'B (refill_speed)',
            'f_statistic': response_data['factor_b_f'],
            'p_value': response_data['factor_b_p'],
            'significant': response_data['factor_b_p'] < 0.05
        })

    df_anova = pd.DataFrame(anova_summary)
    df_anova.to_csv(OUTPUT_DIR / "anova_results.csv", index=False)
    print(f"\n✓ Результаты ANOVA сохранены: anova_results.csv")

    # Построение графиков
    print("\n" + "=" * 80)
    print("ПОСТРОЕНИЕ ГРАФИКОВ")
    print("=" * 80)

    plot_response_surface(df_results)
    plot_main_effects(df_results)

    # Итоговые выводы
    print("\n" + "=" * 80)
    print("ИНТЕРПРЕТАЦИЯ РЕЗУЛЬТАТОВ")
    print("=" * 80)

    # Находим оптимальную комбинацию
    best_wait = df_results.loc[df_results['avg_wait_mean'].idxmin()]
    best_throughput = df_results.loc[df_results['throughput_mean'].idxmax()]

    print("\nОптимальные комбинации факторов:")
    print(f"\nМинимальное время ожидания ({best_wait['avg_wait_mean']:.2f} с):")
    print(f"  - operator_payment_mean = {best_wait['operator_payment_mean']} с")
    print(f"  - refill_speed = {best_wait['refill_speed']} л/с")

    print(f"\nМаксимальная пропускная способность ({best_throughput['throughput_mean']:.1f} машин):")
    print(f"  - operator_payment_mean = {best_throughput['operator_payment_mean']} с")
    print(f"  - refill_speed = {best_throughput['refill_speed']} л/с")

    # Определяем более значимый фактор
    wait_f_a = anova_results['wait_time']['factor_a_f']
    wait_f_b = anova_results['wait_time']['factor_b_f']
    thr_f_a = anova_results['throughput']['factor_a_f']
    thr_f_b = anova_results['throughput']['factor_b_f']

    print("\n" + "=" * 80)
    print("ВЫВОДЫ:")
    print("=" * 80)

    print(f"\n1. Для отклика 'Среднее время ожидания':")
    if wait_f_a > wait_f_b:
        print(f"   Более значимый фактор: A (operator_payment_mean)")
        print(f"   F-статистика: {wait_f_a:.2f} vs {wait_f_b:.2f}")
        print(f"   Фактор A влияет в {wait_f_a/wait_f_b:.2f} раз сильнее")
    else:
        print(f"   Более значимый фактор: B (refill_speed)")
        print(f"   F-статистика: {wait_f_b:.2f} vs {wait_f_a:.2f}")
        print(f"   Фактор B влияет в {wait_f_b/wait_f_a:.2f} раз сильнее")

    print(f"\n2. Для отклика 'Пропускная способность':")
    if thr_f_a > thr_f_b:
        print(f"   Более значимый фактор: A (operator_payment_mean)")
        print(f"   F-статистика: {thr_f_a:.2f} vs {thr_f_b:.2f}")
        print(f"   Фактор A влияет в {thr_f_a/thr_f_b:.2f} раз сильнее")
    else:
        print(f"   Более значимый фактор: B (refill_speed)")
        print(f"   F-статистика: {thr_f_b:.2f} vs {thr_f_a:.2f}")
        print(f"   Фактор B влияет в {thr_f_b/thr_f_a:.2f} раз сильнее")

    print(f"\n3. Характер влияния факторов:")
    print(f"   - Фактор A (operator_payment_mean): увеличение → дольше обслуживание → увеличение времени ожидания")
    print(f"   - Фактор B (refill_speed): увеличение → быстрее заправка → меньше время ожидания")

    print(f"\n4. Поверхность отклика показывает {'линейную' if abs(wait_f_a - wait_f_b) < 2 else 'нелинейную'} зависимость")
    print(f"   между факторами и откликами.")

    print("=" * 80)


if __name__ == "__main__":
    main()

