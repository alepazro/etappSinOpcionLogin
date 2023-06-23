Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation.WebScriptServiceHostFactory
Imports System.Data
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json
Imports System.Data.SqlClient

' NOTE: You can use the "Rename" command on the context menu to change the class name "etrest" in code, svc and config file together.
Public Class etrest
    Implements Ietrest

    Dim bl As New BLCommon

#Region "API"

#Region "Get company users"

    Public Function GetCompanyUsers(ByVal token As String, ByVal sourceId As String) As List(Of user2) Implements Ietrest.GetCompanyUsers
        Dim uList As New List(Of user2)
        Dim u As user2 = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dtData = dl.GetCompanyUsers(token, sourceId)
            If Not IsNothing(dtData) Then
                dvData = dtData.DefaultView
                For Each drv In dvData
                    u = New user2
                    u.firstName = drv.item("FirstName")
                    u.lastName = drv.item("LastName")
                    u.uniqueId = drv.item("UniqueID")
                    uList.Add(u)
                Next
            End If

        Catch ex As Exception

        End Try

        Return uList

    End Function


#End Region

#Region "Mobile Devices"

    Public Function Authorization(ByVal login As String, ByVal password As String, ByVal expDays As String) As user Implements Ietrest.authorization
        Dim u As New user
        Dim dl As New DataLayer
        Dim dvData As DataView = Nothing
        Dim drv As DataRowView
        Dim msg As String = ""
        Dim intExpDays As Integer = 1

        Try
            If Not IsNumeric(expDays) Then
                expDays = "1"
            End If
            intExpDays = CInt(expDays)
            If intExpDays < 1 Then
                intExpDays = 1
            End If
            If intExpDays > 15 Then
                intExpDays = 15
            End If

            'Initialize values...
            u.login = login
            u.token = "INVALIDCREDENTIALS"
            u.expDays = intExpDays

            dvData = dl.ValidateCredentials(login, password, intExpDays, "1", "", 0, 0, msg)
            If msg.Length = 0 Then
                If Not IsNothing(dvData) Then
                    If dvData.Count > 0 Then
                        drv = dvData.Item(0)

                        u.login = login
                        u.firstName = drv.Item("firstName")
                        u.lastName = drv.Item("lastName")
                        u.token = drv.Item("token")
                        u.expDays = intExpDays

                    End If
                End If
            End If

        Catch ex As Exception
            u.login = login
            u.token = "INVALIDCREDENTIALS"
            u.expDays = intExpDays
        Finally

        End Try

        Return u

    End Function

    Public Function Authorization2(ByVal login As String, ByVal password As String, ByVal expDays As String, ByVal sourceId As String, ByVal sourceExt As String) As user Implements Ietrest.authorization2
        Dim u As New user
        Dim dl As New DataLayer
        Dim dvData As DataView = Nothing
        Dim drv As DataRowView
        Dim msg As String = ""
        Dim intExpDays As Integer = 1

        Try
            If Not IsNumeric(expDays) Then
                expDays = "1"
            End If
            intExpDays = CInt(expDays)
            If intExpDays < 1 Then
                intExpDays = 1
            End If
            If intExpDays > 15 Then
                intExpDays = 15
            End If

            'Initialize values...
            u.login = login
            u.token = "INVALIDCREDENTIALS"
            u.expDays = intExpDays

            dvData = dl.ValidateCredentials(login, password, intExpDays, sourceId, sourceExt, 0, 0, msg)
            If msg.Length = 0 Then
                If Not IsNothing(dvData) Then
                    If dvData.Count > 0 Then
                        drv = dvData.Item(0)

                        u.login = login
                        u.firstName = drv.Item("firstName")
                        u.lastName = drv.Item("lastName")
                        u.token = drv.Item("token")
                        u.expDays = intExpDays

                    End If
                End If
            End If

        Catch ex As Exception
            u.login = login
            u.token = "INVALIDCREDENTIALS"
            u.expDays = intExpDays
        Finally

        End Try

        Return u

    End Function

    Public Function GetDevicesList(ByVal token As String) As List(Of FleetDeviceVideo) Implements Ietrest.GetDevicesList
        Dim devList As New List(Of FleetDeviceVideo)
        Dim dev As FleetDeviceVideo = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dsData = dl.getDevices(token, False, "", msg)
            If Not IsNothing(dsData) Then
                If dsData.Tables.Count >= 2 Then
                    dvData = dsData.Tables(1).DefaultView
                    For Each drv In dvData
                        dev = New FleetDeviceVideo
                        dev.id = drv.item("GUID")
                        dev.name = drv.item("Name")
                        dev.eventCode = drv.item("EventCode")
                        dev.eventName = drv.item("EventName")
                        dev.eventDate = drv.item("EventDate").ToString
                        dev.address = drv.item("FullAddress")
                        dev.speed = drv.item("Speed")
                        dev.latitude = drv.item("Latitude")
                        dev.longitude = drv.item("Longitude")
                        If drv.item("DriverID") = 0 Then
                            dev.driverName = "N/A"
                            dev.driverPhone = ""
                        Else
                            dev.driverName = drv.item("DriverName")
                            dev.driverPhone = drv.item("DriverPhone")
                        End If
                        dev.result = "OK"
                        devList.Add(dev)
                    Next
                End If
            End If

            If devList.Count = 0 Then
                dev = New FleetDeviceVideo
                dev.result = "INVALIDTOKEN"
                devList.Add(dev)
            End If

        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetDevicesListCORS(ByVal token As String, ByVal callback As String) As List(Of FleetDeviceVideo) Implements Ietrest.GetDevicesListCORS
        Dim devList As New List(Of FleetDeviceVideo)
        Dim dev As FleetDeviceVideo = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dsData = dl.getDevices(token, False, "", msg)
            If Not IsNothing(dsData) Then
                If dsData.Tables.Count >= 2 Then
                    dvData = dsData.Tables(1).DefaultView
                    For Each drv In dvData
                        dev = New FleetDeviceVideo
                        dev.id = drv.item("GUID")
                        dev.name = drv.item("Name")
                        dev.eventCode = drv.item("EventCode")
                        dev.eventName = drv.item("EventName")
                        dev.eventDate = drv.item("EventDate").ToString
                        dev.address = drv.item("FullAddress")
                        dev.speed = drv.item("Speed")
                        dev.latitude = drv.item("Latitude")
                        dev.longitude = drv.item("Longitude")
                        If drv.item("DriverID") = 0 Then
                            dev.driverName = "N/A"
                            dev.driverPhone = ""
                        Else
                            dev.driverName = drv.item("DriverName")
                            dev.driverPhone = drv.item("DriverPhone")
                        End If
                        dev.result = "OK"
                        devList.Add(dev)
                    Next
                End If
            End If

            If devList.Count = 0 Then
                dev = New FleetDeviceVideo
                dev.result = "INVALIDTOKEN"
                devList.Add(dev)
            End If

        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetDevicesList2(ByVal token As String, ByVal sourceId As String) As List(Of FleetDeviceVideo) Implements Ietrest.GetDevicesList2
        Dim devList As New List(Of FleetDeviceVideo)
        Dim dev As FleetDeviceVideo = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dsData = dl.getDevices(token, False, "", msg)
            If Not IsNothing(dsData) Then
                If dsData.Tables.Count >= 2 Then
                    dvData = dsData.Tables(1).DefaultView
                    For Each drv In dvData
                        dev = New FleetDeviceVideo
                        dev.id = drv.item("GUID")
                        dev.name = drv.item("Name")
                        dev.shortName = drv.item("ShortName")
                        dev.eventCode = drv.item("EventCode")
                        dev.eventName = drv.item("EventName")
                        dev.eventDate = drv.item("EventDate").ToString
                        dev.heading = drv.item("Heading")
                        dev.address = drv.item("FullAddress")
                        dev.speed = drv.item("Speed")
                        dev.latitude = drv.item("Latitude")
                        dev.longitude = drv.item("Longitude")
                        If drv.item("DriverID") = 0 Then
                            dev.driverName = "N/A"
                            dev.driverPhone = ""
                        Else
                            dev.driverName = drv.item("DriverName")
                            dev.driverPhone = drv.item("DriverPhone")
                        End If
                        dev.result = "OK"
                        devList.Add(dev)
                    Next
                End If
            End If

            If devList.Count = 0 Then
                dev = New FleetDeviceVideo
                dev.result = "INVALIDTOKEN"
                devList.Add(dev)
            End If

        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetDeviceInfo(ByVal id As String) As FleetDeviceVideo Implements Ietrest.GetDeviceInfo
        Dim dev As FleetDeviceVideo = Nothing
        Dim msg As String = ""
        Dim dvData As DataView = Nothing
        Dim dl As New DataLayer

        Try
            dvData = dl.getDeviceByGUID(id, msg)

            dev = New FleetDeviceVideo
            dev.result = "INVALIDTOKEN"

            If Not IsNothing(dvData) Then
                For Each drv In dvData
                    dev.id = drv.item("GUID")
                    dev.name = drv.item("Name")
                    dev.eventCode = drv.item("EventCode")
                    dev.eventName = drv.item("EventName")
                    dev.eventDate = drv.item("EventDate").ToString
                    dev.address = drv.item("FullAddress")
                    dev.speed = drv.item("Speed")
                    dev.latitude = drv.item("Latitude")
                    dev.longitude = drv.item("Longitude")
                    If drv.item("DriverID") = 0 Then
                        dev.driverName = "N/A"
                        dev.driverPhone = ""
                    Else
                        dev.driverName = drv.item("DriverName")
                        dev.driverPhone = drv.item("DriverPhone")
                    End If
                    dev.result = "OK"
                Next
            End If

        Catch ex As Exception

        End Try

        Return dev

    End Function

    Public Function GetDeviceInfo2(ByVal id As String, ByVal sourceId As String) As FleetDeviceVideo Implements Ietrest.GetDeviceInfo2
        Dim dev As FleetDeviceVideo = Nothing
        Dim msg As String = ""
        Dim dvData As DataView = Nothing
        Dim dl As New DataLayer

        Try
            dvData = dl.getDeviceByGUID(id, msg)

            dev = New FleetDeviceVideo
            dev.result = "INVALIDTOKEN"

            If Not IsNothing(dvData) Then
                For Each drv In dvData
                    dev.id = drv.item("GUID")
                    dev.name = drv.item("Name")
                    dev.eventCode = drv.item("EventCode")
                    dev.eventName = drv.item("EventName")
                    dev.eventDate = drv.item("EventDate").ToString
                    dev.address = drv.item("FullAddress")
                    dev.speed = drv.item("Speed")
                    dev.latitude = drv.item("Latitude")
                    dev.longitude = drv.item("Longitude")
                    If drv.item("DriverID") = 0 Then
                        dev.driverName = "N/A"
                        dev.driverPhone = ""
                    Else
                        dev.driverName = drv.item("DriverName")
                        dev.driverPhone = drv.item("DriverPhone")
                    End If
                    dev.result = "OK"
                Next
            End If

        Catch ex As Exception

        End Try

        Return dev

    End Function

    Public Function GetTrail(ByVal id As String, ByVal trailDate As String) As List(Of Trail) Implements Ietrest.GetTrail
        Dim trailList As New List(Of Trail)
        Dim trail As Trail = Nothing
        Dim dl As New DataLayer
        Dim dvData As DataView
        Dim msg As String = ""
        Dim drv As DataRowView
        Dim dateFrom As DateTime
        Dim dateTo As DateTime

        Try
            If IsDate(trailDate) Then
                dateFrom = CDate(trailDate)
            Else
                dateFrom = Now.Date
            End If
            dateTo = DateAdd(DateInterval.Day, 1, dateFrom)

            dvData = dl.GetTrail(id, dateFrom, dateTo, msg)
            If msg.Length = 0 Then
                If Not IsNothing(dvData) Then
                    If dvData.Count > 0 Then
                        For Each drv In dvData
                            trail = New Trail
                            trail.evCode = drv.Item("EventCode")
                            trail.evName = drv.Item("EventName")
                            trail.evDate = drv.Item("EventDate").ToString
                            If drv.Item("GeofenceID") > 0 Then
                                trail.loc = drv.Item("GeofenceName")
                            Else
                                trail.loc = drv.Item("FullAddress")
                            End If
                            trail.speed = drv.Item("Speed")
                            trail.lat = drv.Item("Latitude")
                            trail.lng = drv.Item("Longitude")
                            trail.res = "OK"
                            trailList.Add(trail)
                        Next
                    End If
                End If
            End If

            If trailList.Count = 0 Then
                trail = New Trail
                trail.res = "NODATAFOUND"
                trailList.Add(trail)
            End If

        Catch ex As Exception

        Finally

        End Try

        Return trailList

    End Function

    Public Function GetTrail2(ByVal id As String, ByVal trailDate As String, ByVal sourceId As String) As List(Of Trail) Implements Ietrest.GetTrail2
        Dim trailList As New List(Of Trail)
        Dim trail As Trail = Nothing
        Dim dl As New DataLayer
        Dim dvData As DataView
        Dim msg As String = ""
        Dim drv As DataRowView
        Dim dateFrom As DateTime
        Dim dateTo As DateTime

        Try
            If IsDate(trailDate) Then
                dateFrom = CDate(trailDate)
            Else
                dateFrom = Now.Date
            End If
            dateTo = DateAdd(DateInterval.Day, 1, dateFrom)

            dvData = dl.GetTrail(id, dateFrom, dateTo, msg)
            If msg.Length = 0 Then
                If Not IsNothing(dvData) Then
                    If dvData.Count > 0 Then
                        For Each drv In dvData
                            trail = New Trail
                            trail.evCode = drv.Item("EventCode")
                            trail.evName = drv.Item("EventName")
                            trail.evDate = drv.Item("EventDate").ToString
                            trail.evHeading = drv.Item("Heading")
                            If drv.Item("GeofenceID") > 0 Then
                                trail.loc = drv.Item("GeofenceName")
                            Else
                                trail.loc = drv.Item("FullAddress")
                            End If
                            trail.speed = drv.Item("Speed")
                            trail.lat = drv.Item("Latitude")
                            trail.lng = drv.Item("Longitude")
                            trail.res = "OK"
                            trailList.Add(trail)
                        Next
                    End If
                End If
            End If

            If trailList.Count = 0 Then
                trail = New Trail
                trail.res = "NODATAFOUND"
                trailList.Add(trail)
            End If

        Catch ex As Exception

        Finally

        End Try

        Return trailList

    End Function

    Public Function GetTrail3(ByVal id As String, ByVal trailDate As String, ByVal hourFrom As String, ByVal hourTo As String, ByVal sourceId As String) As List(Of Trail) Implements Ietrest.GetTrail3
        Dim trailList As New List(Of Trail)
        Dim trail As Trail = Nothing
        Dim dl As New DataLayer
        Dim dvData As DataView
        Dim msg As String = ""
        Dim drv As DataRowView
        Dim dateFrom As DateTime
        Dim dateTo As DateTime
        Dim intHourFrom As Integer
        Dim intHourTo As Integer

        Try
            If Not IsNumeric(hourFrom) Then
                intHourFrom = 0
            Else
                intHourFrom = CInt(hourFrom)
            End If
            If Not IsNumeric(hourTo) Then
                intHourTo = 23
            Else
                intHourTo = CInt(hourTo)
            End If
            If intHourTo < intHourFrom Then
                intHourFrom = intHourTo
            End If

            If IsDate(trailDate) Then
                dateFrom = CDate(trailDate)
            Else
                dateFrom = Now.Date
            End If
            dateTo = DateAdd(DateInterval.Hour, intHourTo, dateFrom)
            dateFrom = DateAdd(DateInterval.Hour, intHourFrom, dateFrom)

            dvData = dl.GetTrail(id, dateFrom, dateTo, msg)
            If msg.Length = 0 Then
                If Not IsNothing(dvData) Then
                    If dvData.Count > 0 Then
                        For Each drv In dvData
                            trail = New Trail
                            trail.evCode = drv.Item("EventCode")
                            trail.evName = drv.Item("EventName")
                            trail.evDate = drv.Item("EventDate").ToString
                            trail.evHeading = drv.Item("Heading")
                            If drv.Item("GeofenceID") > 0 Then
                                trail.loc = drv.Item("GeofenceName")
                            Else
                                trail.loc = drv.Item("FullAddress")
                            End If
                            trail.speed = drv.Item("Speed")
                            trail.lat = drv.Item("Latitude")
                            trail.lng = drv.Item("Longitude")
                            trail.res = "OK"
                            trailList.Add(trail)
                        Next
                    End If
                End If
            End If

            If trailList.Count = 0 Then
                trail = New Trail
                trail.res = "NODATAFOUND"
                trailList.Add(trail)
            End If

        Catch ex As Exception

        Finally

        End Try

        Return trailList

    End Function

