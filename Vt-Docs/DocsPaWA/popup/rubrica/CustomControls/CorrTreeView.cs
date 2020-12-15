using System;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Drawing;
using Microsoft.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.popup.RubricaDocsPA.CustomControls;

namespace DocsPAWA.popup.RubricaDocsPA
{
    /// <summary>
    /// Summary description for CorrTreeView.
    /// </summary>
    public class CorrTreeView : TreeView, IPostBackDataHandler
    {
        private TreeViewSelectorType _selector_type;
        private DictionaryEntry event_data = new DictionaryEntry();
        private int _item_height = 0;
        private string _selected_cssclass = null;
        private string _codUoAppartenenza;
        private bool _selOrg;
        private DocsPAWA.DocsPaWR.InfoUtente _infoUtente;

        internal const string EVT_EXPAND = "+";
        internal const string EVT_COLLAPSE = "-";
        internal const string EVT_SELECTORFILTER = "_SelectorFilter";

        public new event ClickEventHandler Expand;
        public new event ClickEventHandler Collapse;
        int posizioneGlobale;

        /// <summary>
        /// Viene generato quando l'utente chiede la rimozione di tutti gli
        /// elementi presenti nella CorrDataGrid
        /// </summary>
        public event SelectorFilterHandler SelectorFilter;

        public TreeViewSelectorType SelectorType
        {
            get { return _selector_type; }
            set { _selector_type = value; }
        }

        public int ItemHeight
        {
            get { return _item_height; }
            set { _item_height = value; }
        }

        public string SelectedCssClass
        {
            get { return _selected_cssclass; }
            set { _selected_cssclass = value; }
        }

        internal bool IsSelectable(string tipo, string cod)
        {
            if (SelectorFilter != null)
                return SelectorFilter(this, new SelectorFilterArgs(tipo, cod));
            else
                return true;
        }

        public string CodUoAppartenenza
        {
            get { return _codUoAppartenenza; }
            set { _codUoAppartenenza = value; }
        }

        public DocsPAWA.DocsPaWR.InfoUtente InfoUtente
        {
            set { _infoUtente = value; }
            get { return _infoUtente; }
        }

        public bool SelectedOrganigramma
        {
            set { _selOrg = value; }
            get { return _selOrg; }
        }

        public CorrTreeView()
        {
            this.DefaultStyle.Add("font-family", "Verdana");
            this.DefaultStyle.Add("font-size", "8pt");
            this.BackColor = Color.White;
            this.SelectorType = TreeViewSelectorType.CheckBox;
            this.AutoPostBack = false;
            this.SelectExpands = false;
            this.Width = new System.Web.UI.WebControls.Unit("100%");
            this.Height = new System.Web.UI.WebControls.Unit("100%");
            this.BorderStyle = System.Web.UI.WebControls.BorderStyle.Inset;
            this.BorderWidth = new System.Web.UI.WebControls.Unit("2px");
            this.ID = this.ClientID;
            this.ItemHeight = 16;
        }

        protected override void Render(HtmlTextWriter output)
        {
            string theStyle = String.Format("width:{0};height:{1};background-color:{2};overflow:scroll;border-style:{3};border-width:{4}",
                this.Width.ToString(), this.Height.ToString(),
                String.Format("#{0:x2}{0:x2}{0:x2}",
                this.BackColor.R,
                this.BackColor.G,
                this.BackColor.B),
                this.BorderStyle.ToString(),
                this.BorderWidth.ToString());

            //StreamWriter sw = new StreamWriter (@"c:\test.html");
            //output = new HtmlTextWriter(sw, "\t");

            output.WriteBeginTag("div");
            output.WriteAttribute("style", theStyle);
            output.WriteAttribute("id", this.ClientID);
            output.Write(HtmlTextWriter.TagRightChar);


            checkUtente(this.Nodes, output);

            output.WriteEndTag("div");
            //sw.Close();
        }

        private void checkUtente(TreeNodeCollection nodes, HtmlTextWriter output)
        {
            //Cerco se è stato selezionato l'organigramma per un utente
            //e se l'utente è associato a più ruoli
            string posizioni = "";
            posizioni = contaRuoli(posizioni, nodes, 0);
            if (!string.IsNullOrEmpty(posizioni))
            {
                posizioni = posizioni.Substring(0, posizioni.Length - 1);
                string[] posUtenti = posizioni.Split('@');
                if (posUtenti.Length > 1)
                {
                    if (!SelectedOrganigramma)
                    {
                        do_nodes_PrimoRuolo(nodes, output, 0, Convert.ToInt32(posUtenti[posUtenti.Length - 1]));
                    }
                    else
                    {
                        do_nodes_PrimoRuolo(nodes, output, 0, Convert.ToInt32(posUtenti[0]));
                    }
                    for (int i = 1; i < posUtenti.Length; i++)
                    {
                        output.Write("<br>");
                        do_nodes_RuoliPerUtente(nodes, output, Convert.ToInt32(posUtenti[i - 1]) + 1, Convert.ToInt32(posUtenti[i]), 0);
                    }
                }
                else
                    do_nodes(nodes, output);
            }
            else
                do_nodes(nodes, output);
        }

