using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageRun : ContentPage, IGpsListener, IAcceleromterListener
   {
      const uint   SPEEDO_UPDATE_RATE_MS = 50;       // milliseconds between two calls to update speedo
      const int    SETTLE_COUNT_LIMIT    = 89;       // wait some time after appearance
      const double ROTATION_SOURCE_MIN   = 0.0;      // m/s
      const double ROTATION_SOURCE_MAX   = 88.88;    // m/s -> 320 km/h
      const double ROTATION_TARGET_MIN   = -180.0;   // rotation degree, 0 is up, +180 is down, -90 is left, +90 is right
      const double ROTATION_TARGET_MAX   =  +90.0;   // rotation degree, 0 is up, +180 is down, -90 is left, +90 is right
      const long   GPS_TIMEOUT           = 50000000; // 10 million ticks in a second -> 5 seconds

      IDeviceGps           gpsProvider;
      IDeviceAcceleromter  accelerometerProvider;
      IRunMode             runModeProvider;
      PageMain             pageNavigation;
      RunAdjust            runAdjust;

      Image                imgSpeedo;
      Image                imgNeedle;
      Image                imgDistanceOff;
      Image                imgDistanceQuarter;
      Image                imgDistanceHalf;
      CustomLabel          labelSpeed;
      CustomLabel          labelUnit;
      CustomLabel          labelMode;
      CustomLabel          labelSplitTime;
      CustomLabel          labelStatus;
      CustomLabel          labelDemoMode;

      Image                imgGps;
      Button               btnAbort;
      Button               btnStopVehicle;

      float                accelerationRaw;
      float                speedRaw;
      float                speedAdjustedForDisplay;
      float                filteredSpeedForDisplay;
      float                filteredSpeedForDisplayWithUnit;
      double               needleAngleDegree;

      int                  settleCounter;
      long                 timeStampLastGps;
      RunData              runData;

      MovingAverage        movingAverageForDisplay;

      bool                 isRunning;
      bool                 isLaunchDetected;
      bool                 isGpsOk;
      bool                 isUnitKmh;
      bool                 isAccelerationMode;
      bool                 isDemoMode;

      // ///////////////////////////////////////////////////////////////////////////////////////////
      // debug and test
      Label                dbgLabelAccel;
      Label                dbgLabelAccelPeak;
      float                dbgAccel;
      float                dbgAccelPeak;
      // ///////////////////////////////////////////////////////////////////////////////////////////

      public PageRun(IRunMode runMode, IDeviceGps gps, IDeviceAcceleromter accelerometer, PageMain parentPage, bool demoMode)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         gpsProvider           = gps;
         accelerometerProvider = accelerometer;
         runModeProvider       = runMode;
         pageNavigation        = parentPage;
         isDemoMode            = demoMode;

         var speedoFilter = Settings.GetValueOrDefault(Settings.SpeedoFilter, 1);
         var filterSize   = speedoFilter * 10;
         Debug.LogToFile("filter mode: " + Tools.ToFilterMode(speedoFilter));

         // default = 1*10 = 10 samples = medium // 0 = off // strong = 2*10 = 20 samples
         movingAverageForDisplay = new MovingAverage(filterSize);

         isAccelerationMode = runModeProvider.Mode.Equals(RunModeAcceleration.Mode);

         InitLayout();
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Analytics.TrackPage(Analytics.PAGE_RUN);

         isRunning          = true; // workaround for xamarin.com Bug 35407 - jobject muste not be IntPtr.Zero with Animation
         isLaunchDetected   = false;
         isGpsOk            = false;
         isUnitKmh          = Settings.IsSpeedUnitKph();
         settleCounter      = SETTLE_COUNT_LIMIT;
         timeStampLastGps   = DateTime.Now.Ticks;
         runAdjust          = new RunAdjust();
         runData            = new RunData(runModeProvider.Mode);

         Debug.LogToFile(runModeProvider.ToString());

         Device.StartTimer(TimeSpan.FromMilliseconds(SPEEDO_UPDATE_RATE_MS), OnTimer);

         gpsProvider.SetListener(this);
         accelerometerProvider.SetListener(this);
      }

      protected override void OnDisappearing()
      {
         isRunning = false; // workaround for xamarin.com Bug 35407 - jobject muste not be IntPtr.Zero with Animation
         runData.Store();

         Debug.LogToFileFlush();
         base.OnDisappearing();
      }

      public void OnSleep()
      {
         if (accelerometerProvider != null) accelerometerProvider.SetListener(null);
         if (gpsProvider != null)           gpsProvider.SetListener(null);
      }

      public void OnResume()
      {
         if (accelerometerProvider != null) accelerometerProvider.SetListener(this);
         if (gpsProvider != null)           gpsProvider.SetListener(this);

         SetSplitTimeText();
         SetStatusText(Localization.pageRunStatusResume);
      }

      async void OnButtonAbort(object sender, EventArgs args)
      {
         btnAbort.IsEnabled = false;

         if (IsResultAvailable() == false)
         {
            // the button text is "abort" -> navigate back
            await Navigation.PopAsync();
         }
         else
         {
            // the button text is "results" -> present results
            await Navigation.PopAsync();
            pageNavigation.ShowPageResults();
         }

         btnAbort.IsEnabled = true;
      }

      void InitLayout()
      {
         // gps icon

         imgGps = new Image();
         if (isDemoMode) imgGps.Source = "@drawable/icn_gps_no.png"; // if demo mode -> gps is always off
         else            imgGps.Source = "@drawable/icn_gps_yes.png";
         layout.Children.Add
         (
            imgGps,
            Constraint.RelativeToParent((parent) => parent.Width - imgGps.Width * 1.1),
            Constraint.RelativeToParent((parent) => parent.Y)
         );
         imgGps.SizeChanged += ((obj, sender) => layout.ForceLayout());

         // test mode

         labelMode = new CustomLabel();
         labelMode.Size = CustomLabel.SIZE_CAPTION;
         labelMode.TextColor = Color.FromUint(runModeProvider.Color);
         labelMode.Text = runModeProvider.ModeText;
         //labelMode.SizeChanged += ((obj, sender) => layout.ForceLayout());
         layout.Children.Add
         (
            labelMode,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - labelMode.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.06)
         );
         
         // ///////////////////////////////////////////////////////////////////////////////////////////
         // debug and test
         if (Config.Run.DebugStartEnabled)
         {
            Button btnDebugStart = new Button();
            btnDebugStart.Text = "DebugStart";
            btnDebugStart.Clicked += (sender, e) =>
            {
               DemoAcceleromter demoAccelerometerProvider = accelerometerProvider as DemoAcceleromter;

               if (demoAccelerometerProvider != null) demoAccelerometerProvider.OnAcceleromterLaunchDetected();
               else                                   OnAcceleromterLaunchDetected(); // cast was not successful -> we have a real sensor
            };

            layout.Children.Add
            (
               btnDebugStart,
               Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnDebugStart.Width * 0.5),
               Constraint.RelativeToParent((parent) => parent.Height * 0.1)
            );
         }
         // ///////////////////////////////////////////////////////////////////////////////////////////

         // speedo

         imgSpeedo = new Image();
         imgSpeedo.Source = runModeProvider.Speedo;
         imgSpeedo.Scale = 0.85;
         imgSpeedo.SizeChanged += OnSpeedoBackgroundResize;
         layout.Children.Add
         (
            imgSpeedo,
            Constraint.RelativeToParent((parent) => parent.X),
            Constraint.RelativeToParent((parent) => parent.Y),
            Constraint.RelativeToParent((parent) => parent.Width),
            Constraint.RelativeToParent((parent) => parent.Height)
         );

         // distance indicator

         if (isDemoMode)
         {
            // replace distance indicator in demo mode
            labelDemoMode = new CustomLabel();
            labelDemoMode.Size = CustomLabel.SIZE_CAPTION_DE;
            labelDemoMode.Text = "Demo Mode";
            labelDemoMode.TextColor = Color.FromUint(runModeProvider.Color);
            layout.Children.Add
            (
               labelDemoMode,
               Constraint.RelativeToParent((parent) => parent.Width * 0.5 - labelDemoMode.Width * 0.5),
               Constraint.RelativeToParent((parent) => parent.Height * 0.4)
            );
         }
         else
         {
            if (isAccelerationMode)
            {
               imgDistanceOff = new Image();
               imgDistanceOff.Source = "@drawable/display_distance1.png";
               layout.Children.Add
               (
                  imgDistanceOff,
                  Constraint.RelativeToParent((parent) => parent.Width * 0.5 - imgDistanceOff.Width * 0.5),
                  Constraint.RelativeToParent((parent) => parent.Height * 0.36)
               );
               imgDistanceOff.IsVisible = true;

               imgDistanceQuarter = new Image();
               imgDistanceQuarter.Source = "@drawable/display_distance2.png";
               layout.Children.Add
               (
                  imgDistanceQuarter,
                  Constraint.RelativeToParent((parent) => parent.Width * 0.5 - imgDistanceQuarter.Width * 0.5),
                  Constraint.RelativeToParent((parent) => parent.Height * 0.36)
               );
               imgDistanceQuarter.IsVisible = false;

               imgDistanceHalf = new Image();
               imgDistanceHalf.Source = "@drawable/display_distance3.png";
               layout.Children.Add
               (
                  imgDistanceHalf,
                  Constraint.RelativeToParent((parent) => parent.Width * 0.5 - imgDistanceHalf.Width * 0.5),
                  Constraint.RelativeToParent((parent) => parent.Height * 0.36)
               );
               imgDistanceHalf.IsVisible = false;
            }
         }

         // label speed

         labelSpeed = new CustomLabel();
         labelSpeed.Text = "000";
         labelSpeed.Size = CustomLabel.SIZE_SPEEDO;
         labelSpeed.TextColor = Color.White;
         layout.Children.Add
         (
            labelSpeed,
            Constraint.RelativeToParent((parent) => parent.Width * 0.55),
            Constraint.RelativeToParent((parent) => parent.Height * 0.56)
         );

         // label unit

         labelUnit = new CustomLabel();
         labelUnit.Text = Settings.GetSpeedUnit();
         labelUnit.Size = CustomLabel.SIZE_SPEEDO_UNIT;
         labelSpeed.TextColor = Color.White;
         layout.Children.Add
         (
            labelUnit,
            Constraint.RelativeToParent((parent) => parent.Width * 0.57 + labelSpeed.Width * 0.54),
            Constraint.RelativeToParent((parent) => parent.Height * 0.57 + labelSpeed.Height * 0.8)
         );

         // needle

         imgNeedle = new Image();
         imgNeedle.Source = runModeProvider.Needle;
         //imgNeedle.AnchorY = 1.0 / 378.0 * 340; // 1 / imgHeight * centerOfNeedlePivot
         imgNeedle.AnchorY = 1.0 / 365.0 * 337; // 1 / imgHeight * centerOfNeedlePivot
         //imgNeedle.AnchorY = 1.0 / 487.0 * 450; // 1 / imgHeight * centerOfNeedlePivot
         imgNeedle.Rotation = ROTATION_TARGET_MAX;
         layout.Children.Add
         (
            imgNeedle,
            Constraint.RelativeToView(imgSpeedo, (parent, view) => (view.Width  * 0.5) - (35.0 * 0.5)),
            Constraint.RelativeToView(imgSpeedo, (parent, view) => (view.Height * 0.5) - (165.5)),
            Constraint.Constant(35.0),
            Constraint.Constant(184.0)
         );

         // split time

         if (isAccelerationMode)
         {
            labelSplitTime = new CustomLabel();
            labelSplitTime.Size = CustomLabel.SIZE_LARGE;
            SetSplitTimeText();
            layout.Children.Add
            (
               labelSplitTime,
               Constraint.RelativeToParent((parent) => parent.Width * 0.07),
               Constraint.RelativeToParent((parent) => parent.Height * 0.8)
            );
         }

         // mode status

         labelStatus = new CustomLabel();
         labelStatus.Size = CustomLabel.SIZE_LARGE;
         SetStatusText(runModeProvider.Status);
         layout.Children.Add
         (
            labelStatus,
            Constraint.RelativeToParent(delegate(RelativeLayout parent)
            {
               if (isAccelerationMode) return parent.Width * 0.55;
               else                    return parent.Width * 0.55 - labelStatus.Width * 0.5;
            }),
            Constraint.RelativeToParent((parent) => parent.Height * 0.8)
         );

         // abort button

         btnAbort = new Button();
         btnAbort.Image = Localization.btn_abort;
         btnAbort.BackgroundColor = Color.Transparent;
         btnAbort.Clicked += OnButtonAbort;
         layout.Children.Add
         (
            btnAbort,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnAbort.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height - btnAbort.Height)
         );

         // overlay stop vehicle

         btnStopVehicle = new Button();
         btnStopVehicle.Image = Localization.stop_vehicle;
         btnStopVehicle.BackgroundColor = Color.Transparent;
         btnStopVehicle.IsEnabled = false;
         btnStopVehicle.IsVisible = false;
         layout.Children.Add
         (
            btnStopVehicle,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnStopVehicle.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height - btnStopVehicle.Height)
         );
      }

      public void OnSpeedoBackgroundResize(object sender, EventArgs e)
      {
         // resize needle in relation to page size
         imgNeedle.Scale = (Height / 1107) * 1.3;
      }

      public void OnAcceleromterUpdate() // IAcceleromterListener
      {
         if (isLaunchDetected == true)
         {
            // record high frequence accelerometer samples to get PeakAcceleration later on
            runData.AddEventFast(accelerometerProvider.GetX(),
                                 accelerometerProvider.GetY(),
                                 accelerometerProvider.GetZ(),
                                 filteredSpeedForDisplay);
         }
      }

      public void OnAcceleromterLaunchDetected() // IAcceleromterListener
      {
         if (settleCounter <= 0) // wait for initial needle animation
         {
            if (isLaunchDetected == false) // only once
            {
               //if (speedRaw < runAdjust.StopLimit) // if vehicle is standing still
               {
                  if (IsResultAvailable() == false) // if not finished yet
                  {
                     runModeProvider.OnLaunchDetected();
                     SetStatusText(runModeProvider.Status);
                     Debug.LogToFileEventText("run start");
                     isLaunchDetected = true;
                  }
               }
            }
         }
      }

      public void OnGpsStatusUpdate() // IGpsListener
      {
      }

      public void OnGpsLocationUpdate() // IGpsListener
      {
         timeStampLastGps = DateTime.Now.Ticks;

         accelerometerProvider.SetSpeed(gpsProvider.GetSpeed());

         if (IsResultAvailable() == false)
         {
            // vehicle is standing still before launch -> make location of launch more precise by averaging all locations
            // OR
            // in mode "brake" we are waiting until target speed is reached -> update last known location
            runData.AddStartLocation(gpsProvider, accelerometerProvider.GetTimeStamp());
         }

         // switch distance indicator according to traveled distance
         if (isAccelerationMode == true)
         {
            if (isDemoMode == false) // if demo mode -> distance indicator is disabled
            {
               float distance = runData.GetDistanceFiltered();

               if ((distance <= 201) && (imgDistanceOff.IsVisible == false))
               {
                  // no distance traveled
                  imgDistanceOff.IsVisible     = true;
                  imgDistanceQuarter.IsVisible = false;
                  imgDistanceHalf.IsVisible    = false;
               }
               else if ((distance > 402) && (distance <= 804) && (imgDistanceQuarter.IsVisible == false))
               {
                  // quarter of a mile passed
                  imgDistanceQuarter.IsVisible = true;
                  imgDistanceHalf.IsVisible    = false;
                  imgDistanceOff.IsVisible     = false;
               }
               else if ((distance > 804) && (imgDistanceHalf.IsVisible == false))
               {
                  // half of a mile passed
                  imgDistanceHalf.IsVisible    = true;
                  imgDistanceQuarter.IsVisible = false;
                  imgDistanceOff.IsVisible     = false;
               }
            }
         }
      }

      bool OnTimer()
      {
         if (isRunning == false) // page disappears
         {
            return false; // stop timer
         }

         if (settleCounter >= 0) // page just appeared
         {
            // animate needle from max to zero
            settleCounter -= 2;
            double time = 1.0 - (1.0 / SETTLE_COUNT_LIMIT * settleCounter); // count from 0..1
            needleAngleDegree = Tools.EasingQuadraticInOut(time, 90, -270, 1);
            imgNeedle.RotateTo(needleAngleDegree, SPEEDO_UPDATE_RATE_MS);

            // settle down accelerometer sensor
            accelerometerProvider.SetSpeed(0);

            return true; // restart timer until sensor/device settled from last user touch
         }

         accelerationRaw = accelerometerProvider.GetAcceleration();
         speedRaw        = accelerometerProvider.GetSpeed();

         // inhibit negtive speed
         if (speedRaw < 0) speedRaw = 0;

         if (speedRaw > 1)
         {
            // let speed be higher at start, but true at end
            speedAdjustedForDisplay = runAdjust.GetAdjustedDisplaySpeed(speedRaw);
         }
         else
         {
            // restart detection
            speedAdjustedForDisplay = 0;
         }

         // do not move needle until launch is detected
         if (isLaunchDetected == false)
         {
            speedAdjustedForDisplay = 0;
            speedRaw = 0;
         }

         if (Config.Run.PeakALabelEnabled)
         {
            if (dbgLabelAccel == null)
            {
               dbgLabelAccel = new Label();
               dbgLabelAccel.FontSize = 18;
               dbgLabelAccel.Text = "0.0 m/s^2";
               layout.Children.Add
               (
                  dbgLabelAccel,
                  Constraint.RelativeToParent((parent) => parent.Width * 0.2),
                  Constraint.RelativeToParent((parent) => parent.Height * 0.14)
               );
            }
            if (dbgLabelAccelPeak == null)
            {
               dbgLabelAccelPeak = new Label();
               dbgLabelAccelPeak.FontSize = 18;
               dbgLabelAccelPeak.Text = "0.0 m/s^2 (peak)";
               layout.Children.Add
               (
                  dbgLabelAccelPeak,
                  Constraint.RelativeToParent((parent) => parent.Width * 0.2),
                  Constraint.RelativeToParent((parent) => parent.Height * 0.18)
               );
               layout.ForceLayout();
            }
            dbgAccel = accelerationRaw;
            if (dbgAccel > dbgAccelPeak) dbgAccelPeak = dbgAccel;
            dbgLabelAccel.Text     = dbgAccel.ToString("0.0")     + " m/s^2";
            dbgLabelAccelPeak.Text = dbgAccelPeak.ToString("0.0") + " m/s^2 (peak)";
         }

         if (isDemoMode)
         {
            //labelDemoMode.Text = "speed:" + (int)speedRaw + " accel:" + (int)accelerationRaw;
         }

         // filter (smooth) data
         filteredSpeedForDisplay = movingAverageForDisplay.Get(speedAdjustedForDisplay);

         runModeProvider.Acceleration = accelerationRaw;
         runModeProvider.Speed        = speedRaw;

         // animate needle
         needleAngleDegree = Tools.Project(filteredSpeedForDisplay,
                                           ROTATION_SOURCE_MIN,
                                           ROTATION_SOURCE_MAX,
                                           ROTATION_TARGET_MIN,
                                           ROTATION_TARGET_MAX);

         imgNeedle.RotateTo(needleAngleDegree, SPEEDO_UPDATE_RATE_MS);

         filteredSpeedForDisplayWithUnit = Tools.ToSpeedUnitF(filteredSpeedForDisplay);

         // 123.45 -> 123
         //   0.00 ->   0
         labelSpeed.Text = String.Format("{0:000}", filteredSpeedForDisplayWithUnit);

         if (runModeProvider.IsStarted)
         {
            if (runModeProvider.HasTimeSplitChanged)
            {
               runModeProvider.OnTimeSplit();
               SetSplitTimeText();
               Debug.LogToFileEventText("run split at " + runModeProvider.TimeElapsed);
               btnStopVehicle.IsVisible = true;
               btnAbort.IsVisible = false;
               btnAbort.Image = runModeProvider.ButtonResults;
               runData.Store(); // commit data of this run to database
            }

            SetStatusText(Localization.pageRunTimeTotal + ": " + runModeProvider.TimeElapsed);

            if (runModeProvider.IsFinished)
            {
               runModeProvider.OnFinished();
               SetStatusText(runModeProvider.Status);
               Debug.LogToFileEventText("run stop at " + runModeProvider.TimeElapsed);
               btnAbort.Image = runModeProvider.ButtonResults;
               runData.Store(); // commit data of this run to database
            }
         }

         // reset run if "launch was detected" && "speed is zero" && "no results available yet" && "some time elapsed since launch"
         if (  (runModeProvider.IsReset      == true )
            && (filteredSpeedForDisplay      <  0.1  )
            && (IsResultAvailable()          == false)
            && (runModeProvider.TimeElapsedF > 3     ))
         {
            Debug.LogToFile("run reset at " + runModeProvider.TimeElapsed);
            runModeProvider.OnReset();
            SetSplitTimeText();
            SetStatusText(runModeProvider.Status);
            runData.Reset();
            isLaunchDetected = false;
         }

         // swap from button "stop vehicle" to "results button" if "run finished" and "speed is zero"
         if (  (IsResultAvailable()            )
            && (filteredSpeedForDisplay <  0.1 )
            && (btnStopVehicle.IsVisible       )) // only once
         {
            btnStopVehicle.IsVisible = false;
            btnAbort.IsVisible = true;
         }

         // log data
         if (isLaunchDetected)
         {
            Debug.LogToFile(accelerometerProvider.GetTimeStamp(),
                            accelerationRaw,
                            speedRaw,
                            gpsProvider.GetSpeed(),
                            runData.GetDistanceFiltered(),
                            runData.GetDistanceGps(),
                            runData.GetHeightFiltered(),
                            runData.GetHeightGps());

            runData.AddEventSlow(gpsProvider,
                                 accelerometerProvider.GetTimeStamp(),
                                 accelerationRaw,
                                 speedRaw);
         }

         // gps icon
         if (isDemoMode == false) // if demo mode -> gps is always off
         {
            if ((DateTime.Now.Ticks - timeStampLastGps) > GPS_TIMEOUT) // if gps connection is bad
            {
               if (isGpsOk) // switch to bad icon, but only once
               {
                  imgGps.Source = "@drawable/icn_gps_no.png";
                  isGpsOk = false;
               }
            }
            else
            {
               if (isGpsOk == false) // switch to good icon, but only once
               {
                  imgGps.Source = "@drawable/icn_gps_yes.png";
                  isGpsOk = true;
               }
            }
         }

         return true; // restart timer
      }

      bool IsResultAvailable()
      {
         return btnAbort.Image.File.Contains("results");
      }

      void SetSplitTimeText()
      {
         if (Config.Run.SplitTimeLabelEnabled)
         {
            if (isAccelerationMode)
            {
               labelSplitTime.Text = Localization.pageRunTimeSplit + ": " + runModeProvider.TimeSplit;
            }
         }
      }

      void SetStatusText(string s)
      {
         if (Config.Run.StatusLabelEnabled)
         {
            labelStatus.Text = s;
            layout.ForceLayout(); // redraw scene because text in labelStatus not centered automatically
         }
      }
   }
}
