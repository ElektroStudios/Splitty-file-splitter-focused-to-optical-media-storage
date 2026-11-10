' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="CustomSizeTests.vb" company="Elektro Studios">
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

#Region " CustomSizeTests "

''' <summary>
''' Unit tests for <see cref="CustomSize"/>.
''' </summary>
<TestClass>
Public NotInheritable Class CustomSizeTests

#Region " GetUnitBytes "

    ''' <summary>
    ''' Verifies the bytes of every unit, and that the enum values match the ComboBox item indices (MB, GB, TB).
    ''' </summary>
    <TestMethod>
    Public Sub GetUnitBytes_ReturnsExpected()

        Assert.AreEqual(0, CInt(CustomSizeUnit.Megabytes))
        Assert.AreEqual(1, CInt(CustomSizeUnit.Gigabytes))
        Assert.AreEqual(2, CInt(CustomSizeUnit.Terabytes))

        Assert.AreEqual(1048576L, CustomSize.GetUnitBytes(CustomSizeUnit.Megabytes))
        Assert.AreEqual(1073741824L, CustomSize.GetUnitBytes(CustomSizeUnit.Gigabytes))
        Assert.AreEqual(1099511627776L, CustomSize.GetUnitBytes(CustomSizeUnit.Terabytes))

    End Sub

    ''' <summary>
    ''' Verifies that an undefined unit (for example, ComboBox index -1) throws.
    ''' </summary>
    <TestMethod>
    Public Sub GetUnitBytes_UndefinedUnit_Throws()

        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() CustomSize.GetUnitBytes(CType(-1, CustomSizeUnit)))
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() CustomSize.GetUnitBytes(CType(3, CustomSizeUnit)))

    End Sub

#End Region

#Region " TryParseBytes "

    ''' <summary>
    ''' Verifies valid custom sizes.
    ''' </summary>
    ''' <param name="text">The typed text.</param>
    ''' <param name="unit">The selected unit.</param>
    ''' <param name="expected">The expected bytes.</param>
    <TestMethod>
    <DataRow("1", CustomSizeUnit.Megabytes, 1048576L)>
    <DataRow("700", CustomSizeUnit.Megabytes, 734003200L)>
    <DataRow("1,5", CustomSizeUnit.Megabytes, 1572864L)>
    <DataRow("1.5", CustomSizeUnit.Megabytes, 1572864L)>
    <DataRow("4,7", CustomSizeUnit.Gigabytes, 5046586572L)>
    <DataRow("0,5", CustomSizeUnit.Gigabytes, 536870912L)>
    <DataRow("1", CustomSizeUnit.Terabytes, 1099511627776L)>
    <DataRow("1,", CustomSizeUnit.Megabytes, 1048576L)>
    <DataRow(",5", CustomSizeUnit.Megabytes, 524288L)>
    <DataRow(" 2 ", CustomSizeUnit.Megabytes, 2097152L)>
    <DataRow("0001", CustomSizeUnit.Megabytes, 1048576L)>
    Public Sub TryParseBytes_ValidText_ReturnsTrue(text As String, unit As CustomSizeUnit, expected As Long)

        Dim result As Long
        Assert.IsTrue(CustomSize.TryParseBytes(text, unit, result))
        Assert.AreEqual(expected, result)

    End Sub

    ''' <summary>
    ''' Verifies invalid custom sizes. Some of them made the application crash before (for example, ",").
    ''' </summary>
    ''' <param name="text">The typed or pasted text.</param>
    <TestMethod>
    <DataRow("")>
    <DataRow("   ")>
    <DataRow(",")>
    <DataRow("0")>
    <DataRow("0,0")>
    <DataRow("1,2,3")>
    <DataRow("abc")>
    <DataRow("-5")>
    <DataRow("1e3")>
    <DataRow("1 000")>
    Public Sub TryParseBytes_InvalidText_ReturnsFalse(text As String)

        Dim result As Long = 123L
        Assert.IsFalse(CustomSize.TryParseBytes(text, CustomSizeUnit.Megabytes, result))
        Assert.AreEqual(0L, result)

    End Sub

    ''' <summary>
    ''' Verifies that a null text is rejected.
    ''' </summary>
    <TestMethod>
    Public Sub TryParseBytes_NullText_ReturnsFalse()

        Dim result As Long = 123L
        Assert.IsFalse(CustomSize.TryParseBytes(Nothing, CustomSizeUnit.Megabytes, result))
        Assert.AreEqual(0L, result)

    End Sub

    ''' <summary>
    ''' Verifies that values that do not fit in a <see cref="Long"/> are rejected instead of overflowing.
    ''' </summary>
    <TestMethod>
    Public Sub TryParseBytes_Overflow_ReturnsFalse()

        Dim result As Long
        Assert.IsFalse(CustomSize.TryParseBytes("99999999", CustomSizeUnit.Terabytes, result))
        Assert.IsFalse(CustomSize.TryParseBytes(New String("9"c, 40), CustomSizeUnit.Terabytes, result))

    End Sub

    ''' <summary>
    ''' Verifies that a value smaller than one byte is rejected.
    ''' </summary>
    <TestMethod>
    Public Sub TryParseBytes_LessThanOneByte_ReturnsFalse()

        Dim result As Long
        Assert.IsFalse(CustomSize.TryParseBytes("0,0000001", CustomSizeUnit.Megabytes, result))

    End Sub

    ''' <summary>
    ''' Verifies that the result does not depend on the current culture.
    ''' Before, "1,5" was parsed as 15 under English cultures, where the comma is the thousands separator.
    ''' </summary>
    ''' <param name="cultureName">The culture to set as current culture.</param>
    <TestMethod>
    <DataRow("en-US")>
    <DataRow("es-ES")>
    <DataRow("de-DE")>
    <DataRow("")>
    Public Sub TryParseBytes_IsCultureIndependent(cultureName As String)

        Dim previous As CultureInfo = CultureInfo.CurrentCulture
        Try
            CultureInfo.CurrentCulture = New CultureInfo(cultureName)
            Dim result As Long
            Assert.IsTrue(CustomSize.TryParseBytes("1,5", CustomSizeUnit.Gigabytes, result))
            Assert.AreEqual(1610612736L, result)
        Finally
            CultureInfo.CurrentCulture = previous
        End Try

    End Sub

    ''' <summary>
    ''' Verifies that an undefined unit throws even when the text is invalid.
    ''' </summary>
    <TestMethod>
    Public Sub TryParseBytes_UndefinedUnit_Throws()

        Dim result As Long
        Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Sub() CustomSize.TryParseBytes("1", CType(-1, CustomSizeUnit), result))

    End Sub

