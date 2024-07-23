namespace Livelox2png.entities.livelox;

internal class CourseImage
{
    public double Top { get; set; }
    public double Bottom { get; set; }
    public double Left { get; set; }
    public double Right { get; set; }
    public int CourseId { get; set; }
    public required Projection DefaultProjection { get; set; }
    public required Rectangle BoundingPolygon { get; set; }
    public double MapScale { get; set; }
    public string Url { get; set; } = string.Empty;
}
