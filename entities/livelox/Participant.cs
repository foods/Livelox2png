namespace Livelox2png.entities.livelox;

internal class Participant
{
    public long Id { get; set; }
    public int ClassId { get; set; }
    public TimeInterval? TimeInterval { get; set; }
    public TimeInterval? SessionTimeInterval { get; set; }
}
