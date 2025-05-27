program WarmUp1;
{
 Given points on a plane, find a point on the x such that the maximum
 distance from that point to the given points is the minimum.
}

{$APPTYPE CONSOLE}
uses
  System.SysUtils;

const
  MaxAmount = 10000;
  MinAmount = 2;
  MinAccurace = 1;
  // MaxAmount - maximum amount of points
  // MinAmount - minimum amount of points
  // MinAccurace - minimum accuracy

type
  TNameCoord = (x, y);
  TCoordsPoint = array[1..MaxAmount, TNameCoord] of double;
  // TNameCoord - coordinate name
  // TCoordsPoint - array of points

var
  Point, Dist, Accuracy: double;
  Amount, i: Integer;
  ArrPoints: TCoordsPoint;
  isCorrect : boolean;
  // BestPoint - best point
  // Accuracy - calculation accuracy
  // Amount - amount of points
  // i - cycle counter
  // ArrPoints - array of points
  // isCorrect - flag to confirm the correctness of entering numbers

// Finding the maximum sqrt(distance)
function FindMaxSqrtDistance(const Coordx: Double; const Arr: TCoordsPoint; const ArrLen: integer): Double;
var
  i: Integer;
  sqrtDist: Double;
  // i - cycle counter
  // Dist - distance between points
begin
  // Reset the result
  Result := 0;

  // Finding the maximum distance
  for i := 1 to ArrLen do
  begin

    // Find the distance using the formula
    sqrtDist := sqr(Arr[i, x] - Coordx) + sqr(Arr[i, y] );

    // Update result if needed
    if sqrtDist > Result then
      Result := sqrtDist;
  end;
end;

// Finds the minimum and the maximum value
procedure FindMinMax(const Arr : TCoordsPoint; const ArrLen: integer; var min, max: double);
var
  i: integer;
begin

  min:= Arr[1, x];
  max:= Arr[1, x];
  for i := 2 to ArrLen do
  begin

    if Arr[i, x] > max then
      max:= Arr[i, x]
    else if Arr[i, x] < min then
      min:= Arr[i, x];

  end;

end;

// Find the best point using linear search
procedure FindPointWithMinMaxDistanceLin(const Arr: TCoordsPoint; Const ArrLen: integer; Const CalcAccuracy: double;
           var BestPoint, BestDist: double);
var
  CurrPoint, LastPoint, CurrDist: Double;
  // CurrPoint - current point on the axic
  // CurrPoint - last point on the axic

begin
  FindMinMax(Arr, ArrLen, CurrPoint, LastPoint);

  BestDist := double.MaxValue;

  repeat

    CurrDist:= FindMaxSqrtDistance(CurrPoint, Arr, ArrLen);

    if CurrDist < BestDist then
    begin
      BestDist:= CurrDist;
      BestPoint:= CurrPoint;
    end;

    CurrPoint:= CurrPoint + CalcAccuracy;
  until CurrPoint > LastPoint;

  BestDist:= sqrt(BestDist);
end;

// Find the best point using binary search
procedure FindPointWithMinMaxDistanceBin(const Arr: TCoordsPoint; Const ArrLen: integer; Const CalcAccuracy: double;
          var BestPoint, BestDist: double);
var
  Left, Mid, Right : Double;
  // Left, Mid, Right - left, mid and right bounds of binary search
begin

  // Border initialization
  FindMinMax(Arr, ArrLen, Left, Right);

  // Find a value until reach the given accuracy
  repeat

    // Finding the middle
    Mid := (Left + Right) / 2;

    // Narrowing down the search interval
    if FindMaxSqrtDistance(Mid - CalcAccuracy, Arr, ArrLen) < FindMaxSqrtDistance(Mid + CalcAccuracy, Arr, ArrLen) then
      Right := Mid
    else
      Left := Mid;

  until Right - Left < CalcAccuracy;

  BestPoint:= Mid;
  BestDist:= sqrt(FindMaxSqrtDistance(Mid, Arr, ArrLen));
end;

function RoundAcc(CalcAccuracy: double): integer;
begin

  result := 0;
  while CalcAccuracy < 1 do
  begin
    CalcAccuracy := CalcAccuracy * 10;
    Inc(result);
  end;

end;

begin

  Writeln('The program find a point on the x such that the maximum distance from that point to the given points is the minimum.');
  Writeln;

  Writeln('Enter the calculation accuracy:');


  // Validating the correct input data type
  repeat
    isCorrect:= True;

    // Validate data type
    Try
      Readln(Accuracy);
    Except
      Writeln('Accuracy entered incorrectly');
      isCorrect:= False;
    End;

    // Validate range
    if isCorrect and (Accuracy > MinAccurace) then
    begin
      Writeln('Accuracy entered incorrectly');
      isCorrect:= False;
    end;

  until isCorrect;

  Writeln;
  Writeln('Enter the amount of points:');

   // Validating the correct input data type
  repeat
    isCorrect:= True;

    // Validate data type
    Try
      Readln(Amount);
    Except
      Writeln('Amount of points entered incorrectly');
      isCorrect:= False;
    End;

    // Validate range
    if isCorrect and ((Amount < MinAmount) or (Amount > MaxAmount)) then
    begin
      Writeln('Amount of points entered incorrectly');
      isCorrect:= False;
    end;

  until isCorrect;


  Writeln;
  Writeln('Enter point coordinates x, y.');

  // Cycle to read all the points
  for i := 1 to Amount do
  begin

    Writeln('Point number ', i,' :');

    // Validating the correct input data type
    repeat

      isCorrect:= True;

      // Validate data type
      Try
        Read(ArrPoints[i, x], ArrPoints[i, y]);
      Except
        Writeln('The  entered incorrectly');
        isCorrect:= False;
      End;
    until isCorrect;

  end;

  // Find the best point using binary search:
  FindPointWithMinMaxDistanceBin(ArrPoints, Amount, Accuracy, Point, Dist);

  Writeln('Best point x-coordinate using binary search: ', Point:0:RoundAcc(Accuracy),', Distance: ', Dist:0:RoundAcc(Accuracy));

  Readln;

  FindPointWithMinMaxDistanceLin(ArrPoints, Amount, Accuracy, Point, Dist);

  Writeln('Best point x-coordinate using linary search: ', Point:0:RoundAcc(Accuracy),', Distance: ', Dist:0:RoundAcc(Accuracy));

  Readln;
  Readln;
end.

