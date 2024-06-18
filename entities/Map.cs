using Livelox2png.entities.livelox;

namespace Livelox2png.entities;

internal class Map
{
    public double Rotation { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public MapCorners Corners { get; set; }
    public double ScaleX { get; set; }
    public double ScaleY { get; set; }
    public Coordinate ProjectionOrigin { get; set; }
    public Matrix ProjectionMatrix { get; set; }

    public Map(Activity activity)
    {
        if (activity.Map.Width == 0 || activity.Map.Height == 0)
        {
            throw new ArgumentException("Map dimension cannot be 0");
        }
        Rotation = -activity.Map.Rotation * MathF.PI / 180.0;
        Width = activity.Map.Width;
        Height = activity.Map.Height;
        Corners = new MapCorners
        {
            SouthWest = new Coordinate
            {
                Latitude = activity.Map.Polygon.Vertices[0].Latitude,
                Longitude = activity.Map.Polygon.Vertices[0].Longitude,
            },
            SouthEast = new Coordinate
            {
                Latitude = activity.Map.Polygon.Vertices[1].Latitude,
                Longitude = activity.Map.Polygon.Vertices[1].Longitude,
            },
            NorthEast = new Coordinate
            {
                Latitude = activity.Map.Polygon.Vertices[2].Latitude,
                Longitude = activity.Map.Polygon.Vertices[2].Longitude,
            },
            NorthWest = new Coordinate
            {
                Latitude = activity.Map.Polygon.Vertices[3].Latitude,
                Longitude = activity.Map.Polygon.Vertices[3].Longitude,
            },
        };

        var mapHeightDegrees = Math.Sqrt(
            (Corners.NorthEast.Longitude - Corners.SouthEast.Longitude) * (Corners.NorthEast.Longitude - Corners.SouthEast.Longitude) +
            (Corners.NorthEast.Latitude - Corners.SouthEast.Latitude) * (Corners.NorthEast.Latitude - Corners.SouthEast.Latitude));

        var mapHeightDegreesCtrl = Math.Sqrt(
            (Corners.NorthWest.Longitude - Corners.SouthWest.Longitude) * (Corners.NorthWest.Longitude - Corners.SouthWest.Longitude) +
            (Corners.NorthWest.Latitude - Corners.SouthWest.Latitude) * (Corners.NorthWest.Latitude - Corners.SouthWest.Latitude));

        var mapHeightDiff = mapHeightDegrees - mapHeightDegreesCtrl;

        var mapWidthDegrees = Math.Sqrt(
            (Corners.NorthEast.Longitude - Corners.NorthWest.Longitude) * (Corners.NorthEast.Longitude - Corners.NorthWest.Longitude) +
            (Corners.NorthWest.Latitude - Corners.NorthEast.Latitude) * (Corners.NorthWest.Latitude - Corners.NorthEast.Latitude));

        var mapWidthDegreesCtrl = Math.Sqrt(
            (Corners.SouthEast.Longitude - Corners.SouthWest.Longitude) * (Corners.SouthEast.Longitude - Corners.SouthWest.Longitude) +
            (Corners.SouthWest.Latitude - Corners.SouthEast.Latitude) * (Corners.SouthWest.Latitude - Corners.SouthEast.Latitude));

        var mapWidthDiff = mapWidthDegrees - mapWidthDegreesCtrl;

        ScaleX = (double)Width * 2 / (mapWidthDegrees + mapWidthDegreesCtrl);
        ScaleY = (double)Height * 2 / (mapHeightDegrees + mapHeightDegreesCtrl);

        ProjectionOrigin = new Coordinate
        {
            Latitude = activity.Map.DefaultProjection.Origin.Latitude,
            Longitude = activity.Map.DefaultProjection.Origin.Longitude,
        };

        ProjectionMatrix = new Matrix(activity.Map.DefaultProjection.Matrix);
    }
}
