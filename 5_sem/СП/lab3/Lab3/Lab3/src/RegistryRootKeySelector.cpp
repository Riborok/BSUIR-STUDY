// ReSharper disable CppClangTidyPerformanceNoIntToPtr
#include "../inc/RegistryRootKeySelector.hpp"

const wchar_t* const RegistryRootKeySelector::ROOT_KEY_NAMES[5] = {
    L"HKEY_CURRENT_USER",
    L"HKEY_CLASSES_ROOT",
    L"HKEY_LOCAL_MACHINE",
    L"HKEY_USERS",
    L"HKEY_CURRENT_CONFIG"
};

const HKEY RegistryRootKeySelector::ROOT_HKEYS[5] = {
    HKEY_CURRENT_USER,
    HKEY_CLASSES_ROOT,
    HKEY_LOCAL_MACHINE,
    HKEY_USERS,
    HKEY_CURRENT_CONFIG
};

RegistryRootKeySelector::RegistryRootKeySelector(const HWND parentHwnd, const int id) {
    _hwndComboBox = CreateWindow(
        L"COMBOBOX",
        nullptr,
        WS_VISIBLE | WS_CHILD | CBS_DROPDOWNLIST,
        25, 25, 225, 200,
        parentHwnd,
        reinterpret_cast<HMENU>(id),
        nullptr,
        nullptr
    );

    for (const auto& key : ROOT_KEY_NAMES) {
        SendMessage(_hwndComboBox, CB_ADDSTRING, 0, reinterpret_cast<LPARAM>(key));
    }
    SendMessage(_hwndComboBox, CB_SETCURSEL, 0, 0);
}

HKEY RegistryRootKeySelector::getSelectedRootKey() const {
    const int index = SendMessage(_hwndComboBox, CB_GETCURSEL, 0, 0);
    return ROOT_HKEYS[index];
}
