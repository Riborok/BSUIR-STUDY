import os
from demo.signals_demo import *


def ensure_out_dir(path):
    os.makedirs(path, exist_ok=True)

def fourier_demo():
    print("\n" + "=" * 60)
    print("АНАЛИЗ СИГНАЛОВ С ПОМОЩЬЮ ПРЕОБРАЗОВАНИЙ ФУРЬЕ")
    print("=" * 60)
    
    from demo.fourier_demo import main as fourier_main
    fourier_main()

def main():
    fourier_demo()

if __name__ == '__main__':
    main()
