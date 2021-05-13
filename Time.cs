using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Time : Chunk
    {
        public DateTime DateTime { get; set; }


        public Time(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Decode();
        }

        private void Decode()
        {
            DateTime = new DateTime(
                year: BitConverter.ToUInt16(Data.Take(2).Reverse().ToArray(), 0),//Big Endian
                month: Data.Skip(2).Take(1).Sum(x => (int) x),
                day: Data.Skip(3).Take(1).Sum(x => (int) x),
                hour: Data.Skip(4).Take(1).Sum(x => (int) x),
                minute: Data.Skip(5).Take(1).Sum(x => (int) x),
                second: Data.Skip(6).Take(1).Sum(x => (int) x));
        }

        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine($"Time   : {DateTime.ToLongDateString()}, {DateTime.ToLongTimeString()}");
        }

    }
}
