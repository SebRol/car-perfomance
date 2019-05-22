using System;

namespace perf
{
   public enum LocaleId
   {
      English,
      German
   }

   // abstracts platform LOCALOZATION function
   public interface IDeviceLocale
   {
      LocaleId GetLocale();
   }
}
