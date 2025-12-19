"""
Модуль для создания детальных графиков времени ожидания в очередях на колонках АЗС
По аналогии с графиками времени ожидания в отделах супермаркета
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
# ОТДЕЛЬНЫЕ ГРАФИКИ ВРЕМЕНИ ОЖИДАНИЯ ПО КОЛОНКАМ
# ============================================================================

def create_wait_time_plots(results_df, num_columns, plots_dir="."):
    """Создание отдельных графиков времени ожидания по колонкам"""

    print("=" * 80)
    print("СОЗДАНИЕ ОТДЕЛЬНЫХ ГРАФИКОВ ВРЕМЕНИ ОЖИДАНИЯ ПО КОЛОНКАМ")
    print("=" * 80)

    # Создаем сетку графиков: 2 строки, 3 столбца для 6 колонок
    fig, axes = plt.subplots(2, 3, figsize=(18, 12))
    fig.suptitle('Время ожидания в очередях по колонкам АЗС (дискретные значения)',
                 fontsize=16, weight='bold')

    # Сглаживаем массив осей для удобства итерации
    axes_flat = axes.flatten()

    for col_idx in range(num_columns):
        ax = axes_flat[col_idx]
        column_name = f"Колонка {col_idx}"

        # Получаем данные времени ожидания для колонки
        # В нашей симуляции нет прямой статистики по колонкам,
        # но можем показать утилизацию как аналог
        util_col = f'column_{col_idx}_util'

        if util_col in results_df.columns:
            util_values = results_df[util_col] * 100  # Конвертируем в проценты

            # Создаем точечный график для дискретных значений
            ax.scatter(results_df.index, util_values,
                      marker='o', color=plt.cm.Set3(col_idx / num_columns),
                      s=60, alpha=0.7, edgecolors='black', linewidth=0.5)

            # Добавляем линию среднего значения
            mean_util = util_values.mean()
            ax.axhline(y=mean_util, color='red', linestyle='--', linewidth=2,
                      label=f'Среднее: {mean_util:.2f}%')

            # Настройка графика
            ax.set_title(f'{column_name}\nУтилизация (загрузка)',
                        fontsize=12, weight='bold')
            ax.set_xlabel('Номер репликации')
            ax.set_ylabel('Утилизация (%)')
            ax.grid(True, alpha=0.3)
            ax.legend(fontsize=10)

            # Добавляем статистику
            stats_text = f'Мин: {util_values.min():.1f}%\nМакс: {util_values.max():.1f}%\nСтд: {util_values.std():.2f}%'
            ax.text(0.02, 0.98, stats_text, transform=ax.transAxes,
                   verticalalignment='top',
                   bbox=dict(boxstyle='round', facecolor='wheat', alpha=0.8))
        else:
            ax.text(0.5, 0.5, 'Нет данных', ha='center', va='center',
                   transform=ax.transAxes)
            ax.set_title(f'{column_name}\nНет данных',
                        fontsize=12, weight='bold')

    plt.tight_layout()
    output_path = os.path.join(plots_dir, 'wait_time_by_columns.png')
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()

    print(f"✓ Графики времени ожидания по колонкам сохранены в файл: {output_path}")

    # Дополнительная статистика
    print(f"\n{'=' * 60}")
    print("СТАТИСТИКА ЗАГРУЗКИ ПО КОЛОНКАМ:")
    print(f"{'=' * 60}")

    for col_idx in range(num_columns):
        util_col = f'column_{col_idx}_util'
        if util_col in results_df.columns:
            util_values = results_df[util_col] * 100
            print(f"\nКолонка {col_idx}:")
            print(f"  Средняя утилизация: {util_values.mean():.2f}%")
            print(f"  Минимальная: {util_values.min():.2f}%")
            print(f"  Максимальная: {util_values.max():.2f}%")
            print(f"  Стандартное отклонение: {util_values.std():.2f}%")
            print(f"  Медиана: {util_values.median():.2f}%")


def create_wait_time_comparison_plot(results_df, num_columns, plots_dir="."):
    """Создание сравнительного графика времени ожидания по колонкам"""

    print(f"\n{'=' * 60}")
    print("СОЗДАНИЕ СРАВНИТЕЛЬНОГО ГРАФИКА ЗАГРУЗКИ КОЛОНОК")
    print(f"{'=' * 60}")

    # Собираем данные по всем колонкам
    util_data = []
    column_labels = []

    for col_idx in range(num_columns):
        util_col = f'column_{col_idx}_util'
        if util_col in results_df.columns:
            util_data.append(results_df[util_col].values * 100)
            column_labels.append(f'К{col_idx}')

    if not util_data:
        print("❌ Нет данных для создания графика")
        return

    # Создаем box plot для сравнения
    fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(16, 8))

    # Box plot
    bp = ax1.boxplot(util_data, labels=column_labels, patch_artist=True)

    # Раскрашиваем коробки
    colors = plt.cm.Set3(np.linspace(0, 1, len(util_data)))
    for patch, color in zip(bp['boxes'], colors):
        patch.set_facecolor(color)
        patch.set_alpha(0.7)

    ax1.set_title('Распределение загрузки по колонкам',
                 fontsize=14, weight='bold')
    ax1.set_xlabel('Колонки')
    ax1.set_ylabel('Утилизация (%)')
    ax1.grid(True, alpha=0.3)

    # Bar plot средних значений
    means = [np.mean(data) for data in util_data]
    stds = [np.std(data) for data in util_data]

    bars = ax2.bar(range(len(column_labels)), means, yerr=stds,
                   color=colors, alpha=0.7, capsize=5, edgecolor='black')

    ax2.set_title('Средняя загрузка по колонкам',
                 fontsize=14, weight='bold')
    ax2.set_xlabel('Колонки')
    ax2.set_ylabel('Средняя утилизация (%)')
    ax2.set_xticks(range(len(column_labels)))
    ax2.set_xticklabels(column_labels)
    ax2.grid(True, alpha=0.3, axis='y')

    # Добавляем значения на столбцы
    for i, (bar, mean, std) in enumerate(zip(bars, means, stds)):
        ax2.text(bar.get_x() + bar.get_width()/2,
                bar.get_height() + std + 1,
                f'{mean:.1f}±{std:.1f}',
                ha='center', va='bottom', fontsize=10)

    plt.tight_layout()
    output_path = os.path.join(plots_dir, 'wait_time_comparison_columns.png')
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()

    print(f"✓ Сравнительный график сохранен в файл: {output_path}")


def create_operator_queue_analysis(results_df, plots_dir="."):
    """Анализ очереди к оператору"""

    print(f"\n{'=' * 60}")
    print("СОЗДАНИЕ АНАЛИЗА ОЧЕРЕДИ К ОПЕРАТОРУ")
    print(f"{'=' * 60}")

    fig, axes = plt.subplots(2, 2, figsize=(16, 12))
    fig.suptitle('Анализ очереди к оператору АЗС', fontsize=16, weight='bold')

    # График 1: Среднее время ожидания оплаты
    ax1 = axes[0, 0]
    ax1.scatter(results_df.index, results_df['avg_wait_to_payment_s'],
               marker='o', color='blue', s=60, alpha=0.7,
               edgecolors='black', linewidth=0.5)
    mean_wait = results_df['avg_wait_to_payment_s'].mean()
    ax1.axhline(y=mean_wait, color='red', linestyle='--', linewidth=2,
               label=f'Среднее: {mean_wait:.1f}с')
    ax1.set_title('Среднее время ожидания оплаты', fontsize=12, weight='bold')
    ax1.set_xlabel('Номер репликации')
    ax1.set_ylabel('Время ожидания (секунды)')
    ax1.grid(True, alpha=0.3)
    ax1.legend()

    # График 2: Максимальная длина очереди
    ax2 = axes[0, 1]
    ax2.scatter(results_df.index, results_df['max_queue_length'],
               marker='^', color='orange', s=60, alpha=0.7,
               edgecolors='black', linewidth=0.5)
    mean_queue = results_df['max_queue_length'].mean()
    ax2.axhline(y=mean_queue, color='red', linestyle='--', linewidth=2,
               label=f'Среднее: {mean_queue:.1f}')
    ax2.set_title('Максимальная длина очереди', fontsize=12, weight='bold')
    ax2.set_xlabel('Номер репликации')
    ax2.set_ylabel('Длина очереди (авто)')
    ax2.grid(True, alpha=0.3)
    ax2.legend()

    # График 3: Утилизация оператора
    ax3 = axes[1, 0]
    operator_util = results_df['operator_util'] * 100
    ax3.scatter(results_df.index, operator_util,
               marker='s', color='purple', s=60, alpha=0.7,
               edgecolors='black', linewidth=0.5)
    mean_util = operator_util.mean()
    ax3.axhline(y=mean_util, color='red', linestyle='--', linewidth=2,
               label=f'Среднее: {mean_util:.1f}%')
    ax3.set_title('Утилизация оператора', fontsize=12, weight='bold')
    ax3.set_xlabel('Номер репликации')
    ax3.set_ylabel('Утилизация (%)')
    ax3.grid(True, alpha=0.3)
    ax3.legend()

    # График 4: Распределение времени ожидания
    ax4 = axes[1, 1]
    ax4.hist(results_df['avg_wait_to_payment_s'], bins=15,
            color='skyblue', alpha=0.7, edgecolor='black')
    ax4.axvline(mean_wait, color='red', linestyle='--', linewidth=2,
               label=f'Среднее: {mean_wait:.1f}с')
    ax4.set_title('Распределение времени ожидания', fontsize=12, weight='bold')
    ax4.set_xlabel('Время ожидания (секунды)')
    ax4.set_ylabel('Частота')
    ax4.legend()
    ax4.grid(True, alpha=0.3)

    plt.tight_layout()
    output_path = os.path.join(plots_dir, 'operator_queue_analysis.png')
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()

    print(f"✓ Анализ очереди к оператору сохранен в файл: {output_path}")

    # Статистика
    print(f"\n{'=' * 60}")
    print("СТАТИСТИКА ОЧЕРЕДИ К ОПЕРАТОРУ:")
    print(f"{'=' * 60}")
    print(f"Среднее время ожидания: {mean_wait:.2f} ± {results_df['avg_wait_to_payment_s'].std():.2f} сек")
    print(f"Минимальное: {results_df['avg_wait_to_payment_s'].min():.2f} сек")
    print(f"Максимальное: {results_df['avg_wait_to_payment_s'].max():.2f} сек")
    print(f"\nСредняя макс. длина очереди: {mean_queue:.2f} авто")
    print(f"Средняя утилизация оператора: {mean_util:.2f}%")


def create_column_utilization_heatmap(results_df, num_columns, plots_dir="."):
    """Создание тепловой карты загрузки колонок по репликациям"""

    print(f"\n{'=' * 60}")
    print("СОЗДАНИЕ ТЕПЛОВОЙ КАРТЫ ЗАГРУЗКИ КОЛОНОК")
    print(f"{'=' * 60}")

    # Собираем данные утилизации колонок
    util_cols = [f'column_{i}_util' for i in range(num_columns)]
    util_data = results_df[util_cols].values * 100  # В процентах

    fig, ax = plt.subplots(figsize=(12, 8))

    # Создаем heatmap
    im = ax.imshow(util_data.T, cmap='YlOrRd', aspect='auto', vmin=0, vmax=100)

    # Настройка осей
    ax.set_xticks(np.arange(len(results_df)))
    ax.set_yticks(np.arange(num_columns))
    ax.set_xticklabels(results_df.index)
    ax.set_yticklabels([f'Колонка {i}' for i in range(num_columns)])

    ax.set_xlabel('Номер репликации', fontsize=12)
    ax.set_ylabel('Колонки', fontsize=12)
    ax.set_title('Тепловая карта загрузки колонок по репликациям (%)',
                fontsize=14, weight='bold')

    # Добавляем colorbar
    cbar = plt.colorbar(im, ax=ax)
    cbar.set_label('Утилизация (%)', rotation=270, labelpad=20)

    # Добавляем значения в ячейки
    for i in range(len(results_df)):
        for j in range(num_columns):
            text = ax.text(i, j, f'{util_data[i, j]:.0f}',
                          ha="center", va="center", color="black", fontsize=8)

    plt.tight_layout()
    output_path = os.path.join(plots_dir, 'column_utilization_heatmap.png')
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()

    print(f"✓ Тепловая карта сохранена в файл: {output_path}")


def create_wait_time_timeseries_plots(timeseries_data, num_columns, plots_dir="."):
    """Создание временных рядов метрик АЗС в модельном времени"""

    print(f"\n{'=' * 60}")
    print("СОЗДАНИЕ ВРЕМЕННЫХ РЯДОВ МЕТРИК В МОДЕЛЬНОМ ВРЕМЕНИ")
    print(f"{'=' * 60}")

    # Создаем сетку графиков: 3 строки, 2 столбца
    fig, axes = plt.subplots(3, 2, figsize=(18, 16))
    fig.suptitle('Временные ряды метрик АЗС в модельном времени',
                 fontsize=16, weight='bold')

    # График 1: Пропускная способность (ступенчатый)
    ax1 = axes[0, 0]
    if 'throughput_timeseries' in timeseries_data and not timeseries_data['throughput_timeseries'].empty:
        df = timeseries_data['throughput_timeseries']
        ax1.step(df['time'], df['value'], where='post', linewidth=2,
                alpha=0.8, color='green', label='Пропускная способность')
        ax1.set_title('Пропускная способность во времени\n(ступенчатый отклик)',
                     fontsize=12, weight='bold')
        ax1.set_xlabel('Время (секунды)')
        ax1.set_ylabel('Кумулятивное количество авто')
        ax1.legend(fontsize=10)
        ax1.grid(True, alpha=0.3)

        # Статистика
        if len(df) > 0:
            stats_text = f'Всего точек: {len(df)}\nИтого авто: {df["value"].iloc[-1]:.0f}'
            ax1.text(0.02, 0.98, stats_text, transform=ax1.transAxes,
                    verticalalignment='top',
                    bbox=dict(boxstyle='round', facecolor='lightgreen', alpha=0.8))
    else:
        ax1.text(0.5, 0.5, 'Нет данных', ha='center', va='center', transform=ax1.transAxes)

    # График 2: Состояние оператора (ступенчатый)
    ax2 = axes[0, 1]
    if 'operator_state_timeseries' in timeseries_data and not timeseries_data['operator_state_timeseries'].empty:
        df = timeseries_data['operator_state_timeseries']
        ax2.step(df['time'], df['value'], where='post', linewidth=1.5,
                alpha=0.8, color='blue', label='Состояние')
        ax2.set_title('Состояние оператора во времени\n(ступенчатый отклик)',
                     fontsize=12, weight='bold')
        ax2.set_xlabel('Время (секунды)')
        ax2.set_ylabel('Состояние (0=свободен, 1=занят)')
        ax2.set_ylim(-0.1, 1.1)
        ax2.legend(fontsize=10)
        ax2.grid(True, alpha=0.3)

        # Статистика
        if len(df) > 0:
            busy_time = df[df['value'] == 1]['time'].count()
            total_time = df['time'].iloc[-1] if len(df) > 0 else 0
            stats_text = f'Точек: {len(df)}\nЗанят: {busy_time} событий'
            ax2.text(0.02, 0.98, stats_text, transform=ax2.transAxes,
                    verticalalignment='top',
                    bbox=dict(boxstyle='round', facecolor='lightblue', alpha=0.8))
    else:
        ax2.text(0.5, 0.5, 'Нет данных', ha='center', va='center', transform=ax2.transAxes)

    # График 3: Длина очереди (дискретный с скользящим средним)
    ax3 = axes[1, 0]
    if 'queue_length_timeseries' in timeseries_data and not timeseries_data['queue_length_timeseries'].empty:
        df = timeseries_data['queue_length_timeseries']
        # Дискретный отклик - точечный график
        ax3.scatter(df['time'], df['value'], alpha=0.6, color='orange',
                   s=20, label='Длина очереди')

        # Скользящее среднее
        if 'moving_avg' in df.columns and len(df) > 1:
            ax3.plot(df['time'], df['moving_avg'], alpha=0.9,
                    color='red', linewidth=2, label='Скольз. среднее')

        ax3.set_title('Длина очереди во времени\n(дискретный отклик)',
                     fontsize=12, weight='bold')
        ax3.set_xlabel('Время (секунды)')
        ax3.set_ylabel('Длина очереди (авто)')
        ax3.legend(fontsize=10)
        ax3.grid(True, alpha=0.3)

        # Статистика
        if len(df) > 0:
            stats_text = f'Точек: {len(df)}\nСреднее: {df["value"].mean():.2f}\nМакс: {df["value"].max():.0f}'
            ax3.text(0.02, 0.98, stats_text, transform=ax3.transAxes,
                    verticalalignment='top',
                    bbox=dict(boxstyle='round', facecolor='lightyellow', alpha=0.8))
    else:
        ax3.text(0.5, 0.5, 'Нет данных', ha='center', va='center', transform=ax3.transAxes)

    # График 4: Накопленное топливо (ступенчатый)
    ax4 = axes[1, 1]
    if 'fuel_cumulative_timeseries' in timeseries_data and not timeseries_data['fuel_cumulative_timeseries'].empty:
        df = timeseries_data['fuel_cumulative_timeseries']
        ax4.step(df['time'], df['value'], where='post', linewidth=2,
                alpha=0.8, color='darkgreen', label='Накопл. топливо')
        ax4.set_title('Накопленный объем топлива во времени\n(ступенчатый отклик)',
                     fontsize=12, weight='bold')
        ax4.set_xlabel('Время (секунды)')
        ax4.set_ylabel('Литры')
        ax4.legend(fontsize=10)
        ax4.grid(True, alpha=0.3)

        # Статистика
        if len(df) > 0:
            stats_text = f'Точек: {len(df)}\nВсего: {df["value"].iloc[-1]:.0f}л'
            ax4.text(0.02, 0.98, stats_text, transform=ax4.transAxes,
                    verticalalignment='top',
                    bbox=dict(boxstyle='round', facecolor='lightgreen', alpha=0.8))
    else:
        ax4.text(0.5, 0.5, 'Нет данных', ha='center', va='center', transform=ax4.transAxes)

    # График 5: Заполненность колонок (stacked area)
    ax5 = axes[2, 0]
    if 'columns_occupancy_timeseries' in timeseries_data and not timeseries_data['columns_occupancy_timeseries'].empty:
        df = timeseries_data['columns_occupancy_timeseries']
        if 'columns' in df.columns and len(df) > 0:
            times = df['time'].values
            cols_data = np.array(df['columns'].tolist()).T  # (n_columns, n_samples)

            # Рисуем stacked area для каждой колонки
            colors = plt.cm.Set3(np.linspace(0, 1, num_columns))
            ax5.stackplot(times, *cols_data, labels=[f'К{i}' for i in range(num_columns)],
                         colors=colors, alpha=0.7)

            ax5.set_title('Заполненность колонок во времени\n(stacked area)',
                         fontsize=12, weight='bold')
            ax5.set_xlabel('Время (секунды)')
            ax5.set_ylabel('Количество авто')
            ax5.legend(fontsize=8, ncol=3, loc='upper left')
            ax5.grid(True, alpha=0.3)

            # Статистика
            total_occupancy = cols_data.sum(axis=0)
            stats_text = f'Точек: {len(df)}\nМакс загрузка: {total_occupancy.max():.0f} авто'
            ax5.text(0.02, 0.98, stats_text, transform=ax5.transAxes,
                    verticalalignment='top',
                    bbox=dict(boxstyle='round', facecolor='lightcyan', alpha=0.8))
    else:
        ax5.text(0.5, 0.5, 'Нет данных', ha='center', va='center', transform=ax5.transAxes)

    # График 6: Интенсивность прибытия (скользящее окно)
    ax6 = axes[2, 1]
    if 'throughput_timeseries' in timeseries_data and not timeseries_data['throughput_timeseries'].empty:
        df = timeseries_data['throughput_timeseries']
        if len(df) > 10:
            # Вычисляем производную для интенсивности прибытия
            time_diff = df['time'].diff().fillna(0)
            arrival_rate = 1.0 / time_diff.replace(0, np.nan)  # авто/сек
            arrival_rate = arrival_rate.fillna(0)

            # Скользящее среднее для сглаживания
            window = min(20, len(df) // 5)
            if window > 1:
                arrival_smooth = arrival_rate.rolling(window=window, min_periods=1).mean()
                ax6.plot(df['time'], arrival_smooth, linewidth=2,
                        color='purple', alpha=0.8, label='Интенсивность (сглаж.)')
                ax6.fill_between(df['time'], 0, arrival_smooth, alpha=0.3, color='purple')

                ax6.set_title('Интенсивность прибытия во времени\n(скользящее среднее)',
                             fontsize=12, weight='bold')
                ax6.set_xlabel('Время (секунды)')
                ax6.set_ylabel('Авто/секунду')
                ax6.legend(fontsize=10)
                ax6.grid(True, alpha=0.3)

                # Статистика
                mean_rate = arrival_smooth.mean()
                stats_text = f'Средняя: {mean_rate:.4f} авто/с\n({mean_rate*60:.2f} авто/мин)'
                ax6.text(0.02, 0.98, stats_text, transform=ax6.transAxes,
                        verticalalignment='top',
                        bbox=dict(boxstyle='round', facecolor='lavender', alpha=0.8))
            else:
                ax6.text(0.5, 0.5, 'Недостаточно данных', ha='center', va='center',
                        transform=ax6.transAxes)
        else:
            ax6.text(0.5, 0.5, 'Недостаточно данных', ha='center', va='center',
                    transform=ax6.transAxes)
    else:
        ax6.text(0.5, 0.5, 'Нет данных', ha='center', va='center', transform=ax6.transAxes)

    plt.tight_layout()
    output_path = os.path.join(plots_dir, 'wait_time_timeseries.png')
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    plt.close()

    print(f"✓ Временные ряды метрик сохранены в файл: {output_path}")


def create_all_wait_time_plots(results_df, num_columns, timeseries_data=None, plots_dir="."):
    """Создание всех графиков времени ожидания и загрузки колонок"""

    print("=" * 80)
    print("СОЗДАНИЕ ВСЕХ ГРАФИКОВ ВРЕМЕНИ ОЖИДАНИЯ ПО КОЛОНКАМ АЗС")
    print("=" * 80)

    # 1. Отдельные графики по колонкам
    create_wait_time_plots(results_df, num_columns, plots_dir)

    # 2. Сравнительный график
    create_wait_time_comparison_plot(results_df, num_columns, plots_dir)

    # 3. Анализ очереди к оператору
    create_operator_queue_analysis(results_df, plots_dir)

    # 4. Тепловая карта загрузки колонок
    create_column_utilization_heatmap(results_df, num_columns, plots_dir)

    # 5. Временные ряды метрик (если есть данные)
    if timeseries_data is not None:
        create_wait_time_timeseries_plots(timeseries_data, num_columns, plots_dir)

    print(f"\n{'=' * 80}")
    print("ВСЕ ГРАФИКИ ВРЕМЕНИ ОЖИДАНИЯ СОЗДАНЫ!")
    print(f"{'=' * 80}")
    print("Созданные файлы:")
    print(f"  📊 {plots_dir}/wait_time_by_columns.png - Отдельные графики по колонкам")
    print(f"  📊 {plots_dir}/wait_time_comparison_columns.png - Сравнительный график")
    print(f"  📊 {plots_dir}/operator_queue_analysis.png - Анализ очереди к оператору")
    print(f"  📊 {plots_dir}/column_utilization_heatmap.png - Тепловая карта загрузки")
    if timeseries_data is not None:
        print(f"  📊 {plots_dir}/wait_time_timeseries.png - Временные ряды метрик")
    print(f"{'=' * 80}")


# ============================================================================
# ОСНОВНАЯ ФУНКЦИЯ ДЛЯ ДЕМОНСТРАЦИИ
# ============================================================================

def demonstrate_wait_time_plots(num_replications=20):
    """Демонстрация создания отдельных графиков времени ожидания для АЗС"""

    print("=" * 80)
    print("ДЕМОНСТРАЦИЯ ОТДЕЛЬНЫХ ГРАФИКОВ ВРЕМЕНИ ОЖИДАНИЯ НА АЗС")
    print("=" * 80)

    print(f"\nПараметры демонстрации:")
    print(f"  Время моделирования: {CONFIG['simulation_time']} сек ({CONFIG['simulation_time']/60:.0f} мин)")
    print(f"  Средний интервал прибытия: {CONFIG['arrival_mean']} сек")
    print(f"  Количество колонок: {CONFIG['num_columns_each_side'] * 2}")
    print(f"  Время оплаты: {CONFIG['payment_mu']} ± {CONFIG['payment_sigma']} сек")
    print(f"  Количество репликаций: {num_replications}")

    # Запуск множественных симуляций
    print(f"\n{'=' * 60}")
    print("ЗАПУСК МНОЖЕСТВЕННЫХ СИМУЛЯЦИЙ...")
    print(f"{'=' * 60}")

    results = []
    np.random.seed()
    seeds = [np.random.randint(0, 2**31) for _ in range(num_replications)]

    # Первый прогон с временными рядами
    print(f"  Запуск репликации 1/{num_replications} (seed={seeds[0]}) с временными рядами...")
    first_result, timeseries = run_simulation(seed=seeds[0], config=CONFIG, return_timeseries=True)
    first_result['replication_id'] = 0
    results.append(first_result)

    # Остальные прогоны без временных рядов
    for i in range(1, num_replications):
        print(f"  Запуск репликации {i+1}/{num_replications} (seed={seeds[i]})...")
        result = run_simulation(seed=seeds[i], config=CONFIG)
        result['replication_id'] = i
        results.append(result)

    results_df = pd.DataFrame(results)

    print(f"\n{'=' * 60}")
    print("ВСЕ РЕПЛИКАЦИИ ЗАВЕРШЕНЫ!")
    print(f"{'=' * 60}")

    # Создание всех графиков времени ожидания с временными рядами
    num_columns = CONFIG['num_columns_each_side'] * 2
    create_all_wait_time_plots(results_df, num_columns, timeseries)

    return results_df


if __name__ == "__main__":
    # Запуск демонстрации
    results_df = demonstrate_wait_time_plots(num_replications=20)

    print(f"\n{'=' * 80}")
    print("ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА УСПЕШНО!")
    print(f"{'=' * 80}")

