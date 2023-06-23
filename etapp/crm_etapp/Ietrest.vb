Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.Web

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Ietrest" in both code and config file together.
<ServiceContract()>
Public Interface Ietrest

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="doDummyCall/{noCache}")> _
    Function doDummyCall(ByVal noCache As String) As Boolean

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="auth/{credentials}")> _
    Function Authorization(ByVal credentials As String) As userBasicInfo

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devList/{token}")> _
    Function GetDevicesList(ByVal token As String) As List(Of device)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/{id}")> _
    Function GetDeviceInfo(ByVal id As String) As device

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deviceInfo/{token}/{deviceId}")> _
    Function GetDeviceById(ByVal token As String, ByVal deviceId As String) As device

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevices/{token}?lastFetchOn={lastFetchOn}&qtyPanels={qtyPanels}&devicesPerPanel={devicesPerPanel}")> _
    Function GetDevices(ByVal token As String, ByVal lastFetchOn As String, ByVal qtyPanels As String, ByVal devicesPerPanel As String) As List(Of device)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getMaintSchedules/{token}?deviceId={deviceId}&taskId={taskId}&noCache={noCache}")> _
    Function getMaintSchedules(ByVal token As String, ByVal deviceId As String, ByVal taskId As String, ByVal noCache As String) As List(Of maintSchedule)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getMaintHServices/{token}?deviceId={deviceId}&taskID={taskId}&dateFrom={dateFrom}&dateTo={dateTo}&noCache={noCache}")> _
    Function getMaintHServices(ByVal token As String, ByVal deviceId As String, ByVal taskId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As List(Of maintHServices)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getMaintHFuel/{token}?deviceId={deviceId}&dateFrom={dateFrom}&dateTo={dateTo}&noCache={noCache}")> _
    Function getMaintHFuel(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As List(Of maintHFuel)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getUsersBasicInfo/{token}")> _
    Function GetUsersBasicInfo(ByVal token As String) As List(Of userBasicInfo)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getNotificationsSendTo/{token}?entityName={entityName}&entityId={entityId}")> _
    Function GetNotificationsSendTo(ByVal token As String, ByVal entityName As String, ByVal entityId As String) As List(Of notificationSendTo)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getHotSpots/{token}?deviceId={deviceId}")> _
    Function GetHotSpots(ByVal token As String, ByVal deviceId As String) As List(Of hotSpots)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getBasicList/{token}?entityName={entityName}")> _
    Function GetBasicList(ByVal token As String, ByVal entityName As String) As List(Of basicList)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="idNameList/{token}/{noCache}/{listName}")> _
    Function GetIdNameList(ByVal token As String, ByVal noCache As String, ByVal listName As String) As List(Of basicList)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getValue/{token}/{noCache}/{valueName}")> _
    Function getValue(ByVal token As String, ByVal noCache As String, ByVal valueName As String) As singleValue

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getInfoWindow/{token}/{noCache}/{deviceId}")> _
    Function getInfoWindow(ByVal token As String, ByVal noCache As String, ByVal deviceID As String) As deviceInfoWindow

#Region "CRM Customers"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompanies/{token}")> _
    Function GetCompanies(ByVal token As String) As List(Of basicList)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customerDetails/{token}/{id}/{noCache}")> _
    Function GetCompanyDetails(ByVal token As String, ByVal id As String, ByVal noCache As String) As crm_CustomerDetails

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customers/new/token/{token}")> _
    Function saveNewCustomer(ByVal token As String, ByVal data As CRM_Customer) As CRM_Customer

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveCompanyNote?token={token}")> _
    Function saveCompanyNote(ByVal token As String, ByVal data As companyNote) As responseOk

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompanyNotes?token={token}&custId={custId}")> _
    Function GetCompanyNotes(ByVal token As String, ByVal custId As String) As List(Of companyNote)

#End Region

#Region "Devices"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devices/search/{token}/{searchKey}/{keyValue}/{noCache}")> _
    Function searchDevice(ByVal token As String, ByVal searchKey As String, ByVal keyValue As String, ByVal noCache As String) As searchDeviceResult

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deviceAction/{token}")> _
    Function deviceAction(ByVal token As String, ByVal data As devAction) As callResult

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getAllCompanies/{token}")> _
    Function crm_getAllCompanies(ByVal token As String) As List(Of basicList)

#End Region

#Region "Inventory"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="inventory/{token}")> _
    Function getInventory(ByVal token As String) As List(Of inventory)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="addInventory/{token}")> _
    Function saveNewInventory(ByVal token As String, ByVal data As newInventory) As newInventory

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="assignInventory/{token}")> _
    Function saveAssignment(ByVal token As String, ByVal data As assignedInventory) As assignedInventory

