using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Chrm : Chunk
    {
        private Point WhitePoint { get; set; }
        private Point Red { get; set; }
        private Point Green { get; set; }
        private Point Blue { get; set; }
        public Chrm(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Decode();
        }

        private void Decode()
        {
            WhitePoint = new Point(Chunk.Convert4ByteToInt(Data.Take(4).ToArray())/100000.0,
                Chunk.Convert4ByteToInt(Data.Skip(4).Take(4).ToArray())/ 100000.0);
            Red = new Point(Chunk.Convert4ByteToInt(Data.Skip(8).Take(4).ToArray()) / 100000.0,
                Chunk.Convert4ByteToInt(Data.Skip(12).Take(4).ToArray()) / 100000.0);
            Green = new Point(Chunk.Convert4ByteToInt(Data.Skip(16).Take(4).ToArray()) / 100000.0,
                Chunk.Convert4ByteToInt(Data.Skip(20).Take(4).ToArray()) / 100000.0);
            Blue = new Point(Chunk.Convert4ByteToInt(Data.Skip(24).Take(4).ToArray()) / 100000.0,
                Chunk.Convert4ByteToInt(Data.Skip(28).Take(4).ToArray()) / 100000.0);
        }
        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine($"WhitePoint   : {WhitePoint}");
            Console.WriteLine($"Red   : {Red}");
            Console.WriteLine($"Green   : {Green}");
            Console.WriteLine($"Blue   : {Blue}");
        }
    }
}
