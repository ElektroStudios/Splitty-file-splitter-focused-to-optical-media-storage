<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class About
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
        Dim DesignerRectTracker1 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(About))
        Dim CBlendItems1 As CButtonLib.cBlendItems = New CButtonLib.cBlendItems()
        Dim DesignerRectTracker2 As CButtonLib.DesignerRectTracker = New CButtonLib.DesignerRectTracker()
        Me.LogoPictureBox = New System.Windows.Forms.PictureBox()
        Me.TextBoxDescription = New System.Windows.Forms.TextBox()
        Me.CButton1 = New CButtonLib.CButton()
        Me.GLabel1 = New gLabel.gLabel()
        Me.GLabel2 = New gLabel.gLabel()
        CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LogoPictureBox
        '
        Me.LogoPictureBox.Image = Global.Splitty.My.Resources.Resources.Elektro
        Me.LogoPictureBox.Location = New System.Drawing.Point(182, 56)
        Me.LogoPictureBox.Name = "LogoPictureBox"
        Me.LogoPictureBox.Size = New System.Drawing.Size(220, 176)
        Me.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.LogoPictureBox.TabIndex = 1
        Me.LogoPictureBox.TabStop = False
        '
        'TextBoxDescription
        '
        Me.TextBoxDescription.Location = New System.Drawing.Point(12, 56)
        Me.TextBoxDescription.Margin = New System.Windows.Forms.Padding(6, 3, 3, 3)
        Me.TextBoxDescription.Multiline = True
        Me.TextBoxDescription.Name = "TextBoxDescription"
        Me.TextBoxDescription.ReadOnly = True
        Me.TextBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxDescription.Size = New System.Drawing.Size(164, 149)
        Me.TextBoxDescription.TabIndex = 5
        Me.TextBoxDescription.TabStop = False
        '
        'CButton1
        '
        Me.CButton1.BorderColor = System.Drawing.Color.Black
        DesignerRectTracker1.IsActive = False
        DesignerRectTracker1.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker1.TrackerRectangle"), System.Drawing.RectangleF)
        Me.CButton1.CenterPtTracker = DesignerRectTracker1
        CBlendItems1.iColor = New System.Drawing.Color() {System.Drawing.Color.Silver, System.Drawing.Color.Black, System.Drawing.Color.Gray}
        CBlendItems1.iPoint = New Single() {0!, 0.5!, 1.0!}
        Me.CButton1.ColorFillBlend = CBlendItems1
        Me.CButton1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.CButton1.FocalPoints.CenterPtX = 0.484472!
        Me.CButton1.FocalPoints.CenterPtY = 0.56!
        DesignerRectTracker2.IsActive = False
        DesignerRectTracker2.TrackerRectangle = CType(resources.GetObject("DesignerRectTracker2.TrackerRectangle"), System.Drawing.RectangleF)
        Me.CButton1.FocusPtTracker = DesignerRectTracker2
        Me.CButton1.ImageIndex = 0
        Me.CButton1.Location = New System.Drawing.Point(12, 210)
        Me.CButton1.Name = "CButton1"
        Me.CButton1.Size = New System.Drawing.Size(164, 23)
        Me.CButton1.TabIndex = 7
        Me.CButton1.Text = "&OK"
        '
        'GLabel1
        '
        Me.GLabel1.BackColor = System.Drawing.Color.Transparent
        Me.GLabel1.Font = New System.Drawing.Font("Arial", 15.0!)
        Me.GLabel1.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.GLabel1.Glow = 4
        Me.GLabel1.GlowColor = System.Drawing.Color.Cyan
        Me.GLabel1.Location = New System.Drawing.Point(12, 8)
        Me.GLabel1.Name = "GLabel1"
        Me.GLabel1.Size = New System.Drawing.Size(390, 19)
        Me.GLabel1.TabIndex = 8
        Me.GLabel1.Text = "Splitty 1.7"
        '
        'GLabel2
        '
        Me.GLabel2.BackColor = System.Drawing.Color.Transparent
        Me.GLabel2.Font = New System.Drawing.Font("Arial", 15.0!)
        Me.GLabel2.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.GLabel2.Glow = 4
        Me.GLabel2.GlowColor = System.Drawing.Color.Cyan
        Me.GLabel2.Location = New System.Drawing.Point(12, 32)
        Me.GLabel2.Name = "GLabel2"
        Me.GLabel2.Size = New System.Drawing.Size(390, 19)
        Me.GLabel2.TabIndex = 9
        Me.GLabel2.Text = "© ElektroStudios 2013-2024"
        '
        'About
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Gray
        Me.BackgroundImage = Global.Splitty.My.Resources.Resources.Background
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(414, 242)
        Me.Controls.Add(Me.GLabel2)
        Me.Controls.Add(Me.GLabel1)
        Me.Controls.Add(Me.CButton1)
        Me.Controls.Add(Me.TextBoxDescription)
        Me.Controls.Add(Me.LogoPictureBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "About"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents TextBoxDescription As System.Windows.Forms.TextBox
    Friend WithEvents CButton1 As CButtonLib.CButton
    Friend WithEvents GLabel1 As gLabel.gLabel
    Friend WithEvents GLabel2 As gLabel.gLabel
End Class
