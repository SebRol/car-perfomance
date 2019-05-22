using System;
using Xamarin.Forms;

namespace perf
{
   public class AppStore : IDevicePurchase
   {
      static AppStore instance;
      IDevicePurchase purchaseProvider;
      Overlay         overlay;

      private AppStore()
      {
         #if __IOS__
         purchaseProvider = NOT IMPLEMENTED YET   
         #else
         #if __ANDROID__
         purchaseProvider = Droid.ActivityMain.PlayStore;
         #else
         // WinPhone
         purchaseProvider = NOT IMPLEMENTED YET
         #endif
         #endif
      }

      public static AppStore Instance
      { 
         get
         {
            if (instance == null)
            {
               instance = new AppStore();
            }

            return instance;
         }
      }

      public static void Dispose()
      {
         instance = null;
      }

      public void SetListener(IPurchaseListener listener) // IDevicePurchase
      {
         purchaseProvider.SetListener(listener);
      }

      public void Purchase(PurchaseType purchaseType) // IDevicePurchase
      {
         purchaseProvider.Purchase(purchaseType);
      }

      public void Consume(PurchaseType purchaseType) // IDevicePurchase
      {
         purchaseProvider.Consume(purchaseType);
      }
      
      public bool IsPurchased(PurchaseType purchaseType) // IDevicePurchase
      {
         return purchaseProvider.IsPurchased(purchaseType);
      }

      public bool IsConnected() // IDevicePurchase
      {
         return purchaseProvider.IsConnected();
      }

      public void Purchase()
      {
         Purchase(Config.Purchase.ProductSku);
      }

      public void Consume()
      {
         if (IsPurchased() == true)
         {
            Consume(Config.Purchase.ProductSku);
         }
      }
      
      public bool IsPurchased()
      {
         bool result = true;

         if (Config.Purchase.Enabled)
         {
            result = IsPurchased(Config.Purchase.ProductSku);
         }

         return result;
      }

      // start the app-store if not already purchased
      public bool IsAppStoreShown(RelativeLayout layout)
      {
         bool result = false;

         if (Config.Purchase.Enabled)
         {
            if (IsConnected() == false)
            {
               result = true;
               overlay = new OverlayPurchaseError(layout);
            }
            else if (IsPurchased() == false)
            {
               result = true;
               overlay = new OverlayPurchase(layout);
            }
         }

         return result;
      }

      // inhibit page navigation if appstore overlay is visible
      public bool OnBackButtonPressed()
      {
         // true  -> let overlay handle event
         // false -> let system  handle event
         bool result = false; 

         if (overlay != null)
         {
            result = overlay.OnBackButtonPressed(); 
         }

         return result;
      }
   }
}
