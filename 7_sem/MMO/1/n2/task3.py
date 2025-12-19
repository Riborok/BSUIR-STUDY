def first_positive_row_sum(matrix):
    for idx, row in enumerate(matrix):
        if all(x > 0 for x in row):
            return idx, sum(row)
    return None, 0

n, m = map(int, input("Введите размеры матрицы n m: ").split())
matrix = [list(map(int, input(f"Строка {i+1}: ").split())) for i in range(n)]
idx, total = first_positive_row_sum(matrix)
if idx is not None:
    print(f"Первая положительная строка под номером {idx+1}, сумма элементов = {total}")
else:
    print("Такая строка не найдена")
