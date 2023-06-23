Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter
Imports System.Security.Cryptography

Public Class DataLayer

#Region "Declaratives"

    Private pSysModule As String = "DataLayer.vb"

    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter
    Private strError As String = ""

#End Region

#Region "Authorization"

    Public Function ValidateToken_DEPRECATED(ByVal Token As String) As Boolean
        Dim isValid As Boolean = False

        Try
            strCommand = "CompaniesUsers_ValidateToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.nvarchar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsValid As New SqlClient.SqlParameter("@IsValid", SqlDbType.Bit)
            parIsValid.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsValid)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            isValid = CBool(parIsValid.Value)

        Catch ex As Exception
            isValid = False
            BLErrorHandling.ErrorCapture(pSysModule, "ValidateToken", "", ex.Message & " - Token: " & Token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isValid

    End Function

    Public Function ValidateToken(ByVal Token As String, ByVal sourcePage As String, ByVal sourceId As String, ByVal sourceExt As String) As DataView
        Dim dvData As DataView = Nothing
        Dim dtTable As New DataTable

        Try
            strCommand = "CompaniesUsers_ValidateToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parSourcePage As New SqlClient.SqlParameter("@SourcePage", SqlDbType.NVarChar, 50)
            parSourcePage.Direction = ParameterDirection.Input
            parSourcePage.Value = sourcePage
            Command.Parameters.Add(parSourcePage)

            Dim parSourceId As New SqlClient.SqlParameter("@SourceId", SqlDbType.NVarChar, 50)
            parSourceId.Direction = ParameterDirection.Input
            parSourceId.Value = sourceId
            Command.Parameters.Add(parSourceId)

            Dim parSourceExt As New SqlClient.SqlParameter("@SourceExt", SqlDbType.NVarChar, 50)
            parSourceExt.Direction = ParameterDirection.Input
            parSourceExt.Value = sourceExt
            Command.Parameters.Add(parSourceExt)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtTable)
            adapter.Dispose()
            Command.Dispose()
            dvData = dtTable.DefaultView

        Catch ex As Exception
            dvData = Nothing
            BLErrorHandling.ErrorCapture(pSysModule, "ValidateToken", "", ex.Message & " - Token: " & Token, 0)
        Finally
            conSQL.Dispose()
        End Try

        Return dvData

    End Function

    Public Function CredentialsVerification(ByVal UserName As String, ByVal Password As String, ByVal rememberMe As Boolean, ByRef msg As String) As DataView
        Dim IsValid As Boolean = False
        Dim dvData As DataView = Nothing
        Dim dtTable As New DataTable

        Try
            msg = ""
            UserName = BLCommon.Sanitize(UserName)
            Password = BLCommon.Sanitize(Password)

            strCommand = "CompaniesUsers_ValidateCredentials"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUsername As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 100)
            parUsername.Direction = ParameterDirection.Input
            parUsername.Value = UserName
            Command.Parameters.Add(parUsername)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Direction = ParameterDirection.Input
            parPassword.Value = Password
            Command.Parameters.Add(parPassword)

            Dim parRememberMe As New SqlClient.SqlParameter("@RememberMe", SqlDbType.Bit)
            parRememberMe.Direction = ParameterDirection.Input
            parRememberMe.Value = rememberMe
            Command.Parameters.Add(parRememberMe)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtTable)
            adapter.Dispose()
            Command.Dispose()
            dvData = dtTable.DefaultView

        Catch ex As Exception
            msg = ex.Message
            IsValid = False
            dvData = Nothing
            BLErrorHandling.ErrorCapture(pSysModule, "CredentialsVerification", "", ex.Message & " - UserName: " & UserName, 0)
        Finally
            conSQL.Dispose()
        End Try

        Return dvData

    End Function

#End Region

