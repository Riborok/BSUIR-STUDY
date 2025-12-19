import math

class Circle:
    pi = math.pi

    def __init__(self, radius: float):
        self.radius = radius

    def area(self) -> float:
        return Circle.pi * self.radius ** 2

    def circumference(self) -> float:
        return 2 * Circle.pi * self.radius

    @staticmethod
    def area_static(radius: float) -> float:
        return Circle.pi * radius ** 2

    @classmethod
    def from_diameter(cls, diameter: float) -> 'Circle':
        return cls(diameter / 2)

c1 = Circle(5)
c2 = Circle.from_diameter(20)

print(f"Круг с радиусом {c1.radius} -> площадь: {c1.area():.2f}, окружность: {c1.circumference():.2f}")
print(f"Статический метод площади для радиуса 7: {Circle.area_static(7):.2f}")
print(f"Круг, созданный по диаметру 20, радиус: {c2.radius}, площадь: {c2.area():.2f}")