#End Region

#Region " IsKeyRejected "

    ''' <summary>
    ''' Verifies the accepted keys.
    ''' </summary>
    ''' <param name="key">The typed key.</param>
    ''' <param name="currentText">The current text.</param>
    <TestMethod>
    <DataRow("0"c, "")>
    <DataRow("5"c, "12")>
    <DataRow("9"c, "1,2")>
    <DataRow(","c, "12")>
    <DataRow(","c, "")>
    <DataRow(ChrW(8), "1,2")>
    Public Sub IsKeyRejected_AllowedKey_ReturnsFalse(key As Char, currentText As String)

        Assert.IsFalse(CustomSize.IsKeyRejected(key, currentText))

    End Sub

    ''' <summary>
    ''' Verifies the rejected keys.
    ''' </summary>
    ''' <param name="key">The typed key.</param>
    ''' <param name="currentText">The current text.</param>
    <TestMethod>
    <DataRow("a"c, "")>
    <DataRow("."c, "1")>
    <DataRow("-"c, "")>
    <DataRow(" "c, "1")>
    <DataRow(","c, "1,2")>
    <DataRow(ChrW(13), "1")>
    Public Sub IsKeyRejected_DisallowedKey_ReturnsTrue(key As Char, currentText As String)

        Assert.IsTrue(CustomSize.IsKeyRejected(key, currentText))

    End Sub

    ''' <summary>
    ''' Verifies that a null text is treated as empty.
    ''' </summary>
    <TestMethod>
    Public Sub IsKeyRejected_NullText_DoesNotThrow()

        Assert.IsFalse(CustomSize.IsKeyRejected(","c, Nothing))
        Assert.IsFalse(CustomSize.IsKeyRejected("1"c, Nothing))

    End Sub

#End Region

End Class

#End Region
