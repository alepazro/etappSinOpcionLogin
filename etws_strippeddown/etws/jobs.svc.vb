' NOTE: You can use the "Rename" command on the context menu to change the class name "jobs" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select jobs.svc or jobs.svc.vb at the Solution Explorer and start debugging.
Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports System.ServiceModel.Activation.WebScriptServiceHostFactory
Imports System.Data
Imports System.Web.Script.Serialization
Imports Newtonsoft.Json

Public Class jobjson
    Implements Ijobs

    'Get job from companies
    Public Function GetJobsNew(ByVal token As String, ByVal noCache As String, ByVal statId As String, ByVal wzId As String, ByVal techId As String, ByVal jobNo As String, ByVal custName As String) As List(Of jobnew) Implements Ijobs.GetJobsNew
        Dim lst As New List(Of jobnew)
        Dim dl As New DataLayer

        Try
            lst = dl.GetJobsNew(token, statId, wzId, techId, jobNo, custName)
        Catch ex As Exception

        End Try

        Return lst

    End Function
    Public Function getJobs(ByVal token As String, ByVal filter As String) As List(Of Job3) Implements Ijobs.getJobs
        Dim lst As New List(Of Job3)
        Dim itm As Job3 = Nothing
        Dim jobFilter As jobFilter
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView

        Try
            jobFilter = Newtonsoft.Json.JsonConvert.DeserializeObject(Of jobFilter)(filter)
        Catch ex As Exception
            jobFilter = New jobFilter
        End Try

        Try
            dtData = dl.getJobsForDispatcher(token, _
                                             jobFilter.allActive, _
                                             jobFilter.jobNumber, _
                                             jobFilter.customerId, _
                                             jobFilter.assignedToId, _
                                             jobFilter.priorityId, _
                                             jobFilter.statusId, _
                                             jobFilter.categoryId, _
                                             jobFilter.dueDateFrom, _
                                             jobFilter.dueDateTo)
            dvData = dtData.DefaultView
            For Each drv In dvData
                itm = New Job3
                itm.isOk = True
                itm.msg = ""
                itm.companyId = drv.item("CompanyUniqueKey")
                itm.userId = drv.item("UserGUID")
                itm.userName = drv.item("UserName")
                itm.jobId = drv.item("jobId")
                itm.jobNumber = drv.item("jobNumber")
                itm.customerId = drv.item("CustomerID")
                itm.custName = drv.item("CustomerName")
                itm.custAddress = drv.item("CustomerAddress")
                itm.custContact = drv.item("CustomerContact")
                itm.custPhone = drv.item("CustomerPhone")
                itm.custEmail = drv.item("CustomerEmail")
                itm.custLat = drv.item("Latitude")
                itm.custLng = drv.item("Longitude")
                itm.dueDate = drv.item("dueDate")
                itm.statusId = drv.item("statusId")
                itm.statusName = drv.item("StatusName")
                itm.statusColor = drv.item("StatusColor")
                itm.statusForeColor = drv.item("StatusForeColor")
                itm.priorityId = drv.item("PriorityID")
                itm.priorityName = drv.item("PriorityName")
                itm.categoryId = drv.item("categoryId")
                itm.categoryName = drv.item("categoryName")
                itm.jobDescription = drv.item("JobDescription")
                itm.estDuration = drv.item("DurationMins")
                itm.durationHHMM = drv.item("DurationHHMM")
                lst.Add(itm)
            Next
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getJob(ByVal token As String, ByVal jobId As String) As Jobobject Implements Ijobs.getJob
        Dim itmJson = ""
        Dim itm As Jobobject = Nothing
        Dim dl As New DataLayer

        Try
            itm = dl.getJob(token, jobId)
        Catch ex As Exception
        End Try

        Return itm

    End Function

    Function getCustomersIdName(ByVal token As String, ByVal wzId As String) As List(Of CustomerIdName) Implements Ijobs.getCustomersIdName
        Dim lst As New List(Of CustomerIdName)
        Dim dl As New DataLayer

        Try
            lst = dl.getCustomersIdName(token, wzId)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Public Function getCustomers(ByVal token As String) As List(Of Customer3) Implements Ijobs.getCustomers
        Dim uList As New List(Of Customer3)
        Dim u As Customer3 = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dtData = dl.getCustomers(token, msg)
            If Not IsNothing(dtData) Then
                dvData = dtData.DefaultView
                For Each drv In dvData
                    u = New Customer3
                    u.id = drv.item("CustID")
                    u.name = drv.item("Name")
                    u.street = drv.item("Street")
                    u.city = drv.item("City")
                    u.state = drv.item("state")
                    u.postalCode = drv.item("PostalCode")
                    u.contactName = drv.item("ContactName")
                    u.email = drv.item("ContactEmail")
                    u.phone = drv.item("ContactPhone")
                    uList.Add(u)
                Next
            End If

        Catch ex As Exception

        End Try

        Return uList

    End Function

    Function getJobCustomer(ByVal token As String, ByVal custId As String) As jobCustomer Implements Ijobs.getJobCustomer
        Dim itm As New jobCustomer
        Dim dl As New DataLayer

        Try
            itm = dl.getJobCustomer(token, custId)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Public Function getCompanyUsers(ByVal token As String) As List(Of user2) Implements Ijobs.getCompanyUsers
        Dim uList As New List(Of user2)
        Dim u As user2 = Nothing
        Dim dl As New DataLayer
        Dim dtData As New DataTable
        Dim dvData As DataView
        Dim msg As String = ""

        Try
            dtData = dl.GetCompanyUsersBasicInfo(token)
            If Not IsNothing(dtData) Then
                dvData = dtData.DefaultView
                For Each drv In dvData
                    u = New user2
                    u.firstName = drv.item("FirstName")
                    u.lastName = drv.item("LastName")
                    u.uniqueId = drv.item("ID")
                    uList.Add(u)
                Next
            End If

        Catch ex As Exception

        End Try

        Return uList

    End Function

    Public Function getJobStatusList(ByVal token As String) As List(Of jobStatusList) Implements Ijobs.getJobStatusList
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
                itm.foreColor = drv.item("ForeColor")
                lstItems.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lstItems

    End Function

    Public Function getJobPrioritiesList(ByVal token As String) As List(Of companyIdNameItem) Implements Ijobs.getJobPrioritiesList
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

    Public Function getJobCategoriesList(ByVal token As String) As List(Of idNameItem) Implements Ijobs.getJobCategoriesList
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

    Public Function postCustomer(ByVal token As String, ByVal data As Customer3) As responseOk Implements Ijobs.postCustomer
        Dim resp As New responseOk
        Dim dl As New DataLayer

        Try
            resp.isOk = dl.postCustomer3(token, "ETRACKFIELD", data, resp.isOk, resp.transId, resp.msg)
        Catch ex As Exception

        End Try

        Return resp

    End Function

    Function postJobCustomer(ByVal token As String, ByVal data As jobCustomer) As jobCustomer Implements Ijobs.postJobCustomer
        Dim dl As New DataLayer

        Try
            data = dl.postJobCustomer("ETAPP-JOBSMODULE", token, data)
            data.createdOn = Date.UtcNow.ToString
        Catch ex As Exception

        End Try

        Return data

    End Function

    Public Function postJob(ByVal token As String, ByVal data As Job3) As responseOk Implements Ijobs.postJob
        Dim resp As New responseOk
        Dim dl As New DataLayer
        Dim channelId As Integer = 3 'eTrack Field App

        Try
            resp.isOk = dl.postJobs3(token, "ETRACKFIELD", data, channelId, resp.isOk, resp.transId, resp.msg, resp.docNum)
        Catch ex As Exception
        End Try

        Return resp

    End Function

    Public Function getImage(ByVal token As String, ByVal imageId As String) As imgData Implements Ijobs.getImage
        Dim itm As New imgData
        Dim tmpImg As tmpImage = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim isOk As Boolean = True

        Try
            tmpImg = dl.getImage(token, imageId, isOk, msg)

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
    Public Function getImagesNew(ByVal token As String, ByVal JobUniqueKey As String) As List(Of imgData) Implements Ijobs.getImagesNew
        Dim lst As New List(Of imgData)
        Dim itm As imgData = Nothing
        Dim dl As New DataLayer
        Dim msg As String = ""
        Dim dtData As New DataTable
        Dim isOk As Boolean = True

        Try
            dtData = dl.getImagesNew(token, JobUniqueKey, isOk, msg)
            If (dtData.Rows.Count > 0) Then
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
            End If
        Catch ex As Exception

            'Finally
            '    If lst.Count = 0 Then
            '        itm = New imgData
            '        itm.isOk = False
            '        itm.msg = msg
            '        lst.Add(itm)
            '    End If
        End Try

        Return lst

    End Function
    Public Function postImageNew(ByVal data As imgData) As responseOk Implements Ijobs.postImageNew
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

    Public Function getCountryStates(ByVal token As String) As List(Of idNameItem) Implements Ijobs.getCountryStates
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

    Function GetJobSupportTables(ByVal token As String) As jobSupportTables Implements Ijobs.GetJobSupportTables
        Dim itm As New jobSupportTables
        Dim dl As New DataLayer

        Try
            itm = dl.GetJobSupportTables(token)
            itm.drivers = GetDrivers(token).Cast(Of JsonGetDriver)().ToList()
        Catch ex As Exception

        End Try

        Return itm

    End Function
    Public Function postJobStop(ByVal token As String, ByVal data As JobStop) As responseOk Implements Ijobs.postJobStop

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
    Public Function postJobNew(ByVal token As String, ByVal data As Jobobject) As responseOk Implements Ijobs.postJobNew

        Dim resp As New responseOk
        Dim dl As New DataLayerJobs
        Try
            'data.geo = "https://maps.google.com/?q=" & Math.Round(lat, 5).ToString & "," & Math.Round(lng, 5).ToString
            resp.transId = dl.saveJob(token, data)

            If resp.transId <> "" Then
                resp.isOk = True
            End If
            If data.HasEmail Then
                Dim Services As New ws
                Dim dataemail As New TrakingNumber
                dataemail.DeviceName = data.DeviceName
                dataemail.Device = data.deviceId
                dataemail.SendTo = data.EmailTo
                dataemail.ValidUntil = data.Deliverydatetime
                dataemail.Message = data.job_description
                dataemail.Flat_FromJob = data.Flat_FromJob
                dataemail.JobUniqueKey = resp.transId
                dataemail.Flat_FromBrokerOrder = data.Flat_FromBrokerOrder
                Services.Posttrakingnumber(token, dataemail)
            End If
            If resp.isOk Then
                For Each item In data.jobstoplist
                    item.JobUniqueKey = resp.transId
                    item.DueDate = data.dueDate
                    item.StartOn = data.StartOn
                    item.UpdateFrom = data.UpdateFrom
                    item.StatusID = data.StatusID
                    item.Status = 1
                    dl.addJobStop(token, item)
                Next
                For Each item In data.notes
                    item.jobId = resp.transId
                    dl.addJobNote(token, item)
                Next
            End If
        Catch ex As Exception
            resp.isOk = False
            resp.msg = ex.Message
        End Try
        Return resp

    End Function
    Function GetJobStops(ByVal token As String, ByVal jobUniquekey As String) As List(Of JobStop) Implements Ijobs.GetJobStops
        'Dim json As String = ""
        Dim dl As New DataLayer
        Dim json As New List(Of JobStop)
        Try
            json = dl.Jobs_Stops_Get(token, jobUniquekey)
        Catch ex As Exception
        End Try

        Return json

    End Function
    Function GeocodingLocation_GET(ByVal token As String, ByVal lat As Decimal, ByVal lon As Decimal, ByVal type As String, ByVal fullAddress As String) As String Implements Ijobs.GeocodingLocation_GET
        Dim json As String = ""
        Dim dl As New DataLayerJobs
        Try
            json = dl.GeocodingLocation_GET(token, lat, lon, type, fullAddress)
        Catch ex As Exception
        End Try

        Return json
    End Function
    Function GeocodingLocation_POST(ByVal token As String, ByVal location As Location) As String Implements Ijobs.GeocodingLocation_POST
        Dim json As String = ""
        Dim dl As New DataLayerJobs
        Try
            json = JsonConvert.SerializeObject(dl.GeocodingLocation_POST(token, location))
        Catch ex As Exception
        End Try
        Return json
    End Function
    Function ListStopByCompanies_GET(ByVal token As String) As String Implements Ijobs.ListStopByCompanies_GET
        Dim json As String = ""
        Dim dl As New DataLayerJobs
        Try
            json = JsonConvert.SerializeObject(dl.Jobs_Stops_ListByCompanies_GET(token))
        Catch ex As Exception
        End Try
        Return json
    End Function
    Function GetDrivers(ByVal token As String) As List(Of Object) Implements Ijobs.GetDrivers
        Dim itm As New JsonGetDriver
        Dim listItm As New List(Of Object)
        Dim dl As New DataLayerJobs
        Try
            listItm = dl.GetDrivers(token)
        Catch ex As Exception
        End Try
        Return listItm

    End Function
    Public Function getStatus(ByVal token As String) As List(Of StatusJobs) Implements Ijobs.getStatus
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
    Public Function GetNotes(ByVal token As String, ByVal jobUniquekey As String, uniqueKey As String, action As Byte) As List(Of JobNote) Implements Ijobs.GetNotes
        'Dim json As String = ""
        Dim dl As New DataLayer
        Dim json As New List(Of JobNote)

        Try
            json = dl.Jobs_Notes_Get(token, jobUniquekey, uniqueKey, action)
        Catch ex As Exception
        End Try

        Return json

    End Function
    Public Function postJobNote(ByVal token As String, ByVal data As JobNote) As responseOk Implements Ijobs.postJobNote
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

    Function GetBrokerDetail(ByVal token As String, ByVal jobUniquekey As String) As List(Of BrokerOrder) Implements Ijobs.GetBrokerDetail
        'Dim json As String = ""
        Dim dl As New DataLayer
        Dim json As New List(Of BrokerOrder)
        Try
            Return dl.BrokerDetail(token, jobUniquekey)
        Catch ex As Exception
            Return New List(Of BrokerOrder)
        End Try


    End Function

    Function ResendEmailBrokerOrder(ByVal token As String, ByVal jobUniquekey As String, ByVal emails As String) As responseOk Implements Ijobs.ResendEmailBrokerOrder

        Dim dl As New DataLayer
        Dim resul As New responseOk
        Try
            resul.isOk = dl.BrokerResendEmail(token, jobUniquekey, emails)
        Catch ex As Exception
            resul.isOk = False
        End Try
        Return resul
    End Function


End Class
