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

}