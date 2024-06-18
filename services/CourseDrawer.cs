using Livelox2png.entities;
using Microsoft.Extensions.Configuration;
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
    private readonly ActivityExtractor activityExtractor;

    public CourseDrawer(Map map, Activity activity, Image image, IConfigurationRoot config)
    {
        this.map = map;
        this.activity = activity;
        this.image = image;
        activityExtractor = new ActivityExtractor(activity, config);

        drawingOptions = new DrawingOptions()
        {
            GraphicsOptions = new GraphicsOptions()
            {
                ColorBlendingMode = PixelColorBlendingMode.Darken,
            }
        };
    }

    private void DrawPath(IPath path) => image.Mutate(x => x.Fill(drawingOptions, purple, path));


    /// <summary>
    /// Draws the course (based on the participant) onto the map image
    /// </summary>
    public void Draw()
    {
        var course = activityExtractor.GetCourse();
        var controls = course.Controls ?? [];
        for (var i = 0; i < controls.Count; i++)
        {
            var control = controls[i].Control;

            switch (control.Type)
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
            if (controls.Count > i + 1)
            {
                DrawLine(control, controls[i + 1].Control);
            }

            //if (control.Type == entities.livelox.ControlType.Start && controls.Count > i+1)
            //{
            //    DrawStart(control, controls[i + 1].Control);
            //}
        }
    }

    /// <summary>
    /// Draws a standard control ring
    /// </summary>
    /// <param name="control">The control</param>
    private void DrawControlRing(Control control)
    {
        var ctrlCoord = new Coordinate
        {
            Latitude = control.Position.Latitude,
            Longitude = control.Position.Longitude
        };
        var posP = ctrlCoord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        var size = control.SymbolSize != 0.0 ? (float)control.SymbolSize * 2.3f : 31f;
        var innerSize = control.SymbolLineWidth != 0.0 ? size - (float)control.SymbolLineWidth * 2.3f : 27f;

        EllipsePolygon outerCircle = new EllipsePolygon((float)posP.X, (float)posP.Y, size);
        // Cut out a circle
        IPath controlPath = outerCircle.Clip(new EllipsePolygon((float)posP.X, (float)posP.Y, innerSize));

        DrawPath(controlPath);
    }

    /// <summary>
    /// Draws a start symbyl
    /// </summary>
    /// <param name="control">The start</param>
    /// <param name="nextControl">The first control (to determine angle)</param>
    private void DrawStart(Control control, Control nextControl)
    {
        var coord = new Coordinate() { Latitude = control.Position.Latitude, Longitude = control.Position.Longitude };
        var pos = coord.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        // a = side length of triangle
        var aOuter = control.SymbolSize != 0.0 ? (float)Math.Min(control.SymbolSize, 14f) * 4.6f : 62f;
        var aInner = control.SymbolLineWidth != 0.0 ? aOuter - (float)control.SymbolLineWidth * 8f : 50f;

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

    /// <summary>
    /// Draws a finish symbol
    /// </summary>
    /// <param name="control">The finish control</param>
    private void DrawFinish(Control control)
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

    /// <summary>
    /// Draws a line between two controls
    /// </summary>
    /// <param name="control1">First control</param>
    /// <param name="control2">Second control</param>
    private void DrawLine(Control control1, Control control2)
    {
        var controlLine = GetControlToControlLine(control1, control2);
        var points = new PointF[] {
            new PointF((float)controlLine.from.X, (float)controlLine.from.Y),
            new PointF((float)controlLine.to.X, (float)controlLine.to.Y),
        };
        var lineWidth = (float)Math.Max(Math.Max(control1.SymbolLineWidth, control2.SymbolLineWidth), 4f);

        image.Mutate(x => x.DrawLine(drawingOptions, Pens.Solid(purple, lineWidth), points));
    }

    /// <summary>
    /// Gets a line (to be drawn) between two controls
    /// </summary>
    /// <param name="control1">First control</param>
    /// <param name="control2">Second control</param>
    /// <returns>A structure type with the first and second xy-point of the line</returns>
    private (PointD from, PointD to) GetControlToControlLine(Control control1, Control control2)
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
    /// Plots the control number onto the map image. Previous and next control needs to
    /// be provided in order to determine the location of the text. Text is trivially
    /// placed on the outside of the corner created by the "in" and "out" line to and
    /// from the control
    /// </summary>
    /// <param name="control">the control</param>
    /// <param name="previous">previous control</param>
    /// <param name="next">next control</param>
    /// <param name="controlNumber">control number</param>
    private void DrawControlNumber(Control control, Control previous, Control next, int controlNumber)
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
