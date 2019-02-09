using System;
using System.Collections.Generic;
using System.Text;

namespace FEM
{

    public class Triangle
    {
        public int[] nodes;
        public Triangle[] nb = new Triangle[3];


        public Triangle(int v1, int v2, int v3)
        {
            nodes = new int[3];
            nodes[0] = v1;
            nodes[1] = v2;
            nodes[2] = v3;
        }


    }

}
