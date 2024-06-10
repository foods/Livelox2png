namespace Livelox2png.entities.livelox;

internal class Activity
{
    public required Map Map { get; set; }
    public required TileData TileData { get; set; }
    public IList<Participant>? Participants { get; set; }
    public IList<Course>? Courses { get; set; }
}
