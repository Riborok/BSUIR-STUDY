unit BinaryTree;

interface

uses
  Generics.Collections, Vcl.Graphics, Windows;

type

  TNode = class
  private
    FData: Integer;
    FLeft, FRight: TNode;
    FColor: TColor;
  public
    property Data: Integer read FData;
    property Left: TNode read FLeft;
    property Right: TNode read FRight write FRight;
    property Color: TColor read FColor write FColor;
    constructor Create(const Data: Integer);
  end;

  TNodeAction = procedure(const Node: TNode; const isInterimStep: boolean) of object;
  TFirmwareAction = procedure(const Node: TNode) of object;

  TBinaryTree = class
  private
    FRoot: TNode;
    procedure InsertRec(const Node: TNode; const Data: Integer);
    function DeleteRec(const Node: TNode; const Data: Integer): TNode;
    function DeleteMin(const Node: TNode): Integer;
    procedure FreeTree(const Node: TNode; const isUsed: TDictionary<Integer, Integer>);
    procedure InOrderTraversalRec(const Node: TNode; const Action: TNodeAction);
    procedure PreOrderTraversalRec(const Node: TNode; const Action: TNodeAction);
    procedure PostOrderTraversalRec(const Node: TNode; const Action: TNodeAction);
    procedure RemoveFirmwareRec(const Node: TNode; const isUsed: TDictionary<Integer, Integer>);
    procedure InOrderFirmwareRec(const Node: TNode; const Action: TFirmwareAction);
  public
    destructor Destroy; override;
    constructor Create;
    procedure Insert(const Data: Integer);
    procedure Delete(const Data: Integer);
    procedure Clear();
    property Root: TNode read FRoot;
    procedure RemoveFirmware();
    procedure InOrderTraversal(const Action: TNodeAction);
    procedure PreOrderTraversal(const Action: TNodeAction);
    procedure PostOrderTraversal(const Action: TNodeAction);

    procedure InOrderFirmware(const Action: TFirmwareAction);
  end;

implementation

constructor TNode.Create(const Data: Integer);
begin
  FData := Data;
  FColor := clWhite;
  FLeft := nil;
  FRight := nil;
end;

procedure TBinaryTree.FreeTree(const Node: TNode; const isUsed: TDictionary<Integer, Integer>);
begin
  if ((Node <> nil) and not isUsed.ContainsKey(Node.Data)) then
  begin
    isUsed.Add(Node.Data, -1);
    FreeTree(Node.FLeft, isUsed);
    FreeTree(Node.FRight, isUsed);
    Node.Destroy;
  end;
end;

destructor TBinaryTree.Destroy;
var
  isUsed: TDictionary<Integer, Integer>;
begin
  isUsed := TDictionary<Integer, Integer>.Create();
  FreeTree(FRoot, isUsed);
  isUsed.Destroy;
  inherited;
end;

constructor TBinaryTree.Create;
begin
  FRoot := nil;
end;

procedure TBinaryTree.RemoveFirmware();
var
  isUsed: TDictionary<Integer, Integer>;
begin
  isUsed := TDictionary<Integer, Integer>.Create();
  RemoveFirmwareRec(FRoot, isUsed);
  isUsed.Destroy;
end;

procedure TBinaryTree.RemoveFirmwareRec(const Node: TNode; const isUsed: TDictionary<Integer, Integer>);
begin
  if (Node <> nil) then
  begin
    isUsed.Add(Node.Data, -1);
    RemoveFirmwareRec(Node.FLeft, isUsed);

    if (isUsed.ContainsKey(Node.FRight.Data)) then
      Node.FRight := nil
    else
      RemoveFirmwareRec(Node.FRight, isUsed);
  end;
end;

procedure TBinaryTree.Insert(const Data: Integer);
begin
  if (FRoot = nil) then
    FRoot := TNode.Create(Data)
  else
    InsertRec(FRoot, Data);
end;

