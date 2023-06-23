Imports System.Runtime.Serialization

Public Class response
    <DataMember> Public Status As String = ""
    <DataMember> Public Messagge As String = ""
    <DataMember> Public ListResponse As List(Of Object) = New List(Of Object)
End Class
