// ReSharper disable CppClangTidyPerformanceNoIntToPtr
#include <windows.h>

#include "Controls.hpp"
#include "inc/PathInput.hpp"
#include "inc/RegistryRootKeySelector.hpp"
#include "inc/RegistryUtils.hpp"

static constexpr int ID_KEY_INFO = 1;
static constexpr int ID_CREATE_KEY = 2;
static constexpr int ID_DELETE_KEY = 3;
static constexpr int ID_TRACK_KEY = 4;
static constexpr int ID_SET_VALUE = 5;
static constexpr int ID_DELETE_VALUE = 6;
static constexpr int ID_GET_VALUE = 7;
static constexpr int ID_PATH_CHANGE = 8;

Controls* controls = nullptr;

Controls* createControls(const HWND hwnd);
LRESULT CALLBACK windowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);
void showAvailableFieldNames(const HWND hwnd);
void showMessage(const HWND hwnd, const LPCWSTR message);
void ignoreMessage(const HWND hwnd, const LPCWSTR message);

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    WNDCLASSEX wc = {};
    wc.cbSize = sizeof(WNDCLASSEX);
    wc.lpfnWndProc = windowProc;
    wc.hInstance = hInstance;
    wc.lpszClassName = L"Lab3";
    RegisterClassEx(&wc);

    const HWND hwnd = CreateWindowEx(
        0, L"Lab3", L"Lab3",
        WS_SYSMENU | WS_CAPTION | WS_VISIBLE, CW_USEDEFAULT, CW_USEDEFAULT,
        291, 525, nullptr, nullptr, hInstance, nullptr
    );
    controls = createControls(hwnd);
    showAvailableFieldNames(hwnd);
    MSG msg;
    while (GetMessage(&msg, nullptr, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    delete controls;
    return msg.wParam;
}

Controls* createControls(const HWND hwnd) {
    const auto result = new Controls(hwnd, ID_PATH_CHANGE);
    
    CreateWindow(L"BUTTON", L"KEY INFORMATION", WS_VISIBLE | WS_CHILD, 25, 260, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_KEY_INFO), nullptr, nullptr);
    CreateWindow(L"BUTTON", L"CREATE KEY", WS_VISIBLE | WS_CHILD, 25, 292, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_CREATE_KEY), nullptr, nullptr);
    CreateWindow(L"BUTTON", L"DELETE KEY", WS_VISIBLE | WS_CHILD, 25, 324, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_DELETE_KEY), nullptr, nullptr);
    CreateWindow(L"BUTTON", L"TRACK KEY", WS_VISIBLE | WS_CHILD, 25, 356, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_TRACK_KEY), nullptr, nullptr);
    CreateWindow(L"BUTTON", L"SET VALUE", WS_VISIBLE | WS_CHILD, 25, 388, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_SET_VALUE), nullptr, nullptr);
    CreateWindow(L"BUTTON", L"DELETE VALUE", WS_VISIBLE | WS_CHILD, 25, 420, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_DELETE_VALUE), nullptr, nullptr);
    CreateWindow(L"BUTTON", L"GET VALUE", WS_VISIBLE | WS_CHILD, 25, 452, 225, 22, hwnd,
        reinterpret_cast<HMENU>(ID_GET_VALUE), nullptr, nullptr);
    
    return result;
}



LRESULT CALLBACK windowProc(const HWND hwnd, const UINT uMsg, const WPARAM wParam, const LPARAM lParam) {
    switch (uMsg) {
        case WM_DESTROY:
            PostQuitMessage(0);
        break;

        case WM_COMMAND: {
            const int wmId = LOWORD(wParam);
            const int wmEvent = HIWORD(wParam);
            switch (wmId) {
                case ID_KEY_INFO:
                    RegistryUtils::displayKeyInfo(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                        controls->pathInput.getPath().c_str(),
                        showMessage);
                break;
                    
                case ID_CREATE_KEY:
                    RegistryUtils::createRegistryKey(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                        controls->pathInput.getPath().c_str(),
                        showMessage);
                break;
                    
                case ID_DELETE_KEY:
                    RegistryUtils::deleteRegistryKey(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                        controls->pathInput.getPath().c_str(),
                        showMessage);
                break;
                    
                case ID_TRACK_KEY:
                    RegistryUtils::trackKey(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                    controls->pathInput.getPath().c_str(),
                    showMessage);
                break;
                    
                case ID_SET_VALUE:
                    RegistryUtils::setRegistryValue(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                        controls->pathInput.getPath().c_str(),
                        controls->keyValueEditor.getFieldNameText(),
                        controls->keyValueEditor.getValueText(),
                        controls->keyValueEditor.getSelectedRegType(),
                        showMessage
                    );
                break;

                case ID_DELETE_VALUE:
                    RegistryUtils::deleteRegistryValue(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                        controls->pathInput.getPath().c_str(),
                        controls->keyValueEditor.getFieldNameText(),
                        showMessage);
                break;

                case ID_GET_VALUE: {
                    const auto valueAndType = RegistryUtils::getRegistryValue(hwnd,
                        controls->branchSelector.getSelectedRootKey(),
                        controls->pathInput.getPath().c_str(),
                        controls->keyValueEditor.getFieldNameText(),
                        showMessage);
                    controls->keyValueEditor.setValueText(valueAndType.first.c_str());
                    controls->keyValueEditor.setRegType(valueAndType.second);
                }
                break;
                
                case ID_PATH_CHANGE:
                    if (wmEvent == EN_KILLFOCUS || wmEvent == CBN_SELCHANGE) {
                        showAvailableFieldNames(hwnd);
                    }
                break;
            }
        }
        break;

        default:
            return DefWindowProc(hwnd, uMsg, wParam, lParam);
    }

    return 0;
}

void showAvailableFieldNames(const HWND hwnd) {
    const auto fieldNames = RegistryUtils::getFieldNames(hwnd,
    controls->branchSelector.getSelectedRootKey(),
    controls->pathInput.getPath().c_str(),
    ignoreMessage);
    controls->keyValueEditor.addAvailableFieldNames(fieldNames);
}

void showMessage(const HWND hwnd, const LPCWSTR message) {
    MessageBox(hwnd, message, L"", MB_OK | MB_ICONINFORMATION);
}

void ignoreMessage(const HWND hwnd, const LPCWSTR message) {
    
}