using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;

namespace WpfGraphService
{
    public class GraphService
    {
        private ServiceClient service;

        public GraphService()
        {
            try
            {
                service = new ServiceClient();
            }
            catch (Exception ex)
            {
                service = new ServiceClient(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8732/WpfGraphService/"));
            }
        }

        /// <summary>
        /// Adds an empty figure to the UI
        /// </summary>
        /// <param name="name">The header for the figure</param>
        /// <returns>A valid GUID if successful otherwise zeros</returns>
        public Guid AddFigure(string name)
        {
            try
            {
                return service.AddFigure(name);
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Adds an empty line graph to the given figure
        /// </summary>
        /// <param name="figure">The ID of the figure</param>
        /// <param name="title">The header for the graph</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="showLegend">Switch to disable legend</param>
        /// <param name="position">Enum for legend position</param>
        /// <returns>A valid GUID if successful otherwise zeros</returns>
        public Guid AddLineGraph(Guid figure, string title, string xAxis, string yAxis, bool showLegend = true, LegendPosition position = LegendPosition.TopLeft)
        {
            try
            {
                return service.AddLineGraph(figure, title, xAxis, yAxis, showLegend, position);
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
        public bool Plot(Guid lineGraph, string title, List<double> x, List<double> y)
        {
            return service.Plot(lineGraph, title, x, y);
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
        public bool Plot(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style)
        {
            return service.PlotWithStyle(lineGraph, title, x, y, style);
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
            return service.SetAxisBoundaries(lineGraph, xAxisMin, xAxisMax, yAxisMin, yAxisMax);
        }

        /// <summary>
        /// Adds a contour plot to the given figure
        /// </summary>
        /// <param name="figure">The figure ID</param>
        /// <param name="title">The header for the figure</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="xMin">The minimum x value</param>
        /// <param name="xMax">The maximum x value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="levels">The contour levels</param>
        /// <param name="points">2D array of plot values</param>
        /// <returns>A valid GUID if successful otherwise zeros</returns>
        public Guid Plot(Guid figure, string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points)
        {
            return service.AddContourPlot(figure, title, xAxis, yAxis, xMin, xMax, yMin, yMax, levels, points);
        }

        /// <summary>
        /// Clear the contents of the graph
        /// </summary>
        /// <param name="graph">The graph ID</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool ClearGraph(Guid graph)
        {
            return service.ClearGraph(graph);
        }

        /// <summary>
        /// Navigate the UI to a particular figure
        /// </summary>
        /// <param name="figure">The ID for the figure</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool NavigateTo(Guid figure)
        {
            return service.NavigateTo(figure);
        }

        /// <summary>
        /// Set the number of pages in the UI
        /// </summary>
        /// <param name="count">The number of pages</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetUiHistory(int count)
        {
            return service.SetUiHistory(count);
        }

        /// <summary>
        /// Set the size of the file archive
        /// </summary>
        /// <param name="size">The size in MB</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetArchiveSize(int size)
        {
            return service.SetArchiveSize(size);
        }

        /// <summary>
        /// Close the service
        /// </summary>
        public void Close()
        {
            service.Close();
            service = null;
        }

        private class ServiceClient : ClientBase<IService>
        {
            public ServiceClient() :
                base()
            {
            }

            public ServiceClient(NetTcpBinding binding, EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
            {
            }

            public new void Close()
            {
                try
                {
                    base.Close();
                }
                catch (Exception) { }
            }

            public Guid AddFigure(string name)
            {
                try
                {
                    return Channel.AddFigure(name);
                }
                catch (Exception ex)
                {
                    return Guid.Empty;
                }
            }

            public Guid AddLineGraph(Guid figure, string title, string xAxis, string yAxis, bool showLegend, LegendPosition position)
            {
                try
                {
                    return Channel.AddLineGraph(figure, title, xAxis, yAxis, showLegend, position);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }

            public bool Plot(Guid lineGraph, string title, List<double> x, List<double> y)
            {
                try
                {
                    return Channel.Plot(lineGraph, title, x, y);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public bool PlotWithStyle(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style)
            {
                try
                {
                    return Channel.PlotWithStyle(lineGraph, title, x, y, style);
                }
                catch (Exception)
                {
                    return false;
                }
            }

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

            public Guid AddContourPlot(Guid figure, string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points)
            {
                try
                {
                    return Channel.AddContourPlot(figure, title, xAxis, yAxis, xMin, xMax, yMin, yMax, levels, points);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }

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

            public bool NavigateTo(Guid figure)
            {
                try
                {
                    return Channel.NavigateTo(figure);
                }
                catch (Exception)
                {
                    return false;
                }
            }

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
}
