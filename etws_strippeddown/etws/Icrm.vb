Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.Web

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Icrm" in both code and config file together.
<ServiceContract()>
Public Interface Icrm

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="validateToken/{token}/{noCache}")>
    Function validateToken(ByVal token As String, ByVal noCache As String) As crmEntities.user

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="customers/{token}/{noCache}?search={search}")>
    Function customersGET(ByVal token As String, ByVal noCache As String, ByVal search As String) As String 'List(Of crmEntities.customer)

    <OperationContract()>
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="reportsBasicTables/{token}/{noCache}")>
    Function reportsBasicTablesGET(ByVal token As String, ByVal noCache As String) As crmBasicTables

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="crmreport/{token}")>
    Function getCrmReport(ByVal token As String, ByVal filters As reportsFilters) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCrmDealers/{token}")>
    Function getCrmDealers(ByVal token As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="getCrmDevices?token={token}&devicesId={devicesId}")>
    Function getCrmDevices(ByVal token As String, ByVal devicesId As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="CRM_UpdateDealerDevices?token={token}&idDevices={idDevices}&idDealer={idDealer}")>
    Function CRM_UpdateDealerDevices(ByVal token As String, ByVal idDevices As String, idDealer As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="CRM_GetCompanys?token={token}&search={search}")>
    Function CRM_GetCompanys(ByVal token As String, ByVal search As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="CRM_updateMoveCompany?token={token}&DealersCompany={DealersCompany}&Company={Company}")>
    Function CRM_updateMoveCompany(ByVal token As String, ByVal DealersCompany As String, Company As String) As String

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="crmGetCustomerByUniqueKey?token={token}&uid={uid}")>
    Function crmGetCustomerByUniqueKey(ByVal token As String, ByVal uid As String, Company As String) As String



End Interface
