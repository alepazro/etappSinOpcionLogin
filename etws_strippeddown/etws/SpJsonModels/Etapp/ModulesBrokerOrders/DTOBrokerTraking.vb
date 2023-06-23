Public Class DTOBrokerTraking
    Public BrokerNumber
    Public TrakingNumber As String
    Public TrakingURL As String
    Public PickupAddress As String
    Public PickupDatetime As String
    Public DeliveryAddress As String
    Public DeliveryDatetime As String
    Public DeliveryLatitud As Decimal
    Public DeliveryLongitud As Decimal
    Public PickupLatitude As Decimal
    Public PickupLongitude As Decimal
    Public GUID As String
    Public FlatExpired As Boolean
    Public IconsStops As Boolean
    Public IconsMarker1 As Boolean
    Public IconsMarker2 As Boolean
    Public Latitude As Decimal
    Public Longitude As Decimal
    Public Name As String
    Public IconDevice As String
    Public Stops As New List(Of DTOBrokerOrderStop)
    Public Observations As String
End Class
