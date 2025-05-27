program Iterative;
{
  Finds the whole sequence of hooks
  Finds the entire hook sequence. "è" gives 2 hooks, "ø" - 3 hooks
}

uses
  System.SysUtils, uStack in 'uStack.pas';

// Finds the whole sequence of hooks
procedure OutputAllHooks(AHooksAmount: Integer; var ACount: Integer);
type
  TStackItem = record
    AmountHooks: Integer;
    Sequence: string;
  end;
  // TStackItem - stack element
var
  Stack: TStack<TStackItem>;
  Item: TStackItem;
  NewItem: TStackItem;
  // Stack - store the intermediate states
  // Item - current stack item being processed
  // NewItem - new stack item to be pushed
begin
  // Create the stack
  Stack := TStack<TStackItem>.Create;

  // Initialization for the first iteration
  NewItem.AmountHooks := AHooksAmount;
  NewItem.Sequence := '';
  Stack.Push(NewItem);

  // Finding all sequences
  while Stack.Count > 0 do
  begin
    Item := Stack.Pop;

    if Item.AmountHooks >= 3 then
    begin
      // Decrease the remaining hooks by 3
      NewItem.AmountHooks := Item.AmountHooks - 3;

      // Append 'ø' to the sequence
      NewItem.Sequence := Item.Sequence + 'ø ';

      // Push the new item to the stack for further processing
      Stack.Push(NewItem);
    end;

    if Item.AmountHooks >= 2 then
    begin
      // Decrease the remaining hooks by 2
      NewItem.AmountHooks := Item.AmountHooks - 2;

      // Append 'è' to the sequence
      NewItem.Sequence := Item.Sequence + 'è ';

      // Push the new item to the stack for further processing
      Stack.Push(NewItem);
    end
    else if Item.AmountHooks = 0 then
    begin
      // Output the sequence if there are no more hooks
      Writeln(Item.Sequence);

      // Increment the count of valid sequences
      Inc(ACount);
    end;
  end;

  // Destroy the stack
  Stack.Destroy;
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
  OutputAllHooks(HooksAmount, Count);

  // Output sequence count
  Writeln('Total sequences: ', count);

  Readln;
end.