#Region "Devices"

    Public Function getDevices(ByVal Token As String, ByVal groupId As String, ByVal LastFetchOn As Date, ByRef strError As String) As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "Devices_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parGroupID As New SqlClient.SqlParameter("@GroupID", SqlDbType.NVarChar, 50)
            parGroupID.Direction = ParameterDirection.Input
            parGroupID.Value = groupId
            Command.Parameters.Add(parGroupID)

            Dim parLastFetchOn As New SqlClient.SqlParameter("@LastFetchOn", SqlDbType.DateTime)
            parLastFetchOn.Direction = ParameterDirection.Input
            parLastFetchOn.Value = LastFetchOn
            Command.Parameters.Add(parLastFetchOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dsData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = ex.Message
            Else
                strError = "Error getting devices"
                BLErrorHandling.ErrorCapture(pSysModule, "getDevices", "Token: " & Token, ex.Message & " - Token: " & Token, 0)
            End If
        End Try

        Return dsData

    End Function

    Public Function GetDeviceById(ByVal token As String, ByVal deviceId As String) As device
        Dim dev As device = Nothing

        Try
            strCommand = "Devices_GetByDeviceID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                dev = New device
                dev.latitude = reader.Item("Latitude")
                dev.longitude = reader.Item("Longitude")
                dev.infoTable = reader.Item("InfoTable")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        Finally
            conSQL.Dispose()
        End Try

        Return dev

    End Function

    Public Function GetDeviceBySearchText(ByVal Token As String, ByVal groupId As String, ByVal LastFetchOn As Date, ByRef strError As String, ByVal searchText As String) As DataSet

        Dim dsData As New DataSet

        Try

            strCommand = "Devices_GetByTokenByText"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            conSQL.Open()
            Command = New SqlCommand
            Command.Connection = conSQL


            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            'Dim parGroupID As New SqlClient.SqlParameter("@GroupID", SqlDbType.NVarChar, 50)
            'parGroupID.Direction = ParameterDirection.Input
            'parGroupID.Value = groupId
            'Command.Parameters.Add(parGroupID)

            'Dim parLastFetchOn As New SqlClient.SqlParameter("@LastFetchOn", SqlDbType.DateTime)
            'parLastFetchOn.Direction = ParameterDirection.Input
            'parLastFetchOn.Value = LastFetchOn
            'Command.Parameters.Add(parLastFetchOn)

            Dim parSearch As New SqlClient.SqlParameter("@Text", SqlDbType.NVarChar, 50)
            parSearch.Direction = ParameterDirection.Input
            parSearch.Value = searchText
            Command.Parameters.Add(parSearch)



            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()



        Catch ex As Exception
            dsData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = ex.Message
            Else
                strError = "Error getting devices"
                BLErrorHandling.ErrorCapture(pSysModule, "getDevices", "Token: " & Token, ex.Message & " - Token: " & Token, 0)
            End If
        End Try


        Return dsData
    End Function



#End Region

#Region "Geofences"

    Public Function saveGeofence(ByVal token As String, _
                                 ByVal id As String, _
                                 ByVal GeofenceTypeId As Integer, _
                                 ByVal name As String, _
                                 ByVal contactName As String, _
                                 ByVal phone As String, _
                                 ByVal contactEmail As String, _
                                 ByVal contactSMSAlert As Boolean, _
                                 ByVal contactEmailAlert As Boolean, _
                                 ByVal contactAlertTypeId As Integer, _
                                 ByVal fullAddress As String, _
                                 ByVal street As String, _
                                 ByVal streetNumber As String, _
                                 ByVal route As String, _
                                 ByVal suite As String, _
                                 ByVal city As String, _
                                 ByVal county As String, _
                                 ByVal state As String, _
                                 ByVal postalCode As String, _
                                 ByVal country As String, _
                                 ByVal lat As Decimal, _
                                 ByVal lng As Decimal, _
                                 ByVal GeofenceAlertTypeID As Integer, _
                                 ByVal radius As Integer, _
                                 ByVal comments As String, _
                                 ByVal shapeId As Integer, _
                                 ByVal jsonPolyVerticesTXT As String, _
                                 ByVal KMLData As String, _
                                 ByVal SQLData As String, _
                                 ByVal isSpeedLimit As Boolean, _
                                 ByVal speedLimit As Integer, _
                                 ByVal arrivalMsgId As Integer, _
                                 ByVal arrivalMsgTxt As String, _
                                 ByVal departureMsgId As Integer, _
                                 ByVal departureMsgTxt As String, _
                                 ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "Geofences_INSERT"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGeofenceTypeID As New SqlClient.SqlParameter("@GeofenceTypeID", SqlDbType.Int)
            parGeofenceTypeID.Value = GeofenceTypeId
            Command.Parameters.Add(parGeofenceTypeID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 20)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parContactName As New SqlClient.SqlParameter("@ContactName", SqlDbType.NVarChar, 50)
            parContactName.Value = contactName
            Command.Parameters.Add(parContactName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parContactEmail As New SqlClient.SqlParameter("@ContactEmail", SqlDbType.NVarChar, 100)
            parContactEmail.Value = contactEmail
            Command.Parameters.Add(parContactEmail)

            Dim parContactSMSAlert As New SqlClient.SqlParameter("@ContactSMSAlert", SqlDbType.Bit)
            parContactSMSAlert.Value = contactSMSAlert
            Command.Parameters.Add(parContactSMSAlert)

            Dim parContactEmailAlert As New SqlClient.SqlParameter("@ContactEmailAlert", SqlDbType.Bit)
            parContactEmailAlert.Value = contactEmailAlert
            Command.Parameters.Add(parContactEmailAlert)

            Dim parContactAlertTypeID As New SqlClient.SqlParameter("@ContactAlertTypeID", SqlDbType.Int)
            parContactAlertTypeID.Value = contactAlertTypeId
            Command.Parameters.Add(parContactAlertTypeID)

            Dim parFullAddress As New SqlClient.SqlParameter("@FullAddress", SqlDbType.NVarChar, 100)
            parFullAddress.Value = fullAddress
            Command.Parameters.Add(parFullAddress)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parStreetNumber As New SqlClient.SqlParameter("@StreetNumber", SqlDbType.NVarChar, 10)
            parStreetNumber.Value = streetNumber
            Command.Parameters.Add(parStreetNumber)

            Dim parRoute As New SqlClient.SqlParameter("@Route", SqlDbType.NVarChar, 50)
            parRoute.Value = route
            Command.Parameters.Add(parRoute)

            Dim parSuite As New SqlClient.SqlParameter("@Suite", SqlDbType.NVarChar, 20)
            parSuite.Value = suite
            Command.Parameters.Add(parSuite)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parCounty As New SqlClient.SqlParameter("@County", SqlDbType.NVarChar, 50)
            parCounty.Value = county
            Command.Parameters.Add(parCounty)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 15)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountry As New SqlClient.SqlParameter("@Country", SqlDbType.NVarChar, 20)
            parCountry.Value = country
            Command.Parameters.Add(parCountry)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parGeofenceAlertTypeID As New SqlClient.SqlParameter("@GeofenceAlertTypeID", SqlDbType.Int)
            parGeofenceAlertTypeID.Value = GeofenceAlertTypeID
            Command.Parameters.Add(parGeofenceAlertTypeID)

            Dim parRadius As New SqlClient.SqlParameter("@RadiusFeet", SqlDbType.Int)
            parRadius.Value = radius
            Command.Parameters.Add(parRadius)

            Dim parComments As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, 4000)
            parComments.Value = comments
            Command.Parameters.Add(parComments)

            Dim parShapeId As New SqlClient.SqlParameter("@ShapeId", SqlDbType.Int)
            parShapeId.Value = shapeId
            Command.Parameters.Add(parShapeId)

            Dim parVertices As New SqlClient.SqlParameter("@jsonPolyVerticesTXT", SqlDbType.NVarChar)
            parVertices.Value = jsonPolyVerticesTXT
            Command.Parameters.Add(parVertices)

            Dim parKMLData As New SqlClient.SqlParameter("@KMLData", SqlDbType.NVarChar)
            parKMLData.Value = KMLData
            Command.Parameters.Add(parKMLData)

            Dim parSQLData As New SqlClient.SqlParameter("@SQLData", SqlDbType.NVarChar)
            parSQLData.Value = SQLData
            Command.Parameters.Add(parSQLData)

            Dim parIsSpeedLimit As New SqlClient.SqlParameter("@IsSpeedLimit", SqlDbType.Bit)
            parIsSpeedLimit.Value = isSpeedLimit
            Command.Parameters.Add(parIsSpeedLimit)

            Dim parSpeedLimit As New SqlClient.SqlParameter("@SpeedLimit", SqlDbType.Int)
            parSpeedLimit.Value = speedLimit
            Command.Parameters.Add(parSpeedLimit)

            Dim parArrivalMsgId As New SqlClient.SqlParameter("@ArrivalMsgId", SqlDbType.Int)
            parArrivalMsgId.Value = arrivalMsgId
            Command.Parameters.Add(parArrivalMsgId)

            Dim parDepartureMsgId As New SqlClient.SqlParameter("@DepartureMsgId", SqlDbType.Int)
            parDepartureMsgId.Value = departureMsgId
            Command.Parameters.Add(parDepartureMsgId)

            Dim parArrivalMsgTxt As New SqlClient.SqlParameter("@ArrivalMsgTxt", SqlDbType.NVarChar)
            parArrivalMsgTxt.Value = arrivalMsgTxt
            Command.Parameters.Add(parArrivalMsgTxt)

            Dim parDepartureMsgTxt As New SqlClient.SqlParameter("@DepartureMsgTxt", SqlDbType.NVarChar)
            parDepartureMsgTxt.Value = departureMsgTxt
            Command.Parameters.Add(parDepartureMsgTxt)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error saving Geofence"
            BLErrorHandling.ErrorCapture(pSysModule, "saveGeofence", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function getGeofences_DEPRECATED(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Geofences_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getGeofences(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Geofences_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("geofences")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("geofenceTypeId")
                jsonLine.WriteValue(reader("GeofenceTypeId"))

                jsonLine.WritePropertyName("geofenceTypeName")
                jsonLine.WriteValue(reader("GeofenceTypeName"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("contactName")
                jsonLine.WriteValue(reader("ContactName"))

                jsonLine.WritePropertyName("phone")
                jsonLine.WriteValue(reader("Phone"))

                'GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
                jsonLine.WritePropertyName("contactEmail")
                jsonLine.WriteValue(reader("contactEmail"))

                jsonLine.WritePropertyName("contactSMSAlert")
                jsonLine.WriteValue(reader("contactSMSAlert"))

                jsonLine.WritePropertyName("contactEmailAlert")
                jsonLine.WriteValue(reader("contactEmailAlert"))

                jsonLine.WritePropertyName("contactAlertTypeId")
                jsonLine.WriteValue(reader("contactAlertTypeId"))
                '========================================

                jsonLine.WritePropertyName("street")
                jsonLine.WriteValue(reader("Street"))

                jsonLine.WritePropertyName("streetNumber")
                jsonLine.WriteValue(reader("StreetNumber"))

                jsonLine.WritePropertyName("route")
                jsonLine.WriteValue(reader("Route"))

                jsonLine.WritePropertyName("suite")
                jsonLine.WriteValue(reader("Suite"))

                jsonLine.WritePropertyName("city")
                jsonLine.WriteValue(reader("City"))

                jsonLine.WritePropertyName("county")
                jsonLine.WriteValue(reader("County"))

                jsonLine.WritePropertyName("state")
                jsonLine.WriteValue(reader("State"))

                jsonLine.WritePropertyName("postalCode")
                jsonLine.WriteValue(reader("PostalCode"))

                jsonLine.WritePropertyName("country")
                jsonLine.WriteValue(reader("Country"))

                jsonLine.WritePropertyName("fullAddress")
                jsonLine.WriteValue(reader("FullAddress"))

                jsonLine.WritePropertyName("latitude")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("longitude")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WritePropertyName("radius")
                jsonLine.WriteValue(reader("RadiusFeet"))

                jsonLine.WritePropertyName("iconUrl")
                jsonLine.WriteValue(reader("IconURL"))

                jsonLine.WritePropertyName("geofenceAlertTypeId")
                jsonLine.WriteValue(reader("GeofenceAlertTypeId"))

                jsonLine.WritePropertyName("geofenceAlertTypeName")
                jsonLine.WriteValue(reader("GeofenceAlertTypeName"))

                jsonLine.WritePropertyName("comments")
                jsonLine.WriteValue(reader("Comments"))

                jsonLine.WritePropertyName("shapeId")
                jsonLine.WriteValue(reader("ShapeID"))

                jsonLine.WritePropertyName("jsonPolyVerticesTXT")
                jsonLine.WriteValue(reader("jsonPolyVerticesTXT"))

                jsonLine.WritePropertyName("lastVisitedOn")
                If IsDBNull(reader("LastVisitedOn")) Then
                    jsonLine.WriteValue("N/A")
                Else
                    jsonLine.WriteValue(reader("LastVisitedOn").ToString)
                End If

                jsonLine.WritePropertyName("geofenceInfoTable")
                jsonLine.WriteValue(reader("GeofenceInfoTable"))

                jsonLine.WritePropertyName("isSpeedLimit")
                jsonLine.WriteValue(reader("IsSpeedLimit"))

                jsonLine.WritePropertyName("speedLimit")
                jsonLine.WriteValue(reader("SpeedLimit"))

                jsonLine.WritePropertyName("arrivalMsgId")
                jsonLine.WriteValue(reader("arrivalMsgId"))

                jsonLine.WritePropertyName("departureMsgId")
                jsonLine.WriteValue(reader("departureMsgId"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getGeofences_All(ByVal Token As String, ByVal isExtended As Boolean, ByRef errMsg As String) As String
        Dim strJson As String = ""
        'IF isExtended = true then the dataset contains these additional elements:
        'shapeId
        'iconURL
        'geofenceInfoTable
        'jsonPolyVerticesTXT

        Try
            strCommand = "Geofences_GetAll"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parExt As New SqlClient.SqlParameter("@Extended", SqlDbType.Bit)
            parExt.Value = isExtended
            Command.Parameters.Add(parExt)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("geofences")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            '	   , G.Name

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("geofenceTypeName")
                jsonLine.WriteValue(reader("GeofenceTypeName"))

                jsonLine.WritePropertyName("fullAddress")
                jsonLine.WriteValue(reader("FullAddress"))

                jsonLine.WritePropertyName("latitude")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("longitude")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WritePropertyName("radius")
                jsonLine.WriteValue(reader("RadiusFeet"))

                jsonLine.WritePropertyName("geofenceAlertTypeName")
                jsonLine.WriteValue(reader("GeofenceAlertTypeName"))

                jsonLine.WritePropertyName("shapeId")
                jsonLine.WriteValue(reader("shapeId"))

                If isExtended = True Then


                    jsonLine.WritePropertyName("iconUrl")
                    jsonLine.WriteValue(reader("iconURL"))

                    jsonLine.WritePropertyName("geofenceInfoTable")
                    jsonLine.WriteValue(reader("geofenceInfoTable"))

                    jsonLine.WritePropertyName("jsonPolyVerticesTXT")
                    jsonLine.WriteValue(reader("jsonPolyVerticesTXT"))


                End If

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getGeofences_AllList(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""
        'IF isExtended = true then the dataset contains these additional elements:
        'shapeId
        'iconURL
        'geofenceInfoTable
        'jsonPolyVerticesTXT

        Try
            strCommand = "Geofences_GetAllList"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("geofences")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            '	   , G.Name

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getGeofences_InfoWindow(ByVal Token As String, ByVal id As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Geofences_InfoWindow_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("geofence")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("geofenceInfoTable")
                jsonLine.WriteValue(reader("geofenceInfoTable"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getGeofence(ByVal Token As String, ByVal id As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Geofences_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("geofence")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("geofenceTypeId")
                jsonLine.WriteValue(reader("GeofenceTypeId"))

                jsonLine.WritePropertyName("geofenceTypeName")
                jsonLine.WriteValue(reader("GeofenceTypeName"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("contactName")
                jsonLine.WriteValue(reader("ContactName"))

                jsonLine.WritePropertyName("phone")
                jsonLine.WriteValue(reader("Phone"))

                'GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
                jsonLine.WritePropertyName("contactEmail")
                jsonLine.WriteValue(reader("contactEmail"))

                jsonLine.WritePropertyName("contactSMSAlert")
                jsonLine.WriteValue(reader("contactSMSAlert"))

                jsonLine.WritePropertyName("contactEmailAlert")
                jsonLine.WriteValue(reader("contactEmailAlert"))

                jsonLine.WritePropertyName("contactAlertTypeId")
                jsonLine.WriteValue(reader("contactAlertTypeId"))
                '========================================

                jsonLine.WritePropertyName("street")
                jsonLine.WriteValue(reader("Street"))

                jsonLine.WritePropertyName("streetNumber")
                jsonLine.WriteValue(reader("StreetNumber"))

                jsonLine.WritePropertyName("route")
                jsonLine.WriteValue(reader("Route"))

                jsonLine.WritePropertyName("suite")
                jsonLine.WriteValue(reader("Suite"))

                jsonLine.WritePropertyName("city")
                jsonLine.WriteValue(reader("City"))

                jsonLine.WritePropertyName("county")
                jsonLine.WriteValue(reader("County"))

                jsonLine.WritePropertyName("state")
                jsonLine.WriteValue(reader("State"))

                jsonLine.WritePropertyName("postalCode")
                jsonLine.WriteValue(reader("PostalCode"))

                jsonLine.WritePropertyName("country")
                jsonLine.WriteValue(reader("Country"))

                jsonLine.WritePropertyName("fullAddress")
                jsonLine.WriteValue(reader("FullAddress"))

                jsonLine.WritePropertyName("latitude")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("longitude")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WritePropertyName("radius")
                jsonLine.WriteValue(reader("RadiusFeet"))

                jsonLine.WritePropertyName("iconUrl")
                jsonLine.WriteValue(reader("IconURL"))

                jsonLine.WritePropertyName("geofenceAlertTypeId")
                jsonLine.WriteValue(reader("GeofenceAlertTypeId"))

                jsonLine.WritePropertyName("geofenceAlertTypeName")
                jsonLine.WriteValue(reader("GeofenceAlertTypeName"))

                jsonLine.WritePropertyName("comments")
                jsonLine.WriteValue(reader("Comments"))

                jsonLine.WritePropertyName("shapeId")
                jsonLine.WriteValue(reader("ShapeID"))

                jsonLine.WritePropertyName("jsonPolyVerticesTXT")
                jsonLine.WriteValue(reader("jsonPolyVerticesTXT"))

                jsonLine.WritePropertyName("lastVisitedOn")
                If IsDBNull(reader("LastVisitedOn")) Then
                    jsonLine.WriteValue("N/A")
                Else
                    jsonLine.WriteValue(reader("LastVisitedOn").ToString)
                End If

                jsonLine.WritePropertyName("geofenceInfoTable")
                jsonLine.WriteValue(reader("GeofenceInfoTable"))

                jsonLine.WritePropertyName("isSpeedLimit")
                jsonLine.WriteValue(reader("IsSpeedLimit"))

                jsonLine.WritePropertyName("speedLimit")
                jsonLine.WriteValue(reader("SpeedLimit"))

                jsonLine.WritePropertyName("arrivalMsgId")
                jsonLine.WriteValue(reader("arrivalMsgId"))

                jsonLine.WritePropertyName("departureMsgId")
                jsonLine.WriteValue(reader("departureMsgId"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getGeofencesAlertsTypes(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "GeofencesAlertsTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.nvarchar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting Geofences Alerts Types"
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofencesAlertsTypes", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getGeofencesCustomMessages(ByVal token As String, ByVal msgType As Integer) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList = Nothing

        Try
            strCommand = "Geofences_CustomMessages_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parMsgType As New SqlClient.SqlParameter("@MsgType", SqlDbType.Int)
            parMsgType.Value = msgType
            Command.Parameters.Add(parMsgType)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("id")
                itm.name = reader.Item("Message")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return lst

    End Function

    Public Function getGeofencesTypes(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "GeofencesTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.nvarchar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting Geofences Types"
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofencesTypes", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function removeGeofence(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "Geofences_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.nvarchar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.nvarchar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error removing Geofence"
            BLErrorHandling.ErrorCapture(pSysModule, "removeGeofence", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function geofenceExists(ByVal token As String, ByVal name As String, ByRef strError As String) As Boolean
        Dim bExists As Boolean = False

        Try
            strCommand = "Geofences_Exists"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Direction = ParameterDirection.Input
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parExists As New SqlClient.SqlParameter("@Exists", SqlDbType.Bit)
            parExists.Direction = ParameterDirection.Output
            Command.Parameters.Add(parExists)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bExists = parExists.Value

        Catch ex As Exception
            strError = "Error Geofences_Exists"
            BLErrorHandling.ErrorCapture(pSysModule, "Geofences_Exists", "", ex.Message & " - Token: " & token, 0)
            bExists = False
        End Try

        Return bExists

    End Function

#End Region

#Region "Users"

    Public Function getUsers_DEPRECATED(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesUsers_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getUsers(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""
        Dim dsData As New DataSet

        Try
            strCommand = "CompaniesUsers_GetByToken_v2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.TableMappings.Add("Table", "Users")
            adapter.TableMappings.Add("Table1", "Modules")
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

            dsData.Relations.Add("UsrModules", dsData.Tables("Users").Columns("ID"), dsData.Tables("Modules").Columns("UserID"), False)

            Dim dvModules As DataView
            Dim drvUsr As DataRowView
            Dim drvMod As DataRowView


            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            Dim sbUser As StringBuilder = Nothing
            Dim swUser As StringWriter = Nothing
            Dim jsonUser As Newtonsoft.Json.JsonTextWriter = Nothing

            Dim sbModule As StringBuilder = Nothing
            Dim swModule As StringWriter = Nothing
            Dim jsonModule As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()

            json.WritePropertyName("users")
            json.WriteStartArray()

            For Each drvUsr In dsData.Tables("Users").DefaultView

                sbUser = New StringBuilder
                swUser = New StringWriter(sbUser)
                jsonUser = New Newtonsoft.Json.JsonTextWriter(swUser)

                jsonUser.WriteStartObject()

                jsonUser.WritePropertyName("id")
                jsonUser.WriteValue(drvUsr.Item("GUID"))

                jsonUser.WritePropertyName("name")
                jsonUser.WriteValue(drvUsr.Item("Name"))

                jsonUser.WritePropertyName("firstName")
                jsonUser.WriteValue(drvUsr.Item("FirstName"))

                jsonUser.WritePropertyName("lastName")
                jsonUser.WriteValue(drvUsr.Item("lastName"))

                jsonUser.WritePropertyName("email")
                jsonUser.WriteValue(drvUsr.Item("Email"))

                jsonUser.WritePropertyName("isEmailAlerts")
                jsonUser.WriteValue(drvUsr.Item("IsEmailAlerts"))

                jsonUser.WritePropertyName("phone")
                jsonUser.WriteValue(drvUsr.Item("Phone"))

                jsonUser.WritePropertyName("cellPhone")
                jsonUser.WriteValue(drvUsr.Item("CellPhone"))

                jsonUser.WritePropertyName("isSMSAlerts")
                jsonUser.WriteValue(drvUsr.Item("IsSMSAlerts"))

                jsonUser.WritePropertyName("carrierId")
                jsonUser.WriteValue(drvUsr.Item("SMSGatewayID"))

                jsonUser.WritePropertyName("login")
                jsonUser.WriteValue(drvUsr.Item("Login"))

                jsonUser.WritePropertyName("timeZoneCode")
                jsonUser.WriteValue(drvUsr.Item("TimeZoneCode"))

                jsonUser.WritePropertyName("isDriver")
                jsonUser.WriteValue(drvUsr.Item("IsDriver"))

                jsonUser.WritePropertyName("accessLevelId")
                jsonUser.WriteValue(drvUsr.Item("AccessLevelID"))

                jsonUser.WritePropertyName("accessLevelName")
                jsonUser.WriteValue(drvUsr.Item("AccessLevelName"))

                jsonUser.WritePropertyName("scheduleId")
                jsonUser.WriteValue(drvUsr.Item("ScheduleID"))

                jsonUser.WritePropertyName("scheduleName")
                jsonUser.WriteValue(drvUsr.Item("ScheduleName"))

                jsonUser.WritePropertyName("isAdministrator")
                jsonUser.WriteValue(drvUsr.Item("IsAdministrator"))

                jsonUser.WritePropertyName("isBillingContact")
                If IsDBNull(drvUsr.Item("isBillingContact")) Then
                    jsonUser.WriteValue(False)
                Else
                    jsonUser.WriteValue(drvUsr.Item("isBillingContact"))
                End If

                jsonUser.WritePropertyName("iButton")
                jsonUser.WriteValue(drvUsr.Item("iButton"))

                jsonUser.WritePropertyName("isAllModules")
                If IsDBNull(drvUsr.Item("isAllModules")) Then
                    jsonUser.WriteValue(1)
                Else
                    jsonUser.WriteValue(drvUsr.Item("isAllModules"))
                End If

                'Get the details of this device
                dvModules = drvUsr.CreateChildView("UsrModules")
                jsonUser.WritePropertyName("modules")
                jsonUser.WriteStartArray()

                For Each drvMod In dvModules

                    sbModule = New StringBuilder
                    swModule = New StringWriter(sbModule)
                    jsonModule = New Newtonsoft.Json.JsonTextWriter(swModule)

                    jsonModule.WriteStartObject()
                    jsonModule.WritePropertyName("moduleId")
                    jsonModule.WriteValue(drvMod.Item("ModuleID"))

                    jsonModule.WritePropertyName("name")
                    jsonModule.WriteValue(drvMod.Item("Name"))

                    jsonModule.WriteEndObject()
                    jsonModule.Flush()
                    jsonUser.WriteValue(sbModule.ToString())

                Next

                jsonUser.WriteEnd()
                jsonUser.WriteEndObject()
                jsonUser.Flush()
                json.WriteValue(sbUser.ToString())
            Next

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        End Try

        Return strJson

    End Function

    Public Function getUsersBasicInfo(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesUsers_GetBasicInfo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function saveUser(ByVal token As String, ByVal id As String, ByVal firstName As String, ByVal lastName As String, ByVal email As String, ByVal isEmailAlerts As Boolean, ByVal phone As String, ByVal cellPhone As String, ByVal isSMSAlerts As Boolean, ByVal SMSGatewayID As Integer, ByVal login As String, ByVal password As String, ByVal timeZoneCode As String, ByVal isDriver As Boolean, ByVal accessLevelId As Integer, ByVal scheduleId As String, ByVal isAdministrator As Boolean, ByVal isBillingContact As Boolean, ByVal iButton As String, ByVal isAllModules As Boolean, ByVal dtModules As DataTable, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "CompaniesUsers_UPDATE_v2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parIsEmailAlerts As New SqlClient.SqlParameter("@IsEmailAlerts", SqlDbType.Bit)
            parIsEmailAlerts.Value = isEmailAlerts
            Command.Parameters.Add(parIsEmailAlerts)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parCellPhone As New SqlClient.SqlParameter("@CellPhone", SqlDbType.NVarChar, 50)
            parCellPhone.Value = cellPhone
            Command.Parameters.Add(parCellPhone)

            Dim parIsSMSAlerts As New SqlClient.SqlParameter("@IsSMSAlerts", SqlDbType.Bit)
            parIsSMSAlerts.Value = isSMSAlerts
            Command.Parameters.Add(parIsSMSAlerts)

            Dim parSMSGatewayID As New SqlClient.SqlParameter("@SMSGatewayID", SqlDbType.Int)
            parSMSGatewayID.Value = SMSGatewayID
            Command.Parameters.Add(parSMSGatewayID)

            Dim parLogin As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 50)
            parLogin.Value = login
            Command.Parameters.Add(parLogin)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parTimeZone As New SqlClient.SqlParameter("@TimeZoneCode", SqlDbType.NVarChar, 5)
            parTimeZone.Value = timeZoneCode
            Command.Parameters.Add(parTimeZone)

            Dim parIsDriver As New SqlClient.SqlParameter("@IsDriver", SqlDbType.Bit)
            parIsDriver.Value = isDriver
            Command.Parameters.Add(parIsDriver)

            Dim parAccessLevelID As New SqlClient.SqlParameter("@AccessLevelID", SqlDbType.Int)
            parAccessLevelID.Value = accessLevelId
            Command.Parameters.Add(parAccessLevelID)

            Dim parScheduleID As New SqlClient.SqlParameter("@ScheduleID", SqlDbType.NVarChar, 50)
            parScheduleID.Value = scheduleId
            Command.Parameters.Add(parScheduleID)

            Dim parIsAdministrator As New SqlClient.SqlParameter("@IsAdministrator", SqlDbType.Bit)
            parIsAdministrator.Value = isAdministrator
            Command.Parameters.Add(parIsAdministrator)

            Dim parIsBillingContact As New SqlClient.SqlParameter("@IsBillingContact", SqlDbType.Bit)
            parIsBillingContact.Value = isBillingContact
            Command.Parameters.Add(parIsBillingContact)

            Dim parIButton As New SqlClient.SqlParameter("@IButton", SqlDbType.VarChar, 50)
            parIButton.Value = iButton
            Command.Parameters.Add(parIButton)

            Dim parIsAllModules As New SqlClient.SqlParameter("@IsAllModules", SqlDbType.Bit)
            parIsAllModules.Value = isAllModules
            Command.Parameters.Add(parIsAllModules)

            Dim parModules As New SqlClient.SqlParameter("@ModulesList", SqlDbType.Structured)
            parModules.Value = dtModules
            Command.Parameters.Add(parModules)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            If ex.Message = "Login is not available. Please try a different login." Then
                strError = ex.Message
            Else
                strError = "Error saving New User"
            End If
            BLErrorHandling.ErrorCapture(pSysModule, "saveUser", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function changeUserCredentials(ByVal token As String, ByVal id As String, ByVal login As String, ByVal password As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "CompaniesUsers_CredentialsUpdate"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.nvarchar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parLogin As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 50)
            parLogin.Value = login
            Command.Parameters.Add(parLogin)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.nvarchar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error saving new credentials"
            BLErrorHandling.ErrorCapture(pSysModule, "changeUserCredentials", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function removeUser(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "CompaniesUsers_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.nvarchar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.nvarchar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error removing user"
            BLErrorHandling.ErrorCapture(pSysModule, "removeUser", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function recoverCredentials(ByVal email As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            email = BLCommon.Sanitize(email)

            strCommand = "CompaniesUsers_RecoverCredentials"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error retieving credentials"
            BLErrorHandling.ErrorCapture(pSysModule, "recoverCredentials", "", ex.Message & " - email: " & email, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function getNotificationsSendTo(ByVal token As String, ByVal entityName As String, ByVal entityId As String, ByRef msgError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "GetNotificationsSendTo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEntityName As New SqlClient.SqlParameter("@EntityName", SqlDbType.NVarChar, 50)
            parEntityName.Direction = ParameterDirection.Input
            parEntityName.Value = entityName
            Command.Parameters.Add(parEntityName)

            Dim parEntityId As New SqlClient.SqlParameter("@EntityId", SqlDbType.NVarChar, 50)
            parEntityId.Direction = ParameterDirection.Input
            parEntityId.Value = entityId
            Command.Parameters.Add(parEntityId)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

#End Region

#Region "Alerts"

    Public Function getAlerts(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Alerts_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getAlertsTypes(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "AlertsTypes_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function saveAlert(ByVal token As String, ByVal id As String, ByVal typeId As Integer, ByVal alertName As String, ByVal GeofenceGUID As String,
                              ByVal val As Integer, ByVal isAllDevices As Boolean, ByVal dtDevices As DataTable, ByVal isAllUsers As Boolean, ByVal dtUsers As DataTable,
                              ByVal bMon As Boolean, ByVal bTue As Boolean, ByVal bWed As Boolean, ByVal bThu As Boolean, ByVal bFri As Boolean, ByVal bSat As Boolean, ByVal bSun As Boolean,
                              ByVal hourFrom As Integer, ByVal hourTo As Integer,
                              ByVal minInterval As Integer, ByVal setPointMin As Integer, ByVal setPointMax As Integer, ByRef strError As String) As String
        Dim alertGUID As String = ""

        Try
            strCommand = "Alerts_SAVE_v2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parTypeID.Value = typeId
            Command.Parameters.Add(parTypeID)

            Dim parAlertName As New SqlClient.SqlParameter("@AlertName", SqlDbType.NVarChar, 50)
            parAlertName.Value = alertName
            Command.Parameters.Add(parAlertName)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Value = GeofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            Dim parVal As New SqlClient.SqlParameter("@Value", SqlDbType.Int)
            parVal.Value = val
            Command.Parameters.Add(parVal)

            Dim parIsAllDevices As New SqlClient.SqlParameter("@IsAllDevices", SqlDbType.Bit)
            parIsAllDevices.Value = isAllDevices
            Command.Parameters.Add(parIsAllDevices)

            Dim parDevices As New SqlClient.SqlParameter("@DevicesLst", SqlDbType.Structured)
            parDevices.Value = dtDevices
            Command.Parameters.Add(parDevices)

            Dim parIsAllUsers As New SqlClient.SqlParameter("@IsAllUsers", SqlDbType.Bit)
            parIsAllUsers.Value = isAllUsers
            Command.Parameters.Add(parIsAllUsers)

            Dim parUsers As New SqlClient.SqlParameter("@AlertUsersLst", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parMon As New SqlClient.SqlParameter("@Mon", SqlDbType.Bit)
            parMon.Value = bMon
            Command.Parameters.Add(parMon)

            Dim parTue As New SqlClient.SqlParameter("@Tue", SqlDbType.Bit)
            parTue.Value = bTue
            Command.Parameters.Add(parTue)

            Dim parWed As New SqlClient.SqlParameter("@Wed", SqlDbType.Bit)
            parWed.Value = bWed
            Command.Parameters.Add(parWed)

            Dim parThu As New SqlClient.SqlParameter("@Thu", SqlDbType.Bit)
            parThu.Value = bThu
            Command.Parameters.Add(parThu)

            Dim parFri As New SqlClient.SqlParameter("@Fri", SqlDbType.Bit)
            parFri.Value = bFri
            Command.Parameters.Add(parFri)

            Dim parSat As New SqlClient.SqlParameter("@Sat", SqlDbType.Bit)
            parSat.Value = bSat
            Command.Parameters.Add(parSat)

            Dim parSun As New SqlClient.SqlParameter("@Sun", SqlDbType.Bit)
            parSun.Value = bSun
            Command.Parameters.Add(parSun)

            Dim parHourFrom As New SqlClient.SqlParameter("@HourFrom", SqlDbType.Int, 4)
            parHourFrom.Value = hourFrom
            Command.Parameters.Add(parHourFrom)

            Dim parHourTo As New SqlClient.SqlParameter("@HourTo", SqlDbType.Int, 4)
            parHourTo.Value = hourTo
            Command.Parameters.Add(parHourTo)

            Dim parMinInterval As New SqlClient.SqlParameter("@MinInterval", SqlDbType.Int, 4)
            parMinInterval.Value = minInterval
            Command.Parameters.Add(parMinInterval)

            Dim parSetPointMin As New SqlClient.SqlParameter("@SetPointMin", SqlDbType.Int, 4)
            parSetPointMin.Value = setPointMin
            Command.Parameters.Add(parSetPointMin)

            Dim parSetPointMax As New SqlClient.SqlParameter("@SetPointMax", SqlDbType.Int, 4)
            parSetPointMax.Value = setPointMax
            Command.Parameters.Add(parSetPointMax)

            Dim parAlertGUID As New SqlClient.SqlParameter("@AlertGUID", SqlDbType.NVarChar, 50)
            parAlertGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parAlertGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            alertGUID = CStr(parAlertGUID.Value)

        Catch ex As Exception
            strError = "Error saving alert"
            BLErrorHandling.ErrorCapture(pSysModule, "saveAlert", "", ex.Message & " - Token: " & token, 0)
            alertGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return alertGUID

    End Function

    Public Function removeAlert(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "Alerts_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error removing alert"
            BLErrorHandling.ErrorCapture(pSysModule, "removeAlert", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function getAlertDevices(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "AlertsDevices_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@AlertGUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getAlertUsers(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "AlertsSendTo_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@AlertGUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

#End Region

#Region "Dispatch Jobs "

    Public Function saveWorkOrder(ByVal token As String, ByVal deviceId As String, ByVal driverId As String, ByVal isGeofence As String, ByVal geofenceGUID As String, ByVal name As String, ByVal phone As String, ByVal street As String, ByVal city As String, ByVal state As String, ByVal postalCode As String, ByVal jobDescription As String, ByVal jobAddress As String, ByVal jobMap As String, ByVal lat As Decimal, ByVal lng As Decimal, ByVal sendSMS As Boolean, ByVal smsMessage As String, ByVal via As Integer, ByRef strError As String) As String
        Dim JobGUID As String = ""

        Try
            'strCommand = "SendJobToDeviceViaSMS"
            'strCommand = "WorkOrders_QuickDispatch_SAVE"
            strCommand = "Jobs_QuickDispatch_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 4)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDriverId As New SqlClient.SqlParameter("@DriverID", SqlDbType.NVarChar, 50)
            parDriverId.Value = driverId
            Command.Parameters.Add(parDriverId)

            Dim parIsGeofence As New SqlClient.SqlParameter("@IsGeofence", SqlDbType.Bit)
            If isGeofence.ToLower = "true" Then
                parIsGeofence.Value = True
            Else
                parIsGeofence.Value = False
            End If
            Command.Parameters.Add(parIsGeofence)

            Dim parGeofenceID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.VarChar, 50)
            parGeofenceID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 15)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parJobDescription As New SqlClient.SqlParameter("@JobDescription", SqlDbType.NVarChar, 500)
            parJobDescription.Value = jobDescription
            Command.Parameters.Add(parJobDescription)

            Dim parJobAddress As New SqlClient.SqlParameter("@JobAddress", SqlDbType.NVarChar, 200)
            parJobAddress.Value = jobAddress
            Command.Parameters.Add(parJobAddress)

            Dim parJobMap As New SqlClient.SqlParameter("@JobMap", SqlDbType.NVarChar, 100)
            parJobMap.Value = jobMap
            Command.Parameters.Add(parJobMap)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parSendSMS As New SqlClient.SqlParameter("@SendSMS", SqlDbType.Bit)
            parSendSMS.Value = sendSMS
            Command.Parameters.Add(parSendSMS)

            Dim parSMS As New SqlClient.SqlParameter("@smsMessage", SqlDbType.NVarChar, 500)
            parSMS.Value = smsMessage
            Command.Parameters.Add(parSMS)

            Dim parVia As New SqlClient.SqlParameter("@VIA", SqlDbType.Int)
            parVia.Value = via
            Command.Parameters.Add(parVia)

            Dim parGUID As New SqlClient.SqlParameter("@JobGUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            JobGUID = CStr(parGUID.Value)

        Catch ex As Exception
            strError = "Error saveWorkOrder"
            BLErrorHandling.ErrorCapture(pSysModule, "saveWorkOrder", "", ex.Message & " - Token: " & token, 0)
            JobGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return JobGUID

    End Function

#End Region

#Region "Driving Directions"

    Public Function sendDrivingDirectionsEmail(ByVal Token As String, ByVal DeviceID As String, ByVal dd As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = False
        Dim EmailID As Integer = 0

        Try
            strCommand = "EmailsOutbox_DrivingDirections_INSERT"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = DeviceID
            Command.Parameters.Add(parDeviceID)

            Dim parDD As New SqlClient.SqlParameter("@Content", SqlDbType.NVarChar, 5000)
            parDD.Value = dd
            Command.Parameters.Add(parDD)

            Dim parEmailID As New SqlClient.SqlParameter("@EmailID", SqlDbType.Int)
            parEmailID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parEmailID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            EmailID = CInt(parEmailID.Value)
            If EmailID <= 0 Then
                bResults = False
                strError = "Could not send email.  Check that a driver is assigned to the vehicle."
            Else
                bResults = True
            End If

        Catch ex As Exception
            bResults = False
            strError = "Error sending email"
            BLErrorHandling.ErrorCapture(pSysModule, "sendDrivingDirectionsEmail", "", ex.Message & " - Token: " & Token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Recurrent Reports"

    Public Function saveRecurrentReport(ByVal token As String, ByVal id As String, ByVal reportId As Integer, ByVal param As String, ByVal frequencyId As Integer, ByVal ExcludeWeekends As Boolean, ByVal isAllDevices As Boolean, ByVal dtDevices As DataTable, ByVal isAllUsers As Boolean, ByVal dtUsers As DataTable, ByRef strError As String) As String
        Dim recurrentGUID As String = ""

        Try
            strCommand = "ReportsRecurrent_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parReportId As New SqlClient.SqlParameter("@ReportId", SqlDbType.Int)
            parReportId.Value = reportId
            Command.Parameters.Add(parReportId)

            Dim parParam As New SqlClient.SqlParameter("@Param", SqlDbType.NVarChar, 10)
            parParam.Value = param
            Command.Parameters.Add(parParam)

            Dim parFrequencyId As New SqlClient.SqlParameter("@FrequencyId", SqlDbType.Int)
            parFrequencyId.Value = frequencyId
            Command.Parameters.Add(parFrequencyId)

            Dim parExcludeWeekends As New SqlClient.SqlParameter("@ExcludeWeekends", SqlDbType.Bit)
            parExcludeWeekends.Value = ExcludeWeekends
            Command.Parameters.Add(parExcludeWeekends)

            Dim parIsAllDevices As New SqlClient.SqlParameter("@IsAllDevices", SqlDbType.Bit)
            parIsAllDevices.Value = isAllDevices
            Command.Parameters.Add(parIsAllDevices)

            Dim parDevices As New SqlClient.SqlParameter("@DevicesLst", SqlDbType.Structured)
            parDevices.Value = dtDevices
            Command.Parameters.Add(parDevices)

            Dim parIsAllUsers As New SqlClient.SqlParameter("@IsAllUsers", SqlDbType.Bit)
            parIsAllUsers.Value = isAllUsers
            Command.Parameters.Add(parIsAllUsers)

            Dim parUsers As New SqlClient.SqlParameter("@UsersGUIDLst", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parAlertGUID As New SqlClient.SqlParameter("@RecurrentGUID", SqlDbType.NVarChar, 50)
            parAlertGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parAlertGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            recurrentGUID = CStr(parAlertGUID.Value)

        Catch ex As Exception
            strError = "Error saving recurrent report"
            BLErrorHandling.ErrorCapture(pSysModule, "saveRecurrentReport", "", ex.Message & " - Token: " & token, 0)
            recurrentGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return recurrentGUID

    End Function

    Public Function getReports(ByVal token As String, ByVal IsRecurrent As Boolean, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Reports_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIsRecurrent As New SqlClient.SqlParameter("@ForRecurrent", SqlDbType.Bit)
            parIsRecurrent.Direction = ParameterDirection.Input
            parIsRecurrent.Value = IsRecurrent
            Command.Parameters.Add(parIsRecurrent)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting reports"
            BLErrorHandling.ErrorCapture(pSysModule, "getReports", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getRecurrentReports(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "ReportsRecurrent_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting recurrent reports"
            BLErrorHandling.ErrorCapture(pSysModule, "getRecurrentReports", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getRecurrentFrequencies(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "ReportsRecurrentFrequency_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting reports recurrence frequencies"
            BLErrorHandling.ErrorCapture(pSysModule, "getRecurrentFrequencies", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getRecRepDevices(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "ReportsRecurrentDevices_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@RecurrentGUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getRecRepUsers(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "ReportsRecurrentSendTo_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@RecurrentGUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function removeRecurrentReport(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "ReportsRecurrent_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error removing Recurrent Report"
            BLErrorHandling.ErrorCapture(pSysModule, "removeRecurrentReport", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

#End Region

#Region "Maintenance module"

    Public Function getMaintSchedules(ByVal Token As String, ByVal deviceId As String, ByVal taskId As Integer, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_Schedules_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int, 4)
            parTaskID.Direction = ParameterDirection.Input
            parTaskID.Value = taskId
            Command.Parameters.Add(parTaskID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getMaintenanceTasks(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_Tasks_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting maintenance tasks"
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintenanceTasks", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getHServices(ByVal Token As String, ByVal DeviceID As String, ByVal TaskID As Integer, _
                                 ByVal dateFrom As Date, ByVal dateTo As Date, _
                                 ByRef errMsg As String) As List(Of maintHServices)
        Dim lst As New List(Of maintHServices)
        Dim itm As maintHServices

        Try
            strCommand = "Maint_ServicesLog_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = DeviceID
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int)
            parTaskID.Value = TaskID
            Command.Parameters.Add(parTaskID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.Date)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.Date)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm = New maintHServices
                itm.id = reader.Item("ID")
                itm.deviceName = reader.Item("DeviceName")
                itm.taskName = reader.Item("TaskName")
                itm.serviceDate = reader.Item("ServiceDate")
                itm.serviceType = reader.Item("ServiceType")
                itm.odometer = reader.Item("Odometer")
                itm.cost = reader.Item("TotalCost")
                itm.comments = reader.Item("Comments")

                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function getHFuel(ByVal Token As String, ByVal DeviceID As String, ByVal dateFrom As Date, _
                             ByVal dateTo As Date, ByRef errMsg As String) As List(Of maintHFuel)
        Dim lst As New List(Of maintHFuel)
        Dim itm As maintHFuel

        Try
            strCommand = "Maint_FuelLog_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = DeviceID
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.Date)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.Date)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm = New maintHFuel
                itm.id = reader.Item("ID")
                itm.deviceName = reader.Item("DeviceName")
                itm.fuelDate = reader.Item("FuelDate")
                itm.odometer = reader.Item("Odometer")
                itm.gallons = reader.Item("Gallons")
                itm.cost = reader.Item("TotalCost")
                itm.stateName = reader.Item("StateName")
                itm.comments = reader.Item("Comments")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function getMaintServicesTypes(ByVal token As String, ByRef strError As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Maint_ServicesTypes_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("types")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            strError = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getMaintTasksMeassures(ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_TasksMeassures_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting maintenance tasks meassures"
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintTasksMeassures", "", ex.Message, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function removeTask(ByVal token As String, ByVal id As Integer, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_Tasks_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error removing Task"
            BLErrorHandling.ErrorCapture(pSysModule, "removeTask", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function saveMaintTask(ByVal token As String, ByVal id As Integer, ByVal name As String, ByVal meassureId As Integer, ByVal value As Decimal, ByVal dtUsers As DataTable, ByRef strError As String) As String
        Dim taskId As Integer = 0

        Try
            strCommand = "Maint_Tasks_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parMeassureId As New SqlClient.SqlParameter("@MeassureId", SqlDbType.Int)
            parMeassureId.Value = meassureId
            Command.Parameters.Add(parMeassureId)

            Dim parValue As New SqlClient.SqlParameter("@Value", SqlDbType.Real)
            parValue.Value = value
            Command.Parameters.Add(parValue)

            Dim parUsers As New SqlClient.SqlParameter("@UsersLst", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parTaskId As New SqlClient.SqlParameter("@TaskId", SqlDbType.Int)
            parTaskId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTaskId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            taskId = CInt(parTaskId.Value)

        Catch ex As Exception
            strError = "Error saving task"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintTask", "", ex.Message & " - Token: " & token, 0)
            taskId = 0
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return taskId

    End Function

    Public Function saveMaintSchedule(ByVal token As String, ByVal id As String, ByVal deviceId As String, ByVal taskId As Integer, ByVal taskValue As Decimal, ByVal lastServiceOn As Date, _
                                      ByVal currentValue As Decimal, ByVal notifyBefore As Decimal, ByVal notifyEveryXDays As Integer, ByVal excludeWeekends As Boolean, _
                                      ByVal dtUsers As DataTable, ByRef strError As String) As String
        Dim scheduleGUID As String = ""

        Try
            strCommand = "Maint_Schedules_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int)
            parTaskID.Value = taskId
            Command.Parameters.Add(parTaskID)

            Dim parTaskValue As New SqlClient.SqlParameter("@TaskValue", SqlDbType.Decimal)
            parTaskValue.Value = taskValue
            Command.Parameters.Add(parTaskValue)

            Dim parLastServiceOn As New SqlClient.SqlParameter("@LastServiceOn", SqlDbType.Date)
            parLastServiceOn.Value = lastServiceOn
            Command.Parameters.Add(parLastServiceOn)

            Dim parCurrentValue As New SqlClient.SqlParameter("@CurrentValue", SqlDbType.Decimal)
            parCurrentValue.Value = currentValue
            Command.Parameters.Add(parCurrentValue)

            Dim parNotifyBefore As New SqlClient.SqlParameter("@NotifyBefore", SqlDbType.Decimal)
            parNotifyBefore.Value = notifyBefore
            Command.Parameters.Add(parNotifyBefore)

            Dim parNotifyEveryXDays As New SqlClient.SqlParameter("@NotifyEveryXDays", SqlDbType.Int)
            parNotifyEveryXDays.Value = notifyEveryXDays
            Command.Parameters.Add(parNotifyEveryXDays)

            Dim parExcludeWeekends As New SqlClient.SqlParameter("@ExcludeWeekends", SqlDbType.Bit)
            parExcludeWeekends.Value = excludeWeekends
            Command.Parameters.Add(parExcludeWeekends)

            Dim parUsers As New SqlClient.SqlParameter("@UsersLst", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parScheduleGUID As New SqlClient.SqlParameter("@ScheduleGUID", SqlDbType.NVarChar, 50)
            parScheduleGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parScheduleGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            scheduleGUID = CStr(parScheduleGUID.Value)

        Catch ex As Exception
            strError = "Error saving schedule"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintSchedule", "", ex.Message & " - Token: " & token, 0)
            scheduleGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return scheduleGUID

    End Function

    Public Function getMaintAlertUsers(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_SchedulesSendTo_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function saveMaintServiceLog(ByVal token As String, ByVal id As String, ByVal deviceId As String, ByVal ServiceTypeID As Integer, ByVal taskId As Integer, _
                                        ByVal ServiceDescription As String, ByVal serviceDate As Date, ByVal odometer As Decimal, ByVal cost As Decimal, ByVal meassureValueOnDayOfService As Decimal, _
                                        ByVal comments As String, _
                                        ByRef strError As String) As String
        Dim serviceLogGUID As String = ""

        Try
            strCommand = "Maint_ServicesLog_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parServiceTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parServiceTypeID.Value = ServiceTypeID
            Command.Parameters.Add(parServiceTypeID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int)
            parTaskID.Value = taskId
            Command.Parameters.Add(parTaskID)

            Dim parServiceDescription As New SqlClient.SqlParameter("@ServiceDescription", SqlDbType.NVarChar, -1)
            parServiceDescription.Value = ServiceDescription
            Command.Parameters.Add(parServiceDescription)

            Dim parServiceDate As New SqlClient.SqlParameter("@ServiceDate", SqlDbType.Date)
            parServiceDate.Value = serviceDate
            Command.Parameters.Add(parServiceDate)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parCost As New SqlClient.SqlParameter("@Cost", SqlDbType.Decimal)
            parCost.Value = cost
            Command.Parameters.Add(parCost)

            Dim parComments As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parComments.Value = comments
            Command.Parameters.Add(parComments)

            Dim parVal As New SqlClient.SqlParameter("@Val", SqlDbType.Decimal)
            parVal.Precision = 18
            parVal.Scale = 2
            parVal.Value = meassureValueOnDayOfService
            Command.Parameters.Add(parVal)

            Dim parServiceLogGUID As New SqlClient.SqlParameter("@ServiceLogGUID", SqlDbType.NVarChar, 50)
            parServiceLogGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parServiceLogGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            serviceLogGUID = CStr(parServiceLogGUID.Value)

        Catch ex As Exception
            strError = "Error saving service log"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintServiceLog", "", ex.Message & " - Token: " & token, 0)
            serviceLogGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return serviceLogGUID

    End Function

    Public Function deleteMaintServiceLog(ByVal token As String, ByVal id As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_ServicesLog_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Service Log"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteMaintServiceLog", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function saveMaintFuelLog(ByVal token As String, ByVal id As String, ByVal deviceId As String, _
                                     ByVal fuelingDate As Date, ByVal odometer As Decimal, ByVal gallons As Decimal, ByVal cost As Decimal, ByVal stateId As Integer, ByVal comments As String, _
                                     ByRef strError As String) As String
        Dim FuelLogGUID As String = ""

        Try
            strCommand = "Maint_fuelLog_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parFuelingDate As New SqlClient.SqlParameter("@FuelingDate", SqlDbType.DateTime)
            parFuelingDate.Value = fuelingDate
            Command.Parameters.Add(parFuelingDate)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parGallons As New SqlClient.SqlParameter("@Gallons", SqlDbType.Decimal)
            parGallons.Value = gallons
            Command.Parameters.Add(parGallons)

            Dim parCost As New SqlClient.SqlParameter("@Cost", SqlDbType.Decimal)
            parCost.Value = cost
            Command.Parameters.Add(parCost)

            Dim parStateID As New SqlClient.SqlParameter("@StateID", SqlDbType.Int)
            parStateID.Value = stateId
            Command.Parameters.Add(parStateID)

            Dim parComments As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parComments.Value = comments
            Command.Parameters.Add(parComments)

            Dim parFuelLogGUID As New SqlClient.SqlParameter("@FuelLogGUID", SqlDbType.NVarChar, 50)
            parFuelLogGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parFuelLogGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            FuelLogGUID = CStr(parFuelLogGUID.Value)

        Catch ex As Exception
            strError = "Error saving fuel log"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintFuelLog", "", ex.Message & " - Token: " & token, 0)
            FuelLogGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return FuelLogGUID

    End Function

    Public Function deleteMaintFuelLog(ByVal token As String, ByVal id As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_FuelLog_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Fuel Log"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteMaintFuelLog", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function deleteMaintSchedule(ByVal token As String, ByVal id As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_Schedules_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Maintenance Schedule"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteMaintSchedule", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Schedules"

    Public Function saveSchedule(ByVal token As String, ByVal id As String, ByVal scheduleName As String, ByVal bHasExceptionDays As Boolean, ByVal ApplyGlobalExceptions As Boolean, ByVal dtValues As DataTable, ByRef strError As String) As String
        Dim scheduleGUID As String = ""

        Try
            strCommand = "Schedules_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@scheduleName", SqlDbType.NVarChar, 50)
            parName.Value = scheduleName
            Command.Parameters.Add(parName)

            Dim parHasExceptionDays As New SqlClient.SqlParameter("@HasExceptionDays", SqlDbType.Bit)
            parHasExceptionDays.Value = bHasExceptionDays
            Command.Parameters.Add(parHasExceptionDays)

            Dim parApplyGlobalExceptions As New SqlClient.SqlParameter("@ApplyGlobalExceptions", SqlDbType.Bit)
            parApplyGlobalExceptions.Value = ApplyGlobalExceptions
            Command.Parameters.Add(parApplyGlobalExceptions)

            Dim parValues As New SqlClient.SqlParameter("@ScheduleBlocksLst", SqlDbType.Structured)
            parValues.Value = dtValues
            Command.Parameters.Add(parValues)

            Dim parScheduleGUID As New SqlClient.SqlParameter("@ScheduleGUID", SqlDbType.NVarChar, 50)
            parScheduleGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parScheduleGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            scheduleGUID = CStr(parScheduleGUID.Value)

        Catch ex As Exception
            strError = "Error saving schedule"
            BLErrorHandling.ErrorCapture(pSysModule, "saveSchedule", "", ex.Message & " - Token: " & token, 0)
            scheduleGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return scheduleGUID

    End Function

    Public Function getSchedules(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Schedules_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("schedules")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn").ToString)

                jsonLine.WritePropertyName("hasExceptionDays")
                jsonLine.WriteValue(reader("HasExceptionDays"))

                jsonLine.WritePropertyName("applyGlobalExceptions")
                jsonLine.WriteValue(reader("ApplyGlobalExceptions"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getScheduleDays(ByVal Token As String, ByVal scheduleID As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "ScheduleDays_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ScheduleID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = scheduleID
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("scheduleDays")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("dayId")
                jsonLine.WriteValue(reader("DayID"))

                jsonLine.WritePropertyName("hourFrom")
                jsonLine.WriteValue(reader("HourFrom"))

                jsonLine.WritePropertyName("hourTo")
                jsonLine.WriteValue(reader("HourTo"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

#End Region

#Region "iButtons"

    Public Function getIButtons(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "iButtons_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("ibuttons")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("iButtonHex")
                jsonLine.WriteValue(reader("IButtonHEX"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("iButtonType")
                jsonLine.WriteValue(reader("iButtonType"))

                jsonLine.WritePropertyName("iButtonTypeName")
                jsonLine.WriteValue(reader("iButtonTypeName"))

                jsonLine.WritePropertyName("logoutEventCode")
                jsonLine.WriteValue(reader("LogoutEventCode"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn").ToString)

                jsonLine.WritePropertyName("assignedToId")
                jsonLine.WriteValue(reader("AssignedToId"))

                jsonLine.WritePropertyName("assignedToName")
                jsonLine.WriteValue(reader("AssignedToName"))

                jsonLine.WritePropertyName("isUsedWithoutDriver")
                jsonLine.WriteValue(reader("IsUsedWithoutDriver"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function removeIButton(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim iButtonID As String = ""

        Try
            strCommand = "iButtons_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parIButtonID As New SqlClient.SqlParameter("@iButtonID", SqlDbType.NVarChar, 50)
            parIButtonID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIButtonID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            iButtonID = parIButtonID.Value

        Catch ex As Exception
            strError = "Error removing iButton"
            BLErrorHandling.ErrorCapture(pSysModule, "removeIButton", "", ex.Message & " - Token: " & token, 0)
            iButtonID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return iButtonID

    End Function

    Public Function saveIButton(ByVal token As String, ByVal id As String, ByVal iButtonHEX As String, ByVal name As String, ByVal type As Integer, ByVal assignedToID As String, ByRef strError As String) As String
        Dim IButtonID As String = ""

        Try
            strCommand = "iButtons_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parIDHEX As New SqlClient.SqlParameter("@IDHEX", SqlDbType.NVarChar, 50)
            parIDHEX.Value = iButtonHEX
            Command.Parameters.Add(parIDHEX)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parType As New SqlClient.SqlParameter("@Type", SqlDbType.Int)
            parType.Value = type
            Command.Parameters.Add(parType)

            Dim parAssignedToID As New SqlClient.SqlParameter("@AssignedToID", SqlDbType.NVarChar, 50)
            parAssignedToID.Value = assignedToID
            Command.Parameters.Add(parAssignedToID)

            Dim parIbuttonID As New SqlClient.SqlParameter("@IButtonID", SqlDbType.NVarChar, 50)
            parIbuttonID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIbuttonID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            IButtonID = parIbuttonID.Value

        Catch ex As Exception
            strError = "Error saving iButton"
            BLErrorHandling.ErrorCapture(pSysModule, "saveIButton", "", ex.Message & " - Token: " & token, 0)
            IButtonID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return IButtonID

    End Function

#End Region

#Region "Devices Groups"

    Public Function getDevGroups(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "DevicesGroups_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("groups")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("isPublic")
                jsonLine.WriteValue(reader("isPublic"))

                jsonLine.WritePropertyName("isAllDevices")
                jsonLine.WriteValue(reader("isAllDevices"))

                jsonLine.WritePropertyName("isAllUsers")
                jsonLine.WriteValue(reader("isAllUsers"))

                jsonLine.WritePropertyName("hasSpeedGauge")
                jsonLine.WriteValue(reader("hasSpeedGauge"))

                jsonLine.WritePropertyName("isDefault")
                jsonLine.WriteValue(reader("isDefault"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn").ToString)

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getDevGroupDevices(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "DevicesGroups_Devices_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@GroupGUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getDevGroupUsers(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "DevicesGroups_Users_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@GroupGUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function saveDeviceGroup(ByVal token As String, ByVal ID As String, ByVal name As String, ByVal isAllDevices As Boolean, ByVal dtDevices As DataTable, ByVal isAllUsers As Boolean, ByVal dtUsers As DataTable, ByVal hasSpeedGauge As Boolean, ByRef strError As String) As String
        Dim GroupGUID As String = ""

        Try
            strCommand = "DevicesGroups_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = ID
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 20)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parIsAllDevices As New SqlClient.SqlParameter("@IsAllDevices", SqlDbType.Bit)
            parIsAllDevices.Value = isAllDevices
            Command.Parameters.Add(parIsAllDevices)

            Dim parDevices As New SqlClient.SqlParameter("@Devices", SqlDbType.Structured)
            parDevices.Value = dtDevices
            Command.Parameters.Add(parDevices)

            Dim parIsAllUsers As New SqlClient.SqlParameter("@IsAllUsers", SqlDbType.Bit)
            parIsAllUsers.Value = isAllUsers
            Command.Parameters.Add(parIsAllUsers)

            Dim parUsers As New SqlClient.SqlParameter("@Users", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parHasSG As New SqlClient.SqlParameter("@HasSpeedGauge", SqlDbType.Bit)
            parHasSG.Value = hasSpeedGauge
            Command.Parameters.Add(parHasSG)

            Dim parGroupGUID As New SqlClient.SqlParameter("@GroupGUID", SqlDbType.NVarChar, 50)
            parGroupGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGroupGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GroupGUID = CStr(parGroupGUID.Value)

        Catch ex As Exception
            strError = "Error saving New Group"
            BLErrorHandling.ErrorCapture(pSysModule, "saveDeviceGroup", "", ex.Message & " - Token: " & token, 0)
            GroupGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GroupGUID

    End Function

    Public Function deleteDeviceGroup(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "DevicesGroups_DELETE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error removing DeviceGroup"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteDeviceGroup", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

#End Region

#Region "Company Info Module"

    Public Function getCompanyInfo(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""
        Dim isFound As Boolean = False

        Try
            strCommand = "Companies_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("companyInfo")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                isFound = True

                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("result")
                jsonLine.WriteValue(True)

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("uniqueKey"))

                jsonLine.WritePropertyName("companyName")
                jsonLine.WriteValue(reader("CompanyName"))

                jsonLine.WritePropertyName("phone")
                jsonLine.WriteValue(reader("Phone"))

                jsonLine.WritePropertyName("website")
                jsonLine.WriteValue(reader("Website"))

                jsonLine.WritePropertyName("street")
                jsonLine.WriteValue(reader("Street"))

                jsonLine.WritePropertyName("city")
                jsonLine.WriteValue(reader("City"))

                jsonLine.WritePropertyName("state")
                jsonLine.WriteValue(reader("State"))

                jsonLine.WritePropertyName("postalCode")
                jsonLine.WriteValue(reader("PostalCode"))

                jsonLine.WritePropertyName("countryCode")
                jsonLine.WriteValue(reader("CountryCode"))

                jsonLine.WritePropertyName("industry")
                jsonLine.WriteValue(reader("Industry"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn").ToString)

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            If isFound = False Then
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("result")
                jsonLine.WriteValue(False)

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())

            End If

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function saveCompanyInfo(ByVal Token As String, ByVal name As String, ByVal phone As String, ByVal website As String, ByVal industry As String, ByVal street As String, ByVal city As String, ByVal state As String, ByVal postalCode As String, ByVal countryCode As String, ByVal lat As Decimal, ByVal lng As Decimal, ByRef errMsg As String) As Boolean
        Dim strError As String = ""
        Dim bOk As Boolean = True

        Try
            strCommand = "Companies_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parWebSite As New SqlClient.SqlParameter("@WebSite", SqlDbType.NVarChar, 100)
            parWebSite.Value = website
            Command.Parameters.Add(parWebSite)

            Dim parIndustry As New SqlClient.SqlParameter("@Industry", SqlDbType.NVarChar, -1)
            parIndustry.Value = industry
            Command.Parameters.Add(parIndustry)

            Dim parStreet As New SqlClient.SqlParameter("@ShipStreet", SqlDbType.NVarChar, 100)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@ShipCity", SqlDbType.NVarChar, 50)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@ShipState", SqlDbType.NVarChar, 50)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@ShipPostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountryCode As New SqlClient.SqlParameter("@ShipCountryCode", SqlDbType.NVarChar, 50)
            parCountryCode.Value = countryCode
            Command.Parameters.Add(parCountryCode)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error saving company info"
            BLErrorHandling.ErrorCapture(pSysModule, "saveCompanyInfo", "", ex.Message & " - Token: " & Token, 0)
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

    Public Function getCompanyInfo2(ByVal uid As String)

    End Function

#End Region

#Region "Billing Info Module"

    Public Function getCCInfo(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""
        Dim isFound As Boolean = False

        Try
            strCommand = "Companies_Billing_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("ccInfo")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                isFound = True

                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("result")
                jsonLine.WriteValue(True)

                jsonLine.WritePropertyName("ccType")
                jsonLine.WriteValue(reader("ccType"))

                jsonLine.WritePropertyName("ccNumber")
                jsonLine.WriteValue(reader("ccNumber"))

                jsonLine.WritePropertyName("ccSecCode")
                jsonLine.WriteValue(reader("ccSecCode"))

                jsonLine.WritePropertyName("ccExpMonth")
                jsonLine.WriteValue(reader("ccExpMonth"))

                jsonLine.WritePropertyName("ccExpYear")
                jsonLine.WriteValue(reader("ccExpYear"))

                jsonLine.WritePropertyName("ccFirstName")
                jsonLine.WriteValue(reader("ccFirstName"))

                jsonLine.WritePropertyName("ccLastName")
                jsonLine.WriteValue(reader("ccLastName"))

                jsonLine.WritePropertyName("ccStreet")
                jsonLine.WriteValue(reader("ccStreet"))

                jsonLine.WritePropertyName("ccCity")
                jsonLine.WriteValue(reader("ccCity"))

                jsonLine.WritePropertyName("ccState")
                jsonLine.WriteValue(reader("ccState"))

                jsonLine.WritePropertyName("ccPostalCode")
                jsonLine.WriteValue(reader("ccPostalCode"))

                jsonLine.WritePropertyName("ccCountryCode")
                jsonLine.WriteValue(reader("ccCountryCode"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            If isFound = False Then
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("result")
                jsonLine.WriteValue(False)

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())

            End If

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function saveBillingInfo(ByVal Token As String, ByVal billingContact As String, ByVal billingEmail As String, ByVal billingPhone As String, ByVal type As String, ByVal number As String, ByVal secCode As String, ByVal expMonth As Integer, ByVal expYear As Integer, ByVal firstName As String, ByVal lastName As String, ByVal street As String, ByVal city As String, ByVal state As String, ByVal postalCode As String, ByVal countryCode As String, ByRef errMsg As String) As Boolean
        Dim strError As String = ""
        Dim bOk As Boolean = True

        Try
            strCommand = "Companies_Billing_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parBillingContact As New SqlClient.SqlParameter("@BillingContact", SqlDbType.NVarChar, 100)
            parBillingContact.Value = billingContact
            Command.Parameters.Add(parBillingContact)

            Dim parBillingEmail As New SqlClient.SqlParameter("@BillingEmail", SqlDbType.NVarChar, 100)
            parBillingEmail.Value = billingEmail
            Command.Parameters.Add(parBillingEmail)

            Dim parBillingPhone As New SqlClient.SqlParameter("@BillingPhone", SqlDbType.NVarChar, 50)
            parBillingPhone.Value = billingPhone
            Command.Parameters.Add(parBillingPhone)

            Dim parType As New SqlClient.SqlParameter("@Type", SqlDbType.NVarChar, 10)
            parType.Value = type
            Command.Parameters.Add(parType)

            Dim parNumber As New SqlClient.SqlParameter("@Number", SqlDbType.NVarChar, 30)
            parNumber.Value = number
            Command.Parameters.Add(parNumber)

            Dim parSecCode As New SqlClient.SqlParameter("@SecCode", SqlDbType.NVarChar, 4)
            parSecCode.Value = secCode
            Command.Parameters.Add(parSecCode)

            Dim parExpMonth As New SqlClient.SqlParameter("@ExpMonth", SqlDbType.Int)
            parExpMonth.Value = expMonth
            Command.Parameters.Add(parExpMonth)

            Dim parExpYear As New SqlClient.SqlParameter("@ExpYear", SqlDbType.Int)
            parExpYear.Value = expYear
            Command.Parameters.Add(parExpYear)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 50)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 50)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountryCode As New SqlClient.SqlParameter("@CountryCode", SqlDbType.NVarChar, 50)
            parCountryCode.Value = countryCode
            Command.Parameters.Add(parCountryCode)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error saving billing info"
            BLErrorHandling.ErrorCapture(pSysModule, "saveBillingInfo", "", ex.Message & " - Token: " & Token, 0)
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

#End Region

    Public Function getTrail(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As DateTime, ByVal dateTo As DateTime, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Devices_GetTrail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            strError = "Error getting trail"
            BLErrorHandling.ErrorCapture(pSysModule, "getTrail", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return dtData

    End Function

    Public Function getTrail2(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As DateTime, ByVal dateTo As DateTime, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Devices_GetTrail2"
            If dateTo.ToUniversalTime < "6/1/2015" Then
                conString = ConfigurationManager.AppSettings("HETDB")
            ElseIf dateTo.ToUniversalTime < DateAdd(DateInterval.Day, -3, Date.Now.Date) Then
                conString = ConfigurationManager.AppSettings("HETDB1")
            Else
                conString = ConfigurationManager.AppSettings("ConnectionString")
            End If

            'conString = ConfigurationManager.AppSettings("ConnectionString")

            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            strError = "Error getting trail"
            BLErrorHandling.ErrorCapture(pSysModule, "getTrail2", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return dtData

    End Function

    Public Function getHDevices_InfoWindow(ByVal Token As String, ByVal id As String, ByRef errMsg As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "HDevices_InfoWindow_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.BigInt)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("hist")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("histInfoTable")
                jsonLine.WriteValue(reader("InfoTable"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function DeviceInfoWIndow_GET(ByVal Token As String, ByVal deviceID As String, ByRef errMsg As String) As deviceInfoWindow
        Dim itm As New deviceInfoWindow

        Try
            strCommand = "Devices_InfoWindow_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.VarChar, 10)
            parID.Value = deviceID
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("Name")
                itm.eventCode = reader.Item("EventCode")
                itm.eventName = reader.Item("EventName")
                itm.eventDate = reader.Item("EventDate")
                itm.eventDateString = reader.Item("EventDate").ToString
                itm.eventCodeStartedOnString = reader.Item("EventCodeStartedOn")
                itm.eventCodeStartedOnString = reader.Item("EventCodeStartedOn").ToString
                itm.latitude = reader.Item("Latitude")
                itm.longitude = reader.Item("Longitude")
                itm.speed = reader.Item("Speed")
                itm.heading = reader.Item("Heading")
                itm.gpsAge = reader.Item("GPSAge")
                itm.fullAddress = reader.Item("FullAddress")
                itm.driverName = reader.Item("DriverName")
                itm.iconUrl = reader.Item("IconURL")
                itm.isPowerCut = reader.Item("IsPowerCut")
                itm.isBadIgnitionInstall = reader.Item("IsBadIgnitionInstall")
                itm.isNotWorking = reader.Item("IsNotWorking")
                itm.infoTable = reader.Item("InfoTable")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

#Region "Save Web Forms: Contact Us, Quote, Buy Now"

    Public Function saveWebForm(ByVal FormID As String, ByVal qty As Integer, ByVal ServiceID As Integer, ByVal ShippingOption As Integer, ByVal firstName As String, ByVal lastName As String, ByVal email As String, ByVal phone As String, ByVal company As String, ByVal street As String, ByVal city As String, ByVal state As String, ByVal postalCode As String, ByVal ccType As String, ByVal ccNumber As String, ByVal ccSec As String, ByVal ccExpMonth As Integer, ByVal ccExpYear As Integer, ByVal ccFirstName As String, ByVal ccLastName As String, ByVal ccStreet As String, ByVal ccCity As String, ByVal ccState As String, ByVal ccPostalCode As String, ByVal Message As String, ByVal PromoCode As String, ByVal repId As String) As Boolean
        Dim bResults As Boolean = False

        Try
            strCommand = "SaveWebForm"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parFormID As New SqlClient.SqlParameter("@FormID", SqlDbType.NVarChar, 50)
            parFormID.Value = FormID
            Command.Parameters.Add(parFormID)

            Dim parQty As New SqlClient.SqlParameter("@Qty", SqlDbType.Int)
            parQty.Value = qty
            Command.Parameters.Add(parQty)

            Dim parServiceID As New SqlClient.SqlParameter("@ServiceID", SqlDbType.Int)
            parServiceID.Value = ServiceID
            Command.Parameters.Add(parServiceID)

            Dim parShippingOption As New SqlClient.SqlParameter("@ShippingOption", SqlDbType.Int)
            parShippingOption.Value = ShippingOption
            Command.Parameters.Add(parShippingOption)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parCompany As New SqlClient.SqlParameter("@Company", SqlDbType.NVarChar, 50)
            parCompany.Value = company
            Command.Parameters.Add(parCompany)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCCType As New SqlClient.SqlParameter("@CCType", SqlDbType.NVarChar, 5)
            parCCType.Value = ccType
            Command.Parameters.Add(parCCType)

            Dim parCCNumber As New SqlClient.SqlParameter("@CCNumber", SqlDbType.NVarChar, 50)
            parCCNumber.Value = ccNumber
            Command.Parameters.Add(parCCNumber)

            Dim parCCSecCode As New SqlClient.SqlParameter("@CCSecCode", SqlDbType.NVarChar, 10)
            parCCSecCode.Value = ccSec
            Command.Parameters.Add(parCCSecCode)

            Dim parCCExpMonth As New SqlClient.SqlParameter("@CCExpMonth", SqlDbType.Int)
            parCCExpMonth.Value = ccExpMonth
            Command.Parameters.Add(parCCExpMonth)

            Dim parCCExpYear As New SqlClient.SqlParameter("@CCExpYear", SqlDbType.Int)
            parCCExpYear.Value = ccExpYear
            Command.Parameters.Add(parCCExpYear)

            Dim parCCFirstName As New SqlClient.SqlParameter("@CCFirstName", SqlDbType.NVarChar, 20)
            parCCFirstName.Value = ccFirstName
            Command.Parameters.Add(parCCFirstName)

            Dim parCCLastName As New SqlClient.SqlParameter("@CCLastName", SqlDbType.NVarChar, 20)
            parCCLastName.Value = ccLastName
            Command.Parameters.Add(parCCLastName)

            Dim parCCStreet As New SqlClient.SqlParameter("@CCStreet", SqlDbType.NVarChar, 100)
            parCCStreet.Value = ccStreet
            Command.Parameters.Add(parCCStreet)

            Dim parCCCity As New SqlClient.SqlParameter("@CCCity", SqlDbType.NVarChar, 20)
            parCCCity.Value = ccCity
            Command.Parameters.Add(parCCCity)

            Dim parCCState As New SqlClient.SqlParameter("@CCState", SqlDbType.NVarChar, 20)
            parCCState.Value = ccState
            Command.Parameters.Add(parCCState)

            Dim parCCPostalCode As New SqlClient.SqlParameter("@CCPostalCode", SqlDbType.NVarChar, 15)
            parCCPostalCode.Value = ccPostalCode
            Command.Parameters.Add(parCCPostalCode)

            Dim parMessage As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 5000)
            parMessage.Value = Message
            Command.Parameters.Add(parMessage)

            Dim parPromoCode As New SqlClient.SqlParameter("@PromoCode", SqlDbType.NVarChar, 6)
            parPromoCode.Value = PromoCode
            Command.Parameters.Add(parPromoCode)

            Dim parRepID As New SqlClient.SqlParameter("@RepID", SqlDbType.NVarChar, 5)
            parRepID.Value = repId
            Command.Parameters.Add(parRepID)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResults = CBool(parResult.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function getDocQty(ByVal docGUID As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Docs_GetQtyFromGUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim pardocGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            pardocGUID.Value = docGUID
            Command.Parameters.Add(pardocGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            strError = "Error getting Doc Qty from GUID"
            BLErrorHandling.ErrorCapture(pSysModule, "getDocQty", "", ex.Message & " - docGUID: " & docGUID, 0)
        End Try

        Return dtData

    End Function

    Public Function contactUnsubscribe(ByVal contactGUID As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Contacts_Unsubscribe"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Value = contactGUID
            Command.Parameters.Add(parGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            strError = "Error contact unsubscribe from GUID"
            BLErrorHandling.ErrorCapture(pSysModule, "contactUnsubscribe", "", ex.Message & " - contactGUID: " & contactGUID, 0)
        End Try

        Return dtData

    End Function

    Public Function getFamousQuote(ByVal TypeGUID As String, ByRef strError As String) As String
        Dim Quote As String = ""

        Try
            strCommand = "GetFamousQuote"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parType As New SqlClient.SqlParameter("@TypeGUID", SqlDbType.NVarChar, 50)
            parType.Value = TypeGUID
            Command.Parameters.Add(parType)

            Dim parQuote As New SqlClient.SqlParameter("@Quote", SqlDbType.NVarChar, 5000)
            parQuote.Direction = ParameterDirection.Output
            Command.Parameters.Add(parQuote)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            Quote = CStr(parQuote.Value)

        Catch ex As Exception
            Quote = ""
            strError = "Error getting quote"
            BLErrorHandling.ErrorCapture(pSysModule, "getFamousQuote", "", ex.Message & " - TypeGUID: " & TypeGUID, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return Quote

    End Function

#End Region

    Public Function saveGroup(ByVal token As String, ByVal PanelID As Integer, ByVal GUID As String, ByVal name As String, ByVal IsPublic As Boolean, ByVal dtDevices As DataTable, ByRef strError As String) As String
        Dim GroupGUID As String = ""

        Try
            strCommand = "DevicesGroups_MultiMap_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parPanelID As New SqlClient.SqlParameter("@PanelID", SqlDbType.Int)
            parPanelID.Value = PanelID
            Command.Parameters.Add(parPanelID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Value = GUID
            Command.Parameters.Add(parGUID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 20)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parDevices As New SqlClient.SqlParameter("@Devices", SqlDbType.Structured)
            parDevices.Value = dtDevices
            Command.Parameters.Add(parDevices)

            Dim parIsPublic As New SqlClient.SqlParameter("@IsPublic", SqlDbType.Bit)
            parIsPublic.Value = IsPublic
            Command.Parameters.Add(parIsPublic)

            Dim parGroupGUID As New SqlClient.SqlParameter("@GroupGUID", SqlDbType.NVarChar, 50)
            parGroupGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGroupGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GroupGUID = CStr(parGroupGUID.Value)

        Catch ex As Exception
            strError = "Error saving New Group"
            BLErrorHandling.ErrorCapture(pSysModule, "saveGroup", "", ex.Message & " - Token: " & token, 0)
            GroupGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GroupGUID

    End Function

    Public Function getMTGroupsByPanel(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "DevicesGroups_ByPanel_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting groups"
            BLErrorHandling.ErrorCapture(pSysModule, "getMTGroupsByPanel", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function validateDemoToken(ByVal token As String, ByVal strError As String) As DataView
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing

        Try
            strCommand = "DemoUsers_ValidateCredentials"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
            dvData = dtData.DefaultView

        Catch ex As Exception
            strError = "Error validating demo access"
            BLErrorHandling.ErrorCapture(pSysModule, "validateDemoToken", "", ex.Message & " - Token: " & token, 0)
            dvData = Nothing
        End Try

        Return dvData

    End Function

    Public Function runReport(ByVal IsBatch As Boolean, ByVal token As String, ByVal reportId As Integer, ByVal deviceId As String, ByVal dateFrom As DateTime, ByVal dateTo As DateTime, ByVal hourFrom As Integer, ByVal hourTo As Integer, ByVal param As String, ByVal param2 As String, ByRef strError As String, Optional ByVal IsForExport As Boolean = False, Optional ByRef dtData As DataTable = Nothing) As String
        Dim Result As String = ""

        Try
            If IsBatch Then
                strCommand = "ReportsBatch_INSERT"
            Else
                strCommand = "Reports_RUN"
            End If
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure
            Command.CommandTimeout = 60

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parReportID As New SqlClient.SqlParameter("@ReportID", SqlDbType.Int, 4)
            parReportID.Value = reportId
            Command.Parameters.Add(parReportID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            Dim parHourFrom As New SqlClient.SqlParameter("@HourFrom", SqlDbType.Int, 4)
            parHourFrom.Value = hourFrom
            Command.Parameters.Add(parHourFrom)

            Dim parHourTo As New SqlClient.SqlParameter("@HourTo", SqlDbType.Int, 4)
            parHourTo.Value = hourTo
            Command.Parameters.Add(parHourTo)

            Dim parParam As New SqlClient.SqlParameter("@Param", SqlDbType.NVarChar, 50)
            parParam.Value = param
            Command.Parameters.Add(parParam)

            Dim parParam2 As New SqlClient.SqlParameter("@Param2", SqlDbType.NVarChar, 50)
            parParam2.Value = param2
            Command.Parameters.Add(parParam2)

            Dim parIsForExport As New SqlClient.SqlParameter("@IsForExport", SqlDbType.Bit)
            parIsForExport.Value = IsForExport
            Command.Parameters.Add(parIsForExport)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.NVarChar, -1)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If IsForExport = False Then
                If conSQL.State = ConnectionState.Closed Then
                    conSQL.Open()
                End If
                Command.ExecuteNonQuery()

                Result = CStr(parResult.Value)
            Else
                adapter = New SqlDataAdapter(Command)
                adapter.Fill(dtData)
                adapter.Dispose()
                Command.Dispose()
            End If

        Catch ex As Exception
            strError = "Error executing report"
            BLErrorHandling.ErrorCapture(pSysModule, "runReport", "ReportID: " & reportId.ToString, ex.Message & " - Token: " & token, 0)
            Result = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return Result

    End Function

    Public Function runReportRetroGeo(ByVal token As String, ByVal address As String, ByVal lat As Decimal, ByVal lng As Decimal, ByVal radius As Integer, ByVal minStop As Integer, ByVal dateFrom As Date, ByVal dateTo As Date, ByVal isForExport As Boolean, ByRef strError As String, Optional ByRef dtData As DataTable = Nothing) As String
        Dim Result As String = ""

        Try
            strCommand = "Reports_RetroactiveGeofencing"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure
            Command.CommandTimeout = 45

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parAddress As New SqlClient.SqlParameter("@Address", SqlDbType.NVarChar, 100)
            parAddress.Value = address
            Command.Parameters.Add(parAddress)

            Dim parLat As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLat.Value = lat
            Command.Parameters.Add(parLat)

            Dim parLng As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLng.Value = lng
            Command.Parameters.Add(parLng)

            Dim parRadius As New SqlClient.SqlParameter("@RadiusFeet", SqlDbType.Int)
            parRadius.Value = radius
            Command.Parameters.Add(parRadius)

            Dim parMinStop As New SqlClient.SqlParameter("@MinimumSecs", SqlDbType.Int)
            parMinStop.Value = minStop
            Command.Parameters.Add(parMinStop)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            Dim parIsForExport As New SqlClient.SqlParameter("@IsForExport", SqlDbType.Bit)
            parIsForExport.Value = isForExport
            Command.Parameters.Add(parIsForExport)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.NVarChar, -1)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If isForExport = False Then
                If conSQL.State = ConnectionState.Closed Then
                    conSQL.Open()
                End If
                Command.ExecuteNonQuery()

                Result = CStr(parResult.Value)
            Else
                adapter = New SqlDataAdapter(Command)
                adapter.Fill(dtData)
                adapter.Dispose()
                Command.Dispose()
            End If

        Catch ex As Exception
            strError = "Error executing report"
            BLErrorHandling.ErrorCapture(pSysModule, "runReport", "ReportID: 16", ex.Message & " - Token: " & token, 0)
            Result = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return Result

    End Function

    Public Function getEvents(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "DevicesEvents_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting events"
            BLErrorHandling.ErrorCapture(pSysModule, "getEvents", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getInputs(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "DevicesEvents_GetInputs"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting inputs"
            BLErrorHandling.ErrorCapture(pSysModule, "getInputs", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getCompaniesDevicesEvents(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesDevicesEvents_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting companies devices events"
            BLErrorHandling.ErrorCapture(pSysModule, "getCompaniesDevicesEvents", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getDrivers(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Drivers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting drivers"
            BLErrorHandling.ErrorCapture(pSysModule, "getDrivers", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getDeviceIcons(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Devices_IconsGET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting device icons"
            BLErrorHandling.ErrorCapture(pSysModule, "getDeviceIcons", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function resetPowerCut(ByVal token As String, ByVal deviceId As String, ByRef strError As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Devices_ResetPowerCut"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)

        Catch ex As Exception
            strError = "Error reseting power cut status"
            BLErrorHandling.ErrorCapture(pSysModule, "resetPowerCut", "", ex.Message & " - Token: " & token, 0)
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function resetBadInstallMsg(ByVal token As String, ByVal deviceId As String, ByRef strError As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Devices_resetBadInstallBit"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)

        Catch ex As Exception
            strError = "Error reseting Improper Installation status"
            BLErrorHandling.ErrorCapture(pSysModule, "resetBadInstallMsg", "", ex.Message & " - Token: " & token, 0)
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function saveDeviceChanges(ByVal token As String, ByVal id As String, ByVal name As String, ByVal shortName As String, ByVal driverId As Integer, ByVal idleLimit As Integer, ByVal speedLimit As Integer, ByVal iconId As Integer, ByVal line2 As String, ByVal isARB As Boolean, ByVal arbNumber As String, ByVal dieselMeter As Decimal, ByVal electricMeter As Decimal, ByVal odometerReading As Decimal, ByVal vin As String, ByVal fuelCardId As String, ByVal licensePlate As String, ByVal txtColor As String, ByVal bgndColor As String, ByVal isBuzzerOn As Boolean, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Devices_UserUpdates"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 20)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parShortName As New SqlClient.SqlParameter("@ShortName", SqlDbType.NVarChar, 6)
            parShortName.Value = shortName
            Command.Parameters.Add(parShortName)

            Dim parDriverID As New SqlClient.SqlParameter("@DriverID", SqlDbType.Int, 4)
            parDriverID.Value = driverId
            Command.Parameters.Add(parDriverID)

            Dim parIdleLimit As New SqlClient.SqlParameter("@IdleLimit", SqlDbType.Int, 4)
            parIdleLimit.Value = idleLimit
            Command.Parameters.Add(parIdleLimit)

            Dim parSpeedLimit As New SqlClient.SqlParameter("@SpeedLimit", SqlDbType.Int, 4)
            parSpeedLimit.Value = speedLimit
            Command.Parameters.Add(parSpeedLimit)

            Dim parIconID As New SqlClient.SqlParameter("@IconID", SqlDbType.Int, 4)
            parIconID.Value = iconId
            Command.Parameters.Add(parIconID)

            Dim parLine2 As New SqlClient.SqlParameter("@IconLabelLine2", SqlDbType.NVarChar, 6)
            parLine2.Value = line2
            Command.Parameters.Add(parLine2)

            Dim parIsARB As New SqlClient.SqlParameter("@IsARB", SqlDbType.Bit)
            parIsARB.Value = isARB
            Command.Parameters.Add(parIsARB)

            Dim parARBNumber As New SqlClient.SqlParameter("@arbNumber", SqlDbType.NVarChar, 50)
            parARBNumber.Value = arbNumber
            Command.Parameters.Add(parARBNumber)

            'Dim parDieselOnEventCode As New SqlClient.SqlParameter("@DieselOnEventCode", SqlDbType.NVarChar, 3)
            'parDieselOnEventCode.Value = dieselOnEventCode
            'Command.Parameters.Add(parDieselOnEventCode)

            'Dim parElectricOnEventCode As New SqlClient.SqlParameter("@ElectricOnEventCode", SqlDbType.NVarChar, 3)
            'parElectricOnEventCode.Value = electricOnEventCode
            'Command.Parameters.Add(parElectricOnEventCode)

            Dim parDieselMeter As New SqlClient.SqlParameter("@DieselMeter", SqlDbType.Real)
            parDieselMeter.Value = dieselMeter
            Command.Parameters.Add(parDieselMeter)

            Dim parElectricMeter As New SqlClient.SqlParameter("@ElectricMeter", SqlDbType.Real)
            parElectricMeter.Value = electricMeter
            Command.Parameters.Add(parElectricMeter)

            Dim parOdometer As New SqlClient.SqlParameter("@OdometerReading", SqlDbType.Decimal)
            parOdometer.Value = odometerReading
            Command.Parameters.Add(parOdometer)

            Dim parVIN As New SqlClient.SqlParameter("@VIN", SqlDbType.NVarChar, 50)
            parVIN.Value = vin
            Command.Parameters.Add(parVIN)

            Dim parFuelID As New SqlClient.SqlParameter("@FuelCardUnitID", SqlDbType.NVarChar, 20)
            parFuelID.Value = fuelCardId
            Command.Parameters.Add(parFuelID)

            Dim parLicensePlate As New SqlClient.SqlParameter("@LicensePlate", SqlDbType.NVarChar, 50)
            parLicensePlate.Value = licensePlate
            Command.Parameters.Add(parLicensePlate)

            Dim parTxtColor As New SqlClient.SqlParameter("@TextColor", SqlDbType.NVarChar, 7)
            parTxtColor.Value = txtColor
            Command.Parameters.Add(parTxtColor)

            Dim parBgndColor As New SqlClient.SqlParameter("@BgndColor", SqlDbType.NVarChar, 7)
            parBgndColor.Value = bgndColor
            Command.Parameters.Add(parBgndColor)

            Dim parBuzzer As New SqlClient.SqlParameter("@IsBuzzerOn", SqlDbType.Bit)
            parBuzzer.Value = isBuzzerOn
            Command.Parameters.Add(parBuzzer)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error saving device changes"
            BLErrorHandling.ErrorCapture(pSysModule, "saveDeviceChanges", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return dtData

    End Function

    Public Function getSMSGateways(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "SMSGateways_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting SMS Gateways"
            BLErrorHandling.ErrorCapture(pSysModule, "getSMSGateways", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getDealerBrand(ByVal domain As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Dealers_GetBrand_v2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parDomain As New SqlClient.SqlParameter("@Domain", SqlDbType.NVarChar, 100)
            parDomain.Value = domain
            Command.Parameters.Add(parDomain)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting dealer brand"
            BLErrorHandling.ErrorCapture(pSysModule, "getDealerBrand", "", ex.Message & " - domain: " & domain, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return dtData

    End Function

    Public Function getAccessLevels(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "AppAccessLevels_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting Access Levels"
            BLErrorHandling.ErrorCapture(pSysModule, "getAccessLevels", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getAppFeatures_DEPRECATED(ByVal moduleId As Integer, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "AppModulesFeatures_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parModuleID As New SqlClient.SqlParameter("@ModuleID", SqlDbType.Int)
            parModuleID.Direction = ParameterDirection.Input
            parModuleID.Value = moduleId
            Command.Parameters.Add(parModuleID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()


        Catch ex As Exception
            strError = "Error getting App Features"
            BLErrorHandling.ErrorCapture(pSysModule, "getAppFeatures", "", ex.Message & " - moduleId: " & moduleId, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getAppFeatures(ByVal token As String, ByVal moduleId As Integer, ByRef strError As String) As String
        Dim dsData As New DataSet
        Dim strJson As String = ""

        Try
            strCommand = "AppModulesFeatures_GET_v3"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parModuleID As New SqlClient.SqlParameter("@ModuleID", SqlDbType.Int)
            parModuleID.Direction = ParameterDirection.Input
            parModuleID.Value = moduleId
            Command.Parameters.Add(parModuleID)

            adapter = New SqlDataAdapter(Command)
            adapter.TableMappings.Add("Table", "Features")
            adapter.TableMappings.Add("Table1", "Modules")
            adapter.TableMappings.Add("Table2", "UserPreferences")
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

            Dim drv As DataRowView

            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            Dim sbFeature As StringBuilder = Nothing
            Dim swFeature As StringWriter = Nothing
            Dim jsonFeature As Newtonsoft.Json.JsonTextWriter = Nothing

            Dim sbModule As StringBuilder = Nothing
            Dim swModule As StringWriter = Nothing
            Dim jsonModule As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("features")
            json.WriteStartArray()
            For Each drv In dsData.Tables("Features").DefaultView

                sbFeature = New StringBuilder
                swFeature = New StringWriter(sbFeature)
                jsonFeature = New Newtonsoft.Json.JsonTextWriter(swFeature)

                jsonFeature.WriteStartObject()

                jsonFeature.WritePropertyName("id")
                jsonFeature.WriteValue(drv.Item("ID"))

                jsonFeature.WritePropertyName("moduleId")
                jsonFeature.WriteValue(drv.Item("ModuleID"))

                jsonFeature.WritePropertyName("name")
                jsonFeature.WriteValue(drv.Item("Name"))

                jsonFeature.WritePropertyName("minALID")
                jsonFeature.WriteValue(drv.Item("MinAccessLevelID"))

                jsonFeature.WritePropertyName("minAccessLevelId")
                jsonFeature.WriteValue(drv.Item("MinAccessLevelID"))

                jsonFeature.WriteEndObject()
                jsonFeature.Flush()
                json.WriteValue(sbFeature.ToString())

            Next

            json.WriteEnd()
            'json.WriteEndObject()

            'json.WriteStartObject()
            json.WritePropertyName("modules")
            json.WriteStartArray()
            For Each drv In dsData.Tables("Modules").DefaultView

                sbModule = New StringBuilder
                swModule = New StringWriter(sbModule)
                jsonModule = New Newtonsoft.Json.JsonTextWriter(swModule)

                jsonModule.WriteStartObject()

                jsonModule.WritePropertyName("moduleId")
                jsonModule.WriteValue(drv.Item("ModuleID"))

                jsonModule.WriteEndObject()
                jsonModule.Flush()
                json.WriteValue(sbModule.ToString())

            Next

            json.WriteEnd()

            json.WritePropertyName("userPreferences")
            json.WriteStartArray()
            Dim bFoundPreferences As Boolean = False
            For Each drv In dsData.Tables("UserPreferences").DefaultView

                sbModule = New StringBuilder
                swModule = New StringWriter(sbModule)
                jsonModule = New Newtonsoft.Json.JsonTextWriter(swModule)

                jsonModule.WriteStartObject()

                jsonModule.WritePropertyName("autoZoom")
                jsonModule.WriteValue(drv.Item("AutoZoom"))

                jsonModule.WritePropertyName("autoZoomLevel")
                jsonModule.WriteValue(drv.Item("AutoZoomLevel"))

                jsonModule.WritePropertyName("showTraffic")
                jsonModule.WriteValue(drv.Item("ShowTraffic"))

                jsonModule.WritePropertyName("showGeofences")
                jsonModule.WriteValue(drv.Item("ShowGeofences"))

                jsonModule.WritePropertyName("showLatLngGrid")
                Try
                    jsonModule.WriteValue(drv.Item("ShowLatLngGrid"))
                Catch ex As Exception
                    jsonModule.WriteValue(False)
                End Try

                jsonModule.WriteEndObject()
                jsonModule.Flush()
                json.WriteValue(sbModule.ToString())

                bFoundPreferences = True

            Next

            If bFoundPreferences = False Then
                sbModule = New StringBuilder
                swModule = New StringWriter(sbModule)
                jsonModule = New Newtonsoft.Json.JsonTextWriter(swModule)

                jsonModule.WriteStartObject()

                jsonModule.WritePropertyName("autoZoom")
                jsonModule.WriteValue(False)

                jsonModule.WritePropertyName("showTraffic")
                jsonModule.WriteValue(False)

                jsonModule.WritePropertyName("showGeofences")
                jsonModule.WriteValue(False)

                jsonModule.WriteEndObject()
                jsonModule.Flush()
                json.WriteValue(sbModule.ToString())
            End If

            json.WriteEnd()

            json.WriteEndObject()


            json.Flush()
            strJson = sb.ToString


        Catch ex As Exception
            If ex.Message = "LOGOUT" Then
                strError = ex.Message
            Else
                strError = "Error getting App Features v2"
            End If
            BLErrorHandling.ErrorCapture(pSysModule, "getAppFeatures", "", ex.Message & " - Token: " & token, 0)
            dsData = Nothing
            strJson = ""
        End Try

        Return strJson

    End Function

    Public Function getAllAppModules(ByVal token As String, ByRef strError As String) As String
        Dim dtData As New DataTable
        Dim strJson As String = ""

        Try
            strCommand = "AppModules_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

            Dim drv As DataRowView

            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            Dim sbModule As StringBuilder = Nothing
            Dim swModule As StringWriter = Nothing
            Dim jsonModule As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("modules")
            json.WriteStartArray()
            For Each drv In dtData.DefaultView

                sbModule = New StringBuilder
                swModule = New StringWriter(sbModule)
                jsonModule = New Newtonsoft.Json.JsonTextWriter(swModule)

                jsonModule.WriteStartObject()

                jsonModule.WritePropertyName("moduleId")
                jsonModule.WriteValue(drv.Item("ModuleID"))

                jsonModule.WritePropertyName("name")
                jsonModule.WriteValue(drv.Item("Name"))

                jsonModule.WriteEndObject()
                jsonModule.Flush()
                json.WriteValue(sbModule.ToString())

            Next

            json.WriteEnd()
            json.WriteEndObject()


            json.Flush()
            strJson = sb.ToString


        Catch ex As Exception
            strError = "Error getting App Modules"
            BLErrorHandling.ErrorCapture(pSysModule, "getAllAppModules", "", ex.Message & " - Token: " & token, 0)
            strJson = ""
        End Try

        Return strJson

    End Function

    Public Function getStates(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CountryStates_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting States"
            BLErrorHandling.ErrorCapture(pSysModule, "getStates", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Private Function getDateAsString(ByVal theDate As Date) As String
        Dim strDate As String = ""

        Try
            strDate = DatePart(DateInterval.Month, theDate) & "/" & DatePart(DateInterval.Day, theDate) & "/" & DatePart(DateInterval.Year, theDate)
        Catch ex As Exception
            strDate = ""
        End Try

        Return strDate

    End Function

    Public Function getTokenFromUserGUID(ByVal userGUID As String) As String
        Dim strToken As String = ""

        Try
            strCommand = "getTokenFromUserGUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUserGUID As New SqlClient.SqlParameter("@UserGUID", SqlDbType.NVarChar, 50)
            parUserGUID.Direction = ParameterDirection.Input
            parUserGUID.Value = userGUID
            Command.Parameters.Add(parUserGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            strToken = Command.ExecuteScalar()

        Catch ex As Exception
            strToken = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strToken

    End Function

    Function getMultiTrail_DEPRECATED(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As Date, ByVal dateTo As Date, ByRef strError As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Reports_MultiDayTrail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure


            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("trail")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("deviceId")
                jsonLine.WriteValue(reader("DeviceID"))

                jsonLine.WritePropertyName("latitude")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("longitude")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "getMultiTrail", "", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Function getMultiTrail(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As Date, ByVal dateTo As Date, ByRef strError As String) As String
        Dim strJson As String = ""
        Dim dsData As New DataSet

        Try
            strCommand = "Reports_MultiDayTrail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure


            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            adapter = New SqlDataAdapter(Command)

            adapter.TableMappings.Add("Table", "Header")
            adapter.TableMappings.Add("Table1", "Devices")
            adapter.TableMappings.Add("Table2", "DevicesDet")
            adapter.TableMappings.Add("Table3", "Locations")
            adapter.TableMappings.Add("Table4", "Geofences")

            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

            'Ojo: Si se cambia el nombre de esta relacion, tambien hay que cambiarlo en Chattinc.ascx->myRepeater_ItemDataBound y quisas en otras partes.
            dsData.Relations.Add("DevDet", dsData.Tables("Devices").Columns("DeviceID"), dsData.Tables("DevicesDet").Columns("DeviceID"), False)
            dsData.Relations.Add("DevLoc", dsData.Tables("DevicesDet").Columns("ID"), dsData.Tables("Locations").Columns("DevDetID"), False)
            dsData.Relations.Add("DevGeo", dsData.Tables("DevicesDet").Columns("ID"), dsData.Tables("Geofences").Columns("DevDetID"), False)

            Dim dvDet As DataView
            Dim dvLoc As DataView
            Dim dvGeo As DataView
            Dim drvDev As DataRowView
            Dim drvDet As DataRowView
            Dim drvLoc As DataRowView
            Dim drvGeo As DataRowView

            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            Dim sbDevice As StringBuilder = Nothing
            Dim swDevice As StringWriter = Nothing
            Dim jsonDevice As Newtonsoft.Json.JsonTextWriter = Nothing

            Dim sbDevDet As StringBuilder = Nothing
            Dim swDevDet As StringWriter = Nothing
            Dim jsonDevDet As Newtonsoft.Json.JsonTextWriter = Nothing

            Dim sbLoc As StringBuilder = Nothing
            Dim swLoc As StringWriter = Nothing
            Dim jsonLoc As Newtonsoft.Json.JsonTextWriter = Nothing

            Dim sbGeo As StringBuilder = Nothing
            Dim swGeo As StringWriter = Nothing
            Dim jsonGeo As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()

            json.WritePropertyName("header")
            json.WriteValue(dsData.Tables("Header").DefaultView.Item(0).Item("Header"))

            json.WritePropertyName("devices")
            json.WriteStartArray()

            For Each drvDev In dsData.Tables("Devices").DefaultView

                sbDevice = New StringBuilder
                swDevice = New StringWriter(sbDevice)
                jsonDevice = New Newtonsoft.Json.JsonTextWriter(swDevice)

                jsonDevice.WriteStartObject()
                jsonDevice.WritePropertyName("devId")
                jsonDevice.WriteValue(drvDev.Item("DeviceId"))

                jsonDevice.WritePropertyName("devName")
                jsonDevice.WriteValue(drvDev.Item("DeviceName"))

                'Get the details of this device
                dvDet = drvDev.CreateChildView("DevDet")
                jsonDevice.WritePropertyName("devDet")
                jsonDevice.WriteStartArray()

                For Each drvDet In dvDet

                    sbDevDet = New StringBuilder
                    swDevDet = New StringWriter(sbDevDet)
                    jsonDevDet = New Newtonsoft.Json.JsonTextWriter(swDevDet)

                    jsonDevDet.WriteStartObject()
                    jsonDevDet.WritePropertyName("actDate")
                    jsonDevDet.WriteValue(drvDet.Item("ActivityDate"))

                    jsonDevDet.WritePropertyName("ignOn")
                    jsonDevDet.WriteValue(drvDet.Item("FirstIgnitionON"))

                    jsonDevDet.WritePropertyName("ignOnLoc")
                    jsonDevDet.WriteValue(drvDet.Item("AddressON"))

                    jsonDevDet.WritePropertyName("ignOff")
                    jsonDevDet.WriteValue(drvDet.Item("LastIgnitionOFF"))

                    jsonDevDet.WritePropertyName("ignOffLoc")
                    jsonDevDet.WriteValue(drvDet.Item("AddressOFF"))

                    jsonDevDet.WritePropertyName("hours")
                    jsonDevDet.WriteValue(drvDet.Item("hoursWorked"))

                    jsonDevDet.WritePropertyName("miles")
                    jsonDevDet.WriteValue(drvDet.Item("MilesDriven"))

                    'Get the locations of this device
                    dvLoc = drvDet.CreateChildView("DevLoc")
                    jsonDevDet.WritePropertyName("locs")
                    jsonDevDet.WriteStartArray()

                    For Each drvLoc In dvLoc
                        sbLoc = New StringBuilder
                        swLoc = New StringWriter(sbLoc)
                        jsonLoc = New Newtonsoft.Json.JsonTextWriter(swLoc)

                        jsonLoc.WriteStartObject()
                        jsonLoc.WritePropertyName("lat")
                        jsonLoc.WriteValue(drvLoc.Item("Latitude"))

                        jsonLoc.WritePropertyName("lng")
                        jsonLoc.WriteValue(drvLoc.Item("Longitude"))

                        jsonLoc.WriteEndObject()
                        jsonLoc.Flush()
                        jsonDevDet.WriteValue(sbLoc.ToString())
                    Next

                    jsonDevDet.WriteEnd()

                    'Get the geofences of this device
                    dvGeo = drvDet.CreateChildView("DevGeo")
                    jsonDevDet.WritePropertyName("geo")
                    jsonDevDet.WriteStartArray()

                    For Each drvGeo In dvGeo

                        sbGeo = New StringBuilder
                        swGeo = New StringWriter(sbGeo)
                        jsonGeo = New Newtonsoft.Json.JsonTextWriter(swGeo)

                        jsonGeo.WriteStartObject()
                        jsonGeo.WritePropertyName("geoName")
                        jsonGeo.WriteValue(drvGeo.Item("GeofenceName"))

                        jsonGeo.WritePropertyName("arrival")
                        jsonGeo.WriteValue(drvGeo.Item("ArrivalTime"))

                        jsonGeo.WriteEndObject()
                        jsonGeo.Flush()
                        jsonDevDet.WriteValue(sbGeo.ToString())

                    Next

                    jsonDevDet.WriteEnd()
                    jsonDevDet.WriteEndObject()
                    jsonDevDet.Flush()
                    jsonDevice.WriteValue(sbDevDet.ToString())

                Next

                jsonDevice.WriteEnd()
                jsonDevice.WriteEndObject()
                jsonDevice.Flush()
                json.WriteValue(sbDevice.ToString())
            Next

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

        Catch ex As Exception
            strError = "Error loading List"
            dsData = Nothing
            BLErrorHandling.ErrorCapture(pSysModule, "loadList", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    Function exportActivitySummaryReport(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As Date, ByVal dateTo As Date, ByVal isExport As Boolean, ByRef strError As String) As DataTable
        Dim strJson As String = ""
        Dim dtData As New DataTable

        Try
            strCommand = "Reports_MultiDayTrail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure


            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            Dim parIsExport As New SqlClient.SqlParameter("@IsExport", SqlDbType.Bit)
            parIsExport.Value = isExport
            Command.Parameters.Add(parIsExport)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error loading exportActivitySummaryReport"
            dtData = Nothing
            BLErrorHandling.ErrorCapture(pSysModule, "exportActivitySummaryReport", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return dtData

    End Function

    Public Function getDiagInfo(ByVal Token As String, ByVal deviceID As String, ByVal hId As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Devices_GetDiagInfo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceID
            Command.Parameters.Add(parDeviceID)

            Dim parHID As New SqlClient.SqlParameter("@HID", SqlDbType.NVarChar, 20)
            parHID.Direction = ParameterDirection.Input
            parHID.Value = hId
            Command.Parameters.Add(parHID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("diag")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("consecutive")
                jsonLine.WriteValue(reader("Consecutive"))

                jsonLine.WritePropertyName("gpsAge")
                jsonLine.WriteValue(reader("GPSAge"))

                jsonLine.WritePropertyName("originalEvent")
                jsonLine.WriteValue(reader("OriginalEvent"))

                jsonLine.WritePropertyName("extraData")
                jsonLine.WriteValue(reader("ExtraData"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            strJson = "false"
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function GetHotSpots(ByVal Token As String, ByVal deviceId As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "HotSpots_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function GetBasicList(ByVal Token As String, ByVal entityName As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "getBasicList"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parEntityName As New SqlClient.SqlParameter("@EntityName", SqlDbType.NVarChar, 50)
            parEntityName.Direction = ParameterDirection.Input
            parEntityName.Value = entityName
            Command.Parameters.Add(parEntityName)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function GetIdNameList(ByVal token As String, ByVal entityName As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As New basicList
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "getIdNameList"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEntityName As New SqlClient.SqlParameter("@EntityName", SqlDbType.NVarChar, 50)
            parEntityName.Direction = ParameterDirection.Input
            parEntityName.Value = entityName
            Command.Parameters.Add(parEntityName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("ID")
                itm.name = reader.Item("Name")

                Try
                    If Not IsDBNull(reader.Item("Value1")) Then
                        itm.value1 = reader.Item("Value1")
                    End If
                Catch ex As Exception
                    itm.value1 = ""
                End Try

                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function getValue(ByVal token As String, ByVal valueName As String) As singleValue
        Dim itm As singleValue = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "getValue"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parValueName As New SqlClient.SqlParameter("@ValueName", SqlDbType.NVarChar, 50)
            parValueName.Value = valueName
            Command.Parameters.Add(parValueName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New singleValue
                itm.name = valueName
                itm.value = reader.Item("value")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

#Region "Customers"

    Function getCompanies(ByVal token As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_Customers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("UniqueKey")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Function saveNewCustomer(token As String, data As CRM_Customer) As String
        Dim customerId As String = ""
        Dim isOk As Boolean = False

        Try
            strCommand = "CRM_Customers_New"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = data.firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = data.lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parCompany As New SqlClient.SqlParameter("@Company", SqlDbType.NVarChar, 50)
            parCompany.Value = data.companyName
            Command.Parameters.Add(parCompany)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = data.street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Value = data.state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parBillingContact As New SqlClient.SqlParameter("@BillingContact", SqlDbType.NVarChar, 100)
            parBillingContact.Value = data.billingContact
            Command.Parameters.Add(parBillingContact)

            Dim parBillingEmail As New SqlClient.SqlParameter("@BillingEmail", SqlDbType.NVarChar, 100)
            parBillingEmail.Value = data.billingEmail
            Command.Parameters.Add(parBillingEmail)

            Dim parBillingPhone As New SqlClient.SqlParameter("@BillingPhone", SqlDbType.NVarChar, 50)
            parBillingPhone.Value = data.billingPhone
            Command.Parameters.Add(parBillingPhone)

            Dim parCCStreet As New SqlClient.SqlParameter("@CCStreet", SqlDbType.NVarChar, 100)
            parCCStreet.Value = data.billingStreet
            Command.Parameters.Add(parCCStreet)

            Dim parCCCity As New SqlClient.SqlParameter("@CCCity", SqlDbType.NVarChar, 20)
            parCCCity.Value = data.billingCity
            Command.Parameters.Add(parCCCity)

            Dim parCCState As New SqlClient.SqlParameter("@CCState", SqlDbType.NVarChar, 20)
            parCCState.Value = data.billingState
            Command.Parameters.Add(parCCState)

            Dim parCCPostalCode As New SqlClient.SqlParameter("@CCPostalCode", SqlDbType.NVarChar, 15)
            parCCPostalCode.Value = data.billingPostalCode
            Command.Parameters.Add(parCCPostalCode)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parNewId As New SqlClient.SqlParameter("@NewID", SqlDbType.VarChar, 50)
            parNewId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            customerId = CStr(parNewId.Value)
            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            customerId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return customerId

    End Function

    Function crm_getAllCompanies(ByVal token As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_Customers_GetAll"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("UniqueKey")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Function saveCompanyNote(ByVal token As String, ByVal companyId As String, ByVal note As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "CRM_Customers_SaveNote"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustomerId As New SqlClient.SqlParameter("@CompanyID", SqlDbType.NVarChar, 50)
            parCustomerId.Value = companyId
            Command.Parameters.Add(parCustomerId)

            Dim parNote As New SqlClient.SqlParameter("@Note", SqlDbType.NVarChar, -1)
            parNote.Value = note
            Command.Parameters.Add(parNote)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

        Catch ex As Exception
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return bResult

    End Function

    Function getCompanyNotes(ByVal token As String, ByVal customerID As String) As List(Of companyNote)
        Dim lst As New List(Of companyNote)
        Dim itm As companyNote

        Try
            strCommand = "CRM_Customers_GetNotes"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustomerId As New SqlClient.SqlParameter("@CompanyID", SqlDbType.NVarChar, 50)
            parCustomerId.Value = customerID
            Command.Parameters.Add(parCustomerId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New companyNote
                itm.createdBy = reader.Item("CreatedBy")
                itm.createdOn = reader.Item("CreatedOn").ToString
                itm.note = reader.Item("Note")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

#End Region

#Region "Devices"

    Public Function searchDevice(ByVal token As String, ByVal searchKey As String, ByVal keyValue As String) As searchDeviceResult
        Dim itm As New searchDeviceResult

        Try
            strCommand = "CRM_Devices_SearchDevice"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSearchKey As New SqlClient.SqlParameter("@SearchKey", SqlDbType.NVarChar, 10)
            parSearchKey.Value = searchKey
            Command.Parameters.Add(parSearchKey)

            Dim parKeyValue As New SqlClient.SqlParameter("@KeyValue", SqlDbType.NVarChar, 10)
            parKeyValue.Value = keyValue
            Command.Parameters.Add(parKeyValue)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New searchDeviceResult
                itm.customerGUID = reader.Item("customerGUID")
                itm.customerName = reader.Item("customerName")
                itm.deviceGUID = reader.Item("DeviceGUID")
                itm.deviceId = reader.Item("deviceID")
                itm.deviceName = reader.Item("deviceName")
                itm.serialNumber = reader.Item("serialNumber")
                itm.imei = reader.Item("imei")
                itm.simCarrier = reader.Item("simCarrier")
                itm.simNo = reader.Item("simNo")
                itm.simAreaCode = reader.Item("simAreaCode")
                itm.simPhoneNumber = reader.Item("simPhoneNumber")
                itm.lastUpdatedOn = reader.Item("lastUpdatedOn").ToString
                itm.eventDate = reader.Item("eventDate").ToString
                itm.eventCode = reader.Item("eventCode")
                itm.gpsAge = reader.Item("gpsAge")
                itm.lastGoodGpsOn = reader.Item("lastGoodGpsOn").ToString
                itm.hasSpeedGauge = reader.Item("hasSpeedGauge")
                itm.sgStatus = reader.Item("sgStatus")
                itm.sgFee = reader.Item("sgFee")
                itm.isNotWorking = reader.Item("IsNotWorking")
                itm.notWorkingSince = reader.Item("NotWorkingSince").ToString
                itm.isInactive = reader.Item("isInactive")
                itm.inactiveReason = reader.Item("inactiveReason")
                itm.isRMA = reader.Item("isRMA")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.dataLayer.svc", "", "searchDevice", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function deviceAction(ByVal token As String, ByVal action As String, ByVal id As String, ByVal param1 As String, ByVal param2 As String) As callResult
        Dim res As New callResult

        Try
            strCommand = "CRM_Devices_SetThings"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parAction As New SqlClient.SqlParameter("@Action", SqlDbType.NVarChar, 50)
            parAction.Value = action
            Command.Parameters.Add(parAction)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parParam1 As New SqlClient.SqlParameter("@Param1", SqlDbType.NVarChar, 50)
            parParam1.Value = param1
            Command.Parameters.Add(parParam1)

            Dim parParam2 As New SqlClient.SqlParameter("@Param2", SqlDbType.NVarChar, 50)
            parParam2.Value = param2
            Command.Parameters.Add(parParam2)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)
            res.msg = CStr(parMsg.Value)

        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.dataLayer.svc", "", "deviceAction", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Inventory"

    Public Function getInventory(ByVal token As String) As List(Of inventory)
        Dim lst As New List(Of inventory)
        Dim itm As inventory = Nothing

        Try
            strCommand = "CRM_DealersInventory_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New inventory
                itm.id = reader.Item("id")
                itm.deviceId = reader.Item("deviceId")
                itm.deviceTypeName = reader.Item("deviceTypeName")
                itm.serialNumber = reader.Item("serialNumber")
                itm.imei = reader.Item("imei")
                itm.simNo = reader.Item("simNo").ToString
                itm.simAreaCode = reader.Item("simAreaCode").ToString
                itm.simPhoneNumber = reader.Item("simPhoneNumber").ToString
                itm.createdOn = reader.Item("CreatedOn").ToString
                itm.lastUpdatedOn = reader.Item("LastUpdatedOn").ToString
                itm.eventDate = reader.Item("EventDate").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return lst

    End Function

    Public Function saveNewInventory(ByVal token As String, ByVal data As newInventory, ByRef msg As String) As Boolean
        Dim isOk As Boolean = True

        Try
            msg = ""

            strCommand = "CRM_Devices_AddInventory"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceTypeID As New SqlClient.SqlParameter("@DeviceTypeID", SqlDbType.Int)
            parDeviceTypeID.Value = data.deviceTypeId
            Command.Parameters.Add(parDeviceTypeID)

            Dim parCarrier As New SqlClient.SqlParameter("@Carrier", SqlDbType.NVarChar, 50)
            parCarrier.Value = data.carrier
            Command.Parameters.Add(parCarrier)

            Dim parSerialNumber As New SqlClient.SqlParameter("@SerialNumber", SqlDbType.NVarChar, 50)
            parSerialNumber.Value = data.serialNumber
            Command.Parameters.Add(parSerialNumber)

            Dim parIMEI As New SqlClient.SqlParameter("@IMEI", SqlDbType.NVarChar, 50)
            parIMEI.Value = data.imei
            Command.Parameters.Add(parIMEI)

            Dim parSimNo As New SqlClient.SqlParameter("@SimNo", SqlDbType.NVarChar, 50)
            parSimNo.Value = data.simNo
            Command.Parameters.Add(parSimNo)

            Dim parSimPhone As New SqlClient.SqlParameter("@SimPhone", SqlDbType.NVarChar, 50)
            parSimPhone.Value = data.simPhone
            Command.Parameters.Add(parSimPhone)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return isOk

    End Function

    Public Function confirmShipment(ByVal Token As String, ByVal orderNo As String, ByVal courrierId As String, ByVal trackingNumber As String) As responseOk
        Dim r As New responseOk

        Try
            strCommand = "CRM_Orders_ConfirmShipment"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parOrderNo As New SqlClient.SqlParameter("@OrderNo", SqlDbType.NVarChar, 50)
            parOrderNo.Value = orderNo
            Command.Parameters.Add(parOrderNo)

            Dim parCourrierId As New SqlClient.SqlParameter("@CourrierId", SqlDbType.NVarChar, 50)
            parCourrierId.Value = courrierId
            Command.Parameters.Add(parCourrierId)

            Dim parTrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 50)
            parTrackingNumber.Value = trackingNumber
            Command.Parameters.Add(parTrackingNumber)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            r.isOk = CBool(parIsOk.Value)
            r.msg = CStr(parMsg.Value)

        Catch ex As Exception
            r.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return r

    End Function

    Public Function assignDevice(ByVal Token As String, ByVal custId As String, ByVal orderNo As String, ByVal courrierId As String, ByVal trackingNumber As String, ByVal deviceId As String) As Boolean
        Dim isOk As Boolean = True

        Try
            strCommand = "CRM_Devices_Assign"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            Dim parOrderNo As New SqlClient.SqlParameter("@OrderNo", SqlDbType.NVarChar, 50)
            parOrderNo.Value = orderNo
            Command.Parameters.Add(parOrderNo)

            Dim parCourrierId As New SqlClient.SqlParameter("@CourrierId", SqlDbType.NVarChar, 50)
            parCourrierId.Value = courrierId
            Command.Parameters.Add(parCourrierId)

            Dim parTrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 50)
            parTrackingNumber.Value = trackingNumber
            Command.Parameters.Add(parTrackingNumber)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 20)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return isOk

    End Function

#End Region

#Region "Suspend/Resume Company"

    Function GetCompaniesSuspendedReasons() As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "CompaniesSuspendedReasons_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Function getCompaniesSuspendResume(ByVal token As String) As List(Of Company)
        Dim lst As New List(Of Company)
        Dim itm As Company
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "CRM_Customers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New Company
                itm.id = reader.Item("UniqueKey")
                itm.name = reader.Item("Name")
                itm.isSuspended = reader.Item("bIsSuspended")
                itm.suspendedId = reader.Item("SuspendedID")
                itm.suspendedOn = reader.Item("SuspendedOn").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function saveSuspendCompany(ByVal Token As String, ByVal data As Company) As Boolean
        Dim isOk As Boolean = True

        Try
            strCommand = "CRM_Companies_SuspendResume"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = data.id
            Command.Parameters.Add(parCustID)

            Dim parIsSuspended As New SqlClient.SqlParameter("@IsSuspended", SqlDbType.Bit)
            parIsSuspended.Value = data.isSuspended
            Command.Parameters.Add(parIsSuspended)

            Dim parReasonId As New SqlClient.SqlParameter("@SuspendedID", SqlDbType.Int)
            parReasonId.Value = CInt(data.suspendedId)
            Command.Parameters.Add(parReasonId)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return isOk

    End Function

#End Region

#Region "Work Zones"

    Public Function GetWorkZones(ByVal token As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "WorkZones_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Technicians"

    Public Function GetTechnicians(ByVal token As String, ByVal WorkZoneID As String) As List(Of technician)
        Dim lst As New List(Of technician)
        Dim itm As technician = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Technicians_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parWZ As New SqlClient.SqlParameter("@WorkZoneID", SqlDbType.NVarChar, 50)
            parWZ.Value = WorkZoneID
            Command.Parameters.Add(parWZ)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New technician
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.wzId = reader.Item("WorkZoneID")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Jobs Status"

    Public Function GetJobStatus(ByVal token As String) As List(Of jobStatus)
        Dim lst As New List(Of jobStatus)
        Dim itm As jobStatus = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "JobStatuses_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)


            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New jobStatus
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.qty = reader.Item("Quantity")
                itm.overdue = reader.Item("Overdue")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Jobs"

    Public Function GetJobs(ByVal token As String, ByVal statusId As String, ByVal workZoneId As String, ByVal assignedToId As String, ByVal jobNo As String, ByVal custName As String) As List(Of job)
        Dim lst As New List(Of job)
        Dim itm As job = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Value = statusId
            Command.Parameters.Add(parStatusID)

            Dim parWZID As New SqlClient.SqlParameter("@WorkZoneID", SqlDbType.NVarChar, 50)
            parWZID.Value = workZoneId
            Command.Parameters.Add(parWZID)

            Dim parTechID As New SqlClient.SqlParameter("@AssignedToID", SqlDbType.NVarChar, 50)
            parTechID.Value = assignedToId
            Command.Parameters.Add(parTechID)

            Dim parJobNo As New SqlClient.SqlParameter("@JobNo", SqlDbType.NVarChar, 50)
            parJobNo.Value = jobNo
            Command.Parameters.Add(parJobNo)

            Dim parCustName As New SqlClient.SqlParameter("@CustName", SqlDbType.NVarChar, 50)
            parCustName.Value = custName
            Command.Parameters.Add(parCustName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New job
                itm.id = reader.Item("ID")
                itm.workZoneId = reader.Item("WorkZoneID")
                itm.workZoneName = reader.Item("WorkZoneName")
                itm.jobNumber = reader.Item("JobNumber")
                itm.customerName = reader.Item("CustomerName")
                itm.jobDescription = reader.Item("JobDescription")
                itm.assignedToName = reader.Item("AssignedToName")
                itm.jobStatus = reader.Item("JobStatus")
                itm.priority = reader.Item("Priority")
                itm.createdOn = reader.Item("CreatedOn")
                itm.dueOn = reader.Item("DueOn")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function GetJob(ByVal token As String, ByVal jobId As String) As job
        Dim itm As job = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New job
                itm.id = reader.Item("ID")
                itm.jobNumber = reader.Item("JobNumber")
                itm.workZoneId = reader.Item("WorkZoneID")
                itm.customerId = reader.Item("CustomerID")
                itm.customerName = reader.Item("CustomerName")
                itm.contactName = reader.Item("ContactName")
                itm.phone = reader.Item("Phone")
                itm.address = reader.Item("Address")
                itm.jobDescription = reader.Item("JobDescription")
                itm.categoryId = reader.Item("CategoryID")
                itm.specialtyId = reader.Item("SpecialtyId")
                itm.assignedToId = reader.Item("AssignedToID")
                itm.assignedToName = reader.Item("AssignedToName")
                itm.statusId = reader.Item("StatusID")
                itm.priorityId = reader.Item("PriorityID")
                itm.createdOn = reader.Item("CreatedOn")
                itm.scheduledStart = reader.Item("ScheduledStart")
                itm.durationHH = reader.Item("DurationHH")
                itm.durationMM = reader.Item("DurationMM")
                itm.dueOn = reader.Item("DueOn")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function GetJobSupportTables(ByVal token As String) As jobSupportTables
        Dim st As New jobSupportTables
        Dim itm As basicList = Nothing
        Dim tec As technician = Nothing
        Dim sta As jobStatus = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_GetSupportTables"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            'WORK ZONES
            st.workZones = New List(Of basicList)
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                st.workZones.Add(itm)
            Loop

            'PRIORITIES
            reader.NextResult()
            st.priorities = New List(Of basicList)
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                st.priorities.Add(itm)
            Loop

            'SPECIALTIES
            reader.NextResult()
            st.specialties = New List(Of basicList)
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                st.specialties.Add(itm)
            Loop

            'TECHNICIANS
            reader.NextResult()
            st.technicians = New List(Of technician)
            Do While reader.Read
                tec = New technician
                tec.id = reader.Item("ID")
                tec.name = reader.Item("Name")
                tec.wzId = reader.Item("WorkZoneID")
                st.technicians.Add(tec)
            Loop

            'STATUSES
            reader.NextResult()
            st.statuses = New List(Of jobStatus)
            Do While reader.Read
                sta = New jobStatus
                sta.id = reader.Item("ID")
                sta.name = reader.Item("Name")
                sta.qty = reader.Item("Quantity")
                sta.overdue = reader.Item("Overdue")
                st.statuses.Add(sta)
            Loop

            'CATEGORIES
            reader.NextResult()
            st.categories = New List(Of basicList)
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                st.categories.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return st

    End Function

    Public Function saveJob(ByVal token As String, ByVal job As job, ByRef jobGUID As String, ByRef JobNumber As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Jobs_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = job.id
            Command.Parameters.Add(parID)

            Dim parWZID As New SqlClient.SqlParameter("@WorkZoneID", SqlDbType.NVarChar, 50)
            parWZID.Value = job.workZoneId
            Command.Parameters.Add(parWZID)

            Dim parCustID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustID.Value = job.customerId
            Command.Parameters.Add(parCustID)

            Dim parContactName As New SqlClient.SqlParameter("@ContactName", SqlDbType.NVarChar, 50)
            parContactName.Value = job.contactName
            Command.Parameters.Add(parContactName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = job.phone
            Command.Parameters.Add(parPhone)

            Dim parAddress As New SqlClient.SqlParameter("@Address", SqlDbType.NVarChar, 100)
            parAddress.Value = job.address
            Command.Parameters.Add(parAddress)

            Dim parJobDescription As New SqlClient.SqlParameter("@JobDescription", SqlDbType.NVarChar, -1)
            parJobDescription.Value = job.jobDescription
            Command.Parameters.Add(parJobDescription)

            Dim parCatID As New SqlClient.SqlParameter("@CategoryId", SqlDbType.NVarChar, 50)
            parCatID.Value = job.categoryId
            Command.Parameters.Add(parCatID)

            Dim parSpecID As New SqlClient.SqlParameter("@SpecialtyId", SqlDbType.NVarChar, 50)
            parSpecID.Value = job.specialtyId
            Command.Parameters.Add(parSpecID)

            Dim parAssignedToID As New SqlClient.SqlParameter("@AssignedToId", SqlDbType.NVarChar, 50)
            parAssignedToID.Value = job.assignedToId
            Command.Parameters.Add(parAssignedToID)

            Dim parStatID As New SqlClient.SqlParameter("@StatusId", SqlDbType.NVarChar, 50)
            parStatID.Value = job.statusId
            Command.Parameters.Add(parStatID)

            Dim parPrioID As New SqlClient.SqlParameter("@PriorityId", SqlDbType.NVarChar, 50)
            parPrioID.Value = job.priorityId
            Command.Parameters.Add(parPrioID)

            Dim parScheduledStart As New SqlClient.SqlParameter("@ScheduledStart", SqlDbType.DateTime)
            parScheduledStart.Value = job.scheduledStart
            Command.Parameters.Add(parScheduledStart)

            Dim parHH As New SqlClient.SqlParameter("@DurationHH", SqlDbType.Int)
            parHH.Value = job.durationHH
            Command.Parameters.Add(parHH)

            Dim parMM As New SqlClient.SqlParameter("@DurationMM", SqlDbType.Int)
            parMM.Value = job.durationMM
            Command.Parameters.Add(parMM)

            Dim parDueOn As New SqlClient.SqlParameter("@DueOn", SqlDbType.DateTime)
            parDueOn.Value = job.dueOn
            Command.Parameters.Add(parDueOn)

            Dim parJobGUID As New SqlClient.SqlParameter("@JobGUID", SqlDbType.NVarChar, 50)
            parJobGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parJobGUID)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 50)
            parJobNumber.Direction = ParameterDirection.Output
            Command.Parameters.Add(parJobNumber)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            jobGUID = CStr(parJobGUID.Value)
            JobNumber = CStr(parJobNumber.Value)

        Catch ex As Exception
            strError = "Error saving Job"
            BLErrorHandling.ErrorCapture(pSysModule, "saveJob", "", ex.Message & " - Token: " & token, 0)
            jobGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "Customers / Geofences"

    Public Function CustomerSearch(ByVal token As String, ByVal custName As String) As List(Of customerSearch)
        Dim lst As New List(Of customerSearch)
        Dim itm As customerSearch = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Customers_SEARCH"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 100)
            parName.Value = custName
            Command.Parameters.Add(parName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New customerSearch
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.workZoneId = reader.Item("WorkZoneID")
                itm.workZoneName = reader.Item("WorkZoneName")
                itm.contactName = reader.Item("ContactName")
                itm.address = reader.Item("Address")
                itm.phone = reader.Item("Phone")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Telemetry"

    Public Function GetIOs(ByVal token As String, ByVal deviceId As String) As devIOs
        Dim itm As New devIOs
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devIOs
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("DeviceName")
                itm.name1 = reader.Item("Rel1Name")
                itm.on1Name = reader.Item("Relay1OnName")
                itm.off1Name = reader.Item("Relay1OffName")
                itm.status1 = reader.Item("Rel1Status")
                itm.name2 = reader.Item("Rel2Name")
                itm.on2Name = reader.Item("Relay2OnName")
                itm.off2Name = reader.Item("Relay2OffName")
                itm.status2 = reader.Item("Rel2Status")
                itm.name3 = reader.Item("Rel3Name")
                itm.on3Name = reader.Item("Relay3OnName")
                itm.off3Name = reader.Item("Relay3OffName")
                itm.status3 = reader.Item("Rel3Status")
                itm.name4 = reader.Item("Rel4Name")
                itm.on4Name = reader.Item("Relay4OnName")
                itm.off4Name = reader.Item("Relay4OffName")
                itm.status4 = reader.Item("Rel4Status")

                itm.inputName1 = reader.Item("Input1Name")
                itm.inputOn1Name = reader.Item("Input1OnName")
                itm.inputOff1Name = reader.Item("Input1OffName")
                itm.inputStatus1 = reader.Item("Input1Status")
                itm.inputName2 = reader.Item("Input2Name")
                itm.inputOn2Name = reader.Item("Input2OnName")
                itm.inputOff2Name = reader.Item("Input2OffName")
                itm.inputStatus2 = reader.Item("Input2Status")
                itm.inputName3 = reader.Item("Input3Name")
                itm.inputOn3Name = reader.Item("Input3OnName")
                itm.inputOff3Name = reader.Item("Input3OffName")
                itm.inputStatus3 = reader.Item("Input3Status")
                itm.inputName4 = reader.Item("Input4Name")
                itm.inputOn4Name = reader.Item("Input4OnName")
                itm.inputOff4Name = reader.Item("Input4OffName")
                itm.inputStatus4 = reader.Item("Input4Status")

            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function GetAllDevIOs(ByVal token As String) As List(Of devIOs)
        Dim lst As New List(Of devIOs)
        Dim itm As New devIOs
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = "0"
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devIOs
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("DeviceName")
                itm.name1 = reader.Item("Rel1Name")
                itm.on1Name = reader.Item("Relay1OnName")
                itm.off1Name = reader.Item("Relay1OffName")
                itm.status1 = reader.Item("Rel1Status")
                itm.name2 = reader.Item("Rel2Name")
                itm.on2Name = reader.Item("Relay2OnName")
                itm.off2Name = reader.Item("Relay2OffName")
                itm.status2 = reader.Item("Rel2Status")
                itm.name3 = reader.Item("Rel3Name")
                itm.on3Name = reader.Item("Relay3OnName")
                itm.off3Name = reader.Item("Relay3OffName")
                itm.status3 = reader.Item("Rel3Status")
                itm.name4 = reader.Item("Rel4Name")
                itm.on4Name = reader.Item("Relay4OnName")
                itm.off4Name = reader.Item("Relay4OffName")
                itm.status4 = reader.Item("Rel4Status")

                itm.inputName1 = reader.Item("Input1Name")
                itm.inputOn1Name = reader.Item("Input1OnName")
                itm.inputOff1Name = reader.Item("Input1OffName")
                itm.inputStatus1 = reader.Item("Input1Status")
                itm.inputName2 = reader.Item("Input2Name")
                itm.inputOn2Name = reader.Item("Input2OnName")
                itm.inputOff2Name = reader.Item("Input2OffName")
                itm.inputStatus2 = reader.Item("Input2Status")
                itm.inputName3 = reader.Item("Input3Name")
                itm.inputOn3Name = reader.Item("Input3OnName")
                itm.inputOff3Name = reader.Item("Input3OffName")
                itm.inputStatus3 = reader.Item("Input3Status")
                itm.inputName4 = reader.Item("Input4Name")
                itm.inputOn4Name = reader.Item("Input4OnName")
                itm.inputOff4Name = reader.Item("Input4OffName")
                itm.inputStatus4 = reader.Item("Input4Status")

                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function setOutput(ByVal token As String, ByVal data As setRelay, ByRef msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "Devices_SetDeviceOutput"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parRelNum As New SqlClient.SqlParameter("@RelayNum", SqlDbType.Int, 4)
            parRelNum.Value = data.relayNum
            Command.Parameters.Add(parRelNum)

            Dim parStat As New SqlClient.SqlParameter("@NewStatus", SqlDbType.Bit)
            parStat.Value = data.newStatus
            Command.Parameters.Add(parStat)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error setting output"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.setOutput", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

    Public Function telemetrySetUp(token As String, data As ioSetUp, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            If data.ioType = 1 Then
                strCommand = "Devices_IOs_InputsSetUp"
            Else
                strCommand = "Devices_IOs_OutputsSetUp"
            End If
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIONum As SqlClient.SqlParameter = Nothing
            If data.ioType = 1 Then
                parIONum = New SqlClient.SqlParameter("@InputNum", SqlDbType.Int, 4)
            Else
                parIONum = New SqlClient.SqlParameter("@RelayNum", SqlDbType.Int, 4)
            End If
            parIONum.Value = data.ioNum
            Command.Parameters.Add(parIONum)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parOn As New SqlClient.SqlParameter("@OnStatus", SqlDbType.NVarChar, 50)
            parOn.Value = data.onStatus
            Command.Parameters.Add(parOn)

            Dim parOff As New SqlClient.SqlParameter("@OffStatus", SqlDbType.NVarChar, 50)
            parOff.Value = data.offStatus
            Command.Parameters.Add(parOff)

            Dim parAll As New SqlClient.SqlParameter("@IsAll", SqlDbType.Bit)
            parAll.Value = data.isAll
            Command.Parameters.Add(parAll)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in IO set up"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.telemetrySetUp", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

#End Region

#Region "Hour Meters"

    Public Function GetDevMeters(ByVal token As String, ByVal deviceId As String) As devInputsOnTime
        Dim itm As New devInputsOnTime
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_OnTime_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devInputsOnTime
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("Name")
                itm.ignitionOnTime = reader.Item("IgnitionOnTime")
                itm.ignitionLastSetOn = reader.Item("IgnitionLastSetOn").ToString
                itm.input1Name = reader.Item("Input1Name").ToString
                itm.input1OnTime = reader.Item("Input1OnTime")
                itm.input1LastSetOn = reader.Item("Input1LastSetOn").ToString
                itm.input2Name = reader.Item("Input2Name").ToString
                itm.input2OnTime = reader.Item("Input2OnTime")
                itm.input2LastSetOn = reader.Item("Input2LastSetOn").ToString
                itm.input3Name = reader.Item("Input3Name").ToString
                itm.input3OnTime = reader.Item("Input3OnTime")
                itm.input3LastSetOn = reader.Item("Input3LastSetOn").ToString
                itm.input4Name = reader.Item("Input4Name").ToString
                itm.input4OnTime = reader.Item("Input4OnTime")
                itm.input4LastSetOn = reader.Item("Input4LastSetOn").ToString
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function GetAllDevMeters(ByVal token As String) As List(Of devInputsOnTime)
        Dim lst As New List(Of devInputsOnTime)
        Dim itm As New devInputsOnTime
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_OnTime_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = "0"
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devInputsOnTime
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("Name")
                itm.ignitionOnTime = reader.Item("IgnitionOnTime")
                itm.ignitionLastSetOn = reader.Item("IgnitionLastSetOn").ToString
                itm.input1Name = reader.Item("Input1Name").ToString
                itm.input1OnTime = reader.Item("Input1OnTime")
                itm.input1LastSetOn = reader.Item("Input1LastSetOn").ToString
                itm.input2Name = reader.Item("Input2Name").ToString
                itm.input2OnTime = reader.Item("Input2OnTime")
                itm.input2LastSetOn = reader.Item("Input2LastSetOn").ToString
                itm.input3Name = reader.Item("Input3Name").ToString
                itm.input3OnTime = reader.Item("Input3OnTime")
                itm.input3LastSetOn = reader.Item("Input3LastSetOn").ToString
                itm.input4Name = reader.Item("Input4Name").ToString
                itm.input4OnTime = reader.Item("Input4OnTime")
                itm.input4LastSetOn = reader.Item("Input4LastSetOn").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function saveHourMeter(token As String, data As devInputsOnTimeTransformed, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "Devices_IOs_OnTime_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIgnitionOnTime As New SqlClient.SqlParameter("@IgnitionOnTime", SqlDbType.Int)
            parIgnitionOnTime.Value = data.ignitionOnTime
            Command.Parameters.Add(parIgnitionOnTime)

            Dim parIgnitionSetOn As New SqlClient.SqlParameter("@IgnitionSetOn", SqlDbType.DateTime)
            parIgnitionSetOn.Value = data.ignitionSetOn
            Command.Parameters.Add(parIgnitionSetOn)

            Dim parInput1OnTime As New SqlClient.SqlParameter("@Input1OnTime", SqlDbType.Int)
            parInput1OnTime.Value = data.input1OnTime
            Command.Parameters.Add(parInput1OnTime)

            Dim parInput1SetOn As New SqlClient.SqlParameter("@Input1SetOn", SqlDbType.DateTime)
            parInput1SetOn.Value = data.input1SetOn
            Command.Parameters.Add(parInput1SetOn)

            Dim parInput2OnTime As New SqlClient.SqlParameter("@Input2OnTime", SqlDbType.Int)
            parInput2OnTime.Value = data.input2OnTime
            Command.Parameters.Add(parInput2OnTime)

            Dim parInput2SetOn As New SqlClient.SqlParameter("@Input2SetOn", SqlDbType.DateTime)
            parInput2SetOn.Value = data.input2SetOn
            Command.Parameters.Add(parInput2SetOn)

            Dim parInput3OnTime As New SqlClient.SqlParameter("@Input3OnTime", SqlDbType.Int)
            parInput3OnTime.Value = data.input3OnTime
            Command.Parameters.Add(parInput3OnTime)

            Dim parInput3SetOn As New SqlClient.SqlParameter("@Input3SetOn", SqlDbType.DateTime)
            parInput3SetOn.Value = data.input3SetOn
            Command.Parameters.Add(parInput3SetOn)

            Dim parInput4OnTime As New SqlClient.SqlParameter("@Input4OnTime", SqlDbType.Int)
            parInput4OnTime.Value = data.input4OnTime
            Command.Parameters.Add(parInput4OnTime)

            Dim parInput4SetOn As New SqlClient.SqlParameter("@Input4SetOn", SqlDbType.DateTime)
            parInput4SetOn.Value = data.input4SetOn
            Command.Parameters.Add(parInput4SetOn)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in Hour Meter set up"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.saveHourMeter", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

#End Region

#Region "Geofence Types"

    Public Function getAllGeofenceTypes(ByVal token As String) As List(Of geofenceType)
        Dim lst As New List(Of geofenceType)
        Dim itm As New geofenceType
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "GeofencesTypes_GetByCompanyID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New geofenceType
                itm.companyId = reader.Item("companyId")
                itm.id = reader.Item("id")
                itm.name = reader.Item("Name")
                itm.iconId = reader.Item("IconID")
                itm.iconUrl = reader.Item("IconURL")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function saveGeofenceType(token As String, data As geofenceType, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "GeofencesTypes_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = data.id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parIconID As New SqlClient.SqlParameter("@IconID", SqlDbType.Int)
            parIconID.Value = data.iconId
            Command.Parameters.Add(parIconID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in saveGeofenceType"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.saveGeofenceType", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

    Public Function deleteGeofenceType(ByVal token As String, ByVal id As Integer, ByRef msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "GeofencesTypes_DELETE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in deleteGeofenceType"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.deleteGeofenceType", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

#End Region

#Region "Maintenance"

    Public Function maintSupportListsGET(ByVal token As String) As maintSupportLists
        Dim lists As New maintSupportLists
        Dim reader As SqlDataReader = Nothing
        Dim itm As basicList


        Try
            strCommand = "Maint_SupportLists_Get"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            'SERVICES TYPES
            Dim servicesTypes As New List(Of basicList)
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                servicesTypes.Add(itm)
            Loop
            lists.servicesTypes = servicesTypes

            'TIME REFERENCES
            Dim timeReferences As New List(Of basicList)
            reader.NextResult()
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                timeReferences.Add(itm)
            Loop
            lists.timeReferences = timeReferences

            'VEHICLE TYPES
            Dim vehicleTypes As New List(Of basicList)
            reader.NextResult()
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                vehicleTypes.Add(itm)
            Loop
            lists.vehicleTypes = vehicleTypes

            'MAINTENANCE TASKS
            Dim task As maintTask
            Dim maintTasks As New List(Of maintTask)
            reader.NextResult()
            Do While reader.Read
                task = New maintTask
                task.id = reader.Item("ID")
                task.name = reader.Item("Name")
                task.meassureId = reader.Item("TaskMeassureID")
                task.value = reader.Item("Value")
                task.meassureName = reader.Item("MeassureName")
                maintTasks.Add(task)
            Loop
            lists.maintTasks = maintTasks

            'MAINTENANCE MEASSURES
            Dim meassure As maintMeassure
            Dim meassures As New List(Of maintMeassure)
            reader.NextResult()
            Do While reader.Read
                meassure = New maintMeassure
                meassure.id = reader.Item("ID")
                meassure.name = reader.Item("Name")
                meassure.unitName = reader.Item("UnitName")
                meassures.Add(meassure)
            Loop
            lists.maintMeassures = meassures

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lists

    End Function

    Public Function maintDeviceList(ByVal token As String) As List(Of maintDevice)
        Dim lst As New List(Of maintDevice)
        Dim itm As New maintDevice
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Maint_Devices_Get"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If


            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New maintDevice
                itm.id = reader.Item("id")
                itm.name = reader.Item("Name")
                itm.typeId = reader.Item("VehicleTypeID")
                itm.make = reader.Item("make")
                itm.model = reader.Item("model")
                itm.modelYear = reader.Item("modelYear")
                itm.insuranceCarrier = reader.Item("insuranceCarrier")
                itm.insurancePolicyNo = reader.Item("insurancePolicyNo")
                itm.insurancePremium = reader.Item("insurancePremium")
                itm.insuranceDueOn = reader.Item("insuranceDueOn")
                itm.odometer = reader.Item("odometer")
                itm.ignitionHours = reader.Item("IgnitionOnTime")
                itm.input1Name = reader.Item("input1Name")
                itm.input1Hours = reader.Item("Input1OnTime")
                itm.input2Name = reader.Item("input2Name")
                itm.input2Hours = reader.Item("Input2OnTime")
                itm.input3Name = reader.Item("input3Name")
                itm.input3Hours = reader.Item("Input3OnTime")
                itm.input4Name = reader.Item("input4Name")
                itm.input4Hours = reader.Item("Input4OnTime")
                itm.notes = reader.Item("MaintenanceNotes")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function maintDeviceSave(token As String, data As maintDevice, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "Maint_Devices_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceGUID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.id
            Command.Parameters.Add(parDeviceID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parTypeID.Value = data.typeId
            Command.Parameters.Add(parTypeID)

            Dim parMake As New SqlClient.SqlParameter("@Make", SqlDbType.NVarChar, 50)
            parMake.Value = data.make
            Command.Parameters.Add(parMake)

            Dim parModel As New SqlClient.SqlParameter("@Model", SqlDbType.NVarChar, 50)
            parModel.Value = data.model
            Command.Parameters.Add(parModel)

            Dim parModelYear As New SqlClient.SqlParameter("@ModelYear", SqlDbType.Int)
            parModelYear.Value = data.modelYear
            Command.Parameters.Add(parModelYear)

            Dim parInsCarrier As New SqlClient.SqlParameter("@InsuranceCarrier", SqlDbType.NVarChar, 50)
            parInsCarrier.Value = data.insuranceCarrier
            Command.Parameters.Add(parInsCarrier)

            Dim parInsPolicy As New SqlClient.SqlParameter("@InsurancePolicyNo", SqlDbType.NVarChar, 50)
            parInsPolicy.Value = data.insurancePolicyNo
            Command.Parameters.Add(parInsPolicy)

            Dim parInsPremium As New SqlClient.SqlParameter("@InsurancePremium", SqlDbType.Decimal, 2)
            parInsPremium.Precision = 10
            parInsPremium.Scale = 2
            parInsPremium.Value = data.insurancePremium
            Command.Parameters.Add(parInsPremium)

            Dim parInsDueOn As New SqlClient.SqlParameter("@InsuranceDueOn", SqlDbType.Date)
            parInsDueOn.Value = data.datInsuranceDueOn
            Command.Parameters.Add(parInsDueOn)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            parOdometer.Value = data.odometer
            Command.Parameters.Add(parOdometer)

            Dim parOdometerChanged As New SqlClient.SqlParameter("@OdometerChanged", SqlDbType.Bit)
            parOdometerChanged.Value = data.odometerChanged
            Command.Parameters.Add(parOdometerChanged)

            Dim parIgnitionTime As New SqlClient.SqlParameter("@IgnitionTime", SqlDbType.Int)
            parIgnitionTime.Value = data.ignitionHours * 3600
            Command.Parameters.Add(parIgnitionTime)

            Dim parIgnitionTimeChanged As New SqlClient.SqlParameter("@IgnitionTimeChanged", SqlDbType.Bit)
            parIgnitionTimeChanged.Value = data.ignitionHoursChanged
            Command.Parameters.Add(parIgnitionTimeChanged)

            Dim parInput1Name As New SqlClient.SqlParameter("@Input1Name", SqlDbType.NVarChar, 50)
            parInput1Name.Value = data.input1Name
            Command.Parameters.Add(parInput1Name)

            Dim parInput1OnTime As New SqlClient.SqlParameter("@Input1OnTime", SqlDbType.Int)
            parInput1OnTime.Value = data.input1Hours * 3600
            Command.Parameters.Add(parInput1OnTime)

            Dim parInput1OnTimeChanged As New SqlClient.SqlParameter("@Input1OnTimeChanged", SqlDbType.Bit)
            parInput1OnTimeChanged.Value = data.input1HoursChanged
            Command.Parameters.Add(parInput1OnTimeChanged)

            Dim parInput2Name As New SqlClient.SqlParameter("@Input2Name", SqlDbType.NVarChar, 50)
            parInput2Name.Value = data.input2Name
            Command.Parameters.Add(parInput2Name)

            Dim parInput2OnTime As New SqlClient.SqlParameter("@Input2OnTime", SqlDbType.Int)
            parInput2OnTime.Value = data.input2Hours * 3600
            Command.Parameters.Add(parInput2OnTime)

            Dim parInput2OnTimeChanged As New SqlClient.SqlParameter("@Input2OnTimeChanged", SqlDbType.Bit)
            parInput2OnTimeChanged.Value = data.input2HoursChanged
            Command.Parameters.Add(parInput2OnTimeChanged)

            Dim parInput3Name As New SqlClient.SqlParameter("@Input3Name", SqlDbType.NVarChar, 50)
            parInput3Name.Value = data.input3Name
            Command.Parameters.Add(parInput3Name)

            Dim parInput3OnTime As New SqlClient.SqlParameter("@Input3OnTime", SqlDbType.Int)
            parInput3OnTime.Value = data.input3Hours * 3600
            Command.Parameters.Add(parInput3OnTime)

            Dim parInput3OnTimeChanged As New SqlClient.SqlParameter("@Input3OnTimeChanged", SqlDbType.Bit)
            parInput3OnTimeChanged.Value = data.input3HoursChanged
            Command.Parameters.Add(parInput3OnTimeChanged)

            Dim parInput4Name As New SqlClient.SqlParameter("@Input4Name", SqlDbType.NVarChar, 50)
            parInput4Name.Value = data.input4Name
            Command.Parameters.Add(parInput4Name)

            Dim parInput4OnTime As New SqlClient.SqlParameter("@Input4OnTime", SqlDbType.Int)
            parInput4OnTime.Value = data.input4Hours * 3600
            Command.Parameters.Add(parInput4OnTime)

            Dim parInput4OnTimeChanged As New SqlClient.SqlParameter("@Input4OnTimeChanged", SqlDbType.Bit)
            parInput4OnTimeChanged.Value = data.input4HoursChanged
            Command.Parameters.Add(parInput4OnTimeChanged)

            Dim parNotes As New SqlClient.SqlParameter("@MaintenanceNotes", SqlDbType.VarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error Saving Device"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintDeviceSave", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

    Public Function maintScheduleGetByDevice(ByVal token As String, ByVal deviceId As String, ByVal id As String) As maintDeviceSchedule
        Dim maintSchedule As New maintDeviceSchedule
        Dim schedules As New List(Of scheduleItem)
        Dim meassures As New List(Of maintMeassure)
        Dim itm As scheduleItem
        Dim itm2 As maintMeassure
        Dim reader As SqlDataReader = Nothing
        Dim var1 As Integer = 0

        Try
            strCommand = "Maint_Schedule_GetByDevice"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            maintSchedule.deviceId = deviceId

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New scheduleItem
                itm.id = reader.Item("ID")
                itm.taskId = reader.Item("TaskID")
                itm.taskName = reader.Item("TaskName")
                itm.repeatEveryX = reader.Item("RepeatEveryX")
                itm.repeatEveryTimeRefID = reader.Item("RepeatEveryTimeRefID")
                itm.meassureID = reader.Item("MeassureID")
                itm.meassureValue = reader.Item("MeassureValue")
                itm.frequency = reader.Item("FrequencyInWords")
                itm.taskValue = reader.Item("TaskValue")
                itm.lastServiceOn = reader.Item("LastServiceOn")
                itm.nextDue = reader.Item("NextDue")
                itm.nextMeassureValue = reader.Item("NextMeassureValue")
                itm.daysUntilDue = reader.Item("DaysUntilDue")
                itm.reminderTimeRefId = reader.Item("ReminderTimeRefId")
                itm.reminderTimeRefVal = reader.Item("ReminderTimeRefVal")
                itm.reminderMeassureVal = reader.Item("ReminderMeassureVal")
                itm.reminder = reader.Item("ReminderInWords")
                itm.notes = reader.Item("Notes")
                itm.nextDueInWords = ""
                schedules.Add(itm)
            Loop
            maintSchedule.schedules = schedules

            reader.NextResult()
            Do While reader.Read
                itm2 = New maintMeassure
                itm2.id = reader.Item("ID")
                itm2.name = reader.Item("Name")
                itm2.unitName = reader.Item("UnitName")
                meassures.Add(itm2)
            Loop
            maintSchedule.meassures = meassures

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return maintSchedule

    End Function

    Public Function maintItemSave(ByVal token As String, ByVal deviceId As String, ByVal data As scheduleItem, ByRef id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Maint_Schedules_SaveItem"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = data.id
            Command.Parameters.Add(parID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.NVarChar, 50)
            parTaskID.Value = data.taskId
            Command.Parameters.Add(parTaskID)

            Dim parRepeatEveryX As New SqlClient.SqlParameter("@RepeatEveryX", SqlDbType.Int)
            parRepeatEveryX.Value = data.repeatEveryX
            Command.Parameters.Add(parRepeatEveryX)

            Dim parRepeatEveryTimeRefID As New SqlClient.SqlParameter("@RepeatEveryTimeRefID", SqlDbType.Int)
            parRepeatEveryTimeRefID.Value = data.repeatEveryTimeRefID
            Command.Parameters.Add(parRepeatEveryTimeRefID)

            Dim parMeassureID As New SqlClient.SqlParameter("@MeassureID", SqlDbType.Int)
            parMeassureID.Value = data.meassureID
            Command.Parameters.Add(parMeassureID)

            Dim parMeassureValue As New SqlClient.SqlParameter("@MeassureValue", SqlDbType.Int)
            parMeassureValue.Value = data.meassureValue
            Command.Parameters.Add(parMeassureValue)

            Dim parTaskValue As New SqlClient.SqlParameter("@TaskValue", SqlDbType.Int)
            parTaskValue.Value = data.taskValue
            Command.Parameters.Add(parTaskValue)

            Dim parLastServiceOn As New SqlClient.SqlParameter("@LastServiceOn", SqlDbType.Date)
            parLastServiceOn.Value = data.lastServiceOn
            Command.Parameters.Add(parLastServiceOn)

            Dim parNextDue As New SqlClient.SqlParameter("@NextDue", SqlDbType.Date)
            parNextDue.Value = data.nextDue
            Command.Parameters.Add(parNextDue)

            Dim parNextMeassureValue As New SqlClient.SqlParameter("@NextMeassureValue", SqlDbType.Int)
            parNextMeassureValue.Value = data.nextMeassureValue
            Command.Parameters.Add(parNextMeassureValue)

            Dim parReminderTimeRefId As New SqlClient.SqlParameter("@ReminderTimeRefId", SqlDbType.Int)
            parReminderTimeRefId.Value = data.reminderTimeRefId
            Command.Parameters.Add(parReminderTimeRefId)

            Dim parReminderTimeRefVal As New SqlClient.SqlParameter("@ReminderTimeRefVal", SqlDbType.Int)
            parReminderTimeRefVal.Value = data.reminderTimeRefVal
            Command.Parameters.Add(parReminderTimeRefVal)

            Dim parReminderMeassureVal As New SqlClient.SqlParameter("@ReminderMeassureVal", SqlDbType.Int)
            parReminderMeassureVal.Value = data.reminderMeassureVal
            Command.Parameters.Add(parReminderMeassureVal)

            Dim parValueSinceLastService As New SqlClient.SqlParameter("@ValueSinceLastService", SqlDbType.Decimal)
            parValueSinceLastService.Value = data.valueSinceLastService
            Command.Parameters.Add(parValueSinceLastService)

            Dim parNotes As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parNewID As New SqlClient.SqlParameter("@NewID", SqlDbType.VarChar, 50)
            parNewID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewID)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            id = CStr(parNewID.Value)
            msg = CStr(parMsg.Value)

        Catch ex As Exception
            strError = "Error Saving Schedule Item"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintItemSave", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function maintItemDelete(ByVal token As String, ByVal id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Maint_Schedules_REMOVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Schedule Item"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintItemDelete", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function maintCompletedItemSave_DEPRECATED(ByVal token As String, ByVal deviceId As String, ByVal data As scheduleItem, ByRef id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            'IMPORTANT: THIS CALL TO THIS SP IS INCOMPLETE.  THE PARAM "VAL" IS NOT BEING PASSED AND IT IS VERY IMPORTANT.
            strCommand = "Maint_ServicesLog_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            '@Comments NVARCHAR(MAX) = '',
            '@ServiceLogGUID NVARCHAR(50) = '' OUTPUT

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = data.jobId
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parTypeID.Value = data.jobTypeId
            Command.Parameters.Add(parTypeID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.NVarChar, 50)
            parTaskID.Value = data.taskId
            Command.Parameters.Add(parTaskID)

            Dim parJobDesc As New SqlClient.SqlParameter("@ServiceDescription", SqlDbType.NVarChar, -1)
            parJobDesc.Value = data.jobDescription
            Command.Parameters.Add(parJobDesc)

            Dim parJobDate As New SqlClient.SqlParameter("@ServiceDate", SqlDbType.Date)
            parJobDate.Value = data.completedOn
            Command.Parameters.Add(parJobDate)

            '@Val DECIMAL(18,2) = 0,

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            parOdometer.Value = data.odometer
            Command.Parameters.Add(parOdometer)

            Dim parJobCost As New SqlClient.SqlParameter("@Cost", SqlDbType.Decimal)
            parJobCost.Precision = 18
            parJobCost.Scale = 2
            parJobCost.Value = data.cost
            Command.Parameters.Add(parJobCost)

            'Dim val As Decimal

            'Dim parIgnition As New SqlClient.SqlParameter("@IgnitionOnTime", SqlDbType.Int)
            'parIgnition.Value = data.ignitionHours
            'Command.Parameters.Add(parIgnition)

            'Dim parInput1 As New SqlClient.SqlParameter("@Input1OnTime", SqlDbType.Int)
            'parInput1.Value = data.input1Hours
            'Command.Parameters.Add(parInput1)

            'Dim parInput2 As New SqlClient.SqlParameter("@Input2OnTime", SqlDbType.Int)
            'parInput2.Value = data.input2Hours
            'Command.Parameters.Add(parInput2)

            'Dim parInput3 As New SqlClient.SqlParameter("@Input3OnTime", SqlDbType.Int)
            'parInput3.Value = data.input3Hours
            'Command.Parameters.Add(parInput3)

            'Dim parInput4 As New SqlClient.SqlParameter("@Input4OnTime", SqlDbType.Int)
            'parInput4.Value = data.input4Hours
            'Command.Parameters.Add(parInput4)

            Dim parNotes As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parNewID As New SqlClient.SqlParameter("@ServiceLogGUID", SqlDbType.VarChar, 50)
            parNewID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewID)

            'Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            'parMsg.Direction = ParameterDirection.Output
            'Command.Parameters.Add(parMsg)

            'Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            'parIsOk.Direction = ParameterDirection.Output
            'Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            'bResult = CBool(parIsOk.Value)
            id = CStr(parNewID.Value)
            'msg = CStr(parMsg.Value)

        Catch ex As Exception
            strError = "Error Saving Schedule Item"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintItemSave", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function maintLogGetByDevice(ByVal token As String, ByVal deviceId As String, ByVal id As String) As maintDeviceLog
        Dim maintLog As New maintDeviceLog
        Dim logs As New List(Of scheduleItem)
        Dim itm As scheduleItem
        Dim reader As SqlDataReader = Nothing
        Dim var1 As Integer = 0

        Try
            strCommand = "Maint_ServicesLog_GetByDevice"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.NVarChar, 50)
            parTaskID.Value = ""
            Command.Parameters.Add(parTaskID)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            maintLog.deviceId = deviceId

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New scheduleItem
                itm.id = reader.Item("ID")
                itm.taskId = reader.Item("TaskID")
                itm.taskName = reader.Item("TaskName")
                itm.jobDescription = reader.Item("ServiceDescription")
                itm.completedOn = reader.Item("ServiceDate")
                itm.cost = reader.Item("TotalCost")
                itm.odometer = reader.Item("Odometer")
                itm.ignitionHours = reader.Item("IgnitionOnTime")
                itm.input1Hours = reader.Item("Input1OnTime")
                itm.input2Hours = reader.Item("Input2OnTime")
                itm.input3Hours = reader.Item("Input3OnTime")
                itm.input4Hours = reader.Item("Input4OnTime")
                itm.notes = reader.Item("Comments")
                logs.Add(itm)
            Loop
            maintLog.logs = logs

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return maintLog

    End Function

#End Region

#Region "QB Match"

    Public Function QBMatch_CRMCustomers(ByVal token As String, ByRef isValidRequest As Boolean) As List(Of crmCustomer)
        Dim lst As New List(Of crmCustomer)
        Dim reader As SqlDataReader = Nothing
        Dim itm As crmCustomer

        Try
            strCommand = "CRM_QB_CustomersGET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                itm = New crmCustomer
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.qbId = reader.Item("QB_ListID")
                itm.dealerId = reader.Item("DealerID")
                itm.dealerName = reader.Item("DealerName")
                itm.isMatched = reader.Item("IsMatched")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            isValidRequest = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If

            If lst.Count > 0 Then
                isValidRequest = True
            End If

        End Try

        Return lst

    End Function

    Public Function QBMatch_QBCustomers() As List(Of qbCustomer)
        Dim lst As New List(Of qbCustomer)
        Dim reader As SqlDataReader = Nothing
        Dim itm As qbCustomer

        Try
            strCommand = "CRM_QB_CustomersGET"
            conString = ConfigurationManager.AppSettings("QBDB")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                itm = New qbCustomer
                itm.id = reader.Item("ID")
                itm.companyName = reader.Item("CompanyName")
                itm.contactName = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function qbLinkCustomers(ByVal token As String, ByVal crmId As String, ByVal qbId As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "CRM_QB_CustomersLink"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCRMId As New SqlClient.SqlParameter("@CRMID", SqlDbType.NVarChar, 50)
            parCRMId.Value = crmId
            Command.Parameters.Add(parCRMId)

            Dim parQBId As New SqlClient.SqlParameter("@QBID", SqlDbType.NVarChar, 50)
            parQBId.Value = qbId
            Command.Parameters.Add(parQBId)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in qbLinkCustomers"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.qbLinkCustomers", "", ex.Message & " - Token: " & token, 0)
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "User Preferences"

    Public Function updateUserPref(ByVal token As String, ByVal data As userPreference) As Boolean
        Dim bOk As Boolean = True

        Try
            strCommand = "UserPreference_UPDATE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parModuleName As New SqlClient.SqlParameter("@ModuleName", SqlDbType.NVarChar, 50)
            parModuleName.Value = data.moduleName
            Command.Parameters.Add(parModuleName)

            Dim parPreference As New SqlClient.SqlParameter("@Preference", SqlDbType.NVarChar, 50)
            parPreference.Value = data.preference
            Command.Parameters.Add(parPreference)

            Dim parVal1 As New SqlClient.SqlParameter("@Val1", SqlDbType.NVarChar, 50)
            parVal1.Value = data.val1
            Command.Parameters.Add(parVal1)

            Dim parVal2 As New SqlClient.SqlParameter("@Val2", SqlDbType.NVarChar, 50)
            parVal2.Value = data.val2
            Command.Parameters.Add(parVal2)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bOk = CBool(parIsOk.Value)

        Catch ex As Exception
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

#End Region

#Region "CRM Related"

    Function EmailTypesGET(ByVal token As String, ByVal type As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_EmailTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parType As New SqlClient.SqlParameter("@Type", SqlDbType.NVarChar, 50)
            parType.Direction = ParameterDirection.Input
            parType.Value = type
            Command.Parameters.Add(parType)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Function GenericMastersGET(ByVal token As String, ByVal masterKey As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_GenericMasters_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parMasterKey As New SqlClient.SqlParameter("@MasterKey", SqlDbType.NVarChar, 50)
            parMasterKey.Direction = ParameterDirection.Input
            parMasterKey.Value = masterKey
            Command.Parameters.Add(parMasterKey)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Public Sub sendEmail(ByVal token As String, ByVal custId As String, ByVal emailTypeId As String, ByVal courrierId As String, ByVal trackingNumber As String)
        Try
            strCommand = "CRM_SendEmail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEmailTypeId As New SqlClient.SqlParameter("@EmailTypeId", SqlDbType.Int)
            If IsNumeric(emailTypeId) Then
                parEmailTypeId.Value = emailTypeId
            Else
                parEmailTypeId.Value = 0
            End If
            Command.Parameters.Add(parEmailTypeId)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            Dim parCourrierId As New SqlClient.SqlParameter("@CourrierId", SqlDbType.NVarChar, 50)
            parCourrierId.Value = courrierId
            Command.Parameters.Add(parCourrierId)

            Dim parTrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 50)
            parTrackingNumber.Value = trackingNumber
            Command.Parameters.Add(parTrackingNumber)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error sending email"
            BLErrorHandling.ErrorCapture(pSysModule, "sendEmail", "", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

    End Sub

    Public Sub SendShipmentEmail(ByVal token As String, ByVal orderNo As String, ByVal emailTypeId As String, ByVal courrierId As String, ByVal trackingNumber As String)
        Try
            strCommand = "CRM_SendShipmentEmail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEmailTypeId As New SqlClient.SqlParameter("@EmailTypeId", SqlDbType.Int)
            If IsNumeric(emailTypeId) Then
                parEmailTypeId.Value = emailTypeId
            Else
                parEmailTypeId.Value = 0
            End If
            Command.Parameters.Add(parEmailTypeId)

            Dim parOrderNo As New SqlClient.SqlParameter("@OrderNo", SqlDbType.NVarChar, 50)
            parOrderNo.Value = orderNo
            Command.Parameters.Add(parOrderNo)

            Dim parCourrierId As New SqlClient.SqlParameter("@CourrierId", SqlDbType.NVarChar, 50)
            parCourrierId.Value = courrierId
            Command.Parameters.Add(parCourrierId)

            Dim parTrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 50)
            parTrackingNumber.Value = trackingNumber
            Command.Parameters.Add(parTrackingNumber)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error sending shipment email"
            BLErrorHandling.ErrorCapture(pSysModule, "SendShipmentEmail", "", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

    End Sub

#End Region

#Region "CRM - Invoices"

    Function crmGetInvoices(ByVal token As String, ByVal custId As String) As List(Of invoice)
        Dim lst As New List(Of invoice)
        Dim itm As invoice
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "CRM_InvoicesByCustomer"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New invoice
                itm.invoiceNumber = reader.Item("RefNumber")
                itm.invoiceDate = reader.Item("TxnDate")
                itm.total = reader.Item("Subtotal")
                itm.paid = reader.Item("AppliedAmount")
                itm.balance = reader.Item("BalanceRemaining")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Snooze Report"

    Public Function snoozeReport(ByVal rpt As String, ByVal usr As String, ByVal period As Integer) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Reports_Snooze"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parRpt As New SqlClient.SqlParameter("@EmailTemplateID", SqlDbType.NVarChar, 50)
            parRpt.Value = rpt
            Command.Parameters.Add(parRpt)

            Dim parUsr As New SqlClient.SqlParameter("@UserId", SqlDbType.NVarChar, 50)
            parUsr.Value = usr
            Command.Parameters.Add(parUsr)

            Dim parPeriod As New SqlClient.SqlParameter("@Period", SqlDbType.Int)
            parPeriod.Value = period
            Command.Parameters.Add(parPeriod)

            Dim parIsOk As New SqlClient.SqlParameter("@isOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = parIsOk.Value

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "snoozeReport", "", ex.Message & " - usr: " & usr, 0)
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "SettingsDevices Service"

    Public Function deviceSettingsListGET(ByVal token As String) As List(Of deviceSettings)
        Dim lst As New List(Of deviceSettings)
        Dim reader As SqlDataReader = Nothing
        Dim itm As deviceSettings

        Try
            strCommand = "Devices_DeviceSetting_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                Try
                    itm = New deviceSettings
                    itm.id = reader.Item("ID")
                    itm.deviceId = reader.Item("DeviceID")
                    itm.name = reader.Item("Name")
                    itm.shortName = reader.Item("ShortName")
                    itm.textColor = reader.Item("TextColor")
                    itm.bgndColor = reader.Item("BgndColor")
                    itm.lastUpdatedOn = reader.Item("LastUpdatedOn")
                    itm.lastEventName = reader.Item("EventName")
                    itm.lastEventOn = reader.Item("EventDate")
                    itm.eventCodeStartedOn = reader.Item("EventCodeStartedOn")
                    itm.driverId = reader.Item("DriverID")
                    itm.driverName = reader.Item("DriverName")
                    itm.idleLimit = reader.Item("IdleThreshold")
                    itm.speedLimit = reader.Item("SpeedingThreshold")
                    itm.odometer = reader.Item("OdometerReading")
                    itm.serialNumber = reader.Item("SerialNumber")
                    itm.vin = reader.Item("Vin")
                    itm.licensePlate = reader.Item("LicensePlate")
                    itm.iconId = reader.Item("IconID")
                    itm.iconURL = reader.Item("IconURL")
                    itm.iconLabelLine2 = reader.Item("IconLabelLine2")
                    itm.isARB = reader.Item("IsARB")
                    itm.arbNumber = reader.Item("ARBNumber")
                    itm.dieselMeter = reader.Item("DieselMeter")
                    itm.electricMeter = reader.Item("ElectricMeter")
                    itm.isBuzzerOn = reader.Item("IsBuzzerOn")
                    itm.deviceStatus = reader.Item("DeviceStatus")
                    itm.sleepingSince = reader.Item("SleepingSince")
                    itm.cancelledSince = reader.Item("CancelledSince")
                    itm.assignedOn = reader.Item("AssignedOn")
                    lst.Add(itm)
                Catch ex As Exception

                End Try
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If

        End Try

        Return lst

    End Function

    Public Function deviceSettings_GetDevice(ByVal token As String, ByVal id As String) As deviceSettings
        Dim itm As New deviceSettings
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_DeviceSetting_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                itm.id = reader.Item("ID")
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("Name")
                itm.shortName = reader.Item("ShortName")
                itm.textColor = reader.Item("TextColor")
                itm.bgndColor = reader.Item("BgndColor")
                itm.lastUpdatedOn = reader.Item("LastUpdatedOn")
                itm.lastEventName = reader.Item("EventName")
                itm.lastEventOn = reader.Item("EventDate")
                itm.eventCodeStartedOn = reader.Item("EventCodeStartedOn")
                itm.driverId = reader.Item("DriverID")
                itm.driverName = reader.Item("DriverName")
                itm.idleLimit = reader.Item("IdleThreshold")
                itm.speedLimit = reader.Item("SpeedingThreshold")
                itm.odometer = reader.Item("OdometerReading")
                itm.serialNumber = reader.Item("SerialNumber")
                itm.vin = reader.Item("Vin")
                itm.licensePlate = reader.Item("LicensePlate")
                itm.iconId = reader.Item("IconID")
                itm.iconURL = reader.Item("IconURL")
                itm.iconLabelLine2 = reader.Item("IconLabelLine2")
                itm.isARB = reader.Item("IsARB")
                itm.arbNumber = reader.Item("ARBNumber")
                itm.dieselMeter = reader.Item("DieselMeter")
                itm.electricMeter = reader.Item("ElectricMeter")
                itm.isBuzzerOn = reader.Item("IsBuzzerOn")
                itm.deviceStatus = reader.Item("DeviceStatus")
                itm.sleepingSince = reader.Item("SleepingSince")
                itm.cancelledSince = reader.Item("CancelledSince")
                itm.assignedOn = reader.Item("AssignedOn")
                itm.fuelCardUnitId = reader.Item("FuelCardUnitID")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function deviceSettingsSaveDevice(ByVal token As String, ByVal device As deviceSettings) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Devices_UserUpdates"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = device.deviceId
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = device.name
            Command.Parameters.Add(parName)

            Dim parShortName As New SqlClient.SqlParameter("@ShortName", SqlDbType.NVarChar, 20)
            parShortName.Value = device.shortName
            Command.Parameters.Add(parShortName)

            Dim parTextColor As New SqlClient.SqlParameter("@TextColor", SqlDbType.NVarChar, 10)
            parTextColor.Value = device.textColor
            Command.Parameters.Add(parTextColor)

            Dim parBgndColor As New SqlClient.SqlParameter("@BgndColor", SqlDbType.NVarChar, 10)
            parBgndColor.Value = device.bgndColor
            Command.Parameters.Add(parBgndColor)

            Dim parDriverID As New SqlClient.SqlParameter("@DriverId", SqlDbType.Int)
            If IsNumeric(device.driverId) Then
                parDriverID.Value = device.driverId
            Else
                parDriverID.Value = 0
            End If
            Command.Parameters.Add(parDriverID)

            Dim parIdleLimit As New SqlClient.SqlParameter("@IdleLimit", SqlDbType.Int, 4)
            If IsNumeric(device.idleLimit) Then
                parIdleLimit.Value = device.idleLimit
            Else
                parIdleLimit.Value = 0
            End If
            Command.Parameters.Add(parIdleLimit)

            Dim parSpeedLimit As New SqlClient.SqlParameter("@SpeedLimit", SqlDbType.Int, 4)
            If IsNumeric(device.speedLimit) Then
                parSpeedLimit.Value = device.speedLimit
            Else
                parSpeedLimit.Value = 0
            End If
            Command.Parameters.Add(parSpeedLimit)

            Dim parOdometer As New SqlClient.SqlParameter("@OdometerReading", SqlDbType.Decimal)
            If IsNumeric(device.odometer) Then
                parOdometer.Value = device.odometer
            Else
                parOdometer.Value = 0
            End If
            Command.Parameters.Add(parOdometer)

            Dim parVIN As New SqlClient.SqlParameter("@VIN", SqlDbType.NVarChar, 50)
            parVIN.Value = device.vin
            Command.Parameters.Add(parVIN)

            Dim parLicensePlate As New SqlClient.SqlParameter("@LicensePlate", SqlDbType.NVarChar, 50)
            parLicensePlate.Value = device.licensePlate
            Command.Parameters.Add(parLicensePlate)

            Dim parIconID As New SqlClient.SqlParameter("@IconID", SqlDbType.Int)
            If IsNumeric(device.iconId) Then
                parIconID.Value = device.iconId
            Else
                parIconID.Value = 0
            End If
            Command.Parameters.Add(parIconID)

            Dim parIconLabelLine2 As New SqlClient.SqlParameter("@IconLabelLine2", SqlDbType.NVarChar, 10)
            parIconLabelLine2.Value = device.iconLabelLine2
            Command.Parameters.Add(parIconLabelLine2)

            Dim parIsARB As New SqlClient.SqlParameter("@IsARB", SqlDbType.Bit)
            If device.isARB.ToString.ToLower = "true" Then
                parIsARB.Value = True
            Else
                parIsARB.Value = False
            End If
            Command.Parameters.Add(parIsARB)

            Dim parARBNumber As New SqlClient.SqlParameter("@ARBNumber", SqlDbType.NVarChar, 10)
            parARBNumber.Value = device.arbNumber
            Command.Parameters.Add(parARBNumber)

            Dim parDieselMeter As New SqlClient.SqlParameter("@DieselMeter", SqlDbType.Decimal)
            If IsNumeric(device.dieselMeter) Then
                parDieselMeter.Value = device.dieselMeter
            Else
                parDieselMeter.Value = 0
            End If
            Command.Parameters.Add(parDieselMeter)

            Dim parElectricMeter As New SqlClient.SqlParameter("@ElectricMeter", SqlDbType.Decimal)
            If IsNumeric(device.electricMeter) Then
                parElectricMeter.Value = device.electricMeter
            Else
                parElectricMeter.Value = 0
            End If
            Command.Parameters.Add(parElectricMeter)

            Dim parIsBuzzerOn As New SqlClient.SqlParameter("@IsBuzzerOn", SqlDbType.Bit)
            If device.isBuzzerOn.ToString.ToLower = "true" Then
                parIsBuzzerOn.Value = True
            Else
                parIsBuzzerOn.Value = False
            End If
            Command.Parameters.Add(parIsBuzzerOn)

            Dim parFuelCardID As New SqlClient.SqlParameter("@FuelCardUnitId", SqlDbType.NVarChar, 20)
            parFuelCardID.Value = device.fuelCardUnitId
            Command.Parameters.Add(parFuelCardID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)


            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.transId = CStr(parGUID.Value)
            res.isOk = True

        Catch ex As Exception
            res.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Public Function deviceSettings_ChangeStatus(ByVal token As String, ByVal action As String, ByVal id As String, ByVal usrComment As String) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Devices_DeviceSetting_ChangeStatus"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parAction As New SqlClient.SqlParameter("@Action", SqlDbType.NVarChar, 10)
            parAction.Value = action
            Command.Parameters.Add(parAction)

            Dim parComment As New SqlClient.SqlParameter("@UsrComment", SqlDbType.NVarChar, -1)
            parComment.Value = usrComment
            Command.Parameters.Add(parComment)

            Dim parIsOk As New SqlClient.SqlParameter("@isOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = parIsOk.Value

        Catch ex As Exception
            res.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Fuel Card"

    Public Function saveFuelLogUpload(ByVal token As String, _
                                      ByVal deviceId As String, _
                                      ByVal logDate As String, _
                                      ByVal logTime As String, _
                                      ByVal driver As String, _
                                      ByVal street As String, _
                                      ByVal city As String, _
                                      ByVal state As String, _
                                      ByVal zip As String, _
                                      ByVal galls As Decimal, _
                                      ByVal price As Decimal, _
                                      ByVal amt As Decimal, _
                                      ByVal cardNumber As String, _
                                      ByVal merchantName As String, _
                                      ByVal odometer As Decimal, _
                                      ByVal lat As Decimal, _
                                      ByVal lng As Decimal, _
                                      ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            msg = ""
            strCommand = "Maint_FuelLog_FuelCardUpload"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 20)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parLogDate As New SqlClient.SqlParameter("@LogDate", SqlDbType.NVarChar, 20)
            parLogDate.Value = logDate
            Command.Parameters.Add(parLogDate)

            Dim parLogTime As New SqlClient.SqlParameter("@LogTime", SqlDbType.NVarChar, 20)
            parLogTime.Value = logTime
            Command.Parameters.Add(parLogTime)

            Dim parDriver As New SqlClient.SqlParameter("@Driver", SqlDbType.NVarChar, 20)
            parDriver.Value = driver
            Command.Parameters.Add(parDriver)

            Dim parCardNumber As New SqlClient.SqlParameter("@CardNumber", SqlDbType.NVarChar, 50)
            parCardNumber.Value = cardNumber
            Command.Parameters.Add(parCardNumber)

            Dim parMerchantName As New SqlClient.SqlParameter("@MerchantName", SqlDbType.NVarChar, 50)
            parMerchantName.Value = merchantName
            Command.Parameters.Add(parMerchantName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parZip As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 20)
            parZip.Value = zip
            Command.Parameters.Add(parZip)

            Dim parGalls As New SqlClient.SqlParameter("@Gallons", SqlDbType.Decimal)
            If IsNumeric(galls) Then
                parGalls.Value = galls
            Else
                parGalls.Value = 0
            End If
            Command.Parameters.Add(parGalls)

            Dim parPrice As New SqlClient.SqlParameter("@Price", SqlDbType.Decimal)
            If IsNumeric(price) Then
                parPrice.Value = price
            Else
                parPrice.Value = 0
            End If
            Command.Parameters.Add(parPrice)

            Dim parAmt As New SqlClient.SqlParameter("@Amount", SqlDbType.Decimal)
            If IsNumeric(amt) Then
                parAmt.Value = amt
            Else
                parAmt.Value = 0
            End If
            Command.Parameters.Add(parAmt)


            Dim parOdometer As New SqlClient.SqlParameter("@LogOdometer", SqlDbType.Decimal)
            If IsNumeric(odometer) Then
                parOdometer.Value = odometer
            Else
                parOdometer.Value = 0
            End If
            Command.Parameters.Add(parOdometer)

            Dim parLat As New SqlClient.SqlParameter("@LogLatitude", SqlDbType.Decimal)
            If IsNumeric(lat) Then
                parLat.Value = lat
            Else
                parLat.Value = 0
            End If
            Command.Parameters.Add(parLat)

            Dim ParLng As New SqlClient.SqlParameter("@LogLongitude", SqlDbType.Decimal)
            If IsNumeric(lng) Then
                ParLng.Value = lng
            Else
                ParLng.Value = 0
            End If
            Command.Parameters.Add(ParLng)


            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "Quick Messages"

    Public Function getQuickMsgDriversList(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "QuickMessage_Drivers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting getQuickMsgDriversList"
            BLErrorHandling.ErrorCapture(pSysModule, "getQuickMsgDriversList", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function sendQuickMsg(ByVal token As String, ByVal driverId As Integer, ByVal channel As Integer, ByVal message As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "QuickMessage_SEND"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDriverId As New SqlClient.SqlParameter("@DriverID", SqlDbType.Int, 4)
            parDriverId.Value = driverId
            Command.Parameters.Add(parDriverId)

            Dim parChannel As New SqlClient.SqlParameter("@Channel", SqlDbType.Int, 4)
            parChannel.Value = channel
            Command.Parameters.Add(parChannel)

            Dim parMessage As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 500)
            parMessage.Value = message
            Command.Parameters.Add(parMessage)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CStr(parIsOk.Value)

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "DL.sendQuickMsg", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "OnBoarding"

    Function onboardingPendingCustomers(ByVal token As String) As List(Of pendingOnBoarding)
        Dim itm As pendingOnBoarding
        Dim lst As New List(Of pendingOnBoarding)

        Try
            strCommand = "CRM_OnBoarding_GetPending"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New pendingOnBoarding
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.email = reader.Item("Email")
                itm.phone = reader.Item("Phone")
                itm.billCity = reader.Item("BillCity")
                itm.billState = reader.Item("BillState")
                itm.shipCity = reader.Item("ShipCity")
                itm.shipState = reader.Item("ShipState")
                itm.createdOn = reader.Item("CreatedOn").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Function onboardingPendingCustomerGet(ByVal token As String, ByVal id As String) As onBoardingCustomer
        Dim itm As New onBoardingCustomer

        Try
            strCommand = "CRM_OnBoarding_GetCustomer"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 20)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New onBoardingCustomer
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.email = reader.Item("Email")
                itm.phone = reader.Item("Phone")
                itm.billCity = reader.Item("BillCity")
                itm.billState = reader.Item("BillState")
                itm.shipCity = reader.Item("ShipCity")
                itm.shipState = reader.Item("ShipState")
                itm.createdOn = reader.Item("CreatedOn").ToString
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Function onBoardingDone(ByVal token As String, ByVal id As String, ByRef msg As String) As Boolean
        Dim res As Boolean = True

        Try
            strCommand = "CRM_OnBoarding_CustomerDone"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 20)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res = CStr(parIsOk.Value)


        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "IfByPhone"

    Function smsReplyCatcher(ByVal msg As String) As Boolean
        Dim res As Boolean = True
        Dim dl As New DataLayer

        Try
            strCommand = "IfByPhone_SMSReplyCatcher_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.NVarChar, -1)
            parMsg.Value = msg
            Command.Parameters.Add(parMsg)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res = CStr(parIsOk.Value)

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Google Signature"

    Function GoogleSignature(url As String) As String
        Dim signature As String = ""

        Try
            Dim googleKey As String = ConfigurationManager.AppSettings("GoogleKey")

            Dim encoding As ASCIIEncoding = New ASCIIEncoding()

            'URL-safe decoding
            Dim privateKeyBytes As Byte() = Convert.FromBase64String(googleKey.Replace("-", "+").Replace("_", "/"))

            Dim objURI As Uri = New Uri(url)
            Dim encodedPathAndQueryBytes As Byte() = encoding.GetBytes(objURI.LocalPath & objURI.Query)

            'compute the hash
            Dim algorithm As HMACSHA1 = New HMACSHA1(privateKeyBytes)
            Dim hash As Byte() = algorithm.ComputeHash(encodedPathAndQueryBytes)

            'convert the bytes to string and make url-safe by replacing '+' and '/' characters
            signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_")

            'Add the signature to the existing URI.
            'Return objURI.Scheme & "://" & objURI.Host & objURI.LocalPath & objURI.Query & "&signature=" & signature


        Catch ex As Exception

        End Try

        Return signature

    End Function

#End Region

#Region "Page Engagement"

    Function pageEngagement(ByVal data As engagementTick) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Engagement_PageTicker"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parTransID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.NVarChar, 50)
            parTransID.Value = data.transId
            Command.Parameters.Add(parTransID)

            Dim parDelay As New SqlClient.SqlParameter("@Delay", SqlDbType.Int, 4)
            parDelay.Value = (data.delay / 1000)
            Command.Parameters.Add(parDelay)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Online users"

    Function getOnlineUsers(ByVal token As String) As realTimeActivity
        Dim rta As New realTimeActivity
        Dim lst As New List(Of onlineUser)
        Dim itm As onlineUser

        Try
            strCommand = "CRM_Engagement_OnlineUsers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New onlineUser
                itm.id = reader.Item("ID")
                itm.companyName = reader.Item("CompanyName")
                itm.userName = reader.Item("UserName")
                itm.phone = reader.Item("Phone")
                itm.mobile = reader.Item("CellPhone")
                itm.email = reader.Item("Email")
                itm.currentPage = reader.Item("CurrentPage")
                itm.currentPageTime = reader.Item("CurrentPageTime")
                itm.sessionTime = reader.Item("SessionTime").ToString
                itm.qtyUnits = reader.Item("QtyUnits")
                lst.Add(itm)
            Loop

            rta.onlineUsers = lst

            reader.NextResult()
            Do While reader.Read
                rta.qtyUnits = reader.Item("QtyUnits")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return rta

    End Function

#End Region

#Region "Reports"

    Function getTroubleshootingReport(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String) As List(Of troubleLog)
        Dim lst As New List(Of troubleLog)
        Dim itm As troubleLog
        Dim errMsg As String = ""

        Try
            strCommand = "Reports_Troubleshooting"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.Date)
            parDateFrom.Value = CDate(dateFrom)
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.Date)
            parDateTo.Value = CDate(dateTo)
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm = New troubleLog
                itm.deviceId = reader.Item("DeviceID")
                itm.deviceName = reader.Item("DeviceName")
                itm.serialNumber = reader.Item("SerialNumber")
                itm.lastUpdatedOn = reader.Item("LastUpdatedOn")
                itm.noShowDays = reader.Item("NoShowDays")
                itm.PowerCut = reader.Item("PowerCut")
                itm.MainPowerRestored = reader.Item("MainPowerRestored")
                itm.IllegalPowerUp = reader.Item("IllegalPowerUp")
                itm.PowerDown = reader.Item("PowerDown")
                itm.IgnOffGPS15 = reader.Item("IgnOffGPS15")
                itm.IgnOffSpeed10 = reader.Item("IgnOffSpeed10")
                itm.PowerUp = reader.Item("PowerUp")
                itm.PowerOffBatt = reader.Item("PowerOffBatt")
                itm.TotalEvents = reader.Item("TotalEvents")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

End Class
