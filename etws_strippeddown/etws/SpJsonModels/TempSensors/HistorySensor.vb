Imports System.Runtime.Serialization

Public Class HistorySensor
    <DataMember> Public ID As Integer
    <DataMember> Public EventDate As String
    <DataMember> Public Temperature1 As String
    <DataMember> Public Temperature2 As String
    <DataMember> Public Temperature3 As String
    <DataMember> Public Temperature4 As String
    <DataMember> Public Latitude As String
    <DataMember> Public Longitude As String
    <DataMember> Public NameSensor As String
End Class
