Imports System.Runtime.Serialization

Public Class Location
    <DataMember> Public LatX4 As String = ""
    <DataMember> Public LngX4 As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public Street As String = ""
    <DataMember> Public City As String = ""
    <DataMember> Public State As String = ""
    <DataMember> Public PostalCode As String = ""
    <DataMember> Public CountryCode As String = ""
    <DataMember> Public Hits As Integer = 0
    <DataMember> Public County As String = ""
End Class
