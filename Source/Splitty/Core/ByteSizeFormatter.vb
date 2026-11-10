' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="ByteSizeFormatter.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Globalization

#End Region

#Region " ByteSizeFormatter "

Namespace Core

    ''' <summary>
    ''' Formats byte sizes into human-readable strings (bytes, KB, MB, GB, TB, PB, EB),
    ''' using binary (1024-based) multiples.
    ''' </summary>
    Public NotInheritable Class ByteSizeFormatter

#Region " Constants "

        ''' <summary>
        ''' The number of bytes in one kilobyte.
        ''' </summary>
        Public Const Kilobyte As Long = 1L << 10

        ''' <summary>
        ''' The number of bytes in one megabyte.
        ''' </summary>
        Public Const Megabyte As Long = 1L << 20

        ''' <summary>
        ''' The number of bytes in one gigabyte.
        ''' </summary>
        Public Const Gigabyte As Long = 1L << 30

        ''' <summary>
        ''' The number of bytes in one terabyte.
        ''' </summary>
        Public Const Terabyte As Long = 1L << 40

        ''' <summary>
        ''' The number of bytes in one petabyte.
        ''' </summary>
        Public Const Petabyte As Long = 1L << 50

        ''' <summary>
        ''' The number of bytes in one exabyte.
        ''' </summary>
        Public Const Exabyte As Long = 1L << 60

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="ByteSizeFormatter"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Formats the specified byte size using the current culture.
        ''' </summary>
        ''' <param name="byteSize">The size in bytes.</param>
        ''' <returns>The formatted size, for example "700 bytes", "15 KB" or "4,4 GB".</returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="byteSize"/> is negative.
        ''' </exception>
        Public Shared Function Format(byteSize As Long) As String

            Return ByteSizeFormatter.Format(byteSize, CultureInfo.CurrentCulture)

        End Function

        ''' <summary>
        ''' Formats the specified byte size using the specified culture.
        ''' </summary>
        ''' <param name="byteSize">The size in bytes.</param>
        ''' <param name="culture">The culture used to format the numeric value.</param>
        ''' <returns>The formatted size, for example "700 bytes", "15 KB" or "4.4 GB".</returns>
        ''' <remarks>
        ''' Bytes and kilobytes are rounded to integer values;
        ''' megabytes and bigger units are rounded to one decimal digit.
        ''' </remarks>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="culture"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="byteSize"/> is negative.
        ''' </exception>
        Public Shared Function Format(byteSize As Long, culture As CultureInfo) As String

            If culture Is Nothing Then
                Throw New ArgumentNullException(NameOf(culture))
            End If
            If byteSize < 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(byteSize), byteSize, "Value cannot be negative.")
            End If

            If byteSize < Kilobyte Then
                Return $"{byteSize.ToString(culture)} bytes"

            ElseIf byteSize < Megabyte Then
                Return ByteSizeFormatter.FormatUnit(byteSize, Kilobyte, 0, "KB", culture)

            ElseIf byteSize < Gigabyte Then
                Return ByteSizeFormatter.FormatUnit(byteSize, Megabyte, 1, "MB", culture)

            ElseIf byteSize < Terabyte Then
                Return ByteSizeFormatter.FormatUnit(byteSize, Gigabyte, 1, "GB", culture)

            ElseIf byteSize < Petabyte Then
                Return ByteSizeFormatter.FormatUnit(byteSize, Terabyte, 1, "TB", culture)

            ElseIf byteSize < Exabyte Then
                Return ByteSizeFormatter.FormatUnit(byteSize, Petabyte, 1, "PB", culture)

            Else
                Return ByteSizeFormatter.FormatUnit(byteSize, Exabyte, 1, "EB", culture)

            End If

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Formats a byte size expressed in the specified unit.
        ''' </summary>
        ''' <param name="byteSize">The size in bytes.</param>
        ''' <param name="unitSize">The number of bytes of the unit.</param>
        ''' <param name="decimals">The number of decimal digits to round to.</param>
        ''' <param name="unitName">The unit name to append.</param>
        ''' <param name="culture">The culture used to format the numeric value.</param>
        ''' <returns>The formatted size.</returns>
        Private Shared Function FormatUnit(byteSize As Long, unitSize As Long, decimals As Integer,
                                           unitName As String, culture As CultureInfo) As String

            Dim value As Double = Math.Round(CDbl(byteSize) / CDbl(unitSize), decimals)
            Return $"{value.ToString(culture)} {unitName}"

        End Function

#End Region

    End Class

End Namespace

#End Region
