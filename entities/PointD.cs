namespace Livelox2png.entities;

public class PointD
{
    public double X { get; set; }
    public double Y { get; set; }

    public PointD()
    {
    }

    public PointD(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return X + ", " + Y;
    }

    public static PointD operator -(PointD p0, PointD p1)
    {
        return new PointD(p0.X - p1.X, p0.Y - p1.Y);
    }

    public static PointD operator +(PointD p0, PointD p1)
    {
        return new PointD(p0.X + p1.X, p0.Y + p1.Y);
    }

    public static PointD operator *(double t, PointD p)
    {
        return new PointD(t * p.X, t * p.Y);
    }

    public static PointD operator *(PointD p, double t)
    {
        return new PointD(t * p.X, t * p.Y);
    }

    public static PointD operator /(PointD p, double t)
    {
        return new PointD(p.X / t, p.Y / t);
    }

    public Matrix To3x1Matrix()
    {
        Matrix m = new(3, 1);
        m.SetElement(0, 0, X);
        m.SetElement(1, 0, Y);
        m.SetElement(2, 0, 1);
        return m;
    }
}