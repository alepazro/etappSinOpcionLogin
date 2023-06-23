Imports System.Runtime.Serialization

Public Class CrmDeviceData
    <DataMember> Public Device As New Devices
    <DataMember> Public ListHDevices As New List(Of HDevices)
    <DataMember> Public ListHDevicesInternal As New List(Of HDevicesInternal)
    <DataMember> Public ListHTempSensor As New List(Of TempSensors)
    <DataMember> Public HistorySensor As New List(Of HistorySensor)

End Class
