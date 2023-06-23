Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Ipilot" in both code and config file together.
<ServiceContract()>
Public Interface Ipilot

#Region "Authorization and Credentials"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="authorization?login={login}&password={password}&expDays={expDays}&lat={lat}&lng={lng}")> _
    Function authorization(ByVal login As String, ByVal password As String, ByVal expDays As String, ByVal lat As String, ByVal lng As String) As pilotUser

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="authorization2?login={login}&password={password}&lat={lat}&lng={lng}")> _
    Function authorization2(ByVal login As String, ByVal password As String, ByVal lat As String, ByVal lng As String) As pilotUser2

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="recoverCredentials")> _
    Function recoverCredentials(ByVal data As recoverCredentialsRequest) As responseOk

#End Region

#Region "Basic Information Tables"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDevices?token={token}")> _
    Function getDevices(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCountryStates?token={token}")> _
    Function getCountryStates(ByVal token As String) As List(Of idNameItem)

#End Region

#Region "Devices"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devList/{token}/{sourceId}")> _
    Function GetDevicesList(ByVal token As String, ByVal sourceId As String) As List(Of FleetDeviceVideo)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deviceInfo/{token}/{id}/{sourceId}")> _
    Function GetDeviceInfo(ByVal token As String, ByVal id As String, ByVal sourceId As String) As FleetDeviceVideo

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="trail/{token}/{id}/{trailDate}/{hourFrom}/{hourTo}/{sourceId}")> _
    Function GetTrail(ByVal token As String, ByVal id As String, ByVal trailDate As String, ByVal hourFrom As String, ByVal hourTo As String, ByVal sourceId As String) As List(Of Trail)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getinputs?token={token}&deviceid={deviceid}&count={count}")>
    Function InfoDevicesInputs(ByVal token As String, ByVal deviceid As Integer, ByVal count As Integer) As DevicesInformationInputs

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateinputs/{token}")>
    Function InfoDevicesInputs_Update(ByVal token As String, ByVal data As DevicesInformationInputs) As responseOk

#End Region

#Region "Check In/Out"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="checkInReasons?token={token}&isFullSync={isFullSync}")> _
    Function getCheckInReasons(ByVal token As String, ByVal isFullSync As String) As List(Of checkInReason)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCheckInLog?token={token}")> _
    Function postCheckInLog(ByVal token As String, ByVal data As List(Of checkInEvent)) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateCheckInStatus")> _
    Function updateCheckInStatus(ByVal data As checkInRequest) As responseOk

#End Region

#Region "Jobs"

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mjobs/{token}/{noCache}/{statId}/{wzId}/{techId}/{jobNo}/{custName}")>
    Function GetJobsNew(ByVal token As String, ByVal noCache As String, ByVal statId As String, ByVal wzId As String, ByVal techId As String, ByVal jobNo As String, ByVal custName As String) As List(Of jobnew)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mgetJob?token={token}&id={jobId}")>
    Function getJobNew(ByVal token As String, ByVal jobId As String) As Jobobject

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mgetJobCategoriesList?token={token}")>
    Function getJobCategoriesList(ByVal token As String) As List(Of idNameItem)

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mgetJobPrioritiesList?token={token}")>
    Function getJobPrioritiesList(ByVal token As String) As List(Of companyIdNameItem)
    <OperationContract>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mpostjob?token={token}")>
    Function postJobNew(ByVal token As String, ByVal data As Jobobject) As responseOk
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mGetJobStops?token={token}&jobUniquekey={jobUniquekey}")>
    Function GetJobStops(ByVal token As String, ByVal jobUniquekey As String) As List(Of JobStop)
    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mgetImagesnew?token={token}&JobUniqueKey={JobUniqueKey}")>
    Function getImagesNew(ByVal token As String, ByVal JobUniqueKey As String) As List(Of imgData)
    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mImageNew")>
    Function postImageNew(ByVal data As imgData) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mjobStop/{token}")>
    Function postJobStop(ByVal token As String, ByVal data As JobStop) As responseOk

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mgetstatus?token={token}")>
    Function getStatus(ByVal token As String) As List(Of StatusJobs)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mGetJobNotes?token={token}&jobUniquekey={jobUniquekey}&uniqueKey={uniqueKey}&action={action}")>
    Function GetNotes(ByVal token As String, ByVal jobUniquekey As String, uniqueKey As String, action As Byte) As List(Of JobNote)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mjobNote/{token}")>
    Function postJobNote(ByVal token As String, ByVal data As JobNote) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="mpoststatusjob/{token}")>
    Function postStatusJob(ByVal token As String, ByVal data As JobStatus) As responseOk



    '---------------------



    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJob?token={token}&jobId={jobId}&lat={lat}&lng={lng}")> _
    Function getJob(ByVal token As String, ByVal jobId As String, ByVal lat As String, ByVal lng As String) As pilotJob

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="addJob")> _
    Function addJob(ByVal data As pilotJob) As jobResponse

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateJobStatus")> _
    Function updateJobStatus(ByVal data As pilotJobStatusUpdate) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="addJobNote")> _
    Function addJobNote(ByVal data As pilotJobNote) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="removeJob")> _
    Function removeJob(ByVal data As pilotJobRemove) As responseOk

