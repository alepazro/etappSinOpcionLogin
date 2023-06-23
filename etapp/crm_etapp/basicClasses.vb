Imports System.Runtime.Serialization

<DataContract()> _
Public Class resultOk

    <DataMember> _
    Public isOk As Boolean

    <DataMember> _
    Public msg As String = ""

End Class

Public Class hotSpots
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public lat As Decimal = 0
    Public lng As Decimal = 0
    Public address As String = ""
    Public qty As Integer = 0
    Public lastVisitOn As String = ""
End Class

Public Class notificationSendTo
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public entityId As String = ""
    Public sendToId As String = ""
    Public userId As String = ""
    Public firstName As String = ""
    Public lastName As String = ""
    Public fullName As String = ""
    Public isEmail As Boolean = False
    Public isSms As Boolean = False
End Class

Public Class userBasicInfo
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public id As String = ""
    Public firstName As String = ""
    Public lastName As String = ""
    Public fullName As String = ""
    Public email As String = ""
End Class

Public Class basicList
    Public id As String = ""
    Public value As String = ""
    Public name As String = ""
    Public value1 As String = ""
End Class

Public Class singleValue
    Public name As String = ""
    Public value As String = ""
End Class

Public Class quickMsgDriver
    Public id As String = ""
    Public name As String = ""
    Public phone As String = ""
    Public email As String = ""
    Public deviceId As String = ""
    Public deviceName As String = ""
End Class

Public Class quickMsg
    Public driverId As String = ""
    Public channel As Integer = 0
    Public message As String = ""
End Class

<DataContract()> _
Public Class idName

    <DataMember> _
    Public parentId As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String

End Class

<DataContract()> _
Public Class pendingOnBoarding

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String

    <DataMember> _
    Public email As String

    <DataMember> _
    Public phone As String

    <DataMember> _
    Public billCity As String

    <DataMember> _
    Public billState As String

    <DataMember> _
    Public shipCity As String

    <DataMember> _
    Public shipState As String

    <DataMember> _
    Public createdOn As String

End Class

<DataContract()> _
Public Class onBoardingCustomer

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String

    <DataMember> _
    Public email As String

    <DataMember> _
    Public phone As String

    <DataMember> _
    Public billCity As String

    <DataMember> _
    Public billState As String

    <DataMember> _
    Public shipCity As String

    <DataMember> _
    Public shipState As String

    <DataMember> _
    Public createdOn As String

End Class

<DataContract> _
Public Class realTimeActivity

    <DataMember> _
    Public qtyUnits As Integer

    <DataMember> _
    Public onlineUsers As List(Of onlineUser)

End Class

<DataContract()> _
Public Class onlineUser

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public companyName As String

    <DataMember> _
    Public userName As String

    <DataMember> _
    Public phone As String

    <DataMember> _
    Public mobile As String

    <DataMember> _
    Public email As String

    <DataMember> _
    Public currentPage As String

    <DataMember> _
    Public currentPageTime As String

    <DataMember> _
    Public sessionTime As String

    <DataMember> _
    Public qtyUnits As Integer

End Class

<DataContract> _
Public Class userProfile

    <DataMember> _
    Public companyName As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

<DataContract> _
Public Class troubleLog

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public deviceName As String = ""

    <DataMember> _
    Public serialNumber As String = ""

    <DataMember> _
    Public lastUpdatedOn As String = ""

    <DataMember> _
    Public noShowDays As Integer = 0

    <DataMember> _
    Public powerCut As Integer = 0

    <DataMember> _
    Public mainPowerRestored As Integer = 0

    <DataMember> _
    Public illegalPowerUp As Integer = 0

    <DataMember> _
    Public powerDown As Integer = 0

    <DataMember> _
    Public ignOffGPS15 As Integer = 0

    <DataMember> _
    Public ignOffSpeed10 As Integer = 0

    <DataMember> _
    Public powerUp As Integer = 0

    <DataMember> _
    Public powerOffBatt As Integer = 0

    <DataMember> _
    Public totalEvents As Integer = 0

End Class