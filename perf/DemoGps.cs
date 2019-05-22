using System;

namespace perf
{
   public class DemoGps : IDeviceGps
   {
      DemoAcceleromter demoAcceleromter;

      public DemoGps(DemoAcceleromter demoAcceleromter)
      {
         Debug.LogToFileMethod();
         Debug.LogToFile("gps sensor init (demo mode)");

         this.demoAcceleromter = demoAcceleromter;
      }

      public void SetListener(IGpsListener listener) // IDeviceGps
      {
         // ignore
      }

      public int GetSatellitesVisible() // IDeviceGps
      {
         return 0;
      }

      public int GetSatellitesVisibleWithFix() // IDeviceGps
      {
         return 0;
      }

      public int GetSatellitesWithEphemeris() // IDeviceGps
      {
         return 0;
      }

      public int GetSatellitesWithAlmanac() // IDeviceGps
      {
         return 0;
      }

      public int GetTimeToFirstFix() // IDeviceGps
      {
         return 0;
      }

      public double GetLatitude() // IDeviceGps
      {
         return 0;
      }

      public double GetLongitude() // IDeviceGps
      {
         return 0;
      }

      public double GetAltitude() // IDeviceGps
      {
         return 0;
      }

      public float GetDistance(double startLatitude, double startLongitude, double endLatitude, double endLongitude) // IDeviceGps
      {
         return demoAcceleromter.GetDistance();
      }

      public float GetSpeed() // IDeviceGps
      {
         return 0;
      }

      public float GetHeight(double startAltitude, double endAltitude) // IDeviceGps
      {
         return 0;
      }

      public IDeviceGpsStatus GetStatus() // IDeviceGps
      {
         return IDeviceGpsStatus.ShutOff;
      }

      public void Init() // IDeviceGps
      {
         // ignore
      }

      public void DeInit() // IDeviceGps
      {
         // ignore
      }
   }
}
