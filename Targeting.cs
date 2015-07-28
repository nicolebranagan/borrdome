using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    partial class Program
    {
        public static Entity TargetEntity()
        {
            DrawGame();
            List<Entity> targets = new List<Entity>();
            foreach (Entity u in Program.entities.Where(item => item.WithinRange(entities[0])))
                targets.Add(u);
            if (targets.Count == 0)
            {
                Program.Report("No targets in range.");
                return null;
            }
            else
            {
                Dictionary<ConsoleKey, Entity> dict = new Dictionary<ConsoleKey, Entity>();
                ConsoleKey[] commands = new ConsoleKey[] {ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5,
                                        ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9};
                string[] labels = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

                for (int i = 0; i < targets.Count; i++)
                {
                    Console.BackgroundColor = targets[i].species.backgroundcolor;
                    Console.ForegroundColor = targets[i].species.foregroundcolor;
                    Console.SetCursorPosition(targets[i].x, targets[i].y);
                    Console.Write(labels[i]);
                    dict.Add(commands[i], targets[i]);
                }

                Program.Report("Choose target:", true);
                ConsoleKey attack = Console.ReadKey(true).Key;

                if (dict.ContainsKey(attack))
                    return dict[attack];
                else
                {
                    Program.Report("Invalid command.", true);
                    return null;
                }
            }
        }

        public static Entity TargetEntity(double radius)
        {
            DrawGame();
            List<Entity> targets = new List<Entity>();
            foreach (Entity u in Program.entities.Where(item => item.WithinRange(entities[0], radius)))
                targets.Add(u);
            if (targets.Count == 0)
            {
                Program.Report("No targets in range.");
                return null;
            }
            else
            {
                Dictionary<ConsoleKey, Entity> dict = new Dictionary<ConsoleKey, Entity>();
                ConsoleKey[] commands = new ConsoleKey[] {ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5,
                                        ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9};
                string[] labels = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

                for (int i = 0; i < targets.Count; i++)
                {
                    Console.BackgroundColor = targets[i].species.backgroundcolor;
                    Console.ForegroundColor = targets[i].species.foregroundcolor;
                    Console.SetCursorPosition(targets[i].x, targets[i].y);
                    Console.Write(labels[i]);
                    dict.Add(commands[i], targets[i]);
                }

                Program.Report("Choose target:", true);
                ConsoleKey attack = Console.ReadKey(true).Key;

                if (dict.ContainsKey(attack))
                    return dict[attack];
                else
                {
                    Program.Report("Invalid command.", true);
                    return null;
                }
            }
        }

        public static Thing TargetThing(double radius)
        {
            DrawGame();
            List<Thing> targets = new List<Thing>();
            foreach (Thing u in Program.things.Where(item => item.WithinRange(entities[0], radius)))
                targets.Add(u);
            if (targets.Count == 0)
            {
                Program.Report("No targets in range.");
                return null;
            }
            else
            {
                Dictionary<ConsoleKey, Thing> dict = new Dictionary<ConsoleKey, Thing>();
                ConsoleKey[] commands = new ConsoleKey[] {ConsoleKey.D0, ConsoleKey.D1, ConsoleKey.D2, ConsoleKey.D3, ConsoleKey.D4, ConsoleKey.D5,
                                        ConsoleKey.D6, ConsoleKey.D7, ConsoleKey.D8, ConsoleKey.D9, ConsoleKey.A, ConsoleKey.B, ConsoleKey.C, ConsoleKey.D, ConsoleKey.E, ConsoleKey.F};
                string[] labels = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };

                for (int i = 0; i < targets.Count; i++)
                {
                    Console.BackgroundColor = targets[i].backgroundcolor;
                    Console.ForegroundColor = targets[i].foregroundcolor;
                    Console.SetCursorPosition(targets[i].x, targets[i].y);
                    Console.Write(labels[i]);
                    dict.Add(commands[i], targets[i]);
                }

                Program.Report("Choose target:", true);
                ConsoleKey attack = Console.ReadKey(true).Key;

                if (dict.ContainsKey(attack))
                    return dict[attack];
                else
                {
                    Program.Report("Invalid command.", true);
                    return null;
                }
            }
        }
    }
}