using Livelox2png.entities;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using Control = Livelox2png.entities.livelox.Control;

namespace Livelox2png.services.CourseDrawer;

internal static class FinishDrawer
{
    /// <summary>
    /// Draws a finish symbol
    /// </summary>
    /// <param name="control">The finish control</param>
    public static void DrawFinish(Control control, Map map, Action<IPath> DrawPath)
    {
        var ctrlCoord = new Coordinate
        {
            Latitude = control.Position.Latitude,
            Longitude = control.Position.Longitude
        };
        var posP = ctrlCoord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        // Do four circles, clipping every other

        var bigOuter = control.SymbolSize != 0.0 ? (float)control.SymbolSize * 2.5f : 34f;
        var lineWidth = control.SymbolLineWidth != 0.0 ? (float)control.SymbolLineWidth * 2.3f : 4f;
        var bigInner = bigOuter - lineWidth;

        var smallOuter = bigOuter * 0.7f;
        var smallInner = smallOuter - lineWidth;

        // Big circle
        EllipsePolygon bigOuterCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, bigOuter);
        // Cut out a circle
        IPath bigControlPath = bigOuterCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, bigInner));

        DrawPath(bigControlPath);

        // Small circle
        EllipsePolygon smallOuterCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, smallOuter);
        // Cut out a circle
        IPath smallControlPath = smallOuterCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, smallInner));

        DrawPath(smallControlPath);
    }
}
