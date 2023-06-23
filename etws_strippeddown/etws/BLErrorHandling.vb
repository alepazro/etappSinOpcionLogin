Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class BLErrorHandling

#Region "Declaratives"

    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter

#End Region

    Public Shared Sub ErrorCapture(ByVal sysModule As String, ByVal TargetSite As String, ByVal Source As String, ByVal ErrorMsg As String, ByVal UserID As Integer)
        Dim strCommand As String = ""
        Dim conString As String = ""
        Dim conSQL As SqlConnection
        Dim Command As SqlCommand

        conString = ConfigurationManager.AppSettings("ConnectionString")
        conSQL = New SqlConnection(conString)

        Try
            strCommand = "AppErrors_INSERT"
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSysModule As New SqlClient.SqlParameter("@sysModule", SqlDbType.NVarChar, 50)
            parSysModule.Direction = ParameterDirection.Input
            parSysModule.Value = sysModule
            Command.Parameters.Add(parSysModule)

            Dim parTargetSite As New SqlClient.SqlParameter("@TargetSite", SqlDbType.NVarChar)
            parTargetSite.Direction = ParameterDirection.Input
            parTargetSite.Value = TargetSite
            Command.Parameters.Add(parTargetSite)

            Dim parSource As New SqlClient.SqlParameter("@Source", SqlDbType.NVarChar)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = Source
            Command.Parameters.Add(parSource)

            Dim parErrorMsg As New SqlClient.SqlParameter("@ErrorMsg", SqlDbType.NVarChar)
            parErrorMsg.Direction = ParameterDirection.Input
            parErrorMsg.Value = ErrorMsg
            Command.Parameters.Add(parErrorMsg)

            Dim parUserID As New SqlClient.SqlParameter("@UserID", SqlDbType.Int, 4)
            parUserID.Direction = ParameterDirection.Input
            parUserID.Value = UserID
            Command.Parameters.Add(parUserID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try
    End Sub

End Class
