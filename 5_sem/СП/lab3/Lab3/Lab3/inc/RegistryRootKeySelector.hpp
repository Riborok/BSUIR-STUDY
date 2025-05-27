#pragma once

#include <windows.h>

class RegistryRootKeySelector {
    static const wchar_t* const ROOT_KEY_NAMES[5];
    static const HKEY ROOT_HKEYS[5];

    HWND _hwndComboBox;
public:
    explicit RegistryRootKeySelector(const HWND parentHwnd, const int id);
    HKEY getSelectedRootKey() const;
};
