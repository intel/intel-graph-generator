using System.Drawing;

namespace WpfGraphService
{
    /// <summary>
    /// The position for the line graph legend
    /// </summary>
    public enum LegendPosition
    {
        TopLeft = 0, TopCenter = 1, TopRight = 2,
        BottomLeft = 3, BottomCenter = 4, BottomRight = 5,
        LeftTop = 6, LeftMiddle = 7, LeftBottom = 8,
        RightTop = 9, RightMiddle = 10, RightBottom = 11,
    }

    /// <summary>
    /// The style for the line series
    /// </summary>
    public class LineStyle
    {
        public Color LineColor { get; set; }
        public LineDashedness Dashedness { get; set; }
        public double Thickness { get; set; }
        public bool Smooth { get; set; }
        public MarkerType MarkerType { get; set; }
        public Color MarkerFill { get; set; }
        public Color MarkerStroke { get; set; }
        public double MarkerSize { get; set; }
        public double MarkerStrokeThickness { get; set; }
    }

    /// <summary>
    /// Dash style for lines series
    /// </summary>
    public enum LineDashedness
    {
        /// <summary>
        /// A solid line
        /// </summary>
        Solid,
        /// <summary>
        /// A dashed line
        /// </summary>
        Dashed,
        /// <summary>
        /// A dotted line
        /// </summary>
        Dotted,
        /// <summary>
        /// A dash then a dot
        /// </summary>
        DashDot,
    }

    /// <summary>
    /// Optional lines seriers markers
    /// </summary>
    public enum MarkerType
    {
        /// <summary>
        /// Do not render markers.
        /// </summary>
        None = 0,
        /// <summary>
        /// Render markers as circles.
        /// </summary>
        Circle = 1,
        /// <summary>
        /// Render markers as squares.
        /// </summary>
        Square = 2,
        /// <summary>
        /// Render markers as diamonds.
        /// </summary>
        Diamond = 3,
        /// <summary>
        /// Render markers as triangles.
        /// </summary>
        Triangle = 4,
        /// <summary>
        /// Render markers as crosses (note: this marker type requires the stroke color to be set).
        /// </summary>
        Cross = 5,
        /// <summary>
        /// Renders markers as plus signs (note: this marker type requires the stroke color to be set).
        /// </summary>
        Plus = 6,
        /// <summary>
        /// Renders markers as stars (note: this marker type requires the stroke color to be set).
        /// </summary>
        Star = 7,
        /// <summary>
        /// Render markers by a custom shape (defined by outline).
        /// </summary>
        Custom = 8,
    }

    /// <summary>
    /// Sample styles for lines and markers
    /// </summary>
    public class Styles
    {
        public static readonly LineStyle RedDashDotLine = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00),
            Dashedness = LineDashedness.DashDot,
            Thickness = 3,
            Smooth = true,
        };

        public static readonly LineStyle RedLine = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00),
            Dashedness = LineDashedness.Solid,
            Thickness = 3,
            Smooth = false,
        };

        public static readonly LineStyle GreenLine = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0x00, 0xBB, 0x00),
            Dashedness = LineDashedness.Dashed,
            Thickness = 3,
            Smooth = true,
        };

        public static readonly LineStyle BlueLine = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF),
            Dashedness = LineDashedness.Dotted,
            Thickness = 3,
            Smooth = true,
        };

        public static readonly LineStyle SlateBlueLine = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0x48, 0x3D, 0x8B),
            Dashedness = LineDashedness.Solid,
            Thickness = 3,
            Smooth = true,
        };

        public static readonly LineStyle RedDashDot = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0xDC, 0x14, 0x3C),
            Dashedness = LineDashedness.DashDot,
            Thickness = 8,
            Smooth = true,
        };

        public static readonly LineStyle BlueWithMarkers = new LineStyle
        {
            LineColor = Color.Blue,
            Dashedness = LineDashedness.Solid,
            Thickness = 2,
            Smooth = false,
            MarkerType = MarkerType.Circle,
            MarkerFill = Color.CornflowerBlue,
            MarkerStroke = Color.DarkBlue,
            MarkerSize = 4,
            MarkerStrokeThickness = 2,
        };

        public static readonly LineStyle BlueStarMarkers = new LineStyle
        {
            LineColor = Color.Transparent,
            MarkerType = MarkerType.Star,
            MarkerStroke = Color.FromArgb(0xAA, 0x00, 0x00, 0xFF),
            MarkerSize = 5,
            MarkerStrokeThickness = 2
        };

        public static readonly LineStyle RedCrossMarkers = new LineStyle
        {
            LineColor = Color.Transparent,
            MarkerType = MarkerType.Cross,
            MarkerStroke = Color.FromArgb(0xAA, 0xFF, 0x00, 0x00),
            MarkerSize = 5,
            MarkerStrokeThickness = 2
        };

        public static readonly LineStyle TransparentLine = new LineStyle
        {
            LineColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00),
            Dashedness = LineDashedness.Solid,
            Thickness = 10,
            Smooth = true,
        };

        public static readonly LineStyle GreenDiamondMarker = new LineStyle
        {
            LineColor = Color.Transparent,
            MarkerType = MarkerType.Diamond,
            MarkerFill = Color.Chartreuse,
            MarkerStroke = Color.DarkGreen,
            MarkerSize = 5,
            MarkerStrokeThickness = 2,
        };

        public static readonly LineStyle DiamondMarker = new LineStyle
        {
            LineColor = Color.Transparent,
            MarkerType = MarkerType.Diamond,
            MarkerFill = Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF),
            MarkerStroke = Color.FromArgb(0xFF, 0x00, 0x00, 0xFF),
            MarkerSize = 4,
            MarkerStrokeThickness = 3,
        };

        public static readonly LineStyle BottomLineColor = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0xFF, 0x00, 0x80),
            Dashedness = LineDashedness.Solid,
            Thickness = 10,
            Smooth = false,
        };

        public static readonly LineStyle TopLineColor = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF),
            Dashedness = LineDashedness.Solid,
            Thickness = 4,
            Smooth = false,
        };

        public static readonly LineStyle ErrorMarker = new LineStyle
        {
            LineColor = Color.FromArgb(0xFF, 0xFF, 0x00, 0x00),
            MarkerType = MarkerType.Circle,
            MarkerStroke = Color.Transparent,
            MarkerFill = Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF),
            MarkerSize = 5,
            MarkerStrokeThickness = 1,
            Thickness = 5,
            Smooth = false,
        };
    }
}
