class String:
    def __init__(self, value: str):
        self.value = value

    def length(self) -> int:
        return len(self.value)

    def reverse(self) -> str:
        return self.value[::-1]

    def to_upper(self) -> str:
        return self.value.upper()

    def to_lower(self) -> str:
        return self.value.lower()

    def count_substring(self, sub: str) -> int:
        return self.value.count(sub)

    def find_substring(self, sub: str) -> int:
        return self.value.find(sub)

    def replace_substring(self, old: str, new: str) -> str:
        return self.value.replace(old, new)

s = String("A man a plan a canal Panama")
print("Original:", s)
print("Length:", s.length())
print("Reversed:", s.reverse())
print("Upper:", s.to_upper())
print("Lower:", s.to_lower())
print("Count 'a':", s.count_substring("a"))
print("Find 'plan':", s.find_substring("plan"))
print("Replace 'a' -> 'o':", s.replace_substring("a", "o"))
