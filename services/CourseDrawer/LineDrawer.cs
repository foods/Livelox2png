using Livelox2png.entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using Control = Livelox2png.entities.livelox.Control;
using ConnectionLine = Livelox2png.entities.livelox.ConnectionLine;
using SixLabors.ImageSharp.Processing;

namespace Livelox2png.services.CourseDrawer;

internal static class LineDrawer
{
    /// <summary>
    /// Gets a line (to be drawn) between two controls
    /// </summary>
    /// <param name="control1">First control</param>
    /// <param name="control2">Second control</param>
    /// <returns>A structure type with the first and second xy-point of the line</returns>
    private static (PointD from, PointD to, bool tooShort) GetControlToControlLine(Map map, Control control1, Control control2)
    {
        var coord1 = new Coordinate() { Latitude = control1.Position.Latitude, Longitude = control1.Position.Longitude };
        var coord2 = new Coordinate() { Latitude = control2.Position.Latitude, Longitude = control2.Position.Longitude };
        var pos1 = coord1.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var pos2 = coord2.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        var symbolSize = Math.Min(control1.SymbolSize, control2.SymbolSize);
        var clipLength = symbolSize != 0.0 ? (float)symbolSize * (float)map.Resolution * 1.2f : 40f;

        var centerToCenterDist = LinearAlgebraUtil.LineDistance(pos1, pos2);
        var fromCenterLength = centerToCenterDist / 2 - clipLength;

        if (pos1.X == pos2.X)
        {
            return (
                new PointD(pos1.X, pos1.Y > pos2.Y ? pos1.Y - fromCenterLength : pos1.Y + fromCenterLength),
                new PointD(pos2.X, pos2.Y > pos1.Y ? pos2.Y - fromCenterLength : pos2.Y + fromCenterLength),
                false);
        }

        var k = (pos2.Y - pos1.Y) / (pos2.X - pos1.X);

        // width from center of line
        var b = Math.Sqrt(fromCenterLength * fromCenterLength / (k * k + 1));
        // height from center of line
        var h = b * k;
        // as a vector
        var diffVector = new PointD(b, h);

        var centerOfLine = new PointD((pos1.X + pos2.X) / 2, (pos1.Y + pos2.Y) / 2);

        return (centerOfLine - diffVector, centerOfLine + diffVector, fromCenterLength <= 0);
    }

    /// <summary>
    /// Draws a line between two controls
    /// </summary>
    /// <param name="control1">First control</param>
    /// <param name="control2">Second control</param>
    public static void DrawLine(Control control1, Control control2, Map map, Image image, DrawingOptions drawingOptions)
    {
        var controlLine = GetControlToControlLine(map, control1, control2);
        if (controlLine.tooShort)
        {
            // Don't draw a line if there isn't space to do so
            return;
        }
        var points = new PointF[] {
            new((float)controlLine.from.X, (float)controlLine.from.Y),
            new((float)controlLine.to.X, (float)controlLine.to.Y),
        };
        var lineWidth = control1.SymbolLineWidth != 0.0 ?
            (float)Math.Max(control1.SymbolLineWidth * map.Resolution, control2.SymbolLineWidth * map.Resolution) :
            (float)(6f * map.Resolution * control1.MapScale / 15000f);

        image.Mutate(x => x.DrawLine(drawingOptions, Pens.Solid(DrawingConstants.Purple, lineWidth), points));
    }

    /// <summary>
    /// Draws pre-specified control lines
    /// </summary>
    public static void DrawLine(Control control, Map map, Image image, DrawingOptions drawingOptions, List<ConnectionLine> connections)
    {
        foreach (var connectionLine in connections)
        {
            var coord1 = new Coordinate() { Latitude = connectionLine.Start.Latitude, Longitude = connectionLine.Start.Longitude };
            var coord2 = new Coordinate() { Latitude = connectionLine.End.Latitude, Longitude = connectionLine.End.Longitude };
            var pos1 = coord1.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
            var pos2 = coord2.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
            var points = new PointF[] {
                new((float)pos1.X, (float)pos1.Y),
                new((float)pos2.X, (float)pos2.Y),
            };
            var lineWidth = control.SymbolLineWidth != 0.0 ?
                (float)(control.SymbolLineWidth * map.Resolution):
                (float)(6f * map.Resolution * control.MapScale / 15000f);

            image.Mutate(x => x.DrawLine(drawingOptions, Pens.Solid(DrawingConstants.Purple, lineWidth), points));
        }
    }
}
