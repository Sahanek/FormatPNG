using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Gama : Chunk
    {
        public int Gamma { get; set; }
        public Gama(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Gamma = Chunk.Convert4ByteToInt(data);
        }

        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine($"Gamma : {Gamma/100000.0}");
        }

    }
}
