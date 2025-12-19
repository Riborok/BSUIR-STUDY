"""Module for drawing person components (head, body, legs)."""

from typing import Any

try:
    import tkinter as tk
except Exception:
    import Tkinter as tk  # type: ignore


class PersonDrawer:
    """Helper class for drawing person components."""
    
    @staticmethod
    def draw_head(canvas: tk.Canvas, cx: int, cy: int, tag: str) -> None:
        """Draw person's head at center (cx, cy)."""
        head_r = 14
        # Head
        canvas.create_oval(
            cx - head_r, cy - 40 - head_r,
            cx + head_r, cy - 40 + head_r,
            fill="#FFE0BD", outline="#444", width=2, tags=tag
        )
        # Ears
        canvas.create_oval(
            cx - head_r - 3, cy - 40 - 3,
            cx - head_r + 1, cy - 40 + 3,
            fill="#FFE0BD", outline="#444", tags=tag
        )
        canvas.create_oval(
            cx + head_r - 1, cy - 40 - 3,
            cx + head_r + 3, cy - 40 + 3,
            fill="#FFE0BD", outline="#444", tags=tag
        )
        # Hair tuft
        canvas.create_arc(
            cx - head_r, cy - 40 - head_r - 4,
            cx + head_r, cy - 40,
            start=30, extent=120, style="arc", width=2, tags=tag
        )
    
    @staticmethod
    def draw_eyes(canvas: tk.Canvas, cx: int, cy: int, tag: str, sleeping: bool = False) -> None:
        """Draw person's eyes."""
        eye_dx = 6
        if sleeping:
            # Sleeping eyes as short lines
            canvas.create_line(cx - eye_dx, cy - 42, cx - eye_dx + 4, cy - 42, width=2, tags=tag)
            canvas.create_line(cx + eye_dx - 4, cy - 42, cx + eye_dx, cy - 42, width=2, tags=tag)
        else:
            canvas.create_oval(cx - eye_dx - 2, cy - 44, cx - eye_dx + 2, cy - 40, fill="#111", outline="", tags=tag)
            canvas.create_oval(cx + eye_dx - 2, cy - 44, cx + eye_dx + 2, cy - 40, fill="#111", outline="", tags=tag)
    
    @staticmethod
    def draw_eyes_smooth(canvas: tk.Canvas, cx: int, cy: int, tag: str, mood_value: int, sleeping: bool = False) -> None:
        """Draw person's eyes with smooth mood interpolation (-100 to +100)."""
        eye_dx = 6
        if sleeping:
            # Sleeping eyes as short lines
            canvas.create_line(cx - eye_dx, cy - 42, cx - eye_dx + 4, cy - 42, width=2, tags=tag)
            canvas.create_line(cx + eye_dx - 4, cy - 42, cx + eye_dx, cy - 42, width=2, tags=tag)
        else:
            # Eye size changes slightly with mood
            # Angry: smaller, more intense
            # Happy: slightly larger
            eye_size_adjust = mood_value / 50  # Range: -2 to +2
            eye_size = 2 + (eye_size_adjust * 0.3 if mood_value > 0 else 0)

            canvas.create_oval(
                cx - eye_dx - eye_size, cy - 44,
                cx - eye_dx + eye_size, cy - 40,
                fill="#111", outline="", tags=tag
            )
            canvas.create_oval(
                cx + eye_dx - eye_size, cy - 44,
                cx + eye_dx + eye_size, cy - 40,
                fill="#111", outline="", tags=tag
            )

    @staticmethod
    def draw_mouth(canvas: tk.Canvas, cx: int, cy: int, mood: str, tag: str, sleeping: bool = False) -> None:
        """Draw person's mouth based on mood."""
        if sleeping:
            # Sleeping mouth
            if mood == "happy":
                canvas.create_arc(cx - 8, cy + 2, cx + 8, cy + 12, start=200, extent=140, style="arc", width=2, tags=tag)
            elif mood == "sad":
                canvas.create_arc(cx - 8, cy + 6, cx + 8, cy + 16, start=20, extent=140, style="arc", width=2, tags=tag)
            elif mood == "angry":
                canvas.create_arc(cx - 8, cy + 6, cx + 8, cy + 16, start=20, extent=140, style="arc", width=2, tags=tag)
            else:
                canvas.create_line(cx - 6, cy + 9, cx + 6, cy + 9, width=2, tags=tag)
        else:
            # Awake mouth
            if mood == "happy":
                canvas.create_arc(cx - 10, cy - 36, cx + 10, cy - 24, start=200, extent=140, style="arc", width=2, tags=tag)
            elif mood == "sad":
                canvas.create_arc(cx - 10, cy - 28, cx + 10, cy - 16, start=20, extent=140, style="arc", width=2, tags=tag)
            elif mood == "angry":
                canvas.create_line(cx - 8, cy - 30, cx + 8, cy - 30, width=2, tags=tag)
                # Angry eyebrows
                canvas.create_line(cx - 10, cy - 47, cx - 4, cy - 43, width=2, tags=tag)
                canvas.create_line(cx + 10, cy - 47, cx + 4, cy - 43, width=2, tags=tag)
            else:  # neutral
                canvas.create_line(cx - 8, cy - 30, cx + 8, cy - 30, width=2, tags=tag)

    @staticmethod
    def draw_mouth_smooth(canvas: tk.Canvas, cx: int, cy: int, mood_value: int, tag: str, sleeping: bool = False) -> None:
        """Draw person's mouth with smooth mood interpolation (-100 to +100)."""
        if sleeping:
            # Sleeping mouth - simplified
            if mood_value > 20:
                canvas.create_arc(cx - 8, cy + 2, cx + 8, cy + 12, start=200, extent=140, style="arc", width=2, tags=tag)
            elif mood_value < -20:
                canvas.create_arc(cx - 8, cy + 6, cx + 8, cy + 16, start=20, extent=140, style="arc", width=2, tags=tag)
            else:
                canvas.create_line(cx - 6, cy + 9, cx + 6, cy + 9, width=2, tags=tag)
        else:
            # Awake mouth with smooth interpolation
            # mouth_value ranges from -100 (very sad/angry) to +100 (very happy)

            if mood_value > 0:
                # Happy mouth (smile) - arc curves upward
                # The more positive, the bigger the smile
                width = 10 + (mood_value / 10)  # 10 to 20
                height_offset = -36 + (mood_value / 10)  # -36 to -26 (higher smile)
                start_angle = 200 - (mood_value / 5)  # 200 to 180
                extent = 140 + (mood_value / 5)  # 140 to 160

                canvas.create_arc(
                    cx - width, cy + height_offset,
                    cx + width, cy - 24,
                    start=start_angle, extent=extent,
                    style="arc", width=2, tags=tag
                )

            elif mood_value < -50:
                # Very angry mouth - straight line with angry eyebrows
                y_pos = cy - 30 + (mood_value + 50) / 10  # Slight down curve
                canvas.create_line(cx - 8, y_pos, cx + 8, y_pos, width=2, tags=tag)

                # Angry eyebrows (more intense as mood decreases)
                brow_angle = abs(mood_value) / 10  # More angled when angrier
                canvas.create_line(
                    cx - 10, cy - 47 + brow_angle / 2,
                    cx - 4, cy - 43 - brow_angle / 2,
                    width=2, tags=tag
                )
                canvas.create_line(
                    cx + 10, cy - 47 + brow_angle / 2,
                    cx + 4, cy - 43 - brow_angle / 2,
                    width=2, tags=tag
                )

            elif mood_value < 0:
                # Sad mouth (frown) - arc curves downward
                # The more negative, the bigger the frown
                sad_factor = abs(mood_value)
                width = 10 + (sad_factor / 10)  # 10 to 15
                height_top = cy - 28 - (sad_factor / 10)  # Higher start
                height_bottom = cy - 16 + (sad_factor / 20)  # Lower end
                start_angle = 20 - (sad_factor / 10)
                extent = 140 + (sad_factor / 10)

                canvas.create_arc(
                    cx - width, height_top,
                    cx + width, height_bottom,
                    start=start_angle, extent=extent,
                    style="arc", width=2, tags=tag
                )

                # Slightly sad eyebrows (less intense than angry)
                if mood_value < -20:
                    brow_raise = abs(mood_value) / 20
                    canvas.create_line(
                        cx - 10, cy - 45 + brow_raise,
                        cx - 4, cy - 45 + brow_raise,
                        width=1, tags=tag
                    )
                    canvas.create_line(
                        cx + 10, cy - 45 + brow_raise,
                        cx + 4, cy - 45 + brow_raise,
                        width=1, tags=tag
                    )
            else:
                # Neutral mouth (near zero)
                canvas.create_line(cx - 8, cy - 30, cx + 8, cy - 30, width=2, tags=tag)

    @staticmethod
    def draw_body(canvas: tk.Canvas, cx: int, cy: int, tag: str) -> None:
        """Draw person's body (torso line)."""
        canvas.create_line(cx, cy - 26, cx, cy + 20, width=3, tags=tag)
    
    @staticmethod
    def draw_arms(canvas: tk.Canvas, cx: int, cy: int, mood: str, tag: str, with_arms: bool = True) -> None:
        """Draw person's arms based on mood."""
        if not with_arms:
            return
        
        if mood == "happy":
            canvas.create_line(cx, cy - 14, cx - 18, cy - 26, width=3, tags=tag)
            canvas.create_line(cx, cy - 14, cx + 18, cy - 26, width=3, tags=tag)
        elif mood == "angry":
            canvas.create_line(cx, cy - 14, cx - 18, cy - 6, width=3, tags=tag)
            canvas.create_line(cx, cy - 14, cx + 18, cy - 6, width=3, tags=tag)
        elif mood == "sad":
            canvas.create_line(cx, cy - 14, cx - 18, cy + 4, width=3, tags=tag)
            canvas.create_line(cx, cy - 14, cx + 18, cy + 4, width=3, tags=tag)
        else:  # neutral
            canvas.create_line(cx, cy - 14, cx - 18, cy - 2, width=3, tags=tag)
            canvas.create_line(cx, cy - 14, cx + 18, cy - 2, width=3, tags=tag)
    
    @staticmethod
    def draw_arms_smooth(canvas: tk.Canvas, cx: int, cy: int, mood_value: int, tag: str, with_arms: bool = True) -> None:
        """Draw person's arms with smooth interpolation based on mood_value (-100 to +100)."""
        if not with_arms:
            return

        # Smooth interpolation of arm positions based on mood_value
        # mood_value from -100 (very sad/angry, arms down) to +100 (very happy, arms up)

        # Calculate arm end Y position with smooth interpolation
        # Happy (+100): arms up at cy - 26
        # Neutral (0): arms middle at cy - 2
        # Sad/Angry (-100): arms down at cy + 4

        if mood_value >= 0:
            # Positive mood: interpolate from neutral to happy
            # 0 -> cy - 2, +100 -> cy - 26
            factor = mood_value / 100.0  # 0.0 to 1.0
            arm_y = cy - 2 - (24 * factor)  # -2 to -26
        else:
            # Negative mood: interpolate from neutral to sad/angry
            # 0 -> cy - 2, -100 -> cy + 4
            factor = abs(mood_value) / 100.0  # 0.0 to 1.0
            arm_y = cy - 2 + (6 * factor)  # -2 to +4

        # Draw arms with calculated position
        canvas.create_line(cx, cy - 14, cx - 18, int(arm_y), width=3, tags=tag)
        canvas.create_line(cx, cy - 14, cx + 18, int(arm_y), width=3, tags=tag)

    @staticmethod
    def draw_hands(canvas: tk.Canvas, cx: int, cy: int, tag: str, left_offset: int = -20, right_offset: int = 20, y_offset: int = -2) -> None:
        """Draw person's hands with adjustable position."""
        canvas.create_oval(
            cx + left_offset - 2, cy + y_offset - 2,
            cx + left_offset + 2, cy + y_offset + 2,
            fill="#FFE0BD", outline="#444", tags=tag
        )
        canvas.create_oval(
            cx + right_offset - 2, cy + y_offset - 2,
            cx + right_offset + 2, cy + y_offset + 2,
            fill="#FFE0BD", outline="#444", tags=tag
        )
    
    @staticmethod
    def draw_legs(canvas: tk.Canvas, cx: int, cy: int, tag: str, leg_spread: int = 14) -> None:
        """Draw person's legs with adjustable spread."""
        canvas.create_line(cx, cy + 20, cx - leg_spread, cy + 40, width=3, tags=tag)
        canvas.create_line(cx, cy + 20, cx + leg_spread, cy + 40, width=3, tags=tag)

    @staticmethod
    def draw_shoes(canvas: tk.Canvas, cx: int, cy: int, tag: str, leg_spread: int = 14) -> None:
        """Draw person's shoes with adjustable spread."""
        canvas.create_oval(cx - leg_spread - 6, cy + 40 - 2, cx - leg_spread + 6, cy + 40 + 4, fill="#333", outline="#111", tags=tag)
        canvas.create_oval(cx + leg_spread - 6, cy + 40 - 2, cx + leg_spread + 6, cy + 40 + 4, fill="#333", outline="#111", tags=tag)

    @staticmethod
    def draw_full_person(canvas: tk.Canvas, cx: int, cy: int, mood: str, tag: str, with_arms: bool = True, leg_spread: int = 14) -> None:
        """Draw a complete standing person."""
        PersonDrawer.draw_head(canvas, cx, cy, tag)
        PersonDrawer.draw_eyes(canvas, cx, cy, tag, sleeping=False)
        PersonDrawer.draw_mouth(canvas, cx, cy, mood, tag, sleeping=False)
        PersonDrawer.draw_body(canvas, cx, cy, tag)
        PersonDrawer.draw_arms(canvas, cx, cy, mood, tag, with_arms=with_arms)
        PersonDrawer.draw_hands(canvas, cx, cy, tag)
        PersonDrawer.draw_legs(canvas, cx, cy, tag, leg_spread=leg_spread)
        PersonDrawer.draw_shoes(canvas, cx, cy, tag, leg_spread=leg_spread)

    @staticmethod
    def draw_full_person_smooth(canvas: tk.Canvas, cx: int, cy: int, mood_value: int, tag: str, with_arms: bool = True, leg_spread: int = 14) -> None:
        """Draw a complete standing person with smooth mood interpolation."""
        PersonDrawer.draw_head(canvas, cx, cy, tag)
        PersonDrawer.draw_eyes_smooth(canvas, cx, cy, tag, mood_value, sleeping=False)
        PersonDrawer.draw_mouth_smooth(canvas, cx, cy, mood_value, tag, sleeping=False)
        PersonDrawer.draw_body(canvas, cx, cy, tag)

        # Use smooth arms interpolation
        PersonDrawer.draw_arms_smooth(canvas, cx, cy, mood_value, tag, with_arms=with_arms)

        # Calculate hand position based on mood_value (same as arms)
        if mood_value >= 0:
            factor = mood_value / 100.0
            hand_y = -2 - (24 * factor)  # -2 to -26
        else:
            factor = abs(mood_value) / 100.0
            hand_y = -2 + (6 * factor)  # -2 to +4

        PersonDrawer.draw_hands(canvas, cx, cy, tag, y_offset=int(hand_y))
        PersonDrawer.draw_legs(canvas, cx, cy, tag, leg_spread=leg_spread)
        PersonDrawer.draw_shoes(canvas, cx, cy, tag, leg_spread=leg_spread)


