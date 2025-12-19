import numpy as np
from signal_generators import SignalGenerator, sawtooth, square


class SignalModulation:
    def __init__(self, sample_rate=44100):
        """
        Args:
            sample_rate (int): Частота дискретизации в Гц
        """
        self.sample_rate = sample_rate
        self.generator = SignalGenerator(sample_rate)

    def amplitude_modulation(self, carrier_freq, carrier_duration,
                             modulating_freq, modulation_depth=1,
                             carrier_type='sine', modulating_type='sine'):
        """
        Амплитудная модуляция (AM)
        
        Args:
            carrier_freq (float): Частота несущего сигнала в Гц
            carrier_duration (float): Длительность сигнала в секундах
            modulating_freq (float): Частота модулирующего сигнала в Гц
            modulation_depth (float): Глубина модуляции (0 до 1)
            carrier_type (str): Тип несущего сигнала
            modulating_type (str): Тип модулирующего сигнала
            
        Returns:
            tuple: (time_array, modulated_signal)
        """

        # Генерируем несущий сигнал
        t, carrier = self._generate_signal(carrier_type, carrier_freq,
                                           carrier_duration)

        # Генерируем модулирующий сигнал
        _, modulating = self._generate_signal(modulating_type, modulating_freq,
                                              carrier_duration)

        # Нормализуем модулирующий сигнал к диапазону [-1, 1]
        # FIXME надо или не надо хз
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

        # нормализуем модулирующий сигнал
        # FIXME хз надо или нет
        # modulating = self.generator.normalize_signal(modulating)

        # мгновенная угловая частота
        # ω_inst(x) = 2π · (1 + y_LFO(x)) · f
        instantaneous_freq = 2 * np.pi * (1 + modulating) * carrier_freq

        dt = 1.0 / self.generator.sample_rate
        instantaneous_phase = np.cumsum(instantaneous_freq * dt)

        if carrier_type == 'sine':
            modulated_signal = np.sin(instantaneous_phase)
        elif carrier_type == 'triangle':
            modulated_signal = sawtooth(instantaneous_phase, duty=0.5)
        elif carrier_type == 'sawtooth':
            modulated_signal = sawtooth(instantaneous_phase, 1.0)
        elif carrier_type == 'pulse':
            modulated_signal = square(instantaneous_phase, 0.5)
        else:
            raise ValueError(f"Неподдерживаемый тип несущего сигнала для FM: {carrier_type}")

        return t, modulated_signal

    def _generate_signal(self, signal_type, frequency, duration):
        """
        Args:
            signal_type (str): Тип сигнала
            frequency (float): Частота в Гц
            duration (float): Длительность в секундах
            
        Returns:
            tuple: (time_array, signal_array)
        """
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
