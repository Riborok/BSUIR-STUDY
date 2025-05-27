Program Normal_Implementation;
{
  Matrices A and B have order 3. The user fills in matrices A and B.
  The program finds the value of the matrix expression: (A - B^2) * (2*A + B).
}

{$APPTYPE CONSOLE}

uses System.SysUtils;

Type
  TMatrix = Array of Array of Double;
  // TMatrix - type for a matrix of order 3

Var
  MatrixA: TMatrix;
  MatrixB: TMatrix;
  Res: TMatrix;
  MartixOrded: integer;
  // FirstMatrix, SecondMatrix - matrix A and B respectively
  // Res - the result of the matrix expression
  // MartixOrded - matrix order

// The function finds the sum (difference) of two matrices.
// For the difference, set the third parameter to true
Function MatrixSum(Const FirstMatrix, SecondMatrix: TMatrix;
         Const isDifference: Boolean = False) : TMatrix;
Var
  i, j, Sign: Integer;
  // i,j – cycle counter
  // Sign – sign
Begin

  // Checking for the correctness of matrices
  if (Length(FirstMatrix) = Length(SecondMatrix)) and
     (Length(FirstMatrix[0]) = Length(SecondMatrix[0]) ) then
  begin

    // Determine the sign
    If isDifference then
      Sign:= -1
    Else
      Sign:= 1;

    // Sum the matrices
    SetLength(Result, Length(FirstMatrix), Length(FirstMatrix[0]));
    For i:= Low(Result) to High(Result) do
      For j:= Low(Result[0]) to High(Result[0]) do
        Result[i,j]:=FirstMatrix[i,j] + Sign*SecondMatrix[i,j];
  end;

End;

// The function finds the product of a matrix and a number
Function MultiplyMatrixAndConst(Const Matrix: TMatrix; Const Num: Double):TMatrix;
Var
  i, j:integer;
  // i, j - cycle counter
Begin
  // Finding the product
  SetLength(Result, Length(Matrix), Length(Matrix[0]));
  For i:= Low(Result) to High(Result) do
    For j:= Low(Result[0]) to High(Result[0]) do
      Result[i,j]:=Num*Matrix[i,j];
End;


// The function finds the product of two matrices (matrices must be the same size)
Function MultiplyMatrices(Const FirstMatrix, SecondMatrix: TMatrix): TMatrix;
Var
  i, j, l: Integer;
  // i, j, l - cycle counter
Begin

  if Length(FirstMatrix[0]) = Length(SecondMatrix) then
  begin

    // Multiplying matrices
    SetLength(Result, Length(FirstMatrix), Length(SecondMatrix[0]));
    For i:= Low(Result) to High(Result) do
      For j:= Low(Result[0]) to High(Result[0]) do
      Begin

        // Initializing the value of matrix element
        Result[i,j]:= 0;
        For l:= Low(SecondMatrix) to High(SecondMatrix) do
          Result[i,j]:= Result[i,j] + FirstMatrix[i,l] * SecondMatrix[l,j];

      End;
  end;
End;

// Function to read a matrix
Procedure InputMatrix(var Matrix: TMatrix);
Var
  i,j: integer;
  isCorrect: boolean;
  // i, j - cycle counter
  // isCorrect - flag to confirm the correctness of entering numbers
Begin

  // Matrix input
  for i := Low(Matrix) to High(Matrix) do
  begin
    for j := Low(Matrix[0]) to High(Matrix[0]) do
      repeat
        isCorrect:= True;
        Try
          Read(Matrix[i, j]);
        Except
          isCorrect:= False;
          Writeln('Matrix value entered incorrectly. Repeat again');
        End;
      until isCorrect;

    Readln;
  end;

End;

// Function to read a matrix order
Function InputMatrixOrder(): Integer;
Var
  isCorrect: boolean;
  // isCorrect - flag to confirm the correctness of entering numbers
Begin

  // Matrix input
  repeat
    isCorrect:= True;
    Try
      Read(Result);
    Except
      isCorrect:= False;
      Writeln('Matrix order value entered incorrectly. Repeat again');
    End;
  until isCorrect;

End;

// The procedure outputs a matrix
Procedure OutputMatrix(Const Matrix:TMatrix);
Var
  i,j:integer;
  // i, j - cycle counter
Begin

  // Matrix output
  For i:= low(Matrix) to High(Matrix) do
  Begin
    For j:= Low(Matrix[0]) to High(Matrix[0]) do
      Write(Matrix[i,j]:8:1);
    Writeln
  End;
  Writeln
End;



Begin

  Writeln('Enter the order of matrices');
  MartixOrded:= InputMatrixOrder();
  Writeln;

  // Outputting the initial matrixes
  Writeln('Enter matrix A:');
  SetLength(MatrixA, MartixOrded, MartixOrded);
  InputMatrix(MatrixA);
  Writeln;

  Writeln('Enter matrix B:');
  SetLength(MatrixB, MartixOrded, MartixOrded);
  InputMatrix(MatrixB);
  Writeln;

  // Calculating the value of the expression
  Writeln('(A - B^2) * (2*A + B)=');
  Res:= MultiplyMatrices( MatrixSum(MatrixA, MultiplyMatrices(MatrixB, MatrixB), true),
        MatrixSum(MultiplyMatrixAndConst(MatrixA, 2), MatrixB) );

  OutputMatrix(Res);
  Readln;
End.

