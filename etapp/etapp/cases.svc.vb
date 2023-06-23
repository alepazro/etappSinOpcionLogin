' NOTE: You can use the "Rename" command on the context menu to change the class name "cases" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select cases.svc or cases.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

Public Class cases
    Implements Icases

    Function companiesGET(ByVal token As String, ByVal noCache As String) As List(Of idName) Implements Icases.companiesGET
        Dim lst As New List(Of idName)
        Dim itm As idName
        Dim dl As New crmDataLayer
        Dim dvData As DataView

        Try
            dvData = dl.companiesGetSimpleList(token)
            For Each drv In dvData
                itm = New idName
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.Add(itm)
            Next
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function devicesGET(ByVal token As String, ByVal noCache As String, ByVal companyId As String) As List(Of caseDevice) Implements Icases.devicesGET
        Dim lst As New List(Of caseDevice)
        Dim itm As caseDevice
        Dim dl As New crmDataLayer
        Dim dvData As DataView

        Try
            dvData = dl.devicesGetSimpleList(token, companyId)
            For Each drv In dvData
                itm = New caseDevice
                itm.deviceId = drv.item("DeviceID")
                itm.name = drv.item("Name")
                lst.Add(itm)
            Next
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function techniciansGET(ByVal token As String, ByVal noCache As String) As List(Of idName) Implements Icases.techniciansGET
        Dim lst As New List(Of idName)
        Dim itm As idName
        Dim dl As New crmDataLayer
        Dim dvData As DataView

        Try
            dvData = dl.techniciansGetSimpleList(token)
            For Each drv In dvData
                itm = New idName
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.Add(itm)
            Next
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function casesBasicTablesGET(ByVal token As String, ByVal noCache As String) As crmBasicTables Implements Icases.casesBasicTablesGET
        Dim lst As New crmBasicTables
        Dim itm As idName
        Dim statusItm As statusItem
        Dim dl As New crmDataLayer
        Dim ds As New DataSet

        Try
            ds = dl.CRM_Cases_GetBasicTables()

            'We are expecting the following tables:
            'CasesCategories
            'CasesTypes
            'CasesSubTypes
            'CasesActivityTypes
            'CasesStatus

            lst.casesCategories = New List(Of idName)
            For Each drv In ds.Tables("CasesCategories").DefaultView
                itm = New idName
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.casesCategories.Add(itm)
            Next

            lst.casesTypes = New List(Of idName)
            For Each drv In ds.Tables("CasesTypes").DefaultView
                itm = New idName
                itm.parentId = drv.item("ParentID")
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.casesTypes.Add(itm)
            Next

            lst.casesSubTypes = New List(Of idName)
            For Each drv In ds.Tables("CasesSubTypes").DefaultView
                itm = New idName
                itm.parentId = drv.item("ParentID")
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.casesSubTypes.Add(itm)
            Next

            lst.casesActivityTypes = New List(Of idName)
            For Each drv In ds.Tables("CasesActivityTypes").DefaultView
                itm = New idName
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.casesActivityTypes.Add(itm)
            Next

            lst.casesStatus = New List(Of statusItem)
            For Each drv In ds.Tables("CasesStatus").DefaultView
                statusItm = New statusItem
                statusItm.id = drv.item("ID")
                statusItm.name = drv.item("Name")
                statusItm.isClosed = drv.item("IsClosed")
                lst.casesStatus.Add(statusItm)
            Next


        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function casesGET(ByVal token As String, ByVal noCache As String) As List(Of caseClass) Implements Icases.casesGET
        Dim lst As New List(Of caseClass)
        Dim itm As caseClass
        Dim dl As New crmDataLayer
        Dim ds As New DataSet

        Try
            ds = dl.CRM_Cases_GET(token, "", "", "", "", False, True)   
            For Each drv In ds.Tables("Cases").DefaultView
                itm = loadCaseItem(drv)
                lst.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function casesGetFiltered(ByVal token As String, ByVal filters As caseFilters) As List(Of caseClass) Implements Icases.casesGetFiltered
        Dim lst As New List(Of caseClass)
        Dim itm As caseClass
        Dim dl As New crmDataLayer
        Dim ds As New DataSet

        Try
            ds = dl.CRM_Cases_GET(token, "", "", filters.categoryId, filters.assignedToId, filters.onlyMine, filters.onlyOpen)
            For Each drv In ds.Tables("Cases").DefaultView
                itm = loadCaseItem(drv)
                lst.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function caseGET(ByVal token As String, ByVal noCache As String, ByVal id As String) As caseClass Implements Icases.caseGET
        Dim itm As New caseClass

        Try
            itm = loadCase(token, id)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Private Function loadCase(ByVal token As String, ByVal id As String) As caseClass
        Dim dl As New crmDataLayer
        Dim ds As New DataSet
        Dim itm As New caseClass
        Dim act As caseActivity

        Try
            ds = dl.CRM_Cases_GET(token, id, "", "", "", False, False)
            For Each drv In ds.Tables("Cases").DefaultView
                itm = loadCaseItem(drv)
            Next

            itm.activities = New List(Of caseActivity)
            For Each drv In ds.Tables("Activities").DefaultView
                act = New caseActivity
                act.id = drv.item("ID")
                act.activityTypeId = drv.item("ActivityTypeID")
                act.activityTypeName = drv.item("ActivityTypeName")
                act.createdOn = drv.item("CreatedOn")
                act.notes = drv.item("Notes")
                act.createdById = drv.item("CreatedByID")
                act.createdByName = drv.item("CreatedByName")
                itm.activities.Add(act)
            Next

        Catch ex As Exception

        End Try

        Return itm

    End Function

    Private Function loadCaseItem(ByVal drv As DataRowView) As caseClass
        Dim itm As New caseClass

        Try
            itm.id = drv.Item("ID")
            itm.subject = drv.Item("Subject")
            itm.companyId = drv.Item("CompanyID")
            itm.companyName = drv.Item("CompanyName")
            itm.companyIsSuspended = drv.Item("CompanyIsSuspended")
            itm.deviceId = drv.Item("DeviceID")
            itm.deviceName = drv.Item("DeviceName")
            itm.categoryId = drv.Item("CategoryID")
            itm.categoryName = drv.Item("CategoryName")
            itm.typeId = drv.Item("TypeID")
            itm.typeName = drv.Item("TypeName")
            itm.subTypeId = drv.Item("SubTypeID")
            itm.subTypeName = drv.Item("SubTypeName")
            itm.assignedToId = drv.Item("AssignedToID")
            itm.assignedToName = drv.Item("AssignedToName")
            itm.assignedOn = drv.Item("AssignedOn").ToString
            itm.statusId = drv.Item("StatusID")
            itm.statusName = drv.Item("StatusName")
            itm.isClosed = drv.Item("IsClosed")
            itm.closedOn = drv.Item("ClosedOn").ToString
            itm.createdOn = drv.Item("CreatedOn").ToString
            itm.lastUpdatedOn = drv.Item("LastUpdatedOn").ToString
            itm.notes = drv.Item("Notes")

        Catch ex As Exception

        End Try

        Return itm

    End Function

    Function casePOST(ByVal token As String, ByVal data As caseClass) As caseClass Implements Icases.casePOST
        Dim res As New caseClass
        Dim dl As New crmDataLayer
        Dim caseId As String = ""

        Try
            res.isOk = dl.CRM_Case_SAVE(token, data, caseId)

            If res.isOk = True Then
                res = loadCase(token, caseId)
            End If

        Catch ex As Exception

        End Try

        Return res

    End Function

    Function caseActivityPOST(ByVal token As String, ByVal data As caseActivity) As caseActivity Implements Icases.caseActivityPOST
        Dim res As New caseActivity
        Dim dl As New crmDataLayer

        Try

        Catch ex As Exception

        End Try

        Return res

    End Function

    Function changeStatusPOST(ByVal token As String, ByVal data As changeStatus) As changeStatusResponse Implements Icases.changeStatusPOST
        Dim res As New changeStatusResponse
        Dim dl As New crmDataLayer

        Try
            res.isOk = dl.CRM_Case_ChangeStatus(token, data, res.statusId)

        Catch ex As Exception

        End Try

        Return res

    End Function

