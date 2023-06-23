Imports System.Runtime.Serialization

<DataContract()>
Public Class BrokerOrder
    <DataMember>
    Public ID As Integer
    <DataMember>
    Public CompanyID As Integer
    <DataMember>
    Public DeviceID As Integer
    <DataMember>
    Public DriverID As Integer
    <DataMember>
    Public TrakingID As Integer
    <DataMember>
    Public Name As String
    <DataMember>
    Public BrokerNumber As String
    <DataMember>
    Public PickupAddress As String
    <DataMember>
    Public Pickupdetetime As String
    <DataMember>
    Public PickupAddresscoordinatesLat As Decimal
    <DataMember>
    Public PickupAddresscoordinatesLng As Decimal
    <DataMember>
    Public DeliveryAddress As String
    <DataMember>
    Public Deliverydatetime As String
    <DataMember>
    Public DeliveryAddressscoordinatesLat As Decimal
    <DataMember>
    Public DeliveryAddressscoordinatesLng As Decimal
    <DataMember>
    Public Observaciones As String
    <DataMember>
    Public StatusID As Integer
    <DataMember>
    Public CreateOn As String
    <DataMember>
    Public CreatedBy As Integer
    <DataMember>
    Public EmailSent As String
    <DataMember>
    Public EmailLogo As String
    <DataMember>
    Public EmailTo As String
    <DataMember>
    Public HasEmail As String
    <DataMember>
    Public TrackingWasSent As Boolean
    <DataMember>
    Public TrackingWasSentDate As String
    <DataMember>
    Public HasSms As Boolean
    <DataMember>
    Public SmsPhone As String
End Class
