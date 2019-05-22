using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.App;
using Android.Media;

using perf.Droid;
// register this class at the Xamarin.Forms DependencyService -> can be used in shared code
[assembly: Xamarin.Forms.Dependency(typeof (DeviceSoundAndroid))]
namespace perf.Droid
{
   public class DeviceSoundAndroid : Activity, IDeviceSound
   {
      List<MediaPlayer> mediaPlayers;

      public DeviceSoundAndroid()
      {
      }

      public void Init() // IDeviceSound
      {
         mediaPlayers = new List<MediaPlayer>();
         Array values = Enum.GetValues(typeof(SoundId)); // create array of all identifiers of enum "SoundId" (see IDeviceSound)

         foreach (SoundId id in values)
         {
            string soundFile = id.ToString() + ".mp3";
            var fd = Forms.Context.Assets.OpenFd(soundFile);

            MediaPlayer mp = new MediaPlayer();
            mp.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            mp.Prepare();

            mediaPlayers.Add(mp);
         }
      }

      public void DeInit() // IDeviceSound
      {
         foreach (MediaPlayer mp in mediaPlayers)
         {
            mp.Reset();
            mp.Dispose();
         }

         mediaPlayers.Clear();
      }

      public void Play(SoundId id)
      {
         if (Settings.GetValueOrDefault(Settings.Sound, true) == false)
         {
            // user settings are: sound off
            return;
         }

         // do not play sound on main thread (avoid gui-freeze)
         Task task = new Task
         (
            () => mediaPlayers[(int)id].Start()
         );
         task.Start();
      }
   }
}
