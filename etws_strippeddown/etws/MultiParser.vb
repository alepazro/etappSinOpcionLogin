Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Namespace AntsCode.Util
    Public Class MultipartParser
        Public Sub New(stream As Stream)
            Me.Parse(stream, Encoding.UTF8)
        End Sub

        Public Sub New(stream As Stream, encoding As Encoding)
            Me.Parse(stream, encoding)
        End Sub

        Private Sub Parse(stream As Stream, encoding As Encoding)
            Me.Success = False

            ' Read the stream into a byte array
            Dim data As Byte() = ToByteArray(stream)

            ' Copy to a string for header parsing
            Dim content As String = encoding.GetString(data)

            ' The first line should contain the delimiter
            Dim delimiterEndIndex As Integer = content.IndexOf(vbCr & vbLf)

            If delimiterEndIndex > -1 Then
                Dim delimiter As String = content.Substring(0, content.IndexOf(vbCr & vbLf))

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
                    Dim endIndex As Integer = IndexOf(data, delimiterBytes, startIndex)

                    Dim contentLength As Integer = endIndex - startIndex

                    ' Extract the file contents from the byte array
                    Dim fileData As Byte() = New Byte(contentLength - 1) {}

                    Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength)

                    Me.FileContents = fileData
                    Me.Success = True
                End If
            End If
        End Sub

        Private Function IndexOf(searchWithin As Byte(), serachFor As Byte(), startIndex As Integer) As Integer
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

        Private Function ToByteArray(stream As Stream) As Byte()
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
        End Function

        Public Property Success() As Boolean
            Get
                Return m_Success
            End Get
            Private Set
                m_Success = Value
            End Set
        End Property
        Private m_Success As Boolean

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

End Namespace

