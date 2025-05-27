Program homework2;
{The program find prime numbers, no more n}

{$APPTYPE CONSOLE}

Const
  ExtremePoint = 100000;
  //ExtremePoint - last valid value n

Var
  n, i, k : integer;
  Nums : array [1..ExtremePoint] of boolean;
  {n - finish number of range
  i - cycle counter (current number)
  k - cycle counter (current multiplier)
  Nums - an array in which will cross out non-prime numbers}

Begin
  Writeln('Enter the last number of range (number must be >1, since 1 and negative numbers are not prime, and <=',ExtremePoint,')');
  Readln(n);

  if (n<=1) or (n>ExtremePoint) then
    Writeln('Number must be >1 and <=',ExtremePoint,'. Restart the programm')

  else
  begin

    //1 is not a prime number - cross out it
    Nums[1]:=True;

    //According to the Sieve of Eratosthenes, iterate the numbers (starting from 2)
    //Until the entered number is >= the square of current number
    i:=2;
    while (i*i <= n) do
    begin

      //Check if the number is crossed out.
      if Nums[i]= False then
        //Then cross out the numbers, from the itself number
        //(the square of that number) to the maximum multiplier (n div i)
        for k:= i to (n div i) do
          Nums[i*k] := True;

      //Modify i to the next number
      i:= i + 1;
    end;

    //Write uncrossed numbers, they are prime
    for i:=1 to n do
      if Nums[i] = False then
        writeln(i,' - простое число');
  end;

  Readln;
End.
