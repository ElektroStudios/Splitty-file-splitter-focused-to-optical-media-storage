' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="ArchiveCommandBuilderTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splitty.Core

#End Region

#Region " ArchiveCommandBuilderTests "

''' <summary>
''' Unit tests for <see cref="ArchiveCommandBuilder"/>.
''' </summary>
<TestClass>
Public NotInheritable Class ArchiveCommandBuilderTests

#Region " IsArchiveMode / GetVolumeSearchPattern "

    ''' <summary>
    ''' Verifies which modes create archives.
    ''' </summary>
    ''' <param name="mode">The split mode.</param>
    ''' <param name="expected">The expected result.</param>
    <TestMethod>
    <DataRow(SplitMode.Copy, False)>
    <DataRow(SplitMode.Iso, False)>
    <DataRow(SplitMode.Zip, True)>
    <DataRow(SplitMode.Rar, True)>
    <DataRow(SplitMode.Sfx, True)>
    <DataRow(SplitMode.Tar, True)>
    Public Sub IsArchiveMode_ReturnsExpected(mode As SplitMode, expected As Boolean)

        Assert.AreEqual(expected, ArchiveCommandBuilder.IsArchiveMode(mode))

    End Sub

    ''' <summary>
    ''' Verifies that an undefined mode throws.
    ''' </summary>
    <TestMethod>
    Public Sub IsArchiveMode_UndefinedMode_Throws()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.IsArchiveMode(CType(42, SplitMode)))

    End Sub

    ''' <summary>
    ''' Verifies the volume search patterns.
    ''' </summary>
    ''' <param name="mode">The split mode.</param>
    ''' <param name="expected">The expected pattern.</param>
    <TestMethod>
    <DataRow(SplitMode.Zip, "*.zip.*")>
    <DataRow(SplitMode.Tar, "*.tar.*")>
    <DataRow(SplitMode.Rar, "*part*.rar")>
    <DataRow(SplitMode.Sfx, "*part*.rar")>
    Public Sub GetVolumeSearchPattern_ReturnsExpected(mode As SplitMode, expected As String)

        Assert.AreEqual(expected, ArchiveCommandBuilder.GetVolumeSearchPattern(mode))

    End Sub

    ''' <summary>
    ''' Verifies that non-archive modes have no volume pattern.
    ''' </summary>
    <TestMethod>
    Public Sub GetVolumeSearchPattern_NonArchiveMode_Throws()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.GetVolumeSearchPattern(SplitMode.Copy))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.GetVolumeSearchPattern(SplitMode.Iso))

    End Sub

#End Region

#Region " BuildArchiveArguments "

    ''' <summary>
    ''' Verifies the arguments of every archive mode (same as the original implementation).
    ''' </summary>
    ''' <param name="mode">The split mode.</param>
    ''' <param name="expected">The expected arguments.</param>
    <TestMethod>
    <DataRow(SplitMode.Zip, " a ""D:\Out\Disc.zip"" ""C:\Data"" -v734003200b  -mx=0 -bd -tzip")>
    <DataRow(SplitMode.Tar, " a ""D:\Out\Disc.tar"" ""C:\Data"" -v734003200b  -mx=0 -bd -ttar")>
    <DataRow(SplitMode.Rar, " a ""D:\Out\Disc.rar"" ""C:\Data"" -v734003200b  -m0  -ibck -o+")>
    <DataRow(SplitMode.Sfx, " a -sfx ""D:\Out\Disc.exe"" ""C:\Data"" -v734003200b  -m0  -ibck -o+")>
    Public Sub BuildArchiveArguments_ReturnsExpected(mode As SplitMode, expected As String)

        Assert.AreEqual(expected, ArchiveCommandBuilder.BuildArchiveArguments(mode, "C:\Data", "D:\Out", 734003200L))

    End Sub

    ''' <summary>
    ''' Verifies that trailing separators never produce an escaped quote (\") or a double separator.
    ''' </summary>
    <TestMethod>
    Public Sub BuildArchiveArguments_TrailingSeparators_AreRemoved()

        Dim result As String = ArchiveCommandBuilder.BuildArchiveArguments(SplitMode.Zip, "C:\Data\", "D:\", 100L)
        Assert.AreEqual(" a ""D:\Disc.zip"" ""C:\Data"" -v100b  -mx=0 -bd -tzip", result)

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub BuildArchiveArguments_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.BuildArchiveArguments(SplitMode.Copy, "C:\Data", "D:\Out", 100L))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.BuildArchiveArguments(SplitMode.Iso, "C:\Data", "D:\Out", 100L))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.BuildArchiveArguments(SplitMode.Zip, "C:\Data", "D:\Out", 0L))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() ArchiveCommandBuilder.BuildArchiveArguments(SplitMode.Zip, Nothing, "D:\Out", 100L))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() ArchiveCommandBuilder.BuildArchiveArguments(SplitMode.Zip, "C:\Data", "", 100L))

    End Sub

#End Region

#Region " BuildExtractArguments / BuildIsoArguments "

    ''' <summary>
    ''' Verifies the 7-Zip extraction arguments. The temporary directory always ends with a backslash,
    ''' which must be removed to not escape the closing quote.
    ''' </summary>
    <TestMethod>
    Public Sub BuildExtractArguments_ReturnsExpected()

        Dim result As String = ArchiveCommandBuilder.BuildExtractArguments("C:\Temp\Splitty_WinRAR.7z", "C:\Temp\")
        Assert.AreEqual("e ""C:\Temp\Splitty_WinRAR.7z"" -o""C:\Temp"" -y", result)

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub BuildExtractArguments_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() ArchiveCommandBuilder.BuildExtractArguments(Nothing, "C:\Temp"))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() ArchiveCommandBuilder.BuildExtractArguments("C:\a.7z", ""))

    End Sub

    ''' <summary>
    ''' Verifies the PowerISO arguments (same as the original implementation).
    ''' </summary>
    <TestMethod>
    Public Sub BuildIsoArguments_ReturnsExpected()

        Dim expected As String = " -disable-optimization create -o ""D:\Out\Disc 2.iso"" -add ""D:\Out\Disc 2"" /"
        Assert.AreEqual(expected, ArchiveCommandBuilder.BuildIsoArguments("D:\Out", 2))
        Assert.AreEqual(expected, ArchiveCommandBuilder.BuildIsoArguments("D:\Out\", 2))

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub BuildIsoArguments_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() ArchiveCommandBuilder.BuildIsoArguments(Nothing, 1))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ArchiveCommandBuilder.BuildIsoArguments("D:\Out", 0))

    End Sub

#End Region

End Class

#End Region
