import numpy as np

from core.signal_generators import SignalGenerator


class SignalModulation:
    def __init__(self, sample_rate=44100):
        self.sample_rate = sample_rate
        self.generator = SignalGenerator(sample_rate)

    def amplitude_modulation(self, carrier_freq, carrier_duration,
                             modulating_freq, modulation_depth=1,
                             carrier_type='sine', modulating_type='sine'):

        t, carrier = self._generate_signal(carrier_type, carrier_freq,
                                           carrier_duration)

        _, modulating = self._generate_signal(modulating_type, modulating_freq,
                                              carrier_duration)

        # modulating = self.generator.normalize_signal(modulating)

        # y(t) = A * (1 + m * m(t)) * c(t)
        # m - глубина модуляции, m(t) - модулирующий сигнал, c(t) - несущий
        modulated_signal = (1 + modulation_depth * modulating) * carrier

        return t, modulated_signal

    def frequency_modulation(self, carrier_freq, carrier_duration,
                             modulating_freq, carrier_type='sine',
                             modulating_type='sine'):

        t = self.generator.generate_time_array(carrier_duration)

        _, modulating = self._generate_signal(modulating_type, modulating_freq,
                                              carrier_duration)

        # modulating = self.generator.normalize_signal(modulating)

        # мгновенная угловая частота
        # ω_inst(x) = 2π · (1 + y_LFO(x)) · f
        instantaneous_freq = 2 * np.pi * (1 + modulating) * carrier_freq

        dt = 1.0 / self.generator.sample_rate
        instantaneous_phase = np.cumsum(instantaneous_freq * dt)

        if carrier_type == 'sine':
            modulated_signal = np.sin(instantaneous_phase)
        elif carrier_type == 'triangle':
            modulated_signal = SignalGenerator.sawtooth(instantaneous_phase, 0.5)
        elif carrier_type == 'sawtooth':
            modulated_signal = SignalGenerator.sawtooth(instantaneous_phase, 1.0)
        elif carrier_type == 'pulse':
            modulated_signal = SignalGenerator.square(instantaneous_phase, 0.5)
        else:
            raise ValueError(f"Неподдерживаемый тип несущего сигнала для FM: {carrier_type}")

        return t, modulated_signal

    def _generate_signal(self, signal_type, frequency, duration):
        if signal_type == 'sine':
            return self.generator.generate_sine(frequency, duration)
        elif signal_type == 'triangle':
            return self.generator.generate_triangle(frequency, duration)
        elif signal_type == 'sawtooth':
            return self.generator.generate_sawtooth(frequency, duration)
        elif signal_type == 'pulse':
            return self.generator.generate_pulse(frequency, duration)
        else:
            raise ValueError(f"Неподдерживаемый тип сигнала: {signal_type}")
