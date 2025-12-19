import numpy as np
from numpy import asarray, zeros, place, extract
from scipy import signal


class SignalGenerator:
    def __init__(self, sample_rate=44100):
        self.sample_rate = sample_rate

    def generate_time_array(self, duration):
        return np.linspace(0, duration, int(self.sample_rate * duration), endpoint=False)

    def generate_sine(self, frequency, duration, amplitude=1.0, phase=0):
        t = self.generate_time_array(duration)
        signal_data = amplitude * np.sin(2 * np.pi * frequency * t + phase)
        return t, signal_data

    def generate_pulse(self, frequency, duration, duty_cycle=0.5, amplitude=1.0):
        t = self.generate_time_array(duration)
        signal_data = amplitude * square(2 * np.pi * frequency * t, duty=duty_cycle)
        return t, signal_data

    def generate_triangle(self, frequency, duration, amplitude=1.0):
        t = self.generate_time_array(duration)
        signal_data = amplitude * sawtooth(2 * np.pi * frequency * t, duty=0.5)
        return t, signal_data

    def generate_sawtooth(self, frequency, duration, amplitude=1.0, width=1.0):
        t = self.generate_time_array(duration)
        signal_data = amplitude * sawtooth(2 * np.pi * frequency * t, duty=width)
        return t, signal_data

    def generate_noise(self, duration, noise_type='white', amplitude=1.0):
        t = self.generate_time_array(duration)
        num_samples = len(t)

        if noise_type == 'white':
            noise_data = amplitude * np.random.uniform(-1, 1, num_samples)
        else:
            raise ValueError(f"Неподдерживаемый тип шума: {noise_type}")

        return t, noise_data

    @staticmethod
    def normalize_signal(signal_data, target_amplitude=1.0):
        max_amplitude = np.max(np.abs(signal_data))
        if max_amplitude > 0:
            return signal_data * (target_amplitude / max_amplitude)
        return signal_data


def sawtooth(t, duty=1.0):
    t, d = asarray(t), asarray(duty)
    d = asarray(d + (t - t))
    t = asarray(t + (d - d))
    y = zeros(t.shape, dtype="d")

    # duty must be between 0 and 1 inclusive
    mask1 = (d > 1) | (d < 0)
    place(y, mask1, np.nan)

    # take t modulo 2*pi
    tmod = np.mod(t, 2 * np.pi)

    # on the interval 0 to width*2*pi function is
    # tmod / (pi*w) - 1
    mask2 = (1 - mask1) & (tmod < d * 2 * np.pi)
    tsub = extract(mask2, tmod)
    wsub = extract(mask2, d)
    place(y, mask2, tsub / (np.pi * wsub) - 1)

    # on the interval width*2*pi to 2*pi function is
    # (pi*(w+1)-tmod) / (pi*(1-w))
    mask3 = (1 - mask1) & (1 - mask2)
    tsub = extract(mask3, tmod)
    wsub = extract(mask3, d)
    place(y, mask3, (np.pi * (wsub + 1) - tsub) / (np.pi * (1 - wsub)))
    return y


def square(t, duty=0.5):
    t, d = asarray(t), asarray(duty)
    d = asarray(d + (t - t))  # получение соизмеримых форм, чтобы растянуть до нужной
    t = asarray(t + (d - d))
    y = zeros(t.shape, dtype="d")

    # duty must be between 0 and 1 inclusive
    mask1 = (d > 1) | (d < 0)
    place(y, mask1, np.nan)

    # on the interval 0 to duty*2*pi function is 1
    tmod = np.mod(t, 2 * np.pi)  # mod(fi, 2pi)
    mask2 = (1 - mask1) & (tmod <= d * 2 * np.pi)  # area, where phase < 2pi * duty and duty is correct
    place(y, mask2, 1)

    mask3 = (1 - mask1) & (1 - mask2)  # area, where phase > 2pi * duty and duty is correct
    place(y, mask3, -1)
    return y