using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Itxt : Chunk
    {
        public string DataString { get; set; }
        public Itxt(int length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            DataString = System.Text.Encoding.UTF8.GetString(data);
        }


        public override void WriteChunk()
        {
            Console.WriteLine($"Type   : {System.Text.Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)} is {(CorrectCrc32 ? "Correct" : "Incorrect")}");
            Console.WriteLine($"Data   : {DataString}");
        }

    }
}
