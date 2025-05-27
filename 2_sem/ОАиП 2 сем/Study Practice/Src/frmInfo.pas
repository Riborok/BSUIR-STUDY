unit frmInfo;

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants, System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.StdCtrls, uTypes;

type
  TInfo = class(TForm)
    lbName: TLabel;
    lbAge: TLabel;
    lbHeight: TLabel;
    lbWeight: TLabel;
    lbHabits: TLabel;
    lbHobby: TLabel;
    lbOptions: TLabel;
    lbPreference: TLabel;
    lbMinAge: TLabel;
    lbMaxAge: TLabel;
    lbMinHeight: TLabel;
    lbMaxHeight: TLabel;
    lbMinWeight: TLabel;
    lbMaxWeight: TLabel;

    edtName: TEdit;
    edtAge: TEdit;
    edtHeight: TEdit;
    edtWeight: TEdit;
    edtHabits: TEdit;
    edtHobby: TEdit;
    edtMinAge: TEdit;
    edtMaxAge: TEdit;
    edtMinHeight: TEdit;
    edtMaxHeight: TEdit;
    edtMinWeight: TEdit;
    edtMaxWeight: TEdit;
    btnOK: TButton;
    btnCancel: TButton;

    cbGender: TComboBox;
    lbGender: TLabel;
    procedure FormKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
    procedure FormShow(Sender: TObject);
  private const
    InvalidInput = 'Invalid input. Please enter valid data.';
  public
    { Public declarations }
    function TryGetNodeData(var ANodeData: TNodeData): Boolean;
  end;

var
  Info: TInfo;

implementation

{$R *.dfm}

  procedure TInfo.FormKeyDown(Sender: TObject; var Key: Word; Shift: TShiftState);
  begin
    // Check if the Escape key was pressed.
    if Key = VK_ESCAPE then
      // Set the modal result of the form to mrCancel, indicating that the form should be closed with a cancel result.
      ModalResult := mrCancel
    // Check if the Return key was pressed without the Shift key being held.
    else if (Key = VK_RETURN) and not (ssShift in Shift) then
      // Set the modal result of the form to mrOk, indicating that the form should be closed with an OK result.
      ModalResult := mrOk;
  end;

  procedure TInfo.FormShow(Sender: TObject);
  begin
    // Center the form on the screen.
    Left := (Screen.Width - Width) shr 1;
    Top := (Screen.Height - Height) shr 1;
    // Set the focus to the edtName control, allowing the user to start typing immediately.
    edtName.SetFocus;
  end;

  function TInfo.TryGetNodeData(var ANodeData: TNodeData): Boolean;
  var
    Age, Height, Weight: Integer;
    minAge, maxAge, minHeight, maxHeight, minWeight, maxWeight: Integer;
  begin
    // Populate the form fields with the values from ANodeData
    edtName.Text := ANodeData.Name;
    edtHabits.Text:= ANodeData.Habits;
    edtHobby.Text:= ANodeData.Hobby;
    cbGender.ItemIndex := Ord(ANodeData.isMale);

    edtAge.Text:= IntToStr(ANodeData.Age);
    edtHeight.Text:= IntToStr(ANodeData.Height);
    edtWeight.Text:= IntToStr(ANodeData.Weight);
    edtMinAge.Text:= IntToStr(ANodeData.Preference.minAge);
    edtMaxAge.Text:= IntToStr(ANodeData.Preference.maxAge);
    edtMinHeight.Text:= IntToStr(ANodeData.Preference.minHeight);
    edtMaxHeight.Text:= IntToStr(ANodeData.Preference.maxHeight);
    edtMinWeight.Text:= IntToStr(ANodeData.Preference.minWeight);
    edtMaxWeight.Text:= IntToStr(ANodeData.Preference.maxWeight);

    repeat
      ShowModal;

      if ModalResult = MrOk then
      begin
        Result:= True;

        // Try to convert the input values to integers
        if TryStrToInt(edtAge.Text, Age) and
           TryStrToInt(edtHeight.Text, Height) and
           TryStrToInt(edtWeight.Text, Weight) and
           TryStrToInt(edtMinAge.Text, minAge) and
           TryStrToInt(edtMaxAge.Text, maxAge) and
           TryStrToInt(edtMinHeight.Text, minHeight) and
           TryStrToInt(edtMaxHeight.Text, maxHeight) and
           TryStrToInt(edtMinWeight.Text, minWeight) and
           TryStrToInt(edtMaxWeight.Text, maxWeight) then
        begin
          // Update ANodeData with the validated input values
          ANodeData.Age := Age;
          ANodeData.Height := Height;
          ANodeData.Weight := Weight;
          ANodeData.Preference.minAge := minAge;
          ANodeData.Preference.maxAge := maxAge;
          ANodeData.Preference.minHeight := minHeight;
          ANodeData.Preference.maxHeight := maxHeight;
          ANodeData.Preference.minWeight := minWeight;
          ANodeData.Preference.maxWeight := maxWeight;
          ANodeData.Name := edtName.Text;
          ANodeData.Habits := edtHabits.Text;
          ANodeData.Hobby := edtHobby.Text;
          ANodeData.isMale := Boolean(cbGender.ItemIndex);
        end
        else
        begin
          // If the input values are invalid, show an error message and retry
          ModalResult := mrRetry;
          ShowMessage(InvalidInput);
        end;

      end
      else if ModalResult = mrCancel then
        Result:= False;

    until ModalResult <> mrRetry;

  end;

end.
