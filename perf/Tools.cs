using System;
using System.Globalization;
using Xamarin.Forms;

namespace perf
{
   public class Tools
   {
      Tools()
      {
      }

      /***
       * Projects a value from a source range to a target range (linear mapping, linear transformation).
       * <code>source</code> should be in range between <code>sourceMin</code> and <code>sourceMax</code>.
       * <code>sourceMin</code> should be smaller than <code>sourceMax</code>.
       * <code>targetMin</code> should be smaller than <code>targetMax</code>.
       * @param source    value to be mapped from given source range to given target range
       * @param sourceMin start of the source range
       * @param sourceMax end of the source range
       * @param targetMin start of the target range
       * @param targetMax end of the target range
       * @return          the transformed value from source to target range, preserving the ratio of min and max
       */
      public static double Project(double source, double sourceMin, double sourceMax, double targetMin, double targetMax)
      {
         if (source < sourceMin) return targetMin;
         if (source > sourceMax) return targetMax;
         if (Math.Abs(sourceMax - sourceMin) < Double.Epsilon) return 0;

         return  ((source-sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin)) + targetMin;
      }

      // quadratic easing in/out
      // time goes from 0..1 where 0 is the beginning and 1 is the end
      public static double EasingQuadraticInOut(double time, double start, double end, double duration) 
      {
         time /= duration / 2;

         if (time < 1) return end / 2 * time * time + start;

         time--;

         return -end / 2 * (time * (time - 2) - 1) + start;
      }
      
      public static string ToKilometerPerHour(float meterPerSeconds)
      {
         return String.Format("{0:0}", meterPerSeconds * 3.6f);
      }

      public static string ToKilometerPerHourS(string milesPerHour)
      {
         float f = float.Parse(milesPerHour);

         return (Math.Round(f * 1.60934f)).ToString();
      }

      public static string ToMilesPerHour(float meterPerSeconds)
      {
         return String.Format("{0:0}", meterPerSeconds * 2.23694f);
      }

      public static float ToMilesPerHourF(float meterPerSeconds)
      {
         return meterPerSeconds * 2.23694f;
      }

      public static string ToMilesPerHourS(string kilometerPerHour)
      {
         float f = float.Parse(kilometerPerHour);

         return (Math.Round(f / 1.60934f)).ToString();
      }

      public static float ToMeterPerSecondF(float kilometerPerHour)
      {
         return kilometerPerHour / 3.6f;
      }

      public static float ToMeterPerSecondFM(float milesPerHour)
      {
         return milesPerHour / 2.23694f;
      }

      public static float ToMeterPerSecond(string kilometerPerHour)
      {
         float f = float.Parse(kilometerPerHour);
         return f / 3.6f;
      }
      
      public static float ToMeterPerSecondM(string milesPerHour)
      {
         float f = float.Parse(milesPerHour);
         return f / 2.23694f;
      }
      
      public static float ToMeter(string feet)
      {
         float f = float.Parse(feet);
         return f / 3.28084f;
      }

      public static string ToMeterS(string feet)
      {
         float f = float.Parse(feet);
         return (Math.Round(f / 3.28084f)).ToString();
      }
      
      public static float ToFeet(float meter)
      {
         return meter * 3.28084f;
      }
      
      public static string ToFeetS(string meter)
      {
         float f = float.Parse(meter);
         return (Math.Round(f * 3.28084f)).ToString();
      }
      
      public static string RoundTo10(string number)
      {
         float f = float.Parse(number);
         return (Math.Round(f / 10) * 10).ToString();
      }
      
      public static string RoundTo50(string number)
      {
         float f = float.Parse(number);
         return (Math.Round(f / 50) * 50).ToString();
      }
      
      public static string Add10(string number)
      {
         float f = float.Parse(number);
         return (Math.Round(f + 10)).ToString();
      }

      public static string Add50(string number)
      {
         float f = float.Parse(number);
         return (Math.Round(f + 50)).ToString();
      }

      public static string ToSpeedUnit(float meterPerSecond)
      {
         string result;

         if (Settings.IsSpeedUnitKph()) result = ToKilometerPerHour(meterPerSecond);
         else                           result = ToMilesPerHour(meterPerSecond);

         return result;
      }

      public static float ToSpeedUnitF(float meterPerSecond)
      {
         float result;

         if (Settings.IsSpeedUnitKph()) result = meterPerSecond * 3.6f;
         else                           result = meterPerSecond * 2.23694f;

         return result;
      }

      public static ResultMode ToResultMode(string runMode)
      {
         if      (runMode.Equals(RunModeBrake.Mode     )) return ResultMode.Brake;
         else if (runMode.Equals(RunModeZeroToZero.Mode)) return ResultMode.ZeroToZero;
         else                                             return ResultMode.Speed;
      }

      public static string ToFilterMode(int filterSetting)
      {
         if      (filterSetting == 1) return "medium";
         else if (filterSetting == 2) return "strong";
         else                         return "off";
      }

      public static string ToStopwatchTime(TimeSpan ts)
      {
         if      (ts.Hours   > 0) return ts.ToString("hh\\:mm\\:ss\\.f") + "h"; // 9:1:2:34.567 -> 09:01:02:34.5h
         else if (ts.Minutes > 0) return ts.ToString("mm\\:ss\\.f") + "m";      // 1:2:34.567   -> 01:02:34.5m
         else                     return ts.ToString("s\\.f") + "s";            // 1.234        -> 1.2s
      }

