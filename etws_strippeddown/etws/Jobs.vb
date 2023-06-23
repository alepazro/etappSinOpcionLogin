Imports System.Runtime.Serialization

<DataContract>
Public Class Job

    <DataMember>
    Public isOk As Boolean = True

    <DataMember>
    Public msg As String = ""

    <DataMember>
    Public token As String = ""

    <DataMember>
    Public jobId As String = ""

    <DataMember>
    Public jobNumber As String = ""

    <DataMember>
    Public custId As String = ""

    <DataMember>
    Public locationId As String = ""

    <DataMember>
    Public contactId As String = ""

    <DataMember>
    Public dueDate As String = ""

    <DataMember>
    Public statusId As String = ""

    <DataMember>
    Public categoryId As String = ""

    <DataMember>
    Public priorityId As String = ""

    <DataMember>
    Public jobName As String = ""

    <DataMember>
    Public details As String = ""

    <DataMember>
    Public notes As String = ""

    <DataMember>
    Public createdOn As String = ""

End Class

<DataContract>
Public Class jobnew

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public jobNumber As String = ""

    <DataMember>
    Public workZoneId As String = ""

    <DataMember>
    Public workZoneName As String = ""

    <DataMember>
    Public customerId As String = ""

    <DataMember>
    Public customerName As String = ""

    <DataMember>
    Public contactName As String = ""

    <DataMember>
    Public phone As String = ""

    <DataMember>
    Public address As String = ""

    <DataMember>
    Public jobDescription As String = ""

    <DataMember>
    Public categoryId As String = ""

    <DataMember>
    Public specialtyId As String = ""

    <DataMember>
    Public assignedToId As String = ""

    <DataMember>
    Public assignedToName As String = ""

    <DataMember>
    Public statusId As String = ""

    <DataMember>
    Public jobStatus As String = ""

    <DataMember>
    Public priorityId As String = ""

    <DataMember>
    Public priority As String = ""

    <DataMember>
    Public createdOn As String = ""

    <DataMember>
    Public scheduledStart As String = ""

    <DataMember>
    Public durationHH As Integer = 0

    <DataMember>
    Public durationMM As Integer = 0

    <DataMember>
    Public dueOn As String = ""

    <DataMember>
    Public StartOn As String = ""

    <DataMember>
    Public DurationJob As String = ""


End Class

'{jobId:/jobId/, jobNumber:/jobNumber/, custId:/customerId/, dueDate: /dueDate/, statusId:/statusId/, priorityId: /priorityId/, jobDescription: /jobDescription/, createdOn:/createdOn/}

<DataContract> _
Public Class Job2

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public jobNumber As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public contactId As String = ""

    <DataMember> _
    Public dueDate As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public priorityId As String = ""

    <DataMember> _
    Public jobDescription As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

<DataContract> _
Public Class Job3

    <DataMember>
    Public isOk As Boolean = True
    <DataMember>
    Public msg As String = ""
    <DataMember>
    Public companyId As String = ""
    <DataMember>
    Public userId As String = ""
    <DataMember>
    Public userName As String = ""
    <DataMember>
    Public jobId As String = ""
    <DataMember>
    Public jobNumber As String = ""
    <DataMember>
    Public customerId As String = ""
    <DataMember>
    Public custName As String = ""
    <DataMember>
    Public custAddress As String = ""
    <DataMember>
    Public custContact As String = ""
    <DataMember>
    Public custPhone As String = ""
    <DataMember>
    Public custEmail As String = ""
    <DataMember>
    Public custLat As Decimal
    <DataMember>
    Public custLng As Decimal
    <DataMember>
    Public dueDate As String = ""
    <DataMember>
    Public statusId As String = ""
    <DataMember>
    Public statusName As String = ""
    <DataMember>
    Public statusColor As String = ""
    <DataMember>
    Public statusForeColor As String = ""
    <DataMember>
    Public priorityId As String = ""
    <DataMember>
    Public priorityName As String = ""
    <DataMember>
    Public categoryId As String = ""
    <DataMember>
    Public categoryName As String = ""
    <DataMember>
    Public jobDescription As String = ""
    <DataMember>
    Public estDuration As Integer
    <DataMember>
    Public durationHHMM As String
    <DataMember>
    Public notes As List(Of JobNote)
    <DataMember>
    Public picturesList As List(Of imgData)
    <DataMember>
    Public signature As imgData
    'This comes loaded with an example.
    'stringified json of list(of dynamicField)
    <DataMember>
    Public fie As List(Of jobDynamicField)
    <DataMember>
    Public StartOn As String = ""
    <DataMember>
    Public CustomerGUID As String = ""
    <DataMember>
    Public AssignedToID As String = ""
    <DataMember>
    Public Street As String = ""
    <DataMember>
    Public City As String = ""
    <DataMember>
    Public State As String = ""
    <DataMember>
    Public PostalCode As String = ""
    <DataMember>
    Public jobstoplist As List(Of JobStop) = New List(Of JobStop)
    <DataMember>
    Public RadiusFeet As Integer = 0











