using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormatPNG
{
    //PNG class contains IHDR information chunk
    //IHDR is first chunk. Contains file information and always has 13 bytes.
    internal class PNG
    {
        public static byte[] Header { get; set; }
        public int NumberOfChunks { get; set; }
        public List<Chunk> Chunks { get; set; } = new();

        internal static PNG ParseToPNG(byte[] bytes)
        {
            var pngToReturn = new PNG();
            Header = bytes.Take(8).ToArray();
            var restOfPng = bytes.Skip(8); //Skips 8 bytes png signature
                                           //Skips the first 8 bytes of a PNG file (PNG signature)
            while (restOfPng.Any())
            {
                (Chunk chunk, int skippedBytes) = Chunk.ReadChunk(restOfPng);
                pngToReturn.Chunks.Add(chunk);
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

        public void DeleteNonCriticalChunks()
        {
            string[] criticalChunks = new[] { "IDAT", "IHDR", "PLTE", "IEND" };
            Chunks.RemoveAll(x => !criticalChunks.Contains(Encoding.UTF8.GetString(x.CType)));
        }

        public void WriteToFile()
        {
            using (var fs = new FileStream("C:\\Users\\Grzesiek\\source\\repos\\FormatPNG\\" + "Png.png", FileMode.Create, FileAccess.Write))
            {
                //int length = Header.Length;
                fs.Write(Header, 0, Header.Length);
                foreach (var chunk in Chunks)
                {
                    fs.Write(chunk.Length, 0, chunk.Length.Length);
                    //length += chunk.Length.Length;
                    fs.Write(chunk.CType, 0, chunk.CType.Length );
                    //length += chunk.CType.Length;
                    fs.Write(chunk.Data, 0, chunk.Data.Length);
                    //length += chunk.Data.Length;
                    fs.Write(chunk.Crc32, 0, chunk.Crc32.Length);
                    //length += chunk.Crc32.Length;
                }

            }



           // ByteArrayToFile("Png.png", bytes.ToArray());

            //Bitmap bitmap = new Bitmap();


            //bitmap.Save("C:\\Users\\Grzesiek\\source\\repos\\FormatPNG\\" + "PNG.png", System.Drawing.Imaging.ImageFormat.Png);

        }
        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream("C:\\Users\\Grzesiek\\source\\repos\\FormatPNG\\" + fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

    }
}
