' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="FolderLockTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.IO
Imports System.Security.AccessControl

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splitty.Core

#End Region

#Region " FolderLockTests "

''' <summary>
''' Unit tests for <see cref="FolderLock"/>.
''' </summary>
<TestClass>
Public NotInheritable Class FolderLockTests

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
    ''' Unlocks and deletes the temporary directory of the current test.
    ''' </summary>
    <TestCleanup>
    Public Sub TestCleanup()

        If Not Directory.Exists(Me.testDirectory) Then
            Return
        End If
        If FolderLockTests.IsWindows() Then
            FolderLock.Unlock(Me.testDirectory)
        End If
        Directory.Delete(Me.testDirectory, recursive:=True)

    End Sub

#End Region

#Region " DeniedRights "

    ''' <summary>
    ''' Verifies that the denied rights block creating, writing and deleting.
    ''' </summary>
    <TestMethod>
    Public Sub DeniedRights_ContainsWriteAndDeleteRights()

        Dim rights As FileSystemRights = FolderLock.DeniedRights
        Assert.AreEqual(FileSystemRights.CreateFiles, rights And FileSystemRights.CreateFiles)
        Assert.AreEqual(FileSystemRights.CreateDirectories, rights And FileSystemRights.CreateDirectories)
        Assert.AreEqual(FileSystemRights.WriteData, rights And FileSystemRights.WriteData)
        Assert.AreEqual(FileSystemRights.AppendData, rights And FileSystemRights.AppendData)
        Assert.AreEqual(FileSystemRights.WriteAttributes, rights And FileSystemRights.WriteAttributes)
        Assert.AreEqual(FileSystemRights.WriteExtendedAttributes, rights And FileSystemRights.WriteExtendedAttributes)
        Assert.AreEqual(FileSystemRights.Delete, rights And FileSystemRights.Delete)
        Assert.AreEqual(FileSystemRights.DeleteSubdirectoriesAndFiles, rights And FileSystemRights.DeleteSubdirectoriesAndFiles)

    End Sub

    ''' <summary>
    ''' Verifies that the denied rights never block reading, listing or traversing the directory.
    ''' <para></para>
    ''' The original implementation added overlapping flags with "+" instead of "Or", which produced a mask that
    ''' also denied <see cref="FileSystemRights.ExecuteFile"/> (Traverse), <see cref="FileSystemRights.ReadExtendedAttributes"/>
    ''' and an undefined bit (0x200).
    ''' </summary>
    <TestMethod>
    Public Sub DeniedRights_DoesNotContainReadRights()

        Dim rights As FileSystemRights = FolderLock.DeniedRights
        Dim forbidden As FileSystemRights() = {
            FileSystemRights.ReadData,
            FileSystemRights.ListDirectory,
            FileSystemRights.ReadExtendedAttributes,
            FileSystemRights.ExecuteFile,
            FileSystemRights.Traverse,
            FileSystemRights.ReadAttributes,
            FileSystemRights.ReadPermissions,
            FileSystemRights.ChangePermissions,
            FileSystemRights.TakeOwnership,
            FileSystemRights.Synchronize
        }
        For Each right As FileSystemRights In forbidden
            Assert.AreEqual(CType(0, FileSystemRights), rights And right, $"Unexpected denied right: {right}")
        Next
        Assert.AreEqual(0, CInt(rights) And &H200, "The undefined bit 0x200 must not be set.")

    End Sub

#End Region

#Region " Lock / Unlock (Windows only) "

    ''' <summary>
    ''' Verifies that locking prevents creating files, and unlocking allows it again.
    ''' </summary>
    <TestMethod>
    Public Sub Lock_ThenUnlock_RestoresWriteAccess()

        FolderLockTests.RequireWindows()

        Assert.IsFalse(FolderLock.IsLocked(Me.testDirectory))

        FolderLock.Lock(Me.testDirectory)
        Assert.IsTrue(FolderLock.IsLocked(Me.testDirectory))
        Assert.ThrowsExactly(Of UnauthorizedAccessException)(Sub() File.WriteAllText(Path.Combine(Me.testDirectory, "a.txt"), "x"))

        FolderLock.Unlock(Me.testDirectory)
        Assert.IsFalse(FolderLock.IsLocked(Me.testDirectory))
        File.WriteAllText(Path.Combine(Me.testDirectory, "a.txt"), "x")
        Assert.IsTrue(File.Exists(Path.Combine(Me.testDirectory, "a.txt")))

    End Sub

    ''' <summary>
    ''' Verifies that a locked directory can still be read and listed (the source files are copied while it is locked).
    ''' </summary>
    <TestMethod>
    Public Sub Lock_StillAllowsReading()

        FolderLockTests.RequireWindows()

        Dim subDirectory As String = Path.Combine(Me.testDirectory, "Sub")
        Directory.CreateDirectory(subDirectory)
        File.WriteAllText(Path.Combine(subDirectory, "file.txt"), "content")

        FolderLock.Lock(Me.testDirectory)

        Assert.AreEqual("content", File.ReadAllText(Path.Combine(subDirectory, "file.txt")))
        Assert.AreEqual(1, DirectoryScanner.GetFiles(Me.testDirectory).Count)
        Assert.AreEqual(7L, DirectoryScanner.GetDirectorySize(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that lock and unlock preserve the other explicit rules of the ACL.
    ''' The original implementation replaced the whole ACL with a new one containing a single rule.
    ''' </summary>
    <TestMethod>
    Public Sub LockUnlock_PreservesExistingExplicitRules()

        FolderLockTests.RequireWindows()

        Dim directoryInfo As New DirectoryInfo(Me.testDirectory)
        Dim everyone As New System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, Nothing)
        Dim extraRule As New FileSystemAccessRule(everyone, FileSystemRights.ReadData, AccessControlType.Allow)

        Dim security As DirectorySecurity = directoryInfo.GetAccessControl()
        security.AddAccessRule(extraRule)
        directoryInfo.SetAccessControl(security)
        Dim before As Integer = FolderLockTests.CountExplicitRules(Me.testDirectory)

        FolderLock.Lock(Me.testDirectory)
        Assert.AreEqual(before + 1, FolderLockTests.CountExplicitRules(Me.testDirectory))

        FolderLock.Unlock(Me.testDirectory)
        Assert.AreEqual(before, FolderLockTests.CountExplicitRules(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that unlocking a directory that is not locked does nothing.
    ''' </summary>
    <TestMethod>
    Public Sub Unlock_NotLocked_DoesNothing()

        FolderLockTests.RequireWindows()

        Dim before As Integer = FolderLockTests.CountExplicitRules(Me.testDirectory)
        FolderLock.Unlock(Me.testDirectory)
        Assert.AreEqual(before, FolderLockTests.CountExplicitRules(Me.testDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub LockUnlock_InvalidArguments_Throw()

        Dim missing As String = Path.Combine(Me.testDirectory, "missing")
        Assert.ThrowsExactly(Of ArgumentException)(Sub() FolderLock.Lock(Nothing))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() FolderLock.Unlock(String.Empty))
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() FolderLock.Lock(missing))
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() FolderLock.Unlock(missing))
        Assert.ThrowsExactly(Of DirectoryNotFoundException)(Sub() FolderLock.IsLocked(missing))

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Marks the current test as inconclusive when it is not running on Windows.
    ''' </summary>
    Private Shared Sub RequireWindows()

        If Not FolderLockTests.IsWindows() Then
            Assert.Inconclusive("Windows ACLs are only available on Windows.")
        End If

    End Sub

    ''' <summary>
    ''' Determines whether the tests are running on Windows.
    ''' </summary>
    ''' <returns><see langword="True"/> on Windows; otherwise, <see langword="False"/>.</returns>
    Private Shared Function IsWindows() As Boolean

        Return Environment.OSVersion.Platform = PlatformID.Win32NT

    End Function

    ''' <summary>
    ''' Counts the explicit (non-inherited) access rules of a directory.
    ''' </summary>
    ''' <param name="directoryPath">The directory.</param>
    ''' <returns>The number of explicit rules.</returns>
    Private Shared Function CountExplicitRules(directoryPath As String) As Integer

        Dim security As DirectorySecurity = New DirectoryInfo(directoryPath).GetAccessControl()
        Return security.GetAccessRules(True, False, GetType(System.Security.Principal.SecurityIdentifier)).Count

    End Function

#End Region

End Class

#End Region
