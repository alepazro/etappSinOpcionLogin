﻿Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient

Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter
Imports etws

Public Class DataLayerJobs
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
#Region "Location"
    Function GeocodingLocation_GET(ByVal token As String, ByVal lat As Decimal, ByVal lon As Decimal, ByVal type As String, ByVal fullAddress As String) As String
        Dim itm As Location
        Dim result As String = ""
        Dim response As response = New response
        Try
            strCommand = "GeocodingLocation_GET"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parLat As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            parLat.Value = lat
            Command.Parameters.Add(parLat)

            Dim parLon As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            parLon.Value = lon
            Command.Parameters.Add(parLon)

            Dim partype As New SqlClient.SqlParameter("@Type", SqlDbType.NVarChar, 100)
            partype.Value = type
            Command.Parameters.Add(partype)

            Dim parFullAddress As New SqlClient.SqlParameter("@FullAddress", SqlDbType.NVarChar, 100)
            parFullAddress.Value = "5706 General Washington"
            Command.Parameters.Add(parFullAddress)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            response.Status = "OK"
            response.Messagge = "Requests OK."
            Do While reader.Read
                itm = New Location With {
                    .LatX4 = reader.Item("lat"),
                    .LngX4 = reader.Item("lng"),
                    .FullAddress = reader.Item("FullAddress"),
                    .Street = reader.Item("Street"),
                    .City = reader.Item("City"),
                    .State = reader.Item("State"),
                    .PostalCode = reader.Item("PostalCode"),
                    .County = reader.Item("County")
                }
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
    Public Function GeocodingLocation_POST(ByVal Token As String, ByVal data As Location) As response
        'Dim res As Boolean = False
        Dim response As response = New response
        Try
            strCommand = "GeocodingLocation_POST"
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

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Decimal)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = data.LatX4
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Decimal)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = data.LngX4
            Command.Parameters.Add(parLongitude)

            Dim parFullAddress As New SqlClient.SqlParameter("@FullAddress", SqlDbType.NVarChar, 200)
            parFullAddress.Direction = ParameterDirection.Input
            parFullAddress.Value = data.FullAddress
            Command.Parameters.Add(parFullAddress)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Direction = ParameterDirection.Input
            parStreet.Value = data.Street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Direction = ParameterDirection.Input
            parCity.Value = data.City
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Direction = ParameterDirection.Input
            parState.Value = data.State
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Direction = ParameterDirection.Input
            parPostalCode.Value = data.PostalCode
            Command.Parameters.Add(parPostalCode)

            Dim parCountryCode As New SqlClient.SqlParameter("@CountryCode", SqlDbType.NVarChar, 50)
            parCountryCode.Direction = ParameterDirection.Input
            parCountryCode.Value = data.CountryCode
            Command.Parameters.Add(parCountryCode)

            Dim parHits As New SqlClient.SqlParameter("@Hits", SqlDbType.Int)
            parHits.Direction = ParameterDirection.Input
            parHits.Value = data.Hits
            Command.Parameters.Add(parHits)

            Dim parCounty As New SqlClient.SqlParameter("@County", SqlDbType.NVarChar, 50)
            parCounty.Direction = ParameterDirection.Input
            parCounty.Value = data.County
            Command.Parameters.Add(parCounty)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOK", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            response.Status = CStr(parIsOk.Value)
            response.Messagge = "Success OK."
        Catch ex As Exception
            response.Status = "False"
            response.Messagge = "Error: " + ex.Message
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
        End Try
        Return response
    End Function
