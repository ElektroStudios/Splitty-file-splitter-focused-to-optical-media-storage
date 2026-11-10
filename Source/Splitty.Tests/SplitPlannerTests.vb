' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="SplitPlannerTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Collections.Generic

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splitty.Core

#End Region

#Region " SplitPlannerTests "

''' <summary>
''' Unit tests for <see cref="SplitPlanner"/> and <see cref="PlannedFile"/>.
''' </summary>
<TestClass>
Public NotInheritable Class SplitPlannerTests

#Region " Plan "

    ''' <summary>
    ''' Verifies that an empty file list produces an empty plan.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_NoFiles_ReturnsEmptyPlan()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(New List(Of SourceFileEntry)(), 100L)
        Assert.IsEmpty(plan)
        Assert.AreEqual(0, SplitPlanner.GetDiscCount(plan))

    End Sub

    ''' <summary>
    ''' Verifies that files that fit in a single disc are all assigned to disc 1.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_AllFilesFit_UsesSingleDisc()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(10, 20, 30), 100L)
        CollectionAssert.AreEqual(New Integer() {1, 1, 1}, SplitPlannerTests.DiscNumbers(plan))
        Assert.AreEqual(1, SplitPlanner.GetDiscCount(plan))

    End Sub

    ''' <summary>
    ''' Verifies that a disc filled exactly up to its capacity does not start a new disc.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_ExactCapacity_StaysInSameDisc()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(40, 60, 1), 100L)
        CollectionAssert.AreEqual(New Integer() {1, 1, 2}, SplitPlannerTests.DiscNumbers(plan))

    End Sub

    ''' <summary>
    ''' Verifies the sequential distribution: a file that does not fit starts a new disc.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_SequentialDistribution()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(60, 30, 20, 80, 10, 5, 100), 100L)
        CollectionAssert.AreEqual(New Integer() {1, 1, 2, 2, 3, 3, 4}, SplitPlannerTests.DiscNumbers(plan))
        Assert.AreEqual(4, SplitPlanner.GetDiscCount(plan))

    End Sub

    ''' <summary>
    ''' Verifies that a first file bigger than the disc capacity is assigned to disc 1.
    ''' Before, disc 1 was skipped (never created) and, in ISO mode, the application tried forever
    ''' to create the image of the missing "Disc 1" folder.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_FirstFileBiggerThanCapacity_UsesDiscOne()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(150, 10), 100L)
        CollectionAssert.AreEqual(New Integer() {1, 2}, SplitPlannerTests.DiscNumbers(plan))

    End Sub

    ''' <summary>
    ''' Verifies that a file bigger than the disc capacity is placed alone in its own disc.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_OversizedFileInTheMiddle_IsPlacedAlone()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(10, 500, 10, 600, 600), 100L)
        CollectionAssert.AreEqual(New Integer() {1, 2, 3, 4, 5}, SplitPlannerTests.DiscNumbers(plan))

    End Sub

    ''' <summary>
    ''' Verifies that disc numbers are consecutive, never decrease, and that no disc exceeds its capacity
    ''' unless it contains a single oversized file.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_RandomFiles_RespectsInvariants()

        Const Capacity As Long = 1000L
        Dim random As New Random(12345)
        Dim sizes As New List(Of Long)()
        For index As Integer = 1 To 500
            sizes.Add(CLng(random.Next(0, 1300)))
        Next

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(sizes.ToArray()), Capacity)
        Assert.HasCount(sizes.Count, plan)

        Dim discSizes As New Dictionary(Of Integer, Long)()
        Dim discFileCounts As New Dictionary(Of Integer, Integer)()
        Dim previousDisc As Integer = 1
        For index As Integer = 0 To plan.Count - 1
            Dim item As PlannedFile = plan(index)
            Assert.AreEqual(sizes(index), item.File.Length, "The order of the files must be preserved.")
            Assert.IsTrue(item.DiscNumber = previousDisc OrElse item.DiscNumber = previousDisc + 1, "Disc numbers must be consecutive.")
            previousDisc = item.DiscNumber

            If Not discSizes.ContainsKey(item.DiscNumber) Then
                discSizes(item.DiscNumber) = 0L
                discFileCounts(item.DiscNumber) = 0
            End If
            discSizes(item.DiscNumber) += item.File.Length
            discFileCounts(item.DiscNumber) += 1
        Next

        Assert.AreEqual(1, plan(0).DiscNumber)
        For Each disc As Integer In discSizes.Keys
            If discSizes(disc) > Capacity Then
                Dim nonEmptyFiles As Integer = 0
                For Each item As PlannedFile In plan
                    If item.DiscNumber = disc AndAlso item.File.Length > 0L Then
                        nonEmptyFiles += 1
                    End If
                Next
                Assert.AreEqual(1, nonEmptyFiles, $"Disc {disc} exceeds the capacity with more than one file.")
            End If
        Next

    End Sub

    ''' <summary>
    ''' Verifies that huge file sizes do not overflow.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_HugeSizes_DoesNotOverflow()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(Long.MaxValue, Long.MaxValue, 1L), Long.MaxValue)
        CollectionAssert.AreEqual(New Integer() {1, 2, 3}, SplitPlannerTests.DiscNumbers(plan))

    End Sub

    ''' <summary>
    ''' Verifies that empty files never start a new disc.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_EmptyFiles_NeverStartNewDisc()

        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(SplitPlannerTests.Files(100, 0, 0, 1, 0), 100L)
        CollectionAssert.AreEqual(New Integer() {1, 1, 1, 2, 2}, SplitPlannerTests.DiscNumbers(plan))

    End Sub

    ''' <summary>
    ''' Verifies that the planner is consistent with the minimum disc count:
    ''' it never uses fewer discs than <see cref="DiscCapacity.CalculateDiscCount"/>.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_UsesAtLeastTheMinimumDiscCount()

        Dim files As SourceFileEntry() = SplitPlannerTests.Files(70, 70, 70, 70, 70)
        Dim plan As List(Of PlannedFile) = SplitPlanner.Plan(files, 100L)
        Assert.AreEqual(5, SplitPlanner.GetDiscCount(plan))
        Assert.IsGreaterThanOrEqualTo(DiscCapacity.CalculateDiscCount(350L, 100L), CLng(SplitPlanner.GetDiscCount(plan)))

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub Plan_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplitPlanner.Plan(Nothing, 100L))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() SplitPlanner.Plan(SplitPlannerTests.Files(1), 0L))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() SplitPlanner.Plan(SplitPlannerTests.Files(1), -5L))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplitPlanner.Plan(New SourceFileEntry() {Nothing}, 100L))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() SplitPlanner.GetDiscCount(Nothing))

    End Sub

#End Region

#Region " PlannedFile "

    ''' <summary>
    ''' Verifies that invalid arguments of <see cref="PlannedFile"/> throw.
    ''' </summary>
    <TestMethod>
    Public Sub PlannedFile_InvalidArguments_Throw()

        Dim file As New SourceFileEntry("C:\", "a", 1L)
        Assert.ThrowsExactly(Of ArgumentNullException)(
            Sub()
                Dim unused As New PlannedFile(Nothing, 1)
            End Sub)
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(
            Sub()
                Dim unused As New PlannedFile(file, 0)
            End Sub)

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Creates source file entries with the specified sizes.
    ''' </summary>
    ''' <param name="sizes">The file sizes.</param>
    ''' <returns>The file entries, named "file0", "file1", etc.</returns>
    Private Shared Function Files(ParamArray sizes As Long()) As SourceFileEntry()

        Dim result(sizes.Length - 1) As SourceFileEntry
        For index As Integer = 0 To sizes.Length - 1
            result(index) = New SourceFileEntry("C:\Source", $"file{index}", sizes(index))
        Next
        Return result

    End Function

    ''' <summary>
    ''' Gets the disc numbers of a plan.
    ''' </summary>
    ''' <param name="plan">The plan.</param>
    ''' <returns>The disc number of each planned file.</returns>
    Private Shared Function DiscNumbers(plan As List(Of PlannedFile)) As Integer()

        Dim result(plan.Count - 1) As Integer
        For index As Integer = 0 To plan.Count - 1
            result(index) = plan(index).DiscNumber
        Next
        Return result

    End Function

#End Region

End Class

#End Region
