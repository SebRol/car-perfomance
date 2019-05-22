using System;

namespace perf
{
   // abstracts platform GPS function
   public interface IDeviceGps
   {
      // listener is called on new sensor data aveilable
      void SetListener(IGpsListener listener);

      // gps status
      int GetSatellitesVisible();
      int GetSatellitesVisibleWithFix();
      int GetSatellitesWithEphemeris();
      int GetSatellitesWithAlmanac();
      int GetTimeToFirstFix();

      // gps data
      double GetLatitude();
      double GetLongitude();

      // aaltitude in m
      double GetAltitude();

      // distance in m
      float GetDistance(double startLatitude, double startLongitude, double endLatitude, double endLongitude);

      // height in m
      float GetHeight(double startAltitude, double endAltitude);

      // velocity in m/s
      float GetSpeed();


      // debug info
      IDeviceGpsStatus GetStatus();

      // lifecyle management
      void Init();
      void DeInit();
   }

   public enum IDeviceGpsStatus
   {
      Init,
      Requested,
      ShutOff,
      Stopped,
      Started,
      FirstFix,
      Connected,
      Enabled,
      Disabled
   }
}
