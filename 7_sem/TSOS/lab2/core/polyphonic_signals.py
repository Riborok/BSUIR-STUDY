import numpy as np
from core.signal_generators import SignalGenerator


class PolyphonicSignals:
    def __init__(self, sample_rate=44100):
        self.sample_rate = sample_rate
        self.generator = SignalGenerator(sample_rate)

    def combine_signals(self, signals, weights=None, normalize=True):
        if not signals:
            raise ValueError("Список сигналов не может быть пустым")

        time_array = signals[0][0]

        if weights is None:
            weights = [1.0 / len(signals)] * len(signals)

        if len(weights) != len(signals):
            raise ValueError("Количество весов должно соответствовать количеству сигналов")

        combined_signal = np.zeros_like(signals[0][1])
        for i, (_, signal_data) in enumerate(signals):
            if len(signal_data) != len(combined_signal):
                min_length = min(len(signal_data), len(combined_signal))
                signal_data = signal_data[:min_length]
                combined_signal = combined_signal[:min_length]
                time_array = time_array[:min_length]

            combined_signal += weights[i] * signal_data

        if normalize:
            combined_signal = self.generator.normalize_signal(combined_signal)

        return time_array, combined_signal

    def create_chord(self, frequencies, duration, signal_type='sine'):
        signals = []

        for freq in frequencies:
            if signal_type == 'sine':
                t, s = self.generator.generate_sine(freq, duration)
            elif signal_type == 'triangle':
                t, s = self.generator.generate_triangle(freq, duration)
            elif signal_type == 'sawtooth':
                t, s = self.generator.generate_sawtooth(freq, duration)
            elif signal_type == 'pulse':
                t, s = self.generator.generate_pulse(freq, duration)
            else:
                raise ValueError(f"Неподдерживаемый тип сигнала: {signal_type}")

            signals.append((t, s))

        return self.combine_signals(signals)

    def create_harmonic_series(self, fundamental_freq, num_harmonics, duration,
                               signal_type='sine', harmonic_weights=None):

        frequencies = [fundamental_freq * (i + 1) for i in range(num_harmonics)]

        if harmonic_weights is None:
            harmonic_weights = [1.0 / (i + 1) for i in range(num_harmonics)]

        signals = []
        for freq in frequencies:
            if signal_type == 'sine':
                t, s = self.generator.generate_sine(freq, duration)
            elif signal_type == 'triangle':
                t, s = self.generator.generate_triangle(freq, duration)
            elif signal_type == 'sawtooth':
                t, s = self.generator.generate_sawtooth(freq, duration)
            elif signal_type == 'pulse':
                t, s = self.generator.generate_pulse(freq, duration)
            else:
                raise ValueError(f"Неподдерживаемый тип сигнала: {signal_type}")

            signals.append((t, s))

        return self.combine_signals(signals, weights=harmonic_weights)

    def create_stereo_signal(self, left_signals, right_signals, weights_left=None, weights_right=None):
        time_left, left_channel = self.combine_signals(left_signals, weights_left)
        time_right, right_channel = self.combine_signals(right_signals, weights_right)

        min_length = min(len(left_channel), len(right_channel))
        left_channel = left_channel[:min_length]
        right_channel = right_channel[:min_length]
        time_array = time_left[:min_length]

        stereo_signal = np.column_stack((left_channel, right_channel))

        return time_array, stereo_signal
