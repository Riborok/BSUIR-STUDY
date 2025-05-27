Program ProjectA;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  x, y, Num, Den: real;
  {x - x value
  y - function value
  Num - numerator of fraction
  Den - denominator of fraction}

Begin

  x:=xStart;
  while x<=xLast do
  begin

    //Assign initial values
    Num := 256;
    Den := x*x;

    //The cycle continues until the numerator is equal to 1
    //Since Num is real, then instead of = we enter >=
    while Num >= 1 do
    begin
      y := Num/Den;
      Den:= x*x + y;
      Num := Num / 2;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
    x:= x + Step;
  end;

  Readln;
End.

