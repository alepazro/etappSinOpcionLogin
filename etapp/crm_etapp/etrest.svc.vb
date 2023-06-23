Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter
Imports System.Web


' NOTE: You can use the "Rename" command on the context menu to change the class name "etrest" in code, svc and config file together.
Public Class etrest
    Implements Ietrest

    Public ledOn As String = "https://www.easitrack.net/icons/green_light15x15.png"
    Public ledOff As String = "https://www.easitrack.net/icons/red_light15x15.png"

    Public Function doDummyCall(ByVal noCache As String) As Boolean Implements Ietrest.doDummyCall
        Try

        Catch ex As Exception

        End Try

        Return True

    End Function

    Public Function Authorization(ByVal credentials As String) As userBasicInfo Implements Ietrest.Authorization
        Dim u As userBasicInfo = Nothing

        Try
            u = New userBasicInfo
            u.firstName = "demo"
            u.lastName = "test"
            u.token = "asd-qwe-xcv-qwe-sdf-qwe"

        Catch ex As Exception

        End Try

        Return u

    End Function

    Public Function GetDevicesList(ByVal token As String) As List(Of device) Implements Ietrest.GetDevicesList
        Dim devList As New List(Of device)
        Dim dev As device = Nothing

        Try
            dev = New device
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

            dev = New device
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

            dev = New device
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

            dev = New device
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

    Public Function GetDeviceInfo(ByVal id As String) As device Implements Ietrest.GetDeviceInfo
        Dim dev As device = Nothing

        Try
            dev = New device
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

    Public Function GetDeviceById(ByVal token As String, ByVal deviceId As String) As device Implements Ietrest.GetDeviceById
        Dim dev As device = Nothing
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

    Public Function GetDevices(ByVal token As String, ByVal lastFetchOn As String, ByVal qtyPanels As String, ByVal devicesPerPanel As String) As List(Of device) Implements Ietrest.GetDevices
        Dim devList As New List(Of device)
        Dim dev As device = Nothing
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
                dev = New device
                dev.token = token
                dev.isOk = False
                dev.msg = "INVALIDTOKEN"
                devList.Add(dev)

            Else
                dsData = dl.getDevices(token, "", lastFetchOn, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dsData) Then
                        If dsData.Tables.Count >= 2 Then
                            dvData = dsData.Tables(1).DefaultView
                            For Each drv In dvData
                                dev = New device

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
                                    dev.address = "Location: (" & drv.item("Latitude").ToString & ", " & drv.item("Longitude") & ")"
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
                                End If
                                dev.temp1 = drv.item("Temperature1")
                                dev.temp2 = drv.item("Temperature2")
                                dev.temp3 = drv.item("Temperature3")
                                dev.temp4 = drv.item("Temperature4")
                                Try
                                    If drv.item("Relay1") = True Then
                                        dev.Relay1 = "On"
                                    Else
                                        dev.Relay1 = "Off"
                                    End If
                                    If drv.item("Relay2") = True Then
                                        dev.Relay2 = "On"
                                    Else
                                        dev.Relay2 = "Off"
                                    End If
                                    If drv.item("Relay3") = True Then
                                        dev.Relay3 = "On"
                                    Else
                                        dev.Relay3 = "Off"
                                    End If
                                    If drv.item("Relay4") = True Then
                                        dev.Relay4 = "On"
                                    Else
                                        dev.Relay4 = "Off"
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
                        dev = New device
                        dev.token = token
                        dev.isOk = False
                        dev.msg = "NODATAFOUND"
                        devList.Add(dev)
                    End If
                Else
                    dev = New device
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

    Public Function getMaintSchedules(ByVal token As String, ByVal deviceId As String, ByVal taskId As String, ByVal noCache As String) As List(Of maintSchedule) Implements Ietrest.getMaintSchedules
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
                            itm.taskValueStr = drv.item("TaskValue").ToString & " " & drv.item("TaskMeassureName")

                            If drv.Item("LastServiceOn") = "1/1/1900" Then
                                itm.lastServiceOn = "N/A"
                            Else
                                itm.lastServiceOn = drv.item("LastServiceOn").ToString
                            End If

                            itm.currentValue = drv.item("ValueSinceLastService")
                            itm.currentValueStr = drv.item("ValueSinceLastService").ToString & " " & drv.item("TaskMeassureName")
                            itm.nextServiceStr = (drv.item("TaskValue") - drv.item("ValueSinceLastService")).ToString & " " & drv.item("TaskMeassureName")
                            Try
                                itm.nextService = CDec(drv.item("TaskValue")) - CDec(drv.item("ValueSinceLastService"))
                            Catch ex As Exception
                                itm.nextService = 0
                            End Try
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

    Function getMaintHServices(ByVal token As String, ByVal deviceId As String, ByVal taskId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As List(Of maintHServices) Implements Ietrest.getMaintHServices
        Dim lst As New List(Of maintHServices)
        Dim dl As New DataLayer
        Dim strError As String = ""

        Try
            lst = dl.getHServices(token, deviceId, taskId, dateFrom, dateTo, strError)

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function getMaintHFuel(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As List(Of maintHFuel) Implements Ietrest.getMaintHFuel
        Dim lst As New List(Of maintHFuel)
        Dim dl As New DataLayer
        Dim strError As String = ""

        Try
            lst = dl.getHFuel(token, deviceId, dateFrom, dateTo, strError)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function GetUsersBasicInfo(ByVal token As String) As List(Of userBasicInfo) Implements Ietrest.GetUsersBasicInfo
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

    Public Function GetNotificationsSendTo(ByVal token As String, ByVal entityName As String, ByVal entityId As String) As List(Of notificationSendTo) Implements Ietrest.GetNotificationsSendTo
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

    Public Function GetHotSpots(ByVal token As String, ByVal deviceId As String) As List(Of hotSpots) Implements Ietrest.GetHotSpots
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

    Public Function GetBasicList(ByVal token As String, ByVal entityName As String) As List(Of basicList) Implements Ietrest.GetBasicList
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

    Public Function GetIdNameList(ByVal token As String, ByVal noCache As String, ByVal listName As String) As List(Of basicList) Implements Ietrest.GetIdNameList
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.GetIdNameList(token, listName)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getValue(ByVal token As String, ByVal noCache As String, ByVal valueName As String) As singleValue Implements Ietrest.getValue
        Dim itm As New singleValue
        Dim dl As New DataLayer

        Try
            itm = dl.getValue(token, valueName)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function getInfoWindow(ByVal token As String, ByVal noCache As String, ByVal deviceID As String) As deviceInfoWindow Implements Ietrest.getInfoWindow
        Dim dev As New deviceInfoWindow
        Dim dl As New DataLayer
        Dim msg As String = ""

        Try
            dev = dl.DeviceInfoWIndow_GET(token, deviceID, msg)
        Catch ex As Exception

        End Try

        Return dev

    End Function

#Region "CRM Customers"

    Public Function GetCompanies(ByVal token As String) As List(Of basicList) Implements Ietrest.GetCompanies
        Dim dl As New DataLayer
        Dim lst As New List(Of basicList)

        Try
            lst = dl.getCompanies(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function GetCompanyDetails(ByVal token As String, ByVal id As String, ByVal noCache As String) As crm_CustomerDetails Implements Ietrest.GetCompanyDetails
        Dim dl As New DataLayer
        Dim itm As New crm_CustomerDetails

        Try
            'lst = dl.getCompanies(token)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function saveNewCustomer(ByVal token As String, ByVal data As CRM_Customer) As CRM_Customer Implements Ietrest.saveNewCustomer
        Dim dl As New DataLayer

        Try
            data.id = dl.saveNewCustomer(token, data)
        Catch ex As Exception

        End Try

        Return data

    End Function

    Function saveCompanyNote(ByVal token As String, ByVal data As companyNote) As responseOk Implements Ietrest.saveCompanyNote
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveCompanyNote(token, data.companyId, data.note)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function GetCompanyNotes(ByVal token As String, ByVal custId As String) As List(Of companyNote) Implements Ietrest.GetCompanyNotes
        Dim lst As New List(Of companyNote)
        Dim dl As New DataLayer

        Try
            lst = dl.getCompanyNotes(token, custId)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Devices"

    Function searchDevice(ByVal token As String, ByVal searchKey As String, ByVal keyValue As String, ByVal noCache As String) As searchDeviceResult Implements Ietrest.searchDevice
        Dim dev As New searchDeviceResult
        Dim dl As New DataLayer

        Try
            dev = dl.searchDevice(token, searchKey, keyValue)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("crm_etapp.etrest.svc", "", "searchDevice", ex.Message, 0)
        End Try

        Return dev

    End Function

    Function deviceAction(ByVal token As String, ByVal data As devAction) As callResult Implements Ietrest.deviceAction
        Dim res As New callResult
        Dim dl As New DataLayer

        Try
            res = dl.deviceAction(token, data.action, data.id, data.param1, data.param2)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("crm_etapp.etrest.svc", "", "deviceAction", ex.Message, 0)
        End Try

        Return res

    End Function

    Function crm_getAllCompanies(ByVal token As String) As List(Of basicList) Implements Ietrest.crm_getAllCompanies
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.crm_getAllCompanies(token)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture("crm_etapp.etrest.svc", "", "crm_getAllCompanies", ex.Message, 0)
        End Try

        Return lst

    End Function

#End Region

#Region "Inventory"

    Public Function getInventory(ByVal token As String) As List(Of inventory) Implements Ietrest.getInventory
        Dim lst As New List(Of inventory)
        Dim dl As New DataLayer

        Try
            lst = dl.getInventory(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveNewInventory(ByVal token As String, ByVal data As newInventory) As newInventory Implements Ietrest.saveNewInventory
        Dim dl As New DataLayer

        Try
            data.isOk = dl.saveNewInventory(token, data, data.msg)
        Catch ex As Exception

        End Try

        Return data

    End Function

    Public Function saveAssignment(ByVal token As String, ByVal data As assignedInventory) As assignedInventory Implements Ietrest.saveAssignment
        Dim dl As New DataLayer
        Dim isOk As Boolean = False

        Try
            For Each dev In data.inventory
                isOk = dl.assignDevice(token, data.custId, data.orderNo, data.courrierId, data.trackingNumber, dev)
            Next

            'Send email
            If data.orderNo.Length > 0 Then
                dl.SendShipmentEmail(token, data.orderNo, data.emailTypeId, data.courrierId, data.trackingNumber)
            Else
                dl.sendEmail(token, data.custId, data.emailTypeId, data.courrierId, data.trackingNumber)
            End If

        Catch ex As Exception

        End Try

        Return data

    End Function

#End Region

#Region "Suspend/Reactivate Company"

    Public Function GetCompaniesSuspendedReasons(ByVal token As String) As List(Of basicList) Implements Ietrest.GetCompaniesSuspendedReasons
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.GetCompaniesSuspendedReasons()
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getSuspendCompanies(ByVal token As String) As List(Of Company) Implements Ietrest.getSuspendCompanies
        Dim lst As New List(Of Company)
        Dim dl As New DataLayer

        Try
            lst = dl.getCompaniesSuspendResume(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveSuspendCompany(ByVal token As String, ByVal data As Company) As Company Implements Ietrest.saveSuspendCompany
        Dim dl As New DataLayer

        Try
            dl.saveSuspendCompany(token, data)
        Catch ex As Exception

        End Try

        Return data

    End Function

#End Region

#Region "Geofences"

    Public Function getGeofencesCustomMessages(ByVal token As String, ByVal msgType As String) As List(Of basicList) Implements Ietrest.getGeofencesCustomMessages
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.getGeofencesCustomMessages(token, msgType)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveGeofence(ByVal data As geofenceClass) As responseOk Implements Ietrest.saveGeofence
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

                guid = dl.saveGeofence(data.token, data.id, data.geofenceTypeId, data.name, _
                                       data.contactName, data.phone, data.contactEmail, data.contactSMSAlert, data.contactEmailAlert, data.contactAlertTypeId, _
                                       data.fullAddress, data.street, data.streetNumber, _
                                       data.route, data.suite, data.city, data.county, data.state, data.postalCode, data.country, data.latitude, data.longitude, data.geofenceAlertTypeId, _
                                       data.radius, data.comments, data.shapeId, data.jsonPolyVerticesTXT, data.KMLData, data.SQLData, data.isSpeedLimit, data.speedLimit, _
                                       data.arrivalMsgId, data.arrivalMsgTxt, data.departureMsgId, data.departureMsgTxt, _
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

#End Region

#Region "Work Zones"

    Public Function GetWorkZones(ByVal token As String) As List(Of basicList) Implements Ietrest.GetWorkZones
        Dim lst As New List(Of basicList)
        Dim dl As New DataLayer

        Try
            lst = dl.GetWorkZones(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Technicians"

    Public Function GetTechnicians(ByVal token As String, ByVal WorkZoneID As String) As List(Of technician) Implements Ietrest.GetTechnicians
        Dim lst As New List(Of technician)
        Dim dl As New DataLayer

        Try
            lst = dl.GetTechnicians(token, WorkZoneID)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Jobs Status"

    Public Function GetJobStatus(ByVal token As String, ByVal noCache As String) As List(Of jobStatus) Implements Ietrest.GetJobStatus
        Dim lst As New List(Of jobStatus)
        Dim dl As New DataLayer

        Try
            lst = dl.GetJobStatus(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Jobs"

    Public Function GetJobs(ByVal token As String, ByVal noCache As String, ByVal statId As String, ByVal wzId As String, ByVal techId As String, ByVal jobNo As String, ByVal custName As String) As List(Of job) Implements Ietrest.GetJobs
        Dim lst As New List(Of job)
        Dim dl As New DataLayer

        Try
            lst = dl.GetJobs(token, statId, wzId, techId, jobNo, custName)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function GetJob(ByVal token As String, ByVal noCache As String, ByVal jobId As String) As job Implements Ietrest.GetJob
        Dim dl As New DataLayer
        Dim itm As New job

        Try
            If Not (jobId = "0") Then
                itm = dl.GetJob(token, jobId)
            End If
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function GetJobSupportTables(ByVal token As String, ByVal noCache As String) As jobSupportTables Implements Ietrest.GetJobSupportTables
        Dim dl As New DataLayer
        Dim itm As New jobSupportTables

        Try
            itm = dl.GetJobSupportTables(token)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function saveJob(ByVal token As String, ByVal data As job) As responseOk Implements Ietrest.saveJob
        Dim res As New responseOk
        Dim dl As New DataLayer
        Dim jobGUID As String = ""
        Dim jobNumber As String = ""

        Try
            res.isOk = dl.saveJob(token, data, jobGUID, jobNumber)

        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "Customers / Geofences"

    Public Function CustomerSearch(ByVal token As String, ByVal noCache As String, ByVal custName As String) As List(Of customerSearch) Implements Ietrest.CustomerSearch
        Dim dl As New DataLayer
        Dim lst As New List(Of customerSearch)

        Try
            lst = dl.CustomerSearch(token, custName)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Telemetry"

    Public Function GetIOs(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devIOs Implements Ietrest.GetIOs
        Dim itm As New devIOs
        Dim dl As New DataLayer

        Try
            itm = dl.GetIOs(token, deviceId)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function GetAllDevIOs(ByVal token As String, ByVal noCache As String) As List(Of devIOs) Implements Ietrest.GetAllDevIOs
        Dim lst As New List(Of devIOs)
        Dim dl As New DataLayer

        Try
            lst = dl.GetAllDevIOs(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function setOutput(ByVal token As String, ByVal data As setRelay) As responseOk Implements Ietrest.setOutput
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.setOutput(token, data, res.msg)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

    Public Function telemetrySetUp(ByVal token As String, ByVal data As ioSetUp) As responseOk Implements Ietrest.telemetrySetUp
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

    Public Function GetDevMeters(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As devInputsOnTime Implements Ietrest.GetDevMeters
        Dim itm As New devInputsOnTime
        Dim dl As New DataLayer

        Try
            itm = dl.GetDevMeters(token, deviceId)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function GetAllDevMeters(ByVal token As String, ByVal noCache As String) As List(Of devInputsOnTime) Implements Ietrest.GetAllDevMeters
        Dim lst As New List(Of devInputsOnTime)
        Dim dl As New DataLayer

        Try
            lst = dl.GetAllDevMeters(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveHourMeter(ByVal token As String, ByVal deviceId As String, ByVal data As devInputsOnTime) As responseOk Implements Ietrest.saveHourMeter
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

    Public Function getAllGeofenceTypes(ByVal token As String, ByVal noCache As String) As List(Of geofenceType) Implements Ietrest.getAllGeofenceTypes
        Dim lst As New List(Of geofenceType)
        Dim dl As New DataLayer

        Try
            lst = dl.getAllGeofenceTypes(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveGeofenceType(ByVal token As String, ByVal noCache As String, ByVal data As geofenceType) As responseOk Implements Ietrest.saveGeofenceType
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.saveGeofenceType(token, data, res.msg)
        Catch ex As Exception
            res.isOk = False
        End Try

        Return res

    End Function

    Public Function deleteGeofenceType(ByVal token As String, ByVal id As String) As responseOk Implements Ietrest.deleteGeofenceType
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

    Public Function maintSupportListsGET(ByVal token As String, ByVal noCache As String) As maintSupportLists Implements Ietrest.maintSupportListsGET
        Dim itm As New maintSupportLists
        Dim dl As New DataLayer

        Try
            itm = dl.maintSupportListsGET(token)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function maintDeviceList(ByVal token As String, ByVal noCache As String) As List(Of maintDevice) Implements Ietrest.maintDeviceList
        Dim lst As New List(Of maintDevice)
        Dim dl As New DataLayer

        Try
            lst = dl.maintDeviceList(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function maintDeviceSave(ByVal token As String, ByVal deviceId As String, ByVal data As maintDevice) As maintDevice Implements Ietrest.maintDeviceSave
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

    Public Function maintScheduleGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceSchedule Implements Ietrest.maintScheduleGetByDevice
        Dim lst As New maintDeviceSchedule
        Dim dl As New DataLayer

        Try
            lst = dl.maintScheduleGetByDevice(token, deviceId, "")
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function maintItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem Implements Ietrest.maintItemSave
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

    Public Function maintItemGet(ByVal token As String, ByVal noCache As String, ByVal deviceId As String, ByVal id As String) As scheduleItem Implements Ietrest.maintItemGet
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

    Public Function maintItemDelete(ByVal token As String, ByVal deviceId As String, ByVal id As String) As responseOk Implements Ietrest.maintItemDelete
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

    Public Function maintCompletedItemSave(ByVal token As String, ByVal deviceId As String, ByVal id As String, ByVal data As scheduleItem) As scheduleItem Implements Ietrest.maintCompletedItemSave
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

    Public Function maintLogGetByDevice(ByVal token As String, ByVal noCache As String, ByVal deviceId As String) As maintDeviceLog Implements Ietrest.maintLogGetByDevice
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

    Public Function getQBMatchCustomers(ByVal token As String) As qbMatch Implements Ietrest.getQBMatchCustomers
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

    Public Function qbLinkCustomers(ByVal token As String, ByVal crmId As String, ByVal qbId As String) As responseOk Implements Ietrest.qbLinkCustomers
        Dim itm As New responseOk
        Dim dl As New DataLayer

        Try
            itm.isOk = dl.qbLinkCustomers(token, crmId, qbId)
        Catch ex As Exception
            itm.isOk = False
        End Try

        Return itm

    End Function

    Public Function qbUnLinkCustomers(ByVal token As String, ByVal crmId As String) As responseOk Implements Ietrest.qbUnLinkCustomers
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

    Function updateUserPref(ByVal token As String, ByVal data As userPreference) As responseOk Implements crm_etapp.Ietrest.updateUserPref
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

#End Region

#Region "CRM Related"

    Public Function EmailTypesGET(ByVal token As String, ByVal type As String) As List(Of basicList) Implements Ietrest.EmailTypesGET
        Dim dl As New DataLayer
        Dim lst As New List(Of basicList)

        Try
            lst = dl.EmailTypesGET(token, type)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function GenericMastersGET(ByVal token As String, ByVal masterKey As String) As List(Of basicList) Implements Ietrest.GenericMastersGET
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

    Function crmGetInvoices(ByVal token As String, ByVal custId As String, ByVal noCache As String) As List(Of invoice) Implements Ietrest.crmGetInvoices
        Dim lst As New List(Of invoice)
        Dim dl As New DataLayer

        Try
            lst = dl.crmGetInvoices(token, custId)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Quick Message"

    Public Function getQuickMsgDriversList(ByVal token As String, ByVal noCache As String) As List(Of quickMsgDriver) Implements Ietrest.getQuickMsgDriversList
        Dim lst As New List(Of quickMsgDriver)
        Dim itm As quickMsgDriver = Nothing
        Dim dl As New DataLayer
        Dim strError As String = ""
        Dim dt As New DataTable
        Dim drv As DataRowView

        Try
            dt = dl.getQuickMsgDriversList(token, strError)
            For Each drv In dt.DefaultView
                itm = New quickMsgDriver
                itm.id = drv.Item("ID")
                itm.name = drv.Item("Name")
                itm.phone = drv.Item("Phone")
                itm.email = drv.Item("Email")
                itm.deviceId = drv.Item("DeviceID")
                itm.deviceName = drv.Item("DeviceName")
                lst.Add(itm)
            Next
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function sendQuickMsg(ByVal token As String, ByVal data As quickMsg) As responseOk Implements Ietrest.sendQuickMsg
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.sendQuickMsg(token, data.driverId, data.channel, data.message, res.msg)
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "CRM - Confirm Shipment"

    Public Function confirmShipment(ByVal token As String, ByVal data As confirmShipment) As responseOk Implements Ietrest.confirmShipment
        Dim r As New responseOk
        Dim dl As New DataLayer

        Try
            r = dl.confirmShipment(token, data.orderNo, data.courrierId, data.trackingNumber)

            'Send email
            If r.isOk = True Then
                dl.SendShipmentEmail(token, data.orderNo, data.emailTypeId, data.courrierId, data.trackingNumber)
            End If

        Catch ex As Exception

        End Try

        Return r

    End Function

#End Region

#Region "IfByPhone"

    Function smsReplyCatcher(ByVal msg As String) As Boolean Implements Ietrest.smsReplyCatcher
        Dim res As Boolean = True
        Dim dl As New DataLayer

        Try
            res = dl.smsReplyCatcher(msg)
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "Google Signature"

    Function getGoogleSignature(ByVal data As googURL) As googSig Implements Ietrest.getGoogleSignature
        Dim itm As New googSig
        Dim dl As New DataLayer
        Dim mapUrl As String 

        Try
            mapUrl = HttpUtility.UrlDecode(data._url)
            itm.sig = dl.GoogleSignature(mapUrl)
        Catch ex As Exception

        End Try

        Return itm

    End Function

#End Region

#Region "Engagement"

    Function pageEngagement(ByVal data As engagementTick) As responseOk Implements Ietrest.pageEngagement
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try

            res = dl.pageEngagement(data)

        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "Reports"

    Function getTroubleshootingReport(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal noCache As String) As List(Of troubleLog) Implements Ietrest.getTroubleshootingReport
        Dim lst As New List(Of troubleLog)
        Dim dl As New DataLayer

        Try
            lst = dl.getTroubleshootingReport(token, deviceId, dateFrom, dateTo)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

End Class

