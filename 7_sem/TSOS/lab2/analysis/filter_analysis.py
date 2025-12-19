import matplotlib.pyplot as plt
from scipy.signal import butter, filtfilt, freqz
from typing import Optional, Union
import os
import numpy as np
from typing import Tuple

from analysis.fourier_analysis import FourierAnalyzer
from core.wave_manager import WaveFileManager


class DigitalFilter:
    def __init__(self, sample_rate: int = 44100, output_dir: str = None):
        self.sample_rate = sample_rate
        self.output_dir = output_dir or os.path.join(os.path.dirname(os.path.dirname(__file__)), 'plots')
        self._ensure_output_dir()

    def _ensure_output_dir(self):
        os.makedirs(self.output_dir, exist_ok=True)

    def _generate_filename(self, base_name: str, extension: str = 'png') -> str:
        return os.path.join(self.output_dir, f"{base_name}.{extension}")

    def design_lowpass_filter(self,
                              cutoff_freq: float,
                              order: int = 5,
                              use_manual: bool = True) -> Tuple[np.ndarray, np.ndarray]:
        nyquist_freq = self.sample_rate / 2  # частота Найквиста

        if not use_manual:
            normalized_cutoff = cutoff_freq / nyquist_freq  # нормализация частоты
            b, a = butter(order, normalized_cutoff, 'low', analog=False)  # фильтр Баттерворта готовый
            return b, a
        else:
            wc = cutoff_freq / nyquist_freq  # нормализованная частота

            if wc >= 1.0:
                raise ValueError(f"Частота среза должна быть < {nyquist_freq} Гц")

            omega_c = np.tan(np.pi * wc / 2)  # билинейное преобразование

            poles = []
            for k in range(order):
                theta = np.pi * (2 * k + order + 1) / (2 * order)  # углы полюсов
                pole = omega_c * np.exp(1j * theta)  # аналоговые полюса
                poles.append(pole)

            poles = np.array(poles)

            z_poles = (1 + poles / 2) / (1 - poles / 2)  # полюса в Z-плоскости
            z_zeros = -np.ones(order)  # нули на z = -1

            a = np.poly(z_poles)  # знаменатель по полюсам
            b = np.poly(z_zeros)  # числитель по нулям

            gain = np.sum(a) / np.sum(b)  # расчет усиления
            b = b * gain  # нормализация усиления

            b = b / a[0]  # нормировка a[0] = 1
            a = a / a[0]

            return b, a  # коэффициенты фильтра

    def design_highpass_filter(self,
                               cutoff_freq: float,
                               order: int = 5,
                               use_manual: bool = True) -> Tuple[np.ndarray, np.ndarray]:
        nyquist_freq = self.sample_rate / 2
        if not use_manual:
            normalized_cutoff = cutoff_freq / nyquist_freq
            b, a = butter(order, normalized_cutoff, btype='high', analog=False)
            return b, a
        else:
            wc = cutoff_freq / nyquist_freq

            if wc >= 1.0:
                raise ValueError(f"Частота среза должна быть < {nyquist_freq} Гц")

            omega_c = np.tan(np.pi * wc / 2)

            poles = []
            for k in range(order):
                theta = np.pi * (2 * k + order + 1) / (2 * order)
                pole = omega_c * np.exp(1j * theta)
                poles.append(pole)

            poles = np.array(poles)

            z_poles = (1 + poles / 2) / (1 - poles / 2)

            z_zeros = np.ones(order)

            a = np.poly(z_poles)
            b = np.poly(z_zeros)

            gain = np.polyval(a, -1) / np.polyval(b, -1)
            b = b * gain

            b = b / a[0]
            a = a / a[0]

            return b, a

    def design_bandpass_filter(self,
                               low_freq: float,
                               high_freq: float,
                               order: int = 5,
                               use_manual: bool = True) -> Tuple[np.ndarray, np.ndarray]:
        nyquist_freq = self.sample_rate / 2
        if not use_manual:
            normalized_low = low_freq / nyquist_freq
            normalized_high = high_freq / nyquist_freq

            # if normalized_low >= 1 or normalized_high >= 1:
            #     raise ValueError(f"Частоты среза должны быть меньше половины частоты дискретизации {nyquist_freq} Гц")

            if low_freq >= high_freq:
                raise ValueError("Нижняя частота должна быть меньше верхней частоты")

            b, a = butter(order, [normalized_low, normalized_high], btype='band', analog=False)
            return b, a
        else:
            """
                    Проектирование полосового фильтра

                    Формулы:
                    1. Центральная частота: fc = √(f_low · f_high)
                    2. Ширина полосы: BW = f_high - f_low
                    3. Преобразование ФНЧ -> ПФ: s -> (s² + ωc²)/(s·BW)

                    Альтернативный метод (каскадное соединение):
                    H_bp(z) = H_hp(z) · H_lp(z)
                    где H_hp - ФВЧ с частотой f_low, H_lp - ФНЧ с частотой f_high

                    БИХ-фильтр второго порядка (biquad):
                    H(z) = (b0 + b1·z^(-1) + b2·z^(-2)) / (1 + a1·z^(-1) + a2·z^(-2))

                    Коэффициенты для прямой формы II:
                    ω0 = 2πfc/fs - центральная частота
                    BW = 2π·Δf/fs - ширина полосы
                    α = sin(ω0)·sinh(ln(2)/2 · BW · ω0/sin(ω0))

                    b0 = α
                    b1 = 0
                    b2 = -α
                    a0 = 1 + α
                    a1 = -2·cos(ω0)
                    a2 = 1 - α

                    Args:
                        low_freq: Нижняя частота среза в Гц
                        high_freq: Верхняя частота среза в Гц
                        order: Порядок фильтра

                    Returns:
                        b, a: Коэффициенты фильтра
                    """
            if low_freq >= high_freq:
                raise ValueError("Нижняя частота должна быть < верхней частоты")

            # Нормализация частот
            w_low = low_freq / nyquist_freq
            w_high = high_freq / nyquist_freq

            if w_high >= 1.0:
                raise ValueError(f"Верхняя частота должна быть < {nyquist_freq} Гц")

            # Метод 1: Каскадное соединение ФВЧ и ФНЧ
            # Проектируем ФВЧ с частотой low_freq
            b_hp, a_hp = self.design_highpass_filter(low_freq, order, use_manual=True)

            # Проектируем ФНЧ с частотой high_freq
            b_lp, a_lp = self.design_lowpass_filter(high_freq, order, use_manual=True)

            # Перемножение передаточных функций
            # H(z) = H_hp(z) · H_lp(z)
            from scipy.signal import convolve
            b = convolve(b_hp, b_lp)
            a = convolve(a_hp, a_lp)

            a = np.atleast_1d(a)
            b = np.atleast_1d(b)

            # Нормализация
            b = b / a[0]
            a = a / a[0]

            return b, a

    def apply_filter(self,
                     signal: np.ndarray,
                     b: np.ndarray,
                     a: np.ndarray,
                     use_manual: bool = True) -> np.ndarray:
        if not use_manual:
            filtered_signal = filtfilt(b, a, signal)
            return filtered_signal
        else:
            """
                    Применение фильтра с нулевым фазовым сдвигом (метод filtfilt)

                    Алгоритм:
                    1. Прямая фильтрация: y1 = filter(x)
                    2. Обратная фильтрация: y2 = filter(reverse(y1))
                    3. Результат: y = reverse(y2)

                    Эффект:
                    - Амплитудная характеристика: |H(ω)|² (квадрат оригинальной)
                    - Фазовая характеристика: 0 (нулевой сдвиг фазы)

                    Args:
                        signal: Входной сигнал
                        b, a: Коэффициенты фильтра

                    Returns:
                        Отфильтрованный сигнал без фазового сдвига
                    """
            # Прямая фильтрация
            y_forward = self.apply_filter_direct_form_ii(signal, b, a)

            # Обратная фильтрация
            y_reverse = self.apply_filter_direct_form_ii(y_forward[::-1], b, a)

            # Переворот обратно
            y_final = y_reverse[::-1]

            return y_final

    def get_filter_response(self,
                            b: np.ndarray,
                            a: np.ndarray,
                            num_points: int = 1024) -> Tuple[np.ndarray, np.ndarray, np.ndarray]:
        w, h = freqz(b, a, worN=num_points, fs=self.sample_rate)
        magnitude = np.abs(h)
        phase = np.angle(h)

        return w, magnitude, phase

    def analyze_filter_effect(self, signal: np.ndarray, time: np.ndarray,
                              filter_type: str,
                              cutoff_freqs: Union[float, Tuple[float, float]],
                              order: int = 5, title: str = "Анализ фильтрации",
                              save_plot: bool = True,
                              save_path: Optional[str] = None,
                              use_manual: bool = True,
                              is_noise: bool = False) -> dict:
        """
        Полный анализ эффекта фильтрации с построением графиков
        
        Args:
            signal: входной сигнал
            time: массив времени
            filter_type: тип фильтра ('lowpass', 'highpass', 'bandpass')
            cutoff_freqs: частота(ы) среза
            order: порядок фильтра
            title: заголовок графиков
            save_path: путь для сохранения графика
            save_plot:
            use_manual:
        """
        if filter_type == 'lowpass':
            b, a = self.design_lowpass_filter(cutoff_freqs, order, use_manual=use_manual)
            filter_name = f"НЧ-фильтр (fc = {cutoff_freqs} Гц)"
        elif filter_type == 'highpass':
            b, a = self.design_highpass_filter(cutoff_freqs, order, use_manual=use_manual)
            filter_name = f"ВЧ-фильтр (fc = {cutoff_freqs} Гц)"
        elif filter_type == 'bandpass':
            b, a = self.design_bandpass_filter(cutoff_freqs[0], cutoff_freqs[1], order, use_manual=use_manual)
            filter_name = f"Полосовой фильтр ({cutoff_freqs[0]}-{cutoff_freqs[1]} Гц)"
        else:
            raise ValueError(f"Неподдерживаемый тип фильтра: {filter_type}")

        filtered_signal = self.apply_filter(signal, b, a)
        if is_noise:
            wav = WaveFileManager(sample_rate=44100, bit_depth=16)
            wav.save_mono_wav(os.path.join("plots", f'white_noise_{filter_type}.wav'), filtered_signal,
                              normalize=True)
            return;
        freqs_filter, mag_filter, phase_filter = self.get_filter_response(b, a)

        from scipy.fft import fftfreq

        X_original = FourierAnalyzer.fft_manual(signal)
        X_filtered = FourierAnalyzer.fft_manual(filtered_signal)
        freqs_signal: np.ndarray = fftfreq(len(signal), 1 / self.sample_rate)

        fig, axes = plt.subplots(1, 2, figsize=(15, 12))
        fig.suptitle(f"{title}: {filter_name}", fontsize=16)

        axes[0].plot(time, signal, label='Исходный сигнал', alpha=0.8)
        axes[0].plot(time, filtered_signal, label='Отфильтрованный сигнал', alpha=0.8)
        axes[0].set_title('Временные сигналы')
        axes[0].set_xlabel('Время (с)')
        axes[0].set_ylabel('Амплитуда')
        axes[0].legend()
        axes[0].grid(True)

        n_half = freqs_signal.shape[0] // 2
        axes[1].plot(freqs_signal[:n_half], np.abs(X_original[:n_half]),
                        label='Исходный спектр', alpha=0.8)
        axes[1].plot(freqs_signal[:n_half], np.abs(X_filtered[:n_half]),
                        label='Отфильтрованный спектр', alpha=0.8)
        axes[1].set_title('Амплитудные спектры сигналов')
        axes[1].set_xlabel('Частота (Гц)')
        axes[1].set_ylabel('Амплитуда')
        axes[1].legend()
        axes[1].grid(True)
        axes[1].set_xlim(0, self.sample_rate / 6)

        # Разность спектров
        # spectrum_diff = np.abs(X_original[:n_half]) - np.abs(X_filtered[:n_half])
        # axes[2, 1].plot(freqs_signal[:n_half], spectrum_diff)
        # axes[2, 1].set_title('Разность спектров (исходный - отфильтрованный)')
        # axes[2, 1].set_xlabel('Частота (Гц)')
        # axes[2, 1].set_ylabel('Разность амплитуд')
        # axes[2, 1].grid(True)

        plt.tight_layout()

        if save_plot:
            if save_path is None:
                safe_title = "".join(c if c.isalnum() or c in (' ', '-', '_') else '' for c in title)
                safe_title = safe_title.replace(' ', '_')
                save_path = self._generate_filename(f"filter_analysis_{filter_type}_{safe_title}")

            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"График сохранен: {save_path}")

        #plt.show()

        return {
            'filtered_signal': filtered_signal,
            'filter_coefficients': (b, a),
            'filter_response': (freqs_filter, mag_filter, phase_filter),
            'original_spectrum': X_original,
            'filtered_spectrum': X_filtered,
            'spectrum_freqs': freqs_signal
        }

    @staticmethod
    def apply_filter_direct_form_ii(signal: np.ndarray,
                                    b: np.ndarray,
                                    a: np.ndarray) -> np.ndarray:
        """
        Применение фильтра методом прямой формы II

        Разностное уравнение:
        y[n] = (1/a[0]) · (b[0]·x[n] + b[1]·x[n-1] + ... + b[M]·x[n-M]
                          - a[1]·y[n-1] - a[2]·y[n-2] - ... - a[N]·y[n-N])

        Прямая форма II использует промежуточные переменные состояния:
        w[n] = x[n] - a[1]·w[n-1] - a[2]·w[n-2] - ... - a[N]·w[n-N]
        y[n] = b[0]·w[n] + b[1]·w[n-1] + b[2]·w[n-2] + ... + b[M]·w[n-M]

        Args:
            signal: Входной сигнал
            b: Коэффициенты числителя
            a: Коэффициенты знаменателя

        Returns:
            Отфильтрованный сигнал
        """
        N = len(a) - 1  # Порядок знаменателя
        M = len(b) - 1  # Порядок числителя

        # Нормализация по a[0]
        if a[0] != 1.0:
            b = b / a[0]
            a = a / a[0]

        # Инициализация выходного сигнала
        y = np.zeros_like(signal, dtype=complex)

        # Буферы для хранения предыдущих значений
        x_buffer = np.zeros(M + 1, dtype=complex)  # x[n], x[n-1], ..., x[n-M]
        y_buffer = np.zeros(N + 1, dtype=complex)  # y[n], y[n-1], ..., y[n-N]

        # Обработка каждого отсчета
        for n in range(len(signal)):
            # Сдвиг буферов
            x_buffer[1:] = x_buffer[:-1]
            y_buffer[1:] = y_buffer[:-1]

            # Новый входной отсчет
            x_buffer[0] = signal[n]

            # Вычисление выходного отсчета
            # y[n] = sum(b[k]·x[n-k]) - sum(a[k]·y[n-k])
            y_buffer[0] = 0

            # Часть с числителем (FIR)
            for k in range(min(M + 1, n + 1)):
                y_buffer[0] += b[k] * x_buffer[k]

            # Часть со знаменателем (IIR, обратная связь)
            for k in range(1, min(N + 1, n + 1)):
                y_buffer[0] -= a[k] * y_buffer[k]

            y[n] = y_buffer[0]

        return y.real
