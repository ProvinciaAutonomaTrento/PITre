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
    [ToolboxData("<{0}:OrganizationChartTreeView2 runat=server></{0}:OrganizationChartTreeView2>")]
    public class OrganizationChartTreeView2 : TreeView
    {
        protected override TreeNode CreateNode()
        {
            // Tree node will get its members populated with the data from VIEWSTATE
            return new myTreeNode2();
        }
    }

    [DefaultProperty("Text")]
    [ToolboxData("<{0}:CustomTreeNode2 runat=server></{0}:CustomTreeNode2>")]
    public class myTreeNode2 : System.Web.UI.WebControls.TreeNode
    {

        public string TipoNodo { get; set; } // Tipo Nodo [Possibili Valori: U=(Unità organizz.), R=(Ruolo), U=(Utente) ]
        public string RuoliUO { get; set; }
        public string SottoUO { get; set; }
        public string Livello { get; set; }
        public string ID { get; set; }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] myState = (object[])savedState;
                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                if (myState[1] != null)
                    this.TipoNodo = (string)myState[1];
                if (myState[2] != null)
                    this.RuoliUO = (string)myState[2];
                if (myState[3] != null)
                    this.SottoUO = (string)myState[3];
                if (myState[4] != null)
                    this.Livello = (string)myState[4];
                if (myState[5] != null)
                    this.ID = (string)myState[5];
            }
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[6];
            allStates[0] = baseState;
            allStates[1] = this.TipoNodo;
            allStates[2] = this.RuoliUO;
            allStates[3] = this.SottoUO;
            allStates[4] = this.Livello;
            allStates[5] = this.ID;

            return allStates;
        }

    }

}