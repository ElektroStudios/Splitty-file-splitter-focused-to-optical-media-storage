' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="DirectoryScannerTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Collections.Generic
Imports System.IO

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splitty.Core

#End Region

#Region " DirectoryScannerTests "

''' <summary>
''' Unit tests for <see cref="DirectoryScanner"/> and <see cref="SourceFileEntry"/>.
''' </summary>
<TestClass>
Public NotInheritable Class DirectoryScannerTests

#Region " Fields "

    ''' <summary>
    ''' The temporary directory used by the current test.
    ''' </summary>
    Private testDirectory As String

#End Region

#Region " Setup / Cleanup "

    ''' <summary>
    ''' Creates an empty temporary directory for the current test.
    ''' </summary>
    <TestInitialize>
    Public Sub TestInitialize()

        Me.testDirectory = Path.Combine(Path.GetTempPath(), $"Splitty.Tests.{Guid.NewGuid():N}")
        Directory.CreateDirectory(Me.testDirectory)

    End Sub

    ''' <summary>
    ''' Deletes the temporary directory of the current test.
    ''' </summary>
    <TestCleanup>
    Public Sub TestCleanup()

        If Directory.Exists(Me.testDirectory) Then
            Directory.Delete(Me.testDirectory, recursive:=True)
        End If

    End Sub

#End Region

