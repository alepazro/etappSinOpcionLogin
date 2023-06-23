Imports System.Runtime.Serialization

<DataContract> _
Public Class pilotJob

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public jobNumber As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public custName As String = ""

    <DataMember> _
    Public custAddress As String = ""

    <DataMember> _
    Public custPhone As String = ""

    <DataMember> _
    Public custContact As String = ""

    <DataMember> _
    Public dueDate As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public jobDetails As String = ""

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

<DataContract> _
Public Class jobResponse

    <DataMember> _
    Public isOk As Boolean = False

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public custId As String = ""

End Class

<DataContract> _
Public Class pilotJobStatusUpdate

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public statusId As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

<DataContract> _
Public Class pilotJobNote

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public note As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

<DataContract> _
Public Class pilotJobRemove

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

End Class

