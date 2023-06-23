Imports System.Runtime.Serialization

<DataContract> _
Public Class responseOk

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public transId As String = ""

    <DataMember> _
    Public msg As String = ""

End Class

<DataContract> _
Public Class googURL

    <DataMember> _
    Public _url As String = ""

End Class

<DataContract> _
Public Class googSig

    <DataMember> _
    Public sig As String = ""

End Class

<DataContract> _
Public Class engagementTick

    <DataMember> _
    Public transId As String = ""

    <DataMember> _
    Public delay As Integer

End Class