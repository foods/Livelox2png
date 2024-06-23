namespace Livelox2png.entities.livelox;

internal class Map
{
    public required List<MapTile> Tiles { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required Rectangle BoundingQuadrilateral { get; set; }
    public required Rectangle Polygon { get; set; }
    public required Projection DefaultProjection { get; set; }
    public double Rotation { get; set; }
    public double Resolution { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
