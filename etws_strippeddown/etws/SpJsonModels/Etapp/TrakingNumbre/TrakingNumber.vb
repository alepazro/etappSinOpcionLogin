
Imports System.Runtime.Serialization

<DataContract>
Public Class TrakingNumber
    <DataMember>
    Public ID As Integer = 0
    <DataMember>
    Public TypeID As Integer = 0
    <DataMember>
    Public CompanyID As Integer = 0
    <DataMember>
    Public Device As String = ""
    <DataMember>
    Public DeviceName As String = ""
    <DataMember>
    Public DeviceID As Integer = 0
    <DataMember>
    Public TrackingNumber As String = ""
    <DataMember>
    Public CreatedOn As String = ""
    <DataMember>
    Public UserID As Integer = 0
    <DataMember>
    Public WorkOrderID As Integer = 0
    <DataMember>
    Public SendTo As String = ""
    <DataMember>
    Public Flag_Sent As Boolean = 0
    <DataMember>
    Public SentOn As String = ""
    <DataMember>
    Public ValidUntil As String = ""
    <DataMember>
    Public Flag_Expired As Boolean = 0
    <DataMember>
    Public UTC_Code As String = ""
    <DataMember>
    Public UTC_Convertion As Decimal = 0.0
    <DataMember>
    Public Message As String = ""
    <DataMember>
    Public FreezeMap As Boolean = 0
    <DataMember>
    Public FreezeLat As Decimal = 0.0
    <DataMember>
    Public FreezeLon As Decimal = 0.0
    <DataMember>
    Public ZoomLevel As Integer = 0
    <DataMember>
    Public MapType As String = ""
    <DataMember>
    Public MapIcon As String = ""
    <DataMember>
    Public URLTraking As String = ""
    <DataMember>
    Public Lat As Decimal = 0.0
    <DataMember>
    Public Lng As Decimal = 0.0
    <DataMember>
    Public Flat_FromJob As Boolean = False
    <DataMember>
    Public GeofenceTargetJob As Integer = 0
    <DataMember>
    Public JobUniqueKey As String = 0
    <DataMember>
    Public Flat_FromBrokerOrder As Boolean = False

End Class
