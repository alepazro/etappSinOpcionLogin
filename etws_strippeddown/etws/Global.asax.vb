Imports System.Web.SessionState
Imports System.Web.Routing
Imports System.Web
Imports FM
Imports FM.IceLink

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Public stunServer As FM.IceLink.StunServer
    Public turnServer As FM.IceLink.TurnServer
    Dim users As Object

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started

        Try
            'System.Diagnostics.Debugger.Break()
            FM.IceLink.License.SetKey("fmeyJpZCI6IjdiZTMxY2E3LTRlNjEtNDdjMS04ZGViLWE4MDYwMTIzZTgyZSIsImFpZCI6ImYwZjIwMTU5LTgyODUtNGI5NS1hN2MzLTA0NmFhYmFhNzdhMCIsInBjIjoiSWNlTGluayIsIml0IjpmYWxzZSwidmYiOjYzNjQzMDgxMzY4MDQzMDAwMCwidnQiOjYzNzUyNzQ2NjQ5ODM3MDAwMH0=.J5H2FKaF2COUQPk2lx4ZiMHBTO6WVJnk3+/NBHZhEWTr5CNJZ8f1Soyoqlzan5MK6pNruqS4kwmG36yzrYkXcqfNiEaOBlXApYe3dkdCEAGgdWzrd9a2OZ/OI+jjzLQWfnxhPNd8+jZpyBrk2KY1kalfzeN34fYeu04zD1XzCJQ=")
        Catch ex As Exception
            'MsgBox("license: " & ex.Message)
        End Try
        Try
            users = New Dictionary(Of String, String) From {{"yaquiapp", "yaqui0102"}}

            turnServer = New FM.IceLink.TurnServer(
                Function(ByVal args As FM.IceLink.TurnAuthArgs)

                    If users.ContainsValue((args.Username)) Then
                        Return FM.IceLink.TurnAuthResult.FromPassword(users(args.Username))
                    End If

                    Return Nothing
                End Function)

            turnServer.Start()

        Catch ex As Exception
            'MsgBox("turn: " & ex.Message)
        End Try
        Try
            stunServer = New FM.IceLink.StunServer
            stunServer.Start()
        Catch ex As Exception
            'MsgBox("stun: " & ex.Message)
        End Try
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
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
        Try
            stunServer.Stop()
        Catch ex As Exception

        End Try
    End Sub

End Class