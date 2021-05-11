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

        public int NumberOfChunks { get; set; }
        public List<Chunk> Chunks { get; set; } = new();

        internal static PNG ParseToPNG(byte[] bytes)
        {
            var pngToReturn = new PNG();
            var restOfPng = bytes.Skip(8); //Skips 8 bytes png signature
                                           //Skips the first 8 bytes of a PNG file (PNG signature)
            while (restOfPng.Any())
            {
                (Chunk info, int skippedBytes) = Chunk.ReadChunk(restOfPng);
                pngToReturn.Chunks.Add(info);
                restOfPng = restOfPng.Skip(skippedBytes);
            }

            return pngToReturn.ReadIhdr();
        }

        public PNG ReadIhdr()
        {
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
