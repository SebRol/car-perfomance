using Xamarin.Forms;

namespace perf
{
   // for debug feature switches -> see Config.cs

   public static class Settings
   {
      public const string Language                        = "Language";
      public const string DistanceUnit                    = "DistanceUnit";
      public const string SpeedUnit                       = "SpeedUnit";
      public const string SpeedoFilter                    = "SpeedoFilter";
      public const string Sound                           = "Sound";
      public const string Disclaimer                      = "Disclaimer";
      public const string TipOfDay                        = "TipOfDay";
      public const string TipOfDayCount                   = "TipOfDayCount";
      public const string Purchase                        = "Purchase";

      // factor applied to noise from calibration -> limit for launch detection
      public const string LaunchDetection                 = "LaunchDetection";
      
      // factor applied to noise from calibration -> limit for stand still detection
      public const string StopDetection                   = "StopDetection";

      // size of the low pass filter for distance and height measurement
      public const string DistanceFilter                  = "DistanceFilter";


      // ADJUSTMENTS FOR TIME RESULTS // adjusted = ( gain / ( data + shiftX ) ) + ( data + shiftY )

      // calibration of time measurement (mode acceleration)
      // first set of calibration: applied if acceleration > peak 1
      public const string AccelerationAdjustTimeGain1     = "AccelerationAdjustTimeGain1";
      public const string AccelerationAdjustTimeShiftX1   = "AccelerationAdjustTimeShiftX1";
      public const string AccelerationAdjustTimeShiftY1   = "AccelerationAdjustTimeShiftY1";
      public const string AccelerationAdjustTimePeak1     = "AccelerationAdjustTimePeak1";

      // calibration of time measurement (mode acceleration)
      // second set of calibration: applied if acceleration > peak 2
      public const string AccelerationAdjustTimeGain2     = "AccelerationAdjustTimeGain2";
      public const string AccelerationAdjustTimeShiftX2   = "AccelerationAdjustTimeShiftX2";
      public const string AccelerationAdjustTimeShiftY2   = "AccelerationAdjustTimeShiftY2";
      public const string AccelerationAdjustTimePeak2     = "AccelerationAdjustTimePeak2";

      // calibration of time measurement (mode acceleration)
      // third set of calibration: applied if acceleration > peak 3
      public const string AccelerationAdjustTimeGain3     = "AccelerationAdjustTimeGain3";
      public const string AccelerationAdjustTimeShiftX3   = "AccelerationAdjustTimeShiftX3";
      public const string AccelerationAdjustTimeShiftY3   = "AccelerationAdjustTimeShiftY3";
      public const string AccelerationAdjustTimePeak3     = "AccelerationAdjustTimePeak3";

      // calibraion of time measurement (mode brake)
      public const string BrakeAdjustTimeGain             = "BrakeAdjustTimeGain";
      public const string BrakeAdjustTimeShiftX           = "BrakedjustTimeShiftX";
      public const string BrakeAdjustTimeShiftY           = "BrakeAdjustTimeShiftY";

      // calibraion of time measurement (mode zero to zero)
      public const string ZeroToZeroAdjustTimeGain        = "ZeroToZeroAdjustTimeGain";
      public const string ZeroToZeroAdjustTimeShiftX      = "ZeroToZeroAdjustTimeShiftX";
      public const string ZeroToZeroAdjustTimeShiftY      = "ZeroToZeroAdjustTimeShiftY";


      // ADJUSTMENTS FOR SPEEDO DISPLAY

      // calibration of speedo display
      public const string SpeedAdjustDisplayGain          = "SpeedAdjustDisplayGain";
      public const string SpeedAdjustDisplayShiftX        = "SpeedAdjustDisplayShiftX";
      public const string SpeedAdjustDisplayShiftY        = "SpeedAdjustDisplayShiftY";


      // ADJUSTMENTS FOR DISTANCE RESULTS

      // calibration of acceleration distance measurement
      public const string DistanceAdjustAccelGain         = "DistanceAdjustAccelGain";
      public const string DistanceAdjustAccelShiftX       = "DistanceAdjustAccelShiftX";
      public const string DistanceAdjustAccelShiftY       = "DistanceAdjustAccelShiftY";

      // calibration of brake distance measurement
      public const string DistanceAdjustBrakeGain         = "DistanceAdjustBrakeGain";
      public const string DistanceAdjustBrakeShiftX       = "DistanceAdjustBrakeShiftX";
      public const string DistanceAdjustBrakeShiftY       = "DistanceAdjustBrakeShiftY";


      // DEBUG AND TEST

      // raw sensor logging to file system [0:Disable 1:Enable]
      public const string RawSensorLogging                = "RawSensorLogging";


      // RESULT SETUP

