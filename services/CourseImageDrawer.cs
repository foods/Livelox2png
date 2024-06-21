using Livelox2png.entities.livelox;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Svg;
using System.Drawing.Imaging;

namespace Livelox2png.services;

internal static class CourseImageDrawer
{
    public static async Task DrawCourseImage(Image image, CourseImage courseImage)
    {
        var liveloxClient = new LiveloxClient();
        var svgStream = await liveloxClient.FetchMapImage(courseImage.Url);

        // Load svg using Svg library
        var svgDoc = SvgDocument.Open<SvgDocument>(svgStream);

        var imgHwRatio = (double)image.Height / image.Width;
        var svgHwRatio = (double)svgDoc.Height / (double)svgDoc.Width;
        if ( Math.Abs((imgHwRatio / svgHwRatio) - 1) > 1.02 )
        {
            Console.WriteLine(
                @"Warning. Course image proportions differ from the map image. Course image might be off\
- Livelox2png does not yet support projecting course images."
                );
        }

        // Draw it onto a bitmap
        var bitmap = svgDoc.Draw(0, image.Height);

        // Save it as a png on a memory stream
        using var bitmapStream = new MemoryStream();
        bitmap.Save(bitmapStream, ImageFormat.Png);

        // Load it to an ImageSharp image
        bitmapStream.Position = 0;
        var imageCourse = Image.Load(bitmapStream);

        // Draw it over the image
        image.Mutate(i => i.DrawImage(imageCourse, new Point(1, 1), opacity: 1.0f));
    }
}
