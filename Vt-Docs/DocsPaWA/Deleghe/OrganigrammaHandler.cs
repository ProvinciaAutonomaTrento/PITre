using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI;

namespace DocsPAWA.Deleghe
{
    public class OrganigrammaHandler
    {
        private Microsoft.Web.UI.WebControls.TreeView treeView;
        private Label lbl_messaggio;
        private Label lbl_avviso;
        private DropDownList ddl_ricTipo;
        private HtmlInputHidden hd_returnValueModal;
        private ImageButton btn_find;
        private Button btn_conferma;
        private TextBox txt_ricDesc;
        private Page page;
        private string httpFullPath;
        private string js_function;

        public OrganigrammaHandler(Microsoft.Web.UI.WebControls.TreeView treeView,
            Label lbl_messaggio,
            Label lbl_avviso,
            DropDownList ddl_ricTipo,
            HtmlInputHidden hd_returnValueModal,
            ImageButton btn_find,
            Button btn_conferma,
            TextBox txt_ricDesc,
            string js_function,
            Page page)
        {
            this.treeView = treeView;
            this.lbl_messaggio = lbl_messaggio;
            this.lbl_avviso = lbl_avviso;
            this.ddl_ricTipo = ddl_ricTipo;
            this.hd_returnValueModal = hd_returnValueModal;
            this.btn_find = btn_find;
            this.btn_conferma = btn_conferma;
            this.txt_ricDesc = txt_ricDesc;
            this.js_function = js_function;
            this.page = page;
            this.httpFullPath = DocsPAWA.Utils.getHttpFullPath(this.page);
        }

        public void Load()
        {
            this.page.SetFocus(this.txt_ricDesc);
            if (!string.IsNullOrEmpty(this.hd_returnValueModal.Value) && this.hd_returnValueModal.Value != "undefined")
            {
                this.RicercaNodo(this.hd_returnValueModal.Value, "");
                this.page.SetFocus(this.btn_conferma);
            }
        }

        public void Init()
        {
            this.treeView.Expand += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Expand);
            this.treeView.SelectedIndexChange += new Microsoft.Web.UI.WebControls.SelectEventHandler(this.treeViewUO_SelectedIndexChange);
            this.treeView.Collapse += new Microsoft.Web.UI.WebControls.ClickEventHandler(this.treeViewUO_Collapse);
        }

        public void RicercaUtente(string idUtente, string idRuolo)
        {
            RicercaNodo(idUtente + "_" + idRuolo + "_PC", "PC");
        }

