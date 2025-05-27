#pragma once
#include <string>
#include <windows.h>

void UpdateStringWithInput(std::wstring& s, const TCHAR c, const size_t count);

void UpdateRowBottomPositions(const HWND hWnd);

void UpdateCurrentCell(const int xPos, const int yPos);

void RedrawTable(const HDC hdc);