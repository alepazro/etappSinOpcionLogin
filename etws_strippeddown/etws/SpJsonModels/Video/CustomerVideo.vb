
Imports System.Runtime.Serialization

<DataContract>
Public Class CustomerVideo
    <DataMember>
    Public CompanyName As String = ""
    <DataMember>
    Public FullName As String = ""
    <DataMember>
    Public HasVideo As String = ""
    <DataMember>
    Public VideoKey As String = ""
    <DataMember>
    Public AccountId As String = ""
    <DataMember>
    Public IsDirectDealer As Boolean = False
End Class
Public Class CustomerVideoEp
    <DataMember>
    Public email As String = ""
    <DataMember>
    Public fleetID As String = ""
    <DataMember>
    Public name As String = ""
End Class
