Imports System.Runtime.Serialization

<DataContract()> _
Public Class inventory

    <DataMember()> _
    Public id As String = ""

    <DataMember()> _
    Public deviceId As String = ""

    <DataMember()> _
    Public deviceTypeName As String = ""

    <DataMember()> _
    Public serialNumber As String = ""

    <DataMember()> _
    Public imei As String = ""

    <DataMember()> _
    Public simNo As String = ""

    <DataMember()> _
    Public simAreaCode As String = ""

    <DataMember()> _
    Public simPhoneNumber As String = ""

    <DataMember()> _
    Public createdOn As String = ""

    <DataMember()> _
    Public lastUpdatedOn As String = ""

    <DataMember()> _
    Public eventDate As String = ""

End Class

<DataContract()> _
Public Class assignedInventory

    <DataMember()> _
    Public custId As String = ""

    <DataMember()> _
    Public inventory As New List(Of String)

    <DataMember()> _
    Public emailTypeId As String = ""

    <DataMember()> _
    Public courrierId As String = ""

    <DataMember()> _
    Public trackingNumber As String = ""

End Class

