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
        public Phys(int length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Xaxis = Chunk.Convert4ByteToInt(data.Take(4).ToArray());
            Yaxis = Chunk.Convert4ByteToInt(data.Skip(4).Take(4).ToArray());
            EncodeUnit();
        }

        private void EncodeUnit()
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
            Console.WriteLine($"Type   : {Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)} is {(CorrectCrc32 ? "Correct" : "Incorrect")}");
            Console.WriteLine($"Pixels per {Unit} : {Xaxis}x{Yaxis}");
        }
    }
}
