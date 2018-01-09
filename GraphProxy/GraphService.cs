using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace GraphProxy
{
    public class GraphService : ClientBase<IService>, IService
    {
        /// <summary>
        /// Close the service
        /// </summary>
        public new void Close()
        {
            try
            {
                base.Close();
            }
            catch (Exception) {}
        }

        /// <summary>
        /// Adds an empty graph collection to the UI
        /// </summary>
        /// <param name="name">The header for the collection</param>
        /// <returns>A valid GUID if successful otherwise zeros</returns>
        public Guid AddGraphCollection(string name)
        {
            try
            {
                return Channel.AddGraphCollection(name);
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Adds an empty line graph to the given collection
        /// </summary>
        /// <param name="graphCollection">The ID of the collection</param>
        /// <param name="title">The header for the graph</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="showLegend">Switch to disable legend</param>
        /// <param name="position">Enum for legend position</param>
        /// <returns>A valid GUID if successful otherwise zeros</returns>
        public Guid AddLineGraph(Guid graphCollection, string title, string xAxis, string yAxis, bool showLegend = true, LegendPosition position = LegendPosition.TopLeft)
        {
            try
            {
                return Channel.AddLineGraph(graphCollection, title, xAxis, yAxis, showLegend, position);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Adds a line series to the given line graph
        /// </summary>
        /// <param name="lineGraph">The ID of the line graph</param>
        /// <param name="title">The series name</param>
        /// <param name="x">The x coords</param>
        /// <param name="y">The y coords</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool AddSeries(Guid lineGraph, string title, List<double> x, List<double> y)
        {
            try
            {
                return Channel.AddSeries(lineGraph, title, x, y);  
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a line series to the given line graph
        /// </summary>
        /// <param name="lineGraph">The ID of the line graph</param>
        /// <param name="title">The series name</param>
        /// <param name="x">The x coords</param>
        /// <param name="y">The y coords</param>
        /// <param name="style">A custom style for the series</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool AddSeries(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style)
        {
            return AddSeriesWithStyle(lineGraph, title, x, y, style);
        }

        /// <summary>
        /// Adds a line series to the given line graph
        /// </summary>
        /// <param name="lineGraph">The ID of the line graph</param>
        /// <param name="title">The series name</param>
        /// <param name="x">The x coords</param>
        /// <param name="y">The y coords</param>
        /// <param name="style">A custom style for the series</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool AddSeriesWithStyle(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style)
        {
            try
            {
                return Channel.AddSeriesWithStyle(lineGraph, title, x, y, style);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the view window for the graph
        /// </summary>
        /// <param name="lineGraph">The graph ID</param>
        /// <param name="xAxisMin">The minimum x value</param>
        /// <param name="xAxisMax">The maximum x value</param>
        /// <param name="yAxisMin">The minimum y value</param>
        /// <param name="yAxisMax">The maximum y value</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetAxisBoundaries(Guid lineGraph, double xAxisMin = double.MinValue, double xAxisMax = double.MaxValue, double yAxisMin = double.MinValue, double yAxisMax = double.MaxValue)
        {
            try
            {
                return Channel.SetAxisBoundaries(lineGraph, xAxisMin, xAxisMax, yAxisMin, yAxisMax);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a contour plot to the given graph collection
        /// </summary>
        /// <param name="graphCollection">The graph collection ID</param>
        /// <param name="title">The header for the graph collection</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="xMin">The minimum x value</param>
        /// <param name="xMax">The maximum x value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="levels">The contour levels</param>
        /// <param name="points">2D array of plot values</param>
        /// <returns>A valid GUID if successful otherwise zeros</returns>
        public Guid AddContourPlot(Guid graphCollection, string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points)
        {
            try
            {
                return Channel.AddContourPlot(graphCollection, title, xAxis, yAxis, xMin, xMax, yMin, yMax, levels, points);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Clear the contents of the graph
        /// </summary>
        /// <param name="graph">The graph ID</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool ClearGraph(Guid graph)
        {
            try
            {
                return Channel.ClearGraph(graph);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Navigate the UI to a particular collection
        /// </summary>
        /// <param name="collection">The ID for the collection</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool NavigateTo(Guid collection)
        {
            try
            {
                return Channel.NavigateTo(collection);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Set the number of pages in the UI
        /// </summary>
        /// <param name="count">The number of pages</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetUiHistory(int count)
        {
            try
            {
                return Channel.SetUiHistory(count);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Set the size of the file archive
        /// </summary>
        /// <param name="size">The size in MB</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetArchiveSize(int size)
        {
            try
            {
                return Channel.SetArchiveSize(size);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
