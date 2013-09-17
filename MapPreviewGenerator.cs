using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Nyerguds.Ini;
using System.Drawing;

namespace CncFullMapPreviewGenerator
{
    class MapPreviewGenerator
    {
        static Dictionary<string, int> TiberiumStages = new Dictionary<string, int>();
        static Random MapRandom;
        const int CellSize = 24; // in pixels
        static bool IsLoaded = false;
        IniFile MapINI;
        IniFile TemplatesINI;
        static IniFile TilesetsINI;
        string TheaterFilesExtension;
        Palette Pal;
        CellStruct[,] Cells = new CellStruct[64, 64];
        List<WaypointStruct> Waypoints = new List<WaypointStruct>();
        List<UnitInfo> Units = new List<UnitInfo>();
        List<InfantryInfo> Infantries = new List<InfantryInfo>();

        static Bitmap[] SpawnLocationBitmaps = new Bitmap[8];
        int MapWidth = -1, MapHeight = -1, MapY = -1, MapX = -1;

        public static void Load()
        {
            TilesetsINI = new IniFile("data/tilesets.ini");
            MapRandom = new Random();;

            TiberiumStages.Add("ti1", 0);
            TiberiumStages.Add("ti2", 1);
            TiberiumStages.Add("ti3", 2);
            TiberiumStages.Add("ti4", 3);
            TiberiumStages.Add("ti5", 4);
            TiberiumStages.Add("ti6", 5);
            TiberiumStages.Add("ti7", 6);
            TiberiumStages.Add("ti8", 7);
            TiberiumStages.Add("ti9", 8);
            TiberiumStages.Add("ti10", 9);
            TiberiumStages.Add("ti11", 10);
            TiberiumStages.Add("ti12", 11);
        }

