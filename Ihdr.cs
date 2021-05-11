using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Ihdr: Chunk
    {
        public int Width { get; set; } // 4 bytes
        public int Height { get; set; } //4 bytes
        public int BitDepth { get; set; } //1 byte
        public string ColourType { get; set; } // 1 byte
        public int CompressionMethod { get; set; } // 1 byte
        public int FilterMethod { get; set; } // 1 byte
        public int InterlaceMethod { get; set; } // 1 byte
        public Ihdr(int length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            EncodeData();
        }

        private void EncodeData()
        {

            Width = Chunk.Convert4ByteToInt(Data.Take(4).ToArray());
            Height = Chunk.Convert4ByteToInt(Data.Skip(4).Take(4).ToArray());
            BitDepth = Data.Skip(8).Take(1).Sum(x => (int)x);
            ColourType = Data.Skip(9).Take(1).Sum(x => (int) x) switch
            {
                0 => "Greyscale",
                2 => "Truecolor",
                3 => "Indexed-colour",
                4 => "Greyscale with alpha",
                6 => "Truecolor with alpha",
                _ => throw new ArgumentOutOfRangeException("ColourType: IHDR")
            };

            ;
            CompressionMethod = Data.Skip(10).Take(1).Sum(x => (int)x);
            FilterMethod = Data.Skip(11).Take(1).Sum(x => (int)x);
            InterlaceMethod = Data.Skip(12).Take(1).Sum(x => (int)x);
        }

        public override void WriteChunk()
        {
            Console.WriteLine($"Type   : {System.Text.Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)}");
            Console.WriteLine($"Width   : {Width}");
            Console.WriteLine($"Height   : {Height}");
            Console.WriteLine($"BitDepth   : {BitDepth}");
            Console.WriteLine($"ColourType   : {ColourType}");
            Console.WriteLine($"CompressionMethod   : {CompressionMethod}");
            Console.WriteLine($"FilterMethod   : {FilterMethod}");
            Console.WriteLine($"InterlaceMethod   : {InterlaceMethod}");
        }
    }
}
