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
    public partial class TransmissionTemplate_saveNewOwner : System.Web.UI.Page
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
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_dataentry"] = value;
            }
        }

        private DocsPaWR.ModelloTrasmissione Template
        {
            get
            {
                DocsPaWR.ModelloTrasmissione result = null;
                if (HttpContext.Current.Session["Transmission_template"] != null)
                {
                    result = HttpContext.Current.Session["Transmission_template"] as DocsPaWR.ModelloTrasmissione;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Transmission_template"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.rblShare.Items[0].Text = this.rblShare.Items[0].Text.Replace("@usr@", UserManager.GetUserInSession().descrizione);
                    this.rblShare.Items[1].Text = this.rblShare.Items[1].Text.Replace("@grp@", UserManager.GetSelectedRole().descrizione);


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

        protected void TemplateBtnSave_Click(object sender, EventArgs e)
        {
            try {
                if (this.IsValid)
                {
                    if (this.SaveTemplate())
                    {
                        this.CloseMask();
                    }
                    else
                    {
                        RenderMessage("Errore nella creazione del modello");
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
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

        private bool SaveTemplate()
        {
            try
            {
                this.Transmission.cessione = new CessioneDocumento();
                this.Transmission.cessione.idPeopleNewPropr = this.rblUsers.SelectedValue.Split('_')[0];
                this.Transmission.cessione.idRuoloNewPropr = this.rblUsers.SelectedValue.Split('_')[1];
                this.Transmission = impostaUtenteConNotifica(this.Transmission);
                this.Transmission.daAggiornare = true;


                this.Template.NOME = this.txtTitle.Text;
                if (this.rblShare.Items[0].Selected)
                {
                    for (int k = 0; k < this.Template.MITTENTE.Length; k++)
                    {
                        this.Template.MITTENTE[k].ID_CORR_GLOBALI = 0;
                    }
                }
                else
                {
                    this.Template.ID_PEOPLE = "";
                }

                this.Template.CEDE_DIRITTI = "1";
                this.Template.ID_PEOPLE_NEW_OWNER = this.rblUsers.SelectedValue.Split('_')[0];
                this.Template.ID_GROUP_NEW_OWNER = this.rblUsers.SelectedValue.Split('_')[1];

                if (this.Transmission.mantieniScrittura)
                {
                    this.Template.MANTIENI_LETTURA = "1";
                    this.Template.MANTIENI_SCRITTURA = "1";
                }
                else
                {
                    this.Template.MANTIENI_SCRITTURA = "0";
                    if (this.Transmission.mantieniLettura)
                        this.Template.MANTIENI_LETTURA = "1";
                    else
                        this.Template.MANTIENI_LETTURA = "0";
                }

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
                this.Template.CODICE = "MT_" + TransmissionModelsManager.GetTemplateSystemId();
                TransmissionModelsManager.SaveTemplate(this.Template, infoUtente);

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void TemplateBtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Close Mask
        /// </summary>
        /// <param name="versionId"></param>
        protected void CloseMask()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('TemplateSaveNewOwner');} else {parent.closeAjaxModal('TemplateSaveNewOwner');}", true);
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