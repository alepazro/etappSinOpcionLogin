Imports System.Runtime.Serialization

Public Class CRM_Customers_GetByUniqueKeyCompanies
    <DataMember> Public name As String = ""
    <DataMember> Public newCustomerCase As String = ""
    <DataMember> Public paymentMethod As String = ""
    <DataMember> Public billingDay As String = ""
    <DataMember> Public isVVIP As String = ""
End Class
Public Class CRM_Customers_GetByUniqueKeyUsers
    <DataMember> Public id As String = ""
    <DataMember> Public guid As String = ""
    <DataMember> Public name As String = ""
    <DataMember> Public email As String = ""
    <DataMember> Public phone As String = ""
    <DataMember> Public cellPhone As String = ""
    <DataMember> Public userName As String = ""
    <DataMember> Public lastLoginOn As String = ""
    <DataMember> Public qtyLogins As String = ""
    <DataMember> Public timeZoneCode As String = ""
    <DataMember> Public isDriver As String = ""
    <DataMember> Public createdOn As String = ""
    <DataMember> Public accessLevelID As String = ""
    <DataMember> Public isAdministrator As String = ""
    <DataMember> Public userLogin As String = ""
    <DataMember> Public credentialsReminder As String = ""
    <DataMember> Public newUserCase As String = ""


End Class
Public Class CRM_Customers_GetByUniqueKeyDevices
    <DataMember> Public id As String = ""
    <DataMember> Public guid As String = ""
    <DataMember> Public deviceType As String = ""
    <DataMember> Public deviceId As String = ""
    <DataMember> Public name As String = ""
    <DataMember> Public lastUpdatedOn As String = ""
    <DataMember> Public eventCode As String = ""
    <DataMember> Public eventDate As String = ""
    <DataMember> Public latitude As String = ""
    <DataMember> Public longitude As String = ""
    <DataMember> Public speed As String = ""
    <DataMember> Public gpsStatus As String = ""
    <DataMember> Public address As String = ""
    <DataMember> Public serialNumber As String = ""
    <DataMember> Public carrier As String = ""
    <DataMember> Public simNumber As String = ""
    <DataMember> Public simPhone As String = ""
    <DataMember> Public isInactive As String = ""
    <DataMember> Public isRMA As String = ""
    <DataMember> Public isNotWorking As String = ""
    <DataMember> Public note As String = ""
    <DataMember> Public newDeviceCase As String = ""
    <DataMember> Public monthlyFee As String = ""

End Class
Public Class CRM_Customers_GetByUniqueKeyJSON
    <DataMember> Public ListCompanies As New List(Of CRM_Customers_GetByUniqueKeyCompanies)
    <DataMember> Public ListUsers As New List(Of CRM_Customers_GetByUniqueKeyUsers)
    <DataMember> Public ListDevices As New List(Of CRM_Customers_GetByUniqueKeyDevices)
End Class