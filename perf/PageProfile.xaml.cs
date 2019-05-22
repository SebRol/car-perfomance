using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace perf
{
   public partial class PageProfile : ContentPage, IDisposable
   {
      ObservableCollection<DataItemVehicle> vehicles;
      Database                              database;
      IDeviceAcceleromter                   accelerometerProvider;
      IDeviceSound                          soundProvider;
      TapGestureRecognizer                  tgrButtonHelp;
      TapGestureRecognizer                  tgrButtonBack;

      public PageProfile(IDeviceAcceleromter accelerometer, IDeviceSound sound)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         accelerometerProvider = accelerometer;
         soundProvider         = sound;

         InitLayout();

         database = Database.GetInstance();
         vehicles = new ObservableCollection<DataItemVehicle>(database.GetVehicles());

         SetSelectedItem(SearchVehicle(database.GetActiveProfile()));
         ListViewVehicles.ItemSelected += OnItemSelected;
      }

      public void Dispose() // IDisposable
      {
         imgButtonBack.GestureRecognizers.RemoveAt(0);
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         tgrButtonBack         = null;

         imgButtonHelp.GestureRecognizers.RemoveAt(0);
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         tgrButtonHelp         = null;

         ListViewVehicles.ItemSelected -= OnItemSelected;

         btnProfileEdit.Clicked -= OnButtonEdit;
         btnProfileEdit.Image    = null;

         btnProfileCreate.Clicked -= OnButtonCreate;
         btnProfileCreate.Image    = null;

         vehicles.Clear();
         vehicles = null;

         database              = null;
         accelerometerProvider = null;
         soundProvider         = null;
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_PROFILE);

         btnProfileEdit.Image   = Localization.btn_profile_edit;
         btnProfileCreate.Image = Localization.btn_profile_create;
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }

      public void AddVehicle(string model, string color, float calibration)
      {
         var item = new DataItemVehicle{Model = model, Color = color, Calibration = calibration};

         database.SaveVehicle(item);
         vehicles.Add(item);
         SetSelectedItem(item);
      }

      public void UpdateVehicle(DataItemVehicle vehicle, string model, string color, float calibration)
      {
         vehicle.Model       = model;
         vehicle.Color       = color;
         vehicle.Calibration = calibration;

         database.SaveVehicle(vehicle);
         SetSelectedItem(vehicle);
      }

      public void RemoveVehicle(DataItemVehicle item)
      {
         database.DeleteVehicle(item.Id);
         vehicles.Remove(item);
         SetSelectedItem(vehicles[0]);
      }

      public string GetSelectedVehicleColor()
      {
         var item = (DataItemVehicle)ListViewVehicles.SelectedItem;
         return item.Color;
      }

      void InitLayout()
      {
         tgrButtonBack         = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);

         lblCaption.Text       = Localization.pageProfileHead;

         tgrButtonHelp         = new TapGestureRecognizer();
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
         imgButtonHelp.GestureRecognizers.Add(tgrButtonHelp);
      }

      void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
      {
         DataItemVehicle item = (DataItemVehicle)e.SelectedItem;

         if (item != null)
         {
            database.SaveActiveProfile(item);
         }
      }

      async void OnButtonBackTapped(object sender, EventArgs args)
      {
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         await Navigation.PopAsync();
         tgrButtonBack.Tapped += OnButtonBackTapped;
      }

      async void OnButtonHelpTapped(object sender, EventArgs args)
      {
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         await Navigation.PushAsync(new PageText(PageText.ContentType.Profile));
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
      }

      async void OnButtonCreate(object sender, EventArgs args)
      {
         btnProfileCreate.IsEnabled = false;
         await Navigation.PushAsync(new PageProfileEdit(this, null, false, accelerometerProvider, soundProvider));
         Analytics.TrackEventProfile(Analytics.EVENT_PPROFILE_CREATE);
         btnProfileCreate.IsEnabled = true;
      }

      async void OnButtonEdit(object sender, EventArgs args)
      {
         var item             = (DataItemVehicle)ListViewVehicles.SelectedItem;
         var showDeleteButton = true;

         // ensure at least one item is in the list at all time
         if (vehicles.Count <= 1)
         {
            item = vehicles[0];
            showDeleteButton = false;
         }

         btnProfileEdit.IsEnabled = false;
         await Navigation.PushAsync(new PageProfileEdit(this, item, showDeleteButton, accelerometerProvider, soundProvider));
         Analytics.TrackEventProfile(Analytics.EVENT_PPROFILE_EDIT);
         btnProfileEdit.IsEnabled = true;
      }

      void SetSelectedItem(DataItemVehicle v)
      {
         // workaround to activate a selected item is to nullify the list
         if (v != null)
         {
            ListViewVehicles.SelectedItem = null;
            ListViewVehicles.ItemsSource  = null;
            ListViewVehicles.ItemsSource  = vehicles;
            ListViewVehicles.SelectedItem = v;
         }
      }

      DataItemVehicle SearchVehicle(DataItemVehicle v)
      {
         for (int i = 0; i < vehicles.Count; i++)
         {
            if (vehicles[i].Id == v.Id)
            {
               return vehicles[i];
            }
         }

         return vehicles[0];
      }
   }
}
