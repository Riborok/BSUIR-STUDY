#include <fstream>
#include <iostream>
#include <vector>
#include <Windows.h>

extern "C" __declspec(dllexport) int ReplaceString(const LPVOID params) {
    const HANDLE process = GetCurrentProcess();
    SYSTEM_INFO si;
    GetSystemInfo(&si);
    
    std::ofstream logFile("ReplaceString_log", std::ios::out);
    logFile << "Started, hProcess " << process << '\n';
    
    const char* target = static_cast<char**>(params)[0];
    const size_t targetSize = strlen(target);
    const char* replacement = static_cast<char**>(params)[1];
    const size_t replacementSize = strlen(replacement);
    const size_t minSize = min(targetSize, replacementSize);
    
    logFile << "Target " << target << ", Size " << targetSize << '\n';
    logFile << "Replacement " << replacement << ", Size " << replacementSize << '\n';
    
    logFile << "Minimum app address " << si.lpMinimumApplicationAddress << '\n';
    logFile << "Maximum app address " << si.lpMaximumApplicationAddress << '\n';
    
    char* l = static_cast<char*>(si.lpMinimumApplicationAddress);
    const char* r = static_cast<const char*>(si.lpMaximumApplicationAddress);

    while (l < r) {
        MEMORY_BASIC_INFORMATION mbi;
        if (VirtualQuery(l, &mbi, sizeof(MEMORY_BASIC_INFORMATION))) {
            if (mbi.State == MEM_COMMIT && 
                    (mbi.Protect & (PAGE_READWRITE | PAGE_EXECUTE_READWRITE)) &&
                    !(mbi.Protect & PAGE_GUARD)) {
                logFile << "Address " << &l << ", Size " << mbi.RegionSize << '\n';
                std::vector<char> buffer(mbi.RegionSize);
                size_t bytesRead;
                if (ReadProcessMemory(process, l, buffer.data(), mbi.RegionSize, &bytesRead)) {
                    for (size_t i = 0; i <= bytesRead - targetSize; i++) {
                        if (memcmp(buffer.data() + i, target, targetSize) == 0) {
                            char* addr = l + i;
                            logFile << "Found target string at " << &addr << '\n';
                            
                            if (WriteProcessMemory(process, addr, replacement, minSize + 1, nullptr)) {
                                logFile << "Successfully replaced string" << '\n';
                            } else {
                                logFile << "Failed to replace string" << '\n';
                            }
                        }
                    }
                } else {
                    logFile << "Error ReadProcessMemory() at " << &l << '\n';
                }
            } else {
                logFile << "State " << mbi.State << ", Protect " << mbi.Protect << '\n';
            }

            l += mbi.RegionSize;
        } else {
            logFile << "Error VirtualQuery() at " << &l << '\n';
        }
    }

    logFile << "Completed" << '\n';
    logFile.close();
    return 0;
}
