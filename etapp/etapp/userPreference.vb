Imports System.Runtime.Serialization

''' <summary>
''' Module: TRACKING
''' - Preference: SHOWDEVICE: whether the device is selected or not to display in the map
''' -            Val1: DeviceID
''' -            Val2: true/false
''' </summary>
''' <remarks></remarks>
<DataContract()> _
Public Class userPreference

    <DataMember> _
    Public moduleName As String = "" 'TRACKING

    <DataMember> _
    Public preference As String = "" 'TRACKING: SHOWDEVICE(whether the device is selected or not to display in the map).

    <DataMember> _
    Public val1 As String = "" 'TRACKING.SHOWDEVICE: DEVICEID

    <DataMember> _
    Public val2 As String = "" 'TRACKING.SHOWDEVICE: true/false

End Class