        private string contaRuoli(string posizioni, TreeNodeCollection nodes, int i)
        {
            if (!SelectedOrganigramma)
            {
                string tipo = string.Empty;
                posizioneGlobale = i;
                foreach (TreeNode nd in nodes)
                {
                    if (nd != null && !nd.NodeData.ToUpper().Equals("__DUMMY_NODE__".ToUpper()))
                    {
                        tipo = nd.ImageUrl.Substring(21, 1);
                        if (tipo == "p")
                        {
                            posizioni = posizioni + i + "@";
                        }
                        posizioneGlobale++;
                        if (nd.Nodes.Count > 0)
                        {
                            i++;
                            posizioni = contaRuoli(posizioni, nd.Nodes, posizioneGlobale);
                        }
                    }
                }
            }
            else
            {
                string tipo = string.Empty;
                foreach (TreeNode nd in nodes)
                {
                    if (nd != null && !nd.NodeData.ToUpper().Equals("__DUMMY_NODE__".ToUpper()))
                    {
                        tipo = nd.ImageUrl.Substring(21, 1);
                        if (tipo == "p")
                        {
                            posizioni = posizioni + i + "@";
                        }
                        if (nd.Nodes.Count > 0)
                        {
                            i++;
                            posizioni = contaRuoli(posizioni, nd.Nodes, i);
                        }
                    }
                }
            }

            return posizioni;

        }




        private void do_nodes_PrimoRuolo(TreeNodeCollection nodes, HtmlTextWriter output, int conta, int posUtente)
        {
            int i = conta;
            if (i <= posUtente)
            {
                output.WriteBeginTag("div");
                string display = "block";

                if (nodes.Parent is TreeNode)
                    display = ((TreeNode)nodes.Parent).Expanded ? "block" : "none";

                output.WriteAttribute("style", "left:16px;position:static;display:" + display);
                output.Write(HtmlTextWriter.TagRightChar);

                output.WriteBeginTag("table");
                output.WriteAttribute("border", "0");
                output.Write(HtmlTextWriter.TagRightChar);
                foreach (TreeNode nd in nodes)
                {
                    do_node(nd, output);
                    i = i + 1;
                    if (nd.Nodes.Count > 0)
                        do_nodes_PrimoRuolo(nd.Nodes, output, i, posUtente);

                    output.WriteEndTag("td");
                    output.WriteEndTag("tr");
                }

                output.WriteEndTag("table");
                output.WriteEndTag("div");
            }
        }

