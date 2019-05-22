using System;
using Xamarin.Forms;

namespace perf
{
   public class OverlayPurchase : Overlay, IPurchaseListener
   {
      TransparentButton btnAppStore;

      public OverlayPurchase(RelativeLayout parentLayout) : base(parentLayout)
      {
         Debug.LogToFileMethod();
         Analytics.TrackPage(Analytics.PAGE_PURCHASE);

         // shadow to darken display
         imgOverlay               = new Image();
         imgOverlay.Source        = "@drawable/ovr_shadow.png";
         imgOverlay.SizeChanged  += OnLayoutSizeChanged;
         imgOverlay.Aspect        = Aspect.Fill;
         imgOverlay.WidthRequest  = layout.Width;
         imgOverlay.HeightRequest = layout.Height;

         // app store button message
         imgMessage              = new Image();
         imgMessage.Source       = Localization.ovr_purchase;
         imgMessage.SizeChanged += OnLayoutSizeChanged;

         // app store button
         btnAppStore              = new TransparentButton();
         btnAppStore.SizeChanged += OnLayoutSizeChanged;
         btnAppStore.Clicked     += OnAppStoreClicked;

         // on touch outside app store button -> cancel
         tgrCancel         = new TapGestureRecognizer();
         tgrCancel.Tapped += OnCancelTapped;
         imgOverlay.GestureRecognizers.Add(tgrCancel);
         imgMessage.GestureRecognizers.Add(tgrCancel);

         layout.Children.Add
         (
            imgOverlay,
            Constraint.RelativeToParent(parent => parent.Width * 0.5  - imgOverlay.Width  * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.5 - imgOverlay.Height * 0.5)
         );
         
         layout.Children.Add
         (
            imgMessage,
            Constraint.RelativeToParent(parent => parent.Width * 0.5  - imgMessage.Width  * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.5 - imgMessage.Height * 0.5)
         );
         
         layout.Children.Add
         (
            btnAppStore,
            Constraint.RelativeToView(imgMessage, (parent, view) => view.X + view.Width * 0.25),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Y + view.Height * 0.75),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Width * 0.5),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Height * 0.25)
         );
      }

      public override bool OnBackButtonPressed()
      {
         if (isDisposed) return false; // let system handle event (navigate as normal)

         Dispose();
         return true; // let overlay handle event (prevent page back navigation)
      }

      public void Dispose()
      {
         if (isDisposed) return;

         isDisposed = true;
         layout.Children.Remove(btnAppStore);
         layout.Children.Remove(imgMessage);
         layout.Children.Remove(imgOverlay);
         layout = null;
         //imgOverlay.GestureRecognizers.Remove(tgrCancel); // FIXME crash if called from within touch event handler
         imgOverlay = null;
         //imgMessage.GestureRecognizers.Remove(tgrCancel); // FIXME crash if called from within touch event handler
         imgMessage = null;
         tgrCancel.Tapped -= OnCancelTapped;
         tgrCancel = null;
         btnAppStore.Clicked -= OnAppStoreClicked;
         btnAppStore = null;

         AppStore.Instance.SetListener(null);
      }

      void OnLayoutSizeChanged(object sender, EventArgs args)
      {
         if (isDisposed) return;
         layout.ForceLayout();
      }

      void OnCancelTapped(object sender, EventArgs args)
      {
         Dispose();
      }

      void OnAppStoreClicked(object sender, EventArgs args)
      {
         Analytics.TrackPurchase(Analytics.EVENT_PURCHASE_REQUEST);
         AppStore.Instance.SetListener(this);
         AppStore.Instance.Purchase();
      }

      public void OnPurchaseStatus(PurchaseStatus status) // IPurchaseListener
      {
         // called from app-store thread -> do not update views (use UI thread instead)

         switch (status)
         {
            case PurchaseStatus.Connected:
            Log("appstore connected");
            LogPurchases();
            break;

            case PurchaseStatus.Disconnected:
            Log("appstore disconnected");
            Dispose();
            break;

            case PurchaseStatus.Purchased:
            Log("appstore purchased");
            Analytics.TrackPurchase(Analytics.EVENT_PURCHASE_SUCCESS);
            Dispose();
            break;

            case PurchaseStatus.Consumed:
            Log("appstore *** error *** consumed ***"); // should never happen
            break;

            case PurchaseStatus.Cancelled:
            Log("appstore cancelled");
            Analytics.TrackPurchase(Analytics.EVENT_PURCHASE_CANCEL);
            Dispose();
            break;

            default:
            Dispose();
            break;
         }
      }

      public void OnPurchaseLog(string message) // IPurchaseListener
      {
      }

      void Log(string s,
               [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
      {
         // in app purchase thread (system thread) may have called us -> transform to UI thread
         Device.BeginInvokeOnMainThread (() => 
         {
            Debug.LogToFileMethod(s, methodName);
         });
      }

      void LogPurchases()
      {
         Log("appstore dummy purchased: "     + AppStore.Instance.IsPurchased(PurchaseType.Dummy));
         Log("appstore developer purchased: " + AppStore.Instance.IsPurchased(PurchaseType.Development));
         Log("appstore unlocker purchased: "  + AppStore.Instance.IsPurchased(PurchaseType.Unlocker));
      }
   }
}
