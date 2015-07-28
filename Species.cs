using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace borrdome
{
    [Serializable]
    public class Species
    {
        public string name { get; private set; }

        public int attack { get; private set; }
        public int defense { get; private set; }

        public string face { get; private set; }
        public ConsoleColor foregroundcolor { get; private set; }
        public ConsoleColor backgroundcolor { get; private set; }

        public bool gendered { get; private set; }
        public int eatingrate { get; private set; }
        public bool cantequip { get; private set; }
        public double throwingrange { get; private set; }

        public bool charmer { get; private set; }
        public bool easilycharmed { get; private set; }
        public bool undead { get; private set; }
        public bool ghost { get; private set; }

        public Entity.statuses resistance { get; private set; }

        public int minlevel { get; private set; }
        public int maxlevel { get; private set; }
        public double rarity { get; private set; }
        public bool unique { get; private set; }

        public string ai { get; private set; }

        string _droplist = "";

        public List<Item> droplist { get; private set; }
        public double droprate { get; private set; }

        public bool guarantee { get; private set; }

        

        public static Species FromXmlElement(XmlElement element)
        {
            var species = new Species();
            species.name = element.GetElementsByTagName("Name")[0].InnerText.Trim();
            species.face = element.GetElementsByTagName("Face")[0].InnerText.Trim();

            species.attack = int.Parse(element.GetElementsByTagName("Attack")[0].InnerText.Trim());
            species.defense = int.Parse(element.GetElementsByTagName("Defense")[0].InnerText.Trim());

            //species.foregroundcolor = (ConsoleColor)int.Parse(element.GetElementsByTagName("ForegroundColor")[0].InnerText.Trim());
            //species.backgroundcolor = (ConsoleColor)int.Parse(element.GetElementsByTagName("BackgroundColor")[0].InnerText.Trim());
            species.foregroundcolor = Colors.getColor(element.GetElementsByTagName("ForegroundColor")[0].InnerText.Trim());
            species.backgroundcolor = Colors.getColor(element.GetElementsByTagName("BackgroundColor")[0].InnerText.Trim());

            species.gendered = element.GetElementsByTagName("Gendered").Count == 1;
            species.eatingrate = int.Parse(element.GetElementsByTagName("EatingRate")[0].InnerText.Trim());
            species.cantequip = element.GetElementsByTagName("CantEquip").Count == 1;
            species.throwingrange = double.Parse(element.GetElementsByTagName("ThrowingRange")[0].InnerText.Trim());

            species.charmer = element.GetElementsByTagName("Charmer").Count == 1;
            species.easilycharmed = element.GetElementsByTagName("EasilyCharmed").Count == 1;

            species.ghost = element.GetElementsByTagName("Ghost").Count == 1;

            species.undead = element.GetElementsByTagName("Undead").Count == 1;

            if (element.GetElementsByTagName("Resist").Count == 1)
            {
                string resist = element.GetElementsByTagName("Resist")[0].InnerText.Trim();
                switch (resist)
                {
                    case "Paralyze":
                        {
                            species.resistance = Entity.statuses.Paralyzed;
                            break;
                        }
                    case "Poison":
                        {
                            species.resistance = Entity.statuses.Poison;
                            break;
                        }
                    case "Charm":
                        {
                            species.resistance = Entity.statuses.Charmed;
                            break;
                        }
                    default:
                        {
                            species.resistance = Entity.statuses.Null;
                            break;
                        }
                }
            }
            else
                species.resistance = Entity.statuses.Null;

            if (element.GetElementsByTagName("Guarantee").Count == 1)
            { species.guarantee = true; species.unique = true; }
            else
                species.guarantee = false;

            if (element.GetElementsByTagName("CantSpawn").Count == 1)
                species.minlevel = 9999;
            else
                species.minlevel = int.Parse(element.GetElementsByTagName("MinLevel")[0].InnerText.Trim());
            
            if (element.GetElementsByTagName("MaxLevel").Count == 1)
                species.maxlevel = int.Parse(element.GetElementsByTagName("MaxLevel")[0].InnerText.Trim());
            else
                species.maxlevel = 9999;

            if (element.GetElementsByTagName("Rarity").Count == 1)
                species.rarity = double.Parse(element.GetElementsByTagName("Rarity")[0].InnerText.Trim());
            else
                species.rarity = 1;

            species.unique = element.GetElementsByTagName("Unique").Count == 1;

            if (element.GetElementsByTagName("AI").Count == 1)
                species.ai = element.GetElementsByTagName("AI")[0].InnerText.Trim();
            else
                species.ai = "Still";

            if (element.GetElementsByTagName("Drops").Count == 1)
            {
                species._droplist = element.GetElementsByTagName("Drops")[0].InnerText.Trim();
                if (element.GetElementsByTagName("DropRate").Count == 1)
                    species.droprate = double.Parse(element.GetElementsByTagName("DropRate")[0].InnerText.Trim());
                else
                    species.droprate = 1;
            }
            else
                species.droprate = 0;

            return species;
        }

        public void fillDroplist()
        {
            this.droplist = new List<Item>();
            if (this._droplist != "")
            {
                string[] drops = this._droplist.Split(","[0]);

                foreach (string u in drops)
                {
                    foreach (Item w in Program.items.Where(item => item.truename == u))
                        this.droplist.Add(w);
                }
            }
        }

        public override string ToString()
        {
            return name;
        }

        public AI getAI(Entity body)
        {
            return AI.FromString(ai, body);
        }
    }
}
