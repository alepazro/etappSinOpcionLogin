Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient

Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter
Imports etws

Public Class DataLayer

#Region "Declaratives"

    Private pSysModule As String = "DataLayer.vb"
    Private strCommand As String = ""
    Private conString As String = ""
    Private conSQL As SqlConnection
    Private Command As SqlCommand
    Private adapter As SqlDataAdapter
    Private BL As New BLCommon
    Private strError As String = ""

#End Region

#Region "Authorization and Credentials"

    Public Function ValidateToken(ByVal Token As String, ByVal sourcePage As String, ByVal sourceId As String, ByVal sourceExt As String) As DataView
        Dim dvData As DataView = Nothing
        Dim dtTable As New DataTable

        Try
            strCommand = "CompaniesUsers_ValidateToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parSourcePage As New SqlClient.SqlParameter("@SourcePage", SqlDbType.NVarChar, 50)
            parSourcePage.Direction = ParameterDirection.Input
            parSourcePage.Value = sourcePage
            Command.Parameters.Add(parSourcePage)

            Dim parSourceId As New SqlClient.SqlParameter("@SourceId", SqlDbType.NVarChar, 50)
            parSourceId.Direction = ParameterDirection.Input
            parSourceId.Value = sourceId
            Command.Parameters.Add(parSourceId)

            Dim parSourceExt As New SqlClient.SqlParameter("@SourceExt", SqlDbType.NVarChar, 50)
            parSourceExt.Direction = ParameterDirection.Input
            parSourceExt.Value = sourceExt
            Command.Parameters.Add(parSourceExt)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtTable)
            adapter.Dispose()
            Command.Dispose()
            dvData = dtTable.DefaultView

        Catch ex As Exception
            dvData = Nothing
            BLErrorHandling.ErrorCapture(pSysModule, "ValidateToken", "", ex.Message & " - Token: " & Token, 0)
        Finally
            conSQL.Dispose()
        End Try

        Return dvData

    End Function

    Public Function ValidateCredentials(ByVal login As String, ByVal password As String, ByVal expDays As Integer, ByVal sourceId As String, ByVal sourceExt As String, ByVal latitude As Decimal, ByVal longitude As Decimal, ByRef msg As String) As DataView
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing

        Try
            login = BLCommon.Sanitize(login)
            'password = BL.Sanitize(password)

            strCommand = "CompaniesUsers_ValidateCredentials"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUsername As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 100)
            parUsername.Direction = ParameterDirection.Input
            parUsername.Value = login
            Command.Parameters.Add(parUsername)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Direction = ParameterDirection.Input
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parExpDays As New SqlClient.SqlParameter("@ExpDays", SqlDbType.Int, 4)
            parExpDays.Direction = ParameterDirection.Input
            parExpDays.Value = expDays
            Command.Parameters.Add(parExpDays)

            Dim parSourceId As New SqlClient.SqlParameter("@SourceId", SqlDbType.NVarChar, 50)
            parSourceId.Direction = ParameterDirection.Input
            parSourceId.Value = sourceId
            Command.Parameters.Add(parSourceId)

            Dim parSourceExt As New SqlClient.SqlParameter("@SourceExt", SqlDbType.NVarChar, -1)
            parSourceExt.Direction = ParameterDirection.Input
            parSourceExt.Value = sourceExt
            Command.Parameters.Add(parSourceExt)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = latitude
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = longitude
            Command.Parameters.Add(parLongitude)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
            dvData = dtData.DefaultView

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        Finally
            conSQL.Dispose()
        End Try

        Return dvData

    End Function

    Public Function recoverCredentials(ByVal email As String, ByVal sourceId As String, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            transId = ""
            msg = ""

            strCommand = "CompaniesUsers_RecoverCredentials"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Direction = ParameterDirection.Input
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = sourceId
            Command.Parameters.Add(parSource)

            Dim parResult As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            If CStr(parResult.Value).Length > 0 Then
                transId = CStr(parResult.Value)
                bResult = True
            Else
                transId = ""
                msg = "FAILED"
                bResult = ""
            End If

        Catch ex As Exception
            bResult = False
            msg = "FAILED: " & ex.Message
            transId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "Basic Information Tables"

    Public Function getCountryStates(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CountryStates_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function GetCompanyUsers(ByVal Token As String, ByVal source As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesUsers_GetByAPIToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function GetCompanyUsersBasicInfo(ByVal Token As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesUsers_GetBasicInfo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

#End Region

#Region "Check In/Out"

    Function getCheckInReasons(ByVal token As String, ByVal isFullSync As Boolean) As List(Of checkInReason)
        Dim lst As New List(Of checkInReason)
        Dim itm As checkInReason

        Try
            strCommand = "MOB_Down_CheckInReasons"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIsFullSync As New SqlClient.SqlParameter("@IsFullSync", SqlDbType.Bit)
            parIsFullSync.Value = isFullSync
            Command.Parameters.Add(parIsFullSync)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New checkInReason
                itm.id = reader.Item("ID")
                itm.description = reader.Item("Description")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Function postCheckInLog(ByVal token As String, ByVal itm As checkInEvent) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "CompaniesUsers_CheckInLog_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parId As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parId.Value = itm.id
            Command.Parameters.Add(parId)

            Dim parReasonId As New SqlClient.SqlParameter("@ReasonID", SqlDbType.Int)
            parReasonId.Value = itm.reasonId
            Command.Parameters.Add(parReasonId)

            Dim parComment As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parComment.Value = itm.comments
            Command.Parameters.Add(parComment)

            Dim parIsCheckedIn As New SqlClient.SqlParameter("@IsCheckedIn", SqlDbType.Bit)
            parIsCheckedIn.Value = itm.isCheckedIn
            Command.Parameters.Add(parIsCheckedIn)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(itm.lastCheckInOutChange) Then
                parEventDate.Value = itm.lastCheckInOutChange
            Else
                parEventDate.Value = Date.UtcNow
            End If
            Command.Parameters.Add(parEventDate)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = itm.lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = itm.lng
            Command.Parameters.Add(parLongitude)

            Dim parIsDeleted As New SqlClient.SqlParameter("@IsDeleted", SqlDbType.Bit)
            parIsDeleted.Value = itm.isDeleted
            Command.Parameters.Add(parIsDeleted)

            Dim parDeletedOn As New SqlClient.SqlParameter("@DeletedOn", SqlDbType.DateTime)
            If IsDate(itm.deletedOn) Then
                parDeletedOn.Value = itm.deletedOn
            Else
                parDeletedOn.Value = Date.UtcNow
            End If
            Command.Parameters.Add(parDeletedOn)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parTransId As New SqlClient.SqlParameter("@TransactionID", SqlDbType.NVarChar, 50)
            parTransId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTransId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)
            res.transId = CStr(parTransId.Value)

        Catch ex As Exception
            res.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Public Function updateCheckInStatus(ByVal token As String, ByVal deviceId As String, ByVal odometer As Integer, ByVal isCheckedIn As Boolean, ByVal lat As Decimal, ByVal lng As Decimal, ByVal sourceId As String, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            transId = ""
            msg = ""

            strCommand = "CompaniesUsers_CheckInStatusUPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = sourceId
            Command.Parameters.Add(parSource)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 10)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Int, 4)
            parOdometer.Direction = ParameterDirection.Input
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parIsCheckedIn As New SqlClient.SqlParameter("@IsCheckedIn", SqlDbType.Bit)
            parIsCheckedIn.Direction = ParameterDirection.Input
            parIsCheckedIn.Value = isCheckedIn
            Command.Parameters.Add(parIsCheckedIn)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parTransId As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTransId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTransId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            transId = CStr(parTransId.Value)

        Catch ex As Exception
            bResults = False
            msg = "FAILED: " & ex.Message
            transId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Jobs"

    Public Function GetJobsNew(ByVal token As String, ByVal statusId As String, ByVal workZoneId As String, ByVal assignedToId As String, ByVal jobNo As String, ByVal custName As String) As List(Of jobnew)
        Dim lst As New List(Of jobnew)
        Dim itm As jobnew = Nothing
        Dim reader As SqlDataReader = Nothing
        Dim Duration As String = ""

        Try
            strCommand = "Jobs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Value = statusId
            Command.Parameters.Add(parStatusID)

            Dim parWZID As New SqlClient.SqlParameter("@WorkZoneID", SqlDbType.NVarChar, 50)
            parWZID.Value = workZoneId
            Command.Parameters.Add(parWZID)

            Dim parTechID As New SqlClient.SqlParameter("@AssignedToID", SqlDbType.NVarChar, 50)
            parTechID.Value = assignedToId
            Command.Parameters.Add(parTechID)

            Dim parJobNo As New SqlClient.SqlParameter("@JobNo", SqlDbType.NVarChar, 50)
            parJobNo.Value = jobNo
            Command.Parameters.Add(parJobNo)

            Dim parCustName As New SqlClient.SqlParameter("@CustName", SqlDbType.NVarChar, 50)
            parCustName.Value = custName
            Command.Parameters.Add(parCustName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New jobnew
                itm.id = reader.Item("ID")
                itm.workZoneId = reader.Item("WorkZoneID")
                itm.workZoneName = reader.Item("WorkZoneName")
                itm.jobNumber = reader.Item("JobNumber")
                itm.customerName = reader.Item("CustomerName")
                itm.jobDescription = reader.Item("JobDescription")
                itm.assignedToName = reader.Item("AssignedToName")
                itm.jobStatus = reader.Item("JobStatus")
                itm.priority = reader.Item("Priority")
                itm.createdOn = reader.Item("CreatedOn")
                itm.dueOn = reader.Item("DueOn")
                itm.durationHH = reader.Item("DurationHH")
                itm.durationMM = reader.Item("DurationMM")
                itm.StartOn = reader.Item("StartOn")
                Duration = String.Concat(itm.durationHH.ToString, " Hours ", itm.durationMM.ToString, " Minutes")
                itm.DurationJob = Duration
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function getJobs(ByVal token As String, ByVal jobId As String, ByVal lat As Decimal, ByVal lng As Decimal, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True

            strCommand = "WorkOrders_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 10)
            parJobID.Direction = ParameterDirection.Input
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        Finally
            conSQL.Dispose()
        End Try

        Return dtData

    End Function

    Public Function addJob(ByVal token As String,
                            ByVal jobNumber As String,
                            ByVal custId As String,
                            ByVal custName As String,
                            ByVal custAddress As String,
                            ByVal custPhone As String,
                            ByVal custContact As String,
                            ByVal dueDate As Date,
                            ByVal statusId As String,
                            ByVal jobDetails As String,
                            ByVal notes As String,
                            ByVal lat As Decimal,
                            ByVal lng As Decimal,
                            ByRef newJobId As String,
                            ByRef newCustomerId As String,
                            ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "WorkOrders_eTrackPilot_INSERT"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 10)
            parJobNumber.Direction = ParameterDirection.Input
            parJobNumber.Value = jobNumber
            Command.Parameters.Add(parJobNumber)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 10)
            parCustomerID.Direction = ParameterDirection.Input
            parCustomerID.Value = custId
            Command.Parameters.Add(parCustomerID)

            Dim parCustName As New SqlClient.SqlParameter("@CustName", SqlDbType.NVarChar, 50)
            parCustName.Direction = ParameterDirection.Input
            parCustName.Value = custName
            Command.Parameters.Add(parCustName)

            Dim parCustAddress As New SqlClient.SqlParameter("@CustAddress", SqlDbType.NVarChar, 50)
            parCustAddress.Direction = ParameterDirection.Input
            parCustAddress.Value = custAddress
            Command.Parameters.Add(parCustAddress)

            Dim parCustPhone As New SqlClient.SqlParameter("@CustPhone", SqlDbType.NVarChar, 20)
            parCustPhone.Direction = ParameterDirection.Input
            parCustPhone.Value = custPhone
            Command.Parameters.Add(parCustPhone)

            Dim parCustContact As New SqlClient.SqlParameter("@CustContact", SqlDbType.NVarChar, 20)
            parCustContact.Direction = ParameterDirection.Input
            parCustContact.Value = custContact
            Command.Parameters.Add(parCustContact)

            Dim parDueDate As New SqlClient.SqlParameter("@DueDate", SqlDbType.DateTime)
            parDueDate.Direction = ParameterDirection.Input
            parDueDate.Value = dueDate
            Command.Parameters.Add(parDueDate)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 10)
            parStatusID.Direction = ParameterDirection.Input
            parStatusID.Value = statusId
            Command.Parameters.Add(parStatusID)

            Dim parJobDetails As New SqlClient.SqlParameter("@JobDetails", SqlDbType.NVarChar, 200)
            parJobDetails.Direction = ParameterDirection.Input
            parJobDetails.Value = jobDetails
            Command.Parameters.Add(parJobDetails)

            Dim parNotes As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, 200)
            parNotes.Direction = ParameterDirection.Input
            parNotes.Value = notes
            Command.Parameters.Add(parNotes)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parNewJobID As New SqlClient.SqlParameter("@NewJobID", SqlDbType.VarChar, 10)
            parNewJobID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewJobID)

            Dim parNewCustomerID As New SqlClient.SqlParameter("@NewCustomerID", SqlDbType.VarChar, 10)
            parNewCustomerID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewCustomerID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            newJobId = CStr(parNewJobID.Value)
            newCustomerId = CStr(parNewCustomerID.Value)
            If newJobId.Length > 0 Then
                bResult = True
            End If

        Catch ex As Exception
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function updateJobStatus(ByVal token As String, ByVal jobId As String, ByVal statusId As String, ByVal lat As Decimal, ByVal lng As Decimal, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            strCommand = "WorkOrders_Status_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 10)
            parJobID.Direction = ParameterDirection.Input
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 10)
            parStatusID.Direction = ParameterDirection.Input
            parStatusID.Value = statusId
            Command.Parameters.Add(parStatusID)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function addJobNote(ByVal token As String, ByVal jobId As String, ByVal note As String, ByVal lat As Decimal, ByVal lng As Decimal, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            strCommand = "WorkOrders_Notes_ADD"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 10)
            parJobID.Direction = ParameterDirection.Input
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            Dim parNote As New SqlClient.SqlParameter("@Note", SqlDbType.NVarChar, 200)
            parNote.Direction = ParameterDirection.Input
            parNote.Value = note
            Command.Parameters.Add(parNote)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function removeJob(ByVal token As String, ByVal jobId As String, ByVal lat As Decimal, ByVal lng As Decimal, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            strCommand = "WorkOrders_eTrackPilot_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 10)
            parJobID.Direction = ParameterDirection.Input
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function GetJobSupportTables(ByVal token As String) As jobSupportTables
        Dim st As New jobSupportTables
        Dim itm As selectList = Nothing
        Dim tec As technician = Nothing
        Dim sta As jobStatuses = Nothing
        Dim sa As jobStatusAction = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_GetSupportTables"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            'WORK ZONES
            Dim wzItm As workZonesListItem
            st.workZones = New List(Of workZonesListItem)

            'Always insert a default called unassigned for those customers that are not linked to any WZ.
            wzItm = New workZonesListItem
            wzItm.workZoneId = "0"
            wzItm.name = "Unassigned"
            st.workZones.Add(wzItm)

            Do While reader.Read
                wzItm = New workZonesListItem
                wzItm.workZoneId = reader.Item("ID")
                wzItm.name = reader.Item("Name")
                st.workZones.Add(wzItm)
            Loop

            'PRIORITIES
            reader.NextResult()
            st.priorities = New List(Of selectList)
            Do While reader.Read
                itm = New selectList
                itm.value = reader.Item("ID")
                itm.text = reader.Item("Name")
                st.priorities.Add(itm)
            Loop

            'SPECIALTIES
            reader.NextResult()
            st.specialties = New List(Of selectList)
            Do While reader.Read
                itm = New selectList
                itm.value = reader.Item("ID")
                itm.text = reader.Item("Name")
                st.specialties.Add(itm)
            Loop

            'TECHNICIANS
            reader.NextResult()
            st.technicians = New List(Of technician)
            Do While reader.Read
                tec = New technician
                tec.value = reader.Item("ID")
                tec.text = reader.Item("Name")
                tec.wzId = reader.Item("WorkZoneID")
                st.technicians.Add(tec)
            Loop

            'CATEGORIES
            reader.NextResult()
            st.categories = New List(Of selectList)
            Do While reader.Read
                itm = New selectList
                itm.value = reader.Item("ID")
                itm.text = reader.Item("Name")
                st.categories.Add(itm)
            Loop

            'STATUSES
            reader.NextResult()
            st.statuses = New List(Of jobStatuses)
            Do While reader.Read
                sta = New jobStatuses
                sta.value = reader.Item("ID")
                sta.text = reader.Item("Name")
                'sta.qty = reader.Item("Quantity")
                'sta.overdue = reader.Item("Overdue")
                st.statuses.Add(sta)
            Loop

            'STATUS ACTIONS
            reader.NextResult()
            st.statusActions = New List(Of jobStatusAction)
            Do While reader.Read
                sa = New jobStatusAction
                sa.statusId = reader.Item("StatusID")
                sa.statusName = reader.Item("StatusName")
                sa.actionId = reader.Item("ActionID")
                sa.actionName = reader.Item("ActionName")
                sa.targetStatusId = reader.Item("TargetStatusID")
                st.statusActions.Add(sa)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return st

    End Function

    Public Function addJobStop(ByVal token As String, ByVal data As JobStop) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "Jobs_Stops_Insert_PostV2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim paruniqueKey As New SqlClient.SqlParameter("@UniqueKey", SqlDbType.NVarChar, 50)
            paruniqueKey.Direction = ParameterDirection.Input
            paruniqueKey.Value = data.uniqueKey
            Command.Parameters.Add(paruniqueKey)

            Dim parJobuniqueKey As New SqlClient.SqlParameter("@JobuniqueKey", SqlDbType.NVarChar, 50)
            parJobuniqueKey.Direction = ParameterDirection.Input
            parJobuniqueKey.Value = data.JobUniqueKey
            Command.Parameters.Add(parJobuniqueKey)

            Dim parStartOn As New SqlClient.SqlParameter("@StartOn", SqlDbType.DateTime)
            parStartOn.Direction = ParameterDirection.Input
            parStartOn.Value = data.StartOn
            Command.Parameters.Add(parStartOn)

            Dim parDueDate As New SqlClient.SqlParameter("@DueDate", SqlDbType.DateTime)
            parDueDate.Direction = ParameterDirection.Input
            parDueDate.Value = data.DueDate
            Command.Parameters.Add(parDueDate)

            Dim parStatus As New SqlClient.SqlParameter("@StatusID", SqlDbType.Int)
            parStatus.Direction = ParameterDirection.Input
            If data.Status Then
                parStatus.Value = 1
            Else
                parStatus.Value = 0
            End If

            Command.Parameters.Add(parStatus)

            Dim parUpdateFrom As New SqlClient.SqlParameter("@UpdateFrom", SqlDbType.Int)
            parUpdateFrom.Direction = ParameterDirection.Input
            parUpdateFrom.Value = data.UpdateFrom
            Command.Parameters.Add(parUpdateFrom)

            Dim parStopOrderNumber As New SqlClient.SqlParameter("@StopOrderNumber", SqlDbType.Int)
            parStopOrderNumber.Direction = ParameterDirection.Input
            parStopOrderNumber.Value = data.StopNumber
            Command.Parameters.Add(parStopOrderNumber)

            Dim parDescripction As New SqlClient.SqlParameter("@Descripction", SqlDbType.VarChar, 255)
            parDescripction.Direction = ParameterDirection.Input
            parDescripction.Value = data.Description
            Command.Parameters.Add(parDescripction)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()


            If parIsOk.Value Then
                bResult = True
            End If

        Catch ex As Exception
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function
    Public Function Jobs_Stops_Get(ByVal token As String, ByVal JobID As String) As List(Of JobStop)

        Dim Jstop As New JobStop
        Dim listJobStops As New List(Of JobStop)

        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_Stops_Get"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIdJob As New SqlClient.SqlParameter("@Ukey", SqlDbType.NVarChar, 50)
            parIdJob.Value = JobID
            Command.Parameters.Add(parIdJob)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                Jstop = New JobStop With {
                    .UniqueKey = reader.Item("UniqueKey"),
                    .Name = reader.Item("Name"),
                    .Street = reader.Item("Street"),
                    .FullAddress = reader.Item("FullAddress"),
                    .City = reader.Item("City"),
                    .State = reader.Item("State"),
                    .Description = reader.Item("Description"),
                    .GeofenceGUID = reader.Item("GeofenceGUID"),
                    .Latitude = reader.Item("Latitude"),
                    .Longitude = reader.Item("Longitude")
                }
                listJobStops.Add(Jstop)

            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return listJobStops

    End Function
    Public Function Jobs_Notes_Get(ByVal token As String, ByVal JobID As String, UniqueKey As String, action As Byte) As List(Of JobNote)

        Dim jobNote As New JobNote
        Dim listJobNotes As New List(Of JobNote)

        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_NotesCrud"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobuniqueKey As New SqlClient.SqlParameter("@JobuniqueKey", SqlDbType.NVarChar, 50)
            parJobuniqueKey.Direction = ParameterDirection.Input
            parJobuniqueKey.Value = JobID
            Command.Parameters.Add(parJobuniqueKey)

            Dim paruniqueKey As New SqlClient.SqlParameter("@uniqueKey", SqlDbType.NVarChar, 50)
            paruniqueKey.Direction = ParameterDirection.Input
            paruniqueKey.Value = UniqueKey
            Command.Parameters.Add(paruniqueKey)

            Dim parAction As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            parAction.Direction = ParameterDirection.Input
            parAction.Value = action
            Command.Parameters.Add(parAction)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                jobNote = New JobNote With {
                    .uniqueKey = reader.Item("uniqueKey"),
                    .note = reader.Item("Note"),
                    .eventDate = reader.Item("EventDate"),
                    .lat = reader.Item("Lat"),
                    .lng = reader.Item("Lng"),
                    .createOn = reader.Item("CreatedOn"),
                    .UpdateFrom = reader.Item("UpdateSource")
                }
                listJobNotes.Add(jobNote)

            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return listJobNotes

    End Function
    Public Function addJobNote(ByVal token As String, ByVal data As JobNote) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "Jobs_NotesCrud"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)


            Dim parJobuniqueKey As New SqlClient.SqlParameter("@JobuniqueKey", SqlDbType.NVarChar, 50)
            parJobuniqueKey.Direction = ParameterDirection.Input
            parJobuniqueKey.Value = data.jobId
            Command.Parameters.Add(parJobuniqueKey)

            Dim paruniqueKey As New SqlClient.SqlParameter("@uniqueKey", SqlDbType.NVarChar, 50)
            paruniqueKey.Direction = ParameterDirection.Input
            paruniqueKey.Value = data.uniqueKey
            Command.Parameters.Add(paruniqueKey)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = data.lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = data.lng
            Command.Parameters.Add(parLongitude)

            Dim parDescription As New SqlClient.SqlParameter("@Description", SqlDbType.NVarChar, 200)
            parDescription.Direction = ParameterDirection.Input
            parDescription.Value = data.note
            Command.Parameters.Add(parDescription)

            Dim parUpdateFrom As New SqlClient.SqlParameter("@UpdateFrom", SqlDbType.TinyInt)
            parUpdateFrom.Direction = ParameterDirection.Input
            parUpdateFrom.Value = data.UpdateFrom
            Command.Parameters.Add(parUpdateFrom)

            Dim parAction As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            parAction.Direction = ParameterDirection.Input
            parAction.Value = data.action
            Command.Parameters.Add(parAction)

            Dim parStatus As New SqlClient.SqlParameter("@Status", SqlDbType.TinyInt)
            parStatus.Direction = ParameterDirection.Input
            parStatus.Value = data.status
            Command.Parameters.Add(parStatus)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()


            If parIsOk.Value Then
                bResult = True
            End If

        Catch ex As Exception
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function
    Public Function addJobStatus(ByVal token As String, ByVal data As JobStatus) As Boolean
        Dim bResult As Boolean = False
        Try
            strCommand = "Jobs_StatusLog_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 50)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = data.sourceID
            Command.Parameters.Add(parSource)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)


            Dim parJobuniqueKey As New SqlClient.SqlParameter("@uniquekeyjob", SqlDbType.NVarChar, 50)
            parJobuniqueKey.Direction = ParameterDirection.Input
            parJobuniqueKey.Value = data.jobId
            Command.Parameters.Add(parJobuniqueKey)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Direction = ParameterDirection.Input
            parStatusID.Value = data.statusId
            Command.Parameters.Add(parStatusID)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = data.lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = data.lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            If parIsOk.Value Then
                bResult = True
            End If

        Catch ex As Exception
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function


#End Region

#Region "My Group Location"

#End Region

#Region "Inspection Lists"

    Public Function getInspectionLists(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Inspections_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getInspectionListItems(ByVal token As String, ByVal inspectionId As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "InspectionsDet_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parInspectionID As New SqlClient.SqlParameter("@InspectionID", SqlDbType.NVarChar, 10)
            parInspectionID.Direction = ParameterDirection.Input
            parInspectionID.Value = inspectionId
            Command.Parameters.Add(parInspectionID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function saveInspection(ByVal token As String,
                                   ByVal sourceId As String,
                                   ByVal DeviceId As String,
                                   ByVal Odometer As Integer,
                                   ByVal eventDate As Date,
                                   ByVal ListId As String,
                                   ByVal ItemId As String,
                                   ByVal Passed As Boolean,
                                   ByVal Failed As Boolean,
                                   ByVal Repaired As Boolean,
                                   ByVal Notes As String,
                                   ByVal Lat As Decimal,
                                   ByVal Lng As Decimal,
                                   ByRef transId As String,
                                   ByRef Msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            transId = ""
            Msg = ""

            strCommand = "InspectionsLog_eTrackPilot_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = sourceId
            Command.Parameters.Add(parSource)

            Dim parDeviceId As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 10)
            parDeviceId.Direction = ParameterDirection.Input
            parDeviceId.Value = DeviceId
            Command.Parameters.Add(parDeviceId)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Int)
            parOdometer.Direction = ParameterDirection.Input
            parOdometer.Value = Odometer
            Command.Parameters.Add(parOdometer)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            parEventDate.Direction = ParameterDirection.Input
            parEventDate.Value = eventDate
            Command.Parameters.Add(parEventDate)

            Dim parListId As New SqlClient.SqlParameter("@ListId", SqlDbType.NVarChar, 10)
            parListId.Direction = ParameterDirection.Input
            parListId.Value = ListId
            Command.Parameters.Add(parListId)

            Dim parItemId As New SqlClient.SqlParameter("@ItemId", SqlDbType.NVarChar, 10)
            parItemId.Direction = ParameterDirection.Input
            parItemId.Value = ItemId
            Command.Parameters.Add(parItemId)

            Dim parPassed As New SqlClient.SqlParameter("@Passed", SqlDbType.Bit)
            parPassed.Direction = ParameterDirection.Input
            parPassed.Value = Passed
            Command.Parameters.Add(parPassed)

            Dim parFailed As New SqlClient.SqlParameter("@Failed", SqlDbType.Bit)
            parFailed.Direction = ParameterDirection.Input
            parFailed.Value = Failed
            Command.Parameters.Add(parFailed)

            Dim parRepaired As New SqlClient.SqlParameter("@Repaired", SqlDbType.Bit)
            parRepaired.Direction = ParameterDirection.Input
            parRepaired.Value = Repaired
            Command.Parameters.Add(parRepaired)

            Dim parNotes As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, 100)
            parNotes.Direction = ParameterDirection.Input
            parNotes.Value = Notes
            Command.Parameters.Add(parNotes)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = Lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = Lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTransId As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTransId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTransId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            Msg = CStr(parMsg.Value)
            transId = CStr(parTransId.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Fuel Purchase"

    Public Function saveFuelPurchase(ByVal token As String, ByVal deviceId As String, ByVal odometer As Decimal, ByVal qty As Decimal, ByVal amount As Decimal, ByVal address As String, ByVal stateId As String, ByVal postalCode As String, ByVal eventDate As Date, ByVal lat As Decimal, ByVal lng As Decimal, ByVal sourceId As String, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            transId = ""
            msg = ""

            strCommand = "Maint_FuelLog_eTrackPilot_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = sourceId
            Command.Parameters.Add(parSource)

            Dim parDeviceId As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 10)
            parDeviceId.Direction = ParameterDirection.Input
            parDeviceId.Value = deviceId
            Command.Parameters.Add(parDeviceId)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Real)
            parOdometer.Direction = ParameterDirection.Input
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parAddress As New SqlClient.SqlParameter("@Address", SqlDbType.NVarChar, 100)
            parAddress.Direction = ParameterDirection.Input
            parAddress.Value = address
            Command.Parameters.Add(parAddress)

            Dim parStateID As New SqlClient.SqlParameter("@StateID", SqlDbType.NVarChar, 10)
            parStateID.Direction = ParameterDirection.Input
            parStateID.Value = stateId
            Command.Parameters.Add(parStateID)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 20)
            parPostalCode.Direction = ParameterDirection.Input
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            parEventDate.Direction = ParameterDirection.Input
            parEventDate.Value = eventDate
            Command.Parameters.Add(parEventDate)

            Dim parQty As New SqlClient.SqlParameter("@Qty", SqlDbType.Real)
            parQty.Direction = ParameterDirection.Input
            parQty.Value = qty
            Command.Parameters.Add(parQty)

            Dim parAmount As New SqlClient.SqlParameter("@Amount", SqlDbType.Real)
            parAmount.Direction = ParameterDirection.Input
            parAmount.Value = amount
            Command.Parameters.Add(parAmount)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResults = False
            msg = "FAILED: " & ex.Message
            transId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function saveFuelPurchase2(ByVal token As String, ByVal licensePlate As String, ByVal odometer As Decimal, ByVal gallons As Decimal, ByVal total As Decimal,
                                      ByVal eventDate As Date, ByVal lat As Decimal, ByVal lng As Decimal, ByVal locType As String, ByVal locAccuracy As Decimal,
                                      ByVal sourceId As String, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            transId = ""
            msg = ""

            strCommand = "Maint_FuelLog_eTrackPilot_SAVE2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSource.Direction = ParameterDirection.Input
            parSource.Value = sourceId
            Command.Parameters.Add(parSource)

            Dim parLicensePlate As New SqlClient.SqlParameter("@LicensePlate", SqlDbType.NVarChar, 10)
            parLicensePlate.Value = licensePlate
            Command.Parameters.Add(parLicensePlate)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Real)
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            parEventDate.Value = eventDate
            Command.Parameters.Add(parEventDate)

            Dim parGallons As New SqlClient.SqlParameter("@Gallons", SqlDbType.Real)
            parGallons.Value = gallons
            Command.Parameters.Add(parGallons)

            Dim parTotal As New SqlClient.SqlParameter("@Total", SqlDbType.Real)
            parTotal.Value = total
            Command.Parameters.Add(parTotal)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parLocType As New SqlClient.SqlParameter("@LocType", SqlDbType.NVarChar, 10)
            parLocType.Value = locType
            Command.Parameters.Add(parLocType)

            Dim parLocAccuracy As New SqlClient.SqlParameter("@LocAccuracy", SqlDbType.Decimal)
            parLocAccuracy.Value = locAccuracy
            Command.Parameters.Add(parLocAccuracy)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResults = False
            msg = "FAILED: " & ex.Message
            transId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Driver Status"

    Public Function getDriverStatusList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "DriverStatus_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function updateDriverLog(ByVal token As String, ByVal deviceId As String,
                                    ByVal driverStatusId As String, ByVal origStartDate As String,
                                    ByVal origEndDate As String, ByVal origDurationMins As String,
                                    ByVal startDate As Date, ByVal endDate As Date, ByVal durationMins As Integer,
                                    ByVal address As String, ByVal stateId As String, ByVal postalCode As String,
                                    ByVal odometer As Integer,
                                    ByVal lat As Decimal, ByVal lng As Decimal,
                                    ByVal sourceId As String, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResults As Boolean = False

        Try
            transId = ""
            msg = ""

            strCommand = "DriverLog_eTrackPilot_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSource.Value = sourceId
            Command.Parameters.Add(parSource)

            Dim parDeviceId As New SqlClient.SqlParameter("@DeviceId", SqlDbType.NVarChar, 10)
            parDeviceId.Value = deviceId
            Command.Parameters.Add(parDeviceId)

            Dim parDriverStatusID As New SqlClient.SqlParameter("@DriverStatusID", SqlDbType.NVarChar, 10)
            parDriverStatusID.Direction = ParameterDirection.Input
            parDriverStatusID.Value = driverStatusId
            Command.Parameters.Add(parDriverStatusID)

            Dim parOrigStartDate As New SqlClient.SqlParameter("@OrigStartDate", SqlDbType.NVarChar, 20)
            parOrigStartDate.Direction = ParameterDirection.Input
            parOrigStartDate.Value = origStartDate
            Command.Parameters.Add(parOrigStartDate)

            Dim parOrigEndDate As New SqlClient.SqlParameter("@OrigEndDate", SqlDbType.NVarChar, 20)
            parOrigEndDate.Direction = ParameterDirection.Input
            parOrigEndDate.Value = origEndDate
            Command.Parameters.Add(parOrigEndDate)

            Dim parOrigDurationMins As New SqlClient.SqlParameter("@OrigDurationMins", SqlDbType.NVarChar, 10)
            parOrigDurationMins.Direction = ParameterDirection.Input
            parOrigDurationMins.Value = origDurationMins
            Command.Parameters.Add(parOrigDurationMins)

            Dim parStartDate As New SqlClient.SqlParameter("@StartDate", SqlDbType.DateTime)
            parStartDate.Direction = ParameterDirection.Input
            parStartDate.Value = startDate
            Command.Parameters.Add(parStartDate)

            Dim parEndDate As New SqlClient.SqlParameter("@EndDate", SqlDbType.DateTime)
            parEndDate.Direction = ParameterDirection.Input
            parEndDate.Value = endDate
            Command.Parameters.Add(parEndDate)

            Dim parDurationMins As New SqlClient.SqlParameter("@DurationMins", SqlDbType.Int)
            parDurationMins.Direction = ParameterDirection.Input
            parDurationMins.Value = durationMins
            Command.Parameters.Add(parDurationMins)

            Dim parAddress As New SqlClient.SqlParameter("@Address", SqlDbType.NVarChar, 100)
            parAddress.Direction = ParameterDirection.Input
            parAddress.Value = address
            Command.Parameters.Add(parAddress)

            Dim parStateID As New SqlClient.SqlParameter("@StateID", SqlDbType.NVarChar, 10)
            parStateID.Direction = ParameterDirection.Input
            parStateID.Value = stateId
            Command.Parameters.Add(parStateID)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 20)
            parPostalCode.Direction = ParameterDirection.Input
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Int, 4)
            parOdometer.Direction = ParameterDirection.Input
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResults = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResults = False
            msg = "FAILED: " & ex.Message
            transId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Field Service Module - GetAll* PostAll* Methods"

    Public Function getAllData(ByVal token As String, ByVal lastSyncOn As Date, ByRef isOk As Boolean, ByRef msg As String) As allDataGet
        Dim myData As New allDataGet
        Dim itm As idNameItem
        Dim dtData As New DataTable
        Dim recType As String = ""

        Try
            strCommand = "etPilot_AllData_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLastSyncOn As New SqlClient.SqlParameter("@LastSyncOn", SqlDbType.DateTime)
            parLastSyncOn.Value = lastSyncOn
            Command.Parameters.Add(parLastSyncOn)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim bFirst As Boolean = True

            myData.jobPriorities = New List(Of idNameItem)
            myData.jobStatuses = New List(Of idNameItem)

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New idNameItem
                recType = reader.Item("recType")
                itm.id = reader.Item("id")
                itm.name = reader.Item("name")

                Select Case recType
                    Case "jobPriorities"
                        myData.jobPriorities.Add(itm)
                    Case "jobStatuses"
                        myData.jobStatuses.Add(itm)
                End Select
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            msg = ex.Message
            isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return myData

    End Function

    Public Function getCustomersAllData(ByVal token As String, ByVal lastSyncOn As Date, ByRef data As allDataGet)
        Dim Reader As SqlDataReader
        Dim cust As Customer2
        Dim custCont As CustContact2
        Dim bResults As Boolean = True

        Try
            strCommand = "etPilot_Customers_AllData_GET_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLastSyncOn As New SqlClient.SqlParameter("@LastSyncOn", SqlDbType.DateTime)
            parLastSyncOn.Value = lastSyncOn
            Command.Parameters.Add(parLastSyncOn)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Reader = Command.ExecuteReader
            'GET CUSTOMERS
            While Reader.Read()
                'Write logic to process data for the first result.
                cust = New Customer2
                cust.custId = Reader.Item("CustId")
                cust.name = Reader.Item("Name")
                cust.street = Reader.Item("Street")
                cust.city = Reader.Item("city")
                cust.state = Reader.Item("state")
                cust.postalCode = Reader.Item("postalCode")
                cust.country = Reader.Item("country")
                cust.notes = Reader.Item("Notes")
                cust.createdOn = Reader.Item("CreatedOn").ToString
                data.customers.Add(cust)
            End While

            'GET CONTACTS
            Reader.NextResult()
            While Reader.Read()
                'Write logic to process data for the second result.
                custCont = New CustContact2
                custCont.custId = Reader.Item("custId")
                custCont.contactId = Reader.Item("contactId")
                custCont.firstName = Reader.Item("firstName")
                custCont.lastName = Reader.Item("lastName")
                custCont.phone = Reader.Item("phone")
                custCont.email = Reader.Item("email")
                custCont.isPrimary = Reader.Item("isPointOfContact")
                custCont.createdOn = Reader.Item("createdOn").ToString
                data.custContacts.Add(custCont)
            End While

            If Not Reader.IsClosed Then
                Reader.Close()
            End If

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return bResults

    End Function

