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
        string Theater;
        string PalName;
        IniFile MapINI;
        IniFile TemplatesINI;
        static IniFile TilesetsINI;
        string TheaterFilesExtension;
        Palette Pal;
        Dictionary<String, HouseInfo> HouseColors = new Dictionary<string, HouseInfo>();
        CellStruct[,] Cells = new CellStruct[64, 64];
        List<WaypointStruct> Waypoints = new List<WaypointStruct>();
        List<UnitInfo> Units = new List<UnitInfo>();
        List<InfantryInfo> Infantries = new List<InfantryInfo>();
        List<SmudgeInfo> Smudges = new List<SmudgeInfo>();
        List<StructureInfo> Structures = new List<StructureInfo>();
        List<CellTriggerInfo> CellsTriggers = new List<CellTriggerInfo>();
        Dictionary<string, Palette> ColorRemaps = new Dictionary<string, Palette>();
        List<BibInfo> Bibs = new List<BibInfo>();
        static Dictionary<string, BuildingBibInfo> BuildingBibs = new Dictionary<string, BuildingBibInfo>();
        static Dictionary<string, int> BuildingDamageFrames = new Dictionary<string, int>();

        static Bitmap[] SpawnLocationBitmaps = new Bitmap[8];
        int MapWidth = -1, MapHeight = -1, MapY = -1, MapX = -1;

        public static void Load()
        {
            TilesetsINI = new IniFile("data/tilesets.ini");
            MapRandom = new Random();;

            Load_Building_Damage_Frames();
            Load_Building_Bibs();

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

        static void Load_Building_Bibs()
        {
            BuildingBibs.Add("afld", new BuildingBibInfo("bib1", 1));

            BuildingBibs.Add("bio", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("eye", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("fact", new BuildingBibInfo("bib2", 1));
            BuildingBibs.Add("fix", new BuildingBibInfo("bib2", 2));
            BuildingBibs.Add("hand", new BuildingBibInfo("bib3", 2));
            BuildingBibs.Add("hosp", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("hpad", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("hq", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("miss", new BuildingBibInfo("bib2", 1));
            BuildingBibs.Add("nuke", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("nuk2", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("proc", new BuildingBibInfo("bib2", 2));
            BuildingBibs.Add("pyle", new BuildingBibInfo("bib3", 1));
            BuildingBibs.Add("silo", new BuildingBibInfo("bib3", 0));
            BuildingBibs.Add("weap", new BuildingBibInfo("bib2", 2));
            BuildingBibs.Add("tmpl", new BuildingBibInfo("bib2", 2));

        }
        static void Load_Building_Damage_Frames()
        {
            // the GUN turret building requires special logic as it 
            // might make use of the angle property

            BuildingDamageFrames.Add("afld", 16);
            BuildingDamageFrames.Add("arco", 1);
            BuildingDamageFrames.Add("atwr", 1);
            BuildingDamageFrames.Add("bio", 1);
            BuildingDamageFrames.Add("eye", 16);
            BuildingDamageFrames.Add("fact", 24);
            BuildingDamageFrames.Add("fix", 7);
            BuildingDamageFrames.Add("gtwr", 1);
            BuildingDamageFrames.Add("gun", 64);
            BuildingDamageFrames.Add("hand", 1);
            BuildingDamageFrames.Add("hosp", 4);
            BuildingDamageFrames.Add("hpad", 7);
            BuildingDamageFrames.Add("hq", 16);
            BuildingDamageFrames.Add("miss", 1);
            BuildingDamageFrames.Add("nuk2", 4);
            BuildingDamageFrames.Add("nuke", 4);
            BuildingDamageFrames.Add("obli", 4);
            BuildingDamageFrames.Add("proc", 30);
            BuildingDamageFrames.Add("pyle", 10);
            BuildingDamageFrames.Add("sam", 64);
            BuildingDamageFrames.Add("silo", 5);
            BuildingDamageFrames.Add("weap2", 10);
            BuildingDamageFrames.Add("v01", 1);
            BuildingDamageFrames.Add("v02", 1);
            BuildingDamageFrames.Add("v03", 1);
            BuildingDamageFrames.Add("v04", 1);
            BuildingDamageFrames.Add("v05", 1);
            BuildingDamageFrames.Add("v06", 1);
            BuildingDamageFrames.Add("v07", 1);
            BuildingDamageFrames.Add("v08", 1);
            BuildingDamageFrames.Add("v09", 1);
            BuildingDamageFrames.Add("v10", 1);
            BuildingDamageFrames.Add("v11", 1);
            BuildingDamageFrames.Add("v12", 1);
            BuildingDamageFrames.Add("v13", 1);
            BuildingDamageFrames.Add("v14", 1);
            BuildingDamageFrames.Add("v15", 1);
            BuildingDamageFrames.Add("v16", 1);
            BuildingDamageFrames.Add("v17", 1);
            BuildingDamageFrames.Add("v18", 1);
            BuildingDamageFrames.Add("v19", 14);
            BuildingDamageFrames.Add("v20", 3);
            BuildingDamageFrames.Add("v21", 3);
            BuildingDamageFrames.Add("v22", 3);
            BuildingDamageFrames.Add("v23", 3);
            BuildingDamageFrames.Add("v24", 1);
            BuildingDamageFrames.Add("v25", 1);
            BuildingDamageFrames.Add("v26", 1);
            BuildingDamageFrames.Add("v27", 1);
            BuildingDamageFrames.Add("v28", 1);
            BuildingDamageFrames.Add("v29", 1);
            BuildingDamageFrames.Add("v30", 1);
            BuildingDamageFrames.Add("v31", 1);
            BuildingDamageFrames.Add("v32", 1);
            BuildingDamageFrames.Add("v33", 1);
            BuildingDamageFrames.Add("v34", 1);
            BuildingDamageFrames.Add("v35", 1);
            BuildingDamageFrames.Add("v36", 1);
            BuildingDamageFrames.Add("v37", 1);
            BuildingDamageFrames.Add("tmpl", 5);
        }

        int Frame_From_Building_HP(StructureInfo s)
        {
            if (s.HP > 128) { return 0; }

            int Frame;
            BuildingDamageFrames.TryGetValue(s.Name, out Frame);

            return Frame;
        }

        void Load_Remap_Palettes()
        {
            int[] ShadowIndex = { 3, 4 };

            ColorRemaps.Add("Yellow", Palette.Load("data/" + Theater + "/" + PalName + ".pal", ShadowIndex));

            ColorRemaps.Add("Red", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex, 
                new RGB[] {
                               new RGB(240, 0, 0), // 127
                               new RGB(220, 20, 8), // 126
                               new RGB(196, 40, 20), // 125
                               new RGB(172, 52, 28), // 124
                               new RGB(120, 48, 36), // 122
                               new RGB(96, 8, 0), // 46
                               new RGB(56, 32, 20), // 120
                               new RGB(16, 0, 0), // 47
                               new RGB(196, 40, 20), // 125
                               new RGB(172, 52, 28), // 124
                               new RGB(152, 48, 36), // 123
                               new RGB(120, 48, 36), // 122
                               new RGB(112, 24, 0), // 42
                               new RGB(88, 44, 28), // 121
                               new RGB(56, 32, 20), // 120
                               new RGB(56, 32, 20), // 120                 
                            }));

            ColorRemaps.Add("Teal", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=2,119,118,135,136,138,112,12,118,135,136,137,138,139,114,112
                               new RGB(0, 168, 168), // 2
                               new RGB(116, 148, 156), // 119
                               new RGB(100, 128, 136), // 118
                               new RGB(0, 112, 112), // 135
                               new RGB(4, 92, 100), // 136
                               new RGB(16, 60, 80), // 138
                               new RGB(4, 4, 8), // 112
                               new RGB(0, 0, 0), // 12
                               new RGB(100, 128, 136), // 118
                               new RGB(0, 112, 112), // 135
                               new RGB(4, 92, 100), // 136
                               new RGB(8, 76, 92), // 137
                               new RGB(16, 60, 80), // 138
                               new RGB(20, 52, 72), // 139
                               new RGB(36, 44, 62), // 114
                               new RGB(4, 4, 8), // 112                 
                            }));
            ColorRemaps.Add("Orange", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=24,25,26,27,29,31,46,47,26,27,28,29,30,31,43,47
                               new RGB(236, 172, 72), // 24
                               new RGB(228, 148, 48), // 25
                               new RGB(212, 120, 16), // 26
                               new RGB(196, 96, 0), // 27
                               new RGB(164, 56, 0), // 29
                               new RGB(136, 24, 0), // 31
                               new RGB(96, 8, 0), // 46
                               new RGB(16, 0, 0), // 47
                               new RGB(212, 120, 16), // 26
                               new RGB(196, 96, 0), // 27
                               new RGB(180, 72, 0), // 28
                               new RGB(164, 56, 0), // 29
                               new RGB(152, 40, 0), // 30
                               new RGB(136, 24, 0), // 31
                               new RGB(112, 8, 0), // 43
                               new RGB(16, 0, 0), // 47          
                            }));

            ColorRemaps.Add("Green", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=5,165,166,167,159,142,140,199,166,167,157,3,159,143,142,141
                               new RGB(252, 252, 84), // 5
                               new RGB(208, 240, 0), // 165
                               new RGB(160, 224, 28), // 166
                               new RGB(140, 200, 8), // 167
                               new RGB(60, 152, 56), // 159
                               new RGB(60, 100, 56), // 142
                               new RGB(40, 68, 36), // 140
                               new RGB(24, 24, 24), // 199
                               new RGB(160, 224, 28), // 166
                               new RGB(140, 200, 8), // 167
                               new RGB(172, 176, 32), // 157
                               new RGB(0, 168, 0), // 3
                               new RGB(60, 152, 56), // 159
                               new RGB(60, 100, 56), // 142
                               new RGB(60, 100, 56), // 142
                               new RGB(48, 84, 44), // 141        
                            }));

            ColorRemaps.Add("Gray", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=161,200,201,202,204,205,206,12,201,202,203,204,205,115,198,114
                               new RGB(216, 252, 252), // 161
                               new RGB(220, 220, 228), // 200
                               new RGB(192, 192, 228), // 201
                               new RGB(164, 164, 188), // 202
                               new RGB(100, 100, 124), // 204
                               new RGB(72, 72, 92), // 205
                               new RGB(44, 44, 60), // 206
                               new RGB(0, 0, 0), // 12
                               new RGB(192, 192, 228), // 201
                               new RGB(164, 164, 188), // 202
                               new RGB(132, 132, 156), // 203
                               new RGB(100, 100, 124), // 204
                               new RGB(72, 72, 92), // 205
                               new RGB(56, 72, 76), // 115
                               new RGB(52, 52, 52), // 198
                               new RGB(36, 44, 52), // 114        
                            })); 
            
            ColorRemaps.Add("DarkGray", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=14,195,196,13,169,198,199,112,14,195,196,13,169,198,199,112
                               new RGB(168, 168, 168), // 14
                               new RGB(132, 132, 132), // 195
                               new RGB(108, 108, 108), // 196
                               new RGB(84, 84, 84), // 13
                               new RGB(72, 72, 72), // 169
                               new RGB(52, 52, 52), // 198
                               new RGB(24, 24, 24), // 199
                               new RGB(4, 4, 8), // 112
                               new RGB(168, 168, 168), // 14
                               new RGB(132, 132, 132), // 195
                               new RGB(108, 108, 108), // 196
                               new RGB(84, 84, 84), // 13
                               new RGB(72, 72, 72), // 169
                               new RGB(52, 52, 52), // 198
                               new RGB(24, 24, 24), // 199
                               new RGB(4, 4, 8), // 112        
                            }));
            ColorRemaps.Add("Brown", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=146,152,209,151,173,150,173,183,146,152,209,151,173,150,173,183
                               new RGB(180, 144, 80), // 146
                               new RGB(164, 120, 88), // 152
                               new RGB(168, 172, 76), // 209
                               new RGB(128, 92, 72), // 151
                               new RGB(112, 84, 8), // 173
                               new RGB(104, 76, 56), // 150
                               new RGB(112, 84, 8), // 173
                               new RGB(16, 12, 4), // 183
                               new RGB(180, 144, 80), // 146
                               new RGB(164, 120, 88), // 152
                               new RGB(168, 172, 76), // 209
                               new RGB(84, 84, 84), // 151
                               new RGB(112, 84, 8), // 173
                               new RGB(104, 76, 56), // 150
                               new RGB(112, 84, 8), // 173
                               new RGB(16, 12, 4), // 183      
                            }));

            ColorRemaps.Add("Fire", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=5,149,25,27,29,175,47,12,24,26,28,30,31,31,44,46
                               new RGB(252, 252, 84), // 5
                               new RGB(252, 208, 72), // 149
                               new RGB(228, 148, 48), // 25
                               new RGB(196, 96, 0), // 27
                               new RGB(164, 56, 0), // 29
                               new RGB(128, 16, 0), // 175
                               new RGB(16, 0, 0), // 47
                               new RGB(0, 0, 0), // 12
                               new RGB(236, 172, 72), // 24
                               new RGB(212, 120, 16), // 26
                               new RGB(180, 72, 0), // 28
                               new RGB(152, 40, 0), // 30
                               new RGB(136, 24, 0), // 31
                               new RGB(136, 24, 0), // 31
                               new RGB(96, 16, 0), // 44
                               new RGB(98, 8, 0), // 46      
                            }));
            ColorRemaps.Add("WarmSilver", Palette.Load_With_Remaps("data/" + Theater + "/" + PalName + ".pal", ShadowIndex,
                new RGB[] {
                // RemapIndexes=192,164,132,155,133,197,112,12,163,132,155,133,134,197,154,198
                               new RGB(216, 216, 216), // 192
                               new RGB(216, 208, 192), // 164
                               new RGB(176, 164, 132), // 132
                               new RGB(160, 144, 124), // 155
                               new RGB(144, 128, 116), // 133
                               new RGB(84, 84, 84), // 197
                               new RGB(4, 4, 8), // 112
                               new RGB(0, 0, 0), // 12
                               new RGB(208, 196, 172), // 163
                               new RGB(176, 164, 132), // 132
                               new RGB(160, 144, 124), // 155
                               new RGB(144, 128, 116), // 133
                               new RGB(116, 100, 100), // 134
                               new RGB(84, 84, 84), // 197
                               new RGB(64, 64, 64), // 154
                               new RGB(52, 52, 52), // 198    
                            }));
        }

        public void Load_House_Colors()
        {
            HouseColors.Add("badguy", new HouseInfo("Red", "Gray"));
            HouseColors.Add("goodguy", new HouseInfo("Yellow", "Yellow"));
            HouseColors.Add("special", new HouseInfo("Yellow", "Yellow"));
            HouseColors.Add("neutral", new HouseInfo("Yellow", "Yellow"));

            HouseColors.Add("multi1", new HouseInfo("Teal", "Teal"));
            HouseColors.Add("multi2", new HouseInfo("Orange", "Orange"));
            HouseColors.Add("multi3", new HouseInfo("Green", "Green"));
            HouseColors.Add("multi4", new HouseInfo("Gray", "Gray"));
            HouseColors.Add("multi5", new HouseInfo("Yellow", "Yellow"));
            HouseColors.Add("multi6", new HouseInfo("Red", "Red"));

            foreach (string section in MapINI.getSectionNames())
            {
                string sect = section.ToLower();
                if (HouseColors.ContainsKey(sect))
                {
//                    Console.WriteLine("section = {0}", sect);

                    HouseInfo House = new HouseInfo();
                    HouseColors.TryGetValue(sect, out House);

                    string SecondaryColor = MapINI.getStringValue(section, "SecondaryColorScheme", null);
                    if (SecondaryColor != null) Parse_Secondary_Color(ref House, SecondaryColor);

                    string PrimaryColor = MapINI.getStringValue(section, "PrimaryColorScheme", null);
                    if (PrimaryColor != null) Parse_Primary_Color(ref House, PrimaryColor);

                    HouseColors.Remove(sect);
                    HouseColors.Add(sect, House);
                }
            }

            HouseInfo BadGuy = new HouseInfo();
            HouseColors.TryGetValue("badguy", out BadGuy);

//            Console.WriteLine("PrimaryColor = {0}, SecondaryColor = {1}",
 //               BadGuy.PrimaryColor, BadGuy.SecondaryColor);
        }

        void Parse_Primary_Color(ref HouseInfo House, string color)
        {
            switch (color.ToLower())
            {
                case "gdi": 
                case "neutral":
                case "jurassic":
                    color = "Yellow"; break;
                case "nod": color = "Red"; break;
                default: break;
            }

            House.PrimaryColor = color;
        }

        void Parse_Secondary_Color(ref HouseInfo House, string color)
        {
            switch (color.ToLower())
            {
                case "gdi":
                case "neutral":
                case "jurassic":
                    color = "Yellow"; break;
                case "nod": color = "Gray"; break;
                case "none":
                    if (House.PrimaryColor == "Red") { color = "Red"; }
                    break;
                default: break;
            }

            House.SecondaryColor = color;
        }

        public Palette Remap_For_House(string HouseName, ColorScheme Scheme)
        {
            string Colour = "None";

            HouseInfo House = new HouseInfo();
            HouseColors.TryGetValue(HouseName.ToLower(), out House);

            switch (Scheme)
            {
                case ColorScheme.Primary: Colour = House.PrimaryColor; break;
                case ColorScheme.Secondary: Colour = House.SecondaryColor; break;
                default: break;
            }


            Palette Pal = this.Pal;
            ColorRemaps.TryGetValue(Colour, out Pal);
            return Pal;
        }

        public MapPreviewGenerator(string FileName)
        {

            MapINI = new IniFile(FileName);

            MapHeight = MapINI.getIntValue("Map", "Height", -1);
            MapWidth = MapINI.getIntValue("Map", "Width", -1);
            MapX = MapINI.getIntValue("Map", "X", -1);
            MapY = MapINI.getIntValue("Map", "Y", -1);

            Parse_Theater();
            Load_Remap_Palettes();
            Load_House_Colors();

            string MapBin = FileName.Replace(".ini", ".bin");

//            Console.WriteLine("MapBin = {0}, FileName = {1}", MapBin, FileName);

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
            Parse_Smudges();
            Parse_Units();
            Parse_Infantry();
            Parse_Structures();
            Parse_Cell_Triggers();

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

 //               Console.WriteLine("Waypoint = {0}, Index = {1}", WayPoint, CellIndex);
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

        void Parse_Cell_Triggers()
        {
            var SectionCellTriggers = MapINI.getSectionContent("CellTriggers");

            if (SectionCellTriggers != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionCellTriggers)
                {
                    int CellIndex = int.Parse(entry.Key);
                    string Name = entry.Value;

                    CellTriggerInfo c = new CellTriggerInfo();
                    c.Name = Name;
                    c.Y = CellIndex / 64;
                    c.X = CellIndex % 64;

                    CellsTriggers.Add(c);
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

            Theater = MapINI.getStringValue("Map", "Theater", "temperate");
            Theater = Theater.ToLower();

            switch (Theater)
            {
                case "winter": TheaterFilesExtension = ".win"; break;
                case "snow": TheaterFilesExtension = ".sno"; break;
                case "desert": TheaterFilesExtension = ".des"; break;
                default: TheaterFilesExtension = ".tem"; break;
            }

            PalName = "temperat";

            switch (Theater)
            {
                case "winter": PalName = "winter"; break;
                case "snow": PalName = "snow"; break;
                case "desert": PalName = "desert"; break;
                default: PalName = "temperat";  break;
            }

            int[] ShadowIndex = { 3, 4 };
            Pal = Palette.Load("data/" + Theater + "/" + PalName + ".pal", ShadowIndex);
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

 //                   Console.WriteLine("Unit name = {0}, side {1}, Angle = {2}, X = {3}, Y = {4}", u.Name,
   //                     u.Side, u.Angle, u.X, u.Y);
                }
            }
        }

        void Parse_Smudges()
        {
            var SectionSmudges = MapINI.getSectionContent("Smudges");
            if (SectionSmudges != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionSmudges)
                {
                    string SmudgesCommaString = entry.Value;
                    string[] SmudgesData = SmudgesCommaString.Split(',');

                    SmudgeInfo sm = new SmudgeInfo();
                    sm.Name = SmudgesData[0];
                    int CellIndex = int.Parse(SmudgesData[1]);
                    sm.Y = CellIndex / 64;
                    sm.X = CellIndex % 64;
                    sm.State = int.Parse(SmudgesData[2]);

                    Smudges.Add(sm);
                }
            }
        }

        void Parse_Structures()
        {
            var SectionStructures = MapINI.getSectionContent("Structures");
            if (SectionStructures != null)
            {
                foreach (KeyValuePair<string, string> entry in SectionStructures)
                {
                    string StructCommaString = entry.Value;
                    string[] StructData = StructCommaString.Split(',');

                    // 0=neutral,afld,256,6,0,none
                    StructureInfo s = new StructureInfo();
                    s.Name = StructData[1].ToLower();
                    s.Side = StructData[0];
                    s.Angle = int.Parse(StructData[4]);
                    s.HP = int.Parse(StructData[2]);
                    int CellIndex = int.Parse(StructData[3]);
                    s.Y = CellIndex / 64;
                    s.X = CellIndex % 64;

                    Structures.Add(s);

                    if (s.Name.ToLower() == "weap")
                    {
                        StructureInfo s2 = new StructureInfo();
                        s2.Name = "weap2";
                        s2.Side = s.Side;
                        s2.Angle = s.Angle;
                        s2.HP = s.HP;
                        s2.Y = s.Y;
                        s2.X = s.X;

                        Structures.Add(s2);
                    }

                    if (BuildingBibs.ContainsKey(s.Name))
                    {
                        BuildingBibInfo bi = new BuildingBibInfo();
                        BuildingBibs.TryGetValue(s.Name, out bi);

                        BibInfo bib = new BibInfo();
                        bib.Name = bi.Name; 
                        bib.X = s.X;
                        bib.Y = s.Y + bi.Yoffset;

                        Bibs.Add(bib);
                    }

 //                   Console.WriteLine("structure name = {0}, side {1}, HP = {5}, Angle = {2}, X = {3}, Y = {4}", s.Name,
   //                     s.Side, s.Angle, s.X, s.Y, s.HP);
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

//                    Console.WriteLine("infantry name = {0}, Side = {1}, Angle = {2}, SubCell = {5}, X = {3}, Y = {4}", inf.Name,
//                        inf.Side, inf.Angle, inf.X + subX, inf.Y + subY, inf.SubCell);
                }
            }
        }

        public Bitmap Get_Bitmap(bool OnlyDrawVisible)
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

                    if (data.Overlay != null)
                    {
                        Draw_Overlay(data, g, x, y);
                    }
                }
            }

            Draw_Smudges(g);
            Draw_Bibs(g);
            Draw_Structures(g);
            Draw_Units(g);
            Draw_Infantries(g);

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

            Draw_Waypoints(g);
            Draw_Cell_Triggers(g);

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (Is_Out_Of_Bounds(x, y))
                    {
                        Draw_Out_Of_Bounds(g, x, y);
                    }
                }
            }


            if (OnlyDrawVisible)
            {
                bitMap = Get_In_Bounds_Region(bitMap);
            }

            return bitMap;
        }

        Bitmap Get_In_Bounds_Region(Bitmap srcBitmap)
        {
            Rectangle section = new Rectangle(MapX * TemplateReader.TileSize,
                MapY * TemplateReader.TileSize, MapWidth * TemplateReader.TileSize,
                MapHeight * TemplateReader.TileSize);



            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Draw the specified section of the source bitmap to the new one
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        void Draw_Out_Of_Bounds(Graphics g, int x, int y)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(45, 0, 162, 232)),
                x * TemplateReader.TileSize, y * TemplateReader.TileSize,
                TemplateReader.TileSize, TemplateReader.TileSize);
        }

        void Draw_Text(Graphics g, string text, Font font, Brush brush, int x, int y)
        {
            RectangleF rectf = new RectangleF(x, y, 
                TemplateReader.TileSize, TemplateReader.TileSize);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
//            g.DrawString(text, new Font("Thaoma", 7), Brushes.GreenYellow, rectf);
            g.DrawString(text, font, brush, rectf);
        }

        void Draw_Rectangle(Graphics g, int x, int y)
        {
            Pen p = new Pen(Brushes.GreenYellow, 0.1f);
            g.DrawRectangle(p, x * TemplateReader.TileSize, y * TemplateReader.TileSize,
                TemplateReader.TileSize, TemplateReader.TileSize);
        }

        void Draw_Waypoints(Graphics g)
        {
            foreach (WaypointStruct wp in Waypoints)
            {
                string text = wp.Number.ToString();
                int X_Adjust = 8;
                if (text.Length == 2) X_Adjust = 5;

                Draw_Text(g, wp.Number.ToString(), new Font("Thaoma", 8), Brushes.GreenYellow, 
                    (TemplateReader.TileSize * wp.X) + X_Adjust, (wp.Y * TemplateReader.TileSize) + 6);
                Draw_Rectangle(g, wp.X, wp.Y);
            }

        }

        void Draw_Cell_Triggers(Graphics g)
        {
            foreach (CellTriggerInfo c in CellsTriggers)
            {
                Draw_Text(g, c.Name, new Font("Thaoma", 7), Brushes.Aqua,
                    c.X * TemplateReader.TileSize, (c.Y * TemplateReader.TileSize) + 6);
            }

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

            Palette Remap = Remap_For_House(u.Side, ColorScheme.Secondary);

            if (u.Name.ToLower() == "harv" || u.Name.ToLower() == "mcv")
            {
                Remap = Remap_For_House(u.Side, ColorScheme.Primary);
            }

            Bitmap UnitBitmap = RenderUtils.RenderShp(UnitShp, Remap, 
                Frame_From_Unit_Angle(u.Angle));

            Draw_Centered(g, UnitBitmap, u);

            // Draw vehicle turret
            string Name = u.Name.ToLower();
            if (Name == "htnk" || Name == "ltnk" || Name == "mtnk")
            {
                Bitmap TurretBitmap = RenderUtils.RenderShp(UnitShp, Remap,
                    Frame_From_Unit_Angle(u.Angle) + 32);

                Draw_Centered(g, TurretBitmap, u);
            }
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

            Bitmap TempBitmap = RenderUtils.RenderShp(InfShp, Remap_For_House(inf.Side, ColorScheme.Secondary), 
                Frame_From_Infantry_Angle(inf.Angle));

            int subX, subY;
            Sub_Cell_Pixel_Offsets(inf.SubCell, out subX, out subY);

            g.DrawImage(TempBitmap, inf.X * CellSize + subX, inf.Y * CellSize + subY, TempBitmap.Width, TempBitmap.Height);
        }

        void Draw_Structures(Graphics g)
        {
            foreach (StructureInfo s in Structures)
            {
                Draw_Structure(s, g);
            }
        }

        void Draw_Structure(StructureInfo s, Graphics g)
        {
            string FileName = General_File_String_From_Name(s.Name);

            if (!File.Exists(FileName))
            {
                FileName = File_String_From_Name(s.Name);
            }

            ShpReader StructShp = ShpReader.Load(FileName);

            Bitmap StructBitmap = RenderUtils.RenderShp(StructShp, Remap_For_House(s.Side, ColorScheme.Primary), 
                Frame_From_Building_HP(s));

            g.DrawImage(StructBitmap, s.X * CellSize, s.Y * CellSize, StructBitmap.Width, StructBitmap.Height);
        }

        void Draw_Smudges(Graphics g)
        {
            foreach (SmudgeInfo sm in Smudges)
            {
                Draw_Smudge(sm, g);
            }
        }

        void Draw_Smudge(SmudgeInfo sm, Graphics g)
        {
            TemplateReader SmudgeTemp = TemplateReader.Load(General_File_String_From_Name(sm.Name));

            Bitmap StructBitmap = RenderUtils.RenderTemplate(SmudgeTemp, Pal, sm.State);

            g.DrawImage(StructBitmap, sm.X * CellSize, sm.Y * CellSize, StructBitmap.Width, StructBitmap.Height);
        }


        void Draw_Bibs(Graphics g)
        {
            foreach (BibInfo bib in Bibs)
            {
                Draw_Bib(bib, g);
            }
        }

        void Draw_Bib(BibInfo bib, Graphics g)
        {
            ShpReader BibShp = ShpReader.Load(File_String_From_Name(bib.Name));
            int Frame = 0;

            int maxY = -1; int maxX = -1;
            switch (bib.Name.ToLower())
            {
                case "bib1": maxY = 2; maxX = 4; break;
                case "bib2": maxY = 2; maxX = 3; break;
                case "bib3": maxY = 2; maxX = 2; break;
                default: break;
            }

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    Bitmap StructBitmap = RenderUtils.RenderShp(BibShp, Pal, Frame);

                    g.DrawImage(StructBitmap, (bib.X + x) * CellSize, (bib.Y + y) * CellSize, StructBitmap.Width, StructBitmap.Height);

                    Frame++;
                }
            }
        }

        void Draw_Template(CellStruct Cell, Graphics g, int X, int Y)
        {
          if (Cell.Template == 255  && Cell.Tile == 0) 
          {
              Cell.Tile = MapRandom.Next(0, 15);
          }

            string TemplateString = TilesetsINI.getStringValue("TileSets", Cell.Template.ToString(), "CLEAR1");

            TemplateReader Temp = TemplateReader.Load(File_String_From_Name(TemplateString));

            Bitmap TempBitmap = RenderUtils.RenderTemplate(Temp, Pal, Cell.Tile);
            g.DrawImage(TempBitmap, X * CellSize, Y * CellSize, TempBitmap.Width, TempBitmap.Height);
        }

        string File_String_From_Name(string Name)
        {
            return ("data/" + Theater + "/" + Name + TheaterFilesExtension);
        }

        string General_File_String_From_Name(string Name)
        {
            return ("data/general/" + Name + ".shp");
        }

        void Draw_Overlay(CellStruct Cell, Graphics g, int X, int Y)
        {
            string Overlay = Cell.Overlay.ToLower();
            int Frame = 0;

            if (TiberiumStages.ContainsKey(Overlay.ToLower()))
            {
                Frame = -1;
                TiberiumStages.TryGetValue(Overlay.ToLower(), out Frame);
                int index = MapRandom.Next(1, 12); // creates a number between 1 and 12
                Overlay = string.Format("TI{0}", index);
            }

            string FilePath = File_String_From_Name(Overlay);

            if (!File.Exists(FilePath))
            {
                FilePath = General_File_String_From_Name(Overlay);
            }

            ShpReader Shp = ShpReader.Load(FilePath);

            if (Is_Fence(Overlay))
            {
                Frame = Frame_For_Fence(Overlay, X, Y);
            }

            Bitmap ShpBitmap = RenderUtils.RenderShp(Shp, Pal, Frame);
            g.DrawImage(ShpBitmap, X * CellSize, Y * CellSize, ShpBitmap.Width, ShpBitmap.Height);
        }

        int Frame_For_Fence(string Name, int X, int Y)
        {
            bool Top = Cell_Contains_Same_Overlay(X, Y - 1, Name);
            bool Bottom = Cell_Contains_Same_Overlay(X, Y + 1, Name);
            bool Left = Cell_Contains_Same_Overlay(X - 1, Y, Name);
            bool Right = Cell_Contains_Same_Overlay(X + 1, Y, Name);

            if (Top == true && Bottom == true && Left == true && Right == true)
            {
                return 15;
            }

            if (Top == true && Left == true && Right == true) { return 11; }
            if (Top == true && Right == true && Bottom == true) { return 7; }
            if (Top == true && Left == true && Bottom == true) { return 13; }
            if (Right == true && Left == true && Bottom == true) { return 13; }

            if (Top == true && Right == true) { return 3; }
            if (Bottom == true && Right == true) { return 6; }
            if (Bottom == true && Top == true) { return 5; }
            if (Top == true && Left == true) { return 9; }
            if (Right == true && Left == true) { return 10; }
            if (Left == true && Bottom == true) { return 12; }

            if (Top == true) { return 1; }
            if (Bottom == true) { return 4; }
            if (Left == true) { return 8; }
            if (Right == true) { return 2; }

            return 0;
        }

        bool Cell_Contains_Same_Overlay(int X, int Y, string Name)
        {
            if (Y < 0 || X < 0) return false;
            if (Y > 64 || X > 64) return false;

            if (Cells[X, Y].Overlay == null) return false;
            if (Cells[X, Y].Overlay.ToLower() == Name) return true;

            return false;
        }

        bool Is_Fence(string Name)
        {
            bool ret = false;

            switch (Name.ToLower())
            {
                case "barb":
                case "wood":
                case "sbag":
                case "cycl":
                case "brik":
                ret = true; break;
                default: break;
            }

            return ret;
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

        int Frame_From_Infantry_Angle(int Angle)
        {
            //            Console.WriteLine("Angle = {0}", Angle);

            if (Angle == 0) { return 0; }

            if (Angle > 224) { return 0; }
            if (Angle > 192) { return 1; }
            if (Angle > 160) { return 2; }
            if (Angle > 128) { return 3; }
            if (Angle > 96) { return 4; }
            if (Angle > 64) { return 5; }
            if (Angle > 32) { return 6; }
            if (Angle > 0) { return 7; }

            return -1;
        }

        int Frame_From_Unit_Angle(int Angle)
        {
//            Console.WriteLine("Angle = {0}", Angle);

            if (Angle== 0) { return 0; }

            if (Angle > 248) { return 0; }
            if (Angle > 240) { return 1; }
            if (Angle > 232) { return 2; }
            if (Angle > 224) { return 3; }
            if (Angle > 216) { return 4; }
            if (Angle > 208) { return 5; }
            if (Angle > 200) { return 6; }
            if (Angle > 192) { return 7; }
            if (Angle > 184) { return 8; }
            if (Angle > 176) { return 9; }
            if (Angle > 168) { return 10; }
            if (Angle > 160) { return 11; }
            if (Angle > 152) { return 12; }
            if (Angle > 144) { return 13; }
            if (Angle > 136) { return 14; }
            if (Angle > 128) { return 15; }
            if (Angle > 120) { return 16; }
            if (Angle > 112) { return 17; }
            if (Angle > 104) { return 18; }
            if (Angle > 96) { return 19; }
            if (Angle > 88) { return 20; }
            if (Angle > 80) { return 21; }
            if (Angle > 72) { return 22; }
            if (Angle > 64) { return 23; }
            if (Angle > 56) { return 24; }
            if (Angle > 48) { return 25; }
            if (Angle > 40) { return 26; }
            if (Angle > 32) { return 27; }
            if (Angle > 24) { return 28; }
            if (Angle > 16) { return 29; }
            if (Angle > 8) { return 30; }
            if (Angle > 0) { return 31; }

            return -1;
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
    struct StructureInfo
    {
        public string Name;
        public string Side;
        public int Angle;
        public int X;
        public int Y;
        public int HP;
    }
    struct BibInfo
    {
        public string Name;
        public int X;
        public int Y;
    }
    struct SmudgeInfo
    {
        public string Name;
        public int X;
        public int Y;
        public int State;
    }

    struct BuildingBibInfo
    {
        public string Name;
        public int Yoffset;

        public BuildingBibInfo(string _Name, int _Yoffset)
        {
            Name = _Name;
            Yoffset = _Yoffset;
        }
    }

    struct HouseInfo
    {
        public string PrimaryColor;
        public string SecondaryColor;

        public HouseInfo(string _PrimaryColor, string _SecondaryColor)
        {
            SecondaryColor = _SecondaryColor;
            PrimaryColor = _PrimaryColor;
        }
    }

    struct CellTriggerInfo
    {
        public string Name;
        public int X;
        public int Y;
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
    public struct RGB
    {
        public byte R;
        public byte G;
        public byte B;

        public RGB(byte R_, byte G_, byte B_)
        {
            R = R_;
            G = G_;
            B = B_;
        }
    }
    enum ColorScheme
    {
        Primary,
        Secondary
    }
}
