using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Idat : Chunk
    {
        public Idat(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
        }

        public override void WriteChunk()
        {
            base.WriteChunk();
        }
    }
}
