Program homework1_;
{The program must find a natural number from 1 to n with the maximum sum of divisors}

{$APPTYPE CONSOLE}

Const
  ExtremePoint = 100000;
  //ExtremePoint - last valid value n

Var
  n, i, k, CurrNum, MaxSum, MaxNum : integer;
  DenNums : array [1..ExtremePoint] of integer;
  {n - finish number of range
  i - cycle counter
  k - multiplier factor
  CurrNum - current number
  MaxSum - maximum sum of divisors
  MaxNum - maximum number with sum of divisors
  DenNums - an array in which the sum of the divisors of a number is stored}

Begin
  Writeln('Enter the last number of range (number must be >=1 and <=', ExtremePoint, ')');
  Readln(n);

  if (n<1) or (n>ExtremePoint) then
    Writeln('Number must be >=1 and <=',ExtremePoint,'. Restart the programm')

  else
  begin

    //If the user entered n=1, then the largest number will remain 1
    MaxNum:= 1;

    //Iterate over divisors from 2 to n
    for i := 2 to n do
    begin

      //Initialize the variable for the cycle
      k:=1;
      CurrNum:= i;

      //The cycle continues while the current number <= n
      while CurrNum <= n do
      begin

        //Add the current divisor
        DenNums[CurrNum]:= DenNums[CurrNum] + i;

        //Finding the number with the largest sum of divisors
        if DenNums[CurrNum]>MaxSum then
        begin
          MaxNum:= CurrNum;
          MaxSum:= DenNums[CurrNum];
        end;

        //Modernize to the next iteration
        k:= k+1;
        CurrNum:= i*k;

      end;

    end;

    //Since all numbers have a divisor of 1, programm was not taken into account when comparing.
    //In answer write with +1
    writeln('A number in the range 1..',n,' with maximum sum of divisors: ',MaxNum,'. The sum of the divisors:', (MaxSum + 1));
  end;

  Readln;
End.
