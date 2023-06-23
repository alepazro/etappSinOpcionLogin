Imports System.Runtime.Serialization

Public Class Devices
    <DataMember> Public DeviceType As String = ""
    <DataMember> Public Model As String = ""
    <DataMember> Public DeviceID As String = ""
    <DataMember> Public Name As String = ""
    <DataMember> Public SimNoDB As String = ""
    <DataMember> Public SimNoUnit As String = ""
    <DataMember> Public ReportIgnON As String = ""
    <DataMember> Public ReportTimerIgnOff As String = ""
    <DataMember> Public ReportTurnAngle As String = ""
    <DataMember> Public ReportDistance As String = ""
    <DataMember> Public FakeIgn As String = ""
    <DataMember> Public IgnON As String = ""
    <DataMember> Public IgnOFF As String = ""
    <DataMember> Public ServerIP As String = ""
    <DataMember> Public ServerPort As String = ""
End Class
