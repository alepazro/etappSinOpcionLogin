Imports System.Runtime.Serialization

<DataContract()> _
Public Class job

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public jobNumber As String = ""

    <DataMember> _
    Public workZoneId As String = ""

    <DataMember> _
    Public workZoneName As String = ""

    <DataMember> _
    Public customerId As String = ""

    <DataMember> _
    Public customerName As String = ""

    <DataMember> _
    Public contactName As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public address As String = ""

    <DataMember> _
    Public jobDescription As String = ""

    <DataMember> _
    Public categoryId As String = ""

    <DataMember> _
    Public specialtyId As String = ""

    <DataMember> _
    Public assignedToId As String = ""

    <DataMember> _
    Public assignedToName As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public jobStatus As String = ""

    <DataMember> _
    Public priorityId As String = ""

    <DataMember> _
    Public priority As String = ""

    <DataMember> _
    Public createdOn As String = ""

    <DataMember> _
    Public scheduledStart As String = ""

    <DataMember> _
    Public durationHH As Integer = 0

    <DataMember> _
    Public durationMM As Integer = 0

    <DataMember> _
    Public dueOn As String = ""

End Class

<DataContract()> _
Public Class jobSupportTables

    <DataMember> _
    Public workZones As List(Of basicList)

    <DataMember> _
    Public priorities As List(Of basicList)

    <DataMember> _
    Public specialties As List(Of basicList)

    <DataMember> _
    Public technicians As List(Of technician)

    <DataMember> _
    Public statuses As List(Of jobStatus)

    <DataMember> _
    Public categories As List(Of basicList)

End Class

<DataContract()> _
Public Class jobStatus

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public qty As Integer = 0

    <DataMember> _
    Public overdue As Integer = 0

End Class

<DataContract()> _
Public Class technician

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public wzId As String = ""

End Class

<DataContract()> _
Public Class customerSearch

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public workZoneId As String = ""

    <DataMember> _
    Public workZoneName As String = ""

    <DataMember> _
    Public contactName As String = ""

    <DataMember> _
    Public address As String = ""

    <DataMember> _
    Public phone As String = ""

End Class
