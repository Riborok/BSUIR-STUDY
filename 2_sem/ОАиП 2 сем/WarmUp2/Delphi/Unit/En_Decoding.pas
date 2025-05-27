unit En_Decoding;
{
  This unit contains procedures and functions for encoding and decoding messages
  using a matrix transposition cipher algorithm. The matrix transposition cipher algorithm
  involves arranging characters of the message in a matrix in a certain order, and then
  reading off the characters in a different order to obtain the encoded message.
  The decoding procedure reverses the encoding process to obtain the original message
}

interface

type
  // The TEncodedWithKey record is used to store both the key and the encoded text
  // It is used in the Encoding procedure to return both values to the main program
  TEncodedWithKey = Record
    Key: string;
    EncodedStr: string;
  End;

// The Encoding procedure takes a file name as input, reads the text from the file,
// encodes it using a specific algorithm, and writes the encoded text and key to output files
procedure Encoding(const ATextFileName: string);

// The Decoding procedure takes two file names as input - one for the encoded text and
// one for the key, reads the text from both files, decodes the text using the same
// algorithm that was used to encode it, and writes the decoded text to an output file.
procedure Decoding(const ATextFileName, AKeyFileName: string);

implementation
uses
  System.SysUtils;
Type
  TDirection = array [0..3] of char;
  TMatrix = array of array of Char;
  TAction = procedure (var AMatrix: TMatrix; var AStr: string; const I, J, L: Integer);
  // TDirection - the TDirection type is an array of characters that represent
  // the four directions used in the algorithm: Up, Right, Down, and Left.

  // TMatrix - type is a dynamic array that is used to store the matrix of
  // characters used in the algorithm.

  // TAction - type is a procedure type that takes a matrix of characters, a string,
  // and three integer arguments. It is used to perform the main action of the
  // algorithm - either encoding or decoding.
const
  Ñlockwise: TDirection = ('U','R','D','L');
  AntiÑlockwise: TDirection = ('U','L','D','R');
  TrashChar = #152;
  AmountToReset = 2;
  DirectionAmount = 4;
  // Ñlockwise is a TDirection array that represents the clockwise
  // directions used in the algorithm.

  // AntiÑlockwise is a TDirection array that represents the counterclockwise
  // directions used in the algorithm.

  // TrashChar is a character that is used as a placeholder when a
  // character is deleted from the matrix.

  // AmountToReset is an integer that represents the number of times a character
  // must pass through the entire matrix in order to be considered
  // "fully encoded" by the algorithm

  // DirectionAmount is an integer that represents the number of
  // directions used in the algorithm.



// These procedures are used to write characters to and from a matrix.
// WriteToMatrix writes the character from AStr to AMatrix at coordinates I, J.
// WriteToStr writes the character from AMatrix at coordinates I, J to AStr at index L.
procedure WriteToMatrix(var AMatrix: TMatrix; var AStr: string; const I, J, L: Integer);
begin
  AMatrix[I, J] := AStr[L];
end;
procedure WriteToStr(var AMatrix: TMatrix; var AStr: string; const I, J, L: Integer);
begin
  AStr[L]:= AMatrix[I, J];
end;

// Set the directions and current direction based on input number
// Assign clockwise or anticlockwise directions based on input number
// Update the current direction index based on input number
procedure GetDirections(const ANum: Integer; var ADirections: TDirection;
                        var ACurrDir: Integer);
begin
  case ANum of
    0:
    begin
      ADirections := Ñlockwise;
      ACurrDir := 0;
    end;
    1:
    begin
      ADirections := Ñlockwise;
      ACurrDir := 1;
    end;
    2:
    begin
      ADirections := Ñlockwise;
      ACurrDir := 2;
    end;
    3:
    begin
      ADirections := Ñlockwise;
      ACurrDir := 3;
    end;
    4:
    begin
      ADirections := AntiÑlockwise;
      ACurrDir := 0;
    end;
    5:
    begin
      ADirections := AntiÑlockwise;
      ACurrDir := 1;
    end;
    6:
    begin
      ADirections := AntiÑlockwise;
      ACurrDir := 2;
    end;
    7:
    begin
      ADirections := AntiÑlockwise;
      ACurrDir := 3;
    end;
  end;
