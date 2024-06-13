using Livelox2png.entities;

namespace Livelox2png.projection;

internal class Projector
{
    private readonly Map Map;

    public Projector(Map map)
    {
        Map = map;
    }

    public PointD Project(Coordinate coordinate)
    {
        // latitude diff between coordinate and NW corner
        var d_lat = Map.Corners.NorthWest.Latitude - coordinate.Latitude;
        // latitude diff between coordinate and SW corner
        var e_lat = Map.Corners.NorthWest.Latitude - Map.Corners.SouthWest.Latitude - d_lat;
        var d_long = coordinate.Longitude - Map.Corners.NorthWest.Longitude;
        var e_long = coordinate.Longitude - Map.Corners.SouthWest.Longitude;

        return new PointD {
            X = (e_long * Math.Cos(Map.Rotation) - e_lat * Math.Sin(Map.Rotation)) * Map.ScaleX,
            Y = (d_lat * Math.Cos(Map.Rotation) - d_long * Math.Sin(Map.Rotation)) * Map.ScaleY,
        };
    }
}
