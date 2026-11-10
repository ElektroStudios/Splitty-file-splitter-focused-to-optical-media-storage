' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="DirectoryScanner.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Collections.Generic
Imports System.IO
Imports System.Threading

#End Region

#Region " SourceFileEntry "

Namespace Core

    ''' <summary>
    ''' Represents a file of the source directory that has to be distributed among discs.
    ''' </summary>
    Public NotInheritable Class SourceFileEntry

#Region " Properties "

        ''' <summary>
        ''' Gets the full path of the directory that contains the file.
        ''' </summary>
        Public ReadOnly Property DirectoryPath As String

        ''' <summary>
        ''' Gets the file name, including its extension.
        ''' </summary>
        Public ReadOnly Property FileName As String

        ''' <summary>
        ''' Gets the file size, in bytes.
        ''' </summary>
        Public ReadOnly Property Length As Long

        ''' <summary>
        ''' Gets the full path of the file.
        ''' </summary>
        Public ReadOnly Property FullPath As String
            Get
                Return $"{Me.DirectoryPath.TrimEnd("\"c)}\{Me.FileName}"
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SourceFileEntry"/> class.
        ''' </summary>
        ''' <param name="directoryPath">The full path of the directory that contains the file.</param>
        ''' <param name="fileName">The file name, including its extension.</param>
        ''' <param name="length">The file size, in bytes.</param>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="directoryPath"/> or <paramref name="fileName"/> is null or empty.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="length"/> is negative.
        ''' </exception>
        Public Sub New(directoryPath As String, fileName As String, length As Long)

            If String.IsNullOrEmpty(directoryPath) Then
                Throw New ArgumentException("Value cannot be null or empty.", NameOf(directoryPath))
            End If
            If String.IsNullOrEmpty(fileName) Then
                Throw New ArgumentException("Value cannot be null or empty.", NameOf(fileName))
            End If
            If length < 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(length), length, "Value cannot be negative.")
            End If

            Me.DirectoryPath = directoryPath
            Me.FileName = fileName
            Me.Length = length

        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Returns a string that represents this instance.
        ''' </summary>
        ''' <returns>The full path and the size of the file.</returns>
        Public Overrides Function ToString() As String

            Return $"{Me.FullPath} ({Me.Length} bytes)"

        End Function

#End Region

    End Class

End Namespace

#End Region

#Region " DirectoryScanner "

Namespace Core

    ''' <summary>
    ''' Enumerates the files of a directory tree, iteratively (without recursion).
    ''' </summary>
    ''' <remarks>
    ''' Subdirectories that have both <see cref="FileAttributes.Hidden"/> and <see cref="FileAttributes.System"/>
    ''' attributes (for example, "System Volume Information" or "$RECYCLE.BIN" of a drive root) are skipped,
    ''' because they belong to the operating system and are not readable by the user.
    ''' The root directory itself is never skipped (drive roots have both attributes).
    ''' </remarks>
    Public NotInheritable Class DirectoryScanner

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="DirectoryScanner"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets all the files of the specified directory and its subdirectories.
        ''' <para></para>
        ''' The order is a pre-order, depth-first traversal: the files of a directory come first,
        ''' followed by the files of each of its subdirectories (in the order returned by the file system).
        ''' </summary>
        ''' <param name="directoryPath">The full path of the root directory.</param>
        ''' <returns>The list of files found.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="directoryPath"/> is null or empty.
        ''' </exception>
        ''' <exception cref="DirectoryNotFoundException">
        ''' Thrown when <paramref name="directoryPath"/> does not exist.
        ''' </exception>
        Public Shared Function GetFiles(directoryPath As String) As List(Of SourceFileEntry)

            Dim root As DirectoryInfo = DirectoryScanner.GetExistingDirectory(directoryPath)
            Dim result As New List(Of SourceFileEntry)()

            Dim pending As New Stack(Of DirectoryInfo)()
            pending.Push(root)

            While pending.Count > 0
                Dim current As DirectoryInfo = pending.Pop()

                For Each file As FileInfo In current.GetFiles()
                    Thread.CurrentThread.Join(0)
                    result.Add(New SourceFileEntry(current.FullName, file.Name, file.Length))
                Next

                ' Pushed in reverse order so they are popped in the file system order.
                Dim subDirectories As DirectoryInfo() = current.GetDirectories()
                For index As Integer = subDirectories.Length - 1 To 0 Step -1
                    Thread.CurrentThread.Join(0)
                    If Not DirectoryScanner.IsProtectedSystemDirectory(subDirectories(index)) Then
                        pending.Push(subDirectories(index))
                    End If
                Next
            End While

            Return result

        End Function

        ''' <summary>
        ''' Gets the total size, in bytes, of the files of the specified directory and its subdirectories.
        ''' </summary>
        ''' <param name="directoryPath">The full path of the root directory.</param>
        ''' <returns>The total size in bytes.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="directoryPath"/> is null or empty.
        ''' </exception>
        ''' <exception cref="DirectoryNotFoundException">
        ''' Thrown when <paramref name="directoryPath"/> does not exist.
        ''' </exception>
        Public Shared Function GetDirectorySize(directoryPath As String) As Long

            Dim root As DirectoryInfo = DirectoryScanner.GetExistingDirectory(directoryPath)
            Dim total As Long = 0L

            Dim pending As New Stack(Of DirectoryInfo)()
            pending.Push(root)

            While pending.Count > 0
                Dim current As DirectoryInfo = pending.Pop()

                For Each file As FileInfo In current.EnumerateFiles()
                    Thread.CurrentThread.Join(0)
                    total += file.Length
                Next

                For Each subDirectory As DirectoryInfo In current.EnumerateDirectories()
                    Thread.CurrentThread.Join(0)
                    If Not DirectoryScanner.IsProtectedSystemDirectory(subDirectory) Then
                        pending.Push(subDirectory)
                    End If
                Next
            End While

            Return total

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Determines whether a directory is a protected operating system directory (hidden and system).
        ''' </summary>
        ''' <param name="directory">The directory.</param>
        ''' <returns><see langword="True"/> if the directory must be skipped; otherwise, <see langword="False"/>.</returns>
        Friend Shared Function IsProtectedSystemDirectory(directory As DirectoryInfo) As Boolean

            Const ProtectedAttributes As FileAttributes = FileAttributes.Hidden Or FileAttributes.System
            Return (directory.Attributes And ProtectedAttributes) = ProtectedAttributes

        End Function

        ''' <summary>
        ''' Validates the specified directory path and returns its <see cref="DirectoryInfo"/>.
        ''' </summary>
        ''' <param name="directoryPath">The directory path.</param>
        ''' <returns>The <see cref="DirectoryInfo"/> of the existing directory.</returns>
        Private Shared Function GetExistingDirectory(directoryPath As String) As DirectoryInfo

            If String.IsNullOrEmpty(directoryPath) Then
                Throw New ArgumentException("Value cannot be null or empty.", NameOf(directoryPath))
            End If

            Dim directory As New DirectoryInfo(directoryPath)
            If Not directory.Exists Then
                Throw New DirectoryNotFoundException($"Directory not found: {directoryPath}")
            End If
            Return directory

        End Function

#End Region

    End Class

End Namespace

#End Region
