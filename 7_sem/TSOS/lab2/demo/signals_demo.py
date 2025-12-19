from core.signal_generators import SignalGenerator
from core.polyphonic_signals import PolyphonicSignals
from core.signal_modulation import SignalModulation
from core.wave_manager import WaveFileManager
from core.utils import note_to_frequency
import numpy as np
import os.path


def demo_basic_signals(out_dir, sr=44100, duration=2.0, frequency=440, amplitude=0.6, normalize=False):
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
    for ntype in ['white', 'pink', 'brown']:
        t, n = gen.generate_noise(duration, noise_type=ntype, amplitude=amplitude)
        wav.save_mono_wav(os.path.join(out_dir, f'noise_{ntype}.wav'), n, normalize=normalize)


def demo_polyphony(out_dir, sr=44100, duration=2.0, amplitude=0.4, normalize=False):
    poly = PolyphonicSignals(sample_rate=sr)
    gen = SignalGenerator(sample_rate=sr)
    wav = WaveFileManager(sample_rate=sr, bit_depth=16)

    # Chord A minor (A4=440, C5≈523.25, E5≈659.25)
    freqs = [note_to_frequency('A4'), note_to_frequency('C5'), note_to_frequency('E5')]
    t, chord = poly.create_chord(freqs, duration, signal_type='sine')
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
    wav.save_mono_wav(os.path.join(out_dir, 'poly_sin_and_sin.wav'), signal, normalize=False)

    t, harm = poly.create_harmonic_series(220.0, num_harmonics=6, duration=duration, signal_type='sine')
    wav.save_mono_wav(os.path.join(out_dir, 'poly_harmonics_220Hz.wav'), harm)

    left = []
    right = []
    for f in freqs:
        left.append(gen.generate_triangle(f, duration, amplitude=amplitude))
        right.append(gen.generate_sawtooth(f, duration, amplitude=amplitude))
    t, stereo = poly.create_stereo_signal(left, right)
    wav.save_from_stereo_array(os.path.join(out_dir, 'stereo_triangle_vs_saw.wav'), stereo)


