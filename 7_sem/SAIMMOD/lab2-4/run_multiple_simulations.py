"""
Script to run gas station simulation multiple times with random seeds
and generate an Excel report with results and comprehensive visualizations
"""

import sys
import pandas as pd
import numpy as np
from datetime import datetime
import os
from multiprocessing import Pool, cpu_count

# Import the simulation module to use its run_simulation function
sys.path.insert(0, os.path.dirname(__file__))
from gas_station_simulation import run_simulation, CONFIG
from visualization import GasStationVisualizer
from wait_time_plots import create_all_wait_time_plots
from comprehensive_visualization import create_comprehensive_visualization


def run_single_simulation(args):
    """
    Helper function to run a single simulation with given seed and config
    This is used for parallel processing

    Args:
        args: tuple of (run_number, seed, config)

    Returns:
        tuple of (run_number, result dict)
    """
    run_num, seed, config = args

    print(f"  Запуск прогона {run_num} (seed={seed})...")

    # Run simulation with the seed
    result = run_simulation(seed=seed, config=config)

    # Add seed to result for tracking
    result['seed'] = seed

    print(f"  ✓ Прогон {run_num} завершен: {result['throughput']} авто, "
          f"ожидание {result['avg_wait_to_payment_s']:.1f}с, "
          f"утилизация {result['operator_util']:.1%}")

    return run_num, result


def run_simulations(num_runs=50, config=None, num_workers=None):
    """
    Run simulation multiple times in parallel with different random seeds

    Args:
        num_runs: Number of simulation runs
        config: Configuration dictionary (if None, uses default CONFIG)
        num_workers: Number of parallel workers (if None, uses CPU count)

    Returns:
        tuple: (DataFrame with results from all runs, timeseries dict from first run)
    """
    if config is None:
        config = CONFIG

    if num_workers is None:
        num_workers = min(cpu_count(), num_runs)

    print(f"Запуск {num_runs} симуляций в {num_workers} параллельных процессах...")
    print("=" * 60)

    # Generate random seeds for all runs
    np.random.seed()  # Ensure we get different seeds each time
    seeds = [np.random.randint(0, 2**31) for _ in range(num_runs)]

    # Первый прогон выполняем отдельно для сбора временных рядов
    print(f"  Запуск прогона 1 (seed={seeds[0]}) с временными рядами...")
    first_result, timeseries = run_simulation(seed=seeds[0], config=config, return_timeseries=True)
    first_result['seed'] = seeds[0]  # Add seed to result
    print(f"  ✓ Прогон 1 завершен: {first_result['throughput']} авто, "
          f"ожидание {first_result['avg_wait_to_payment_s']:.1f}с, "
          f"утилизация {first_result['operator_util']:.1%}")

    results = [first_result]

    # Prepare arguments for parallel execution (остальные прогоны)
    args_list = [(i+2, seeds[i+1], config) for i in range(num_runs-1)]

    if num_runs > 1:
        # Run simulations in parallel
        with Pool(processes=num_workers) as pool:
            # Use imap_unordered for better performance with progress feedback
            for run_num, result in pool.imap_unordered(run_single_simulation, args_list):
                results.append(result)

    # Sort results by run number (they may come back out of order)
    print("\n" + "=" * 60)
    print("Все прогоны завершены!")

    # Create DataFrame from results
    df = pd.DataFrame(results)
    return df, timeseries


