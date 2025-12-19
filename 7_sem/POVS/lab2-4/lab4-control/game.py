import tkinter as tk
import random
import serial
import threading
from collections import deque

COM_PORT = 'COM7'
BAUDRATE = 115200

class SensitiveAimGame:
    CENTER_X = 2030
    CENTER_Y = 2020
    
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("Aim Game")
        self.root.geometry("800x600")
        self.root.resizable(False, False)
        
        self.canvas = tk.Canvas(self.root, width=800, height=500, bg='black')
        self.canvas.pack()
        
        self.info_frame = tk.Frame(self.root, bg='gray20')
        self.info_frame.pack(fill='x')
        
        self.time_label = tk.Label(self.info_frame, text="Time: 60", font=('Arial', 14), fg='white', bg='darkblue')
        self.time_label.pack(side='left', padx=10)
        
        self.score_label = tk.Label(self.info_frame, text="Score: 0", font=('Arial', 14), fg='white', bg='darkgreen')
        self.score_label.pack(side='left', padx=10)
        
        self.sensitivity_label = tk.Label(self.info_frame, text="Sensitivity: 5", font=('Arial', 12), fg='white', bg='purple')
        self.sensitivity_label.pack(side='left', padx=10)

        self.controls_label = tk.Label(self.info_frame, 
                                     text="D=Start/Restart B=Shoot C=+Sens A=-Sens", 
                                     font=('Arial', 10), fg='yellow', bg='black')
        self.controls_label.pack(side='right', padx=10)
        
        self.crosshair_x = 400  
        self.crosshair_y = 250
        self.crosshair_size = 15
        self.sensitivity = 5  
        
        self.targets = []
        self.score = 0
        self.game_time = 60
        self.game_active = False
        self.game_started = False
        
        self.joystick_data = {"x": 2030, "y": 2020}
        self.button_pressed = None
        self.serial_data = deque(maxlen=10)
        
        self.root.after(50, self.game_loop)
        self.root.after(1000, self.update_timer)
        self.setup_serial()
        self.show_start_screen()
    
    def setup_serial(self):
        try:
            self.ser = serial.Serial(COM_PORT, BAUDRATE, timeout=1)
            self.serial_thread = threading.Thread(target=self.read_serial, daemon=True)
            self.serial_thread.start()
        except:
            self.ser = None
    
    def read_serial(self):
        while True:
            if self.ser and self.ser.in_waiting > 0:
                try:
                    line = self.ser.readline().decode('utf-8', errors='ignore').strip()
                    if line:
                        self.serial_data.append(line)
                        
                        if line.startswith("JX") and "Y" in line:
                            try:
                                x_start = line.find("X") + 1
                                y_start = line.find("Y") + 1
                                x_end = line.find("Y")
                                
                                x_val = int(line[x_start:x_end])
                                y_val = int(line[y_start:])
                                
                                self.joystick_data["x"] = x_val
                                self.joystick_data["y"] = y_val
                            except:
                                pass
                        
                        elif line.startswith("B") and len(line) >= 2:
                            btn = line[1:].strip()
                            if btn in ["A", "B", "C", "D"]:
                                self.button_pressed = btn
                                self.handle_button_press(btn)
                except:
                    pass    
    
    def handle_button_press(self, button):
        if button == "D": 
            if not self.game_active:
                self.start_game()
            else:
                self.start_game()  
                
        elif button == "B" and self.game_active:  
            self.shoot()
            
        elif button == "C" and self.game_active: 
            self.sensitivity = min(10, self.sensitivity + 1)
            self.sensitivity_label.config(text=f"Sensitivity: {self.sensitivity}")
            
        elif button == "A" and self.game_active:  
            self.sensitivity = max(1, self.sensitivity - 1)
            self.sensitivity_label.config(text=f"Sensitivity: {self.sensitivity}")
    
    def update_crosshair_from_joystick(self):
        if not self.game_active:
            return
            
        DEAD_ZONE = 100  
        
        delta_x = self.joystick_data["y"] - self.CENTER_X
        delta_y = self.joystick_data["x"] - self.CENTER_Y
        
        if abs(delta_x) < DEAD_ZONE:
            delta_x = 0
        if abs(delta_y) < DEAD_ZONE:
            delta_y = 0
        
        speed_multiplier = self.sensitivity / 2.5
        
        if delta_x != 0:
            direction_x = 1 if delta_x > 0 else -1
            speed_x = min(12, abs(delta_x) / 500.0 * speed_multiplier)
            self.crosshair_x += direction_x * speed_x
        
        if delta_y != 0:
            direction_y = 1 if delta_y > 0 else -1
            speed_y = min(12, abs(delta_y) / 500.0 * speed_multiplier)
            self.crosshair_y -= direction_y * speed_y
        
        self.crosshair_x = max(self.crosshair_size, min(800 - self.crosshair_size, self.crosshair_x))
        self.crosshair_y = max(self.crosshair_size, min(500 - self.crosshair_size, self.crosshair_y))
    
    def show_start_screen(self):
        self.canvas.delete("all")
        self.canvas.create_text(400, 180, text="AIM GAME", fill="yellow", font=('Arial', 36, 'bold'))
        self.canvas.create_text(400, 420, text="Press D to start", fill="yellow", font=('Arial', 16, 'bold'))
        self.canvas.create_text(400, 480, text=f"Sensitivity: {self.sensitivity}", fill="orange", font=('Arial', 12))
    
    def start_game(self):
        self.game_active = True
        self.game_started = True
        self.game_time = 60
        self.score = 0
        self.targets = []
        self.sensitivity = 5
        
        self.crosshair_x = 400
        self.crosshair_y = 250
        
        self.score_label.config(text="Score: 0")
        self.time_label.config(text="Time: 60")
        self.sensitivity_label.config(text=f"Sensitivity: {self.sensitivity}")
        
        for _ in range(6):
            self.create_target()
    
    def create_target(self):
        size = random.randint(20, 35)
        self.targets.append({
            "x": random.randint(size, 800 - size),
            "y": random.randint(size, 400 - size),
            "size": size,
            "speed_x": random.uniform(-2, 2),
            "speed_y": random.uniform(-1.5, 1.5),
            "color": random.choice(['red', 'orange', 'yellow', 'pink', 'cyan']),
            "points": 10
        })
    
    def shoot(self):
        hit = False
        
        for target in self.targets[:]:
            distance = ((self.crosshair_x - target["x"]) ** 2 + (self.crosshair_y - target["y"]) ** 2) ** 0.5
            
            if distance < target["size"] + 10:  
                self.targets.remove(target)
                self.score += target["points"] 
                hit = True
                break
        
        if hit:
            self.canvas.create_oval(
                self.crosshair_x - 25, self.crosshair_y - 25,
                self.crosshair_x + 25, self.crosshair_y + 25,
                outline="green", width=4
            )
            self.canvas.create_text(
                self.crosshair_x, self.crosshair_y - 40,
                text="+10", fill="lime", font=('Arial', 16, 'bold')
            )
        else:
            self.canvas.create_oval(
                self.crosshair_x - 20, self.crosshair_y - 20,
                self.crosshair_x + 20, self.crosshair_y + 20,
                outline="red", width=3
            )
    
    def update_timer(self):
        if self.game_active:
            self.game_time -= 1
            self.time_label.config(text=f"Time: {self.game_time}")
            
            if self.game_time <= 0:
                self.game_active = False
        
        self.root.after(1000, self.update_timer)
    
    def game_loop(self):
        if self.game_active:
            self.update_crosshair_from_joystick()
            self.update_game()
        
        self.draw_game()
        self.root.after(50, self.game_loop)
    
    def update_game(self):
        for target in self.targets:
            target["x"] += target["speed_x"]
            target["y"] += target["speed_y"]
            
            if target["x"] <= target["size"] or target["x"] >= 800 - target["size"]:
                target["speed_x"] *= -1
            if target["y"] <= target["size"] or target["y"] >= 400 - target["size"]:
                target["speed_y"] *= -1
        
        if random.random() < 0.05 and len(self.targets) < 10:
            self.create_target()
    
    def draw_game(self):
        self.canvas.delete("all")
        
        if not self.game_started:
            self.show_start_screen()
            return
        
        for target in self.targets:
            self.canvas.create_oval(
                target["x"] - target["size"], target["y"] - target["size"],
                target["x"] + target["size"], target["y"] + target["size"],
                fill=target["color"], outline="white", width=2
            )
        
        cross_size = self.crosshair_size
        
        self.canvas.create_line(
            self.crosshair_x - cross_size, self.crosshair_y,
            self.crosshair_x + cross_size, self.crosshair_y,
            fill="red", width=3
        )
        
        self.canvas.create_line(
            self.crosshair_x, self.crosshair_y - cross_size,
            self.crosshair_x, self.crosshair_y + cross_size,
            fill="red", width=3
        )
        
        self.canvas.create_oval(
            self.crosshair_x - cross_size, self.crosshair_y - cross_size,
            self.crosshair_x + cross_size, self.crosshair_y + cross_size,
            outline="yellow", width=2
        )
            
        self.canvas.create_oval(
            self.crosshair_x - 3, self.crosshair_y - 3,
            self.crosshair_x + 3, self.crosshair_y + 3,
            fill="white"
        )
        
        self.score_label.config(text=f"Score: {self.score}")
        
        if not self.game_active and self.game_started:
            self.canvas.create_text(400, 200, text="GAME OVER", fill="red", font=('Arial', 36))
            self.canvas.create_text(400, 250, text=f"Final Score: {self.score}", fill="yellow", font=('Arial', 24))
            self.canvas.create_text(400, 300, text="Press D to restart", fill="lime", font=('Arial', 18))
    
    def run(self):
        self.root.mainloop()

if __name__ == "__main__":
    game = SensitiveAimGame()
    game.run()