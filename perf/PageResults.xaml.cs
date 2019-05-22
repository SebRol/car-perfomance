using System;
using System.Text;
using Xamarin.Forms;
using System.Collections.Generic;

namespace perf
{
   public partial class PageResults : ContentPage
   {
      string             mode;
      StackLayout        lytHorizontalTab;
      Button             btnTabAcceleration;
      Button             btnTabBrake;
      Button             btnTabZeroToZero;
      Button             btnTabResults;
      Button             btnSpeed;
      Button             btnDistance;
      Button             btnHeight;
      Button             btnPrevious;
      Button             btnNext;
      Button             btnShare;
      ListView           listView;
      List<ResultItem>   listViewItems;
      Button             btnNew;
      DataItemRun        itemRun;
      Database           database;
      CustomLabel        lblRunInfo;
      CustomPlot         plotView;
      Button             btnDelete;

      public PageResults(string mode)
      {
         InitializeComponent();
         Debug.LogToFileMethod();

         NavigationPage.SetHasBackButton(this, false);
         NavigationPage.SetHasNavigationBar(this, false);

         this.mode = mode;
         database = Database.GetInstance();
         itemRun = database.GetRunLast(mode);

         CreateListView();
         InitLayout();

         UpdateItemRun();
      }

      void InitLayout()
      {
         // tab of 4 buttons on top

         lytHorizontalTab = new StackLayout();
         lytHorizontalTab.Orientation = StackOrientation.Horizontal;
         lytHorizontalTab.HorizontalOptions = LayoutOptions.CenterAndExpand;
         lytHorizontalTab.Spacing = -1;

         btnTabAcceleration = new Button();
         btnTabAcceleration.Image = Localization.tab_acceleration;
         btnTabAcceleration.BackgroundColor = Color.Transparent;
         btnTabAcceleration.Clicked += OnButtonTabAcceleration;

         btnTabBrake = new Button();
         btnTabBrake.Image = Localization.tab_brake;
         btnTabBrake.BackgroundColor = Color.Transparent;
         btnTabBrake.Clicked += OnButtonTabBrake;

         btnTabZeroToZero = new Button();
         btnTabZeroToZero.Image = Localization.tab_zero;
         btnTabZeroToZero.BackgroundColor = Color.Transparent;
         btnTabZeroToZero.Clicked += OnButtonTabZeroToZero;

         btnTabResults = new Button();
         btnTabResults.BackgroundColor = Color.Transparent;
         btnTabResults.Clicked += OnButtonTabResults;

         if (mode.Equals("Acceleration"))
         {
            btnTabAcceleration.Image = Localization.tab_acceleration_active;
            btnTabResults.Image = Localization.tab_acceleration_results;
         }
         else if (mode.Equals("Brake"))
         {
            btnTabBrake.Image = Localization.tab_brake_active;
            btnTabResults.Image = Localization.tab_brake_results;
         }
         else
         {
            btnTabZeroToZero.Image = Localization.tab_zero_active;
            btnTabResults.Image = Localization.tab_zero_results;
         }

         lytHorizontalTab.Children.Add(btnTabAcceleration);
         lytHorizontalTab.Children.Add(btnTabBrake);
         lytHorizontalTab.Children.Add(btnTabZeroToZero);
         lytHorizontalTab.Children.Add(btnTabResults);

         layout.Children.Add
         (
            lytHorizontalTab,
            Constraint.RelativeToParent((parent) => parent.X),
            Constraint.RelativeToParent((parent) => parent.Y),
            Constraint.RelativeToParent((parent) => parent.Width)
         );

         lytHorizontalTab.SizeChanged += OnLayoutSizeChanged;

         // mode button distance

         btnDistance = new Button();
         btnDistance.Image = Localization.btn_speeddist;
         btnDistance.BackgroundColor = Color.Transparent;
         btnDistance.Clicked += OnButtonDistance;

         layout.Children.Add
         (
            btnDistance,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - btnSpeed.Width * 0.5),
            Constraint.RelativeToView(lytHorizontalTab, (parent, view) => view.Y + view.Height * 0.9)
         );

