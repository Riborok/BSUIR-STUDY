"""UART communication for control device (mood and leg spread control)."""

import threading
import time
from typing import Callable, Optional

try:
    import serial
except ImportError:
    serial = None
    print("Warning: pyserial not installed. UART control device will not work.")


class UARTControlDevice:
    """Class for managing UART communication with control device."""

    def __init__(self, port: str = "COM4", baudrate: int = 115200):
        """Initialize UART control device communicator.

        Args:
            port: COM port name (e.g., 'COM4')
            baudrate: Baud rate (default: 115200)
        """
        self.port = port
        self.baudrate = baudrate
        self.ser: Optional[serial.Serial] = None
        self.running = False
        self.thread: Optional[threading.Thread] = None

        # Callbacks for control commands
        self.mood_increase_callback: Optional[Callable[[], None]] = None
        self.mood_decrease_callback: Optional[Callable[[], None]] = None
        self.leg_narrow_callback: Optional[Callable[[], None]] = None
        self.leg_widen_callback: Optional[Callable[[], None]] = None
        # New: callback for joystick direction. Receives a string like 'N','NE','E',... or 'CENTER'
        self.direction_callback: Optional[Callable[[str], None]] = None

    def set_callbacks(self,
                     mood_increase: Callable[[], None],
                     mood_decrease: Callable[[], None],
                     leg_narrow: Callable[[], None],
                     leg_widen: Callable[[], None]) -> None:
        """Set callbacks for control commands.

        Args:
            mood_increase: Callback for increasing mood
            mood_decrease: Callback for decreasing mood
            leg_narrow: Callback for narrowing legs
            leg_widen: Callback for widening legs
        """
        self.mood_increase_callback = mood_increase
        self.mood_decrease_callback = mood_decrease
        self.leg_narrow_callback = leg_narrow
        self.leg_widen_callback = leg_widen

    def set_direction_callback(self, cb: Callable[[str], None]) -> None:
        """Set callback to receive joystick direction strings.

        Callback receives one argument: direction string among
        {'N','NE','E','SE','S','SW','W','NW','CENTER'}.
        """
        self.direction_callback = cb

    def connect(self) -> bool:
        """Connect to the control device.

        Returns:
            True if connection successful, False otherwise
        """
        if serial is None:
            print("pyserial not installed. Cannot connect to control device.")
            return False

        try:
            print(f"Попытка подключения к устройству управления на {self.port}...")
            self.ser = serial.Serial(
                port=self.port,
                baudrate=self.baudrate,
                bytesize=serial.EIGHTBITS,
                parity=serial.PARITY_NONE,
                stopbits=serial.STOPBITS_ONE,
                timeout=0.1
            )
            print(f"✓ Устройство управления подключено к {self.port}")
            return True
        except Exception as e:
            print(f"✗ Ошибка подключения к устройству управления {self.port}: {e}")
            return False

    def disconnect(self) -> None:
        """Disconnect from control device."""
        self.stop()
        if self.ser and self.ser.is_open:
            self.ser.close()
            print(f"Устройство управления {self.port} отключено")

    def start(self) -> None:
        """Start reading from control device in background thread."""
        if self.ser is None or not self.ser.is_open:
            print("Устройство управления не подключено")
            return

        self.running = True
        self.thread = threading.Thread(target=self._read_loop, daemon=True)
        self.thread.start()
        print("Запущен поток чтения устройства управления")

    def stop(self) -> None:
        """Stop reading from control device."""
        self.running = False
        if self.thread:
            self.thread.join(timeout=1.0)

    def _read_loop(self) -> None:
        """Background thread loop for reading control commands."""
        buffer = b""

        # We'll accumulate bytes until we have a newline (\n) or carriage return terminated message
        while self.running:
            try:
                if self.ser and self.ser.in_waiting > 0:
                    data = self.ser.read(self.ser.in_waiting)

                    # Output what was read (for debugging)
                    print(f"[UART] Получено: {data} (hex: {data.hex()}, ASCII: {repr(data)})")

                    buffer += data

                    # Process complete lines terminated by '\n' or '\r'
                    while b'\n' in buffer or b'\r' in buffer:
                        # find earliest terminator
                        idx_n = buffer.find(b'\n') if b'\n' in buffer else None
                        idx_r = buffer.find(b'\r') if b'\r' in buffer else None

                        # choose smallest positive index
                        indices = [i for i in (idx_n, idx_r) if i is not None and i >= 0]
                        if not indices:
                            break
                        term_idx = min(indices)

                        line = buffer[:term_idx]
                        # remove processed line and any following CR/LF characters
                        # drop any combination of CR and LF at start
                        rest = buffer[term_idx:]
                        # consume leading CR/LF chars
                        cut = 0
                        for ch in rest:
                            if ch in (0x0A, 0x0D):
                                cut += 1
                            else:
                                break
                        buffer = buffer[term_idx+cut:]

                        if line:
                            # Process full textual message
                            try:
                                text = line.decode('ascii', errors='ignore')
                                self._process_text_message(text.strip())
                            except Exception as e:
                                print(f"Ошибка при обработке сообщения: {e}")

                time.sleep(0.01)  # Small delay to avoid busy waiting

            except Exception as e:
                print(f"Ошибка чтения устройства управления: {e}")
                time.sleep(0.1)

    def _process_text_message(self, text: str) -> None:
        """Process textual message from device.

        Expected joystick message from MCU: 'JX<val>Y<val>' where <val> are integers.
        The MCU (STM) sends messages like: "JX123Y456\r\n".
        We also keep support for single-character numeric commands '1'..'4'.
        """
        if not text:
            return

        print(f"[UART TXT] '{text}'")

        # If message is a single char '1'..'4', keep old behavior
        if len(text) == 1 and text in ('1', '2', '3', '4'):
            if text == '1' and callable(self.mood_increase_callback):
                self.mood_increase_callback()
            elif text == '2' and callable(self.mood_decrease_callback):
                self.mood_decrease_callback()
            elif text == '3' and callable(self.leg_narrow_callback):
                self.leg_narrow_callback()
            elif text == '4' and callable(self.leg_widen_callback):
                self.leg_widen_callback()
            return

        # Parse joystick message
        # Look for pattern JX<number>Y<number>
        text = text.upper()
        if text.startswith('JX') and 'Y' in text:
            try:
                # remove leading 'J'
                body = text[1:]
                # body now like 'X123Y456' or 'X0Y0'
                x_part, y_part = body.split('Y', 1)
                # x_part like 'X123'
                if x_part.startswith('X'):
                    x_str = x_part[1:]
                    y_str = y_part
                    x_val = int(x_str) if x_str.isdigit() else 0
                    y_val = int(y_str) if y_str.lstrip('-').isdigit() else 0

                    # Convert ADC values (0..4095 typical) to direction
                    direction = self._xy_to_direction(x_val, y_val)
                    print(f"[UART] Parsed joystick X={x_val} Y={y_val} -> {direction}")
                    # Вызвать старый direction_callback (если нужен) и карту действий
                    if callable(self.direction_callback):
                        try:
                            self.direction_callback(direction)
                        except Exception as e:
                            print(f"Ошибка в direction_callback: {e}")

                    # Новый: диспетчеризовать направление на существующие действия
                    try:
                        self._dispatch_direction_actions(direction)
                    except Exception as e:
                        print(f"Ошибка при вызове action callbacks: {e}")
                    return
            except Exception as e:
                print(f"Ошибка парсинга joystick сообщения: {e}")

        # Unknown textual message
        print(f"Неизвестное сообщение: '{text}'")

    def _xy_to_direction(self, x: int, y: int) -> str:
        """Convert joystick ADC x,y to direction string.

        Assumptions:
        - ADC values range 0..4095 (STM32 12-bit). If values are in other range, behavior still works.
        - Center is approximately mid-scale (2048). We use deadzone to avoid jitter.

        Returns one of: 'N','NE','E','SE','S','SW','W','NW','CENTER'
        """
        # Normalize to -1..1
        try:
            max_adc = 4095.0
            cx = (x - max_adc/2.0) / (max_adc/2.0)
            cy = (y - max_adc/2.0) / (max_adc/2.0)
        except Exception:
            cx = 0.0
            cy = 0.0

        # Deadzone threshold
        dz = 0.2
        if abs(cx) < dz and abs(cy) < dz:
            return 'CENTER'

        # Compute angle in degrees, 0 = right (E), 90 = up (N)
        import math
        angle = math.degrees(math.atan2(cy, cx))
        # atan2 returns -180..180; convert to 0..360
        if angle < 0:
            angle += 360.0

        # Map angle to 8 sectors (each 45 degrees), center at N=90
        # We'll use boundaries centered on compass points: E=0, NE=45, N=90, ...
        # Determine nearest direction
        directions = [
            ('E', 0), ('NE', 45), ('N', 90), ('NW', 135),
            ('W', 180), ('SW', 225), ('S', 270), ('SE', 315)
        ]

        # find minimal angular difference
        best = None
        best_diff = 360.0
        for name, ang in directions:
            diff = abs((angle - ang + 180) % 360 - 180)
            if diff < best_diff:
                best_diff = diff
                best = name

        return best or 'CENTER'

    def _dispatch_direction_actions(self, direction: str) -> None:
        """Map direction to callbacks as requested by user:

        Up (N) -> leg_widen_callback
        Down (S) -> leg_narrow_callback
        Left (W) -> mood_decrease_callback
        Right (E) -> mood_increase_callback

        For diagonal directions (NE,NW,SE,SW) both relevant callbacks are invoked
        (e.g., NE -> N and E => leg_widen + mood_increase).
        CENTER does nothing.
        """
        if not direction:
            return

        # Define helper to call safely
        def safe_call(cb: Optional[Callable[[], None]]):
            if callable(cb):
                try:
                    cb()
                except Exception as e:
                    print(f"Ошибка в callback: {e}")

        dir_upper = direction.upper()

        # Primary directions
        if dir_upper == 'N':
            safe_call(self.leg_widen_callback)
            return
        if dir_upper == 'S':
            safe_call(self.leg_narrow_callback)
            return
        if dir_upper == 'W':
            safe_call(self.mood_decrease_callback)
            return
        if dir_upper == 'E':
            safe_call(self.mood_increase_callback)
            return

        # Diagonals: call both components
        if dir_upper == 'NE':
            safe_call(self.leg_widen_callback)
            safe_call(self.mood_increase_callback)
            return
        if dir_upper == 'NW':
            safe_call(self.leg_widen_callback)
            safe_call(self.mood_decrease_callback)
            return
        if dir_upper == 'SE':
            safe_call(self.leg_narrow_callback)
            safe_call(self.mood_increase_callback)
            return
        if dir_upper == 'SW':
            safe_call(self.leg_narrow_callback)
            safe_call(self.mood_decrease_callback)
            return

        # CENTER or unknown -> do nothing
        return

