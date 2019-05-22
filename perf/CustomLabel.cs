using Xamarin.Forms;

namespace perf
{
   public class CustomLabel : Label
   {
      public const int SIZE_SMALL       = 1;
      public const int SIZE_MEDIUM      = 2;
      public const int SIZE_CAPTION     = 3;
      public const int SIZE_LARGE       = 4;
      public const int SIZE_BIG         = 5;
      public const int SIZE_SPEEDO      = 6;
      public const int SIZE_SPEEDO_UNIT = 7;
      public const int SIZE_CAPTION_DE  = 8;

      public static readonly BindableProperty SizeProperty = BindableProperty.Create("Size", typeof(int), typeof(CustomLabel), default(int), propertyChanged: OnSizePropertyChanged);

      public int Size
      {
         get
         {
            return (int)GetValue(SizeProperty); 
         }

         set
         {
            SetValue(SizeProperty, value); 
         }
      }

      static void OnSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
      {         
      }
   }
}
