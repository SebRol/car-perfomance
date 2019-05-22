using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;

using perf;
using perf.Droid;

[assembly: ExportRendererAttribute(typeof(CustomIcon), typeof(CustomIconRenderer))]
namespace perf.Droid
{
   public class CustomIconRenderer : ViewRenderer<CustomIcon, ImageView>
   {
      private bool _isDisposed;

      public CustomIconRenderer()
      {
         base.AutoPackage = false;
      }

      protected override void Dispose(bool disposing)
      {
         if (_isDisposed)
         {
            return;
         }

         _isDisposed = true;
         base.Dispose(disposing);
      }

      protected override void OnElementChanged(ElementChangedEventArgs<CustomIcon> e)
      {
         base.OnElementChanged(e);

         if (e.OldElement == null)
         {
            SetNativeControl(new ImageView(Context));
         }

         UpdateBitmap(e.OldElement);
      }

      protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         base.OnElementPropertyChanged(sender, e);

         if (e.PropertyName == CustomIcon.SourceProperty.PropertyName)
         {
            UpdateBitmap(null);
         }
         else if (e.PropertyName == CustomIcon.ForegroundProperty.PropertyName)
         {
            UpdateBitmap(null);
         }
      }

      private void UpdateBitmap(CustomIcon previous = null)
      {
         if (!_isDisposed)
         {
            var d = Resources.GetDrawable(Element.Source).Mutate();

            d.SetColorFilter(new LightingColorFilter(Element.Foreground.ToAndroid(), Element.Foreground.ToAndroid()));
            d.Alpha = Element.Foreground.ToAndroid().A;

            Control.SetImageDrawable(d);
            ((IVisualElementController)Element).NativeSizeChanged();
         }
      }
   }
}
