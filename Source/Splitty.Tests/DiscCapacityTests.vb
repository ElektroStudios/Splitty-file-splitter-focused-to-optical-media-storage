' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="DiscCapacityTests.vb" company="Elektro Studios">
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

#Region " DiscCapacityTests "

''' <summary>
''' Unit tests for <see cref="DiscCapacity"/>.
''' </summary>
<TestClass>
Public NotInheritable Class DiscCapacityTests

    ''' <summary>
    ''' Verifies the capacity of every supported disc format.
    ''' </summary>
    ''' <param name="discType">The disc format.</param>
    ''' <param name="expected">The expected capacity in bytes.</param>
    <TestMethod>
    <DataRow(DiscType.CD, 734003200L)>
    <DataRow(DiscType.CD800MB, 829440393L)>
    <DataRow(DiscType.CD900MB, 912383803L)>
    <DataRow(DiscType.Dvd5, 4700000000L)>
    <DataRow(DiscType.Dvd9, 8500000000L)>
    <DataRow(DiscType.Dvd10, 9395240960L)>
    <DataRow(DiscType.BR, 25025314816L)>
    <DataRow(DiscType.BRDoubleLayer, 50050629632L)>
    <DataRow(DiscType.BRMiniDisc, 7791181824L)>
    <DataRow(DiscType.BRMiniDiscDoubleLayer, 15582363648L)>
    Public Sub GetCapacity_ReturnsExpectedBytes(discType As DiscType, expected As Long)

        Assert.AreEqual(expected, DiscCapacity.GetCapacity(discType))

    End Sub

    ''' <summary>
    ''' Verifies that every defined disc format has a positive capacity.
    ''' </summary>
    <TestMethod>
    Public Sub GetCapacity_AllDefinedValues_ArePositive()

        For Each value As DiscType In [Enum].GetValues(GetType(DiscType))
            Assert.IsGreaterThan(0L, DiscCapacity.GetCapacity(value), $"Disc type: {value}")
        Next

    End Sub

    ''' <summary>
    ''' Verifies that bigger formats of the same family have bigger capacities.
    ''' </summary>
    <TestMethod>
    Public Sub GetCapacity_FormatsOfSameFamily_AreOrdered()

        Assert.IsLessThan(DiscCapacity.GetCapacity(DiscType.CD800MB), DiscCapacity.GetCapacity(DiscType.CD))
        Assert.IsLessThan(DiscCapacity.GetCapacity(DiscType.CD900MB), DiscCapacity.GetCapacity(DiscType.CD800MB))
        Assert.IsLessThan(DiscCapacity.GetCapacity(DiscType.Dvd9), DiscCapacity.GetCapacity(DiscType.Dvd5))
        Assert.IsLessThan(DiscCapacity.GetCapacity(DiscType.Dvd10), DiscCapacity.GetCapacity(DiscType.Dvd9))
        Assert.IsLessThan(DiscCapacity.GetCapacity(DiscType.BRDoubleLayer), DiscCapacity.GetCapacity(DiscType.BR))
        Assert.AreEqual(DiscCapacity.GetCapacity(DiscType.BR) * 2L, DiscCapacity.GetCapacity(DiscType.BRDoubleLayer))
        Assert.AreEqual(DiscCapacity.GetCapacity(DiscType.BRMiniDisc) * 2L, DiscCapacity.GetCapacity(DiscType.BRMiniDiscDoubleLayer))

    End Sub

    ''' <summary>
    ''' Verifies that an undefined disc format throws.
    ''' </summary>
    <TestMethod>
    Public Sub GetCapacity_UndefinedValue_Throws()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() DiscCapacity.GetCapacity(CType(999, DiscType)))

    End Sub

    ''' <summary>
    ''' Verifies the disc count calculation (ceiling division).
    ''' </summary>
    ''' <param name="totalBytes">The total amount of data.</param>
    ''' <param name="capacityBytes">The disc capacity.</param>
    ''' <param name="expected">The expected disc count.</param>
    <TestMethod>
    <DataRow(0L, 100L, 0L)>
    <DataRow(1L, 100L, 1L)>
    <DataRow(99L, 100L, 1L)>
    <DataRow(100L, 100L, 1L)>
    <DataRow(101L, 100L, 2L)>
    <DataRow(200L, 100L, 2L)>
    <DataRow(201L, 100L, 3L)>
    <DataRow(1468006400L, 734003200L, 2L)>
    <DataRow(1468006401L, 734003200L, 3L)>
    Public Sub CalculateDiscCount_ReturnsCeiling(totalBytes As Long, capacityBytes As Long, expected As Long)

        Assert.AreEqual(expected, DiscCapacity.CalculateDiscCount(totalBytes, capacityBytes))

    End Sub

    ''' <summary>
    ''' Verifies that the calculation is exact with values where floating point arithmetic loses precision.
    ''' </summary>
    <TestMethod>
    Public Sub CalculateDiscCount_HugeValues_IsExact()

        Assert.AreEqual(Long.MaxValue, DiscCapacity.CalculateDiscCount(Long.MaxValue, 1L))
        Assert.AreEqual(2L, DiscCapacity.CalculateDiscCount(Long.MaxValue, (Long.MaxValue \ 2L) + 1L))
        Assert.AreEqual(1L, DiscCapacity.CalculateDiscCount(Long.MaxValue - 1L, Long.MaxValue))

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw instead of returning NaN or Infinity.
    ''' </summary>
    <TestMethod>
    Public Sub CalculateDiscCount_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() DiscCapacity.CalculateDiscCount(100L, 0L))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() DiscCapacity.CalculateDiscCount(100L, -1L))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() DiscCapacity.CalculateDiscCount(-1L, 100L))

    End Sub

    ''' <summary>
    ''' Verifies that the ISO overhead is smaller than the smallest disc.
    ''' </summary>
    <TestMethod>
    Public Sub IsoOverheadBytes_IsSmallerThanSmallestDisc()

        Assert.IsGreaterThan(0L, DiscCapacity.IsoOverheadBytes)
        Assert.IsLessThan(DiscCapacity.GetCapacity(DiscType.CD), DiscCapacity.IsoOverheadBytes)

    End Sub

End Class

#End Region
