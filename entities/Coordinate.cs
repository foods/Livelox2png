using Livelox2png.util;

namespace Livelox2png.entities;

public class Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public override string ToString()
    {
        double lon = Math.Abs(Longitude);
        double lat = Math.Abs(Latitude);
        string secondFormat = "00";
        return
          Math.Floor(lat) + "° " +
          Math.Floor(60 * (lat - Math.Floor(lat))).ToString("00") + "' " +
          Math.Floor((3600 * (lat - Math.Floor(lat))) % 60).ToString(secondFormat) + "\" " +
          (Latitude < 0 ? "S" : "N") + "   " +
          Math.Floor(lon) + "° " +
          Math.Floor(60 * (lon - Math.Floor(lon))).ToString("00") + "' " +
          Math.Floor((3600 * (lon - Math.Floor(lon))) % 60).ToString(secondFormat) + "\" " +
          (Longitude < 0 ? "W" : "E");
    }

    /// <summary>
    /// Applies an ortographic projection to the coordinate.
    /// http://en.wikipedia.org/wiki/Orthographic_projection_%28cartography%29
    /// </summary>
    /// <param name="projectionOrigin">The origin longitude/latitude coordinate of the projection</param>
    /// <returns>A point that gives the distance in meters from the projection origin</returns>
    public PointD Project(Coordinate projectionOrigin)
    {
        const double rho = 6378200; // earth radius in metres
        double lambda0 = projectionOrigin.Longitude.ToRadians();
        double phi0 = projectionOrigin.Latitude.ToRadians();

        double lambda = Longitude.ToRadians();
        double phi = Latitude.ToRadians();
        return new PointD(rho * Math.Cos(phi) * Math.Sin(lambda - lambda0),
                          rho * (Math.Cos(phi0) * Math.Sin(phi) - Math.Sin(phi0) * Math.Cos(phi) * Math.Cos(lambda - lambda0)));
    }
    public Matrix To3x1Matrix()
    {
        Matrix m = new Matrix(3, 1);
        m.SetElement(0, 0, Longitude);
        m.SetElement(1, 0, Latitude);
        m.SetElement(2, 0, 1);
        return m;
    }
}
