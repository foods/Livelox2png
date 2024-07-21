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
        var res = (float)map.Resolution;
        
        var size = control.SymbolSize != 0.0 ? (float)control.SymbolSize * res : 40f * res;
        var innerSize = control.SymbolLineWidth != 0.0 ? size - (float)control.SymbolLineWidth * res : 35f * res;

        EllipsePolygon outerCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, size);
        // Cut out a circle
        IPath controlPath = outerCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, innerSize));

        DrawPath(controlPath);
    }
}
