using Livelox2png.entities;
using Livelox2png.entities.livelox;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Svg;
using System.Drawing.Imaging;

namespace Livelox2png.services;

internal static class CourseImageDrawer
{
    public static async Task DrawCourseImage(Image image, CourseImage courseImage, entities.Map map)
    {
        var liveloxClient = new LiveloxClient();
        var svgStream = await liveloxClient.FetchMapImage(courseImage.Url);

        // Load svg using Svg library
        var svgDoc = SvgDocument.Open<SvgDocument>(svgStream);

        // Using the bounding polygon of the courseimage, calculate the PointD of the corners
        var corners = new MapCorners
        {
            SouthWest = new entities.Coordinate
            {
                Latitude = courseImage.BoundingPolygon.Vertices[0].Latitude,
                Longitude = courseImage.BoundingPolygon.Vertices[0].Longitude,
            },
            SouthEast = new entities.Coordinate
            {
                Latitude = courseImage.BoundingPolygon.Vertices[1].Latitude,
                Longitude = courseImage.BoundingPolygon.Vertices[1].Longitude,
            },
            NorthEast = new entities.Coordinate
            {
                Latitude = courseImage.BoundingPolygon.Vertices[2].Latitude,
                Longitude = courseImage.BoundingPolygon.Vertices[2].Longitude,
            },
            NorthWest = new entities.Coordinate
            {
                Latitude = courseImage.BoundingPolygon.Vertices[3].Latitude,
                Longitude = courseImage.BoundingPolygon.Vertices[3].Longitude,
            },
        };

        var nw = corners.NorthWest.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);
        var se = corners.SouthEast.ProjectAndTransform(map.ProjectionOrigin, map.ProjectionMatrix);

        // Calculate svg width & height based on the projected corners
        var height = (int)Math.Abs(nw.Y - se.Y);
        var width = (int)Math.Abs(nw.X - se.X);

        // Draw it onto a bitmap
        var bitmap = svgDoc.Draw(width, height);

        // Save it as a png on a memory stream
        using var bitmapStream = new MemoryStream();
        bitmap.Save(bitmapStream, ImageFormat.Png);

        // Load it to an ImageSharp image
        bitmapStream.Position = 0;
        var imageCourse = Image.Load(bitmapStream);

        // Draw it over the image using the NW bounding box point as backgroundLocation param
        image.Mutate(i => i.DrawImage(imageCourse, new Point((int)nw.X, (int)nw.Y), opacity: 1.0f));
    }
}
