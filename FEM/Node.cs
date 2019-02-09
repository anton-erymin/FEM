using System;
using System.Collections.Generic;
using System.Text;

namespace FEM
{
    public class Node
    {
        public double x;
        public double y;
        public bool added = false;
        public double dist;

        public bool boundary = false;
        public double value;

        public bool isload = false;
        public double load;


        public Node(double x, double y)
        {
            this.x = x;
            this.y = y;

            //dist = (double)Math.Sqrt(x * x + y * y);
        }
    }
}
