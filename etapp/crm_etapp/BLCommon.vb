Imports System.Data
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.IO
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports Newtonsoft.Json.JsonConverter
Imports System.Net.WebRequest
Imports System.Net.HttpWebRequest
Imports System.Net

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Text.StringBuilder

Public Class BLCommon

    Public Shared pSysModule As String = "ETWS or ETCRMWS"

    Public Function table2CSV(ByVal dtData As DataTable) As String
        Dim _sep As String = ""
        Dim sep_char As String = ","
        Dim builder As New System.Text.StringBuilder
        Dim strResult As String = ""
        Dim strCol As String = ""

        Try
            For Each col As DataColumn In dtData.Columns
                builder.Append(_sep).Append(col.ColumnName)
                _sep = sep_char
            Next

            'writer.WriteLine(builder.ToString())
            strResult = builder.ToString()

            For Each row As DataRow In dtData.Rows
                _sep = ""
                builder = New System.Text.StringBuilder
                For Each col As DataColumn In dtData.Columns
                    If col.DataType.Name = "String" Then
                        strCol = row(col.ColumnName)
                        strCol = strCol.Replace("""", "'")
                        builder.Append(_sep).Append("""" & strCol & """")
                    Else
                        builder.Append(_sep).Append(row(col.ColumnName))
                    End If
                    _sep = sep_char
                Next
                'writer.WriteLine(builder.ToString())
                strResult = strResult & vbCrLf & builder.ToString()

            Next

        Catch ex As Exception

        End Try

        Return strResult

    End Function

    Public Shared Function Sanitize(ByVal strOriginal As String) As String
        Dim sbSanitized As New StringBuilder(Trim(strOriginal))

        Try
            'Someone who knows all emails contain this, could attempt to bring every record by using an adHoc query of @, as Email like '%@%' would bring them all.  Same with other characters.
            If sbSanitized.ToString = "@" Then
                sbSanitized.Replace("@", "_@_")
            End If
            If sbSanitized.ToString = "." Then
                sbSanitized.Replace(".", "_._")
            End If
            If sbSanitized.ToString = "." Then
                sbSanitized.Replace(".", "_._")
            End If
            sbSanitized.Replace("'", "''")
            sbSanitized.Replace("""", "''")
            sbSanitized.Replace("--", "- -")

            'strSanitized = strSanitized.Replace("'", "''")
            'strSanitized = strSanitized.Replace("""", " ")
            'strSanitized = strSanitized.Replace("--", " ")
            'strSanitized = strSanitized.Replace(";", ",")
            'strSanitized = strSanitized.Replace("%", "")
            'strSanitized = strSanitized.Replace("/*", "@ @ @")
            'strSanitized = strSanitized.Replace("*/", "@ @ @")
            'strSanitized = strSanitized.Replace("[", "[[]")
            'strSanitized = strSanitized.Replace("_", "[_]")
            'strSanitized = strSanitized.Replace("#", "[#]")
            'strSanitized = strSanitized.Replace("/", " ")
            'strSanitized = strSanitized.Replace("\", " ")

            If UCase(sbSanitized.ToString).IndexOf("XP_") >= 0 Then
                sbSanitized.Replace("XP_", "XP _")
            End If
            If UCase(sbSanitized.ToString).IndexOf("SP_") >= 0 Then
                sbSanitized.Replace("SP_", "SP _")
            End If
            If UCase(sbSanitized.ToString).IndexOf("EXEC ") >= 0 Then
                sbSanitized.Replace("EXEC ", "EXEC_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("DROP ") >= 0 Then
                sbSanitized.Replace("DROP ", "DROP_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("DELETE ") >= 0 Then
                sbSanitized.Replace("DELETE ", "DELETE_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("ALTER ") >= 0 Then
                sbSanitized.Replace("ALTER ", "ALTER_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("UPDATE ") >= 0 Then
                sbSanitized.Replace("UPDATE ", "UPDATE_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("INSERT ") >= 0 Then
                sbSanitized.Replace("INSERT ", "INSERT_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("SELECT ") >= 0 Then
                sbSanitized.Replace("SELECT ", "SELECT_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("INFORMATION_SCHEMA") >= 0 Then
                sbSanitized.Replace("INFORMATION_SCHEMA ", "INFORMATION SCHEMA_ ")
            End If
            If UCase(sbSanitized.ToString).IndexOf("SHUTDOWN ") >= 0 Then
                sbSanitized.Replace("SHUTDOWN ", "SHUTDOWN_ ")
            End If
            '
        Catch ex As Exception
            sbSanitized.Append("xxxxxxxx")
            Throw New Exception(ex.Message)
        End Try

        Return sbSanitized.ToString

    End Function

    Public Function StripTags(ByVal html As String) As String
        ' Remove HTML tags.
        Return Regex.Replace(html, "<.*?>", "")
    End Function

    Public Shared Function IsGUID(ByVal expression As String) As Boolean
        If expression IsNot Nothing Then
            Dim guidRegEx As New Regex("^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$")

            Return guidRegEx.IsMatch(expression)
        End If
        Return False
    End Function

#Region "Private Methods"

    Public Shared Function LoadJsonResult(ByVal result As String, Optional ByVal strVal As String = "") As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()
            json.WritePropertyName("result")
            json.WriteValue(result)
            json.WritePropertyName("value")
            json.WriteValue(strVal)
            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonResult", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonError(ByVal strError As String) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()
            json.WritePropertyName("result")
            json.WriteValue("failure")
            json.WritePropertyName("error")
            json.WriteValue(strError)
            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonError", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonDealerInfo(ByVal dtData As DataTable) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing
        Dim isOk As Boolean = False

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            For Each drv In dtData.DefaultView
                isOk = True
                json.WriteStartObject()
                json.WritePropertyName("result")
                json.WriteValue(drv.item("Brand"))
                json.WritePropertyName("id")
                json.WriteValue(drv.item("ID"))
                json.WritePropertyName("showAccountSettings")
                json.WriteValue(drv.item("showAccountSettings"))
                json.WriteEndObject()
                json.Flush()
            Next

            If isOk = False Then
                json.WriteStartObject()
                json.WritePropertyName("result")
                json.WriteValue("eTrack")
                json.WritePropertyName("id")
                json.WriteValue("")
                json.WritePropertyName("showAccountSettings")
                json.WriteValue(True)
                json.WriteEndObject()
                json.Flush()
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonDealerInfo", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonCredentials(ByVal dvData As DataView) As String
        Dim sbCredentials As StringBuilder = Nothing
        Dim swCredentials As StringWriter = Nothing
        Dim jsonCredentials As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            If dvData.Count > 0 Then
                sbCredentials = New StringBuilder
                swCredentials = New StringWriter(sbCredentials)
                jsonCredentials = New Newtonsoft.Json.JsonTextWriter(swCredentials)

                jsonCredentials.WriteStartObject()
                jsonCredentials.WritePropertyName("tokenCookie")
                jsonCredentials.WriteValue(ConfigurationManager.AppSettings("tokenCookie"))
                jsonCredentials.WritePropertyName("isValid")
                jsonCredentials.WriteValue(dvData.Item(0).Item("IsValid"))
                jsonCredentials.WritePropertyName("token")
                jsonCredentials.WriteValue(dvData.Item(0).Item("token"))

                jsonCredentials.WritePropertyName("companyName")
                Try
                    jsonCredentials.WriteValue(dvData.Item(0).Item("companyName"))
                Catch ex As Exception
                    jsonCredentials.WriteValue("")
                End Try

                jsonCredentials.WritePropertyName("dealerId")
                Try
                    jsonCredentials.WriteValue(dvData.Item(0).Item("DealerID"))
                Catch ex As Exception
                    jsonCredentials.WriteValue(-1)
                End Try

                jsonCredentials.WritePropertyName("fullName")
                jsonCredentials.WriteValue(dvData.Item(0).Item("FullName"))
                jsonCredentials.WritePropertyName("welcomeTitle")
                jsonCredentials.WriteValue(dvData.Item(0).Item("welcomeTitle"))
                jsonCredentials.WritePropertyName("accessLevelId")
                jsonCredentials.WriteValue(dvData.Item(0).Item("AccessLevelID"))
                jsonCredentials.WritePropertyName("isAdministrator")
                jsonCredentials.WriteValue(dvData.Item(0).Item("IsAdministrator"))
                jsonCredentials.WritePropertyName("defaultModuleId")

                If IsNothing(dvData.Item(0).Item("DefaultModuleID")) Then
                    jsonCredentials.WriteValue(1)
                ElseIf IsDBNull(dvData.Item(0).Item("DefaultModuleID")) Then
                    jsonCredentials.WriteValue(1)
                Else
                    jsonCredentials.WriteValue(dvData.Item(0).Item("DefaultModuleID"))
                End If

                jsonCredentials.WritePropertyName("isSuspended")
                Try
                    jsonCredentials.WriteValue(dvData.Item(0).Item("isSuspended"))
                Catch ex As Exception
                    jsonCredentials.WriteValue("0")
                End Try

                jsonCredentials.WritePropertyName("suspendedReasonId")
                Try
                    jsonCredentials.WriteValue(dvData.Item(0).Item("suspendedReasonId"))
                Catch ex As Exception
                    jsonCredentials.WriteValue("0")
                End Try

                jsonCredentials.WritePropertyName("UserGUID")
                Try
                    jsonCredentials.WriteValue(dvData.Item(0).Item("UserGUID"))
                Catch ex As Exception
                    jsonCredentials.WriteValue("")
                End Try

                jsonCredentials.WritePropertyName("transactionId")
                Try
                    jsonCredentials.WriteValue(dvData.Item(0).Item("TransactionId"))
                Catch ex As Exception
                    jsonCredentials.WriteValue("")
                End Try

                jsonCredentials.WriteEndObject()
                jsonCredentials.Flush()
            End If

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonCredentials", "", ex.Message, 0)
        End Try

        Return sbCredentials.ToString()

    End Function

    Public Shared Function LoadJsonToken(ByVal token As String) As String
        Dim sbCredentials As StringBuilder = Nothing
        Dim swCredentials As StringWriter = Nothing
        Dim jsonCredentials As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sbCredentials = New StringBuilder
            swCredentials = New StringWriter(sbCredentials)
            jsonCredentials = New Newtonsoft.Json.JsonTextWriter(swCredentials)

            jsonCredentials.WriteStartObject()
            jsonCredentials.WritePropertyName("tokenCookie")
            jsonCredentials.WriteValue(ConfigurationManager.AppSettings("tokenCookie"))
            jsonCredentials.WritePropertyName("token")
            jsonCredentials.WriteValue(token)
            jsonCredentials.WriteEndObject()
            jsonCredentials.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonToken", "", ex.Message, 0)
        End Try

        Return sbCredentials.ToString()

    End Function

    Public Shared Function LoadJsonEnvelope(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("minLat")
            json.WriteValue(drv.Item("MinLat"))

            json.WritePropertyName("minLon")
            json.WriteValue(drv.Item("MinLon"))

            json.WritePropertyName("maxLat")
            json.WriteValue(drv.Item("MaxLat"))

            json.WritePropertyName("maxLon")
            json.WriteValue(drv.Item("MaxLon"))

            json.WritePropertyName("lastFetchOn")
            json.WriteValue(drv.Item("LastFetchOn").ToString)

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonEnvelope", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonMyDevices(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("DeviceID"))

            json.WritePropertyName("deviceId")
            json.WriteValue(drv.Item("DeviceID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("shortName")
            If Not IsNothing(drv.Item("ShortName")) Then
                json.WriteValue(drv.Item("ShortName"))
            Else
                json.WriteValue("")
            End If

            json.WritePropertyName("lastUpdatedOn")
            json.WriteValue(drv.Item("LastUpdatedOn"))

            json.WritePropertyName("lastUpdatedOnString")
            json.WriteValue(drv.Item("LastUpdatedOn").ToString)

            json.WritePropertyName("eventCode")
            json.WriteValue(drv.Item("EventCode"))

            json.WritePropertyName("eventName")
            json.WriteValue(drv.Item("EventName"))

            json.WritePropertyName("eventDate")
            json.WriteValue(drv.Item("EventDate"))

            json.WritePropertyName("eventDateString")
            json.WriteValue(drv.Item("EventDate").ToString)

            json.WritePropertyName("eventCodeStartedOnString")
            If Not IsNothing(drv.Item("EventCodeStartedOn")) Then
                json.WriteValue(drv.Item("EventCodeStartedOn").ToString)
            Else
                json.WriteValue(drv.Item("EventDate").ToString)
            End If

            json.WritePropertyName("latitude")
            json.WriteValue(drv.Item("Latitude"))

            json.WritePropertyName("longitude")
            json.WriteValue(drv.Item("Longitude"))

            json.WritePropertyName("snailLat1")
            json.WriteValue(drv.Item("snailLat1"))

            json.WritePropertyName("snailLng1")
            json.WriteValue(drv.Item("snailLng1"))

            json.WritePropertyName("snailLat2")
            json.WriteValue(drv.Item("snailLat2"))

            json.WritePropertyName("snailLng2")
            json.WriteValue(drv.Item("snailLng2"))

            json.WritePropertyName("snailLat3")
            json.WriteValue(drv.Item("snailLat3"))

            json.WritePropertyName("snailLng3")
            json.WriteValue(drv.Item("snailLng3"))

            json.WritePropertyName("snailLat4")
            json.WriteValue(drv.Item("snailLat4"))

            json.WritePropertyName("snailLng4")
            json.WriteValue(drv.Item("snailLng4"))

            json.WritePropertyName("speed")
            json.WriteValue(drv.Item("Speed"))

            json.WritePropertyName("heading")
            json.WriteValue(drv.Item("Heading"))

            'json.WritePropertyName("satellites")
            'json.WriteValue(drv.Item("Satellites"))

            'json.WritePropertyName("gpsStatus")
            'json.WriteValue(drv.Item("GPSStatus"))

            json.WritePropertyName("gpsAge")
            If Not IsNothing(drv.Item("gpsAge")) Then
                json.WriteValue(drv.Item("GPSAge"))
            Else
                json.WriteValue(-1)
            End If

            json.WritePropertyName("fullAddress")
            json.WriteValue(drv.Item("FullAddress"))

            json.WritePropertyName("geofenceId")
            json.WriteValue(drv.Item("GeofenceID"))

            json.WritePropertyName("geofenceName")
            json.WriteValue(drv.Item("GeofenceName"))

            json.WritePropertyName("driverId")
            json.WriteValue(drv.Item("DriverID"))

            json.WritePropertyName("driverName")
            json.WriteValue(drv.Item("DriverName"))

            json.WritePropertyName("iconId")
            json.WriteValue(drv.Item("IconID"))

            json.WritePropertyName("iconUrl")
            json.WriteValue(drv.Item("IconURL"))

            json.WritePropertyName("iconLabelLine2")
            json.WriteValue(drv.Item("IconLabelLine2"))

            json.WritePropertyName("idleThreshold")
            json.WriteValue(drv.Item("IdleThreshold"))

            json.WritePropertyName("speedThreshold")
            json.WriteValue(drv.Item("SpeedingThreshold"))

            json.WritePropertyName("infoTable")
            json.WriteValue(drv.Item("InfoTable"))

            If Not IsNothing(drv.Item("IsARB")) Then
                json.WritePropertyName("isARB")
                json.WriteValue(drv.Item("IsARB"))

                json.WritePropertyName("arbNumber")
                json.WriteValue(drv.Item("ARBNumber"))

                'json.WritePropertyName("dieselOnEventCode")
                'json.WriteValue(drv.Item("DieselOnEventCode"))

                'json.WritePropertyName("electricOnEventCode")
                'json.WriteValue(drv.Item("ElectricOnEventCode"))

                json.WritePropertyName("dieselMeter")
                json.WriteValue(drv.Item("DieselMeter"))

                json.WritePropertyName("electricMeter")
                json.WriteValue(drv.Item("ElectricMeter"))
            Else
                json.WritePropertyName("isARB")
                json.WriteValue(False)

                json.WritePropertyName("arbNumber")
                json.WriteValue("")

                'json.WritePropertyName("dieselOnEventCode")
                'json.WriteValue("0")

                'json.WritePropertyName("electricOnEventCode")
                'json.WriteValue("0")

                json.WritePropertyName("dieselMeter")
                json.WriteValue(0)

                json.WritePropertyName("electricMeter")
                json.WriteValue(0)

            End If

            json.WritePropertyName("odometerReading")
            If Not IsNothing(drv.Item("OdometerReading")) Then
                json.WriteValue(drv.Item("OdometerReading"))
            Else
                json.WriteValue(0)
            End If

            json.WritePropertyName("isPowerCut")
            If Not IsNothing(drv.Item("IsPowerCut")) Then
                json.WriteValue(drv.Item("IsPowerCut"))
            Else
                json.WriteValue(False)
            End If

            json.WritePropertyName("isNotWorking")
            If Not IsNothing(drv.Item("isNotWorking")) Then
                json.WriteValue(drv.Item("isNotWorking"))
            Else
                json.WriteValue(False)
            End If

            json.WritePropertyName("txtColor")
            If Not IsNothing(drv.Item("TextColor")) Then
                json.WriteValue(drv.Item("TextColor"))
            Else
                json.WriteValue("#000000")
            End If

            json.WritePropertyName("bgndColor")
            If Not IsNothing(drv.Item("BgndColor")) Then
                json.WriteValue(drv.Item("BgndColor"))
            Else
                json.WriteValue("#ffffff")
            End If

            json.WritePropertyName("isBuzzerOn")
            If Not IsNothing(drv.Item("isBuzzerOn")) Then
                json.WriteValue(drv.Item("isBuzzerOn"))
            Else
                json.WriteValue(False)
            End If

            Try
                json.WritePropertyName("serialNumber")
                If Not IsNothing(drv.Item("serialNumber")) Then
                    json.WriteValue(drv.Item("serialNumber"))
                Else
                    json.WriteValue("")
                End If
            Catch ex As Exception
                json.WriteValue("")
            End Try

            Try
                json.WritePropertyName("vin")
                If Not IsNothing(drv.Item("vin")) Then
                    json.WriteValue(drv.Item("vin"))
                Else
                    json.WriteValue("")
                End If
            Catch ex As Exception
                json.WriteValue("")
            End Try

            Try
                json.WritePropertyName("fuelCardId")
                If Not IsNothing(drv.Item("fuelCardUnitId")) Then
                    json.WriteValue(drv.Item("fuelCardUnitId"))
                Else
                    json.WriteValue("")
                End If
            Catch ex As Exception
                json.WriteValue("")
            End Try

            Try
                json.WritePropertyName("licensePlate")
                If Not IsNothing(drv.Item("licensePlate")) Then
                    json.WriteValue(drv.Item("licensePlate"))
                Else
                    json.WriteValue("")
                End If
            Catch ex As Exception
                json.WriteValue("")
            End Try

            Try
                If Not IsNothing(drv.Item("hasInputs")) Then

                    json.WritePropertyName("hasInputs")
                    json.WriteValue(drv.Item("hasInputs"))

                    json.WritePropertyName("hasPortExpander")
                    json.WriteValue(drv.Item("hasPortExpander"))

                    json.WritePropertyName("sw1_isConnected")
                    json.WriteValue(drv.Item("sw1_isConnected"))

                    json.WritePropertyName("sw2_isConnected")
                    json.WriteValue(drv.Item("sw2_isConnected"))

                    json.WritePropertyName("sw3_isConnected")
                    json.WriteValue(drv.Item("sw3_isConnected"))

                    json.WritePropertyName("sw4_isConnected")
                    json.WriteValue(drv.Item("sw4_isConnected"))

                    json.WritePropertyName("input1Name")
                    json.WriteValue(drv.Item("input1Name"))

                    json.WritePropertyName("input2Name")
                    json.WriteValue(drv.Item("input2Name"))

                    json.WritePropertyName("input3Name")
                    json.WriteValue(drv.Item("input3Name"))

                    json.WritePropertyName("input4Name")
                    json.WriteValue(drv.Item("input4Name"))

                    json.WritePropertyName("sw1")
                    json.WriteValue(drv.Item("sw1"))

                    json.WritePropertyName("sw2")
                    json.WriteValue(drv.Item("sw2"))

                    json.WritePropertyName("sw3")
                    json.WriteValue(drv.Item("sw3"))

                    json.WritePropertyName("sw4")
                    json.WriteValue(drv.Item("sw4"))

                    json.WritePropertyName("pto1")
                    json.WriteValue(drv.Item("pto1"))

                    json.WritePropertyName("pto2")
                    json.WriteValue(drv.Item("pto2"))

                    json.WritePropertyName("pto3")
                    json.WriteValue(drv.Item("pto3"))

                    json.WritePropertyName("pto4")
                    json.WriteValue(drv.Item("pto4"))

                    json.WritePropertyName("pto5")
                    json.WriteValue(drv.Item("pto5"))

                    json.WritePropertyName("pto6")
                    json.WriteValue(drv.Item("pto6"))

                    json.WritePropertyName("pto7")
                    json.WriteValue(drv.Item("pto7"))

                    json.WritePropertyName("pto8")
                    json.WriteValue(drv.Item("pto8"))

                End If
            Catch ex As Exception

            End Try

            json.WritePropertyName("isBadIgnitionInstall")
            Try
                json.WriteValue(drv.Item("isBadIgnitionInstall"))
            Catch ex As Exception
                json.WriteValue(False)
            End Try

            Try
                json.WritePropertyName("relay1")
                json.WriteValue(drv.Item("relay1"))

                json.WritePropertyName("relay2")
                json.WriteValue(drv.Item("relay2"))

                json.WritePropertyName("relay3")
                json.WriteValue(drv.Item("relay3"))

                json.WritePropertyName("relay4")
                json.WriteValue(drv.Item("relay4"))
            Catch ex As Exception

            End Try

            json.WritePropertyName("showDevice")
            Try
                If Not IsNothing(drv.Item("showDevice")) Then
                    json.WriteValue(drv.Item("showDevice"))
                Else
                    json.WriteValue(True)
                End If
            Catch ex As Exception
                json.WriteValue(True)
            End Try

            json.WritePropertyName("hasIButtonWithoutDriver")
            Try
                If Not IsNothing(drv.Item("hasIButtonWithoutDriver")) Then
                    json.WriteValue(drv.Item("hasIButtonWithoutDriver"))
                Else
                    json.WriteValue(0)
                End If
            Catch ex As Exception
                json.WriteValue(0)
            End Try

            json.WritePropertyName("iButtonRaw")
            Try
                If Not IsNothing(drv.Item("iButtonRaw")) Then
                    json.WriteValue(drv.Item("iButtonRaw"))
                Else
                    json.WriteValue("")
                End If
            Catch ex As Exception
                json.WriteValue("")
            End Try


            json.WritePropertyName("SWCameras")
            Try
                json.WriteValue(drv.Item("SWCameras"))
            Catch ex As Exception
                json.WriteValue(0)
            End Try

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonMyDevices", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonTrail(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("eventCode")
            json.WriteValue(drv.Item("EventCode"))

            json.WritePropertyName("eventName")
            json.WriteValue(drv.Item("EventName"))

            json.WritePropertyName("eventDate")
            json.WriteValue(drv.Item("EventDate"))

            json.WritePropertyName("eventDateString")
            json.WriteValue(drv.Item("EventDate").ToString)

            json.WritePropertyName("latitude")
            json.WriteValue(drv.Item("Latitude"))

            json.WritePropertyName("longitude")
            json.WriteValue(drv.Item("Longitude"))

            json.WritePropertyName("speed")
            json.WriteValue(drv.Item("Speed"))

            json.WritePropertyName("heading")
            json.WriteValue(drv.Item("Heading"))

            json.WritePropertyName("fullAddress")
            json.WriteValue(drv.Item("FullAddress"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonTrail", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadDocQty(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("qty")
            json.WriteValue(drv.Item("Qty"))

            json.WritePropertyName("shippingOption")
            json.WriteValue(drv.Item("ShippingOption"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadDocQty", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadUnsubscribeInfo(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("ContactName"))

            json.WritePropertyName("quote")
            json.WriteValue(drv.Item("Quote"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadUnsubscribeInfo", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadFamousQuote(ByVal Quote As String) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("quote")
            json.WriteValue(Quote)

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadFamousQuote", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function createDevicesTable(ByVal token As String, ByVal jsonDevicesTXT As String) As DataTable
        Dim dtDevices As New DataTable
        Dim jsonObj As Object
        Dim device As Object
        Dim row As DataRow

        Dim deviceId As String = ""
        Dim deviceName As String = ""

        Try
            dtDevices.Columns.Add("DeviceID", GetType(String))
            dtDevices.Columns.Add("DeviceName", GetType(String))

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonDevicesTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    device = jsonObj(i)

                    If Not IsNothing(device.item("deviceId")) Then
                        deviceId = CStr(device.item("deviceId").value)
                    Else
                        deviceId = ""
                    End If
                    If Not IsNothing(device.item("deviceName")) Then
                        deviceName = CStr(device.item("deviceName").value)
                    Else
                        deviceName = ""
                    End If

                    row = dtDevices.NewRow
                    row("DeviceID") = deviceId
                    row("DeviceName") = deviceName
                    dtDevices.Rows.Add(row)

                Next
            End If

        Catch ex As Exception
            dtDevices = Nothing
        End Try

        Return dtDevices

    End Function

    Public Shared Function createModulesTable(ByVal token As String, ByVal jsonModulesTXT As String) As DataTable
        Dim dtModules As New DataTable
        Dim jsonObj As Object
        Dim modu As Object
        Dim row As DataRow

        Dim moduleId As Integer = 0
        Dim moduleName As String = ""

        Try
            dtModules.Columns.Add("ModuleID", GetType(Int32))
            dtModules.Columns.Add("ModuleName", GetType(String))
            dtModules.Columns.Add("UserID", GetType(Int32))

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonModulesTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    modu = jsonObj(i)

                    If Not IsNothing(modu.item("moduleId")) Then
                        If IsNumeric(0) Then
                            moduleId = CInt(modu.item("moduleId").value)
                        Else
                            moduleId = 0
                        End If
                    Else
                        moduleId = 0
                    End If
                    If Not IsNothing(modu.item("name")) Then
                        moduleName = CStr(modu.item("name").value)
                    Else
                        moduleName = ""
                    End If

                    row = dtModules.NewRow
                    row("ModuleID") = moduleId
                    row("ModuleName") = moduleName
                    row("UserID") = 0
                    dtModules.Rows.Add(row)

                Next
            End If

        Catch ex As Exception
            dtModules = Nothing
        End Try

        Return dtModules

    End Function

    Public Shared Function createIdNameTable(ByVal jsonTXT As String) As DataTable
        Dim dtData As New DataTable
        Dim jsonObj As Object
        Dim rec As Object
        Dim row As DataRow

        Dim id As String = ""
        Dim name As String = ""

        Try
            dtData.Columns.Add("ID", GetType(String))
            dtData.Columns.Add("Name", GetType(String))

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    rec = jsonObj(i)

                    If Not IsNothing(rec.item("id")) Then
                        id = CStr(rec.item("id").value)
                    Else
                        id = ""
                    End If
                    If Not IsNothing(rec.item("name")) Then
                        name = CStr(rec.item("name").value)
                    Else
                        name = ""
                    End If

                    row = dtData.NewRow
                    row("ID") = id
                    row("Name") = name
                    dtData.Rows.Add(row)

                Next
            End If

        Catch ex As Exception
            dtData = Nothing
        End Try

        Return dtData

    End Function

    Public Shared Function createUsersTable(ByVal token As String, ByVal jsonUsersTXT As String) As DataTable
        Dim dtUsers As New DataTable
        Dim jsonObj As Object
        Dim user As Object
        Dim row As DataRow

        Dim userGUID As String = ""

        Try
            dtUsers.Columns.Add("UserGUID", GetType(String))

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonUsersTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    user = jsonObj(i)

                    If Not IsNothing(user.item("id")) Then
                        userGUID = user.item("id").value
                    Else
                        userGUID = 0
                    End If

                    row = dtUsers.NewRow
                    row("UserGUID") = userGUID
                    dtUsers.Rows.Add(row)

                Next
            End If

        Catch ex As Exception
            dtUsers = Nothing
        End Try

        Return dtUsers

    End Function

    Public Shared Function createAlertUsersTable(ByVal token As String, ByVal jsonUsersTXT As String) As DataTable
        Dim dtUsers As New DataTable
        Dim jsonObj As Object
        Dim user As Object
        Dim row As DataRow

        Dim userGUID As String = ""
        Dim isEmail As Boolean = False
        Dim isSMS As Boolean = False

        Try
            dtUsers.Columns.Add("UserGUID", GetType(String))
            dtUsers.Columns.Add("isEmail", GetType(Boolean))
            dtUsers.Columns.Add("isSMS", GetType(Boolean))

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonUsersTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    user = jsonObj(i)

                    If Not IsNothing(user.item("id")) Then
                        userGUID = user.item("id").value
                    Else
                        userGUID = 0
                    End If

                    isEmail = False
                    If Not IsNothing(user.item("isEmail")) Then
                        If user.item("isEmail").value = True Then
                            isEmail = True
                        End If
                    End If

                    isSMS = False
                    If Not IsNothing(user.item("isSMS")) Then
                        If user.item("isSMS").value = True Then
                            isSMS = True
                        End If
                    End If

                    row = dtUsers.NewRow
                    row("UserGUID") = userGUID
                    row("isEmail") = isEmail
                    row("isSMS") = isSMS
                    dtUsers.Rows.Add(row)

                Next
            End If

        Catch ex As Exception
            dtUsers = Nothing
        End Try

        Return dtUsers

    End Function

    Public Shared Function LoadMTGroups(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("deviceId")
            json.WriteValue(drv.Item("DeviceID"))

            json.WritePropertyName("deviceName")
            json.WriteValue(drv.Item("DeviceName"))

            json.WritePropertyName("deviceShortName")
            If Not IsNothing(drv.Item("DeviceShortName")) Then
                json.WriteValue(drv.Item("DeviceShortName"))
            Else
                json.WriteValue("")
            End If

            json.WritePropertyName("panelId")
            json.WriteValue(drv.Item("PanelID"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadMTGroups", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadReports(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("hasParam")
            json.WriteValue(drv.Item("HasParam"))

            json.WritePropertyName("paramName")
            json.WriteValue(drv.Item("ParamName"))

            json.WritePropertyName("showDates")
            json.WriteValue(drv.Item("showDates"))

            json.WritePropertyName("showHours")
            json.WriteValue(drv.Item("showHours"))

            json.WritePropertyName("isDelayed")
            json.WriteValue(drv.Item("isDelayed24Hours"))

            json.WritePropertyName("showAllDevicesOption")
            json.WriteValue(drv.Item("showAllDevicesOption"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadReports", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadRecurrentReports(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("reportId")
            json.WriteValue(drv.Item("ReportID"))

            json.WritePropertyName("reportName")
            json.WriteValue(drv.Item("ReportName"))

            json.WritePropertyName("frequencyId")
            json.WriteValue(drv.Item("FrequencyID"))

            json.WritePropertyName("frequencyName")
            json.WriteValue(drv.Item("FrequencyName"))

            json.WritePropertyName("excludeWeekends")
            json.WriteValue(drv.Item("ExcludeWeekends"))

            json.WritePropertyName("isAllDevices")
            json.WriteValue(drv.Item("IsAllDevices"))

            json.WritePropertyName("createdOn")
            json.WriteValue(drv.Item("CreatedOn").ToString)

            json.WritePropertyName("hasParam")
            json.WriteValue(drv.Item("HasParam").ToString)

            json.WritePropertyName("paramName")
            json.WriteValue(drv.Item("ParamName").ToString)

            json.WritePropertyName("paramValue")
            json.WriteValue(drv.Item("ParamValue").ToString)

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadRecurrentReports", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadFrequencies(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("description")
            json.WriteValue(drv.Item("Description"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadFrequencies", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadEvents(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("EventCode"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadEvents", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadDrivers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("phone")
            json.WriteValue(drv.Item("Phone"))

            json.WritePropertyName("email")
            json.WriteValue(drv.Item("Email"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadDrivers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadIcons(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("url")
            json.WriteValue(drv.Item("URL"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadIcons", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonGeofences(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("GUID"))

            json.WritePropertyName("geofenceTypeId")
            json.WriteValue(drv.Item("GeofenceTypeId"))

            json.WritePropertyName("geofenceTypeName")
            json.WriteValue(drv.Item("GeofenceTypeName"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("street")
            json.WriteValue(drv.Item("Street"))

            json.WritePropertyName("streetNumber")
            json.WriteValue(drv.Item("StreetNumber"))

            json.WritePropertyName("route")
            json.WriteValue(drv.Item("Route"))

            json.WritePropertyName("suite")
            json.WriteValue(drv.Item("Suite"))

            json.WritePropertyName("city")
            json.WriteValue(drv.Item("City"))

            json.WritePropertyName("county")
            json.WriteValue(drv.Item("County"))

            json.WritePropertyName("state")
            json.WriteValue(drv.Item("State"))

            json.WritePropertyName("postalCode")
            json.WriteValue(drv.Item("PostalCode"))

            json.WritePropertyName("country")
            json.WriteValue(drv.Item("Country"))

            json.WritePropertyName("fullAddress")
            json.WriteValue(drv.Item("FullAddress"))

            json.WritePropertyName("latitude")
            json.WriteValue(drv.Item("Latitude"))

            json.WritePropertyName("longitude")
            json.WriteValue(drv.Item("Longitude"))

            json.WritePropertyName("radius")
            json.WriteValue(drv.Item("RadiusFeet"))

            json.WritePropertyName("iconUrl")
            json.WriteValue(drv.Item("IconURL"))

            json.WritePropertyName("geofenceAlertTypeId")
            json.WriteValue(drv.Item("GeofenceAlertTypeId"))

            json.WritePropertyName("geofenceAlertTypeName")
            json.WriteValue(drv.Item("GeofenceAlertTypeName"))

            json.WritePropertyName("comments")
            json.WriteValue(drv.Item("Comments"))

            json.WritePropertyName("shapeId")
            json.WriteValue(drv.Item("ShapeID"))

            json.WritePropertyName("jsonPolyVerticesTXT")
            json.WriteValue(drv.Item("jsonPolyVerticesTXT"))

            json.WritePropertyName("lastVisitedOn")
            If IsDBNull(drv.Item("LastVisitedOn")) Then
                json.WriteValue("N/A")
            Else
                json.WriteValue(drv.Item("LastVisitedOn").ToString)
            End If

            json.WritePropertyName("geofenceInfoTable")
            json.WriteValue(drv.Item("GeofenceInfoTable"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonGeofences", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonUsers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("GUID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("firstName")
            json.WriteValue(drv.Item("FirstName"))

            json.WritePropertyName("lastName")
            json.WriteValue(drv.Item("lastName"))

            json.WritePropertyName("email")
            json.WriteValue(drv.Item("Email"))

            json.WritePropertyName("isEmailAlerts")
            json.WriteValue(drv.Item("IsEmailAlerts"))

            json.WritePropertyName("phone")
            json.WriteValue(drv.Item("Phone"))

            json.WritePropertyName("cellPhone")
            json.WriteValue(drv.Item("CellPhone"))

            json.WritePropertyName("isSMSAlerts")
            json.WriteValue(drv.Item("IsSMSAlerts"))

            json.WritePropertyName("carrierId")
            json.WriteValue(drv.Item("SMSGatewayID"))

            json.WritePropertyName("login")
            json.WriteValue(drv.Item("Login"))

            json.WritePropertyName("timeZoneCode")
            json.WriteValue(drv.Item("TimeZoneCode"))

            json.WritePropertyName("isDriver")
            json.WriteValue(drv.Item("IsDriver"))

            json.WritePropertyName("accessLevelId")
            json.WriteValue(drv.Item("AccessLevelID"))

            json.WritePropertyName("accessLevelName")
            json.WriteValue(drv.Item("AccessLevelName"))

            json.WritePropertyName("scheduleId")
            json.WriteValue(drv.Item("ScheduleID"))

            json.WritePropertyName("scheduleName")
            json.WriteValue(drv.Item("ScheduleName"))

            json.WritePropertyName("isAdministrator")
            json.WriteValue(drv.Item("IsAdministrator"))

            json.WritePropertyName("iButton")
            json.WriteValue(drv.Item("iButton"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonUsers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonCarriers(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonCarriers", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonGeoAlertsTypes(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonGeoAlertsTypes", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonGeoTypes(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonGeoTypes", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonAccessLevels(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("description")
            json.WriteValue(drv.Item("Description"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonAccessLevels", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function LoadJsonAppFeatures(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WritePropertyName("minALID")
            json.WriteValue(drv.Item("MinAccessLevelID"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonAppFeatures", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Sub getFormattedData(ByVal verticesTXT As String, ByRef KMLData As String, ByRef SQLData As String)
        Dim jsonObj As Object
        Dim vertice As Object
        Dim lat As Double
        Dim lng As Double

        Dim firstLat As Double = 0
        Dim firstLng As Double = 0

        Try

            KMLData = "<Polygon><outerBoundaryIs><coordinates>"
            SQLData = "POLYGON(("

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(verticesTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    vertice = jsonObj(i)

                    If Not IsNothing(vertice.item("lat")) Then
                        lat = CDbl(vertice.item("lat").value)
                    Else
                        lat = 0
                    End If

                    If Not IsNothing(vertice.item("lng")) Then
                        lng = CDbl(vertice.item("lng").value)
                    Else
                        lng = 0
                    End If

                    If i > 0 Then
                        KMLData &= " "
                        SQLData &= ","
                    Else
                        firstLat = lat
                        firstLng = lng
                    End If

                    KMLData &= lng.ToString & "," & lat.ToString
                    SQLData &= lng.ToString & " " & lat.ToString

                Next
            End If

            KMLData &= "</coordinates></outerBoundaryIs></Polygon>"
            SQLData &= "," & firstLng.ToString & " " & firstLat.ToString & "))"

        Catch ex As Exception
            KMLData = ""
        End Try
    End Sub

    Public Shared Sub getFormattedData(ByVal lat As Double, ByVal lng As Double, ByRef KMLData As String, ByRef SQLData As String)
        Try
            KMLData = "<Point><coordinates>"
            KMLData &= lng.ToString & "," & lat.ToString
            KMLData &= "</coordinates></Point>"

            SQLData = "POINT("
            SQLData &= lng.ToString & " " & lat.ToString
            SQLData &= ")"

        Catch ex As Exception
            KMLData = ""
            SQLData = ""
        End Try
    End Sub

    Public Shared Function LoadJsonStates(ByVal drv As DataRowView) As String
        Dim sb As StringBuilder = Nothing
        Dim sw As StringWriter = Nothing
        Dim json As Newtonsoft.Json.JsonTextWriter = Nothing

        Try
            sb = New StringBuilder
            sw = New StringWriter(sb)
            json = New Newtonsoft.Json.JsonTextWriter(sw)

            json.WriteStartObject()

            json.WritePropertyName("id")
            json.WriteValue(drv.Item("ID"))

            json.WritePropertyName("name")
            json.WriteValue(drv.Item("Name"))

            json.WriteEndObject()
            json.Flush()

        Catch ex As Exception
            BLErrorHandling.ErrorCapture(pSysModule, "LoadJsonStates", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

    Public Shared Function createScheduleValuesTable(ByVal token As String, ByVal jsonValuesTXT As String) As DataTable
        Dim dtValues As New DataTable
        Dim jsonObj As Object
        Dim value As Object
        Dim row As DataRow

        Dim day As Integer = 0
        Dim blockFrom As Integer = 0
        Dim blockTo As Integer = 0

        Try
            dtValues.Columns.Add("WeekDay", GetType(Integer))
            dtValues.Columns.Add("FromHour", GetType(Integer))
            dtValues.Columns.Add("ToHour", GetType(Integer))

            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonValuesTXT)
            If Not IsNothing(jsonObj) Then
                For i = 0 To (jsonObj.count - 1)
                    value = jsonObj(i)

                    If Not IsNothing(value.item("day")) Then
                        day = CStr(value.item("day").value)

                        If Not IsNothing(value.item("from")) Then
                            blockFrom = CStr(value.item("from").value)
                        Else
                            blockFrom = 0
                        End If
                        If Not IsNothing(value.item("to")) Then
                            blockTo = CStr(value.item("to").value)
                        Else
                            blockTo = 0
                        End If
                    Else
                        day = 0
                    End If

                    If day > 0 Then
                        row = dtValues.NewRow
                        row("WeekDay") = day
                        row("FromHour") = blockFrom
                        row("ToHour") = blockTo
                        dtValues.Rows.Add(row)
                    End If
                Next
            End If

        Catch ex As Exception
            dtValues = Nothing
        End Try

        Return dtValues

    End Function

#End Region

End Class
