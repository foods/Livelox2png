using Livelox2png.entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using Control = Livelox2png.entities.livelox.Control;
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
    private static (PointD from, PointD to) GetControlToControlLine(Map map, Control control1, Control control2)
    {
        var coord1 = new Coordinate() { Latitude = control1.Position.Latitude, Longitude = control1.Position.Longitude };
        var coord2 = new Coordinate() { Latitude = control2.Position.Latitude, Longitude = control2.Position.Longitude };
        var pos1 = coord1.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var pos2 = coord2.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        var symbolSize = Math.Min(control1.SymbolSize, control2.SymbolSize);
        var clipLength = symbolSize != 0.0 ? (float)symbolSize * 3.0f : 40f;

        var centerToCenterDist = LinearAlgebraUtil.LineDistance(pos1, pos2);
        var fromCenterLength = centerToCenterDist / 2 - clipLength;

        if (pos1.X == pos2.X)
        {
            return (
                new PointD(pos1.X, pos1.Y > pos2.Y ? pos1.Y - fromCenterLength : pos1.Y + fromCenterLength),
                new PointD(pos2.X, pos2.Y > pos1.Y ? pos2.Y - fromCenterLength : pos2.Y + fromCenterLength));
        }

        var k = (pos2.Y - pos1.Y) / (pos2.X - pos1.X);

        // width from center of line
        var b = Math.Sqrt(fromCenterLength * fromCenterLength / (k * k + 1));
        // height from center of line
        var h = b * k;
        // as a vector
        var diffVector = new PointD(b, h);

        var centerOfLine = new PointD((pos1.X + pos2.X) / 2, (pos1.Y + pos2.Y) / 2);

        return (centerOfLine - diffVector, centerOfLine + diffVector);
    }

    /// <summary>
    /// Draws a line between two controls
    /// </summary>
    /// <param name="control1">First control</param>
    /// <param name="control2">Second control</param>
    public static void DrawLine(Control control1, Control control2, Map map, Image image, DrawingOptions drawingOptions)
    {
        var controlLine = GetControlToControlLine(map, control1, control2);
        var points = new PointF[] {
            new((float)controlLine.from.X, (float)controlLine.from.Y),
            new((float)controlLine.to.X, (float)controlLine.to.Y),
        };
        var lineWidth = (float)Math.Max(Math.Max(control1.SymbolLineWidth, control2.SymbolLineWidth), 4f);

        image.Mutate(x => x.DrawLine(drawingOptions, Pens.Solid(DrawingConstants.Purple, lineWidth), points));
    }

}
