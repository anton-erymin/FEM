using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;


namespace FEM
{
    class FiniteElement
    {
        public double a1, b1, c1;
        public double a2, b2, c2;
        public double a3, b3, c3;
        public double s;
        public double xm, ym;

        public DenseMatrix ElementMatrix(Node v1, Node v2, Node v3)
        {
            double[] x = new double[3];
            double[] y = new double[3];

            x[0] = v1.x; y[0] = v1.y;
            x[1] = v2.x; y[1] = v2.y;
            x[2] = v3.x; y[2] = v3.y;

            xm = (x[0] + x[1] + x[2]) / 3.0f;
            ym = (y[0] + y[1] + y[2]) / 3.0f;

            BasisFunction(x, y, out a1, out b1, out c1);

            x[0] = v2.x; y[0] = v2.y;
            x[1] = v1.x; y[1] = v1.y;
            x[2] = v3.x; y[2] = v3.y;

            BasisFunction(x, y, out a2, out b2, out c2);

            x[0] = v3.x; y[0] = v3.y;
            x[1] = v2.x; y[1] = v2.y;
            x[2] = v1.x; y[2] = v1.y;

            BasisFunction(x, y, out a3, out b3, out c3);


            double[,] mdet = new double[3, 3];
            mdet[0, 0] = x[0]; mdet[0, 1] = y[0]; mdet[0, 2] = 1.0f;
            mdet[1, 0] = x[1]; mdet[1, 1] = y[1]; mdet[1, 2] = 1.0f;
            mdet[2, 0] = x[2]; mdet[2, 1] = y[2]; mdet[2, 2] = 1.0f;

            DenseMatrix dm = new DenseMatrix(mdet);
            s = Math.Abs(dm.Determinant());

            mdet[0, 0] = (a1 * a1 + b1 * b1) * s; 
            mdet[0, 1] = (a1 * a2 + b1 * b2) * s;
            mdet[0, 2] = (a1 * a3 + b1 * b3) * s;

            mdet[1, 0] = mdet[0, 1];
            mdet[1, 1] = (a2 * a2 + b2 * b2) * s;
            mdet[1, 2] = (a2 * a3 + b2 * b3) * s;

            mdet[2, 0] = mdet[0, 2]; 
            mdet[2, 1] = mdet[1, 2];
            mdet[2, 2] = (a3 * a3 + b3 * b3) * s;


            dm = new DenseMatrix(mdet);

            return dm;
        }


        public DenseVector GetVector()
        {
            double[] N = new double[3];

            N[0] = (a1 * xm + b1 * ym + c1) * s;
            N[1] = (a2 * xm + b2 * ym + c2) * s;
            N[2] = (a3 * xm + b3 * ym + c3) * s;

            return new DenseVector(N);
        }



        private void BasisFunction(double[] x, double[] y, out double a, out double b, out double c)
        {
            double[,] mdet = new double[3, 3];
            mdet[0, 0] = x[0]; mdet[0, 1] = y[0]; mdet[0, 2] = 1.0f;
            mdet[1, 0] = x[1]; mdet[1, 1] = y[1]; mdet[1, 2] = 1.0f;
            mdet[2, 0] = x[2]; mdet[2, 1] = y[2]; mdet[2, 2] = 1.0f;

            DenseMatrix dm = new DenseMatrix(mdet);
            double det = dm.Determinant();

            a = (y[1] - y[2]) / det;
            b = -(x[1] - x[2]) / det;
            c = (x[1] * y[2] - x[2] * y[1]) / det;

        }

    }
}