#End Region
#Region "jobs"
    Public Function saveJob(ByVal token As String, data As Jobobject) As String
        Dim JobGUID As String = ""
        Try
            'strCommand = "SendJobToDeviceViaSMS"
            'strCommand = "WorkOrders_QuickDispatch_SAVE"
            strCommand = "Jobs_QuickDispatch_SAVE_NEW"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parJobNumber As New SqlClient.SqlParameter("@JobNumber", SqlDbType.NVarChar, 20)
            parJobNumber.Value = data.JobNumber
            Command.Parameters.Add(parJobNumber)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.NVarChar, 4)
            parDeviceID.Value = data.deviceId
            Command.Parameters.Add(parDeviceID)

            Dim parDriverId As New SqlClient.SqlParameter("@DriverID", SqlDbType.Int)
            parDriverId.Value = Integer.Parse(data.driverId)
            Command.Parameters.Add(parDriverId)

            Dim parIsGeofence As New SqlClient.SqlParameter("@IsGeofence", SqlDbType.Bit)
            If data.isGeofence.ToLower = "true" Then
                parIsGeofence.Value = True
            Else
                parIsGeofence.Value = False
            End If
            Command.Parameters.Add(parIsGeofence)

            Dim parGeofenceID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.VarChar, 50)
            parGeofenceID.Value = data.geof_Id
            Command.Parameters.Add(parGeofenceID)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 50)
            parName.Value = data.geof_name
            Command.Parameters.Add(parName)

            Dim parPhone As New SqlClient.SqlParameter("@Phone", SqlDbType.NVarChar, 50)
            parPhone.Value = data.geof_phone
            Command.Parameters.Add(parPhone)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 50)
            parStreet.Value = data.geof_street
            Command.Parameters.Add(parStreet)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 20)
            parCity.Value = data.geof_city
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 15)
            parState.Value = data.geof_state
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 15)
            parPostalCode.Value = data.geof_postalCode
            Command.Parameters.Add(parPostalCode)

            Dim parJobDescription As New SqlClient.SqlParameter("@JobDescription", SqlDbType.NVarChar, 500)
            parJobDescription.Value = data.job_description
            Command.Parameters.Add(parJobDescription)

            Dim parJobAddress As New SqlClient.SqlParameter("@JobAddress", SqlDbType.NVarChar, 200)
            parJobAddress.Value = data.geof_street
            Command.Parameters.Add(parJobAddress)

            'Dim parJobMap As New SqlClient.SqlParameter("@JobMap", SqlDbType.NVarChar, 100)
            'parJobMap.Value = data.
            'Command.Parameters.Add(parJobMap)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Value = data.geof_latitud
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Value = data.geof_longitud
            Command.Parameters.Add(parLongitude)

            Dim parSendSMS As New SqlClient.SqlParameter("@SendSMS", SqlDbType.Bit)
            parSendSMS.Value = False 'data.send_sms
            Command.Parameters.Add(parSendSMS)

            'Dim parSMS As New SqlClient.SqlParameter("@smsMessage", SqlDbType.NVarChar, 500)
            'parSMS.Value = data.sm
            'Command.Parameters.Add(parSMS)

            Dim parVia As New SqlClient.SqlParameter("@VIA", SqlDbType.Int)
            parVia.Value = 1
            Command.Parameters.Add(parVia)

            Dim parDurationHH As New SqlClient.SqlParameter("@DurationHH", SqlDbType.Int)
            parDurationHH.Value = data.hour
            Command.Parameters.Add(parDurationHH)

            Dim pardurationMM As New SqlClient.SqlParameter("@DurationMM", SqlDbType.Int)
            pardurationMM.Value = data.minute
            Command.Parameters.Add(pardurationMM)

            Dim pardueDate As New SqlClient.SqlParameter("@VisitDate", SqlDbType.DateTime)
            pardueDate.Value = data.dueDate
            Command.Parameters.Add(pardueDate)

            Dim parStartOn As New SqlClient.SqlParameter("@StartOn", SqlDbType.DateTime)
            parStartOn.Value = data.StartOn
            Command.Parameters.Add(parStartOn)

            Dim parjobcategories As New SqlClient.SqlParameter("@CategoryGUID", SqlDbType.NVarChar, 100)
            parjobcategories.Value = data.jobcategories
            Command.Parameters.Add(parjobcategories)

            Dim parjobpriority As New SqlClient.SqlParameter("@PriorityGUID", SqlDbType.NVarChar, 100)
            parjobpriority.Value = data.jobpriority
            Command.Parameters.Add(parjobpriority)

            Dim paruniqueKey As New SqlClient.SqlParameter("@uniqueKey", SqlDbType.NVarChar, 50)
            paruniqueKey.Value = data.UniqueKey
            Command.Parameters.Add(paruniqueKey)

            Dim parRadiusFeet As New SqlClient.SqlParameter("@radiusFeet", SqlDbType.Int)
            parRadiusFeet.Value = data.RadiusFeet
            Command.Parameters.Add(parRadiusFeet)

            Dim parUpdateFrom As New SqlClient.SqlParameter("@UpdateFrom", SqlDbType.Int)
            parUpdateFrom.Value = data.UpdateFrom
            Command.Parameters.Add(parUpdateFrom)

            Dim parStatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.TinyInt)
            parStatusID.Value = data.StatusID
            Command.Parameters.Add(parStatusID)


            Dim parGUID As New SqlClient.SqlParameter("@JobGUID", SqlDbType.NVarChar, 50)
            parGUID.Direction = ParameterDirection.Output
            Command.Parameters.Add(parGUID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            JobGUID = CStr(parGUID.Value)

        Catch ex As Exception
            strError = "Error saveWorkOrderNEW"
            BLErrorHandling.ErrorCapture(pSysModule, "saveWorkOrderNEW", "", ex.Message & " - Token: " & token, 0)
            JobGUID = ""
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return JobGUID

    End Function
    Public Function addJobStop(ByVal token As String, ByVal data As JobStop) As Boolean
        Dim bResult As Boolean = False

        Try
            strCommand = "Jobs_Stops_Insert_Post"
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

            Dim parID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parID.Direction = ParameterDirection.Input
            parID.Value = data.ID
            Command.Parameters.Add(parID)

            Dim parJobID As New SqlClient.SqlParameter("@JobuniqueKey", SqlDbType.VarChar, 100)
            parJobID.Direction = ParameterDirection.Input
            parJobID.Value = data.JobUniqueKey
            Command.Parameters.Add(parJobID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.VarChar, 20)
            parDeviceID.Direction = ParameterDirection.Input
            parDeviceID.Value = data.DeviceID
            Command.Parameters.Add(parDeviceID)

            Dim parDriverId As New SqlClient.SqlParameter("@DriverId", SqlDbType.Int)
            parDriverId.Direction = ParameterDirection.Input
            parDriverId.Value = data.DriverId
            Command.Parameters.Add(parDriverId)

            Dim parName As New SqlClient.SqlParameter("@Name", SqlDbType.NVarChar, 100)
            parName.Direction = ParameterDirection.Input
            parName.Value = data.Name
            Command.Parameters.Add(parName)

            Dim parLatitude As New SqlClient.SqlParameter("@Latitude", SqlDbType.Real)
            parLatitude.Direction = ParameterDirection.Input
            parLatitude.Value = data.Latitude
            Command.Parameters.Add(parLatitude)

            Dim parLongitude As New SqlClient.SqlParameter("@Longitude", SqlDbType.Real)
            parLongitude.Direction = ParameterDirection.Input
            parLongitude.Value = data.Longitude
            Command.Parameters.Add(parLongitude)

            Dim parStreet As New SqlClient.SqlParameter("@Street", SqlDbType.NVarChar, 200)
            parStreet.Direction = ParameterDirection.Input
            parStreet.Value = data.Street
            Command.Parameters.Add(parStreet)

            Dim parFullAddress As New SqlClient.SqlParameter("@FullAddress", SqlDbType.NVarChar, 200)
            parFullAddress.Direction = ParameterDirection.Input
            parFullAddress.Value = data.FullAddress
            Command.Parameters.Add(parFullAddress)

            Dim parCity As New SqlClient.SqlParameter("@City", SqlDbType.NVarChar, 50)
            parCity.Direction = ParameterDirection.Input
            parCity.Value = data.City
            Command.Parameters.Add(parCity)

            Dim parState As New SqlClient.SqlParameter("@State", SqlDbType.NVarChar, 50)
            parState.Direction = ParameterDirection.Input
            parState.Value = data.State
            Command.Parameters.Add(parState)

            Dim parPostalCode As New SqlClient.SqlParameter("@PostalCode", SqlDbType.NVarChar, 50)
            parPostalCode.Direction = ParameterDirection.Input
            parPostalCode.Value = data.PostalCode
            Command.Parameters.Add(parPostalCode)

            Dim parDueDate As New SqlClient.SqlParameter("@DueDate", SqlDbType.DateTime)
            parDueDate.Direction = ParameterDirection.Input
            parDueDate.Value = data.DueDate
            Command.Parameters.Add(parDueDate)

            Dim parCompletedOn As New SqlClient.SqlParameter("@CompletedOn", SqlDbType.DateTime)
            parCompletedOn.Direction = ParameterDirection.Input
            parCompletedOn.Value = data.CompletedOn
            Command.Parameters.Add(parCompletedOn)

            Dim parStartOn As New SqlClient.SqlParameter("@StartOn", SqlDbType.DateTime)
            parStartOn.Direction = ParameterDirection.Input
            parStartOn.Value = data.StartOn
            Command.Parameters.Add(parStartOn)

            Dim parCreatedBy As New SqlClient.SqlParameter("@CreatedBy", SqlDbType.Int)
            parCreatedBy.Direction = ParameterDirection.Input
            parCreatedBy.Value = data.CreatedBy
            Command.Parameters.Add(parCreatedBy)

            Dim parStatus As New SqlClient.SqlParameter("@Status", SqlDbType.Int)
            parStatus.Direction = ParameterDirection.Input
            parStatus.Value = Integer.Parse(1)
            Command.Parameters.Add(parStatus)

            Dim parLastUpdate As New SqlClient.SqlParameter("@LastUpdate", SqlDbType.DateTime)
            parLastUpdate.Direction = ParameterDirection.Input
            parLastUpdate.Value = data.LastUpdate
            Command.Parameters.Add(parLastUpdate)

            Dim paruniqueKey As New SqlClient.SqlParameter("@uniqueKey", SqlDbType.NVarChar, 50)
            paruniqueKey.Direction = ParameterDirection.Input
            paruniqueKey.Value = data.UniqueKey
            Command.Parameters.Add(paruniqueKey)

            Dim pardescription As New SqlClient.SqlParameter("@Description", SqlDbType.NVarChar, 100)
            pardescription.Direction = ParameterDirection.Input
            pardescription.Value = data.Description
            Command.Parameters.Add(pardescription)

            Dim parIsGeofence As New SqlClient.SqlParameter("@IsGeofence", SqlDbType.Bit)
            parIsGeofence.Direction = ParameterDirection.Input
            parIsGeofence.Value = data.IsGeofence
            Command.Parameters.Add(parIsGeofence)

            Dim parGeofenceGUID As New SqlClient.SqlParameter("@GeofenceGUID", SqlDbType.VarChar, 100)
            parGeofenceGUID.Direction = ParameterDirection.Input
            parGeofenceGUID.Value = data.GeofenceGUID
            Command.Parameters.Add(parGeofenceGUID)

            Dim parPhon As New SqlClient.SqlParameter("@Phone", SqlDbType.VarChar, 20)
            parPhon.Direction = ParameterDirection.Input
            parPhon.Value = data.Phone
            Command.Parameters.Add(parPhon)

            Dim parUpdateFrom As New SqlClient.SqlParameter("@UpdateFrom", SqlDbType.Int)
            parUpdateFrom.Direction = ParameterDirection.Input
            parUpdateFrom.Value = data.UpdateFrom
            Command.Parameters.Add(parUpdateFrom)

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
    Function Jobs_Stops_ListByCompanies_GET(ByVal token As String) As String
        Dim itm As JobStop
        Dim result As String = ""
        Dim response As response = New response
        Try
            strCommand = "Jobs_Stops_ListByCompanies"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            Dim reader As SqlDataReader = Command.ExecuteReader
            response.Status = "OK"
            response.Messagge = "Requests OK."
            Do While reader.Read
                itm = New JobStop With {
                    .Name = reader.Item("Name"),
                    .Street = reader.Item("Street"),
                    .FullAddress = reader.Item("FullAddress"),
                    .City = reader.Item("City"),
                    .State = reader.Item("State"),
                    .PostalCode = reader.Item("PostalCode"),
                    .Phone = reader.Item("Phone"),
                    .GeofenceGUID = reader.Item("geofencesGUID"),
                    .Latitude = reader.Item("Latitude"),
                    .Longitude = reader.Item("Longitude")
                }
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
#End Region

End Class
