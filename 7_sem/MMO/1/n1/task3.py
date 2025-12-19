original_list = [1, 34, 'qwerty', 12, 13, 16, 'Love', 'Python']

numbers = [item for item in original_list if isinstance(item, (int, float))]
words = [item for item in original_list if isinstance(item, str)]

print(f"Числовой список: {numbers}")
print(f"Список слов: {words}")

sum_numbers = sum(numbers)
product = 1
for num in numbers:
    product *= num

print(f"Сумма: {sum_numbers}")
print(f"Произведение: {product}")

sorted_numbers = sorted(numbers, reverse=True)
print(f"Три наибольших: {sorted_numbers[:3]}")