using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDatalLibrary
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:OrganizationChartTreeView runat=server></{0}:OrganizationChartTreeView>")]
    public class OrganizationChartTreeView : TreeView
    {
        protected override TreeNode CreateNode()
        {
            // Tree node will get its members populated with the data from VIEWSTATE
            return new OrganizationChartTreeNode();
        }
    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:CustomTreeNode runat=server></{0}:CustomTreeNode>")]
    public class OrganizationChartTreeNode : TreeNode
    {
        public string SystemID { get; set; }
        public string Descrizione { get; set; }
        public string NodeData { get; set; }
        public string ElementType { get; set; }
        public string At { get; set; }
        public string Cc { get; set; }
        public int listPosition { get; set; }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] myState = (object[])savedState;
                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                if (myState[1] != null)
                    this.SystemID = (string)myState[1];
                if (myState[2] != null)
                    this.Descrizione = (string)myState[2];
                if (myState[3] != null)
                    this.NodeData = (string)myState[3];
                if (myState[4] != null)
                    this.ElementType = (string)myState[4];
                if (myState[5] != null)
                    this.At = (string)myState[5];
                if (myState[6] != null)
                    this.Cc = (string)myState[6];
                if (myState[7] != null)
                    this.listPosition = (int)myState[7];
            }
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[8];
            allStates[0] = baseState;
            allStates[1] = this.SystemID;
            allStates[2] = this.Descrizione;
            allStates[3] = this.NodeData;
            allStates[4] = this.ElementType;
            allStates[5] = this.At;
            allStates[6] = this.Cc;
            allStates[7] = this.listPosition;

            return allStates;
        }

    }
}