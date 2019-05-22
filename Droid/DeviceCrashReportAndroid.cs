using System;
using Java.Lang;
using Android.OS;
using Android.Content;
using Xamarin.Forms;

namespace perf.Droid
{
   public class DeviceCrashReportAndroid
   {
      private DeviceCrashReportAndroid()
      {
      }

      public static void sendMail(Throwable ex, string mailAddress, string appName)
      {
         var intent = new Intent(Intent.ActionSend);
         intent.PutExtra(Intent.ExtraEmail, new [] {mailAddress});
         //intent.PutExtra(Intent.ExtraCc, new [] {"todo"});
         intent.PutExtra(Intent.ExtraSubject, appName + ": Crash Report");
         intent.PutExtra(Intent.ExtraText, GetExceptionString(ex, appName));
         intent.PutExtra(Intent.ExtraHtmlText, true);
         intent.SetType("message/rfc822");
         Forms.Context.StartActivity(Intent.CreateChooser(intent, "Send Crash Report"));
      }

      static string GetExceptionString(Throwable ex, string appName)
      {
         string result = "";
         string newLine = System.Environment.NewLine;

         string exception = ex.ToString();
         var nowDate = string.Format("{0:dd-MMMM-yyyy}", DateTime.Now);
         var nowTime = string.Format("{0:HH-mm-ss}", DateTime.Now);
         string device = "";

         //device += "VERSION.BaseOs: "           + Build.VERSION.BaseOs         + newLine;
         device += "VERSION.Codename: "         + Build.VERSION.Codename       + newLine;
         device += "VERSION.Incremental: "      + Build.VERSION.Incremental    + newLine;
         //device += "VERSION.PreviewSdkInt: "  + Build.VERSION.PreviewSdkInt  + newLine;
         device += "VERSION.Release: "          + Build.VERSION.Release        + newLine;
         device += "VERSION.Sdk: "              + Build.VERSION.Sdk            + newLine;
         device += "VERSION.SdkInt: "           + Build.VERSION.SdkInt         + newLine;
         //device += "VERSION.SecurityPatch: "  + Build.VERSION.SecurityPatch  + newLine;
         device += "Board: "                    + Build.Board                  + newLine;
         device += "Bootloader: "               + Build.Bootloader             + newLine;
         device += "Brand: "                    + Build.Brand                  + newLine;
         device += "CpuAbi: "                   + Build.CpuAbi                 + newLine;
         device += "CpuAbi2: "                  + Build.CpuAbi2                + newLine;
         device += "Device: "                   + Build.Device                 + newLine;
         device += "Display: "                  + Build.Display                + newLine;
         device += "Fingerprint: "              + Build.Fingerprint            + newLine;
         device += "Hardware: "                 + Build.Hardware               + newLine;
         device += "Host: "                     + Build.Host                   + newLine;
         device += "Id: "                       + Build.Id                     + newLine;
         device += "Manufacturer: "             + Build.Manufacturer           + newLine;
         device += "Model: "                    + Build.Model                  + newLine;
         device += "Product: "                  + Build.Product                + newLine;
         device += "Radio: "                    + Build.Radio                  + newLine;
         device += "RadioVersion: "             + Build.RadioVersion           + newLine;
         device += "Serial: "                   + Build.Serial                 + newLine;
         device += "Tags: "                     + Build.Tags                   + newLine;
         device += "Time: "                     + Build.Time                   + newLine;
         device += "Type: "                     + Build.Type                   + newLine;
         device += "Unknown: "                  + Build.Unknown                + newLine;
         device += "User: "                     + Build.User                   + newLine;

         result = "app: " + appName + newLine + 
                  "date: " + nowDate + newLine + 
                  "time: " + nowTime + newLine + 
                  newLine +
                  "exception: " + newLine + 
                  exception + newLine +
                  "device: " + newLine + device +
                  newLine + 
                  new DeviceInfoAndroid() + newLine;

         return result;
      }
   }
}
