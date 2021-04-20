using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormatPNG
{
    //PNG class contains IHDR information chunk
    //IHDR is first chunk. Contains file information and always has 13 bytes.
    internal class PNG
    {
        public int Width { get; set; } // 4 bytes
        public int Height { get; set; } //4 bytes
        public int BitDepth { get; set; } //1 byte
        public int ColorType { get; set; } // 1 byte
        public int CompressionMethod { get; set; } // 1 byte
        public int FilterMethod { get; set; } // 1 byte
        public int InterlaceMethod { get; set; } // 1 byte
        public int NumberOfChunks { get; set; }
        public List<Chunk> Chunks { get; set; } = new();

        internal static PNG ParseToPNG(byte[] bytes)
        {
            var PNGToReturn = new PNG();
            var restOfPNG = bytes.Skip(8); //Skips 8 bytes png signature
                                           //Skips the first 8 bytes of a PNG file (PNG signature)
            while (restOfPNG.Any())
            {
                (Chunk info, int skippedBytes) = Chunk.ReadChunk(restOfPNG);
                PNGToReturn.Chunks.Add(info);
                restOfPNG = restOfPNG.Skip(skippedBytes);
            }

            return PNGToReturn.ReadIHDR();
        }

        public PNG ReadIHDR()
        {
            Chunk IHDR = this.Chunks.First();
            Width = Chunk.ConvertLengthToInt(IHDR.Data.Take(4).ToArray());
            Height = Chunk.ConvertLengthToInt(IHDR.Data.Skip(4).Take(4).ToArray());
            BitDepth = IHDR.Data.Skip(8).Take(1).Sum(x => (int)x);
            ColorType = IHDR.Data.Skip(9).Take(1).Sum(x => (int)x);
            CompressionMethod = IHDR.Data.Skip(10).Take(1).Sum(x => (int)x);
            FilterMethod = IHDR.Data.Skip(11).Take(1).Sum(x => (int)x);
            InterlaceMethod = IHDR.Data.Skip(12).Take(1).Sum(x => (int)x);
            NumberOfChunks = this.Chunks.Count;
            return this;
        }

        public void WritePNG()
        {
            var json = JsonSerializer.Serialize(this);
            var rootEl = JsonDocument.Parse(json).RootElement;
            var props = rootEl.EnumerateObject();

            while (props.MoveNext())
            {
                var prop = props.Current;
                if (!prop.Name.Equals("Chunks"))
                    Console.WriteLine($"{prop.Name}: {prop.Value}");
                else
                    foreach (var chunk in Chunks)
                    {
                        Console.WriteLine("\n");
                        chunk.WriteChunk();    
                    }
            }


        }
    }
}
