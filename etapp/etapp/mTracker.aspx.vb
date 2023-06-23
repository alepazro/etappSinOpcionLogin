Public Class mTracker
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim UDID As String = ""
        Dim source As Integer = 0
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim accuracy As Decimal = 0
        Dim timestamp As String = ""

        Try
            If Not IsNothing(Request.QueryString("udid")) Then
                UDID = Request.QueryString("udid")
            End If
            If Not IsNothing(Request.QueryString("source")) Then
                source = Request.QueryString("source")
            End If
            If Not IsNothing(Request.QueryString("lat")) Then
                lat = Request.QueryString("lat")
            End If
            If Not IsNothing(Request.QueryString("lng")) Then
                lng = Request.QueryString("lng")
            End If
            If Not IsNothing(Request.QueryString("acc")) Then
                accuracy = Request.QueryString("acc")
            End If
            If Not IsNothing(Request.QueryString("time")) Then
                timestamp = Request.QueryString("time")
            End If



        Catch ex As Exception

        End Try
    End Sub

End Class