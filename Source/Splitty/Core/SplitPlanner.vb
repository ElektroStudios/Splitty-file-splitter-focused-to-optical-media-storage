' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="SplitPlanner.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Collections.Generic

#End Region

#Region " PlannedFile "

Namespace Core

    ''' <summary>
    ''' Represents a source file assigned to a specific disc.
    ''' </summary>
    Public NotInheritable Class PlannedFile

#Region " Properties "

        ''' <summary>
        ''' Gets the source file.
        ''' </summary>
        Public ReadOnly Property File As SourceFileEntry

        ''' <summary>
        ''' Gets the one-based number of the disc the file is assigned to.
        ''' </summary>
        Public ReadOnly Property DiscNumber As Integer

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="PlannedFile"/> class.
        ''' </summary>
        ''' <param name="file">The source file.</param>
        ''' <param name="discNumber">The one-based number of the disc the file is assigned to.</param>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="file"/> is <see langword="Nothing"/>.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="discNumber"/> is lower than 1.
        ''' </exception>
        Public Sub New(file As SourceFileEntry, discNumber As Integer)

            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If
            If discNumber < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(discNumber), discNumber, "Value must be greater than zero.")
            End If

            Me.File = file
            Me.DiscNumber = discNumber

        End Sub

#End Region

    End Class

End Namespace

#End Region

#Region " SplitPlanner "

Namespace Core

    ''' <summary>
    ''' Distributes a sequence of files among discs of a fixed capacity.
    ''' </summary>
    ''' <remarks>
    ''' Files are assigned sequentially, preserving their order:
    ''' a new disc is started when the next file does not fit in the remaining space of the current disc.
    ''' A file bigger than the disc capacity is placed alone in its own disc.
    ''' </remarks>
    Public NotInheritable Class SplitPlanner

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="SplitPlanner"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Assigns each file to a disc.
        ''' </summary>
        ''' <param name="files">The files to distribute, in the order they must be stored.</param>
        ''' <param name="capacityBytes">The capacity of a single disc, in bytes.</param>
        ''' <returns>The files with their assigned disc number, in the same order as <paramref name="files"/>.</returns>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="files"/> is <see langword="Nothing"/>, or contains a null element.
        ''' </exception>
        ''' <exception cref="ArgumentOutOfRangeException">
        ''' Thrown when <paramref name="capacityBytes"/> is lower than or equal to zero.
        ''' </exception>
        Public Shared Function Plan(files As IEnumerable(Of SourceFileEntry), capacityBytes As Long) As List(Of PlannedFile)

            If files Is Nothing Then
                Throw New ArgumentNullException(NameOf(files))
            End If
            If capacityBytes <= 0L Then
                Throw New ArgumentOutOfRangeException(NameOf(capacityBytes), capacityBytes, "Value must be greater than zero.")
            End If

            Dim result As New List(Of PlannedFile)()
            Dim discNumber As Integer = 1
            Dim usedBytes As Long = 0L
            Dim discHasFiles As Boolean = False

            For Each file As SourceFileEntry In files
                If file Is Nothing Then
                    Throw New ArgumentNullException(NameOf(files), "The collection contains a null element.")
                End If

                ' Compared as "remaining space" to avoid any possible overflow of (usedBytes + file.Length).
                Dim fits As Boolean = file.Length <= (capacityBytes - usedBytes)
                If discHasFiles AndAlso Not fits Then
                    discNumber += 1
                    usedBytes = 0L
                End If

                result.Add(New PlannedFile(file, discNumber))
                usedBytes += Math.Min(file.Length, capacityBytes)
                discHasFiles = True
            Next

            Return result

        End Function

        ''' <summary>
        ''' Gets the number of discs used by the specified plan.
        ''' </summary>
        ''' <param name="plan">The plan returned by <see cref="SplitPlanner.Plan"/>.</param>
        ''' <returns>The number of discs used; zero if the plan is empty.</returns>
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="plan"/> is <see langword="Nothing"/>.
        ''' </exception>
        Public Shared Function GetDiscCount(plan As IList(Of PlannedFile)) As Integer

            If plan Is Nothing Then
                Throw New ArgumentNullException(NameOf(plan))
            End If
            If plan.Count = 0 Then
                Return 0
            End If
            Return plan(plan.Count - 1).DiscNumber

        End Function

#End Region

    End Class

End Namespace

#End Region
