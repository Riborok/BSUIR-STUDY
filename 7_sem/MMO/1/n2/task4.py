def main():
    try:
        x = int(input("Введите числитель: "))
        y = int(input("Введите знаменатель: "))
        result = x / y
    except ZeroDivisionError as e:
        print(f"Ошибка деления на ноль: {e}")
    except ValueError as e:
        print(f"Неверный ввод числа: {e}")
    else:
        print(f"Результат деления: {result}")
    finally:
        print("Блок finally выполнен.")

main()
