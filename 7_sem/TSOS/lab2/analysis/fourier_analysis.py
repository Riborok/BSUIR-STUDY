import os
from typing import Tuple, Optional
import matplotlib.pyplot as plt
import numpy as np
from scipy.fft import fftfreq


class FourierAnalyzer:
    def __init__(self, sample_rate: int = 44100, output_dir: str = None):
        self.sample_rate = sample_rate
        self.output_dir = output_dir or os.path.join(os.path.dirname(os.path.dirname(__file__)), 'plots')
        self._ensure_output_dir()

    def _ensure_output_dir(self):
        os.makedirs(self.output_dir, exist_ok=True)

    def _generate_filename(self, base_name: str, extension: str = 'png') -> str:
        return os.path.join(self.output_dir, f"{base_name}.{extension}")

    @staticmethod
    def dft_manual(x: np.ndarray) -> np.ndarray:
        N = len(x)
        X = np.zeros(N, dtype=complex)

        for k in range(N):
            for n in range(N):
                X[k] += x[n] * np.exp(-2j * np.pi * k * n / N)

        return X / N * 2

    @staticmethod
    def idft_manual(X: np.ndarray) -> np.ndarray:
        N = len(X)
        x = np.zeros(N, dtype=complex)

        for n in range(N):
            for k in range(N):
                x[n] += X[k] * np.exp(2j * np.pi * k * n / N)

        return x / 2

    @staticmethod
    def fft_manual(P: np.ndarray) -> np.ndarray:
        n = len(P)
        if n == 1:
            return P

        if n & (n - 1) != 0:
            return np.fft.fft(P)

        y_even = FourierAnalyzer.fft_manual(P[::2])
        y_odd = FourierAnalyzer.fft_manual(P[1::2])

        y = np.zeros(n, dtype=complex)

        k_values = np.arange(n // 2)
        w_k = np.exp(-2j * np.pi * k_values / n)

        y[:n // 2] = y_even + w_k * y_odd
        y[n // 2:] = y_even - w_k * y_odd

        return y / n * 2

    @staticmethod
    def ifft_manual(Y: np.ndarray) -> np.ndarray:
        """
        A recursive implementation of the
        1D Cooley-Tukey IFFT using numpy arrays
        """
        # Y - [Y0, Y1,... Yn-1] frequency domain representation
        n = len(Y)  # n is a power of 2
        if n == 1:
            return Y

        # Check if n is a power of 2, fallback to numpy if not
        if n & (n - 1) != 0:
            return np.fft.ifft(Y)

        # Recursive calls on even and odd indices
        y_even = FourierAnalyzer.ifft_manual(Y[::2])
        y_odd = FourierAnalyzer.ifft_manual(Y[1::2])

        # Initialize result array with complex dtype
        y = np.zeros(n, dtype=complex)

        # Vectorized computation of twiddle factors (positive exponent for IFFT)
        k_values = np.arange(n // 2)
        w_k = np.exp(2j * np.pi * k_values / n)

        # Compute butterfly operations
        y[:n // 2] = y_even + w_k * y_odd
        y[n // 2:] = y_even - w_k * y_odd

        # Scale by 1/n for the final result (only at the top level)
        # Note: This scaling should be done outside this function or
        # you need to track if this is the top-level call
        return y * 2

    @staticmethod
    def ifft_manual_scaled(Y: np.ndarray) -> np.ndarray:
        """
        IFFT with proper 1/n scaling
        """
        result = FourierAnalyzer.ifft_manual(Y)
        return result / 2

    def signal_analysis(self,
                        signal: np.ndarray,
                        use_manual: bool = True,
                        use_discrete: bool = True) -> Tuple[np.ndarray, np.ndarray, np.ndarray]:
        """
        Returns:
            freqs: массив частот
            magnitude: амплитудный спектр
            phase: фазовый спектр
        """
        if use_manual:
            if len(signal) > 1024:
                print(f"Внимание: ручное ДПФ для {len(signal)} отсчетов может быть медленным")
            if use_discrete:
                X = self.dft_manual(signal)
            else:
                X = self.fft_manual(signal)
        else:
            X = np.fft.fft(signal) / (len(signal)) * 2

        freqs = fftfreq(len(signal), 1 / self.sample_rate)

        magnitude = np.abs(X)
        phase = np.angle(X)

        return freqs, magnitude, phase

    def reconstruct_signal(self,
                           magnitude: np.ndarray,
                           phase: np.ndarray,
                           use_manual: bool = True,
                           use_discrete: bool = True) -> np.ndarray:
        X = magnitude * np.exp(1j * phase)

        if use_manual:
            if use_discrete:
                reconstructed = self.idft_manual(X)
            else:
                reconstructed = self.ifft_manual(X)
        else:
            # np version
            reconstructed = np.fft.ifft(X) * (len(X)) / 2

        return np.real(reconstructed)

    def analyze_and_plot(self, signal: np.ndarray, time: np.ndarray,
                         title: str = "Анализ сигнала",
                         use_manual: bool = True,
                         use_discrete: bool = True,
                         save_plot: bool = True,
                         save_path: Optional[str] = None) -> dict:

        freqs, magnitude, phase = self.signal_analysis(signal, use_manual, use_discrete)
        reconstructed = self.reconstruct_signal(magnitude, phase, use_manual, use_discrete)

        fig, axes = plt.subplots(2, 3, figsize=(15, 10))
        fig.suptitle(f"{title} ({'ДПФ' if use_discrete else 'БПФ'})", fontsize=16)

        axes[0, 0].plot(time, signal)
        axes[0, 0].set_title('Исходный сигнал')
        axes[0, 0].set_xlabel('Время (с)')
        axes[0, 0].set_ylabel('Амплитуда')
        axes[0, 0].grid(True)

        # Показываем только положительные частоты до половины частоты дискретизации
        n_half = len(freqs) // 2
        axes[0, 1].plot(freqs[:n_half], magnitude[:n_half])
        axes[0, 1].set_title('Амплитудный спектр')
        axes[0, 1].set_xlabel('Частота (Гц)')
        axes[0, 1].set_ylabel('Амплитуда')
        axes[0, 1].grid(True)
        ymax = np.max(magnitude[:n_half]) if n_half > 0 else 1.0
        axes[0, 1].set_ylim(0, ymax * 1.1)

        axes[0, 2].plot(freqs[:n_half], phase[:n_half])
        axes[0, 2].set_title('Фазовый спектр')
        axes[0, 2].set_xlabel('Частота (Гц)')
        axes[0, 2].set_ylabel('Фаза (рад)')
        axes[0, 2].grid(True)

        axes[1, 0].plot(time, reconstructed)
        axes[1, 0].set_title('Восстановленный сигнал')
        axes[1, 0].set_xlabel('Время (с)')
        axes[1, 0].set_ylabel('Амплитуда')
        axes[1, 0].grid(True)

        axes[1, 1].plot(time, signal, label='Исходный', alpha=0.7)
        axes[1, 1].plot(time, reconstructed, '--', label='Восстановленный', alpha=0.7)
        axes[1, 1].set_title('Сравнение сигналов')
        axes[1, 1].set_xlabel('Время (с)')
        axes[1, 1].set_ylabel('Амплитуда')
        axes[1, 1].legend()
        axes[1, 1].grid(True)

        plt.tight_layout()

        if save_plot:
            if save_path is None:
                safe_title = "".join(c if c.isalnum() or c in (' ', '-', '_') else '' for c in title)
                safe_title = safe_title.replace(' ', '_')
                method = 'ДПФ' if use_discrete else 'БПФ'
                save_path = self._generate_filename(f"fourier_analysis_{method}_{safe_title}")

            plt.savefig(save_path, dpi=300, bbox_inches='tight')
            print(f"График сохранен: {save_path}")

        #plt.show()

        return {
            'freqs': freqs,
            'magnitude': magnitude,
            'phase': phase,
            'reconstructed': reconstructed,
        }
