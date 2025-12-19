def find_min_even(tuple_data):
    even_elements = [x for x in tuple_data if isinstance(x, (int, float)) and x % 2 == 0]

    if even_elements:
        return min(even_elements)
    else:
        return tuple_data[0]

test_tuples = [
    (1, 3, 5, 7, 9),
    (2, 4, 6, 8, 10),
    (1, 2, 3, 4, 5),
    (7, 3, 1, 9)
]

for t in test_tuples:
    result = find_min_even(t)
    print(f"{t} -> {result}")