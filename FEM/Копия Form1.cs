using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Delone
{
    public partial class Form1 : Form
    {
        List<Vertex> verts;
        List<Triangle> triangles;

        float xr = 15.0f;
        float yr = 10.0f;
        int cx = 50;
        int cy = 100;
        int zoom = 20;
        int j = 3;


        public Form1()
        {
            InitializeComponent();

            
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            verts = new List<Vertex>();

            int nx = 15;
            int ny = 10;
            float dx = xr / nx;
            float dy = yr / ny;

            float x, y;
            for (int i = 0; i < nx + 1; i++)
            {
                x = i * dx;
                for (int j = 0; j < ny + 1; j++)
                {
                    y = j * dy;
                    verts.Add(new Vertex(x, y));
                }
            }



            verts.Sort(CompareVertex);

            triangles = new List<Triangle>();
            triangles.Add(new Triangle(0, 1, 2));
            verts[0].added = true;
            verts[1].added = true;
            verts[2].added = true;

            

        }


        private int CompareVertex(Vertex v1, Vertex v2)
        {
            if (v1.dist == v2.dist)
                return 0;
            else if (v1.dist > v2.dist)
                return 1;
            else return -1;
        }



        private void DrawVertices()
        {
            Graphics gr = Graphics.FromHwnd(this.Handle);

            for (int i = 0; i < verts.Count; i++)
            {
                int sx = cx + (int)(verts[i].x * (float)this.Width / zoom);
                int sy = this.Height - cy - (int)(verts[i].y * (float)this.Width / zoom);

                gr.DrawEllipse(Pens.Black, sx - 1, sy - 1, 2, 2);
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
                    sx[j] = cx + (int)(verts[triangles[i].verts[j]].x * (float)this.Width / zoom);
                    sy[j] = this.Height - cy - (int)(verts[triangles[i].verts[j]].y * (float)this.Width / zoom);
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
            AddNext();
            DrawTriangles();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Triangulate();
        }


        private void Triangulate()
        {
            // Проходимся по всем недобавленным точкам
            for (int i = 3; i < verts.Count; i++)
            {
                Vertex nextv = verts[i];
                if (nextv.added) continue;
           
                Triangle cur = triangles[0];
                // Ищем треугольник, в который попадает точка или ближайший к ней
                while (true)
                {
                    Vertex center = GetCenter(cur);

                    // Проверяем пересечение отрезка из центра текущего треугольника в новую точку со всеми его сторонами
                    bool f1 = Intersect(center, nextv, verts[cur.verts[0]], verts[cur.verts[1]]);
                    bool f2 = Intersect(center, nextv, verts[cur.verts[1]], verts[cur.verts[2]]);
                    bool f3 = Intersect(center, nextv, verts[cur.verts[0]], verts[cur.verts[2]]);

                    // Если пересечений нет
                    if ((f1 || f2 || f3) == false)
                    {
                        // Значит новая точка лежит внутри текущего треугольника
                        // Разбиваем текущий треугольник

                        Triangle t1, t2, t3;
                        t1 = new Triangle(i, cur.verts[0], cur.verts[1]);
                        t2 = new Triangle(i, cur.verts[1], cur.verts[2]);
                        t3 = new Triangle(i, cur.verts[0], cur.verts[2]);

                        t1.nb[0] = t3;
                        t1.nb[1] = cur.nb[0];
                        t1.nb[2] = t2;

                        t2.nb[0] = t1;
                        t2.nb[1] = cur.nb[1];
                        t2.nb[2] = t3;

                        t3.nb[0] = t1;
                        t3.nb[1] = cur.nb[2];
                        t3.nb[2] = t2;


                        triangles.Remove(cur);
                        triangles.Add(t1);
                        triangles.Add(t2);
                        triangles.Add(t3);

                        break;
                    }

                    // Если отрезок пересекает хоть одну из сторон
                    else
                    {
                        Triangle nextTriangle = null;
                        int side = 0;
                        if (f1)
                        {
                            nextTriangle = cur.nb[0];
                            side = 0;
                        }
                        if (f2)
                        {
                            nextTriangle = cur.nb[1];
                            side = 1;
                        }
                        if (f3)
                        {
                            nextTriangle = cur.nb[2];
                            side = 2;
                        }
                        
                        // Если соседнего треугольника нет
                        if (nextTriangle == null)
                        {
                            // то достраиваем

                            int a = 0, b = 0;
                            switch (side)
                            {
                                case 0:
                                    a = 0;
                                    b = 1;
                                    break;
                                case 1:
                                    a = 1;
                                    b = 2;
                                    break;
                                case 2:
                                    a = 0;
                                    b = 2;
                                    break;
                            }

                            Triangle newt = new Triangle(i, cur.verts[a], cur.verts[b]);
                            cur.nb[side] = newt;
                            newt.nb[1] = cur;
                            triangles.Add(newt);



                            break;
                        }
                        else cur = nextTriangle;


                    }



                }


                
            }




        }


        public float Area(Vertex v1, Vertex v2, Vertex v3)
        {
            return Math.Abs(v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y)) / 2;
        }


        public bool InTriangle(Triangle t, Vertex v)
        {
            float s = Area(v, verts[t.verts[0]], verts[t.verts[1]]) + 
                      Area(v, verts[t.verts[0]], verts[t.verts[2]]) + 
                      Area(v, verts[t.verts[1]], verts[t.verts[2]]);

            if (s <= Area(verts[t.verts[0]], verts[t.verts[1]], verts[t.verts[2]]))
                return true;
            
            return false;
        }


        public bool Intersect(Vertex a1, Vertex a2, Vertex b1, Vertex b2)
        {
            float d = (a1.x - a2.x) * (b2.y - b1.y) - (a1.y - a2.y) * (b2.x - b1.x);

            if (Math.Abs(d) < 0.00001)
                return false;


            float d1 = (a1.x - b1.x) * (b2.y - b1.y) - (a1.y - b1.y) * (b2.x - b1.x);
            float d2 = (a1.x - a2.x) * (a1.y - b1.y) - (a1.y - a2.y) * (a1.x - b1.x);
            float ta = d1 / d;
            float tb = d2 / d;

            if (ta >= 0 && ta <= 1 && tb >= 0 && tb <= 1)
                return true;

            return false;
        }


        public Vertex GetCenter(Triangle t)
        {
            return new Vertex((verts[t.verts[0]].x + verts[t.verts[1]].x + verts[t.verts[2]].x) / 3,
                              (verts[t.verts[0]].y + verts[t.verts[1]].y + verts[t.verts[2]].y) / 3);  
        }



        private void AddNext()
        {
            Vertex nextv = verts[j];
            if (nextv.added) return;

            Triangle cur = triangles[0];
            // Ищем треугольник, в который попадает точка или ближайший к ней
            while (true)
            {
                Vertex center = GetCenter(cur);

                // Проверяем пересечение отрезка из центра текущего треугольника в новую точку со всеми его сторонами
                bool f1 = Intersect(center, nextv, verts[cur.verts[0]], verts[cur.verts[1]]);
                bool f2 = Intersect(center, nextv, verts[cur.verts[1]], verts[cur.verts[2]]);
                bool f3 = Intersect(center, nextv, verts[cur.verts[0]], verts[cur.verts[2]]);

                // Если пересечений нет
                if ((f1 || f2 || f3) == false)
                {
                    // Значит новая точка лежит внутри текущего треугольника
                    // Разбиваем текущий треугольник

                    Triangle t1, t2, t3;
                    t1 = new Triangle(j, cur.verts[0], cur.verts[1]);
                    t2 = new Triangle(j, cur.verts[1], cur.verts[2]);
                    t3 = new Triangle(j, cur.verts[0], cur.verts[2]);

                    t1.nb[0] = t3;
                    t1.nb[1] = cur.nb[0];
                    t1.nb[2] = t2;

                    t2.nb[0] = t1;
                    t2.nb[1] = cur.nb[1];
                    t2.nb[2] = t3;

                    t3.nb[0] = t1;
                    t3.nb[1] = cur.nb[2];
                    t3.nb[2] = t2;


                    triangles.Remove(cur);
                    triangles.Add(t1);
                    triangles.Add(t2);
                    triangles.Add(t3);

                    break;
                }

                // Если отрезок пересекает хоть одну из сторон
                else
                {
                    Triangle nextTriangle = null;
                    int side = 0;
                    if (f1)
                    {
                        nextTriangle = cur.nb[0];
                        side = 0;
                    }
                    if (f2)
                    {
                        nextTriangle = cur.nb[1];
                        side = 1;
                    }
                    if (f3)
                    {
                        nextTriangle = cur.nb[2];
                        side = 2;
                    }

                    // Если соседнего треугольника нет
                    if (nextTriangle == null)
                    {
                        // то достраиваем

                        int a = 0, b = 0;
                        switch (side)
                        {
                            case 0:
                                a = 0;
                                b = 1;
                                break;
                            case 1:
                                a = 1;
                                b = 2;
                                break;
                            case 2:
                                a = 0;
                                b = 2;
                                break;
                        }

                        Triangle newt = new Triangle(j, cur.verts[a], cur.verts[b]);
                        cur.nb[side] = newt;
                        newt.nb[1] = cur;
                        triangles.Add(newt);


                        if (!DeloneCondition(cur, newt, side))
                            MessageBox.Show("Need to rebuild");


                        break;
                    }
                    else cur = nextTriangle;


                }



            }

            j++;

        }



        public bool DeloneCondition(Triangle t1, Triangle t2, int side)
        {
            float x0 = 0, y0 = 0;
            float x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0;
            int a = 0, b = 0;

            switch (side)
            {
                case 0:
                    a = t1.verts[0];
                    b = t1.verts[1];

                    x1 = verts[a].x;
                    y1 = verts[a].y;
                    x2 = verts[t1.verts[2]].x;
                    y2 = verts[t1.verts[2]].y;
                    x3 = verts[b].x;
                    y3 = verts[b].y;
                    
                    break;
                case 1:
                    a = t1.verts[1];
                    b = t1.verts[2];

                    x1 = verts[a].x;
                    y1 = verts[a].y;
                    x2 = verts[t1.verts[0]].x;
                    y2 = verts[t1.verts[0]].y;
                    x3 = verts[b].x;
                    y3 = verts[b].y;
                    
                    break;
                case 2:
                    a = t1.verts[0];
                    b = t1.verts[2];

                    x1 = verts[a].x;
                    y1 = verts[a].y;
                    x2 = verts[t1.verts[1]].x;
                    y2 = verts[t1.verts[1]].y;
                    x3 = verts[b].x;
                    y3 = verts[b].y;
                    break;
            }

            int i = 0;
            for (i = 0; i < 3; i++)
            {
                if (t2.verts[i] != a && t2.verts[i] != b)
                    break;
            }

            x0 = verts[t2.verts[i]].x;
            y0 = verts[t2.verts[i]].y;


            float sa = (x0 - x1) * (x0 - x3) + (y0 - y1) * (y0 - y3);
            float sb = (x2 - x1) * (x2 - x3) + (y2 - y1) * (y2 - y3);

            bool flag = false;
            if (sa < 0 && sb < 0)
                flag = false;
            else if (sa >= 0 && sb >= 0)
                flag = true;
            else
            {
                sa = ((x0 - x1) * (y0 - y3) + (x0 - x3) * (y0 - y1)) * sb + ((x2 - x1) * (y2 - y3) + (x2 - x3) * (y2 - y1)) * sa;
                if (sa >= 0)
                    flag = true;
            }



            return flag;


        }


    }
}
