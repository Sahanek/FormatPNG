using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Z = 1 - x - y;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
