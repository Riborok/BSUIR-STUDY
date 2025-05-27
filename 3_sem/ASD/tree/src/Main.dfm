object MainASD: TMainASD
  Left = 0
  Top = 0
  Caption = 'MainASD'
  ClientHeight = 437
  ClientWidth = 897
  Color = clBtnFace
  DoubleBuffered = True
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = 'Segoe UI'
  Font.Style = []
  WindowState = wsMaximized
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  TextHeight = 15
  object Memo: TMemo
    AlignWithMargins = True
    Left = 5
    Top = 259
    Width = 887
    Height = 178
    Margins.Left = 5
    Margins.Top = 0
    Margins.Right = 5
    Margins.Bottom = 0
    Align = alBottom
    Ctl3D = False
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -23
    Font.Name = 'Times New Roman'
    Font.Style = []
    Lines.Strings = (
      'Memo')
    ParentCtl3D = False
    ParentFont = False
    ReadOnly = True
    ScrollBars = ssBoth
    TabOrder = 0
  end
  object ScrollBox: TScrollBox
    AlignWithMargins = True
    Left = 3
    Top = 32
    Width = 891
    Height = 224
    HorzScrollBar.Tracking = True
    VertScrollBar.Tracking = True
    Align = alClient
    Color = clWindow
    ParentColor = False
    TabOrder = 1
    OnMouseWheel = ScrollBoxMouseWheel
    object PaintBox: TPaintBox
      Left = 0
      Top = 0
      Width = 420
      Height = 140
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -19
      Font.Name = 'Times New Roman'
      Font.Style = []
      ParentFont = False
      OnPaint = PaintBoxPaint
    end
  end
  object tbarMenu: TToolBar
    Left = 0
    Top = 0
    Width = 897
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
    TabOrder = 2
    Transparent = False
    object btnGenerate: TButton
      Left = 0
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Generate'
      TabOrder = 5
      OnClick = btnGenerateClick
    end
    object btnAdd: TButton
      AlignWithMargins = True
      Left = 75
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Add'
      TabOrder = 0
      OnClick = btnAddClick
    end
    object btnClear: TButton
      Left = 150
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Clear'
      TabOrder = 6
      OnClick = btnClearClick
    end
    object btnDelete: TButton
      AlignWithMargins = True
      Left = 225
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Delete'
      TabOrder = 1
      OnClick = btnDeleteClick
    end
    object btnPreOrder: TButton
      Left = 300
      Top = 0
      Width = 109
      Height = 29
      Caption = 'PreOrder (RAB)'
      TabOrder = 2
      OnClick = TraversalClick
    end
    object btnInOrder: TButton
      Tag = 1
      Left = 409
      Top = 0
      Width = 109
      Height = 29
      Caption = 'InOrder (ARB)'
      TabOrder = 3
      OnClick = TraversalClick
    end
    object btnPostOrder: TButton
      Tag = 2
      Left = 518
      Top = 0
      Width = 109
      Height = 29
      Caption = 'PostOrder (ABR)'
      TabOrder = 4
      OnClick = TraversalClick
    end
    object btnFirmware: TButton
      Left = 627
      Top = 0
      Width = 75
      Height = 29
      Caption = 'Firmware'
      TabOrder = 7
      OnClick = btnFirmwareClick
    end
    object btnRemFirmware: TButton
      Left = 702
      Top = 0
      Width = 89
      Height = 29
      Caption = 'Rem Firmware'
      TabOrder = 8
      OnClick = btnRemFirmwareClick
    end
  end
end
