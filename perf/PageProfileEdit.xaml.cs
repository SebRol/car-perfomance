using System;
using Xamarin.Forms;

namespace perf
{
   class ColorButton : Button
   {
      const double COLOR_BTN_SIZE = 210;

      public readonly Point  position;
      public readonly Size   size;
      public readonly string color;
      public readonly string colorPickerImage;

      public ColorButton(double posX, double posY, string colorHex, string imgResource)
      {
         position         = new Point(posX, posY);
         size             = new Size(COLOR_BTN_SIZE, COLOR_BTN_SIZE);
         color            = colorHex;
         colorPickerImage = imgResource;
         BackgroundColor  = Color.White;
         Opacity          = 0;
      }
   }

   public partial class PageProfileEdit : ContentPage, IDisposable
   {
      // control "btn_profile_symbol1.png"
      const double BTN_COLOR_WIDTH  = 966;
      const double BTN_COLOR_HEIGHT = 629;

      PageProfile          storage;
      DataItemVehicle      item;
      DataItemVehicle      itemCalibration;
      string               color;
      float                calibration;
      IDeviceAcceleromter  accelerometerProvider;
      IDeviceSound         soundProvider;
      bool                 isDirty; // if true -> user updated model, color or calibration -> new vehicle is created in database

      // layout top
      Image                imgButtonBack;
      TapGestureRecognizer tgrButtonBack;
      CustomLabel          lblCaption;
      Image                imgButtonHelp;
      TapGestureRecognizer tgrButtonHelp;

      // layout middle
      Button               btnCalibration;
      Button               btnVehicle;
      Button               btnColor;
      CustomEntry          entryVehicle;

      // layout bottom
      Button               btnDelete;

#if false
      // drawable-xxxhdpi
      readonly ColorButton[] colorButtons = // control "btn_profile_symbol1.png" has 8 buttons inside
      {
         new ColorButton(55,  214, "#2AB3FF", "@drawable/btn_profile_symbol1.png"), // top left, blue
         new ColorButton(355, 214, "#35FF36", "@drawable/btn_profile_symbol2.png"), // green
         new ColorButton(654, 214, "#FF6565", "@drawable/btn_profile_symbol3.png"), // red
         new ColorButton(952, 214, "#FFA743", "@drawable/btn_profile_symbol4.png"), // top right, gold
         new ColorButton(55,  514, "#EF6AFF", "@drawable/btn_profile_symbol5.png"), // lower left, pink
         new ColorButton(355, 514, "#FFFFFF", "@drawable/btn_profile_symbol6.png"), // white
         new ColorButton(654, 514, "#2E2E2E", "@drawable/btn_profile_symbol7.png"), // silver
         new ColorButton(952, 514, "#000000", "@drawable/btn_profile_symbol8.png")  // lower right, black
      };
#endif

      // drawable-xxhdpi
      readonly ColorButton[] colorButtons = // control "btn_profile_symbol1.png" has 8 buttons inside
      {
         new ColorButton(53,  172, "#2AB3FF", "@drawable/btn_profile_symbol1.png"), // top left, blue
         new ColorButton(277, 172, "#35FF36", "@drawable/btn_profile_symbol2.png"), // green
         new ColorButton(502, 172, "#FF6565", "@drawable/btn_profile_symbol3.png"), // red
         new ColorButton(725, 172, "#FFA743", "@drawable/btn_profile_symbol4.png"), // top right, gold
         new ColorButton(53,  396, "#EF6AFF", "@drawable/btn_profile_symbol5.png"), // lower left, pink
         new ColorButton(277, 396, "#FFFFFF", "@drawable/btn_profile_symbol6.png"), // white
         new ColorButton(502, 396, "#2E2E2E", "@drawable/btn_profile_symbol7.png"), // silver
         new ColorButton(725, 396, "#000000", "@drawable/btn_profile_symbol8.png")  // lower right, black
      };

      public PageProfileEdit(PageProfile page, DataItemVehicle vehicle, bool showDeleteButton, IDeviceAcceleromter accelerometer, IDeviceSound sound)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         storage               = page;
         item                  = vehicle;
         accelerometerProvider = accelerometer;
         soundProvider         = sound;
         isDirty               = false;

         InitLayout(showDeleteButton, (item == null)); // if item is null -> create profile mode, not edit mode

         // init with default color
         color = colorButtons[0].color;

         if (item != null)
         {
            calibration       = item.Calibration;
            entryVehicle.Text = item.Model;

            if (item.Color != null)
            {
               // overwite with given color
               color = item.Color;
            }
         }
         else
         {
            calibration       = 0;
            entryVehicle.Text = Localization.pageProfileEditDefault;
         }
      }

