using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DtD_LootViewer
{
    class lootContainernames
    {

        public static Dictionary<int,string> getNames()
        {
            //Initialize the names of Each of the Container IDs
            Dictionary<int, string> tempNames = new Dictionary<int, string>();

            //Attempt to Load the Container Names from the XML file, if Failed, set Defaults
            try
            {
                System.Xml.Linq.XElement rootElement = System.Xml.Linq.XElement.Load("containernames.xml");
                foreach(KeyValuePair<string, string> kv in rootElement.Elements().ToDictionary(key => key.Name.ToString(), val => val.Value))
                {
                    tempNames.Add(Int32.Parse(kv.Key.Substring(2)), kv.Value);
                }
            }
            catch
            {
                //File doesn't exist, create records manually
                tempNames.Add(1, "Misc Storage");
                tempNames.Add(2, "Trash");
                tempNames.Add(3, "Cabinet");
                tempNames.Add(4, "Fridge");
                tempNames.Add(5, "Oven");
                tempNames.Add(6, "Nightstand/Desk");
                tempNames.Add(7, "Medicine Cabinet");
                tempNames.Add(8, "Sink");
                tempNames.Add(9, "Backpacks/duffles");
                tempNames.Add(10, "Empty Storage Containers");
                tempNames.Add(11, "Small Safes");
                tempNames.Add(14, "Mailbox");
                tempNames.Add(15, "Zombie Loot");
                tempNames.Add(16, "Trash Can");
                tempNames.Add(17, "Zombie Cop");
                tempNames.Add(19, "Car");
                tempNames.Add(20, "Air Conditioner (Not used after 14.6)");
                tempNames.Add(21, "Zombie Nurse");
                tempNames.Add(22, "Garage Storage");
                tempNames.Add(24, "Hornets");
                tempNames.Add(25, "Bird Nest");
                tempNames.Add(34, "Supply Crate");
                tempNames.Add(35, "Stump");
                tempNames.Add(36, "Suitcase");
                tempNames.Add(37, "Corpse");
                tempNames.Add(38, "Zombie Fat");
                tempNames.Add(39, "Shotgun Messiah");
                tempNames.Add(40, "Munitions Box");
                tempNames.Add(41, "Player Backpack");
                tempNames.Add(42, "Gun Safe");
                tempNames.Add(43, "Lockers");
                tempNames.Add(44, "File Cabinet");
                tempNames.Add(45, "Cooler");
                tempNames.Add(48, "Purse");
                tempNames.Add(49, "Bookcase");
                tempNames.Add(50, "Shotgun Messiah Crate");
                tempNames.Add(51, "Pill Case");
                tempNames.Add(52, "Beer Cooler");
                tempNames.Add(53, "Working Stiffs Crate");
                tempNames.Add(54, "Shopping Cart");
                tempNames.Add(55, "Cash Register");
                tempNames.Add(56, "Apache Artifact Chest");
                tempNames.Add(57, "Miner's Chest");
                tempNames.Add(58, "Shopping Basket");
                tempNames.Add(59, "Shamway Crate");
                tempNames.Add(60, "Bookstore Bookcase");
                tempNames.Add(61, "Zombie Feral");
                tempNames.Add(62, "Minibike Storage");
                tempNames.Add(63, "Mountainman Chest");
                tempNames.Add(64, "Gas Pump");
                tempNames.Add(65, "Hazmat Gear");
                tempNames.Add(66, "Zombie Lumberjack");
                tempNames.Add(67, "Toilet");
                tempNames.Add(68, "Dumpster");
            }

            return tempNames;

        }
    }
}
