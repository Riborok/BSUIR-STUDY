"""Module for drawing stage scenes."""

import math
from typing import Any

try:
    import tkinter as tk
except Exception:
    import Tkinter as tk  # type: ignore

from person_drawer import PersonDrawer
from constants import STAGE_LABELS


class StageDrawer:
    """Class for drawing all stage scenes."""
    
    def __init__(self, canvas: tk.Canvas):
        self.canvas = canvas
    
    def draw_stage(self, idx: int, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Draw a stage scene by index using a dispatch dictionary."""
        stage_drawers = {
            0: self.draw_stage_1_alarm_sleep,
            1: self.draw_stage_2_wake_disable_alarm,
            2: self.draw_stage_3_breakfast,
            3: self.draw_stage_4_commute,
            4: self.draw_stage_5_lecture,
            5: self.draw_stage_6_coding_on_laptop,
            6: self.draw_stage_7_break_walk,
            7: self.draw_stage_8_lunch,
            8: self.draw_stage_9_group_study,
            9: self.draw_stage_10_exam_prep,
            10: self.draw_stage_11_evening_relax,
            11: self.draw_stage_12_night_sleep,
        }
        
        drawer = stage_drawers.get(idx)
        if drawer:
            drawer(x, y, tag, mood, leg_spread=leg_spread, mood_value=mood_value)

    def draw_stage_1_alarm_sleep(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 1: Sleep with alarm (no sound)."""
        # Bed
        self.canvas.create_rectangle(
            x - 44, y - 12, x + 44, y + 22,
            fill="#DDE6F1", outline="#6C7A89", width=2, tags=tag
        )
        # Bed legs
        self.canvas.create_rectangle(x - 44, y + 22, x - 36, y + 26, outline="#6C7A89", tags=tag)
        self.canvas.create_rectangle(x + 36, y + 22, x + 44, y + 26, outline="#6C7A89", tags=tag)
        # Pillow
        self.canvas.create_oval(x - 40, y - 28, x + 2, y - 8, fill="#FFFFFF", outline="#6C7A89", tags=tag)
        # Person lying
        head_cx = x - 10
        head_cy = y - 22
        self.canvas.create_oval(
            head_cx - 12, head_cy - 12,
            head_cx + 12, head_cy + 12,
            fill="#FFE0BD", outline="#444", tags=tag
        )
        # Sleeping eyes
        self.canvas.create_line(head_cx - 6, head_cy - 2, head_cx - 2, head_cy - 2, width=2, tags=tag)
        self.canvas.create_line(head_cx + 2, head_cy - 2, head_cx + 6, head_cy - 2, width=2, tags=tag)
        # Mouth by mood (sleeping)
        PersonDrawer.draw_mouth(self.canvas, head_cx, head_cy, mood, tag, sleeping=True)
        # Angry eyebrows for angry mood when sleeping
        if mood == "angry":
            self.canvas.create_line(head_cx - 8, head_cy - 6, head_cx - 2, head_cy - 8, width=2, tags=tag)
            self.canvas.create_line(head_cx + 8, head_cy - 6, head_cx + 2, head_cy - 8, width=2, tags=tag)
        # Zzz
        self.canvas.create_text(x + 52, y - 40, text="Z", font=("Segoe UI", 10), fill="#555", tags=tag)
        self.canvas.create_text(x + 64, y - 52, text="Z", font=("Segoe UI", 12), fill="#777", tags=tag)
        # Alarm clock with bells (visual only, no sound)
        self.canvas.create_oval(x + 28, y - 40, x + 56, y - 12, fill="#FFEEEE", outline="#AA4444", width=2, tags=tag)
        self.canvas.create_oval(x + 26, y - 46, x + 34, y - 38, fill="#FFEEEE", outline="#AA4444", tags=tag)
        self.canvas.create_oval(x + 50, y - 46, x + 58, y - 38, fill="#FFEEEE", outline="#AA4444", tags=tag)
        self.canvas.create_line(x + 42, y - 26, x + 42, y - 34, width=2, tags=tag)
        self.canvas.create_line(x + 42, y - 26, x + 48, y - 22, width=2, tags=tag)
        # Ringers
        self.canvas.create_line(x + 60, y - 44, x + 68, y - 48, fill="#000", tags=tag)
        self.canvas.create_line(x + 60, y - 16, x + 68, y - 12, fill="#000", tags=tag)
        self.canvas.create_text(x, y + 36, text=STAGE_LABELS[0], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_2_wake_disable_alarm(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 2: Wake up and disable alarm."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Bedside table
        self.canvas.create_rectangle(x + 10, y - 6, x + 56, y + 16, outline="#777", fill="#F5F5F5", tags=tag)
        # Alarm clock off
        self.canvas.create_oval(x + 16, y - 22, x + 42, y + 0, fill="#EEE", outline="#888", tags=tag)
        self.canvas.create_line(x + 16, y - 11, x + 42, y - 11, fill="#C44", width=3, tags=tag)
        self.canvas.create_line(x + 22, y - 17, x + 36, y - 5, fill="#C44", width=2, tags=tag)
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[1], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_3_breakfast(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 3: Breakfast - fork should be in hand, table at correct height."""
        # Draw person first
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, with_arms=True, leg_spread=leg_spread)

        # Table - positioned correctly above feet (not under legs)
        table_y = y + 28  # Table top at reasonable height (above feet at y+40)
        self.canvas.create_line(x - 50, table_y, x + 50, table_y, width=4, fill="#8B4513", tags=tag)
        # Table legs
        self.canvas.create_line(x - 40, table_y, x - 40, table_y + 8, width=3, fill="#654321", tags=tag)
        self.canvas.create_line(x + 40, table_y, x + 40, table_y + 8, width=3, fill="#654321", tags=tag)
        
        # Plate on table
        plate_y = table_y - 6
        self.canvas.create_oval(x - 16, plate_y - 6, x + 16, plate_y + 6, outline="#444", fill="#FFFDF7", tags=tag)
        
        # Cup with steam
        self.canvas.create_rectangle(x + 22, plate_y - 8, x + 34, plate_y + 2, outline="#444", fill="#FFFFFF", tags=tag)
        self.canvas.create_line(x + 34, plate_y - 2, x + 36, plate_y - 2, width=2, tags=tag)
        self.canvas.create_line(x + 26, plate_y - 14, x + 24, plate_y - 18, fill="#AAA", tags=tag)
        self.canvas.create_line(x + 30, plate_y - 16, x + 28, plate_y - 20, fill="#AAA", tags=tag)
        
        # Fork in RIGHT hand (right hand is at x + 20, y - 2) - positioned correctly in hand
        fork_hand_x = x + 20
        fork_hand_y = y - 2
        # Fork tines (3 prongs) pointing toward plate
        self.canvas.create_line(fork_hand_x, fork_hand_y, fork_hand_x - 6, fork_hand_y - 10, width=2, fill="#888", tags=tag)
        self.canvas.create_line(fork_hand_x - 2, fork_hand_y - 2, fork_hand_x - 6, fork_hand_y - 8, width=1.5, fill="#888", tags=tag)
        self.canvas.create_line(fork_hand_x + 2, fork_hand_y - 2, fork_hand_x - 2, fork_hand_y - 8, width=1.5, fill="#888", tags=tag)
        # Fork handle (extends from hand)
        self.canvas.create_line(fork_hand_x, fork_hand_y, fork_hand_x + 8, fork_hand_y + 6, width=2, fill="#666", tags=tag)
        
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[2], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_4_commute(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 4: Commute to university."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Ground and motion lines
        self.canvas.create_line(x - 40, y + 44, x + 40, y + 44, fill="#888", tags=tag)
        self.canvas.create_line(x - 10, y + 30, x - 2, y + 26, fill="#888", tags=tag)
        self.canvas.create_line(x + 10, y + 32, x + 18, y + 28, fill="#888", tags=tag)
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[3], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_5_lecture(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 5: Lecture."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Board
        self.canvas.create_rectangle(x + 24, y - 40, x + 90, y - 8, outline="#444", fill="#E8F6FF", width=2, tags=tag)
        # Board scribbles
        self.canvas.create_line(x + 30, y - 30, x + 50, y - 30, fill="#2B5", width=2, tags=tag)
        self.canvas.create_line(x + 30, y - 24, x + 60, y - 24, fill="#2B5", tags=tag)
        self.canvas.create_line(x + 30, y - 18, x + 42, y - 18, fill="#2B5", tags=tag)
        # Lecturer inside the board, centered
        board_cx = (x + 24 + x + 90) // 2
        board_cy = (y - 40 + y - 8) // 2
        # Draw small person (no clothes inside board)
        head_r = 6
        self.canvas.create_oval(
            board_cx - head_r, board_cy - 18 - head_r,
            board_cx + head_r, board_cy - 18 + head_r,
            fill="#FFE0BD", outline="#444", tags=tag
        )
        self.canvas.create_line(board_cx, board_cy - 12, board_cx, board_cy + 8, width=2, tags=tag)
        # Pointer to the board
        self.canvas.create_line(board_cx, board_cy - 8, board_cx + 16, board_cy - 12, width=2, tags=tag)
        self.canvas.create_line(board_cx, board_cy - 8, board_cx - 10, board_cy - 2, width=2, tags=tag)
        self.canvas.create_line(board_cx, board_cy + 8, board_cx - 8, board_cy + 20, width=2, tags=tag)
        self.canvas.create_line(board_cx, board_cy + 8, board_cx + 8, board_cy + 20, width=2, tags=tag)
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[4], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_6_coding_on_laptop(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 6: Coding on laptop."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Desk - positioned correctly
        desk_y = y + 28
        self.canvas.create_line(x - 50, desk_y, x + 50, desk_y, width=4, fill="#8B4513", tags=tag)
        # Laptop on desk
        self.canvas.create_rectangle(x - 20, desk_y - 20, x + 6, desk_y - 6, outline="#333", fill="#EEE", tags=tag)
        for i in range(-16, 4, 4):
            self.canvas.create_line(x + i, desk_y - 10, x + i + 2, desk_y - 10, fill="#666", tags=tag)
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[5], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_7_break_walk(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 7: Break walk."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Tree
        self.canvas.create_rectangle(x + 56, y - 30, x + 62, y + 12, outline="#6B4E2E", fill="#8B5A2B", tags=tag)
        self.canvas.create_oval(x + 42, y - 56, x + 76, y - 22, fill="#79C267", outline="#4E8A3B", tags=tag)
        # Sun with rays
        self.canvas.create_oval(x + 36, y - 70, x + 60, y - 46, fill="#FFEE88", outline="#D4B500", tags=tag)
        for k in range(8):
            ang = math.radians(45 * k)
            sx = (x + 48) + 14 * math.cos(ang)
            sy = (y - 58) + 14 * math.sin(ang)
            ex = (x + 48) + 22 * math.cos(ang)
            ey = (y - 58) + 22 * math.sin(ang)
            self.canvas.create_line(sx, sy, ex, ey, fill="#D4B500", tags=tag)
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[6], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_8_lunch(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 8: Lunch - food and drink in hands."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Hand positions: left hand at x - 20, y - 2; right hand at x + 20, y - 2
        
        # Sausage in dough in RIGHT hand (right hand is at x + 20, y - 2)
        hand_right_x = x + 20
        hand_right_y = y - 2
        self.canvas.create_oval(
            hand_right_x + 2, hand_right_y - 8,
            hand_right_x + 14, hand_right_y - 2,
            fill="#F5C38B", outline="#A46B3B", tags=tag
        )
        # Bite marks on sausage
        self.canvas.create_arc(
            hand_right_x + 10, hand_right_y - 10,
            hand_right_x + 16, hand_right_y - 4,
            start=210, extent=100, style="arc", tags=tag
        )
        
        # Drink cup in LEFT hand (left hand is at x - 20, y - 2)
        hand_left_x = x - 20
        hand_left_y = y - 2
        # Cup body
        self.canvas.create_rectangle(
            hand_left_x - 6, hand_left_y - 8,
            hand_left_x + 2, hand_left_y + 2,
            outline="#444", fill="#E8F6FF", tags=tag
        )
        # Cup handle
        self.canvas.create_line(
            hand_left_x + 2, hand_left_y - 4,
            hand_left_x + 4, hand_left_y - 4, width=2, tags=tag
        )
        # Steam from cup
        self.canvas.create_line(
            hand_left_x - 4, hand_left_y - 10,
            hand_left_x - 6, hand_left_y - 14, fill="#AAA", tags=tag
        )
        self.canvas.create_line(
            hand_left_x - 2, hand_left_y - 12,
            hand_left_x - 4, hand_left_y - 16, fill="#AAA", tags=tag
        )
        
        self.canvas.create_text(x, y + 64, text=STAGE_LABELS[7], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_9_group_study(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 9: Walking home."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x - 8, y, mood_value, tag, leg_spread=leg_spread)
        # House
        self.canvas.create_rectangle(x + 12, y - 12, x + 46, y + 12, outline="#333", fill="#F7F7F7", tags=tag)
        self.canvas.create_polygon(x + 10, y - 12, x + 29, y - 28, x + 48, y - 12, outline="#333", fill="#EAD9C4", tags=tag)
        # Windows and door
        self.canvas.create_rectangle(x + 16, y - 6, x + 24, y + 2, outline="#333", fill="#E6F2FF", tags=tag)
        self.canvas.create_rectangle(x + 34, y - 6, x + 42, y + 2, outline="#333", fill="#E6F2FF", tags=tag)
        self.canvas.create_rectangle(x + 26, y, x + 34, y + 12, outline="#333", tags=tag)
        self.canvas.create_oval(x + 32, y + 6, x + 34, y + 8, fill="#333", outline="#333", tags=tag)
        self.canvas.create_text(x, y + 60, text=STAGE_LABELS[8], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_10_exam_prep(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 10: Exam preparation - fix table position."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # Table - positioned correctly at desk height (between waist and legs), not under legs
        # Person center y, legs start at y+20, so table at y+25-30 is good
        table_y = y + 26  # Table top at comfortable desk height
        self.canvas.create_line(x - 50, table_y, x + 50, table_y, width=4, fill="#8B4513", tags=tag)
        # Table legs (below table, extending down)
        self.canvas.create_line(x - 40, table_y, x - 40, table_y + 10, width=3, fill="#654321", tags=tag)
        self.canvas.create_line(x + 40, table_y, x + 40, table_y + 10, width=3, fill="#654321", tags=tag)
        
        # Book on table
        book_top_y = table_y - 12
        self.canvas.create_rectangle(x - 10, book_top_y, x + 12, table_y, outline="#333", fill="#EEE", tags=tag)
        # Open book (split in middle)
        self.canvas.create_line(x + 1, book_top_y, x + 1, table_y, width=2, fill="#666", tags=tag)
        self.canvas.create_polygon(x + 2, book_top_y, x + 20, book_top_y, x + 14, table_y + 4, x - 4, table_y + 4, outline="#555", fill="#FFFDF7", tags=tag)
        
        # Desk lamp
        lamp_base_y = table_y
        self.canvas.create_line(x - 30, lamp_base_y, x - 30, lamp_base_y - 20, width=2, tags=tag)
        self.canvas.create_line(x - 30, lamp_base_y - 20, x - 16, lamp_base_y - 28, width=2, tags=tag)
        self.canvas.create_oval(x - 20, lamp_base_y - 36, x - 10, lamp_base_y - 28, fill="#FFD966", outline="#CCB34D", tags=tag)
        
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[9], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_11_evening_relax(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 11: Evening relaxation."""
        PersonDrawer.draw_full_person_smooth(self.canvas, x, y, mood_value, tag, leg_spread=leg_spread)
        # TV with stand
        self.canvas.create_rectangle(x + 24, y - 24, x + 72, y + 6, outline="#333", fill="#F5F5F5", tags=tag)
        self.canvas.create_rectangle(x + 42, y + 6, x + 54, y + 10, outline="#333", fill="#333", tags=tag)
        # Controller with buttons
        self.canvas.create_oval(x - 26, y - 6, x - 10, y + 4, outline="#333", fill="#EAEAEA", tags=tag)
        self.canvas.create_oval(x - 20 - 1, y - 1 - 1, x - 20 + 1, y - 1 + 1, fill="#C44", outline="#C44", tags=tag)
        self.canvas.create_oval(x - 16 - 1, y + 1 - 1, x - 16 + 1, y + 1 + 1, fill="#4A9", outline="#4A9", tags=tag)
        # Small sofa
        self.canvas.create_rectangle(x - 36, y + 20, x + 6, y + 30, outline="#555", fill="#D9E1F2", tags=tag)
        self.canvas.create_text(x, y + 56, text=STAGE_LABELS[10], font=("Segoe UI", 10), fill="#333", tags=tag)
    
    def draw_stage_12_night_sleep(self, x: int, y: int, tag: str, mood: str, leg_spread: int = 14, mood_value: int = 0) -> None:
        """Stage 12: Night sleep."""
        # Bed, no alarm
        self.canvas.create_rectangle(x - 44, y - 12, x + 44, y + 22, fill="#EDE7F6", outline="#6C7A89", width=2, tags=tag)
        self.canvas.create_rectangle(x - 44, y + 22, x - 36, y + 26, outline="#6C7A89", tags=tag)
        self.canvas.create_rectangle(x + 36, y + 22, x + 44, y + 26, outline="#6C7A89", tags=tag)
        self.canvas.create_oval(x - 40, y - 28, x + 2, y - 8, fill="#FFFFFF", outline="#6C7A89", tags=tag)
        
        head_cx = x - 10
        head_cy = y - 22
        self.canvas.create_oval(head_cx - 12, head_cy - 12, head_cx + 12, head_cy + 12, fill="#FFE0BD", outline="#444", tags=tag)
        # Sleeping eyes
        self.canvas.create_line(head_cx - 6, head_cy - 2, head_cx - 2, head_cy - 2, width=2, tags=tag)
        self.canvas.create_line(head_cx + 2, head_cy - 2, head_cx + 6, head_cy - 2, width=2, tags=tag)
        # Mouth by mood
        PersonDrawer.draw_mouth(self.canvas, head_cx, head_cy, mood, tag, sleeping=True)
        # Angry eyebrows for angry mood when sleeping
        if mood == "angry":
            self.canvas.create_line(head_cx - 8, head_cy - 6, head_cx - 2, head_cy - 8, width=2, tags=tag)
            self.canvas.create_line(head_cx + 8, head_cy - 6, head_cx + 2, head_cy - 8, width=2, tags=tag)
        
        self.canvas.create_text(x, y + 36, text=STAGE_LABELS[11], font=("Segoe UI", 10), fill="#333", tags=tag)

