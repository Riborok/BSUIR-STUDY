#include <iostream>
#include <windows.h>

using ReplaceFunc = int (*)(const LPVOID);

const static std::string TARGET = "Hello, world!";
const static std::string REPLACEMENT = "Goodbye!";

int main() {
    HMODULE hDll = LoadLibraryA("D:\\Desktop\\lab5\\x64\\Debug\\StringReplacer.dll");

    ReplaceFunc replaceFunc = reinterpret_cast<ReplaceFunc>(GetProcAddress(hDll, "ReplaceString"));

    char* str = static_cast<char*>(VirtualAlloc(nullptr, TARGET.size() + 1, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE));
    memcpy(str, TARGET.c_str(), TARGET.length() + 1);
    
    std::cout << "Before:" << '\n';
    std::cout << str << '\n';

    const char* params[2] = { TARGET.c_str(), REPLACEMENT.c_str() };
    replaceFunc(reinterpret_cast<const LPVOID>(params));

    std::cout << "After:" << '\n';
    std::cout << str << '\n';

    VirtualFree(str, 0, MEM_RELEASE);
    FreeLibrary(hDll);
    return 0;
}