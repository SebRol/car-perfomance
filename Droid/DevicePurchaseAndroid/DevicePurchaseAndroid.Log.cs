using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

// https://components.xamarin.com/gettingstarted/xamarin.inappbilling
// https://github.com/flagbug/SaneInAppBillingHandler/blob/master/SaneInAppBillingHandler/SaneInAppBillingHandler.cs

// partial class providing MAIN code

using perf.Droid;
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DevicePurchaseAndroid))]
namespace perf.Droid
{
   public partial class DevicePurchaseAndroid : IDevicePurchase
   {
      void Log(string s = "",
               [System.Runtime.CompilerServices.CallerMemberName] string methodName     = "",
               [System.Runtime.CompilerServices.CallerFilePath]   string sourceFilePath = "")
      {
         if (s != null)
         {
            if (Config.Purchase.ListenerLogEnabled)
            {
               if (listener != null) listener.OnPurchaseLog(s);
            }

            DeviceDebugAndroid.LogToFileMethodStatic("appstore " + s, methodName, sourceFilePath);
         }
      }

      void Log(IList<Product> products)
      {
         if (products != null)
         {
            foreach (var p in products)
            {
               Log("product type: "                + p.Type);
               Log("product title: "               + p.Title);
               Log("product description: "         + p.Description);
               Log("product product id: "          + p.ProductId);
               Log("product price: "               + p.Price);
               Log("product price currency code: " + p.Price_Currency_Code);
               Log("product price amount micros: " + p.Price_Amount_Micros);
               Log("---");
            }
         }
      }
   }
}
