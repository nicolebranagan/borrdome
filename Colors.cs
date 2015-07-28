using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    public class Colors
    {
        class ColorSet
        {
            List<int> colorlist = new List<int>();

            ConsoleColor[] colormatch;
            ConsoleColor[] darkcolor;

            public ColorSet()
            {
                colorlist.Add(1); colorlist.Add(2); colorlist.Add(3); colorlist.Add(4); colorlist.Add(5);
                colorlist.Add(6); colorlist.Add(7);

                colormatch = new ConsoleColor[7];
                darkcolor = new ConsoleColor[7];
                int rand;

                for (int i = 0; i < 7; i++)
                {
                    rand = Program.random.Next(0, colorlist.Count());
                    colormatch[i] = (ConsoleColor)(colorlist[rand] + 8);
                    darkcolor[i] = (ConsoleColor)(colorlist[rand]);
                    colorlist.Remove(colorlist[rand]);
                }
            }

            public ConsoleColor LightColor(int i)
            {
                return colormatch[i];
            }

            public ConsoleColor DarkColor(int i)
            {
                return darkcolor[i];
            }
        }

        static List<ColorSet> colorsets;

        public static void Initialize()
        {
            colorsets = new List<ColorSet>();
            colorsets.Add(new ColorSet());
        }

        public static ConsoleColor getColor(string identifier)
        {
            bool dark = false;
            if (identifier.Length > 2)
            {
                int ColorSetNumber = int.Parse(identifier.Substring(8,1));
                if (identifier.Substring(9, 1) == "*") dark = true;
                int ColorNumber = int.Parse(identifier.Substring(10, 1));

                if (ColorSetNumber >= colorsets.Count)
                {
                    for (int i = 0; i <= (ColorSetNumber - colorsets.Count); i++)
                        colorsets.Add(new ColorSet());
                }

                if (dark)
                    return colorsets[ColorSetNumber].DarkColor(ColorNumber);
                else
                    return colorsets[ColorSetNumber].LightColor(ColorNumber);
            }
            else
                return (ConsoleColor)int.Parse(identifier);
        }

        public static string getColorName(int namingway)
        {
            switch (namingway)
            {
                case 0: { return "Black"; }
                case 1: { return "Blue"; }
                case 2: { return "Green"; }
                case 3: { return "Cyan"; }
                case 4: { return "Red"; }
                case 5: { return "Magenta"; }
                case 6: { return "Yellow"; }
                case 7: { return "Grey"; }
                case 8: { return "Grey"; }
                case 9: { return "Blue"; }
                case 10: { return "Green"; }
                case 11: { return "Cyan"; }
                case 12: { return "Red"; }
                case 13: { return "Magenta"; }
                case 14: { return "Yellow"; }
                case 15: { return "White"; }
                default: { return "Bland"; }
            }
        }

    }

    [Serializable]
    public class DungeonColors
    {
        public ConsoleColor FloorColor1 { get; private set; }
        public ConsoleColor FloorColor2 { get; private set; }
        public ConsoleColor WallColor1 { get; private set; }
        public ConsoleColor WallColor2 { get; private set; }
        public ConsoleColor ElseColor { get; private set; }

        private DungeonColors(ConsoleColor FloorColor1, ConsoleColor FloorColor2, ConsoleColor WallColor1, ConsoleColor WallColor2, 
            ConsoleColor ElseColor)
        {
            this.FloorColor1 = FloorColor1;
            this.FloorColor2 = FloorColor2;
            this.WallColor1 = WallColor1;
            this.WallColor2 = WallColor2;
            this.ElseColor = ElseColor;
        }

        static List<DungeonColors> possiblecolors = new List<DungeonColors>();

        public static void initialize()
        {
            possiblecolors.Add(new DungeonColors(
                ConsoleColor.DarkGray, ConsoleColor.Gray,
                ConsoleColor.White, ConsoleColor.Gray,
                ConsoleColor.Gray));

            possiblecolors.Add(new DungeonColors(
                ConsoleColor.DarkBlue, ConsoleColor.DarkGray,
                ConsoleColor.Cyan, ConsoleColor.DarkCyan,
                ConsoleColor.Gray));

            possiblecolors.Add(new DungeonColors(
                ConsoleColor.DarkGreen, ConsoleColor.Green,
                ConsoleColor.DarkMagenta, ConsoleColor.Magenta,
                ConsoleColor.Gray));

            possiblecolors.Add(new DungeonColors(
                ConsoleColor.DarkGray, ConsoleColor.DarkYellow,
                ConsoleColor.DarkYellow, ConsoleColor.Yellow,
                ConsoleColor.Gray));

            possiblecolors.Add(new DungeonColors(
                ConsoleColor.DarkGray, ConsoleColor.DarkRed,
                ConsoleColor.DarkRed, ConsoleColor.Red,
                ConsoleColor.Gray));
        }

        public static DungeonColors newColor()
        {
            int rand = Program.random.Next(0, possiblecolors.Count);
            return possiblecolors[rand];
        }
    }
}
