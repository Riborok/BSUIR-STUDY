Program homework3Rep;
{The program devide the number into 3 terms (with repetitions)}

{$APPTYPE CONSOLE}

Var
  Num, Terms1, Terms2, Terms3 : integer;
  {Num - entered number
  Terms1 - first term
  Terms2 - second term
  Terms3 - third term}

Begin

  //Number must be >=3, since the terms cannot be equal to 0
  Writeln('Enter number (must be >=3)');
  Readln(Num);

  if Num<3 then
    Writeln('Number must be >=3. Restart the programm')

  else
  begin

    //Iterate over the first term, from 1 to the number - 2
    //(since the other two terms must be greater than 0)
    for Terms1:=1 to (Num-2) do

      //Iterate over the first term, from 1 to the number - 1
      //(since the last terms must be greater than 0)
      for Terms2:=1 to (Num-Terms1-1) do
      begin

        //Find the third term by subtracting the first and second from the number
        Terms3:=Num - Terms1 - Terms2;
        writeln(Terms1, ' ', Terms2, ' ', Terms3);
      end;
  end;

Readln;
End.
