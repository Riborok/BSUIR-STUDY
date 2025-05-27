Program ProjectD;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  CoeffNum, Fact: int64;
  x, y, Num, Den, k: real;
  {CoeffNum - numerator coefficient that varies from 1 to 15
  Fact - considers factorial
  k - free member
  x - x value
  y - function value
  Num - numerator of fraction
  Den - denominator of fraction}

Begin

  x:=xStart;
  while x<=xLast do
  begin

    //Assign initial values
    k:= 2024;
    den:= 4048;
    CoeffNum:=1;
    Fact:=1;

    //The cycle will continue until the numerator coefficient is <=15
    while CoeffNum<=15 do
    begin

      Num:=Fact * (x-CoeffNum);
      y := Num / Den;
      Den:= k + y;
      k:= k/2;
      CoeffNum:= CoeffNum + 1;

      //The factorial of the next number is equal to the previous factorial times the next number
      Fact:= Fact*CoeffNum;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
    x:= x + Step;
  end;

  Readln;
End.

