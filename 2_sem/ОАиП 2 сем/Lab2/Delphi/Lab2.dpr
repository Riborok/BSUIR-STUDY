program Lab2;
{
 This program performs an analysis of the time complexity of binary
 insertion sort and selection sort algorithms. It generates arrays of
 random, sorted, and reverse-ordered elements and calculates the theoretical
 and practical number of comparisons for each sorting algorithm on each array
 type. The results are displayed in a table. The user can choose to recalculate
 the program.
}

{$APPTYPE CONSOLE}
uses
  System.SysUtils,
  uSortAnalyze in 'uSortAnalyze.pas';

const
  MinAmOfElements = 3;
  ÑontinuationStr = 'Yes';
  // MinAmOfElements - minimum amount of elements in the array
  // ÑontinuationStr - string for continue cycle

type

  TNumOfComp = record
    BinaryInsertionPract: Integer;
    SelectionPract: Integer;
  end;

  TRes = record
    Rand, Sorted, Reversed: TNumOfComp;
    BinaryInsertionTheor, SelectionTheor : Integer;
  end;

  TArrays = record
    RandArr, SortedArr, ReversedArr : TArray;
  end;

  // TNumOfComp - record with a number of comparisons
  // TRes - record with results for each type of array
  // TArrays - record with arrays

var
  Arrays: TArrays;
  AmOfElements : Integer;
  IsCorrect : Boolean;
  Ñhoice : string;
  Res: TRes;
  // Arrays - record with arrays
  // AmOfElements - number of elements in the array
  // IsCorrect - flag for validation of input data
  // Ñhoice - string for continue cycle
  // Res - record with results

// Fills all the required arrays up to AAmOfElements - 1 element
function GetArrays(AAmOfElements : Integer): TArrays;
var
  I: Integer;
begin
  Randomize;
  Dec(AAmOfElements);
  for I := 0 to AAmOfElements do
  begin
    Result.RandArr[I] := Random(AAmOfElements);
    Result.SortedArr[I] := I;
    Result.ReversedArr[AAmOfElements - I] := I;
  end;
end;

begin

  repeat
    Write('Enter the number of elements in the array. No more than ');
    Writeln(High(TArray) + 1,' and no less than ', MinAmOfElements);

    //Cycle with postcondition for entering correct data.
    repeat

      //Initialize the flag
      IsCorrect:= True;

      //Validating the correct input data type
      try
        Readln(AmOfElements);
      except
        Writeln('Wrong input of amount of numbers! It must be an integer');
        IsCorrect:= False;
      end;

      //Validate Range
      if ((AmOfElements > High(TArray) + 1) or (AmOfElements < MinAmOfElements)) and IsCorrect then
      begin
        Write('Wrong input of amount of numbers! It must be No more than ');
        Writeln(High(TArray) + 1,' and no less than ', MinAmOfElements);
        IsCorrect:= False;
      end;
    until IsCorrect;

    Arrays := GetArrays(AmOfElements);

    // Calculations
    Res.BinaryInsertionTheor := CalcBinaryInsertionTheor(AmOfElements);
    Res.SelectionTheor := CalcSelectionTheor(AmOfElements);

    Res.Rand.BinaryInsertionPract := CalcBinaryInsertionPract(Arrays.RandArr, AmOfElements);
    Res.Rand.SelectionPract := CalcSelectionPract(Arrays.RandArr, AmOfElements);

    Res.Sorted.BinaryInsertionPract := CalcBinaryInsertionPract(Arrays.SortedArr, AmOfElements);
    Res.Sorted.SelectionPract := CalcSelectionPract(Arrays.SortedArr, AmOfElements);

    Res.Reversed.BinaryInsertionPract := CalcBinaryInsertionPract(Arrays.ReversedArr, AmOfElements);
    Res.Reversed.SelectionPract := CalcSelectionPract(Arrays.ReversedArr, AmOfElements);

    //Outputting results in table form
    Writeln(' _________________________________________________________________________________');
    Writeln('|                    |            | Binary insertion sort |     Selection sort    |');
    Writeln('| Amount of elements | Array type |_______________________|_______________________|');
    Writeln('|                    |            | Practical | Theorical | Practical | Theorical |');

    //Part with random
    Writeln('|____________________|____________|___________|___________|___________|___________|');
    Writeln('|', AmOfElements:14, '      |  Random    |', Res.Rand.BinaryInsertionPract:8, '   |', Res.BinaryInsertionTheor:8, '   |', Res.Rand.SelectionPract:8, '   |', Res.SelectionTheor:8, '   |');

    //Part with sorted
    Writeln('|____________________|____________|___________|___________|___________|___________|');
    Writeln('|                    |  Sorted    |', Res.Sorted.BinaryInsertionPract:8, '   |', Res.BinaryInsertionTheor:8, '   |', Res.Sorted.SelectionPract:8, '   |', Res.SelectionTheor:8, '   |');

    //Part with reverse
    Writeln('|____________________|____________|___________|___________|___________|___________|');
    Writeln('|                    |  Reversed  |', Res.Reversed.BinaryInsertionPract:8, '   |', Res.BinaryInsertionTheor:8, '   |', Res.Reversed.SelectionPract:8, '   |', Res.SelectionTheor:8, '   |');
    Writeln('|____________________|____________|___________|___________|___________|___________|');

    // Give the opportunity to continue the calculations
    Writeln;
    Writeln('Do you want to recalculate? If yes, please enter ''',ÑontinuationStr,'''');
    Readln(Ñhoice);
  until Ñhoice <> ÑontinuationStr;
end.
