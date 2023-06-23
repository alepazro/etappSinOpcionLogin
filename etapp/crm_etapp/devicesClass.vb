Imports System.Runtime.Serialization

<DataContract()> _
Public Class deviceSettings

    <DataMember> _
    Public id As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public shortName As String = ""

    <DataMember> _
    Public textColor As String = ""

    <DataMember> _
    Public bgndColor As String = ""

    <DataMember> _
    Public lastUpdatedOn As String = ""

    <DataMember> _
    Public lastEventName As String = ""

    <DataMember> _
    Public lastEventOn As String = ""

    <DataMember> _
    Public eventCodeStartedOn As String = ""

    <DataMember> _
    Public driverId As String = ""

    <DataMember> _
    Public driverName As String = ""

    <DataMember> _
    Public idleLimit As Integer = 0

    <DataMember> _
    Public speedLimit As Integer = 0

    <DataMember> _
    Public odometer As Integer = 0

    <DataMember> _
    Public vin As String = ""

    <DataMember> _
    Public licensePlate As String = ""

    <DataMember> _
    Public serialNumber As String = ""

    <DataMember> _
    Public iconId As String = ""

    <DataMember> _
    Public iconURL As String = ""

    <DataMember> _
    Public iconLabelLine2 As String = ""

    <DataMember> _
    Public isARB As Boolean = False

    <DataMember> _
    Public arbNumber As String = ""

    <DataMember> _
    Public dieselMeter As Integer

    <DataMember> _
    Public electricMeter As Integer

    <DataMember> _
    Public isBuzzerOn As Boolean = False

    <DataMember> _
    Public deviceStatus As Integer = 0 '1: Active, 2: Sleeping, 3: Cancelled

    <DataMember> _
    Public sleepingSince As String = ""

    <DataMember> _
    Public cancelledSince As String = ""

    <DataMember> _
    Public assignedOn As String = ""

    <DataMember> _
    Public fuelCardUnitId As String = ""

    'make
    'model
    'modelYear
    'licensePlate
    'externalId
    'hasSpeedGauge

    'installedOn

    'isInactive
    'inactivationReasonId
    'inactivationOn

    'isRMA
    'rmaDate
    'rmaReplacementId

    'isNotWorking
    'notWorkingDetectedOn
    'notWorkingLastNotifiedOn

    'monthlyFee

    'isPowerCut
    'lastPowerCutOn

    'lastGoodGPSOn


End Class

Public Class device
    Public panelId As Integer = 0
    Public token As String = ""
    Public isOk As Boolean = False
    Public msg As String = ""
    Public id As String = ""
    Public name As String = ""
    Public eventCode As String = ""
    Public eventName As String = ""
    Public eventDate As String = ""
    Public lastUpdatedOn As String = ""
    Public address As String = ""
    Public speed As String = ""
    Public heading As String = ""
    Public latitude As Decimal = 0
    Public longitude As Decimal = 0
    Public iconUrl As String = ""
    Public driverName As String = ""
    Public hasInputs As Boolean = False
    Public hasPortExpander As Boolean = False
    Public sw1 As String = "-"
    Public sw2 As String = "-"
    Public sw3 As String = "-"
    Public sw4 As String = "-"
    Public pto1 As String = "-"
    Public pto2 As String = "-"
    Public pto3 As String = "-"
    Public pto4 As String = "-"
    Public pto5 As String = "-"
    Public pto6 As String = "-"
    Public pto7 As String = "-"
    Public pto8 As String = "-"
    Public temp1 As String = ""
    Public temp2 As String = ""
    Public temp3 As String = ""
    Public temp4 As String = ""
    Public relay1 As String = ""
    Public relay2 As String = ""
    Public relay3 As String = ""
    Public relay4 As String = ""

    Public infoTable As String = ""
End Class

<DataContract()> _
Public Class searchDeviceResult

    <DataMember> _
    Public customerGUID As String = ""

    <DataMember> _
    Public customerName As String = ""

    <DataMember> _
    Public deviceGUID As String = ""

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public deviceName As String = ""

    <DataMember> _
    Public serialNumber As String = ""

    <DataMember> _
    Public imei As String = ""

    <DataMember> _
    Public simCarrier As String = ""

    <DataMember> _
    Public simNo As String = ""

    <DataMember> _
    Public simAreaCode As String = ""

    <DataMember> _
    Public simPhoneNumber As String = ""

    <DataMember> _
    Public lastUpdatedOn As String = ""

    <DataMember> _
    Public eventDate As String = ""

    <DataMember> _
    Public eventCode As String = ""

    <DataMember> _
    Public gpsAge As Integer = 0

    <DataMember> _
    Public lastGoodGpsOn As String = ""

    <DataMember> _
    Public hasSpeedGauge As Boolean = False

    <DataMember> _
    Public sgStatus As String = ""

    <DataMember> _
    Public sgFee As Decimal = 0

    <DataMember> _
    Public isNotWorking As Boolean = False

    <DataMember> _
    Public notWorkingSince As String = ""

    <DataMember> _
    Public isInactive As Boolean = False

    <DataMember> _
    Public inactiveReason As String = ""

    <DataMember> _
    Public isRMA As Boolean = False

End Class

