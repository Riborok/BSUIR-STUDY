Program Lab1FuncImmediately;
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
  LeftExpressionPart, LeftExpression, RightExpressionPart, RightExpression, ResultOfExpression: TMatrix;
  // FirstMatrix, SecondMatrix - matrix A and B respectively
  // ResultOfExpression - the result of the matrix expression

// The function finds the sum (difference) of two matrices.
// For the difference, set the third parameter to true
Procedure MatrixSum(Const FirstMatrix, SecondMatrix: TMatrix; var Result: TMatrix;
         Const isDifference: Boolean = False);
Var
  i, j, Sign: Integer;
  // i,j – cycle counter
  // Sign – sign
Begin

  // Determine the sign
  If isDifference then
    Sign:= -1
  Else
    Sign:= 1;

  // Sum the matrices
  For i:= 1 to 3 do
    For j:= 1 to 3 do
      Result[i,j]:=FirstMatrix[i,j]+Sign*SecondMatrix[i,j];
End;

// The function finds the product of a matrix and a number
Procedure MultiplyMatrixAndConst(Const Matrix: TMatrix; Const Num: Integer; var Result: TMatrix);
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
Procedure MultiplyMatrices(Const FirstMatrix, SecondMatrix: TMatrix; var Result: TMatrix);
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
Procedure InputMatrix(var Result: TMatrix);
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
Procedure OutputMatrix(Const Matrix: TMatrix);
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
  InputMatrix(MatrixA);
  Writeln;
  Writeln('Enter matrix B:');
  InputMatrix(MatrixB);
  Writeln;

  // Calculating the value of the expression
  Writeln('(A - B^2) * (2*A + B)=');

  // (A - B^2)
  MultiplyMatrices(MatrixB, MatrixB, LeftExpressionPart);
  MatrixSum(MatrixA, LeftExpressionPart, LeftExpression, True);

  // (2*A + B)
  MultiplyMatrixAndConst(MatrixA, 2, RightExpressionPart);
  MatrixSum(RightExpressionPart, MatrixB, RightExpression);

  // (A - B^2) * (2*A + B)
  MultiplyMatrices(LeftExpression, RightExpression, ResultOfExpression);

  OutputMatrix(ResultOfExpression);
  Readln;
End.

