Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.Web
' NOTE: You can use the "Rename" command on the context menu to change the interface name "IdeviceSvc" in both code and config file together.
<ServiceContract()>
Public Interface IdeviceSvc

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/settingsList/{token}/{noCache}")> _
    Function deviceSettingsListGET(ByVal token As String, ByVal noCache As String) As List(Of deviceSettings)

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/{token}/{noCache}/{id}")> _
    Function getDevice(ByVal token As String, ByVal noCache As String, ByVal id As String) As deviceSettings

    <OperationContract()> _
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/{token}/{action}/{id}")> _
    Function saveDevice(ByVal token As String, ByVal action As String, ByVal id As String, ByVal data As deviceSettings) As responseOk

    <OperationContract()> _
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="device/{token}/{noCache}/{action}/{id}/{usrComment}")> _
    Function changeDeviceStatus(ByVal token As String, ByVal noCache As String, ByVal action As String, ByVal id As String, ByVal usrComment As String) As responseOk

End Interface
