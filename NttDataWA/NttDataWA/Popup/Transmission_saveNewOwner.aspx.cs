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
    public partial class Transmission_saveNewOwner : System.Web.UI.Page
    {

        #region Fields

        public int maxLength = 2000;

        #endregion

        #region Properties

        private DocsPaWR.Trasmissione Transmission
        {
            get
            {
                DocsPaWR.Trasmissione result = null;
                if (HttpContext.Current.Session["Transmission_dataentry"] != null)
                {
                    result = HttpContext.Current.Session["Transmission_dataentry"] as DocsPaWR.Trasmissione;
                }
                else if (this.FromMassiveAct)
                {
                    result = TrasmManager.getGestioneTrasmissione(this);
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_dataentry"] = value;
            }
        }

        private bool SaveButNotTransmit
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["SaveButNotTransmit"] != null)
                {
                    result = (bool)HttpContext.Current.Session["SaveButNotTransmit"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["SaveButNotTransmit"] = value;
            }
        }

        private bool FromMassiveAct
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["fromMassiveAct"] != null)
                {
                    result = (bool)HttpContext.Current.Session["fromMassiveAct"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["fromMassiveAct"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();

                    if (!this.SaveButNotTransmit) this.TransmissionBtnSave.Text = "Trasmetti";


                    foreach (TrasmissioneSingola ts in this.Transmission.trasmissioniSingole)
                    {
                        Type tipoCI = ts.corrispondenteInterno.GetType();
                        string idGruppo = "";

                        if (tipoCI == typeof(DocsPaWR.Utente))
                            idGruppo = ((DocsPaWR.Utente)(ts.corrispondenteInterno)).idPeople;
                        if (tipoCI == typeof(DocsPaWR.Ruolo))
                            idGruppo = ((DocsPaWR.Ruolo)(ts.corrispondenteInterno)).idGruppo;

                        foreach (TrasmissioneUtente tru in ts.trasmissioneUtente)
                        {
                            ListItem item = new ListItem();
                            item.Text = tru.utente.descrizione;
                            item.Value = tru.utente.idPeople + "_" + idGruppo;
                            this.rblUsers.Items.Add(item);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.TransmissionBtnSave.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnSave", language);
            this.TransmissionBtnClose.Text = Utils.Languages.GetLabelFromCode("TransmissionsBtnClose", language);
            this.RequiredFieldValidator2.ErrorMessage = "<br />"+Utils.Languages.GetLabelFromCode("TransmissionReqUser", language);
            this.TransmissionTransferRightsDescription.Text = Utils.Languages.GetLabelFromCode("TransmissionTransferRightsDescription", language);
        }

        protected void TransmissionBtnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            //try {
                if (this.IsValid)
                {
                    if (this.SaveTransmission())
                    {
                        this.CloseMask();
                    }
                    else
                    {
                        RenderMessage("<strong>Errore nel salvataggio della trasmissione</strong>");
                    }
                }
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        private DocsPaWR.Trasmissione impostaUtenteConNotifica(DocsPaWR.Trasmissione trasm)
        {
            foreach (DocsPaWR.TrasmissioneSingola trasmS in trasm.trasmissioniSingole)
            {
                foreach (DocsPaWR.TrasmissioneUtente trasmU in trasmS.trasmissioneUtente)
                {
                    if (trasmU.utente.idPeople == this.rblUsers.SelectedValue.Split('_')[0])
                        trasmU.daNotificare = true;
                    else
                        trasmU.daNotificare = false;
                }
            }

            return trasm;
        }

        private bool SaveTransmission()
        {
            try
            {
                if (this.Transmission.utente!=null)
                    this.Transmission.utente.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                if (DocumentManager.getSelectedRecord()!=null)
                    this.Transmission.infoDocumento = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());
                else if (ProjectManager.getProjectInSession()!=null)
                    this.Transmission.infoFascicolo = ProjectManager.getInfoFascicoloDaFascicolo(ProjectManager.getProjectInSession());

                DocsPaWR.CessioneDocumento objCessione = new DocsPaWR.CessioneDocumento();
                objCessione.docCeduto = true;
                objCessione.idPeople = UserManager.GetInfoUser().idPeople;
                objCessione.idRuolo = UserManager.GetInfoUser().idGruppo;
                objCessione.idPeopleNewPropr = this.rblUsers.SelectedValue.Split('_')[0];
                objCessione.idRuoloNewPropr = this.rblUsers.SelectedValue.Split('_')[1];
                objCessione.userId = UserManager.GetInfoUser().userId;
                this.Transmission.cessione = objCessione;
                this.Transmission = impostaUtenteConNotifica(this.Transmission);

                if (this.SaveButNotTransmit)
                {
                    this.Transmission = TrasmManager.saveTrasm(this, this.Transmission);
                }
                else
                {
                    InfoUtente infoUser = UserManager.GetInfoUser();
                    this.Transmission = TrasmManager.saveExecuteTrasm(this, this.Transmission, infoUser);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void TransmissionBtnClose_Click(object sender, EventArgs e)
        {
            //try {
                this.CloseMask();
            //}
            //catch (System.Exception ex)
            //{
            //    UIManager.AdministrationManager.DiagnosticError(ex);
            //    return;
            //}
        }

        /// <summary>
        /// Close Mask
        /// </summary>
        /// <param name="versionId"></param>
        protected void CloseMask()
        {
            if (this.FromMassiveAct)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('TransmitNewOwner', '', parent);", true);
            else
            {
                if (this.SaveButNotTransmit)
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('SaveNewOwner');", true);
                else
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('TransmitNewOwner');", true);
            }
        }

        /// <summary>
        /// Render Message
        /// </summary>
        /// <param name="message"></param>
        protected virtual void RenderMessage(string message)
        {
            rowMessage.InnerHtml = message;
            rowMessage.Visible = true;
        }

    }
}