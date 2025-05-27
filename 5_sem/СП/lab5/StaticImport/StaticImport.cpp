#include <iostream>
#include <windows.h>

extern "C" __declspec(dllimport) int ReplaceString(const LPVOID);

const static std::string TARGET = "Hello, world!";
const static std::string REPLACEMENT = "Goodbye!";

int main() {
    char* str = static_cast<char*>(VirtualAlloc(nullptr, TARGET.size() + 1, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE));
    memcpy(str, TARGET.c_str(), TARGET.length() + 1);
    
    std::cout << "Before:" << '\n';
    std::cout << str << '\n';

    const char* params[2] = { TARGET.c_str(), REPLACEMENT.c_str() };
    ReplaceString(reinterpret_cast<const LPVOID>(params));

    std::cout << "After:" << '\n';
    std::cout << str << '\n';

    VirtualFree(str, 0, MEM_RELEASE);
    return 0;
}