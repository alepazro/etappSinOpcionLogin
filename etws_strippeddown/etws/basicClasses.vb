Imports System.Runtime.Serialization

<DataContract()> Public Class class_userHost
    <DataMember> Public IsOk As Boolean = True
    <DataMember> Public ip As String = ""
    <DataMember> Public userAgent As String = ""
    <DataMember> Public host As String = ""
    <DataMember> Public vId As Integer = 0 'VisitorId. Es creado para rastrear los visitantes de yaquiapp.  Cuando el visitante esta autenticado como usuario yaqui, se hace match de visitorId con userId. Pero si no es usuario o no esta autenticado, nos permite seguirlo en sus visitas.
    <DataMember> Public authToken As String = ""
End Class

<DataContract>
Public Class deviceCommand

    <DataMember>
    Public deviceId As String = ""

    <DataMember>
    Public cmdId As Integer = 0

    <DataMember>
    Public cmd As String = ""

End Class
<DataContract()>
Public Class idName

    <DataMember>
    Public parentId As String = ""

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public name As String

End Class

<DataContract> _
Public Class deviceResponse

    <DataMember> _
    Public responseDate As String = ""

    <DataMember> _
    Public responseData As String = ""

End Class

<DataContract> _
Public Class quickContact

    <DataMember> _
    Public formId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public phone As String = ""

End Class

<DataContract> _
Public Class contactForm

    <DataMember> _
    Public formId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public message As String = ""

End Class

<DataContract> _
Public Class smsReply

    <DataMember> _
    Public message As String = ""

    <DataMember> _
    Public [to] As String = ""

    <DataMember> _
    Public [from] As String = ""

End Class

Public Class hotSpots
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public lat As Decimal = 0
    Public lng As Decimal = 0
    Public address As String = ""
    Public qty As Integer = 0
    Public lastVisitOn As String = ""
End Class

Public Class notificationSendTo
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public entityId As String = ""
    Public sendToId As String = ""
    Public userId As String = ""
    Public firstName As String = ""
    Public lastName As String = ""
    Public fullName As String = ""
    Public isEmail As Boolean = False
    Public isSms As Boolean = False
End Class

Public Class userBasicInfo
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public id As String = ""
    Public firstName As String = ""
    Public lastName As String = ""
    Public fullName As String = ""
    Public email As String = ""
End Class

Public Class basicList
    Public id As String = ""
    Public value As String = ""
    Public name As String = ""
End Class

Public Class workZonesListItem
    Public workZoneId As String = ""
    Public name As String = ""
End Class

Public Class selectList
    Public value As String = ""
    Public text As String = ""
End Class

<DataContract> _
Public Class quoteForm

    <DataMember> _
    Public formId As String = ""

    <DataMember> _
    Public qty As Integer = 0

    <DataMember> _
    Public serviceId As Integer = 0

    <DataMember> _
    Public ship As Integer = 0

    <DataMember> _
    Public fn As String = ""

    <DataMember> _
    Public ln As String = ""

    <DataMember> _
    Public email As String = ""

    <DataMember> _
    Public ph As String = ""

    <DataMember> _
    Public co As String = ""

End Class

<DataContract> _
Public Class companyInfo

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public companyName As String = ""

    <DataMember> _
    Public phone As String = ""

    <DataMember> _
    Public website As String = ""

    <DataMember> _
    Public industry As String = ""

    <DataMember> _
    Public street As String = ""

    <DataMember> _
    Public city As String = ""

    <DataMember> _
    Public state As String = ""

    <DataMember> _
    Public postalCode As String = ""

    <DataMember> _
    Public countryCode As String = ""

    <DataMember> _
    Public lat As Decimal = 0

    <DataMember> _
    Public lng As Decimal = 0

    <DataMember> _
    Public createdOn As String = ""

End Class

<DataContract> _
Public Class companyInfo2

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public companyName As String = ""

    <DataMember> _
    Public firstName As String = ""

    <DataMember> _
    Public lastName As String = ""

    <DataMember> _
    Public token As String = ""

End Class

<DataContract> _
Public Class ccInfo

    <DataMember> _
    Public billingContact As String = ""

    <DataMember> _
    Public billingEmail As String = ""

    <DataMember> _
    Public billingPhone As String = ""

    <DataMember> _
    Public ccType As String = ""

    <DataMember> _
    Public ccNumber As String = ""

    <DataMember> _
    Public ccSecCode As String = ""

    <DataMember> _
    Public ccExpMonth As Integer = 0

    <DataMember> _
    Public ccExpYear As Integer = 0

    <DataMember> _
    Public ccFirstName As String = ""

    <DataMember> _
    Public ccLastName As String = ""

    <DataMember> _
    Public ccStreet As String = ""

    <DataMember> _
    Public ccCity As String = ""

    <DataMember> _
    Public ccState As String = ""

    <DataMember> _
    Public ccPostalCode As String = ""

    <DataMember> _
    Public ccCountryCode As String = ""

End Class

