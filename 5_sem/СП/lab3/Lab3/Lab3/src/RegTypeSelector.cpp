#include "../inc/RegTypeSelector.hpp"

const wchar_t* const RegTypeSelector::REG_TYPE_NAMES[4] = {
    L"REG_NONE",
    L"REG_SZ",
    L"REG_DWORD",
    L"REG_QWORD"
};

const DWORD RegTypeSelector::REG_TYPE_VALUES[4] = {
    REG_NONE,
    REG_SZ,
    REG_DWORD,
    REG_QWORD
};

RegTypeSelector::RegTypeSelector(const HWND parentHwnd, const int x, const int y, const int width, const int height) {
    _hwndComboBox = CreateWindow(
        L"COMBOBOX",
        nullptr,
        WS_VISIBLE | WS_CHILD | CBS_DROPDOWNLIST,
        x, y, width, height,
        parentHwnd,
        nullptr,
        nullptr,
        nullptr
    );

    for (const auto& typeName : REG_TYPE_NAMES) {
        SendMessage(_hwndComboBox, CB_ADDSTRING, 0, reinterpret_cast<LPARAM>(typeName));
    }
    SendMessage(_hwndComboBox, CB_SETCURSEL, 0, 0);
}

DWORD RegTypeSelector::getSelectedRegType() const {
    const int index = SendMessage(_hwndComboBox, CB_GETCURSEL, 0, 0);
    return REG_TYPE_VALUES[index];
}

void RegTypeSelector::setRegType(const DWORD regType) const {
    for (int i = 0; i < 4; ++i) {
        if (REG_TYPE_VALUES[i] == regType) {
            SendMessage(_hwndComboBox, CB_SETCURSEL, i, 0);
            return;
        }
    }
    SendMessage(_hwndComboBox, CB_SETCURSEL, 0, 0);
}
