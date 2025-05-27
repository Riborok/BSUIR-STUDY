unit Main;

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants, System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.ExtCtrls, Vcl.StdCtrls, BinaryTree, Generics.Collections,
  Vcl.Menus, Vcl.ToolWin, Vcl.ComCtrls, Firmware;

type
  TMainASD = class(TForm)
    Memo: TMemo;
    ScrollBox: TScrollBox;
    PaintBox: TPaintBox;
    tbarMenu: TToolBar;
    btnAdd: TButton;
    btnDelete: TButton;
    btnPreOrder: TButton;
    btnInOrder: TButton;
    btnPostOrder: TButton;
    btnGenerate: TButton;
    btnClear: TButton;
    btnFirmware: TButton;
    btnRemFirmware: TButton;
    procedure FormCreate(Sender: TObject);
    procedure PaintBoxPaint(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure btnAddClick(Sender: TObject);
    procedure btnDeleteClick(Sender: TObject);
    procedure ScrollBoxMouseWheel(Sender: TObject; Shift: TShiftState;
      WheelDelta: Integer; MousePos: TPoint; var Handled: Boolean);
    procedure TraversalClick(Sender: TObject);
    procedure btnGenerateClick(Sender: TObject);
    procedure btnClearClick(Sender: TObject);
    procedure btnFirmwareClick(Sender: TObject);
    procedure btnRemFirmwareClick(Sender: TObject);
  private
    BinaryTree: TBinaryTree;
    isStitched: Boolean;
    function HandleInput(out Data: Integer; const Task: string): Boolean;
    function GetLeftCenter(const Node: TNode; const AllWidth: TDictionary<Integer, Integer>): Integer;
    function GetRightCenter(const Node: TNode; const AllWidth: TDictionary<Integer, Integer>): Integer;
    procedure DrawTree(const Canvas: TCanvas; const Node: TNode;
      const X, Y: Integer; const AllWidth: TDictionary<Integer, Integer>);
    function GetMaxWidthHeight(const Node: TNode;
      const AllWidth: TDictionary<Integer, Integer>): Integer;
    procedure TraversalAction(const Node: TNode; const isInterimStep: boolean);
    procedure RedrawTree(const Canvas: TCanvas);
  private
    const
      NodeRadius = 25;
      Y_Indent = NodeRadius + 42;
      X_Indent = NodeRadius + 42;
  public
    { Public declarations }
  end;

var
  MainASD: TMainASD;


implementation

{$R *.dfm}

procedure TMainASD.FormCreate(Sender: TObject);
var
  I: Integer;
begin
  Memo.Clear();
  Randomize();
  BinaryTree := TBinaryTree.Create();
  isStitched := False;
end;

procedure TMainASD.FormDestroy(Sender: TObject);
begin
  BinaryTree.Destroy;
end;

procedure TMainASD.TraversalAction(const Node: TNode; const isInterimStep: boolean);
var
  I: Integer;
  PrevColor: TColor;
  Str: string;
begin
  if (Node <> nil) then
  begin
    PrevColor := Node.Color;
    if (isInterimStep)  then
    begin
      Node.Color := RGB(255, 255, 220);
      Str := IntToStr(Node.Data);
    end
    else
    begin
      Node.Color := clYellow;
      Str := '<' + IntToStr(Node.Data) + '>';
    end;

    if Memo.Lines.Count = 0 then
      Memo.Lines.Add(Str)
    else
      Memo.Lines[Memo.Lines.Count - 1] := Memo.Lines[Memo.Lines.Count - 1] +
        ' ' + Str;
    RedrawTree(PaintBox.Canvas);
    Node.Color := PrevColor;
  end
  else
  begin
    Memo.Lines[Memo.Lines.Count - 1] := Memo.Lines[Memo.Lines.Count - 1] + ' 0';
    RedrawTree(PaintBox.Canvas);
  end;

  for I := 1 to 42 do
  begin
    Application.ProcessMessages();
    Sleep(10);
  end;

end;

function TMainASD.GetRightCenter(const Node: TNode; const AllWidth: TDictionary<Integer, Integer>): Integer;
begin
  if (Node.Left <> nil) then
  begin
    if (AllWidth.ContainsKey(Node.Data)) then
    begin
      if (AllWidth.ContainsKey(Node.Left.Data)) then
        Result := AllWidth[Node.Left.Data]
      else
        Result := X_Indent;
      Inc(Result, NodeRadius shl 1);
    end
    else
      Result := NodeRadius shl 1
  end
  else
    Result := X_Indent;
end;

function TMainASD.GetLeftCenter(const Node: TNode; const AllWidth: TDictionary<Integer, Integer>): Integer;
begin
  if (Node.Right <> nil) then
  begin
    if (AllWidth.ContainsKey(Node.Data)) then
    begin
      if (AllWidth.ContainsKey(Node.Right.Data)) then
        Result := AllWidth[Node.Right.Data]
      else
        Result := X_Indent;
      Inc(Result, NodeRadius shl 1);
    end
    else
      Result := NodeRadius shl 1
  end
  else
    Result := X_Indent;
end;

procedure TMainASD.RedrawTree(const Canvas: TCanvas);
var
  AllWidth: TDictionary<Integer, Integer>;
  Width, Height: Integer;
begin
  if (BinaryTree.Root <> nil) then
  begin
    AllWidth := TDictionary<Integer, Integer>.Create;
    Height := GetMaxWidthHeight(BinaryTree.Root, AllWidth);
    Width := AllWidth[BinaryTree.Root.Data];

    PaintBox.Width := Width + X_Indent;
    PaintBox.Height := Height + Y_Indent;

    DrawTree(Canvas, BinaryTree.Root,
      GetRightCenter(BinaryTree.Root, AllWidth), Y_Indent, AllWidth);
    AllWidth.Destroy;
  end
  else
  begin
    PaintBox.Width := X_Indent;
    PaintBox.Height := Y_Indent;
  end;
end;

procedure TMainASD.PaintBoxPaint(Sender: TObject);
begin
  RedrawTree(PaintBox.Canvas);
end;


procedure TMainASD.TraversalClick(Sender: TObject);
begin
  if (not isStitched) then
  begin
    Memo.Clear;
    case TButton(Sender).Tag of
      0: BinaryTree.PreOrderTraversal(TraversalAction);
      1: BinaryTree.InOrderTraversal(TraversalAction);
      2: BinaryTree.PostOrderTraversal(TraversalAction);
    end;
    PaintBox.Invalidate;
  end;
end;

procedure TMainASD.ScrollBoxMouseWheel(Sender: TObject; Shift: TShiftState;
  WheelDelta: Integer; MousePos: TPoint; var Handled: Boolean);
const
  ScrollStep = 42 shl 1;
begin
  if ssShift in Shift then
  begin
    if WheelDelta > 0 then
      ScrollBox.HorzScrollBar.Position := ScrollBox.HorzScrollBar.Position - ScrollStep
    else
      ScrollBox.HorzScrollBar.Position := ScrollBox.HorzScrollBar.Position + ScrollStep;
  end
  else
  begin
    if WheelDelta > 0 then
      ScrollBox.VertScrollBar.Position := ScrollBox.VertScrollBar.Position - ScrollStep
    else
      ScrollBox.VertScrollBar.Position := ScrollBox.VertScrollBar.Position + ScrollStep;
  end;
end;

function TMainASD.HandleInput(out Data: Integer; const Task: string): Boolean;
var
  str: string;
begin
  str := InputBox('Enter Number', Task, '');

  Result := False;
  if str <> '' then
  begin
    if TryStrToInt(str, Data) then
      Result := True
    else
      ShowMessage('Enter a valid number.');
  end;
end;

procedure TMainASD.btnAddClick(Sender: TObject);
var
  Data: Integer;
begin
  if (not isStitched and HandleInput(Data, 'Enter the number:')) then
  begin
    BinaryTree.Insert(Data);
    PaintBox.Invalidate;
  end;
end;

procedure TMainASD.btnClearClick(Sender: TObject);
begin
    BinaryTree.Clear();
    isStitched := False;
    PaintBox.Invalidate;
end;

procedure TMainASD.btnRemFirmwareClick(Sender: TObject);
begin
  if (isStitched) then
  begin
    isStitched := False;
    BinaryTree.RemoveFirmware();
    PaintBox.Invalidate;
  end;
end;

procedure TMainASD.btnDeleteClick(Sender: TObject);
var
  Data: Integer;
begin
  if (HandleInput(Data, 'Enter the number:')) then
  begin
    if (isStitched) then
    begin
      isStitched := False;
      BinaryTree.RemoveFirmware();
      BinaryTree.Delete(Data);
      btnFirmwareClick(Sender);
    end
    else
      BinaryTree.Delete(Data);

    PaintBox.Invalidate;
  end;
end;

procedure TMainASD.btnFirmwareClick(Sender: TObject);
var
  Firmware: TFirmware;
begin
  if (not isStitched) then
  begin
    isStitched := True;
    Firmware := TFirmware.Create();
    BinaryTree.InOrderFirmware(Firmware.Action);
    Firmware.Action(BinaryTree.Root);
    Firmware.Destroy;
    PaintBox.Invalidate;
  end;
end;

procedure TMainASD.btnGenerateClick(Sender: TObject);
var
  Amount, I: Integer;
begin
  if (not isStitched and HandleInput(Amount, 'Enter amount of numbers:')) then
  begin
    for I := 1 to Amount do
      BinaryTree.Insert(Random(19999) - 9999);
    PaintBox.Invalidate;
  end;
end;

procedure TMainASD.DrawTree(const Canvas: TCanvas;
  const Node: TNode; const X, Y: Integer; const AllWidth: TDictionary<Integer, Integer>);
var
  NextX, NextY: Integer;
  procedure DrawNode(const Canvas: TCanvas; const Node: TNode; const X, Y: Integer);
  begin
    Canvas.Ellipse(X - NodeRadius, Y - NodeRadius, X + NodeRadius, Y + NodeRadius);

    Canvas.TextOut(X - Canvas.TextWidth(IntToStr(Node.Data)) shr 1,
      Y - Canvas.TextHeight(IntToStr(Node.Data)) shr 1,
      IntToStr(Node.Data));
  end;
begin
  if (not AllWidth.ContainsKey(Node.Data)) then
  begin
    Canvas.Brush.Color := clBlue;
    DrawNode(Canvas, Node, X, Y);
    Exit;
  end;

  Canvas.Brush.Color := Node.Color;
  DrawNode(Canvas, Node, X, Y);

  AllWidth.Remove(Node.Data);

  if Node.Left <> nil then
  begin
    NextX := X - GetLeftCenter(Node.Left, AllWidth);
    NextY := Y + Y_Indent;
    Canvas.MoveTo(X, Y + NodeRadius);
    Canvas.LineTo(NextX, NextY);
    DrawTree(Canvas, Node.Left, NextX, NextY, AllWidth);
  end;

  if Node.Right <> nil then
  begin
    NextX := X + GetRightCenter(Node.Right, AllWidth);
    NextY := Y + Y_Indent;
    Canvas.MoveTo(X, Y + NodeRadius);
    Canvas.LineTo(NextX, NextY);
    DrawTree(Canvas, Node.Right, NextX, NextY, AllWidth);
  end;
end;

function Max(const Num1, Num2: Integer): Integer;
begin
  Result := Num1;
  if Num2 > Result then
    Result := Num2;
end;

function TMainASD.GetMaxWidthHeight(const Node: TNode;
  const AllWidth: TDictionary<Integer, Integer>): Integer;
var
  Width: Integer;
  LeftHeight, RightHeight: Integer;
begin
  if (AllWidth.ContainsKey(Node.Data)) then
    Exit(Y_Indent);

  AllWidth.Add(Node.Data, X_Indent);

  LeftHeight := 0;
  RightHeight := 0;

  Width := X_Indent;
  Result := Y_Indent;

  if (Node.Left <> nil) then
  begin
    LeftHeight := GetMaxWidthHeight(Node.Left, AllWidth);

    Inc(Width, AllWidth[Node.Left.Data]);
  end;

  if (Node.Right <> nil) then
  begin
    RightHeight := GetMaxWidthHeight(Node.Right, AllWidth);

    Inc(Width, AllWidth[Node.Right.Data]);
  end;

  AllWidth.AddOrSetValue(Node.Data, Width);
  Result := Max(Result + LeftHeight, Result + RightHeight);
end;

end.
