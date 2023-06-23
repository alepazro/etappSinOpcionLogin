Imports System.Runtime.Serialization

<DataContract>
Public Class BrokerHasTracking
    <DataMember>
    Public BrokerNumber As String
    <DataMember>
    Public Pickupdetetime As String
    <DataMember>
    Public Deliverydatetime As String
    <DataMember>
    Public TrackingWasSent As Boolean
    <DataMember>
    Public ExistTraking As Boolean
End Class
