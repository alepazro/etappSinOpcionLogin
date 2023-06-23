Imports System.Runtime.Serialization

Public Class tmpImage

    Public imageId As String
    Public fileName As String
    Public fileType As String
    Public imgType As Integer
    Public imgData As Byte()
    Public isOk As Boolean
    Public msg As String
    Public UrlImagen As String
    Public Status As String

End Class

<DataContract> _
Public Class imgData

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public jobId As String = ""

    <DataMember> _
    Public imageId As String = ""

    <DataMember> _
    Public imgType As Integer = 0 '1: Picture, 2: Signature

    <DataMember> _
    Public imgId As String = ""

    <DataMember> _
    Public imgName As String = ""

    <DataMember> _
    Public fileName As String = ""

    <DataMember> _
    Public fileType As String = "" 'PNG, JPG, BMP,...

    <DataMember> _
    Public imgData As String = ""

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public lat As String = ""

    <DataMember> _
    Public lng As String = ""

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember>
    Public msg As String = "" 'This is the note associated to the image, entered by the user
    <DataMember>
    Public UrlImagen As String = ""
    <DataMember>
    Public Status As String = ""
    <DataMember>
    Public GUID As String = ""
    <DataMember>
    Public UpdateFrom As Integer = 0 '1 devices 2 web

End Class
