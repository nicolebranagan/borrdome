#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    partial class Program
    {
        static public bool DEBUG = false;

        static public Random random = new Random();
        public static List<Species> species;
        public static List<Item> items;
        public static List<Spell> spells;

        static public Map themap;
        public static List<Entity> entities;
        public static List<Thing> things; // Things on the board

        static public ConsoleKeyInfo LastKey;

        public static int turns = 0;
        /*public static string bottombar_label = "Welcome to dungeon!";
        public static int bottombar_timer = 4;*/

        public static int currentlevel = 1;
        public static int newlevel = 0;
        public static bool newlevelflag = false;
        public static bool climbfrombackwards = false;

        static void Main(string[] args)
        {
            Console.Initialize();
            Colors.Initialize();
            DungeonColors.initialize();
            // The rightmost 20 pixels form a status area

            LoadContent();

            entities = new List<Entity>();
            bool newgame = Welcome();

            if (newgame) NewDungeonLevel(1, 0);

            while (true)
                GameLoop();
        }

        static void LoadContent()
        {
            species = new List<Species>();
            var specDoc = new XmlDocument();
            specDoc.Load("species.xml");

            foreach (var entry in specDoc.GetElementsByTagName("Species"))
                species.Add(Species.FromXmlElement((XmlElement)entry));

            spells = new List<Spell>();
            specDoc.Load("spells.xml");

            foreach (var entry in specDoc.GetElementsByTagName("Spell"))
                spells.Add(Spell.FromXmlElement((XmlElement)entry));

            items = new List<Item>();
            specDoc.Load("item.xml");

            foreach (var entry in specDoc.GetElementsByTagName("Item"))
                items.Add(Item.FromXmlElement((XmlElement)entry));

            foreach (Species u in species)
                u.fillDroplist();
        }

        static void NewDungeonLevel(int level, int priorlevel)
        {
            Entity Backup = entities[0];

            if (priorlevel != 0)
                StoredLevel.Thaw(priorlevel);

            StoredLevel.Unthaw(level, Backup);

            if (climbfrombackwards)
            { entities[0].x = themap.end.x; entities[0].y = themap.end.y; }
            else
            { entities[0].x = themap.start.x; entities[0].y = themap.start.y; }

            DrawGame();
        }

        public static void DrawGame()
        {
            themap.DrawWorld();

            ClearSidebar();
            BuildSidebar();

            foreach (Thing u in things)
                u.Draw();

            foreach (Entity u in Program.entities)
                u.Draw();
        }

        static void WipeGame()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            /*for (int i = 0; i < 60; i++)
                for (int j = 0; j < 25; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write(" ");
                }*/
            Console.Clear();
            BuildSidebar(false);
        }

        static void GameLoop()
        {
            BuildSidebar();
            LastKey = Console.ReadKey(true);
            if (Program.LastKey.Key == ConsoleKey.F1)
            {
                Program.HelpScreen();
                LastKey = Console.ReadKey(true);
            }
            else if (Program.LastKey.Key == ConsoleKey.Escape)
            {
                StoredLevels.Save();
                Environment.Exit(0);
            }
            else
                Program.CheatMonitor();

            themap.IterateSmellMap();
            turns = turns + 1;

            foreach (Thing u in things)
            {
                u.Draw();
            }
            foreach (Entity u in entities)
            {
                u.Loop();
                u.Draw();
            }
            entities.RemoveAll(item => (item.health <= 0) && (item != entities[0]));

            if (newlevelflag)
            {
                NewDungeonLevel(newlevel, currentlevel);
                currentlevel = newlevel;
                newlevelflag = false;
            }
        }

        static bool UseLastMsg = true;

        private class report
        {
            public int index;
            string message;
            public int timer;

            public report(string message)
            {
                this.message = message;
                this.timer = 5;
                this.index = 0;
                foreach (report u in Program.reports)
                {
                    if (u.index >= this.index)
                        this.index = u.index + 1;
                }
            }

            public override string ToString()
            {
                return message;
            }
        }

        static List<report> reports = new List<report>();

        public static void Report(string report, bool isSystemMessage = false)
        {
            /*
            if ((bottombar_timer > 0) && !isSystemMessage && UseLastMsg)
            {
                bottombar_label = String.Concat(report, "; ", bottombar_label);
                if (bottombar_label.Length > 58)
                    bottombar_label = bottombar_label.Substring(0, 58);
            }
            else
                bottombar_label = report;

            if (isSystemMessage) UseLastMsg = false; else UseLastMsg = true;

            bottombar_timer = 5;
            themap.DrawRow(24);
            BuildSidebar();*/
            if (isSystemMessage || !UseLastMsg) reports = new List<report>();
            if (isSystemMessage) UseLastMsg = false; else UseLastMsg = true;

            reports.Insert(0, new report(report));
            ShowReport();
        }

        static void ShowReport()
        {
            int column = 0;
            string message;

            if (reports.Count == 0)
            {
                themap.DrawRow(24);
                return;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGray;

            reports.RemoveAll(item => item.timer <= 0);

            foreach (report u in reports)
            {
                if ((column != 0) && (column != 59))
                {
                    Console.SetCursorPosition(column, 24);
                    Console.Write(";");
                    column = column + 1;
                }

                if (column > 59)
                    break;

                message = string.Concat(" ", u.ToString());

                if ((message.Length + column) > 59)
                {
                    message = message.Substring(0, 60 - column);
                }

                if (u.timer < 3)
                    Console.ForegroundColor = ConsoleColor.Gray;
                if (u.timer < 2)
                    Console.BackgroundColor = ConsoleColor.Black;

                Console.SetCursorPosition(column, 24);
                Console.Write(message);

                u.timer = u.timer - 1;
                column = column + message.Length;
            }
            if ((column < 59) && (column != 0))
                Console.Write(" ");

            themap.DrawRow(24, column + 1);

            /*Console.SetCursorPosition(column, 24);
            if (bottombar_timer > 0)
            {
                if (bottombar_timer < 3)
                    Console.ForegroundColor = ConsoleColor.Gray;
                if (bottombar_timer < 2)
                    Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(String.Concat(" ", bottombar_label, " "));
                bottombar_timer = bottombar_timer - 1;
            }
            else
            {
                themap.DrawRow(24);
            }*/
        }

        static void ClearSidebar()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < 24; i++)
            {
                Console.SetCursorPosition(60, i);
                Console.Write("|                   ");
            }
            Console.SetCursorPosition(60, 24);
            Console.Write("|                  ");
        }

        static void BuildSidebar(bool TopAndBottomBar = true)
        {
            if (TopAndBottomBar)
            {
                // Top bar
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkGray;

                Console.SetCursorPosition(0, 0);
                Console.Write(String.Concat(" Health: ", entities[0].health.ToString(), " Food: ", entities[0].food.ToString(), " "));

                // Bottom Bar
                /*Console.SetCursorPosition(0, 24);
                if (bottombar_timer > 0)
                {
                    if (bottombar_timer < 3)
                        Console.ForegroundColor = ConsoleColor.Gray;
                    if (bottombar_timer < 2)
                        Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(String.Concat(" ", bottombar_label, " "));
                    bottombar_timer = bottombar_timer - 1;
                }
                else
                {
                    themap.DrawRow(24);
                }*/
                ShowReport();
            }

            // Begin Sidebar

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            for (int i = 0; i < 25; i++)
            {
                Console.SetCursorPosition(60, i);
                Console.Write("|");
            }

            int row = 0;
            if (DEBUG)
            {
                Console.SetCursorPosition(61, row);
                Console.Write("DEBUG");
            }

            row = row + 1;
            Console.SetCursorPosition(64, row);
            Console.Write("- Borr-Dome -");
            row = row + 1;
            Console.SetCursorPosition(66, row);
            string levelname = currentlevel.ToString().PadLeft(2, '0');
            Console.Write(String.Concat("Level: ", levelname));

            row = row + 2;
            Console.SetCursorPosition(62, row);
            Console.Write("Player: ");

            row = row + 1;
            Console.SetCursorPosition(63, row);
            Console.ForegroundColor = entities[0].species.foregroundcolor;
            Console.BackgroundColor = entities[0].species.backgroundcolor;
            Console.Write("@");
            Console.SetCursorPosition(65, row);
            switch (entities[0].status)
            {
                case Entity.statuses.Charmed:
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    }
                case Entity.statuses.Poison:
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    }
                case Entity.statuses.Paralyzed:
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    }
                default:
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            if (entities[0].species.gendered)
                Console.Write(String.Concat(entities[0].GenderMarker(), " ", Helper.FormatString(entities[0].species.ToString(), 13)));
            else
                Console.Write(Helper.FormatString(entities[0].species.ToString(), 15));
            Console.ForegroundColor = ConsoleColor.Gray;

            row = row + 1;
            Console.SetCursorPosition(65, row);
            Console.Write(String.Concat("ATK ", entities[0].attack.ToString(), "  "));
            row = row + 1;
            Console.SetCursorPosition(65, row);
            Console.Write(String.Concat("DEF ", entities[0].defense.ToString(), "  "));
            /*
            row = row + 2;
            Console.SetCursorPosition(62, row);
            Console.Write("Other Actors: ");
            for (int i = 0; i < themap.usedspecies.Count; i++)
            {
                row = row + 1;
                if ((row + 3) > 25)
                    break;
                Console.SetCursorPosition(63, row);

                Console.ForegroundColor = themap.usedspecies[i].foregroundcolor;
                Console.BackgroundColor = themap.usedspecies[i].backgroundcolor;
                Console.Write(themap.usedspecies[i].face);
                Console.SetCursorPosition(65, row);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(Helper.FormatString(themap.usedspecies[i].ToString(), 15));
            }

            if ((row + 3) < 25)
            {
                row = row + 1;
                Console.SetCursorPosition(60, row);
                Console.Write("|                   ");
                row = row + 1;
                Console.SetCursorPosition(62, row);
                Console.Write("Items:            ");
                for (int j = 0; j < themap.useditems.Count; j++)
                {
                    row = row + 1;
                    if ((row + 3) > 25)
                        break;
                    Console.SetCursorPosition(63, row);

                    Console.ForegroundColor = themap.useditems[j].foregroundcolor;
                    Console.BackgroundColor = themap.useditems[j].backgroundcolor;
                    Console.Write(themap.useditems[j].face);
                    Console.SetCursorPosition(65, row);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(Helper.FormatString(themap.useditems[j].name, 15));
                }

                if ((row + 3) < 25)
                {
                    for (int i = 0; i < (25 - (row + 3)); i++)
                    {
                        row = row + 1;
                        Console.SetCursorPosition(63, row);
                        Console.Write(Helper.FormatString(" ", 16));
                    }
                }
                else
                {
                    //row = row + 1;
                    Console.SetCursorPosition(63, row);
                    Console.Write(Helper.FormatString(" etc.", 16));
                }
            }
            else
            {
                //row = row + 1;
                Console.SetCursorPosition(63, row);
                Console.Write(Helper.FormatString(" etc.", 16));
            }*/
            row = row + 2;
            Console.SetCursorPosition(62, row);
            Console.Write("(I)nventory:");
            for (int j = 0; j < Program.entities[0].inventory.Count; j++)
            {
                row = row + 1;
                if ((row + 3) > 25)
                    break;
                Console.SetCursorPosition(63, row);

                Console.ForegroundColor = Program.entities[0].inventory[j].item.foregroundcolor;
                Console.BackgroundColor = Program.entities[0].inventory[j].item.backgroundcolor;
                Console.Write(Program.entities[0].inventory[j].item.face);
                Console.SetCursorPosition(65, row);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(Helper.FormatString(Program.entities[0].inventory[j].name, 15));
            }

            if ((row + 3) < 25)
            {
                for (int i = 0; i < (25 - (row + 3)); i++)
                {
                    row = row + 1;
                    Console.SetCursorPosition(63, row);
                    Console.Write(Helper.FormatString(" ", 16));
                }
            }
            else
            {
                //row = row + 1;
                Console.SetCursorPosition(63, row);
                Console.Write(Helper.FormatString(" etc.", 16));
            }
        }
    }
}