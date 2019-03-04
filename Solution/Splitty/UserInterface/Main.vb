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

    Dim architecture = Get_OS_Architecture()

#End Region

#Region " Properties "

    Public Property SelectedDirectory() As String
        Get
            Return TextBox_Input_Folder.Text
        End Get
        Set(ByVal value As String)
            TextBox_Input_Folder.Text = value
        End Set
    End Property

    Public Property SelectedOutputDirectory() As String
        Get
            Return TextBox_Output_Folder.Text
        End Get
        Set(ByVal value As String)
            TextBox_Output_Folder.Text = value
        End Set
    End Property

#End Region

#Region " Form "

#Region " Load/Close "

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = My.Settings.Version
        PictureBox_Disc.BackgroundImage = GrayScale_Image(PictureBox_Disc.BackgroundImage, GrayScale.MidGray)
        Button_Split.Image = GrayScale_Image(Button_Split.Image, GrayScale.MidGray)
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Try
            If Not ThreadSplit.ThreadState = Threading.ThreadState.Unstarted And Not ThreadSplit.ThreadState = Threading.ThreadState.Aborted And Not ThreadSplit.ThreadState = Threading.ThreadState.Stopped Then
                Dim answer = MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "01"), My.Settings.Version, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                If answer = MsgBoxResult.Yes Then
                    Set_Folder_Access(SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
                    Kill_Process("Splitty_7zip")
                    Kill_Process("Splitty_WinRar")
                    Kill_Process("Splitty_Piso")
                    Try : ThreadSplit.Abort() : Catch : End Try
                    End
                Else
                    e.Cancel = True
                End If
            Else
                Set_Folder_Access(SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
                Try : ThreadSplit.Abort() : Catch : End Try
            End If
        Catch
        End Try
    End Sub

#End Region

#Region " Buttons folders "

    ' Input folder
    Private Sub Button_Input_Folder_Click(sender As Object, e As MouseEventArgs) Handles Button_Input_Folder.ClickButtonArea, Button_Folder.ClickButtonArea
        Dim selectFolder As New Ookii.Dialogs.VistaFolderBrowserDialog
        selectFolder.ShowNewFolderButton = True
        If selectFolder.ShowDialog.ToString() = "OK" Then
            If selectFolder.SelectedPath = SelectedOutputDirectory Then
                MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "04"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                SelectedDirectory = Nothing
                Button_Split.Enabled = False
                GroupPanel_Options.Enabled = False
                GroupBox_Mode.Enabled = False
                GroupBox_Information.Enabled = False
                Label_Size_Value.Text = "0"
                Label_Discs_Value.Text = "0"
                Exit Sub
            End If
            SelectedDirectory = (selectFolder.SelectedPath)
            Create_FileSystemWatcher(SelectedDirectory)
            TextBox_Input_Folder.Text = SelectedDirectory
            SelectedDirSizeBytes = Get_Directory_Size(New IO.DirectoryInfo(SelectedDirectory), True) ' Set Bytes of directory size
            SelectedDirSizeConvertedBytes = Round_Bytes(SelectedDirSizeBytes) ' Set MB or GB or TB of directory size
            Label_Size_Value.Text = SelectedDirSizeConvertedBytes ' & "   (" & Selected_Dir_Size_Bytes & " Bytes)"
            Button_Output_Folder.Enabled = True
            TextBox_Output_Folder.Enabled = True
            GroupPanel_Options.Enabled = True
            GroupBox_Mode.Enabled = True
            RadioButton_CD.Checked = False
            RadioButton_CD.Checked = True
            Label_Discs_Name.Visible = True
            Label_Discs_Value.Visible = True
            Label_Size_Name.Visible = True
            Label_Size_Value.Visible = True
            TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Output folder
    Private Sub Button_Output_Folder_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Output_Folder.ClickButtonArea, CButton1.ClickButtonArea
        Dim selectFolder As New Ookii.Dialogs.VistaFolderBrowserDialog
        selectFolder.ShowNewFolderButton = True
        If selectFolder.ShowDialog.ToString() = "OK" Then
            If selectFolder.SelectedPath = SelectedDirectory Then
                MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "03"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                SelectedOutputDirectory = Nothing
                Exit Sub
            End If
            SelectedOutputDirectory = (selectFolder.SelectedPath)
            TextBox_Output_Folder.Text = SelectedOutputDirectory

            If RadioButton_Custom_Size.Checked = True Then
                If Not TextBox_Custom_Size.Text = 0 Then
                    Button_Split.Enabled = True
                    Button_Split.ForeColor = Color.Black
                    Button_Split.Enabled = True
                End If
            Else
                Button_Split.Image = My.Resources.split
                Button_Split.ForeColor = Color.Black
                Button_Split.Enabled = True
            End If

        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

#End Region

#Region " Language "

    Private Sub EnglishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnglishToolStripMenuItem.Click
        LanguageResource = "ENG"
        Label_Input.Location = New Point(15, Label_Input.Location.Y)
        Label_Capacity.Size = New Point(69, Label_Capacity.Size.Height)
        Label_Discs_Name.Size = New Point(54, Label_Discs_Name.Size.Height)
        Label_Size_Name.Size = New Point(60, Label_Size_Name.Size.Height)
        Label_Discs_Value.Location = New Point(58, Label_Discs_Value.Location.Y)
        Label_Size_Value.Location = New Point(58, Label_Size_Value.Location.Y)
        Change_Lang()
    End Sub

    Private Sub SpanishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SpanishToolStripMenuItem.Click
        LanguageResource = "SPA"
        Label_Input.Location = New Point(0, Label_Input.Location.Y)
        Label_Capacity.Size = New Point(78, Label_Capacity.Size.Height)
        Label_Discs_Name.Size = New Point(64, Label_Discs_Name.Size.Height)
        Label_Size_Name.Size = New Point(64, Label_Size_Name.Size.Height)
        Label_Discs_Value.Location = New Point(68, Label_Discs_Value.Location.Y)
        Label_Size_Value.Location = New Point(68, Label_Size_Value.Location.Y)
        Change_Lang()
    End Sub


    Private Sub Change_Lang()
        Label_Input.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "11")
        Label_Output.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "12")
        Label_Capacity.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "13")
        Label_Mode.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "14")
        Label_Information.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "15")
        Label_Size_Name.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "16")
        Label_Discs_Name.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "17")
        Button_Split.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "18")
        Button_Stop.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "19")
        RadioButton_Copy.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "20")
        RadioButton_Custom_Size.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "21")
        AboutToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "22")
        LanguageToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "23")
        EnglishToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "24")
        SpanishToolStripMenuItem.Text = My.Resources.ResourceManager.GetObject(LanguageResource & "25")
        TextBox_Input_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
        If Not TextBox_Output_Folder.HintDetails.HintText = "" Then TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
        If Not Label_Discs_Value.Text = Nothing Then Label_Discs_Value.Text = Math.Ceiling((SelectedDirSizeBytes / SelectedDiscBytes)) & My.Resources.ResourceManager.GetObject(LanguageResource & "26")
    End Sub

