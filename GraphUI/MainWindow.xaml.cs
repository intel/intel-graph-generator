using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using GraphProxy;
using GraphUI.Properties;
using OxyPlot;
using OxyPlot.Wpf;
using LegendPosition = GraphProxy.LegendPosition;
using LineStyle = GraphProxy.LineStyle;

namespace GraphUI
{
    /// <summary>
    /// UI logic
    /// </summary>
    public partial class MainWindow : Window, IService
    {
        #region Data Members

        /// <summary>
        /// Singleton instance of window
        /// </summary>
        public static MainWindow Instance { get; private set; }

        /// <summary>
        /// The web service
        /// </summary>
        private readonly ServiceHost _host;

        /// <summary>
        /// Graph collections available in UI
        /// </summary>
        private readonly List<GraphCollection> _graphCollections = new List<GraphCollection>();

        /// <summary>
        /// Dictionary to correlate guids to graph collections
        /// </summary>
        private readonly Dictionary<Guid, GraphCollection> _graphCollectionDictionary = new Dictionary<Guid, GraphCollection>();

        /// <summary>
        /// Dictionary to correlate guids to plots
        /// </summary>
        private readonly Dictionary<Guid, PlotView> _plotViewDictionary = new Dictionary<Guid, PlotView>();

        private const string ArchiveDirectory = "SavedGraphs";

        #endregion Data Members

        #region Public Methods

        /// <summary>
        /// Initializes UI
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Left = Settings.Default.MainLeft;
            Top = Settings.Default.MainTop;
            Height = Settings.Default.MainHeight;
            Width = Settings.Default.MainWidth;

            XBackButton.IsEnabled = false;
            XForwardButton.IsEnabled = false;
            XSaveButton.IsEnabled = false;

            Instance = this;

