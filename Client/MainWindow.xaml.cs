using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GraphProxy;
using System.Drawing;
using System.Threading;
using Client.Properties;

namespace Client
{
    /// <summary>
    /// Sample application showing how to use the graph service
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Sample data for contour map

        private const double Xmin = -Math.PI;
        private const double Xmax = Math.PI;
        private const double Ymin = -Math.PI;
        private const double Ymax = Math.PI;
        private readonly double[] _levels = { 0.25, .5, .75, 1, 1.25, 1.5, 1.75 };

        #endregion Sample data for contour map

        #region Sample data for lines

        private static readonly List<double> XSinc = new List<double>{0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5, 10, 10.5, 11, 11.5, 12, 12.5, 13, 13.5, 14, 14.5, 15, 15.5, 16, 16.5, 17, 17.5, 18, 18.5, 19, 19.5, 20};
        private static readonly List<double> YSinc = new List<double>{0, 0.239712769, 0.841470985, 1.49624248, 1.818594854, 1.49618036, 0.423360024, -1.227741297, -3.027209981, -4.398885529, -4.794621373, -3.880471791, -1.676492989, 1.398279923, 4.598906191, 7.034999826, 7.914865973, 6.787140457, 3.709066367, -0.713935644, -5.440211109, -9.23680548, -10.99989227, -10.06770001, -6.438875016, -0.829023717, 5.462171479, 10.85108976, 13.86850298, 13.55597831, 9.754317602, 3.20024597, -4.606453067, -11.74445815, -16.34375736, -17.0734551, -13.51777044, -6.335891442, 2.847666984, 11.80802746, 18.25890501};

        private static readonly List<double> XSpiral1 = new List<double> {0.540302306, -0.832293673, -2.96997749, -2.614574483, 1.418310927, 5.76102172, 5.27731578, -1.16400027, -8.200172357, -8.390715291, 0.048682678, 10.1262475, 11.79680816, 1.914321055, -11.39531869, -15.32255169, -4.677776747, 11.88570075, 18.78538775, 8.161641236, -11.50231446, -21.99913818, -12.25515947, 10.18029618, 24.7800703, 16.81990238, -7.887747836, -26.95296426, -21.69366836, 4.627543497, 28.35701309, 26.69514754, -0.438132658, -28.85138934, -31.62922718, -4.606692827, 28.32031992, 36.29279847, 10.39907436, -26.67752247, -40.48091038};
        private static readonly List<double> YSpiral1 = new List<double> {0.841470985, 1.818594854, 0.423360024, -3.027209981, -4.794621373, -1.676492989, 4.598906191, 7.914865973, 3.709066367, -5.440211109, -10.99989227, -6.438875016, 5.462171479, 13.86850298, 9.754317602, -4.606453067, -16.34375736, -13.51777044, 2.847666984, 18.25890501, 17.56976841, -0.194728804, -19.4630693, -21.73388069, -3.308793752, 19.82651971, 25.82215007, 7.585362073, -19.24538264, -29.64094872, -12.52516701, 17.6456538, 32.99709138, 17.98881133, -14.98639343, -35.70403872, -23.81091093, 11.26200599, 37.58802007, 29.80452642, -6.503529421};

        // Goes the other way
        //private static readonly List<double> XSpiral1 = new List<double> {-0.653643621, -0.316193699, 0.567324371, 1.771674436, 2.88051086, 3.41805669, 3.015609017, 1.55985893, -0.727500169, -3.311065465, -5.466781571, -6.481619015, -5.873500704, -3.56652696, 0.035405584, 4.108090449, 7.594685629, 9.479083652, 9.074467815, 6.246666965, 1.5041094, -4.081629068, -9.116254954, -12.23066829, -12.44957324, -9.482360276, -3.852286733, 3.181879467, 9.904750624, 14.56263585, 15.81927389, 13.130947, 6.937395051, -1.392362427, -9.859126684, -16.31314696, -18.9992557, -17.02944048, -10.65666041, -1.269058527, 8.907759154};
        //private static readonly List<double> YSpiral1 = new List<double> {-0.756802495, -1.466295176, -1.917848549, -1.763850814, -0.838246495, 0.752919958, 2.627946395, 4.220999895, 4.946791233, 4.391679119, 2.472710911, -0.488482283, -3.808147776, -6.5977182, -7.999921652, -7.441343485, -4.829156262, -0.630058025, 4.201670368, 8.439736479, 10.89668091, 10.75129314, 7.803454082, 2.580843524, -3.742743117, -9.609102122, -13.45956489, -14.14657708, -11.2648087, -5.308449586, 2.398035355, 9.99140785, 15.52006926, 17.4445214, 15.05980149, 8.725321557, -0.168174877, -9.499902993, -16.92440808, -20.46068157, -19.0171456};