#End Region

#Region " Drag&Drop Input Folder "

    ' FolderTextbox Drag-Drop
    Private Sub Input_Folder_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles TextBox_Input_Folder.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim folderObject As String() = e.Data.GetData(DataFormats.FileDrop)
            If System.IO.Directory.Exists(folderObject(0)) Then
                TextBox_Input_Folder.Text = folderObject(0)
                SelectedDirectory = folderObject(0)
                If SelectedDirectory = SelectedOutputDirectory Then
                    MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "04"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                    SelectedDirectory = Nothing
                    Button_Split.Enabled = False
                    GroupPanel_Options.Enabled = False
                    GroupBox_Mode.Enabled = False
                    GroupBox_Information.Enabled = False
                    Label_Size_Value.Text = "0"
                    Label_Discs_Value.Text = "0"
                    Exit Sub
                End If
                Create_FileSystemWatcher(SelectedDirectory)
                SelectedDirSizeBytes = Get_Directory_Size(New IO.DirectoryInfo(SelectedDirectory), True) ' Set Bytes of directory size
                SelectedDirSizeConvertedBytes = Round_Bytes(SelectedDirSizeBytes) ' Set MB or GB or TB of directory size
                Label_Size_Value.Text = SelectedDirSizeConvertedBytes '& "   (" & Selected_Dir_Size_Bytes & " Bytes)"
                Button_Output_Folder.Enabled = True
                TextBox_Output_Folder.Enabled = True
                GroupPanel_Options.Enabled = True
                GroupBox_Mode.Enabled = True
                RadioButton_CD.Checked = False
                RadioButton_CD.Checked = True
                Label_Discs_Name.Visible = True
                Label_Discs_Value.Visible = True
                Label_Size_Name.Visible = True
                Label_Size_Value.Visible = True
                TextBox_Output_Folder.HintDetails.HintText = My.Resources.ResourceManager.GetObject(LanguageResource & "02")
            Else
                MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "05"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        End If
    End Sub

    Private Sub Output_Folder_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles TextBox_Output_Folder.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim folderObject As String() = e.Data.GetData(DataFormats.FileDrop)
            If System.IO.Directory.Exists(folderObject(0)) Then
                SelectedOutputDirectory = (folderObject(0))
                TextBox_Output_Folder.Text = SelectedOutputDirectory
                If SelectedOutputDirectory = SelectedDirectory Then
                    MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "03"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                    SelectedOutputDirectory = Nothing
                    Exit Sub
                End If
                If RadioButton_Custom_Size.Checked = True Then
                    If Not TextBox_Custom_Size.Text = 0 Then
                        Button_Split.Enabled = True
                        Button_Split.ForeColor = Color.Black
                        Button_Split.Enabled = True
                    End If
                Else
                    Button_Split.Image = My.Resources.split
                    Button_Split.ForeColor = Color.Black
                    Button_Split.Enabled = True
                End If

            End If
            Flush_Memory(Application.ExecutablePath)
        Else
            MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "05"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Stop)
        End If
    End Sub


    ' FolderTextbox Drag-Enter
    Private Sub Folder_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles TextBox_Input_Folder.DragEnter, TextBox_Output_Folder.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then e.Effect = DragDropEffects.All
    End Sub

#End Region

#Region " FileSystem Watcher "

    Private Sub Create_FileSystemWatcher(ByVal path As String)
        FileSystemWatcher1.Path = path
        'AddHandler FileSystemWatcher1.Changed, AddressOf Watcher_Changed
        AddHandler FileSystemWatcher1.Created, AddressOf Watcher_Changed
        AddHandler FileSystemWatcher1.Deleted, AddressOf Watcher_Changed
        AddHandler FileSystemWatcher1.Renamed, AddressOf Watcher_Changed
    End Sub

    Private Sub Watcher_Changed(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs)
        If Button_Split.Visible Then
            Dim answer = MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "32"), My.Settings.Version, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If answer = MsgBoxResult.Yes Then
                SelectedDirSizeBytes = Get_Directory_Size(New IO.DirectoryInfo(SelectedDirectory), True) ' Set Bytes of directory size
                SelectedDirSizeConvertedBytes = Round_Bytes(SelectedDirSizeBytes) ' Set MB or GB or TB of directory size
                Label_Size_Value.Text = SelectedDirSizeConvertedBytes ' & "   (" & Selected_Dir_Size_Bytes & " Bytes)"
                Label_Discs_Value.Text = Math.Ceiling((SelectedDirSizeBytes / SelectedDiscBytes)) & " " & Label_Discs_Value.Text.Split(" ")(1)
            End If
        End If
    End Sub

