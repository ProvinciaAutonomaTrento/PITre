using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.Web.Services.Protocols;
using Microsoft.Web.UI.WebControls;
using System.Globalization;
using System.Configuration;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{

    public partial class RicercaSottoFascicoli : System.Web.UI.Page
    {
        protected Ruolo userRuolo;
        protected Registro userReg;
        protected Hashtable HashFolder;
        protected Microsoft.Web.UI.WebControls.TreeView Folders;
        protected Fascicolo fascicoloSelezionato = null;

        private int indexH;


        protected void Page_Load(object sender, EventArgs e)
        {
            // Response.Expires = -1;

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            getParameterUser();
            string idFascicolo = Request.QueryString["idfascicolo"];
            string desc = Request.QueryString["desc"];
            if (this.Page.Session["azione"] != null && this.Page.Session["azione"].ToString() == "ricerca")
            {

                string idFolderSel = Request.QueryString["idFolder"];
                if (string.IsNullOrEmpty(idFolderSel) && Folders != null)
                {
                    idFolderSel = Folders.GetNodeFromIndex(Folders.SelectedNodeIndex).ID;
                }

                CaricaDati(idFascicolo);

                foreach (Microsoft.Web.UI.WebControls.TreeNode node in this.Folders.Nodes)
                {
                    cerca(idFolderSel, node);
                }

                Session.Remove("azione");


                //CaricaDatiRicerca(idFascicolo, idFolderSel);
                //Session.Remove("azione");
            }
            else
            {
                if (!IsPostBack)
                {
                    CaricaDati(idFascicolo);
                    if (!string.IsNullOrEmpty(desc))
                        cerca(desc);
                }
            }
        }

        private bool cerca(string idFolderSel, Microsoft.Web.UI.WebControls.TreeNode node)
        {
            bool retValue = false;

            if (node.ID == idFolderSel)
            {
                this.Folders.SelectedNodeIndex = node.GetNodeIndex();
                retValue = true;
            }
            else
            {
                foreach (Microsoft.Web.UI.WebControls.TreeNode child in node.Nodes)
                {
                    retValue = (this.cerca(idFolderSel, child));

                    if (retValue)
                        break; 
                }
            }

            return retValue;
        }

        private void getParameterUser()
        {
            userRuolo = UserManager.getRuolo(this);
            userReg = UserManager.getRegistroSelezionato(this);

        }

        private void CaricaDati(string idFascicolo)
        {

            ClearTreeView();
            fascicoloSelezionato = FascicoliManager.getFascicoloById(this, idFascicolo);
            FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);
            Folder folder = FascicoliManager.getFolder(this, fascicoloSelezionato.systemID, "");

            LoadTreeviewLivelloZero(folder, fascicoloSelezionato);

        }

        private void CaricaDatiRicerca(string idFascicolo, string idFolderSel)
        {

            ClearTreeView();
            fascicoloSelezionato = FascicoliManager.getFascicoloById(this, idFascicolo);
            FascicoliManager.setFascicoloSelezionato(this, fascicoloSelezionato);
            Folder folder = FascicoliManager.getFolder(this, fascicoloSelezionato.systemID, "");

            Microsoft.Web.UI.WebControls.TreeNode rootFolder = new Microsoft.Web.UI.WebControls.TreeNode();
            //Creo la root folder dell'albero
            rootFolder.Text = fascicoloSelezionato.codice;
            rootFolder.ID = fascicoloSelezionato.systemID;

            if (folder.childs.Length > 0)
            {
                LoadTreeview(rootFolder, folder, idFolderSel);
            }

            Folders.Nodes.Add(rootFolder);

            Microsoft.Web.UI.WebControls.TreeNode nodoSel = getSelectedNodeFolder();
            if (nodoSel != null)
                Folders.SelectedNodeIndex = nodoSel.GetNodeIndex();
        }


        private Microsoft.Web.UI.WebControls.TreeNode getSelectedNodeFolder()
        {
            Microsoft.Web.UI.WebControls.TreeNode nodeToSelect = new Microsoft.Web.UI.WebControls.TreeNode();
            if (this.Page.Session["RicercaSottoFascicoli.nodoSelezionato"] != null)
            {
                nodeToSelect = (Microsoft.Web.UI.WebControls.TreeNode)this.Page.Session["RicercaSottoFascicoli.nodoSelezionato"];
            }
            return nodeToSelect;
        }
        #region Treeview

        private void LoadTreeview(Microsoft.Web.UI.WebControls.TreeNode myTreeNodo, DocsPaWR.Folder folder, string idFolderSel)
        {
            //if (folder.descrizione.ToLower() != myTreeNodo.Text)
            if (folder.descrizione.ToLower() != "root folder" && folder.descrizione.ToLower() != myTreeNodo.Text)
            {
                Microsoft.Web.UI.WebControls.TreeNode newAddedNode = addFolderNode(myTreeNodo, folder);

                if (folder.systemID == idFolderSel)
                {
                    this.ExpandAllParentNodes(newAddedNode);
                    this.Page.Session["RicercaSottoFascicoli.nodoSelezionato"] = newAddedNode;

                }

                myTreeNodo = newAddedNode;
            }
            int g = folder.childs.Length;
            for (int j = 0; j < g; j++)
            {
                DocsPaWR.Folder newFolder = folder.childs[j];

                //richiama la funzione ricorsivamente
                LoadTreeview(myTreeNodo, newFolder, idFolderSel);
            }
        }

        private Microsoft.Web.UI.WebControls.TreeNode addFolderNode(Microsoft.Web.UI.WebControls.TreeNode parentNode, DocsPaWR.Folder folder)
        {
            Microsoft.Web.UI.WebControls.TreeNode node = new Microsoft.Web.UI.WebControls.TreeNode();

            node.Text = folder.descrizione;
            node.ID = folder.systemID;

            //aggiunge il nodo creato al nodo genitore
            parentNode.Nodes.Add(node);

            return node;
        }

        private void ExpandAllParentNodes(Microsoft.Web.UI.WebControls.TreeNode node)
        {
            if (node.Parent != null)
            {
                Microsoft.Web.UI.WebControls.TreeNode parentNode = node.Parent as Microsoft.Web.UI.WebControls.TreeNode;

                if (parentNode != null)
                {
                    parentNode.Expanded = true;
                    this.ExpandAllParentNodes(parentNode);
                    //parentNode = null;
                }
            }
        }

        private void LoadTreeviewLivelloZero(DocsPaWR.Folder folder, DocsPaWR.Fascicolo fasc)
        {
            try
            {

                Microsoft.Web.UI.WebControls.TreeNode rootFolder = new Microsoft.Web.UI.WebControls.TreeNode();
                //Creo la root folder dell'albero
                rootFolder.Text = fasc.codice;
                rootFolder.ID = fasc.systemID;

                //carico il 1 livello
                CaricaNodi(rootFolder, folder);

                rootFolder.Expanded = true;
                //aggiungo la root folder alla collezione dei nodi dell'albero
                Folders.Nodes.Add(rootFolder);


            }
            catch
            {
                //   this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                //   this.GUI("ResetAll");
            }
        }

        private void ClearTreeView()
        {
            Folders.Nodes.Clear();
        }

        protected void Folders_Expand(object sender, TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    Microsoft.Web.UI.WebControls.TreeNode TreeNodo;
                    TreeNodo = (Microsoft.Web.UI.WebControls.TreeNode)Folders.GetNodeFromIndex(e.Node);

                    if (TreeNodo.Nodes.Count > 0)
                        TreeNodo.Nodes.Clear();

                    this.LoadTreeViewLivelloFigli(e.Node, TreeNodo.ID);

                    Folders.SelectedNodeIndex = e.Node;
                }
                else
                {
                    Microsoft.Web.UI.WebControls.TreeNode TreeNodo;
                    TreeNodo = (Microsoft.Web.UI.WebControls.TreeNode)Folders.GetNodeFromIndex(e.Node);
                    CaricaDati(TreeNodo.ID);
                }
            }
            catch
            {

            }
        }

        private void LoadTreeViewLivelloFigli(string indice, string idFolder)
        {
            try
            {
                string idFasc = Folders.Nodes[0].ID;
                Folders.SelectedNodeIndex = indice;

                Microsoft.Web.UI.WebControls.TreeNode myTreeNodo;
                myTreeNodo = (Microsoft.Web.UI.WebControls.TreeNode)Folders.GetNodeFromIndex(indice);
                myTreeNodo.Expanded = true;

                if (myTreeNodo.Nodes.Count > 0)
                    myTreeNodo.Nodes.RemoveAt(0);


                Microsoft.Web.UI.WebControls.TreeNode nodoFiglio;

                DocsPaWR.Folder folder = FascicoliManager.getFolder(this, idFasc, idFolder);

                if (folder != null)
                {
                    CaricaNodi(myTreeNodo, folder);
                }

            }
            catch
            {
                //this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
                //this.GUI("ResetAll");
            }
        }

        private static void CaricaNodi(Microsoft.Web.UI.WebControls.TreeNode myTreeNodo, DocsPaWR.Folder folder)
        {
            if (folder.childs.Length > 0)
            {
                for (int k = 0; k < folder.childs.Length; k++)
                {
                    Microsoft.Web.UI.WebControls.TreeNode node = new Microsoft.Web.UI.WebControls.TreeNode();

                    node.Text = folder.childs[k].descrizione;
                    node.ID = folder.childs[k].systemID;
                    node.Expanded = false;

                    if (folder.childs[k].childs.Length != 0)
                    {
                        Microsoft.Web.UI.WebControls.TreeNode nodeFiglio = new Microsoft.Web.UI.WebControls.TreeNode();
                        nodeFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        node.Nodes.Add(nodeFiglio);
                    }
                    myTreeNodo.Nodes.Add(node);

                }
            }
        }

        protected void Folders_SelectedIndexChange(object sender, TreeViewSelectEventArgs e)
        {

            try
            {

            }
            catch
            {

            }

        }

        #endregion

        #region EVENTI
        protected void btn_cerca_Click(object sender, ImageClickEventArgs e)
        {
            string descrizione = txt_descrizione.Text.Trim();
            Fascicolo fascSelezionato = FascicoliManager.getFascicoloSelezionato(this);
            string idFascicolo = fascSelezionato.systemID;
            string myHtml = string.Empty;
            this.Page.Session.Add("azione", "ricerca");
            if (descrizione != string.Empty)
            {
                Folder[] risultatiFolder = FascicoliManager.getFolderByDescrizione(this, idFascicolo, descrizione);

                if (risultatiFolder != null)
                {

                    for (int i = 0; i < risultatiFolder.Length; i++)
                    {
                        myHtml += "<TR bgColor=#FAFAFA>";
                        myHtml += "<TD class=testo_grigio_scuro><a href='RicercaSottofascicoli.aspx?idFolder=" + ((Folder)risultatiFolder[i]).systemID + "&idfascicolo=" + ((Folder)risultatiFolder[i]).idFascicolo + " ' class='testo_grigio_scuro'>" + fascSelezionato.codice + "</a></TD>";
                        myHtml += "<TD class=testo_grigio>" + ((Folder)risultatiFolder[i]).descrizione + "</TD>";
                        myHtml += "</TR>";
                    }

                    lbl_td.Text = myHtml;
                    pnl_ric.Visible = true;
                    Folders.Height = new Unit("180");
                }
                else
                {
                    lbl_td.Text = "";
                    pnl_ric.Visible = false;
                    lbl_msg.Text = "Nessun risultato trovato!";
                }
            }
        }

        
        private void cerca(string desc)
        {
            string descrizione = desc.Trim();
            txt_descrizione.Text = descrizione;
            Fascicolo fascSelezionato = FascicoliManager.getFascicoloSelezionato(this);
            string idFascicolo = fascSelezionato.systemID;
            string myHtml = string.Empty;
            this.Page.Session.Add("azione", "ricerca");
            if (descrizione != string.Empty)
            {
                Folder[] risultatiFolder = FascicoliManager.getFolderByDescrizione(this, idFascicolo, descrizione);
                if (risultatiFolder != null)
                {
                    for (int i = 0; i < risultatiFolder.Length; i++)
                    {
                        myHtml += "<TR bgColor=#FAFAFA>";
                        myHtml += "<TD class=testo_grigio_scuro><a href='RicercaSottofascicoli.aspx?idFolder=" + ((Folder)risultatiFolder[i]).systemID + "&idfascicolo=" + ((Folder)risultatiFolder[i]).idFascicolo + " ' class='testo_grigio_scuro'>" + fascSelezionato.codice + "</a></TD>";
                        myHtml += "<TD class=testo_grigio>" + ((Folder)risultatiFolder[i]).descrizione + "</TD>";
                        myHtml += "</TR>";
                    }
                    lbl_td.Text = myHtml;
                    pnl_ric.Visible = true;
                    Folders.Height = new Unit("180");
                }
                else
                {
                    lbl_msg.Text = "Nessun risultato trovato!";
                }
            }
        }

        protected void btn_ok_Click(object sender, EventArgs e)
        {
            string codice = string.Empty;
            string descrizione = string.Empty;
            string key = Folders.SelectedNodeIndex;
            bool isRootFolder = false;

            if (Folders.SelectedNodeIndex != "0")
            {
                Microsoft.Web.UI.WebControls.TreeNode NodoSel;
                NodoSel = (Microsoft.Web.UI.WebControls.TreeNode)Folders.GetNodeFromIndex(key);
                fascicoloSelezionato = FascicoliManager.getFascicoloSelezionato(this);

                string codFascicolo = fascicoloSelezionato.codice;
                string descrFasc = fascicoloSelezionato.descrizione;
                CalcolaFascicolazioneRapida(NodoSel, ref codice, ref descrizione, ref isRootFolder, codFascicolo);
                codice = codFascicolo + "//" + codice.Substring(0, codice.Length - 1);
                descrizione = descrFasc + "//" + descrizione.Substring(0, descrizione.Length - 1);


                Folder folderSel = FascicoliManager.getFolder(this, NodoSel.ID);

                if (fascicoloSelezionato != null)
                    fascicoloSelezionato.folderSelezionato = folderSel;

                FascicoliManager.setCodiceFascRapida(this, codice);
                FascicoliManager.setDescrizioneFascRapida(this, descrizione);
                FascicoliManager.setFascicoloSelezionatoFascRapida(this, fascicoloSelezionato);
                FascicoliManager.setFolderSelezionato(this, folderSel);
            }

            Session.Add("NodeIndexRicercaSottoFascicoli", Folders.SelectedNodeIndex);

            Response.Write("<script>window.returnValue = 'Y'; window.close();</script>");
        }

        protected void btn_chiudi_risultato_Click(object sender, ImageClickEventArgs e)
        {
            lbl_td.Text = "";
            pnl_ric.Visible = false;
        }

        private static void CalcolaFascicolazioneRapida(Microsoft.Web.UI.WebControls.TreeNode NodoSel, ref string codice, ref string descrizione, ref bool isRootFolder, string codFascicolo)
        {
            Microsoft.Web.UI.WebControls.TreeNode NodoParent;

            //if (NodoSel.Text != codFascicolo)
            //{
            codice = NodoSel.Text + "/" + codice;
            descrizione = NodoSel.Text + "/" + descrizione;
            NodoParent = (Microsoft.Web.UI.WebControls.TreeNode)NodoSel.Parent;
            if (NodoParent.Text.Equals(codFascicolo))
                isRootFolder = true;
            if (!isRootFolder)
                CalcolaFascicolazioneRapida(NodoParent, ref codice, ref descrizione, ref isRootFolder, codFascicolo);
            //}

        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chiudi", "window.close();", true);
        }
        #endregion




    }
}
