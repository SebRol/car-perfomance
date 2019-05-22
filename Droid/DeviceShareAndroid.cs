using Android.Content;
using System.Threading.Tasks;

// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
using perf.Droid;
using Android.Net;
using Java.IO;
[assembly: Xamarin.Forms.Dependency(typeof (DeviceShareAndroid))]

namespace perf.Droid
{
   public class DeviceShareAndroid : IDeviceShare
   {
// This async method lacks 'await' operators and will run synchronously. 
// Consider using the 'await' operator to await non-blocking API calls, 
// or 'await Task.Run(...)' to do CPU-bound work on a background thread.
#pragma warning disable 1998

      public async Task Share(string title = null, string subject = null, string text = null, string imagePath = null)
      {
         var intent        = new Intent(Intent.ActionSend);
         var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);

         intent.PutExtra(Intent.ExtraSubject, subject ?? string.Empty);
         intent.PutExtra(Intent.ExtraText,    text    ?? string.Empty);

         if (imagePath == null)
         {
            intent.SetType("text/plain");
         }
         else
         {
            intent.SetType("application/image");
            using (File imageFile = new File(imagePath))
            {
               using (Uri imageUri = Uri.FromFile(imageFile))
               {               
                  intent.PutExtra(Intent.ExtraStream, imageUri);
               }
            }
         }

         intent.SetFlags(ActivityFlags.ClearTop);
         intent.SetFlags(ActivityFlags.NewTask);

         chooserIntent.SetFlags(ActivityFlags.ClearTop);
         chooserIntent.SetFlags(ActivityFlags.NewTask);

         Android.App.Application.Context.StartActivity(chooserIntent);
      }
#pragma warning restore 1998
   }
}
