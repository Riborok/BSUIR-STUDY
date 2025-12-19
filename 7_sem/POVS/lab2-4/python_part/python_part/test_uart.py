"""Test UART connection to STM32."""

import time
from uart_communication import UARTCommunicator

def test_uart_connection(port="COM3"):
    """Test UART connection and data reception."""
    print("=" * 60)
    print("ТЕСТ UART ПОДКЛЮЧЕНИЯ К STM32")
    print("=" * 60)

    # List available ports
    UARTCommunicator.list_available_ports()

    # Create communicator
    uart = UARTCommunicator(port=port, baudrate=115200)

    # Set callback to print received data
    received_positions = []
    def on_position(stage_index):
        position = stage_index + 1
        received_positions.append(position)
        print(f"  ← Получена позиция: {position} (индекс {stage_index})")

    uart.set_position_callback(on_position)

    # Try to connect
    print(f"\n{'─' * 60}")
    if not uart.connect():
        print("\n✗ Не удалось подключиться к UART")
        return False

    # Start reading
    print(f"{'─' * 60}")
    if not uart.start():
        print("\n✗ Не удалось запустить поток чтения")
        uart.disconnect()
        return False

    # Test for 10 seconds
    print(f"\n{'─' * 60}")
    print("Прослушивание данных в течение 10 секунд...")
    print("(STM32 должна отправлять текущую позицию)")
    print(f"{'─' * 60}\n")

    try:
        for i in range(10):
            time.sleep(1)
            if len(received_positions) > 0:
                last_pos = received_positions[-1]
                print(f"  [{i+1}/10] Последняя позиция: {last_pos} | Всего получено: {len(received_positions)}")
            else:
                print(f"  [{i+1}/10] Данных пока нет...")

        # Try sending speed
        print(f"\n{'─' * 60}")
        print("Попытка отправить скорость '3' на STM32...")
        if uart.send_speed(3):
            print("✓ Скорость отправлена")
        else:
            print("✗ Ошибка отправки скорости")

        time.sleep(2)

    except KeyboardInterrupt:
        print("\n\nПрервано пользователем")

    # Statistics
    print(f"\n{'─' * 60}")
    print("СТАТИСТИКА:")
    print(f"  Всего получено позиций: {len(received_positions)}")
    if len(received_positions) > 0:
        print(f"  Первая позиция: {received_positions[0]}")
        print(f"  Последняя позиция: {received_positions[-1]}")
        print(f"  Уникальные позиции: {sorted(set(received_positions))}")
    else:
        print("  ✗ НЕ ПОЛУЧЕНО НИ ОДНОЙ ПОЗИЦИИ!")
        print("\n  Возможные причины:")
        print("    1. STM32 не отправляет данные")
        print("    2. Неправильная конфигурация UART на STM32")
        print("    3. Проблема с проводами TX/RX (проверьте перекрёстное подключение)")
        print("    4. Разная скорость передачи (проверьте 115200 на обоих устройствах)")
    print(f"{'─' * 60}\n")

    # Cleanup
    uart.stop()
    uart.disconnect()

    return len(received_positions) > 0


if __name__ == "__main__":
    import sys

    port = "COM3"
    if len(sys.argv) > 1:
        port = sys.argv[1]

    print(f"\nИспользуется порт: {port}")
    print("Для использования другого порта: python test_uart.py COMX\n")

    success = test_uart_connection(port)

    if success:
        print("\n✓ ТЕСТ ПРОЙДЕН: Данные получены успешно!")
        exit(0)
    else:
        print("\n✗ ТЕСТ НЕ ПРОЙДЕН: Проблема с подключением или получением данных")
        exit(1)

