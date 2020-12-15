using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDatalLibrary;
using System.Data;
namespace NttDataWA.Popup
{
    public partial class SignatureSelectedItems : System.Web.UI.Page
    {
        #region Properties

        public static string componentType = Constans.TYPE_ACTIVEX;

        protected Dictionary<String, String> ListCheck
        {
            get
            {
                Dictionary<String, String> result = null;
                if (HttpContext.Current.Session["listCheck"] != null)
                {
                    result = HttpContext.Current.Session["listCheck"] as Dictionary<String, String>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listCheck"] = value;
            }
        }

		protected Dictionary<String, FileToSign> ListToSign
        {
            get
            {
                Dictionary<String, FileToSign> result = null;
                if (HttpContext.Current.Session["listToSign"] != null)
                {
                    result = HttpContext.Current.Session["listToSign"] as Dictionary<String, FileToSign>;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listToSign"] = value;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiLibroFirma
        {
            get
            {
                if (HttpContext.Current.Session["ListaElementiLibroFirma"] != null)
                    return (List<ElementoInLibroFirma>)HttpContext.Current.Session["ListaElementiLibroFirma"];
                else
                    return null;
            }
        }

        private MassiveOperationReport ReportSignatureSelected
        {
            get
            {
                if (HttpContext.Current.Session["ReportSignatureSelected"] != null)
                    return (MassiveOperationReport)HttpContext.Current.Session["ReportSignatureSelected"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["ReportSignatureSelected"] = value;
            }
        }

        private List<ElementoInLibroFirma> ListaElementiFiltrati
        {
            get
            {
                List<ElementoInLibroFirma> result = null;
                if (HttpContext.Current.Session["ListaElementiFiltrati"] != null)
                {
                    result = HttpContext.Current.Session["ListaElementiFiltrati"] as List<ElementoInLibroFirma>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListaElementiFiltrati"] = value;
            }
        }

        private string tipoSupportoFirma
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["LibroFirma_TipoFirma"] != null)
                {
                    result = HttpContext.Current.Session["LibroFirma_TipoFirma"].ToString();
                }
                return result;
            }
        }

        private List<string> ListIdDocumentSigned
        {
            get
            {
                List<string> result = null;
                if (HttpContext.Current.Session["ListIdDocumentSigned"] != null)
                {
                    result = HttpContext.Current.Session["ListIdDocumentSigned"] as List<string>;
                }
                return result;
            }

            set
            {
                HttpContext.Current.Session["ListIdDocumentSigned"] = value;
            }
        }

        private bool AddElementiInADL
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["AddElementiInADL"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["AddElementiInADL"].ToString());
                }
                return result;
            }
        }
        #endregion

