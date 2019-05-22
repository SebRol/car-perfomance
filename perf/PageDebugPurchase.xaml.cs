using System;
using Xamarin.Forms;

namespace perf
{
   public partial class PageDebugPurchase : ContentPage, IPurchaseListener
   {
      CustomLabel lblCaption;

      Button      btnTestPurchaseBuy;
      Button      btnTestPurchaseConsume;

      Button      btnTestCancel;
      Button      btnTestRefund;
      Button      btnTestUnavailable;

      Button      btnDevelopmentBuy;
      Button      btnDevelopmentConsume;

      Button      btnUnlockerBuy;
      Button      btnUnlockerConsume;

      Button      btnClearLog;

      Label       lblStatus;
      ScrollView  scrollView;

      public PageDebugPurchase()
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         InitLayout();
      }

      void InitLayout()
      {
         lblCaption = new CustomLabel();
         lblCaption.Text = "Debug - In App Purchase";
         lblCaption.Size = CustomLabel.SIZE_CAPTION;
         lblCaption.TextColor = Color.FromHex("4D4C4A");
         lblCaption.SizeChanged += OnLayoutSizeChanged;
         layout.Children.Add
         (
            lblCaption,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - lblCaption.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.03)
         );

         // buttons to test purchase
         
         btnTestPurchaseBuy = new Button();
         btnTestPurchaseBuy.Text = "Purchase Dummy";
         btnTestPurchaseBuy.Clicked += OnButtonDummyPurchaseBuy;

         layout.Children.Add
         (
            btnTestPurchaseBuy,
            Constraint.RelativeToParent(parent => parent.Width * 0.05),
            Constraint.RelativeToParent(parent => parent.Height * 0.1)
         );
         
         btnTestPurchaseConsume = new Button();
         btnTestPurchaseConsume.Text = "Consume Dummy";
         btnTestPurchaseConsume.Clicked += OnButtonDummyPurchaseConsume;

         layout.Children.Add
         (
            btnTestPurchaseConsume,
            Constraint.RelativeToParent(parent => parent.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.1)
         );

         // buttons to test cancel, refund, unavailable

         btnTestCancel = new Button();
         btnTestCancel.Text = "Test Cancel";
         btnTestCancel.Clicked += OnButtonTestCancel;

         layout.Children.Add
         (
            btnTestCancel,
            Constraint.RelativeToParent(parent => parent.Width * 0.05),
            Constraint.RelativeToParent(parent => parent.Height * 0.2)
         );

         btnTestRefund = new Button();
         btnTestRefund.Text = "Test Refund";
         btnTestRefund.Clicked += OnButtonTestRefund;

         layout.Children.Add
         (
            btnTestRefund,
            Constraint.RelativeToParent(parent => parent.Width * 0.05),
            Constraint.RelativeToParent(parent => parent.Height * 0.3)
         );

         btnTestUnavailable = new Button();
         btnTestUnavailable.Text = "Test Unavailable";
         btnTestUnavailable.Clicked += OnButtonTestUnavailable;

         layout.Children.Add
         (
            btnTestUnavailable,
            Constraint.RelativeToParent(parent => parent.Width * 0.05),
            Constraint.RelativeToParent(parent => parent.Height * 0.4)
         );

         // button to clear log

         btnClearLog = new Button();
         btnClearLog.Text = "Clear Log";
         btnClearLog.Clicked += OnButtonClearLog;

         layout.Children.Add
         (
            btnClearLog,
            Constraint.RelativeToParent(parent => parent.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.4)
         );
   
         // buttons to test development buy

         btnDevelopmentBuy = new Button();
         btnDevelopmentBuy.Text = "Buy Development ! USE REAL MONEY !";
         btnDevelopmentBuy.Clicked += OnButtonDevelopmentBuy;
         btnDevelopmentBuy.SizeChanged += OnLayoutSizeChanged;

         layout.Children.Add
         (
            btnDevelopmentBuy,
            Constraint.RelativeToParent(parent => parent.Width * 0.05),
            Constraint.RelativeToParent(parent => parent.Height * 0.5)
         );

         btnDevelopmentConsume = new Button();
         btnDevelopmentConsume.Text = "Consume Development";
         btnDevelopmentConsume.Clicked += OnButtonDevelopmentConsume;
         btnDevelopmentConsume.SizeChanged += OnLayoutSizeChanged;

         layout.Children.Add
         (
            btnDevelopmentConsume,
            Constraint.RelativeToParent(parent => parent.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.2)
         );

         // buttons to test unlocker buy

         btnUnlockerBuy = new Button();
         btnUnlockerBuy.Text = "Buy Unlocker ! USE REAL MONEY !";
         btnUnlockerBuy.Clicked += OnButtonUnlockerBuy;
         btnUnlockerBuy.SizeChanged += OnLayoutSizeChanged;

         layout.Children.Add
         (
            btnUnlockerBuy,
            Constraint.RelativeToParent(parent => parent.Width * 0.05),
            Constraint.RelativeToParent(parent => parent.Height * 0.6)
         );

