"""UART communication module for STM32 interface."""

import threading
import time
from typing import Callable, Optional

try:
    import serial
    import serial.tools.list_ports
    SERIAL_AVAILABLE = True
except ImportError:
    SERIAL_AVAILABLE = False
    print("Warning: pyserial not installed. Install with: pip install pyserial")


class UARTCommunicator:
    """Handles UART communication with STM32."""
    
    @staticmethod
    def list_available_ports():
        """List all available serial ports."""
        if not SERIAL_AVAILABLE:
            print("pyserial is not installed")
            return []

        ports = serial.tools.list_ports.comports()
        available = []
        print("\nДоступные COM-порты:")
        for port in ports:
            print(f"  {port.device} - {port.description}")
            available.append(port.device)
        if not available:
            print("  Нет доступных COM-портов")
        return available

    def __init__(self, port: str = "COM3", baudrate: int = 115200):
        """Initialize UART communicator.
        
        Args:
            port: Serial port name (e.g., "COM3" on Windows, "/dev/ttyUSB0" on Linux)
            baudrate: Baud rate (default 115200)
        """
        self.port = port
        self.baudrate = baudrate
        self.serial: Optional[serial.Serial] = None
        self.running = False
        self.read_thread: Optional[threading.Thread] = None
        self.position_callback: Optional[Callable[[int], None]] = None
        self.last_sent_speed = 0
        
    def set_position_callback(self, callback: Callable[[int], None]) -> None:
        """Set callback for receiving position updates from STM32."""
        self.position_callback = callback
    
    def connect(self) -> bool:
        """Connect to STM32 via UART."""
        if not SERIAL_AVAILABLE:
            print("Error: pyserial is not available")
            return False
        
        try:
            print(f"\nПопытка подключения к {self.port} на скорости {self.baudrate} бод...")
            self.serial = serial.Serial(
                port=self.port,
                baudrate=self.baudrate,
                bytesize=serial.EIGHTBITS,
                parity=serial.PARITY_NONE,
                stopbits=serial.STOPBITS_ONE,
                timeout=0.1,  # Shorter timeout for more responsive reading
                write_timeout=1.0,
                xonxoff=False,  # Disable software flow control
                rtscts=False,   # Disable hardware (RTS/CTS) flow control
                dsrdtr=False    # Disable hardware (DSR/DTR) flow control
            )
            # Prevent DTR from resetting the board
            self.serial.dtr = False
            self.serial.rts = False

            # Flush any existing data in buffers
            time.sleep(0.1)
            self.serial.reset_input_buffer()
            self.serial.reset_output_buffer()

            print(f"✓ Успешно подключено к {self.port} на скорости {self.baudrate} бод")
            return True
        except serial.SerialException as e:
            print(f"✗ Ошибка подключения к {self.port}: {e}")
            print("\nВозможные причины:")
            print("  1. Плата STM32 не подключена")
            print("  2. Неправильный COM-порт")
            print("  3. Порт занят другой программой")
            print("  4. Драйверы USB-UART не установлены")
            self.list_available_ports()
            return False
    
    def disconnect(self) -> None:
        """Disconnect from STM32."""
        self.running = False
        if self.read_thread and self.read_thread.is_alive():
            self.read_thread.join(timeout=1.0)
        if self.serial and self.serial.is_open:
            self.serial.close()
            print("Disconnected from STM32")
    
    def send_speed(self, speed: int) -> bool:
        """Send speed value (1-5) to STM32.
        
        Args:
            speed: Speed value from 1 to 5
            
        Returns:
            True if sent successfully, False otherwise
        """
        if not self.serial or not self.serial.is_open:
            return False
        
        if speed < 1 or speed > 5:
            print(f"Warning: Speed {speed} is out of range (1-5)")
            return False
        
        if speed == self.last_sent_speed:
            return True  # Already sent this speed
        
        try:
            # Send as ASCII character '1'-'5'
            speed_byte = bytes([ord(str(speed))])
            self.serial.write(speed_byte)
            self.last_sent_speed = speed
            print(f"Sent speed {speed} to STM32")
            return True
        except Exception as e:
            print(f"Error sending speed: {e}")
            return False
    
    def _read_loop(self) -> None:
        """Background thread for reading position from STM32."""
        while self.running:
            if not self.serial or not self.serial.is_open:
                time.sleep(0.1)
                continue
            
            try:
                # Flush old data to avoid processing outdated positions
                if self.serial.in_waiting > 10:
                    # Too much data accumulated, read and discard old data
                    self.serial.read(self.serial.in_waiting - 1)

                if self.serial.in_waiting > 0:
                    # Read one byte (position 1-12)
                    data = self.serial.read(1)
                    if data:
                        position = int(data[0])
                        # Validate position range
                        if 1 <= position <= 12:
                            if self.position_callback:
                                # Convert 1-based to 0-based index
                                stage_index = position - 1
                                # Call in main thread context
                                self.position_callback(stage_index)
                        else:
                            # May be ASCII digit, try to parse
                            try:
                                if 49 <= data[0] <= 57:  # '1' to '9'
                                    position = data[0] - 48  # Convert ASCII to number
                                    if 1 <= position <= 12:
                                        if self.position_callback:
                                            stage_index = position - 1
                                            self.position_callback(stage_index)
                                else:
                                    print(f"Warning: Received invalid position {position} (0x{data[0]:02X})")
                            except:
                                print(f"Warning: Received invalid data {data[0]} (0x{data[0]:02X})")
                else:
                    time.sleep(0.01)  # Small delay when no data
            except serial.SerialException as e:
                print(f"Serial error: {e}")
                time.sleep(0.1)
            except Exception as e:
                print(f"Error reading from serial: {e}")
                time.sleep(0.1)
    
    def start(self) -> bool:
        """Start reading thread."""
        if not self.serial or not self.serial.is_open:
            print("Error: Not connected to STM32")
            return False
        
        if self.running:
            return True  # Already running
        
        self.running = True
        self.read_thread = threading.Thread(target=self._read_loop, daemon=True)
        self.read_thread.start()
        print("Started UART reading thread")
        return True
    
    def stop(self) -> None:
        """Stop reading thread."""
        self.running = False
        if self.read_thread and self.read_thread.is_alive():
            self.read_thread.join(timeout=1.0)
        print("Stopped UART reading thread")

