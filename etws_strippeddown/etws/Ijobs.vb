Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Ijobs" in both code and config file together.
<ServiceContract()>
Public Interface Ijobs

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobs/{token}/{noCache}/{statId}/{wzId}/{techId}/{jobNo}/{custName}")>
    Function GetJobsNew(ByVal token As String, ByVal noCache As String, ByVal statId As String, ByVal wzId As String, ByVal techId As String, ByVal jobNo As String, ByVal custName As String) As List(Of jobnew)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobsList?token={token}&filter={filter}")> _
    Function getJobs(ByVal token As String, ByVal filter As String) As List(Of Job3)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJob?token={token}&id={jobId}")>
    Function getJob(ByVal token As String, ByVal jobId As String) As Jobobject

    ''' <summary>
    ''' Created: 4/30/2016
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="wzId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustomersIdName?token={token}&wzId={wzId}")> _
    Function getCustomersIdName(ByVal token As String, ByVal wzId As String) As List(Of CustomerIdName)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustomersList?token={token}")> _
    Function getCustomers(ByVal token As String) As List(Of Customer3)

    ''' <summary>
    ''' Created: 4/30/2016
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="custId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCustomer?token={token}&custId={custId}")> _
    Function getJobCustomer(ByVal token As String, ByVal custId As String) As jobCustomer

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getUsersList?token={token}")> _
    Function getCompanyUsers(ByVal token As String) As List(Of user2)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobStatusList?token={token}")> _
    Function getJobStatusList(ByVal token As String) As List(Of jobStatusList)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobPrioritiesList?token={token}")> _
    Function getJobPrioritiesList(ByVal token As String) As List(Of companyIdNameItem)

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getJobCategoriesList?token={token}")> _
    Function getJobCategoriesList(ByVal token As String) As List(Of idNameItem)

    ''' <summary>
    ''' Created: 4/30/2016
    ''' </summary>
    ''' <param name="token"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJobCustomer?token={token}")> _
    Function postJobCustomer(ByVal token As String, ByVal data As jobCustomer) As jobCustomer

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postCustomer/{token}")> _
    Function postCustomer(ByVal token As String, ByVal data As Customer3) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postJob/{token}")> _
    Function postJob(ByVal token As String, ByVal data As Job3) As responseOk

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getImage?token={token}&imageId={imageId}")> _
    Function getImage(ByVal token As String, ByVal imageId As String) As imgData

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getImagesnew?token={token}&JobUniqueKey={JobUniqueKey}")>
    Function getImagesNew(ByVal token As String, ByVal JobUniqueKey As String) As List(Of imgData)
    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="ImageNew")>
    Function postImageNew(ByVal data As imgData) As responseOk
    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCountryStates?token={token}")> _
    Function getCountryStates(ByVal token As String) As List(Of idNameItem)

    ''' <summary>
    ''' Created: 4/30/2016
    ''' </summary>
    ''' <param name="token"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobSupportTables?token={token}")>
    Function GetJobSupportTables(ByVal token As String) As jobSupportTables

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobStop/{token}")>
    Function postJobStop(ByVal token As String, ByVal data As JobStop) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="GetJobStops?token={token}&jobUniquekey={jobUniquekey}")>
    Function GetJobStops(ByVal token As String, ByVal jobUniquekey As String) As List(Of JobStop)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getLocation?token={token}&lat={lat}&lon={lon}&type={type}&fullAddress={fullAddress}")>
    Function GeocodingLocation_GET(ByVal token As String, ByVal lat As Decimal, ByVal lon As Decimal, ByVal type As String, ByVal fullAddress As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postLocation?token={token}")>
    Function GeocodingLocation_POST(ByVal token As String, ByVal data As Location) As String

    <OperationContract>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="postjob?token={token}")>
    Function postJobNew(ByVal token As String, ByVal data As Jobobject) As responseOk

    <OperationContract>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getstopcompanies?token={token}")>
    Function ListStopByCompanies_GET(ByVal token As String) As String

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="drivers?token={token}")>
    Function GetDrivers(ByVal token As String) As List(Of Object)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getstatus?token={token}")>
    Function getStatus(ByVal token As String) As List(Of StatusJobs)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="GetJobNotes?token={token}&jobUniquekey={jobUniquekey}&uniqueKey={uniqueKey}&action={action}")>
    Function GetNotes(ByVal token As String, ByVal jobUniquekey As String, uniqueKey As String, action As Byte) As List(Of JobNote)

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="jobNote?token={token}")>
    Function postJobNote(ByVal token As String, ByVal data As JobNote) As responseOk
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="broker/detail?token={token}&jobUniquekey={jobUniquekey}")>
    Function GetBrokerDetail(ByVal token As String, ByVal jobUniquekey As String) As List(Of BrokerOrder)
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="resendemail?token={token}&jobUniquekey={jobUniquekey}&emails={emails}")>
    Function ResendEmailBrokerOrder(ByVal token As String, ByVal jobUniquekey As String, ByVal emails As String) As responseOk

End Interface