End Class

<DataContract> _
Public Class JobAsset

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public assetId As String = ""

    <DataMember> _
    Public categoryId As String = ""

    <DataMember> _
    Public details As String = ""

End Class

<DataContract> _
Public Class JobStatus

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public eventDate As String = ""

    <DataMember>
    Public lat As Decimal

    <DataMember>
    Public lng As Decimal
    <DataMember>
    Public sourceID As String = ""

End Class

<DataContract> _
Public Class JobNote

    <DataMember> _
    Public token As String = ""

    <DataMember>
    Public jobId As String = ""

    <DataMember>
    Public uniqueKey As String = ""

    <DataMember> _
    Public noteId As String = ""

    <DataMember> _
    Public note As String = ""

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember>
    Public lng As String = ""

    <DataMember>
    Public createOn As String = ""

    <DataMember>
    Public UpdateFrom As Byte = 0

    <DataMember>
    Public action As Byte = 0

    <DataMember>
    Public status As Byte = 0


End Class

<DataContract> _
Public Class JobDelete

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

<DataContract> _
Public Class JobToRemove

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public jobId As String = ""

End Class

'{jobNumber:/jobNumber/, customerId: /customerId/, assignedToId: /userId/, priorityId: /priorityId/, statusId: /statusId/, dueDateFrom: /dueDateFrom/, dueDateTo: /dueDateTo/}
<DataContract> _
Public Class jobFilter

    <DataMember> _
    Public allActive As Boolean = False

    <DataMember> _
    Public jobNumber As String = ""

    <DataMember> _
    Public customerId As String = ""

    <DataMember> _
    Public assignedToId As String = ""

    <DataMember> _
    Public priorityId As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public categoryId As String = ""

    <DataMember> _
    Public dueDateFrom As String = ""

    <DataMember> _
    Public dueDateTo As String = ""

End Class

<DataContract()> _
Public Class jobStatuses

    <DataMember> _
    Public value As String = ""

    <DataMember> _
    Public text As String = ""

    <DataMember> _
    Public qty As Integer = 0

    <DataMember> _
    Public overdue As Integer = 0

End Class

<DataContract()> _
Public Class jobStatusAction

    <DataMember> _
    Public statusId As Integer = 0

    <DataMember> _
    Public statusName As String = ""

    <DataMember> _
    Public actionId As Integer = 0

    <DataMember> _
    Public actionName As String = ""

    <DataMember> _
    Public targetStatusId As Integer = 0

End Class

<DataContract()> _
Public Class technician

    <DataMember> _
    Public value As String = ""

    <DataMember> _
    Public text As String = ""

    <DataMember> _
    Public wzId As String = ""

    Friend name As Object

End Class

