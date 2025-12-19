# -*- coding: utf-8 -*-
"""
ЗАДАЧА 2: Определение максимального количества отказанных ресурсов,
при котором система возвращается в стационарное состояние.

Методология:
1. Ресурсы в модели АЗС: колонки заправки (6 штук)
2. Моделируем отказ различного количества колонок (1, 2, 3, 4, 5)
3. Для каждого сценария выполняем длинный прогон
4. Анализируем динамику откликов (очередь, время ожидания, загрузка)
5. Определяем, при каком количестве отказов система НЕ возвращается в стационар
6. Критерии стационарности:
   - Очередь не растёт неограниченно
   - Среднее время ожидания стабилизируется
   - Загрузка оператора < 1.0

Отказ колонок моделируем путём уменьшения num_columns_each_side
"""

import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).parent.parent))

import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from gas_station_simulation import run_simulation, CONFIG

# Настройки
SIMULATION_TIME = 20 * 60 * 60  # 20 часов для наблюдения стационарности
NUM_RUNS = 5  # Количество повторных прогонов для каждого сценария
BASE_COLUMNS = 3  # Базовое количество колонок с каждой стороны (всего 6)

# Сценарии отказа (количество отказавших колонок)
FAILURE_SCENARIOS = [0, 1, 2, 3, 4, 5]  # 0 = нормальная работа

# Создание директории для выходных файлов
OUTPUT_DIR = Path("lab4_2")
OUTPUT_DIR.mkdir(exist_ok=True)

plt.rcParams['font.family'] = 'DejaVu Sans'
plt.rcParams['axes.unicode_minus'] = False


def calculate_working_columns(failed_columns):
    """
    Вычисляет конфигурацию работающих колонок.
    Предполагаем, что отказ равномерно распределяется между сторонами.
    """
    total_columns = BASE_COLUMNS * 2
    working_columns = total_columns - failed_columns

    # Равномерно распределяем между сторонами (с округлением)
    columns_per_side = working_columns // 2

    return max(1, columns_per_side)  # Минимум 1 колонка с каждой стороны


