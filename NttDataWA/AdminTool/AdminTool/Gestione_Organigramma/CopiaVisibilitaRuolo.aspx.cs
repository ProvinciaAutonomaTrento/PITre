using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.utils;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;
namespace SAAdminTool.AdminTool.Gestione_Organigramma
{
    public partial class CopiaVisibilitaRuolo : System.Web.UI.Page
    {
        public string codRuoloOrigine;
        public string descRuoloOrigine;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            string idAmm = this.Request.QueryString["idAmm"].ToString();
            
            codRuoloOrigine = this.Request.QueryString["codRuolo"].ToString();
            descRuoloOrigine = this.Request.QueryString["descRuolo"].ToString();

            DocsPaWR.Utente ut = new SAAdminTool.DocsPaWR.Utente();
            ut.codiceAmm = codiceAmministrazione;
            ut.idAmministrazione = idAmm;
            ut.tipoIE = "I";
            Session.Add("userData", ut);

            lbl_ruoloOrigine.Text = codRuoloOrigine + " - " + descRuoloOrigine;

            setVisibilityRblEstendiVis();
			/* ABBATANGELI GIANLUIGI
             * Funzione statica che aggiunge alla pagina passata per parametro (this)
             * il codice necessario per mantenere viva la sessione. */
            utils.AlertPostLoad.KeepSessionAlive(this);
        }

