Program DelphiLab1FuncIntermediate;
{
  Matrices A and B have order 3. The user fills in matrices A and B.
  The program finds the value of the matrix expression: (A - B^2) * (2*A + B).
}

{$APPTYPE CONSOLE}

uses System.SysUtils;

Type
  TMatrix = Array[1..3, 1..3] of Integer;
  // TMatrix - type for a matrix of order 3

Var
  MatrixA: TMatrix;
  MatrixB: TMatrix;
  LeftExpression, RightExpression, Res: TMatrix;
  // FirstMatrix, SecondMatrix - matrix A and B respectively
  // LeftExpression, RightExpression, Res - the result of the matrix expression

// The function finds the sum (difference) of two matrices.
// For the difference, set the third parameter to true
Function MatrixSum(Const FirstMatrix, SecondMatrix: TMatrix;
         Const isDifference: Boolean = False) : TMatrix;
Var
  i, j, Sign: Integer;
  // i,j – cycle counter
  // Sign – sign
Begin

  // Determine the sign
  If isDifference then
    Sign:= -1
  Else
    Sign:=1;

  // Sum the matrices
  For i:= 1 to 3 do
    For j:= 1 to 3 do
      Result[i,j]:=FirstMatrix[i,j]+Sign*SecondMatrix[i,j];
End;

// The function finds the product of a matrix and a number
Function MultiplyMatrixAndConst(Const Matrix: TMatrix; Const Num: Integer):TMatrix;
Var
  i, j:integer;
  // i, j - cycle counter
Begin
  // Finding the product
  For i:= 1 to 3 do
    For j:= 1 to 3 do
      Result[i,j]:=Num*Matrix[i,j];
End;


// The function finds the product of two matrices (matrices must be the same size)
Function MultiplyMatrices(Const FirstMatrix, SecondMatrix: TMatrix): TMatrix;
Var
  i, j, l: Integer;
  // i, j, l - cycle counter
Begin

  // Multiplying matrices
  For i:= 1 to 3 do
    For j:= 1 to 3 do
    Begin

      // Initializing the value of matrix element
      Result[i,j]:= 0;
      For l:= 1 to 3 do
        Result[i,j]:= Result[i,j] + FirstMatrix[i,l] * SecondMatrix[l,j];

    End;
End;

// Function to read a matrix
Function InputMatrix(): TMatrix;
Var
  i,j: integer;
  isCorrect: boolean;
  // i, j - cycle counter
  // isCorrect - flag to confirm the correctness of entering numbers
Begin

  // Matrix input
  for i := 1 to 3 do
  begin
    for j := 1 to 3 do
      repeat
        isCorrect:= True;
        Try
          Read(Result[i, j]);
        Except
          isCorrect:= False;
          Writeln('Matrix value entered incorrectly. Repeat again');
        End;
      until isCorrect;

    Readln;
  end;

End;

// The procedure outputs a matrix
Procedure OutputMatrix(Const Matrix:TMatrix);
Var
  i,j:integer;
  // i, j - cycle counter
Begin

  // Matrix output
  For i:= 1 to 3 do
  Begin
    For j:= 1 to 3 do
      Write(Matrix[i,j]:5);
    Writeln
  End;
  Writeln
End;



Begin

  // Outputting the initial matrixes
  Writeln('Enter matrix A:');
  MatrixA := InputMatrix();
  Writeln;
  Writeln('Enter matrix B:');
  MatrixB := InputMatrix();
  Writeln;

  Writeln('B^2 =');
  LeftExpression:= MultiplyMatrices(MatrixB, MatrixB);
  OutputMatrix(LeftExpression);
  Writeln;

  Writeln('(A - B^2) =');
  LeftExpression:= MatrixSum(MatrixA, LeftExpression, true);
  OutputMatrix(LeftExpression);
  Writeln;

  Writeln('2*A =');
  RightExpression:= MultiplyMatrixAndConst(MatrixA, 2);
  OutputMatrix(RightExpression);
  Writeln;

  Writeln('(2*A + B) =');
  RightExpression:= MatrixSum(RightExpression, MatrixB);
  OutputMatrix(RightExpression);
  Writeln;

  // Calculating the value of the expression
  Writeln('(A - B^2) * (2*A + B) =');
  Res:= MultiplyMatrices(LeftExpression, RightExpression);

  OutputMatrix(Res);
  Readln;
End.

