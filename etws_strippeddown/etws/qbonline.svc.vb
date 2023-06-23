' NOTE: You can use the "Rename" command on the context menu to change the class name "qbonline" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select qbonline.svc or qbonline.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.IO
Imports Intuit.Ipp
Imports Intuit.Ipp.Security


Public Class qbonline
    Implements Iqbonline

    Public Sub DoWork() Implements Iqbonline.DoWork
    End Sub

    Private Sub qbAuthenticate()
        Dim accessToken As String = ""
        Dim accessTokenSecret As String = ""
        Dim consumerKey As String = ""
        Dim consumerSecret As String = ""
        'Dim oauthValidator As OAuthRequestValidator

        Try
            'oauthValidator = New OAuthRequestValidator(accessToken, accessTokenSecret, consumerKey, consumerSecret)


        Catch ex As Exception

        End Try
    End Sub

End Class
