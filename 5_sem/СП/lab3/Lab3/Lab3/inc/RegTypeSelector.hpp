#pragma once

#include <windows.h>

class RegTypeSelector {
    static const wchar_t* const REG_TYPE_NAMES[4];
    static const DWORD REG_TYPE_VALUES[4];
    
    HWND _hwndComboBox;
public:
    explicit RegTypeSelector(const HWND parentHwnd, const int x, const int y, const int width, const int height);
    DWORD getSelectedRegType() const;
    void setRegType(const DWORD regType) const;
};
