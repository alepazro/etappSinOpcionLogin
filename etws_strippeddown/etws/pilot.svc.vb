' NOTE: You can use the "Rename" command on the context menu to change the class name "pilot" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select pilot.svc or pilot.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation.WebScriptServiceHostFactory
Imports System.Data
Imports System.Web.Script.Serialization
Imports System.IO

Public Class pilot
    Implements Ipilot

#Region "Authorization and Credentials"

    Public Function authorization(ByVal login As String, ByVal password As String, ByVal expDays As String, ByVal lat As String, ByVal lng As String) As pilotUser Implements Ipilot.authorization
        Dim u As New pilotUser
        Dim dl As New DataLayer
        Dim intExpDays As Integer
        Dim decLat As Decimal = 0
        Dim decLng As Decimal = 0
        Dim dvData As DataView
        Dim msg As String = ""
        Dim drv As DataRowView

        Try
            If Not IsNumeric(expDays) Then
                expDays = "1"
            End If
            intExpDays = CInt(expDays)
            If intExpDays < 1 Then
                intExpDays = 1
            End If

            If Not IsNumeric(lat) Then
                lat = "0"
            End If
            decLat = CDec(lat)

            If Not IsNumeric(lng) Then
                lng = "0"
            End If
            decLng = CDec(lng)

            u.isOk = False
            u.msg = "INVALIDCREDENTIALS"

            dvData = dl.ValidateCredentials(login, password, intExpDays, "PILOT", "", decLat, decLng, msg)
            If Not IsNothing(dvData) Then
                If dvData.Count > 0 Then
                    drv = dvData.Item(0)
                    u.isOk = True
                    u.msg = ""
                    u.fullName = drv.Item("FullName")
                    u.isCheckedIn = drv.Item("IsCheckedIn")
                    u.checkedInSince = drv.Item("CheckedInSince")
                    u.token = drv.Item("Token")
                    u.deviceId = drv.Item("DeviceID")

                    Try
                        u.versionId = drv.Item("VersionName")
                    Catch ex As Exception

                    End Try
                End If
            End If

        Catch ex As Exception
            u.isOk = False
            u.msg = "INVALIDCREDENTIALS"
        End Try

        Return u

    End Function

    Public Function authorization2(ByVal login As String, ByVal password As String, ByVal lat As String, ByVal lng As String) As pilotUser2 Implements Ipilot.authorization2
        Dim u As New pilotUser2
        Dim dl As New DataLayer
        Dim decLat As Decimal = 0
        Dim decLng As Decimal = 0
        Dim dvData As DataView
        Dim msg As String = ""
        Dim drv As DataRowView

        Try
            If Not IsNumeric(lat) Then
                lat = "0"
            End If
            decLat = CDec(lat)

            If Not IsNumeric(lng) Then
                lng = "0"
            End If
            decLng = CDec(lng)

            u.isOk = False
            u.msg = "INVALIDCREDENTIALS"

            dvData = dl.ValidateCredentials(login, password, 999, "PILOT", "", decLat, decLng, msg)
            If Not IsNothing(dvData) Then
                If dvData.Count > 0 Then
                    drv = dvData.Item(0)
                    u.isOk = True
                    u.msg = msg
                    u.companyId = drv.Item("CompanyUniqueKey")
                    u.userId = drv.Item("GUID")
                    u.fullName = drv.Item("FullName")
                    u.token = drv.Item("Token")
                    u.deviceId = drv.Item("DeviceID")

                    Try
                        u.versionId = drv.Item("VersionName")
                    Catch ex As Exception

                    End Try
                End If
            End If

        Catch ex As Exception
            u.isOk = False
            u.msg = "INVALIDCREDENTIALS"
        End Try

        Return u

    End Function

    Public Function recoverCredentials(ByVal data As recoverCredentialsRequest) As responseOk Implements Ipilot.recoverCredentials
        Dim r As New responseOk
        Dim dl As New DataLayer

        Try
            r.isOk = dl.recoverCredentials(data.email, "PILOT", r.transId, r.msg)

        Catch ex As Exception
            r.isOk = False
        End Try

        Return r

    End Function

#End Region

