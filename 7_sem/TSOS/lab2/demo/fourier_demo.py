import os
import sys

from analysis.filter_analysis import DigitalFilter
from analysis.fourier_analysis import FourierAnalyzer
from core.signal_generators import SignalGenerator

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))


def demo_basic_fourier_analysis():
    print("=== Демонстрация анализа Фурье ===")
    sr = 1024 # кратность 2^n sample rate - частота дискретизации
    duration = 1.0

    generator = SignalGenerator(sr)
    analyzer = FourierAnalyzer(sr)

    # 3 sin (10 + 5 + 20)
    # t, signal1 = generator.generate_sine(10, duration, 0.8)
    # _, signal2 = generator.generate_sine(5, duration, 0.6)
    # _, signal3 = generator.generate_sine(30, duration, 0.4)
    #
    # complex_signal = signal1 + signal2 + signal3
    #
    # print("\nАнализ полифонического сигнала с помощью ДПФ...")
    # analyzer.analyze_and_plot(
    #     complex_signal, t,
    #     title="Сложный сигнал (3 sin)",
    #     use_manual=False,
    #     use_discrete=True,
    # )
    # print("\nАнализ полифонического сигнала с помощью БПФ...")
    # analyzer.analyze_and_plot(
    #     complex_signal, t,
    #     title="Сложный сигнал (3 sin)",
    #     use_manual=False,
    #     use_discrete=False,
    # )


def demo_signal_types_analysis():
    print("\n=== Анализ различных типов сигналов ===")

    sr = 2048
    duration = 0.5
    frequency = 10

    generator = SignalGenerator(sr)
    analyzer = FourierAnalyzer(sr)

    signals = {
        'Синусоида': generator.generate_sine(frequency, duration, 0.8),
        'Прямоугольник': generator.generate_pulse(frequency, duration, duty_cycle=0.5, amplitude=0.8),
        'Треугольник': generator.generate_triangle(frequency, duration, 0.8),
        'Пилообразный': generator.generate_sawtooth(frequency, duration, 0.8),
    }

    for signal_name, (t, signal_data) in signals.items():
        print(f"\nАнализ сигнала: {signal_name}")

        results = analyzer.analyze_and_plot(
            signal_data, t,
            title=f"[ДПФ] {signal_name} ({frequency} Гц)",
            use_manual=True,
            use_discrete=True,
        )

        results = analyzer.analyze_and_plot(
            signal_data, t,
            title=f"[БПФ] {signal_name} ({frequency} Гц)",
            use_manual=False,
            use_discrete=False,
        )


def demo_digital_filtering():
    """Демонстрация цифровой фильтрации"""
    print("\n=== Демонстрация цифровой фильтрации ===")

    sr = 2048
    duration = 1.0

    generator = SignalGenerator(sr)
    generator2 = SignalGenerator(44100)
    filter_analyzer = DigitalFilter(sr)

    filter_analyzer2 = DigitalFilter(44100)

    # Создаем сигнал с несколькими частотными компонентами
    t, low_freq = generator.generate_sine(5, duration, 0.6)  # Низкая частота
    _, mid_freq = generator.generate_sine(20, duration, 0.8)  # Средняя частота
    _, high_freq = generator.generate_sine(50, duration, 0.4)  # Высокая частота  # Шум

    _, white_noise = generator2.generate_noise(5, amplitude=0.8)
    filter_analyzer2.analyze_filter_effect(
        white_noise, t,
        filter_type='lowpass',
        cutoff_freqs=1000,
        order=5,
        title="БЕЛЫЙ-ШУМ-НЧ-фильтр",
        use_manual=False,
        is_noise=True
        ,
    )
    filter_analyzer2.analyze_filter_effect(
        white_noise, t,
        filter_type='highpass',
        cutoff_freqs=10000,
        order=5,
        title="БЕЛЫЙ-ШУМ-НЧ-фильтр",
        use_manual=False,
        is_noise = True
        ,
    )
    filter_analyzer2.analyze_filter_effect(
        white_noise, t,
        filter_type='bandpass',
        cutoff_freqs=(1000, 10000),
        order=5,
        title="БЕЛЫЙ-ШУМ-НЧ-фильтр",
        use_manual=False,
        is_noise = True
        ,
    )

    # Сложный сигнал
    complex_signal = low_freq + mid_freq + high_freq
    complex_signal = generator.normalize_signal(complex_signal)

    print("Анализ НЧ-фильтра (частота среза 15 Гц)...")
    filter_analyzer.analyze_filter_effect(
        complex_signal, t,
        filter_type='lowpass',
        cutoff_freqs=15,
        order=5,
        title="НЧ-фильтр",
        use_manual=False
        ,
    )

    print("Анализ ВЧ-фильтра (частота среза 40 Гц)...")
    filter_analyzer.analyze_filter_effect(
        complex_signal, t,
        filter_type='highpass',
        cutoff_freqs=40,
        order=5,
        title="ВЧ-фильтр",
        use_manual=False,
    )

    print("Анализ полосового фильтра (15-40 Гц)...")
    filter_analyzer.analyze_filter_effect(
        complex_signal, t,
        filter_type='bandpass',
        cutoff_freqs=(15, 40),
        order=3,
        title="Полосовой фильтр",
        use_manual=False,
    )


def main():
    print("Демонстрация анализа сигналов с помощью преобразований Фурье")
    print("=" * 60)

    try:
        # полифония
        # demo_basic_fourier_analysis()

        # базовые сигналы ДПФ + БПФ
        # demo_signal_types_analysis()

        # Цифровая фильтрация
        demo_digital_filtering()

        print("\n" + "=" * 60)
        print("Готово!")

    except Exception as e:
        print(f"Ошибка во время выполнения: {e}")
        import traceback
        traceback.print_exc()


if __name__ == "__main__":
    main()
