Program homework2_var1;
{Find all natural numbers (up to 100), that when squared, give a palindrome}

{$APPTYPE CONSOLE}

Const
  LastNum = 100;
  //LastNum - last number to check

Var
  i, Inverted, Rem, Square : integer;
  {i - cycle counter (current number)
  Inverted - current inverted number
  Rem - the remainder to be added to Inverted
  Square - square current number}

Begin

  //Iterate numbers
  for i:=1 to LastNum do
  begin

    //Find the square current number
    Square:=i*i;

    //Find the remainder to be added to Inverted
    Rem:=Square;

    //Initialize the current inverted number
    Inverted:=0;

    //Start inverting. Go until the remainder > 0
    while Rem>0 do
    begin

      //Find inverted number
      Inverted:=Inverted*10 + Rem mod 10;

      //Find the remainder
      Rem:=Rem div 10;
    end;

    //If the square of the number is equal to the inverted square, then this gives a palindrome
    if Square=Inverted then
      writeln('Number ',i,' that when squared, give a palindrome (',Square,')');
  end;

  Readln;
End.
