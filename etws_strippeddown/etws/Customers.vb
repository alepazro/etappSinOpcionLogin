Imports System.Runtime.Serialization

<DataContract> _
Public Class Customer

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public salesTaxId As String = ""

    <DataMember> _
    Public adCampaignId As String = ""

    <DataMember> _
    Public customerTypeId As String = ""

    <DataMember> _
    Public paymentTermsId As String = ""

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

'custId:/custId/, name:/name/, street: /street/, city: /city/, state: /state/, postalCode: /postalCode/, country: /country/, notes: /notes/, createdOn: /createdOn/ 

<DataContract> _
Public Class Customer2

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public street As String = ""

    <DataMember> _
    Public city As String = ""

    <DataMember> _
    Public state As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public country As String = ""

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

'{id:/id/, name:/name/, street:/street/, city: /city/, state: /state/, postalCode: /postalCode/, country: /country/, contactName: /contactName/, email: /email/, phone: /phone/, lat: /latitude/, lng: /longitude/}
<DataContract> _
Public Class Customer3

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public street As String = ""

    <DataMember> _
    Public city As String = ""

    <DataMember> _
    Public state As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public country As String = ""

    <DataMember> _
    Public contactName As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public lat As Decimal = 0

    <DataMember> _
    Public lng As Decimal = 0

End Class

<DataContract> _
Public Class jobCustomer

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public street As String = ""

    <DataMember> _
    Public streetNumber As String = ""

    <DataMember> _
    Public route As String = ""

    <DataMember> _
    Public city As String = ""

    <DataMember> _
    Public county As String = ""

    <DataMember> _
    Public state As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public country As String = ""

    <DataMember> _
    Public fullAddress As String = ""

    <DataMember> _
    Public lat As Decimal = 0

    <DataMember> _
    Public lng As Decimal = 0

    <DataMember> _
    Public contactName As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public workZoneId As String = ""

    <DataMember> _
    Public notes As String = ""

    <DataMember> _
    Public createdOn As String = ""

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public sysMsg As String = ""

End Class

<DataContract> _
Public Class CustomerIdName

    <DataMember> _
    Public value As String = ""

    <DataMember> _
    Public text As String = ""

    <DataMember> _
    Public workZoneId As String = ""

End Class

<DataContract> _
Public Class CustLocation

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public locationId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public address1 As String = ""

    <DataMember> _
    Public address2 As String = ""

    <DataMember> _
    Public city As String = ""

    <DataMember> _
    Public stateId As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

<DataContract> _
Public Class CustContact

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public locId As String = ""

    <DataMember> _
    Public contactId As String = ""

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public altPhone As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public isPointOfContact As String = ""

    <DataMember> _
    Public comment As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

'{custId:/custId/, contactId: /contactId/, firstName: /firstName/, lastName: /lastName/, phone: /phone/,  email: /email/, isPrimary: /isPrimary/, createdOn: /createdOn/ }

<DataContract> _
Public Class CustContact2

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public contactId As String = ""

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public isPrimary As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class

<DataContract> _
Public Class CustAsset

    <DataMember> _
    Public isOk As Boolean = True

    <DataMember> _
    Public msg As String = ""

    <DataMember> _
    Public token As String = ""

    <DataMember> _
    Public custId As String = ""

    <DataMember> _
    Public locId As String = ""

    <DataMember> _
    Public assetId As String = ""

    <DataMember> _
    Public assetTypeId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public description As String = ""

    <DataMember> _
    Public manufacturer As String = ""

    <DataMember> _
    Public model As String = ""

    <DataMember> _
    Public serialNumber As String = ""

    <DataMember> _
    Public locationArea As String = ""

    <DataMember> _
    Public locationSubArea As String = ""

    <DataMember> _
    Public locationSpot As String = ""

    <DataMember> _
    Public createdOn As String = ""

End Class