#Region " GetFiles "

    ''' <summary>
    ''' Verifies that an empty directory returns no files.
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_EmptyDirectory_ReturnsEmptyList()

        Assert.IsEmpty(DirectoryScanner.GetFiles(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that files of all the subdirectories are returned with their sizes and containing directories.
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_NestedDirectories_ReturnsAllFilesWithSizes()

        Me.CreateFile("root.bin", 10)
        Me.CreateFile(Path.Combine("A", "a.bin"), 20)
        Me.CreateFile(Path.Combine("A", "B", "b.bin"), 30)
        Me.CreateFile(Path.Combine("C", "c.bin"), 0)

        Dim files As List(Of SourceFileEntry) = DirectoryScanner.GetFiles(Me.testDirectory)

        Assert.HasCount(4, files)
        Dim byName As New Dictionary(Of String, SourceFileEntry)()
        For Each entry As SourceFileEntry In files
            byName.Add(entry.FileName, entry)
        Next

        Assert.AreEqual(10L, byName("root.bin").Length)
        Assert.AreEqual(20L, byName("a.bin").Length)
        Assert.AreEqual(30L, byName("b.bin").Length)
        Assert.AreEqual(0L, byName("c.bin").Length)

        Assert.AreEqual(Me.FullPathOf(""), byName("root.bin").DirectoryPath)
        Assert.AreEqual(Me.FullPathOf("A"), byName("a.bin").DirectoryPath)
        Assert.AreEqual(Me.FullPathOf(Path.Combine("A", "B")), byName("b.bin").DirectoryPath)
        Assert.AreEqual(Me.FullPathOf("C"), byName("c.bin").DirectoryPath)

    End Sub

    ''' <summary>
    ''' Verifies the pre-order, depth-first order: the files of a directory come before the files of its subdirectories,
    ''' and a whole subtree is listed before its next sibling.
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_ReturnsPreOrderDepthFirst()

        Me.CreateFile(Path.Combine("A", "B", "ab.bin"), 1)
        Me.CreateFile(Path.Combine("A", "a.bin"), 1)
        Me.CreateFile(Path.Combine("C", "c.bin"), 1)
        Me.CreateFile("root.bin", 1)

        Dim files As List(Of SourceFileEntry) = DirectoryScanner.GetFiles(Me.testDirectory)
        Dim names As New List(Of String)()
        For Each entry As SourceFileEntry In files
            names.Add(entry.FileName)
        Next

        Assert.AreEqual("root.bin", names(0), "Files of the root directory must come first.")
        Assert.IsLessThan(names.IndexOf("ab.bin"), names.IndexOf("a.bin"), "Files of A must come before files of A\B.")

        ' The whole A subtree must be contiguous (it is either before or after C, never interleaved).
        Dim indexA As Integer = names.IndexOf("a.bin")
        Dim indexAB As Integer = names.IndexOf("ab.bin")
        Dim indexC As Integer = names.IndexOf("c.bin")
        Assert.IsTrue((indexC < indexA) OrElse (indexC > indexAB), "The A subtree must not be interleaved with C.")

    End Sub

    ''' <summary>
    ''' Verifies that a deep directory tree is supported (the scan is iterative, not recursive).
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_DeepTree_ReturnsAllFiles()

        Dim relative As String = String.Empty
        For level As Integer = 1 To 20
            relative = Path.Combine(relative, "d")
            Me.CreateFile(Path.Combine(relative, $"f{level}.bin"), level)
        Next

        Dim files As List(Of SourceFileEntry) = DirectoryScanner.GetFiles(Me.testDirectory)
        Assert.HasCount(20, files)
        Assert.AreEqual("f1.bin", files(0).FileName)
        Assert.AreEqual("f20.bin", files(19).FileName)

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() DirectoryScanner.GetFiles(Nothing))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() DirectoryScanner.GetFiles(String.Empty))
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() DirectoryScanner.GetFiles(Path.Combine(Me.testDirectory, "missing")))

    End Sub

#End Region

#Region " GetDirectorySize "

    ''' <summary>
    ''' Verifies the total size of a directory tree.
    ''' </summary>
    <TestMethod>
    Public Sub GetDirectorySize_NestedDirectories_ReturnsSum()

        Me.CreateFile("root.bin", 100)
        Me.CreateFile(Path.Combine("A", "a.bin"), 200)
        Me.CreateFile(Path.Combine("A", "B", "b.bin"), 300)
        Directory.CreateDirectory(Path.Combine(Me.testDirectory, "Empty"))

        Assert.AreEqual(600L, DirectoryScanner.GetDirectorySize(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that the directory size equals the sum of the files returned by <see cref="DirectoryScanner.GetFiles"/>.
    ''' </summary>
    <TestMethod>
    Public Sub GetDirectorySize_MatchesSumOfGetFiles()

        For index As Integer = 1 To 15
            Me.CreateFile(Path.Combine($"dir{index Mod 4}", $"file{index}.bin"), index * 7)
        Next

        Dim sum As Long = 0L
        For Each entry As SourceFileEntry In DirectoryScanner.GetFiles(Me.testDirectory)
            sum += entry.Length
        Next

        Assert.AreEqual(sum, DirectoryScanner.GetDirectorySize(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that an empty directory has zero size.
    ''' </summary>
    <TestMethod>
    Public Sub GetDirectorySize_EmptyDirectory_ReturnsZero()

        Assert.AreEqual(0L, DirectoryScanner.GetDirectorySize(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that a missing directory throws, instead of silently returning zero.
    ''' </summary>
    <TestMethod>
    Public Sub GetDirectorySize_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() DirectoryScanner.GetDirectorySize(Nothing))
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() DirectoryScanner.GetDirectorySize(Path.Combine(Me.testDirectory, "missing")))

    End Sub

    ''' <summary>
    ''' Verifies that hidden + system subdirectories (like "System Volume Information") are skipped,
    ''' while only-hidden or only-system subdirectories are included.
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_HiddenSystemSubdirectory_IsSkipped()

        If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
            Assert.Inconclusive("The System file attribute is only supported on Windows.")
        End If

        Me.CreateFile(Path.Combine("Protected", "p.bin"), 5)
        Me.CreateFile(Path.Combine("HiddenOnly", "h.bin"), 6)
        Me.CreateFile(Path.Combine("SystemOnly", "s.bin"), 7)
        Me.CreateFile("root.bin", 8)

        Dim protectedDirectory As New DirectoryInfo(Path.Combine(Me.testDirectory, "Protected"))
        Dim hiddenDirectory As New DirectoryInfo(Path.Combine(Me.testDirectory, "HiddenOnly"))
        Dim systemDirectory As New DirectoryInfo(Path.Combine(Me.testDirectory, "SystemOnly"))
        protectedDirectory.Attributes = protectedDirectory.Attributes Or FileAttributes.Hidden Or FileAttributes.System
        hiddenDirectory.Attributes = hiddenDirectory.Attributes Or FileAttributes.Hidden
        systemDirectory.Attributes = systemDirectory.Attributes Or FileAttributes.System
        Try
            Assert.HasCount(3, DirectoryScanner.GetFiles(Me.testDirectory))
            Assert.AreEqual(21L, DirectoryScanner.GetDirectorySize(Me.testDirectory))
        Finally
            protectedDirectory.Attributes = FileAttributes.Directory
            hiddenDirectory.Attributes = FileAttributes.Directory
            systemDirectory.Attributes = FileAttributes.Directory
        End Try

    End Sub

    ''' <summary>
    ''' Verifies that the root directory is scanned even if it is hidden and system (like a drive root).
    ''' </summary>
    <TestMethod>
    Public Sub GetFiles_HiddenSystemRoot_IsScanned()

        If Environment.OSVersion.Platform <> PlatformID.Win32NT Then
            Assert.Inconclusive("The System file attribute is only supported on Windows.")
        End If

        Me.CreateFile("root.bin", 3)
        Dim root As New DirectoryInfo(Me.testDirectory)
        root.Attributes = root.Attributes Or FileAttributes.Hidden Or FileAttributes.System
        Try
            Assert.HasCount(1, DirectoryScanner.GetFiles(Me.testDirectory))
            Assert.AreEqual(3L, DirectoryScanner.GetDirectorySize(Me.testDirectory))
        Finally
            root.Attributes = FileAttributes.Directory
        End Try

    End Sub

#End Region

#Region " SourceFileEntry "

    ''' <summary>
    ''' Verifies the properties of <see cref="SourceFileEntry"/>.
    ''' </summary>
    <TestMethod>
    Public Sub SourceFileEntry_Properties()

        Dim entry As New SourceFileEntry("C:\Movies\", "film.mkv", 42L)
        Assert.AreEqual("C:\Movies\", entry.DirectoryPath)
        Assert.AreEqual("film.mkv", entry.FileName)
        Assert.AreEqual(42L, entry.Length)
        Assert.AreEqual("C:\Movies\film.mkv", entry.FullPath)
        Assert.AreEqual("C:\Movies\film.mkv (42 bytes)", entry.ToString())

        Assert.AreEqual("C:\film.mkv", New SourceFileEntry("C:\", "film.mkv", 1L).FullPath)

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub SourceFileEntry_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(
            Sub()
                Dim unused As New SourceFileEntry(Nothing, "a", 1L)
            End Sub)
        Assert.ThrowsExactly(Of ArgumentException)(
            Sub()
                Dim unused As New SourceFileEntry("C:\", "", 1L)
            End Sub)
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
            Sub()
                Dim unused As New SourceFileEntry("C:\", "a", -1L)
            End Sub)

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Creates a file of the specified size inside the test directory, creating its parent directories.
    ''' </summary>
    ''' <param name="relativePath">The path of the file, relative to the test directory.</param>
    ''' <param name="size">The file size in bytes.</param>
    Private Sub CreateFile(relativePath As String, size As Integer)

        Dim fullPath As String = Path.Combine(Me.testDirectory, relativePath)
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath))
        File.WriteAllBytes(fullPath, New Byte(size - 1) {})

    End Sub

    ''' <summary>
    ''' Gets the full path of a directory relative to the test directory, as reported by <see cref="DirectoryInfo.FullName"/>.
    ''' </summary>
    ''' <param name="relativePath">The relative path, or an empty string for the test directory itself.</param>
    ''' <returns>The full path.</returns>
    Private Function FullPathOf(relativePath As String) As String

        Return New DirectoryInfo(Path.Combine(Me.testDirectory, relativePath)).FullName.TrimEnd(Path.DirectorySeparatorChar)

    End Function

#End Region

End Class

#End Region
