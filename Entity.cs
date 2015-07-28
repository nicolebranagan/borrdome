using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    [Serializable]
    public class Entity
    {
        public enum genders
        {
            Female,
            Male,
            None
        }

        public enum statuses
        {
            Normal,
            Poison,
            Charmed,
            Paralyzed,
            Null // your status should never be null
        }
        
        public int x { get; set; }
        public int y { get; set; }
        public Species species { get; set; }

        int _maxspells = 4;

        public int maxspells
        {
            get
            {
                return _maxspells;
            }
            set
            {
                if (value < 10)
                    _maxspells = value;
                else
                    _maxspells = 10;
            }
        }
        public List<InventoryItem> inventory;
        public List<LearnedSpell> spells;

        protected ConsoleColor foregroundcolor;
        protected ConsoleColor backgroundcolor;
        protected string face;
        protected genders gender;

        public string name {
            get
            {
                string name;
                if (this.species.gendered)
                    name = String.Concat(this.GenderMarker(), " ", this.species.ToString());
                else
                    name = this.species.ToString();
                return name;
            }
        }

        int _attack = 0;
        int _defense = 0;

        public int health = 100;
        public int attack 
        { 
            get 
            { 
                int atk = _attack;
                foreach (InventoryItem u in this.inventory.Where(item => item.equipped))
                    atk = atk + u.effect().Attack;
                return atk;
            } 

            protected set
            {
                _attack = value;
            }
        }
        public int defense
        {
            get
            {
                int def = _defense;
                foreach (InventoryItem u in this.inventory.Where(item => item.equipped))
                    def = def + u.effect().Defense;
                return def;
            }

            protected set
            {
                _defense = value;
            }
        }

        public statuses status = statuses.Normal;
        int statusTimer = 0;

        bool dead = false;

        public int food = 0;

        public Entity(int x, int y, string face, Species species, ConsoleColor fgcolor, ConsoleColor bgcolor, genders gender = genders.Female)
        {
            this.x = x;
            this.y = y;
            this.face = face;
            this.species = species;

            this.attack = species.attack;
            this.defense = species.defense;

            this.foregroundcolor = fgcolor;
            this.backgroundcolor = bgcolor;
            if (this.species.gendered)
                this.gender = gender;
            else
                this.gender = genders.None;

            this.inventory = new List<InventoryItem>();
            this.spells = new List<LearnedSpell>();
        }

        protected Entity()
        {
           // only inheritors are allowed to use this
           this.inventory = new List<InventoryItem>();
           this.spells = new List<LearnedSpell>();
        }

        public void Draw()
        {
            if (!dead)
            {
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = backgroundcolor;
                Console.ForegroundColor = foregroundcolor;
                Console.Write(face);
            }
        }

        public void Erase()
        {
            Program.themap.DrawTile(this.x, this.y);
        }

        public virtual void Move(int x, int y)
        {
            if (!(this.status == statuses.Paralyzed) && Program.themap.CanMove(this.x + x, this.y + y, this.species.ghost))
            {
                this.Erase();
                this.x += x;
                this.y += y;
            }
            else if (this == Program.entities[0])
                if (this.status == statuses.Paralyzed)
                    Program.Report("Fast asleep!");
                else
                    Program.Report("Can't move there!");

        }

        public virtual void Loop()
        {
            if (status != statuses.Normal)
            {
                if (status == statuses.Poison)
                {
                    this.health = this.health - 10;
                    if (this.health <= 0)
                    {
                        Program.Report(String.Concat(name, " died!"));
                        this.die("Poison");
                    }
                }
                if (statusTimer == 0)
                {
                    statuses OldStatus = this.status;
                    this.status = statuses.Normal;
                    if (this == Program.entities[0])
                    {
                        string message = "No longer ";
                        switch (OldStatus)
                        {
                            case statuses.Charmed:
                                { message = String.Concat(message, "charmed!"); break; }
                            case statuses.Poison:
                                { message = String.Concat(message, "poisoned!"); break; }
                            case statuses.Paralyzed:
                                { message = String.Concat(message, "paralyzed!"); break; }
                        }
                        Program.Report(message);
                    }
                }
                else
                    statusTimer = statusTimer - 1;
            }

            foreach (LearnedSpell u in this.spells)
                u.Loop();
        }

        public string GenderMarker()
        {
            if (gender == genders.Female)
                return "F";
            else if (gender == genders.Male)
                return "M";
            else
                return "X";
        }

        protected void defend(double damage, string attacker)
        {
            // To do: Reduce damage based on modifiers

            int damageTaken = (int)Math.Ceiling(damage / (defense / 10));

            this.health = this.health - damageTaken;
            
            string name;
            if (this == Program.entities[0])
                name = "You";
            else if (this.species.gendered)
                name = String.Concat(this.GenderMarker(), " ", this.species.ToString());
            else
                name = this.species.ToString();

            if (this.health > 0)
                Program.Report(String.Concat(name, " took ", damageTaken.ToString(), " damage"));
            else
            {
                Program.Report(String.Concat(name, " died!"));
                this.die(attacker);
            }
            
        }

        public virtual void die(string murderer)
        {
            Program.themap.DrawTile(this.x, this.y);
            this.dead = true;
        }

        protected void attackTarget(Entity target)
        {
            // To do: Calculate attack amount
            double rand = Program.random.NextDouble();
            rand = (rand * 0.4) + 0.8; // from 0.8 to 1.2
            target.defend(this.attack * rand, this.name);
        }

        public bool Transform(AttributeChange change, string transformer)
        {
            bool condition = true;
            if (change.Spell != null) condition = this.Learn(change.Spell);

            if (condition)
            {
                this.LimitedTransform(change);
                if ((this.species.undead) && (change.Health > 0))
                    this.health -= change.Health;
                else
                    this.health += change.Health;
                if (this.health <= 0)
                    this.die(transformer);
                this._attack += change.Attack;
                this._defense += change.Defense;
                this.food += change.Food;
                this.maxspells += change.MoreSpells;
            }
            return condition;
        }

        public void LimitedTransform(AttributeChange change)
        {
            // Certain changes happen whether you equip or use it
            bool genderchange = false;
            bool specieschange = false;

            string oldname = this.name;

            if (change.NewSpecies != null)
            {
                this.ChangeSpecies(change.NewSpecies);
                specieschange = true;
            }

            if ((this.species.gendered || ((change.NewSpecies != null) && (change.NewSpecies.gendered))) && (change.NewGender != AttributeChange.StatusGenders.None))
            {
                if (change.NewGender == AttributeChange.StatusGenders.Flip)
                    this.FlipGender();
                else if (change.NewGender == AttributeChange.StatusGenders.Female)
                    this.gender = genders.Female;
                else if (change.NewGender == AttributeChange.StatusGenders.Male)
                    this.gender = genders.Male;
                genderchange = true;
            }

            if (change.Status != statuses.Null)
                 this.ChangeStatus(change.Status);

            if (this == Program.entities[0])
            {
                if (genderchange)
                    Program.Report(String.Concat("You are now a ", Program.entities[0].GenderMarker(), " ", Program.entities[0].species.ToString()));
                else if (specieschange)
                    Program.Report(String.Concat("You are now a ", Program.entities[0].species.ToString()));
            }
            else if (genderchange || specieschange)
            {
                Program.Report(String.Concat(oldname, " became a ", this.name,"!"));
            }
        }

        public void FlipGender()
        {
            if (this.gender == genders.Female)
                this.gender = genders.Male;
            else if (this.gender == genders.Male)
                this.gender = genders.Female;
        }

        public void Charm(Entity charmer)
        {
            bool failedtocharm = true;
            if (charmer.canCharm(this))
            {
                double percentage = 0.7;
                if (this.gender == charmer.gender) percentage = 0.3;
                if (percentage > Program.random.NextDouble())
                {
                    failedtocharm = false;
                    this.ChangeStatus(statuses.Charmed);
                }
            }

            if (failedtocharm && (charmer == Program.entities[0]))
                Program.Report(String.Concat(name, " ignores your advances!"));
        }

        public void ChangeStatus(statuses NewStatus)
        {
            if (this.species.resistance == NewStatus)
                return;

            this.status = NewStatus;
            if (NewStatus == statuses.Poison)
                this.statusTimer = 6;
            else
                this.statusTimer = 12;
            string message = "";
            if (this != Program.entities[0])
            {
                message = String.Concat(this.name, " is now ");
            }
            switch (NewStatus)
            {
                case statuses.Charmed:
                    { message = String.Concat(message, "Charmed!"); break; }
                case statuses.Poison:
                    { message = String.Concat(message, "Poisoned!"); break; }
                case statuses.Paralyzed:
                    { message = String.Concat(message, "Paralyzed!"); break; }
            }

            if (NewStatus == statuses.Normal)
            {
                if (this != Program.entities[0])
                    message = String.Concat(this.name, " seems revived!");
                else
                    message = String.Concat("You feel revived!");
            }

            Program.Report(message);
        }

        public void ChangeSpecies(Species NewSpecies)
        {
            Species oldSpecies = this.species;
            this.species = NewSpecies;
            this.foregroundcolor = this.species.foregroundcolor;
            this.backgroundcolor = this.species.backgroundcolor;
            this.attack = species.attack;
            this.defense = species.defense;
            if (this != Program.entities[0])
                this.face = this.species.face;
            if (this.species.cantequip)
            {
                foreach (InventoryItem u in this.inventory)
                    u.equipped = false;
            }
            if (!(oldSpecies.gendered) && NewSpecies.gendered && (this.gender == genders.None))
                if (Program.random.Next(0, 2) > 0)
                    this.gender = genders.Female;
                else
                    this.gender = genders.Male;
        }

        public bool Get(Item newItem)
        {
            InventoryItem item = new InventoryItem(newItem);
            if (inventory.Count == 10)
            {
                if (this == Program.entities[0])
                    Program.Report(String.Concat("Can't get ", item.name, "- Inventory is full!"));
                return false;
            }
            else
            {
                inventory.Add(item);
                if (this == Program.entities[0])
                    Program.Report(String.Concat("Got ", item.name));
                return true;
            }
        }

        public void Equip(InventoryItem item)
        {
            if (this.inventory.Contains(item) && (((this.CanEquip(item)) && (item.equipped == false)) || (item.equipped == true)) && (item.equippable == true) && (!this.species.cantequip))
            {
                item.Equip(this);
            }
            else if (this == Program.entities[0])
                Program.Report(String.Concat("Can't equip ", item.name, " right now!"));
        }

        public bool WithinRange(Entity enemy)
        {
            return ((enemy != this) && (Helper.WithinCircle(this.x, this.y, enemy.species.throwingrange, enemy.x, enemy.y)));
        }

        public bool WithinRange(Entity enemy, double radius)
        {
            return ((enemy != this) && (Helper.WithinCircle(this.x, this.y, radius, enemy.x, enemy.y)));
        }

        private bool CanEquip(InventoryItem item)
        {
            bool result = true;

            foreach (InventoryItem u in this.inventory.Where(u => u.equipped))
                if (u.item.equipslot == item.item.equipslot)
                    { result = false; break; }

            return result;
        }

        public bool Learn(Spell spell)
        {
            LearnedSpell newSpell = new LearnedSpell(spell);
            if ((spells.Count < maxspells) && (this.spells.Where(item => (item.DuplicateCheck(spell) == true)).Count() == 0))
            {
                this.spells.Add(new LearnedSpell(spell));
                if (this == Program.entities[0]) Program.Report(String.Concat("Learned ", spell.name,"!"));
                return true;
            }
            else
                return false;
        }

        public void Forget(LearnedSpell spell)
        {
            if (spells.Contains(spell))
            {
                spells.Remove(spell);
                if (this == Program.entities[0])
                    Program.Report(String.Concat("Forgot ", spell.name, "!"));
            }
        }

        public virtual Entity GetTarget(double range)
        {
            if (Program.entities[0].WithinRange(this, range))
            {
                return Program.entities[0];
            }
            else
                return null;
        }

        public bool canCharm(Entity target)
        {
            if (this.species.gendered && target.species.gendered)
                return ((target.species == this.species) || (target.species.easilycharmed) || (this.species.charmer));
            else
                return false;
        }

        public AttributeChange.StatusGenders GetStatusGender()
        {
            if (this.gender == genders.Male)
                return AttributeChange.StatusGenders.Male;
            else if (this.gender == genders.Male)
                return AttributeChange.StatusGenders.Female;
            else
                return AttributeChange.StatusGenders.None;
        }
    }
}