#End Region

#Region "Suspend/Reactivate company"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCompaniesSuspendedReasons/{token}")> _
    Function GetCompaniesSuspendedReasons(ByVal token As String) As List(Of basicList)

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="suspendCompany/{token}")> _
    Function getSuspendCompanies(ByVal token As String) As List(Of Company)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="suspendCompany/{token}")> _
    Function saveSuspendCompany(ByVal token As String, ByVal data As Company) As Company

#End Region

#Region "Geofences"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGeofencesMessages/{token}/{msgType}")> _
    Function getGeofencesCustomMessages(ByVal token As String, ByVal msgType As String) As List(Of basicList)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveGeofence")> _
    Function saveGeofence(ByVal data As geofenceClass) As responseOk

#End Region

#Region "Work Zones"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="workZones/{token}")> _
    Function GetWorkZones(ByVal token As String) As List(Of basicList)

#End Region

#Region "Technicians"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="technicians/{token}/{wzId}")> _
    Function GetTechnicians(ByVal token As String, ByVal wzId As String) As List(Of technician)

#End Region

#Region "Jobs Status"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobStatus/{token}/{noCache}")> _
    Function GetJobStatus(ByVal token As String, ByVal noCache As String) As List(Of jobStatus)

#End Region

#Region "Jobs"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobs/{token}/{noCache}/{statId}/{wzId}/{techId}/{jobNo}/{custName}")> _
    Function GetJobs(ByVal token As String, ByVal noCache As String, ByVal statId As String, ByVal wzId As String, ByVal techId As String, ByVal jobNo As String, ByVal custName As String) As List(Of job)

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobs/{token}/{noCache}/{jobId}")> _
    Function GetJob(ByVal token As String, ByVal noCache As String, ByVal jobId As String) As job

    ''' <summary>
    ''' Created: 1/1/2015 (estimated date)
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="noCache"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobSupportTables/{token}/{noCache}")> _
    Function GetJobSupportTables(ByVal token As String, ByVal noCache As String) As jobSupportTables

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobs/{token}")> _
    Function saveJob(ByVal token As String, ByVal data As job) As responseOk

#End Region

#Region "Customers / Geofences"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customerSearch/{token}/{noCache}/{custName}")> _
    Function CustomerSearch(ByVal token As String, ByVal noCache As String, ByVal custName As String) As List(Of customerSearch)

#End Region

#Region "Telemetry"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetry/{token}/{noCache}/{deviceId}")> _
    Function GetIOs(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devIOs

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetry/{token}/{noCache}")> _
    Function GetAllDevIOs(ByVal token As String, ByVal noCache As String) As List(Of devIOs)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetry/{token}")> _
    Function setOutput(ByVal token As String, ByVal data As setRelay) As responseOk

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="telemetrySetUp/{token}")> _
    Function telemetrySetUp(ByVal token As String, ByVal data As ioSetUp) As responseOk

#End Region

#Region "Hour Meters"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hourMeters/{token}/{noCache}/{deviceId}")> _
    Function GetDevMeters(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devInputsOnTime

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hourMeters/{token}/{noCache}")> _
    Function GetAllDevMeters(ByVal token As String, ByVal noCache As String) As List(Of devInputsOnTime)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="hourMeters/{token}/{deviceId}")> _
    Function saveHourMeter(ByVal token As String, ByVal deviceId As String, ByVal data As devInputsOnTime) As responseOk

#End Region

