Program homework2;
{The program find prime numbers, no more n}

{$APPTYPE CONSOLE}

Var
  n, i, k, LastDiv : integer;
  FindDel :boolean;
  {n - finish number of range
  i - cycle counter (current supposed number)
  k - cycle counter (current checked divisor)
  LastDiv - the last divisor that need to check
  FindDel - if a divisor is found, then the variable is true}

Begin
  Writeln('Enter the last number of range (number must be >1, since 1 and negative numbers are not prime)');
  Readln(n);

  if (n<=1) then
    Writeln('Number must be >1 (this range has no prime numbers). Restart the programm')

  else;
  begin

    //Find prime numbers exist between 2 and n (since 1 is not a prime number).
    for i := 2 to n do
    begin

      //Find the last divisor that need to check
      LastDiv := Trunc(exp(1/2*ln(i)));

      //If LastDiv equal 1, then it is a prime number
      if (LastDiv = 1) then
        Writeln(i,' - prime number')

      //Else check.
      else
      begin

        //Initialize FindDel for the next iteration
        FindDel:= False;

        //Cheking for divisors from 2 to LastDiv
        k:= 2;
        while (k<=LastDiv) and (FindDel = False) do
        begin

          //If the number has divisors than it's not a prime number. Exit the cycle
          if i mod k = 0 then
            FindDel:= True;

          //If reach the last divisor that need to check (and it is not a divider),
          //then this is a prime number
          if (k = LastDiv) and (FindDel = False)  then
            Writeln(i,' - prime number');

          //Modernize k
          k:= k+1;
        end;
      end;
    end;
  end;

  Readln;
End.
