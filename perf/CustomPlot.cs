using System;
using System.Collections.Generic;
using Xamarin.Forms;
using OxyPlot.Xamarin.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using System.Linq;

namespace perf
{
   public class CustomPlot : PlotView
   {
      readonly OxyColor colorWhite       = OxyColor.FromRgb(190, 190, 190);
      readonly OxyColor colorGray        = OxyColor.FromRgb(111, 111, 111);
      readonly OxyColor colorGreen       = OxyColor.FromRgb(0, 255, 0);
      readonly OxyColor colorTransparent = OxyColor.FromArgb(1, 0, 0, 0);

      public CustomPlot()
      {
         VerticalOptions   = LayoutOptions.FillAndExpand;
         HorizontalOptions = LayoutOptions.Fill;

         Model                     = new PlotModel();
         Model.Background          = colorTransparent;
         Model.PlotAreaBorderColor = colorWhite;
         Model.TitleColor          = colorWhite;
      }

      public void Update(RunData runData, SubMode subMode)
      {
         List<Event> events         = runData.GetEventsAdjusted(subMode);
         LineSeries  lineSeries     = new LineSeries();
         float       speedUnit      = (Settings.IsSpeedUnitKph())      ? 3.6f : 2.23694f;
         float       distanceUnit   = (Settings.IsDistanceUnitMeter()) ? 1.0f : 3.28084f;
         LinearAxis  xAxis          = null;
         LinearAxis  yAxis          = null;
         string      titleTime      = Localization.quantityTime     + " [" + Localization.unitSecond + "]";
         string      titleSpeed     = Localization.quantitySpeed    + " [" + ((Settings.IsSpeedUnitKph())      ? Localization.unitKph   : Localization.unitMph)  + "]";
         string      titleDistance  = Localization.quantityDistance + " [" + ((Settings.IsDistanceUnitMeter()) ? Localization.unitMeter : Localization.unitFeet) + "]";
         string      titleHeight    = Localization.quantityHeight   + " [" + ((Settings.IsDistanceUnitMeter()) ? Localization.unitMeter : Localization.unitFeet) + "]";
         float       xVal           = 0;
         float       xMin           = 0;
         float       xMax           = 0;
         float       yVal           = 0;
         float       yMin           = 0;
         float       yMax           = 0;

         List<DouglasPeucker.Point> fullList  = new List<DouglasPeucker.Point>();
         double                     tolerance = 0;

         switch (subMode)
         {
         case SubMode.TimeSpeed:
            foreach (Event e in events)
            {
               xVal = e.time;
               yVal = e.speed * speedUnit;

               fullList.Add(new DouglasPeucker.Point(xVal, yVal));
               GetMinMax(xVal, yVal, ref xMin, ref xMax, ref yMin, ref yMax);
            }

            tolerance = GetReductionTolerance(xMin, xMax, yMin, yMax);
            BuildReducedSeries(lineSeries, fullList, tolerance);

            xAxis = CreateAxis(AxisPosition.Bottom, titleTime);
            yAxis = CreateAxis(AxisPosition.Left,   titleSpeed);
            break;
            
         case SubMode.TimeDistance:
            foreach (Event e in events)
            {
               xVal = e.time;
               yVal = e.distance * distanceUnit;

               fullList.Add(new DouglasPeucker.Point(xVal, yVal));
               GetMinMax(xVal, yVal, ref xMin, ref xMax, ref yMin, ref yMax);
            }

            tolerance = GetReductionTolerance(xMin, xMax, yMin, yMax);
            BuildReducedSeries(lineSeries, fullList, tolerance);

            xAxis = CreateAxis(AxisPosition.Bottom, titleTime);
            yAxis = CreateAxis(AxisPosition.Left,   titleDistance);
            break;

         case SubMode.Distance:
            foreach (Event e in events)
            {
               xVal = e.distance * distanceUnit;
               yVal = e.speed    * speedUnit;

               fullList.Add(new DouglasPeucker.Point(xVal, yVal));
               GetMinMax(xVal, yVal, ref xMin, ref xMax, ref yMin, ref yMax);
            }

            tolerance = GetReductionTolerance(xMin, xMax, yMin, yMax);
            BuildReducedSeries(lineSeries, fullList, tolerance);

            xAxis = CreateAxis(AxisPosition.Bottom, titleDistance);
            yAxis = CreateAxis(AxisPosition.Left,   titleSpeed);
            break;

         case SubMode.Height:
            foreach (Event e in events)
            {
               xVal = e.time;
               yVal = e.height * distanceUnit;

               fullList.Add(new DouglasPeucker.Point(xVal, yVal));
               GetMinMax(xVal, yVal, ref xMin, ref xMax, ref yMin, ref yMax);
            }

            tolerance = GetReductionTolerance(xMin, xMax, yMin, yMax);
            BuildReducedSeries(lineSeries, fullList, tolerance);

            xAxis = CreateAxis(AxisPosition.Bottom, titleTime);
            yAxis = CreateAxis(AxisPosition.Left,   titleHeight);
            break;
         }

         lineSeries.Color = colorGreen;

         if (Config.Result.PlotSmoothingEnabled)
         {
            // plot smoothing needs at last 3 samples (bezier curve)
            if (lineSeries.Points.Count >= 3)
            {
               lineSeries.Smooth = true;
            }
         }

         Model.Axes.Clear();
         xAxis.Maximum = Get110Percent(xMax);
         xAxis.Minimum = Get110Percent(xMin);
         Model.Axes.Add(xAxis);
         yAxis.Maximum = Get110Percent(yMax);
         yAxis.Minimum = Get110Percent(yMin);
         Model.Axes.Add(yAxis);

         Model.Series.Clear();
         Model.Series.Add(lineSeries);

         Model.InvalidatePlot(true);
      }

