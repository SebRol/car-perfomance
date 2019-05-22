using System;
using Android.App;
using Android.Content;
using Android.Locations;

using perf.Droid;

// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DeviceGpsAndroid))]
namespace perf.Droid
{
   public class DeviceGpsAndroid : Activity, ILocationListener, IDeviceGps, GpsStatus.IListener
   {
      IGpsListener listener;
      LocationManager locationManager;
      GpsStatus gpsStatus;
      Location location;
      int gpsSatellitesVisible;
      int gpsSatellitesUsedInFix;
      int gpsSatellitesWithEphemeris;
      int gpsSatellitesWithAlmanac;
      IDeviceGpsStatus status = IDeviceGpsStatus.Init;

      public DeviceGpsAndroid()
      {
      }

      void InitSensor()
      {
         locationManager =  Application.Context.GetSystemService(Context.LocationService) as LocationManager;
         DeviceDebugAndroid.LogToFileStatic("gps sensor requested");

         if (locationManager != null)
         {
            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
               locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 0, 0, this);
               locationManager.AddGpsStatusListener(this);
               location = locationManager.GetLastKnownLocation(LocationManager.GpsProvider);

               status = IDeviceGpsStatus.Requested;
               DeviceDebugAndroid.LogToFileStatic("gps sensor init ok");
            }
            else
            {
               status = IDeviceGpsStatus.Disabled;
               DeviceDebugAndroid.LogToFileStatic("ERROR: gps sensor not enabled");
            }
         }
         else
         {
            status = IDeviceGpsStatus.Disabled;
            DeviceDebugAndroid.LogToFileStatic("ERROR: gps locationManager is null");
         }
      }

      public void OnLocationChanged(Location location) // ILocationListener
      {
         this.location = location;
         if (listener != null) listener.OnGpsLocationUpdate();

         DeviceDebugAndroid.LogToFileSensorGpsSpeedStatic((location != null) ? location.Speed : 0);
      }

      public void OnProviderDisabled(string provider) // ILocationListener
      {
         status = IDeviceGpsStatus.Disabled;
         DeviceDebugAndroid.LogToFileMethodStatic(status.ToString());
      }

      public void OnProviderEnabled(string provider) // ILocationListener
      {
         status = IDeviceGpsStatus.Enabled;
         DeviceDebugAndroid.LogToFileMethodStatic(status.ToString());
      }

      public void OnStatusChanged(string provider, Availability status, Android.OS.Bundle extras) // ILocationListener
      {
         if (status != Availability.Available)
         {
            this.status = IDeviceGpsStatus.Stopped;
         }

         //DeviceDebugAndroid.LogToFileMethodStatic(status.ToString());
      }

      public void OnGpsStatusChanged(GpsEvent e) // GpsStatus.IListener
      {
         if (locationManager != null)
         {
            gpsStatus = locationManager.GetGpsStatus(gpsStatus);

            switch (e)
            {
            case GpsEvent.FirstFix:
               status = IDeviceGpsStatus.FirstFix;
               DeviceDebugAndroid.LogToFileMethodStatic("gps: " + status.ToString());
               break;

            case GpsEvent.SatelliteStatus:
               if (gpsStatus != null)
               {
                  gpsSatellitesVisible = 0;
                  gpsSatellitesUsedInFix = 0;
                  gpsSatellitesWithEphemeris = 0;
                  gpsSatellitesWithAlmanac = 0;

                  var satellites = gpsStatus.Satellites;
                  if (satellites != null)
                  {
                     Java.Util.IIterator iterator = satellites.Iterator();
                     {
                        while (iterator.HasNext)
                        {
                           gpsSatellitesVisible++;

                           var s = (GpsSatellite)iterator.Next();
                           if (s.UsedInFix())  gpsSatellitesUsedInFix++;
                           if (s.HasEphemeris) gpsSatellitesWithEphemeris++;
                           if (s.HasAlmanac)   gpsSatellitesWithAlmanac++;
                        }
                     }

                     if (gpsSatellitesUsedInFix > 3)
                     {
                        status = IDeviceGpsStatus.Connected;
                     }
                  }
               }
               break;

            case GpsEvent.Started:
               status = IDeviceGpsStatus.Started;
               DeviceDebugAndroid.LogToFileMethodStatic("gps: " + status.ToString());
               break;

            case GpsEvent.Stopped:
               status = IDeviceGpsStatus.Stopped;
               DeviceDebugAndroid.LogToFileMethodStatic("gps: " + status.ToString());
               break;
            }

            if (listener != null) listener.OnGpsStatusUpdate();
         }
      }

      public void SetListener(IGpsListener listener) // IDeviceGps
      {
         this.listener = listener;
      }

      public int GetSatellitesVisible() // IDeviceGps
      {
         return gpsSatellitesVisible;
      }

      public int GetSatellitesVisibleWithFix() // IDeviceGps
      {
         return gpsSatellitesUsedInFix;
      }

      public int GetSatellitesWithEphemeris() // IDeviceGps
      {
         return gpsSatellitesWithEphemeris;
      }

      public int GetSatellitesWithAlmanac() // IDeviceGps
      {
         return gpsSatellitesWithAlmanac;
      }

      public int GetTimeToFirstFix() // IDeviceGps
      {
         return (gpsStatus != null) ? gpsStatus.TimeToFirstFix : 0;
      }

      public double GetLatitude() // IDeviceGps
      {
         return (location != null) ? location.Latitude : 0;
      }

      public double GetLongitude() // IDeviceGps
      {
         return (location != null) ? location.Longitude : 0;
      }

      public double GetAltitude() // IDeviceGps
      {
         return (location != null) ? location.Altitude : 0;
      }

      public float GetDistance(double startLatitude, double startLongitude, double endLatitude, double endLongitude) // IDeviceGps
      {
         float[] result = new float[3];

         Location.DistanceBetween(startLatitude, startLongitude, endLatitude, endLongitude, result);

         return result[0];
      }

      public float GetSpeed() // IDeviceGps
      {
         return (location != null) ? location.Speed : 0;
      }

      public float GetHeight(double startAltitude, double endAltitude) // IDeviceGps
      {
         return (float)(startAltitude - endAltitude);
      }

      public IDeviceGpsStatus GetStatus() // IDeviceGps
      {
         return status;
      }

      public void Init() // IDeviceGps
      {
         InitSensor();
      }

      public void DeInit() // IDeviceGps
      {
         if (locationManager != null)
         {  
            locationManager.RemoveUpdates(this);
            locationManager.RemoveGpsStatusListener(this);
            locationManager = null;
         }

         if (location != null)
         {
            location.Reset();
            location = null;
         }

         listener = null;
         gpsStatus = null;
         status = IDeviceGpsStatus.ShutOff;
      }
   }
}
