using Livelox2png.entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using Control = Livelox2png.entities.livelox.Control;

namespace Livelox2png.services.CourseDrawer;

internal static class ControlRingDrawer
{
    /// <summary>
    /// Draws a standard control ring
    /// </summary>
    /// <param name="control">The control</param>
    public static void DrawControlRing(Control control, Map map, Action<IPath> DrawPath)
    {
        var ctrlCoord = new Coordinate
        {
            Latitude = control.Position.Latitude,
            Longitude = control.Position.Longitude
        };
        var posP = ctrlCoord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var res = (float)map.Resolution;
        
        var size = control.SymbolSize != 0.0 ?
            (float)(control.SymbolSize + control.SymbolLineWidth / 2) * res :
            (float)(40f * res * control.MapScale / 15000f);
        var innerSize = control.SymbolLineWidth != 0.0 ?
            size - (float)control.SymbolLineWidth * res :
            (float)(35f * res * control.MapScale / 15000f);

        EllipsePolygon outerCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, size);
        // Cut out a circle
        IPath controlPath = outerCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, innerSize));


        foreach (var circleGap in control.CircleGaps ?? [])
        {
            // Cut out a triangle that has its first corner in the center of the control,
            // then the other two based on the angle and with a length that is a bit longer
            // than the radius of the control circle

            var startVector = LinearAlgebraUtil.GetNormalizedVector(circleGap.StartAngle);
            var endVector = LinearAlgebraUtil.GetNormalizedVector(circleGap.StartAngle + circleGap.Distance);
            // Y coordinates in graphics are reversed
            startVector.Y = -startVector.Y;
            endVector.Y = -endVector.Y;

            var cutOutTriangle = new PointD[]
            {
                posP,
                posP + startVector * size * 1.5,
                posP + endVector * size * 1.5,
            };

            controlPath = controlPath.Clip(new Polygon(cutOutTriangle.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray()));
        }

        DrawPath(controlPath);
    }
}
