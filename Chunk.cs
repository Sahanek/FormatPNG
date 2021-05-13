using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Force.Crc32;

namespace FormatPNG
{
    //Chunks starts with 4 byte length of data chunks, then 4 byte of name,
    //then Data and finally CRC32 of chunk data
    internal class Chunk
    {
        public Chunk(byte[] length, byte[] cType, byte[] data, byte[] crc32)
        {
            Length = length;
            CType = cType;
            Data = data;
            Crc32 = crc32;
            (CorrectCrc32, Crc32) = CheckCrc32();
        }

        public byte[] Length { get; set; } //chunk data length
        public byte[] CType { get; set; } //chunk type
        public byte[] Data { get; set; } // chunk Data
        public byte[] Crc32 { get; set; } //CRC32 of chunk data
        public bool CorrectCrc32 { get; set; }

        private (bool, byte[]) CheckCrc32()
        {
            var inputArray = new byte[CType.Length + Data.Length];
            Array.Copy(CType, inputArray, CType.Length);
            Array.Copy(Data, 0, inputArray, CType.Length, Data.Length);
            var crc32 = Crc32Algorithm.Compute(inputArray);

            return (crc32 == Chunk.Convert4ByteToUInt(Crc32), BitConverter.GetBytes(crc32).Reverse().ToArray()); 
        }
        internal static int Convert4ByteToInt(byte[] bytes)
        {
            var copyArray = bytes.ToArray();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(copyArray);
            if (bytes.Length is > 4 or 0) throw new Exception("Invalid chunk length");
            var number = BitConverter.ToInt32(copyArray, 0);
            return number;
        }
        internal static uint Convert4ByteToUInt(byte[] bytes)
        {
            var copyArray = bytes.ToArray();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(copyArray);
            if (bytes.Length is > 4 or 0) throw new Exception("Invalid chunk length");
            var number = BitConverter.ToUInt32(copyArray, 0);
            return number;
        }

        internal static (Chunk, int skippedBytes) ReadChunk(IEnumerable<byte> remainingPNG)
        {
            var Length = remainingPNG.Take(4).ToArray();
            var LengthInt = Chunk.Convert4ByteToInt(Length);
            var CType = remainingPNG.Skip(4).Take(4).ToArray();
            var Data = remainingPNG.Skip(8).Take(LengthInt).ToArray();
            var CRC32 = remainingPNG.Skip(8 + LengthInt).Take(4).ToArray();
            //return (new Chunk(Length, CType, Data, CRC32), 12 + Length);
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            var titleCaseChunk = ti.ToTitleCase(Encoding.UTF8.GetString(CType).ToLower());
            return titleCaseChunk switch
            {
                "Ihdr" => (new Ihdr(Length, CType, Data, CRC32), 12 + LengthInt),
                "Srgb" => (new Srgb(Length, CType, Data, CRC32), 12 + LengthInt),
                "Gama" => (new Gama(Length, CType, Data, CRC32), 12 + LengthInt),
                "Phys" => (new Phys(Length, CType, Data, CRC32), 12 + LengthInt),
                "Text" => (new Text(Length, CType, Data, CRC32), 12 + LengthInt),
                "Chrm" => (new Chrm(Length, CType, Data, CRC32), 12 + LengthInt),
                "Time" => (new Time(Length, CType, Data, CRC32), 12 + LengthInt),
                "Plte" => (new Plte(Length, CType, Data, CRC32), 12 + LengthInt),
                "Itxt" => (new Itxt(Length, CType, Data, CRC32), 12 + LengthInt),
                "Idat" => (new Idat(Length, CType, Data, CRC32), 12 + LengthInt),
                "Ztxt" => (new Ztxt(Length, CType, Data, CRC32), 12 + LengthInt),
                _ => (new UnknownChunk(Length, CType, Data, CRC32), 12 + LengthInt),
            };
        }
        public virtual void WriteChunk()
        {
            Console.WriteLine($"Type   : {Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Chunk.Convert4ByteToUInt(Length)}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)} is {(CorrectCrc32 ? "Correct" : "Incorrect")}");
        }
    }
}

