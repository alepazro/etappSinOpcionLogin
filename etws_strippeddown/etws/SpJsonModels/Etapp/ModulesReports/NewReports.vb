Imports System.Runtime.Serialization

Public Class NewReports

    '<DataMember> Public ID As Integer = 0
    <DataMember> Public UserToken As String = ""
    <DataMember> Public HDevicesID As String = ""
    <DataMember> Public Name As String = ""
    <DataMember> Public EventDate As DateTime = Nothing
    <DataMember> Public SensorID As String = ""
    <DataMember> Public Temp As String = ""

End Class
Public Class JsonActivityDetailReport

    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public ThisDay As String = ""
    <DataMember> Public ThisTime As String = ""
    <DataMember> Public EventName As String = ""
    <DataMember> Public MaxSpeed As String = ""
    <DataMember> Public Duration As String = ""
    <DataMember> Public Meters As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public DriverName As String = ""

End Class
Public Class JsonDailyActivityReport

    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public IgnitionOn As String = ""
    <DataMember> Public Departed As String = ""
    <DataMember> Public Arrived As String = ""
    <DataMember> Public IgnitionOff As String = ""
    <DataMember> Public DurationMinutes As String = ""
    <DataMember> Public StoppedMinutes As String = ""
    <DataMember> Public Miles As String = ""
End Class
Public Class JsonReports_BasicEvents2

    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public EventName As String = ""
    <DataMember> Public Speed As String = ""
    <DataMember> Public Heading As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public Lat As String = ""
    <DataMember> Public Lng As String = ""

End Class

Public Class JsonReports_Idles

    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public IdleLocation As String = ""
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public IdleStart As String = ""
    <DataMember> Public IdleEnd As String = ""
    <DataMember> Public Duration As String = ""
    <DataMember> Public Latitude As String = ""
    <DataMember> Public Longitude As String = ""

End Class
Public Class JsonReports_Utilization_v2

    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public TotalMiles As String = ""
    <DataMember> Public TotalTravelTime As String = ""
    <DataMember> Public TotalIdlingTime As String = ""
    <DataMember> Public PorcInTransit As String = ""
    <DataMember> Public PorcIdle As String = ""
    <DataMember> Public PorcStopped As String = ""
    <DataMember> Public Speed00_35 As String = ""
    <DataMember> Public Speed36_65 As String = ""
    <DataMember> Public SpeedOver65 As String = ""
    <DataMember> Public SpeedingAlerts As String = ""
    <DataMember> Public IdleAlerts As String = ""
    <DataMember> Public QtyStops As String = ""

End Class
Public Class JsonReports_StateMiles
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public StateName As String = ""
    <DataMember> Public EventMiles As String = ""
End Class
Public Class JsonReports_StateMiles2
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public StateName As String = ""
    <DataMember> Public EventMiles As String = ""


End Class
Public Class JsonReports_DriverBehaviorLog
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DriverFirstName As String = ""
    <DataMember> Public DriverLastName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public EventName As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public Speed As String = ""
    <DataMember> Public miliGs As String = ""
End Class
Public Class JsonReports_Devices_InputsTimer
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public IgnTimer As String = ""
    <DataMember> Public SW1Timer As String = ""
    <DataMember> Public SW2Timer As String = ""
    <DataMember> Public SW3Timer As String = ""
    <DataMember> Public SW4Timer As String = ""
End Class
Public Class JsonReports_InputsTimersByHour
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public ThisDateTime As String = ""
    <DataMember> Public IgnTimer As String = ""
    <DataMember> Public SW1Timer As String = ""
    <DataMember> Public SW2Timer As String = ""
    <DataMember> Public SW3Timer As String = ""
    <DataMember> Public SW4Timer As String = ""

End Class
Public Class JsonReports_FuelLog
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public FuelDate As String = ""
    <DataMember> Public DeviceOdometer As String = ""
    <DataMember> Public Gallons As String = ""
    <DataMember> Public TotalCost As String = ""
    <DataMember> Public Address As String = ""
    <DataMember> Public PostalCode As String = ""
    <DataMember> Public StateProv As String = ""
    <DataMember> Public Comments As String = ""

End Class
Public Class JsonReports_DevicesNotWorking
    <DataMember> Public counter As String = ""
    <DataMember> Public name As String = ""
    <DataMember> Public LastUpdatedOn As String = ""

End Class
Public Class JsonReports_GeofencesVisits
    <DataMember> Public ReportName As String = ""
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public Arrival As String = ""
    <DataMember> Public Departure As String = ""
    <DataMember> Public Duration As String = ""

End Class
Public Class JsonReports_DevicesList
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceID As String = ""
    <DataMember> Public DeviceType As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public ServerDate As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public EventName As String = ""
    <DataMember> Public Speed As String = ""
    <DataMember> Public Heading As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public SerialNumber As String = ""
    <DataMember> Public GPSStatus As String = ""
    <DataMember> Public GPSAge As String = ""

