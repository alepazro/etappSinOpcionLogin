Imports System
Imports System.ServiceModel.Web

' NOTE: You can use the "Rename" command on the context menu to change the class name "Service1" in code, svc and config file together.
Public Class Service1
    Implements IService1

    Public Sub DoWork() Implements IService1.DoWork
    End Sub

    Public Function GetData(ByVal ID As String) As String Implements IService1.GetData

        Return "Hello"

    End Function

End Class
