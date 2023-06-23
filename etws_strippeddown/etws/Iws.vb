Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation

Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Iws" in both code and config file together.
<ServiceContract()>
Public Interface Iws

    <OperationContract()>
    Sub DoWork()

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveWebForm")>
    Function saveForm(ByVal data As webForm) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveWebForm2")>
    Function saveForm2(ByVal data As webForm) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDocQty?docId={docId}")>
    Function getDocQty(ByVal docId As String) As qtyDoc

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getShoppingCartInfo?token={token}")>
    Function getShoppingCartInfo(ByVal token As String) As webForm

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompanyInfo?token={token}")>
    Function getCompanyInfo(ByVal token As String) As companyInfo

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompanyByUID?uid={uid}")>
    Function getCompanyByUID(ByVal uid As String) As companyInfo2

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveCompanyInfo?token={token}")>
    Function saveCompanyInfo(ByVal token As String, ByVal data As companyInfo) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCCInfo?token={token}")>
    Function getCCInfo(ByVal token As String) As ccInfo

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveBillingInfo?token={token}")>
    Function saveBillingInfo(ByVal token As String, ByVal data As ccInfo) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getQuote")>
    Function getQuote(ByVal data As quoteForm) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getPrice")>
    Function getBasePrice() As priceList

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCameras")>
    Function getCameras(ByVal data As wsRequest) As wsCamerasResponse

#Region "Devices Commands OTA"

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="sendDeviceCommand")>
    Function sendDeviceCommand(ByVal data As deviceCommand) As responseOk

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDeviceResponses?deviceId={deviceId}")>
    Function getDeviceResponses(ByVal deviceId As String) As List(Of deviceResponse)

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="api/v1/device/event/save")>
    Function saveDeviceEvent(ByVal data As class_parsedMessage) As etwsResponse

#End Region

#Region "TFTP Automation"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getScripts")>
    Function getCfgScripts() As List(Of cfgFile)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveScript")>
    Function saveCfgScript(ByVal data As cfgFile) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="resetScriptStatus/{id}")>
    Function resetCfgScript(ByVal id As String) As responseOk

#End Region

#Region "crm"
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="crmGetDeviceData?t={t}&did={did}")>
    Function crmGetDeviceData(ByVal t As String, ByVal did As String) As String

#End Region
#Region "Sensors"
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="savesensor/{token}")>
    Function PostTempSensorAdd(ByVal token As String, ByVal data As TempSensors) As responseSensor
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updatesensor/{token}")>
    Function UpdateTempSensorUpdate(ByVal token As String, ByVal data As TempSensors) As responseSensor
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="sensors/{token}")>
    Function GetTempSensors(ByVal token As String) As List(Of TempSensors)


#End Region
#Region "trakingnumber"
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="gettn/{trakingnumber}")>
    Function GettrakingnumberExt(ByVal trakingnumber As String) As TrakingNumberExt
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getlisttn/{token}")>
    Function Gettrakingnumber(ByVal token As String) As List(Of TrakingNumber)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updatetn/{token}")>
    Function Puttrakingnumber(ByVal token As String, ByVal data As TrakingNumber) As responseSensor

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="savetn/{token}")>
    Function Posttrakingnumber(ByVal token As String, ByVal data As TrakingNumber) As responseSensor

#End Region
End Interface
