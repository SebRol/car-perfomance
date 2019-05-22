using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

// https://components.xamarin.com/gettingstarted/xamarin.inappbilling
// https://github.com/flagbug/SaneInAppBillingHandler/blob/master/SaneInAppBillingHandler/SaneInAppBillingHandler.cs

// partial class providing INTERFACE code

using perf.Droid;
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DevicePurchaseAndroid))]
namespace perf.Droid
{
   public partial class DevicePurchaseAndroid : IDevicePurchase
   {
      public void SetListener(IPurchaseListener listener) // IDevicePurchase
      {
         this.listener = listener;

         if (inAppBillingServiceConnection != null)
         {
            if ((inAppBillingServiceConnection.Connected      == true) &&
                (inAppBillingServiceConnection.BillingHandler != null ))
            {
               NotifyListener(PurchaseStatus.Connected);
            }
         }
      }

      public async void Purchase(PurchaseType purchaseType) // IDevicePurchase
      {
         Log("purchase product: " + purchaseType);

         if (inAppBillingServiceConnection == null)
         {
            Log("*** error *** service not created ***");
            return;
         }

         if ((inAppBillingServiceConnection.Connected      == false) ||
             (inAppBillingServiceConnection.BillingHandler == null ))
         {
            Log("*** error *** service not connected ***");
            return;
         }

         IList<Product> products = null;
         string         sku     = GetProductSku(purchaseType);
         List<string>   skuList = new List<string>{sku};
         int            timeout = 5000; // milliseconds

         Log("query inventory for sku: " + skuList[0]);

         Task<IList<Product>> task = inAppBillingServiceConnection.BillingHandler.QueryInventoryAsync(skuList, ItemType.Product);
         if (await Task.WhenAny(task, Task.Delay(timeout)) == task) 
         {
            // Task completed within timeout
            // Consider that the task may have faulted or been canceled
            // We re-await the task so that any exceptions/cancellation is rethrown
            products = await task;
            Log("query inventory task returned");
         } 
         else 
         { 
            // timeout/cancellation logic
            Log("*** error *** query inventory task timeout ***");
         }

         if (products != null)
         {
            //Log(products);

            if (products.Count == 1) // we only queried one product
            {
               Log("buy product: " + products[0].ProductId);
               inAppBillingServiceConnection.BillingHandler.BuyProduct(products[0]);
               // the activity goes to sleep at this point, because play-store takes over the display
            }
            else
            {
               Log("*** error *** product inventory is empty *** ");
            }
         }
         else
         {
            Log("*** error *** product inventory is empty (product already bought?) *** ");
         }
      }

      public void Consume(PurchaseType purchaseType) // IDevicePurchase
      {
         Log("consume product: " + purchaseType);

         if (inAppBillingServiceConnection == null)
         {
            Log("*** error *** service not created ***");
            return;
         }

         if ((inAppBillingServiceConnection.Connected      == false) ||
             (inAppBillingServiceConnection.BillingHandler == null ))
         {
            Log("*** error *** service not connected ***");
            return;
         }

         if (IsPurchased(purchaseType) == false)
         {
            Log("*** error *** purchase was not bought before ***");
            return;
         }

         Purchase purchase = GetPurchase(purchaseType);

         if (purchase != null)
         {
            bool result = inAppBillingServiceConnection.BillingHandler.ConsumePurchase(purchase);
            Log("consume product result: " + result);
         }
         else
         {
            Log("*** error *** no purchase found for type: " + purchaseType + " ***");
         }
      }

      public bool IsPurchased(PurchaseType purchaseType) // IDevicePurchase
      {
         bool result = false;

         if (purchases != null)
         {
            foreach (Purchase p in purchases)
            {
               string packageName = GetPackageName();

               if (p.PackageName.Contains(packageName))
               {
                  string sku = GetProductSku(purchaseType);

                  if (p.ProductId.Contains(sku))
                  {
                     result = true;
                  }
               }
            }
         }
         else
         {
            Log("*** error *** state of purchases unknown ***");
         }

         return result;
      }

      public bool IsConnected() // IDevicePurchase
      {
         bool result = true;

         if (inAppBillingServiceConnection == null)
         {
            result = false;
         }
         else
         {
            if ((inAppBillingServiceConnection.Connected      == false) ||
                (inAppBillingServiceConnection.BillingHandler == null ))
            {
               result = false;
            }
         }

         return result;
      }

      void NotifyListener(PurchaseStatus status)
      {
         if (listener != null) listener.OnPurchaseStatus(status);
      }
   }
}
