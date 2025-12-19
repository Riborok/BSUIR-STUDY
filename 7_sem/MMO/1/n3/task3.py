class Transport:
    def __init__(self, name: str, speed_kmh: float, cost_per_km: float):
        self.name = name
        self.speed = speed_kmh
        self.cost_rate = cost_per_km

    def travel_time(self, distance_km: float) -> float:
        return distance_km / self.speed

    def travel_cost(self, distance_km: float) -> float:
        return distance_km * self.cost_rate

    def mode(self) -> str:
        raise NotImplementedError()

    @staticmethod
    def write_to_file(filename: str, content: str):
        with open(filename, 'w', encoding='utf-8') as f:
            f.write(content)

class Airplane(Transport):
    def mode(self) -> str:
        return "Воздушный транспорт"

class Train(Transport):
    def mode(self) -> str:
        return "Железнодорожный транспорт"

class Car(Transport):
    def mode(self) -> str:
        return "Автомобильный транспорт"

routes = [
    ("Москва", "Санкт-Петербург", 700),
    ("Москва", "Казань", 800),
    ("Санкт-Петербург", "Казань", 1300)
]
transports = [
    Airplane("Самолет", 800, 5.0),
    Train("Поезд", 100, 1.0),
    Car("Автомобиль", 60, 0.5)
]
report_lines = []
for city_from, city_to, dist in routes:
    report_lines.append(f"Маршрут: {city_from} -> {city_to}, {dist} км")
    results = []
    for t in transports:
        time = t.travel_time(dist)
        cost = t.travel_cost(dist)
        results.append((t, time, cost))
        report_lines.append(f"  {t.mode()} ({t.name}): время {time:.2f} ч, стоимость {cost:.2f}")
    fastest = min(results, key=lambda x: x[1])
    cheapest = min(results, key=lambda x: x[2])
    report_lines.append(f"  -> Быстрый вариант: {fastest[0].mode()} ({fastest[0].name}), время {fastest[1]:.2f} ч")
    report_lines.append(f"  -> Дешевый вариант: {cheapest[0].mode()} ({cheapest[0].name}), стоимость {cheapest[2]:.2f}")
    report_lines.append('')
report = '\n'.join(report_lines)
print(report)
Transport.write_to_file('transport_report.txt', report)
