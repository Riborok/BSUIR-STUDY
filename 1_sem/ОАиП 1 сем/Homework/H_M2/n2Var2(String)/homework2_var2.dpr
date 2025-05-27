Program homework2_var2;
{Find all natural numbers (up to 100), that when squared, give a palindrome}

{$APPTYPE CONSOLE}
Uses
  System.SysUtils;

Const
  LastNum = 100;
  //LastNum - last number to check

Var
  i, j, Square, len, half : integer;
  flag :boolean;
  {i - cycle counter (current number)
  j - cycle counter (current element)
  Square - square current number
  len - array (string) length
  half - half the length of a number
  flag - the flag will be true if found inconsistencies for a palindrome}

Begin

  //Iterate numbers
  for i:=1 to LastNum do
  begin

    //Find the square current number
    Square:=i*i;

    //Find the array (string) length 
    len:=length(IntToStr(Square));

    //Reset the flag for the current iteration
    flag:=False;

    //If length=1 then the number gives a palindrome
    if len = 1 then
      writeln('Number ',i,' that when squared, give a palindrome (',Square,')')

    //Else looking for numbers that cause a palindrome
    //Split the string in half and compare the elements
    else
    begin
      j:=1;
      half:= len div 2;
      while (j<=half) and (flag=False) do
      begin

        //If some element is not equal, exit the loop
        if (IntToStr(Square)[j] <> IntToStr(Square)[len - j + 1]) then
          flag:=True

        //If compared all the elements, then found the number that gives the palidrome
        else if j = half then
          writeln('Number ',i,' that when squared, give a palindrome (',Square,')');

        //Modernize j
        j:= j+1;
      end;
    end;
  end;

  Readln;
End.
