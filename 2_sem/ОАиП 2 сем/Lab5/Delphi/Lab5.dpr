program Lab5;
{
 Output N lines of Pascal's triangle
}

uses
  System.SysUtils;

type
  TMatrix = array of array of Integer;

// Calculate AAmount lines of Pascal's triangle
function CalculatePascalTriangle(const AAmount: Integer):TMatrix;
var
  I, J: Integer;
  // I, J - loop counter
begin
  // Set length for Result
  SetLength(Result, AAmount);

  // Calculate Pascal's triangle
  for I := 0 to AAmount do
  begin
    SetLength(Result[I], I + 1);

    // First element in each row is always 1
    Result[I, 0] := 1;

    // Last element in each row is always 1
    Result[I, I] := 1;

    // Calculate each element in the row based on the values above it
    for J := 1 to I - 1 do
      Result[I, J] := Result[I-1, J-1] +
        Result[I-1, J];
  end;
end;

// Outputs the matrix to a file in the desired format
procedure OutputInFile(const AOutput: TextFile; const AMatrix: TMatrix);
var
  MaxLengthNum: Integer;
  i, j: Integer;
  numStr: string;
begin
  Reset(AOutput);
  Rewrite(AOutput);

  // Calculate the maximum length of a number, taking into account additional padding
  MaxLengthNum := Length(IntToStr(aMatrix[High(AMatrix), (High(AMatrix) + 1) shr 1]));

  for i := Low(AMatrix) to High(AMatrix) do
  begin
    Write(AOutput, StringOfChar(' ', High(AMatrix) - i + MaxLengthNum * (High(AMatrix) - i)));
    for j := Low(AMatrix[i]) to High(AMatrix[i]) do
    begin
      // Convert the current number to a string
      numStr := IntToStr(AMatrix[i][j]);

      // Add padding spaces to align the number to the right
      numStr := StringOfChar(' ', MaxLengthNum - Length(numStr)) + numStr;

      // Write the formatted number and a space to the output file
      Write(AOutput, numStr, ' ');
    end;

    // Write a newline character to move to the next line in the output file
    WriteLn(AOutput);
  end;

end;

procedure WriteToFile(const AMatrix: TMatrix);
var
   Output: TextFile;
   // Output - output text file where the Pascal's triangle will be written
begin
  AssignFile(Output, ExtractFilePath(ParamStr(0)) + 'PascalTriangle.txt');
  OutputInFile(Output, AMatrix);
  CloseFile(Output);
end;

const
  MaxAmount = 35;
  MinAmount = 0;

var
  PascalTriangle: TMatrix;
  Amount: Integer;
  IsCorrect: Boolean;
  // PascalTriangle - store the Pascal's triangle
  // Amount - amount of rows to be calculated in Pascal's triangle
  // IsCorrect - flag indicating the correctness of user input
  // Output - output text file where the Pascal's triangle will be written
begin
  WriteLn('Please enter a positive number');
  IsCorrect := False;

  repeat
    // Prompt the user to enter a number and validate the input
    IsCorrect := True;
    try
      Readln(Amount);
    except
      IsCorrect := False;
      WriteLn('Incorrect data entered. Try again');
    end;

    // Check if the entered number is negative
    if IsCorrect and (Amount < MinAmount) or (Amount > MaxAmount) then
    begin
      IsCorrect := False;
      WriteLn('Incorrect data entered. Try again');
    end;
  until IsCorrect;

  // Calculate Pascal's triangle based on the entered amount
  PascalTriangle := CalculatePascalTriangle(Amount);

  // Output file and write Pascal's triangle to it
  WriteToFile(PascalTriangle);
end.

