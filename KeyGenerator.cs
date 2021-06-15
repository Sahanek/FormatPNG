using System;
using System.Collections.Generic;
using System.FormatPNG;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace FormatPNG
{
    public class KeyGenerator
    {
        public int KeySize { get; set; }
        public int PrimeSize { get; set; }
        public BigInteger N { get; private set; }
        public BigInteger E { get; private set; } // Klucz szyfrujący
        public BigInteger D { get; private set; }

        public KeyGenerator(int keySize)
        {
            KeySize = keySize;
            PrimeSize = keySize / 2;
            GenerateKeys();

        }


        public BigInteger Nwd(BigInteger a, BigInteger b) // Największy wspólny dzielnik
        {
            if (a == 0)
                return b;

            return Nwd(b % a, a);
        }

        public BigInteger GenerateLargePrime()
        {
            var rng = new RNGCryptoServiceProvider();
            var byteArray = new byte[PrimeSize / 8];
            while (true)
            {
                rng.GetBytes(byteArray);
                var number = new BigInteger(byteArray, true);
                if (number.IsPrime(25, out double x))
                    return number;
            }
        }
        public void GenerateKeys()
        {
            while (true)
            {
                BigInteger p = 0, q = 0;
                long LastN;
                do
                {
                    p = GenerateLargePrime();
                    q = GenerateLargePrime();
                    N = p * q;
                }
                while (N.GetBitLength() != KeySize || Nwd(p, q) != 1);

                LastN = N.GetBitLength();
                BigInteger phi = (p - 1) * (q - 1);
                while (true)
                {
                    var e = BigIntegerEx.GenerateRandomBigInteger(BigInteger.Pow(2, KeySize - 1),
                        BigInteger.Pow(2, KeySize));
                    var bitSize = e.GetBitLength();
                    if (e < phi && Nwd(e, phi) == 1 && e.GetBitLength() == KeySize)
                    {
                        E = e;

                        break;
                    }

                }

                D = E.ModInverse(phi);
                if(D.GetBitLength() == KeySize)
                    break;
            }


            //if (E.GetBitLength() == KeySize && D.GetBitLength() == KeySize)
            //{
            //BigInteger m = 12345;
            //Console.WriteLine($"Private key {E}, {N}");
            //Console.WriteLine($"Public key {D}, {N}");

            //Console.WriteLine($"Private key {E.GetBitLength()}, {N.GetBitLength()}");
            //Console.WriteLine($"Public key {D.GetBitLength()}");

            //var c = BigInteger.ModPow(m, E, N);
            //var x =  BigInteger.ModPow(c, D, N);
            //Console.WriteLine($"{m},  {c}, {x}");

            //}

        }

    }
}
