using System;
using System.Globalization;
using System.Collections.Generic;
using Xamarin.Forms;

namespace perf
{
   class CalibrationEntry
   {
      public StackLayout    stack;
      public StackLayout    line;
      public Label          label;
      public CustomEntry    entry;
      public string         setting;
      public float          settingDefault;

      public CalibrationEntry(StackLayout stackLayout, string labelText, string setting, float settingDefault)
      {
         this.stack              = stackLayout;
         this.setting            = setting;
         this.settingDefault     = settingDefault;

         line                    = new StackLayout();
         line.Orientation        = StackOrientation.Horizontal;

         label                   = new Label();
         label.Text              = labelText;
         label.FontSize          = 17;
         label.HorizontalOptions = LayoutOptions.Start;
         label.VerticalOptions   = LayoutOptions.Center;

         entry                   = new CustomEntry();
         entry.Text              = Settings.GetValueOrDefault(setting, settingDefault).ToString();
         entry.BackgroundColor   = Color.Black;
         entry.WidthRequest      = 70;
         entry.Keyboard          = Keyboard.Numeric;
         entry.Completed        += OnEntryComplete;
         entry.Unfocused        += OnEntryComplete;
         entry.HorizontalOptions = LayoutOptions.EndAndExpand;

         line.Children.Add(label);
         line.Children.Add(entry);

         this.stack.Children.Add(line);

         if (label.Text.Equals(""))
         {
            label.Text = "- - - - - - - - - - - - - - - - -";
            label.HorizontalOptions = LayoutOptions.CenterAndExpand;
            entry.IsVisible = false;
         }

         label.SizeChanged += ((obj, sender) => this.stack.ForceLayout());
         entry.SizeChanged += ((obj, sender) => this.stack.ForceLayout());
      }

      public void OnEntryComplete(object sender, EventArgs args)
      {
         try
         {
            float f = float.Parse(entry.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            Settings.AddOrUpdateValue(setting, f);
         }
         catch (Exception e)
         {
            Debug.LogToFileMethod(e.ToString());
         }

         entry.Text = Settings.GetValueOrDefault(setting, settingDefault).ToString();
      }
   }

   public partial class PageDebugRunAdjust : ContentPage
   {
      static bool isPinAccepted = false; // remember access to calibration access for the lifetime of this app running, forget on app exit

      List<CalibrationEntry> calibrationEntries;
      Label                  label;
      CustomEntry            entry;

      public PageDebugRunAdjust()
      {
         InitializeComponent();

         if (isPinAccepted) ShowLayoutUnLocked(); // pin was entered once ok
         else               ShowLayoutLocked();   // ask for pin
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_RUNADJUST);

         Device.StartTimer(TimeSpan.FromMilliseconds(50), () => // Focus() is not reliable if called immediately
         {
            if (isPinAccepted == false) entry.Focus(); // pop up soft keyboard to enter pin

            return false; // stop timer
         });
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }
      
      void ShowLayoutLocked() // enable pin entry
      {
         Title = "Restricted Access";

         label                   = new Label();
         label.Text              = "\n\nEnter PIN";
         label.FontSize          = 40; // DO NOT CHANGE -> SECURITY PIN DEPENDS ON SIZE = 40
         label.HorizontalOptions = LayoutOptions.Center;
         label.VerticalOptions   = LayoutOptions.Center;

         entry                   = new CustomEntry();
         entry.FontSize          = 40;
         entry.WidthRequest      = 100;
         entry.Text              = " **** ";
         entry.BackgroundColor   = Color.Black;
         entry.Keyboard          = Keyboard.Numeric;
         entry.Completed        += OnEntryComplete;
         entry.Unfocused        += OnEntryComplete;
         entry.HorizontalOptions = LayoutOptions.Center;
         entry.VerticalOptions   = LayoutOptions.Center;

         layout.Children.Add(label);
         layout.Children.Add(entry);
      }