<DataContract()> _
Public Class devIOs

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public name As String = ""

    'INPUTS
    '============================================
    <DataMember> _
    Public inputName1 As String = ""

    <DataMember> _
    Public inputOn1Name As String = ""

    <DataMember> _
    Public inputOff1Name As String = ""

    <DataMember> _
    Public inputStatus1 As Boolean = False

    <DataMember> _
    Public inputName2 As String = ""

    <DataMember> _
    Public inputOn2Name As String = ""

    <DataMember> _
    Public inputOff2Name As String = ""

    <DataMember> _
    Public inputStatus2 As Boolean = False

    <DataMember> _
    Public inputName3 As String = ""

    <DataMember> _
    Public inputOn3Name As String = ""

    <DataMember> _
    Public inputOff3Name As String = ""

    <DataMember> _
    Public inputStatus3 As Boolean = False

    <DataMember> _
    Public inputName4 As String = ""

    <DataMember>
    Public inputOn4Name As String = ""

    <DataMember> _
    Public inputOff4Name As String = ""

    <DataMember> _
    Public inputStatus4 As Boolean = False

    'OUTPUTS
    '============================================
    <DataMember> _
    Public name1 As String = ""

    <DataMember> _
    Public on1Name As String = ""

    <DataMember> _
    Public off1Name As String = ""

    <DataMember> _
    Public status1 As Boolean = False

    <DataMember> _
    Public name2 As String = ""

    <DataMember> _
    Public on2Name As String = ""

    <DataMember> _
    Public off2Name As String = ""

    <DataMember> _
    Public status2 As Boolean = False

    <DataMember> _
    Public name3 As String = ""

    <DataMember> _
    Public on3Name As String = ""

    <DataMember> _
    Public off3Name As String = ""

    <DataMember> _
    Public status3 As Boolean = False

    <DataMember> _
    Public name4 As String = ""

    <DataMember> _
    Public on4Name As String = ""

    <DataMember> _
    Public off4Name As String = ""

    <DataMember> _
    Public status4 As Boolean = False

End Class

<DataContract()> _
Public Class setRelay

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public relayNum As Integer

    <DataMember> _
    Public newStatus As Boolean = False

End Class

<DataContract()> _
Public Class ioSetUp

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public ioType As Integer '1: Input, 2: Output

    <DataMember> _
    Public ioNum As Integer

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public onStatus As String = ""

    <DataMember> _
    Public offStatus As String = ""

    <DataMember> _
    Public isAll As Boolean = False

End Class

<DataContract()> _
Public Class devInputsOnTime

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public ignitionOnTime As Decimal

    <DataMember> _
    Public ignitionLastSetOn As String = ""

    <DataMember> _
    Public input1Name As String = ""

    <DataMember> _
    Public input1OnTime As Decimal

    <DataMember> _
    Public input1LastSetOn As String = ""

    <DataMember> _
    Public input2Name As String = ""

    <DataMember> _
    Public input2OnTime As Decimal

    <DataMember> _
    Public input2LastSetOn As String = ""

    <DataMember> _
    Public input3Name As String = ""

    <DataMember> _
    Public input3OnTime As Decimal

    <DataMember> _
    Public input3LastSetOn As String = ""

    <DataMember> _
    Public input4Name As String = ""

    <DataMember> _
    Public input4OnTime As Decimal

    <DataMember> _
    Public input4LastSetOn As String = ""

End Class

<DataContract()> _
Public Class devInputsOnTimeTransformed

    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public ignitionOnTime As Integer

    <DataMember> _
    Public ignitionSetOn As DateTime

    <DataMember> _
    Public input1OnTime As Integer

    <DataMember> _
    Public input1SetOn As DateTime

    <DataMember> _
    Public input2OnTime As Integer

    <DataMember> _
    Public input2SetOn As DateTime

    <DataMember> _
    Public input3OnTime As Integer

    <DataMember> _
    Public input3SetOn As DateTime

    <DataMember> _
    Public input4OnTime As Integer

    <DataMember> _
    Public input4SetOn As DateTime

End Class

<DataContract()> _
Public Class devAction

    <DataMember> _
    Public action As String

    <DataMember> _
    Public id As String

    <DataMember> _
    Public param1 As String

    <DataMember> _
    Public param2 As String

End Class

<DataContract()> _
Public Class callResult

    <DataMember> _
    Public isOk As Boolean

    <DataMember> _
    Public msg As String

End Class

<DataContract()> _
Public Class deviceInfoWindow


    <DataMember> _
    Public deviceId As String = ""

    <DataMember> _
    Public name As String = ""

    <DataMember> _
    Public fullAddress As String = ""

    <DataMember> _
    Public eventCode As String = ""

    <DataMember> _
    Public eventName As String = ""

    <DataMember> _
    Public eventDateString As String = ""

    <DataMember> _
    Public eventCodeStartedOnString As String = ""

    <DataMember> _
    Public speed As Integer = 0

    <DataMember> _
    Public heading As String = ""

    <DataMember> _
    Public eventDate As Date

    <DataMember> _
    Public latitude As Decimal

    <DataMember> _
    Public longitude As Decimal

    <DataMember> _
    Public isPowerCut As Boolean

    <DataMember> _
    Public isBadIgnitionInstall As Boolean

    <DataMember> _
    Public isNotWorking As Boolean

    <DataMember> _
    Public gpsAge As Integer

    <DataMember> _
    Public gpsAgeAlert As Integer

    <DataMember> _
    Public driverName As String = ""

    <DataMember> _
    Public iconUrl As String = ""

    <DataMember> _
    Public infoTable As String = ""

End Class