procedure TBinaryTree.InsertRec(const Node: TNode; const Data: Integer);
begin
  if (Data < Node.FData) then
  begin
    if (Node.FLeft = nil) then
      Node.FLeft := TNode.Create(Data)
    else
      InsertRec(Node.FLeft, Data);
  end
  else if (Data > Node.FData) then
  begin
    if (Node.FRight = nil) then
      Node.FRight := TNode.Create(Data)
    else
      InsertRec(Node.FRight, Data);
  end;
end;

procedure TBinaryTree.Clear();
var
  isUsed: TDictionary<Integer, Integer>;
begin
  isUsed := TDictionary<Integer, Integer>.Create();
  FreeTree(FRoot, isUsed);
  isUsed.Destroy;
  FRoot := nil;
end;

function TBinaryTree.DeleteMin(const Node: TNode): Integer;
var
  Parent, CurrNode: TNode;
begin
  Parent := nil;
  CurrNode := Node;

  while CurrNode.FLeft <> nil do
  begin
    Parent := CurrNode;
    CurrNode := CurrNode.FLeft;
  end;

  if (Parent <> nil) then
    Parent.FLeft := CurrNode.FRight;

  Result := CurrNode.FData;
  CurrNode.Destroy;
end;

procedure TBinaryTree.Delete(const Data: Integer);
begin
  FRoot := DeleteRec(FRoot, Data);
end;

function TBinaryTree.DeleteRec(const Node: TNode; const Data: Integer): TNode;
begin
  if (Node = nil) then
    Result := nil
  else
  begin
    Result := Node;
    if (Data < Node.FData) then
      Node.FLeft := DeleteRec(Node.FLeft, Data)
    else if (Data > Node.FData) then
      Node.FRight := DeleteRec(Node.FRight, Data)
    else
    begin
      if (Node.FRight = nil) then
      begin
        Result := Node.FLeft;
        Node.Destroy;
      end
      else if (Node.FLeft = nil) then
      begin
        Result := Node.FRight;
        Node.Destroy;
      end
      else
      begin
        Node.FData := DeleteMin(Node.FRight);
      end;
    end;
  end;
end;

procedure TBinaryTree.InOrderFirmware(const Action: TFirmwareAction);
begin
  InOrderFirmwareRec(FRoot, Action);
end;

procedure TBinaryTree.InOrderFirmwareRec(const Node: TNode; const Action: TFirmwareAction);
begin
  if (Node <> nil) then
  begin
    InOrderFirmwareRec(Node.FLeft, Action);
    Action(Node);
    InOrderFirmwareRec(Node.FRight, Action);
  end;
end;

procedure TBinaryTree.InOrderTraversal(const Action: TNodeAction);
begin
  InOrderTraversalRec(FRoot, Action);
end;

procedure TBinaryTree.InOrderTraversalRec(const Node: TNode; const Action: TNodeAction);
begin
  if (Node <> nil) then
  begin
    Action(Node, True);
    InOrderTraversalRec(Node.FLeft, Action);
    Action(Node, False);
    InOrderTraversalRec(Node.FRight, Action);
    Action(Node, True);
  end
  else
    Action(nil, True);
end;

procedure TBinaryTree.PostOrderTraversal(const Action: TNodeAction);
begin
  PostOrderTraversalRec(FRoot, Action);
end;

procedure TBinaryTree.PostOrderTraversalRec(const Node: TNode; const Action: TNodeAction);
begin
  if (Node <> nil) then
  begin
    Action(Node, True);
    PostOrderTraversalRec(Node.FLeft, Action);
    Action(Node, True);
    PostOrderTraversalRec(Node.FRight, Action);
    Action(Node, False);
  end
  else
    Action(nil, True);
end;

procedure TBinaryTree.PreOrderTraversal(const Action: TNodeAction);
begin
  PreOrderTraversalRec(FRoot, Action);
end;

procedure TBinaryTree.PreOrderTraversalRec(const Node: TNode; const Action: TNodeAction);
begin
  if (Node <> nil) then
  begin
    Action(Node, False);
    PreOrderTraversalRec(Node.FLeft, Action);
    Action(Node, True);
    PreOrderTraversalRec(Node.FRight, Action);
    Action(Node, True);
  end
  else
    Action(nil, True);
end;

end.
