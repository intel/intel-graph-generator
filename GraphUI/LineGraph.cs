using System;
using System.Collections.Generic;
using WpfGraphService;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using LineSeries = OxyPlot.Series.LineSeries;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LegendPosition = OxyPlot.LegendPosition;
using LineStyle = OxyPlot.LineStyle;
using MarkerType = OxyPlot.MarkerType;

namespace GraphUI
{
    public class LineGraph : PlotView
    {
        private LineGraph(){}

        /// <summary>
        /// Creates an empty line graph
        /// </summary>
        /// <param name="title">The graph title</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="showLegend">Switch to show legend</param>
        /// <param name="position">Enum to position legend</param>
        public LineGraph(string title, string xAxis, string yAxis, bool showLegend, LegendPosition position) : this()
        {
            if (showLegend)
            {
                Model = new PlotModel
                {
                    Title = title,
                    LegendTitle = "Legend",
                    LegendPosition = position,
                    LegendOrientation = LegendOrientation.Vertical,
                    LegendPlacement = LegendPlacement.Inside,
                    LegendBackground = OxyColors.Transparent,
                    LegendBorder = OxyColors.Transparent,
                };
            }
            else
            {
                Model = new PlotModel
                {
                    Title = title,
                    IsLegendVisible = false
                };
            }
            Model.Axes.Add(new LinearAxis { Title = xAxis, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot,  Position = AxisPosition.Bottom});
            Model.Axes.Add(new LinearAxis { Title = yAxis, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot});
        }

        /// <summary>
        /// Sets the initial viewing area on the graph
        /// </summary>
        /// <param name="xAxisMinimum">Initial x minimum</param>
        /// <param name="xAxisMaximum">Initial x maximum</param>
        /// <param name="yAxisMinValue">Initial y minimum</param>
        /// <param name="yAxisMaxValue">Initial y maximum</param>
        public void SetAxisBoundaries(double xAxisMinimum = double.MinValue, double xAxisMaximum = double.MaxValue, double yAxisMinValue = double.MinValue, double yAxisMaxValue = double.MaxValue)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Model.Axes[0].Minimum = xAxisMinimum;
                Model.Axes[0].Maximum = xAxisMaximum;
                Model.Axes[1].Minimum = yAxisMinValue;
                Model.Axes[1].Maximum = yAxisMaxValue;
                Model.InvalidatePlot(true);
            }));
        }

        /// <summary>
        /// Adds a line series to the plot
        /// </summary>
        /// <param name="x">The x values</param>
        /// <param name="y">The y values</param>
        /// <param name="title">The name in the legend</param>
        public void AddSeries(List<double> x, List<double> y, string title)
        {
            var series = new LineSeries
            {
                Title = title,
                CanTrackerInterpolatePoints = false, // snap to plot values
                Smooth = false, // show raw data
                StrokeThickness = 5,
            };

            for (var i = 0; i < x.Count; i++)
            {
                series.Points.Add(new DataPoint(x[i], y[i]));
            }

            Dispatcher.Invoke(new Action(() =>
            {
                Model.Series.Add(series);
                Model.InvalidatePlot(true);
            }));
        }

        /// <summary>
        /// Adds a line series to the plot
        /// </summary>
        /// <param name="title">The name in the legend</param>
        /// <param name="x">The x values</param>
        /// <param name="y">The y values</param>
        /// <param name="style">The line style</param>
        public void AddSeries(List<double> x, List<double> y, string title, WpfGraphService.LineStyle style)
        {
            double[] dashes = null;

            switch (style.Dashedness)
            {
                case LineDashedness.Solid:
                    break;
                case LineDashedness.Dashed:
                    dashes = new double[] {4};
                    break;
                case LineDashedness.Dotted:
                    dashes = new double[] { 1 };
                    break;
                case LineDashedness.DashDot:
                    dashes = new double[] { 4, 2, 1, 2};
                    break;
            }

            var series = new LineSeries
            {
                CanTrackerInterpolatePoints = false, // use raw values only
                Title = title,
                StrokeThickness = style.Thickness,
                Smooth = style.Smooth,
                Color = OxyColor.FromArgb(style.LineColor.A, style.LineColor.R, style.LineColor.G, style.LineColor.B),
                Dashes = dashes,
                MarkerType = (MarkerType)style.MarkerType,
                MarkerSize = style.MarkerSize,
                MarkerFill = OxyColor.FromArgb(style.MarkerFill.A, style.MarkerFill.R, style.MarkerFill.G, style.MarkerFill.B),
                MarkerStroke = OxyColor.FromArgb(style.MarkerStroke.A, style.MarkerStroke.R, style.MarkerStroke.G, style.MarkerStroke.B),
                MarkerStrokeThickness = style.MarkerStrokeThickness,
            };

            for (var i = 0; i < x.Count; i++)
            {
                series.Points.Add(new DataPoint(x[i], y[i]));
            }

            Dispatcher.Invoke(new Action(() =>
            {
                Model.Series.Add(series);
                Model.InvalidatePlot(true);
            }));
        }
    }
}
