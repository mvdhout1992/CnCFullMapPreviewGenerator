using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Globalization;

namespace CncFullMapPreviewGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make sure the Parse() functions parse commas and periods correctly
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            MapPreviewGenerator.Load();

            var MapPreview = new MapPreviewGenerator("x.ini");
            MapPreview.Get_Bitmap().Save("derp.png");

//            Console.Read();
        }
    }
}

//            int[] ShadowIndex = { };
//            Palette Pal = Palette.Load("data/temperat.pal", ShadowIndex);
//            ShpReader SHP = ShpReader.Load("data/dome.shp");
//            Bitmap ShpBitmap = RenderUtils.RenderShp(SHP, Pal, 1);

//            TemplateReader Template = TemplateReader.Load("data/bridge2h.tem", Pal);
//            Bitmap ShpBitmap = RenderUtils.RenderTemplate(Template, Pal);
//            ShpBitmap.Save("derp.png");