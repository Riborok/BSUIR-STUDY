"""Constants for the Life Cycle Application."""

NUM_STAGES = 12
WINDOW_WIDTH = 1000
WINDOW_HEIGHT = 800
CENTER_X = WINDOW_WIDTH // 2
CENTER_Y = WINDOW_HEIGHT // 2
CIRCLE_RADIUS = 300

# Key mappings for stages (0-indexed)
STAGE_KEY_MAP = {
    "1": 0,
    "2": 1,
    "3": 2,
    "4": 3,
    "5": 4,
    "6": 5,
    "7": 6,
    "8": 7,
    "9": 8,
    "0": 9,
    "-": 10,
    "=": 11,
}

# Mood mappings
MOOD_MAP = {
    "z": "neutral",
    "x": "happy",
    "c": "sad",
    "v": "angry",
}

# Leg spread mappings
LEG_SPREAD_MAP = {
    "q": "narrow",   # Narrow stance
    "w": "normal",   # Normal stance
    "e": "wide",     # Wide stance
}

# Leg spread limits
LEG_SPREAD_MIN = 4    # Minimum leg spread (pixels)
LEG_SPREAD_MAX = 50   # Maximum leg spread (pixels)
LEG_SPREAD_DEFAULT = 14  # Default leg spread (pixels)
LEG_SPREAD_STEP = 2   # Step size for leg spread adjustment

# Mood limits
MOOD_MIN = -100       # Minimum mood value
MOOD_MAX = 100        # Maximum mood value
MOOD_DEFAULT = 0      # Default mood value (neutral)
MOOD_STEP = 15         # Step size for mood adjustment

# Stage labels for display
STAGE_LABELS = {
    0: "1) Сон с будильником",
    1: "2) Проснулся, выключил будильник",
    2: "3) Завтрак",
    3: "4) Пешком в универ",
    4: "5) На лекции",
    5: "6) Лабораторные/Практические занятия",
    6: "7) Перерыв, прогулка",
    7: "8) Обед",
    8: "9) Поход домой",
    9: "10) Лабы/самообучение",
    10: "11) Вечерний отдых",
    11: "12) Ночной сон",
}

# Stage index display (for labels near positions)
def get_stage_display_index(idx: int) -> str:
    """Get display index for stage (1-based to display character)."""
    idx_1based = idx + 1
    if idx_1based <= 9:
        return str(idx_1based)
    elif idx_1based == 10:
        return "0"
    elif idx_1based == 11:
        return "-"
    else:  # 12
        return "="


