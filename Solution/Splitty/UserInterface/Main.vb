Public Class Main

#Region " Declarations "

    Public SelectedDirSizeBytes As Long
    Public SelectedDirSizeConvertedBytes As String
    Public SelectedDiscFormat As String
    Public SelectedDiscFormatLabel As String
    Public SelectedDiscBytes As Long

    Public CopyMode As String = "Copy"

    Public FilesList As New List(Of String)
    Public TotalFilesNumber As Integer
    Public CachedSize As Long

    Public ThreadProcess As System.Threading.Thread
    Public ThreadProcessIsCompleted As Boolean = False

    Public ThreadSplit As System.Threading.Thread
    Public ThreadIsCompleted As Boolean = False
    Public WantToCancelThread As Boolean = False

    Public TempDir As String = System.IO.Path.GetTempPath()

    Public Shared LanguageResource = "ENG"

    Dim architecture = Me.Get_OS_Architecture()

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

        Try
            If Not Me.ThreadSplit.ThreadState = Threading.ThreadState.Unstarted And Not Me.ThreadSplit.ThreadState = Threading.ThreadState.Aborted And Not Me.ThreadSplit.ThreadState = Threading.ThreadState.Stopped Then
                Dim answer = MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "01"), My.Settings.Version, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                If answer = MsgBoxResult.Yes Then
                    Me.Set_Folder_Access(Me.SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
                    Me.Kill_Process("Splitty_7zip")
                    Me.Kill_Process("Splitty_WinRar")
                    Me.Kill_Process("Splitty_Piso")
                    Try : Me.ThreadSplit.Abort() : Catch : End Try
                    End
                Else
                    e.Cancel = True
                End If
            Else
                Me.Set_Folder_Access(Me.SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
                Try : Me.ThreadSplit.Abort() : Catch : End Try
            End If
        Catch
        End Try
    End Sub

#End Region

#Region " Buttons folders "

    ' Input folder
    Private Sub Button_Input_Folder_Click(sender As Object, e As MouseEventArgs) Handles Button_Input_Folder.ClickButtonArea, Button_Folder.ClickButtonArea
        Dim selectFolder As New Ookii.Dialogs.VistaFolderBrowserDialog With {
            .ShowNewFolderButton = True
        }
        If selectFolder.ShowDialog.ToString() = "OK" Then
            If selectFolder.SelectedPath = Me.SelectedOutputDirectory Then
                MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "04"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                Me.SelectedDirectory = Nothing
                Me.Button_Split.Enabled = False
                Me.GroupPanel_Options.Enabled = False
                Me.GroupBox_Mode.Enabled = False
                Me.GroupBox_Information.Enabled = False
                Me.Label_Size_Value.Text = "0"
                Me.Label_Discs_Value.Text = "0"
                Exit Sub
            End If
            Me.SelectedDirectory = (selectFolder.SelectedPath)
            Me.Create_FileSystemWatcher(Me.SelectedDirectory)
            Me.TextBox_Input_Folder.Text = Me.SelectedDirectory
            Me.SelectedDirSizeBytes = Me.Get_Directory_Size(New IO.DirectoryInfo(Me.SelectedDirectory), True) ' Set Bytes of directory size
            Me.SelectedDirSizeConvertedBytes = Me.Round_Bytes(Me.SelectedDirSizeBytes) ' Set MB or GB or TB of directory size
            Me.Label_Size_Value.Text = Me.SelectedDirSizeConvertedBytes ' & "   (" & Selected_Dir_Size_Bytes & " Bytes)"
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
            Me.TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Output folder
    Private Sub Button_Output_Folder_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Output_Folder.ClickButtonArea, CButton1.ClickButtonArea
        Dim selectFolder As New Ookii.Dialogs.VistaFolderBrowserDialog With {
            .ShowNewFolderButton = True
        }
        If selectFolder.ShowDialog.ToString() = "OK" Then
            If selectFolder.SelectedPath = Me.SelectedDirectory Then
                MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "03"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                Me.SelectedOutputDirectory = Nothing
                Exit Sub
            End If
            Me.SelectedOutputDirectory = (selectFolder.SelectedPath)
            Me.TextBox_Output_Folder.Text = Me.SelectedOutputDirectory

            If Me.RadioButton_Custom_Size.Checked = True Then
                If Not Me.TextBox_Custom_Size.Text = 0 Then
                    Me.Button_Split.Enabled = True
                    Me.Button_Split.ForeColor = Color.Black
                    Me.Button_Split.Enabled = True
                End If
            Else
                Me.Button_Split.Image = My.Resources.split
                Me.Button_Split.ForeColor = Color.Black
                Me.Button_Split.Enabled = True
            End If

        End If
        Me.Flush_Memory(Application.ExecutablePath)
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
        Me.Label_Input.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "11")
        Me.Label_Output.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "12")
        Me.Label_Capacity.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "13")
        Me.Label_Mode.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "14")
        Me.Label_Information.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "15")
        Me.Label_Size_Name.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "16")
        Me.Label_Discs_Name.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "17")
        Me.Button_Split.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "18")
        Me.Button_Stop.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "19")
        Me.RadioButton_Copy.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "20")
        Me.RadioButton_Custom_Size.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "21")
        Me.AboutToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "22")
        Me.LanguageToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "23")
        Me.EnglishToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "24")
        Me.SpanishToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "25")
        Me.TextBox_Input_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
        If Not Me.TextBox_Output_Folder.HintDetails.HintText = "" Then Me.TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
        If Not Me.Label_Discs_Value.Text = Nothing Then Me.Label_Discs_Value.Text = Math.Ceiling((Me.SelectedDirSizeBytes / Me.SelectedDiscBytes)) & My.Resources.ResourceManager.GetObject(LanguageResource & "26")
    End Sub