#Region "Basic Information Tables"

    Public Function getDevices(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getDevices
        Dim lstDevices As New List(Of idNameItem)
        Dim dev As idNameItem = Nothing
        Dim dsData As DataSet
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dvData As DataView

        Try
            dsData = dl.getDevices(token, False, "", msg)
            If Not IsNothing(dsData) Then
                If dsData.Tables.Count >= 2 Then
                    dvData = dsData.Tables(1).DefaultView
                    For Each drv In dvData
                        dev = New idNameItem
                        dev.id = drv.item("uniqueKey")
                        dev.name = drv.item("Name")
                        lstDevices.Add(dev)
                    Next
                End If
            End If

        Catch ex As Exception

        End Try

        Return lstDevices

    End Function

    Public Function getCountryStates(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getCountryStates
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCountryStates(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("shortCode")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

#End Region

#Region "Devices"

    Public Function GetDevicesList(ByVal token As String, ByVal sourceId As String) As List(Of FleetDeviceVideo) Implements Ipilot.GetDevicesList
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
                        dev.isOk = True
                        devList.Add(dev)
                    Next
                End If
            End If

            If devList.Count = 0 Then
                dev = New FleetDeviceVideo
                dev.result = "INVALIDTOKEN"
                dev.isOk = False
                devList.Add(dev)
            End If

        Catch ex As Exception

        End Try

        Return devList

    End Function

    Public Function GetDeviceInfo(ByVal token As String, ByVal id As String, ByVal sourceId As String) As FleetDeviceVideo Implements Ipilot.GetDeviceInfo
        Dim dev As FleetDeviceVideo = Nothing
        Dim msg As String = ""
        Dim dvData As DataView = Nothing
        Dim dl As New DataLayer

        Try
            dvData = dl.Devices_GetByGUID_BasicInfo(id, msg)

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

    Public Function GetTrail(ByVal token As String, ByVal id As String, ByVal trailDate As String, ByVal hourFrom As String, ByVal hourTo As String, ByVal sourceId As String) As List(Of Trail) Implements Ipilot.GetTrail
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

    Public Function InfoDevicesInputs(ByVal token As String, ByVal deviceid As Integer, ByVal count As Integer) As DevicesInformationInputs Implements Ipilot.InfoDevicesInputs
        Dim dl As New DataLayer
        Dim resonse As New DevicesInformationInputs
        resonse = dl.InfoDevicesInputs(token, deviceid, count)
        Return resonse

    End Function
    Public Function InfoDevicesInputs_Update(ByVal token As String, ByVal data As DevicesInformationInputs) As responseOk Implements Ipilot.InfoDevicesInputs_Update
        Dim dl As New DataLayer
        Dim resonse As New responseOk
        Dim count As Integer = 1
        While (count <= data.Output)
            If count = 1 Then
                resonse = dl.InfoDevicesInputs_Update(token, data, count, data.Output1)
            ElseIf count = 2 Then
                resonse = dl.InfoDevicesInputs_Update(token, data, count, data.Output2)
            ElseIf count = 3 Then
                resonse = dl.InfoDevicesInputs_Update(token, data, count, data.Output3)
            Else
                resonse = dl.InfoDevicesInputs_Update(token, data, count, data.Output4)
            End If
            count += 1
        End While
        Return resonse

    End Function

#End Region

#Region "Check In/Out"

    Function getCheckInReasons(ByVal token As String, ByVal isFullSync As String) As List(Of checkInReason) Implements Ipilot.getCheckInReasons
        Dim lst As New List(Of checkInReason)
        Dim dl As New DataLayer
        Dim bIsFullSync As Boolean = True

        Try
            If isFullSync = "1" Then
                bIsFullSync = True
            Else
                bIsFullSync = False
            End If
            lst = dl.getCheckInReasons(token, bIsFullSync)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function postCheckInLog(ByVal token As String, ByVal data As List(Of checkInEvent)) As responseOk Implements Ipilot.postCheckInLog
        Dim res As New responseOk
        Dim dl As New DataLayer
        Dim itm As checkInEvent

        Try
            For Each itm In data
                res = dl.postCheckInLog(token, itm)
            Next
        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function updateCheckInStatus(ByVal data As checkInRequest) As responseOk Implements Ipilot.updateCheckInStatus
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim odometer As Integer = 0
        Dim isCheckedIn As Boolean = False
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0

        Try
            If Not IsNumeric(data.odometer) Then
                data.odometer = "0"
            End If
            odometer = CInt(data.odometer)

            If data.isCheckedIn.ToLower = "true" Then
                isCheckedIn = True
            Else
                isCheckedIn = False
            End If

            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            r.isOk = dl.updateCheckInStatus(data.token, data.deviceId, odometer, isCheckedIn, lat, lng, "PILOT", r.transId, r.msg)

        Catch ex As Exception
            r.isOk = False
        End Try

        Return r

    End Function

#End Region

#Region "Jobs"
    Public Function GetJobsNew(ByVal token As String, ByVal noCache As String, ByVal statId As String, ByVal wzId As String, ByVal techId As String, ByVal jobNo As String, ByVal custName As String) As List(Of jobnew) Implements Ipilot.GetJobsNew
        'get jobs from compani
        Dim lst As New List(Of jobnew)
        Dim dl As New DataLayer

        Try
            lst = dl.GetJobsNew(token, statId, wzId, techId, jobNo, custName)
        Catch ex As Exception

        End Try

        Return lst

    End Function
    Public Function getJobNew(ByVal token As String, ByVal jobId As String) As Jobobject Implements Ipilot.getJobNew
        'get job single
        Dim itmJson = ""
        Dim itm As Jobobject = Nothing
        Dim dl As New DataLayer

        Try
            itm = dl.getJob(token, jobId)
        Catch ex As Exception
        End Try

        Return itm

    End Function
    Public Function getJobCategoriesList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getJobCategoriesList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getJobCategoriesList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function
    Public Function getJobPrioritiesList(ByVal token As String) As List(Of companyIdNameItem) Implements Ipilot.getJobPrioritiesList
        Dim lstItems As New List(Of companyIdNameItem)
        Dim itm As companyIdNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getJobPrioritiesList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New companyIdNameItem
                itm.companyId = drv.item("CompanyUniqueKey")
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function
    Public Function postJobNew(ByVal token As String, ByVal data As Jobobject) As responseOk Implements Ipilot.postJobNew

        Dim resp As New responseOk
        Dim dl As New DataLayerJobs

        Try
            'data.geo = "https://maps.google.com/?q=" & Math.Round(lat, 5).ToString & "," & Math.Round(lng, 5).ToString
            resp.transId = dl.saveJob(token, data)
            If resp.transId <> "" Then
                resp.isOk = True
            End If
            If resp.isOk Then
                For Each item In data.jobstoplist
                    item.JobUniqueKey = resp.transId
                    item.CompanyID = 0
                    item.DeviceID = data.deviceId
                    item.DriverId = data.driverId
                    item.DueDate = data.dueDate
                    item.StartOn = data.StartOn
                    dl.addJobStop(token, item)
                Next
            End If


        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try
        Return resp

    End Function
    Function GetJobStops(ByVal token As String, ByVal jobUniquekey As String) As List(Of JobStop) Implements Ipilot.GetJobStops
        'Dim json As String = ""
        Dim dl As New DataLayer
        Dim json As New List(Of JobStop)

        Try
            json = dl.Jobs_Stops_Get(token, jobUniquekey)
        Catch ex As Exception
        End Try

        Return json

    End Function
    Public Function getImagesNew(ByVal token As String, ByVal JobUniqueKey As String) As List(Of imgData) Implements Ipilot.getImagesNew
        Dim lst As New List(Of imgData)
        Dim itm As imgData = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getImagesNew(token, JobUniqueKey, isOk, msg)

            For Each drv In dtData.DefaultView
                itm = New imgData
                itm.isOk = True
                itm.imageId = drv.item("imgGuid") 'This is the ID assigned by the server
                itm.imgName = drv.item("ImgName")
                itm.fileName = drv.item("Filename")
                itm.fileType = drv.item("fileType")
                itm.UrlImagen = drv.item("AWS_URL")
                itm.eventDate = drv.item("CreatedOn")
                lst.Add(itm)
            Next
        Catch ex As Exception

        End Try

        Return lst

    End Function
    Public Function postImageNew(ByVal data As imgData) As responseOk Implements Ipilot.postImageNew
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Dim eventDate As DateTime
        Dim decLat As Decimal = 0
        Dim decLng As Decimal = 0
        Dim intImgType As Integer = 0

        Try
            If IsNumeric(data.imgType) Then
                intImgType = data.imgType
            Else
                intImgType = 1 '1: Picture, 2: Signature
            End If
            If IsDate(data.eventDate) Then
                eventDate = CDate(data.eventDate)
            Else
                eventDate = Date.UtcNow
            End If

            If IsNumeric(data.lat) Then
                decLat = CDec(data.lat)
            End If
            If IsNumeric(data.lng) Then
                decLng = CDec(data.lng)
            End If

            resp = dl.postImageNew(data)

        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try

        Return resp

    End Function

    Public Function postJobStop(ByVal token As String, ByVal data As JobStop) As responseOk Implements Ipilot.postJobStop

        Dim resp As New responseOk
        Dim dl As New DataLayer
        Try
            resp.isOk = dl.addJobStop(token, data)
        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try
        Return resp

    End Function

    Public Function getStatus(ByVal token As String) As List(Of StatusJobs) Implements Ipilot.getStatus
        Dim lstItems As New List(Of StatusJobs)
        Dim itm As StatusJobs = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getJobStatus(token)
            For Each drv In dtData.DefaultView
                itm = New StatusJobs
                itm.UniqueKey = drv.item("UniqueKey")
                itm.Name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function
    Public Function GetNotes(ByVal token As String, ByVal jobUniquekey As String, uniqueKey As String, action As Byte) As List(Of JobNote) Implements Ipilot.GetNotes
        'Dim json As String = ""
        Dim dl As New DataLayer
        Dim json As New List(Of JobNote)

        Try
            json = dl.Jobs_Notes_Get(token, jobUniquekey, uniqueKey, action)
        Catch ex As Exception
        End Try

        Return json

    End Function
    Public Function postJobNote(ByVal token As String, ByVal data As JobNote) As responseOk Implements Ipilot.postJobNote
        Dim resp As New responseOk
        Dim dl As New DataLayer
        Try
            resp.isOk = dl.addJobNote(token, data)
        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try
        Return resp

    End Function

    Public Function postStatusJob(ByVal token As String, ByVal data As JobStatus) As responseOk Implements Ipilot.postStatusJob
        Dim resp As New responseOk
        Dim dl As New DataLayer
        Try
            resp.isOk = dl.addJobStatus(token, data)
        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try
        Return resp

    End Function


    '-------------------------------------------------------
    Public Function getJob(ByVal token As String, ByVal jobId As String, ByVal lat As String, ByVal lng As String) As pilotJob Implements Ipilot.getJob
        Dim job As pilotJob = Nothing
        Dim dtData As New DataTable
        Dim dl As New DataLayer
        Dim decLat As Decimal
        Dim decLng As Decimal
        Dim msg As String = ""
        Dim isOk As Boolean = True

        Try
            If Not IsNumeric(lat) Then
                lat = "0"
            End If
            decLat = CDec(lat)

            If Not IsNumeric(lng) Then
                lng = "0"
            End If
            decLng = CDec(lng)

            dtData = dl.getJobs(token, jobId, decLat, decLng, isOk, msg)
            For Each drv In dtData.DefaultView
                job = getJobItem(drv)
            Next

        Catch ex As Exception

        End Try

        Return job

    End Function

    Private Function getJobItem(ByVal drv As DataRowView) As pilotJob
        Dim job As pilotJob = Nothing

        Try
            job = New pilotJob
            job.jobId = drv.Item("JobID")
            job.jobNumber = drv.Item("JobNumber")
            job.custId = drv.Item("CustomerID")
            job.custName = drv.Item("CustomerName")
            job.custAddress = drv.Item("CustomerAddress")
            job.custPhone = drv.Item("CustomerPhone")
            job.custContact = drv.Item("CustomerContact")
            job.dueDate = drv.Item("DueDate")
            job.statusId = drv.Item("StatusID")
            job.jobDetails = drv.Item("JobDetails")

        Catch ex As Exception

        End Try

        Return job

    End Function

    Public Function addJob(ByVal data As pilotJob) As jobResponse Implements Ipilot.addJob
        Dim resp As New jobResponse
        Dim dl As New DataLayer
        Dim dueDate As Date
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0

        Try
            If Not IsDate(data.dueDate) Then
                data.dueDate = Date.UtcNow
            End If
            dueDate = data.dueDate
            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            data.custName = HttpUtility.UrlDecode(data.custName)
            data.custAddress = HttpUtility.UrlDecode(data.custAddress)
            data.custPhone = HttpUtility.UrlDecode(data.custPhone)
            data.custContact = HttpUtility.UrlDecode(data.custContact)
            data.jobDetails = HttpUtility.UrlDecode(data.jobDetails)
            data.notes = HttpUtility.UrlDecode(data.notes)

            resp.isOk = dl.addJob(data.token, data.jobNumber, data.custId, data.custName, data.custAddress, data.custPhone, data.custContact, dueDate, data.statusId, data.jobDetails, data.notes, lat, lng, resp.jobId, resp.custId, resp.msg)

        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function updateJobStatus(ByVal data As pilotJobStatusUpdate) As responseOk Implements Ipilot.updateJobStatus
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0

        Try
            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            r.isOk = dl.updateJobStatus(data.token, data.jobId, data.statusId, lat, lng, msg)
            r.msg = msg
        Catch ex As Exception
            r.isOk = False
        End Try

        Return r

    End Function

    Public Function addJobNote(ByVal data As pilotJobNote) As responseOk Implements Ipilot.addJobNote
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0

        Try
            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            data.note = HttpUtility.UrlDecode(data.note)

            r.isOk = dl.addJobNote(data.token, data.jobId, data.note, lat, lng, msg)
            r.msg = msg
        Catch ex As Exception
            r.isOk = False
        End Try

        Return r

    End Function

    Public Function removeJob(ByVal data As pilotJobRemove) As responseOk Implements Ipilot.removeJob
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0

        Try
            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            r.isOk = dl.removeJob(data.token, data.jobId, lat, lng, msg)
            r.msg = msg
        Catch ex As Exception
            r.isOk = False
        End Try

        Return r

    End Function

#End Region

#Region "My Group Location"

    Public Function getMyGroup(ByVal token As String, ByVal lat As String, ByVal lng As String) As List(Of myTeam) Implements Ipilot.getMyGroup
        Dim lstDevices As New List(Of myTeam)
        Dim dev As myTeam = Nothing
        Dim dsData As DataSet
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dvData As DataView
        Dim decLat As Decimal
        Dim decLng As Decimal

        Try
            If Not IsNumeric(lat) Then
                lat = "0"
            End If
            decLat = CDec(lat)

            If Not IsNumeric(lng) Then
                lng = "0"
            End If
            decLng = CDec(lng)

            dsData = dl.getDevices(token, False, "", msg)
            If Not IsNothing(dsData) Then
                If dsData.Tables.Count >= 2 Then
                    dvData = dsData.Tables(1).DefaultView
                    For Each drv In dvData
                        dev = New myTeam
                        dev.id = drv.item("uniqueKey")
                        dev.name = drv.item("Name")
                        dev.lat = drv.item("Latitude")
                        dev.lng = drv.item("Longitude")
                        dev.address = drv.item("FullAddress")
                        dev.evTime = drv.item("EventDate").ToString
                        dev.iconUrl = drv.item("IconURL")
                        lstDevices.Add(dev)
                    Next
                End If
            End If

        Catch ex As Exception

        End Try

        Return lstDevices

    End Function

#End Region

#Region "Inspection Lists"

    Public Function getInspectionLists(ByVal token As String) As List(Of inspectionList) Implements Ipilot.getInspectionLists
        Dim lsts As New List(Of inspectionList)
        Dim lst As inspectionList = Nothing
        Dim dtData As New DataTable
        Dim dl As New DataLayer
        Dim msg As String = ""

        Try

            dtData = dl.getInspectionLists(token, msg)
            For Each drv In dtData.DefaultView
                lst = New inspectionList
                lst.id = drv.item("ID")
                lst.name = drv.item("Name")
                lsts.Add(lst)
            Next

        Catch ex As Exception

        End Try

        Return lsts

    End Function

    Public Function getInspectionListItems(ByVal token As String, ByVal listId As String) As List(Of inspectionItem) Implements Ipilot.getInspectionListItems
        Dim lsts As New List(Of inspectionItem)
        Dim lst As inspectionItem = Nothing
        Dim dtData As New DataTable
        Dim dl As New DataLayer
        Dim msg As String = ""

        Try
            dtData = dl.getInspectionListItems(token, listId, msg)
            For Each drv In dtData.DefaultView
                lst = New inspectionItem
                lst.listId = drv.item("listId")
                lst.itemId = drv.item("itemId")
                lst.name = drv.item("Name")
                lsts.Add(lst)
            Next

        Catch ex As Exception

        End Try

        Return lsts

    End Function

    Public Function saveInspection(ByVal data As inspectionLog) As responseOk Implements Ipilot.saveInspection
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim odometer As Integer = 0
        Dim dateEventDate As Date

        Try
            If Not IsNumeric(data.odometer) Then
                data.odometer = "0"
            End If
            odometer = CInt(data.odometer)

            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            If Not IsDate(data.eventDate) Then
                data.eventDate = Date.UtcNow.ToString
            End If
            dateEventDate = CDate(data.eventDate)

            r.isOk = dl.saveInspection(data.token, "PILOT", data.deviceId, odometer, dateEventDate, data.listId, data.itemId, data.passed, data.failed, data.repaired, data.notes, lat, lng, r.transId, r.msg)

        Catch ex As Exception

        End Try

        Return r

    End Function

#End Region

#Region "Fuel Purchase"

    Public Function saveFuelPurchase(ByVal data As fuelPurchase) As responseOk Implements Ipilot.saveFuelPurchase
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim odometer As Decimal = 0
        Dim dateEventDate As Date
        Dim qty As Decimal = 0
        Dim amount As Decimal = 0

        Try
            If Not IsNumeric(data.odometer) Then
                data.odometer = 0
            End If
            odometer = CDec(data.odometer)

            If Not IsNumeric(data.qty) Then
                data.qty = 0
            End If
            qty = CDec(data.qty)

            If Not IsNumeric(data.amount) Then
                data.amount = 0
            End If
            amount = CDec(data.amount)

            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            data.eventDate = HttpUtility.UrlDecode(data.eventDate)
            If Not IsDate(data.eventDate) Then
                data.eventDate = Date.UtcNow.ToString
            End If
            dateEventDate = CDate(data.eventDate)

            r.isOk = dl.saveFuelPurchase(data.token, data.deviceId, odometer, qty, amount, data.address, data.stateId, data.postalCode, dateEventDate, lat, lng, "PILOT", r.transId, r.msg)

        Catch ex As Exception

        End Try

        Return r

    End Function

    Public Function saveFuelPurchase2(ByVal data As fuelPurchase2) As responseOk Implements Ipilot.saveFuelPurchase2
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim odometer As Decimal = 0
        Dim dateEventDate As Date
        Dim qty As Decimal = 0
        Dim amount As Decimal = 0

        Try
            If Not IsNumeric(data.odometer) Then
                data.odometer = 0
            End If
            odometer = CDec(data.odometer)

            If Not IsNumeric(data.gallons) Then
                data.gallons = 0
            End If
            qty = CDec(data.gallons)

            If Not IsNumeric(data.total) Then
                data.total = 0
            End If
            amount = CDec(data.total)

            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            data.eventDate = HttpUtility.UrlDecode(data.eventDate)
            If Not IsDate(data.eventDate) Then
                data.eventDate = Date.UtcNow.ToString
            End If
            dateEventDate = CDate(data.eventDate)

            r.isOk = dl.saveFuelPurchase2(data.token, data.licensePlate, odometer, qty, amount, dateEventDate, lat, lng, data.locType, data.locAccuracy, "ETRACKFIELD", r.transId, r.msg)

        Catch ex As Exception

        End Try

        Return r

    End Function

#End Region

#Region "Driver Status"

    Public Function getDriverStatusList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getDriverStatusList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getDriverStatusList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function updateDriverLog(ByVal data As driverLog) As responseOk Implements Ipilot.updateDriverLog
        Dim r As New responseOk
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim origStartDate As String = ""
        Dim origEndDate As String = ""
        Dim origDurationMins As String = ""
        Dim startDate As Date
        Dim endDate As Date
        Dim durationMins As Integer
        Dim lat As Decimal = 0
        Dim lng As Decimal = 0
        Dim odometer As Integer = 0

        Try
            data.startDate = HttpUtility.UrlDecode(data.startDate)
            origStartDate = data.startDate
            If Not IsDate(data.startDate) Then
                data.startDate = Date.UtcNow.ToString
            End If
            startDate = CDate(data.startDate)

            data.endDate = HttpUtility.UrlDecode(data.endDate)
            origEndDate = data.endDate
            If Not IsDate(data.endDate) Then
                data.endDate = Date.UtcNow.ToString
            End If
            endDate = CDate(data.endDate)

            origDurationMins = data.durationMins
            If Not IsNumeric(data.durationMins) Then
                data.durationMins = 0
            End If
            durationMins = CDec(data.durationMins)

            If Not IsNumeric(data.odometer) Then
                data.odometer = "0"
            End If
            odometer = CInt(data.odometer)

            If Not IsNumeric(data.lat) Then
                data.lat = 0
            End If
            lat = CDec(data.lat)

            If Not IsNumeric(data.lng) Then
                data.lng = 0
            End If
            lng = CDec(data.lng)

            r.isOk = dl.updateDriverLog(data.token, data.deviceId, data.driverStatus, origStartDate, origEndDate, origDurationMins, startDate, endDate, durationMins, data.address, data.stateId, data.postalCode, odometer, lat, lng, "PILOT", r.transId, r.msg)
            r.msg = msg
        Catch ex As Exception
            r.isOk = False
        End Try

        Return r

    End Function

#End Region

#Region "Field Service Module - GetAll* PostAll* Methods"

    Public Function getAllData(ByVal token As String, ByVal lastSyncOn As String) As allDataGet Implements Ipilot.getAllData
        Dim data As New allDataGet
        Dim dl As New DataLayer
        Dim dateLastSyncOn As Date

        Try
            If IsDate(lastSyncOn) Then
                dateLastSyncOn = Date.Parse(lastSyncOn)
            Else
                dateLastSyncOn = Date.Parse("1/1/2000")
            End If

            data = dl.getAllData(token, dateLastSyncOn, data.isOk, data.msg)

            data.jobs = New List(Of Job2)
            data.customers = New List(Of Customer2)
            data.custContacts = New List(Of CustContact2)

            If data.isOk Then
                'Get Jobs
                Dim dtJobs As New DataTable
                Dim jobItm As Job2

                dtJobs = dl.getJobs2(token, dateLastSyncOn, data.isOk, data.msg)
                For Each drv In dtJobs.DefaultView
                    jobItm = New Job2
                    jobItm.jobId = drv.item("jobId")
                    jobItm.jobNumber = drv.item("jobNumber")
                    jobItm.custId = drv.item("custId")
                    jobItm.contactId = drv.item("contactId")
                    jobItm.dueDate = drv.item("dueDate")
                    jobItm.statusId = drv.item("statusId")
                    jobItm.priorityId = drv.item("priorityId")
                    jobItm.jobDescription = drv.item("jobName")
                    jobItm.createdOn = drv.item("createdOn").ToString
                    data.jobs.Add(jobItm)
                Next
            End If

            If data.isOk = True Then
                'Get Customers Data
                dl.getCustomersAllData(token, dateLastSyncOn, data)
            End If


        Catch ex As Exception

        End Try

        Return data

    End Function

    Public Function postAllData(ByVal data As allDataPost) As allDataPostResult Implements Ipilot.postAllData
        Dim result As New allDataPostResult
        Dim res As postResult
        Dim failed As List(Of failedRecord)
        Dim failedRec As failedRecord
        Dim dl As New DataLayer
        Dim isOk As Boolean = False
        Dim msg As String = ""
        Dim transId As String = ""
        Dim globalToken As String = ""

        Try
            If Not IsNothing(data.token) Then
                globalToken = data.token
            End If
        Catch ex As Exception
            globalToken = ""
        End Try

        'Jobs
        res = New postResult
        res.isOk = True
        failed = New List(Of failedRecord)
        res.failed = failed
        result.jobs = res

        Try
            If Not IsNothing(data.jobs) Then
                Dim job As New Job2
                For Each job In data.jobs

                    If IsNothing(job.token) Then
                        job.token = globalToken
                    End If

                    isOk = dl.postJobs2("PILOT", job, isOk, transId, msg)
                    If isOk = False Then
                        result.jobs.isOk = False
                        failedRec = New failedRecord
                        failedRec.id = job.jobId
                        failedRec.msg = msg
                        result.jobs.failed.Add(failedRec)
                    End If
                Next
            End If
        Catch ex As Exception
            result.jobs.isOk = False
        End Try

        'jobStatuses
        res = New postResult
        res.isOk = True
        failed = New List(Of failedRecord)
        res.failed = failed
        result.jobStatuses = res

        Try
            If Not IsNothing(data.jobStatuses) Then
                Dim stat As New JobStatus
                For Each stat In data.jobStatuses

                    If IsNothing(stat.token) Then
                        stat.token = globalToken
                    End If

                    isOk = dl.postJobsStatusLog("PILOT", stat, isOk, transId, msg)
                    If isOk = False Then
                        result.jobStatuses.isOk = False
                        failedRec = New failedRecord
                        failedRec.id = stat.statusId
                        failedRec.msg = msg
                        failedRec.ref1 = stat.jobId
                        failedRec.ref2 = ""
                        result.jobStatuses.failed.Add(failedRec)
                    End If
                Next
            End If
        Catch ex As Exception
            result.jobStatuses.isOk = False
        End Try

        'jobNotes
        res = New postResult
        res.isOk = True
        failed = New List(Of failedRecord)
        res.failed = failed
        result.jobNotes = res

        Try
            If Not IsNothing(data.jobNotes) Then
                Dim note As New JobNote
                For Each note In data.jobNotes

                    If IsNothing(note.token) Then
                        note.token = globalToken
                    End If

                    isOk = dl.postJobsNotesLog("PILOT", note, isOk, transId, msg)
                    If isOk = False Then
                        result.jobNotes.isOk = False
                        failedRec = New failedRecord
                        failedRec.id = note.note
                        failedRec.msg = msg
                        failedRec.ref1 = note.jobId
                        failedRec.ref2 = ""
                        result.jobNotes.failed.Add(failedRec)
                    End If
                Next
            End If
        Catch ex As Exception
            result.jobNotes.isOk = False
        End Try

        'customers
        res = New postResult
        res.isOk = True
        failed = New List(Of failedRecord)
        res.failed = failed
        result.customers = res

        Try
            If Not IsNothing(data.customers) Then
                Dim cus As New Customer2
                For Each cus In data.customers

                    isOk = True

                    If IsNothing(cus.token) Then
                        cus.token = globalToken
                    End If
                    If cus.custId = "" Then
                        isOk = False
                        msg = "INVALID_CUSTID"
                    End If

                    If isOk Then
                        If cus.name = "" Then
                            isOk = False
                            msg = "INVALID_NAME"
                        End If
                    End If

                    If isOk Then
                        isOk = dl.postCustomer2("PILOT", cus, isOk, transId, msg)
                    End If

                    If isOk = False Then
                        result.customers.isOk = False
                        failedRec = New failedRecord
                        failedRec.id = cus.custId
                        failedRec.msg = msg
                        result.customers.failed.Add(failedRec)
                    End If
                Next
            End If
        Catch ex As Exception
            result.customers.isOk = False
        End Try

        'custContacts
        res = New postResult
        res.isOk = True
        failed = New List(Of failedRecord)
        res.failed = failed
        result.custContacts = res

        Try
            If Not IsNothing(data.custContacts) Then
                Dim con As New CustContact2
                For Each con In data.custContacts

                    If IsNothing(con.token) Then
                        con.token = globalToken
                    End If

                    isOk = dl.postCustContact2("PILOT", con, isOk, transId, msg)
                    If isOk = False Then
                        result.custContacts.isOk = False
                        failedRec = New failedRecord
                        failedRec.id = con.contactId
                        failedRec.msg = msg
                        result.custContacts.failed.Add(failedRec)
                    End If
                Next
            End If
        Catch ex As Exception
            result.custContacts.isOk = False
        End Try

        Return result

    End Function

#End Region

#Region "Field Service Module - Basic Information Tables"

    Public Function getMarketingCampaignsList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getMarketingCampaignsList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getMarketingCampaignsList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function getSalesTaxesList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getSalesTaxesList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getSalesTaxesList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function getCustomerTypesList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getCustomerTypesList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustomerTypesList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function getPaymentTermsList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getPaymentTermsList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getPaymentTermsList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function getJobStatusList(ByVal token As String) As List(Of companyIdNameItem) Implements Ipilot.getJobStatusList
        Dim lstItems As New List(Of companyIdNameItem)
        Dim itm As companyIdNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getJobStatusList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New companyIdNameItem
                itm.companyId = drv.item("CompanyUniqueKey")
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function getJobStatusList2(ByVal token As String) As List(Of jobStatusList) Implements Ipilot.getJobStatusList2
        Dim lstItems As New List(Of jobStatusList)
        Dim itm As jobStatusList = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getJobStatusList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New jobStatusList
                itm.companyId = drv.item("CompanyUniqueKey")
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                itm.order = drv.item("Sequence")
                itm.color = drv.item("Color")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function


    Public Function getAssetTypesList(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getAssetTypesList
        Dim lstItems As New List(Of idNameItem)
        Dim itm As idNameItem = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getAssetTypesList(token, msg)
            For Each drv In dtData.DefaultView
                itm = New idNameItem
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

#End Region

#Region "Field Service Module - Work Information - Jobs"

    Public Function getJobs(ByVal token As String) As List(Of Job) Implements Ipilot.getJobs
        Dim lst As New List(Of Job)
        Dim itm As Job = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getJobs(token, "1/1/1900", isOk, msg)
            For Each drv In dtData.DefaultView
                itm = New Job
                itm.jobId = drv.item("jobId")
                itm.jobNumber = drv.item("jobNumber")
                itm.custId = drv.item("custId")
                itm.contactId = drv.item("contactId")
                itm.locationId = drv.item("locationId")
                itm.dueDate = drv.item("dueDate")
                itm.statusId = drv.item("statusId")
                itm.categoryId = drv.item("categoryId")
                itm.priorityId = drv.item("priorityId")
                itm.jobName = drv.item("jobName")
                itm.details = drv.item("details")
                itm.notes = drv.item("notes")
                itm.createdOn = drv.item("createdOn").ToString
                lst.Add(itm)
            Next


        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New Job
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getJobAssets(ByVal token As String) As List(Of JobAsset) Implements Ipilot.getJobAssets
        Dim lst As New List(Of JobAsset)
        Dim itm As JobAsset = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getJobsAssets(token, "1/1/1900", isOk, msg)
            For Each drv In dtData.DefaultView
                itm = New JobAsset
                itm.jobId = drv.item("jobId")
                itm.assetId = drv.item("assetId")
                itm.categoryId = drv.item("categoryId")
                itm.details = drv.item("details")
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New JobAsset
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function postJob(ByVal data As Job) As responseOk Implements Ipilot.postJob
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postJobs("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function postJobAsset(ByVal data As JobAsset) As responseOk Implements Ipilot.postJobAsset
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postJobsAssets("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function postJobStatusLog(ByVal data As JobStatus) As responseOk Implements Ipilot.postJobStatusLog
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postJobsStatusLog("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        Finally
            BLErrorHandling.ErrorCapture("pilot.svc", "postJobsStatusLog", "", "Invoked postJobStatusLog - Token: " & data.token, 0)
        End Try

        Return resp

    End Function

    Public Function postJobNotesLog(ByVal data As JobNote) As responseOk Implements Ipilot.postJobNotesLog
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postJobsNotesLog("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

#End Region

#Region "Field Service Module - Work Information - Jobs 2"

    Public Function getJobs2(ByVal token As String) As List(Of Job2) Implements Ipilot.getJobs2
        Dim lst As New List(Of Job2)
        Dim itm As Job2 = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getJobs2(token, "1/1/1900", isOk, msg)
            For Each drv In dtData.DefaultView
                itm = New Job2
                itm.jobId = drv.item("jobId")
                itm.jobNumber = drv.item("jobNumber")
                itm.custId = drv.item("custId")
                itm.contactId = drv.item("contactId")
                itm.dueDate = drv.item("dueDate")
                itm.statusId = drv.item("statusId")
                itm.priorityId = drv.item("priorityId")
                itm.jobDescription = drv.item("jobName")
                itm.createdOn = drv.item("createdOn").ToString
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New Job2
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function postJob2(ByVal data As Job2) As responseOk Implements Ipilot.postJob2
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postJobs2("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function deleteJob(ByVal data As JobDelete) As responseOk Implements Ipilot.deleteJob
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            If Not IsDate(data.eventDate) Then
                data.eventDate = Date.UtcNow.ToString
            End If
            resp.isOk = dl.deleteJob("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

#End Region

#Region "Field Service Module - Work Information - Jobs 3"

    Public Function getJobs3(ByVal token As String, ByVal isFullSync As String) As List(Of Job3) Implements Ipilot.getJobs3
        Dim lst As New List(Of Job3)
        Dim itm As Job3 = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True
        Dim bIsFullSync As Boolean = False


        'TESTING THE DYNAMIC FIELDS
        '===========================
        Dim df As jobDynamicField
        Dim dfList As New List(Of jobDynamicField)

        'TEXT
        Dim opt As List(Of String)

        df = New jobDynamicField
        df.id = "q2w3e4r"
        df.type = "text"
        df.label = "Second Name"
        df.options = New List(Of String)
        df.defaultVal = "Doe"
        df.helpText = "Enter 2nd name"
        dfList.Add(df)

        'TEXTAREA
        df = New jobDynamicField
        df.id = "drftg6"
        df.type = "textarea"
        df.label = "More descriptions"
        df.options = New List(Of String)
        df.defaultVal = ""
        df.helpText = "Enter more description"
        dfList.Add(df)

        'SELECT
        df = New jobDynamicField
        opt = New List(Of String)
        opt.Add("Broken Pipe")
        opt.Add("Leaking Pipe")
        opt.Add("Other problem")

        df.id = "f5g6fd"
        df.type = "select"
        df.label = "Pick one"
        df.options = opt
        df.defaultVal = ""
        df.helpText = "Select one of the above"
        dfList.Add(df)

        'RADIOLIST
        df = New jobDynamicField
        opt = New List(Of String)
        opt.Add("Broken Pipe")
        opt.Add("Leaking Pipe")
        opt.Add("Other problem")

        df.id = "f5g6fd2"
        df.type = "radiolist"
        df.label = "Pick one"
        df.options = opt
        df.defaultVal = ""
        df.helpText = "Select one of the above"
        dfList.Add(df)

        'CHECKLIST
        df = New jobDynamicField
        opt = New List(Of String)
        opt.Add("Broken Pipe")
        opt.Add("Leaking Pipe")
        opt.Add("Other problem")

        df.id = "f5g6fd3"
        df.type = "checklist"
        df.label = "Pick one"
        df.options = opt
        df.defaultVal = ""
        df.helpText = "Select one of the above"
        dfList.Add(df)

        'PHONE
        df = New jobDynamicField
        opt = New List(Of String)
        df.id = "q2w3e4r1"
        df.type = "phone"
        df.label = "Other phone number"
        'df.options = New List(Of String)
        df.defaultVal = ""
        df.helpText = "Enter phone"
        dfList.Add(df)

        'EMAIL
        df = New jobDynamicField
        opt = New List(Of String)
        df.id = "q2w3e4r2"
        df.type = "email"
        df.label = "Other email address"
        'df.options = New List(Of String)
        df.defaultVal = ""
        df.helpText = "Enter email"
        dfList.Add(df)

        'HYPERLINK to web site
        df = New jobDynamicField
        opt = New List(Of String)
        df.id = "q2w3e4r3"
        df.type = "hyperlink"
        df.label = "Go to google"
        'df.options = New List(Of String)
        df.defaultVal = "https://www.google.com"
        df.helpText = "Enter email"
        dfList.Add(df)

        'HYPERLINK to PDF
        df = New jobDynamicField
        opt = New List(Of String)
        df.id = "q2w3e4r3"
        df.type = "hyperlink"
        df.label = "Click to open PDF"
        'df.options = New List(Of String)
        df.defaultVal = "https://alliance.la.asu.edu/maps/US-NAMES.pdf"
        df.helpText = "Enter email"
        dfList.Add(df)

        'NUMERIC
        df = New jobDynamicField
        df.id = "q2w3e4rnum"
        df.type = "numeric"
        df.label = "Enter number"
        'df.options = New List(Of String)
        df.defaultVal = "10"
        df.helpText = "Enter a numeric value"
        dfList.Add(df)

        'DATETIME
        df = New jobDynamicField
        df.id = "q2w3e4rdat"
        df.type = "datetime"
        df.label = "Select date and time"
        'df.options = New List(Of String)
        df.defaultVal = Date.Now.ToString
        df.helpText = "Enter date and time"
        dfList.Add(df)

        'TITLE
        df = New jobDynamicField
        df.id = "q2w3e4rtit"
        df.type = "title"
        df.label = "This renders as a H1 in html: Bold big text."
        'df.options = New List(Of String)
        df.defaultVal = ""
        'df.helpText = "Enter date and time"
        dfList.Add(df)

        'PARAGRAPH
        df = New jobDynamicField
        df.id = "q2w3e4rpara"
        df.type = "paragraph"
        df.label = "This renders as a <p> in html: Normal text that can cover several lines. It is used to present general text as instructions, a manual, a contract, etc."
        'df.options = New List(Of String)
        'df.defaultVal = ""
        'df.helpText = "Enter date and time"
        dfList.Add(df)


        '============================

        Try
            If isFullSync.ToLower = "true" Then
                bIsFullSync = True
            End If

            dtData = dl.getJobs3(token, bIsFullSync, isOk, msg)
            For Each drv In dtData.DefaultView
                itm = New Job3
                itm.isOk = isOk
                itm.msg = msg
                itm.companyId = drv.item("CompanyUniqueKey")
                itm.userId = drv.item("UserGUID")
                itm.jobId = drv.item("jobId")
                itm.jobNumber = drv.item("jobNumber")
                itm.custName = drv.item("CustomerName")
                itm.custAddress = drv.item("CustomerAddress")
                itm.custContact = drv.item("CustomerContact")
                itm.custPhone = drv.item("CustomerPhone")
                itm.custEmail = drv.item("CustomerEmail")
                itm.custLat = drv.item("Latitude")
                itm.custLng = drv.item("Longitude")
                itm.dueDate = drv.item("dueDate")
                itm.statusId = drv.item("statusId")
                itm.priorityId = drv.item("PriorityID")
                itm.jobDescription = drv.item("JobDescription")
                itm.estDuration = drv.item("DurationMins")

                itm.fie = dfList

                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New Job3
                itm.isOk = isOk 'If no data and isOk = true, that means that there are no records to sync.
                If isOk = True Then
                    itm.msg = "NORECORDSFOUND"
                Else
                    itm.msg = msg
                End If
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getJobsToRemove(ByVal token As String) As List(Of JobToRemove) Implements Ipilot.getJobsToRemove
        Dim lst As New List(Of JobToRemove)
        Dim itm As JobToRemove = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getJobsToRemove(token, isOk, msg)
            For Each drv In dtData.DefaultView
                itm = New JobToRemove
                itm.isOk = isOk
                itm.msg = msg
                itm.jobId = drv.item("jobId")
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New JobToRemove
                itm.isOk = True
                itm.msg = "NORECORDSFOUND"
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Field Service Module - Dynamic Fields"

    Public Function getFieldTypes(ByVal token As String) As List(Of idNameItem) Implements Ipilot.getFieldTypes
        Dim lst As New List(Of idNameItem)

        Try

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getDynamicTemplates(ByVal token As String) As List(Of dynamicTemplate) Implements Ipilot.getDynamicTemplates
        Dim lst As New List(Of dynamicTemplate)

        Try

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveDynamicTemplate(ByVal token As String, ByVal data As dynamicTemplate) As responseOk Implements Ipilot.saveDynamicTemplate
        Dim res As New responseOk

        Try

        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function getDynamicFields(ByVal token As String, ByVal templateId As String) As List(Of dynamicField) Implements Ipilot.getDynamicFields
        Dim lst As New List(Of dynamicField)

        Try

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function saveDynamicField(ByVal token As String, ByVal data As dynamicField) As responseOk Implements Ipilot.saveDynamicField
        Dim res As New responseOk

        Try

        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function deleteDynamicField(ByVal token As String, ByVal templateId As String, ByVal fieldId As String) As responseOk Implements Ipilot.deleteDynamicField
        Dim res As New responseOk

        Try

        Catch ex As Exception

        End Try

        Return res

    End Function

    Public Function saveDynamicAnswers(ByVal data As List(Of dynamicAnswer)) As responseOk Implements Ipilot.saveDynamicAnswers
        Dim resp As New responseOk
        Dim ans As dynamicAnswer
        Dim dl As New DataLayer

        Try
            For Each ans In data
                resp.isOk = dl.saveDynamicAnswer("etField", ans, resp.isOk, resp.transId, resp.msg)
            Next
        Catch ex As Exception

        End Try

        Return resp

    End Function

#End Region

#Region "Field Service Module - Work Information - Customers"

    Public Function getCustomers(ByVal token As String) As List(Of Customer) Implements Ipilot.getCustomers
        Dim lst As New List(Of Customer)
        Dim itm As Customer = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustomers(token, msg)
            For Each drv In dtData.DefaultView
                itm = New Customer
                itm.custId = drv.item("CustId")
                itm.name = drv.item("Name")
                itm.salesTaxId = drv.item("SalesTaxID")
                itm.adCampaignId = drv.item("AdCampaignID")
                itm.customerTypeId = drv.item("CustomerTypeID")
                itm.paymentTermsId = drv.item("PaymentTermsID")
                itm.notes = drv.item("Notes")
                itm.createdOn = drv.item("CreatedOn").ToString
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New Customer
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getCustLocations(ByVal token As String) As List(Of CustLocation) Implements Ipilot.getCustLocations
        Dim lst As New List(Of CustLocation)
        Dim itm As CustLocation = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustLocations(token, msg)
            For Each drv In dtData.DefaultView
                itm = New CustLocation
                itm.custId = drv.item("custId")
                itm.locationId = drv.item("locationId")
                itm.name = drv.item("name")
                itm.address1 = drv.item("address1")
                itm.address2 = drv.item("address2")
                itm.city = drv.item("city")
                itm.stateId = drv.item("stateId")
                itm.postalCode = drv.item("postalCode")
                itm.phone = drv.item("phone")
                itm.createdOn = drv.item("createdOn").ToString
                itm.isOk = True
                itm.msg = ""
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New CustLocation
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getCustContacts(ByVal token As String) As List(Of CustContact) Implements Ipilot.getCustContacts
        Dim lst As New List(Of CustContact)
        Dim itm As CustContact = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustContacts(token, msg)
            For Each drv In dtData.DefaultView
                itm = New CustContact
                itm.custId = drv.item("custId")
                itm.locId = drv.item("locId")
                itm.contactId = drv.item("contactId")
                itm.firstName = drv.item("firstName")
                itm.lastName = drv.item("lastName")
                itm.phone = drv.item("phone")
                itm.altPhone = drv.item("altPhone")
                itm.email = drv.item("email")
                itm.isPointOfContact = drv.item("isPointOfContact")
                itm.comment = drv.item("comment")
                itm.createdOn = drv.item("createdOn").ToString
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New CustContact
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getCustAssets(ByVal token As String) As List(Of CustAsset) Implements Ipilot.getCustAssets
        Dim lst As New List(Of CustAsset)
        Dim itm As CustAsset = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustAssets(token, msg)
            For Each drv In dtData.DefaultView
                itm = New CustAsset
                itm.custId = drv.item("custId")
                itm.locId = drv.item("locationId")
                itm.assetId = drv.item("assetId")
                itm.assetTypeId = drv.item("assetTypeId")
                itm.name = drv.item("name")
                itm.description = drv.item("description")
                itm.manufacturer = drv.item("manufacturer")
                itm.model = drv.item("model")
                itm.serialNumber = drv.item("serialNumber")
                itm.locationArea = drv.item("locationArea")
                itm.locationSubArea = drv.item("locationSubArea")
                itm.locationSpot = drv.item("locationSpot")
                itm.createdOn = drv.item("createdOn").ToString
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New CustAsset
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function postCustomer(ByVal data As Customer) As responseOk Implements Ipilot.postCustomer
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            data.isOk = True
            If data.custId = "" Then
                data.isOk = False
                data.msg = "INVALID_CUSTID"
            End If

            If data.isOk Then
                If data.name = "" Then
                    data.isOk = False
                    data.msg = "INVALID_NAME"
                End If
            End If
            If data.isOk Then
                resp.isOk = dl.postCustomer("PILOT", data, resp.isOk, resp.transId, resp.msg)
            End If
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function postCustLocation(ByVal data As CustLocation) As responseOk Implements Ipilot.postCustLocation
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postCustLocation("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function postCustContact(ByVal data As CustContact) As responseOk Implements Ipilot.postCustContact
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postCustContact("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function postCustAsset(ByVal data As CustAsset) As responseOk Implements Ipilot.postCustAsset
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postCustAsset("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

#End Region

#Region "Field Service Module - Work Information - Customers 2"

    Public Function getCustomers2(ByVal token As String) As List(Of Customer2) Implements Ipilot.getCustomers2
        Dim lst As New List(Of Customer2)
        Dim itm As Customer2 = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustomers(token, msg)
            For Each drv In dtData.DefaultView
                itm = New Customer2
                itm.custId = drv.item("CustId")
                itm.name = drv.item("Name")
                itm.street = drv.item("Street")
                itm.city = drv.item("city")
                itm.state = drv.item("state")
                itm.postalCode = drv.item("postalCode")
                itm.country = drv.item("country")
                itm.notes = drv.item("Notes")
                itm.createdOn = drv.item("CreatedOn").ToString
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New Customer2
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getCustContacts2(ByVal token As String) As List(Of CustContact2) Implements Ipilot.getCustContacts2
        Dim lst As New List(Of CustContact2)
        Dim itm As CustContact2 = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable

        Try
            dtData = dl.getCustContacts(token, msg)
            For Each drv In dtData.DefaultView
                itm = New CustContact2
                itm.custId = drv.item("custId")
                itm.contactId = drv.item("contactId")
                itm.firstName = drv.item("firstName")
                itm.lastName = drv.item("lastName")
                itm.phone = drv.item("phone")
                itm.email = drv.item("email")
                itm.isPrimary = drv.item("isPointOfContact")
                itm.createdOn = drv.item("createdOn").ToString
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New CustContact2
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function postCustomer2(ByVal data As Customer2) As responseOk Implements Ipilot.postCustomer2
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            data.isOk = True
            If data.custId = "" Then
                data.isOk = False
                data.msg = "INVALID_CUSTID"
            End If

            If data.isOk Then
                If data.name = "" Then
                    data.isOk = False
                    data.msg = "INVALID_NAME"
                End If
            End If
            If data.isOk Then
                resp.isOk = dl.postCustomer2("PILOT", data, resp.isOk, resp.transId, resp.msg)
            End If
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Public Function postCustContact2(ByVal data As CustContact2) As responseOk Implements Ipilot.postCustContact2
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postCustContact2("PILOT", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

#End Region

#Region "Upload Image"

    'Public Function postImage(ByVal jobId As String, ByVal imgType As String, ByVal fileType As String, ByVal data As String) As responseOk Implements Ipilot.postImage
    Public Function postImage(ByVal data As imgData) As responseOk Implements Ipilot.postImage
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Dim eventDate As DateTime
        Dim decLat As Decimal = 0
        Dim decLng As Decimal = 0
        Dim intImgType As Integer = 0

        Try
            'Convert Base64 String to byte()
            Dim imageBytes As Byte()
            imageBytes = Convert.FromBase64String(data.imgData)

            'Convert byte() to image
            'Dim ms As New MemoryStream(imageBytes, 0, imageBytes.Length)
            'ms.Write(imageBytes, 0, imageBytes.Length)
            'Dim img As System.Drawing.Image
            'img = System.Drawing.Image.FromStream(ms, True)
            'img.Save("c:\temp\test.png")


            If IsNumeric(data.imgType) Then
                intImgType = data.imgType
            Else
                intImgType = 1 '1: Picture, 2: Signature
            End If
            If IsDate(data.eventDate) Then
                eventDate = CDate(data.eventDate)
            Else
                eventDate = Date.UtcNow
            End If

            If IsNumeric(data.lat) Then
                decLat = CDec(data.lat)
            End If
            If IsNumeric(data.lng) Then
                decLng = CDec(data.lng)
            End If

            resp.isOk = dl.postImage("PILOT", data.token, data.jobId, intImgType, data.imgId, data.imgName, data.fileName, data.fileType, eventDate, decLat, decLng, imageBytes, resp.isOk, resp.transId, resp.msg)

        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try

        Return resp

    End Function
    Public Function getImages(ByVal token As String) As List(Of imgData) Implements Ipilot.getImages
        Dim lst As New List(Of imgData)
        Dim itm As imgData = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getImages(token, "1/1/1900", isOk, msg)
            For Each drv In dtData.DefaultView
                itm = New imgData
                itm.isOk = True
                itm.jobId = drv.item("jobId")
                itm.imageId = drv.item("imageId") 'This is the ID assigned by the server
                itm.imgId = drv.item("imgId") 'This is the ID assigned by the device
                itm.imgName = drv.item("imgName")
                itm.imgType = drv.item("ImageType")
                itm.fileName = drv.item("fileName")
                itm.fileType = drv.item("fileType")
                itm.eventDate = drv.item("eventDate")
                itm.imgData = "[Binary content]"
                itm.lat = drv.item("lat")
                itm.lng = drv.item("lng")
                lst.Add(itm)
            Next

        Catch ex As Exception

        Finally
            If lst.Count = 0 Then
                itm = New imgData
                itm.isOk = False
                itm.msg = msg
                lst.Add(itm)
            End If
        End Try

        Return lst

    End Function

    Public Function getImage(ByVal token As String, ByVal id As String) As imgData Implements Ipilot.getImage
        Dim itm As New imgData
        Dim tmpImg As tmpImage = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim isOk As Boolean = True

        Try
            tmpImg = dl.getImage(token, id, isOk, msg)

            If tmpImg.isOk = True Then
                itm.isOk = True
                itm.imageId = tmpImg.imageId
                itm.fileName = tmpImg.fileName
                itm.fileType = tmpImg.fileType
                itm.imgType = tmpImg.imgType
                itm.imgData = Convert.ToBase64String(tmpImg.imgData)
                'itm.imgData = "data:image/" & itm.fileType.ToLower & ";charset=utf-8;base64," & itm.imgData
                itm.imgData = "data:image/" & itm.fileType.ToLower & ";base64," & itm.imgData
            Else
                itm.isOk = False
                itm.msg = "NOTFOUND"
            End If

        Catch ex As Exception
            itm = New imgData
            itm.isOk = False
            itm.msg = msg
        Finally

        End Try

        Return itm

    End Function

#End Region

#Region "Transactions Viewer"

    Public Function getTransactionData(ByVal transId As String, ByVal noCache As String) As String Implements Ipilot.getTransactionData
        Dim transData As String = ""
        Dim dl As New DataLayer

        Try
            transData = dl.getTransactionData(transId)
        Catch ex As Exception

        End Try

        Return transData

    End Function

#End Region

#Region "Alerts API"

    Function getAlerts(ByVal token As String, ByVal isFullSync As String) As List(Of alert) Implements Ipilot.getAlerts
        Dim lst As New List(Of alert)
        Dim dl As New DataLayer
        Dim bIsFullSync As Boolean = False

        Try
            If isFullSync.ToLower = "true" Or isFullSync = "1" Then
                bIsFullSync = True
            End If
            lst = dl.getAlerts(token, bIsFullSync)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "Notifications API"

    Function postRegistrationId(ByVal data As registrationId) As responseOk Implements Ipilot.postRegistrationId
        Dim res As New responseOk
        Dim dl As New DataLayer

        Try
            res = dl.postRegistrationId(data)
        Catch ex As Exception

        End Try

        Return res

    End Function

    Function getNotificationTopics(ByVal token As String, ByVal isFullSync As String) As List(Of notificationTopic) Implements Ipilot.getNotificationTopics
        Dim lst As New List(Of notificationTopic)
        Dim dl As New DataLayer
        Dim bIsFullSync As Boolean = False

        Try
            If isFullSync.ToLower = "true" Or isFullSync = "1" Then
                bIsFullSync = True
            End If

            lst = dl.getNotificationTopics(token, bIsFullSync)
        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

End Class
