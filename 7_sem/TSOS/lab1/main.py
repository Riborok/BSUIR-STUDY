import os
import numpy as np

from signal_generators import SignalGenerator
from polyphonic_signals import PolyphonicSignals
from signal_modulation import SignalModulation
from wave_manager import WaveFileManager


def ensure_out_dir(path):
    os.makedirs(path, exist_ok=True)


def demo_basic_signals(out_dir, sr=44100, duration=2.0, frequency=440, amplitude=0.6, normalize=True):
    gen = SignalGenerator(sample_rate=sr)
    wav = WaveFileManager(sample_rate=sr, bit_depth=16)

    # sin
    t, s = gen.generate_sine(frequency, duration, amplitude=amplitude)
    wav.save_mono_wav(os.path.join(out_dir, f'sine_{frequency}Hz.wav'), s, normalize=normalize)

    # pulse
    for duty in [0.2, 0.5, 0.8, 1.0]:
        t, p = gen.generate_pulse(frequency, duration, duty_cycle=duty, amplitude=amplitude)
        wav.save_mono_wav(os.path.join(out_dir, f'pulse_{frequency}Hz_duty{int(duty * 100)}.wav'), p,
                          normalize=normalize)

    # triangle
    t, tr = gen.generate_triangle(frequency, duration, amplitude=amplitude)
    wav.save_mono_wav(os.path.join(out_dir, f'triangle_{frequency}Hz.wav'), tr, normalize=normalize)

    # sawtooth (rising width=1.0; falling width=0.0)
    t, sw = gen.generate_sawtooth(frequency, duration, amplitude=amplitude, width=1.0)
    wav.save_mono_wav(os.path.join(out_dir, f'sawtooth_rising_{frequency}Hz.wav'), sw, normalize=normalize)
    t, sf = gen.generate_sawtooth(frequency, duration, amplitude=amplitude, width=0.0)
    wav.save_mono_wav(os.path.join(out_dir, f'sawtooth_falling_{frequency}Hz.wav'), sf, normalize=normalize)

    # Noise
    for ntype in ['white']:
        t, n = gen.generate_noise(duration, noise_type=ntype, amplitude=amplitude)
        wav.save_mono_wav(os.path.join(out_dir, f'noise_{ntype}.wav'), n, normalize=normalize)


def demo_polyphony(out_dir, sr=44100, duration=2.0, amplitude=0.4, normalize=True):
    poly = PolyphonicSignals(sample_rate=sr)
    gen = SignalGenerator(sample_rate=sr)
    wav = WaveFileManager(sample_rate=sr, bit_depth=16)

    # Chord A minor (A4=440, C5≈523.25, E5≈659.25)
    freqs = [440.0, 523.25, 659.25]
    t, chord = poly.create_chord(freqs, duration, signal_type='sine', amplitude=amplitude)
    wav.save_mono_wav(os.path.join(out_dir, 'poly_chord_Amin_sine.wav'), chord)

    # triangle + sawtooth
    t, tr = gen.generate_triangle(440, duration, amplitude=amplitude)
    t, sw = gen.generate_sawtooth(440, duration, amplitude=amplitude)
    t, signal = poly.combine_signals([(t, tr), (t, sw)])
    wav.save_mono_wav(os.path.join(out_dir, 'poly_sawtooth_and_triangle.wav'), signal)

    # sin + sin
    t, sin = gen.generate_sine(2000, duration, amplitude=0.7)
    t, sin2 = gen.generate_sine(400, duration, amplitude=0.7, phase=np.pi / 4)
    t, signal = poly.combine_signals([(t, sin), (t, sin2)], weights=[1.0, 1.0], normalize=True)
    wav.save_mono_wav(os.path.join(out_dir, 'poly_sin_and_sin.wav'), signal, normalize=True)