        public MapPreviewGenerator(string FileName)
        {

            MapINI = new IniFile(FileName);

            MapHeight = MapINI.getIntValue("Map", "Height", -1);
            MapWidth = MapINI.getIntValue("Map", "Width", -1);
            MapX = MapINI.getIntValue("Map", "X", -1);
            MapY = MapINI.getIntValue("Map", "Y", -1);

            Parse_Theater();

            string MapBin = FileName.Replace(".ini", ".bin");

            Console.WriteLine("MapBin = {0}, FileName = {1}", MapBin, FileName);

            CellStruct[] Raw = new CellStruct[64*64];

            byte[] fileBytes = File.ReadAllBytes(MapBin);

            var ByteReader = new FastByteReader(fileBytes);


            // Parse templates
            int i = 0;
            while (!ByteReader.Done())
            {

                Raw[i].Template = ByteReader.ReadByte();
                Raw[i].Tile = ByteReader.ReadByte();

                //                Console.WriteLine("{0} = {1}", i, Cells[i].Template);
                ++i;

                if (i == 64 * 64)
                    break;
            }

            Parse_Waypoints();
            Parse_Terrain(Raw);
            Parse_Overlay(Raw);
            Parse_Units();
            Parse_Infantry();

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    int Index = (x * 64) + y;
                    Cells[y, x] = Raw[Index];
                }
            }
        }

        void Parse_Waypoints()
        {
            var SectionKeyValues = MapINI.getSectionContent("Waypoints");

            foreach (KeyValuePair<string, string> entry in SectionKeyValues)
            {
                int WayPoint = int.Parse(entry.Key);
                int CellIndex = int.Parse(entry.Value);

                Console.WriteLine("Waypoint = {0}, Index = {1}", WayPoint, CellIndex);
                WaypointStruct WP = new WaypointStruct();
                WP.Number = WayPoint;
                WP.X =  CellIndex % 64;
                WP.Y = CellIndex / 64;
                Waypoints.Add(WP);
            }
        }

        void Parse_Overlay(CellStruct[] Raw)
        {
            var SectionOverlay = MapINI.getSectionContent("Overlay");

            if (SectionOverlay != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionOverlay)
                {
                    int Cell = int.Parse(entry.Key);
                    string Overlay = entry.Value;

                    Raw[Cell].Overlay = Overlay;

                    //                Console.WriteLine("{0} = {1}", Cell, Terrain);
                }
            }
        }

        void Parse_Terrain(CellStruct[] Raw)
        {
            var SectionTerrrain = MapINI.getSectionContent("Terrain");

            if (SectionTerrrain != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionTerrrain)
                {
                    int Cell = int.Parse(entry.Key);
                    string Terrain = entry.Value;

                    Raw[Cell].Terrain = Terrain;

                    // Console.WriteLine("{0} = {1}", Cell, Terrain);
                }
            }
        }

        void Parse_Theater()
        {

            string Theater = MapINI.getStringValue("Map", "Theater", "temperate");
            Theater = Theater.ToLower();

            switch (Theater)
            {
                case "winter": TheaterFilesExtension = ".win"; break;
                case "snow": TheaterFilesExtension = ".sno"; break;
                case "desert": TheaterFilesExtension = ".des"; break;
                default: TheaterFilesExtension = ".tem"; break;
            }

            string PalName = "temperat";

            switch (Theater)
            {
                case "winter": PalName = "winter"; break;
                case "snow": PalName = "snow"; break;
                case "desert": PalName = "desert"; break;
                default: PalName = "temperat";  break;
            }

            int[] ShadowIndex = { 3, 4 };
            Pal = Palette.Load("data/" + PalName + ".pal", ShadowIndex);
        }

        void Sub_Cell_Pixel_Offsets(int SubCell, out int X, out int Y)
        {
            X = -19; Y = -9;

            switch (SubCell)
            {
                case 1: X += 0; Y += 0; break;
                case 2: X += 11; Y += 0; break;
                case 3: Y += 11; break;
                case 4: X += 11; Y += 11; break;
                case 0: X += 6; Y += 6; break;
                default: break;
            }
        }

        void Parse_Units()
        {
            var SectionUnits = MapINI.getSectionContent("Units");
            if (SectionUnits != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionUnits)
                {
                    string UnitCommaString = entry.Value;
                    string[] UnitData = UnitCommaString.Split(',');

                    UnitInfo u = new UnitInfo();
                    u.Name = UnitData[1];
                    u.Side = UnitData[0];
                    u.Angle = int.Parse(UnitData[4]);

                    int CellIndex = int.Parse(UnitData[3]);
                    u.Y = CellIndex / 64;
                    u.X = CellIndex % 64;

                    Units.Add(u);

                    Console.WriteLine("Unit name = {0}, side {1}, Angle = {2}, X = {3}, Y = {4}", u.Name,
                        u.Side, u.Angle, u.X, u.Y);
                }
            }
        }

        void Parse_Infantry()
        {
            // 0=neutral,c1,256,2973,2,guard,3,none
            var SectionInfantry = MapINI.getSectionContent("Infantry");
            if (SectionInfantry != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionInfantry)
                {
                    string InfCommaString = entry.Value;
                    string[] InfData = InfCommaString.Split(',');

                    InfantryInfo inf = new InfantryInfo();
                    inf.Name = InfData[1];
                    inf.Side = InfData[0];
                    inf.Angle = int.Parse(InfData[6]);
                    inf.SubCell = int.Parse(InfData[4]);

                    int CellIndex = int.Parse(InfData[3]);
                    inf.Y = CellIndex / 64;
                    inf.X = CellIndex % 64;

                    Infantries.Add(inf);

                    int subX; int subY;
                    Sub_Cell_Pixel_Offsets(inf.SubCell, out subX, out subY);

                    Console.WriteLine("infantry name = {0}, Side = {1}, Angle = {2}, SubCell = {5}, X = {3}, Y = {4}", inf.Name,
                        inf.Side, inf.Angle, inf.X + subX, inf.Y + subY, inf.SubCell);
                }
            }
        }

        public Bitmap Get_Bitmap()
        {
            Bitmap bitMap = new Bitmap(64 * CellSize, 64 * CellSize);
            Graphics g = Graphics.FromImage(bitMap);


            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    CellStruct data = Cells[x, y];

                    Draw_Template(data, g, x, y);
                }
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    CellStruct data = Cells[x, y];

                    if (data.Terrain != null)
                    {
                        Draw_Terrain(data, g, x, y);
                    }
                }
            }


            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    CellStruct data = Cells[x, y];

                    if (data.Overlay != null)
                    {
                         Draw_Overlay(data, g, x, y);
                    }
                }
            }

            Draw_Units(g);
            Draw_Infantries(g);



/*            if (Is_Out_Of_Bounds(x, y))
            {
                // whatever
            }*/

//            Graphics g = Graphics.FromImage(_Bitmap);

