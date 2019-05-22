using System;
using Xamarin.Forms;
using System.Text;

namespace perf
{
   public partial class PageHelp : ContentPage, IDisposable
   {
      public const string ModeAcceleration = "Acceleration";
      public const string ModeBrake        = "Brake";
      public const string ModeZeroToZero   = "ZeroToZero";
      public const string ModeAppStore     = "AppStore";

      string               mode;
      Image                imgButtonBack;
      TapGestureRecognizer tgrButtonBack;
      CustomLabel          lblCaption;
      Image                imgButtonHelp;
      ScrollView           scrollView;
      CustomLabel          lblText;

      public PageHelp(string mode)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         this.mode = mode;

         InitLayout();
      }

      public void Dispose() // IDisposable
      {
         imgButtonBack.Source = null;
         imgButtonBack.GestureRecognizers.RemoveAt(0);
         imgButtonBack = null;
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         tgrButtonBack = null;

         lblCaption = null;

         imgButtonHelp.Source = null;
         imgButtonHelp = null;

         lblText.SizeChanged -= OnLayoutSizeChanged;
         lblText = null;

         scrollView = null;
         mode = null;
      }

      void InitLayout()
      {
         // head

         imgButtonBack = new Image();
         imgButtonBack.Source = "@drawable/btn_back.png";
         tgrButtonBack = new TapGestureRecognizer();
         tgrButtonBack.Tapped += OnButtonBackTapped;
         imgButtonBack.GestureRecognizers.Add(tgrButtonBack);
         layout.Children.Add
         (
            imgButtonBack,
            Constraint.RelativeToParent(parent => parent.Width * 0.036),
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         lblCaption = new CustomLabel();
         lblCaption.Text = Localization.pageHelpHead;
         lblCaption.Size = CustomLabel.SIZE_CAPTION;
         lblCaption.TextColor = Color.FromHex("4D4C4A");
         layout.Children.Add
         (
            lblCaption,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - lblCaption.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.03)
         );

         imgButtonHelp = new Image();
         imgButtonHelp.Source = "@drawable/icn_help.png";
         layout.Children.Add
         (
            imgButtonHelp,
            Constraint.RelativeToParent(parent => parent.Width * 0.81),
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         // body
            
         lblText = new CustomLabel();
         lblText.Size = CustomLabel.SIZE_LARGE;
         lblText.TextColor = Color.White;
         lblText.Text = GetText();
         lblText.SizeChanged += OnLayoutSizeChanged;

         scrollView = new ScrollView();
         scrollView.Content = lblText;
         layout.Children.Add
         (
            scrollView,
            Constraint.RelativeToParent(parent => parent.Width / 2 - scrollView.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.2),
            Constraint.RelativeToParent(parent => parent.Width * 0.8),
            Constraint.RelativeToParent(parent => parent.Height * 0.78)
         );
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }

      void OnLayoutSizeChanged(object sender, EventArgs args)
      {
         layout.ForceLayout();
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_HELP);
      }

      async void OnButtonBackTapped(object sender, EventArgs args)
      {
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         await Navigation.PopAsync();
         tgrButtonBack.Tapped += OnButtonBackTapped;
      }
      
      string GetText()
      {
         return GetTextHelp()       + 
                Environment.NewLine + 
                Environment.NewLine +
                GetTextPurchase()   + 
                Environment.NewLine + 
                GetTextAbout()      +
                Environment.NewLine + 
                GetTextContact();
      }

      string GetTextHelp()
      {
         string result;

         switch (mode)
         {
            case ModeAcceleration: result = Localization.pageHelpMode1;    break;
            case ModeBrake:        result = Localization.pageHelpMode2;    break;
            case ModeZeroToZero:   result = Localization.pageHelpMode3;    break;
            case ModeAppStore:     result = Localization.pageHelpAppStore; break;
            default:               result = "Sorry, No Help Available";    break;
         }

         return result;
      }

      string GetTextPurchase()
      {
         StringBuilder sb = new StringBuilder();
         string yes       = Localization.partYes;
         string no        = Localization.partNo;
         string newLine   = Environment.NewLine;

         sb.Append(Localization.pageHelpPurchase);
         sb.Append(":");
         sb.Append(newLine);

         sb.Append(Config.Purchase.ProductSku);
         sb.Append(": ");
         sb.Append((AppStore.Instance.IsPurchased(Config.Purchase.ProductSku) == true) ? yes : no);
         sb.Append(newLine);

         return sb.ToString();
      }

      string GetTextAbout()
      {
         StringBuilder sb = new StringBuilder();
         IDeviceInfo info = DependencyService.Get<IDeviceInfo>();
         string newLine   = Environment.NewLine;

         sb.Append(Localization.pageHelpAbout);
         sb.Append(":");
         sb.Append(newLine);

         sb.Append("Name: ");
         sb.Append(info.AppName);
         sb.Append(newLine);

         sb.Append("Version: ");
         sb.Append(info.AppVersion);
         sb.Append(newLine);

         return sb.ToString();
      }

      string GetTextContact()
      {
         StringBuilder sb = new StringBuilder();
         IDeviceInfo info = DependencyService.Get<IDeviceInfo>();
         string newLine   = Environment.NewLine;

         sb.Append(Localization.pageHelpContact);
         sb.Append(": ");
         sb.Append(info.AppContact);
         sb.Append(newLine);
         sb.Append(newLine);

         sb.Append(Localization.pageHelpFollow);
         sb.Append(newLine);

         return sb.ToString();
      }
   }
}