#End Region

#Region " Radio buttons Capacity "

    Private Sub RadioButton_CD_Click(sender As Object, e As EventArgs) Handles RadioButton_CD.CheckedChanged
        If RadioButton_CD.Checked = True Then
            SelectedDiscFormat = "CD"
            SelectedDiscBytes = 734003200
            PictureBox_Disc.BackgroundImage = My.Resources.cd
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.CD))) & " CD's"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_CD800_Click(sender As Object, e As EventArgs) Handles RadioButton_CD800.CheckedChanged
        If RadioButton_CD800.Checked = True Then
            SelectedDiscFormat = "CD800"
            SelectedDiscBytes = 829440393.216
            PictureBox_Disc.BackgroundImage = My.Resources.cd
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.CD800MB))) & SelectedDiscFormatLabel & " CD's"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_CD900_Click(sender As Object, e As EventArgs) Handles RadioButton_CD900.CheckedChanged
        If RadioButton_CD900.Checked = True Then
            SelectedDiscFormat = "CD900"
            SelectedDiscBytes = 912383803.392
            PictureBox_Disc.BackgroundImage = My.Resources.cd
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.CD900MB))) & SelectedDiscFormatLabel & " CD's"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd5Click(sender As Object, e As EventArgs) Handles RadioButton_DVD5.CheckedChanged
        If RadioButton_DVD5.Checked = True Then
            SelectedDiscFormat = "DVD5"
            SelectedDiscBytes = 4700000000
            PictureBox_Disc.BackgroundImage = My.Resources.dvd
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.Dvd5))) & SelectedDiscFormatLabel & " DVD's"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd9Click(sender As Object, e As EventArgs) Handles RadioButton_DVD9.CheckedChanged
        If RadioButton_DVD9.Checked = True Then
            SelectedDiscFormat = "DVD9"
            SelectedDiscBytes = 8500000000
            PictureBox_Disc.BackgroundImage = My.Resources.dvd
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.Dvd9))) & SelectedDiscFormatLabel & " DVD's"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButtonDvd10Click(sender As Object, e As EventArgs) Handles RadioButton_DVD10.CheckedChanged
        If RadioButton_DVD10.Checked = True Then
            SelectedDiscFormat = "DVD10"
            SelectedDiscBytes = 9395240960
            PictureBox_Disc.BackgroundImage = My.Resources.dvd
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.Dvd10))) & SelectedDiscFormatLabel & " DVD's"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_Click(sender As Object, e As EventArgs) Handles RadioButton_BluRay.CheckedChanged
        If RadioButton_BluRay.Checked = True Then
            SelectedDiscFormat = "BR"
            SelectedDiscBytes = 25025314816
            PictureBox_Disc.BackgroundImage = My.Resources.bluray
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.BR))) & SelectedDiscFormatLabel & " BluRays"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_DL_Click(sender As Object, e As EventArgs) Handles RadioButton_BluRay_DL.CheckedChanged
        If RadioButton_BluRay_DL.Checked = True Then
            SelectedDiscFormat = "BR-DL"
            SelectedDiscBytes = 50050629632
            PictureBox_Disc.BackgroundImage = My.Resources.bluray
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.BRDoubleLayer))) & SelectedDiscFormatLabel & " BluRays"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    Private Sub RadioButton_BluRay_MiniDisc_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_BluRay_MiniDisc.CheckedChanged
        If RadioButton_BluRay_MiniDisc.Checked = True Then
            SelectedDiscFormat = "BR-MD"
            SelectedDiscBytes = 7791181824
            PictureBox_Disc.BackgroundImage = My.Resources.bluray
            Label_Discs_Value.Text = Math.Ceiling((Convert_To_Disc_Size(SelectedDirSizeBytes, SizeType.Bytes, DiscType.BRMiniDisc))) & SelectedDiscFormatLabel & " BluRays"
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

#End Region

#Region " Radio buttons Copy mode "

    Private Sub RadioButtonCopy(sender As Object, e As EventArgs) Handles RadioButton_Copy.CheckedChanged
        CopyMode = "Copy"
    End Sub

    Private Sub RadioButtonRar(sender As Object, e As EventArgs) Handles RadioButton_Rar.CheckedChanged
        CopyMode = "Rar"
    End Sub

    Private Sub RadioButtonZip(sender As Object, e As EventArgs) Handles RadioButton_Zip.CheckedChanged
        CopyMode = "Zip"
    End Sub

    Private Sub RadioButtonSfx(sender As Object, e As EventArgs) Handles RadioButton_SFX.CheckedChanged
        CopyMode = "Sfx"
    End Sub

    Private Sub RadioButtonIso(sender As Object, e As EventArgs) Handles RadioButton_ISO.CheckedChanged
        CopyMode = "Iso"
    End Sub

    Private Sub RadioButtonTar(sender As Object, e As EventArgs) Handles RadioButton_TAR.CheckedChanged
        CopyMode = "Tar"
    End Sub

#End Region