      void ShowLayoutUnLocked() // enable protected calibration access
      {
         Title = "Development Calibration";

         calibrationEntries = new List<CalibrationEntry>();

         calibrationEntries.Add(new CalibrationEntry(layout, "Launch Detection [factor]",                   Settings.LaunchDetection,             Settings.DefaultLaunchDetection));
         calibrationEntries.Add(new CalibrationEntry(layout, "Stand Still Detection [factor]",              Settings.StopDetection,               Settings.DefaultStopDetection));
         calibrationEntries.Add(new CalibrationEntry(layout, "Distance Filter [size]",                      Settings.DistanceFilter,              Settings.DefaultDistanceFilter));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Display Speed Adjust Gain [factor]",          Settings.SpeedAdjustDisplayGain,      Settings.DefaultSpeedAdjustDisplayGain));
         calibrationEntries.Add(new CalibrationEntry(layout, "Display Speed Adjust Shift X [factor]",       Settings.SpeedAdjustDisplayShiftX,    Settings.DefaultSpeedAdjustDisplayShiftX));
         calibrationEntries.Add(new CalibrationEntry(layout, "Display Speed Adjust Shift Y [factor]",       Settings.SpeedAdjustDisplayShiftY,    Settings.DefaultSpeedAdjustDisplayShiftY));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Acceleration Adjust Gain 1 [factor]",    Settings.AccelerationAdjustTimeGain1, Settings.DefaultAccelerationAdjustTimeGain1));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Acceleration Adjust Peak 1 [m/s^2]",     Settings.AccelerationAdjustTimePeak1, Settings.DefaultAccelerationAdjustTimePeak1));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Acceleration Adjust Gain 2 [factor]",    Settings.AccelerationAdjustTimeGain2, Settings.DefaultAccelerationAdjustTimeGain2));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Acceleration Adjust Peak 2 [m/s^2]",     Settings.AccelerationAdjustTimePeak2, Settings.DefaultAccelerationAdjustTimePeak2));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Acceleration Adjust Gain 3 [factor]",    Settings.AccelerationAdjustTimeGain3, Settings.DefaultAccelerationAdjustTimeGain3));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Acceleration Adjust Peak 3 [m/s^2]",     Settings.AccelerationAdjustTimePeak3, Settings.DefaultAccelerationAdjustTimePeak3));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time Brake Adjust Gain [factor]",             Settings.BrakeAdjustTimeGain,         Settings.DefaultBrakeAdjustTimeGain));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Time ZeroToZero Adjust Gain [factor]",        Settings.ZeroToZeroAdjustTimeGain,    Settings.DefaultZeroToZeroAdjustTimeGain));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Distance Adjust Acceleration Gain [factor]",  Settings.DistanceAdjustAccelGain,     Settings.DefaultDistanceAdjustAccelGain));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Distance Adjust Brake/Zero Gain [factor]",    Settings.DistanceAdjustBrakeGain,     Settings.DefaultDistanceAdjustBrakeGain));
         calibrationEntries.Add(new CalibrationEntry(layout, "", "", 0));
         calibrationEntries.Add(new CalibrationEntry(layout, "Raw Sensor Logging [0:Disable 1:Enable]",     Settings.RawSensorLogging,            Settings.DefaultRawSensorLogging));
      }

      void OnEntryComplete(object sender, EventArgs args) // check pin entry
      {
         try
         {
            int i = int.Parse(entry.Text, CultureInfo.InvariantCulture);

            if (i == (label.FontSize + 3) * 100 + (label.FontSize + 7)) // = 4347 = hex ascii = 0x43 0x47 = C G = Cem Gumus
            {
               isPinAccepted = true;           // remember access to calibration access for the lifetime of this app running
               Config.EnableCalibrationMode(); // show additional labels in PageResult and LivePeakA in PageRun
               ClearLayoutLocked();            // disable pin entry
               ShowLayoutUnLocked();           // enable protected calibration access

               Analytics.TrackPage(Analytics.PAGE_RUNADJUST_EDIT);
            }
         }
         catch (Exception e)
         {
            Debug.LogToFileMethod(e.ToString());
         }
      }

      void ClearLayoutLocked() // disable pin entry
      {
         // cannot remove ui elements from within event hanlder -> set size to zero
         label.Text          = "";
         label.FontSize      = 0;
         label.HeightRequest = 0;
         label.WidthRequest  = 0;
         entry.Text          = "";
         entry.FontSize      = 0;
         entry.HeightRequest = 0;
         entry.WidthRequest  = 0;
         entry.Completed    -= OnEntryComplete;
         entry.Unfocused    -= OnEntryComplete;
      }
   }
}