         btnUnlockerConsume = new Button();
         btnUnlockerConsume.Text = "Consume Unlocker";
         btnUnlockerConsume.Clicked += OnButtonUnlockerConsume;
         btnUnlockerConsume.SizeChanged += OnLayoutSizeChanged;

         layout.Children.Add
         (
            btnUnlockerConsume,
            Constraint.RelativeToParent(parent => parent.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.3)
         );

         // text output of test

         lblStatus = new Label();
         lblStatus.Text = "-------------------------- log --------------------------";
         lblStatus.SizeChanged += OnLayoutSizeChanged;

         scrollView = new ScrollView();
         scrollView.Content = lblStatus;
         layout.Children.Add
         (
            scrollView,
            Constraint.RelativeToParent(parent => parent.Width / 2 - scrollView.Width * 0.5),
            Constraint.RelativeToParent(parent => parent.Height * 0.7),
            Constraint.RelativeToParent(parent => parent.Width * 0.95),
            Constraint.RelativeToParent(parent => parent.Height * 0.25)
         );
      }

      public void OnSleep()
      {
      }

      public void OnResume()
      {
      }

      protected override void OnAppearing()
      {
         base.OnAppearing();

         Debug.LogToFileMethod();
         Analytics.TrackPage(Analytics.PAGE_PURCHASE);
         AppStore.Instance.SetListener(this);
      }

      public void OnPurchaseStatus(PurchaseStatus status) // IPurchaseListener
      {
         // called from app-store thread -> do not update views (use UI thread instead)

         switch (status)
         {
            case PurchaseStatus.Connected:
            Log("OnStatusUpdate Connected");
            LogPurchases();
            break;

            case PurchaseStatus.Disconnected:
            Log("OnStatusUpdate Disconnected");
            break;

            case PurchaseStatus.Purchased:
            Log("OnStatusUpdate Purchased");
            break;

            case PurchaseStatus.Consumed:
            Log("OnStatusUpdate Consumed");
            break;

            case PurchaseStatus.Cancelled:
            Log("OnStatusUpdate Cancelled");
            break;

            default:
            break;
         }
      }

      public void OnPurchaseLog(string message) // IPurchaseListener
      {
         Log(message);
      }

      void OnLayoutSizeChanged(object sender, EventArgs args)
      {
         layout.ForceLayout();
      }

      void OnButtonDummyPurchaseBuy(object sender, EventArgs args)
      {
         Log("OnButtonDummyPurchaseBuy");
         AppStore.Instance.Purchase(PurchaseType.Dummy);
      }
      
      void OnButtonDummyPurchaseConsume(object sender, EventArgs args)
      {
         Log("OnButtonDummyPurchaseConsume");
         AppStore.Instance.Consume(PurchaseType.Dummy);
      }

      void OnButtonTestCancel(object sender, EventArgs args)
      {
         Log("OnButtonTestCancel");
         AppStore.Instance.Purchase(PurchaseType.Cancel);
      }

      void OnButtonTestRefund(object sender, EventArgs args)
      {
         Log("OnButtonTestRefund");
         AppStore.Instance.Purchase(PurchaseType.Refund);
      }

      void OnButtonTestUnavailable(object sender, EventArgs args)
      {
         Log("OnButtonTestUnavailable");
         AppStore.Instance.Purchase(PurchaseType.Unavailable);
      }

      void OnButtonDevelopmentBuy(object sender, EventArgs args)
      {
         Log("OnButtonDevelopmentBuy");
         AppStore.Instance.Purchase(PurchaseType.Development);
      }

      void OnButtonDevelopmentConsume(object sender, EventArgs args)
      {
         Log("OnButtonDevelopmentConsume");
         AppStore.Instance.Consume(PurchaseType.Development);
      }

      void OnButtonUnlockerBuy(object sender, EventArgs args)
      {
         Log("OnButtonUnlockerBuy");
         AppStore.Instance.Purchase(PurchaseType.Unlocker);
      }

      void OnButtonUnlockerConsume(object sender, EventArgs args)
      {
         Log("OnButtonUnlockerConsume");
         AppStore.Instance.Consume(PurchaseType.Unlocker);
      }

      void OnButtonClearLog(object sender, EventArgs args)
      {
         Log("OnButtonClearLog");
         lblStatus.Text = "";
         scrollView.ScrollToAsync(0, 0, false);
         lblStatus.Text = "-------------------------- log --------------------------";
      }

      void LogPurchases()
      {
         Log("Dummy purchased: "       + AppStore.Instance.IsPurchased(PurchaseType.Dummy));
         Log("Development purchased: " + AppStore.Instance.IsPurchased(PurchaseType.Development));
         Log("Unlocker purchased: "    + AppStore.Instance.IsPurchased(PurchaseType.Unlocker));
      }

      void Log(string s)
      {
         Device.BeginInvokeOnMainThread (() => // in app purchase worker threads may have called us
         {
            lblStatus.Text += "\n" + s;
            scrollView.ScrollToAsync(0, 999999, false);
         });
      }
   }
}
