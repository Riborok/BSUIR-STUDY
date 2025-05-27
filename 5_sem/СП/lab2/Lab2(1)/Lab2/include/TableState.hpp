#pragma once

#include <string>
#include <windows.h>

#include "WindowConfig.hpp"

extern SIZE windowSize;
extern HFONT font;
extern POINT currentCell;
extern std::wstring tableText[TABLE_ROWS][TABLE_COLS];
extern double rowBottomPositions[TABLE_ROWS];
extern double columnWidth;