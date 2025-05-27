unit uTypes;

interface
uses
  System.SysUtils;

type
  // Represents the preferences for a node
  TPreference = record
    minAge, maxAge: Integer;
    minHeight, maxHeight: Integer;
    minWeight, maxWeight: Integer;
  end;

  // Represents the data stored in each node of the linked list
  TNodeData = record
    SerialNum: Integer;
    Name: ShortString;
    Age: Integer;
    Height: Integer;
    Weight: Integer;
    Habits: ShortString;
    Hobby: ShortString;
    isMale: Boolean;
    Preference: TPreference;
  end;

  // Pointer to a node in the linked list
  PNode = ^TNode;

  // Represents a node in the linked list
  TNode = record
    Data: TNodeData;
    Prev: PNode;
    Next: PNode;
  end;

  // Function type for comparing two TNodeData instances
  TCompareFunction = function(const AFirst, ASecond: TNodeData): Boolean;

  // Doubly linked list class
  TDoublyLinkedList = class
  private
    FHead: PNode;
    FTail: PNode;
    function CreateNode(const AData: TNodeData): PNode;
  public
    constructor Create;
    destructor Destroy; override;
    property Head: PNode read FHead;
    procedure AddNode(const AData: TNodeData);
    procedure RemoveNode(const ANode: PNode);
    procedure Clear;
    function FindNodeByName(const AName: string): PNode;
    procedure SortByParameter(Compare: TCompareFunction);
  end;

  // Comparison functions for sorting the linked list based on different criteria
  function CompareBySerialNumAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareBySerialNumDesc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByNameAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByNameDesc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByAgeAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByAgeDesc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByHeightAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByHeightDesc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByWeightAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByWeightDesc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByHabitsAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByHabitsDesc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByHobbyAsc(const AFirst, ASecond: TNodeData): Boolean;
  function CompareByHobbyDesc(const AFirst, ASecond: TNodeData): Boolean;

implementation

  constructor TDoublyLinkedList.Create;
  begin
    inherited;
    FHead := nil;
    FTail := nil;
  end;

  destructor TDoublyLinkedList.Destroy;
  begin
    Clear;
    inherited;
  end;

  procedure TDoublyLinkedList.SortByParameter(Compare: TCompareFunction);
  var
    Current, Running, Support: PNode;
    TempData: TNodeData;
    Swapped: Boolean;
  begin
    // Start with the head node
    Current := FHead;

    repeat
      // Initialize variables
      Swapped := False;
      Support := Current;
      Running := Current.Next;

      while Running <> nil do
      begin
        if Compare(Running.Data, Support.Data) then
          // Update the support node if the running node's data should come before
          Support := Running;

        // Move to the next node
        Running := Running.Next;
      end;

      if Support <> Current then
      begin
        // Swap the data between the current node and the support node
        TempData := Current.Data;
        Current.Data := Support.Data;
        Support.Data := TempData;
        // Set the swapped flag to true
        Swapped := True;
      end;

      // Move to the next node
      Current := Current.Next;

    // Repeat until no more swaps are made
    until not Swapped;
  end;

  function TDoublyLinkedList.CreateNode(const AData: TNodeData): PNode;
  begin
    // Allocate memory for a new node
    New(Result);
    // Set the data of the new node
    Result^.Data := AData;
    // Initialize the previous pointer as nil
    Result^.Prev := nil;
    // Initialize the next pointer as nil
    Result^.Next := nil;
  end;

  procedure TDoublyLinkedList.AddNode(const AData: TNodeData);
  var
    NewNode: PNode;
  begin
    // Create a new node with the given data
    NewNode := CreateNode(AData);
    if FHead = nil then
    begin
      // If the list is empty, set the new node as both the head and tail
      FHead := NewNode;
      FTail := NewNode;
    end
    else
    begin
      // Append the new node after the current tail
      FTail^.Next := NewNode;
      // Update the previous pointer of the new node
      NewNode^.Prev := FTail;
      // Update the tail to the new node
      FTail := NewNode;
    end;
  end;

  procedure TDoublyLinkedList.RemoveNode(const ANode: PNode);
  begin
    if ANode = FHead then
      // Update the head if the node to be removed is the head
      FHead := ANode^.Next;
    if ANode = FTail then
      // Update the tail if the node to be removed is the tail
      FTail := ANode^.Prev;
    if ANode^.Prev <> nil then
      // Update the next pointer of the previous node
      ANode^.Prev^.Next := ANode^.Next;
    if ANode^.Next <> nil then
      // Update the previous pointer of the next node
      ANode^.Next^.Prev := ANode^.Prev;

    // Deallocate the memory occupied by the removed node
    Dispose(ANode);
  end;

  procedure TDoublyLinkedList.Clear;
  var
    CurrentNode: PNode;
    NextNode: PNode;
  begin
    CurrentNode := FHead;
    while CurrentNode <> nil do
    begin
      NextNode := CurrentNode^.Next;
      // Deallocates the memory occupied by the current node
      Dispose(CurrentNode);
      CurrentNode := NextNode;
    end;

    // Resets the head (first) node to nil
    FHead := nil;
    // Resets the tail (last) node to nil
    FTail := nil;
  end;

  function TDoublyLinkedList.FindNodeByName(const AName: string): PNode;
  var
    CurrentNode: PNode;
  begin
    // Initialize the result to nil
    Result := nil;

    // Start searching from the head (first) node
    CurrentNode := FHead;
    while CurrentNode <> nil do
    begin
      // Check if the name of the current node matches the desired name
      if CurrentNode^.Data.Name = AName then
      begin
        // Store the reference to the current node as the result
        Result := CurrentNode;

        // Exit the loop since the node has been found
        Break;
      end;

      // Move to the next node in the list
      CurrentNode := CurrentNode^.Next;
    end;
  end;

  function CompareBySerialNumAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.SerialNum < ASecond.SerialNum;
  end;

  function CompareBySerialNumDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.SerialNum > ASecond.SerialNum;
  end;

  function CompareByNameAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Name < ASecond.Name;
  end;

  function CompareByNameDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Name > ASecond.Name;
  end;

  function CompareByAgeAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Age < ASecond.Age;
  end;

  function CompareByAgeDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Age > ASecond.Age;
  end;

  function CompareByHeightAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Height < ASecond.Height;
  end;

  function CompareByHeightDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Height > ASecond.Height;
  end;

  function CompareByWeightAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Weight < ASecond.Weight;
  end;

  function CompareByWeightDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Weight > ASecond.Weight;
  end;

  function CompareByHabitsAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Habits < ASecond.Habits;
  end;

  function CompareByHabitsDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Habits > ASecond.Habits;
  end;

  function CompareByHobbyAsc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Hobby < ASecond.Hobby;
  end;

  function CompareByHobbyDesc(const AFirst, ASecond: TNodeData): Boolean;
  begin
    Result := AFirst.Hobby > ASecond.Hobby;
  end;

end.
