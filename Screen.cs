using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    partial class Program
    {
        public static void HelpScreen()
        {
            WipeGame();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Command Help");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("(F1) - This help screen");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("Arrow Keys - Move or Attack in four directions");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Vi-keys or Numpad - Move or Attack in eight directions");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("(P) - Peer (look) at something nearby");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(G) - Grab something nearby");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(C) - Try to charm an opponent");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("(I) - Enter the inventory management screen");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(E) - Equip or unequip an item in your inventory");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(L) - Look at an item in your inventory");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(D) - Drop an item from your inventory");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(T) - Throw an item from your inventory");


            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("(M) - Enter the magic management screen");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("All other keys rest.");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("Press any key to return to the game.");
            Console.ReadKey(true);
            DrawGame();
        }

        public static void DeathScreen(string murderer)
        {
            // Clearing stuff out in preparation

            themap.DrawRow(24);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            for (int i = 0; i < 60; i++)
                for (int j = 0; j < 6; j++)
                {
                    Console.SetCursorPosition(i, j);
                    if (j != 5)
                        Console.Write(" ");
                    else
                        Console.Write("_");
                }

            // Begin

            int row = 0;

            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("You have died.");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write(String.Concat("Killed by ",murderer," on level ",currentlevel.ToString(),"."));

            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Try harder next time!");

            while (true)
            {
            }
        }

        public static void EndScreen()
        {
            WipeGame();
            int score = 0;

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("You have left the Borr-Dome alive, well done.");
            row = row + 2;
            Console.SetCursorPosition(1, row);

            if (WinCondition())
            {
                score = score + 1000;
                Console.Write("And you have obtained the three scales which you sought.");
                row = row + 1;
                Console.SetCursorPosition(1, row);
                Console.Write("Well done, you have earned 1000 points.");
            }
            else
                Console.Write("But you have failed your mission.");

            if (Program.entities[0].species != species[0])
            {
                score = score - 200;
                row = row + 2;
                Console.SetCursorPosition(1, row);
                Console.Write("Upon leaving, you pay a wizard 200 gold to restore");
                row = row + 1;
                Console.SetCursorPosition(1, row);
                Console.Write("yourself to humanity.");
            }

            if (Program.entities[0].inventory.Count != 0)
            {
                int money = GetValue();
                score = score + money;
                row = row + 2;
                Console.SetCursorPosition(1, row);
                Console.Write(String.Concat("You earn ", money.ToString(), " gold selling your items."));
            }

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write(String.Concat("Total score: ", score.ToString()));

            while (true)
            {
            }
        }

        private static bool WinCondition()
        {
            List<InventoryItem> inv = Program.entities[0].inventory;
            bool alpha = false;
            bool beta = false;
            bool gamma = false;
            
            foreach (InventoryItem u in inv)
            {
                if (u.item.truename == "Alpha Dragon Scale")
                    alpha = true;
                else if (u.item.truename == "Beta Dragon Scale")
                    beta = true;
                else if (u.item.truename == "Gamma Dragon Scale")
                    gamma = true;
            }

            return (alpha && beta && gamma);
        }

        private static int GetValue()
        {
            List<InventoryItem> inv = Program.entities[0].inventory;
            int value = 0;
            foreach (InventoryItem u in inv)
            {
                value = value + u.item.value;
            }
            return value;
        }

        public static void InventoryScreen()
        {
            WipeGame();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Inventory:");
            row = row + 1;


            InventoryItem test = GetItem(row, true);

            if (test != null)
            {
                if (test.useable)
                {
                    Report(String.Concat("Used ", test.name, "!"));
                    test.Use(entities[0]);
                    entities[0].inventory.Remove(test);
                }
                else
                {
                    Report(String.Concat("Can't use!"));
                }
                DrawGame();
            }
            else
            {
                ConsoleKey command = Program.LastKey.Key;
                if (command == ConsoleKey.E)
                {
                    EquipScreen();
                }
                else if (command == ConsoleKey.D)
                {
                    DropScreen();
                }
                else if (command == ConsoleKey.L)
                {
                    LookScreen();
                }
                else if (command == ConsoleKey.T)
                {
                    ThrowScreen();
                }
                else
                    DrawGame();
            }
        }

        public static void EquipScreen()
        {
            WipeGame();

            int row = 1;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(1, row);
            Console.Write(Helper.FormatString("Equip which item?", 58));
            InventoryItem test = GetItem(row + 1);

            if (test != null)
            {
                entities[0].Equip(test);
            }
                DrawGame();
        }

        public static void DropScreen()
        {
            WipeGame();

            int row = 1;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(1, row);
            Console.Write(Helper.FormatString("Drop which item?", 58));
            InventoryItem test = GetItem(row + 1);

            if (test != null)
                if (test.equipped == false)
                {
                    Program.things.Add(new ItemThing(Program.entities[0].x, Program.entities[0].y, test.item));
                    Program.entities[0].inventory.Remove(test);
                }
                else
                    Program.Report("Can't drop equipped item!");

            DrawGame();
        }

        public static void LookScreen()
        {
            WipeGame();

            int row = 1;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(1, row);
            Console.Write(Helper.FormatString("Look at which item?", 58));
            InventoryItem test = GetItem(row + 1);

            if (test != null)
            {
                Program.Report(test.item.description);
            }

            DrawGame();
        }

        public static void ThrowScreen()
        {
            WipeGame();

            int row = 1;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(1, row);
            Console.Write(Helper.FormatString("Throw which item?", 58));
            InventoryItem test = GetItem(row + 1);

            if (test != null)
                if (test.item.throwable == true)
                {
                    Program.Report("Throw where?", true);
                    Entity target = TargetEntity();
                    if (target != null)
                    {
                        test.item.Throw(target);
                        Program.entities[0].inventory.Remove(test);
                    }
                }
                else
                {
                    Program.Report("Can't throw that!");
                }
            DrawGame();
        }

        private static InventoryItem GetItem(int row, bool msg = false, int selected = 0)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Dictionary<ConsoleKey, InventoryItem> dict = BuildItemDictionary();
            int initrow = row;
            row = WriteInventory(row, selected);

            if (msg)
            {
                row = row + 2;
                Console.SetCursorPosition(1, row);
                Console.Write("(L)ook, (E)quip/Unequip, (D)rop, (T)hrow");
                row = row + 2;
                Console.SetCursorPosition(1, row);
                Console.Write("Press any number to use,");
                row = row + 1;
                Console.SetCursorPosition(1, row);
                Console.Write("or Up/Down + Enter or Spacebar to choose an item.");
            }

            Program.LastKey = Console.ReadKey(true);
            ConsoleKey command = LastKey.Key;
            if (dict.ContainsKey(command))
            {
                return dict[command];
            }
            else if (LastKey.Key == ConsoleKey.UpArrow)
            {
                selected = Math.Max(selected - 1, 0);
                return GetItem(initrow, msg, selected);
            }
            else if (LastKey.Key == ConsoleKey.DownArrow)
            {
                selected = Math.Min(selected + 1, Program.entities[0].inventory.Count - 1);
                return GetItem(initrow, msg, selected);
            }
            else if (((LastKey.Key == ConsoleKey.Spacebar) || (LastKey.Key == ConsoleKey.Enter)) && (Program.entities[0].inventory.Count != 0))
            {
                return Program.entities[0].inventory[selected];
            }
            else
                return null;
        }

        private static Dictionary<ConsoleKey, InventoryItem> BuildItemDictionary()
        {
            Dictionary<ConsoleKey, InventoryItem> dict = new Dictionary<ConsoleKey,InventoryItem>();
            ConsoleKey[] commands = new ConsoleKey[] {ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5,
            ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9};
            if (entities[0].inventory.Count != 0)
            {
                for (int i = 0; i < entities[0].inventory.Count; i++)
                {
                    dict.Add(commands[i], entities[0].inventory[i]);
                }
            }
            return dict;
        }

        /*public static void MagicScreen()
        {
            WipeGame();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Current Spells:");
            row = row + 1;
            Dictionary<ConsoleKey, LearnedSpell> dict = BuildSpellDictionary();
            //row = WriteSpells(row);


            ConsoleKey command = Console.ReadKey(true).Key;
            if (dict.ContainsKey(command))
            {
                dict[command].Cast(Program.entities[0]);
            }
            else if (command == ConsoleKey.F)
            {
                Console.SetCursorPosition(1, row);
                Console.Write(Helper.FormatString("Forget which spell?", 58));
                LearnedSpell test = GetSpell(dict);
                if (test != null)
                    entities[0].Forget(test);
            }
            else
                Program.Report("Not a spell!");

            DrawGame();
        }*/

        public static void MagicScreen()
        {
            WipeGame();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Current spells:");
            row = row + 1;


            LearnedSpell test = GetSpell(row, true);

            if (test != null)
            {
                test.Cast(Program.entities[0]);
                DrawGame();
            }
            else
            {
                ConsoleKey command = Program.LastKey.Key;
                if (command == ConsoleKey.F)
                {
                    ForgetScreen();
                }
                else
                    DrawGame();
            }
        }

        public static void ForgetScreen()
        {
            WipeGame();

            int row = 1;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(1, row);
            Console.Write(Helper.FormatString("Forget which spell?", 58));
            row = row + 1;
            LearnedSpell test = GetSpell(row);

            if (test != null)
            {
                entities[0].Forget(test);
            }

            DrawGame();
        }

        private static LearnedSpell GetSpell(int row, bool msg = false, int selected = 0)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;

            Dictionary<ConsoleKey, LearnedSpell> dict = BuildSpellDictionary();
            int initrow = row;
            row = WriteSpells(row, selected);

            if (msg)
            {
                row = row + 2;
                Console.SetCursorPosition(1, row);
                Console.Write("(F)orget, or select a spell to use.");
                row = row + 1;
                Console.SetCursorPosition(1, row);
                Console.Write(String.Concat("Current maximum spells: ", Program.entities[0].maxspells));
            }

            Program.LastKey = Console.ReadKey(true);
            ConsoleKey command = LastKey.Key;
            if (dict.ContainsKey(command))
            {
                return dict[command];
            }
            else if (LastKey.Key == ConsoleKey.UpArrow)
            {
                selected = Math.Max(selected - 1, 0);
                return GetSpell(initrow, msg, selected);
            }
            else if (LastKey.Key == ConsoleKey.DownArrow)
            {
                selected = Math.Min(selected + 1, Program.entities[0].spells.Count - 1);
                return GetSpell(initrow, msg, selected);
            }
            else if (((LastKey.Key == ConsoleKey.Spacebar) || (LastKey.Key == ConsoleKey.Enter)) && (Program.entities[0].spells.Count != 0))
            {
                return Program.entities[0].spells[selected];
            }
            else
                return null;
        }


        private static Dictionary<ConsoleKey, LearnedSpell> BuildSpellDictionary()
        {
            Dictionary<ConsoleKey, LearnedSpell> dict = new Dictionary<ConsoleKey, LearnedSpell>();
            ConsoleKey[] commands = new ConsoleKey[] {ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5,
            ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9};
            if (entities[0].spells.Count != 0)
            {
                for (int i = 0; i < entities[0].spells.Count; i++)
                {
                    dict.Add(commands[i], entities[0].spells[i]);
                }
            }
            return dict;
        }

        private static int WriteInventory(int row, int selected)
        {
            string[] commands = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            if (entities[0].inventory.Count != 0)
            {
                for (int i = 0; i < entities[0].inventory.Count; i++)
                {
                    row = row + 1;
                    Console.SetCursorPosition(1, row);
                    if (i == selected)
                        Console.Write(String.Concat("* (", commands[i], ")"));
                    else
                        Console.Write(String.Concat("  (", commands[i], ")"));
                    
                    entities[0].inventory[i].Draw(7, row);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(9,row);
                    string marker = entities[0].inventory[i].name;
                    if (entities[0].inventory[i].equipped)
                        marker = String.Concat(marker, " (", entities[0].inventory[i].item.GetSlotName(),")");
                    else if (entities[0].inventory[i].equippable)
                        marker = String.Concat(marker, " ( )");

                    Console.Write(marker);
                }
            }

            return row;
        }

        private static int WriteSpells(int row, int selected)
        {
            string[] commands = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            if (entities[0].spells.Count != 0)
            {
                for (int i = 0; i < entities[0].spells.Count; i++)
                {
                    row = row + 1;
                    Console.SetCursorPosition(1, row);
                    if (i == selected)
                        Console.Write(String.Concat("* (", commands[i], ") "));
                    else
                        Console.Write(String.Concat("  (", commands[i], ") "));
                    //Console.Write(String.Concat("(", commands[i], ") "));
                    string marker = entities[0].spells[i].name;

                    if (entities[0].spells[i].charges != 0)
                        marker = String.Concat(marker, " (", entities[0].spells[i].charges.ToString(), ")");
                    else
                        marker = String.Concat(marker, " (charging)");

                    Console.Write(marker);
                }
            }

            return row;
        }

        private static InventoryItem GetItem(Dictionary<ConsoleKey, InventoryItem> dict)
        {
            ConsoleKey command = Console.ReadKey(true).Key;
            if (dict.ContainsKey(command))
                return dict[command];
            else
                return null;
        }

        private static LearnedSpell GetSpell(Dictionary<ConsoleKey, LearnedSpell> dict)
        {
            ConsoleKey command = Console.ReadKey(true).Key;
            if (dict.ContainsKey(command))
                return dict[command];
            else
                return null;
        }

        public static InventoryItem ItemSelectionScreen(string request)
        {
            WipeGame();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write(request);
            return GetItem(row + 1);
        }
    }
}