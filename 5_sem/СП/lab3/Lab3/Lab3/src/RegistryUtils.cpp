#include "../inc/RegistryUtils.hpp"

#include <string>
#include <vector>

void RegistryUtils::displayKeyInfo(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of) {
    HKEY keyHandle;
    LONG result = RegOpenKeyEx(rootKey, subKey, 0, KEY_QUERY_VALUE, &keyHandle);
    
    if (result == ERROR_SUCCESS) {
        DWORD numberOfSubKeys = 0;
        DWORD maxSubKeyLength = 0;
        DWORD numberOfValues = 0;
        DWORD maxValueNameLength = 0;
        DWORD maxValueDataLength = 0;
        DWORD securityDescriptor = 0;
        FILETIME lastWriteTime;

        result = RegQueryInfoKey(
            keyHandle,
            nullptr,
            nullptr,
            nullptr,
            &numberOfSubKeys,
            &maxSubKeyLength,
            nullptr,
            &numberOfValues,
            &maxValueNameLength,
            &maxValueDataLength,
            &securityDescriptor,
            &lastWriteTime
        );

        if (result == ERROR_SUCCESS) {
            SYSTEMTIME systemTime;
            FileTimeToSystemTime(&lastWriteTime, &systemTime);
            
            const std::wstring message = 
                std::wstring(L"Registry Key Information:\n") +
                L"Number of subkeys: " + std::to_wstring(numberOfSubKeys) + L"\n" +
                L"Max subkey name length: " + std::to_wstring(maxSubKeyLength) + L"\n" +
                L"Number of values: " + std::to_wstring(numberOfValues) + L"\n" +
                L"Max value name length: " + std::to_wstring(maxValueNameLength) + L"\n" +
                L"Max value data size: " + std::to_wstring(maxValueDataLength) + L"\n" +
                L"Last write time: " +
                    std::to_wstring(systemTime.wDay) + L"/" +
                    std::to_wstring(systemTime.wMonth) + L"/" +
                    std::to_wstring(systemTime.wYear) + L" " +
                    std::to_wstring(systemTime.wHour) + L":" +
                    std::to_wstring(systemTime.wMinute) + L":" +
                    std::to_wstring(systemTime.wSecond) + L"\n";
            
            of(hwnd, message.c_str());
        } else {
            of(hwnd, (L"Failed to get key info. Error code " + std::to_wstring(result)).c_str());
        }
        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to open registry key. Error code " + std::to_wstring(result)).c_str());
    }
}

void RegistryUtils::createRegistryKey(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of) {
    HKEY keyHandle;
    DWORD disposition;

    const LONG result = RegCreateKeyEx(rootKey, subKey, 0, nullptr, REG_OPTION_NON_VOLATILE,
        KEY_ALL_ACCESS, nullptr, &keyHandle, &disposition);

    if (result == ERROR_SUCCESS) {
        if (disposition == REG_CREATED_NEW_KEY) {
            of(hwnd, L"Key created successfully.");
        } else {
            of(hwnd, L"Key already exists.");
        }
        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to create key. Error code " + std::to_wstring(result)).c_str());
    }
}

void RegistryUtils::deleteRegistryKey(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of) {
    const LONG result = RegDeleteKey(rootKey, subKey);

    if (result == ERROR_SUCCESS) {
        of(hwnd, L"Key deleted successfully.");
    } else {
        of(hwnd, (L"Failed to delete key. Error code " + std::to_wstring(result)).c_str());
    }
}

void RegistryUtils::trackKey(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey, const OutputFunc& of) {
    HKEY keyHandle;

    LONG result = RegOpenKeyEx(rootKey, subKey, 0, KEY_NOTIFY, &keyHandle);
    
    if (result == ERROR_SUCCESS) {
        result = RegNotifyChangeKeyValue(keyHandle,
            TRUE,
            REG_NOTIFY_CHANGE_NAME | REG_NOTIFY_CHANGE_ATTRIBUTES | REG_NOTIFY_CHANGE_LAST_SET | REG_NOTIFY_CHANGE_SECURITY,
            nullptr,
            false
        );
        if (result == ERROR_SUCCESS) {
            of(hwnd, L"The registry key has been modified.");
        } else {
            of(hwnd, (L"Failed to track key. Error code " + std::to_wstring(result)).c_str());
        }
        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to open registry key. Error code " + std::to_wstring(result)).c_str());
    }
}

std::vector<BYTE> convertValueFromString(const std::wstring& valueData, const DWORD valueType) {
    std::vector<BYTE> dataVector;

    switch (valueType) {
        case REG_SZ: {
            dataVector.resize((valueData.size() + 1) * sizeof(wchar_t));
            memcpy(dataVector.data(), valueData.c_str(), dataVector.size());
            return dataVector;
        }

        case REG_DWORD: {
            const DWORD dwordValue = std::stoul(valueData);
            dataVector.resize(sizeof(DWORD));
            memcpy(dataVector.data(), &dwordValue, sizeof(DWORD));
            return dataVector;
        }

        case REG_QWORD: {
            const ULONGLONG qwordValue = std::stoull(valueData);
            dataVector.resize(sizeof(ULONGLONG));
            memcpy(dataVector.data(), &qwordValue, sizeof(ULONGLONG));
            return dataVector;
        }

        default:
            return dataVector;
    }
}

