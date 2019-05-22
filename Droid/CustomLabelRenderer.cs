using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Widget;
using Android.Graphics;
using Android.Text;
using Android.Text.Util;
using Android.Content;
using Android.Content.PM;
using Android.Net;

using System.Collections.Generic;

using Java.Lang;

using perf.Droid;


[assembly: ExportRenderer(typeof(perf.CustomLabel), typeof(CustomLabelRenderer))]
namespace perf.Droid
{
   public class TextWatcher : Object, ITextWatcher
   {
      public void AfterTextChanged(IEditable s)
      {
         // ensure a blank space at the end of each label.text
         // because the italic font is cut off at the right border

         if (s == null) return;
         if (s.Length() < 1) return;
         if (s.CharAt(s.Length() - 1).CompareTo(' ') == 0) return;

         s.Append(' ');
      }

      public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
      {
      }

      public void OnTextChanged(ICharSequence s, int start, int before, int count)
      {
      }
   }

   public class CustomLabelRenderer : LabelRenderer
   {
      static readonly Typeface font            = Typeface.CreateFromAsset(Forms.Context.Assets, "font.ttf");
      static bool              isMailSupported = false;
      static bool              isFirstRun      = true;

      public CustomLabelRenderer()
      {
         // evaluate mail function only once
         if (isFirstRun == true)
         {
            isFirstRun      = false;
            isMailSupported = IsMailSupported();
         }
      }

      bool IsMailSupported()
      {
         Intent             intent  = new Intent(Intent.ActionSendto, Uri.FromParts("mailto","example@mail.com", null));
         PackageManager     manager = Android.App.Application.Context.PackageManager;
         IList<ResolveInfo> infos   = manager.QueryIntentActivities(intent, PackageInfoFlags.MatchAll);

         if (infos.Count > 0)
         {
            // at least one application can handle the MAIL intent on Android
            return true;
         }

         return false;
      }

      protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
      {
         base.OnElementChanged(e);

         if (e.NewElement != null)
         {
            TextView label = Control;
            int      size  = ((CustomLabel)e.NewElement).Size;

            if (size > 0) // is valid
            {
               switch (size)
               {
                  case CustomLabel.SIZE_SPEEDO:      label.TextSize = 80;                                                          break;
                  case CustomLabel.SIZE_SPEEDO_UNIT: label.TextSize = 30;                                                          break;
                  case CustomLabel.SIZE_BIG:         label.TextSize = 80;                                                          break;
                  case CustomLabel.SIZE_LARGE:       label.TextSize = (float)Device.GetNamedSize(NamedSize.Large,  typeof(Label)); break;
                  case CustomLabel.SIZE_CAPTION:     label.TextSize = 40;                                                          break;
                  case CustomLabel.SIZE_CAPTION_DE:  label.TextSize = 30;                                                          break;
                  case CustomLabel.SIZE_MEDIUM:      label.TextSize = (float)Device.GetNamedSize(NamedSize.Medium, typeof(Label)); break;
                  default:                           label.TextSize = (float)Device.GetNamedSize(NamedSize.Small,  typeof(Label)); break;
               }
            }

            if (isMailSupported)
            {
               // make mail/url addresses clickable

               // set the auto link mask to capture link-able data
               // info: make sure to set text after setting the mask
               label.AutoLinkMask = MatchOptions.EmailAddresses | MatchOptions.WebUrls;
            }

            label.Typeface = font;

            // hyperlinks shall be readable on light and dark background
            label.SetLinkTextColor(Android.Graphics.Color.Rgb(100, 100, 255));

            // ensure a blank space at the end of each label.text
            // because the italic font is cut off at the right border
            label.Append(" ");

            label.AddTextChangedListener(new TextWatcher());
         }
      }
   }
}
