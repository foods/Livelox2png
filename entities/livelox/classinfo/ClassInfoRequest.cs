namespace Livelox2png.entities.livelox.classinfo;

public class ClassInfoRequest
{
    public string? EventId { get; set; }
    public int[] ClassIds { get; set; } = Array.Empty<int>();
    public int[] CourseIds { get; set; } = Array.Empty<int>();
    public int[] RelayLegs { get; set; } = Array.Empty<int>();
    public int[] RelayLegGroupIds { get; set; } = Array.Empty<int>();
}
