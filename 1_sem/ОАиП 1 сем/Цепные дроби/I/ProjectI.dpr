Program ProjectI;

Const
  xStart = 0.5;
  xLast = 0.8;
  Step = 0.05;
  {xStart - start value for x
  xLast - last value for x
  Step - step with which x changes}

Var
  CoeffNum, k: integer;
  x, y, Num, Den: real;
  {CoeffNum - ñoefficient in the numerator
  n - last power x
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
    k:= 12;
    CoeffNum:= 6;
    Den:=13;

    //The cycle will continue until free member >=1
    while k>=1 do
    begin

      Num:= CoeffNum*CoeffNum*(x-1);
      Den := k + Num/Den;
      k:= k - 1;

      //CoeffNum decreases when k becomes odd
      if (k mod 2) = 0 then
        CoeffNum:= CoeffNum - 1;
    end;

    //Find the value of the expression
    y:= (x-1)/Den;

    writeln('x = ', x:4:2, ' y = ', y:0:5);

    //Modernize x
	  x:= x + Step;
  end;

  Readln;
End.

