' NOTE: You can use the "Rename" command on the context menu to change the class name "cases" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select cases.svc or cases.svc.vb at the Solution Explorer and start debugging.

Imports System
Imports System.Collections.Generic
Imports System.ServiceModel.Web
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter

Public Class Brokers
    Implements IBrokers
    Public Function CreateBroker(ByVal token As String, ByVal data As BrokerOrder) As responseOk Implements IBrokers.CreateBroker
        Dim resp As New responseOk
        Dim dl As New DataLayerBrokerOrder
        Dim geofencesGuid As String = ""
        Try

            resp = dl.CreateBroker(token, data)
            If resp.isOk Then
                If data.HasEmail Then
                    PostTraking(token, resp.transId, data.BrokerNumber, data.Pickupdetetime, data.Deliverydatetime, data.EmailTo)
                    PostSentEmail(token, resp.transId, data.EmailTo, False, data.Observaciones)
                End If
                If data.HasSms Then
                    If data.HasEmail = False Then
                        PostTraking(token, resp.transId, data.BrokerNumber, data.Pickupdetetime, data.Deliverydatetime, data.EmailTo)
                    End If
                    PostBrokerSMS(token, resp.transId, data.Observaciones, data.SmsPhone)
                End If
                geofencesGuid = dl.saveGeofence(token, 0, 12, data.BrokerNumber, "-1", "-1", "-1", False, False, 0, data.DeliveryAddress, "-1", "-1", "-1", "-1", "-1", "-1", "-1", "-1", "-1", data.DeliveryAddressscoordinatesLat, data.DeliveryAddressscoordinatesLng, 1, 200, "-1", 1, "-1", "-1", "-1", 0, 0, 418, "-1", 419, "-1", False, resp.msg)
                dl.UpdateGeofencesBroker(token, geofencesGuid, resp.transId)

            End If
        Catch ex As Exception
            resp.isOk = False
        End Try
        Return resp

    End Function
    Public Function GetBrokersDevices(ByVal token As String, ByVal lastFetchOn As String) As List(Of BrokerDevices) Implements IBrokers.GetBrokersDevices
        Dim devicesList As New List(Of BrokerDevices)
        Dim devices As BrokerDevices = Nothing
        Dim dl As New DataLayerBrokerOrder
        Try
            devicesList = dl.GetBrokersDevices(token, lastFetchOn)
        Catch ex As Exception
        End Try
        Return devicesList
    End Function
    Public Function GetBrokersOrder(ByVal token As String, ByVal BrokerID As Integer, ByVal DeviceID As Integer) As List(Of DTOBrokerOrder) Implements IBrokers.GetBrokersOrder
        Dim listbrokers As New List(Of DTOBrokerOrder)
        Dim dl As New DataLayerBrokerOrder
        Try
            listbrokers = dl.GetBrokersOrder(token, BrokerID, DeviceID)
        Catch ex As Exception
        End Try
        Return listbrokers
    End Function
    Public Function GetBrokersOrderStops(ByVal token As String, ByVal BrokerID As Integer) As List(Of DTOBrokerOrderStop) Implements IBrokers.GetBrokersOrderStops
        Dim listbrokersStops As New List(Of DTOBrokerOrderStop)
        Dim dl As New DataLayerBrokerOrder
        Try
            listbrokersStops = dl.GetBrokersOrderStops(token, BrokerID)
        Catch ex As Exception
        End Try
        Return listbrokersStops
    End Function
    Public Function PostCancellBrokersOrder(ByVal token As String, ByVal BrokerID As Integer, ByVal DeviceID As Integer) As responseOk Implements IBrokers.PostCancellBrokersOrder
        Dim response As New responseOk
        Dim dl As New DataLayerBrokerOrder
        Try
            response = dl.PostCancellBrokersOrder(token, BrokerID, DeviceID)
        Catch ex As Exception
        End Try
        Return response
    End Function
    Public Function PostSentEmail(ByVal token As String, ByVal brokerID As Integer, ByVal emails As String, ByVal resend As Boolean, ByVal observations As String) As responseOk Implements IBrokers.PostSentEmail
        Dim response As New responseOk
        Dim dl As New DataLayerBrokerOrder
        Dim output() As String
        Dim hasTracking As BrokerHasTracking = Nothing

        Try
            hasTracking = dl.ValidateTrackingWasSend(token, brokerID)
            If hasTracking.ExistTraking = False Then
                PostTraking(token, brokerID, hasTracking.BrokerNumber, hasTracking.Pickupdetetime, hasTracking.Deliverydatetime, "")
            End If
            output = Split(emails, ";")
            For Each email In output
                response = dl.PostSentEmail(token, brokerID, email, resend, observations)
            Next
            dl.UpdateEmailsInBroker(token, brokerID, emails)
        Catch ex As Exception
        End Try
        Return response
    End Function
    Public Function TrakingNumberExt(ByVal trakingnumber As String) As DTOBrokerTraking Implements IBrokers.TrakingNumberExt
        Dim dl As New DataLayerBrokerOrder
        Dim resonse As New DTOBrokerTraking
        resonse = dl.GetBrokerTrakingExt(trakingnumber)
        Return resonse

    End Function
    Public Function PostTraking(ByVal token As String, ByVal brokerID As Integer, ByVal number As String, ByVal datefrom As DateTime, ByVal dateto As DateTime, ByVal emails As String) As responseOk Implements IBrokers.PostTraking
        Dim response As New responseOk
        Dim dl As New DataLayerBrokerOrder
        Try
            response = dl.PostTraking(token, brokerID, number, datefrom, dateto, emails)
        Catch ex As Exception
        End Try
        Return response
    End Function

    Public Function CreateBrokerStop(ByVal token As String, ByVal data As DTOBrokerOrderStop) As responseOk Implements IBrokers.CreateBrokerStop
        Dim resp As New responseOk
        Dim dl As New DataLayerBrokerOrder
        data.Pickupdetetime = data.Pickupdetetime.Replace("T", " ")
        resp = dl.CreateBrokerStop(token, data)
        Return resp
    End Function

    Public Function PostDeleteStop(ByVal token As String, ByVal StopID As Integer) As responseOk Implements IBrokers.PostDeleteStop
        Dim response As New responseOk
        Dim dl As New DataLayerBrokerOrder
        Try
            response = dl.PostDeleteStop(token, StopID)
        Catch ex As Exception
        End Try
        Return response
    End Function
    Public Function PostBrokerSMS(ByVal token As String, PBrokerID As Integer, Pobservations As String, PPhoneNumber As String) As responseOk Implements IBrokers.PostBrokerSMS
        Dim response As New responseOk
        Dim dl As New DataLayerBrokerOrder
        Dim hasTracking As BrokerHasTracking = Nothing
        Dim output1() As String
        Try
            hasTracking = dl.ValidateTrackingWasSend(token, PBrokerID)
            If hasTracking.ExistTraking = False Then
                PostTraking(token, PBrokerID, hasTracking.BrokerNumber, hasTracking.Pickupdetetime, hasTracking.Deliverydatetime, "")
            End If
            output1 = Split(PPhoneNumber, ";")
            For Each phone In output1
                response = dl.PostBrokerSMS(token, PBrokerID, Pobservations, phone)
            Next
        Catch ex As Exception
        End Try
        Return response
    End Function

End Class
