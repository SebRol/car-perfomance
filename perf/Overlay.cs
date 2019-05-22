using System;
using Xamarin.Forms;

namespace perf
{
   public abstract class Overlay
   {
      protected class TransparentButton : Button
      {
         public TransparentButton()
         {
            BackgroundColor = Color.Transparent;
            Opacity = 0;
         }
      }

      protected RelativeLayout       layout;
      protected Image                imgOverlay;
      protected Image                imgMessage;
      protected TapGestureRecognizer tgrCancel;
      protected bool                 isDisposed;

      public Overlay(RelativeLayout parentLayout)
      {
         layout = parentLayout;
      }

      // inherited classes must override this
      public abstract bool OnBackButtonPressed();
   }
}
