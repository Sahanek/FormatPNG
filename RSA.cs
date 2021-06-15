using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class RSA
    {
        public (BigInteger E, BigInteger N) PublicKey { get; set; }

        public (BigInteger D, BigInteger N) PrivateKey { get; set; }
        public int KeySize { get; set; }
        public int ChunkByteLength { get; set; }
        public int EncryptedChunkByteLength { get; set; }
        public List<byte> OriginalIdat { get; set; }
        public List<byte> EncryptedIdat { get; set; }
        public PNG Png { get; set; }
        public BigInteger IV { get; set; }

        public RSA(int keySize)
        {
            KeyGenerator keys = new KeyGenerator(keySize);
            KeySize = keySize;
            PublicKey = (keys.E, keys.N);
            PrivateKey = (keys.D, keys.N);
            //PublicKey = (2273875819, 635096947523);
            //PrivateKey = (3298349659, 635096947523);
            ChunkByteLength = keySize / 8 - 1;
            EncryptedChunkByteLength = keySize / 8;
            var byteArray = File.ReadAllBytes("sample.png");
            Png = PNG.ParseToPNG(byteArray);
            Png.WritePNG();
            Console.WriteLine("##########################################3");
            EncryptCBCPng();
            Png.Rewrite();
            Png.WriteToFile("EncryptedCBC.png");

            var byteArrayEncrypted = File.ReadAllBytes("EncryptedCBC.png");
            Png = PNG.ParseToPNG(byteArrayEncrypted);
            DecryptCBCPng();
            Png.WritePNG();
            Console.WriteLine("After Decrypt");
            Png.WriteToFile("DecryptedCBC.png");


        }

        public void EncryptPng()
        {
            var EncryptedBytes = new List<byte>();
            var IdatChunks = Png.Chunks.Where(c => c is Idat).Select(c => c as Idat).ToList();
            OriginalIdat = IdatChunks.First().Data.ToList();


            //var test = new BigInteger(new byte[] {230, 83, 22}, isUnsigned: true).ToByteArray().Take(3).ToArray();
            //var test1 = BigInteger.ModPow(new BigInteger(test, isUnsigned: true), 2678085547, 3020160293).ToByteArray();
            //var test2 = BigInteger.ModPow(new BigInteger(test1, isUnsigned: true),
            // 2341467403, 3020160293).ToByteArray();
            //test1
            foreach (var chunk in IdatChunks)
            {
                int i = 0;
                EncryptedBytes = new List<byte>();
                byte[] xByteArray;
                for (i = 0; i < chunk.Data.Length; i += ChunkByteLength)
                {
                    var bytesToEncrypt = chunk.Data.Skip(i).Take(ChunkByteLength).ToArray();
                    var m = new BigInteger(bytesToEncrypt, isUnsigned: true);
                    var x = BigInteger.ModPow(m, PublicKey.E, PublicKey.N);
                    xByteArray = x.ToByteArray();
                    if (xByteArray.Length > EncryptedChunkByteLength)
                        xByteArray = xByteArray.Take(EncryptedChunkByteLength).ToArray();
                    if (xByteArray.Length == EncryptedChunkByteLength - 1)
                    {
                        var temp = xByteArray.ToList();
                        temp.Add(0);
                        xByteArray = temp.ToArray();
                    }

                    var y = BigInteger.ModPow(new BigInteger(xByteArray, isUnsigned: true),
                        PrivateKey.D, PrivateKey.N);
                    var yBytes = y.ToByteArray();
                    if(yBytes.Length == 4 && yBytes[3] != 0)
                       Console.WriteLine();
                    if (!Enumerable.SequenceEqual(yBytes.Take(ChunkByteLength).ToArray(), bytesToEncrypt))
                        Console.WriteLine();
                    EncryptedBytes.AddRange(xByteArray);
                }

                if (chunk.Data.Length - i + ChunkByteLength != 0)
                {
                    xByteArray = BigInteger
                        .ModPow(
                            new BigInteger(chunk.Data.Skip(i).Take(chunk.Data.Length - i + ChunkByteLength).ToArray(),
                                isUnsigned: true), PublicKey.E, PublicKey.N).ToByteArray();
                    if (xByteArray.Length > EncryptedChunkByteLength)
                        xByteArray = xByteArray.Take(EncryptedChunkByteLength).ToArray();
                    if (xByteArray.Length == EncryptedChunkByteLength - 1)
                    {
                        var temp = xByteArray.ToList();
                        temp.Add(0);
                        xByteArray = temp.ToArray();
                    }
                    if(xByteArray[0] != 0)
                        EncryptedBytes.AddRange(xByteArray);
                }

                var Length = BitConverter.GetBytes(EncryptedBytes.Count).Reverse().ToArray();
                chunk.Length = Length;
                chunk.Data = EncryptedBytes.ToArray();
            }
        }

        public void DecryptPng()
        {

            var DecryptedBytes = new List<byte>();
            var IdatChunks = Png.Chunks.Where(c => c is Idat).Select(c => c as Idat).ToList();
            foreach (var chunk in IdatChunks)
            {
                int i = 0;
                DecryptedBytes = new List<byte>();
                for (i = 0; i < chunk.Data.Length; i += EncryptedChunkByteLength)
                {
                    //var originalData = OriginalIdat.Skip(DecryptedBytes.Count)
                    // .Take(ChunkByteLength).ToArray();
                    var bytesToDecrypt = chunk.Data.Skip(i).Take(EncryptedChunkByteLength).ToArray();
                    //var encyptedBytes = EncryptedIdat.Skip(i).Take(EncryptedChunkByteLength).ToArray();
                    if (bytesToDecrypt.Length == 1)
                        break;
                    //var m = new BigInteger(bytesToEncrypt, false, true);
                    //var zBitSize = PublicKey.N.GetBitLength();
                    //var x = BigInteger.ModPow(m, PublicKey.E, PublicKey.N);
                    //var xBitSize = x.GetBitLength();
                    var y = BigInteger.ModPow(new BigInteger(bytesToDecrypt, isUnsigned: true), PrivateKey.D, PrivateKey.N).ToByteArray();
                    //var x = BigInteger.ModPow(new BigInteger(encyptedBytes, isUnsigned: true), PrivateKey.D, PrivateKey.N).ToByteArray();
                    if (y.Length > ChunkByteLength)
                    {
                        //if(y[3]!=0) 
                        //    Console.WriteLine();
                        y = y.Take(ChunkByteLength).ToArray();
                    }

                    if (y.Length == ChunkByteLength - 1)
                    {
                        var temp = y.ToList();
                        temp.Add(0);
                        y = temp.ToArray();
                    }
                    //if(!Enumerable.SequenceEqual(encyptedBytes ,bytesToDecrypt))
                    //    Console.WriteLine();

                    //if(!Enumerable.SequenceEqual(y,originalData))
                    //    Console.WriteLine();
                    DecryptedBytes.AddRange(y);
                }
                var Length = BitConverter.GetBytes(DecryptedBytes.Count).Reverse().ToArray();
                chunk.Length = Length;
                chunk.Data = DecryptedBytes.ToArray();
            }
        }

        public void EncryptCBCPng()
        {

            var EncryptedBytes = new List<byte>();
            var IdatChunks = Png.Chunks.Where(c => c is Idat).Select(c => c as Idat).ToList();
            //OriginalIdat = IdatChunks.First().Data.ToList();

            //var test = new BigInteger(new byte[] {230, 83, 22}, isUnsigned: true).ToByteArray().Take(3).ToArray();
            //var test1 = BigInteger.ModPow(new BigInteger(test, isUnsigned: true), 2678085547, 3020160293).ToByteArray();
            //var test2 = BigInteger.ModPow(new BigInteger(test1, isUnsigned: true),
            // 2341467403, 3020160293).ToByteArray();
            //test1

            IV = BigIntegerEx.GenerateRandomBigInteger(BigInteger.Pow(2, KeySize - 2),
                BigInteger.Pow(2, KeySize - 1));
            var prev = IV;
            //var prevSize = prev.GetBitLength();
            //Console.WriteLine($"{IV.GetBitLength()}");
            foreach (var chunk in IdatChunks)
            {
                EncryptedBytes = new List<byte>();
                int i = 0;
                for (i = 0; i < chunk.Data.Length; i += ChunkByteLength)
                {
                    var bytesToEncrypt = chunk.Data.Skip(i).Take(ChunkByteLength).ToArray();
                    var m = new BigInteger(bytesToEncrypt, isUnsigned: true);
                    var xor = m ^ prev;
                    //var xorArray = xor.ToByteArray();
                    //var bigXor = new BigInteger(xorArray, isUnsigned: true);
                    var x = BigInteger.ModPow(xor, PublicKey.E, PublicKey.N);
                    var xByteArray = x.ToByteArray();
                    // //#################################
                    //var xBig = new BigInteger(xByteArray, isUnsigned: true);
                    //if (xBig != x)
                    //  Console.WriteLine();
                    //var y = BigInteger.ModPow(xBig, PrivateKey.D, PrivateKey.N);
                    //if (y != bigXor)
                    //    Console.WriteLine();

                    //var yBytes = y.ToByteArray();
                    //var yBig = new BigInteger(y.ToByteArray(), isUnsigned: true);
                    //var xorBack = prev ^ y;
                    //if (xorBack != m)
                    //   Console.WriteLine();
                    //var xorBytes = xorBack.ToByteArray();
                    //###############################
                    prev = x;
                    if (xByteArray.Length > EncryptedChunkByteLength)
                        xByteArray = xByteArray.Take(EncryptedChunkByteLength).ToArray();
                    if (xByteArray.Length == EncryptedChunkByteLength - 1)
                    {
                        var temp = xByteArray.ToList();
                        temp.Add(0);
                        xByteArray = temp.ToArray();
                    }
                    EncryptedBytes.AddRange(xByteArray);
                }

                if (chunk.Data.Length - i + ChunkByteLength != 0)
                {
                    EncryptedBytes.AddRange(BigInteger.ModPow(new BigInteger(chunk.Data.Skip(i).Take(chunk.Data.Length - i + ChunkByteLength).ToArray(), isUnsigned: true), PublicKey.E, PublicKey.N).ToByteArray());
                }

                var Length = BitConverter.GetBytes(EncryptedBytes.Count).Reverse().ToArray();
                chunk.Length = Length;
                chunk.Data = EncryptedBytes.ToArray();
                EncryptedIdat = EncryptedBytes.ToList();
            }
        }

        public void DecryptCBCPng()
        {

            var DecryptedBytes = new List<byte>();
            var IdatChunks = Png.Chunks.Where(c => c is Idat).Select(c => c as Idat).ToList();
            var prev = IV;
            foreach (var chunk in IdatChunks)
            {
                DecryptedBytes = new List<byte>();
                int i = 0;
                for (i = 0; i < chunk.Data.Length; i += EncryptedChunkByteLength)
                {
                    //var originalData = OriginalIdat.Skip(DecryptedBytes.Count)
                     //.Take(ChunkByteLength).ToArray();
                    var bytesToDecrypt = chunk.Data.Skip(i).Take(EncryptedChunkByteLength).ToArray();
                    //var encyptedBytes = EncryptedIdat.Skip(i).Take(EncryptedChunkByteLength).ToArray();
                    if (bytesToDecrypt.Length == 1)
                        break;
                    //var m = new BigInteger(bytesToEncrypt, false, true);
                    //var zBitSize = PublicKey.N.GetBitLength();
                    //var x = BigInteger.ModPow(m, PublicKey.E, PublicKey.N);
                    //var xBitSize = x.GetBitLength();
                    var y = BigInteger.ModPow(new BigInteger(bytesToDecrypt, isUnsigned: true), PrivateKey.D, PrivateKey.N).ToByteArray();
                    //var x = BigInteger.ModPow(new BigInteger(encyptedBytes, isUnsigned: true), PrivateKey.D, PrivateKey.N).ToByteArray();
                    //if(!Enumerable.SequenceEqual(encyptedBytes ,bytesToDecrypt))
                    //    Console.WriteLine();

                    
                    var xor = prev ^ new BigInteger(y, isUnsigned: true);

                    var xorBytes = xor.ToByteArray();
                    if (xorBytes.Length > ChunkByteLength)
                    {
                        //if(y[3]!=0) 
                        //    Console.WriteLine();
                        xorBytes = xorBytes.Take(ChunkByteLength).ToArray();
                    }

                    if (xorBytes.Length == ChunkByteLength -1)
                    {
                        var temp = xorBytes.ToList();
                        temp.Add(0);
                        xorBytes = temp.ToArray();
                    }

                    //if (!Enumerable.SequenceEqual(xorBytes, originalData))
                     //   Console.WriteLine();
                    prev = new BigInteger(bytesToDecrypt, isUnsigned: true);
                    DecryptedBytes.AddRange(xorBytes);
                }
                var Length = BitConverter.GetBytes(DecryptedBytes.Count ).Reverse().ToArray();
                chunk.Length = Length;
                chunk.Data = DecryptedBytes.ToArray();
            }
        }
    }
}

