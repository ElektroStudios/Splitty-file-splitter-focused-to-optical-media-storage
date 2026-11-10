' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="SplitPathsTests.vb" company="Elektro Studios">
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

#Region " SplitPathsTests "

''' <summary>
''' Unit tests for <see cref="SplitPaths"/>.
''' </summary>
<TestClass>
Public NotInheritable Class SplitPathsTests

#Region " GetRoot / GetPathWithoutRoot "

    ''' <summary>
    ''' Verifies the root detection of drive, UNC and long paths.
    ''' </summary>
    ''' <param name="path">The full path.</param>
    ''' <param name="expectedRoot">The expected root.</param>
    ''' <param name="expectedRest">The expected path without root.</param>
    <TestMethod>
    <DataRow("C:\", "C:\", "")>
    <DataRow("C:", "C:\", "")>
    <DataRow("C:\Movies", "C:\", "Movies")>
    <DataRow("C:\Movies\Action\", "C:\", "Movies\Action")>
    <DataRow("d:/Movies/Action", "d:\", "Movies\Action")>
    <DataRow("\\nas\share", "\\nas\share\", "")>
    <DataRow("\\nas\share\", "\\nas\share\", "")>
    <DataRow("\\nas\share\Movies\Action", "\\nas\share\", "Movies\Action")>
    <DataRow("\\?\C:\Movies", "\\?\C:\", "Movies")>
    <DataRow("\\?\UNC\nas\share\Movies", "\\?\UNC\nas\share\", "Movies")>
    Public Sub GetRoot_And_GetPathWithoutRoot(path As String, expectedRoot As String, expectedRest As String)

        Assert.AreEqual(expectedRoot, SplitPaths.GetRoot(path))
        Assert.AreEqual(expectedRest, SplitPaths.GetPathWithoutRoot(path))

    End Sub

    ''' <summary>
    ''' Verifies that relative and malformed paths throw.
    ''' </summary>
    ''' <param name="path">The invalid path.</param>
    <TestMethod>
    <DataRow("")>
    <DataRow("Movies\Action")>
    <DataRow("\Movies")>
    <DataRow("\\nas")>
    <DataRow("\\nas\")>
    <DataRow("\\\share")>
    <DataRow("1:\Movies")>
    Public Sub GetPathWithoutRoot_NotRooted_Throws(path As String)

        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.GetPathWithoutRoot(path))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.GetRoot(path))

    End Sub

#End Region

#Region " GetDiscDirectory / GetDiscIsoFile "

    ''' <summary>
    ''' Verifies the disc folder and ISO image paths.
    ''' </summary>
    ''' <param name="output">The output directory.</param>
    ''' <param name="expectedDirectory">The expected disc folder.</param>
    <TestMethod>
    <DataRow("D:\Output", "D:\Output\Disc 3")>
    <DataRow("D:\Output\", "D:\Output\Disc 3")>
    <DataRow("D:\", "D:\Disc 3")>
    <DataRow("\\nas\share\Output", "\\nas\share\Output\Disc 3")>
    Public Sub GetDiscDirectory_And_GetDiscIsoFile(output As String, expectedDirectory As String)

        Assert.AreEqual(expectedDirectory, SplitPaths.GetDiscDirectory(output, 3))
        Assert.AreEqual($"{expectedDirectory}.iso", SplitPaths.GetDiscIsoFile(output, 3))

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub GetDiscDirectory_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.GetDiscDirectory(Nothing, 1))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.GetDiscDirectory(String.Empty, 1))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() SplitPaths.GetDiscDirectory("D:\Output", 0))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() SplitPaths.GetDiscIsoFile("D:\Output", -1))

    End Sub

#End Region

