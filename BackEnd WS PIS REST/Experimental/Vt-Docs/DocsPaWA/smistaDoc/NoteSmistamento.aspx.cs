using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.smistaDoc
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NoteSmistamento : System.Web.UI.Page
    {   
        /// <summary>
        /// 
        /// </summary>
        private string Id { get; set; } 
        
        /// <summary>
        /// 
        /// </summary>
        private string Tipo { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        private string WithWorkFlow { get; set; } //true o false a seconda del tipo della ragione di trasmissione selezionata
        
        /// <summary>
        /// 
        /// </summary>
        private bool EnableDtaScadenza { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            this.GetCalendarControl("txt_dtaScadenza").txt_Data.Visible = true;
            this.GetCalendarControl("txt_dtaScadenza").btn_Cal.Visible = true;

            this.FillParametriQueryString();
            
            if (!IsPostBack)
            {
                string descrizione;
                DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = this.GetDatiAggiuntivi(out descrizione);

                if (datiAggiuntivi == null || (datiAggiuntivi != null && datiAggiuntivi.Length == 0))
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "DatiAggiuntiviNonTrovati", @"alert('Dati aggiuntivi non trovati per l\'oggetto richiesto'); self.close();", true);
                }
                else
                {
                    this.ImpostaVisibTipoTrasm();

                    if (this.WithWorkFlow.ToUpper().Equals("TRUE"))
                        this.EnableDtaScadenza = true;

                    if (datiAggiuntivi.Length == 1)
                        this.FetchDatiAggiuntivi(datiAggiuntivi[0]);

                    string textLabel = GetImage();

                    this.lblCorr.Text = textLabel + " " + descrizione;

                    //il campo data è abilitato o meno a seconda del tipo della ragione di trasmissione:
                    //Solamente se la ragione di trasmissione prevede workFlow è abilitato.
                    this.GetCalendarControl("txt_dtaScadenza").txt_Data.Enabled = this.EnableDtaScadenza;
                    this.GetCalendarControl("txt_dtaScadenza").btn_Cal.Visible = this.EnableDtaScadenza;

                    if (!this.GetCalendarControl("txt_dtaScadenza").txt_Data.Enabled)
                    {
                        this.GetCalendarControl("txt_dtaScadenza").txt_Data.Text = "";
                    }
                }
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
        /// Caricamento parametri query string
        /// </summary>
        private void FillParametriQueryString()
        {
            this.Id = Request.QueryString["id"];
            this.Tipo = Request.QueryString["tipo"];
            this.WithWorkFlow = Request.QueryString["enableDtaScadenza"].ToString();
        }

        /// <summary>
        /// Caricamento dati aggiuntivi nei campi della user interface
        /// </summary>
        /// <param name="datiAggiuntivi"></param>
        protected void FetchDatiAggiuntivi(DocsPaWR.datiAggiuntiviSmistamento datiAggiuntivi)
        {
            if (datiAggiuntivi != null)
            {
                this.txtNoteInd.Text = datiAggiuntivi.NoteIndividuali;
                this.GetCalendarControl("txt_dtaScadenza").txt_Data.Text = datiAggiuntivi.dtaScadenza;

                if (this.pnl_tipoTrasm.Visible)
                    this.ddl_tipo.SelectedIndex = this.ddl_tipo.Items.IndexOf(this.ddl_tipo.Items.FindByValue(datiAggiuntivi.tipoTrasm));
            }
        }

        /// <summary>
        /// Reperimento gestore dati di sessione dello smistamento
        /// </summary>
        /// <returns></returns>
        private SmistaDocManager GetSmistaDocManager()
        {
            return SmistaDocSessionManager.GetSmistaDocManager();
        }

        /// 
        /// </summary>
        /// <param name="controlId"></param>
        /// <returns></returns>
        private DocsPAWA.UserControls.Calendar GetCalendarControl(string controlId)
        {
            return (DocsPAWA.UserControls.Calendar)this.FindControl(controlId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetImage()
        {
            string retValue = string.Empty;
       
            switch (this.Tipo)
            {
                case "U":
                    retValue = "uo_noexp";
                    break;

                case "R":
                    retValue = "ruolo_noexp";
                    break;

                case "P":
                    retValue = "utente_noexp"; 
                    break;
            }

            retValue = "<img src='../images/smistamento/" + retValue + ".gif' border='0'>";

            return retValue;
        }

        /// <summary>
        /// Reperimento dati aggiuntivi smistamento
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.datiAggiuntiviSmistamento[] GetDatiAggiuntivi()
        {
            string descrizione;
            return this.GetDatiAggiuntivi(out descrizione);
        }

        /// <summary>
        /// Reperimento dati aggiuntivi smistamento
        /// </summary>
        /// <param name="descrizione"></param>
        /// <returns></returns>
        protected DocsPaWR.datiAggiuntiviSmistamento[] GetDatiAggiuntivi(out string descrizione)
        {
            descrizione = string.Empty;
            SmistaDocManager manager = this.GetSmistaDocManager();

            datiAggiuntiviSmistamento[] datiAggiuntivi = this.GetDatiAggiuntivi(this.Id, out descrizione, manager.GetUOAppartenenza());

            if (datiAggiuntivi == null)
                datiAggiuntivi = this.GetDatiAggiuntivi(this.Id, out descrizione, manager.GetUOInferiori());

            return datiAggiuntivi;
        }

        /// <summary>
        /// </summary>
        /// <param name="uoSmistamento"></param>
        /// <param name="descrizione"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected DocsPaWR.datiAggiuntiviSmistamento[] GetDatiAggiuntivi(string id, out string descrizione, params DocsPaWR.UOSmistamento[] uoSmistamento)
        {
            descrizione = string.Empty;
            DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = null;

            if (uoSmistamento != null)
            {
                foreach (DocsPaWR.UOSmistamento uo in uoSmistamento)
                {
                    if (uo.ID == id)
                    {
                        descrizione = uo.Descrizione;

                        // L'id fa riferimento ad una UO, le quali non dispongono dei dati aggiuntivi.
                        // In tal caso, si ricercano i dati aggiuntivi di tutti i ruoli di riferimento della stessa.
                        DocsPaWR.RuoloSmistamento[] ruoliRiferimento = uo.Ruoli.Where(e => e.RuoloRiferimento).ToArray();

                        datiAggiuntivi = new datiAggiuntiviSmistamento[ruoliRiferimento.Length];
                        for (int i = 0; i < ruoliRiferimento.Length; i++)
                            datiAggiuntivi[i] = ruoliRiferimento[i].datiAggiuntiviSmistamento;

                        break;
                    }
                    else
                    {
                        foreach (DocsPaWR.RuoloSmistamento ruolo in uo.Ruoli)
                        {
                            datiAggiuntivi = this.GetDatiAggiuntivi(id, ruolo, out descrizione);

                            if (datiAggiuntivi != null)
                                break;
                        }

                        if (datiAggiuntivi == null)
                        {
                            datiAggiuntivi = this.GetDatiAggiuntivi(id, out descrizione, uo.UoInferiori);

                            if (datiAggiuntivi != null)
                                break;
                        }

                        if (datiAggiuntivi != null)
                            break;
                    }
                }
            }

            return datiAggiuntivi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ruoloSmistamento"></param>
        /// <param name="descrizione"></param>
        /// <returns></returns>
        protected DocsPaWR.datiAggiuntiviSmistamento[] GetDatiAggiuntivi(string id, DocsPaWR.RuoloSmistamento ruoloSmistamento, out string descrizione)
        {
            descrizione = string.Empty;
            DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = null;

            if (ruoloSmistamento.ID == id)
            {
                datiAggiuntivi = new datiAggiuntiviSmistamento[1] { ruoloSmistamento.datiAggiuntiviSmistamento };
                descrizione = ruoloSmistamento.Descrizione;
            }
            else
            {
                foreach (DocsPaWR.UtenteSmistamento utente in ruoloSmistamento.Utenti)
                {
                    if (utente.ID == id)
                    {
                        datiAggiuntivi = new datiAggiuntiviSmistamento[1] { utente.datiAggiuntiviSmistamento };
                        descrizione = utente.Denominazione;
                        break;
                    }
                }
            }

            return datiAggiuntivi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = this.GetDatiAggiuntivi();

            this.PerformActionSave(datiAggiuntivi);
        }

        /// <summary>
        /// Metodo per la validazione dei dati immessi
        /// </summary>
        /// <param name="dataScadenza"></param>
        /// <returns></returns>
        private string CheckFields(string dataScadenza)
        {
            string errMessage = string.Empty;

            //controllo validazione sulla data di scadenza
            if (dataScadenza.Length > 0)
            {
                if (!Utils.isDate(dataScadenza))
                {
                    errMessage = "Errore nel formato della data di scadenza. \\nIl formato richiesto è gg/mm/aaaa";
                    
                    return errMessage;
                }
            }

            return errMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datiAggiuntiviCorrenti"></param>
        protected void PerformActionSave(DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntiviCorrenti)
        {
            //Per rimozione note nei documenti
            Session.Add("datiAggiuntivi", datiAggiuntiviCorrenti);

            string note = this.txtNoteInd.Text.Trim();
            string dataScadenza = this.GetCalendarControl("txt_dtaScadenza").txt_Data.Text;
            string msg = string.Empty;
            string tipoTrasm = string.Empty;

            if (this.pnl_tipoTrasm.Visible)
                tipoTrasm = this.ddl_tipo.Items[this.ddl_tipo.SelectedIndex].Value;

            if (!string.IsNullOrEmpty(note) && note.Length > 255)
                note = note.Substring(0, 254);

            msg = this.CheckFields(dataScadenza);

            if (string.IsNullOrEmpty(msg))
            {
                foreach (DocsPaWR.datiAggiuntiviSmistamento datiAggiuntivi in datiAggiuntiviCorrenti)
                {
                    datiAggiuntivi.NoteIndividuali = note;
                    datiAggiuntivi.dtaScadenza = dataScadenza;
                    datiAggiuntivi.tipoTrasm = tipoTrasm;
                }

                this.PerformActionClose();
            }
            else
            {
                string script = "<SCRIPT language='javascript'>alert('" + msg + "');document.getElementById('" + this.GetCalendarControl("txt_dtaScadenza").txt_Data.ID + "').focus(); </SCRIPT>";
                this.ClientScript.RegisterStartupScript(this.GetType(), "focus", script);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void PerformActionClose()
        {
            Response.Write("<script>window.close();</script>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnClose_Click(object sender, EventArgs e)
        {
            this.PerformActionClose();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ImpostaVisibTipoTrasm()
        {
            this.pnl_tipoTrasm.Visible = false;
            this.pnl_tipoTrasm.Visible = (this.Tipo.Equals("U") || this.Tipo.Equals("R"));
        }
    }
}
