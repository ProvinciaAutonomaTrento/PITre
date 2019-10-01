using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class NoteSmistamento : System.Web.UI.Page
    {
        #region Session

        private string Type
        {
            get
            {
                if (HttpContext.Current.Session["typeURP"] != null)
                    return HttpContext.Current.Session["typeURP"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["typeURP"] = value;
            }
        }

        private string Id
        {
            get
            {
                if (HttpContext.Current.Session["idGrdUoApp"] != null)
                    return HttpContext.Current.Session["idGrdUoApp"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["idGrdUoApp"] = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            //this.GetCalendarControl("txt_dtaScadenza").txt_Data.Visible = true;
            //this.GetCalendarControl("txt_dtaScadenza").btn_Cal.Visible = true;

            //this.FillParametriQueryString();

            if (!this.IsPostBack)
            {
                this.InitializePage();

                string descrizione;
                DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = this.GetDatiAggiuntivi(out descrizione);

                // Gabriele Melini 29-10-2014
                // modifiche per gestire le UO senza ruolo di riferimento
                // per le quali non ci sono dati aggiuntivi

                if (datiAggiuntivi == null || (datiAggiuntivi != null && datiAggiuntivi.Length == 0))
                {
                    if(!this.Type.Equals("U"))
                        this.ClientScript.RegisterStartupScript(this.GetType(), "DatiAggiuntiviNonTrovati", @"alert('Dati aggiuntivi non trovati per l\'oggetto richiesto'); self.close();", true);
                }
                else
                {
                    if (datiAggiuntivi.Length == 1)
                        this.FetchDatiAggiuntivi(datiAggiuntivi[0]);
                }

                this.ImpostaVisibTipoTrasm();
                string textLabel = GetImage();
                this.lblCorr.Text = textLabel + " " + descrizione;

                
            }
            this.RefreshScript();
        }

        private void RefreshScript()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshSelect", "refreshSelect();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);

            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshprojectTxtDescrizione", "charsLeft('projectTxtDescrizione', '2000' , '" + this.projectLtrDescrizione.Text.Replace("'", "\'") + "');", true);
            //this.projectTxtDescrizione_chars.Attributes["rel"] = "projectTxtDescrizione_'2000'_" + this.projectLtrDescrizione.Text;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshtxtNoteInd", "charsLeft('txtNoteInd', '" + this.txtNoteInd.MaxLength + "' , '" + this.LtrNoteInd.Text.Replace("'", "\'") + "');", true);
            this.txtNoteInd_chars.Attributes["rel"] = "txtNoteInd_'" + this.txtNoteInd.MaxLength + "'_" + this.LtrNoteInd.Text;
        }

        private void InitializePage()
        {
            this.InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();

            this.LitNoteSmistamentoScadTitle.Text = Utils.Languages.GetLabelFromCode("LitNoteSmistamentoScadTitle", language);
            this.LitNoteSmistamentoScadTitle.Text = Utils.Languages.GetLabelFromCode("LitNoteSmistamentoScadTitle", language);
            this.LitNoteSmistamentoType.Text = Utils.Languages.GetLabelFromCode("LitNoteSmistamentoType", language);
            this.lblNoteInd.Text = Utils.Languages.GetLabelFromCode("NoteSmistamentoNoteInd", language);
            this.BtnSave.Text = Utils.Languages.GetLabelFromCode("GenericBtnSave", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.ddl_tipo.Items[0].Text = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeSingle", language);
            this.ddl_tipo.Items[1].Text = Utils.Languages.GetLabelFromCode("TransmissionDdlTypeMulti", language);
            this.LtrNoteInd.Text = Utils.Languages.GetLabelFromCode("DocumentLitVisibleNotesChars", language) + " ";
        }

        private void ImpostaVisibTipoTrasm()
        {
            this.pnl_tipoTrasm.Visible = false;
            this.pnl_tipoTrasm.Visible = (this.Type.Equals("U") || this.Type.Equals("R"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetImage()
        {
            string retValue = string.Empty;

            switch (this.Type)
            {
                case "U":
                    retValue = "uo_icon";
                    break;

                case "R":
                    retValue = "role2_icon";
                    break;

                case "P":
                    retValue = "user_icon";
                    break;
            }

            retValue = "<img src='../images/icons/" + retValue + ".png' border='0'>";

            return retValue;
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
                this.txt_dtaScadenza.Text = datiAggiuntivi.dtaScadenza;

                if (this.pnl_tipoTrasm.Visible)
                    this.ddl_tipo.SelectedIndex = this.ddl_tipo.Items.IndexOf(this.ddl_tipo.Items.FindByValue(datiAggiuntivi.tipoTrasm));
            }
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
                        if (utente.FlagCompetenza || utente.FlagConoscenza)
                        {
                            datiAggiuntivi = new datiAggiuntiviSmistamento[1] { utente.datiAggiuntiviSmistamento };
                            descrizione = utente.Denominazione;
                            break;
                        }
                    }
                }
            }

            return datiAggiuntivi;
        }

        protected void PerformActionSave(DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntiviCorrenti)
        {
            //Per rimozione note nei documenti
            Session.Add("datiAggiuntivi", datiAggiuntiviCorrenti);

            string note = this.txtNoteInd.Text.Trim();
            string dataScadenza = this.txt_dtaScadenza.Text;
            string msg = string.Empty;
            string tipoTrasm = string.Empty;

            if (this.pnl_tipoTrasm.Visible)
                tipoTrasm = this.ddl_tipo.Items[this.ddl_tipo.SelectedIndex].Value;

            if (!string.IsNullOrEmpty(note) && note.Length > 255)
                note = note.Substring(0, 254);

            foreach (DocsPaWR.datiAggiuntiviSmistamento datiAggiuntivi in datiAggiuntiviCorrenti)
            {
                datiAggiuntivi.NoteIndividuali = note;
                datiAggiuntivi.dtaScadenza = dataScadenza;
                datiAggiuntivi.tipoTrasm = tipoTrasm;
            }

            this.PerformActionClose();
        }

        protected void PerformActionClose()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('NoteSmistamento','up');", true);
        }

        /// <summary>
        /// Reperimento gestore dati di sessione dello smistamento
        /// </summary>
        /// <returns></returns>
        private SmistaDocManager GetSmistaDocManager()
        {
            return SmistaDocSessionManager.GetSmistaDocManager();
        }

        protected void ddl_tipo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            DocsPaWR.datiAggiuntiviSmistamento[] datiAggiuntivi = this.GetDatiAggiuntivi();
            this.PerformActionSave(datiAggiuntivi);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            this.PerformActionClose();
        }
    }
}