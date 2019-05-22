using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace perf
{
   // abstracts platform EXIT function
   public interface IDeviceExit
   {
      // exit app
      void Exit();
   }
}
