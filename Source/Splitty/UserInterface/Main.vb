Imports System.Runtime.InteropServices

Imports Microsoft.Win32

Imports Splitty.Core

Public Class Main

#Region " Declarations "

    Public SelectedDirSizeBytes As Long
    Public SelectedDirSizeConvertedBytes As String
    Public SelectedDiscFormat As String
    Public SelectedDiscBytes As Long

    Public CopyMode As SplitMode = SplitMode.Copy

    Public FilesList As New List(Of SourceFileEntry)
    Public TotalFilesNumber As Integer

    Public ThreadProcess As System.Threading.Thread
    Public ThreadProcessIsCompleted As Boolean = False

    Public ThreadSplit As System.Threading.Thread
    Public ThreadIsCompleted As Boolean = False
    Public WantToCancelThread As Boolean = False

    Public TempDir As String = System.IO.Path.GetTempPath()

    Public Shared LanguageResource = "ENG"

    Dim architecture = Me.Get_OS_Architecture()

    ''' <summary>
    ''' The maximum number of times PowerISO is executed to create the ISO image of a disc before giving up.
    ''' </summary>
    Private Const MaxIsoAttempts As Integer = 3

    ''' <summary>
    ''' The source directory currently locked with <see cref="FolderLock.Lock"/>, or <see langword="Nothing"/>.
    ''' </summary>
    Private lockedDirectory As String

    ''' <summary>
    ''' The exception thrown by the split thread, or <see langword="Nothing"/> if the split succeeded.
    ''' </summary>
    Private splitError As Exception

    ''' <summary>
    ''' Indicates whether the "reload source folder" question is currently shown.
    ''' </summary>
    Private isAskingReload As Boolean

#End Region

#Region " Properties "

    Public Property SelectedDirectory() As String
        Get
            Return Me.TextBox_Input_Folder.Text
        End Get
        Set(value As String)
            Me.TextBox_Input_Folder.Text = value
        End Set
    End Property

    Public Property SelectedOutputDirectory() As String
        Get
            Return Me.TextBox_Output_Folder.Text
        End Get
        Set(value As String)
            Me.TextBox_Output_Folder.Text = value
        End Set
    End Property

#End Region

#Region " Form "

#Region " Load/Close "

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = My.Settings.Version
        Me.PictureBox_Disc.BackgroundImage = Me.GrayScale_Image(Me.PictureBox_Disc.BackgroundImage, GrayScale.MidGray)
        Me.Button_Split.Image = Me.GrayScale_Image(Me.Button_Split.Image, GrayScale.MidGray)
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Me.ThreadSplit IsNot Nothing AndAlso Me.ThreadSplit.IsAlive Then
            Dim answer As DialogResult = MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "01"), My.Settings.Version, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If answer = DialogResult.Yes Then
                Me.WantToCancelThread = True
                Me.Kill_Process("Splitty_7zip")
                Me.Kill_Process("Splitty_WinRar")
                Me.Kill_Process("Splitty_Piso")
                Try : Me.ThreadSplit.Abort() : Catch : End Try
                Me.Unlock_Locked_Directory()
                End
            Else
                e.Cancel = True
            End If
        Else
            Me.Unlock_Locked_Directory()
        End If


    End Sub

    ''' <summary>
    ''' Raises the HandleCreated event and applies the dark title bar if the user's system theme is set to dark.
    ''' </summary>
    ''' <param name="e">An EventArgs that contains the event data.</param>
    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Main.ApplyDarkTitleBar(Me.Handle)
    End Sub

#End Region

