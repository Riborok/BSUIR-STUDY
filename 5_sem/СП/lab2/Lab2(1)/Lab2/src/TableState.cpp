#include "../include/TableState.hpp"

SIZE windowSize = {WINDOW_WIDTH, WINDOW_HEIGHT};
HFONT font = nullptr;
POINT currentCell = {};
std::wstring tableText[TABLE_ROWS][TABLE_COLS];
double rowBottomPositions[TABLE_ROWS] = {};
double columnWidth = 0;