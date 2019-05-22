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
      IPurchaseListener             listener;
      Activity                      activity;
      InAppBillingServiceConnection inAppBillingServiceConnection;
      IList<Purchase>               purchases;

      public DevicePurchaseAndroid()
      {
      }

      public void SetActivity(Activity activity)
      {
         this.activity = activity;
      }

      public void Connect()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         if (activity != null)
         {
            inAppBillingServiceConnection = new InAppBillingServiceConnection(activity, GetPublicKey());

            // below events are unregistered in OnDisconnect()
            inAppBillingServiceConnection.OnConnected         += OnConnected;
            inAppBillingServiceConnection.OnInAppBillingError += OnInAppBillingError;
            inAppBillingServiceConnection.OnDisconnected      += OnDisconnected;

            inAppBillingServiceConnection.Connect();
         }
      }

      public void Disconnect()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         if (inAppBillingServiceConnection != null)
         {
            if (inAppBillingServiceConnection.Connected)
            {
               inAppBillingServiceConnection.Disconnect();
            }
         }
      }

      Purchase GetPurchase(PurchaseType purchaseType)
      {
         Purchase result = null;

         if (purchases != null)
         {
            string sku = GetProductSku(purchaseType);

            foreach (Purchase p in purchases)
            {
               if (p.ProductId.Contains(sku))
               {
                  result = p;
                  break;
               }
            }
         }

         return result;
      }

      void LoadPurchases()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         if (purchases != null) purchases.Clear();
         else                   purchases = new List<Purchase>();

         if (inAppBillingServiceConnection != null)
         {
            if (inAppBillingServiceConnection.BillingHandler != null)
            {
               purchases = inAppBillingServiceConnection.BillingHandler.GetPurchases(ItemType.Product);
            }
         }
      }

      string GetProductSku(PurchaseType purchaseType)
      {
         string result;

         switch (purchaseType)
         {
            case PurchaseType.Dummy:
            result = ReservedTestProductIDs.Purchased;
            break;

            case PurchaseType.Cancel:
            result = ReservedTestProductIDs.Canceled;
            break;

            case PurchaseType.Refund:
            result = ReservedTestProductIDs.Refunded;
            break;

            case PurchaseType.Unavailable:
            result = ReservedTestProductIDs.Unavailable;
            break;

            case PurchaseType.Development:
            result = GetDevelopmentSku();
            break;

            case PurchaseType.Unlocker:
            result = GetUnlockerSku();
            break;

            default:
            result = "empty";
            break;
         }

         return result;
      }

      List<string> GetProductSkuAll()
      {
         List<string> result = new List<string>();

         result.Add(GetProductSku(PurchaseType.Dummy));
         result.Add(GetProductSku(PurchaseType.Development));
         result.Add(GetProductSku(PurchaseType.Unlocker));

         return result;
      }
   }
}
