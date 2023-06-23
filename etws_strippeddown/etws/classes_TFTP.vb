Imports System.Runtime.Serialization

<DataContract()>
Public Class cfgFile

    <DataMember()> Public id As Integer
    <DataMember()> Public folder As String
    <DataMember()> Public fullName As String
    <DataMember()> Public name As String
    <DataMember()> Public content As String

End Class

