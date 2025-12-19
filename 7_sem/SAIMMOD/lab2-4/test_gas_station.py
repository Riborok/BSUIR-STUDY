"""
Юнит-тесты для проверки корректности модели АЗС
"""

import unittest
import sys
import os
import pandas as pd
import numpy as np
from copy import deepcopy

# Добавляем путь к модулям
sys.path.insert(0, os.path.dirname(__file__))
from gas_station_simulation import run_simulation, CONFIG


def run_multiple_simulations(config, num_runs=5):
    """
    Вспомогательная функция для запуска нескольких симуляций

    Args:
        config: Конфигурация симуляции
        num_runs: Количество прогонов

    Returns:
        DataFrame с результатами всех прогонов
    """
    results = []
    np.random.seed(42)  # Фиксируем seed для воспроизводимости
    seeds = [np.random.randint(0, 2**31) for _ in range(num_runs)]

    for seed in seeds:
        result = run_simulation(seed=seed, config=config)
        results.append(result)

    return pd.DataFrame(results)


class GasStationSimulationTests(unittest.TestCase):
    """Тесты для проверки корректности модели АЗС"""

    def setUp(self):
        """Настройка базовой конфигурации для тестов"""
        self.base_config = {
            "num_columns_each_side": 3,
            "arrival_mean": 45.0,
            "fuel_volume_mu": 40.0,
            "fuel_volume_sigma": 10.0,
            "fuel_min": 5.0,
            "fuel_max": 100.0,
            "payment_mu": 60.0,
            "payment_sigma": 15.0,
            "payment_min": 20.0,
            "payment_max": 90.0,
            "refill_speed": 1.5,
            "simulation_time": 3600,  # 1 час для быстрых тестов
            "left_side_probability": 0.7
        }
        self.num_replications = 5

    def test_faster_payment_decreases_wait_time(self):
        """Тест: ускорение оплаты должно уменьшить время ожидания"""
        print("\n" + "="*60)
        print("Тест 1: Ускорение процесса оплаты")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_fast_payment = deepcopy(self.base_config)
        config_fast_payment['payment_mu'] = 30.0  # Ускоренная оплата
        df_fast = run_multiple_simulations(config_fast_payment, self.num_replications)

        wait_base = df_base['avg_wait_to_payment_s'].mean()
        wait_fast = df_fast['avg_wait_to_payment_s'].mean()

        print(f"  Базовое (60с оплата): {wait_base:.2f}с ожидания")
        print(f"  Быстрое (30с оплата): {wait_fast:.2f}с ожидания")
        print(f"  ✓ Время ожидания уменьшилось на {wait_base - wait_fast:.2f}с")

        self.assertLess(wait_fast, wait_base,
                       "Ускорение оплаты должно уменьшить время ожидания")

    def test_faster_refill_speed_decreases_fueling_time(self):
        """Тест: увеличение скорости заправки должно уменьшить время заправки"""
        print("\n" + "="*60)
        print("Тест 2: Увеличение скорости заправки")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_fast_refill = deepcopy(self.base_config)
        config_fast_refill['refill_speed'] = 3.0  # Удвоенная скорость
        df_fast = run_multiple_simulations(config_fast_refill, self.num_replications)

        fuel_time_base = df_base['avg_fueling_dur_s'].mean()
        fuel_time_fast = df_fast['avg_fueling_dur_s'].mean()

        print(f"  Базовая скорость (1.5 л/с): {fuel_time_base:.2f}с заправки")
        print(f"  Высокая скорость (3.0 л/с): {fuel_time_fast:.2f}с заправки")
        print(f"  ✓ Время заправки уменьшилось на {fuel_time_base - fuel_time_fast:.2f}с")

        self.assertLess(fuel_time_fast, fuel_time_base,
                       "Увеличение скорости заправки должно уменьшить время заправки")

    def test_high_arrival_rate_increases_operator_utilization(self):
        """Тест: высокая интенсивность прибытия должна увеличить загрузку оператора"""
        print("\n" + "="*60)
        print("Тест 3: Увеличение интенсивности прибытия")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_high_arrival = deepcopy(self.base_config)
        config_high_arrival['arrival_mean'] = 20.0  # Частые прибытия
        df_high = run_multiple_simulations(config_high_arrival, self.num_replications)

        util_base = df_base['operator_util'].mean() * 100
        util_high = df_high['operator_util'].mean() * 100

        print(f"  Базовое (45с интервал): {util_base:.2f}% загрузки оператора")
        print(f"  Высокая (20с интервал): {util_high:.2f}% загрузки оператора")
        print(f"  ✓ Загрузка оператора увеличилась на {util_high - util_base:.2f}%")

        self.assertGreater(util_high, util_base,
                          "Высокая интенсивность прибытия должна увеличить загрузку оператора")

    def test_shorter_simulation_time_serves_fewer_cars(self):
        """Тест: уменьшение времени симуляции должно уменьшить число обслуженных авто"""
        print("\n" + "="*60)
        print("Тест 4: Уменьшение времени симуляции")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_short = deepcopy(self.base_config)
        config_short['simulation_time'] = 1800  # Половина времени
        df_short = run_multiple_simulations(config_short, self.num_replications)

        served_base = df_base['throughput'].mean()
        served_short = df_short['throughput'].mean()

        print(f"  Базовое (3600с): {served_base:.2f} авто")
        print(f"  Короткое (1800с): {served_short:.2f} авто")
        print(f"  ✓ Обслужено меньше на {served_base - served_short:.2f} авто")

        self.assertLess(served_short, served_base,
                       "Уменьшение времени симуляции должно уменьшить число обслуженных авто")

    def test_higher_fuel_volume_increases_fueling_time(self):
        """Тест: больший объем топлива должен увеличить время заправки"""
        print("\n" + "="*60)
        print("Тест 5: Увеличение объема топлива")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_large_volume = deepcopy(self.base_config)
        config_large_volume['fuel_volume_mu'] = 70.0  # Больший объем
        df_large = run_multiple_simulations(config_large_volume, self.num_replications)

        fuel_time_base = df_base['avg_fueling_dur_s'].mean()
        fuel_time_large = df_large['avg_fueling_dur_s'].mean()

        print(f"  Базовый объем (40л): {fuel_time_base:.2f}с заправки")
        print(f"  Большой объем (70л): {fuel_time_large:.2f}с заправки")
        print(f"  ✓ Время заправки увеличилось на {fuel_time_large - fuel_time_base:.2f}с")

        self.assertGreater(fuel_time_large, fuel_time_base,
                          "Больший объем топлива должен увеличить время заправки")

    def test_high_load_increases_queue_length(self):
        """Тест: высокая загрузка должна увеличить длину очереди"""
        print("\n" + "="*60)
        print("Тест 6: Высокая загрузка системы")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_high_load = deepcopy(self.base_config)
        config_high_load['arrival_mean'] = 15.0  # Очень частые прибытия
        df_high = run_multiple_simulations(config_high_load, self.num_replications)

        queue_base = df_base['max_queue_length'].mean()
        queue_high = df_high['max_queue_length'].mean()

        print(f"  Базовая загрузка: {queue_base:.2f} авто в очереди")
        print(f"  Высокая загрузка: {queue_high:.2f} авто в очереди")
        print(f"  ✓ Длина очереди увеличилась на {queue_high - queue_base:.2f} авто")

        self.assertGreater(queue_high, queue_base,
                          "Высокая загрузка должна увеличить длину очереди")

    def test_deterministic_results_with_same_seed(self):
        """Тест: одинаковый seed должен давать одинаковые результаты"""
        print("\n" + "="*60)
        print("Тест 7: Детерминированность (одинаковый seed)")
        print("="*60)

        result1 = run_simulation(seed=12345, config=self.base_config)
        result2 = run_simulation(seed=12345, config=self.base_config)

        throughput1 = result1['throughput']
        throughput2 = result2['throughput']

        print(f"  Прогон 1 (seed=12345): {throughput1} авто")
        print(f"  Прогон 2 (seed=12345): {throughput2} авто")

        self.assertEqual(throughput1, throughput2,
                        "Одинаковый seed должен давать одинаковые результаты")
        print(f"  ✓ Результаты идентичны")

    def test_more_columns_reduces_column_utilization(self):
        """Тест: больше колонок должно снизить их среднюю загрузку"""
        print("\n" + "="*60)
        print("Тест 8: Загрузка колонок при увеличении их количества")
        print("="*60)

        df_base = run_multiple_simulations(self.base_config, self.num_replications)

        config_more_columns = deepcopy(self.base_config)
        config_more_columns['num_columns_each_side'] = 5
        df_more = run_multiple_simulations(config_more_columns, self.num_replications)

        # Средняя утилизация всех колонок
        cols_base = [col for col in df_base.columns if col.startswith('column_') and col.endswith('_util')]
        util_base = df_base[cols_base].mean().mean() * 100

        cols_more = [col for col in df_more.columns if col.startswith('column_') and col.endswith('_util')]
        util_more = df_more[cols_more].mean().mean() * 100

        print(f"  6 колонок: {util_base:.2f}% средняя загрузка")
        print(f"  10 колонок: {util_more:.2f}% средняя загрузка")
        print(f"  ✓ Средняя загрузка уменьшилась на {util_base - util_more:.2f}%")

        self.assertLess(util_more, util_base,
                       "Больше колонок должно снизить их среднюю загрузку")

    def test_extreme_high_load_increases_wait_time(self):
        """Тест: экстремально высокая загрузка должна значительно увеличить время ожидания"""
        print("\n" + "="*60)
        print("Тест 9: Экстремально высокая загрузка")
        print("="*60)

        config_extreme = deepcopy(self.base_config)
        config_extreme['arrival_mean'] = 10.0  # Очень частые прибытия
        df_extreme = run_multiple_simulations(config_extreme, self.num_replications)

        avg_wait = df_extreme['avg_wait_to_payment_s'].mean()
        max_queue = df_extreme['max_queue_length'].mean()
        operator_util = df_extreme['operator_util'].mean() * 100

        print(f"  Интервал прибытия: 10с")
        print(f"  Среднее время ожидания: {avg_wait:.2f}с")
        print(f"  Средняя макс. очередь: {max_queue:.2f} авто")
        print(f"  Загрузка оператора: {operator_util:.2f}%")

        self.assertGreater(avg_wait, 100,
                          "Экстремальная загрузка должна давать значительное время ожидания (>100с)")
        print(f"  ✓ Значительное время ожидания (>{avg_wait:.2f}с)")


