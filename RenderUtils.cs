using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace CncFullMapPreviewGenerator
{
    static class RenderUtils
    {
        static public Bitmap RenderShp(ShpReader shp, Palette p, int Frame_)
        {
            var frame = shp[Frame_];

            var bitmap = new Bitmap(shp.Width, shp.Height, PixelFormat.Format8bppIndexed);

            bitmap.Palette = p.AsSystemPalette();

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                byte* q = (byte*)data.Scan0.ToPointer();
                var stride2 = data.Stride;

                for (var i = 0; i < shp.Width; i++)
                    for (var j = 0; j < shp.Height; j++)
                        q[j * stride2 + i] = frame.Image[i + shp.Width * j];
            }

            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static Bitmap RenderTemplate(TemplateReader template, Palette p, int frame)
        {
            var bitmap = new Bitmap(TemplateReader.TileSize, TemplateReader.TileSize,
                PixelFormat.Format8bppIndexed);

            bitmap.Palette = p.AsSystemPalette();

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                byte* q = (byte*)data.Scan0.ToPointer();
                var stride = data.Stride;

                        if (template.TileBitmapBytes[frame] != null)
                        {
                            var rawImage = template.TileBitmapBytes[frame];
                            for (var i = 0; i < TemplateReader.TileSize; i++)
                                for (var j = 0; j < TemplateReader.TileSize; j++)
                                    q[j * stride + i] = rawImage[i + TemplateReader.TileSize * j];
                        }
                        else
                        {
                            for (var i = 0; i < TemplateReader.TileSize; i++)
                                for (var j = 0; j < TemplateReader.TileSize; j++)
                                    q[j * stride + i] = 0;
                        }
            }

            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static Bitmap RenderTemplate(TemplateReader template, Palette p)
        {

            var bitmap = new Bitmap(TemplateReader.TileSize * template.Width, TemplateReader.TileSize * template.Height,
                PixelFormat.Format8bppIndexed);

            bitmap.Palette = p.AsSystemPalette();

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            unsafe
            {
                byte* q = (byte*)data.Scan0.ToPointer();
                var stride = data.Stride;

                for (var u = 0; u < template.Width; u++)
                    for (var v = 0; v < template.Height; v++)
                        if (template.TileBitmapBytes[u + v * template.Width] != null)
                        {
                            var rawImage = template.TileBitmapBytes[u + v * template.Width];
                            for (var i = 0; i < TemplateReader.TileSize; i++)
                                for (var j = 0; j < TemplateReader.TileSize; j++)
                                    q[(v * TemplateReader.TileSize + j) * stride + u * TemplateReader.TileSize + i] = rawImage[i + TemplateReader.TileSize * j];
                        }
                        else
                        {
                            for (var i = 0; i < TemplateReader.TileSize; i++)
                                for (var j = 0; j < TemplateReader.TileSize; j++)
                                    q[(v * TemplateReader.TileSize + j) * stride + u * TemplateReader.TileSize + i] = 0;
                        }
            }

            bitmap.UnlockBits(data);
            return bitmap;
        }

    }
}
