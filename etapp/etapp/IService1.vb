Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IService1" in both code and config file together.
<ServiceContract()>
Public Interface IService1

    <OperationContract()>
    Sub DoWork()

    <OperationContract()> _
    <WebGet(BodyStyle:=WebMessageBodyStyle.Wrapped, ResponseFormat:=WebMessageFormat.Json, UriTemplate:="data/{id}")> _
    Function GetData(ByVal ID As String) As String

End Interface
