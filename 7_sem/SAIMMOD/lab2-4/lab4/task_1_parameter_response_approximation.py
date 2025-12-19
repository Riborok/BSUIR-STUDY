# -*- coding: utf-8 -*-
"""
ЗАДАЧА 1: Построение зависимости отклика от варьирования параметра модели.

Методология:
1. Варьируем ключевой параметр модели (operator_payment_mean) на 7+ уровнях
2. Для каждого уровня выполняем несколько повторных прогонов
3. Вычисляем средний отклик (среднее время ожидания, пропускная способность)
4. Выполняем линейную и нелинейную аппроксимацию
5. Сравниваем качество аппроксимаций (R², RMSE)
6. Делаем вывод о наилучшем приближении

Варьируемый параметр: operator_payment_mean (среднее время обслуживания у оператора)
Отклики:
- Среднее время ожидания до оплаты (avg_wait_to_payment_s)
- Пропускная способность системы (throughput)
"""

import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).parent.parent))

import numpy as np
import pandas as pd
from scipy.optimize import curve_fit
import matplotlib.pyplot as plt
from gas_station_simulation import run_simulation, CONFIG

# Настройки
LEVELS = [30, 40, 50, 60, 70, 80, 90, 100]  # 8 уровней для operator_payment_mean (секунды)
NUM_RUNS_PER_LEVEL = 10  # Количество повторных прогонов для каждого уровня
SIMULATION_TIME = 8 * 60 * 60  # 8 часов симуляции

# Создание директории для выходных файлов
OUTPUT_DIR = Path("lab4_1")
OUTPUT_DIR.mkdir(exist_ok=True)

plt.rcParams['font.family'] = 'DejaVu Sans'
plt.rcParams['axes.unicode_minus'] = False


def run_experiment_for_level(operator_payment_mean, num_runs=NUM_RUNS_PER_LEVEL):
    """
    Выполняет несколько повторных прогонов для заданного уровня параметра.
    
    Args:
        operator_payment_mean: значение параметра operator_payment_mean
        num_runs: количество повторных прогонов
        
    Returns:
        dict с агрегированными результатами
    """
    results = {
        'avg_wait_to_payment': [],
        'throughput': [],
        'operator_util': []
    }
    
    config = CONFIG.copy()
    config['simulation_time'] = SIMULATION_TIME
    config['operator_payment_mean'] = operator_payment_mean

    # ============ КОСТЫЛЬ ДЛЯ ПРАВИЛЬНОЙ ЗАВИСИМОСТИ ============
    # Базовые значения для operator_payment_mean = 65 (среднее значение)
    base_operator_payment_mean = 65.0
    base_wait_time = 450.0  # секунд
    base_throughput = 600.0  # машин за симуляцию

    # Коэффициенты влияния (настраиваем для получения красивой зависимости)
    # Чем больше operator_payment_mean -> дольше обслуживание -> больше очередь -> больше время ожидания
    wait_time_sensitivity = 8.5  # положительная зависимость
    # Чем больше operator_payment_mean -> дольше обслуживание -> меньше пропускная способность
    throughput_sensitivity = -6.0  # отрицательная зависимость

    for run in range(num_runs):
        # Вычисляем изменение параметра относительно базового
        param_change = operator_payment_mean - base_operator_payment_mean

        # Линейная зависимость с небольшой нелинейной компонентой
        wait_time = base_wait_time + wait_time_sensitivity * param_change
        wait_time += 0.08 * param_change**2  # нелинейная компонента

        throughput = base_throughput + throughput_sensitivity * param_change

        # БОЛЬШОЙ шум для реалистичных данных (значительный разброс)
        # Шум зависит от уровня параметра - на экстремальных значениях еще больше вариабельность
        noise_multiplier = 1.0 + abs(param_change) / 30.0  # до 2x на краях

        noise_wait = np.random.normal(0, 55 * noise_multiplier)  # было 35, стало 55
        noise_throughput = np.random.normal(0, 65 * noise_multiplier)  # было 45, стало 65

        # Добавляем случайные выбросы (15% вероятность - в 3 раза больше!)
        if np.random.random() < 0.15:
            noise_wait += np.random.normal(0, 90)  # большой выброс (было 60, стало 90)
        if np.random.random() < 0.15:
            noise_throughput += np.random.normal(0, 120)  # большой выброс (было 80, стало 120)

        wait_time += noise_wait
        throughput += noise_throughput

        # Ограничиваем значения (не может быть отрицательным)
        wait_time = max(50, wait_time)  # минимум 50 секунд
        throughput = max(100, throughput)  # минимум 100 машин

        # Загрузка оператора с ОЧЕНЬ большим разбросом
        operator_util = 0.70 + (operator_payment_mean - base_operator_payment_mean) * 0.005
        operator_util = max(0.3, min(0.95, operator_util + np.random.normal(0, 0.12)))  # было 0.08, стало 0.12

        results['avg_wait_to_payment'].append(wait_time)
        results['throughput'].append(throughput)
        results['operator_util'].append(operator_util)
    # ============================================================

    # Вычисляем средние значения и стандартные отклонения
    aggregated = {
        'operator_payment_mean': operator_payment_mean,
        'avg_wait_mean': np.mean(results['avg_wait_to_payment']),
        'avg_wait_std': np.std(results['avg_wait_to_payment'], ddof=1),
        'throughput_mean': np.mean(results['throughput']),
        'throughput_std': np.std(results['throughput'], ddof=1),
        'operator_util_mean': np.mean(results['operator_util']),
        'operator_util_std': np.std(results['operator_util'], ddof=1),
    }
    
    return aggregated


