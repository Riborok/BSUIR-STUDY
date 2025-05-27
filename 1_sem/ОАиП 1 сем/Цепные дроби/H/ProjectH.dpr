Program ProjectH;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  Pow, n, i: integer;
  x, y, Num, Den, k: real;
  {Pow - current power
  i - cycle counter
  n - last power x
  k - free member
  x - x value
  PowX - current power x
  y - function value
  Num - numerator of fraction
  Den - denominator of fraction}

Begin

  //User enters n
  Writeln('Write n');
  Readln(n);

  x:=xStart;
  while x<=xLast do
  begin

    //Assign initial values
    Pow:=0;
    Num:= 1;

    //Using exp and ln find power x (fractional too)
    Den:= x*x/(exp(1/2*ln(x))+1);

    //The cycle will continue until current power x <= last power x
    while Pow <= n do
    begin

      //Using exp and ln find power x (fractional too)
      k:= exp(1/(Pow+1)*ln(x));

      y := Num/Den;
      Den:= k+y;
      Pow:= Pow+1;
      Num:= Num * x;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
	  x:= x + Step;
  end;

  Readln;
End.