#End Region

#Region "My Group Location"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getMyGroup?token={token}&lat={lat}&lng={lng}")> _
    Function getMyGroup(ByVal token As String, ByVal lat As String, ByVal lng As String) As List(Of myTeam)

#End Region

#Region "Inspection Lists"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getInspectionLists?token={token}")> _
    Function getInspectionLists(ByVal token As String) As List(Of inspectionList)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getInspectionListItems?token={token}&listId={listId}")> _
    Function getInspectionListItems(ByVal token As String, ByVal listId As String) As List(Of inspectionItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveInspection")> _
    Function saveInspection(ByVal data As inspectionLog) As responseOk

#End Region

#Region "Fuel Purchase"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveFuelPurchase")> _
    Function saveFuelPurchase(ByVal data As fuelPurchase) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveFuelPurchase2")> _
    Function saveFuelPurchase2(ByVal data As fuelPurchase2) As responseOk

#End Region

#Region "Driver Status"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDriverStatusList?token={token}")> _
    Function getDriverStatusList(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="updateDriverLog")> _
    Function updateDriverLog(ByVal data As driverLog) As responseOk

#End Region

#Region "Field Service Module - GetAll* PostAll* Methods"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getAllData?token={token}&lastSyncOn={lastSyncOn}")> _
    Function getAllData(ByVal token As String, ByVal lastSyncOn As String) As allDataGet

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postAllData")> _
    Function postAllData(ByVal data As allDataPost) As allDataPostResult

#End Region

#Region "Field Service Module - Basic Information Tables"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getAdsList?token={token}")> _
    Function getMarketingCampaignsList(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getSalesTaxesList?token={token}")> _
    Function getSalesTaxesList(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustomerTypesList?token={token}")> _
    Function getCustomerTypesList(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getPaymentTermsList?token={token}")> _
    Function getPaymentTermsList(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobStatusList?token={token}")> _
    Function getJobStatusList(ByVal token As String) As List(Of companyIdNameItem)

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobStatusList2?token={token}")>
    Function getJobStatusList2(ByVal token As String) As List(Of jobStatusList)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getAssetTypesList?token={token}")> _
    Function getAssetTypesList(ByVal token As String) As List(Of idNameItem)

#End Region

#Region "Field Service Module - Work Information - Jobs"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobs?token={token}")> _
    Function getJobs(ByVal token As String) As List(Of Job)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobAssets?token={token}")> _
    Function getJobAssets(ByVal token As String) As List(Of JobAsset)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJob")> _
    Function postJob(ByVal data As Job) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJobAsset")> _
    Function postJobAsset(ByVal data As JobAsset) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJobStatusLog")> _
    Function postJobStatusLog(ByVal data As JobStatus) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJobNotesLog")> _
    Function postJobNotesLog(ByVal data As JobNote) As responseOk

#End Region

#Region "Field Service Module - Work Information - Jobs 2"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobs2?token={token}")> _
    Function getJobs2(ByVal token As String) As List(Of Job2)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJob2")> _
    Function postJob2(ByVal data As Job2) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="DELETE", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deleteJob")> _
    Function deleteJob(ByVal data As JobDelete) As responseOk

#End Region

#Region "Field Service Module - Work Information - Jobs 3"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobs3?token={token}&isFullSync={isFullSync}")> _
    Function getJobs3(ByVal token As String, ByVal isFullSync As String) As List(Of Job3)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobsToRemove?token={token}")> _
    Function getJobsToRemove(ByVal token As String) As List(Of JobToRemove)

#End Region

#Region "Field Service Module - Work Information - Customers"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustomers?token={token}")> _
    Function getCustomers(ByVal token As String) As List(Of Customer)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustLocations?token={token}")> _
    Function getCustLocations(ByVal token As String) As List(Of CustLocation)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustContacts?token={token}")> _
    Function getCustContacts(ByVal token As String) As List(Of CustContact)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustAssets?token={token}")> _
    Function getCustAssets(ByVal token As String) As List(Of CustAsset)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustomer")> _
    Function postCustomer(ByVal data As Customer) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustLocation")> _
    Function postCustLocation(ByVal data As CustLocation) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustContact")> _
    Function postCustContact(ByVal data As CustContact) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustAsset")> _
    Function postCustAsset(ByVal data As CustAsset) As responseOk

#End Region

#Region "Field Service Module - Work Information - Customers 2"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustomers2?token={token}")> _
    Function getCustomers2(ByVal token As String) As List(Of Customer2)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustContacts2?token={token}")> _
    Function getCustContacts2(ByVal token As String) As List(Of CustContact2)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustomer2")> _
    Function postCustomer2(ByVal data As Customer2) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustContact2")> _
    Function postCustContact2(ByVal data As CustContact2) As responseOk

#End Region

#Region "Field Service Module - Dynamic Fields"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getFieldTypes?token={token}")> _
    Function getFieldTypes(ByVal token As String) As List(Of idNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDynamicTemplates?token={token}")> _
    Function getDynamicTemplates(ByVal token As String) As List(Of dynamicTemplate)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveTemplate/{token}")> _
    Function saveDynamicTemplate(ByVal token As String, ByVal data As dynamicTemplate) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getDynamicFields?token={token}&templateId={templateId}")> _
    Function getDynamicFields(ByVal token As String, ByVal templateId As String) As List(Of dynamicField)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveDynamicField/{token}")> _
    Function saveDynamicField(ByVal token As String, ByVal data As dynamicField) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deleteDynamicField/{token}/{templateId}/{fieldId}")> _
    Function deleteDynamicField(ByVal token As String, ByVal templateId As String, ByVal fieldId As String) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="saveDynamicAnswers")> _
    Function saveDynamicAnswers(ByVal data As List(Of dynamicAnswer)) As responseOk

#End Region

#Region "Image Upload"

    '<OperationContract()> _
    '<WebInvoke(Method:="POST", UriTemplate:="postImage/{jobId}/{imgType}/{fileType}")> _
    'Function postImage(ByVal jobId As String, ByVal imgType As String, ByVal fileType As String, ByVal data As String) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postImage")>
    Function postImage(ByVal data As imgData) As responseOk

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getImages?token={token}")>
    Function getImages(ByVal token As String) As List(Of imgData)
    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getImage?token={token}&id={id}")> _
    Function getImage(ByVal token As String, ByVal id As String) As imgData

#End Region

#Region "Transactions Viewer"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getTransactionData?transId={transId}&noCache={noCache}")> _
    Function getTransactionData(ByVal transId As String, ByVal noCache As String) As String

#End Region

#Region "Alerts API"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="alerts?token={token}&isFullSync={isFullSync}")> _
    Function getAlerts(ByVal token As String, ByVal isFullSync As String) As List(Of alert)

#End Region

#Region "Notifications API"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postRegId")> _
    Function postRegistrationId(ByVal data As registrationId) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getTopics?token={token}&isFullSync={isFullSync}")> _
    Function getNotificationTopics(ByVal token As String, ByVal isFullSync As String) As List(Of notificationTopic)

#End Region

End Interface
