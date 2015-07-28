using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    public partial class Program
    {
        static bool CheatMonitor()
        {
#if DEBUG
            if (Program.LastKey.Key == ConsoleKey.Insert)
            {
                DEBUG = true;
                CheatScreen();
                return true;
            }
            return true;
#else
            return false;
#endif
        }

        static void CheatScreen()
        {
            Program.WipeGame();
            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Welcome to Debug.");
            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("Choose your poisson:");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(0) Overshoot");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(1) Choose a level");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(2) Nymphate everyone");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(3) Save");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("(4) Load");

            Program.LastKey = Console.ReadKey(true);
            switch (Program.LastKey.Key)
            {
                case ConsoleKey.D0:
                    {
                        AttributeChange test = new AttributeChange();
                        test.setSpecies("Naga");
                        test.NewGender = AttributeChange.StatusGenders.Female;
                        test.Food = Int32.MaxValue - 10000;
                        test.Health = 10000;
                        test.MoreSpells = 10;
                        test.setSpell("Destroy");
                        Program.entities[0].Transform(test, "the goddess");
                        break;
                    }
                case ConsoleKey.D1:
                    {
                        Console.Clear();
                        int level = Int32.Parse(System.Console.ReadLine());
                        newlevel = level;
                        newlevelflag = true;
                        break;
                    }
                case ConsoleKey.D2:
                    {
                        AttributeChange test = new AttributeChange();
                        test.setSpecies("Nymph");
                        test.NewGender = AttributeChange.StatusGenders.Female;
                        test.Status = Entity.statuses.Charmed;
                        foreach (Entity u in Program.entities)
                        {
                            u.Transform(test, "the goddess");
                        }
                        break;
                    }
                case ConsoleKey.D3:
                    {
                        StoredLevels.Save();
                        break;
                    }
                case ConsoleKey.D4:
                    {
                        StoredLevels.Load();
                        break;
                    }
            }
            Program.DrawGame();
        }
    }
}