      LinearAxis CreateAxis(AxisPosition position, string title)
      {
         LinearAxis axis = new LinearAxis();

         axis.Position           = position;
         axis.TicklineColor      = colorWhite; // numbers
         axis.TextColor          = colorWhite; // numbers
         axis.FontSize           = 20;         // numbers
         axis.MajorGridlineStyle = LineStyle.Automatic;
         axis.MajorGridlineColor = colorGray;
         axis.TitleColor         = colorWhite; // legend
         axis.TitleFontSize      = 20;         // legend
         axis.Title              = title;      // legend
         axis.IsPanEnabled       = false;
         axis.IsZoomEnabled      = false;

         return axis;
      }

      void GetMinMax(float xVal, float yVal, ref float xMin, ref float xMax, ref float yMin, ref float yMax)
      {
         xMin = (xVal < xMin) ? xVal : xMin;
         xMax = (xVal > xMax) ? xVal : xMax;
         yMin = (yVal < yMin) ? yVal : yMin;
         yMax = (yVal > yMax) ? yVal : yMax;
      }

      double GetReductionTolerance(float xMin, float xMax, float yMin, float yMax)
      {
         double result = 0;

         double toleranceX = Math.Abs((xMax - xMin) / 10);
         double toleranceY = Math.Abs((yMax - yMin) / 10);

         result = Math.Min(toleranceX, toleranceY);

         return result;
      }

      void BuildReducedSeries(LineSeries lineSeries, List<DouglasPeucker.Point> fullList, double tolerance)
      {
         if (Config.Result.PlotSmoothingEnabled)
         {
            // reduce the samples (keep only significant) -> smoothing in plot is much better
            IList<DouglasPeucker.Point> reducedList = DouglasPeucker.Reduce(fullList, tolerance);

            foreach (DouglasPeucker.Point p in reducedList)
            {
               lineSeries.Points.Add(new DataPoint(p.X, p.Y));
            }
         }
         else
         {
            // no smoothing in plot -> copy all samples
            foreach (DouglasPeucker.Point p in fullList)
            {
               lineSeries.Points.Add(new DataPoint(p.X, p.Y));
            }
         }
      }

      float Get110Percent(float f)
      {
         return f * 1.1f;
      }
   }
}