        #region Standard method
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.InitializeLanguage();
                this.InitializePage();
            }
            else
            {
                ReadRetValueFromPopup();
            }
        }


        private void InitializePage()
        {
            GetMessaggeConfirm();
			this.ListToSign = new Dictionary<string, FileToSign>();
            HttpContext.Current.Session["massiveSignReport"] = null;
            NttDataWA.SmartClient.FirmaDigitaleResultManager.ClearData();
            this.ListIdDocumentSigned = null;
        }

        private void InitializeLanguage()
        {
            string language = UserManager.GetUserLanguage();
            this.SignatureSelectedItemsConfirm.Text = Utils.Languages.GetLabelFromCode("SignatureSelectedItemsConfirm", language);
            this.SignatureSelectedItemsCancel.Text = Utils.Languages.GetLabelFromCode("SignatureSelectedItemsCancel", language);
            this.MassiveDigitalSignature.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("MassiveAddAdlUserBtnReport", language);
            this.MassiveDigitalSignatureApplet.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveDigitalSignatureSocket.Title = Utils.Languages.GetLabelFromCode("SearchDocumentMassiveDigitalSignatureTitle", language);
            this.MassiveSignatureHSM.Title = Utils.Languages.GetLabelFromCode("MassiveSignatureHSMTitle", language);
        }

        private void ReadRetValueFromPopup()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            if (!string.IsNullOrEmpty(this.MassiveDigitalSignature.ReturnValue))
            {
                MassiveOperationReport report = HttpContext.Current.Session["massiveSignReport"] as MassiveOperationReport;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveDigitalSignature','');", true);
                HttpContext.Current.Session["CommandType"] = null;
                HttpContext.Current.Session.Remove("massiveSignReport");
                this.UpdateReportSignature(report);
                this.DigitalSignatureSelectedItem();
                return;
            }

            if (!string.IsNullOrEmpty(this.MassiveDigitalSignatureApplet.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveDigitalSignatureApplet','');", true);
                MassiveOperationReport report = HttpContext.Current.Session["massiveSignReport"] as MassiveOperationReport;
                HttpContext.Current.Session["CommandType"] = null;
                HttpContext.Current.Session.Remove("massiveSignReport");
                this.UpdateReportSignature(report);
                this.DigitalSignatureSelectedItem();
                return;
            }

            if (!string.IsNullOrEmpty(this.MassiveDigitalSignatureSocket.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveDigitalSignatureSocket','');", true);
                MassiveOperationReport report = HttpContext.Current.Session["massiveSignReport"] as MassiveOperationReport;
                HttpContext.Current.Session["CommandType"] = null;
                HttpContext.Current.Session.Remove("massiveSignReport");
                this.UpdateReportSignature(report);
                this.DigitalSignatureSelectedItem();
                return;
            }

            if (!string.IsNullOrEmpty(this.MassiveSignatureHSM.ReturnValue))
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('MassiveSignatureHSM','');", true);
                MassiveOperationReport report = HttpContext.Current.Session["massiveSignReport"] as MassiveOperationReport;
                HttpContext.Current.Session["CommandType"] = null;
                HttpContext.Current.Session.Remove("massiveSignReport");
                this.UpdateReportSignature(report);
                this.DigitalSignatureSelectedItem();
                return;
            }
        }

        #endregion

        #region  Event button

        protected void SignatureSelectedItemsConfirm_Click(object sender, EventArgs e)
        {
            bool noDigitalSign = true;

            try
            {
                ReportSignatureSelected = new MassiveOperationReport();
                //MassiveOperationReport report = new MassiveOperationReport();
                MassiveOperationUtils.ItemsStatus = null;
                MassiveOperationReport.MassiveOperationResultEnum result;
                string oggetto = string.Empty;
                string details;
                string popupCall = string.Empty;
                List<FileRequest> fileReqToSign = new List<FileRequest>();
                this.ListIdDocumentSigned = new List<string>();
                Allegato attach;
                Documento doc;

                #region AVANZAMENTO ITER
                List<ElementoInLibroFirma> listElementAdvancementProcess = (from i in this.ListaElementiLibroFirma
                                                                            where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                                            select i).ToList();
                if (listElementAdvancementProcess != null && listElementAdvancementProcess.Count > 0)
                {
                    foreach (ElementoInLibroFirma element in listElementAdvancementProcess)
                    {
                        if (string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale))
                        {
                            doc = new Documento()
                            {
                                descrizione = element.InfoDocumento.Oggetto,
                                docNumber = element.InfoDocumento.Docnumber,
                                versionId = element.InfoDocumento.VersionId,
                                version = element.InfoDocumento.NumVersione.ToString(),
                                versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                inLibroFirma = true

                            };
                            fileReqToSign.Add(doc as FileRequest);
                        }
                        else
                        {
                            attach = new Allegato()
                            {
                                descrizione = element.InfoDocumento.Oggetto,
                                docNumber = element.InfoDocumento.Docnumber,
                                versionId = element.InfoDocumento.VersionId,
                                version = element.InfoDocumento.NumVersione.ToString(),
                                versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                inLibroFirma = true

                            };
                            fileReqToSign.Add(attach as FileRequest);
                        }
                    }
                    bool isAdvancementProcess = true;
                    List<FirmaResult> firmaResult = LibroFirmaManager.PutElectronicSignatureMassive(fileReqToSign, isAdvancementProcess);

                    if (firmaResult != null)
                    {
                        foreach (FirmaResult r in firmaResult)
                        {
                            if (string.IsNullOrEmpty(r.errore))
                            {
                                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                                details = "Avanzamento iter avvenuto con successo";
                                oggetto = r.fileRequest.descrizione;
                                ReportSignatureSelected.AddReportRow(
                                    oggetto,
                                    result,
                                    details);
                                ListIdDocumentSigned.Add(r.fileRequest.docNumber);
                            }
                            else
                            {
                                result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                                oggetto = r.fileRequest.descrizione;
                                details = String.Format(
                                    "Si sono verificati degli errori durante la procedura di avanzamento. {0}",
                                    r.errore);
                                ReportSignatureSelected.AddReportRow(
                                    oggetto,
                                    result,
                                    details);

                            }

                        }
                    }
                }
                #endregion

                #region SOTTOSCRIZIONE

                fileReqToSign.Clear();
                List<ElementoInLibroFirma> listElementSubscription = (from i in this.ListaElementiLibroFirma
                                                                      where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.VERIFIED) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                                      select i).ToList();
                if (listElementSubscription != null && listElementSubscription.Count > 0)
                {
                    foreach (ElementoInLibroFirma element in listElementSubscription)
                    {
                        if (string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale))
                        {
                            doc = new Documento()
                            {
                                descrizione = element.InfoDocumento.Oggetto,
                                docNumber = element.InfoDocumento.Docnumber,
                                versionId = element.InfoDocumento.VersionId,
                                version = element.InfoDocumento.NumVersione.ToString(),
                                versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                tipoFirma = element.TipoFirmaFile,
                                inLibroFirma = true

                            };
                            fileReqToSign.Add(doc as FileRequest);
                        }
                        else
                        {
                            attach = new Allegato()
                            {
                                descrizione = element.InfoDocumento.Oggetto,
                                docNumber = element.InfoDocumento.Docnumber,
                                versionId = element.InfoDocumento.VersionId,
                                version = element.InfoDocumento.NumVersione.ToString(),
                                versionLabel = element.InfoDocumento.NumAllegato.ToString(),
                                tipoFirma = element.TipoFirmaFile,
                                inLibroFirma = true

                            };
                            fileReqToSign.Add(attach as FileRequest);
                        }
                    }
                    bool isAdvancementProcess = false;
                    List<FirmaResult> firmaResult = LibroFirmaManager.PutElectronicSignatureMassive(fileReqToSign, isAdvancementProcess);

                    if (firmaResult != null)
                    {
                        foreach (FirmaResult r in firmaResult)
                        {
                            if (string.IsNullOrEmpty(r.errore))
                            {
                                result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                                details = "Sottoscrizione avvenuta con successo";
                                oggetto = r.fileRequest.descrizione;
                                ReportSignatureSelected.AddReportRow(
                                    oggetto,
                                    result,
                                    details);
                                ListIdDocumentSigned.Add(r.fileRequest.docNumber);
                            }
                            else
                            {
                                result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                                oggetto = r.fileRequest.descrizione;
                                details = String.Format(
                                    "Si sono verificati degli errori durante la procedura di sottoscrizione. {0}",
                                    r.errore);
                                ReportSignatureSelected.AddReportRow(
                                    oggetto,
                                    result,
                                    details);
                            }

                        }
                    }
                }

                #endregion

                #region DIGITALE
                ListCheck = new Dictionary<String, String>();
				ListToSign = new Dictionary<string, FileToSign>();
				
                //FIRMA PADES
                fileReqToSign.Clear();
                List<ElementoInLibroFirma> listElementSignPADES = (from i in this.ListaElementiLibroFirma
                                                                   where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                                   select i).ToList();
                string idDocumento = null;
                string fileExstention = "PDF";
                FileToSign file = null;
                String isFirmato = null;
                String signType = null;
				
				if (listElementSignPADES != null && listElementSignPADES.Count > 0)
                {
                    noDigitalSign = false;
					foreach (ElementoInLibroFirma element in listElementSignPADES)
                    {
                         
                        idDocumento = "P" + element.InfoDocumento.Docnumber;
                        ListCheck.Add(idDocumento, idDocumento);
                        isFirmato = element != null ? element.FileOriginaleFirmato : "0";
                        signType = element != null ? element.TipoFirmaFile : TipoFirma.NESSUNA_FIRMA;
                        if (this.ListToSign != null)
                        {
                            if (!this.ListToSign.ContainsKey(idDocumento))
                            {
                                
                                file = new FileToSign(fileExstention, isFirmato, signType);
                                this.ListToSign.Add(idDocumento, file);
                            }
                            else
                            {
                                file = this.ListToSign[idDocumento];
                                file.signed = isFirmato;
                                file.fileExtension = fileExstention;
                            }
                        }
                    }
                }

                //FIRMA CADES
                fileReqToSign.Clear();
                List<ElementoInLibroFirma> listElementSignCADES = (from i in this.ListaElementiLibroFirma
                                                                   where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                                   select i).ToList();
                if (listElementSignCADES != null && listElementSignCADES.Count > 0)
                {
                    noDigitalSign = false;
                    foreach (ElementoInLibroFirma element in listElementSignCADES)
                    {
                        idDocumento = "C" + element.InfoDocumento.Docnumber;
                        ListCheck.Add(idDocumento, idDocumento);
                        isFirmato = element != null ? element.FileOriginaleFirmato : "0";
                        signType = element != null ? element.TipoFirmaFile : TipoFirma.NESSUNA_FIRMA;
                        if (this.ListToSign != null)
                        {
                            if (!this.ListToSign.ContainsKey(idDocumento))
                            {
                                isFirmato = element != null ? element.FileOriginaleFirmato : "0";
                                file = new FileToSign(fileExstention, isFirmato, signType);
                                this.ListToSign.Add(idDocumento, file);
                            }
                            else
                            {
                                file = this.ListToSign[idDocumento];
                                file.signed = isFirmato;
                                file.fileExtension = fileExstention;
                                file.signType = signType;
                            }
                        }
                    }
                }

                #endregion

                if (noDigitalSign)
                    DigitalSignatureSelectedItem();
                else
                {
                    switch (tipoSupportoFirma)
                    {
                        case "H":
                            HttpContext.Current.Session["CommandType"] = null;
                            popupCall = "ajaxModalPopupMassiveSignatureHSM();";
                            break;

                        case "L":
                            componentType = UserManager.getComponentType(Request.UserAgent);
                            if (componentType == Constans.TYPE_ACTIVEX || componentType == Constans.TYPE_SMARTCLIENT)
                                 popupCall = "ajaxModalPopupMassiveDigitalSignature();";
                            else if(componentType == Constans.TYPE_APPLET)
                            {
                                HttpContext.Current.Session["CommandType"] = null;
                                 popupCall = "ajaxModalPopupMassiveDigitalSignatureApplet();";
                            }
                            else if (componentType == Constans.TYPE_SOCKET)
                            {
                                 HttpContext.Current.Session["CommandType"] = null;
                                 popupCall = "ajaxModalPopupMassiveDigitalSignatureSocket();";
                            }
                            break;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "MassiveSignatureHSM", popupCall, true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        private string CheckSign(SchedaDocumento schedaDoc)
        {
            string msgError = string.Empty;

            FileRequest fileReq = schedaDoc.documenti[0];
            bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");
            string estenxione = FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant();

            #region FILE NON ACQUISITO
            if (string.IsNullOrEmpty(fileReq.fileSize) || int.Parse(fileReq.fileSize) == 0)
            {
                msgError = "Il file non risulta acquisito, quindi non è possibile applicare la firma HSM.";
                return msgError;
            }
            #endregion

            #region DOCUMENTO ANNULLATO

            if (schedaDoc.protocollo != null && schedaDoc.protocollo.protocolloAnnullato != null && !string.IsNullOrEmpty(schedaDoc.protocollo.protocolloAnnullato.dataAnnullamento))
            {
                msgError = "Il file non è stato firmato in quanto il protocollo risulta annullato.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN CESTINO

            if (schedaDoc.inCestino != null && schedaDoc.inCestino == "1")
            {
                msgError = "Il file non è stato firmato in quanto il documento è stato rimosso.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO CONSOLIDATO

            if (schedaDoc.ConsolidationState != null && schedaDoc.ConsolidationState.State != DocsPaWR.DocumentConsolidationStateEnum.None)
            {
                msgError = "Il file non è stato firmato in quanto il documento è consolidato.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN CHECKOUT

            if (schedaDoc.checkOutStatus != null && !string.IsNullOrEmpty(schedaDoc.checkOutStatus.ID))
            {
                msgError = "Il file non è stato firmato in quanto il documento è bloccato.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN SOLA LETTURA

            if (Convert.ToInt32(schedaDoc.accessRights) == Convert.ToInt32(HMdiritti.HMdiritti_Read))
            {
                msgError = "Il file non è stato firmato in quanto il documento è in sola lettura.";
                return msgError;
            }

            #endregion

            #region DOCUMENTO IN ATTESA DI ACCETTAZIONE

            if (Convert.ToInt32(schedaDoc.accessRights) == Convert.ToInt32(HMdiritti.HDdiritti_Waiting))
            {
                msgError = "Il file non è stato firmato in quanto il documento è in attesa di accettazione.";
                return msgError;
            }

            #endregion

            #region VERFICA SE è OBBLIGATORIA LA CONVERSIONE IN PDF DEL FILE
            if (!isPdf)
            {
                msgError = "Non è stato possibile firmare il documento. Effettuare la conversione in PDF del file.";
                return msgError;
            }

            #endregion

            return msgError;
        }

        private void UpdateReportSignature(MassiveOperationReport report)
        {
            if (report != null)
            {
                List<string> idDocumentSigned = new List<string>();
                string idDocumento = string.Empty;
                foreach (DataRow row in report.GetDataSet().Tables[0].Rows)
                {
                    idDocumento = row["ObjId"].ToString().Replace("P", "").Replace("C", "");
                    string oggetto = (from i in this.ListaElementiLibroFirma
                                      where i.InfoDocumento.Docnumber.Equals(idDocumento)
                                      select i.InfoDocumento.Oggetto).FirstOrDefault();
                    ReportSignatureSelected.AddReportRow(
                        oggetto,
                        ((MassiveOperationReport.MassiveOperationResultEnum)Enum.Parse(typeof(MassiveOperationReport.MassiveOperationResultEnum),
                        row["Result"].ToString())), row["Details"].ToString());

                    if (row["Result"].ToString().Equals(MassiveOperationReport.MassiveOperationResultEnum.OK.ToString()))
                        this.ListIdDocumentSigned.Add(idDocumento);
                }

            }
            else
            {
                foreach (string id in this.ListCheck.Keys)
                {
                    string oggetto = (from i in this.ListaElementiLibroFirma where i.InfoDocumento.Docnumber.Equals(id.Replace("P", "").Replace("C", "")) select i.InfoDocumento.Oggetto).FirstOrDefault();
                    ReportSignatureSelected.AddReportRow(
                        oggetto,
                        MassiveOperationReport.MassiveOperationResultEnum.KO,
                        "Firma annullata dall'utente");
                }
            }
        }

        private void DigitalSignatureSelectedItem()
        {
            //Per tutti i documenti lavorati aggiungo in area di lavoro
            if(AddElementiInADL && ListIdDocumentSigned.Count> 0)
                this.AggiungiInADL(ListIdDocumentSigned);

            HttpContext.Current.Session.Remove("ListIdDocumentSigned");
            // Introduzione della riga di summary
            if (ReportSignatureSelected.NotWorked == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('SignatureSelectedItems','');", true);
                return;
            }

            string[] pars = new string[] { "" + ReportSignatureSelected.Worked, "" + ReportSignatureSelected.NotWorked };
            ReportSignatureSelected.AddSummaryRow("Documenti lavorati: {0} - Documenti non lavorati: {1}", pars);

            this.generateReport(ReportSignatureSelected, "Sottoscizione degli elementi selezionati");

            this.plcMessage.Attributes.Add("style", "display:none");
            this.UpPnlMessage.Update();
        }

        private void generateReport(MassiveOperationReport report, string titolo)
        {
            this.grdReport.DataSource = report.GetDataSet();
            this.grdReport.DataBind();
            this.pnlReport.Visible = true;
            this.upReport.Update();

            string template = "../xml/massiveOp_formatPdfExport.xml";
            report.GenerateDataSetForExport(Server.MapPath(template), titolo);

            this.SignatureSelectedItemsCancel.Text = Utils.Languages.GetLabelFromCode("SignatureSelectedItemsClose", UserManager.GetUserLanguage());
            this.SignatureSelectedItemsConfirm.Enabled = false;
            this.BtnReport.Visible = true;
            this.UpPnlButtons.Update();
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "reallowOp", "reallowOp();", true);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "visualReport", "parent.ajaxModalPopupMassiveReport();", true);
        }

        protected void SignatureSelectedItemsCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "<script>reallowOp();</script>", false);
            try
            {
                ScriptManager.RegisterClientScriptBlock(this.UpPnlButtons, this.UpPnlButtons.GetType(), "closeAJM", "parent.closeAjaxModal('SignatureSelectedItems','');", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }


        #endregion

        #region Utils

        private void AggiungiInADL(List<string> listIdDocToAdd)
        {
            try
            {
                List<WorkingArea> listWorkingArea = new List<WorkingArea>();
                WorkingArea objectArea = null;
                string idDocumentoPrincipale = string.Empty;
                InfoDocumento infoDocPrincipale;
                foreach (string id in listIdDocToAdd)
                {
                    ElementoInLibroFirma elemento = (from e in this.ListaElementiFiltrati
                                                     where e.InfoDocumento.Docnumber.Equals(id)
                                                     select e).FirstOrDefault();
                    if (elemento != null)
                    {
                        objectArea = new WorkingArea();
                        objectArea.ObjectType = AreaLavoroTipoOggetto.DOCUMENTO;
                        if (!string.IsNullOrEmpty(elemento.InfoDocumento.IdDocumentoPrincipale) &&
                            !ListIdDocumentSigned.Contains(elemento.InfoDocumento.IdDocumentoPrincipale))
                        {
                            idDocumentoPrincipale = elemento.InfoDocumento.IdDocumentoPrincipale;
                            infoDocPrincipale = UIManager.DocumentManager.GetInfoDocumento(idDocumentoPrincipale, idDocumentoPrincipale, this);
                            objectArea.IdObject = idDocumentoPrincipale;
                            objectArea.TipoDocumento = infoDocPrincipale.tipoProto;
                            objectArea.IdRegistro = infoDocPrincipale.idRegistro;
                            objectArea.Motivo = Utils.Languages.GetLabelFromCode("SignatureSelectedItemAreaMotivoAllegato", UIManager.UserManager.GetUserLanguage());
                        }
                        else
                        {
                            objectArea.IdObject = elemento.InfoDocumento.Docnumber;
                            objectArea.TipoDocumento = elemento.InfoDocumento.TipoProto;
                            objectArea.IdRegistro = elemento.InfoDocumento.IdRegistro;
                            objectArea.Motivo = Utils.Languages.GetLabelFromCode("SignatureSelectedItemAreaMotivo", UIManager.UserManager.GetUserLanguage());
                        }
                        listWorkingArea.Add(objectArea);
                    }
                }
                if (listWorkingArea != null && listWorkingArea.Count > 0)
                    UIManager.DocumentManager.AddMassiveObjectInADL(listWorkingArea);
            }
            catch (Exception e)
            {

            }
        }

        private void GetMessaggeConfirm()
        {
            string language = UserManager.GetUserLanguage();
            string typeDigitalSignatureItems = string.Empty;
            string label = string.Empty;
            string msgTypeSignature = string.Empty;
            int countSingatureSelectedItems = (from i in this.ListaElementiLibroFirma
                                               where i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                               select i).ToList().Count;
            if (countSingatureSelectedItems > 0)
            {
                int countDigitalSignatureCades = (from i in this.ListaElementiLibroFirma
                                                  where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_CADES) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                  select i).ToList().Count;
                int countDigitalSignaturePades = (from i in this.ListaElementiLibroFirma
                                                  where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.SIGN_PADES) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                  select i).ToList().Count;
                int countElettronicSignature = (from i in this.ListaElementiLibroFirma
                                                where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.VERIFIED) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                                select i).ToList().Count;
                int countAdvancemenProcess = (from i in this.ListaElementiLibroFirma
                                              where i.TipoFirma.Equals(LibroFirmaManager.TypeEvent.ADVANCEMENT_PROCESS) && i.StatoFirma.Equals(TipoStatoElemento.DA_FIRMARE)
                                              select i).ToList().Count;
                label = (countSingatureSelectedItems > 1) ? "SignatureTotalSelectedItemsLblMessage" : "SignatureTotalSelectedItemLblMessage";
                string messagge = Utils.Languages.GetMessageFromCode(label, language).Replace("@@", countSingatureSelectedItems.ToString());
                if (countElettronicSignature > 0)
                {
                    label = (countElettronicSignature > 1) ? "LibroFirmaLabelElementsSelected" : "LibroFirmaLabelElementSelected";
                    string msgElettronicSignature = countElettronicSignature.ToString() + " " + Utils.Languages.GetLabelFromCode(label, language);
                    msgTypeSignature += "<li> " + Utils.Languages.GetMessageFromCode("ElettronicVerifidSignatureSelectedItemsLblMessage", language).Replace("@@", msgElettronicSignature) + "</li>";
                }
                if (countAdvancemenProcess > 0)
                {
                    label = (countAdvancemenProcess > 1) ? "LibroFirmaLabelElementsSelected" : "LibroFirmaLabelElementSelected";
                    string msgAdvancementProcess = countAdvancemenProcess.ToString() + " " + Utils.Languages.GetLabelFromCode(label, language);
                    msgTypeSignature += "<li> " + Utils.Languages.GetMessageFromCode("AdvancementProcessSelectedItemsLblMessage", language).Replace("@@", msgAdvancementProcess) + "</li>";
                }
                if (countDigitalSignatureCades > 0)
                {
                    label = (countDigitalSignatureCades > 1) ? "LibroFirmaLabelElementsSelected" : "LibroFirmaLabelElementSelected";
                    string msgDigilatSignatureCades = countDigitalSignatureCades.ToString() + " " + Utils.Languages.GetLabelFromCode(label, language);
                    msgTypeSignature += "<li> " + Utils.Languages.GetMessageFromCode("DigitalSignatureCADESSelectedItemsLblMessage", language).Replace("@@", msgDigilatSignatureCades) + "</li>";
                }
                if (countDigitalSignaturePades > 0)
                {
                    label = (countDigitalSignaturePades > 1) ? "LibroFirmaLabelElementsSelected" : "LibroFirmaLabelElementSelected";
                    string msgDigilatSignaturePades = countDigitalSignaturePades.ToString() + " " + Utils.Languages.GetLabelFromCode(label, language);
                    msgTypeSignature += "<li> " + Utils.Languages.GetMessageFromCode("DigitalSignaturePADESSelectedItemsLblMessage", language).Replace("@@", msgDigilatSignaturePades) + "</li>";
                }

                if (countDigitalSignatureCades > 0 && countDigitalSignaturePades > 0 && tipoSupportoFirma.Equals("H"))
                {
                    msgTypeSignature += Utils.Languages.GetMessageFromCode("DigitalSignatureRequestOtherOTP", language);
                }
                this.lblMessage.Text += messagge.Replace("##", msgTypeSignature);


                this.imgConfirm.ImageUrl = "~/Images/Common/messager_question.gif";
            }
            else
            {
                this.lblMessage.Text = Utils.Languages.GetMessageFromCode("DigitalSignatureNoElementsToSignSelected", language);
                this.imgConfirm.ImageUrl = "~/Images/Common/messager_warning.png";
                this.SignatureSelectedItemsConfirm.Visible = false;
            }
        }
        #endregion
    }


}