#End Region

#Region "Embedded map add-on"

    Public Function easiTrackMap(ByVal token As String, ByVal callback As String) As List(Of FleetDeviceVideo) Implements Ietrest.easiTrackMap
        Dim devices As New List(Of FleetDeviceVideo)
        Dim dev As FleetDeviceVideo
        Dim dsData As DataSet
        Dim dvData As DataView
        Dim msg As String = ""
        Dim dl As New DataLayer

        Dim strScript As String = ""

        Try
            dsData = dl.getDevices(token, True, "", msg)
            If Not IsNothing(dsData) Then
                If dsData.Tables.Count >= 2 Then
                    dvData = dsData.Tables(1).DefaultView
                    For Each drv In dvData
                        dev = New FleetDeviceVideo
                        dev.id = drv.item("GUID")
                        dev.name = drv.item("Name")
                        'dev.eventCode = drv.item("EventCode")
                        'dev.eventName = drv.item("EventName")
                        'dev.eventDate = drv.item("EventDate").ToString
                        'dev.address = drv.item("FullAddress")
                        'dev.speed = drv.item("Speed")
                        dev.latitude = drv.item("Latitude")
                        dev.longitude = drv.item("Longitude")
                        'dev.result = "OK"
                        devices.Add(dev)
                    Next
                End If
            End If

            'Dim result As New jsonpResult(devices)

            'Return result

            'Build the response
            'Response.ClearHeaders()
            'Response.AppendHeader("Pragma", "no-cache")
            'Response.AppendHeader("Expires", "Mon, 26 Jul 1997 05:00:00 GMT")
            'Response.AppendHeader("Last-Modified", Date.UtcNow)
            'Response.AppendHeader("P3P", "CP=\""IDC DSP COR CURa ADMa OUR IND PHY ONL COM STA\""")
            'HttpContext.Current.Response.ContentType = "application/x-javascript"

            'JavaScriptSerializer serializer = new JavaScriptSerializer();
            'string ser = serializer.Serialize(Data);
            Dim serializer As New JavaScriptSerializer
            Dim ser = serializer.Serialize(devices)

            strScript = callback & "(" & ser & ");"
            'HttpContext.Current.Response.Write(strScript)
            'HttpContext.Current.Response.End()

        Catch ex As Exception

        End Try

        Return devices

    End Function

