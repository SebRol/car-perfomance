using System;

namespace perf
{
   // abstracts platform IN_APP_BILLING function

   public enum PurchaseType
   {
      Dummy,
      Cancel,
      Refund,
      Unavailable,
      Development,
      Unlocker
   }

   public interface IDevicePurchase
   {
      // listener is called on new data available
      void SetListener(IPurchaseListener listener);

      // buy given product
      void Purchase(PurchaseType purchaseType);

      // consume given product
      void Consume(PurchaseType purchaseType);

      // true if user has bought the unlocker
      bool IsPurchased(PurchaseType purchaseType);

      // true if billing service is available
      bool IsConnected();
   }
}
