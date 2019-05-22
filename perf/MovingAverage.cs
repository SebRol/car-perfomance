using System.Linq;
using System.Collections.Generic;

namespace perf
{
   public class MovingAverage
   {
      public const int FILTER_MEDIUM = 10;
      public const int FILTER_STRONG = 20;

      readonly int _filter;
      readonly Queue<float> _q;

      public MovingAverage(int filter)
      {
         _filter = filter;

         if (_filter <= 0)
         {
            // avoid queue size of 0 or negative size
            _filter = 1;
         }

         _q = new Queue<float>(_filter);
      }

      public void Reset()
      {
         _q.Clear();
      }

      public float Get(float value)
      {
         if (_filter <= 1) // filtering set to OFF
            return value;

         _q.Enqueue(value);

         if (_q.Count > _filter)
         {
            _q.Dequeue();
         }

         if (_q.Count == 0)
         {
            return 0;
         }

         return _q.Average();
      }
   }
}