#End Region

#Region "eTrack API - Devices"

    Public Function GetDevices(ByVal token As String) As List(Of device2) Implements Ietrest.GetDevices
        Dim devList As New List(Of device2)
        Dim dev As device2 = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                dev = New device2
                dev.apiToken = token
                dev.isOk = False
                dev.msg = "INVALIDTOKEN"
                devList.Add(dev)

            Else
                dsData = dl.getDevices(token, True, "", msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dsData) Then
                        If dsData.Tables.Count >= 2 Then
                            dvData = dsData.Tables(1).DefaultView
                            For Each drv In dvData
                                dev = New device2
                                dev.apiToken = token
                                dev.vehicleId = drv.item("GUID")
                                dev.name = drv.item("Name")
                                dev.eventTypeId = drv.item("EventCode")
                                dev.lastUpdatedOn = drv.item("LastUpdatedOn").ToString
                                dev.eventDate = drv.item("EventDate").ToString
                                dev.speed = drv.item("Speed")
                                dev.latitude = drv.item("Latitude")
                                dev.longitude = drv.item("Longitude")
                                dev.isOk = True
                                dev.msg = ""
                                devList.Add(dev)
                            Next
                        End If
                    End If

                    If devList.Count = 0 Then
                        dev = New device2
                        dev.apiToken = token
                        dev.isOk = False
                        dev.msg = "NODATAFOUND"
                        devList.Add(dev)
                    End If
                Else
                    dev = New device2
                    dev.apiToken = token
                    dev.isOk = False
                    dev.msg = msg
                    devList.Add(dev)
                End If
            End If

        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetDevice(ByVal token As String, ByVal deviceGUID As String) As device2 Implements Ietrest.GetDevice
        Dim dev As device2 = Nothing
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dev = New device2
            dev.isOk = True

            If Not BLCommon.IsGUID(token) Then
                dev.apiToken = token
                dev.isOk = False
                dev.msg = "INVALIDTOKEN"

            ElseIf Not BLCommon.IsGUID(deviceGUID) Then
                If deviceGUID.Length > 0 And deviceGUID.Length < 15 Then
                    deviceGUID = BLCommon.Sanitize(deviceGUID)
                Else
                    dev.apiToken = token
                    dev.isOk = False
                    dev.msg = "INVALIDVEHICLEID"
                End If
            End If

            If dev.isOk = True Then
                dsData = dl.getDevices(token, True, deviceGUID, msg)
                If msg.Length = 0 Then
                    dev.apiToken = token
                    dev.isOk = False
                    dev.msg = "NODATAFOUND"

                    If Not IsNothing(dsData) Then
                        If dsData.Tables.Count >= 2 Then
                            dvData = dsData.Tables(1).DefaultView
                            For Each drv In dvData
                                dev.apiToken = token
                                dev.vehicleId = drv.item("GUID")
                                dev.name = drv.item("Name")
                                dev.eventTypeId = drv.item("EventCode")
                                dev.lastUpdatedOn = drv.item("LastUpdatedOn").ToString
                                dev.eventDate = drv.item("EventDate").ToString
                                dev.speed = drv.item("Speed")
                                dev.latitude = drv.item("Latitude")
                                dev.longitude = drv.item("Longitude")
                                dev.isOk = True
                                dev.msg = ""
                            Next
                        End If
                    End If
                Else
                    dev.apiToken = token
                    dev.isOk = False
                    dev.msg = msg
                End If
            End If

        Catch ex As Exception

        End Try

        Return dev

    End Function

    Public Function GetHistory(ByVal token As String, ByVal deviceGUID As String, ByVal dateFrom As Date, ByVal dateTo As Date) As List(Of History) Implements Ietrest.GetHistory
        Dim historyList As New List(Of History)
        Dim hist As History = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            hist = New History
            hist.isOk = True

            If Not BLCommon.IsGUID(token) Then
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDTOKEN"
                historyList.Add(hist)

            ElseIf Not BLCommon.IsGUID(deviceGUID) Then
                If deviceGUID.Length > 0 And deviceGUID.Length < 15 Then
                    deviceGUID = BLCommon.Sanitize(deviceGUID)
                Else
                    hist.apiToken = token
                    hist.isOk = False
                    hist.msg = "INVALIDVEHICLEID"
                    historyList.Add(hist)
                End If

            ElseIf Not IsDate(dateFrom) Then
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDDATEFROM"
                historyList.Add(hist)

            ElseIf Not IsDate(dateTo) Then
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDDATETO"
                historyList.Add(hist)

            ElseIf dateFrom > dateTo Then
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDTIMEFRAME"
                historyList.Add(hist)

            ElseIf DateDiff(DateInterval.Hour, dateFrom, dateTo) > 24 Then
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "TIMEFRAMEOVER24HOURS"
                historyList.Add(hist)
            End If

            If hist.isOk = True Then
                dtData = dl.getDeviceHistory(token, True, deviceGUID, dateFrom, dateTo, msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            hist = New History
                            hist.apiToken = token
                            hist.isOk = True
                            hist.msg = ""

                            hist.vehicleId = drv.item("GUID")
                            hist.eventTypeId = drv.item("EventCode")
                            hist.eventDate = drv.item("EventDate").ToString
                            hist.latitude = drv.item("Latitude")
                            hist.longitude = drv.item("Longitude")
                            hist.heading = drv.item("Heading")
                            hist.speed = drv.item("Speed")
                            hist.geofenceId = drv.item("GeofenceGUID")

                            historyList.Add(hist)
                        Next
                    End If

                    If historyList.Count = 0 Then
                        hist.apiToken = token
                        hist.isOk = False
                        hist.msg = "NODATAFOUND"
                        historyList.Add(hist)
                    End If
                Else
                    hist.apiToken = token
                    hist.isOk = False
                    hist.msg = msg
                    historyList.Add(hist)
                End If
            End If

        Catch ex As Exception

        End Try

        Return historyList

    End Function

    Public Function getHistory2(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String) As List(Of class_DeviceHistory) Implements Ietrest.getHistory2
        Dim lst As New List(Of class_DeviceHistory)
        Dim dl As New DataLayer
        Dim strError As String = ""
        Dim isOk As Boolean = True

        Try
            If Not IsDate(dateFrom) Or Not IsDate(dateTo) Then
                strError = "BAD_DATES"
                isOk = False
            End If

            If isOk = True Then
                lst = dl.getDeviceHistory2(token, deviceId, dateFrom, dateTo)
            End If
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getNathan(ByVal token As String, ByVal deviceId As String, ByVal pointer As String) As List(Of class_DeviceHistory) Implements Ietrest.getNathan
        Dim lst As New List(Of class_DeviceHistory)
        Dim dl As New DataLayer
        Dim strError As String = ""
        Dim isOk As Boolean = True
        Dim lPointer As Long = 0

        Try
            If IsNumeric(pointer) Then
                lPointer = CType(pointer, Long)
            End If

            If isOk = True Then
                lst = dl.getNathan(token, deviceId, lPointer)
            End If
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "eTrack API - Geofences"

    Public Function GetGeofences(ByVal token As String) As List(Of Geofence) Implements Ietrest.GetGeofences
        Dim geoList As New List(Of Geofence)
        Dim geo As Geofence = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                geo = New Geofence
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDTOKEN"
                geoList.Add(geo)

            Else
                dtData = dl.getGeofences(token, True, "", msg)
                If msg.Length = 0 Then
                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            geo = New Geofence
                            geo.apiToken = token
                            geo.geofenceId = drv.item("GUID")
                            geo.name = drv.item("Name")
                            geo.latitude = drv.item("Latitude")
                            geo.longitude = drv.item("Longitude")
                            geo.radius = drv.item("RadiusFeet")
                            geo.address = drv.item("FullAddress")
                            geo.alertStatusId = drv.item("GeofenceAlertTypeID")
                            geo.speedLimit = drv.item("SpeedLimit")
                            geo.speedLimitEnabled = drv.item("IsSpeedLimit")
                            geo.geofenceTypeId = drv.item("GeofenceTypeGUID")
                            geo.isOk = True
                            geo.msg = ""
                            geoList.Add(geo)
                        Next
                    End If

                    If geoList.Count = 0 Then
                        geo = New Geofence
                        geo.apiToken = token
                        geo.isOk = False
                        geo.msg = "NODATAFOUND"
                        geoList.Add(geo)
                    End If
                Else
                    geo = New Geofence
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = msg
                    geoList.Add(geo)
                End If
            End If

        Catch ex As Exception

        End Try

        Return geoList

    End Function

    Public Function GetGeofence(ByVal token As String, ByVal geofenceGUID As String) As Geofence Implements Ietrest.GetGeofence
        Dim geo As Geofence = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                geo = New Geofence
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDTOKEN"

            ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                geo = New Geofence
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDGEOFENCEID"

            Else

                dtData = dl.getGeofences(token, True, geofenceGUID, msg)
                If msg.Length = 0 Then
                    geo = New Geofence
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = "NODATAFOUND"

                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            geo.apiToken = token
                            geo.geofenceId = drv.item("GUID")
                            geo.name = drv.item("Name")
                            geo.latitude = drv.item("Latitude")
                            geo.longitude = drv.item("Longitude")
                            geo.radius = drv.item("RadiusFeet")
                            geo.address = drv.item("FullAddress")
                            geo.alertStatusId = drv.item("GeofenceAlertTypeID")
                            geo.speedLimit = drv.item("SpeedLimit")
                            geo.speedLimitEnabled = drv.item("IsSpeedLimit")
                            geo.geofenceTypeId = drv.item("GeofenceTypeGUID")
                            geo.isOk = True
                            geo.msg = ""
                        Next
                    End If

                Else
                    geo = New Geofence
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = msg

                End If
            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function GetGeofenceAlertStatus(ByVal token As String, ByVal geofenceGUID As String) As GeofenceAlertStatus Implements Ietrest.GetGeofenceAlertStatus
        Dim geo As GeofenceAlertStatus = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                geo = New GeofenceAlertStatus
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDTOKEN"

            ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                geo = New GeofenceAlertStatus
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDGEOFENCEID"

            Else

                dtData = dl.getGeofenceAlertStatus(token, True, geofenceGUID, msg)
                If msg.Length = 0 Then
                    geo = New GeofenceAlertStatus
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = "NODATAFOUND"

                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            geo.apiToken = token
                            geo.geofenceId = drv.item("GUID")
                            geo.alertStatusID = drv.item("GeofenceAlertTypeID")
                            geo.isOk = True
                            geo.msg = ""
                        Next
                    End If

                Else
                    geo = New GeofenceAlertStatus
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = msg
                End If
            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function GetGeofenceCrossings(ByVal token As String, ByVal geofenceGUID As String, ByVal dateFrom As Date, ByVal dateTo As Date) As List(Of GeofenceCrossings) Implements Ietrest.GetGeofenceCrossings
        Dim geoHist As New List(Of GeofenceCrossings)
        Dim hist As GeofenceCrossings = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                hist = New GeofenceCrossings
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDTOKEN"
                geoHist.Add(hist)

            ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                hist = New GeofenceCrossings
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDGEOFENCEID"
                geoHist.Add(hist)

            ElseIf Not IsDate(dateFrom) Then
                hist = New GeofenceCrossings
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDDATEFROM"
                geoHist.Add(hist)

            ElseIf Not IsDate(dateTo) Then
                hist = New GeofenceCrossings
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDDATETO"
                geoHist.Add(hist)

            ElseIf dateFrom > dateTo Then
                hist = New GeofenceCrossings
                hist.apiToken = token
                hist.isOk = False
                hist.msg = "INVALIDTIMEFRAME"
                geoHist.Add(hist)

            Else

                dtData = dl.GetGeofenceCrossings(token, True, geofenceGUID, dateFrom, dateTo, msg)
                If msg.Length = 0 Then

                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            hist = New GeofenceCrossings
                            hist.apiToken = token
                            hist.geofenceId = drv.item("GeofenceGUID")
                            hist.vehicleId = drv.item("DeviceGUID")
                            hist.dateFrom = drv.item("Arrival").ToString
                            hist.dateTo = drv.item("Departure").ToString
                            hist.isOk = True
                            hist.msg = ""
                            geoHist.Add(hist)
                        Next
                    End If

                    If geoHist.Count = 0 Then
                        hist = New GeofenceCrossings
                        hist.apiToken = token
                        hist.isOk = False
                        hist.msg = "NODATAFOUND"
                        geoHist.Add(hist)

                    End If
                Else
                    hist = New GeofenceCrossings
                    hist.apiToken = token
                    hist.isOk = False
                    hist.msg = msg
                    geoHist.Add(hist)
                End If
            End If

        Catch ex As Exception

        End Try

        Return geoHist

    End Function

    Public Function GetGeofenceSpeedLimitStatus(ByVal token As String, ByVal geofenceGUID As String) As GeofenceSpeedLimitStatus Implements Ietrest.GetGeofenceSpeedLimitStatus
        Dim geo As GeofenceSpeedLimitStatus = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                geo = New GeofenceSpeedLimitStatus
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDTOKEN"

            ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                geo = New GeofenceSpeedLimitStatus
                geo.apiToken = token
                geo.isOk = False
                geo.msg = "INVALIDGEOFENCEID"

            Else

                dtData = dl.GetGeofenceSpeedLimitStatus(token, True, geofenceGUID, msg)
                If msg.Length = 0 Then
                    geo = New GeofenceSpeedLimitStatus
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = "NODATAFOUND"

                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            geo.apiToken = token
                            geo.geofenceId = drv.item("GUID")
                            geo.isEnabled = drv.item("IsSpeedLimit")
                            geo.speedLimit = drv.item("SpeedLimit")
                            geo.isOk = True
                            geo.msg = ""
                        Next
                    End If

                Else
                    geo = New GeofenceSpeedLimitStatus
                    geo.apiToken = token
                    geo.isOk = False
                    geo.msg = msg
                End If
            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function DeleteGeofence(ByVal data As GeofenceRequest) As GeofenceResponse Implements Ietrest.DeleteGeofence
        Dim bResult As Boolean = False
        Dim geo As GeofenceResponse = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""

        Dim token As String = data.apiToken
        Dim geofenceGUID As String = data.geofenceId

        Try
            If Not BLCommon.IsGUID(token) Then
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = False
                geo.msg = "INVALIDTOKEN"

            ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = False
                geo.msg = "INVALIDGEOFENCEID"

            Else

                bResult = dl.DeleteGeofence(token, True, geofenceGUID, msg)
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = bResult
                geo.msg = msg
            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function SetGeofenceAlertType(ByVal data As GeofenceAlertTypeRequest) As GeofenceResponse Implements Ietrest.SetGeofenceAlertType
        Dim bResult As Boolean = False
        Dim geo As GeofenceResponse = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""

        Dim token As String = data.apiToken
        Dim geofenceGUID As String = data.geofenceId
        Dim alertStatusId As Integer = data.alertTypeId

        Try
            If Not BLCommon.IsGUID(token) Then
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = False
                geo.msg = "INVALIDTOKEN"

            ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = False
                geo.msg = "INVALIDGEOFENCEID"

            ElseIf alertStatusId < 0 Or alertStatusId > 3 Then
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = False
                geo.msg = "INVALIDALERTSTATUSID"

            Else

                bResult = dl.SetGeofenceAlertStatus(token, True, geofenceGUID, alertStatusId, msg)
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = bResult
                geo.msg = msg
            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function SetGeofenceSpeedLimitStatus(ByVal data As GeofenceSpeedLimitRequest) As GeofenceResponse Implements Ietrest.SetGeofenceSpeedLimitStatus
        Dim bResult As Boolean = False
        Dim geo As GeofenceResponse = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""

        Dim token As String = data.apiToken
        Dim geofenceGUID As String = data.geofenceId
        Dim isEnabled As Boolean = False

        Try
            isEnabled = data.isEnabled
        Catch ex As Exception
            isEnabled = False
        End Try

        Dim speedLimit As Integer

        Try
            If Not IsNumeric(data.speedLimit) Then
                geo = New GeofenceResponse
                geo.apiToken = token
                geo.geofenceId = geofenceGUID
                geo.isOk = False
                geo.msg = "INVALIDSPEEDFORMAT"

            Else
                speedLimit = data.speedLimit

                If Not BLCommon.IsGUID(token) Then
                    geo = New GeofenceResponse
                    geo.apiToken = token
                    geo.geofenceId = geofenceGUID
                    geo.isOk = False
                    geo.msg = "INVALIDTOKEN"

                ElseIf Not BLCommon.IsGUID(geofenceGUID) Then
                    geo = New GeofenceResponse
                    geo.apiToken = token
                    geo.geofenceId = geofenceGUID
                    geo.isOk = False
                    geo.msg = "INVALIDGEOFENCEID"

                ElseIf speedLimit < 0 Or speedLimit > 199 Then
                    geo = New GeofenceResponse
                    geo.apiToken = token
                    geo.geofenceId = geofenceGUID
                    geo.isOk = False
                    geo.msg = "INVALIDSPEEDLIMIT"

                Else

                    bResult = dl.SetGeofenceSpeedLimitStatus(token, True, geofenceGUID, isEnabled, speedLimit, msg)
                    geo = New GeofenceResponse
                    geo.apiToken = token
                    geo.geofenceId = geofenceGUID
                    geo.isOk = bResult
                    geo.msg = msg
                End If

            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function addGeofence(ByVal data As geofenceUpdate) As GeofenceResponse Implements Ietrest.addGeofence
        Dim geo As GeofenceResponse = Nothing

        Try
            geo = updateGeofence(data)
        Catch ex As Exception

        End Try

        Return geo

    End Function

    Public Function updateGeofence(ByVal data As geofenceUpdate) As GeofenceResponse Implements Ietrest.updateGeofence
        Dim bResult As Boolean = False
        Dim geo As GeofenceResponse = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""

        Try
            geo = New GeofenceResponse
            geo.apiToken = data.apiToken
            geo.geofenceId = ""
            geo.isOk = True

            geo.isOk = validateGeofenceData(data, geo.msg)

            If geo.isOk Then
                geo.geofenceId = dl.GeofencesUPDATE(data.apiToken, data.geofenceId, data.geofenceName, data.street, data.city, data.state, data.postalCode, data.latitude, data.longitude, data.radiusFeet, data.alertStatusId, data.speedLimitAlertEnabled, data.speedLimit, data.geofenceTypeId, msg)
                If geo.geofenceId.Length = 0 Then
                    geo.isOk = False
                    geo.msg = msg
                Else
                    geo.isOk = True
                    geo.msg = ""
                End If
            End If

        Catch ex As Exception

        End Try

        Return geo

    End Function

    Private Function validateGeofenceData(ByRef data As geofenceUpdate, ByRef msg As String) As Boolean
        Dim isOk As Boolean = True

        Try
            If Not BLCommon.IsGUID(data.apiToken) Then
                msg = "INVALIDTOKEN"
                isOk = False
            ElseIf data.geofenceName.Length = 0 Then
                msg = "INVALIDNAME"
                isOk = False
            End If

            If isOk Then
                If data.latitude = 0 Then
                    If data.street.Length = 0 Then
                        msg = "INVALIDSTREET"
                        isOk = False
                    ElseIf data.city.Length = 0 Then
                        msg = "INVALIDCITY"
                        isOk = False
                    ElseIf data.state.Length = 0 Then
                        msg = "INVALIDSTATE"
                        isOk = False
                    ElseIf data.postalCode.Length = 0 Then
                        msg = "INVALIDPOSTALCODE"
                        isOk = False
                    End If
                    If isOk Then
                        data.street = bl.Sanitize(data.street)
                        data.city = bl.Sanitize(data.city)
                        data.state = bl.Sanitize(data.state)
                        data.postalCode = bl.Sanitize(data.postalCode)
                    End If
                End If
            End If

        Catch ex As Exception
            isOk = False
        End Try

        Return isOk

    End Function

    Public Function GetGeofenceTypes(ByVal token As String) As List(Of GeofenceTypeResponse) Implements Ietrest.GetGeofenceTypes
        Dim lst As New List(Of GeofenceTypeResponse)
        Dim itm As GeofenceTypeResponse = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            If Not BLCommon.IsGUID(token) Then
                itm = New GeofenceTypeResponse
                itm.apiToken = token
                itm.isOk = False
                itm.msg = "INVALIDTOKEN"
                lst.Add(itm)

            Else

                dtData = dl.GetGeofenceTypes(token, msg)
                If msg.Length = 0 Then

                    If Not IsNothing(dtData) Then
                        dvData = dtData.DefaultView
                        For Each drv In dvData
                            itm = New GeofenceTypeResponse
                            itm.apiToken = token
                            itm.isOk = True
                            itm.msg = ""
                            itm.id = drv.item("ID")
                            itm.name = drv.item("Name")
                            lst.Add(itm)
                        Next
                    End If

                    If lst.Count = 0 Then
                        itm = New GeofenceTypeResponse
                        itm.apiToken = token
                        itm.isOk = False
                        itm.msg = "NODATAFOUND"
                        lst.Add(itm)

                    End If
                Else
                    itm = New GeofenceTypeResponse
                    itm.apiToken = token
                    itm.isOk = False
                    itm.msg = msg
                    lst.Add(itm)
                End If
            End If

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function addGeofenceType(ByVal data As GeofenceType) As etResponse Implements Ietrest.addGeofenceType
        Dim res As etResponse = Nothing

        Try
            res = saveGeofenceType(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function updateGeofenceType(ByVal data As GeofenceType) As etResponse Implements Ietrest.updateGeofenceType
        Dim res As etResponse = Nothing

        Try
            res = saveGeofenceType(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function saveGeofenceType(ByVal data As GeofenceType) As etResponse
        Dim res As etResponse = Nothing
        Dim newId As String = ""
        Dim dl As New DataLayer
        Dim msg As String = ""

        Dim token As String = data.apiToken
        Dim ID As String = data.id
        Dim Name As String = data.name

        Try
            If ID <> "" Then
                If Not BLCommon.IsGUID(token) Then
                    res = New etResponse
                    res.apiToken = token
                    res.isOk = False
                    res.msg = "INVALIDID"
                    Return res
                End If
            End If

            If Not BLCommon.IsGUID(token) Then
                res = New etResponse
                res.apiToken = token
                res.isOk = False
                res.msg = "INVALIDTOKEN"
            Else
                newId = dl.addGeofenceType(token, ID, Name, msg)
                If newId.Length > 0 Then
                    res = New etResponse
                    res.apiToken = token
                    res.id = newId
                    res.isOk = True
                    res.msg = msg
                Else
                    res = New etResponse
                    res.apiToken = token
                    res.id = ""
                    res.isOk = False
                    res.msg = msg
                End If
            End If

        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function deleteGeofenceType(ByVal token As String, ByVal id As String) As etResponse Implements Ietrest.deleteGeofenceType
        Dim res As etResponse = Nothing
        Dim bResult As Boolean = False
        Dim dl As New DataLayer
        Dim msg As String = ""

        Try
            If id <> "" Then
                If Not BLCommon.IsGUID(token) Then
                    res = New etResponse
                    res.apiToken = token
                    res.isOk = False
                    res.msg = "INVALIDID"
                    Return res
                End If
            End If

            If Not BLCommon.IsGUID(token) Then
                res = New etResponse
                res.apiToken = token
                res.isOk = False
                res.msg = "INVALIDTOKEN"
            Else
                bResult = dl.deleteGeofenceType(token, id, msg)
                res = New etResponse
                res.apiToken = token
                res.isOk = bResult
                res.msg = msg
                res.id = id
            End If

        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function isPointInGeofence(ByVal token As String, ByVal lat As String, ByVal lng As String) As pointInGeofence Implements Ietrest.isPointInGeofence
        Dim res As New pointInGeofence
        Dim decLat As Decimal = 0
        Dim decLng As Decimal = 0
        Dim dl As New DataLayer
        Dim msg As String = ""

        Try
            If IsNumeric(lat) Then
                decLat = CDec(lat)
            End If
            If IsNumeric(lng) Then
                decLng = CDec(lng)
            End If

            If decLat = 0 Or decLng = 0 Then
                res.isInside = False
            Else
                res = dl.isPointInGeofence(token, lat, lng, msg)
            End If

        Catch ex As Exception
            res.isInside = False
        End Try

        Return res

    End Function

#End Region

#Region "AWS"

    Function awsBouncesPOST(ByVal data As awsSES) As awsSES Implements Ietrest.awsBouncesPOST
        Dim emailBounced As String = ""
        Dim strData As String = ""
        Dim dl As New DataLayer
        Dim serializer As New JavaScriptSerializer
        Dim SubscribeURL As String = ""
        Dim responsebody As String = ""

        Try
            strData = serializer.Serialize(data)
            dl.blockedEmail_UPDATE(Date.Now.ToString, "AWS: Data Received", strData)

            If Not IsNothing(data.Type) Then
                If data.Type = "SubscriptionConfirmation" Then
                    SubscribeURL = data.SubscribeURL

                    Using client As New Net.WebClient
                        Dim reqparm As New Specialized.NameValueCollection
                        'reqparm.Add("param1", "somevalue")
                        'reqparm.Add("param2", "othervalue")
                        Dim responsebytes = client.UploadValues(SubscribeURL, "POST", reqparm)
                        responsebody = (New Text.UTF8Encoding).GetString(responsebytes)
                        dl.blockedEmail_UPDATE(Date.Now.ToString, "SubscriptionConfirmed", responsebody)
                    End Using

                End If
            Else
                strData = serializer.Serialize(data)

                If data.notificationType = "Bounce" Then
                    If data.bounce.bouncedRecipients.Count > 0 Then
                        For Each ele In data.bounce.bouncedRecipients
                            emailBounced = ele.emailAddress
                            dl.blockedEmail_UPDATE(emailBounced, data.notificationType, strData)
                        Next
                    End If
                ElseIf data.notificationType = "Complaint" Then
                    If data.complaint.complainedRecipients.Count > 0 Then
                        For Each ele In data.complaint.complainedRecipients
                            emailBounced = ele.emailAddress
                            dl.blockedEmail_UPDATE(emailBounced, data.notificationType, strData)
                        Next
                    End If
                End If
            End If

        Catch ex As Exception
            strData = serializer.Serialize(data)
            dl.blockedEmail_UPDATE(Date.Now.ToString, "AWS: Caught Exception", strData)
        End Try

        Return data

    End Function

#End Region

#Region "WLIUS ELOG"

    Function getWliusApiKey(ByVal token As String) As etWliusResponse Implements Ietrest.getWliusApiKey
        Dim res As New etWliusResponse
        Dim dl As New DataLayer

        Try
            res = dl.getWliusApiKey(token)
            res.UserToken = token

        Catch ex As Exception

        End Try

        Return res

    End Function
    Function getHasVideoApiKey(ByVal token As String) As etHvideoResponse Implements Ietrest.getHasVideoApiKey
        Dim res As New etHvideoResponse
        Dim dl As New DataLayer

        Try
            res = dl.GetHvideo_APIKEY(token)
            res.UserToken = token

        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#End Region

#Region "FDT Services"

    Function fdtContact(ByVal data As contactForm) As responseOk Implements Ietrest.fdtContact
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.ContactFrm(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "ET Site Services"

    Function ContactFrm(ByVal data As contactForm) As responseOk Implements Ietrest.ContactFrm
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.ContactFrm(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function quickContactFrm(ByVal data As quickContact) As responseOk Implements Ietrest.quickContactFrm
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.quickContactFrm(data)
        Catch ex As Exception

        End Try

        Return res

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


#Region "CRM"

    Function crmCasesMessageAdd(ByVal data As etAddCrmMessageRequest) As etResponse Implements Ietrest.crmCasesMessageAdd
        Dim dl As New DataLayer
        Dim res As New etResponse

        Try
            res = dl.crmCasesMessageAdd(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "REPORT"

    'nomenclature: lowerCamelCase
    'Function getTemperatureLog(ByVal token As String, ByVal reportId As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String, ByVal hourFrom As String, ByVal hourTo As String, ByVal isBatch As String) As List(Of jsonTempeatureLog) Implements Ietrest.getTemperatureLog
    Function getReportNew(ByVal Token As String, ByVal IsBatch As Integer, ByVal RecurrentID As Integer, ByVal CompanyID As Integer, ByVal GroupID As Integer,
                              ByVal IsAllDevices As Integer, ByVal ExcludeWeekends As Integer, ByVal ReportID As Integer, ByVal DeviceID As String, ByVal DateFrom As String, ByVal DateTo As String,
                              ByVal HourFrom As String, ByVal HourTo As String, ByVal ThisDayOfWeek As Integer, ByVal Param As String, ByVal Param2 As String, ByVal IsForExport As Integer) As String Implements Ietrest.getReportNew
        Dim ListJson As List(Of Object) = Nothing
        Dim dl As New DataLayer
        Dim Res As String = ""
        Dim reader = Nothing
        Try
            'dateFrom = dateFrom.Replace("/", "-")
            'dateTo = dateTo.Replace("/", "-")
            DateFrom = DateFrom + " " + HourFrom + ":00"
            DateTo = DateTo + " " + HourTo + ":00"
            Dim date1 As DateTime = DateTime.Parse(DateFrom) 'DateTime.ParseExact(dateFrom, "MM-dd-yyyy HH:mm tt", Nothing)
            Dim date2 As DateTime = DateTime.Parse(DateTo) 'DateTime.ParseExact(dateFrom, "MM-dd-yyyy HH:mm tt", Nothing)

            Dim fechaSalida3 As String = date1.ToString("yyyy-MM-dd HH:mm:ss")
            Dim fechaSalida4 As String = date2.ToString("yyyy-MM-dd HH:mm:ss")


            If (ReportID = 17) Then
                ListJson = dl.GetMultiDayTrail(Token, DeviceID, fechaSalida3, fechaSalida4, IsForExport)
            ElseIf (ReportID = 37) Then
                ListJson = dl.getTroubleshootingReport(Token, DeviceID, DateFrom, DateTo)
            Else
                ListJson = dl.ReportNew(Token, IsBatch, RecurrentID, CompanyID, GroupID, IsAllDevices, ExcludeWeekends, ReportID, DeviceID, fechaSalida3, fechaSalida4, HourFrom, HourTo, ThisDayOfWeek, Param, Param2, IsForExport)
            End If

            If (ListJson Is Nothing) Then
                Res = ""
            ElseIf ListJson.Count > 0 Then
                Res = JsonConvert.SerializeObject(ListJson)
            End If

        Catch ex As Exception
            Res = ex.Message
        End Try


        Return Res
    End Function
    Function CompaniesDevicesEvents(ByVal Token As String) As String Implements Ietrest.CompaniesDevicesEvents
        Dim ListJson As List(Of Object) = Nothing
        Dim dl As New DataLayer
        Dim Res As String = ""
        Dim reader = Nothing
        Try

            ListJson = dl.CompaniesDevicesEvents(Token)

            If (ListJson Is Nothing) Then
                Res = ""
            ElseIf ListJson.Count > 0 Then
                Res = JsonConvert.SerializeObject(ListJson)
            End If

        Catch ex As Exception
            Res = ex.Message


        End Try


        Return Res
    End Function

    Public Function GeofencesAll(Token As String) As String Implements Ietrest.GeofencesAll
        Dim ListJson As List(Of Object) = Nothing
        Dim dl As New DataLayer
        Dim Res As String = ""
        Dim reader = Nothing
        Try

            ListJson = dl.getGeofences_All(Token, 0, "")

            If (ListJson Is Nothing) Then
                Res = ""
            ElseIf ListJson.Count > 0 Then
                Res = JsonConvert.SerializeObject(ListJson)
            End If

        Catch ex As Exception
            Res = ex.Message
        End Try

        Return Res
    End Function
    Public Function DriversAll(Token As String) As String Implements Ietrest.DriversAll
        Dim ListJson As List(Of Object) = Nothing
        Dim dl As New DataLayer
        Dim Res As String = ""
        Dim reader = Nothing
        Try

            ListJson = dl.GetDrivers(Token)

            If (ListJson Is Nothing) Then
                Res = ""
            ElseIf ListJson.Count > 0 Then
                Res = JsonConvert.SerializeObject(ListJson)
            End If

        Catch ex As Exception
            Res = ex.Message
        End Try

        Return Res
    End Function

    Public Function postSendFeedBack(token As String, visitedPage As String, type As String, description As String) As String Implements Ietrest.postSendFeedBack
        Dim dl As New DataLayer
        Dim result As Boolean = False
        Dim response As String = ""
        Dim itm As basicList = Nothing
        Try
            result = dl.PostSendFeedBack(token, visitedPage, type, description)

            If result Then
                itm = New basicList With {
               .id = token,
               .value = "OK"
                }
            Else
                itm = New basicList With {
               .id = token,
               .value = "-1",
               .name = "error when registering the information"
                }
            End If
        Catch ex As Exception
            itm = New basicList With {
               .id = token,
               .value = "-1",
               .name = "error when registering the information: " + ex.Message
                }
        Finally
            response = JsonConvert.SerializeObject(itm)

        End Try
        Return response
    End Function
    Public Function GetFeedbakTypes(ByVal token As String) As String Implements Ietrest.GetFeedbakTypes
        Dim itm As basicList = Nothing
        Dim dl As New DataLayer
        Dim result As Boolean = False
        Dim response As String = ""
        Try
            response = dl.GetFeedbakTypes(token)
        Catch ex As Exception
            response = "{Status:-1,Messagge:" + ex.Message + "}"
        End Try

        Return response
    End Function
#End Region

#Region "VIDEO"
    Public Function ValidateTokenVideo(ByVal token As String, ByVal ep As Boolean) As CustomerVideoEp Implements Ietrest.ValidateTokenVideo
        Dim dev As New CustomerVideoEp
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""
        Dim validator As Boolean = False
        Try
            dsData = dl.ValidateTokenVideo(token, ep)
            If Not IsNothing(dsData) Then
                dvData = dsData.Tables(0).DefaultView
                For Each drv In dvData
                    dev.name = drv.item("Video_AccName")
                    dev.email = drv.item("Video_email")
                    dev.fleetID = drv.item("accountid")
                Next

            End If
        Catch ex As Exception

        End Try
        Return dev

    End Function
    Public Function ValidateTokenVideoApp(ByVal token As String, ByVal ep As Boolean) As CustomerVideo Implements Ietrest.ValidateTokenVideoApp
        Dim dev As New CustomerVideo
        Dim dl As New DataLayer
        Dim dsData As New DataSet
        Dim dvData As DataView
        Dim msg As String = ""
        Dim validator As Boolean = False
        Try

            dsData = dl.ValidateTokenVideo(token, ep)
            If Not IsNothing(dsData) Then
                dvData = dsData.Tables(0).DefaultView
                For Each drv In dvData
                    dev.CompanyName = drv.item("Video_AccName")
                    dev.HasVideo = drv.item("hasvideo")
                    dev.VideoKey = drv.item("videokey")
                    dev.AccountId = drv.item("accountid")
                    dev.IsDirectDealer = drv.item("IsDirectDealer")
                Next
            End If
        Catch ex As Exception

        End Try
        Return dev

    End Function


#End Region



End Class