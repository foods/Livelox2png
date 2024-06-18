namespace Livelox2png.entities.livelox.classinfo;

public class ClassBlobRequest
{
    public int? EventId { get; set; }
    public int[] ClassIds { get; set; } = Array.Empty<int>();
    public int[]? CourseIds { get; set; }
    public int[]? RelayLegs { get; set; }
    public int[]? RelayLegGroupIds { get; set; }
    public RouteReductionProperties RouteReductionProperties { get; set; } = new RouteReductionProperties();
    public bool IncludeMap { get; set; } = true;
    public bool IncludeCourses { get; set; } = true;
    public bool SkipStoreInCache { get; set; }

    public ClassBlobRequest(int classId)
    {
        ClassIds = [classId];
    }
}
