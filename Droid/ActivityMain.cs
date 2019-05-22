using Java.Lang;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Xamarin.Forms.Platform.Android;

namespace perf.Droid
{
   [Activity(Label                = "@string/applicationName", 
             Icon                 = "@drawable/icon",
             Theme                = "@style/Main",
             ScreenOrientation    = ScreenOrientation.Portrait,
             ConfigurationChanges = ConfigChanges.ScreenSize)]
   public class ActivityMain : FormsAppCompatActivity, Thread.IUncaughtExceptionHandler
   {
      private static DevicePurchaseAndroid devicePurchseAndroid;
      public  static IDevicePurchase       PlayStore { get { return devicePurchseAndroid; } }

      protected override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);
         DeviceDebugAndroid.LogToFileMethodStatic();

         // setup handler for uncaught exceptions
         Thread.DefaultUncaughtExceptionHandler = this;

         // request screen is always on
         Window.AddFlags(WindowManagerFlags.KeepScreenOn);

         //UncaughtException(null, new Exception());

         // our google play store
         devicePurchseAndroid = new DevicePurchaseAndroid();

         // start xamarin.forms app
         LoadApplication(new App());
      }

      // activity is visible
      protected override void OnStart()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         base.OnStart();

         devicePurchseAndroid.SetActivity(this);

         // connect to google play store
         devicePurchseAndroid.Connect();
      }

      // activity is not visible or being distroyed by the system
      protected override void OnStop()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         if (Config.Purchase.ConsumeOnExitEnabled)
         {
            base.OnStop();
            // disconnect from google play store
            devicePurchseAndroid.Disconnect();   
         }
         else
         {
            // disconnect from google play store
            devicePurchseAndroid.Disconnect();
            base.OnStop();
         }
      }

      // called by the in-app-purchase service (google play store)
      protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
      {
         // forward from google play store to our app
         devicePurchseAndroid.OnActivityResult(requestCode, resultCode, data);
      }

      public void UncaughtException(Thread thread, Throwable ex)
      {
         #if false
         Intent intent = new Intent(Forms.Context, typeof(ActivityCrash));
         intent.SetFlags(ActivityFlags.NewTask); // required when starting from application
         intent.SetFlags(ActivityFlags.DebugLogResolution); // required when starting from application
         intent.PutExtra("text", GetExceptionString(ex));
         Android.Util.Log.Debug("     ****       ", "startung intent");
         //StartActivity(intent);
         Forms.Context.StartActivity(intent);
         Android.Util.Log.Debug("     ****       ", "intent started");
         #else
         DeviceCrashReportAndroid.sendMail(ex, GetContactMail(), GetApplicationName());
         #endif

         // kill this activity
         Finish();
      }

      string GetApplicationName()
      {
         return ApplicationContext.Resources.GetString(Resource.String.applicationName);
      }

      string GetContactMail()
      {
         return ApplicationContext.Resources.GetString(Resource.String.contactMail);
      }
   }
}
