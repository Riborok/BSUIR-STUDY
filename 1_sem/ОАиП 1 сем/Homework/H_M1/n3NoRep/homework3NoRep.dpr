Program homework3NoRep;
{The program devide the number into 3 terms (no repetitions)}

{$APPTYPE CONSOLE}

Const
  ExtremePoint = 10000;
  //ExtremePoint - last valid value number

Var
  Num, Terms1, Terms2, Terms3, MaxTerms, MinTerms : integer;
  AlreadyFound:  array[1..ExtremePoint, 1..ExtremePoint] of Boolean;
  {Num - entered number
  Terms1 - the first term
  Terms2 - the second term
  Terms3 - the third term
  MaxTerms - maximum value of terms in cycle
  MinTerms - minimum value of terms in cycle
  AlreadyFound - array in which cross out the combitations which were}


Begin

  //Number must be >=3, since the terms cannot be equal to 0
  Writeln('Enter number (must be >=3 and <=',ExtremePoint,')');
  Readln(Num);

  if (Num<3) or (Num>ExtremePoint) then
    Writeln('Number must be >=3 and <=',ExtremePoint,'. Restart the programm')

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

        //Find the maximum and minimum value of terms in cycle
        if (Terms1 >= Terms2) then
        begin
          MaxTerms := Terms1;
          MinTerms := Terms2;
        end
        else
        begin
          MaxTerms := Terms2;
          MinTerms := Terms1;
        end;
        if (Terms3 > MaxTerms) then
          MaxTerms:=Terms3
        else if (Terms3 < MinTerms) then
          MinTerms:=Terms3;

        //Check, if this combination was not cross out
        if AlreadyFound[MaxTerms][MinTerms]=False then
        begin

          //Then write this combination and cross out in array
          writeln(Terms1, ' ', Terms2, ' ', Terms3);
          AlreadyFound[MaxTerms][MinTerms] := True;
        end;
      end;
  end;

Readln;
End.
