import ast

def process(arg):
    if isinstance(arg, list):
        return sum(x for x in arg if isinstance(x, (int, float)))
    elif isinstance(arg, dict):
        vals = sorted(arg.values(), reverse=True)
        return vals[:3]
    elif isinstance(arg, (int, float)):
        s = str(abs(int(arg)))
        return sum(int(d) for d in s)
    elif isinstance(arg, str):
        return len(arg.split())
    else:
        raise TypeError(f"Unsupported type: {type(arg)}")

if __name__ == "__main__":
    tests = [
        [1, 2, 3, 4.5],
        {'a': 10, 'b': 3, 'c': 7, 'd': 2, 'e': 100},
        12345,
        "Hello world this is a test"
    ]
    for arg in tests:
        print(f"{arg!r} -> {process(arg)}")
