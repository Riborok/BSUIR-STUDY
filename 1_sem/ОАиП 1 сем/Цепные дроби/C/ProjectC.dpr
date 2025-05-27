Program ProjectC;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  k: integer;
  x, y, Num, Den: real;
  {k - free member
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
    k:= 200;
    Den:= 201;

    //The cycle will be until the free term >=0
    while k>=0 do
    begin
      Num:=x-k;
      y := Num / Den;
      Den:= k - y;
      k:= k-1;
    end;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
	  x:= x + Step;
  end;

  Readln;
End.

