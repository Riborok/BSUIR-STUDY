from typing import List, Dict, Any
import math

from LemerGenerator import GENERATOR_CONFIGS


def prime_divisors(num: int) -> List[int]:
    divs: List[int] = []
    n = num

    if n % 2 == 0:
        divs.append(2)
        while n % 2 == 0:
            n //= 2

    p = 3
    while p * p <= n:
        if n % p == 0:
            divs.append(p)
            while n % p == 0:
                n //= p
        p += 2

    if n > 1:
        divs.append(n)

    return divs


def period_criteria(a: int, c: int, m: int) -> Dict[str, Any]:
    res = {}
    b = a - 1
    g = math.gcd(c, m)
    res["gcd_ok"] = (g == 1)
    res["gcd_val"] = g
    primes = prime_divisors(m)
    res["prime_factors"] = primes
    res["b"] = b
    res["primes_ok"] = all(b % p == 0 for p in primes)
    res["need_4"] = m % 4 == 0
    res["b4_ok"] = (b % 4 == 0) if res["need_4"] else True
    res["all_ok"] = res["gcd_ok"] and res["primes_ok"] and res["b4_ok"]
    res["period"] = m if res["all_ok"] else "< m"
    return res


def analyze_configs():
    print("=" * 50)
    print("АНАЛИЗ ГЕНЕРАТОРОВ")
    print("=" * 50)

    best = None
    best_p = 0

    for kind, config in GENERATOR_CONFIGS.items():
        a, c, m = config["a"], config["c"], config["m"]
        name = config["name"]

        print(f"\n{name} ({kind})")
        print(f"a={a}, c={c}, m={m}")

        r = period_criteria(a, c, m)

        print("\nПроверка критериев:")
        print(f"  НОД(c,m) = {r['gcd_val']}  ->  {'✓' if r['gcd_ok'] else '✗'}")
        print(f"  b = a - 1 = {r['b']}")
        print(f"  Простые делители m: {r['prime_factors']}")
        for p in r["prime_factors"]:
            div_ok = r["b"] % p == 0
            print(f"    b mod {p} = {r['b'] % p}  ->  {'✓' if div_ok else '✗'}")
        if r["need_4"]:
            print(f"    b mod 4 = {r['b'] % 4}  ->  {'✓' if r['b4_ok'] else '✗'}")

        print("\nВыводы:")
        print(f"  Проверка всех условий: {'OK' if r['all_ok'] else 'FAIL'}")
        print(f"  Ожидаемый период по критериям: {r['period']}")

        if r["all_ok"]:
            theory_period = m
        elif kind == "mul":
            theory_period = m - 1
        else:
            theory_period = 0

        if theory_period > best_p:
            best_p = theory_period
            best = name

    print("\n" + "=" * 50)
    print(f"Лучший генератор: {best} (период {best_p})")


analyze_configs()