using System.Runtime.Serialization;

namespace Livelox2png.entities;

#region Internal Maths utility
internal class Maths
{
    /// <summary>
    ///  sqrt(a^2 + b^2) without under/overflow.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>

    public static double Hypot(double a, double b)
    {
        double r;
        if (Math.Abs(a) > Math.Abs(b))
        {
            r = b / a;
            r = Math.Abs(a) * Math.Sqrt(1 + r * r);
        }
        else if (b != 0)
        {
            r = a / b;
            r = Math.Abs(b) * Math.Sqrt(1 + r * r);
        }
        else
        {
            r = 0.0;
        }
        return r;
    }
}
#endregion   // Internal Maths utility

/// <summary>.NET GeneralMatrix class.
/// 
/// The .NET GeneralMatrix Class provides the fundamental operations of numerical
/// linear algebra.  Various constructors create Matrices from two dimensional
/// arrays of double precision floating point numbers.  Various "gets" and
/// "sets" provide access to submatrices and matrix elements.  Several methods 
/// implement basic matrix arithmetic, including matrix addition and
/// multiplication, matrix norms, and element-by-element array operations.
/// Methods for reading and printing matrices are also included.  All the
/// operations in this version of the GeneralMatrix Class involve real matrices.
/// Complex matrices may be handled in a future version.
/// 
/// Five fundamental matrix decompositions, which consist of pairs or triples
/// of matrices, permutation vectors, and the like, produce results in five
/// decomposition classes.  These decompositions are accessed by the GeneralMatrix
/// class to compute solutions of simultaneous linear equations, determinants,
/// inverses and other matrix functions.  The five decompositions are:
/// <P><UL>
/// <LI>Cholesky Decomposition of symmetric, positive definite matrices.
/// <LI>LU Decomposition of rectangular matrices.
/// <LI>QR Decomposition of rectangular matrices.
/// <LI>Singular Value Decomposition of rectangular matrices.
/// <LI>Eigenvalue Decomposition of both symmetric and nonsymmetric square matrices.
/// </UL>
/// <DL>
/// <DT><B>Example of use:</B></DT>
/// <P>
/// <DD>Solve a linear system A x = b and compute the residual norm, ||b - A x||.
/// <P><PRE>
/// double[][] vals = {{1.,2.,3},{4.,5.,6.},{7.,8.,10.}};
/// GeneralMatrix A = new GeneralMatrix(vals);
/// GeneralMatrix b = GeneralMatrix.Random(3,1);
/// GeneralMatrix x = A.Solve(b);
/// GeneralMatrix r = A.Multiply(x).Subtract(b);
/// double rnorm = r.NormInf();
/// </PRE></DD>
/// </DL>
/// </summary>
/// <author>  
/// The MathWorks, Inc. and the National Institute of Standards and Technology.
/// </author>
/// <version>  5 August 1998
/// </version>

[Serializable]
public class Matrix : System.ICloneable, System.Runtime.Serialization.ISerializable, System.IDisposable
{
    #region Class variables

    /// <summary>Array for internal storage of elements.
    /// @serial internal array storage.
    /// </summary>
    private double[][] a;

    /// <summary>Row and column dimensions.
    /// @serial row dimension.
    /// @serial column dimension.
    /// </summary>
    private int m, n;

    #endregion //  Class variables

    #region Constructors

    /// <summary>Construct an m-by-n matrix of zeros. </summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    public Matrix(int m, int n)
    {
        this.m = m;
        this.n = n;
        a = new double[m][];
        for (int i = 0; i < m; i++)
        {
            a[i] = new double[n];
        }
    }

