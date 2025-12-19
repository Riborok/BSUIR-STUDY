shop_items = {
    "Тормозные колодки": ["Керамические колодки для передних тормозов", 2500, 15],
    "Масляный фильтр": ["Высококачественный масляный фильтр", 450, 25],
    "Свечи зажигания": ["Иридиевые свечи зажигания комплект 4шт", 1200, 10],
    "Аккумулятор": ["Автомобильный аккумулятор 60Ah", 4500, 8],
    "Амортизатор": ["Передний амортизатор газовый", 3200, 12]
}

def show_descriptions():
    print("\n=== ОПИСАНИЯ ===")
    for name, info in shop_items.items():
        print(f"{name} – {info[0]}")

def show_prices():
    print("\n=== ЦЕНЫ ===")
    for name, info in shop_items.items():
        print(f"{name} – {info[1]} руб.")

def show_quantities():
    print("\n=== КОЛИЧЕСТВО ===")
    for name, info in shop_items.items():
        print(f"{name} – {info[2]} шт.")

def show_all_info():
    print("\n=== ВСЯ ИНФОРМАЦИЯ ===")
    for name, info in shop_items.items():
        print(f"{name}: {info[0]}, {info[1]} руб., {info[2]} шт.")

def make_purchase():
    print("\n=== ПОКУПКА ===")
    total_cost = 0

    while True:
        item_name = input("Название товара (n - выход): ")
        if item_name.lower() == 'n':
            break

        if item_name in shop_items:
            quantity = int(input("Количество: "))
            if quantity <= shop_items[item_name][2]:
                cost = shop_items[item_name][1] * quantity
                total_cost += cost
                shop_items[item_name][2] -= quantity
                print(f"Добавлено: {item_name} x{quantity} = {cost} руб.")
                print(f"Осталось: {shop_items[item_name][2]} шт.")
            else:
                print("Недостаточно товара!")
        else:
            print("Товар не найден!")

    print(f"Общая стоимость: {total_cost} руб.")

while True:
    print("\n=== МЕНЮ ===")
    print("1. Просмотр описания")
    print("2. Просмотр цены")
    print("3. Просмотр количества")
    print("4. Всю информацию")
    print("5. Покупка")
    print("6. До свидания")

    choice = input("Выбор (1-6): ")

    if choice == "1":
        show_descriptions()
    elif choice == "2":
        show_prices()
    elif choice == "3":
        show_quantities()
    elif choice == "4":
        show_all_info()
    elif choice == "5":
        make_purchase()
    elif choice == "6":
        print("До свидания!")
        break