#Region "OnBoarding"

    Function onboardingPendingCustomers(ByVal token As String, ByVal noCache As String) As List(Of pendingOnBoarding) Implements Icases.onboardingPendingCustomers
        Dim dl As New DataLayer
        Dim lst As New List(Of pendingOnBoarding)

        Try
            lst = dl.onboardingPendingCustomers(token)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function onboardingPendingCustomerGet(ByVal token As String, ByVal noCache As String, ByVal id As String) As onBoardingCustomer Implements Icases.onboardingPendingCustomerGet
        Dim itm As New onBoardingCustomer
        Dim dl As New DataLayer

        Try
            itm = dl.onboardingPendingCustomerGet(token, id)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Function onBoardingDone(ByVal token As String, ByVal id As String, ByVal data As onBoardingCustomer) As resultOk Implements Icases.onBoardingDone
        Dim res As New resultOk
        Dim dl As New DataLayer

        Try
            res.isOk = dl.onBoardingDone(token, id, res.msg)

        Catch ex As Exception

        End Try

        Return res

    End Function

#End Region

#Region "Online Support"

    Function getOnlineUsers(ByVal token As String, ByVal noCache As String) As realTimeActivity Implements Icases.getOnlineUsers
        Dim rta As New realTimeActivity
        Dim dl As New DataLayer

        Try
            rta = dl.getOnlineUsers(token)
        Catch ex As Exception

        End Try

        Return rta

    End Function

    Function getUserProfile(ByVal token As String, ByVal noCache As String, ByVal id As String) As userProfile Implements Icases.getUserProfile
        Dim up As New userProfile

        Try

            up.companyName = "ACME, INC"
            up.createdOn = "2/13/2014"

        Catch ex As Exception

        End Try

        Return up

    End Function

#End Region

End Class
