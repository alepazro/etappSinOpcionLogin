Imports System.Runtime.Serialization

Namespace crmEntities

    <DataContract> _
    Public Class user

        <DataMember> _
        Public isOk As Boolean = False

        <DataMember> _
        Public msg As String = ""

        <DataMember> _
        Public id As String = ""

        <DataMember> _
        Public dealerId As String = ""

        <DataMember> _
        Public token As String = ""

        <DataMember> _
        Public tokenCookie As String = ""

        <DataMember> _
        Public firstName As String = ""

        <DataMember> _
        Public fullName As String = ""

        <DataMember> _
        Public welcomeTitle As String = ""

        <DataMember> _
        Public isLimitedAccess As Boolean

        <DataMember> _
        Public chatLicense As String = ""

    End Class

    <DataContract()> _
    Public Class customer

        <DataMember> _
        Public id As String = ""

        <DataMember> _
        Public name As String = ""

        <DataMember> _
        Public phone As String = ""

        <DataMember> _
        Public email As String = ""

        <DataMember> _
        Public createdOn As String = ""

        <DataMember> _
        Public salesRepId As String = ""

        <DataMember> _
        Public salesRep As String = ""

        <DataMember> _
        Public isSuspended As String = ""

        <DataMember> _
        Public suspendedOn As String = ""

        <DataMember> _
        Public uniqueKey As String = ""

        <DataMember> _
        Public userLogin As String = ""

        <DataMember> _
        Public newCustomerCase As String = ""

        <DataMember> _
        Public totalUnits As Integer = 0

        <DataMember> _
        Public notInstalled As Integer = 0

        <DataMember> _
        Public workingUnits As Integer = 0

        <DataMember> _
        Public notWorkingUnits As Integer = 0

        <DataMember> _
        Public usersList As String

    End Class

End Namespace