//            Draw_Spawn_Locations(ref g, _ScaleFactor);
//            g.Flush();

            return bitMap;
        }

        void Draw_Units(Graphics g)
        {
            foreach (UnitInfo u in Units)
            {
                Draw_Unit(u, g);
            }

        }

        void Draw_Unit(UnitInfo u, Graphics g)
        {
            ShpReader UnitShp = ShpReader.Load(General_File_String_From_Name(u.Name));

            Bitmap UnitBitmap = RenderUtils.RenderShp(UnitShp, Pal, 0);

            Draw_Centered(g, UnitBitmap, u);
        }

        void Draw_Centered(Graphics g, Bitmap bitMap, UnitInfo u)
        {
            int X = (u.X * CellSize) + 12 - (bitMap.Width / 2);
            int Y = (u.Y * CellSize) + 12 - (bitMap.Height / 2);

            g.DrawImage(bitMap, X, Y, bitMap.Width, bitMap.Height);
        }

        void Draw_Infantries(Graphics g)
        {
            foreach (InfantryInfo i in Infantries)
            {
                Draw_Infantry(i, g);
            }

        }

        void Draw_Infantry(InfantryInfo inf, Graphics g)
        {
            ShpReader InfShp = ShpReader.Load(General_File_String_From_Name(inf.Name));

            Bitmap TempBitmap = RenderUtils.RenderShp(InfShp, Pal, 0);
            int subX, subY;
            Sub_Cell_Pixel_Offsets(inf.SubCell, out subX, out subY);

            g.DrawImage(TempBitmap, inf.X * CellSize + subX, inf.Y * CellSize + subY, TempBitmap.Width, TempBitmap.Height);
        }

        void Draw_Template(CellStruct Cell, Graphics g, int X, int Y)
        {
//            if (Cell.Tile != 0 ) { return; }

            string TemplateString = TilesetsINI.getStringValue("TileSets", Cell.Template.ToString(), "0");

            TemplateReader Temp = TemplateReader.Load(File_String_From_Name(TemplateString));

            Bitmap TempBitmap = RenderUtils.RenderTemplate(Temp, Pal, Cell.Tile);
            g.DrawImage(TempBitmap, X * CellSize, Y * CellSize, TempBitmap.Width, TempBitmap.Height);
        }

        string File_String_From_Name(string Name)
        {
            return ("data/" + Name + TheaterFilesExtension);
        }

        string General_File_String_From_Name(string Name)
        {
            return ("data/general/" + Name + ".shp");
        }

        void Draw_Overlay(CellStruct Cell, Graphics g, int X, int Y)
        {
            string Overlay = Cell.Overlay;
            int Frame = 0;

            if (TiberiumStages.ContainsKey(Overlay))
            {
                Frame = -1;
                TiberiumStages.TryGetValue(Overlay, out Frame);
                int index = MapRandom.Next(1, 12); // creates a number between 1 and 12
                Overlay = string.Format("TI{0}", index);
            }

            ShpReader Shp = ShpReader.Load(File_String_From_Name(Overlay));

            Bitmap ShpBitmap = RenderUtils.RenderShp(Shp, Pal, Frame);
            g.DrawImage(ShpBitmap, X * CellSize, Y * CellSize, ShpBitmap.Width, ShpBitmap.Height);
        }

        void Draw_Terrain(CellStruct Cell, Graphics g, int X, int Y)
        {
            string[] TerrainData = Cell.Terrain.Split(',');

            ShpReader Shp = ShpReader.Load(File_String_From_Name(TerrainData[0]));

            Bitmap ShpBitmap = RenderUtils.RenderShp(Shp, Pal, 0);
            g.DrawImage(ShpBitmap, X * CellSize, Y * CellSize, ShpBitmap.Width, ShpBitmap.Height);
        }

        bool Is_Out_Of_Bounds(int X, int Y)
        {
            if (MapX > X || X >= MapX + MapWidth)
                return true;

            if (MapY > Y || Y >= MapY + MapHeight)
                return true;

            return false;
        }

    }

    struct UnitInfo
    {
        public string Name;
        public string Side;
        public int Angle;
        public int X;
        public int Y;
    }

    struct InfantryInfo
    {
        public string Name;
        public string Side;
        public int Angle;
        public int X;
        public int Y;
        public int SubCell;
    }

    struct CellStruct
    {
        public int Template;
        public int Tile;
        public string Overlay;
        public string Terrain;
        public int Waypoint;
    }

    struct WaypointStruct
    {
        public int Number;
        public int X;
        public int Y;
    }

    enum TerrainType
    {
        Clear = 0,
        Water,
        Road,
        Rock,
        Tree,
        River,
        Rough,
        Wall,
        Beach,
        Ore,
        Gems,
    }
}
