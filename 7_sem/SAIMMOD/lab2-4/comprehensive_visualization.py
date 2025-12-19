"""
Модуль для создания объединенной визуализации всех графиков АЗС в одном файле
По аналогии с comprehensive visualization супермаркета
"""

import matplotlib.pyplot as plt
import matplotlib
matplotlib.use('Agg')
import pandas as pd
import numpy as np
import sys
import os

sys.path.insert(0, os.path.dirname(__file__))
from gas_station_simulation import run_simulation, CONFIG


# ============================================================================
# ОБЪЕДИНЕННАЯ ВИЗУАЛИЗАЦИЯ ВСЕХ ГРАФИКОВ
# ============================================================================

def create_comprehensive_visualization(results_df, timeseries_data, num_columns, output_path="comprehensive_gas_station_results.png"):
    """Создание полной визуализации всех графиков АЗС в одном файле"""
    
    print("=" * 80)
    print("СОЗДАНИЕ ПОЛНОЙ ВИЗУАЛИЗАЦИИ ВСЕХ ГРАФИКОВ АЗС")
    print("=" * 80)
    
    # Создаем большую сетку графиков: 5 строк, 4 столбца = 20 графиков
    fig = plt.figure(figsize=(24, 30))
    fig.suptitle('Полная визуализация результатов симуляции АЗС',
                 fontsize=20, weight='bold', y=0.98)
    
    # График 1: Пропускная способность по репликациям
    ax1 = plt.subplot(5, 4, 1)
    ax1.bar(results_df.index, results_df['throughput'],
            color='green', alpha=0.7, edgecolor='black', linewidth=0.5)
    mean_val = results_df['throughput'].mean()
    ax1.axhline(y=mean_val, color='red', linestyle='--', linewidth=2,
               label=f'Среднее: {mean_val:.1f}')
    ax1.set_title('Пропускная способность', fontsize=12, weight='bold')
    ax1.set_xlabel('Репликация')
    ax1.set_ylabel('Количество авто')
    ax1.legend(fontsize=8)
    ax1.grid(True, alpha=0.3, axis='y')

    # График 2: Среднее время ожидания оплаты
    ax2 = plt.subplot(5, 4, 2)
    ax2.scatter(results_df.index, results_df['avg_wait_to_payment_s'],
               marker='s', color='blue', s=60, alpha=0.7, edgecolors='black', linewidth=0.5)
    mean_val = results_df['avg_wait_to_payment_s'].mean()
    ax2.axhline(y=mean_val, color='red', linestyle='--', linewidth=2,
               label=f'Среднее: {mean_val:.1f}с')
    ax2.set_title('Среднее время ожидания оплаты', fontsize=12, weight='bold')
    ax2.set_xlabel('Репликация')
    ax2.set_ylabel('Секунды')
    ax2.legend(fontsize=8)
    ax2.grid(True, alpha=0.3)
    
    # График 3: Загрузка колонок
    ax3 = plt.subplot(5, 4, 3)
    for i in range(num_columns):
        util_col = f'column_{i}_util'
        if util_col in results_df.columns:
            ax3.scatter(results_df.index, results_df[util_col] * 100,
                       marker='o', label=f'К{i}', s=40, alpha=0.7)
    ax3.set_title('Загрузка колонок', fontsize=12, weight='bold')
    ax3.set_xlabel('Репликация')
    ax3.set_ylabel('Процент (%)')
    ax3.legend(fontsize=8, ncol=2)
    ax3.grid(True, alpha=0.3)
    
    # График 4: Корреляции метрик
    ax4 = plt.subplot(5, 4, 4)
    correlation_cols = ['throughput', 'avg_wait_to_payment_s', 'operator_util']
    correlation_cols = [col for col in correlation_cols if col in results_df.columns]
    if len(correlation_cols) >= 2:
        corr = results_df[correlation_cols].corr().round(2)
        
        im = ax4.imshow(corr, cmap='coolwarm', aspect='auto', vmin=-1, vmax=1)
        ax4.set_xticks(range(len(correlation_cols)))
        ax4.set_yticks(range(len(correlation_cols)))
        ax4.set_xticklabels(['Пропускн.', 'Ожидание', 'Оператор'], rotation=45, ha='right', fontsize=8)
        ax4.set_yticklabels(['Пропускн.', 'Ожидание', 'Оператор'], fontsize=8)
        ax4.set_title('Корреляции метрик', fontsize=12, weight='bold')
        
        # Добавляем значения корреляций
        for i in range(len(correlation_cols)):
            for j in range(len(correlation_cols)):
                color = 'white' if abs(corr.iloc[i, j]) > 0.5 else 'black'
                ax4.text(j, i, f'{corr.iloc[i, j]:.2f}', ha='center', va='center',
                        color=color, fontsize=10)
        
        plt.colorbar(im, ax=ax4, shrink=0.8)
    
    # Графики 5-8: Временные ряды - пропускная способность, оператор, очередь, топливо (СТУПЕНЧАТЫЕ)
    ax5 = plt.subplot(5, 4, 5)
    if 'throughput_timeseries' in timeseries_data and not timeseries_data['throughput_timeseries'].empty:
        df = timeseries_data['throughput_timeseries']
        ax5.step(df['time'], df['value'], where='post', linewidth=2,
                alpha=0.8, color='green', label='Пропускная способность')
        ax5.set_title('Пропускная способность\n(ступенчатый)', fontsize=10, weight='bold')
        ax5.set_xlabel('Время (с)')
        ax5.set_ylabel('Кум. авто')
        ax5.legend(fontsize=8)
        ax5.grid(True, alpha=0.3)
    
    ax6 = plt.subplot(5, 4, 6)
    if 'operator_state_timeseries' in timeseries_data and not timeseries_data['operator_state_timeseries'].empty:
        df = timeseries_data['operator_state_timeseries']
        ax6.step(df['time'], df['value'], where='post', linewidth=2,
                alpha=0.8, color='blue', label='Состояние')
        ax6.set_title('Состояние оператора\n(ступенчатый)', fontsize=10, weight='bold')
        ax6.set_xlabel('Время (с)')
        ax6.set_ylabel('Занят (0/1)')
        ax6.set_ylim(-0.1, 1.1)
        ax6.legend(fontsize=8)
        ax6.grid(True, alpha=0.3)
    
    ax7 = plt.subplot(5, 4, 7)
    if 'queue_length_timeseries' in timeseries_data and not timeseries_data['queue_length_timeseries'].empty:
        df = timeseries_data['queue_length_timeseries']
        ax7.scatter(df['time'], df['value'], alpha=0.6, color='orange', s=20, label='Длина')
        if 'moving_avg' in df.columns:
            ax7.plot(df['time'], df['moving_avg'], alpha=0.9, color='red',
                    linewidth=2, label='Скольз. ср.')
        ax7.set_title('Длина очереди\n(дискретный)', fontsize=10, weight='bold')
        ax7.set_xlabel('Время (с)')
        ax7.set_ylabel('Длина')
        ax7.legend(fontsize=8)
        ax7.grid(True, alpha=0.3)
    
    ax8 = plt.subplot(5, 4, 8)
    if 'fuel_cumulative_timeseries' in timeseries_data and not timeseries_data['fuel_cumulative_timeseries'].empty:
        df = timeseries_data['fuel_cumulative_timeseries']
        ax8.step(df['time'], df['value'], where='post', linewidth=2,
                alpha=0.8, color='darkgreen', label='Топливо')
        ax8.set_title('Накопленное топливо\n(ступенчатый)', fontsize=10, weight='bold')
        ax8.set_xlabel('Время (с)')
        ax8.set_ylabel('Литры')
        ax8.legend(fontsize=8)
        ax8.grid(True, alpha=0.3)
    
    # Графики 9-12: Загрузка колонок по репликациям (ДИСКРЕТНЫЕ)
    for i in range(min(4, num_columns)):
        ax = plt.subplot(5, 4, 9 + i)
        util_col = f'column_{i}_util'
        if util_col in results_df.columns:
            util_values = results_df[util_col] * 100
            
            # Дискретный отклик - точечный график
            ax.scatter(results_df.index, util_values,
                      marker='o', color=plt.cm.Set3(i / num_columns),
                      s=60, alpha=0.7, edgecolors='black', linewidth=0.5)
            
            # Линия среднего
            mean_util = util_values.mean()
            ax.axhline(y=mean_util, color='red', linestyle='--', linewidth=2,
                      label=f'Ср: {mean_util:.1f}%')
            
            ax.set_title(f'Колонка {i}\nУтилизация', fontsize=10, weight='bold')
            ax.set_xlabel('Репликация')
            ax.set_ylabel('Утилизация (%)')
            ax.legend(fontsize=8)
            ax.grid(True, alpha=0.3)
            
            # Статистика
            stats_text = f'Мин: {util_values.min():.1f}%\nМакс: {util_values.max():.1f}%'
            ax.text(0.02, 0.98, stats_text, transform=ax.transAxes,
                   verticalalignment='top',
                   bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.8), fontsize=8)
    
    # Графики 13-17: Метрики по репликациям
    # График 13: Утилизация оператора
    ax13 = plt.subplot(5, 4, 13)
    operator_util = results_df['operator_util'] * 100
    ax13.scatter(results_df.index, operator_util,
                marker='D', color='purple', s=60, alpha=0.7, edgecolors='black', linewidth=0.5)
    mean_val = operator_util.mean()
    ax13.axhline(y=mean_val, color='red', linestyle='--', linewidth=2,
                label=f'Среднее: {mean_val:.1f}%')
    ax13.set_title('Утилизация оператора', fontsize=10, weight='bold')
    ax13.set_xlabel('Репликация')
    ax13.set_ylabel('Процент (%)')
    ax13.legend(fontsize=8)
    ax13.grid(True, alpha=0.3)
    
    # График 14: Максимальная длина очереди
    ax14 = plt.subplot(5, 4, 14)
    ax14.scatter(results_df.index, results_df['max_queue_length'],
                marker='^', color='orange', s=60, alpha=0.7, edgecolors='black', linewidth=0.5)
    mean_val = results_df['max_queue_length'].mean()
    ax14.axhline(y=mean_val, color='red', linestyle='--', linewidth=2,
                label=f'Среднее: {mean_val:.1f}')
    ax14.set_title('Макс. длина очереди', fontsize=10, weight='bold')
    ax14.set_xlabel('Репликация')
    ax14.set_ylabel('Авто')
    ax14.legend(fontsize=8)
    ax14.grid(True, alpha=0.3)
    
    # График 15: Время в системе
    ax15 = plt.subplot(5, 4, 15)
    ax15.scatter(results_df.index, results_df['avg_time_in_system_s'],
                marker='o', color='teal', s=60, alpha=0.7, edgecolors='black', linewidth=0.5)
    mean_val = results_df['avg_time_in_system_s'].mean()
    ax15.axhline(y=mean_val, color='red', linestyle='--', linewidth=2,
                label=f'Среднее: {mean_val:.1f}с')
    ax15.set_title('Время в системе', fontsize=10, weight='bold')
    ax15.set_xlabel('Репликация')
    ax15.set_ylabel('Секунды')
    ax15.legend(fontsize=8)
    ax15.grid(True, alpha=0.3)
    
    # График 17: Boxplot распределения загрузки колонок
    ax17 = plt.subplot(5, 4, 17)
    ax17.scatter(results_df.index, results_df['avg_fueling_dur_s'],
                marker='p', color='brown', s=60, alpha=0.7, edgecolors='black', linewidth=0.5)
    mean_val = results_df['avg_fueling_dur_s'].mean()
    ax17.axhline(y=mean_val, color='red', linestyle='--', linewidth=2,
                label=f'Среднее: {mean_val:.1f}с')
    ax17.set_title('Время заправки', fontsize=10, weight='bold')
    ax17.set_xlabel('Репликация')
    ax17.set_ylabel('Секунды')
    ax17.legend(fontsize=8)
    ax17.grid(True, alpha=0.3)
    
    # Графики 18-20: Распределения (гистограммы)
    # График 18: Распределение времени ожидания
    ax18 = plt.subplot(5, 4, 18)
    ax18.hist(results_df['avg_wait_to_payment_s'], bins=15,
             color='skyblue', alpha=0.7, edgecolor='black')
    mean_val = results_df['avg_wait_to_payment_s'].mean()
    ax18.axvline(mean_val, color='red', linestyle='--', linewidth=2,
                label=f'Среднее: {mean_val:.1f}с')
    ax18.set_title('Распределение\nвремени ожидания', fontsize=10, weight='bold')
    ax18.set_xlabel('Секунды')
    ax18.set_ylabel('Частота')
    ax18.legend(fontsize=8)
    ax18.grid(True, alpha=0.3)
    
    # График 19: Распределение времени заправки
    ax19 = plt.subplot(5, 4, 19)
    ax19.hist(results_df['avg_fueling_dur_s'], bins=15,
             color='lightcoral', alpha=0.7, edgecolor='black')
    mean_val = results_df['avg_fueling_dur_s'].mean()
    ax19.axvline(mean_val, color='red', linestyle='--', linewidth=2,
                label=f'Среднее: {mean_val:.1f}с')
    ax19.set_title('Распределение\nвремени заправки', fontsize=10, weight='bold')
    ax19.set_xlabel('Секунды')
    ax19.set_ylabel('Частота')
    ax19.legend(fontsize=8)
    ax19.grid(True, alpha=0.3)
    
    # График 20: Заполненность колонок (stacked area)
    ax20 = plt.subplot(5, 4, 20)
    if 'columns_occupancy_timeseries' in timeseries_data and not timeseries_data['columns_occupancy_timeseries'].empty:
        df = timeseries_data['columns_occupancy_timeseries']
        if 'columns' in df.columns and len(df) > 0:
            times = df['time'].values
            cols_data = np.array(df['columns'].tolist()).T
            
            colors = plt.cm.Set3(np.linspace(0, 1, num_columns))
            ax20.stackplot(times, *cols_data, labels=[f'К{i}' for i in range(num_columns)],
                          colors=colors, alpha=0.7)
            
            ax20.set_title('Заполненность колонок\n(stacked area)', fontsize=10, weight='bold')
            ax20.set_xlabel('Время (с)')
            ax20.set_ylabel('Авто')
            ax20.legend(fontsize=7, ncol=2, loc='upper left')
            ax20.grid(True, alpha=0.3)
    
    plt.tight_layout()
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()
    
    print(f"✓ Полная визуализация сохранена в файл: {output_path}")
    
    # Дополнительная статистика
    print(f"\n{'=' * 60}")
    print("СТАТИСТИКА ЗАГРУЗКИ КОЛОНОК:")
    print(f"{'=' * 60}")
    
    for i in range(num_columns):
        util_col = f'column_{i}_util'
        if util_col in results_df.columns:
            util_values = results_df[util_col] * 100
            print(f"\nКолонка {i}:")
            print(f"  Средняя утилизация: {util_values.mean():.2f}%")
            print(f"  Минимальная: {util_values.min():.2f}%")
            print(f"  Максимальная: {util_values.max():.2f}%")
            print(f"  Стандартное отклонение: {util_values.std():.2f}%")