#Region " Buttons folders "

    ' Input folder
    Private Sub Button_Input_Folder_Click(sender As Object, e As MouseEventArgs) Handles Button_Input_Folder.ClickButtonArea, Button_Folder.ClickButtonArea
        Dim selectFolder As New Ookii.Dialogs.VistaFolderBrowserDialog With {
            .ShowNewFolderButton = True
        }
        If selectFolder.ShowDialog() = DialogResult.OK Then
            Me.Select_Input_Directory(selectFolder.SelectedPath)
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Output folder
    Private Sub Button_Output_Folder_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Output_Folder.ClickButtonArea, CButton1.ClickButtonArea
        Dim selectFolder As New Ookii.Dialogs.VistaFolderBrowserDialog With {
            .ShowNewFolderButton = True
        }
        If selectFolder.ShowDialog() = DialogResult.OK Then
            Me.Select_Output_Directory(selectFolder.SelectedPath)
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ''' <summary>
    ''' Validates and applies a new input (source) directory.
    ''' </summary>
    ''' <param name="directoryPath">The selected directory.</param>
    Private Sub Select_Input_Directory(directoryPath As String)

        If Not String.IsNullOrEmpty(Me.SelectedOutputDirectory) Then
            If SplitPaths.AreSameDirectory(directoryPath, Me.SelectedOutputDirectory) Then
                MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "04"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                Me.Reset_Input_Selection()
                Exit Sub
            End If
            If SplitPaths.IsSameOrInside(Me.SelectedOutputDirectory, directoryPath) Then
                MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "33"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                Me.Reset_Input_Selection()
                Exit Sub
            End If
        End If

        Dim directorySize As Long
        Try
            directorySize = DirectoryScanner.GetDirectorySize(directoryPath)
        Catch ex As Exception When TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is IO.IOException OrElse TypeOf ex Is Security.SecurityException
            MessageBox.Show(ex.Message, My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
            Me.Reset_Input_Selection()
            Exit Sub
        End Try

        Me.SelectedDirectory = directoryPath
        Me.Create_FileSystemWatcher(Me.SelectedDirectory)
        Me.Set_Input_Directory_Size(directorySize)
        Me.Button_Output_Folder.Enabled = True
        Me.TextBox_Output_Folder.Enabled = True
        Me.GroupPanel_Options.Enabled = True
        Me.GroupBox_Mode.Enabled = True
        Me.RadioButton_CD.Checked = False
        Me.RadioButton_CD.Checked = True
        Me.Label_Discs_Name.Visible = True
        Me.Label_Discs_Value.Visible = True
        Me.Label_Size_Name.Visible = True
        Me.Label_Size_Value.Visible = True
        Me.TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetString(LanguageResource & "02")
        Me.Update_Split_Button_State()

    End Sub

    ''' <summary>
    ''' Clears the input directory selection and disables the controls that depend on it.
    ''' </summary>
    Private Sub Reset_Input_Selection()

        Me.SelectedDirectory = Nothing
        Me.Button_Split.Enabled = False
        Me.GroupPanel_Options.Enabled = False
        Me.GroupBox_Mode.Enabled = False
        Me.GroupBox_Information.Enabled = False
        Me.Label_Size_Value.Text = "0"
        Me.Label_Discs_Value.Text = "0"

    End Sub

    ''' <summary>
    ''' Validates and applies a new output directory.
    ''' </summary>
    ''' <param name="directoryPath">The selected directory.</param>
    Private Sub Select_Output_Directory(directoryPath As String)

        If Not String.IsNullOrEmpty(Me.SelectedDirectory) Then
            If SplitPaths.AreSameDirectory(directoryPath, Me.SelectedDirectory) Then
                MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "03"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                Me.SelectedOutputDirectory = Nothing
                Me.Update_Split_Button_State()
                Exit Sub
            End If
            ' The source directory is locked (write-denied, inherited) while splitting,
            ' so the output directory cannot be inside it.
            If SplitPaths.IsSameOrInside(directoryPath, Me.SelectedDirectory) Then
                MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "33"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                Me.SelectedOutputDirectory = Nothing
                Me.Update_Split_Button_State()
                Exit Sub
            End If
        End If

        Me.SelectedOutputDirectory = directoryPath
        Me.Update_Split_Button_State()

    End Sub

    ''' <summary>
    ''' Enables the split button only when input directory, output directory and a valid disc size are set.
    ''' </summary>
    Private Sub Update_Split_Button_State()

        Dim ready As Boolean = Not String.IsNullOrEmpty(Me.SelectedDirectory) AndAlso
                               Not String.IsNullOrEmpty(Me.SelectedOutputDirectory) AndAlso
                               Me.SelectedDiscBytes > 0
        If ready Then
            Me.Button_Split.Image = My.Resources.split
            Me.Button_Split.ForeColor = Color.Black
        End If
        Me.Button_Split.Enabled = ready

    End Sub

#End Region

#Region " Language "

    Private Sub EnglishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnglishToolStripMenuItem.Click
        LanguageResource = "ENG"
        Me.Label_Input.Location = New Point(15, Me.Label_Input.Location.Y)
        Me.Label_Capacity.Size = New Point(69, Me.Label_Capacity.Size.Height)
        Me.Label_Discs_Name.Size = New Point(54, Me.Label_Discs_Name.Size.Height)
        Me.Label_Size_Name.Size = New Point(60, Me.Label_Size_Name.Size.Height)
        Me.Label_Discs_Value.Location = New Point(58, Me.Label_Discs_Value.Location.Y)
        Me.Label_Size_Value.Location = New Point(58, Me.Label_Size_Value.Location.Y)
        Me.Change_Lang()
    End Sub

    Private Sub SpanishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpanishToolStripMenuItem.Click
        LanguageResource = "SPA"
        Me.Label_Input.Location = New Point(0, Me.Label_Input.Location.Y)
        Me.Label_Capacity.Size = New Point(78, Me.Label_Capacity.Size.Height)
        Me.Label_Discs_Name.Size = New Point(64, Me.Label_Discs_Name.Size.Height)
        Me.Label_Size_Name.Size = New Point(64, Me.Label_Size_Name.Size.Height)
        Me.Label_Discs_Value.Location = New Point(68, Me.Label_Discs_Value.Location.Y)
        Me.Label_Size_Value.Location = New Point(68, Me.Label_Size_Value.Location.Y)
        Me.Change_Lang()
    End Sub


    Private Sub Change_Lang()
        Me.Label_Input.Text = My.Resources.ResourceManager.GetString(LanguageResource & "11")
        Me.Label_Output.Text = My.Resources.ResourceManager.GetString(LanguageResource & "12")
        Me.Label_Capacity.Text = My.Resources.ResourceManager.GetString(LanguageResource & "13")
        Me.Label_Mode.Text = My.Resources.ResourceManager.GetString(LanguageResource & "14")
        Me.Label_Information.Text = My.Resources.ResourceManager.GetString(LanguageResource & "15")
        Me.Label_Size_Name.Text = My.Resources.ResourceManager.GetString(LanguageResource & "16")
        Me.Label_Discs_Name.Text = My.Resources.ResourceManager.GetString(LanguageResource & "17")
        Me.Button_Split.Text = My.Resources.ResourceManager.GetString(LanguageResource & "18")
        Me.Button_Stop.Text = My.Resources.ResourceManager.GetString(LanguageResource & "19")
        Me.RadioButton_Copy.Text = My.Resources.ResourceManager.GetString(LanguageResource & "20")
        Me.RadioButton_Custom_Size.Text = My.Resources.ResourceManager.GetString(LanguageResource & "21")
        Me.AboutToolStripMenuItem.Text = My.Resources.ResourceManager.GetString(LanguageResource & "22")
        Me.LanguageToolStripMenuItem.Text = My.Resources.ResourceManager.GetString(LanguageResource & "23")
        Me.EnglishToolStripMenuItem.Text = My.Resources.ResourceManager.GetString(LanguageResource & "24")
        Me.SpanishToolStripMenuItem.Text = My.Resources.ResourceManager.GetString(LanguageResource & "25")
        Me.TextBox_Input_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetString(LanguageResource & "02")
        If Not Me.TextBox_Output_Folder.HintDetails.HintText = "" Then Me.TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetString(LanguageResource & "02")
        Me.Update_Discs_Label()
    End Sub

#End Region

#Region " Drag&Drop Input Folder "

    ' FolderTextbox Drag-Drop
    Private Sub Input_Folder_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox_Input_Folder.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim folderObject As String() = e.Data.GetData(DataFormats.FileDrop)
            If System.IO.Directory.Exists(folderObject(0)) Then
                Me.Select_Input_Directory(folderObject(0))
            Else
                MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "05"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub Output_Folder_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox_Output_Folder.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim folderObject As String() = e.Data.GetData(DataFormats.FileDrop)
            If System.IO.Directory.Exists(folderObject(0)) Then
                Me.Select_Output_Directory(folderObject(0))
            Else
                MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "05"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub


    ' FolderTextbox Drag-Enter
    Private Sub Folder_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox_Input_Folder.DragEnter, TextBox_Output_Folder.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then e.Effect = DragDropEffects.All
    End Sub

#End Region

#Region " FileSystem Watcher "

    Private Sub Create_FileSystemWatcher(path As String)
        ' The event handlers are attached once, with the "Handles" clause of Watcher_Changed.
        ' (Before, a new set of handlers was added each time a folder was selected, so the
        '  reload question was shown several times per change.)
        Me.FileSystemWatcher1.Path = path
    End Sub

    Private Sub Watcher_Changed(sender As Object, e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Created, FileSystemWatcher1.Deleted, FileSystemWatcher1.Renamed
        If Not Me.Button_Split.Visible OrElse Me.isAskingReload OrElse String.IsNullOrEmpty(Me.SelectedDirectory) Then
            Exit Sub
        End If

        Me.isAskingReload = True
        Try
            Dim answer As DialogResult = MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "32"), My.Settings.Version, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If answer = DialogResult.Yes Then
                Try
                    Me.Set_Input_Directory_Size(DirectoryScanner.GetDirectorySize(Me.SelectedDirectory))
                Catch ex As Exception When TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is IO.IOException OrElse TypeOf ex Is Security.SecurityException
                    MessageBox.Show(ex.Message, My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                End Try
            End If
        Finally
            Me.isAskingReload = False
        End Try
    End Sub

    ''' <summary>
    ''' Sets the size of the input directory and updates the related labels.
    ''' </summary>
    ''' <param name="directorySize">The directory size, in bytes.</param>
    Private Sub Set_Input_Directory_Size(directorySize As Long)

        Me.SelectedDirSizeBytes = directorySize
        Me.SelectedDirSizeConvertedBytes = ByteSizeFormatter.Format(Me.SelectedDirSizeBytes)
        Me.Label_Size_Value.Text = Me.SelectedDirSizeConvertedBytes
        Me.Update_Discs_Label()

    End Sub

    ''' <summary>
    ''' Updates the label that shows the number of discs needed for the selected disc size.
    ''' </summary>
    Private Sub Update_Discs_Label()

        Dim discCount As Long = If(Me.SelectedDiscBytes > 0,
                                   DiscCapacity.CalculateDiscCount(Me.SelectedDirSizeBytes, Me.SelectedDiscBytes),
                                   0L)
        Dim suffix As String
        Select Case Me.SelectedDiscFormat
            Case "CD", "CD800", "CD900"
                suffix = " CD's"
            Case "DVD5", "DVD9", "DVD10"
                suffix = " DVD's"
            Case "BR", "BR-DL", "BR-MD"
                suffix = " BluRays"
            Case Else
                suffix = My.Resources.ResourceManager.GetString(LanguageResource & "26")
        End Select
        Me.Label_Discs_Value.Text = $"{discCount}{suffix}"

    End Sub

#End Region

#Region " Radio buttons Capacity "

    Private Sub RadioButton_CD_Click(sender As Object, e As EventArgs) Handles RadioButton_CD.CheckedChanged
        If Me.RadioButton_CD.Checked Then Me.Select_Disc_Type(DiscType.CD, "CD", My.Resources.cd)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_CD800_Click(sender As Object, e As EventArgs) Handles RadioButton_CD800.CheckedChanged
        If Me.RadioButton_CD800.Checked Then Me.Select_Disc_Type(DiscType.CD800MB, "CD800", My.Resources.cd)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_CD900_Click(sender As Object, e As EventArgs) Handles RadioButton_CD900.CheckedChanged
        If Me.RadioButton_CD900.Checked Then Me.Select_Disc_Type(DiscType.CD900MB, "CD900", My.Resources.cd)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd5Click(sender As Object, e As EventArgs) Handles RadioButton_DVD5.CheckedChanged
        If Me.RadioButton_DVD5.Checked Then Me.Select_Disc_Type(DiscType.Dvd5, "DVD5", My.Resources.dvd)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd9Click(sender As Object, e As EventArgs) Handles RadioButton_DVD9.CheckedChanged
        If Me.RadioButton_DVD9.Checked Then Me.Select_Disc_Type(DiscType.Dvd9, "DVD9", My.Resources.dvd)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd10Click(sender As Object, e As EventArgs) Handles RadioButton_DVD10.CheckedChanged
        If Me.RadioButton_DVD10.Checked Then Me.Select_Disc_Type(DiscType.Dvd10, "DVD10", My.Resources.dvd)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_Click(sender As Object, e As EventArgs) Handles RadioButton_BluRay.CheckedChanged
        If Me.RadioButton_BluRay.Checked Then Me.Select_Disc_Type(DiscType.BR, "BR", My.Resources.bluray)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_DL_Click(sender As Object, e As EventArgs) Handles RadioButton_BluRay_DL.CheckedChanged
        If Me.RadioButton_BluRay_DL.Checked Then Me.Select_Disc_Type(DiscType.BRDoubleLayer, "BR-DL", My.Resources.bluray)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_MiniDisc_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_BluRay_MiniDisc.CheckedChanged
        If Me.RadioButton_BluRay_MiniDisc.Checked Then Me.Select_Disc_Type(DiscType.BRMiniDisc, "BR-MD", My.Resources.bluray)
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ''' <summary>
    ''' Applies a standard disc format as the selected disc size.
    ''' </summary>
    ''' <param name="discType">The disc format.</param>
    ''' <param name="formatName">The internal format name.</param>
    ''' <param name="image">The image that represents the disc format.</param>
    Private Sub Select_Disc_Type(discType As DiscType, formatName As String, image As Image)

        Me.SelectedDiscFormat = formatName
        Me.SelectedDiscBytes = DiscCapacity.GetCapacity(discType)
        Me.PictureBox_Disc.BackgroundImage = image
        Me.Update_Discs_Label()
        Me.Update_Split_Button_State()

    End Sub

#End Region

#Region " Radio buttons Copy mode "

    Private Sub RadioButtonCopy(sender As Object, e As EventArgs) Handles RadioButton_Copy.CheckedChanged
        If Me.RadioButton_Copy.Checked Then Me.CopyMode = SplitMode.Copy
    End Sub

    Private Sub RadioButtonRar(sender As Object, e As EventArgs) Handles RadioButton_Rar.CheckedChanged
        If Me.RadioButton_Rar.Checked Then Me.CopyMode = SplitMode.Rar
    End Sub

    Private Sub RadioButtonZip(sender As Object, e As EventArgs) Handles RadioButton_Zip.CheckedChanged
        If Me.RadioButton_Zip.Checked Then Me.CopyMode = SplitMode.Zip
    End Sub

    Private Sub RadioButtonSfx(sender As Object, e As EventArgs) Handles RadioButton_SFX.CheckedChanged
        If Me.RadioButton_SFX.Checked Then Me.CopyMode = SplitMode.Sfx
    End Sub

    Private Sub RadioButtonIso(sender As Object, e As EventArgs) Handles RadioButton_ISO.CheckedChanged
        If Me.RadioButton_ISO.Checked Then Me.CopyMode = SplitMode.Iso
    End Sub

    Private Sub RadioButtonTar(sender As Object, e As EventArgs) Handles RadioButton_TAR.CheckedChanged
        If Me.RadioButton_TAR.Checked Then Me.CopyMode = SplitMode.Tar
    End Sub

#End Region

#Region " Custom size option "

    ' RadioButton custom size
    Private Sub RadioButton_Custom(sender As Object, e As EventArgs) Handles RadioButton_Custom_Size.CheckedChanged
        If Me.RadioButton_Custom_Size.Checked Then
            Me.SelectedDiscFormat = "CUSTOM"
            Me.PictureBox_Disc.BackgroundImage = My.Resources.custom
            Me.TextBox_Custom_Size.Enabled = True
            Me.ComboBox_Custom_Size.Enabled = True

            If Me.ComboBox_Custom_Size.SelectedIndex < 0 Then
                Me.ComboBox_Custom_Size.SelectedIndex = 0
            End If
            Me.Apply_Custom_Size()
        Else
            Me.TextBox_Custom_Size.HintDetails.HintText = ""
            Me.TextBox_Custom_Size.Enabled = False
            Me.ComboBox_Custom_Size.Enabled = False
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Textbox custom size
    Private Sub TextBox_Custom_Size_TextChanged(sender As Object, e As EventArgs) Handles TextBox_Custom_Size.TextChanged
        ' Setting the Text property raises this event again with the corrected text.
        If Me.TextBox_Custom_Size.Text.Contains(".") Then
            Me.TextBox_Custom_Size.Text = Me.TextBox_Custom_Size.Text.Replace(".", ",")
            Exit Sub
        End If
        If Me.TextBox_Custom_Size.Text.Length = 0 Then
            Me.TextBox_Custom_Size.Text = "0"
            Exit Sub
        End If

        If Me.RadioButton_Custom_Size.Checked Then
            Me.Apply_Custom_Size()
        End If
    End Sub

    ' Textbox custom size (Keypress)
    Private Sub TextBox_Custom_Size_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox_Custom_Size.KeyPress
        e.Handled = CustomSize.IsKeyRejected(e.KeyChar, Me.TextBox_Custom_Size.Text)
    End Sub

    ' ComboBox custom size
    Private Sub ComboBox_Custom_Size_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_Custom_Size.SelectedIndexChanged
        If Me.RadioButton_Custom_Size.Checked Then
            Me.Apply_Custom_Size()
        End If
    End Sub

    ''' <summary>
    ''' Parses the custom size typed by the user and applies it as the selected disc size.
    ''' An invalid or zero size disables the split button.
    ''' </summary>
    Private Sub Apply_Custom_Size()

        Dim unit As CustomSizeUnit = CType(Math.Max(0, Me.ComboBox_Custom_Size.SelectedIndex), CustomSizeUnit)
        Dim bytes As Long
        Me.SelectedDiscBytes = If(CustomSize.TryParseBytes(Me.TextBox_Custom_Size.Text, unit, bytes), bytes, 0)
        Me.Update_Discs_Label()
        Me.Update_Split_Button_State()

    End Sub

#End Region

#Region " Button Split "

    ' Button SPLIT
    Private Sub Button_Split_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Split.ClickButtonArea

        If Me.RadioButton_Custom_Size.Checked = True And Me.SelectedDiscBytes > Me.SelectedDirSizeBytes Then
            MessageBox.Show(
                My.Resources.ResourceManager.GetString(LanguageResource & "29") & vbNewLine & vbNewLine &
                My.Resources.ResourceManager.GetString(LanguageResource & "30") & Me.SelectedDirSizeConvertedBytes & vbNewLine &
                My.Resources.ResourceManager.GetString(LanguageResource & "31") & ByteSizeFormatter.Format(Me.SelectedDiscBytes) _
                , My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Exit Sub
        End If

        Dim tempImage = Me.PictureBox_Disc.BackgroundImage
        Me.PictureBox_Disc.BackgroundImage = Me.GrayScale_Image(Me.PictureBox_Disc.BackgroundImage, GrayScale.MidGray)

        Me.splitError = Nothing
        Me.ThreadProcessIsCompleted = False
        Me.ThreadIsCompleted = False
        Me.ThreadSplit = New Threading.Thread(AddressOf Me.Split_Thread) With {
            .IsBackground = True
        }

        Me.Button_Split.Enabled = False
        Me.Button_Split.Visible = False
        Me.Button_Stop.Visible = True
        Me.Button_Stop.Enabled = True
        Me.GroupPanel_Options.Enabled = False
        Me.GroupBox_Mode.Enabled = False
        Me.TextBox_Output_Folder.Enabled = False
        Me.TextBox_Input_Folder.Enabled = False
        Me.Button_Input_Folder.Enabled = False
        Me.Button_Output_Folder.Enabled = False

        Me.ThreadSplit.Start()
        While Not Me.ThreadIsCompleted = True
            Application.DoEvents()
            Threading.Thread.Sleep(15) ' Avoids 100% CPU usage of the UI thread while waiting.
        End While

        If Me.splitError IsNot Nothing Then
            MessageBox.Show($"{My.Resources.ResourceManager.GetString(LanguageResource & "34")}{vbNewLine}{vbNewLine}{Me.splitError.Message}", My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        ElseIf Not Me.WantToCancelThread Then
            MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "09"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        End If
        Me.InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Me.Set_TaskBar_Status(TaskBarStatus.Disabled)

        Me.WantToCancelThread = False
        Me.ProgBarPlus.TextShow = ProgBar.ProgBarPlus.eTextShow.None
        Me.Button_Stop.Visible = False
        Me.Button_Stop.Enabled = False
        Me.Button_Split.Enabled = True
        Me.Button_Split.Visible = True
        Me.GroupPanel_Options.Enabled = True
        Me.GroupBox_Mode.Enabled = True
        Me.TextBox_Output_Folder.Enabled = True
        Me.TextBox_Input_Folder.Enabled = True
        Me.Button_Input_Folder.Enabled = True
        Me.Button_Output_Folder.Enabled = True
        Me.PictureBox_Disc.BackgroundImage = tempImage

        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Button STOP
    Private Sub Button_Stop_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Stop.ClickButtonArea
        Me.WantToCancelThread = True
        Me.Set_TaskBar_Value(100, 100)
        Me.InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Me.Set_TaskBar_Status(TaskBarStatus.Stopped)
        Me.Kill_Process("Splitty_7zip")
        Me.Kill_Process("Splitty_WinRar")
        Me.Kill_Process("Splitty_Piso")
        MessageBox.Show(My.Resources.ResourceManager.GetString(LanguageResource & "28"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        Me.Set_TaskBar_Value(0, 100)
        Me.InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Me.Set_TaskBar_Status(TaskBarStatus.Disabled)
    End Sub

#End Region

#Region "  Menu "

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        About.ShowDialog()
    End Sub

#End Region

#End Region

#Region " Methods "

#Region " Folder Lock Functions "

    ''' <summary>
    ''' Locks the source directory (see <see cref="FolderLock"/>) so its contents cannot be modified while splitting.
    ''' </summary>
    Private Sub Lock_Selected_Directory()

        FolderLock.Lock(Me.SelectedDirectory)
        Me.lockedDirectory = Me.SelectedDirectory

    End Sub

    ''' <summary>
    ''' Unlocks the source directory previously locked by <see cref="Lock_Selected_Directory"/>, if any.
    ''' </summary>
    Private Sub Unlock_Locked_Directory()

        Dim directoryPath As String = Threading.Interlocked.Exchange(Me.lockedDirectory, Nothing)
        If directoryPath IsNot Nothing AndAlso IO.Directory.Exists(directoryPath) Then
            FolderLock.Unlock(directoryPath)
        End If

    End Sub

#End Region

#Region " Flush memory Function "

    Declare Function SetProcessWorkingSetSize Lib "kernel32.dll" (process As IntPtr, minimumWorkingSetSize As Integer, maximumWorkingSetSize As Integer) As Integer

    Public Function Flush_Memory(processToFlush) As Boolean
        Try
            GC.Collect()
            GC.WaitForPendingFinalizers()
            If (Environment.OSVersion.Platform = PlatformID.Win32NT) Then
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1)
                Dim myProcesses As Process() = Process.GetProcessesByName(processToFlush)
                Dim myProcess As Process
                'Dim ProcessInfo As Process
                For Each myProcess In myProcesses
                    Application.DoEvents()
                    SetProcessWorkingSetSize(myProcess.Handle, -1, -1)
                Next myProcess
            End If
            Return True
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
        Return False
    End Function

#End Region

#Region " GrayScale Image Function "

    Enum GrayScale
        LightGray
        MidGray
        DarkGray
    End Enum

    Private Function GrayScale_Image(image As Image, grayTone As GrayScale) As Bitmap
        Dim imageBitmap As New Bitmap(image.Width, image.Height)
        Dim imageGraphic As Graphics = Graphics.FromImage(imageBitmap)
        Dim colorMatrix As System.Drawing.Imaging.ColorMatrix = Nothing
        Select Case grayTone
            Case GrayScale.LightGray : colorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0.5, 0.5, 0.5, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
            Case GrayScale.MidGray : colorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0.5, 0.5, 0.5, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
            Case GrayScale.DarkGray : colorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
        End Select
        Dim imageAttributes As New System.Drawing.Imaging.ImageAttributes()
        imageAttributes.SetColorMatrix(colorMatrix)
        imageGraphic.DrawImage(image, New Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes)
        imageGraphic.Dispose()
        Return imageBitmap
    End Function

#End Region

#Region " Run Process Function "

    Private Function Run_Process(processName As String, Optional processArguments As String = Nothing, Optional readOutput As Boolean = False, Optional processHide As Boolean = False)

        Try

            Dim myProcess As New Process()
            Dim myProcessInfo As New ProcessStartInfo With {
                .FileName = processName, ' Process filename
                .Arguments = processArguments, ' Process arguments
                .CreateNoWindow = processHide, ' Show or hide the process Window
                .UseShellExecute = False, ' Don't use system shell to execute the process
                .RedirectStandardOutput = readOutput, '  Redirect (1) Output
                .RedirectStandardError = readOutput ' Redirect non (1) Output
                }
            myProcess.EnableRaisingEvents = True ' Raise events
            myProcess.StartInfo = myProcessInfo
            myProcess.Start() ' Run the process NOW

            myProcess.WaitForExit() ' Wait X ms to kill the process (Default value is 999999999 ms which is 277 Hours)

            Dim errorlevel = myProcess.ExitCode ' Stores the ExitCode of the process
            If Not errorlevel = 0 Then Return False ' Returns the Exitcode if is not 0

            If readOutput = True Then
                Dim processErrorOutput As String = myProcess.StandardOutput.ReadToEnd() ' Stores the Error Output (If any)
                Dim processStandardOutput As String = myProcess.StandardOutput.ReadToEnd() ' Stores the Standard Output (If any)
                ' Return output by priority
                If processErrorOutput IsNot Nothing Then Return processErrorOutput ' Returns the ErrorOutput (if any)
                If processStandardOutput IsNot Nothing Then Return processStandardOutput ' Returns the StandardOutput (if any)
            End If

        Catch
            'MsgBox(ex.Message)
            Return Nothing ' Returns nothing if the process can't be found or started.
        End Try

        Return True ' Returns True if Read_Output argument is set to False and the process finished without errors.

    End Function

#End Region

#Region " Kill Process Function "

    Private Function Kill_Process(ByRef processName As String) As Boolean
        Dim proc() As Process = Process.GetProcesses
        For procNum As Integer = 0 To proc.GetUpperBound(0)
            Application.DoEvents()
            If proc(procNum).ProcessName.ToUpper = processName.ToUpper Then
                Try : proc(procNum).Kill() : Return True ' Process killed OK
                Catch : Return False : End Try ' Can't kill process
            End If
        Next
        Return Nothing ' Process not found
    End Function

#End Region

#Region " Load Resource To Disk Function "

    Private Function Load_Resource_To_Disk(resource As Byte(), targetFile As String) As Boolean
        Try
            Dim bufferFileStream As New IO.FileStream(targetFile, IO.FileMode.Create, IO.FileAccess.Write)
            bufferFileStream.Write(resource, 0, resource.Length) : bufferFileStream.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

#End Region

#Region " Invoke Control "

    Public Sub InvokeControl(Of T As Control)(control As T, action As Action(Of T))
        If control.InvokeRequired Then
            control.Invoke(New Action(Of T, Action(Of T))(AddressOf Me.InvokeControl), New Object() {control, action})
        Else
            action(control)
        End If
    End Sub

#End Region

#Region " Get OS Architecture Function "

    Private Function Get_OS_Architecture() As Integer
        Dim bits = Runtime.InteropServices.Marshal.SizeOf(GetType(IntPtr)) * 8
        Select Case bits
            Case 32 : Return 32 ' x86
            Case 64 : Return 64 ' x64
            Case Else : Return Nothing ' 128 bits? xD
        End Select
    End Function

#End Region

#Region " Hex to Byte-Array Function "

    Private Function HexToByteArray(hexString As String) As Byte()
        Dim bytesArray((hexString.Length \ 2) - 1) As Byte
        For i As Integer = 0 To hexString.Length - 1 Step 2
            Application.DoEvents()
            Dim hexByte As String = hexString.Substring(i, 2)
            Dim byteValue As Byte = Byte.Parse(hexByte, Globalization.NumberStyles.AllowHexSpecifier)
            bytesArray(i \ 2) = byteValue
        Next
        Return bytesArray
    End Function

#End Region

#Region " Set TaskBar Status Function "

    Public Enum TaskBarStatus
        Normal = 2     ' Blue
        Stopped = 4    ' Red
        Paused = 8     ' Yellow
        Disabled = 0   ' No colour
        Undefinied = 1 ' Marquee
    End Enum

    Private Function Set_TaskBar_Status(taskBarStatus As TaskBarStatus) As Boolean
        Try : Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(taskBarStatus)
            Return True
        Catch ex As Exception : Throw New Exception(ex.Message)
        End Try
    End Function

#End Region

#Region " Set TaskBar Value Function "

    Private Function Set_TaskBar_Value(currentValue As Integer, maxValue As Integer) As Boolean
        Try : Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressValue(currentValue, maxValue)
            Return True
        Catch ex As Exception : Throw New Exception(ex.Message)
        End Try
    End Function

#End Region

#Region " Other "
    ''' <summary>
    ''' Applies the immersive dark mode attribute to the window's title bar.
    ''' </summary>
    ''' <param name="hwnd">The handle of the window.</param>
    Friend Shared Sub ApplyDarkTitleBar(hwnd As IntPtr)

        If hwnd = IntPtr.Zero Then
            Throw New ArgumentNullException(NameOf(hwnd), "Window handle cannot be zero.")
        End If

        If Not Main.IsWindowsDarkModeEnabled() Then
            Exit Sub
        End If

        Dim useDarkMode As Integer = 1
        Dim attributeSize As Integer = System.Runtime.InteropServices.Marshal.SizeOf(GetType(Integer))

        ' DWMWA_USE_IMMERSIVE_DARK_MODE (Windows 10 version 2004 and later)
        Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20
        Dim result As Integer = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, useDarkMode, attributeSize)

        ' If the API call fails, fallback to DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 (Windows 10 versions 1809 to 1909)
        If result <> 0 Then
            Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, useDarkMode, attributeSize)
        End If

    End Sub

    ''' <summary>
    ''' Determines if the Windows dark theme is currently enabled for applications.
    ''' </summary>
    ''' <returns><see langword="True"/> if dark mode is enabled; otherwise, <see langword="False"/>.</returns>
    Private Shared Function IsWindowsDarkModeEnabled() As Boolean
        Try
            Using key As Microsoft.Win32.RegistryKey =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")
                If key Is Nothing Then
                    Return False
                End If

                Dim value As Object = key.GetValue("AppsUseLightTheme")
                Return value IsNot Nothing AndAlso CInt(value) = 0
            End Using
        Catch ex As Exception
            ' Ignore.
            ' Throw New InvalidOperationException("Failed to read Windows theme registry key to determine dark mode status.", ex)
            Return False
        End Try

    End Function

#End Region

#End Region

#Region " Threads "

#Region " Split Thread "

    Sub Split_Thread()

        Try
            Me.Split_Thread_Core()
        Catch ex As Exception
            Me.splitError = ex
        Finally
            ' Always restore the access to the source directory, even if the split failed.
            Try
                Me.Unlock_Locked_Directory()
            Catch ex As Exception
                If Me.splitError Is Nothing Then Me.splitError = ex
            End Try
            Me.ThreadProcessIsCompleted = False
            Me.ThreadIsCompleted = True
        End Try

    End Sub

    Private Sub Split_Thread_Core()

        Me.FilesList.Clear()
        Me.TotalFilesNumber = 0
        Me.Set_TaskBar_Value(0, 100)
        Me.Set_TaskBar_Status(TaskBarStatus.Normal)

        ' The ISO overhead is applied to a local copy;
        ' before, it was subtracted from SelectedDiscBytes on every ISO split, shrinking the disc size each time.
        Dim discCapacityBytes As Long = Me.SelectedDiscBytes
        If Me.CopyMode = SplitMode.Iso Then
            discCapacityBytes -= DiscCapacity.IsoOverheadBytes
        End If

        ' // Extract resources //
        '
        ' Extract - 7zip
        If Not Me.CopyMode = SplitMode.Copy Then
            If Me.architecture = 32 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_7zip.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_7zip_x86, Me.TempDir & "Splitty_7zip.exe")
                If Not IO.File.Exists(Me.TempDir & "7z.dll") Then Me.Load_Resource_To_Disk(My.Resources._7z_x86, Me.TempDir & "7z.dll")
            ElseIf Me.architecture = 64 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_7zip.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_7zip_x64, Me.TempDir & "Splitty_7zip.exe")
                If Not IO.File.Exists(Me.TempDir & "7z.dll") Then Me.Load_Resource_To_Disk(My.Resources._7z_x64, Me.TempDir & "7z.dll")
            End If
        End If

        ' Extract - PowerISO
        If Me.CopyMode = SplitMode.Iso Then
            If Not IO.File.Exists(Me.TempDir & "Splitty_Piso.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_PowerISO_x86, Me.TempDir & "Splitty_PowerISO.7z")
            Dim powerIsoKey = Me.HexToByteArray("000a42494c4c2047415445535ad50adc4f5ca6f9efc1252aadf9847f") ' PowerISO Premium Key
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\PowerISO", "USER", powerIsoKey, Microsoft.Win32.RegistryValueKind.Binary)

            ' Extract - WinRAR
        ElseIf Me.CopyMode = SplitMode.Rar Or Me.CopyMode = SplitMode.Sfx Then
            If Me.architecture = 32 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_WinRAR.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_WinRAR_x86, Me.TempDir & "Splitty_WinRAR.zip")
            ElseIf Me.architecture = 64 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_WinRAR.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_WinRAR_x64, Me.TempDir & "Splitty_WinRAR.zip")
            End If
            Me.Run_Process(Me.TempDir & "Splitty_7zip.exe", ArchiveCommandBuilder.BuildExtractArguments(Me.TempDir & "Splitty_WinRAR.zip", Me.TempDir), False, True)
        End If

        If Me.CopyMode = SplitMode.Copy Or Me.CopyMode = SplitMode.Iso Then
            If Not Me.WantToCancelThread = True Then

                Me.Lock_Selected_Directory()

                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 0)
                Me.FilesList = DirectoryScanner.GetFiles(Me.SelectedDirectory)
                Me.TotalFilesNumber = Me.FilesList.Count
                Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(Me.FilesList, discCapacityBytes)

                ' ProgressBar
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Max = Me.TotalFilesNumber)
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetString(LanguageResource & "06"))
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextShow = ProgBar.ProgBarPlus.eTextShow.FormatString)

                If Me.CopyMode = SplitMode.Iso Then
                    Me.Run_Process(Me.TempDir & "Splitty_7zip.exe", ArchiveCommandBuilder.BuildExtractArguments(Me.TempDir & "Splitty_PowerISO.7z", Me.TempDir), False, True)
                    Try
                        IO.File.Delete(Me.TempDir & "Splitty_PowerISO.7z")
                        IO.File.Delete(Me.TempDir & "Splitty_7zip.exe")
                    Catch ex As Exception
                    End Try
                End If

                ' Copy / Iso
                Dim currentDisc As Integer = 1
                For Each item As PlannedFile In plan
                    Application.DoEvents()
                    If Me.WantToCancelThread Then
                        Exit For
                    End If

                    If item.DiscNumber <> currentDisc Then
                        If Me.CopyMode = SplitMode.Iso Then
                            Me.Create_Disc_Iso(currentDisc)
                        End If
                        currentDisc = item.DiscNumber
                    End If

                    Me.Copy_Planned_File(item)
                    Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                    Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                    Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value += 1)
                Next

                ' Before, the ISO of a missing "Disc 1" folder was retried forever when the source directory was empty.
                If Me.CopyMode = SplitMode.Iso AndAlso plan.Count > 0 AndAlso Not Me.WantToCancelThread Then
                    Me.Create_Disc_Iso(currentDisc)
                End If
            End If
            Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = "")

        ElseIf Not Me.WantToCancelThread Then

            ' Zip / Rar / Sfx / Tar
            Dim volumeCountMax As Integer = CInt(Math.Max(1L, DiscCapacity.CalculateDiscCount(Me.SelectedDirSizeBytes, Me.SelectedDiscBytes)))
            Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Max = volumeCountMax)
            If volumeCountMax = 1 Then Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetString(LanguageResource & "08")) Else Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetString(LanguageResource & "27"))
            Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextShow = ProgBar.ProgBarPlus.eTextShow.FormatString)

            Dim volumePattern As String = ArchiveCommandBuilder.GetVolumeSearchPattern(Me.CopyMode)
            Me.ThreadProcess = New Threading.Thread(AddressOf Me.Process_Thread) With {
                .IsBackground = True
            }
            Me.ThreadProcess.Start(Me.CopyMode)
            While Not Me.ThreadProcessIsCompleted = True
                If Me.WantToCancelThread Then
                    Exit While
                End If
                Dim volumeCount As Integer = IO.Directory.GetFiles(Me.SelectedOutputDirectory, volumePattern).Length
                Dim progressValue As Integer = Math.Min(volumeCountMax, Math.Max(1, volumeCount))
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = progressValue)
                Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                Threading.Thread.Sleep(250) ' Avoids a busy loop that continuously enumerates the output directory.
            End While

        End If

        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = " ")
        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 0)

        ' Clean up
        Try
            IO.File.Delete(Me.TempDir & "7z_x64.dll")
            IO.File.Delete(Me.TempDir & "7z_x86.dll")
            IO.File.Delete(Me.TempDir & "Default.SFX")
            IO.File.Delete(Me.TempDir & "Default32.SFX")
            IO.File.Delete(Me.TempDir & "PowerISO.exe")
            IO.File.Delete(Me.TempDir & "rarreg.key")
            IO.File.Delete(Me.TempDir & "Splitty_7zip.exe")
            IO.File.Delete(Me.TempDir & "Splitty_7zip_x64.exe")
            IO.File.Delete(Me.TempDir & "Splitty_7zip_x86.exe")
            IO.File.Delete(Me.TempDir & "Splitty_Piso.exe")
            IO.File.Delete(Me.TempDir & "Splitty_WinRAR.exe")
            IO.File.Delete(Me.TempDir & "Splitty_WinRAR.zip")
            IO.File.Delete(Me.TempDir & "Splitty_WinRAR_x64.zip")
            IO.File.Delete(Me.TempDir & "Splitty_WinRAR_x86.zip")
        Catch : End Try

        If Not Me.WantToCancelThread AndAlso Me.splitError Is Nothing Then
            Me.Set_TaskBar_Value(100, 100)
            Me.InvokeControl(Me, Sub(x) x.Text = "100% Splitty")
        End If

    End Sub

    ''' <summary>
    ''' Copies a source file to the folder of its disc, replicating its directory structure.
    ''' </summary>
    ''' <param name="item">The planned file.</param>
    ''' <remarks>
    ''' Copy errors are not ignored anymore: they abort the split and are reported to the user.
    ''' </remarks>
    Private Sub Copy_Planned_File(item As PlannedFile)

        Dim destinationDirectory As String = SplitPaths.GetDestinationDirectory(Me.SelectedOutputDirectory, item.DiscNumber, item.File.DirectoryPath)
        IO.Directory.CreateDirectory(destinationDirectory)
        IO.File.Copy(item.File.FullPath, $"{destinationDirectory}\{item.File.FileName}", overwrite:=True)

    End Sub

    ''' <summary>
    ''' Creates the ISO image of a disc folder with PowerISO, then deletes the folder.
    ''' </summary>
    ''' <param name="discNumber">The one-based disc number.</param>
    ''' <exception cref="InvalidOperationException">
    ''' Thrown when PowerISO fails <see cref="MaxIsoAttempts"/> times
    ''' (before, it was retried forever, freezing the split).
    ''' </exception>
    Private Sub Create_Disc_Iso(discNumber As Integer)

        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = $"{My.Resources.ResourceManager.GetString(LanguageResource & "07")}{discNumber} ...")

        Dim isoFile As String = SplitPaths.GetDiscIsoFile(Me.SelectedOutputDirectory, discNumber)
        If IO.File.Exists(isoFile) Then
            IO.File.Delete(isoFile)
        End If

        Dim attempt As Integer = 0
        Do
            If Me.WantToCancelThread Then
                Exit Sub
            End If
            attempt += 1
            Dim result As Object = Me.Run_Process(Me.TempDir & "Splitty_Piso.exe", ArchiveCommandBuilder.BuildIsoArguments(Me.SelectedOutputDirectory, discNumber), False, True)
            If TypeOf result Is Boolean AndAlso CBool(result) Then
                Exit Do
            End If
            If attempt >= MaxIsoAttempts Then
                Throw New InvalidOperationException($"PowerISO failed to create the ISO image after {MaxIsoAttempts} attempts: {isoFile}")
            End If
        Loop

        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetString(LanguageResource & "06"))
        IO.Directory.Delete(SplitPaths.GetDiscDirectory(Me.SelectedOutputDirectory, discNumber), recursive:=True)

    End Sub

#End Region

#Region " Process thread "

    Private Sub Process_Thread(process As Object)
        Try
            Dim mode As SplitMode = CType(process, SplitMode)
            Dim executable As String = If(mode = SplitMode.Zip OrElse mode = SplitMode.Tar, "Splitty_7zip.exe", "Splitty_WinRar.exe")
            Dim arguments As String = ArchiveCommandBuilder.BuildArchiveArguments(mode, Me.SelectedDirectory, Me.SelectedOutputDirectory, Me.SelectedDiscBytes)
            Dim result As Object = Me.Run_Process(Me.TempDir & executable, arguments, False, True)

            ' Before, a failure of the archiver (non-zero exit code, or process not found) was ignored
            ' and the split was reported as successful.
            If Not (TypeOf result Is Boolean AndAlso CBool(result)) AndAlso Not Me.WantToCancelThread Then
                Me.splitError = New InvalidOperationException($"The archiver process failed: {executable}{arguments}")
            End If
        Catch ex As Exception
            Me.splitError = ex
        Finally
            Me.ThreadProcessIsCompleted = True
        End Try
    End Sub

#End Region

#End Region


End Class
