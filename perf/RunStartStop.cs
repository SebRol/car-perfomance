using System;
namespace perf
{
   public class RunStartStop
   {
      DataItemVehicle vehicle;
      float launchDetectionLimit;
      float launchDetectionFactor;
      float stopDetectionLimit;
      float stopDetectionFactor;

      public RunStartStop(DataItemVehicle itemVehicle)
      {
         Debug.LogToFileMethod();

         vehicle = itemVehicle;

         launchDetectionLimit  = 0;
         launchDetectionFactor = Settings.GetValueOrDefault(Settings.LaunchDetection, Settings.DefaultLaunchDetection);
         stopDetectionLimit    = 0;
         stopDetectionFactor   = Settings.GetValueOrDefault(Settings.StopDetection, Settings.DefaultStopDetection);

         if (vehicle.Calibration < PageCalibration.CALIBRATION_SUCCESS_LIMIT)
         {
            // profile was never calibrated -> use default
            launchDetectionLimit = 1 * launchDetectionFactor;
            stopDetectionLimit   = 1 * stopDetectionFactor;
         }
         else
         {
            // apply calibration to launch and stop detection
            launchDetectionLimit = vehicle.Calibration * launchDetectionFactor;
            stopDetectionLimit   = vehicle.Calibration * stopDetectionFactor;
         }
      }
      
      public float GetStartLimit()
      {
         return launchDetectionLimit;
      }

      public float GetStopLimit()
      {
         return stopDetectionLimit;
      }

      public string GetStartString()
      {
         return "launch detection [calibration, factor, limit]: " +
                    vehicle.Calibration.ToString("0.00")   + ", " +
                    launchDetectionFactor.ToString("0.00") + ", " +
                    launchDetectionLimit.ToString("0.00");
      }

      public string GetStopString()
      {
         return "stop detection [calibration, factor, limit]: " +
                    vehicle.Calibration.ToString("0.00") + ", " +
                    stopDetectionFactor.ToString("0.00") + ", " +
                    stopDetectionLimit.ToString("0.00");
      }
   }
}
