using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mime;

namespace FormatPNG
{
    internal class Plte : Chunk
    {
        public List<RGB> Rgbs { get; set; }
        public Plte(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Rgbs = new List<RGB>();
            Decode();
            SaveImage();
        }

        private void Decode()
        {
            for (int i = 0; i < Data.Length; i+=3)
            {
                Rgbs.Add(new RGB(Data[i], Data[i+1], Data[i+2]));
            }
        }

        public void SaveImage()
        {
            Bitmap bitmap = new Bitmap(10 * Rgbs.Count, 10, PixelFormat.Format32bppArgb);
            for (var i = 0; i < Rgbs.Count; i++)
            {
                var pixel = Rgbs[i].GetColor();
                for (int x = i*10; x < 10*(i+1); x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        bitmap.SetPixel(x,y, pixel);

                    }
                }
            }

            bitmap.Save("C:\\Users\\Grzesiek\\source\\repos\\FormatPNG\\" + "PLTE.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);

        }

        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine("( R , G , B )");
            Rgbs.ForEach(x => Console.WriteLine($"({x.R}, {x.G}, {x.B})"));
        }

    }

}
