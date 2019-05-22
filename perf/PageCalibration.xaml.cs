using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageCalibration : ContentPage, IAcceleromterListener, IDisposable
   {
      public const float CALIBRATION_SUCCESS_LIMIT = 0.001f; // m/s^2

      const int          SAMPLE_INTERVAL_MS        = 50;     // every 50 milliseconds
      const int          SAMPLE_DURATION_MS        = 5000;   // calibration lasts 5 seconds total

      enum CalibrationState
      {
         Init,
         Run,
         End
      }

      DataItemVehicle       vehicle;
      IDeviceAcceleromter   accelerometerProvider;
      IDeviceSound          soundProvider;
      CalibrationState      state;
      int                   timeMilliSeconds;
      MovingAverage         calibrationFilter;
      float                 calibration;            // m/s^2 acceleration noise
      PageMain              pageNavigation;
      AccelerometerRecorder accelerometerRecorder;

      // page head
      Image                 imgButtonBack;
      TapGestureRecognizer  tgrButtonBack;
      CustomLabel           lblCaption;
      Image                 imgButtonHelp;
      TapGestureRecognizer  tgrButtonHelp;

      // page body
      CustomLabel           lblHelp;
      Image                 imgCalibAnim1;
      Image                 imgCalibAnim2;
      Image                 imgCalibAnim3;
      Image                 imgCalibAnim4;
      CustomLabel           lblText;

      // page foot
      Button                btnOk;

      public PageCalibration(DataItemVehicle item, IDeviceAcceleromter accelerometer, IDeviceSound sound, PageMain parentPage)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         vehicle               = item;
         accelerometerProvider = accelerometer;
         soundProvider         = sound;
         state                 = CalibrationState.Init;
         timeMilliSeconds      = 0;
         calibrationFilter     = new MovingAverage(SAMPLE_DURATION_MS / SAMPLE_INTERVAL_MS);
         calibration           = 0;
         pageNavigation        = parentPage;
         accelerometerRecorder  = new AccelerometerRecorder(false); // false: do not use queue mode

         InitLayout();
      }

      public void Dispose() // IDisposable
      {
         imgButtonBack.Source = null;
         imgButtonBack.GestureRecognizers.RemoveAt(0);
         imgButtonBack = null;
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         tgrButtonBack = null;

         lblCaption = null;

         imgButtonHelp.Source = null;
         imgButtonHelp.GestureRecognizers.RemoveAt(0);
         imgButtonHelp = null;
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         tgrButtonHelp = null;

         lblHelp = null;

         imgCalibAnim1.Source = null;
         imgCalibAnim1.SizeChanged -= OnLayoutSizeChanged;
         imgCalibAnim1 = null;

         imgCalibAnim2.Source = null;
         imgCalibAnim2 = null;

         imgCalibAnim3.Source = null;
         imgCalibAnim3 = null;

         imgCalibAnim4.Source = null;
         imgCalibAnim4 = null;

         lblText.SizeChanged -= OnLayoutSizeChanged;
         lblText = null;

         btnOk.Image = null;
         btnOk.Clicked -= OnButtonOkClicked;
         btnOk = null;

         vehicle = null;
         accelerometerProvider = null;
         accelerometerRecorder = null;
         soundProvider = null;
         calibrationFilter = null;
         pageNavigation = null;
      }

      void InitLayout()
      {
         // page head

         imgButtonBack = new Image();
         imgButtonBack.Source = "@drawable/btn_back.png";
         tgrButtonBack = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);
         layout.Children.Add
         (
            imgButtonBack,
            Constraint.RelativeToParent(parent => parent.Width * 0.036),
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         lblCaption = new CustomLabel();
         lblCaption.Text = Localization.pageCalibrationHead;
         lblCaption.Size = CustomLabel.SIZE_CAPTION;
         lblCaption.TextColor = Color.FromHex("4D4C4A");
         layout.Children.Add
         (
            lblCaption,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - lblCaption.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.022)
         );

         imgButtonHelp = new Image();
         imgButtonHelp.Source = "@drawable/btn_help_profile.png";
         tgrButtonHelp = new TapGestureRecognizer();
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
         imgButtonHelp.GestureRecognizers.Add(tgrButtonHelp);
         layout.Children.Add
         (
            imgButtonHelp,
            Constraint.RelativeToParent(parent => parent.Width * 0.81),
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         // page body

         lblHelp = new CustomLabel();
         lblHelp.Text = Localization.pageCalibrationHelp;
         lblHelp.Size = CustomLabel.SIZE_MEDIUM;
         lblHelp.HorizontalTextAlignment = TextAlignment.Center;
         layout.Children.Add
         (
            lblHelp,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - lblHelp.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.18),
            Constraint.RelativeToParent((parent) => parent.Width * 0.8),
            Constraint.RelativeToParent((parent) => parent.Height * 0.4)
         );
         lblHelp.SizeChanged += OnLayoutSizeChanged;

         // add 4 images above each other, switch one after another visible
         // cheap solution: just switch image.source -> will result in flickering

         imgCalibAnim1 = new Image();
         imgCalibAnim1.Source = "@drawable/display_calibrate1.png";
         layout.Children.Add
         (
            imgCalibAnim1,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - imgCalibAnim1.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.56 - imgCalibAnim1.Height * 0.5)
         );
         imgCalibAnim1.IsVisible = true;
         imgCalibAnim1.SizeChanged += OnLayoutSizeChanged;

         imgCalibAnim2 = new Image();
         imgCalibAnim2.Source = "@drawable/display_calibrate2.png";
         layout.Children.Add
         (
            imgCalibAnim2,
            Constraint.RelativeToView(imgCalibAnim1, (p, v) => v.X),
            Constraint.RelativeToView(imgCalibAnim1, (p, v) => v.Y)
         );
         imgCalibAnim2.IsVisible = false;

         imgCalibAnim3 = new Image();
         imgCalibAnim3.Source = "@drawable/display_calibrate3.png";
         layout.Children.Add
         (
            imgCalibAnim3,
            Constraint.RelativeToView(imgCalibAnim1, (p, v) => v.X),
            Constraint.RelativeToView(imgCalibAnim1, (p, v) => v.Y)
         );
         imgCalibAnim3.IsVisible = false;

         imgCalibAnim4 = new Image();
         imgCalibAnim4.Source = "@drawable/display_calibrate4.png";
         layout.Children.Add
         (
            imgCalibAnim4,
            Constraint.RelativeToView(imgCalibAnim1, (p, v) => v.X),
            Constraint.RelativeToView(imgCalibAnim1, (p, v) => v.Y)
         );
         imgCalibAnim4.IsVisible = false;

         lblText = new CustomLabel();
         lblText.Text = Localization.pageCalibrationStart;
         lblText.Size = CustomLabel.SIZE_LARGE;
         layout.Children.Add
         (
            lblText,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - lblText.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.79)
         );
         lblText.SizeChanged += OnLayoutSizeChanged;

         // page foot

         btnOk = new Button();
         btnOk.Image = Localization.btn_calibration_run;
         btnOk.BackgroundColor = Color.Transparent;
         btnOk.Clicked += OnButtonOkClicked;
         layout.Children.Add
         (
            btnOk,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnOk.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height - btnOk.Height)
         );
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_CALIBRATION);
      }

      protected override bool OnBackButtonPressed()
      {
         ExitPage();

         // true = cancel event = prevent system default
         return true;
      }

      void OnButtonBackTapped(object sender, EventArgs args)
      {
         ExitPage();
      }

      void OnLayoutSizeChanged(object sender, EventArgs args)
      {
         layout.ForceLayout();
      }

      async void OnButtonHelpTapped(object sender, EventArgs args)
      {
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         await Navigation.PushAsync(new PageText(PageText.ContentType.Calibration));
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
      }

      void OnButtonOkClicked(object sender, EventArgs args)
      {
         switch (state)
         {
         case CalibrationState.Init:
            state = CalibrationState.Run;
            btnOk.IsEnabled = false;
            soundProvider.Play(SoundId.calibration);
            Device.StartTimer(TimeSpan.FromMilliseconds(SAMPLE_INTERVAL_MS), OnTimer);
            break;

         case CalibrationState.Run:
            break;

         case CalibrationState.End:
            ExitPage();
            break;
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
      }

      async void ExitPage()
      {
         btnOk.IsEnabled = false;

         bool success = false;

         if (state == CalibrationState.End)
         {
            if (GetCalibration() >= CALIBRATION_SUCCESS_LIMIT)
            {
               success = true;
               accelerometerRecorder.StoreAxisOffset(vehicle);
               vehicle.Calibration = GetCalibration();
               Database.GetInstance().SaveVehicle(vehicle);
               Debug.LogToFile("calibration stored to profile: " + vehicle.Calibration.ToString("0.00"));
            }
         }

         await Navigation.PopAsync();

         if ((pageNavigation != null) && success)
         {
            // at main-page user pressed run-button, but vehicle was not calibrated -> goto run-page now
            pageNavigation.ShowPageRun();
         }

         btnOk.IsEnabled = true;
      }

      bool OnTimer()
      {
         timeMilliSeconds += SAMPLE_INTERVAL_MS;

         if (timeMilliSeconds > SAMPLE_DURATION_MS)
         {
            btnOk.IsEnabled = true;
            state = CalibrationState.End;

            if (GetCalibration() < CALIBRATION_SUCCESS_LIMIT)
            {
               // not successful
               lblText.Text = Localization.pageCalibrationNotOk;
               lblText.TextColor = Color.FromHex("FF0000");
            }
            else
            {
               // successful
               lblText.Text = Localization.pageCalibrationOk;
               lblText.TextColor = Color.FromHex("00FF00");
            }

            btnOk.Image = Localization.btn_calibration_done;
            return false; // stop timer
         }

         var sensorXDelta = accelerometerProvider.GetXDelta();
         var sensorYDelta = accelerometerProvider.GetYDelta();
         var sensorZDelta = accelerometerProvider.GetZDelta();

         var noise = Math.Abs(sensorXDelta + sensorYDelta + sensorZDelta);

         calibration = calibrationFilter.Get(noise);

         if (Config.Calibration.LiveLabelEnabled)
         {
            lblText.Text = "" + GetCalibration().ToString("0.0") + " m/s²";
         }

         // animate, switch one image after another visible
         switch (timeMilliSeconds)
         {
         case 300:
            accelerometerProvider.SetListener(this);
            imgCalibAnim1.IsVisible = false;
            imgCalibAnim2.IsVisible = true;
            break;
         case 2200:
            imgCalibAnim2.IsVisible = false;
            imgCalibAnim3.IsVisible = true;
            break;
         case 3800:
            imgCalibAnim3.IsVisible = false;
            imgCalibAnim4.IsVisible = true;
            break;
         case 4500:
            accelerometerProvider.SetListener(null);
            break;
         }

         return true; // restart timer
      }

      float GetCalibration()
      {
         if (Config.Calibration.AlwaysSuccessEnabled) return 3;
         else                                         return (calibration * 10);
      }
   }
}
