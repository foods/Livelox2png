namespace Livelox2png.entities;

public static class LinearAlgebraUtil
{
    public static PointD Transform(PointD p, Matrix transformationMatrix)
    {
        return (transformationMatrix * p.To3x1Matrix()).ToPointD();
    }

    public static PointD Rotate(PointD point, PointD rotationCenter, double angleInRadians)
    {
        var rotated =
          new Matrix(new[] { 1, 0, 0, 0, 1, 0, rotationCenter.X, rotationCenter.Y, 1 }, 3) *
          new Matrix(new[] { Math.Cos(angleInRadians), -Math.Sin(angleInRadians), 0, Math.Sin(angleInRadians), Math.Cos(angleInRadians), 0, 0, 0, 1 }, 3) *
          new Matrix(new[] { 1, 0, 0, 0, 1, 0, -rotationCenter.X, -rotationCenter.Y, 1 }, 3) *
          point.To3x1Matrix();

        return new PointD(rotated.GetElement(0, 0), rotated.GetElement(1, 0));
    }

    public static PointD Normalize(PointD vector)
    {
        if (vector.X == 0.0 && vector.Y == 0.0) return new PointD(0.0, 0.0);
        double length = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        return new PointD(vector.X / length, vector.Y / length);
    }

    public static double LineDistance(PointD p0, PointD p1)
    {
        return Math.Sqrt((p1.X - p0.X) * (p1.X - p0.X) + (p1.Y - p0.Y) * (p1.Y - p0.Y));
    }
}