using System;

namespace perf
{
   // feature and ui-elemetns enable/disable
   // for calibration and user preference -> see Settings.cs
   // for adjusting time measurement      -> see PageDebugRunAdjust.cs
   public static class Config
   {
      public static class Calibration
      {
         public static bool AlwaysSuccessEnabled    = false; // if true -> calibration cannot fail in PageCalibration
         public static bool LiveLabelEnabled        = false; // if true -> show live calibration value to user in PageCalibration
      }

      public static class Debug
      {
         public static bool LoggingEnabled          = false; // if true -> log files are created
         public static bool PagePurchaseEnabled     = false; // if true -> add page for in-app-purchase test
         public static bool PageRunAdjustEnabled    = true;  // if true -> add page for run adjust config
         public static bool PageSensorEnabled       = false; // if true -> add page for sensor test
      }

      public static class Main
      {
         public static bool ButtonRunAlwayEnabled   = true;  // if true -> user needs no gps to go from PageMain to PageRun
      }

      public static class Purchase
      {
         public static bool ConsumeOnExitEnabled    = false; // if true -> consume in-app-purchase on exit
         public static bool Enabled                 = true;  // if true -> user has to purchase function brake, zerotozero and result-setup
         public static bool ListenerLogEnabled      = false; // if true -> listeners of purchase events also get logging events
         public static PurchaseType ProductSku      = PurchaseType.Unlocker; // the in-app product to use for purchases
      }

      public static class Result
      {
         public static bool CalibrationLabelEnabled = false; // if true -> calibration curve and peak-a is visible in PageResults
         public static bool PlotSmoothingEnabled    = true;  // if true -> xy-plot line is smoothed in PageResults
         public static bool ShareDetailsEnabled     = false; // if true -> all measurement data is attached in share function in PageResults
      }

      public static class Run
      {
         public static bool DebugStartEnabled       = false; // if true -> button to start run with fake accelerometer is visible in PageRun
         public static bool DemoModeEnabled         = true;  // if true -> if no gps signal available -> fake accelerometer and gps sensor in PageRun
         public static bool PeakALabelEnabled       = false; // if true -> label with live peak-a value is visible in PageRun
         public static bool SplitTimeLabelEnabled   = false; // if true -> label with split times is visible in PageRun
         public static bool StatusLabelEnabled      = false; // if true -> label with status text is visible in PageRun
      }
      
      public static class Setup
      {
         public static bool DebugLabelEnabled       = false; // if true -> labels with debug info are visible in PageSetup
      }

      // setup config to enable configuration of RunAdjust
      public static void EnableCalibrationMode()
      {
         Debug.LoggingEnabled           = true;
         Result.CalibrationLabelEnabled = true;
         Result.ShareDetailsEnabled     = true;
         Run.PeakALabelEnabled          = true;
      }

      // setup config to enable debugging helpers
      public static void EnableDebugBuild()
      {
         Config.Debug.LoggingEnabled       = true;
         Config.Main.ButtonRunAlwayEnabled = true;
         Config.Purchase.Enabled           = false;
         Config.Result.ShareDetailsEnabled = true;
         Config.Run.DebugStartEnabled      = true;
         Config.Setup.DebugLabelEnabled    = true;
      }

      // check if configuration is ok for release on app store
      public static bool IsConfigForReleaseBuildValid()
      {
         bool result = true;

         if (Config.Calibration.AlwaysSuccessEnabled == true)                  result = false;
         if (Config.Calibration.LiveLabelEnabled     == true)                  result = false;
         if (Config.Debug.LoggingEnabled             == true)                  result = false;
         if (Config.Debug.PagePurchaseEnabled        == true)                  result = false;
         if (Config.Debug.PageRunAdjustEnabled       == false)                 result = false;
         if (Config.Debug.PageSensorEnabled          == true)                  result = false;
         if (Config.Main.ButtonRunAlwayEnabled       == false)                 result = false;
         if (Config.Purchase.ConsumeOnExitEnabled    == true)                  result = false;
         if (Config.Purchase.Enabled                 == false)                 result = false;
         if (Config.Purchase.ListenerLogEnabled      == true)                  result = false;
         if (Config.Purchase.ProductSku              != PurchaseType.Unlocker) result = false;
         if (Config.Result.CalibrationLabelEnabled   == true)                  result = false;
         if (Config.Result.PlotSmoothingEnabled      == false)                 result = false;
         if (Config.Result.ShareDetailsEnabled       == true)                  result = false;
         if (Config.Run.DebugStartEnabled            == true)                  result = false;
         if (Config.Run.DemoModeEnabled              == false)                 result = false;
         if (Config.Run.PeakALabelEnabled            == true)                  result = false;
         if (Config.Run.SplitTimeLabelEnabled        == true)                  result = false;
         if (Config.Run.StatusLabelEnabled           == true)                  result = false;
         if (Config.Setup.DebugLabelEnabled          == true)                  result = false;

         return result;
      }
   }
}