end;

// This procedure performs matrix traversal, based on the specified direction
// and current direction. It calls the specified action for each matrix element,
// allowing you to encode or decode the message. The matrix is traversed in a
// spiral pattern starting from the center, with the direction and reset frequency
// alternating in a predetermined manner.
procedure MatrixPass(var AMatrix: TMatrix; var AStr: string;
                     const ADirections: TDirection; ACurrDir: Integer;
                     const AAction :TAction);
var
  ConsecutiveQuantity, ResetCounter, Counter: Integer;
  I, J, L: Integer;
begin
  ConsecutiveQuantity := 1;
  Counter := 0;
  ResetCounter := 0;

  I := Length(AMatrix) shr 1;
  J := I;
  for L := 1 to Length(AStr) do
  begin
    AAction(AMatrix, AStr, I, J, L);
    Case ADirections[ACurrDir] of
      'U': Dec(I);
      'D': Inc(I);
      'L': Dec(J);
      'R': Inc(J);
    End;
    Inc(Counter);
    if Counter = ConsecutiveQuantity then
    begin
      Inc(ACurrDir);
      ACurrDir := ACurrDir mod DirectionAmount;

      Inc(ResetCounter);
      Counter := 0;
      if ResetCounter = AmountToReset then
      begin
        ResetCounter := 0;
        Inc(ConsecutiveQuantity);
      end;
    end;
  end;
end;

// Rounds up a single-precision floating-point number to the nearest integer
// greater than or equal to the number. If the fractional part of the number is
// greater than 0, then the result is rounded up to the next integer.
function Ceil(const X: Single): Integer;
begin
  Result := Trunc(X);
  if Frac(X) > 0 then
    Inc(Result);
end;

// This procedure performs a traversal of the given matrix, modifying the
// characters in the string based on the current position and action performed
// at each step. The traversal direction is determined by the current direction
// and the direction change sequence. The consecutive quantity of steps in each
// direction before changing direction is determined by the amount to reset value.
function GenerateDecodedText(var AText, AKey: string): string;
var
  MatrixOrder, I, J, K, L: Integer;
  Matrix: TMatrix;
  CurrDir: Integer;
  Directions : TDirection;
begin
  // Calculate the size of the matrix to be used to encode the text
  MatrixOrder:= Ceil(sqrt(Length(AText)));
  Inc(MatrixOrder, Ord(not Odd(MatrixOrder)));

  // Create a matrix to store the encoded characters
  SetLength(Matrix, MatrixOrder, MatrixOrder);

  // Set up the result string to have the same length as the input string
  SetLength(Result, Length(AText));
  Result:= AText;
  AText:= '';

  // Loop over each character in the key
  for K := Length(AKey) downto 1 do
  begin
    for I := 0 to MatrixOrder - 1 do
      for J := 0 to MatrixOrder - 1 do
        Matrix[I, J] := TrashChar;

    // Get the direction and starting position for encoding the characters
    GetDirections(StrToInt(AKey[K]), Directions, CurrDir);

    // Pass over the matrix, encoding the characters
    MatrixPass(Matrix, Result, Directions, CurrDir, WriteToMatrix);

    // Put the encoded characters back into the result string
    L := 1;
    for I := 0 to High(Matrix) do
      for J := 0 to High(Matrix) do
        if (Matrix[I, J] <> TrashChar) then
        begin
          Matrix[I, J] := Result[L];
          Inc(L);
        end;

    // Get the direction and starting position for decoding the characters
    GetDirections(StrToInt(AKey[K]), Directions, CurrDir);

    // Pass over the matrix again, decoding the characters and putting them
    // into the output string
    MatrixPass(Matrix, Result, Directions, CurrDir, WriteToStr);
  end;
end;

// This procedure reads the encoded text and the encryption key from text files,
// generates the decoded text using the GenerateDecodedText function, and writes
// the decoded text to a new file.
procedure Decoding(const ATextFileName, AKeyFileName: string);
var
  flOutput, flInput, flKey: TextFile;
  InputString, InputKey: String;
