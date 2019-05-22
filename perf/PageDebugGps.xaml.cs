using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageDebugGps : ContentPage, IGpsListener
   {
      IDeviceGps gpsProvider;
      uint heartBeat;
      double startLatitude;
      double startLongitude;

      public PageDebugGps(IDeviceGps gps)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         Title = "Debug Location";
         NavigationPage.SetHasNavigationBar(this, false);

         gpsProvider = gps;

         Device.StartTimer(TimeSpan.FromSeconds(1), () =>
         {
            heartBeat++;
            labelHeartBeat.Text = "Heartbeat: " + heartBeat;
            if (heartBeat % 5 == 0) OnGpsStatusUpdate();
            return true; // restart timer
         });
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();
         Debug.LogToFileMethod();

         if (gpsProvider != null)
         {
            gpsProvider.SetListener(this);
         }
      }

      public void OnSleep()
      {
         Debug.LogToFileMethod();
         if (gpsProvider != null) gpsProvider.SetListener(null);
      }

      public void OnResume()
      {
         Debug.LogToFileMethod();
         if (gpsProvider != null) gpsProvider.SetListener(this);
      }

      public void OnGpsLocationUpdate()
      {
         labelLatitude.Text                         = "Latitude: "                          + gpsProvider.GetLatitude();
         labelLongitude.Text                        = "Longitude: "                         + gpsProvider.GetLongitude();
         labelSpeed.Text                            = "Speed: "                             + gpsProvider.GetSpeed();
         labelDistance.Text                         = "Distance: "                          + gpsProvider.GetDistance(startLatitude, startLongitude, gpsProvider.GetLatitude(), gpsProvider.GetLongitude());

         startLatitude = gpsProvider.GetLatitude();
         startLongitude = gpsProvider.GetLongitude();
      }

      public void OnGpsStatusUpdate()
      {
         labelProviderStatus.Text                   = "GpsStatus: "                         + gpsProvider.GetStatus();
         labelGpsStatusSatellitesVisible.Text       = "GpsStatusSatellitesVisible: "        + gpsProvider.GetSatellitesVisible();
         labelGpsStatusSatellitesUsedInFix.Text     = "GpsStatusSatellitesVisibleWithFix: " + gpsProvider.GetSatellitesVisibleWithFix();
         labelGpsStatusSatellitesWithEphemeris.Text = "GpsStatusSatellitesWithEphemeris: "  + gpsProvider.GetSatellitesWithEphemeris();
         labelGpsStatusSatellitesWithAlmanac.Text   = "GpsStatusSatellitesWithAlmanac: "    + gpsProvider.GetSatellitesWithAlmanac();
         labelGpsStatusTimeToFirstFix.Text          = "GpsStatusTimeToFirstFix: "           + gpsProvider.GetTimeToFirstFix() / 1000 + " seconds";
      }
   }
}
