Imports System
Imports System.Collections.Generic
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Web
Imports System.Text
Imports System.ServiceModel.Activation
Imports System.IO

' NOTE: You can use the "Rename" command on the context menu to change the interface name "Iqbonline" in both code and config file together.
<ServiceContract()>
Public Interface Iqbonline

    <OperationContract()>
    Sub DoWork()

End Interface
