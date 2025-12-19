def calculate_pi_gregory(n_terms):
    pi_quarter = 0
    for i in range(n_terms):
        term = (-1)**i / (2*i + 1)
        pi_quarter += term
    return pi_quarter * 4

pi_result = calculate_pi_gregory(1000)
print(f"π = {pi_result}")