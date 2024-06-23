using Livelox2png.entities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Control = Livelox2png.entities.livelox.Control;

namespace Livelox2png.services.CourseDrawer;

internal static class ControlNumberDrawer
{
    /// <summary>
    /// Plots the control number onto the map image. Previous and next control needs to
    /// be provided in order to determine the location of the text. Text is trivially
    /// placed on the outside of the corner created by the "in" and "out" line to and
    /// from the control
    /// </summary>
    /// <param name="control">the control</param>
    /// <param name="previous">previous control</param>
    /// <param name="next">next control</param>
    /// <param name="controlNumber">control number</param>
    public static void DrawControlNumber(Control control, Control previous, Control next, int controlNumber, Image image, Map map)
    {
        // The control number 
        var posPrevCoord = new Coordinate() { Latitude = previous.Position.Latitude, Longitude = previous.Position.Longitude };
        var posCoord = new Coordinate() { Latitude = control.Position.Latitude, Longitude = control.Position.Longitude };
        var posNextCoord = new Coordinate() { Latitude = next.Position.Latitude, Longitude = next.Position.Longitude };

        var posPrev = posPrevCoord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var pos = posCoord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var posNext = posNextCoord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        // Consider the previous and next control as vectors from control. Add them
        var vectorPrev = LinearAlgebraUtil.Normalize(posPrev - pos);
        var vectorNext = LinearAlgebraUtil.Normalize(posNext - pos);
        var avgVector = vectorPrev + vectorNext;

        var distance = control.SymbolSize != 0.0 ? (float)control.SymbolSize * map.Resolution * 2.0f : 70f;

        // Normalize the vector and subtract it from the pos to find coordinates for Control number
        var controlNumberPos = pos - LinearAlgebraUtil.Normalize(avgVector) * distance;

        var fontSize = control.SymbolSize != 0.0 ? (int)(control.SymbolSize * map.Resolution * 1.4f) : 45;
        var font = SystemFonts.CreateFont("Arial", fontSize, FontStyle.Bold);
        var origin = new System.Numerics.Vector2((float)controlNumberPos.X, (float)controlNumberPos.Y);
        var textOptions = new RichTextOptions(font)
        {
            Origin = origin,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        image.Mutate(x => x.DrawText(textOptions, controlNumber.ToString(), Brushes.Solid(DrawingConstants.Purple), Pens.Solid(Color.White, 1)));
    }

}
