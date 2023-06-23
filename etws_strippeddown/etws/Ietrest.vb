Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation

Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Ietrest" in both code and config file together.
<ServiceContract()> _
Public Interface Ietrest

#Region "API"

#Region "Get company users"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="users/{token}/{sourceId}")> _
    Function GetCompanyUsers(ByVal token As String, ByVal sourceId As String) As List(Of user2)

#End Region

#Region "Mobile Devices"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Wrapped, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="authorization?login={login}&password={password}&expDays={expDays}")> _
    Function authorization(ByVal login As String, ByVal password As String, ByVal expDays As String) As user

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Wrapped, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="authorization2?login={login}&password={password}&expDays={expDays}&sourceId={sourceId}&sourceExt={sourceExt}")> _
    Function authorization2(ByVal login As String, ByVal password As String, ByVal expDays As String, ByVal sourceId As String, ByVal sourceExt As String) As user

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devList/{token}")> _
    Function GetDevicesList(ByVal token As String) As List(Of FleetDeviceVideo)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devListCORS/{token}?callback={callback}")> _
    Function GetDevicesListCORS(ByVal token As String, ByVal callback As String) As List(Of FleetDeviceVideo)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devList2/{token}/{sourceId}")> _
    Function GetDevicesList2(ByVal token As String, ByVal sourceId As String) As List(Of FleetDeviceVideo)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/{id}")> _
    Function GetDeviceInfo(ByVal id As String) As FleetDeviceVideo

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device2/{id}/{sourceId}")> _
    Function GetDeviceInfo2(ByVal id As String, ByVal sourceId As String) As FleetDeviceVideo

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Wrapped, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="trail?id={id}&trailDate={trailDate}")> _
    Function GetTrail(ByVal id As String, ByVal trailDate As String) As List(Of Trail)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Wrapped, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="trail2?id={id}&trailDate={trailDate}&sourceId={sourceId}")> _
    Function GetTrail2(ByVal id As String, ByVal trailDate As String, ByVal sourceId As String) As List(Of Trail)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Wrapped, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="trail3?id={id}&trailDate={trailDate}&hourFrom={hourFrom}&hourTo={hourTo}&sourceId={sourceId}")> _
    Function GetTrail3(ByVal id As String, ByVal trailDate As String, ByVal hourFrom As String, ByVal hourTo As String, ByVal sourceId As String) As List(Of Trail)

#End Region

#Region "Embedded map add-on"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="easiTrackMap/{token}?callback={callback}")> _
    Function easiTrackMap(ByVal token As String, ByVal callback As String) As List(Of FleetDeviceVideo)

#End Region

#Region "eTrack API - Devices / Enterprise Security API"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevices/{token}")> _
    Function GetDevices(ByVal token As String) As List(Of device2)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevices/{token}/{deviceId}")> _
    Function GetDevice(ByVal token As String, ByVal deviceId As String) As device2

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getHistory/{token}/{deviceId}?dateFrom={dateFrom}&dateTo={dateTo}")> _
    Function GetHistory(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As Date, ByVal dateTo As Date) As List(Of History)

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="api/v1/getHistory2?token={token}&deviceId={deviceId}&dateFrom={dateFrom}&dateTo={dateTo}")>
    Function getHistory2(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String) As List(Of class_DeviceHistory)

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="api/v1/getNathan?token={token}&deviceId={deviceId}&pointer={pointer}")>
    Function getNathan(ByVal token As String, ByVal deviceId As String, ByVal pointer As String) As List(Of class_DeviceHistory)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofences/{token}")> _
    Function GetGeofences(ByVal token As String) As List(Of Geofence)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofences/{token}/{geofenceId}")> _
    Function GetGeofence(ByVal token As String, ByVal geofenceId As String) As Geofence

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofenceAlertStatus/{token}/{geofenceId}")> _
    Function GetGeofenceAlertStatus(ByVal token As String, ByVal geofenceId As String) As GeofenceAlertStatus

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofenceCrossings/{token}/{geofenceId}?dateFrom={dateFrom}&dateTo={dateTo}")> _
    Function GetGeofenceCrossings(ByVal token As String, ByVal geofenceId As String, ByVal dateFrom As Date, ByVal dateTo As Date) As List(Of GeofenceCrossings)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofenceSpeedLimitStatus/{token}/{geofenceId}")> _
    Function GetGeofenceSpeedLimitStatus(ByVal token As String, ByVal geofenceId As String) As GeofenceSpeedLimitStatus

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deleteGeofence")> _
    Function DeleteGeofence(ByVal data As GeofenceRequest) As GeofenceResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="setGeofenceAlertType")> _
    Function SetGeofenceAlertType(ByVal data As GeofenceAlertTypeRequest) As GeofenceResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="setGeofenceSpeedLimitStatus")> _
    Function SetGeofenceSpeedLimitStatus(ByVal data As GeofenceSpeedLimitRequest) As GeofenceResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="addGeofence")> _
    Function addGeofence(ByVal data As geofenceUpdate) As GeofenceResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateGeofence")> _
    Function updateGeofence(ByVal data As geofenceUpdate) As GeofenceResponse

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofenceTypes/{token}")> _
    Function GetGeofenceTypes(ByVal token As String) As List(Of GeofenceTypeResponse)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="addGeofenceType")> _
    Function addGeofenceType(ByVal data As GeofenceType) As etResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateGeofenceType")> _
    Function updateGeofenceType(ByVal data As GeofenceType) As etResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deleteGeofenceType/{token}/{id}")> _
    Function deleteGeofenceType(ByVal token As String, ByVal id As String) As etResponse

