Imports System.Runtime.Serialization

<DataContract> Public Class class_DeviceHistory

    <DataMember> Public deviceId As String = ""
    <DataMember> Public eventName As String = ""
    <DataMember> Public eventDate As String = ""
    <DataMember> Public geofenceName As String = ""
    <DataMember> Public driverFirstName As String = ""
    <DataMember> Public driverLastName As String = ""
    <DataMember> Public createdOn As String = ""

End Class

Public Class quickMsgDriver
    Public id As String = ""
    Public name As String = ""
    Public phone As String = ""
    Public email As String = ""
    Public deviceId As String = ""
    Public deviceName As String = ""
End Class

Public Class quickMsg
    Public driverId As String = ""
    Public channel As Integer = 0
    Public message As String = ""
End Class

<DataContract()> Public Class etwsResponse
    <DataMember> Public IsOk As Boolean = True
    <DataMember> Public Msg As String = ""
End Class

<DataContract()> Public Class class_parsedMessage
    <DataMember> Public parserId As Integer = 0
    <DataMember> Public workerId As Integer = 0
    <DataMember> Public serverIP As String = ""
    <DataMember> Public port As Integer = 0
    <DataMember> Public deviceFamily As String = ""
    <DataMember> Public isBrief As Boolean = False
    <DataMember> Public SerialNumber As String = ""
    <DataMember> Public EventCode As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public Latitude As Double = 0
    <DataMember> Public Longitude As Double = 0
    <DataMember> Public IgnitionStatus As Integer = -1
    <DataMember> Public Speed As Decimal = 0
    <DataMember> Public decHeading As Decimal = 0
    <DataMember> Public Heading As String = ""
    <DataMember> Public originalEvent As String = ""
    <DataMember> Public GPSAge As Integer = 0
    <DataMember> Public GPSStatus As Integer = -1
    <DataMember> Public Odometer As Decimal = 0
    <DataMember> Public IOStatus As Decimal = -1
    <DataMember> Public Consecutive As Integer = 0
    <DataMember> Public Relays As Integer
    <DataMember> Public rssi As Integer = 0
    <DataMember> Public SW1 As Boolean = False
    <DataMember> Public SW2 As Boolean = False
    <DataMember> Public SW3 As Boolean = False
    <DataMember> Public SW4 As Boolean = False
    <DataMember> Public relay1 As Boolean = False
    <DataMember> Public relay2 As Boolean = False
    <DataMember> Public relay3 As Boolean = False
    <DataMember> Public relay4 As Boolean = False
    <DataMember> Public hasTemperature As Boolean = False
    <DataMember> Public temperature1 As Integer = 0
    <DataMember> Public temperature2 As Integer = 0
    <DataMember> Public temperature3 As Integer = 0
    <DataMember> Public temperature4 As Integer = 0
    <DataMember> Public iButtonID As String = ""
    <DataMember> Public ExtraData As String = ""
    <DataMember> Public QtySats As Integer = 0
End Class

<DataContract()> Public Class etAddCrmMessageRequest
    <DataMember> Public Token As String = ""
    <DataMember> Public DeviceId As String = ""
    <DataMember> Public Subject As String = ""
    <DataMember> Public Note As String = ""
End Class

