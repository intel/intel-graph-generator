using System;
using System.Collections.Generic;
using WpfGraphService;

namespace GraphUI
{
    public class Service : IService
    {
        public Guid AddFigure(string name)
        {
            return MainWindow.Instance.AddFigure(name);
        }

        public Guid AddLineGraph(Guid figure, string title, string xAxis, string yAxis, bool showLegend = true, LegendPosition position = LegendPosition.TopLeft)
        {
            return MainWindow.Instance.AddLineGraph(figure, title, xAxis, yAxis, showLegend, position);
        }

        public bool Plot(Guid lineGraph, string title, List<double> x, List<double> y)
        {
            return MainWindow.Instance.Plot(lineGraph, title, x, y);
        }

        public bool PlotWithStyle(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style)
        {
            return MainWindow.Instance.AddSeries(lineGraph, title, x, y, style);
        }

        public bool SetAxisBoundaries(Guid lineGraph, double xAxisMin = double.MinValue, double xAxisMax = double.MaxValue, double yAxisMin = double.MinValue, double yAxisMax = double.MaxValue)
        {
            return MainWindow.Instance.SetAxisBoundaries(lineGraph, xAxisMin, xAxisMax, yAxisMin, yAxisMax);
        }

        public Guid AddContourPlot(Guid figure, string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points)
        {
            return MainWindow.Instance.AddContourPlot(figure, title, xAxis, yAxis, xMin, xMax, yMin, yMax, levels, points);
        }

        public bool ClearGraph(Guid graph)
        {
            return MainWindow.Instance.ClearGraph(graph);
        }

        public bool NavigateTo(Guid figure)
        {
            return MainWindow.Instance.NavigateTo(figure);
        }

        public bool SetUiHistory(int count)
        {
            return MainWindow.Instance.SetUiHistory(count);
        }

        public bool SetArchiveSize(int size)
        {
            return MainWindow.Instance.SetArchiveSize(size);
        }
    }
}
