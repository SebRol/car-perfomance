using System;

namespace perf
{
   // listener of platform IN_APP_BILLING function

   public enum PurchaseStatus
   {
      Connected,     // service is up and running, product inventory and purchases are fetched from server
      Disconnected,  // service is down
      Purchased,     // product was bought
      Consumed,      // product was consumed -> can be purchased again
      Cancelled      // purchase process interrupted
   }

   public interface IPurchaseListener
   {
      // called on change of purchase state
      // called from app-store thread -> do not update views (use UI thread instead)
      void OnPurchaseStatus(PurchaseStatus status);

      // called on new log message regarding purchase subsystem
      // called from app-store thread -> do not update views (use UI thread instead)
      void OnPurchaseLog(string message);
   }
}
