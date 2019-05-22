using System;
using Xamarin.Forms;
using System.Collections.Specialized;

namespace perf
{
   public partial class PageSetup : ContentPage
   {
      const int    SPEED_START    = 0;       // kph
      const int    SPEED_END      = 320;     // kph
      const int    DISTANCE_START = 0;       // meter
      const int    DISTANCE_END   = 3000;    // meter
      const int    ITEM_COUNT_MAX = 10;      // listView items
      const string SPINNER_SPACE  = "         ";

      Image                imgButtonBack;
      TapGestureRecognizer tgrButtonBack;
      CustomLabel          lblCaption;
      Image                imgButtonHelp;
      TapGestureRecognizer tgrButtonHelp;
      CustomLabel          lblMode;
      Image                imgSwitchSpeedDistance;
      TapGestureRecognizer tgrSwitchSpeedDistance;
      Button               btnAdd;
      Button               btnRemove;
      Button               btnDefault;
      Label                lblDebug;
      Picker               pickerStart;
      Picker               pickerEnd;
      Button               btnPickerStart;
      Button               btnPickerEnd;
      ListView             listView;
      ResultMode           mode;
      RunResult            runResult;

      public PageSetup(ResultMode resultMode)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         mode = resultMode;

         CreateRunResult();
         CreateListView();

         InitLayout();
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
         lblCaption.Text = Localization.pageSetupCaption;
         lblCaption.Size = CustomLabel.SIZE_CAPTION;
         lblCaption.TextColor = Color.FromHex("4D4C4A");
         layout.Children.Add
         (
            lblCaption,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - lblCaption.Width * 0.5), 
            Constraint.RelativeToParent(parent => parent.Height * 0.03)
         );

         imgButtonHelp = new Image();
         imgButtonHelp.Source = "@drawable/btn_help_setup.png";
         tgrButtonHelp = new TapGestureRecognizer();
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
         imgButtonHelp.GestureRecognizers.Add(tgrButtonHelp);
         layout.Children.Add
         (
            imgButtonHelp,
            Constraint.RelativeToParent(parent => parent.Width * 0.81), 
            Constraint.RelativeToParent(parent => parent.Height * 0.022)
         );

         // body

         lblMode = new CustomLabel();
         lblMode.Size = CustomLabel.SIZE_CAPTION_DE;
         if      (mode == ResultMode.Brake)      lblMode.Text      = Localization.runModeBrake;
         else if (mode == ResultMode.ZeroToZero) lblMode.Text      = Localization.runModeZeroToZero;
         else                                    lblMode.Text      = Localization.runModeAcceleration;

         if      (mode == ResultMode.Brake)      lblMode.TextColor = Color.FromHex("FFB02B");
         else if (mode == ResultMode.ZeroToZero) lblMode.TextColor = Color.FromHex("0CFF71");
         else                                    lblMode.TextColor = Color.FromHex("00FFFF");

