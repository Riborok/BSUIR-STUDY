Program homework1;
{The program find perfect numbers in the range from m to n}

{$APPTYPE CONSOLE}

Var
  m, n, i, k, MaxDiv, SumDiv : integer;
  NoPerf: boolean;
  {m - start number of range
  n - finish number of range
  i - cycle counter (current supposed number)
  k - cycle counter (supposed divisor of a number)
  MaxDiv - maximum supposed divisor of a number
  SumDiv - the sum of the divisors of a number
  NoPerf - if find the perfect number, it is true}

Begin
  Writeln('Enter range (the range must be from a natural number)');
  Read(m);
  Readln(n);

  if (m<=0) or (n<=0) then
    Writeln('The range must be from a natural number. Restart the programm.')

  else
  begin

    //Iterating the numbers from the range from m to n
    for i := m to n do
    begin

      //Zero out the sum of the divisors of the previous number
      SumDiv:= 0;

      //Found maximum supposed divisor of a number
      MaxDiv:= i div 2;

      //Iterating over the supposed divisors of a number
      for k := 1 to MaxDiv do

        //Check if it is a divisor then add to the sum of the divisors
        if (i mod k =0) then
          SumDiv:= SumDiv + k;

      //If the sum of the divisors is equal to this number, then this is a perfect number.
      if SumDiv = i then
      begin
        Writeln(i,' - perfect number');
        NoPerf:=True;
      end;
    end;

    //If there are no perfect numbers, write an error
    if NoPerf = False then
      Writeln('This range does not have perfect numbers');
  end;

  Readln;
End.
