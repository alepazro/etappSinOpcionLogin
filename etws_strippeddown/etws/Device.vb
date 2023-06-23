Imports System.Runtime.Serialization

Public Class FleetDeviceVideo
    Public id As String = ""
    Public name As String = ""
    Public shortName As String = ""
    Public eventCode As String = ""
    Public eventName As String = ""
    Public eventDate As String = ""
    Public heading As String = ""
    Public address As String = ""
    Public speed As String = ""
    Public latitude As Decimal = 0
    Public longitude As Decimal = 0
    Public result As String = ""
    Public isOk As Boolean = True
    Public driverName As String = ""
    Public driverPhone As String = ""
End Class

Public Class device2
    Public vehicleId As String = ""
    Public name As String = ""
    Public latitude As Decimal = 0
    Public longitude As Decimal = 0
    Public lastUpdatedOn As String = ""
    Public eventTypeId As String = ""
    Public eventDate As String = ""
    Public speed As Integer = 0
    Public apiToken As String = ""
    Public isOk As Boolean
    Public msg As String = ""
End Class

<DataContract> _
Public Class checkInReason

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public description As String = ""

End Class

<DataContract> _
Public Class checkInEvent

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public isCheckedIn As Boolean = False

    <DataMember> _
    Public lastCheckInOutChange As String = ""

    <DataMember> _
    Public reasonId As Integer = 0

    <DataMember> _
    Public lat As Decimal = 0

    <DataMember> _
    Public lng As Decimal = 0

    <DataMember> _
    Public comments As String = ""

    <DataMember> _
    Public isDeleted As Boolean = False

    <DataMember> _
    Public deletedOn As String = ""

End Class

<DataContract> _
Public Class checkInRequest

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public odometer As String = ""

    <DataMember> _
    Public isCheckedIn As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

<DataContract> _
Public Class myTeam

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public lat As Decimal = 0

    <DataMember> _
    Public lng As Decimal = 0

    <DataMember> _
    Public address As String = ""

    <DataMember> _
    Public evTime As String = ""

    <DataMember> _
    Public iconUrl As String = ""

End Class

<DataContract()> _
Public Class allDataGet

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public jobStatuses As List(Of idNameItem)

    <DataMember> _
    Public jobPriorities As List(Of idNameItem)

    <DataMember> _
    Public jobs As List(Of Job2)

    <DataMember> _
    Public customers As List(Of Customer2)

    <DataMember> _
    Public custContacts As List(Of CustContact2)

End Class

<DataContract()> _
Public Class allDataPost

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobs As List(Of Job2)

    <DataMember> _
    Public jobStatuses As List(Of JobStatus)

    <DataMember> _
    Public jobNotes As List(Of JobNote)

    <DataMember> _
    Public customers As List(Of Customer2)

    <DataMember> _
    Public custContacts As List(Of CustContact2)

End Class

<DataContract()> _
Public Class allDataPostResult

    <DataMember> _
    Public jobs As postResult

    <DataMember> _
    Public jobStatuses As postResult

    <DataMember> _
    Public jobNotes As postResult

    <DataMember> _
    Public customers As postResult

    <DataMember> _
    Public custContacts As postResult

End Class

<DataContract()> _
Public Class postResult

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public failed As List(Of failedRecord)

End Class

<DataContract()> _
Public Class failedRecord

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public ref1 As String = ""

    <DataMember> _
    Public ref2 As String = ""

End Class

<DataContract> _
Public Class idNameItem

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

End Class

<DataContract> _
Public Class companyIdNameItem

    <DataMember> _
    Public companyId As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

End Class

<DataContract> _
Public Class jobStatusList

    <DataMember> _
    Public companyId As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public order As Integer

    <DataMember> _
    Public color As String = ""

    <DataMember> _
    Public foreColor As String = ""

End Class

<DataContract> _
Public Class fuelPurchase

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public odometer As String = ""

    <DataMember> _
    Public qty As String

    <DataMember> _
    Public amount As String

    <DataMember> _
    Public address As String = ""

    <DataMember> _
    Public stateId As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

'{token:/token/, licensePlate:/ licensePlate /, odometer:/odometer/, eventDate: /eventDate/, gallons: /gallons/, total:/total, lat:/latitude/, lng:/longitude/, locType:/locType/, locAccuracy:/locAccuracy/}
<DataContract> _
Public Class fuelPurchase2

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public licensePlate As String = ""

    <DataMember> _
    Public odometer As String = ""

    <DataMember> _
    Public gallons As String

    <DataMember> _
    Public total As String

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

    <DataMember> _
    Public locType As String = ""

    <DataMember> _
    Public locAccuracy As String = ""

End Class

<DataContract> _
Public Class alert

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public alert As String = ""

    <DataMember> _
    Public device As String = ""

    <DataMember> _
    Public driver As String = ""

    <DataMember> _
    Public address As String = ""

    <DataMember> _
    Public lat As Decimal

    <DataMember> _
    Public lng As Decimal

    <DataMember> _
    Public speed As Integer

    <DataMember> _
    Public eventDate As String

End Class

