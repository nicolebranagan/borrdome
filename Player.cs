using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    [Serializable]
    class Player : Entity
    {
        public Player(genders gender)
        {
            this.x = 0;
            this.y = 0;
            this.face = "@";
            this.species = Program.species[0];
            this.attack = species.attack;
            this.defense = species.defense;
            this.foregroundcolor = this.species.foregroundcolor;
            this.backgroundcolor = this.species.backgroundcolor;
            this.gender = gender;
            this.food = 9000;
        }

        public override void Loop()
        {
            if (Program.LastKey.Key == ConsoleKey.I)
                Program.InventoryScreen();

            //(L)ook, (E)quip/Unequip, (D)rop, (T)hrow,

            else if (Program.LastKey.Key == ConsoleKey.L)
                Program.LookScreen();
            else if (Program.LastKey.Key == ConsoleKey.E)
                Program.EquipScreen();
            else if (Program.LastKey.Key == ConsoleKey.D)
                Program.DropScreen();
            else if (Program.LastKey.Key == ConsoleKey.T)
                Program.ThrowScreen();

            else if (Program.LastKey.Key == ConsoleKey.M)
                Program.MagicScreen();

            else if (Program.LastKey.Key == ConsoleKey.Q)
                this.Learn(Program.spells[0]);

            else if (Program.LastKey.Key == ConsoleKey.P)
                Look();
            else if (Program.LastKey.Key == ConsoleKey.C)
                Charm();
            else if (Program.LastKey.Key == ConsoleKey.G)
                Grab();
            else
            {
                Vector dir = getDirection();
                if (dir != null)
                    this.Move(dir.x, dir.y);
            }

            this.food -= this.species.eatingrate;
            if (this.food <= 0)
                this.die("starvation");
            else if (this.food <= 500)
                Program.Report("You feel as if you will die for lack of food.");
            else if (this.food <= 1000)
                Program.Report("Your stomach rumbles.");

            base.Loop();
        }

        private Vector getDirection()
        {
            if ((Program.LastKey.Key == ConsoleKey.UpArrow) || (Program.LastKey.Key == ConsoleKey.K) || (Program.LastKey.Key == ConsoleKey.NumPad8))
                return new Vector(0, -1);
            else if ((Program.LastKey.Key == ConsoleKey.DownArrow) || (Program.LastKey.Key == ConsoleKey.J) || (Program.LastKey.Key == ConsoleKey.NumPad2))
                return new Vector(0, 1);
            else if ((Program.LastKey.Key == ConsoleKey.LeftArrow) || (Program.LastKey.Key == ConsoleKey.H) || (Program.LastKey.Key == ConsoleKey.NumPad4))
                return new Vector(-1, 0);
            else if ((Program.LastKey.Key == ConsoleKey.RightArrow) || (Program.LastKey.Key == ConsoleKey.L) || (Program.LastKey.Key == ConsoleKey.NumPad6))
                return new Vector(1, 0);
            else if ((Program.LastKey.Key == ConsoleKey.Y) || (Program.LastKey.Key == ConsoleKey.NumPad7))
                return new Vector(-1, -1);
            else if ((Program.LastKey.Key == ConsoleKey.U) || (Program.LastKey.Key == ConsoleKey.NumPad9))
                return new Vector(1, -1);
            else if ((Program.LastKey.Key == ConsoleKey.B) || (Program.LastKey.Key == ConsoleKey.NumPad1))
                return new Vector(-1, 1);
            else if ((Program.LastKey.Key == ConsoleKey.N) || (Program.LastKey.Key == ConsoleKey.NumPad3))
                return new Vector(1, 1);
            else
                return null;
        }

        public override void Move(int x, int y)
        {
            bool condition = true;

            foreach (Thing u in Program.things)
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    condition = u.Bump(this);
                    break;
                }
            }

            foreach (Entity u in Program.entities.Where(item => item != this))
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    if ((this.status == statuses.Poison) || (this.status == statuses.Normal)) this.attackTarget(u);
                    else if (this.status == statuses.Paralyzed) Program.Report("Paralyzed!");
                    else if (this.status == statuses.Charmed) Program.Report("Charmed- can't attack!");
                    condition = false;
                    break;
                }
            }

            if (condition)
                base.Move(x, y);
        }

        private void Grab()
        {
            Program.Report("(G)et what?", true);
            Program.LastKey = Console.ReadKey(true);
            Vector dir = getDirection();
            if (dir != null)
                this.Take(dir.x, dir.y);
            else
                Program.Report("Nevermind then.");
        }

        private void Take(int x, int y)
        {
            bool condition = true;

            foreach (Thing u in Program.things)
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    u.Take(this);
                    condition = false;
                    break;
                }
            }

            if (condition) Program.Report("Nothing there to take!");
        }

        private void Look()
        {
            Program.Report("(P)eer where?", true);
            Program.LastKey = Console.ReadKey(true);
            Vector dir = getDirection();
            if (dir != null)
                this.Look2(dir.x, dir.y);
            else
                Program.Report("Nevermind then.");
        }

        private void Look2(int x, int y)
        {
            foreach (Thing u in Program.things)
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    Program.Report(u.description);
                    break;
                }
            }

            foreach (Entity u in Program.entities)
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    string desc = "You see a";
                    if (u.species.gendered)
                        desc = String.Concat(desc, "n ", u.GenderMarker());
                    desc = String.Concat(desc, " ", u.species.name);
                    Program.Report(desc);
                    break;
                }
            }
        }

        private void Charm()
        {
            Program.Report("(C)harm who?", true);
            Program.LastKey = Console.ReadKey(true);
            Vector dir = getDirection();
            if (dir != null)
                this.Charm2(dir.x, dir.y);
            else
                Program.Report("Nevermind then.");
        }

        private void Charm2(int x, int y)
        {
            bool nocharm = true;
            foreach (Entity u in Program.entities)
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    u.Charm(this);
                    nocharm = false;
                    break;
                }
            }

            if (nocharm) Program.Report("No one there to charm!");
        }

        public override Entity GetTarget(double range)
        {
            Program.Report("Target who?", true);
            return Program.TargetEntity(range);
        }

        public override void die(string murderer)
        {
            Program.DeathScreen(murderer);
        }
    }
}
