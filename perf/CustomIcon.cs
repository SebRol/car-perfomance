using Xamarin.Forms;

namespace perf
{
   public class CustomIcon : View
   {
      // Foreground

      public static readonly BindableProperty ForegroundProperty = BindableProperty.Create("Foreground", typeof(Color), typeof(CustomIcon), default(Color), propertyChanged: OnForegroundPropertyChanged);

      public Color Foreground 
      { 
         get 
         { 
            return (Color)GetValue(ForegroundProperty); 
         }

         set 
         { 
            SetValue(ForegroundProperty, value); 
         } 
      }

      static void OnForegroundPropertyChanged(BindableObject bindable, object oldValue, object newValue)
      {         
      }

      // Source

      public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(string), typeof(CustomIcon), default(string), propertyChanged: OnSourcePropertyChanged);

      public string Source 
      {
         get 
         { 
            return (string)GetValue(SourceProperty); 
         } 

         set 
         { 
            SetValue(SourceProperty, value); 
         } 
      }

      static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
      {         
      }
   }
}
