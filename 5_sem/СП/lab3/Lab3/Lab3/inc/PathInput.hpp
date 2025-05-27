#pragma once

#include <string>
#include <windows.h>

class PathInput {
    HWND _hwndEdit;
public:
    explicit PathInput(const HWND parentHwnd, const int id);
    std::wstring getPath() const;
};
