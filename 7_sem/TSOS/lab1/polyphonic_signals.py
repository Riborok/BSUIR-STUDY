import numpy as np
from signal_generators import SignalGenerator


class PolyphonicSignals:
    def __init__(self, sample_rate=44100):
        self.sample_rate = sample_rate
        self.generator = SignalGenerator(sample_rate)

    def combine_signals(self, signals, weights=None, normalize=True):
        if not signals:
            raise ValueError("Список сигналов не может быть пустым")

        # берем временную ось от первого сигнала
        time_array = signals[0][0]

        if weights is None:
            weights = [1.0 / len(signals)] * len(signals)

        if len(weights) != len(signals):
            raise ValueError("Количество весов должно соответствовать количеству сигналов")

        # суммируем сигналы с весами
        combined_signal = np.zeros_like(signals[0][1])
        for i, (_, signal_data) in enumerate(signals):
            # все сигналы имеют одинаковую длину
            if len(signal_data) != len(combined_signal):
                min_length = min(len(signal_data), len(combined_signal))
                signal_data = signal_data[:min_length]
                combined_signal = combined_signal[:min_length]
                time_array = time_array[:min_length]

            combined_signal += weights[i] * signal_data

        if normalize:
            combined_signal = self.generator.normalize_signal(combined_signal)

        return time_array, combined_signal

    def create_chord(self, frequencies, duration, signal_type='sine', **kwargs):
        """
        Создание аккорда из нескольких частот
        
        Args:
            frequencies (list): Список частот в Гц
            duration (float): Длительность в секундах
            signal_type (str): Тип сигнала ('sine', 'triangle', 'sawtooth', 'pulse')
            **kwargs: Дополнительные параметры для генератора сигналов
            
        Returns:
            tuple: (time_array, chord_signal)
        """
        signals = []

        for freq in frequencies:
            if signal_type == 'sine':
                t, s = self.generator.generate_sine(freq, duration, **kwargs)
            elif signal_type == 'triangle':
                t, s = self.generator.generate_triangle(freq, duration, **kwargs)
            elif signal_type == 'sawtooth':
                t, s = self.generator.generate_sawtooth(freq, duration, **kwargs)
            elif signal_type == 'pulse':
                t, s = self.generator.generate_pulse(freq, duration, **kwargs)
            else:
                raise ValueError(f"Неподдерживаемый тип сигнала: {signal_type}")

            signals.append((t, s))

        return self.combine_signals(signals)