End Class
Public Class JsonReports_DriverWorkHoursbyiButton
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DriverName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public ClockIn As String = ""
    <DataMember> Public ClockOut As String = ""
    <DataMember> Public TotalHours As String = ""
End Class
Public Class JsonReports_PointtoPoint
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public StartTime As String = ""
    <DataMember> Public StartAddress As String = ""
    <DataMember> Public StartGeofenceID As String = ""
    <DataMember> Public StopTime As String = ""
    <DataMember> Public StopAddress As String = ""
    <DataMember> Public StopGeofenceID As String = ""
End Class
Public Class JsonReports_Latitude_Longitude
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public EventName As String = ""
    <DataMember> Public Latitude As String = ""
    <DataMember> Public Longitude As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public GeofenceName As String = ""

End Class
Public Class JsonCompaniesDevicesEvents
    <DataMember> Public EventCode As String = ""
    <DataMember> Public Name As String = ""
    <DataMember> Public AlertTypeID As String = ""
    <DataMember> Public IsInternalEvent As String = ""
    <DataMember> Public IsDriverBehavior As String = ""
    <DataMember> Public IsInput As String = ""
    <DataMember> Public InputNum As String = ""
    <DataMember> Public IsPortExpander As String = ""
    <DataMember> Public EventDescription As String = ""
    <DataMember> Public IsMoving As String = ""
    <DataMember> Public IsStopped As String = ""
    <DataMember> Public IsHealthCheck As String = ""

End Class
Public Class JsonTimeCard
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public StartLocation As String = ""
    <DataMember> Public StartTime As String = ""
    <DataMember> Public EndLocation As String = ""
    <DataMember> Public EndTime As String = ""
    <DataMember> Public WorkMinutes As String = ""
End Class
Public Class JsonReports_ShortTrips
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public StartTime As String = ""
    <DataMember> Public StartAddress As String = ""
    <DataMember> Public StartGeofenceID As String = ""
    <DataMember> Public TripDurationSecs As String = ""
    <DataMember> Public TripDistanceMeters As String = ""
    <DataMember> Public TripMaxSpeed As String = ""
    <DataMember> Public StopTime As String = ""
    <DataMember> Public StopAddress As String = ""
    <DataMember> Public StopGeofenceID As String = ""
    <DataMember> Public StopDurationSecs As String = ""
End Class
Public Class JsonReports_Stops
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public StopAddress As String = ""
    <DataMember> Public TripDistanceMeters As String = ""
    <DataMember> Public TripMaxSpeed As String = ""
    <DataMember> Public TripDurationSecs As String = ""
    <DataMember> Public StopTime As String = ""
    <DataMember> Public NextStartTime As String = ""
    <DataMember> Public StopDurationSecs As String = ""
    <DataMember> Public StopLatitude As String = ""
    <DataMember> Public StopLongitude As String = ""
    <DataMember> Public StopGeofenceName As String = ""
End Class
Public Class JsonReports_DailyPerformance
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public TotalMiles As String = ""
    <DataMember> Public TotalTravelTime As String = ""
    <DataMember> Public TotalIdlingTime As String = ""
    <DataMember> Public PorcInTransit As String = ""
    <DataMember> Public PorcIdle As String = ""
    <DataMember> Public PorcStopped As String = ""
    <DataMember> Public Speed00_35 As String = ""
    <DataMember> Public Speed36_65 As String = ""
    <DataMember> Public SpeedOver65 As String = ""
    <DataMember> Public FirstIgnitionON As String = ""
    <DataMember> Public LastIgnitionOFF As String = ""
    <DataMember> Public SpeedingAlerts As String = ""
    <DataMember> Public IdleAlerts As String = ""
    <DataMember> Public QtyStops As String = ""
End Class
Public Class JsonGeoFencesAll
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public Id As String = ""
    <DataMember> Public Name As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public Latitude As String = ""
    <DataMember> Public Longitude As String = ""
    <DataMember> Public RadiusFeet As String = ""
    <DataMember> Public GeofenceAlertTypeName As String = ""
    <DataMember> Public GeofenceTypeName As String = ""
    <DataMember> Public ShapeID As String = ""
    <DataMember> Public jsonPolyVerticesTXT As String = ""
    <DataMember> Public IconURL As String = ""
    <DataMember> Public GeofenceInfoTable As String = ""
End Class
Public Class JsonGeofencesInOut
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public Arrival As String = ""
    <DataMember> Public Departure As String = ""
    <DataMember> Public Duration As String = ""
