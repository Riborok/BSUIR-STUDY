unit Main;

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants, System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.StdCtrls, Vcl.ToolWin, Vcl.ComCtrls,
  Vcl.Grids, Vcl.ExtCtrls, GraphUtils;

type
  TGraph = class(TForm)
    SG: TStringGrid;
    tbarMenu: TToolBar;
    btnSTV: TButton;
    Splitter1: TSplitter;
    pnl: TPanel;
    mm: TMemo;
    btnFind: TButton;
    Splitter2: TSplitter;
    Image1: TImage;
    Button1: TButton;
    procedure btnSTVClick(Sender: TObject);
    procedure SGDrawCell(Sender: TObject; ACol, ARow: Integer; Rect: TRect;
      State: TGridDrawState);
    procedure FormCreate(Sender: TObject);
    procedure SGSetEditText(Sender: TObject; ACol, ARow: Integer;
      const Value: string);
    procedure btnFindClick(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    AdjacencyMatrix: TMatrix;
    function Draw(const Center: Integer):TBitmap;
  public
    { Public declarations }
  end;
  TPoint = record
      Name:Byte;
      X,Y:Integer;
    end;

var
  Graph: TGraph;

const
  Infinity = 100000;
  Rad= 23;
  ArrowLen = 15;
  ArrowRot= Pi/12;

implementation

{$R *.dfm}

procedure DrawPoint(var bmp:TBitmap; Point:TPoint);
begin
  bmp.Canvas.Ellipse(Point.X - Rad ,Point.Y - Rad,Point.X + Rad ,Point.Y + Rad);
  bmp.Canvas.FloodFill(Point.X,Point.Y,clBlack,fsBorder);
  bmp.Canvas.TextOut(Point.X - 4,Point.Y - 14,IntToStr(Point.Name));
end;

function TGraph.Draw(const Center: Integer):TBitmap;
var
  BmpRad:Integer;
  AngleStep:Real;
  i,j:Integer;
  Points:array of TPoint;
  X1,X2,Y1,Y2,X3,Y3, OOO, AAA, S:Integer;
  Phi:Real;
begin
  SetLength(Points, Length(AdjacencyMatrix));
  Result:=TBitmap.Create;
  S := Length(AdjacencyMatrix) * 25;
  Result.Height:= S + 150;
  Result.Width:= S + 150;
  Result.Canvas.Font.Height := 25;
  Result.Canvas.Brush.Color:= clWhite;

  BmpRad:= (S div 2) - Rad + 40;
  AngleStep:= 2*Pi/(High(AdjacencyMatrix));

  j:=0;
  for i:= 0 to High(AdjacencyMatrix) do
  begin
    if i= Center Then Continue;
    Points[i].Name:=i;
    Points[i].X:=BmpRad + 2*Rad + 10 + Round(BmpRad * Cos(Pi /10   + j*AngleStep));
    Points[i].Y:=BmpRad + Rad + 5 - Round(BmpRad * Sin(Pi /10  + j*AngleStep));
    DrawPoint(Result, Points[i]);
    j:=j+1;
  end;
  Points[Center].Name:=Center;

  OOO := 0;
  if Center = 0 then
    OOO := 1;

  AAA := Length(Points) div 2 + OOO;
  if AAA >= Length(Points) then
    AAA := High(Points);

  Points[Center].X:= ((Points[OOO].X + Points[AAA].X) div 2) - Rad div 2;
  Points[Center].Y:= ((Points[OOO].Y + Points[AAA].Y) div 2) - Rad div 2;
  if Length(Points) < 3 then
  begin
    Points[Center].X:= Points[OOO].X - Rad * 3 div 2;
    Points[Center].Y:= Points[OOO].Y - Rad * 3 div 2;
  end;

  DrawPoint(Result, Points[Center]);

  for i:= 0 to High(AdjacencyMatrix) do
    for j:= 0 to High(AdjacencyMatrix) do
      if (AdjacencyMatrix[i,j] <> Infinity) and (AdjacencyMatrix[i, j] <> 0)then
      begin
        if Abs(Points[i].Y - Points[j].Y) < Rad*2 then
        begin
          Y1:=Points[i].Y;
          Y2:=Points[j].Y;
          if Points[i].X < Points[j].X then
          begin
            X1:=Points[i].X + Rad;
            X2:=Points[j].X - Rad
          end
          else
          begin
            X1:=Points[i].X - Rad;
            X2:=Points[j].X + Rad
          end
        end
        else
        begin
          X1:=Points[i].X;
          X2:=Points[j].X;
          if Points[i].Y < Points[j].Y then
          begin
            Y1:=Points[i].Y + Rad;
            Y2:=Points[j].Y - Rad
          end
          else
          begin
            Y1:=Points[i].Y - Rad;
            Y2:=Points[j].Y + Rad
          end
        end;

        Result.Canvas.MoveTo(X1,Y1);
        Result.Canvas.LineTo(X2,Y2);


        Phi:=ArcTan(Abs(Y1-Y2) / Abs(X1-X2));
        if Y2-Y1 < 0 then
          Phi:=-Phi;
        if X2-X1 > 0 then
          Phi:=Pi - Phi;
        X1:=X2 + Round(ArrowLen*Cos(Phi-ArrowRot));
        Y1:=Y2 - Round(ArrowLen*Sin(Phi-ArrowRot));
        Result.Canvas.LineTo(X1,Y1);
        X3:=X2 + Round(ArrowLen*Cos(Phi+ArrowRot));
        Y3:=Y2 - Round(ArrowLen*Sin(Phi+ArrowRot));
        Result.Canvas.MoveTo(X2,Y2);
        Result.Canvas.LineTo(X3,Y3);
        Result.Canvas.LineTo(X1,Y1);

        X1:=X2 + Round(ArrowLen*3*Cos(Phi));
        Y1:=Y2 - Round(ArrowLen*3*Sin(Phi));
        Result.Canvas.Brush.Color:= RGB(192,255,253);
        Result.Canvas.TextOut(X1,Y1,IntToStr(AdjacencyMatrix[i,j]));
        Result.Canvas.Brush.Color:=clWhite;
      end;
end;

procedure TGraph.btnFindClick(Sender: TObject);
var
  fromVertexStr, toVertexStr: string;
  fromVertex, toVertex, i, j: Integer;
  Paths: TPaths;
begin
  fromVertexStr := InputBox('Find', 'Enter the starting vertex:', '');
  toVertexStr := InputBox('Find', 'Enter the target vertex:', '');
  if (fromVertexStr = '') or (toVertexStr = '') then
    Exit;

  if TryStrToInt(fromVertexStr, fromVertex) and TryStrToInt(toVertexStr, toVertex) and
    (fromVertex >= 0) and (fromVertex < Length(AdjacencyMatrix)) and
    (toVertex >= 0) and (toVertex < Length(AdjacencyMatrix)) then
  begin


    mm.Lines[0] := 'Center: ' + IntToStr(FindCenter(AdjacencyMatrix));

    Paths := FindAllPaths(AdjacencyMatrix, fromVertex, toVertex);
    if (Paths.Count = 0) then
    begin
      Paths.Destroy;
      Exit;
    end;

    mm.Lines[1] := 'Shortest Path: (' + IntToStr(Paths[0].Distance) + ') ';
    for i := 0 to Paths[0].Vertices.Count - 1 do
      mm.Lines[1] := mm.Lines[1] + IntToStr(Paths[0].Vertices[i]) + ' ';

    mm.Lines[2] := 'Longest Path: (' + IntToStr(Paths[Paths.Count-1].Distance) + ') ';
    for i := 0 to Paths[Paths.Count-1].Vertices.Count - 1 do
      mm.Lines[2] := mm.Lines[2] + IntToStr(Paths[Paths.Count-1].Vertices[i]) + ' ';

    mm.Lines[3] := '';
    for i := 0 to Paths.Count - 1 do
    begin
      mm.Lines[i + 4] := IntToStr(i + 1) + ': (' + IntToStr(Paths[i].Distance) + ') ';
        for j := 0 to Paths[i].Vertices.Count - 1 do
          mm.Lines[i + 4] := mm.Lines[i + 4] + IntToStr(Paths[i].Vertices[j]) + ' ';
    end;

    for i := 0 to Paths.Count - 1 do
      Paths[i].Vertices.Destroy;
    Paths.Destroy;
  end
  else
  begin
    ShowMessage('Invalid input');
  end;
end;

procedure TGraph.btnSTVClick(Sender: TObject);
var
  VertexCountStr: string;
  VertexCount, i, j: Integer;
begin
  VertexCountStr := InputBox('Vertex Count', 'Enter the amount of vertices:', '');

  if VertexCountStr <> '' then
  begin
    if TryStrToInt(VertexCountStr, VertexCount) then
    begin
      if VertexCount > 1 then
      begin
        SetLength(AdjacencyMatrix, VertexCount, VertexCount);

        Inc(VertexCount);
        SG.RowCount := VertexCount;
        SG.ColCount := VertexCount;

        for i := 1 to VertexCount do
          for j := 1 to VertexCount do
            if SG.Cells[i, j] = '' then
              SG.Cells[i, j] := '0';
        Image1.Picture:= TPicture(Draw(FindCenter(AdjacencyMatrix)));
      end
      else
        ShowMessage('Invalid input. Enter a positive integer number greater than zero');
    end
    else
      ShowMessage('Invalid input. Enter a valid integer number');
  end;
end;


procedure TGraph.Button1Click(Sender: TObject);
begin
  for var I := 0 to High(AdjacencyMatrix) do
    for var J := 0 to High(AdjacencyMatrix[0]) do
    begin
      AdjacencyMatrix[I, J] := 0;
      SG.Cells[I + 1, J + 1] := '0';
    end;
  try
      for var I := 0 to High(AdjacencyMatrix) do
        for var J := 0 to High(AdjacencyMatrix[0]) do
        begin
          var a: Integer := Random(60) - 50;
          if a >= 0 then
          begin
            AdjacencyMatrix[I, J] := a;
            SG.Cells[I + 1, J + 1] := inttostr(a);
          end;
        end;
    SG.Invalidate;
    Image1.Picture:= TPicture(Draw(FindCenter(AdjacencyMatrix)));
  except
    Button1Click(Sender);
  end;
end;

procedure TGraph.FormCreate(Sender: TObject);
var
  i, j, VertexCount: Integer;
begin
  mm.Lines.Clear;
  for i := 1 to 1000 do
    mm.Lines.Add('');

  VertexCount := SG.RowCount;
  for i := 1 to VertexCount do
    for j := 1 to VertexCount do
      if SG.Cells[i, j] = '' then
        SG.Cells[i, j] := '0';
  SetLength(AdjacencyMatrix, VertexCount - 1, VertexCount - 1);
end;

procedure TGraph.SGDrawCell(Sender: TObject; ACol, ARow: Integer; Rect: TRect;
  State: TGridDrawState);
var
  CellText: string;
begin
  if (ACol = 0) and (ARow > 0) then
  begin
    CellText := IntToStr(ARow - 1);
    SG.Canvas.FillRect(Rect);
    SG.Canvas.TextOut(Rect.Left + 2, Rect.Top + 2, CellText);
  end
  else if (ARow = 0) and (ACol > 0) then
  begin
    CellText := IntToStr(ACol - 1);
    SG.Canvas.FillRect(Rect);
    SG.Canvas.TextOut(Rect.Left + 2, Rect.Top + 2, CellText);
  end;
end;

procedure TGraph.SGSetEditText(Sender: TObject; ACol, ARow: Integer;
  const Value: string);
var
  Weight: Integer;
begin
  if Value = '' then
    Exit;

  if TryStrToInt(Value, Weight) and (Weight >= 0) then
  begin
    AdjacencyMatrix[ACol - 1, ARow - 1] := Weight;
    Image1.Picture:= TPicture(Draw(FindCenter(AdjacencyMatrix)));
  end
  else
  begin
    ShowMessage('Invalid input. Enter a valid integer positive number');
    SG.Cells[ACol, ARow] := '0';
  end;
end;

end.
