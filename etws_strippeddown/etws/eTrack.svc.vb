' NOTE: You can use the "Rename" command on the context menu to change the class name "eTrack" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select eTrack.svc or eTrack.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation.WebScriptServiceHostFactory
Imports System.Data
Imports System.Web.Script.Serialization
Imports System.Net.HttpWebRequest

Public Class eTrack
    Implements IeTrack

    Public ledOn As String = "https://easitrack.net/icons/green_light15x15.png"
    Public ledOff As String = "https://easitrack.net/icons/red_light15x15.png"

#Region "Authentication"

    Function validateCredentials(ByVal login As String, ByVal password As String) As webUser Implements IeTrack.validateCredentials
        Dim user As New webUser
        Dim dl As New DataLayer
        Dim dv As DataView
        Dim msg As String = ""

        Try
            dv = dl.ValidateCredentials(login, password, 999, "", "", 0, 0, msg)
            If Not IsNothing(dv) Then
                If dv.Count = 1 Then
                    user.tokenCookie = ConfigurationManager.AppSettings("tokenCookie")
                    user.isValid = dv.Item(0).Item("isValid")
                    user.token = dv.Item(0).Item("token")
                    user.companyName = dv.Item(0).Item("companyName")
                    user.fullName = dv.Item(0).Item("fullName")
                    user.welcomeTitle = dv.Item(0).Item("welcomeTitle")
                    user.accessLevelId = dv.Item(0).Item("AccessLevelID")
                    user.isAdministrator = dv.Item(0).Item("isAdministrator")
                    user.defaultModuleId = dv.Item(0).Item("defaultModuleId")
                    user.isSuspended = dv.Item(0).Item("isSuspended")
                    user.suspendedReasonId = dv.Item(0).Item("suspendedReasonId")
                    user.UserGUID = dv.Item(0).Item("UserGUID")
                End If
            End If

        Catch ex As Exception

        End Try

        Return user

    End Function

    Function validateToken(ByVal token As String, ByVal sourcePage As String, ByVal sourceId As String) As webUser Implements IeTrack.validateToken
        Dim user As New webUser
        Dim dl As New DataLayer
        Dim dv As DataView

        Try
            dv = dl.ValidateToken(token, sourcePage, sourceId, "")
            If Not IsNothing(dv) Then
                If dv.Count = 1 Then
                    user.tokenCookie = ConfigurationManager.AppSettings("tokenCookie")
                    user.isValid = dv.Item(0).Item("isValid")
                    user.token = dv.Item(0).Item("token")
                    user.companyName = dv.Item(0).Item("companyName")
                    user.fullName = dv.Item(0).Item("fullName")
                    user.welcomeTitle = dv.Item(0).Item("welcomeTitle")
                    user.accessLevelId = dv.Item(0).Item("AccessLevelID")
                    user.isAdministrator = dv.Item(0).Item("isAdministrator")
                    user.defaultModuleId = dv.Item(0).Item("defaultModuleId")
                    user.isSuspended = dv.Item(0).Item("isSuspended")
                    user.suspendedReasonId = dv.Item(0).Item("suspendedReasonId")
                    user.UserGUID = dv.Item(0).Item("UserGUID")

                    Try
                        user.versionId = dv.Item(0).Item("VersionName")
                    Catch ex As Exception

                    End Try
                End If
            End If
        Catch ex As Exception

        End Try

        Return user

    End Function

