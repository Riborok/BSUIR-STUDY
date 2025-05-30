Program Dijkstra;
{
 Enter the labyrinth, 0 - the cell is passable, 1 - the cell is impassable.
 Possible to move between cells that have a common side. Find closest way
}

{$APPTYPE CONSOLE}

uses
  System.SysUtils;

Const
  Convert = '123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
  MinSizes = 4;
  MaxSizes = length(Convert);
  //Convert - storing values 1..35 to exchange between symbols and their values and vice versa
  //MinSizes - minimal allowable sizes in a labyrinth
  //MaxSizes - maximum allowable sizes in a labyrinth

Var
  Lab, Way : array [1..MaxSizes, 1..MaxSizes] of Byte;
  SizeI, SizeJ, StartI, StartJ, i, j : Byte;
  CurrNumStep, CoordExitI, CoordExitJ : Byte;
  flag: Boolean;
  //Lab - an array that stores the entered labyrinth
  //Way - an array that stores path to the exit
  //SizeI - entered size by lines
  //SizeJ - entered size by columns
  //StartI - start coordinates by lines
  //StartJ - start coordinates by columns
  //i,j - cycle counters
  //CurrNumStep - current number step in the Way
  //CoordExitI - nearest exit coordinate (i)
  //CoordExitJ - nearest exit coordinate (j)
  //flag - flag to confirm the correctness of entering numbers



procedure Input;
var
  LargerSize, i, j : Byte;
  StrStartCoords : string[4];
  //LargerSize - the largest value of the sizes
  //i,j - cycle counters
  //StrStartCoords - string stores the symbols value of the starting coordinates
begin
  Writeln('Enter the size of the labyrinth (i j), i and j belongs to ',MinSizes,'..',MaxSizes);

  //Cycle with postcondition for entering correct data.
  Repeat

    //Initialize the flag
    flag:= False;

    //Validating the correct input data type
    Try
      Readln(SizeI, SizeJ);
    Except
      Writeln('Invalid data type entered');
      flag:= True;
    End;

    //Validate Range
    if (not (SizeI in [MinSizes..MaxSizes]) or not (SizeJ in [MinSizes..MaxSizes])) and not flag then
    begin
      Writeln('(i j) do not belong to the range!');
      flag:= True;
    end;

  Until not flag;

  Writeln;
  Writeln('Enter the labyrinth. 0 - the cell is passable, 1 - the cell is impassable.');
  Writeln('Possible to move between cells that have a common side');

  //Finding the largest size
  if SizeI > SizeJ then
    LargerSize:= SizeI
  else
    LargerSize:= SizeJ;

  //If the largest size >= 10, inform the user about the replacements
  if LargerSize >= 10 then
  begin
    Writeln;
    Writeln('For convenience, numbers consisting of two digits will be represented as follows:');
    for i := 10 to LargerSize do
      Writeln(Convert[i],' = ',i);
  end;

  //Writing columns and boundaries for understanding
  Writeln;
  Write('  ');
  for j := 1 to SizeJ do
    Write(Convert[j],' ');
  Writeln;
  Write('  ');
  for j := 1 to SizeJ do
    Write('__');
  Writeln;

  //Cycle for reading a labyrinth (line)
  i:= 1;
  while (i <= SizeI) and not flag do
  begin

    //Write the line number
    Write(Convert[i],'|');

    //Cycle for reading a labyrinth (column)
    j:= 1;
    while (j <= SizeJ) and not flag do
    begin

      //Validating the correct input data type
      Try
        Read(Lab[i,j]);
      Except
        Writeln('Invalid data type entered! Restart the program');
        flag:= True;
      End;

      //Validate Range
      if (Lab[i,j] <> 0) and (Lab[i,j] <> 1) then
      begin
        Writeln('Number do not belong to the range! Restart the program');
        flag:= True;
      end;

      //Modernize j
      Inc(j);
    end;

    //Modernize i
    Inc(i);
  end;

  //Input validation
  if not flag then
  begin

    Writeln;
    Writeln('Enter start position (i j). This position must be in a passable cell (0)');
    Readln;

    //Cycle with postcondition for entering correct data.
    Repeat

      //Initialize the flag
      flag:= False;

      Readln(StrStartCoords);

      //Validating the correct input data type
      StartI:= Pos(StrStartCoords[1], Convert);
      StartJ:= Pos(StrStartCoords[3], Convert);
      if (StartI = 0) or (StartJ = 0) or (length(StrStartCoords) <> 3) then
      begin
        Writeln('Invalid data type entered!');
        flag:= True;
      end

      //Checking, the position must be in the labyrinth
      else if (StartI > SizeI) or (StartJ > SizeJ) then
      begin
        Writeln('Position not in the labyrinth!');
        flag:= True;
      end

      //Checking, the position must be in a passable cell
      else if Lab[StartI, StartJ] <> 0 then
      begin
        Writeln('Position not in a passable cell!');
        flag:= True;
      end;

    Until not flag ;

  end;
