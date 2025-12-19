import heapq, os, sys
import numpy as np, pandas as pd
from dataclasses import dataclass, field
from typing import List, Optional

# ------------------ CONFIG ------------------
CONFIG = {
    "num_columns_each_side": 3,  # L1..L3 and R1..R3 -> total 6 columns
    "arrival_mean": 45.0,        # mean interarrival time in seconds (1/λ) - базовое значение
    "fuel_volume_mu": 40.0,      # normal/lognormal params will be used (we'll use normal truncated)
    "fuel_volume_sigma": 10.0,
    "fuel_min": 5.0,
    "fuel_max": 100.0,
    "payment_mu": 60.0,          # seconds
    "payment_sigma": 15.0,
    "payment_min": 25.0,
    "payment_max": 55.0,
    "refill_speed": 1.5,         # liters per second (fixed)
    "simulation_time": 10 * 60 * 60,  # simulate 2 hours
    "left_side_probability": 0.65,  # probability that a car has fuel tank on the left side

    # ============ КОСТЫЛЬНОЕ РЕШЕНИЕ ДЛЯ СТАЦИОНАРНОГО СОСТОЯНИЯ ============
    "use_warmup": True,           # включить режим прогрева + стационарное состояние
    "warmup_time": 20 * 60,       # 10 минут прогрева (машины приходят ЧАСТО)
    "warmup_arrival_mean": 15.0,  # во время прогрева машины приходят каждые 15 сек (быстро заполняем)
    # После прогрева: машина приходит ТОЛЬКО когда другая уезжает (костыль для стационарности)
    # ========================================================================
}

# Use random seed from command-line argument or generate random one
if len(sys.argv) > 1:
    SEED = int(sys.argv[1])
else:
    SEED = np.random.randint(0, 2**31)

np.random.seed(SEED)
print(f"Running simulation with seed: {SEED}")

# ------------------ Data classes ------------------
_event_counter = 0
def next_event_counter():
    global _event_counter
    _event_counter += 1
    return _event_counter

@dataclass
class Car:
    id: int
    arrival_time: float
    side: str  # 'L' or 'R'
    volume: float
    payment_time: float
    refill_speed: float
    state: str = "idle"  # idle, queued_global, queued_local, service, paying, fueling, departed
    column_id: Optional[int] = None  # index of column assigned (0..5)
    slot_index: Optional[int] = None  # 0=farthest,1=middle,2=service
    time_entered_queue: Optional[float] = None
    time_start_payment: Optional[float] = None
    time_start_fueling: Optional[float] = None
    time_end_fueling: Optional[float] = None
    wait_to_payment: Optional[float] = None  # individual wait
    fueling_duration: Optional[float] = None

@dataclass
class Column:
    id: int
    side: str  # 'L' or 'R'
    slots: List[Optional[int]] = field(default_factory=lambda: [None, None, None])  # car ids
    busy_time_accum: float = 0.0
    last_busy_start: Optional[float] = None

    def occupancy(self) -> int:
        return sum(1 for s in self.slots if s is not None)

    def has_space(self) -> bool:
        return self.occupancy() < len(self.slots)

    def first_free_from_back(self) -> Optional[int]:
        if self.slots[2] is None:
            return 2
        if self.slots[1] is None:
            return 1
        if self.slots[0] is None:
            return 0
        return None

    def service_slot_free(self) -> bool:
        return self.slots[2] is None

@dataclass(order=True)
class Event:
    time: float
    counter: int
    type: str
    car_id: Optional[int] = field(compare=False, default=None)
    info: dict = field(compare=False, default_factory=dict)

