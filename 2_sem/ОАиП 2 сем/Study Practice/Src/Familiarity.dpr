program Familiarity;

uses
  Vcl.Forms,
  frmMain in 'frmMain.pas' {Main},
  uTypes in 'uTypes.pas',
  frmInfo in 'frmInfo.pas' {Info};

{$R *.res}

begin
  Application.Initialize;
  Application.MainFormOnTaskbar := True;
  Application.CreateForm(TMain, Main);
  Application.CreateForm(TInfo, Info);
  Application.Run;
end.
