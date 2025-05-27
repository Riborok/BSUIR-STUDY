program ASD;

uses
  Vcl.Forms,
  Main in 'Main.pas' {Graph},
  GraphUtils in 'GraphUtils.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TGraph, Graph);
  Application.Run;
end.
