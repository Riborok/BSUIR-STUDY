Program ProjectF;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  Pow, i: integer;
  x, PowX, y, Num, Den, k: real;
  {Pow - current power x (fractional too)
  i - cycle counter
  k - free member
  PowX - current power x
  x - x value
  y - function value
  Num - numerator of fraction
  Den - denominator of fraction}

Begin

  x:=xStart;
  while x<=xLast do
  begin

    //Assign initial values
    Pow:= 2;
    PowX:= x*x;

    //Using exp and ln find power x (fractional too)
    Den:= exp(1/2*ln(x));

    //The cycle will continue until the power <=10
    while Pow<=10 do
    begin

      //Using exp and ln find power x (fractional too)
      //Note that the fractional power is 1 more than the integer power in the numerator
      k:= exp(1/(pow+1)*ln(x));

      Num:= PowX*(x-Pow);
      y := Num / Den;
      Den:= k + y;
      Pow:= Pow + 1;
      PowX:= PowX * x;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
	  x:= x + Step;
  end;

  Readln;
End.

