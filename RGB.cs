using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class RGB 
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public RGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color GetColor()
        {
            return Color.FromArgb(R, G, B);
        }
    }
}
