import numpy as np
import wave
import struct
import os


class WaveFileManager:
    def __init__(self, sample_rate=44100, bit_depth=16):
        """
        Args:
            sample_rate (int): Частота дискретизации в Гц
            bit_depth (int): Разрядность (8, 16, 24, 32)
        """
        self.sample_rate = sample_rate
        self.bit_depth = bit_depth

    def save_mono_wav(self, filename, signal_data, normalize=True):
        """
        Args:
            filename (str): Имя файла
            signal_data (numpy.ndarray): Данные сигнала
            normalize (bool): Нормализовать сигнал перед сохранением
        """
        os.makedirs(os.path.dirname(filename) if os.path.dirname(filename) else '.', exist_ok=True)

        if normalize:
            signal_data = WaveFileManager._normalize_for_wav(signal_data)

        # Конвертируем в нужный формат
        if self.bit_depth == 16:
            # 16-bit signed integer
            signal_data = np.int16(signal_data * 32767)
            sample_width = 2
        elif self.bit_depth == 32:
            # 32-bit float
            signal_data = np.float32(signal_data)
            sample_width = 4
        else:
            raise ValueError(f"Неподдерживаемая разрядность: {self.bit_depth}")

        with wave.open(filename, 'wb') as wav_file:
            wav_file.setnchannels(1)  # моно
            wav_file.setsampwidth(sample_width)
            wav_file.setframerate(self.sample_rate)

            # Записываем данные
            if self.bit_depth == 16:
                wav_file.writeframes(signal_data.tobytes())
            elif self.bit_depth == 32:
                # Для 32-bit float нужно использовать struct
                for sample in signal_data:
                    wav_file.writeframes(struct.pack('<f', sample))

    def save_stereo_wav(self, filename, left_channel, right_channel, normalize=True):
        """
        Args:
            filename (str): Имя файла
            left_channel (numpy.ndarray): Левый канал
            right_channel (numpy.ndarray): Правый канал
            normalize (bool): Нормализовать сигнал перед сохранением
        """
        os.makedirs(os.path.dirname(filename) if os.path.dirname(filename) else '.', exist_ok=True)

        # Убеждаемся, что каналы имеют одинаковую длину
        min_length = min(len(left_channel), len(right_channel))
        left_channel = left_channel[:min_length]
        right_channel = right_channel[:min_length]

        # Нормализация
        if normalize:
            left_channel = WaveFileManager._normalize_for_wav(left_channel)
            right_channel = WaveFileManager._normalize_for_wav(right_channel)

        # Конвертируем в нужный формат
        if self.bit_depth == 16:
            left_channel = np.int16(left_channel * 32767)
            right_channel = np.int16(right_channel * 32767)
            sample_width = 2
        elif self.bit_depth == 32:
            left_channel = np.float32(left_channel)
            right_channel = np.float32(right_channel)
            sample_width = 4
        else:
            raise ValueError(f"Неподдерживаемая разрядность: {self.bit_depth}")

        with wave.open(filename, 'wb') as wav_file:
            wav_file.setnchannels(2)  # стерео
            wav_file.setsampwidth(sample_width)
            wav_file.setframerate(self.sample_rate)

            # Записываем данные поочередно (LRLRLR)
            if self.bit_depth == 16:
                stereo_data = np.empty(len(left_channel) * 2, dtype=np.int16)
                stereo_data[0::2] = left_channel
                stereo_data[1::2] = right_channel
                wav_file.writeframes(stereo_data.tobytes())
            elif self.bit_depth == 32:
                for i in range(len(left_channel)):
                    wav_file.writeframes(struct.pack('<f', left_channel[i]))
                    wav_file.writeframes(struct.pack('<f', right_channel[i]))

    def save_from_stereo_array(self, filename, stereo_array, normalize=True):
        if stereo_array.ndim != 2 or stereo_array.shape[1] != 2:
            raise ValueError("Стерео массив должен иметь форму (samples, 2)")

        left_channel = stereo_array[:, 0]
        right_channel = stereo_array[:, 1]

        self.save_stereo_wav(filename, left_channel, right_channel, normalize)

    @staticmethod
    def _normalize_for_wav(signal_data):
        max_amplitude = np.max(np.abs(signal_data))
        if max_amplitude > 0:
            return signal_data / max_amplitude
        return signal_data

    @staticmethod
    def load_wav(filename):
        with wave.open(filename, 'rb') as wav_file:
            frames = wav_file.getnframes()
            sample_rate = wav_file.getframerate()
            sample_width = wav_file.getsampwidth()
            channels = wav_file.getnchannels()

            raw_data = wav_file.readframes(frames)

            if sample_width == 2:  # 16-bit
                signal_data = np.frombuffer(raw_data, dtype=np.int16)
                signal_data = signal_data.astype(np.float32) / 32767.0
            elif sample_width == 4:  # 32-bit float
                signal_data = np.frombuffer(raw_data, dtype=np.float32)
            else:
                raise ValueError(f"Неподдерживаемая разрядность: {sample_width * 8}")

            if channels == 2:  # стерео
                signal_data = signal_data.reshape(-1, 2)

            return sample_rate, signal_data

    @staticmethod
    def get_file_info(filename):
        with wave.open(filename, 'rb') as wav_file:
            info = {
                'frames': wav_file.getnframes(),
                'sample_rate': wav_file.getframerate(),
                'sample_width': wav_file.getsampwidth(),
                'channels': wav_file.getnchannels(),
                'duration': wav_file.getnframes() / wav_file.getframerate()
            }
            return info
