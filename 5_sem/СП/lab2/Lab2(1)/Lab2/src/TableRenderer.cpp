#include "../include/TableRenderer.hpp"
#include "../include/TableState.hpp"

#include <vector>
#include <windowsx.h>

void _calculateRowHeightsAndDefaults(const HDC hdc, int& defaultRowQuantity, int& remainingHeightForDefault, int rowHeights[]);
int _calculateDefaultRowHeight();
int _calculateRowHeight(const HDC hdc, const std::wstring* textRow);
void _updateRowBottomPositionsInternal(const int rowHeights[], const int defaultRowQuantity, const int remainingHeightForDefault);
double _calculateAdjustedDefaultRowHeight(const int remainingHeightForDefault, const int defaultRowQuantity);
double _calculateDefaultRowPosition(const double adjustedDefaultRowHeight, const int rowIndex);
double _calculateCustomRowPosition(const int rowHeight, const int rowIndex);

void DrawTextWithFont(const HDC hdc, const std::wstring& text, RECT* rect, const UINT format);

int _calculateColumn(const int xPos);
int _binarySearchRow(const int yPos);

void _setupBufferHdc(const HDC hdc, HDC& bufferHdc, HBITMAP& bufferBitmap, HBITMAP& oldBitmap);
void _fillBackground(const HDC hdc);
void _createGraphicsObjects(HPEN& pen, HPEN& dedicatedPen, HBRUSH& brushHollow);
void _selectGraphicsObjects(const HDC bufferHdc, const HPEN pen, const HBRUSH brush, HPEN& oldPen, HBRUSH& oldBrush);
void _drawTable(const HDC hdc, const HPEN dedicatedPen);
void _drawCell(const HDC hdc, const int row, const int col);
RECT _calculateCellRect(const int row, const int col);
void _copyToHdc(const HDC hdc, const HDC bufferHdc);
void _cleanupGraphicsObjects(const HDC bufferHdc, const HBITMAP oldBitmap, const HBITMAP bufferBitmap, const HPEN oldPen, const HBRUSH oldBrush, const HPEN dedicatedPen);

double _getRowTopPosition(const int rowIndex);
double _getRowBottomPosition(const int rowIndex);
double _getColumnLeftPosition(const int colIndex);
double _getColumnRightPosition(const int colIndex);

void UpdateStringWithInput(std::wstring& s, const TCHAR c, const size_t count) {
    if (c != VK_BACK) {
        s.append(count, c);
    } else if (!s.empty()){
        const int lengthToRemove = min(count, s.length());
        s.erase(s.length() - lengthToRemove, lengthToRemove);
    }
}

void UpdateRowBottomPositions(const HWND hWnd) {
    HDC hdc = GetDC(hWnd);
    
    int rowHeights[TABLE_ROWS];
    int defaultRowQuantity = 0;
    int remainingHeightForDefault = 0;
    
    _calculateRowHeightsAndDefaults(hdc, defaultRowQuantity, remainingHeightForDefault, rowHeights);
    _updateRowBottomPositionsInternal(rowHeights, defaultRowQuantity, remainingHeightForDefault);
    ReleaseDC(hWnd, hdc);
}

void _calculateRowHeightsAndDefaults(const HDC hdc, int& defaultRowQuantity, int& remainingHeightForDefault, int rowHeights[]) {
    const int defaultRowHeight = _calculateDefaultRowHeight();
    defaultRowQuantity = 0;
    remainingHeightForDefault = windowSize.cy;
    
    for (int i = 0; i < TABLE_ROWS; i++) {
        rowHeights[i] = _calculateRowHeight(hdc, tableText[i]);
        if (defaultRowHeight >= rowHeights[i]) {
            defaultRowQuantity++;
        } else {
            remainingHeightForDefault -= rowHeights[i];
        }
    }
}

int _calculateDefaultRowHeight() {
    return static_cast<int>(static_cast<double>(windowSize.cy) / TABLE_ROWS);
}

int _calculateRowHeight(const HDC hdc, const std::wstring* textRow) {
    int result = 0;
    for (int col = 0; col < TABLE_COLS; col++) {
        RECT textRect = { 0, 0, static_cast<int>(columnWidth), 0 };
        DrawTextWithFont(hdc, textRow[col], &textRect, DT_CALCRECT | DT_WORDBREAK | DT_EDITCONTROL);
        result = max(textRect.bottom, result);
    }
    return result;
}

void _updateRowBottomPositionsInternal(const int rowHeights[], const int defaultRowQuantity, const int remainingHeightForDefault) {
    const int defaultRowHeight = _calculateDefaultRowHeight();
    const double adjustedDefaultRowHeight = _calculateAdjustedDefaultRowHeight(remainingHeightForDefault, defaultRowQuantity);

    for (int i = 0; i < TABLE_ROWS; i++) {
        if (defaultRowHeight >= rowHeights[i]) {
            rowBottomPositions[i] = _calculateDefaultRowPosition(adjustedDefaultRowHeight, i);
        } else {
            rowBottomPositions[i] = _calculateCustomRowPosition(rowHeights[i], i);
        }
    }
}

double _calculateAdjustedDefaultRowHeight(const int remainingHeightForDefault, const int defaultRowQuantity) {
    return static_cast<double>(remainingHeightForDefault) / static_cast<double>(defaultRowQuantity);
}

double _calculateDefaultRowPosition(const double adjustedDefaultRowHeight, const int rowIndex) {
    return adjustedDefaultRowHeight + _getRowTopPosition(rowIndex);
}

double _calculateCustomRowPosition(const int rowHeight, const int rowIndex) {
    return static_cast<double>(rowHeight) + _getRowTopPosition(rowIndex);
}

