unit uStack;

interface

type
  // Stack
  TStack<T> = class
  private type
    PItem = ^TItem;
    TItem = record
      FData: T;
      FNext: PItem;
    end;
    // Item - stack element
  private
    FTop: PItem;
    FCount: Integer;
    // FTop - top of the stack
    // FCount - amount of items in the stack
  public
    constructor Create;
    destructor Destroy; override;

    // Pushes an item onto the stack
    procedure Push(const AItem: T);

    // Clears the stack
    procedure Clear;

    // Pops an item from the stack
    function Pop: T;

    // Returns the item at the top of the stack without removing it
    function Peek: T;

    // Number of items in the stack
    property Count: Integer read FCount;
  end;

implementation

  constructor TStack<T>.Create;
  begin
    // Initialize top of the stack to nil
    FTop := nil;

    // Initialize count to 0
    FCount := 0;
  end;

  destructor TStack<T>.Destroy;
  begin
    // Clear the stack before destroying the object
    Clear;
    inherited;
  end;

  procedure TStack<T>.Clear;
  var
    I: Integer;
  begin
    // Pop each item from the stack to clear it
    for I := FCount - 1 downto 0 do
      Pop;
  end;

  procedure TStack<T>.Push(const AItem: T);
  var
    NewItem: PItem;
  begin
    // Create a new item for the stack
    New(NewItem);

    // Set the new item as the top of the stack
    NewItem^.FData := AItem;
    NewItem^.FNext := FTop;
    FTop := NewItem;

    // Increase the count of items in the stack
    Inc(FCount);
  end;

  function TStack<T>.Pop: T;
  var
    Item: PItem;
  begin
    // Get the top item from the stack
    Item := FTop;
    FTop := FTop^.FNext;

    // Get the data from the top item
    Result := Item^.FData;

    // Free the memory allocated for the top item
    Dispose(Item);

    // Decrease the count of items in the stack
    Dec(FCount);
  end;

  function TStack<T>.Peek: T;
  begin
    // Return the data from the top item without removing it
    Result := FTop^.FData;
  end;

end.

