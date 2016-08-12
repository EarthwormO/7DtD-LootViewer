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
            public int useCount = 0;
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
            public string name = "";
            public List<lootGroupContents> contents = new List<lootGroupContents>();
        }


        private Dictionary<string, lootGroup> _lootGroups = new Dictionary<string, lootGroup>();
        private Dictionary<int, lootContainer> _lootContainer = new Dictionary<int, lootContainer>();
        public Dictionary<int, string> _containerNames = new Dictionary<int, string>();
        public Dictionary<string, Dictionary<int, Decimal>> _LootProbTmpl = new Dictionary<string, Dictionary<int, Decimal>>();
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

        private Dictionary<int,string> getContainerBinding(Dictionary<int, lootContainer> tempcontainer)
        {
            //Take the Loot container Object and return just a List of IDs and Names
            Dictionary<int, string> tempDict = new Dictionary<int, string>();
            foreach(KeyValuePair<int, lootContainer> lc in tempcontainer)
            {
                tempDict.Add(lc.Key, lc.Key + ":" + lc.Value.name);
            }
            return tempDict;
        }


        private List<lootGroupContents> explodeGroup(ref string groupName, ref lootGroupContents incGroupDetails)
        {
            //Parse through incoming Group and get all Items/groups in that group into a single List with Probability Modifier
            //i.e. a group with Probability .2 with 4 Items in it, has a 25% chance to pick any of the 4 items, but each item has a real probabilty of 5%
            //incGroupDetails should have Counts and Probabilities coming in.  Can't equate full Probability yet, as we won't have full count of items in Container yet

            List<lootGroupContents> tempLGClist = new List<lootGroupContents>();

            //int groupCount = _lootGroups[groupName].contents.Count();
            decimal groupTotProb = _lootGroups[groupName].lgTotProb;
            List<lootGroupContents> containerContents = new List<lootGroupContents>();

            //Increment Count of Group Use, allows removal of unused Groups
            _lootGroups[groupName].useCount++;

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
                    //tempLGC.prob = lGC.prob;
                    tempLGC.prob = lGC.prob * incGroupDetails.prob;
                    List<lootGroupContents> subLGClist = new List<lootGroupContents>();
                    if (incGroupDetails.prob == 30)
                    {
                        M_Output.AppendText("");
                    }

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
                    tempLGC.tempProbMod = tempLGC.tempProbMod * incGroupDetails.prob;

                    //Since this is coming from an ExplodeGroup, the Item is from a group, set IsGroup to True
                    tempLGC.isGroup = true;

                    //Add current item to the final Container List
                    containerContents.Add(tempLGC);
                }

            }
            return containerContents;
        }

        private void readProbTemplates(XmlNodeList ProbTmpl)
        {
            //Parse through the Loot XML pased in and load in Probability Templates
            //Dictionary <String nameofTempl, List<int> MinLevel>
            foreach (XmlElement pt in ProbTmpl)
            {
                //Check to ensure it is loading a ProbTemplate not a Loot Quality Template
                string parentNode = pt.ParentNode.Name;
                if (parentNode == "lootprobtemplate")
                {
                    string ProbTmplName = pt.ParentNode.Attributes["name"].Value;
                    //Check to see if LootProbTmpl already exists
                    if (!_LootProbTmpl.ContainsKey(ProbTmplName))
                    {
                        //Probability Template does not exist, create it
                        Dictionary<int, Decimal> tempList = new Dictionary<int, Decimal>();
                        _LootProbTmpl.Add(ProbTmplName,tempList);
                    }
                    //Dictionary ProbTmpl now exists, parse the line being worked, and add it to the List
                    string level = pt.GetAttribute("level");
                    string prob = pt.GetAttribute("prob");


                    //Need to parse Level to Decimal (Minimum)
                    decimal decLevel = 1;
                    if (Decimal.TryParse(level.Substring(level.IndexOf(",")+1), out decLevel))
                    {
                        //Have Decimal Level (Which is a percent of Total Level), Equate to actual Level Int
                        int levelInt = (int)Math.Floor(Decimal.Parse(T_MaxSkill.Text) * decLevel);

                        decimal decProb = 1;
                        if (Decimal.TryParse(prob, out decProb))
                        {
                            //Was able to successfully parse Level and Probability, add it to the Template
                            _LootProbTmpl[ProbTmplName].Add(levelInt, decProb);
                        }
                    }
                }


            }
        }

        private void B_Load_Click(object sender, EventArgs e)
        {
            //Read Import file for all currently specified Container Names
            _containerNames = lootContainernames.getNames();

            XmlDocument doc = new XmlDocument();
            doc.Load(T_XMLFile.Text);

            XmlNodeList ProbTmpl = doc.GetElementsByTagName("loot");
            readProbTemplates(ProbTmpl);

            XmlNodeList itemRefList = doc.GetElementsByTagName("item");

            //manually add the "Empty" Loot Group since it is missed in the Search for "item"
            lootGroup emptyLG = new lootGroup();
            lootGroupContents emptyLGContents = new lootGroupContents();
            emptyLGContents.count = 1;
            emptyLGContents.item = "nothing";
            emptyLGContents.prob = 1;
            emptyLG.count = 1;
            emptyLG.lgTotProb = 1;
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
                    string probTmpl = lg.GetAttribute("loot_prob_template");

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
                    else
                    {
                        if(probTmpl != "")
                        {
                            //This is a Probability Template, not a Straight Probability, find the matching Probability Template and pull the Probability form there
                            foreach(KeyValuePair<int,Decimal> kv in _LootProbTmpl[probTmpl])
                            {
                                if(Int32.Parse(T_Player.Text) < kv.Key)
                                {
                                    tempLGContents.prob = kv.Value;
                                    break;
                                }
                            }
                            M_Output.AppendText("");
                        }
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

                        //Attempt to lookup the Name of this Container ID, and add it if it exists
                        if (!_containerNames.TryGetValue(containerID, out tempContainer.name))
                        {
                            //Container failed to get a name, add the Container ID to the _ContainerNames, with no name
                            _containerNames.Add(containerID, "");
                        }

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
                    string probTmpl = lg.GetAttribute("loot_prob_template");

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
                        tempLGContents.tempProbMod  = decValue;
                    }
                    else
                    {
                        if (probTmpl != "")
                        {
                            //This is a Probability Template, not a Straight Probability, find the matching Probability Template and pull the Probability form there
                            foreach (KeyValuePair<int, Decimal> kv in _LootProbTmpl[probTmpl])
                            {
                                if (Int32.Parse(T_Player.Text) < kv.Key)
                                {
                                    tempLGContents.prob = kv.Value;
                                    break;
                                }
                            }
                            M_Output.AppendText("");
                        }
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

            //Save All Found Container Names back to the XML File to allow user to edit it and add names for next run.
            System.Xml.Linq.XElement el = new System.Xml.Linq.XElement("root", _containerNames.Select(kv => new System.Xml.Linq.XElement("ID" + kv.Key.ToString(), kv.Value)));
            string fileName = "containernames.xml";
            el.Save(fileName);


            M_Output.Clear();
            //Equate Container's Total Probability:
            foreach (KeyValuePair<int,lootContainer> lc in _lootContainer)
            {
                M_Output.AppendText("Loot Container: " + lc.Key + Environment.NewLine);
                M_Output.AppendText(Environment.NewLine);

                decimal tempProb = 0;

                //Added to break code for testing specific Containers
                if (lc.Key == 5)
                {
                    M_Output.AppendText("");
                }

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

            V_LContainer.DataSource = new BindingSource(getContainerBinding(_lootContainer), null);
            V_LContainer.DisplayMember = "Value";
            V_LContainer.ValueMember = "Key";
            V_LContainer.Refresh();
            V_LContainer.Enabled = true;

            _itemNames.Sort();

            //Parse through the Containers and add Containers Witohut names to import file.

        }


        private void V_LContainer_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Display list of Loot Container Contents when selected
            M_Output.Clear();
            if(r_Item.Checked)
            {

                //Looking for all containers containing a specific Item
                foreach (KeyValuePair<int, lootContainer> lc in _lootContainer)
                {
                    List<lootGroupContents> tempLG = lc.Value.contents.FindAll(x => x.item == (string)V_LContainer.SelectedValue);
                    if (tempLG.Count > 0)
                    {
                        foreach (lootGroupContents item in tempLG)
                        {
                            M_Output.AppendText(lc.Key + ":" + lc.Value.name + ", Prob in container: " + Math.Round(item.prob * 100, 2) + '%' + Environment.NewLine);
                        }
                    }
                }

            }
            else
            {
                //Looking for all items in a Single container
                List<lootGroupContents> SortedList;
                if (r_Alpha.Checked)
                {
                    SortedList = _lootContainer[(int)V_LContainer.SelectedValue].contents.OrderBy(o => o.item).ToList();
                }
                else
                {
                    SortedList = _lootContainer[(int)V_LContainer.SelectedValue].contents.OrderByDescending(o => o.prob).ToList();
                }

                //List<lootGroupContents> SortedList = _lootContainer[(int)V_LContainer.SelectedValue].contents.OrderBy(o => o.item).ToList();
                foreach (lootGroupContents item in SortedList)
                {
                    //Check for MinMax
                    string tempCount = "Count: ";
                    if (item.minmax) { tempCount += item.minCount.ToString() + "-" + item.maxCount.ToString(); } else { tempCount += item.count.ToString(); }
                    M_Output.AppendText("     " + item.item + ", " + Math.Round(item.prob * 100, 1) + "%  " + tempCount + Environment.NewLine);
                }
                //Update Count Textbox
                if (_lootContainer[(int)V_LContainer.SelectedValue].minmax)
                {
                    //There is a Min and Max, not just a 1
                    T_Count.Text = _lootContainer[(int)V_LContainer.SelectedValue].minCount.ToString() + " - " + _lootContainer[(int)V_LContainer.SelectedValue].maxCount.ToString();
                }
                else
                {
                    T_Count.Text = _lootContainer[(int)V_LContainer.SelectedValue].count.ToString();
                }
                
            }

        }

        private void gb_SearchChanged()
        {
            //Search Type Changed
            if(r_Item.Checked)
                {
                //Search by Item Name
                l_Filter.Text = "Item:";
                V_LContainer.DataSource = _itemNames;
                V_LContainer.Refresh();
                V_LContainer.Enabled = true;
            }
            if (r_LC.Checked)
                {
                //Search by Loot Container ID
                l_Filter.Text = "LootContainer:";
                V_LContainer.DataSource = new BindingSource(getContainerBinding(_lootContainer), null);
                V_LContainer.DisplayMember = "Value";
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

        private void B_ListGroups_Click(object sender, EventArgs e)
        {
            //List all Groups and their Count of uses
            Dictionary<string, int> tempGroupList = new Dictionary<string, int>();
            foreach (KeyValuePair<string,lootGroup> kv in _lootGroups)
            {
                tempGroupList.Add(kv.Key, kv.Value.useCount);
            }
            var sortedList = tempGroupList.ToList();
            sortedList.Sort((k, v) => k.Value.CompareTo(v.Value));

            M_Output.Clear();
            M_Output.AppendText("Loot Groups defined in loot.xml" + Environment.NewLine);
            foreach (KeyValuePair<string, int> kv in sortedList)
            {
                M_Output.AppendText("  " + kv.Key + ": " + kv.Value + Environment.NewLine);
            }

        }

    }
}
