using Java.Lang;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace perf.Droid
{
   [Activity(Label                = "@string/applicationName", 
             Icon                 = "@drawable/icon",
             Theme                = "@style/Splash",
             MainLauncher         = true,
             NoHistory            = true,
             ScreenOrientation    = ScreenOrientation.Portrait,
             ConfigurationChanges = ConfigChanges.ScreenSize)]
   public class ActivitySplash : FormsAppCompatActivity
   {
      protected override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

#if DEBUG
         Config.EnableDebugBuild();
#else
         if (Config.IsConfigForReleaseBuildValid() == false)
         {
            DeviceCrashReportAndroid.sendMail(new Throwable("Release build: Config check not passed"), 
                                              ApplicationContext.Resources.GetString(Resource.String.applicationName), 
                                              ApplicationContext.Resources.GetString(Resource.String.contactMail));
         }
#endif

         DeviceDebugAndroid.LogToFileMethodStatic();

         // init xamarin.forms framework in a new backgound task, splash stays on screen
         Task task = new Task
         (
            delegate
            {
               Forms.Init(this, savedInstanceState);
               OxyPlot.Xamarin.Forms.Platform.Android.Forms.Init();
            }
         );
         
         task.Start();
      }


      protected override void OnStart()
      {
         base.OnStart();
         DeviceDebugAndroid.LogToFileMethodStatic();

         // switch to the main app in a new backgound task
         Task task = new Task
         (
            () => StartActivity(typeof(ActivityMain))
         );
         
         task.Start();
      }
   }
}
