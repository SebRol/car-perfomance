using System;
using Xamarin.Forms;

namespace perf
{
   public class OverlayPurchaseError : Overlay
   {
      TransparentButton btnOk;
      TransparentButton btnHelp;

      public OverlayPurchaseError(RelativeLayout parentLayout) : base(parentLayout)
      {
         Debug.LogToFileMethod();
         Analytics.TrackPage(Analytics.PAGE_PURCHASE_ERROR);

         // shadow to darken display
         imgOverlay               = new Image();
         imgOverlay.Source        = "@drawable/ovr_shadow.png";
         imgOverlay.SizeChanged  += OnLayoutSizeChanged;
         imgOverlay.Aspect        = Aspect.Fill;
         imgOverlay.WidthRequest  = layout.Width;
         imgOverlay.HeightRequest = layout.Height;

         // app store message
         imgMessage              = new Image();
         imgMessage.Source       = Localization.ovr_purchase_error;
         imgMessage.SizeChanged += OnLayoutSizeChanged;

         // ok button
         btnOk              = new TransparentButton();
         btnOk.SizeChanged += OnLayoutSizeChanged;
         btnOk.Clicked     += OnButtonOkClicked;

         // help button
         btnHelp              = new TransparentButton();
         btnHelp.SizeChanged += OnLayoutSizeChanged;
         btnHelp.Clicked     += OnButtonHelpClicked;

         // on touch outside ok button -> cancel
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
            btnOk,
            Constraint.RelativeToView(imgMessage, (parent, view) => view.X + view.Width * 0.25),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Y + view.Height * 0.75),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Width * 0.5),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Height * 0.25)
         );
         
         layout.Children.Add
         (
            btnHelp,
            Constraint.RelativeToView(imgMessage, (parent, view) => view.X + view.Width * 0.7),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Y + view.Height * 0.0),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Width * 0.3),
            Constraint.RelativeToView(imgMessage, (parent, view) => view.Height * 0.3)
         );
      }

      public override bool OnBackButtonPressed()
      {
         if (isDisposed) return false; // let system handle event (navigate as normal)

         Dispose();
         return true; // let overlay handle event (prevent page back navigation)
      }

      void Dispose()
      {
         if (isDisposed) return;

         isDisposed = true;
         layout.Children.Remove(btnHelp);
         layout.Children.Remove(btnOk);
         layout.Children.Remove(imgMessage);
         layout.Children.Remove(imgOverlay);
         layout = null;
         //imgOverlay.GestureRecognizers.Remove(tgrCancel); // FIXME crash if called from within touch event handler
         imgOverlay = null;
         //imgMessage.GestureRecognizers.Remove(tgrCancel); // FIXME crash if called from within touch event handler
         imgMessage = null;
         tgrCancel.Tapped -= OnCancelTapped;
         tgrCancel = null;
         btnOk.Clicked -= OnButtonOkClicked;
         btnOk = null;
         btnHelp.Clicked -= OnButtonHelpClicked;
         btnHelp = null;
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
      
      void OnButtonOkClicked(object sender, EventArgs args)
      {
         Dispose();
      }

      void OnButtonHelpClicked(object sender, EventArgs args)
      {
         Dispose();
         EventBus.GetInstance().Publish(new HelpAppStoreEvent()); // send event -> someone should show PagrHelp with text about AppStore
      }
   }
}
