#pragma once

#include <string>
#include <vector>
#include <windows.h>

#include "RegTypeSelector.hpp"

class KeyValueEditor {
    HWND _hwndFieldNameLabel;
    HWND _hwndFieldNameInput;
    HWND _hwndValueLabel;
    HWND _hwndValueInput;
    RegTypeSelector _regTypeSelector;

public:
    explicit KeyValueEditor(const HWND parentHwnd);

    std::wstring getFieldNameText() const;
    std::wstring getValueText() const;
    DWORD getSelectedRegType() const;
    void setValueText(const LPCWSTR text) const;
    void setRegType(const DWORD regType) const;
    void addAvailableFieldNames(const std::vector<std::wstring>& fieldNames) const;
    void resetAvailableFieldNames() const;
};