#Region " GetDestinationDirectory "

    ''' <summary>
    ''' Verifies the destination directory of a source file, which replicates the source directory structure
    ''' (as the original implementation did with drive paths). UNC sources made the application crash before.
    ''' </summary>
    ''' <param name="sourceDirectory">The source file directory.</param>
    ''' <param name="expected">The expected destination directory.</param>
    <TestMethod>
    <DataRow("C:\Movies\Action", "D:\Output\Disc 2\Movies\Action")>
    <DataRow("C:\Movies\Action\", "D:\Output\Disc 2\Movies\Action")>
    <DataRow("C:\", "D:\Output\Disc 2")>
    <DataRow("\\nas\share\Movies", "D:\Output\Disc 2\Movies")>
    <DataRow("\\nas\share", "D:\Output\Disc 2")>
    <DataRow("\\?\C:\Movies", "D:\Output\Disc 2\Movies")>
    Public Sub GetDestinationDirectory_ReturnsExpected(sourceDirectory As String, expected As String)

        Assert.AreEqual(expected, SplitPaths.GetDestinationDirectory("D:\Output", 2, sourceDirectory))

    End Sub

    ''' <summary>
    ''' Verifies that the result for drive paths is the same as the original implementation:
    ''' <c>output &amp; "\Disc " &amp; n &amp; directory.Split(":"c)(1)</c>.
    ''' </summary>
    <TestMethod>
    Public Sub GetDestinationDirectory_DrivePaths_MatchOriginalBehavior()

        Dim directories As String() = {"C:\Data", "C:\Data\Sub Folder", "E:\a\b\c\d"}
        For Each directory As String In directories
            Dim original As String = $"D:\Output\Disc 5{directory.Split(":"c)(1)}"
            Assert.AreEqual(original, SplitPaths.GetDestinationDirectory("D:\Output", 5, directory))
        Next

    End Sub

#End Region

#Region " AreSameDirectory / IsSameOrInside "

    ''' <summary>
    ''' Verifies the directory equality check (case-insensitive, ignoring trailing separators).
    ''' </summary>
    ''' <param name="a">The first path.</param>
    ''' <param name="b">The second path.</param>
    ''' <param name="expected">The expected result.</param>
    <TestMethod>
    <DataRow("C:\Data", "C:\Data", True)>
    <DataRow("C:\Data", "c:\DATA", True)>
    <DataRow("C:\Data\", "C:\Data", True)>
    <DataRow("C:/Data", "C:\Data", True)>
    <DataRow("C:\", "c:", True)>
    <DataRow("C:\Data", "C:\Data2", False)>
    <DataRow("C:\Data", "D:\Data", False)>
    <DataRow("C:\Data\Sub", "C:\Data", False)>
    Public Sub AreSameDirectory_ReturnsExpected(a As String, b As String, expected As Boolean)

        Assert.AreEqual(expected, SplitPaths.AreSameDirectory(a, b))
        Assert.AreEqual(expected, SplitPaths.AreSameDirectory(b, a))

    End Sub

    ''' <summary>
    ''' Verifies the nesting check used to reject an output directory inside the input directory.
    ''' </summary>
    ''' <param name="path">The path to check.</param>
    ''' <param name="parent">The possible parent.</param>
    ''' <param name="expected">The expected result.</param>
    <TestMethod>
    <DataRow("C:\Data", "C:\Data", True)>
    <DataRow("C:\Data\Out", "C:\Data", True)>
    <DataRow("c:\data\out\deep", "C:\Data\", True)>
    <DataRow("C:\Data", "C:\", True)>
    <DataRow("\\nas\share\Data\Out", "\\nas\share\Data", True)>
    <DataRow("C:\Data2", "C:\Data", False)>
    <DataRow("C:\DataOut", "C:\Data", False)>
    <DataRow("C:\", "C:\Data", False)>
    <DataRow("D:\Data\Out", "C:\Data", False)>
    Public Sub IsSameOrInside_ReturnsExpected(path As String, parent As String, expected As Boolean)

        Assert.AreEqual(expected, SplitPaths.IsSameOrInside(path, parent))

    End Sub

    ''' <summary>
    ''' Verifies that null or empty paths throw.
    ''' </summary>
    <TestMethod>
    Public Sub Comparisons_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.AreSameDirectory(Nothing, "C:\"))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.AreSameDirectory("C:\", String.Empty))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.IsSameOrInside(Nothing, "C:\"))
        Assert.ThrowsExactly(Of ArgumentException)(Sub() SplitPaths.IsSameOrInside("C:\", Nothing))

    End Sub

#End Region

End Class

#End Region
