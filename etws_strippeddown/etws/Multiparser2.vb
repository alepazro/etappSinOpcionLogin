Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Generic


Namespace HttpUtils
    Public Class HttpContentParser
        Public Sub New(stream As Stream)
            Me.Parse(stream, Encoding.UTF8)
        End Sub

        Public Sub New(stream As Stream, encoding As Encoding)
            Me.Parse(stream, encoding)
        End Sub

        Private Sub Parse(stream As Stream, encoding As Encoding)
            Me.Success = False

            ' Read the stream into a byte array
            Dim data As Byte() = Misc.ToByteArray(stream)

            ' Copy to a string for header parsing
            Dim content As String = encoding.GetString(data)

            Dim name As String = String.Empty
            Dim value As String = String.Empty
            Dim lookForValue As Boolean = False
            Dim charCount As Integer = 0

            For Each c In content
                If c = "="c Then
                    lookForValue = True
                ElseIf c = "&"c Then
                    lookForValue = False
                    AddParameter(name, value)
                    name = String.Empty
                    value = String.Empty
                ElseIf Not lookForValue Then
                    name += c
                Else
                    value += c
                End If

                If System.Threading.Interlocked.Increment(charCount) = content.Length Then
                    AddParameter(name, value)
                    Exit For
                End If
            Next

            ' Get the start & end indexes of the file contents
            'int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
            'Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());

            ' If some data has been successfully received, set success to true
            If Parameters.Count <> 0 Then
                Me.Success = True
            End If
        End Sub

        Private Sub AddParameter(name As String, value As String)
            If Not String.IsNullOrWhiteSpace(name) AndAlso Not String.IsNullOrWhiteSpace(value) Then
                Parameters.Add(name.Trim(), value.Trim())
            End If
        End Sub

        Public Parameters As IDictionary(Of String, String) = New Dictionary(Of String, String)()

        Public Property Success() As Boolean
            Get
                Return m_Success
            End Get
            Private Set
                m_Success = Value
            End Set
        End Property

        Private m_Success As Boolean

    End Class

    Public Class HttpMultipartParser
        Public Sub New(stream As Stream, filePartName__1 As String)
            FilePartName = filePartName__1
            Me.Parse(stream, Encoding.UTF8)
        End Sub

        Public Sub New(stream As Stream, encoding As Encoding, filePartName__1 As String)
            FilePartName = filePartName__1
            Me.Parse(stream, encoding)
        End Sub

        Private Sub Parse(stream As Stream, encoding As Encoding)
            Me.Success = False

            ' Read the stream into a byte array
            Dim data As Byte() = Misc.ToByteArray(stream)

            ' Copy to a string for header parsing
            Dim content As String = encoding.GetString(data)

            ' The first line should contain the delimiter
            Dim delimiterEndIndex As Integer = content.IndexOf(vbCr & vbLf)

            If delimiterEndIndex > -1 Then
                Dim delimiter As String = content.Substring(0, content.IndexOf(vbCr & vbLf))

                Dim sections As String() = content.Split(New String() {delimiter}, StringSplitOptions.RemoveEmptyEntries)

                For Each s As String In sections
                    If s.Contains("Content-Disposition") Then
                        ' If we find "Content-Disposition", this is a valid multi-part section
                        ' Now, look for the "name" parameter
                        Dim nameMatch As Match = New Regex("(?<=name\=\"")(.*?)(?=\"")").Match(s)
                        Dim name As String = nameMatch.Value.Trim().ToLower()

                        If name = FilePartName Then
                            ' Look for Content-Type
                            Dim re As New Regex("(?<=Content\-Type:)(.*?)(?=\r\n\r\n)")
                            Dim contentTypeMatch As Match = re.Match(content)

                            ' Look for filename
                            re = New Regex("(?<=filename\=\"")(.*?)(?=\"")")
                            Dim filenameMatch As Match = re.Match(content)

                            ' Did we find the required values?
                            If contentTypeMatch.Success AndAlso filenameMatch.Success Then
                                ' Set properties
                                Me.ContentType = contentTypeMatch.Value.Trim()
                                Me.Filename = filenameMatch.Value.Trim()

                                ' Get the start & end indexes of the file contents
                                Dim startIndex As Integer = contentTypeMatch.Index + contentTypeMatch.Length + vbCr & vbLf & vbCr & vbLf.Length

                                Dim delimiterBytes As Byte() = encoding.GetBytes(Convert.ToString(vbCr & vbLf) & delimiter)
                                Dim endIndex As Integer = Misc.IndexOf(data, delimiterBytes, startIndex)

                                Dim contentLength As Integer = endIndex - startIndex

                                ' Extract the file contents from the byte array
                                Dim fileData As Byte() = New Byte(contentLength - 1) {}

                                Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength)

                                Me.FileContents = fileData
                            End If
                        ElseIf Not String.IsNullOrWhiteSpace(name) Then
                            ' Get the start & end indexes of the file contents
                            Dim startIndex As Integer = nameMatch.Index + nameMatch.Length + vbCr & vbLf & vbCr & vbLf.Length
                            Parameters.Add(name, s.Substring(startIndex).TrimEnd(New Char() {ControlChars.Cr, ControlChars.Lf}).Trim())
                        End If
                    End If
                Next

                ' If some data has been successfully received, set success to true
                If FileContents IsNot Nothing OrElse Parameters.Count <> 0 Then
                    Me.Success = True
                End If
            End If
        End Sub

        Public Parameters As IDictionary(Of String, String) = New Dictionary(Of String, String)()

        Public Property FilePartName() As String
            Get
                Return m_FilePartName
            End Get
            Private Set
                m_FilePartName = Value
            End Set
        End Property
        Private m_FilePartName As String

        Public Property Success() As Boolean
            Get
                Return m_Success
            End Get
            Private Set
                m_Success = Value
            End Set
        End Property
        Private m_Success As Boolean

        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Private Set
                m_Title = Value
            End Set
        End Property
        Private m_Title As String

        Public Property UserId() As Integer
            Get
                Return m_UserId
            End Get
            Private Set
                m_UserId = Value
            End Set
        End Property
        Private m_UserId As Integer

        Public Property ContentType() As String
            Get
                Return m_ContentType
            End Get
            Private Set
                m_ContentType = Value
            End Set
        End Property
        Private m_ContentType As String

        Public Property Filename() As String
            Get
                Return m_Filename
            End Get
            Private Set
                m_Filename = Value
            End Set
        End Property
        Private m_Filename As String

        Public Property FileContents() As Byte()
            Get
                Return m_FileContents
            End Get
            Private Set
                m_FileContents = Value
            End Set
        End Property
        Private m_FileContents As Byte()
    End Class

    Public NotInheritable Class Misc
        Private Sub New()
        End Sub
        Public Shared Function IndexOf(searchWithin As Byte(), serachFor As Byte(), startIndex As Integer) As Integer
            Dim index As Integer = 0
            Dim startPos As Integer = Array.IndexOf(searchWithin, serachFor(0), startIndex)

            If startPos <> -1 Then
                While (startPos + index) < searchWithin.Length
                    If searchWithin(startPos + index) = serachFor(index) Then
                        index += 1
                        If index = serachFor.Length Then
                            Return startPos
                        End If
                    Else
                        startPos = Array.IndexOf(Of Byte)(searchWithin, serachFor(0), startPos + index)
                        If startPos = -1 Then
                            Return -1
                        End If
                        index = 0
                    End If
                End While
            End If

            Return -1
        End Function

        Public Shared Function ToByteArray(stream As Stream) As Byte()
            Dim buffer As Byte() = New Byte(32767) {}
            Using ms As New MemoryStream()
                While True
                    Dim read As Integer = stream.Read(buffer, 0, buffer.Length)
                    If read <= 0 Then
                        Return ms.ToArray()
                    End If
                    ms.Write(buffer, 0, read)
                End While
            End Using

            Return Nothing

        End Function

    End Class

End Namespace