#End Region

#Region "Field Service Module - Basic Information Tables"

    Public Function getMarketingCampaignsList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "MarketingCampaigns_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getSalesTaxesList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "SalesTaxes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getCustomerTypesList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CustomerTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getPaymentTermsList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "PaymentTerms_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getJobStatusList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_JobStatuses_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getJobCategoriesList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_JobCategories_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getJobPrioritiesList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_JobPriorities_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function
    Public Function getJobStatus(ByVal token As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "JobStatuses_GETNEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            'dtData = Nothing
            'If ex.Message = "LOGOUT" Then
            '    msg = "TOKENEXPIRED"
            'Else
            '    msg = "OTHERERROR"
            'End If
        End Try

        Return dtData

    End Function

    Public Function getAssetTypesList(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "AssetTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

#End Region

#Region "Field Service Module - Work Information - Jobs"

    Public Function getJobs(ByVal token As String, ByVal lastSyncOn As Date, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True
            strCommand = "etPilot_Jobs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLastSyncOn As New SqlClient.SqlParameter("@LastSyncOn", SqlDbType.DateTime)
            parLastSyncOn.Value = lastSyncOn
            Command.Parameters.Add(parLastSyncOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try

        Return dtData

    End Function

    Public Function getJobs2(ByVal token As String, ByVal lastSyncOn As Date, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True
            strCommand = "etPilot_Jobs_GET_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLastSyncOn As New SqlClient.SqlParameter("@LastSyncOn", SqlDbType.DateTime)
            parLastSyncOn.Value = lastSyncOn
            Command.Parameters.Add(parLastSyncOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try

        Return dtData

    End Function

    Public Function getJobs3(ByVal token As String, ByVal isFullSync As Boolean, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True
            strCommand = "MOB_Down_Jobs"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIsFullSync As New SqlClient.SqlParameter("@IsFullSync", SqlDbType.Bit)
            parIsFullSync.Value = isFullSync
            Command.Parameters.Add(parIsFullSync)

            Dim parisOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parisOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parisOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.NVarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try

        Return dtData

    End Function

    Public Function getJobsToRemove(ByVal token As String, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True
            strCommand = "MOB_Down_JobsToRemove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parisOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parisOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parisOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.NVarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try

        Return dtData

    End Function

    Public Function getJobsForDispatcher(ByVal token As String,
                                         ByVal allActive As Boolean,
                                         ByVal jobNumber As String,
                                         ByVal customerId As String,
                                         ByVal assignedToId As String,
                                         ByVal priorityId As String,
                                         ByVal statusId As String,
                                         ByVal categoryId As String,
                                         ByVal dueDateFrom As String,
                                         ByVal dueDateTo As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Jobs_GetForDispatcher"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parAllActive As New SqlClient.SqlParameter("@AllActive", SqlDbType.Bit)
            parAllActive.Value = allActive
            Command.Parameters.Add(parAllActive)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 50)
            parJobNumber.Value = jobNumber
            Command.Parameters.Add(parJobNumber)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = customerId
            Command.Parameters.Add(parCustomerID)

            Dim parAssignedTo As New SqlClient.SqlParameter("@AssignedToId", SqlDbType.NVarChar, 50)
            parAssignedTo.Value = assignedToId
            Command.Parameters.Add(parAssignedTo)

            Dim parPrio As New SqlClient.SqlParameter("@PriorityId", SqlDbType.NVarChar, 50)
            parPrio.Value = priorityId
            Command.Parameters.Add(parPrio)

            Dim parStat As New SqlClient.SqlParameter("@StatusId", SqlDbType.NVarChar, 50)
            parStat.Value = statusId
            Command.Parameters.Add(parStat)

            Dim parCat As New SqlClient.SqlParameter("@CategoryId", SqlDbType.NVarChar, 50)
            parCat.Value = categoryId
            Command.Parameters.Add(parCat)

            Dim parDueFrom As New SqlClient.SqlParameter("@DueDateFrom", SqlDbType.NVarChar, 50)
            parDueFrom.Value = dueDateFrom
            Command.Parameters.Add(parDueFrom)

            Dim parDueTo As New SqlClient.SqlParameter("@DueDateTo", SqlDbType.NVarChar, 50)
            parDueTo.Value = dueDateTo
            Command.Parameters.Add(parDueTo)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getJob(ByVal token As String, ByVal jobId As String) As Jobobject 'Job3
        Dim itm As New Jobobject 'Job3

        Try
            strCommand = "Jobs_GetJob_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                'itm.companyId = reader.Item("CompanyUniqueKey")
                'itm.userId = reader.Item("UserGUID")
                'itm.userName = reader.Item("UserName")
                'itm.jobId = reader.Item("jobId")
                itm.UniqueKey = reader.Item("jobId")
                itm.JobNumber = reader.Item("jobNumber")
                itm.driverId = reader.Item("AssignedToID")
                itm.geof_Id = reader.Item("CustomerID")
                itm.geof_name = reader.Item("CustomerName")
                itm.geof_street = reader.Item("CustomerAddress")
                'itm.custContact = reader.Item("CustomerContact")
                itm.geof_phone = reader.Item("CustomerPhone")
                'itm.custEmail = reader.Item("CustomerEmail")
                itm.geof_latitud = reader.Item("Latitude")
                itm.geof_longitud = reader.Item("Longitude")
                itm.dueDate = reader.Item("dueDate")
                itm.StatusID = reader.Item("statusID")
                itm.StatusName = reader.Item("StatusName")
                'itm.statusColor = reader.Item("StatusColor")
                itm.jobpriority = reader.Item("PriorityID")
                'itm.priorityName = reader.Item("PriorityName")
                itm.jobcategories = reader.Item("categoryId")
                'itm.categoryName = reader.Item("categoryName")
                itm.job_description = reader.Item("JobDescription")
                ''itm.estDuration = reader.Item("DurationMins")
                itm.hour = reader.Item("DurationHHMM")
                itm.StartOn = reader.Item("StartOn")
                'itm.CustomerGUID = reader.Item("CustomerGUID")
                'itm.AssignedToID = reader.Item("AssignedToID")
                itm.deviceId = reader.Item("DeviceID")
                itm.geof_street = reader.Item("Street")
                itm.geof_city = reader.Item("City")
                'itm.State = reader.Item("State")
                itm.geof_postalCode = reader.Item("PostalCode")
                itm.RadiusFeet = reader.Item("RadiusFeet")

            Loop

            'GET Notes
            itm.notes = New List(Of JobNote)
            Dim note As JobNote
            reader.NextResult()
            While reader.Read()
                note = New JobNote
                note.jobId = reader.Item("JobID")
                note.uniqueKey = reader.Item("uniqueKey")
                note.note = reader.Item("Note")
                note.lat = reader.Item("Lat")
                note.lng = reader.Item("Lng")
                note.eventDate = reader.Item("EventDate")
                itm.notes.Add(note)
            End While

            'GET Pictures list
            itm.picturesList = New List(Of imgData)
            Dim pic As imgData
            reader.NextResult()
            While reader.Read()
                pic = New imgData
                pic.imageId = reader.Item("imgGuid")
                pic.jobId = reader.Item("JobID")
                pic.imgName = reader.Item("ImgName")
                pic.fileName = reader.Item("Filename")
                pic.fileType = reader.Item("FileType")
                pic.imgType = reader.Item("ImageType")
                pic.eventDate = reader.Item("EventDate")
                pic.UrlImagen = reader.Item("AWS_URL")
                itm.picturesList.Add(pic)
            End While

            'Get Signature
            'Dim signature As imgData
            'reader.NextResult()
            'While reader.Read
            '    signature = New imgData
            '    signature.jobId = reader.Item("JobID")
            '    signature.imageId = reader.Item("ImageID")
            '    signature.imgName = reader.Item("ImgName")
            '    signature.eventDate = reader.Item("EventDate")
            '    signature.lat = reader.Item("Lat")
            '    signature.lng = reader.Item("Lng")
            '    signature.fileName = reader.Item("fileName")
            '    signature.fileType = reader.Item("fileType")
            '    signature.imgData = "" 'Convert.ToBase64String(reader.Item("ImageContent"))
            '    signature.imgData = "" '"data:image/" & reader.Item("fileType").ToLower & ";base64," & signature.imgData
            '    itm.signature = signature
            'End While

            'GET Stops
            itm.jobstoplist = New List(Of JobStop)
            Dim jstop As JobStop
            reader.NextResult()
            While reader.Read
                jstop = New JobStop
                jstop.uniqueKey = reader.Item("UniqueKey")
                jstop.Name = reader.Item("Name")
                jstop.Street = reader.Item("Street")
                jstop.FullAddress = reader.Item("FullAddress")
                jstop.City = reader.Item("City")
                jstop.State = reader.Item("State")
                jstop.Description = reader.Item("Description")
                jstop.GeofenceGUID = reader.Item("GeofenceGUID")
                jstop.Latitude = reader.Item("Latitude")
                jstop.Longitude = reader.Item("Longitude")
                itm.jobstoplist.Add(jstop)
            End While

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            itm.msg = ex.Message
            itm.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return itm

    End Function

    Public Function getJobsAssets(ByVal token As String, ByVal lastSyncOn As Date, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True
            strCommand = "etPilot_JobsAssets_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLastSyncOn As New SqlClient.SqlParameter("@LastSyncOn", SqlDbType.DateTime)
            parLastSyncOn.Value = lastSyncOn
            Command.Parameters.Add(parLastSyncOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try
        Return dtData

    End Function

    Public Function postJobs(ByVal sourceId As String, ByVal data As Job, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etPilot_Jobs_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 50)
            parJobNumber.Value = data.jobNumber
            Command.Parameters.Add(parJobNumber)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parLocationID As New SqlClient.SqlParameter("@LocationID", SqlDbType.NVarChar, 50)
            parLocationID.Value = data.locationId
            Command.Parameters.Add(parLocationID)

            Dim parContactID As New SqlClient.SqlParameter("@ContactID", SqlDbType.NVarChar, 50)
            parContactID.Value = data.contactId
            Command.Parameters.Add(parContactID)

            Dim parDueDate As New SqlClient.SqlParameter("@DueDate", SqlDbType.DateTime)
            parDueDate.Value = data.dueDate
            Command.Parameters.Add(parDueDate)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Value = data.statusId
            Command.Parameters.Add(parStatusID)

            Dim parCategoryID As New SqlClient.SqlParameter("@CategoryID", SqlDbType.NVarChar, 50)
            parCategoryID.Value = data.categoryId
            Command.Parameters.Add(parCategoryID)

            Dim parPriorityID As New SqlClient.SqlParameter("@PriorityID", SqlDbType.NVarChar, 50)
            parPriorityID.Value = data.priorityId
            Command.Parameters.Add(parPriorityID)

            Dim parJobName As New SqlClient.SqlParameter("@JobName", SqlDbType.NVarChar, 250)
            parJobName.Value = data.jobName
            Command.Parameters.Add(parJobName)

            Dim parDetails As New SqlClient.SqlParameter("@Details", SqlDbType.NVarChar, -1)
            parDetails.Value = data.details
            Command.Parameters.Add(parDetails)

            Dim parNotes As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function postJobs2(ByVal sourceId As String, ByVal data As Job2, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etPilot_Jobs_POST_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 50)
            parJobNumber.Value = data.jobNumber
            Command.Parameters.Add(parJobNumber)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parContactID As New SqlClient.SqlParameter("@ContactID", SqlDbType.NVarChar, 50)
            parContactID.Value = data.contactId
            Command.Parameters.Add(parContactID)

            Dim parDueDate As New SqlClient.SqlParameter("@DueDate", SqlDbType.DateTime)
            parDueDate.Value = data.dueDate
            Command.Parameters.Add(parDueDate)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Value = data.statusId
            Command.Parameters.Add(parStatusID)

            Dim parPriorityID As New SqlClient.SqlParameter("@PriorityID", SqlDbType.NVarChar, 50)
            parPriorityID.Value = data.priorityId
            Command.Parameters.Add(parPriorityID)

            Dim parJobName As New SqlClient.SqlParameter("@JobName", SqlDbType.NVarChar, 250)
            parJobName.Value = data.jobDescription
            Command.Parameters.Add(parJobName)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function postJobs3(ByVal token As String, ByVal sourceId As String, ByVal data As Job3, ByVal channelID As Integer, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String, ByRef docNum As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etPilot_Jobs_POST_V3"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 50)
            parJobNumber.Value = data.jobNumber
            Command.Parameters.Add(parJobNumber)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.customerId
            Command.Parameters.Add(parCustomerID)

            Dim parContact As New SqlClient.SqlParameter("@ContactName", SqlDbType.NVarChar, 100)
            parContact.Value = data.custContact
            Command.Parameters.Add(parContact)

            Dim parContactAddress As New SqlClient.SqlParameter("@ContactAddress", SqlDbType.NVarChar, 200)
            parContactAddress.Value = data.custAddress
            Command.Parameters.Add(parContactAddress)

            Dim parContactEmail As New SqlClient.SqlParameter("@ContactEmail", SqlDbType.NVarChar, 100)
            parContactEmail.Value = data.custEmail
            Command.Parameters.Add(parContactEmail)

            Dim parContactPhone As New SqlClient.SqlParameter("@ContactPhone", SqlDbType.NVarChar, 20)
            parContactPhone.Value = data.custPhone
            Command.Parameters.Add(parContactPhone)

            Dim parDueDate As New SqlClient.SqlParameter("@DueDate", SqlDbType.DateTime)
            parDueDate.Value = data.dueDate
            Command.Parameters.Add(parDueDate)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Value = data.statusId
            Command.Parameters.Add(parStatusID)

            Dim parPriorityID As New SqlClient.SqlParameter("@PriorityID", SqlDbType.NVarChar, 50)
            parPriorityID.Value = data.priorityId
            Command.Parameters.Add(parPriorityID)

            Dim parCategoryID As New SqlClient.SqlParameter("@CategoryID", SqlDbType.NVarChar, 50)
            parCategoryID.Value = data.categoryId
            Command.Parameters.Add(parCategoryID)

            Dim parAssignedToID As New SqlClient.SqlParameter("@AssignedToID", SqlDbType.NVarChar, 50)
            parAssignedToID.Value = data.userId
            Command.Parameters.Add(parAssignedToID)

            Dim parDuration As New SqlClient.SqlParameter("@DurationMins", SqlDbType.Int)
            parDuration.Value = data.estDuration
            Command.Parameters.Add(parDuration)

            Dim parChannelID As New SqlClient.SqlParameter("@ChannelID", SqlDbType.Int)
            parChannelID.Value = channelID
            Command.Parameters.Add(parChannelID)

            Dim parJobName As New SqlClient.SqlParameter("@JobDescription", SqlDbType.NVarChar, -1)
            parJobName.Value = data.jobDescription
            Command.Parameters.Add(parJobName)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            Dim parDocNum As New SqlClient.SqlParameter("@DocNum", SqlDbType.VarChar, 20)
            parDocNum.Direction = ParameterDirection.Output
            Command.Parameters.Add(parDocNum)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)
            docNum = CStr(parDocNum.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function postJobsAssets(ByVal sourceId As String, ByVal data As JobAsset, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etPilot_JobsAssets_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parAssetID As New SqlClient.SqlParameter("@AssetID", SqlDbType.NVarChar, 50)
            parAssetID.Value = data.assetId
            Command.Parameters.Add(parAssetID)

            Dim parCategoryID As New SqlClient.SqlParameter("@CategoryID", SqlDbType.NVarChar, 50)
            parCategoryID.Value = data.categoryId
            Command.Parameters.Add(parCategoryID)

            Dim parDetails As New SqlClient.SqlParameter("@Details", SqlDbType.NVarChar, -1)
            parDetails.Value = data.details
            Command.Parameters.Add(parDetails)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function postJobsStatusLog(ByVal sourceId As String, ByVal data As JobStatus, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "MOB_Upl_JobStatusLog"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.NVarChar, 50)
            parStatusID.Value = data.statusId
            Command.Parameters.Add(parStatusID)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(data.eventDate) Then
                parEventDate.Value = CDate(data.eventDate)
            Else
                parEventDate.Value = "1/1/1900"
            End If
            Command.Parameters.Add(parEventDate)

            Dim parOrigEventDate As New SqlClient.SqlParameter("@OrigEventDate", SqlDbType.NVarChar, 50)
            parOrigEventDate.Value = data.eventDate
            Command.Parameters.Add(parOrigEventDate)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            If IsNumeric(data.lat) Then
                parLatitude.Value = CDec(data.lat)
            Else
                parLatitude.Value = 0
            End If
            Command.Parameters.Add(parLatitude)

            Dim parOrigLatitude As New SqlClient.SqlParameter("@OrigLatitude", SqlDbType.NVarChar, 50)
            parOrigLatitude.Value = data.lat
            Command.Parameters.Add(parOrigLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            If IsNumeric(data.lng) Then
                parLongitude.Value = CDec(data.lng)
            Else
                parLongitude.Value = 0
            End If
            Command.Parameters.Add(parLongitude)

            Dim parOrigLongitude As New SqlClient.SqlParameter("@OrigLongitude", SqlDbType.NVarChar, 50)
            parOrigLongitude.Value = data.lng
            Command.Parameters.Add(parOrigLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
            BLErrorHandling.ErrorCapture(pSysModule, "dataLayer.postJobsStatusLog", "", ex.Message & " - Token: " & data.token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function postJobsNotesLog(ByVal sourceId As String, ByVal data As JobNote, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "MOB_Upl_JobNotesLog"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parNoteID As New SqlClient.SqlParameter("@NoteID", SqlDbType.NVarChar, 50)
            parNoteID.Value = data.noteId
            Command.Parameters.Add(parNoteID)

            Dim parNote As New SqlClient.SqlParameter("@Note", SqlDbType.NVarChar, -1)
            parNote.Value = data.note
            Command.Parameters.Add(parNote)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(data.eventDate) Then
                parEventDate.Value = CDate(data.eventDate)
            Else
                parEventDate.Value = "1/1/1900"
            End If
            Command.Parameters.Add(parEventDate)

            Dim parOrigEventDate As New SqlClient.SqlParameter("@OrigEventDate", SqlDbType.NVarChar, 50)
            parOrigEventDate.Value = data.eventDate
            Command.Parameters.Add(parOrigEventDate)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            If IsNumeric(data.lat) Then
                parLatitude.Value = CDec(data.lat)
            Else
                parLatitude.Value = 0
            End If
            Command.Parameters.Add(parLatitude)

            Dim parOrigLatitude As New SqlClient.SqlParameter("@OrigLatitude", SqlDbType.NVarChar, 50)
            parOrigLatitude.Value = data.lat
            Command.Parameters.Add(parOrigLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            If IsNumeric(data.lng) Then
                parLongitude.Value = CDec(data.lng)
            Else
                parLongitude.Value = 0
            End If
            Command.Parameters.Add(parLongitude)

            Dim parOrigLongitude As New SqlClient.SqlParameter("@OrigLongitude", SqlDbType.NVarChar, 50)
            parOrigLongitude.Value = data.lng
            Command.Parameters.Add(parOrigLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function deleteJob(ByVal sourceId As String, ByVal data As JobDelete, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etPilot_Jobs_DELETE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(data.eventDate) Then
                parEventDate.Value = CDate(data.eventDate)
            Else
                parEventDate.Value = "1/1/1900"
            End If
            Command.Parameters.Add(parEventDate)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            If IsNumeric(data.lat) Then
                parLatitude.Value = CDec(data.lat)
            Else
                parLatitude.Value = 0
            End If
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            If IsNumeric(data.lng) Then
                parLongitude.Value = CDec(data.lng)
            Else
                parLongitude.Value = 0
            End If
            Command.Parameters.Add(parLongitude)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "Field Service Module - Work Information - Customers"

    Function getCustomersIdName(ByVal token As String, ByVal wzId As String) As List(Of CustomerIdName)
        Dim lst As New List(Of CustomerIdName)
        Dim itm As CustomerIdName
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_CustomersIdNameGet"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parWZID As New SqlClient.SqlParameter("@WorkZoneID", SqlDbType.NVarChar, 50)
            parWZID.Value = wzId
            Command.Parameters.Add(parWZID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New CustomerIdName
                itm.value = reader.Item("CustID")
                itm.text = reader.Item("Name")
                itm.workZoneId = reader.Item("WorkZoneID")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function getCustomers(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_Customers_GET_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
        End Try

        Return dtData

    End Function

    Function getJobCustomer(ByVal token As String, ByVal custId As String) As jobCustomer
        Dim itm As New jobCustomer
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Jobs_CustomersGet"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New jobCustomer
                itm.id = reader.Item("CustID")
                itm.name = reader.Item("Name")
                itm.street = reader.Item("Street")
                itm.streetNumber = reader.Item("streetNumber")
                itm.route = reader.Item("route")
                itm.city = reader.Item("City")
                itm.county = reader.Item("county")
                itm.state = reader.Item("State")
                itm.postalCode = reader.Item("PostalCode")
                itm.country = reader.Item("Country")
                itm.fullAddress = reader.Item("FullAddress")
                itm.lat = reader.Item("Latitude")
                itm.lng = reader.Item("Longitude")
                itm.contactName = reader.Item("ContactName")
                itm.phone = reader.Item("ContactPhone")
                itm.email = reader.Item("ContactEmail")
                itm.workZoneId = reader.Item("WorkZoneID")
                itm.notes = reader.Item("Notes")
                itm.createdOn = reader.Item("CreatedOn").ToString
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function getCustLocations(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_CustLocations_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
        End Try

        Return dtData

    End Function

    Public Function getCustContacts(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_CustContacts_GET_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
        End Try

        Return dtData

    End Function

    Public Function getCustAssets(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etPilot_CustAssets_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
        End Try

        Return dtData

    End Function

    Public Function postCustomer(ByVal SourceID As String, ByVal data As Customer,
                                 ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean

        Try
            strCommand = "etPilot_Customers_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parCustName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parCustName.Value = data.name
            Command.Parameters.Add(parCustName)

            Dim parSalesTax As New SqlClient.SqlParameter("@SalesTaxID", SqlDbType.NVarChar, 50)
            parSalesTax.Value = data.salesTaxId
            Command.Parameters.Add(parSalesTax)

            Dim parAd As New SqlClient.SqlParameter("@adCampaignId", SqlDbType.NVarChar, 50)
            parAd.Value = data.adCampaignId
            Command.Parameters.Add(parAd)

            Dim parCustomerType As New SqlClient.SqlParameter("@customerTypeId", SqlDbType.NVarChar, 50)
            parCustomerType.Value = data.customerTypeId
            Command.Parameters.Add(parCustomerType)

            Dim parPaymentTerm As New SqlClient.SqlParameter("@paymentTermsId", SqlDbType.NVarChar, 50)
            parPaymentTerm.Value = data.paymentTermsId
            Command.Parameters.Add(parPaymentTerm)

            Dim parNotes As New SqlClient.SqlParameter("@notes", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

    Public Function postCustomer2(ByVal SourceID As String, ByVal data As Customer2,
                                 ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean

        Try
            strCommand = "etPilot_Customers_POST_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parCustName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parCustName.Value = data.name
            Command.Parameters.Add(parCustName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = data.street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Value = data.state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountry As New SqlClient.SqlParameter("@Country", SqlDbType.NVarChar, 50)
            parCountry.Value = data.country
            Command.Parameters.Add(parCountry)

            Dim parNotes As New SqlClient.SqlParameter("@notes", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

    Public Function postCustomer3(ByVal token As String,
                                  ByVal SourceID As String,
                                  ByVal data As Customer3,
                                  ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean

        Try
            strCommand = "etPilot_Customers_POST_V3"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.id
            Command.Parameters.Add(parCustomerID)

            Dim parCustName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parCustName.Value = data.name
            Command.Parameters.Add(parCustName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = data.street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Value = data.state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parLat As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            parLat.Value = data.lat
            Command.Parameters.Add(parLat)

            Dim parLng As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            parLng.Value = data.lng
            Command.Parameters.Add(parLng)

            Dim parContactName As New SqlClient.SqlParameter("@ContactName", SqlDbType.NVarChar, 100)
            parContactName.Value = data.contactName
            Command.Parameters.Add(parContactName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

    Function postJobCustomer(ByVal SourceID As String, ByVal token As String, ByVal data As jobCustomer) As jobCustomer
        Try
            strCommand = "Jobs_CustomersPost"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.id
            Command.Parameters.Add(parCustomerID)

            Dim parCustName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parCustName.Value = data.name
            Command.Parameters.Add(parCustName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = data.street
            Command.Parameters.Add(parStreet)

            Dim parStreetNo As New SqlClient.SqlParameter("@StreetNumber", SqlDbType.NVarChar, 50)
            parStreetNo.Value = data.streetNumber
            Command.Parameters.Add(parStreetNo)

            Dim parRoute As New SqlClient.SqlParameter("@Route", SqlDbType.NVarChar, 50)
            parRoute.Value = data.route
            Command.Parameters.Add(parRoute)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parCounty As New SqlClient.SqlParameter("@County", SqlDbType.NVarChar, 50)
            parCounty.Value = data.county
            Command.Parameters.Add(parCounty)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Value = data.state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountry As New SqlClient.SqlParameter("@Country", SqlDbType.NVarChar, 50)
            parCountry.Value = data.country
            Command.Parameters.Add(parCountry)

            Dim parFullAddress As New SqlClient.SqlParameter("@FullAddress", SqlDbType.NVarChar, 100)
            parFullAddress.Value = data.fullAddress
            Command.Parameters.Add(parFullAddress)

            Dim parLat As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            parLat.Value = data.lat
            Command.Parameters.Add(parLat)

            Dim parLng As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            parLng.Value = data.lng
            Command.Parameters.Add(parLng)

            Dim parContactName As New SqlClient.SqlParameter("@ContactName", SqlDbType.NVarChar, 100)
            parContactName.Value = data.contactName
            Command.Parameters.Add(parContactName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parWZID As New SqlClient.SqlParameter("@WorkZoneID", SqlDbType.NVarChar, 50)
            parWZID.Value = data.workZoneId
            Command.Parameters.Add(parWZID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parUniqueID As New SqlClient.SqlParameter("@UniqueID", SqlDbType.VarChar, 50)
            parUniqueID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parUniqueID)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            data.id = CStr(parUniqueID.Value)
            data.isOk = CBool(parIsOk.Value)
            data.sysMsg = CStr(parMsg.Value)

        Catch ex As Exception
            data.isOk = False
            data.sysMsg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return data

    End Function

    Public Function postCustLocation(ByVal SourceID As String, ByVal data As CustLocation, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Try
            isOk = True
            strCommand = "etPilot_CustLocation_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parLocationID As New SqlClient.SqlParameter("@LocationID", SqlDbType.NVarChar, 50)
            parLocationID.Value = data.locationId
            Command.Parameters.Add(parLocationID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parAddress1 As New SqlClient.SqlParameter("@Address1", SqlDbType.NVarChar, 100)
            parAddress1.Value = data.address1
            Command.Parameters.Add(parAddress1)

            Dim parAddress2 As New SqlClient.SqlParameter("@Address2", SqlDbType.NVarChar, 100)
            parAddress2.Value = data.address2
            Command.Parameters.Add(parAddress2)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parStateID As New SqlClient.SqlParameter("@StateID", SqlDbType.NVarChar, 10)
            parStateID.Value = data.stateId
            Command.Parameters.Add(parStateID)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 20)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

    Public Function postCustContact(ByVal SourceID As String, ByVal data As CustContact, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Try
            isOk = True
            strCommand = "etPilot_CustContact_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parLocID As New SqlClient.SqlParameter("@LocationID", SqlDbType.NVarChar, 50)
            parLocID.Value = data.locId
            Command.Parameters.Add(parLocID)

            Dim parContactID As New SqlClient.SqlParameter("@ContactID", SqlDbType.NVarChar, 50)
            parContactID.Value = data.contactId
            Command.Parameters.Add(parContactID)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = data.firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = data.lastName
            Command.Parameters.Add(parLastName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parAltPhone As New SqlClient.SqlParameter("@AltPhone", SqlDbType.NVarChar, 20)
            parAltPhone.Value = data.altPhone
            Command.Parameters.Add(parAltPhone)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parIsPointOfContact As New SqlClient.SqlParameter("@IsPointOfContact", SqlDbType.Bit)
            If data.isPointOfContact.ToLower = "true" Or data.isPointOfContact = "1" Then
                parIsPointOfContact.Value = True
            Else
                parIsPointOfContact.Value = False
            End If
            Command.Parameters.Add(parIsPointOfContact)

            Dim parComment As New SqlClient.SqlParameter("@Comment", SqlDbType.NVarChar, -1)
            parComment.Value = data.comment
            Command.Parameters.Add(parComment)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

    Public Function postCustContact2(ByVal SourceID As String, ByVal data As CustContact2, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Try
            isOk = True
            strCommand = "etPilot_CustContact_POST_V2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parContactID As New SqlClient.SqlParameter("@ContactID", SqlDbType.NVarChar, 50)
            parContactID.Value = data.contactId
            Command.Parameters.Add(parContactID)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = data.firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = data.lastName
            Command.Parameters.Add(parLastName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parIsPointOfContact As New SqlClient.SqlParameter("@IsPointOfContact", SqlDbType.Bit)
            If data.isPrimary.ToLower = "true" Or data.isPrimary = "1" Then
                parIsPointOfContact.Value = True
            Else
                parIsPointOfContact.Value = False
            End If
            Command.Parameters.Add(parIsPointOfContact)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

    Public Function postCustAsset(ByVal SourceID As String, ByVal data As CustAsset, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Try
            strCommand = "etPilot_CustAsset_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = SourceID
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parCustomerID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustomerID.Value = data.custId
            Command.Parameters.Add(parCustomerID)

            Dim parLocationID As New SqlClient.SqlParameter("@LocationID", SqlDbType.NVarChar, 50)
            parLocationID.Value = data.locId
            Command.Parameters.Add(parLocationID)

            Dim parAssetID As New SqlClient.SqlParameter("@AssetID", SqlDbType.NVarChar, 50)
            parAssetID.Value = data.assetId
            Command.Parameters.Add(parAssetID)

            Dim parAssetTypeID As New SqlClient.SqlParameter("@AssetTypeID", SqlDbType.NVarChar, 50)
            parAssetTypeID.Value = data.assetTypeId
            Command.Parameters.Add(parAssetTypeID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 100)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parDescription As New SqlClient.SqlParameter("@Description", SqlDbType.NVarChar, 500)
            parDescription.Value = data.description
            Command.Parameters.Add(parDescription)

            Dim parManufacturer As New SqlClient.SqlParameter("@Manufacturer", SqlDbType.NVarChar, 50)
            parManufacturer.Value = data.manufacturer
            Command.Parameters.Add(parManufacturer)

            Dim parModel As New SqlClient.SqlParameter("@Model", SqlDbType.NVarChar, 50)
            parModel.Value = data.model
            Command.Parameters.Add(parModel)

            Dim parSN As New SqlClient.SqlParameter("@SerialNumber", SqlDbType.NVarChar, 50)
            parSN.Value = data.serialNumber
            Command.Parameters.Add(parSN)

            Dim parLocArea As New SqlClient.SqlParameter("@LocationArea", SqlDbType.NVarChar, 50)
            parLocArea.Value = data.locationArea
            Command.Parameters.Add(parLocArea)

            Dim parLocSubArea As New SqlClient.SqlParameter("@LocationSubArea", SqlDbType.NVarChar, 50)
            parLocSubArea.Value = data.locationSubArea
            Command.Parameters.Add(parLocSubArea)

            Dim parLocSpot As New SqlClient.SqlParameter("@LocationSpot", SqlDbType.NVarChar, 50)
            parLocSpot.Value = data.locationSpot
            Command.Parameters.Add(parLocSpot)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            isOk = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return isOk

    End Function

#End Region

#Region "Field Service Module - Dynamic Fields"

    Public Function saveDynamicAnswer(ByVal sourceId As String, ByVal data As dynamicAnswer, ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "MOB_Upl_DynamicAnswers"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = data.jobId
            Command.Parameters.Add(parJobID)

            Dim parID As New SqlClient.SqlParameter("@QuestionID", SqlDbType.NVarChar, 50)
            parID.Value = data.fieldId
            Command.Parameters.Add(parID)

            Dim parValue As New SqlClient.SqlParameter("@AnswerValue", SqlDbType.NVarChar, -1)
            parValue.Value = data.answer
            Command.Parameters.Add(parValue)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(data.eventDate) Then
                parEventDate.Value = CDate(data.eventDate)
            Else
                parEventDate.Value = "1/1/1900"
            End If
            Command.Parameters.Add(parEventDate)

            Dim parOrigEventDate As New SqlClient.SqlParameter("@OrigEventDate", SqlDbType.NVarChar, 50)
            parOrigEventDate.Value = data.eventDate
            Command.Parameters.Add(parOrigEventDate)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)

        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "Upload Image"

    Public Function getImages(ByVal token As String, ByVal lastSyncOn As Date, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            isOk = True
            strCommand = "etPilot_JobsImages_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLastSyncOn As New SqlClient.SqlParameter("@LastSyncOn", SqlDbType.DateTime)
            parLastSyncOn.Value = lastSyncOn
            Command.Parameters.Add(parLastSyncOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try

        Return dtData

    End Function
    Public Function getImagesNew(ByVal token As String, ByVal JobUniqueKey As String, ByRef isOk As Boolean, ByRef msg As String) As DataTable
        Dim dtData As New DataTable
        Try
            isOk = True
            strCommand = "etPilot_JobsImages_GET_New"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobUniqueKey As New SqlClient.SqlParameter("@JobUniqueKey", SqlDbType.NVarChar, 100)
            parJobUniqueKey.Direction = ParameterDirection.Input
            parJobUniqueKey.Value = JobUniqueKey
            Command.Parameters.Add(parJobUniqueKey)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            msg = ex.Message
            isOk = False
        End Try

        Return dtData

    End Function

    Public Function getImage(ByVal token As String, ByVal id As String, ByRef isOk As Boolean, ByRef msg As String) As tmpImage
        Dim itm As New tmpImage

        Try
            isOk = True
            strCommand = "etPilot_JobsImage_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ImageID", SqlDbType.VarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            itm.isOk = False

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm.isOk = True
                itm.imageId = reader.Item("imageId")
                itm.imgType = reader.Item("ImageType")
                itm.fileName = reader.Item("fileName")
                itm.fileType = reader.Item("fileType")
                itm.imgData = reader.Item("ImageContent")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            itm.isOk = False
            itm.msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return itm

    End Function







    Public Function postImage(ByVal sourceId As String, ByVal token As String, ByVal jobId As String, ByVal imgType As Integer, ByVal imgId As String, ByVal imgName As String, ByVal fileName As String, ByVal fileType As String, ByVal eventDate As DateTime, ByVal lat As Decimal, ByVal lng As Decimal, ByVal img As Byte(), ByRef isOk As Boolean, ByRef transId As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etPilot_Job_Image_POST"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parSourceID As New SqlClient.SqlParameter("@SourceID", SqlDbType.NVarChar, 10)
            parSourceID.Value = sourceId
            Command.Parameters.Add(parSourceID)

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = jobId
            Command.Parameters.Add(parJobID)

            Dim parImgType As New SqlClient.SqlParameter("@ImgType", SqlDbType.Int)
            parImgType.Value = imgType
            Command.Parameters.Add(parImgType)

            Dim parImgID As New SqlClient.SqlParameter("@ImgID", SqlDbType.NVarChar, 50)
            parImgID.Value = imgId
            Command.Parameters.Add(parImgID)

            Dim parImgName As New SqlClient.SqlParameter("@ImgName", SqlDbType.NVarChar, 100)
            parImgName.Value = imgName
            Command.Parameters.Add(parImgName)

            Dim parFileName As New SqlClient.SqlParameter("@FileName", SqlDbType.NVarChar, 100)
            parFileName.Value = fileName
            Command.Parameters.Add(parFileName)

            Dim parFileType As New SqlClient.SqlParameter("@FileType", SqlDbType.NVarChar, 10)
            parFileType.Value = fileType
            Command.Parameters.Add(parFileType)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(eventDate) Then
                parEventDate.Value = CDate(eventDate)
            Else
                parEventDate.Value = "1/1/1900"
            End If
            Command.Parameters.Add(parEventDate)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            If IsNumeric(lat) Then
                parLatitude.Value = CDec(lat)
            Else
                parLatitude.Value = 0
            End If
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            If IsNumeric(lng) Then
                parLongitude.Value = CDec(lng)
            Else
                parLongitude.Value = 0
            End If
            Command.Parameters.Add(parLongitude)

            Dim parImg As New SqlClient.SqlParameter("@Image", SqlDbType.Image)
            parImg.Value = img
            Command.Parameters.Add(parImg)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parTranID As New SqlClient.SqlParameter("@TransactionID", SqlDbType.VarChar, 50)
            parTranID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTranID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            msg = CStr(parMsg.Value)
            transId = CStr(parTranID.Value)


        Catch ex As Exception
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function
    Public Function postImageNew(ByVal img As imgData) As responseOk
        Dim bResult As responseOk = New responseOk()

        Try
            strCommand = "etPilot_Job_Image_POST_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = img.token
            Command.Parameters.Add(parToken)

            Dim parJobID As New SqlClient.SqlParameter("@JobID", SqlDbType.NVarChar, 50)
            parJobID.Value = img.jobId
            Command.Parameters.Add(parJobID)

            Dim parImgGUID As New SqlClient.SqlParameter("@ImgGUID", SqlDbType.NVarChar, 100)
            parImgGUID.Value = img.GUID
            Command.Parameters.Add(parImgGUID)

            Dim parEventDate As New SqlClient.SqlParameter("@EventDate", SqlDbType.DateTime)
            If IsDate(img.eventDate) Then
                parEventDate.Value = CDate(img.eventDate)
            Else
                parEventDate.Value = "1/1/1900"
            End If
            Command.Parameters.Add(parEventDate)

            Dim parImgName As New SqlClient.SqlParameter("@ImgName", SqlDbType.NVarChar, 100)
            parImgName.Value = img.imgName
            Command.Parameters.Add(parImgName)

            Dim parFileName As New SqlClient.SqlParameter("@FileName", SqlDbType.NVarChar, 100)
            parFileName.Value = img.fileName
            Command.Parameters.Add(parFileName)

            Dim parFileType As New SqlClient.SqlParameter("@FileType", SqlDbType.NVarChar, 10)
            parFileType.Value = img.fileType
            Command.Parameters.Add(parFileType)

            Dim parUrlImagen As New SqlClient.SqlParameter("@UrlImagen", SqlDbType.NVarChar, 255)
            parUrlImagen.Value = img.UrlImagen
            Command.Parameters.Add(parUrlImagen)

            Dim parStatus As New SqlClient.SqlParameter("@Status", SqlDbType.Int)
            parStatus.Value = Integer.Parse(img.Status)
            Command.Parameters.Add(parStatus)

            Dim parUpdateFrom As New SqlClient.SqlParameter("@UpdateFrom", SqlDbType.Int)
            parUpdateFrom.Value = Integer.Parse(img.UpdateFrom)
            Command.Parameters.Add(parUpdateFrom)

            Dim parImgType As New SqlClient.SqlParameter("@imgType", SqlDbType.Int)
            parImgType.Value = Integer.Parse(img.imgType)
            Command.Parameters.Add(parImgType)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 50)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()
            bResult.isOk = CBool(parIsOk.Value)
            bResult.msg = CStr(parMsg.Value)

        Catch ex As Exception
            bResult.isOk = False
            bResult.msg = "Error registering image " + ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "Transaction Viewer"

    Public Function getTransactionData(ByVal transId As String) As String
        Dim transData As String = ""

        Try
            strCommand = "CompaniesUsersActivityLog_GetByTransID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parTransId As New SqlClient.SqlParameter("@TransactionID", SqlDbType.NVarChar, 50)
            parTransId.Direction = ParameterDirection.Input
            parTransId.Value = transId
            Command.Parameters.Add(parTransId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                transData = reader.Item("jsonData")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            transData = "FAILED"
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return transData

    End Function

#End Region

#Region "AWS"

    Public Function blockedEmail_UPDATE(ByVal emailAddress As String, ByVal notificationType As String, ByVal jsonData As String) As Boolean
        Dim bOk As Boolean = True

        Try
            strCommand = "BlockedEmails_AWS_INSERT"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parEmailAddress As New SqlClient.SqlParameter("@EmailAddress", SqlDbType.VarChar, 100)
            parEmailAddress.Value = emailAddress
            Command.Parameters.Add(parEmailAddress)

            Dim parNotificationType As New SqlClient.SqlParameter("@NotificationType", SqlDbType.VarChar, 100)
            parNotificationType.Value = notificationType
            Command.Parameters.Add(parNotificationType)

            Dim parJsonData As New SqlClient.SqlParameter("@JsonData", SqlDbType.VarChar, -1)
            parJsonData.Value = jsonData
            Command.Parameters.Add(parJsonData)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bOk = CBool(parIsOk.Value)

        Catch ex As Exception
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

#End Region

#Region "Users"

    Public Function getUsers_DEPRECATED(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesUsers_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getUsers(ByVal Token As String, ByRef errMsg As String) As String
        Dim strJson As String = ""
        Dim dsData As New DataSet

        Try
            strCommand = "CompaniesUsers_GetByToken_v2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.TableMappings.Add("Table", "Users")
            adapter.TableMappings.Add("Table1", "Modules")
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

            dsData.Relations.Add("UsrModules", dsData.Tables("Users").Columns("ID"), dsData.Tables("Modules").Columns("UserID"), False)

            Dim dvModules As DataView
            Dim drvUsr As DataRowView
            Dim drvMod As DataRowView


            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)

            Dim sbUser As StringBuilder = Nothing
            Dim swUser As StringWriter = Nothing
            Dim jsonUser As Newtonsoft.Json.JsonTextWriter = Nothing

            Dim sbModule As StringBuilder = Nothing
            Dim swModule As StringWriter = Nothing
            Dim jsonModule As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()

            json.WritePropertyName("users")
            json.WriteStartArray()

            For Each drvUsr In dsData.Tables("Users").DefaultView

                sbUser = New StringBuilder
                swUser = New StringWriter(sbUser)
                jsonUser = New Newtonsoft.Json.JsonTextWriter(swUser)

                jsonUser.WriteStartObject()

                jsonUser.WritePropertyName("id")
                jsonUser.WriteValue(drvUsr.Item("GUID"))

                jsonUser.WritePropertyName("name")
                jsonUser.WriteValue(drvUsr.Item("Name"))

                jsonUser.WritePropertyName("firstName")
                jsonUser.WriteValue(drvUsr.Item("FirstName"))

                jsonUser.WritePropertyName("lastName")
                jsonUser.WriteValue(drvUsr.Item("lastName"))

                jsonUser.WritePropertyName("email")
                jsonUser.WriteValue(drvUsr.Item("Email"))

                jsonUser.WritePropertyName("isEmailAlerts")
                jsonUser.WriteValue(drvUsr.Item("IsEmailAlerts"))

                jsonUser.WritePropertyName("phone")
                jsonUser.WriteValue(drvUsr.Item("Phone"))

                jsonUser.WritePropertyName("cellPhone")
                jsonUser.WriteValue(drvUsr.Item("CellPhone"))

                jsonUser.WritePropertyName("isSMSAlerts")
                jsonUser.WriteValue(drvUsr.Item("IsSMSAlerts"))

                jsonUser.WritePropertyName("carrierId")
                jsonUser.WriteValue(drvUsr.Item("SMSGatewayID"))

                jsonUser.WritePropertyName("login")
                jsonUser.WriteValue(drvUsr.Item("Login"))

                jsonUser.WritePropertyName("timeZoneCode")
                jsonUser.WriteValue(drvUsr.Item("TimeZoneCode"))

                jsonUser.WritePropertyName("isDriver")
                jsonUser.WriteValue(drvUsr.Item("IsDriver"))

                jsonUser.WritePropertyName("accessLevelId")
                jsonUser.WriteValue(drvUsr.Item("AccessLevelID"))

                jsonUser.WritePropertyName("accessLevelName")
                jsonUser.WriteValue(drvUsr.Item("AccessLevelName"))

                jsonUser.WritePropertyName("scheduleId")
                jsonUser.WriteValue(drvUsr.Item("ScheduleID"))

                jsonUser.WritePropertyName("scheduleName")
                jsonUser.WriteValue(drvUsr.Item("ScheduleName"))

                jsonUser.WritePropertyName("isAdministrator")
                jsonUser.WriteValue(drvUsr.Item("IsAdministrator"))

                jsonUser.WritePropertyName("isBillingContact")
                If IsDBNull(drvUsr.Item("isBillingContact")) Then
                    jsonUser.WriteValue(False)
                Else
                    jsonUser.WriteValue(drvUsr.Item("isBillingContact"))
                End If

                jsonUser.WritePropertyName("iButton")
                jsonUser.WriteValue(drvUsr.Item("iButton"))

                jsonUser.WritePropertyName("isAllModules")
                If IsDBNull(drvUsr.Item("isAllModules")) Then
                    jsonUser.WriteValue(1)
                Else
                    jsonUser.WriteValue(drvUsr.Item("isAllModules"))
                End If

                'Get the details of this device
                dvModules = drvUsr.CreateChildView("UsrModules")
                jsonUser.WritePropertyName("modules")
                jsonUser.WriteStartArray()

                For Each drvMod In dvModules

                    sbModule = New StringBuilder
                    swModule = New StringWriter(sbModule)
                    jsonModule = New Newtonsoft.Json.JsonTextWriter(swModule)

                    jsonModule.WriteStartObject()
                    jsonModule.WritePropertyName("moduleId")
                    jsonModule.WriteValue(drvMod.Item("ModuleID"))

                    jsonModule.WritePropertyName("name")
                    jsonModule.WriteValue(drvMod.Item("Name"))

                    jsonModule.WriteEndObject()
                    jsonModule.Flush()
                    jsonUser.WriteValue(sbModule.ToString())

                Next

                jsonUser.WriteEnd()
                jsonUser.WriteEndObject()
                jsonUser.Flush()
                json.WriteValue(sbUser.ToString())
            Next

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        End Try

        Return strJson

    End Function

    Public Function getUsersBasicInfo(ByVal Token As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "CompaniesUsers_GetBasicInfo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function saveUser(ByVal token As String, ByVal id As String, ByVal firstName As String, ByVal lastName As String, ByVal email As String, ByVal isEmailAlerts As Boolean, ByVal phone As String, ByVal cellPhone As String, ByVal isSMSAlerts As Boolean, ByVal SMSGatewayID As Integer, ByVal login As String, ByVal password As String, ByVal timeZoneCode As String, ByVal isDriver As Boolean, ByVal accessLevelId As Integer, ByVal scheduleId As String, ByVal isAdministrator As Boolean, ByVal isBillingContact As Boolean, ByVal iButton As String, ByVal isAllModules As Boolean, ByVal dtModules As DataTable, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "CompaniesUsers_UPDATE_v2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parIsEmailAlerts As New SqlClient.SqlParameter("@IsEmailAlerts", SqlDbType.Bit)
            parIsEmailAlerts.Value = isEmailAlerts
            Command.Parameters.Add(parIsEmailAlerts)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parCellPhone As New SqlClient.SqlParameter("@CellPhone", SqlDbType.NVarChar, 50)
            parCellPhone.Value = cellPhone
            Command.Parameters.Add(parCellPhone)

            Dim parIsSMSAlerts As New SqlClient.SqlParameter("@IsSMSAlerts", SqlDbType.Bit)
            parIsSMSAlerts.Value = isSMSAlerts
            Command.Parameters.Add(parIsSMSAlerts)

            Dim parSMSGatewayID As New SqlClient.SqlParameter("@SMSGatewayID", SqlDbType.Int)
            parSMSGatewayID.Value = SMSGatewayID
            Command.Parameters.Add(parSMSGatewayID)

            Dim parLogin As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 50)
            parLogin.Value = login
            Command.Parameters.Add(parLogin)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parTimeZone As New SqlClient.SqlParameter("@TimeZoneCode", SqlDbType.NVarChar, 5)
            parTimeZone.Value = timeZoneCode
            Command.Parameters.Add(parTimeZone)

            Dim parIsDriver As New SqlClient.SqlParameter("@IsDriver", SqlDbType.Bit)
            parIsDriver.Value = isDriver
            Command.Parameters.Add(parIsDriver)

            Dim parAccessLevelID As New SqlClient.SqlParameter("@AccessLevelID", SqlDbType.Int)
            parAccessLevelID.Value = accessLevelId
            Command.Parameters.Add(parAccessLevelID)

            Dim parScheduleID As New SqlClient.SqlParameter("@ScheduleID", SqlDbType.NVarChar, 50)
            parScheduleID.Value = scheduleId
            Command.Parameters.Add(parScheduleID)

            Dim parIsAdministrator As New SqlClient.SqlParameter("@IsAdministrator", SqlDbType.Bit)
            parIsAdministrator.Value = isAdministrator
            Command.Parameters.Add(parIsAdministrator)

            Dim parIsBillingContact As New SqlClient.SqlParameter("@IsBillingContact", SqlDbType.Bit)
            parIsBillingContact.Value = isBillingContact
            Command.Parameters.Add(parIsBillingContact)

            Dim parIButton As New SqlClient.SqlParameter("@IButton", SqlDbType.VarChar, 50)
            parIButton.Value = iButton
            Command.Parameters.Add(parIButton)

            Dim parIsAllModules As New SqlClient.SqlParameter("@IsAllModules", SqlDbType.Bit)
            parIsAllModules.Value = isAllModules
            Command.Parameters.Add(parIsAllModules)

            Dim parModules As New SqlClient.SqlParameter("@ModulesList", SqlDbType.Structured)
            parModules.Value = dtModules
            Command.Parameters.Add(parModules)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            If ex.Message = "Login is not available. Please try a different login." Then
                strError = ex.Message
            Else
                strError = "Error saving New User"
            End If
            BLErrorHandling.ErrorCapture(pSysModule, "saveUser", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function changeUserCredentials(ByVal token As String, ByVal id As String, ByVal login As String, ByVal password As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "CompaniesUsers_CredentialsUpdate"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parLogin As New SqlClient.SqlParameter("@Login", SqlDbType.NVarChar, 50)
            parLogin.Value = login
            Command.Parameters.Add(parLogin)

            Dim parPassword As New SqlClient.SqlParameter("@Password", SqlDbType.NVarChar, 50)
            parPassword.Value = password
            Command.Parameters.Add(parPassword)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error saving new credentials"
            BLErrorHandling.ErrorCapture(pSysModule, "changeUserCredentials", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function removeUser(ByVal token As String, ByVal id As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            strCommand = "CompaniesUsers_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error removing user"
            BLErrorHandling.ErrorCapture(pSysModule, "removeUser", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function recoverCredentials(ByVal email As String, ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            email = BLCommon.Sanitize(email)

            strCommand = "CompaniesUsers_RecoverCredentials"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error retieving credentials"
            BLErrorHandling.ErrorCapture(pSysModule, "recoverCredentials", "", ex.Message & " - email: " & email, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function getNotificationsSendTo(ByVal token As String, ByVal entityName As String, ByVal entityId As String, ByRef msgError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "GetNotificationsSendTo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEntityName As New SqlClient.SqlParameter("@EntityName", SqlDbType.NVarChar, 50)
            parEntityName.Direction = ParameterDirection.Input
            parEntityName.Value = entityName
            Command.Parameters.Add(parEntityName)

            Dim parEntityId As New SqlClient.SqlParameter("@EntityId", SqlDbType.NVarChar, 50)
            parEntityId.Direction = ParameterDirection.Input
            parEntityId.Value = entityId
            Command.Parameters.Add(parEntityId)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

#End Region

#Region "Other methods"

    Public Function getDevices(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal deviceGUID As String, ByRef strError As String) As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "Devices_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parDeviceGUID As New SqlClient.SqlParameter("@DeviceGUID", SqlDbType.NVarChar, 50)
            parDeviceGUID.Direction = ParameterDirection.Input
            parDeviceGUID.Value = deviceGUID
            Command.Parameters.Add(parDeviceGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dsData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        Finally

            conSQL.Dispose()
        End Try

        Return dsData

    End Function

    Public Function getDevices(ByVal Token As String, ByVal LastFetchOn As Date, ByRef strError As String) As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "Devices_GetByToken_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parLastFetchOn As New SqlClient.SqlParameter("@LastFetchOn", SqlDbType.DateTime)
            parLastFetchOn.Direction = ParameterDirection.Input
            parLastFetchOn.Value = LastFetchOn
            Command.Parameters.Add(parLastFetchOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dsData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = ex.Message
            Else
                strError = "Error getting devices"
                BLErrorHandling.ErrorCapture(pSysModule, "getDevices", "Token: " & Token, ex.Message & " - Token: " & Token, 0)
            End If
        Finally

            conSQL.Dispose()
        End Try

        Return dsData

    End Function
    Public Function getDevicesBrokers(ByVal Token As String, ByVal LastFetchOn As Date, ByRef strError As String) As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "Devices_GetByTokenBrokerOrder_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parLastFetchOn As New SqlClient.SqlParameter("@LastFetchOn", SqlDbType.DateTime)
            parLastFetchOn.Direction = ParameterDirection.Input
            parLastFetchOn.Value = LastFetchOn
            Command.Parameters.Add(parLastFetchOn)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dsData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = ex.Message
            Else
                strError = "Error getting devices"
                BLErrorHandling.ErrorCapture(pSysModule, "getDevices", "Token: " & Token, ex.Message & " - Token: " & Token, 0)
            End If
        Finally

            conSQL.Dispose()
        End Try

        Return dsData

    End Function

    Public Function GetDeviceById(ByVal token As String, ByVal deviceId As String) As deviceDet
        Dim dev As deviceDet = Nothing

        Try
            strCommand = "Devices_GetByDeviceID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                dev = New deviceDet
                dev.latitude = reader.Item("Latitude")
                dev.longitude = reader.Item("Longitude")
                dev.infoTable = reader.Item("InfoTable")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return dev

    End Function

    Public Function getDeviceByGUID(ByVal GUID As String, ByRef strError As String) As DataView
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing

        Try
            strCommand = "Devices_GetByGUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Input
            parGUID.Value = GUID
            Command.Parameters.Add(parGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        End Try

        Return dtData.DefaultView

    End Function

    Public Function Devices_GetByGUID_BasicInfo(ByVal GUID As String, ByRef strError As String) As DataView
        Dim dtData As New DataTable
        Dim dvData As DataView = Nothing

        Try
            strCommand = "Devices_GetByGUID_BasicInfo"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Input
            parGUID.Value = GUID
            Command.Parameters.Add(parGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        End Try

        Return dtData.DefaultView

    End Function

    Function GetTrail(ByVal id As String, ByVal dateFrom As DateTime, ByVal dateTo As DateTime, ByRef msg As String) As DataView
        Dim dtData As New DataTable

        Try
            strCommand = "Devices_GetTrailByGUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parDeviceGUID As New SqlClient.SqlParameter("@DeviceGUID", SqlDbType.NVarChar, 50)
            parDeviceGUID.Direction = ParameterDirection.Input
            parDeviceGUID.Value = id
            Command.Parameters.Add(parDeviceGUID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Direction = ParameterDirection.Input
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Direction = ParameterDirection.Input
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData.DefaultView

    End Function

    Public Function getGeofences(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Geofences_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getDeviceHistory(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal deviceGUID As String, ByVal dateFrom As Date, ByVal dateTo As Date, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etAPI_getDeviceHistory"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parDeviceGUID As New SqlClient.SqlParameter("@DeviceGUID", SqlDbType.NVarChar, 50)
            parDeviceGUID.Direction = ParameterDirection.Input
            parDeviceGUID.Value = deviceGUID
            Command.Parameters.Add(parDeviceGUID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Direction = ParameterDirection.Input
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Direction = ParameterDirection.Input
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function getDeviceHistory2(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As Date, ByVal dateTo As Date) As List(Of class_DeviceHistory)
        Dim lst As New List(Of class_DeviceHistory)
        Dim itm As class_DeviceHistory

        Try
            strCommand = "etAPI_GetDeviceHistory2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 10)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Direction = ParameterDirection.Input
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Direction = ParameterDirection.Input
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New class_DeviceHistory
                itm.deviceId = reader.Item("DeviceID")
                itm.eventName = reader.Item("EventName")
                itm.eventDate = reader.Item("EventDate").ToString
                itm.geofenceName = reader.Item("GeofenceName")
                itm.driverFirstName = reader.Item("DriverFirstName")
                itm.driverLastName = reader.Item("DriverLastName")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If


        Catch ex As Exception
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return lst

    End Function

    Public Function getNathan(ByVal token As String, ByVal deviceId As String, ByVal dataPointer As Long) As List(Of class_DeviceHistory)
        Dim lst As New List(Of class_DeviceHistory)
        Dim itm As class_DeviceHistory
        Dim strError As String = ""

        Try
            strCommand = "etAPI_GetDeviceHistoryNathan"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 10)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parPointer As New SqlClient.SqlParameter("@DataPointer", SqlDbType.BigInt)
            parPointer.Direction = ParameterDirection.Input
            parPointer.Value = dataPointer
            Command.Parameters.Add(parPointer)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New class_DeviceHistory
                itm.deviceId = reader.Item("DeviceID")
                itm.eventName = reader.Item("EventName")
                itm.eventDate = reader.Item("EventDate").ToString
                itm.geofenceName = reader.Item("GeofenceName")
                itm.driverFirstName = reader.Item("DriverFirstName")
                itm.driverLastName = reader.Item("DriverLastName")
                itm.createdOn = reader.Item("CreatedOn")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If


        Catch ex As Exception
            strError = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return lst

    End Function

    Public Function getGeofenceAlertStatus(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etAPI_getGeofenceAlertStatus"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        Finally

        End Try

        Return dtData

    End Function

    Public Function GetGeofenceCrossings(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByVal dateFrom As Date, ByVal dateTo As Date, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etAPI_getGeofenceCrossing"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)
            parDateFrom.Direction = ParameterDirection.Input
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Direction = ParameterDirection.Input
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function GetGeofenceSpeedLimitStatus(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etAPI_getGeofenceSpeedLimitStatus"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function DeleteGeofence(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByRef strError As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etAPI_deleteGeofence"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parResult.Value)

        Catch ex As Exception
            bResult = False
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "Delete geofence failed: " & ex.Message
            End If

        End Try

        Return bResult

    End Function

    Public Function SetGeofenceAlertStatus(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByVal AlertTypeID As Integer, ByRef strError As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etAPI_setGeofenceAlertStatus"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            Dim parAlertTypeID As New SqlClient.SqlParameter("@AlertTypeID", SqlDbType.Int)
            parAlertTypeID.Direction = ParameterDirection.Input
            parAlertTypeID.Value = AlertTypeID
            Command.Parameters.Add(parAlertTypeID)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parResult.Value)

        Catch ex As Exception
            bResult = False
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "Setting alert type in geofence has failed."
            End If
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function SetGeofenceSpeedLimitStatus(ByVal Token As String, ByVal isAPIToken As Boolean, ByVal geofenceGUID As String, ByVal isEnabled As Boolean, ByVal speedLimit As Integer, ByRef strError As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etAPI_setGeofenceSpeedLimit"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parIsAPIToken As New SqlClient.SqlParameter("@IsAPIToken", SqlDbType.Bit)
            parIsAPIToken.Direction = ParameterDirection.Input
            parIsAPIToken.Value = isAPIToken
            Command.Parameters.Add(parIsAPIToken)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            Dim parIsEnabled As New SqlClient.SqlParameter("@IsEnabled", SqlDbType.Bit)
            parIsEnabled.Direction = ParameterDirection.Input
            parIsEnabled.Value = isEnabled
            Command.Parameters.Add(parIsEnabled)

            Dim parSpeedLimit As New SqlClient.SqlParameter("@SpeedLimit", SqlDbType.Int)
            parSpeedLimit.Direction = ParameterDirection.Input
            parSpeedLimit.Value = speedLimit
            Command.Parameters.Add(parSpeedLimit)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parResult.Value)

        Catch ex As Exception
            bResult = False
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "Setting Speed Limit in geofence has failed."
            End If
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function GeofencesUPDATE(ByVal apiToken As String, ByVal geofenceGUID As String, ByVal geofenceName As String, ByVal street As String, ByVal city As String, ByVal state As String, ByVal postalCode As String, ByVal latitude As Decimal, ByVal longitude As Decimal, ByVal radiusFeet As Integer, ByVal alertTypeId As Integer, ByVal isSpeedLimit As Boolean, ByVal speedLimit As Integer, ByVal geofenceTypeId As String, ByRef msg As String) As String
        Try
            strCommand = "etAPI_Geofences_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = apiToken
            Command.Parameters.Add(parToken)

            Dim parGeofenceID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.NVarChar, 50)
            parGeofenceID.Direction = ParameterDirection.Input
            parGeofenceID.Value = geofenceGUID
            Command.Parameters.Add(parGeofenceID)

            Dim parGeofenceName As New SqlClient.SqlParameter("@GeofenceName", SqlDbType.NVarChar, 50)
            parGeofenceName.Direction = ParameterDirection.Input
            parGeofenceName.Value = geofenceName
            Command.Parameters.Add(parGeofenceName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Direction = ParameterDirection.Input
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Direction = ParameterDirection.Input
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Direction = ParameterDirection.Input
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 20)
            parPostalCode.Direction = ParameterDirection.Input
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = latitude
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = longitude
            Command.Parameters.Add(parLongitude)

            Dim parRadius As New SqlClient.SqlParameter("@RadiusFeet", SqlDbType.Int, 4)
            parRadius.Direction = ParameterDirection.Input
            parRadius.Value = radiusFeet
            Command.Parameters.Add(parRadius)

            Dim parAlertTypeID As New SqlClient.SqlParameter("@AlertTypeID", SqlDbType.Int, 4)
            parAlertTypeID.Direction = ParameterDirection.Input
            parAlertTypeID.Value = alertTypeId
            Command.Parameters.Add(parAlertTypeID)

            Dim parIsSpeedLimit As New SqlClient.SqlParameter("@IsSpeedLimit", SqlDbType.Bit)
            parIsSpeedLimit.Direction = ParameterDirection.Input
            parIsSpeedLimit.Value = isSpeedLimit
            Command.Parameters.Add(parIsSpeedLimit)

            Dim parSpeedLimit As New SqlClient.SqlParameter("@SpeedLimit", SqlDbType.Int, 4)
            parSpeedLimit.Direction = ParameterDirection.Input
            parSpeedLimit.Value = speedLimit
            Command.Parameters.Add(parSpeedLimit)

            Dim parGeofenceTypeGUID As New SqlClient.SqlParameter("@GeofenceTypeGUID", SqlDbType.VarChar, 50)
            parGeofenceTypeGUID.Direction = ParameterDirection.Input
            parGeofenceTypeGUID.Value = geofenceTypeId
            Command.Parameters.Add(parGeofenceTypeGUID)

            Dim parGeoID As New SqlClient.SqlParameter("@GeoID", SqlDbType.NVarChar, 50)
            parGeoID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGeoID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            geofenceGUID = CStr(parGeoID.Value)

        Catch ex As Exception
            geofenceGUID = ""
            msg = "Failed creating/updateing geofence: " '& ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return geofenceGUID

    End Function

    Public Function GetGeofenceTypes(ByVal token As String, ByRef msg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "etAPI_GeofencesTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@ApiToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "OTHERERROR"
            End If
        End Try

        Return dtData

    End Function

    Public Function addGeofenceType(ByVal apiToken As String, ByVal ID As String, ByVal Name As String, ByRef msg As String) As String
        Dim newId As String = ""

        Try
            strCommand = "etAPI_GeofencesTypes_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = apiToken
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Direction = ParameterDirection.Input
            parName.Value = Name
            Command.Parameters.Add(parName)

            Dim parNewID As New SqlClient.SqlParameter("@NewID", SqlDbType.NVarChar, 50)
            parNewID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            newId = CStr(parNewID.Value)

        Catch ex As Exception
            newId = ""
            msg = "Failed creating/updateing geofence type"
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return newId

    End Function

    Function deleteGeofenceType(ByVal token As String, ByVal id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "etAPI_GeofenceTypes_DELETE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            bResult = CBool(parResult.Value)

        Catch ex As Exception
            bResult = False
            If ex.Message = "LOGOUT" Then
                msg = "TOKENEXPIRED"
            Else
                msg = "Delete geofence type failed: " & ex.Message
            End If
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function GetHotSpots(ByVal Token As String, ByVal deviceId As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "HotSpots_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function GetBasicList(ByVal Token As String, ByVal entityName As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "getBasicList"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parEntityName As New SqlClient.SqlParameter("@EntityName", SqlDbType.NVarChar, 50)
            parEntityName.Direction = ParameterDirection.Input
            parEntityName.Value = entityName
            Command.Parameters.Add(parEntityName)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()
        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function GetIdNameList(ByVal token As String, ByVal entityName As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As New basicList
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "getBasicList"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEntityName As New SqlClient.SqlParameter("@EntityName", SqlDbType.NVarChar, 50)
            parEntityName.Direction = ParameterDirection.Input
            parEntityName.Value = entityName
            Command.Parameters.Add(parEntityName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("id")
                itm.name = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Customers / Geofences"

    Public Function CustomerSearch(ByVal token As String, ByVal custName As String) As List(Of customerSearch)
        Dim lst As New List(Of customerSearch)
        Dim itm As customerSearch = Nothing
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Customers_SEARCH"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 100)
            parName.Value = custName
            Command.Parameters.Add(parName)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New customerSearch
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.workZoneId = reader.Item("WorkZoneID")
                itm.workZoneName = reader.Item("WorkZoneName")
                itm.contactName = reader.Item("ContactName")
                itm.address = reader.Item("Address")
                itm.phone = reader.Item("Phone")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Devices"

    Public Function searchDevice(ByVal token As String, ByVal searchKey As String, ByVal keyValue As String) As searchDeviceResult
        Dim itm As New searchDeviceResult

        Try
            strCommand = "CRM_Devices_SearchDevice"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parSearchKey As New SqlClient.SqlParameter("@SearchKey", SqlDbType.NVarChar, 10)
            parSearchKey.Value = searchKey
            Command.Parameters.Add(parSearchKey)

            Dim parKeyValue As New SqlClient.SqlParameter("@KeyValue", SqlDbType.NVarChar, 10)
            parKeyValue.Value = keyValue
            Command.Parameters.Add(parKeyValue)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New searchDeviceResult
                itm.customerGUID = reader.Item("customerGUID")
                itm.customerName = reader.Item("customerName")
                itm.deviceGUID = reader.Item("DeviceGUID")
                itm.deviceId = reader.Item("deviceID")
                itm.deviceName = reader.Item("deviceName")
                itm.serialNumber = reader.Item("serialNumber")
                itm.imei = reader.Item("imei")
                itm.simCarrier = reader.Item("simCarrier")
                itm.simNo = reader.Item("simNo")
                itm.simAreaCode = reader.Item("simAreaCode")
                itm.simPhoneNumber = reader.Item("simPhoneNumber")
                itm.lastUpdatedOn = reader.Item("lastUpdatedOn").ToString
                itm.eventDate = reader.Item("eventDate").ToString
                itm.eventCode = reader.Item("eventCode")
                itm.gpsAge = reader.Item("gpsAge")
                itm.lastGoodGpsOn = reader.Item("lastGoodGpsOn").ToString
                itm.hasSpeedGauge = reader.Item("hasSpeedGauge")
                itm.sgStatus = reader.Item("sgStatus")
                itm.sgFee = reader.Item("sgFee")
                itm.isNotWorking = reader.Item("IsNotWorking")
                itm.notWorkingSince = reader.Item("NotWorkingSince").ToString
                itm.isInactive = reader.Item("isInactive")
                itm.inactiveReason = reader.Item("inactiveReason")
                itm.isRMA = reader.Item("isRMA")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.dataLayer.svc", "", "searchDevice", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function deviceAction(ByVal token As String, ByVal action As String, ByVal id As String, ByVal param1 As String, ByVal param2 As String) As callResult
        Dim res As New callResult

        Try
            strCommand = "CRM_Devices_SetThings"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parAction As New SqlClient.SqlParameter("@Action", SqlDbType.NVarChar, 50)
            parAction.Value = action
            Command.Parameters.Add(parAction)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parParam1 As New SqlClient.SqlParameter("@Param1", SqlDbType.NVarChar, 50)
            parParam1.Value = param1
            Command.Parameters.Add(parParam1)

            Dim parParam2 As New SqlClient.SqlParameter("@Param2", SqlDbType.NVarChar, 50)
            parParam2.Value = param2
            Command.Parameters.Add(parParam2)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)
            res.msg = CStr(parMsg.Value)

        Catch ex As Exception
            BLErrorHandling.ErrorCapture("etapp.dataLayer.svc", "", "deviceAction", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function
    Public Function InfoDevicesInputs(ByVal ptoken As String, ByVal pdevideid As Integer, ByVal pCount As Integer) As DevicesInformationInputs
        Dim response As New DevicesInformationInputs
        Dim objtemperature As Temperature
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "etPilot_INPUT_OUTPUT_TEMPERATURE_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim token As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 100) With {
                .Value = ptoken
            }
            Command.Parameters.Add(token)

            Dim devideid As New SqlClient.SqlParameter("@DeviceID", SqlDbType.Int) With {
                .Value = pdevideid
            }
            Command.Parameters.Add(devideid)

            Dim count As New SqlClient.SqlParameter("@Count", SqlDbType.Int) With {
                .Value = pCount
            }
            Command.Parameters.Add(count)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 1
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    response.ID = reader.Item("ID")
                    response.CompanyID = reader.Item("CompanyID")
                    response.DeviceID = reader.Item("DeviceID")
                    response.Name = reader.Item("Name")
                    response.Input1Name = reader.Item("Input1Name")
                    response.Input2Name = reader.Item("Input2Name")
                    response.Input3Name = reader.Item("Input3Name")
                    response.Input4Name = reader.Item("Input4Name")
                    response.Output1Name = reader.Item("Output1Name")
                    response.Output2Name = reader.Item("Output2Name")
                    response.Output3Name = reader.Item("Output3Name")
                    response.Output4Name = reader.Item("Output4Name")
                    response.Input1 = reader.Item("SW1")
                    response.Input2 = reader.Item("SW2")
                    response.Input3 = reader.Item("SW3")
                    response.Input4 = reader.Item("SW4")
                    response.Output1 = reader.Item("Relay1")
                    response.Output2 = reader.Item("Relay2")
                    response.Output3 = reader.Item("Relay3")
                    response.Output4 = reader.Item("Relay4")
                    response.AngInput = reader.Item("AngInput")
                    response.DigInput = reader.Item("DigInput")
                    response.Output = reader.Item("Output")
                Loop
                reader.NextResult()
                Do While reader.Read
                    objtemperature = New Temperature
                    objtemperature.DeviceID = reader.Item("DeviceID")
                    objtemperature.Temperature1 = reader.Item("Temperature1")
                    objtemperature.Temperature2 = reader.Item("Temperature2")
                    objtemperature.Temperature3 = reader.Item("Temperature3")
                    objtemperature.Temperature4 = reader.Item("Temperature4")

                    response.Temperatures.Add(objtemperature)
                Loop
                If Not reader.IsClosed Then
                    reader.Close()
                End If
            Else
                response = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            response = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return response
    End Function

    Public Function InfoDevicesInputs_Update(ByVal token As String, data As DevicesInformationInputs, ByVal pRelayNum As Integer, ByVal pRelayNumValue As Boolean) As responseOk
        Dim response As New responseOk
        Try
            strCommand = "etPilot_INPUT_OUTPUT_TEMPERATURE_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim ID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = data.ID
            Command.Parameters.Add(ID)

            Dim RelayNum As New SqlClient.SqlParameter("@RelayNum", SqlDbType.TinyInt)
            RelayNum.Direction = ParameterDirection.Input
            RelayNum.Value = pRelayNum
            Command.Parameters.Add(RelayNum)

            Dim RelayNumValue As New SqlClient.SqlParameter("@RelayNumValue", SqlDbType.Bit)
            RelayNumValue.Direction = ParameterDirection.Input
            RelayNumValue.Value = pRelayNumValue
            Command.Parameters.Add(RelayNumValue)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.VarChar, 50)
            Action.Direction = ParameterDirection.Input
            Action.Value = 2
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@ISOK", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()
            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrder_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrder_SP-ACTION-1", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function

#End Region

#Region "Inventory"

    Public Function getInventory(ByVal token As String) As List(Of inventory)
        Dim lst As New List(Of inventory)
        Dim itm As inventory = Nothing

        Try
            strCommand = "CRM_DealersInventory_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New inventory
                itm.id = reader.Item("id")
                itm.deviceId = reader.Item("deviceId")
                itm.deviceTypeName = reader.Item("deviceTypeName")
                itm.serialNumber = reader.Item("serialNumber")
                itm.imei = reader.Item("imei")
                itm.simNo = reader.Item("simNo").ToString
                itm.simAreaCode = reader.Item("simAreaCode").ToString
                itm.simPhoneNumber = reader.Item("simPhoneNumber").ToString
                itm.createdOn = reader.Item("CreatedOn").ToString
                itm.lastUpdatedOn = reader.Item("LastUpdatedOn").ToString
                itm.eventDate = reader.Item("EventDate").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return lst

    End Function

    Public Function assignDevice(ByVal Token As String, ByVal custId As String, ByVal courrierId As String, ByVal trackingNumber As String, ByVal deviceId As String) As Boolean
        Dim isOk As Boolean = True

        Try
            strCommand = "CRM_Devices_Assign"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            Dim parCourrierId As New SqlClient.SqlParameter("@CourrierId", SqlDbType.NVarChar, 50)
            parCourrierId.Value = courrierId
            Command.Parameters.Add(parCourrierId)

            Dim parTrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 50)
            parTrackingNumber.Value = trackingNumber
            Command.Parameters.Add(parTrackingNumber)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 20)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return isOk

    End Function

#End Region

#Region "Suspend/Resume Company"

    Function GetCompaniesSuspendedReasons() As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "CompaniesSuspendedReasons_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Function getCompaniesSuspendResume(ByVal token As String) As List(Of Company)
        Dim lst As New List(Of Company)
        Dim itm As Company
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "CRM_Customers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New Company
                itm.id = reader.Item("UniqueKey")
                itm.name = reader.Item("Name")
                itm.isSuspended = reader.Item("bIsSuspended")
                itm.suspendedId = reader.Item("SuspendedID")
                itm.suspendedOn = reader.Item("SuspendedOn").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function saveSuspendCompany(ByVal Token As String, ByVal data As Company) As Boolean
        Dim isOk As Boolean = True

        Try
            strCommand = "CRM_Companies_SuspendResume"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = data.id
            Command.Parameters.Add(parCustID)

            Dim parIsSuspended As New SqlClient.SqlParameter("@IsSuspended", SqlDbType.Bit)
            parIsSuspended.Value = data.isSuspended
            Command.Parameters.Add(parIsSuspended)

            Dim parReasonId As New SqlClient.SqlParameter("@SuspendedID", SqlDbType.Int)
            parReasonId.Value = CInt(data.suspendedId)
            Command.Parameters.Add(parReasonId)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()

        End Try

        Return isOk

    End Function

#End Region

#Region "Telemetry"

    Public Function GetIOs(ByVal token As String, ByVal deviceId As String) As devIOs
        Dim itm As New devIOs
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devIOs
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("DeviceName")
                itm.name1 = reader.Item("Rel1Name")
                itm.on1Name = reader.Item("Relay1OnName")
                itm.off1Name = reader.Item("Relay1OffName")
                itm.status1 = reader.Item("Rel1Status")
                itm.name2 = reader.Item("Rel2Name")
                itm.on2Name = reader.Item("Relay2OnName")
                itm.off2Name = reader.Item("Relay2OffName")
                itm.status2 = reader.Item("Rel2Status")
                itm.name3 = reader.Item("Rel3Name")
                itm.on3Name = reader.Item("Relay3OnName")
                itm.off3Name = reader.Item("Relay3OffName")
                itm.status3 = reader.Item("Rel3Status")
                itm.name4 = reader.Item("Rel4Name")
                itm.on4Name = reader.Item("Relay4OnName")
                itm.off4Name = reader.Item("Relay4OffName")
                itm.status4 = reader.Item("Rel4Status")

                itm.inputName1 = reader.Item("Input1Name")
                itm.inputOn1Name = reader.Item("Input1OnName")
                itm.inputOff1Name = reader.Item("Input1OffName")
                itm.inputStatus1 = reader.Item("Input1Status")
                itm.inputName2 = reader.Item("Input2Name")
                itm.inputOn2Name = reader.Item("Input2OnName")
                itm.inputOff2Name = reader.Item("Input2OffName")
                itm.inputStatus2 = reader.Item("Input2Status")
                itm.inputName3 = reader.Item("Input3Name")
                itm.inputOn3Name = reader.Item("Input3OnName")
                itm.inputOff3Name = reader.Item("Input3OffName")
                itm.inputStatus3 = reader.Item("Input3Status")
                itm.inputName4 = reader.Item("Input4Name")
                itm.inputOn4Name = reader.Item("Input4OnName")
                itm.inputOff4Name = reader.Item("Input4OffName")
                itm.inputStatus4 = reader.Item("Input4Status")

            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function GetAllDevIOs(ByVal token As String) As List(Of devIOs)
        Dim lst As New List(Of devIOs)
        Dim itm As New devIOs
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = "0"
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devIOs
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("DeviceName")
                itm.name1 = reader.Item("Rel1Name")
                itm.on1Name = reader.Item("Relay1OnName")
                itm.off1Name = reader.Item("Relay1OffName")
                itm.status1 = reader.Item("Rel1Status")
                itm.name2 = reader.Item("Rel2Name")
                itm.on2Name = reader.Item("Relay2OnName")
                itm.off2Name = reader.Item("Relay2OffName")
                itm.status2 = reader.Item("Rel2Status")
                itm.name3 = reader.Item("Rel3Name")
                itm.on3Name = reader.Item("Relay3OnName")
                itm.off3Name = reader.Item("Relay3OffName")
                itm.status3 = reader.Item("Rel3Status")
                itm.name4 = reader.Item("Rel4Name")
                itm.on4Name = reader.Item("Relay4OnName")
                itm.off4Name = reader.Item("Relay4OffName")
                itm.status4 = reader.Item("Rel4Status")

                itm.inputName1 = reader.Item("Input1Name")
                itm.inputOn1Name = reader.Item("Input1OnName")
                itm.inputOff1Name = reader.Item("Input1OffName")
                itm.inputStatus1 = reader.Item("Input1Status")
                itm.inputName2 = reader.Item("Input2Name")
                itm.inputOn2Name = reader.Item("Input2OnName")
                itm.inputOff2Name = reader.Item("Input2OffName")
                itm.inputStatus2 = reader.Item("Input2Status")
                itm.inputName3 = reader.Item("Input3Name")
                itm.inputOn3Name = reader.Item("Input3OnName")
                itm.inputOff3Name = reader.Item("Input3OffName")
                itm.inputStatus3 = reader.Item("Input3Status")
                itm.inputName4 = reader.Item("Input4Name")
                itm.inputOn4Name = reader.Item("Input4OnName")
                itm.inputOff4Name = reader.Item("Input4OffName")
                itm.inputStatus4 = reader.Item("Input4Status")

                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function setOutput(ByVal token As String, ByVal data As setRelay, ByRef msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "Devices_SetDeviceOutput"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parRelNum As New SqlClient.SqlParameter("@RelayNum", SqlDbType.Int, 4)
            parRelNum.Value = data.relayNum
            Command.Parameters.Add(parRelNum)

            Dim parStat As New SqlClient.SqlParameter("@NewStatus", SqlDbType.Bit)
            parStat.Value = data.newStatus
            Command.Parameters.Add(parStat)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error setting output"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.setOutput", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

    Public Function telemetrySetUp(token As String, data As ioSetUp, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            If data.ioType = 1 Then
                strCommand = "Devices_IOs_InputsSetUp"
            Else
                strCommand = "Devices_IOs_OutputsSetUp"
            End If
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIONum As SqlClient.SqlParameter = Nothing
            If data.ioType = 1 Then
                parIONum = New SqlClient.SqlParameter("@InputNum", SqlDbType.Int, 4)
            Else
                parIONum = New SqlClient.SqlParameter("@RelayNum", SqlDbType.Int, 4)
            End If
            parIONum.Value = data.ioNum
            Command.Parameters.Add(parIONum)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parOn As New SqlClient.SqlParameter("@OnStatus", SqlDbType.NVarChar, 50)
            parOn.Value = data.onStatus
            Command.Parameters.Add(parOn)

            Dim parOff As New SqlClient.SqlParameter("@OffStatus", SqlDbType.NVarChar, 50)
            parOff.Value = data.offStatus
            Command.Parameters.Add(parOff)

            Dim parAll As New SqlClient.SqlParameter("@IsAll", SqlDbType.Bit)
            parAll.Value = data.isAll
            Command.Parameters.Add(parAll)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in IO set up"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.telemetrySetUp", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

#End Region

#Region "Hour Meters"

    Public Function GetDevMeters(ByVal token As String, ByVal deviceId As String) As devInputsOnTime
        Dim itm As New devInputsOnTime
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_OnTime_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devInputsOnTime
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("Name")
                itm.ignitionOnTime = reader.Item("IgnitionOnTime")
                itm.ignitionLastSetOn = reader.Item("IgnitionLastSetOn").ToString
                itm.input1Name = reader.Item("Input1Name").ToString
                itm.input1OnTime = reader.Item("Input1OnTime")
                itm.input1LastSetOn = reader.Item("Input1LastSetOn").ToString
                itm.input2Name = reader.Item("Input2Name").ToString
                itm.input2OnTime = reader.Item("Input2OnTime")
                itm.input2LastSetOn = reader.Item("Input2LastSetOn").ToString
                itm.input3Name = reader.Item("Input3Name").ToString
                itm.input3OnTime = reader.Item("Input3OnTime")
                itm.input3LastSetOn = reader.Item("Input3LastSetOn").ToString
                itm.input4Name = reader.Item("Input4Name").ToString
                itm.input4OnTime = reader.Item("Input4OnTime")
                itm.input4LastSetOn = reader.Item("Input4LastSetOn").ToString
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function GetAllDevMeters(ByVal token As String) As List(Of devInputsOnTime)
        Dim lst As New List(Of devInputsOnTime)
        Dim itm As New devInputsOnTime
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_IOs_OnTime_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = "0"
            Command.Parameters.Add(parDeviceID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New devInputsOnTime
                itm.deviceId = reader.Item("DeviceID")
                itm.name = reader.Item("Name")
                itm.ignitionOnTime = reader.Item("IgnitionOnTime")
                itm.ignitionLastSetOn = reader.Item("IgnitionLastSetOn").ToString
                itm.input1Name = reader.Item("Input1Name").ToString
                itm.input1OnTime = reader.Item("Input1OnTime")
                itm.input1LastSetOn = reader.Item("Input1LastSetOn").ToString
                itm.input2Name = reader.Item("Input2Name").ToString
                itm.input2OnTime = reader.Item("Input2OnTime")
                itm.input2LastSetOn = reader.Item("Input2LastSetOn").ToString
                itm.input3Name = reader.Item("Input3Name").ToString
                itm.input3OnTime = reader.Item("Input3OnTime")
                itm.input3LastSetOn = reader.Item("Input3LastSetOn").ToString
                itm.input4Name = reader.Item("Input4Name").ToString
                itm.input4OnTime = reader.Item("Input4OnTime")
                itm.input4LastSetOn = reader.Item("Input4LastSetOn").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function saveHourMeter(token As String, data As devInputsOnTimeTransformed, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "Devices_IOs_OnTime_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parIgnitionOnTime As New SqlClient.SqlParameter("@IgnitionOnTime", SqlDbType.Int)
            parIgnitionOnTime.Value = data.ignitionOnTime
            Command.Parameters.Add(parIgnitionOnTime)

            Dim parIgnitionSetOn As New SqlClient.SqlParameter("@IgnitionSetOn", SqlDbType.DateTime)
            parIgnitionSetOn.Value = data.ignitionSetOn
            Command.Parameters.Add(parIgnitionSetOn)

            Dim parInput1OnTime As New SqlClient.SqlParameter("@Input1OnTime", SqlDbType.Int)
            parInput1OnTime.Value = data.input1OnTime
            Command.Parameters.Add(parInput1OnTime)

            Dim parInput1SetOn As New SqlClient.SqlParameter("@Input1SetOn", SqlDbType.DateTime)
            parInput1SetOn.Value = data.input1SetOn
            Command.Parameters.Add(parInput1SetOn)

            Dim parInput2OnTime As New SqlClient.SqlParameter("@Input2OnTime", SqlDbType.Int)
            parInput2OnTime.Value = data.input2OnTime
            Command.Parameters.Add(parInput2OnTime)

            Dim parInput2SetOn As New SqlClient.SqlParameter("@Input2SetOn", SqlDbType.DateTime)
            parInput2SetOn.Value = data.input2SetOn
            Command.Parameters.Add(parInput2SetOn)

            Dim parInput3OnTime As New SqlClient.SqlParameter("@Input3OnTime", SqlDbType.Int)
            parInput3OnTime.Value = data.input3OnTime
            Command.Parameters.Add(parInput3OnTime)

            Dim parInput3SetOn As New SqlClient.SqlParameter("@Input3SetOn", SqlDbType.DateTime)
            parInput3SetOn.Value = data.input3SetOn
            Command.Parameters.Add(parInput3SetOn)

            Dim parInput4OnTime As New SqlClient.SqlParameter("@Input4OnTime", SqlDbType.Int)
            parInput4OnTime.Value = data.input4OnTime
            Command.Parameters.Add(parInput4OnTime)

            Dim parInput4SetOn As New SqlClient.SqlParameter("@Input4SetOn", SqlDbType.DateTime)
            parInput4SetOn.Value = data.input4SetOn
            Command.Parameters.Add(parInput4SetOn)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in Hour Meter set up"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.saveHourMeter", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

#End Region

#Region "Geofences"

    Public Function getGeofencesCustomMessages(ByVal token As String, ByVal msgType As Integer) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList = Nothing

        Try
            strCommand = "Geofences_CustomMessages_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parMsgType As New SqlClient.SqlParameter("@MsgType", SqlDbType.Int)
            parMsgType.Value = msgType
            Command.Parameters.Add(parMsgType)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("id")
                itm.name = reader.Item("Message")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return lst

    End Function

    Public Function saveGeofence(ByVal token As String,
                                 ByVal id As String,
                                 ByVal GeofenceTypeId As Integer,
                                 ByVal name As String,
                                 ByVal contactName As String,
                                 ByVal phone As String,
                                 ByVal contactEmail As String,
                                 ByVal contactSMSAlert As Boolean,
                                 ByVal contactEmailAlert As Boolean,
                                 ByVal contactAlertTypeId As Integer,
                                 ByVal fullAddress As String,
                                 ByVal street As String,
                                 ByVal streetNumber As String,
                                 ByVal route As String,
                                 ByVal suite As String,
                                 ByVal city As String,
                                 ByVal county As String,
                                 ByVal state As String,
                                 ByVal postalCode As String,
                                 ByVal country As String,
                                 ByVal lat As Decimal,
                                 ByVal lng As Decimal,
                                 ByVal GeofenceAlertTypeID As Integer,
                                 ByVal radius As Integer,
                                 ByVal comments As String,
                                 ByVal shapeId As Integer,
                                 ByVal jsonPolyVerticesTXT As String,
                                 ByVal KMLData As String,
                                 ByVal SQLData As String,
                                 ByVal isSpeedLimit As Boolean,
                                 ByVal speedLimit As Integer,
                                 ByVal arrivalMsgId As Integer,
                                 ByVal arrivalMsgTxt As String,
                                 ByVal departureMsgId As Integer,
                                 ByVal departureMsgTxt As String,
                                 ByVal isStopForJob As Boolean,
                                 ByRef strError As String) As String
        Dim GUID As String = ""

        Try
            'strCommand = "Geofences_INSERT"
            strCommand = "Geofences_INSERT_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parGeofenceTypeID As New SqlClient.SqlParameter("@GeofenceTypeID", SqlDbType.Int)
            parGeofenceTypeID.Value = GeofenceTypeId
            Command.Parameters.Add(parGeofenceTypeID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 20)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parContactName As New SqlClient.SqlParameter("@ContactName", SqlDbType.NVarChar, 50)
            parContactName.Value = contactName
            Command.Parameters.Add(parContactName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parContactEmail As New SqlClient.SqlParameter("@ContactEmail", SqlDbType.NVarChar, 100)
            parContactEmail.Value = contactEmail
            Command.Parameters.Add(parContactEmail)

            Dim parContactSMSAlert As New SqlClient.SqlParameter("@ContactSMSAlert", SqlDbType.Bit)
            parContactSMSAlert.Value = contactSMSAlert
            Command.Parameters.Add(parContactSMSAlert)

            Dim parContactEmailAlert As New SqlClient.SqlParameter("@ContactEmailAlert", SqlDbType.Bit)
            parContactEmailAlert.Value = contactEmailAlert
            Command.Parameters.Add(parContactEmailAlert)

            Dim parContactAlertTypeID As New SqlClient.SqlParameter("@ContactAlertTypeID", SqlDbType.Int)
            parContactAlertTypeID.Value = contactAlertTypeId
            Command.Parameters.Add(parContactAlertTypeID)

            Dim parFullAddress As New SqlClient.SqlParameter("@FullAddress", SqlDbType.NVarChar, 100)
            parFullAddress.Value = fullAddress
            Command.Parameters.Add(parFullAddress)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parStreetNumber As New SqlClient.SqlParameter("@StreetNumber", SqlDbType.NVarChar, 10)
            parStreetNumber.Value = streetNumber
            Command.Parameters.Add(parStreetNumber)

            Dim parRoute As New SqlClient.SqlParameter("@Route", SqlDbType.NVarChar, 50)
            parRoute.Value = route
            Command.Parameters.Add(parRoute)

            Dim parSuite As New SqlClient.SqlParameter("@Suite", SqlDbType.NVarChar, 20)
            parSuite.Value = suite
            Command.Parameters.Add(parSuite)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parCounty As New SqlClient.SqlParameter("@County", SqlDbType.NVarChar, 50)
            parCounty.Value = county
            Command.Parameters.Add(parCounty)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 15)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountry As New SqlClient.SqlParameter("@Country", SqlDbType.NVarChar, 20)
            parCountry.Value = country
            Command.Parameters.Add(parCountry)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = lng
            Command.Parameters.Add(parLongitude)

            Dim parGeofenceAlertTypeID As New SqlClient.SqlParameter("@GeofenceAlertTypeID", SqlDbType.Int)
            parGeofenceAlertTypeID.Value = GeofenceAlertTypeID
            Command.Parameters.Add(parGeofenceAlertTypeID)

            Dim parRadius As New SqlClient.SqlParameter("@RadiusFeet", SqlDbType.Int)
            parRadius.Value = radius
            Command.Parameters.Add(parRadius)

            Dim parComments As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, 4000)
            parComments.Value = comments
            Command.Parameters.Add(parComments)

            Dim parShapeId As New SqlClient.SqlParameter("@ShapeId", SqlDbType.Int)
            parShapeId.Value = shapeId
            Command.Parameters.Add(parShapeId)

            Dim parVertices As New SqlClient.SqlParameter("@jsonPolyVerticesTXT", SqlDbType.NVarChar)
            parVertices.Value = jsonPolyVerticesTXT
            Command.Parameters.Add(parVertices)

            Dim parKMLData As New SqlClient.SqlParameter("@KMLData", SqlDbType.NVarChar)
            parKMLData.Value = KMLData
            Command.Parameters.Add(parKMLData)

            Dim parSQLData As New SqlClient.SqlParameter("@SQLData", SqlDbType.NVarChar)
            parSQLData.Value = SQLData
            Command.Parameters.Add(parSQLData)

            Dim parIsSpeedLimit As New SqlClient.SqlParameter("@IsSpeedLimit", SqlDbType.Bit)
            parIsSpeedLimit.Value = isSpeedLimit
            Command.Parameters.Add(parIsSpeedLimit)

            Dim parSpeedLimit As New SqlClient.SqlParameter("@SpeedLimit", SqlDbType.Int)
            parSpeedLimit.Value = speedLimit
            Command.Parameters.Add(parSpeedLimit)

            Dim parArrivalMsgId As New SqlClient.SqlParameter("@ArrivalMsgId", SqlDbType.Int)
            parArrivalMsgId.Value = arrivalMsgId
            Command.Parameters.Add(parArrivalMsgId)

            Dim parDepartureMsgId As New SqlClient.SqlParameter("@DepartureMsgId", SqlDbType.Int)
            parDepartureMsgId.Value = departureMsgId
            Command.Parameters.Add(parDepartureMsgId)

            Dim parArrivalMsgTxt As New SqlClient.SqlParameter("@ArrivalMsgTxt", SqlDbType.NVarChar)
            parArrivalMsgTxt.Value = arrivalMsgTxt
            Command.Parameters.Add(parArrivalMsgTxt)

            Dim parDepartureMsgTxt As New SqlClient.SqlParameter("@DepartureMsgTxt", SqlDbType.NVarChar)
            parDepartureMsgTxt.Value = departureMsgTxt
            Command.Parameters.Add(parDepartureMsgTxt)

            Dim parIsStopForJob As New SqlClient.SqlParameter("@IsStopForJob", SqlDbType.Bit)
            parIsStopForJob.Value = isStopForJob
            Command.Parameters.Add(parIsStopForJob)

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            GUID = parGUID.Value

        Catch ex As Exception
            strError = "Error saving Geofence"
            BLErrorHandling.ErrorCapture(pSysModule, "saveGeofence", "", ex.Message & " - Token: " & token, 0)
            GUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return GUID

    End Function

    Public Function isPointInGeofence(ByVal token As String, ByVal lat As Decimal, ByVal lng As Decimal, ByRef msg As String) As pointInGeofence
        Dim res As New pointInGeofence

        Try
            strCommand = "Geofences_ValidatePointInside"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@APIToken", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLat As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            parLat.Value = lat
            Command.Parameters.Add(parLat)

            Dim parLng As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            parLng.Value = lng
            Command.Parameters.Add(parLng)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                res.isInside = True
                res.geofenceId = reader.Item("GUID")
                res.geofenceName = reader.Item("Name")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return res

    End Function

    Public Function geofence_validateName(ByVal token As String, ByVal id As String, ByVal name As String) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Geofences_INSERT"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 20)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, 100)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)
            res.msg = CStr(parMsg.Value)

        Catch ex As Exception
            res.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If

        End Try

        Return res

    End Function

#End Region

#Region "Geofence Types"

    Public Function getAllGeofenceTypes(ByVal token As String) As List(Of GeofenceType)
        Dim lst As New List(Of GeofenceType)
        Dim itm As New GeofenceType
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "GeofencesTypes_GetByCompanyID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New GeofenceType
                itm.companyId = reader.Item("companyId")
                itm.id = reader.Item("id")
                itm.name = reader.Item("Name")
                itm.iconId = reader.Item("IconID")
                itm.iconUrl = reader.Item("IconURL")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function saveGeofenceType(token As String, data As GeofenceType, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "GeofencesTypes_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = data.id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parIconID As New SqlClient.SqlParameter("@IconID", SqlDbType.Int)
            parIconID.Value = data.iconId
            Command.Parameters.Add(parIconID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in saveGeofenceType"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.saveGeofenceType", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

    Public Function deleteGeofenceType(ByVal token As String, ByVal id As Integer, ByRef msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "GeofencesTypes_DELETE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in deleteGeofenceType"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.deleteGeofenceType", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

#End Region

#Region "Maintenance module"

    Public Function getMaintSchedules(ByVal Token As String, ByVal deviceId As String, ByVal taskId As Integer, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_Schedules_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int, 4)
            parTaskID.Direction = ParameterDirection.Input
            parTaskID.Value = taskId
            Command.Parameters.Add(parTaskID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getMaintenanceTasks(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_Tasks_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting maintenance tasks"
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintenanceTasks", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function getHServices(ByVal Token As String, ByVal DeviceID As String, ByVal TaskID As Integer, ByVal dateFrom As Date, ByVal dateTo As Date, ByRef errMsg As String) As String
        Dim strJson As String

        Try
            strCommand = "Maint_ServicesLog_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = DeviceID
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int)
            parTaskID.Value = TaskID
            Command.Parameters.Add(parTaskID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.Date)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.Date)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("services")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("deviceName")
                jsonLine.WriteValue(reader("DeviceName"))

                jsonLine.WritePropertyName("serviceDateString")
                jsonLine.WriteValue(BLCommon.getDateAsString(reader("ServiceDate")))

                jsonLine.WritePropertyName("serviceType")
                jsonLine.WriteValue(reader("ServiceType"))

                jsonLine.WritePropertyName("taskName")
                jsonLine.WriteValue(reader("TaskName"))

                jsonLine.WritePropertyName("odometer")
                jsonLine.WriteValue(reader("Odometer"))

                jsonLine.WritePropertyName("cost")
                jsonLine.WriteValue(reader("TotalCost"))

                jsonLine.WritePropertyName("comments")
                jsonLine.WriteValue(reader("Comments"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getHFuel(ByVal Token As String, ByVal DeviceID As String, ByVal dateFrom As Date, ByVal dateTo As Date, ByRef errMsg As String) As String
        Dim strJson As String

        Try
            strCommand = "Maint_FuelLog_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = DeviceID
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.Date)
            parDateFrom.Value = dateFrom
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.Date)
            parDateTo.Value = dateTo
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("fueling")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("deviceName")
                jsonLine.WriteValue(reader("DeviceName"))

                jsonLine.WritePropertyName("fuelDateString")
                jsonLine.WriteValue(BLCommon.getDateAsString(reader("FuelDate")))

                jsonLine.WritePropertyName("odometer")
                jsonLine.WriteValue(reader("Odometer"))

                jsonLine.WritePropertyName("gallons")
                jsonLine.WriteValue(reader("Gallons"))

                jsonLine.WritePropertyName("cost")
                jsonLine.WriteValue(reader("TotalCost"))

                jsonLine.WritePropertyName("stateName")
                jsonLine.WriteValue(reader("StateName"))

                jsonLine.WritePropertyName("comments")
                jsonLine.WriteValue(reader("Comments"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getMaintServicesTypes(ByVal token As String, ByRef strError As String) As String
        Dim strJson As String = ""

        Try
            strCommand = "Maint_ServicesTypes_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            'Build json array
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            Dim json As New Newtonsoft.Json.JsonTextWriter(sw)


            Dim sbLine As StringBuilder = Nothing
            Dim swLine As StringWriter = Nothing
            Dim jsonLine As Newtonsoft.Json.JsonTextWriter = Nothing

            json.WriteStartObject()
            json.WritePropertyName("types")
            json.WriteStartArray()

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                sbLine = New StringBuilder
                swLine = New StringWriter(sbLine)
                jsonLine = New Newtonsoft.Json.JsonTextWriter(swLine)

                jsonLine.WriteStartObject()

                jsonLine.WritePropertyName("id")
                jsonLine.WriteValue(reader("ID"))

                jsonLine.WritePropertyName("name")
                jsonLine.WriteValue(reader("Name"))

                jsonLine.WriteEndObject()
                jsonLine.Flush()

                json.WriteValue(sbLine.ToString())
            Loop

            json.WriteEnd()
            json.WriteEndObject()
            json.Flush()
            strJson = sb.ToString

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            strError = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return strJson

    End Function

    Public Function getMaintTasksMeassures(ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_TasksMeassures_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting maintenance tasks meassures"
            BLErrorHandling.ErrorCapture(pSysModule, "getMaintTasksMeassures", "", ex.Message, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function removeTask(ByVal token As String, ByVal id As Integer, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_Tasks_Remove"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error removing Task"
            BLErrorHandling.ErrorCapture(pSysModule, "removeTask", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function saveMaintTask(ByVal token As String, ByVal id As Integer, ByVal name As String, ByVal meassureId As Integer, ByVal value As Decimal, ByVal dtUsers As DataTable, ByRef strError As String) As String
        Dim taskId As Integer = 0

        Try
            strCommand = "Maint_Tasks_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = name
            Command.Parameters.Add(parName)

            Dim parMeassureId As New SqlClient.SqlParameter("@MeassureId", SqlDbType.Int)
            parMeassureId.Value = meassureId
            Command.Parameters.Add(parMeassureId)

            Dim parValue As New SqlClient.SqlParameter("@Value", SqlDbType.Real)
            parValue.Value = value
            Command.Parameters.Add(parValue)

            Dim parUsers As New SqlClient.SqlParameter("@UsersLst", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parTaskId As New SqlClient.SqlParameter("@TaskId", SqlDbType.Int)
            parTaskId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parTaskId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            taskId = CInt(parTaskId.Value)

        Catch ex As Exception
            strError = "Error saving task"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintTask", "", ex.Message & " - Token: " & token, 0)
            taskId = 0
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return taskId

    End Function

    Public Function saveMaintSchedule(ByVal token As String, ByVal id As String, ByVal deviceId As String, ByVal taskId As Integer, ByVal taskValue As Decimal, ByVal lastServiceOn As Date,
                                      ByVal currentValue As Decimal, ByVal notifyBefore As Decimal, ByVal notifyEveryXDays As Integer, ByVal excludeWeekends As Boolean,
                                      ByVal dtUsers As DataTable, ByRef strError As String) As String
        Dim scheduleGUID As String = ""

        Try
            strCommand = "Maint_Schedules_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int)
            parTaskID.Value = taskId
            Command.Parameters.Add(parTaskID)

            Dim parTaskValue As New SqlClient.SqlParameter("@TaskValue", SqlDbType.Decimal)
            parTaskValue.Value = taskValue
            Command.Parameters.Add(parTaskValue)

            Dim parLastServiceOn As New SqlClient.SqlParameter("@LastServiceOn", SqlDbType.Date)
            parLastServiceOn.Value = lastServiceOn
            Command.Parameters.Add(parLastServiceOn)

            Dim parCurrentValue As New SqlClient.SqlParameter("@CurrentValue", SqlDbType.Decimal)
            parCurrentValue.Value = currentValue
            Command.Parameters.Add(parCurrentValue)

            Dim parNotifyBefore As New SqlClient.SqlParameter("@NotifyBefore", SqlDbType.Decimal)
            parNotifyBefore.Value = notifyBefore
            Command.Parameters.Add(parNotifyBefore)

            Dim parNotifyEveryXDays As New SqlClient.SqlParameter("@NotifyEveryXDays", SqlDbType.Int)
            parNotifyEveryXDays.Value = notifyEveryXDays
            Command.Parameters.Add(parNotifyEveryXDays)

            Dim parExcludeWeekends As New SqlClient.SqlParameter("@ExcludeWeekends", SqlDbType.Bit)
            parExcludeWeekends.Value = excludeWeekends
            Command.Parameters.Add(parExcludeWeekends)

            Dim parUsers As New SqlClient.SqlParameter("@UsersLst", SqlDbType.Structured)
            parUsers.Value = dtUsers
            Command.Parameters.Add(parUsers)

            Dim parScheduleGUID As New SqlClient.SqlParameter("@ScheduleGUID", SqlDbType.NVarChar, 50)
            parScheduleGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parScheduleGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            scheduleGUID = CStr(parScheduleGUID.Value)

        Catch ex As Exception
            strError = "Error saving schedule"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintSchedule", "", ex.Message & " - Token: " & token, 0)
            scheduleGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return scheduleGUID

    End Function

    Public Function getMaintAlertUsers(ByVal Token As String, ByVal ID As String, ByRef errMsg As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Maint_SchedulesSendTo_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parID.Direction = ParameterDirection.Input
            parID.Value = ID
            Command.Parameters.Add(parID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            errMsg = ex.Message
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function saveMaintServiceLog(ByVal token As String, ByVal id As String, ByVal deviceId As String, ByVal ServiceTypeID As Integer, ByVal taskId As Integer,
                                        ByVal ServiceDescription As String, ByVal serviceDate As Date, ByVal odometer As Decimal, ByVal cost As Decimal, ByVal meassureValueOnDayOfService As Decimal,
                                        ByVal comments As String,
                                        ByRef strError As String) As String
        Dim serviceLogGUID As String = ""

        Try
            strCommand = "Maint_ServicesLog_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parServiceTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parServiceTypeID.Value = ServiceTypeID
            Command.Parameters.Add(parServiceTypeID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.Int)
            parTaskID.Value = taskId
            Command.Parameters.Add(parTaskID)

            Dim parServiceDescription As New SqlClient.SqlParameter("@ServiceDescription", SqlDbType.NVarChar, -1)
            parServiceDescription.Value = ServiceDescription
            Command.Parameters.Add(parServiceDescription)

            Dim parServiceDate As New SqlClient.SqlParameter("@ServiceDate", SqlDbType.Date)
            parServiceDate.Value = serviceDate
            Command.Parameters.Add(parServiceDate)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parCost As New SqlClient.SqlParameter("@Cost", SqlDbType.Decimal)
            parCost.Value = cost
            Command.Parameters.Add(parCost)

            Dim parComments As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parComments.Value = comments
            Command.Parameters.Add(parComments)

            Dim parVal As New SqlClient.SqlParameter("@Val", SqlDbType.Decimal)
            parVal.Precision = 18
            parVal.Scale = 2
            parVal.Value = meassureValueOnDayOfService
            Command.Parameters.Add(parVal)

            Dim parServiceLogGUID As New SqlClient.SqlParameter("@ServiceLogGUID", SqlDbType.NVarChar, 50)
            parServiceLogGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parServiceLogGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            serviceLogGUID = CStr(parServiceLogGUID.Value)

        Catch ex As Exception
            strError = "Error saving service log"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintServiceLog", "", ex.Message & " - Token: " & token, 0)
            serviceLogGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return serviceLogGUID

    End Function

    Public Function deleteMaintServiceLog(ByVal token As String, ByVal id As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_ServicesLog_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Service Log"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteMaintServiceLog", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function saveMaintFuelLog(ByVal token As String, ByVal id As String, ByVal deviceId As String,
                                     ByVal fuelingDate As Date, ByVal odometer As Decimal, ByVal gallons As Decimal, ByVal cost As Decimal, ByVal stateId As Integer, ByVal comments As String,
                                     ByRef strError As String) As String
        Dim FuelLogGUID As String = ""

        Try
            strCommand = "Maint_fuelLog_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            'This ID is actually the GUID of the record.
            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parFuelingDate As New SqlClient.SqlParameter("@FuelingDate", SqlDbType.Date)
            parFuelingDate.Value = fuelingDate
            Command.Parameters.Add(parFuelingDate)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Value = odometer
            Command.Parameters.Add(parOdometer)

            Dim parGallons As New SqlClient.SqlParameter("@Gallons", SqlDbType.Decimal)
            parGallons.Value = gallons
            Command.Parameters.Add(parGallons)

            Dim parCost As New SqlClient.SqlParameter("@Cost", SqlDbType.Decimal)
            parCost.Value = cost
            Command.Parameters.Add(parCost)

            Dim parStateID As New SqlClient.SqlParameter("@StateID", SqlDbType.Int)
            parStateID.Value = stateId
            Command.Parameters.Add(parStateID)

            Dim parComments As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parComments.Value = comments
            Command.Parameters.Add(parComments)

            Dim parFuelLogGUID As New SqlClient.SqlParameter("@FuelLogGUID", SqlDbType.NVarChar, 50)
            parFuelLogGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parFuelLogGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            FuelLogGUID = CStr(parFuelLogGUID.Value)

        Catch ex As Exception
            strError = "Error saving fuel log"
            BLErrorHandling.ErrorCapture(pSysModule, "saveMaintFuelLog", "", ex.Message & " - Token: " & token, 0)
            FuelLogGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return FuelLogGUID

    End Function

    Public Function deleteMaintFuelLog(ByVal token As String, ByVal id As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_FuelLog_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Fuel Log"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteMaintFuelLog", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function deleteMaintSchedule(ByVal token As String, ByVal id As String, ByRef strError As String) As Boolean
        Dim bResults As Boolean = True

        Try
            strCommand = "Maint_Schedules_REMOVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Maintenance Schedule"
            BLErrorHandling.ErrorCapture(pSysModule, "deleteMaintSchedule", "", ex.Message & " - Token: " & token, 0)
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

#End Region

#Region "Maintenance"

    Public Function maintSupportListsGET(ByVal token As String) As maintSupportLists
        Dim lists As New maintSupportLists
        Dim reader As SqlDataReader = Nothing
        Dim itm As basicList


        Try
            strCommand = "Maint_SupportLists_Get"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            'SERVICES TYPES
            Dim servicesTypes As New List(Of basicList)
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                servicesTypes.Add(itm)
            Loop
            lists.servicesTypes = servicesTypes

            'TIME REFERENCES
            Dim timeReferences As New List(Of basicList)
            reader.NextResult()
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                timeReferences.Add(itm)
            Loop
            lists.timeReferences = timeReferences

            'VEHICLE TYPES
            Dim vehicleTypes As New List(Of basicList)
            reader.NextResult()
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                vehicleTypes.Add(itm)
            Loop
            lists.vehicleTypes = vehicleTypes

            'MAINTENANCE TASKS
            Dim task As maintTask
            Dim maintTasks As New List(Of maintTask)
            reader.NextResult()
            Do While reader.Read
                task = New maintTask
                task.id = reader.Item("ID")
                task.name = reader.Item("Name")
                task.meassureId = reader.Item("TaskMeassureID")
                task.value = reader.Item("Value")
                task.meassureName = reader.Item("MeassureName")
                maintTasks.Add(task)
            Loop
            lists.maintTasks = maintTasks

            'MAINTENANCE MEASSURES
            Dim meassure As maintMeassure
            Dim meassures As New List(Of maintMeassure)
            reader.NextResult()
            Do While reader.Read
                meassure = New maintMeassure
                meassure.id = reader.Item("ID")
                meassure.name = reader.Item("Name")
                meassure.unitName = reader.Item("UnitName")
                meassures.Add(meassure)
            Loop
            lists.maintMeassures = meassures

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lists

    End Function

    Public Function maintDeviceList(ByVal token As String) As List(Of maintDevice)
        Dim lst As New List(Of maintDevice)
        Dim itm As New maintDevice
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Maint_Devices_Get"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If


            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New maintDevice
                itm.id = reader.Item("id")
                itm.name = reader.Item("Name")
                itm.typeId = reader.Item("VehicleTypeID")
                itm.make = reader.Item("make")
                itm.model = reader.Item("model")
                itm.modelYear = reader.Item("modelYear")
                itm.insuranceCarrier = reader.Item("insuranceCarrier")
                itm.insurancePolicyNo = reader.Item("insurancePolicyNo")
                itm.insurancePremium = reader.Item("insurancePremium")
                itm.insuranceDueOn = reader.Item("insuranceDueOn")
                itm.odometer = reader.Item("odometer")
                itm.ignitionHours = reader.Item("IgnitionOnTime")
                itm.input1Name = reader.Item("input1Name")
                itm.input1Hours = reader.Item("Input1OnTime")
                itm.input2Name = reader.Item("input2Name")
                itm.input2Hours = reader.Item("Input2OnTime")
                itm.input3Name = reader.Item("input3Name")
                itm.input3Hours = reader.Item("Input3OnTime")
                itm.input4Name = reader.Item("input4Name")
                itm.input4Hours = reader.Item("Input4OnTime")
                itm.notes = reader.Item("MaintenanceNotes")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function maintDeviceSave(token As String, data As maintDevice, msg As String) As Boolean
        Dim result As Boolean = True

        Try
            strCommand = "Maint_Devices_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceGUID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.id
            Command.Parameters.Add(parDeviceID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parTypeID.Value = data.typeId
            Command.Parameters.Add(parTypeID)

            Dim parMake As New SqlClient.SqlParameter("@Make", SqlDbType.NVarChar, 50)
            parMake.Value = data.make
            Command.Parameters.Add(parMake)

            Dim parModel As New SqlClient.SqlParameter("@Model", SqlDbType.NVarChar, 50)
            parModel.Value = data.model
            Command.Parameters.Add(parModel)

            Dim parModelYear As New SqlClient.SqlParameter("@ModelYear", SqlDbType.Int)
            parModelYear.Value = data.modelYear
            Command.Parameters.Add(parModelYear)

            Dim parInsCarrier As New SqlClient.SqlParameter("@InsuranceCarrier", SqlDbType.NVarChar, 50)
            parInsCarrier.Value = data.insuranceCarrier
            Command.Parameters.Add(parInsCarrier)

            Dim parInsPolicy As New SqlClient.SqlParameter("@InsurancePolicyNo", SqlDbType.NVarChar, 50)
            parInsPolicy.Value = data.insurancePolicyNo
            Command.Parameters.Add(parInsPolicy)

            Dim parInsPremium As New SqlClient.SqlParameter("@InsurancePremium", SqlDbType.Decimal, 2)
            parInsPremium.Precision = 10
            parInsPremium.Scale = 2
            parInsPremium.Value = data.insurancePremium
            Command.Parameters.Add(parInsPremium)

            Dim parInsDueOn As New SqlClient.SqlParameter("@InsuranceDueOn", SqlDbType.Date)
            parInsDueOn.Value = data.datInsuranceDueOn
            Command.Parameters.Add(parInsDueOn)

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            parOdometer.Value = data.odometer
            Command.Parameters.Add(parOdometer)

            Dim parOdometerChanged As New SqlClient.SqlParameter("@OdometerChanged", SqlDbType.Bit)
            parOdometerChanged.Value = data.odometerChanged
            Command.Parameters.Add(parOdometerChanged)

            Dim parIgnitionTime As New SqlClient.SqlParameter("@IgnitionTime", SqlDbType.Int)
            parIgnitionTime.Value = data.ignitionHours * 3600
            Command.Parameters.Add(parIgnitionTime)

            Dim parIgnitionTimeChanged As New SqlClient.SqlParameter("@IgnitionTimeChanged", SqlDbType.Bit)
            parIgnitionTimeChanged.Value = data.ignitionHoursChanged
            Command.Parameters.Add(parIgnitionTimeChanged)

            Dim parInput1Name As New SqlClient.SqlParameter("@Input1Name", SqlDbType.NVarChar, 50)
            parInput1Name.Value = data.input1Name
            Command.Parameters.Add(parInput1Name)

            Dim parInput1OnTime As New SqlClient.SqlParameter("@Input1OnTime", SqlDbType.Int)
            parInput1OnTime.Value = data.input1Hours * 3600
            Command.Parameters.Add(parInput1OnTime)

            Dim parInput1OnTimeChanged As New SqlClient.SqlParameter("@Input1OnTimeChanged", SqlDbType.Bit)
            parInput1OnTimeChanged.Value = data.input1HoursChanged
            Command.Parameters.Add(parInput1OnTimeChanged)

            Dim parInput2Name As New SqlClient.SqlParameter("@Input2Name", SqlDbType.NVarChar, 50)
            parInput2Name.Value = data.input2Name
            Command.Parameters.Add(parInput2Name)

            Dim parInput2OnTime As New SqlClient.SqlParameter("@Input2OnTime", SqlDbType.Int)
            parInput2OnTime.Value = data.input2Hours * 3600
            Command.Parameters.Add(parInput2OnTime)

            Dim parInput2OnTimeChanged As New SqlClient.SqlParameter("@Input2OnTimeChanged", SqlDbType.Bit)
            parInput2OnTimeChanged.Value = data.input2HoursChanged
            Command.Parameters.Add(parInput2OnTimeChanged)

            Dim parInput3Name As New SqlClient.SqlParameter("@Input3Name", SqlDbType.NVarChar, 50)
            parInput3Name.Value = data.input3Name
            Command.Parameters.Add(parInput3Name)

            Dim parInput3OnTime As New SqlClient.SqlParameter("@Input3OnTime", SqlDbType.Int)
            parInput3OnTime.Value = data.input3Hours * 3600
            Command.Parameters.Add(parInput3OnTime)

            Dim parInput3OnTimeChanged As New SqlClient.SqlParameter("@Input3OnTimeChanged", SqlDbType.Bit)
            parInput3OnTimeChanged.Value = data.input3HoursChanged
            Command.Parameters.Add(parInput3OnTimeChanged)

            Dim parInput4Name As New SqlClient.SqlParameter("@Input4Name", SqlDbType.NVarChar, 50)
            parInput4Name.Value = data.input4Name
            Command.Parameters.Add(parInput4Name)

            Dim parInput4OnTime As New SqlClient.SqlParameter("@Input4OnTime", SqlDbType.Int)
            parInput4OnTime.Value = data.input4Hours * 3600
            Command.Parameters.Add(parInput4OnTime)

            Dim parInput4OnTimeChanged As New SqlClient.SqlParameter("@Input4OnTimeChanged", SqlDbType.Bit)
            parInput4OnTimeChanged.Value = data.input4HoursChanged
            Command.Parameters.Add(parInput4OnTimeChanged)

            Dim parNotes As New SqlClient.SqlParameter("@MaintenanceNotes", SqlDbType.VarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            result = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error Saving Device"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintDeviceSave", "", ex.Message & " - Token: " & token, 0)
            result = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function

    Public Function maintScheduleGetByDevice(ByVal token As String, ByVal deviceId As String, ByVal id As String) As maintDeviceSchedule
        Dim maintSchedule As New maintDeviceSchedule
        Dim schedules As New List(Of scheduleItem)
        Dim meassures As New List(Of maintMeassure)
        Dim itm As scheduleItem
        Dim itm2 As maintMeassure
        Dim reader As SqlDataReader = Nothing
        Dim var1 As Integer = 0

        Try
            strCommand = "Maint_Schedule_GetByDevice"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            maintSchedule.deviceId = deviceId

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New scheduleItem
                itm.id = reader.Item("ID")
                itm.taskId = reader.Item("TaskID")
                itm.taskName = reader.Item("TaskName")
                itm.repeatEveryX = reader.Item("RepeatEveryX")
                itm.repeatEveryTimeRefId = reader.Item("RepeatEveryTimeRefID")
                itm.meassureId = reader.Item("MeassureID")
                itm.meassureValue = reader.Item("MeassureValue")
                itm.frequency = reader.Item("FrequencyInWords")
                itm.taskValue = reader.Item("TaskValue")
                itm.lastServiceOn = reader.Item("LastServiceOn")
                itm.nextDue = reader.Item("NextDue")
                itm.nextMeassureValue = reader.Item("NextMeassureValue")
                itm.daysUntilDue = reader.Item("DaysUntilDue")
                itm.reminderTimeRefId = reader.Item("ReminderTimeRefId")
                itm.reminderTimeRefVal = reader.Item("ReminderTimeRefVal")
                itm.reminderMeassureVal = reader.Item("ReminderMeassureVal")
                itm.reminder = reader.Item("ReminderInWords")
                itm.notes = reader.Item("Notes")
                itm.nextDueInWords = ""
                schedules.Add(itm)
            Loop
            maintSchedule.schedules = schedules

            reader.NextResult()
            Do While reader.Read
                itm2 = New maintMeassure
                itm2.id = reader.Item("ID")
                itm2.name = reader.Item("Name")
                itm2.unitName = reader.Item("UnitName")
                meassures.Add(itm2)
            Loop
            maintSchedule.meassures = meassures

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return maintSchedule

    End Function

    Public Function maintItemSave(ByVal token As String, ByVal deviceId As String, ByVal data As scheduleItem, ByRef id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Maint_Schedules_SaveItem"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = data.id
            Command.Parameters.Add(parID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.NVarChar, 50)
            parTaskID.Value = data.taskId
            Command.Parameters.Add(parTaskID)

            Dim parRepeatEveryX As New SqlClient.SqlParameter("@RepeatEveryX", SqlDbType.Int)
            parRepeatEveryX.Value = data.repeatEveryX
            Command.Parameters.Add(parRepeatEveryX)

            Dim parRepeatEveryTimeRefID As New SqlClient.SqlParameter("@RepeatEveryTimeRefID", SqlDbType.Int)
            parRepeatEveryTimeRefID.Value = data.repeatEveryTimeRefId
            Command.Parameters.Add(parRepeatEveryTimeRefID)

            Dim parMeassureID As New SqlClient.SqlParameter("@MeassureID", SqlDbType.Int)
            parMeassureID.Value = data.meassureId
            Command.Parameters.Add(parMeassureID)

            Dim parMeassureValue As New SqlClient.SqlParameter("@MeassureValue", SqlDbType.Int)
            parMeassureValue.Value = data.meassureValue
            Command.Parameters.Add(parMeassureValue)

            Dim parTaskValue As New SqlClient.SqlParameter("@TaskValue", SqlDbType.Int)
            parTaskValue.Value = data.taskValue
            Command.Parameters.Add(parTaskValue)

            Dim parLastServiceOn As New SqlClient.SqlParameter("@LastServiceOn", SqlDbType.Date)
            parLastServiceOn.Value = data.lastServiceOn
            Command.Parameters.Add(parLastServiceOn)

            Dim parNextDue As New SqlClient.SqlParameter("@NextDue", SqlDbType.Date)
            parNextDue.Value = data.nextDue
            Command.Parameters.Add(parNextDue)

            Dim parNextMeassureValue As New SqlClient.SqlParameter("@NextMeassureValue", SqlDbType.Int)
            parNextMeassureValue.Value = data.nextMeassureValue
            Command.Parameters.Add(parNextMeassureValue)

            Dim parReminderTimeRefId As New SqlClient.SqlParameter("@ReminderTimeRefId", SqlDbType.Int)
            parReminderTimeRefId.Value = data.reminderTimeRefId
            Command.Parameters.Add(parReminderTimeRefId)

            Dim parReminderTimeRefVal As New SqlClient.SqlParameter("@ReminderTimeRefVal", SqlDbType.Int)
            parReminderTimeRefVal.Value = data.reminderTimeRefVal
            Command.Parameters.Add(parReminderTimeRefVal)

            Dim parReminderMeassureVal As New SqlClient.SqlParameter("@ReminderMeassureVal", SqlDbType.Int)
            parReminderMeassureVal.Value = data.reminderMeassureVal
            Command.Parameters.Add(parReminderMeassureVal)

            Dim parValueSinceLastService As New SqlClient.SqlParameter("@ValueSinceLastService", SqlDbType.Decimal)
            parValueSinceLastService.Value = data.valueSinceLastService
            Command.Parameters.Add(parValueSinceLastService)

            Dim parNotes As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parNewID As New SqlClient.SqlParameter("@NewID", SqlDbType.VarChar, 50)
            parNewID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewID)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CBool(parIsOk.Value)
            id = CStr(parNewID.Value)
            msg = CStr(parMsg.Value)

        Catch ex As Exception
            strError = "Error Saving Schedule Item"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintItemSave", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function maintItemDelete(ByVal token As String, ByVal id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Maint_Schedules_REMOVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error deleting Schedule Item"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintItemDelete", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function maintCompletedItemSave_DEPRECATED(ByVal token As String, ByVal deviceId As String, ByVal data As scheduleItem, ByRef id As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            'IMPORTANT: THIS CALL TO THIS SP IS INCOMPLETE.  THE PARAM "VAL" IS NOT BEING PASSED AND IT IS VERY IMPORTANT.
            strCommand = "Maint_ServicesLog_SAVE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            '@Comments NVARCHAR(MAX) = '',
            '@ServiceLogGUID NVARCHAR(50) = '' OUTPUT

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = data.jobId
            Command.Parameters.Add(parID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTypeID As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parTypeID.Value = data.jobTypeId
            Command.Parameters.Add(parTypeID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.NVarChar, 50)
            parTaskID.Value = data.taskId
            Command.Parameters.Add(parTaskID)

            Dim parJobDesc As New SqlClient.SqlParameter("@ServiceDescription", SqlDbType.NVarChar, -1)
            parJobDesc.Value = data.jobDescription
            Command.Parameters.Add(parJobDesc)

            Dim parJobDate As New SqlClient.SqlParameter("@ServiceDate", SqlDbType.Date)
            parJobDate.Value = data.completedOn
            Command.Parameters.Add(parJobDate)

            '@Val DECIMAL(18,2) = 0,

            Dim parOdometer As New SqlClient.SqlParameter("@Odometer", SqlDbType.Decimal)
            parOdometer.Precision = 18
            parOdometer.Scale = 2
            parOdometer.Value = data.odometer
            Command.Parameters.Add(parOdometer)

            Dim parJobCost As New SqlClient.SqlParameter("@Cost", SqlDbType.Decimal)
            parJobCost.Precision = 18
            parJobCost.Scale = 2
            parJobCost.Value = data.cost
            Command.Parameters.Add(parJobCost)

            Dim val As Decimal

            'Dim parIgnition As New SqlClient.SqlParameter("@IgnitionOnTime", SqlDbType.Int)
            'parIgnition.Value = data.ignitionHours
            'Command.Parameters.Add(parIgnition)

            'Dim parInput1 As New SqlClient.SqlParameter("@Input1OnTime", SqlDbType.Int)
            'parInput1.Value = data.input1Hours
            'Command.Parameters.Add(parInput1)

            'Dim parInput2 As New SqlClient.SqlParameter("@Input2OnTime", SqlDbType.Int)
            'parInput2.Value = data.input2Hours
            'Command.Parameters.Add(parInput2)

            'Dim parInput3 As New SqlClient.SqlParameter("@Input3OnTime", SqlDbType.Int)
            'parInput3.Value = data.input3Hours
            'Command.Parameters.Add(parInput3)

            'Dim parInput4 As New SqlClient.SqlParameter("@Input4OnTime", SqlDbType.Int)
            'parInput4.Value = data.input4Hours
            'Command.Parameters.Add(parInput4)

            Dim parNotes As New SqlClient.SqlParameter("@Comments", SqlDbType.NVarChar, -1)
            parNotes.Value = data.notes
            Command.Parameters.Add(parNotes)

            Dim parNewID As New SqlClient.SqlParameter("@ServiceLogGUID", SqlDbType.VarChar, 50)
            parNewID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewID)

            'Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.VarChar, -1)
            'parMsg.Direction = ParameterDirection.Output
            'Command.Parameters.Add(parMsg)

            'Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            'parIsOk.Direction = ParameterDirection.Output
            'Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            'bResult = CBool(parIsOk.Value)
            id = CStr(parNewID.Value)
            'msg = CStr(parMsg.Value)

        Catch ex As Exception
            strError = "Error Saving Schedule Item"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.maintItemSave", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

    Public Function maintLogGetByDevice(ByVal token As String, ByVal deviceId As String, ByVal id As String) As maintDeviceLog
        Dim maintLog As New maintDeviceLog
        Dim logs As New List(Of scheduleItem)
        Dim itm As scheduleItem
        Dim reader As SqlDataReader = Nothing
        Dim var1 As Integer = 0

        Try
            strCommand = "Maint_ServicesLog_GetByDevice"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parTaskID As New SqlClient.SqlParameter("@TaskID", SqlDbType.NVarChar, 50)
            parTaskID.Value = ""
            Command.Parameters.Add(parTaskID)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 50)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            maintLog.deviceId = deviceId

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New scheduleItem
                itm.id = reader.Item("ID")
                itm.taskId = reader.Item("TaskID")
                itm.taskName = reader.Item("TaskName")
                itm.jobDescription = reader.Item("ServiceDescription")
                itm.completedOn = reader.Item("ServiceDate")
                itm.cost = reader.Item("TotalCost")
                itm.odometer = reader.Item("Odometer")
                itm.ignitionHours = reader.Item("IgnitionOnTime")
                itm.input1Hours = reader.Item("Input1OnTime")
                itm.input2Hours = reader.Item("Input2OnTime")
                itm.input3Hours = reader.Item("Input3OnTime")
                itm.input4Hours = reader.Item("Input4OnTime")
                itm.notes = reader.Item("Comments")
                logs.Add(itm)
            Loop
            maintLog.logs = logs

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return maintLog

    End Function

#End Region

#Region "QB Match"

    Public Function QBMatch_CRMCustomers(ByVal token As String, ByRef isValidRequest As Boolean) As List(Of crmCustomer)
        Dim lst As New List(Of crmCustomer)
        Dim reader As SqlDataReader = Nothing
        Dim itm As crmCustomer

        Try
            strCommand = "CRM_QB_CustomersGET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                itm = New crmCustomer
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.qbId = reader.Item("QB_ListID")
                itm.dealerId = reader.Item("DealerID")
                itm.dealerName = reader.Item("DealerName")
                itm.isMatched = reader.Item("IsMatched")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            isValidRequest = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If

            If lst.Count > 0 Then
                isValidRequest = True
            End If

        End Try

        Return lst

    End Function

    Public Function QBMatch_QBCustomers() As List(Of qbCustomer)
        Dim lst As New List(Of qbCustomer)
        Dim reader As SqlDataReader = Nothing
        Dim itm As qbCustomer

        Try
            strCommand = "CRM_QB_CustomersGET"
            conString = ConfigurationManager.AppSettings("QBDB")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                itm = New qbCustomer
                itm.id = reader.Item("ID")
                itm.companyName = reader.Item("CompanyName")
                itm.contactName = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Public Function qbLinkCustomers(ByVal token As String, ByVal crmId As String, ByVal qbId As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "CRM_QB_CustomersLink"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCRMId As New SqlClient.SqlParameter("@CRMID", SqlDbType.NVarChar, 50)
            parCRMId.Value = crmId
            Command.Parameters.Add(parCRMId)

            Dim parQBId As New SqlClient.SqlParameter("@QBID", SqlDbType.NVarChar, 50)
            parQBId.Value = qbId
            Command.Parameters.Add(parQBId)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CStr(parIsOk.Value)

        Catch ex As Exception
            strError = "Error in qbLinkCustomers"
            BLErrorHandling.ErrorCapture(pSysModule, "DL.qbLinkCustomers", "", ex.Message & " - Token: " & token, 0)
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "User Preferences"

    Public Function updateUserPref(ByVal token As String, ByVal data As userPreference) As Boolean
        Dim bOk As Boolean = True

        Try
            strCommand = "UserPreference_UPDATE"

            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parModuleName As New SqlClient.SqlParameter("@ModuleName", SqlDbType.NVarChar, 50)
            parModuleName.Value = data.moduleName
            Command.Parameters.Add(parModuleName)

            Dim parPreference As New SqlClient.SqlParameter("@Preference", SqlDbType.NVarChar, 50)
            parPreference.Value = data.preference
            Command.Parameters.Add(parPreference)

            Dim parVal1 As New SqlClient.SqlParameter("@Val1", SqlDbType.NVarChar, 50)
            parVal1.Value = data.val1
            Command.Parameters.Add(parVal1)

            Dim parVal2 As New SqlClient.SqlParameter("@Val2", SqlDbType.NVarChar, 50)
            parVal2.Value = data.val2
            Command.Parameters.Add(parVal2)

            Dim parVal3 As New SqlClient.SqlParameter("@Val3", SqlDbType.NVarChar, 10)
            parVal3.Value = data.val3
            Command.Parameters.Add(parVal3)

            Dim parlat As New SqlClient.SqlParameter("@lat", SqlDbType.Decimal)
            parlat.Value = data.lat
            Command.Parameters.Add(parlat)

            Dim parlng As New SqlClient.SqlParameter("@lng", SqlDbType.Decimal)
            parlng.Value = data.lng
            Command.Parameters.Add(parlng)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bOk = CBool(parIsOk.Value)

        Catch ex As Exception
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function
    Public Function updateInactivePreferences(ByVal token As String, ByVal data As String) As Boolean
        Dim bOk As Boolean = True
        Try
            strCommand = "updateInactivePreferences"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parModuleName As New SqlClient.SqlParameter("@ModuleName", SqlDbType.NVarChar, 50)
            parModuleName.Value = "MULTITRAKING"
            Command.Parameters.Add(parModuleName)

            Dim parGroupsID As New SqlClient.SqlParameter("@GroupsID", SqlDbType.NVarChar, 50)
            parGroupsID.Value = data
            Command.Parameters.Add(parGroupsID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bOk = CBool(parIsOk.Value)

        Catch ex As Exception
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

#End Region

#Region "CRM Related"

    Function EmailTypesGET(ByVal token As String, ByVal type As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_EmailTypes_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parType As New SqlClient.SqlParameter("@Type", SqlDbType.NVarChar, 50)
            parType.Direction = ParameterDirection.Input
            parType.Value = type
            Command.Parameters.Add(parType)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Function GenericMastersGET(ByVal token As String, ByVal masterKey As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_GenericMasters_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parMasterKey As New SqlClient.SqlParameter("@MasterKey", SqlDbType.NVarChar, 50)
            parMasterKey.Direction = ParameterDirection.Input
            parMasterKey.Value = masterKey
            Command.Parameters.Add(parMasterKey)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("ID")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Public Sub sendEmail(ByVal token As String, ByVal custId As String, ByVal emailTypeId As String, ByVal courrierId As String, ByVal trackingNumber As String)
        Try
            strCommand = "CRM_SendEmail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parEmailTypeId As New SqlClient.SqlParameter("@EmailTypeId", SqlDbType.Int)
            If IsNumeric(emailTypeId) Then
                parEmailTypeId.Value = emailTypeId
            Else
                parEmailTypeId.Value = 0
            End If
            Command.Parameters.Add(parEmailTypeId)

            Dim parCustID As New SqlClient.SqlParameter("@CustID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            Dim parCourrierId As New SqlClient.SqlParameter("@CourrierId", SqlDbType.NVarChar, 50)
            parCourrierId.Value = courrierId
            Command.Parameters.Add(parCourrierId)

            Dim parTrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 50)
            parTrackingNumber.Value = trackingNumber
            Command.Parameters.Add(parTrackingNumber)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error sending email"
            BLErrorHandling.ErrorCapture(pSysModule, "sendEmail", "", ex.Message & " - Token: " & token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

    End Sub

#End Region

#Region "CRM Customers"

    Function crm_getAllCompanies(ByVal token As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_Customers_GetAll"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("UniqueKey")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Function getCompanies(ByVal token As String) As List(Of basicList)
        Dim lst As New List(Of basicList)
        Dim itm As basicList

        Try
            strCommand = "CRM_Customers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New basicList
                itm.id = reader.Item("UniqueKey")
                itm.value = reader.Item("Name")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try


        Return lst

    End Function

    Function saveNewCustomer(token As String, data As CRM_Customer) As String
        Dim customerId As String = ""
        Dim isOk As Boolean = False

        Try
            strCommand = "CRM_Customers_New"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = data.firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = data.lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parCompany As New SqlClient.SqlParameter("@Company", SqlDbType.NVarChar, 50)
            parCompany.Value = data.companyName
            Command.Parameters.Add(parCompany)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = data.street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Value = data.state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parBillingContact As New SqlClient.SqlParameter("@BillingContact", SqlDbType.NVarChar, 100)
            parBillingContact.Value = data.billingContact
            Command.Parameters.Add(parBillingContact)

            Dim parBillingEmail As New SqlClient.SqlParameter("@BillingEmail", SqlDbType.NVarChar, 100)
            parBillingEmail.Value = data.billingEmail
            Command.Parameters.Add(parBillingEmail)

            Dim parBillingPhone As New SqlClient.SqlParameter("@BillingPhone", SqlDbType.NVarChar, 50)
            parBillingPhone.Value = data.billingPhone
            Command.Parameters.Add(parBillingPhone)

            Dim parCCStreet As New SqlClient.SqlParameter("@CCStreet", SqlDbType.NVarChar, 100)
            parCCStreet.Value = data.billingStreet
            Command.Parameters.Add(parCCStreet)

            Dim parCCCity As New SqlClient.SqlParameter("@CCCity", SqlDbType.NVarChar, 20)
            parCCCity.Value = data.billingCity
            Command.Parameters.Add(parCCCity)

            Dim parCCState As New SqlClient.SqlParameter("@CCState", SqlDbType.NVarChar, 20)
            parCCState.Value = data.billingState
            Command.Parameters.Add(parCCState)

            Dim parCCPostalCode As New SqlClient.SqlParameter("@CCPostalCode", SqlDbType.NVarChar, 15)
            parCCPostalCode.Value = data.billingPostalCode
            Command.Parameters.Add(parCCPostalCode)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parNewId As New SqlClient.SqlParameter("@NewID", SqlDbType.VarChar, 50)
            parNewId.Direction = ParameterDirection.Output
            Command.Parameters.Add(parNewId)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            customerId = CStr(parNewId.Value)
            isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            customerId = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return customerId

    End Function

#End Region

#Region "CRM - Invoices"

    Function crmGetInvoices(ByVal token As String, ByVal custId As String) As List(Of invoice)
        Dim lst As New List(Of invoice)
        Dim itm As invoice
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "CRM_InvoicesByCustomer"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parCustID As New SqlClient.SqlParameter("@CustomerID", SqlDbType.NVarChar, 50)
            parCustID.Value = custId
            Command.Parameters.Add(parCustID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New invoice
                itm.invoiceNumber = reader.Item("RefNumber")
                itm.invoiceDate = reader.Item("TxnDate")
                itm.total = reader.Item("Subtotal")
                itm.paid = reader.Item("AppliedAmount")
                itm.balance = reader.Item("BalanceRemaining")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Snooze Report"

    Public Function snoozeReport(ByVal rpt As String, ByVal usr As String, ByVal period As Integer) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "Reports_Snooze"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parRpt As New SqlClient.SqlParameter("@EmailTemplateID", SqlDbType.NVarChar, 50)
            parRpt.Value = rpt
            Command.Parameters.Add(parRpt)

            Dim parUsr As New SqlClient.SqlParameter("@UserId", SqlDbType.NVarChar, 50)
            parUsr.Value = usr
            Command.Parameters.Add(parUsr)

            Dim parPeriod As New SqlClient.SqlParameter("@Period", SqlDbType.Int)
            parPeriod.Value = period
            Command.Parameters.Add(parPeriod)

            Dim parIsOk As New SqlClient.SqlParameter("@isOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = parIsOk.Value

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "snoozeReport", "", ex.Message & " - usr: " & usr, 0)
            bResult = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "IfByPhone calls"

    Public Sub saveIncomingCalls(ByVal dateTime As String,
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
                                 ByVal talkMinutes As String)

        Try
            strCommand = "IfByPhone_Calls_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parDateTime As New SqlClient.SqlParameter("@DateTime", SqlDbType.NVarChar, 50)
            parDateTime.Value = dateTime
            Command.Parameters.Add(parDateTime)

            Dim parSID As New SqlClient.SqlParameter("@SID", SqlDbType.NVarChar, 50)
            parSID.Value = sid
            Command.Parameters.Add(parSID)

            Dim parCallType As New SqlClient.SqlParameter("@CallType", SqlDbType.NVarChar, 50)
            parCallType.Value = callType
            Command.Parameters.Add(parCallType)

            Dim parFirstAction As New SqlClient.SqlParameter("@FirstAction", SqlDbType.NVarChar, 50)
            parFirstAction.Value = firstAction
            Command.Parameters.Add(parFirstAction)

            Dim parLastAction As New SqlClient.SqlParameter("@LastAction", SqlDbType.NVarChar, 50)
            parLastAction.Value = lastAction
            Command.Parameters.Add(parLastAction)

            Dim parcalledNumber As New SqlClient.SqlParameter("@CalledNumber", SqlDbType.NVarChar, 50)
            parcalledNumber.Value = calledNumber
            Command.Parameters.Add(parcalledNumber)

            Dim parCallerId As New SqlClient.SqlParameter("@CallerId", SqlDbType.NVarChar, 50)
            parCallerId.Value = callerId
            Command.Parameters.Add(parCallerId)

            Dim parTransferType As New SqlClient.SqlParameter("@TransferType", SqlDbType.NVarChar, 50)
            parTransferType.Value = transferType
            Command.Parameters.Add(parTransferType)

            Dim parTransferredToNumber As New SqlClient.SqlParameter("@TransferredToNumber", SqlDbType.NVarChar, 50)
            parTransferredToNumber.Value = transferredToNumber
            Command.Parameters.Add(parTransferredToNumber)

            Dim parCallTransferStatus As New SqlClient.SqlParameter("@CallTransferStatus", SqlDbType.NVarChar, 50)
            parCallTransferStatus.Value = callTransferStatus
            Command.Parameters.Add(parCallTransferStatus)

            Dim parPhoneLabel As New SqlClient.SqlParameter("@PhoneLabel", SqlDbType.NVarChar, 50)
            parPhoneLabel.Value = phoneLabel
            Command.Parameters.Add(parPhoneLabel)

            Dim parCallDuration As New SqlClient.SqlParameter("@CallDuration", SqlDbType.NVarChar, 50)
            parCallDuration.Value = callDuration
            Command.Parameters.Add(parCallDuration)

            Dim parTalkMinutes As New SqlClient.SqlParameter("@TalkMinutes", SqlDbType.NVarChar, 50)
            parTalkMinutes.Value = talkMinutes
            Command.Parameters.Add(parTalkMinutes)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

    End Sub

    Function smsReplyCatcher(ByVal msg As smsReply) As Boolean
        Dim res As Boolean = True
        Dim dl As New DataLayer

        Try
            strCommand = "IfByPhone_SMSReplyCatcher_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.NVarChar, -1)
            parMsg.Value = msg.message
            Command.Parameters.Add(parMsg)

            Dim parTo As New SqlClient.SqlParameter("@To", SqlDbType.NVarChar, 50)
            parTo.Value = msg.to
            Command.Parameters.Add(parTo)

            Dim parFrom As New SqlClient.SqlParameter("@From", SqlDbType.NVarChar, 50)
            parFrom.Value = msg.from
            Command.Parameters.Add(parFrom)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res = CStr(parIsOk.Value)

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "ET Site Services"

    Public Function ContactFrm(ByVal data As contactForm) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "SaveWebForm"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parFormID As New SqlClient.SqlParameter("@FormID", SqlDbType.NVarChar, 50)
            parFormID.Value = data.formId
            Command.Parameters.Add(parFormID)

            Dim parName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parMsg As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, -1)
            parMsg.Value = data.message
            Command.Parameters.Add(parMsg)

            Dim parIsOk As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            res.isOk = False
            res.msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Public Function quickContactFrm(ByVal data As quickContact) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "SaveWebForm"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parFormID As New SqlClient.SqlParameter("@FormID", SqlDbType.NVarChar, 50)
            parFormID.Value = data.formId
            Command.Parameters.Add(parFormID)

            Dim parName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 50)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = data.email
            Command.Parameters.Add(parEmail)

            Dim parIsOk As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)

        Catch ex As Exception
            res.isOk = False
            res.msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Function getBasePrice() As priceList
        Dim itm As New priceList

        Try
            strCommand = "Items_Price_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                itm.device = reader.Item("Device")
                itm.service = reader.Item("Service")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Function getCameras(ByVal data As wsRequest) As wsCamerasResponse
        Dim res As New wsCamerasResponse

        Try
            strCommand = "Devices_Cameras_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader

            Dim itm As wsCamera

            Do While reader.Read
                itm = New wsCamera
                itm.SerialNumber = reader.Item("SerialNumber")
                itm.LastUpdatedOn = reader.Item("LastUpdatedOn")
                res.Cameras.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Alerts API"

    Function getAlerts(ByVal token As String, ByVal isFullSync As Boolean) As List(Of alert)
        Dim lst As New List(Of alert)
        Dim itm As alert

        Try
            strCommand = "MOB_Down_Alerts"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIsFullSync As New SqlClient.SqlParameter("@IsFullSync", SqlDbType.Bit)
            parIsFullSync.Value = isFullSync
            Command.Parameters.Add(parIsFullSync)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New alert
                itm.id = reader.Item("ID")
                itm.alert = reader.Item("AlertName")
                itm.device = reader.Item("DeviceName")
                itm.driver = reader.Item("DriverName")
                itm.address = reader.Item("FullAddress")
                itm.lat = reader.Item("Latitude")
                itm.lng = reader.Item("Longitude")
                itm.speed = reader.Item("Speed")
                itm.eventDate = reader.Item("EventDate")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Save Web Forms: Contact Us, Quote, Buy Now"

    Public Function saveWebForm(ByVal FormID As String, ByVal qty As Integer, ByVal ServiceID As Integer, ByVal ShippingOption As Integer,
                                ByVal firstName As String, ByVal lastName As String, ByVal email As String, ByVal phone As String, ByVal cellPhone As String,
                                ByVal company As String, ByVal street As String, ByVal city As String, ByVal state As String,
                                ByVal postalCode As String, ByVal ccType As String, ByVal ccNumber As String, ByVal ccSec As String,
                                ByVal ccExpMonth As Integer, ByVal ccExpYear As Integer, ByVal ccFirstName As String, ByVal ccLastName As String,
                                ByVal ccStreet As String, ByVal ccCity As String, ByVal ccState As String, ByVal ccPostalCode As String,
                                ByVal Message As String, ByVal PromoCode As String, ByVal repId As String,
                                ByVal isOBDOption As Boolean, ByVal isPostedSLOption As Boolean) As Boolean
        Dim bResults As Boolean = False

        Try
            strCommand = "SaveWebForm"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parFormID As New SqlClient.SqlParameter("@FormID", SqlDbType.NVarChar, 50)
            parFormID.Value = FormID
            Command.Parameters.Add(parFormID)

            Dim parQty As New SqlClient.SqlParameter("@Qty", SqlDbType.Int)
            parQty.Value = qty
            Command.Parameters.Add(parQty)

            Dim parServiceID As New SqlClient.SqlParameter("@ServiceID", SqlDbType.Int)
            parServiceID.Value = ServiceID
            Command.Parameters.Add(parServiceID)

            Dim parIsOBOption As New SqlClient.SqlParameter("@IsOBDOption", SqlDbType.Bit)
            parIsOBOption.Value = isOBDOption
            Command.Parameters.Add(parIsOBOption)

            Dim parIsPSLOption As New SqlClient.SqlParameter("@isPostedSLOption", SqlDbType.Bit)
            parIsPSLOption.Value = isPostedSLOption
            Command.Parameters.Add(parIsPSLOption)

            Dim parShippingOption As New SqlClient.SqlParameter("@ShippingOption", SqlDbType.Int)
            parShippingOption.Value = ShippingOption
            Command.Parameters.Add(parShippingOption)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parCellPhone As New SqlClient.SqlParameter("@CellPhone", SqlDbType.NVarChar, 20)
            parCellPhone.Value = cellPhone
            Command.Parameters.Add(parCellPhone)

            Dim parCompany As New SqlClient.SqlParameter("@Company", SqlDbType.NVarChar, 50)
            parCompany.Value = company
            Command.Parameters.Add(parCompany)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCCType As New SqlClient.SqlParameter("@CCType", SqlDbType.NVarChar, 5)
            parCCType.Value = ccType
            Command.Parameters.Add(parCCType)

            Dim parCCNumber As New SqlClient.SqlParameter("@CCNumber", SqlDbType.NVarChar, 50)
            parCCNumber.Value = ccNumber
            Command.Parameters.Add(parCCNumber)

            Dim parCCSecCode As New SqlClient.SqlParameter("@CCSecCode", SqlDbType.NVarChar, 10)
            parCCSecCode.Value = ccSec
            Command.Parameters.Add(parCCSecCode)

            Dim parCCExpMonth As New SqlClient.SqlParameter("@CCExpMonth", SqlDbType.Int)
            parCCExpMonth.Value = ccExpMonth
            Command.Parameters.Add(parCCExpMonth)

            Dim parCCExpYear As New SqlClient.SqlParameter("@CCExpYear", SqlDbType.Int)
            parCCExpYear.Value = ccExpYear
            Command.Parameters.Add(parCCExpYear)

            Dim parCCFirstName As New SqlClient.SqlParameter("@CCFirstName", SqlDbType.NVarChar, 20)
            parCCFirstName.Value = ccFirstName
            Command.Parameters.Add(parCCFirstName)

            Dim parCCLastName As New SqlClient.SqlParameter("@CCLastName", SqlDbType.NVarChar, 20)
            parCCLastName.Value = ccLastName
            Command.Parameters.Add(parCCLastName)

            Dim parCCStreet As New SqlClient.SqlParameter("@CCStreet", SqlDbType.NVarChar, 100)
            parCCStreet.Value = ccStreet
            Command.Parameters.Add(parCCStreet)

            Dim parCCCity As New SqlClient.SqlParameter("@CCCity", SqlDbType.NVarChar, 20)
            parCCCity.Value = ccCity
            Command.Parameters.Add(parCCCity)

            Dim parCCState As New SqlClient.SqlParameter("@CCState", SqlDbType.NVarChar, 20)
            parCCState.Value = ccState
            Command.Parameters.Add(parCCState)

            Dim parCCPostalCode As New SqlClient.SqlParameter("@CCPostalCode", SqlDbType.NVarChar, 15)
            parCCPostalCode.Value = ccPostalCode
            Command.Parameters.Add(parCCPostalCode)

            Dim parMessage As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 5000)
            parMessage.Value = Message
            Command.Parameters.Add(parMessage)

            Dim parPromoCode As New SqlClient.SqlParameter("@PromoCode", SqlDbType.NVarChar, 6)
            parPromoCode.Value = PromoCode
            Command.Parameters.Add(parPromoCode)

            Dim parRepID As New SqlClient.SqlParameter("@RepID", SqlDbType.NVarChar, 5)
            parRepID.Value = repId
            Command.Parameters.Add(parRepID)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResults = CBool(parResult.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function saveWebForm_v2(ByVal FormID As String, ByVal qtyGX As Integer,
        ByVal qtyTF As Integer, ByVal qtyOBDTracker As Integer, ByVal qtyAssets As Integer, ByVal ServiceID As Integer, ByVal ShippingOption As Integer,
        ByVal firstName As String, ByVal lastName As String, ByVal email As String, ByVal phone As String, ByVal cellPhone As String,
        ByVal company As String, ByVal street As String, ByVal city As String, ByVal state As String,
        ByVal postalCode As String, ByVal ccType As String, ByVal ccNumber As String, ByVal ccSec As String,
        ByVal ccExpMonth As Integer, ByVal ccExpYear As Integer, ByVal ccFirstName As String, ByVal ccLastName As String,
        ByVal ccStreet As String, ByVal ccCity As String, ByVal ccState As String, ByVal ccPostalCode As String,
        ByVal Message As String, ByVal PromoCode As String, ByVal repId As String,
        ByVal qtyOBDConnector As Integer, ByVal isPostedSLOption As Boolean) As Boolean

        Dim bResults As Boolean = False

        Try
            strCommand = "SaveWebForm"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parFormID As New SqlClient.SqlParameter("@FormID", SqlDbType.NVarChar, 50)
            parFormID.Value = FormID
            Command.Parameters.Add(parFormID)

            Dim parQtyGX As New SqlClient.SqlParameter("@QtyGX", SqlDbType.Int)
            parQtyGX.Value = qtyGX
            Command.Parameters.Add(parQtyGX)

            Dim parQtyTF As New SqlClient.SqlParameter("@QtyTF", SqlDbType.Int)
            parQtyTF.Value = qtyTF
            Command.Parameters.Add(parQtyTF)

            Dim parQtyOBDTracker As New SqlClient.SqlParameter("@QtyOBDTracker", SqlDbType.Int)
            parQtyOBDTracker.Value = qtyOBDTracker
            Command.Parameters.Add(parQtyOBDTracker)

            Dim parQtyAssets As New SqlClient.SqlParameter("@QtyAssets", SqlDbType.Int)
            parQtyAssets.Value = qtyAssets
            Command.Parameters.Add(parQtyAssets)

            Dim parServiceID As New SqlClient.SqlParameter("@ServiceID", SqlDbType.Int)
            parServiceID.Value = ServiceID
            Command.Parameters.Add(parServiceID)

            Dim parQtyOBDConnector As New SqlClient.SqlParameter("@QtyOBDConnector", SqlDbType.Int)
            parQtyOBDConnector.Value = qtyOBDConnector
            Command.Parameters.Add(parQtyOBDConnector)

            Dim parIsPSLOption As New SqlClient.SqlParameter("@isPostedSLOption", SqlDbType.Bit)
            parIsPSLOption.Value = isPostedSLOption
            Command.Parameters.Add(parIsPSLOption)

            Dim parShippingOption As New SqlClient.SqlParameter("@ShippingOption", SqlDbType.Int)
            parShippingOption.Value = ShippingOption
            Command.Parameters.Add(parShippingOption)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 20)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 20)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parEmail As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 100)
            parEmail.Value = email
            Command.Parameters.Add(parEmail)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 20)
            parPhone.Value = phone
            Command.Parameters.Add(parPhone)

            Dim parCellPhone As New SqlClient.SqlParameter("@CellPhone", SqlDbType.NVarChar, 20)
            parCellPhone.Value = cellPhone
            Command.Parameters.Add(parCellPhone)

            Dim parCompany As New SqlClient.SqlParameter("@Company", SqlDbType.NVarChar, 50)
            parCompany.Value = company
            Command.Parameters.Add(parCompany)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 20)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCCType As New SqlClient.SqlParameter("@CCType", SqlDbType.NVarChar, 5)
            parCCType.Value = ccType
            Command.Parameters.Add(parCCType)

            Dim parCCNumber As New SqlClient.SqlParameter("@CCNumber", SqlDbType.NVarChar, 50)
            parCCNumber.Value = ccNumber
            Command.Parameters.Add(parCCNumber)

            Dim parCCSecCode As New SqlClient.SqlParameter("@CCSecCode", SqlDbType.NVarChar, 10)
            parCCSecCode.Value = ccSec
            Command.Parameters.Add(parCCSecCode)

            Dim parCCExpMonth As New SqlClient.SqlParameter("@CCExpMonth", SqlDbType.Int)
            parCCExpMonth.Value = ccExpMonth
            Command.Parameters.Add(parCCExpMonth)

            Dim parCCExpYear As New SqlClient.SqlParameter("@CCExpYear", SqlDbType.Int)
            parCCExpYear.Value = ccExpYear
            Command.Parameters.Add(parCCExpYear)

            Dim parCCFirstName As New SqlClient.SqlParameter("@CCFirstName", SqlDbType.NVarChar, 20)
            parCCFirstName.Value = ccFirstName
            Command.Parameters.Add(parCCFirstName)

            Dim parCCLastName As New SqlClient.SqlParameter("@CCLastName", SqlDbType.NVarChar, 20)
            parCCLastName.Value = ccLastName
            Command.Parameters.Add(parCCLastName)

            Dim parCCStreet As New SqlClient.SqlParameter("@CCStreet", SqlDbType.NVarChar, 100)
            parCCStreet.Value = ccStreet
            Command.Parameters.Add(parCCStreet)

            Dim parCCCity As New SqlClient.SqlParameter("@CCCity", SqlDbType.NVarChar, 20)
            parCCCity.Value = ccCity
            Command.Parameters.Add(parCCCity)

            Dim parCCState As New SqlClient.SqlParameter("@CCState", SqlDbType.NVarChar, 20)
            parCCState.Value = ccState
            Command.Parameters.Add(parCCState)

            Dim parCCPostalCode As New SqlClient.SqlParameter("@CCPostalCode", SqlDbType.NVarChar, 15)
            parCCPostalCode.Value = ccPostalCode
            Command.Parameters.Add(parCCPostalCode)

            Dim parMessage As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 5000)
            parMessage.Value = Message
            Command.Parameters.Add(parMessage)

            Dim parPromoCode As New SqlClient.SqlParameter("@PromoCode", SqlDbType.NVarChar, 6)
            parPromoCode.Value = PromoCode
            Command.Parameters.Add(parPromoCode)

            Dim parRepID As New SqlClient.SqlParameter("@RepID", SqlDbType.NVarChar, 5)
            parRepID.Value = repId
            Command.Parameters.Add(parRepID)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.Bit)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResults = CBool(parResult.Value)

        Catch ex As Exception
            bResults = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResults

    End Function

    Public Function getDocQty(ByVal docGUID As String) As qtyDoc
        Dim itm As New qtyDoc

        Try
            strCommand = "Docs_GetQtyFromGUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim pardocGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            pardocGUID.Value = docGUID
            Command.Parameters.Add(pardocGUID)

            'Qty, ShippingOption
            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                itm.qty = reader.Item("Qty")
                itm.shippingOption = reader.Item("ShippingOption")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function contactUnsubscribe(ByVal contactGUID As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "Contacts_Unsubscribe"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parGUID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 50)
            parGUID.Value = contactGUID
            Command.Parameters.Add(parGUID)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dtData = Nothing
            strError = "Error contact unsubscribe from GUID"
            BLErrorHandling.ErrorCapture(pSysModule, "contactUnsubscribe", "", ex.Message, 0)
        End Try

        Return dtData

    End Function

    Public Function getFamousQuote(ByVal TypeGUID As String, ByRef strError As String) As String
        Dim Quote As String = ""

        Try
            strCommand = "GetFamousQuote"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parType As New SqlClient.SqlParameter("@TypeGUID", SqlDbType.NVarChar, 50)
            parType.Value = TypeGUID
            Command.Parameters.Add(parType)

            Dim parQuote As New SqlClient.SqlParameter("@Quote", SqlDbType.NVarChar, 5000)
            parQuote.Direction = ParameterDirection.Output
            Command.Parameters.Add(parQuote)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            Quote = CStr(parQuote.Value)

        Catch ex As Exception
            Quote = ""
            strError = "Error getting quote"
            BLErrorHandling.ErrorCapture(pSysModule, "getFamousQuote", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return Quote

    End Function

    Public Function getShoppingCartInfo(ByVal Token As String) As webForm
        Dim wf As New webForm

        Try
            strCommand = "ShoppingCart_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                wf.fn = reader.Item("FirstName")
                wf.ln = reader.Item("LastName")
                wf.email = reader.Item("Email")
                wf.co = reader.Item("CompanyName")
                wf.ph = reader.Item("Phone")
                wf.street = reader.Item("Street")
                wf.city = reader.Item("City")
                wf.state = reader.Item("State")
                wf.postalCode = reader.Item("postalCode")
                wf.ccType = reader.Item("ccType")
                wf.ccNo = reader.Item("ccNumber")
                wf.ccMonth = reader.Item("ccExpMonth")
                wf.ccYear = reader.Item("ccExpYear")
                wf.ccFn = reader.Item("ccFirstName")
                wf.ccLn = reader.Item("ccLastName")
                wf.ccStreet = reader.Item("ccStreet")
                wf.ccCity = reader.Item("ccCity")
                wf.ccState = reader.Item("ccState")
                wf.ccPostal = reader.Item("ccPostalCode")
                wf.price = reader.Item("Price")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return wf

    End Function

    Public Function getCompanyInfo(ByVal Token As String) As companyInfo
        Dim c As New companyInfo

        Try
            strCommand = "Companies_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                c.id = reader.Item("uniqueKey")
                c.companyName = reader.Item("CompanyName")
                c.phone = reader.Item("Phone")
                c.website = reader.Item("Website")
                c.street = reader.Item("Street")
                c.city = reader.Item("City")
                c.state = reader.Item("State")
                c.postalCode = reader.Item("PostalCode")
                c.countryCode = reader.Item("CountryCode")
                c.industry = reader.Item("Industry")
                c.createdOn = reader.Item("CreatedOn").ToString
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return c

    End Function

    Public Function getCompanyInfo2(ByVal uid As String, ByRef errMsg As String) As companyInfo2
        Dim itm As New companyInfo2
        Dim strJson As String = ""
        Dim isFound As Boolean = False

        Try
            strCommand = "Companies_GetByUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUID As New SqlClient.SqlParameter("@UID", SqlDbType.NVarChar, 50)
            parUID.Value = uid
            Command.Parameters.Add(parUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                isFound = True
                itm.id = reader.Item("uniqueKey")
                itm.companyName = reader.Item("CompanyName")
                itm.firstName = reader.Item("FirstName")
                itm.lastName = reader.Item("LastName")
                itm.token = reader.Item("Token")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            errMsg = ex.Message
            strJson = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Public Function saveCompanyInfo(ByVal Token As String, ByVal data As companyInfo) As Boolean
        Dim bOk As Boolean = True

        Try
            strCommand = "Companies_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.companyName
            Command.Parameters.Add(parName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = data.phone
            Command.Parameters.Add(parPhone)

            Dim parWebSite As New SqlClient.SqlParameter("@WebSite", SqlDbType.NVarChar, 100)
            parWebSite.Value = data.website
            Command.Parameters.Add(parWebSite)

            Dim parIndustry As New SqlClient.SqlParameter("@Industry", SqlDbType.NVarChar, -1)
            parIndustry.Value = data.industry
            Command.Parameters.Add(parIndustry)

            Dim parStreet As New SqlClient.SqlParameter("@ShipStreet", SqlDbType.NVarChar, 100)
            parStreet.Value = data.street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@ShipCity", SqlDbType.NVarChar, 50)
            parCity.Value = data.city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@ShipState", SqlDbType.NVarChar, 50)
            parState.Value = data.state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@ShipPostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = data.postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountryCode As New SqlClient.SqlParameter("@ShipCountryCode", SqlDbType.NVarChar, 50)
            parCountryCode.Value = data.countryCode
            Command.Parameters.Add(parCountryCode)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = data.lat
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = data.lng
            Command.Parameters.Add(parLongitude)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "saveCompanyInfo", "", ex.Message, 0)
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

    Public Function getCCInfo(ByVal Token As String) As ccInfo
        Dim cc As New ccInfo

        Try
            strCommand = "Companies_Billing_GetByToken"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                cc.billingContact = reader.Item("BillingContact")
                cc.billingEmail = reader.Item("billingEmail")
                cc.billingPhone = reader.Item("billingPhone")
                cc.ccType = reader.Item("CCType")
                cc.ccNumber = reader.Item("ccNumber")
                cc.ccSecCode = reader.Item("ccSecCode")
                cc.ccExpMonth = reader.Item("ccExpMonth")
                cc.ccExpYear = reader.Item("ccExpYear")
                cc.ccFirstName = reader.Item("ccFirstName")
                cc.ccLastName = reader.Item("ccLastName")
                cc.ccStreet = reader.Item("ccStreet")
                cc.ccCity = reader.Item("ccCity")
                cc.ccState = reader.Item("ccState")
                cc.ccPostalCode = reader.Item("ccPostalCode")
                cc.ccCountryCode = reader.Item("ccCountryCode")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return cc

    End Function

    Public Function saveBillingInfo(ByVal Token As String, ByVal data As ccInfo) As Boolean
        Dim strError As String = ""
        Dim bOk As Boolean = True

        Try
            strCommand = "Companies_Billing_UPDATE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parBillingContact As New SqlClient.SqlParameter("@BillingContact", SqlDbType.NVarChar, 100)
            parBillingContact.Value = data.billingContact
            Command.Parameters.Add(parBillingContact)

            Dim parBillingEmail As New SqlClient.SqlParameter("@BillingEmail", SqlDbType.NVarChar, 100)
            parBillingEmail.Value = data.billingEmail
            Command.Parameters.Add(parBillingEmail)

            Dim parBillingPhone As New SqlClient.SqlParameter("@BillingPhone", SqlDbType.NVarChar, 50)
            parBillingPhone.Value = data.billingPhone
            Command.Parameters.Add(parBillingPhone)

            Dim parType As New SqlClient.SqlParameter("@Type", SqlDbType.NVarChar, 10)
            parType.Value = data.ccType
            Command.Parameters.Add(parType)

            Dim parNumber As New SqlClient.SqlParameter("@Number", SqlDbType.NVarChar, 30)
            parNumber.Value = data.ccNumber
            Command.Parameters.Add(parNumber)

            Dim parSecCode As New SqlClient.SqlParameter("@SecCode", SqlDbType.NVarChar, 4)
            parSecCode.Value = data.ccSecCode
            Command.Parameters.Add(parSecCode)

            Dim parExpMonth As New SqlClient.SqlParameter("@ExpMonth", SqlDbType.Int)
            parExpMonth.Value = data.ccExpMonth
            Command.Parameters.Add(parExpMonth)

            Dim parExpYear As New SqlClient.SqlParameter("@ExpYear", SqlDbType.Int)
            parExpYear.Value = data.ccExpYear
            Command.Parameters.Add(parExpYear)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 50)
            parFirstName.Value = data.ccFirstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 50)
            parLastName.Value = data.ccLastName
            Command.Parameters.Add(parLastName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = data.ccStreet
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = data.ccCity
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Value = data.ccState
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = data.ccPostalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountryCode As New SqlClient.SqlParameter("@CountryCode", SqlDbType.NVarChar, 50)
            parCountryCode.Value = data.ccCountryCode
            Command.Parameters.Add(parCountryCode)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error saving billing info"
            BLErrorHandling.ErrorCapture(pSysModule, "saveBillingInfo", "", ex.Message, 0)
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

    Public Function saveBillingInfoByUID(ByVal UID As String, ByVal type As String, ByVal number As String, ByVal secCode As String, ByVal expMonth As Integer, ByVal expYear As Integer, ByVal firstName As String, ByVal lastName As String, ByVal street As String, ByVal city As String, ByVal state As String, ByVal postalCode As String, ByVal countryCode As String, ByRef errMsg As String) As Boolean
        Dim strError As String = ""
        Dim bOk As Boolean = True

        Try
            strCommand = "Companies_Billing_UpdateByUID"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parUID As New SqlClient.SqlParameter("@UID", SqlDbType.NVarChar, 50)
            parUID.Value = UID
            Command.Parameters.Add(parUID)

            Dim parType As New SqlClient.SqlParameter("@Type", SqlDbType.NVarChar, 10)
            parType.Value = type
            Command.Parameters.Add(parType)

            Dim parNumber As New SqlClient.SqlParameter("@Number", SqlDbType.NVarChar, 30)
            parNumber.Value = number
            Command.Parameters.Add(parNumber)

            Dim parSecCode As New SqlClient.SqlParameter("@SecCode", SqlDbType.NVarChar, 4)
            parSecCode.Value = secCode
            Command.Parameters.Add(parSecCode)

            Dim parExpMonth As New SqlClient.SqlParameter("@ExpMonth", SqlDbType.Int)
            parExpMonth.Value = expMonth
            Command.Parameters.Add(parExpMonth)

            Dim parExpYear As New SqlClient.SqlParameter("@ExpYear", SqlDbType.Int)
            parExpYear.Value = expYear
            Command.Parameters.Add(parExpYear)

            Dim parFirstName As New SqlClient.SqlParameter("@FirstName", SqlDbType.NVarChar, 50)
            parFirstName.Value = firstName
            Command.Parameters.Add(parFirstName)

            Dim parLastName As New SqlClient.SqlParameter("@LastName", SqlDbType.NVarChar, 50)
            parLastName.Value = lastName
            Command.Parameters.Add(parLastName)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 100)
            parStreet.Value = street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Value = city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Value = state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Value = postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountryCode As New SqlClient.SqlParameter("@CountryCode", SqlDbType.NVarChar, 50)
            parCountryCode.Value = countryCode
            Command.Parameters.Add(parCountryCode)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

        Catch ex As Exception
            strError = "Error saving billing info"
            BLErrorHandling.ErrorCapture(pSysModule, "saveBillingInfoByUID", "", ex.Message, 0)
            bOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bOk

    End Function

#End Region

#Region "Notifications API"

    Function postRegistrationId(ByVal data As registrationId) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Notifications_RegistrationId_SET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = data.token
            Command.Parameters.Add(parToken)

            Dim parSource As New SqlClient.SqlParameter("@SourceID", SqlDbType.Int, 4)
            parSource.Value = data.sourceId
            Command.Parameters.Add(parSource)

            Dim parRegistrationId As New SqlClient.SqlParameter("@RegistrationId", SqlDbType.NVarChar, 50)
            parRegistrationId.Value = data.regId
            Command.Parameters.Add(parRegistrationId)

            Dim parAppName As New SqlClient.SqlParameter("@AppName", SqlDbType.NVarChar, 50)
            parAppName.Value = data.appName
            Command.Parameters.Add(parAppName)

            Dim parIsOk As New SqlClient.SqlParameter("@isOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Function getNotificationTopics(ByVal token As String, ByVal isFullSync As Boolean) As List(Of notificationTopic)
        Dim lst As New List(Of notificationTopic)
        Dim itm As notificationTopic

        Try
            strCommand = "MOB_Down_Topics"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIsFullSync As New SqlClient.SqlParameter("@IsFullSync", SqlDbType.Bit)
            parIsFullSync.Value = isFullSync
            Command.Parameters.Add(parIsFullSync)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New notificationTopic
                itm.topic = reader.Item("Topic")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

#End Region

#Region "Devices Commands OTA"

    Function sendDeviceCommand(ByVal data As deviceCommand) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Devices_OTACmdByText_SEND"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parCmdId As New SqlClient.SqlParameter("@CommandId", SqlDbType.Int, 4)
            parCmdId.Value = data.cmdId
            Command.Parameters.Add(parCmdId)

            Dim parCmd As New SqlClient.SqlParameter("@Command", SqlDbType.NVarChar, 100)
            parCmd.Value = data.cmd
            Command.Parameters.Add(parCmd)

            Dim parIsOk As New SqlClient.SqlParameter("@isOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Function getDeviceResponses(ByVal deviceId As String) As List(Of deviceResponse)
        Dim lst As New List(Of deviceResponse)
        Dim itm As deviceResponse

        Try

        Catch ex As Exception

        End Try

        Return lst

    End Function

#End Region

#Region "TFTP Automation"

    Function getCfgScripts() As List(Of cfgFile)
        Dim lst As New List(Of cfgFile)
        Dim itm As cfgFile = Nothing

        Try
            strCommand = "Devices_ConfigFiles_GetDirtyFiles"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                itm = New cfgFile
                itm.id = reader.Item("ID")
                itm.folder = reader.Item("FilePath")
                itm.fullName = reader.Item("FullFileName")
                itm.name = reader.Item("FileNameNoExt")
                itm.content = reader.Item("FileContent")
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        End Try

        Return lst

    End Function

    Function saveCfgScript(ByVal data As cfgFile) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Devices_ConfigFiles_SAVE"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parFolder As New SqlClient.SqlParameter("@FilePath", SqlDbType.NVarChar, 500)
            parFolder.Value = data.folder
            Command.Parameters.Add(parFolder)

            Dim parFullName As New SqlClient.SqlParameter("@FullFileName", SqlDbType.NVarChar, 500)
            parFullName.Value = data.fullName
            Command.Parameters.Add(parFullName)

            Dim parName As New SqlClient.SqlParameter("@FileNameNoExt", SqlDbType.NVarChar, 200)
            parName.Value = data.name
            Command.Parameters.Add(parName)

            Dim parContent As New SqlClient.SqlParameter("@FileContent", SqlDbType.NVarChar, -1)
            parContent.Value = data.content
            Command.Parameters.Add(parContent)

            Dim parIsOk As New SqlClient.SqlParameter("@isOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

    Function resetCfgScript(ByVal id As Integer) As responseOk
        Dim res As New responseOk

        Try
            strCommand = "Devices_ConfigFiles_SetDirtyFlagOff"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parID As New SqlClient.SqlParameter("@ConfigFileID", SqlDbType.Int)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res.isOk = True

        Catch ex As Exception
            res.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Fleet HeartBeat"

    Public Function getFleetHeartBeat(ByVal token As String) As fleetHeartBeat
        Dim f As New fleetHeartBeat

        Try
            strCommand = "Reports_FleetHeartBeat"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                f.fleetMiles = reader.Item("fleetMiles")
                f.drivingHours = reader.Item("drivingHours")
                f.idleHours = reader.Item("idleHours")
                f.mostActive.name = reader.Item("MostActiveName")
                f.mostActive.miles = reader.Item("MostActiveMiles")
                f.bestDriver.name = reader.Item("BestDriverName")
                f.bestDriver.incidents = reader.Item("BestDriverIncidents")
                f.worstDriver.name = reader.Item("WorstDriverName")
                f.worstDriver.incidents = reader.Item("WorstDriverIncidents")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return f

    End Function

#End Region

#Region "WLIUS"

    Public Function getWliusApiKey(ByVal token As String) As etWliusResponse
        Dim res As New etWliusResponse

        Try
            strCommand = "Companies_GetWLIUS_APIKEY"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                res.HasElog = reader.Item("hasWLIUS_ELOG")
                res.ApiKey = reader.Item("WLIUS_APIKEY")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function
    Public Function GetHvideo_APIKEY(ByVal token As String) As etHvideoResponse
        Dim res As New etHvideoResponse

        Try
            strCommand = "Companies_GetHvideo_APIKEY"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader
            reader = Command.ExecuteReader
            Do While reader.Read
                res.HasVideo = reader.Item("Has_Video")
                res.ApiKey = reader.Item("SW_VideoKey")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region

#Region "Quick Messages"

    Public Function getQuickMsgDriversList(ByVal token As String, ByRef strError As String) As DataTable
        Dim dtData As New DataTable

        Try
            strCommand = "QuickMessage_Drivers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dtData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            strError = "Error getting getQuickMsgDriversList"
            BLErrorHandling.ErrorCapture(pSysModule, "getQuickMsgDriversList", "", ex.Message & " - Token: " & token, 0)
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Function sendQuickMsg(ByVal token As String, ByVal driverId As Integer, ByVal channel As Integer, ByVal message As String, ByRef msg As String) As Boolean
        Dim bResult As Boolean = True

        Try
            strCommand = "QuickMessage_SEND"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDriverId As New SqlClient.SqlParameter("@DriverID", SqlDbType.Int, 4)
            parDriverId.Value = driverId
            Command.Parameters.Add(parDriverId)

            Dim parChannel As New SqlClient.SqlParameter("@Channel", SqlDbType.Int, 4)
            parChannel.Value = channel
            Command.Parameters.Add(parChannel)

            Dim parMessage As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 500)
            parMessage.Value = message
            Command.Parameters.Add(parMessage)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            bResult = CStr(parIsOk.Value)

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "DL.sendQuickMsg", "", ex.Message & " - Token: " & token, 0)
            bResult = False
            msg = ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return bResult

    End Function

#End Region

#Region "CRM"

    Function crmCasesMessageAdd(ByVal data As etAddCrmMessageRequest) As etResponse
        Dim res As New etResponse

        Try
            strCommand = "CRM_Cases_InsertFromApp"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim Token As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            Token.Value = data.Token
            Command.Parameters.Add(Token)

            Dim strDeviceId As New SqlClient.SqlParameter("@DeviceId", SqlDbType.VarChar, 20)
            strDeviceId.Value = data.DeviceId
            Command.Parameters.Add(strDeviceId)

            Dim Subject As New SqlClient.SqlParameter("@Subject", SqlDbType.NVarChar, -1)
            Subject.Value = data.Subject
            Command.Parameters.Add(Subject)

            Dim Note As New SqlClient.SqlParameter("@Notes", SqlDbType.NVarChar, -1)
            Note.Value = data.Note
            Command.Parameters.Add(Note)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            Dim parMsg As New SqlClient.SqlParameter("@Msg", SqlDbType.NVarChar, 100)
            parMsg.Direction = ParameterDirection.Output
            Command.Parameters.Add(parMsg)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()

            res.isOk = CBool(parIsOk.Value)
            res.msg = CStr(parMsg.Value)

        Catch ex As Exception
            res.isOk = False
            res.msg = ex.Message
            BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return res

    End Function
    Function CRM_HDevices_GET(token As String, did As String) As String
        Dim strJson As String = ""
        Dim crmDeviceData As New CrmDeviceData
        Dim listCrmDeviceData As New List(Of CrmDeviceData)

        Dim hdevice
        Dim hdevicesInternal
        Dim tempsensor
        Dim HistorySensor
        Try
            strCommand = "CRM_HDevices_GET_NewCRM"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 100)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDID As New SqlClient.SqlParameter("@GUID", SqlDbType.NVarChar, 100)
            parDID.Direction = ParameterDirection.Input
            parDID.Value = did
            Command.Parameters.Add(parDID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            'This Stored Procedure throws 2 result sets...
            Dim reader As SqlDataReader = Command.ExecuteReader

            Do While reader.Read
                crmDeviceData.Device.DeviceType = reader("DeviceType")
                crmDeviceData.Device.Model = reader("Model")
                crmDeviceData.Device.DeviceID = reader("DeviceID")
                crmDeviceData.Device.Name = reader("Name")
                crmDeviceData.Device.SimNoDB = reader("SimNoDB")
                crmDeviceData.Device.SimNoUnit = reader("SimNoUnit")
                crmDeviceData.Device.ReportIgnON = reader("ReportIgnON")
                crmDeviceData.Device.ReportTimerIgnOff = reader("ReportTimerIgnOff")
                crmDeviceData.Device.ReportTurnAngle = reader("ReportTurnAngle")
                crmDeviceData.Device.ReportDistance = reader("ReportDistance")
                crmDeviceData.Device.FakeIgn = reader("FakeIgn")
                crmDeviceData.Device.IgnON = reader("IgnON")
                crmDeviceData.Device.IgnOFF = reader("IgnOFF")
                crmDeviceData.Device.ServerIP = reader("ServerIP")
                crmDeviceData.Device.ServerPort = reader("ServerPort")
            Loop
            reader.NextResult()

            Do While reader.Read
                hdevice = New HDevices With {
                    .id = reader("ID"),
                    .ignitionStatus = reader("IgnitionStatus"),
                    .eventCode = reader("EventCode"),
                    .eventName = reader("EventName"),
                    .eventDate = reader("EventDate"),
                    .speed = reader("Speed"),
                    .createdOn = reader("CreatedOn"),
                    .gpsAge = reader("GPSAge"),
                    .gpsCount = reader("GPSCount"),
                    .consecutive = reader("Consecutive"),
                    .deviceTypeID = reader("DeviceTypeID"),
                    .isBrief = reader("IsBrief"),
                    .originalEvent = reader("OriginalEvent"),
                    .lat = reader("Latitude"),
                    .lng = reader("Longitude"),
                    .rssi = reader("RSSI"),
                    .ble = reader("Ble"),
                    .bi = reader("BI"),
                    .be = reader("BE"),
                    .msgDelay = reader("MsgDelay"),
                    .temperature1 = reader("Temperature1"),
                    .temperature2 = reader("Temperature2"),
                    .temperature3 = reader("Temperature3"),
                    .temperature4 = reader("Temperature4")
                }
                crmDeviceData.ListHDevices.Add(hdevice)
            Loop
            'once done, jumps to the second result set
            reader.NextResult()

            Do While reader.Read
                hdevicesInternal = New HDevicesInternal With {
                   .MessageType = reader("MessageType"),
                   .MessageDevice = reader("MessageDevice"),
                   .IP = reader("IP"),
                   .Port = reader("Port"),
                   .CreatedOn = reader("CreatedOn")
                }
                crmDeviceData.ListHDevicesInternal.Add(hdevicesInternal)
            Loop
            reader.NextResult()

            Do While reader.Read
                tempsensor = New TempSensors With {
                   .ID = reader("ID"),
                   .IMEI = reader("IMEI"),
                   .TempNumber = reader("TempNumber"),
                   .SensorID = reader("SensorID"),
                   .Name = reader("Name"),
                   .CreatedOn = reader("CreatedOn"),
                .LastUpdatedOn = reader("LastUpdatedOn")
                }
                crmDeviceData.ListHTempSensor.Add(tempsensor)
            Loop
            reader.NextResult()
            Do While reader.Read
                HistorySensor = New HistorySensor With {
                   .ID = reader("ID"),
                   .EventDate = reader("EventDate"),
                   .Temperature1 = reader("Temperature1"),
                   .Temperature2 = reader("Temperature2"),
                   .Temperature3 = reader("Temperature3"),
                   .Temperature4 = reader("Temperature4"),
                .Latitude = reader("Latitude"),
                .Longitude = reader("Longitude"),
                .NameSensor = reader("NameSensor")
                }
                crmDeviceData.HistorySensor.Add(HistorySensor)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If
            strJson = JsonConvert.SerializeObject(crmDeviceData)
        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "CRM_HDevices_GET", "", ex.Message, 0)
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return strJson

    End Function

#End Region

#Region "Reports"
    Public Function ReportNew(ByVal Token As String, ByVal IsBatch As Boolean, ByVal RecurrentID As Integer, ByVal CompanyID As Integer, ByVal GroupID As Integer,
                              ByVal IsAllDevices As Boolean, ByVal ExcludeWeekends As Boolean, ByVal ReportID As Integer, ByVal DeviceID As String, ByVal DateFrom As String, ByVal DateTo As String,
                              ByVal HourFrom As Integer, ByVal HourTo As Integer, ByVal ThisDayOfWeek As Integer, ByVal Param As String, ByVal Param2 As String, ByVal IsForExport As Boolean) As List(Of Object) 'List(Of Object)

        Dim res
        Dim ListJson As List(Of Object) = New List(Of Object)
        Dim reader As SqlDataReader = Nothing
        Try
            'strCommand = "etAPI_ReportsKendo"
            strCommand = "Reports_RUN_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50) With {
                .Value = Token
            }
            Command.Parameters.Add(parToken)

            Dim parReportID As New SqlClient.SqlParameter("@ReportID", SqlDbType.Int, 50) With {
                .Value = ReportID
            }
            Command.Parameters.Add(parReportID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50) With {
                .Value = DeviceID 'A291 
                }
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime) With {
                .Value = DateFrom
            }
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)
            parDateTo.Value = DateTo
            Command.Parameters.Add(parDateTo)

            Dim parParam As New SqlClient.SqlParameter("@Param", SqlDbType.NVarChar, 50)
            parParam.Value = Param
            Command.Parameters.Add(parParam)

            Dim parParam2 As New SqlClient.SqlParameter("@Param2", SqlDbType.NVarChar, 50)
            parParam2.Value = Param2
            Command.Parameters.Add(parParam2)

            Dim parIsForExport As New SqlClient.SqlParameter("@IsForExport", SqlDbType.Bit)
            parIsForExport.Value = 1
            Command.Parameters.Add(parIsForExport)

            Dim parResult As New SqlClient.SqlParameter("@Result", SqlDbType.NVarChar, -1)
            parResult.Direction = ParameterDirection.Output
            Command.Parameters.Add(parResult)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If reader.HasRows Then
                'If (ReportID = 28) Then
                '    Do While reader.Read
                '        res = New NewReports
                '        'res.ID = reader.Item("ID")
                '        res.UserToken = reader.Item("Token")
                '        res.HDevicesID = reader.Item("HDevicesID")
                '        res.Name = reader.Item("Name")
                '        res.EventDate = reader.Item("EventDate")
                '        res.SensorID = reader.Item("SensorID")
                '        res.Temp = reader.Item("Temp")
                '        ListJson.Add(res)

                '    Loop
                If (ReportID = 22) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonActivityDetailReport With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .ThisDay = reader.Item("ThisDay").ToString(),
                            .ThisTime = reader.Item("ThisTime").ToString(),
                            .EventName = reader.Item("EventName"),
                            .MaxSpeed = reader.Item("MaxSpeed").ToString(),
                            .Duration = reader.Item("Duration").ToString(),
                            .Meters = reader.Item("Meters").ToString(),
                            .FullAddress = reader.Item("FullAddress"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .DriverName = reader.Item("DriverName")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 23) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonDailyActivityReport With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .IgnitionOn = reader.Item("IgnitionOn"),
                            .Departed = reader.Item("Departed"),
                            .Arrived = reader.Item("Arrived"),
                            .IgnitionOff = reader.Item("IgnitionOff"),
                            .DurationMinutes = reader.Item("DurationMinutes"),
                            .StoppedMinutes = reader.Item("StoppedMinutes"),
                            .Miles = reader.Item("Miles")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 1 Or ReportID = 2 Or ReportID = 3) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_BasicEvents2 With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .EventDate = reader.Item("EventDate"),
                            .EventName = reader.Item("EventName"),
                            .Speed = reader.Item("Speed"),
                            .Heading = reader.Item("Heading"),
                            .FullAddress = reader.Item("FullAddress"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .Lat = reader.Item("Lat"),
                            .Lng = reader.Item("Lng")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 4) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_Idles With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .IdleLocation = reader.Item("IdleLocation"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .IdleStart = reader.Item("IdleStart"),
                            .IdleEnd = reader.Item("IdleEnd"),
                            .Duration = reader.Item("Duration"),
                            .Latitude = reader.Item("Latitude"),
                            .Longitude = reader.Item("Longitude")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 5) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_Utilization_v2 With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .TotalMiles = reader.Item("TotalMiles"),
                            .TotalTravelTime = reader.Item("TotalTravelTime"),
                            .TotalIdlingTime = reader.Item("TotalIdlingTime"),
                            .PorcInTransit = reader.Item("PorcInTransit"),
                            .PorcIdle = reader.Item("PorcIdle"),
                            .PorcStopped = reader.Item("PorcStopped"),
                            .Speed00_35 = reader.Item("Speed00_35"),
                            .Speed36_65 = reader.Item("Speed36_65"),
                            .SpeedOver65 = reader.Item("SpeedOver65"),
                            .SpeedingAlerts = reader.Item("SpeedingAlerts"),
                            .IdleAlerts = reader.Item("IdleAlerts"),
                            .QtyStops = reader.Item("QtyStops")}
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 6 Or ReportID = 24 Or ReportID = 26) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_StateMiles With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .EventDate = reader.Item("EventDate"),
                            .StateName = reader.Item("StateName"),
                            .EventMiles = reader.Item("EventMiles")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 7 Or ReportID = 15) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonGeofencesInOut With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .Arrival = reader.Item("Arrival"),
                            .Departure = reader.Item("Departure"),
                            .Duration = reader.Item("Duration")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 10) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonTimeCard With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .StartLocation = reader.Item("StartLocation"),
                            .StartTime = reader.Item("StartTime"),
                            .EndLocation = reader.Item("EndLocation"),
                            .EndTime = reader.Item("EndTime"),
                            .WorkMinutes = reader.Item("WorkMinutes")
                        }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 11) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_ShortTrips With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .StartTime = reader.Item("StartTime"),
                            .StartAddress = reader.Item("StartAddress"),
                            .StartGeofenceID = reader.Item("StartGeofenceID"),
                            .TripDurationSecs = reader.Item("TripDurationSecs"),
                            .TripDistanceMeters = reader.Item("TripDistanceMeters"),
                            .TripMaxSpeed = reader.Item("TripMaxSpeed"),
                            .StopTime = reader.Item("StopTime"),
                            .StopAddress = reader.Item("StopAddress"),
                            .StopGeofenceID = reader.Item("StopGeofenceID"),
                            .StopDurationSecs = reader.Item("StopDurationSecs")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 12) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_Stops With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .StopAddress = reader.Item("StopAddress"),
                            .TripDistanceMeters = reader.Item("TripDistanceMeters"),
                            .TripMaxSpeed = reader.Item("TripMaxSpeed"),
                            .TripDurationSecs = reader.Item("TripDurationSecs"),
                            .StopTime = reader.Item("StopTime"),
                            .NextStartTime = reader.Item("NextStartTime"),
                            .StopDurationSecs = reader.Item("StopDurationSecs"),
                            .StopLatitude = reader.Item("StopLatitude"),
                            .StopLongitude = reader.Item("StopLongitude"),
                            .StopGeofenceName = reader.Item("StopGeofenceName")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 13 Or ReportID = 25) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_StateMiles2 With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .StateName = reader.Item("StateName"),
                            .EventMiles = reader.Item("EventMiles")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 14) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DailyPerformance With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .EventDate = reader.Item("EventDate"),
                            .TotalMiles = reader.Item("TotalMiles"),
                            .TotalTravelTime = reader.Item("TotalTravelTime"),
                            .TotalIdlingTime = reader.Item("TotalIdlingTime"),
                            .PorcInTransit = reader.Item("PorcInTransit"),
                            .PorcIdle = reader.Item("PorcIdle"),
                            .PorcStopped = reader.Item("PorcStopped"),
                            .Speed00_35 = reader.Item("Speed00_35"),
                            .Speed36_65 = reader.Item("Speed36_65"),
                            .SpeedOver65 = reader.Item("SpeedOver65"),
                            .FirstIgnitionON = reader.Item("FirstIgnitionON"),
                            .LastIgnitionOFF = reader.Item("LastIgnitionOFF"),
                            .SpeedingAlerts = reader.Item("SpeedingAlerts"),
                            .IdleAlerts = reader.Item("IdleAlerts"),
                            .QtyStops = reader.Item("QtyStops")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 18) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DriverLogSummary With {
                            .ReportName = reader.Item("ReportName"),
                            .DriverName = reader.Item("DriverName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .LogInTime = reader.Item("LogInTime"),
                            .LogOutTime = reader.Item("LogOutTime"),
                            .Duration = reader.Item("Duration")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 19) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DriverLogDetailed With {
                            .ReportName = reader.Item("ReportName"),
                            .DriverName = reader.Item("DriverName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .StatusName = reader.Item("StatusName"),
                            .EventDate = reader.Item("EventDate")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 20) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_TRU With {
                            .ReportName = reader.Item("ReportName"),
                            .CompanyName = reader.Item("CompanyName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .ARBNumber = reader.Item("ARBNumber"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .GeofenceType = reader.Item("GeofenceType"),
                            .GeoTimeIn = reader.Item("GeoTimeIn"),
                            .GeoTimeOut = reader.Item("GeoTimeOut"),
                            .GeoMinutes = reader.Item("GeoMinutes"),
                            .DieselInitMinutes = reader.Item("DieselInitMinutes"),
                            .DieselEndMinutes = reader.Item("DieselEndMinutes"),
                            .DieselMinutes = reader.Item("DieselMinutes"),
                            .ElectricInitMinutes = reader.Item("ElectricInitMinutes"),
                            .ElectricEndMinutes = reader.Item("ElectricEndMinutes"),
                            .ElectricMinutes = reader.Item("ElectricMinutes")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 21) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_TRU_Log With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .Obj = reader.Item("Obj"),
                            .EventType = reader.Item("EventType"),
                            .EventDate = reader.Item("EventDate"),
                            .PartialAcumMinutes = reader.Item("PartialAcumMinutes"),
                            .AcumMinutes = reader.Item("AcumMinutes"),
                            .DieselAcumMinutes = reader.Item("DieselAcumMinutes"),
                            .DieselStatus = reader.Item("DieselStatus"),
                            .ElectricAcumMinutes = reader.Item("ElectricAcumMinutes"),
                            .ElectricStatus = reader.Item("ElectricStatus")
                        }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 27) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DriverBehaviorLog With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DriverFirstName = reader.Item("DriverFirstName"),
                            .DriverLastName = reader.Item("DriverLastName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .EventName = reader.Item("EventName"),
                            .FullAddress = reader.Item("FullAddress"),
                            .EventDate = reader.Item("EventDate"),
                            .Speed = reader.Item("Speed"),
                            .miliGs = reader.Item("miliGs")
                        }
                        ListJson.Add(res)
                    Loop
                    'ElseIf (ReportID = 28) Then
                    '    Do While reader.Read
                    '        'res.ID = reader.Item("ID")
                    '        res = New JsonReports_TemperatureLog_NEW With {
                    '            .HDevicesID = reader.Item("HDevicesID"),
                    '            .Name = reader.Item("Name"),
                    '            .EventDate = reader.Item("EventDate"),
                    '            .SensorID = reader.Item("SensorID"),
                    '            .Temp = reader.Item("Temp")
                    '        }
                    '        ListJson.Add(res)
                    '    Loop
                ElseIf (ReportID = 28) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_TemperatureLog_NEW With {
                            .HDevicesID = reader.Item("HDevicesID"),
                            .EventDate = reader.Item("EventDate"),
                            .Speed = reader.Item("Speed"),
                            .FullAddress = reader.Item("FullAddress"),
                            .Latitude = reader.Item("Latitude"),
                            .Longitude = reader.Item("Longitude"),
                            .Temperature1 = reader.Item("Temperature1"),
                            .Temperature2 = reader.Item("Temperature2"),
                            .Temperature3 = reader.Item("Temperature3"),
                            .Temperature4 = reader.Item("Temperature4"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .EventName = reader.Item("EventName")
                        }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 29) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_Devices_InputsTimer With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .IgnTimer = reader.Item("IgnTimer"),
                            .SW1Timer = reader.Item("SW1Timer"),
                            .SW2Timer = reader.Item("SW2Timer"),
                            .SW3Timer = reader.Item("SW3Timer"),
                            .SW4Timer = reader.Item("SW4Timer")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 30) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_InputsTimersByHour With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .ThisDateTime = reader.Item("ThisDateTime"),
                            .IgnTimer = reader.Item("IgnTimer"),
                            .SW1Timer = reader.Item("SW1Timer"),
                            .SW2Timer = reader.Item("SW2Timer"),
                            .SW3Timer = reader.Item("SW3Timer"),
                            .SW4Timer = reader.Item("SW4Timer")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 31) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_InputsTimersByHour With {
                            .ReportName = reader.Item("ReportName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .ThisDateTime = reader.Item("ThisDay"),
                            .IgnTimer = reader.Item("IgnTimer"),
                            .SW1Timer = reader.Item("SW1Timer"),
                            .SW2Timer = reader.Item("SW2Timer"),
                            .SW3Timer = reader.Item("SW3Timer"),
                            .SW4Timer = reader.Item("SW4Timer")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 32) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_FuelLog With {
                            .ReportName = reader.Item("ReportName"),
                            .DateFrom = reader.Item("DateFrom"),
                            .DateTo = reader.Item("DateTo"),
                            .DeviceName = reader.Item("DeviceName"),
                            .FuelDate = reader.Item("FuelDate"),
                            .DeviceOdometer = reader.Item("DeviceOdometer"),
                            .Gallons = reader.Item("Gallons"),
                            .TotalCost = reader.Item("TotalCost"),
                            .Address = reader.Item("Address"),
                            .PostalCode = reader.Item("PostalCode"),
                            .StateProv = reader.Item("StateProv"),
                            .Comments = reader.Item("Comments")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 33) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DevicesNotWorking With {
                            .counter = reader.Item("counter"),
                            .name = reader.Item("name"),
                            .LastUpdatedOn = reader.Item("LastUpdatedOn")
                        }
                        ListJson.Add(res)

                    Loop
                ElseIf (ReportID = 34) Then 'pendiente terminaar
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_GeofencesVisits With {
                            .ReportName = reader.Item("ReportName"),
                            .GeofenceName = reader.Item("GeofenceName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .Arrival = reader.Item("Arrival"),
                            .Departure = reader.Item("Departure"),
                            .Duration = reader.Item("Duration")
                        }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 35) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DevicesList With {
                                .ReportName = reader.Item("ReportName"),
                                .DeviceID = reader.Item("DeviceID"),
                                .DeviceType = reader.Item("DeviceType"),
                                .DeviceName = reader.Item("DeviceName"),
                                .ServerDate = reader.Item("ServerDate"),
                                .EventDate = reader.Item("EventDate"),
                                .EventName = reader.Item("EventName"),
                                .Speed = reader.Item("Speed"),
                                .Heading = reader.Item("Heading"),
                                .FullAddress = reader.Item("FullAddress"),
                                .SerialNumber = reader.Item("SerialNumber"),
                                .GPSStatus = reader.Item("GPSStatus"),
                                .GPSAge = reader.Item("GPSAge")
                            }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 36) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_DriverWorkHoursbyiButton With {
                            .ReportName = reader.Item("ReportName"),
                            .DriverName = reader.Item("DriverName"),
                            .DeviceName = reader.Item("DeviceName"),
                            .ClockIn = reader.Item("ClockIn"),
                            .ClockOut = reader.Item("ClockOut"),
                            .TotalHours = reader.Item("TotalHours")
                        }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 38) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_PointtoPoint With {
                                .ReportName = reader.Item("ReportName"),
                                .DateFrom = reader.Item("DateFrom"),
                                .DateTo = reader.Item("DateTo"),
                                .DeviceName = reader.Item("DeviceName"),
                                .StartTime = reader.Item("StartTime"),
                                .StartAddress = reader.Item("StartAddress"),
                                .StartGeofenceID = reader.Item("StartAddress"),
                                .StopTime = reader.Item("StopTime"),
                                .StopAddress = reader.Item("StopAddress"),
                                .StopGeofenceID = reader.Item("StopAddress")
                            }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 39) Then
                    Do While reader.Read
                        'res.ID = reader.Item("ID")
                        res = New JsonReports_Latitude_Longitude With {
                                .ReportName = reader.Item("ReportName"),
                                .DateFrom = reader.Item("DateFrom"),
                                .DateTo = reader.Item("DateTo"),
                                .DeviceName = reader.Item("DeviceName"),
                                .EventDate = reader.Item("EventDate"),
                                .EventName = reader.Item("EventName"),
                                .Latitude = reader.Item("Latitude"),
                                .Longitude = reader.Item("Longitude"),
                                .FullAddress = reader.Item("FullAddress"),
                                .GeofenceName = reader.Item("GeofenceName")
                            }
                        ListJson.Add(res)
                    Loop
                ElseIf (ReportID = 40) Then
                    Do While reader.Read
                        res = New JsonReports_TemperatureLog_NEW2 With {
                            .Name = reader.Item("Name"),
                            .EventDate = reader.Item("EventDate"),
                            .NameSensor = reader.Item("NameSensor"),
                            .SensorID = reader.Item("SensorID"),
                            .Temp = reader.Item("Temp"),
                            .LightLevel = reader.Item("LightLevel"),
                            .BatteryLevel = reader.Item("BatteryLevel"),
                            .Humidity = reader.Item("Humidity"),
                            .Altitude = reader.Item("Altitude"),
                            .RSSI = reader.Item("RSSI")
                        }
                        ListJson.Add(res)
                    Loop
                Else
                    ListJson = Nothing
                End If
            Else
                ListJson = Nothing
            End If
            If Not reader.IsClosed Then
                reader.Close()
            End If
        Catch ex As Exception
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return ListJson


    End Function
    Function CompaniesDevicesEvents(ByVal Token As String) As List(Of Object)
        Dim res
        Dim ListJson As List(Of Object) = New List(Of Object)
        Dim reader As SqlDataReader = Nothing
        Try
            'strCommand = "etAPI_ReportsKendo"
            strCommand = "CompaniesDevicesEvents_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            Do While reader.Read
                'res.ID = reader.Item("ID")
                res = New JsonCompaniesDevicesEvents With {
                    .EventCode = reader.Item("EventCode"),
                    .Name = reader.Item("Name"),
                    .AlertTypeID = reader.Item("AlertTypeID"),
                    .IsInternalEvent = reader.Item("IsInternalEvent"),
                    .IsDriverBehavior = reader.Item("IsDriverBehavior"),
                    .IsInput = reader.Item("IsInput"),
                    .InputNum = reader.Item("InputNum"),
                    .IsPortExpander = reader.Item("IsPortExpander"),
                    .EventDescription = reader.Item("EventDescription"),
                    .IsMoving = reader.Item("IsMoving"),
                    .IsStopped = reader.Item("IsStopped"),
                    .IsHealthCheck = reader.Item("IsHealthCheck")
                }
                ListJson.Add(res)

            Loop
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try


        Return ListJson

    End Function
    Public Function getGeofences_All(ByVal Token As String, ByVal isExtended As Boolean, ByRef errMsg As String) As List(Of Object)
        Dim res
        Dim ListJson As List(Of Object) = New List(Of Object)
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Geofences_GetAll"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parExt As New SqlClient.SqlParameter("@Extended", SqlDbType.Bit)
            parExt.Value = isExtended
            Command.Parameters.Add(parExt)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            Do While reader.Read
                'res.ID = reader.Item("ID")
                res = New JsonGeoFencesAll With {
                    .Id = reader.Item("GUID"),
                    .Name = reader.Item("Name"),
                    .FullAddress = reader.Item("FullAddress"),
                    .Latitude = reader.Item("Latitude"),
                    .Longitude = reader.Item("Longitude"),
                    .RadiusFeet = reader.Item("RadiusFeet"),
                    .GeofenceAlertTypeName = reader.Item("GeofenceAlertTypeName"),
                    .GeofenceTypeName = reader.Item("GeofenceTypeName"),
                    .ShapeID = reader.Item("ShapeID"),
                    .jsonPolyVerticesTXT = reader.Item("jsonPolyVerticesTXT"),
                    .IconURL = reader.Item("IconURL"),
                    .GeofenceInfoTable = reader.Item("GeofenceInfoTable")
                }
                ListJson.Add(res)
            Loop
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try


        Return ListJson

    End Function
    Public Function GetDrivers(ByVal Token As String) As List(Of Object)
        Dim res
        Dim ListJson As New List(Of Object)
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Drivers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50) With {
                .Value = Token
            }
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            Do While reader.Read
                'res.ID = reader.Item("ID")
                res = New JsonGetDriver With {
                    .ID = reader.Item("ID"),
                    .Name = reader.Item("Name"),
                    .Phone = reader.Item("Phone"),
                    .Email = reader.Item("Email")
                }
                ListJson.Add(res)
            Loop
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return ListJson

    End Function
    Public Function GetMultiDayTrail(ByVal Token As String, ByVal DeviceID As String, ByVal DateFrom As String, ByVal DateTo As String, ByVal IsExport As Boolean) As List(Of Object)
        Dim res
        Dim ListJson As New List(Of Object)
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Reports_MultiDayTrail"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50) With {
                .Value = Token
            }
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50) With {
                .Value = "A288"
            }
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime) With {
                .Value = DateFrom
            }
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime) With {
                .Value = DateTo
            }
            Command.Parameters.Add(parDateTo)

            Dim parIsExport As New SqlClient.SqlParameter("@IsExport", SqlDbType.Bit) With {
                .Value = IsExport
            }
            Command.Parameters.Add(parIsExport)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    res = New JsonReports_MultiDayTrail With {
                        .DeviceID = reader.Item("DeviceID"),
                        .DeviceName = reader.Item("DeviceName"),
                        .ActivityDate = reader.Item("ActivityDate"),
                        .FirstIgnitionON = reader.Item("FirstIgnitionON"),
                        .AddressON = reader.Item("AddressON"),
                        .LastIgnitionOFF = reader.Item("LastIgnitionOFF"),
                        .AddressOFF = reader.Item("AddressOFF"),
                        .MilesDriven = reader.Item("MilesDriven"),
                        .HoursWorked = reader.Item("HoursWorked")
                    }
                    ListJson.Add(res)
                Loop
            Else
                ListJson = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return ListJson
    End Function
    Function getTroubleshootingReport(ByVal token As String, ByVal deviceId As String, ByVal dateFrom As String, ByVal dateTo As String) As List(Of Object)
        Dim res
        Dim ListJson As New List(Of Object)
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Reports_Troubleshooting"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50)
            parDeviceID.Value = "A288"
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.Date)
            parDateFrom.Value = CDate(dateFrom)
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.Date)
            parDateTo.Value = CDate(dateTo)
            Command.Parameters.Add(parDateTo)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    res = New TroubleLog With {
                        .DeviceID = reader.Item("DeviceID"),
                        .DeviceName = reader.Item("DeviceName"),
                        .SerialNumber = reader.Item("SerialNumber"),
                        .LastUpdatedOn = reader.Item("LastUpdatedOn"),
                        .NoShowDays = reader.Item("NoShowDays"),
                        .PowerCut = reader.Item("PowerCut"),
                        .MainPowerRestored = reader.Item("MainPowerRestored"),
                        .IllegalPowerUp = reader.Item("IllegalPowerUp"),
                        .PowerDown = reader.Item("PowerDown"),
                        .IgnOffGPS15 = reader.Item("IgnOffGPS15"),
                        .IgnOffSpeed10 = reader.Item("IgnOffSpeed10"),
                        .PowerUp = reader.Item("PowerUp"),
                        .PowerOffBatt = reader.Item("PowerOffBatt"),
                        .TotalEvents = reader.Item("TotalEvents")
                    }
                    ListJson.Add(res)
                Loop
            End If
            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception
            ListJson = Nothing

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return ListJson

    End Function



#End Region
#Region "Online users"

    Function getOnlineUsers(ByVal token As String) As realTimeActivity
        Dim rta As New realTimeActivity
        Dim lst As New List(Of onlineUser)
        Dim itm As onlineUser

        Try
            strCommand = "CRM_Engagement_OnlineUsers_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New onlineUser
                itm.id = reader.Item("ID")
                itm.companyName = reader.Item("CompanyName")
                itm.userName = reader.Item("UserName")
                itm.phone = reader.Item("Phone")
                itm.mobile = reader.Item("CellPhone")
                itm.email = reader.Item("Email")
                itm.currentPage = reader.Item("CurrentPage")
                itm.currentPageTime = reader.Item("CurrentPageTime")
                itm.sessionTime = reader.Item("SessionTime").ToString
                itm.qtyUnits = reader.Item("QtyUnits")
                lst.Add(itm)
            Loop

            rta.onlineUsers = lst

            reader.NextResult()
            Do While reader.Read
                rta.qtyUnits = reader.Item("QtyUnits")
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return rta

    End Function

#End Region
#Region "OnBoarding"

    Function onboardingPendingCustomers(ByVal token As String) As List(Of pendingOnBoarding)
        Dim itm As pendingOnBoarding
        Dim lst As New List(Of pendingOnBoarding)

        Try
            strCommand = "CRM_OnBoarding_GetPending"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New pendingOnBoarding
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.email = reader.Item("Email")
                itm.phone = reader.Item("Phone")
                itm.billCity = reader.Item("BillCity")
                itm.billState = reader.Item("BillState")
                itm.shipCity = reader.Item("ShipCity")
                itm.shipState = reader.Item("ShipState")
                itm.createdOn = reader.Item("CreatedOn").ToString
                lst.Add(itm)
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return lst

    End Function

    Function onboardingPendingCustomerGet(ByVal token As String, ByVal id As String) As onBoardingCustomer
        Dim itm As New onBoardingCustomer

        Try
            strCommand = "CRM_OnBoarding_GetCustomer"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 20)
            parID.Value = id
            Command.Parameters.Add(parID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            Do While reader.Read
                itm = New onBoardingCustomer
                itm.id = reader.Item("ID")
                itm.name = reader.Item("Name")
                itm.email = reader.Item("Email")
                itm.phone = reader.Item("Phone")
                itm.billCity = reader.Item("BillCity")
                itm.billState = reader.Item("BillState")
                itm.shipCity = reader.Item("ShipCity")
                itm.shipState = reader.Item("ShipState")
                itm.createdOn = reader.Item("CreatedOn").ToString
            Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return itm

    End Function

    Function onBoardingDone(ByVal token As String, ByVal id As String, ByRef msg As String) As Boolean
        Dim res As Boolean = True

        Try
            strCommand = "CRM_OnBoarding_CustomerDone"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.NVarChar, 20)
            parID.Value = id
            Command.Parameters.Add(parID)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res = CStr(parIsOk.Value)


        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region
#Region "FeedBack"
    Public Function PostSendFeedBack(ByVal Token As String, ByVal visitedPage As String, ByVal type As String, ByVal descríption As String) As Boolean
        Dim res As Boolean = False
        Try
            strCommand = "SP_CRM_CustomerFeedback"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parVisitedPage As New SqlClient.SqlParameter("@VisitedPage", SqlDbType.NVarChar, 50)
            parVisitedPage.Direction = ParameterDirection.Input
            parVisitedPage.Value = visitedPage
            Command.Parameters.Add(parVisitedPage)

            Dim parType As New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)
            parType.Direction = ParameterDirection.Input
            parType.Value = Integer.Parse(type)
            Command.Parameters.Add(parType)

            Dim pardescríption As New SqlClient.SqlParameter("@Description", SqlDbType.NVarChar, 50)
            pardescríption.Direction = ParameterDirection.Input
            pardescríption.Value = descríption
            Command.Parameters.Add(pardescríption)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOK", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            res = CStr(parIsOk.Value)
        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function
    Function GetFeedbakTypes(ByVal token As String) As String
        Dim itm As FeedBackTypes
        Dim listItm As List(Of FeedBackTypes) = New List(Of FeedBackTypes)
        Dim result As String = ""
        Dim response As response = New response
        Try
            strCommand = "SP_CRM_FeedbakTypes"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            response.Status = "OK"
            response.Messagge = ""
            Do While reader.Read
                itm = New FeedBackTypes
                itm.ID = reader.Item("ID")
                itm.Name = reader.Item("Name")
                response.ListResponse.Add(itm)
            Loop
            If Not reader.IsClosed Then
                reader.Close()
            End If
        Catch ex As Exception
            response.Status = "-1"
            response.Messagge = ex.Message
        Finally
            result = JsonConvert.SerializeObject(response)
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return result

    End Function
#End Region
#Region "VIDEO"
    Public Function ValidateTokenVideo(ByVal Token As String, ByVal ExternalProvider As Boolean) As DataSet
        Dim dsData As New DataSet

        Try
            strCommand = "Video_ValidateTokenAPP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim parExternalProvider As New SqlClient.SqlParameter("@ExternalProvider", SqlDbType.Bit)
            parExternalProvider.Direction = ParameterDirection.Input
            parExternalProvider.Value = ExternalProvider
            Command.Parameters.Add(parExternalProvider)


            adapter = New SqlDataAdapter(Command)
            adapter.Fill(dsData)
            adapter.Dispose()
            Command.Dispose()

        Catch ex As Exception
            dsData = Nothing
            If ex.Message = "LOGOUT" Then
                strError = "TOKENEXPIRED"
            Else
                strError = "OTHERERROR"
            End If
        Finally

            conSQL.Dispose()
        End Try

        Return dsData

    End Function

#End Region
#Region "TempSensor"
    Public Function PostTempSensor(ByVal Token As String, ByVal TempSensors As TempSensors, ByVal paction As Integer) As Integer
        Dim res As Integer = 0
        Try
            strCommand = "TempSensors_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 255)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim ID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = TempSensors.ID
            Command.Parameters.Add(ID)

            Dim IMEI As New SqlClient.SqlParameter("@IMEI", SqlDbType.NVarChar, 255)
            IMEI.Direction = ParameterDirection.Input
            IMEI.Value = TempSensors.IMEI
            Command.Parameters.Add(IMEI)

            Dim TempNumber As New SqlClient.SqlParameter("@TempNumber", SqlDbType.TinyInt)
            TempNumber.Direction = ParameterDirection.Input
            TempNumber.Value = TempSensors.TempNumber
            Command.Parameters.Add(TempNumber)

            Dim SensorID As New SqlClient.SqlParameter("@SensorID", SqlDbType.NVarChar, 50)
            SensorID.Direction = ParameterDirection.Input
            SensorID.Value = TempSensors.SensorID
            Command.Parameters.Add(SensorID)

            Dim Name As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            Name.Direction = ParameterDirection.Input
            Name.Value = TempSensors.Name
            Command.Parameters.Add(Name)

            Dim Did As New SqlClient.SqlParameter("@Did", SqlDbType.NVarChar, 100)
            Did.Direction = ParameterDirection.Input
            Did.Value = TempSensors.Did
            Command.Parameters.Add(Did)

            Dim Reassigned As New SqlClient.SqlParameter("@Reassigned", SqlDbType.Bit)
            Reassigned.Direction = ParameterDirection.Input
            Reassigned.Value = TempSensors.Reassigned
            Command.Parameters.Add(Reassigned)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Direction = ParameterDirection.Input
            Action.Value = paction
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOK", SqlDbType.TinyInt)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            res = parIsOk.Value

            'res = CStr(parIsOk.Value)
        Catch ex As Exception
            Console.WriteLine(ex.Message)


        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function
    Public Function GetSensors(ByVal Token As String, ByVal DeviceID As String, ByVal DateFrom As String, ByVal DateTo As String, ByVal IsExport As Boolean) As List(Of Object)
        Dim res
        Dim ListJson As New List(Of Object)
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "TempSensors_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50) With {
                .Value = Token
            }
            Command.Parameters.Add(parToken)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 50) With {
                .Value = "A288"
            }
            Command.Parameters.Add(parDeviceID)

            Dim parDateFrom As New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime) With {
                .Value = DateFrom
            }
            Command.Parameters.Add(parDateFrom)

            Dim parDateTo As New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime) With {
                .Value = DateTo
            }
            Command.Parameters.Add(parDateTo)

            Dim parIsExport As New SqlClient.SqlParameter("@IsExport", SqlDbType.Bit) With {
                .Value = IsExport
            }
            Command.Parameters.Add(parIsExport)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    res = New JsonReports_MultiDayTrail With {
                        .DeviceID = reader.Item("DeviceID"),
                        .DeviceName = reader.Item("DeviceName"),
                        .ActivityDate = reader.Item("ActivityDate"),
                        .FirstIgnitionON = reader.Item("FirstIgnitionON"),
                        .AddressON = reader.Item("AddressON"),
                        .LastIgnitionOFF = reader.Item("LastIgnitionOFF"),
                        .AddressOFF = reader.Item("AddressOFF"),
                        .MilesDriven = reader.Item("MilesDriven"),
                        .HoursWorked = reader.Item("HoursWorked")
                    }
                    ListJson.Add(res)
                Loop
            Else
                ListJson = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return ListJson
    End Function
    Public Function GetSensors2(ByVal Token As String) As List(Of TempSensors)
        Dim res
        Dim ListJson As New List(Of TempSensors)
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "TempSensors_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50) With {
                .Value = Token
            }
            Command.Parameters.Add(parToken)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 2
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    res = New TempSensors With {
                        .ID = reader.Item("ID"),
                        .Devices = reader.Item("Devices"),
                        .Name = reader.Item("Name"),
                        .TempNumber = reader.Item("TempNumber"),
                        .LastUpdatedOn = reader.Item("LastUpdatedOn")
                    }
                    ListJson.Add(res)
                Loop
            Else
                ListJson = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            ListJson = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return ListJson
    End Function


#End Region
#Region "Trakingnumber"
    Public Function GettrakingnumberExt(ByVal ptrakingnumber As String) As TrakingNumberExt
        Dim res As New TrakingNumberExt
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "TrakingNumber_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim TrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 100) With {
                .Value = ptrakingnumber
            }
            Command.Parameters.Add(TrackingNumber)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 2
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    res = New TrakingNumberExt With {
                        .Device = reader.Item("Device"),
                        .TrackingNumber = reader.Item("TrackingNumber"),
                        .ValidUntil = reader.Item("ValidUntil"),
                        .Message = reader.Item("Message"),
                        .Lat = reader.Item("Lat"),
                        .Lng = reader.Item("lng"),
                        .MapIcon = reader.Item("MapIcon"),
                        .Flag_Expired = reader.Item("Flag_Expired"),
                        .lattarget = reader.Item("lattarget"),
                        .longtarget = reader.Item("longtarget"),
                        .icontarget = reader.Item("icontarget"),
                        .Flat_FromJob = reader.Item("Flat_FromJob"),
                        .Flat_FromBrokerOrder = reader.Item("Flat_FromBrokerOrder"),
                        .PickupAddress = reader.Item("PickupAddress"),
                        .Pickupdetetime = reader.Item("Pickupdetetime"),
                        .PickupAddresscoordinatesLat = reader.Item("PickupAddresscoordinatesLat"),
                        .PickupAddresscoordinatesLng = reader.Item("PickupAddresscoordinatesLng"),
                        .DeliveryAddress = reader.Item("DeliveryAddress"),
                        .Deliverydatetime = reader.Item("Deliverydatetime"),
                        .DeliveryAddressscoordinatesLat = reader.Item("DeliveryAddressscoordinatesLat"),
                        .DeliveryAddressscoordinatesLng = reader.Item("DeliveryAddressscoordinatesLng")
                        }
                Loop
            Else
                res = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            res = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return res
    End Function
    Public Function Gettrakingnumber(ByVal ptoken As String) As List(Of TrakingNumber)
        Dim res As New List(Of TrakingNumber)
        Dim trakingnumber
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "TrakingNumber_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim token As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 100) With {
                .Value = ptoken
            }
            Command.Parameters.Add(token)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 5
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    trakingnumber = New TrakingNumber With {
                        .ID = reader.Item("ID"),
                        .TypeID = reader.Item("TypeID"),
                        .CompanyID = reader.Item("CompanyID"),
                        .DeviceID = reader.Item("DeviceID"),
                        .TrackingNumber = reader.Item("TrackingNumber"),
                        .CreatedOn = reader.Item("CreatedOn"),
                        .UserID = reader.Item("UserID"),
                        .WorkOrderID = reader.Item("WorkOrderID"),
                        .SendTo = reader.Item("SendTo"),
                        .Flag_Sent = reader.Item("Flag_Sent"),
                        .SentOn = reader.Item("SentOn"),
                        .ValidUntil = reader.Item("ValidUntil"),
                        .Flag_Expired = reader.Item("Flag_Expired"),
                        .UTC_Code = reader.Item("UTC_Code"),
                        .UTC_Convertion = reader.Item("UTC_Convertion"),
                        .Message = reader.Item("Message"),
                        .FreezeMap = reader.Item("FreezeMap"),
                        .FreezeLat = reader.Item("FreezeLat"),
                        .FreezeLon = reader.Item("FreezeLon"),
                        .ZoomLevel = reader.Item("ZoomLevel"),
                        .MapType = reader.Item("MapType"),
                        .MapIcon = reader.Item("MapIcon"),
                        .URLTraking = reader.Item("URLTraking"),
                        .Device = reader.Item("Device"),
                        .Lat = reader.Item("Lat"),
                        .Lng = reader.Item("Lng")
                    }
                    res.Add(trakingnumber)
                Loop
            Else
                res = Nothing
            End If
        Catch ex As Exception
            'res.isOk = False
            'res.msg = ex.Message
            'BLErrorHandling.ErrorCapture(pSysModule, "DL.CRM_InsFromApp", "", ex.Message & " - Token: " & data.Token, 0)
            res = Nothing
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try
        Return res
    End Function
    Public Function Puttrakingnumber(ByVal Token As String, ByVal pTrakingNumber As TrakingNumber, ByVal paction As Integer) As Integer
        Dim res As Integer = 0
        Try
            strCommand = "TrakingNumber_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 255)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)

            Dim ID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = pTrakingNumber.ID
            Command.Parameters.Add(ID)

            Dim ValidUntil As New SqlClient.SqlParameter("@ValidUntil", SqlDbType.DateTime)
            ValidUntil.Direction = ParameterDirection.Input
            ValidUntil.Value = pTrakingNumber.ValidUntil
            Command.Parameters.Add(ValidUntil)

            Dim SendTo As New SqlClient.SqlParameter("@SendTo", SqlDbType.NVarChar, 255)
            SendTo.Direction = ParameterDirection.Input
            SendTo.Value = pTrakingNumber.SendTo
            Command.Parameters.Add(SendTo)

            Dim Flag_Expired As New SqlClient.SqlParameter("@Flag_Expired", SqlDbType.Bit)
            Flag_Expired.Direction = ParameterDirection.Input
            Flag_Expired.Value = pTrakingNumber.Flag_Expired
            Command.Parameters.Add(Flag_Expired)

            Dim Message As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 255)
            Message.Direction = ParameterDirection.Input
            Message.Value = pTrakingNumber.Message
            Command.Parameters.Add(Message)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Direction = ParameterDirection.Input
            Action.Value = paction
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOK", SqlDbType.TinyInt)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            res = parIsOk.Value

            'res = CStr(parIsOk.Value)
        Catch ex As Exception
            Console.WriteLine(ex.Message)


        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function
    Public Function Posttrakingnumber(ByVal Token As String, ByVal pTrakingNumber As TrakingNumber, ByVal paction As Integer, ByVal pemail As String) As Integer
        Dim res As Integer = 0
        Try
            strCommand = "TrakingNumber_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 255)
            parToken.Direction = ParameterDirection.Input
            parToken.Value = Token
            Command.Parameters.Add(parToken)


            Dim Device As New SqlClient.SqlParameter("@Device", SqlDbType.NVarChar, 50)
            Device.Direction = ParameterDirection.Input
            Device.Value = pTrakingNumber.Device
            Command.Parameters.Add(Device)

            Dim SendTo As New SqlClient.SqlParameter("@SendTo", SqlDbType.NVarChar, 255)
            SendTo.Direction = ParameterDirection.Input
            SendTo.Value = pTrakingNumber.SendTo
            Command.Parameters.Add(SendTo)

            Dim ValidUntil As New SqlClient.SqlParameter("@ValidUntil", SqlDbType.DateTime)
            ValidUntil.Direction = ParameterDirection.Input
            ValidUntil.Value = pTrakingNumber.ValidUntil
            Command.Parameters.Add(ValidUntil)

            Dim Message As New SqlClient.SqlParameter("@Message", SqlDbType.NVarChar, 255)
            Message.Direction = ParameterDirection.Input
            Message.Value = pTrakingNumber.Message
            Command.Parameters.Add(Message)

            Dim URL As New SqlClient.SqlParameter("@URL", SqlDbType.NVarChar, 255)
            URL.Direction = ParameterDirection.Input
            URL.Value = pTrakingNumber.URLTraking
            Command.Parameters.Add(URL)

            Dim Email As New SqlClient.SqlParameter("@Email", SqlDbType.NVarChar, 5000)
            Email.Direction = ParameterDirection.Input
            Email.Value = pemail
            Command.Parameters.Add(Email)

            Dim TrackingNumber As New SqlClient.SqlParameter("@TrackingNumber", SqlDbType.NVarChar, 100)
            TrackingNumber.Direction = ParameterDirection.Input
            TrackingNumber.Value = pTrakingNumber.TrackingNumber
            Command.Parameters.Add(TrackingNumber)

            Dim Flat_FromJob As New SqlClient.SqlParameter("@Flat_FromJob", SqlDbType.Bit)
            Flat_FromJob.Direction = ParameterDirection.Input
            Flat_FromJob.Value = pTrakingNumber.Flat_FromJob
            Command.Parameters.Add(Flat_FromJob)

            Dim GeofenceTargetJob As New SqlClient.SqlParameter("@GeofenceTargetJob", SqlDbType.Int)
            GeofenceTargetJob.Direction = ParameterDirection.Input
            GeofenceTargetJob.Value = pTrakingNumber.GeofenceTargetJob
            Command.Parameters.Add(GeofenceTargetJob)

            Dim JobUniqueKey As New SqlClient.SqlParameter("@JobUniqueKey", SqlDbType.NVarChar, 200)
            JobUniqueKey.Direction = ParameterDirection.Input
            JobUniqueKey.Value = pTrakingNumber.JobUniqueKey
            Command.Parameters.Add(JobUniqueKey)

            Dim Flat_FromBrokerOrder As New SqlClient.SqlParameter("@Flat_FromBrokerOrder", SqlDbType.Bit)
            Flat_FromBrokerOrder.Direction = ParameterDirection.Input
            Flat_FromBrokerOrder.Value = pTrakingNumber.Flat_FromBrokerOrder
            Command.Parameters.Add(Flat_FromBrokerOrder)

            Dim WorkOrderID As New SqlClient.SqlParameter("@WorkOrderID", SqlDbType.Int)
            WorkOrderID.Direction = ParameterDirection.Input
            WorkOrderID.Value = pTrakingNumber.WorkOrderID
            Command.Parameters.Add(WorkOrderID)



            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Direction = ParameterDirection.Input
            Action.Value = paction
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOK", SqlDbType.TinyInt)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            res = parIsOk.Value

            'res = CStr(parIsOk.Value)
        Catch ex As Exception
            Console.WriteLine(ex.Message)


        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return res

    End Function

#End Region
#Region "Broker Order"
    Public Function BrokerDetail(ByVal token As String, ByVal jobUniquekey As String) As List(Of BrokerOrder)

        Dim broker As New BrokerOrder
        Dim listbroker As New List(Of BrokerOrder)

        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "BrokerOrder_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIdJob As New SqlClient.SqlParameter("@uniqueKey", SqlDbType.NVarChar, 50)
            parIdJob.Value = jobUniquekey
            Command.Parameters.Add(parIdJob)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Value = 2
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            'Do While reader.Read
            '    broker = New BrokerOrder With {
            '        .UniqueKey = reader.Item("UniqueKey"),
            '        .DeviceName = reader.Item("DeviceName"),
            '        .DriverName = reader.Item("DriverName"),
            '        .JobName = reader.Item("JobName"),
            '        .JobNumber = reader.Item("JobNumber"),
            '        .PickupAddress = reader.Item("PickupAddress"),
            '        .Pickupdetetime = reader.Item("Pickupdetetime"),
            '        .DeliveryAddress = reader.Item("DeliveryAddress"),
            '        .Deliverydatetime = reader.Item("Deliverydatetime"),
            '        .Observations = reader.Item("Observations"),
            '        .SendTo = reader.Item("SendTo"),
            '        .TrackingNumber = reader.Item("TrackingNumber")
            '    }
            '    listbroker.Add(broker)

            'Loop

            If Not reader.IsClosed Then
                reader.Close()
            End If

        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return listbroker

    End Function
    Public Function BrokerResendEmail(ByVal token As String, ByVal jobUniquekey As String, ByVal email As String) As Boolean
        Dim resul As Boolean = False
        Try
            strCommand = "TrakingNumber_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIdJob As New SqlClient.SqlParameter("@JobUniqueKey", SqlDbType.NVarChar, 50)
            parIdJob.Value = jobUniquekey
            Command.Parameters.Add(parIdJob)

            Dim parSendTo As New SqlClient.SqlParameter("@SendTo", SqlDbType.NVarChar, 500)
            parSendTo.Value = email
            Command.Parameters.Add(parSendTo)


            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Value = 6
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Command.ExecuteNonQuery()
            resul = Convert.ToBoolean(parIsOk.Value)
        Catch ex As Exception

        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try

        Return resul

    End Function

#End Region
End Class
