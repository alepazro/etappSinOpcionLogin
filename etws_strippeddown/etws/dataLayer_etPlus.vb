Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient

Public Class dataLayer_etPlus

#Region "Declaratives"

    Private pSysModule As String = "dataLayer_etPlus.vb"
    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter
    Private BL As New BLCommon

#End Region

    Public Function ValidateCredentials(ByVal login As String, ByVal password As String, ByVal expDays As Integer, ByVal latitude As Decimal, ByVal longitude As Decimal) As etPlus_User
        Dim itm As New etPlus_User

        Try
            strCommand = "etPlus_ValidateUser"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUsername As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 100)
            parUsername.Value = login
            Command.Parameters.Add(parUsername)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parExpDays As New SqlClient.SqlParameter("@ExpDays", SqlDbType.Int, 4)
            parExpDays.Value = expDays
            Command.Parameters.Add(parExpDays)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = latitude
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = longitude
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Int)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            itm.isOk = False

            Do While reader.Read
                itm.firstName = reader.Item("firstName")
                itm.lastName = reader.Item("lastName")
                itm.email = reader.Item("email")
                itm.cellPhone = reader.Item("cellPhone")
                itm.login = reader.Item("Login")
                itm.password = reader.Item("Password")
                itm.token = reader.Item("token")
                itm.tokenValidUntil = reader.Item("tokenValidUntil").ToString
                itm.isOk = reader.Item("IsOk")
                itm.result = reader.Item("Result")
                itm.msg = reader.Item("Msg")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

            If itm.isOk = False Then
                itm.result = CInt(parResult.Value)
                itm.msg = CStr(parMsg.Value)
            End If

        Catch ex As Exception
            itm.result = -99
            itm.msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function getUserGUID(ByVal login As String, ByVal password As String) As etPlus_UserGUID
        Dim itm As New etPlus_UserGUID

        Try
            strCommand = "etPlus_User_GetGUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUsername As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 100)
            parUsername.Value = login
            Command.Parameters.Add(parUsername)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Int)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            itm.isOk = False

            Do While reader.Read
                itm.firstName = reader.Item("FirstName")
                itm.lastName = reader.Item("LastName")
                itm.email = reader.Item("Email")
                itm.userGUID = reader.Item("GUID")
                itm.isOk = reader.Item("IsOk")
                itm.result = reader.Item("Result")
                itm.msg = reader.Item("Msg")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

            If itm.isOk = False Then
                itm.result = CInt(parResult.Value)
                itm.msg = CStr(parMsg.Value)
            End If

        Catch ex As Exception
            itm.result = -99
            itm.msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

#Region "HGeofences"

    Function geofencesHist(ByVal tkn As String) As hGeofencesResponse
        Dim res As New hGeofencesResponse
        Dim itm As hGeofenceEvent = Nothing

        Try
            strCommand = "MOB_HGeofences_DataPump_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parAPIToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parAPIToken.Value = tkn
            Command.Parameters.Add(parAPIToken)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.NVarChar, 100)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parRequestId As New SqlClient.SqlParameter("@RequestID", SqlDbType.NVarChar, 20)
            parRequestId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parRequestId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New hGeofenceEvent
                itm.deviceId = reader.Item("DeviceID")
                itm.deviceName = reader.Item("DeviceName")
                itm.eventType = reader.Item("EventType")
                itm.geofenceName = reader.Item("GeofenceName")

                Try
                    itm.eventDate = reader.Item("EventDate").ToString
                Catch ex As Exception

                End Try

                Try
                    itm.iButtonRaw = reader.Item("iButtonRaw").ToString
                Catch ex As Exception

                End Try

                itm.driverFirstName = reader.Item("DriverFirstName")
                itm.driverLastName = reader.Item("DriverLastName")

                res.events.Add(itm)

            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

            res.isOk = CBool(parIsOk.Value)
            res.msg = CStr(parMsg.Value)
            res.requestId = CStr(parRequestId.Value)

        Catch ex As Exception
            res.isOk = False
            res.msg = "UNKNOWN_ERROR"
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return res

    End Function

#End Region

End Class
