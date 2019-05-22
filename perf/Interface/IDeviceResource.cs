
namespace perf
{
   public enum IDeviceResourceString
   {
      helpMode1       = "helpMode1",
      helpMode2       = "helpMode2",
      helpMode3       = "helpMode3",
      helpSettings    = "helpSettings",
      helpProfile     = "helpProfile",
      helpResults     = "helpResults",
      tip01           = "tip01",
      tip02           = "tip02",
      tip03           = "tip03",
      tip04           = "tip04",
      tip05           = "tip05",
      tip06           = "tip06",
      tip07           = "tip07",
      tip08           = "tip08",
      tip09           = "tip09",
      tip10           = "tip10",
      tip11           = "tip11",
      tip12           = "tip12",
      tip13           = "tip13",
      tip14           = "tip14",
      tip15           = "tip15",
      tip16           = "tip16",
      tip17           = "tip17",
      tip18           = "tip18",
      tip19           = "tip19",
      tip20           = "tip20"
   }

   public interface IDeviceResource
   {
      void SetEnglish(bool isEnglish);
      string GetLoacleString(IDeviceResourceString key);
   }
}
