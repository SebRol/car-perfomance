using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

// partial class providing SECURITY code

using perf.Droid;
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DevicePurchaseAndroid))]
namespace perf.Droid
{
   public partial class DevicePurchaseAndroid : IDevicePurchase
   {
      string GetPublicKey()
      {
         // License key for this application. Base64-encoded RSA public key.
         string result = Security.Unify
         (
            new string[]
            {
               "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret",
               "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret",
               "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret",
               "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret",
               "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret",
               "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret"
            },
            new int[]
            {
               44-1, 11-1, 40-1, 71-1, 50-1, 21-1, 43-1, 29-1, 49-1, 56-1, 22-1, 76-1, 78-1, 14-1, 24-1, 61-1, 41-1,
               28-1, 55-1, 1-1, 73-1, 48-1, 15-1, 58-1, 74-1, 57-1, 16-1, 54-1, 5-1, 81-1, 19-1, 4-1, 9-1, 52-1, 32-1,
               31-1, 25-1, 42-1, 13-1, 39-1, 26-1, 79-1, 17-1, 6-1, 72-1, 63-1, 34-1, 3-1, 60-1, 20-1, 12-1, 45-1,
               36-1, 2-1, 64-1, 68-1, 7-1, 53-1, 47-1, 27-1, 18-1, 65-1, 37-1, 59-1, 62-1, 51-1, 66-1, 10-1, 38-1,
               80-1, 69-1, 67-1, 30-1, 35-1, 8-1, 70-1, 46-1, 33-1, 75-1, 77-1, 23-1
            }
         );

         return result;
      }

      string GetPackageName()
      {
         string result = Security.Unify
         (
            //             0         1         2         3         4         5         6         7         8
            new string[] { "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret" },
            new int[]    {   2-1,  3-1,   9-1,     5-1,      8-1,   7-1,    4-1,   1-1,   6-1 }
         );

         return result;
      }

      string GetDevelopmentSku()
      {
         string result = Security.Unify
         (
            new string[] { "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret" },
            new int[]    { 2-1, 7-1, 5-1, 1-1, 3-1, 6-1, 8-1, 9-1, 10-1, 4-1, 11-1 }
         );

         return result;
      }

      string GetUnlockerSku()
      {
         string result = Security.Unify
         (
            new string[] { "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret", "secret" },
            new int[]    {      3,     2,    9,      1,      6,    8,      7,    0,    5,     10,     4 }
         );

         return result;
      }
   }
}
