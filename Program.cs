using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CncFullMapPreviewGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] ShadowIndex = { };
            Palette Pal = Palette.Load("data/temperat.pal", ShadowIndex);
            ShpReader SHP = ShpReader.Load("data/dome.shp");
            Bitmap ShpBitmap = RenderUtils.RenderShp(SHP, Pal, 1);

//            TemplateReader Template = TemplateReader.Load("data/bridge2h.tem", Pal);
//            Bitmap ShpBitmap = RenderUtils.RenderTemplate(Template, Pal);

            ShpBitmap.Save("derp.png");
        }
    }
}
