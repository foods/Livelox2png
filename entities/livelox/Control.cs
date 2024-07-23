namespace Livelox2png.entities.livelox;

internal enum ControlType
{
    Start = 0,
    Control = 1,
    Finish = 2,
}

internal class Control
{
    public int NumericCode { get; set; }
    public ControlType Type { get; set; }
    public required Coordinate Position { get; set; }
    public int ProjectionEpsgCode { get; set; }
    // public int MapScale { get; set; }
    public required MapPosition MapPosition { get; set; }
    public required string Code { get; set; }
    public double SymbolSize { get; set; }
    public double SymbolLineWidth { get; set; }
    public double MapScale { get; set; }
}