end;



//Procedure to writing the path
procedure PathOutput(CoordI, CoordJ: Byte);
var
  PrevNumStep: Byte;
  //PrevNumStep - previous number step in the Way
begin

  //Find the previous number step in the Way
  //to find previous coordinates in the path
  PrevNumStep:= Way[CoordI, CoordJ] - 1;

  //Looking for a path to the starting cell
  if (CoordI <> StartI) or (CoordJ <> StartJ) then
  begin
    if Way[CoordI, CoordJ-1] = PrevNumStep then
      PathOutput(CoordI, CoordJ-1)
    else if Way[CoordI-1, CoordJ] = PrevNumStep then
      PathOutput(CoordI-1, CoordJ)
    else if Way[CoordI, CoordJ+1] = PrevNumStep then
      PathOutput(CoordI, CoordJ+1)
    else if Way[CoordI+1, CoordJ] = PrevNumStep then
      PathOutput(CoordI+1, CoordJ);
  end;

  //Write coordinates
  Write('(',Convert[CoordI],',',Convert[CoordJ],') ');

end;



//Procedure for finding a path
procedure DijkstraClosestWay(CoordI, CoordJ: Byte);
var
  NextNumStep: Byte;
  //NextNumStep - next number step in the Way
begin

  //Increase CurrNumStep and add it to the array Way at the current coordinates
  Inc(CurrNumStep);
  NextNumStep:= CurrNumStep + 1;
  Way[CoordI,CoordJ]:= CurrNumStep;

  //�hecking for a nearest exit
  if ((CoordI = 1) or (CoordJ = 1) or (CoordI = SizeI) or (CoordJ = SizeJ)) and (CurrNumStep < Way[CoordExitI,CoordExitJ]) then
  begin
    CoordExitI:= CoordI;
    CoordExitJ:= CoordJ;
  end

  //Else looking for an neighboring, available cell.
  //Also look for the shortest path to the cell. And if found, go into it
  else
  begin
    if (Lab[CoordI, CoordJ+1] = 0) and (Way[CoordI, CoordJ+1] > NextNumStep) then
      DijkstraClosestWay(CoordI, CoordJ+1);
    if (Lab[CoordI+1, CoordJ] = 0) and (Way[CoordI+1, CoordJ] > NextNumStep) then
      DijkstraClosestWay(CoordI+1, CoordJ);
    if (Lab[CoordI, CoordJ-1] = 0) and (Way[CoordI, CoordJ-1] > NextNumStep) then
      DijkstraClosestWay(CoordI, CoordJ-1);
    if (Lab[CoordI-1, CoordJ] = 0) and (Way[CoordI-1, CoordJ] > NextNumStep) then
      DijkstraClosestWay(CoordI-1, CoordJ);
  end;

  //Next, decrease Dec and exit the current cell to the previous
  Dec(CurrNumStep);

end;



Begin

  //Call the procedure to write data
  Input;

  //If the labyrinth is entered correctly, then looking for a path
  if not flag then
  begin

    //According to Dijkstra's algorithm, assume that initially all cells
    //can be reached by an infinitely long path
    for i := 1 to SizeI do
      for j := 1 to SizeJ do
        Way[i,j]:= 255;

    //Also assume that initially the exit coordinates are 1,1 (since this cell
    //in the labyrinth does not make sense) so that the path length is infinitely large
    CoordExitI:= 1;
    CoordExitJ:= 1;

    //Initialize the variables and go to the procedure DijkstraClosestWay
    CurrNumStep:= 0;
    DijkstraClosestWay(StartI, StartJ);

    //Checking if the path is found (if the expected exit is 1,1 - the path isnt found)
    if (CoordExitI = 1) and (CoordExitJ = 1) then
      Writeln('Entered labyrinth has no way out')
    else
    begin
      Writeln('Found the nearest way. Amount of steps: ',Way[CoordExitI,CoordExitJ]);
      PathOutput(CoordExitI, CoordExitJ);
    end;
  end;

  Readln;
  Readln;
End.
