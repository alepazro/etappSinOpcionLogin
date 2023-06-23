Imports System.Runtime.Serialization

<DataContract()> _
Public Class caseClass

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public companyId As String

    <DataMember> _
    Public companyName As String

    <DataMember> _
    Public companyIsSuspended As Boolean

    <DataMember> _
    Public deviceId As String

    <DataMember> _
    Public deviceName As String

    <DataMember> _
    Public categoryId As String

    <DataMember> _
    Public categoryName As String

    <DataMember> _
    Public typeId As String

    <DataMember> _
    Public typeName As String

    <DataMember> _
    Public subTypeId As String

    <DataMember> _
    Public subTypeName As String

    <DataMember> _
    Public statusId As String

    <DataMember> _
    Public statusName As String

    <DataMember> _
    Public assignedToId As String

    <DataMember> _
    Public assignedToName As String

    <DataMember> _
    Public assignedOn As String

    <DataMember> _
    Public subject As String

    <DataMember> _
    Public notes As String

    <DataMember> _
    Public activities As List(Of caseActivity)

    <DataMember> _
    Public createdById As String = ""

    <DataMember> _
    Public createdOn As String = ""

    <DataMember> _
    Public lastUpdatedOn As String = ""

    <DataMember> _
    Public isClosed As Boolean

    <DataMember> _
    Public closedOn As String = ""

End Class

<DataContract()> _
Public Class caseActivity

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public caseId As String = ""

    <DataMember> _
    Public activityTypeId As String = ""

    <DataMember> _
    Public activityTypeName As String = ""

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public createdById As String = ""

    <DataMember> _
    Public createdByName As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

<DataContract()> _
Public Class crmBasicTables

    <DataMember>
    Public crmReports As List(Of idName)

    <DataMember>
    Public casesCategories As List(Of idName)

    <DataMember> _
    Public casesTypes As List(Of idName)

    <DataMember> _
    Public casesSubTypes As List(Of idName)

    <DataMember> _
    Public casesActivityTypes As List(Of idName)

    <DataMember> _
    Public casesStatus As List(Of statusItem)

End Class

<DataContract()> _
Public Class caseDevice

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public name As String = ""

End Class

<DataContract()> _
Public Class changeStatus

    <DataMember> _
    Public caseId As String = ""

    <DataMember> _
    Public action As String = ""

    <DataMember> _
    Public param1 As String = ""

    <DataMember> _
    Public param2 As String = ""

End Class

<DataContract()> _
Public Class changeStatusResponse

    <DataMember> _
    Public isOk As Boolean

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public statusName As String = ""

End Class

<DataContract()> _
Public Class statusItem

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public isClosed As Boolean

End Class

<DataContract()>
Public Class caseFilters

    <DataMember>
    Public categoryId As String = ""

    <DataMember>
    Public assignedToId As String = ""

    <DataMember>
    Public onlyMine As Boolean = False

    <DataMember>
    Public onlyOpen As Boolean = False

End Class

<DataContract()>
Public Class reportsFilters

    <DataMember> Public reportId As String = ""
    <DataMember> Public param1 As String = ""
    <DataMember> Public userToken As String = ""

End Class


<DataContract()>
Public Class reportClass

    <DataMember>
    Public content As Object

End Class
