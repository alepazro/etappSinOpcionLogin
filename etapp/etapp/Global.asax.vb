Imports System.Web.SessionState
Imports System.Web.Routing

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request

        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*")

        If (HttpContext.Current.Request.HttpMethod = "OPTIONS") Then
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST")
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept")
            HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000")
            HttpContext.Current.Response.End()
        End If

    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
        Dim ex As New Exception
        ex = Server.GetLastError
        If ex.Message = "File does not exits." Then
            Throw New Exception(HttpContext.Current.Request.Url.ToString)
        End If
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class