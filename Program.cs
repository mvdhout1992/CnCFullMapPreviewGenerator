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
            if (args.Length < 1)
            {
                Console.WriteLine("ERROR: Not enough arguments, at least 1 argument is needed.");
                Console.WriteLine("Usage: ExeFile InputMap [OutputImage] [--DrawVisibleOnly]");
                return;
            }

            bool DrawVisibleOnly = false;
            string OutFile = null;

            if (args.Length >= 2)
            {
                if (args[1].ToLower() == "--drawvisibleonly")
                {
                    DrawVisibleOnly = true;
                }
                else
                {
                    OutFile = args[1];
                }
            }

            if (args.Length >= 3)
            {
                DrawVisibleOnly = args[2].ToLower() == "--drawvisibleonly";
            }

            if (OutFile == null)
                OutFile = args[0].ToLower().Replace(".ini", ".png");

            // Make sure the Parse() functions parse commas and periods correctly
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            MapPreviewGenerator.Load();

            var MapPreview = new MapPreviewGenerator(args[0]);
            MapPreview.Get_Bitmap(DrawVisibleOnly).Save(OutFile);
        }


        static void Main2(string[] args)
        {
            int[] ShadowIndex = { 3, 4 };
            RGB[] Remaps = { 
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                               new RGB(255, 0, 0),
                            
                            };
            Palette Pal = Palette.Load_With_Remaps("data/temperate/temperat.pal", ShadowIndex, Remaps);
//            Palette Pal = Palette.Load("data/temperat.pal", ShadowIndex);
            ShpReader SHP = ShpReader.Load("data/general/proc.shp");
            Bitmap ShpBitmap = RenderUtils.RenderShp(SHP, Pal, 19);

//            TemplateReader Template = TemplateReader.Load("data/bridge2h.tem", Pal);
//            Bitmap ShpBitmap = RenderUtils.RenderTemplate(Template, Pal);
            ShpBitmap.Save("derp.png");
        }
    }
}