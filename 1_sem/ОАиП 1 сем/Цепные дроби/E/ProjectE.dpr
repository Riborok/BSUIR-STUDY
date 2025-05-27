Program ProjectE;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  i: integer;
  x, PowX, y, Num, Den, k: real;
  {i - cycle counter
  PowX - current power x
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
    k:= 256;
    Den:= 512;
    PowX:= x;

    //The cycle will continue while 2*k>=1 (look at the free member in the numerator, it is 2 times more k)
    while 2*k>=1 do
    begin

      Num := PowX + k*2;
      y := Num / Den;
      Den:= k + y;
      k:= k / 2;
      PowX:= PowX * x;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
	  x:= x + Step;
  end;

  Readln;
End.