def demo_modulation(out_dir, sr=44100, duration=2.0):
    mod = SignalModulation(sample_rate=sr)
    wav = WaveFileManager(sample_rate=sr, bit_depth=16)

    print("Генерация примеров модуляции...")

    # =================
    # АМПЛИТУДНАЯ МОДУЛЯЦИЯ (AM)
    # =================

    # 1. Классическое тремоло - синус модулирует синус
    print("1. AM: Классическое тремоло (sine → sine)")
    t, am_tremolo = mod.amplitude_modulation(
        carrier_freq=440,  # Ля первой октавы
        carrier_duration=duration,
        modulating_freq=4,  # 4 Гц - медленные колебания громкости
        carrier_type='sine',
        modulating_type='sine'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_sine_sine.wav'), am_tremolo, normalize=True)

    # 2. Эффект "вертолета" - прямоугольный LFO
    print("2. AM: Эффект вертолета (sine ← pulse)")
    t, am_helicopter = mod.amplitude_modulation(
        carrier_freq=200,
        carrier_duration=duration,
        modulating_freq=15,  # Быстрые переключения
        modulation_depth=0.9,
        carrier_type='sine',
        modulating_type='pulse'  # Резкие переключения вкл/выкл
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_pulse_sine.wav'), am_helicopter, normalize=True)

    # 3. Мягкое тремоло - треугольник модулирует синус
    print("3. AM: Мягкое тремоло (sine ← triangle)")
    t, am_soft = mod.amplitude_modulation(
        carrier_freq=660,  # Ми второй октавы
        carrier_duration=duration,
        modulating_freq=2,  # Медленная модуляция
        modulation_depth=0.6,
        carrier_type='sine',
        modulating_type='triangle'  # Линейные нарастания/спады
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_triangle_sine.wav'), am_soft, normalize=True)

    # 4. Агрессивная модуляция - прямоугольник модулирует пилообразный
    print("4. AM: Агрессивная модуляция (sawtooth ← pulse)")
    t, am_aggressive = mod.amplitude_modulation(
        carrier_freq=150,
        carrier_duration=duration,
        modulating_freq=8,
        modulation_depth=0.7,
        carrier_type='sawtooth',  # Яркий, резкий тембр
        modulating_type='pulse'  # Резкие переключения
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_pulse_sawtooth.wav'), am_aggressive, normalize=True)

    # =================
    # ЧАСТОТНАЯ МОДУЛЯЦИЯ (FM)
    # =================

    print("7. FM: Сирена (sine ~ triangle)")
    t, fm_siren = mod.frequency_modulation(
        carrier_freq=800,
        carrier_duration=duration,
        modulating_freq=4,
        carrier_type='sine',
        modulating_type='triangle'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_triangle_sine.wav'), fm_siren, normalize=True)

    # 8. Ступенчатые изменения частоты - прямоугольный LFO
    print("8. FM: Ступенчатые изменения (sine ~ pulse)")
    t, fm_stepped = mod.frequency_modulation(
        carrier_freq=330,
        carrier_duration=duration,
        modulating_freq=3,  # 3 переключения в секунду
        carrier_type='sine',
        modulating_type='pulse'  # Резкие скачки частоты
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_pulse_sine.wav'), fm_stepped, normalize=True)

    # 9. Экспериментальная FM - треугольник модулирует пилообразный
    print("9. FM: Экспериментальная (triangle ~ sawtooth)")
    t, fm_experimental = mod.frequency_modulation(
        carrier_freq=180,
        carrier_duration=duration,
        modulating_freq=7,
        carrier_type='triangle',  # Мягкий тембр
        modulating_type='sawtooth'  # Асимметричная модуляция
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_sawtooth_triangle.wav'), fm_experimental, normalize=True)

    # 10. Глубокая FM модуляция - создание сложных тембров
    print("10. FM: Глубокая модуляция (pulse ~ sine)")
    t, fm_deep = mod.frequency_modulation(
        carrier_freq=110,  # Низкая частота
        carrier_duration=duration,
        modulating_freq=220,  # Модуляция в 2 раза выше
        carrier_type='pulse',  # Богатый гармониками
        modulating_type='sine'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_sine_pulse.wav'), fm_deep, normalize=True)

    # =================
    # СРАВНИТЕЛЬНЫЕ ПРИМЕРЫ
    # =================

    # чистый синус для сравнения
    t, reference_sine = mod.generator.generate_sine(440, duration, amplitude=0.8)
    wav.save_mono_wav(os.path.join(out_dir, 'reference_sine_440Hz.wav'), reference_sine, normalize=True)

    # прямоугольник для сравнения
    t, reference_pulse = mod.generator.generate_pulse(220, duration, amplitude=0.8)
    wav.save_mono_wav(os.path.join(out_dir, 'reference_pulse_220Hz.wav'), reference_pulse, normalize=True)


def main():
    out_dir = os.path.join(os.path.dirname(__file__), 'out_wav')
    ensure_out_dir(out_dir)

    sr = 44100
    duration = 3

    # task 1a
    demo_basic_signals(out_dir, sr, duration, frequency=400, amplitude=0.7)

    # task 1b
    demo_polyphony(out_dir, sr, duration)

    # task 1B
    demo_modulation(out_dir, sr, duration)

    print(f'Готово. Файлы сохранены в папку: {out_dir}')


if __name__ == '__main__':
    main()
