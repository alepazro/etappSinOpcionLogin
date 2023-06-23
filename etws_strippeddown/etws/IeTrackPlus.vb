Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IeTrackPlus" in both code and config file together.
<ServiceContract()>
Public Interface IeTrackPlus

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="validateUser?login={login}&password={password}&expDays={expDays}&lat={lat}&lng={lng}")> _
    Function validateUser(ByVal login As String, ByVal password As String, ByVal expDays As String, ByVal lat As String, ByVal lng As String) As etPlus_User

    <OperationContract()> _
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="userGUID?login={login}&password={password}")> _
    Function getUserGUID(ByVal login As String, ByVal password As String) As etPlus_UserGUID

#Region "HGeofences"

    <OperationContract()>
    <WebInvoke(BodyStyle:=WebMessageBodyStyle.Bare, Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="api/v1/geofencesHist?tkn={tkn}")>
    Function geofencesHist(ByVal tkn As String) As hGeofencesResponse

#End Region

End Interface