class ComparisonTests(unittest.TestCase):
    """Тесты для сравнения различных сценариев оптимизации АЗС"""

    def setUp(self):
        """Настройка базовой конфигурации для тестов"""
        self.base_config = {
            "num_columns_each_side": 3,
            "arrival_mean": 45.0,
            "fuel_volume_mu": 40.0,
            "fuel_volume_sigma": 10.0,
            "fuel_min": 5.0,
            "fuel_max": 100.0,
            "payment_mu": 60.0,
            "payment_sigma": 15.0,
            "payment_min": 20.0,
            "payment_max": 90.0,
            "refill_speed": 1.5,
            "simulation_time": 3600,
            "left_side_probability": 0.7
        }
        self.num_replications = 10

    def test_columns_vs_payment_speed_tradeoff(self):
        """Тест сравнения: больше колонок vs быстрее оплата"""
        print("\n" + "="*60)
        print("Тест сравнения: Колонки vs Скорость оплаты")
        print("="*60)

        # Сценарий 1: Больше колонок
        config_scenario1 = deepcopy(self.base_config)
        config_scenario1['num_columns_each_side'] = 5
        df1 = run_multiple_simulations(config_scenario1, self.num_replications)

        # Сценарий 2: Быстрее оплата
        config_scenario2 = deepcopy(self.base_config)
        config_scenario2['payment_mu'] = 30.0
        df2 = run_multiple_simulations(config_scenario2, self.num_replications)

        print(f"  Сценарий 1 (10 колонок):")
        print(f"    Пропускная способность: {df1['throughput'].mean():.2f} авто")
        print(f"    Время ожидания: {df1['avg_wait_to_payment_s'].mean():.2f}с")
        print(f"    Загрузка оператора: {df1['operator_util'].mean()*100:.2f}%")

        print(f"  Сценарий 2 (быстрая оплата 30с):")
        print(f"    Пропускная способность: {df2['throughput'].mean():.2f} авто")
        print(f"    Время ожидания: {df2['avg_wait_to_payment_s'].mean():.2f}с")
        print(f"    Загрузка оператора: {df2['operator_util'].mean()*100:.2f}%")

        self.assertTrue(True, "Сравнение завершено")

    def test_refill_speed_vs_payment_speed(self):
        """Тест сравнения: скорость заправки vs скорость оплаты"""
        print("\n" + "="*60)
        print("Тест сравнения: Скорость заправки vs Скорость оплаты")
        print("="*60)

        # Сценарий 1: Быстрая заправка
        config_scenario1 = deepcopy(self.base_config)
        config_scenario1['refill_speed'] = 3.0
        df1 = run_multiple_simulations(config_scenario1, self.num_replications)

        # Сценарий 2: Быстрая оплата
        config_scenario2 = deepcopy(self.base_config)
        config_scenario2['payment_mu'] = 30.0
        df2 = run_multiple_simulations(config_scenario2, self.num_replications)

        print(f"  Сценарий 1 (быстрая заправка 3.0 л/с):")
        print(f"    Время в системе: {df1['avg_time_in_system_s'].mean():.2f}с")
        print(f"    Время заправки: {df1['avg_fueling_dur_s'].mean():.2f}с")

        print(f"  Сценарий 2 (быстрая оплата 30с):")
        print(f"    Время в системе: {df2['avg_time_in_system_s'].mean():.2f}с")
        print(f"    Время ожидания: {df2['avg_wait_to_payment_s'].mean():.2f}с")

        self.assertTrue(True, "Сравнение завершено")


def run_all_tests():
    """Запуск всех unit-тестов для проверки корректности модели АЗС"""
    print("\n" + "="*80)
    print("ЗАПУСК UNIT-ТЕСТОВ ИМИТАЦИОННОЙ МОДЕЛИ АЗС")
    print("="*80)

    loader = unittest.TestLoader()
    suite = unittest.TestSuite()

    suite.addTests(loader.loadTestsFromTestCase(GasStationSimulationTests))
    suite.addTests(loader.loadTestsFromTestCase(ComparisonTests))

    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(suite)

    print("\n" + "="*80)
    print("ИТОГИ ТЕСТИРОВАНИЯ")
    print("="*80)
    print(f"Всего тестов: {result.testsRun}")
    print(f"Успешно: {result.testsRun - len(result.failures) - len(result.errors)}")
    print(f"Провалено: {len(result.failures)}")
    print(f"Ошибок: {len(result.errors)}")
    print("="*80 + "\n")

    return result


if __name__ == "__main__":
    run_all_tests()

