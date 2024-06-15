using Livelox2png.entities;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Activity = Livelox2png.entities.livelox.Activity;
using Control = Livelox2png.entities.livelox.Control;

namespace Livelox2png.services;

internal class CourseDrawer
{
    private readonly Map map;
    private readonly Activity activity;
    private readonly Image image;
    private readonly DrawingOptions drawingOptions;
    private readonly Rgba32 purple = new(192, 42, 186);
    public CourseDrawer(Map map, Activity activity, Image image)
    {
        this.map = map;
        this.activity = activity;
        this.image = image;

        drawingOptions = new DrawingOptions()
        {
            GraphicsOptions = new GraphicsOptions()
            {
                ColorBlendingMode = PixelColorBlendingMode.Darken,
            }
        };
    }

    private void DrawPath(IPath path) => image.Mutate(x => x.Fill(drawingOptions, purple, path));


    public void Draw()
    {

        var controls = activity.Courses?.First()?.Controls ?? [];
        for (var i = 0; i < controls.Count; i++)
        {
            var control = controls[i].Control;

            switch(control.Type)
            {
                case entities.livelox.ControlType.Start:
                    if (controls.Count >= i)
                    {
                        DrawStart(control, controls[i + 1].Control);
                    }
                    break;
                case entities.livelox.ControlType.Control:
                    DrawControlRing(control);
                    DrawControlNumber(control, controls[i - 1].Control, controls[i + 1].Control, i);
                    break;
                case entities.livelox.ControlType.Finish:
                    DrawFinish(control);
                    break;

            }
            if (controls.Count > i+1)
            {
                DrawLine(control, controls[i+1].Control);
            }

            if (control.Type == entities.livelox.ControlType.Start && controls.Count > i+1)
            {
                DrawStart(control, controls[i + 1].Control);
            }
        }
    }

    private void DrawControlRing(Control control)
    {
        var ctrlCoord = new Coordinate
        {
            Latitude = control.Position.Latitude,
            Longitude = control.Position.Longitude
        };
        var pos = ctrlCoord.Project(map.ProjectionOrigin);
        var posP = LinearAlgebraUtil.Transform(pos, map.ProjectionMatrix);
        Console.WriteLine($"Control {control.Code}: {posP}");

        var size = control.SymbolSize != 0.0 ? (float)control.SymbolSize * 2.3f : 31f;
        var innerSize = control.SymbolLineWidth != 0.0 ? size - (float)control.SymbolLineWidth * 2.3f : 27f;

        EllipsePolygon outerCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, size);
        // Cut out a circle
        IPath controlPath = outerCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, innerSize));

        DrawPath(controlPath);
    }

    private void DrawStart(Control control, Control nextControl) { }

    private void DrawFinish(Control control) { }

    private void DrawLine(Control control1, Control control2) { }

    private void DrawControlNumber(Control control, Control previous, Control next, int controlNumber) {
        var posPrevCoord = new Coordinate() { Latitude = previous.Position.Latitude, Longitude = previous.Position.Longitude };
        var posCoord = new Coordinate() { Latitude = control.Position.Latitude, Longitude = control.Position.Longitude };
        var posNextCoord = new Coordinate() { Latitude = next.Position.Latitude, Longitude = next.Position.Longitude };

        var posPrev = LinearAlgebraUtil.Transform(posPrevCoord.Project(map.ProjectionOrigin), map.ProjectionMatrix);
        var pos = LinearAlgebraUtil.Transform(posCoord.Project(map.ProjectionOrigin), map.ProjectionMatrix);
        var posNext = LinearAlgebraUtil.Transform(posNextCoord.Project(map.ProjectionOrigin), map.ProjectionMatrix);

        // Consider the previous and next control as vectors from control. Add them
        var vectorPrev = LinearAlgebraUtil.Normalize(posPrev - pos);
        var vectorNext = LinearAlgebraUtil.Normalize(posNext - pos);
        var avgVector = vectorPrev + vectorNext;

        var distance = control.SymbolSize != 0.0 ? (float)control.SymbolSize * 4.8f : 70f;

        // Normalize the vector and subtract it from the pos to find coordinates for Control number
        var controlNumberPos = pos - LinearAlgebraUtil.Normalize(avgVector) * distance;

        var fontSize = control.SymbolSize != 0.0 ? (int)(control.SymbolSize * 3) : 45;
        var font = SystemFonts.CreateFont("Arial", fontSize, FontStyle.Bold);
        var origin = new System.Numerics.Vector2((float)controlNumberPos.X, (float)controlNumberPos.Y);
        var textOptions = new RichTextOptions(font)
        {
            Origin = origin,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        image.Mutate(x => x.DrawText(textOptions, controlNumber.ToString(), Brushes.Solid(purple), Pens.Solid(Color.White, 1)));
    }
}