void UpdateCurrentCell(const int xPos, const int yPos) {
    currentCell.x = _calculateColumn(xPos);
    currentCell.y = _binarySearchRow(yPos);
}

int _calculateColumn(const int xPos) {
    return xPos / static_cast<int>(columnWidth);
}

int _binarySearchRow(const int yPos) {
    int left = 0;
    int right = TABLE_ROWS - 1;
    while (left <= right) {
        const int mid = (left + right) / 2;
        if ((mid == 0 || yPos >= rowBottomPositions[mid - 1]) && yPos < rowBottomPositions[mid]) {
            return mid;
        }
        else if (yPos >= rowBottomPositions[mid]) {
            left = mid + 1;
        } 
        else {
            right = mid - 1;
        }
    }
    return -1;
}

void RedrawTable(const HDC hdc) {
    HDC bufferHdc;
    HBITMAP bufferBitmap;
    HBITMAP oldBitmap;
    HPEN pen;
    HPEN dedicatedPen;
    HBRUSH brush;
    HPEN oldPen;
    HBRUSH oldBrush;

    _setupBufferHdc(hdc, bufferHdc, bufferBitmap, oldBitmap);
    _fillBackground(bufferHdc);
    _createGraphicsObjects(pen, dedicatedPen, brush);
    _selectGraphicsObjects(bufferHdc, pen, brush, oldPen, oldBrush);
    _drawTable(bufferHdc, dedicatedPen);
    _copyToHdc(hdc, bufferHdc);
    _cleanupGraphicsObjects(bufferHdc, oldBitmap, bufferBitmap, oldPen, oldBrush, dedicatedPen);
}

void _setupBufferHdc(const HDC hdc, HDC& bufferHdc, HBITMAP& bufferBitmap, HBITMAP& oldBitmap) {
    bufferHdc = CreateCompatibleDC(hdc);
    bufferBitmap = CreateCompatibleBitmap(hdc, windowSize.cx, windowSize.cy);
    oldBitmap = static_cast<HBITMAP>(SelectObject(bufferHdc, bufferBitmap));
}

void _fillBackground(const HDC hdc) {
    const RECT rect = { 0, 0, windowSize.cx, windowSize.cy };
    FillRect(hdc, &rect, GetStockBrush(WHITE_BRUSH));
}

void _createGraphicsObjects(HPEN& pen, HPEN& dedicatedPen, HBRUSH& brushHollow) {
    pen = GetStockPen(BLACK_PEN);
    dedicatedPen = CreatePen(PS_SOLID, 2, RGB(244,169,0));
    brushHollow = GetStockBrush(HOLLOW_BRUSH);
}

void _selectGraphicsObjects(const HDC bufferHdc, const HPEN pen, const HBRUSH brush, HPEN& oldPen, HBRUSH& oldBrush) {
    oldPen = static_cast<HPEN>(SelectObject(bufferHdc, pen));
    oldBrush = static_cast<HBRUSH>(SelectObject(bufferHdc, brush));
}

void _drawTable(const HDC hdc, const HPEN dedicatedPen) {
    for (int row = 0; row < TABLE_ROWS; row++) {
        for (int col = 0; col < TABLE_COLS; col++) {
            _drawCell(hdc, row, col);
        }
    }
    const HPEN oldPen = static_cast<HPEN>(SelectObject(hdc, dedicatedPen));
    _drawCell(hdc, currentCell.y, currentCell.x);
    SelectObject(hdc, oldPen);
}

void _drawCell(const HDC hdc, const int row, const int col) {
    RECT cellRect = _calculateCellRect(row, col);
    DrawTextWithFont(hdc, tableText[row][col], &cellRect, DT_WORDBREAK | DT_EDITCONTROL);
    Rectangle(hdc, cellRect.left, cellRect.top, cellRect.right, cellRect.bottom);
}

RECT _calculateCellRect(const int row, const int col) {
    return { 
        static_cast<int>(_getColumnLeftPosition(col)), 
        static_cast<int>(_getRowTopPosition(row)), 
        static_cast<int>(_getColumnRightPosition(col)), 
        static_cast<int>(_getRowBottomPosition(row))
    };
}

void _copyToHdc(const HDC hdc, const HDC bufferHdc) {
    BitBlt(hdc, 0, 0, windowSize.cx, windowSize.cy, bufferHdc, 0, 0, SRCCOPY);
}

void _cleanupGraphicsObjects(const HDC bufferHdc, const HBITMAP oldBitmap, const HBITMAP bufferBitmap, const HPEN oldPen, const HBRUSH oldBrush, const HPEN dedicatedPen) {
    SelectObject(bufferHdc, oldBitmap);
    SelectObject(bufferHdc, oldPen);
    SelectObject(bufferHdc, oldBrush);
    DeleteObject(bufferBitmap);
    DeleteObject(dedicatedPen);
    DeleteDC(bufferHdc);
}

double _getRowTopPosition(const int rowIndex) {
    return rowIndex == 0 ? 0 : rowBottomPositions[rowIndex - 1];
}

double _getRowBottomPosition(const int rowIndex) {
    return rowBottomPositions[rowIndex];
}

double _getColumnLeftPosition(const int colIndex) {
    return static_cast<double>(colIndex) * columnWidth;
}

double _getColumnRightPosition(const int colIndex) {
    return static_cast<double>(colIndex + 1) * columnWidth;
}

void DrawTextWithFont(const HDC hdc, const std::wstring& text, RECT* rect, const UINT format) {
    const HFONT oldFont = static_cast<HFONT>(SelectObject(hdc, font));
    DrawText(hdc, text.c_str(), -1, rect, format);
    SelectObject(hdc, oldFont);
}