      public void Dispose() // IDisposable
      {
         imgButtonBack.GestureRecognizers.RemoveAt(0);
         imgButtonBack.Source  = null;
         imgButtonBack         = null;
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         tgrButtonBack         = null;

         lblCaption = null;

         imgButtonHelp.GestureRecognizers.RemoveAt(0);
         imgButtonHelp.Source  = null;
         imgButtonHelp         = null;
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         tgrButtonHelp         = null;

         btnCalibration.Clicked -= OnButtonCalibration;
         btnCalibration.Image    = null;
         btnCalibration          = null;

         btnVehicle.Clicked -= OnButtonVehicle;
         btnVehicle.Image    = null;
         btnVehicle          = null;

         entryVehicle.Completed -= OnEntryVehicleComplete;
         entryVehicle.Unfocused -= OnEntryVehicleComplete;
         entryVehicle            = null;

         btnColor.Image = null;
         btnColor       = null;

         if (btnDelete != null)
         {
            btnDelete.Clicked -= OnButtonDelete;
            btnDelete.Image    = null;
            btnDelete          = null;
         }

         for (int i = 0; i < colorButtons.Length; i++)
         {
            colorButtons[i].SizeChanged -= OnColorButtonSizeChanged;
            colorButtons[i].Clicked     -= OnColorButtonClicked;
            colorButtons[i]              = null;
         }

         storage               = null;
         item                  = null;
         itemCalibration       = null;
         accelerometerProvider = null;
         soundProvider         = null;
      }

      void InitLayout(bool showDeleteButton, bool isCreateMode)
      {
         // layout top

         imgButtonBack         = new Image();
         imgButtonBack.Source  = "@drawable/btn_back.png";
         tgrButtonBack         = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);
         layout.Children.Add
         (
            imgButtonBack,
            Constraint.RelativeToParent(parent => parent.Width * 0.036),
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         lblCaption           = new CustomLabel();
         lblCaption.Size      = CustomLabel.SIZE_CAPTION;
         lblCaption.TextColor = Color.FromHex("4D4C4A");
         if (isCreateMode) lblCaption.Text = Localization.pageProfileEditCreate; // create profile
         else              lblCaption.Text = Localization.pageProfileEditHead;   // edit profile
         layout.Children.Add
         (
            lblCaption,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - lblCaption.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.03)
         );

         imgButtonHelp         = new Image();
         imgButtonHelp.Source  = "@drawable/btn_help_profile.png";
         tgrButtonHelp         = new TapGestureRecognizer();
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
         imgButtonHelp.GestureRecognizers.Add(tgrButtonHelp);
         layout.Children.Add
         (
            imgButtonHelp,
            Constraint.RelativeToParent(parent => parent.Width * 0.81),
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         // layout middle

         btnCalibration                 = new Button();
         btnCalibration.Image           = Localization.btn_profile_calibration;
         btnCalibration.BackgroundColor = Color.Transparent;
         btnCalibration.Clicked        += OnButtonCalibration;
         layout.Children.Add
         (
            btnCalibration,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnCalibration.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.23 - btnCalibration.Height * 0.5)
         );

