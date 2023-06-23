Imports System.Runtime.Serialization

<DataContract>
Public Class Jobobject
    <DataMember>
    Public UniqueKey As String = ""
    <DataMember>
    Public JobNumber As String = ""
    <DataMember>
    Public deviceId As String = ""
    <DataMember>
    Public isGeofence As String = ""
    <DataMember>
    Public geof_Id As String = ""
    <DataMember>
    Public geof_name As String = ""
    <DataMember>
    Public geof_phone As String = ""
    <DataMember>
    Public geof_street As String = ""
    <DataMember>
    Public geof_city As String = ""
    <DataMember>
    Public geof_state As String = ""
    <DataMember>
    Public geof_postalCode As String = ""
    <DataMember>
    Public job_description As String = ""
    <DataMember>
    Public geof_latitud As String = ""
    <DataMember>
    Public geof_longitud As String = ""
    <DataMember>
    Public send_sms As String = ""
    <DataMember>
    Public driverId As String = ""
    <DataMember>
    Public via As String = ""
    <DataMember>
    Public hour As String = ""
    <DataMember>
    Public minute As String = ""
    <DataMember>
    Public dueDate As String = ""
    <DataMember>
    Public StartOn As String = ""
    <DataMember>
    Public jobpriority As String = ""
    <DataMember>
    Public jobcategories As String = ""
    <DataMember>
    Public jobstoplist As List(Of JobStop) = New List(Of JobStop)
    <DataMember>
    Public notes As List(Of JobNote) = New List(Of JobNote)
    <DataMember>
    Public picturesList As List(Of imgData) = New List(Of imgData)
    <DataMember>
    Public RadiusFeet As Integer = 0
    <DataMember>
    Public UpdateFrom As Integer = 0
    <DataMember>
    Public signature As imgData
    <DataMember>
    Public msg As String = ""
    <DataMember>
    Public StatusID As String = ""
    <DataMember>
    Public isOk As Boolean = False



End Class
