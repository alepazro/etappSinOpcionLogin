' NOTE: You can use the "Rename" command on the context menu to change the class name "eTrackPlus" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select eTrackPlus.svc or eTrackPlus.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation.WebScriptServiceHostFactory
Imports System.Data
Imports System.Web.Script.Serialization
Imports System.IO

Public Class eTrackPlus
    Implements IeTrackPlus

    Public Function validateUser(ByVal login As String, ByVal password As String, ByVal expDays As String, ByVal lat As String, ByVal lng As String) As etPlus_User Implements IeTrackPlus.validateUser
        Dim user As New etPlus_User
        Dim dl As New dataLayer_etPlus
        Dim intExpDays As Integer
        Dim decLat As Decimal
        Dim decLng As Decimal

        Try
            If IsNumeric(expDays) Then
                intExpDays = CInt(expDays)
            End If
            If IsNumeric(lat) Then
                decLat = CDec(lat)
            End If
            If IsNumeric(lng) Then
                decLng = CDec(lng)
            End If
            user = dl.ValidateCredentials(login, password, intExpDays, decLat, decLng)
        Catch ex As Exception

        End Try

        Return user

    End Function

    Public Function getUserGUID(ByVal login As String, ByVal password As String) As etPlus_UserGUID Implements IeTrackPlus.getUserGUID
        Dim u As New etPlus_UserGUID
        Dim dl As New dataLayer_etPlus

        Try
            u = dl.getUserGUID(login, password)
        Catch ex As Exception

        End Try

        Return u

    End Function


#Region "HGeofences"

    Function geofencesHist(ByVal tkn As String) As hGeofencesResponse Implements IeTrackPlus.geofencesHist
        Dim dl As New dataLayer_etPlus
        Dim itm As New hGeofencesResponse

        Try
            itm = dl.geofencesHist(tkn)
        Catch ex As Exception

        End Try

        Return itm

    End Function

#End Region

End Class
