#include <windows.h>
#include <iostream>
#include <string>
#include <tlhelp32.h>

const static std::string DLL_PATH = "D:\\Desktop\\lab5\\x64\\Debug\\StringReplacer.dll";
const static std::string FUNC_NAME = "ReplaceString";

LPVOID AllocateAndWriteVirtMemory(const HANDLE hProc, const void* data, const size_t size) {
    const LPVOID remoteMemory = VirtualAllocEx(hProc, nullptr, size, MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
    if (!remoteMemory) {
        std::cout << "Failed to allocate virtual memory\n";
        return nullptr;
    }
    if (!WriteProcessMemory(hProc, remoteMemory, data, size, nullptr)) {
        std::cout << "Failed to write into virtual memory\n";
        VirtualFreeEx(hProc, remoteMemory, 0, MEM_RELEASE);
        return nullptr;
    }
    return remoteMemory;
}

bool LoadDllIntoProcess(const HANDLE hProc) {
    const LPVOID loadLibraryA = GetProcAddress(GetModuleHandle(L"kernel32.dll"), "LoadLibraryA");
    if (!loadLibraryA) {
        std::cout << "Failed to get address of LoadLibraryA\n";
        return false;
    }

    const LPVOID dllPathAddr = AllocateAndWriteVirtMemory(hProc, DLL_PATH.c_str(), DLL_PATH.length() + 1);
    const HANDLE hThread = CreateRemoteThread(hProc, nullptr, 0, 
        reinterpret_cast<LPTHREAD_START_ROUTINE>(loadLibraryA), dllPathAddr, 0, nullptr);
    if (hThread) {
        WaitForSingleObject(hThread, INFINITE);
        CloseHandle(hThread);
        std::cout << "DLL loaded successfully, ";
        DWORD exitCode;
        if (GetExitCodeThread(hThread, &exitCode)) {
            std::cout << " Exit code: " << exitCode << '\n';
        } else {
            std::cout << " Failed to get exit code\n";
        }
        VirtualFreeEx(hProc, dllPathAddr, 0, MEM_RELEASE);
        return true;
    } else {
        std::cout << "Failed to load DLL\n";
        VirtualFreeEx(hProc, dllPathAddr, 0, MEM_RELEASE);
        return false;
    }
}

HMODULE FindInjectedDLL(const DWORD pid, const std::string& dllPath) {
    HMODULE hInjectedModule = nullptr;
    const HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, pid);
    if (hSnapshot != INVALID_HANDLE_VALUE) {
        MODULEENTRY32 moduleEntry;
        moduleEntry.dwSize = sizeof(MODULEENTRY32);

        if (Module32First(hSnapshot, &moduleEntry)) {
            do {
                char moduleNameA[MAX_PATH];
                WideCharToMultiByte(CP_ACP, 0, moduleEntry.szModule, -1, moduleNameA,
                    MAX_PATH, nullptr, nullptr);
                std::cout << "Loaded DLL: " << moduleNameA << '\n';
                if (dllPath.find(moduleNameA) != std::string::npos) {
                    hInjectedModule = moduleEntry.hModule;
                    break;
                }
            } while (Module32Next(hSnapshot, &moduleEntry));
        }
        CloseHandle(hSnapshot);
    }
    return hInjectedModule;
}

uintptr_t GetFuncOffsetFromModule() {
    HMODULE hLocalModule = LoadLibraryA(DLL_PATH.c_str());
    if (!hLocalModule) {
        std::cout << "Failed to load DLL\n";
        return 0;
    }
    FARPROC funcAddr = GetProcAddress(hLocalModule, FUNC_NAME.c_str());
    if (!funcAddr) {
        std::cout << "Failed to get function address\n";
        FreeLibrary(hLocalModule);
        return 0;
    }
    const uintptr_t offset = reinterpret_cast<uintptr_t>(funcAddr) - reinterpret_cast<uintptr_t>(hLocalModule);
    FreeLibrary(hLocalModule);
    return offset;
}

bool RunRemoteFunc(const HANDLE hProc, const LPTHREAD_START_ROUTINE remoteFuncAddr, const LPVOID remoteArgs) {
    HANDLE hThread = CreateRemoteThread(hProc, nullptr, 0, remoteFuncAddr, remoteArgs, 0, nullptr);
    if (hThread) {
        WaitForSingleObject(hThread, INFINITE);
        CloseHandle(hThread);
        std::cout << "Function ran successfully, ";
        
        DWORD exitCode;
        if (GetExitCodeThread(hThread, &exitCode)) {
            std::cout << " Exit code: " << exitCode << '\n';
        } else {
            std::cout << " Failed to get exit code\n";
        }
        return true;
    } else {
        std::cout << "Failed to run remote function\n";
        return false;
    }
}

int main() {
    DWORD pid;
    std::cin >> pid;
    const std::string target = "Hello, world!";
    const std::string replacement = "Goodbye!";

    const HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
    if (!hProc) {
        std::cout << "Failed to open process\n";
        return -1;
    }
    
    if (!LoadDllIntoProcess(hProc)) {
        CloseHandle(hProc);
        return -1;
    }
    
    HMODULE hInjectedModule = FindInjectedDLL(pid, DLL_PATH);
    if (!hInjectedModule) {
        std::cout << "Failed to find injected DLL\n";
        CloseHandle(hProc);
        return -1;
    }
    const uintptr_t offset = GetFuncOffsetFromModule();
    if (offset == 0) {
        CloseHandle(hProc);
        return -1;
    }
    const uintptr_t remoteFuncAddr = reinterpret_cast<uintptr_t>(hInjectedModule) + offset;

    const LPVOID remoteTarget = AllocateAndWriteVirtMemory(hProc, target.c_str(), target.length() + 1);
    if (!remoteTarget) {
        CloseHandle(hProc);
        return -1;
    }

    const LPVOID remoteReplacement = AllocateAndWriteVirtMemory(hProc, replacement.c_str(), replacement.length() + 1);
    if (!remoteReplacement) {
        VirtualFreeEx(hProc, remoteTarget, 0, MEM_RELEASE);
        CloseHandle(hProc);
        return -1;
    }

    const LPVOID args[2] = { remoteTarget, remoteReplacement };
    const LPVOID remoteArgs = AllocateAndWriteVirtMemory(hProc, args, 2 * sizeof(LPVOID));
    if (!remoteArgs) {
        VirtualFreeEx(hProc, remoteTarget, 0, MEM_RELEASE);
        VirtualFreeEx(hProc, remoteReplacement, 0, MEM_RELEASE);
        CloseHandle(hProc);
        return -1;
    }
    RunRemoteFunc(hProc, reinterpret_cast<LPTHREAD_START_ROUTINE>(remoteFuncAddr), remoteArgs);
    
    VirtualFreeEx(hProc, remoteTarget, 0, MEM_RELEASE);
    VirtualFreeEx(hProc, remoteReplacement, 0, MEM_RELEASE);
    VirtualFreeEx(hProc, remoteArgs, 0, MEM_RELEASE);
    CloseHandle(hProc);

    std::cin.get();
    return 0;
}
