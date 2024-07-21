using Livelox2png.entities;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using Control = Livelox2png.entities.livelox.Control;

namespace Livelox2png.services.CourseDrawer;

internal static class StartDrawer
{
    /// <summary>
    /// Draws a start symbyl
    /// </summary>
    /// <param name="control">The start</param>
    /// <param name="nextControl">The first control (to determine angle)</param>
    public static void DrawStart(Control control, Control nextControl, Map map, Action<IPath> DrawPath)
    {
        var coord = new Coordinate() { Latitude = control.Position.Latitude, Longitude = control.Position.Longitude };
        var pos = coord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        // a = side length of triangle
        var aOuter = control.SymbolSize != 0.0 ? (float)control.SymbolSize * map.Resolution * 0.3f : 96f * map.Resolution;
        var aInner = control.SymbolLineWidth != 0.0 ? aOuter - (float)control.SymbolLineWidth * map.Resolution * 3f : 74f * map.Resolution;

        // r = radius of inscribed circle
        var rOuter = Math.Sqrt(3) * aOuter / 6;
        var altitudeOuter = Math.Sqrt(3) * aOuter / 2;
        var rInner = Math.Sqrt(3) * aInner / 6;
        var altitudeInner = Math.Sqrt(3) * aInner / 2;

        var outer = new PointD[] {
            new(pos.X, pos.Y - altitudeOuter + rOuter), // top
            new(pos.X + aOuter / 2, pos.Y + rOuter), // bottom right
            new(pos.X - aOuter / 2, pos.Y + rOuter), // bottom left
        };

        var inner = new PointD[] {
            new(pos.X, pos.Y - altitudeInner + rInner), // top
            new(pos.X + aInner / 2, pos.Y + rInner), // bottom right
            new(pos.X - aInner / 2, pos.Y + rInner), // bottom left
        };

        // Angle to first control
        var coord1 = new Coordinate() { Latitude = nextControl.Position.Latitude, Longitude = nextControl.Position.Longitude };
        var pos1 = coord1.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var angle = LinearAlgebraUtil.GetAngleR(pos1 - pos) + Math.PI / 2;

        Polygon triangle = new Polygon(outer.Select(p => LinearAlgebraUtil.RotateF(p, pos, -angle)).ToArray());
        IPath trianglePath = triangle.Clip(new Polygon(inner.Select(p => LinearAlgebraUtil.RotateF(p, pos, -angle)).ToArray()));

        DrawPath(trianglePath);
    }
}
