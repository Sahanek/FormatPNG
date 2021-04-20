using System;
using System.Collections.Generic;
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
        public Chunk(int length, string cType, byte[] data, byte[] crc32)
        {
            Length = length;
            CType = cType;
            Data = data;
            Crc32 = crc32;
        }

        public int Length { get; set; } //chunk data length
        public string CType { get; set; } //chunk type
        public byte[] Data { get; set; } // chunk Data
        public byte[] Crc32 { get; set; } //CRC32 of chunk data


        internal static int ConvertLengthToInt(byte[] lengthBytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(lengthBytes);
            if (lengthBytes.Length > 4 || lengthBytes.Length == 0) throw new Exception("Invalid chunk length");
            int number = BitConverter.ToInt32(lengthBytes, 0);
            return number;
        }

        internal static (Chunk, int skippedBytes) ReadChunk(IEnumerable<byte> remainingPNG)
        {
            var Length = Chunk.ConvertLengthToInt(remainingPNG.Take(4).ToArray());
            var CType = System.Text.Encoding.UTF8.GetString(remainingPNG.Skip(4).Take(4).ToArray());
            var Data = remainingPNG.Skip(8).Take(Length).ToArray();
            var CRC32 = remainingPNG.Skip(8 + Length).Take(4).ToArray();

            return (new Chunk(Length, CType, Data, CRC32), 12 + Length);
        }
        public void WriteChunk()
        {
            Console.WriteLine($"Type   : {CType}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)}");
            Console.WriteLine($"Data   : {String.Join(' ', Data)}");
        }
    }
}