        private void do_node(TreeNode nd, HtmlTextWriter output)
        {
            output.WriteBeginTag("tr");
            output.WriteAttribute("height", ItemHeight.ToString());
            output.Write(HtmlTextWriter.TagRightChar);

            output.WriteBeginTag("td");
            if (nd.GetNodeIndex() == this.SelectedNodeIndex)
            {
                if (SelectedStyle.CssText != null && SelectedStyle.CssText != "")
                    output.WriteAttribute("style", this.SelectedStyle.CssText);
                if (SelectedCssClass != null && SelectedCssClass != "")
                    output.WriteAttribute("class", this.SelectedCssClass);
            }
            else
            {
                if (DefaultStyle.CssText != null && DefaultStyle.CssText != "")
                    output.WriteAttribute("style", this.DefaultStyle.CssText);
                if (CssClass != null && CssClass != "")
                    output.WriteAttribute("class", this.CssClass);
            }

            output.WriteAttribute("data", nd.NodeData);
            output.Write(HtmlTextWriter.TagRightChar);

            DocsPaWR.ElementoRubrica er = UserManager.getElementoRubrica(this.Page, nd.NodeData);

            if (((SelectorFilter == null)) ||
                        SelectorFilter(this, new SelectorFilterArgs("I", nd.NodeData)))
            {
                if ((nd.NodeData != "__DUMMY_NODE__" && (((NodoRubrica)nd).SelectAllowed)))
                {
                    if (this.SelectorType == TreeViewSelectorType.CheckBox)
                    {
                        output.WriteBeginTag("input");
                        output.WriteAttribute("type", "checkbox");
                        output.WriteAttribute("id", nd.ID);
                        output.WriteAttribute("name", "__CBX_" + this.ID);
                        if (er != null && er.disabledTrasm)
                            output.WriteAttribute("disabled", "");
                        output.WriteAttribute("value", nd.ID);
                        output.Write(HtmlTextWriter.TagRightChar);
                    }
                    else
                        if (this.SelectorType == TreeViewSelectorType.RadioButton)
                        {
                            output.WriteBeginTag("input");
                            output.WriteAttribute("type", "radio");
                            output.WriteAttribute("id", "_CBX_[" + nd.ID + "]_CBX_");
                            output.WriteAttribute("name", "_CBX__CBX_");
                            output.WriteAttribute("value", nd.ID);
                            output.Write(HtmlTextWriter.TagRightChar);
                        }
                }
                else
                {
                    output.WriteBeginTag("div");
                    output.WriteAttribute("style", "display:inline;width:20px");
                    output.Write(HtmlTextWriter.TagRightChar);
                    output.WriteEndTag("div");
                }
            }

            else
            {
                //Questo controllo viene effettuato in quanto se è atttiva la chiave "RUBRICA_PROTO_USA_SMISTAMENTO", 
                //una eventuale ricerca in rubrica delle UO-Ruoli-Utenti che appartengono a UO sottoposte quella 
                //dell'utente loggato, devono essere comunque selezionabili - PER ANAS
                DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                if ((nd.NodeData != "__DUMMY_NODE__" && ((NodoRubrica)nd).SelectAllowed))
                {
                    if ((nd.NodeData != "__DUMMY_NODE__" && wws.verificaDipendezaCodRubrica(this._codUoAppartenenza, nd.NodeData, this._infoUtente)))
                    {
                        if (this.SelectorType == TreeViewSelectorType.CheckBox)
                        {
                            output.WriteBeginTag("input");
                            output.WriteAttribute("type", "checkbox");
                            output.WriteAttribute("id", nd.ID);
                            output.WriteAttribute("name", "__CBX_" + this.ID);
                            if (er != null && er.disabledTrasm)
                                output.WriteAttribute("disabled","");
                            output.WriteAttribute("value", nd.ID);
                            output.Write(HtmlTextWriter.TagRightChar);
                        }
                        else
                            if (this.SelectorType == TreeViewSelectorType.RadioButton)
                            {
                                output.WriteBeginTag("input");
                                output.WriteAttribute("type", "radio");
                                output.WriteAttribute("id", "_CBX_[" + nd.ID + "]_CBX_");
                                output.WriteAttribute("name", "_CBX__CBX_");
                                output.WriteAttribute("value", nd.ID);
                                output.Write(HtmlTextWriter.TagRightChar);
                            }
                    }
                    else
                    {
                        output.WriteBeginTag("div");
                        output.WriteAttribute("style", "display:inline;width:20px");
                        output.Write(HtmlTextWriter.TagRightChar);
                        output.WriteEndTag("div");
                    }
                }
            }
            if ((nd.Expandable == ExpandableValue.Always || nd.Expandable == ExpandableValue.CheckOnce) || (nd.Expandable == ExpandableValue.Auto && nd.Nodes.Count > 0))
                wrapPostbackLink(output, nd);
            else
            {
                if (nd.ImageUrl != null && nd.ImageUrl != "")
                {
                    output.WriteBeginTag("img");
                    output.WriteAttribute("src", nd.ImageUrl);
                    output.Write(HtmlTextWriter.TagRightChar);
                }
            }

            if(er != null && er.disabledTrasm)
                output.Write("<font color=\"red\">" + HttpUtility.HtmlEncode(nd.Text) + "</font>");
            else
                output.Write(HttpUtility.HtmlEncode(nd.Text));
        }

        private void do_nodes_RuoliPerUtente(TreeNodeCollection nodes, HtmlTextWriter output, int conta, int fine, int inizio)
        {
            foreach (TreeNode nodo in nodes)
            {
                int i = inizio;
                if (i < conta)
                {
                    if (nodo.Nodes.Count > 0)
                    {
                        i = i + 1;
                        do_nodes_RuoliPerUtente(nodo.Nodes, output, conta, fine, i);
                    }
                }
                else
                {
                    int n = conta;
                    if (n <= fine)
                    {
                        output.WriteBeginTag("div");
                        string display = "block";

                        if (nodes.Parent is TreeNode)
                            display = ((TreeNode)nodes.Parent).Expanded ? "block" : "none";

                        output.WriteAttribute("style", "left:16px;position:static;display:" + display);
                        output.Write(HtmlTextWriter.TagRightChar);

                        output.WriteBeginTag("table");
                        output.WriteAttribute("border", "0");
                        output.Write(HtmlTextWriter.TagRightChar);
                        foreach (TreeNode nd in nodes)
                        {
                            do_node(nd, output);

                            if (nd.Nodes.Count > 0)
                            {
                                n = n + 1;
                                do_nodes_RuoliPerUtente(nd.Nodes, output, n, fine, fine);
                            }
                            output.WriteEndTag("td");
                            output.WriteEndTag("tr");
                        }

                        output.WriteEndTag("table");
                        output.WriteEndTag("div");
                    }
                }
            }
        }

