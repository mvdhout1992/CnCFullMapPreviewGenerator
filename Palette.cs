using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Reflection;

namespace CncFullMapPreviewGenerator
{
    public class Palette
    {
        uint[] colors;
        public Color GetColor(int index)
        {
            return Color.FromArgb((int)colors[index]);
        }

        public void SetColor(int index, Color color)
        {
            colors[index] = (uint)color.ToArgb();
        }

        public void SetColor(int index, uint color)
        {
            colors[index] = (uint)color;
        }

        public uint[] Values
        {
            get { return colors; }
        }

        public Palette(Stream s, int[] remapShadow)
        {
            colors = new uint[256];

            using (BinaryReader reader = new BinaryReader(s))
            {
                for (int i = 0; i < 256; i++)
                {
                    byte r = (byte)(reader.ReadByte() << 2);
                    byte g = (byte)(reader.ReadByte() << 2);
                    byte b = (byte)(reader.ReadByte() << 2);
                    colors[i] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
                }
            }

            colors[0] = 0; //convert black background to transparency
            foreach (int i in remapShadow)
                colors[i] = 140u << 24;
        }

        public Palette(Palette p, IPaletteRemap r)
        {
            colors = new uint[256];
            for (int i = 0; i < 256; i++)
                colors[i] = (uint)r.GetRemappedColor(Color.FromArgb((int)p.colors[i]), i).ToArgb();
        }

        public Palette(Palette p)
        {
            colors = (uint[])p.colors.Clone();
        }

        public ColorPalette AsSystemPalette()
        {
            ColorPalette pal;
            using (var b = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
                pal = b.Palette;

            for (var i = 0; i < 256; i++)
                pal.Entries[i] = GetColor(i);

            // hack around a mono bug -- the palette flags get set wrong.
            if (Platform.CurrentPlatform != PlatformType.Windows)
                typeof(ColorPalette).GetField("flags",
                    BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pal, 1);

            return pal;
        }

        public static Palette Load(string filename, int[] remap)
        {
            using (var s = File.OpenRead(filename))
                return new Palette(s, remap);
        }

        public static Palette Load_With_Remaps(string filename, int[] shadowreamp, uint[] remaps)
        {
            if (remaps.Length != 16) throw new ArgumentException("remaps int array needs to have size of 16..");

            using (var s = File.OpenRead(filename))
            {
                // 		RemapIndex: 176, 178, 180, 182, 184, 186, 189, 191, 177, 179, 181, 183, 185, 187, 188, 190
                Palette p = new Palette(s, shadowreamp);
                p.colors[176] = remaps[0];
                p.colors[178] = remaps[1];
                p.colors[180] = remaps[2];
                p.colors[182] = remaps[3];
                p.colors[184] = remaps[4];
                p.colors[186] = remaps[5];
                p.colors[189] = remaps[6];
                p.colors[191] = remaps[7];
                p.colors[177] = remaps[8];
                p.colors[179] = remaps[9];
                p.colors[181] = remaps[10];
                p.colors[183] = remaps[11];
                p.colors[185] = remaps[12];
                p.colors[187] = remaps[13];
                p.colors[188] = remaps[14];
                p.colors[190] = remaps[15];

                return p;
            }
        }
    }

    public interface IPaletteRemap { Color GetRemappedColor(Color original, int index); }
}
