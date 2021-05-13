﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Text : Chunk
    {
        public string Keyword { get; set; }
        public string Txt { get; set; }
        public Text(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Decode();
        }

        public void Decode()
        {

            int startPosition = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] == 0)
                {
                    Keyword = System.Text.Encoding.UTF8.GetString(Data.Skip(startPosition).Take(i).ToArray());
                    startPosition = ++i;
                    Txt = System.Text.Encoding.UTF8.GetString(Data.Skip(startPosition)
                        .Take(Data.Length - startPosition).ToArray());
                    break;
                }

            }
        }

        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine($"{Keyword}: {Txt}");
        }
    }
}
