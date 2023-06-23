' NOTE: You can use the "Rename" command on the context menu to change the class name "ws" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select ws.svc or ws.svc.vb at the Solution Explorer and start debugging.
Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation.WebScriptServiceHostFactory
Imports System.Data
Imports System.Web.Script.Serialization
Imports etws.BLCommon

Imports System.IO
Imports Newtonsoft.Json

Public Class ws
    Implements Iws

    Public Sub DoWork() Implements Iws.DoWork
    End Sub

    Function saveForm(ByVal data As webForm) As responseOk Implements Iws.saveForm
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveWebForm(data.formId, data.qty, data.serviceId, data.ship, data.fn, data.ln, data.email, data.ph, data.cell,
                                      data.co, data.street, data.city, data.state, data.postalCode, data.ccType, data.ccNo, data.ccSec, data.ccMonth, data.ccYear, data.ccFn, data.ccLn,
                                      data.ccStreet, data.ccCity, data.ccState, data.ccPostal, data.msg, data.promoCode, data.repId, data.isOBDOption, data.isPostedSLOption)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function saveForm2(ByVal data As webForm) As responseOk Implements Iws.saveForm2
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveWebForm_v2(data.formId, data.qtyGX, data.qtyTF, data.qtyOBDTracker, data.qtyAssets, data.serviceId, data.ship, data.fn, data.ln, data.email, data.ph, data.cell, data.co, data.street, data.city, data.state, data.postalCode, data.ccType, data.ccNo, data.ccSec, data.ccMonth, data.ccYear, data.ccFn, data.ccLn, data.ccStreet, data.ccCity, data.ccState, data.ccPostal, data.msg, data.promoCode, data.repId, data.qtyOBDConnector, data.isPostedSLOption)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function getDocQty(ByVal docId As String) As qtyDoc Implements Iws.getDocQty
        Dim itm As New qtyDoc
        Dim dl As New DataLayer

        Try
            itm = dl.getDocQty(docId)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Function getShoppingCartInfo(ByVal token As String) As webForm Implements Iws.getShoppingCartInfo
        Dim wf As New webForm
        Dim dl As New DataLayer

        Try
            wf = dl.getShoppingCartInfo(token)
        Catch ex As Exception

        End Try

        Return wf

    End Function

    Function getCompanyInfo(ByVal token As String) As companyInfo Implements Iws.getCompanyInfo
        Dim c As New companyInfo
        Dim dl As New DataLayer

        Try
            c = dl.getCompanyInfo(token)
        Catch ex As Exception

        End Try

        Return c

    End Function

    Function getCompanyByUID(ByVal uid As String) As companyInfo2 Implements Iws.getCompanyByUID
        Dim itm As New companyInfo2
        Dim dl As New DataLayer
        Dim err As String = ""

        Try
            itm = dl.getCompanyInfo2(uid, err)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Function saveCompanyInfo(ByVal token As String, ByVal data As companyInfo) As responseOk Implements Iws.saveCompanyInfo
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveCompanyInfo(token, data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function getCCInfo(ByVal token As String) As ccInfo Implements Iws.getCCInfo
        Dim cc As New ccInfo
        Dim dl As New DataLayer

        Try
            cc = dl.getCCInfo(token)
        Catch ex As Exception

        End Try

        Return cc

    End Function

    Function saveBillingInfo(ByVal token As String, ByVal data As ccInfo) As responseOk Implements Iws.saveBillingInfo
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveBillingInfo(token, data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function getQuote(ByVal data As quoteForm) As responseOk Implements Iws.getQuote
        Dim res As New responseOk
        Dim dl As New DataLayer
        Try
            res.isOk = dl.saveWebForm(data.formId, data.qty, data.serviceId, data.ship, data.fn, data.ln, data.email, data.ph, "", data.co, "", "", "", "", "", "", "", 0, 0, "", "", "", "", "", "", "", "", "", False, False)
        Catch ex As Exception

        End Try
        Return res
    End Function

    Function getBasePrice() As priceList Implements Iws.getBasePrice
        Dim itm As New priceList
        Dim dl As New DataLayer

        Try
            itm = dl.getBasePrice
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Function getCameras(ByVal data As wsRequest) As wsCamerasResponse Implements Iws.getCameras
        Dim itm As New wsCamerasResponse
        Dim dl As New DataLayer
        Try
            itm = dl.getCameras(data)
        Catch ex As Exception

        End Try

        Return itm

    End Function

#Region "Devices Commands OTA"

    Function sendDeviceCommand(ByVal data As deviceCommand) As responseOk Implements Iws.sendDeviceCommand
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.sendDeviceCommand(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function getDeviceResponses(ByVal deviceId As String) As List(Of deviceResponse) Implements Iws.getDeviceResponses
        Dim lst As New List(Of deviceResponse)
        Dim dl As New DataLayer

        Try

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function saveDeviceEvent(ByVal data As class_parsedMessage) As etwsResponse Implements Iws.saveDeviceEvent
        Dim dl As New dataLayerAPI
        Dim itm As New etwsResponse

        Try
            itm = dl.HDevices_INSERT(data)
        Catch ex As Exception

        End Try

        Return itm

    End Function

#End Region

#Region "TFTP Automation"

    Function getCfgScripts() As List(Of cfgFile) Implements Iws.getCfgScripts
        Dim lst As New List(Of cfgFile)
        Dim dl As New DataLayer

        Try
            lst = dl.getCfgScripts()
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function saveCfgScript(ByVal data As cfgFile) As responseOk Implements Iws.saveCfgScript
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.saveCfgScript(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function resetCfgScript(ByVal id As String) As responseOk Implements Iws.resetCfgScript
        Dim res As New responseOk
        Dim dl As New DataLayer
        Dim intID As Integer

        Try
            If IsNumeric(id) Then
                intID = CInt(id)
                res = dl.resetCfgScript(id)
            End If
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "CRM"
    Public Function crmGetDeviceData(ByVal t As String, ByVal did As String) As String Implements Iws.crmGetDeviceData
        Dim strResult As String = ""
        Dim token As String = ""
        ''Dim pdid As String = ""
        Dim dl As New DataLayer
        'Dim parameter = JsonConvert.DeserializeObject(Data)
        Try
            If IsGUID(t) Then
                strResult = dl.CRM_HDevices_GET(t, did)
            End If

            If strResult = "" Then
                strResult = "false"
            End If
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("crmGetDeviceData", "crmGetDeviceData", "", ex.Message, 0)
        End Try

        Return strResult
    End Function


#End Region

#Region "Sensors"
    Public Function PostTempSensorAdd(ByVal token As String, ByVal data As TempSensors) As responseSensor Implements Iws.PostTempSensorAdd
        Dim dl As New DataLayer
        Dim resonse As New responseSensor
        resonse.isOk = dl.PostTempSensor(token, data, 1)
        Return resonse

    End Function
    Public Function UpdateTempSensorUpdate(ByVal token As String, ByVal data As TempSensors) As responseSensor Implements Iws.UpdateTempSensorUpdate
        Dim dl As New DataLayer
        Dim resonse As New responseSensor
        resonse.isOk = dl.PostTempSensor(token, data, data.Action)
        Return resonse

    End Function
    Public Function GetTempSensors(ByVal token As String) As List(Of TempSensors) Implements Iws.GetTempSensors
        Dim dl As New DataLayer
        Dim sensors As New List(Of TempSensors)
        Dim resonse As New responseSensor
        sensors = dl.GetSensors2(token)
        Return sensors

    End Function


#End Region

#Region "TrakingNumber"
    Public Function GettrakingnumberExt(ByVal trakingnumber As String) As TrakingNumberExt Implements Iws.GettrakingnumberExt
        Dim dl As New DataLayer
        Dim resonse As New TrakingNumberExt
        resonse = dl.GettrakingnumberExt(trakingnumber)
        Return resonse

    End Function
    Public Function Gettrakingnumber(ByVal token As String) As List(Of TrakingNumber) Implements Iws.Gettrakingnumber
        Dim dl As New DataLayer
        Dim list As New List(Of TrakingNumber)
        list = dl.Gettrakingnumber(token)
        Return list

    End Function
    Public Function Puttrakingnumber(ByVal token As String, ByVal data As TrakingNumber) As responseSensor Implements Iws.Puttrakingnumber
        Dim dl As New DataLayer
        Dim resonse As New responseSensor
        resonse.isOk = dl.Puttrakingnumber(token, data, 3)
        Return resonse

    End Function
    Public Function Posttrakingnumber(ByVal token As String, ByVal data As TrakingNumber) As responseSensor Implements Iws.Posttrakingnumber
        Dim dl As New DataLayer
        Dim resonse As New responseSensor
        Dim sGUID As String
        sGUID = System.Guid.NewGuid.ToString()
        data.TrackingNumber = sGUID
        data.URLTraking = "trakingnumber.html?guid=" + sGUID
        Dim email As String = "<!DOCTYPE html>"
        email += "<html>"
        email += "<head>"
        email += "<meta http-equiv=Content-Type content=text/html; charset=UTF-8/>"
        email += "<meta name=viewport content=width=device-width, initial-scale=1.0>"
        email += "<link href =https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css rel=stylesheet integrity=sha384-EVSTQN3/azprG1Anm3QDgpJLIm9Nao0Yz1ztcQTwFspd3yD65VohhpuuCOmLASjC crossorigin=anonymous>"
        email += "</head>"
        email += "<body>"
        email += "<div class=container>"
        email += "<div class=row>"
        email += "<div class=col-12>"
        email += "<h1>Traking Number</h1><br>"
        email += "<label>Hello,<br>A new tracking number was created for the device " + data.DeviceName + "</label>"
        email += "</div>"
        email += "</div>"
        email += "<div class=row>"
        email += "<div Class=col-12>"
        email += " <p>observations:</p>" + data.Message + "<br>"
        email += " <span>To view the tracking, visit the following link</span><br>"
        email += "<a href=https://pre.easitrack.net/trakingnumber.html?guid=" + sGUID + "><span>View Traking</span></a><br>"
        email += "</div>"
        email += "</div>"
        email += "<div class=row>"
        email += "<div Class=col-12><img src=https://pre.easitrack.net/images/A7.png width=200 height=200></div>"
        email += "</div>"
        email += "</div>"
        email += "<script src=https://code.jquery.com/jquery-3.2.1.slim.min.js integrity=sha384-KJ3o2DKtIkvYIK3UENzmM7KCkRr/rE9/Qpg6aAZGJwFDMVNA/GpGFF93hXpG5KkN crossorigin=anonymous></script>"
        email += "<script src=https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.2/dist/umd/popper.min.js integrity=sha384-IQsoLXl5PILFhosVNubq5LC7Qb9DXgDA9i+tQ8Zj3iwWAwPtgFTxbJ8NT4GN1R8p crossorigin=anonymous></script>"
        email += "<script src=https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/js/bootstrap.min.js integrity=sha384-cVKIPhGWiC2Al4u+LWgxfKTRIcfu0JTxR+EQDz/bgldoEyl4H0zUF0QKbrJ0EcQF crossorigin=anonymous></script>"
        email += "</body>"
        email += "</html>"
        resonse.isOk = dl.Posttrakingnumber(token, data, 1, email)
        Return resonse

    End Function

#End Region




End Class
