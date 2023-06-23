Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace jsonP
    ''' <summary>
    ''' Custom Http module for support Jsonp response
    ''' </summary>
    Public Class JsonpModule
        Implements IHttpModule
        ''' <summary>
        ''' callback Parameter Name for Jsonp request
        ''' </summary>
        Private Const CallbackParameterName As String = "callback"

        ''' <summary>
        ''' Initializes a module and prepares it to handle requests.
        ''' </summary>
        ''' <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
            AddHandler context.BeginRequest, New EventHandler(AddressOf Me.Application_BeginRequest)
            AddHandler context.EndRequest, New EventHandler(AddressOf Me.Application_EndRequest)
        End Sub

        ''' <summary>
        ''' Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        ''' </summary>
        Public Sub Dispose() Implements IHttpModule.Dispose
        End Sub

        ''' <summary>
        ''' Handles the BeginRequest event of the Application control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        Private Sub Application_BeginRequest(sender As Object, e As EventArgs)
            Dim httpApplication As HttpApplication = DirectCast(sender, HttpApplication)

            'Make sure only apply to Jsonp requests
            If Not String.IsNullOrEmpty(httpApplication.Context.Request.Params(CallbackParameterName)) Then
                If String.IsNullOrEmpty(httpApplication.Context.Request.ContentType) Then
                    httpApplication.Context.Request.ContentType = "application/json; charset=utf-8"
                End If

                httpApplication.Context.Response.Write(httpApplication.Context.Request.Params(CallbackParameterName) & "(")
            End If
        End Sub

        ''' <summary>
        ''' Handles the EndRequest event of the Application control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        Private Sub Application_EndRequest(sender As Object, e As EventArgs)
            Dim httpApplication As HttpApplication = DirectCast(sender, HttpApplication)
            If Not String.IsNullOrEmpty(httpApplication.Context.Request.Params(CallbackParameterName)) Then
                httpApplication.Context.Response.Write(")")
            End If
        End Sub

    End Class
End Namespace