            if (string.IsNullOrEmpty(Settings.Default.ArchiveLocation))
            {
                Settings.Default.ArchiveLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), ArchiveDirectory);
            }

            try
            {
                _host = new ServiceHost(typeof(Service));
                _host.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error = " + ex.Message);
                _host.Abort();
            }
        }

        /// <summary>
        /// Creates an empty collection
        /// </summary>
        /// <param name="name">The name for the group</param>
        public Guid AddGraphCollection(string name)
        {
            GraphCollection gc = null;

            // Show new directoryName in UI
            Dispatcher.Invoke(new Action(() =>
            {
                gc = new GraphCollection(name);
                _graphCollections.Add(gc);

                if (Settings.Default.AutoNavigate || XPanel.Children.Count <= 0)
                {
                    NavigateTo(gc);
                }

                UpdateHistory();
            }));

            var guid = Guid.NewGuid();
            _graphCollectionDictionary.Add(guid, gc);
            return guid;
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
            if (!_graphCollectionDictionary.ContainsKey(graphCollection)) return Guid.Empty;

            var guid = Guid.NewGuid();
            _plotViewDictionary.Add(guid, _graphCollectionDictionary[graphCollection].AddLineGraph(title, xAxis, yAxis, showLegend, (OxyPlot.LegendPosition)position)); 
            return guid;
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
            if (!_graphCollectionDictionary.ContainsKey(graphCollection)) return Guid.Empty;

            var guid = Guid.NewGuid();
            _plotViewDictionary.Add(guid, _graphCollectionDictionary[graphCollection].AddContourPlot(title, xAxis, yAxis, xMin, xMax, yMin, yMax, levels, points)); 
            return guid;
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
            if (_plotViewDictionary.ContainsKey(lineGraph))
            {
                ((LineGraph) _plotViewDictionary[lineGraph]).AddSeries(x, y, title);
                return true;
            }
            return false;
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
            return AddSeries(lineGraph, title, x, y, style);
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
            if (_plotViewDictionary.ContainsKey(lineGraph))
            {
                ((LineGraph) _plotViewDictionary[lineGraph]).AddSeries(x, y, title, style);
                return true;
            }
            return false;
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
            if (_plotViewDictionary.ContainsKey(lineGraph))
            {
                ((LineGraph)_plotViewDictionary[lineGraph]).SetAxisBoundaries(xAxisMin, xAxisMax, yAxisMin, yAxisMax);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Navigate the UI to a particular collection
        /// </summary>
        /// <param name="collection">The ID for the collection</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool NavigateTo(Guid collection)
        {
            if (_graphCollectionDictionary.ContainsKey(collection))
            {
                NavigateTo(_graphCollectionDictionary[collection]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the number of pages in the UI
        /// </summary>
        /// <param name="count">The number of pages</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetUiHistory(int count)
        {
            Settings.Default.NumPagesHistory = count;
            Settings.Default.Save();
            return UpdateHistory();
        }

        /// <summary>
        /// Set the size of the file archive
        /// </summary>
        /// <param name="size">The size in MB</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SetArchiveSize(int size)
        {
            if (size == 0)
            {
                Settings.Default.ArchiveEnabled = false;
            }
            else if (size > 0)
            {
                Settings.Default.MaxArchiveSize = size;
                Settings.Default.ArchiveEnabled = true;
            }
            return true;
        }

        /// <summary>
        /// Removes and archives old collections in the user interface
        /// </summary>
        /// <returns>True if successful, False otherwise</returns>
        public bool UpdateHistory()
        {
            var numCollectionsToRemove = 0;

            if (Settings.Default.HistoryEnabled)
            {
                numCollectionsToRemove = Math.Max(0, _graphCollections.Count - Settings.Default.NumPagesHistory);
            }
            else
            {
                numCollectionsToRemove = Math.Max(0, _graphCollections.Count - 1);
            }

            if (Settings.Default.ArchiveEnabled)
            {
                for (var i = 0; i < numCollectionsToRemove; i++)
                {
                    SaveCollection(_graphCollections[i]);
                }
            }

            for (var i = 0; i < numCollectionsToRemove; i++)
            {
                var keysWithMatchingValues = _graphCollectionDictionary.Where(p => p.Value == _graphCollections[i]).Select(p => p.Key);
                _graphCollectionDictionary.Remove(keysWithMatchingValues.ToList()[0]);
            }

            var reversed = _plotViewDictionary.ToDictionary(x => x.Value, x => x.Key);
            for (var i = 0; i < numCollectionsToRemove; i++)
            {
                foreach (var plot in _graphCollections[i]._plots)
                {
                    if (reversed.ContainsKey(plot))
                    {
                        _plotViewDictionary.Remove(reversed[plot]);
                    }
                }
            }

            _graphCollections.RemoveRange(0, numCollectionsToRemove);

            UpdateControlState();

            UpdateArchive();

            return true;
        }

        /// <summary>
        /// Clear the contents of the graph
        /// </summary>
        /// <param name="graph">The graph ID</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool ClearGraph(Guid graph)
        {
            if (_plotViewDictionary.ContainsKey(graph))
            {
                _plotViewDictionary[graph].Model.Series.Clear();
                _plotViewDictionary[graph].Model.InvalidatePlot(true);
            }
            return true;
        }

        #endregion Public Methods

        #region Event Handlers

        /// <summary>
        /// Navigate to previous graph collection
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnBack(object sender, RoutedEventArgs e)
        {
            var index = _graphCollections.IndexOf((GraphCollection)XPanel.Children[0]);
            if (index > 0)
            {
                NavigateTo(_graphCollections[--index]);
            }
            else
            {
                NavigateTo(_graphCollections[0]);
            }
        }

        /// <summary>
        /// Navigate to next graph collection
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnForward(object sender, RoutedEventArgs e)
        {
            var index = _graphCollections.IndexOf((GraphCollection)XPanel.Children[0]);
            NavigateTo(_graphCollections[++index]);
        }

        /// <summary>
        /// Update available size for graphs
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GraphCollection.AvailableHeight = e.NewSize.Height - (XBackButton.ActualHeight + XBackButton.Margin.Top + XBackButton.Margin.Bottom);
            if (XPanel.Children.Count > 0)
            {
                ((GraphCollection)XPanel.Children[0]).Resize();
            }
        }

        /// <summary>
        /// Shows the option dialog
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnOptions(object sender, RoutedEventArgs e)
        {
            new OptionsDialog().Show();
        }

        /// <summary>
        /// Open the file archive in file explorer
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnOpenArchive(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.Default.ArchiveLocation);
        }

        /// <summary>
        /// Save settings and graphs before exiting
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (OptionsDialog.Instance != null)
            {
                OptionsDialog.Instance.Close();
            }

            Settings.Default.MainLeft = Left;
            Settings.Default.MainTop = Top;
            Settings.Default.MainHeight = Height;
            Settings.Default.MainWidth = Width;
            Settings.Default.Save();

            Hide();
            _host.Close();

            if (Settings.Default.ArchiveEnabled)
            {
                foreach (var collection in _graphCollections)
                {
                    SaveCollection(collection);
                }
            }
        }

        /// <summary>
        /// Save current plots
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSaveCollection(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var plot in ((GraphCollection)XPanel.Children[0])._plots)
                {
                    Save(plot, ((GraphCollection)XPanel.Children[0]).CollectionName, dialog.SelectedPath);
                }
            }
        }

        #endregion Event Handlers

        #region Private Methods

        /// <summary>
        /// Saves the plots in the given collection
        /// </summary>
        /// <param name="collection">The collection</param>
        private void SaveCollection(GraphCollection collection)
        {
            var archiveName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff - ") + collection.CollectionName;
            foreach (var plot in collection._plots)
            {
                Save(plot, archiveName, Settings.Default.ArchiveLocation);
            }
        }

        /// <summary>
        /// Update control state as needed
        /// </summary>
        private void UpdateControlState()
        {
            if (XPanel.Children.Count <= 0)
            {
                XBackButton.IsEnabled = false;
                XForwardButton.IsEnabled = false;
                XSaveButton.IsEnabled = false;
                return;
            }

            var gc = (GraphCollection)XPanel.Children[0];

            if (_graphCollections.IndexOf(gc) > 0)
            {
                XBackButton.IsEnabled = true;
            }
            else
            {
                XBackButton.IsEnabled = false;
            }

            if (_graphCollections.IndexOf(gc) < _graphCollections.Count - 1)
            {
                XForwardButton.IsEnabled = true;
            }
            else
            {
                XForwardButton.IsEnabled = false;
            }

            XSaveButton.IsEnabled = true;
        }

        /// <summary>
        /// Keep archive under max size
        /// </summary>
        private static void UpdateArchive()
        {
            while (DirSize(new DirectoryInfo(Settings.Default.ArchiveLocation)) / 1024 / 1024 > Settings.Default.MaxArchiveSize)
            {
                var directories = new DirectoryInfo(Settings.Default.ArchiveLocation).GetDirectories();

                // Keep at least one directory
                if (directories.Length <= 1)
                {
                    break;
                }

                directories.OrderByDescending(file => file.CreationTime).ToList().Last().Delete(true);
            }
        }

        /// <summary>
        /// Navaigate to the given collection
        /// </summary>
        /// <param name="gc">The collection</param>
        private void NavigateTo(GraphCollection gc)
        {
            XPanel.Children.Clear();
            XPanel.Children.Add(gc);
            UpdateControlState();
        }

        /// <summary>
        /// Get the size of a directory
        /// </summary>
        /// <param name="directory">The directory</param>
        /// <returns></returns>
        private static double DirSize(DirectoryInfo directory)
        {
            double size = 0;

            if (directory.Exists)
            {
                foreach (var file in directory.GetFiles())
                {
                    size += file.Length;
                }

                // Recursively add in subdirectories
                size += directory.GetDirectories().Sum(d => DirSize(d));
            }

            return size;
        }

        /// <summary>
        /// Save an individual plot as a PNG
        /// </summary>
        /// <param name="view">The plot</param>
        /// <param name="directoryName">The name of the archive sub-directory</param>
        /// <param name="dir">The archive base directory</param>
        /// <param name="newArchive">Optional argument to delete existing sub-archive</param>
        private void Save(PlotView view, string directoryName, string dir, bool newArchive = false)
        {
            var pngExporter = new PngExporter { Width = Settings.Default.ImageWidth, Height = Settings.Default.ImageHeight, Background = OxyColors.White };
            var archiveDir = Path.Combine(dir, directoryName);
            var archiveFile = Path.Combine(archiveDir, view.Model.Title + ".png");

            if (newArchive)
            {
                if (Directory.Exists(archiveDir))
                {
                    Directory.Delete(archiveDir, true);
                }
                Directory.CreateDirectory(archiveDir);
            }
            else if (!Directory.Exists(archiveDir))
            {
                Directory.CreateDirectory(archiveDir);
            }

            if (File.Exists(archiveFile))
            {
                File.Delete(archiveFile);
            }

            pngExporter.ExportToFile(view.Model, archiveFile);
        }

        #endregion Private Methods
    }
}
