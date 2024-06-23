using Livelox2png.entities;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Activity = Livelox2png.entities.livelox.Activity;

namespace Livelox2png.services.CourseDrawer;

internal class CourseDrawer
{
    private readonly Map map;
    private readonly Activity activity;
    private readonly Image image;
    private readonly DrawingOptions drawingOptions;
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

    private void DrawPath(IPath path) => image.Mutate(x => x.Fill(drawingOptions, DrawingConstants.Purple, path));


    /// <summary>
    /// Draws the course (based on the participant) onto the map image
    /// </summary>
    public async Task Draw()
    {
        var course = activityExtractor.GetCourse();
        if (course.CourseImages != null && course.CourseImages.Any())
        {
            foreach (var courseImage in course.CourseImages)
            {
                await CourseImageDrawer.DrawCourseImage(image, courseImage);
            }
            // Don't plot the course if there are course images
            return;
        }
        var controls = course.Controls ?? [];
        var controlIndex = 0;

        var getNextControl = (int ctrlIndex) =>
        {
            while (ctrlIndex < controls.Count)
            {
                if (controls[ctrlIndex].Control.Type == entities.livelox.ControlType.Control ||
                controls[ctrlIndex].Control.Type == entities.livelox.ControlType.Finish)
                {
                    return controls[ctrlIndex].Control;
                }
                ctrlIndex++;
            }
            return null;
        };

        for (var i = 0; i < controls.Count; i++)
        {
            var control = controls[i].Control;

            switch (control.Type)
            {
                case entities.livelox.ControlType.Start:
                    if (controls.Count >= i)
                    {
                        StartDrawer.DrawStart(control, controls[i + 1].Control, map, DrawPath);
                    }
                    break;
                case entities.livelox.ControlType.Control:
                    controlIndex++;
                    ControlRingDrawer.DrawControlRing(control, map, DrawPath);
                    ControlNumberDrawer.DrawControlNumber(control, controls[i - 1].Control, getNextControl(i + 1), controlIndex, image);
                    break;
                case entities.livelox.ControlType.Finish:
                    FinishDrawer.DrawFinish(control, map, DrawPath);
                    break;

            }
            if (controls.Count > i + 1 && controls[i + 1].Control.Type != entities.livelox.ControlType.Start)
            {
                LineDrawer.DrawLine(control, controls[i + 1].Control, map, image, drawingOptions);
            }
        }
    }
}