# ------------------ Simulator state ------------------
class Simulator:
    def __init__(self, config):
        self.config = config
        self.time = 0.0
        self.event_list: List[Event] = []
        self.next_car_id = 0
        self.columns: List[Column] = []
        for i in range(config["num_columns_each_side"]):
            self.columns.append(Column(id=i, side='L'))
        for i in range(config["num_columns_each_side"]):
            self.columns.append(Column(id=i+config["num_columns_each_side"], side='R'))
        self.cars: dict[int, Car] = {}
        self.global_queue: List[int] = []
        self.operator_busy = False
        self.operator_busy_time = 0.0
        self.operator_last_busy_start: Optional[float] = None
        self.operator_queue: List[int] = []

        # Warmup state tracking
        self.warmup_time = config.get("warmup_time", 0.0)
        self.use_warmup = config.get("use_warmup", False)
        self.warmup_completed = False
        self.stats_start_time = 0.0  # Time when stats collection started

        # Параметры для костыльного стационарного состояния
        self.warmup_arrival_mean = config.get("warmup_arrival_mean", config["arrival_mean"])
        self.arrival_rate_history = []  # track for analysis

        self.stats = {
            "throughput": 0,
            "total_waiting_time_to_payment": 0.0,
            "waiting_count": 0,
            "queue_length_time_series": [],  # (time, len(global_queue))
            "throughput_time_series": [],  # (time, cumulative_throughput)
            "operator_state_time_series": [],  # (time, busy_flag)
            "columns_occupancy_time_series": [],  # (time, [occupancies...])
        }
        self.trace = []
        self._car_counter = 0

    def check_warmup_complete(self):
        """
        Проверяем, закончился ли период прогрева.
        После прогрева система уже заполнена и переходим в стационарный режим.
        """
        if self.use_warmup and not self.warmup_completed and self.time >= self.warmup_time:
            self.warmup_completed = True
            self.stats_start_time = self.time
            self.add_trace(f"=== WARMUP COMPLETE at t={self.time:.1f}s ===")
            self.add_trace(f"    System filled. Switching to STATIONARY mode")
            self.add_trace(f"    New cars will arrive ONLY when another car departs (костыль)")
            self.add_trace(f"    Current queue length: {len(self.global_queue)}")
            self.add_trace(f"    Current column occupancy: {[c.occupancy() for c in self.columns]}")

    def schedule(self, time: float, type: str, car_id: Optional[int]=None, info: dict=None):
        if info is None:
            info = {}
        ev = Event(time=time, counter=next_event_counter(), type=type, car_id=car_id, info=info)
        heapq.heappush(self.event_list, ev)

    def sample_interarrival(self):
        """
        Sample interarrival time.
        КОСТЫЛЬНОЕ РЕШЕНИЕ:
        - До прогрева: машины приходят ЧАСТО (warmup_arrival_mean) -> быстро заполняем систему
        - После прогрева: возвращаем None - новые машины НЕ приходят по таймеру,
          а приходят только когда другая машина уезжает (см. end_fueling)
        """
        if self.use_warmup:
            if self.warmup_completed:
                # После прогрева - не генерируем автоматически
                return None
            else:
                # Во время прогрева - быстрое заполнение (частые приходы)
                arrival_mean = self.warmup_arrival_mean
                return np.random.exponential(arrival_mean)
        else:
            # Без прогрева - обычная интенсивность
            return np.random.exponential(self.config["arrival_mean"])

    def sample_volume(self):
        while True:
            v = np.random.normal(self.config["fuel_volume_mu"], self.config["fuel_volume_sigma"])
            if v >= self.config["fuel_min"] and v <= self.config["fuel_max"]:
                return v

    def sample_payment_time(self):
        while True:
            t = np.random.normal(self.config["payment_mu"], self.config["payment_sigma"])
            if t >= self.config["payment_min"] and t <= self.config["payment_max"]:
                return t

    def add_trace(self, text):
        timestamped = f"{self.time:.3f}: {text}"
        self.trace.append(timestamped)

    def record_snapshot(self):
        # record global queue len
        self.stats["queue_length_time_series"].append((self.time, len(self.global_queue)))
        # throughput
        self.stats["throughput_time_series"].append((self.time, self.stats["throughput"]))
        # operator state (0/1)
        self.stats["operator_state_time_series"].append((self.time, 1 if self.operator_busy else 0))
        # columns occupancy snapshot
        occ = [c.occupancy() for c in self.columns]
        self.stats["columns_occupancy_time_series"].append((self.time, occ))

    def find_best_column_for_side(self, side: str) -> Optional[Column]:
        candidates = [c for c in self.columns if c.side == side and c.has_space()]
        if not candidates:
            return None
        candidates.sort(key=lambda c: (c.occupancy(), c.id))
        return candidates[0]

    def find_first_matching_in_global(self, side: str) -> Optional[int]:
        for idx, cid in enumerate(self.global_queue):
            if self.cars[cid].side == side:
                return idx
        return None

    def process_arrival(self, car: Car):
        self.add_trace(f"ARRIVAL car{car.id} side={car.side} vol={car.volume:.1f}")
        col = self.find_best_column_for_side(car.side)
        if col is not None:
            slot = col.first_free_from_back()
            assert slot is not None
            col.slots[slot] = car.id
            car.column_id = col.id
            car.slot_index = slot
            car.state = "queued_local"
            self.add_trace(f" car{car.id} -> Column{col.id} slot{slot}")
            if slot == 2:
                car.time_entered_queue = self.time
                self.try_start_payment_for_car(car)
            else:
                car.time_entered_queue = self.time
        else:
            car.state = "queued_global"
            car.time_entered_queue = self.time
            self.global_queue.append(car.id)
            self.add_trace(f" car{car.id} -> GLOBAL_QUEUE (len={len(self.global_queue)})")

    def try_start_payment_for_car(self, car: Car):
        if car.slot_index != 2:
            return
        if (not self.operator_busy) and len(self.operator_queue) == 0:
            self.start_payment(car)
        else:
            self.operator_queue.append(car.id)
            self.add_trace(f" car{car.id} queued for OPERATOR (queue_len={len(self.operator_queue)})")

    def start_payment(self, car: Car):
        car.state = "paying"
        car.time_start_payment = self.time
        payment_duration = car.payment_time
        # operator becomes busy
        if not self.operator_busy:
            self.operator_last_busy_start = self.time
        self.operator_busy = True
        self.add_trace(f" START_PAYMENT car{car.id} duration={payment_duration:.1f}s")
        # record operator state change
        self.stats["operator_state_time_series"].append((self.time, 1))
        self.schedule(self.time + payment_duration, "EndPayment", car_id=car.id)

    def end_payment(self, car: Car):
        car.state = "paid"
        self.add_trace(f" END_PAYMENT car{car.id}")
        
        if car.time_entered_queue is not None and car.time_start_payment is not None:
            wait = car.time_start_payment - car.time_entered_queue
            car.wait_to_payment = wait
            self.stats["total_waiting_time_to_payment"] += wait
            self.stats["waiting_count"] += 1

        # operator becomes free now; accumulate busy time
        if self.operator_last_busy_start is not None:
            self.operator_busy_time += (self.time - self.operator_last_busy_start)
            self.operator_last_busy_start = None
        self.operator_busy = False
        # record operator free state
        self.stats["operator_state_time_series"].append((self.time, 0))
        if car.slot_index == 2 and car.state == "paid":
            self.start_fueling(car)
        # start next payment from operator queue if any
        if len(self.operator_queue) > 0:
            next_car_id = self.operator_queue.pop(0)
            next_car = self.cars[next_car_id]
            # operator becomes busy again at current time
            self.operator_busy = True
            self.operator_last_busy_start = self.time
            next_car.time_start_payment = self.time
            self.add_trace(f" START_PAYMENT from_queue car{next_car.id} dur={next_car.payment_time:.1f}s")
            self.schedule(self.time + next_car.payment_time, "EndPayment", car_id=next_car.id)

    def start_fueling(self, car: Car):
        if car.slot_index != 2:
            return
        car.state = "fueling"
        car.time_start_fueling = self.time
        duration = car.volume / car.refill_speed
        car.fueling_duration = duration
        col = self.columns[car.column_id]
        if col.last_busy_start is None:
            col.last_busy_start = self.time
        self.add_trace(f" START_FUEL car{car.id} col{col.id} dur={duration:.1f}s vol={car.volume:.1f}")
        self.schedule(self.time + duration, "EndFueling", car_id=car.id)

    def end_fueling(self, car: Car):
        car.state = "departed"
        car.time_end_fueling = self.time
        self.add_trace(f" END_FUEL car{car.id} vol={car.volume:.1f}")
        self.stats["throughput"] += 1
        # update column busy time accumulation
        col = self.columns[car.column_id]
        if col.last_busy_start is not None:
            col.busy_time_accum += (self.time - col.last_busy_start)
            col.last_busy_start = None
        # free service slot
        col.slots[2] = None

        # free slot movement logic (same as original)
        if col.slots[1] is not None:
            moving_car_id = col.slots[1]
            col.slots[1] = None
            col.slots[2] = moving_car_id
            moving_car = self.cars[moving_car_id]
            moving_car.slot_index = 2
            moving_car.state = "queued_local"
            self.add_trace(f" MOVE local car{moving_car_id} slot1->slot2 on col{col.id}")
            self.try_start_payment_for_car(moving_car)
            if col.slots[0] is not None:
                moving_car_id2 = col.slots[0]
                col.slots[0] = None
                col.slots[1] = moving_car_id2
                moving_car2 = self.cars[moving_car_id2]
                moving_car2.slot_index = 1
                self.add_trace(f" MOVE local car{moving_car_id2} slot0->slot1 on col{col.id}")
                idx = self.find_first_matching_in_global(col.side)
                if idx is not None:
                    global_car_id = self.global_queue.pop(idx)
                    gcar = self.cars[global_car_id]
                    col.slots[0] = gcar.id
                    gcar.column_id = col.id
                    gcar.slot_index = 0
                    gcar.state = "queued_local"
                    self.add_trace(f" TRANSFER_GLOBAL car{gcar.id} -> col{col.id} slot0")
            else:
                idx = self.find_first_matching_in_global(col.side)
                if idx is not None:
                    global_car_id = self.global_queue.pop(idx)
                    gcar = self.cars[global_car_id]
                    col.slots[0] = gcar.id
                    gcar.column_id = col.id
                    gcar.slot_index = 0
                    gcar.state = "queued_local"
                    self.add_trace(f" TRANSFER_GLOBAL car{gcar.id} -> col{col.id} slot0")

        elif col.slots[0] is not None:
            moving_car_id = col.slots[0]
            col.slots[0] = None
            col.slots[2] = moving_car_id
            moving_car = self.cars[moving_car_id]
            moving_car.slot_index = 2
            moving_car.state = "queued_local"
            self.add_trace(f" MOVE local car{moving_car_id} slot0->slot2 on col{col.id}")
            self.try_start_payment_for_car(moving_car)
            idx = self.find_first_matching_in_global(col.side)
            if idx is not None:
                global_car_id = self.global_queue.pop(idx)
                gcar = self.cars[global_car_id]
                col.slots[0] = gcar.id
                gcar.column_id = col.id
                gcar.slot_index = 0
                gcar.state = "queued_local"
                self.add_trace(f" TRANSFER_GLOBAL car{gcar.id} -> col{col.id} slot0")
        else:
            idx = self.find_first_matching_in_global(col.side)
            if idx is not None:
                global_car_id = self.global_queue.pop(idx)
                gcar = self.cars[global_car_id]
                col.slots[2] = gcar.id
                gcar.column_id = col.id
                gcar.slot_index = 2
                gcar.state = "queued_local"
                gcar.time_entered_queue = self.time
                self.add_trace(f" TRANSFER_GLOBAL car{gcar.id} -> col{col.id} slot2")
                self.try_start_payment_for_car(gcar)

        # record throughput time series and snapshot after processing movement
        self.stats["throughput_time_series"].append((self.time, self.stats["throughput"]))

        # ============ КОСТЫЛЬ ДЛЯ СТАЦИОНАРНОГО СОСТОЯНИЯ ============
        # После прогрева: когда машина уезжает, практически сразу приходит новая
        if self.use_warmup and self.warmup_completed:
            # Небольшая случайная задержка (0.1-2 сек) для реалистичности
            delay = np.random.uniform(0.5, 1)
            self.schedule(self.time + delay, "Arrival_NewCar")
            self.add_trace(f" [STATIONARY] Scheduling new arrival in {delay:.1f}s (car departed)")
        # ================================================================


    def schedule_initial_arrival(self):
        t = self.time + self.sample_interarrival()
        self.schedule(t, "Arrival_NewCar")

    def run(self, until=None):
        self.schedule_initial_arrival()
        until = until if until is not None else self.config["simulation_time"]
        # initial snapshot
        self.record_snapshot()
        while self.event_list:
            ev = heapq.heappop(self.event_list)
            self.time = ev.time
            if self.time > until:
                break

            # Check if warmup period has ended
            self.check_warmup_complete()

            if ev.type == "Arrival_NewCar":
                car_id = self.next_car_id
                self.next_car_id += 1
                side = 'L' if np.random.random() < self.config["left_side_probability"] else 'R'
                volume = self.sample_volume()
                payment_time = self.sample_payment_time()
                refill_speed = self.config["refill_speed"]
                car = Car(id=car_id, arrival_time=self.time, side=side, volume=volume,
                          payment_time=payment_time, refill_speed=refill_speed)
                self.cars[car_id] = car
                self.process_arrival(car)

                # Планируем следующий приход только если не в стационарном режиме
                interarrival = self.sample_interarrival()
                if interarrival is not None:
                    tnext = self.time + interarrival
                    self.schedule(tnext, "Arrival_NewCar")
                # snapshot
                self.record_snapshot()
            elif ev.type == "EndPayment":
                car = self.cars[ev.car_id]
                if car.time_start_payment is None:
                    car.time_start_payment = ev.time - car.payment_time
                self.end_payment(car)
                self.record_snapshot()
            elif ev.type == "EndFueling":
                car = self.cars[ev.car_id]
                self.end_fueling(car)
                # snapshot after fueling end recorded inside end_fueling too
                self.record_snapshot()
            else:
                pass
        # finish accumulating busy times at simulation end
        if self.operator_last_busy_start is not None:
            self.operator_busy_time += (self.time - self.operator_last_busy_start)
            self.operator_last_busy_start = None
        for col in self.columns:
            if col.last_busy_start is not None:
                col.busy_time_accum += (self.time - col.last_busy_start)
                col.last_busy_start = None
        # final snapshot
        self.record_snapshot()
        self.add_trace(f"SIM_END time={self.time:.1f} throughput={self.stats['throughput']} global_q={len(self.global_queue)}")