#End Region

    Public Function GetDevicesList(ByVal token As String) As List(Of deviceDet) Implements IeTrack.GetDevicesList
        Dim devList As New List(Of deviceDet)
        Dim dev As deviceDet = Nothing

        Try
            dev = New deviceDet
            dev.id = "F3B90383-731A-4215-BE30-20174719B422"
            dev.name = "Van 1"
            dev.eventCode = "01"
            dev.eventName = "Ignition ON"
            dev.eventDate = "11/16/2012 10:37 PM"
            dev.address = "123 Main Street, Oklahoma City, OK"
            dev.speed = "0"
            dev.latitude = 35.448082
            dev.longitude = -97.583916
            devList.Add(dev)

            dev = New deviceDet
            dev.id = "9A30F48C-1405-4F99-86C0-FDF04610FA5C"
            dev.name = "Truck"
            dev.eventCode = "02"
            dev.eventName = "Ignition OFF"
            dev.eventDate = "11/16/2012 8:28 PM"
            dev.address = "1791 S Agnew Ave, Oklahoma City, OK 73108"
            dev.speed = "0"
            dev.latitude = 35.469818
            dev.longitude = -97.554703
            devList.Add(dev)

            dev = New deviceDet
            dev.id = "8842D063-A9D0-4B6B-899D-77B48294D536"
            dev.name = "Corvette"
            dev.eventCode = "03"
            dev.eventName = "In Transit"
            dev.eventDate = "11/16/2012 11:49 PM"
            dev.address = "I-44, Oklahoma City, OK 73119"
            dev.speed = "45"
            dev.latitude = 35.417633
            dev.longitude = -97.579697
            devList.Add(dev)

            dev = New deviceDet
            dev.id = "8842D063-A9D0-4B6B-899D-77B48294D536"
            dev.name = "Bus"
            dev.eventCode = "04"
            dev.eventName = "Idle"
            dev.eventDate = "11/16/2012 4:15 PM"
            dev.address = "7245 JBC Street, Oklahoma City, OK 73119"
            dev.speed = "0"
            dev.latitude = 35.469833
            dev.longitude = -97.585266
            devList.Add(dev)

        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetDeviceInfo(ByVal id As String) As deviceDet Implements IeTrack.GetDeviceInfo
        Dim dev As deviceDet = Nothing

        Try
            dev = New deviceDet
            dev.id = "F3B90383-731A-4215-BE30-20174719B422"
            dev.name = "Van 1"
            dev.eventCode = "01"
            dev.eventName = "Ignition ON"
            dev.eventDate = "11/16/2012 10:37 PM"
            dev.address = "123 Main Street, Oklahoma City, OK"
            dev.speed = "0"
            dev.latitude = 35.448082
            dev.longitude = -97.583916

        Catch ex As Exception

        End Try

        Return dev

    End Function

    Public Function GetDeviceById(ByVal token As String, ByVal deviceId As String) As deviceDet Implements IeTrack.GetDeviceById
        Dim dev As deviceDet = Nothing
        Dim dl As New DataLayer

        Try
            dev = dl.GetDeviceById(token, deviceId)
            'dev.id = "F3B90383-731A-4215-BE30-20174719B422"
            'dev.name = "Van 1"
            'dev.eventCode = "01"
            'dev.eventName = "Ignition ON"
            'dev.eventDate = "11/16/2012 10:37 PM"
            'dev.address = "123 Main Street, Oklahoma City, OK"
            'dev.speed = "0"
            'dev.latitude = 35.448082
            'dev.longitude = -97.583916

        Catch ex As Exception

        End Try

        Return dev

    End Function

    Public Function GetDevices(ByVal token As String, ByVal lastFetchOn As String, ByVal qtyPanels As String, ByVal devicesPerPanel As String) As List(Of deviceDet) Implements IeTrack.GetDevices
        Dim devList As New List(Of deviceDet)
        Dim dev As deviceDet = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""
        Dim intQtyPanels As Integer = 1
        Dim intDevicesPerPanel As Integer = 9999
        Dim intCurrentPanel As Integer = 1
        Dim intDevCounter As Integer = 0

        Try
            If IsNumeric(qtyPanels) Then
                intQtyPanels = CInt(qtyPanels)
                If intQtyPanels = 0 Then
                    intQtyPanels = 1
                End If
            End If
            If IsNumeric(devicesPerPanel) Then
                intDevicesPerPanel = CInt(devicesPerPanel)
                If intDevicesPerPanel = 0 Then
                    intDevicesPerPanel = 9999
                End If
            End If

            If Not BLCommon.IsGUID(token) Then
                dev = New deviceDet
                dev.token = token
                dev.isOk = False
                dev.msg = "INVALIDTOKEN"
                devList.Add(dev)

            Else
                dsData = dl.getDevices(token, lastFetchOn, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dsData) Then
                        If dsData.Tables.Count >= 2 Then
                            dvData = dsData.Tables(1).DefaultView
                            For Each drv In dvData
                                dev = New deviceDet

                                intDevCounter += 1
                                If intDevCounter > intDevicesPerPanel Then
                                    intCurrentPanel += 1
                                    intDevCounter = 1
                                End If

                                dev.panelId = intCurrentPanel
                                dev.token = token
                                dev.id = drv.item("GUID")
                                dev.name = drv.item("Name")
                                dev.eventCode = drv.item("EventCode")
                                dev.eventName = drv.item("EventName")
                                dev.lastUpdatedOn = drv.item("LastUpdatedOn").ToString
                                dev.eventDate = drv.item("EventDate").ToString
                                dev.address = drv.item("fullAddress")
                                If dev.address.Length = 0 Then
                                    dev.address = "Processing Location of " & drv.item("Latitude").ToString & ", " & drv.item("Longitude").ToString
                                End If
                                dev.driverName = drv.item("DriverName")
                                dev.speed = drv.item("Speed")
                                dev.heading = drv.item("Heading")
                                dev.latitude = drv.item("Latitude")
                                dev.longitude = drv.item("Longitude")
                                dev.iconUrl = drv.item("IconURL")
                                dev.hasInputs = drv.item("hasInputs")
                                dev.hasPortExpander = drv.item("hasPortExpander")
                                If drv.item("hasInputs") = True Then
                                    dev.sw1 = IIf(drv.item("sw1") = True, ledOn, ledOff)
                                    dev.sw2 = IIf(drv.item("sw2") = True, ledOn, ledOff)
                                    dev.sw3 = IIf(drv.item("sw3") = True, ledOn, ledOff)
                                    dev.sw4 = IIf(drv.item("sw4") = True, ledOn, ledOff)
                                End If
                                If drv.item("hasPortExpander") = True Then
                                    dev.pto1 = IIf(drv.item("pto1") = True, ledOn, ledOff)
                                    dev.pto2 = IIf(drv.item("pto2") = True, ledOn, ledOff)
                                    dev.pto3 = IIf(drv.item("pto3") = True, ledOn, ledOff)
                                    dev.pto4 = IIf(drv.item("pto4") = True, ledOn, ledOff)
                                    dev.pto5 = IIf(drv.item("pto5") = True, ledOn, ledOff)
                                    dev.pto6 = IIf(drv.item("pto6") = True, ledOn, ledOff)
                                    dev.pto7 = IIf(drv.item("pto7") = True, ledOn, ledOff)
                                    dev.pto8 = IIf(drv.item("pto8") = True, ledOn, ledOff)
                                Else
                                    dev.pto1 = ledOff
                                    dev.pto2 = ledOff
                                    dev.pto3 = ledOff
                                    dev.pto4 = ledOff
                                    dev.pto5 = ledOff
                                    dev.pto6 = ledOff
                                    dev.pto7 = ledOff
                                    dev.pto8 = ledOff
                                End If
                                dev.temp1 = drv.item("Temperature1")
                                dev.temp2 = drv.item("Temperature2")
                                dev.temp3 = drv.item("Temperature3")
                                dev.temp4 = drv.item("Temperature4")

                                dev.NameSensor1 = drv.item("sensor1")
                                dev.NameSensor2 = drv.item("sensor2")
                                dev.NameSensor3 = drv.item("sensor3")
                                dev.NameSensor4 = drv.item("sensor4")
                                dev.hasAssignments = drv.item("hasAssignments")

                                Try
                                    If drv.item("Relay1") = True Then
                                        dev.relay1 = "On"
                                    Else
                                        dev.relay1 = "Off"
                                    End If
                                    If drv.item("Relay2") = True Then
                                        dev.relay2 = "On"
                                    Else
                                        dev.relay2 = "Off"
                                    End If
                                    If drv.item("Relay3") = True Then
                                        dev.relay3 = "On"
                                    Else
                                        dev.relay3 = "Off"
                                    End If
                                    If drv.item("Relay4") = True Then
                                        dev.relay4 = "On"
                                    Else
                                        dev.relay4 = "Off"
                                    End If
                                Catch ex As Exception

                                End Try
                                dev.isOk = True
                                dev.msg = ""
                                devList.Add(dev)
                            Next
                        End If
                    End If

                    If devList.Count = 0 Then
                        dev = New deviceDet
                        dev.token = token
                        dev.isOk = False
                        dev.msg = "NODATAFOUND"
                        devList.Add(dev)
                    End If
                Else
                    dev = New deviceDet
                    dev.token = token
                    dev.isOk = False
                    dev.msg = msg
                    devList.Add(dev)
                End If
            End If

        Catch ex As Exception

        End Try

        Return devList

    End Function
    Public Function GetDevicesBrokerOrder(ByVal token As String, ByVal lastFetchOn As String, ByVal qtyPanels As String, ByVal devicesPerPanel As String) As List(Of deviceDet) Implements IeTrack.GetDevicesBrokerOrder
        Dim devList As New List(Of deviceDet)
        Dim dev As deviceDet = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""
        Dim intQtyPanels As Integer = 1
        Dim intDevicesPerPanel As Integer = 9999
        Dim intCurrentPanel As Integer = 1
        Dim intDevCounter As Integer = 0

        Try
            If IsNumeric(qtyPanels) Then
                intQtyPanels = CInt(qtyPanels)
                If intQtyPanels = 0 Then
                    intQtyPanels = 1
                End If
            End If
            If IsNumeric(devicesPerPanel) Then
                intDevicesPerPanel = CInt(devicesPerPanel)
                If intDevicesPerPanel = 0 Then
                    intDevicesPerPanel = 9999
                End If
            End If

            If Not BLCommon.IsGUID(token) Then
                dev = New deviceDet
                dev.token = token
                dev.isOk = False
                dev.msg = "INVALIDTOKEN"
                devList.Add(dev)

            Else
                dsData = dl.getDevicesBrokers(token, lastFetchOn, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dsData) Then
                        If dsData.Tables.Count >= 2 Then
                            dvData = dsData.Tables(1).DefaultView
                            For Each drv In dvData
                                dev = New deviceDet

                                intDevCounter += 1
                                If intDevCounter > intDevicesPerPanel Then
                                    intCurrentPanel += 1
                                    intDevCounter = 1
                                End If

                                dev.panelId = intCurrentPanel
                                dev.token = token
                                dev.id = drv.item("GUID")
                                dev.name = drv.item("Name")
                                dev.eventCode = drv.item("EventCode")
                                dev.eventName = drv.item("EventName")
                                dev.lastUpdatedOn = drv.item("LastUpdatedOn").ToString
                                dev.eventDate = drv.item("EventDate").ToString
                                dev.address = drv.item("fullAddress")
                                If dev.address.Length = 0 Then
                                    dev.address = "Processing Location of " & drv.item("Latitude").ToString & ", " & drv.item("Longitude").ToString
                                End If
                                dev.driverName = drv.item("DriverName")
                                dev.speed = drv.item("Speed")
                                dev.heading = drv.item("Heading")
                                dev.latitude = drv.item("Latitude")
                                dev.longitude = drv.item("Longitude")
                                dev.iconUrl = drv.item("IconURL")
                                dev.hasInputs = drv.item("hasInputs")
                                dev.hasPortExpander = drv.item("hasPortExpander")
                                If drv.item("hasInputs") = True Then
                                    dev.sw1 = IIf(drv.item("sw1") = True, ledOn, ledOff)
                                    dev.sw2 = IIf(drv.item("sw2") = True, ledOn, ledOff)
                                    dev.sw3 = IIf(drv.item("sw3") = True, ledOn, ledOff)
                                    dev.sw4 = IIf(drv.item("sw4") = True, ledOn, ledOff)
                                End If
                                If drv.item("hasPortExpander") = True Then
                                    dev.pto1 = IIf(drv.item("pto1") = True, ledOn, ledOff)
                                    dev.pto2 = IIf(drv.item("pto2") = True, ledOn, ledOff)
                                    dev.pto3 = IIf(drv.item("pto3") = True, ledOn, ledOff)
                                    dev.pto4 = IIf(drv.item("pto4") = True, ledOn, ledOff)
                                    dev.pto5 = IIf(drv.item("pto5") = True, ledOn, ledOff)
                                    dev.pto6 = IIf(drv.item("pto6") = True, ledOn, ledOff)
                                    dev.pto7 = IIf(drv.item("pto7") = True, ledOn, ledOff)
                                    dev.pto8 = IIf(drv.item("pto8") = True, ledOn, ledOff)
                                Else
                                    dev.pto1 = ledOff
                                    dev.pto2 = ledOff
                                    dev.pto3 = ledOff
                                    dev.pto4 = ledOff
                                    dev.pto5 = ledOff
                                    dev.pto6 = ledOff
                                    dev.pto7 = ledOff
                                    dev.pto8 = ledOff
                                End If
                                dev.temp1 = drv.item("Temperature1")
                                dev.temp2 = drv.item("Temperature2")
                                dev.temp3 = drv.item("Temperature3")
                                dev.temp4 = drv.item("Temperature4")

                                dev.NameSensor1 = drv.item("sensor1")
                                dev.NameSensor2 = drv.item("sensor2")
                                dev.NameSensor3 = drv.item("sensor3")
                                dev.NameSensor4 = drv.item("sensor4")
                                dev.hasAssignments = drv.item("hasAssignments")
                                dev.jobuniqueKey = drv.item("jobuniqueKey")

                                Try
                                    If drv.item("Relay1") = True Then
                                        dev.relay1 = "On"
                                    Else
                                        dev.relay1 = "Off"
                                    End If
                                    If drv.item("Relay2") = True Then
                                        dev.relay2 = "On"
                                    Else
                                        dev.relay2 = "Off"
                                    End If
                                    If drv.item("Relay3") = True Then
                                        dev.relay3 = "On"
                                    Else
                                        dev.relay3 = "Off"
                                    End If
                                    If drv.item("Relay4") = True Then
                                        dev.relay4 = "On"
                                    Else
                                        dev.relay4 = "Off"
                                    End If
                                Catch ex As Exception

                                End Try
                                dev.isOk = True
                                dev.msg = ""
                                devList.Add(dev)
                            Next
                        End If
                    End If

                    If devList.Count = 0 Then
                        dev = New deviceDet
                        dev.token = token
                        dev.isOk = False
                        dev.msg = "NODATAFOUND"
                        devList.Add(dev)
                    End If
                Else
                    dev = New deviceDet
                    dev.token = token
                    dev.isOk = False
                    dev.msg = msg
                    devList.Add(dev)
                End If
            End If
        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetMainSchedules(ByVal token As String, ByVal deviceId As String, ByVal taskId As String) As List(Of maintSchedule) Implements IeTrack.GetMaintScheduled
        Dim lst As New List(Of maintSchedule)
        Dim itm As maintSchedule = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""
        Dim intTaskId As Integer = 0

        Try
            If IsNumeric(taskId) Then
                intTaskId = CInt(taskId)
            End If
            If Not BLCommon.IsGUID(token) Then
                itm = New maintSchedule
                itm.token = token
                itm.isOk = False
                itm.msg = "INVALIDTOKEN"
                lst.Add(itm)

            Else
                dtData = dl.getMaintSchedules(token, deviceId, intTaskId, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            itm = New maintSchedule
                            itm.token = token
                            itm.isOk = True
                            itm.msg = ""
                            itm.id = drv.item("ID")
                            itm.deviceId = drv.item("DeviceID")
                            itm.deviceName = drv.item("DeviceName")
                            itm.taskId = drv.item("TaskID")
                            itm.taskName = drv.item("TaskName")
                            itm.taskMeassureId = drv.item("TaskMeassureID")
                            itm.taskMeassureName = drv.item("TaskMeassureName")
                            itm.taskValue = drv.item("TaskValue")

                            If drv.Item("LastServiceOn") = "1/1/1900" Then
                                itm.lastServiceOn = "N/A"
                            Else
                                itm.lastServiceOn = drv.item("LastServiceOn").ToString
                            End If

                            itm.currentValue = drv.item("ValueSinceLastService")
                            itm.notifyBefore = drv.item("NotifyBefore")
                            itm.notifyEveryXDays = drv.item("NotifyEveryXDays")
                            itm.excludeWeekends = drv.item("ExcludeWeekends")
                            itm.createdOn = drv.item("CreatedOn").ToString
                            lst.Add(itm)
                        Next
                    End If

                    If lst.Count = 0 Then
                        itm = New maintSchedule
                        itm.token = token
                        itm.isOk = False
                        itm.msg = "NODATAFOUND"
                        lst.Add(itm)
                    End If
                Else
                    itm = New maintSchedule
                    itm.token = token
                    itm.isOk = False
                    itm.msg = msg
                    lst.Add(itm)
                End If
            End If

        Catch ex As Exception
            itm = New maintSchedule
            itm.token = token
            itm.isOk = False
            itm.msg = msg
            lst.Add(itm)
        End Try

        Return lst

    End Function

    Public Function GetUsersBasicInfo(ByVal token As String) As List(Of userBasicInfo) Implements IeTrack.GetUsersBasicInfo
        Dim lst As New List(Of userBasicInfo)
        Dim itm As userBasicInfo = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                itm = New userBasicInfo
                itm.token = token
                itm.isOk = False
                itm.msg = "INVALIDTOKEN"
                lst.Add(itm)
            Else
                dtData = dl.getUsersBasicInfo(token, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            itm = New userBasicInfo
                            itm.token = token
                            itm.isOk = True
                            itm.msg = ""
                            itm.id = drv.item("ID")
                            itm.firstName = drv.item("FirstName")
                            itm.lastName = drv.item("LastName")
                            itm.fullName = drv.item("FullName")
                            itm.email = drv.item("Email")
                            lst.Add(itm)
                        Next
                    End If

                    If lst.Count = 0 Then
                        itm = New userBasicInfo
                        itm.token = token
                        itm.isOk = False
                        itm.msg = "NODATAFOUND"
                        lst.Add(itm)
                    End If
                Else
                    itm = New userBasicInfo
                    itm.token = token
                    itm.isOk = False
                    itm.msg = msg
                    lst.Add(itm)
                End If
            End If

        Catch ex As Exception
            itm = New userBasicInfo
            itm.token = token
            itm.isOk = False
            itm.msg = msg
            lst.Add(itm)
        End Try

        Return lst

    End Function

    Public Function GetNotificationsSendTo(ByVal token As String, ByVal entityName As String, ByVal entityId As String) As List(Of notificationSendTo) Implements IeTrack.GetNotificationsSendTo
        Dim lst As New List(Of notificationSendTo)
        Dim itm As notificationSendTo = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                itm = New notificationSendTo
                itm.token = token
                itm.isOk = False
                itm.msg = "INVALIDTOKEN"
                lst.Add(itm)
            Else
                dtData = dl.getNotificationsSendTo(token, entityName, entityId, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            itm = New notificationSendTo
                            itm.token = token
                            itm.isOk = True
                            itm.msg = ""
                            itm.entityId = drv.item("EntityID")
                            itm.sendToId = drv.item("SendToID")
                            itm.userId = drv.item("UserID")
                            itm.firstName = drv.item("FirstName")
                            itm.lastName = drv.item("LastName")
                            itm.fullName = drv.item("FullName")
                            itm.isEmail = drv.item("IsEmail")
                            itm.isSms = drv.item("IsSMS")
                            lst.Add(itm)
                        Next
                    End If

                    If lst.Count = 0 Then
                        itm = New notificationSendTo
                        itm.token = token
                        itm.isOk = False
                        itm.msg = "NODATAFOUND"
                        lst.Add(itm)
                    End If
                Else
                    itm = New notificationSendTo
                    itm.token = token
                    itm.isOk = False
                    itm.msg = msg
                    lst.Add(itm)
                End If
            End If

        Catch ex As Exception
            itm = New notificationSendTo
            itm.token = token
            itm.isOk = False
            itm.msg = msg
            lst.Add(itm)
        End Try

        Return lst

    End Function

    Public Function GetHotSpots(ByVal token As String, ByVal deviceId As String) As List(Of hotSpots) Implements IeTrack.GetHotSpots
        Dim lst As New List(Of hotSpots)
        Dim itm As hotSpots = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                itm = New hotSpots
                itm.token = token
                itm.isOk = False
                itm.msg = "INVALIDTOKEN"
                lst.Add(itm)
            Else
                dtData = dl.GetHotSpots(token, deviceId, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            itm = New hotSpots
                            itm.token = token
                            itm.isOk = True
                            itm.msg = ""
                            itm.lat = drv.item("Latitude")
                            itm.lng = drv.item("Longitude")
                            itm.address = drv.item("FullAddress")
                            itm.qty = drv.item("Qty")
                            itm.lastVisitOn = drv.item("LastVisitOn").ToString
                            lst.Add(itm)
                        Next
                    End If

                    If lst.Count = 0 Then
                        itm = New hotSpots
                        itm.token = token
                        itm.isOk = False
                        itm.msg = "NODATAFOUND"
                        lst.Add(itm)
                    End If
                Else
                    itm = New hotSpots
                    itm.token = token
                    itm.isOk = False
                    itm.msg = msg
                    lst.Add(itm)
                End If
            End If

        Catch ex As Exception
            itm = New hotSpots
            itm.token = token
            itm.isOk = False
            itm.msg = msg
            lst.Add(itm)
        End Try

        Return lst

    End Function

    Public Function GetBasicList(ByVal token As String, ByVal entityName As String) As List(Of basicList) Implements IeTrack.GetBasicList
        Dim lst As New List(Of basicList)
        Dim itm As basicList = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                itm = New basicList
                itm.id = "" = token
                itm.value = ""
                lst.Add(itm)
            Else
                dtData = dl.GetBasicList(token, entityName, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            itm = New basicList
                            itm.id = drv.item("ID")
                            itm.value = drv.item("Value")
                            lst.Add(itm)
                        Next
                    End If

                    If lst.Count = 0 Then
                        itm = New basicList
                        itm.id = "" = token
                        itm.value = ""
                        lst.Add(itm)
                    End If
                Else
                    itm = New basicList
                    itm.id = "" = token
                    itm.value = ""
                    lst.Add(itm)
                End If
            End If

        Catch ex As Exception
            itm = New basicList
            itm.id = "" = token
            itm.value = ""
            lst.Add(itm)
        End Try

        Return lst

    End Function

    Public Function GetIdNameList(ByVal token As String, ByVal noCache As String, ByVal listName As String) As List(Of basicList) Implements IeTrack.GetIdNameList
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.GetIdNameList(token, listName)
        Catch ex As Exception

        End Try

        Return lst

    End Function




#Region "Geofences"

    Public Function getGeofencesCustomMessages(ByVal token As String, ByVal msgType As String) As List(Of basicList) Implements IeTrack.getGeofencesCustomMessages
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.getGeofencesCustomMessages(token, msgType)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveGeofence(ByVal data As geofenceClass) As responseOk Implements IeTrack.saveGeofence
        Dim res As New responseOk
        Dim dl As New DataLayer
        Dim guid As String = ""

        Try
            If BLCommon.IsGUID(data.token) Then

                If data.shapeId = 2 Then
                    BLCommon.getFormattedData(data.jsonPolyVerticesTXT, data.KMLData, data.SQLData)
                Else
                    BLCommon.getFormattedData(data.latitude, data.longitude, data.KMLData, data.SQLData)
                End If

                guid = dl.saveGeofence(data.token, data.id, data.geofenceTypeId, data.name,
                                       data.contactName, data.phone, data.contactEmail, data.contactSMSAlert, data.contactEmailAlert, data.contactAlertTypeId,
                                       data.fullAddress, data.street, data.streetNumber,
                                       data.route, data.suite, data.city, data.county, data.state, data.postalCode, data.country, data.latitude, data.longitude, data.geofenceAlertTypeId,
                                       data.radius, data.comments, data.shapeId, data.jsonPolyVerticesTXT, data.KMLData, data.SQLData, data.isSpeedLimit, data.speedLimit,
                                       data.arrivalMsgId, data.arrivalMsgTxt, data.departureMsgId, data.departureMsgTxt, data.IsStopForJob,
                                       res.msg)
            Else
                res.isOk = False
                res.msg = "Invalid Token"
            End If

            If guid.Length > 0 Then
                res.isOk = True
                res.msg = ""
            Else
                res.isOk = False
            End If

        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function geofence_validateName(ByVal token As String, ByVal id As String, ByVal name As String) As responseOk Implements IeTrack.geofence_validateName
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.geofence_validateName(token, id, name)
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "Customers / Geofences"

    Public Function CustomerSearch(ByVal token As String, ByVal noCache As String, ByVal custName As String) As List(Of customerSearch) Implements IeTrack.CustomerSearch
        Dim dl As New DataLayer
        Dim lst As New List(Of customerSearch)

        Try
            lst = dl.CustomerSearch(token, custName)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Devices"

    Function searchDevice(ByVal token As String, ByVal searchKey As String, ByVal keyValue As String, ByVal noCache As String) As searchDeviceResult Implements IeTrack.searchDevice
        Dim dev As New searchDeviceResult
        Dim dl As New DataLayer

        Try
            dev = dl.searchDevice(token, searchKey, keyValue)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.etrest.svc", "", "searchDevice", ex.Message, 0)
        End Try

        Return dev

    End Function

    Function deviceAction(ByVal token As String, ByVal data As devAction) As callResult Implements IeTrack.deviceAction
        Dim res As New callResult
        Dim dl As New DataLayer

        Try
            res = dl.deviceAction(token, data.action, data.id, data.param1, data.param2)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.etrest.svc", "", "deviceAction", ex.Message, 0)
        End Try

        Return res

    End Function

    Function crm_getAllCompanies(ByVal token As String) As List(Of basicList) Implements IeTrack.crm_getAllCompanies
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.crm_getAllCompanies(token)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.etrest.svc", "", "crm_getAllCompanies", ex.Message, 0)
        End Try

        Return lst

    End Function

#End Region

#Region "Inventory"

    Public Function getInventory(ByVal token As String) As List(Of inventory) Implements IeTrack.getInventory
        Dim lst As New List(Of inventory)
        Dim dl As New DataLayer

        Try
            lst = dl.getInventory(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveAssignment(ByVal token As String, ByVal data As assignedInventory) As assignedInventory Implements IeTrack.saveAssignment
        Dim dl As New DataLayer

        Try
            For Each dev In data.inventory
                dl.assignDevice(token, data.custId, data.courrierId, data.trackingNumber, dev)
            Next

            'Send email
            dl.sendEmail(token, data.custId, data.emailTypeId, data.courrierId, data.trackingNumber)

        Catch ex As Exception

        End Try

        Return data

    End Function

#End Region

#Region "Suspend/Reactivate Company"

    Public Function GetCompaniesSuspendedReasons(ByVal token As String) As List(Of basicList) Implements IeTrack.GetCompaniesSuspendedReasons
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.GetCompaniesSuspendedReasons()
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getSuspendCompanies(ByVal token As String) As List(Of Company) Implements IeTrack.getSuspendCompanies
        Dim lst As New List(Of Company)
        Dim dl As New DataLayer

        Try
            lst = dl.getCompaniesSuspendResume(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveSuspendCompany(ByVal token As String, ByVal data As Company) As Company Implements IeTrack.saveSuspendCompany
        Dim dl As New DataLayer

        Try
            dl.saveSuspendCompany(token, data)
        Catch ex As Exception

        End Try

        Return data

    End Function

#End Region

#Region "Telemetry"

    Public Function GetIOs(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devIOs Implements IeTrack.GetIOs
        Dim itm As New devIOs
        Dim dl As New DataLayer

        Try
            itm = dl.GetIOs(token, deviceId)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function GetAllDevIOs(ByVal token As String, ByVal noCache As String) As List(Of devIOs) Implements IeTrack.GetAllDevIOs
        Dim lst As New List(Of devIOs)
        Dim dl As New DataLayer

        Try
            lst = dl.GetAllDevIOs(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function setOutput(ByVal token As String, ByVal data As setRelay) As responseOk Implements IeTrack.setOutput
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.setOutput(token, data, res.msg)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

    Public Function telemetrySetUp(ByVal token As String, ByVal data As ioSetUp) As responseOk Implements IeTrack.telemetrySetUp
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.telemetrySetUp(token, data, res.msg)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

#End Region

#Region "Hour Meters"

    Public Function GetDevMeters(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devInputsOnTime Implements IeTrack.GetDevMeters
        Dim itm As New devInputsOnTime
        Dim dl As New DataLayer

        Try
            itm = dl.GetDevMeters(token, deviceId)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function GetAllDevMeters(ByVal token As String, ByVal noCache As String) As List(Of devInputsOnTime) Implements IeTrack.GetAllDevMeters
        Dim lst As New List(Of devInputsOnTime)
        Dim dl As New DataLayer

        Try
            lst = dl.GetAllDevMeters(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveHourMeter(ByVal token As String, ByVal deviceId As String, ByVal data As devInputsOnTime) As responseOk Implements IeTrack.saveHourMeter
        Dim res As New responseOk
        Dim dl As New DataLayer
        Dim dev As New devInputsOnTimeTransformed

        Try
            dev.deviceId = data.deviceId
            dev.ignitionOnTime = data.ignitionOnTime * 3600
            If IsDate(data.ignitionLastSetOn) Then
                dev.ignitionSetOn = CDate(data.ignitionLastSetOn)
            Else
                dev.ignitionSetOn = Date.Now
            End If

            dev.input1OnTime = data.input1OnTime * 3600
            If IsDate(data.input1LastSetOn) Then
                dev.input1SetOn = CDate(data.input1LastSetOn)
            Else
                dev.input1SetOn = Date.Now
            End If

            dev.input2OnTime = data.input2OnTime * 3600
            If IsDate(data.input2LastSetOn) Then
                dev.input2SetOn = CDate(data.input2LastSetOn)
            Else
                dev.input2SetOn = Date.Now
            End If

            dev.input3OnTime = data.input3OnTime * 3600
            If IsDate(data.input3LastSetOn) Then
                dev.input3SetOn = CDate(data.input3LastSetOn)
            Else
                dev.input3SetOn = Date.Now
            End If

            dev.input4OnTime = data.input4OnTime * 3600
            If IsDate(data.input4LastSetOn) Then
                dev.input4SetOn = CDate(data.input4LastSetOn)
            Else
                dev.input4SetOn = Date.Now
            End If

            res.isOk = dl.saveHourMeter(token, dev, res.msg)

        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

#End Region

#Region "Geofence Types"

    Public Function getAllGeofenceTypes(ByVal token As String, ByVal noCache As String) As List(Of GeofenceType) Implements IeTrack.getAllGeofenceTypes
        Dim lst As New List(Of GeofenceType)
        Dim dl As New DataLayer

        Try
            lst = dl.getAllGeofenceTypes(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveGeofenceType(ByVal token As String, ByVal noCache As String, ByVal data As GeofenceType) As responseOk Implements IeTrack.saveGeofenceType
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveGeofenceType(token, data, res.msg)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

    Public Function deleteGeofenceType(ByVal token As String, ByVal id As String) As responseOk Implements IeTrack.deleteGeofenceType
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.deleteGeofenceType(token, id, res.msg)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

#End Region

#Region "Maintenance"

    Public Function maintSupportListsGET(ByVal token As String, ByVal noCache As String) As maintSupportLists Implements IeTrack.maintSupportListsGET
        Dim itm As New maintSupportLists
        Dim dl As New DataLayer

        Try
            itm = dl.maintSupportListsGET(token)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function maintDeviceList(ByVal token As String, ByVal noCache As String) As List(Of maintDevice) Implements IeTrack.maintDeviceList
        Dim lst As New List(Of maintDevice)
        Dim dl As New DataLayer

        Try
            lst = dl.maintDeviceList(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function maintDeviceSave(ByVal token As String, ByVal deviceId As String, ByVal data As maintDevice) As maintDevice Implements IeTrack.maintDeviceSave
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            If IsDate(data.insuranceDueOn) Then
                data.datInsuranceDueOn = CDate(data.insuranceDueOn)
            Else
                data.datInsuranceDueOn = Date.Now
            End If

            res.isOk = dl.maintDeviceSave(token, data, res.msg)

        Catch ex As Exception

        End Try

        Return data

    End Function

    Public Function maintScheduleGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceSchedule Implements IeTrack.maintScheduleGetByDevice
        Dim lst As New maintDeviceSchedule
        Dim dl As New DataLayer

        Try
            lst = dl.maintScheduleGetByDevice(token, deviceId, "")
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function maintItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem Implements IeTrack.maintItemSave
        Dim itm As New scheduleItem
        Dim dl As New DataLayer

        Try
            If itm.isNew Then
                itm.id = ""
            End If
            itm.isOk = dl.maintItemSave(token, deviceId, data, itm.id, itm.msg)
            If itm.isOk Then
                Dim devSchedule As New maintDeviceSchedule
                devSchedule = dl.maintScheduleGetByDevice(token, deviceId, itm.id)
                itm = devSchedule.schedules(0)
            End If
        Catch ex As Exception
            itm.isOk = False
            itm.msg = ex.Message
        End Try

        Return itm

    End Function

    Public Function maintItemGet(ByVal token As String, ByVal noCache As String, ByVal deviceId As String, ByVal id As String) As scheduleItem Implements IeTrack.maintItemGet
        Dim itm As New scheduleItem
        Dim dl As New DataLayer

        Try
            Dim devSchedule As New maintDeviceSchedule
            devSchedule = dl.maintScheduleGetByDevice(token, deviceId, id)
            itm = devSchedule.schedules(0)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function maintItemDelete(ByVal token As String, ByVal deviceId As String, ByVal id As String) As responseOk Implements IeTrack.maintItemDelete
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.maintItemDelete(token, id, res.msg)
            If res.isOk Then
                res.transId = id
            End If
        Catch ex As Exception
            res.isOk = False
            res.msg = ex.Message
        End Try

        Return res

    End Function

    Public Function maintCompletedItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem Implements IeTrack.maintCompletedItemSave
        Dim itm As New scheduleItem
        Dim dl As New DataLayer

        Try
            'itm.isOk = dl.maintCompletedItemSave(token, deviceId, data, itm.id, itm.msg)
            If itm.isOk Then
                Dim devSchedule As New maintDeviceSchedule
                If data.id.Length > 0 Then
                    devSchedule = dl.maintScheduleGetByDevice(token, deviceId, data.id)
                    itm = devSchedule.schedules(0)
                End If
            End If
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function maintLogGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceLog Implements IeTrack.maintLogGetByDevice
        Dim itm As New maintDeviceLog
        Dim dl As New DataLayer

        Try
            itm = dl.maintLogGetByDevice(token, deviceId, "")
        Catch ex As Exception

        End Try

        Return itm

    End Function

#End Region

#Region "QB Match"

    Public Function getQBMatchCustomers(ByVal token As String) As qbMatch Implements IeTrack.getQBMatchCustomers
        Dim itm As New qbMatch
        Dim isValidUser As Boolean
        Dim dl As New DataLayer

        Try
            itm.crmCustomers = dl.QBMatch_CRMCustomers(token, isValidUser)
            If isValidUser Then
                itm.qbCustomers = dl.QBMatch_QBCustomers()
            End If
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function qbLinkCustomers(ByVal token As String, ByVal crmId As String, ByVal qbId As String) As responseOk Implements IeTrack.qbLinkCustomers
        Dim itm As New responseOk
        Dim dl As New DataLayer

        Try
            itm.isOk = dl.qbLinkCustomers(token, crmId, qbId)
        Catch ex As Exception
            itm.isOk = False
        End Try

        Return itm

    End Function

    Public Function qbUnLinkCustomers(ByVal token As String, ByVal crmId As String) As responseOk Implements IeTrack.qbUnLinkCustomers
        Dim itm As New responseOk
        Dim dl As New DataLayer

        Try
            itm.isOk = dl.qbLinkCustomers(token, crmId, "")
        Catch ex As Exception
            itm.isOk = False
        End Try

        Return itm

    End Function

#End Region

#Region "User Preferences"

    Function updateUserPref(ByVal token As String, ByVal data As userPreference) As responseOk Implements IeTrack.updateUserPref
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.updateUserPref(token, data)

        Catch ex As Exception
            res.isOk = False
            res.msg = ex.Message
        End Try

        Return res

    End Function
    Function updateUserPrefGroup(ByVal token As String, ByVal data As List(Of userPreference)) As responseOk Implements IeTrack.updateUserPrefGroup
        Dim res As New responseOk
        Dim dl As New DataLayer
        Dim groupsID As String = ""
        Try
            For Each item As userPreference In data
                groupsID += item.val1 + ","
                res.isOk = dl.updateUserPref(token, item)
            Next
            groupsID = Left(groupsID, Len(groupsID) - 1)
            res.isOk = dl.updateInactivePreferences(token, groupsID)
        Catch ex As Exception
            res.isOk = False
            res.msg = ex.Message
        End Try

        Return res

    End Function

#End Region

#Region "CRM Customers"

    Public Function GetCompanies(ByVal token As String) As List(Of basicList) Implements IeTrack.GetCompanies
        Dim dl As New DataLayer
        Dim lst As New List(Of basicList)

        Try
            lst = dl.getCompanies(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function GetCompanyDetails(ByVal token As String, ByVal id As String, ByVal noCache As String) As crm_CustomerDetails Implements IeTrack.GetCompanyDetails
        Dim dl As New DataLayer
        Dim itm As New crm_CustomerDetails

        Try
            'lst = dl.getCompanies(token)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function saveNewCustomer(ByVal token As String, ByVal data As CRM_Customer) As CRM_Customer Implements IeTrack.saveNewCustomer
        Dim dl As New DataLayer

        Try
            data.id = dl.saveNewCustomer(token, data)
        Catch ex As Exception

        End Try

        Return data

    End Function

#End Region

#Region "CRM Related"

    Public Function EmailTypesGET(ByVal token As String, ByVal type As String) As List(Of basicList) Implements IeTrack.EmailTypesGET
        Dim dl As New DataLayer
        Dim lst As New List(Of basicList)

        Try
            lst = dl.EmailTypesGET(token, type)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function GenericMastersGET(ByVal token As String, ByVal masterKey As String) As List(Of basicList) Implements IeTrack.GenericMastersGET
        Dim dl As New DataLayer
        Dim lst As New List(Of basicList)

        Try
            lst = dl.GenericMastersGET(token, masterKey)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "CRM - Invoices"

    Function crmGetInvoices(ByVal token As String, ByVal custId As String, ByVal noCache As String) As List(Of invoice) Implements IeTrack.crmGetInvoices
        Dim lst As New List(Of invoice)
        Dim dl As New DataLayer

        Try
            lst = dl.crmGetInvoices(token, custId)
            If lst.Count = 0 Then
                Dim i As New invoice
                i.paid = 0
                lst.Add(i)
            End If
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "IfByPhone calls"

    Function saveIncomingCalls(ByVal dateTime As String,
                               ByVal sid As String,
                               ByVal callType As String,
                               ByVal firstAction As String,
                               ByVal lastAction As String,
                               ByVal calledNumber As String,
                               ByVal callerId As String,
                               ByVal transferType As String,
                               ByVal transferredToNumber As String,
                               ByVal callTransferStatus As String,
                               ByVal phoneLabel As String,
                               ByVal callDuration As String,
                               ByVal talkMinutes As String) As Boolean Implements IeTrack.saveIncomingCalls

        Dim dl As New DataLayer

        Try
            dl.saveIncomingCalls(dateTime, sid, callType, firstAction, lastAction, calledNumber, callerId, transferType, transferredToNumber, callTransferStatus, phoneLabel, callDuration, talkMinutes)
        Catch ex As Exception

        End Try

        Return True

    End Function

    Function smsReplyCatcher(ByVal from_number As String, ByVal message As String, ByVal to_number As String) As Boolean Implements IeTrack.smsReplyCatcher
        Dim res As Boolean = True
        Dim dl As New DataLayer
        Dim data As New smsReply

        Try
            data.message = message
            data.to = to_number
            data.from = from_number
            res = dl.smsReplyCatcher(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region


#Region "Fleet HeartBeat"

    Function getFleetHeartBeat(ByVal token As String, ByVal noCache As String) As fleetHeartBeat Implements IeTrack.getFleetHeartBeat
        Dim f As New fleetHeartBeat
        Dim dl As New DataLayer

        Try
            f = dl.getFleetHeartBeat(token)
        Catch ex As Exception

        End Try

        Return f

    End Function

    Function fleetheartbeatHist(ByVal token As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As fleetHeartBeat Implements IeTrack.fleetheartbeatHist
        Dim f As New fleetHeartBeat
        Dim dl As New DataLayer

        Try

        Catch ex As Exception

        End Try

        Return f

    End Function

#End Region

#Region "Sensors"
    Public Function PostTempSensor(ByVal token As String, ByVal data As TempSensors) As responseSensor Implements IeTrack.PostTempSensor
        Dim dl As New DataLayer
        Dim resonse As New responseSensor
        resonse.isOk = dl.PostTempSensor(token, data, data.Action)
        Return resonse
    End Function


#End Region

End Class
