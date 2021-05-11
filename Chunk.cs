using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormatPNG
{
    //Chunks starts with 4 byte length of data chunks, then 4 byte of name,
    //then Data and finally CRC32 of chunk data
    internal class Chunk
    {
        public Chunk(int length, byte[] cType, byte[] data, byte[] crc32)
        {
            Length = length;
            CType = cType;
            Data = data;
            Crc32 = crc32;
        }

        public int Length { get; set; } //chunk data length
        public byte[] CType { get; set; } //chunk type
        public byte[] Data { get; set; } // chunk Data
        public byte[] Crc32 { get; set; } //CRC32 of chunk data


        internal static int Convert4ByteToInt(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            if (bytes.Length is > 4 or 0) throw new Exception("Invalid chunk length");
            int number = BitConverter.ToInt32(bytes, 0);
            return number;
        }

        internal static (Chunk, int skippedBytes) ReadChunk(IEnumerable<byte> remainingPNG)
        {
            var Length = Chunk.Convert4ByteToInt(remainingPNG.Take(4).ToArray());
            var CType = remainingPNG.Skip(4).Take(4).ToArray();
            var Data = remainingPNG.Skip(8).Take(Length).ToArray();
            var CRC32 = remainingPNG.Skip(8 + Length).Take(4).ToArray();
            //return (new Chunk(Length, CType, Data, CRC32), 12 + Length);
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            var titleCaseChunk = ti.ToTitleCase(Encoding.UTF8.GetString(CType).ToLower());
            return titleCaseChunk switch
            {
                "Ihdr" => (new Ihdr(Length, CType, Data, CRC32), 12 + Length),
                "Srgb" => (new Srgb(Length, CType, Data, CRC32), 12 + Length),
                "Gama" => (new Gama(Length, CType, Data, CRC32), 12 + Length),
                "Phys" => (new Phys(Length, CType, Data, CRC32), 12 + Length),
                "Text" => (new Text(Length, CType, Data, CRC32), 12 + Length),
                "Chrm" => (new Chrm(Length, CType, Data, CRC32), 12 + Length),
                "Time" => (new Time(Length, CType, Data, CRC32), 12 + Length),
                _ => (new Chunk(Length, CType, Data, CRC32), 12 + Length),
            };
        }
        public virtual void WriteChunk()
        {
            Console.WriteLine($"Type   : {Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)}");
            Console.WriteLine($"Data   : {String.Join(' ', Data)}");
        }
    }
}

