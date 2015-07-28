using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    [Serializable]
    public abstract class AI
    {
        protected int frequency = 0;
        protected int timer = 0;
        
        public abstract void Loop(Entity body);

        public static AI FromString(string label, Entity body)
        {
            string[] elements = label.Split(","[0]);
            string type = elements[0];
            
            AI newAI;
            switch (type)
            {
                case "Seeker":
                    {
                        newAI = new SeekerAI(label, body);
                        break;
                    }
                case "Dumb":
                    {
                        newAI = new DumbAI(int.Parse(elements[1]));
                        break;
                    }
                default:
                    {
                        newAI = new StillAI();
                        break;
                    }
            }

            return newAI;
        }

        protected void MoveRandomly(Entity body)
        {
            double rand = Program.random.Next(0, 4);
            if (rand == 0)
                body.Move(0, -1);
            else if (rand == 1)
                body.Move(0, 1);
            else if (rand == 2)
                body.Move(-1, 0);
            else
                body.Move(1, 0);
        }

        protected virtual bool PhysicalAttack(Entity body)
        {
            int PlayerX = Program.entities[0].x;
            int PlayerY = Program.entities[0].y;
            int delX = PlayerX - body.x;
            int delY = PlayerY - body.y;

            if ((Math.Abs(delX) <= 1) && (Math.Abs(delY) <= 1))
            {
                body.Move(delX, delY);
                return true;
            }
            else
                return false;
        }
    }

    // StillAI: Perfectly still. Obviously.
    [Serializable]
    public class StillAI : AI
    {
        public override void Loop(Entity body)
        {
        }
    }

    // DumbAI: move randomly
    [Serializable]
    public class DumbAI : AI
    {   
        public DumbAI(int frequency)
        {
            this.frequency = frequency;
            this.timer = Program.random.Next(0,frequency);
        }
        
        public override void Loop(Entity body)
        {
            MoveRandomly(body);
        }
    }

    [Serializable]
    public class SeekerAI : AI
    {
        bool waitcautiously;
        double castprob;
        double charmprob;
        int mindist = 10;

        public SeekerAI(string optionstring, Entity body)
        {
            this.frequency = 5;
            this.castprob = 0;
            this.charmprob = 0;
            this.waitcautiously = true;
            string[] options = optionstring.Split(","[0]);

            foreach (string u in options)
            {
                string[] phrase = u.Split("="[0]);
                switch (phrase[0])
                {
                    case "frequency":
                        {
                            this.frequency = int.Parse(phrase[1]);
                            break;
                        }
                    case "impatient":
                        {
                            this.waitcautiously = false;
                            break;
                        }
                    case "spell":
                        {
                            Spell spell = null;
                            foreach (Spell w in Program.spells.Where(item => item.name == phrase[1]))
                                spell = w;
                            if (spell != null)
                                body.Learn(spell);
                            break;
                        }
                    case "castprob":
                        {
                            this.castprob = double.Parse(phrase[1]);
                            break;
                        }
                    case "charmprob":
                        {
                            this.charmprob = double.Parse(phrase[1]);
                            break;
                        }
                    case "mindist":
                        {
                            this.mindist = int.Parse(phrase[1]);
                            break;
                        }
                }
            }

            this.timer = Program.random.Next(0, frequency);
        }
        
        public override void Loop(Entity body)
        {
            // if (!PhysicalAttack(body) && (timer == 0))
            if (timer == 0)
            {
                LearnedSpell spell = null;
                double maxrange = 9999;
                foreach (LearnedSpell u in body.spells.Where(item => Program.entities[0].WithinRange(body, item.range)))
                    if (u.range < maxrange)
                    {
                        spell = u;
                        maxrange = u.range;
                    }
                if ((spell != null) && (castprob > Program.random.NextDouble()))
                {
                    spell.Cast(body);
                }
                else if (!PhysicalAttack(body))
                {
                    Vector test = Helper.RollUp(Program.themap.smellmap, 60, 25, body.x, body.y);
                    if (test != null)
                        body.Move(test.x, test.y);
                    else if (!this.waitcautiously)
                        MoveRandomly(body);
                }
                timer = frequency;
            }
            else
            {
                timer = timer - 1;
                PhysicalAttack(body);
            }
        }

        protected override bool PhysicalAttack(Entity body)
        {
            int PlayerX = Program.entities[0].x;
            int PlayerY = Program.entities[0].y;
            int delX = PlayerX - body.x;
            int delY = PlayerY - body.y;

            if ((Math.Abs(delX) <= 1) && (Math.Abs(delY) <= 1))
            {
                if (body.canCharm(Program.entities[0]) && (charmprob > Program.random.NextDouble()))
                    Program.entities[0].Charm(body);
                else
                    body.Move(delX, delY);
                return true;
            }
            else
                return false;
        }
    }
}
