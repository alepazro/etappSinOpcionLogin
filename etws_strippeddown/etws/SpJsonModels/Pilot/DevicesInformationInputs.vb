Imports System.Runtime.Serialization

<DataContract>
Public Class DevicesInformationInputs
    <DataMember>
    Public ID As Integer
    <DataMember>
    Public CompanyID As Integer
    <DataMember>
    Public DeviceID As String = ""
    <DataMember>
    Public Name As String = ""
    <DataMember>
    Public Input1Name As String = ""
    <DataMember>
    Public Input2Name As String = ""
    <DataMember>
    Public Input3Name As String = ""
    <DataMember>
    Public Input4Name As String = ""
    <DataMember>
    Public Output1Name As String = ""
    <DataMember>
    Public Output2Name As String = ""
    <DataMember>
    Public Output3Name As String = ""
    <DataMember>
    Public Output4Name As String = ""
    <DataMember>
    Public Input1 As Boolean = False
    <DataMember>
    Public Input2 As Boolean = False
    <DataMember>
    Public Input3 As Boolean = False
    <DataMember>
    Public Input4 As Boolean = False
    <DataMember>
    Public Output1 As Boolean = False
    <DataMember>
    Public Output2 As Boolean = False
    <DataMember>
    Public Output3 As Boolean = False
    <DataMember>
    Public Output4 As Boolean = False
    <DataMember>
    Public Temperatures As New List(Of Temperature)
    <DataMember>
    Public AngInput As Decimal = 0.0
    <DataMember>
    Public DigInput As Integer = 0
    <DataMember>
    Public Output As Integer = 0


End Class
