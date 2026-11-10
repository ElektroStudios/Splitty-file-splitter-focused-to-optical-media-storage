' ***********************************************************************
' Assembly : Splitty
' Author   : Elektro
' ***********************************************************************
' <copyright file="FolderLock.vb" company="Elektro Studios">
'     Copyright (c) Elektro Studios. All rights reserved.
' </copyright>
' ***********************************************************************

Option Strict On
Option Explicit On
Option Infer Off

#Region " Imports "

Imports System.IO
Imports System.Security.AccessControl
Imports System.Security.Principal

#End Region

#Region " FolderLock "

Namespace Core

    ''' <summary>
    ''' Temporarily prevents the current user from modifying the contents of a directory,
    ''' by adding (and later removing) a single "deny" access rule to the existing ACL of the directory.
    ''' </summary>
    ''' <remarks>
    ''' The existing access rules of the directory are preserved: only the rule added by
    ''' <see cref="FolderLock.Lock"/> is removed by <see cref="FolderLock.Unlock"/>.
    ''' </remarks>
    Public NotInheritable Class FolderLock

#Region " Properties "

        ''' <summary>
        ''' Gets the rights denied to the current user while a directory is locked:
        ''' creating, writing and deleting files and subdirectories.
        ''' <para></para>
        ''' Reading, listing and traversing the directory remain allowed.
        ''' </summary>
        Public Shared ReadOnly Property DeniedRights As FileSystemRights
            Get
                Return FileSystemRights.WriteData Or               ' Also CreateFiles.
                       FileSystemRights.AppendData Or              ' Also CreateDirectories.
                       FileSystemRights.WriteAttributes Or
                       FileSystemRights.WriteExtendedAttributes Or
                       FileSystemRights.Delete Or
                       FileSystemRights.DeleteSubdirectoriesAndFiles
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Prevents a default instance of the <see cref="FolderLock"/> class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Adds a rule to the ACL of the specified directory that denies <see cref="FolderLock.DeniedRights"/>
        ''' to the current user, inherited by all its files and subdirectories.
        ''' </summary>
        ''' <param name="directoryPath">The directory to lock.</param>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="directoryPath"/> is null or empty.
        ''' </exception>
        ''' <exception cref="DirectoryNotFoundException">
        ''' Thrown when <paramref name="directoryPath"/> does not exist.
        ''' </exception>
        Public Shared Sub Lock(directoryPath As String)

            Dim directory As DirectoryInfo = FolderLock.GetExistingDirectory(directoryPath)
            Dim security As DirectorySecurity = directory.GetAccessControl(AccessControlSections.Access)
            security.AddAccessRule(FolderLock.CreateDenyRule())
            directory.SetAccessControl(security)

        End Sub

        ''' <summary>
        ''' Removes the rule added by <see cref="FolderLock.Lock"/> from the ACL of the specified directory.
        ''' <para></para>
        ''' It does nothing if the directory is not locked.
        ''' </summary>
        ''' <param name="directoryPath">The directory to unlock.</param>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="directoryPath"/> is null or empty.
        ''' </exception>
        ''' <exception cref="DirectoryNotFoundException">
        ''' Thrown when <paramref name="directoryPath"/> does not exist.
        ''' </exception>
        Public Shared Sub Unlock(directoryPath As String)

            Dim directory As DirectoryInfo = FolderLock.GetExistingDirectory(directoryPath)
            Dim security As DirectorySecurity = directory.GetAccessControl(AccessControlSections.Access)
            security.RemoveAccessRuleSpecific(FolderLock.CreateDenyRule())
            directory.SetAccessControl(security)

        End Sub

        ''' <summary>
        ''' Determines whether the specified directory has the rule added by <see cref="FolderLock.Lock"/>.
        ''' </summary>
        ''' <param name="directoryPath">The directory to check.</param>
        ''' <returns><see langword="True"/> if the directory is locked; otherwise, <see langword="False"/>.</returns>
        ''' <exception cref="ArgumentException">
        ''' Thrown when <paramref name="directoryPath"/> is null or empty.
        ''' </exception>
        ''' <exception cref="DirectoryNotFoundException">
        ''' Thrown when <paramref name="directoryPath"/> does not exist.
        ''' </exception>
        Public Shared Function IsLocked(directoryPath As String) As Boolean

            Dim directory As DirectoryInfo = FolderLock.GetExistingDirectory(directoryPath)
            Dim security As DirectorySecurity = directory.GetAccessControl(AccessControlSections.Access)
            Dim currentUser As SecurityIdentifier = FolderLock.GetCurrentUser()

            For Each rule As FileSystemAccessRule In security.GetAccessRules(True, False, GetType(SecurityIdentifier))
                If rule.AccessControlType = AccessControlType.Deny AndAlso
                   currentUser.Equals(rule.IdentityReference) AndAlso
                   (rule.FileSystemRights And FolderLock.DeniedRights) = FolderLock.DeniedRights Then
                    Return True
                End If
            Next
            Return False

        End Function

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Creates the deny rule applied by <see cref="FolderLock.Lock"/>.
        ''' </summary>
        ''' <returns>The access rule.</returns>
        Private Shared Function CreateDenyRule() As FileSystemAccessRule

            Return New FileSystemAccessRule(FolderLock.GetCurrentUser(),
                                            FolderLock.DeniedRights,
                                            InheritanceFlags.ContainerInherit Or InheritanceFlags.ObjectInherit,
                                            PropagationFlags.None,
                                            AccessControlType.Deny)

        End Function

        ''' <summary>
        ''' Gets the security identifier of the current Windows user.
        ''' </summary>
        ''' <returns>The security identifier.</returns>
        Private Shared Function GetCurrentUser() As SecurityIdentifier

            Using identity As WindowsIdentity = WindowsIdentity.GetCurrent()
                If identity.User Is Nothing Then
                    Throw New InvalidOperationException("Cannot determine the security identifier of the current user.")
                End If
                Return identity.User
            End Using

        End Function

        ''' <summary>
        ''' Validates the specified directory path and returns its <see cref="DirectoryInfo"/>.
        ''' </summary>
        ''' <param name="directoryPath">The directory path.</param>
        ''' <returns>The <see cref="DirectoryInfo"/> of the existing directory.</returns>
        Private Shared Function GetExistingDirectory(directoryPath As String) As DirectoryInfo

            If String.IsNullOrEmpty(directoryPath) Then
                Throw New ArgumentException("Value cannot be null or empty.", NameOf(directoryPath))
            End If

            Dim directory As New DirectoryInfo(directoryPath)
            If Not directory.Exists Then
                Throw New DirectoryNotFoundException($"Directory not found: {directoryPath}")
            End If
            Return directory

        End Function

#End Region

    End Class

End Namespace

#End Region
