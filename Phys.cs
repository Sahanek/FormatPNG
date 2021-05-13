using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Phys : Chunk
    {
        public int Xaxis { get; set; }
        public int Yaxis { get; set; }
        public string Unit { get; set; }
        public Phys(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Xaxis = Chunk.Convert4ByteToInt(data.Take(4).ToArray());
            Yaxis = Chunk.Convert4ByteToInt(data.Skip(4).Take(4).ToArray());
            DecodeUnit();
        }

        private void DecodeUnit()
        {
            Unit = (int) Data[8] switch
            {
                0 => "Unknown",
                1 => "Meter",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine($"Pixels per {Unit} : {Xaxis}x{Yaxis}");
        }
    }
}