#End Region

#Region " Drag&Drop Input Folder "

    ' FolderTextbox Drag-Drop
    Private Sub Input_Folder_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox_Input_Folder.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim folderObject As String() = e.Data.GetData(DataFormats.FileDrop)
            If System.IO.Directory.Exists(folderObject(0)) Then
                Me.TextBox_Input_Folder.Text = folderObject(0)
                Me.SelectedDirectory = folderObject(0)
                If Me.SelectedDirectory = Me.SelectedOutputDirectory Then
                    MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "04"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                    Me.SelectedDirectory = Nothing
                    Me.Button_Split.Enabled = False
                    Me.GroupPanel_Options.Enabled = False
                    Me.GroupBox_Mode.Enabled = False
                    Me.GroupBox_Information.Enabled = False
                    Me.Label_Size_Value.Text = "0"
                    Me.Label_Discs_Value.Text = "0"
                    Exit Sub
                End If
                Me.Create_FileSystemWatcher(Me.SelectedDirectory)
                Me.SelectedDirSizeBytes = Me.Get_Directory_Size(New IO.DirectoryInfo(Me.SelectedDirectory), True) ' Set Bytes of directory size
                Me.SelectedDirSizeConvertedBytes = Me.Round_Bytes(Me.SelectedDirSizeBytes) ' Set MB or GB or TB of directory size
                Me.Label_Size_Value.Text = Me.SelectedDirSizeConvertedBytes '& "   (" & Selected_Dir_Size_Bytes & " Bytes)"
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
                Me.TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
            Else
                MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "05"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        End If
    End Sub

    Private Sub Output_Folder_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox_Output_Folder.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim folderObject As String() = e.Data.GetData(DataFormats.FileDrop)
            If System.IO.Directory.Exists(folderObject(0)) Then
                Me.SelectedOutputDirectory = (folderObject(0))
                Me.TextBox_Output_Folder.Text = Me.SelectedOutputDirectory
                If Me.SelectedOutputDirectory = Me.SelectedDirectory Then
                    MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "03"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                    Me.SelectedOutputDirectory = Nothing
                    Exit Sub
                End If
                If Me.RadioButton_Custom_Size.Checked = True Then
                    If Not Me.TextBox_Custom_Size.Text = 0 Then
                        Me.Button_Split.Enabled = True
                        Me.Button_Split.ForeColor = Color.Black
                        Me.Button_Split.Enabled = True
                    End If
                Else
                    Me.Button_Split.Image = My.Resources.split
                    Me.Button_Split.ForeColor = Color.Black
                    Me.Button_Split.Enabled = True
                End If

            End If
            Me.Flush_Memory(Application.ExecutablePath)
        Else
            MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "05"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End If
    End Sub


    ' FolderTextbox Drag-Enter
    Private Sub Folder_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles TextBox_Input_Folder.DragEnter, TextBox_Output_Folder.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then e.Effect = DragDropEffects.All
    End Sub

#End Region

#Region " FileSystem Watcher "

    Private Sub Create_FileSystemWatcher(path As String)
        Me.FileSystemWatcher1.Path = path
        'AddHandler FileSystemWatcher1.Changed, AddressOf Watcher_Changed
        AddHandler Me.FileSystemWatcher1.Created, AddressOf Me.Watcher_Changed
        AddHandler Me.FileSystemWatcher1.Deleted, AddressOf Me.Watcher_Changed
        AddHandler Me.FileSystemWatcher1.Renamed, AddressOf Me.Watcher_Changed
    End Sub

    Private Sub Watcher_Changed(sender As Object, e As System.IO.FileSystemEventArgs)
        If Me.Button_Split.Visible Then
            Dim answer = MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "32"), My.Settings.Version, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If answer = MsgBoxResult.Yes Then
                Me.SelectedDirSizeBytes = Me.Get_Directory_Size(New IO.DirectoryInfo(Me.SelectedDirectory), True) ' Set Bytes of directory size
                Me.SelectedDirSizeConvertedBytes = Me.Round_Bytes(Me.SelectedDirSizeBytes) ' Set MB or GB or TB of directory size
                Me.Label_Size_Value.Text = Me.SelectedDirSizeConvertedBytes ' & "   (" & Selected_Dir_Size_Bytes & " Bytes)"
                Me.Label_Discs_Value.Text = Math.Ceiling((Me.SelectedDirSizeBytes / Me.SelectedDiscBytes)) & " " & Me.Label_Discs_Value.Text.Split(" ")(1)
            End If
        End If
    End Sub

#End Region

