' ***********************************************************************
' Assembly : Splitty.Tests
' Author   : Elektro
' ***********************************************************************
' <copyright file="LocalizedResourcesTests.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.Collections.Generic
Imports System.Resources
Imports System.Text.RegularExpressions

Imports Microsoft.VisualStudio.TestTools.UnitTesting

#End Region

#Region " LocalizedResourcesTests "

''' <summary>
''' Unit tests for the localized UI strings ("ENGxx" and "SPAxx" resources) of the application.
''' </summary>
<TestClass>
Public NotInheritable Class LocalizedResourcesTests

#Region " Constants "

    ''' <summary>
    ''' The highest resource string index used by the application.
    ''' </summary>
    Private Const LastResourceIndex As Integer = 34

    ''' <summary>
    ''' The language prefixes used by the application.
    ''' </summary>
    Private Shared ReadOnly LanguagePrefixes As String() = {"ENG", "SPA"}

#End Region

#Region " Tests "

    ''' <summary>
    ''' Verifies that every resource string exists, and is not empty, in every language.
    ''' </summary>
    <TestMethod>
    Public Sub AllResourceStrings_ExistInAllLanguages()

        Dim manager As ResourceManager = Global.Splitty.My.Resources.ResourceManager
        For index As Integer = 1 To LastResourceIndex
            For Each prefix As String In LanguagePrefixes
                Dim name As String = $"{prefix}{index:00}"
                Dim value As String = manager.GetString(name)
                Assert.IsNotNull(value, $"Missing resource string: {name}")
                Assert.IsFalse(String.IsNullOrWhiteSpace(value), $"Empty resource string: {name}")
            Next
        Next

    End Sub

    ''' <summary>
    ''' Verifies that the translations use the same composite format placeholders ({0}, {1}, ...),
    ''' so they can be safely used with the same format arguments.
    ''' </summary>
    <TestMethod>
    Public Sub AllResourceStrings_HaveSamePlaceholdersInAllLanguages()

        Dim manager As ResourceManager = Global.Splitty.My.Resources.ResourceManager
        For index As Integer = 1 To LastResourceIndex
            Dim english As String = manager.GetString($"ENG{index:00}")
            Dim spanish As String = manager.GetString($"SPA{index:00}")
            CollectionAssert.AreEquivalent(LocalizedResourcesTests.GetPlaceholders(english),
                                           LocalizedResourcesTests.GetPlaceholders(spanish),
                                           $"Placeholders differ in resource index {index:00}.")
        Next

    End Sub

    ''' <summary>
    ''' Verifies that the progress bar format strings can be formatted with three arguments
    ''' (current value, percent and maximum), as the progress bar control does.
    ''' </summary>
    ''' <param name="index">The resource string index.</param>
    <TestMethod>
    <DataRow(6)>
    <DataRow(8)>
    <DataRow(27)>
    Public Sub ProgressFormatStrings_AreValidCompositeFormats(index As Integer)

        Dim manager As ResourceManager = Global.Splitty.My.Resources.ResourceManager
        For Each prefix As String In LanguagePrefixes
            Dim format As String = manager.GetString($"{prefix}{index:00}")
            Dim result As String = String.Format(format, 1, 50, 2)
            Assert.IsFalse(result.Contains("{"), $"Unformatted placeholder in {prefix}{index:00}: {result}")
        Next

    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Gets the distinct composite format placeholders of a string.
    ''' </summary>
    ''' <param name="value">The string.</param>
    ''' <returns>The placeholder indices.</returns>
    Private Shared Function GetPlaceholders(value As String) As List(Of String)

        Dim result As New List(Of String)()
        For Each match As Match In Regex.Matches(value, "\{(\d+)\}")
            If Not result.Contains(match.Groups(1).Value) Then
                result.Add(match.Groups(1).Value)
            End If
        Next
        Return result

    End Function

#End Region

End Class

#End Region