        public void RicercaNodo(string returnValue, string tipo)
        {
            try
            {
                //this.hd_returnValueModal.Value = "";
                string[] appo = returnValue.Split('_');
                string idCorrGlobale = appo[0];
                string idParent = appo[1];
                if (string.IsNullOrEmpty(tipo))
                    tipo = this.ddl_ricTipo.SelectedValue;
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaUOLivelloZero(idAmm);
                this.LoadTreeviewLivelloZero(theManager.getListaUO());
                theManager.ListaIDParentRicerca(idParent, tipo);
                if (theManager.getListaIDParentRicerca() != null && theManager.getListaIDParentRicerca().Count > 0)
                {
                    ArrayList lista = new ArrayList();
                    theManager.getListaIDParentRicerca().Reverse();
                    lista = theManager.getListaIDParentRicerca();
                    lista.Add(Convert.ToInt32(idCorrGlobale));

                    for (int n = 1; n <= lista.Count - 1; n++)
                    {
                        OrganigrammaTreeNode TreeNodo;
                        TreeNodo = (OrganigrammaTreeNode)this.treeView.GetNodeFromIndex(treeView.SelectedNodeIndex);
                        foreach (OrganigrammaTreeNode nodo in TreeNodo.Nodes)
                        {
                            if (nodo.ID.Equals(lista[n].ToString()) && nodo.ID != idCorrGlobale)
                            {
                                if (nodo.getTipoNodo().Equals("U"))
                                {
                                    this.LoadTreeViewLivelloFigli(nodo.GetNodeIndex(), nodo.getTipoNodo());
                                }
                                else
                                {
                                    nodo.Expanded = true;
                                }
                                treeView.SelectedNodeIndex = nodo.GetNodeIndex();
                                break;
                            }
                            if (nodo.ID.Equals(lista[n].ToString()) && nodo.ID.Equals(idCorrGlobale))
                            {
                                treeView.SelectedNodeIndex = nodo.GetNodeIndex();
                                break;
                            }
                        }
                    }
                }
                this.hd_returnValueModal.Value = "";
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        public void InizializeTree()
        {
            try
            {
                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                if (!string.IsNullOrEmpty(idAmm))
                {
                    theManager.ListaUOLivelloZero(idAmm);
                    if (theManager.getListaUO() != null && theManager.getListaUO().Count > 0)
                    {
                        this.btn_find.Attributes.Add("onclick", js_function+"('" + idAmm + "');");
                        this.LoadTreeviewLivelloZero(theManager.getListaUO());
                    }
                }
                else
                {
                    this.lbl_avviso.Text = "Attenzione! l'amministrazione corrente non risulta essere presente nel database.<br><br>Effettuare il Chiudi e trasmetti.";
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void LoadTreeviewLivelloZero(ArrayList listaUO)
        {
            try
            {
                treeView.Nodes.Clear();
                Microsoft.Web.UI.WebControls.TreeNode treenode = new Microsoft.Web.UI.WebControls.TreeNode();
                treenode.Text = "Organigramma";
                treeView.Nodes.Add(treenode);

                Microsoft.Web.UI.WebControls.TreeNode tNode = new Microsoft.Web.UI.WebControls.TreeNode();
                tNode = treeView.Nodes[0];

                OrganigrammaTreeNode nodoT;
                OrganigrammaTreeNode nodoFiglio;

                foreach (DocsPAWA.DocsPaWR.OrgUO uo in listaUO)
                {
                    nodoT = new OrganigrammaTreeNode();
                    nodoT.ID = uo.IDCorrGlobale;
                    nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
                    nodoT.ImageUrl = httpFullPath + "/AdminTool/Images/uo.gif";
                    tNode.Nodes.Add(nodoT);
                    nodoT.setTipoNodo("U");
                    nodoT.setIDCorrGlobale(uo.IDCorrGlobale);
                    nodoT.setCodice(uo.Codice);
                    nodoT.setCodiceRubrica(uo.CodiceRubrica);
                    nodoT.setDescrizione(uo.Descrizione);
                    nodoT.setLivello(uo.Livello);
                    nodoT.setIDAmministrazione(uo.IDAmministrazione);
                    nodoT.setCodRegInterop(uo.CodiceRegistroInterop);
                    nodoT.setPercorso(uo.Descrizione + " &gt; ");

                    if ((!uo.Ruoli.Equals("0")) || (!uo.SottoUo.Equals("0")))
                    {
                        nodoFiglio = new OrganigrammaTreeNode();
                        nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        nodoT.Nodes.Add(nodoFiglio);
                    }
                    else
                    {
                        nodoT.Text = uo.CodiceRubrica + " - " + uo.Descrizione;
                    }
                }
                tNode.Expanded = true;
                this.SelezionaPrimo();
                this.LoadTreeViewLivelloFigli("0.0", "U");
            }
            catch
            {
                lbl_messaggio.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void SelezionaPrimo()
        {
            try
            {
                treeView.SelectedNodeIndex = "0.0";
                OrganigrammaTreeNode TreeNodo;
                TreeNodo = (OrganigrammaTreeNode) treeView.GetNodeFromIndex("0.0");
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void LoadTreeViewLivelloFigli(string indice, string tipoNodo)
        {
            try
            {
                treeView.SelectedNodeIndex = indice;
                OrganigrammaTreeNode TreeNodo;
                TreeNodo = (OrganigrammaTreeNode) treeView.GetNodeFromIndex(indice);
                TreeNodo.Expanded = true;
                if (TreeNodo.Nodes.Count > 0)
                    TreeNodo.Nodes.RemoveAt(0);
                OrganigrammaTreeNode nodoRuoli;
                OrganigrammaTreeNode nodoUtenti;
                OrganigrammaTreeNode nodoUO;
                OrganigrammaTreeNode nodoFiglio;
                Amministrazione.Manager.OrganigrammaManager theManager = new Amministrazione.Manager.OrganigrammaManager();
                theManager.ListaRuoliUO(TreeNodo.getIDCorrGlobale());
                ArrayList lista = new ArrayList();
                lista = theManager.getListaRuoliUO();
                // ... ruoli
                if (lista != null && lista.Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgRuolo ruolo in lista)
                    {
                        nodoRuoli = new OrganigrammaTreeNode();
                        nodoRuoli.ID = ruolo.IDCorrGlobale;
                        nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
                        nodoRuoli.ImageUrl = httpFullPath + "/AdminTool/Images/ruolo.gif";

                        TreeNodo.Nodes.Add(nodoRuoli);

                        nodoRuoli.setTipoNodo("R");
                        nodoRuoli.setIDCorrGlobale(ruolo.IDCorrGlobale);
                        nodoRuoli.setIDTipoRuolo(ruolo.IDTipoRuolo);
                        nodoRuoli.setIDGruppo(ruolo.IDGruppo);
                        nodoRuoli.setCodice(ruolo.Codice);
                        nodoRuoli.setCodiceRubrica(ruolo.CodiceRubrica);
                        nodoRuoli.setDescrizione(ruolo.Descrizione);
                        nodoRuoli.setDiRiferimento(ruolo.DiRiferimento);
                        nodoRuoli.setIDAmministrazione(ruolo.IDAmministrazione);
                        nodoRuoli.setPercorso(TreeNodo.getPercorso() + ruolo.Descrizione + " &gt; ");
                        nodoRuoli.setResponsabile(ruolo.Responsabile);
                        // ... utenti
                        if (ruolo.Utenti.Length > 0)
                        {
                            foreach (DocsPAWA.DocsPaWR.OrgUtente utente in ruolo.Utenti)
                            {
                                nodoUtenti = new OrganigrammaTreeNode();
                                nodoUtenti.ID = utente.IDCorrGlobale;
                                nodoUtenti.Text = utente.CodiceRubrica + " - " + utente.Cognome + " " + utente.Nome;
                                nodoUtenti.ImageUrl = httpFullPath + "/AdminTool/Images/utente.gif";
                                nodoRuoli.Nodes.Add(nodoUtenti);
                                nodoUtenti.setTipoNodo("P");
                                nodoUtenti.setIDCorrGlobale(utente.IDCorrGlobale);
                                nodoUtenti.setIDPeople(utente.IDPeople);
                                nodoUtenti.setCodice(utente.Codice);
                                nodoUtenti.setCodiceRubrica(utente.CodiceRubrica);
                                nodoUtenti.setIDAmministrazione(utente.IDAmministrazione);
                            }
                        } // fine inserimento utenti	
                        else
                        {
                            nodoRuoli.Text = ruolo.CodiceRubrica + " - " + ruolo.Descrizione;
                        }
                    } // fine inserimento ruoli 						
                }

                // ... uo sottostanti				
                int livello = Convert.ToInt32(TreeNodo.getLivello()) + 1;

                theManager.ListaUO(TreeNodo.getIDCorrGlobale(), livello.ToString(), TreeNodo.getIDAmministrazione());
                lista = theManager.getListaUO();

                if (lista != null && lista.Count > 0)
                {
                    foreach (DocsPAWA.DocsPaWR.OrgUO sub_uo in lista)
                    {
                        nodoUO = new OrganigrammaTreeNode();
                        nodoUO.ID = sub_uo.IDCorrGlobale;
                        nodoUO.Text = sub_uo.CodiceRubrica + " - " + sub_uo.Descrizione;
                        nodoUO.ImageUrl = httpFullPath + "/AdminTool/Images/uo.gif";
                        TreeNodo.Nodes.Add(nodoUO);
                        nodoUO.setTipoNodo("U");
                        nodoUO.setIDCorrGlobale(sub_uo.IDCorrGlobale);
                        nodoUO.setCodice(sub_uo.Codice);
                        nodoUO.setCodiceRubrica(sub_uo.CodiceRubrica);
                        nodoUO.setDescrizione(sub_uo.Descrizione);
                        nodoUO.setLivello(sub_uo.Livello);
                        nodoUO.setIDAmministrazione(sub_uo.IDAmministrazione);
                        nodoUO.setCodRegInterop(sub_uo.CodiceRegistroInterop);
                        nodoUO.setPercorso(TreeNodo.getPercorso() + sub_uo.Descrizione + " &gt; ");

                        if ((!sub_uo.Ruoli.Equals("0")) || (!sub_uo.SottoUo.Equals("0")))
                        {
                            nodoFiglio = new OrganigrammaTreeNode();
                            nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                            nodoUO.Nodes.Add(nodoFiglio);
                        }
                        else
                        {
                            nodoUO.Text = sub_uo.CodiceRubrica + " - " + sub_uo.Descrizione;
                        }
                    } // fine inserimento uo sottostanti
                }
            }
            catch
            {
                lbl_messaggio.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void treeViewUO_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
        {
            try
            {
                if (!e.NewNode.Equals("0"))
                {
                    OrganigrammaTreeNode TreeNodo;
                    TreeNodo = (OrganigrammaTreeNode)treeView.GetNodeFromIndex(e.NewNode);
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void treeViewUO_Expand(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    OrganigrammaTreeNode TreeNodo;
                    TreeNodo = (OrganigrammaTreeNode)treeView.GetNodeFromIndex(e.Node);

                    if (TreeNodo.getTipoNodo().Equals("U"))
                    {
                        if (TreeNodo.Nodes.Count > 0)
                            TreeNodo.Nodes.Clear();

                        this.LoadTreeViewLivelloFigli(e.Node, TreeNodo.getTipoNodo());
                    }
                    treeView.SelectedNodeIndex = e.Node;
                }
                else
                {
                    this.InizializeTree();
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        private void treeViewUO_Collapse(object sender, Microsoft.Web.UI.WebControls.TreeViewClickEventArgs e)
        {
            try
            {
                if (e.Node != "0")
                {
                    OrganigrammaTreeNode TreeNodo;
                    TreeNodo = (OrganigrammaTreeNode) treeView.GetNodeFromIndex(e.Node);

                    Microsoft.Web.UI.WebControls.TreeNode nodoFiglio;

                    if (TreeNodo.getTipoNodo().Equals("U"))
                    {
                        if (TreeNodo.Nodes.Count > 0)
                            TreeNodo.Nodes.Clear();

                        nodoFiglio = new Microsoft.Web.UI.WebControls.TreeNode();
                        nodoFiglio.Text = "<font color='#ff0000'>&nbsp;loading...</font>";
                        TreeNodo.Nodes.Add(nodoFiglio);
                    }
                    treeView.SelectedNodeIndex = e.Node;
                }
            }
            catch
            {
                this.lbl_avviso.Text = "Attenzione! si è verificato un errore";
            }
        }

        public OrganigrammaTreeNode getNodeSelected()
        {
            return (OrganigrammaTreeNode) treeView.GetNodeFromIndex(treeView.SelectedNodeIndex);
        }

    }
}