    /// <summary>Construct an m-by-n constant matrix.</summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    /// <param name="s">   Fill the matrix with this scalar value.
    /// </param>
    public Matrix(int m, int n, double s)
    {
        this.m = m;
        this.n = n;
        a = new double[m][];
        for (int i = 0; i < m; i++)
        {
            a[i] = new double[n];
        }
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = s;
            }
        }
    }

    /// <summary>Construct a matrix from a 2-D array.</summary>
    /// <param name="A">   Two-dimensional array of doubles.
    /// </param>
    /// <exception cref="System.ArgumentException">   All rows must have the same length
    /// </exception>
    /// <seealso cref="Create">
    /// </seealso>
    public Matrix(double[][] A)
    {
        m = A.Length;
        n = A[0].Length;
        for (int i = 0; i < m; i++)
        {
            if (A[i].Length != n)
            {
                throw new System.ArgumentException("All rows must have the same length.");
            }
        }
        this.a = A;
    }

    /// <summary>Construct a matrix quickly without checking arguments.</summary>
    /// <param name="A">   Two-dimensional array of doubles.
    /// </param>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    public Matrix(double[][] A, int m, int n)
    {
        this.a = A;
        this.m = m;
        this.n = n;
    }

    /// <summary>Construct a matrix from a one-dimensional packed array</summary>
    /// <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).
    /// </param>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <exception cref="System.ArgumentException">   Array length must be a multiple of m.
    /// </exception>
    public Matrix(double[] vals, int m)
    {
        this.m = m;
        n = (m != 0 ? vals.Length / m : 0);
        if (m * n != vals.Length)
        {
            throw new System.ArgumentException("Array length must be a multiple of m.");
        }
        a = new double[m][];
        for (int i = 0; i < m; i++)
        {
            a[i] = new double[n];
        }
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = vals[i + j * m];
            }
        }
    }

    /// <summary>
    /// Deserialization consructor.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>

    #endregion //  Constructors

    #region Public Properties
    /// <summary>Access the internal two-dimensional array.</summary>
    /// <returns>     Pointer to the two-dimensional array of matrix elements.
    /// </returns>
    virtual public double[][] Array
    {
        get
        {
            return a;
        }
    }
    /// <summary>Copy the internal two-dimensional array.</summary>
    /// <returns>     Two-dimensional array copy of matrix elements.
    /// </returns>
    virtual public double[][] ArrayCopy
    {
        get
        {
            double[][] C = new double[m][];
            for (int i = 0; i < m; i++)
            {
                C[i] = new double[n];
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i][j] = a[i][j];
                }
            }
            return C;
        }

    }
    /// <summary>Make a one-dimensional column packed copy of the internal array.</summary>
    /// <returns>     Matrix elements packed in a one-dimensional array by columns.
    /// </returns>
    virtual public double[] ColumnPackedCopy
    {
        get
        {
            double[] vals = new double[m * n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vals[i + j * m] = a[i][j];
                }
            }
            return vals;
        }

    }

    /// <summary>Make a one-dimensional row packed copy of the internal array.</summary>
    /// <returns>     Matrix elements packed in a one-dimensional array by rows.
    /// </returns>
    virtual public double[] RowPackedCopy
    {
        get
        {
            double[] vals = new double[m * n];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    vals[i * n + j] = a[i][j];
                }
            }
            return vals;
        }
    }

    /// <summary>Get row dimension.</summary>
    /// <returns>     m, the number of rows.
    /// </returns>
    virtual public int RowDimension
    {
        get
        {
            return m;
        }
    }

    /// <summary>Get column dimension.</summary>
    /// <returns>     n, the number of columns.
    /// </returns>
    virtual public int ColumnDimension
    {
        get
        {
            return n;
        }
    }
    #endregion   // Public Properties

    #region	 Public Methods

    /// <summary>Construct a matrix from a copy of a 2-D array.</summary>
    /// <param name="A">   Two-dimensional array of doubles.
    /// </param>
    /// <exception cref="System.ArgumentException">   All rows must have the same length
    /// </exception>

    public static Matrix Create(double[][] A)
    {
        int m = A.Length;
        int n = A[0].Length;
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            if (A[i].Length != n)
            {
                throw new System.ArgumentException("All rows must have the same length.");
            }
            for (int j = 0; j < n; j++)
            {
                C[i][j] = A[i][j];
            }
        }
        return X;
    }

    /// <summary>Make a deep copy of a matrix</summary>

    public virtual Matrix Copy()
    {
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = a[i][j];
            }
        }
        return X;
    }

    /// <summary>Get a single element.</summary>
    /// <param name="i">   Row index.
    /// </param>
    /// <param name="j">   Column index.
    /// </param>
    /// <returns>     A(i,j)
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">  
    /// </exception>

    public virtual double GetElement(int i, int j)
    {
        return a[i][j];
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <returns>     A(i0:i1,j0:j1)
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual Matrix GetMatrix(int i0, int i1, int j0, int j1)
    {
        Matrix X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
        double[][] B = X.Array;
        try
        {
            for (int i = i0; i <= i1; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    B[i - i0][j - j0] = a[i][j];
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
        return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <returns>     A(r(:),c(:))
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual Matrix GetMatrix(int[] r, int[] c)
    {
        Matrix X = new Matrix(r.Length, c.Length);
        double[][] B = X.Array;
        try
        {
            for (int i = 0; i < r.Length; i++)
            {
                for (int j = 0; j < c.Length; j++)
                {
                    B[i][j] = a[r[i]][c[j]];
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
        return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <returns>     A(i0:i1,c(:))
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual Matrix GetMatrix(int i0, int i1, int[] c)
    {
        Matrix X = new Matrix(i1 - i0 + 1, c.Length);
        double[][] B = X.Array;
        try
        {
            for (int i = i0; i <= i1; i++)
            {
                for (int j = 0; j < c.Length; j++)
                {
                    B[i - i0][j] = a[i][c[j]];
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
        return X;
    }

    /// <summary>Get a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <returns>     A(r(:),j0:j1)
    /// </returns>
    /// <exception cref="System.IndexOutOfRangeException">   Submatrix indices
    /// </exception>

    public virtual Matrix GetMatrix(int[] r, int j0, int j1)
    {
        Matrix X = new Matrix(r.Length, j1 - j0 + 1);
        double[][] B = X.Array;
        try
        {
            for (int i = 0; i < r.Length; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    B[i][j - j0] = a[r[i]][j];
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
        return X;
    }

    /// <summary>Set a single element.</summary>
    /// <param name="i">   Row index.
    /// </param>
    /// <param name="j">   Column index.
    /// </param>
    /// <param name="s">   A(i,j).
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  
    /// </exception>

    public virtual void SetElement(int i, int j, double s)
    {
        a[i][j] = s;
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <param name="X">   A(i0:i1,j0:j1)
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int i0, int i1, int j0, int j1, Matrix X)
    {
        try
        {
            for (int i = i0; i <= i1; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    a[i][j] = X.GetElement(i - i0, j - j0);
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <param name="X">   A(r(:),c(:))
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int[] r, int[] c, Matrix X)
    {
        try
        {
            for (int i = 0; i < r.Length; i++)
            {
                for (int j = 0; j < c.Length; j++)
                {
                    a[r[i]][c[j]] = X.GetElement(i, j);
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="r">   Array of row indices.
    /// </param>
    /// <param name="j0">  Initial column index
    /// </param>
    /// <param name="j1">  Final column index
    /// </param>
    /// <param name="X">   A(r(:),j0:j1)
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int[] r, int j0, int j1, Matrix X)
    {
        try
        {
            for (int i = 0; i < r.Length; i++)
            {
                for (int j = j0; j <= j1; j++)
                {
                    a[r[i]][j] = X.GetElement(i, j - j0);
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
    }

    /// <summary>Set a submatrix.</summary>
    /// <param name="i0">  Initial row index
    /// </param>
    /// <param name="i1">  Final row index
    /// </param>
    /// <param name="c">   Array of column indices.
    /// </param>
    /// <param name="X">   A(i0:i1,c(:))
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
    /// </exception>

    public virtual void SetMatrix(int i0, int i1, int[] c, Matrix X)
    {
        try
        {
            for (int i = i0; i <= i1; i++)
            {
                for (int j = 0; j < c.Length; j++)
                {
                    a[i][c[j]] = X.GetElement(i - i0, j);
                }
            }
        }
        catch (System.IndexOutOfRangeException e)
        {
            throw new System.IndexOutOfRangeException("Submatrix indices", e);
        }
    }

    /// <summary>Matrix transpose.</summary>
    /// <returns>    A'
    /// </returns>

    public virtual Matrix Transpose()
    {
        Matrix X = new Matrix(n, m);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[j][i] = a[i][j];
            }
        }
        return X;
    }

    /// <summary>One norm</summary>
    /// <returns>    maximum column sum.
    /// </returns>

    public virtual double Norm1()
    {
        double f = 0;
        for (int j = 0; j < n; j++)
        {
            double s = 0;
            for (int i = 0; i < m; i++)
            {
                s += System.Math.Abs(a[i][j]);
            }
            f = System.Math.Max(f, s);
        }
        return f;
    }


    /// <summary>Infinity norm</summary>
    /// <returns>    maximum row sum.
    /// </returns>

    public virtual double NormInf()
    {
        double f = 0;
        for (int i = 0; i < m; i++)
        {
            double s = 0;
            for (int j = 0; j < n; j++)
            {
                s += System.Math.Abs(a[i][j]);
            }
            f = System.Math.Max(f, s);
        }
        return f;
    }

    /// <summary>Frobenius norm</summary>
    /// <returns>    sqrt of sum of squares of all elements.
    /// </returns>

    public virtual double NormF()
    {
        double f = 0;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                f = Maths.Hypot(f, a[i][j]);
            }
        }
        return f;
    }

    /// <summary>Unary minus</summary>
    /// <returns>    -A
    /// </returns>

    public virtual Matrix UnaryMinus()
    {
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = -a[i][j];
            }
        }
        return X;
    }

    /// <summary>C = A + B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A + B
    /// </returns>

    public virtual Matrix Add(Matrix B)
    {
        CheckMatrixDimensions(B);
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = a[i][j] + B.a[i][j];
            }
        }
        return X;
    }

    /// <summary>A = A + B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A + B
    /// </returns>

    public virtual Matrix AddEquals(Matrix B)
    {
        CheckMatrixDimensions(B);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = a[i][j] + B.a[i][j];
            }
        }
        return this;
    }

    /// <summary>C = A - B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A - B
    /// </returns>

    public virtual Matrix Subtract(Matrix B)
    {
        CheckMatrixDimensions(B);
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = a[i][j] - B.a[i][j];
            }
        }
        return X;
    }

    /// <summary>A = A - B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A - B
    /// </returns>

    public virtual Matrix SubtractEquals(Matrix B)
    {
        CheckMatrixDimensions(B);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = a[i][j] - B.a[i][j];
            }
        }
        return this;
    }

    /// <summary>Element-by-element multiplication, C = A.*B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.*B
    /// </returns>

    public virtual Matrix ArrayMultiply(Matrix B)
    {
        CheckMatrixDimensions(B);
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = a[i][j] * B.a[i][j];
            }
        }
        return X;
    }

    /// <summary>Element-by-element multiplication in place, A = A.*B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.*B
    /// </returns>

    public virtual Matrix ArrayMultiplyEquals(Matrix B)
    {
        CheckMatrixDimensions(B);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = a[i][j] * B.a[i][j];
            }
        }
        return this;
    }

    /// <summary>Element-by-element right division, C = A./B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A./B
    /// </returns>

    public virtual Matrix ArrayRightDivide(Matrix B)
    {
        CheckMatrixDimensions(B);
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = a[i][j] / B.a[i][j];
            }
        }
        return X;
    }

    /// <summary>Element-by-element right division in place, A = A./B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A./B
    /// </returns>

    public virtual Matrix ArrayRightDivideEquals(Matrix B)
    {
        CheckMatrixDimensions(B);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = a[i][j] / B.a[i][j];
            }
        }
        return this;
    }

    /// <summary>Element-by-element left division, C = A.\B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.\B
    /// </returns>

    public virtual Matrix ArrayLeftDivide(Matrix B)
    {
        CheckMatrixDimensions(B);
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = B.a[i][j] / a[i][j];
            }
        }
        return X;
    }

    /// <summary>Element-by-element left division in place, A = A.\B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     A.\B
    /// </returns>

    public virtual Matrix ArrayLeftDivideEquals(Matrix B)
    {
        CheckMatrixDimensions(B);
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = B.a[i][j] / a[i][j];
            }
        }
        return this;
    }

    /// <summary>Multiply a matrix by a scalar, C = s*A</summary>
    /// <param name="s">   scalar
    /// </param>
    /// <returns>     s*A
    /// </returns>

    public virtual Matrix Multiply(double s)
    {
        Matrix X = new Matrix(m, n);
        double[][] C = X.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                C[i][j] = s * a[i][j];
            }
        }
        return X;
    }

    /// <summary>Multiply a matrix by a scalar in place, A = s*A</summary>
    /// <param name="s">   scalar
    /// </param>
    /// <returns>     replace A by s*A
    /// </returns>

    public virtual Matrix MultiplyEquals(double s)
    {
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                a[i][j] = s * a[i][j];
            }
        }
        return this;
    }

    /// <summary>Linear algebraic matrix multiplication, A * B</summary>
    /// <param name="B">   another matrix
    /// </param>
    /// <returns>     Matrix product, A * B
    /// </returns>
    /// <exception cref="System.ArgumentException">  Matrix inner dimensions must agree.
    /// </exception>

    public virtual Matrix Multiply(Matrix B)
    {
        if (B.m != n)
        {
            throw new System.ArgumentException("GeneralMatrix inner dimensions must agree.");
        }
        Matrix X = new Matrix(m, B.n);
        double[][] C = X.Array;
        double[] Bcolj = new double[n];
        for (int j = 0; j < B.n; j++)
        {
            for (int k = 0; k < n; k++)
            {
                Bcolj[k] = B.a[k][j];
            }
            for (int i = 0; i < m; i++)
            {
                double[] Arowi = a[i];
                double s = 0;
                for (int k = 0; k < n; k++)
                {
                    s += Arowi[k] * Bcolj[k];
                }
                C[i][j] = s;
            }
        }
        return X;
    }

    #region Operator Overloading

    /// <summary>
    ///  Addition of matrices
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static Matrix operator +(Matrix m1, Matrix m2)
    {
        return m1.Add(m2);
    }

    /// <summary>
    /// Subtraction of matrices
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static Matrix operator -(Matrix m1, Matrix m2)
    {
        return m1.Subtract(m2);
    }

    /// <summary>
    /// Multiplication of matrices
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    /// <returns></returns>
    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        return m1.Multiply(m2);
    }

    #endregion   //Operator Overloading

    public virtual double Trace()
    {
        double t = 0;
        for (int i = 0; i < System.Math.Min(m, n); i++)
        {
            t += a[i][i];
        }
        return t;
    }

    /// <summary>Generate matrix with random elements</summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    /// <returns>     An m-by-n matrix with uniformly distributed random elements.
    /// </returns>

    public static Matrix Random(int m, int n)
    {
        System.Random random = new System.Random();

        Matrix A = new Matrix(m, n);
        double[][] X = A.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                X[i][j] = random.NextDouble();
            }
        }
        return A;
    }

    /// <summary>Generate identity matrix</summary>
    /// <param name="m">   Number of rows.
    /// </param>
    /// <param name="n">   Number of colums.
    /// </param>
    /// <returns>     An m-by-n matrix with ones on the diagonal and zeros elsewhere.
    /// </returns>

    public static Matrix Identity(int m, int n)
    {
        Matrix A = new Matrix(m, n);
        double[][] X = A.Array;
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                X[i][j] = (i == j ? 1.0 : 0.0);
            }
        }
        return A;
    }

    #endregion //  Public Methods

    #region	 Private Methods

    /// <summary>Check if size(A) == size(B) *</summary>

    private void CheckMatrixDimensions(Matrix B)
    {
        if (B.m != m || B.n != n)
        {
            throw new System.ArgumentException("GeneralMatrix dimensions must agree.");
        }
    }
    #endregion //  Private Methods

    #region Implement IDisposable
    /// <summary>
    /// Do not make this method virtual.
    /// A derived class should not be able to override this method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Dispose(bool disposing) executes in two distinct scenarios.
    /// If disposing equals true, the method has been called directly
    /// or indirectly by a user's code. Managed and unmanaged resources
    /// can be disposed.
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed.
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
        // This object will be cleaned up by the Dispose method.
        // Therefore, you should call GC.SupressFinalize to
        // take this object off the finalization queue 
        // and prevent finalization code for this object
        // from executing a second time.
        if (disposing)
            GC.SuppressFinalize(this);
    }

    /// <summary>
    /// This destructor will run only if the Dispose method 
    /// does not get called.
    /// It gives your base class the opportunity to finalize.
    /// Do not provide destructors in types derived from this class.
    /// </summary>
    ~Matrix()
    {
        // Do not re-create Dispose clean-up code here.
        // Calling Dispose(false) is optimal in terms of
        // readability and maintainability.
        Dispose(false);
    }
    #endregion //  Implement IDisposable

    /// <summary>Clone the GeneralMatrix object.</summary>
    public System.Object Clone()
    {
        return this.Copy();
    }

    /// <summary>
    /// A method called when serializing this class
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("m", m);
        info.AddValue("n", n);
        info.AddValue("a", a);
    }

    public PointD ToPointD()
    {
        if (m != 3 || n != 1)
        {
            throw new Exception("Method only valid for 3x1 matrices");
        }
        return new PointD(GetElement(0, 0), GetElement(1, 0)); 
    }
}

