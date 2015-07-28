using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    [Serializable]
    public class Spell
    {
        public enum targetTypes
        {
            Self,
            Enemy,
            Item
        }
        
        public string name { get; set; }

        public int chargetime { get; set; }
        public int charges { get; set; }
        public double probability { get; set; }

        public bool targetself = false;
        public bool target1enemy = false;
        public bool targetenemies = false;
        public bool targetitem = false;
        public bool targetthing = false;
        public bool identify = false;
        public bool uncurse = false;
        public bool clone = false;
        public double range;

        public AttributeChange delta = new AttributeChange();

        public static Spell FromXmlElement(XmlElement element)
        {
            Spell spell = new Spell();

            spell.name = element.GetElementsByTagName("Name")[0].InnerText.Trim();

            spell.chargetime = int.Parse(element.GetElementsByTagName("ChargeTime")[0].InnerText.Trim());
            spell.charges = int.Parse(element.GetElementsByTagName("Charges")[0].InnerText.Trim());

            spell.probability = double.Parse(element.GetElementsByTagName("Probability")[0].InnerText.Trim());

            // Target
            foreach (XmlNode u in element.GetElementsByTagName("Target"))
            {
                string target = u.InnerText.Trim();
                if (target == "Self") spell.targetself = true;
                if (target == "Enemy") spell.target1enemy = true;
                if (target == "Enemies") spell.targetenemies = true;
                if (target == "Item") spell.targetitem = true;
                if (target == "Thing") spell.targetthing = true;
            }

            if (element.GetElementsByTagName("Range").Count == 1)
                spell.range = int.Parse(element.GetElementsByTagName("Range")[0].InnerText.Trim());
            else
                spell.range = 1;

            spell.identify = element.GetElementsByTagName("Identify").Count == 1;
            spell.uncurse = element.GetElementsByTagName("Uncurse").Count == 1;
            spell.clone = element.GetElementsByTagName("Clone").Count == 1;

            // Change effects (All wrapped up in "delta")
            if (element.GetElementsByTagName("Health").Count == 1)
                spell.delta.Health = int.Parse(element.GetElementsByTagName("Health")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Attack").Count == 1)
                spell.delta.Attack = int.Parse(element.GetElementsByTagName("Attack")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Defense").Count == 1)
                spell.delta.Defense = int.Parse(element.GetElementsByTagName("Defense")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Food").Count == 1)
                spell.delta.Food = int.Parse(element.GetElementsByTagName("Food")[0].InnerText.Trim());
            if (element.GetElementsByTagName("MoreSpells").Count == 1)
                spell.delta.MoreSpells = int.Parse(element.GetElementsByTagName("MoreSpells")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Gender").Count == 1)
            {
                string result = element.GetElementsByTagName("Gender")[0].InnerText.Trim();
                if (result == "Female")
                    spell.delta.NewGender = AttributeChange.StatusGenders.Female;
                else if (result == "Male")
                    spell.delta.NewGender = AttributeChange.StatusGenders.Male;
                else if (result == "Flip")
                    spell.delta.NewGender = AttributeChange.StatusGenders.Flip;
            }
            if (element.GetElementsByTagName("Species").Count == 1)
                spell.delta.setSpecies(element.GetElementsByTagName("Species")[0].InnerText.Trim());

            // Status effects - you can only have one
            if (element.GetElementsByTagName("Paralyze").Count == 1)
                spell.delta.Status = Entity.statuses.Paralyzed;
            if (element.GetElementsByTagName("Charm").Count == 1)
                spell.delta.Status = Entity.statuses.Charmed;
            if (element.GetElementsByTagName("Poison").Count == 1)
                spell.delta.Status = Entity.statuses.Poison;

            return spell;
        }

        public bool Cast(Entity caster, bool willwork)
        {
            bool result = true;
            AttributeChange del = this.delta;
            if (clone)
            {
                Entity source = caster.GetTarget(this.range);
                if (source != null)
                {
                    del = new AttributeChange();
                    del.NewSpecies = source.species;
                    del.NewGender = source.GetStatusGender();
                    del.Status = source.status;
                }
                else
                {
                    if (caster == Program.entities[0]) Program.Report("Invalid target.");
                    return false;
                }
            }

            if (targetself)
                {
                    if (willwork) caster.Transform(del, "themself");
                }
            if (target1enemy)
                {
                    Entity spelltarget = caster.GetTarget(this.range);
                    if (spelltarget != null)
                        if (willwork) spelltarget.Transform(del, caster.name);
                    else
                    {
                        if (caster == Program.entities[0]) Program.Report("Invalid target.");
                        result = false;
                    }
                }

            if (targetenemies)
            {
                if (willwork) 
                    foreach (Entity u in Program.entities.Where(item => item.WithinRange(caster, this.range)))
                        u.Transform(del, caster.name);
            }
            if (targetitem)
            {
                if (caster == Program.entities[0])
                {
                    InventoryItem item = Program.ItemSelectionScreen(String.Concat("Which item to use ", this.name, " on?"));
                    if (item != null)
                    {
                        if (willwork)
                        {
                            if (this.identify) item.Identify();
                            else if (this.uncurse) item.cursed = false;
                        }
                    }
                    else
                    {
                        Program.Report("Invalid target.");
                        result = false;
                    }
                }
            }
            if (targetthing)
            {
                if (caster == Program.entities[0])
                {
                    Thing item = Program.TargetThing(this.range);
                    if (item != null)
                    {
                        if (willwork)
                        {
                            item.Destroy();
                        }
                    }
                    else
                    {
                        Program.Report("Invalid target.");
                        result = false;
                    }
                }
            }
            return result;
        }
    }

    [Serializable]
    public class LearnedSpell
    {
        Spell spell;
        
        public string name;
        public int charges;
        int timer;
        public double range
        {
            get
            {
                return this.spell.range;
            }
        }

        public LearnedSpell(Spell spell)
        {
            this.spell = spell;
            this.name = spell.name;
            this.charges = spell.charges;
            this.timer = spell.chargetime;
        }

        public void Loop()
        {
            if (this.timer == 0)
            {
                this.charges = spell.charges;
                this.timer = spell.chargetime;
            }
            else if (this.charges == 0)
            {
                this.timer = this.timer - 1;
            }
        }

        public void Cast(Entity user)
        {
            string oldname = user.name;
            bool player = (user == Program.entities[0]);
            bool success = false;
            bool willwork = true;
            if (this.charges != 0)
            {
                willwork = (spell.probability > Program.random.NextDouble());
                success = this.spell.Cast(user, willwork);
                if (success) this.charges = this.charges - 1;
                if (!willwork && player) Program.Report("Spell failed!");
                else if (willwork && player) Program.Report(String.Concat("Cast ", this.name, "!"));
                else if (willwork && !player) Program.Report(String.Concat(oldname, " used ", this.name, "!"));
            }
            else if (player) Program.Report("Not enough charges!");
        }

        public bool DuplicateCheck(Spell spell)
        {
            return (spell == this.spell);
        }
    }
}
