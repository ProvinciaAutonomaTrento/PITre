using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SAAdminTool.AdminTool.Gestione_Titolario
{
    public partial class IndiceSistematico : System.Web.UI.Page
    {
        protected string idAmministrazione = string.Empty;
        protected SAAdminTool.DocsPaWR.OrgNodoTitolario nodoSelezionato = null;

        private SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
		
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session["AdminBookmark"] = "Indice Sistematico";

                //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------
                if (Session.IsNewSession)
                {
                    Response.Redirect("../Exit.aspx?FROM=EXPIRED");
                }

                AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
                if (!ws.CheckSession(Session.SessionID))
                {
                    Response.Redirect("../Exit.aspx?FROM=ABORT");
                }
                // ---------------------------------------------------------------

                
                if (Session["nodoSelPerIndice"] != null)
                {
                    //Recupero il nodo selezionato
                    nodoSelezionato = (SAAdminTool.DocsPaWR.OrgNodoTitolario)Session["nodoSelPerIndice"];
                    
                    //Valorizzo l'idAmministrazione
                    idAmministrazione = wws.getIdAmmByCod(nodoSelezionato.CodiceAmministrazione);

                    //Recupero il titolario di appartenenza
                    SAAdminTool.DocsPaWR.OrgTitolario titolario = wws.getTitolarioById(nodoSelezionato.ID_Titolario);

                    //Valorizzo delle label di informazione
                    lbl_titolario.Text = titolario.Descrizione;
                    lbl_nodoTitolario.Text = nodoSelezionato.Codice + " - " + nodoSelezionato.Descrizione;

                    if (!IsPostBack)
                    {
                        //Carico le voci disponibili per l'amministrazione 
                        caricaVociDisponibili();

                        //Cerico le voci associate al nodo selezionato
                        caricaVociAssociate();
                    }
                }                
            }
            catch
            {
                ClientScript.RegisterStartupScript(this.GetType(), "errore", "alert('Si è verificato un errore durante il caricamento della pagina.');", true);
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            //Rimuovo da session il nodo selezionato
            Session.Remove("nodoSelPerIndice");

            //Chiudo la popup
            ClientScript.RegisterStartupScript(this.GetType(), "chiusura", "window.close();", true);
        }

        protected void btn_aggiungiVoce_Click(object sender, ImageClickEventArgs e)
        {
            if (txt_nuovaVoce.Text != "")
            {
                //Inserisco la nuova voce
                SAAdminTool.DocsPaWR.VoceIndiceSistematico nuovaVoce = new SAAdminTool.DocsPaWR.VoceIndiceSistematico();
                nuovaVoce.idAmm = idAmministrazione;
                nuovaVoce.voceIndice = txt_nuovaVoce.Text.Replace("'", "''");
                if (wws.existVoceIndice(nuovaVoce) == null)
                {
                    wws.addNuovaVoceIndice(nuovaVoce);
                    
                    //Ricarico la lista della voci disponibili
                    caricaVociDisponibili();
                }

                txt_nuovaVoce.Text = "";
            }
        }

        protected void btn_rimuoviVoce_Click(object sender, ImageClickEventArgs e)
        {
            if (lbx_vociDisponibili.SelectedItem != null)
            {
                SAAdminTool.DocsPaWR.VoceIndiceSistematico voceDaEliminare = new SAAdminTool.DocsPaWR.VoceIndiceSistematico();
                ListItem itemSelezionato = lbx_vociDisponibili.SelectedItem;
                voceDaEliminare.systemId = itemSelezionato.Value;
                voceDaEliminare.voceIndice = itemSelezionato.Text;
                voceDaEliminare.idProject = nodoSelezionato.ID;
                
                //Verifico che la voce non sia associata a qualche nodo
                if (wws.isAssociataVoce(voceDaEliminare,false))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "voceNonEliminabile", "alert('Non è possibile eliminare una voce associata ad un nodo di un titolario.');", true);
                    return;
                }

                //Elimino la voce selezionata
                wws.removeVoceIndice(voceDaEliminare);

                //Ricarico la lista della voci disponibili
                caricaVociDisponibili();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "eliminaVoce", "alert('Selezionare una voce da eliminare fra quelle disponibili.');", true);
            }
        }

        protected void btn_associaVoce_Click(object sender, EventArgs e)
        {
            if (lbx_vociDisponibili.SelectedItem != null)
            {
                ListItem itemSelezionato = lbx_vociDisponibili.SelectedItem;

                //Verifico che la voce non è già associata
                for (int i = 0; i < lbx_vociAssociate.Items.Count; i++)
                {
                    if(itemSelezionato.Text.Equals(lbx_vociAssociate.Items[i].Text))
                        return;
                }

                //Creo la voce di indice da associare
                SAAdminTool.DocsPaWR.VoceIndiceSistematico voceDaAssociare = new SAAdminTool.DocsPaWR.VoceIndiceSistematico();
                voceDaAssociare.systemId = itemSelezionato.Value;
                voceDaAssociare.idAmm = idAmministrazione;
                voceDaAssociare.voceIndice = itemSelezionato.Text;
                voceDaAssociare.idProject = nodoSelezionato.ID;

                //Associo la voce al nodo di titolario selezioanto
                wws.associaVoceIndice(voceDaAssociare);

                //Ricarico le voci associate
                caricaVociAssociate();
            }
        }

        protected void btn_disassociaVoce_Click(object sender, EventArgs e)
        {
            if (lbx_vociAssociate.SelectedItem != null)
            {
                //Creo la voce di indice da dissociare
                ListItem itemSelezionato = lbx_vociAssociate.SelectedItem;
                SAAdminTool.DocsPaWR.VoceIndiceSistematico voceDaDissociare = new SAAdminTool.DocsPaWR.VoceIndiceSistematico();
                voceDaDissociare.systemId = itemSelezionato.Value;
                voceDaDissociare.idAmm = idAmministrazione;
                voceDaDissociare.voceIndice = itemSelezionato.Text;
                voceDaDissociare.idProject = nodoSelezionato.ID;

                //Dissocio la voce dal nodo di titolario selezioanto
                wws.dissociaVoceIndice(voceDaDissociare);

                //Ricarico le voci associate
                caricaVociAssociate();
            }
        }

        protected void caricaVociDisponibili()
        {
            ArrayList listaVoci = new ArrayList(wws.getIndiceByIdAmm(idAmministrazione));
            if (listaVoci != null)
            {
                lbx_vociDisponibili.Items.Clear();
                foreach (SAAdminTool.DocsPaWR.VoceIndiceSistematico voce in listaVoci)
                {
                    ListItem item = new ListItem(voce.voceIndice,voce.systemId);
                    lbx_vociDisponibili.Items.Add(item);
                }
            }
        }

        protected void caricaVociAssociate()
        {
            ArrayList listaVociAssociate = new ArrayList(wws.getIndiceByIdProject(nodoSelezionato.ID));
            if (listaVociAssociate != null)
            {
                lbx_vociAssociate.Items.Clear();
                foreach (SAAdminTool.DocsPaWR.VoceIndiceSistematico voce in listaVociAssociate)
                {
                    ListItem item = new ListItem(voce.voceIndice, voce.systemId);
                    lbx_vociAssociate.Items.Add(item);
                }
            }
        }
        
    }
}
