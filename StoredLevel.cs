using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace borrdome
{
    [Serializable]
    public class StoredLevel
    {
        static List<StoredLevel> storedlevels = new List<StoredLevel>();

        int index;
        Map map;
        List<Entity> entities;
        List<Thing> things;

        public static void Unthaw(int index, Entity player)
        {
            StoredLevel uncoverLevel = null;

            foreach (StoredLevel u in storedlevels.Where(item => item.index == index))
                uncoverLevel = u;

            if (uncoverLevel == null)
                NewDungeonLevel(index, player);
            else
            {
                Program.themap = uncoverLevel.map;
                Program.entities = new List<Entity>();
                Program.entities.Add(player);
                Program.entities.AddRange(uncoverLevel.entities);
                Program.things = uncoverLevel.things;
            }
        }

        public static void Thaw(int index)
        {
            storedlevels.RemoveAll(item => item.index == index);

            Program.entities.Remove(Program.entities[0]);
            storedlevels.Add(new StoredLevel(index, Program.themap, Program.entities, Program.things));
        }

        static void NewDungeonLevel(int level, Entity player)
        {
            Program.themap = new Map(0.05, 0.07, level);

            Program.entities = new List<Entity>();
            Program.entities.Add(player);

            Program.things = new List<Thing>();
            Program.themap.SpawnThings(level);
            Program.themap.SpawnEnemies(level);
        }

        public StoredLevel(int index, Map map, List<Entity> entities, List<Thing> things)
        {
            this.index = index;
            this.map = map;
            this.entities = entities;
            this.things = things;
        }

        public static List<StoredLevel> getLevels()
        {
            return storedlevels;
        }

        public static void setLevels(List<StoredLevel> list)
        {
            storedlevels = list;
        }
    }

    [Serializable]
    public class StoredLevels
    {
        public List<StoredLevel> storedlevels;
        public Entity player;
        public int currentlevel;

        public static void Save()
        {
            Entity player = Program.entities[0];
            StoredLevel.Thaw(Program.currentlevel);

            StoredLevels slvl = new StoredLevels(StoredLevel.getLevels(), player);
            Stream str = File.Open("save.dat", FileMode.Create);
            BinaryFormatter ber = new BinaryFormatter();

            ber.Serialize(str, slvl);
            str.Close();
        }

        public static void Load()
        {
            StoredLevels slvl;
            Stream str = File.Open("save.dat", FileMode.Open);
            BinaryFormatter ber = new BinaryFormatter();
            slvl = (StoredLevels)ber.Deserialize(str);
            str.Close();

            StoredLevel.setLevels(slvl.storedlevels);
            StoredLevel.Unthaw(slvl.currentlevel, slvl.player);
            Program.DrawGame();
        }

        StoredLevels(List<StoredLevel> list, Entity player)
        {
            this.storedlevels = list;
            this.player = player;
            this.currentlevel = Program.currentlevel;
        }

        StoredLevels()
        {
        }
    }
}
