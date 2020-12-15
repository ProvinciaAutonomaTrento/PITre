using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DocsPAWA.smistaDoc
{
    public partial class selezioneSmistamento : System.Web.UI.Page
    {

        #region class myTreeNode
        public class myTreeNode : Microsoft.Web.UI.WebControls.TreeNode
        {
            //Tipo Nodo [Possibili Valori: U=(Unità organizz.), R=(Ruolo), P=(Utente) ]
            public string getTipoNodo()
            {
                return ViewState["tipoNodo"].ToString();
            }
            public void setTipoNodo(string tNodo)
            {
                ViewState["tipoNodo"] = tNodo;
            }

        }

        #endregion
      

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!Page.IsPostBack)
            {
                this.pulisciTreeView();
                smistaDoc.SmistaDocManager docManager = this.GetSmistaDocManager();
                DocsPaWR.UOSmistamento uoAppartenenza = docManager.GetUOAppartenenza();
                this.caricaTreeView(uoAppartenenza);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public smistaDoc.SmistaDocManager GetSmistaDocManager()
        {
            // Reperimento oggetto "DocumentoTrasmesso" corrente
            return SmistaDocSessionManager.GetSmistaDocManager();
        }

        private void caricaTreeViewUO(DocsPaWR.UOSmistamento uo, bool primaUo)
        {
            myTreeNode treenode = new myTreeNode();
            if (primaUo)
            {
                myTreeNode treenodeIntestazione = new myTreeNode();
                treenodeIntestazione.Text = "Selezioni Utente";
                treenodeIntestazione.DefaultStyle["font-weight"] = "bold";
                treenodeIntestazione.setTipoNodo("IS");
                treenodeIntestazione.Expanded = true; // Espande il nodo
                this.treeViewSelezioni.Nodes.Add(treenodeIntestazione);                                
            }
            treenode = (myTreeNode)treeViewSelezioni.Nodes[0];
            myTreeNode treenodeUo = new myTreeNode();
            treenodeUo.Text = uo.Descrizione;
            if (uo.FlagCompetenza || uo.FlagConoscenza)
                treenodeUo.Text += (uo.FlagCompetenza) ? " - (COMPETENZA)" : " - (CONOSCENZA)";
            treenodeUo.ID = uo.ID;
            treenodeUo.setTipoNodo("U");
            treenodeUo.Expanded = true;
            treenodeUo.DefaultStyle["font-weight"] = "bold";
            treenode.Nodes.Add(treenodeUo);
            if (uo.Ruoli != null && uo.Ruoli.Length > 0)
            {
                foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                {
                    if (ruolo.FlagCompetenza || ruolo.FlagConoscenza || this.isOneUtenteChecked(ruolo))
                    {
                        myTreeNode nodoRuolo = new myTreeNode();
                        nodoRuolo.Text = ruolo.Descrizione;
                        if ((ruolo.FlagCompetenza || ruolo.FlagConoscenza) && !uo.FlagCompetenza && !uo.FlagConoscenza)
                            nodoRuolo.Text += (ruolo.FlagCompetenza) ? " - (COMPETENZA)" : " - (CONOSCENZA)";
                        nodoRuolo.ID = ruolo.ID;
                        nodoRuolo.setTipoNodo("R");
                        nodoRuolo.Expanded = true;
                        treenodeUo.Nodes.Add(nodoRuolo);
                        this.caricaTreeViewUtenti(ruolo, nodoRuolo, false);
                    }
                }
            }
        }

        private void caricaTreeViewUtenti(DocsPAWA.DocsPaWR.RuoloSmistamento ruolo, myTreeNode nodoRuolo, bool daModello)
        {
            if (ruolo.Utenti != null && ruolo.Utenti.Length > 0)
            {
                foreach (DocsPAWA.DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
                {
                    if (utente.FlagCompetenza || utente.FlagConoscenza)
                    {
                        myTreeNode nodoUtente = new myTreeNode();
                        nodoUtente.Text = utente.Denominazione;
                        if (!ruolo.FlagCompetenza && !ruolo.FlagConoscenza)
                            nodoUtente.Text += (utente.FlagCompetenza) ? " - (COMPETENZA)" : " - (CONOSCENZA)";
                        nodoUtente.ID = utente.ID;
                        nodoUtente.setTipoNodo("P");
                        nodoUtente.Expanded = true;
                        nodoRuolo.Nodes.Add(nodoUtente);
                    }
                    else
                    {
                        if (daModello)
                        {
                            myTreeNode nodoUtente = new myTreeNode();
                            nodoUtente.Text = utente.Denominazione;                           
                            nodoUtente.ID = utente.ID;
                            nodoUtente.setTipoNodo("P");
                            nodoUtente.Expanded = true;
                            nodoRuolo.Nodes.Add(nodoUtente);
                        }
                    }
                }
            }
        }

        private void caricaTreeView(DocsPaWR.UOSmistamento uoAppartenenza)
        {
            bool primaUo = true;
            if (uoAppartenenza.FlagCompetenza || uoAppartenenza.FlagConoscenza || this.isOneRuoloChecked(uoAppartenenza))
            {
                this.caricaTreeViewUO(uoAppartenenza,  primaUo);
                primaUo = false;
            }
            this.scorriUoInferiori(uoAppartenenza, primaUo);
            this.caricaTreeViewUoTrasmRapida(uoAppartenenza);
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
                foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
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
                foreach (DocsPAWA.DocsPaWR.UtenteSmistamento utente in ruolo.Utenti)
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

        private void pulisciTreeView()
        {
            this.treeViewSelezioni.Nodes.Clear();
        }

        protected void btn_indietro_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }

        protected void treeViewSelezioni_SelectedIndexChange(object sender, Microsoft.Web.UI.WebControls.TreeViewSelectEventArgs e)
        {
            myTreeNode TreeNodo;
            TreeNodo = (myTreeNode)treeViewSelezioni.GetNodeFromIndex(e.NewNode);
            string tipoNodo = TreeNodo.getTipoNodo();
            if (tipoNodo == "U")
            {
                Response.Write("<script>window.returnValue='" + TreeNodo.ID + "'; window.close();</script>");
            }
        }

        private void caricaTreeViewUoTrasmRapida(DocsPaWR.UOSmistamento uoAppartenenza)
        {           
            if(uoAppartenenza.UoSmistaTrasAutomatica!=null && uoAppartenenza.UoSmistaTrasAutomatica.Length>0)
            {
                myTreeNode treenodeIntestazione = new myTreeNode();
                treenodeIntestazione.Text = "Modello";                
                treenodeIntestazione.DefaultStyle["font-weight"] = "bold";
                treenodeIntestazione.setTipoNodo("IM");
                treenodeIntestazione.Expanded = true;
                this.treeViewSelezioni.Nodes.Add(treenodeIntestazione);
               
                foreach (DocsPaWR.UOSmistamento uo in uoAppartenenza.UoSmistaTrasAutomatica)
                {                  
                    if (!string.IsNullOrEmpty(uo.ragioneTrasmRapida))
                    {
                        myTreeNode treenodeUo = new myTreeNode();
                        
                        treenodeUo.Text = uo.Descrizione + " - (" + uo.ragioneTrasmRapida + ")";
                        treenodeUo.ID = uo.ID;
                        treenodeUo.Expanded = true;
                        treenodeUo.setTipoNodo("UM");
                        treenodeIntestazione.Nodes.Add(treenodeUo);

                        if (uo.Ruoli != null && uo.Ruoli.Length > 0)
                        {
                            foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                            {
                                myTreeNode nodoRuolo = new myTreeNode();
                                nodoRuolo.Text = ruolo.Descrizione;
                                nodoRuolo.ID = ruolo.ID;
                                nodoRuolo.setTipoNodo("R");
                                nodoRuolo.Expanded = true;
                                treenodeUo.Nodes.Add(nodoRuolo);
                                this.caricaTreeViewUtenti(ruolo, nodoRuolo, true);
                            }
                        }
                    }
                    else
                    {
                        if (uo.Ruoli != null && uo.Ruoli.Length > 0)
                        {
                            foreach (DocsPAWA.DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                            {
                                if (!string.IsNullOrEmpty(ruolo.ragioneTrasmRapida))
                                {
                                    myTreeNode nodoRuolo = new myTreeNode();
                                    nodoRuolo.Text = ruolo.Descrizione + " - (" + ruolo.ragioneTrasmRapida + ")";
                                    nodoRuolo.ID = ruolo.ID;
                                    nodoRuolo.setTipoNodo("R");
                                    nodoRuolo.Expanded = true;
                                    treenodeIntestazione.Nodes.Add(nodoRuolo);
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
                                                nodoUtente.setTipoNodo("P");
                                                nodoUtente.Expanded = true;
                                                treenodeIntestazione.Nodes.Add(nodoUtente);
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

    }
}
