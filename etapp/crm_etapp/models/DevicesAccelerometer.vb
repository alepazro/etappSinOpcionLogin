
Imports System.Runtime.Serialization

<DataContract>
Public Class DevicesAccelerometer
    <DataMember>
    Public ID As Integer = 0
    <DataMember>
    Public DeviceID As String = ""
    <DataMember>
    Public deviceName As String = ""
    <DataMember>
    Public CompanyID As String = ""
    <DataMember>
    Public CompanyName As String = ""
    <DataMember>
    Public currentConfiguration As String = ""
    <DataMember>
    Public configurationToSend As String = ""
    <DataMember>
    Public VehicleType As String = ""
End Class

