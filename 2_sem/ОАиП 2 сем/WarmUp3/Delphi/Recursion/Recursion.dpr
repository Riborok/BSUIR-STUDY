program Recursion;
{
  Finds the whole sequence of hooks
  Finds the entire hook sequence. "è" gives 2 hooks, "ø" - 3 hooks
}

uses
  SysUtils;

// Finds the whole sequence of hooks
procedure OutputAllHooks(const AHooksAmount: Integer; const ASequence: string; var ACount: Integer);
begin
  if AHooksAmount >= 3 then
    // Recursively find sequences with three hooks
    OutputAllHooks(AHooksAmount - 3, ASequence + 'ø ', ACount);

  if AHooksAmount >= 2 then
    // Recursively find sequences with two hooks
    OutputAllHooks(AHooksAmount - 2, ASequence + 'è ', ACount)
  else if AHooksAmount = 0 then
  begin
    // Output the sequence if there are no more hooks
    Writeln(ASequence);

    // Increment the count of valid sequences
    Inc(ACount);
  end;
end;

// Returns the correct number of hooks
function GetAmountOfHooks: Integer;
const
  Min = 2;
  Max = 1000;
  // Maximum and minimum allowed value for the number of hooks
var
  IsCorrect: Boolean;
  // Flag to indicate if the input is correct
begin
  Writeln('Enter amount of hooks: ');

  // Repeat the loop until the input is correct
  repeat
    IsCorrect := True;
    try
      // Read the user input and assign it to the function result
      Readln(Result);
    except
      // Output an error message for invalid input
      IsCorrect := False;
      Writeln('Incorrect input');
    end;

    if ((Result < Min) or (Result > Max)) and IsCorrect then
    begin
      IsCorrect := False;

      // Output an error message for invalid input
      Writeln('Incorrect input');
    end;

  until IsCorrect;
  Writeln;
end;

var
  Count: Integer;
  HooksAmount : Integer;
begin

  // Initialize the count of valid sequences
  Count:= 0;

  // Finds the whole sequence of hooks
  HooksAmount := GetAmountOfHooks;
  Writeln('Sequences:');
  OutputAllHooks(HooksAmount, '', Count);

  // Output sequence count
  Writeln('Total sequences: ', count);

  Readln;
end.
