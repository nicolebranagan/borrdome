using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    [Serializable]
    class NPC : Entity
    {
        AI ai;
        Item dropitem = null;
        
        public NPC(int x, int y, Species species)
        {
            this.x = x;
            this.y = y;
            this.species = species;
            this.attack = species.attack;
            this.defense = species.defense;
            this.face = this.species.face;
            this.foregroundcolor = this.species.foregroundcolor;
            this.backgroundcolor = this.species.backgroundcolor;
            if (this.species.gendered)
            {
                if (Program.random.Next(0, 2) == 0)
                    this.gender = genders.Female;
                else
                    this.gender = genders.Male;
            }
            else
                this.gender = genders.None;

            this.ai = this.species.getAI(this);

            if (this.species.droprate != 0)
            {
                int rand = Program.random.Next(0, this.species.droplist.Count);
                if (this.species.droprate > Program.random.NextDouble())
                    this.dropitem = this.species.droplist[rand];
            }
        }

        public override void Loop()
        {
            this.ai.Loop(this);
            base.Loop();
        }

        public override void die(string murderer)
        {
            if (this.dropitem != null)
                Program.things.Add(new ItemThing(x, y, dropitem));
            base.die(murderer);
        }

        public override void Move(int x, int y)
        {
            bool condition = true;
            bool canAttack = false;

            if ((this.status == statuses.Normal) || (this.status == statuses.Poison))
                canAttack = true;

            foreach (Entity u in Program.entities.Where(item => item != this))
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    condition = false;
                    if (u == Program.entities[0] && canAttack) // only attack the player
                        this.attackTarget(u);
                }
            }

            foreach (Thing u in Program.things)
            {
                if ((u.x == (this.x + x)) && (u.y == (this.y + y)))
                {
                    condition = u.Bump(this);
                }
            }
            

            if (condition)
                base.Move(x, y);
        }
    }
}
