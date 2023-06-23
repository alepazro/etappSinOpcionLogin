Imports System.Runtime.Serialization

<DataContract()> _
Public Class geofenceClass

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public geofenceTypeId As Integer = 0

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public contactName As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public contactEmail As String = ""

    <DataMember> _
    Public contactSMSAlert As Boolean = False

    <DataMember> _
    Public contactEmailAlert As Boolean = False

    <DataMember> _
    Public contactAlertTypeId As Integer = 0

    <DataMember> _
    Public fullAddress As String = ""

    <DataMember> _
    Public street As String = ""

    <DataMember> _
    Public streetNumber As String = ""

    <DataMember> _
    Public route As String = ""

    <DataMember> _
    Public suite As String = ""

    <DataMember> _
    Public city As String = ""

    <DataMember> _
    Public county As String = ""

    <DataMember> _
    Public state As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public country As String = ""

    <DataMember> _
    Public latitude As Decimal = 0

    <DataMember> _
    Public longitude As Decimal = 0

    <DataMember> _
    Public geofenceAlertTypeId As Integer = 0

    <DataMember> _
    Public radius As Integer = 0

    <DataMember> _
    Public comments As String = ""

    <DataMember> _
    Public shapeId As Integer = 0

    <DataMember> _
    Public jsonPolyVerticesTXT As String = ""

    <DataMember> _
    Public KMLData As String = ""

    <DataMember> _
    Public SQLData As String = ""

    <DataMember> _
    Public isSpeedLimit As Boolean = False

    <DataMember> _
    Public speedLimit As Integer = 0

    <DataMember> _
    Public arrivalMsgId As Integer = 0

    <DataMember> _
    Public arrivalMsgTxt As String = ""

    <DataMember> _
    Public departureMsgId As Integer = 0

    <DataMember> _
    Public departureMsgTxt As String = ""

End Class

<DataContract()> _
Public Class geofenceType

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public companyId As Integer = 0

    <DataMember> _
    Public id As Integer = 0

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public iconId As Integer = 0

    <DataMember> _
    Public iconUrl As String = ""

End Class
