using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Wpf;

namespace GraphUI
{
    /// <summary>
    /// Interaction logic for GraphCollection.xaml
    /// </summary>
    public partial class GraphCollection : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Backing property for CollectionName
        /// </summary>
        private string _collectionName;

        /// <summary>
        /// Collection header in UI
        /// </summary>
        public string CollectionName
        {
            get { return _collectionName; }
            set
            {
                _collectionName = value;
                RaisePropertyChanged("CollectionName");
            }
        }

        /// <summary>
        /// All plots in collection
        /// </summary>
        public List<PlotView> _plots = new List<PlotView>();

        /// <summary>
        /// Hosting control needs to set this for sizing to work properly
        /// because Grid doesn't stretch to take up available space
        /// </summary>
        public static double AvailableHeight { get; set; }

        /// <summary>
        /// Basic class initialization
        /// </summary>
        private GraphCollection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialization with collection header
        /// </summary>
        /// <param name="name">The header</param>
        public GraphCollection(string name) : this()
        {
            CollectionName = name;
        }

        /// <summary>
        /// Adds an empty line graph to the current graph group
        /// </summary>
        /// <param name="title">The graph title</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="showLegend">Boolean indicationg if legend is shown</param>
        /// <param name="position">Enumeration indication position of legend</param>
        /// <returns>void</returns>
        public LineGraph AddLineGraph(string title, string xAxis, string yAxis, bool showLegend = true, LegendPosition position = LegendPosition.TopLeft)
        {
            LineGraph graph = null;

            Dispatcher.Invoke(new Action(() =>
            {
                graph = new LineGraph(title, xAxis, yAxis, showLegend, position);
                AddGraph(graph);
            }));

            return graph;
        }

        /// <summary>
        /// Adds a countour map to the current graph group
        /// </summary>
        /// <param name="title">The graph title</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        /// <param name="xMin">The minimum x value to graph</param>
        /// <param name="xMax">The maximun x value to graph</param>
        /// <param name="yMin">The minimum y value to graph</param>
        /// <param name="yMax">The maximun y value to graph</param>
        /// <param name="levels">The countours to display on the graph</param>
        /// <param name="points">A 2-D array of the z values</param>
        public ContourPlot AddContourPlot(string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[] levels, double[][] points)
        {
            ContourPlot graph = null;
            Dispatcher.Invoke(new Action(() =>
            {
                graph = new ContourPlot(title, xAxis, yAxis, xMin, xMax, yMin, yMax, points, levels);
                AddGraph(graph);
            }));

            return graph;
        }

        /// <summary>
        /// Adds a graph to the UI and adjusts layout as needed
        /// </summary>
        /// <param name="view">The new graph</param>
        private void AddGraph(PlotView view)
        {
            _plots.Add(view);

            XGrid.Children.Clear();
            XGrid.ColumnDefinitions.Clear();
            XGrid.RowDefinitions.Clear();

            // Create appropriate dimension in grid
            while (XGrid.RowDefinitions.Count * XGrid.ColumnDefinitions.Count < _plots.Count)
            {
                XGrid.ColumnDefinitions.Add(new ColumnDefinition());

                if (XGrid.RowDefinitions.Count * XGrid.ColumnDefinitions.Count < _plots.Count)
                {
                    var row = new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) };
                    XGrid.RowDefinitions.Add(row);
                }
            }

            // Add items to grid
            for (var row = 0; row < XGrid.RowDefinitions.Count; row++)
            {
                for (var col = 0; col < XGrid.ColumnDefinitions.Count; col++)
                {
                    // Break if all graphs have been placed
                    if (XGrid.ColumnDefinitions.Count * row + col >= _plots.Count)
                    {
                        break;
                    }

                    var item = _plots[row * XGrid.ColumnDefinitions.Count + col];
                    Grid.SetRow(item, row);
                    Grid.SetColumn(item, col);
                    XGrid.Children.Add(item);
                }
            }

            Resize();
        }

        /// <summary>
        /// Resizes the plots to the available height
        /// </summary>
        public void Resize()
        {
            foreach (var view in XGrid.Children)
            {
                ((PlotView)view).Height = (AvailableHeight - XGroupName.ActualHeight) / XGrid.RowDefinitions.Count;
            }
        }

        #region Events and Handlers

        /// <summary>
        /// Event mechanism to inform bindings of data changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Eventing mechanism for 2-way binding
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates UI as needed after all elements have been loaded
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Resize();
        }

        /// <summary>
        /// Resizes controls as needed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize();
        }

        #endregion Events and Handlers
    }
}
