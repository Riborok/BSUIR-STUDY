"""
Скрипт для быстрого запуска визуализации с оптимальным количеством репликаций
Все графики сохраняются в папку "графики"
"""

import sys
import os

# Добавляем текущую директорию в путь
sys.path.insert(0, os.path.dirname(__file__))

from run_multiple_simulations import main

if __name__ == "__main__":
    # Устанавливаем sys.argv для запуска с 30 репликациями
    sys.argv = ['run_with_visualization.py', '30']
    main()

