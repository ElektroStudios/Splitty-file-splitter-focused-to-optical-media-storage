' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="ByteSizeFormatterTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Globalization

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Splitty.Core

#End Region

#Region " ByteSizeFormatterTests "

''' <summary>
''' Unit tests for <see cref="ByteSizeFormatter"/>.
''' </summary>
<TestClass>
Public NotInheritable Class ByteSizeFormatterTests

    ''' <summary>
    ''' Verifies the unit constants.
    ''' </summary>
    <TestMethod>
    Public Sub UnitConstants_AreBinaryMultiples()

        Assert.AreEqual(1024L, ByteSizeFormatter.Kilobyte)
        Assert.AreEqual(1048576L, ByteSizeFormatter.Megabyte)
        Assert.AreEqual(1073741824L, ByteSizeFormatter.Gigabyte)
        Assert.AreEqual(1099511627776L, ByteSizeFormatter.Terabyte)
        Assert.AreEqual(1125899906842624L, ByteSizeFormatter.Petabyte)
        Assert.AreEqual(1152921504606846976L, ByteSizeFormatter.Exabyte)

    End Sub

    ''' <summary>
    ''' Verifies the formatted output with the invariant culture, including unit boundaries.
    ''' </summary>
    ''' <param name="bytes">The size in bytes.</param>
    ''' <param name="expected">The expected string.</param>
    <TestMethod>
    <DataRow(0L, "0 bytes")>
    <DataRow(1L, "1 bytes")>
    <DataRow(1023L, "1023 bytes")>
    <DataRow(1024L, "1 KB")>
    <DataRow(15360L, "15 KB")>
    <DataRow(1048575L, "1024 KB")>
    <DataRow(1048576L, "1 MB")>
    <DataRow(1572864L, "1.5 MB")>
    <DataRow(734003200L, "700 MB")>
    <DataRow(1073741824L, "1 GB")>
    <DataRow(4700000000L, "4.4 GB")>
    <DataRow(25025314816L, "23.3 GB")>
    <DataRow(1099511627776L, "1 TB")>
    <DataRow(1649267441664L, "1.5 TB")>
    <DataRow(1125899906842624L, "1 PB")>
    <DataRow(1152921504606846976L, "1 EB")>
    Public Sub Format_InvariantCulture_ReturnsExpected(bytes As Long, expected As String)

        Assert.AreEqual(expected, ByteSizeFormatter.Format(bytes, CultureInfo.InvariantCulture))

    End Sub

    ''' <summary>
    ''' Verifies that sizes of one petabyte or more are formatted (previously, Nothing was returned).
    ''' </summary>
    <TestMethod>
    Public Sub Format_MaxValue_DoesNotReturnNothing()

        Dim result As String = ByteSizeFormatter.Format(Long.MaxValue, CultureInfo.InvariantCulture)
        Assert.AreEqual("8 EB", result)

    End Sub

    ''' <summary>
    ''' Verifies that the decimal separator of the specified culture is used.
    ''' </summary>
    <TestMethod>
    Public Sub Format_SpanishCulture_UsesCommaDecimalSeparator()

        Assert.AreEqual("4,4 GB", ByteSizeFormatter.Format(4700000000L, New CultureInfo("es-ES")))

    End Sub

    ''' <summary>
    ''' Verifies that the overload without culture uses the current culture.
    ''' </summary>
    <TestMethod>
    Public Sub Format_WithoutCulture_UsesCurrentCulture()

        Dim previous As CultureInfo = CultureInfo.CurrentCulture
        Try
            CultureInfo.CurrentCulture = New CultureInfo("es-ES")
            Assert.AreEqual("1,5 MB", ByteSizeFormatter.Format(1572864L))
            CultureInfo.CurrentCulture = New CultureInfo("en-US")
            Assert.AreEqual("1.5 MB", ByteSizeFormatter.Format(1572864L))
        Finally
            CultureInfo.CurrentCulture = previous
        End Try

    End Sub

    ''' <summary>
    ''' Verifies that invalid arguments throw.
    ''' </summary>
    <TestMethod>
    Public Sub Format_InvalidArguments_Throw()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() ByteSizeFormatter.Format(-1L, CultureInfo.InvariantCulture))
        Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ByteSizeFormatter.Format(1L, Nothing))

    End Sub

End Class

#End Region
