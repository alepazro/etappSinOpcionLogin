Imports System.Runtime.Serialization

<DataContract> _
Public Class user2

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public uniqueId As String = ""

End Class

<DataContract> _
Public Class user

    <DataMember> _
    Public login As String = ""

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public expDays As Integer = 0

End Class

<DataContract> _
Public Class webUser

    <DataMember> _
    Public tokenCookie As String = ""

    <DataMember> _
    Public isValid As Boolean = False

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public companyName As String = ""

    <DataMember> _
    Public fullName As String = ""

    <DataMember> _
    Public welcomeTitle As String = ""

    <DataMember> _
    Public accessLevelId As Integer = 0

    <DataMember> _
    Public isAdministrator As Boolean = False

    <DataMember> _
    Public defaultModuleId As Integer = 0

    <DataMember> _
    Public isSuspended As Boolean = False

    <DataMember> _
    Public suspendedReasonId As Integer = 0

    <DataMember> _
    Public UserGUID As String = ""

    <DataMember> Public versionId As String = ""
End Class

<DataContract> _
Public Class pilotUser

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public fullName As String = ""

    <DataMember> _
    Public isCheckedIn As Boolean = False

    <DataMember> _
    Public checkedInSince As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember>
    Public deviceId As String = ""

    <DataMember> Public versionId As String = ""

End Class

<DataContract> _
Public Class pilotUser2

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public companyId As String = ""

    <DataMember> _
    Public userId As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public fullName As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> Public versionId As String = ""

End Class

<DataContract> _
Public Class recoverCredentialsRequest

    <DataMember> _
    Public email As String = ""

End Class

<DataContract()> _
Public Class userPreference

    <DataMember> _
    Public moduleName As String = "" 'TRACKING

    <DataMember> _
    Public preference As String = "" 'TRACKING: SHOWDEVICE(whether the device is selected or not to display in the map).

    <DataMember> _
    Public val1 As String = "" 'TRACKING.SHOWDEVICE: DEVICEID

    <DataMember>
    Public val2 As String = "" 'TRACKING.SHOWDEVICE: true/false
    <DataMember>
    Public val3 As String = "" 'MULTITRACKING MAP: 1,2,3,4
    <DataMember>
    Public lat As Decimal = 0.0 'TRACKING
    <DataMember>
    Public lng As Decimal = 0.0 'TRACKING

End Class