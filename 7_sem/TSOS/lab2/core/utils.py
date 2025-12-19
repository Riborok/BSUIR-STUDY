def note_to_frequency(note, multiplier=1.0):
    base_frequencies = {
        'C': 261.63,
        'C#': 277.18,
        'D': 293.66,
        'D#': 311.13,
        'E': 329.63,
        'F': 349.23,
        'F#': 369.99,
        'G': 392.00,
        'G#': 415.30,
        'A': 440.00,
        'A#': 466.16,
        'B': 493.88
    }
    for item in base_frequencies.keys():
        base_frequencies[item] = base_frequencies[item] * multiplier

    import re
    match = re.match(r'([A-Ga-g#b]+)(\d+)', note)
    if not match:
        raise ValueError(f"Неверный формат ноты: {note}")

    base_note = match.group(1).upper()
    octave = int(match.group(2))

    if base_note not in base_frequencies:
        raise ValueError(f"Неизвестная нота: {base_note}")

    base_frequency = base_frequencies[base_note]
    frequency = base_frequency * (2 ** (octave - 4))

    return frequency
