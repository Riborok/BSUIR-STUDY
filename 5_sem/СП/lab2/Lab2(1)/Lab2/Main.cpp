#include <windows.h>
#include <windowsx.h>
#include <string>
#include <vector>

#include "include/TableRenderer.hpp"
#include "include/TableState.hpp"
#include "include/WindowConfig.hpp"

LRESULT CALLBACK WindowProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam);

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    WNDCLASSEX wc = {};
    wc.cbSize = sizeof(WNDCLASSEX);
    wc.lpfnWndProc = WindowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"Lab2";
    RegisterClassEx(&wc);
    
    CreateWindowEx(
        0, L"Lab2", L"MAIN",
        WS_OVERLAPPEDWINDOW | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT,
        WINDOW_WIDTH, WINDOW_HEIGHT, nullptr, nullptr, hInstance, nullptr
    );

    font = CreateFont(
    28,
    0,
    0,
    0,
    FW_NORMAL,
    FALSE,
    FALSE,
    FALSE,
    DEFAULT_CHARSET,
    OUT_DEFAULT_PRECIS,
    CLIP_DEFAULT_PRECIS,
    DEFAULT_QUALITY,
    DEFAULT_PITCH | FF_SWISS,
    L"Times New Roman");
    MSG msg;
    while (GetMessage(&msg, nullptr, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    DeleteObject(font);

    return static_cast<int>(msg.wParam);
}

LRESULT CALLBACK WindowProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam) {
    switch (message) {
        
    case WM_PAINT: {
        PAINTSTRUCT ps;
        const HDC hdc = BeginPaint(hWnd, &ps);
        RedrawTable(hdc);
        EndPaint(hWnd, &ps);
        break;
    }

    case WM_CHAR: {
        const TCHAR c = static_cast<TCHAR>(wParam);
        const int count = LOWORD(lParam);
        std::wstring& str = tableText[currentCell.y][currentCell.x];
        UpdateStringWithInput(str, c, count);
        UpdateRowBottomPositions(hWnd);
        InvalidateRect(hWnd, nullptr, FALSE);
        break;
    }

    case WM_SIZE:{
        windowSize.cx = LOWORD(lParam);
        windowSize.cy = HIWORD(lParam);
        columnWidth = static_cast<double>(windowSize.cx) / TABLE_COLS;
        UpdateRowBottomPositions(hWnd);
        InvalidateRect(hWnd, nullptr, FALSE);
        break;
    }

    case WM_LBUTTONDOWN: {
        const int xPos = LOWORD(lParam);
        const int yPos = HIWORD(lParam);
        UpdateCurrentCell(xPos, yPos);
        InvalidateRect(hWnd, nullptr, FALSE);
        break;
    }

    case WM_DESTROY:
        PostQuitMessage(0);
        break;

    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    
    return 0;
}