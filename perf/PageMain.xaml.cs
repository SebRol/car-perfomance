using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageMain : ContentPage, IDisposable, IGpsListener, IAcceleromterListener
   {
      const int   CONDITION_UPDATE_RATE_MS  = 100;
      const int   CONDITION_NOT_OK_TIMEOUT  = 10;
      const int   CONDITION_NOT_OK          = -2;
      const int   CONDITION_OK              = -1;
      const long  GPS_TIMEOUT               = 50000000; // 10 million ticks in a second -> 5 seconds

      PageRun               pageRun;
      PageConfig            pageConfig;
      PageProfile           pageProfile;
      PageResults           pageResults;
      PageHelp              pageHelp;
      PageSetup             pageSetup;

      // //////////////////////////////////////////////////////////////////////////////////////////////////////
      // debug and test
      PageDebugGps           pageDebugGps;
      PageDebugAccelerometer pageDebugAccelerometer;
      PageDebugPurchase      pageDebugPurchase;
      PageDebugRunAdjust     pageDebugRunAdjust;
      Button                 btnDebugGps;
      Button                 btnDebugAccelerometer;
      Image                  imgDebugRunAdjust;
      TapGestureRecognizer   tgrDebugRunAdjust;
      // //////////////////////////////////////////////////////////////////////////////////////////////////////

      bool                  isStartUpNoteShown;

      bool                  isRunning;
      bool                  isMountOk;
      bool                  isGpsOk;
      bool                  isVelocityOk;
      bool                  isDemoMode;
      bool                  isImageFadeLocked;

      int                   mountNotOkTimeout;
      int                   gpsNotOkTimeout;
      int                   velocityNotOkTimeout;
      long                  timeStampLastGps;

      IDeviceGps            gpsProvider;
      IDeviceAcceleromter   accelerometerProvider;
      IRunMode              runModeProvider;
      IDeviceSound          soundProvider;

      Image                 imgBackground;

      StackLayout           lytHorizontalTab;
      Button                btnTabAcceleration;
      Button                btnTabBrake;
      Button                btnTabZeroToZero;
      Button                btnTabResults;

      Image                 imgGpsStateOk;
      Image                 imgMountStateOk;
      Image                 imgVelocityStateOk;
      Image                 imgGpsStateNotOk;
      Image                 imgMountStateNotOk;
      Image                 imgVelocityStateNotOk;

      CustomLabel           lblGpsState;
      CustomLabel           lblMountState;
      CustomLabel           lblVelocityState;

      Button                btnHelp;
      Button                btnDebugPurchase;
      Button                btnConfig;
      Button                btnSetup;
      Button                btnRun;
      Button                btnProfile;

      AccelerometerRecorder accelerometerRecorder;

      public PageMain()
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         // DependencyService decides if to use iOS or Android
         gpsProvider            = DependencyService.Get<IDeviceGps>();
         accelerometerProvider  = DependencyService.Get<IDeviceAcceleromter>();
         soundProvider          = DependencyService.Get<IDeviceSound>();

         runModeProvider        = new RunModeAcceleration(accelerometerProvider, soundProvider);
         accelerometerRecorder  = new AccelerometerRecorder(true) ; // true: use queue mode to limit recording size

         // on first start -> use device language -> update our settings
         // on next starts -> use our updated settings
         LocaleId deviceLanguage = DependencyService.Get<IDeviceLocale>().GetLocale();
         // set language, use system language as default
         Localization.isEnglish = Settings.GetValueOrDefault(Settings.Language, deviceLanguage == LocaleId.English);
         Settings.AddOrUpdateValue(Settings.Language, Localization.isEnglish);

         InitLayout();
         InitLocalisation();
         InitHelp();

         bool showDisclaimer = Settings.GetValueOrDefault(Settings.Disclaimer, true);
         bool showTipOfDay   = Settings.GetValueOrDefault(Settings.TipOfDay, true);
         if (showDisclaimer || showTipOfDay)
         {
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
               if (isStartUpNoteShown == false) // only show once
               {
                  isStartUpNoteShown = true;

                  if (showDisclaimer)
                  {
                     Navigation.PushAsync(new PageText(PageText.ContentType.Disclaimer));
                  }
                  else
                  {
                     Navigation.PushAsync(new PageText(PageText.ContentType.TipOfDay));
                  }
               }

               return false; // stop timer
            });
         }
      }

      public void Dispose() // IDisposable
      {
         imgBackground.Source = null;
         imgBackground = null;

         imgGpsStateOk.Source = null;
         imgGpsStateOk = null;

         imgGpsStateNotOk.Source = null;
         imgGpsStateNotOk = null;

         imgMountStateOk.Source = null;
         imgMountStateOk = null;

         imgMountStateNotOk.Source = null;
         imgMountStateNotOk = null;

         imgVelocityStateOk.Source = null;
         imgVelocityStateOk = null;

         imgVelocityStateNotOk.Source = null;
         imgVelocityStateNotOk = null;

         if (Config.Debug.PageRunAdjustEnabled)
         {
            imgDebugRunAdjust.Source = null;
            imgDebugRunAdjust = null;
            tgrDebugRunAdjust.Tapped -= OnImageDebugRunAdjust;
            tgrDebugRunAdjust = null;
         }

         lblGpsState.SizeChanged      -= OnLayoutSizeChanged;
         lblMountState.SizeChanged    -= OnLayoutSizeChanged;
         lblVelocityState.SizeChanged -= OnLayoutSizeChanged;

         lblGpsState = null;
         lblMountState = null;
         lblVelocityState = null;

         lytHorizontalTab = null;

         btnTabAcceleration.Image = null;
         btnTabAcceleration.Clicked -= OnButtonTabAcceleration;
         btnTabAcceleration = null;

         btnTabBrake.Image = null;
         btnTabBrake.Clicked -= OnButtonTabBrake;
         btnTabBrake = null;

         btnTabZeroToZero.Image = null;
         btnTabZeroToZero.Clicked -= OnButtonTabZeroToZero;
         btnTabZeroToZero = null;

         btnTabResults.Image = null;
         btnTabResults.Clicked -= OnButtonTabResults;
         btnTabResults = null;
         
         btnHelp.Image = null;
         btnHelp.Clicked -= OnButtonHelp;
         btnHelp = null;

         if (Config.Debug.PagePurchaseEnabled)
         {
            btnDebugPurchase.Image = null;
            btnDebugPurchase.Clicked -= OnButtonDebugPurchase;
            btnDebugPurchase = null;  
         }

         btnConfig = null;
         btnConfig.Image = null;
         btnConfig.Clicked -= OnButtonConfig;

         btnSetup.Image = null;
         btnSetup.Clicked -= OnButtonSetup;
         btnSetup = null;

         btnRun.Image = null;
         btnRun.Clicked -= OnButtonRun;
         btnRun = null;

         btnProfile.Image = null;
         btnProfile.Clicked -= OnButtonProfile;
         btnProfile = null;

         gpsProvider = null;
         accelerometerProvider = null;
         runModeProvider = null;
         soundProvider = null;
         accelerometerRecorder = null;
         pageRun = null;
         pageConfig = null;
         pageProfile = null;
         pageResults = null;
         pageHelp = null;
         pageDebugRunAdjust = null;
         pageDebugPurchase = null;
         pageSetup = null;
      }

      void InitLayout()
      {
         double fontSize = Device.GetNamedSize(NamedSize.Medium, typeof(CustomLabel));

         imgBackground = new Image();
         imgBackground.Source = runModeProvider.Background;
         imgBackground.Scale = 0.9;

         imgGpsStateOk = new Image();
         imgGpsStateOk.Source = "@drawable/icn_gps_yes.png";

         imgGpsStateNotOk = new Image();
         imgGpsStateNotOk.Source = "@drawable/icn_gps_no.png";

         imgMountStateOk = new Image();
         imgMountStateOk.Source = "@drawable/icn_mount_yes.png";

         imgMountStateNotOk = new Image();
         imgMountStateNotOk.Source = "@drawable/icn_mount_no.png";
         imgMountStateNotOk.WidthRequest = imgMountStateNotOk.Width;
         imgMountStateNotOk.HeightRequest = imgMountStateNotOk.Height;

         imgVelocityStateOk = new Image();
         imgVelocityStateOk.Source = "@drawable/icn_velocity_yes.png";

         imgVelocityStateNotOk = new Image();
         imgVelocityStateNotOk.Source = "@drawable/icn_velocity_no.png";

         lblGpsState = new CustomLabel();
         lblGpsState.Size = CustomLabel.SIZE_MEDIUM;

         lblMountState = new CustomLabel();
         lblMountState.Size = CustomLabel.SIZE_MEDIUM;

         lblVelocityState = new CustomLabel();
         lblVelocityState.Size = CustomLabel.SIZE_MEDIUM;

         lytHorizontalTab = new StackLayout();
         lytHorizontalTab.Orientation = StackOrientation.Horizontal;
         lytHorizontalTab.HorizontalOptions = LayoutOptions.CenterAndExpand;
         lytHorizontalTab.Spacing = -1;

         btnTabAcceleration = new Button();
         btnTabAcceleration.Image = Localization.tab_acceleration_active;
         btnTabAcceleration.BackgroundColor = Color.Transparent;
         btnTabAcceleration.Clicked += OnButtonTabAcceleration;

         btnTabBrake = new Button();
         btnTabBrake.Image = Localization.tab_brake;
         btnTabBrake.BackgroundColor = Color.Transparent;
         btnTabBrake.Clicked += OnButtonTabBrake;

         btnTabZeroToZero = new Button();
         btnTabZeroToZero.Image = Localization.tab_zero;
         btnTabZeroToZero.BackgroundColor = Color.Transparent;
         btnTabZeroToZero.Clicked += OnButtonTabZeroToZero;

         btnTabResults = new Button();
         btnTabResults.Image = Localization.tab_results;
         btnTabResults.BackgroundColor = Color.Transparent;
         btnTabResults.Clicked += OnButtonTabResults;

         btnHelp = new Button();
         btnHelp.Image = "@drawable/btn_help.png";
         btnHelp.BackgroundColor = Color.Transparent;
         btnHelp.Clicked += OnButtonHelp;

         if (Config.Debug.PagePurchaseEnabled)
         {
            btnDebugPurchase = new Button();
            btnDebugPurchase.Text = "Purchase";
            btnDebugPurchase.Clicked += OnButtonDebugPurchase;
         }

         btnConfig = new Button();
         btnConfig.Image = "@drawable/btn_settings.png";
         btnConfig.BackgroundColor = Color.Transparent;
         btnConfig.Clicked += OnButtonConfig;

         btnSetup = new Button();
         btnSetup.Image = runModeProvider.ButtonSetup;
         btnSetup.BackgroundColor = Color.Transparent;
         btnSetup.Clicked += OnButtonSetup;

         btnRun = new Button();
         btnRun.Image = Localization.btn_run_inactive;
         btnRun.BackgroundColor = Color.Transparent;
         btnRun.Clicked += OnButtonRun;

         btnProfile = new Button();
         btnProfile.Image = "@drawable/btn_profile_c1.png";
         btnProfile.BackgroundColor = Color.Transparent;
         btnProfile.Clicked += OnButtonProfile;

         if (Config.Debug.PageRunAdjustEnabled)
         {
            imgDebugRunAdjust = new Image();
            imgDebugRunAdjust.Source = "@drawable/btn_debug_calibration.png";
            tgrDebugRunAdjust = new TapGestureRecognizer();
            tgrDebugRunAdjust.Tapped += OnImageDebugRunAdjust;
            tgrDebugRunAdjust.NumberOfTapsRequired = 2;
            imgDebugRunAdjust.GestureRecognizers.Add(tgrDebugRunAdjust);
         }

         if (Config.Debug.PageSensorEnabled)
         {
            btnDebugGps = new Button();
            btnDebugGps.Text = "DebugGps";
            btnDebugGps.Clicked += OnButtonDebugGps;

            btnDebugAccelerometer = new Button();
            btnDebugAccelerometer.Text = "DebugAccelerometer";
            btnDebugAccelerometer.Clicked += OnButtonDebugAccelerometer;
         }

         // background image

         layout.Children.Add
         (
            imgBackground,
            Constraint.RelativeToParent((parent) => parent.X),
            Constraint.RelativeToParent((parent) => parent.Y),
            Constraint.RelativeToParent((parent) => parent.Width),
            Constraint.RelativeToParent((parent) => parent.Height)
         );

         // tab

         lytHorizontalTab.Children.Add
         (
            btnTabAcceleration
         );

         lytHorizontalTab.Children.Add
         (
            btnTabBrake
         );

         lytHorizontalTab.Children.Add
         (
            btnTabZeroToZero
         );

         lytHorizontalTab.Children.Add
         (
            btnTabResults
         );

         layout.Children.Add
         (
            lytHorizontalTab,
            Constraint.RelativeToParent((parent) => parent.X),
            Constraint.RelativeToParent((parent) => parent.Y),
            Constraint.RelativeToParent((parent) => parent.Width)
         );

         // help

         layout.Children.Add
         (
            btnHelp,
            Constraint.RelativeToView(btnConfig, (parent, view) => view.X),
            Constraint.RelativeToView(lytHorizontalTab, (parent, view) => view.Y + view.Height)
         );

         if (Config.Debug.PageRunAdjustEnabled)
         {
            // calibration (invisble png)

            layout.Children.Add
            (
               imgDebugRunAdjust,
               Constraint.RelativeToParent((parent) => parent.Width * 0.5 - imgDebugRunAdjust.Width * 0.5),
               Constraint.RelativeToView(lytHorizontalTab, (parent, view) => view.Y + view.Height * 1.2)
            );
         }

         // purchase

         if (Config.Debug.PagePurchaseEnabled)
         {
            layout.Children.Add
            (
               btnDebugPurchase,
               Constraint.RelativeToView(btnSetup, (parent, view) => view.X),
               Constraint.RelativeToView(btnHelp, (parent, view) => view.Y + view.Height)
            );
         }

         // setup

         layout.Children.Add
         (
            btnSetup,
            Constraint.RelativeToView(btnProfile, (parent, view) => view.X),
            Constraint.RelativeToView(btnHelp, (parent, view) => view.Y)
         );

         if (Config.Debug.PageSensorEnabled)
         {
            layout.Children.Add
            (
               btnDebugGps,
               Constraint.RelativeToParent((parent) => parent.Width * 0.08),
               Constraint.RelativeToParent((parent) => parent.Height * 0.25)
            );

            layout.Children.Add
            (
               btnDebugAccelerometer,
               Constraint.RelativeToParent((parent) => parent.Width * 0.08),
               Constraint.RelativeToParent((parent) => parent.Height * 0.35)
            );
         }

         // precondition state

         layout.Children.Add
         (
            imgGpsStateNotOk,
            Constraint.RelativeToParent((parent) => parent.Width * 0.27),
            Constraint.RelativeToParent((parent) => parent.Height * 0.30)
         );

         layout.Children.Add
         (
            imgGpsStateOk,
            Constraint.RelativeToView(imgGpsStateNotOk, (parent, view) => view.X),
            Constraint.RelativeToView(imgGpsStateNotOk, (parent, view) => view.Y)
         );

         layout.Children.Add
         (
            lblGpsState,
            Constraint.RelativeToView(imgGpsStateNotOk, (parent, view) => view.X + view.Width + fontSize * 0.7),
            Constraint.RelativeToView(imgGpsStateNotOk, (parent, view) => view.Y + view.Height / 2 - lblGpsState.Height / 2)
         );

         layout.Children.Add
         (
            imgMountStateNotOk,
            Constraint.RelativeToParent((parent) => parent.Width * 0.27),
            Constraint.RelativeToParent((parent) => parent.Height * 0.44)
         );

         layout.Children.Add
         (
            imgMountStateOk,
            Constraint.RelativeToView(imgMountStateNotOk, (parent, view) => view.X),
            Constraint.RelativeToView(imgMountStateNotOk, (parent, view) => view.Y)
         );

         layout.Children.Add
         (
            lblMountState,
            Constraint.RelativeToView(imgMountStateNotOk, (parent, view) => view.X + view.Width + fontSize * 0.7),
            Constraint.RelativeToView(imgMountStateNotOk, (parent, view) => view.Y + view.Height / 2 - lblMountState.Height / 2)
         );

         layout.Children.Add
         (
            imgVelocityStateNotOk,
            Constraint.RelativeToParent((parent) => parent.Width * 0.27),
            Constraint.RelativeToParent((parent) => parent.Height * 0.58)
         );

         layout.Children.Add
         (
            imgVelocityStateOk,
            Constraint.RelativeToView(imgVelocityStateNotOk, (parent, view) => view.X),
            Constraint.RelativeToView(imgVelocityStateNotOk, (parent, view) => view.Y)
         );

         layout.Children.Add
         (
            lblVelocityState,
            Constraint.RelativeToView(imgVelocityStateNotOk, (parent, view) => view.X + view.Width + fontSize * 0.7),
            Constraint.RelativeToView(imgVelocityStateNotOk, (parent, view) => view.Y + view.Height / 2 - lblVelocityState.Height / 2)
         );

         // buttons at bottom

         layout.Children.Add
         (
            btnConfig,
            Constraint.RelativeToParent((parent) => (parent.Width - btnConfig.Width - btnRun.Width - btnProfile.Width) / 3 ),
            Constraint.RelativeToParent((parent) => parent.Height - btnConfig.Height)
         );

         layout.Children.Add
         (
            btnRun,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnRun.Width / 2),
            Constraint.RelativeToParent((parent) => parent.Height - btnRun.Height)
         );

         layout.Children.Add
         (
            btnProfile,
            Constraint.RelativeToParent((parent) => parent.Width - (parent.Width - btnConfig.Width - btnRun.Width - btnProfile.Width) / 3 - btnProfile.Width),
            Constraint.RelativeToParent((parent) => parent.Height - btnProfile.Height)
         );

         // center views based on their width
         lblGpsState.SizeChanged      += OnLayoutSizeChanged;
         lblMountState.SizeChanged    += OnLayoutSizeChanged;
         lblVelocityState.SizeChanged += OnLayoutSizeChanged;
      }

      void InitState()
      {
         imgGpsStateOk.IsVisible    = false;
         imgGpsStateNotOk.IsVisible = true;
         lblGpsState.Text           = Localization.pageMainGpsNotOk;
         lblGpsState.TextColor      = Color.Red;

         imgMountStateOk.IsVisible    = false;
         imgMountStateNotOk.IsVisible = true;
         lblMountState.Text           = Localization.pageMainMountNotOk;
         lblMountState.TextColor      = Color.Red;

         imgVelocityStateOk.IsVisible    = false;
         imgVelocityStateNotOk.IsVisible = true;
         lblVelocityState.Text           = Localization.pageMainSpeedNotOk;
         lblVelocityState.TextColor      = Color.Red;

         mountNotOkTimeout    = CONDITION_NOT_OK_TIMEOUT;
         velocityNotOkTimeout = CONDITION_NOT_OK_TIMEOUT;
         gpsNotOkTimeout      = CONDITION_NOT_OK;

         btnRun.Image = Localization.btn_run_inactive;

         if (Config.Main.ButtonRunAlwayEnabled == false)
         {
            btnRun.IsEnabled = false;
         }

         isGpsOk      = false;
         isMountOk    = false;
         isVelocityOk = false;

         // load colored image depending on active profile
         btnProfile.Image = GetProfileButtonImage();

         runModeProvider.Reset();
         accelerometerRecorder.Reset();
      }

      void InitLocalisation()
      {
         EventBus.GetInstance().Subscribe<LocalisationChangeEvent>(e =>
         {
            // event handler for LocalisationChangeEvent

            if (btnTabAcceleration.Image.File.Contains("_active")) btnTabAcceleration.Image  = Localization.tab_acceleration_active;
            else                                                   btnTabAcceleration.Image  = Localization.tab_acceleration;
            if (btnTabBrake.Image.File.Contains("_active"))        btnTabBrake.Image         = Localization.tab_brake_active;
            else                                                   btnTabBrake.Image         = Localization.tab_brake;
            if (btnTabZeroToZero.Image.File.Contains("_active"))   btnTabZeroToZero.Image    = Localization.tab_zero_active;
            else                                                   btnTabZeroToZero.Image    = Localization.tab_zero;

            btnTabResults.Image = Localization.tab_results;
         });
      }

      void InitHelp()
      {
         EventBus.GetInstance().Subscribe<HelpAppStoreEvent>(e =>
         {
            // event handler for HelpAppStoreEvent

            pageHelp = new PageHelp(PageHelp.ModeAppStore);
            Navigation.PushAsync(pageHelp);
         });
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Debug.LogToFileMethod();
         Analytics.TrackPage(Analytics.PAGE_MAIN);

         InitState();
         ApplyCalibration(false); // but do not log details to file

         if (gpsProvider != null)
         {
            gpsProvider.SetListener(this);
         }

         if (accelerometerProvider != null)
         {
            // register for launch detection event
            accelerometerProvider.SetListener(this);
         }

         isRunning = true;
         timeStampLastGps = DateTime.Now.Ticks;
         Device.StartTimer(TimeSpan.FromMilliseconds(CONDITION_UPDATE_RATE_MS), OnTimer);
      }

      protected override void OnDisappearing()
      {
         isRunning = false; // stops timer, see OnTimer()
         base.OnDisappearing();
      }

      protected override bool OnBackButtonPressed()
      {
         // inhibit page navigation if appstore overlay is visible (just remove overlay)
         return AppStore.Instance.OnBackButtonPressed();
      }

      public void OnSleep()
      {
         Debug.LogToFileMethod();

         if (pageRun            != null)  pageRun.OnSleep();
         if (pageConfig         != null)  pageConfig.OnSleep();
         if (pageProfile        != null)  pageProfile.OnSleep();
         if (pageResults        != null)  pageResults.OnSleep();
         if (pageHelp           != null)  pageHelp.OnSleep();
         if (pageDebugRunAdjust != null)  pageDebugRunAdjust.OnSleep();
         if (pageDebugPurchase  != null)  pageDebugPurchase.OnSleep();
         if (pageSetup          != null)  pageSetup.OnSleep();

         if (Config.Debug.PageSensorEnabled)
         {
            if (pageDebugGps           != null) pageDebugGps.OnSleep();
            if (pageDebugAccelerometer != null) pageDebugAccelerometer.OnSleep();
         }

         if (Config.Purchase.ConsumeOnExitEnabled)
         {
            AppStore.Instance.Consume();
         }

         gpsProvider.DeInit();
         accelerometerProvider.DeInit();
         soundProvider.DeInit();
      }

      public void OnResume()
      {
         Debug.LogToFileMethod();

         gpsProvider.Init();
         accelerometerProvider.Init();
         soundProvider.Init();

         if (pageRun            != null)  pageRun.OnResume();
         if (pageConfig         != null)  pageConfig.OnResume();
         if (pageProfile        != null)  pageProfile.OnResume();
         if (pageResults        != null)  pageResults.OnResume();
         if (pageHelp           != null)  pageHelp.OnResume();
         if (pageDebugRunAdjust != null)  pageDebugRunAdjust.OnResume();
         if (pageDebugPurchase  != null)  pageDebugPurchase.OnResume();
         if (pageSetup          != null)  pageSetup.OnResume();

         if (Config.Debug.PageSensorEnabled)
         {
            if (pageDebugGps           != null) pageDebugGps.OnResume();
            if (pageDebugAccelerometer != null) pageDebugAccelerometer.OnResume();
         }
      }

      async public void ShowPageResults()
      {
         pageResults = new PageResults(runModeProvider.Mode);
         await Navigation.PushAsync(pageResults);
      }

      async public void ShowPageRun()
      {
         ApplyCalibration(true);

         if (isDemoMode)
         {
            DemoAcceleromter demoAcceleromter = new DemoAcceleromter(runModeProvider.Mode, accelerometerProvider);
            DemoGps          demoGps          = new DemoGps(demoAcceleromter);

            pageRun = new PageRun(runModeProvider, 
                                  demoGps, 
                                  demoAcceleromter, 
                                  this,
                                  true);
         }
         else
         {
            pageRun = new PageRun(runModeProvider, 
                                  gpsProvider, 
                                  accelerometerProvider, 
                                  this,
                                  false);
         }

         await Navigation.PushAsync(pageRun);
      }

      void OnLayoutSizeChanged(object sender, EventArgs args)
      {
         layout.ForceLayout();
      }

      void OnButtonTabAcceleration(object sender, EventArgs args)
      {
         if (btnTabAcceleration.Image.File.Contains("_active") == false)
         {
            btnTabAcceleration.Image = Localization.tab_acceleration_active;
            btnTabBrake.Image        = Localization.tab_brake;
            btnTabZeroToZero.Image   = Localization.tab_zero;

            runModeProvider = new RunModeAcceleration(accelerometerProvider, soundProvider);
            SetModeGraphics();
         }
      }

      void OnButtonTabBrake(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         if (btnTabBrake.Image.File.Contains("_active") == false)
         {
            btnTabAcceleration.Image = Localization.tab_acceleration;
            btnTabBrake.Image        = Localization.tab_brake_active;
            btnTabZeroToZero.Image   = Localization.tab_zero;

            runModeProvider = new RunModeBrake(accelerometerProvider, soundProvider);
            SetModeGraphics();
         }
      }

      void OnButtonTabZeroToZero(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         if (btnTabZeroToZero.Image.File.Contains("_active") == false)
         {
            btnTabAcceleration.Image = Localization.tab_acceleration;
            btnTabBrake.Image = Localization.tab_brake;
            btnTabZeroToZero.Image = Localization.tab_zero_active;

            runModeProvider = new RunModeZeroToZero(accelerometerProvider, soundProvider);
            SetModeGraphics();
         }
      }

      async void OnButtonTabResults(object sender, EventArgs args)
      {
         btnTabResults.IsEnabled = false;
         pageResults = new PageResults(runModeProvider.Mode);
         await Navigation.PushAsync(pageResults);
         btnTabResults.IsEnabled = true;
      }

      async void OnButtonHelp(object sender, EventArgs args)
      {
         btnHelp.IsEnabled = false;
         pageHelp = new PageHelp(runModeProvider.Mode);
         await Navigation.PushAsync(pageHelp);
         btnHelp.IsEnabled = true;
      }

      async void OnImageDebugRunAdjust(object sender, EventArgs args)
      {
         if (Config.Debug.PageRunAdjustEnabled)
         {
            imgDebugRunAdjust.IsEnabled = false;
            pageDebugRunAdjust = new PageDebugRunAdjust();
            await Navigation.PushAsync(pageDebugRunAdjust);
            imgDebugRunAdjust.IsEnabled = true;
         }
      }

      async void OnButtonDebugPurchase(object sender, EventArgs args)
      {
         if (Config.Debug.PagePurchaseEnabled)
         {
            btnDebugPurchase.IsEnabled = false;
            pageDebugPurchase = new PageDebugPurchase();
            await Navigation.PushAsync(pageDebugPurchase);
            btnDebugPurchase.IsEnabled = true;
         }
      }

      async void OnButtonConfig(object sender, EventArgs args)
      {
         btnConfig.IsEnabled = false;
         pageConfig = new PageConfig();
         await Navigation.PushAsync(pageConfig);
         btnConfig.IsEnabled = true;
      }

      async void OnButtonSetup(object sender, EventArgs args)
      {
         btnSetup.IsEnabled = false;
         pageSetup = new PageSetup(runModeProvider.SetupMode);
         await Navigation.PushAsync(pageSetup);
         btnSetup.IsEnabled = true;
      }

      async void OnButtonRun(object sender, EventArgs args)
      {
         btnRun.IsEnabled = false;

         DataItemVehicle vehicle = Database.GetInstance().GetActiveProfile();

         // if gps is off -> demo mode
         if (Config.Run.DemoModeEnabled) isDemoMode = (isGpsOk == false);
         else                            isDemoMode = false; // demo mode disabled by config

         if ( 
              (vehicle.Calibration < PageCalibration.CALIBRATION_SUCCESS_LIMIT) // profile was never calibrated
              ||                                                                // OR
              (accelerometerRecorder.IsPositionEqual(vehicle) == false)         // position of calibrated device in car mount has changed
            )
         {
            // launch calibration page -> after that, the run page is shown automaticaly
            await Navigation.PushAsync(new PageCalibration(vehicle, accelerometerProvider, soundProvider, this));
         }
         else
         {
            // launch run page
            ShowPageRun();
         }

         btnRun.IsEnabled = true;
      }

      async void OnButtonProfile(object sender, EventArgs args)
      {
         btnProfile.IsEnabled = false;
         pageProfile = new PageProfile(accelerometerProvider, soundProvider);
         await Navigation.PushAsync(pageProfile);
         btnProfile.IsEnabled = true;
      }

      void OnButtonDebugGps(object sender, EventArgs args)
      {
         if (Config.Debug.PageSensorEnabled)
         {
            pageDebugGps = new PageDebugGps(gpsProvider);
            Navigation.PushAsync(pageDebugGps);
         }
      }

      void OnButtonDebugAccelerometer(object sender, EventArgs args)
      {
         if (Config.Debug.PageSensorEnabled)
         {
            pageDebugAccelerometer = new PageDebugAccelerometer(accelerometerProvider);
            Navigation.PushAsync(pageDebugAccelerometer);
         }
      }

      public void OnAcceleromterUpdate() // IAcceleromterListener
      {
         accelerometerRecorder.Add(accelerometerProvider.GetX(), 
                                   accelerometerProvider.GetY(), 
                                   accelerometerProvider.GetZ(), 
                                   0);
      }

      public void OnAcceleromterLaunchDetected() // IAcceleromterListener
      {
         // device is not mounted
         mountNotOkTimeout = CONDITION_NOT_OK_TIMEOUT;
      }

      public void OnGpsStatusUpdate() // IGpsListener
      {
      }

      public void OnGpsLocationUpdate() // IGpsListener
      {
         float s = gpsProvider.GetSpeed();

         timeStampLastGps = DateTime.Now.Ticks;

         if (s > 2) // TODO make m/s configurable
         {
            // vehicle is moving
            velocityNotOkTimeout = CONDITION_NOT_OK_TIMEOUT;
         }

         if (accelerometerProvider != null)
         {
            // calibrate accelerometer with gps data
            accelerometerProvider.SetSpeed(s);
         }

         if (gpsProvider.GetStatus() == IDeviceGpsStatus.Connected)
         {
            // gps is ready to go
            if (gpsNotOkTimeout == CONDITION_NOT_OK)
            {
               // setup final cool down
               gpsNotOkTimeout = CONDITION_NOT_OK_TIMEOUT;
            }
         }
         else
         {
            // wait for gps
            gpsNotOkTimeout = CONDITION_NOT_OK;
         }
      }

      bool OnTimer()
      {
         if (isRunning == false)
         {
            return false; // stop timer if page disappears
         }

         // update icons red/green state
         ConditionCheckMount();
         ConditionCheckVelocity();
         ConditionCheckGps();

         // update button RUN state
         if (isGpsOk && isMountOk && isVelocityOk)
         {
            btnRun.Image = runModeProvider.ButtonRun;
            btnRun.IsEnabled = true;
         }
         else
         {
            btnRun.Image = Localization.btn_run_inactive;

            if (Config.Main.ButtonRunAlwayEnabled == false)
            {
               btnRun.IsEnabled = false;
            }
         }

         return true; // restart timer
      }

      void ConditionCheckMount()
      {
         if (mountNotOkTimeout == CONDITION_NOT_OK_TIMEOUT)
         {
            imgMountStateOk.IsVisible = false;
            imgMountStateNotOk.IsVisible = true;
            lblMountState.Text = Localization.pageMainMountNotOk;
            lblMountState.TextColor = Color.Red;
            isMountOk = false;
         }

         if (mountNotOkTimeout == 0)
         {
            imgMountStateOk.IsVisible = true;
            imgMountStateNotOk.IsVisible = false;
            lblMountState.Text = Localization.pageMainMountOk;
            lblMountState.TextColor = Color.White;
            isMountOk = true;
         }

         if (mountNotOkTimeout > CONDITION_OK) mountNotOkTimeout--;
      }

      void ConditionCheckVelocity()
      {
         if (velocityNotOkTimeout == CONDITION_NOT_OK_TIMEOUT)
         {
            imgVelocityStateOk.IsVisible = false;
            imgVelocityStateNotOk.IsVisible = true;
            lblVelocityState.Text = Localization.pageMainSpeedNotOk;
            lblVelocityState.TextColor = Color.Red;
            isVelocityOk = false;
         }

         if (velocityNotOkTimeout == 0)
         {
            imgVelocityStateOk.IsVisible = true;
            imgVelocityStateNotOk.IsVisible = false;
            lblVelocityState.Text = Localization.pageMainSpeedOk;
            lblVelocityState.TextColor = Color.White;
            isVelocityOk = true;
         }

         if (velocityNotOkTimeout > CONDITION_OK) velocityNotOkTimeout--;
      }

      void ConditionCheckGps()
      {
         if ((DateTime.Now.Ticks - timeStampLastGps) > GPS_TIMEOUT) // if gps connection is bad
         {
            // wait for gps
            gpsNotOkTimeout = CONDITION_NOT_OK;
         }

         if ((gpsNotOkTimeout == CONDITION_NOT_OK) && (isGpsOk == true))
         {
            imgGpsStateOk.IsVisible = false;
            imgGpsStateNotOk.IsVisible = true;
            lblGpsState.Text = Localization.pageMainGpsNotOk;
            lblGpsState.TextColor = Color.Red;
            isGpsOk = false;
         }

         if (gpsNotOkTimeout == CONDITION_NOT_OK_TIMEOUT)
         {
            imgGpsStateOk.IsVisible = false;
            imgGpsStateNotOk.IsVisible = true;
            lblGpsState.Text = Localization.pageMainGpsNotOk;
            lblGpsState.TextColor = Color.Red;
            isGpsOk = false;
         }

         if (gpsNotOkTimeout == 0)
         {
            imgGpsStateOk.IsVisible = true;
            imgGpsStateNotOk.IsVisible = false;
            lblGpsState.Text = Localization.pageMainGpsOk;
            lblGpsState.TextColor = Color.White;
            isGpsOk = true;
         }

         if (gpsNotOkTimeout > CONDITION_OK) gpsNotOkTimeout--;
      }

      string GetProfileButtonImage()
      {
         string result = "@drawable/btn_profile_c1.png"; // blue
         DataItemVehicle v = Database.GetInstance().GetActiveProfile();

         if      (v.Color.Equals("#2AB3FF")) result = "@drawable/btn_profile_c1.png"; // blue
         else if (v.Color.Equals("#35FF36")) result = "@drawable/btn_profile_c2.png"; // green
         else if (v.Color.Equals("#FF6565")) result = "@drawable/btn_profile_c3.png"; // red
         else if (v.Color.Equals("#FFA743")) result = "@drawable/btn_profile_c4.png"; // gold
         else if (v.Color.Equals("#EF6AFF")) result = "@drawable/btn_profile_c5.png"; // pink
         else if (v.Color.Equals("#FFFFFF")) result = "@drawable/btn_profile_c6.png"; // white
         else if (v.Color.Equals("#2E2E2E")) result = "@drawable/btn_profile_c7.png"; // silver
         else if (v.Color.Equals("#000000")) result = "@drawable/btn_profile_c8.png"; // black

         return result;
      }

      void ApplyCalibration(bool logDetailsToFile)
      {
         // apply calibration of given profile to acceleration sensor launch detection
         DataItemVehicle vehicle   = Database.GetInstance().GetActiveProfile();
         RunStartStop    startStop = new RunStartStop(vehicle);

         if (logDetailsToFile == true)
         {
            string latitude  = gpsProvider.GetLatitude().ToString().Replace(',', '.');
            string longitute = gpsProvider.GetLongitude().ToString().Replace(',', '.');

            Debug.LogToFile("location [latitude, longitude]: " + latitude + ", " + longitute);
            Debug.LogToFile(startStop.GetStartString());
            Debug.LogToFile(startStop.GetStopString());

            Analytics.TrackLocation(latitude, longitute);
         }

         accelerometerProvider.SetForceDetectLimit(startStop.GetStartLimit());
         accelerometerProvider.SetAxisOffset(vehicle.AxisOffsetX,
                                             vehicle.AxisOffsetY,
                                             vehicle.AxisOffsetZ);

         runModeProvider.StopDetectionLimit = startStop.GetStopLimit();
      }

      async void SetModeGraphics()
      {
         if (isImageFadeLocked == true) // if fading right now -> do not start another fade
         {
            imgBackground.Source = runModeProvider.Background;
            btnSetup.Image       = runModeProvider.ButtonSetup;
         }
         else
         {
            isImageFadeLocked        = true;        // fading is locked
            await imgBackground.FadeTo(0, 150);     // fade old out
            imgBackground.Source     = runModeProvider.Background;
            btnSetup.Image           = runModeProvider.ButtonSetup;
            await imgBackground.FadeTo(1, 150);     // fade new in
            isImageFadeLocked        = false;       // fading is unlocked
         }
      }
   }
}
