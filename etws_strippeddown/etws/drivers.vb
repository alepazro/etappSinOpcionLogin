Imports System.Runtime.Serialization

<DataContract> _
Public Class driverLog

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public driverStatus As String = ""

    <DataMember> _
    Public startDate As String = ""

    <DataMember> _
    Public endDate As String = ""

    <DataMember> _
    Public durationMins As String = ""

    <DataMember> _
    Public address As String = ""

    <DataMember> _
    Public stateId As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public odometer As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class
