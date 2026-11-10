' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="SplitPaths.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " SplitPaths "

Namespace Core

    ''' <summary>
    ''' Builds and validates the Windows paths used by the split operation.
    ''' </summary>
    ''' <remarks>
    ''' All the methods work with Windows path syntax (backslash separators, drive letters and UNC paths)
    ''' through plain string manipulation, so their results are deterministic and do not depend on the file system.
    ''' </remarks>
    Public NotInheritable Class SplitPaths

#Region " Constants "

        ''' <summary>
        ''' The Windows directory separator.
        ''' </summary>
        Private Const Separator As Char = "\"c

        ''' <summary>
        ''' The alternative directory separator accepted in input paths.
        ''' </summary>
        Private Const AltSeparator As Char = "/"c

        ''' <summary>
        ''' The name prefix of the per-disc output folders and ISO images.
        ''' </summary>
        Public Const DiscNamePrefix As String = "Disc "

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="SplitPaths"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the root part of a Windows path:
        ''' a drive ("C:\"), a UNC share ("\\server\share\") or a long path prefix ("\\?\C:\", "\\?\UNC\server\share\").
        ''' </summary>
        ''' <param name="path">The full path.</param>
        ''' <returns>The root, always ending with a backslash.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="path"/> is null or empty, or is not a rooted Windows path.
        ''' </exception>
        Public Shared Function GetRoot(path As String) As String

            Dim normalized As String = SplitPaths.Normalize(path)
            Dim rootLength As Integer = SplitPaths.GetRootLength(normalized)
            If rootLength <= 0 Then
                Throw New ArgumentException($"The path is not a rooted Windows path: {path}", NameOf(path))
            End If

            Dim root As String = normalized.Substring(0, rootLength)
            Return If(root.EndsWith(Separator), root, $"{root}{Separator}")

        End Function

        ''' <summary>
        ''' Gets the part of a Windows path that follows its root.
        ''' <para></para>
        ''' For example, "C:\Movies\Action" returns "Movies\Action", and "\\nas\share\Movies" returns "Movies".
        ''' </summary>
        ''' <param name="path">The full path.</param>
        ''' <returns>The path without its root and without leading or trailing separators. Empty for a root path.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="path"/> is null or empty, or is not a rooted Windows path.
        ''' </exception>
        Public Shared Function GetPathWithoutRoot(path As String) As String

            Dim normalized As String = SplitPaths.Normalize(path)
            Dim rootLength As Integer = SplitPaths.GetRootLength(normalized)
            If rootLength <= 0 Then
                Throw New ArgumentException($"The path is not a rooted Windows path: {path}", NameOf(path))
            End If

            Return normalized.Substring(rootLength).Trim(Separator)

        End Function

        ''' <summary>
        ''' Gets the output folder of the specified disc, for example "D:\Output\Disc 1".
        ''' </summary>
        ''' <param name="outputDirectory">The output directory selected by the user.</param>
        ''' <param name="discNumber">The one-based disc number.</param>
        ''' <returns>The full path of the disc folder.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="outputDirectory"/> is null or empty.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="discNumber"/> is lower than 1.
        ''' </exception>
        Public Shared Function GetDiscDirectory(outputDirectory As String, discNumber As Integer) As String

            SplitPaths.ValidateDiscArguments(outputDirectory, discNumber)
            Dim baseDirectory As String = SplitPaths.Normalize(outputDirectory).TrimEnd(Separator)
            Return $"{baseDirectory}{Separator}{DiscNamePrefix}{discNumber}"

        End Function

        ''' <summary>
        ''' Gets the ISO image file of the specified disc, for example "D:\Output\Disc 1.iso".
        ''' </summary>
        ''' <param name="outputDirectory">The output directory selected by the user.</param>
        ''' <param name="discNumber">The one-based disc number.</param>
        ''' <returns>The full path of the ISO image file.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="outputDirectory"/> is null or empty.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="discNumber"/> is lower than 1.
        ''' </exception>
        Public Shared Function GetDiscIsoFile(outputDirectory As String, discNumber As Integer) As String

            Return $"{SplitPaths.GetDiscDirectory(outputDirectory, discNumber)}.iso"

        End Function

        ''' <summary>
        ''' Gets the destination directory of a source file inside the folder of its disc.
        ''' <para></para>
        ''' The full source directory structure (without its root) is replicated inside the disc folder,
        ''' for example "C:\Movies\Action" with disc 2 and output "D:\Output" returns "D:\Output\Disc 2\Movies\Action".
        ''' </summary>
        ''' <param name="outputDirectory">The output directory selected by the user.</param>
        ''' <param name="discNumber">The one-based disc number.</param>
        ''' <param name="sourceFileDirectory">The full path of the directory that contains the source file.</param>
        ''' <returns>The full path of the destination directory.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="outputDirectory"/> is null or empty,
        ''' or <paramref name="sourceFileDirectory"/> is not a rooted Windows path.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="discNumber"/> is lower than 1.
        ''' </exception>
        Public Shared Function GetDestinationDirectory(outputDirectory As String, discNumber As Integer,
                                                       sourceFileDirectory As String) As String

            Dim discDirectory As String = SplitPaths.GetDiscDirectory(outputDirectory, discNumber)
            Dim relative As String = SplitPaths.GetPathWithoutRoot(sourceFileDirectory)
            Return If(relative.Length = 0, discDirectory, $"{discDirectory}{Separator}{relative}")

        End Function

        ''' <summary>
        ''' Determines whether two Windows paths refer to the same directory (case-insensitive).
        ''' </summary>
        ''' <param name="pathA">The first path.</param>
        ''' <param name="pathB">The second path.</param>
        ''' <returns><see langword="True"/> if both paths are equal; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="pathA"/> or <paramref name="pathB"/> is null or empty.
        ''' </exception>
        Public Shared Function AreSameDirectory(pathA As String, pathB As String) As Boolean

            Dim a As String = SplitPaths.TrimTrailingSeparators(SplitPaths.Normalize(pathA))
            Dim b As String = SplitPaths.TrimTrailingSeparators(SplitPaths.Normalize(pathB))
            Return String.Equals(a, b, StringComparison.OrdinalIgnoreCase)

        End Function

        ''' <summary>
        ''' Determines whether a Windows path is the same as, or is located inside, another directory (case-insensitive).
        ''' </summary>
        ''' <param name="path">The path to check.</param>
        ''' <param name="parentDirectory">The possible parent directory.</param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="path"/> equals <paramref name="parentDirectory"/>
        ''' or is one of its descendants; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="path"/> or <paramref name="parentDirectory"/> is null or empty.
        ''' </exception>
        Public Shared Function IsSameOrInside(path As String, parentDirectory As String) As Boolean

            Dim child As String = SplitPaths.TrimTrailingSeparators(SplitPaths.Normalize(path))
            Dim parent As String = SplitPaths.TrimTrailingSeparators(SplitPaths.Normalize(parentDirectory))

            If String.Equals(child, parent, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
            Dim parentPrefix As String = If(parent.EndsWith(Separator), parent, $"{parent}{Separator}")
            Return child.StartsWith(parentPrefix, StringComparison.OrdinalIgnoreCase)

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Validates a path argument and converts its alternative separators to backslashes.
        ''' </summary>
        ''' <param name="path">The path.</param>
        ''' <returns>The normalized path.</returns>
        Private Shared Function Normalize(path As String) As String

            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentException("Value cannot be null or empty.", NameOf(path))
            End If
            Return path.Trim().Replace(AltSeparator, Separator)

        End Function

        ''' <summary>
        ''' Removes the trailing separators of a path, keeping the separator of a drive root ("C:\").
        ''' </summary>
        ''' <param name="path">The path.</param>
        ''' <returns>The path without trailing separators.</returns>
        Friend Shared Function TrimTrailingSeparators(path As String) As String

            Dim trimmed As String = path.TrimEnd(Separator)
            ' "C:\" must not become "C:" (that means "current directory of drive C").
            If trimmed.Length = 2 AndAlso trimmed(1) = ":"c Then
                Return $"{trimmed}{Separator}"
            End If
            Return trimmed

        End Function

        ''' <summary>
        ''' Gets the length of the root part of a normalized Windows path.
        ''' </summary>
        ''' <param name="path">The normalized path.</param>
        ''' <returns>The root length, or zero if the path is not rooted.</returns>
        Private Shared Function GetRootLength(path As String) As Integer

            Const LongPathPrefix As String = "\\?\"
            Const LongUncPrefix As String = "\\?\UNC\"

            If path.StartsWith(LongUncPrefix, StringComparison.OrdinalIgnoreCase) Then
                Return SplitPaths.GetUncRootLength(path, LongUncPrefix.Length)
            End If

            If path.StartsWith(LongPathPrefix, StringComparison.Ordinal) Then
                Dim driveRoot As Integer = SplitPaths.GetDriveRootLength(path, LongPathPrefix.Length)
                Return If(driveRoot > 0, LongPathPrefix.Length + driveRoot, 0)
            End If

            If path.StartsWith("\\", StringComparison.Ordinal) Then
                Return SplitPaths.GetUncRootLength(path, 2)
            End If

            Return SplitPaths.GetDriveRootLength(path, 0)

        End Function

        ''' <summary>
        ''' Gets the length of a "X:\" drive root that starts at the specified index.
        ''' </summary>
        ''' <param name="path">The normalized path.</param>
        ''' <param name="startIndex">The index where the drive letter is expected.</param>
        ''' <returns>The drive root length (2 or 3), or zero if there is no drive root.</returns>
        Private Shared Function GetDriveRootLength(path As String, startIndex As Integer) As Integer

            If path.Length < startIndex + 2 Then
                Return 0
            End If
            If Not Char.IsLetter(path(startIndex)) OrElse path(startIndex + 1) <> ":"c Then
                Return 0
            End If
            If path.Length > startIndex + 2 AndAlso path(startIndex + 2) = Separator Then
                Return 3
            End If
            Return 2

        End Function

        ''' <summary>
        ''' Gets the length of a "\\server\share" UNC root, where the server name starts at the specified index.
        ''' </summary>
        ''' <param name="path">The normalized path.</param>
        ''' <param name="serverIndex">The index where the server name starts.</param>
        ''' <returns>The UNC root length (including its trailing separator, if any), or zero if it is not a valid UNC root.</returns>
        Private Shared Function GetUncRootLength(path As String, serverIndex As Integer) As Integer

            Dim shareSeparator As Integer = path.IndexOf(Separator, serverIndex)
            If shareSeparator <= serverIndex Then
                Return 0 ' No server name, or no share name.
            End If

            Dim shareEnd As Integer = path.IndexOf(Separator, shareSeparator + 1)
            If shareEnd < 0 Then
                Return If(shareSeparator + 1 < path.Length, path.Length, 0)
            End If
            If shareEnd = shareSeparator + 1 Then
                Return 0 ' Empty share name.
            End If
            Return shareEnd + 1

        End Function

        ''' <summary>
        ''' Validates the arguments used to build the paths of a disc.
        ''' </summary>
        ''' <param name="outputDirectory">The output directory.</param>
        ''' <param name="discNumber">The one-based disc number.</param>
        Private Shared Sub ValidateDiscArguments(outputDirectory As String, discNumber As Integer)

            If String.IsNullOrEmpty(outputDirectory) Then
                Throw New ArgumentException("Value cannot be null or empty.", NameOf(outputDirectory))
            End If
            If discNumber < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(discNumber), discNumber, "Value must be greater than zero.")
            End If

        End Sub

#End Region

    End Class

End Namespace

#End Region
