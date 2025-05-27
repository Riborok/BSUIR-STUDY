// ReSharper disable CppClangTidyPerformanceNoIntToPtr
#include "../inc/PathInput.hpp"

PathInput::PathInput(const HWND parentHwnd, const int id) {
    _hwndEdit = CreateWindow(
        L"EDIT",
        L"",
        WS_VISIBLE | WS_CHILD | WS_BORDER | ES_AUTOHSCROLL,
        25, 58, 225, 22,
        parentHwnd,
        reinterpret_cast<HMENU>(id),
        nullptr,
        nullptr
    );
}

std::wstring PathInput::getPath() const {
    wchar_t buffer[MAXUINT16];
    GetWindowText(_hwndEdit, buffer, MAXUINT16);
    return {buffer};
}
