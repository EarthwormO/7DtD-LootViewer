using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace _7DtD_LootViewer
{
    public partial class LootViewer : Form
    {
        public LootViewer()
        {
            InitializeComponent();
        }

        private class lootGroupContents
        {
            //Hold the detailed list of items in the Loot group
            public string item = "";
            public int count = 1;
            public int minCount = 0;
            public int maxCount = 0;
            public decimal prob = 1;
            public decimal tempProbMod = 1;
            public bool minmax = false;
            public bool isGroup = false;
        }
        private class countParse
        {
            //temporarily holds the values that can be parsed from count
            public int count = 1;
            public int minCount = 0;
            public int maxCount = 0;
            public bool incAll = false;
            public bool minmax = false;
        }

        private class lootGroup
        {
            //Hold the pertinent info about any specific Loot group by name
            public int count = 1;
            public int minCount = 0;
            public int maxCount = 0;
            public bool incAll = false;
            public bool minmax = false;
            public decimal lgTotProb = 0;
            public List<lootGroupContents> contents = new List<lootGroupContents>();
        }

        private class lootContainer
        {
            //Hold the pertinent info about any specific Loot Container by ID
            public int count = 1;
            public int minCount = 0;
            public int maxCount = 0;
            public bool incAll = false;
            public bool minmax = false;
            public string size = "";
            public List<lootGroupContents> contents = new List<lootGroupContents>();
        }


        private Dictionary<string, lootGroup> _lootGroups = new Dictionary<string, lootGroup>();
        private Dictionary<int, lootContainer> _lootContainer = new Dictionary<int, lootContainer>();
        public Dictionary<int, string> _containerNames = new Dictionary<int, string>();
        public List<string> _itemNames = new List<string>();

        private bool parseCount(string count, countParse countValues)
        {
            //Parse all the possible Count values of anything.
            if (count == "all")
            {
                //All Items listed in the lootgroup are provided
                countValues.incAll = true;
                return true;
            }
            else if (count.IndexOf(',') >= 0)
            {
                //Count is a Min and Max
                //Get Min number first
                int minValue;
                if (Int32.TryParse(count.Substring(0, count.IndexOf(',')), out minValue))
                {
                    countValues.minmax = true;
                    countValues.minCount = minValue;
                }
                else
                {
                    //Failed to parse Count to number
                    return false;
                }

                //Get Max number
                int maxValue;
                if (Int32.TryParse(count.Substring(count.IndexOf(',') + 1, count.Length - (count.IndexOf(',') + 1)), out maxValue))
                {
                    countValues.minmax = true;
                    countValues.maxCount = maxValue;
                }
                else
                {
                    //Failed to parse Count to number
                    return false;
                }
                return true;
            }
            else
            {
                //Count is a single value, attempt to convert the string to an int and store
                int value;
                if (Int32.TryParse(count, out value))
                {
                    countValues.count = value;
                }
                else
                {
                    //Failed to parse Count to number
                    return false;
                }
                return true;
            }
        }

        //private decimal getTotalProbabilty(string name)
        //{
        //    //Find the total of all Probabilty at the level of the Group = name
        //    decimal tempProb=0;
        //    if(_lootGroups[name].contents.Count < 1)
        //    {
        //        //Special programming for the "Empty" group
        //        return 1;
        //    }
        //    foreach (lootGroupContents lGC in _lootGroups[name].contents)
        //    {
        //        tempProb += lGC.prob;
        //    }
        //    return tempProb;
        //}

        private List<lootGroupContents> explodeGroup(ref string groupName, ref lootGroupContents incGroupDetails)
        {
            //Parse through incoming Group and get all Items/groups in that group into a single List with Probability Modifier
            //i.e. a group with Probability .2 with 4 Items in it, has a 25% chance to pick any of the 4 items, but each item has a real probabilty of 5%
            //incGroupDetails should have Counts and Probabilities coming in.  Can't equate full Probability yet, as we won't have full count of items in Container yet

            List<lootGroupContents> tempLGClist = new List<lootGroupContents>();

            //int groupCount = _lootGroups[groupName].contents.Count();
            decimal groupTotProb = _lootGroups[groupName].lgTotProb;
            List<lootGroupContents> containerContents = new List<lootGroupContents>();

            foreach (lootGroupContents lGC in _lootGroups[groupName].contents)
            {
                //Loop through each content, if it is an Item equate tempPropMod (probability modifier) and add to the outExploded List
                //If it is a Group, call explodeGroup recursively, and add its results to outExploded List
                if (lGC.isGroup)
                {
                    //Group, call explodeGroup again
                    lootGroupContents tempLGC = new lootGroupContents();
                    tempLGC.item = lGC.item;
                    tempLGC.count = lGC.count;
                    tempLGC.minCount = lGC.minCount;
                    tempLGC.maxCount = lGC.maxCount;
                    tempLGC.prob = lGC.prob;
                    List<lootGroupContents> subLGClist = new List<lootGroupContents>();
                    subLGClist = explodeGroup(ref tempLGC.item, ref tempLGC);

                    //Take the returned sub list and equate the corrected ProbMod for items in the group.
                    decimal curProbMod = lGC.prob / groupTotProb;
                    foreach (lootGroupContents sLGC in subLGClist)
                    {
                        sLGC.tempProbMod = curProbMod * sLGC.tempProbMod;
                    }

                    containerContents.AddRange(subLGClist);
                }
                else
                {
                    //Item, store ProbMod
                    
                    //Get Count of items in this group to help equate Prob
                    
                
                    lootGroupContents tempLGC = new lootGroupContents();
                    tempLGC.item = lGC.item;
                    tempLGC.count = lGC.count;
                    tempLGC.minCount = lGC.minCount;
                    tempLGC.maxCount = lGC.maxCount;
                    tempLGC.prob = lGC.prob;
                    if(groupTotProb!=0)
                    {
                        tempLGC.tempProbMod = lGC.prob / groupTotProb;
                    }
                    else
                    {
                        tempLGC.tempProbMod = 1;
                    }
            
                    //Since this is coming from an ExplodeGroup, the Item is from a group, set IsGroup to True
                    tempLGC.isGroup = true;

                    //Add current item to the final Container List
                    containerContents.Add(tempLGC);
                }

            }
            return containerContents;
        }

        private void B_Load_Click(object sender, EventArgs e)
        {
            _containerNames = lootContainernames.getNames();

            XmlDocument doc = new XmlDocument();
            doc.Load(T_XMLFile.Text);

            XmlNodeList itemRefList = doc.GetElementsByTagName("item");

            //manually add the "Empty" Loot Group since it is missed in the Search for "item"
            lootGroup emptyLG = new lootGroup();
            lootGroupContents emptyLGContents = new lootGroupContents();
            emptyLGContents.count = 1;
            emptyLGContents.item = "nothing";
            emptyLGContents.prob = 1;
            emptyLG.count = 1;
            emptyLG.contents.Add(emptyLGContents);
            _lootGroups.Add("empty", emptyLG);

            foreach (XmlElement lg in itemRefList)
            {
                //Parse through each item value in the loot.xml, Since Item will be Name or Group.
                //Get the value of the parent element to get the Lootgroup name or loot container id
                //For each loot group, parse out each and every item, so in the end we have a Loot container with all possible items, no groups listed

                //First, check the Parent Element
                string parentNode = lg.ParentNode.Name;
                if(parentNode=="lootgroup")
                {
                    //Lootgroup, process as a lootgroup
                    string lootgroupName = lg.ParentNode.Attributes["name"].Value;
                    if(!_lootGroups.ContainsKey(lootgroupName))
                    {
                        //Lootgroup doesn't exist, add it
                        M_Output.AppendText("This is a Loot Group: " + lootgroupName + Environment.NewLine);

                        lootGroup tempGroup = new lootGroup();

                        //Parse the Count Value of the lootgroup
                        string lgcount;
                        try
                        {
                            lgcount = lg.ParentNode.Attributes["count"].Value;
                        }
                        catch
                        {
                            //Null Ref, no Count value for this Parent node specified, only 1 of the items is selected, assume count = 1
                            lgcount = "1";
                        }
                        countParse tempValueLG = new countParse();
                        if(parseCount(lgcount, tempValueLG))
                        {
                            tempGroup.count = tempValueLG.count;
                            tempGroup.minCount = tempValueLG.minCount;
                            tempGroup.maxCount = tempValueLG.maxCount;
                            tempGroup.minmax = tempValueLG.minmax;
                            tempGroup.incAll = tempValueLG.incAll;
                        }
                        else
                        {
                            //Failed to parse the Count properly
                            M_Output.AppendText("Failed to parse Count=" + lgcount + "Into its values properly for lootgroup(" + lootgroupName + Environment.NewLine);
                        }
                        _lootGroups.Add(lootgroupName, tempGroup);
                    }
                    //lootgroup already exists/now exists, check to see if this is a group or item

                    //Element is an Item, add it to the LG
                    //Parse the Count Parameter if there is one:
                    lootGroupContents tempLGContents = new lootGroupContents();
                    countParse tempValue = new countParse();
                    string count = lg.GetAttribute("count");
                    string prob = lg.GetAttribute("prob");

                    if (parseCount(count, tempValue))//Parse Count into its fields, if it fails, assume Count=1 by leaving it default.
                    {
                        tempLGContents.count = tempValue.count;
                        tempLGContents.minCount = tempValue.minCount;
                        tempLGContents.maxCount = tempValue.maxCount;
                        tempLGContents.minmax = tempValue.minmax;
                    }

                    //Try to parse Prob to a Decimal, if it fails assume it is a 1 by leaving it default.
                    decimal decValue;
                    if (Decimal.TryParse(prob, out decValue))
                    {
                        tempLGContents.prob = decValue;
                    }

                    string name="";
                    if (lg.GetAttribute("name") != "")
                    {
                        //Element is an Item, use Item Name
                        name = lg.GetAttribute("name");
                        tempLGContents.item = name;
                        tempLGContents.isGroup = false;

                    }
                    else if (lg.GetAttribute("group") != "")
                    {
                        //Element is a Group, Add it to the Contents of the current lootgroup
                        //Check to see if this group already exists in the Dictionary, if not error
                        name = lg.GetAttribute("group");
                        tempLGContents.item = name;
                        tempLGContents.isGroup = true;
                    }
                    else
                    {
                        //Element is unknown
                        M_Output.AppendText("Unknown Element" + Environment.NewLine);
                    }

                    _lootGroups[lootgroupName].contents.Add(tempLGContents);
                    _lootGroups[lootgroupName].lgTotProb += tempLGContents.prob;
                    M_Output.AppendText(name + ", " + count + ", " + prob + Environment.NewLine);



                }
                else if(parentNode=="lootcontainer")
                {
                    //Loot Container, parse loot items out and figure Probabilities
                    //First parse all parts of the Loot Container itself

                    //Check to see if the Container has already been saved
                    int containerID = Int32.Parse(lg.ParentNode.Attributes["id"].Value);
                    if (!_lootContainer.ContainsKey(containerID))
                    {
                        //LootContainer doesn't exist, add it
                        M_Output.AppendText("This is a Loot Container: " + containerID + Environment.NewLine);

                        lootContainer tempContainer = new lootContainer();

                        //Parse the Count Value of the lootgroup
                        tempContainer.size = lg.ParentNode.Attributes["size"].Value; ;

                        countParse tempValueLC = new countParse();
                        string countLC = lg.ParentNode.Attributes["count"].Value;

                        if (parseCount(countLC, tempValueLC))//Parse Count into its fields, if it fails, assume Count=1 by leaving it default.
                        {
                            tempContainer.count = tempValueLC.count;
                            tempContainer.minCount = tempValueLC.minCount;
                            tempContainer.maxCount = tempValueLC.maxCount;
                            tempContainer.minmax = tempValueLC.minmax;
                        }
                        else
                        {
                            //Failed to parse the Count properly
                            M_Output.AppendText("Failed to parse Count=" + countLC + "Into its values properly for lootContainer: " + containerID + Environment.NewLine);
                        }
                        _lootContainer.Add(containerID, tempContainer);
                    }

                    //Container exists/now exists, parse this Item/Group into the List

                    //Parse Count value if there is one
                    lootGroupContents tempLGContents = new lootGroupContents();
                    countParse tempValue = new countParse();
                    string count = lg.GetAttribute("count");
                    string prob = lg.GetAttribute("prob");

                    if (parseCount(count, tempValue))//Parse Count into its fields, if it fails, assume Count=1 by leaving it default.
                    {
                        tempLGContents.count = tempValue.count;
                        tempLGContents.minCount = tempValue.minCount;
                        tempLGContents.maxCount = tempValue.maxCount;
                        tempLGContents.minmax = tempValue.minmax;
                    }
                    //Try to parse Prob to a Decimal, if it fails assume it is a 1 by leaving it default.
                    decimal decValue;
                    if (Decimal.TryParse(prob, out decValue))
                    {
                        tempLGContents.prob = decValue;
                    }

                    //Determine if this is an Item, or a Group
                    string name = "";
                    if (lg.GetAttribute("name") != "")
                    {
                        //Element is an Item, use Item Name
                        name = lg.GetAttribute("name");
                        tempLGContents.item = name;
                        tempLGContents.isGroup = false;
                        _lootContainer[containerID].contents.Add(tempLGContents);

                    }
                    else if (lg.GetAttribute("group") != "")
                    {
                        //Element is a Group, For Containers, we want to parse the Group out to individual Items.
                        name = lg.GetAttribute("group");

                        //Parse through the Group for its Items
                        List<lootGroupContents> explodedContents = new List<lootGroupContents>();
                        explodedContents = explodeGroup(ref name, ref tempLGContents);
                        _lootContainer[containerID].contents.AddRange(explodedContents);
                        
                    }
                    else
                    {
                        //Element is unknown
                        M_Output.AppendText("Unknown Element" + Environment.NewLine);
                    }


                }


            }

            M_Output.Clear();
            //Equate Container's Total Probability:
            foreach (KeyValuePair<int,lootContainer> lc in _lootContainer)
            {
                M_Output.AppendText("Loot Container: " + lc.Key + Environment.NewLine);
                M_Output.AppendText(Environment.NewLine);

                decimal tempProb = 0;
                foreach (lootGroupContents lGC in lc.Value.contents)
                {
                    tempProb += lGC.tempProbMod;
                }

                decimal containerTotProb = tempProb;

                List <lootGroupContents> SortedList = lc.Value.contents.OrderBy(o => o.item).ToList();
                foreach (lootGroupContents item in SortedList)
                {
                    item.prob = item.tempProbMod / containerTotProb;
                    item.tempProbMod = 0;
                    //M_Output.AppendText("     " + item.item + ", " + Math.Round(item.tempProbMod,3) + ", " + Math.Round(item.tempProbMod/containerTotProb, 3) + Environment.NewLine);

                //Add to the list of Item Names if not already in it.
                if(!_itemNames.Contains(item.item))
                    {
                        _itemNames.Add(item.item);
                    }
                }
            }
            V_LContainer.DataSource = new BindingSource(_lootContainer, null);
            V_LContainer.DisplayMember = "Key";
            V_LContainer.ValueMember = "Key";
            V_LContainer.Refresh();
            V_LContainer.Enabled = true;

            _itemNames.Sort();

        }


        private void V_LContainer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Display list of Loot Container Contents when selected
            M_Output.Clear();
            if(r_Item.Checked)
            {

                foreach (KeyValuePair<int, lootContainer> lc in _lootContainer)
                {
                    List<lootGroupContents> tempLG = lc.Value.contents.FindAll(x => x.item == (string)V_LContainer.SelectedValue);
                    if(tempLG.Count > 0)
                    {
                        foreach(lootGroupContents item in tempLG)
                        {
                            M_Output.AppendText("Container: " + lc.Key + ", Prob in container: " + Math.Round(item.prob * 100, 2) + '%' + Environment.NewLine);
                        }
                    }
                }

            }
            else
            {
                List<lootGroupContents> SortedList = _lootContainer[(int)V_LContainer.SelectedValue].contents.OrderBy(o => o.item).ToList();
                foreach (lootGroupContents item in SortedList)
                {
                    M_Output.AppendText("     " + item.item + ", " + Math.Round(item.prob * 100, 1) + '%' + Environment.NewLine);
                }
            }

        }

        private void gb_SearchChanged()
        {
            //Search Type Changed
            if(r_Item.Checked)
                {
                //Search by Item Name
                V_LContainer.DataSource = _itemNames;
                V_LContainer.Refresh();
                V_LContainer.Enabled = true;
            }
            if (r_LC.Checked)
                {
                    //Search by Loot Container ID
                    V_LContainer.DataSource = new BindingSource(_lootContainer, null);
                    V_LContainer.DisplayMember = "Key";
                    V_LContainer.ValueMember = "Key";
                    V_LContainer.Refresh();
                    V_LContainer.Enabled = true;
                }
        }

        private void r_LC_CheckedChanged(object sender, EventArgs e)
        {
            gb_SearchChanged();
        }

        private void r_Item_CheckedChanged(object sender, EventArgs e)
        {
            gb_SearchChanged();
        }
    }
}