def generate_excel_report(df, output_filename="simulation_results.xlsx"):
    """
    Generate Excel report with simulation results and statistics

    Args:
        df: DataFrame with simulation results
        output_filename: Name of the output Excel file
    """
    print(f"\nСоздание Excel отчета: {output_filename}")

    # Create Excel writer
    with pd.ExcelWriter(output_filename, engine='openpyxl') as writer:
        # Sheet 1: All runs data
        df.to_excel(writer, sheet_name='Все прогоны', index=False)

        # Sheet 2: Summary statistics
        summary_stats = pd.DataFrame({
            'Метрика': [],
            'Среднее': [],
            'Медиана': [],
            'Ст. откл.': [],
            'Мин': [],
            'Макс': []
        })

        metrics_to_analyze = [
            ('throughput', 'Пропускная способность (авто)'),
            ('avg_wait_to_payment_s', 'Среднее время ожидания (с)'),
            ('avg_fueling_dur_s', 'Среднее время заправки (с)'),
            ('avg_time_in_system_s', 'Среднее время в системе (с)'),
            ('operator_util', 'Утилизация оператора'),
            ('max_queue_length', 'Макс. длина очереди'),
        ]

        stats_data = []
        for col_name, display_name in metrics_to_analyze:
            if col_name in df.columns:
                stats_data.append({
                    'Метрика': display_name,
                    'Среднее': df[col_name].mean(),
                    'Медиана': df[col_name].median(),
                    'Ст. откл.': df[col_name].std(),
                    'Мин': df[col_name].min(),
                    'Макс': df[col_name].max()
                })

        summary_stats = pd.DataFrame(stats_data)
        summary_stats.to_excel(writer, sheet_name='Статистика', index=False)

        # Sheet 3: Column utilizations
        column_cols = [col for col in df.columns if col.startswith('column_') and col.endswith('_util')]
        if column_cols:
            column_utils = df[['seed'] + column_cols].copy()
            column_utils.columns = ['Seed'] + [f'Колонка {i}' for i in range(len(column_cols))]
            column_utils.to_excel(writer, sheet_name='Утилизация колонок', index=False)

            # Add average utilization
            avg_util = pd.DataFrame([{
                'Метрика': 'Средняя утилизация',
                **{f'Колонка {i}': df[col].mean() for i, col in enumerate(column_cols)}
            }])
            avg_util.to_excel(writer, sheet_name='Утилизация колонок',
                            index=False, startrow=len(column_utils)+2)

        # Sheet 4: Configuration used
        config_data = []
        for key, value in CONFIG.items():
            config_data.append({
                'Параметр': key,
                'Значение': value
            })
        config_df = pd.DataFrame(config_data)
        config_df.to_excel(writer, sheet_name='Конфигурация', index=False)

        # Sheet 5: Metadata
        metadata = pd.DataFrame([{
            'Дата создания': datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
            'Количество прогонов': len(df),
            'Время симуляции (с)': CONFIG['simulation_time'],
            'Количество колонок': CONFIG['num_columns_each_side'] * 2,
        }])
        metadata.to_excel(writer, sheet_name='Метаданные', index=False)

    print(f"✓ Excel отчет сохранен: {output_filename}")

    # Print summary to console
    print("\n" + "=" * 60)
    print("СВОДНАЯ СТАТИСТИКА")
    print("=" * 60)
    print(summary_stats.to_string(index=False))
    print("=" * 60)


def main():
    """Main function to run simulations and generate report"""

    # Configuration
    num_runs = 50  # Number of simulation runs (default changed to 50)
    output_file = "simulation_results.xlsx"

    # Check if custom number of runs specified via command line
    if len(sys.argv) > 1:
        try:
            num_runs = int(sys.argv[1])
        except ValueError:
            print(f"Ошибка: '{sys.argv[1]}' не является числом. Используем значение по умолчанию: {num_runs}")

    if len(sys.argv) > 2:
        output_file = sys.argv[2]

    print("╔" + "═" * 58 + "╗")
    print("║" + " " * 10 + "ЗАПУСК МНОЖЕСТВЕННЫХ СИМУЛЯЦИЙ" + " " * 18 + "║")
    print("╚" + "═" * 58 + "╝")
    print(f"\nКоличество прогонов: {num_runs}")
    print(f"Файл результатов: {output_file}")
    print()

    # Создаем папку для графиков
    plots_dir = "графики"
    if not os.path.exists(plots_dir):
        os.makedirs(plots_dir)
        print(f"✓ Создана папка: {plots_dir}")

    # Run simulations
    results_df, timeseries = run_simulations(num_runs=num_runs, config=CONFIG)

    # Generate Excel report
    generate_excel_report(results_df, output_filename=output_file)

    # Generate visualizations
    print("\n" + "=" * 60)
    print("СОЗДАНИЕ ВИЗУАЛИЗАЦИЙ")
    print("=" * 60)

    # 1. Комплексный график (16 подграфиков)
    visualizer = GasStationVisualizer(results_df, timeseries)
    visualizer.create_comprehensive_plot(os.path.join(plots_dir, 'gas_station_comprehensive.png'))

    # 2. Полная объединенная визуализация (20 графиков) - как в супермаркете
    num_columns = CONFIG['num_columns_each_side'] * 2
    create_comprehensive_visualization(results_df, timeseries, num_columns,
                                      os.path.join(plots_dir, 'comprehensive_gas_station_results.png'))

    # 3. Детальные графики времени ожидания по колонкам
    print("\n" + "=" * 60)
    print("СОЗДАНИЕ ДЕТАЛЬНЫХ ГРАФИКОВ ВРЕМЕНИ ОЖИДАНИЯ")
    print("=" * 60)
    create_all_wait_time_plots(results_df, num_columns, timeseries, plots_dir)

    print("\n✓ Все операции завершены успешно!")
    print(f"\nСозданные файлы:")
    print(f"  - {output_file} (Excel отчет)")
    print(f"  - {plots_dir}/gas_station_comprehensive.png (комплексный график 16)")
    print(f"  - {plots_dir}/comprehensive_gas_station_results.png (полная визуализация 20)")
    print(f"  - {plots_dir}/wait_time_by_columns.png (графики по колонкам)")
    print(f"  - {plots_dir}/wait_time_comparison_columns.png (сравнительный график)")
    print(f"  - {plots_dir}/operator_queue_analysis.png (анализ очереди)")
    print(f"  - {plots_dir}/column_utilization_heatmap.png (тепловая карта)")
    print(f"  - {plots_dir}/wait_time_timeseries.png (временные ряды метрик)")


if __name__ == "__main__":
    main()

