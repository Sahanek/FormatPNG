using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Itxt : Chunk
    {
        public string Keyword { get; set; }
        public string Txt { get; set; }
        public byte CompressionFlag { get; set; }
        public byte CompressionMethod { get; set; }
        public string LanguageTag { get; set; } = string.Empty;
        public string TranslatedKeyword { get; set; } = string.Empty;

        public Itxt(byte[] length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
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
                   Keyword = System.Text.Encoding.UTF8.GetString(Data.Skip(startPosition).Take(i - startPosition).ToArray());
                    i++;// Null Separator

                    CompressionFlag = Data[i++];
                    CompressionMethod = Data[i++];

                    //Language Tag
                    startPosition = i;
                    if (Data[i] != 0)
                    {
                        while (i < Data.Length && Data[i] != 0) i++; //Dopóki nie dotrze do Nulla
                        LanguageTag = System.Text.Encoding.UTF8.GetString(Data.Skip(startPosition)
                            .Take(i - startPosition).ToArray());
                    }
                    //Translated Keyword
                    startPosition = ++i;
                    if (Data[i] != 0)
                    {
                        while (i < Data.Length && Data[i] != 0) i++; //Dopóki nie dotrze do Nulla
                        TranslatedKeyword = System.Text.Encoding.UTF8.GetString(Data.Skip(startPosition)
                            .Take(i - startPosition).ToArray());

                    }

                    startPosition = ++i;

                    //Text
                    Txt =  System.Text.Encoding.UTF8.GetString(Data.Skip(startPosition)
                        .Take(Data.Length - startPosition).ToArray());
                }
            }
        }
        public override void WriteChunk()
        {
            base.WriteChunk();
            Console.WriteLine($"Compression Flag : {CompressionFlag}");
            Console.WriteLine($"Compression Method : {CompressionMethod}");
            Console.WriteLine($"Language Tag : {LanguageTag}");
            Console.WriteLine($"Translated Keyword : {TranslatedKeyword}");
            Console.WriteLine($"{Keyword}: {Txt}");
        }

    }
}
