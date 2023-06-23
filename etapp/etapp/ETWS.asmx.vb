Imports System.Data
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.IO
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter
Imports System.Net.WebRequest
Imports System.Net.HttpWebRequest
Imports System.Net
Imports etapp.BLCommon

<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="https://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class ETWS
    Inherits System.Web.Services.WebService

    Private pSysModule As String = "ETWS.asmx.vb"

#Region "Public Methods"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function ValidateCredentials() As String
        Dim BL As New DataLayer
        Dim strResult As String = ""
        Dim login As String = ""
        Dim pw As String = ""
        Dim language As String = ""
        Dim rememberMe As Boolean = False
        Dim newApp As Boolean = False
        Dim msg As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("login")) Then
                login = HttpContext.Current.Request.Form("login")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("pw")) Then
                pw = HttpContext.Current.Request.Form("pw")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("rememberMe")) Then
                rememberMe = CBool(HttpContext.Current.Request.Form("rememberMe"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("language")) Then
                language = HttpContext.Current.Request.Form("language")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("newApp")) Then
                newApp = HttpContext.Current.Request.Form("newApp")
            End If

            Dim dvData As DataView = Nothing
            Try
                If IsNothing(BL) Then
                    Throw New Exception("BL is nothing")
                End If
                dvData = BL.CredentialsVerification(login, pw, rememberMe, msg, newApp)
            Catch ex As Exception
                BLErrorHandling.ErrorCapture(pSysModule, "ValidateCredentials", "Login: " & login & " - PW: " & pw, "Error calling CredentialsVerification: " & ex.Message & " - login: " & login, 0)
            End Try

            If Not IsNothing(dvData) Then
                If dvData.Count > 0 Then
                    strResult = LoadJsonCredentials(dvData)
                Else
                    BLErrorHandling.ErrorCapture(pSysModule, "ValidateCredentials", "Login: " & login & " - PW: " & pw, "Wrong Credentials" & " - login: " & login, 0)
                    strResult = "false"
                    msg = "Invalid Credentials.  Please try again OR click in 'Resend me my credentials' to recover your credentials."
                    strResult = LoadJsonError(msg)
                End If
            Else
                BLErrorHandling.ErrorCapture(pSysModule, "ValidateCredentials", "Login: " & login & " - PW: " & pw, "Wrong Credentials" & " - login: " & login, 0)
                strResult = "false"
                msg = "Invalid Credentials.  Please try again OR click in 'Resend me my credentials' to recover your credentials."
                strResult = LoadJsonError(msg)
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "ValidateCredentials", "Login: " & login & " - PW: " & pw, ex.Message & " - login: " & login, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function validateToken() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim sourcePage As String = ""
        Dim sourceId As String = ""
        Dim BL As New DataLayer
        Dim clientIP As String = ""

        Try
            Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
            clientIP = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(clientIP) Then
                clientIP = context.Request.ServerVariables("REMOTE_ADDR")
            End If
        Catch ex As Exception

        End Try

        Try
            If String.IsNullOrEmpty(clientIP) Then
                clientIP = HttpContext.Current.Request.ServerVariables(32)
            End If
        Catch ex As Exception
            clientIP = ""
        End Try

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sourcePage")) Then
                sourcePage = HttpContext.Current.Request.Form("sourcePage")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sourceId")) Then
                sourceId = HttpContext.Current.Request.Form("sourceId")
            End If

            Dim sourceExt As String = ""
            sourceExt = clientIP

            Dim dvData As DataView
            If IsGUID(token) Then
                dvData = BL.ValidateToken(token, sourcePage, sourceId, sourceExt)
            Else
                dvData = Nothing
            End If

            If Not IsNothing(dvData) Then
                strResult = LoadJsonCredentials(dvData)
            Else
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "validateToken", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getTokenFromUserGUID() As String
        Dim strResult As String = ""
        Dim userGUID As String = ""
        Dim BL As New DataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("userGUID")) Then
                userGUID = HttpContext.Current.Request.Form("userGUID")
            End If

            If IsGUID(userGUID) Then
                strResult = BL.getTokenFromUserGUID(userGUID)
            End If

            If strResult.Length > 0 Then
                strResult = LoadJsonToken(strResult)
            Else
                strResult = LoadJsonError("Could not get access to this account..")
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "getTokenFromUserGUID", "", ex.Message & " - userGUID: " & userGUID, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDevices() As String
        Dim token As String = ""
        Dim dsData As DataSet
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""
        Dim lastFetchOn As Date = Date.Parse("1/1/1900")
        Dim callSource As Integer = 0
        Dim groupId As String = ""
        Dim search As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lastRefreshOn")) Then
                If IsDate(HttpContext.Current.Request.Form("lastRefreshOn")) Then
                    lastFetchOn = CDate(HttpContext.Current.Request.Form("lastRefreshOn"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("source")) Then
                If IsNumeric(HttpContext.Current.Request.Form("source")) Then
                    callSource = CInt(HttpContext.Current.Request.Form("source"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("groupId")) Then
                groupId = HttpContext.Current.Request.Form("groupId")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("search")) Then
                search = HttpContext.Current.Request.Form("search")
            End If


            If callSource = 3 Then
                lastFetchOn = Date.Parse("1/1/1900")
            End If

            If IsGUID(token) Then
                dsData = DL.getDevices(token, groupId, lastFetchOn, strError)
            Else
                dsData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dsData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                json.WritePropertyName("envelope")
                json.WriteStartArray()
                For Each drv In dsData.Tables(0).DefaultView
                    json.WriteValue(LoadJsonEnvelope(drv))
                Next
                json.WriteEnd()

                'List of units
                json.WritePropertyName("myDevices")
                json.WriteStartArray()

                For Each drv In dsData.Tables(1).DefaultView
                    json.WriteValue(LoadJsonMyDevices(drv))
                Next

                json.WriteEnd()

                'json.WritePropertyName("groups")
                'json.WriteStartArray()
                'For Each drv In dsData.Tables(2).DefaultView
                '    json.WriteValue(LoadJsonGroups(drv))
                'Next
                'json.WriteEnd()

                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getDevices", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDevicesGroupsNew() As String
        Dim token As String = ""
        Dim dsData As DataSet
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""
        Dim lastFetchOn As Date = Date.Parse("1/1/1900")
        Dim callSource As Integer = 0
        Dim groupId As String = ""
        Dim search As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lastRefreshOn")) Then
                If IsDate(HttpContext.Current.Request.Form("lastRefreshOn")) Then
                    lastFetchOn = CDate(HttpContext.Current.Request.Form("lastRefreshOn"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("source")) Then
                If IsNumeric(HttpContext.Current.Request.Form("source")) Then
                    callSource = CInt(HttpContext.Current.Request.Form("source"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("groupId")) Then
                groupId = HttpContext.Current.Request.Form("groupId")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("search")) Then
                search = HttpContext.Current.Request.Form("search")
            End If


            If callSource = 3 Then
                lastFetchOn = Date.Parse("1/1/1900")
            End If

            If IsGUID(token) Then
                dsData = DL.getDevicesGroups(token, groupId, lastFetchOn, strError)
            Else
                dsData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dsData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                json.WritePropertyName("envelope")
                json.WriteStartArray()
                For Each drv In dsData.Tables(0).DefaultView
                    json.WriteValue(LoadJsonEnvelope(drv))
                Next
                json.WriteEnd()

                'List of units
                json.WritePropertyName("myDevices")
                json.WriteStartArray()

                For Each drv In dsData.Tables(1).DefaultView
                    json.WriteValue(LoadJsonMyDevicesGroupNew(drv))
                Next

                json.WriteEnd()

                json.WritePropertyName("groups")
                json.WriteStartArray()
                For Each drv In dsData.Tables(2).DefaultView
                    json.WriteValue(LoadJsonGroups(drv))
                Next
                json.WriteEnd()

                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getDevicesGroupsNew", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    '<WebMethod()>
    '<ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDeviceBySearchText() As String
        Dim token As String = ""
        Dim dsData As DataSet
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""
        Dim lastFetchOn As Date = Date.Parse("1/1/1900")
        Dim callSource As Integer = 0
        Dim groupId As String = ""
        Dim searchText As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lastRefreshOn")) Then
                If IsDate(HttpContext.Current.Request.Form("lastRefreshOn")) Then
                    lastFetchOn = CDate(HttpContext.Current.Request.Form("lastRefreshOn"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("source")) Then
                If IsNumeric(HttpContext.Current.Request.Form("source")) Then
                    callSource = CInt(HttpContext.Current.Request.Form("source"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("groupId")) Then
                groupId = HttpContext.Current.Request.Form("groupId")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("searchText")) Then
                searchText = HttpContext.Current.Request.Form("searchText")
            End If


            If callSource = 3 Then
                lastFetchOn = Date.Parse("1/1/1900")
            End If

            If IsGUID(token) Then
                dsData = DL.GetDeviceBySearchText(token, groupId, lastFetchOn, strError, searchText)
            Else
                dsData = Nothing
                strError = "Invalid Token"
            End If
            If Not IsNothing(dsData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                json.WritePropertyName("envelope")
                json.WriteStartArray()
                For Each drv In dsData.Tables(0).DefaultView
                    json.WriteValue(LoadJsonEnvelope(drv))
                Next
                json.WriteEnd()

                'List of units
                json.WritePropertyName("myDevices")
                json.WriteStartArray()

                For Each drv In dsData.Tables(1).DefaultView
                    json.WriteValue(LoadJsonMyDevicesGroupNew(drv))
                Next

                json.WriteEnd()

                json.WritePropertyName("groups")
                json.WriteStartArray()
                For Each drv In dsData.Tables(2).DefaultView
                    json.WriteValue(LoadJsonGroups(drv))
                Next
                json.WriteEnd()

                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If
        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getDevices", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveWorkOrder() As String
        Dim DL As New DataLayer
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim dd As String = ""
        Dim name As String = ""
        Dim phone As String = ""
        Dim street As String = ""
        Dim city As String = ""
        Dim state As String = ""
        Dim postalCode As String = ""
        Dim jobDescription As String = ""
        Dim bResults As Boolean = False
        Dim strResult As String = ""
        Dim strError As String = ""
        Dim strSMS As String = ""
        Dim jobGUID As String = ""
        Dim strAddress As String = ""
        Dim isGeofence As String = ""
        Dim geofenceGUID As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim sendSMS As Boolean = False
        Dim driverId As Integer = 0
        Dim jobMap As String = ""
        Dim via As Integer = 0

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isGeofence")) Then
                If HttpContext.Current.Request.Form("isGeofence").ToLower = "true" Then
                    isGeofence = True
                Else
                    isGeofence = False
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("geofenceId")) Then
                geofenceGUID = CStr(HttpContext.Current.Request.Form("geofenceId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("phone")) Then
                phone = HttpContext.Current.Request.Form("phone")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("postalCode")) Then
                postalCode = HttpContext.Current.Request.Form("postalCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("description")) Then
                jobDescription = HttpContext.Current.Request.Form("description")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lat")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lat")) Then
                    lat = CDec(HttpContext.Current.Request.Form("lat"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lng")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lng")) Then
                    lng = CDec(HttpContext.Current.Request.Form("lng"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sendSMS")) Then
                sendSMS = HttpContext.Current.Request.Form("sendSMS")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("driverId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("driverId")) Then
                    driverId = CInt(HttpContext.Current.Request.Form("driverId"))
                Else
                    driverId = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("via")) Then
                If IsNumeric(HttpContext.Current.Request.Form("via")) Then
                    via = CInt(HttpContext.Current.Request.Form("via"))
                Else
                    via = 0
                End If
            End If

            'Build the address
            Dim jobAddress As String = ""
            strAddress = ""
            If street.Length > 0 Then
                strAddress = street
            End If
            If city.Length > 0 Then
                If strAddress.Length > 0 Then
                    strAddress &= " "
                End If
                strAddress &= city
            End If
            If postalCode.Length > 0 Then
                If strAddress.Length > 0 Then
                    strAddress &= " "
                End If
                strAddress &= postalCode
            End If

            If via = 2 Then
                'If via = email, build the email
                'Buid the SMS Message
                strSMS = "JOB:" & name & vbCrLf

                If jobDescription.Length > 0 Then
                    strSMS &= " [" & jobDescription & "] " & vbCrLf
                End If
                If phone.Length > 0 Then
                    strSMS &= " [Ph:" & phone & "] " & vbCrLf
                End If

                If strAddress.Length > 0 Then
                    jobAddress = strAddress
                    strSMS &= "[" & strAddress & "] " & vbCrLf
                End If

                jobMap = "https://maps.google.com/?q=" & lat.ToString & "," & lng.ToString
                If lat <> 0 Then
                    strSMS &= "[" & jobMap & "]"
                End If
            Else
                'Buid the SMS Message
                strSMS = "JOB:" & name

                If jobDescription.Length > 0 Then
                    strSMS &= " [" & jobDescription & "] "
                End If
                If phone.Length > 0 Then
                    strSMS &= " [Ph: " & phone & "] "
                End If

                If strAddress.Length > 0 Then
                    jobAddress = strAddress
                    strSMS &= "[" & strAddress & "] "
                End If

                jobMap = "https://maps.google.com/?q=" & Math.Round(lat, 5).ToString & "," & Math.Round(lng, 5).ToString
                If lat <> 0 Then
                    strSMS &= "[" & jobMap & "]"
                End If

                'Comment the following truncate because the stored procedure will chop it and send it in batches of messages until the entire message is sent.
                'If strSMS.Length > 160 Then
                '    strSMS = strSMS.Substring(0, 160)
                'End If
            End If

            jobGUID = DL.saveWorkOrder(token, deviceId, driverId, isGeofence, geofenceGUID, name, phone, street, city, state, postalCode, jobDescription, jobAddress, jobMap, lat, lng, sendSMS, strSMS, via, strError)

            If jobGUID.Length > 0 Then
                strResult = LoadJsonResult(jobGUID)
            Else
                strResult = LoadJsonResult("NOTSENT")
                'strResult = LoadJsonError(strError)
            End If
        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveWorkOrderNEW() As String
        Dim DL As New DataLayer
        Dim token As String = ""
        Dim JobNumber As String = ""
        Dim deviceId As String = ""
        Dim dd As String = ""
        Dim name As String = ""
        Dim phone As String = ""
        Dim street As String = ""
        Dim city As String = ""
        Dim state As String = ""
        Dim postalCode As String = ""
        Dim jobDescription As String = ""
        Dim bResults As Boolean = False
        Dim strResult As String = ""
        Dim strError As String = ""
        Dim strSMS As String = ""
        Dim jobGUID As String = ""
        Dim strAddress As String = ""
        Dim isGeofence As String = ""
        Dim geofenceGUID As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim sendSMS As Boolean = False
        Dim driverId As Integer = 0
        Dim jobMap As String = ""
        Dim via As Integer = 0
        Dim durationHH As Integer = 0
        Dim durationMM As Integer = 0
        Dim p_DueDate As DateTime
        Dim StartOn As DateTime
        Dim jobpriority As String = ""
        Dim jobcategories As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("JobNumber")) Then
                JobNumber = HttpContext.Current.Request.Form("JobNumber")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isGeofence")) Then
                If HttpContext.Current.Request.Form("isGeofence").ToLower = "true" Then
                    isGeofence = True
                Else
                    isGeofence = False
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("geofenceId")) Then
                geofenceGUID = CStr(HttpContext.Current.Request.Form("geofenceId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("phone")) Then
                phone = HttpContext.Current.Request.Form("phone")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("postalCode")) Then
                postalCode = HttpContext.Current.Request.Form("postalCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("description")) Then
                jobDescription = HttpContext.Current.Request.Form("description")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lat")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lat")) Then
                    lat = CDec(HttpContext.Current.Request.Form("lat"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lng")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lng")) Then
                    lng = CDec(HttpContext.Current.Request.Form("lng"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sendSMS")) Then
                sendSMS = HttpContext.Current.Request.Form("sendSMS")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("driverId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("driverId")) Then
                    driverId = CInt(HttpContext.Current.Request.Form("driverId"))
                Else
                    driverId = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("via")) Then
                If IsNumeric(HttpContext.Current.Request.Form("via")) Then
                    via = CInt(HttpContext.Current.Request.Form("via"))
                Else
                    via = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("durationHH")) Then
                If IsNumeric(HttpContext.Current.Request.Form("durationHH")) Then
                    durationHH = CInt(HttpContext.Current.Request.Form("durationHH"))
                Else
                    durationHH = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("durationMM")) Then
                If IsNumeric(HttpContext.Current.Request.Form("durationMM")) Then
                    durationMM = CInt(HttpContext.Current.Request.Form("durationMM"))
                Else
                    durationMM = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dueDate")) Then
                If IsDate(HttpContext.Current.Request.Form("dueDate")) Then
                    p_DueDate = CDate(HttpContext.Current.Request.Form("dueDate"))
                Else
                    p_DueDate = Now.Date
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("StartOn")) Then
                If IsDate(HttpContext.Current.Request.Form("StartOn")) Then
                    StartOn = CDate(HttpContext.Current.Request.Form("StartOn"))
                Else
                    StartOn = Now.Date
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("jobpriority")) Then
                jobpriority = HttpContext.Current.Request.Form("jobpriority")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("jobcategories")) Then
                jobcategories = HttpContext.Current.Request.Form("jobcategories")
            End If

            'Build the address
            Dim jobAddress As String = ""
            strAddress = ""
            If street.Length > 0 Then
                strAddress = street
            End If
            If city.Length > 0 Then
                If strAddress.Length > 0 Then
                    strAddress &= " "
                End If
                strAddress &= city
            End If
            If postalCode.Length > 0 Then
                If strAddress.Length > 0 Then
                    strAddress &= " "
                End If
                strAddress &= postalCode
            End If

            If via = 2 Then
                'If via = email, build the email
                'Buid the SMS Message
                strSMS = "JOB:" & name & vbCrLf

                If jobDescription.Length > 0 Then
                    strSMS &= " [" & jobDescription & "] " & vbCrLf
                End If
                If phone.Length > 0 Then
                    strSMS &= " [Ph:" & phone & "] " & vbCrLf
                End If

                If strAddress.Length > 0 Then
                    jobAddress = strAddress
                    strSMS &= "[" & strAddress & "] " & vbCrLf
                End If

                jobMap = "https://maps.google.com/?q=" & lat.ToString & "," & lng.ToString
                If lat <> 0 Then
                    strSMS &= "[" & jobMap & "]"
                End If
            Else
                'Buid the SMS Message
                strSMS = "JOB:" & name

                If jobDescription.Length > 0 Then
                    strSMS &= " [" & jobDescription & "] "
                End If
                If phone.Length > 0 Then
                    strSMS &= " [Ph: " & phone & "] "
                End If

                If strAddress.Length > 0 Then
                    jobAddress = strAddress
                    strSMS &= "[" & strAddress & "] "
                End If

                jobMap = "https://maps.google.com/?q=" & Math.Round(lat, 5).ToString & "," & Math.Round(lng, 5).ToString
                If lat <> 0 Then
                    strSMS &= "[" & jobMap & "]"
                End If

                'Comment the following truncate because the stored procedure will chop it and send it in batches of messages until the entire message is sent.
                'If strSMS.Length > 160 Then
                '    strSMS = strSMS.Substring(0, 160)
                'End If
            End If

            jobGUID = DL.saveWorkOrderNEW(token, JobNumber, deviceId, driverId, isGeofence, geofenceGUID, name, phone, street, city, state, postalCode, jobDescription, jobAddress, jobMap, lat, lng, sendSMS, strSMS, via, durationHH, durationMM, p_DueDate, StartOn, jobpriority, jobcategories, strError)

            If jobGUID.Length > 0 Then
                strResult = LoadJsonResult(jobGUID)
            Else
                strResult = LoadJsonResult("NOTSENT")
                'strResult = LoadJsonError(strError)
            End If
        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function sendDrivingDirectionsEmail() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim bResults As Boolean = False
        Dim strResult As String = ""
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim dd As String = ""

        Dim startAddress As String = ""
        Dim endAddress As String = ""
        Dim distance As String = ""
        Dim duration As String = ""
        Dim steps As String = ""

        Dim stepDistance = ""
        Dim stepDuration = ""

        Try
            Try
                If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                    token = HttpContext.Current.Request.Form("t")
                End If
            Catch ex As Exception
                If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                    token = HttpContext.Current.Request.Form("t")
                End If
            End Try
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dd")) Then
                dd = HttpContext.Current.Request.Form("dd")
            End If

            Dim jsonObj As Object
            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(dd)

            If Not IsNothing(jsonObj) Then
                If Not IsNothing(jsonObj.item("routes")) Then
                    If (jsonObj.item("routes").count >= 1) Then
                        If Not IsNothing(jsonObj("routes")(0)("legs")) Then
                            If (jsonObj("routes")(0)("legs").count >= 1) Then
                                startAddress = jsonObj("routes")(0)("legs")(0).item("start_address").value()
                                endAddress = jsonObj("routes")(0)("legs")(0).item("end_address").value()
                                distance = jsonObj("routes")(0)("legs")(0).item("distance").item("text").value()
                                duration = jsonObj("routes")(0)("legs")(0).item("duration").item("text").value()

                                If Not IsNothing(jsonObj("routes")(0)("legs")(0)("steps")) Then
                                    steps = "<div>"
                                    steps &= "<span style='font-size:14px;font-weight:600;'>Destiation: </span>" & endAddress & "<br />"
                                    steps &= "<span style='font-size:14px;font-weight:600;'>Estimated Distance: </span>" & distance & "<br />"
                                    steps &= "<span style='font-size:14px;font-weight:600;'>Estimated Time: </span>" & duration & "<br /><br />"
                                    For i = 0 To (jsonObj("routes")(0)("legs")(0)("steps").count() - 1)
                                        steps &= "<span style='font-size:14px;font-weight:600;'>" & (i + 1) & ". </span>" & jsonObj("routes")(0)("legs")(0)("steps")(i).item("instructions").value() & "<br />"
                                        stepDistance = jsonObj("routes")(0)("legs")(0)("steps")(i).item("distance").item("text").value()
                                        stepDuration = jsonObj("routes")(0)("legs")(0)("steps")(i).item("duration").item("text").value()
                                    Next
                                    steps &= "</div>"
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            If (steps.Length > 0) Then
                bResults = DL.sendDrivingDirectionsEmail(token, deviceId, steps, strError)
            End If

            If bResults Then
                strResult = LoadJsonResult("true")
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getTrail() As String
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim dateFrom As DateTime
        Dim dateTo As DateTime
        Dim hourFrom As Integer
        Dim hourTo As Integer
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateFrom")) Then
                If IsDate(HttpContext.Current.Request.Form("dateFrom")) Then
                    dateFrom = CDate(HttpContext.Current.Request.Form("dateFrom"))
                Else
                    dateFrom = Now.Date
                End If
            Else
                dateFrom = Now.Date
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourFrom")) Then
                hourFrom = HttpContext.Current.Request.Form("hourFrom")
            Else
                hourFrom = 0
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourTo")) Then
                hourTo = HttpContext.Current.Request.Form("hourTo")
            Else
                hourTo = 24
            End If

            If hourFrom > hourTo Then
                dateTo = DateAdd(DateInterval.Hour, hourTo, DateAdd(DateInterval.Day, 1, dateFrom))
            Else
                dateTo = DateAdd(DateInterval.Hour, hourTo, dateFrom)
            End If
            dateFrom = DateAdd(DateInterval.Hour, hourFrom, dateFrom)

            If IsGUID(token) Then
                dtData = DL.getTrail(token, deviceId, dateFrom, dateTo, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("trail")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonMyDevices(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getTrail", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getTrail2() As String
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim dateFrom As DateTime
        Dim dateTo As DateTime
        Dim hourFrom As Integer
        Dim hourTo As Integer
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateFrom")) Then
                If IsDate(HttpContext.Current.Request.Form("dateFrom")) Then
                    dateFrom = CDate(HttpContext.Current.Request.Form("dateFrom"))
                Else
                    dateFrom = Now.Date
                End If
            Else
                dateFrom = Now.Date
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourFrom")) Then
                hourFrom = HttpContext.Current.Request.Form("hourFrom")
            Else
                hourFrom = 0
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourTo")) Then
                hourTo = HttpContext.Current.Request.Form("hourTo")
            Else
                hourTo = 24
            End If

            If hourFrom > hourTo Then
                dateTo = DateAdd(DateInterval.Hour, hourTo, DateAdd(DateInterval.Day, 1, dateFrom))
            Else
                dateTo = DateAdd(DateInterval.Hour, hourTo, dateFrom)
            End If
            dateFrom = DateAdd(DateInterval.Hour, hourFrom, dateFrom)

            If IsGUID(token) Then
                dtData = DL.getTrail2(token, deviceId, dateFrom, dateTo, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("trail")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonTrail(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getTrail2", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getHDevices_InfoWindow() As String
        Dim token As String = ""
        Dim id As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                Try
                    id = CStr(HttpContext.Current.Request.Form("id"))
                Catch ex As Exception
                    id = ""
                End Try
            End If

            If IsGUID(token) Then
                strJson = DL.getHDevices_InfoWindow(token, id, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getHDevices_InfoWindow", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMultiTrail() As String
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim dateFrom As DateTime
        Dim dateTo As DateTime
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateFrom")) Then
                If IsDate(HttpContext.Current.Request.Form("dateFrom")) Then
                    dateFrom = CDate(HttpContext.Current.Request.Form("dateFrom"))
                Else
                    dateFrom = Now.Date
                End If
            Else
                dateFrom = Now.Date
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateTo")) Then
                If IsDate(HttpContext.Current.Request.Form("dateTo")) Then
                    dateTo = CDate(HttpContext.Current.Request.Form("dateTo"))
                Else
                    dateTo = Now.Date
                End If
            Else
                dateTo = Now.Date
            End If

            If deviceId = "0" Then
                dateFrom = dateTo
            End If
            dateTo = DateAdd(DateInterval.Day, 1, dateTo)

            If IsGUID(token) Then
                strJson = DL.getMultiTrail(token, deviceId, dateFrom, dateTo, strError)
            End If

            If strJson = "" Then
                If strError = "" Then
                    strError = "NODATAFOUND"
                End If
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getMultiTrail", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveWebForm() As String
        Dim bResults As Boolean = True
        Dim strResult As String = ""
        Dim formID As String = ""
        Dim strQty As String = ""
        Dim strServiceID As String = ""
        Dim strShippingOption As String = ""
        Dim firstName As String = ""
        Dim lastName As String = ""
        Dim email As String = ""
        Dim phone As String = ""
        Dim company As String = ""
        Dim street As String = ""
        Dim city As String = ""
        Dim state As String = ""
        Dim postalCode As String = ""
        Dim ccType As String = ""
        Dim ccNumber As String = ""
        Dim ccSec As String = ""
        Dim ccExpMonth As String = ""
        Dim ccExpYear As String = ""

        Dim ccFirstName As String = ""
        Dim ccLastName As String = ""
        Dim ccStreet As String = ""
        Dim ccCity As String = ""
        Dim ccState As String = ""
        Dim ccPostalCode As String = ""
        Dim strMessage As String = ""
        Dim promoCode As String = ""

        Dim repId As String = ""

        Dim DL As New DataLayer
        Dim intQty As Integer
        Dim intServiceID As Integer = 0
        Dim intShippingOption As Integer
        Dim intExpMonth As Integer
        Dim intExpYear As Integer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("formId")) Then
                formID = HttpContext.Current.Request.Form("formId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("qty")) Then
                strQty = HttpContext.Current.Request.Form("qty")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("serviceId")) Then
                strServiceID = HttpContext.Current.Request.Form("serviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ship")) Then
                strShippingOption = HttpContext.Current.Request.Form("ship")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("fn")) Then
                firstName = HttpContext.Current.Request.Form("fn")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ln")) Then
                lastName = HttpContext.Current.Request.Form("ln")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("email")) Then
                email = HttpContext.Current.Request.Form("email")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ph")) Then
                phone = HttpContext.Current.Request.Form("ph")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("co")) Then
                company = HttpContext.Current.Request.Form("co")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("postal")) Then
                postalCode = HttpContext.Current.Request.Form("postal")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccType")) Then
                ccType = HttpContext.Current.Request.Form("ccType")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccNo")) Then
                ccNumber = HttpContext.Current.Request.Form("ccNo")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccSec")) Then
                ccSec = HttpContext.Current.Request.Form("ccSec")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccMonth")) Then
                ccExpMonth = HttpContext.Current.Request.Form("ccMonth")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccYear")) Then
                ccExpYear = HttpContext.Current.Request.Form("ccYear")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccFn")) Then
                ccFirstName = HttpContext.Current.Request.Form("ccFn")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccLn")) Then
                ccLastName = HttpContext.Current.Request.Form("ccLn")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccStreet")) Then
                ccStreet = HttpContext.Current.Request.Form("ccStreet")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccCity")) Then
                ccCity = HttpContext.Current.Request.Form("ccCity")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccState")) Then
                ccState = HttpContext.Current.Request.Form("ccState")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ccPostal")) Then
                ccPostalCode = HttpContext.Current.Request.Form("ccPostal")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("msg")) Then
                strMessage = HttpContext.Current.Request.Form("msg")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("promoCode")) Then
                promoCode = HttpContext.Current.Request.Form("promoCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("repId")) Then
                repId = HttpContext.Current.Request.Form("repId")
            End If

            If IsNumeric(strQty) Then
                intQty = CInt(strQty)
            Else
                intQty = 0
            End If
            If IsNumeric(strServiceID) Then
                intServiceID = CInt(strServiceID)
            Else
                intServiceID = 13
            End If
            If IsNumeric(strShippingOption) Then
                intShippingOption = CInt(strShippingOption)
            Else
                intShippingOption = 0
            End If
            If IsNumeric(ccExpMonth) Then
                intExpMonth = CInt(ccExpMonth)
            Else
                intExpMonth = 0
            End If
            If IsNumeric(ccExpYear) Then
                intExpYear = CInt(ccExpYear)
            Else
                intExpYear = 0
            End If

            If IsGUID(formID) Then
                bResults = DL.saveWebForm(formID, intQty, intServiceID, intShippingOption, firstName, lastName, email, phone, company, street, city, state, postalCode, ccType, ccNumber, ccSec, intExpMonth, intExpYear, ccFirstName, ccLastName, ccStreet, ccCity, ccState, ccPostalCode, strMessage, promoCode, repId)
            Else
                bResults = False
            End If

            If bResults = True Then
                strResult = LoadJsonResult(bResults.ToString)
            Else
                strResult = LoadJsonError("Failed saving form")
            End If

        Catch ex As Exception
            strResult = LoadJsonError("Failed saving form")
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDocQty() As String
        Dim bResults As Boolean = True
        Dim strJson As String = ""
        Dim docId As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("d")) Then
                docId = HttpContext.Current.Request.Form("d")
            End If

            If IsGUID(docId) Then
                dtData = DL.getDocQty(docId, strError)
            Else
                dtData = Nothing
                strError = "Invalid ID"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("doc")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadDocQty(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getDocQty", "", ex.Message & " - docId: " & docId, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function contactUnsubscribe() As String
        Dim bResults As Boolean = True
        Dim strJson As String = ""
        Dim contactId As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                contactId = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(contactId) Then
                dtData = DL.contactUnsubscribe(contactId, strError)
            Else
                dtData = Nothing
                strError = "Invalid ID"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("info")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadUnsubscribeInfo(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "contactUnsubscribe", "", ex.Message & " - contactId: " & contactId, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getFamousQuote() As String
        Dim bResults As Boolean = True
        Dim strJson As String = ""
        Dim TypeGUID As String = ""
        Dim Quote As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("type")) Then
                TypeGUID = HttpContext.Current.Request.Form("type")
            End If

            If IsGUID(TypeGUID) Then
                Quote = DL.getFamousQuote(TypeGUID, strError)
            Else
                Quote = ""
                strError = "Invalid quote type"
            End If

            If Quote.Length > 0 Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

                json.WriteStartObject()
                json.WritePropertyName("quote")
                json.WriteStartArray()
                json.WriteValue(LoadFamousQuote(Quote))
                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getFamousQuote", "", ex.Message & " - TypeGUID: " & TypeGUID, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveGroup() As String
        Dim GroupGUID As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim ID As String = ""
        Dim panelId As Integer = 0
        Dim name As String = ""
        Dim isPublic As Boolean = False
        Dim jsonDevicesTXT As String = ""
        Dim dtDevices As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("p")) Then
                panelId = HttpContext.Current.Request.Form("p")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                ID = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isPublic")) Then
                isPublic = HttpContext.Current.Request.Form("isPublic")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("devices")) Then
                jsonDevicesTXT = HttpContext.Current.Request.Form("devices")
            End If

            If IsGUID(token) Then

                If ID.Length > 0 Then
                    If Not IsGUID(ID) Then
                        strError = "Invalid GroupID"
                    End If
                End If
                If strError.Length = 0 Then
                    dtDevices = createDevicesTable(token, jsonDevicesTXT)
                    GroupGUID = DL.saveGroup(token, panelId, ID, name, isPublic, dtDevices, strError)
                End If
            Else
                strError = "Invalid Token"
            End If

            If IsGUID(GroupGUID) Then
                strResult = LoadJsonResult("true", GroupGUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMTGroupsByPanel() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getMTGroupsByPanel(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("groups")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadMTGroups(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If


        Catch ex As Exception
            strJson = LoadJsonError("Error loading groups")
            BLErrorHandling.ErrorCapture(pSysModule, "getMTGroupsByPanel", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getReports() As String
        Dim token As String = ""
        Dim intIsRecurrent As Integer = -1
        Dim bIsRecurrent As Boolean = False
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isRecurrent")) Then
                intIsRecurrent = HttpContext.Current.Request.Form("isRecurrent")
            End If

            If intIsRecurrent > 0 Then
                bIsRecurrent = True
            End If

            If IsGUID(token) Then
                dtData = DL.getReports(token, bIsRecurrent, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("reports")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadReports(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading reports")
            BLErrorHandling.ErrorCapture(pSysModule, "getReports", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function validateDemoToken() As String
        Dim demoToken As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim dvData As DataView = Nothing
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                demoToken = HttpContext.Current.Request.Form("t")
            End If
            If IsGUID(demoToken) Then
                dvData = DL.validateDemoToken(demoToken, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dvData) Then
                strJson = LoadJsonCredentials(dvData)
            Else
                strJson = "false"
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error processing demo access")
            BLErrorHandling.ErrorCapture(pSysModule, "validateDemoToken", "", ex.Message & " - demoToken: " & demoToken, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function runReport() As String
        Dim strResult As String = ""
        Dim strError As String = ""

        Try
            strResult = processReport(strError)

            If strResult.Length > 0 Then
                strResult = LoadJsonResult(strResult)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception

        End Try


        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function runReportRetroGeo() As String
        Dim DL As New DataLayer
        Dim strResult As String = ""
        Dim strError As String = ""

        Dim token As String = ""
        Dim address As String = ""
        Dim strLat As String = ""
        Dim strLng As String = ""
        Dim strRadius As String = ""
        Dim strMinTime As String = ""
        Dim strDateFrom As String = ""
        Dim strDateTo As String = ""
        Dim strHourFrom As String = ""
        Dim strHourTo As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("address")) Then
                address = HttpContext.Current.Request.Form("address")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lat")) Then
                strLat = HttpContext.Current.Request.Form("lat")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lng")) Then
                strLng = HttpContext.Current.Request.Form("lng")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("radius")) Then
                strRadius = HttpContext.Current.Request.Form("radius")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("minTime")) Then
                strMinTime = HttpContext.Current.Request.Form("minTime")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateFrom")) Then
                strDateFrom = HttpContext.Current.Request.Form("dateFrom")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateTo")) Then
                strDateTo = HttpContext.Current.Request.Form("dateTo")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourFrom")) Then
                strHourFrom = HttpContext.Current.Request.Form("hourFrom")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourTo")) Then
                strHourTo = HttpContext.Current.Request.Form("hourTo")
            End If

            Dim lat As Decimal = 0
            If IsNumeric(strLat) Then
                lat = CDec(strLat)
            End If

            Dim lng As Decimal = 0
            If IsNumeric(strLng) Then
                lng = CDec(strLng)
            End If

            Dim radius As Integer = 0
            If IsNumeric(strRadius) Then
                radius = CInt(strRadius)
            End If

            Dim minTime As Integer = 0
            If IsNumeric(strMinTime) Then
                minTime = CInt(strMinTime)
            End If
            minTime = minTime * 60 'Convert to seconds

            Dim dateFrom As Date
            If IsDate(strDateFrom) Then
                dateFrom = CDate(strDateFrom)
            Else
                dateFrom = Date.UtcNow
            End If

            Dim dateTo As Date
            If IsDate(strDateTo) Then
                dateTo = CDate(strDateTo)
            Else
                dateTo = Date.UtcNow
            End If

            Dim hourFrom As Integer = 0
            If IsNumeric(strHourFrom) Then
                hourFrom = CInt(strHourFrom)
            End If
            dateFrom = DateAdd(DateInterval.Hour, hourFrom, dateFrom)

            Dim hourTo As Integer = 0
            If IsNumeric(strHourTo) Then
                hourTo = CInt(strHourTo)
            End If
            dateTo = DateAdd(DateInterval.Hour, hourTo, dateTo)

            'Call the store procedure
            strResult = DL.runReportRetroGeo(token, address, lat, lng, radius, minTime, dateFrom, dateTo, False, strError)

            'Prepare results
            If strResult.Length > 0 Then
                strResult = LoadJsonResult(strResult)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception

        End Try


        Return strResult

    End Function

    Private Function processReport(ByRef strError As String) As String
        Dim DL As New DataLayer
        Dim strResult As String = ""
        Dim strIsBatch As String = ""
        Dim isBatch As Boolean = False
        Dim token As String = ""
        Dim strReportId As String = ""
        Dim deviceId As String = ""
        Dim strDateFrom As String = ""
        Dim strDateTo As String = ""
        Dim strHourFrom As String = ""
        Dim strHourTo As String = ""
        Dim intHourFrom As Integer = 0
        Dim intHourTo As Integer = 24
        Dim param As String = ""
        Dim param2 As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isBatch")) Then
                strIsBatch = HttpContext.Current.Request.Form("isBatch")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("rId")) Then
                strReportId = HttpContext.Current.Request.Form("rId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dId")) Then
                deviceId = HttpContext.Current.Request.Form("dId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("df")) Then
                strDateFrom = HttpContext.Current.Request.Form("df")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dt")) Then
                strDateTo = HttpContext.Current.Request.Form("dt")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("tf")) Then
                strHourFrom = HttpContext.Current.Request.Form("tf")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("tt")) Then
                strHourTo = HttpContext.Current.Request.Form("tt")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("p")) Then
                param = HttpContext.Current.Request.Form("p")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("p2")) Then
                param2 = HttpContext.Current.Request.Form("p2")
            End If
            If Not IsNumeric(param2) Then
                param2 = "0"
            End If

            If IsGUID(token) Then
                Dim reportId As Integer = 0
                Dim dateFrom As DateTime
                Dim dateTo As DateTime

                If IsNumeric(strReportId) Then
                    reportId = CInt(strReportId)
                End If
                If IsDate(strDateFrom) Then
                    dateFrom = CDate(strDateFrom)
                    dateFrom = DateAdd(DateInterval.Hour, CInt(strHourFrom), dateFrom)
                Else
                    dateFrom = Now.Date
                End If

                If IsDate(strDateTo) Then
                    dateTo = CDate(strDateTo)
                    dateTo = DateAdd(DateInterval.Hour, CInt(strHourTo), dateTo)
                Else
                    dateTo = Now.Date
                End If
                If IsNumeric(strHourFrom) Then
                    intHourFrom = CInt(strHourFrom)
                Else
                    intHourFrom = 0
                End If
                If IsNumeric(strHourTo) Then
                    intHourTo = CInt(strHourTo)
                Else
                    intHourTo = 24
                End If

                If strIsBatch = "true" Then
                    isBatch = True
                Else
                    isBatch = False
                End If

                strResult = DL.runReport(isBatch, token, reportId, deviceId, dateFrom, dateTo, strHourFrom, strHourTo, param, param2, strError)
            Else
                strError = "Invalid Token"
            End If


        Catch ex As Exception
            strResult = LoadJsonError("Error processing report")
            BLErrorHandling.ErrorCapture(pSysModule, "processReport", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getEvents() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getEvents(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("events")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadEvents(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading events")
            BLErrorHandling.ErrorCapture(pSysModule, "getEvents", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getInputs() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getInputs(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("inputs")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadEvents(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading events")
            BLErrorHandling.ErrorCapture(pSysModule, "getEvents", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getCompaniesDevicesEvents() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getCompaniesDevicesEvents(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("events")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadEvents(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading events")
            BLErrorHandling.ErrorCapture(pSysModule, "getEvents", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDrivers() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getDrivers(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("drivers")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadDrivers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading drivers")
            BLErrorHandling.ErrorCapture(pSysModule, "getDrivers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDeviceIcons() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getDeviceIcons(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("icons")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadIcons(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading icons")
            BLErrorHandling.ErrorCapture(pSysModule, "getDeviceIcons", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveDeviceChanges() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim name As String = ""
        Dim shortName As String = ""
        Dim driverId As Integer = 0
        Dim idleLimit As Integer = 0
        Dim speedLimit As Integer = 0
        Dim iconId As Integer = 0
        Dim line2 As String = ""
        Dim isARB As Boolean = False
        Dim arbNumber As String = ""
        'Dim dieselOnEventCode As String = "0"
        'Dim electricOnEventCode As String = "0"
        Dim dieselMeter As Decimal = 0
        Dim electricMeter As Decimal = 0
        Dim odometerReading As Decimal = 0
        Dim vin As String = ""
        Dim fuelCardId As String = ""
        Dim licensePlate As String = ""
        Dim txtColor As String = ""
        Dim bgndColor As String = ""
        Dim dtData As New DataTable
        Dim isBuzzerOn As Boolean = False

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("n")) Then
                name = HttpContext.Current.Request.Form("n")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sn")) Then
                shortName = HttpContext.Current.Request.Form("sn")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("line2")) Then
                line2 = HttpContext.Current.Request.Form("line2")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("driverId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("driverId")) Then
                    driverId = CInt(HttpContext.Current.Request.Form("driverId"))
                Else
                    driverId = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("idleLimit")) Then
                If IsNumeric(HttpContext.Current.Request.Form("idleLimit")) Then
                    idleLimit = CInt(HttpContext.Current.Request.Form("idleLimit"))
                Else
                    idleLimit = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("speedLimit")) Then
                If IsNumeric(HttpContext.Current.Request.Form("speedLimit")) Then
                    speedLimit = CInt(HttpContext.Current.Request.Form("speedLimit"))
                Else
                    speedLimit = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("iconId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("iconId")) Then
                    iconId = CInt(HttpContext.Current.Request.Form("iconId"))
                Else
                    iconId = 0
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("isARB")) Then
                isARB = HttpContext.Current.Request.Form("isARB")
            End If
            If isARB = True Then
                If Not IsNothing(HttpContext.Current.Request.Form("arbNumber")) Then
                    arbNumber = HttpContext.Current.Request.Form("arbNumber")
                End If
                'If Not IsNothing(HttpContext.Current.Request.Form("electricOnEventCode")) Then
                '    electricOnEventCode = HttpContext.Current.Request.Form("electricOnEventCode")
                'End If
                'If Not IsNothing(HttpContext.Current.Request.Form("dieselOnEventCode")) Then
                '    dieselOnEventCode = HttpContext.Current.Request.Form("dieselOnEventCode")
                'End If

                If Not IsNothing(HttpContext.Current.Request.Form("dieselMeter")) Then
                    If IsNumeric(HttpContext.Current.Request.Form("dieselMeter")) Then
                        dieselMeter = CDec(HttpContext.Current.Request.Form("dieselMeter"))
                    End If
                End If
                If Not IsNothing(HttpContext.Current.Request.Form("electricMeter")) Then
                    If IsNumeric(HttpContext.Current.Request.Form("electricMeter")) Then
                        electricMeter = CDec(HttpContext.Current.Request.Form("electricMeter"))
                    End If
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("vin")) Then
                vin = CStr(HttpContext.Current.Request.Form("vin"))
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("fuelCardId")) Then
                fuelCardId = CStr(HttpContext.Current.Request.Form("fuelCardId"))
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("license")) Then
                licensePlate = CStr(HttpContext.Current.Request.Form("license"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("odometer")) Then
                If IsNumeric(HttpContext.Current.Request.Form("odometer")) Then
                    odometerReading = CInt(HttpContext.Current.Request.Form("odometer"))
                Else
                    odometerReading = 0
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("txtColor")) Then
                txtColor = HttpContext.Current.Request.Form("txtColor")
            Else
                txtColor = "#000000"
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("bgndColor")) Then
                bgndColor = HttpContext.Current.Request.Form("bgndColor")
            Else
                bgndColor = "#ffffff"
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("IsBuzzerOn")) Then
                If HttpContext.Current.Request.Form("IsBuzzerOn") = "true" Then
                    isBuzzerOn = True
                End If
            End If

            If IsGUID(token) Then
                dtData = DL.saveDeviceChanges(token, id, name, shortName, driverId, idleLimit, speedLimit, iconId, line2, isARB, arbNumber, dieselMeter, electricMeter, odometerReading, vin, fuelCardId, licensePlate, txtColor, bgndColor, isBuzzerOn, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then
                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                'Unit Details
                json.WriteStartObject()
                json.WritePropertyName("device")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonMyDevices(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "saveDeviceChanges", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function ifByPhone_ClickToCall() As String
        Dim url As String = ""
        Dim result As String = ""
        Dim phone As String = ""
        Dim sourceId As String = ""
        Dim IfByPhone_PublicKey As String = ConfigurationManager.AppSettings("IfByPHone_PublicKey")

        'sourceId is defined as a block in IfByPhone.  Where to find it:
        ' 1) Go to Developer Tools / Building Block IDs.  Under Click-To-Call you'll see the IDs available.  The Click-To name corresponds to the web page.
        ' 2) Go to Basic Services / Click-To-Call.  You'll find there the different options created.
        ' Each page that includes a Click-To-Call feature has to pass the Block ID (called here sourceId).
        '
        'These are the current sources (as of 1/11/2012):
        ' 60141 - Hardware Page
        ' 60161 - System Page
        ' 60171 - Installation Page
        ' 60181 - Default (in case we miss a page)
        ' 60191 - Prices Page
        ' 60201 - FAQ Page
        ' 60211 - ContactUs Page
        ' 60221 - Quote Page
        ' 60231 - Testimonials Page
        ' 60241 - Any of the Thank you pages

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("sourceId")) Then
                sourceId = HttpContext.Current.Request.Form("sourceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("phone")) Then
                phone = HttpContext.Current.Request.Form("phone")
            End If

            If phone.Length = 10 Then

                If sourceId.Length = 0 Then
                    sourceId = ConfigurationManager.AppSettings("IfByPHone_DefaultClickToCallBlockId")
                End If

                url = "https://www.ifbyphone.com/click_to_xyz.php?phone_to_call=" & phone & "&click_id=" & sourceId & "&key=" & IfByPhone_PublicKey

                Dim webClient As System.Net.WebClient = New System.Net.WebClient()
                result = webClient.DownloadString(url)

                result = LoadJsonResult("success", result)

                'Possible results:
                'Call Connected
                'Phone Busy (my notes: the clicker's phone is busy)

            End If

        Catch ex As Exception

        End Try

        Return result

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getSMSGateways() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getSMSGateways(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("smsGateways")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonCarriers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading SMS Gateways")
            BLErrorHandling.ErrorCapture(pSysModule, "getSMSGateways", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDealerBrand() As String
        Dim domain As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("d")) Then
                domain = HttpContext.Current.Request.Form("d")
            End If

            dtData = DL.getDealerBrand(domain, strError)
            strJson = LoadJsonDealerInfo(dtData)

        Catch ex As Exception
            strJson = LoadJsonError("Error loading dealer brand")
            BLErrorHandling.ErrorCapture(pSysModule, "getDealerBrand", "", ex.Message & " - domain: " & domain, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAccessLevels() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getAccessLevels(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("levels")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAccessLevels(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading Access Levels")
            BLErrorHandling.ErrorCapture(pSysModule, "getAccessLevels", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAppFeatures_DEPRECATED() As String
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim token As String = ""
        Dim moduleId As Integer = 0
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = CInt(HttpContext.Current.Request.Form("t"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("moduleId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("moduleId")) Then
                    moduleId = CInt(HttpContext.Current.Request.Form("moduleId"))
                End If
            End If

            dtData = DL.getAppFeatures_DEPRECATED(moduleId, strError)

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("features")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAppFeatures(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading App Features")
            BLErrorHandling.ErrorCapture(pSysModule, "getAppFeatures", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAppFeatures() As String
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim token As String = ""
        Dim moduleId As Integer = 0
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("moduleId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("moduleId")) Then
                    moduleId = CInt(HttpContext.Current.Request.Form("moduleId"))
                End If
            End If

            strJson = DL.getAppFeatures(token, moduleId, strError)
            If strJson = "" Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading App Features")
            BLErrorHandling.ErrorCapture(pSysModule, "getAppFeatures", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAllAppModules() As String
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim token As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            strJson = DL.getAllAppModules(token, strError)
            If strJson = "" Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading App Modules")
            BLErrorHandling.ErrorCapture(pSysModule, "getAllAppModules", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getStates() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getStates(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("states")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonStates(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading states")
            BLErrorHandling.ErrorCapture(pSysModule, "getStates", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDiagInfo() As String
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim jsonData As String = ""
        Dim hId As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hId")) Then
                hId = HttpContext.Current.Request.Form("hId")
            End If

            If IsGUID(token) Then
                jsonData = DL.getDiagInfo(token, deviceId, hId)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("events")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadEvents(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading diag info")
            BLErrorHandling.ErrorCapture(pSysModule, "getDiagInfo", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

#End Region

#Region "Users Module"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getUsers_DEPRECATED() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getUsers_DEPRECATED(token, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                'List of units
                json.WritePropertyName("users")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonUsers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getUsers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getUsers() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getUsers(token, strError)
            End If
            If strJson = "" Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getUsers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveUser() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim firstName As String = ""
        Dim lastName As String = ""
        Dim email As String = ""
        Dim phone As String = ""
        Dim cell As String = ""
        Dim carrierId As Integer = 0
        Dim login As String = ""
        Dim password As String = ""
        Dim timeZoneCode As String = ""
        Dim isDriver As Boolean = False
        Dim accessLevelId As Integer = 0
        Dim isEmailAlerts As Boolean = False
        Dim isSMSAlerts As Boolean = False
        Dim scheduleId As String = ""
        Dim isAdministrator As Boolean = False
        Dim isBillingContact As Boolean = False
        Dim iButton As String = ""
        Dim isAllModules As Boolean = False
        Dim jsonModulesTXT As String = ""
        Dim dtModules As DataTable
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("fn")) Then
                firstName = HttpContext.Current.Request.Form("fn")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("ln")) Then
                lastName = HttpContext.Current.Request.Form("ln")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("e")) Then
                email = HttpContext.Current.Request.Form("e")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isEmailAlerts")) Then
                If HttpContext.Current.Request.Form("isEmailAlerts").ToLower = "true" Then
                    isEmailAlerts = True
                Else
                    isEmailAlerts = False
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("p")) Then
                phone = HttpContext.Current.Request.Form("p")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("cell")) Then
                cell = HttpContext.Current.Request.Form("cell")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isSMSAlerts")) Then
                isSMSAlerts = HttpContext.Current.Request.Form("isSMSAlerts")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("carrierId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("carrierId")) Then
                    carrierId = HttpContext.Current.Request.Form("carrierId")
                Else
                    carrierId = 0
                End If

            End If
            If Not IsNothing(HttpContext.Current.Request.Form("l")) Then
                login = HttpContext.Current.Request.Form("l")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("pw")) Then
                password = HttpContext.Current.Request.Form("pw")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("tz")) Then
                If HttpContext.Current.Request.Form("tz") = "null" Then
                    timeZoneCode = "EST"
                Else
                    timeZoneCode = HttpContext.Current.Request.Form("tz")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isD")) Then
                isDriver = HttpContext.Current.Request.Form("isD")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("al")) Then
                If IsNumeric(HttpContext.Current.Request.Form("al")) Then
                    accessLevelId = CInt(HttpContext.Current.Request.Form("al"))
                Else
                    accessLevelId = 2
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("scheduleId")) Then
                If HttpContext.Current.Request.Form("scheduleId") = "null" Then
                    scheduleId = ""
                Else
                    scheduleId = HttpContext.Current.Request.Form("scheduleId")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("iButton")) Then
                iButton = HttpContext.Current.Request.Form("iButton")
                If iButton = "0" Then
                    iButton = ""
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAdmin")) Then
                If (HttpContext.Current.Request.Form("isAdmin")).ToLower = "true" Then
                    isAdministrator = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isBillingContact")) Then
                If (HttpContext.Current.Request.Form("isBillingContact")).ToLower = "true" Then
                    isBillingContact = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllModules")) Then
                If (HttpContext.Current.Request.Form("isAllModules")).ToLower = "true" Then
                    isAllModules = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("modules")) Then
                jsonModulesTXT = HttpContext.Current.Request.Form("modules")
            End If

            dtModules = createModulesTable(token, jsonModulesTXT)

            If IsGUID(token) Then
                GUID = DL.saveUser(token, id, firstName, lastName, email, isEmailAlerts, phone, cell, isSMSAlerts, carrierId, login, password, timeZoneCode, isDriver, accessLevelId, scheduleId, isAdministrator, isBillingContact, iButton, isAllModules, dtModules, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function changeUserCredentials() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim login As String = ""
        Dim password As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("login")) Then
                login = HttpContext.Current.Request.Form("login")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("pw")) Then
                password = HttpContext.Current.Request.Form("pw")
            End If

            If IsGUID(token) Then
                GUID = DL.changeUserCredentials(token, id, login, password, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function removeUser() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                GUID = DL.removeUser(token, id, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function recoverCredentials() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim email As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("email")) Then
                email = HttpContext.Current.Request.Form("email")
            End If

            GUID = DL.recoverCredentials(email, strError)

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID, "OK")
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = LoadJsonError("FAILED RETRIEVING CREDENTIALS")
        End Try

        Return strResult

    End Function

#End Region

#Region "Recurrent Reports Module"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getRecurrentReports() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getRecurrentReports(token, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                'List of units
                json.WritePropertyName("reports")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadRecurrentReports(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getRecurrentReports", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getRecRepFrequencies() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getRecurrentFrequencies(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("frequencies")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadFrequencies(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading recurrent frequencies")
            BLErrorHandling.ErrorCapture(pSysModule, "getRecRepFrequencies", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getRecRepDevices() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getRecRepDevices(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("myDevices")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonRecRepDevices(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading RecRep devices")
            BLErrorHandling.ErrorCapture(pSysModule, "getRecRepDevices", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getRecRepUsers() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getRecRepUsers(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("users")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonRecRepUsers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading RecRep users")
            BLErrorHandling.ErrorCapture(pSysModule, "getRecRepUsers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveRecurrentReport() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = "" 'This is actually the GUID of the record
        Dim reportId As Integer = 0
        Dim param As String = ""
        Dim frequencyId As Integer = 0
        Dim excludeWeekends As Boolean = False
        Dim isAllDevices As Boolean = False
        Dim jsonDevicesTXT As String = ""
        Dim dtDevices As New DataTable
        Dim isAllUsers As Boolean = False
        Dim jsonUsersTXT As String = ""
        Dim dtUsers As New DataTable
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("reportId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("reportId")) Then
                    reportId = HttpContext.Current.Request.Form("reportId")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("param")) Then
                param = HttpContext.Current.Request.Form("param")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("frequencyId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("frequencyId")) Then
                    frequencyId = HttpContext.Current.Request.Form("frequencyId")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("excludeWeekends")) Then
                If (HttpContext.Current.Request.Form("excludeWeekends")).ToLower = "true" Then
                    excludeWeekends = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllDevices")) Then
                If (HttpContext.Current.Request.Form("isAllDevices")).ToLower = "true" Then
                    isAllDevices = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("devices")) Then
                jsonDevicesTXT = HttpContext.Current.Request.Form("devices")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllUsers")) Then
                If (HttpContext.Current.Request.Form("isAllUsers")).ToLower = "true" Then
                    isAllUsers = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("users")) Then
                jsonUsersTXT = HttpContext.Current.Request.Form("users")
            End If

            'Converts the jsonArrays to tables
            dtDevices = createDevicesTable(token, jsonDevicesTXT)
            dtUsers = createUsersTable(token, jsonUsersTXT)

            If IsGUID(token) Then
                GUID = DL.saveRecurrentReport(token, id, reportId, param, frequencyId, excludeWeekends, isAllDevices, dtDevices, isAllUsers, dtUsers, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
            BLErrorHandling.ErrorCapture(pSysModule, "saveRecurrentReport", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function removeRecurrentReport() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                GUID = DL.removeRecurrentReport(token, id, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    Private Function LoadJsonRecRepDevices(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("deviceId")
            json.WriteValue(drv.Item("DeviceID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonRecRepDevices", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonRecRepUsers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("GUID"))

            json.WritePropertyName("firstName")
            json.WriteValue(drv.Item("FirstName"))

            json.WritePropertyName("lastName")
            json.WriteValue(drv.Item("LastName"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonRecRepUsers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

#End Region

#Region "Schedules"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveSchedule() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim scheduleGUID As String = ""
        Dim scheduleName As String = ""
        Dim jsonValuesTXT As String = ""
        Dim bHasExceptionDays As Boolean = False
        Dim bApplyGlobalExceptions As Boolean = False
        Dim dtValues As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                scheduleGUID = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("scheduleName")) Then
                scheduleName = HttpContext.Current.Request.Form("scheduleName")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hasExceptionDays")) Then
                If (HttpContext.Current.Request.Form("hasExceptionDays")).ToLower = "true" Then
                    bHasExceptionDays = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("applyGlobalExceptions")) Then
                If (HttpContext.Current.Request.Form("applyGlobalExceptions")).ToLower = "true" Then
                    bApplyGlobalExceptions = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("values")) Then
                jsonValuesTXT = HttpContext.Current.Request.Form("values")
            End If

            'Converts the jsonArrays to tables
            dtValues = createScheduleValuesTable(token, jsonValuesTXT)

            If IsGUID(token) Then
                scheduleGUID = DL.saveSchedule(token, scheduleGUID, scheduleName, bHasExceptionDays, bApplyGlobalExceptions, dtValues, strError)
            Else
                scheduleGUID = ""
                strError = "Invalid Token"
            End If

            If scheduleGUID.Length > 0 Then
                strResult = LoadJsonResult(scheduleGUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
            BLErrorHandling.ErrorCapture(pSysModule, "saveSchedule", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getSchedules() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getSchedules(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getSchedules", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getScheduleDays() As String
        Dim token As String = ""
        Dim scheduleId As String = ""
        Dim strJson As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                scheduleId = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                strJson = DL.getScheduleDays(token, scheduleId, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getScheduleDays", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

#End Region

#Region "iButtons"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getIButtons() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getIButtons(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getIButtons", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function removeIButton() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                GUID = DL.removeIButton(token, id, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveIButton() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim id As String = ""
        Dim name As String = ""
        Dim type As Integer = 0
        Dim assignedToId As String = ""
        Dim newId As String = ""
        Dim iButtonHEX As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("type")) Then
                If IsNumeric(HttpContext.Current.Request.Form("type")) Then
                    type = CInt(HttpContext.Current.Request.Form("type"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("assignedToId")) Then
                assignedToId = HttpContext.Current.Request.Form("assignedToId")
            End If

            If IsNumeric(id) Then
                Try
                    iButtonHEX = Hex(id)
                Catch ex As Exception
                    iButtonHEX = ""
                End Try
            End If

            If IsGUID(token) Then
                newId = DL.saveIButton(token, id, iButtonHEX, name, type, assignedToId, strError)
            Else
                newId = ""
                strError = "Invalid Token"
            End If

            If newId.Length > 0 Then
                strResult = LoadJsonResult(newId, iButtonHEX)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
            BLErrorHandling.ErrorCapture(pSysModule, "saveIButton", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strResult

    End Function

#End Region

#Region "Devices"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function resetPowerCut() As String
        Dim DL As New DataLayer
        Dim bResult As Boolean
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim deviceId As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If


            If IsGUID(token) Then
                bResult = DL.resetPowerCut(token, deviceId, strError)
            Else
                bResult = False
                strError = "Invalid Token"
            End If

            If bResult = True Then
                strResult = LoadJsonResult(True)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function resetBadInstallMsg() As String
        Dim DL As New DataLayer
        Dim bResult As Boolean
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim deviceId As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If


            If IsGUID(token) Then
                bResult = DL.resetBadInstallMsg(token, deviceId, strError)
            Else
                bResult = False
                strError = "Invalid Token"
            End If

            If bResult = True Then
                strResult = LoadJsonResult(True)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

#End Region

#Region "Devices Groups"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDevGroups() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getDevGroups(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getDevGroups", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDevGroupDevices() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getDevGroupDevices(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("myDevices")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAlertDevices(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading DevGroup devices")
            BLErrorHandling.ErrorCapture(pSysModule, "getDevGroupDevices", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getDevGroupUsers() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getDevGroupUsers(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("users")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonDevGroupUsers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading DevGroup users")
            BLErrorHandling.ErrorCapture(pSysModule, "getDevGroupUsers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveDeviceGroup() As String
        Dim GroupGUID As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim ID As String = ""
        Dim name As String = ""
        Dim isAllDevices As Boolean = False
        Dim isAllUsers As Boolean = False
        Dim jsonDevicesTXT As String = ""
        Dim jsonUsersTXT As String = ""
        Dim dtDevices As New DataTable
        Dim dtUsers As New DataTable
        Dim hasSpeedGauge As Boolean = False

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                ID = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllDevices")) Then
                isAllDevices = HttpContext.Current.Request.Form("isAllDevices")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllUsers")) Then
                isAllUsers = HttpContext.Current.Request.Form("isAllUsers")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hasSG")) Then
                hasSpeedGauge = HttpContext.Current.Request.Form("hasSG")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("devices")) Then
                jsonDevicesTXT = HttpContext.Current.Request.Form("devices")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("users")) Then
                jsonUsersTXT = HttpContext.Current.Request.Form("users")
            End If

            dtDevices = createDevicesTable(token, jsonDevicesTXT)
            dtUsers = createUsersTable(token, jsonUsersTXT)


            If IsGUID(token) Then
                ID = DL.saveDeviceGroup(token, ID, name, isAllDevices, dtDevices, isAllUsers, dtUsers, hasSpeedGauge, strError)
            Else
                ID = ""
                strError = "Invalid Token"
            End If

            If ID.Length > 0 Then
                strResult = LoadJsonResult(ID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function deleteDeviceGroup() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                GUID = DL.deleteDeviceGroup(token, id, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

#End Region

#Region "Geofences Module"

    'This method has been replaced by etrest.svc/saveGeofence
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveGeofence_DEPRECATED() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim name As String = ""
        Dim fullAddress As String = ""
        Dim street As String = ""
        Dim streetNumber As String = ""
        Dim route As String = ""
        Dim suite As String = ""
        Dim city As String = ""
        Dim county As String = ""
        Dim state As String = ""
        Dim postalCode As String = ""
        Dim country As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim geofenceAlertTypeId As Integer = 0
        Dim geofenceTypeId As Integer = 0
        Dim radius As Integer = 0
        Dim comments As String = ""
        Dim shapeId As Integer = 0
        Dim jsonPolyVerticesTXT As String = ""
        Dim contactName As String = ""

        'GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
        Dim contactEmail As String = ""
        Dim contactAlertTypeId As Integer = 0
        Dim contactSMSAlert As Boolean = False
        Dim contactEmailAlert As Boolean = False
        '========================================

        Dim phone As String = ""
        Dim isSpeedLimit As Boolean = False
        Dim speedLimit As Integer = 0
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("contactName")) Then
                contactName = HttpContext.Current.Request.Form("contactName")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("phone")) Then
                phone = HttpContext.Current.Request.Form("phone")
            End If

            'GEOFENCE CONTACT NOTIFICATIONS. 11/23/2013
            If Not IsNothing(HttpContext.Current.Request.Form("contEmail")) Then
                contactEmail = HttpContext.Current.Request.Form("contEmail")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("contSMSAlert")) Then
                If HttpContext.Current.Request.Form("contSMSAlert").ToLower = "true" Then
                    contactSMSAlert = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("contEmailAlert")) Then
                If HttpContext.Current.Request.Form("contEmailAlert").ToLower = "true" Then
                    contactEmailAlert = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("contAlertType")) Then
                If IsNumeric(HttpContext.Current.Request.Form("contAlertType")) Then
                    contactAlertTypeId = CInt(HttpContext.Current.Request.Form("contAlertType"))
                End If
            End If
            '========================================

            If Not IsNothing(HttpContext.Current.Request.Form("geofenceTypeId")) Then
                geofenceTypeId = HttpContext.Current.Request.Form("geofenceTypeId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("fullAddress")) Then
                fullAddress = HttpContext.Current.Request.Form("fullAddress")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("streetNumber")) Then
                streetNumber = HttpContext.Current.Request.Form("streetNumber")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("route")) Then
                route = HttpContext.Current.Request.Form("route")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("suite")) Then
                suite = HttpContext.Current.Request.Form("suite")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("county")) Then
                county = HttpContext.Current.Request.Form("county")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("postalCode")) Then
                postalCode = HttpContext.Current.Request.Form("postalCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("country")) Then
                country = HttpContext.Current.Request.Form("country")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lat")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lat")) Then
                    lat = CDec(HttpContext.Current.Request.Form("lat"))
                Else
                    lat = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lng")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lng")) Then
                    lng = CDec(HttpContext.Current.Request.Form("lng"))
                Else
                    lng = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("alertTypeId")) Then
                geofenceAlertTypeId = CInt(HttpContext.Current.Request.Form("alertTypeId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("radius")) Then
                If IsNumeric(HttpContext.Current.Request.Form("radius")) Then
                    radius = CInt(HttpContext.Current.Request.Form("radius"))
                Else
                    radius = 0
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("comments")) Then
                comments = HttpContext.Current.Request.Form("comments")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("shapeId")) Then
                shapeId = CDec(HttpContext.Current.Request.Form("shapeId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("vertices")) Then
                jsonPolyVerticesTXT = HttpContext.Current.Request.Form("vertices")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("isSpeedLimit")) Then
                If (HttpContext.Current.Request.Form("isSpeedLimit")).ToLower = "true" Then
                    isSpeedLimit = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("speedLimit")) Then
                If IsNumeric(HttpContext.Current.Request.Form("speedLimit")) Then
                    speedLimit = CInt(HttpContext.Current.Request.Form("speedLimit"))
                Else
                    speedLimit = 0
                End If
            End If


            Dim KMLData As String = ""
            Dim SQLData As String = ""
            If shapeId = 2 Then
                getFormattedData(jsonPolyVerticesTXT, KMLData, SQLData)
            Else
                getFormattedData(lat, lng, KMLData, SQLData)
            End If

            If IsGUID(token) Then
                GUID = DL.saveGeofence(token, id, geofenceTypeId, name, contactName, phone, contactEmail, contactSMSAlert, contactEmailAlert, contactAlertTypeId,
                                       fullAddress, street, streetNumber,
                                       route, suite, city, county, state, postalCode, country, lat, lng, geofenceAlertTypeId,
                                       radius, comments, shapeId, jsonPolyVerticesTXT, KMLData, SQLData, isSpeedLimit, speedLimit, 0, "", 0, "", strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofences() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getGeofences(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofences", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofences_All() As String
        Dim token As String = ""
        Dim isExtended As Boolean = False
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("extended")) Then
                Try
                    isExtended = CBool(HttpContext.Current.Request.Form("extended"))
                Catch ex As Exception
                    isExtended = False
                End Try
            End If

            If IsGUID(token) Then
                strJson = DL.getGeofences_All(token, isExtended, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofences_All", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofences_AllList() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getGeofences_AllList(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofences_AllList", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofences_InfoWindow() As String
        Dim token As String = ""
        Dim id As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                Try
                    id = CStr(HttpContext.Current.Request.Form("id"))
                Catch ex As Exception
                    id = ""
                End Try
            End If

            If IsGUID(token) Then
                strJson = DL.getGeofences_InfoWindow(token, id, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofences_All", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofence() As String
        Dim token As String = ""
        Dim id As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                strJson = DL.getGeofence(token, id, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofence", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function removeGeofence() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                GUID = DL.removeGeofence(token, id, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofencesAlertsTypes() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getGeofencesAlertsTypes(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("types")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonGeoAlertsTypes(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading Geofences Alerts Types")
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofencesAlertsTypes", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getGeofencesTypes() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getGeofencesTypes(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("types")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonGeoTypes(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading Geofences Types")
            BLErrorHandling.ErrorCapture(pSysModule, "getGeofencesTypes", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function geofenceExists() As String
        Dim token As String = ""
        Dim geofenceName As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim bExists As Boolean = False

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                geofenceName = HttpContext.Current.Request.Form("name")
            End If

            If IsGUID(token) Then
                bExists = DL.geofenceExists(token, geofenceName, strError)
            Else
                strError = "Invalid Token"
            End If

            If strError.Length = 0 Then
                strJson = LoadJsonResult(bExists.ToString.ToLower)
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error in geofenceExists")
            BLErrorHandling.ErrorCapture(pSysModule, "geofenceExists", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

#End Region

#Region "Alerts Module"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAlerts() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getAlerts(token, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                'List of alerts
                json.WritePropertyName("alerts")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAlerts(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getAlerts", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAlertsTypes() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getAlertsTypes(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("types")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAlertsTypes(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading alerts types")
            BLErrorHandling.ErrorCapture(pSysModule, "getAlertsTypes", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveAlert() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = "" 'This is actually the GUID of the record
        Dim typeId As Integer = 0
        Dim name As String = ""
        Dim GeofenceGUID As String = ""
        Dim val As Integer = 0
        Dim isAllDevices As Boolean = False
        Dim jsonDevicesTXT As String = ""
        Dim dtDevices As New DataTable
        Dim isAllUsers As Boolean = False
        Dim jsonUsersTXT As String = ""
        Dim bMon As Boolean = False
        Dim bTue As Boolean = False
        Dim bWed As Boolean = False
        Dim bThu As Boolean = False
        Dim bFri As Boolean = False
        Dim bSat As Boolean = False
        Dim bSun As Boolean = False
        Dim hourFrom As Integer = 0
        Dim hourTo As Integer = 0
        Dim minInterval As Integer = 0
        Dim dtUsers As New DataTable
        Dim GUID As String = ""
        Dim setPointMin As Integer = 0
        Dim setPointMax As Integer = 0

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("typeId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("typeId")) Then
                    typeId = HttpContext.Current.Request.Form("typeId")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("geofenceGUID")) Then
                GeofenceGUID = HttpContext.Current.Request.Form("geofenceGUID")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("val")) Then
                If IsNumeric(HttpContext.Current.Request.Form("val")) Then
                    val = HttpContext.Current.Request.Form("val")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllDevices")) Then
                If (HttpContext.Current.Request.Form("isAllDevices")).ToLower = "true" Then
                    isAllDevices = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("devices")) Then
                jsonDevicesTXT = HttpContext.Current.Request.Form("devices")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("isAllUsers")) Then
                If (HttpContext.Current.Request.Form("isAllUsers")).ToLower = "true" Then
                    isAllUsers = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("users")) Then
                jsonUsersTXT = HttpContext.Current.Request.Form("users")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("mon")) Then
                If (HttpContext.Current.Request.Form("mon")).ToLower = "true" Then
                    bMon = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("tue")) Then
                If (HttpContext.Current.Request.Form("tue")).ToLower = "true" Then
                    bTue = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("wed")) Then
                If (HttpContext.Current.Request.Form("wed")).ToLower = "true" Then
                    bWed = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("thu")) Then
                If (HttpContext.Current.Request.Form("thu")).ToLower = "true" Then
                    bThu = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("fri")) Then
                If (HttpContext.Current.Request.Form("fri")).ToLower = "true" Then
                    bFri = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sat")) Then
                If (HttpContext.Current.Request.Form("sat")).ToLower = "true" Then
                    bSat = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("sun")) Then
                If (HttpContext.Current.Request.Form("sun")).ToLower = "true" Then
                    bSun = True
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourFrom")) Then
                If IsNumeric(HttpContext.Current.Request.Form("hourFrom")) Then
                    hourFrom = HttpContext.Current.Request.Form("hourFrom")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("hourTo")) Then
                If IsNumeric(HttpContext.Current.Request.Form("hourTo")) Then
                    hourTo = HttpContext.Current.Request.Form("hourTo")
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("minInterval")) Then
                If IsNumeric(HttpContext.Current.Request.Form("minInterval")) Then
                    minInterval = HttpContext.Current.Request.Form("minInterval")
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("spMin")) Then
                If IsNumeric(HttpContext.Current.Request.Form("spMin")) Then
                    setPointMin = CInt(HttpContext.Current.Request.Form("spMin"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("spMax")) Then
                If IsNumeric(HttpContext.Current.Request.Form("spMax")) Then
                    setPointMax = CInt(HttpContext.Current.Request.Form("spMax"))
                End If
            End If

            'Converts the jsonArrays to tables
            dtDevices = createDevicesTable(token, jsonDevicesTXT)
            dtUsers = createAlertUsersTable(token, jsonUsersTXT)

            If IsGUID(token) Then
                GUID = DL.saveAlert(token, id, typeId, name, GeofenceGUID, val, isAllDevices, dtDevices, isAllUsers, dtUsers, bMon, bTue, bWed, bThu, bFri, bSat, bSun, hourFrom, hourTo, minInterval, setPointMin, setPointMax, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function removeAlert() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                GUID = DL.removeAlert(token, id, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAlertDevices() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getAlertDevices(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("myDevices")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAlertDevices(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading alert devices")
            BLErrorHandling.ErrorCapture(pSysModule, "getAlertDevices", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getAlertUsers() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getAlertUsers(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("users")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonAlertUsers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading alert users")
            BLErrorHandling.ErrorCapture(pSysModule, "getAlertUsers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    Private Function LoadJsonAlerts(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("GUID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("alertTypeId")
            json.WriteValue(drv.Item("AlertTypeID"))

            json.WritePropertyName("alertTypeName")
            json.WriteValue(drv.Item("AlertTypeName"))

            json.WritePropertyName("value")
            json.WriteValue(drv.Item("Value"))

            Try
                json.WritePropertyName("minInterval")
                json.WriteValue(drv.Item("MinInterval"))
            Catch ex As Exception
                json.WriteValue(0)
            End Try

            json.WritePropertyName("mon")
            If IsDBNull(drv.Item("Mon")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("Mon"))
            End If

            json.WritePropertyName("tue")
            If IsDBNull(drv.Item("tue")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("tue"))
            End If

            json.WritePropertyName("wed")
            If IsDBNull(drv.Item("wed")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("wed"))
            End If

            json.WritePropertyName("thu")
            If IsDBNull(drv.Item("thu")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("thu"))
            End If

            json.WritePropertyName("fri")
            If IsDBNull(drv.Item("fri")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("fri"))
            End If

            json.WritePropertyName("sat")
            If IsDBNull(drv.Item("sat")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("sat"))
            End If

            json.WritePropertyName("sun")
            If IsDBNull(drv.Item("sun")) Then
                json.WriteValue(False)
            Else
                json.WriteValue(drv.Item("sun"))
            End If

            json.WritePropertyName("hourFrom")
            If IsDBNull(drv.Item("CheckFrom")) Then
                json.WriteValue(0)
            Else
                json.WriteValue(drv.Item("CheckFrom"))
            End If

            json.WritePropertyName("hourTo")
            If IsDBNull(drv.Item("CheckTo")) Then
                json.WriteValue(0)
            Else
                json.WriteValue(drv.Item("CheckTo"))
            End If

            json.WritePropertyName("setPointMin")
            If IsDBNull(drv.Item("setPointMin")) Then
                json.WriteValue(0)
            Else
                json.WriteValue(drv.Item("setPointMin"))
            End If

            json.WritePropertyName("setPointMax")
            If IsDBNull(drv.Item("setPointMax")) Then
                json.WriteValue(0)
            Else
                json.WriteValue(drv.Item("setPointMax"))
            End If

            Dim valueDescription As String = ""
            If drv.Item("AlertTypeID") = 7 Then
                If drv.Item("Mon") = True Then
                    valueDescription &= "M."
                End If
                If drv.Item("Tue") = True Then
                    valueDescription &= "T."
                End If
                If drv.Item("Wed") = True Then
                    valueDescription &= "W."
                End If
                If drv.Item("Thu") = True Then
                    valueDescription &= "Th."
                End If
                If drv.Item("Fri") = True Then
                    valueDescription &= "F."
                End If
                If drv.Item("Sat") = True Then
                    valueDescription &= "S."
                End If
                If drv.Item("Sun") = True Then
                    valueDescription &= "Su."
                End If
                valueDescription &= "/" & drv.Item("CheckFrom").ToString & " to " & drv.Item("CheckTo").ToString
            Else
                valueDescription = drv.Item("ValueDescription")
            End If
            json.WritePropertyName("valueDescription")
            json.WriteValue(valueDescription)


            json.WritePropertyName("isAllDevices")
            json.WriteValue(drv.Item("isAllDevices"))

            json.WritePropertyName("createdOn")
            json.WriteValue(drv.Item("CreatedOn"))

            json.WritePropertyName("createdOnString")
            json.WriteValue(drv.Item("CreatedOn").ToString)

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonAlerts", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonAlertsTypes(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonAlertsTypes", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonAlertDevices(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("deviceId")
            json.WriteValue(drv.Item("DeviceID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonAlertDevices", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonAlertUsers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("GUID"))

            json.WritePropertyName("firstName")
            json.WriteValue(drv.Item("FirstName"))

            json.WritePropertyName("lastName")
            json.WriteValue(drv.Item("LastName"))

            json.WritePropertyName("isEmail")
            json.WriteValue(drv.Item("IsEmail"))

            json.WritePropertyName("isSMS")
            json.WriteValue(drv.Item("IsSMS"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonAlertUsers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonDevGroupUsers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonDevGroupUsers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

#End Region

#Region "Maintenance Module"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMaintSchedules() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getMaintSchedules(token, "0", 0, strError)
            Else
                dtData = Nothing
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()

                'List of schedules
                json.WritePropertyName("schedules")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonSchedules(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintSchedules", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMaintenanceTasks() As String
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim token As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getMaintenanceTasks(token, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("tasks")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonMaintenanceTasks(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading Maintenance Tasks")
            BLErrorHandling.ErrorCapture(pSysModule, "getTasks", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMaintTasksMeassures() As String
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim token As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                dtData = DL.getMaintTasksMeassures(strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("meassures")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonMaintTasksMeassures(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading Maintenance Tasks Meassures")
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintTasksMeassures", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMaintServicesTypes() As String
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim strError As String = ""
        Dim token As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getMaintServicesTypes(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strJson = ""
                strError = "Invalid Token"
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading Maintenance Services Types")
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintServicesTypes", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getHServices_DEPRECATED() As String
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim taskId As Integer = 0
        Dim dateFrom As Date
        Dim dateTo As Date
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("taskId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("taskId")) Then
                    taskId = CInt(HttpContext.Current.Request.Form("taskId"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateFrom")) Then
                If IsDate(HttpContext.Current.Request.Form("dateFrom")) Then
                    dateFrom = CDate(HttpContext.Current.Request.Form("dateFrom"))
                Else
                    dateFrom = Date.Now.Date
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateTo")) Then
                If IsDate(HttpContext.Current.Request.Form("dateTo")) Then
                    dateTo = CDate(HttpContext.Current.Request.Form("dateTo"))
                Else
                    dateTo = Date.Now.Date
                End If
            End If

            If IsGUID(token) Then
                'strJson = DL.getHServices(token, deviceId, taskId, dateFrom, dateTo, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strJson = ""
                strError = "Invalid Token"
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getHServices", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getHFuel_DEPRECATED() As String
        Dim token As String = ""
        Dim deviceId As String = ""
        Dim taskId As Integer = 0
        Dim dateFrom As Date
        Dim dateTo As Date
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateFrom")) Then
                If IsDate(HttpContext.Current.Request.Form("dateFrom")) Then
                    dateFrom = CDate(HttpContext.Current.Request.Form("dateFrom"))
                Else
                    dateFrom = Date.Now.Date
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("dateTo")) Then
                If IsDate(HttpContext.Current.Request.Form("dateTo")) Then
                    dateTo = CDate(HttpContext.Current.Request.Form("dateTo"))
                Else
                    dateTo = Date.Now.Date
                End If
            End If

            If IsGUID(token) Then
                'strJson = DL.getHFuel(token, deviceId, dateFrom, dateTo, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strJson = ""
                strError = "Invalid Token"
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getHFuel", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function removeTask() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As Integer = 0
        Dim bResults As Boolean = True

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                If IsNumeric(HttpContext.Current.Request.Form("id")) Then
                    id = CInt(HttpContext.Current.Request.Form("id"))
                End If
            End If

            If IsGUID(token) Then
                bResults = DL.removeTask(token, id, strError)
            Else
                bResults = False
                strError = "Invalid Token"
            End If

            If bResults = True Then
                strResult = LoadJsonResult("true")
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveMaintTask() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = "" 'This is actually the GUID of the record
        Dim name As String = ""
        Dim meassureId As Integer = 0
        Dim value As Decimal = 0
        Dim jsonUsersTXT As String = ""
        Dim taskId As Integer = 0
        Dim dtUsers As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("n")) Then
                name = HttpContext.Current.Request.Form("n")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("m")) Then
                If IsNumeric(HttpContext.Current.Request.Form("m")) Then
                    meassureId = HttpContext.Current.Request.Form("m")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("v")) Then
                If IsNumeric(HttpContext.Current.Request.Form("v")) Then
                    value = HttpContext.Current.Request.Form("v")
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("users")) Then
                jsonUsersTXT = HttpContext.Current.Request.Form("users")
            End If

            'Converts the jsonArrays to tables
            dtUsers = createAlertUsersTable(token, jsonUsersTXT)

            If IsGUID(token) Then
                taskId = DL.saveMaintTask(token, id, name, meassureId, value, dtUsers, strError)
            Else
                taskId = 0
                strError = "Invalid Token"
            End If

            If taskId > 0 Then
                strResult = LoadJsonResult(taskId)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveMaintSchedule() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = "" 'This is actually the GUID of the record
        Dim deviceId As String = ""
        Dim taskId As Integer = 0
        Dim taskValue As Decimal = 0
        Dim lastServiceOn As Date = CDate("1/1/1900")
        Dim currentValue As Decimal = 0
        Dim notifyBefore As Decimal = 0
        Dim notifyEveryXDays As Integer = 0
        Dim excludeWeekends As Boolean = True
        Dim jsonUsersTXT As String = ""
        Dim dtUsers As New DataTable
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("taskId")) Then
                taskId = CInt(HttpContext.Current.Request.Form("taskId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("taskValue")) Then
                taskValue = CDec(HttpContext.Current.Request.Form("taskValue"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lastServiceOn")) Then
                If IsDate(HttpContext.Current.Request.Form("lastServiceOn")) Then
                    lastServiceOn = CDate(HttpContext.Current.Request.Form("lastServiceOn"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("currentValue")) Then
                If IsNumeric(HttpContext.Current.Request.Form("currentValue")) Then
                    currentValue = CDec(HttpContext.Current.Request.Form("currentValue"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("notifyBefore")) Then
                If IsNumeric(HttpContext.Current.Request.Form("notifyBefore")) Then
                    notifyBefore = CDec(HttpContext.Current.Request.Form("notifyBefore"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("notifyEveryXDays")) Then
                If IsNumeric(HttpContext.Current.Request.Form("notifyEveryXDays")) Then
                    notifyEveryXDays = CInt(HttpContext.Current.Request.Form("notifyEveryXDays"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("excludeWeekends")) Then
                excludeWeekends = HttpContext.Current.Request.Form("excludeWeekends")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("users")) Then
                jsonUsersTXT = HttpContext.Current.Request.Form("users")
            End If

            'Converts the jsonArrays to tables
            dtUsers = createAlertUsersTable(token, jsonUsersTXT)

            If IsGUID(token) Then
                GUID = DL.saveMaintSchedule(token, id, deviceId, taskId, taskValue, lastServiceOn, currentValue, notifyBefore, notifyEveryXDays, excludeWeekends, dtUsers, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function deleteMaintSchedule() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim bResults As Boolean = True

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                bResults = DL.deleteMaintSchedule(token, id, strError)
            Else
                bResults = False
                strError = "Invalid Token"
            End If

            If bResults = True Then
                strResult = LoadJsonResult("true")
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getMaintAlertUsers() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strJson As String = ""
        Dim id As String = ""
        Dim strError As String = ""
        Dim dtData As New DataTable

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                dtData = DL.getMaintAlertUsers(token, id, strError)
            Else
                strError = "Invalid Token"
            End If

            If Not IsNothing(dtData) Then

                'Build json array
                Dim sb As New StringBuilder
                Dim sw As New StringWriter(sb)
                Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
                Dim drv As DataRowView

                json.WriteStartObject()
                json.WritePropertyName("users")
                json.WriteStartArray()

                For Each drv In dtData.DefaultView
                    json.WriteValue(LoadJsonMaintAlertUsers(drv))
                Next

                json.WriteEnd()
                json.WriteEndObject()
                json.Flush()
                strJson = sb.ToString
            Else
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError("Error loading MaintAlert users")
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintAlertUsers", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveMaintServiceLog() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = "" 'This is actually the GUID of the record
        Dim deviceId As String = ""
        Dim serviceTypeId As Integer = 0
        Dim taskId As Integer = 0
        Dim serviceDescription As String = ""
        Dim strServiceDate As String = ""
        Dim serviceDate As Date
        Dim odometer As Decimal = 0
        Dim cost As Decimal = 0
        Dim comments As String = ""
        Dim meassureValueOnDayOfService As Decimal = 0
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("serviceTypeId")) Then
                serviceTypeId = CInt(HttpContext.Current.Request.Form("serviceTypeId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("taskId")) Then
                taskId = CInt(HttpContext.Current.Request.Form("taskId"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("serviceDescription")) Then
                serviceDescription = HttpContext.Current.Request.Form("serviceDescription")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("serviceDate")) Then
                strServiceDate = HttpContext.Current.Request.Form("serviceDate")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("odometer")) Then
                If IsNumeric(HttpContext.Current.Request.Form("odometer")) Then
                    odometer = CDec(HttpContext.Current.Request.Form("odometer"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("cost")) Then
                If IsNumeric(HttpContext.Current.Request.Form("cost")) Then
                    cost = CDec(HttpContext.Current.Request.Form("cost"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("comments")) Then
                comments = HttpContext.Current.Request.Form("comments")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("val")) Then
                If IsNumeric(HttpContext.Current.Request.Form("val")) Then
                    meassureValueOnDayOfService = CDec(HttpContext.Current.Request.Form("val"))
                End If
            End If

            If IsDate(strServiceDate) Then
                serviceDate = CDate(strServiceDate)
            Else
                serviceDate = Date.UtcNow
            End If

            If IsGUID(token) Then
                GUID = DL.saveMaintServiceLog(token, id, deviceId, serviceTypeId, taskId, serviceDescription, serviceDate, odometer, cost, meassureValueOnDayOfService, comments, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function deleteMaintServiceLog() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim bResults As Boolean = True

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                bResults = DL.deleteMaintServiceLog(token, id, strError)
            Else
                bResults = False
                strError = "Invalid Token"
            End If

            If bResults = True Then
                strResult = LoadJsonResult("true")
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveMaintFuelLog() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = "" 'This is actually the GUID of the record
        Dim deviceId As String = ""
        Dim strFuelingDate As String = ""
        Dim fuelingDate As Date
        Dim odometer As Decimal = 0
        Dim gallons As Decimal = 0
        Dim cost As Decimal = 0
        Dim stateId As Integer
        Dim comments As String = ""
        Dim GUID As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("deviceId")) Then
                deviceId = HttpContext.Current.Request.Form("deviceId")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("fuelingDate")) Then
                strFuelingDate = HttpContext.Current.Request.Form("fuelingDate")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("odometer")) Then
                If IsNumeric(HttpContext.Current.Request.Form("odometer")) Then
                    odometer = CDec(HttpContext.Current.Request.Form("odometer"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("gallons")) Then
                If IsNumeric(HttpContext.Current.Request.Form("gallons")) Then
                    gallons = CDec(HttpContext.Current.Request.Form("gallons"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("cost")) Then
                If IsNumeric(HttpContext.Current.Request.Form("cost")) Then
                    cost = CDec(HttpContext.Current.Request.Form("cost"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("stateId")) Then
                If IsNumeric(HttpContext.Current.Request.Form("stateId")) Then
                    stateId = CInt(HttpContext.Current.Request.Form("stateId"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("comments")) Then
                comments = HttpContext.Current.Request.Form("comments")
            End If

            If IsDate(strFuelingDate) Then
                fuelingDate = CDate(strFuelingDate)
            Else
                fuelingDate = Date.UtcNow
            End If

            If IsGUID(token) Then
                GUID = DL.saveMaintFuelLog(token, id, deviceId, fuelingDate, odometer, gallons, cost, stateId, comments, strError)
            Else
                GUID = ""
                strError = "Invalid Token"
            End If

            If GUID.Length > 0 Then
                strResult = LoadJsonResult(GUID)
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function deleteMaintFuelLog() As String
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strResult As String = ""
        Dim token As String = ""
        Dim id As String = ""
        Dim bResults As Boolean = True

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("id")) Then
                id = HttpContext.Current.Request.Form("id")
            End If

            If IsGUID(token) Then
                bResults = DL.deleteMaintFuelLog(token, id, strError)
            Else
                bResults = False
                strError = "Invalid Token"
            End If

            If bResults = True Then
                strResult = LoadJsonResult("true")
            Else
                strResult = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strResult = "false"
        End Try

        Return strResult

    End Function

    Private Function LoadJsonMaintenanceTasks(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("taskMeassureId")
            json.WriteValue(drv.Item("TaskMeassureId"))

            json.WritePropertyName("taskMeassureName")
            json.WriteValue(drv.Item("MeassureName"))

            json.WritePropertyName("value")
            json.WriteValue(drv.Item("Value"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonMaintenanceTasks", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonMaintTasksMeassures(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonMaintTasksMeassures", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonSchedules(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("deviceId")
            json.WriteValue(drv.Item("DeviceID"))

            json.WritePropertyName("deviceName")
            json.WriteValue(drv.Item("DeviceName"))

            json.WritePropertyName("taskId")
            json.WriteValue(drv.Item("TaskID"))

            json.WritePropertyName("taskName")
            json.WriteValue(drv.Item("TaskName"))

            json.WritePropertyName("taskMeassureId")
            json.WriteValue(drv.Item("TaskMeassureID"))

            json.WritePropertyName("taskMeassureName")
            json.WriteValue(drv.Item("TaskMeassureName"))

            json.WritePropertyName("taskValue")
            json.WriteValue(drv.Item("TaskValue"))

            Dim strDate As String = ""
            strDate = DatePart(DateInterval.Month, drv.Item("LastServiceOn")) & "/" & DatePart(DateInterval.Day, drv.Item("LastServiceOn")) & "/" & DatePart(DateInterval.Year, drv.Item("LastServiceOn"))

            If drv.Item("LastServiceOn") = "1/1/1900" Then
                json.WritePropertyName("lastServiceOn")
                json.WriteValue("N/A")

                json.WritePropertyName("lastServiceOnString")
                json.WriteValue("N/A")
            Else
                json.WritePropertyName("lastServiceOn")
                json.WriteValue(drv.Item("LastServiceOn"))

                json.WritePropertyName("lastServiceOnString")
                json.WriteValue(strDate)
            End If

            json.WritePropertyName("currentValue")
            json.WriteValue(drv.Item("ValueSinceLastService"))

            json.WritePropertyName("notifyBefore")
            json.WriteValue(drv.Item("NotifyBefore"))

            json.WritePropertyName("notifyEveryXDays")
            json.WriteValue(drv.Item("NotifyEveryXDays"))

            json.WritePropertyName("excludeWeekends")
            json.WriteValue(drv.Item("ExcludeWeekends"))

            json.WritePropertyName("createdOn")
            json.WriteValue(drv.Item("CreatedOn"))

            json.WritePropertyName("createdOnString")
            json.WriteValue(drv.Item("CreatedOn").ToString)

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonSchedules", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Private Function LoadJsonMaintAlertUsers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("GUID"))

            json.WritePropertyName("firstName")
            json.WriteValue(drv.Item("FirstName"))

            json.WritePropertyName("lastName")
            json.WriteValue(drv.Item("LastName"))

            json.WritePropertyName("isEmail")
            json.WriteValue(drv.Item("IsEmail"))

            json.WritePropertyName("isSMS")
            json.WriteValue(drv.Item("IsSMS"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonMaintAlertUsers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

#End Region

#Region "Company Info Module"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getCompanyInfo() As String
        Dim token As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getCompanyInfo(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getCompanyInfo", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveCompanyInfo() As String
        Dim token As String = ""
        Dim name As String = ""
        Dim phone As String = ""
        Dim website As String = ""
        Dim industry As String = ""
        Dim street As String = ""
        Dim city As String = ""
        Dim state As String = ""
        Dim postalCode As String = ""
        Dim countryCode As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""
        Dim bOk As Boolean

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("name")) Then
                name = HttpContext.Current.Request.Form("name")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("phone")) Then
                phone = HttpContext.Current.Request.Form("phone")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("website")) Then
                website = HttpContext.Current.Request.Form("website")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("industry")) Then
                industry = HttpContext.Current.Request.Form("industry")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("postalCode")) Then
                postalCode = HttpContext.Current.Request.Form("postalCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("countryCode")) Then
                countryCode = HttpContext.Current.Request.Form("countryCode")
            End If

            '1/19/2013: Lat and Lng will be zero at this point.  When doing the update, the IsGeolocated will be set to zero, and a batch program will calculate this information from Google Maps.
            If Not IsNothing(HttpContext.Current.Request.Form("lat")) Then
                lat = CDec(HttpContext.Current.Request.Form("lat"))
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lng")) Then
                lng = CDec(HttpContext.Current.Request.Form("lng"))
            End If

            If IsGUID(token) Then
                bOk = DL.saveCompanyInfo(token, name, phone, website, industry, street, city, state, postalCode, countryCode, lat, lng, strError)
                If bOk = False Then
                    strError = "Unable to save data"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            Else
                strJson = LoadJsonResult(bOk.ToString.ToLower)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "saveCompanyInfo", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

#End Region

#Region "Billing Info Module"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function getCCInfo() As String
        Dim token As String = ""
        Dim dtData As New DataTable
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strJson = DL.getCCInfo(token, strError)
                If strJson.Length = 0 Then
                    strError = "No data found"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "getCCInfo", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveBillingInfo() As String
        Dim token As String = ""
        Dim billingContact As String = ""
        Dim billingEmail As String = ""
        Dim billingPhone As String = ""
        Dim type As String = ""
        Dim number As String = ""
        Dim secCode As String = ""
        Dim expMonth As Integer = 0
        Dim expYear As Integer = 0
        Dim fName As String = ""
        Dim lName As String = ""
        Dim street As String = ""
        Dim city As String = ""
        Dim state As String = ""
        Dim postalCode As String = ""
        Dim countryCode As String = ""
        Dim DL As New DataLayer
        Dim strError As String = ""
        Dim strJson As String = ""
        Dim bOk As Boolean

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("billingContact")) Then
                billingContact = HttpContext.Current.Request.Form("billingContact")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("billingEmail")) Then
                billingEmail = HttpContext.Current.Request.Form("billingEmail")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("billingPhone")) Then
                billingPhone = HttpContext.Current.Request.Form("billingPhone")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("type")) Then
                type = HttpContext.Current.Request.Form("type")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("number")) Then
                number = HttpContext.Current.Request.Form("number")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("secCode")) Then
                secCode = HttpContext.Current.Request.Form("secCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("expMonth")) Then
                If IsNumeric(HttpContext.Current.Request.Form("expMonth")) Then
                    expMonth = CInt(HttpContext.Current.Request.Form("expMonth"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("expYear")) Then
                If IsNumeric(HttpContext.Current.Request.Form("expYear")) Then
                    expYear = CInt(HttpContext.Current.Request.Form("expYear"))
                End If
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("fName")) Then
                fName = HttpContext.Current.Request.Form("fName")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("lName")) Then
                lName = HttpContext.Current.Request.Form("lName")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("postalCode")) Then
                postalCode = HttpContext.Current.Request.Form("postalCode")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("countryCode")) Then
                countryCode = HttpContext.Current.Request.Form("countryCode")
            End If

            If IsGUID(token) Then
                bOk = DL.saveBillingInfo(token, billingContact, billingEmail, billingPhone, type, number, secCode, expMonth, expYear, fName, lName, street, city, state, postalCode, countryCode, strError)
                If bOk = False Then
                    strError = "Unable to save data"
                End If
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            Else
                strJson = LoadJsonResult(bOk.ToString.ToLower)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "saveBillingInfo", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

#End Region

#Region "Mobile - Public Methods"

#End Region

#Region "Snooze Report"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function snoozeReport() As String
        Dim rpt As String = ""
        Dim usr As String = ""
        Dim period As Integer = 0
        Dim DL As New DataLayer
        Dim bResult As Boolean = True
        Dim strError As String = ""
        Dim strJson As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("rpt")) Then
                rpt = HttpContext.Current.Request.Form("rpt")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("usr")) Then
                usr = HttpContext.Current.Request.Form("usr")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("period")) Then
                If IsNumeric(HttpContext.Current.Request.Form("period")) Then
                    period = CInt(HttpContext.Current.Request.Form("period"))
                Else
                    period = 0
                End If
            End If

            If IsGUID(usr) Then
                bResult = DL.snoozeReport(rpt, usr, period)
                strJson = LoadJsonResult("result", bResult.ToString)
            Else
                strError = "Invalid Token"
                strJson = ""
            End If

            If strError.Length > 0 Then
                strJson = LoadJsonError(strError)
            End If

        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "snoozeReport", "", ex.Message & " - usr: " & usr, 0)
        End Try

        Return strJson

    End Function

#End Region

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function uploadFile() As String
        Dim strJson As String = ""
        Dim token As String = ""

        Dim file As HttpPostedFile

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("token")) Then
                token = HttpContext.Current.Request.Form("token")
            End If

            file = HttpContext.Current.Request.Files(0)
            If file IsNot Nothing AndAlso file.ContentLength > 0 Then
                Dim fname As String = Path.GetFileName(file.FileName)
                file.SaveAs(Server.MapPath(Path.Combine("~/App_Data/", fname)))
            End If

            If HttpContext.Current.Request.Files.Count > 0 Then
                Dim path__1 As String = HttpContext.Current.Server.MapPath("~/Temp")
                If Not Directory.Exists(path__1) Then
                    Directory.CreateDirectory(path__1)
                End If

                file = HttpContext.Current.Request.Files(0)

                Dim fileName As String

                If HttpContext.Current.Request.Browser.Browser.ToUpper() = "IE" Then
                    Dim files As String() = file.FileName.Split(New Char() {"\"c})
                    fileName = files(files.Length - 1)
                Else
                    fileName = file.FileName
                End If
                Dim strFileName As String = fileName
                fileName = Path.Combine(path__1, fileName)
                file.SaveAs(fileName)


                Dim msg As String = "{"
                msg += String.Format("error:'{0}'," & vbLf, String.Empty)
                msg += String.Format("msg:'{0}'" & vbLf, strFileName)
                msg += "}"


                HttpContext.Current.Response.Write(msg)

            End If


            strJson = LoadJsonError("TEST")
        Catch ex As Exception
            strJson = LoadJsonError(ex.Message)
            BLErrorHandling.ErrorCapture(pSysModule, "uploadFile", "", ex.Message & " - Token: " & token, 0)
        End Try

        Return strJson

    End Function

#Region "Fuel Card log upload"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function saveFuelLogUpload() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim bResult As Boolean = True
        Dim dl As New DataLayer
        Dim strError As String = ""

        Dim device As String = ""
        Dim cardNumber As String = ""
        Dim merchantName As String = ""
        Dim logDate As String = ""
        Dim logTime As String = ""
        Dim driver As String = ""
        Dim street As String = ""
        Dim city As String = ""
        Dim state As String = ""
        Dim zip As String = ""
        Dim galls As Decimal = 0
        Dim price As Decimal = 0
        Dim amt As Decimal = 0
        Dim msg As String = ""
        Dim odometer As Decimal = 0
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("device")) Then
                device = HttpContext.Current.Request.Form("device")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("cardNumber")) Then
                cardNumber = HttpContext.Current.Request.Form("cardNumber")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("merchantName")) Then
                merchantName = HttpContext.Current.Request.Form("merchantName")
            End If


            If Not IsNothing(HttpContext.Current.Request.Form("date")) Then
                logDate = HttpContext.Current.Request.Form("date")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("time")) Then
                logTime = HttpContext.Current.Request.Form("time")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("driver")) Then
                driver = HttpContext.Current.Request.Form("driver")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("street")) Then
                street = HttpContext.Current.Request.Form("street")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("city")) Then
                city = HttpContext.Current.Request.Form("city")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("state")) Then
                state = HttpContext.Current.Request.Form("state")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("zip")) Then
                zip = HttpContext.Current.Request.Form("zip")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("galls")) Then
                If IsNumeric(HttpContext.Current.Request.Form("galls")) Then
                    galls = CDec(HttpContext.Current.Request.Form("galls"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("price")) Then
                If IsNumeric(HttpContext.Current.Request.Form("price")) Then
                    price = CDec(HttpContext.Current.Request.Form("price"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("amt")) Then
                If IsNumeric(HttpContext.Current.Request.Form("amt")) Then
                    amt = CDec(HttpContext.Current.Request.Form("amt"))
                End If
            End If

            If price = 0 And amt <> 0 And galls <> 0 Then
                price = amt / galls
            End If


            If Not IsNothing(HttpContext.Current.Request.Form("odometer")) Then
                If IsNumeric(HttpContext.Current.Request.Form("odometer")) Then
                    odometer = CDec(HttpContext.Current.Request.Form("odometer"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("lat")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lat")) Then
                    lat = CDec(HttpContext.Current.Request.Form("lat"))
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("lng")) Then
                If IsNumeric(HttpContext.Current.Request.Form("lng")) Then
                    lng = CDec(HttpContext.Current.Request.Form("lng"))
                End If
            End If


            If IsGUID(token) Then
                bResult = dl.saveFuelLogUpload(token, device, logDate, logTime, driver, street, city, state, zip, galls, price, amt, cardNumber, merchantName, odometer, lat, lng, msg)
            Else
                bResult = False
                strError = LoadJsonError("Invalid Token")
            End If

            If bResult = True Then
                strResult = LoadJsonResult("result", "OK")
            Else
                strResult = LoadJsonError(msg)
            End If

        Catch ex As Exception

        End Try


        Return strResult

    End Function

#End Region

End Class


'<WebMethod()> _
'<ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
'Public Function getGeofences() As String
'    Dim token As String = ""
'    Dim dtData As New DataTable
'    Dim DL As New DataLayer
'    Dim strError As String = ""
'    Dim strJson As String = ""

'    Try
'        If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
'            token = HttpContext.Current.Request.Form("t")
'        End If

'        If IsGUID(token) Then
'            dtData = DL.getGeofences(token, strError)
'        Else
'            dtData = Nothing
'            strError = "Invalid Token"
'        End If

'        If Not IsNothing(dtData) Then

'            'Build json array
'            Dim sb As New StringBuilder
'            Dim sw As New StringWriter(sb)
'            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)
'            Dim drv As DataRowView

'            json.WriteStartObject()

'            'List of units
'            json.WritePropertyName("geofences")
'            json.WriteStartArray()

'            For Each drv In dtData.DefaultView
'                json.WriteValue(LoadJsonGeofences(drv))
'            Next

'            json.WriteEnd()
'            json.WriteEndObject()
'            json.Flush()
'            strJson = sb.ToString
'        Else
'            strJson = LoadJsonError(strError)
'        End If

'    Catch ex As Exception
'        strJson = LoadJsonError(ex.Message)
'        BLErrorHandling.ErrorCapture(pSysModule, "getGeofences", "", ex.Message  & " - Token: " & token, 0)
'    End Try

'    Return strJson

'End Function