begin
  Assignfile(flOutput, '..\..\..\Files\Decoded\Decoded_' + ATextFileName + '.txt');
  Rewrite(flOutput);
  Assignfile(flInput, '..\..\..\Files\Encoded\' + ATextFileName + '.txt');
  Reset(flInput);
  Assignfile(flKey, '..\..\..\Files\Encoded\' + AKeyFileName + '.txt');
  Reset(flKey);

  Read(flInput, InputString);
  Read(flKey, InputKey);

  Write(flOutput, GenerateDecodedText(InputString, InputKey));

  Close(flInput);
  Close(flOutput);
  Close(flKey);
end;

// This function generates an encoded version of the input string by repeatedly passing
// over a matrix of characters, modifying the string in each pass according to the
// current traversal direction and action. The number of passes is randomized.
// The generated key is the sequence of traversal directions in clockwise order,
// with an additional offset added for counterclockwise traversal.
function GenerateEncodedText(var AText: String): TEncodedWithKey;
const
  MinPasses = 7;
  MaxRandom = 420;
var
  MatrixOrder, I, J, K, L: Integer;
  Matrix: TMatrix;
  CurrDir: Integer;
  Directions : TDirection;
begin
  // Calculate the size of the matrix to be used to encode the text
  MatrixOrder:= Ceil(sqrt(Length(AText)));
  Inc(MatrixOrder, Ord(not Odd(MatrixOrder)));

  // Create a matrix to store the encoded characters
  SetLength(Matrix, MatrixOrder, MatrixOrder);

  // Initialize the result struct with the encoded string and an empty key
  Result.Key := '';
  Result.EncodedStr:= AText;
  AText:= '';

  // Randomize the number of passes to make over the matrix
  Randomize;
  for K := 1 to Random(MaxRandom) + MinPasses do
  begin

    // Initialize the matrix with trash characters
    for I := 0 to MatrixOrder - 1 do
      for J := 0 to MatrixOrder - 1 do
        Matrix[I, J] := TrashChar;

    // Get the traversal direction and sequence for the pass
    GetDirections(Random(8), Directions, CurrDir);

    // Add the current traversal direction to the key
    if Directions = Ñlockwise then
      Result.Key := Result.Key + IntToStr(CurrDir)
    else
      Result.Key := Result.Key + IntToStr(CurrDir + DirectionAmount);

    // Pass over the matrix, modifying the encoded string
    MatrixPass(Matrix, Result.EncodedStr, Directions, CurrDir, WriteToMatrix);

    // Put the modified characters back into the encoded string
    L:= 1;
    for I := 0 to High(Matrix) do
      for J := 0 to  High(Matrix) do
        if Matrix[I, J] <> TrashChar then
        begin
          Result.EncodedStr[L] := Matrix[I, J];
          Inc(L);
        end;
  end;
end;

// This procedure encodes the input text file by generating an encoded text and
// an encryption key. It reads the input text file, generates the encoded text
// and key using the GenerateEncodedText function, and writes the encoded text
// and key to separate files.
procedure Encoding(const ATextFileName: string);
var
  flOutput, flInput, flKey: TextFile;
  InputString: String;
  EncodedWithKey: TEncodedWithKey;
begin
  Assignfile(flInput, '..\..\..\Files\Decoded\' + ATextFileName + '.txt');
  Reset(flInput);
  Assignfile(flOutput, '..\..\..\Files\Encoded\Encoded_' + ATextFileName + '.txt');
  Rewrite(flOutput);
  Assignfile(flKey, '..\..\..\Files\Encoded\Key_' + ATextFileName + '.txt');
  Rewrite(flKey);

  Read(flInput, InputString);

  EncodedWithKey:= GenerateEncodedText(InputString);

  Write(flOutput, EncodedWithKey.EncodedStr);
  Write(flKey, EncodedWithKey.Key);

  Close(flInput);
  Close(flOutput);
  Close(flKey);
end;

end.
