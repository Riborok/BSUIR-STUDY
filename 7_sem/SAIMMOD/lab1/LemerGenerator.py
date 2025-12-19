from typing import List
import math


class LemerGenerator:

    def __init__(self, a: int, c: int, m: int, x0: int) -> None:
        self.a = a
        self.c = c
        self.m = m
        self.x0 = x0
        self.x = x0
        self._check_params()

    def _check_params(self) -> None:
        if not (0 < self.a < self.m):
            raise ValueError("a должно быть в диапазоне (0, m)")
        if not (0 < self.x0 < self.m):
            raise ValueError("x0 должно быть в диапазоне (0, m)")
        if self.c != 0 and math.gcd(self.c, self.m) != 1:
            raise ValueError("c и m должны быть взаимно простыми")

    def next(self) -> int:
        self.x = (self.a * self.x + self.c) % self.m
        return self.x

    def sequence(self, n: int) -> List[int]:
        return [self.next() for _ in range(n)]

    def reset(self) -> None:
        self.x = self.x0

    def period(self, limit: int = 10 ** 6) -> int:
        self.reset()
        seen = {}
        max_iter = min(self.m, limit)
        for i in range(max_iter):
            v = self.next()
            if v in seen:
                self.reset()
                return i - seen[v]
            seen[v] = i
        self.reset()
        return -1


GENERATOR_CONFIGS = {
    "mul": {
        "a": 16807,
        "c": 0,
        "m": 2 ** 31 - 1,
        "name": "Мультипликативный"
    },
    "mix": {
        "a": 22695477,
        "c": 1,
        "m": 2 ** 32,
        "name": "Смешанный"
    }
}


def build_optimal_generator(kind: str = "mul", x0: int = 1) -> LemerGenerator:
    if kind not in GENERATOR_CONFIGS:
        available_kinds = ', '.join(GENERATOR_CONFIGS.keys())
        raise ValueError(f"Неизвестный тип '{kind}'. Доступные типы: {available_kinds}")

    config = GENERATOR_CONFIGS[kind]

    gen = LemerGenerator(
        a=config["a"],
        c=config["c"],
        m=config["m"],
        x0=x0
    )

    gen.kind = kind
    gen.config_name = config["name"]

    return gen
