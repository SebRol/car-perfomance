using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageText : ContentPage
   {
      public enum ContentType
      {
         Calibration,
         CalibrationRemind,
         Disclaimer,
         Profile,
         ProfileEdit,
         Results,
         Config,
         Setup,
         TipOfDay
      }

      TapGestureRecognizer tgrButtonBack;

      public PageText(ContentType type)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         switch (type)
         {
         case ContentType.Calibration:
            InitLayoutCommon(Localization.pageTextHelpHead, Localization.pageTextHelpCalibration);
            break;
         case ContentType.CalibrationRemind:
            InitLayoutCommon(Localization.pageTextCalibRemindHead, Localization.pageTextCalibRemindText);
            break;
         case ContentType.Disclaimer:
            InitLayoutDisclaimer();
            break;
         case ContentType.Profile:
            InitLayoutCommon(Localization.pageTextHelpHead, Localization.pageTextHelpProfile);
            break;
         case ContentType.ProfileEdit:
            InitLayoutCommon(Localization.pageTextHelpHead, Localization.pageTextHelpProfileEdit);
            break;
         case ContentType.Results:
            InitLayoutCommon(Localization.pageTextHelpHead, Localization.pageTextHelpResults);
            break;
         case ContentType.Config:
            InitLayoutCommon(Localization.pageTextHelpHead, Localization.pageTextHelpConfig);
            break;
         case ContentType.Setup:
            InitLayoutCommon(Localization.pageTextHelpHead, Localization.pageTextHelpSetup);
            break;
         case ContentType.TipOfDay:
            InitLayoutTipOfDay();
            break;
         }
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_TIPOFDAY);
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }

      void InitLayoutDisclaimer()
      {
         // head
         imgButtonBack.IsVisible = false;
         imgButtonBack.IsEnabled = false;
         lblCaption.Text = Localization.pageDisclaimerHead;
         lblCaption.Size = (Localization.isEnglish) ? CustomLabel.SIZE_CAPTION : CustomLabel.SIZE_CAPTION_DE;
         imgButtonHelp.Source = "@drawable/icn_disclaimer.png";
         // body
         lblText.Text = Localization.pageDisclaimerText;
         // bottom
         lblSwitch.Text = Localization.pageDisclaimerSwitch;
         tglSwitch.IsToggled = false;
         tglSwitch.Toggled += OnSwitchDisclaimerClicked;
         btnOk.Image = Localization.btn_agree_inactive;
         btnOk.IsEnabled = false;
      }

      void InitLayoutTipOfDay()
      {
         // head
         tgrButtonBack = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);
         lblCaption.Text = Localization.pageTipOfDayHead;
         imgButtonHelp.Source = "@drawable/icn_tip.png";
         // body
         lblText.Text = getTipOfDayText();
         // bottom
         lblSwitch.Text = Localization.pageTipOfDaySwitch;
         tglSwitch.IsToggled = true;
         tglSwitch.Toggled += OnSwitchTipOfDayClicked;
         btnOk.Image = "@drawable/btn_ok.png";
      }

      void InitLayoutCommon(string head, string body)
      {
         // head
         tgrButtonBack = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);
         lblCaption.Text = head;
         lblCaption.Size = (Localization.isEnglish) ? CustomLabel.SIZE_CAPTION : CustomLabel.SIZE_CAPTION_DE;
         imgButtonHelp.Source = "@drawable/icn_help.png";
         // body
         lblText.Text = body;
         // bottom
         lblSwitch.IsVisible = false;
         tglSwitch.IsVisible = false;
         btnOk.Image = "@drawable/btn_ok.png";
      }

      protected override bool OnBackButtonPressed()
      {
         bool disclaimeriIsShown = Settings.GetValueOrDefault(Settings.Disclaimer, true);

         if (disclaimeriIsShown)
         {
            // disclaimer not accepted -> exit app (use dependency service to execute on iOS or Android)
            DependencyService.Get<IDeviceExit>().Exit();
         }
         else
         {
            // disclaimer was accepted -> pop this page -> goto main page
            Navigation.PopAsync();
         }

         // true = cancel event = prevent system default
         return true;
      }

      async void OnButtonBackTapped(object sender, EventArgs args)
      {
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         await Navigation.PopAsync();
         tgrButtonBack.Tapped += OnButtonBackTapped;
      }

      void OnSwitchDisclaimerClicked(object sender, EventArgs args)
      {
         Settings.AddOrUpdateValue(Settings.Disclaimer, ! tglSwitch.IsToggled); // if switch is ON -> do not show disclaimer again

         if (tglSwitch.IsToggled) 
         {
            btnOk.IsEnabled = true;
            btnOk.Image = Localization.btn_agree;
         }
         else                     
         {
            btnOk.IsEnabled = false;
            btnOk.Image = Localization.btn_agree_inactive;
         }
      }

      void OnSwitchTipOfDayClicked(object sender, EventArgs args)
      {
         Settings.AddOrUpdateValue(Settings.TipOfDay, tglSwitch.IsToggled); // id switch in ON -> show tipofday again on next start
      }

      async void OnButtonOk(object sender, EventArgs args)
      {
         btnOk.IsEnabled = false;
         await Navigation.PopAsync();
         btnOk.IsEnabled = true;
      }

      string getTipOfDayText()
      {
         string result;
         int    index = Settings.GetValueOrDefault(Settings.TipOfDayCount, 1);

         switch (index)
         {
            case  1: result = Localization.tip01; break;
            case  2: result = Localization.tip02; break;
            case  3: result = Localization.tip03; break;
            case  4: result = Localization.tip04; break;
            case  5: result = Localization.tip05; break;
            case  6: result = Localization.tip06; break;
            case  7: result = Localization.tip07; break;
            case  8: result = Localization.tip08; break;
            case  9: result = Localization.tip09; break;
            case 10: result = Localization.tip10; break;
            case 11: result = Localization.tip11; break;
            case 12: result = Localization.tip12; break;
            case 13: result = Localization.tip13; break;
            case 14: result = Localization.tip14; break;
            case 15: result = Localization.tip15; break;
            case 16: result = Localization.tip16; break;
            case 17: result = Localization.tip17; break;
            case 18: result = Localization.tip18; break;
            case 19: result = Localization.tip19; break;
            case 20: result = Localization.tip20; break;
            default: result = "No tip available"; break;
         }

         index++;
         if (index > 9) // we only have 9 instead of 20 tips available
         {
            index = 1;
         }

         Settings.AddOrUpdateValue(Settings.TipOfDayCount, index);

         return result;
      }
   }
}
