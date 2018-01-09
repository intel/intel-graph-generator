using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using HeatMapSeries = OxyPlot.Series.HeatMapSeries;
using LinearColorAxis = OxyPlot.Axes.LinearColorAxis;

namespace GraphUI
{
    public class ContourPlot : PlotView
    {
        private ContourPlot()
        {
        }

        /// <summary>
        /// Creates a contour plot
        /// </summary>
        /// <param name="title">The title for the plot</param>
        /// <param name="xMin">The minimum x value</param>
        /// <param name="xMax">The maximum x value</param>
        /// <param name="yMin">The minimum y value</param>
        /// <param name="yMax">The maximum y value</param>
        /// <param name="points">A rectangular array of the values to plot</param>
        /// <param name="levels">The countours to display on the graph</param>
        /// <param name="xAxis">The x-axis label</param>
        /// <param name="yAxis">The y-axis label</param>
        public ContourPlot(string title, string xAxis, string yAxis, double xMin, double xMax, double yMin, double yMax, double[][] points, double[] levels) : this()
        {
            Model = new PlotModel
            {
                Title = title,
                IsLegendVisible = false,
            };

            Model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(500), HighColor = OxyColors.Gray, LowColor = OxyColors.Black });
            Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Title = xAxis, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Position = AxisPosition.Bottom });
            Model.Axes.Add(new OxyPlot.Axes.LinearAxis { Title = yAxis, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });

            Model.Series.Add(new HeatMapSeries
            {
                X0 = xMin,
                X1 = xMax,
                Y0 = yMin,
                Y1 = yMax,
                Data = To2D(points)
            });

            Model.Series.Add(new ContourSeries
            {
                ColumnCoordinates = ArrayBuilder.CreateVector(xMin, xMax, points.Length),
                RowCoordinates = ArrayBuilder.CreateVector(yMin, yMax, points[0].Length),
                ContourLevels = levels,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 3,
                Data = To2D(points),
                LabelBackground = OxyColors.Transparent,
            });
        }

        /// <summary>
        /// Helper method to convert 2-Dimensional array to jagged arrays, useful for serializing
        /// </summary>
        /// <param name="source">The 2-D array</param>
        /// <returns>The jagged array</returns>
        private static double[][] ToJagged(double[,] source)
        {
            int numOfColumns = source.GetLength(0);
            int numOfRows = source.GetLength(1);
            double[][] jaggedArray = new double[numOfColumns][];

            for (var c = 0; c < numOfColumns; c++)
            {
                jaggedArray[c] = new double[numOfRows];
                for (var r = 0; r < numOfRows; r++)
                {
                    jaggedArray[c][r] = source[c, r];
                }
            }

            return jaggedArray;
        }

        /// <summary>
        /// Helper method to convert jagged arrays to 2-Dimensional arrays
        /// </summary>
        /// <param name="source">The jagged array</param>
        /// <returns>The 2-D array</returns>
        private static double[,] To2D(double[][] source)
        {
            try
            {
                int firstDim = source.Length;
                int secondDim = source.GroupBy(row => row.Length).Single().Key;

                var result = new double[firstDim, secondDim];
                for (var i = 0; i < firstDim; ++i)
                    for (var j = 0; j < secondDim; ++j)
                        result[i, j] = source[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }
    }
}