        private static readonly List<double> XSpiral2 = new List<double> {0.540302306, 0.106105803, -0.832293673, -2.002859039, -2.96997749, -3.277598406, -2.614574483, -0.948581097, 1.418310927, 3.897683759, 5.76102172, 6.347819567, 5.27731578, 2.599764884, -1.16400027, -5.117101173, -8.200172357, -9.473135484, -8.390715291, -4.993137744, 0.048682678, 5.558004726, 10.1262475, 12.47247849, 11.79680816, 8.031428955, 1.914321055, -5.146401868, -11.39531869, -15.16602867, -15.32255169, -11.58955145, -4.677776747, 3.840199356, 11.88570075, 17.38121053, 18.78538775, 15.51839191, 8.161641236, -1.631053129, -11.50231446};
        private static readonly List<double> YSpiral2 = new List<double> { 0.841470985, 1.49624248, 1.818594854, 1.49618036, 0.423360024, -1.227741297, -3.027209981, -4.398885529, -4.794621373, -3.880471791, -1.676492989, 1.398279923, 4.598906191, 7.034999826, 7.914865973, 6.787140457, 3.709066367, -0.713935644, -5.440211109, -9.23680548, -10.99989227, -10.06770001, -6.438875016, -0.829023717, 5.462171479, 10.85108976, 13.86850298, 13.55597831, 9.754317602, 3.20024597, -4.606453067, -11.74445815, -16.34375736, -17.0734551, -13.51777044, -6.335891442, 2.847666984, 11.80802746, 18.25890501, 20.43501078, 17.56976841 };

        #endregion Sample data for lines

        /// <summary>
        /// The graph service
        /// </summary>
        private readonly GraphService _graphService;

