using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic.Factorization;
using MathNet.Numerics.LinearAlgebra.Generic.Solvers;
using System.IO;
using System.Globalization;


namespace FEM
{
    class FEMSolver
    {
        // Глобальная матрица жесткости системы
        private DenseMatrix K;
        // Глобальный вектор нагрузок системы
        private DenseVector F;


        public void Analys(List<Node> nodes, List<Triangle> triangles, int nx, int ny)
        {
            // Собственно метод МКЭ: сборка и решение СЛАУ

            K = new DenseMatrix(nodes.Count);
            F = new DenseVector(nodes.Count);
            FiniteElement element = new FiniteElement();

            // Ансамблирование
            // Цикл по всем КЭ
            for (int k = 0; k < triangles.Count; k++)
            {
                Triangle tr = triangles[k];

                // Локальная матрица жетскости КЭ
                DenseMatrix Ke = element.ElementMatrix(nodes[tr.nodes[0]], nodes[tr.nodes[1]], nodes[tr.nodes[2]]);
                // Локальный вектор нагрузок КЭ
                DenseVector Fe = element.GetVector();

                // Добавление локальной матрицы к глобальной
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        K[tr.nodes[i], tr.nodes[j]] += Ke[i, j];
                    }

                    // Добавление локального вектора к глобальному
                    if (nodes[tr.nodes[i]].isload)
                    {
                        F[tr.nodes[i]] += nodes[tr.nodes[i]].load * Fe[i];
                    }
                }
            }


            // Внесение в систему граничных условий
            for (int i = 0; i < nodes.Count; i++)
            {
                if (!nodes[i].boundary) continue;

                for (int j = 0; j < nodes.Count; j++)
                {
                    F[j] -= nodes[i].value * K[j, i];
                    K[i, j] = 0;
                    K[j, i] = 0;
                }

                K[i, i] = 1;
                F[i] = nodes[i].value;
            }


            // Решение системы
            DenseVector x = (DenseVector)K.LU().Solve(F);


            // Вывод результатов в файл
            FileStream fs = File.Create("all.txt");
            StreamWriter sw = new StreamWriter(fs);

            for (int i = 0; i < nx + 1; i++)
            {
                for (int j = 0; j < ny + 1; j++)
                {
                    int k = i * (ny + 1) + j;
                    double err = Math.Abs(1 - x[k] / Fan(nodes[k].x, nodes[k].y));

                    String s = String.Format(CultureInfo.CreateSpecificCulture("en-US"),
                                             "{0,14:E}  {1,14:E}  {2,14:E}   {3,14:E}",
                                              nodes[k].x, nodes[k].y, x[k], err);
                    sw.WriteLine(s);

                }
                sw.WriteLine();
            }

            sw.Close();
            fs.Close();
        }


        private double Fan(double x, double y)
        {
            return x * x - y * y;
        }
    }
}
