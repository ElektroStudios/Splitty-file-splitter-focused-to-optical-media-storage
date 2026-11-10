' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="DiscCapacity.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " DiscType "

Namespace Core

    ''' <summary>
    ''' Specifies the optical disc formats supported by Splitty.
    ''' </summary>
    Public Enum DiscType As Integer

        ''' <summary>
        ''' Standard CD (700 MB).
        ''' </summary>
        CD = 0

        ''' <summary>
        ''' Overburned CD (800 MB).
        ''' </summary>
        CD800MB = 1

        ''' <summary>
        ''' Overburned CD (900 MB).
        ''' </summary>
        CD900MB = 2

        ''' <summary>
        ''' Single layer DVD (DVD-5).
        ''' </summary>
        Dvd5 = 3

        ''' <summary>
        ''' Double layer DVD (DVD-9).
        ''' </summary>
        Dvd9 = 4

        ''' <summary>
        ''' Double sided DVD (DVD-10).
        ''' </summary>
        Dvd10 = 5

        ''' <summary>
        ''' Single layer Blu-ray.
        ''' </summary>
        BR = 6

        ''' <summary>
        ''' Double layer Blu-ray.
        ''' </summary>
        BRDoubleLayer = 7

        ''' <summary>
        ''' Single layer Blu-ray mini disc.
        ''' </summary>
        BRMiniDisc = 8

        ''' <summary>
        ''' Double layer Blu-ray mini disc.
        ''' </summary>
        BRMiniDiscDoubleLayer = 9

    End Enum

End Namespace

#End Region

#Region " DiscCapacity "

Namespace Core

    ''' <summary>
    ''' Provides the storage capacities of the supported optical disc formats,
    ''' and disc count calculations based on those capacities.
    ''' </summary>
    Public NotInheritable Class DiscCapacity

#Region " Constants "

        ''' <summary>
        ''' Extra bytes reserved per disc for the ISO 9660 file system structures
        ''' when the split mode is ISO image creation.
        ''' </summary>
        Public Const IsoOverheadBytes As Long = 93491

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="DiscCapacity"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Gets the storage capacity, in bytes, of the specified disc format.
        ''' </summary>
        ''' <param name="discType">The disc format.</param>
        ''' <returns>The storage capacity in bytes.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="discType"/> is not a defined <see cref="DiscType"/> value.
        ''' </exception>
        Public Shared Function GetCapacity(discType As DiscType) As Long

            Select Case discType
                Case DiscType.CD : Return 734003200L
                Case DiscType.CD800MB : Return 829440393L
                Case DiscType.CD900MB : Return 912383803L
                Case DiscType.Dvd5 : Return 4700000000L
                Case DiscType.Dvd9 : Return 8500000000L
                Case DiscType.Dvd10 : Return 9395240960L
                Case DiscType.BR : Return 25025314816L
                Case DiscType.BRDoubleLayer : Return 50050629632L
                Case DiscType.BRMiniDisc : Return 7791181824L
                Case DiscType.BRMiniDiscDoubleLayer : Return 15582363648L
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(discType), discType, $"Unsupported disc type: {CInt(discType)}")
            End Select

        End Function

        ''' <summary>
        ''' Calculates the minimum number of discs needed to store the specified amount of data.
        ''' </summary>
        ''' <param name="totalBytes">The total amount of data, in bytes.</param>
        ''' <param name="capacityBytes">The capacity of a single disc, in bytes.</param>
        ''' <returns>The number of discs needed. Returns zero when <paramref name="totalBytes"/> is zero.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="totalBytes"/> is negative,
        ''' or when <paramref name="capacityBytes"/> is lower than or equal to zero.
        ''' </exception>
        Public Shared Function CalculateDiscCount(totalBytes As Long, capacityBytes As Long) As Long

            If totalBytes < 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(totalBytes), totalBytes, "Value cannot be negative.")
            End If
            If capacityBytes <= 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(capacityBytes), capacityBytes, "Value must be greater than zero.")
            End If

            ' Integer ceiling division, avoids floating point precision loss with huge values.
            Dim quotient As Long = totalBytes \ capacityBytes
            If totalBytes Mod capacityBytes <> 0L Then
                quotient += 1L
            End If
            Return quotient

        End Function

#End Region

    End Class

End Namespace

#End Region
