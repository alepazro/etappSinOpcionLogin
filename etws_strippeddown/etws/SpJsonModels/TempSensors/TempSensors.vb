Imports System.Runtime.Serialization

Public Class TempSensors
    <DataMember> Public ID As Integer
    <DataMember> Public IMEI As String
    <DataMember> Public TempNumber As Integer
    <DataMember> Public SensorID As String
    <DataMember> Public Devices As String
    <DataMember> Public Name As String
    <DataMember> Public Did As String
    <DataMember> Public Reassigned As Boolean
    <DataMember> Public CreatedOn? As DateTime
    <DataMember> Public LastUpdatedOn As String
    <DataMember> Public Action As Integer
End Class
