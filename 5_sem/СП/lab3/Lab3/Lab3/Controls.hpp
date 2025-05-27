#pragma once

#include "inc/KeyValueEditor.hpp"
#include "inc/PathInput.hpp"
#include "inc/RegistryRootKeySelector.hpp"

struct Controls {
    RegistryRootKeySelector branchSelector;
    PathInput pathInput;
    KeyValueEditor keyValueEditor;

    explicit Controls(const HWND parentHwnd, const int pathId):
        branchSelector(parentHwnd, pathId),
        pathInput(parentHwnd, pathId),
        keyValueEditor(parentHwnd)
    { }
};
