using System.Text.Json.Serialization;

namespace Livelox2png.entities.livelox;

internal class Course
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public float Length { get; set; }
    public double ControlConnectionLineWidth { get; set; }
    public required List<CourseControl> Controls { get; set; }
    public required List<CompetitionClass> Classes { get; set; }
    public List<CourseImage>? CourseImages { get; set; }
}
