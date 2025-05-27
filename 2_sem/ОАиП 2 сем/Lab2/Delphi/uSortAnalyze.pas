unit uSortAnalyze;

interface

type
  TArray = array [0..9999] of Integer;

function CalcSelectionPract(AArray: TArray; const AAmOfElements: Integer): Integer;
function CalcBinaryInsertionPract(AArray: TArray; const AAmOfElements: Integer): Integer;

function CalcSelectionTheor(const AAmOfElements: Integer): Integer;
function CalcBinaryInsertionTheor(const AAmOfElements: Integer): Integer;
implementation

// This function calculates the practical complexity of sorting an array of
// integers using binary insertion sort. The function takes an array of integers
// and the number of elements in the array as input parameters. The function
// returns the number of comparisons performed during sorting.
function CalcBinaryInsertionPract(AArray: TArray; const AAmOfElements: Integer): Integer;
var
  I, J, L, R, M, Temp: Integer;
  // I, J - loop counter
  // L - left index of the unsorted portion of the array
  // R - right index of the unsorted portion of the array
  // M - middle index used for binary search
  // Temp - temporary variable used for swapping elements during sorting
begin
  // Initialize the number of comparisons to zero
  Result:= 0;

  // Iterate over each element in the array
  for I := 1 to AAmOfElements - 1 do
  begin
    Temp := AArray[I];
    L := 0;
    R := I - 1;

    // Perform binary search until the left and right indices overlap
    while L <= R do
    begin
      Inc(Result);
      M := (L + R) div 2;
      if Temp < AArray[M] then
        R := M - 1
      else
        L := M + 1;
    end;

    // Shift the elements to the right to make room for the current
    for J := I - 1 downto L do
      AArray[J + 1] := AArray[J];
    AArray[L] := Temp;
  end;
end;


// This function calculates the practical complexity of sorting an array of
// integers using selection sort. The function takes an array of integers
// and the number of elements in the array as input parameters. The function
// returns the number of comparisons performed during sorting.
function CalcSelectionPract(AArray: TArray; const AAmOfElements: Integer): Integer;
var
  I, J, MinIndex, Temp: Integer;
  // I, J - loop counter
  // MinIndex - index of the current minimum element
  // Temp - temporary variable used for swapping elements during sorting
begin
  // Initialize the number of comparisons to zero
  Result := 0;

  // For each element in the array except the last one
  for I := 0 to AAmOfElements - 2 do
  begin
    MinIndex := I;

    // Find the minimum element in the unsorted portion of the array
    for J := I + 1 to AAmOfElements - 1 do
    begin
      Inc(Result);
      if AArray[J] < AArray[MinIndex] then
        MinIndex := J;
    end;

    // Swap the minimum element with the current element
    Temp := AArray[I];
    AArray[I] := AArray[MinIndex];
    AArray[MinIndex] := Temp;
  end;
end;

// This function calculates the theoretical number of comparisons needed
// for selection sort algorithm to sort an array of AAmOfElements elements.
// It uses the formula (n^2 - n) / 2 to calculate the number of comparisons
function CalcSelectionTheor(const AAmOfElements: Integer): Integer;
begin
  Result := Round((sqr(AAmOfElements) - AAmOfElements) / 2);
end;

// This function calculates the theoretical number of comparisons needed
// for binary insertion sort algorithm to sort an array of AAmOfElements elements.
// It uses the formula n*(log2(n)) / 2 to calculate the number of comparisons
function CalcBinaryInsertionTheor(const AAmOfElements: Integer): Integer;
begin
  Result := Round((AamOfElements * (ln(AAmOfElements) / ln(2)))) - AAmOfElements;
end;

end.