#Region "Geofence Types"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceType/{token}/{noCache}")> _
    Function getAllGeofenceTypes(ByVal token As String, ByVal noCache As String) As List(Of geofenceType)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceType/{token}/{noCache}")> _
    Function saveGeofenceType(ByVal token As String, ByVal noCache As String, ByVal data As geofenceType) As responseOk

    <OperationContract()> _
    <WebInvoke(Method:="DELETE", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="geofenceType/{token}/{id}")> _
    Function deleteGeofenceType(ByVal token As String, ByVal id As String) As responseOk

#End Region

#Region "Maintenance"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintSupportLists/{token}/{noCache}")> _
    Function maintSupportListsGET(ByVal token As String, ByVal noCache As String) As maintSupportLists

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintDevice/{token}/{noCache}")> _
    Function maintDeviceList(ByVal token As String, ByVal noCache As String) As List(Of maintDevice)

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintSchedule/{token}/{noCache}/{deviceId}")> _
    Function maintScheduleGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceSchedule

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintDevice/{token}/{deviceId}")> _
    Function maintDeviceSave(ByVal token As String, ByVal deviceId As String, ByVal data As maintDevice) As maintDevice

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{noCache}/{deviceId}/{id}")> _
    Function maintItemGet(ByVal token As String, ByVal noCache As String, ByVal deviceId As String, ByVal id As String) As scheduleItem

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{deviceId}/{id}")> _
    Function maintItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem

    <OperationContract()> _
    <WebInvoke(Method:="DELETE", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{deviceId}/{id}")> _
    Function maintItemDelete(ByVal token As String, ByVal deviceId As String, ByVal id As String) As responseOk

    <OperationContract()> _
    <WebInvoke(Method:="PUT", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintItem/{token}/{deviceId}/{id}")> _
    Function maintCompletedItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="maintLog/{token}/{noCache}/{deviceId}")> _
    Function maintLogGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceLog

#End Region

#Region "QB Match"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="qbMatch/{token}")> _
    Function getQBMatchCustomers(ByVal token As String) As qbMatch

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="qbMatch/{token}/{crmId}/{qbId}")> _
    Function qbLinkCustomers(ByVal token As String, ByVal crmId As String, ByVal qbId As String) As responseOk

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="qbMatch/{token}/{crmId}")> _
    Function qbUnLinkCustomers(ByVal token As String, ByVal crmId As String) As responseOk

#End Region

#Region "User Preferences"

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateUserPref/{token}")> _
    Function updateUserPref(ByVal token As String, ByVal data As userPreference) As responseOk

#End Region

#Region "CRM Related"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getEmailTypes/{token}/{type}")> _
    Function EmailTypesGET(ByVal token As String, ByVal type As String) As List(Of basicList)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getGenericMasters/{token}/{masterKey}")> _
    Function GenericMastersGET(ByVal token As String, ByVal masterKey As String) As List(Of basicList)

#End Region

#Region "CRM - Invoices"

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="invoice/{token}/{custId}/{noCache}")> _
    Function crmGetInvoices(ByVal token As String, ByVal custId As String, ByVal noCache As String) As List(Of invoice)

#End Region

#Region "Quick Message"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getQuickMsgDriversList/{token}/{noCache}")> _
    Function getQuickMsgDriversList(ByVal token As String, ByVal noCache As String) As List(Of quickMsgDriver)

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="quickMsg/{token}")> _
    Function sendQuickMsg(ByVal token As String, ByVal data As quickMsg) As responseOk

#End Region

#Region "CRM - Confirm Shipment"

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="confirmShipment/{token}")> _
    Function confirmShipment(ByVal token As String, ByVal data As confirmShipment) As responseOk

#End Region

#Region "IfByPhone"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="smsReply/{msg}")> _
    Function smsReplyCatcher(ByVal msg As String) As Boolean

#End Region

#Region "Google Signature"

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getSignature")> _
    Function getGoogleSignature(ByVal data As googURL) As googSig

#End Region

#Region "Engagement"

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="pageEngagement")>
    Function pageEngagement(ByVal data As engagementTick) As responseOk

#End Region

#Region "Reports"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getTroubleshootingReport/{token}?deviceId={deviceId}&dateFrom={dateFrom}&dateTo={dateTo}&noCache={noCache}")> _
    Function getTroubleshootingReport(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As List(Of troubleLog)

#End Region

End Interface


