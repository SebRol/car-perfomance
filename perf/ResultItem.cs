using System;
using System.Collections.Generic;

namespace perf
{
   public class ResultItem : IComparable
   {
      public string Left  { private set; get; }
      public string Right { private set; get; }

      public ResultItem(string left, string right)
      {
         Left  = left;
         Right = right;
      }

      public void ToKph()
      {
         string leftOld;
         string leftNew;
         string rightOld;
         string rightNew;

         GetStartEnd(out leftOld, out rightOld);

         leftNew  = Tools.ToKilometerPerHourS(leftOld);
         rightNew = Tools.ToKilometerPerHourS(rightOld);

         Left  = Left.Replace (leftOld,  leftNew );
         Right = Right.Replace(rightOld, rightNew);
      }
      
      public void ToMph()
      {
         string leftOld;
         string leftNew;
         string rightOld;
         string rightNew;

         GetStartEnd(out leftOld, out rightOld);

         leftNew  = Tools.ToMilesPerHourS(leftOld);
         rightNew = Tools.ToMilesPerHourS(rightOld);

         leftNew  = Tools.RoundTo10(leftNew);
         rightNew = Tools.RoundTo10(rightNew);

         // ensure left/right values are not equal (may happen due to rounding error)
         if (leftNew.Equals(rightNew)) rightNew = Tools.Add10(rightNew);

         Left  = Left.Replace (leftOld,  leftNew );
         Right = Right.Replace(rightOld, rightNew);
      }

      public void ToMeter()
      {
         string leftOld;
         string leftNew;
         string rightOld;
         string rightNew;

         GetStartEnd(out leftOld, out rightOld);

         leftNew  = Tools.ToMeterS(leftOld);
         rightNew = Tools.ToMeterS(rightOld);

         // special case for 1/8, 1/4, 1/2 mile distance -> avoid rounding error
         if      (leftOld.Equals(  "660")) leftNew  = "201";
         else if (leftOld.Equals( "1320")) leftNew  = "402";
         else if (leftOld.Equals( "2640")) leftNew  = "804";
         if      (rightOld.Equals( "660")) rightNew = "201";
         else if (rightOld.Equals("1320")) rightNew = "402";
         else if (rightOld.Equals("2640")) rightNew = "804";

         Left  = Left.Replace (leftOld,  leftNew );
         Right = Right.Replace(rightOld, rightNew);
      }

      public void ToFeet()
      {
         string leftOld;
         string leftNew;
         string rightOld;
         string rightNew;

         GetStartEnd(out leftOld, out rightOld);

         leftNew  = Tools.ToFeetS(leftOld);
         rightNew = Tools.ToFeetS(rightOld);

         leftNew  = Tools.RoundTo50(leftNew);
         rightNew = Tools.RoundTo50(rightNew);

         // ensure left/right values are not equal (may happen due to rounding error)
         if (leftNew.Equals(rightNew)) rightNew = Tools.Add50(rightNew);

         // special case for 1/8, 1/4, 1/2 mile distance -> avoid rounding error
         if      (leftOld.Equals( "201")) leftNew  = "660";
         else if (leftOld.Equals( "402")) leftNew  = "1320";
         else if (leftOld.Equals( "804")) leftNew  = "2640";
         if      (rightOld.Equals("201")) rightNew = "660";
         else if (rightOld.Equals("402")) rightNew = "1320";
         else if (rightOld.Equals("804")) rightNew = "2640";

         Left  = Left.Replace (leftOld,  leftNew );
         Right = Right.Replace(rightOld, rightNew);
      }

      public override string ToString()
      {
         return string.Format("{0},{1}", Left, Right);
      }

      public int CompareTo(object o)
      {
         int result;
         ResultItem a = this;
         ResultItem b = (ResultItem)o;

         result = Tools.AlphaNumericCompare(a.Left, b.Left);
         if (result == 0)
         {
            result = Tools.AlphaNumericCompare(a.Right, b.Right);
         }

         return result;
      }

      public void GetStartEnd(out string start, out string end)
      {
         int index;
         int length;

         if (  (Left.IndexOf("@")  < 0)
             || (Right.IndexOf("@") < 0)
             || (Left.Length       == 0)
             || (Right.Length      == 0))
         {
            // parameters are invalid
            start = "0";
            end   = "0";
            return;
         }

         // left: strip "Start @"
         index  = Left.IndexOf("@") + 1;
         length = Left.Length - index;
         start  = Left.Substring(index, length);

         // right: strip "End @" or "Target @"
         index  = Right.IndexOf("@") + 1;
         length = Right.Length - index;
         end    = Right.Substring(index, length);

         // right: strip " (1/4 mile)"
         length = end.IndexOf(" ");
         if (length > 0)
         {
            end = end.Substring(0, length);
         }
      }
   }
}