#Region " Radio buttons Capacity "

    Private Sub RadioButton_CD_Click(sender As Object, e As EventArgs) Handles RadioButton_CD.CheckedChanged
        If Me.RadioButton_CD.Checked = True Then
            Me.SelectedDiscFormat = "CD"
            Me.SelectedDiscBytes = 734003200
            Me.PictureBox_Disc.BackgroundImage = My.Resources.cd
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.CD))) & " CD's"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_CD800_Click(sender As Object, e As EventArgs) Handles RadioButton_CD800.CheckedChanged
        If Me.RadioButton_CD800.Checked = True Then
            Me.SelectedDiscFormat = "CD800"
            Me.SelectedDiscBytes = 829440393.216
            Me.PictureBox_Disc.BackgroundImage = My.Resources.cd
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.CD800MB))) & Me.SelectedDiscFormatLabel & " CD's"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_CD900_Click(sender As Object, e As EventArgs) Handles RadioButton_CD900.CheckedChanged
        If Me.RadioButton_CD900.Checked = True Then
            Me.SelectedDiscFormat = "CD900"
            Me.SelectedDiscBytes = 912383803.392
            Me.PictureBox_Disc.BackgroundImage = My.Resources.cd
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.CD900MB))) & Me.SelectedDiscFormatLabel & " CD's"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd5Click(sender As Object, e As EventArgs) Handles RadioButton_DVD5.CheckedChanged
        If Me.RadioButton_DVD5.Checked = True Then
            Me.SelectedDiscFormat = "DVD5"
            Me.SelectedDiscBytes = 4700000000
            Me.PictureBox_Disc.BackgroundImage = My.Resources.dvd
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.Dvd5))) & Me.SelectedDiscFormatLabel & " DVD's"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd9Click(sender As Object, e As EventArgs) Handles RadioButton_DVD9.CheckedChanged
        If Me.RadioButton_DVD9.Checked = True Then
            Me.SelectedDiscFormat = "DVD9"
            Me.SelectedDiscBytes = 8500000000
            Me.PictureBox_Disc.BackgroundImage = My.Resources.dvd
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.Dvd9))) & Me.SelectedDiscFormatLabel & " DVD's"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd10Click(sender As Object, e As EventArgs) Handles RadioButton_DVD10.CheckedChanged
        If Me.RadioButton_DVD10.Checked = True Then
            Me.SelectedDiscFormat = "DVD10"
            Me.SelectedDiscBytes = 9395240960
            Me.PictureBox_Disc.BackgroundImage = My.Resources.dvd
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.Dvd10))) & Me.SelectedDiscFormatLabel & " DVD's"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_Click(sender As Object, e As EventArgs) Handles RadioButton_BluRay.CheckedChanged
        If Me.RadioButton_BluRay.Checked = True Then
            Me.SelectedDiscFormat = "BR"
            Me.SelectedDiscBytes = 25025314816
            Me.PictureBox_Disc.BackgroundImage = My.Resources.bluray
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.BR))) & Me.SelectedDiscFormatLabel & " BluRays"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_DL_Click(sender As Object, e As EventArgs) Handles RadioButton_BluRay_DL.CheckedChanged
        If Me.RadioButton_BluRay_DL.Checked = True Then
            Me.SelectedDiscFormat = "BR-DL"
            Me.SelectedDiscBytes = 50050629632
            Me.PictureBox_Disc.BackgroundImage = My.Resources.bluray
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.BRDoubleLayer))) & Me.SelectedDiscFormatLabel & " BluRays"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_MiniDisc_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_BluRay_MiniDisc.CheckedChanged
        If Me.RadioButton_BluRay_MiniDisc.Checked = True Then
            Me.SelectedDiscFormat = "BR-MD"
            Me.SelectedDiscBytes = 7791181824
            Me.PictureBox_Disc.BackgroundImage = My.Resources.bluray
            Me.Label_Discs_Value.Text = Math.Ceiling((Me.Convert_To_Disc_Size(Me.SelectedDirSizeBytes, SizeType.Bytes, DiscType.BRMiniDisc))) & Me.SelectedDiscFormatLabel & " BluRays"
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

#End Region

#Region " Radio buttons Copy mode "

    Private Sub RadioButtonCopy(sender As Object, e As EventArgs) Handles RadioButton_Copy.CheckedChanged
        Me.CopyMode = "Copy"
    End Sub

    Private Sub RadioButtonRar(sender As Object, e As EventArgs) Handles RadioButton_Rar.CheckedChanged
        Me.CopyMode = "Rar"
    End Sub

    Private Sub RadioButtonZip(sender As Object, e As EventArgs) Handles RadioButton_Zip.CheckedChanged
        Me.CopyMode = "Zip"
    End Sub

    Private Sub RadioButtonSfx(sender As Object, e As EventArgs) Handles RadioButton_SFX.CheckedChanged
        Me.CopyMode = "Sfx"
    End Sub

    Private Sub RadioButtonIso(sender As Object, e As EventArgs) Handles RadioButton_ISO.CheckedChanged
        Me.CopyMode = "Iso"
    End Sub

    Private Sub RadioButtonTar(sender As Object, e As EventArgs) Handles RadioButton_TAR.CheckedChanged
        Me.CopyMode = "Tar"
    End Sub

#End Region

