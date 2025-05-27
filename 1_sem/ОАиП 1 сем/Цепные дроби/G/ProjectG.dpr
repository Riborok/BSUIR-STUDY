Program ProjectG;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  Coeff, CoeffK: integer;
  x, y, Num, Den: real;
  {Coeff - free coefficient in the numerator and free term
  CoeffK - ñoefficient in the free term (before x)
  x - x value
  y - function value
  Num - numerator of fraction}

Begin

  x:=xStart;
  while x<=xLast do
  begin

    //Assign initial values
    Coeff:= 10;
    CoeffK:= 19;
    Den:= 11+21*x;

    //The cycle will continue until the free coefficient in the numerator and free term >=11
    while Coeff>=1 do
    begin
      Num:=Coeff*Coeff*x*(1+x);
      Den := (Coeff + CoeffK*x) - (Num/Den);
      Coeff:= Coeff - 1;
      CoeffK:= CoeffK - 2;
    end;

    //Find the value of the expression
    y:= x/Den;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
	  x:= x + Step;
  end;

  Readln;
End.

