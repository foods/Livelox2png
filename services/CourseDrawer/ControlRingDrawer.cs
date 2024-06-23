using Livelox2png.entities;
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

        //var size = control.SymbolSize != 0.0 ? (float)control.SymbolSize * 2.3f : 31f;
        var size = control.SymbolSize != 0.0 ? (float)control.SymbolSize * (float)map.Resolution : 31f;
        var innerSize = control.SymbolLineWidth != 0.0 ? size - (float)control.SymbolLineWidth * (float)map.Resolution : 27f;

        EllipsePolygon outerCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, size);
        // Cut out a circle
        IPath controlPath = outerCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, innerSize));

        DrawPath(controlPath);
    }
}
