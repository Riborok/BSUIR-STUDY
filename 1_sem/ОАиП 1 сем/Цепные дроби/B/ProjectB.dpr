Program ProjectB;

Const
  FirstPow = 11;
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {FirstPow - first power numerator
  xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  i: integer;
  x, y, Num, Den: real;
  {i - cycle counter
  x - x value
  y - function value
  Num - numerator of fraction
  Den - denominator of fraction}

Begin

  //Assign initial values for numerator and denominator
  x:=xStart;
  while x<=xLast do
  begin

    //Assign initial values
    Num:=1;
    for i := 1 to FirstPow do
      Num:= Num * x;
    Den:= Num * x;

    //Power of the numerator decreases from 11 to 1
    for i:= FirstPow downto 1 do
    begin

      y := Num / Den;

      //The numerator is equal to the free member (k)
      Den:= Num + y;

      Num := Num / x;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
    x:= x + Step;
  end;

  Readln;
End.

