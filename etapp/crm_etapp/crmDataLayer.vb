Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

Public Class crmDataLayer

#Region "Declaratives"

    Private pSysModule As String = "crmDataLayer.vb"

    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter

#End Region

#Region "New Methods"

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

    Function customersGET(ByVal token As String, ByVal search As String) As List(Of crmEntities.customer)
        Dim lst As New List(Of crmEntities.customer)
        Dim itm As crmEntities.customer

        Try
            strCommand = "CRM_Customers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSearch As New SqlClient.SqlParameter("@SearchTerm", SqlDbType.NVarChar, 50)
            parSearch.Value = search
            Command.Parameters.Add(parSearch)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm = New crmEntities.customer
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.phone = reader.Item("Phone")
                itm.email = reader.Item("Email")
                itm.createdOn = reader.Item("CreatedOn").ToString
                itm.salesRepId = reader.Item("SalesRepID")
                itm.salesRep = reader.Item("SalesRepName")
                itm.isSuspended = reader.Item("IsSuspended")
                itm.suspendedOn = reader.Item("SuspendedOn")
                itm.uniqueKey = reader.Item("UniqueKey")
                itm.userLogin = reader.Item("PrimaryUserLogin")
                itm.newCustomerCase = reader.Item("NewCustomerCase")
                itm.totalUnits = reader.Item("TotalUnits")
                itm.notInstalled = reader.Item("NotInstalled")
                itm.workingUnits = reader.Item("WorkingUnits")
                itm.notWorkingUnits = reader.Item("NotWorkingUnits")
                itm.usersList = reader.Item("UsersList")

                lst.Add(itm)

            Loop

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

        Return lst

    End Function

#End Region

