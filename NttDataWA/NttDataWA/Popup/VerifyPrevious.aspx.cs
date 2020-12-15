using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class VerifyPrevious : System.Web.UI.Page
    {

        #region Properties

        private SchedaDocumento DocumentInWorking
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["document"] != null)
                {
                    result = HttpContext.Current.Session["document"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["document"] = value;
            }
        }

        private DocsPaWR.InfoProtocolloDuplicato[] datiProtDupl
        {
            get
            {
                return Session["VerifyPrevious_datiProtDupl"] as DocsPaWR.InfoProtocolloDuplicato[];
            }
            set
            {
                Session["VerifyPrevious_datiProtDupl"] = value;
            }
        }

        private DocsPaWR.EsitoRicercaDuplicatiEnum esitoRicercaDuplicati
        {
            get
            {
                return (DocsPaWR.EsitoRicercaDuplicatiEnum)Session["VerifyPrevious_esitoRicercaDuplicati"];
            }
            set
            {
                Session["VerifyPrevious_esitoRicercaDuplicati"] = value;
            }
        }

        //private int transmissionsModel
        //{
        //    get
        //    {
        //        return (int)Session["VerifyPrevious_trasmModel"];
        //    }
        //    set
        //    {
        //        Session["VerifyPrevious_trasmModel"] = value;
        //    }
        //}

        private string idProfile
        {
            get
            {
                return HttpContext.Current.Session["Visibility_idProfile"] as string;
            }
            set
            {
                HttpContext.Current.Session["Visibility_idProfile"] = value;
            }
        }

        protected SchedaDocumento documentWIP
        {
            get
            {
                return HttpContext.Current.Session["Visibility_document"] as SchedaDocumento;
            }
            set
            {
                HttpContext.Current.Session["Visibility_document"] = value;
            }
        }

        protected SchedaDocumento documentPrevious
        {
            get
            {
                return HttpContext.Current.Session["Visibility_document2"] as SchedaDocumento;
            }
            set
            {
                HttpContext.Current.Session["Visibility_document2"] = value;
            }
        }

        protected FileRequest filePrevious
        {
            get
            {
                return HttpContext.Current.Session["Visibility_fileRequest"] as FileRequest;
            }
            set
            {
                HttpContext.Current.Session["Visibility_fileRequest"] = value;
            }
        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitPage();

                    if (this.datiProtDupl != null)
                    {
                        this.lblData.Text = this.datiProtDupl[0].dataProtocollo;
                        this.txtSegnatura.Text = this.datiProtDupl[0].segnaturaProtocollo;
                        this.txtOggetto.Text = this.datiProtDupl[0].uoProtocollatore;
                        this.lblIdDocumento.Text = this.datiProtDupl[0].idProfile;
                        this.lblNumProtocollo.Text = this.datiProtDupl[0].numProto;
                        this.idProfile = this.datiProtDupl[0].idProfile;

                        if (!DocumentManager.CheckDocumentUserVisibility(this, this.idProfile, UserManager.GetInfoUser()))
                        {
                            this.btn_VisDoc.Enabled = false;                        
                        }
                        else
                        {
                            DocumentManager.setSelectedRecord(DocumentManager.getDocumentDetails(this, this.datiProtDupl[0].idProfile, this.datiProtDupl[0].idProfile));
                            FileManager.setSelectedFile(DocumentManager.getSelectedRecord().documenti[0]);

                            if (string.IsNullOrEmpty(FileManager.GetFileRequest().fileName))
                                this.btn_VisDoc.Enabled = false;                           
                        }

                        if (this.datiProtDupl[0].docAcquisito == "1")
                            this.btn_VisDoc.Visible = true;
                    }

                    

                    switch (esitoRicercaDuplicati)
                    {
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.ProtocolloNullo:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgProtocolloNullo");
                            break;
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.NoProtocolloIngresso:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgNoProtocolloIngresso");
                            break;
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.NoMittente:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgNoMittente");
                            break;
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteProtocollo:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteProtocollo");
                            break;
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteData:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteData");
                            break;
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteOggetto:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteOggetto");
                            break;
                        case DocsPaWR.EsitoRicercaDuplicatiEnum.ErroreGenerico:
                            this.messager.Text = this.GetLabel("VerifyPreviousMsgErroreGenerico");
                            break;
                    }


                    if (Session["VerifyPrevious_RecordOk"] != null && Session["VerifyPrevious_RecordOk"].ToString().ToUpper().Equals("FALSE"))
                    {
                        this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteProtocolloContinue");

                        switch (esitoRicercaDuplicati)
                        {
                            case DocsPaWR.EsitoRicercaDuplicatiEnum.NoMittente:
                                this.messager.Text = this.GetLabel("VerifyPreviousMsgNoMittenteProto");
                                break;
                            case DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteProtocollo:
                                this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteProtocolloProto");
                                break;
                            case DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteData:
                                this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteData");
                                break;
                            case DocsPaWR.EsitoRicercaDuplicatiEnum.DuplicatiMittenteOggetto:
                                this.messager.Text = this.GetLabel("VerifyPreviousMsgDuplicatiMittenteOggetto");
                                break;
                        }


                        this.BtnOk.Visible = true;
                    }
                    else
                    {
                        this.BtnOk.Visible = false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitPage()
        {
            this.InitLanguage();
            this.documentPrevious = DocumentManager.getSelectedRecord();
            this.filePrevious = FileManager.getSelectedFile();
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("GenericBtnOk", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("VerifyPreviousBtnClose", language);
            this.litNumProt.Text = Utils.Languages.GetLabelFromCode("VerifyPreviousNumProt", language);
            this.litDate.Text = Utils.Languages.GetLabelFromCode("VerifyPreviousDate", language);
            this.litID.Text = Utils.Languages.GetLabelFromCode("VerifyPreviousID", language);
            this.litSegnature.Text = Utils.Languages.GetLabelFromCode("VerifyPreviousSegnature", language);
            this.litOffice.Text = Utils.Languages.GetLabelFromCode("VerifyPreviousOffice", language);
            this.btnVisibility.ToolTip = Utils.Languages.GetLabelFromCode("VerifyVisibility", language);
            this.btn_VisDoc.ToolTip = Utils.Languages.GetLabelFromCode("VerifyVisDoc", language);
        }

        private string GetLabel(string labelId)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(labelId, language);
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            this.CloseMask(true);
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            this.CloseMask(false);
        }

        protected void CloseMask(bool withReturnValue)
        {
            this.ClearSessionData();

            string retValue = string.Empty;
            if (withReturnValue)
            {
                retValue = "up";
            }
            else
            {
                retValue = string.Empty;
            }

            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('VerifyPrevious', '" + retValue + "');</script></body></html>");
            Response.End();
        }

        private void ClearSessionData()
        {
            // Gabriele Melini 19-06-2014
            // pezza per pulire la sessione in caso di page_load ripetuti
            // (problema firefox)
            Session["VerifyPrevious_datiProtDupl"] = null;


            Session["VerifyPrevious_RecordOk"] = null;
            this.idProfile = null;
            this.documentWIP = null;

            DocumentManager.setSelectedRecord(this.documentPrevious);
            this.documentPrevious = null;

            FileManager.setSelectedFile(this.filePrevious);
            this.filePrevious = null;
        }

        protected void btnVisibility_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupVisibility", "parent.ajaxModalPopupVisibility();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void btn_VisDoc_Click(object sender, EventArgs e)
        {
            try {
                Session["IsPreviousVersion"] = true;
                Session["isZoom"] = true;
                DocumentViewer.OpenDocumentViewer = true;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ajaxModalPopupVerifyPreviousViewer", "parent.ajaxModalPopupVerifyPreviousViewer();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}
