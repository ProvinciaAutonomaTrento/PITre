using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Document
{
    public partial class SearchDocument : System.Web.UI.Page
    {
        private string PROTOCOL = "P";
        private string NOTPROTOCOL = "N";

        //XDEMO
        public string IdAmministrazione = "361";
        public string titolario = "7067503";
        //FINE DEMO

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.RblTypeProtocol.Items[0].Attributes.Add("class", "orange");
                    this.PProtocolType.InnerText = "A";
                    this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrange");
                    this.container.Attributes.Add("class", "borderOrange");
                    this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxOrange");
                    this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxOrangeBg");
                    this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabOrangeDxBorder");
                    this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabOrange");
                    this.DocumentBtnPrepared.Visible = false;
                    this.DocumentBtnPrint.Visible = false;
                    this.DocumentBtnRemove.Visible = false;
                    this.DocumentBtnUndo.Enabled = true;
                    this.DocumentBtnSave.Enabled = true;
                    this.DocumentBntRecord.Enabled = false;
                    this.PnlRecipients.Visible = false;
                    this.DocumentBtnRepeat.Enabled = true;
                    this.DocumentBtnAdL.Enabled = true;
                    this.DocumentBtnSend.Visible = false;
                    this.DocumentBtnTransmit.Visible = false;
                    this.RblTypeProtocol.Enabled = false;
                    this.PnlTypeDocument.Visible = true;

                    ListItem item = new ListItem();
                    item.Text = "PAT";
                    item.Value = "11";
                    this.DdlRegistries.Items.Add(item);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void PopulateDDLRegistry(DocsPaWR.Ruolo role)
        {
   
        }

        protected void RblTypeProtocol_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.RblTypeProtocol.SelectedValue))
                {
                    if (this.RblTypeProtocol.SelectedValue.Equals("A"))
                    {
                        this.RblTypeProtocol.Items[0].Attributes.Add("class", "orange");
                        this.PProtocolType.InnerText = "A";
                        this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxOrange");
                        this.container.Attributes.Add("class", "borderOrange");
                        this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxOrange");
                        this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxOrangeBg");
                        this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabOrangeDxBorder");
                        this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabOrange");
                        this.PnlMultipleSender.Visible = true;
                        this.PnlMeansSender.Visible = true;
                        this.PnlRecipients.Visible = false;
                        this.DocumentBtnSend.Visible = false;
                        this.DocumentBtnTransmit.Visible = false;
                    }
                    else
                    {
                        if (this.RblTypeProtocol.SelectedValue.Equals("P"))
                        {
                            this.RblTypeProtocol.Items[1].Attributes.Add("class", "green");
                            this.PProtocolType.InnerText = "P";
                            this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxGreen");
                            this.container.Attributes.Add("class", "borderGreen");
                            this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxGreen");
                            this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxGreenBg");
                            this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabGreenDxBorder");
                            this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabGreen");
                            this.PnlMultipleSender.Visible = false;
                            this.PnlMeansSender.Visible = false;
                            this.PnlRecipients.Visible = true;
                            this.DocumentBtnSend.Visible = true;
                            this.DocumentBtnSend.Enabled = false;
                            this.DocumentBtnTransmit.Visible = false;
                        }
                        else
                        {
                            if (this.RblTypeProtocol.SelectedValue.Equals("I"))
                            {
                                this.RblTypeProtocol.Items[2].Attributes.Add("class", "blue");
                                this.PProtocolType.InnerText = "I";
                                this.PnlcontainerDocumentTopSx.Attributes.Add("class", "containerDocumentTopSxBlue");
                                this.container.Attributes.Add("class", "borderBlue");
                                this.containerDocumentTopDx.Attributes.Add("class", "containerDocumentTopDxBlue");
                                this.containerDocumentTopCxOrange.Attributes.Add("class", "containerDocumentTopCxBlueBg");
                                this.containerDocumentTabDxBorder.Attributes.Add("class", "containerDocumentTabBlueDxBorder");
                                this.containerDocumentTab.Attributes.Add("class", "containerDocumentTabBlue");
                                this.PnlMultipleSender.Visible = false;
                                this.PnlMeansSender.Visible = false;
                                this.PnlRecipients.Visible = true;
                                this.DocumentBtnSend.Visible = false;
                                this.DocumentBtnTransmit.Visible = true;
                                this.DocumentBtnTransmit.Enabled = false;
                            }
                        }
                    }
                    this.UpLetterTypeProtocol.Update();
                    this.UpContainer.Update();
                    this.UpcontainerDocumentTopDx.Update();
                    this.UpcontainerDocumentTopCx.Update();
                    this.UpcontainerDocumentTabDxBorder.Update();
                    this.UpContainerDocumentTab.Update();
                    this.UpPnlMultipleSender.Update();
                    this.UpPnlMeansSender.Update();
                    this.UpPnlRecipients.Update();
                    this.UpDocumentButtons.Update();
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void DocumentBntRecord_Click(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.TxtCodeSender.Text) && !string.IsNullOrEmpty(this.TxtDescriptionSender.Text) && !string.IsNullOrEmpty(this.TxtCodeProject.Text) && !string.IsNullOrEmpty(this.TxtDescriptionProject.Text) && !string.IsNullOrEmpty(this.TxtObject.Text))
                {
                    this.Record();
                    this.UpcontainerDocumentTopCx.Update();
                    this.UpDocumentButtons.Update();
                    this.UpTypeProtocol.Update();
                    this.UpContainerDocumentTab.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Inserire tutti i valori obbligatori');", true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void Record()
        {
            DocsPaWR.ResultProtocollazione result = DocsPaWR.ResultProtocollazione.OK;
            DocsPaWR.Fascicolo fasc = new DocsPaWR.Fascicolo();
            string resFunz = string.Empty;
            SchedaDocumento doc = UIManager.DocumentManager.NewSchedaDocumento();
            doc.oggetto = new Oggetto();
            doc.oggetto.descrizione = this.TxtObject.Text;
            doc.oggetto.daAggiornare = true;
            DocsPaWR.Corrispondente sender = UIManager.AddressBookManager.GetCorrespondentBySystemId(this.IdSender.Value);
            doc.tipoProto = this.RblTypeProtocol.SelectedValue;
            if (this.RblTypeProtocol.SelectedValue.Equals("A"))
            {
                doc.protocollo = new DocsPaWR.ProtocolloEntrata();
                ((ProtocolloEntrata)doc.protocollo).mittente = sender;
            }
            doc.registro = UIManager.RegistryManager.GetRegistryInSession();
            fasc = UIManager.ProjectManager.GetProjectByCode(doc.registro, this.TxtCodeProject.Text);
            //  fasc = FascicoliManager.getFascicoloById(this.Page, this.id_Fasc.Value);
            SchedaDocumento resultDoc = new SchedaDocumento();

            doc.id_rf_prot = "347392";
            doc.id_rf_invio_ricevuta = "347392";
            doc.cod_rf_prot = "RFD320";

            resultDoc = UIManager.DocumentManager.Record(this.Page, doc, fasc, null, out result, ref resFunz);

            //Crea il file o se esistente una nuova versione
            // 5. Acquisizione del file al documento

            //FileRequest versioneCorrente = (FileRequest)resultDoc.documenti[0];

            //string txtPathFile = Server.MapPath("~/images/demo/documento_principale.pdf");

            ////Acquisisco il documento
            //byte[] content = null;
            //content = File.ReadAllBytes(txtPathFile);

            //FileDocumento fileDocumento = new FileDocumento
            //{
            //    name = "documento_principale.pdf",
            //    fullName = "documento_principale.pdf",
            //    content = content,
            //    length = content.Length,
            //    bypassFileContentValidation = true
            //};


            //docsPaWS.DocumentoPutFile(versioneCorrente, fileDocumento, UserManager.getInfoUtente(this));


            //this.Document = resultDoc;

            this.LblIdDocument.Text = resultDoc.systemId;
            this.LblReferenceCode.Text = resultDoc.protocollo.segnatura;
            this.RblTypeProtocol.Enabled = false;

            this.DocumentBntRecord.Enabled = false;
            this.DocumentBtnSave.Enabled = true;
            this.DocumentBtnUndo.Enabled = true;
            this.DocumentBtnRepeat.Enabled = true;
            this.DocumentBtnAdL.Enabled = true;

            this.LinkAttachedFiles.Enabled = true;
            this.LinkClassificationSchemes.Enabled = true;
            this.LinkEvents.Enabled = true;
            this.LinkTransmissions.Enabled = true;
            this.LinkVisibility.Enabled = true;
            this.link2.Attributes.Remove("class");
            this.link3.Attributes.Remove("class");
            this.link4.Attributes.Remove("class");
            this.link5.Attributes.Remove("class");
            this.link6.Attributes.Remove("class");
            this.link2.Attributes.Add("class", "docOther");
            this.link3.Attributes.Add("class", "docOther");
            this.link4.Attributes.Add("class", "docOther");
            this.link5.Attributes.Add("class", "docOther");
            this.link6.Attributes.Add("class", "docOther");

        }

        protected void TxtCodeSender_OnTextChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.TxtCodeSender.Text))
                {
                    this.SearchCorrespondent(TxtCodeSender.Text);
                }
                else
                {
                    this.TxtCodeSender.Text = string.Empty;
                    this.TxtDescriptionSender.Text = string.Empty;
                    this.IdSender.Value = string.Empty;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_prole", "alert('Mittente non trovato');", true);
                }
                this.UpPnlSender.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchCorrespondent(string addressCode)
        {
            //DocsPaWR.Corrispondente corr = UIManager.AddressBookManager.GetCorrespondentFromCodeIE(addressCode, AddressbookTipoUtente.GLOBALE);
            //if (corr == null)
            //{
            //    this.TxtCodeSender.Text = string.Empty;
            //    this.TxtDescriptionSender.Text = string.Empty;
            //    this.IdSender.Value = string.Empty;
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_found", "alert('Mittente non trovato');", true);
            //}
            //else
            //{
            //    this.TxtCodeSender.Text = corr.codiceRubrica;
            //    this.TxtDescriptionSender.Text = corr.descrizione;
            //    this.IdSender.Value = corr.systemId;
            //}
        }

        protected void TxtCodeProject_OnTextChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.TxtCodeProject.Text))
                {
                    this.SearchProject();
                }
                else
                {
                    this.TxtCodeProject.Text = string.Empty;
                    this.TxtDescriptionProject.Text = string.Empty;
                    this.IdProject.Value = string.Empty;
                }
                this.UpPnlProject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SearchProject()
        {

            // Lista dei fascicoli restituiti dalla ricerca
            Fascicolo[] projectList;

            projectList = UIManager.ProjectManager.GetProjectFromCodeNoSecurity(this.TxtCodeProject.Text, this.IdAmministrazione, this.titolario, true);

            if (projectList == null || projectList.Length == 0)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_project", "alert('Nessun fascicolo trovato con questo codice');", true);
            }
            else
            {

                this.TxtCodeProject.Text = projectList[1].codice;
                this.TxtDescriptionProject.Text = projectList[1].descrizione;
                this.IdProject.Value = projectList[1].systemID;

            }

        }

        protected void DdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(DdlTypeDocument.SelectedValue))
                {
                    this.PnlTypeDocument.Visible = true;
                }
                else
                {
                    this.PnlTypeDocument.Visible = false;
                }
                this.UpPnlTypeDocument.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void LinkViewFileDocument(object sender, EventArgs e)
        {
            try {
                this.PnlDocumentData.Visible = true;
                this.PnlDocumentNotAcuired.Visible = false;
                this.PnlDocumentAttached.Visible = true;
                this.UpPnlDocumentData.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshTipsy", "tooltipTipsy();", true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshFrame", "resizeIframe();", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}