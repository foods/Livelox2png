using SixLabors.ImageSharp;

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

    public static PointF RotateF(PointD point, PointD rotationCenter, double angleInRadians)
    {
        var rotated =
          new Matrix(new[] { 1, 0, 0, 0, 1, 0, rotationCenter.X, rotationCenter.Y, 1 }, 3) *
          new Matrix(new[] { Math.Cos(angleInRadians), -Math.Sin(angleInRadians), 0, Math.Sin(angleInRadians), Math.Cos(angleInRadians), 0, 0, 0, 1 }, 3) *
          new Matrix(new[] { 1, 0, 0, 0, 1, 0, -rotationCenter.X, -rotationCenter.Y, 1 }, 3) *
          point.To3x1Matrix();

        return new PointF((float)rotated.GetElement(0, 0), (float)rotated.GetElement(1, 0));
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

    public static double DotProduct(PointD v0, PointD v1)
    {
        return v0.X * v1.X + v0.Y * v1.Y;
    }
    /// <summary>
    /// Gets angle in radians (-PI <= a <= PI) of vector relative to x axis.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static double GetAngleR(PointD v)
    {
        PointD normalizedV = Normalize(v);
        double dp = DotProduct(normalizedV, new PointD(1, 0));
        if (dp > 1.0) dp = 1.0;
        else if (dp < -1.0) dp = -1.0;
        double angle;
        if (v.Y < 0)
            angle = 2 * Math.PI - Math.Acos(dp);
        else
            angle = Math.Acos(dp);
        if (angle > Math.PI) angle -= 2 * Math.PI;
        return angle;
    }

    /// <summary>
    /// Gets angle in radians (-PI <= a <= PI) between two vectors.
    /// </summary>
    /// <param name="v0"></param>
    /// <param name="v1"></param>
    /// <returns></returns>
    public static double GetAngleR(PointD v0, PointD v1)
    {
        double a0 = GetAngleR(v0);
        double a1 = GetAngleR(v1) + 2 * Math.PI;

        double diff = (a1 - a0) % (2 * Math.PI);
        if (diff > Math.PI) diff -= 2 * Math.PI;
        return diff;
    }
}