def run_comprehensive_simulation(num_replications=20):
    """Запуск полной симуляции с объединенной визуализацией"""
    
    print("=" * 80)
    print("ЗАПУСК ПОЛНОЙ СИМУЛЯЦИИ АЗС С ОБЪЕДИНЕННОЙ ВИЗУАЛИЗАЦИЕЙ")
    print("=" * 80)
    
    print(f"\nПараметры симуляции:")
    print(f"  Время моделирования: {CONFIG['simulation_time']} сек ({CONFIG['simulation_time']/60:.0f} мин)")
    print(f"  Средний интервал прибытия: {CONFIG['arrival_mean']} сек")
    print(f"  Количество колонок: {CONFIG['num_columns_each_side'] * 2}")
    print(f"  Время оплаты: {CONFIG['payment_mu']} ± {CONFIG['payment_sigma']} сек")
    print(f"  Количество репликаций: {num_replications}")
    
    # Запуск множественных симуляций
    print(f"\n{'=' * 60}")
    print("ЗАПУСК СИМУЛЯЦИИ...")
    print(f"{'=' * 60}")
    
    results = []
    np.random.seed()
    seeds = [np.random.randint(0, 2**31) for _ in range(num_replications)]
    
    # Первый прогон с временными рядами
    print(f"  Запуск репликации 1/{num_replications} (seed={seeds[0]}) с временными рядами...")
    first_result, timeseries = run_simulation(seed=seeds[0], config=CONFIG, return_timeseries=True)
    first_result['replication_id'] = 0
    results.append(first_result)
    
    # Остальные прогоны
    for i in range(1, num_replications):
        print(f"  Запуск репликации {i+1}/{num_replications} (seed={seeds[i]})...")
        result = run_simulation(seed=seeds[i], config=CONFIG)
        result['replication_id'] = i
        results.append(result)
    
    results_df = pd.DataFrame(results)
    
    print(f"\n{'=' * 60}")
    print("ВСЕ РЕПЛИКАЦИИ ЗАВЕРШЕНЫ!")
    print(f"{'=' * 60}")
    
    # Вывод сводной статистики
    print(f"\n{'=' * 60}")
    print("СВОДНАЯ СТАТИСТИКА:")
    print(f"{'=' * 60}")
    
    summary_stats = {
        'Пропускная способность': results_df['throughput'].mean(),
        'Время ожидания (с)': results_df['avg_wait_to_payment_s'].mean(),
        'Время заправки (с)': results_df['avg_fueling_dur_s'].mean(),
        'Время в системе (с)': results_df['avg_time_in_system_s'].mean(),
        'Утилизация оператора (%)': results_df['operator_util'].mean() * 100,
        'Макс. длина очереди': results_df['max_queue_length'].mean()
    }
    
    for metric, value in summary_stats.items():
        print(f"  {metric}: {value:.2f}")
    
    # Создание папки для графиков если её нет
    plots_dir = "графики"
    if not os.path.exists(plots_dir):
        os.makedirs(plots_dir)
    
    # Создание полной визуализации
    num_columns = CONFIG['num_columns_each_side'] * 2
    output_path = os.path.join(plots_dir, 'comprehensive_gas_station_results.png')
    create_comprehensive_visualization(results_df, timeseries, num_columns, output_path)
    
    print(f"\n{'=' * 80}")
    print("СИМУЛЯЦИЯ ЗАВЕРШЕНА!")
    print(f"{'=' * 80}")
    print("Созданные файлы:")
    print(f"  📊 {output_path} - Полная визуализация (20 графиков)")
    print(f"{'=' * 80}")
    
    return results_df, timeseries


if __name__ == "__main__":
    # Запуск полной симуляции
    results_df, timeseries = run_comprehensive_simulation(num_replications=20)
    
    print(f"\n{'=' * 80}")
    print("ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА УСПЕШНО!")
    print(f"{'=' * 80}")

