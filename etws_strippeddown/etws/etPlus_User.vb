Imports System.Runtime.Serialization

<DataContract> _
Public Class etPlus_User

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public result As Integer = 0

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public cellPhone As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public login As String = ""

    <DataMember> _
    Public password As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public tokenValidUntil As String = ""

End Class

<DataContract> _
Public Class etPlus_UserGUID

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public result As Integer = 0

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public userGUID As String = ""

End Class
