using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace borrdome
{
    partial class Program
    {
        
        public static bool Welcome()
        {
            Console.Clear();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Welcome to Borr-Dome.");

            if (File.Exists("save.dat"))
            {
                row = row + 2;
                Console.SetCursorPosition(1, row);
                Console.Write("A saved adventure exists. Load it? (Y/N)");
                row = row + 1;
                Console.SetCursorPosition(1, row);
                Console.Write("Otherwise, it will be deleted.");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    StoredLevels.Load();
                    File.Delete("save.dat");
                    return false;
                }
                else
                {
                    File.Delete("save.dat");
                    GeneratePlayer();
                }
            }
            else
                GeneratePlayer();
            return true;
        }
        
        public static void GeneratePlayer()
        {
            Console.Clear();

            int row = 0;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("Welcome to Borr-Dome.");

            Entity.genders gender;
            string genderstring;
            if (Program.random.Next(0, 2) >= 1)
            {
                gender = Entity.genders.Female;
                genderstring = "female";
            }
            else
            {
                gender = Entity.genders.Male;
                genderstring = "male";
            }

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write(String.Concat("You are a ", genderstring, " adventurer who has chosen to set out for the legendary dome"));
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("of the dark wizard Borr, armed with naught but your wits. (Was that wise?)");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("Your mission, taken from the wizard himself, is to kill the three dragons");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("that have taken up residence on the lowest levels of the dome, and bring back");
            row = row + 1;
            Console.SetCursorPosition(1, row);
            Console.Write("their scales as proof.");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("But who really knows what you shall find within?");

            row = row + 2;
            Console.SetCursorPosition(1, row);
            Console.Write("Press any key to begin.");

            Program.entities.Add(new Player(gender));
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}