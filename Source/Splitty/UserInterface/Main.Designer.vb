<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim CBlendItems1 As ProgBar.cBlendItems = New ProgBar.cBlendItems()
        Dim CFocalPoints1 As ProgBar.cFocalPoints = New ProgBar.cFocalPoints()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Dim DesignerRectTracker1 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim CBlendItems2 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker2 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim DesignerRectTracker3 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim CBlendItems3 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker4 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim DesignerRectTracker5 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim CBlendItems4 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker6 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim DesignerRectTracker7 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim CBlendItems5 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker8 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim DesignerRectTracker9 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim CBlendItems6 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker10 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim DesignerRectTracker11 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim CBlendItems7 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker12 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Me.TextBox_Input_Folder = New TextBoxHint.TextBoxHint()
        Me.VistaFolderBrowserDialog1 = New Ookii.Dialogs.VistaFolderBrowserDialog()
        Me.RadioButton_CD = New System.Windows.Forms.RadioButton()
        Me.RadioButton_CD800 = New System.Windows.Forms.RadioButton()
        Me.RadioButton_DVD5 = New System.Windows.Forms.RadioButton()
        Me.RadioButton_DVD9 = New System.Windows.Forms.RadioButton()
        Me.RadioButton_BluRay = New System.Windows.Forms.RadioButton()
        Me.RadioButton_BluRay_DL = New System.Windows.Forms.RadioButton()
        Me.GroupPanel_Options = New CodeVendor.Controls.Grouper()
        Me.RadioButton_BluRay_MiniDisc = New System.Windows.Forms.RadioButton()
        Me.RadioButton_DVD10 = New System.Windows.Forms.RadioButton()
        Me.RadioButton_CD900 = New System.Windows.Forms.RadioButton()
        Me.PictureBox_Disc = New System.Windows.Forms.PictureBox()
        Me.ComboBox_Custom_Size = New System.Windows.Forms.ComboBox()
        Me.TextBox_Custom_Size = New TextBoxHint.TextBoxHint()
        Me.RadioButton_Custom_Size = New System.Windows.Forms.RadioButton()
        Me.TextBox_Output_Folder = New TextBoxHint.TextBoxHint()
        Me.TextBox_Output_Dir = New TextBoxHint.TextBoxHint()
        Me.TextBox_Folder = New TextBoxHint.TextBoxHint()
        Me.ProgBarPlus = New ProgBar.ProgBarPlus()
        Me.Button_Output_Folder = New CButtonLib.CButton()
        Me.Button_Input_Folder = New CButtonLib.CButton()
        Me.Button_Split = New CButtonLib.CButton()
        Me.CButton1 = New CButtonLib.CButton()
        Me.Button_Folder = New CButtonLib.CButton()
        Me.Button_Stop = New CButtonLib.CButton()
        Me.RadioButton_SFX = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Zip = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Rar = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Copy = New System.Windows.Forms.RadioButton()
        Me.Label_Size_Value = New System.Windows.Forms.Label()
        Me.Label_Discs_Value = New System.Windows.Forms.Label()
        Me.GroupBox_Information = New CodeVendor.Controls.Grouper()
        Me.Label_Discs_Name = New System.Windows.Forms.Label()
        Me.Label_Size_Name = New System.Windows.Forms.Label()
        Me.GroupBox_Mode = New CodeVendor.Controls.Grouper()
        Me.RadioButton_TAR = New System.Windows.Forms.RadioButton()
        Me.RadioButton_ISO = New System.Windows.Forms.RadioButton()
        Me.Label_Mode = New gLabel.gLabel()
        Me.Label_Capacity = New gLabel.gLabel()
        Me.Label_Information = New gLabel.gLabel()
        Me.Label_Input = New System.Windows.Forms.Label()
        Me.Label_Output = New System.Windows.Forms.Label()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.LanguageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnglishToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SpanishToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FileSystemWatcher1 = New System.IO.FileSystemWatcher()
        Me.GroupPanel_Options.SuspendLayout()
        CType(Me.PictureBox_Disc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox_Information.SuspendLayout()
        Me.GroupBox_Mode.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.FileSystemWatcher1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TextBox_Input_Folder
        '
        Me.TextBox_Input_Folder.AllowDrop = True
        Me.TextBox_Input_Folder.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.TextBox_Input_Folder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox_Input_Folder.ForeColor = System.Drawing.Color.LightGray
        Me.TextBox_Input_Folder.HintDetails.HintColor = System.Drawing.Color.DarkGray
        Me.TextBox_Input_Folder.HintDetails.HintFadeSpeed = 10
        Me.TextBox_Input_Folder.HintDetails.HintText = " Drop a folder here..."
        Me.TextBox_Input_Folder.Location = New System.Drawing.Point(56, 35)
        Me.TextBox_Input_Folder.Name = "TextBox_Input_Folder"
        Me.TextBox_Input_Folder.ReadOnly = True
        Me.TextBox_Input_Folder.Size = New System.Drawing.Size(336, 20)
        Me.TextBox_Input_Folder.TabIndex = 1
        '
        'RadioButton_CD
        '
        Me.RadioButton_CD.AutoSize = True
        Me.RadioButton_CD.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_CD.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_CD.Location = New System.Drawing.Point(13, 14)
        Me.RadioButton_CD.Name = "RadioButton_CD"
        Me.RadioButton_CD.Size = New System.Drawing.Size(80, 17)
        Me.RadioButton_CD.TabIndex = 6
        Me.RadioButton_CD.TabStop = True
        Me.RadioButton_CD.Text = "CD 700 MB"
        Me.RadioButton_CD.UseVisualStyleBackColor = True
        '
        'RadioButton_CD800
        '
        Me.RadioButton_CD800.AutoSize = True
        Me.RadioButton_CD800.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_CD800.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_CD800.Location = New System.Drawing.Point(13, 40)
        Me.RadioButton_CD800.Name = "RadioButton_CD800"
        Me.RadioButton_CD800.Size = New System.Drawing.Size(80, 17)
        Me.RadioButton_CD800.TabIndex = 7
        Me.RadioButton_CD800.TabStop = True
        Me.RadioButton_CD800.Text = "CD 800 MB"
        Me.RadioButton_CD800.UseVisualStyleBackColor = True
        '
        'RadioButton_DVD5
        '
        Me.RadioButton_DVD5.AutoSize = True
        Me.RadioButton_DVD5.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_DVD5.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_DVD5.Location = New System.Drawing.Point(108, 14)
        Me.RadioButton_DVD5.Name = "RadioButton_DVD5"
        Me.RadioButton_DVD5.Size = New System.Drawing.Size(54, 17)
        Me.RadioButton_DVD5.TabIndex = 9
        Me.RadioButton_DVD5.TabStop = True
        Me.RadioButton_DVD5.Text = "DVD5"
        Me.RadioButton_DVD5.UseVisualStyleBackColor = True
        '
        'RadioButton_DVD9
        '
        Me.RadioButton_DVD9.AutoSize = True
        Me.RadioButton_DVD9.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_DVD9.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_DVD9.Location = New System.Drawing.Point(108, 40)
        Me.RadioButton_DVD9.Name = "RadioButton_DVD9"
        Me.RadioButton_DVD9.Size = New System.Drawing.Size(54, 17)
        Me.RadioButton_DVD9.TabIndex = 10
        Me.RadioButton_DVD9.TabStop = True
        Me.RadioButton_DVD9.Text = "DVD9"
        Me.RadioButton_DVD9.UseVisualStyleBackColor = True
        '
        'RadioButton_BluRay
        '
        Me.RadioButton_BluRay.AutoSize = True
        Me.RadioButton_BluRay.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_BluRay.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_BluRay.Location = New System.Drawing.Point(179, 14)
        Me.RadioButton_BluRay.Name = "RadioButton_BluRay"
        Me.RadioButton_BluRay.Size = New System.Drawing.Size(59, 17)
        Me.RadioButton_BluRay.TabIndex = 11
        Me.RadioButton_BluRay.TabStop = True
        Me.RadioButton_BluRay.Text = "BluRay"
        Me.RadioButton_BluRay.UseVisualStyleBackColor = True
        '
        'RadioButton_BluRay_DL
        '
        Me.RadioButton_BluRay_DL.AutoSize = True
        Me.RadioButton_BluRay_DL.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_BluRay_DL.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_BluRay_DL.Location = New System.Drawing.Point(179, 40)
        Me.RadioButton_BluRay_DL.Name = "RadioButton_BluRay_DL"
        Me.RadioButton_BluRay_DL.Size = New System.Drawing.Size(76, 17)
        Me.RadioButton_BluRay_DL.TabIndex = 12
        Me.RadioButton_BluRay_DL.TabStop = True
        Me.RadioButton_BluRay_DL.Text = "BluRay DL"
        Me.RadioButton_BluRay_DL.UseVisualStyleBackColor = True
        '
        'GroupPanel_Options
        '
        Me.GroupPanel_Options.BackgroundColor = System.Drawing.Color.Transparent
        Me.GroupPanel_Options.BackgroundGradientColor = System.Drawing.Color.White
        Me.GroupPanel_Options.BackgroundGradientMode = CodeVendor.Controls.Grouper.GroupBoxGradientMode.None
        Me.GroupPanel_Options.BorderColor = System.Drawing.Color.Black
        Me.GroupPanel_Options.BorderThickness = 1.0!
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_BluRay_DL)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_BluRay_MiniDisc)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_DVD10)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_CD900)
        Me.GroupPanel_Options.Controls.Add(Me.PictureBox_Disc)
        Me.GroupPanel_Options.Controls.Add(Me.ComboBox_Custom_Size)
        Me.GroupPanel_Options.Controls.Add(Me.TextBox_Custom_Size)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_Custom_Size)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_CD)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_CD800)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_BluRay)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_DVD5)
        Me.GroupPanel_Options.Controls.Add(Me.RadioButton_DVD9)
        Me.GroupPanel_Options.CustomGroupBoxColor = System.Drawing.Color.White
        Me.GroupPanel_Options.Enabled = False
        Me.GroupPanel_Options.GroupImage = Nothing
        Me.GroupPanel_Options.GroupTitle = ""
        Me.GroupPanel_Options.Location = New System.Drawing.Point(8, 95)
        Me.GroupPanel_Options.Name = "GroupPanel_Options"
        Me.GroupPanel_Options.Padding = New System.Windows.Forms.Padding(20)
        Me.GroupPanel_Options.PaintGroupBox = False
        Me.GroupPanel_Options.RoundCorners = 5
        Me.GroupPanel_Options.ShadowColor = System.Drawing.Color.DarkGray
        Me.GroupPanel_Options.ShadowControl = False
        Me.GroupPanel_Options.ShadowThickness = 3
        Me.GroupPanel_Options.Size = New System.Drawing.Size(417, 125)
        Me.GroupPanel_Options.TabIndex = 13
        '
        'RadioButton_BluRay_MiniDisc
        '
        Me.RadioButton_BluRay_MiniDisc.AutoSize = True
        Me.RadioButton_BluRay_MiniDisc.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_BluRay_MiniDisc.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_BluRay_MiniDisc.Location = New System.Drawing.Point(179, 66)
        Me.RadioButton_BluRay_MiniDisc.Name = "RadioButton_BluRay_MiniDisc"
        Me.RadioButton_BluRay_MiniDisc.Size = New System.Drawing.Size(102, 17)
        Me.RadioButton_BluRay_MiniDisc.TabIndex = 19
        Me.RadioButton_BluRay_MiniDisc.TabStop = True
        Me.RadioButton_BluRay_MiniDisc.Text = "BluRay MiniDisc"
        Me.RadioButton_BluRay_MiniDisc.UseVisualStyleBackColor = True
        '
        'RadioButton_DVD10
        '
        Me.RadioButton_DVD10.AutoSize = True
        Me.RadioButton_DVD10.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_DVD10.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_DVD10.Location = New System.Drawing.Point(108, 66)
        Me.RadioButton_DVD10.Name = "RadioButton_DVD10"
        Me.RadioButton_DVD10.Size = New System.Drawing.Size(60, 17)
        Me.RadioButton_DVD10.TabIndex = 18
        Me.RadioButton_DVD10.TabStop = True
        Me.RadioButton_DVD10.Text = "DVD10"
        Me.RadioButton_DVD10.UseVisualStyleBackColor = True
        '
        'RadioButton_CD900
        '
        Me.RadioButton_CD900.AutoSize = True
        Me.RadioButton_CD900.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_CD900.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_CD900.Location = New System.Drawing.Point(13, 66)
        Me.RadioButton_CD900.Name = "RadioButton_CD900"
        Me.RadioButton_CD900.Size = New System.Drawing.Size(80, 17)
        Me.RadioButton_CD900.TabIndex = 17
        Me.RadioButton_CD900.TabStop = True
        Me.RadioButton_CD900.Text = "CD 900 MB"
        Me.RadioButton_CD900.UseVisualStyleBackColor = True
        '
        'PictureBox_Disc
        '
        Me.PictureBox_Disc.BackgroundImage = Global.Splitty.My.Resources.Resources.cd
        Me.PictureBox_Disc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PictureBox_Disc.Location = New System.Drawing.Point(289, 6)
        Me.PictureBox_Disc.Name = "PictureBox_Disc"
        Me.PictureBox_Disc.Size = New System.Drawing.Size(120, 114)
        Me.PictureBox_Disc.TabIndex = 16
        Me.PictureBox_Disc.TabStop = False
        '
        'ComboBox_Custom_Size
        '
        Me.ComboBox_Custom_Size.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.ComboBox_Custom_Size.Cursor = System.Windows.Forms.Cursors.Hand
        Me.ComboBox_Custom_Size.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox_Custom_Size.Enabled = False
        Me.ComboBox_Custom_Size.ForeColor = System.Drawing.Color.LightGray
        Me.ComboBox_Custom_Size.FormattingEnabled = True
        Me.ComboBox_Custom_Size.Items.AddRange(New Object() {"MB", "GB", "TB"})
        Me.ComboBox_Custom_Size.Location = New System.Drawing.Point(179, 90)
        Me.ComboBox_Custom_Size.Name = "ComboBox_Custom_Size"
        Me.ComboBox_Custom_Size.Size = New System.Drawing.Size(65, 21)
        Me.ComboBox_Custom_Size.TabIndex = 15
        '
        'TextBox_Custom_Size
        '
        Me.TextBox_Custom_Size.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.TextBox_Custom_Size.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox_Custom_Size.Enabled = False
        Me.TextBox_Custom_Size.ForeColor = System.Drawing.Color.LightGray
        Me.TextBox_Custom_Size.HintDetails.HintColor = System.Drawing.Color.DarkGray
        Me.TextBox_Custom_Size.HintDetails.HintFadeSpeed = 4
        Me.TextBox_Custom_Size.Location = New System.Drawing.Point(108, 91)
        Me.TextBox_Custom_Size.Name = "TextBox_Custom_Size"
        Me.TextBox_Custom_Size.Size = New System.Drawing.Size(53, 20)
        Me.TextBox_Custom_Size.TabIndex = 14
        Me.TextBox_Custom_Size.Text = "0"
        '
        'RadioButton_Custom_Size
        '
        Me.RadioButton_Custom_Size.AutoSize = True
        Me.RadioButton_Custom_Size.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_Custom_Size.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_Custom_Size.Location = New System.Drawing.Point(13, 92)
        Me.RadioButton_Custom_Size.Name = "RadioButton_Custom_Size"
        Me.RadioButton_Custom_Size.Size = New System.Drawing.Size(81, 17)
        Me.RadioButton_Custom_Size.TabIndex = 13
        Me.RadioButton_Custom_Size.TabStop = True
        Me.RadioButton_Custom_Size.Text = "Custom size"
        Me.RadioButton_Custom_Size.UseVisualStyleBackColor = True
        '
        'TextBox_Output_Folder
        '
        Me.TextBox_Output_Folder.AllowDrop = True
        Me.TextBox_Output_Folder.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.TextBox_Output_Folder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox_Output_Folder.Enabled = False
        Me.TextBox_Output_Folder.ForeColor = System.Drawing.Color.LightGray
        Me.TextBox_Output_Folder.HintDetails.HintColor = System.Drawing.Color.DarkGray
        Me.TextBox_Output_Folder.HintDetails.HintFadeSpeed = 10
        Me.TextBox_Output_Folder.Location = New System.Drawing.Point(56, 61)
        Me.TextBox_Output_Folder.Name = "TextBox_Output_Folder"
        Me.TextBox_Output_Folder.ReadOnly = True
        Me.TextBox_Output_Folder.Size = New System.Drawing.Size(336, 20)
        Me.TextBox_Output_Folder.TabIndex = 14
        '
        'TextBox_Output_Dir
        '
        Me.TextBox_Output_Dir.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.TextBox_Output_Dir.Enabled = False
        Me.TextBox_Output_Dir.ForeColor = System.Drawing.Color.LightGray
        Me.TextBox_Output_Dir.HintDetails.HintColor = System.Drawing.Color.DarkGray
        Me.TextBox_Output_Dir.HintDetails.HintText = "Output folder"
        Me.TextBox_Output_Dir.Location = New System.Drawing.Point(12, 41)
        Me.TextBox_Output_Dir.Name = "TextBox_Output_Dir"
        Me.TextBox_Output_Dir.Size = New System.Drawing.Size(384, 20)
        Me.TextBox_Output_Dir.TabIndex = 14
        '
        'TextBox_Folder
        '
        Me.TextBox_Folder.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.TextBox_Folder.ForeColor = System.Drawing.Color.LightGray
        Me.TextBox_Folder.HintDetails.HintColor = System.Drawing.Color.DarkGray
        Me.TextBox_Folder.HintDetails.HintText = "Input folder"
        Me.TextBox_Folder.Location = New System.Drawing.Point(12, 15)
        Me.TextBox_Folder.Name = "TextBox_Folder"
        Me.TextBox_Folder.Size = New System.Drawing.Size(384, 20)
        Me.TextBox_Folder.TabIndex = 1
        '
        'ProgBarPlus
        '
        Me.ProgBarPlus.BarBackColor = System.Drawing.Color.Transparent
        CBlendItems1.iColor = New System.Drawing.Color() {System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(154, Byte), Integer), CType(CType(205, Byte), Integer), CType(CType(50, Byte), Integer))}
        CBlendItems1.iPoint = New Single() {0.0!, 1.0!}
        Me.ProgBarPlus.BarColorBlend = CBlendItems1
        Me.ProgBarPlus.BarColorSolid = System.Drawing.Color.YellowGreen
        Me.ProgBarPlus.BarColorSolidB = System.Drawing.Color.White
        Me.ProgBarPlus.BarPadding = New System.Windows.Forms.Padding(0)
        Me.ProgBarPlus.BarStyleFill = ProgBar.ProgBarPlus.eBarStyle.GradientLinear
        Me.ProgBarPlus.BarStyleLinear = System.Drawing.Drawing2D.LinearGradientMode.Horizontal
        Me.ProgBarPlus.BarStyleTexture = Nothing
        Me.ProgBarPlus.BorderColor = System.Drawing.Color.Transparent
        Me.ProgBarPlus.Corners.All = 4
        Me.ProgBarPlus.Corners.LowerLeft = 4
        Me.ProgBarPlus.Corners.LowerRight = 4
        Me.ProgBarPlus.Corners.UpperLeft = 4
        Me.ProgBarPlus.Corners.UpperRight = 4
        Me.ProgBarPlus.CylonMove = 5.0!
        CFocalPoints1.CenterPoint = CType(resources.GetObject("CFocalPoints1.CenterPoint"), System.Drawing.PointF)
        CFocalPoints1.FocusScales = CType(resources.GetObject("CFocalPoints1.FocusScales"), System.Drawing.PointF)
        Me.ProgBarPlus.FocalPoints = CFocalPoints1
        Me.ProgBarPlus.Font = New System.Drawing.Font("Arial", 10.0!)
        Me.ProgBarPlus.ForeColor = System.Drawing.Color.White
        Me.ProgBarPlus.Location = New System.Drawing.Point(183, 301)
        Me.ProgBarPlus.Name = "ProgBarPlus"
        Me.ProgBarPlus.ShapeTextFont = New System.Drawing.Font("Arial", 30.0!)
        Me.ProgBarPlus.Size = New System.Drawing.Size(242, 52)
        Me.ProgBarPlus.TabIndex = 18
        Me.ProgBarPlus.TextFormat = "Process {1}% Done"
        Me.ProgBarPlus.TextShadow = True
        Me.ProgBarPlus.TextShadowColor = System.Drawing.Color.Black
        Me.ProgBarPlus.Value = 0
        '
        'Button_Output_Folder
        '
        Me.Button_Output_Folder.BackColor = System.Drawing.Color.Transparent
        Me.Button_Output_Folder.BorderColor = System.Drawing.Color.Transparent
        DesignerRectTracker1.IsActive = False
        DesignerRectTracker1.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker1.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Output_Folder.CenterPtTracker = DesignerRectTracker1
        CBlendItems2.iColor = New System.Drawing.Color() {System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent}
        CBlendItems2.iPoint = New Single() {0.0!, 0.5!, 1.0!}
        Me.Button_Output_Folder.ColorFillBlend = CBlendItems2
        Me.Button_Output_Folder.Corners.All = 2
        Me.Button_Output_Folder.Corners.LowerLeft = 2
        Me.Button_Output_Folder.Corners.LowerRight = 2
        Me.Button_Output_Folder.Corners.UpperLeft = 2
        Me.Button_Output_Folder.Corners.UpperRight = 2
        Me.Button_Output_Folder.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button_Output_Folder.Enabled = False
        Me.Button_Output_Folder.FocalPoints.CenterPtX = 1.0!
        Me.Button_Output_Folder.FocalPoints.CenterPtY = 0.4!
        DesignerRectTracker2.IsActive = False
        DesignerRectTracker2.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker2.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Output_Folder.FocusPtTracker = DesignerRectTracker2
        Me.Button_Output_Folder.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Button_Output_Folder.Image = Global.Splitty.My.Resources.Resources.search
        Me.Button_Output_Folder.ImageAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.Button_Output_Folder.ImageIndex = 0
        Me.Button_Output_Folder.ImageSize = New System.Drawing.Size(26, 26)
        Me.Button_Output_Folder.Location = New System.Drawing.Point(398, 57)
        Me.Button_Output_Folder.Name = "Button_Output_Folder"
        Me.Button_Output_Folder.Size = New System.Drawing.Size(32, 31)
        Me.Button_Output_Folder.TabIndex = 15
        Me.Button_Output_Folder.Text = ""
        '
        'Button_Input_Folder
        '
        Me.Button_Input_Folder.BackColor = System.Drawing.Color.Transparent
        Me.Button_Input_Folder.BorderColor = System.Drawing.Color.Transparent
        DesignerRectTracker3.IsActive = False
        DesignerRectTracker3.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker3.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Input_Folder.CenterPtTracker = DesignerRectTracker3
        CBlendItems3.iColor = New System.Drawing.Color() {System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent}
        CBlendItems3.iPoint = New Single() {0.0!, 0.5!, 1.0!}
        Me.Button_Input_Folder.ColorFillBlend = CBlendItems3
        Me.Button_Input_Folder.Corners.All = 2
        Me.Button_Input_Folder.Corners.LowerLeft = 2
        Me.Button_Input_Folder.Corners.LowerRight = 2
        Me.Button_Input_Folder.Corners.UpperLeft = 2
        Me.Button_Input_Folder.Corners.UpperRight = 2
        Me.Button_Input_Folder.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button_Input_Folder.FocalPoints.CenterPtX = 1.0!
        Me.Button_Input_Folder.FocalPoints.CenterPtY = 0.4!
        DesignerRectTracker4.IsActive = False
        DesignerRectTracker4.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker4.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Input_Folder.FocusPtTracker = DesignerRectTracker4
        Me.Button_Input_Folder.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Button_Input_Folder.Image = Global.Splitty.My.Resources.Resources.search
        Me.Button_Input_Folder.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.Button_Input_Folder.ImageIndex = 0
        Me.Button_Input_Folder.ImageSize = New System.Drawing.Size(26, 26)
        Me.Button_Input_Folder.Location = New System.Drawing.Point(398, 29)
        Me.Button_Input_Folder.Name = "Button_Input_Folder"
        Me.Button_Input_Folder.Size = New System.Drawing.Size(32, 31)
        Me.Button_Input_Folder.TabIndex = 4
        Me.Button_Input_Folder.Text = ""
        Me.Button_Input_Folder.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'Button_Split
        '
        Me.Button_Split.BackColor = System.Drawing.Color.Transparent
        Me.Button_Split.BorderColor = System.Drawing.Color.Black
        DesignerRectTracker5.IsActive = False
        DesignerRectTracker5.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker5.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Split.CenterPtTracker = DesignerRectTracker5
        CBlendItems4.iColor = New System.Drawing.Color() {System.Drawing.Color.GreenYellow, System.Drawing.Color.YellowGreen, System.Drawing.Color.DarkGreen}
        CBlendItems4.iPoint = New Single() {0.0!, 0.5!, 1.0!}
        Me.Button_Split.ColorFillBlend = CBlendItems4
        Me.Button_Split.ColorFillSolid = System.Drawing.Color.YellowGreen
        Me.Button_Split.Corners.All = 5
        Me.Button_Split.Corners.LowerLeft = 5
        Me.Button_Split.Corners.LowerRight = 5
        Me.Button_Split.Corners.UpperLeft = 5
        Me.Button_Split.Corners.UpperRight = 5
        Me.Button_Split.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button_Split.Enabled = False
        DesignerRectTracker6.IsActive = False
        DesignerRectTracker6.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker6.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Split.FocusPtTracker = DesignerRectTracker6
        Me.Button_Split.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.Button_Split.ForeColor = System.Drawing.Color.Black
        Me.Button_Split.Image = Global.Splitty.My.Resources.Resources.split
        Me.Button_Split.ImageAlign = System.Drawing.ContentAlignment.TopRight
        Me.Button_Split.ImageIndex = 0
        Me.Button_Split.ImageSize = New System.Drawing.Size(60, 60)
        Me.Button_Split.Location = New System.Drawing.Point(8, 301)
        Me.Button_Split.Name = "Button_Split"
        Me.Button_Split.Size = New System.Drawing.Size(167, 52)
        Me.Button_Split.TabIndex = 0
        Me.Button_Split.Text = "SPLIT"
        Me.Button_Split.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'CButton1
        '
        Me.CButton1.BackColor = System.Drawing.Color.Transparent
        Me.CButton1.BorderColor = System.Drawing.Color.Transparent
        DesignerRectTracker7.IsActive = False
        DesignerRectTracker7.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker7.TrackerRectangle"), System.Drawing.RectangleF)
        Me.CButton1.CenterPtTracker = DesignerRectTracker7
        CBlendItems5.iColor = New System.Drawing.Color() {System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent}
        CBlendItems5.iPoint = New Single() {0.0!, 0.5!, 1.0!}
        Me.CButton1.ColorFillBlend = CBlendItems5
        Me.CButton1.Corners.All = 2
        Me.CButton1.Corners.LowerLeft = 2
        Me.CButton1.Corners.LowerRight = 2
        Me.CButton1.Corners.UpperLeft = 2
        Me.CButton1.Corners.UpperRight = 2
        Me.CButton1.Enabled = False
        Me.CButton1.FocalPoints.CenterPtX = 1.0!
        Me.CButton1.FocalPoints.CenterPtY = 0.4!
        DesignerRectTracker8.IsActive = False
        DesignerRectTracker8.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker8.TrackerRectangle"), System.Drawing.RectangleF)
        Me.CButton1.FocusPtTracker = DesignerRectTracker8
        Me.CButton1.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.CButton1.Image = Global.Splitty.My.Resources.Resources.dvd
        Me.CButton1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.CButton1.ImageIndex = 0
        Me.CButton1.ImageSize = New System.Drawing.Size(26, 26)
        Me.CButton1.Location = New System.Drawing.Point(402, 37)
        Me.CButton1.Name = "CButton1"
        Me.CButton1.Size = New System.Drawing.Size(32, 31)
        Me.CButton1.TabIndex = 15
        Me.CButton1.Text = ""
        '
        'Button_Folder
        '
        Me.Button_Folder.BackColor = System.Drawing.Color.Transparent
        Me.Button_Folder.BorderColor = System.Drawing.Color.Transparent
        DesignerRectTracker9.IsActive = False
        DesignerRectTracker9.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker9.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Folder.CenterPtTracker = DesignerRectTracker9
        CBlendItems6.iColor = New System.Drawing.Color() {System.Drawing.Color.Transparent, System.Drawing.Color.Transparent, System.Drawing.Color.Transparent}
        CBlendItems6.iPoint = New Single() {0.0!, 0.5!, 1.0!}
        Me.Button_Folder.ColorFillBlend = CBlendItems6
        Me.Button_Folder.Corners.All = 2
        Me.Button_Folder.Corners.LowerLeft = 2
        Me.Button_Folder.Corners.LowerRight = 2
        Me.Button_Folder.Corners.UpperLeft = 2
        Me.Button_Folder.Corners.UpperRight = 2
        Me.Button_Folder.FocalPoints.CenterPtX = 1.0!
        Me.Button_Folder.FocalPoints.CenterPtY = 0.4!
        DesignerRectTracker10.IsActive = False
        DesignerRectTracker10.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker10.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Folder.FocusPtTracker = DesignerRectTracker10
        Me.Button_Folder.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Button_Folder.Image = Global.Splitty.My.Resources.Resources.dvd
        Me.Button_Folder.ImageAlign = System.Drawing.ContentAlignment.TopLeft
        Me.Button_Folder.ImageIndex = 0
        Me.Button_Folder.ImageSize = New System.Drawing.Size(26, 26)
        Me.Button_Folder.Location = New System.Drawing.Point(402, 9)
        Me.Button_Folder.Name = "Button_Folder"
        Me.Button_Folder.Size = New System.Drawing.Size(32, 31)
        Me.Button_Folder.TabIndex = 4
        Me.Button_Folder.Text = ""
        Me.Button_Folder.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'Button_Stop
        '
        Me.Button_Stop.BackColor = System.Drawing.Color.Transparent
        Me.Button_Stop.BorderColor = System.Drawing.Color.Black
        DesignerRectTracker11.IsActive = False
        DesignerRectTracker11.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker11.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Stop.CenterPtTracker = DesignerRectTracker11
        CBlendItems7.iColor = New System.Drawing.Color() {System.Drawing.Color.White, System.Drawing.Color.Red, System.Drawing.Color.DarkRed}
        CBlendItems7.iPoint = New Single() {0.0!, 0.5!, 1.0!}
        Me.Button_Stop.ColorFillBlend = CBlendItems7
        Me.Button_Stop.ColorFillSolid = System.Drawing.Color.Red
        Me.Button_Stop.Corners.All = 5
        Me.Button_Stop.Corners.LowerLeft = 5
        Me.Button_Stop.Corners.LowerRight = 5
        Me.Button_Stop.Corners.UpperLeft = 5
        Me.Button_Stop.Corners.UpperRight = 5
        Me.Button_Stop.Cursor = System.Windows.Forms.Cursors.Hand
        Me.Button_Stop.Enabled = False
        Me.Button_Stop.FocalPoints.CenterPtX = 0.5269461!
        Me.Button_Stop.FocalPoints.CenterPtY = 0.5769231!
        DesignerRectTracker12.IsActive = False
        DesignerRectTracker12.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker12.TrackerRectangle"), System.Drawing.RectangleF)
        Me.Button_Stop.FocusPtTracker = DesignerRectTracker12
        Me.Button_Stop.Font = New System.Drawing.Font("Arial", 25.0!, System.Drawing.FontStyle.Bold)
        Me.Button_Stop.ForeColor = System.Drawing.Color.Black
        Me.Button_Stop.Image = Global.Splitty.My.Resources.Resources.split
        Me.Button_Stop.ImageAlign = System.Drawing.ContentAlignment.TopRight
        Me.Button_Stop.ImageIndex = 0
        Me.Button_Stop.ImageSize = New System.Drawing.Size(60, 60)
        Me.Button_Stop.Location = New System.Drawing.Point(8, 301)
        Me.Button_Stop.Name = "Button_Stop"
        Me.Button_Stop.Size = New System.Drawing.Size(167, 52)
        Me.Button_Stop.TabIndex = 19
        Me.Button_Stop.Text = "STOP"
        Me.Button_Stop.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.Button_Stop.Visible = False
        '
        'RadioButton_SFX
        '
        Me.RadioButton_SFX.AutoSize = True
        Me.RadioButton_SFX.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_SFX.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_SFX.Location = New System.Drawing.Point(12, 35)
        Me.RadioButton_SFX.Name = "RadioButton_SFX"
        Me.RadioButton_SFX.Size = New System.Drawing.Size(40, 17)
        Me.RadioButton_SFX.TabIndex = 5
        Me.RadioButton_SFX.Text = "Sfx"
        Me.RadioButton_SFX.UseVisualStyleBackColor = True
        '
        'RadioButton_Zip
        '
        Me.RadioButton_Zip.AutoSize = True
        Me.RadioButton_Zip.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_Zip.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_Zip.Location = New System.Drawing.Point(117, 35)
        Me.RadioButton_Zip.Name = "RadioButton_Zip"
        Me.RadioButton_Zip.Size = New System.Drawing.Size(40, 17)
        Me.RadioButton_Zip.TabIndex = 4
        Me.RadioButton_Zip.Text = "Zip"
        Me.RadioButton_Zip.UseVisualStyleBackColor = True
        '
        'RadioButton_Rar
        '
        Me.RadioButton_Rar.AutoSize = True
        Me.RadioButton_Rar.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_Rar.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_Rar.Location = New System.Drawing.Point(117, 13)
        Me.RadioButton_Rar.Name = "RadioButton_Rar"
        Me.RadioButton_Rar.Size = New System.Drawing.Size(42, 17)
        Me.RadioButton_Rar.TabIndex = 2
        Me.RadioButton_Rar.Text = "Rar"
        Me.RadioButton_Rar.UseVisualStyleBackColor = True
        '
        'RadioButton_Copy
        '
        Me.RadioButton_Copy.AutoSize = True
        Me.RadioButton_Copy.Checked = True
        Me.RadioButton_Copy.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_Copy.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_Copy.Location = New System.Drawing.Point(12, 13)
        Me.RadioButton_Copy.Name = "RadioButton_Copy"
        Me.RadioButton_Copy.Size = New System.Drawing.Size(49, 17)
        Me.RadioButton_Copy.TabIndex = 0
        Me.RadioButton_Copy.TabStop = True
        Me.RadioButton_Copy.Text = "Copy"
        Me.RadioButton_Copy.UseVisualStyleBackColor = True
        '
        'Label_Size_Value
        '
        Me.Label_Size_Value.AutoSize = True
        Me.Label_Size_Value.BackColor = System.Drawing.Color.Transparent
        Me.Label_Size_Value.Font = New System.Drawing.Font("Arial", 10.0!)
        Me.Label_Size_Value.ForeColor = System.Drawing.Color.Black
        Me.Label_Size_Value.Location = New System.Drawing.Point(58, 14)
        Me.Label_Size_Value.Name = "Label_Size_Value"
        Me.Label_Size_Value.Size = New System.Drawing.Size(16, 16)
        Me.Label_Size_Value.TabIndex = 5
        Me.Label_Size_Value.Text = "0"
        Me.Label_Size_Value.Visible = False
        '
        'Label_Discs_Value
        '
        Me.Label_Discs_Value.AutoSize = True
        Me.Label_Discs_Value.BackColor = System.Drawing.Color.Transparent
        Me.Label_Discs_Value.Font = New System.Drawing.Font("Arial", 10.0!)
        Me.Label_Discs_Value.ForeColor = System.Drawing.Color.Black
        Me.Label_Discs_Value.Location = New System.Drawing.Point(58, 36)
        Me.Label_Discs_Value.Name = "Label_Discs_Value"
        Me.Label_Discs_Value.Size = New System.Drawing.Size(16, 16)
        Me.Label_Discs_Value.TabIndex = 17
        Me.Label_Discs_Value.Text = "0"
        Me.Label_Discs_Value.Visible = False
        '
        'GroupBox_Information
        '
        Me.GroupBox_Information.BackgroundColor = System.Drawing.Color.Transparent
        Me.GroupBox_Information.BackgroundGradientColor = System.Drawing.Color.Transparent
        Me.GroupBox_Information.BackgroundGradientMode = CodeVendor.Controls.Grouper.GroupBoxGradientMode.None
        Me.GroupBox_Information.BorderColor = System.Drawing.Color.Black
        Me.GroupBox_Information.BorderThickness = 1.0!
        Me.GroupBox_Information.Controls.Add(Me.Label_Discs_Name)
        Me.GroupBox_Information.Controls.Add(Me.Label_Size_Value)
        Me.GroupBox_Information.Controls.Add(Me.Label_Discs_Value)
        Me.GroupBox_Information.Controls.Add(Me.Label_Size_Name)
        Me.GroupBox_Information.CustomGroupBoxColor = System.Drawing.Color.White
        Me.GroupBox_Information.Font = New System.Drawing.Font("Arial", 9.0!)
        Me.GroupBox_Information.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.GroupBox_Information.GroupImage = Nothing
        Me.GroupBox_Information.GroupTitle = ""
        Me.GroupBox_Information.Location = New System.Drawing.Point(183, 231)
        Me.GroupBox_Information.Name = "GroupBox_Information"
        Me.GroupBox_Information.Padding = New System.Windows.Forms.Padding(20)
        Me.GroupBox_Information.PaintGroupBox = False
        Me.GroupBox_Information.RoundCorners = 5
        Me.GroupBox_Information.ShadowColor = System.Drawing.Color.DarkGray
        Me.GroupBox_Information.ShadowControl = False
        Me.GroupBox_Information.ShadowThickness = 3
        Me.GroupBox_Information.Size = New System.Drawing.Size(242, 61)
        Me.GroupBox_Information.TabIndex = 23
        '
        'Label_Discs_Name
        '
        Me.Label_Discs_Name.BackColor = System.Drawing.Color.Transparent
        Me.Label_Discs_Name.Font = New System.Drawing.Font("Arial", 10.0!, System.Drawing.FontStyle.Bold)
        Me.Label_Discs_Name.ForeColor = System.Drawing.Color.Black
        Me.Label_Discs_Name.Location = New System.Drawing.Point(7, 35)
        Me.Label_Discs_Name.Name = "Label_Discs_Name"
        Me.Label_Discs_Name.Size = New System.Drawing.Size(54, 16)
        Me.Label_Discs_Name.TabIndex = 19
        Me.Label_Discs_Name.Text = "Discs :"
        Me.Label_Discs_Name.Visible = False
        '
        'Label_Size_Name
        '
        Me.Label_Size_Name.BackColor = System.Drawing.Color.Transparent
        Me.Label_Size_Name.Font = New System.Drawing.Font("Arial", 9.89!, System.Drawing.FontStyle.Bold)
        Me.Label_Size_Name.ForeColor = System.Drawing.Color.Black
        Me.Label_Size_Name.Location = New System.Drawing.Point(7, 13)
        Me.Label_Size_Name.Name = "Label_Size_Name"
        Me.Label_Size_Name.Size = New System.Drawing.Size(60, 16)
        Me.Label_Size_Name.TabIndex = 18
        Me.Label_Size_Name.Text = "Data   :"
        Me.Label_Size_Name.Visible = False
        '
        'GroupBox_Mode
        '
        Me.GroupBox_Mode.BackgroundColor = System.Drawing.Color.Transparent
        Me.GroupBox_Mode.BackgroundGradientColor = System.Drawing.Color.Transparent
        Me.GroupBox_Mode.BackgroundGradientMode = CodeVendor.Controls.Grouper.GroupBoxGradientMode.None
        Me.GroupBox_Mode.BorderColor = System.Drawing.Color.Black
        Me.GroupBox_Mode.BorderThickness = 1.0!
        Me.GroupBox_Mode.Controls.Add(Me.RadioButton_TAR)
        Me.GroupBox_Mode.Controls.Add(Me.RadioButton_ISO)
        Me.GroupBox_Mode.Controls.Add(Me.RadioButton_SFX)
        Me.GroupBox_Mode.Controls.Add(Me.RadioButton_Copy)
        Me.GroupBox_Mode.Controls.Add(Me.RadioButton_Zip)
        Me.GroupBox_Mode.Controls.Add(Me.RadioButton_Rar)
        Me.GroupBox_Mode.CustomGroupBoxColor = System.Drawing.Color.White
        Me.GroupBox_Mode.Enabled = False
        Me.GroupBox_Mode.GroupImage = Nothing
        Me.GroupBox_Mode.GroupTitle = ""
        Me.GroupBox_Mode.Location = New System.Drawing.Point(8, 231)
        Me.GroupBox_Mode.Name = "GroupBox_Mode"
        Me.GroupBox_Mode.Padding = New System.Windows.Forms.Padding(20)
        Me.GroupBox_Mode.PaintGroupBox = False
        Me.GroupBox_Mode.RoundCorners = 5
        Me.GroupBox_Mode.ShadowColor = System.Drawing.Color.DarkGray
        Me.GroupBox_Mode.ShadowControl = False
        Me.GroupBox_Mode.ShadowThickness = 3
        Me.GroupBox_Mode.Size = New System.Drawing.Size(167, 61)
        Me.GroupBox_Mode.TabIndex = 24
        '
        'RadioButton_TAR
        '
        Me.RadioButton_TAR.AutoSize = True
        Me.RadioButton_TAR.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_TAR.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_TAR.Location = New System.Drawing.Point(66, 35)
        Me.RadioButton_TAR.Name = "RadioButton_TAR"
        Me.RadioButton_TAR.Size = New System.Drawing.Size(41, 17)
        Me.RadioButton_TAR.TabIndex = 7
        Me.RadioButton_TAR.Text = "Tar"
        Me.RadioButton_TAR.UseVisualStyleBackColor = True
        '
        'RadioButton_ISO
        '
        Me.RadioButton_ISO.AutoSize = True
        Me.RadioButton_ISO.Cursor = System.Windows.Forms.Cursors.Hand
        Me.RadioButton_ISO.ForeColor = System.Drawing.Color.Black
        Me.RadioButton_ISO.Location = New System.Drawing.Point(66, 13)
        Me.RadioButton_ISO.Name = "RadioButton_ISO"
        Me.RadioButton_ISO.Size = New System.Drawing.Size(39, 17)
        Me.RadioButton_ISO.TabIndex = 6
        Me.RadioButton_ISO.Text = "Iso"
        Me.RadioButton_ISO.UseVisualStyleBackColor = True
        '
        'Label_Mode
        '
        Me.Label_Mode.BackColor = System.Drawing.Color.Transparent
        Me.Label_Mode.Font = New System.Drawing.Font("Lucida Console", 10.0!)
        Me.Label_Mode.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Label_Mode.Glow = 4
        Me.Label_Mode.GlowColor = System.Drawing.Color.Cyan
        Me.Label_Mode.Location = New System.Drawing.Point(17, 223)
        Me.Label_Mode.Name = "Label_Mode"
        Me.Label_Mode.Size = New System.Drawing.Size(37, 16)
        Me.Label_Mode.TabIndex = 18
        Me.Label_Mode.Text = "Mode"
        Me.Label_Mode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label_Capacity
        '
        Me.Label_Capacity.BackColor = System.Drawing.Color.Transparent
        Me.Label_Capacity.Font = New System.Drawing.Font("Lucida Console", 10.0!)
        Me.Label_Capacity.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Label_Capacity.Glow = 4
        Me.Label_Capacity.GlowColor = System.Drawing.Color.Cyan
        Me.Label_Capacity.Location = New System.Drawing.Point(17, 85)
        Me.Label_Capacity.Name = "Label_Capacity"
        Me.Label_Capacity.Size = New System.Drawing.Size(69, 16)
        Me.Label_Capacity.TabIndex = 25
        Me.Label_Capacity.Text = "Capacity"
        Me.Label_Capacity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label_Information
        '
        Me.Label_Information.BackColor = System.Drawing.Color.Transparent
        Me.Label_Information.Font = New System.Drawing.Font("Lucida Console", 10.0!)
        Me.Label_Information.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.Label_Information.Glow = 4
        Me.Label_Information.GlowColor = System.Drawing.Color.Cyan
        Me.Label_Information.Location = New System.Drawing.Point(190, 223)
        Me.Label_Information.Name = "Label_Information"
        Me.Label_Information.Size = New System.Drawing.Size(94, 16)
        Me.Label_Information.TabIndex = 26
        Me.Label_Information.Text = "Information"
        Me.Label_Information.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label_Input
        '
        Me.Label_Input.AutoSize = True
        Me.Label_Input.BackColor = System.Drawing.Color.Transparent
        Me.Label_Input.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Input.ForeColor = System.Drawing.Color.Black
        Me.Label_Input.Location = New System.Drawing.Point(15, 38)
        Me.Label_Input.Name = "Label_Input"
        Me.Label_Input.Size = New System.Drawing.Size(41, 15)
        Me.Label_Input.TabIndex = 20
        Me.Label_Input.Text = "Input :"
        '
        'Label_Output
        '
        Me.Label_Output.AutoSize = True
        Me.Label_Output.BackColor = System.Drawing.Color.Transparent
        Me.Label_Output.Font = New System.Drawing.Font("Arial", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Output.ForeColor = System.Drawing.Color.Black
        Me.Label_Output.Location = New System.Drawing.Point(6, 64)
        Me.Label_Output.Name = "Label_Output"
        Me.Label_Output.Size = New System.Drawing.Size(51, 15)
        Me.Label_Output.TabIndex = 27
        Me.Label_Output.Text = "Output :"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.BackColor = System.Drawing.Color.Silver
        Me.MenuStrip1.BackgroundImage = Global.Splitty.My.Resources.Resources.Background
        Me.MenuStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LanguageToolStripMenuItem, Me.AboutToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(434, 24)
        Me.MenuStrip1.TabIndex = 29
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'LanguageToolStripMenuItem
        '
        Me.LanguageToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EnglishToolStripMenuItem, Me.SpanishToolStripMenuItem})
        Me.LanguageToolStripMenuItem.ForeColor = System.Drawing.Color.Black
        Me.LanguageToolStripMenuItem.Name = "LanguageToolStripMenuItem"
        Me.LanguageToolStripMenuItem.Size = New System.Drawing.Size(71, 20)
        Me.LanguageToolStripMenuItem.Text = "Language"
        '
        'EnglishToolStripMenuItem
        '
        Me.EnglishToolStripMenuItem.Image = Global.Splitty.My.Resources.Resources.United_States
        Me.EnglishToolStripMenuItem.Name = "EnglishToolStripMenuItem"
        Me.EnglishToolStripMenuItem.Size = New System.Drawing.Size(115, 22)
        Me.EnglishToolStripMenuItem.Text = "English"
        '
        'SpanishToolStripMenuItem
        '
        Me.SpanishToolStripMenuItem.Image = Global.Splitty.My.Resources.Resources.Spain
        Me.SpanishToolStripMenuItem.Name = "SpanishToolStripMenuItem"
        Me.SpanishToolStripMenuItem.Size = New System.Drawing.Size(115, 22)
        Me.SpanishToolStripMenuItem.Text = "Spanish"
        '
        'AboutToolStripMenuItem
        '
        Me.AboutToolStripMenuItem.ForeColor = System.Drawing.Color.Black
        Me.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem"
        Me.AboutToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.AboutToolStripMenuItem.Text = "About..."
        '
        'FileSystemWatcher1
        '
        Me.FileSystemWatcher1.EnableRaisingEvents = True
        Me.FileSystemWatcher1.SynchronizingObject = Me
        '
        'Form1
        '
        Me.BackColor = System.Drawing.Color.Black
        Me.BackgroundImage = Global.Splitty.My.Resources.Resources.Background
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(434, 362)
        Me.Controls.Add(Me.Button_Split)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.Label_Capacity)
        Me.Controls.Add(Me.Label_Mode)
        Me.Controls.Add(Me.Label_Information)
        Me.Controls.Add(Me.GroupBox_Mode)
        Me.Controls.Add(Me.GroupBox_Information)
        Me.Controls.Add(Me.ProgBarPlus)
        Me.Controls.Add(Me.GroupPanel_Options)
        Me.Controls.Add(Me.Button_Output_Folder)
        Me.Controls.Add(Me.TextBox_Output_Folder)
        Me.Controls.Add(Me.Button_Input_Folder)
        Me.Controls.Add(Me.TextBox_Input_Folder)
        Me.Controls.Add(Me.Label_Output)
        Me.Controls.Add(Me.Label_Input)
        Me.Controls.Add(Me.Button_Stop)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.GroupPanel_Options.ResumeLayout(False)
        Me.GroupPanel_Options.PerformLayout()
        CType(Me.PictureBox_Disc, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox_Information.ResumeLayout(False)
        Me.GroupBox_Information.PerformLayout()
        Me.GroupBox_Mode.ResumeLayout(False)
        Me.GroupBox_Mode.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.FileSystemWatcher1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button_Split As CButtonLib.CButton
    Friend WithEvents TextBox_Input_Folder As TextBoxHint.TextBoxHint
    Friend WithEvents VistaFolderBrowserDialog1 As Ookii.Dialogs.VistaFolderBrowserDialog
    Friend WithEvents Button_Input_Folder As CButtonLib.CButton
    Friend WithEvents RadioButton_CD As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_CD800 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_DVD5 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_DVD9 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_BluRay As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_BluRay_DL As System.Windows.Forms.RadioButton
    Friend WithEvents GroupPanel_Options As CodeVendor.Controls.Grouper
    Friend WithEvents ComboBox_Custom_Size As System.Windows.Forms.ComboBox
    Friend WithEvents TextBox_Custom_Size As TextBoxHint.TextBoxHint
    Friend WithEvents RadioButton_Custom_Size As System.Windows.Forms.RadioButton
    Friend WithEvents TextBox_Output_Folder As TextBoxHint.TextBoxHint
    Friend WithEvents Button_Output_Folder As CButtonLib.CButton
    Friend WithEvents PictureBox_Disc As System.Windows.Forms.PictureBox
    Friend WithEvents CButton1 As CButtonLib.CButton
    Friend WithEvents Button_Folder As CButtonLib.CButton
    Friend WithEvents TextBox_Output_Dir As TextBoxHint.TextBoxHint
    Friend WithEvents TextBox_Folder As TextBoxHint.TextBoxHint
    Friend WithEvents ProgBarPlus As ProgBar.ProgBarPlus
    Friend WithEvents Button_Stop As CButtonLib.CButton
    Friend WithEvents RadioButton_SFX As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_Zip As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_Rar As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_Copy As System.Windows.Forms.RadioButton
    Friend WithEvents Label_Size_Value As System.Windows.Forms.Label
    Friend WithEvents Label_Discs_Value As System.Windows.Forms.Label
    Friend WithEvents RadioButton_BluRay_MiniDisc As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_DVD10 As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_CD900 As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox_Information As CodeVendor.Controls.Grouper
    Friend WithEvents GroupBox_Mode As CodeVendor.Controls.Grouper
    Friend WithEvents Label_Mode As gLabel.gLabel
    Friend WithEvents Label_Capacity As gLabel.gLabel
    Friend WithEvents Label_Information As gLabel.gLabel
    Friend WithEvents Label_Discs_Name As System.Windows.Forms.Label
    Friend WithEvents Label_Size_Name As System.Windows.Forms.Label
    Friend WithEvents Label_Input As System.Windows.Forms.Label
    Friend WithEvents Label_Output As System.Windows.Forms.Label
    Friend WithEvents RadioButton_ISO As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_TAR As System.Windows.Forms.RadioButton
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents LanguageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EnglishToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SpanishToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FileSystemWatcher1 As System.IO.FileSystemWatcher

End Class
