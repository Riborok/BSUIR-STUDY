"""Test script for UART control device (second device)."""

import sys
import time

try:
    import serial
except ImportError:
    print("ERROR: pyserial not installed. Install with: pip install pyserial")
    sys.exit(1)


def test_control_device(port="COM4", baudrate=115200):
    """Test UART control device connection and commands.

    Args:
        port: COM port name
        baudrate: Baud rate
    """
    print(f"\n{'='*60}")
    print(f"ТЕСТ UART УСТРОЙСТВА УПРАВЛЕНИЯ")
    print(f"{'='*60}\n")

    print(f"Порт: {port}")
    print(f"Скорость: {baudrate} бод")
    print(f"Протокол команд:")
    print(f"  1 = Увеличить настроение")
    print(f"  2 = Уменьшить настроение")
    print(f"  3 = Сузить ноги")
    print(f"  4 = Расширить ноги\n")

    # Try to connect
    try:
        print(f"Подключение к {port}...", end=" ")
        ser = serial.Serial(
            port=port,
            baudrate=baudrate,
            bytesize=serial.EIGHTBITS,
            parity=serial.PARITY_NONE,
            stopbits=serial.STOPBITS_ONE,
            timeout=1
        )
        print("✓ УСПЕШНО")
    except Exception as e:
        print(f"✗ ОШИБКА: {e}")
        print("\nВозможные причины:")
        print("  1. Устройство не подключено")
        print("  2. Неправильный COM-порт")
        print("  3. Порт занят другой программой")
        print("  4. Драйверы USB-UART не установлены")
        return False

    print("\nОжидание команд от устройства...")
    print("Нажмите Ctrl+C для выхода\n")

    command_names = {
        ord('1'): "Увеличить настроение",
        ord('2'): "Уменьшить настроение",
        ord('3'): "Сузить ноги",
        ord('4'): "Расширить ноги"
    }

    try:
        count = 0
        while True:
            if ser.in_waiting > 0:
                data = ser.read(ser.in_waiting)

                for byte in data:
                    count += 1
                    cmd_name = command_names.get(byte, f"Неизвестная команда (0x{byte:02X})")

                    timestamp = time.strftime("%H:%M:%S")
                    print(f"[{timestamp}] #{count:3d} | Команда: '{chr(byte)}' (0x{byte:02X}) → {cmd_name}")

            time.sleep(0.01)

    except KeyboardInterrupt:
        print("\n\nТест прерван пользователем")

    finally:
        ser.close()
        print(f"\nПолучено команд: {count}")
        print(f"Соединение с {port} закрыто")

    return True


def list_available_ports():
    """List all available COM ports."""
    try:
        from serial.tools import list_ports
        ports = list(list_ports.comports())

        if ports:
            print("\nДоступные COM-порты:")
            for port in ports:
                print(f"  {port.device} - {port.description}")
        else:
            print("\nCOM-порты не найдены")
    except Exception as e:
        print(f"Ошибка при получении списка портов: {e}")


def interactive_test():
    """Interactive test mode - send commands manually."""
    port = input("Введите COM-порт (например, COM4): ").strip()
    if not port:
        port = "COM4"

    try:
        ser = serial.Serial(port, 115200, timeout=1)
        print(f"\n✓ Подключено к {port}")
        print("\nРежим интерактивного теста")
        print("Введите команды для отправки:")
        print("  1 - Увеличить настроение")
        print("  2 - Уменьшить настроение")
        print("  3 - Сузить ноги")
        print("  4 - Расширить ноги")
        print("  q - Выход\n")

        while True:
            cmd = input("Команда: ").strip()

            if cmd.lower() == 'q':
                break

            if cmd in ['1', '2', '3', '4']:
                ser.write(cmd.encode())
                print(f"✓ Отправлено: {cmd}")
            else:
                print("✗ Неверная команда")

        ser.close()
        print("Соединение закрыто")

    except Exception as e:
        print(f"✗ Ошибка: {e}")


if __name__ == "__main__":
    print("\n" + "="*60)
    print("ТЕСТИРОВАНИЕ UART УСТРОЙСТВА УПРАВЛЕНИЯ")
    print("="*60)

    list_available_ports()

    print("\nВыберите режим:")
    print("  1 - Прослушивание команд от устройства (по умолчанию)")
    print("  2 - Интерактивная отправка команд")

    choice = input("\nВыбор: ").strip()

    if choice == '2':
        interactive_test()
    else:
        # Get port from command line or use default
        port = sys.argv[1] if len(sys.argv) > 1 else "COM4"
        test_control_device(port)

