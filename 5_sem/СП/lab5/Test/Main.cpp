#include <iostream>
#include <windows.h>

const static std::string STR = "Hello, world!";

int main() {
    std::cout << "pid " << GetCurrentProcessId() << '\n';

    char* str = static_cast<char*>(VirtualAlloc(nullptr, STR.length() + 1, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE));
	memcpy(str, STR.c_str(), STR.length() + 1);
	
	while (!GetAsyncKeyState(VK_ESCAPE)) {
        std::cout << str << '\n';
        Sleep(1000);
	}
	
    VirtualFree(str, 0, MEM_RELEASE);
	return 0;
}
