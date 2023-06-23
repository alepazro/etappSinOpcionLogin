Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation

Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IeTrack" in both code and config file together.
<ServiceContract()>
Public Interface IeTrack

#Region "Authentication"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="validateCredentials?login={login}&password={password}")>
    Function validateCredentials(ByVal login As String, ByVal password As String) As webUser

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="validateToken?token={token}&sourcePage={sourcePage}&sourceId={sourceId}")>
    Function validateToken(ByVal token As String, ByVal sourcePage As String, ByVal sourceId As String) As webUser

#End Region

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devList/{token}")>
    Function GetDevicesList(ByVal token As String) As List(Of deviceDet)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/{id}")>
    Function GetDeviceInfo(ByVal id As String) As deviceDet

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deviceInfo/{token}/{deviceId}")>
    Function GetDeviceById(ByVal token As String, ByVal deviceId As String) As deviceDet

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevices/{token}?lastFetchOn={lastFetchOn}&qtyPanels={qtyPanels}&devicesPerPanel={devicesPerPanel}")>
    Function GetDevices(ByVal token As String, ByVal lastFetchOn As String, ByVal qtyPanels As String, ByVal devicesPerPanel As String) As List(Of deviceDet)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevicesbrokers/{token}?lastFetchOn={lastFetchOn}&qtyPanels={qtyPanels}&devicesPerPanel={devicesPerPanel}")>
    Function GetDevicesBrokerOrder(ByVal token As String, ByVal lastFetchOn As String, ByVal qtyPanels As String, ByVal devicesPerPanel As String) As List(Of deviceDet)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getMaintSchedules/{token}?deviceId={deviceId}&taskId={taskId}")>
    Function GetMaintScheduled(ByVal token As String, ByVal deviceId As String, ByVal taskId As String) As List(Of maintSchedule)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getUsersBasicInfo/{token}")>
    Function GetUsersBasicInfo(ByVal token As String) As List(Of userBasicInfo)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getNotificationsSendTo/{token}?entityName={entityName}&entityId={entityId}")>
    Function GetNotificationsSendTo(ByVal token As String, ByVal entityName As String, ByVal entityId As String) As List(Of notificationSendTo)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getHotSpots/{token}?deviceId={deviceId}")>
    Function GetHotSpots(ByVal token As String, ByVal deviceId As String) As List(Of hotSpots)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getBasicList/{token}?entityName={entityName}")>
    Function GetBasicList(ByVal token As String, ByVal entityName As String) As List(Of basicList)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="idNameList/{token}/{noCache}/{listName}")>
    Function GetIdNameList(ByVal token As String, ByVal noCache As String, ByVal listName As String) As List(Of basicList)

#Region "Geofences"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofencesMessages/{token}/{msgType}")>
    Function getGeofencesCustomMessages(ByVal token As String, ByVal msgType As String) As List(Of basicList)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveGeofence")>
    Function saveGeofence(ByVal data As geofenceClass) As responseOk

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceValidateName/{token}?id={id}&name={name}")>
    Function geofence_validateName(ByVal token As String, ByVal id As String, ByVal name As String) As responseOk

#End Region

#Region "Customers / Geofences"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customerSearch/{token}/{noCache}/{custName}")>
    Function CustomerSearch(ByVal token As String, ByVal noCache As String, ByVal custName As String) As List(Of customerSearch)

#End Region

#Region "Devices"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devices/search/{token}/{searchKey}/{keyValue}/{noCache}")>
    Function searchDevice(ByVal token As String, ByVal searchKey As String, ByVal keyValue As String, ByVal noCache As String) As searchDeviceResult

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deviceAction/{token}")>
    Function deviceAction(ByVal token As String, ByVal data As devAction) As callResult

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getAllCompanies/{token}")>
    Function crm_getAllCompanies(ByVal token As String) As List(Of basicList)

#End Region

#Region "Inventory"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="inventory/{token}")>
    Function getInventory(ByVal token As String) As List(Of inventory)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="assignInventory/{token}")>
    Function saveAssignment(ByVal token As String, ByVal data As assignedInventory) As assignedInventory

#End Region

#Region "Suspend/Reactivate company"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompaniesSuspendedReasons/{token}")>
    Function GetCompaniesSuspendedReasons(ByVal token As String) As List(Of basicList)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="suspendCompany/{token}")>
    Function getSuspendCompanies(ByVal token As String) As List(Of Company)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="suspendCompany/{token}")>
    Function saveSuspendCompany(ByVal token As String, ByVal data As Company) As Company

#End Region