<DataContract> Public Class webForm
    <DataMember> Public formId As String = ""
    <DataMember> Public qty As Integer
    <DataMember> Public qtyGX As Integer
    <DataMember> Public qtyTF As Integer
    <DataMember> Public qtyOBDTracker As Integer
    <DataMember> Public qtyAssets As Integer
    <DataMember> Public serviceId As Integer
    <DataMember> Public isOBDOption As Boolean = False
    <DataMember> Public qtyOBDConnector As Integer
    <DataMember> Public isPostedSLOption As Boolean = False
    <DataMember> Public ship As Integer
    <DataMember> Public fn As String = ""
    <DataMember> Public ln As String = ""
    <DataMember> Public email As String = ""
    <DataMember> Public ph As String = ""
    <DataMember> Public cell As String = ""
    <DataMember> Public co As String = ""
    <DataMember> Public street As String = ""
    <DataMember> Public city As String = ""
    <DataMember> Public state As String = ""
    <DataMember> Public postalCode As String = ""
    <DataMember> Public ccType As String = ""
    <DataMember> Public ccNo As String = ""
    <DataMember> Public ccSec As String = ""
    <DataMember> Public ccMonth As Integer = 0
    <DataMember> Public ccYear As Integer = 0
    <DataMember> Public ccFn As String = ""
    <DataMember> Public ccLn As String = ""
    <DataMember> Public ccStreet As String = ""
    <DataMember> Public ccCity As String = ""
    <DataMember> Public ccState As String = ""
    <DataMember> Public ccPostal As String = ""
    <DataMember> Public promoCode As String = ""
    <DataMember> Public msg As String = ""
    <DataMember> Public repId As String = ""
    <DataMember> Public price As Decimal = 0
End Class

<DataContract> Public Class webForm_old

    <DataMember> Public formId As String = ""

    <DataMember>
    Public qty As Integer

    <DataMember>
    Public serviceId As Integer

    <DataMember>
    Public isOBDOption As Boolean = False

    <DataMember>
    Public isPostedSLOption As Boolean = False

    <DataMember>
    Public ship As Integer

    <DataMember>
    Public fn As String = ""

    <DataMember>
    Public ln As String = ""

    <DataMember>
    Public email As String = ""

    <DataMember>
    Public ph As String = ""

    <DataMember>
    Public cell As String = ""

    <DataMember>
    Public co As String = ""

    <DataMember>
    Public street As String = ""

    <DataMember>
    Public city As String = ""

    <DataMember>
    Public state As String = ""

    <DataMember>
    Public postalCode As String = ""

    <DataMember>
    Public ccType As String = ""

    <DataMember>
    Public ccNo As String = ""

    <DataMember>
    Public ccSec As String = ""

    <DataMember>
    Public ccMonth As Integer = 0

    <DataMember>
    Public ccYear As Integer = 0

    <DataMember>
    Public ccFn As String = ""

    <DataMember>
    Public ccLn As String = ""

    <DataMember>
    Public ccStreet As String = ""

    <DataMember>
    Public ccCity As String = ""

    <DataMember>
    Public ccState As String = ""

    <DataMember>
    Public ccPostal As String = ""

    <DataMember>
    Public promoCode As String = ""

    <DataMember>
    Public msg As String = ""

    <DataMember>
    Public repId As String = ""

    <DataMember>
    Public price As Decimal = 0

End Class

<DataContract> _
Public Class qtyDoc

    <DataMember> _
    Public qty As Integer = 0

    <DataMember> _
    Public shippingOption As Integer = 0

End Class

<DataContract>
Public Class priceList

    <DataMember>
    Public device As Decimal = 0

    <DataMember>
    Public service As Decimal = 0

End Class

<DataContract()> Public Class wsRequest
    <DataMember> Public ClientId As String = "" 'Nombre del programa que llama
End Class

<DataContract()> Public Class wsCamerasResponse
    <DataMember> Public IsOk As Boolean = True
    <DataMember> Public Msg As String = ""
    <DataMember> Public Cameras As New List(Of wsCamera)
End Class

<DataContract()> Public Class wsCamera
    <DataMember> Public SerialNumber As String = ""
    <DataMember> Public LastUpdatedOn As String = ""
End Class

<DataContract>
Public Class realTimeActivity

    <DataMember>
    Public qtyUnits As Integer

    <DataMember>
    Public onlineUsers As List(Of onlineUser)

End Class
<DataContract()>
Public Class onlineUser

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public companyName As String

    <DataMember>
    Public userName As String

    <DataMember>
    Public phone As String

    <DataMember>
    Public mobile As String

    <DataMember>
    Public email As String

    <DataMember>
    Public currentPage As String

    <DataMember>
    Public currentPageTime As String

    <DataMember>
    Public sessionTime As String

    <DataMember>
    Public qtyUnits As Integer

End Class
Public Class userProfile

    <DataMember>
    Public companyName As String = ""

    <DataMember>
    Public createdOn As String = ""

End Class
<DataContract()>
Public Class pendingOnBoarding

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public name As String

    <DataMember>
    Public email As String

    <DataMember>
    Public phone As String

    <DataMember>
    Public billCity As String

    <DataMember>
    Public billState As String

    <DataMember>
    Public shipCity As String

    <DataMember>
    Public shipState As String

    <DataMember>
    Public createdOn As String

End Class
<DataContract()>
Public Class onBoardingCustomer

    <DataMember>
    Public id As String = ""

    <DataMember>
    Public name As String

    <DataMember>
    Public email As String

    <DataMember>
    Public phone As String

    <DataMember>
    Public billCity As String

    <DataMember>
    Public billState As String

    <DataMember>
    Public shipCity As String

    <DataMember>
    Public shipState As String

    <DataMember>
    Public createdOn As String

End Class
<DataContract()>
Public Class resultOk

    <DataMember>
    Public isOk As Boolean

    <DataMember>
    Public msg As String = ""

End Class

