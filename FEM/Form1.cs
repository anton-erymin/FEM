using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace FEM
{
    public partial class Form1 : Form
    {
        List<Node> nodes;
        List<Triangle> triangles;

        double xr = 1.0f;
        double yr = 1.0f;

        int cx = 50;
        int cy = 100;
        int zoom = 2;

        int nx, ny;


        public Form1()
        {
            InitializeComponent();     
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            nodes = new List<Node>();
            triangles = new List<Triangle>();

            nx = 20;
            ny = 20;
            double dx = xr / nx;
            double dy = yr / ny;

            double x, y;
            for (int i = 0; i < nx + 1; i++)
            {
                x = i * dx;
                for (int j = 0; j < ny + 1; j++)
                {
                    y = j * dy;

                    Node v = new Node(x, y);

                    if ((i == 0 || i == nx) || (j == 0 || j == ny))
                    {
                        //v.boundary = true;

                        //if (i == 0)
                        //    v.value = 0;// -y * y;
                        //if (i == nx)
                        //    v.value = 0;// 1 - y * y;
                        //if (j == 0)
                        //    v.value = 0;// x* x;
                        //if (j == ny)
                        //    v.value = 0;//x * x - 1;


                    }

                    nodes.Add(v);                   
                }
            }



            int k = 1;
            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    int p1 = i * (ny + 1) + j;
                    int p2 = i * (ny + 1) + j + 1;
                    int p3 = (i + 1) * (ny + 1) + j + 1;
                    int p4 = (i + 1) * (ny + 1) + j;

                    triangles.Add(new Triangle(p1, p2, p3));
                    triangles.Add(new Triangle(p1, p3, p4));

                }
            }



            //nodes[((nx + 1) * (ny + 1) - 1) / 2].isload = true;
            //nodes[((nx + 1) * (ny + 1) - 1) / 2].load = 50.0f;
            //nodes[((nx + 1) * (ny + 1) - 1) / 4].isload = true;
            //nodes[((nx + 1) * (ny + 1) - 1) / 4].load = 50.0f;

        }

        private void DrawVertices()
        {
            Graphics gr = Graphics.FromHwnd(this.Handle);

            for (int i = 0; i < nodes.Count; i++)
            {
                int sx = cx + (int)(nodes[i].x * (double)this.Width / zoom);
                int sy = this.Height - cy - (int)(nodes[i].y * (double)this.Width / zoom);

                gr.DrawEllipse(Pens.Black, sx - 1, sy - 1, 2, 2);
                if (nodes[i].boundary)
                    gr.DrawEllipse(Pens.Black, sx - 3, sy - 3, 6, 6);
            }
        }


        private void DrawTriangles()
        {
            Graphics gr = Graphics.FromHwnd(this.Handle);

            for (int i = 0; i < triangles.Count; i++)
            {
                int[] sx = new int[3];
                int[] sy = new int[3];

                for (int j = 0; j < 3; j++)
                {
                    sx[j] = cx + (int)(nodes[triangles[i].nodes[j]].x * (double)this.Width / zoom);
                    sy[j] = this.Height - cy - (int)(nodes[triangles[i].nodes[j]].y * (double)this.Width / zoom);
                }


                gr.DrawLine(Pens.Green, sx[0], sy[0], sx[1], sy[1]);
                gr.DrawLine(Pens.Green, sx[0], sy[0], sx[2], sy[2]);
                gr.DrawLine(Pens.Green, sx[2], sy[2], sx[1], sy[1]);
                
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DrawVertices();
            DrawTriangles();
        }


        private void button2_Click(object sender, EventArgs e)
        {        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FEMSolver solver = new FEMSolver();
            solver.Analys(nodes, triangles, nx, ny);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawVertices();
            DrawTriangles();
        }


    }
}