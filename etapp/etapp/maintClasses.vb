Imports System.Runtime.Serialization

<DataContract()> _
Public Class maintDevice

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public typeId As Integer 'TRUCK, CRANE, CAR, VAN, etc.

    <DataMember> _
    Public make As String = ""

    <DataMember> _
    Public model As String = ""

    <DataMember> _
    Public modelYear As Integer

    <DataMember> _
    Public insuranceCarrier As String = ""

    <DataMember> _
    Public insurancePolicyNo As String = ""

    <DataMember> _
    Public insurancePremium As Decimal

    <DataMember> _
    Public insuranceDueOn As String = ""

    <DataMember> _
    Public datInsuranceDueOn As Date

    <DataMember> _
    Public odometer As Decimal

    <DataMember> _
    Public odometerChanged As Boolean = False

    <DataMember> _
    Public ignitionHours As Decimal

    <DataMember> _
    Public ignitionHoursChanged As Boolean = False

    <DataMember> _
    Public input1Name As String = ""

    <DataMember> _
    Public input1Hours As Decimal

    <DataMember> _
    Public input1HoursChanged As Boolean = False

    <DataMember> _
    Public input2Name As String = ""

    <DataMember> _
    Public input2Hours As Decimal

    <DataMember> _
    Public input2HoursChanged As Boolean = False

    <DataMember> _
    Public input3Name As String = ""

    <DataMember> _
    Public input3Hours As Decimal

    <DataMember> _
    Public input3HoursChanged As Boolean = False

    <DataMember> _
    Public input4Name As String = ""

    <DataMember> _
    Public input4Hours As Decimal

    <DataMember> _
    Public input4HoursChanged As Boolean = False

    <DataMember> _
    Public notes As String

End Class

<DataContract()> _
Public Class maintDeviceSchedule

    <DataMember> _
    Public deviceId As String = "" 'GUID

    <DataMember> _
    Public schedules As New List(Of scheduleItem)

    <DataMember> _
    Public meassures As New List(Of maintMeassure)

End Class

<DataContract()> _
Public Class maintDeviceLog

    <DataMember> _
    Public deviceId As String = "" 'GUID

    <DataMember> _
    Public logs As New List(Of scheduleItem)

End Class

<DataContract()> _
Public Class maintSupportLists

    <DataMember> _
    Public servicesTypes As New List(Of basicList)

    <DataMember> _
    Public timeReferences As New List(Of basicList)

    <DataMember> _
    Public vehicleTypes As New List(Of basicList)

    <DataMember> _
    Public maintTasks As New List(Of maintTask)

    <DataMember> _
    Public maintMeassures As New List(Of maintMeassure)

End Class

<DataContract()> _
Public Class maintTask

    <DataMember> _
    Public id As String

    <DataMember> _
    Public name As String

    <DataMember> _
    Public meassureId As Integer

    <DataMember> _
    Public value As Decimal

    <DataMember> _
    Public meassureName As String

End Class

<DataContract()> _
Public Class maintMeassure

    <DataMember> _
    Public id As Integer

    <DataMember> _
    Public name As String

    <DataMember> _
    Public unitName As String

End Class

<DataContract> _
Public Class maintHServicesParams

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public taskId As String = ""

    <DataMember> _
    Public dateFrom As String = ""

    <DataMember> _
    Public dateTo As String = ""

End Class

<DataContract> _
Public Class maintHServices

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public deviceName As String = ""

    <DataMember> _
    Public taskName As String = ""

    <DataMember> _
    Public serviceDate As String = ""

    <DataMember> _
    Public serviceType As String = ""

    <DataMember> _
    Public odometer As Integer

    <DataMember> _
    Public cost As Decimal

    <DataMember> _
    Public comments As String = ""

End Class

<DataContract> _
Public Class maintHFuel

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public deviceName As String = ""

    <DataMember> _
    Public fuelDate As String = ""

    <DataMember> _
    Public odometer As Integer

    <DataMember> _
    Public gallons As Decimal

    <DataMember> _
    Public cost As Decimal

    <DataMember> _
    Public stateName As String = ""

    <DataMember> _
    Public comments As String = ""

End Class

Public Class maintSchedule 'Old Version of Maintenance Module
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public id As String = ""
    Public deviceId As String = ""
    Public deviceName As String = ""
    Public taskId As String = ""
    Public taskName As String = ""
    Public taskMeassureId As String = ""
    Public taskMeassureName As String = ""
    Public taskValue As String = ""
    Public taskValueStr As String = ""
    Public lastServiceOn As String = ""
    Public currentValue As String = ""
    Public currentValueStr As String = ""
    Public nextServiceStr As String = ""
    Public nextService As Decimal = 0
    Public notifyBefore As Decimal = 0
    Public notifyEveryXDays As String = ""
    Public excludeWeekends As String = ""
    Public createdOn As String = ""
End Class

<DataContract()> _
Public Class scheduleItem

    <DataMember> _
    Public isNew As Boolean = False

    <DataMember> _
    Public id As String = "" 'GUID

    <DataMember> _
    Public taskId As String = "" 'GUID

    <DataMember> _
    Public taskName As String = ""

    <DataMember> _
    Public repeatEveryX As Integer = 0

    <DataMember> _
    Public repeatEveryTimeRefId As String = "0"

    <DataMember> _
    Public meassureId As Decimal = 0

    <DataMember> _
    Public meassureValue As Decimal = 0

    <DataMember> _
    Public frequency As String = "" 'FrequencyInWords

    <DataMember> _
    Public taskValue As Decimal = 0

    <DataMember> _
    Public lastServiceOn As String = ""

    <DataMember> _
    Public nextDue As String = ""

    <DataMember> _
    Public daysUntilDue As Integer

    <DataMember> _
    Public nextMeassureValue As Decimal = 0

    <DataMember> _
    Public nextDueInWords As String = "" ' example: 5,600 hours in Input 1

    <DataMember> _
    Public reminderTimeRefId As Integer = 0

    <DataMember> _
    Public reminderTimeRefVal As Integer = 0

    <DataMember> _
    Public reminderMeassureVal As Decimal = 0

    <DataMember> _
    Public reminder As String = "" 'ReminderInWords

    <DataMember> _
    Public valueSinceLastService As Decimal = 0

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public jobTypeId As Integer = 0

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public jobDescription As String = ""

    <DataMember> _
    Public cost As Decimal = 0

    <DataMember> _
    Public completedOn As String = Date.Now.ToString

    <DataMember> _
    Public odometer As Decimal

    <DataMember> _
    Public ignitionHours As Decimal

    <DataMember> _
    Public input1Hours As Decimal

    <DataMember> _
    Public input2Hours As Decimal

    <DataMember> _
    Public input3Hours As Decimal

    <DataMember> _
    Public input4Hours As Decimal

End Class


' CAST(S.ValueSinceLastService AS INT) AS ValueSinceLastService,

