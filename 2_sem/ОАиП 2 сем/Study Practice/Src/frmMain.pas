unit frmMain;
{
  The program will have two lists: a list of grooms and a list of brides.
  Each candidate (groom or bride) in the list will be characterized by the
  following fields: candidate's serial number, candidate's information (name,
  age, height, weight, habits, hobbies), and partner preferences (in the form
  of a Min-Max range for age, height, and weight).
}

interface

uses
  Winapi.Windows, Winapi.Messages, System.SysUtils, System.Variants, System.Classes, Vcl.Graphics,
  Vcl.Controls, Vcl.Forms, Vcl.Dialogs, Vcl.ComCtrls, Vcl.Grids, Vcl.ExtCtrls,
  Vcl.StdCtrls, uTypes, frmInfo, Vcl.Imaging.jpeg, System.UITypes;

type
  TMain = class(TForm)
    Panel: TPanel;
    btnAdd: TButton;
    btnDelete: TButton;
    lvMale: TListView;
    lvFemale: TListView;
    btnEdit: TButton;
    lbMale: TLabel;
    lbFemale: TLabel;
    cbMale: TComboBox;
    cbFemale: TComboBox;
    btnMaleOffer: TButton;
    btnFemaleOffer: TButton;
    Image: TImage;
    SaveDialog: TSaveDialog;
    OpenDialog: TOpenDialog;
    btnOpen: TButton;
    btnSave: TButton;
    cbSort: TComboBox;
    cbSortGender: TComboBox;
    btnSort: TButton;
    procedure FormCreate(Sender: TObject);
    procedure btnAddClick(Sender: TObject);
    procedure btnDeleteClick(Sender: TObject);
    procedure btnEditClick(Sender: TObject);
    procedure lvMaleChange(Sender: TObject; Item: TListItem;
      Change: TItemChange);
    procedure lvFemaleChange(Sender: TObject; Item: TListItem;
      Change: TItemChange);
    procedure btnMaleOfferClick(Sender: TObject);
    procedure btnFemaleOfferClick(Sender: TObject);
    procedure SaveDataToFile(Sender: TObject);
    procedure LoadDataFromFile(Sender: TObject);
    procedure ImageClick(Sender: TObject);
  private const
    // The output message displayed when confirming a mutual liking
    Output = ', you liked the person. Click yes if you like it too.';

    // The file extension for the Oi files
    OIExt = '.oi';

    // The file filter description for the Open and Save dialogs
    FMOI = 'Oi Files (*' + OIExt + ')|*' + OIExt;

    // An array of sorting functions used for sorting the data.
    // Each function corresponds to a specific sorting criteria
    SortingFunctions: array[0..13] of TCompareFunction = (
      CompareBySerialNumAsc,
      CompareBySerialNumDesc,
      CompareByNameAsc,
      CompareByNameDesc,
      CompareByAgeAsc,
      CompareByAgeDesc,
      CompareByHeightAsc,
      CompareByHeightDesc,
      CompareByWeightAsc,
      CompareByWeightDesc,
      CompareByHabitsAsc,
      CompareByHabitsDesc,
      CompareByHobbyAsc,
      CompareByHobbyDesc
    );
  private
    { Private declarations }
    MaleList, FemaleList: TDoublyLinkedList;
    procedure UpdateMaleList;
    procedure UpdateFemaleList;
    class function CheckCompatibility(const First, Second: TNodeData): Boolean; static;
    class function CheckCompatibilityHelper(const First, Second: TNodeData): Boolean; static;
    class function GetNodeDataDescription(const NodeData: TNodeData): string; static;
  public
    { Public declarations }
    destructor Destroy; override;
  end;

var
  Main: TMain;

implementation

