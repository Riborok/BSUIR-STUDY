"""
Модуль визуализации результатов симуляции АЗС
Создает комплексные графики по аналогии с визуализацией супермаркета
"""

import matplotlib.pyplot as plt
import matplotlib
matplotlib.use('Agg')
import pandas as pd
import numpy as np
from typing import Dict, List
import os


class GasStationVisualizer:
    """Класс для визуализации результатов множественных прогонов симуляции АЗС"""

    def __init__(self, results_df: pd.DataFrame, timeseries_data: Dict = None):
        """
        Args:
            results_df: DataFrame с результатами всех репликаций
            timeseries_data: Словарь с временными рядами из первой репликации
        """
        self.results_df = results_df
        self.timeseries_data = timeseries_data or {}
        self.num_replications = len(results_df)

    def create_comprehensive_plot(self, output_file='gas_station_comprehensive.png'):
        """Создание комплексного графика со всеми метриками"""

        fig = plt.figure(figsize=(20, 16))

        # График 1: Пропускная способность (throughput) по репликациям
        ax1 = plt.subplot(4, 4, 1)
        ax1.bar(self.results_df.index, self.results_df['throughput'],
                color='green', alpha=0.7, edgecolor='black', linewidth=0.5)
        ax1.axhline(self.results_df['throughput'].mean(), color='red',
                   linestyle='--', linewidth=2, alpha=0.7,
                   label=f"Среднее: {self.results_df['throughput'].mean():.1f}")
        ax1.set_title('Пропускная способность (авто)', fontsize=12, weight='bold')
        ax1.set_xlabel('Репликация')
        ax1.set_ylabel('Количество авто')
        ax1.legend(fontsize=8)
        ax1.grid(True, alpha=0.3, axis='y')

        # График 2: Среднее время ожидания оплаты
        ax2 = plt.subplot(4, 4, 2)
        ax2.scatter(self.results_df.index, self.results_df['avg_wait_to_payment_s'],
                   marker='s', color='blue', s=50)
        ax2.axhline(self.results_df['avg_wait_to_payment_s'].mean(), color='red',
                   linestyle='--', linewidth=2, alpha=0.7,
                   label=f"Среднее: {self.results_df['avg_wait_to_payment_s'].mean():.1f}с")
        ax2.set_title('Среднее время ожидания оплаты', fontsize=12, weight='bold')
        ax2.set_xlabel('Репликация')
        ax2.set_ylabel('Секунды')
        ax2.legend(fontsize=8)
        ax2.grid(True, alpha=0.3)

        # График 3: Утилизация оператора
        ax3 = plt.subplot(4, 4, 3)
        ax3.scatter(self.results_df.index, self.results_df['operator_util'] * 100,
                   marker='D', color='purple', s=50)
        ax3.axhline(self.results_df['operator_util'].mean() * 100, color='red',
                   linestyle='--', linewidth=2, alpha=0.7,
                   label=f"Среднее: {self.results_df['operator_util'].mean()*100:.1f}%")
        ax3.set_title('Загрузка оператора', fontsize=12, weight='bold')
        ax3.set_xlabel('Репликация')
        ax3.set_ylabel('Процент (%)')
        ax3.legend(fontsize=8)
        ax3.grid(True, alpha=0.3)

        # График 4: Максимальная длина очереди
        ax4 = plt.subplot(4, 4, 4)
        ax4.scatter(self.results_df.index, self.results_df['max_queue_length'],
                   marker='^', color='orange', s=50)
        ax4.axhline(self.results_df['max_queue_length'].mean(), color='red',
                   linestyle='--', linewidth=2, alpha=0.7,
                   label=f"Среднее: {self.results_df['max_queue_length'].mean():.1f}")
        ax4.set_title('Максимальная длина очереди', fontsize=12, weight='bold')
        ax4.set_xlabel('Репликация')
        ax4.set_ylabel('Количество авто')
        ax4.legend(fontsize=8)
        ax4.grid(True, alpha=0.3)

        # График 5: Временной ряд - пропускная способность (СТУПЕНЧАТЫЙ)
        ax5 = plt.subplot(4, 4, 5)
        if 'throughput_timeseries' in self.timeseries_data:
            df = self.timeseries_data['throughput_timeseries']
            if not df.empty:
                ax5.step(df['time'], df['value'], where='post',
                        linewidth=2, alpha=0.8, color='green')
                ax5.set_title('Пропускная способность во времени', fontsize=10, weight='bold')
                ax5.set_xlabel('Время (с)')
                ax5.set_ylabel('Кум. авто')
                ax5.grid(True, alpha=0.3)

        # График 6: Временной ряд - состояние оператора (СТУПЕНЧАТЫЙ)
        ax6 = plt.subplot(4, 4, 6)
        if 'operator_state_timeseries' in self.timeseries_data:
            df = self.timeseries_data['operator_state_timeseries']
            if not df.empty:
                ax6.step(df['time'], df['value'], where='post',
                        linewidth=1.5, alpha=0.8, color='blue')
                ax6.set_title('Состояние оператора', fontsize=10, weight='bold')
                ax6.set_xlabel('Время (с)')
                ax6.set_ylabel('Занят (0/1)')
                ax6.set_ylim(-0.1, 1.1)
                ax6.grid(True, alpha=0.3)

        # График 7: Временной ряд - длина очереди (ДИСКРЕТНЫЙ)
        ax7 = plt.subplot(4, 4, 7)
        if 'queue_length_timeseries' in self.timeseries_data:
            df = self.timeseries_data['queue_length_timeseries']
            if not df.empty:
                ax7.plot(df['time'], df['value'], alpha=0.6, color='orange',
                        linewidth=1, label='Значения')
                if 'moving_avg' in df.columns:
                    ax7.plot(df['time'], df['moving_avg'], alpha=0.9,
                            color='red', linewidth=2, label='Скольз. среднее')
                ax7.set_title('Длина очереди во времени', fontsize=10, weight='bold')
                ax7.set_xlabel('Время (с)')
                ax7.set_ylabel('Длина очереди')
                ax7.legend(fontsize=8)
                ax7.grid(True, alpha=0.3)

        # График 8: Временной ряд - накопленное топливо (СТУПЕНЧАТЫЙ)
        ax8 = plt.subplot(4, 4, 8)
        if 'fuel_cumulative_timeseries' in self.timeseries_data:
            df = self.timeseries_data['fuel_cumulative_timeseries']
            if not df.empty:
                ax8.step(df['time'], df['value'], where='post',
                        linewidth=2, alpha=0.8, color='darkgreen')
                ax8.set_title('Накопленный объем топлива', fontsize=10, weight='bold')
                ax8.set_xlabel('Время (с)')
                ax8.set_ylabel('Литры')
                ax8.grid(True, alpha=0.3)

        # График 9: Утилизация колонок (bar chart)
        ax9 = plt.subplot(4, 4, 9)
        column_cols = [col for col in self.results_df.columns
                      if col.startswith('column_') and col.endswith('_util')]
        if column_cols:
            column_utils = [self.results_df[col].mean() * 100 for col in column_cols]
            colors = plt.cm.Set3(np.linspace(0, 1, len(column_utils)))
            bars = ax9.bar(range(len(column_utils)), column_utils, color=colors)
            ax9.set_xticks(range(len(column_utils)))
            ax9.set_xticklabels([f'К{i}' for i in range(len(column_utils))], fontsize=9)
            ax9.set_title('Средняя загрузка колонок', fontsize=12, weight='bold')
            ax9.set_ylabel('Процент (%)')
            ax9.grid(True, alpha=0.3, axis='y')

            # Добавим значения на столбцы
            for bar in bars:
                height = bar.get_height()
                ax9.text(bar.get_x() + bar.get_width()/2., height,
                        f'{height:.1f}%', ha='center', va='bottom', fontsize=8)

        # График 10: Распределение времени ожидания
        ax10 = plt.subplot(4, 4, 10)
        ax10.hist(self.results_df['avg_wait_to_payment_s'], bins=15,
                 color='skyblue', alpha=0.7, edgecolor='black')
        ax10.axvline(self.results_df['avg_wait_to_payment_s'].mean(),
                    color='red', linestyle='--', linewidth=2,
                    label=f"Среднее: {self.results_df['avg_wait_to_payment_s'].mean():.1f}с")
        ax10.set_title('Распределение времени ожидания', fontsize=12, weight='bold')
        ax10.set_xlabel('Секунды')
        ax10.set_ylabel('Частота')
        ax10.legend(fontsize=8)
        ax10.grid(True, alpha=0.3)

        # График 11: Распределение времени заправки
        ax11 = plt.subplot(4, 4, 11)
        ax11.hist(self.results_df['avg_fueling_dur_s'], bins=15,
                 color='lightcoral', alpha=0.7, edgecolor='black')
        ax11.axvline(self.results_df['avg_fueling_dur_s'].mean(),
                    color='red', linestyle='--', linewidth=2,
                    label=f"Среднее: {self.results_df['avg_fueling_dur_s'].mean():.1f}с")
        ax11.set_title('Распределение времени заправки', fontsize=12, weight='bold')
        ax11.set_xlabel('Секунды')
        ax11.set_ylabel('Частота')
        ax11.legend(fontsize=8)
        ax11.grid(True, alpha=0.3)

        # График 12: Общее время в системе
        ax12 = plt.subplot(4, 4, 12)
        ax12.scatter(self.results_df.index, self.results_df['avg_time_in_system_s'],
                    marker='o', color='teal', s=50)
        ax12.axhline(self.results_df['avg_time_in_system_s'].mean(), color='red',
                    linestyle='--', linewidth=2, alpha=0.7,
                    label=f"Среднее: {self.results_df['avg_time_in_system_s'].mean():.1f}с")
        ax12.set_title('Среднее время в системе', fontsize=12, weight='bold')
        ax12.set_xlabel('Репликация')
        ax12.set_ylabel('Секунды')
        ax12.legend(fontsize=8)
        ax12.grid(True, alpha=0.3)


        # График 14: Заполненность колонок во времени (stacked area)
        ax14 = plt.subplot(4, 4, 14)
        if 'columns_occupancy_timeseries' in self.timeseries_data:
            df = self.timeseries_data['columns_occupancy_timeseries']
            if not df.empty and 'columns' in df.columns:
                # Создаем stacked area chart
                times = df['time'].values
                cols_data = np.array(df['columns'].tolist()).T  # Транспонируем для stacking

                # Создаем cumulative sum для stacked area
                cum = np.cumsum(cols_data, axis=0)

                # Рисуем области
                colors = plt.cm.Set3(np.linspace(0, 1, cols_data.shape[0]))
                for i in range(cols_data.shape[0]):
                    top = cum[i]
                    bottom = cum[i-1] if i > 0 else np.zeros_like(top)
                    ax14.fill_between(times, bottom, top, color=colors[i],
                                     alpha=0.7, label=f'К{i}')

                ax14.set_title('Заполненность колонок', fontsize=10, weight='bold')
                ax14.set_xlabel('Время (с)')
                ax14.set_ylabel('Количество авто')
                ax14.legend(fontsize=7, ncol=2)
                ax14.grid(True, alpha=0.3)

        # График 15: Корреляция между метриками
        ax15 = plt.subplot(4, 4, 15)
        correlation_cols = ['throughput', 'avg_wait_to_payment_s',
                          'operator_util', 'max_queue_length']
        correlation_cols = [col for col in correlation_cols if col in self.results_df.columns]
        if len(correlation_cols) >= 2:
            corr = self.results_df[correlation_cols].corr()
            im = ax15.imshow(corr, cmap='coolwarm', aspect='auto', vmin=-1, vmax=1)
            ax15.set_xticks(range(len(correlation_cols)))
            ax15.set_yticks(range(len(correlation_cols)))
            ax15.set_xticklabels([col[:15] for col in correlation_cols],
                                rotation=45, ha='right', fontsize=8)
            ax15.set_yticklabels([col[:15] for col in correlation_cols], fontsize=8)
            ax15.set_title('Корреляция метрик', fontsize=12, weight='bold')

            # Добавляем значения корреляций
            for i in range(len(correlation_cols)):
                for j in range(len(correlation_cols)):
                    text = ax15.text(j, i, f'{corr.iloc[i, j]:.2f}',
                                   ha="center", va="center", color="black", fontsize=8)

            plt.colorbar(im, ax=ax15, fraction=0.046, pad=0.04)

        # График 16: Boxplot утилизации колонок
        ax16 = plt.subplot(4, 4, 16)
        if column_cols:
            column_data = [self.results_df[col] * 100 for col in column_cols]
            bp = ax16.boxplot(column_data, labels=[f'К{i}' for i in range(len(column_cols))],
                             patch_artist=True)

            # Раскрашиваем boxplot
            colors = plt.cm.Pastel1(np.linspace(0, 1, len(column_cols)))
            for patch, color in zip(bp['boxes'], colors):
                patch.set_facecolor(color)

            ax16.set_title('Распределение загрузки колонок', fontsize=12, weight='bold')
            ax16.set_ylabel('Процент (%)')
            ax16.grid(True, alpha=0.3, axis='y')

        plt.tight_layout()
        plt.savefig(output_file, dpi=300, bbox_inches='tight')
        print(f"✓ Комплексный график сохранен: {output_file}")
        plt.close()

    def create_summary_table(self, output_file='summary_table.png'):
        """Создание таблицы со сводной статистикой"""

        # Выбираем ключевые метрики
        metrics = {
            'throughput': 'Пропускная способность (авто)',
            'avg_wait_to_payment_s': 'Ср. время ожидания (с)',
            'avg_fueling_dur_s': 'Ср. время заправки (с)',
            'avg_time_in_system_s': 'Ср. время в системе (с)',
            'operator_util': 'Загрузка оператора (%)',
            'max_queue_length': 'Макс. длина очереди'
        }

        summary_data = []
        for col, label in metrics.items():
            if col in self.results_df.columns:
                values = self.results_df[col]
                if col == 'operator_util':
                    values = values * 100
                summary_data.append({
                    'Метрика': label,
                    'Среднее': f'{values.mean():.2f}',
                    'Ст. откл.': f'{values.std():.2f}',
                    'Мин': f'{values.min():.2f}',
                    'Макс': f'{values.max():.2f}',
                    'Медиана': f'{values.median():.2f}'
                })

        summary_df = pd.DataFrame(summary_data)

        # Создаем таблицу как изображение
        fig, ax = plt.subplots(figsize=(12, 6))
        ax.axis('tight')
        ax.axis('off')

        table = ax.table(cellText=summary_df.values,
                        colLabels=summary_df.columns,
                        cellLoc='center',
                        loc='center',
                        colWidths=[0.3, 0.14, 0.14, 0.14, 0.14, 0.14])

        table.auto_set_font_size(False)
        table.set_fontsize(10)
        table.scale(1, 2)

        # Раскрашиваем заголовки
        for i in range(len(summary_df.columns)):
            table[(0, i)].set_facecolor('#4CAF50')
            table[(0, i)].set_text_props(weight='bold', color='white')

        # Раскрашиваем строки (чередование цветов)
        for i in range(1, len(summary_df) + 1):
            color = '#f0f0f0' if i % 2 == 0 else 'white'
            for j in range(len(summary_df.columns)):
                table[(i, j)].set_facecolor(color)

        plt.title('Сводная статистика симуляции АЗС',
                 fontsize=14, weight='bold', pad=20)

        plt.savefig(output_file, dpi=300, bbox_inches='tight')
        print(f"✓ Таблица статистики сохранена: {output_file}")
        plt.close()


def load_timeseries_from_first_run(trace_file='azs_trace.txt',
                                   per_car_file='per_car_stats.csv'):
    """
    Загрузка временных рядов из первого прогона
    Парсит файлы трассировки для создания временных рядов
    """
    timeseries_data = {}

    # Попытка загрузить данные о машинах
    if os.path.exists(per_car_file):
        df = pd.read_csv(per_car_file)

        # Создаем временные ряды на основе событий
        # (это упрощенная версия, в реальности нужно парсить trace файл)
        pass

    return timeseries_data

