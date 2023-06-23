Imports System.Runtime.Serialization

<DataContract>
Public Class Temperature
    <DataMember>
    Public DeviceID As Integer
    <DataMember>
    Public Temperature1 As Decimal
    <DataMember>
    Public Temperature2 As Decimal
    <DataMember>
    Public Temperature3 As Decimal
    <DataMember>
    Public Temperature4 As Decimal
End Class
