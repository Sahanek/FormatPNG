using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace FormatPNG
{
    class Program
    {
        static void Main(string[] args)
        {

            //KeyGenerator keys = new KeyGenerator(32);

            var Rsa = new RSA(512);
            //BigInteger big = 2;
            //double a;

            //big.IsPrime(25, out double a);
            //Console.WriteLine(big.IsPrime(25, out double a));
            //var byteArray = File.ReadAllBytes("images7_ITXt.png");

            //var Png = PNG.ParseToPNG(byteArray);
            //Png.WritePNG();

            //Console.WriteLine("\nDelete Non critical Chunks\n");
            //Png.DeleteNonCriticalChunks();
            //Console.WriteLine("\nDeleted non critical Chunks\n");
            //Png.WritePNG();
            //Png.WriteToFile();

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