#End Region

#Region "Geofences"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="isPointInGeofence/{token}/{lat}/{lng}")> _
    Function isPointInGeofence(ByVal token As String, ByVal lat As String, ByVal lng As String) As pointInGeofence

#End Region

#Region "AWS"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="awsBouncesPOST")> _
    Function awsBouncesPOST(ByVal data As awsSES) As awsSES

#End Region

#Region "WLIUS ELOG"

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="wlius/key/{token}")>
    Function getWliusApiKey(ByVal token As String) As etWliusResponse

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hvideo/key/{token}")>
    Function getHasVideoApiKey(ByVal token As String) As etHvideoResponse

#End Region

#End Region

#Region "FDT Services"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="fdtContact")> _
    Function fdtContact(ByVal data As contactForm) As responseOk

#End Region

#Region "ET site services"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="contactForm")> _
    Function ContactFrm(ByVal data As contactForm) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="quickContact")> _
    Function quickContactFrm(ByVal data As quickContact) As responseOk

#End Region

#Region "Quick Message"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getQuickMsgDriversList/{token}/{noCache}")>
    Function getQuickMsgDriversList(ByVal token As String, ByVal noCache As String) As List(Of quickMsgDriver)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="quickMsg/{token}")>
    Function sendQuickMsg(ByVal token As String, ByVal data As quickMsg) As responseOk

#End Region

#Region "CRM"

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="api/v1/crmcase/message/add")>
    Function crmCasesMessageAdd(ByVal data As etAddCrmMessageRequest) As etResponse


#End Region

#Region "Reports"
    '<OperationContract()>
    '<WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="reports?token={token}&reportID={reportID}&deviceID={deviceID}&dateFrom={dateFrom}&dateTo={dateTo}&hourFrom={hourFrom}&hourTo={hourTo}&isBatch={isBatch}")>
    ''Function getTemperatureLog(ByVal token As String, ByVal reportID As String, ByVal deviceID As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal hourFrom As String, ByVal hourTo As String, ByVal isBatch As String) As String ' List(Of jsonTempeatureLog)
    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="reports?token={token}&IsBatch={IsBatch}&RecurrentID={RecurrentID}&CompanyID={CompanyID}&GroupID={GroupID}&IsAllDevices={IsAllDevices}&ExcludeWeekends={ExcludeWeekends}&ReportID={ReportID}&DeviceID={DeviceID}&DateFrom={DateFrom}&DateTo={DateTo}&HourFrom={HourFrom}&HourTo={HourTo}&ThisDayOfWeek={ThisDayOfWeek}&Param={Param}&Param2={Param2}&IsForExport={IsForExport}")>
    Function getReportNew(ByVal Token As String, ByVal IsBatch As Integer, ByVal RecurrentID As Integer, ByVal CompanyID As Integer, ByVal GroupID As Integer, ByVal IsAllDevices As Integer, ByVal ExcludeWeekends As Integer, ByVal ReportID As Integer, ByVal DeviceID As String, ByVal DateFrom As String, ByVal DateTo As String, ByVal HourFrom As String, ByVal HourTo As String, ByVal ThisDayOfWeek As Integer, ByVal Param As String, ByVal Param2 As String, ByVal IsForExport As Integer) As String

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevicesEvents?token={token}")>
    Function CompaniesDevicesEvents(ByVal Token As String) As String

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofences?token={token}")>
    Function GeofencesAll(ByVal Token As String) As String

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDrivers?token={token}")>
    Function DriversAll(ByVal Token As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postSendFeedBack?token={token}&visitedPage={visitedPage}&type={type}&description={description}")>
    Function postSendFeedBack(token As String, visitedPage As String, type As String, description As String) As String

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="GetFeedbakTypes?token={token}")>
    Function GetFeedbakTypes(ByVal token As String) As String



#End Region

#Region "Video"
    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="sso/video/{token}?ep={ep}")>
    Function ValidateTokenVideo(ByVal token As String, ByVal ep As Boolean) As CustomerVideoEp
    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="sso/videoapp/{token}?ep={ep}")>
    Function ValidateTokenVideoApp(ByVal token As String, ByVal ep As Boolean) As CustomerVideo

#End Region
End Interface


