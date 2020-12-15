using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class SmistaDocSelectionsDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!Page.IsPostBack)
            {
                this.InitializePage();
                this.pulisciTreeView();
                SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
                this.caricaTreeView(uoAppartenenza);
            }
        }

        protected void InitializePage()
        {
            this.InitializeLabel();
        }

        protected void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SmistaDocSelDetBtnClose.Text = Utils.Languages.GetLabelFromCode("SmistaDocSelDetBtnClose", language);
        }

        public SmistaDocManager GetSmistaDocManager()
        {
            string idDocumento = Request.QueryString["DOC_NUMBER"];

            if (idDocumento == null) idDocumento = "";

            // Reperimento oggetto "DocumentoTrasmesso" corrente
            return SmistaDocSessionManager.GetSmistaDocManager(idDocumento);
        }

        private void caricaTreeView(DocsPaWR.UOSmistamento uoAppartenenza)
        {
            bool primaUo = true;
            if (uoAppartenenza.FlagCompetenza || uoAppartenenza.FlagConoscenza || this.isOneRuoloChecked(uoAppartenenza))
            {
                this.caricaTreeViewUO(uoAppartenenza, primaUo);
                primaUo = false;
            }
            this.scorriUoInferiori(uoAppartenenza, primaUo);
            this.caricaTreeViewUoTrasmRapida(uoAppartenenza);
        }

        private void caricaTreeViewUO(DocsPaWR.UOSmistamento uo, bool primaUo)
        {
            myTreeNode treenode = new myTreeNode();

            if (primaUo)
            {
                myTreeNode treenodeIntestazione = new myTreeNode();
                treenodeIntestazione.Text = "Selezioni Utente";
                treenodeIntestazione.TIPO = "IS";
                treenodeIntestazione.Expanded = true;
                treenodeIntestazione.Selected = true;
                this.TreeSmistaDocSelection.Nodes.Add(treenodeIntestazione);
            }
            treenode = (myTreeNode)TreeSmistaDocSelection.Nodes[0];
            myTreeNode treenodeUo = new myTreeNode();
            treenodeUo.Text = uo.Descrizione;
            if (uo.FlagCompetenza || uo.FlagConoscenza)
                treenodeUo.Text += (uo.FlagCompetenza) ? " - (COMPETENZA)" : " - (CONOSCENZA)";
            treenodeUo.ID = uo.ID;
            treenodeUo.TIPO = "U";
            treenodeUo.Expanded = true;
            treenodeUo.ImageUrl = "~/Images/Icons/uo_icon.png";
            treenode.ChildNodes.Add(treenodeUo);

            if (uo.Ruoli != null && uo.Ruoli.Length > 0)
            {
                foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                {
                    if (ruolo.FlagCompetenza || ruolo.FlagConoscenza || this.isOneUtenteChecked(ruolo))
                    {
                        myTreeNode nodoRuolo = new myTreeNode();
                        nodoRuolo.Text = ruolo.Descrizione;
                        if ((ruolo.FlagCompetenza || ruolo.FlagConoscenza) && !uo.FlagCompetenza && !uo.FlagConoscenza)
                            nodoRuolo.Text += (ruolo.FlagCompetenza) ? " - (COMPETENZA)" : " - (CONOSCENZA)";
                        nodoRuolo.ID = ruolo.ID;
                        nodoRuolo.TIPO = "R";
                        nodoRuolo.Expanded = true;
                        nodoRuolo.ImageUrl = "~/Images/Icons/role2_icon.png";
                        treenodeUo.ChildNodes.Add(nodoRuolo);
                        this.caricaTreeViewUtenti(ruolo, nodoRuolo, false);
                    }
                }
            }
        }

        private void caricaTreeViewUtenti(DocsPaWR.RuoloSmistamento ruolo, myTreeNode nodoRuolo, bool daModello)
        {
            if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
            {
                foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                {
                    if (utente.FlagCompetenza || utente.FlagConoscenza)
                    {
                        myTreeNode nodoUtente = new myTreeNode();
                        nodoUtente.Text = utente.Denominazione;
                        if (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza)
                            nodoUtente.Text += (utente.FlagCompetenza) ? " - (COMPETENZA)" : " - (CONOSCENZA)";
                        nodoUtente.ID = utente.ID;
                        nodoUtente.TIPO = "P";
                        nodoUtente.Expanded = true;
                        nodoUtente.ImageUrl = "~/Images/Icons/user_icon.png";
                        nodoRuolo.ChildNodes.Add(nodoUtente);
                    }
                    else
                    {
                        if (daModello)
                        {
                            myTreeNode nodoUtente = new myTreeNode();
                            nodoUtente.Text = utente.Denominazione;
                            nodoUtente.ID = utente.ID;
                            nodoUtente.TIPO = "P";
                            nodoUtente.Expanded = true;
                            nodoRuolo.ChildNodes.Add(nodoUtente);
                        }
                    }
                }
            }
        }

        private void scorriUoInferiori(DocsPaWR.UOSmistamento uo, bool primaUo)
        {
            if (uo.UoInferiori != null && uo.UoInferiori.Length > 0)
            {
                foreach (DocsPaWR.UOSmistamento uoInf in uo.UoInferiori)
                {
                    if (uoInf.FlagCompetenza || uoInf.FlagConoscenza || this.isOneRuoloChecked(uoInf))
                    {
                        this.caricaTreeViewUO(uoInf, primaUo);
                        primaUo = false;
                    }
                    scorriUoInferiori(uoInf, primaUo);
                }
            }
        }

        private bool isOneRuoloChecked(DocsPaWR.UOSmistamento uo)
        {
            bool result = false;
            if (uo.Ruoli != null && uo.Ruoli.Length > 0)
            {
                foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                {
                    if (ruolo.FlagCompetenza || ruolo.FlagConoscenza || this.isOneUtenteChecked(ruolo))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private bool isOneUtenteChecked(DocsPaWR.RuoloSmistamento ruolo)
        {
            bool result = false;
            if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
            {
                foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                {
                    if (utente.FlagCompetenza || utente.FlagConoscenza)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }



        private void caricaTreeViewUoTrasmRapida(DocsPaWR.UOSmistamento uoAppartenenza)
        {
            if (uoAppartenenza.UoSmistaTrasAutomatica != null && uoAppartenenza.UoSmistaTrasAutomatica.Length > 0)
            {
                myTreeNode treenodeIntestazione = new myTreeNode();
                treenodeIntestazione.Text = "Modello";
                treenodeIntestazione.TIPO = "IM";
                treenodeIntestazione.Expanded = true;
                this.TreeSmistaDocSelection.Nodes.Add(treenodeIntestazione);

                foreach (DocsPaWR.UOSmistamento uo in uoAppartenenza.UoSmistaTrasAutomatica)
                {
                    if (!string.IsNullOrEmpty(uo.ragioneTrasmRapida))
                    {
                        myTreeNode treenodeUo = new myTreeNode();

                        treenodeUo.Text = uo.Descrizione + " - (" + uo.ragioneTrasmRapida + ")";
                        treenodeUo.ID = uo.ID;
                        treenodeUo.Expanded = true;
                        treenodeUo.TIPO = "UM";
                        treenodeIntestazione.ChildNodes.Add(treenodeUo);

                        if (uo.Ruoli != null && uo.Ruoli.Length > 0)
                        {
                            foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                            {
                                myTreeNode nodoRuolo = new myTreeNode();
                                nodoRuolo.Text = ruolo.Descrizione;
                                nodoRuolo.ID = ruolo.ID;
                                nodoRuolo.TIPO = "R";
                                nodoRuolo.Expanded = true;
                                treenodeUo.ChildNodes.Add(nodoRuolo);
                                this.caricaTreeViewUtenti(ruolo, nodoRuolo, true);
                            }
                        }
                    }
                    else
                    {
                        if (uo.Ruoli != null && uo.Ruoli.Length > 0)
                        {
                            foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                            {
                                if (!string.IsNullOrEmpty(ruolo.ragioneTrasmRapida))
                                {
                                    myTreeNode nodoRuolo = new myTreeNode();
                                    nodoRuolo.Text = ruolo.Descrizione + " - (" + ruolo.ragioneTrasmRapida + ")";
                                    nodoRuolo.ID = ruolo.ID;
                                    nodoRuolo.TIPO = "R";
                                    nodoRuolo.Expanded = true;
                                    treenodeIntestazione.ChildNodes.Add(nodoRuolo);
                                    this.caricaTreeViewUtenti(ruolo, nodoRuolo, true);
                                }
                                else
                                {
                                    if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
                                    {
                                        foreach (DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                                        {
                                            if (!string.IsNullOrEmpty(utente.ragioneTrasmRapida))
                                            {
                                                myTreeNode nodoUtente = new myTreeNode();
                                                nodoUtente.Text = utente.Denominazione + " - (" + utente.ragioneTrasmRapida + ")";
                                                nodoUtente.ID = utente.ID;
                                                nodoUtente.TIPO = "P";
                                                nodoUtente.Expanded = true;
                                                treenodeIntestazione.ChildNodes.Add(nodoUtente);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void pulisciTreeView()
        {
            this.TreeSmistaDocSelection.Nodes.Clear();
        }

        protected void SmistaDocSelDetBtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('SmistaDocSelectionsDetails','');", true);
        }

        protected void TreeSmistaDocSelection_SelectedNodeChanged(object sender, EventArgs e)
        {
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)TreeSmistaDocSelection.SelectedNode; // .GetNodeFromIndex(e.NewNode);

            string tipoNodo = TreeNodo.TIPO; // .getTipoNodo();
            if (tipoNodo == "U")
            {
                //SmistaDocSelDetBtnClose_Click(null, null);
                // Response.Write("<script>window.returnValue='" + TreeNodo.ID + "'; window.close();</script>");
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "disallow", "parent.disallowOp('Content2');", true);
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('SmistaDocSelectionsDetails','" + TreeNodo.ID + "');", true);
            }
        }
    }
}