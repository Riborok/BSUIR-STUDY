#pragma once

#include <string>
#include <vector>
#include <windows.h>

namespace RegistryUtils {
    using OutputFunc = void(*)(const HWND, const LPCWSTR);
    
    void displayKeyInfo(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of);
    void createRegistryKey(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of);
    void deleteRegistryKey(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of);
    void trackKey(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of);
    void setRegistryValue(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey,
        const std::wstring& valueName, const std::wstring& valueData, const DWORD valueType, const OutputFunc& of);
    void deleteRegistryValue(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey,
        const std::wstring& valueName, const OutputFunc& of);
    std::pair<std::wstring, DWORD> getRegistryValue(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey,
            const std::wstring& valueName, const OutputFunc& of);
    std::vector<std::wstring> getFieldNames(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of);
}