      public static string ToStopwatchTime(float seconds)
      {
         TimeSpan ts = TimeSpan.FromSeconds(seconds);

         if      (ts.Hours   > 0) return ts.ToString("hh\\:mm\\:ss\\.ff") + "h"; // 9:1:2:34.567 -> 09:01:02:34.56h
         else if (ts.Minutes > 0) return ts.ToString("mm\\:ss\\.ff") + "m";      // 1:2:34.567   -> 01:02:34.56m
         else                     return ts.ToString("s\\.ff") + "s";            // 1.234        -> 1.23s
      }

      public static string ToLocaleDate(DateTime dateTime)
      {
         IFormatProvider ifp = null;

         if (Localization.isEnglish) ifp = CultureInfo.CreateSpecificCulture("en-US");
         else                        ifp = CultureInfo.CreateSpecificCulture("de-DE");

         string result = dateTime.ToString("dd MMMM yyyy", ifp);

         return result;
      }

      public static string ToResultFormat(float value, SubMode subMode, string unit)
      {
         if ((subMode == SubMode.TimeDistance) || subMode == SubMode.TimeSpeed) 
         {
            return ToStopwatchTime(value);  
         }
         else
         {
            if (unit.Equals(Localization.unitFeet))
            {
               return ToFeet(value).ToString("0.00") + unit;
            }
            else
            {
               return value.ToString("0.00") + unit;
            }
         }
      }

      public static string ReplaceLastOccurrence(string source, string find, string replace)
      {
         int place = source.LastIndexOf(find, StringComparison.Ordinal); // ordinal = use ascii code

         if (place == -1) return source;

         string result = source.Remove(place, find.Length).Insert(place, replace);

         return result;
      }

      public static string RemoveTillEnd(string text, string toBeRemoved)
      {
         string result = text;
         int    index  = result.IndexOf(toBeRemoved, StringComparison.Ordinal);

         if (index >= 0)
         {
            result = result.Remove(index);
         }

         return result;
      }

      // RemoveDigits("12.123456", 4) -> "12.1234"
      // RemoveDigits("12.12",     4) -> "12.12"
      // RemoveDigits("12.12",     0) -> "12"
      public static string RemoveDigits(string number, int digitsToRemain)
      {
         string result = number;
         int    index  = result.IndexOf('.', 0); // start search from 0 position

         // decimal point might also be ','
         if (index < 0) index = result.IndexOf(',', 0);

         if (index >= 0)
         {
            // found decimal point at 0-based index
            if (digitsToRemain == 0)
            {
               // cut off all, including decimal point
               result = result.Substring(0, index);
            }
            else
            {
               // don't go beyond length of input
               int min = Math.Min(result.Length - index + 1, digitsToRemain + 1);
               if ((min > 0) && ((index + min) <= result.Length))
               {
                  result = result.Substring(0, index + min);
               }
            }
         }

         return result;
      }

      // kudos to dotnetperls.com/alphanumeric-sorting
      public static int AlphaNumericCompare(string s1, string s2)
      {
         if ((s1 == null) || (s2 == null))
         {
            return 0;
         }

         int len1 = s1.Length;
         int len2 = s2.Length;
         int marker1 = 0;
         int marker2 = 0;

         // Walk through two the strings with two markers.
         while (marker1 < len1 && marker2 < len2)
         {
            char ch1 = s1[marker1];
            char ch2 = s2[marker2];

            // Some buffers we can build up characters in for each chunk.
            char[] space1 = new char[len1];
            int loc1 = 0;
            char[] space2 = new char[len2];
            int loc2 = 0;

            // Walk through all following characters that are digits or
            // characters in BOTH strings starting at the appropriate marker.
            // Collect char arrays.
            do
            {
               space1[loc1++] = ch1;
               marker1++;

               if (marker1 < len1)
               {
                  ch1 = s1[marker1];
               }
               else
               {
                  break;
               }
            }
            while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

            do
            {
               space2[loc2++] = ch2;
               marker2++;

               if (marker2 < len2)
               {
                  ch2 = s2[marker2];
               }
               else
               {
                  break;
               }
            }
            while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

            // If we have collected numbers, compare them numerically.
            // Otherwise, if we have strings, compare them alphabetically.
            string str1 = new string(space1);
            string str2 = new string(space2);

            int result;

            if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
            {
               int thisNumericChunk = int.Parse(str1);
               int thatNumericChunk = int.Parse(str2);
               result = thisNumericChunk.CompareTo(thatNumericChunk);
            }
            else
            {
               result = string.Compare(str1, str2, StringComparison.Ordinal); // ordinal = use ascii code
            }

            if (result != 0)
            {
               return result;
            }
         }

         return len1 - len2;
      }
   }

   public class StringToColorConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         Color result = Color.Default;

         if (value != null)
         {
            string s = value.ToString();

            if      (s.StartsWith("#", StringComparison.Ordinal)) result = Color.FromHex(s); // ordinal = use ascii code
            else if (s.Equals("Accent"))                          result = Color.Accent;
         }

         return result;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return null;
      }
   }
}