#Region " Custom size option "

    ' RadioButton custom size
    Private Sub RadioButton_Custom(sender As Object, e As EventArgs) Handles RadioButton_Custom_Size.CheckedChanged
        If RadioButton_Custom_Size.Checked = True Then
            SelectedDiscFormat = "CUSTOM"
            PictureBox_Disc.BackgroundImage = My.Resources.custom
            TextBox_Custom_Size.Enabled = True
            ComboBox_Custom_Size.Enabled = True
            'TextBox_Custom_Size.HintDetails.HintText = "0123456789,"

            If ComboBox_Custom_Size.SelectedItem = Nothing Then
                Label_Discs_Value.Text = "0"
                ComboBox_Custom_Size.SelectedIndex = 0
            End If
            If TextBox_Custom_Size.Text = 0 Or SelectedOutputDirectory = Nothing Then Button_Split.Enabled = False Else Button_Split.Enabled = True
        Else
            If SelectedOutputDirectory = Nothing Then Button_Split.Enabled = False Else Button_Split.Enabled = True
            TextBox_Custom_Size.HintDetails.HintText = ""
            TextBox_Custom_Size.Enabled = False
            ComboBox_Custom_Size.Enabled = False
        End If
        Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Textbox custom size
    Private Sub TextBox_Custom_Size_TextChanged(sender As Object, e As EventArgs) Handles TextBox_Custom_Size.TextChanged, RadioButton_Custom_Size.CheckedChanged
        TextBox_Custom_Size.Text = TextBox_Custom_Size.Text.Replace(".", ",")

        If TextBox_Custom_Size.Text.Length = 0 Then TextBox_Custom_Size.Text = 0

        If Not TextBox_Custom_Size.Text.Length = 0 Then
            If ComboBox_Custom_Size.SelectedIndex = 0 Then
                SelectedDiscBytes = (TextBox_Custom_Size.Text * 1048576)
            End If

            If ComboBox_Custom_Size.SelectedIndex = 1 Then
                SelectedDiscBytes = (TextBox_Custom_Size.Text * 1073741824)
            End If

            If ComboBox_Custom_Size.SelectedIndex = 2 Then
                SelectedDiscBytes = (TextBox_Custom_Size.Text * 1099511627776)
            End If

            Label_Discs_Value.Text = Math.Ceiling((SelectedDirSizeBytes / SelectedDiscBytes)) & My.Resources.ResourceManager.GetObject(LanguageResource & "26")
        End If

        If SelectedOutputDirectory = Nothing Or TextBox_Custom_Size.Text = 0 Then Button_Split.Enabled = False Else Button_Split.Enabled = True
    End Sub

    ' Textbox custom size (Keypress)
    Private Sub TextBox_Custom_Size_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox_Custom_Size.KeyPress
        e.Handled = NumericOnly(e.KeyChar)
    End Sub

    ' ComboBox custom size
    Private Sub ComboBox_Custom_Size_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_Custom_Size.SelectedIndexChanged
        If ComboBox_Custom_Size.SelectedIndex = 0 Then
            SelectedDiscBytes = (TextBox_Custom_Size.Text * 1048576)
        End If
        If ComboBox_Custom_Size.SelectedIndex = 1 Then
            SelectedDiscBytes = (TextBox_Custom_Size.Text * 1073741824)
        End If
        If ComboBox_Custom_Size.SelectedIndex = 2 Then
            SelectedDiscBytes = (TextBox_Custom_Size.Text * 1099511627776)
        End If
        Label_Discs_Value.Text = Math.Ceiling((SelectedDirSizeBytes / SelectedDiscBytes)) & " of custom storage"
    End Sub

#End Region