#Region " Custom size option "

    ' RadioButton custom size
    Private Sub RadioButton_Custom(sender As Object, e As EventArgs) Handles RadioButton_Custom_Size.CheckedChanged
        If Me.RadioButton_Custom_Size.Checked = True Then
            Me.SelectedDiscFormat = "CUSTOM"
            Me.PictureBox_Disc.BackgroundImage = My.Resources.custom
            Me.TextBox_Custom_Size.Enabled = True
            Me.ComboBox_Custom_Size.Enabled = True
            'TextBox_Custom_Size.HintDetails.HintText = "0123456789,"

            If Me.ComboBox_Custom_Size.SelectedItem = Nothing Then
                Me.Label_Discs_Value.Text = "0"
                Me.ComboBox_Custom_Size.SelectedIndex = 0
            End If
            If Me.TextBox_Custom_Size.Text = 0 Or Me.SelectedOutputDirectory = Nothing Then Me.Button_Split.Enabled = False Else Me.Button_Split.Enabled = True
        Else
            If Me.SelectedOutputDirectory = Nothing Then Me.Button_Split.Enabled = False Else Me.Button_Split.Enabled = True
            Me.TextBox_Custom_Size.HintDetails.HintText = ""
            Me.TextBox_Custom_Size.Enabled = False
            Me.ComboBox_Custom_Size.Enabled = False
        End If
        Me.Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Textbox custom size
    Private Sub TextBox_Custom_Size_TextChanged(sender As Object, e As EventArgs) Handles TextBox_Custom_Size.TextChanged, RadioButton_Custom_Size.CheckedChanged
        Me.TextBox_Custom_Size.Text = Me.TextBox_Custom_Size.Text.Replace(".", ",")

        If Me.TextBox_Custom_Size.Text.Length = 0 Then Me.TextBox_Custom_Size.Text = 0

        If Not Me.TextBox_Custom_Size.Text.Length = 0 Then
            If Me.ComboBox_Custom_Size.SelectedIndex = 0 Then
                Me.SelectedDiscBytes = (Me.TextBox_Custom_Size.Text * 1048576)
            End If

            If Me.ComboBox_Custom_Size.SelectedIndex = 1 Then
                Me.SelectedDiscBytes = (Me.TextBox_Custom_Size.Text * 1073741824)
            End If

            If Me.ComboBox_Custom_Size.SelectedIndex = 2 Then
                Me.SelectedDiscBytes = (Me.TextBox_Custom_Size.Text * 1099511627776)
            End If

            Me.Label_Discs_Value.Text = Math.Ceiling((Me.SelectedDirSizeBytes / Me.SelectedDiscBytes)) & My.Resources.ResourceManager.GetObject(LanguageResource & "26")
        End If

        If Me.SelectedOutputDirectory = Nothing Or Me.TextBox_Custom_Size.Text = 0 Then Me.Button_Split.Enabled = False Else Me.Button_Split.Enabled = True
    End Sub

    ' Textbox custom size (Keypress)
    Private Sub TextBox_Custom_Size_KeyPress(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox_Custom_Size.KeyPress
        e.Handled = Me.NumericOnly(e.KeyChar)
    End Sub

    ' ComboBox custom size
    Private Sub ComboBox_Custom_Size_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_Custom_Size.SelectedIndexChanged
        If Me.ComboBox_Custom_Size.SelectedIndex = 0 Then
            Me.SelectedDiscBytes = (Me.TextBox_Custom_Size.Text * 1048576)
        End If
        If Me.ComboBox_Custom_Size.SelectedIndex = 1 Then
            Me.SelectedDiscBytes = (Me.TextBox_Custom_Size.Text * 1073741824)
        End If
        If Me.ComboBox_Custom_Size.SelectedIndex = 2 Then
            Me.SelectedDiscBytes = (Me.TextBox_Custom_Size.Text * 1099511627776)
        End If
        Me.Label_Discs_Value.Text = Math.Ceiling((Me.SelectedDirSizeBytes / Me.SelectedDiscBytes)) & " of custom storage"
    End Sub

#End Region

#Region " Button Split "

    ' Button SPLIT
    Private Sub Button_Split_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Split.ClickButtonArea

        If Me.RadioButton_Custom_Size.Checked = True And Me.SelectedDiscBytes > Me.SelectedDirSizeBytes Then
            MessageBox.Show(
                My.Resources.ResourceManager.GetObject(LanguageResource & "29") & vbNewLine & vbNewLine &
                My.Resources.ResourceManager.GetObject(LanguageResource & "30") & Me.SelectedDirSizeConvertedBytes & vbNewLine &
                My.Resources.ResourceManager.GetObject(LanguageResource & "31") & Me.Round_Bytes(Me.SelectedDiscBytes) _
                , My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Exit Sub
        End If

        Dim tempImage = Me.PictureBox_Disc.BackgroundImage
        Me.PictureBox_Disc.BackgroundImage = Me.GrayScale_Image(Me.PictureBox_Disc.BackgroundImage, GrayScale.MidGray)

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
        End While

        Me.Set_Folder_Access(Me.SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
        If Not Me.WantToCancelThread Then MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "09"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
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
        Me.Set_Folder_Access(Me.SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
        Me.Set_TaskBar_Value(100, 100)
        Me.InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Me.Set_TaskBar_Status(TaskBarStatus.Stopped)
        Me.Kill_Process("Splitty_7zip")
        Me.Kill_Process("Splitty_WinRar")
        Me.Kill_Process("Splitty_Piso")
        MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "28"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
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

#Region " Functions "

#Region " Get Directory Size Function "

    Private Function Get_Directory_Size(directory As IO.DirectoryInfo, includeSubfolders As Boolean) As Long
        Try
            Dim dirTotalSize As Long = directory.EnumerateFiles().Sum(Function(file) file.Length)
            If includeSubfolders Then dirTotalSize += directory.EnumerateDirectories().Sum(Function(dir) Me.Get_Directory_Size(dir, True))
            Return dirTotalSize
        Catch
        End Try
        Return Nothing
    End Function

#End Region

#Region " Get All Files Function "

    Public Sub Get_All_Files(aDir As IO.DirectoryInfo)
        Dim nextDir As IO.DirectoryInfo
        Me.WorkWithFilesInDir(aDir)
        For Each nextDir In aDir.GetDirectories
            Application.DoEvents()
            Me.Get_All_Files(nextDir)
        Next
    End Sub

    Public Sub WorkWithFilesInDir(aDir As IO.DirectoryInfo)
        Dim aFile As IO.FileInfo
        For Each aFile In aDir.GetFiles()
            Application.DoEvents()
            Try
                Me.FilesList.Add(aFile.DirectoryName & "|" & aFile.Name & "|" & aFile.Length)
                Me.TotalFilesNumber += 1
            Catch pathTooLongException As Exception
                Dim answer = MessageBox.Show(pathTooLongException.Message, My.Settings.Version, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                If answer = Windows.Forms.DialogResult.Abort Then
                    Me.Button_Stop.PerformClick()
                    Exit For
                    Exit Sub
                End If
                If answer = Windows.Forms.DialogResult.Retry Then Me.Get_All_Files(New IO.DirectoryInfo(Me.SelectedDirectory))
            End Try
        Next
    End Sub

#End Region

#Region " Convert Bytes Function "

    Public Function Round_Bytes(byteSize As Long) As String

        Dim sizeOfKB As Long = 1024 ' 1KB
        Dim sizeOfMB As Long = 1048576 ' 1MB
        Dim sizeOfGB As Long = 1073741824 ' 1GB
        Dim sizeOfTB As Long = 1099511627776 ' 1TB
        Dim sizeofPB As Long = 1125899906842624 ' 1PB

        Dim tempFileSize As Double

        If byteSize < sizeOfKB Then 'Filesize is in Bytes
            tempFileSize = Me.Convert_Bytes(byteSize, ConvTo.B)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize) & " bytes" 'Return our converted value

        ElseIf byteSize >= sizeOfKB And byteSize < sizeOfMB Then 'Filesize is in Kilobytes
            tempFileSize = Me.Convert_Bytes(byteSize, ConvTo.KB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize) & " KB"

        ElseIf byteSize >= sizeOfMB And byteSize < sizeOfGB Then ' Filesize is in Megabytes
            tempFileSize = Me.Convert_Bytes(byteSize, ConvTo.MB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize, 1) & " MB"

        ElseIf byteSize >= sizeOfGB And byteSize < sizeOfTB Then 'Filesize is in Gigabytes
            tempFileSize = Me.Convert_Bytes(byteSize, ConvTo.GB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize, 1) & " GB"

        ElseIf byteSize >= sizeOfTB And byteSize < sizeofPB Then 'Filesize is in Terabytes
            tempFileSize = Me.Convert_Bytes(byteSize, ConvTo.TB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize, 1) & " TB"
        Else
            Return Nothing 'Invalid filesize so return Nothing
        End If
    End Function

    Public Function Convert_Bytes(bytes As Long, convertTo As ConvTo) As Double
        If ConvTo.IsDefined(GetType(ConvTo), convertTo) Then
            Return bytes / (1024 ^ convertTo)
        Else
            Return -1 'An invalid value was passed to this function so exit
        End If
    End Function

    Public Enum ConvTo
        'Enumerations for file size conversions
        B = 0
        KB = 1
        MB = 2
        GB = 3
        TB = 4
        PB = 5
        EB = 6
        ZI = 7
        YI = 8
    End Enum

#End Region

#Region " Convert To Disc Size Function "

    Private Function Convert_To_Disc_Size(fileSize As Double, sizeType As SizeType, toDiscType As DiscType) As Double
        Dim size As Double = Me.GetSize(toDiscType)
        If (size < 0) Then Throw New ArgumentException("Tamaño de disco no localizado")
        Return fileSize * DirectCast(sizeType, Long) / size
    End Function

    Enum SizeType As Long
        Bytes = 1
        Kilobytes = 1024
        Megabytes = 1048576
        Gigabytes = 1073741824
        Terabytes = 1099511627776
    End Enum

    Enum DiscType
        CD
        CD800MB
        CD900MB
        Dvd5
        Dvd9
        Dvd10
        BR
        BRDoubleLayer
        BRMiniDisc
        BRMiniDiscDoubleLayer
    End Enum

    Private Function GetSize(discType As DiscType) As Double
        Select Case discType
            Case DiscType.CD : Return 734003200 ' CD Standard
            Case DiscType.CD800MB : Return 829440393.216 ' CD 800 MB
            Case DiscType.CD900MB : Return 912383803.392 ' CD 900 MB
            Case DiscType.Dvd5 : Return 4700000000 ' DVD Standard (DVD5)
            Case DiscType.Dvd9 : Return 8500000000 ' DVD Double Layer (DVD9)
            Case DiscType.Dvd10 : Return 9395240960 ' DVD Double Layer (DVD10)
            Case DiscType.BR : Return 25025314816 ' BluRay Standard
            Case DiscType.BRDoubleLayer : Return 50050629632 ' BluRay Double Layer
            Case DiscType.BRMiniDisc : Return 7791181824 ' BluRay MiniDisc Standard
            Case DiscType.BRMiniDiscDoubleLayer : Return 15582363648 ' BluRay MiniDisc Double Layer
            Case Else
                Return -1 ' Por si se declara un nuevo valor en el enumerador sin especificar tamaño
        End Select
    End Function

#End Region

#Region " Copy File Function "

    Private Function Copy_File(file As String, targetPath As String,
                               Optional forceTargetPath As Boolean = False, Optional forceFileReplace As Boolean = False,
                               Optional attributes As System.IO.FileAttributes = IO.FileAttributes.Normal)

        Dim fileInformation = My.Computer.FileSystem.GetFileInfo(file) ' Get Input File Information

        ' Directory
        If Not forceTargetPath And Not My.Computer.FileSystem.DirectoryExists(targetPath) Then
            Return False ' Target Directory don't exists
        ElseIf forceTargetPath Then
            Try
                My.Computer.FileSystem.CreateDirectory(targetPath) ' Create directory
            Catch ex As Exception
                'Return False
                Return ex.Message ' Directory can't be created maybe beacuse user permissions
            End Try
        End If

        ' File
        Try
            My.Computer.FileSystem.CopyFile(file, targetPath & "\" & fileInformation.Name, forceFileReplace) ' Copies the file
            If Not attributes = IO.FileAttributes.Normal Then My.Computer.FileSystem.GetFileInfo(targetPath & "\" & fileInformation.Name).Attributes = attributes ' Apply File Attributes
            Return True ' File is copied OK
        Catch ex As Exception
            'Return False
            Return ex.Message ' File can't be created maybe beacuse user permissions
        End Try
    End Function

#End Region

#Region " Get File Info Function "

    Const FullName As String = "FullName"
    Const DirectoryName As String = "DirectoryName"
    Const FileName As String = "Name"
    Const Extension As String = "Extension"
    Const Length As String = "Length"

    Private Function Get_File_Info(file As String, information As String)
        Dim fileInformation = My.Computer.FileSystem.GetFileInfo(file)
        If information = FullName Then Return fileInformation.FullName _
            Else If information = DirectoryName Then Return fileInformation.DirectoryName _
            Else If information = FileName Then Return fileInformation.Name _
            Else If information = Extension Then Return fileInformation.Extension _
            Else If information = Length Then Return fileInformation.Length
        Return False
    End Function

#End Region

#Region "Only numbers in textbox function"

    Public Function NumericOnly(eChar As Char) As Boolean
        Dim chkStr As String = "0123456789,"
        If chkStr.IndexOf(eChar) > -1 OrElse eChar = vbBack Then
            If eChar = Chr(44) And Me.TextBox_Custom_Size.Text.Contains(",") Then Return True
            Return False
        Else
            Return True
        End If
    End Function

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

#Region " Delimit String Function "

    Private Function Delimit_String(str As String, delimiterA As String, Optional delimiterB As String = "", Optional ignoreCase As Boolean = False, Optional leftOrRight As String = "Right") As String
        Dim compareMethod As Integer = 0 ' Don't ignore case
        If ignoreCase = True Then compareMethod = 1 ' Ignore Case

        If Not leftOrRight.ToUpper = "LEFT" And Not leftOrRight.ToUpper = "RIGHT" _
            Then Return False ' Returns false if the Left_Or_Right argument is in incorrect format

        If compareMethod = 0 Then
            If Not str.Contains(delimiterA) Or Not str.Contains(delimiterB) _
                Then Return False ' Returns false if one of the delimiters in NormalCase can 't be found
        Else
            If Not str.ToUpper.Contains(delimiterA.ToUpper) Or Not str.ToUpper.Contains(delimiterB.ToUpper) _
            Then Return False ' Returns false if one of the delimiters in IgnoreCase can 't be found
        End If

        Try
            If leftOrRight.ToUpper = "LEFT" Then str = Split(str, delimiterA, , compareMethod)(0) _
                Else If leftOrRight.ToUpper = "RIGHT" Then str = Split(str, delimiterA, , compareMethod)(1)

            If delimiterB IsNot Nothing Then
                If leftOrRight.ToUpper = "LEFT" Then str = Split(str, delimiterB, , compareMethod)(1) _
                 Else If leftOrRight.ToUpper = "RIGHT" Then str = Split(str, delimiterB, , compareMethod)(0)
            End If

            Return str ' Returns the splitted string
        Catch ex As Exception
            Return ex.Message ' Returns exception if index is out of range
        End Try
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

#Region " Folder Access Function "

    Public Enum FolderAccess
        Create = System.Security.AccessControl.FileSystemRights.CreateDirectories + System.Security.AccessControl.FileSystemRights.CreateFiles
        Delete = System.Security.AccessControl.FileSystemRights.Delete + System.Security.AccessControl.FileSystemRights.DeleteSubdirectoriesAndFiles
        Write = System.Security.AccessControl.FileSystemRights.AppendData + System.Security.AccessControl.FileSystemRights.Write + Security.AccessControl.FileSystemRights.WriteAttributes + System.Security.AccessControl.FileSystemRights.WriteData + System.Security.AccessControl.FileSystemRights.WriteExtendedAttributes
    End Enum

    Public Enum Action
        Allow = 0
        Deny = 1
    End Enum

    Private Function Set_Folder_Access(path As String, folderAccess As FolderAccess, action As Action) As Boolean
        Try
            Dim folderInfo As New IO.DirectoryInfo(path)
            Dim folderAcl As New System.Security.AccessControl.DirectorySecurity
            folderAcl.AddAccessRule(
                New System.Security.AccessControl.FileSystemAccessRule(
                    My.User.Name,
                    folderAccess,
                    System.Security.AccessControl.InheritanceFlags.ContainerInherit Or System.Security.AccessControl.InheritanceFlags.ObjectInherit, System.Security.AccessControl.PropagationFlags.None,
                    action))
            folderInfo.SetAccessControl(folderAcl)
            Return True
        Catch ex As Exception
            Throw New Exception(ex.Message)
            ' Return False
        End Try
    End Function

#End Region

#End Region

#Region " Threads "

#Region " Split Thread "

    Sub Split_Thread()

        Me.FilesList.Clear()
        Me.CachedSize = 0
        Me.TotalFilesNumber = 0
        Me.Set_TaskBar_Value(0, 100)
        Me.Set_TaskBar_Status(TaskBarStatus.Normal)

        ' // Extract resources //
        '
        ' Extract - 7zip
        If Not Me.CopyMode = "Copy" Then
            If Me.architecture = 32 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_7zip.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_7zip_x86, Me.TempDir & "Splitty_7zip.exe")
                If Not IO.File.Exists(Me.TempDir & "7z.dll") Then Me.Load_Resource_To_Disk(My.Resources._7z_x86, Me.TempDir & "7z.dll")
            ElseIf Me.architecture = 64 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_7zip.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_7zip_x64, Me.TempDir & "Splitty_7zip.exe")
                If Not IO.File.Exists(Me.TempDir & "7z.dll") Then Me.Load_Resource_To_Disk(My.Resources._7z_x64, Me.TempDir & "7z.dll")
            End If
        End If

        ' Extract - PowerISO
        If Me.CopyMode = "Iso" Then
            If Not IO.File.Exists(Me.TempDir & "Splitty_Piso.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_PowerISO_x86, Me.TempDir & "Splitty_PowerISO.7z")
            Me.SelectedDiscBytes -= 93491 ' Extra bytes needed to create the ISO file
            Dim powerIsoKey = Me.HexToByteArray("000a42494c4c2047415445535ad50adc4f5ca6f9efc1252aadf9847f") ' PowerISO Premium Key
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\PowerISO", "USER", powerIsoKey, Microsoft.Win32.RegistryValueKind.Binary)

            ' Extract - WinRAR
        ElseIf Me.CopyMode = "Rar" Or Me.CopyMode = "Sfx" Then
            If Me.architecture = 32 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_WinRAR.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_WinRAR_x86, Me.TempDir & "Splitty_WinRAR.7z")
            ElseIf Me.architecture = 64 Then
                If Not IO.File.Exists(Me.TempDir & "Splitty_WinRAR.exe") Then Me.Load_Resource_To_Disk(My.Resources.Splitty_WinRAR_x64, Me.TempDir & "Splitty_WinRAR.7z")
            End If
            Me.Run_Process(Me.TempDir & "Splitty_7zip.exe", "e " & """" & Me.TempDir & "Splitty_WinRAR.7z" & """" & " -o" & """" & Me.TempDir & """" & " -y", False, True)
        End If

        If Me.CopyMode = "Copy" Or Me.CopyMode = "Iso" Then
            If Not Me.WantToCancelThread = True Then

                Me.Set_Folder_Access(Me.SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Deny)

                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 0)
                Me.Get_All_Files(New IO.DirectoryInfo(Me.SelectedDirectory))

                ' ProgressBar
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Max = Me.TotalFilesNumber)
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "06"))
                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextShow = ProgBar.ProgBarPlus.eTextShow.FormatString)

                If Me.CopyMode = "Iso" Then
                    Me.Run_Process(Me.TempDir & "Splitty_7zip.exe", "e " & """" & Me.TempDir & "Splitty_PowerISO.7z" & """" & " -o" & """" & Me.TempDir & """" & " -y", False, True)
                    Try
                        IO.File.Delete(Me.TempDir & "Splitty_PowerISO.7z")
                        IO.File.Delete(Me.TempDir & "Splitty_7zip.exe")
                    Catch ex As Exception
                    End Try
                End If

                ' Copy / Iso
                Dim folderNum As Integer = 1
                For Each file In Me.FilesList
                    Application.DoEvents()
                    If Not Me.WantToCancelThread = True Then
                        Me.CachedSize += file.Split("|")(2)
                        If Not Me.CachedSize > Me.SelectedDiscBytes Then
                            Me.Copy_File(file.Split("|")(0) & "\" & file.Split("|")(1), Me.SelectedOutputDirectory & "\Disc " & folderNum & Me.Get_File_Info(file.Split("|")(0) & "\" & file.Split("|")(1), DirectoryName).Split(":")(1), True, True)

                            Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                            Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")

                        Else
                            If Me.CopyMode = "Iso" Then
                                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "07") & folderNum & " ...")
                                If IO.File.Exists(Me.SelectedOutputDirectory & "\Disc " & folderNum & ".iso") Then Try : IO.File.Delete(Me.SelectedOutputDirectory & "\Disc " & folderNum & ".iso") : Catch : End Try
                                While Not Me.Run_Process(Me.TempDir & "Splitty_Piso.exe", " -disable-optimization create -o " & """" & Me.SelectedOutputDirectory & "\Disc " & folderNum & ".iso" & """" & " -add " & """" & Me.SelectedOutputDirectory & "\Disc " & folderNum & """" & " /", False, True) = True
                                    If Me.WantToCancelThread Then
                                        Exit While
                                    End If
                                End While
                                Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "06"))
                                Try : IO.Directory.Delete(Me.SelectedOutputDirectory & "\Disc " & folderNum, True) : Catch : End Try
                            End If
                            Me.CachedSize = Nothing
                            Me.CachedSize += file.Split("|")(2)
                            folderNum += 1
                            Me.Copy_File(file.Split("|")(0) & "\" & file.Split("|")(1), Me.SelectedOutputDirectory & "\Disc " & folderNum & Me.Get_File_Info(file.Split("|")(0) & "\" & file.Split("|")(1), DirectoryName).Split(":")(1), True, True)
                        End If
                        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value += 1)
                    Else
                        Exit For
                    End If
                Next
                If Me.CopyMode = "Iso" Then
                    Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "07") & folderNum & " ...")
                    If IO.File.Exists(Me.SelectedOutputDirectory & "\Disc " & folderNum & ".iso") Then Try : IO.File.Delete(Me.SelectedOutputDirectory & "\Disc " & folderNum & ".iso") : Catch : End Try
                    While Not Me.Run_Process(Me.TempDir & "Splitty_Piso.exe", " -disable-optimization create -o " & """" & Me.SelectedOutputDirectory & "\Disc " & folderNum & ".iso" & """" & " -add " & """" & Me.SelectedOutputDirectory & "\Disc " & folderNum & """" & " /", False, True) = True
                        If Me.WantToCancelThread Then
                            Exit While
                        End If
                    End While
                    Try : IO.Directory.Delete(Me.SelectedOutputDirectory & "\Disc " & folderNum, True) : Catch : End Try
                End If
            End If
            Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = "")

        Else

            Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Max = Me.Label_Discs_Value.Text.Split(" ")(0))
            If Me.Label_Discs_Value.Text.Split(" ")(0) = 1 Then Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "08")) Else Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "27"))
            Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextShow = ProgBar.ProgBarPlus.eTextShow.FormatString)

            ' Zip
            If Me.CopyMode = "Zip" And Not Me.WantToCancelThread = True Then
                Me.ThreadProcess = New Threading.Thread(AddressOf Me.Process_Thread) With {
                    .IsBackground = True
                }
                Me.ThreadProcess.Start("7zip")
                While Not Me.ThreadProcessIsCompleted = True
                    If Me.WantToCancelThread Then
                        Exit While
                    Else
                        Dim volumeCount As Integer = IO.Directory.GetFiles(Me.SelectedOutputDirectory, "*.zip.*").Length
                        If volumeCount = 0 Then Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 1) Else Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = volumeCount)
                        Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                        Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                        Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                        Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                    End If
                End While
            End If

            ' Rar
            If Me.CopyMode = "Rar" And Not Me.WantToCancelThread = True Then
                Me.ThreadProcess = New Threading.Thread(AddressOf Me.Process_Thread) With {
                    .IsBackground = True
                }
                Me.ThreadProcess.Start("Rar")
                While Not Me.ThreadProcessIsCompleted = True
                    If Me.WantToCancelThread Then
                        Exit While
                    Else
                        Dim volumeCount As Integer = IO.Directory.GetFiles(Me.SelectedOutputDirectory, "*part*.rar").Length
                        If volumeCount = 0 Then Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 1) Else Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = volumeCount)
                        Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                        Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                    End If
                End While
            End If

            ' Sfx
            If Me.CopyMode = "Sfx" And Not Me.WantToCancelThread = True Then
                Me.ThreadProcess = New Threading.Thread(AddressOf Me.Process_Thread) With {
                    .IsBackground = True
                }
                Me.ThreadProcess.Start("Sfx")
                While Not Me.ThreadProcessIsCompleted = True
                    If Me.WantToCancelThread Then
                        Exit While
                    Else
                        Dim volumeCount As Integer = IO.Directory.GetFiles(Me.SelectedOutputDirectory, "*part*.rar").Length
                        If volumeCount = 0 Then Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 1) Else Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = volumeCount)
                        Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                        Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                    End If
                End While
            End If
        End If

        ' Tar
        If Me.CopyMode = "Tar" And Not Me.WantToCancelThread = True Then
            Me.ThreadProcess = New Threading.Thread(AddressOf Me.Process_Thread) With {
                .IsBackground = True
            }
            Me.ThreadProcess.Start("Tar")
            While Not Me.ThreadProcessIsCompleted = True
                If Me.WantToCancelThread Then
                    Exit While
                Else
                    Dim volumeCount As Integer = IO.Directory.GetFiles(Me.SelectedOutputDirectory, "*.tar.*").Length
                    If volumeCount = 0 Then Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 1) Else Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = volumeCount)
                    Me.Set_TaskBar_Value(Me.ProgBarPlus.ValuePercent, 100)
                    Me.InvokeControl(Me, Sub(x) x.Text = Me.ProgBarPlus.ValuePercent & "% Splitty")
                End If
            End While
        End If

        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.TextFormat = " ")
        Me.InvokeControl(Me.ProgBarPlus, Sub(x) x.Value = 0)

        ' Clean up
        Try
            IO.File.Delete(Me.TempDir & "7z.dll")
            IO.File.Delete(Me.TempDir & "Splitty_7zip.exe")
            IO.File.Delete(Me.TempDir & "PowerISO.exe")
            IO.File.Delete(Me.TempDir & "Splitty_Piso.exe")
            IO.File.Delete(Me.TempDir & "Splitty_WinRAR.7z")
            IO.File.Delete(Me.TempDir & "Splitty_WinRAR.exe")
            IO.File.Delete(Me.TempDir & "rarreg.key")
            IO.File.Delete(Me.TempDir & "Default.SFX")
        Catch : End Try

        If Not Me.WantToCancelThread Then
            Me.Set_TaskBar_Value(100, 100)
            Me.InvokeControl(Me, Sub(x) x.Text = "100% Splitty")
        End If

        Me.Set_Folder_Access(Me.SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
        Me.ThreadProcessIsCompleted = False
        Me.ThreadIsCompleted = True
    End Sub

#End Region

#Region " Process thread "

    Private Sub Process_Thread(process As String)
        If process = "7zip" Then
            Me.Run_Process(Me.TempDir & "Splitty_7zip.exe", " a " & """" & Me.SelectedOutputDirectory & "\Disc.zip" & """" & " " & """" & Me.SelectedDirectory & """" & " -v" & Me.SelectedDiscBytes.ToString & "b " & " -mx=0 -bd -tzip", False, True)
        ElseIf process = "Rar" Then
            Me.Run_Process(Me.TempDir & "Splitty_WinRar.exe", " a " & """" & Me.SelectedOutputDirectory & "\Disc.rar" & """" & " " & """" & Me.SelectedDirectory & """" & " -v" & Me.SelectedDiscBytes.ToString & "b " & " -m0  -ibck -o+", False, True)
        ElseIf process = "Sfx" Then
            Me.Run_Process(Me.TempDir & "Splitty_WinRar.exe", " a -sfx " & """" & Me.SelectedOutputDirectory & "\Disc.exe" & """" & " " & """" & Me.SelectedDirectory & """" & " -v" & Me.SelectedDiscBytes.ToString & "b " & " -m0  -ibck -o+", False, True)
        ElseIf process = "Tar" Then
            Me.Run_Process(Me.TempDir & "Splitty_7zip.exe", " a " & """" & Me.SelectedOutputDirectory & "\Disc.tar" & """" & " " & """" & Me.SelectedDirectory & """" & " -v" & Me.SelectedDiscBytes.ToString & "b " & " -mx=0 -bd -ttar", False, True)
        End If
        Me.ThreadProcessIsCompleted = True
    End Sub

#End Region

#End Region


End Class