# ------------------ Run simulation ------------------
def run_simulation(seed=None, config=None, return_timeseries=False):
    """Run simulation with given seed and config, return summary results

    Args:
        seed: Random seed for reproducibility
        config: Configuration dictionary
        return_timeseries: If True, also return timeseries data for visualization

    Returns:
        summary dict or (summary dict, timeseries dict) if return_timeseries=True
    """
    if config is None:
        config = CONFIG

    if seed is not None:
        np.random.seed(seed)

    sim = Simulator(config)
    sim.run(until=config["simulation_time"])

    # ------------------ Postprocessing and additional plots ------------------
    # Build series for plotting
    q_times, q_vals = zip(*sim.stats["queue_length_time_series"]) if sim.stats["queue_length_time_series"] else ([],[])
    thr_times, thr_vals = zip(*sim.stats["throughput_time_series"]) if sim.stats["throughput_time_series"] else ([],[])
    op_times, op_vals = zip(*sim.stats["operator_state_time_series"]) if sim.stats["operator_state_time_series"] else ([],[])
    occ_times = [t for t,occ in sim.stats["columns_occupancy_time_series"]]
    occ_matrix = np.array([occ for t,occ in sim.stats["columns_occupancy_time_series"]]) if sim.stats["columns_occupancy_time_series"] else np.array([])  # shape (n_samples, n_columns)

    # Gather per-car arrays (ИСКЛЮЧАЕМ данные прогрева)
    warmup_time = sim.warmup_time if sim.use_warmup else 0.0
    waits = [c.wait_to_payment for c in sim.cars.values()
             if c.wait_to_payment is not None and c.arrival_time >= warmup_time]
    fueling_durs = [c.fueling_duration for c in sim.cars.values()
                   if c.fueling_duration is not None and c.arrival_time >= warmup_time]
    time_in_system = [(c.time_end_fueling - c.arrival_time) for c in sim.cars.values()
                      if c.time_end_fueling is not None and c.arrival_time >= warmup_time]


    # Save trace for inspection
    trace_path = "azs_trace.txt"
    with open(trace_path, "w") as f:
        for line in sim.trace:
            f.write(line + "\n")

    # Save a CSV summary of per-car stats (ИСКЛЮЧАЕМ данные прогрева)
    rows = []
    for c in sim.cars.values():
        if c.arrival_time >= warmup_time:  # Только машины после прогрева
            rows.append({
                "car_id": c.id,
                "arrival_time": c.arrival_time,
                "time_start_payment": c.time_start_payment,
                "time_start_fueling": c.time_start_fueling,
                "time_end_fueling": c.time_end_fueling,
                "wait_to_payment": c.wait_to_payment,
                "fueling_duration": c.fueling_duration,
                "column_id": c.column_id,
                "side": c.side
            })
    df = pd.DataFrame(rows)
    df.to_csv("per_car_stats.csv", index=False)

    # Save arrival rate adjustments history (for stationarity analysis)
    if sim.arrival_rate_history:
        pd.DataFrame(sim.arrival_rate_history).to_csv("arrival_rate_adjustments.csv", index=False)

    # Print quick summary and files produced
    print("Produced files:")
    for fname in ["azs_trace.txt", "per_car_stats.csv", "summary_metrics.csv", "arrival_rate_adjustments.csv"]:
        if os.path.exists(fname):
            print(" -", fname)

    print("\nKey numeric summaries:")
    if sim.use_warmup:
        print(f"Warmup time: {sim.warmup_time:.1f}s ({sim.warmup_time/60:.1f} min)")
        print("Adaptive arrival rate after warmup: ENABLED")
    print("Total throughput:", sim.stats["throughput"])
    print("Average wait to payment (s):", (sum(waits)/len(waits)) if len(waits)>0 else None)
    print("Average fueling duration (s):", (sum(fueling_durs)/len(fueling_durs)) if len(fueling_durs)>0 else None)
    print("Operator busy time (s):", sim.operator_busy_time)
    print("Columns busy times (s):", [round(c.busy_time_accum,3) for c in sim.columns])

    # Display small table of overall metrics
    summary = {
        "seed": seed if seed is not None else "unknown",
        "simulation_time": sim.time,
        "warmup_time": sim.warmup_time if sim.use_warmup else 0.0,
        "use_warmup": sim.use_warmup,
        "throughput": sim.stats["throughput"],
        "avg_wait_to_payment_s": (sum(waits)/len(waits)) if len(waits)>0 else 0.0,
        "avg_fueling_dur_s": (sum(fueling_durs)/len(fueling_durs)) if len(fueling_durs)>0 else 0.0,
        "avg_time_in_system_s": (sum(time_in_system)/len(time_in_system)) if len(time_in_system)>0 else 0.0,
        "operator_util": sim.operator_busy_time / sim.time if sim.time>0 else 0.0,
        "global_queue_final_len": len(sim.global_queue),
        "max_queue_length": max(q_vals) if q_vals else 0,
    }

    # Add column utilizations
    for i, col in enumerate(sim.columns):
        summary[f"column_{i}_util"] = col.busy_time_accum / sim.time if sim.time > 0 else 0.0

    pd.DataFrame([summary]).to_csv("summary_metrics.csv", index=False)

    # Prepare timeseries data if requested
    if return_timeseries:
        timeseries = {
            'throughput_timeseries': pd.DataFrame({
                'time': list(thr_times),
                'value': list(thr_vals)
            }) if thr_times else pd.DataFrame(),

            'operator_state_timeseries': pd.DataFrame({
                'time': list(op_times),
                'value': list(op_vals)
            }) if op_times else pd.DataFrame(),

            'queue_length_timeseries': pd.DataFrame({
                'time': list(q_times),
                'value': list(q_vals)
            }) if q_times else pd.DataFrame(),


            'columns_occupancy_timeseries': pd.DataFrame({
                'time': occ_times,
                'columns': [list(occ) for occ in occ_matrix]
            }) if len(occ_times) > 0 else pd.DataFrame()
        }

        # Добавляем скользящие средние для дискретных откликов
        if not timeseries['queue_length_timeseries'].empty:
            window = min(10, len(timeseries['queue_length_timeseries']))
            timeseries['queue_length_timeseries']['moving_avg'] = \
                timeseries['queue_length_timeseries']['value'].rolling(
                    window=window, min_periods=1).mean()

        return summary, timeseries

    return summary


if __name__ == "__main__":
    # Run simulation with seed from command line or random
    results = run_simulation(seed=SEED, config=CONFIG)
    print("\n=== Simulation Summary ===")
    for key, value in results.items():
        print(f"{key}: {value}")

