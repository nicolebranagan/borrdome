using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    [Serializable]
    public class Map
    {
        public string[,] map = new string[60, 25];
        public int[,] smellmap = new int[60, 25];

        public bool[,] occupied = new bool[12, 5];
        public Vector start = new Vector(0,0);
        public Vector end = new Vector(0,0);

        public List<Vector> enemyslots;
        public List<Vector> thingslots;

        public List<Species> usedspecies;
        public List<Item> useditems;

        DungeonColors colorset;

        public Map(double enemy_prob, double item_prob, int level)
        {
            colorset = DungeonColors.newColor();

            enemyslots = new List<Vector>();
            thingslots = new List<Vector>();

            usedspecies = new List<Species>();
            useditems = new List<Item>();

            for (int i = 0; i < 60; i++)
                for (int j = 0; j < 25; j++)
                {
                    map[i, j] = " ";
                    smellmap[i, j] = 0;
                }

            Algorithm1();

            int refine = 0;
            if (level > 9)
                refine = 1;
            if (level > 19)
                refine = 2;
            Algorithm2(refine);

            FillLists(enemy_prob, item_prob);

            // In theory, we now have a map!
        }

        private void Algorithm1()
        {
            bool[,] connected = new bool[12, 5];
            bool[,] connect_u = new bool[12, 5];
            bool[,] connect_d = new bool[12, 5];
            bool[,] connect_l = new bool[12, 5];
            bool[,] connect_r = new bool[12, 5];

            // Clear out the arrays
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 5; j++)
                {
                    connected[i, j] = false;
                    connect_u[i, j] = false;
                    connect_d[i, j] = false;
                    connect_l[i, j] = false;
                    connect_r[i, j] = false;
                }

            // Random interconnection algorithm

            int x_pos = Program.random.Next(0, 12);
            int y_pos = Program.random.Next(0, 5);
            int visited = 1;
            int shouldvisit = Program.random.Next(30, 45);

            start.x = (x_pos * 5) + 2;
            start.y = (y_pos * 5) + 2;
            connected[x_pos, y_pos] = true;

            while (visited < shouldvisit)
            {
                int side = Program.random.Next(0, 4); // 0 - up 1 - down 2 - left 3 - right
                if ((side == 0) && (y_pos == 0))
                    side = 1;
                if ((side == 1) && (y_pos == 4))
                    side = 0;
                if ((side == 2) && (x_pos == 0))
                    side = 3;
                if ((side == 3) && (x_pos == 11))
                    side = 2;

                switch (side)
                {
                    case 0:
                        {
                            connect_u[x_pos, y_pos] = true;
                            y_pos = y_pos - 1;
                            connect_d[x_pos, y_pos] = true;
                            break;
                        }
                    case 1:
                        {
                            connect_d[x_pos, y_pos] = true;
                            y_pos = y_pos + 1;
                            connect_u[x_pos, y_pos] = true;
                            break;
                        }
                    case 2:
                        {
                            connect_l[x_pos, y_pos] = true;
                            x_pos = x_pos - 1;
                            connect_r[x_pos, y_pos] = true;
                            break;
                        }
                    case 3:
                        {
                            connect_r[x_pos, y_pos] = true;
                            x_pos = x_pos + 1;
                            connect_l[x_pos, y_pos] = true;
                            break;
                        }

                }

                if (connected[x_pos, y_pos] == false)
                {
                    connected[x_pos, y_pos] = true;
                    visited = visited + 1;
                }
            }

            end.x = (x_pos * 5) + 2;
            end.y = (y_pos * 5) + 2;

            // Now we can draw the map based on our random algorithm
            for (int i = 0; i < 12; i++)
                for (int j = 0; j < 5; j++)
                {
                    if (connected[i, j])
                    {
                        int x = i * 5;
                        int y = j * 5;

                        for (int k = 0; k < 5; k++)
                            for (int l = 0; l < 5; l++)
                            {
                                map[x + k, y + l] = ".";
                            }

                        for (int k = 0; k < 5; k++)
                        {
                            if (!connect_u[i, j])
                                map[x + k, y] = "#";
                            if (!connect_d[i, j])
                                map[x + k, y + 4] = "#";
                            if (!connect_l[i, j])
                                map[x, y + k] = "#";
                            if (!connect_r[i, j])
                                map[x + 4, y + k] = "#";
                        }
                    }
                }
        }

        private void Algorithm2(int refinements)
        {
            for (int i = 0; i < 60; i++)
                for (int j = 0; j < 25; j++)
                    if (map[i, j] == " ")
                        map[i, j] = "#";

            //caveify
            string[,] oldmap = map;

            for (int h = 0; h < refinements; h++)
            {
                for (int i = 1; i < 59; i++)
                    for (int j = 1; j < 24; j++)
                    {
                        List<string> tiles = Helper.GetSurroundingTiles(oldmap, 59, 24, i, j);
                        int walls = tiles.Where(item => item == "#").Count();
                        if ((map[i, j] == "#") && (walls < 4))
                            map[i, j] = ".";
                        else if ((map[i, j] == ".") && (walls > 4))
                            map[i, j] = "#";
                    }
            }

            //refine
            oldmap = map;
            for (int i = 0; i < 60; i++)
                for (int j = 0; j < 25; j++)
                {
                    List<string> tiles = Helper.GetSurroundingTiles(oldmap, 59, 24, i, j);
                    int walls = tiles.Where(item => item == ".").Count();
                    if ((map[i, j] == "#") && (walls == 0))
                        map[i, j] = " ";
                }
        }

        private void FillLists(double enemy_prob, double item_prob)
        {
            for (int i = 0; i < 60; i++)
                for (int j = 0; j < 25; j++)
                    if (map[i, j] == ".")
                    {
                        Vector room = new Vector();
                        room.x = i; room.y = j;
                        if (Program.random.NextDouble() < item_prob)
                            thingslots.Add(room);
                        else if (Program.random.NextDouble() < enemy_prob)
                            enemyslots.Add(room);
                    }

            thingslots.Remove(start);
            thingslots.Remove(end);
            enemyslots.Remove(start);
            enemyslots.Remove(end);
        }

        public void DrawWorld()
        {
            for (int i = 0; i < 60; i++)
                for (int j = 0; j < 25; j++)
                    this.DrawTile(i, j);
        }

        public void DrawRow(int row, int column = 0)
        {
            for (int i = column; i < 60; i++)
               this.DrawTile(i, row);
        }

        public void DrawTile(int x, int y)
        {
            string tile = this.map[x, y];
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Black;
            if (tile == ".")
            {
                if ((x + y * y) % 6 == 0)
                    Console.ForegroundColor = colorset.FloorColor2;
                else
                    Console.ForegroundColor = colorset.FloorColor1;
            }
            else if (tile == "#")
            {
                if ((x + y) % 7 == 0)
                    Console.ForegroundColor = colorset.WallColor2;
                else
                    Console.ForegroundColor = colorset.WallColor1;
            }
            else
            {
                Console.ForegroundColor = colorset.ElseColor;
            }

            Console.Write(this.map[x, y]);
            foreach (Thing u in Program.things.Where(item => ((item.x == x) && (item.y == y))))
                u.Draw();
        }

        public void SpawnEnemies(int level)
        {
            List<Species> allowedEnemies = new List<Species>();
            foreach (Species u in Program.species.Where(item => (item.minlevel <= level) && (item.maxlevel >= level)))
                allowedEnemies.Add(u);

            foreach (Species u in allowedEnemies)
                if (u.guarantee)
                {
                    int rand = Program.random.Next(0, enemyslots.Count);
                    Vector slot = enemyslots[rand];
                    Entity newEnemy = new NPC(slot.x, slot.y, u);
                    enemyslots.Remove(slot);
                    Program.entities.Add(newEnemy);
                }

            allowedEnemies.RemoveAll(item => item.guarantee);

            foreach (Vector u in enemyslots)
            {
                int rand = Program.random.Next(0, allowedEnemies.Count);

                Entity proposedEnemy = new NPC(u.x, u.y, allowedEnemies[rand]);

                if (allowedEnemies[rand].rarity >= Program.random.NextDouble())
                {
                    Program.entities.Add(proposedEnemy);
                    if (!usedspecies.Contains(allowedEnemies[rand])) 
                        usedspecies.Add(allowedEnemies[rand]);
                    if (allowedEnemies[rand].unique)
                        allowedEnemies.Remove(allowedEnemies[rand]);
                }
            }

        }

        public void SpawnThings(int level)
        {
            List<Item> allowedItems = new List<Item>();
            foreach (Item u in Program.items.Where(item => (item.minlevel <= level) && (item.maxlevel >= level)))
                allowedItems.Add(u);

            if (start.Equals(new Vector(0, 0)))
            {
                start = thingslots[0];
                thingslots.Remove(start);
            }

            if (end.Equals(new Vector(0, 0)))
            {
                end = thingslots[thingslots.Count - 1];
                thingslots.Remove(end);
            }

            Program.things.Add(new Stairs(start, -1));
            if (level != 25) Program.things.Add(new Stairs(end, 1));

            foreach (Vector u in thingslots)
            {
                int rand = Program.random.Next(0, allowedItems.Count);

                Thing proposedThing = new ItemThing(u.x, u.y, allowedItems[rand]);

                if (allowedItems[rand].rarity >= Program.random.NextDouble())
                {
                    Program.things.Add(proposedThing);
                    if (!useditems.Contains(allowedItems[rand]) && !allowedItems[rand].noshow)
                        useditems.Add(allowedItems[rand]);
                    if (allowedItems[rand].unique)
                        allowedItems.Remove(allowedItems[rand]);
                }
            }
        }

        public bool CanMove(int x, int y, bool ghost)
        {
            // At some point this should have more conditions
            //return (((map[x, y] != "#") && ghost) && (map[x,y] != " "));
            if ((y == 0) || (y == 24) || ( x==0 ) || (x == 59)) return false;
            switch (map[x,y])
            {
                case ".": { return true; }
                case "#": { return ghost; }
                default: { return false; }
            }
        }

        public void IterateSmellMap()
        {
            smellmap[Program.entities[0].x, Program.entities[0].y] = 20;
            bool loop = true;
            while (loop)
            {
                int[,] legacymap = smellmap;
                for (int i = 0; i < 60; i++)
                    for (int j = 0; j < 25; j++)
                        if (map[i, j] == ".")
                        {
                            int test = Helper.MaxOfSurroundingTiles(legacymap, 60, 25, i, j) - 1;
                            if (test >= 0)
                                smellmap[i, j] = test;
                        }

                loop = false;
                for (int i = 0; i < 60; i++)
                    for (int j = 0; j < 25; j++)
                        if (legacymap[i, j] != smellmap[i, j]) loop = true;
            }
        }
    }
}