End Class
Public Class JsonReports_TRU
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public CompanyName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public ARBNumber As String = ""
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public GeofenceType As String = ""
    <DataMember> Public GeoTimeIn As String = ""
    <DataMember> Public GeoTimeOut As String = ""
    <DataMember> Public GeoMinutes As String = ""
    <DataMember> Public DieselInitMinutes As String = ""
    <DataMember> Public DieselEndMinutes As String = ""
    <DataMember> Public DieselMinutes As String = ""
    <DataMember> Public ElectricInitMinutes As String = ""
    <DataMember> Public ElectricEndMinutes As String = ""
    <DataMember> Public ElectricMinutes As String = ""
End Class
Public Class JsonReports_TRU_Log
    '<DataMember> Public ID As Integer = 0
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DateFrom As String = ""
    <DataMember> Public DateTo As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public Obj As String = ""
    <DataMember> Public EventType As String = ""
    <DataMember> Public EventDate As String = ""
    <DataMember> Public PartialAcumMinutes As String = ""
    <DataMember> Public AcumMinutes As String = ""
    <DataMember> Public DieselAcumMinutes As String = ""
    <DataMember> Public DieselStatus As String = ""
    <DataMember> Public ElectricAcumMinutes As String = ""
    <DataMember> Public ElectricStatus As String = ""
End Class
Public Class JsonGetDriver
    <DataMember> Public ID As String = ""
    <DataMember> Public Name As String = ""
    <DataMember> Public Phone As String = ""
    <DataMember> Public Email As String = ""
End Class
Public Class JsonReports_DriverLogSummary
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DriverName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public LogInTime As String = ""
    <DataMember> Public LogOutTime As String = ""
    <DataMember> Public Duration As String = ""
End Class
Public Class JsonReports_DriverLogDetailed
    <DataMember> Public ReportName As String = ""
    <DataMember> Public DriverName As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public StatusName As String = ""
    <DataMember> Public EventDate As String = ""
End Class
Public Class JsonReports_MultiDayTrail
    <DataMember> Public DeviceID As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public ActivityDate As String = ""
    <DataMember> Public FirstIgnitionON As String = ""
    <DataMember> Public AddressON As String = ""
    <DataMember> Public LastIgnitionOFF As String = ""
    <DataMember> Public AddressOFF As String = ""
    <DataMember> Public MilesDriven As String = ""
    <DataMember> Public HoursWorked As String = ""
End Class
'Public Class JsonReports_TemperatureLog_NEW
'    <DataMember> Public HDevicesID As String = ""
'    <DataMember> Public Name As String = ""
'    <DataMember> Public EventDate As String = ""
'    <DataMember> Public SensorID As String = ""
'    <DataMember> Public Temp As String = ""
'End Class
Public Class JsonReports_TemperatureLog_NEW
    <DataMember> Public HDevicesID As String = ""
    <DataMember> Public EventDate As DateTime
    <DataMember> Public Speed As String = ""
    <DataMember> Public FullAddress As String = ""
    <DataMember> Public Latitude As String = ""
    <DataMember> Public Longitude As String = ""
    <DataMember> Public Temperature1 As Decimal
    <DataMember> Public Temperature2 As Decimal
    <DataMember> Public Temperature3 As Decimal
    <DataMember> Public Temperature4 As Decimal
    <DataMember> Public GeofenceName As String = ""
    <DataMember> Public EventName As String = ""
End Class

Public Class JsonReports_TemperatureLog_NEW2
    '<DataMember> Public HDevicesID As String
    <DataMember> Public Name As String
    <DataMember> Public SensorID As String
    <DataMember> Public NameSensor As String
    <DataMember> Public EventDate As DateTime
    <DataMember> Public Temp As Decimal
    <DataMember> Public LightLevel As Decimal
    <DataMember> Public BatteryLevel As Decimal
    <DataMember> Public Humidity As Decimal
    <DataMember> Public Altitude As Decimal
    <DataMember> Public RSSI As Decimal
End Class

Public Class TroubleLog
    <DataMember> Public DeviceID As String = ""
    <DataMember> Public DeviceName As String = ""
    <DataMember> Public SerialNumber As String = ""
    <DataMember> Public LastUpdatedOn As String = ""
    <DataMember> Public NoShowDays As Integer = 0
    <DataMember> Public PowerCut As Integer = 0
    <DataMember> Public MainPowerRestored As Integer = 0
    <DataMember> Public IllegalPowerUp As Integer = 0
    <DataMember> Public PowerDown As Integer = 0
    <DataMember> Public IgnOffGPS15 As Integer = 0
    <DataMember> Public IgnOffSpeed10 As Integer = 0
    <DataMember> Public PowerUp As Integer = 0
    <DataMember> Public PowerOffBatt As Integer = 0
    <DataMember> Public TotalEvents As Integer = 0
End Class

