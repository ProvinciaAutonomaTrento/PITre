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
    [ToolboxData("<{0}:CustomTreeView runat=server></{0}:CustomTreeView>")]
    public class CustomTreeView : TreeView
    {
        protected override TreeNode CreateNode()
        {
            // Tree node will get its members populated with the data from VIEWSTATE
            return new myTreeNode();
        }
    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:CustomTreeNode runat=server></{0}:CustomTreeNode>")]
    public class myTreeNode : TreeNode
    {

        public string ID { get; set; }
        public string IDRECORD { get; set; }
        public string CODICE { get; set; }
        public string DESCRIZIONE { get; set; }
        public string NUMMESICONSERVAZIONE { get; set; }
        public string REGISTRO { get; set; }
        public string LIVELLO { get; set; }
        public string PARENT { get; set; }
        public string CODLIV { get; set; }
        public string TIPO { get; set; }


        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] myState = (object[])savedState;
                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                if (myState[1] != null)
                    this.ID = (string)myState[1];
                if (myState[2] != null)
                    this.IDRECORD = (string)myState[2];
                if (myState[3] != null)
                    this.CODICE = (string)myState[3];
                if (myState[4] != null)
                    this.DESCRIZIONE = (string)myState[4];
                if (myState[5] != null)
                    this.NUMMESICONSERVAZIONE = (string)myState[5];
                if (myState[6] != null)
                    this.REGISTRO = (string)myState[6];
                if (myState[7] != null)
                    this.LIVELLO = (string)myState[7];
                if (myState[8] != null)
                    this.PARENT = (string)myState[8];
                if (myState[9] != null)
                    this.CODLIV = (string)myState[9];
                if (myState[10] != null)
                    this.TIPO = (string)myState[10];

            }
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[11];
            allStates[0] = baseState;
            allStates[1] = this.ID;
            allStates[2] = this.IDRECORD;
            allStates[3] = this.CODICE;
            allStates[4] = this.DESCRIZIONE;
            allStates[5] = this.NUMMESICONSERVAZIONE;
            allStates[6] = this.REGISTRO;
            allStates[7] = this.LIVELLO;
            allStates[8] = this.PARENT;
            allStates[9] = this.CODLIV;
            allStates[10] = this.TIPO;

            return allStates;
        }
    }
}