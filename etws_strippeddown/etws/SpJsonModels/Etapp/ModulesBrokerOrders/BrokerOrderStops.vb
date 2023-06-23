Imports System.Runtime.Serialization

<DataContract()>
Public Class BrokerOrderStops
    <DataMember>
    Public ID As Integer
    <DataMember>
    Public BrokerOrderID As Integer
    <DataMember>
    Public PickupAddress As String
    <DataMember>
    Public Pickupdetetime As DateTime
    <DataMember>
    Public PickupAddresscoordinatesLat As Double
    <DataMember>
    Public PickupAddresscoordinatesLng As Double
    <DataMember>
    Public DeliveryAddress As String
    <DataMember>
    Public Deliverydatetime As DateTime
    <DataMember>
    Public DeliveryAddressscoordinatesLat As Double
    <DataMember>
    Public DeliveryAddressscoordinatesLng As Double
    <DataMember>
    Public Observations As String
    <DataMember>
    Public CreateOn As DateTime
    <DataMember>
    Public CreatedBy As Integer
    <DataMember>
    Public StatusID As Integer
End Class
