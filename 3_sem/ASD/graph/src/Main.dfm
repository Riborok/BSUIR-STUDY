object Graph: TGraph
  Left = 0
  Top = 0
  Caption = 'Graph'
  ClientHeight = 421
  ClientWidth = 917
  Color = clBtnFace
  DoubleBuffered = True
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = 'Segoe UI'
  Font.Style = []
  OnCreate = FormCreate
  TextHeight = 15
  object Splitter1: TSplitter
    Left = 0
    Top = 220
    Width = 917
    Height = 16
    Cursor = crVSplit
    Align = alBottom
    ExplicitLeft = 8
    ExplicitTop = 279
    ExplicitWidth = 937
  end
  object Image1: TImage
    Left = 0
    Top = 29
    Width = 917
    Height = 191
    Align = alClient
    ExplicitLeft = 8
    ExplicitTop = 35
    ExplicitWidth = 105
    ExplicitHeight = 105
  end
  object tbarMenu: TToolBar
    Left = 0
    Top = 0
    Width = 917
    Height = 29
    ButtonHeight = 29
    ButtonWidth = 56
    Caption = 'tbarMenu'
    Color = clBlack
    DoubleBuffered = True
    GradientStartColor = clMenuHighlight
    HotTrackColor = clBlack
    ParentColor = False
    ParentDoubleBuffered = False
    ParentShowHint = False
    ShowHint = True
    TabOrder = 0
    Transparent = False
    object btnSTV: TButton
      Left = 0
      Top = 0
      Width = 75
      Height = 29
      Caption = 'SetVertices'
      TabOrder = 0
      OnClick = btnSTVClick
    end
    object btnFind: TButton
      Left = 75
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Find'
      TabOrder = 1
      OnClick = btnFindClick
    end
    object Button1: TButton
      Left = 150
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Random'
      TabOrder = 2
      OnClick = Button1Click
    end
  end
  object pnl: TPanel
    Left = 0
    Top = 236
    Width = 917
    Height = 185
    Align = alBottom
    ShowCaption = False
    TabOrder = 1
    object Splitter2: TSplitter
      Left = 646
      Top = 1
      Width = 6
      Height = 183
      Align = alRight
      ExplicitLeft = 662
    end
    object SG: TStringGrid
      Left = 1
      Top = 1
      Width = 645
      Height = 183
      Align = alClient
      ColCount = 2
      DefaultColWidth = 62
      RowCount = 2
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -19
      Font.Name = 'Segoe UI'
      Font.Style = []
      Options = [goFixedVertLine, goFixedHorzLine, goVertLine, goHorzLine, goRangeSelect, goEditing, goThumbTracking, goFixedRowDefAlign]
      ParentFont = False
      TabOrder = 0
      OnDrawCell = SGDrawCell
      OnSetEditText = SGSetEditText
    end
    object mm: TMemo
      Left = 652
      Top = 1
      Width = 264
      Height = 183
      Align = alRight
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -15
      Font.Name = 'Segoe UI'
      Font.Style = []
      Lines.Strings = (
        'mm')
      ParentFont = False
      ReadOnly = True
      ScrollBars = ssBoth
      TabOrder = 1
    end
  end
end