<DataContract()> _
Public Class jobSupportTables

    <DataMember> _
    Public workZones As List(Of workZonesListItem)

    <DataMember> _
    Public priorities As List(Of selectList)

    <DataMember> _
    Public specialties As List(Of selectList)

    <DataMember> _
    Public technicians As List(Of technician)

    <DataMember> _
    Public categories As List(Of selectList)

    <DataMember> _
    Public statuses As List(Of jobStatuses)

    <DataMember>
    Public statusActions As List(Of jobStatusAction)

    <DataMember>
    Public drivers As New List(Of JsonGetDriver)




End Class

<DataContract()>
Public Class customerSearch

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public name As String = ""

    <DataMember>
    Public workZoneId As String = ""

    <DataMember>
    Public workZoneName As String = ""

    <DataMember>
    Public contactName As String = ""

    <DataMember>
    Public address As String = ""

    <DataMember>
    Public phone As String = ""

End Class
<DataContract()>
Public Class JobStop
    <DataMember>
    Public ID As Integer
    <DataMember>
    Public uniqueKey As String
    <DataMember>
    Public JobUniqueKey As String
    <DataMember>
    Public CompanyID As Integer
    <DataMember>
    Public DeviceID As String
    <DataMember>
    Public DriverId As Integer
    <DataMember>
    Public Name As String = ""
    <DataMember>
    Public Latitude As Decimal = 0.0
    <DataMember>
    Public Longitude As Decimal = 0.0
    <DataMember>
    Public Street As String = ""
    <DataMember>
    Public FullAddress As String = ""
    <DataMember>
    Public City As String = ""
    <DataMember>
    Public State As String = ""
    <DataMember>
    Public PostalCode As String = ""
    <DataMember>
    Public DueDate As String = ""
    <DataMember>
    Public CompletedOn As String = ""
    <DataMember>
    Public StartOn As String = ""
    <DataMember>
    Public CreatedON As String = ""
    <DataMember>
    Public CreatedBy As String = ""
    <DataMember>
    Public StatusID As String = ""
    <DataMember>
    Public LastUpdate As String = ""
    <DataMember>
    Public Phone As String = ""
    <DataMember>
    Public Description As String = ""
    <DataMember>
    Public IsGeofence As Boolean = False
    <DataMember>
    Public GeofenceGUID As String = ""
    <DataMember>
    Public UpdateFrom As Integer = 0
    <DataMember>
    Public StopNumber As Integer = 0
    <DataMember>
    Public Status As Boolean = False


End Class
<DataContract()>
Public Class JobStopPost
    <DataMember>
    Public ID As Integer
    <DataMember>
    Public uniqueKey As String
    <DataMember>
    Public JobUniqueKey As String
    <DataMember>
    Public CompanyID As Integer
    <DataMember>
    Public DeviceID As String
    <DataMember>
    Public DriverId As Integer
    <DataMember>
    Public Name As String = ""
    <DataMember>
    Public Latitude As Decimal = 0.0
    <DataMember>
    Public Longitude As Decimal = 0.0
    <DataMember>
    Public Street As String = ""
    <DataMember>
    Public FullAddress As String = ""
    <DataMember>
    Public City As String = ""
    <DataMember>
    Public State As String = ""
    <DataMember>
    Public PostalCode As String = ""
    <DataMember>
    Public DueDate As String = ""
    <DataMember>
    Public CompletedOn As String = ""
    <DataMember>
    Public StartOn As String = ""
    <DataMember>
    Public CreatedON As String = ""
    <DataMember>
    Public CreatedBy As String = ""
    <DataMember>
    Public Status As String = ""
    <DataMember>
    Public LastUpdate As String = ""
    <DataMember>
    Public Phone As String = ""
    <DataMember>
    Public Description As String = ""
    <DataMember>
    Public IsGeofence As Boolean = False
    <DataMember>
    Public GeofenceGUID As String = ""
    <DataMember>
    Public UpdateFrom As Integer = 0


End Class
<DataContract()>
Public Class StatusJobs
    <DataMember>
    Public UniqueKey As String
    <DataMember>
    Public Name As String
End Class

