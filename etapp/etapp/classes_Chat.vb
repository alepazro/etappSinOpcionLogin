Imports System.Runtime.Serialization

<DataContract()> _
Public Class SimpleChatMessage

    <DataMember> _
    Public [alias] As String = ""

    <DataMember> _
    Public text As String = ""

End Class

Public Class SimpleChatData
    Private Shared Logs As New Stack(Of SimpleChatMessage)(20)
    Private Shared LogsLock As New Object()

    Public Shared Function LastTwenty() As SimpleChatMessage()
        SyncLock LogsLock
            Return Logs.ToArray()
        End SyncLock
    End Function

    Public Shared Sub Add(log As SimpleChatMessage)
        SyncLock LogsLock
            Logs.Push(log)
            While Logs.Count > 20
                Logs.Pop()
            End While
        End SyncLock
    End Sub

End Class
