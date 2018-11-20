using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.ServiceModel;
using WpfGraphService;
using GraphUI.Properties;
using OxyPlot;
using OxyPlot.Wpf;
using LegendPosition = WpfGraphService.LegendPosition;
using LineStyle = WpfGraphService.LineStyle;

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
        /// Graph figures available in UI
        /// </summary>
        private readonly List<Figure> _figures = new List<Figure>();

        /// <summary>
        /// Dictionary to correlate guids to graph figures
        /// </summary>
        private readonly Dictionary<Guid, Figure> _figureDictionary = new Dictionary<Guid, Figure>();

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
        /// Creates an empty figure
        /// </summary>
        /// <param name="name">The name for the group</param>
        public Guid AddFigure(string name)
        {
            Figure figure = null;

            // Show new directoryName in UI
            Dispatcher.Invoke(new Action(() =>
            {
                figure = new Figure(name);
                _figures.Add(figure);

                if (Settings.Default.AutoNavigate || XPanel.Children.Count <= 0)
                {
                    NavigateTo(figure);
                }

                UpdateHistory();
            }));

            var guid = Guid.NewGuid();
            _figureDictionary.Add(guid, figure);
            return guid;
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
            if (!_figureDictionary.ContainsKey(figure)) return Guid.Empty;

            var guid = Guid.NewGuid();
            _plotViewDictionary.Add(guid, _figureDictionary[figure].AddLineGraph(title, xAxis, yAxis, showLegend, (OxyPlot.LegendPosition)position)); 
            return guid;
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
        public Guid AddContourPlot(Guid figure, string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points)
        {
            if (!_figureDictionary.ContainsKey(figure)) return Guid.Empty;

            var guid = Guid.NewGuid();
            _plotViewDictionary.Add(guid, _figureDictionary[figure].AddContourPlot(title, xAxis, yAxis, xMin, xMax, yMin, yMax, levels, points)); 
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
        public bool Plot(Guid lineGraph, string title, List<double> x, List<double> y)
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
        public bool PlotWithStyle(Guid lineGraph, string title, List<double> x, List<double> y, LineStyle style)
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
        /// Navigate the UI to a particular figure
        /// </summary>
        /// <param name="figure">The ID for the figure</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool NavigateTo(Guid figure)
        {
            if (_figureDictionary.ContainsKey(figure))
            {
                NavigateTo(_figureDictionary[figure]);
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
        /// Removes and archives old figures in the user interface
        /// </summary>
        /// <returns>True if successful, False otherwise</returns>
        public bool UpdateHistory()
        {
            var numFiguresToRemove = 0;

            if (Settings.Default.HistoryEnabled)
            {
                numFiguresToRemove = Math.Max(0, _figures.Count - Settings.Default.NumPagesHistory);
            }
            else
            {
                numFiguresToRemove = Math.Max(0, _figures.Count - 1);
            }

            if (Settings.Default.ArchiveEnabled)
            {
                for (var i = 0; i < numFiguresToRemove; i++)
                {
                    SaveFigure(_figures[i]);
                }
            }

            for (var i = 0; i < numFiguresToRemove; i++)
            {
                var keysWithMatchingValues = _figureDictionary.Where(p => p.Value == _figures[i]).Select(p => p.Key);
                _figureDictionary.Remove(keysWithMatchingValues.ToList()[0]);
            }

            var reversed = _plotViewDictionary.ToDictionary(x => x.Value, x => x.Key);
            for (var i = 0; i < numFiguresToRemove; i++)
            {
                foreach (var plot in _figures[i]._plots)
                {
                    if (reversed.ContainsKey(plot))
                    {
                        _plotViewDictionary.Remove(reversed[plot]);
                    }
                }
            }

            _figures.RemoveRange(0, numFiguresToRemove);

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
        /// Navigate to previous figure
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnBack(object sender, RoutedEventArgs e)
        {
            var index = _figures.IndexOf((Figure)XPanel.Children[0]);
            if (index > 0)
            {
                NavigateTo(_figures[--index]);
            }
            else
            {
                NavigateTo(_figures[0]);
            }
        }

        /// <summary>
        /// Navigate to next figure
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnForward(object sender, RoutedEventArgs e)
        {
            var index = _figures.IndexOf((Figure)XPanel.Children[0]);
            NavigateTo(_figures[++index]);
        }

        /// <summary>
        /// Update available size for graphs
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Figure.AvailableHeight = e.NewSize.Height - (XBackButton.ActualHeight + XBackButton.Margin.Top + XBackButton.Margin.Bottom);
            if (XPanel.Children.Count > 0)
            {
                ((Figure)XPanel.Children[0]).Resize();
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
                foreach (var figure in _figures)
                {
                    SaveFigure(figure);
                }
            }
        }

        /// <summary>
        /// Save current plots
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSaveFigure(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var plot in ((Figure)XPanel.Children[0])._plots)
                {
                    Save(plot, ((Figure)XPanel.Children[0]).FigureName, dialog.SelectedPath);
                }
            }
        }

        #endregion Event Handlers

        #region Private Methods

        /// <summary>
        /// Saves the plots in the given figure
        /// </summary>
        /// <param name="figure">The figure</param>
        private void SaveFigure(Figure figure)
        {
            var archiveName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss.fff - ") + figure.FigureName;
            foreach (var plot in figure._plots)
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

            var figure = (Figure)XPanel.Children[0];

            if (_figures.IndexOf(figure) > 0)
            {
                XBackButton.IsEnabled = true;
            }
            else
            {
                XBackButton.IsEnabled = false;
            }

            if (_figures.IndexOf(figure) < _figures.Count - 1)
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
        /// Navigate to the given figure
        /// </summary>
        /// <param name="figure">The figure</param>
        private void NavigateTo(Figure figure)
        {
            XPanel.Children.Clear();
            XPanel.Children.Add(figure);
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
        /// <param name="baseDir">The archive base directory</param>
        /// <param name="newArchive">Optional argument to delete existing sub-archive</param>
        private void Save(PlotView view, string directoryName, string baseDir, bool newArchive = false)
        {
            var pngExporter = new PngExporter { Width = Settings.Default.ImageWidth, Height = Settings.Default.ImageHeight, Background = OxyColors.White };
            var archiveDir = Path.Combine(baseDir, CreateSafeDirectoryName(directoryName));
            var archiveFile = Path.Combine(archiveDir, CreateSafeFileName(view.Model.Title) + ".png");

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

        public static string CreateSafeDirectoryName(string str)
        {
            return str.Replace(".", "")
                      .Replace("/", "")
                      .Replace("\"", "")
                      .Replace("*", "")
                      .Replace(":", "")
                      .Replace("?", "")
                      .Replace("<", "")
                      .Replace(">", "")
                      .Replace("|", "");
        }

        public static string CreateSafeFileName(string str)
        {
            return str.Replace("/", "")
                      .Replace("\"", "")
                      .Replace("*", "")
                      .Replace(":", "")
                      .Replace("?", "")
                      .Replace("<", "")
                      .Replace(">", "")
                      .Replace("|", "")
                      .Replace(";", "");
        }

        #endregion Private Methods
    }
}
