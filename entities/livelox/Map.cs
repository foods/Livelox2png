namespace Livelox2png.entities.livelox;

internal class Map
{
    public required List<MapTile> Tiles { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required Rectangle BoundingQuadrilateral { get; set; }
    public required Rectangle Polygon { get; set; }
}
