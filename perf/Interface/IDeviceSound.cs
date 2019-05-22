using System;

namespace perf
{
   public enum SoundId
   {
      acc1_launch = 0,
      acc2_reached,
      brk1_ready,
      brk2_brake,
      brk3_stopped,
      calibration,
      failure,
      zero1_launch,
      zero2_reached,
      zero3_stopped
   }

   // abstracts platform SOUND function
   public interface IDeviceSound
   {
      // lifecyle management
      void Init();
      void DeInit();

      void Play(SoundId id);
   }
}