        /// <summary>
        /// Initializes class, graph service and state
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Left = Settings.Default.Left;
            Top = Settings.Default.Top;
            _graphService = new GraphService();
        }

        /// <summary>
        /// Creates a variety of graphs to show capabilities
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event Args</param>
        private void OnCreateGraphsMethod1(object sender, RoutedEventArgs e)
        {
            for (var i = 1; i <= 4; i++) 
            {
                // Create an empty graph collection
                var graphCollection = _graphService.AddGraphCollection(string.Format("Graph Collection {0}", i));

                // Multi-series using default line styles
                var lineGraph = _graphService.AddLineGraph(graphCollection, "Graph 1", "Current (mA)", "Voltage (V)");
                _graphService.AddSeries(lineGraph, "y1", XSinc, YSinc.Select(y => y +  0).ToList());
                _graphService.AddSeries(lineGraph, "y2", XSinc, YSinc.Select(y => y +  4).ToList());
                _graphService.AddSeries(lineGraph, "y3", XSinc, YSinc.Select(y => y +  8).ToList());
                _graphService.AddSeries(lineGraph, "y4", XSinc, YSinc.Select(y => y + 12).ToList());

                // Multi-series using custom line styles
                lineGraph = _graphService.AddLineGraph(graphCollection, "Graph 2", "Current (mA)", "Voltage (V)");
                _graphService.AddSeries(lineGraph, "y1", XSinc, YSinc.Select(y => y +  0).ToList(), Styles.RedLine);
                _graphService.AddSeries(lineGraph, "y2", XSinc, YSinc.Select(y => y +  4).ToList(), Styles.BlueLine);
                _graphService.AddSeries(lineGraph, "y3", XSinc, YSinc.Select(y => y +  8).ToList(), Styles.GreenLine);
                _graphService.AddSeries(lineGraph, "y4", XSinc, YSinc.Select(y => y + 12).ToList(), Styles.SlateBlueLine);

                // Single series with different lines styles, markers and axis boundaries
                lineGraph = _graphService.AddLineGraph(graphCollection, "Graph 3", "time", "space", false);
                _graphService.AddSeries(lineGraph, "Smooth", XSpiral1, YSpiral1, Styles.RedDashDot);
                _graphService.AddSeries(lineGraph, "Not Smooth", XSpiral1, YSpiral1, Styles.BlueWithMarkers);
                _graphService.SetAxisBoundaries(lineGraph, XSpiral1.Min() - .1 * (XSpiral1.Max() - XSpiral1.Min()), XSpiral1.Max() + .1 * (XSpiral1.Max() - XSpiral1.Min()), YSpiral1.Min() - .1 * (YSpiral1.Max() - YSpiral1.Min()), YSpiral1.Max() + .1 * (YSpiral1.Max() - YSpiral1.Min()));

                // Multi-series with markers only
                lineGraph = _graphService.AddLineGraph(graphCollection, "Graph 4", "time", "space", false);
                _graphService.AddSeries(lineGraph, "Series 1", XSpiral1, YSpiral1, Styles.BlueStarMarkers);
                _graphService.AddSeries(lineGraph, "Series 2", XSpiral2, YSpiral2, Styles.RedCrossMarkers);
                _graphService.SetAxisBoundaries(lineGraph, XSpiral1.Min() - .1 * (XSpiral1.Max() - XSpiral1.Min()), XSpiral1.Max() + .1 * (XSpiral1.Max() - XSpiral1.Min()), YSpiral1.Min() - .1 * (YSpiral1.Max() - YSpiral1.Min()), YSpiral1.Max() + .1 * (YSpiral1.Max() - YSpiral1.Min()));

                // Multi-series with dynamically changing style
                lineGraph = _graphService.AddLineGraph(graphCollection, "Graph 5", "time", "space", false);
                var xMarkerCoords = new List<double>();
                var yMarkerCoords = new List<double>();
                for (double j = 1; j < XSpiral1.Count; j++)
                {
                    Styles.TransparentLine.LineColor = Color.FromArgb(0xA0, 
                        (int)(0xff * (Math.Sin((j + 0) / 1.5) + 1) / 2), 
                        (int)(0xff * (Math.Sin((j + 1) / 2.0) + 1) / 2), 
                        (int)(0xff * (Math.Sin((j + 2) / 2.5) + 1) / 2));
                    _graphService.AddSeries(lineGraph, null, new List<double> { 0, XSpiral1[(int)j] }, new List<double> { 0, YSpiral1[(int)j] }, Styles.TransparentLine);
                    xMarkerCoords.Add(XSpiral1[(int)j]);
                    yMarkerCoords.Add(YSpiral1[(int)j]);
                }
                _graphService.SetAxisBoundaries(lineGraph, XSpiral1.Min() - .025 * (XSpiral1.Max() - XSpiral1.Min()), XSpiral1.Max() + .025 * (XSpiral1.Max() - XSpiral1.Min()), YSpiral1.Min() - .025 * (YSpiral1.Max() - YSpiral1.Min()), YSpiral1.Max() + .025 * (YSpiral1.Max() - YSpiral1.Min()));
                _graphService.AddSeries(lineGraph, null, xMarkerCoords, yMarkerCoords, Styles.GreenDiamondMarker);

                // Heatmap / Contour plot
                _graphService.AddContourPlot(graphCollection, "Graph 6", "x", "y", Xmin, Xmax, Ymin, Ymax, _levels, GetPoints(Xmin, Xmax, Ymin, Ymax, 100, Surface.Algorithm));
            }
        }

        /// <summary>
        /// Dictionary relating graph collections to graphs, used in OnCreateGraphsMethod2(...)
        /// </summary>
        private readonly Dictionary<Guid, List<Guid>> _collections = new Dictionary<Guid, List<Guid>>();

        /// <summary>
        /// Creates a group of graph collections that are updated in a cycle
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnCreateGraphsMethod2(object sender, RoutedEventArgs e)
        {
            #region Setup For Method 2

            var _collections = new Dictionary<Guid, List<Guid>>();
            var a = _graphService.AddGraphCollection("Collection A");
            _collections[a] = new List<Guid>()
            {
                _graphService.AddLineGraph(a, "A1", "x", "y", false),
            };

            var b = _graphService.AddGraphCollection("Collection B");
            _collections[b] = new List<Guid>()
            {
                _graphService.AddLineGraph(b, "B1", "x", "y", false),
                _graphService.AddLineGraph(b, "B2", "x", "y", false),
            };

            var c = _graphService.AddGraphCollection("Collection C");
            _collections[c] = new List<Guid>()
            {
                _graphService.AddLineGraph(c, "C1", "x", "y", false),
                _graphService.AddLineGraph(c, "C2", "x", "y", false),
                _graphService.AddLineGraph(c, "C3", "x", "y", false),
            };
            _graphService.SetUiHistory(_collections.Count);
            _graphService.SetArchiveSize(5);

            #endregion Setup For Method 2

            for (var i = 0; i < 3; i++)
            {
                foreach (var collection in _collections.Keys)
                {
                    foreach (var graph in _collections[collection])
                    {
                        _graphService.ClearGraph(graph);
                    }

                    _graphService.NavigateTo(collection);

                    foreach (var graph in _collections[collection])
                    {
                        Thread.Sleep(250);

                        const int numLines = 6;
                        var xCoords = new List<double>[numLines];
                        var yCoords = new List<double>[numLines];
                        for (var j = 0; j < xCoords.Length; j++)
                        {
                            xCoords[j] = new List<double>();
                            yCoords[j] = new List<double>();
                        }

                        for (var j = 0; j < XSpiral1.Count; j++)
                        {
                            xCoords[j%numLines].Add(XSpiral1[j]);
                            yCoords[j%numLines].Add(YSpiral1[j]);
                        }

                        _graphService.SetAxisBoundaries(graph, XSpiral1.Min() - .1 * (XSpiral1.Max() - XSpiral1.Min()), XSpiral1.Max() + .1 * (XSpiral1.Max() - XSpiral1.Min()), YSpiral1.Min() - .1 * (YSpiral1.Max() - YSpiral1.Min()), YSpiral1.Max() + .1 * (YSpiral1.Max() - YSpiral1.Min()));

                        for (var j = 0; j < numLines; j++)
                        {
                            Styles.TransparentLine.LineColor = Color.FromArgb(0xA0,
                                (int)(0xff * (Math.Cos((j + 2) / 1.5) + 1) / 2),
                                (int)(0xff * (Math.Cos((j + 3) / 2.0) + 1) / 2),
                                (int)(0xff * (Math.Cos((j + 4) / 2.5) + 1) / 2));

                            _graphService.AddSeries(graph, null, xCoords[j], yCoords[j], Styles.TransparentLine);
                        }

                        _graphService.AddSeries(graph, null, XSpiral1, YSpiral1, Styles.SlateBlueLine);
                        Thread.Sleep(250);
                    }
                }
            }
        }

        /// <summary>
        /// Creates and nxn array representing the algorithms surface
        /// </summary>
        /// <param name="xmin">The minimum x value</param>
        /// <param name="xmax">The maximum x value</param>
        /// <param name="ymin">The minimum y value</param>
        /// <param name="ymax">The maximum y value</param>
        /// <param name="steps">The number of steps to take in each direction</param>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public static double[][] GetPoints(double xmin, double xmax, double ymin, double ymax, int steps, Func<double, double, double> algorithm)
        {
            var data = new double[steps][];

            var xStepSize = (xmax - xmin) / steps;
            var yStepSize = (ymax - ymin) / steps;

            for (var xStep = 0; xStep < steps; xStep++)
            {
                var column = new double[steps];
                for (var yStep = 0; yStep < steps; yStep++)
                {
                    column[yStep] = algorithm(xmin + xStep * xStepSize, ymin + yStep * yStepSize);
                }
                data[xStep] = column;
            }

            return data;
        }

        /// <summary>
        /// Close the graph service and save state information
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">Event args</param>
        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _graphService.Close();
            Settings.Default.Left = Left;
            Settings.Default.Top = Top;
            Settings.Default.Save();
        }
    }

    /// <summary>
    /// Sample class containing a method that maps x and y to z
    /// </summary>
    public static class Surface
    {
        /// <summary>
        /// An algorithm describing a surface
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <returns>The z value</returns>
        public static double Algorithm(double x, double y)
        {
            return 1 + 2*Math.Cos(x + y) * Math.Cos(x - y) / 2;
        }
    }
}
