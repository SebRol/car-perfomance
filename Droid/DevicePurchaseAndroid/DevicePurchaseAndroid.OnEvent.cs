using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

// partial class providing ON_EVENT code

using perf.Droid;
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DevicePurchaseAndroid))]
namespace perf.Droid
{
   public partial class DevicePurchaseAndroid : IDevicePurchase
   {
      public void OnActivityResult(int requestCode, Result resultCode, Intent data)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore result [requestCode, resultCode]: " + requestCode + ", " + resultCode);

         if (inAppBillingServiceConnection != null)
         {
            if (inAppBillingServiceConnection.BillingHandler != null)
            {
               inAppBillingServiceConnection.BillingHandler.HandleActivityResult(requestCode, resultCode, data);
            }
         }
      }

      void OnConnected()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         if (inAppBillingServiceConnection != null)
         {
            if (inAppBillingServiceConnection.BillingHandler != null)
            {
               inAppBillingServiceConnection.BillingHandler.OnUserCanceled                    += OnUserCanceled;
               inAppBillingServiceConnection.BillingHandler.OnGetProductsError                += OnGetProductsError;
               inAppBillingServiceConnection.BillingHandler.OnProductPurchased                += OnProductPurchased;
               inAppBillingServiceConnection.BillingHandler.OnPurchaseConsumed                += OnPurchaseConsumed;
               inAppBillingServiceConnection.BillingHandler.OnProductPurchasedError           += OnProductPurchasedError;
               inAppBillingServiceConnection.BillingHandler.OnPurchaseConsumedError           += OnPurchaseConsumedError;
               inAppBillingServiceConnection.BillingHandler.OnPurchaseFailedValidation        += OnPurchaseFailedValidation;
               inAppBillingServiceConnection.BillingHandler.OnInvalidOwnedItemsBundleReturned += OnInvalidOwnedItemsBundleReturned;
               inAppBillingServiceConnection.BillingHandler.QueryInventoryError               += OnQueryInventoryError;
               inAppBillingServiceConnection.BillingHandler.InAppBillingProcesingError        += OnInAppBillingProcessingError;
               inAppBillingServiceConnection.BillingHandler.BuyProductError                   += OnBuyProductError;

               LoadPurchases();
               NotifyListener(PurchaseStatus.Connected);
            }
         }
      }

      void OnDisconnected()
      {
         DeviceDebugAndroid.LogToFileMethodStatic();

         if (inAppBillingServiceConnection != null)
         {
            if (inAppBillingServiceConnection.BillingHandler != null)
            {
               inAppBillingServiceConnection.BillingHandler.OnUserCanceled                    -= OnUserCanceled;
               inAppBillingServiceConnection.BillingHandler.OnGetProductsError                -= OnGetProductsError;
               inAppBillingServiceConnection.BillingHandler.OnProductPurchased                -= OnProductPurchased;
               inAppBillingServiceConnection.BillingHandler.OnPurchaseConsumed                -= OnPurchaseConsumed;
               inAppBillingServiceConnection.BillingHandler.OnProductPurchasedError           -= OnProductPurchasedError;
               inAppBillingServiceConnection.BillingHandler.OnPurchaseConsumedError           -= OnPurchaseConsumedError;
               inAppBillingServiceConnection.BillingHandler.OnPurchaseFailedValidation        -= OnPurchaseFailedValidation;
               inAppBillingServiceConnection.BillingHandler.OnInvalidOwnedItemsBundleReturned -= OnInvalidOwnedItemsBundleReturned;
               inAppBillingServiceConnection.BillingHandler.QueryInventoryError               -= OnQueryInventoryError;
               inAppBillingServiceConnection.BillingHandler.InAppBillingProcesingError        -= OnInAppBillingProcessingError;
               inAppBillingServiceConnection.BillingHandler.BuyProductError                   -= OnBuyProductError;
            }

            inAppBillingServiceConnection.OnConnected         -= OnConnected;
            inAppBillingServiceConnection.OnInAppBillingError -= OnInAppBillingError;
            inAppBillingServiceConnection.OnDisconnected      -= OnDisconnected;

            NotifyListener(PurchaseStatus.Disconnected);
         }
      }

      void OnInAppBillingError(InAppBillingErrorType error, string message)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore *** error *** " + error + " // message: " + message);
      }

      void OnGetProductsError(int responseCode, Bundle ownedItems)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore responseCode: " + responseCode + " // ownedItems: " + ownedItems);
      }

      void OnProductPurchasedError(int responseCode, string sku)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore responseCode: " + responseCode + " // sku: " + sku);
      }

      void OnPurchaseConsumedError(int responseCode, string token)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore responseCode: " + responseCode + " // token: " + token);
      }

      void OnInvalidOwnedItemsBundleReturned(Bundle ownedItems)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore ownedItems: " + ownedItems);
      }

      void OnPurchaseFailedValidation(Purchase purchase, string purchaseData, string purchaseSignature)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore purchase: " + purchase + " // purchaseData: " + purchaseData + " // purchaseSignature: " + purchaseSignature);
      }

      void OnUserCanceled()
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore user cancaled");
         NotifyListener(PurchaseStatus.Cancelled);
      }

      void OnProductPurchased(int response, Purchase purchase, string purchaseData, string purchaseSignature)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore response: " + response + " // purchase: " + purchase + " // purchaseData: " + purchaseData + " // purchaseSignature: " + purchaseSignature);

         LoadPurchases();
         NotifyListener(PurchaseStatus.Purchased);
      }

      void OnPurchaseConsumed(string token)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore token: " + token);

         LoadPurchases();
         NotifyListener(PurchaseStatus.Consumed);
      }

      void OnQueryInventoryError(int responseCode, Bundle skuDetails)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore responseCode: " + responseCode + " // skuDetails: " + skuDetails);
      }

      void OnInAppBillingProcessingError(string message)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore message: " + message);
      }

      void OnBuyProductError(int responseCode, string sku)
      {
         DeviceDebugAndroid.LogToFileMethodStatic("appstore responseCode: " + responseCode + " // sku: " + sku);

         NotifyListener(PurchaseStatus.Cancelled);
      }
   }
}
