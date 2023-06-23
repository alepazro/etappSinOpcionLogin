Imports System.Runtime.Serialization

<DataContract>
Public Class etWliusResponse

    <DataMember> Public UserToken As String = ""
    <DataMember> Public HasElog As Boolean = False
    <DataMember> Public ApiKey As String = ""

End Class