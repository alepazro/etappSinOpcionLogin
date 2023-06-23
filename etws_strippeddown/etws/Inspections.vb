Imports System.Runtime.Serialization

<DataContract> _
Public Class inspectionList

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

End Class

<DataContract> _
Public Class inspectionItem

    <DataMember> _
    Public listId As String = ""

    <DataMember> _
    Public itemId As String = ""

    <DataMember> _
    Public name As String = ""

End Class

<DataContract> _
Public Class inspectionLog

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public odometer As Integer = 0

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public listId As String = ""

    <DataMember> _
    Public itemId As String = ""

    <DataMember> _
    Public passed As Boolean = False

    <DataMember> _
    Public failed As Boolean = False

    <DataMember> _
    Public repaired As Boolean = False

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public lat As Decimal = 0

    <DataMember> _
    Public lng As Decimal = 0

End Class
