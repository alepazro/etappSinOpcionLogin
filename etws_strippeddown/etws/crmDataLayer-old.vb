Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

Public Class crmDataLayerOld

#Region "Declaratives"

    Private pSysModule As String = "crmDataLayer.vb"

    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter

#End Region

    Public Function CRM_Users_ValidateCredentials(ByVal UserName As String, ByVal Password As String, ByRef msg As String) As String
        Dim strJson As String = ""

        Try
            msg = ""
            UserName = BLCommon.Sanitize(UserName)
            Password = BLCommon.Sanitize(Password)

            strCommand = "CRM_Users_ValidateCredentials"
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


            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read

                json.WritePropertyName("isValid")
                json.WriteValue(reader("IsValid"))

                json.WritePropertyName("tokenCookie")
                json.WriteValue(ConfigurationManager.AppSettings("crmTokenCookie"))

                json.WritePropertyName("token")
                json.WriteValue(reader("Token"))

                json.WritePropertyName("fullName")
                json.WriteValue(reader("FullName"))

                json.WritePropertyName("welcomeTitle")
                json.WriteValue(reader("WelcomeTitle"))

                json.WritePropertyName("isLimitedAccess")
                json.WriteValue(reader("isLimitedAccess"))

            Loop

            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            msg = ex.Message
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Users_ValidateCredentials", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Public Function CRM_Users_ValidateToken(ByVal Token As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_Users_ValidateToken"
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

            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read

                json.WritePropertyName("isValid")
                json.WriteValue(reader("IsValid"))

                json.WritePropertyName("tokenCookie")
                json.WriteValue(ConfigurationManager.AppSettings("crmTokenCookie"))

                json.WritePropertyName("token")
                json.WriteValue(reader("Token"))

                json.WritePropertyName("fullName")
                json.WriteValue(reader("FullName"))

                json.WritePropertyName("welcomeTitle")
                json.WriteValue(reader("WelcomeTitle"))

                json.WritePropertyName("isLimitedAccess")
                json.WriteValue(reader("isLimitedAccess"))

            Loop

            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Users_ValidateToken", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Public Function CRM_Customers_GET(ByVal token As String) As String
        Dim strJson As String = ""

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

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("customers")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("id"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("phone")
                jsonLine.WriteValue(reader("phone"))

                jsonLine.WritePropertyName("email")
                jsonLine.WriteValue(reader("email"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("createdOn").ToString)

                jsonLine.WritePropertyName("salesRepId")
                jsonLine.WriteValue(reader("salesRepId"))

                jsonLine.WritePropertyName("salesRep")
                jsonLine.WriteValue(reader("salesRepName"))

                jsonLine.WritePropertyName("isSuspended")
                jsonLine.WriteValue(reader("isSuspended"))

                jsonLine.WritePropertyName("suspendedOn")
                jsonLine.WriteValue(reader("suspendedOn").ToString)

                jsonLine.WritePropertyName("uniqueKey")
                jsonLine.WriteValue(reader("UniqueKey"))

                jsonLine.WritePropertyName("userLogin")
                jsonLine.WriteValue(reader("PrimaryUserLogin"))

                jsonLine.WritePropertyName("newCustomerCase")
                jsonLine.WriteValue(reader("newCustomerCase"))

                jsonLine.WritePropertyName("totalUnits")
                jsonLine.WriteValue(reader("totalUnits"))

                jsonLine.WritePropertyName("notInstalled")
                jsonLine.WriteValue(reader("notInstalled"))

                jsonLine.WritePropertyName("workingUnits")
                jsonLine.WriteValue(reader("workingUnits"))

                jsonLine.WritePropertyName("notWorkingUnits")
                jsonLine.WriteValue(reader("NotWorkingUnits"))

                jsonLine.WritePropertyName("usersList")
                jsonLine.WriteValue(reader("UsersList"))

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
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Users_ValidateToken", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Function CRM_Customers_GetByUniqueKey(ByVal token As String, ByVal uid As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_Customers_GetByUniqueKey"
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

            Dim parUID As New SqlClient.SqlParameter("@UniqueKey", SqlDbType.NVarChar, 50)
            parUID.Direction = ParameterDirection.Input
            parUID.Value = uid
            Command.Parameters.Add(parUID)

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
            json.WritePropertyName("company")
            json.WriteStartArray()

            'This Stored Procedure throws 2 result sets...
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("newCustomerCase")
                jsonLine.WriteValue(reader("newCustomerCase"))

                jsonLine.WritePropertyName("paymentMethod")
                jsonLine.WriteValue(reader("paymentMethod"))

                jsonLine.WritePropertyName("billingDay")
                jsonLine.WriteValue(reader("billingDay"))

                jsonLine.WritePropertyName("isVVIP")
                jsonLine.WriteValue(reader("isVVIP"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()

            'once done, jumps to the second result set
            reader.NextResult()

            json.WritePropertyName("users")
            json.WriteStartArray()

            'Here the reader reads the fist result set
            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("id"))

                jsonLine.WritePropertyName("guid")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("email")
                jsonLine.WriteValue(reader("Email"))

                jsonLine.WritePropertyName("phone")
                jsonLine.WriteValue(reader("Phone"))

                jsonLine.WritePropertyName("cellPhone")
                jsonLine.WriteValue(reader("CellPhone"))

                jsonLine.WritePropertyName("userName")
                jsonLine.WriteValue(reader("UserName"))

                jsonLine.WritePropertyName("lastLoginOn")
                jsonLine.WriteValue(reader("LastLoginOn"))

                jsonLine.WritePropertyName("qtyLogins")
                jsonLine.WriteValue(reader("QtyLogins"))

                jsonLine.WritePropertyName("timeZoneCode")
                jsonLine.WriteValue(reader("TimeZoneCode"))

                jsonLine.WritePropertyName("isDriver")
                jsonLine.WriteValue(reader("IsDriver"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn"))

                jsonLine.WritePropertyName("accessLevelID")
                jsonLine.WriteValue(reader("AccessLevelID"))

                jsonLine.WritePropertyName("isAdministrator")
                jsonLine.WriteValue(reader("IsAdministrator"))

                jsonLine.WritePropertyName("userLogin")
                jsonLine.WriteValue(reader("UserLogin"))

                jsonLine.WritePropertyName("newUserCase")
                jsonLine.WriteValue(reader("newUserCase"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()

            'once done, jumps to the second result set
            reader.NextResult()

            json.WritePropertyName("devices")
            json.WriteStartArray()

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("id"))

                jsonLine.WritePropertyName("guid")
                jsonLine.WriteValue(reader("GUID"))

                jsonLine.WritePropertyName("deviceType")
                jsonLine.WriteValue(reader("DeviceType"))

                jsonLine.WritePropertyName("deviceId")
                jsonLine.WriteValue(reader("DeviceID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("lastUpdatedOn")
                jsonLine.WriteValue(reader("LastUpdatedOn"))

                jsonLine.WritePropertyName("eventCode")
                jsonLine.WriteValue(reader("EventCode"))

                jsonLine.WritePropertyName("eventDate")
                jsonLine.WriteValue(reader("EventDate"))

                jsonLine.WritePropertyName("latitude")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("longitude")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WritePropertyName("speed")
                jsonLine.WriteValue(reader("Speed"))

                jsonLine.WritePropertyName("gpsStatus")
                jsonLine.WriteValue(reader("GPSStatus"))

                jsonLine.WritePropertyName("address")
                jsonLine.WriteValue(reader("FullAddress"))

                jsonLine.WritePropertyName("serialNumber")
                jsonLine.WriteValue(reader("SerialNumber"))

                jsonLine.WritePropertyName("carrier")
                jsonLine.WriteValue(reader("SimCarrier"))

                jsonLine.WritePropertyName("simNumber")
                jsonLine.WriteValue(reader("SimNumber"))

                jsonLine.WritePropertyName("simPhone")
                jsonLine.WriteValue(reader("SimPhone"))

                jsonLine.WritePropertyName("isInactive")
                jsonLine.WriteValue(reader("IsInactive"))

                jsonLine.WritePropertyName("isRMA")
                jsonLine.WriteValue(reader("IsRMA"))

                jsonLine.WritePropertyName("isNotWorking")
                jsonLine.WriteValue(reader("IsNotWorking"))

                jsonLine.WritePropertyName("note")
                jsonLine.WriteValue(reader("Note"))

                jsonLine.WritePropertyName("newDeviceCase")
                jsonLine.WriteValue(reader("newDeviceCase"))

                jsonLine.WritePropertyName("monthlyFee")
                jsonLine.WriteValue(reader("MonthlyFee"))

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
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Customers_GetByUniqueKey", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Function CRM_HDevices_GET(token As String, did As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_HDevices_GET"
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

            Dim parDID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parDID.Direction = ParameterDirection.Input
            parDID.Value = did
            Command.Parameters.Add(parDID)

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
            json.WritePropertyName("device")
            json.WriteStartArray()

            'This Stored Procedure throws 2 result sets...
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()

            'once done, jumps to the second result set
            reader.NextResult()

            json.WritePropertyName("dataUser")
            json.WriteStartArray()

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("id"))

                jsonLine.WritePropertyName("ignitionStatus")
                jsonLine.WriteValue(reader("IgnitionStatus"))

                jsonLine.WritePropertyName("eventCode")
                jsonLine.WriteValue(reader("EventCode"))

                jsonLine.WritePropertyName("eventName")
                jsonLine.WriteValue(reader("EventName"))

                jsonLine.WritePropertyName("eventDate")
                jsonLine.WriteValue(reader("EventDate"))

                jsonLine.WritePropertyName("speed")
                jsonLine.WriteValue(reader("Speed"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn"))

                jsonLine.WritePropertyName("gpsAge")
                jsonLine.WriteValue(reader("GPSAge"))

                jsonLine.WritePropertyName("consecutive")
                jsonLine.WriteValue(reader("Consecutive"))

                jsonLine.WritePropertyName("deviceTypeID")
                jsonLine.WriteValue(reader("DeviceTypeID"))

                jsonLine.WritePropertyName("isBrief")
                jsonLine.WriteValue(reader("IsBrief"))

                jsonLine.WritePropertyName("originalEvent")
                jsonLine.WriteValue(reader("OriginalEvent"))

                jsonLine.WritePropertyName("lat")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("lng")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WritePropertyName("extraData")
                jsonLine.WriteValue(reader("ExtraData"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())

            Loop

            json.WriteEnd()

            'once done, jumps to the second result set
            reader.NextResult()

            json.WritePropertyName("dataInternal")
            json.WriteStartArray()

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("id"))

                jsonLine.WritePropertyName("ignitionStatus")
                jsonLine.WriteValue(reader("IgnitionStatus"))

                jsonLine.WritePropertyName("eventCode")
                jsonLine.WriteValue(reader("EventCode"))

                jsonLine.WritePropertyName("eventName")
                jsonLine.WriteValue(reader("EventName"))

                jsonLine.WritePropertyName("eventDate")
                jsonLine.WriteValue(reader("EventDate"))

                jsonLine.WritePropertyName("speed")
                jsonLine.WriteValue(reader("Speed"))

                jsonLine.WritePropertyName("createdOn")
                jsonLine.WriteValue(reader("CreatedOn"))

                jsonLine.WritePropertyName("gpsAge")
                jsonLine.WriteValue(reader("GPSAge"))

                jsonLine.WritePropertyName("consecutive")
                jsonLine.WriteValue(reader("Consecutive"))

                jsonLine.WritePropertyName("deviceTypeID")
                jsonLine.WriteValue(reader("DeviceTypeID"))

                jsonLine.WritePropertyName("isBrief")
                jsonLine.WriteValue(reader("IsBrief"))

                jsonLine.WritePropertyName("originalEvent")
                jsonLine.WriteValue(reader("OriginalEvent"))

                jsonLine.WritePropertyName("lat")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("lng")
                jsonLine.WriteValue(reader("Longitude"))

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
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_HDevices_GET", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Function crmCustomerSaveBillingSpecs(ByVal token As String, ByVal uid As String, ByVal paymentMethod As Integer, ByVal billingDay As Integer, ByVal isVVIP As Boolean) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_Customers_BillingSpecs_SAVE"
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

            Dim parUID As New SqlClient.SqlParameter("@UniqueKey", SqlDbType.NVarChar, 50)
            parUID.Direction = ParameterDirection.Input
            parUID.Value = uid
            Command.Parameters.Add(parUID)

            Dim parPM As New SqlClient.SqlParameter("@PaymentMethod", SqlDbType.Int)
            parPM.Value = paymentMethod
            Command.Parameters.Add(parPM)

            Dim parBD As New SqlClient.SqlParameter("@BillingDay", SqlDbType.Int)
            parBD.Value = billingDay
            Command.Parameters.Add(parBD)

            Dim parIsVVIP As New SqlClient.SqlParameter("@IsVVIP", SqlDbType.Bit)
            parIsVVIP.Value = isVVIP
            Command.Parameters.Add(parIsVVIP)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmCustomerSaveBillingSpecs", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function
    Function validateToken(ByVal token As String) As crmEntities.user
        Dim itm As New crmEntities.user

        Try
            strCommand = "CRM_Users_ValidateToken"
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

            itm.isOk = False
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm.isOk = reader.Item("IsValid")
                itm.id = reader.Item("ID")
                itm.dealerId = reader.Item("DealerID")
                itm.tokenCookie = ConfigurationManager.AppSettings("crmTokenCookie")
                itm.token = reader.Item("Token")
                itm.firstName = reader.Item("FirstName")
                itm.fullName = reader.Item("FullName")
                itm.welcomeTitle = reader.Item("WelcomeTitle")
                itm.isLimitedAccess = reader.Item("IsLimitedAccess")
                itm.chatLicense = reader.Item("ChatLicense")
                itm.idDealers = reader.Item("idDealers")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            itm.isOk = False
            itm.msg = ex.Message
            BLErrorHandling.ErrorCapture(pSysModule, "crmDataLayer.validateToken", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return itm

    End Function
End Class
