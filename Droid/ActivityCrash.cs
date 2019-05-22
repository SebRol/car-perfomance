using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace perf.Droid
{
   [Activity(Label                = "@string/applicationName", 
             Icon                 = "@drawable/icon",
             Theme                = "@style/Crash",
             ScreenOrientation    = ScreenOrientation.Portrait,
             ConfigurationChanges = ConfigChanges.ScreenSize)]
   public class ActivityCrash : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
   {
      const string TAG = "         *******          ";

      protected override void OnResume()
      {
         base.OnResume();
         Android.Util.Log.Debug(TAG, "set content view");
         SetContentView(Resource.Drawable.layout_crash);

         Intent i = Intent;
         if (i != null)
         {
            Android.Util.Log.Debug(TAG, "intend != null");
            Bundle b = i.Extras;
            if (b != null)
            {
               Android.Util.Log.Debug(TAG, "bundle != null");
               string text = b.GetString("text");
               if (text != null)
               {
                  Android.Util.Log.Debug(TAG, "text != null");
                  TextView tv = FindViewById<TextView>(Resource.Id.text);
                  if (tv != null)
                  {
                     Android.Util.Log.Debug(TAG, "textview != null");
                     tv.Text = text;
                  }
               }
            }
         }
      }
   }
}
