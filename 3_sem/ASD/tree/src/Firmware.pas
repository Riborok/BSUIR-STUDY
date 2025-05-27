unit Firmware;

interface

uses
  BinaryTree, Vcl.Graphics;

type
  TFirmware = class
  private
    FPrev: TNode;
  public
    constructor Create();
    procedure Action(const Node: TNode);
  end;

implementation

constructor TFirmware.Create();
begin
  FPrev := nil;
end;

procedure TFirmware.Action(const Node: TNode);
begin
  if (FPrev <> nil) then
  begin
    FPrev.Right	:= Node;
    FPrev := nil;
  end;
  if (Node.Right = nil) then
    FPrev := Node;
end;

end.
