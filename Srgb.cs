using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormatPNG
{
    internal class Srgb : Chunk
    {
        public string RenderingIntent { get; set; }

        public Srgb(int length, byte[] cType, byte[] data, byte[] crc32) : base(length, cType, data, crc32)
        {
            Encode();
        }

        private void Encode()
        {
            RenderingIntent = (int) Data[0] switch
            {
                0 => "Perceptual",
                1 => "Relative colorimetric",
                2 => "Saturation",
                3 => "Absolute colorimetric",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        public override void WriteChunk()
        {
            Console.WriteLine($"Type   : {Encoding.UTF8.GetString(CType)}");
            Console.WriteLine($"Length : {Length}");
            Console.WriteLine($"CRC32  : {String.Join(' ', Crc32)}");
            Console.WriteLine($"RenderingIntent : {RenderingIntent}");
        }
    }
    /*
    0  Perceptual intent is for images preferring good adaptation to the output device gamut at the expense
    of colorimetric accuracy, like photographs.
    1  Relative colorimetric intent is for images requiring color appearance matching (relative to the output
    device white point), like logos.
    2  Saturation intent is for images preferring preservation of saturation at the expense of hue and lightness,
    like charts and graphs.
    3  Absolute colorimetric intent is for images requiring preservation of absolute colorimetry, like proofs 
    (previews of images destined for a different output device).
     */
}
