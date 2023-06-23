Imports System.Runtime.Serialization

<DataContract>
Public Class BrokerDevices
    <DataMember>
    Public ID As Integer
    <DataMember>
    Public DeviceID As String
    <DataMember>
    Public ShortName As String
    <DataMember>
    Public Name As String
    <DataMember>
    Public LastUpdatedOn As String
    <DataMember>
    Public EventCode As String
    <DataMember>
    Public EventName As String
    <DataMember>
    Public EventDate As String
    <DataMember>
    Public Speed As String
    <DataMember>
    Public GPSStatus As String
    <DataMember>
    Public GPSAge As String
    <DataMember>
    Public DriverID As Integer
    <DataMember>
    Public DriverName As String
    <DataMember>
    Public FullAddress As String
    <DataMember>
    Public DriverPhone As String
    <DataMember>
    Public IconID As Integer
    <DataMember>
    Public IconURL As String
    <DataMember>
    Public TextColor As String
    <DataMember>
    Public BgndColor As String
    <DataMember>
    Public CountBrokers As Integer
End Class