#Region "Telemetry"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetry/{token}/{noCache}/{deviceId}")>
    Function GetIOs(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devIOs

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetry/{token}/{noCache}")>
    Function GetAllDevIOs(ByVal token As String, ByVal noCache As String) As List(Of devIOs)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetry/{token}")>
    Function setOutput(ByVal token As String, ByVal data As setRelay) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetrySetUp/{token}")>
    Function telemetrySetUp(ByVal token As String, ByVal data As ioSetUp) As responseOk

#End Region

#Region "Hour Meters"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hourMeters/{token}/{noCache}/{deviceId}")>
    Function GetDevMeters(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devInputsOnTime

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hourMeters/{token}/{noCache}")>
    Function GetAllDevMeters(ByVal token As String, ByVal noCache As String) As List(Of devInputsOnTime)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hourMeters/{token}/{deviceId}")>
    Function saveHourMeter(ByVal token As String, ByVal deviceId As String, ByVal data As devInputsOnTime) As responseOk

#End Region

#Region "Geofence Types"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceType/{token}/{noCache}")>
    Function getAllGeofenceTypes(ByVal token As String, ByVal noCache As String) As List(Of GeofenceType)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceType/{token}/{noCache}")>
    Function saveGeofenceType(ByVal token As String, ByVal noCache As String, ByVal data As GeofenceType) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="DELETE", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceType/{token}/{id}")>
    Function deleteGeofenceType(ByVal token As String, ByVal id As String) As responseOk

#End Region

#Region "Maintenance"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintSupportLists/{token}/{noCache}")>
    Function maintSupportListsGET(ByVal token As String, ByVal noCache As String) As maintSupportLists

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintDevice/{token}/{noCache}")>
    Function maintDeviceList(ByVal token As String, ByVal noCache As String) As List(Of maintDevice)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintSchedule/{token}/{noCache}/{deviceId}")>
    Function maintScheduleGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceSchedule

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintDevice/{token}/{deviceId}")>
    Function maintDeviceSave(ByVal token As String, ByVal deviceId As String, ByVal data As maintDevice) As maintDevice

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{noCache}/{deviceId}/{id}")>
    Function maintItemGet(ByVal token As String, ByVal noCache As String, ByVal deviceId As String, ByVal id As String) As scheduleItem

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{deviceId}/{id}")>
    Function maintItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem

    <OperationContract()>
    <WebInvoke(Method:="DELETE", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{deviceId}/{id}")>
    Function maintItemDelete(ByVal token As String, ByVal deviceId As String, ByVal id As String) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="PUT", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{deviceId}/{id}")>
    Function maintCompletedItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintLog/{token}/{noCache}/{deviceId}")>
    Function maintLogGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceLog

#End Region

#Region "QB Match"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="qbMatch/{token}")>
    Function getQBMatchCustomers(ByVal token As String) As qbMatch

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="qbMatch/{token}/{crmId}/{qbId}")>
    Function qbLinkCustomers(ByVal token As String, ByVal crmId As String, ByVal qbId As String) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="qbMatch/{token}/{crmId}")>
    Function qbUnLinkCustomers(ByVal token As String, ByVal crmId As String) As responseOk

#End Region

#Region "User Preferences"

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateUserPref/{token}")>
    Function updateUserPref(ByVal token As String, ByVal data As userPreference) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateUserPrefGroups/{token}")>
    Function updateUserPrefGroup(ByVal token As String, ByVal data As List(Of userPreference)) As responseOk

#End Region

#Region "CRM Customers"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompanies/{token}")>
    Function GetCompanies(ByVal token As String) As List(Of basicList)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customerDetails/{token}/{id}/{noCache}")>
    Function GetCompanyDetails(ByVal token As String, ByVal id As String, ByVal noCache As String) As crm_CustomerDetails

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customers/new/token/{token}")>
    Function saveNewCustomer(ByVal token As String, ByVal data As CRM_Customer) As CRM_Customer

#End Region

#Region "CRM Related"

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getEmailTypes/{token}/{type}")>
    Function EmailTypesGET(ByVal token As String, ByVal type As String) As List(Of basicList)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGenericMasters/{token}/{masterKey}")>
    Function GenericMastersGET(ByVal token As String, ByVal masterKey As String) As List(Of basicList)

#End Region

#Region "CRM - Invoices"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="invoice/{token}/{custId}/{noCache}")>
    Function crmGetInvoices(ByVal token As String, ByVal custId As String, ByVal noCache As String) As List(Of invoice)

#End Region

#Region "IfByPhone calls"

    'dateTime={dateTime}&sid={sid}&callType={callType}&firstAction={firstAction}&lastAction={lastAction}&calledNumber={calledNumber}&callerId={callerId}&transferType={transferType}&transferredToNumber={transferredToNumber}&callTransferStatus={callTransferStatus}&phoneLabel={phoneLabel}

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="incomingCall?dateTime={dateTime}&sid={sid}&callType={callType}&firstAction={firstAction}&lastAction={lastAction}&calledNumber={calledNumber}&callerId={callerId}&transferType={transferType}&transferredToNumber={transferredToNumber}&callTransferStatus={callTransferStatus}&phoneLabel={phoneLabel}&callDuration={callDuration}&talkMinutes={talkMinutes}")>
    Function saveIncomingCalls(ByVal dateTime As String,
                               ByVal sid As String,
                               ByVal callType As String,
                               ByVal firstAction As String,
                               ByVal lastAction As String,
                               ByVal calledNumber As String,
                               ByVal callerId As String,
                               ByVal transferType As String,
                               ByVal transferredToNumber As String,
                               ByVal callTransferStatus As String,
                               ByVal phoneLabel As String,
                               ByVal callDuration As String,
                               ByVal talkMinutes As String) As Boolean

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="smsReply?from_number={from_number}&message={message}&to_number={to_number}")>
    Function smsReplyCatcher(ByVal from_number As String, ByVal message As String, ByVal to_number As String) As Boolean

#End Region

#Region "Fleet HeartBeat"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="fleetheartbeat/{token}/{noCache}")>
    Function getFleetHeartBeat(ByVal token As String, ByVal noCache As String) As fleetHeartBeat


    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="fleetheartbeatHist/{token}/{dateFrom}/{dateTo}/{noCache}")>
    Function fleetheartbeatHist(ByVal token As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As fleetHeartBeat

#End Region

#Region "Sensors"
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="savesensor/{token}")>
    Function PostTempSensor(ByVal token As String, ByVal data As TempSensors) As responseSensor



#End Region
End Interface