{$R *.dfm}

  // Updates the male list
  procedure TMain.UpdateMaleList;
  var
    CurrentNode: PNode;
    NodeData: TNodeData;
  begin
    lvMale.Clear;
    CurrentNode := MaleList.Head;

    // Clear the male list view before updating it.
    lvMale.Clear;

    // Traverse through each node in the MaleList.
    while CurrentNode <> nil do
    begin
      NodeData := CurrentNode.Data;

      // Add the name of the node data as an item in the male list view.
      lvMale.AddItem(NodeData.Name, nil);

      CurrentNode := CurrentNode.Next;
    end;
  end;

  // Updates the female list
  procedure TMain.UpdateFemaleList;
  var
    CurrentNode: PNode;
    NodeData: TNodeData;
  begin
    lvFemale.Clear;
    CurrentNode := FemaleList.Head;

    // Clear the female list view before updating it.
    lvFemale.Clear;

    // Traverse through each node in the FemaleList.
    while CurrentNode <> nil do
    begin
      NodeData := CurrentNode.Data;

      // Add the name of the node data as an item in the female list view.
      lvFemale.AddItem(NodeData.Name, nil);

      CurrentNode := CurrentNode.Next;
    end;
  end;

  // Event for deleting a node
  procedure TMain.btnDeleteClick(Sender: TObject);
  begin
    // Check if a male item is selected in the list view.
    if lvMale.Selected <> nil then
    begin
      // Remove the selected male node from the MaleList and delete the corresponding item from the list view.
      MaleList.RemoveNode(MaleList.FindNodeByName(lvMale.Selected.Caption));
      lvMale.Items.Delete(lvMale.Selected.Index);
      UpdateMaleList;
    end;

    // Check if a female item is selected in the list view.
    if lvFemale.Selected <> nil then
    begin
      // Remove the selected female node from the FemaleList and delete the corresponding item from the list view.
      FemaleList.RemoveNode(FemaleList.FindNodeByName(lvFemale.Selected.Caption));
      lvFemale.Items.Delete(lvFemale.Selected.Index);
      UpdateFemaleList;
    end;
  end;

  // Event for node change
  procedure TMain.btnEditClick(Sender: TObject);
  var
    Node: PNode;
    begin
    // Check if a male item is selected in the list view.
    if lvMale.Selected <> nil then
    begin
      // Find the selected male node in the MaleList.
      Node := MaleList.FindNodeByName(lvMale.Selected.Caption);
      if Info.TryGetNodeData(Node^.Data) then
        // Update the caption of the selected item in the male list
        // view with the updated name from the node data.
        lvMale.Selected.Caption := Node^.Data.Name;
    end;

    // Check if a female item is selected in the list view.
    if lvFemale.Selected <> nil then
    begin
      // Find the selected female node in the FemaleList.
      Node := FemaleList.FindNodeByName(lvFemale.Selected.Caption);
      if Info.TryGetNodeData(Node^.Data) then
        // Update the caption of the selected item in the female list
        // view with the updated name from the node data.
        lvFemale.Selected.Caption := Node^.Data.Name;
    end;
  end;

  // Destroy the main form
  destructor TMain.Destroy;
  begin
    // Destroy the MaleList and FemaleList instances
    MaleList.Destroy;
    FemaleList.Destroy;
  end;

  // Event for adding node
  procedure TMain.btnAddClick(Sender: TObject);
  var
    NodeData: TNodeData;
  begin
    // Initialize NodeData with the default values of TNodeData.
    NodeData := Default(TNodeData);

    // Try to get the node data from the Info component.
    if Info.TryGetNodeData(NodeData) then
    begin
      // Check the selected gender in the Info component.
      if Boolean(Info.cbGender.ItemIndex) then
      begin
        // Add the NodeData as a new node to the FemaleList.
        FemaleList.AddNode(NodeData);
        // Add a new item with the name of NodeData to the lvFemale list view.
        lvFemale.AddItem(NodeData.Name, nil);
      end
      else
      begin
        // Add the NodeData as a new node to the MaleList.
        MaleList.AddNode(NodeData);
        // Add a new item with the name of NodeData to the lvMale list view.
        lvMale.AddItem(NodeData.Name, nil);
      end;
    end;
  end;

  // Creating the main form
  procedure TMain.FormCreate(Sender: TObject);
  begin
    // Create instances of TDoublyLinkedList for MaleList and FemaleList.
    MaleList := TDoublyLinkedList.Create;
    FemaleList := TDoublyLinkedList.Create;

    // Set the default file extensions for save and open dialogs.
    SaveDialog.DefaultExt := OIExt;
    OpenDialog.DefaultExt := OIExt;

    // Set the filters for save and open dialogs to display only OI files.
    SaveDialog.Filter := FMOI;
    OpenDialog.Filter := FMOI;

    // Position the form at the center of the screen.
    Left := (Screen.Width - Width) shr 1;
    Top := (Screen.Height - Height) shr 1;
  end;

  class function TMain.CheckCompatibility(const First, Second: TNodeData): Boolean;
  begin
    // Check the compatibility between First and Second by calling the CheckCompatibilityHelper
    // for both combinations: First with Second and Second with First.
    Result := CheckCompatibilityHelper(First, Second) and CheckCompatibilityHelper(Second, First);
  end;


  class function TMain.CheckCompatibilityHelper(const First, Second: TNodeData): Boolean;
  begin
    // Check the compatibility between First and Second based on their
    // attributes and preferences. Return True if the attributes of
    // First (age, height, weight) fall within the range of preferences
    // specified
    Result := (First.Age >= Second.Preference.MinAge) and
              (First.Age <= Second.Preference.MaxAge) and
              (First.Height >= Second.Preference.MinHeight) and
              (First.Height <= Second.Preference.MaxHeight) and
              (First.Weight >= Second.Preference.MinWeight) and
              (First.Weight <= Second.Preference.MaxWeight);
  end;

  procedure TMain.btnFemaleOfferClick(Sender: TObject);
  var
    Answer, I: integer;
    MainNode, CurrNode: PNode;
  begin
    if cbFemale.ItemIndex > -1 then
    begin
      // Get the main female node based on the selected item in the lvFemale list view.
      MainNode := FemaleList.FindNodeByName(lvFemale.Selected.Caption);

      // Get the corresponding male node based on the selected item in the cbFemale combo box.
      CurrNode := MaleList.FindNodeByName(cbFemale.Items.Strings[cbFemale.ItemIndex]);

      // Display a message dialog to offer the male node to the female, showing relevant information.
      Answer := MessageDlg(CurrNode.Data.Name + Output + sLineBreak +
        GetNodeDataDescription(MainNode.Data), mtInformation, [mbYes, mbNo], 0);

      case Answer of
        mrYes:
        begin
          // If the female accepts the offer (clicked "Yes" button), proceed with the following actions:

          // Remove the female node from the FemaleList.
          FemaleList.RemoveNode(MainNode);

          // Delete the corresponding item from the lvFemale list view.
          lvFemale.Items.Delete(lvFemale.Selected.Index);

          // Iterate through the lvMale list view to find the item matching the male node.
          // Delete that item from the lvMale list view.
          for I := 0 to lvMale.Items.Count - 1 do
            if lvMale.Items[I].Caption = CurrNode.Data.Name then
            begin
              lvMale.Items.Delete(I);
              Break;
            end;

          // Remove the male node from the MaleList.
          MaleList.RemoveNode(CurrNode);

          // Update the male and female list views to reflect the changes.
          Self.UpdateMaleList;
          Self.UpdateFemaleList;
        end;
      end;
    end;
  end;

  procedure TMain.lvFemaleChange(Sender: TObject; Item: TListItem; Change: TItemChange);
  var
    I: Integer;
    MainNode, CurrNode: TNodeData;
  begin
    cbFemale.Items.Clear;

    // Check if there are items in lvFemale and lvMale lists, and if a female item is selected.
    if (lvFemale.Items.Count > 0) and (lvMale.Items.Count > 0) and (lvFemale.Selected <> nil) then
    begin
      // Retrieve the data of the selected female node.
      MainNode := FemaleList.FindNodeByName(lvFemale.Selected.Caption).Data;

      // Iterate through the lvMale items and check compatibility with the selected female node.
      for I := 0 to lvMale.Items.Count - 1 do
      begin
        // Retrieve the data of the current male node.
        CurrNode := MaleList.FindNodeByName(lvMale.Items[I].Caption).Data;

        // Check compatibility between the selected female node and the current male node.
        // If compatible, add the current male node's name to the cbFemale combo box.
        if CheckCompatibility(MainNode, CurrNode) then
          cbFemale.Items.Add(CurrNode.Name);
      end;
    end;

    // Trigger the lvMaleChange event to update the male compatibility information.
    if Sender <> nil then
      lvMaleChange(nil, Item, Change);
  end;

  procedure TMain.btnMaleOfferClick(Sender: TObject);
  var
    Answer, I: Integer;
    MainNode, CurrNode: PNode;
  begin
    // Check if an item is selected in the cbMale combo box.
    if cbMale.ItemIndex > -1 then
    begin
      // Retrieve the data of the selected male node.
      MainNode := MaleList.FindNodeByName(lvMale.Selected.Caption);

      // Retrieve the data of the female node selected in the cbMale combo box.
      CurrNode := FemaleList.FindNodeByName(cbMale.Items.Strings[cbMale.ItemIndex]);

      // Display a message dialog showing information about the male and female nodes.
      Answer := MessageDlg(CurrNode.Data.Name + Output + sLineBreak +
        GetNodeDataDescription(MainNode.Data), mtInformation, [mbYes, mbNo], 0);

      case Answer of
        // If the user chooses 'Yes', indicating acceptance:
        mrYes:
        begin
          // Remove the selected male node from the MaleList.
          MaleList.RemoveNode(MainNode);

          // Delete the selected male item from the lvMale list view.
          lvMale.Items.Delete(lvMale.Selected.Index);

          // Iterate through the items in the lvFemale list view.
          for I := 0 to lvFemale.Items.Count - 1 do
          begin
            // Check if the name of the current female item matches the name of the accepted female node.
            if lvFemale.Items[I].Caption = CurrNode.Data.Name then
            begin
              // Delete the current female item from the lvFemale list view.
              lvFemale.Items.Delete(I);
              Break;
            end;
          end;

          // Remove the accepted female node from the FemaleList.
          FemaleList.RemoveNode(CurrNode);

          // Update the male list view.
          Self.UpdateMaleList;

          // Update the female list view.
          Self.UpdateFemaleList;
        end;
      end;
    end;
  end;

  procedure TMain.lvMaleChange(Sender: TObject; Item: TListItem; Change: TItemChange);
  var
    I: Integer;
    MainNode, CurrNode: TNodeData;
  begin
    // Clear the items in the cbMale combo box.
    cbMale.Items.Clear;

    // Check if there are items in both lvMale and lvFemale list
    // views and an item is selected in lvMale.
    if (lvMale.Items.Count > 0) and (lvFemale.Items.Count > 0) and (lvMale.Selected <> nil) then
    begin
      // Retrieve the data of the selected male node.
      MainNode := MaleList.FindNodeByName(lvMale.Selected.Caption).Data;

      // Iterate through the items in the lvFemale list view.
      for I := 0 to lvFemale.Items.Count - 1 do
      begin
        // Retrieve the data of the current female node.
        CurrNode := FemaleList.FindNodeByName(lvFemale.Items[I].Caption).Data;

        // Check the compatibility between the selected male node and
        // the current female node.
        if CheckCompatibility(MainNode, CurrNode) then
          cbMale.Items.Add(CurrNode.Name);
      end;
    end;

    // Trigger the lvFemaleChange event to update the cbFemale combo box.
    if Sender <> nil then
      lvFemaleChange(nil, Item, Change);
  end;

  class function TMain.GetNodeDataDescription(const NodeData: TNodeData): string;
  begin
    // Build a string description of the NodeData.
    Result := 'Name: ' + NodeData.Name + sLineBreak +
              'Age: ' + IntToStr(NodeData.Age) + sLineBreak +
              'Height: ' + IntToStr(NodeData.Height) + sLineBreak +
              'Weight: ' + IntToStr(NodeData.Weight) + sLineBreak +
              'Habits: ' + NodeData.Habits + sLineBreak +
              'Hobby: ' + NodeData.Hobby;
  end;

  procedure TMain.ImageClick(Sender: TObject);
  begin
    // Check if a sorting option is selected in cbSort.
    if cbSort.ItemIndex > -1 then
    begin
      // Check the selected gender in cbSortGender.
      case cbSortGender.ItemIndex of
        0:
          // If the selected gender is male and there are items in lvMale, sort the MaleList based on the selected sorting option.
          if lvMale.Items.Count > 0 then
          begin
            MaleList.SortByParameter(SortingFunctions[cbSort.ItemIndex]);
            Self.UpdateMaleList;
          end;
        1:
          // If the selected gender is female and there are items in lvFemale, sort the FemaleList based on the selected sorting option.
          if lvFemale.Items.Count > 0 then
          begin
            FemaleList.SortByParameter(SortingFunctions[cbSort.ItemIndex]);
            Self.UpdateFemaleList;
          end;
      end;
    end;
  end;

  procedure TMain.SaveDataToFile(Sender: TObject);
  var
    DataFile: file of TNodeData;
    CurrentNode: PNode;
    NodeData: TNodeData;
  begin
    // Check if the user selected a file to save the data.
    if SaveDialog.Execute then
    begin
      // Assign the file specified in the SaveDialog to the DataFile variable.
      AssignFile(DataFile, SaveDialog.FileName);
      // Open the DataFile for writing, creating a new file or overwriting an existing one.
      Rewrite(DataFile);

      // Write the data from the MaleList to the file.
      CurrentNode := MaleList.Head;
      while CurrentNode <> nil do
      begin
        // Get the data from the current node.
        NodeData := CurrentNode^.Data;
        // Write the NodeData to the DataFile.
        Write(DataFile, NodeData);
        // Move to the next node in the MaleList.
        CurrentNode := CurrentNode^.Next;
      end;

      // Write the data from the FemaleList to the file.
      CurrentNode := FemaleList.Head;
      while CurrentNode <> nil do
      begin
        // Get the data from the current node.
        NodeData := CurrentNode^.Data;
        // Write the NodeData to the DataFile.
        Write(DataFile, NodeData);
        // Move to the next node in the FemaleList.
        CurrentNode := CurrentNode^.Next;
      end;

      // Close the DataFile after writing the data.
      CloseFile(DataFile);
    end;
  end;

  procedure TMain.LoadDataFromFile(Sender: TObject);
  var
    DataFile: file of TNodeData;
    NodeData: TNodeData;
  begin
    // Check if the user selected a file to load the data from.
    if SaveDialog.Execute then
    begin
      // Assign the file specified in the SaveDialog to the DataFile variable.
      AssignFile(DataFile, SaveDialog.FileName);
      // Open the DataFile for reading.
      Reset(DataFile);

      // Clear the MaleList and FemaleList to remove any existing data.
      MaleList.Clear;
      FemaleList.Clear;

      // Read data from the DataFile until the end of the file is reached.
      while not Eof(DataFile) do
      begin
        // Read a NodeData from the DataFile.
        Read(DataFile, NodeData);
        // Determine whether the NodeData belongs to a male or female based on the 'isMale' flag.
        case NodeData.isMale of
          True:
          begin
            // Add the NodeData to the MaleList.
            MaleList.AddNode(NodeData);
            // Add an item to the lvMale ListView with the NodeData's name.
            lvMale.AddItem(NodeData.Name, nil);
          end;
          False:
          begin
            // Add the NodeData to the FemaleList.
            FemaleList.AddNode(NodeData);
            // Add an item to the lvFemale ListView with the NodeData's name.
            lvFemale.AddItem(NodeData.Name, nil);
          end;
        end;
      end;

      // Close the DataFile after reading the data.
      CloseFile(DataFile);
    end;
  end;

end.