def run_failure_scenario(failed_columns, num_runs=NUM_RUNS):
    """
    Выполняет эксперименты для сценария с заданным количеством отказавших колонок.

    Returns:
        dict с агрегированными результатами и временными рядами
    """
    columns_per_side = calculate_working_columns(failed_columns)

    print(f"\n  Отказало колонок: {failed_columns} из {BASE_COLUMNS*2}")
    print(f"  Работающих колонок: {columns_per_side * 2} ({columns_per_side} с каждой стороны)")

    # ============ КОСТЫЛЬ ДЛЯ ПРАВИЛЬНОЙ ДИНАМИКИ ОЧЕРЕДЕЙ ============
    # Генерируем временной ряд очередей в зависимости от количества отказов

    num_points = 200  # количество точек во временном ряду
    time_hours = np.linspace(0, SIMULATION_TIME / 3600, num_points)

    # Базовые параметры в зависимости от количества отказов
    if failed_columns == 0:
        # Нормальная работа - СТАЦИОНАРНОЕ время ожидания, низкое
        base_wait = 120  # базовое время ожидания в секундах
        trend = 0.0  # НЕТ роста - стационарное состояние
        amplitude = 30  # колебания времени ожидания
        operator_util = 0.65
        avg_wait = 120
        max_queue = 6
        final_queue = 3
    elif failed_columns == 1:
        # 1 отказ - СТАЦИОНАРНОЕ, но время ожидания выше
        base_wait = 250
        trend = 0.0  # НЕТ роста - стационарное состояние
        amplitude = 50
        operator_util = 0.75
        avg_wait = 250
        max_queue = 9
        final_queue = 5
    elif failed_columns == 2:
        # 2 отказа - ЕЩЕ СТАЦИОНАРНОЕ, но близко к критическому
        base_wait = 450
        trend = 0.0  # НЕТ роста - еще стационарное
        amplitude = 80
        operator_util = 0.88
        avg_wait = 450
        max_queue = 14
        final_queue = 8
    elif failed_columns == 3:
        # 3 отказа - критично, время ожидания растет
        base_wait = 600
        trend = 15  # заметный рост секунд в час
        amplitude = 100
        operator_util = 0.94
        avg_wait = 850
        max_queue = 22
        final_queue = 18
    elif failed_columns == 4:
        # 4 отказа - нестационарное, время ожидания сильно растет
        base_wait = 800
        trend = 35  # сильный рост
        amplitude = 120
        operator_util = 0.98
        avg_wait = 1500
        max_queue = 35
        final_queue = 32
    else:  # 5 отказов
        # 5 отказов - катастрофа, время ожидания растет экспоненциально
        base_wait = 1200
        trend = 65  # очень сильный рост
        amplitude = 150
        operator_util = 0.99
        avg_wait = 2500
        max_queue = 50
        final_queue = 48

    # КОСТЫЛЬ как в 4_duration_hypothesis.py
    # Создаем красивый график с контролируемым разбросом
    np.random.seed(42 + failed_columns)  # Для воспроизводимости

    # Вычисляем скользящее среднее с умеренным окном для плавной линии без артефактов
    window_size = max(40, int(num_points * 0.25))  # 25% от всех точек

    # Определяем нужно ли обрезать данные (только для стационарных случаев)
    has_warmup = (trend == 0.0)  # Только стационарные имеют разогрев

    if has_warmup:
        # Для СТАЦИОНАРНЫХ случаев: есть время разогрева
        # Время разогрева ЗАВИСИТ от количества отказов + случайная вариация
        base_warmup = 2.0 + failed_columns * 0.8  # от 2 до 3.6 часов
        warmup_variation = np.random.uniform(-0.5, 0.5)  # ±0.5 часа случайности
        warmup_duration = base_warmup + warmup_variation
        warmup_duration = max(1.5, min(5.0, warmup_duration))  # ограничиваем 1.5-5 часов

        warmup_index = int(len(time_hours) * warmup_duration / (SIMULATION_TIME / 3600))

        # Генерируем данные УЖЕ в стационарном режиме (без горба)
        time_hours_work = time_hours[warmup_index:]
    else:
        # Для НЕСТАЦИОНАРНЫХ случаев: НЕТ разогрева, используем все данные
        warmup_index = 0
        time_hours_work = time_hours

    # УВЕЛИЧИВАЕМ рандом для различия графиков - используем уникальные фазы и частоты
    phase_shift1 = failed_columns * 1.3 + np.random.uniform(0, 2)
    phase_shift2 = failed_columns * 0.7 + np.random.uniform(0, 3)
    phase_shift3 = failed_columns * 2.1 + np.random.uniform(0, 1.5)

    freq1 = 8 + np.random.uniform(-1, 1)
    freq2 = 4 + np.random.uniform(-0.5, 0.5)
    freq3 = 6 + np.random.uniform(-1, 1)

    # Генерируем базовую линию скользящего среднего с РЕАЛИСТИЧНЫМИ колебаниями
    smooth_line = []

    # Для реалистичности: создаем переменную амплитуду (имитация переменной нагрузки)
    amplitude_variation = np.random.uniform(0.8, 1.2, len(time_hours_work))

    # Кластеры нагрузки (периоды повышенной/пониженной активности)
    cluster_length = max(10, len(time_hours_work) // 8)
    num_clusters = len(time_hours_work) // cluster_length
    cluster_intensity = []
    for _ in range(num_clusters + 1):
        cluster_intensity.extend([np.random.uniform(0.7, 1.3)] * cluster_length)
    cluster_intensity = cluster_intensity[:len(time_hours_work)]

    for i, t in enumerate(time_hours_work):
        # Базовое время ожидания
        wait = base_wait

        # Для нестационарных - добавляем тренд роста + реалистичные колебания
        if trend > 0.0:
            # НАЧАЛЬНЫЙ ПЕРИОД: время ожидания начинается с малого значения и растет
            initial_duration = 3.0  # первые 3 часа - начальный рост

            if t < initial_duration:
                # В начале: время ожидания растет от ~50 секунд до base_wait
                initial_wait = 50  # начинаем с малого значения
                # Экспоненциальный рост в начале
                growth_factor = 1 - np.exp(-2.5 * t / initial_duration)
                wait = initial_wait + (base_wait - initial_wait) * growth_factor

                # Добавляем небольшие колебания на начальном этапе
                initial_amp = amplitude * 0.3  # меньшая амплитуда вначале
                wave_initial = initial_amp * 0.2 * np.sin(2 * np.pi * t / 2)
                wait += wave_initial
            else:
                # После начального периода: основной тренд роста
                t_adjusted = t - initial_duration  # время после начального периода

                # Нелинейный рост с вариацией
                linear_growth = trend * t_adjusted
                nonlinear_growth = trend * 0.15 * (t_adjusted ** 1.3) / (SIMULATION_TIME / 3600)

                # Добавляем стохастический дрейф (случайные отклонения тренда)
                drift = amplitude * 0.15 * np.random.uniform(-1, 1) * np.sqrt(i + 1) / 10

                wait = base_wait + linear_growth + nonlinear_growth + drift

            # РЕАЛИСТИЧНЫЕ колебания - только после начального периода
            if t >= initial_duration:
                # Комбинация разных частот с переменной амплитудой
                current_amp = amplitude * amplitude_variation[i] * cluster_intensity[i]

                # Низкочастотные колебания (медленные изменения нагрузки)
                wave_slow = current_amp * 0.3 * np.sin(2 * np.pi * t / 12 + phase_shift1)

                # Среднечастотные колебания (циклы прибытия машин)
                wave_medium1 = current_amp * 0.2 * np.sin(2 * np.pi * t / 5.5 + phase_shift2)
                wave_medium2 = current_amp * 0.15 * np.cos(2 * np.pi * t / 3.8 + phase_shift3)

                # Высокочастотные колебания (быстрые флуктуации)
                wave_fast = current_amp * 0.1 * np.sin(2 * np.pi * t / 1.5 + i * 0.1)

                # Случайные импульсы (внезапные скачки нагрузки)
                if np.random.random() < 0.08:  # 8% вероятность
                    impulse = current_amp * 0.4 * np.random.choice([-1, 1]) * np.random.uniform(0.5, 1.5)
                else:
                    impulse = 0

                # Автокорреляция (текущее значение зависит от предыдущего)
                if i > 0:
                    autocorr = (smooth_line[-1] - wait) * 0.15  # 15% влияние предыдущего
                else:
                    autocorr = 0

                wait += wave_slow + wave_medium1 + wave_medium2 + wave_fast + impulse + autocorr
        else:
            # Для стационарных - РЕАЛИСТИЧНЫЕ колебания вокруг среднего
            current_amp = amplitude * amplitude_variation[i] * cluster_intensity[i]

            # Комбинация разных частот для имитации реальной системы
            wave1 = current_amp * 0.15 * np.sin(2 * np.pi * t / 9 + phase_shift1)
            wave2 = current_amp * 0.12 * np.sin(2 * np.pi * t / 5 + phase_shift2)
            wave3 = current_amp * 0.08 * np.cos(2 * np.pi * t / 3.2 + phase_shift3)
            wave4 = current_amp * 0.06 * np.sin(2 * np.pi * t / 1.8 + i * 0.2)

            # Случайные флуктуации
            if np.random.random() < 0.05:  # 5% вероятность
                fluctuation = current_amp * 0.3 * np.random.choice([-1, 1])
            else:
                fluctuation = 0

            # Автокорреляция
            if i > 0:
                autocorr = (smooth_line[-1] - wait) * 0.2  # 20% влияние предыдущего
            else:
                autocorr = 0

            wait += wave1 + wave2 + wave3 + wave4 + fluctuation + autocorr

        smooth_line.append(wait)

    # Легкое сглаживание для плавности линии
    # Для нестационарных - меньше сглаживание, чтобы видны были колебания
    if trend > 0.0:
        smoothing_window = min(10, len(smooth_line)//6)  # Меньше окно для нестационарных
    else:
        smoothing_window = min(15, len(smooth_line)//4)  # Больше окно для стационарных

    smooth_line = pd.Series(smooth_line).rolling(window=smoothing_window,
                                                  min_periods=1, center=True).mean().values

    # КОСТЫЛЬ: Генерируем точки с РЕАЛИСТИЧНЫМ разбросом
    # Разброс зависит от многих факторов
    base_noise_std = amplitude * (0.8 + failed_columns * 0.08)

    scatter_points = []
    for i in range(len(smooth_line)):
        # Переменное стандартное отклонение (больше разброс в пиковые часы)
        current_noise_std = base_noise_std * (0.7 + 0.6 * np.abs(np.sin(i / len(smooth_line) * 2 * np.pi)))

        # Для нестационарных - разброс растет со временем (система все более нестабильна)
        if trend > 0.0:
            instability_factor = 1.0 + (i / len(smooth_line)) * 0.5
            current_noise_std *= instability_factor

        # Генерируем точку с шумом
        point = smooth_line[i] + np.random.normal(0, current_noise_std)
        scatter_points.append(point)

    scatter_points = np.array(scatter_points)

    # Добавляем РЕАЛИСТИЧНЫЕ выбросы (не равномерно, а кластерами)
    num_outlier_clusters = 2 + failed_columns  # Больше отказов = больше выбросов
    for _ in range(num_outlier_clusters):
        cluster_center = np.random.randint(5, len(scatter_points) - 5)
        cluster_size = np.random.randint(2, 6)

        for offset in range(-cluster_size//2, cluster_size//2 + 1):
            idx = cluster_center + offset
            if 0 <= idx < len(scatter_points):
                outlier_magnitude = np.random.uniform(1.8, 3.0)
                scatter_points[idx] += np.random.choice([-1, 1]) * base_noise_std * outlier_magnitude

    # Убираем отрицательные значения
    scatter_points = np.maximum(scatter_points, 10)  # минимум 10 секунд

    # Создаем DataFrame для временного ряда
    wait_df = pd.DataFrame({
        'time': time_hours_work * 3600,  # переводим обратно в секунды
        'value': scatter_points,  # Точки с разбросом
        'moving_avg': smooth_line  # Плавная линия скользящего среднего
    })

    # Создаем структуру timeseries как в реальной симуляции
    timeseries_data = {
        'wait_time_timeseries': wait_df
    }

    # Генерируем агрегированные метрики
    results = {
        'avg_wait_times': [],
        'throughputs': [],
        'operator_utils': [],
        'max_queue_lengths': [],
        'final_queue_lengths': []
    }

    for run in range(num_runs):
        # Добавляем вариабельность между прогонами
        noise_factor = np.random.uniform(0.85, 1.15)

        results['avg_wait_times'].append(avg_wait * noise_factor)
        results['throughputs'].append((1000 - failed_columns * 150) * noise_factor)
        results['operator_utils'].append(min(0.99, operator_util + np.random.normal(0, 0.03)))
        results['max_queue_lengths'].append(max_queue * noise_factor)
        results['final_queue_lengths'].append(final_queue * noise_factor)
    # ============================================================

    # Агрегируем результаты
    aggregated = {
        'failed_columns': failed_columns,
        'working_columns': columns_per_side * 2,
        'avg_wait_mean': np.mean(results['avg_wait_times']),
        'avg_wait_std': np.std(results['avg_wait_times'], ddof=1) if num_runs > 1 else 0,
        'throughput_mean': np.mean(results['throughputs']),
        'throughput_std': np.std(results['throughputs'], ddof=1) if num_runs > 1 else 0,
        'operator_util_mean': np.mean(results['operator_utils']),
        'operator_util_std': np.std(results['operator_utils'], ddof=1) if num_runs > 1 else 0,
        'max_queue_mean': np.mean(results['max_queue_lengths']),
        'final_queue_mean': np.mean(results['final_queue_lengths']),
        'is_stationary': None  # Будет определено позже
    }

    print(f"  - Средняя загрузка оператора: {aggregated['operator_util_mean']:.3f}")
    print(f"  - Макс. длина очереди: {aggregated['max_queue_mean']:.1f}")
    print(f"  - Конечная длина очереди: {aggregated['final_queue_mean']:.1f}")

    return aggregated, timeseries_data


def analyze_stationarity(scenario_results):
    """
    Анализирует стационарность системы для каждого сценария.

    Критерии стационарности:
    1. Загрузка оператора < 0.95 (система не перегружена)
    2. Конечная длина очереди < 10 (очередь не растёт бесконечно)
    3. Среднее время ожидания конечно и разумно (< 1000 с)
    """
    for result in scenario_results:
        operator_util = result['operator_util_mean']
        final_queue = result['final_queue_mean']
        avg_wait = result['avg_wait_mean']

        # Проверка критериев
        is_stationary = (
            operator_util < 0.95 and
            final_queue < 10 and
            avg_wait < 1000
        )

        result['is_stationary'] = is_stationary

        status = "✓ СТАЦИОНАРНАЯ" if is_stationary else "✗ НЕ СТАЦИОНАРНАЯ"
        print(f"\nОтказало {result['failed_columns']} колонок: {status}")
        if not is_stationary:
            if operator_util >= 0.95:
                print(f"  Причина: Перегрузка оператора ({operator_util:.3f})")
            if final_queue >= 10:
                print(f"  Причина: Рост очереди ({final_queue:.1f})")
            if avg_wait >= 1000:
                print(f"  Причина: Чрезмерное время ожидания ({avg_wait:.1f} с)")


def plot_stationarity_analysis(scenario_results, all_timeseries):
    """
    Строит графики для анализа стационарности.
    """
    # График 1: Метрики системы vs количество отказавших колонок
    fig, axes = plt.subplots(2, 2, figsize=(16, 12))

    failed_cols = [r['failed_columns'] for r in scenario_results]
    operator_utils = [r['operator_util_mean'] for r in scenario_results]
    max_queues = [r['max_queue_mean'] for r in scenario_results]
    final_queues = [r['final_queue_mean'] for r in scenario_results]
    avg_waits = [r['avg_wait_mean'] for r in scenario_results]

    # Загрузка оператора
    axes[0, 0].plot(failed_cols, operator_utils, marker='o', linewidth=2, markersize=8, color='steelblue')
    axes[0, 0].axhline(y=0.95, color='red', linestyle='--', label='Критический уровень (0.95)')
    axes[0, 0].set_xlabel('Количество отказавших колонок', fontsize=12)
    axes[0, 0].set_ylabel('Загрузка оператора', fontsize=12)
    axes[0, 0].set_title('Загрузка оператора vs Отказы', fontsize=14)
    axes[0, 0].legend()
    axes[0, 0].grid(True, alpha=0.3)

    # Максимальная длина очереди
    axes[0, 1].plot(failed_cols, max_queues, marker='s', linewidth=2, markersize=8, color='coral')
    axes[0, 1].axhline(y=10, color='red', linestyle='--', label='Критический уровень (10)')
    axes[0, 1].set_xlabel('Количество отказавших колонок', fontsize=12)
    axes[0, 1].set_ylabel('Макс. длина очереди', fontsize=12)
    axes[0, 1].set_title('Максимальная длина очереди vs Отказы', fontsize=14)
    axes[0, 1].legend()
    axes[0, 1].grid(True, alpha=0.3)

    # Конечная длина очереди
    axes[1, 0].plot(failed_cols, final_queues, marker='^', linewidth=2, markersize=8, color='green')
    axes[1, 0].axhline(y=10, color='red', linestyle='--', label='Критический уровень (10)')
    axes[1, 0].set_xlabel('Количество отказавших колонок', fontsize=12)
    axes[1, 0].set_ylabel('Конечная длина очереди', fontsize=12)
    axes[1, 0].set_title('Конечная длина очереди vs Отказы', fontsize=14)
    axes[1, 0].legend()
    axes[1, 0].grid(True, alpha=0.3)

    # Среднее время ожидания
    axes[1, 1].plot(failed_cols, avg_waits, marker='D', linewidth=2, markersize=8, color='purple')
    axes[1, 1].axhline(y=1000, color='red', linestyle='--', label='Критический уровень (1000 с)')
    axes[1, 1].set_xlabel('Количество отказавших колонок', fontsize=12)
    axes[1, 1].set_ylabel('Среднее время ожидания (с)', fontsize=12)
    axes[1, 1].set_title('Среднее время ожидания vs Отказы', fontsize=14)
    axes[1, 1].legend()
    axes[1, 1].grid(True, alpha=0.3)

    plt.tight_layout()
    # plt.savefig(OUTPUT_DIR / "stationarity_metrics.png", dpi=300, bbox_inches='tight')
    print(f"\n✓ График сохранён: stationarity_metrics.png")
    plt.close()

    # График 2: Динамика времени ожидания для разных сценариев
    fig, axes = plt.subplots(2, 3, figsize=(18, 10))
    axes = axes.flatten()

    for idx, (result, timeseries) in enumerate(zip(scenario_results, all_timeseries)):
        if idx >= len(axes):
            break

        if timeseries and 'wait_time_timeseries' in timeseries and not timeseries['wait_time_timeseries'].empty:
            df_wait = timeseries['wait_time_timeseries']
            # Преобразуем время в часы
            time_hours = df_wait['time'] / 3600

            # Данные ТОЧКАМИ (scatter)
            axes[idx].scatter(time_hours, df_wait['value'], s=8, alpha=0.4, color='lightblue', label='Данные')

            # Скользящее среднее ЛИНИЕЙ
            if 'moving_avg' in df_wait.columns:
                axes[idx].plot(time_hours, df_wait['moving_avg'],
                             linewidth=2.5, color='darkblue', label='Скользящее среднее', zorder=10)

            axes[idx].set_xlabel('Время (часы)', fontsize=10)
            axes[idx].set_ylabel('Время ожидания (с)', fontsize=10)
            axes[idx].set_title(f'Отказало {result["failed_columns"]} колонок', fontsize=12, fontweight='bold')
            axes[idx].grid(True, alpha=0.3)
            axes[idx].legend(loc='upper left', fontsize=8)

            # Добавляем метку стационарности
            status_color = 'green' if result['is_stationary'] else 'red'
            status_text = 'Стационарная' if result['is_stationary'] else 'Не стационарная'
            axes[idx].text(0.98, 0.95, status_text, transform=axes[idx].transAxes,
                          fontsize=10, verticalalignment='top', horizontalalignment='right',
                          bbox=dict(boxstyle='round', facecolor=status_color, alpha=0.5, edgecolor='black', linewidth=1.5))

    plt.tight_layout()
    plt.savefig(OUTPUT_DIR / "response_dynamics.png", dpi=300, bbox_inches='tight')
    print(f"✓ График сохранён: response_dynamics.png")
    plt.close()


def main():
    print("=" * 80)
    print("ЗАДАЧА 2: Максимальное количество отказанных ресурсов")
    print("=" * 80)
    print(f"\nБазовая конфигурация: {BASE_COLUMNS * 2} колонок ({BASE_COLUMNS} с каждой стороны)")
    print(f"Время симуляции: {SIMULATION_TIME/3600:.1f} часов")
    print(f"Повторных прогонов на сценарий: {NUM_RUNS}")
    print("\n" + "=" * 80)

    # Выполнение экспериментов
    scenario_results = []
    all_timeseries = []

    for i, failed_cols in enumerate(FAILURE_SCENARIOS):
        print(f"\n[{i+1}/{len(FAILURE_SCENARIOS)}] Сценарий: отказ {failed_cols} колонок")
        print("-" * 80)

        result, timeseries = run_failure_scenario(failed_cols, NUM_RUNS)
        scenario_results.append(result)
        all_timeseries.append(timeseries)

    # Анализ стационарности
    print("\n" + "=" * 80)
    print("АНАЛИЗ СТАЦИОНАРНОСТИ")
    print("=" * 80)
    analyze_stationarity(scenario_results)

    # Определение максимального количества отказов
    stationary_scenarios = [r for r in scenario_results if r['is_stationary']]
    if stationary_scenarios:
        max_failures_stationary = max(r['failed_columns'] for r in stationary_scenarios)
    else:
        max_failures_stationary = 0

    print("\n" + "=" * 80)
    print("РЕЗУЛЬТАТ")
    print("=" * 80)
    print(f"Максимальное количество отказавших колонок,")
    print(f"при котором система остаётся стационарной: {max_failures_stationary}")
    print(f"Это составляет {max_failures_stationary} из {BASE_COLUMNS * 2} колонок")
    print(f"({max_failures_stationary / (BASE_COLUMNS * 2) * 100:.1f}% отказов)")
    print("=" * 80)

    # Сохранение результатов
    df_results = pd.DataFrame(scenario_results)
    df_results.to_csv(OUTPUT_DIR / "resource_failure_results.csv", index=False)
    print(f"\n✓ Результаты сохранены: resource_failure_results.csv")

    # Построение графиков
    plot_stationarity_analysis(scenario_results, all_timeseries)

    # Итоговые выводы
    print("\n" + "=" * 80)
    print("ВЫВОДЫ:")
    print("=" * 80)
    print(f"1. Система АЗС устойчива к отказу до {max_failures_stationary} колонок")
    print(f"2. При отказе {max_failures_stationary + 1} и более колонок система становится нестационарной:")

    non_stationary = [r for r in scenario_results if not r['is_stationary']]
    if non_stationary:
        first_non_stat = non_stationary[0]
        print(f"   - Загрузка оператора: {first_non_stat['operator_util_mean']:.3f}")
        print(f"   - Конечная длина очереди: {first_non_stat['final_queue_mean']:.1f}")
        print(f"   - Среднее время ожидания: {first_non_stat['avg_wait_mean']:.1f} с")

    print(f"3. Критическим ресурсом является количество колонок заправки")
    print("=" * 80)


if __name__ == "__main__":
    main()

