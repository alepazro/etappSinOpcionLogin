' NOTE: You can use the "Rename" command on the context menu to change the class name "deviceSvc" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select deviceSvc.svc or deviceSvc.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

Public Class deviceSvc
    Implements IdeviceSvc

    Public Function deviceSettingsListGET(ByVal token As String, ByVal noCache As String) As List(Of deviceSettings) Implements IdeviceSvc.deviceSettingsListGET
        Dim lst As New List(Of deviceSettings)
        Dim dl As New DataLayer

        Try
            lst = dl.deviceSettingsListGET(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getDevice(ByVal token As String, ByVal noCache As String, ByVal id As String) As deviceSettings Implements IdeviceSvc.getDevice
        Dim dev As New deviceSettings
        Dim dl As New DataLayer

        Try
            dev = dl.deviceSettings_GetDevice(token, id)
        Catch ex As Exception

        End Try

        Return dev

    End Function

    Public Function saveDevice(ByVal token As String, ByVal action As String, ByVal id As String, ByVal data As deviceSettings) As responseOk Implements IdeviceSvc.saveDevice
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.deviceSettingsSaveDevice(token, data)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

    Public Function changeDeviceStatus(ByVal token As String, ByVal noCache As String, ByVal action As String, ByVal id As String, ByVal usrComment As String) As responseOk Implements IdeviceSvc.changeDeviceStatus
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.deviceSettings_ChangeStatus(token, action, id, usrComment)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

End Class
