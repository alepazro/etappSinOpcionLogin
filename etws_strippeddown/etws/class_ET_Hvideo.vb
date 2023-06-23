Imports System.Runtime.Serialization

<DataContract>
Public Class etHvideoResponse

    <DataMember> Public UserToken As String = ""
    <DataMember> Public HasVideo As Boolean = False
    <DataMember> Public ApiKey As String = ""

End Class