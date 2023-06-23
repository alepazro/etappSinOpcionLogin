Imports System.Runtime.Serialization


<DataContract>
Public Class hGeofenceEvent

    <DataMember> Public deviceId As String = ""
    <DataMember> Public deviceName As String = ""
    <DataMember> Public eventType As String = "" 'IN/OUT
    <DataMember> Public geofenceName As String = ""
    <DataMember> Public eventDate As String = ""
    <DataMember> Public iButtonRaw As String = ""
    <DataMember> Public driverFirstName As String = ""
    <DataMember> Public driverLastName As String = ""

End Class

<DataContract>
Public Class hGeofencesResponse

    <DataMember> Public requestId As String = ""
    <DataMember> Public events As New List(Of hGeofenceEvent)
    <DataMember> Public msg As String = ""
    <DataMember> Public isOk As Boolean = True

End Class