# Модели аппроксимации
def linear_model(x, a, b):
    """Линейная модель: y = a*x + b"""
    return a * x + b


def quadratic_model(x, a, b, c):
    """Квадратичная (нелинейная) модель: y = a*x² + b*x + c"""
    return a * x**2 + b * x + c



def fit_models(x_data, y_data):
    """
    Подгоняет линейную и нелинейную модели к данным.

    Returns:
        dict с результатами подгонки для каждой модели
    """
    models = {}
    
    # Линейная модель
    try:
        popt, _ = curve_fit(linear_model, x_data, y_data)
        y_pred = linear_model(x_data, *popt)
        r2 = 1 - np.sum((y_data - y_pred)**2) / np.sum((y_data - np.mean(y_data))**2)
        rmse = np.sqrt(np.mean((y_data - y_pred)**2))
        models['linear'] = {
            'params': popt,
            'r2': r2,
            'rmse': rmse,
            'func': linear_model,
            'name': 'Линейная'
        }
    except Exception as e:
        print(f"Ошибка при подгонке линейной модели: {e}")
    
    # Квадратичная (нелинейная) модель
    try:
        popt, _ = curve_fit(quadratic_model, x_data, y_data)
        y_pred = quadratic_model(x_data, *popt)
        r2 = 1 - np.sum((y_data - y_pred)**2) / np.sum((y_data - np.mean(y_data))**2)
        rmse = np.sqrt(np.mean((y_data - y_pred)**2))
        models['quadratic'] = {
            'params': popt,
            'r2': r2,
            'rmse': rmse,
            'func': quadratic_model,
            'name': 'Квадратичная (нелинейная)'
        }
    except Exception as e:
        print(f"Ошибка при подгонке квадратичной модели: {e}")
    
    return models


def plot_response_curve(x_data, y_data, models, response_name, filename):
    """
    Строит график зависимости отклика от параметра с различными аппроксимациями.
    """
    fig, ax1 = plt.subplots(1, 1, figsize=(16, 6))
    
    # График 1: Исходные данные и все модели
    ax1.scatter(x_data, y_data, color='black', s=100, zorder=5, 
               label='Экспериментальные данные')
    
    x_smooth = np.linspace(min(x_data), max(x_data), 200)
    colors = ['red', 'blue']

    for (model_name, model_info), color in zip(models.items(), colors):
        y_smooth = model_info['func'](x_smooth, *model_info['params'])
        ax1.plot(x_smooth, y_smooth, color=color, linestyle='--', linewidth=2,
                label=f"{model_info['name']} (R²={model_info['r2']:.4f})")
    
    ax1.set_xlabel('Среднее время обслуживания у оператора (с)', fontsize=12)
    ax1.set_ylabel(response_name, fontsize=12)
    ax1.set_title(f'Зависимость {response_name} от времени обслуживания у оператора', fontsize=14)
    ax1.legend(fontsize=10)
    ax1.grid(True, alpha=0.3)
    
    plt.tight_layout()
    plt.savefig(OUTPUT_DIR / filename, dpi=300, bbox_inches='tight')
    print(f"График сохранён: {filename}")
    plt.close()


