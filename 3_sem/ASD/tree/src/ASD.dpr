program ASD;

uses
  Vcl.Forms,
  Main in 'Main.pas' {MainASD},
  BinaryTree in 'BinaryTree.pas',
  Firmware in 'Firmware.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TMainASD, MainASD);
  Application.Run;
end.