      // user defined setup of result analysis
      public const string ResultSetupSpeed                = "ResultSetupSpeed";
      public const string ResultSetupDistance             = "ResultSetupDistance";
      public const string ResultSetupBrake                = "ResultSetupBrake";
      public const string ResultSetupZeroToZero           = "ResultSetupZeroToZero";


      // DEFAULTS

      public const float DefaultLaunchDetection                = 3.5f;   // factor applied to calibrated acceleration-noise, above this limit, vehicle is moving
      public const float DefaultStopDetection                  = 1.5f;   // factor applied to calibrated acceleration-noise, below this limit, vehicle is stand-still
      public const float DefaultDistanceFilter                 = 10;     // size of the low pass filter for distance and height measurement

      // time measurement: accelertion: first set of calibration: applied if acceleration > peak 1
      public const float DefaultAccelerationAdjustTimeGain1    = 1.0f;   // time measurement: attac gain applied to raw data
      public const float DefaultAccelerationAdjustTimeShiftX1  = 0.0f;   // time measurement: move data left-right
      public const float DefaultAccelerationAdjustTimeShiftY1  = 0.0f;   // time measurement: move data up-down
      public const float DefaultAccelerationAdjustTimePeak1    = 0.0f;   // time measurement: if peak-acceleration is above this limit -> use this set of calibration

      // time measurement: accelertion: second set of calibration: applied if acceleration > peak 2
      public const float DefaultAccelerationAdjustTimeGain2    = 0.97f;
      public const float DefaultAccelerationAdjustTimeShiftX2  = 0.0f;
      public const float DefaultAccelerationAdjustTimeShiftY2  = 0.0f;
      public const float DefaultAccelerationAdjustTimePeak2    = 3.5f;

      // time measurement: accelertion: third set of calibration: applied if acceleration > peak 3
      public const float DefaultAccelerationAdjustTimeGain3    = 0.95f;
      public const float DefaultAccelerationAdjustTimeShiftX3  = 0.0f;
      public const float DefaultAccelerationAdjustTimeShiftY3  = 0.0f;
      public const float DefaultAccelerationAdjustTimePeak3    = 4.8f;
      
      // time measurement: brake
      public const float DefaultBrakeAdjustTimeGain        = 1.1f;
      public const float DefaultBrakeAdjustTimeShiftX      = 0.0f;
      public const float DefaultBrakeAdjustTimeShiftY      = 0.0f;

      // time measurement: zero to zero
      public const float DefaultZeroToZeroAdjustTimeGain   = 0.5f;
      public const float DefaultZeroToZeroAdjustTimeShiftX = 0.0f;
      public const float DefaultZeroToZeroAdjustTimeShiftY = 0.0f;

      // speedo display
      public const float DefaultSpeedAdjustDisplayGain     = 0.5f;
      public const float DefaultSpeedAdjustDisplayShiftX   = 0.0f;
      public const float DefaultSpeedAdjustDisplayShiftY   = 0.0f;
      
      // distance measurement: acceleration
      public const float DefaultDistanceAdjustAccelGain    = 1.0f;
      public const float DefaultDistanceAdjustAccelShiftX  = 0.0f;
      public const float DefaultDistanceAdjustAccelShiftY  = 0.0f;

      // distance measurement: brake
      public const float DefaultDistanceAdjustBrakeGain    = 5.5f;
      public const float DefaultDistanceAdjustBrakeShiftX  = 0.0f;
      public const float DefaultDistanceAdjustBrakeShiftY  = 0.0f;

      // raw sensor logging to file system [0:Disable 1:Enable]
      public const float DefaultRawSensorLogging           = 0;


      static readonly ISettings settings;

      static Settings()
      {
         Debug.LogToFileMethod();
         // DependencyService decides if to use iOS or Android implementation
         settings = DependencyService.Get<ISettings>();
      }

      public static T GetValueOrDefault<T>(string key, T defaultValue = default(T))
      {
         return settings.GetValueOrDefault(key, defaultValue);
      }

      public static bool AddOrUpdateValue<T>(string key, T value)
      {
         return settings.AddOrUpdateValue(key, value);
      }

      public static void Remove(string key)
      {
         settings.Remove(key);
      }

      public static string GetDistanceUnit()
      {
         if (GetValueOrDefault(DistanceUnit, true))
         {
            return Localization.unitMeter;
         }
         else
         {
            return Localization.unitFeet;
         }
      }

      public static string GetSpeedUnit()
      {
         if (GetValueOrDefault(SpeedUnit, true))
         {
            return Localization.unitKph;
         }
         else
         {
            return Localization.unitMph;
         }
      }

      public static bool IsSpeedUnitKph()
      {
         if (GetValueOrDefault(SpeedUnit, true))
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      public static bool IsDistanceUnitMeter()
      {
         if (GetValueOrDefault(DistanceUnit, true))
         {
            return true;
         }
         else
         {
            return false;
         }
      }
   }
}
