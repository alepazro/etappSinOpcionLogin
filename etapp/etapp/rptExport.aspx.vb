Public Class rptExport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim strResult As String = ""
        Dim strReportId As String = ""
        Dim reportName As String = ""

        Try
            If Not IsNothing(Request.QueryString("rId")) Then
                strReportId = Request.QueryString("rId")
            End If

            If strReportId = "16" Then
                strResult = retroGeoReport(reportName)
            ElseIf strReportId = "17" Then
                strResult = activitySummaryReport(reportName)
            Else
                strResult = standardReports(reportName)
            End If

            If strResult.Length = 0 Then
                strResult = "No data found"
            End If

            If reportName.Length = 0 Then
                reportName = "Report"
            End If

            Response.Clear()
            'Response.ContentType = "application/vnd.ms-excel"
            Response.ContentType = "text/csv"
            Response.AddHeader("content-type", "text/csv")
            Response.AddHeader("content-disposition", "attachment;filename=" & reportName & ".csv")
            Response.ContentEncoding = System.Text.Encoding.UTF8
            Response.Write(strResult)
            Response.End()

        Catch ex As Exception

        End Try
    End Sub

    Private Function standardReports(ByRef reportName As String) As String
        Dim strResult As String = ""

        Dim DL As New DataLayer
        Dim BL As New BLCommon
        Dim strError As String = ""
        Dim token As String = ""
        Dim strReportId As String = ""
        Dim deviceId As String = ""
        Dim strDateFrom As String = ""
        Dim strDateTo As String = ""
        Dim strHourFrom As String = ""
        Dim strHourTo As String = ""
        Dim intHourFrom As Integer = 0
        Dim intHourTo As Integer = 24
        Dim param As String = ""
        Dim param2 As String = ""

        Dim dtData As New DataTable

        Try
            If Not IsNothing(Request.QueryString("t")) Then
                token = Request.QueryString("t")
            End If
            If Not IsNothing(Request.QueryString("rId")) Then
                strReportId = Request.QueryString("rId")
            End If
            If Not IsNothing(Request.QueryString("dId")) Then
                deviceId = Request.QueryString("dId")
            End If
            If Not IsNothing(Request.QueryString("df")) Then
                strDateFrom = Request.QueryString("df")
            End If
            If Not IsNothing(Request.QueryString("dt")) Then
                strDateTo = Request.QueryString("dt")
            End If
            If Not IsNothing(Request.QueryString("tf")) Then
                strHourFrom = Request.QueryString("tf")
            End If
            If Not IsNothing(Request.QueryString("tt")) Then
                strHourTo = Request.QueryString("tt")
            End If
            If Not IsNothing(Request.QueryString("p")) Then
                param = Request.QueryString("p")
            End If
            If Not IsNothing(Request.QueryString("p2")) Then
                param2 = Request.QueryString("p2")
            End If
            If Not IsNumeric(param2) Then
                param2 = "0"
            End If

            Dim reportId As Integer = 0
            Dim dateFrom As DateTime
            Dim dateTo As DateTime

            If IsNumeric(strReportId) Then
                reportId = CInt(strReportId)
            End If
            If IsDate(strDateFrom) Then
                dateFrom = CDate(strDateFrom)
                dateFrom = DateAdd(DateInterval.Hour, CInt(strHourFrom), dateFrom)
            Else
                dateFrom = Now.Date
            End If

            If IsDate(strDateTo) Then
                dateTo = CDate(strDateTo)
                dateTo = DateAdd(DateInterval.Hour, CInt(strHourTo), dateTo)
            Else
                dateTo = Now.Date
            End If
            If IsNumeric(strHourFrom) Then
                intHourFrom = CInt(strHourFrom)
            Else
                intHourFrom = 0
            End If
            If IsNumeric(strHourTo) Then
                intHourTo = CInt(strHourTo)
            Else
                intHourTo = 24
            End If

            'token = Request.Form(0)
            If reportId = 17 Then
                strResult = activitySummaryReport(reportName)
            Else
                strResult = DL.runReport(False, token, reportId, deviceId, dateFrom, dateTo, intHourFrom, intHourTo, param, param2, strError, True, dtData)
                strResult = BL.table2CSV(dtData)
            End If

            reportName = dtData.Rows(0).Item("ReportName")
            If reportName.Length = 0 Then
                reportName = "ExportedReport"
            End If

        Catch ex As Exception

        End Try

        Return strResult

    End Function

    Private Function retroGeoReport(ByRef reportName As String) As String
        Dim strResult As String = ""

        Dim DL As New DataLayer
        Dim BL As New BLCommon
        Dim token As String = ""
        Dim address As String = ""
        Dim strLat As String = ""
        Dim strLng As String = ""
        Dim strRadius As String = ""
        Dim strMinTime As String = ""
        Dim strDateFrom As String = ""
        Dim strDateTo As String = ""
        Dim strHourFrom As String = ""
        Dim strHourTo As String = ""
        Dim strError As String = ""

        Dim dtData As New DataTable

        Try
            If Not IsNothing(Request.QueryString("t")) Then
                token = Request.QueryString("t")
            End If
            If Not IsNothing(Request.QueryString("address")) Then
                address = Request.QueryString("address")
            End If
            If Not IsNothing(Request.QueryString("lat")) Then
                strLat = Request.QueryString("lat")
            End If
            If Not IsNothing(Request.QueryString("lng")) Then
                strLng = Request.QueryString("lng")
            End If
            If Not IsNothing(Request.QueryString("radius")) Then
                strRadius = Request.QueryString("radius")
            End If
            If Not IsNothing(Request.QueryString("minTime")) Then
                strMinTime = Request.QueryString("minTime")
            End If
            If Not IsNothing(Request.QueryString("dateFrom")) Then
                strDateFrom = Request.QueryString("dateFrom")
            End If
            If Not IsNothing(Request.QueryString("dateTo")) Then
                strDateTo = Request.QueryString("dateTo")
            End If
            If Not IsNothing(Request.QueryString("hourFrom")) Then
                strHourFrom = Request.QueryString("hourFrom")
            End If
            If Not IsNothing(Request.QueryString("hourTo")) Then
                strHourTo = Request.QueryString("hourTo")
            End If

            Dim lat As Decimal = 0
            If IsNumeric(strLat) Then
                lat = CDec(strLat)
            End If

            Dim lng As Decimal = 0
            If IsNumeric(strLng) Then
                lng = CDec(strLng)
            End If

            Dim radius As Integer = 0
            If IsNumeric(strRadius) Then
                radius = CInt(strRadius)
            End If

            Dim minTime As Integer = 0
            If IsNumeric(strMinTime) Then
                minTime = CInt(strMinTime)
            End If
            minTime = minTime * 60 'Convert to seconds

            Dim dateFrom As Date
            If IsDate(strDateFrom) Then
                dateFrom = CDate(strDateFrom)
            Else
                dateFrom = Date.UtcNow
            End If

            Dim dateTo As Date
            If IsDate(strDateTo) Then
                dateTo = CDate(strDateTo)
            Else
                dateTo = Date.UtcNow
            End If

            Dim hourFrom As Integer = 0
            If IsNumeric(strHourFrom) Then
                hourFrom = CInt(strHourFrom)
            End If
            dateFrom = DateAdd(DateInterval.Hour, hourFrom, dateFrom)

            Dim hourTo As Integer = 0
            If IsNumeric(strHourTo) Then
                hourTo = CInt(strHourTo)
            End If
            dateTo = DateAdd(DateInterval.Hour, hourTo, dateTo)

            'Call the store procedure
            strResult = DL.runReportRetroGeo(token, address, lat, lng, radius, minTime, dateFrom, dateTo, True, strError, dtData)
            strResult = BL.table2CSV(dtData)

            reportName = dtData.Rows(0).Item("ReportName")

        Catch ex As Exception

        End Try

        Return strResult

    End Function

    Private Function activitySummaryReport(ByRef reportName As String) As String
        Dim strResult As String = ""

        Dim DL As New DataLayer
        Dim BL As New BLCommon
        Dim token As String = ""
        Dim strError As String = ""
        Dim strReportId As String = ""
        Dim deviceId As String = ""
        Dim strDateFrom As String = ""
        Dim strDateTo As String = ""
        Dim strHourFrom As String = ""
        Dim strHourTo As String = ""
        Dim intHourFrom As Integer = 0
        Dim intHourTo As Integer = 24
        Dim dtData As New DataTable

        Try
            If Not IsNothing(Request.QueryString("t")) Then
                token = Request.QueryString("t")
            End If
            If Not IsNothing(Request.QueryString("rId")) Then
                strReportId = Request.QueryString("rId")
            End If
            If Not IsNothing(Request.QueryString("dId")) Then
                deviceId = Request.QueryString("dId")
            End If
            If Not IsNothing(Request.QueryString("df")) Then
                strDateFrom = Request.QueryString("df")
            End If
            If Not IsNothing(Request.QueryString("dt")) Then
                strDateTo = Request.QueryString("dt")
            End If
            If Not IsNothing(Request.QueryString("tf")) Then
                strHourFrom = Request.QueryString("tf")
            End If
            If Not IsNothing(Request.QueryString("tt")) Then
                strHourTo = Request.QueryString("tt")
            End If

            Dim reportId As Integer = 0
            Dim dateFrom As DateTime
            Dim dateTo As DateTime

            If IsNumeric(strReportId) Then
                reportId = CInt(strReportId)
            End If
            If IsDate(strDateFrom) Then
                dateFrom = CDate(strDateFrom)
                dateFrom = DateAdd(DateInterval.Hour, CInt(strHourFrom), dateFrom)
            Else
                dateFrom = Now.Date
            End If

            If IsDate(strDateTo) Then
                dateTo = CDate(strDateTo)
                dateTo = DateAdd(DateInterval.Hour, CInt(strHourTo), dateTo)
            Else
                dateTo = Now.Date
            End If
            If IsNumeric(strHourFrom) Then
                intHourFrom = CInt(strHourFrom)
            Else
                intHourFrom = 0
            End If
            If IsNumeric(strHourTo) Then
                intHourTo = CInt(strHourTo)
            Else
                intHourTo = 24
            End If

            dtData = DL.exportActivitySummaryReport(token, deviceId, dateFrom, dateTo, True, strError)
            strResult = BL.table2CSV(dtData)
            reportName = dtData.Rows(0).Item("ReportName")

        Catch ex As Exception

        End Try

        Return strResult

    End Function

End Class