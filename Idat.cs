using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Idat : Chunk
    {
        public Idat(int length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
        }

        public override void WriteChunk()
        {
            Console.WriteLine($"Type   : {System.Text.Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)} is {(CorrectCrc32 ? "Correct" : "Incorrect")}");

        }
    }
}
