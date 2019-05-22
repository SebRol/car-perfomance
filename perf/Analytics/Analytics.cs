
namespace perf
{
   public static class Analytics
   {
      public const string PAGE_CALIBRATION                  = "Calibration";
      public const string PAGE_HELP                         = "Help";
      public const string PAGE_MAIN                         = "Main";
      public const string PAGE_PROFILE                      = "Profile";
      public const string PAGE_PROFILE_EDIT                 = "ProfileEdit";
      public const string PAGE_PURCHASE                     = "Purchase";
      public const string PAGE_PURCHASE_ERROR               = "PurchaseError";
      public const string PAGE_RESULT                       = "Result";
      public const string PAGE_RUN                          = "Run";
      public const string PAGE_SETUP                        = "Setup";
      public const string PAGE_CONFIG                       = "Configuration";
      public const string PAGE_TIPOFDAY                     = "TipOfDay";
      public const string PAGE_RUNADJUST                    = "RunAdjust";
      public const string PAGE_RUNADJUST_EDIT               = "RunAdjustEdit";
      public const string EVENT_CONFIG_LANGUAGE_ENGLISH     = "LanguageEnglish";
      public const string EVENT_CONFIG_LANGUAGE_GERMAN      = "LanguageGerman";
      public const string EVENT_CONFIG_SPEED_KPH            = "SpeedKph";
      public const string EVENT_CONFIG_SPEED_MPH            = "SpeedMph";
      public const string EVENT_CONFIG_DISTANCE_METER       = "DistanceMeter";
      public const string EVENT_CONFIG_DISTANCE_FEET        = "DistanceFeet";
      public const string EVENT_CONFIG_TIPOFDAY_ON          = "TipOfDayOn";
      public const string EVENT_CONFIG_TIPOFDAY_OFF         = "TipOfDayOff";
      public const string EVENT_CONFIG_FILTER_MEDIUM        = "FilterMedium";
      public const string EVENT_CONFIG_FILTER_STRONG        = "FilterStrong";
      public const string EVENT_CONFIG_FILTER_OFF           = "FilterOff";
      public const string EVENT_CONFIG_SOUND_ON             = "SoundOn";
      public const string EVENT_CONFIG_SOUND_OFF            = "SoundOff";
      public const string EVENT_PPROFILE_CREATE             = "Create";
      public const string EVENT_PPROFILE_EDIT               = "Edit";
      public const string EVENT_PPROFILE_VEHICLE            = "Vehicle";
      public const string EVENT_PPROFILE_COLOR              = "Color";
      public const string EVENT_PPROFILE_DELETE             = "Delete";
      public const string EVENT_PPROFILE_CALIBRATE          = "Calibrate";
      public const string EVENT_PURCHASE_REQUEST            = "Request";
      public const string EVENT_PURCHASE_SUCCESS            = "Success";
      public const string EVENT_PURCHASE_CANCEL             = "Cancel";
      public const string EVENT_RESULT_SPEED                = "Speed";
      public const string EVENT_RESULT_DISTANCE             = "Distance";
      public const string EVENT_RESULT_HEIGHT               = "Height";
      public const string EVENT_RESULT_SHARE                = "Share";
      public const string EVENT_RESULT_PREVIOUS             = "Previous";
      public const string EVENT_RESULT_NEXT                 = "Next";
      public const string EVENT_RESULT_DELETE               = "Delete";
      public const string EVENT_RUN_ACCELERATION_START      = "AccelerationStart";
      public const string EVENT_RUN_ACCELERATION_STOP       = "AccelerationStop";
      public const string EVENT_RUN_ACCELERATION_SPLIT      = "AccelerationSplit";
      public const string EVENT_RUN_ACCELERATION_RESET      = "AccelerationReset";
      public const string EVENT_RUN_ACCELERATION_DEMO       = "AccelerationDemo";
      public const string EVENT_RUN_BRAKE_START             = "BrakeStart";
      public const string EVENT_RUN_BRAKE_STOP              = "BrakeStop";
      public const string EVENT_RUN_BRAKE_SPLIT             = "BrakeSplit";
      public const string EVENT_RUN_BRAKE_RESET             = "BrakeReset";
      public const string EVENT_RUN_BRAKE_DEMO              = "BrakeDemo";
      public const string EVENT_RUN_ZEROTOZERO_START        = "ZeroToZeroStart";
      public const string EVENT_RUN_ZEROTOZERO_STOP         = "ZeroToZeroStop";
      public const string EVENT_RUN_ZEROTOZERO_SPLIT        = "ZeroToZeroSplit";
      public const string EVENT_RUN_ZEROTOZERO_RESET        = "ZeroToZeroReset";
      public const string EVENT_RUN_ZEROTOZERO_DEMO         = "ZeroToZeroDemo";
      public const string EVENT_SETUP_SWITCH_SPEED_DISTANCE = "SwitchSpeedDistance";
      public const string EVENT_SETUP_ADD                   = "Add";
      public const string EVENT_SETUP_REMOVE                = "Remove";
      public const string EVENT_SETUP_DEFAULT               = "Default";

      static IGoogleAnalytics current;

      public static IGoogleAnalytics Current
      {
         get
         {
            if (current == null)
            {
               current = new GoogleAnalyticsImplementation();

               current.Config.TrackingId     = "UA-12345678-9"; // this is not a real tracking id, this is to be replaced on release
               current.Config.AppName        = "CarPerformance";
               current.Config.UseSecure      = true;
               current.Config.Debug          = false;

               current.InitTracker();
            }

            return current;
         }
      }

      public static void TrackPage(string screenName)
      {
         Current.Tracker.SendView(screenName);
      }

      public static void TrackEventProfile(string action)
      {
         Current.Tracker.SendEvent("Profile", action);
      }

      public static void TrackEventResult(string action)
      {
         Current.Tracker.SendEvent("Result", action);
      }

      public static void TrackEventRun(string action)
      {
         Current.Tracker.SendEvent("Run", action);
      }

      public static void TrackEventTimeSplit(string action, string time, int speed)
      {
         Current.Tracker.SendEvent("Run", action, time, speed);
      }

      public static void TrackEventSetup(string action)
      {
         Current.Tracker.SendEvent("Setup", action);
      }

      public static void TrackEventConfig(string action)
      {
         Current.Tracker.SendEvent("Config", action);
      }

      public static void TrackLocation(string latitude, string longitute)
      {
         // 0.12   digits on gps coords -> 1.1 km    precision
         // 0.123  digits on gps coords -> 111 meter precision
         // 0.1234 digits on gps coords -> 11 meter  precision
         latitude  = Tools.RemoveDigits(latitude,  2); // 12.123456 -> 12.12
         longitute = Tools.RemoveDigits(longitute, 2);

         // "1" is the metric "GpsLocation", created in the google analytics "custom dimension" tab
         Current.Tracker.SetCustomDimension(1, latitude + "," + longitute);
      }

      public static void TrackPurchase(string action)
      {
         Current.Tracker.SendEvent("Purchase", action);
      }
   }
}