void RegistryUtils::setRegistryValue(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey,
        const std::wstring& valueName, const std::wstring& valueData, const DWORD valueType, const OutputFunc& of) {
    HKEY keyHandle;
    
    LONG result = RegOpenKeyEx(rootKey, subKey, 0, KEY_SET_VALUE, &keyHandle);
    
    if (result == ERROR_SUCCESS) {
        std::vector<BYTE> dataVector;
        try {
            dataVector = convertValueFromString(valueData, valueType);
        } catch (const std::exception&) {
            of(hwnd, L"Failed to convert value data");
            RegCloseKey(keyHandle);
            return;
        }

        result = RegSetValueEx(keyHandle, valueName.c_str(), 0, valueType,
            dataVector.data(), dataVector.size());

        if (result == ERROR_SUCCESS) {
            of(hwnd, L"Value was set successfully.");
        } else {
            of(hwnd, (L"Failed to set value. Error code " + std::to_wstring(result)).c_str());
        }

        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to open registry key. Error code " + std::to_wstring(result)).c_str());
    }
}

void RegistryUtils::deleteRegistryValue(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey,
        const std::wstring& valueName, const OutputFunc& of) {
    HKEY keyHandle;

    LONG result = RegOpenKeyEx(rootKey, subKey, 0, KEY_SET_VALUE, &keyHandle);
    
    if (result == ERROR_SUCCESS) {
        result = RegDeleteValue(keyHandle, valueName.c_str());

        if (result == ERROR_SUCCESS) {
            of(hwnd, L"Value deleted successfully.");
        } else {
            of(hwnd, (L"Failed to delete value. Error code " + std::to_wstring(result)).c_str());
        }

        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to open registry key. Error code " + std::to_wstring(result)).c_str());
    }
}

std::wstring convertValueToString(const std::vector<BYTE>& dataVector, const DWORD valueType) {
    switch (valueType) {
    case REG_SZ: {
        return {reinterpret_cast<const wchar_t*>(dataVector.data())};
    }

    case REG_DWORD: {
        DWORD dwordValue;
        memcpy(&dwordValue, dataVector.data(), sizeof(DWORD));
        return std::to_wstring(dwordValue);
    }

    case REG_QWORD: {
        ULONGLONG qwordValue;
        memcpy(&qwordValue, dataVector.data(), sizeof(ULONGLONG));
        return std::to_wstring(qwordValue);
    }

    default:
        return L"";
    }
}


std::pair<std::wstring, DWORD> RegistryUtils::getRegistryValue(const HWND hwnd, const HKEY rootKey, const LPCWSTR subKey,
        const std::wstring& valueName, const OutputFunc& of) {
    HKEY keyHandle;
    std::wstring value;
    DWORD valueType = 0;
    BYTE buffer[MAXUINT16];
    DWORD dataSize = sizeof(buffer);

    LONG result = RegOpenKeyEx(rootKey, subKey, 0, KEY_QUERY_VALUE, &keyHandle);
    
    if (result == ERROR_SUCCESS) {
        result = RegQueryValueEx(keyHandle, valueName.c_str(), nullptr, &valueType, buffer, &dataSize);
        if (result == ERROR_SUCCESS) {
            value = convertValueToString(std::vector<BYTE>{buffer, buffer + dataSize}, valueType);
        } else {
            of(hwnd, (L"Failed to retrieve value data. Error code " + std::to_wstring(result)).c_str());
        }

        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to open registry key. Error code " + std::to_wstring(result)).c_str());
    }

    return {value, valueType};
}

std::vector<std::wstring> RegistryUtils::getFieldNames(const HWND hwnd, const HKEY rootKey,
        const LPCWSTR subKey, const OutputFunc& of) {
    std::vector<std::wstring> fieldNames;
    
    HKEY keyHandle;
    LONG result = RegOpenKeyEx(rootKey, subKey, 0, KEY_READ, &keyHandle);

    if (result == ERROR_SUCCESS) {
        do {
            wchar_t valueName[MAXUINT16];
            DWORD valueNameSize = MAXUINT16;
            
            result = RegEnumValue(keyHandle, fieldNames.size(), valueName, &valueNameSize,
                nullptr, nullptr, nullptr, nullptr);

            if (result == ERROR_SUCCESS) {
                fieldNames.emplace_back(valueName, valueNameSize);
            } else if (result != ERROR_NO_MORE_ITEMS) {
                of(hwnd, (L"Error enumerating registry values. Error code " + std::to_wstring(result)).c_str());
                break;
            }
        } while (result != ERROR_NO_MORE_ITEMS);
        
        RegCloseKey(keyHandle);
    } else {
        of(hwnd, (L"Failed to open registry key. Error code " + std::to_wstring(result)).c_str());
    }
    return fieldNames;
}