#Region " Button Split "

    ' Button SPLIT
    Private Sub Button_Split_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Split.ClickButtonArea

        If RadioButton_Custom_Size.Checked = True And SelectedDiscBytes > SelectedDirSizeBytes Then
            MessageBox.Show( _
                My.Resources.ResourceManager.GetObject(LanguageResource & "29") & vbNewLine & vbNewLine & _
                My.Resources.ResourceManager.GetObject(LanguageResource & "30") & SelectedDirSizeConvertedBytes & vbNewLine & _
                My.Resources.ResourceManager.GetObject(LanguageResource & "31") & Round_Bytes(SelectedDiscBytes) _
                , My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Exit Sub
        End If

        Dim tempImage = PictureBox_Disc.BackgroundImage
        PictureBox_Disc.BackgroundImage = GrayScale_Image(PictureBox_Disc.BackgroundImage, GrayScale.MidGray)

        ThreadProcessIsCompleted = False
        ThreadIsCompleted = False
        ThreadSplit = New Threading.Thread(AddressOf Split_Thread)
        ThreadSplit.IsBackground = True

        Button_Split.Enabled = False
        Button_Split.Visible = False
        Button_Stop.Visible = True
        Button_Stop.Enabled = True
        GroupPanel_Options.Enabled = False
        GroupBox_Mode.Enabled = False
        TextBox_Output_Folder.Enabled = False
        TextBox_Input_Folder.Enabled = False
        Button_Input_Folder.Enabled = False
        Button_Output_Folder.Enabled = False

        ThreadSplit.Start()
        While Not ThreadIsCompleted = True
            Application.DoEvents()
        End While

        Set_Folder_Access(SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
        If Not WantToCancelThread Then MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "09"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Set_TaskBar_Status(TaskBarStatus.Disabled)

        WantToCancelThread = False
        ProgBarPlus.TextShow = ProgBar.ProgBarPlus.eTextShow.None
        Button_Stop.Visible = False
        Button_Stop.Enabled = False
        Button_Split.Enabled = True
        Button_Split.Visible = True
        GroupPanel_Options.Enabled = True
        GroupBox_Mode.Enabled = True
        TextBox_Output_Folder.Enabled = True
        TextBox_Input_Folder.Enabled = True
        Button_Input_Folder.Enabled = True
        Button_Output_Folder.Enabled = True
        PictureBox_Disc.BackgroundImage = tempImage

        Flush_Memory(Application.ExecutablePath)
    End Sub

    ' Button STOP
    Private Sub Button_Stop_ClickButtonArea(sender As Object, e As MouseEventArgs) Handles Button_Stop.ClickButtonArea
        WantToCancelThread = True
        Set_Folder_Access(SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
        Set_TaskBar_Value(100, 100)
        InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Set_TaskBar_Status(TaskBarStatus.Stopped)
        Kill_Process("Splitty_7zip")
        Kill_Process("Splitty_WinRar")
        Kill_Process("Splitty_Piso")
        MessageBox.Show(My.Resources.ResourceManager.GetObject(LanguageResource & "28"), My.Settings.Version, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
        Set_TaskBar_Value(0, 100)
        InvokeControl(Me, Sub(x) x.Text = My.Settings.Version)
        Set_TaskBar_Status(TaskBarStatus.Disabled)
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
            If includeSubfolders Then dirTotalSize += directory.EnumerateDirectories().Sum(Function(dir) Get_Directory_Size(dir, True))
            Return dirTotalSize
        Catch
        End Try
        Return Nothing
    End Function

#End Region

#Region " Get All Files Function "

    Public Sub Get_All_Files(ByVal aDir As IO.DirectoryInfo)
        Dim nextDir As IO.DirectoryInfo
        WorkWithFilesInDir(aDir)
        For Each nextDir In aDir.GetDirectories
            Application.DoEvents()
            Get_All_Files(nextDir)
        Next
    End Sub

    Public Sub WorkWithFilesInDir(ByVal aDir As IO.DirectoryInfo)
        Dim aFile As IO.FileInfo
        For Each aFile In aDir.GetFiles()
            Application.DoEvents()
            Try
                FilesList.Add(aFile.DirectoryName & "|" & aFile.Name & "|" & aFile.Length)
                TotalFilesNumber += 1
            Catch pathTooLongException As Exception
                Dim answer = MessageBox.Show(pathTooLongException.Message, My.Settings.Version, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1)
                If answer = Windows.Forms.DialogResult.Abort Then
                    Button_Stop.PerformClick()
                    Exit For
                    Exit Sub
                End If
                If answer = Windows.Forms.DialogResult.Retry Then Get_All_Files(New IO.DirectoryInfo(SelectedDirectory))
            End Try
        Next
    End Sub

#End Region

#Region " Convert Bytes Function "

    Public Function Round_Bytes(ByVal byteSize As Long) As String

        Dim sizeOfKB As Long = 1024 ' 1KB
        Dim sizeOfMB As Long = 1048576 ' 1MB
        Dim sizeOfGB As Long = 1073741824 ' 1GB
        Dim sizeOfTB As Long = 1099511627776 ' 1TB
        Dim sizeofPB As Long = 1125899906842624 ' 1PB

        Dim tempFileSize As Double

        If byteSize < sizeOfKB Then 'Filesize is in Bytes
            tempFileSize = Convert_Bytes(byteSize, ConvTo.B)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize) & " bytes" 'Return our converted value

        ElseIf byteSize >= sizeOfKB And byteSize < sizeOfMB Then 'Filesize is in Kilobytes
            tempFileSize = Convert_Bytes(byteSize, ConvTo.KB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize) & " KB"

        ElseIf byteSize >= sizeOfMB And byteSize < sizeOfGB Then ' Filesize is in Megabytes
            tempFileSize = Convert_Bytes(byteSize, ConvTo.MB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize, 1) & " MB"

        ElseIf byteSize >= sizeOfGB And byteSize < sizeOfTB Then 'Filesize is in Gigabytes
            tempFileSize = Convert_Bytes(byteSize, ConvTo.GB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize, 1) & " GB"

        ElseIf byteSize >= sizeOfTB And byteSize < sizeofPB Then 'Filesize is in Terabytes
            tempFileSize = Convert_Bytes(byteSize, ConvTo.TB)
            If tempFileSize = -1 Then Return Nothing
            Return Math.Round(tempFileSize, 1) & " TB"
        Else
            Return Nothing 'Invalid filesize so return Nothing
        End If
    End Function

    Public Function Convert_Bytes(ByVal bytes As Long, ByVal convertTo As ConvTo) As Double
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

    Private Function Convert_To_Disc_Size(ByVal fileSize As Double, ByVal sizeType As SizeType, ByVal toDiscType As DiscType) As Double
        Dim size As Double = GetSize(toDiscType)
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

    Private Function GetSize(ByVal discType As DiscType) As Double
        Select Case discType
            Case discType.CD : Return 734003200 ' CD Standard
            Case discType.CD800MB : Return 829440393.216 ' CD 800 MB
            Case discType.CD900MB : Return 912383803.392 ' CD 900 MB
            Case discType.Dvd5 : Return 4700000000 ' DVD Standard (DVD5)
            Case discType.Dvd9 : Return 8500000000 ' DVD Double Layer (DVD9)
            Case discType.Dvd10 : Return 9395240960 ' DVD Double Layer (DVD10)
            Case discType.BR : Return 25025314816 ' BluRay Standard
            Case discType.BRDoubleLayer : Return 50050629632 ' BluRay Double Layer
            Case discType.BRMiniDisc : Return 7791181824 ' BluRay MiniDisc Standard
            Case discType.BRMiniDiscDoubleLayer : Return 15582363648 ' BluRay MiniDisc Double Layer
            Case Else
                Return -1 ' Por si se declara un nuevo valor en el enumerador sin especificar tamaño
        End Select
    End Function

#End Region

#Region " Copy File Function "

    Private Function Copy_File(ByVal file As String, ByVal targetPath As String, _
                               Optional ByVal forceTargetPath As Boolean = False, Optional ByVal forceFileReplace As Boolean = False, _
                               Optional ByVal attributes As System.IO.FileAttributes = IO.FileAttributes.Normal)

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

    Private Function Get_File_Info(ByVal file As String, ByVal information As String)
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

    Public Function NumericOnly(ByVal eChar As Char) As Boolean
        Dim chkStr As String = "0123456789,"
        If chkStr.IndexOf(eChar) > -1 OrElse eChar = vbBack Then
            If eChar = Chr(44) And TextBox_Custom_Size.Text.Contains(",") Then Return True
            Return False
        Else
            Return True
        End If
    End Function

#End Region

#Region " Flush memory Function "

    Declare Function SetProcessWorkingSetSize Lib "kernel32.dll" (ByVal process As IntPtr, ByVal minimumWorkingSetSize As Integer, ByVal maximumWorkingSetSize As Integer) As Integer

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

    Private Function GrayScale_Image(ByVal image As Image, ByVal grayTone As GrayScale) As Bitmap
        Dim imageBitmap As Bitmap = New Bitmap(image.Width, image.Height)
        Dim imageGraphic As Graphics = Graphics.FromImage(imageBitmap)
        Dim colorMatrix As System.Drawing.Imaging.ColorMatrix = Nothing
        Select Case grayTone
            Case GrayScale.LightGray : colorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0.5, 0.5, 0.5, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
            Case GrayScale.MidGray : colorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0.5, 0.5, 0.5, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
            Case GrayScale.DarkGray : colorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() {New Single() {0, 0, 0, 0, 0}, New Single() {0, 0, 0, 0, 0}, New Single() {0.2, 0.2, 0.2, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})
        End Select
        Dim imageAttributes As System.Drawing.Imaging.ImageAttributes = New System.Drawing.Imaging.ImageAttributes()
        imageAttributes.SetColorMatrix(colorMatrix)
        imageGraphic.DrawImage(image, New Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes)
        imageGraphic.Dispose()
        Return imageBitmap
    End Function

#End Region

#Region " Delimit String Function "

    Private Function Delimit_String(ByVal str As String, ByVal delimiterA As String, Optional ByVal delimiterB As String = "", Optional ByVal ignoreCase As Boolean = False, Optional ByVal leftOrRight As String = "Right") As String
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

    Private Function Run_Process(ByVal processName As String, Optional processArguments As String = Nothing, Optional readOutput As Boolean = False, Optional processHide As Boolean = False)

        Try

            Dim myProcess As New Process()
            Dim myProcessInfo As New ProcessStartInfo()

            myProcessInfo.FileName = processName ' Process filename
            myProcessInfo.Arguments = processArguments ' Process arguments
            myProcessInfo.CreateNoWindow = processHide ' Show or hide the process Window
            myProcessInfo.UseShellExecute = False ' Don't use system shell to execute the process
            myProcessInfo.RedirectStandardOutput = readOutput '  Redirect (1) Output
            myProcessInfo.RedirectStandardError = readOutput ' Redirect non (1) Output
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

    Private Function Load_Resource_To_Disk(ByVal resource As Byte(), ByVal targetFile As String) As Boolean
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

    Public Sub InvokeControl(Of T As Control)(ByVal control As T, ByVal action As Action(Of T))
        If control.InvokeRequired Then
            control.Invoke(New Action(Of T, Action(Of T))(AddressOf InvokeControl), New Object() {control, action})
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

    Private Function HexToByteArray(ByVal hexString As String) As Byte()
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

    Private Function Set_TaskBar_Status(ByVal taskBarStatus As TaskBarStatus) As Boolean
        Try : Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance.SetProgressState(taskBarStatus)
            Return True
        Catch ex As Exception : Throw New Exception(ex.Message)
        End Try
    End Function

#End Region

#Region " Set TaskBar Value Function "

    Private Function Set_TaskBar_Value(ByVal currentValue As Integer, ByVal maxValue As Integer) As Boolean
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

    Private Function Set_Folder_Access(ByVal path As String, ByVal folderAccess As FolderAccess, ByVal action As Action) As Boolean
        Try
            Dim folderInfo As IO.DirectoryInfo = New IO.DirectoryInfo(path)
            Dim folderAcl As New System.Security.AccessControl.DirectorySecurity
            folderAcl.AddAccessRule( _
                New System.Security.AccessControl.FileSystemAccessRule( _
                    My.User.Name, _
                    folderAccess, _
                    System.Security.AccessControl.InheritanceFlags.ContainerInherit Or System.Security.AccessControl.InheritanceFlags.ObjectInherit, System.Security.AccessControl.PropagationFlags.None, _
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

        FilesList.Clear()
        CachedSize = 0
        TotalFilesNumber = 0
        Set_TaskBar_Value(0, 100)
        Set_TaskBar_Status(TaskBarStatus.Normal)

        ' // Extract resources //
        '
        ' Extract - 7zip
        If Not CopyMode = "Copy" Then
            If architecture = 32 Then
                If Not IO.File.Exists(TempDir & "Splitty_7zip.exe") Then Load_Resource_To_Disk(My.Resources.Splitty_7zip_x86, TempDir & "Splitty_7zip.exe")
            ElseIf architecture = 64 Then
                If Not IO.File.Exists(TempDir & "Splitty_7zip.exe") Then Load_Resource_To_Disk(My.Resources.Splitty_7zip_x64, TempDir & "Splitty_7zip.exe")
                If Not IO.File.Exists(TempDir & "7z.dll") Then Load_Resource_To_Disk(My.Resources._7z, TempDir & "7z.dll")
            End If
        End If

        ' Extract - PowerISO
        If CopyMode = "Iso" Then
            If Not IO.File.Exists(TempDir & "Splitty_Piso.exe") Then Load_Resource_To_Disk(My.Resources.Splitty_PowerISO_x86, TempDir & "Splitty_PowerISO.7z")
            SelectedDiscBytes -= 93491 ' Extra bytes needed to create the ISO file
            Dim powerIsoKey = HexToByteArray("000a42494c4c2047415445535ad50adc4f5ca6f9efc1252aadf9847f") ' PowerISO Premium Key
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\PowerISO", "USER", powerIsoKey, Microsoft.Win32.RegistryValueKind.Binary)

            ' Extract - WinRAR
        ElseIf CopyMode = "Rar" Or CopyMode = "Sfx" Then
            If architecture = 32 Then
                If Not IO.File.Exists(TempDir & "Splitty_WinRAR.exe") Then Load_Resource_To_Disk(My.Resources.Splitty_WinRAR_x86, TempDir & "Splitty_WinRAR.7z")
            ElseIf architecture = 64 Then
                If Not IO.File.Exists(TempDir & "Splitty_WinRAR.exe") Then Load_Resource_To_Disk(My.Resources.Splitty_WinRAR_x64, TempDir & "Splitty_WinRAR.7z")
            End If
            Run_Process(TempDir & "Splitty_7zip.exe", "e " & """" & TempDir & "Splitty_WinRAR.7z" & """" & " -o" & """" & TempDir & """" & " -y", False, True)
        End If

        If CopyMode = "Copy" Or CopyMode = "Iso" Then
            If Not WantToCancelThread = True Then

                Set_Folder_Access(SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Deny)

                InvokeControl(ProgBarPlus, Sub(x) x.Value = 0)
                Get_All_Files(New IO.DirectoryInfo(SelectedDirectory))

                ' ProgressBar
                InvokeControl(ProgBarPlus, Sub(x) x.Max = TotalFilesNumber)
                InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "06"))
                InvokeControl(ProgBarPlus, Sub(x) x.TextShow = ProgBar.ProgBarPlus.eTextShow.FormatString)

                If CopyMode = "Iso" Then
                    Run_Process(TempDir & "Splitty_7zip.exe", "e " & """" & TempDir & "Splitty_PowerISO.7z" & """" & " -o" & """" & TempDir & """" & " -y", False, True)
                    Try
                        IO.File.Delete(TempDir & "Splitty_PowerISO.7z")
                        IO.File.Delete(TempDir & "Splitty_7zip.exe")
                    Catch ex As Exception
                    End Try
                End If

                ' Copy / Iso
                Dim folderNum As Integer = 1
                For Each file In FilesList
                    Application.DoEvents()
                    If Not WantToCancelThread = True Then
                        CachedSize += file.Split("|")(2)
                        If Not CachedSize > SelectedDiscBytes Then
                            Copy_File(file.Split("|")(0) & "\" & file.Split("|")(1), SelectedOutputDirectory & "\Disc " & folderNum & Get_File_Info(file.Split("|")(0) & "\" & file.Split("|")(1), DirectoryName).Split(":")(1), True, True)

                            Set_TaskBar_Value(ProgBarPlus.ValuePercent, 100)
                            InvokeControl(Me, Sub(x) x.Text = ProgBarPlus.ValuePercent & "% Splitty")

                        Else
                            If CopyMode = "Iso" Then
                                InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "07") & folderNum & " ...")
                                If IO.File.Exists(SelectedOutputDirectory & "\Disc " & folderNum & ".iso") Then Try : IO.File.Delete(SelectedOutputDirectory & "\Disc " & folderNum & ".iso") : Catch : End Try
                                While Not Run_Process(TempDir & "Splitty_Piso.exe", " -disable-optimization create -o " & """" & SelectedOutputDirectory & "\Disc " & folderNum & ".iso" & """" & " -add " & """" & SelectedOutputDirectory & "\Disc " & folderNum & """" & " /", False, True) = True
                                    If WantToCancelThread Then
                                        Exit While
                                    End If
                                End While
                                InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "06"))
                                Try : IO.Directory.Delete(SelectedOutputDirectory & "\Disc " & folderNum, True) : Catch : End Try
                            End If
                            CachedSize = Nothing
                            CachedSize += file.Split("|")(2)
                            folderNum += 1
                            Copy_File(file.Split("|")(0) & "\" & file.Split("|")(1), SelectedOutputDirectory & "\Disc " & folderNum & Get_File_Info(file.Split("|")(0) & "\" & file.Split("|")(1), DirectoryName).Split(":")(1), True, True)
                        End If
                        InvokeControl(ProgBarPlus, Sub(x) x.Value += 1)
                    Else
                        Exit For
                    End If
                Next
                If CopyMode = "Iso" Then
                    InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "07") & folderNum & " ...")
                    If IO.File.Exists(SelectedOutputDirectory & "\Disc " & folderNum & ".iso") Then Try : IO.File.Delete(SelectedOutputDirectory & "\Disc " & folderNum & ".iso") : Catch : End Try
                    While Not Run_Process(TempDir & "Splitty_Piso.exe", " -disable-optimization create -o " & """" & SelectedOutputDirectory & "\Disc " & folderNum & ".iso" & """" & " -add " & """" & SelectedOutputDirectory & "\Disc " & folderNum & """" & " /", False, True) = True
                        If WantToCancelThread Then
                            Exit While
                        End If
                    End While
                    Try : IO.Directory.Delete(SelectedOutputDirectory & "\Disc " & folderNum, True) : Catch : End Try
                End If
            End If
            InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = "")

        Else

            InvokeControl(ProgBarPlus, Sub(x) x.Max = Label_Discs_Value.Text.Split(" ")(0))
            If Label_Discs_Value.Text.Split(" ")(0) = 1 Then InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "08")) Else InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = My.Resources.ResourceManager.GetObject(LanguageResource & "27"))
            InvokeControl(ProgBarPlus, Sub(x) x.TextShow = ProgBar.ProgBarPlus.eTextShow.FormatString)

            ' Zip
            If CopyMode = "Zip" And Not WantToCancelThread = True Then
                ThreadProcess = New Threading.Thread(AddressOf Process_Thread)
                ThreadProcess.IsBackground = True
                ThreadProcess.Start("7zip")
                While Not ThreadProcessIsCompleted = True
                    If WantToCancelThread Then
                        Exit While
                    Else
                        Dim volumeCount As Integer = IO.Directory.GetFiles(SelectedOutputDirectory, "*.zip.*").Length
                        If volumeCount = 0 Then InvokeControl(ProgBarPlus, Sub(x) x.Value = 1) Else InvokeControl(ProgBarPlus, Sub(x) x.Value = volumeCount)
                        Set_TaskBar_Value(ProgBarPlus.ValuePercent, 100)
                        InvokeControl(Me, Sub(x) x.Text = ProgBarPlus.ValuePercent & "% Splitty")
                        Set_TaskBar_Value(ProgBarPlus.ValuePercent, 100)
                        InvokeControl(Me, Sub(x) x.Text = ProgBarPlus.ValuePercent & "% Splitty")
                    End If
                End While
            End If

            ' Rar
            If CopyMode = "Rar" And Not WantToCancelThread = True Then
                ThreadProcess = New Threading.Thread(AddressOf Process_Thread)
                ThreadProcess.IsBackground = True
                ThreadProcess.Start("Rar")
                While Not ThreadProcessIsCompleted = True
                    If WantToCancelThread Then
                        Exit While
                    Else
                        Dim volumeCount As Integer = IO.Directory.GetFiles(SelectedOutputDirectory, "*part*.rar").Length
                        If volumeCount = 0 Then InvokeControl(ProgBarPlus, Sub(x) x.Value = 1) Else InvokeControl(ProgBarPlus, Sub(x) x.Value = volumeCount)
                        Set_TaskBar_Value(ProgBarPlus.ValuePercent, 100)
                        InvokeControl(Me, Sub(x) x.Text = ProgBarPlus.ValuePercent & "% Splitty")
                    End If
                End While
            End If

            ' Sfx
            If CopyMode = "Sfx" And Not WantToCancelThread = True Then
                ThreadProcess = New Threading.Thread(AddressOf Process_Thread)
                ThreadProcess.IsBackground = True
                ThreadProcess.Start("Sfx")
                While Not ThreadProcessIsCompleted = True
                    If WantToCancelThread Then
                        Exit While
                    Else
                        Dim volumeCount As Integer = IO.Directory.GetFiles(SelectedOutputDirectory, "*part*.rar").Length
                        If volumeCount = 0 Then InvokeControl(ProgBarPlus, Sub(x) x.Value = 1) Else InvokeControl(ProgBarPlus, Sub(x) x.Value = volumeCount)
                        Set_TaskBar_Value(ProgBarPlus.ValuePercent, 100)
                        InvokeControl(Me, Sub(x) x.Text = ProgBarPlus.ValuePercent & "% Splitty")
                    End If
                End While
            End If
        End If

        ' Tar
        If CopyMode = "Tar" And Not WantToCancelThread = True Then
            ThreadProcess = New Threading.Thread(AddressOf Process_Thread)
            ThreadProcess.IsBackground = True
            ThreadProcess.Start("Tar")
            While Not ThreadProcessIsCompleted = True
                If WantToCancelThread Then
                    Exit While
                Else
                    Dim volumeCount As Integer = IO.Directory.GetFiles(SelectedOutputDirectory, "*.tar.*").Length
                    If volumeCount = 0 Then InvokeControl(ProgBarPlus, Sub(x) x.Value = 1) Else InvokeControl(ProgBarPlus, Sub(x) x.Value = volumeCount)
                    Set_TaskBar_Value(ProgBarPlus.ValuePercent, 100)
                    InvokeControl(Me, Sub(x) x.Text = ProgBarPlus.ValuePercent & "% Splitty")
                End If
            End While
        End If

        InvokeControl(ProgBarPlus, Sub(x) x.TextFormat = " ")
        InvokeControl(ProgBarPlus, Sub(x) x.Value = 0)

        ' Clean up
        Try
            IO.File.Delete(TempDir & "7z.dll")
            IO.File.Delete(TempDir & "Splitty_7zip.exe")
            IO.File.Delete(TempDir & "PowerISO.exe")
            IO.File.Delete(TempDir & "Splitty_Piso.exe")
            IO.File.Delete(TempDir & "Splitty_WinRAR.7z")
            IO.File.Delete(TempDir & "Splitty_WinRAR.exe")
            IO.File.Delete(TempDir & "rarreg.key")
            IO.File.Delete(TempDir & "Default.SFX")
        Catch : End Try

        If Not WantToCancelThread Then
            Set_TaskBar_Value(100, 100)
            InvokeControl(Me, Sub(x) x.Text = "100% Splitty")
        End If

        Set_Folder_Access(SelectedDirectory, FolderAccess.Create + FolderAccess.Write + FolderAccess.Delete, Action.Allow)
        ThreadProcessIsCompleted = False
        ThreadIsCompleted = True
    End Sub