         // mode button speed (left of DISTANCE)

         btnSpeed = new Button();
         btnSpeed.Image = Localization.btn_speedtime_active;
         btnSpeed.BackgroundColor = Color.Transparent;
         btnSpeed.Clicked += OnButtonSpeed;

         layout.Children.Add
         (
            btnSpeed,
            Constraint.RelativeToView(btnDistance, (parent, View) => View.X - btnSpeed.Width * 1.04),
            Constraint.RelativeToView(btnDistance, (parent, view) => view.Y)
         );

         // mode button height (right of DISTANCE)

         btnHeight = new Button();
         btnHeight.Image = Localization.btn_height;
         btnHeight.BackgroundColor = Color.Transparent;
         btnHeight.Clicked += OnButtonHeight;

         layout.Children.Add
         (
            btnHeight,
            Constraint.RelativeToView(btnDistance, (parent, View) => View.X + btnSpeed.Width * 1.04),
            Constraint.RelativeToView(btnDistance, (parent, view) => view.Y)
         );

         // share button

         btnShare = new Button();
         btnShare.Image = "@drawable/btn_share.png";
         btnShare.BackgroundColor = Color.Transparent;
         btnShare.Clicked += OnButtonShare;

         layout.Children.Add
         (
            btnShare,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - btnShare.Width * 0.5),
            Constraint.RelativeToView(btnDistance, (parent, view) => view.Y + view.Height * 1.08)
         );

         // previous button (left of SHARE)

         btnPrevious = new Button();
         btnPrevious.Image = "@drawable/btn_previous.png";
         btnPrevious.BackgroundColor = Color.Transparent;
         btnPrevious.Clicked += OnButtonPrevious;

         layout.Children.Add
         (
            btnPrevious,
            Constraint.RelativeToParent(parent => parent.Width * 0.15 - btnPrevious.Width * 0.5),
            Constraint.RelativeToView(btnShare, (parent, view) => view.Y)
         );

         // next button (right of SHARE)

         btnNext = new Button();
         btnNext.Image = "@drawable/btn_next.png";
         btnNext.BackgroundColor = Color.Transparent;
         btnNext.Clicked += OnButtonNext;

         layout.Children.Add
         (
            btnNext,
            Constraint.RelativeToParent(parent => parent.Width * 0.85 - btnNext.Width * 0.5),
            Constraint.RelativeToView(btnShare, (parent, view) => view.Y)
         );

         // new run button

         btnNew = new Button();

         btnNew.BackgroundColor = Color.Transparent;
         btnNew.Clicked += OnButtonNew;
         ShowButtonRun();
         layout.Children.Add
         (
            btnNew,
            Constraint.RelativeToParent((parent) => parent.Width * 0.5 - btnNew.Width * 0.5),
            Constraint.RelativeToParent((parent) => parent.Height - btnNew.Height)
         );

         // results in a list view
         // + label with info of run         in the header of the list
         // + plot of run                    in the footer of the list
         // + button to delete displayed run in the footer of the list

         // list view header : info label

         lblRunInfo = new CustomLabel();
         lblRunInfo.Size = CustomLabel.SIZE_MEDIUM;
         lblRunInfo.HorizontalTextAlignment = TextAlignment.Center;
         lblRunInfo.Text = GetRunInfo();

         StackLayout headerLayout = new StackLayout();
         headerLayout.Children.Add(lblRunInfo);
         headerLayout.Children.Add(new Label {Text = " "}); // add some space between run-info and result-list
         listView.Header = headerLayout;

         // list view footer : plot

         plotView = new CustomPlot();
         plotView.HeightRequest = 280;

         // list view footer : delete button

         btnDelete = new Button();
         btnDelete.Image = Localization.btn_delete;
         btnDelete.BackgroundColor = Color.Transparent;
         btnDelete.Clicked += OnButtonDelete;

         StackLayout footerLayout = new StackLayout();
         footerLayout.Children.Add(new Label {Text = " "}); // add some space between result-list and plot
         footerLayout.Children.Add(plotView);
         footerLayout.Children.Add(btnDelete);
         listView.Footer = footerLayout;

