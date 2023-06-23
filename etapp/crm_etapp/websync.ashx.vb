Imports System.Web
Imports System.Web.Services
Imports FM
Imports FM.WebSync.Server


Public Class websync
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        context.Response.Write("Hello World!")

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class

Public Class SimpleChatEvents

    <WebSyncEvent(EventType.BeforeConnect, "/etChat/{dealerId}", FM.WebSync.Server.FilterType.Template)> _
    Public Shared Sub beforeConnect(sender As Object, e As WebSyncEventArgs)
        Dim dealerId As String = ""
        'If e.Matches.Length > 0 Then
        '    dealerId = e.Match.GetParameter("dealerId")
        'End If

        'Do something here with this event...

    End Sub

    <WebSyncEvent(EventType.AfterConnect, "/etChat/{dealerId}", FM.WebSync.Server.FilterType.Template)> _
    Public Shared Sub afterConnect(sender As Object, e As WebSyncEventArgs)
        Dim dealerId As String = ""
        'If e.Matches.Length > 0 Then
        '    dealerId = e.Match.GetParameter("dealerId")
        'End If

        'Do something here with this event...

    End Sub

    <WebSyncEvent(EventType.BeforeSubscribe, "/etChat/{dealerId}", FM.WebSync.Server.FilterType.Template)> _
    Public Shared Sub beforeSubscribe(sender As Object, e As WebSyncEventArgs)
        Dim dealerId As String = ""
        'If e.Matches.Length > 0 Then
        '    dealerId = e.Match.GetParameter("dealerId")
        'End If

        'Do something here with this event...

    End Sub

    <WebSyncEvent(EventType.AfterSubscribe, "/etChat/{dealerId}", FM.WebSync.Server.FilterType.Template)> _
    Public Shared Sub InitialLoad(sender As Object, e As WebSyncEventArgs)

        Dim dealerId As String = ""
        'If e.Matches.Length > 0 Then
        '    dealerId = e.Match.GetParameter("dealerId")
        'End If

        ' load the chat log history
        'e.SetExtensionValueJson("logs", Json.Serialize(SimpleChatData.LastTwenty()))
    End Sub

    <WebSyncEvent(EventType.BeforePublish, "/etChat/{dealerId}", FM.WebSync.Server.FilterType.Template)> _
    Public Shared Sub LogRequest(sender As Object, e As WebSyncEventArgs)
        ' store the chat log
        Dim dealerId As String = ""
        'If e.Matches.Length > 0 Then
        '    dealerId = e.Match.GetParameter("dealerId")
        'End If

        'We will comment this for now until we can segment the history by dealer... 
        'this, to avoid dealers seen chats of other dealers...
        'SimpleChatData.Add(Json.Deserialize(Of SimpleChatMessage)(e.PublishInfo.DataJson))
    End Sub

    <WebSyncEvent(EventType.AfterPublish, "/etChat/{dealerId}", FM.WebSync.Server.FilterType.Template)> _
    Public Shared Sub afterPublish(sender As Object, e As WebSyncEventArgs)
        ' store the chat log
        Dim dealerId As String = ""
        'If e.Matches.Length > 0 Then
        '    dealerId = e.Match.GetParameter("dealerId")
        'End If

    End Sub

End Class
