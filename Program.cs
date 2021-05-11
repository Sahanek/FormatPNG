using System;
using System.IO;
using System.Linq;

namespace FormatPNG
{
    class Program
    {
        static void Main(string[] args)
        {
            var byteArray = File.ReadAllBytes("sample.png");

            var Png = PNG.ParseToPNG(byteArray);
            Png.WritePNG();

            //Console.WriteLine($"Chunk Length: {info.Length}\n Chunk Type: {info.CType}\n" +
            //$" Chunk Data: {String.Join(' ', info.Data)}\n Chunk CRC32 : {String.Join(' ', info.Crc32)}\n");
            //foreach (var data in info.Data)
            //{
            //    Console.Write($"{data} ");
            //}
            //foreach (var bytes in byteArray)
            //{
            //    Console.Write(bytes + " ");
            //}
            //Console.WriteLine($"Hello World! {byteArray.Length}");
        }
    }
}
