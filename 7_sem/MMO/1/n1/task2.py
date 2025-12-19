def analyze_text(text):
    vowels = "аеиоуыэюяёaeiouy"
    consonants = "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxyz"

    text_lower = text.lower()
    vowel_count = 0
    consonant_count = 0
    vowel_letters = []

    for char in text_lower:
        if char in vowels:
            vowel_count += 1
            if char not in vowel_letters:
                vowel_letters.append(char)
        elif char in consonants:
            consonant_count += 1

    print(f"Гласных: {vowel_count}")
    print(f"Согласных: {consonant_count}")

    if vowel_count == consonant_count:
        print(f"Все гласные буквы: {vowel_letters}")

text = input("Введите текст: ")
analyze_text(text)