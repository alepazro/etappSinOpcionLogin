Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient

Public Class dataLayerAPI

#Region "Declaratives"

    Private pSysModule As String = "DataLayerAPI.vb"
    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter
    Private BL As New BLCommon

#End Region

#Region "Devices API"

    Public Function getDevices(ByVal APIToken As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etAPI_getDevices"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parAPIToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parAPIToken.Direction = ParameterDirection.Input
            parAPIToken.Value = APIToken
            Command.Parameters.Add(parAPIToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = ex.Message
            Else
                strError = "Error getting devices"
            End If
        End Try

        Return dtData

    End Function

#End Region

#Region "Devices Events - to feed HDevices"

    Public Function HDevices_INSERT(ByVal parsedMsg As class_parsedMessage) As etwsResponse
        Dim configurationAppSettings As New System.Configuration.AppSettingsReader()
        Dim bResults As Boolean = True
        Dim strErrorMsg As String = ""
        Dim res As New etwsResponse

        Try
            strCommand = "HDevices_INSERT"
            conString = DirectCast(configurationAppSettings.GetValue("ConnectionString", GetType(String)), String)
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parDeviceTypeID As New SqlClient.SqlParameter("@DeviceTypeID", SqlDbType.Int)
            parDeviceTypeID.Value = 0
            Command.Parameters.Add(parDeviceTypeID)

            Dim parDeviceFamily As New SqlClient.SqlParameter("@DeviceFamily", SqlDbType.VarChar, 5)
            parDeviceFamily.Value = parsedMsg.deviceFamily
            Command.Parameters.Add(parDeviceFamily)

            Dim parSerialNumber As New SqlClient.SqlParameter("@SerialNumber", SqlDbType.VarChar, 50)
            parSerialNumber.Value = parsedMsg.SerialNumber
            Command.Parameters.Add(parSerialNumber)

            Dim parEventCode As New SqlClient.SqlParameter("@EventCode", SqlDbType.VarChar, 3)
            parEventCode.Value = parsedMsg.EventCode
            Command.Parameters.Add(parEventCode)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If Not IsDate(parsedMsg.EventDate) Then
                parsedMsg.EventDate = Date.Parse("1/1/1900")
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": EventDate IS NOT DATETIME WHEN IT SHOULD BE" & vbCrLf
            End If
            If Not IsDate(parsedMsg.EventDate) Then
                parsedMsg.EventDate = Date.UtcNow.ToString
            End If
            parEventDate.Value = parsedMsg.EventDate
            Command.Parameters.Add(parEventDate)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            If Not IsNumeric(parsedMsg.Latitude) Then
                parsedMsg.Latitude = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": Latitude IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parLatitude.Value = parsedMsg.Latitude
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            If Not IsNumeric(parsedMsg.Longitude) Then
                parsedMsg.Longitude = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": Longitude IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parLongitude.Value = parsedMsg.Longitude
            Command.Parameters.Add(parLongitude)

            Dim parSpeed As New SqlClient.SqlParameter("@Speed", SqlDbType.Int)
            If Not IsNumeric(parsedMsg.Speed) Then
                parsedMsg.Speed = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": Speed IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parSpeed.Value = parsedMsg.Speed
            Command.Parameters.Add(parSpeed)

            Dim parDecHeading As New SqlClient.SqlParameter("@decHeading", SqlDbType.Real)
            If Not IsNumeric(parsedMsg.decHeading) Then
                parsedMsg.decHeading = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": decHeading IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parDecHeading.Value = parsedMsg.decHeading
            Command.Parameters.Add(parDecHeading)

            Dim parHeading As New SqlClient.SqlParameter("@Heading", SqlDbType.VarChar, 3)
            parHeading.Value = parsedMsg.Heading
            Command.Parameters.Add(parHeading)

            Dim parGPSStatus As New SqlClient.SqlParameter("@GPSStatus", SqlDbType.Int)
            If Not IsNumeric(parsedMsg.GPSStatus) Then
                parsedMsg.GPSStatus = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": GPSStatus IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parGPSStatus.Value = parsedMsg.GPSStatus
            Command.Parameters.Add(parGPSStatus)

            Dim parGPSAge As New SqlClient.SqlParameter("@GPSAge", SqlDbType.Int)
            If Not IsNumeric(parsedMsg.GPSAge) Then
                parsedMsg.GPSAge = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": GPSAge IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parGPSAge.Value = parsedMsg.GPSAge
            Command.Parameters.Add(parGPSAge)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            If Not IsNumeric(parsedMsg.Odometer) Then
                parsedMsg.Odometer = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": Odometer IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parOdometer.Value = parsedMsg.Odometer
            Command.Parameters.Add(parOdometer)

            Dim parIOStatus As New SqlClient.SqlParameter("@IOStatus", SqlDbType.Real)
            If Not IsNumeric(parsedMsg.IOStatus) Then
                parsedMsg.IOStatus = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": IOStatus IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parIOStatus.Value = parsedMsg.IOStatus
            Command.Parameters.Add(parIOStatus)

            Dim parConsecutive As New SqlClient.SqlParameter("@Consecutive", SqlDbType.Int)
            If Not IsNumeric(parsedMsg.Consecutive) Then
                parsedMsg.Consecutive = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": Consecutive IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parConsecutive.Value = parsedMsg.Consecutive
            Command.Parameters.Add(parConsecutive)

            Dim parIsBrief As New SqlClient.SqlParameter("@IsBrief", SqlDbType.Bit)
            parIsBrief.Value = parsedMsg.isBrief
            Command.Parameters.Add(parIsBrief)

            Dim parIsSuperBrief As New SqlClient.SqlParameter("@IsSuperBrief", SqlDbType.Bit)
            parIsSuperBrief.Value = False
            Command.Parameters.Add(parIsSuperBrief)

            Dim parExtraData As New SqlClient.SqlParameter("@ExtraData", SqlDbType.NVarChar, 50)
            parExtraData.Value = parsedMsg.ExtraData
            Command.Parameters.Add(parExtraData)

            Dim parIP As New SqlClient.SqlParameter("@IP", SqlDbType.NVarChar, 50)
            parIP.Value = parsedMsg.serverIP
            Command.Parameters.Add(parIP)

            Dim parPort As New SqlClient.SqlParameter("@Port", SqlDbType.Int)
            If Not IsNumeric(parsedMsg.port) Then
                parsedMsg.port = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": port IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parPort.Value = parsedMsg.port
            Command.Parameters.Add(parPort)

            Dim parIgnitionStatus As New SqlClient.SqlParameter("@IgnitionStatus", SqlDbType.Int)
            If Not IsNumeric(parsedMsg.IgnitionStatus) Then
                parsedMsg.IgnitionStatus = -1
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": IgnitionStatus IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parIgnitionStatus.Value = parsedMsg.IgnitionStatus
            Command.Parameters.Add(parIgnitionStatus)

            Dim parOriginalEvent As New SqlClient.SqlParameter("@OriginalEvent", SqlDbType.NVarChar, 3)
            parOriginalEvent.Value = parsedMsg.originalEvent
            Command.Parameters.Add(parOriginalEvent)

            Dim parIButton As New SqlClient.SqlParameter("@IButtonID", SqlDbType.VarChar, 50)
            parIButton.Value = parsedMsg.iButtonID
            Command.Parameters.Add(parIButton)

            Dim parHasTemperature As New SqlClient.SqlParameter("@HasTemperature", SqlDbType.Bit)
            parHasTemperature.Value = parsedMsg.hasTemperature
            Command.Parameters.Add(parHasTemperature)

            Dim parTemperature1 As New SqlClient.SqlParameter("@Temperature1", SqlDbType.Int, 4)
            parTemperature1.Value = parsedMsg.temperature1
            Command.Parameters.Add(parTemperature1)

            Dim parTemperature2 As New SqlClient.SqlParameter("@Temperature2", SqlDbType.Int, 4)
            parTemperature2.Value = parsedMsg.temperature2
            Command.Parameters.Add(parTemperature2)

            Dim parTemperature3 As New SqlClient.SqlParameter("@Temperature3", SqlDbType.Int, 4)
            parTemperature3.Value = parsedMsg.temperature3
            Command.Parameters.Add(parTemperature3)

            Dim parTemperature4 As New SqlClient.SqlParameter("@Temperature4", SqlDbType.Int, 4)
            parTemperature4.Value = parsedMsg.temperature4
            Command.Parameters.Add(parTemperature4)

            Dim parQtySats As New SqlClient.SqlParameter("@Satellites", SqlDbType.Int, 4)
            parQtySats.Value = parsedMsg.QtySats
            Command.Parameters.Add(parQtySats)


            Dim parSW1 As New SqlClient.SqlParameter("@SW1", SqlDbType.Bit)
            parSW1.Value = parsedMsg.SW1
            Command.Parameters.Add(parSW1)

            Dim parSW2 As New SqlClient.SqlParameter("@SW2", SqlDbType.Bit)
            parSW2.Value = parsedMsg.SW2
            Command.Parameters.Add(parSW2)

            Dim parSW3 As New SqlClient.SqlParameter("@SW3", SqlDbType.Bit)
            parSW3.Value = parsedMsg.SW3
            Command.Parameters.Add(parSW3)

            Dim parSW4 As New SqlClient.SqlParameter("@SW4", SqlDbType.Bit)
            parSW4.Value = parsedMsg.SW4
            Command.Parameters.Add(parSW4)

            Dim parRelays As New SqlClient.SqlParameter("@Relays", SqlDbType.Int, 4)
            parRelays.Value = parsedMsg.Relays
            Command.Parameters.Add(parRelays)

            Dim parRelay1 As New SqlClient.SqlParameter("@Relay1", SqlDbType.Bit)
            parRelay1.Value = parsedMsg.relay1
            Command.Parameters.Add(parRelay1)

            Dim parRelay2 As New SqlClient.SqlParameter("@Relay2", SqlDbType.Bit)
            parRelay2.Value = parsedMsg.relay2
            Command.Parameters.Add(parRelay2)

            Dim parRelay3 As New SqlClient.SqlParameter("@Relay3", SqlDbType.Bit)
            parRelay3.Value = parsedMsg.relay3
            Command.Parameters.Add(parRelay3)

            Dim parRelay4 As New SqlClient.SqlParameter("@Relay4", SqlDbType.Bit)
            parRelay4.Value = parsedMsg.relay4
            Command.Parameters.Add(parRelay4)

            Dim parRSSI As New SqlClient.SqlParameter("@RSSI", SqlDbType.SmallInt)
            If Not IsNumeric(parsedMsg.rssi) Then
                parsedMsg.rssi = 0
                strErrorMsg = "Serial: " & parsedMsg.SerialNumber & ": RSSI IS NOT NUMERIC WHEN IT SHOULD BE" & vbCrLf
            End If
            parRSSI.Value = parsedMsg.rssi * -1
            Command.Parameters.Add(parRSSI)

            Dim parParserID As New SqlClient.SqlParameter("@ParserID", SqlDbType.TinyInt)
            parParserID.Value = parsedMsg.parserId
            Command.Parameters.Add(parParserID)

            Dim parWorkerID As New SqlClient.SqlParameter("@WorkerID", SqlDbType.TinyInt)
            parWorkerID.Value = parsedMsg.workerId
            Command.Parameters.Add(parWorkerID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim parTimeStamp5 As New SqlClient.SqlParameter("timestamp5", SqlDbType.DateTime2)
            parTimeStamp5.Value = Date.UtcNow
            Command.Parameters.Add(parTimeStamp5)

            Command.ExecuteNonQuery()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture("ETWS.Listener", "", "", ex.Message, 0)
            res.IsOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            If Not IsNothing(Command) Then
                Command.Dispose()
            End If
        End Try

        Try
            If strErrorMsg.Length > 0 Then
                BLErrorHandling.ErrorCapture("ETWS.Listener", "", "", "Serial: " & parsedMsg.SerialNumber & ": " & strErrorMsg, 0)
            End If
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

End Class