         btnVehicle                 = new Button();
         btnVehicle.Image           = Localization.btn_profile_vehicle;
         btnVehicle.BackgroundColor = Color.Transparent;
         btnVehicle.Clicked        += OnButtonVehicle;
         layout.Children.Add
         (
            btnVehicle,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnVehicle.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.36 - btnVehicle.Height * 0.5)
         );

         entryVehicle                 = new CustomEntry();
         entryVehicle.IsVisible       = false;
         entryVehicle.BackgroundColor = Color.Black;
         entryVehicle.FontSize       *= 2;
         entryVehicle.SizeChanged    += OnLayoutSizeChanged; // width of layout is not known on page create -> delegate setup of entry.width to callback
         entryVehicle.Completed      += OnEntryVehicleComplete;
         entryVehicle.Unfocused      += OnEntryVehicleComplete;
         layout.Children.Add
         (
            entryVehicle,
            Constraint.RelativeToView(btnVehicle, (parent, view) => (view.X + view.Width  * 0.5 - entryVehicle.Width  * 0.5)),
            Constraint.RelativeToView(btnVehicle, (parent, view) => (view.Y + view.Height * 0.5 - entryVehicle.Height * 0.5 ))
         );

         btnColor                 = new Button();
         btnColor.IsEnabled       = false;
         btnColor.Image           = GetColorPickerImageFromItem(item);
         btnColor.BackgroundColor = Color.Transparent;
         layout.Children.Add
         (
            btnColor,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnColor.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * 0.62 - btnColor.Height * 0.5)
         );

         for (int i = 0; i < colorButtons.Length; i++)
         {
            ColorButton b  = colorButtons[i];
            b.SizeChanged += OnColorButtonSizeChanged;
            b.Clicked     += OnColorButtonClicked;

            layout.Children.Add
            (
               b,
               Constraint.RelativeToView(btnColor, (parent, view) => (view.X + b.position.X * ((view.Width / BTN_COLOR_WIDTH < view.Height / BTN_COLOR_HEIGHT) ? view.Width / BTN_COLOR_WIDTH : view.Height / BTN_COLOR_HEIGHT))),
               Constraint.RelativeToView(btnColor, (parent, view) => (view.Y + b.position.Y * ((view.Width / BTN_COLOR_WIDTH < view.Height / BTN_COLOR_HEIGHT) ? view.Width / BTN_COLOR_WIDTH : view.Height / BTN_COLOR_HEIGHT)))
            );
         }

         // layout bottom

         if (showDeleteButton)
         {
            btnDelete                  = new Button();
            btnDelete.Image            = Localization.btn_profile_delete;
            btnDelete.BackgroundColor  = Color.Transparent;
            btnDelete.Clicked         += OnButtonDelete;
            layout.Children.Add
            (
               btnDelete,
               Constraint.RelativeToParent((parent) => parent.Width / 2 - btnDelete.Width / 2),
               Constraint.RelativeToParent((parent) => parent.Height - btnDelete.Height)
            );
         }
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_PROFILE_EDIT);
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }

      protected override bool OnBackButtonPressed()
      {
         UpdateEntry();
         return base.OnBackButtonPressed();
      }

      void OnLayoutSizeChanged(object sender, EventArgs args)
      {
         // width of layout was not known on page create -> setup entry.width in this callback
         entryVehicle.WidthRequest = Width * 0.8;
      }

      async void OnButtonHelpTapped(object sender, EventArgs args)
      {
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         await Navigation.PushAsync(new PageText(PageText.ContentType.ProfileEdit));
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
      }

      void UpdateEntry()
      {
         if (itemCalibration != null)
         {
            // calibration page was shown
            calibration = itemCalibration.Calibration;
         }

         if (item != null)
         {
            // update entry
            storage.UpdateVehicle(item, entryVehicle.Text, color, calibration);
         }
         else
         {
            // create entry, but only if user updated model, color or calibration
            if (isDirty == true)
            {
               // "example" profile was changed by user
               storage.AddVehicle(entryVehicle.Text, color, calibration);
            }
         }
      }

      async void OnButtonBackTapped(object sender, EventArgs args)
      {
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         UpdateEntry();
         await Navigation.PopAsync();
         tgrButtonBack.Tapped += OnButtonBackTapped;
      }

      async void OnButtonCalibration(object sender, EventArgs args)
      {
         btnCalibration.IsEnabled = false;
         isDirty                  = true;
         itemCalibration          = DataItem.CreateDefaultVehicle();
         await Navigation.PushAsync(new PageCalibration(itemCalibration, accelerometerProvider, soundProvider, null));
         Analytics.TrackEventProfile(Analytics.EVENT_PPROFILE_CALIBRATE);
         btnCalibration.IsEnabled = true;
      }

      void OnButtonVehicle(object sender, EventArgs args)
      {
         btnVehicle.IsVisible   = false;
         entryVehicle.IsVisible = true;
         isDirty                = true;
         entryVehicle.Focus();
         Analytics.TrackEventProfile(Analytics.EVENT_PPROFILE_VEHICLE);
      }

      void OnEntryVehicleComplete(object sender, EventArgs args)
      {
         btnVehicle.IsVisible   = true;
         entryVehicle.IsVisible = false;
         isDirty                = true;
      }

      void OnColorButtonSizeChanged(object sender, EventArgs args)
      {
         ColorButton cb   = sender as ColorButton;
         var scale        = (btnColor.Width / BTN_COLOR_WIDTH < btnColor.Height / BTN_COLOR_HEIGHT) ? btnColor.Width / BTN_COLOR_WIDTH : btnColor.Height / BTN_COLOR_HEIGHT;
         cb.WidthRequest  = cb.size.Width  * scale;
         cb.HeightRequest = cb.size.Height * scale;
      }

      void OnColorButtonClicked(object sender, EventArgs args)
      {
         ColorButton cb = sender as ColorButton;
         color          = cb.color;
         btnColor.Image = cb.colorPickerImage;
         isDirty        = true;
         Analytics.TrackEventProfile(Analytics.EVENT_PPROFILE_COLOR);
      }

      async void OnButtonDelete(object sender, EventArgs args)
      {
         btnDelete.IsEnabled = false;
         storage.RemoveVehicle(item);
         await Navigation.PopAsync();
         Analytics.TrackEventProfile(Analytics.EVENT_PPROFILE_DELETE);
         btnDelete.IsEnabled = true;
      }

      string GetColorPickerImageFromItem(DataItemVehicle itm)
      {
         string result = colorButtons[0].colorPickerImage;

         if (itm != null)
         {
            if (itm.Color != null)
            {
               for (int i = 0; i < colorButtons.Length; i++)
               {
                  string c = colorButtons[i].color;
                  if (itm.Color.Equals(c))
                  {
                     result = colorButtons[i].colorPickerImage;
                     break;
                  }
               }
            }
         }

         return result;
      }
   }
}
