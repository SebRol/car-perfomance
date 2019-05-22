using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageDebugAccelerometer : ContentPage, IAcceleromterListener
   {
      IDeviceAcceleromter accelerometerProvider;
      uint heartBeat;

      public PageDebugAccelerometer(IDeviceAcceleromter accelerometer)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasNavigationBar(this, false);

         Title = "Debug Accelerometer";

         accelerometerProvider = accelerometer;

         Device.StartTimer(TimeSpan.FromSeconds(1), () =>
         {
            heartBeat++;
            labelHeartBeat.Text = "Heartbeat: " + heartBeat;
            return true; // restart timer
         });
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Debug.LogToFileMethod();

         if (accelerometerProvider != null)
         {
            accelerometerProvider.SetListener(this);

            labelForceDetectLimit.Text = "ForceDetectLimit: " + sliderForceDetectLimit.Value;
            accelerometerProvider.SetForceDetectLimit((float)sliderForceDetectLimit.Value);

            sliderForceDetectLimit.ValueChanged += (object sender, ValueChangedEventArgs e) =>
            {
               labelForceDetectLimit.Text = "ForceDetectLimit: " + e.NewValue;
               accelerometerProvider.SetForceDetectLimit((float)sliderForceDetectLimit.Value);
            };

            labelTimeDetectLimit.Text = "TimeDetectLimit: " + sliderTimeDetectLimit.Value;
            accelerometerProvider.SetTimeDetectLimit((long)sliderTimeDetectLimit.Value);

            sliderTimeDetectLimit.ValueChanged += (object sender, ValueChangedEventArgs e) =>
            {
               labelTimeDetectLimit.Text = "TimeDetectLimit: " + e.NewValue;
               accelerometerProvider.SetTimeDetectLimit((long)sliderTimeDetectLimit.Value);
            };

            labelTimeUpdateInterval.Text = "UpdateInterval: " + sliderTimeUpdateInterval.Value;
            accelerometerProvider.SetTimeUpdateInterval((long)sliderTimeUpdateInterval.Value);

            sliderTimeUpdateInterval.ValueChanged += (object sender, ValueChangedEventArgs e) =>
            {
               labelTimeUpdateInterval.Text = "UpdateInterval: " + e.NewValue;
               accelerometerProvider.SetTimeUpdateInterval((long)sliderTimeUpdateInterval.Value);
            };

            labelLaunchDetected.Text = "";
         }
      }

      public void OnSleep()
      {
         Debug.LogToFileMethod();
         if (accelerometerProvider != null) accelerometerProvider.SetListener(null);
      }

      public void OnResume()
      {
         Debug.LogToFileMethod();
         if (accelerometerProvider != null) accelerometerProvider.SetListener(this);
      }

      public void OnAcceleromterUpdate() // IAcceleromterListener
      {
         labelAcceleromterStatus.Text      = "AccelerometerStatus: "      + accelerometerProvider.GetStatus();
         labelAcceleromterValueX.Text      = "AccelerometerValueX: "      + accelerometerProvider.GetX();
         labelAcceleromterValueY.Text      = "AccelerometerValueY: "      + accelerometerProvider.GetY();
         labelAcceleromterValueZ.Text      = "AccelerometerValueZ: "      + accelerometerProvider.GetZ();
         labelAcceleromterValueXDelta.Text = "AccelerometerValueXDelta: " + accelerometerProvider.GetXDelta();
         labelAcceleromterValueYDelta.Text = "AccelerometerValueYDelta: " + accelerometerProvider.GetYDelta();
         labelAcceleromterValueZDelta.Text = "AccelerometerValueZDelta: " + accelerometerProvider.GetZDelta();
         labelAcceleromterSpeed.Text       = "AcceleromterSpeed: "        + accelerometerProvider.GetSpeed();
         labelAcceleromterInfo.Text        = "AccelerometerInfo: "        + accelerometerProvider.GetInfo();
      }

      public void OnAcceleromterLaunchDetected() // IAcceleromterListener
      {
         Debug.LogToFileMethod();
         if (labelLaunchDetected.Text.Equals(""))
         {
            labelLaunchDetected.Text = "*** Launch Detected ***";

            Device.StartTimer(System.TimeSpan.FromSeconds(1), () =>
            {
               labelLaunchDetected.Text = "";
               return false; // stop timer
            });
         }
      }
   }
}
