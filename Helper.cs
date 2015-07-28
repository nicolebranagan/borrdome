using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    public class Helper
    {
        public static string FormatString(string initial, int length)
        {
            string final = initial;
            if (initial.Length > length)
                final = initial.Substring(0, length);
            else if (initial.Length < length)
            {
                int time = (length - initial.Length);
                for (int i = 0; i < time; i++)
                    final = String.Concat(final, " ");
            }

            return final;
        }

        public static bool WithinCircle(int x1, int y1, double radius, int x2, int y2)
        {
            return (Math.Sqrt((Math.Pow((x1 - x2),2)) + (Math.Pow((y1 - y2),2))) < radius);
        }

        public static int MaxOfSurroundingTiles(int[,] map, int max_x, int max_y, int x, int y)
        {
            List<int> tiles = new List<int>();

            if (y > 0) tiles.Add(map[x, y - 1]);
            if (y < max_y) tiles.Add(map[x, y + 1]);
            if (x > 0) tiles.Add(map[x - 1, y]);
            if (x < max_x) tiles.Add(map[x + 1, y]);

            if ((y > 0) && (x > 0)) tiles.Add(map[x - 1, y - 1]);
            if ((y < max_y) && (x > 0)) tiles.Add(map[x - 1, y + 1]);
            if ((y > 0) && (x < max_x)) tiles.Add(map[x + 1, y - 1]);
            if ((y < max_y) && (x < max_x)) tiles.Add(map[x + 1, y + 1]);

            return tiles.Max();
        }

        public static List<string> GetSurroundingTiles(string[,] map, int max_x, int max_y, int x, int y)
        {
            List<string> tiles = new List<string>();

            if (y > 0) tiles.Add(map[x, y - 1]);
            if (y < max_y) tiles.Add(map[x, y + 1]);
            if (x > 0) tiles.Add(map[x - 1, y]);
            if (x < max_x) tiles.Add(map[x + 1, y]);

            if ((y > 0) && (x > 0)) tiles.Add(map[x - 1, y - 1]);
            if ((y < max_y) && (x > 0)) tiles.Add(map[x - 1, y + 1]);
            if ((y > 0) && (x < max_x)) tiles.Add(map[x + 1, y - 1]);
            if ((y < max_y) && (x < max_x)) tiles.Add(map[x + 1, y + 1]);

            return tiles;
        }

        public static Vector RollUp(int[,] map, int max_x, int max_y, int x, int y, int minimum = 10)
        {
            Dictionary<Vector, Vector> tiles = new Dictionary<Vector, Vector>();
            
            if (y > 0) tiles.Add(new Vector(x, y - 1), new Vector(0,-1));
            if (y < max_y) tiles.Add(new Vector(x, y + 1), new Vector(0,1));
            if (x > 0) tiles.Add(new Vector(x - 1, y), new Vector(-1,0));
            if (x < max_x) tiles.Add(new Vector(x + 1, y), new Vector(1,0));

            if ((y > 0) && (x > 0)) tiles.Add(new Vector(x - 1, y - 1), new Vector(-1,-1));
            if ((y < max_y) && (x > 0)) tiles.Add(new Vector(x - 1, y + 1), new Vector(-1, 1));
            if ((y > 0) && (x < max_x)) tiles.Add(new Vector(x + 1, y - 1), new Vector(1, -1));
            if ((y < max_y) && (x < max_x)) tiles.Add(new Vector(x + 1, y + 1), new Vector(1,1));

            int maxvalue = 0;
            Vector key = new Vector(0,0);

            foreach (Vector u in tiles.Keys)
                if (map[u.x, u.y] > maxvalue)
                {
                    maxvalue = map[u.x, u.y];
                    key = u;
                }

            if (maxvalue < minimum)
                return null;
            else
                return tiles[key];
        }

        public static string[,] DrawVerticalLine(string[,] map, int column, string face, int start, int max)
        {
            for (int i = start; i < max; i++)
            {
                map[column, i] = face;
            }
            return map;
        }

        public static string[,] DrawHorizontalLine(string[,] map, int row, string face, int start, int max)
        {
            for (int i = start; i < max; i++)
            {
                map[i, row] = face;
            }
            return map;
        }
    }

    /*public class Tree
    {
        protected class TreeElement
        {
            public int index;
            public int above;
            public int[] data;

            public TreeElement(int index, int above, int[] data)
            {
                this.index = index;
                this.above = above;
                this.data = data;
            }
        }

        List<TreeElement> TreeList = new List<TreeElement>();

        public Tree(int[] data)
        {
            TreeList.Add(new TreeElement(0, -1, data));
        }

        public int AddElement(int parent, int[] data)
        {
            int identifier = 1;
            foreach (TreeElement u in TreeList)
            {
                if (u.index >= identifier)
                {
                    identifier = u.index + 1;
                }
            }
            TreeList.Add(new TreeElement(identifier, parent, data));
            return identifier;
        }

        public List<int> GetChildren(int index)
        {
            List<int> result = new List<int>();
            foreach (TreeElement u in TreeList.Where(item => item.above == index))
                result.Add(u.index);
            return result;
        }

        public int GetParent(int index)
        {
            int result = -1;
            foreach (TreeElement u in TreeList.Where(item => item.index == index))
            {
                result = u.index;
                break;
            }
            return result;
        }

        public int[] GetData(int index)
        {
            int[] result = null;
            foreach (TreeElement u in TreeList.Where(item => item.index == index))
            {
                result = u.data;
                break;
            }
            return result;
        }

        public int GetMax()
        {
            int max = 0;
            foreach (TreeElement u in TreeList)
            {
                if (u.index >= max)
                {
                    max = u.index;
                }
            }
            return max;
        }
    }*/

    [Serializable]
    public class Vector : IEquatable<Vector>
    {
        public int x { get; set; }
        public int y { get; set; }

        public bool Equals(Vector vec)
        {
            if ((vec.x == this.x) && (vec.y == this.y))
                return true;
            else
                return false;
        }

        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                Vector vec = (Vector)obj;
                if ((vec.x == this.x) && (vec.y == this.y))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