        private void do_nodes(TreeNodeCollection nodes, HtmlTextWriter output)
        {
            output.WriteBeginTag("div");
            string display = "block";

            if (nodes.Parent is TreeNode)
                display = ((TreeNode)nodes.Parent).Expanded ? "block" : "none";

            output.WriteAttribute("style", "left:16px;position:static;display:" + display);
            output.Write(HtmlTextWriter.TagRightChar);

            output.WriteBeginTag("table");
            output.WriteAttribute("border", "0");
            output.Write(HtmlTextWriter.TagRightChar);
            foreach (TreeNode nd in nodes)
            {
                do_node(nd, output);

                if (nd.Nodes.Count > 0)
                    do_nodes(nd.Nodes, output);

                output.WriteEndTag("td");
                output.WriteEndTag("tr");
            }

            output.WriteEndTag("table");
            output.WriteEndTag("div");
        }

        private void wrapPostbackLink(HtmlTextWriter output, TreeNode nd)
        {
            output.WriteBeginTag("a");
            string tgt = this.ClientID + ":" + (nd.Expanded ? "-" : "+");
            output.WriteAttribute("href", "javascript:doWait(); __doPostBack('" + tgt + "','" + nd.GetNodeIndex() + "');");
            output.Write(HtmlTextWriter.TagRightChar);
            if (nd.ImageUrl != null && nd.ImageUrl != "")
            {
                output.WriteBeginTag("img");
                output.WriteAttribute("src", nd.ImageUrl);
                output.WriteAttribute("border", "0");
                output.Write(HtmlTextWriter.TagRightChar);
            }
            output.WriteEndTag("a");
        }

        protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            HttpContext ctx = HttpContext.Current;
            string tgt = ctx.Request.Form["__EVENTTARGET"];
            string arg = ctx.Request.Form["__EVENTARGUMENT"];
            if (tgt == null || arg == null)
                return false;

            string[] a_tgt = tgt.Split(new char[] { ':' });
            if (a_tgt[0].Equals(this.UniqueID))
            {
                // Ok, l'evento è per noi
                string[] a_arg = arg.Split(new char[] { ':' });
                switch (a_tgt[1])
                {
                    case CorrTreeView.EVT_EXPAND:
                    case CorrTreeView.EVT_COLLAPSE:
                        this.GetNodeFromIndex(a_arg[0]).Expanded = !this.GetNodeFromIndex(a_arg[0]).Expanded;
                        event_data.Key = a_tgt[1];
                        event_data.Value = a_arg[0];
                        break;

                    case CorrTreeView.EVT_SELECTORFILTER:
                        this.event_data.Key = CorrTreeView.EVT_SELECTORFILTER;
                        this.event_data.Value = new string[] { a_arg[0], a_arg[1] };
                        break;

                    default:
                        throw new NotImplementedException();
                }
                return true;
            }
            return false;
        }
        #region IPostBackDataHandler Members

        protected override void RaisePostDataChangedEvent()
        {
            TreeNode nd = this.GetNodeFromIndex((string)event_data.Value);
            switch ((string)event_data.Key)
            {
                case CorrTreeView.EVT_COLLAPSE:
                    nd.Expanded = false;
                    if (this.Collapse != null)
                        this.Collapse(this, new TreeViewClickEventArgs((string)event_data.Value));
                    break;

                case CorrTreeView.EVT_EXPAND:
                    nd.Expanded = true;
                    if (this.Expand != null)
                        this.Expand(this, new TreeViewClickEventArgs((string)event_data.Value));
                    break;

                case CorrTreeView.EVT_SELECTORFILTER:
                    if (SelectorFilter != null)
                    {
                        string[] args = (string[])event_data.Value;
                        SelectorFilter(this, new SelectorFilterArgs(args[0], args[1]));
                    }
                    break;
            }
        }

        #endregion

        private TreeNode get_node_from_data(TreeNodeCollection nodes, string data)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.NodeData == data)
                    return node;

                return get_node_from_data(node.Nodes, data);
            }
            return null;
        }

        public TreeNode GetNodeFromData(string data)
        {
            return get_node_from_data(this.Nodes, data);
        }
    }

    public enum TreeViewSelectorType
    {
        None,
        CheckBox,
        RadioButton
    }
}
