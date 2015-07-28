using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    [Serializable]
    public class Item
    {
        public enum equipslots
        {
            Helmet,
            Armor,
            Weapon,
            Ring
        }
        
        public string name { get; set; }
        public string truename { get; set; }

        public bool potion { get; private set; } // this means that the item can be identified by use

        public string face { get; protected set; }
        public ConsoleColor foregroundcolor { get; protected set; }
        public ConsoleColor backgroundcolor { get; protected set; }
        public string description { get; set; }

        public bool equippable { get; set; } // must be equipped
            public equipslots equipslot { get; set; }

        public bool useable { get; set; } // must be used to get effect
        public bool throwable { get; private set; }
        public double curseprob { get; private set; }

        public bool onbump { get; private set; } // works when you bump it, not when you use it
        public bool ontake { get; private set; } // works when you get it, not when you use it
        public bool noshow { get; private set; } // in the list
        public bool remain { get; private set; }

        public AttributeChange delta = new AttributeChange();

        public int minlevel { get; private set; }
        public int maxlevel { get; private set; }
        public double rarity { get; private set; }
        public bool unique { get; private set; } 

        public int value { get; private set; }

        public static Item FromXmlElement(XmlElement element)
        {
            Item item = new Item();

            item.name = element.GetElementsByTagName("Name")[0].InnerText.Trim();

            if (element.GetElementsByTagName("TrueName").Count == 1)
                item.truename = element.GetElementsByTagName("TrueName")[0].InnerText.Trim();
            else
                item.truename = item.name;

            item.potion = ((element.GetElementsByTagName("Potion").Count == 1) || (item.name == "Potion"));

            // Appearance

            item.face = element.GetElementsByTagName("Face")[0].InnerText.Trim();
            //item.foregroundcolor = (ConsoleColor)int.Parse(element.GetElementsByTagName("ForegroundColor")[0].InnerText.Trim());
            //item.backgroundcolor = (ConsoleColor)int.Parse(element.GetElementsByTagName("BackgroundColor")[0].InnerText.Trim());
            item.foregroundcolor = Colors.getColor(element.GetElementsByTagName("ForegroundColor")[0].InnerText.Trim());
            item.backgroundcolor = Colors.getColor(element.GetElementsByTagName("BackgroundColor")[0].InnerText.Trim());
            item.description = element.GetElementsByTagName("Description")[0].InnerText.Trim();

            if (item.potion)
            {
                item.name = String.Concat(Colors.getColorName((int)item.foregroundcolor), " ", item.name);
            }

            // Use flags

            item.equippable = element.GetElementsByTagName("Equippable").Count == 1;
            item.useable = element.GetElementsByTagName("Useable").Count == 1;
            item.onbump = element.GetElementsByTagName("OnBump").Count == 1;
            item.ontake = element.GetElementsByTagName("OnTake").Count == 1;
            item.noshow = element.GetElementsByTagName("NoShow").Count == 1;
            item.remain = element.GetElementsByTagName("Remain").Count == 1;
            item.throwable = element.GetElementsByTagName("Throwable").Count == 1;

            if (element.GetElementsByTagName("CurseProbability").Count == 1)
                item.curseprob = double.Parse(element.GetElementsByTagName("CurseProbability")[0].InnerText.Trim());
            else
                item.curseprob = 0;

            // Equip nonsense
            if (element.GetElementsByTagName("EquipmentType").Count == 1)
            {
                string typename = element.GetElementsByTagName("EquipmentType")[0].InnerText.Trim();
                switch (typename)
                {
                    case "Helmet":
                        { item.equipslot = equipslots.Helmet; break; }
                    case "Armor":
                        { item.equipslot = equipslots.Armor; break; }
                    case "Weapon":
                        { item.equipslot = equipslots.Weapon; break; }
                    default:
                        { item.equipslot = equipslots.Ring; break; }
                }
            }
            else
                item.equipslot = equipslots.Ring;

            // Change effects (All wrapped up in "delta")
            if (element.GetElementsByTagName("Health").Count == 1)
                item.delta.Health = int.Parse(element.GetElementsByTagName("Health")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Attack").Count == 1)
                item.delta.Attack = int.Parse(element.GetElementsByTagName("Attack")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Defense").Count == 1)
                item.delta.Defense = int.Parse(element.GetElementsByTagName("Defense")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Food").Count == 1)
                item.delta.Food = int.Parse(element.GetElementsByTagName("Food")[0].InnerText.Trim());
            if (element.GetElementsByTagName("MoreSpells").Count == 1)
                item.delta.MoreSpells = int.Parse(element.GetElementsByTagName("MoreSpells")[0].InnerText.Trim());
            if (element.GetElementsByTagName("Gender").Count == 1)
            {
                string result = element.GetElementsByTagName("Gender")[0].InnerText.Trim();
                if (result == "Female")
                    item.delta.NewGender = AttributeChange.StatusGenders.Female;
                else if (result == "Male")
                    item.delta.NewGender = AttributeChange.StatusGenders.Male;
                else if (result == "Flip")
                    item.delta.NewGender = AttributeChange.StatusGenders.Flip;
            }
            if (element.GetElementsByTagName("Species").Count == 1)
                item.delta.setSpecies(element.GetElementsByTagName("Species")[0].InnerText.Trim());

            if (element.GetElementsByTagName("Spell").Count == 1)
                item.delta.setSpell(element.GetElementsByTagName("Spell")[0].InnerText.Trim());

            // Status effects - you can only have one
            if (element.GetElementsByTagName("Normal").Count == 1)
                item.delta.Status = Entity.statuses.Normal;
            if (element.GetElementsByTagName("Paralyze").Count == 1)
                item.delta.Status = Entity.statuses.Paralyzed;
            if (element.GetElementsByTagName("Charm").Count == 1)
                item.delta.Status = Entity.statuses.Charmed;
            if (element.GetElementsByTagName("Poison").Count == 1)
                item.delta.Status = Entity.statuses.Poison;

            // Gameplay characteristics
            if (element.GetElementsByTagName("CantSpawn").Count == 1)
                item.minlevel = 9999;
            else
                item.minlevel = int.Parse(element.GetElementsByTagName("MinLevel")[0].InnerText.Trim());

            if (element.GetElementsByTagName("MaxLevel").Count == 1)
                item.maxlevel = int.Parse(element.GetElementsByTagName("MaxLevel")[0].InnerText.Trim());
            else
                item.maxlevel = 9999;

            if (element.GetElementsByTagName("Rarity").Count == 1)
                item.rarity = double.Parse(element.GetElementsByTagName("Rarity")[0].InnerText.Trim());
            else
                item.rarity = 1;

            item.unique = element.GetElementsByTagName("Unique").Count == 1;

            item.value = int.Parse(element.GetElementsByTagName("Value")[0].InnerText.Trim());

            return item;
        }

        public static Item Copy(Item o_item)
        {
            Item c_item = new Item();
            c_item.name = o_item.name;
            c_item.truename = o_item.truename;

            c_item.potion = o_item.potion;

            c_item.face = o_item.face;
            c_item.backgroundcolor = o_item.backgroundcolor;
            c_item.foregroundcolor = o_item.foregroundcolor;
            c_item.description = o_item.description;

            c_item.equippable = o_item.equippable;
            c_item.useable = o_item.useable;
            c_item.ontake = c_item.ontake;
            c_item.onbump = o_item.onbump; // how the fuck did you pick up an instant item
            c_item.throwable = o_item.throwable;
            c_item.curseprob = o_item.curseprob;
            c_item.remain = o_item.remain;

            c_item.equipslot = o_item.equipslot;

            c_item.delta = o_item.delta;

            c_item.minlevel = o_item.minlevel;
            c_item.rarity = o_item.rarity;
            c_item.value = o_item.value;

            return c_item;
        }

        public void Draw(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.BackgroundColor = this.backgroundcolor;
            Console.ForegroundColor = this.foregroundcolor;
            Console.Write(this.face);
        }

        public void Throw(Entity target)
        {
            target.Transform(this.delta, this.name);
        }

        public string GetSlotName()
        {
            switch (this.equipslot)
            {
                case equipslots.Helmet:
                     return "Helmet";
                case equipslots.Armor:
                     return "Armor"; 
                case equipslots.Weapon:
                     return "Weapon"; 
                default:
                     return "Ring"; 
            }
        }
    }

    [Serializable]
    public class InventoryItem
    {
        public Item item;

        public string name { get; protected set; }

        public bool cursed = false;
        public bool equipped;

        public bool useable;
        public bool equippable;

        public InventoryItem(Item item)
        {
            this.name = item.name;
            this.item = Item.Copy(item);
            this.useable = item.useable;
            this.equippable = item.equippable;
            
            if (this.item.curseprob > Program.random.NextDouble())
                this.cursed = true;
            this.equipped = false;
        }

        public virtual bool Use(Entity target, bool ForceUse = false)
        {
            bool ret;
            if (!item.useable && !ForceUse) return false;
            else
            {
                ret = target.Transform(item.delta, this.name);
            }
            if (item.potion)
                this.Identify();
            return ret;
        }

        public virtual void Equip(Entity target)
        {
            if (!equipped)
            {
                target.LimitedTransform(item.delta);
                if (target == Program.entities[0])
                    Program.Report(String.Concat("You equipped ", this.item.name, "!"));
                this.equipped = true;
            }
            else if (!cursed && equipped)
            {
                this.equipped = false;
                if (target == Program.entities[0])
                    Program.Report(String.Concat("Unequipped ", this.item.name, "!"));
            }
            else if (cursed && equipped)
            {
                if (target == Program.entities[0])
                    Program.Report(String.Concat(this.item.name, " won't come off!"));
            }
        }

        public AttributeChange effect()
        {
            return this.item.delta;
        }

        public void Draw(int x, int y)
        {
            this.item.Draw(x, y);
        }

        public void Identify()
        {
            string newdesc = String.Concat("It's the ", this.item.truename, ".");
            this.name = this.item.truename;
            this.item.description = newdesc;

            foreach (Item u in Program.items.Where(item => item.truename == this.item.truename))
            {
                u.name = u.truename;
                u.description = newdesc;
            }
            foreach (InventoryItem u in Program.entities[0].inventory.Where(item => item.item.truename == this.item.truename))
            {
                u.name = u.item.truename;
                u.item.description = newdesc;
            }
            foreach (Thing u in Program.things)
            {
                u.Refresh();
            }
            Program.Report(String.Concat("It's the ", this.name, "!"));
        }
    }

    [Serializable]
    public class AttributeChange
    {
        public enum StatusGenders
        {
            None,
            Female,
            Male,
            Flip
        }
        
        public int Health = 0;
        public int Attack = 0;
        public int Defense = 0;
        public int Food = 0;
        public int MoreSpells = 0;
        public StatusGenders NewGender = StatusGenders.None;
        public Species NewSpecies = null;
        public Entity.statuses Status = Entity.statuses.Null;
        public Spell Spell = null;

        public void setSpecies(string name)
        {
            foreach (Species u in Program.species)
            {
                if (u.name == name)
                {
                    this.NewSpecies = u;
                    break;
                }
            }
        }

        public void setSpell(string name)
        {
            foreach (Spell u in Program.spells)
            {
                if (u.name == name)
                {
                    this.Spell = u;
                    break;
                }
            }
        }
    }
}
