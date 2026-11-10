' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="CustomSize.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Globalization

#End Region

#Region " CustomSizeUnit "

Namespace Core

    ''' <summary>
    ''' Specifies the units available for a custom disc size.
    ''' <para></para>
    ''' The numeric values match the item indices of the custom size unit ComboBox.
    ''' </summary>
    Public Enum CustomSizeUnit As Integer

        ''' <summary>
        ''' Megabytes (1024^2 bytes).
        ''' </summary>
        Megabytes = 0

        ''' <summary>
        ''' Gigabytes (1024^3 bytes).
        ''' </summary>
        Gigabytes = 1

        ''' <summary>
        ''' Terabytes (1024^4 bytes).
        ''' </summary>
        Terabytes = 2

    End Enum

End Namespace

#End Region

#Region " CustomSize "

Namespace Core

    ''' <summary>
    ''' Provides parsing and keyboard input validation for custom disc sizes typed by the user.
    ''' </summary>
    Public NotInheritable Class CustomSize

#Region " Constants "

        ''' <summary>
        ''' The decimal separator character shown in the custom size TextBox.
        ''' </summary>
        Public Const DecimalSeparator As Char = ","c

        ''' <summary>
        ''' The characters accepted by the custom size TextBox.
        ''' </summary>
        Private Const AllowedChars As String = "0123456789,"

        ''' <summary>
        ''' The backspace character.
        ''' </summary>
        Private Const BackspaceChar As Char = ChrW(8)

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="CustomSize"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the number of bytes of the specified custom size unit.
        ''' </summary>
        ''' <param name="unit">The custom size unit.</param>
        ''' <returns>The number of bytes of one unit.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="unit"/> is not a defined <see cref="CustomSizeUnit"/> value.
        ''' </exception>
        Public Shared Function GetUnitBytes(unit As CustomSizeUnit) As Long

            Select Case unit
                Case CustomSizeUnit.Megabytes : Return ByteSizeFormatter.Megabyte
                Case CustomSizeUnit.Gigabytes : Return ByteSizeFormatter.Gigabyte
                Case CustomSizeUnit.Terabytes : Return ByteSizeFormatter.Terabyte
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(unit), unit, $"Unsupported custom size unit: {CInt(unit)}")
            End Select

        End Function

        ''' <summary>
        ''' Tries to convert the text of the custom size TextBox to a number of bytes.
        ''' <para></para>
        ''' Both comma and dot are accepted as decimal separator, regardless of the current culture.
        ''' </summary>
        ''' <param name="text">The text to parse, for example "4,7".</param>
        ''' <param name="unit">The unit of the value.</param>
        ''' <param name="result">
        ''' When this method returns <see langword="True"/>, contains the size in bytes (truncated to an integer value);
        ''' otherwise, zero.
        ''' </param>
        ''' <returns>
        ''' <see langword="True"/> if <paramref name="text"/> represents a size greater than zero bytes
        ''' that fits in a <see cref="Long"/>; otherwise, <see langword="False"/>.
        ''' </returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="unit"/> is not a defined <see cref="CustomSizeUnit"/> value.
        ''' </exception>
        Public Shared Function TryParseBytes(text As String, unit As CustomSizeUnit, ByRef result As Long) As Boolean

            result = 0L
            Dim unitBytes As Long = CustomSize.GetUnitBytes(unit)

            If String.IsNullOrWhiteSpace(text) Then
                Return False
            End If

            Dim normalized As String = text.Trim().Replace(CustomSize.DecimalSeparator, "."c)
            Dim value As Decimal
            If Not Decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, value) Then
                Return False
            End If

            Dim bytes As Decimal
            Try
                bytes = Decimal.Truncate(value * CDec(unitBytes))
            Catch ex As OverflowException
                Return False
            End Try

            If bytes <= 0D OrElse bytes > CDec(Long.MaxValue) Then
                Return False
            End If

            result = CLng(bytes)
            Return True

        End Function

        ''' <summary>
        ''' Determines whether a key typed in the custom size TextBox must be rejected.
        ''' <para></para>
        ''' Only digits, backspace and a single decimal separator (comma) are accepted.
        ''' </summary>
        ''' <param name="keyChar">The typed character.</param>
        ''' <param name="currentText">The current text of the TextBox.</param>
        ''' <returns>
        ''' <see langword="True"/> if the key must be rejected (handled); otherwise, <see langword="False"/>.
        ''' </returns>
        Public Shared Function IsKeyRejected(keyChar As Char, currentText As String) As Boolean

            If keyChar = BackspaceChar Then
                Return False
            End If

            If AllowedChars.IndexOf(keyChar) < 0 Then
                Return True
            End If

            If keyChar = CustomSize.DecimalSeparator AndAlso
               currentText IsNot Nothing AndAlso
               currentText.IndexOf(CustomSize.DecimalSeparator) >= 0 Then
                Return True
            End If

            Return False

        End Function

#End Region

    End Class

End Namespace

#End Region
