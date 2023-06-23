Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Text.StringBuilder

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


Public Class BLCommon

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

    Public Shared Function IsGUID(ByVal expression As String) As Boolean
        If expression IsNot Nothing Then
            Dim guidRegEx As New Regex("^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$")

            Return guidRegEx.IsMatch(expression)
        End If
        Return False
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

    Public Shared Function getDateAsString(ByVal theDate As Date) As String
        Dim strDate As String = ""

        Try
            strDate = DatePart(DateInterval.Month, theDate) & "/" & DatePart(DateInterval.Day, theDate) & "/" & DatePart(DateInterval.Year, theDate)
        Catch ex As Exception
            strDate = ""
        End Try

        Return strDate

    End Function

    Public Function StripTags(ByVal html As String) As String
        ' Remove HTML tags.
        Return Regex.Replace(html, "<.*?>", "")
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
            BLErrorHandling.ErrorCapture("etws.BLCommon", "LoadJsonError", "", ex.Message, 0)
        End Try

        Return sb.ToString()

    End Function

End Class
