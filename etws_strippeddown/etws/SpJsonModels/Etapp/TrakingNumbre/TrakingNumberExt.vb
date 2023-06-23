
Imports System.Runtime.Serialization

<DataContract>
Public Class TrakingNumberExt
    <DataMember>
    Public Device As String = ""
    <DataMember>
    Public TrackingNumber As String = ""
    <DataMember>
    Public ValidUntil As String = ""
    <DataMember>
    Public Observations As String = ""
    <DataMember>
    Public Lat As Decimal = 0.0
    <DataMember>
    Public Lng As Decimal = 0.0
    <DataMember>
    Public Icon As String = ""
    <DataMember>
    Public Message As String = ""
    <DataMember>
    Public MapIcon As String = ""
    <DataMember>
    Public Flag_Expired As Boolean = False
    <DataMember>
    Public lattarget As Decimal = 0.0
    <DataMember>
    Public longtarget As Decimal = 0.0
    <DataMember>
    Public icontarget As String = ""
    <DataMember>
    Public Flat_FromJob As Boolean = False
    <DataMember>
    Public Flat_FromBrokerOrder As Boolean
    <DataMember>
    Public PickupAddress As String = ""
    <DataMember>
    Public Pickupdetetime As String = ""
    <DataMember>
    Public PickupAddresscoordinatesLat As Decimal = 0.0
    <DataMember>
    Public PickupAddresscoordinatesLng As Decimal = 0.0
    <DataMember>
    Public DeliveryAddress As String = ""
    <DataMember>
    Public Deliverydatetime As String = ""
    <DataMember>
    Public DeliveryAddressscoordinatesLat As Decimal = 0.0
    <DataMember>
    Public DeliveryAddressscoordinatesLng As Decimal = 0.0
End Class
