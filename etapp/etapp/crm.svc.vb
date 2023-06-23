' NOTE: You can use the "Rename" command on the context menu to change the class name "crm" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select crm.svc or crm.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

Public Class crm
    Implements Icrm

    Function validateToken(ByVal token As String, ByVal noCache As String) As crmEntities.user Implements Icrm.validateToken
        Dim itm As New crmEntities.user
        Dim dl As New crmDataLayer

        Try
            itm = dl.validateToken(token)
        Catch ex As Exception

        End Try

        Return itm

    End Function

    Function customersGET(ByVal token As String, ByVal noCache As String, ByVal search As String) As List(Of crmEntities.customer) Implements Icrm.customersGET
        Dim dl As New crmDataLayer
        Dim lst As New List(Of crmEntities.customer)

        Try
            lst = dl.customersGET(token, search)
        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function reportsBasicTablesGET(ByVal token As String, ByVal noCache As String) As crmBasicTables Implements Icrm.reportsBasicTablesGET
        Dim lst As New crmBasicTables
        Dim itm As idName
        Dim dl As New crmDataLayer
        Dim ds As New DataSet

        Try
            ds = dl.CRM_Reports_GetBasicTables()

            'We are expecting the following tables:
            'CrmReports

            lst.crmReports = New List(Of idName)
            For Each drv In ds.Tables("CRMReports").DefaultView
                itm = New idName
                itm.id = drv.item("ID")
                itm.name = drv.item("Name")
                lst.crmReports.Add(itm)
            Next

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function getCrmReport(ByVal token As String, ByVal filters As reportsFilters) As String Implements Icrm.getCrmReport
        Dim lst As New List(Of reportClass)
        Dim itm As New reportClass
        Dim json As String = ""
        Dim dt As New DataTable
        Dim dl As New crmDataLayer

        Try
            dt = dl.CRM_GetCRMReport(filters.reportId, filters.param1, filters.userToken)
            json = GetJson(dt)

            itm.content = json

        Catch ex As Exception

        End Try

        Return json

    End Function

    Public Function GetJson(ByVal dt As DataTable) As String
        Dim serializer As New System.Web.Script.Serialization.JavaScriptSerializer()
        Dim rows As New List(Of Dictionary(Of String, Object))()
        Dim row As Dictionary(Of String, Object) = Nothing
        For Each dr As DataRow In dt.Rows
            row = New Dictionary(Of String, Object)()
            For Each dc As DataColumn In dt.Columns
                row.Add(dc.ColumnName.Trim(), dr(dc))
            Next
            rows.Add(row)
        Next
        Return serializer.Serialize(rows)
    End Function


End Class