        protected void txt_codRuoloDest_TextChanged(object sender, EventArgs e)
        {
                DocsPaWR.ElementoRubrica corrSearch = getElementoRubrica();

                if (corrSearch != null)
                {
                    if (!string.IsNullOrEmpty(codRuoloOrigine) && !codRuoloOrigine.ToUpper().Equals(txt_codRuoloDest.Text.Trim().ToUpper()))
                    {
                        txt_codRuoloDest.Text = corrSearch.codice;
                        txt_descRuoloDest.Text = corrSearch.descrizione;
                        
                    }
                    else
                    {
                        txt_codRuoloDest.Text = string.Empty;
                        txt_descRuoloDest.Text = string.Empty;
                        ClientScript.RegisterStartupScript(this.GetType(), "emptySearch_1", "alert('Il ruolo di origine e quello di destinazione coincidono !');", true);
                    }
                }
                else
                {
                    txt_codRuoloDest.Text = string.Empty;
                    txt_descRuoloDest.Text = string.Empty;
                    ClientScript.RegisterStartupScript(this.GetType(), "emptySearch_2", "alert('Nessun ruolo trovato con il codice inserito !');", true);
                }                            
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {
            DocsPaWR.CopyVisibility copyVisibility = getCopyVisibility();

            if (string.IsNullOrEmpty(copyVisibility.codRuoloDestinazione))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "emptySearch_3", "alert('Selezionare un ruolo di destinazione !');", true);
            }
            else
            {
                AmmUtils.WebServiceLink wsLink = new AmmUtils.WebServiceLink();

                SAAdminTool.AdminTool.Manager.SessionManager session = new SAAdminTool.AdminTool.Manager.SessionManager();
               InfoUtenteAmministratore iuA= session.getUserAmmSession();
               InfoUtente ut = new InfoUtente();
               ut.idPeople = iuA.idPeople;
               ut.idGruppo = (iuA.idGruppo == null ? "0" : iuA.idGruppo);
               ut.idCorrGlobali = (iuA.idCorrGlobali == null ? "0" : iuA.idCorrGlobali);
               ut.idAmministrazione = iuA.idAmministrazione;
               ut.sede = iuA.sede;
               ut.userId = iuA.userId;
              

                DocsPaWR.EsitoOperazione esitoOperazione = wsLink.CopyVisibility(ut, copyVisibility);
                // ABBATANGELI GIANLUIGI
                if (esitoOperazione.Codice == 0) {
                    //ABBATANGELI GIANLUIGI
                    PrintReportRequest request = new PrintReportRequest
                    {
                        ContextName = "CopiaVisibilita",
                        SearchFilters = null,
                        UserInfo = ut,
                        Title = "Report copia visibilità"
                    };


                    ReportingUtils.PrintRequest = request;
                    if(esitoOperazione.Descrizione.Contains('*'))
                        esitoOperazione.Descrizione = esitoOperazione.Descrizione.Split('*')[0];

                    ClientScript.RegisterStartupScript(this.GetType(), "esitoOperazione", "if (confirm('" + esitoOperazione.Descrizione + "\\n\\rEsportare il risultato?')) {" + ReportScript + "}", true);
                }
                else {
                    ClientScript.RegisterStartupScript(this.GetType(), "esitoOperazione", "alert('"+esitoOperazione.Descrizione+"');", true);
                }
            }          
        }

		// ABBATANGELI GIANLUIGI
        /// <summary>
        /// Script per l'apertura della pagina di export report
        /// </summary>
        public static String ReportScript
        {
            get
            {
                return ReportingUtils.GetOpenReportPageScript(false);
            }
        }
		
        private DocsPaWR.CopyVisibility getCopyVisibility()
        {
            DocsPaWR.CopyVisibility copyVisibility = new DocsPaWR.CopyVisibility();

            //Ruolo Origine
            copyVisibility.codRuoloOrigine = this.Request.QueryString["codRuolo"].ToString();
            copyVisibility.descRuoloOrigine = this.Request.QueryString["descRuolo"].ToString();
            copyVisibility.idGruppoRuoloOrigine = this.Request.QueryString["idGruppo"].ToString();
            copyVisibility.idCorrGlobRuoloOrigine = this.Request.QueryString["idCorrGlobRuolo"].ToString();

            //Ruolo Destinazione
            DocsPaWR.ElementoRubrica corrSearch = getElementoRubrica();
            if (corrSearch != null)
            {
                DocsPaWR.Corrispondente corrDest = UserManager.getCorrispondenteBySystemID(this, corrSearch.systemId);
                copyVisibility.codRuoloDestinazione = corrDest.codiceRubrica;
                copyVisibility.descRuoloDestinazione = corrDest.descrizione;
                copyVisibility.idGruppoRuoloDestinazione = ((DocsPaWR.Ruolo)corrDest).idGruppo;
                copyVisibility.idCorrGlobRuoloDestinazione = corrDest.systemId;
            }           

            //Criteri di copia
            copyVisibility.idAmm = this.Request.QueryString["idAmm"].ToString();
            copyVisibility.docProtocollati = cbx_docProtocollati.Checked;
            copyVisibility.docNonProtocollati = cbx_docNonProtocollati.Checked;
            copyVisibility.fascicoliProcedimentali = cbx_fascicoliProcedimentali.Checked;
            copyVisibility.visibilitaAttiva = cbx_visibilitaAttiva.Checked;
            copyVisibility.precedenteCopiaVisibilita = cbx_precCopiaVisibilita.Checked;
            copyVisibility.estendiVisibilita = rbl_estendiVisibilita.SelectedValue;

            return copyVisibility;            
        }

        private DocsPaWR.ElementoRubrica getElementoRubrica()
        {
            DocsPaWR.ElementoRubrica elRubrica = null;

            if (!String.IsNullOrEmpty(this.txt_codRuoloDest.Text.Trim()))
            {
                DocsPaWR.ParametriRicercaRubrica qco = new DocsPaWR.ParametriRicercaRubrica();
                UserManager.setQueryRubricaCaller(ref qco);
                qco.codice = txt_codRuoloDest.Text.Trim();
                qco.tipoIE = SAAdminTool.DocsPaWR.AddressbookTipoUtente.INTERNO;
                qco.calltype = DocsPaWR.RubricaCallType.CALLTYPE_TUTTI_RUOLI;
                qco.doListe = false;
                qco.doRuoli = true;
                qco.doUo = false;
                qco.doUo = false;
                qco.queryCodiceEsatta = true;

                DocsPaWR.ElementoRubrica[] corrSearch = UserManager.getElementiRubrica(this.Page, qco);

                if (corrSearch != null && corrSearch.Length == 1 && corrSearch[0].tipo.ToUpper().Equals("R"))
                {
                    elRubrica = corrSearch[0];
                }
            }

            return elRubrica;
        }

        private void setVisibilityRblEstendiVis()
        {
            if (!Utils.GetAbilitazioneAtipicita())
            {
                foreach (ListItem li in rbl_estendiVisibilita.Items)
                {
                    if (li.Text == "Escludi i documenti / fascicoli atipici")
                    {
                        li.Attributes.Add("style", "display:none");
                    }
                }
            }
        }
       
    }
}