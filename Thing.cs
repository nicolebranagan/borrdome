using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    [Serializable]
    public abstract class Thing
    {
        public int x;
        public int y;
        public string description;

        public ConsoleColor backgroundcolor;
        public ConsoleColor foregroundcolor;

        public bool exists = true;

        public Thing()
        {
        }

        public abstract void Draw();

        public abstract bool Bump(Entity bumper); // Return true if you can move through it, remove false otherwise

        public abstract void Take(Entity taker);

        public virtual void Refresh()
        {
        }

        public bool WithinRange(Entity enemy)
        {
            return Helper.WithinCircle(this.x, this.y, enemy.species.throwingrange, enemy.x, enemy.y);
        }

        public bool WithinRange(Entity enemy, double radius)
        {
            return Helper.WithinCircle(this.x, this.y, radius, enemy.x, enemy.y);
        }

        public virtual void Destroy()
        {
            if (Program.things.Contains(this))
                Program.things.Remove(this);
        }
    }

    [Serializable]
    public class Stairs : Thing
    {
        string face;
        int delta;
        
        public Stairs(Vector position, int delta)
        {
            this.x = position.x;
            this.y = position.y;
            this.description = "A spiral of stairs.";
            this.delta = delta;
            if (delta < 0)
                face = "<";
            else
                face = ">";

            this.backgroundcolor = ConsoleColor.Black;
            this.foregroundcolor = ConsoleColor.Gray;
        }

        public override void Draw()
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(this.face);
        }

        public override bool Bump(Entity bumper)
        {
            if (bumper == Program.entities[0])
            {
                if ((Program.currentlevel + delta) > 0)
                {
                    Program.newlevel = Program.currentlevel + delta;
                    Program.newlevelflag = true;
                    if (delta < 0)
                        Program.climbfrombackwards = true;
                    else
                        Program.climbfrombackwards = false;
                }
                else
                    Program.EndScreen();
                return true;
            }
            else
                return true;
        }

        public override void Take(Entity taker)
        {
            Program.Report("Can't get that!");
        }

        public override void Destroy()
        {
            ;
        }
    }

    [Serializable]
    public class ItemThing : Thing
    {
        private Item item;
        
        public override void Draw()
        {
            if (exists) this.item.Draw(this.x, this.y);
        }

        public ItemThing(int x, int y, Item item)
        {
            this.x = x;
            this.y = y;
            this.item = item;
            this.description = item.description;
            this.backgroundcolor = item.backgroundcolor;
            this.foregroundcolor = item.foregroundcolor;
        }

        private void Got()
        {
            if (!this.item.remain)
            {
                exists = false;
                Program.themap.DrawTile(this.x, this.y);
                x = -1;
                y = -1;
            }
        }

        public override bool Bump(Entity bumper)
        {
            if (item.onbump)
            {
                InventoryItem newitem = new InventoryItem(item);
                string oldname = bumper.name;
                newitem.Use(bumper, true);
                if (bumper == Program.entities[0])
                    Program.Report(String.Concat("Hit by ", newitem.name, "!"));
                else
                    Program.Report(String.Concat(oldname, " was hit by ", newitem.name, "!"));
                this.Got();
                if (!this.item.remain)
                    return true;
                else
                    return false;
            }
            else
            {
                if (bumper == Program.entities[0])
                    Program.Report(description);
                return false;
            }
        }

        public override void Take(Entity taker)
        {
            if (item.ontake)
            {
                InventoryItem newitem = new InventoryItem(item);
                newitem.Use(taker, true);
                if (taker == Program.entities[0])
                    Program.Report(String.Concat("Hit by ", newitem.name, "!"));
                this.Got();
            }
            else
            {
                if (taker.Get(item))
                    this.Got();
            }

        }

        public override void Refresh()
        {
            this.description = item.description;
        }

        public override void Destroy()
        {
            Program.Report(String.Concat(this.item.name, " destroyed!"));
            base.Destroy();
        }
    }

}