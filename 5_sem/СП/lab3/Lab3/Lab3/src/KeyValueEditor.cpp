#include "../inc/KeyValueEditor.hpp"

KeyValueEditor::KeyValueEditor(const HWND parentHwnd): _regTypeSelector(parentHwnd, 25, 206, 225, 500) {
    _hwndFieldNameLabel = CreateWindow(
        L"STATIC",
        L"Field:",
        WS_VISIBLE | WS_CHILD,
        25, 112, 225, 22,
        parentHwnd,
        nullptr,
        nullptr,
        nullptr
    );

    _hwndFieldNameInput = CreateWindow(
        L"COMBOBOX",
        L"",
        WS_VISIBLE | WS_CHILD | CBS_DROPDOWN | CBS_AUTOHSCROLL,
        25, 134, 225, 500,
        parentHwnd,
        nullptr,
        nullptr,
        nullptr
    );

    _hwndValueLabel = CreateWindow(
        L"STATIC",
        L"Value:",
        WS_VISIBLE | WS_CHILD,
        25, 162, 225, 22,
        parentHwnd,
        nullptr,
        nullptr,
        nullptr
    );

    _hwndValueInput = CreateWindow(
        L"EDIT",
        L"",
        WS_VISIBLE | WS_CHILD | WS_BORDER | ES_AUTOHSCROLL,
        25, 184, 225, 22,
        parentHwnd,
        nullptr,
        nullptr,
        nullptr
    );
}

std::wstring KeyValueEditor::getFieldNameText() const {
    wchar_t buffer[MAXUINT16];
    GetWindowText(_hwndFieldNameInput, buffer, MAXUINT16);
    return {buffer};
}

std::wstring KeyValueEditor::getValueText() const {
    wchar_t buffer[MAXUINT16];
    GetWindowText(_hwndValueInput, buffer, MAXUINT16);
    return {buffer};
}

DWORD KeyValueEditor::getSelectedRegType() const {
    return _regTypeSelector.getSelectedRegType();
}

void KeyValueEditor::setValueText(const LPCWSTR text) const {
    SetWindowText(_hwndValueInput, text);
}

void KeyValueEditor::setRegType(const DWORD regType) const {
    _regTypeSelector.setRegType(regType);
}

void KeyValueEditor::addAvailableFieldNames(const std::vector<std::wstring>& fieldNames) const {
    resetAvailableFieldNames();
    for (const auto& fieldName : fieldNames) {
        SendMessage(_hwndFieldNameInput, CB_ADDSTRING, 0, reinterpret_cast<LPARAM>(fieldName.c_str()));
    }
}

void KeyValueEditor::resetAvailableFieldNames() const {
    SendMessage(_hwndFieldNameInput, CB_RESETCONTENT, 0, 0);
}
