' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="ArchiveCommandBuilder.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " SplitMode "

Namespace Core

    ''' <summary>
    ''' Specifies how the source directory is split.
    ''' </summary>
    Public Enum SplitMode As Integer

        ''' <summary>
        ''' Files are copied to one folder per disc.
        ''' </summary>
        Copy = 0

        ''' <summary>
        ''' Files are stored in a multi-volume RAR archive (WinRAR).
        ''' </summary>
        Rar = 1

        ''' <summary>
        ''' Files are stored in a multi-volume ZIP archive (7-Zip).
        ''' </summary>
        Zip = 2

        ''' <summary>
        ''' Files are stored in a multi-volume self-extracting RAR archive (WinRAR).
        ''' </summary>
        Sfx = 3

        ''' <summary>
        ''' Files are copied to one folder per disc, and each folder is converted to an ISO image (PowerISO).
        ''' </summary>
        Iso = 4

        ''' <summary>
        ''' Files are stored in a multi-volume TAR archive (7-Zip).
        ''' </summary>
        Tar = 5

    End Enum

End Namespace

#End Region

#Region " ArchiveCommandBuilder "

Namespace Core

    ''' <summary>
    ''' Builds the command-line arguments passed to the external tools (7-Zip, WinRAR and PowerISO).
    ''' </summary>
    Public NotInheritable Class ArchiveCommandBuilder

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ArchiveCommandBuilder"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Determines whether the specified mode creates a multi-volume archive.
        ''' </summary>
        ''' <param name="mode">The split mode.</param>
        ''' <returns><see langword="True"/> for <see cref="SplitMode.Rar"/>, <see cref="SplitMode.Zip"/>,
        ''' <see cref="SplitMode.Sfx"/> and <see cref="SplitMode.Tar"/>; otherwise, <see langword="False"/>.</returns>
        Public Shared Function IsArchiveMode(mode As SplitMode) As Boolean

            Select Case mode
                Case SplitMode.Rar, SplitMode.Zip, SplitMode.Sfx, SplitMode.Tar
                    Return True
                Case SplitMode.Copy, SplitMode.Iso
                    Return False
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(mode), mode, $"Unsupported split mode: {CInt(mode)}")
            End Select

        End Function

        ''' <summary>
        ''' Gets the search pattern that matches the volumes created by the specified archive mode.
        ''' </summary>
        ''' <param name="mode">The split mode. It must be an archive mode.</param>
        ''' <returns>The file search pattern.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="mode"/> is not an archive mode.
        ''' </exception>
        Public Shared Function GetVolumeSearchPattern(mode As SplitMode) As String

            Select Case mode
                Case SplitMode.Zip : Return "*.zip.*"
                Case SplitMode.Tar : Return "*.tar.*"
                Case SplitMode.Rar, SplitMode.Sfx : Return "*part*.rar"
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(mode), mode, $"Not an archive split mode: {mode}")
            End Select

        End Function

        ''' <summary>
        ''' Builds the arguments to create a multi-volume archive of the source directory.
        ''' </summary>
        ''' <param name="mode">The split mode. It must be an archive mode.</param>
        ''' <param name="sourceDirectory">The directory to archive.</param>
        ''' <param name="outputDirectory">The directory where the volumes are created.</param>
        ''' <param name="volumeSizeBytes">The size of each volume, in bytes.</param>
        ''' <returns>The command-line arguments for 7-Zip (ZIP and TAR) or WinRAR (RAR and SFX).</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="sourceDirectory"/> or <paramref name="outputDirectory"/> is null or empty.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="mode"/> is not an archive mode,
        ''' or <paramref name="volumeSizeBytes"/> is lower than or equal to zero.
        ''' </exception>
        Public Shared Function BuildArchiveArguments(mode As SplitMode, sourceDirectory As String,
                                                     outputDirectory As String, volumeSizeBytes As Long) As String

            ArchiveCommandBuilder.ValidatePath(sourceDirectory, NameOf(sourceDirectory))
            ArchiveCommandBuilder.ValidatePath(outputDirectory, NameOf(outputDirectory))
            If volumeSizeBytes <= 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(volumeSizeBytes), volumeSizeBytes, "Value must be greater than zero.")
            End If

            Dim source As String = SplitPaths.TrimTrailingSeparators(sourceDirectory)
            Dim output As String = outputDirectory.TrimEnd("\"c)
            Dim volume As String = $"-v{volumeSizeBytes}b"

            Select Case mode
                Case SplitMode.Zip
                    Return $" a ""{output}\Disc.zip"" ""{source}"" {volume}  -mx=0 -bd -tzip"
                Case SplitMode.Tar
                    Return $" a ""{output}\Disc.tar"" ""{source}"" {volume}  -mx=0 -bd -ttar"
                Case SplitMode.Rar
                    Return $" a ""{output}\Disc.rar"" ""{source}"" {volume}  -m0  -ibck -o+"
                Case SplitMode.Sfx
                    Return $" a -sfx ""{output}\Disc.exe"" ""{source}"" {volume}  -m0  -ibck -o+"
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(mode), mode, $"Not an archive split mode: {mode}")
            End Select

        End Function

        ''' <summary>
        ''' Builds the 7-Zip arguments to extract an archive to a directory, overwriting existing files.
        ''' </summary>
        ''' <param name="archiveFile">The archive to extract.</param>
        ''' <param name="targetDirectory">The extraction directory.</param>
        ''' <returns>The command-line arguments for 7-Zip.</returns>
        ''' <remarks>
        ''' Trailing separators of <paramref name="targetDirectory"/> are removed,
        ''' because a backslash followed by a quote is parsed as an escaped quote by most command-line parsers.
        ''' </remarks>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="archiveFile"/> or <paramref name="targetDirectory"/> is null or empty.
        ''' </exception>
        Public Shared Function BuildExtractArguments(archiveFile As String, targetDirectory As String) As String

            ArchiveCommandBuilder.ValidatePath(archiveFile, NameOf(archiveFile))
            ArchiveCommandBuilder.ValidatePath(targetDirectory, NameOf(targetDirectory))

            Dim target As String = SplitPaths.TrimTrailingSeparators(targetDirectory)
            Return $"e ""{archiveFile}"" -o""{target}"" -y"

        End Function

        ''' <summary>
        ''' Builds the PowerISO arguments to create the ISO image of a disc folder.
        ''' </summary>
        ''' <param name="outputDirectory">The output directory selected by the user.</param>
        ''' <param name="discNumber">The one-based disc number.</param>
        ''' <returns>The command-line arguments for PowerISO.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="outputDirectory"/> is null or empty.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="discNumber"/> is lower than 1.
        ''' </exception>
        Public Shared Function BuildIsoArguments(outputDirectory As String, discNumber As Integer) As String

            Dim isoFile As String = SplitPaths.GetDiscIsoFile(outputDirectory, discNumber)
            Dim discDirectory As String = SplitPaths.GetDiscDirectory(outputDirectory, discNumber)
            Return $" -disable-optimization create -o ""{isoFile}"" -add ""{discDirectory}"" /"

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Validates that a path argument is not null or empty.
        ''' </summary>
        ''' <param name="value">The path.</param>
        ''' <param name="paramName">The parameter name.</param>
        Private Shared Sub ValidatePath(value As String, paramName As String)

            If String.IsNullOrEmpty(value) Then
                Throw New ArgumentException("Value cannot be null or empty.", paramName)
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