         double modePosition;
         if      (mode == ResultMode.Brake)      modePosition      = 0.19;
         else if (mode == ResultMode.ZeroToZero) modePosition      = 0.19;
         else                                    modePosition      = 0.13;
         layout.Children.Add
         (
            lblMode,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - lblMode.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height * modePosition)
         );

         if (mode == ResultMode.Speed)
         {
            // mode switch

            imgSwitchSpeedDistance = new Image();
            imgSwitchSpeedDistance.Source = Localization.btn_setup_switch_speed;
            tgrSwitchSpeedDistance = new TapGestureRecognizer();
            tgrSwitchSpeedDistance.Tapped += OnButtonSwitchSpeedDistanceTapped;
            imgSwitchSpeedDistance.GestureRecognizers.Add(tgrSwitchSpeedDistance);
            layout.Children.Add
            (
               imgSwitchSpeedDistance,
               Constraint.RelativeToParent((parent) => parent.Width * 0.5 - imgSwitchSpeedDistance.Width * 0.5),
               Constraint.RelativeToParent((parent) => parent.Height * 0.20)
            );
            imgSwitchSpeedDistance.SizeChanged += ((obj, sender) => layout.ForceLayout());
         }

         // button default

         btnDefault = new Button();
         btnDefault.Image = "@drawable/btn_setup_default.png";
         btnDefault.BackgroundColor = Color.Transparent;
         btnDefault.Clicked += OnButtonDefault;
         layout.Children.Add
         (
            btnDefault,
            Constraint.RelativeToParent((parent) => parent.Width - btnDefault.Width * 0.9),
            Constraint.RelativeToParent((parent) => parent.Height * 0.15)
         );
         
         // picker start

         if      (mode == ResultMode.ZeroToZero) CreatePickerZeroToZeroStart();
         else if (mode == ResultMode.Brake)      CreatePickerBrakeStart();
         else                                    CreatePickerSpeedStart();
         pickerStart.IsVisible = false;
         pickerStart.SelectedIndexChanged += OnPickerSelectedIndexChanged;
         layout.Children.Add
         (
            pickerStart,
            Constraint.RelativeToParent((parent) => parent.Width * 0.03),
            Constraint.RelativeToParent((parent) => parent.Height * 0.27)
         );

         // button picker start

         btnPickerStart = new Button();
         btnPickerStart.Image = Localization.btn_setup_start;
         btnPickerStart.BackgroundColor = Color.Transparent;
         btnPickerStart.Clicked += OnButtonPickerStartCicked;
         layout.Children.Add
         (
            btnPickerStart,
            Constraint.RelativeToView(pickerStart, (parent, view) => view.X),
            Constraint.RelativeToView(pickerStart, (parent, view) => view.Y)
         );

         // picker end

         if      (mode == ResultMode.ZeroToZero) CreatePickerZeroToZeroEnd();
         else if (mode == ResultMode.Brake)      CreatePickerBrakeEnd();
         else                                    CreatePickerSpeedEnd();
         pickerEnd.IsVisible = false;
         pickerEnd.SelectedIndexChanged += OnPickerSelectedIndexChanged;
         layout.Children.Add
         (
            pickerEnd,
            Constraint.RelativeToParent((parent) => parent.Width * 0.9 - pickerEnd.Width),
            Constraint.RelativeToParent((parent) => parent.Height * 0.27)
         );

         // button picker end

         btnPickerEnd = new Button();
         btnPickerEnd.Image = Localization.btn_setup_stop;
         btnPickerEnd.BackgroundColor = Color.Transparent;
         btnPickerEnd.Clicked += OnButtonPickerEndCicked;
         layout.Children.Add
         (
            btnPickerEnd,
            Constraint.RelativeToView(pickerEnd, (parent, view) => view.X + view.Width * 0.25),
            Constraint.RelativeToView(pickerEnd, (parent, view) => view.Y)
         );

         // debug

         if (Config.Setup.DebugLabelEnabled)
         {
            lblDebug = new Label();
            SetDebugText("item count: " + runResult.Count);
            layout.Children.Add
            (
               lblDebug,
               Constraint.RelativeToParent((parent) => parent.Width * 0.5 - lblDebug.Width * 0.5),
               Constraint.RelativeToParent((parent) => parent.Height * 0.7)
            );
            lblDebug.SizeChanged += (sender, e) => layout.ForceLayout();
         }

         // add, remove

         btnAdd = new Button();
         btnAdd.BackgroundColor = Color.Transparent;
         SetButtonAddActive(false);
         btnAdd.Clicked += OnButtonAdd;
         layout.Children.Add
         (
            btnAdd,
            Constraint.RelativeToParent((parent) => parent.Width * 0.25 - btnAdd.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height - btnAdd.Height * 1.1)
         );

         btnRemove = new Button();
         btnRemove.Clicked += OnButtonRemove;
         btnRemove.BackgroundColor = Color.Transparent;
         SetButtonRemoveActive(false);
         layout.Children.Add
         (
            btnRemove,
            Constraint.RelativeToParent((parent) => parent.Width * 0.75 - btnRemove.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height - btnRemove.Height * 1.1)
         );

         // list view

         layout.Children.Add
         (
            listView,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - listView.Width * 0.5),
            Constraint.RelativeToView(pickerStart, (parent, view) => view.Y + view.Height * 1.3), 
            Constraint.RelativeToParent(parent => parent.Width * 0.8),
            Constraint.RelativeToParent(parent => btnAdd.Y - listView.Y)
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
         Analytics.TrackPage(Analytics.PAGE_SETUP);
      }

      protected override bool OnBackButtonPressed()
      {
         // inhibit page navigation if appstore overlay is visible (just remove overlay)
         // true  -> let appstore-overlay handle event
         // false -> let system handle event
         bool result = AppStore.Instance.OnBackButtonPressed();

         if (result == false)
         {
            // overlay was not visible -> do normal page back navigation
            StoreRunResult();
            result = base.OnBackButtonPressed();
         }

         return result;
      }

      async void OnButtonBackTapped(object sender, EventArgs args)
      {
         tgrButtonBack.Tapped -= OnButtonBackTapped;
         StoreRunResult();
         await Navigation.PopAsync();
         tgrButtonBack.Tapped += OnButtonBackTapped;
      }

      async void OnButtonHelpTapped(object sender, EventArgs args) 
      {
         tgrButtonHelp.Tapped -= OnButtonHelpTapped;
         await Navigation.PushAsync(new PageText(PageText.ContentType.Setup));
         tgrButtonHelp.Tapped += OnButtonHelpTapped;
      }

      void OnButtonSwitchSpeedDistanceTapped(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         // detach old mode from display
         listView.ItemsSource = null;

         // switch to new mode
         if (mode == ResultMode.Speed)
         {
            mode = ResultMode.Distance;
            CreatePickerDistanceStart();
            CreatePickerDistanceEnd();
            imgSwitchSpeedDistance.Source = Localization.btn_setup_switch_distance;
         }
         else if (mode == ResultMode.Distance)
         {
            mode = ResultMode.Speed;
            CreatePickerSpeedStart();
            CreatePickerSpeedEnd();
            imgSwitchSpeedDistance.Source = Localization.btn_setup_switch_speed;
         }
         else
         {
            SetDebugText("invalid mode");
         }

         // hide picker, display buttons instead
         pickerStart.IsVisible    = false;
         pickerEnd.IsVisible      = false;
         btnPickerStart.IsVisible = true;
         btnPickerEnd.IsVisible   = true;

         // disable add, remove
         SetButtonAddActive(false);
         SetButtonRemoveActive(false);

         // load new mode from settings
         CreateRunResult();

         // display new mode
         listView.ItemsSource = runResult.Items;

         Analytics.TrackEventSetup(Analytics.EVENT_SETUP_SWITCH_SPEED_DISTANCE);
      }

      void OnButtonAdd(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         if (pickerStart.SelectedIndex == -1)   return;
         if (pickerEnd.SelectedIndex   == -1)   return;
         if (runResult.Count >= ITEM_COUNT_MAX) return;

         string start = pickerStart.Items[pickerStart.SelectedIndex];
         string end   = pickerEnd.Items[pickerEnd.SelectedIndex];

         start = RemoveUnit(start);
         end   = RemoveUnit(end);

         if (IsItemValid(start, end))
         {
            // replace item if zero-to-zero mode
            if (mode == ResultMode.ZeroToZero)
            {
               runResult.Clear();
            }

            ResultItem item = runResult.Add(start, end);
            listView.ItemsSource = runResult.Items;
            listView.ScrollTo(item, ScrollToPosition.MakeVisible, true);
         }
         else
         {
            SetDebugText("invalid item");
         }

         Analytics.TrackEventSetup(Analytics.EVENT_SETUP_ADD);
      }

      void OnButtonRemove(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         if (listView.SelectedItem != null)
         {
            runResult.Remove((ResultItem)listView.SelectedItem);
         }

         Analytics.TrackEventSetup(Analytics.EVENT_SETUP_REMOVE);
      }

      void OnButtonDefault(object sender, EventArgs args)
      {
         runResult.Clear();
         runResult.AddDefaultItems();

         Analytics.TrackEventSetup(Analytics.EVENT_SETUP_DEFAULT);
      }
      
      void OnButtonPickerStartCicked(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         pickerStart.IsVisible = true;
         btnPickerStart.IsVisible = false;
         pickerStart.Focus(); // note: focus/unfocus events not fired by picker if this method is used
      }

      void OnButtonPickerEndCicked(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         pickerEnd.IsVisible = true;
         btnPickerEnd.IsVisible = false;
         pickerEnd.Focus(); // note: focus/unfocus events not fired by picker if this method is used
      }

      void OnPickerSelectedIndexChanged(object sender, EventArgs args)
      {
         if (IsPickerValid())
         {
            SetButtonAddActive(true);
         }
      }

      void OnListViewItemSelected(object sender, EventArgs args)
      {
         SetButtonRemoveActive(true);
      }

      void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         if (e.Action == NotifyCollectionChangedAction.Remove)
         {
            SetDebugText("item remove: " + e.OldItems[0] + " // count: " + runResult.Count);
            SetButtonRemoveActive(false);
            SetButtonAddActive(IsPickerValid());               
         }
         else if (e.Action == NotifyCollectionChangedAction.Add)
         {
            SetDebugText("item add: " + e.NewItems[0] + " // count: " + runResult.Count);
            SetButtonAddActive(false);

            if (runResult.Count >= ITEM_COUNT_MAX)
            {
               SetDebugText("item limit reached: " + ITEM_COUNT_MAX);
            }
         }
      }

      void CreatePickerSpeedStart()
      {
         if (pickerStart == null) pickerStart = new Picker();
         else                     pickerStart.Items.Clear();

         pickerStart.Title = Localization.pageSetupSpeedStart + SPINNER_SPACE;

         string value;
         for (int i = SPEED_START; i <= SPEED_END; i += 10)
         {
            value = "Start @" + i + Settings.GetSpeedUnit();
            pickerStart.Items.Add(value);
         }
      }

      void CreatePickerSpeedEnd()
      {
         if (pickerEnd == null) pickerEnd = new Picker();
         else                   pickerEnd.Items.Clear();

         pickerEnd.Title = Localization.pageSetupSpeedEnd + SPINNER_SPACE;

         string value;
         for (int i = SPEED_START; i <= SPEED_END; i += 10)
         {
            value = "End @" + i + Settings.GetSpeedUnit();
            pickerEnd.Items.Add(value);
         }
      }

      void CreatePickerDistanceStart()
      {
         if (pickerStart == null) pickerStart = new Picker();
         else                     pickerStart.Items.Clear();

         pickerStart.Title = Localization.pageSetupDistStart + SPINNER_SPACE;

         string value;
         for (int i = DISTANCE_START; i <= DISTANCE_END; i += 50)
         {
            value = "Start @" + i + Settings.GetDistanceUnit();
            pickerStart.Items.Add(value);
         }
      }

      void CreatePickerDistanceEnd()
      {
         if (pickerEnd == null) pickerEnd = new Picker();
         else                   pickerEnd.Items.Clear();

         pickerEnd.Title = Localization.pageSetupDistEnd + SPINNER_SPACE;

         string value;
         for (int i = DISTANCE_START; i <= DISTANCE_END; i += 50)
         {
            value = "End @" + i + Settings.GetDistanceUnit();
            pickerEnd.Items.Add(value);
         }
      }

      void CreatePickerBrakeStart()
      {
         if (pickerStart == null) pickerStart = new Picker();
         else                     pickerStart.Items.Clear();

         pickerStart.Title = Localization.pageSetupSpeedStart + SPINNER_SPACE;

         string value;
         for (int i = SPEED_START + 10; i <= SPEED_END; i += 10)
         {
            value = "Start @" + i + Settings.GetSpeedUnit();
            pickerStart.Items.Add(value);
         }
      }

      void CreatePickerBrakeEnd()
      {
         if (pickerEnd == null) pickerEnd = new Picker();
         else                   pickerEnd.Items.Clear();

         pickerEnd.Title = Localization.pageSetupSpeedEnd + SPINNER_SPACE;

         string value;
         for (int i = SPEED_START; i <= SPEED_END; i += 10)
         {
            value = "End @" + i + Settings.GetSpeedUnit();
            pickerEnd.Items.Add(value);
         }
      }

      void CreatePickerZeroToZeroStart()
      {
         if (pickerStart == null) pickerStart = new Picker();
         else                     pickerStart.Items.Clear();

         pickerStart.Title = Localization.pageSetupZeroStart + SPINNER_SPACE;

         string value;
         value = "Start @" + 0 + Settings.GetSpeedUnit();
         pickerStart.Items.Add(value);
      }

      void CreatePickerZeroToZeroEnd()
      {
         if (pickerEnd == null) pickerEnd = new Picker();
         else                   pickerEnd.Items.Clear();

         pickerEnd.Title = Localization.pageSetupZeroEnd + SPINNER_SPACE;

         string value;
         for (int i = SPEED_START + 10; i <= SPEED_END; i += 10)
         {
            value = "Target @" + i + Settings.GetSpeedUnit();
            pickerEnd.Items.Add(value);
         }
      }
      
      void CreateListView()
      {
         listView = new ListView 
         {
            ItemTemplate = new DataTemplate(() => 
            {
               Label labelLeft = new Label();
               labelLeft.HorizontalOptions = LayoutOptions.StartAndExpand;
               labelLeft.VerticalOptions = LayoutOptions.CenterAndExpand;
               //labelLeft.FontSize *= 1.5;
               labelLeft.SetBinding(Label.TextProperty, "Left");

               Label labelRight = new Label();
               labelRight.HorizontalOptions = LayoutOptions.EndAndExpand;
               labelRight.VerticalOptions = LayoutOptions.CenterAndExpand;
               //labelRight.FontSize *= 1.5;
               labelRight.SetBinding(Label.TextProperty, "Right");

               StackLayout line = new StackLayout();
               line.Orientation = StackOrientation.Horizontal;
               line.Children.Add(labelLeft);
               line.Children.Add(labelRight);

               ViewCell result = new ViewCell();
               result.View = line;

               return result;
            })
         };

         listView.ItemsSource = runResult.Items;
         listView.ItemSelected += OnListViewItemSelected;
      }

      void CreateRunResult()
      {
         if (runResult != null)
         {
            // save old mode to settings
            StoreRunResult();
         }

         runResult = new RunResult(mode);
         runResult.Load();

         runResult.CollectionChanged += OnCollectionChanged;
      }

      void StoreRunResult()
      {
         // ensure valid setup
         if (runResult.Items.Count == 0)
         {
            runResult.Clear();
            runResult.AddDefaultItems();
         }

         runResult.Store();
      }

      string RemoveUnit(string s)
      {
         s = s.Replace(Localization.unitKph,   "");
         s = s.Replace(Localization.unitMph,   "");
         s = s.Replace(Localization.unitMeter, "");
         s = s.Replace(Localization.unitFeet,  "");

         return s;
      }

      bool IsItemValid(string start, string end)
      {
         bool       result    = true;
         ResultItem item      = new ResultItem(start, end);
         string     strStart;
         string     strEnd;
         int        s;
         int        e;

         item.GetStartEnd(out strStart, out strEnd);
         s = int.Parse(strStart);
         e = int.Parse(strEnd);

         // accel: check if start speed > end speed
         if ((mode == ResultMode.Speed) || (mode == ResultMode.Distance))
         {
            if (s >= e) result = false;
         }

         // brake: check if end dist > start dist
         if (mode == ResultMode.Brake)
         {
            if (e >= s) result = false;
         }

         // ensure unique items
         foreach (ResultItem ri in runResult.Items) 
         {
            if (item.CompareTo(ri) == 0) result = false;
         }

         return result;
      }

      bool IsPickerValid()
      {
         if (pickerStart.SelectedIndex == -1)   return false;
         if (pickerEnd.SelectedIndex   == -1)   return false;
         if (runResult.Count >= ITEM_COUNT_MAX) return false;

         string start = pickerStart.Items[pickerStart.SelectedIndex];
         string end   = pickerEnd.Items[pickerEnd.SelectedIndex];

         start = RemoveUnit(start);
         end   = RemoveUnit(end);

         if (IsItemValid(start, end))
         {
            return true;
         }

         return false;
      }

      void SetButtonAddActive(bool isActive)
      {
         if (isActive)
         {
            btnAdd.Image = Localization.btn_setup_add;
            btnAdd.IsEnabled = true;
         }
         else
         {
            btnAdd.Image = Localization.btn_setup_add_inactive;
            btnAdd.IsEnabled = false;
         }
      }

      void SetButtonRemoveActive(bool isActive)
      {
         if (isActive)
         {
            btnRemove.Image = Localization.btn_setup_remove;
            btnRemove.IsEnabled = true;
         }
         else
         {
            btnRemove.Image = Localization.btn_setup_remove_inactive;
            btnRemove.IsEnabled = false;
         }
      }

      void SetDebugText(string s)
      {
         if (Config.Setup.DebugLabelEnabled) lblDebug.Text = s;
      }
   }
}
