Imports System.Data.SqlClient

Public Class DataLayerBrokerOrder
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
#Region "Broker Order"

    Public Function GetBrokersOrder(ByVal token As String, ByVal BrokerID As Integer, ByVal DeviceID As Integer) As List(Of DTOBrokerOrder)

        Dim broker As New DTOBrokerOrder
        Dim listbroker As New List(Of DTOBrokerOrder)

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

            Dim parBrokerID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parBrokerID.Value = BrokerID
            Command.Parameters.Add(parBrokerID)

            Dim parDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.Int)
            parDeviceID.Value = DeviceID
            Command.Parameters.Add(parDeviceID)

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

            Do While reader.Read
                broker = New DTOBrokerOrder With {
                    .ID = reader.Item("ID"),
                    .CompanyID = reader.Item("CompanyID"),
                    .DeviceID = reader.Item("DeviceID"),
                    .DriverID = reader.Item("DriverID"),
                    .TrakingID = reader.Item("TrakingID"),
                    .Name = reader.Item("Name"),
                    .BrokerNumber = reader.Item("BrokerNumber"),
                    .PickupAddress = reader.Item("PickupAddress"),
                    .Pickupdetetime = reader.Item("Pickupdetetime"),
                    .PickupAddresscoordinatesLat = reader.Item("PickupAddresscoordinatesLat"),
                    .PickupAddresscoordinatesLng = reader.Item("PickupAddresscoordinatesLng"),
                    .DeliveryAddress = reader.Item("DeliveryAddress"),
                    .Deliverydatetime = reader.Item("Deliverydatetime"),
                    .DeliveryAddressscoordinatesLat = reader.Item("DeliveryAddressscoordinatesLat"),
                    .DeliveryAddressscoordinatesLng = reader.Item("DeliveryAddressscoordinatesLng"),
                    .Observaciones = reader.Item("Observaciones"),
                    .StatusID = reader.Item("StatusID"),
                    .CreateOn = reader.Item("CreateOn"),
                    .CreatedBy = reader.Item("CreatedBy"),
                    .EmailSent = reader.Item("EmailSent"),
                    .EmailLogo = reader.Item("EmailLogo"),
                    .TrackingWasSent = reader.Item("TrackingWasSent"),
                    .TrackingWasSentDate = reader.Item("TrackingWasSentDate"),
                    .SendTo = reader.Item("SendTo"),
                    .DriverName = reader.Item("DriverName"),
                    .TrackingNumber = reader.Item("TrackingNumber"),
                    .TrackingStatus = reader.Item("TrackingStatus"),
                    .DeviceName = reader.Item("DeviceName"),
                    .CountStops = reader.Item("CountStops"),
                    .UrlTraking = reader.Item("URLTraking")
                }
                listbroker.Add(broker)
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

        Return listbroker

    End Function
    Public Function GetBrokersDevices(ByVal token As String, ByVal lastUpdate As String) As List(Of BrokerDevices)

        Dim brokerDevice As New BrokerDevices
        Dim listbroker As New List(Of BrokerDevices)

        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "Devices_GetByTokenBrokerOrder_NEWV2"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim parBrokerID As New SqlClient.SqlParameter("@LastFetchOn", SqlDbType.VarChar, 100)
            parBrokerID.Value = lastUpdate
            Command.Parameters.Add(parBrokerID)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                brokerDevice = New BrokerDevices With {
                    .ID = reader.Item("ID"),
                    .DeviceID = reader.Item("DeviceID"),
                    .ShortName = reader.Item("ShortName"),
                    .Name = reader.Item("Name"),
                    .LastUpdatedOn = reader.Item("LastUpdatedOn"),
                    .EventCode = reader.Item("EventCode"),
                    .EventName = reader.Item("EventName"),
                    .EventDate = reader.Item("EventDate"),
                    .Speed = reader.Item("Speed"),
                    .GPSStatus = reader.Item("GPSStatus"),
                    .GPSAge = reader.Item("GPSAge"),
                    .DriverID = reader.Item("DriverID"),
                    .DriverName = reader.Item("DriverName"),
                    .FullAddress = reader.Item("FullAddress"),
                    .DriverPhone = reader.Item("DriverPhone"),
                    .IconID = reader.Item("IconID"),
                    .IconURL = reader.Item("IconURL"),
                    .TextColor = reader.Item("TextColor"),
                    .BgndColor = reader.Item("BgndColor"),
                    .CountBrokers = reader.Item("CountBrokers")
                }
                listbroker.Add(brokerDevice)
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

        Return listbroker

    End Function
    Public Function CreateBroker(ByVal token As String, data As BrokerOrder) As responseOk
        Dim response As New responseOk
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

            Dim ID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Output
            ID.Value = data.ID
            Command.Parameters.Add(ID)

            Dim DeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.Int)
            DeviceID.Direction = ParameterDirection.Input
            DeviceID.Value = data.DeviceID
            Command.Parameters.Add(DeviceID)

            Dim DeviceName As New SqlClient.SqlParameter("@DeviceName", SqlDbType.VarChar, 100)
            DeviceName.Direction = ParameterDirection.Output
            Command.Parameters.Add(DeviceName)

            Dim DriverID As New SqlClient.SqlParameter("@DriverID", SqlDbType.Int)
            DriverID.Direction = ParameterDirection.Input
            DriverID.Value = data.DriverID
            Command.Parameters.Add(DriverID)

            Dim TrakingID As New SqlClient.SqlParameter("@TrakingID", SqlDbType.Int)
            TrakingID.Direction = ParameterDirection.Input
            TrakingID.Value = data.TrakingID
            Command.Parameters.Add(TrakingID)

            Dim Name As New SqlClient.SqlParameter("@Name", SqlDbType.VarChar, 50)
            Name.Direction = ParameterDirection.Input
            Name.Value = data.Name
            Command.Parameters.Add(Name)

            Dim BrokerNumber As New SqlClient.SqlParameter("@BrokerNumber", SqlDbType.NVarChar, 50)
            BrokerNumber.Direction = ParameterDirection.Input
            BrokerNumber.Value = data.BrokerNumber
            Command.Parameters.Add(BrokerNumber)

            Dim PickupAddress As New SqlClient.SqlParameter("@PickupAddress", SqlDbType.NVarChar, 255)
            PickupAddress.Direction = ParameterDirection.Input
            PickupAddress.Value = data.PickupAddress
            Command.Parameters.Add(PickupAddress)

            Dim Pickupdetetime As New SqlClient.SqlParameter("@Pickupdetetime", SqlDbType.DateTime)
            Pickupdetetime.Direction = ParameterDirection.Input
            Pickupdetetime.Value = data.Pickupdetetime
            Command.Parameters.Add(Pickupdetetime)

            Dim PickupAddresscoordinatesLat As New SqlClient.SqlParameter("@PickupAddresscoordinatesLat", SqlDbType.Decimal)
            PickupAddresscoordinatesLat.Direction = ParameterDirection.Input
            PickupAddresscoordinatesLat.Value = data.PickupAddresscoordinatesLat
            Command.Parameters.Add(PickupAddresscoordinatesLat)

            Dim PickupAddresscoordinatesLng As New SqlClient.SqlParameter("@PickupAddresscoordinatesLng", SqlDbType.Decimal)
            PickupAddresscoordinatesLng.Direction = ParameterDirection.Input
            PickupAddresscoordinatesLng.Value = data.PickupAddresscoordinatesLng
            Command.Parameters.Add(PickupAddresscoordinatesLng)

            Dim DeliveryAddress As New SqlClient.SqlParameter("@DeliveryAddress", SqlDbType.VarChar, 500)
            DeliveryAddress.Direction = ParameterDirection.Input
            DeliveryAddress.Value = data.DeliveryAddress
            Command.Parameters.Add(DeliveryAddress)

            Dim Deliverydatetime As New SqlClient.SqlParameter("@Deliverydatetime", SqlDbType.DateTime)
            Deliverydatetime.Direction = ParameterDirection.Input
            Deliverydatetime.Value = data.Deliverydatetime
            Command.Parameters.Add(Deliverydatetime)

            Dim DeliveryAddressscoordinatesLat As New SqlClient.SqlParameter("@DeliveryAddressscoordinatesLat", SqlDbType.Decimal)
            DeliveryAddressscoordinatesLat.Direction = ParameterDirection.Input
            DeliveryAddressscoordinatesLat.Value = data.DeliveryAddressscoordinatesLat
            Command.Parameters.Add(DeliveryAddressscoordinatesLat)

            Dim DeliveryAddressscoordinatesLng As New SqlClient.SqlParameter("@DeliveryAddressscoordinatesLng", SqlDbType.Decimal)
            DeliveryAddressscoordinatesLng.Direction = ParameterDirection.Input
            DeliveryAddressscoordinatesLng.Value = data.DeliveryAddressscoordinatesLng
            Command.Parameters.Add(DeliveryAddressscoordinatesLng)

            Dim Observaciones As New SqlClient.SqlParameter("@Observaciones", SqlDbType.NVarChar, 500)
            Observaciones.Direction = ParameterDirection.Input
            Observaciones.Value = data.Observaciones
            Command.Parameters.Add(Observaciones)

            Dim StatusID As New SqlClient.SqlParameter("@StatusID", SqlDbType.TinyInt)
            StatusID.Direction = ParameterDirection.Input
            StatusID.Value = data.StatusID
            Command.Parameters.Add(StatusID)

            Dim CreateOn As New SqlClient.SqlParameter("@CreateOn", SqlDbType.DateTime)
            CreateOn.Direction = ParameterDirection.Input
            CreateOn.Value = data.CreateOn
            Command.Parameters.Add(CreateOn)

            Dim CreatedBy As New SqlClient.SqlParameter("@CreatedBy", SqlDbType.Int)
            CreatedBy.Direction = ParameterDirection.Input
            CreatedBy.Value = data.CreatedBy
            Command.Parameters.Add(CreatedBy)

            Dim EmailSent As New SqlClient.SqlParameter("@EmailSent", SqlDbType.NVarChar)
            EmailSent.Value = data.EmailSent
            Command.Parameters.Add(EmailSent)

            Dim EmailLogo As New SqlClient.SqlParameter("@EmailLogo", SqlDbType.VarChar, 100)
            EmailLogo.Value = data.EmailLogo
            Command.Parameters.Add(EmailLogo)

            Dim TrackingWasSent As New SqlClient.SqlParameter("@TrackingWasSent", SqlDbType.Bit)
            TrackingWasSent.Value = 0
            Command.Parameters.Add(TrackingWasSent)

            Dim TrackingWasSentDate As New SqlClient.SqlParameter("@TrackingWasSentDate", SqlDbType.DateTime)
            TrackingWasSentDate.Value = "1991-01-01 00:00:00.000"
            Command.Parameters.Add(TrackingWasSentDate)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.Int)
            Action.Value = 1
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            response.isOk = Convert.ToBoolean(IsOk.Value)
            response.transId = CStr(ID.Value)
            response.docNum = CStr(DeviceName.Value)

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
    Public Function PostCancellBrokersOrder(ByVal token As String, ByVal BrokerID As Integer, ByVal DeviceID As Integer) As responseOk
        Dim response As New responseOk
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

            Dim PID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            PID.Direction = ParameterDirection.Output
            PID.Value = BrokerID
            Command.Parameters.Add(PID)

            Dim pDeviceID As New SqlClient.SqlParameter("@DeviceID", SqlDbType.Int)
            pDeviceID.Direction = ParameterDirection.Input
            pDeviceID.Value = DeviceID
            Command.Parameters.Add(pDeviceID)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.Int)
            Action.Value = 4
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            Dim PBROKERID As New SqlClient.SqlParameter("@PBROKERID", SqlDbType.Int)
            PBROKERID.Direction = ParameterDirection.Input
            PBROKERID.Value = BrokerID
            Command.Parameters.Add(PBROKERID)

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

    Public Function GetBrokersOrderStops(ByVal token As String, ByVal BrokerID As Integer) As List(Of DTOBrokerOrderStop)

        Dim brokerstop As New DTOBrokerOrderStop
        Dim listbrokerstops As New List(Of DTOBrokerOrderStop)

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

            Dim parBrokerID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            parBrokerID.Value = BrokerID
            Command.Parameters.Add(parBrokerID)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Value = 5
            Command.Parameters.Add(Action)

            Dim parIsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Bit)
            parIsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(parIsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                brokerstop = New DTOBrokerOrderStop With {
                    .ID = reader.Item("ID"),
                    .BrokerOrderID = reader.Item("BrokerOrderID"),
                    .DeviceID = reader.Item("DeviceID"),
                    .PickupAddress = reader.Item("PickupAddress"),
                    .Pickupdetetime = reader.Item("Pickupdetetime"),
                    .PickupAddresscoordinatesLat = reader.Item("PickupAddresscoordinatesLat"),
                    .PickupAddresscoordinatesLng = reader.Item("PickupAddresscoordinatesLng"),
                    .Observations = reader.Item("Observations"),
                    .CreatedOn = reader.Item("CreateOn"),
                    .StatusID = reader.Item("StatusID")
                }
                listbrokerstops.Add(brokerstop)
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

        Return listbrokerstops

    End Function
    Public Function PostSentEmail(ByVal token As String, ByVal brokerID As Integer, ByVal emails As String, ByVal resend As Boolean, ByVal observations As String) As responseOk
        Dim response As New responseOk
        Try
            strCommand = "BrokerOrderBuilEmail_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim ID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = brokerID
            Command.Parameters.Add(ID)

            Dim pemails As New SqlClient.SqlParameter("@EmailTo", SqlDbType.VarChar, 100)
            pemails.Direction = ParameterDirection.Input
            pemails.Value = emails
            Command.Parameters.Add(pemails)

            Dim pobservations As New SqlClient.SqlParameter("@Observarions", SqlDbType.VarChar, 500)
            pobservations.Direction = ParameterDirection.Input
            pobservations.Value = observations
            Command.Parameters.Add(pobservations)

            Dim presend As New SqlClient.SqlParameter("@ResendEmail", SqlDbType.Bit)
            presend.Direction = ParameterDirection.Input
            presend.Value = resend
            Command.Parameters.Add(presend)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrderBuilEmail_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrderBuilEmail_SP", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function
    Public Function GetBrokerTrakingExt(ByVal ptrakingnumber As String) As DTOBrokerTraking
        Dim res As New DTOBrokerTraking
        Dim objStop As DTOBrokerOrderStop
        Dim reader As SqlDataReader = Nothing

        Try
            strCommand = "BrokerOrder_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim TrackingNumber As New SqlClient.SqlParameter("@GuidTraking", SqlDbType.NVarChar, 100) With {
                .Value = ptrakingnumber
            }
            Command.Parameters.Add(TrackingNumber)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt) With {
                .Value = 6
            }
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            reader = Command.ExecuteReader
            If (reader.HasRows) Then
                Do While reader.Read
                    'res.ID = reader.Item("ID")
                    res.BrokerNumber = reader.Item("BrokerNumber")
                    res.TrakingNumber = reader.Item("TrakingNumber")
                    res.TrakingURL = reader.Item("URLTraking")
                    res.GUID = reader.Item("GUID")
                    res.PickupAddress = reader.Item("PickupAddress")
                    res.PickupDatetime = reader.Item("Pickupdetetime")
                    res.PickupLatitude = reader.Item("PickupAddresscoordinatesLat")
                    res.PickupLongitude = reader.Item("PickupAddresscoordinatesLng")
                    res.DeliveryAddress = reader.Item("DeliveryAddress")
                    res.DeliveryDatetime = reader.Item("Deliverydatetime")
                    res.DeliveryLatitud = reader.Item("DeliveryAddressscoordinatesLat")
                    res.DeliveryLongitud = reader.Item("DeliveryAddressscoordinatesLng")
                    res.FlatExpired = reader.Item("FlatExpired")
                    res.Latitude = reader.Item("Latitude")
                    res.Longitude = reader.Item("Longitude")
                    res.Name = reader.Item("Name")
                    res.IconDevice = reader.Item("IconDevice")
                    res.Observations = reader.Item("Observaciones")
                Loop
                reader.NextResult()
                Do While reader.Read
                    objStop = New DTOBrokerOrderStop
                    objStop.PickupAddress = reader.Item("PickupAddress")
                    objStop.Pickupdetetime = reader.Item("Pickupdetetime")
                    objStop.PickupAddresscoordinatesLat = reader.Item("PickupAddresscoordinatesLat")
                    objStop.PickupAddresscoordinatesLng = reader.Item("PickupAddresscoordinatesLng")
                    res.Stops.Add(objStop)
                Loop
                If Not reader.IsClosed Then
                    reader.Close()
                End If
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

    Public Function PostTraking(ByVal token As String, ByVal pbrokerID As Integer, ByVal number As String, ByVal datefrom As String, ByVal dateto As String, ByVal emails As String) As responseOk
        Dim response As New responseOk
        Try
            strCommand = "BrokerOrderTracking_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim BrokerID As New SqlClient.SqlParameter("@BrokerID", SqlDbType.Int)
            BrokerID.Direction = ParameterDirection.Input
            BrokerID.Value = pbrokerID
            Command.Parameters.Add(BrokerID)

            Dim BrokerNumber As New SqlClient.SqlParameter("@BrokerNumber", SqlDbType.VarChar, 100)
            BrokerNumber.Direction = ParameterDirection.Input
            BrokerNumber.Value = number
            Command.Parameters.Add(BrokerNumber)

            Dim DatetimeFrom As New SqlClient.SqlParameter("@DatetimeFrom", SqlDbType.DateTime)
            DatetimeFrom.Direction = ParameterDirection.Input
            DatetimeFrom.Value = CDate(datefrom)
            Command.Parameters.Add(DatetimeFrom)

            Dim DatetimeTo As New SqlClient.SqlParameter("@DatetimeTo", SqlDbType.DateTime)
            DatetimeTo.Direction = ParameterDirection.Input
            DatetimeTo.Value = CDate(dateto)
            Command.Parameters.Add(DatetimeTo)

            Dim pemails As New SqlClient.SqlParameter("@Emails", SqlDbType.VarChar, 255)
            pemails.Direction = ParameterDirection.Input
            pemails.Value = emails
            Command.Parameters.Add(pemails)

            'Dim URL As New SqlClient.SqlParameter("@URL", SqlDbType.VarChar)
            'URL.Direction = ParameterDirection.Output
            'Command.Parameters.Add(URL)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrderTracking_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrderTracking_SP", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function

    Public Function CreateBrokerStop(ByVal token As String, ByVal pstop As DTOBrokerOrderStop) As responseOk
        Dim response As New responseOk
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

            Dim BrokerID As New SqlClient.SqlParameter("@PBROKERID", SqlDbType.Int)
            BrokerID.Direction = ParameterDirection.Input
            BrokerID.Value = pstop.BrokerOrderID
            Command.Parameters.Add(BrokerID)

            Dim PickupAddress As New SqlClient.SqlParameter("@PickupAddress", SqlDbType.VarChar, 100)
            PickupAddress.Direction = ParameterDirection.Input
            PickupAddress.Value = pstop.PickupAddress
            Command.Parameters.Add(PickupAddress)

            Dim Pickupdetetime As New SqlClient.SqlParameter("@Pickupdetetime", SqlDbType.DateTime)
            Pickupdetetime.Direction = ParameterDirection.Input
            Pickupdetetime.Value = pstop.Pickupdetetime
            Command.Parameters.Add(Pickupdetetime)

            Dim PickupAddresscoordinatesLat As New SqlClient.SqlParameter("@PickupAddresscoordinatesLat", SqlDbType.Decimal)
            PickupAddresscoordinatesLat.Direction = ParameterDirection.Input
            PickupAddresscoordinatesLat.Value = pstop.PickupAddresscoordinatesLat
            Command.Parameters.Add(PickupAddresscoordinatesLat)

            Dim PickupAddresscoordinatesLng As New SqlClient.SqlParameter("@PickupAddresscoordinatesLng", SqlDbType.Decimal)
            PickupAddresscoordinatesLng.Direction = ParameterDirection.Input
            PickupAddresscoordinatesLng.Value = pstop.PickupAddresscoordinatesLng
            Command.Parameters.Add(PickupAddresscoordinatesLng)

            Dim Observaciones As New SqlClient.SqlParameter("@Observaciones", SqlDbType.VarChar)
            Observaciones.Direction = ParameterDirection.Input
            Observaciones.Value = pstop.Observations
            Command.Parameters.Add(Observaciones)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.TinyInt)
            Action.Value = 7
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()
            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrder_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrder_SP_Action7", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function
    Public Function UpdateGeofencesBroker(ByVal token As String, geofencesGuid As String, brokerid As Integer) As responseOk
        Dim response As New responseOk
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

            Dim ID As New SqlClient.SqlParameter("@PBROKERID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = brokerid
            Command.Parameters.Add(ID)

            Dim pgeofencesGuid As New SqlClient.SqlParameter("@geofencesGUID", SqlDbType.VarChar, 255)
            pgeofencesGuid.Direction = ParameterDirection.Input
            pgeofencesGuid.Value = geofencesGuid
            Command.Parameters.Add(pgeofencesGuid)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.Int)
            Action.Value = 8
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrder_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrder_SP-ACTION-8", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function

    Public Function PostDeleteStop(ByVal token As String, stopid As Integer) As responseOk
        Dim response As New responseOk
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

            Dim ID As New SqlClient.SqlParameter("@ID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = stopid
            Command.Parameters.Add(ID)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.Int)
            Action.Value = 10
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrder_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrder_SP-ACTION-10", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function

    Public Function PostBrokerSMS(ByVal token As String, PBrokerID As Integer, Pobservations As String, PPhoneNumber As String) As responseOk
        Dim response As New responseOk
        Try
            strCommand = "BrokerOrderSendSMS_SP"
            conString = ConfigurationManager.AppSettings("ConnectionString")
            conSQL = New SqlConnection(conString)
            Command = New SqlCommand
            Command.Connection = conSQL
            Command.CommandText = strCommand
            Command.CommandType = CommandType.StoredProcedure

            Dim parToken As New SqlClient.SqlParameter("@Token", SqlDbType.NVarChar, 50)
            parToken.Value = token
            Command.Parameters.Add(parToken)

            Dim BrokerID As New SqlClient.SqlParameter("@BrokerID", SqlDbType.Int)
            BrokerID.Direction = ParameterDirection.Input
            BrokerID.Value = PBrokerID
            Command.Parameters.Add(BrokerID)

            Dim observations As New SqlClient.SqlParameter("@observations", SqlDbType.NVarChar, 100)
            observations.Direction = ParameterDirection.Input
            observations.Value = Pobservations
            Command.Parameters.Add(observations)

            Dim PhoneNumber As New SqlClient.SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 15)
            PhoneNumber.Direction = ParameterDirection.Input
            PhoneNumber.Value = PPhoneNumber
            Command.Parameters.Add(PhoneNumber)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrderSendSMS_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrderSendSMS_SP", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response

    End Function

    Public Function UpdateEmailsInBroker(ByVal token As String, ByVal brokerID As Integer, ByVal emails As String)
        Dim response As New responseOk
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

            Dim ID As New SqlClient.SqlParameter("@PBROKERID", SqlDbType.Int)
            ID.Direction = ParameterDirection.Input
            ID.Value = brokerID
            Command.Parameters.Add(ID)

            Dim pemails As New SqlClient.SqlParameter("@EmailSent", SqlDbType.VarChar, 500)
            pemails.Direction = ParameterDirection.Input
            pemails.Value = emails
            Command.Parameters.Add(pemails)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.Int)
            Action.Value = 11
            Command.Parameters.Add(Action)

            Dim IsOk As New SqlClient.SqlParameter("@IsOk", SqlDbType.Int)
            IsOk.Direction = ParameterDirection.Output
            Command.Parameters.Add(IsOk)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If
            Command.ExecuteNonQuery()

            response.isOk = Convert.ToBoolean(IsOk.Value)

        Catch ex As Exception
            strError = "Error BrokerOrder_SP"
            BLErrorHandling.ErrorCapture(pSysModule, "BrokerOrder_SP-ACTION-11", "", ex.Message & " - Token: " & token, 0)
            response.isOk = False
        Finally
            If conSQL.State = ConnectionState.Open Then
                conSQL.Close()
            End If
            conSQL.Dispose()
        End Try

        Return response
    End Function
    Public Function ValidateTrackingWasSend(ByVal token As String, ByVal brokerid As Integer) As BrokerHasTracking

        Dim reader As SqlDataReader = Nothing
        Dim result As New BrokerHasTracking
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

            Dim parBrokerID As New SqlClient.SqlParameter("@PBROKERID", SqlDbType.Int)
            parBrokerID.Value = brokerid
            Command.Parameters.Add(parBrokerID)

            Dim Action As New SqlClient.SqlParameter("@Action", SqlDbType.Int)
            Action.Value = 12
            Command.Parameters.Add(Action)

            If conSQL.State = ConnectionState.Closed Then
                conSQL.Open()
            End If

            reader = Command.ExecuteReader

            Do While reader.Read
                result.BrokerNumber = reader.Item("BrokerNumber")
                result.Pickupdetetime = reader.Item("Pickupdetetime")
                result.Deliverydatetime = reader.Item("Deliverydatetime")
                result.TrackingWasSent = Convert.ToBoolean(reader.Item("TrackingWasSent"))
                result.ExistTraking = Convert.ToBoolean(reader.Item("ExistTraking"))
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

        Return result

    End Function


#End Region
#Region "Geofence"
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

#End Region

End Class
