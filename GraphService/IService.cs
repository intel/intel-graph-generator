using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace WpfGraphService
{
    [ServiceContract]
    public interface IService
    {
        /// <summary>
        /// Creates a new group of graphs
        /// </summary>
        /// <param name="name">The name for the group</param>
        /// <returns>An identifier for the figure</returns>
        [OperationContract]
        Guid AddFigure(string name);

        /// <summary>
        /// Adds an empty line graph to the given figure
        /// </summary>
        /// <param name="figure">The figure to add the graph to</param>
        /// <param name="title">The title of the graph</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="showLegend">Boolean to hide the legend</param>
        /// <param name="position">Enum to set the legend postion</param>
        /// <returns></returns>
        [OperationContract]
        Guid AddLineGraph(Guid figure, string title, string xAxis, string yAxis, bool showLegend = true, LegendPosition position = LegendPosition.TopLeft);

        /// <summary>
        /// Adds a series to the given line graph
        /// </summary>
        /// <param name="lineGraph">The line graph to add the series to</param>
        /// <param name="title">An identifier for the series (shown in legend)</param>
        /// <param name="x">The x values</param>
        /// <param name="y">The y values</param>
        [OperationContract]
        bool Plot(Guid lineGraph, string title, List<double> x, List<double> y);

        /// <summary>
        /// Adds a series to the given line graph
        /// </summary>
        /// <param name="lineGraph">The line graph to add the series to</param>
        /// <param name="title">An identifier for the series (shown in legend)</param>
        /// <param name="x">The x values</param>
        /// <param name="y">The y values</param>
        /// <param name="style">A custom style for the series</param>
        [OperationContract]
        bool PlotWithStyle(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style);

        /// <summary>
        /// Sets the view window for the graph
        /// </summary>
        /// <param name="lineGraph">The graph to size</param>
        /// <param name="xAxisMin">The minimum x value</param>
        /// <param name="xAxisMax">The maximum x value</param>
        /// <param name="yAxisMin">The minimum y value</param>
        /// <param name="yAxisMax">The maximum y value</param>
        [OperationContract]
        bool SetAxisBoundaries(Guid lineGraph, double xAxisMin = double.MinValue, double xAxisMax = double.MaxValue, double yAxisMin = double.MinValue, double yAxisMax = double.MaxValue);

        /// <summary>
        /// Adds a contour plot to the given figure
        /// </summary>
        /// <param name="figure">The figure to add the plot to</param>
        /// <param name="title">The title of the contour plot</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="xMin">The minimum x value</param>
        /// <param name="xMax">The maximum x value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="levels">The values for the contour lines</param>
        /// <param name="points">Two dimensional array of values to plot</param>
        [OperationContract]
        Guid AddContourPlot(Guid figure, string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points);

        /// <summary>
        /// Clears the contents in an individual graph
        /// </summary>
        /// <param name="graph">The graph to clear</param>
        [OperationContract]
        bool ClearGraph(Guid graph);

        /// <summary>
        /// Directs the UI to navigate to a particular figure
        /// </summary>
        /// <param name="figure">The figure to navigate to</param>
        [OperationContract]
        bool NavigateTo(Guid figure);

        /// <summary>
        /// Set the number of pages stored in the UI
        /// </summary>
        /// <param name="count">The number of pages</param>
        [OperationContract]
        bool SetUiHistory(int count);

        /// <summary>
        /// Sets the maximum size for the archive, a value of 0 disables archiving
        /// </summary>
        /// <param name="size">The sive in MB</param>
        [OperationContract]
        bool SetArchiveSize(int size);
    }
}
