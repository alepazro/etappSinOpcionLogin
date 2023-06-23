Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.Web

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Icases" in both code and config file together.
<ServiceContract()>
Public Interface Icases

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="companies/{token}/{noCache}")> _
    Function companiesGET(ByVal token As String, ByVal noCache As String) As List(Of idName)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devices/{token}/{noCache}?companyId={companyId}")> _
    Function devicesGET(ByVal token As String, ByVal noCache As String, ByVal companyId As String) As List(Of caseDevice)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="technicians/{token}/{noCache}")> _
    Function techniciansGET(ByVal token As String, ByVal noCache As String) As List(Of idName)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="casesBasicTables/{token}/{noCache}")> _
    Function casesBasicTablesGET(ByVal token As String, ByVal noCache As String) As crmBasicTables

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="cases/{token}/{noCache}")> _
    Function casesGET(ByVal token As String, ByVal noCache As String) As List(Of caseClass)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="case/{token}/{noCache}?id={id}")> _
    Function caseGET(ByVal token As String, ByVal noCache As String, ByVal id As String) As caseClass

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="case/{token}")> _
    Function casePOST(ByVal token As String, ByVal data As caseClass) As caseClass

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="caseActivity/{token}")> _
    Function caseActivityPOST(ByVal token As String, ByVal data As caseActivity) As caseActivity

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="changeStatus/{token}")> _
    Function changeStatusPOST(ByVal token As String, ByVal data As changeStatus) As changeStatusResponse

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="casesFiltered/{token}")> _
    Function casesGetFiltered(ByVal token As String, ByVal filters As caseFilters) As List(Of caseClass)

#Region "Onboarding"

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="onboarding/pending/{token}/{noCache}")> _
    Function onboardingPendingCustomers(ByVal token As String, ByVal noCache As String) As List(Of pendingOnBoarding)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="onboarding/pending/{token}/{noCache}/{id}")> _
    Function onboardingPendingCustomerGet(ByVal token As String, ByVal noCache As String, ByVal id As String) As onBoardingCustomer

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="onboarding/pending/{token}/{id}")> _
    Function onBoardingDone(ByVal token As String, ByVal id As String, ByVal data As onBoardingCustomer) As resultOk

#End Region

#Region "Online Support"

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="onlineSupport/onlineUsers/{token}/{noCache}")> _
    Function getOnlineUsers(ByVal token As String, ByVal noCache As String) As realTimeActivity

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="onlineSupport/userProfile/{token}/{noCache}/{id}")> _
    Function getUserProfile(ByVal token As String, ByVal noCache As String, ByVal id As String) As userProfile

#End Region

End Interface
