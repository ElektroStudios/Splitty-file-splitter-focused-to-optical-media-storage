' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' Modified : 22-December-2014
' ***********************************************************************
' <copyright file="About.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

'Option Explicit On
'Option Strict On
'Option Infer Off

#Region " Imports "

Imports Splitty

#End Region

#Region " About "

''' <summary>
''' The About Form.
''' </summary>
Public Class About

    ''' <summary>
    ''' Handles the Load event of the About Form.
    ''' </summary>
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    Private Sub About_Load(sender As Object, e As EventArgs) _
    Handles MyBase.Load

        Me.Text = My.Settings.Version
        Me.TextBoxDescription.Text = My.Resources.ResourceManager.GetObject(Main.LanguageResource & CStr(10))

    End Sub

    ''' <summary>
    ''' Handles the ClickButtonArea event of the CButton1 control.
    ''' </summary>
    ''' <param name="Sender">The source of the event.</param>
    ''' <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
    Private Sub CButton1_ClickButtonArea(Sender As Object, e As MouseEventArgs) _
    Handles CButton1.ClickButtonArea

        Me.Close()

    End Sub

End Class

#End Region