#Region "Original Methods"

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

                json.WritePropertyName("idDealers")
                json.WriteValue(reader("idDealers"))

                json.WritePropertyName("idUser")
                json.WriteValue(reader("idUser"))

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

    Public Function companiesGetSimpleList(ByVal token As String) As DataView
        Dim dtData As New DataTable

        Try
            strCommand = "CRM_Companies_GetIdName"
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

        End Try

        Return dtData.DefaultView

    End Function

    Public Function devicesGetSimpleList(ByVal token As String, ByVal companyId As String) As DataView
        Dim dtData As New DataTable

        Try
            strCommand = "CRM_Devices_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCompanyId As New SqlClient.SqlParameter("@CompanyID", SqlDbType.NVarChar, 50)
            parCompanyId.Value = companyId
            Command.Parameters.Add(parCompanyId)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception

        End Try

        Return dtData.DefaultView

    End Function

    Public Function techniciansGetSimpleList(ByVal token As String) As DataView
        Dim dtData As New DataTable

        Try
            strCommand = "CRM_Technicians_GetIdName"
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

        End Try

        Return dtData.DefaultView

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

                jsonLine.WritePropertyName("credentialsReminder")
                jsonLine.WriteValue(reader("credentialsReminder"))

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

    Function sendCredentialsEmail(ByVal token As String, ByVal guid As String) As Boolean
        Try
            strCommand = "CRM_Customers_SendCredentialsEmail"
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

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Input
            parGUID.Value = guid
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "sendCredentialsEmail", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return True

    End Function

    Function CRM_HDevices_GET(token As String, did As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_HDevices_GET_NewCRM"
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

                jsonLine.WritePropertyName("DeviceType")
                jsonLine.WriteValue(reader("DeviceType"))

                jsonLine.WritePropertyName("DeviceID")
                jsonLine.WriteValue(reader("DeviceID"))

                jsonLine.WritePropertyName("Model")
                jsonLine.WriteValue(reader("Model"))

                jsonLine.WritePropertyName("Name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WritePropertyName("SimNoDB")
                jsonLine.WriteValue(reader("SimNoDB"))


                jsonLine.WritePropertyName("SimNoUnit")
                jsonLine.WriteValue(reader("SimNoUnit"))

                jsonLine.WritePropertyName("ReportIgnON")
                jsonLine.WriteValue(reader("ReportIgnON"))

                jsonLine.WritePropertyName("ReportTimerIgnOff")
                jsonLine.WriteValue(reader("ReportTimerIgnOff"))

                jsonLine.WritePropertyName("ReportTurnAngle")
                jsonLine.WriteValue(reader("ReportTurnAngle"))

                jsonLine.WritePropertyName("ReportDistance")
                jsonLine.WriteValue(reader("ReportDistance"))

                jsonLine.WritePropertyName("FakeIgn")
                jsonLine.WriteValue(reader("FakeIgn"))

                jsonLine.WritePropertyName("IgnON")
                jsonLine.WriteValue(reader("IgnON"))

                jsonLine.WritePropertyName("IgnOFF")
                jsonLine.WriteValue(reader("IgnOFF"))


                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()

            'once done, jumps to the second result set
            reader.NextResult()

            json.WritePropertyName("devicedata")
            json.WriteStartArray()

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("ID")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("IgnitionStatus")
                jsonLine.WriteValue(reader("IgnitionStatus"))

                jsonLine.WritePropertyName("EventCode")
                jsonLine.WriteValue(reader("EventCode"))

                jsonLine.WritePropertyName("EventName")
                jsonLine.WriteValue(reader("EventName"))

                jsonLine.WritePropertyName("EventDate")
                jsonLine.WriteValue(reader("EventDate"))

                jsonLine.WritePropertyName("Speed")
                jsonLine.WriteValue(reader("Speed"))

                jsonLine.WritePropertyName("CreatedOn")
                jsonLine.WriteValue(reader("CreatedOn"))

                jsonLine.WritePropertyName("GPSAge")
                jsonLine.WriteValue(reader("GPSAge"))

                jsonLine.WritePropertyName("GPSCount")
                jsonLine.WriteValue(reader("GPSCount"))

                jsonLine.WritePropertyName("Consecutive")
                jsonLine.WriteValue(reader("Consecutive"))

                jsonLine.WritePropertyName("DeviceTypeID")
                jsonLine.WriteValue(reader("DeviceTypeID"))

                jsonLine.WritePropertyName("IsBrief")
                jsonLine.WriteValue(reader("IsBrief"))

                jsonLine.WritePropertyName("OriginalEvent")
                jsonLine.WriteValue(reader("OriginalEvent"))

                jsonLine.WritePropertyName("Latitude")
                jsonLine.WriteValue(reader("Latitude"))

                jsonLine.WritePropertyName("Longitude")
                jsonLine.WriteValue(reader("Longitude"))

                jsonLine.WritePropertyName("RSSI")
                jsonLine.WriteValue(reader("RSSI"))

                jsonLine.WritePropertyName("Ble")
                jsonLine.WriteValue(reader("Ble"))

                jsonLine.WritePropertyName("BI")
                jsonLine.WriteValue(reader("BI"))

                jsonLine.WritePropertyName("BE")
                jsonLine.WriteValue(reader("BE"))

                jsonLine.WritePropertyName("MsgDelay")
                jsonLine.WriteValue(reader("MsgDelay"))

                jsonLine.WritePropertyName("Temperature1")
                jsonLine.WriteValue(reader("Temperature1"))

                jsonLine.WritePropertyName("Temperature2")
                jsonLine.WriteValue(reader("Temperature2"))

                jsonLine.WritePropertyName("Temperature3")
                jsonLine.WriteValue(reader("Temperature3"))

                jsonLine.WritePropertyName("Temperature4")
                jsonLine.WriteValue(reader("Temperature4"))

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
                jsonLine.WritePropertyName("MessageType")
                jsonLine.WriteValue(reader("MessageType"))

                jsonLine.WritePropertyName("MessageDevice")
                jsonLine.WriteValue(reader("MessageDevice"))

                jsonLine.WritePropertyName("IP")
                jsonLine.WriteValue(reader("IP"))

                jsonLine.WritePropertyName("Port")
                jsonLine.WriteValue(reader("Port"))

                jsonLine.WritePropertyName("CreatedOn")
                jsonLine.WriteValue(reader("CreatedOn"))

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

    Function crmCustomerSnoozeCollections(ByVal token As String, ByVal uid As String) As String
        Dim result As String = ""

        Try
            strCommand = "CRM_Customers_SnoozeCollections"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parUID As New SqlClient.SqlParameter("@UniqueKey", SqlDbType.NVarChar, 50)
            parUID.Value = uid
            Command.Parameters.Add(parUID)

            Dim parRes As New SqlClient.SqlParameter("@Result", SqlDbType.NVarChar, 50)
            parRes.Direction = ParameterDirection.Output
            Command.Parameters.Add(parRes)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            result = parRes.Value

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmCustomerSnoozeCollections", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return result

    End Function

    Function CRM_Cases_GetBasicTables() As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "CRM_Cases_GetBasicTables"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            adapter = New SqlDataAdapter(Command)
            adapter.TableMappings.Add("Table", "CasesCategories")
            adapter.TableMappings.Add("Table1", "CasesTypes")
            adapter.TableMappings.Add("Table2", "CasesSubTypes")
            adapter.TableMappings.Add("Table3", "CasesActivityTypes")
            adapter.TableMappings.Add("Table4", "CasesStatus")
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception

        End Try

        Return dsData

    End Function

    Function CRM_Reports_GetBasicTables() As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "CRM_Reports_GetBasicTables"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            adapter = New SqlDataAdapter(Command)
            adapter.TableMappings.Add("Table", "CRMReports")
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception

        End Try

        Return dsData

    End Function

    Function CRM_GetCRMReport(ByVal reportGUID As String, ByVal param1 As String, ByVal userToken As String) As DataTable
        Dim dt As New DataTable

        Try
            strCommand = "CRM_Reports_GetReport"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parGUID As New SqlClient.SqlParameter("@ReportGUID", SqlDbType.NVarChar, 50)
            parGUID.Value = reportGUID
            Command.Parameters.Add(parGUID)

            Dim parParam1 As New SqlClient.SqlParameter("@Param1", SqlDbType.NVarChar, 50)
            parParam1.Value = param1
            Command.Parameters.Add(parParam1)

            Dim parUserToken As New SqlClient.SqlParameter("@UserToken", SqlDbType.NVarChar, 50)
            parUserToken.Value = userToken
            Command.Parameters.Add(parUserToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dt)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception

        End Try

        Return dt

    End Function

    Function CRM_Cases_GET(ByVal token As String, ByVal caseId As String, ByVal companyId As String, ByVal categoryId As String, ByVal assignedToId As String, ByVal onlyMine As Boolean, ByVal onlyOpen As Boolean) As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "CRM_Cases_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parId As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parId.Value = caseId
            Command.Parameters.Add(parId)

            Dim parCompanyId As New SqlClient.SqlParameter("@CompanyId", SqlDbType.NVarChar, 50)
            parCompanyId.Value = companyId
            Command.Parameters.Add(parCompanyId)

            Dim parCategoryID As New SqlClient.SqlParameter("@CategoryId", SqlDbType.NVarChar, 50)
            parCategoryID.Value = categoryId
            Command.Parameters.Add(parCategoryID)

            Dim parAssignedTo As New SqlClient.SqlParameter("@AssignedToID", SqlDbType.NVarChar, 50)
            parAssignedTo.Value = assignedToId
            Command.Parameters.Add(parAssignedTo)

            Dim parOnlyMine As New SqlClient.SqlParameter("@OnlyMine", SqlDbType.Bit)
            parOnlyMine.Value = onlyMine
            Command.Parameters.Add(parOnlyMine)

            Dim parOnlyOpen As New SqlClient.SqlParameter("@OnlyOpen", SqlDbType.Bit)
            parOnlyOpen.Value = onlyOpen
            Command.Parameters.Add(parOnlyOpen)

            adapter = New SqlDataAdapter(Command)
            adapter.TableMappings.Add("Table", "Cases")

            If caseId.Length > 0 Then
                adapter.TableMappings.Add("Table1", "Activities")
            End If

            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception

        End Try

        Return dsData

    End Function

    Function CRM_Case_SAVE(ByVal token As String, ByVal data As caseClass, ByRef caseId As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "CRM_Cases_SaveCase"
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
            parID.Value = data.id
            Command.Parameters.Add(parID)

            Dim parCompanyID As New SqlClient.SqlParameter("@CompanyID", SqlDbType.NVarChar, 50)
            parCompanyID.Value = data.companyId
            Command.Parameters.Add(parCompanyID)

            Dim parCategoryID As New SqlClient.SqlParameter("@CategoryID", SqlDbType.NVarChar, 50)
            parCategoryID.Value = data.categoryId
            Command.Parameters.Add(parCategoryID)

            Dim parTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.NVarChar, 50)
            parTypeID.Value = data.typeId
            Command.Parameters.Add(parTypeID)

            Dim parSubTypeID As New SqlClient.SqlParameter("@SubTypeID", SqlDbType.NVarChar, 50)
            If Not IsNothing(data.subTypeId) Then
                parSubTypeID.Value = data.subTypeId
            Else
                parSubTypeID.Value = ""
            End If
            Command.Parameters.Add(parSubTypeID)

            Dim parAssignedToID As New SqlClient.SqlParameter("@AssignedToID", SqlDbType.NVarChar, 50)
            If Not IsNothing(data.assignedToId) Then
                parAssignedToID.Value = data.assignedToId
            Else
                parAssignedToID.Value = ""
            End If
            Command.Parameters.Add(parAssignedToID)

            Dim parSubject As New SqlClient.SqlParameter("@Subject", SqlDbType.NVarChar, -1)
            parSubject.Value = data.subject
            Command.Parameters.Add(parSubject)

            Dim parNotes As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, -1)
            'We just process the Notes via this function on the INSERT.  From that point on, notes follow a different path via the ADDNOTE action (see ChangeStatus).
            If data.id.Length = 0 Then
                parNotes.Value = data.notes
                Command.Parameters.Add(parNotes)
            End If

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parNewId As New SqlClient.SqlParameter("@NewID", SqlDbType.NVarChar, 50)
            parNewId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            caseId = CStr(parNewId.Value)

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

    Function CRM_Case_ChangeStatus(ByVal token As String, ByVal data As changeStatus, ByRef newStatusId As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "CRM_Cases_ChangeStatus"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCaseID As New SqlClient.SqlParameter("@CaseID", SqlDbType.NVarChar, 50)
            parCaseID.Value = data.caseId
            Command.Parameters.Add(parCaseID)

            Dim parAction As New SqlClient.SqlParameter("@Action", SqlDbType.NVarChar, 20)
            parAction.Value = data.action
            Command.Parameters.Add(parAction)

            Dim parParam1 As New SqlClient.SqlParameter("@Param1", SqlDbType.NVarChar, -1)
            parParam1.Value = data.param1
            Command.Parameters.Add(parParam1)

            Dim parParam2 As New SqlClient.SqlParameter("@Param2", SqlDbType.NVarChar, -1)
            parParam2.Value = data.param2
            Command.Parameters.Add(parParam2)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parNewStatusId As New SqlClient.SqlParameter("@NewStatusID", SqlDbType.NVarChar, 50)
            parNewStatusId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewStatusId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            newStatusId = CStr(parNewStatusId.Value)

        Catch ex As Exception
            bResult = False
            newStatusId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return bResult

    End Function
    Function CRM_HDevices_GetSendSMS(token As String, did As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_HDevices_GetSendSMS"
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
            'json.WritePropertyName("device")
            json.WritePropertyName("datasendSMS")
            json.WriteStartArray()

            'This Stored Procedure throws 2 result sets...
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("DeviceID")
                jsonLine.WriteValue(reader("DeviceID"))

                jsonLine.WritePropertyName("PhoneNumber")
                jsonLine.WriteValue(reader("PhoneNumber"))

                jsonLine.WritePropertyName("Destination")
                jsonLine.WriteValue(reader("Destination"))

                jsonLine.WritePropertyName("SMS")
                jsonLine.WriteValue(reader("SMS"))

                jsonLine.WritePropertyName("IsProcessed")
                jsonLine.WriteValue(reader("IsProcessed"))

                jsonLine.WritePropertyName("ProcessedOn")
                jsonLine.WriteValue(reader("ProcessedOn"))

                jsonLine.WritePropertyName("SMSCMessage")
                jsonLine.WriteValue(reader("SMSCMessage"))

                jsonLine.WritePropertyName("MessageID")
                jsonLine.WriteValue(reader("MessageID"))

                jsonLine.WritePropertyName("CreatedOn")
                jsonLine.WriteValue(reader("CreatedOn"))

                jsonLine.WritePropertyName("SimCarrier")
                jsonLine.WriteValue(reader("SimCarrier"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()

            'once done, jumps to the second result set
            reader.NextResult()

            json.WritePropertyName("dataResponseSMS")
            json.WriteStartArray()

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("DeviceID")
                jsonLine.WriteValue(reader("DeviceID"))

                jsonLine.WritePropertyName("Response")
                jsonLine.WriteValue(reader("Response"))

                jsonLine.WritePropertyName("AreaCode")
                jsonLine.WriteValue(reader("AreaCode"))

                jsonLine.WritePropertyName("PhoneNumber")
                jsonLine.WriteValue(reader("PhoneNumber"))

                jsonLine.WritePropertyName("CreatedOn")
                jsonLine.WriteValue(reader("CreatedOn"))

                jsonLine.WritePropertyName("IsProcessed")
                jsonLine.WriteValue(reader("IsProcessed"))

                jsonLine.WritePropertyName("ProcessedOn")
                jsonLine.WriteValue(reader("ProcessedOn"))

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
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_HDevices_GetSendSMS", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function
    Function CRM_MoveUnit_ToAotherDealer(ByVal token As String, ByVal uid As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "CRM_MoveUnit_ToAotherDealer"
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

                jsonLine.WritePropertyName("credentialsReminder")
                jsonLine.WriteValue(reader("credentialsReminder"))

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

    Function CRM_GetCrmDealers(ByVal token As String) As DataTable
        Dim dt As New DataTable

        Try
            strCommand = "CRM_Dealers_GET"
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
            adapter.Fill(dt)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Dealers_GET", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return dt

    End Function

    Function CRM_GetCrmDevices(ByVal token As String, ByVal devicesId As String) As String
        Dim strJson As String = ""
        Try
            strCommand = "CRM_Devices_GET_NewCRM"
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

            Dim parDevicesId As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 50)
            parDevicesId.Direction = ParameterDirection.Input
            parDevicesId.Value = devicesId
            Command.Parameters.Add(parDevicesId)

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
            json.WritePropertyName("Devices")
            json.WriteStartArray()

            'This Stored Procedure throws 2 result sets...
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("ID")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("DeviceID")
                jsonLine.WriteValue(reader("DeviceID"))

                jsonLine.WritePropertyName("NameDevices")
                jsonLine.WriteValue(reader("NameDevices"))

                jsonLine.WritePropertyName("idCompanies")
                jsonLine.WriteValue(reader("idCompanies"))

                jsonLine.WritePropertyName("company")
                jsonLine.WriteValue(reader("company"))

                jsonLine.WritePropertyName("idDealers")
                jsonLine.WriteValue(reader("idDealers"))

                jsonLine.WritePropertyName("Dealers")
                jsonLine.WriteValue(reader("Dealers"))


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
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Devices_GET_NewCRM", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

    Function CRM_UpdateDealerDevices(ByVal token As String, ByVal idDevices As String, ByVal idDealer As String) As String
        Dim strJson As String = ""
        Try
            strCommand = "CRM_UpdateDealersDevices_NEWCRM"
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

            Dim parDevicesId As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 50)
            parDevicesId.Direction = ParameterDirection.Input
            parDevicesId.Value = idDevices
            Command.Parameters.Add(parDevicesId)

            Dim paridDealer As New SqlClient.SqlParameter("@DealerId", SqlDbType.NVarChar, 50)
            paridDealer.Direction = ParameterDirection.Input
            paridDealer.Value = idDealer
            Command.Parameters.Add(paridDealer)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'This Stored Procedure throws 2 result sets...
            Command.ExecuteReader()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Devices_GET_NewCRM", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function
    Function CRM_Get_Company(ByVal token As String, ByVal search As String) As String
        Dim strJson As String = ""
        Try
            strCommand = "CRM_Get_Company"
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

            Dim parSearch As New SqlClient.SqlParameter("@Search", SqlDbType.NVarChar, 50)
            parSearch.Direction = ParameterDirection.Input
            parSearch.Value = search
            Command.Parameters.Add(parSearch)


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
            json.WritePropertyName("Companys")
            json.WriteStartArray()

            'This Stored Procedure throws 2 result sets...
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("idCompany")
                jsonLine.WriteValue(reader("idCompany"))

                jsonLine.WritePropertyName("idDealers")
                jsonLine.WriteValue(reader("idDealers"))

                jsonLine.WritePropertyName("nameDealers")
                jsonLine.WriteValue(reader("nameDealers"))

                jsonLine.WritePropertyName("nameCompany")
                jsonLine.WriteValue(reader("nameCompany"))

                jsonLine.WritePropertyName("Phone")
                jsonLine.WriteValue(reader("Phone"))

                jsonLine.WritePropertyName("Email")
                jsonLine.WriteValue(reader("Email"))

                jsonLine.WritePropertyName("CreatedOn")
                jsonLine.WriteValue(reader("CreatedOn"))


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
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_Get_Company", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function
    Function CRM_updateMoveCompany(ByVal token As String, ByVal TargetDealerID As String, ByVal CompanyID As String) As String
        Dim strJson As String = ""
        Try
            strCommand = "CRM_MoveCompany"
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

            Dim paridDealer As New SqlClient.SqlParameter("@TargetDealerID", SqlDbType.NVarChar, 50)
            paridDealer.Direction = ParameterDirection.Input
            paridDealer.Value = TargetDealerID
            Command.Parameters.Add(paridDealer)

            Dim parCompanyId As New SqlClient.SqlParameter("@CompanyID", SqlDbType.NVarChar, 50)
            parCompanyId.Direction = ParameterDirection.Input
            parCompanyId.Value = CompanyID
            Command.Parameters.Add(parCompanyId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'This Stored Procedure throws 2 result sets...
            Command.ExecuteReader()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_MoveCompany", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function
    Public Function getdevicesAccelerometer(ByVal ptoken As String, ByVal pcompany As Integer) As List(Of DevicesAccelerometer)
        Dim res As New List(Of DevicesAccelerometer)
        Dim accelerometer
        Dim reader As SqlDataReader = Nothing
        'pcompany = 1770
        Try
            strCommand = "CRM_Devices_Accelerometer_Set"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim token As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 100) With {
                .Value = ptoken
            }
            Command.Parameters.Add(token)

            Dim CompanyID As New SqlClient.SqlParameter("@CompanyID", SqlDbType.Int) With {
                .Value = pcompany
            }
            Command.Parameters.Add(CompanyID)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 1
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    accelerometer = New DevicesAccelerometer With {
                        .ID = reader.Item("ID"),
                        .DeviceID = reader.Item("DeviceID"),
                        .deviceName = reader.Item("deviceName"),
                        .CompanyID = reader.Item("CompanyID"),
                        .CompanyName = reader.Item("CompanyName"),
                        .currentConfiguration = reader.Item("currentConfiguration"),
                        .VehicleType = reader.Item("VehicleType")
                    }
                    res.Add(accelerometer)
                Loop
            Else
                res = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            res = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return res
    End Function
    Public Function getDevices_AccelConfiguration(ByVal ptoken As String) As List(Of Devices_AccelConfiguration)
        Dim res As New List(Of Devices_AccelConfiguration)
        Dim accelerometer
        Dim reader As SqlDataReader = Nothing
        'pcompany = 1770
        Try
            strCommand = "CRM_Devices_Accelerometer_Set"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim token As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 100) With {
                .Value = ptoken
            }
            Command.Parameters.Add(token)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 2
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    accelerometer = New Devices_AccelConfiguration With {
                        .ID = reader.Item("ID"),
                        .VehicleType = reader.Item("VehicleType"),
                        .SMS = reader.Item("SMS"),
                        .Network = reader.Item("Network")
                    }
                    res.Add(accelerometer)
                Loop
            Else
                res = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            res = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return res
    End Function
    Public Function putDevices_AccelConfiguration(ByVal Token As String, ByVal deviceid As Integer, ByVal pcomandid As Integer, ByVal paction As Integer) As Integer
        Dim res As Integer = 0
        Try
            strCommand = "CRM_Devices_Accelerometer_Set"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 255)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)


            Dim Device As New SqlClient.SqlParameter("@DeviceID", SqlDbType.Int)
            Device.Direction = ParameterDirection.Input
            Device.Value = deviceid
            Command.Parameters.Add(Device)

            Dim comandid As New SqlClient.SqlParameter("@CommandID", SqlDbType.Int)
            comandid.Direction = ParameterDirection.Input
            comandid.Value = pcomandid
            Command.Parameters.Add(comandid)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Direction = ParameterDirection.Input
            Action.Value = paction
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOK", SqlDbType.TinyInt)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            res = parIsOk.Value

            'res = CStr(parIsOk.Value)
        Catch ex As Exception
            Console.WriteLine(ex.Message)


        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

End Class
