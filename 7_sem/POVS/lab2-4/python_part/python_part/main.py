"""Main application for Life Cycle visualization."""

import math
import threading
import time
import sys

try:
    import tkinter as tk
    from tkinter import ttk
    from tkinter import messagebox
except Exception:
    # For some Python installs, Tkinter can be capitalized
    import Tkinter as tk  # type: ignore
    import ttk  # type: ignore
    messagebox = None

try:
    import winsound  # Only available on Windows
except Exception:
    winsound = None  # Fallback: no sound

from constants import (
    NUM_STAGES, WINDOW_WIDTH, WINDOW_HEIGHT, CENTER_X, CENTER_Y,
    CIRCLE_RADIUS, get_stage_display_index,
    LEG_SPREAD_MIN, LEG_SPREAD_MAX, LEG_SPREAD_DEFAULT, LEG_SPREAD_STEP,
    MOOD_MIN, MOOD_MAX, MOOD_DEFAULT, MOOD_STEP
)
from stage_drawers import StageDrawer
from uart_communication import UARTCommunicator
from uart_control_device import UARTControlDevice
from typing import Optional


class LifeCycleApp:
    """Main application class for life cycle visualization."""
    
    def __init__(self, root: tk.Tk) -> None:
        self.root = root
        self.root.title("Жизненный цикл человечка — 12 этапов")
        
        # Make window resizable
        self.root.minsize(WINDOW_WIDTH, WINDOW_HEIGHT)
        
        # Base dimensions for scaling
        self.base_width = WINDOW_WIDTH
        self.base_height = WINDOW_HEIGHT
        self.scale_x = 1.0
        self.scale_y = 1.0
        
        # Current canvas dimensions
        self.canvas_width = WINDOW_WIDTH
        self.canvas_height = WINDOW_HEIGHT
        self.canvas_center_x = CENTER_X
        self.canvas_center_y = CENTER_Y
        self.canvas_radius = CIRCLE_RADIUS
        
        # Speed control
        self.speed = tk.IntVar(value=1)
        
        # UART communication
        self.uart_communicator: Optional[UARTCommunicator] = None
        self.uart_port = tk.StringVar(value="COM3")  # Default port

        self.uart_control_device: Optional[UARTControlDevice] = None
        self.control_port = tk.StringVar(value="COM4")  # Default port for control device

        # Create control panel
        self._create_control_panel()
        
        # Initialize UART connection
        self._init_uart()
        
        # Initialize control device
        self._init_control_device()

        # Create canvas
        self.canvas = tk.Canvas(
            self.root, width=self.canvas_width, height=self.canvas_height, bg="#FFFFFF"
        )
        self.canvas.pack(fill=tk.BOTH, expand=True)
        
        # Bind resize event
        self.canvas.bind("<Configure>", self._on_canvas_resize)
        
        # Ensure key focus
        try:
            self.root.focus_force()
        except Exception:
            pass
        self.canvas.focus_set()

        # Initialize stage drawer
        self.stage_drawer = StageDrawer(self.canvas)

        # State
        self.stage_positions = self._compute_stage_positions()
        self.drawn_stages: set[int] = set()
        self.current_stage: int | None = None
        self.stage_tags = [f"stage_{i}" for i in range(NUM_STAGES)]
        self.stage_mood_values: list[int] = [MOOD_DEFAULT for _ in range(NUM_STAGES)]  # Mood values from MOOD_MIN to MOOD_MAX
        self.stage_leg_spreads: list[int] = [LEG_SPREAD_DEFAULT for _ in range(NUM_STAGES)]  # Default leg spread
        self.arrows: list[tuple[int, int]] = []
        self.arrow_tags: list[str] = []

        # History of last stages for display (only current)
        self.stage_history: list[int] = []
        self.max_history_length = 1

        # Alarm thread (kept for potential future use, but disabled on stage 0)
        self._alarm_thread: threading.Thread | None = None
        self._alarm_stop = threading.Event()

        # Guides/UI - draw in correct order
        self._draw_guides()
        self._draw_labels()  # Labels outside circle
        self._bind_keys()
        # Click-to-focus for reliability
        self.canvas.bind("<Button-1>", lambda e: self.canvas.focus_set())

    def _create_control_panel(self) -> None:
        """Create control panel with speed selector and UART port."""
        control_frame = tk.Frame(self.root)
        control_frame.pack(fill=tk.X, padx=10, pady=5)
        
        # Speed label
        speed_label = tk.Label(control_frame, text="Скорость:", font=("Segoe UI", 10))
        speed_label.pack(side=tk.LEFT, padx=5)
        
        # Speed combobox
        speed_combo = ttk.Combobox(
            control_frame, 
            textvariable=self.speed,
            values=[1, 2, 3, 4, 5],
            state="readonly",
            width=5,
            font=("Segoe UI", 10)
        )
        speed_combo.pack(side=tk.LEFT, padx=5)
        speed_combo.bind("<<ComboboxSelected>>", self._on_speed_change)
        
        # UART port selector
        port_label = tk.Label(control_frame, text="COM порт:", font=("Segoe UI", 10))
        port_label.pack(side=tk.LEFT, padx=(20, 5))
        
        port_entry = tk.Entry(control_frame, textvariable=self.uart_port, width=8, font=("Segoe UI", 10))
        port_entry.pack(side=tk.LEFT, padx=5)
        
        connect_btn = tk.Button(control_frame, text="Подключить", command=self._reconnect_uart, font=("Segoe UI", 9))
        connect_btn.pack(side=tk.LEFT, padx=5)
        
        # Connection status
        self.uart_status_label = tk.Label(control_frame, text="[Не подключено]", font=("Segoe UI", 9), fg="red")
        self.uart_status_label.pack(side=tk.LEFT, padx=10)

        # Control device port selector
        control_port_label = tk.Label(control_frame, text="Устр. управл.:", font=("Segoe UI", 10))
        control_port_label.pack(side=tk.LEFT, padx=(20, 5))

        control_port_entry = tk.Entry(control_frame, textvariable=self.control_port, width=8, font=("Segoe UI", 10))
        control_port_entry.pack(side=tk.LEFT, padx=5)

        control_connect_btn = tk.Button(control_frame, text="Подключить", command=self._reconnect_control_device, font=("Segoe UI", 9))
        control_connect_btn.pack(side=tk.LEFT, padx=5)

        # Control device status
        self.control_status_label = tk.Label(control_frame, text="[Не подключено]", font=("Segoe UI", 9), fg="red")
        self.control_status_label.pack(side=tk.LEFT, padx=10)

        # Mood controls
        mood_label = tk.Label(control_frame, text="Настроение:", font=("Segoe UI", 10))
        mood_label.pack(side=tk.LEFT, padx=(20, 5))

        mood_down_btn = tk.Button(control_frame, text="- (Z)", command=lambda: self.on_mood_change(-MOOD_STEP), font=("Segoe UI", 9), width=5)
        mood_down_btn.pack(side=tk.LEFT, padx=2)

        mood_up_btn = tk.Button(control_frame, text="+ (X)", command=lambda: self.on_mood_change(MOOD_STEP), font=("Segoe UI", 9), width=5)
        mood_up_btn.pack(side=tk.LEFT, padx=2)

        # Leg spread controls
        leg_label = tk.Label(control_frame, text="Ширина ног:", font=("Segoe UI", 10))
        leg_label.pack(side=tk.LEFT, padx=(20, 5))

        leg_decrease_btn = tk.Button(control_frame, text="- (Q)", command=lambda: self.on_leg_spread_change(-LEG_SPREAD_STEP), font=("Segoe UI", 9), width=5)
        leg_decrease_btn.pack(side=tk.LEFT, padx=2)

        leg_increase_btn = tk.Button(control_frame, text="+ (E)", command=lambda: self.on_leg_spread_change(LEG_SPREAD_STEP), font=("Segoe UI", 9), width=5)
        leg_increase_btn.pack(side=tk.LEFT, padx=2)

    def _init_uart(self) -> None:
        """Initialize UART communication."""
        try:
            self.uart_communicator = UARTCommunicator(port=self.uart_port.get())
            self.uart_communicator.set_position_callback(self._on_stm32_position_received)
            
            if self.uart_communicator.connect():
                self.uart_communicator.start()
                # Send initial speed
                self.uart_communicator.send_speed(self.speed.get())
                self.uart_status_label.config(text="[Подключено]", fg="green")
            else:
                self.uart_status_label.config(text="[Ошибка подключения]", fg="red")
        except Exception as e:
            print(f"Error initializing UART: {e}")
            self.uart_status_label.config(text="[Ошибка]", fg="red")
    
    def _reconnect_uart(self) -> None:
        """Reconnect to UART with new port."""
        if self.uart_communicator:
            self.uart_communicator.disconnect()
        
        self._init_uart()
    
    def _init_control_device(self) -> None:
        """Initialize UART control device."""
        try:
            self.uart_control_device = UARTControlDevice(port=self.control_port.get())

            # Set callbacks for control commands
            self.uart_control_device.set_callbacks(
                mood_increase=lambda: self.root.after(0, self.on_mood_change, MOOD_STEP),
                mood_decrease=lambda: self.root.after(0, self.on_mood_change, -MOOD_STEP),
                leg_narrow=lambda: self.root.after(0, self.on_leg_spread_change, -LEG_SPREAD_STEP),
                leg_widen=lambda: self.root.after(0, self.on_leg_spread_change, LEG_SPREAD_STEP)
            )

            if self.uart_control_device.connect():
                self.uart_control_device.start()
                self.control_status_label.config(text="[Подключено]", fg="green")
            else:
                self.control_status_label.config(text="[Ошибка подключения]", fg="red")
        except Exception as e:
            print(f"Error initializing control device: {e}")
            self.control_status_label.config(text="[Ошибка]", fg="red")

    def _reconnect_control_device(self) -> None:
        """Reconnect to control device with new port."""
        if self.uart_control_device:
            self.uart_control_device.disconnect()

        self._init_control_device()

    def _on_stm32_position_received(self, stage_index: int) -> None:
        """Handle position received from STM32 (called from UART thread)."""
        # Schedule update in main thread
        self.root.after(0, self._update_stage_from_stm32, stage_index)
    
    def _update_stage_from_stm32(self, stage_index: int) -> None:
        """Update stage from STM32 position (called in main thread)."""
        if stage_index < 0 or stage_index >= NUM_STAGES:
            return
        
        prev_stage = self.current_stage
        self.current_stage = stage_index

        # Copy mood from previous stage to new stage when transitioning
        # This remembers the mood and applies it to the next stage
        if prev_stage is not None and prev_stage != stage_index:
            prev_mood_value = self.stage_mood_values[prev_stage]
            prev_leg_spread = self.stage_leg_spreads[prev_stage]
            # Copy mood value and leg spread from previous stage to new stage (remember state)
            self.stage_mood_values[stage_index] = prev_mood_value
            self.stage_leg_spreads[stage_index] = prev_leg_spread

        # Update history
        if stage_index not in self.stage_history or (self.stage_history and self.stage_history[-1] != stage_index):
            self.stage_history.append(stage_index)

            # Keep only last 3 stages
            if len(self.stage_history) > self.max_history_length:
                # Remove oldest stage
                old_stage = self.stage_history.pop(0)
                self._erase_stage(old_stage)

        # Draw stage with current mood
        self._draw_stage(stage_index)
        
        # Draw arrow from previous to current if there was a transition
        if prev_stage is not None and prev_stage != stage_index:
            # Clear all previous arrows
            self._clear_all_arrows()

            # Draw arrows between stages in history
            for i in range(len(self.stage_history) - 1):
                from_idx = self.stage_history[i]
                to_idx = self.stage_history[i + 1]
                self._draw_arrow(from_idx, to_idx)

            self._send_arrows_to_background()
    
    def _on_speed_change(self, event=None) -> None:
        """Handle speed change event."""
        speed_value = self.speed.get()
        # Send speed to STM32
        if self.uart_communicator:
            self.uart_communicator.send_speed(speed_value)
    
    def _on_canvas_resize(self, event: tk.Event) -> None:
        """Handle canvas resize event."""
        if event.width < 100 or event.height < 100:
            return
        
        # Update canvas dimensions
        self.canvas_width = event.width
        self.canvas_height = event.height
        
        # Calculate scale factors
        self.scale_x = self.canvas_width / self.base_width
        self.scale_y = self.canvas_height / self.base_height
        
        # Use minimum scale to maintain aspect ratio (or use separate scales)
        # For now, use separate scales for better window utilization
        self.canvas_center_x = self.canvas_width // 2
        self.canvas_center_y = self.canvas_height // 2
        self.canvas_radius = int(CIRCLE_RADIUS * min(self.scale_x, self.scale_y))
        
        # Clear canvas and redraw everything
        self.canvas.delete("all")
        
        # Recompute positions
        self.stage_positions = self._compute_stage_positions()
        
        # Redraw in correct order: guides -> labels -> arrows -> stages
        self._draw_guides()
        self._draw_labels()
        
        # Clear all arrows and redraw them based on history
        self.arrows.clear()
        self.arrow_tags.clear()
        for i in range(len(self.stage_history) - 1):
            from_idx = self.stage_history[i]
            to_idx = self.stage_history[i + 1]
            self._draw_arrow(from_idx, to_idx)
        
        # Redraw only stages in history
        self.drawn_stages.clear()
        for stage_idx in self.stage_history:
            self._draw_stage(stage_idx)
        
        # Ensure arrows are still on background
        self._send_arrows_to_background()
    
    def _compute_stage_positions(self) -> list[tuple[int, int]]:
        """Compute positions for all stages on a circle."""
        positions: list[tuple[int, int]] = []
        for i in range(NUM_STAGES):
            angle_deg = -90 + (360 / NUM_STAGES) * i
            angle_rad = math.radians(angle_deg)
            x = int(self.canvas_center_x + self.canvas_radius * math.cos(angle_rad))
            y = int(self.canvas_center_y + self.canvas_radius * math.sin(angle_rad))
            positions.append((x, y))
        return positions

    def _draw_guides(self) -> None:
        """Draw the outer circle guide."""
        self.canvas.create_oval(
            self.canvas_center_x - self.canvas_radius,
            self.canvas_center_y - self.canvas_radius,
            self.canvas_center_x + self.canvas_radius,
            self.canvas_center_y + self.canvas_radius,
            outline="#EAEAEA",
        )

    def _draw_labels(self) -> None:
        """Draw instructions and stage labels."""
        # Calculate font size based on scale
        font_size = max(8, int(12 * min(self.scale_x, self.scale_y)))
        label_font_size = max(7, int(10 * min(self.scale_x, self.scale_y)))
        
        # Instructions
        lines = [
            "Этапы управляются через STM32 плату",
            "Настроение: z=нейтр. x=счастл. c=грустн. v=злой (для текущего этапа)",
            "1: сон с будильником → 2: проснулся и выключил будильник → … → 12: ночной сон",
        ]
        self.canvas.create_text(
            int(20 * self.scale_x),
            int(20 * self.scale_y),
            anchor="nw",
            text="\n".join(lines),
            font=("Segoe UI", font_size),
            fill="#333333",
        )

        # Stage index labels - all outside circle at same distance from center
        label_radius = self.canvas_radius + int(50 * min(self.scale_x, self.scale_y))  # Fixed distance outside circle
        for idx in range(NUM_STAGES):
            angle_deg = -90 + (360 / NUM_STAGES) * idx
            angle_rad = math.radians(angle_deg)
            # Position label outside circle at fixed radius
            label_x = int(self.canvas_center_x + label_radius * math.cos(angle_rad))
            label_y = int(self.canvas_center_y + label_radius * math.sin(angle_rad))
            display_text = get_stage_display_index(idx)
            self.canvas.create_text(
                label_x, label_y, text=display_text,
                font=("Segoe UI", label_font_size), fill="#777777"
            )

    def _bind_keys(self) -> None:
        """Bind keyboard keys for mood changes and leg spread control."""
        # Mood keys - cycle through moods
        self.root.bind_all("z", lambda e: self.on_mood_change(-MOOD_STEP))  # Previous mood
        self.root.bind_all("x", lambda e: self.on_mood_change(MOOD_STEP))   # Next mood

        # Also bind uppercase versions
        self.root.bind_all("Z", lambda e: self.on_mood_change(-MOOD_STEP))
        self.root.bind_all("X", lambda e: self.on_mood_change(MOOD_STEP))

        # Leg spread keys
        self.root.bind_all("q", lambda e: self.on_leg_spread_change(-LEG_SPREAD_STEP))  # Decrease
        self.root.bind_all("e", lambda e: self.on_leg_spread_change(LEG_SPREAD_STEP))   # Increase

        # Also bind uppercase versions
        self.root.bind_all("Q", lambda e: self.on_leg_spread_change(-LEG_SPREAD_STEP))
        self.root.bind_all("E", lambda e: self.on_leg_spread_change(LEG_SPREAD_STEP))


    # ---------------- Event Handlers ----------------


    def on_mood_change(self, delta: int) -> None:
        """Handle mood change with delta (incremental adjustment)."""
        if self.current_stage is None:
            return

        # Apply delta to current mood value
        current_value = self.stage_mood_values[self.current_stage]
        new_value = current_value + delta

        # Clamp to limits defined in constants
        new_value = max(MOOD_MIN, min(MOOD_MAX, new_value))

        self.stage_mood_values[self.current_stage] = new_value

        # Redraw only this stage with new mood
        self._erase_stage(self.current_stage)
        self._draw_stage(self.current_stage)
        # Ensure arrows stay on background
        self._send_arrows_to_background()

    def on_leg_spread_change(self, delta: int) -> None:
        """Handle leg spread change with delta (incremental adjustment)."""
        if self.current_stage is None:
            return

        # Apply delta to current leg spread
        current_spread = self.stage_leg_spreads[self.current_stage]
        new_spread = current_spread + delta

        # Clamp to limits defined in constants
        new_spread = max(LEG_SPREAD_MIN, min(LEG_SPREAD_MAX, new_spread))

        self.stage_leg_spreads[self.current_stage] = new_spread

        # Redraw only this stage with new leg spread
        self._erase_stage(self.current_stage)
        self._draw_stage(self.current_stage)
        # Ensure arrows stay on background
        self._send_arrows_to_background()

    # ---------------- Drawing ----------------
    def _draw_stage(self, idx: int) -> None:
        """Draw a stage scene."""
        x, y = self.stage_positions[idx]
        tag = self.stage_tags[idx]
        mood_value = self.stage_mood_values[idx]
        leg_spread = self.stage_leg_spreads[idx]

        # To avoid overlap with the circle border, slightly shift inward
        x_in = int(self.canvas_center_x + (self.canvas_radius - 30) * ((x - self.canvas_center_x) / max(1, math.hypot(x - self.canvas_center_x, y - self.canvas_center_y))))
        y_in = int(self.canvas_center_y + (self.canvas_radius - 30) * ((y - self.canvas_center_y) / max(1, math.hypot(x - self.canvas_center_x, y - self.canvas_center_y))))

        # Use stage drawer to draw the scene with mood_value
        # Pass empty string for mood since we're using mood_value now
        self.stage_drawer.draw_stage(idx, x_in, y_in, tag, "", leg_spread, mood_value)

        self.drawn_stages.add(idx)

    def _erase_stage(self, idx: int) -> None:
        """Erase a stage from the canvas."""
        self.canvas.delete(self.stage_tags[idx])
        if idx in self.drawn_stages:
            self.drawn_stages.remove(idx)

    def _draw_arrow(self, from_idx: int, to_idx: int) -> None:
        """Draw an arrow from one stage to another."""
        # Check if arrow already exists
        tag = f"arrow_{from_idx}_{to_idx}"
        if tag in self.arrow_tags:
            return  # Arrow already drawn
        
        (x1, y1) = self.stage_positions[from_idx]
        (x2, y2) = self.stage_positions[to_idx]

        # Move a bit inward from the circle so arrow does not overlap border
        def inward(x: int, y: int, delta: int = 10) -> tuple[int, int]:
            vx = x - self.canvas_center_x
            vy = y - self.canvas_center_y
            d = max(1.0, math.hypot(vx, vy))
            scale = (self.canvas_radius - delta) / d
            return int(self.canvas_center_x + vx * scale), int(self.canvas_center_y + vy * scale)

        sx, sy = inward(x1, y1, 24)
        ex, ey = inward(x2, y2, 24)

        # Control points outside circle to create a gentle curve
        mid_angle = math.atan2((sy + ey) / 2 - self.canvas_center_y, (sx + ex) / 2 - self.canvas_center_x)
        ctrl_r = self.canvas_radius + 60
        cx = int(self.canvas_center_x + ctrl_r * math.cos(mid_angle))
        cy = int(self.canvas_center_y + ctrl_r * math.sin(mid_angle))

        # Create arrow with special tag for all arrows
        self.canvas.create_line(
            sx, sy, cx, cy, ex, ey,
            smooth=True, arrow="last", width=2, fill="#000000", 
            tags=(tag, "arrows")  # Individual tag + group tag
        )
        self.arrows.append((from_idx, to_idx))
        self.arrow_tags.append(tag)
        
        # Immediately send to background
        self._send_arrows_to_background()
    
    def _clear_all_arrows(self) -> None:
        """Clear all arrows from canvas."""
        for tag in self.arrow_tags:
            self.canvas.delete(tag)
        self.arrows.clear()
        self.arrow_tags.clear()

    def _send_arrows_to_background(self) -> None:
        """Send all arrows to background (below all other elements)."""
        # Move all arrows to the bottom of the stack
        for tag in self.arrow_tags:
            try:
                self.canvas.tag_lower(tag)
            except Exception:
                pass
        # Also use the group tag
        try:
            self.canvas.tag_lower("arrows")
        except Exception:
            pass

    # ---------------- Alarm Handling ----------------
    def _start_alarm(self) -> None:
        """Start alarm sound (currently disabled)."""
        # Alarm sound disabled on stage 0
        pass

    def _stop_alarm(self) -> None:
        """Stop alarm sound."""
        self._alarm_stop.set()

    def _alarm_worker(self) -> None:
        """Alarm worker thread (currently unused)."""
        # Disabled - no sound on stage 1
        while not self._alarm_stop.is_set():
            time.sleep(0.3)


def main() -> None:
    """Main entry point."""
    root = tk.Tk()
    app = LifeCycleApp(root)
    try:
        root.mainloop()
    except KeyboardInterrupt:
        pass
    finally:
        app._stop_alarm()
        # Disconnect from UART devices
        if app.uart_communicator:
            app.uart_communicator.disconnect()
        if app.uart_control_device:
            app.uart_control_device.disconnect()


if __name__ == "__main__":
    main()
