#include <windows.h>
#include <math.h>

#define M_PI 3.14159265358979323846

#define ID_EXIT 4242

ACCEL accelerator_table[] = {
    { FCONTROL | FVIRTKEY, 'Q', ID_EXIT }, // CTRL + Q
    { FVIRTKEY, VK_ESCAPE, ID_EXIT }, // ESCAPE
};

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
void DrawRotatedRectangle(HDC hdc);
POINT RotatePointAroundTarget(POINT vector, POINT target_point, const double sina, const double cosa);
void HandleMovement(WPARAM key);
void HandleMouseWheel(int delta, bool is_alt_pressed);

int rect_x = 5;
int rect_y = 5;
int rect_width = 100;
int rect_height = 100;
double rect_angle = 0.0;
int speed = 10;
double angular_speed_degrees = 10.0;
double angular_speed_radians = angular_speed_degrees * M_PI / 180.0;

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    WNDCLASSEX wc = {};
    wc.cbSize = sizeof(WNDCLASSEX);
    wc.lpfnWndProc = WindowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"Lab1";
    RegisterClassEx(&wc);
    
    HWND hwnd = CreateWindowEx(
        0, L"Lab1", L"MAIN",
        WS_OVERLAPPEDWINDOW | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT,
        1000, 600, NULL, NULL, hInstance, NULL
    );
    
    HACCEL hAccelerator_table = CreateAcceleratorTable(accelerator_table, ARRAYSIZE(accelerator_table));
    MSG msg;
    while (GetMessage(&msg, 0, 0, 0)) {
        if (!TranslateAccelerator(hwnd, hAccelerator_table, &msg)) {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    DestroyAcceleratorTable(hAccelerator_table);
    return msg.wParam;
}

LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
    switch (uMsg) {
        case WM_DESTROY:
            PostQuitMessage(0);
        break;
        
        case WM_PAINT: {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hwnd, &ps);
            FillRect(hdc, &ps.rcPaint, (HBRUSH)(COLOR_WINDOW + 1));
            DrawRotatedRectangle(hdc);
            EndPaint(hwnd, &ps);
        }
        break;

        case WM_COMMAND:
        if (LOWORD(wParam) == ID_EXIT) {
            PostQuitMessage(0);
        }
        break;

        case WM_KEYDOWN: {
            HandleMovement(wParam);
            InvalidateRect(hwnd, NULL, TRUE);
        }
        break;

        case WM_MOUSEWHEEL: {
            int delta = GET_WHEEL_DELTA_WPARAM(wParam);
            bool is_alt_pressed = GetAsyncKeyState(VK_MENU);
            HandleMouseWheel(delta, is_alt_pressed);
            InvalidateRect(hwnd, NULL, TRUE);
        }
        break;

        default:
            return DefWindowProc(hwnd, uMsg, wParam, lParam);
    }

    return 0;
}

void FillRotatedRectangle(HDC hdc, POINT points[], int point_count, COLORREF color) {
    HBRUSH hBrush = CreateSolidBrush(color);
    HBRUSH oldBrush = (HBRUSH)SelectObject(hdc, hBrush);

    Polygon(hdc, points, point_count);

    SelectObject(hdc, oldBrush);
    DeleteObject(hBrush);
}

void DrawRotatedRectangle(HDC hdc) {
    constexpr int point_count = 4;
    
    double sina = sin(rect_angle);
    double cosa = cos(rect_angle);
    POINT target_point = { rect_x + rect_width / 2, rect_y + rect_height / 2 };
    
    POINT points[point_count] = {
        { rect_x, rect_y },
        { rect_x + rect_width, rect_y },
        { rect_x + rect_width, rect_y + rect_height },
        { rect_x, rect_y + rect_height }
    };
    
    for (int i = 0; i < point_count; i++) {
        points[i] = RotatePointAroundTarget(points[i], target_point, sina, cosa);
    }
    
    FillRotatedRectangle(hdc, points, point_count, RGB(100, 200, 300));
}

POINT RotatePointAroundTarget(POINT vector, POINT target_point, const double sina, const double cosa) {
    const double delta_x = vector.x - target_point.x;
    const double delta_y = vector.y - target_point.y;

    POINT result;
    result.x = target_point.x + delta_x * cosa - delta_y * sina;
    result.y = target_point.y + delta_x * sina + delta_y * cosa;
    return result;
}

void HandleMovement(WPARAM key) {
    switch (key) {
        case VK_UP:
        case 'W':
            rect_y -= speed;
            break;
        case VK_DOWN:
        case 'S':
            rect_y += speed;
            break;
        case VK_LEFT:
        case 'A':
            rect_x -= speed;
            break;
        case VK_RIGHT:
        case 'D':
            rect_x += speed;
            break;
        case 'E':
            rect_angle += angular_speed_radians;
            break;
        case 'Q':
            rect_angle -= angular_speed_radians;
            break;
    }
}

void HandleMouseWheel(int delta, bool is_alt_pressed) {
    if (is_alt_pressed) {
        rect_x += delta > 0 ? speed : -speed;
    } else {
        rect_y += delta > 0 ? -speed : speed;
    }
}