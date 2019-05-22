using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace perf
{
   // abstracts platform SHARE function
   public interface IDeviceShare
   {
      /// <summary>
      /// Simply share text on compatible services
      /// </summary>
      /// <param name="title"    >Title of popup on share (not included in message)</param>
      /// <param name="subject"  >Subject to share</param>
      /// <param name="text"     >Text to share</param>
      /// <param name="imagePath">Path to an image to share</param>
      /// <returns>awaitable Task</returns>
      Task Share(string title = null, string subject = null, string text = null, string imagePath = null);
   }
}
