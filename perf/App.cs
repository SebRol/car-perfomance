using Xamarin.Forms;

namespace perf
{
   public class App : Application
   {
      readonly PageMain pageMain;

      public App()
      {
         Debug.Init();
         Debug.LogToFileMethod();

         pageMain = new PageMain();

         // the root page of the app
         MainPage = new NavigationPage(pageMain);
      }

      protected override void OnStart()
      {
         Debug.LogToFileMethod();

         // when the app starts
         pageMain.OnResume();
      }

      protected override void OnSleep()
      {
         Debug.LogToFileMethod();

         // when the app sleeps
         pageMain.OnSleep();

         // invalidate reference to the app store
         AppStore.Dispose();
      }

      protected override void OnResume()
      {
         Debug.LogToFileMethod();

         // when the app resumes
         pageMain.OnResume();
      }
   }
}
