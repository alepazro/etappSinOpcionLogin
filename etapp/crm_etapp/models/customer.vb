Imports System.Runtime.Serialization

<DataContract> _
Public Class companyNote

    <DataMember> _
    Public companyId As String = ""

    <DataMember> _
    Public createdBy As String = ""

    <DataMember> _
    Public createdOn As String = ""

    <DataMember> _
    Public note As String = ""

End Class

<DataContract()> _
Public Class CRM_Customer

    <DataMember()>
    Public id As String = ""

    <DataMember()> _
    Public firstName As String = ""

    <DataMember()> _
    Public lastName As String = ""

    <DataMember()> _
    Public email As String = ""

    <DataMember()> _
    Public phone As String = ""

    <DataMember()> _
    Public companyName As String = ""

    <DataMember()> _
    Public street As String = ""

    <DataMember()> _
    Public city As String = ""

    <DataMember()> _
    Public state As String = ""

    <DataMember()> _
    Public postalCode As String = ""

    <DataMember()> _
    Public isOk As Boolean = False

    <DataMember()> _
    Public billingContact As String = ""

    <DataMember()> _
    Public billingEmail As String = ""

    <DataMember()> _
    Public billingPhone As String = ""

    <DataMember()> _
    Public billingStreet As String = ""

    <DataMember()> _
    Public billingCity As String = ""

    <DataMember()> _
    Public billingState As String = ""

    <DataMember()> _
    Public billingPostalCode As String = ""

End Class

<DataContract()> _
Public Class crm_CustomerDetails

    <DataMember()> _
    Public isOk As Boolean = False

    <DataMember()> _
    Public id As String = ""

    <DataMember()> _
    Public paymentMethod As Integer

    <DataMember()> _
    Public billingDay As Integer

    <DataMember()> _
    Public ccNumber As String = ""

    <DataMember()> _
    Public ccExpMonth As Integer

    <DataMember()> _
    Public ccExpYear As Integer

    <DataMember()> _
    Public ccSecCode As String = ""

    <DataMember()> _
    Public ccFirstName As String = ""

    <DataMember()> _
    Public ccLastName As String = ""

    <DataMember()> _
    Public billingStreet As String = ""

    <DataMember()> _
    Public billingCity As String = ""

    <DataMember()> _
    Public billingState As String = ""

    <DataMember()> _
    Public billingPostalCode As String = ""

    <DataMember()> _
    Public IsVIP As Boolean = False

    <DataMember()> _
    Public lastCollectionNotificationOn As String = ""

    <DataMember()> _
    Public IsBillingInfoAudited As Boolean = False 'TRUE: Means that we went thru the details of billing and all parameters are valid.

End Class

<DataContract()> _
Public Class Company

    <DataMember()> _
    Public id As String = ""

    <DataMember()> _
    Public name As String = ""

    <DataMember()> _
    Public isSuspended As Boolean

    <DataMember()> _
    Public suspendedId As String

    <DataMember()> _
    Public suspendedOn As String = ""

End Class

<DataContract()> _
Public Class qbMatch

    <DataMember()> _
    Public crmCustomers As New List(Of crmCustomer)

    <DataMember()> _
    Public qbCustomers As New List(Of qbCustomer)

End Class

<DataContract()> _
Public Class crmCustomer

    <DataMember()> _
    Public id As String = ""

    <DataMember()> _
    Public name As String = ""

    <DataMember()> _
    Public qbId As String = ""

    <DataMember()> _
    Public dealerId As Integer = 0

    <DataMember()> _
    Public dealerName As String = ""

    <DataMember()> _
    Public isMatched As Boolean

End Class

<DataContract()> _
Public Class qbCustomer

    <DataMember()> _
    Public id As String = ""

    <DataMember()> _
    Public companyName As String = ""

    <DataMember()> _
    Public contactName As String = ""

End Class

<DataContract()> _
Public Class invoice

    <DataMember()> _
    Public invoiceNumber As String = ""

    <DataMember()> _
    Public invoiceDate As String = ""

    <DataMember()> _
    Public total As Decimal

    <DataMember()> _
    Public paid As Decimal

    <DataMember()> _
    Public balance As Decimal

End Class