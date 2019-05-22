using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using perf.Droid;

[assembly: ExportRenderer(typeof(perf.CustomEntry), typeof(CustomEntryRenderer))]
namespace perf.Droid
{
   public class CustomEntryRenderer : EntryRenderer
   {
      protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
      {
         base.OnElementChanged(e);

         if (e.OldElement == null) 
         {
            var nativeEntryEditText = Control;
            nativeEntryEditText.SetSelectAllOnFocus(true);
         }
      }
   }
}
