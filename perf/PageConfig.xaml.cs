using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageConfig : ContentPage
   {
      bool                 languageIsEnglish;
      bool                 distanceIsMetric;
      bool                 speedUnitIsMetric;
      int                  speedoFilter;
      bool                 soundIsOn;
      bool                 tipOfDayIsOn;
      TapGestureRecognizer tgrButtonBack;
      TapGestureRecognizer tgrButtonHelp;

      public PageConfig()
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         InitLayout();
         LoadDefault();
         InitButtons();
         InitLocalisation();
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_CONFIG);
      }

      void InitLayout()
      {
         tgrButtonBack = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);

         lblCaption.Text = Localization.pageConfigHead;

         tgrButtonHelp = new TapGestureRecognizer();
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
         imgButtonHelp.GestureRecognizers.Add(tgrButtonHelp);
      }

      void LoadDefault()
      {
         languageIsEnglish = Settings.GetValueOrDefault(Settings.Language,     true);
         distanceIsMetric  = Settings.GetValueOrDefault(Settings.DistanceUnit, true);
         speedUnitIsMetric = Settings.GetValueOrDefault(Settings.SpeedUnit,    true);
         speedoFilter      = Settings.GetValueOrDefault(Settings.SpeedoFilter, 1);
         soundIsOn         = Settings.GetValueOrDefault(Settings.Sound,        true);
         tipOfDayIsOn      = Settings.GetValueOrDefault(Settings.TipOfDay,     true);
      }

      void InitButtons()
      {
         btnLanguage.Image  = (languageIsEnglish) ? "@drawable/btn_settings_english.png" : "@drawable/btn_settings_deutsch.png";
         btnDistance.Image  = (distanceIsMetric)  ? Localization.btn_settings_meter      : Localization.btn_settings_feet;
         btnSpeedUnit.Image = (speedUnitIsMetric) ? Localization.btn_settings_kph        : Localization.btn_settings_mph;

         if (speedoFilter == 1)
            btnFilter.Image = Localization.btn_settings_filter_medium;
         else if (speedoFilter == 2)
            btnFilter.Image = Localization.btn_settings_filter_strong;
         else
            btnFilter.Image = Localization.btn_settings_filter_off;
         
         btnSounds.Image   = (soundIsOn)    ? Localization.btn_settings_sound_on   : Localization.btn_settings_sound_off;
         btnTipOfDay.Image = (tipOfDayIsOn) ? Localization.btn_settings_tip_yes    : Localization.btn_settings_tip_no;
      }

      void InitLocalisation()
      {
         EventBus.GetInstance().Subscribe<LocalisationChangeEvent>(e =>
         {
            lblCaption.Text = Localization.pageConfigHead;
            InitButtons();
         });
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
         await Navigation.PushAsync(new PageText(PageText.ContentType.Config));
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
      }

      void OnButtonLanguage(object sender, EventArgs args)
      {
         languageIsEnglish = !languageIsEnglish;

         if (languageIsEnglish == true)
         {
            btnLanguage.Image = "@drawable/btn_settings_english.png";
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_LANGUAGE_ENGLISH);
         }
         else
         {
            btnLanguage.Image = "@drawable/btn_settings_deutsch.png";
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_LANGUAGE_GERMAN);
         }

         Settings.AddOrUpdateValue(Settings.Language, languageIsEnglish);

         Localization.isEnglish = languageIsEnglish;
         EventBus.GetInstance().Publish(new LocalisationChangeEvent());
      }


      void OnButtonDistance(object sender, EventArgs args)
      {
         distanceIsMetric = !distanceIsMetric;

         if (distanceIsMetric == true)
         {
            btnDistance.Image = Localization.btn_settings_meter;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_DISTANCE_METER);
         }
         else
         {
            btnDistance.Image = Localization.btn_settings_feet;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_DISTANCE_FEET);
         }

         Settings.AddOrUpdateValue(Settings.DistanceUnit, distanceIsMetric);
      }


      void OnButtonSpeedUnit(object sender, EventArgs args)
      {
         speedUnitIsMetric = !speedUnitIsMetric;

         if (speedUnitIsMetric == true)
         {
            btnSpeedUnit.Image = Localization.btn_settings_kph;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_SPEED_KPH);
         }
         else
         {
            btnSpeedUnit.Image = Localization.btn_settings_mph;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_SPEED_MPH);
         }

         Settings.AddOrUpdateValue(Settings.SpeedUnit, speedUnitIsMetric);
      }


      void OnButtonFilter(object sender, EventArgs args)
      {
         speedoFilter++;

         if (speedoFilter == 1)
         {
            btnFilter.Image = Localization.btn_settings_filter_medium;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_FILTER_MEDIUM);
         }
         else if (speedoFilter == 2)
         {
            btnFilter.Image = Localization.btn_settings_filter_strong;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_FILTER_STRONG);
         }
         else
         {
            btnFilter.Image = Localization.btn_settings_filter_off;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_FILTER_OFF);
            speedoFilter = 0;
         }

         Settings.AddOrUpdateValue(Settings.SpeedoFilter, speedoFilter);
      }


      void OnButtonSounds(object sender, EventArgs args)
      {
         soundIsOn = !soundIsOn;

         if (soundIsOn == true)
         {
            btnSounds.Image = Localization.btn_settings_sound_on;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_SOUND_ON);
         }
         else
         {
            btnSounds.Image = Localization.btn_settings_sound_off;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_SOUND_OFF);
         }

         Settings.AddOrUpdateValue(Settings.Sound, soundIsOn);
      }


      void OnButtonTipOfDay(object sender, EventArgs args)
      {
         tipOfDayIsOn = !tipOfDayIsOn;

         if (tipOfDayIsOn == true)
         {
            btnTipOfDay.Image = Localization.btn_settings_tip_yes;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_TIPOFDAY_ON);
         }
         else
         {
            btnTipOfDay.Image = Localization.btn_settings_tip_no;
            Analytics.TrackEventConfig(Analytics.EVENT_CONFIG_TIPOFDAY_OFF);
         }

         Settings.AddOrUpdateValue(Settings.TipOfDay, tipOfDayIsOn);
      }
   }
}
