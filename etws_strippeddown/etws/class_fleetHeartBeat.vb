Imports System.Runtime.Serialization

'$scope.fleet = {
'    fleetMiles: 0,
'    drivingHours: 0,
'    idleHours: 0,
'    mostActive: { name '', miles: 0 },
'    bestDriver: { name '', incidents: 0 },
'    worstDriver: { name: '', incidents: 0 }
'};

<DataContract()> Public Class fleetHeartBeat

    <DataMember> Public fleetMiles As Decimal = 0
    <DataMember> Public drivingHours As Decimal = 0
    <DataMember> Public idleHours As Decimal = 0
    <DataMember> Public mostActive As New deviceActivity
    <DataMember> Public bestDriver As New driverBehavior
    <DataMember> Public worstDriver As New driverBehavior

End Class

<DataContract()> Public Class deviceActivity

    <DataMember> Public name As String = ""
    <DataMember> Public miles As Decimal = 0

End Class

<DataContract()> Public Class driverBehavior

    <DataMember> Public name As String = ""
    <DataMember> Public incidents As Integer = 0

End Class