#End Region

#Region " Process thread "

    Private Sub Process_Thread(ByVal process As String)
        If process = "7zip" Then
            Run_Process(TempDir & "Splitty_7zip.exe", " a " & """" & SelectedOutputDirectory & "\Disc.zip" & """" & " " & """" & SelectedDirectory & """" & " -v" & SelectedDiscBytes.ToString & "b " & " -mx=0 -bd -tzip", False, True)
        ElseIf process = "Rar" Then
            Run_Process(TempDir & "Splitty_WinRar.exe", " a " & """" & SelectedOutputDirectory & "\Disc.rar" & """" & " " & """" & SelectedDirectory & """" & " -v" & SelectedDiscBytes.ToString & "b " & " -m0  -ibck -o+", False, True)
        ElseIf process = "Sfx" Then
            Run_Process(TempDir & "Splitty_WinRar.exe", " a -sfx " & """" & SelectedOutputDirectory & "\Disc.exe" & """" & " " & """" & SelectedDirectory & """" & " -v" & SelectedDiscBytes.ToString & "b " & " -m0  -ibck -o+", False, True)
        ElseIf process = "Tar" Then
            Run_Process(TempDir & "Splitty_7zip.exe", " a " & """" & SelectedOutputDirectory & "\Disc.tar" & """" & " " & """" & SelectedDirectory & """" & " -v" & SelectedDiscBytes.ToString & "b " & " -mx=0 -bd -ttar", False, True)
        End If
        ThreadProcessIsCompleted = True
    End Sub

#End Region

#End Region


End Class