         layout.Children.Add
               (
                  btnNext,
                  Constraint.RelativeToParent(parent => parent.Width * 0.85 - btnNext.Width * 0.5),
                  Constraint.RelativeToView(btnShare, (parent, view) => view.Y)
                 );

         layout.Children.Add
         (
            listView,
            Constraint.RelativeToParent(parent => parent.Width * 0.5 - listView.Width * 0.5),
            Constraint.RelativeToView(btnShare, (parent, view) => view.Y + view.Height * 1.1),
            Constraint.RelativeToParent(parent => parent.Width * 0.8),
            Constraint.RelativeToView(btnNew, (parent, view) => view.Y - listView.Y)
         );
      }

      protected override void OnAppearing()
      {
         Analytics.TrackPage(Analytics.PAGE_RESULT);
      }

      protected override bool OnBackButtonPressed()
      {
         // inhibit page navigation if appstore overlay is visible (just remove overlay)
         return AppStore.Instance.OnBackButtonPressed();
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

      void OnButtonTabAcceleration(object sender, EventArgs args)
      {
         if (btnTabAcceleration.Image.File.Contains("_active") == false) // if already active -> return
         {
            ShowTabAcceleration();
            mode = RunModeAcceleration.Mode;
            UpdateItemRun(database.GetRunLast(mode));
         }
      }

      void OnButtonTabBrake(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         if (btnTabBrake.Image.File.Contains("_active") == false) // if already active -> return
         {
            ShowTabBrake();
            mode = RunModeBrake.Mode;
            UpdateItemRun(database.GetRunLast(mode));
         }
      }

      void OnButtonTabZeroToZero(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         if (btnTabZeroToZero.Image.File.Contains("_active") == false) // if already active -> return
         {
            ShowTabZeroToZero();
            mode = RunModeZeroToZero.Mode;
            UpdateItemRun(database.GetRunLast(mode));
         }
      }

      void OnButtonTabResults(object sender, EventArgs args)
      {
      }

      void OnButtonSpeed(object sender, EventArgs args)
      {
         if (btnSpeed.Image.File.Contains("active") == false) // if already active -> return
         {
            ShowVariantSpeed();
            CreateResultTime(SubMode.TimeSpeed, Localization.unitSecond, Localization.pageResultSpeed, Settings.GetSpeedUnit());

            if (mode.Equals(RunModeAcceleration.Mode))
            {
               AddResultDistance(SubMode.TimeDistance, Localization.unitSecond, Localization.pageResultDistance, Settings.GetDistanceUnit());
            }

            Analytics.TrackEventResult(Analytics.EVENT_RESULT_SPEED);
         }
      }

      void OnButtonDistance(object sender, EventArgs args)
      {
         if (btnDistance.Image.File.Contains("active") == false) // if already active -> return
         {
            ShowVariantDistance();
            CreateResultTime(SubMode.Distance, Settings.GetDistanceUnit(), Localization.pageResultSpeed, Settings.GetSpeedUnit());
            Analytics.TrackEventResult(Analytics.EVENT_RESULT_DISTANCE);
         }
      }

      void OnButtonHeight(object sender, EventArgs args)
      {
         if (btnHeight.Image.File.Contains("active") == false) // if already active -> return
         {
            ShowVariantHeight();
            CreateResultTime(SubMode.Height, Settings.GetDistanceUnit(), Localization.pageResultSpeed, Settings.GetSpeedUnit());
            Analytics.TrackEventResult(Analytics.EVENT_RESULT_HEIGHT);
         }
      }

      void OnButtonPrevious(object sender, EventArgs args)
      {
         DataItemRun item = database.GetRunPrevious(mode, itemRun.Id);

         if (IsItemRunEmpty(item) == false)
         {
            UpdateItemRun(item);
            Analytics.TrackEventResult(Analytics.EVENT_RESULT_PREVIOUS);
         }
      }

      void OnButtonNext(object sender, EventArgs args)
      {
         DataItemRun item = database.GetRunNext(mode, itemRun.Id);

         if (IsItemRunEmpty(item) == false)
         {
            UpdateItemRun(item);
            Analytics.TrackEventResult(Analytics.EVENT_RESULT_NEXT);
         }
      }

      void OnButtonShare(object sender, EventArgs args)
      {
         // check if feature is purchased, show app-store if not purchased
         if (AppStore.Instance.IsAppStoreShown(layout)) return;

         IDeviceShare sharer    = DependencyService.Get<IDeviceShare>();
         string       title     = Localization.pageResultShareTitle;
         string       subject   = Localization.pageResultShareSubject + " " + itemRun.Id;
         string       text      = GetShareText();

         sharer.Share(title, subject, text);
         Analytics.TrackEventResult(Analytics.EVENT_RESULT_SHARE);
      }

      void OnButtonDelete(object sender, EventArgs args)
      {
         if (IsItemRunEmpty(itemRun) == false)
         {
            database.DeleteRun(itemRun);

            DataItemRun item = database.GetRunPrevious(mode, itemRun.Id);
            if (IsItemRunEmpty(item) == true)
            {
               item = database.GetRunNext(mode, itemRun.Id);
               if (IsItemRunEmpty(item) == true)
               {
                  item = database.GetRunLast(mode);
               }
            }

            UpdateItemRun(item);
            Analytics.TrackEventResult(Analytics.EVENT_RESULT_DELETE);
         }
      }

      void OnListViewSelected(object sender, EventArgs args)
      {
         // disable selection feature
         ((ListView)sender).SelectedItem = null;
      }

      async void OnButtonNew(object sender, EventArgs args)
      {
         btnNew.IsEnabled = false;
         await Navigation.PopAsync();
         btnNew.IsEnabled = true;
      }

      void ShowTabAcceleration()
      {
         btnTabAcceleration.Image = Localization.tab_acceleration_active;
         btnTabBrake.Image        = Localization.tab_brake;
         btnTabZeroToZero.Image   = Localization.tab_zero;
         btnTabResults.Image      = Localization.tab_acceleration_results;
      }

      void ShowTabBrake()
      {
         btnTabAcceleration.Image = Localization.tab_acceleration;
         btnTabBrake.Image        = Localization.tab_brake_active;
         btnTabZeroToZero.Image   = Localization.tab_zero;
         btnTabResults.Image      = Localization.tab_brake_results;
      }

      void ShowTabZeroToZero()
      {
         btnTabAcceleration.Image = Localization.tab_acceleration;
         btnTabBrake.Image        = Localization.tab_brake;
         btnTabZeroToZero.Image   = Localization.tab_zero_active;
         btnTabResults.Image      = Localization.tab_zero_results;
      }

      void ShowVariantSpeed()
      {
         btnSpeed.Image    = Localization.btn_speedtime_active;
         btnDistance.Image = Localization.btn_speeddist;
         btnHeight.Image   = Localization.btn_height;
      }

      void ShowVariantDistance()
      {
         btnSpeed.Image    = Localization.btn_speedtime;
         btnDistance.Image = Localization.btn_speeddist_active;
         btnHeight.Image   = Localization.btn_height;
      }

      void ShowVariantHeight()
      {
         btnSpeed.Image    = Localization.btn_speedtime;
         btnDistance.Image = Localization.btn_speeddist;
         btnHeight.Image   = Localization.btn_height_active;
      }

      void ShowButtonPrevious()
      {
         if (IsItemRunEmpty(database.GetRunPrevious(mode, itemRun.Id))) btnPrevious.IsVisible = false;
         else                                                           btnPrevious.IsVisible = true;
      }

      void ShowButtonNext()
      {
         if (IsItemRunEmpty(database.GetRunNext(mode, itemRun.Id))) btnNext.IsVisible = false;
         else                                                       btnNext.IsVisible = true;
      }

      void ShowButtonShare()
      {
         if (IsItemRunEmpty()) btnShare.IsVisible = false;
         else                  btnShare.IsVisible = true;
      }

      void ShowButtonDelete()
      {
         if (IsItemRunEmpty()) { btnDelete.Image = Localization.btn_delete_inactive; btnDelete.IsEnabled = false; }
         else                  { btnDelete.Image = Localization.btn_delete;          btnDelete.IsEnabled = true; }
      }

      void ShowButtonRun()
      {
         if      (mode.Equals(RunModeZeroToZero.Mode)) btnNew.Image = Localization.btn_mode3_new;
         else if (mode.Equals(RunModeBrake.Mode))      btnNew.Image = Localization.btn_mode2_new;
         else                                          btnNew.Image = Localization.btn_mode1_new;
      }

      void UpdateItemRun(DataItemRun item = null)
      {
         if (item != null) itemRun = item;

         lblRunInfo.Text = GetRunInfo();

         if (btnSpeed.Image.File.Contains("active"))
         {
            CreateResultTime(SubMode.TimeSpeed, Localization.unitSecond, Localization.pageResultSpeed, Settings.GetSpeedUnit());

            if (mode.Equals(RunModeAcceleration.Mode))
            {
               AddResultDistance(SubMode.TimeDistance, Localization.unitSecond, Localization.pageResultDistance, Settings.GetDistanceUnit());
            }
         }
         else if (btnDistance.Image.File.Contains("active"))
         {
            CreateResultTime(SubMode.Distance, Settings.GetDistanceUnit(), Localization.pageResultSpeed, Settings.GetSpeedUnit());
         }
         else
         {
            CreateResultTime(SubMode.Height, Settings.GetDistanceUnit(), Localization.pageResultSpeed, Settings.GetSpeedUnit());
         }

         ShowButtonPrevious();
         ShowButtonNext();
         ShowButtonShare();
         ShowButtonDelete();
         ShowButtonRun();
      }

      void CreateResultTime(SubMode subMode, string subModeUnit, string label, string modeUnit)
      {
         RunData    runData    = new RunData(mode);
         ResultMode resultMode = Tools.ToResultMode(mode);
         RunResult  runResult  = new RunResult(resultMode);

         runData.Load(itemRun);
         runResult.Load();

         listViewItems = new List<ResultItem>();
         listViewItems.Add(new ResultItem(label + " (" + modeUnit + ")", ""));

         Debug.LogToFile("run [id, date, time]: " + itemRun.Id + " // " + itemRun.Date.ToString("dd MMMM yyyy") + " // " + itemRun.Date.ToString("HH:mm"));

         foreach (ResultItem input in runResult.Items)
         {
            ResultItem output = runData.GetResult(input, subMode, subModeUnit);
            listViewItems.Add(output);
            Debug.LogToFile("result " + mode + " (" + subMode + ")" + ": " + output.Left + " " + modeUnit + " = " + output.Right);
         }

         listView.ItemsSource = listViewItems;

         plotView.Update(runData, subMode);

         if (Config.Result.CalibrationLabelEnabled)
         {
            lblRunInfo.Text = Tools.RemoveTillEnd(lblRunInfo.Text, Environment.NewLine + "Peak Acceleration");
         }
      }

      void AddResultDistance(SubMode subMode, string subModeUnit, string label, string modeUnit)
      {
         RunData    runData    = new RunData(mode);
         ResultMode resultMode = Tools.ToResultMode(mode);
         RunResult  runResult  = new RunResult(ResultMode.Distance);

         runData.Load(itemRun);
         runResult.Load();

         listViewItems.Add(new ResultItem(label + " (" + modeUnit + ")", ""));

         foreach (ResultItem input in runResult.Items)
         {
            ResultItem output = runData.GetResult(input, subMode, subModeUnit);
            listViewItems.Add(output);
            Debug.LogToFile("result " + mode + " (" + subMode + ")" + ": " + output.Left + " " + modeUnit + " = " + output.Right);
         }

         if (Config.Result.CalibrationLabelEnabled)
         {
            if (subMode == SubMode.TimeDistance)
            {
               if (lblRunInfo.Text.Contains("Calibration") == false) // do not add this text twice
               {
                  lblRunInfo.Text += Environment.NewLine + GetCalibrationRunInfo(runData);
                  Debug.LogToFile(runData.ToString()); // prints calibration from runAdjust
               }
            }
         }
      }

      string GetCalibrationRunInfo(RunData runData)
      {
         string result;

         string peakA = runData.GetPeakAcceleration().ToString("0.0");
         result = "Peak Acceleration: " + peakA + Environment.NewLine;

         int index = runData.GetCalibrationIndex();
         if (index == 0) result += "Calibration Curve: 0 (no curve found)";
         else            result += "Calibration Curve: " + index;

         return result;
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
               labelLeft.FontSize *= 1.5;
               labelLeft.SetBinding(Label.TextProperty, "Left");

               Label labelRight = new Label();
               labelRight.HorizontalOptions = LayoutOptions.EndAndExpand;
               labelRight.VerticalOptions = LayoutOptions.CenterAndExpand;
               labelRight.FontSize *= 1.5;
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

         listView.ItemSelected += OnListViewSelected;
      }

      string GetRunInfo()
      {
         string result;

         if (IsItemRunEmpty())
         {
            result = Localization.pageResultEmpty;
         }
         else
         {
            result =
               Localization.pageResultInfoRun     + ": " + itemRun.Id                             + Environment.NewLine +
               Localization.pageResultInfoDate    + ": " + Tools.ToLocaleDate(itemRun.Date)       + Environment.NewLine + // 22 May 2017 // OR // 22 Mai 2017
               Localization.pageResultInfoTime    + ": " + itemRun.Date.ToString("HH:mm")         + Environment.NewLine + // 07:00 // 24 hour clock // hour is always 2 digits
               Localization.pageResultInfoVehicle + ": " + database.GetVehicle(itemRun.VehicleId).Model;
         }

         return result;
      }

      string GetShareTextForMode(RunData runData, RunResult runResult, SubMode subMode, string subModeUnit, string modeUnit)
      {
         string result = "";

         foreach (ResultItem input in runResult.Items)
         {
            ResultItem output = runData.GetResult(input, subMode, subModeUnit);
            result += mode + " (" + subMode + ")" + ": " + output.Left + " " + modeUnit + " = " + output.Right + Environment.NewLine;
         }

         return result;
      }

      string GetShareText()
      {
         StringBuilder result = new StringBuilder();

         if (IsItemRunEmpty())
         {
            result.Append(Localization.pageResultEmpty);
         }
         else
         {
            string runResults = "";

            RunData    runData    = new RunData(mode);
            ResultMode resultMode = Tools.ToResultMode(mode);
            RunResult  runResult  = new RunResult(resultMode);

            runData.Load(itemRun);
            runResult.Load();

            runResults += GetShareTextForMode(   runData, runResult, SubMode.TimeSpeed,    Localization.unitSecond,    Settings.GetSpeedUnit());
            if (mode.Equals("Acceleration"))
            {
               runResults += GetShareTextForMode(runData, runResult, SubMode.TimeDistance, Localization.unitSecond,    Settings.GetDistanceUnit());
            }
            runResults += GetShareTextForMode(   runData, runResult, SubMode.Distance,     Settings.GetDistanceUnit(), Settings.GetSpeedUnit());
            runResults += GetShareTextForMode(   runData, runResult, SubMode.Height,       Settings.GetDistanceUnit(), Settings.GetSpeedUnit());

            result.Append("Car Performance - Result").Append(Environment.NewLine);
            result.Append(Environment.NewLine);
            result.Append(Environment.NewLine);
            result.Append(GetRunInfo()).Append(Environment.NewLine);
            result.Append(Environment.NewLine);
            result.Append(runResults).Append(Environment.NewLine);
            result.Append(Environment.NewLine);

            if (Config.Result.ShareDetailsEnabled)
            {
               result.Append(runData.GetEventsCommaSeparated());
               result.Append(Environment.NewLine);
            }
         }

         return result.ToString();
      }

      bool IsItemRunEmpty(DataItemRun item = null)
      {
         bool result = false;

         if (item == null) item = itemRun;

         if ((item.Id <= 0) || (item.VehicleId <= 0))
         {
            result = true;
         }

         return result;
      }
   }
}
