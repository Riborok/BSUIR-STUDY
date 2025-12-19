class Worker:
    def __init__(self, name: str, surname: str, position: str, wage: float, bonus: float):
        self.name = name
        self.surname = surname
        self.position = position
        self.income = {"wage": wage, "bonus": bonus}

class Position(Worker):
    def get_full_name(self) -> str:
        return f"{self.name} {self.surname}"

    def get_total_income(self) -> float:
        return self.income.get("wage", 0) + self.income.get("bonus", 0)

if __name__ == "__main__":
    pos1 = Position("Иван", "Иванов", "Менеджер", 50000, 10000)
    pos2 = Position("Петр", "Петров", "Разработчик", 80000, 20000)

    for pos in (pos1, pos2):
        print(f"Сотрудник: {pos.get_full_name()} | Должность: {pos.position}")
        print(f"Доход с учетом премии: {pos.get_total_income()}")
        print('---')
