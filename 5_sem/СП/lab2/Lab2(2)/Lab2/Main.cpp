#include <windows.h>
#include <windowsx.h>
#include <string>

constexpr double M_PI = 3.14159265358979323846;

LRESULT CALLBACK WindowProc(HWND, UINT, WPARAM, LPARAM);
void UpdateTextWithInput(const TCHAR c, const size_t count);
void DrawTextInCircle(const HDC hdc);
void FillBackground(const HDC hdc);
SIZE CalculateTextWidth(const HDC hdc);
SIZE CalculateStrSize(const HDC hdc, const std::wstring& str, const int cchText);
HFONT CreateRotatedFont(const double angle);

constexpr int WINDOW_WIDTH = 1000;
constexpr int WINDOW_HEIGHT = 600;
constexpr int MIN_LETTER_DISTANCE = 30; 

std::wstring text;
SIZE windowSize = {WINDOW_WIDTH, WINDOW_HEIGHT};

LOGFONT lf = {
    20,
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
    L"Times New Roman"
};

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
    
    MSG msg;
    while (GetMessage(&msg, nullptr, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return static_cast<int>(msg.wParam);
}

LRESULT CALLBACK WindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
    switch (msg) {
    case WM_PAINT: {
        PAINTSTRUCT ps;
        const HDC hdc = BeginPaint(hWnd, &ps);
        DrawTextInCircle(hdc);
        EndPaint(hWnd, &ps);
        break;
    }

    case WM_CHAR: {
        const TCHAR c = static_cast<TCHAR>(wParam);
        const int count = LOWORD(lParam);
        UpdateTextWithInput(c, count);
        InvalidateRect(hWnd, nullptr, TRUE); 
        break;
    }
        
    case WM_SIZE: {
        windowSize.cx = LOWORD(lParam);
        windowSize.cy = HIWORD(lParam);
        InvalidateRect(hWnd, nullptr, FALSE);
        break;
    }

    case WM_DESTROY:
        PostQuitMessage(0);
        break;

    default:
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }

    return 0;
}

void UpdateTextWithInput(const TCHAR c, const size_t count) {
    if (c != VK_BACK) {
        text.append(count, c);
    } else if (!text.empty()){
        const int lengthToRemove = min(count, text.length());
        text.erase(text.length() - lengthToRemove, lengthToRemove);
    }
}

void DrawTextInCircle(const HDC hdc) {
    FillBackground(hdc);
    const int length = static_cast<int>(text.length());
    if (length == 0) {
        return;
    }

    const HFONT hFont = CreateRotatedFont(0);
    const HFONT hOldFont = static_cast<HFONT>(SelectObject(hdc, hFont));
    
    const SIZE textSize = CalculateTextWidth(hdc);
    const int perimeter = textSize.cx + MIN_LETTER_DISTANCE * (length - 1);
    const double radius = perimeter / (2 * M_PI);
    const double angleStep = 2 * M_PI / length;
    
    const int circleRadius = static_cast<int>(radius + textSize.cy);
    const int centerX = windowSize.cx / 2;
    const int centerY = windowSize.cy / 2;
    const int left = centerX - circleRadius;
    const int top = centerY - circleRadius;
    const int right = centerX + circleRadius;
    const int bottom = centerY + circleRadius;
    Ellipse(hdc, left, top, right, bottom);

    for (size_t i = 0; i < length; i++) {
        const double angle = i * angleStep - M_PI / 2;
        const int textX = centerX + static_cast<int>(radius * cos(angle));
        const int textY = centerY + static_cast<int>(radius * sin(angle));
        const HFONT hFont = CreateRotatedFont(angle);
        const HFONT hOldFont = static_cast<HFONT>(SelectObject(hdc, hFont));
        TextOut(hdc, textX, textY, &text[i], 1);
        SelectObject(hdc, hOldFont);
        DeleteObject(hFont);
    }

    SelectObject(hdc, hOldFont);
    DeleteObject(hFont);
}

void FillBackground(const HDC hdc) {
    const RECT rect = { 0, 0, windowSize.cx, windowSize.cy };
    FillRect(hdc, &rect, GetStockBrush(WHITE_BRUSH));
}

SIZE CalculateTextWidth(const HDC hdc) {
    return CalculateStrSize(hdc, text, -1);
}

SIZE CalculateStrSize(const HDC hdc, const std::wstring& str, const int cchText) {
    RECT textRect = { 0, 0, 0, 0 };
    DrawText(hdc, str.c_str(), cchText, &textRect, DT_CALCRECT);
    return {
        textRect.right - textRect.left,
        textRect.bottom - textRect.top
    };
}

HFONT CreateRotatedFont(const double angle) {
    double angleInDegrees = angle * (180.0 / M_PI);
    lf.lfEscapement = -(int)((angleInDegrees + 90) * 10);
    return CreateFontIndirect(&lf);
}