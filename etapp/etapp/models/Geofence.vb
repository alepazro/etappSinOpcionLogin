Imports System.Runtime.Serialization

<DataContract>
Public Class Geofence

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public name As String = ""

    <DataMember>
    Public fullAddress As String = ""

    <DataMember>
    Public latitude As String = ""

    <DataMember>
    Public longitude As String = ""

    <DataMember>
    Public radiusFeet As String = ""

    <DataMember>
    Public geofenceAlertTypeName As String = ""

    <DataMember>
    Public geofenceTypeName As String = ""

    <DataMember>
    Public shapeID As String = ""

    <DataMember>
    Public jsonPolyVerticesTXT As String = ""

    <DataMember>
    Public IconURL As String = ""

    <DataMember>
    Public geofenceInfoTable As String = ""

End Class

