Imports System.Runtime.Serialization

<DataContract()> _
Public Class dynamicTemplate

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String

End Class

<DataContract()> _
Public Class dynamicField

    <DataMember> _
    Public templateId As String = ""

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public fieldId As String

    <DataMember> _
    Public typeId As String

    <DataMember> _
    Public label As String

    <DataMember> _
    Public jsonOptions As String

    <DataMember> _
    Public defaultValue As String

    <DataMember> _
    Public helpText As String

End Class

<DataContract()> _
Public Class jobDynamicField

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public type As String = ""

    <DataMember> _
    Public label As String

    <DataMember> _
    Public options As List(Of String)

    <DataMember> _
    Public defaultVal As String

    <DataMember> _
    Public helpText As String

End Class

<DataContract()> _
Public Class dynamicAnswer

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public fieldId As String = ""

    <DataMember> _
    Public answer As String = ""

    <DataMember> _
    Public eventDate As String = ""

End Class