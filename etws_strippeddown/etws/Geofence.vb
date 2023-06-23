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

    <DataMember>
    Public departureMsgTxt As String = ""

    <DataMember>
    Public IsStopForJob As Boolean = False

End Class

Public Class Geofence
    Public apiToken As String = ""
    Public isOk As Boolean
    Public msg As String = ""

    Public geofenceId As String = ""
    Public name As String = ""
    Public latitude As Decimal = 0
    Public longitude As Decimal = 0
    Public radius As Integer = 0
    Public address As String = ""
    Public alertStatusId As Integer = 0
    Public speedLimit As Integer = 0
    Public speedLimitEnabled As Boolean = False
    Public geofenceTypeId As String = ""
End Class

Public Class GeofenceAlertStatus
    Public apiToken As String = ""
    Public isOk As Boolean
    Public msg As String = ""

    Public geofenceId As String = ""
    Public alertStatusID As Integer = -1
End Class

Public Class GeofenceCrossings
    Public apiToken As String = ""
    Public isOk As Boolean
    Public msg As String = ""

    Public geofenceId As String = ""
    Public vehicleId As String = ""
    Public dateFrom As String = ""
    Public dateTo As String = ""
End Class

Public Class GeofenceSpeedLimitStatus
    Public apiToken As String = ""
    Public isOk As Boolean
    Public msg As String = ""

    Public geofenceId As String = ""
    Public isEnabled As Boolean = False
    Public speedLimit As Integer = 0

End Class

<DataContract()> _
Public Class GeofenceRequest

    <DataMember> _
    Public apiToken As String = ""

    <DataMember> _
    Public geofenceId As String = ""
End Class

<DataContract()> _
Public Class GeofenceResponse

    <DataMember> _
    Public apiToken As String = ""

    <DataMember> _
    Public geofenceId As String = ""

    <DataMember> _
    Public isOk As Boolean

    <DataMember> _
    Public msg As String = ""

End Class

<DataContract()> _
Public Class GeofenceAlertTypeRequest

    <DataMember> _
    Public apiToken As String = ""

    <DataMember> _
    Public geofenceId As String = ""

    <DataMember> _
    Public alertTypeId As Integer

End Class

<DataContract()> _
Public Class GeofenceSpeedLimitRequest

    <DataMember> _
    Public apiToken As String = ""

    <DataMember> _
    Public geofenceId As String = ""

    <DataMember> _
    Public isEnabled As Boolean = False

    <DataMember> _
    Public speedLimit As Integer = 0

End Class

<DataContract()> _
Public Class geofenceUpdate
    <DataMember> _
    Public apiToken As String = ""
    <DataMember> _
    Public geofenceId As String = ""
    <DataMember> _
    Public geofenceName As String = ""
    <DataMember> _
    Public street As String = ""
    <DataMember> _
    Public city As String = ""
    <DataMember> _
    Public state As String = ""
    <DataMember> _
    Public postalCode As String = ""
    <DataMember> _
    Public latitude As Decimal = 0
    <DataMember> _
    Public longitude As Decimal = 0
    <DataMember> _
    Public radiusFeet As Integer
    <DataMember> _
    Public alertStatusId As Integer = 0
    <DataMember> _
    Public speedLimitAlertEnabled As Boolean = False
    <DataMember> _
    Public speedLimit As Integer = 0
    <DataMember> _
    Public geofenceTypeId As String = ""
End Class

<DataContract()> _
Public Class GeofenceType

    <DataMember> _
    Public apiToken As String = ""

    <DataMember> _
    Public companyId As Integer = 0

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public iconId As Integer = 0

    <DataMember> _
    Public iconUrl As String = ""

End Class

<DataContract()> _
Public Class GeofenceTypeResponse

    <DataMember> _
    Public apiToken As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public isOk As Boolean

    <DataMember> _
    Public msg As String = ""

End Class

<DataContract()> _
Public Class geofences_eTrackPilot

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public fullAddress As String = ""

    <DataMember> _
    Public contactName As String = ""

    <DataMember> _
    Public phone As String = ""

End Class

<DataContract()> _
Public Class pointInGeofence

    <DataMember> _
    Public isInside As Boolean = False

    <DataMember> _
    Public geofenceName As String = ""

    <DataMember> _
    Public geofenceId As String = ""

End Class