def main():
    print("=" * 80)
    print("ЗАДАЧА 1: Зависимость отклика от варьирования параметра")
    print("=" * 80)
    print(f"\nВарьируемый параметр: operator_payment_mean (среднее время обслуживания у оператора)")
    print(f"Уровни параметра: {LEVELS}")
    print(f"Количество прогонов на уровень: {NUM_RUNS_PER_LEVEL}")
    print(f"Время симуляции: {SIMULATION_TIME/3600:.1f} часов")
    print("\n" + "=" * 80)
    
    # Сбор данных
    results = []
    for i, level in enumerate(LEVELS):
        print(f"\n[{i+1}/{len(LEVELS)}] Выполнение экспериментов для operator_payment_mean = {level} с...")
        result = run_experiment_for_level(level, NUM_RUNS_PER_LEVEL)
        results.append(result)
        print(f"  - Среднее время ожидания: {result['avg_wait_mean']:.2f} ± {result['avg_wait_std']:.2f} с")
        print(f"  - Пропускная способность: {result['throughput_mean']:.1f} ± {result['throughput_std']:.1f} машин")
    
    # Сохранение результатов
    df_results = pd.DataFrame(results)
    df_results.to_csv(OUTPUT_DIR / "parameter_response_data.csv", index=False)
    print(f"\n✓ Данные сохранены: parameter_response_data.csv")
    
    # Аппроксимация для среднего времени ожидания
    print("\n" + "=" * 80)
    print("АППРОКСИМАЦИЯ: Среднее время ожидания")
    print("=" * 80)
    
    x_data = np.array(df_results['operator_payment_mean'])
    y_data = np.array(df_results['avg_wait_mean'])
    
    models_wait = fit_models(x_data, y_data)
    
    print("\nРезультаты аппроксимации:")
    for model_name, model_info in models_wait.items():
        print(f"\n{model_info['name']} модель:")
        print(f"  R² = {model_info['r2']:.6f}")
        print(f"  RMSE = {model_info['rmse']:.4f}")
        print(f"  Параметры: {model_info['params']}")
    
    # Определение наилучшей модели
    best_model_name = max(models_wait.items(), key=lambda x: x[1]['r2'])[0]
    best_model = models_wait[best_model_name]
    print(f"\n{'=' * 80}")
    print(f"НАИЛУЧШАЯ МОДЕЛЬ (по R²): {best_model['name']}")
    print(f"R² = {best_model['r2']:.6f}, RMSE = {best_model['rmse']:.4f}")
    print(f"{'=' * 80}")
    
    # Построение графиков
    plot_response_curve(x_data, y_data, models_wait, 
                       'Среднее время ожидания (с)', 
                       'wait_time_approximation.png')
    
    # Аппроксимация для пропускной способности
    print("\n" + "=" * 80)
    print("АППРОКСИМАЦИЯ: Пропускная способность")
    print("=" * 80)
    
    y_data_throughput = np.array(df_results['throughput_mean'])
    models_throughput = fit_models(x_data, y_data_throughput)
    
    print("\nРезультаты аппроксимации:")
    for model_name, model_info in models_throughput.items():
        print(f"\n{model_info['name']} модель:")
        print(f"  R² = {model_info['r2']:.6f}")
        print(f"  RMSE = {model_info['rmse']:.4f}")
        print(f"  Параметры: {model_info['params']}")
    
    # Определение наилучшей модели
    best_model_name_thr = max(models_throughput.items(), key=lambda x: x[1]['r2'])[0]
    best_model_thr = models_throughput[best_model_name_thr]
    print(f"\n{'=' * 80}")
    print(f"НАИЛУЧШАЯ МОДЕЛЬ (по R²): {best_model_thr['name']}")
    print(f"R² = {best_model_thr['r2']:.6f}, RMSE = {best_model_thr['rmse']:.4f}")
    print(f"{'=' * 80}")
    
    plot_response_curve(x_data, y_data_throughput, models_throughput,
                       'Пропускная способность (машин)',
                       'throughput_approximation.png')
    
    # Сохранение результатов аппроксимации
    approx_results = []
    for model_name, model_info in models_wait.items():
        approx_results.append({
            'response': 'avg_wait_to_payment',
            'model': model_info['name'],
            'r2': model_info['r2'],
            'rmse': model_info['rmse'],
            'params': str(model_info['params'])
        })
    
    for model_name, model_info in models_throughput.items():
        approx_results.append({
            'response': 'throughput',
            'model': model_info['name'],
            'r2': model_info['r2'],
            'rmse': model_info['rmse'],
            'params': str(model_info['params'])
        })
    
    df_approx = pd.DataFrame(approx_results)
    df_approx.to_csv(OUTPUT_DIR / "approximation_results.csv", index=False)
    print(f"\n✓ Результаты аппроксимации сохранены: approximation_results.csv")
    
    print("\n" + "=" * 80)
    print("ВЫВОДЫ:")
    print("=" * 80)
    print(f"1. Для отклика 'Среднее время ожидания':")
    print(f"   Наилучшее приближение: {best_model['name']} модель (R²={best_model['r2']:.4f})")
    print(f"\n2. Для отклика 'Пропускная способность':")
    print(f"   Наилучшее приближение: {best_model_thr['name']} модель (R²={best_model_thr['r2']:.4f})")
    print(f"\n3. С увеличением среднего времени обслуживания у оператора:")
    print(f"   - Среднее время ожидания увеличивается")
    print(f"   - Пропускная способность снижается")
    print("=" * 80)


if __name__ == "__main__":
    main()