def demo_modulation(out_dir, sr=44100, duration=2.0):
    mod = SignalModulation(sample_rate=sr)
    wav = WaveFileManager(sample_rate=sr, bit_depth=16)

    print("Генерация примеров модуляции...")

    # =================
    # АМПЛИТУДНАЯ МОДУЛЯЦИЯ (AM)
    # =================

    t, am_tremolo = mod.amplitude_modulation(
        carrier_freq=440,  # Ля первой октавы
        carrier_duration=duration,
        modulating_freq=4,  # 4 Гц - медленные колебания громкости
        modulation_depth=0.8,  # 80% глубина модуляции
        carrier_type='sine',
        modulating_type='sine'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_tremolo_sine.wav'), am_tremolo, normalize=True)

    t, am_helicopter = mod.amplitude_modulation(
        carrier_freq=200,
        carrier_duration=duration,
        modulating_freq=15,  # Быстрые переключения
        modulation_depth=0.9,
        carrier_type='sine',
        modulating_type='pulse'  # Резкие переключения вкл/выкл
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_helicopter.wav'), am_helicopter, normalize=True)

    t, am_soft = mod.amplitude_modulation(
        carrier_freq=660,  # Ми второй октавы
        carrier_duration=duration,
        modulating_freq=2,  # Медленная модуляция
        modulation_depth=0.6,
        carrier_type='sine',
        modulating_type='triangle'  # Линейные нарастания/спады
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_soft_tremolo.wav'), am_soft, normalize=True)

    t, am_aggressive = mod.amplitude_modulation(
        carrier_freq=150,
        carrier_duration=duration,
        modulating_freq=8,
        modulation_depth=0.7,
        carrier_type='sawtooth',  # Яркий, резкий тембр
        modulating_type='pulse'  # Резкие переключения
    )
    wav.save_mono_wav(os.path.join(out_dir, 'am_aggressive.wav'), am_aggressive, normalize=True)

    # =================
    # ЧАСТОТНАЯ МОДУЛЯЦИЯ (FM)
    # =================

    t, fm_vibrato = mod.frequency_modulation(
        carrier_freq=440,
        carrier_duration=duration,
        modulating_freq=5,  # 5 Гц - типичная скорость вибрато
        carrier_type='sine',
        modulating_type='sine'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_vibrato.wav'), fm_vibrato, normalize=True)

    t, fm_siren = mod.frequency_modulation(
        carrier_freq=800,
        carrier_duration=duration,
        modulating_freq=5,  # Медленная модуляция
        carrier_type='sine',
        modulating_type='triangle'  # Плавные нарастания/спады частоты
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_siren.wav'), fm_siren, normalize=True)

    t, fm_stepped = mod.frequency_modulation(
        carrier_freq=330,
        carrier_duration=duration,
        modulating_freq=2,  # 3 переключения в секунду
        carrier_type='sine',
        modulating_type='pulse'  # Резкие скачки частоты
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_stepped.wav'), fm_stepped, normalize=True)

    t, fm_experimental = mod.frequency_modulation(
        carrier_freq=180,
        carrier_duration=duration,
        modulating_freq=7,
        carrier_type='triangle',  # Мягкий тембр
        modulating_type='sawtooth'  # Асимметричная модуляция
    )
    wav.save_mono_wav(os.path.join(out_dir, 'fm_experimental.wav'), fm_experimental, normalize=True)

    # =================
    # СРАВНИТЕЛЬНЫЕ ПРИМЕРЫ
    # =================

    # AM версия
    t, compare_am = mod.amplitude_modulation(
        carrier_freq=350,
        carrier_duration=duration,
        modulating_freq=6,
        modulation_depth=0.7,
        carrier_type='sine',
        modulating_type='sine'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'compare_am.wav'), compare_am, normalize=True)

    # FM версия (те же частоты)
    t, compare_fm = mod.frequency_modulation(
        carrier_freq=350,
        carrier_duration=duration,
        modulating_freq=6,
        carrier_type='sine',
        modulating_type='sine'
    )
    wav.save_mono_wav(os.path.join(out_dir, 'compare_fm.wav'), compare_fm, normalize=True)

    # чистый синус для сравнения
    t, reference_sine = mod.generator.generate_sine(440, duration, amplitude=0.8)
    wav.save_mono_wav(os.path.join(out_dir, 'reference_sine_440Hz.wav'), reference_sine, normalize=False)

    # прямоугольник для сравнения
    t, reference_pulse = mod.generator.generate_pulse(220, duration, amplitude=0.8)
    wav.save_mono_wav(os.path.join(out_dir, 'reference_pulse_220Hz.wav'), reference_pulse, normalize=False)


def generate_moonlight_sonata_excerpt(out_dir, sr=44100):
    poly = PolyphonicSignals(sample_rate=sr)
    wav = WaveFileManager(sample_rate=sr, bit_depth=16)

    notes = [
        (note_to_frequency('C#3'), 0.1),
        (note_to_frequency('G#3'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#4'), 0.1), ],
        (note_to_frequency('E4'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('G#4'), 0.1), ],

        (note_to_frequency('C#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('E4'), 0.1), ],
        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('C#5'), 0.1), ],

        (note_to_frequency('E4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#4'), 0.1), ],
        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('E5'), 0.1), ],

        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#5'), 0.1), ],
        (note_to_frequency('E5'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('G#5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('E5'), 0.1), ],
        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('E5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('E6'), 0.1), ],

        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('E6'), 0.1),

        [(note_to_frequency('C#4'), 0.2), (note_to_frequency('C#3'), 0.2),
         (note_to_frequency('G#5'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('E6'), 0.2),
         (note_to_frequency('G#6'), 0.2), ],
        [(note_to_frequency('G#3'), 0.2),
         (note_to_frequency('G#5'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('E6'), 0.2),
         (note_to_frequency('G#6'), 0.2), ],

        # 2
        (note_to_frequency('C3'), 0.1),
        (note_to_frequency('G#3'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C4'), 0.1), ],
        (note_to_frequency('D#4'), 0.1),
        [(note_to_frequency('C3'), 0.1), (note_to_frequency('G#4'), 0.1), ],

        (note_to_frequency('C4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('D#4'), 0.1), ],
        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('C3'), 0.1), (note_to_frequency('C5'), 0.1), ],

        (note_to_frequency('D#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#4'), 0.1), ],
        (note_to_frequency('C5'), 0.1),
        [(note_to_frequency('C3'), 0.1), (note_to_frequency('D#5'), 0.1), ],

        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C5'), 0.1), ],
        (note_to_frequency('D#5'), 0.1),
        [(note_to_frequency('C3'), 0.1), (note_to_frequency('G#5'), 0.1), ],

        (note_to_frequency('C5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('D#5'), 0.1), ],
        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('C3'), 0.1), (note_to_frequency('C6'), 0.1), ],

        (note_to_frequency('D#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('C6'), 0.1),
        [(note_to_frequency('C3'), 0.1), (note_to_frequency('D#6'), 0.1), ],

        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('D#6'), 0.1),

        [(note_to_frequency('C4'), 0.2), (note_to_frequency('C3'), 0.2),
         (note_to_frequency('G#5'), 0.2),
         (note_to_frequency('C6'), 0.2),
         (note_to_frequency('D#6'), 0.2),
         (note_to_frequency('G#6'), 0.2), ],
        [(note_to_frequency('G#3'), 0.2),
         (note_to_frequency('G#5'), 0.2),
         (note_to_frequency('C6'), 0.2),
         (note_to_frequency('D#6'), 0.2),
         (note_to_frequency('G#6'), 0.2), ],

        # 3
        (note_to_frequency('B2'), 0.1),
        (note_to_frequency('C#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('F4'), 0.1), ],
        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('B2'), 0.1), (note_to_frequency('C#5'), 0.1), ],

        (note_to_frequency('F4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#4'), 0.1), ],
        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('B2'), 0.1), (note_to_frequency('F5'), 0.1), ],

        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#5'), 0.1), ],
        (note_to_frequency('F5'), 0.1),
        [(note_to_frequency('B2'), 0.1), (note_to_frequency('G#5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('F5'), 0.1), ],
        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('B2'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('F5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('B2'), 0.1), (note_to_frequency('F6'), 0.1), ],

        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('F6'), 0.1),
        [(note_to_frequency('B2'), 0.1), (note_to_frequency('G#6'), 0.1), ],

        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('F6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('B2'), 0.2), (note_to_frequency('B3'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('F6'), 0.2),
         (note_to_frequency('G#6'), 0.2),
         (note_to_frequency('C#7'), 0.2), ],
        [(note_to_frequency('G#3'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('F6'), 0.2),
         (note_to_frequency('G#6'), 0.2),
         (note_to_frequency('C#7'), 0.2), ],

        # 4
        (note_to_frequency('A2'), 0.1),
        (note_to_frequency('C#4'), 0.1),
        [(note_to_frequency('A3'), 0.1), (note_to_frequency('F#4'), 0.1), ],
        (note_to_frequency('A4'), 0.1),
        [(note_to_frequency('A2'), 0.1), (note_to_frequency('C#5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('A3'), 0.1), (note_to_frequency('F#5'), 0.1), ],
        (note_to_frequency('A5'), 0.1),
        [(note_to_frequency('A2'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('A3'), 0.1), (note_to_frequency('F#6'), 0.1), ],
        (note_to_frequency('A6'), 0.1),

        [(note_to_frequency('A2'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('F#6'), 0.2),
         (note_to_frequency('C#7'), 0.2),
         ],
        [(note_to_frequency('A3'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('F#6'), 0.2),
         (note_to_frequency('C#7'), 0.2),
         ],

        # 5
        (note_to_frequency('A2'), 0.1),
        (note_to_frequency('C#4'), 0.1),
        [(note_to_frequency('A3'), 0.1), (note_to_frequency('E4'), 0.1), ],
        (note_to_frequency('G4'), 0.1),
        [(note_to_frequency('A2'), 0.1), (note_to_frequency('C#5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('A3'), 0.1), (note_to_frequency('E5'), 0.1), ],
        (note_to_frequency('G5'), 0.1),
        [(note_to_frequency('A2'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('A3'), 0.1), (note_to_frequency('E6'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        [(note_to_frequency('A2'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('G6'), 0.2),
         (note_to_frequency('C#7'), 0.2),
         ],
        [(note_to_frequency('A3'), 0.2),
         (note_to_frequency('C#6'), 0.2),
         (note_to_frequency('G6'), 0.2),
         (note_to_frequency('C#7'), 0.2),
         ],

        [(note_to_frequency('G#2'), 0.4),
         (note_to_frequency('G#3'), 0.4),
         (note_to_frequency('C6'), 0.4),
         (note_to_frequency('G#6'), 0.4),
         (note_to_frequency('C7'), 0.4),
         ],

        # 6
        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('D#5'), 0.1),
         (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('E5'), 0.1),
         (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('F#5'), 0.1),
         (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('D#5'), 0.1),
         (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('F#5'), 0.1),
         (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('E5'), 0.1),
         (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('A#5'), 0.1),
         (note_to_frequency('F#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('G#5'), 0.1),
         (note_to_frequency('E6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('F#5'), 0.1),
         (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('E5'), 0.1),
         (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('D#5'), 0.1),
         (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A5'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        # повтор 6
        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [
            (note_to_frequency('C5'), 0.1),
            (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [
            (note_to_frequency('C5'), 0.1),
            (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('D#5'), 0.1),
         (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('E5'), 0.1),
         (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('F#5'), 0.1),
         (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('D#5'), 0.1),
         (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('F#5'), 0.1),
         (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('E5'), 0.1),
         (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('A#5'), 0.1),
         (note_to_frequency('F#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('G#5'), 0.1),
         (note_to_frequency('E6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('F#5'), 0.1),
         (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('E5'), 0.1),
         (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('D#5'), 0.1),
         (note_to_frequency('C6'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A5'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A5'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A5'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A5'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C5'), 0.1),
         (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('G#6'), 0.1),

        [(note_to_frequency('G#4'), 0.1),
         (note_to_frequency('C#5'), 0.1),
         (note_to_frequency('A5'), 0.1), ],
        (note_to_frequency('G6'), 0.1),

        [(note_to_frequency('G#4'), 0.4),
         (note_to_frequency('C5'), 0.4),
         (note_to_frequency('G#5'), 0.4), ],

        [(note_to_frequency('G#2'), 1),
         (note_to_frequency('C#3'), 1),
         (note_to_frequency('G#4'), 1), ],

        # 7
        (note_to_frequency('C#3'), 0.1),
        (note_to_frequency('G#3'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#4'), 0.1), ],
        (note_to_frequency('E4'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('G#4'), 0.1), ],

        (note_to_frequency('C#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('E4'), 0.1), ],
        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('C#5'), 0.1), ],

        (note_to_frequency('E4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#4'), 0.1), ],
        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('E5'), 0.1), ],

        (note_to_frequency('G#4'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('C#5'), 0.1), ],
        (note_to_frequency('E5'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('G#5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('E5'), 0.1), ],
        (note_to_frequency('G#5'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('E5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#5'), 0.1), ],
        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('C#3'), 0.1), (note_to_frequency('E6'), 0.1), ],

        (note_to_frequency('E6'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G#6'), 0.1), ],
        (note_to_frequency('C#7'), 0.1),

        [(note_to_frequency('C#4'), 0.2), (note_to_frequency('C#3'), 0.2),
         (note_to_frequency('E6'), 0.2),
         (note_to_frequency('E7'), 0.2), ],

        [(note_to_frequency('G#3'), 0.2),
         (note_to_frequency('E6'), 0.2),
         (note_to_frequency('E7'), 0.2), ],

        # 8
        (note_to_frequency('A#2'), 0.1),
        (note_to_frequency('E4'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('G4'), 0.1), ],
        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('E5'), 0.1), ],

        (note_to_frequency('G4'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('C#5'), 0.1), ],
        (note_to_frequency('E5'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('G5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('E5'), 0.1), ],
        (note_to_frequency('G5'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('E5'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('G5'), 0.1), ],
        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('E6'), 0.1), ],

        (note_to_frequency('G5'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('E6'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('G6'), 0.1), ],

        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('E6'), 0.1), ],
        (note_to_frequency('G6'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('C#7'), 0.1), ],

        (note_to_frequency('E6'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('G6'), 0.1), ],
        (note_to_frequency('C#7'), 0.1),

        [(note_to_frequency('A#3'), 0.2), (note_to_frequency('A#2'), 0.2),
         (note_to_frequency('E6'), 0.2),
         (note_to_frequency('E7'), 0.2), ],

        [(note_to_frequency('C#4'), 0.2),
         (note_to_frequency('E6'), 0.2),
         (note_to_frequency('E7'), 0.2), ],

        # 9
        (note_to_frequency('G2'), 0.1),
        (note_to_frequency('D#4'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('A#4'), 0.1), ],
        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('G3'), 0.1), (note_to_frequency('D#5'), 0.1), ],

        (note_to_frequency('A#4'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('C#5'), 0.1), ],
        (note_to_frequency('D#5'), 0.1),
        [(note_to_frequency('G3'), 0.1), (note_to_frequency('A#5'), 0.1), ],

        (note_to_frequency('C#5'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('D#5'), 0.1), ],
        (note_to_frequency('A#5'), 0.1),
        [(note_to_frequency('G3'), 0.1), (note_to_frequency('C#6'), 0.1), ],

        (note_to_frequency('D#5'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('A#5'), 0.1), ],
        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('G3'), 0.1), (note_to_frequency('D#6'), 0.1), ],

        (note_to_frequency('A#5'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('D#6'), 0.1),
        [(note_to_frequency('G3'), 0.1), (note_to_frequency('A#6'), 0.1), ],

        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('C#4'), 0.1), (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('A#6'), 0.1),
        [(note_to_frequency('A#3'), 0.1), (note_to_frequency('C#7'), 0.1), ],

        (note_to_frequency('A#6'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('D#6'), 0.1), ],
        (note_to_frequency('C#6'), 0.1),
        [(note_to_frequency('G3'), 0.1), (note_to_frequency('A#6'), 0.1), ],

        (note_to_frequency('D#6'), 0.1),
        [(note_to_frequency('D#4'), 0.1), (note_to_frequency('C#6'), 0.1), ],
        (note_to_frequency('A#5'), 0.1),
        [(note_to_frequency('G#3'), 0.1), (note_to_frequency('B5'), 0.1), ],
    ]

    melody = []
    for item in notes:
        if isinstance(item, list):  # Если это аккорд
            signals = []
            for freq, duration in item:
                t, note = poly.generator.generate_sine(freq, duration, amplitude=0.8)
                signals.append(note)
            chord_signal = np.sum(signals, axis=0)
            melody.append(chord_signal)
        else:
            freq, duration = item
            t, note = poly.generator.generate_sine(freq, duration, amplitude=0.8)
            melody.append(note)

    melody_signal = np.concatenate(melody)
    wav.save_mono_wav(os.path.join(out_dir, 'moonlight_sonata_with_chords.wav'), melody_signal)
