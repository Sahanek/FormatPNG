using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal static class  PngReader
    {
        internal static void Read(string fileName)
        {
            var byteArray = File.ReadAllBytes("images7_ITXt.png");

            var PNG = new PNG();

            var byteLength = byteArray.Take(4);
        }
    }
}
