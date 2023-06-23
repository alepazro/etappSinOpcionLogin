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
Public Interface IBrokers

    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="new?token={token}")>
    Function CreateBroker(ByVal token As String, ByVal data As BrokerOrder) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="devices?token={token}&lastFetchOn={lastFetchOn}")>
    Function GetBrokersDevices(ByVal token As String, ByVal lastFetchOn As String) As List(Of BrokerDevices)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="brokers?token={token}&BrokerID={BrokerID}&DeviceID={DeviceID}")>
    Function GetBrokersOrder(ByVal token As String, ByVal BrokerID As Integer, ByVal DeviceID As Integer) As List(Of DTOBrokerOrder)
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="stops?token={token}&BrokerID={BrokerID}")>
    Function GetBrokersOrderStops(ByVal token As String, ByVal BrokerID As Integer) As List(Of DTOBrokerOrderStop)

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="delete?token={token}&BrokerID={BrokerID}&DeviceID={DeviceID}")>
    Function PostCancellBrokersOrder(ByVal token As String, ByVal BrokerID As Integer, ByVal DeviceID As Integer) As responseOk
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="sendemail?token={token}&brokerID={brokerID}&emails={emails}&resend={resend}&observations={observations}")>
    Function PostSentEmail(ByVal token As String, ByVal brokerID As Integer, ByVal emails As String, ByVal resend As Boolean, ByVal observations As String) As responseOk
    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="gettn/{trakingnumber}")>
    Function TrakingNumberExt(ByVal trakingnumber As String) As DTOBrokerTraking
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="posttraking?token={token}&brokerID={brokerID}&number={number}&datefrom={datefrom}&dateto={dateto}")>
    Function PostTraking(ByVal token As String, ByVal brokerID As Integer, ByVal number As String, ByVal datefrom As DateTime, ByVal dateto As DateTime, ByVal emails As String) As responseOk
    <OperationContract()>
    <WebInvoke(Method:="POST", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="newstop?token={token}")>
    Function CreateBrokerStop(ByVal token As String, ByVal data As DTOBrokerOrderStop) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="deletestop?token={token}&StopID={StopID}")>
    Function PostDeleteStop(ByVal token As String, ByVal StopID As Integer) As responseOk

    <OperationContract()>
    <WebInvoke(Method:="GET", BodyStyle:=WebMessageBodyStyle.Bare, RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="brokersms?token={token}&PBrokerID={PBrokerID}&Pobservations={Pobservations}&PPhoneNumber={PPhoneNumber}")>
    Function PostBrokerSMS(ByVal token As String, PBrokerID As Integer, Pobservations As String, PPhoneNumber As String) As responseOk


End Interface


