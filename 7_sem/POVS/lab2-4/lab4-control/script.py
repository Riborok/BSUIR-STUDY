import serial
import time
from datetime import datetime

def main():
    port = input("COM порт (например COM3): ")
    
    try:
        # Открываем COM-порт
        ser = serial.Serial(port, 115200, timeout=1)
        print(f"Подключено к {port}")
        print("Ожидание данных... (для выхода нажмите Ctrl+C)")
        
   
        # Бесконечный цикл чтения данных
        while True:
            if ser.in_waiting > 0:  # Если есть данные для чтения
                try:
                    # Читаем строку из порта
                    data = ser.readline().decode('utf-8').strip()
                    if data:  # Если строка не пустая
                        timestamp = datetime.now().strftime("%H:%M:%S")
                        print(f"[{timestamp}] {data}")
                except UnicodeDecodeError:
                    # Если не удалось декодировать UTF-8, читаем как байты
                    data_bytes = ser.readline()
                    timestamp = datetime.now().strftime("%H:%M:%S")
                    print(f"[{timestamp}] RAW: {data_bytes}")
            
            # Небольшая пауза чтобы не нагружать процессор
            time.sleep(0.01)
            
    except serial.SerialException as e:
        print(f"Ошибка COM-порта: {e}")
    except KeyboardInterrupt:
        print("\nЗавершение работы...")
    except Exception as e:
        print(f"Неожиданная ошибка: {e}")
    finally:
        # Всегда закрываем порт при выходе
        if 'ser' in locals() and ser.is_open:
            ser.close()
            print("COM-порт закрыт")

if __name__ == "__main__":
    main()