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


' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="https://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class ETCRMWS
    Inherits System.Web.Services.WebService

    Private pSysModule As String = "ETCRMWS.asmx"

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmValidateCredentials() As String
        Dim dl As New crmDataLayer
        Dim strResult As String = ""
        Dim login As String = ""
        Dim pw As String = ""
        Dim msg As String = ""

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("login")) Then
                login = HttpContext.Current.Request.Form("login")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("pw")) Then
                pw = HttpContext.Current.Request.Form("pw")
            End If

            strResult = dl.CRM_Users_ValidateCredentials(login, pw, msg)

            If strResult = "" Then
                strResult = LoadJsonError(msg)
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmValidateCredentials", "Login: " & login & " - PW: " & pw, ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmValidateToken() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strResult = dl.CRM_Users_ValidateToken(token)
            End If

            If strResult = "" Then
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmValidateToken", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmGetCustomers() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If

            If IsGUID(token) Then
                strResult = dl.CRM_Customers_GET(token)
            End If

            If strResult = "" Then
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmGetCustomers", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmGetCustomerByUniqueKey() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim uid As String = ""
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("uid")) Then
                uid = HttpContext.Current.Request.Form("uid")
            End If

            If IsGUID(token) Then
                strResult = dl.CRM_Customers_GetByUniqueKey(token, uid)
            End If

            If strResult = "" Then
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmGetCustomerByUniqueKey", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmGetDeviceData() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim did As String = ""
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("did")) Then
                did = HttpContext.Current.Request.Form("did")
            End If

            If IsGUID(token) Then
                strResult = dl.CRM_HDevices_GET(token, did)
            End If

            If strResult = "" Then
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmGetDeviceData", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmCustomerSaveBillingSpecs() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim uid As String = ""
        Dim paymentMethod As String = ""
        Dim billingDay As String = ""
        Dim isVVIP As Boolean = False
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("uid")) Then
                uid = HttpContext.Current.Request.Form("uid")
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("pm")) Then
                If IsNumeric(HttpContext.Current.Request.Form("pm")) Then
                    paymentMethod = HttpContext.Current.Request.Form("pm")
                Else
                    paymentMethod = 1
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("isVVIP")) Then
                If HttpContext.Current.Request.Form("isVVIP").ToLower = "true" Then
                    isVVIP = True
                End If
            End If

            If Not IsNothing(HttpContext.Current.Request.Form("bd")) Then
                If IsNumeric(HttpContext.Current.Request.Form("bd")) Then
                    billingDay = HttpContext.Current.Request.Form("bd")
                Else
                    billingDay = 1
                End If
            End If


            If IsGUID(token) Then
                strResult = dl.crmCustomerSaveBillingSpecs(token, uid, paymentMethod, billingDay, isVVIP)
            End If

            If strResult = "" Then
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmCustomerSaveBillingDet", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function crmCustomerSnoozeCollections() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim uid As String = ""
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("uid")) Then
                uid = HttpContext.Current.Request.Form("uid")
            End If

            If IsGUID(token) Then
                strResult = dl.crmCustomerSnoozeCollections(token, uid)
            End If

            If strResult = "" Then
                strResult = "false"
            Else
                strResult = "{res:'" & strResult & "'}"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "crmCustomerSnoozeCollections", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

    <WebMethod()> _
   <ScriptMethod(ResponseFormat:=ResponseFormat.Json)> _
    Public Function sendCredentialsEmail() As String
        Dim strResult As String = ""
        Dim token As String = ""
        Dim guid As String = ""
        Dim dl As New crmDataLayer

        Try
            If Not IsNothing(HttpContext.Current.Request.Form("t")) Then
                token = HttpContext.Current.Request.Form("t")
            End If
            If Not IsNothing(HttpContext.Current.Request.Form("guid")) Then
                guid = HttpContext.Current.Request.Form("guid")
            End If

            If IsGUID(token) Then
                strResult = dl.sendCredentialsEmail(token, guid)
            End If

            If strResult = "" Then
                strResult = "false"
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "sendCredentialsEmail", "", ex.Message, 0)
        End Try

        Return strResult

    End Function

End Class