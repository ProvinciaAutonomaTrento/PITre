using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;
using System.Web.UI.HtmlControls;
using NttDataWA.UserControls;

namespace NttDataWA.Popup
{
    public partial class AnswerSearchDocuments : System.Web.UI.Page
    {
        #region Fields
        
        protected int nRec;
        protected DocsPaWR.FiltroRicerca fV1;
        protected DocsPaWR.FiltroRicerca[] fVList;
        protected ArrayList Dg_elem;
        protected DocsPaWR.SchedaDocumento schedaDocUscita;
        protected DocsPaWR.SchedaDocumento schedaDocIngresso;
        protected DocsPaWR.SchedaDocumento schedaDoc;
        
        #endregion

        #region Properties

        private SchedaDocumento DocumentWIP
        {
            get
            {
                SchedaDocumento result = null;
                if (HttpContext.Current.Session["Answer.DocumentWIP"] != null)
                {
                    result = HttpContext.Current.Session["Answer.DocumentWIP"] as SchedaDocumento;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Answer.DocumentWIP"] = value;
            }
        }

        private DocsPaWR.InfoUtente InfoUser
        {
            get
            {
                DocsPaWR.InfoUtente result = null;
                if (HttpContext.Current.Session["infoUser"] != null)
                {
                    result = HttpContext.Current.Session["infoUser"] as DocsPaWR.InfoUtente;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["infoUser"] = value;
            }
        }

        private bool InternalRecordEnable
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["internalRecordEnable"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["internalRecordEnable"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["internalRecordEnable"] = value;
            }
        }

        private DocsPaWR.FiltroRicerca[][] ListaFiltri
        {
            get
            {
                DocsPaWR.FiltroRicerca[][] result = null;
                if (HttpContext.Current.Session["ListaFiltri"] != null)
                {
                    result = (DocsPaWR.FiltroRicerca[][])HttpContext.Current.Session["ListaFiltri"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ListaFiltri"] = value;
            }
        }

        private DocsPaWR.InfoDocumento[] infoDoc
        {
            get
            {
                DocsPaWR.InfoDocumento[] result = null;
                if (HttpContext.Current.Session["infoDoc"] != null)
                {
                    result = (DocsPaWR.InfoDocumento[])HttpContext.Current.Session["infoDoc"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["infoDoc"] = value;
            }
        }

        private DocsPaWR.InfoDocumento infoDocSel
        {
            get
            {
                DocsPaWR.InfoDocumento result = null;
                if (HttpContext.Current.Session["infoDocSel"] != null)
                {
                    result = (DocsPaWR.InfoDocumento)HttpContext.Current.Session["infoDocSel"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["infoDocSel"] = value;
            }
        }

        private DocsPaWR.Corrispondente[] listaCorrTo
        {
            get
            {
                DocsPaWR.Corrispondente[] result = null;
                if (HttpContext.Current.Session["listaCorrTo"] != null)
                {
                    result = (DocsPaWR.Corrispondente[])HttpContext.Current.Session["listaCorrTo"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaCorrTo"] = value;
            }
        }

        private DocsPaWR.Corrispondente[] listaCorrCC
        {
            get
            {
                DocsPaWR.Corrispondente[] result = null;
                if (HttpContext.Current.Session["listaCorrCC"] != null)
                {
                    result = (DocsPaWR.Corrispondente[])HttpContext.Current.Session["listaCorrCC"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["listaCorrCC"] = value;
            }
        }

        private int PageSize
        {
            get
            {
                int toReturn = 10;
                if (HttpContext.Current.Session["PageSize"] != null) Int32.TryParse(HttpContext.Current.Session["PageSize"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PageSize"] = value;
            }
        }

        private int SelectedPage
        {
            get
            {
                int toReturn = 1;
                if (HttpContext.Current.Session["SelectedPage"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedPage"].ToString(), out toReturn);
                if (toReturn < 1) toReturn = 1;

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedPage"] = value;
            }

        }

        private int SelectedRowIndex
        {
            get
            {
                int toReturn = -1;
                if (HttpContext.Current.Session["SelectedRowIndex"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedRowIndex"].ToString(), out toReturn);

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedRowIndex"] = value;
            }

        }

        private int PagesCount
        {
            get
            {
                int toReturn = 0;
                if (HttpContext.Current.Session["PagesCount"] != null) Int32.TryParse(HttpContext.Current.Session["PagesCount"].ToString(), out toReturn);
                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["PagesCount"] = value;
            }
        }

        private int SelectedCorrIndex
        {
            get
            {
                int toReturn = -1;
                if (HttpContext.Current.Session["SelectedCorrIndex"] != null) Int32.TryParse(HttpContext.Current.Session["SelectedCorrIndex"].ToString(), out toReturn);

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["SelectedCorrIndex"] = value;
            }

        }

        private string FilterTipoDoc
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterTipoDoc"] != null) toReturn = HttpContext.Current.Session["FilterTipoDoc"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterTipoDoc"] = value;
            }
        }

        private bool FilterADL
        {
            get
            {
                bool toReturn = false;
                if (HttpContext.Current.Session["FilterADL"] != null) toReturn = (bool)HttpContext.Current.Session["FilterADL"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterADL"] = value;
            }
        }

        private string FilterRF
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterRF"] != null) toReturn = HttpContext.Current.Session["FilterRF"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterRF"] = value;
            }
        }

        private string FilterNumProt
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumProt"] != null) toReturn = HttpContext.Current.Session["FilterNumProt"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumProt"] = value;
            }
        }

        private string FilterNumProtFrom
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumProtFrom"] != null) toReturn = HttpContext.Current.Session["FilterNumProtFrom"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumProtFrom"] = value;
            }
        }

        private string FilterNumProtTo
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumProtTo"] != null) toReturn = HttpContext.Current.Session["FilterNumProtTo"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumProtTo"] = value;
            }
        }

        private string FilterNumProtYear
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumProtYear"] != null) toReturn = HttpContext.Current.Session["FilterNumProtYear"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumProtYear"] = value;
            }
        }

        private string FilterDtProt
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDtProt"] != null) toReturn = HttpContext.Current.Session["FilterDtProt"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDtProt"] = value;
            }
        }

        private string FilterDtProtFrom
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDtProtFrom"] != null) toReturn = HttpContext.Current.Session["FilterDtProtFrom"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDtProtFrom"] = value;
            }
        }

        private string FilterDtProtTo
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDtProtTo"] != null) toReturn = HttpContext.Current.Session["FilterDtProtTo"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDtProtTo"] = value;
            }
        }

        private string FilterNumDoc
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumDoc"] != null) toReturn = HttpContext.Current.Session["FilterNumDoc"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumDoc"] = value;
            }
        }

        private string FilterNumDocFrom
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumDocFrom"] != null) toReturn = HttpContext.Current.Session["FilterNumDocFrom"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumDocFrom"] = value;
            }
        }

        private string FilterNumDocTo
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterNumDocTo"] != null) toReturn = HttpContext.Current.Session["FilterNumDocTo"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterNumDocTo"] = value;
            }
        }

        private string FilterDtDoc
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDtDoc"] != null) toReturn = HttpContext.Current.Session["FilterDtDoc"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDtDoc"] = value;
            }
        }

        private string FilterDtDocFrom
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDtDocFrom"] != null) toReturn = HttpContext.Current.Session["FilterDtDocFrom"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDtDocFrom"] = value;
            }
        }

        private string FilterDtDocTo
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["FilterDtDocTo"] != null) toReturn = HttpContext.Current.Session["FilterDtDocTo"].ToString();

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["FilterDtDocTo"] = value;
            }
        }

        private DataTable ProtoOut
        {
            get
            {
                DataTable result = null;
                if (HttpContext.Current.Session["ProtoOut"] != null)
                {
                    result = (DataTable)HttpContext.Current.Session["ProtoOut"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ProtoOut"] = value;
            }
        }

        private DataSet Recipients
        {
            get
            {
                DataSet result = null;
                if (HttpContext.Current.Session["Recipients"] != null)
                {
                    result = (DataSet)HttpContext.Current.Session["Recipients"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["Recipients"] = value;
            }
        }

        private Hashtable ht_destinatariTO_CC
        {
            get
            {
                Hashtable result = null;
                if (HttpContext.Current.Session["ht_destinatariTO_CC"] != null)
                {
                    result = (Hashtable)HttpContext.Current.Session["ht_destinatariTO_CC"];
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["ht_destinatariTO_CC"] = value;
            }
        }

        private string AnswerProtocolSender
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolSender"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolSender"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolSender"] = value;
            }
        }

        private string AnswerProtocolSubject
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolSubject"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolSubject"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolSubject"] = value;
            }
        }

        private string AnswerProtocolIsProtocol
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolIsProtocol"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolIsProtocol"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolIsProtocol"] = value;
            }
        }

        private string AnswerProtocolIsOccasional
        {
            get
            {
                string toReturn = null;
                if (HttpContext.Current.Session["AnswerProtocolIsOccasional"] != null) toReturn = (string)HttpContext.Current.Session["AnswerProtocolIsOccasional"];

                return toReturn;
            }
            set
            {
                HttpContext.Current.Session["AnswerProtocolIsOccasional"] = value;
            }
        }

        private DocsPaWR.Templates Template
        {
            get
            {
                Templates result = null;
                if (HttpContext.Current.Session["template1"] != null)
                {
                    result = HttpContext.Current.Session["template1"] as Templates;
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["template1"] = value;
            }
        }

        private bool EnableStateDiagram
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableStateDiagram"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableStateDiagram"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableStateDiagram"] = value;
            }
        }

        private bool CustomDocuments
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["customDocuments"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["customDocuments"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["customDocuments"] = value;
            }
        }

        private bool SearchCorrespondentIntExtWithDisabled
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["searchCorrespondentIntExtWithDisabled"] = value;
            }
        }

        protected int MaxLenghtObject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
            }
        }

        private bool EnableCodeObject
        {
            get
            {
                bool result = false;
                if (HttpContext.Current.Session["enableCodeObject"] != null)
                {
                    result = bool.Parse(HttpContext.Current.Session["enableCodeObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["enableCodeObject"] = value;
            }
        }

        private string ReturnValue
        {
            get
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Session["ReturnValuePopup"].ToString()))
                    return HttpContext.Current.Session["ReturnValuePopup"].ToString();
                else
                    return string.Empty;
            }
        }

        private bool AddDoc
        {
            set
            {
                HttpContext.Current.Session["AddDocInProject"] = value;
            }
        }
        #endregion 


        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializePage();
                    this.Template = null;
                    this.plc_doc.Visible = false;
                    this.plc_proto.Visible = false;

                    if (!this.InternalRecordEnable)
                        this.rbl_TipoDoc.Items.Remove(this.rbl_TipoDoc.Items.FindByValue("Interno"));


                    this.DocumentWIP = DocumentManager.getSelectedRecord();
                    if (this.DocumentWIP != null && this.DocumentWIP.protocollo != null && !string.IsNullOrEmpty(this.DocumentWIP.systemId) && string.IsNullOrEmpty(this.DocumentWIP.protocollo.numero))
                    {
                        this.rbl_TipoDoc.Items.FindByValue("Predisposti").Selected = true;
                        this.plc_proto.Visible = true;
                    }
                    else
                    {
                        switch (this.DocumentWIP.tipoProto.ToUpper())
                        {
                            case "P":
                                this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected = true;
                                this.plc_proto.Visible = true;
                                break;
                            case "A":
                                this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected = true;
                                this.plc_proto.Visible = true;
                                break;
                            case "N":
                            case "NP":
                            case "G":
                                this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected = true;
                                this.plc_doc.Visible = true;
                                break;
                            case "I":
                                this.rbl_TipoDoc.Items.FindByValue("Interno").Selected = true;
                                this.plc_proto.Visible = true;
                                break;
                        }
                    }
                }
                else
                {
                    if (((ScriptManager)Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                    {
                        // detect action from async postback
                        switch (((ScriptManager)Master.FindControl("ScriptManager1")).AsyncPostBackSourceElementID)
                        {
                            case "upPnlGridList":
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                                this.LoadData(true);
                                break;
                        }
                    }

                    this.ReApplyScripts();

                    // gestione del valore di ritorno della modal Dialog 
                    if (!string.IsNullOrEmpty(this.AvviseProtocol.ReturnValue) && this.AvviseProtocol.ReturnValue!="undefined")
                    {
                        string retValue = this.GestioneAvvisoModale(this.AvviseProtocol.ReturnValue);
                        if (retValue != "C")
                        {
                            this.ClearSessionData();
                            this.CloseMask(true);
                        }
                        else
                        {
                            this.AvviseProtocol.ReturnValue = string.Empty;
                            //this.LoadData(true);
                        }
                    }

                    this.setValueReturn();
                }
                if (this.CustomDocuments)
                {
                    this.PnlTypeDocument.Controls.Clear();
                    if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                    {
                        if (this.Template == null || !this.Template.SYSTEM_ID.ToString().Equals(this.DocumentDdlTypeDocument.SelectedValue))
                        {
                            this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        }
                        if (this.CustomDocuments)
                        {
                            this.PopulateProfiledDocument();
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

        protected void InitializePage()
        {
            this.InfoUser = UIManager.UserManager.GetInfoUser();
            this.Form.DefaultButton = this.BtnSearch.UniqueID;

            this.InitLanguage();
            this.LoadKeys();
            if (this.EnableCodeObject)
            {
                this.PnlCodeObject.Visible = true;
                this.PnlCodeObject2.Attributes.Add("class", "colHalf2");
                this.PnlCodeObject3.Attributes.Add("class", "colHalf3");
                this.PnlCodeObject.Attributes.Add("class", "colHalf");
                this.TxtObject.Attributes.Remove("class");
                this.TxtObject.Attributes.Add("class", "txt_objectRight");
            }
            this.AddDoc = true;
            this.SelectedRowIndex = -1; 
            this.LoadTypeDocuments();
            this.ReApplyScripts();
            this.LoadRegisters();
            this.getLettereProtocolli();
        }

        private void InitLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.chk_ADL.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocChkADL", language);
            this.litRegistry.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocRegistry", language);
            this.litNumProtocol.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocNumProtocol", language);
            this.ddl_numProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocSingleValue", language), "0"));
            this.ddl_numProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocInterval", language), "1"));
            this.ddl_numProto.Items[0].Selected = true;
            this.litDateProtocol.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocDateProtocol", language);
            this.ddl_dtaProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocSingleValue", language), "0"));
            this.ddl_dtaProto.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocInterval", language), "1"));
            this.ddl_dtaProto.Items[0].Selected = true;
            this.litIdDocument.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocIdDocument", language);
            this.ddl_numDoc.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocSingleValue", language), "0"));
            this.ddl_numDoc.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocInterval", language), "1"));
            this.ddl_numDoc.Items[0].Selected = true;
            this.litDateDocument.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocDateDocument", language);
            this.ddl_dtaDoc.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocSingleValue", language), "0"));
            this.ddl_dtaDoc.Items.Add(new ListItem(Utils.Languages.GetLabelFromCode("AnswerSearchDocInterval", language), "1"));
            this.ddl_dtaDoc.Items[0].Selected = true;
            this.litSelectRecipient.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocSelectRecipient", language);
            this.BtnOk.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocBtnOk", language);
            this.BtnSearch.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocBtnSearch", language);
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocBtnClose", language);
            this.pred.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocPred", language);
            this.SearchDocumentLitTypology.Text = Utils.Languages.GetLabelFromCode("SearchDocumentLitTypology", language);
            this.DocumentDdlTypeDocument.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("DocumentDdlTypeDocument", language));
            this.Object.Title = Utils.Languages.GetLabelFromCode("TitleObjectPopup", language);
            this.LblAddDocOgetto.Text = Utils.Languages.GetLabelFromCode("AnswerSearchDocumentObject", language);
            this.projectLitVisibleObjectChars.Text = Utils.Languages.GetLabelFromCode("DocumentLitObjectChAv", language);
        }

        protected void LoadKeys()
        {
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(this.InfoUser.idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString())) && Utils.InitConfigurationKeys.GetValue(this.InfoUser.idAmministrazione, DBKeys.FE_INTERNAL_PROTOCOL.ToString()).Equals("1"))
            {
                this.InternalRecordEnable = true;
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.ProfilazioneDinamica.ToString()].Equals("1"))
            {
                this.CustomDocuments = true;
            }

            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()]) && System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.DiagrammiStato.ToString()].Equals("1"))
            {
                this.EnableStateDiagram = true;
            }
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.USE_CODICE_OGGETTO.ToString()]) && bool.Parse(System.Configuration.ConfigurationManager.AppSettings[WebConfigKeys.USE_CODICE_OGGETTO.ToString()]))
            {
                this.EnableCodeObject = true;
            }
        }

        public void ClearSessionData()
        {
            this.ListaFiltri = null;
            this.infoDoc = null;
            this.infoDocSel = null;
            this.listaCorrTo = null;
            this.listaCorrCC = null;
            this.ProtoOut = null;
            this.Recipients = null;
            this.ht_destinatariTO_CC = null;
            Session.Remove("AddDocInProject");
            RicercaDocumentiSessionMng.RemoveListaInfoDocumenti(this);
            RicercaDocumentiSessionMng.removeCorrispondenteRispostaUSCITA(this);
            RicercaDocumentiSessionMng.removeCorrispondenteRispostaINGRESSO(this);
        }
        private void setValueReturn()
        {
            if (!string.IsNullOrEmpty(this.Object.ReturnValue))
            {
                this.TxtObject.Text = this.ReturnValue.Split('#').First();
                if (this.ReturnValue.Split('#').Length > 1)
                    this.TxtCodeObject.Text = this.ReturnValue.Split('#').Last();
                this.UpdPnlObject.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "setReturnValue", "SetRetValue('Object','');", true);
            }
        }
        private string GestioneAvvisoModale(string valore)
        {
            string retValue = string.Empty;
            try
            {
                //prendo il protocollo in ingresso in sessione
                schedaDocIngresso = this.DocumentWIP;

                //prendo il protocollo in uscita selezionato dal datagrid
                retValue = valore;
                switch (valore)
                {
                    case "Y": //Gestione pulsante SI, CONTINUA

                        /* Alla pressione del pulsante CONTINUA l'utente vuole proseguire il 
                         * collegamento nonostante oggetto e corrispondente dei due protocolli siano diversi */

                        if (schedaDocIngresso != null && schedaDocIngresso.protocollo != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            this.AvviseProtocol.ReturnValue = "";

                            //QUI CRASHA PERCHé MITTENTE è null
                            if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                            {
                                ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this);
                            }
                            this.DocumentWIP = schedaDocIngresso;
                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde

                            this.CloseMask(true);
                        }

                        //Per la concatenazione con i grigi
                        if (schedaDocIngresso != null && (schedaDocIngresso.tipoProto.Equals("G") || schedaDocIngresso.tipoProto.ToUpper().Equals("N") || schedaDocIngresso.tipoProto.Equals("NP")))
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            this.AvviseProtocol.ReturnValue = "";

                            this.DocumentWIP = schedaDocIngresso;
                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde

                            this.CloseMask(true);
                        }

                        //FINE CONCATENAZIONE CON I GRIGI

                        break;

                    case "N":

                        /* Alla pressione del pulsante NO, RESETTA l'utente vuole proseguire il collegamento 
                         * con i dati che ha digitato sulla pagina di protocollo */

                        //Per la concatenazione con i grigi

                        if (schedaDocIngresso != null && (schedaDocIngresso.tipoProto.Equals("G") || schedaDocIngresso.tipoProto.ToUpper().Equals("N") || schedaDocIngresso.tipoProto.Equals("NP")))
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            if (schedaDocIngresso.oggetto != null)
                            {
                                schedaDocIngresso.oggetto.descrizione = infoDocSel.oggetto.ToString();
                            }
                            else
                            {
                                DocsPaWR.Oggetto ogg = new DocsPaWR.Oggetto();
                                ogg.descrizione = infoDocSel.oggetto.ToString();
                                schedaDocIngresso.oggetto = ogg;
                            }

                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde
                            this.DocumentWIP = schedaDocIngresso;
                            this.CloseMask(true);

                            this.AvviseProtocol.ReturnValue = "";
                        }

                        //FINE GESTIONE DOCUMENTI CON RISPOSTA

                        if (schedaDocIngresso != null && schedaDocIngresso.protocollo != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }

                            if (schedaDocIngresso.oggetto != null)
                            {
                                schedaDocIngresso.oggetto.descrizione = infoDocSel.oggetto.ToString();
                            }
                            else
                            {
                                DocsPaWR.Oggetto ogg = new DocsPaWR.Oggetto();
                                ogg.descrizione = infoDocSel.oggetto.ToString();
                                schedaDocIngresso.oggetto = ogg;
                            }

                            if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                            {
                                ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this);
                            }

                            //metto in sessione la scheda documento con le informazioni del protocollo a cui risponde
                            this.DocumentWIP = schedaDocIngresso;
                            this.CloseMask(true);

                            this.AvviseProtocol.ReturnValue = "";
                        }

                        break;

                    case "S":
                        //non posso modificare il mittente o oggetto, quindi il pulsante continua
                        //si limiterà a popolare il campo risposta al protocollo con l'infoDoc corrente
                        if (schedaDocIngresso != null && schedaDocIngresso.protocollo != null)
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocIngresso.rispostaDocumento = infoDocSel;
                                schedaDocIngresso.modificaRispostaDocumento = true;
                            }
                        }
                        this.DocumentWIP = schedaDocIngresso;
                        this.CloseMask(true);

                        break;
                }
            }
            catch
            {

            }

            return retValue;
        }

        protected void rbl_TipoDoc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try {
                plc_countRecord.Visible = false;
                grdList.Visible = false;
                pnl_corr.Visible = false;
                string searchType = null;

                if (this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected || this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
                {
                    plc_proto.Visible = true;
                    plc_doc.Visible = false;
                    searchType = "Protocollo";
                    clearSearchDoc(searchType);
                }

                if (this.rbl_TipoDoc.Items.FindByValue("Interno") != null)
                {
                    if (this.rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                    {
                        plc_proto.Visible = true;
                        plc_doc.Visible = false;
                        searchType = "Protocollo";
                        clearSearchDoc(searchType);
                    }
                }

                if (this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || this.rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                    plc_proto.Visible = false;
                    plc_doc.Visible = true;
                    searchType = "Documento";
                    clearSearchDoc(searchType);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            try {
                this.CloseMask(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// VERIFICA SE è STATO SELEZIONATO un documento
        /// </summary>
        /// <returns></returns>
        private bool verificaSelezioneDocumento()
        {
            bool verificaSelezione = false;

            if (this.grdList.SelectedIndex >= 0)
                verificaSelezione = true;

            return verificaSelezione;
        }

        /// <summary>
        /// VERIFICA SE è STATO SELEZIONATO ALMENO UNA OPZIONE (CORRISPONDENTE) NEL PANNELLO pnl_corr
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private bool verificaSelezione(out int itemIndex)
        {
            itemIndex = this.SelectedCorrIndex;

            return (itemIndex>-1);
        }

        /// <summary>
        /// VERIFICA SE è STATO SELEZIONATO ALMENO UNA OPZIONE (CORRISPONDENTE) NEL PANNELLO pnl_corr
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private bool verificaSelezioneDocumento(out int itemIndex)
        {
            itemIndex = this.SelectedRowIndex;

            return (itemIndex > -1);
        }

        /// <summary>
        /// VERIFICA se il mittente del protocollo in ingresso coincide con il destinatario selezionato
        /// del protocollo in uscita a cui si sta rispondendo
        /// </summary>
        /// <param name="destSelected"></param>
        /// <param name="mittCorrente"></param>
        /// <returns></returns>
        private bool verificaUguaglianzacorrispondenti(DocsPaWR.Corrispondente destSelected, DocsPaWR.Corrispondente mittCorrente)
        {
            bool verificaUguaglianza = false;
            if (destSelected.systemId == mittCorrente.systemId)
            {
                verificaUguaglianza = true;
            }
            return verificaUguaglianza;
        }

        private bool verificaUguaglianzacorrispondenti(DocsPaWR.Corrispondente[] destSelected, DocsPaWR.Corrispondente[] destCorrenti)
        {
            bool verificaUguaglianza = false;

            foreach (DocsPaWR.Corrispondente dc in destCorrenti)
            {
                foreach (DocsPaWR.Corrispondente ds in destSelected)
                {
                    if (ds.systemId == dc.systemId)
                    {
                        verificaUguaglianza = true;
                    }
                }
            }

            return verificaUguaglianza;
        }

        private DocsPaWR.SchedaDocumento CopiaCorrispondenti(DocsPaWR.SchedaDocumento schedaDocUscita)
        {
            DocsPaWR.Corrispondente[] listaCorrIng = null;
            if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
            {
                listaCorrIng[0] = RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this);
                ArrayList listDest = new ArrayList();
                if (((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari != null)
                {
                    foreach (DocsPaWR.Corrispondente corrDocIng in listaCorrIng)
                    {
                        bool trovato = false;
                        foreach (DocsPaWR.Corrispondente corrDocUscita in ((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari)
                        {
                            if (corrDocIng.systemId == corrDocUscita.systemId)
                                trovato = true;
                        }
                        if (!trovato)
                            listDest.Add(corrDocIng);
                    }
                    listDest.CopyTo(((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari);
                }
            }
            return schedaDocUscita;
        }

        protected void BtnOk_Click(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                DocsPaWR.Corrispondente destSelected = null;
                bool avanzaDoc;
                bool diventaOccasionale = true;

                //itemIndex: indice item del datagrid che contiene radio selezionato
                int itemIndex = -1;
            #region PROTOCOLLO PARTENZA
            if (this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
            {
                /*	bool avanza:
                 *	- true: si può procedere perchè un corrispondente è stato selezionato;		
                 *  - false : non si procede e si avvisa l'utente che deve selezionarne uno */
                avanzaDoc = this.verificaSelezioneDocumento();
                if (avanzaDoc)
                {
                    bool avanzaCor = verificaSelezione(out itemIndex);
                    DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());

                    if (avanzaCor || infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G") || infoDocumentoLav.tipoProto.Equals("P") || (infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I") && infoDocumentoLav.docNumber != null && string.IsNullOrEmpty(infoDocumentoLav.segnatura)))
                    {
                        bool mittenteOK = false;
                        bool oggettoOK = false;
                        string oggettoProtoSel = "";

                        schedaDocUscita = this.DocumentWIP;

                        //NEL CASO IN CUI PARTO DA UN DOCUMENTO GRIGIO O RISPONDO A UNO IN PARTENZA CON UNO IN PARTENZA
                        if (infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G") || infoDocumentoLav.tipoProto.Equals("P") || ((infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I")) && infoDocumentoLav.docNumber != null && string.IsNullOrEmpty(infoDocumentoLav.segnatura)))
                        {
                            verificaSelezioneDocumento(out itemIndex);

                            if (itemIndex > -1)
                            {
                                this.infoDocSel = (DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                            }


                            if (schedaDocUscita != null)
                            {
                                if (this.grdList.SelectedIndex >= 0)
                                {
                                    oggettoProtoSel = this.infoDocSel.oggetto;
                                }
                                //inizio verifica congruenza campo oggetto
                                if (schedaDocUscita.docNumber != null)
                                {
                                    if (schedaDocUscita.oggetto != null && !string.IsNullOrEmpty(schedaDocUscita.oggetto.descrizione))
                                    {
                                        if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                        {
                                            if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                            {
                                                oggettoOK = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    oggettoOK = true;
                                    schedaDocUscita.oggetto.descrizione = oggettoProtoSel;

                                }
                                if (infoDocumentoLav.tipoProto != null)
                                {
                                    if (infoDocumentoLav.tipoProto.Equals("P") && schedaDocUscita.protocollo != null && !oggettoOK)
                                    {
                                        if (!string.IsNullOrEmpty(schedaDocUscita.protocollo.segnatura))
                                        {
                                            //se i corrisp non coincidono si lancia un avviso	all'utente 
                                            this.AnswerProtocolSender = mittenteOK.ToString();
                                            this.AnswerProtocolSubject = oggettoOK.ToString();
                                            this.AnswerProtocolIsProtocol = true.ToString();
                                            this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                        }
                                    }
                                    bool first_step = false;
                                    if (infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G") || infoDocumentoLav.tipoProto.Equals("R") || infoDocumentoLav.tipoProto.Equals("C") || ((infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I")) && infoDocumentoLav.docNumber != null && string.IsNullOrEmpty(infoDocumentoLav.segnatura)))
                                    {
                                        first_step = true;
                                        if (!oggettoOK)
                                        {
                                            if (!infoDocSel.segnatura.Equals(""))
                                            {
                                                this.AnswerProtocolSender = null;
                                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                                this.AnswerProtocolIsProtocol = false.ToString();
                                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                            }
                                            else
                                            {
                                                this.AnswerProtocolSender = null;
                                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                                this.AnswerProtocolIsProtocol = true.ToString();
                                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                            }
                                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                        }
                                        else
                                        {
                                            if (this.infoDocSel != null)
                                            {
                                                schedaDocUscita.rispostaDocumento = this.infoDocSel;
                                                schedaDocUscita.modificaRispostaDocumento = true;
                                            }

                                            this.DocumentWIP = schedaDocUscita;
                                            this.CloseMask(true);
                                        }
                                    }
                                    if (first_step == false)
                                    {
                                        if (infoDocumentoLav.segnatura != null)
                                        {
                                            if (infoDocumentoLav.tipoProto.Equals("P") && string.IsNullOrEmpty(infoDocumentoLav.segnatura))
                                            {
                                                if (!oggettoOK)
                                                {
                                                    if (!infoDocSel.segnatura.Equals(""))
                                                    {
                                                        this.AnswerProtocolSender = null;
                                                        this.AnswerProtocolSubject = oggettoOK.ToString();
                                                        this.AnswerProtocolIsProtocol = false.ToString();
                                                        this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                    }
                                                    else
                                                    {
                                                        this.AnswerProtocolSender = null;
                                                        this.AnswerProtocolSubject = oggettoOK.ToString();
                                                        this.AnswerProtocolIsProtocol = true.ToString();
                                                        this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                    }
                                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                                }
                                                else
                                                {
                                                    if (this.infoDocSel != null)
                                                    {
                                                        schedaDocUscita.rispostaDocumento = this.infoDocSel;
                                                        schedaDocUscita.modificaRispostaDocumento = true;
                                                    }

                                                    this.DocumentWIP = schedaDocUscita;
                                                    this.CloseMask(true);
                                                }

                                            }
                                            else
                                            {
                                                if (!oggettoOK)
                                                {
                                                    if (!infoDocSel.segnatura.Equals(""))
                                                    {
                                                        this.AnswerProtocolSender = null;
                                                        this.AnswerProtocolSubject = oggettoOK.ToString();
                                                        this.AnswerProtocolIsProtocol = false.ToString();
                                                        this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                    }
                                                    else
                                                    {
                                                        this.AnswerProtocolSender = null;
                                                        this.AnswerProtocolSubject = oggettoOK.ToString();
                                                        this.AnswerProtocolIsProtocol = true.ToString();
                                                        this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                    }
                                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                                }
                                                else
                                                {
                                                    if (this.infoDocSel != null)
                                                    {
                                                        schedaDocUscita.rispostaDocumento = this.infoDocSel;
                                                        schedaDocUscita.modificaRispostaDocumento = true;
                                                    }

                                                    this.DocumentWIP = schedaDocUscita;
                                                    this.CloseMask(true);
                                                }

                                            }

                                        }
                                        else
                                        {
                                            if (this.infoDocSel != null)
                                            {
                                                schedaDocUscita.rispostaDocumento = this.infoDocSel;
                                                schedaDocUscita.modificaRispostaDocumento = true;
                                            }

                                            this.DocumentWIP = schedaDocUscita;
                                            this.CloseMask(true);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!oggettoOK)
                                    {
                                        //se i corrisp non coincidono si lancia un avviso	all'utente 
                                        if (!infoDocSel.segnatura.Equals(""))
                                        {
                                            this.AnswerProtocolSender = null;
                                            this.AnswerProtocolSubject = oggettoOK.ToString();
                                            this.AnswerProtocolIsProtocol = false.ToString();
                                            this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                        }
                                        else
                                        {
                                            this.AnswerProtocolSender = null;
                                            this.AnswerProtocolSubject = oggettoOK.ToString();
                                            this.AnswerProtocolIsProtocol = true.ToString();
                                            this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                        }
                                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                    }
                                    else
                                    {
                                        if (this.infoDocSel != null)
                                        {
                                            schedaDocUscita.rispostaDocumento = this.infoDocSel;
                                            schedaDocUscita.modificaRispostaDocumento = true;
                                        }

                                        this.DocumentWIP = schedaDocUscita;
                                        this.CloseMask(true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (avanzaCor)
                            {
                                //ricavo la system_id del corrispondente selezionato, contenuta nella prima colonna del datagrid
                                string key = this.grdCorr.Rows[itemIndex].Cells[0].Text;

                                //prendo la hashTable che contiene i corrisp dalla sessisone
                                if (ht_destinatariTO_CC != null)
                                {
                                    if (ht_destinatariTO_CC.ContainsKey(key))
                                    {
                                        //prendo il corrispondente dalla hashTable secondo quanto è stato richiesto dall'utente
                                        destSelected = (DocsPaWR.Corrispondente)ht_destinatariTO_CC[key];

                                        #region CHIARIMENTO
                                        /*	CASI POSSIBILI PER RISPONDERE A UN PROTOCOLLO	
							*			----------------------------------------------------------------------------------
							*			caso 1 - il documento in ingresso (il protocollo di risposta) non è protocollato
							*						
							*					caso 1.1 - il campo mittente non è valorizzato, quindi posso
							*							   popolarlo con quello selezionato dall'utente
							*					
							*					caso 1.2 - il campo mittente è popolato, se sono diversi avviso l'utente
							*			
							*			Analogo il discorso per il campo oggetto, se sono diversi avviso l'utente e gli
							*			daremo la possibilità di scegliere se proseguire o meno con l'operazione
							*			di collegamento	
							* 		   
							*			-----------------------------------------------------------------------------------
							*			caso 2 - il documento in ingresso è già protocollato, ha quindi un mittente
							*					
							*					caso 2.1 - mittente scelto è diverso da quello corrente, si avvisa l'utente
							*		     				   e si da la possibilità si proseguire o meno con il collegamento
							* 
							*			Analogo il discorso per il campo oggetto
							*			-----------------------------------------------------------------------------------
							* */
                                        #endregion

                                        //prendo la scheda documento corrente in sessione
                                        schedaDocIngresso = this.DocumentWIP;

                                        //Gestione corrispondenti nel caso di Corrispondenti extra AOO
                                        if (UserManager.isFiltroAooEnabled())
                                        {

                                            if (schedaDocIngresso != null)
                                            {
                                                DocsPaWR.Registro tempRegUser = UserManager.getRegisterSelected();
                                                if (tempRegUser!=null && tempRegUser.systemId != this.infoDocSel.idRegistro && !string.IsNullOrEmpty(destSelected.idRegistro))
                                                {
                                                    DocsPaWR.Corrispondente tempDest = destSelected;
                                                    tempDest.codiceRubrica = null;
                                                    tempDest.idOld = "0";
                                                    tempDest.systemId = null;
                                                    tempDest.tipoCorrispondente = "O";
                                                    tempDest.tipoIE = null;
                                                    tempDest.idRegistro = tempRegUser.systemId;
                                                    tempDest.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                                                    diventaOccasionale = false;
                                                    destSelected = tempDest;
                                                }
                                            }
                                        }

                                        if (schedaDocIngresso != null)
                                        {
                                            #region GESTIONE RISPOSTA AL PROTOCOLLO NON ANCORA PROTOCOLLATA
                                            //se non è protocollato
                                            if (schedaDocIngresso.protocollo != null && string.IsNullOrEmpty(schedaDocIngresso.protocollo.numero))
                                            {
                                                if (!this.schedaDocIngresso.tipoProto.Equals("P") && !this.schedaDocIngresso.tipoProto.Equals("I"))
                                                {
                                                    // il campo mittente della scheda documento relativa al protocollo di risposta non è popolato
                                                    if (((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente == null)
                                                    {
                                                        mittenteOK = true;
                                                        //popolo il campo mittente con il destinatario selezionato dal protocollo a cui ri risponde 
                                                        RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                    }
                                                    else
                                                    {
                                                        //il campo mittente è stato popolato prima di selezionere il protocollo a cui rispondere,
                                                        //quindi si verifica se i corrispondenti coincidono. In caso affermativo si procede con il collegamento,
                                                        //in caso contrario avviso l'utente, dando la possibilità di scegliere se proseguire con i nuovi dati o meno.

                                                        //per i mittenti occasionali come si fa ?
                                                        bool verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                                        if (verificaUguaglianzaCorr)
                                                        {
                                                            mittenteOK = true;
                                                        }
                                                        else
                                                        {
                                                            //setto il destinatario selezionato in sessione
                                                            RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    mittenteOK = true;
                                                }

                                                if (this.grdList.SelectedIndex >= 0)
                                                {
                                                    oggettoProtoSel = this.grdList.Rows[this.grdList.SelectedIndex].Cells[2].Text;
                                                    oggettoProtoSel = this.infoDocSel.oggetto;
                                                }
                                                //inizio verifica congruenza campo oggetto
                                                if (schedaDocIngresso.oggetto != null && !string.IsNullOrEmpty(schedaDocIngresso.oggetto.descrizione))
                                                {
                                                    if (!string.IsNullOrEmpty(oggettoProtoSel))
                                                    {
                                                        if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                        {
                                                            oggettoOK = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    oggettoOK = true;
                                                    if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                    {
                                                        DocsPaWR.Oggetto ogg = new DocsPaWR.Oggetto();
                                                        ogg.descrizione = oggettoProtoSel.ToString();
                                                        schedaDocIngresso.oggetto = ogg;
                                                    }
                                                }

                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }

                                                if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected && schedaDocIngresso.tipoProto.Equals("A"))
                                                {
                                                    if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                                                    {
                                                        ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;
                                                    }
                                                }

                                                if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected && schedaDocIngresso.tipoProto.Equals("I"))
                                                {
                                                    ((DocsPaWR.ProtocolloInterno)schedaDocIngresso.protocollo).mittente = destSelected;
                                                }

                                                this.DocumentWIP = schedaDocIngresso;

                                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);

                                                if (!diventaOccasionale)
                                                {
                                                    mittenteOK = true;
                                                    oggettoOK = true;

                                                    this.AnswerProtocolSender = mittenteOK.ToString();
                                                    this.AnswerProtocolSubject = oggettoOK.ToString();
                                                    this.AnswerProtocolIsProtocol = false.ToString();
                                                    this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                                }
                                                else
                                                {
                                                    this.CloseMask(true);
                                                }
                                            }
                                        }


                                        #endregion

                                        #region GESTIONE RISPOSTA AL PROTOCOLLO GIA' PRECEDENTEMENTE PROTOCOLLATA

                                        if (schedaDocIngresso.protocollo != null && !string.IsNullOrEmpty(schedaDocIngresso.protocollo.numero))
                                        {
                                            bool verificaUguaglianzaCorr = false;
                                            //INSERITO PER BUG SE DA UN PROTOCOLLO INTERNO CERCO UNO IN PARTENZA
                                            if (schedaDocIngresso.tipoProto.Equals("I"))
                                            {
                                                verificaUguaglianzaCorr = false;
                                            }
                                            else
                                            {
                                                if (schedaDocIngresso.protocollo.GetType() == typeof(DocsPaWR.ProtocolloEntrata))
                                                {
                                                    verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                                }
                                                else
                                                {
                                                    foreach (Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocIngresso.protocollo).destinatari)
                                                        if (verificaUguaglianzacorrispondenti(destSelected, c)) {
                                                            verificaUguaglianzaCorr = true;
                                                            break;
                                                        }
                                                }
                                            }
                                            if (verificaUguaglianzaCorr)
                                            {
                                                mittenteOK = true;
                                            }
                                            if (this.grdList.SelectedIndex >= 0)
                                            {
                                                oggettoProtoSel = this.infoDocSel.oggetto;
                                            }
                                            //inizio verifica congruenza campo oggetto
                                            if (schedaDocIngresso.oggetto != null && !string.IsNullOrEmpty(schedaDocIngresso.oggetto.descrizione))
                                            {
                                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                                {
                                                    if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                    {
                                                        oggettoOK = true;
                                                    }
                                                }
                                            }
                                            if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                                            {
                                                //se i corrisp non coincidono si lancia un avviso	all'utente 
                                                this.AnswerProtocolSender = mittenteOK.ToString();
                                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                                this.AnswerProtocolIsProtocol = true.ToString();
                                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                            }
                                            else
                                            {
                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }

                                                this.DocumentWIP = schedaDocIngresso;
                                                this.CloseMask(true);
                                            }
                                        #endregion
                                        }
                                    }
                                    else
                                    {
                                        //se entro qui è perchè si è verificato errore
                                        throw new Exception("Errore nella gestione dei corrispondenti nella risposta al protocollo");
                                    }
                                }
                            }
                            else
                            {
                                //avviso l'utente che non ha selezionato nessun corrispondente
                                string language = UIManager.UserManager.GetUserLanguage();
                                string msg = "ChainsNoCorrespondentSelected";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                        }
                    }
                    else
                    {
                        //avviso l'utente che non ha selezionato nessun corrispondente
                        string language = UIManager.UserManager.GetUserLanguage();
                        string msg = "ChainsNoCorrespondentSelected";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                    }
                }
                else
                {
                    //avviso l'utente che non ha selezionato nulla
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msg = "ChainsAnswerNoDocumentSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
            #endregion

            #region PROTOCOLLO ARRIVO
            if (this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected)
            {
                avanzaDoc = verificaSelezioneDocumento(out itemIndex);
                if (avanzaDoc)
                {
                    bool mittenteOK = false;
                    bool oggettoOK = false;
                    string oggettoProtoSel = "";

                    ArrayList listDest = new ArrayList();
                    //prendo il mittente del documento selezionato
                    DocsPaWR.Corrispondente CorrMitt = null;

                    if (itemIndex > -1)
                    {
                        infoDocSel = (DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                    }

                    if (infoDocSel != null)
                    {
                        //prendo il dettaglio del documento e estraggo il mittente del protocollo
                        DocsPaWR.SchedaDocumento schedaDocIn = new DocsPaWR.SchedaDocumento();
                        schedaDocIn = DocumentManager.getDocumentDetails(this, infoDocSel.idProfile, infoDocSel.docNumber);
                        //prendo il mittente
                        CorrMitt = ((DocsPaWR.ProtocolloEntrata)schedaDocIn.protocollo).mittente;
                        listDest.Add(CorrMitt);

                    }
                    #region CHIARIMENTO
                    /*	CASI POSSIBILI PER RISPONDERE A UN PROTOCOLLO	
							*			----------------------------------------------------------------------------------
							*			caso 1 - il documento in uscita (il protocollo di risposta) non è protocollato
							*						
							*					caso 1.1 - il campo destinatario viene popolato con il mittente 
							*		                    del documento selezionato
							*					
							*					
							*			
							*			Analogo il discorso per il campo oggetto, se sono diversi avviso l'utente e gli
							*			daremo la possibilità di scegliere se proseguire o meno con l'operazione
							*			di collegamento	
							* 		   
							*			-----------------------------------------------------------------------------------
							*			caso 2 - il documento in uscita è già protocollato
							*					
							*					caso 2.1 - destinatario scelto è diverso da quello corrente, si avvisa l'utente
							*		     				   e si da la possibilità si proseguire o meno con il collegamento
							* 
							*			Analogo il discorso per il campo oggetto
							*			-----------------------------------------------------------------------------------
							* */
                    #endregion

                    //prendo la scheda documento corrente in sessione
                    schedaDocUscita = this.DocumentWIP;

                    //Gestione corrispondenti nel caso di Corrispondenti extra AOO
                    if (UserManager.isFiltroAooEnabled())
                    {

                        if (schedaDocUscita != null)
                        {
                            DocsPaWR.Registro tempRegUser = UserManager.getRegisterSelected();
                            if (tempRegUser!=null && tempRegUser.systemId != infoDocSel.idRegistro && !string.IsNullOrEmpty(CorrMitt.idRegistro))
                            {
                                DocsPaWR.Corrispondente tempDest = CorrMitt;
                                tempDest.codiceRubrica = null;
                                tempDest.idOld = "0";
                                tempDest.systemId = null;
                                tempDest.tipoCorrispondente = "O";
                                tempDest.tipoIE = null;
                                tempDest.idRegistro = tempRegUser.systemId;
                                tempDest.idAmministrazione = UserManager.GetUserInSession().idAmministrazione;
                                diventaOccasionale = false;
                                CorrMitt = tempDest;
                            }
                        }
                    }

                    if (schedaDocUscita != null)
                    {
                        #region GESTIONE RISPOSTA AL PROTOCOLLO NON ANCORA PROTOCOLLATA
                        //se non è protocollato
                        if (schedaDocUscita.protocollo != null && string.IsNullOrEmpty(schedaDocUscita.protocollo.numero))
                        {
                            if (!schedaDocUscita.tipoProto.Equals("A"))
                            {
                                if (((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari == null)
                                {
                                    mittenteOK = true;

                                }
                                else
                                {
                                    //aggiungo il mittente ai destinatari già presenti
                                    DocsPaWR.Corrispondente[] corr = ((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari;
                                    foreach (DocsPaWR.Corrispondente c in corr)
                                    {
                                        listDest.Add(c);
                                    }

                                    mittenteOK = true;
                                }
                            }
                            else
                            {
                                mittenteOK = true;
                            }

                            DocsPaWR.Corrispondente[] corrispondenti = new DocsPaWR.Corrispondente[listDest.Count];
                            listDest.CopyTo(corrispondenti);
                            RicercaDocumentiSessionMng.setCorrispondenteRispostaINGRESSO(this, corrispondenti);

                            if (itemIndex >= 0)
                            {
                                oggettoProtoSel = this.infoDocSel.oggetto;
                            }
                            //inizio verifica congruenza campo oggetto
                            if (schedaDocUscita.oggetto != null && !string.IsNullOrEmpty(schedaDocUscita.oggetto.descrizione))
                            {
                                if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                {
                                    if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                    {
                                        oggettoOK = true;
                                    }
                                }
                            }
                            else
                            {

                                oggettoOK = true;
                                if (!string.IsNullOrEmpty(oggettoProtoSel))
                                {
                                    DocsPaWR.Oggetto ogg = new DocsPaWR.Oggetto();
                                    ogg.descrizione = oggettoProtoSel.ToString();
                                    schedaDocUscita.oggetto = ogg;
                                }
                            }

                            if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                            {
                                //se i corrisp non coincidono si lancia un avviso	all'utente 
                                this.AnswerProtocolSender = mittenteOK.ToString();
                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                this.AnswerProtocolIsProtocol = false.ToString();
                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                            }
                            else
                            {

                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                schedaDocUscita.modificaRispostaDocumento = true;

                                if (!schedaDocUscita.tipoProto.Equals("A"))
                                {
                                    if (RicercaDocumentiSessionMng.getCorrispondenteRispostaINGRESSO(this) != null)
                                    {
                                        schedaDocUscita = CopiaCorrispondenti(schedaDocUscita);
                                    }
                                }

                                if (schedaDocUscita.tipoProto.Equals("P"))
                                {
                                    DocsPaWR.Corrispondente[] corrispondentiTemp = new DocsPaWR.Corrispondente[1];
                                    corrispondentiTemp[0] = CorrMitt;
                                    ((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari = corrispondentiTemp;
                                }

                                if (schedaDocUscita.tipoProto.Equals("I"))
                                {
                                    DocsPaWR.Corrispondente[] corrispondentiTemp = new DocsPaWR.Corrispondente[1];
                                    corrispondentiTemp[0] = CorrMitt;
                                    ((DocsPaWR.ProtocolloInterno)schedaDocUscita.protocollo).destinatari = corrispondentiTemp;
                                }

                                this.DocumentWIP = schedaDocUscita;

                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);

                                this.CloseMask(true);
                            }
                        }
                    }


                    #endregion

                    #region GESTIONE RISPOSTA AL PROTOCOLLO GIA' PRECEDENTEMENTE PROTOCOLLATA

                    if (schedaDocUscita.protocollo != null && !string.IsNullOrEmpty(schedaDocUscita.protocollo.numero))
                    {
                        DocsPaWR.Corrispondente[] corrispondenti = new DocsPaWR.Corrispondente[listDest.Count];
                        listDest.CopyTo(corrispondenti);


                        bool verificaUguaglianzaCorr = true;

                        if (schedaDocUscita.tipoProto.Equals("P"))
                            verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(corrispondenti, ((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari);
                        if (schedaDocUscita.tipoProto.Equals("A"))
                            verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(CorrMitt, ((DocsPaWR.ProtocolloEntrata)schedaDocUscita.protocollo).mittente);

                        if (verificaUguaglianzaCorr)
                        {
                            mittenteOK = true;
                        }
                        if (this.grdList.SelectedIndex >= 0)
                        {
                            oggettoProtoSel = this.infoDocSel.oggetto;
                        }
                        //inizio verifica congruenza campo oggetto
                        if (schedaDocUscita.oggetto != null && !string.IsNullOrEmpty(schedaDocUscita.oggetto.descrizione))
                        {
                            if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                            {
                                if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                {
                                    oggettoOK = true;
                                }
                            }
                        }
                        if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                        {
                            //se i corrisp non coincidono si lancia un avviso	all'utente 
                            this.AnswerProtocolSender = mittenteOK.ToString();
                            this.AnswerProtocolSubject = oggettoOK.ToString();
                            this.AnswerProtocolIsProtocol = true.ToString();
                            this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                        }
                        else
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                schedaDocUscita.modificaRispostaDocumento = true;
                            }

                            this.DocumentWIP = schedaDocUscita;
                            this.CloseMask(true);
                        }
                    }
                    #endregion

                    #region GESTIONE RISPOSTA AD UN DOCUMENTO NON PROTOCOLLATO

                    if (schedaDocUscita.tipoProto.ToUpper().Equals("N") || schedaDocUscita.tipoProto.Equals("NP") || schedaDocUscita.tipoProto.Equals("G"))
                    {
                        if (this.grdList.SelectedIndex >= 0)
                        {
                            oggettoProtoSel = this.infoDocSel.oggetto;
                        }
                        //inizio verifica congruenza campo oggetto
                        if (schedaDocUscita.docNumber != null)
                        {
                            if (schedaDocUscita.oggetto != null && !string.IsNullOrEmpty(schedaDocUscita.oggetto.descrizione))
                            {
                                if (!string.IsNullOrEmpty(oggettoProtoSel))
                                {
                                    if (schedaDocUscita.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                    {
                                        oggettoOK = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            oggettoOK = true;
                            schedaDocUscita.oggetto.descrizione = oggettoProtoSel;
                        }
                        if (!oggettoOK)
                        {
                            //se i corrisp non coincidono si lancia un avviso	all'utente 
                            if (!infoDocSel.segnatura.Equals(""))
                            {
                                this.AnswerProtocolSender = null;
                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                this.AnswerProtocolIsProtocol = false.ToString();
                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                            }
                            else
                            {
                                this.AnswerProtocolSender = null;
                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                this.AnswerProtocolIsProtocol = true.ToString();
                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                            }
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                        }
                        else
                        {
                            if (infoDocSel != null)
                            {
                                schedaDocUscita.rispostaDocumento = infoDocSel;
                                schedaDocUscita.modificaRispostaDocumento = true;
                            }

                            this.DocumentWIP = schedaDocUscita;
                            this.CloseMask(true);
                        }
                    }

                    #endregion
                }
                else
                {
                    //avviso l'utente che non ha selezionato nulla
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msg = "ChainsAnswerNoDocumentSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
            #endregion

            #region DOCUMENTI NON PROTOCOLLATI
            if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
            {
                if (verificaSelezioneDocumento(out itemIndex))
                {
                    //prendo la scheda documento corrente in sessione
                    schedaDoc = this.DocumentWIP;
                    if (schedaDoc != null)
                    {
                        if (itemIndex > -1)
                        {
                            infoDocSel = (DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                        }
                        infoDocSel.isCatenaTrasversale = "1";
                        if (infoDocSel != null)
                        {
                            schedaDoc.rispostaDocumento = infoDocSel;
                            schedaDoc.modificaRispostaDocumento = true;
                            if (infoDocSel.oggetto != null && string.IsNullOrEmpty(schedaDoc.oggetto.descrizione))
                            {
                                schedaDoc.oggetto.descrizione = infoDocSel.oggetto;
                            }
                        }
                        this.DocumentWIP = schedaDoc;
                        this.CloseMask(true);
                    }

                }
                else
                {
                    //avviso l'utente che non ha selezionato nulla
                    string language = UIManager.UserManager.GetUserLanguage();
                    string msg = "ChainsAnswerNoDocumentSelected";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                }
            }
            #endregion

            #region PROTOCOLLO INTERNO

            if (rbl_TipoDoc.Items.FindByValue("Interno") != null)
            {
                if (rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                {
                    DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());

                    if (infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G") || infoDocumentoLav.tipoProto.Equals("I") || infoDocumentoLav.tipoProto.Equals("R") || infoDocumentoLav.tipoProto.Equals("C") || ((infoDocumentoLav.tipoProto.Equals("P") || infoDocumentoLav.tipoProto.Equals("A")) && infoDocumentoLav.docNumber != null))
                    {
                        if (verificaSelezioneDocumento(out itemIndex))
                        {
                            bool oggettoOK = false;
                            string oggettoProtoSel = "";

                            //prendo la scheda documento corrente in sessione
                            schedaDoc = this.DocumentWIP;
                            if (schedaDoc != null)
                            {
                                if (itemIndex > -1)
                                {
                                    infoDocSel = (DocsPaWR.InfoDocumento)this.infoDoc[itemIndex];
                                    oggettoProtoSel = this.infoDocSel.oggetto;
                                }

                                //inizio verifica congruenza campo oggetto
                                if (schedaDoc.docNumber != null)
                                {
                                    if (schedaDoc.oggetto != null && !string.IsNullOrEmpty(schedaDoc.oggetto.descrizione))
                                    {
                                        if (oggettoProtoSel != null && oggettoProtoSel != String.Empty)
                                        {
                                            if (schedaDoc.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                            {
                                                oggettoOK = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    oggettoOK = true;
                                    schedaDoc.oggetto.descrizione = oggettoProtoSel;

                                }

                                if (oggettoOK)
                                {
                                    infoDocSel.isCatenaTrasversale = "1";
                                    if (infoDocSel != null)
                                    {
                                        if (infoDocumentoLav.tipoProto.Equals("I") || infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("G"))
                                        {
                                            if (schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.protocollo.segnatura))
                                            {
                                                //CASO IN CUI IL PROTOCOLLO DI PARTENZA E' PROTOCOLLATO (POSSIBILE QUALCOSA IN SEGUITO VERRA' AGGIUNTO
                                            }
                                            else
                                            {
                                                DocsPaWR.SchedaDocumento intDoc = DocumentManager.getDocumentDetails(this, infoDocSel.idProfile, infoDocSel.docNumber);

                                                //PRENDO IL MITTENTE DEL PROTOCOLLO INTERNO DI PARTENZA
                                                DocsPaWR.Corrispondente corrispondenteMitt = ((DocsPaWR.ProtocolloInterno)intDoc.protocollo).mittente;

                                                DocsPaWR.Corrispondente corrispondenteMittAttuale = null;

                                                if (schedaDoc.protocollo != null)
                                                {
                                                    //PRENDO IL MITTENTE DEL PROTOCOLLO INTERNO DI NUOVA CREAZIONE
                                                    if (((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente != null)
                                                    {
                                                        corrispondenteMittAttuale = ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).mittente;
                                                    }

                                                    //UNISCO I DESTINATARI DEL PROTOCOLLO DI PARTENZA CON QUELLO CONCATENATO SOLO SE NON PRESENTI
                                                    if (((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari != null)
                                                    {
                                                        foreach (DocsPaWR.Corrispondente corr in ((DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatari)
                                                        {
                                                            if (!UserManager.esisteCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corr))
                                                            {
                                                                ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corr);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = ((DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatari;
                                                    }

                                                    //AGGIUNGO IL MITTENTE AI DESTINATARI SE NON PRESENTE
                                                    if (!UserManager.esisteCorrispondente(((DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatari, corrispondenteMitt))
                                                    {
                                                        ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.addCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corrispondenteMitt);
                                                    }

                                                    int cancellaDest = -1;

                                                    if (corrispondenteMittAttuale != null)
                                                    {

                                                        for (int i = 0; i < (((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari).Length; i++)
                                                        {
                                                            DocsPaWR.Corrispondente temp = ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari[i];
                                                            if (temp.systemId.Equals(corrispondenteMittAttuale.systemId))
                                                            {
                                                                cancellaDest = i;
                                                                break;
                                                            }
                                                        }

                                                        if (cancellaDest != -1)
                                                        {
                                                            //SE IL MITTENTE è presente nei destinatari lo elimino
                                                            ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari = UserManager.removeCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, cancellaDest);
                                                        }
                                                    }

                                                    if (((DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza != null && ((DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza.Length > 0)
                                                    {
                                                        foreach (DocsPaWR.Corrispondente corr in ((DocsPaWR.ProtocolloInterno)intDoc.protocollo).destinatariConoscenza)
                                                        {
                                                            if (!UserManager.esisteCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatari, corr) && !UserManager.esisteCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, corr))
                                                            {
                                                                ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = UserManager.addCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, corr);
                                                            }
                                                        }

                                                        cancellaDest = -1;

                                                        for (int i = 0; i < (((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza).Length; i++)
                                                        {
                                                            DocsPaWR.Corrispondente temp = ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza[i];
                                                            if (temp.systemId.Equals(corrispondenteMitt.systemId))
                                                            {
                                                                cancellaDest = i;
                                                                break;
                                                            }
                                                        }

                                                        if (cancellaDest != -1)
                                                        {
                                                            //SE IL MITTENTE ATTUALE è presente nei destinatari lo elimino
                                                            ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, cancellaDest);
                                                        }

                                                        if (corrispondenteMittAttuale != null)
                                                        {
                                                            cancellaDest = -1;

                                                            for (int i = 0; i < (((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza).Length; i++)
                                                            {
                                                                DocsPaWR.Corrispondente temp = ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza[i];
                                                                if (temp.systemId.Equals(corrispondenteMittAttuale.systemId))
                                                                {
                                                                    cancellaDest = i;
                                                                    break;
                                                                }
                                                            }

                                                            if (cancellaDest != -1)
                                                            {
                                                                //SE IL MITTENTE è presente nei destinatari lo elimino
                                                                ((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = UserManager.removeCorrispondente(((DocsPaWR.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza, cancellaDest);
                                                            }
                                                        }

                                                    }

                                                }

                                            }

                                        }

                                        schedaDoc.rispostaDocumento = infoDocSel;
                                        schedaDoc.modificaRispostaDocumento = true;
                                        if (infoDocSel.oggetto != null && string.IsNullOrEmpty(schedaDoc.oggetto.descrizione))
                                        {
                                            schedaDoc.oggetto.descrizione = infoDocSel.oggetto;
                                        }
                                    }

                                    this.DocumentWIP = schedaDoc;
                                    RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);
                                    this.CloseMask(true);
                                }
                                else
                                {
                                    // oggetti non coincidono
                                    if (!infoDocSel.segnatura.Equals(""))
                                    {
                                        this.AnswerProtocolSender = null;
                                        this.AnswerProtocolSubject = oggettoOK.ToString();
                                        this.AnswerProtocolIsProtocol = false.ToString();
                                        this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                    }
                                    else
                                    {
                                        this.AnswerProtocolSender = null;
                                        this.AnswerProtocolSubject = oggettoOK.ToString();
                                        this.AnswerProtocolIsProtocol = true.ToString();
                                        this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                    }
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                }
                            }
                        }
                        else
                        {
                            //avviso l'utente che non ha selezionato nulla
                            string language = UIManager.UserManager.GetUserLanguage();
                            string msg = "ChainsAnswerNoDocumentSelected";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                     }
                    //FINE PARTE DA DOCUMENTO GRIGIO
                    else
                    {
                        //RICHIAMATO DA PROTOCOLLO
                        avanzaDoc = verificaSelezioneDocumento();
                        if (avanzaDoc)
                        {
                            bool avanzaCor = verificaSelezione(out itemIndex);
                            if (avanzaCor)
                            {
                                bool mittenteOK = false;
                                bool oggettoOK = false;
                                string oggettoProtoSel = "";
                                //ricavo la system_id del corrispondente selezionato, contenuta nella prima colonna del datagrid
                                string key = this.grdCorr.Rows[itemIndex].Cells[0].Text;

                                //prendo la hashTable che contiene i corrisp dalla sesisone
                                if (ht_destinatariTO_CC != null)
                                {
                                    if (ht_destinatariTO_CC.ContainsKey(key))
                                    {
                                        //prendo il corrispondente dalla hashTable secondo quanto è stato richiesto dall'utente
                                        destSelected = (DocsPaWR.Corrispondente)ht_destinatariTO_CC[key];
                                        //prendo la scheda documento corrente in sessione
                                        schedaDocIngresso = this.DocumentWIP;

                                        //Gestione corrispondenti nel caso di Corrispondenti extra AOO
                                        if (UserManager.isFiltroAooEnabled())
                                        {

                                            if (schedaDocIngresso != null)
                                            {
                                                DocsPaWR.Registro tempRegUser = UserManager.getRegisterSelected();
                                                if (tempRegUser!=null && tempRegUser.systemId != infoDocSel.idRegistro && !string.IsNullOrEmpty(destSelected.idRegistro))
                                                {
                                                    DocsPaWR.Corrispondente tempDest = destSelected;
                                                    tempDest.codiceRubrica = null;
                                                    tempDest.idOld = "0";
                                                    tempDest.systemId = null;
                                                    tempDest.tipoCorrispondente = "O";
                                                    tempDest.tipoIE = null;
                                                    tempDest.idRegistro = tempRegUser.systemId;
                                                    tempDest.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
                                                    diventaOccasionale = false;
                                                    destSelected = tempDest;
                                                }
                                            }
                                        }

                                        if (schedaDocIngresso != null)
                                        {
                                            #region GESTIONE RISPOSTA AL PROTOCOLLO NON ANCORA PROTOCOLLATA
                                            //se non è protocollato
                                            if (schedaDocIngresso.protocollo != null && string.IsNullOrEmpty(schedaDocIngresso.protocollo.numero))
                                            {
                                                if (!this.schedaDocIngresso.tipoProto.Equals("P") && !this.schedaDocIngresso.tipoProto.Equals("I"))
                                                {
                                                    // il campo mittente della scheda documento relativa al protocollo di risposta non è popolato
                                                    if (((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente == null)
                                                    {
                                                        mittenteOK = true;
                                                        //popolo il campo mittente con il destinatario selezionato dal protocollo a cui ri risponde 
                                                        RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                    }
                                                    else
                                                    {
                                                        //il campo mittente è stato popolato prima di selezionere il protocollo a cui rispondere,
                                                        //quindi si verifica se i corrispondenti coincidono. In caso affermativo si procede con il collegamento,
                                                        //in caso contrario avviso l'utente, dando la possibilità di scegliere se proseguire con i nuovi dati o meno.

                                                        //per i mittenti occasionali come si fa ?
                                                        bool verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                                        if (verificaUguaglianzaCorr)
                                                        {
                                                            mittenteOK = true;
                                                        }
                                                        else
                                                        {
                                                            //setto il destinatario selezionato in sessione
                                                            RicercaDocumentiSessionMng.setCorrispondenteRispostaUSCITA(this, destSelected);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    mittenteOK = true;
                                                }

                                                if (this.grdList.SelectedIndex >= 0)
                                                {
                                                    oggettoProtoSel = this.infoDocSel.oggetto;
                                                }
                                                //inizio verifica congruenza campo oggetto
                                                if (schedaDocIngresso.oggetto != null && !string.IsNullOrEmpty(schedaDocIngresso.oggetto.descrizione))
                                                {
                                                    if (!string.IsNullOrEmpty(oggettoProtoSel))
                                                    {
                                                        if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                        {
                                                            oggettoOK = true;
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    oggettoOK = true;
                                                    if (!string.IsNullOrEmpty(oggettoProtoSel))
                                                    {
                                                        DocsPaWR.Oggetto ogg = new DocsPaWR.Oggetto();
                                                        ogg.descrizione = oggettoProtoSel.ToString();
                                                        schedaDocIngresso.oggetto = ogg;
                                                    }
                                                }

                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }

                                                if (schedaDocIngresso.tipoProto.Equals("A"))
                                                {
                                                    if (RicercaDocumentiSessionMng.getCorrispondenteRispostaUSCITA(this) != null)
                                                    {
                                                        ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente = destSelected;
                                                    }
                                                }

                                                if (schedaDocIngresso.tipoProto.Equals("P"))
                                                {
                                                    ((DocsPaWR.ProtocolloUscita)schedaDocIngresso.protocollo).mittente = destSelected;
                                                }

                                                this.DocumentWIP = schedaDocIngresso;

                                                RicercaDocumentiSessionMng.SetDialogReturnValue(this, true);

                                                this.CloseMask(true);
                                            }
                                        }


                                        #endregion

                                        #region GESTIONE RISPOSTA AL PROTOCOLLO GIA' PRECEDENTEMENTE PROTOCOLLATA

                                        if (schedaDocIngresso.protocollo != null && !string.IsNullOrEmpty(schedaDocIngresso.protocollo.numero))
                                        {
                                            bool verificaUguaglianzaCorr = false;
                                            //INSERITO PER BUG SE DA UN PROTOCOLLO INTERNO CERCO UNO IN PARTENZA
                                            if (schedaDocIngresso.tipoProto.Equals("I"))
                                            {
                                                verificaUguaglianzaCorr = false;
                                            }
                                            else
                                            {
                                                if (schedaDocIngresso.protocollo.GetType() == typeof(DocsPaWR.ProtocolloEntrata))
                                                {
                                                    verificaUguaglianzaCorr = verificaUguaglianzacorrispondenti(destSelected, ((DocsPaWR.ProtocolloEntrata)schedaDocIngresso.protocollo).mittente);
                                                }
                                                else
                                                {
                                                    foreach (Corrispondente c in ((DocsPaWR.ProtocolloUscita)schedaDocIngresso.protocollo).destinatari)
                                                        if (verificaUguaglianzacorrispondenti(destSelected, c)) {
                                                            verificaUguaglianzaCorr = true;
                                                            break;
                                                        }
                                                }
                                            }
                                            if (verificaUguaglianzaCorr)
                                            {
                                                mittenteOK = true;
                                            }
                                            if (this.grdList.SelectedIndex >= 0)
                                            {
                                                oggettoProtoSel = this.infoDocSel.oggetto;
                                            }
                                            //inizio verifica congruenza campo oggetto
                                            if (schedaDocIngresso.oggetto != null && !string.IsNullOrEmpty(schedaDocIngresso.oggetto.descrizione))
                                            {
                                                if (!string.IsNullOrEmpty(oggettoProtoSel))
                                                {
                                                    if (schedaDocIngresso.oggetto.descrizione.ToUpper().Equals(oggettoProtoSel.ToUpper()))
                                                    {
                                                        oggettoOK = true;
                                                    }
                                                }
                                            }
                                            if (!mittenteOK || !oggettoOK || !diventaOccasionale)
                                            {
                                                //se i corrisp non coincidono si lancia un avviso	all'utente 
                                                this.AnswerProtocolSender = mittenteOK.ToString();
                                                this.AnswerProtocolSubject = oggettoOK.ToString();
                                                this.AnswerProtocolIsProtocol = true.ToString();
                                                this.AnswerProtocolIsOccasional = diventaOccasionale.ToString();
                                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxModalPopupAvviseProtocol", "ajaxModalPopupAvviseProtocol();", true);
                                            }
                                            else
                                            {
                                                if (infoDocSel != null)
                                                {
                                                    schedaDocIngresso.rispostaDocumento = infoDocSel;
                                                    schedaDocIngresso.modificaRispostaDocumento = true;
                                                }

                                                this.DocumentWIP = schedaDocIngresso;
                                                this.CloseMask(true);
                                            }
                                        #endregion
                                        }
                                    }
                                    else
                                    {
                                        //se entro qui è perchè si è verificato errore
                                        throw new Exception("Errore nella gestione dei corrispondenti nella risposta al protocollo");
                                    }
                                }

                            }
                            else
                            {
                                //avviso l'utente che non ha selezionato nessun corrispondente
                                string language = UIManager.UserManager.GetUserLanguage();
                                string msg = "ChainsNoCorrespondentSelected";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                            }
                        }
                        else
                        {
                            //avviso l'utente che non ha selezionato nulla
                            string language = UIManager.UserManager.GetUserLanguage();
                            string msg = "ChainsAnswerNoDocumentSelected";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');} else {parent.ajaxDialogModal('" + msg.Replace("'", @"\'") + "', 'warning', '');}", true);
                        }
                    }
                }
            }
            #endregion
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected virtual void RenderMessage(string message)
        {
            rowMessage.InnerHtml = message;
            rowMessage.Visible = true;
        }

        protected void CloseMask(bool withReturnValue)
        {
            //Response.Write("<html><body><script type=\"text/javascript\">if (parent.fra_main) {parent.fra_main.closeAjaxModal('AnswerSearchDocuments');} else {parent.closeAjaxModal('AnswerSearchDocuments');}</script></body></html>");
            //Response.End();

            this.ClearSessionData();

            string returnValue = "";
            if (withReturnValue) returnValue = "true";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('AnswerSearchDocuments', '"+returnValue+"');} else {parent.closeAjaxModal('AnswerSearchDocuments', '"+returnValue+"');};", true);
        }

        private void clearSearchDoc(string searchType)
        {
            if (searchType.Equals("Protocollo"))
            {
                txtInitDoc.Text = "";
                txtEndNumDoc.Text = "";
                txtInitDtaDoc.Text = "";
                txtEndDataDoc.Text = "";
                txtEndDataDoc.Text = "";
                txtInitDtaDoc.Text = "";
                ddl_numDoc.SelectedIndex = 0;
                ddl_dtaDoc.SelectedIndex = 0;
                pnlEndNumDoc.Visible = false;
                pnlEndDataDoc.Visible = false;
            }

            if (searchType.Equals("Documento"))
            {
                txtInitNumProto.Text = "";
                txtEndNumProto.Text = "";
                txt_annoProto.Text = "";
                txtInitDtaProto.Text = "";
                txtEndDataProtocollo.Text = "";
                ddl_numProto.SelectedIndex = 0;
                ddl_dtaProto.SelectedIndex = 0;
                pnlEndNumProto.Visible = false;
                pnlEndDataProtocollo.Visible = false;
            }
        }

        private void resetOption()
        {
            foreach (GridViewRow dgItem in this.grdCorr.Rows)
            {
                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                optCorr.Checked = false;
            }

            this.SelectedCorrIndex = -1;
            this.SelectedPage = -1;
            this.SelectedRowIndex = -1;
            this.grid_corrindex.Value = "-1";
            this.grid_pageindex.Value = "-1";
            this.grid_rowindex.Value = "-1";
        }

        protected void BtnSearch_Click(object sender, System.EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

            try
            {
                //impostazioni iniziali
                this.pnl_corr.Visible = false;
                this.grdList.Visible = false;
                this.plc_countRecord.Visible = false;
                this.resetOption();

                this.SelectedPage = 1;

                if (this.RicercaDocumenti())
                {
                    this.LoadData(true);
                    this.upPnlFilters.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        protected void DocumentDdlTypeDocument_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    if (this.CustomDocuments)
                    {
                        this.Template = ProfilerDocManager.getTemplateById(this.DocumentDdlTypeDocument.SelectedItem.Value);
                        if (this.Template != null)
                        {

                            if (this.EnableStateDiagram)
                            {
                                this.DocumentDdlStateDiagram.ClearSelection();

                                //Verifico se esiste un diagramma di stato associato al tipo di documento
                                //Modifica per la visualizzazione solo degli stati per cui esistono documenti in essi
                                string idDiagramma = DiagrammiManager.getDiagrammaAssociato(this.DocumentDdlTypeDocument.SelectedValue).ToString();
                                if (!string.IsNullOrEmpty(idDiagramma) && !idDiagramma.Equals("0"))
                                {
                                    this.PnlStateDiagram.Visible = true;

                                    //Inizializzazione comboBox
                                    this.DocumentDdlStateDiagram.Items.Clear();
                                    ListItem itemEmpty = new ListItem();
                                    this.DocumentDdlStateDiagram.Items.Add(itemEmpty);

                                    DocsPaWR.Stato[] statiDg = DiagrammiManager.getStatiPerRicerca(idDiagramma, "D");
                                    foreach (Stato st in statiDg)
                                    {
                                        ListItem item = new ListItem(st.DESCRIZIONE, Convert.ToString(st.SYSTEM_ID));
                                        this.DocumentDdlStateDiagram.Items.Add(item);
                                    }

                                    this.ddlStateCondition.Visible = true;
                                    this.PnlStateDiagram.Visible = true;
                                }
                                else
                                {
                                    this.ddlStateCondition.Visible = false;
                                    this.PnlStateDiagram.Visible = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.Template = null;
                    Session["templateRicerca"] = null;
                    this.PnlTypeDocument.Controls.Clear();
                    if (this.EnableStateDiagram)
                    {
                        this.DocumentDdlStateDiagram.ClearSelection();
                        this.PnlStateDiagram.Visible = false;
                        this.ddlStateCondition.Visible = false;
                    }
                }
                this.UpPnlTypeDocument.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void ReApplyChosenScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen_deselect", "$('.chzn-select-deselect').chosen({ allow_single_deselect: true, no_results_text: 'Nessun risultato trovato' });", true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "chosen", "$('.chzn-select').chosen({ no_results_text: 'Nessun risultato trovato' });", true);
        }

        private void ReApplyDatePickerScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "datepicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void ReApplyOnlyNumbersScript()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "onlynumbers", "OnlyNumbers();", true);
        }

        private void ReApplyScripts()
        {
            this.ReApplyChosenScript();
            this.ReApplyDatePickerScript();
            this.ReApplyOnlyNumbersScript();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshNoteChars", "charsLeft('TxtObject', " + this.MaxLenghtObject + ", '" + this.projectLitVisibleObjectChars.Text.Replace("'", "\'") + "');", true);
            this.TxtObject_chars.Attributes["rel"] = "TxtObject_" + this.MaxLenghtObject + "_" + this.projectLitVisibleObjectChars.Text;
        }

        private void LoadRegisters()
        {
            Registro[] reg = UserManager.GetRegistersList();

            if (reg.Length > 1 && UserManager.isFiltroAooEnabled())
            {
                for (int i = 0; i < reg.Length; i++)
                {
                    Registro register = UserManager.getRegistroBySistemId(this.Page, reg[i].systemId);
                    if (!register.Sospeso)
                    {
                        ListItem li = new ListItem(reg[i].codRegistro, reg[i].systemId);
                        this.ddl_reg.Items.Add(li);
                    }
                }
            }
            else
            {
                pnl_catene_extra_aoo.Visible = false;
            }
        }

        private void getLettereProtocolli()
        {
            this.opArr.Text = DocumentManager.GetDescriptionLabel("A");
            this.opPart.Text = DocumentManager.GetDescriptionLabel("P");
            this.opInt.Text = DocumentManager.GetDescriptionLabel("I");
            this.nnprot.Text = DocumentManager.GetDescriptionLabel("G");
          
        }

        protected void ddl_numProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                this.txtEndNumProto.Text = "";

                if (this.ddl_numProto.SelectedIndex == 0)
                {
                    this.pnlEndNumProto.Visible = false;
                    this.lblInitNumProto.Visible = false;
                }
                else
                {
                    this.pnlEndNumProto.Visible = true;
                    this.lblInitNumProto.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaProto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                this.txtEndDataProtocollo.Text = "";
                if (this.ddl_dtaProto.SelectedIndex == 0)
                {
                    this.pnlEndDataProtocollo.Visible = false;
                    this.lblInitDtaProto.Visible = false;

                }
                else
                {
                    this.pnlEndDataProtocollo.Visible = true;
                    this.lblInitDtaProto.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtCodeObject_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
                List<DocsPaWR.Registro> registries = new List<Registro>();
                registries = UIManager.RegistryManager.GetListRegistriesAndRF(UIManager.RoleManager.GetRoleInSession().systemId, "1", string.Empty).ToList<DocsPaWR.Registro>();
                registries.Add(UIManager.RegistryManager.GetRegistryInSession());

                List<string> aL = new List<string>();
                if (registries != null)
                {
                    for (int i = 0; i < registries.Count; i++)
                    {
                        aL.Add(registries[i].systemId);
                    }
                }

                DocsPaWR.Oggetto[] listaObj = null;

                // E' inutile finire nel backend se la casella di testo è vuota (a parte il fatto che 
                // la funzione, in questo caso, restituisce tutto l'oggettario)
                if (!string.IsNullOrEmpty(this.TxtCodeObject.Text.Trim()))
                {
                    //In questo momento tralascio la descrizione oggetto che metto come stringa vuota
                    listaObj = DocumentManager.getListaOggettiByCod(aL.ToArray<string>(), string.Empty, this.TxtCodeObject.Text);
                }
                else
                {
                    listaObj = new DocsPaWR.Oggetto[] { 
                            new DocsPaWR.Oggetto()
                            {
                                descrizione = String.Empty,
                                codOggetto = String.Empty
                            }};
                }

                if (listaObj != null && listaObj.Length > 0)
                {
                    this.TxtObject.Text = listaObj[0].descrizione;
                    this.TxtCodeObject.Text = listaObj[0].codOggetto;
                }
                else
                {
                    this.TxtObject.Text = string.Empty;
                    this.TxtCodeObject.Text = string.Empty;
                }



                this.UpdPnlObject.Update();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_numDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                this.txtEndNumDoc.Text = "";

                if (this.ddl_numDoc.SelectedIndex == 0)
                {
                    this.pnlEndNumDoc.Visible = false;
                    this.lblInitNumDoc.Visible = false;
                }
                else
                {
                    this.pnlEndNumDoc.Visible = true;
                    this.lblInitNumDoc.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void ddl_dtaDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                this.txtEndDataDoc.Text = "";

                if (this.ddl_dtaDoc.SelectedIndex == 0)
                {
                    this.pnlEndDataDoc.Visible = false;
                    this.lblInitDtaDoc.Visible = false;
                }
                else
                {
                    this.pnlEndDataDoc.Visible = true;
                    this.lblInitDtaDoc.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void resetSelection(object sender, ImageClickEventArgs e)
        {
            try {
                foreach (GridViewRow dgItem in this.grdCorr.Rows)
                {
                    RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                    optCorr.Checked = false;
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void checkOPT(object sender, EventArgs e)
        {
            try {
                int i = 0;
                foreach (GridViewRow GVR in this.grdCorr.Rows)
                {
                    RadioButton optCorr = GVR.Cells[3].FindControl("optCorr") as RadioButton;
                    if (optCorr != null)
                    {
                        optCorr.Checked = optCorr.Equals(sender);
                        if (optCorr.Checked)
                        {
                            this.grdCorr.SelectedIndex = i;
                            this.SelectedCorrIndex = i;
                            this.grid_corrindex.Value = i.ToString();
                        }
                    }
                    i++;
                }

                this.FillDataGrid(false);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void checkOPTDoc(object sender, EventArgs e)
        {
            try {
                int i = 0;
                foreach (GridViewRow GVR in this.grdList.Rows)
                {
                    RadioButton optCorr = GVR.Cells[6].FindControl("optCorr") as RadioButton;
                    if (optCorr != null)
                    {
                        optCorr.Checked = optCorr.Equals(sender);
                        if (optCorr.Checked)
                        {
                            this.grdList.SelectedIndex = i;
                            this.SelectedRowIndex = i;
                            this.grid_rowindex.Value = i.ToString();
                        }
                    }
                    i++;
                }

                this.BindGrid();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }        

        protected bool RicercaDocumenti()
        {
            try
            {
                //array contenitore degli array filtro di ricerca
                this.ListaFiltri = new DocsPaWR.FiltroRicerca[1][];
                this.ListaFiltri[0] = new DocsPaWR.FiltroRicerca[1];
                fVList = new DocsPaWR.FiltroRicerca[0];

                #region Filtro per REGISTRO
                if (!this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected && UserManager.getRegisterSelected()!=null)
                {
                    if (!UserManager.isFiltroAooEnabled())
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        fV1.valore = UserManager.getRegisterSelected().systemId;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    else
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.REGISTRO.ToString();
                        if (ddl_reg.Items.Count <= 1)
                        {
                            fV1.valore = UserManager.getRegisterSelected().systemId;
                        }
                        else
                        {
                            fV1.valore = this.ddl_reg.SelectedItem.Value;
                        }
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }

                #endregion

                #region filtro NUMERO DI PROTOCOLLO
                if (this.ddl_numProto.SelectedIndex == 0)
                {
                    if (this.txtInitNumProto.Text != null && !this.txtInitNumProto.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO.ToString();
                        fV1.valore = this.txtInitNumProto.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {//valore singolo carico NUM_PROTOCOLLO_DAL - NUM_PROTOCOLLO_AL
                    if (!this.txtInitNumProto.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_DAL.ToString();
                        fV1.valore = this.txtInitNumProto.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndNumProto.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.NUM_PROTOCOLLO_AL.ToString();
                        fV1.valore = this.txtEndNumProto.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro ANNO DI PROTOCOLLO

                fV1 = new DocsPaWR.FiltroRicerca();
                fV1.argomento = DocsPaWR.FiltriDocumento.ANNO_PROTOCOLLO.ToString();
                fV1.valore = this.txt_annoProto.Text;
                fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                #endregion

                #region filtro DATA PROTOCOLLO
                if (this.ddl_dtaProto.SelectedIndex == 0)
                {
                    //valore singolo specificato per DATA_PROTOCOLLO
                    if (!this.txtInitDtaProto.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_IL.ToString();
                        fV1.valore = this.txtInitDtaProto.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    //valore singolo carico DATA_PROTOCOLLO_DAL - DATA_PROTOCOLLO_AL
                    if (!this.txtInitDtaProto.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.txtInitDtaProto.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndDataProtocollo.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_PROT_PRECEDENTE_IL.ToString();
                        fV1.valore = this.txtEndDataProtocollo.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro per ricerca protocollo in PARTENZA
                if (this.rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_PARTENZA.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "P";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ricerca protocollo in ARRIVO
                if (this.rbl_TipoDoc.Items.FindByValue("Arrivo").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PROT_ARRIVO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "A";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ricerca protocollo INTERNI
                if (this.rbl_TipoDoc.Items.FindByValue("Interno") != null)
                {
                    if (this.rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.PROT_INTERNO.ToString();
                        fV1.valore = "true";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                        fV1.valore = "I";
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro per ricerca documenti non protocollati
                if (this.rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.TIPO.ToString();
                    fV1.valore = "G";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region filtro per ricerca documenti predisposti
                if (this.rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.PREDISPOSTO.ToString();
                    fV1.valore = "true";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region NUOVO SVILUPPO CONCATENAMENTO TRASVERSALE PER TUTTI I TIPI DI DOCUMENTI
                #region filtro NUMERO DI DOCUMENTO
                if (this.ddl_numDoc.SelectedIndex == 0)
                {
                    if (this.txtInitDoc.Text != null && !this.txtInitDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER.ToString();
                        fV1.valore = this.txtInitDoc.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    //valore singolo carico DOCNUMBER_DAL - DOCNUMBER_AL
                    if (!this.txtInitDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_DAL.ToString();
                        fV1.valore = this.txtInitDoc.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (!this.txtEndNumDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DOCNUMBER_AL.ToString();
                        fV1.valore = this.txtEndNumDoc.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion

                #region filtro DATA CREAZIONE
                if (this.ddl_dtaDoc.SelectedIndex == 0)
                {
                    //valore singolo specificato per DATA_CREAZIONE
                    if (!this.txtInitDtaDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_IL.ToString();
                        fV1.valore = this.txtInitDtaDoc.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                else
                {
                    //valore singolo carico DATA_CREAZIONE_DAL - DATA_CREAZIONE_AL
                    if (!this.txtInitDtaDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_SUCCESSIVA_AL.ToString();
                        fV1.valore = this.txtInitDtaDoc.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (!this.txtEndDataDoc.Text.Equals(""))
                    {
                        fV1 = new DocsPaWR.FiltroRicerca();
                        fV1.argomento = DocsPaWR.FiltriDocumento.DATA_CREAZIONE_PRECEDENTE_IL.ToString();
                        fV1.valore = this.txtEndDataDoc.Text;
                        fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                }
                #endregion


                #endregion

                #region FILTRO OGGETTO

                if (!string.IsNullOrEmpty(this.TxtObject.Text))
                {
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriDocumento.OGGETTO.ToString();
                    fV1.valore = utils.DO_AdattaString(this.TxtObject.Text);
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }
                #endregion

                #region FILTRO TIPOLOGIA DOCUMENTALE

                #region TIPOLOGIA

                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];
                        controllaCampi(oggettoCustom, oggettoCustom.SYSTEM_ID.ToString());
                    }

                    fV1 = new FiltroRicerca();
                    fV1.argomento = FiltriDocumento.TIPO_ATTO.ToString();
                    fV1.valore = this.DocumentDdlTypeDocument.SelectedValue;
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion

                #region FILTRO PROFILAZIONE DINAMICA

                if (!string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue))
                {
                    this.PopulateProfiledDocument();
                    fV1 = new DocsPaWR.FiltroRicerca();
                    fV1.argomento = DocsPaWR.FiltriFascicolazione.PROFILAZIONE_DINAMICA.ToString();
                    fV1.template = this.Template;
                    fV1.valore = "Profilazione Dinamica";
                    fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                }

                #endregion

                if (!UIManager.UserManager.IsAuthorizedFunctions("GRID_PERSONALIZATION"))
                {
                    if (!string.IsNullOrEmpty(DocumentDdlTypeDocument.SelectedValue))
                    {
                        Templates template = DocumentManager.getTemplateById(DocumentDdlTypeDocument.SelectedValue, UserManager.GetInfoUser());
                        if (template != null)
                        {
                            OggettoCustom customObjectTemp = new OggettoCustom();
                            customObjectTemp = template.ELENCO_OGGETTI.Where(
                            r => r.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE" && r.DA_VISUALIZZARE_RICERCA == "1").
                            FirstOrDefault();
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriDocumento.CONTATORE_GRIGLIE_NO_CUSTOM.ToString();
                            fV1.valore = customObjectTemp.TIPO_CONTATORE;
                            fV1.nomeCampo = template.SYSTEM_ID.ToString();
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);

                            // Creazione di un filtro per la profilazione
                            fV1 = new FiltroRicerca();
                            fV1.argomento = FiltriDocumento.PROFILATION_FIELD_FOR_ORDER.ToString();
                            fV1.valore = customObjectTemp.SYSTEM_ID.ToString();
                            fV1.nomeCampo = customObjectTemp.DESCRIZIONE;
                            fVList = utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                    }
                }

                #endregion

                this.ListaFiltri[0] = fVList;
                return true;

            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        private bool controllaCampi(DocsPaWR.OggettoCustom oggettoCustom, string idOggetto)
        {
            //In questo metodo, oltre al controllo si salvano i valori dei campi inseriti 
            //dall'utente nel template in sessione. Solo successivamente, quanto verra' salvato 
            //il documento i suddetti valori verranno riportai nel Db vedi metodo "btn_salva_Click" della "docProfilo.aspx"

            //Label_Avviso.Visible = false;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "CampoDiTesto":
                    CustomTextArea textBox = (CustomTextArea)PnlTypeDocument.FindControl(idOggetto);
                    if (textBox != null)
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            //SetFocus(textBox);
                            oggettoCustom.VALORE_DATABASE = textBox.Text;
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = textBox.Text;
                    }
                    break;
                case "CasellaDiSelezione":
                    CheckBoxList checkBox = (CheckBoxList)PnlTypeDocument.FindControl(idOggetto);
                    if (checkBox != null)
                    {
                        if (checkBox.SelectedIndex == -1)
                        {
                            //SetFocus(checkBox);
                            for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Length; i++)
                                oggettoCustom.VALORI_SELEZIONATI[i] = null;

                            return true;
                        }

                        oggettoCustom.VALORI_SELEZIONATI = new string[checkBox.Items.Count];
                        oggettoCustom.VALORE_DATABASE = "";
                        for (int i = 0; i < checkBox.Items.Count; i++)
                        {
                            if (checkBox.Items[i].Selected)
                            {
                                oggettoCustom.VALORI_SELEZIONATI[i] = checkBox.Items[i].Text;
                            }
                        }
                    }
                    break;
                case "MenuATendina":
                    DropDownList dropDwonList = (DropDownList)PnlTypeDocument.FindControl(idOggetto);
                    if (dropDwonList != null)
                    {
                        if (dropDwonList.SelectedItem.Text.Equals(""))
                        {
                            //SetFocus(dropDwonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = dropDwonList.SelectedItem.Text;
                    }
                    break;
                case "SelezioneEsclusiva":
                    RadioButtonList radioButtonList = (RadioButtonList)PnlTypeDocument.FindControl(idOggetto);
                    if (radioButtonList != null)
                    {
                        if (oggettoCustom.VALORE_DATABASE == "-1" || radioButtonList.SelectedIndex == -1 || radioButtonList.SelectedValue == "-1")
                        {
                            //SetFocus(radioButtonList);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }
                        oggettoCustom.VALORE_DATABASE = radioButtonList.SelectedItem.Text;
                    }
                    break;
                case "Data":
                    UserControls.Calendar dataDa = (UserControls.Calendar)PnlTypeDocument.FindControl("da_" + oggettoCustom.SYSTEM_ID.ToString());
                    UserControls.Calendar dataA = (UserControls.Calendar)PnlTypeDocument.FindControl("a_" + oggettoCustom.SYSTEM_ID.ToString());

                    if (dataDa != null && dataA != null)
                    {
                        if (dataDa.Text.Equals("") && dataA.Text.Equals(""))
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }

                        if (dataDa.Text.Equals("") && dataA.Text != "")
                        {
                            //SetFocus(dataDa.txt_Data);
                            oggettoCustom.VALORE_DATABASE = "";
                            return true;
                        }

                        if (dataDa.Text != "" && dataA.Text != "")
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text + "@" + dataA.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text + "@" + dataA.Text;

                        if (dataDa.Text != "" && dataA.Text == "")
                            //oggettoCustom.VALORE_DATABASE = dataDa.txt_Data.Text;
                            oggettoCustom.VALORE_DATABASE = dataDa.Text;
                    }

                    break;
                case "Contatore":
                    CustomTextArea contatoreDa = (CustomTextArea)PnlTypeDocument.FindControl("da_" + idOggetto);
                    CustomTextArea contatoreA = (CustomTextArea)PnlTypeDocument.FindControl("a_" + idOggetto);
                    //Controllo la valorizzazione di campi ed eventualmente notifico gli errori
                    CustomTextArea dataRepertorioDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                    CustomTextArea dataRepertorioA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);
                    if (dataRepertorioDa != null && dataRepertorioA != null)
                    {
                        if (dataRepertorioDa.Text != "" && dataRepertorioA.Text != "")
                            oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text + "@" + dataRepertorioA.Text;

                        if (dataRepertorioDa.Text != "" && dataRepertorioA.Text == "")
                            oggettoCustom.DATA_INSERIMENTO = dataRepertorioDa.Text;
                    }
                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "T":
                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }
                            break;
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa != null && ddlAoo != null && contatoreDa.Text.Equals("") && ddlAoo.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                return true;
                            }

                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (contatoreDa != null && ddlRf != null && contatoreDa.Text.Equals("") && ddlRf.SelectedValue.Equals(""))
                            {
                                //SetFocus(contatoreDa);
                                oggettoCustom.VALORE_DATABASE = "";
                                oggettoCustom.ID_AOO_RF = "";
                                return true;
                            }

                            if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text.Equals(""))
                                oggettoCustom.VALORE_DATABASE = "";
                            break;
                    }

                    if (contatoreDa != null && contatoreA != null && contatoreDa.Text.Equals("") && contatoreA.Text != "")
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }

                    try
                    {
                        if (contatoreDa != null && contatoreDa.Text != null && contatoreDa.Text != "")
                            Convert.ToInt32(contatoreDa.Text);
                        if (contatoreA != null && contatoreA.Text != null && contatoreA.Text != "")
                            Convert.ToInt32(contatoreA.Text);
                    }
                    catch (Exception ex)
                    {
                        //SetFocus(contatoreDa);
                        oggettoCustom.VALORE_DATABASE = "";
                        return true;
                    }


                    //I campi sono valorizzati correttamente procedo
                    if (contatoreDa != null && contatoreA != null)
                    {
                        if (contatoreDa.Text != "" && contatoreA.Text != "")
                            oggettoCustom.VALORE_DATABASE = contatoreDa.Text + "@" + contatoreA.Text;

                        if (contatoreDa.Text != "" && contatoreA.Text == "")
                            oggettoCustom.VALORE_DATABASE = contatoreDa.Text;
                    }

                    switch (oggettoCustom.TIPO_CONTATORE)
                    {
                        case "A":
                            DropDownList ddlAoo = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (ddlAoo != null)
                                oggettoCustom.ID_AOO_RF = ddlAoo.SelectedValue;
                            break;
                        case "R":
                            DropDownList ddlRf = (DropDownList)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString() + "_menu");
                            if (ddlRf != null)
                                oggettoCustom.ID_AOO_RF = ddlRf.SelectedValue;
                            break;
                    }
                    break;
                case "Corrispondente":
                    UserControls.CorrespondentCustom corr = (UserControls.CorrespondentCustom)PnlTypeDocument.FindControl(oggettoCustom.SYSTEM_ID.ToString());
                    DocsPaWR.Corrispondente corrispondente = new DocsPaWR.Corrispondente();

                    if (corr != null)
                    {
                        // 1 - Ambedue i campi del corrispondente non sono valorizzati
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                            return true;
                        }
                        // 2 - E' stato valorizzato solo il campo descrizione del corrispondente
                        if (string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom) && !string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                        {
                            oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                        }
                        // 3 - E' valorizzato il campo codice del corrispondente
                        if (!string.IsNullOrEmpty(corr.TxtCodeCorrespondentCustom))
                        {
                            //Cerco il corrispondente
                            if (!string.IsNullOrEmpty(corr.IdCorrespondentCustom))
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteBySystemIDDisabled(corr.IdCorrespondentCustom);
                            else
                                corrispondente = UIManager.AddressBookManager.getCorrispondenteByCodRubrica(corr.TxtCodeCorrespondentCustom, false);

                            //corrispondente = UserManager.getCorrispondenteByCodRubrica(this, corr.CODICE_TEXT);
                            // 3.1 - Corrispondente trovato per codice
                            if (corrispondente != null)
                            {
                                oggettoCustom.VALORE_DATABASE = corrispondente.systemId;
                                oggettoCustom.ESTENDI_STORICIZZATI = corr.ChkStoryCustomCorrespondentCustom;
                            }
                            // 3.2 - Corrispondente non trovato per codice
                            else
                            {
                                // 3.2.1 - Campo descrizione non valorizzato
                                if (string.IsNullOrEmpty(corr.TxtDescriptionCorrespondentCustom))
                                {
                                    oggettoCustom.VALORE_DATABASE = string.Empty;
                                    return true;
                                }
                                // 3.2.2 - Campo descrizione valorizzato
                                else
                                    oggettoCustom.VALORE_DATABASE = corr.TxtDescriptionCorrespondentCustom;
                            }
                        }
                    }
                    break;
                case "ContatoreSottocontatore":
                    break;


            }
            return false;
        }

        protected bool checkIfNewSearch()
        {
            if (
                    (this.FilterTipoDoc != this.rbl_TipoDoc.SelectedValue)
                    || (this.FilterADL != this.chk_ADL.Checked)
                    || (this.FilterRF != this.ddl_reg.SelectedValue)
                    || (this.FilterNumProt != this.ddl_numProto.SelectedValue)
                    || (this.FilterNumProtFrom != this.txtInitNumProto.Text)
                    || (this.FilterNumProtTo != this.txtEndNumProto.Text)
                    || (this.FilterNumProtYear != this.txt_annoProto.Text)
                    || (this.FilterDtProt != this.ddl_dtaProto.SelectedValue)
                    || (this.FilterDtProtFrom != this.txtInitDtaProto.Text)
                    || (this.FilterDtProtTo != this.txtEndDataProtocollo.Text)
                    || (this.FilterNumDoc != this.ddl_numDoc.SelectedValue)
                    || (this.FilterNumDocFrom != this.txtInitDoc.Text)
                    || (this.FilterNumDocTo != this.txtEndNumDoc.Text)
                    || (this.FilterDtDoc != this.ddl_dtaDoc.SelectedValue)
                    || (this.FilterDtDocFrom != this.txtInitDtaDoc.Text)
                    || (this.FilterDtDocTo != this.txtEndDataDoc.Text)
               ) return true;
            return false;
        }

        private void LoadData(bool updateGrid)
        {
            // reset paging
            bool isNewSearch = this.checkIfNewSearch();
            if (isNewSearch)
            {
                this.SelectedPage = 1;
                this.SelectedRowIndex = -1;
                this.grdList.SelectedIndex = this.SelectedRowIndex;
                this.FilterTipoDoc = this.rbl_TipoDoc.SelectedValue;
                this.FilterADL = this.chk_ADL.Checked;
                this.FilterRF = this.ddl_reg.SelectedValue;
                this.FilterNumProt = this.ddl_numProto.SelectedValue;
                this.FilterNumProtFrom = this.txtInitNumProto.Text;
                this.FilterNumProtTo = this.txtEndNumProto.Text;
                this.FilterNumProtYear = this.txt_annoProto.Text;
                this.FilterDtProt = this.ddl_dtaProto.SelectedValue;
                this.FilterDtProtFrom = this.txtInitDtaProto.Text;
                this.FilterDtProtTo = this.txtEndDataProtocollo.Text;
                this.FilterNumDoc = this.ddl_numDoc.SelectedValue;
                this.FilterNumDocFrom = this.txtInitDoc.Text;
                this.FilterNumDocTo = this.txtEndNumDoc.Text;
                this.FilterDtDoc = this.ddl_dtaDoc.SelectedValue;
                this.FilterDtDocFrom = this.txtInitDtaDoc.Text;
                this.FilterDtDocTo = this.txtEndDataDoc.Text;
                this.grid_pageindex.Value = this.SelectedPage.ToString();
                this.grid_rowindex.Value = this.SelectedRowIndex.ToString();
            }
            else if (this.SelectedPage.ToString() != this.grid_pageindex.Value)
            {
                this.SelectedPage = 1;
                if (!string.IsNullOrEmpty(this.grid_pageindex.Value)) this.SelectedPage = int.Parse(this.grid_pageindex.Value);
                this.SelectedRowIndex = -1;
                this.grid_rowindex.Value = this.SelectedRowIndex.ToString();
                this.grdList.SelectedIndex = this.SelectedRowIndex;
            }


            DocsPaWR.InfoUtente infoUt = UserManager.GetInfoUser();
            SearchResultInfo[] idProfileList;
            int pagesCount = 0;

            if (!this.chk_ADL.Checked == true)
            {
                bool grigi = false;
                if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected) grigi = true;

                this.infoDoc = DocumentManager.getQueryInfoDocumentoPaging(infoUt.idGruppo, infoUt.idPeople, this, this.ListaFiltri, this.SelectedPage, out pagesCount, out nRec, true, grigi, true, false, out idProfileList);
                this.PagesCount = pagesCount;

                //Controllo inserito per non far visualizzare se stesso
                DocsPaWR.SchedaDocumento doc = DocumentManager.getSelectedRecord();
                if (doc != null)
                {
                    DocsPaWR.InfoDocumento infoDocumentoLav = DocumentManager.getInfoDocumento(doc);
                    if (infoDocumentoLav != null)
                    {
                        List<DocsPaWR.InfoDocumento> tempDoc = new List<DocsPaWR.InfoDocumento>();
                        tempDoc = this.infoDoc.ToList<DocsPaWR.InfoDocumento>();
                        foreach (DocsPaWR.InfoDocumento infDocTemp in infoDoc)
                        {
                            if (infDocTemp.docNumber == infoDocumentoLav.docNumber)
                            {
                                tempDoc.Remove(infDocTemp);
                                break;
                            }
                        }
                        this.infoDoc = tempDoc.ToArray();
                    }
                }
                //FINE CONTROLLO INSERIMENTO PER NON FAR VISUALIZZARE SE STESSO
            }
            else
            {
                DocsPaWR.AreaLavoro areaLavoro = DocumentManager.getListaAreaLavoro(this, "P", "0", this.SelectedPage, out pagesCount, out nRec, UserManager.getRegisterSelected().systemId, this.ListaFiltri);
                this.PagesCount = pagesCount;

                this.infoDoc = new DocsPaWR.InfoDocumento[areaLavoro.lista.Length];

                for (int i = 0; i < areaLavoro.lista.Length; i++)
                    this.infoDoc[i] = (DocsPaWR.InfoDocumento)areaLavoro.lista[i];
            }


            this.plc_countRecord.Visible = true;
            this.lbl_countRecord.Text = Utils.Languages.GetLabelFromCode("AnswerSearchResult", UIManager.UserManager.GetUserLanguage()) + " " + nRec.ToString();

            if (!this.chk_ADL.Checked && string.IsNullOrEmpty(this.txtInitNumProto.Text)
                && string.IsNullOrEmpty(this.txtInitDtaProto.Text) && string.IsNullOrEmpty(this.txtInitDoc.Text)
                && string.IsNullOrEmpty(this.txtInitDtaDoc.Text) && string.IsNullOrEmpty(this.DocumentDdlTypeDocument.SelectedValue)
               )
            {
                string searchIntervalYears = Utils.InitConfigurationKeys.GetValue("0", DBKeys.MAX_YEARS_SEARCHABLE.ToString());
                if (!string.IsNullOrEmpty(searchIntervalYears) && !searchIntervalYears.Equals("0"))
                {
                    string date = (Convert.ToDateTime(DocumentManager.toDay()).AddYears(-Convert.ToInt32(searchIntervalYears))).ToShortDateString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "if (parent.fra_main) {parent.fra_main.ajaxDialogModal('WarningSearchDocumentIntervalYears', 'warning', '', '" + date + "');} else {parent.ajaxDialogModal('WarningSearchDocumentIntervalYears', 'warning', '', '" + date + "');}", true);
                }
            }

            if (this.infoDoc != null && this.infoDoc.Length > 0)
            {
                this.BindGrid();
            }

        }

        public void BindGrid()
        {
            DocsPaWR.InfoDocumento currentDoc;

            DocsPaWR.SchedaDocumento doc = DocumentManager.getSelectedRecord();
            DocsPaWR.InfoDocumento infoDocumentoLav = null;
            if (doc!=null) infoDocumentoLav = DocumentManager.getInfoDocumento(doc);

            if (this.infoDoc != null && this.infoDoc.Length > 0)
            {
                this.Dg_elem = new ArrayList();
                string descrDoc = string.Empty;
                int numProt = new Int32();

                for (int i = 0; i < this.infoDoc.Length; i++)
                {
                    currentDoc = ((DocsPaWR.InfoDocumento)this.infoDoc[i]);

                    string dataApertura = "";
                    if (currentDoc.dataApertura != null && currentDoc.dataApertura.Length > 0)
                        dataApertura = currentDoc.dataApertura.Substring(0, 10);


                    if (currentDoc.numProt != null && !currentDoc.numProt.Equals(""))
                    {
                        numProt = Int32.Parse(currentDoc.numProt);
                        descrDoc = numProt.ToString();
                    }
                    else
                    {
                        descrDoc = currentDoc.docNumber;
                    }

                    descrDoc = descrDoc + "\n" + dataApertura;
                    string MittDest = "";

                    if (currentDoc.mittDest != null && currentDoc.mittDest.Length > 0)
                    {
                        for (int g = 0; g < currentDoc.mittDest.Length; g++)
                            MittDest += currentDoc.mittDest[g] + " - ";
                        if (currentDoc.mittDest.Length > 0)
                            MittDest = MittDest.Substring(0, MittDest.Length - 3);
                    }

                    this.Dg_elem.Add(new ProtocolloDataGridItem(descrDoc, currentDoc.oggetto, MittDest, currentDoc.codRegistro, i));
                }

                this.grdList.Columns[0].HeaderText = this.GetLabel("AnswerDate");
                this.grdList.Columns[1].HeaderText = this.GetLabel("AnswerRegistry");
                this.grdList.Columns[2].HeaderText = this.GetLabel("AnswerSubject");
                this.grdList.Columns[3].HeaderText = this.GetLabel("AnswerRecipient");
                this.grdList.Columns[4].HeaderText = this.GetLabel("AnswerRecipientShort");
                this.grdList.Columns[5].HeaderText = this.GetLabel("AnswerSender");
                this.grdList.Columns[6].HeaderText = this.GetLabel("AnswerSelect");

                this.grdList.DataSource = this.Dg_elem;
                this.grdList.DataBind();
                this.grdList.Visible = true;


                // rebuild navigator
                this.buildGridNavigator();


                // display rows
                for (int i = 0; i < this.grdList.Rows.Count; i++)
                {
                    Label lbl_numDoc = this.grdList.Rows[i].Cells[0].Controls[1] as Label;
                    lbl_numDoc.Font.Bold = true;
                    if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected || rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                    {
                        lbl_numDoc.ForeColor = Color.Black;
                    }
                    else
                    {
                        lbl_numDoc.ForeColor = Color.Red;
                    }

                    if (i == this.SelectedRowIndex)
                    {
                        this.grdList.Rows[i].CssClass = "selectedrow";
                        RadioButton optCorr = this.grdList.Rows[i].Cells[6].FindControl("optCorr") as RadioButton;
                        if (optCorr != null) optCorr.Checked = true;
                    }
                }

                // display columns
                if (rbl_TipoDoc.Items.FindByValue("Non Protocollato").Selected) 
                {
                    this.grdList.Columns[1].Visible = false;
                    this.grdList.Columns[3].Visible = false;
                    this.grdList.Columns[4].Visible = false;
                    this.grdList.Columns[5].Visible = false;
                    this.grdList.Columns[6].Visible = true;
                }
                if (rbl_TipoDoc.Items.FindByValue("Arrivo").Selected)
                {
                    if (infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G"))
                    {
                        this.grdList.Columns[1].Visible = true;
                        this.grdList.Columns[3].Visible = false;
                        this.grdList.Columns[4].Visible = false;
                        this.grdList.Columns[5].Visible = false;
                        this.grdList.Columns[6].Visible = true;
                    }
                    else
                    {
                        this.grdList.Columns[1].Visible = true;
                        this.grdList.Columns[3].Visible = false;
                        this.grdList.Columns[4].Visible = false;
                        this.grdList.Columns[5].Visible = true;
                        this.grdList.Columns[6].Visible = true;
                    }
                }
                if (rbl_TipoDoc.Items.FindByValue("Partenza").Selected)
                {
                    if (infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G"))
                    {
                        this.grdList.Columns[1].Visible = true;
                        this.grdList.Columns[3].Visible = false;
                        this.grdList.Columns[4].Visible = false;
                        this.grdList.Columns[5].Visible = false;
                        this.grdList.Columns[6].Visible = true;
                    }
                    else
                    {
                        if (infoDocumentoLav != null)
                        {
                            if (infoDocumentoLav.tipoProto.Equals("P"))
                            {
                                this.grdList.Columns[1].Visible = true;
                                this.grdList.Columns[3].Visible = false;
                                this.grdList.Columns[4].Visible = false;
                                this.grdList.Columns[5].Visible = false;
                                this.grdList.Columns[6].Visible = true;
                            }
                            else
                            {
                                if ((infoDocumentoLav.tipoProto.Equals("A") || infoDocumentoLav.tipoProto.Equals("I")) && infoDocumentoLav.docNumber != null && infoDocumentoLav.segnatura != null)
                                {
                                    if (infoDocumentoLav.segnatura.Equals(""))
                                    {
                                        this.grdList.Columns[1].Visible = true;
                                        this.grdList.Columns[3].Visible = false;
                                        this.grdList.Columns[4].Visible = false;
                                        this.grdList.Columns[5].Visible = false;
                                        this.grdList.Columns[6].Visible = true;
                                    }
                                    else
                                    {
                                        this.grdList.Columns[1].Visible = true;
                                        this.grdList.Columns[3].Visible = false;
                                        this.grdList.Columns[4].Visible = true;
                                        this.grdList.Columns[5].Visible = false;
                                        this.grdList.Columns[6].Visible = false;
                                    }
                                }
                                else
                                {
                                    this.grdList.Columns[1].Visible = true;
                                    this.grdList.Columns[3].Visible = false;
                                    this.grdList.Columns[4].Visible = true;
                                    this.grdList.Columns[5].Visible = false;
                                    this.grdList.Columns[6].Visible = false;
                                }
                            }
                        }
                    }
                }
                if (rbl_TipoDoc.Items.FindByValue("Interno") != null)
                {
                    if (rbl_TipoDoc.Items.FindByValue("Interno").Selected)
                    {
                        if (infoDocumentoLav.tipoProto.ToUpper().Equals("N") || infoDocumentoLav.tipoProto.Equals("NP") || infoDocumentoLav.tipoProto.Equals("G"))
                        {
                            this.grdList.Columns[1].Visible = true;
                            this.grdList.Columns[3].Visible = true;
                            this.grdList.Columns[4].Visible = false;
                            this.grdList.Columns[5].Visible = false;
                            this.grdList.Columns[6].Visible = true;
                        }
                        else
                        {
                            if (infoDocumentoLav.tipoProto.Equals("I") || ((infoDocumentoLav.tipoProto.Equals("P") || infoDocumentoLav.tipoProto.Equals("A")) && infoDocumentoLav.docNumber != null))
                            {
                                this.grdList.Columns[1].Visible = true;
                                this.grdList.Columns[3].Visible = true;
                                this.grdList.Columns[4].Visible = false;
                                this.grdList.Columns[5].Visible = false;
                                this.grdList.Columns[6].Visible = true;
                            }
                            else
                            {
                                this.grdList.Columns[1].Visible = true;
                                this.grdList.Columns[3].Visible = false;
                                this.grdList.Columns[4].Visible = true;
                                this.grdList.Columns[5].Visible = false;
                                this.grdList.Columns[6].Visible = false;
                            }
                        }
                    }
                }

                if (rbl_TipoDoc.Items.FindByValue("Predisposti").Selected)
                {
                    this.grdList.Columns[1].Visible = true;
                    this.grdList.Columns[3].Visible = true;
                    this.grdList.Columns[4].Visible = false;
                    this.grdList.Columns[5].Visible = false;
                    this.grdList.Columns[6].Visible = true;
                }
            }
            else
            {
                this.grdList.Visible = false;
                this.rowMessage.Visible = true;
            }

            
            this.upPnlFilters.Update();
        }

        protected void buildGridNavigator()
        {
            this.plcNavigator.Controls.Clear();

            if (this.PagesCount > 1)
            {
                Panel panel = new Panel();
                panel.EnableViewState = true;
                panel.CssClass = "recordNavigator";

                int startFrom = 1;
                if (this.SelectedPage > 6) startFrom = this.SelectedPage - 5;

                int endTo = 10;
                if (this.SelectedPage > 6) endTo = this.SelectedPage + 5;
                if (endTo > this.PagesCount) endTo = this.PagesCount;

                if (startFrom > 1)
                {
                    LinkButton btn = new LinkButton();
                    btn.EnableViewState = true;
                    btn.Text = "...";
                    btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val(" + (startFrom - 1) + "); __doPostBack('upPnlGridList', ''); return false;";
                    panel.Controls.Add(btn);
                }

                for (int i = startFrom; i <= endTo; i++)
                {
                    if (i == this.SelectedPage)
                    {
                        Literal lit = new Literal();
                        lit.Text = "<span>" + i.ToString() + "</span>";
                        panel.Controls.Add(lit);
                    }
                    else
                    {
                        LinkButton btn = new LinkButton();
                        btn.EnableViewState = true;
                        btn.Text = i.ToString();
                        btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val($(this).text()); __doPostBack('upPnlGridList', ''); return false;";
                        panel.Controls.Add(btn);
                    }
                }

                if (endTo < this.PagesCount)
                {
                    LinkButton btn = new LinkButton();
                    btn.EnableViewState = true;
                    btn.Text = "...";
                    btn.Attributes["onclick"] = "disallowOp('Content2'); $('#grid_pageindex').val(" + endTo + "); __doPostBack('upPnlGridList', ''); return false;";
                    panel.Controls.Add(btn);
                }

                this.plcNavigator.Controls.Add(panel);
            }

        }

        protected void DisplayRecipients(object sender, EventArgs e)
        {
            try {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);

                // highlight selected row
                int i = 0;
                foreach (GridViewRow GVR in this.grdList.Rows)
                {
                    CustomImageButton img = GVR.Cells[4].FindControl("imgRecipients") as CustomImageButton;
                    if (img!=null && img.Equals(sender))
                    {
                        this.grdList.SelectedIndex = i;
                        this.SelectedRowIndex = i;
                        this.grid_rowindex.Value = i.ToString();
                    }
                    i++;
                }
                this.BindGrid();


                if (this.SelectedRowIndex > -1)
                    this.infoDocSel = (DocsPaWR.InfoDocumento)this.infoDoc[this.SelectedRowIndex];

                if (this.infoDocSel != null)
                {
                    //prendo il dettaglio del documento e estraggo i destinatari del protocollo
                    DocsPaWR.SchedaDocumento schedaDocUscita = new DocsPaWR.SchedaDocumento();
                    schedaDocUscita = DocumentManager.getDocumentDetails(this, infoDocSel.idProfile, infoDocSel.docNumber);
                    //prendo i destinatari in To
                    this.listaCorrTo = ((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatari;
                    //prendo i destinatari in CC
                    this.listaCorrCC = ((DocsPaWR.ProtocolloUscita)schedaDocUscita.protocollo).destinatariConoscenza;

                    this.FillDataGrid(true);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Caricamento griglia destinatari del protocollo in uscita selezionato
        /// </summary>
        /// <param name="uoApp"></param>
        private void FillDataGrid(bool selectDefault)
        {
            this.Recipients = this.CreateGridDataSetDestinatari();
            this.CaricaGridDataSetDestinatari(this.Recipients, this.listaCorrTo, this.listaCorrCC);
            this.grdCorr.DataSource = this.Recipients;
            this.grdCorr.DataBind();

            // Impostazione corrispondente predefinito
            if (selectDefault) this.SelectDefaultCorrispondente();

            // display rows
            for (int i = 0; i < this.grdCorr.Rows.Count; i++)
            {
                if (i == this.SelectedCorrIndex)
                {
                    this.grdCorr.Rows[i].CssClass = "selectedrow";
                    RadioButton optCorr = this.grdCorr.Rows[i].Cells[3].FindControl("OptCorr") as RadioButton;
                    if (optCorr != null) optCorr.Checked = true;
                }
            }
        }

        protected void grdCorr_PageIndexChanged(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.grdCorr.SelectedIndex = -1;
                this.grdCorr.PageIndex = e.NewPageIndex;
                //this.grdList.PageIndex = this.grdList.PageIndex + 1;
                this.grdCorr.DataSource = this.Recipients;
                this.grdCorr.DataBind();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private DataSet CreateGridDataSetDestinatari()
        {
            DataSet retValue = new DataSet();

            this.ProtoOut = new DataTable("GRID_TABLE_DESTINATARI");
            this.ProtoOut.Columns.Add("SYSTEM_ID", typeof(string));
            this.ProtoOut.Columns.Add("TIPO_CORR", typeof(string));
            this.ProtoOut.Columns.Add("DESC_CORR", typeof(string));
            retValue.Tables.Add(this.ProtoOut);

            return retValue;
        }

        /// <summary>
        /// Caricamento dataset utilizzato per le griglie
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="uo"></param>
        private void CaricaGridDataSetDestinatari(DataSet ds, DocsPaWR.Corrispondente[] listaCorrTo, DocsPaWR.Corrispondente[] listaCorrCC)
        {
            DataTable dt = ds.Tables["GRID_TABLE_DESTINATARI"];
            this.ht_destinatariTO_CC = new Hashtable();
            string tipoURP = "";

            if (listaCorrTo != null && listaCorrTo.Length > 0)
            {
                for (int i = 0; i < listaCorrTo.Length; i++)
                {
                    if (listaCorrTo[i].tipoCorrispondente != null && listaCorrTo[i].tipoCorrispondente.Equals("O"))
                    {
                        this.AppendDataRow(dt, listaCorrTo[i].tipoCorrispondente, listaCorrTo[i].systemId, "&nbsp;" + listaCorrTo[i].descrizione);
                    }
                    else
                    {
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        {
                            tipoURP = "U";
                        }
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        {
                            tipoURP = "R";
                        }
                        if (listaCorrTo[i].GetType().Equals(typeof(DocsPaWR.Utente)))
                        {
                            tipoURP = "P";
                        }
                        this.AppendDataRow(dt, listaCorrTo[i].tipoIE, listaCorrTo[i].systemId, this.GetImage(tipoURP) + " - " + listaCorrTo[i].descrizione);
                    }
                    this.ht_destinatariTO_CC.Add(listaCorrTo[i].systemId, listaCorrTo[i]);
                }
            }
            if (listaCorrCC != null && listaCorrCC.Length > 0)
            {
                for (int i = 0; i < listaCorrCC.Length; i++)
                {
                    if (listaCorrCC[i].tipoCorrispondente != null && listaCorrCC[i].tipoCorrispondente.Equals("O"))
                    {
                        this.AppendDataRow(dt, listaCorrCC[i].tipoCorrispondente, listaCorrCC[i].systemId, "&nbsp;" + listaCorrCC[i].descrizione + " (CC)");
                    }
                    else
                    {
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPaWR.UnitaOrganizzativa)))
                        {
                            tipoURP = "U";
                        }
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPaWR.Ruolo)))
                        {
                            tipoURP = "R";
                        }
                        if (listaCorrCC[i].GetType().Equals(typeof(DocsPaWR.Utente)))
                        {
                            tipoURP = "P";
                        }
                        this.AppendDataRow(dt, listaCorrCC[i].tipoIE, listaCorrCC[i].systemId, this.GetImage(tipoURP) + " - " + listaCorrCC[i].descrizione + " (CC)");
                    }
                    this.ht_destinatariTO_CC.Add(listaCorrCC[i].systemId, listaCorrCC[i]);
                }
            }
            if ((listaCorrTo != null && listaCorrTo.Length > 0) || (listaCorrCC != null && listaCorrCC.Length > 0))
            {
                this.pnl_corr.Visible = true;
            }
        }

        private void LoadTypeDocuments()
        {
            DocsPaWR.TipologiaAtto[] listaTipologiaAtto;
            if (this.CustomDocuments)
            {
                listaTipologiaAtto = DocumentManager.getTipoAttoPDInsRic(this, UserManager.GetInfoUser().idAmministrazione, RoleManager.GetRoleInSession().idGruppo, "1");
            }
            else
                listaTipologiaAtto = DocumentManager.getListaTipologiaAtto(this);

            this.DocumentDdlTypeDocument.Items.Clear();

            ListItem item = new ListItem(string.Empty, string.Empty);
            this.DocumentDdlTypeDocument.Items.Add(item);

            if (listaTipologiaAtto != null)
            {
                for (int i = 0; i < listaTipologiaAtto.Length; i++)
                {
                    item = new ListItem();
                    item.Text = listaTipologiaAtto[i].descrizione;
                    item.Value = listaTipologiaAtto[i].systemId;
                    this.DocumentDdlTypeDocument.Items.Add(item);
                }
            }
        }

        #region CAMPI PROFILO DEL DOCUMENTO
        protected void PopulateProfiledDocument()
        {
            this.PnlTypeDocument.Controls.Clear();
            this.inserisciComponenti(false);
        }

        private void inserisciComponenti(bool readOnly)
        {
            List<AssDocFascRuoli> dirittiCampiRuolo = ProfilerDocManager.getDirittiCampiTipologiaDoc(RoleManager.GetRoleInSession().idGruppo, this.Template.SYSTEM_ID.ToString());

            for (int i = 0, index = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
            {
                DocsPaWR.OggettoCustom oggettoCustom = (DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i];

                ProfilerDocManager.addNoRightsCustomObject(dirittiCampiRuolo, oggettoCustom);

                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                {
                    case "CampoDiTesto":
                        this.inserisciCampoDiTesto(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "CasellaDiSelezione":
                        this.inserisciCasellaDiSelezione(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "MenuATendina":
                        this.inserisciMenuATendina(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "SelezioneEsclusiva":
                        this.inserisciSelezioneEsclusiva(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Contatore":
                        this.inserisciContatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Data":
                        this.inserisciData(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Corrispondente":
                        SearchCorrespondentIntExtWithDisabled = true;
                        this.inserisciCorrispondente(oggettoCustom, readOnly, index++, dirittiCampiRuolo);
                        break;
                    case "Link":
                        //this.inserisciLink(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "ContatoreSottocontatore":
                        this.inserisciContatoreSottocontatore(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                    case "Separatore":
                        this.inserisciCampoSeparatore(oggettoCustom);
                        break;
                    case "OggettoEsterno":
                        this.inserisciOggettoEsterno(oggettoCustom, readOnly, dirittiCampiRuolo);
                        break;
                }
            }
        }

        public void inserisciOggettoEsterno(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichetta = new Label();
            etichetta.EnableViewState = true;

            if ("SI".Equals(oggettoCustom.CAMPO_OBBLIGATORIO))
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichetta.Text = oggettoCustom.DESCRIZIONE;
            }
            etichetta.CssClass = "weight";
            UserControls.IntegrationAdapter intAd = (UserControls.IntegrationAdapter)this.LoadControl("../UserControls/IntegrationAdapter.ascx");
            intAd.ID = oggettoCustom.SYSTEM_ID.ToString();
            intAd.View = UserControls.IntegrationAdapterView.INSERT_MODIFY;
            intAd.ManualInsertCssClass = "txt_textdata_counter_disabled_red";
            intAd.EnableViewState = true;
            //Verifico i diritti del ruolo sul campo
            impostaDirittiRuoloSulCampo(etichetta, intAd, oggettoCustom, this.Template, dirittiCampiRuolo);

            intAd.ConfigurationValue = oggettoCustom.CONFIG_OBJ_EST;
            IntegrationAdapterValue value = new IntegrationAdapterValue(oggettoCustom.CODICE_DB, oggettoCustom.VALORE_DATABASE, oggettoCustom.MANUAL_INSERT);
            intAd.Value = value;

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            if (etichetta.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichetta);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (intAd.Visible)
            {
                divColValue.Controls.Add(intAd);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

        }

        private void inserisciCampoSeparatore(DocsPaWR.OggettoCustom oggettoCustom)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoSeparatore = new Label();
            etichettaCampoSeparatore.CssClass = "weight";
            etichettaCampoSeparatore.EnableViewState = true;
            etichettaCampoSeparatore.Text = oggettoCustom.DESCRIZIONE.ToUpper();

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col_full_line";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCampoSeparatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaCampoSeparatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


        }

        private void inserisciContatoreSottocontatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
                return;

            Label etichettaContatoreSottocontatore = new Label();
            etichettaContatoreSottocontatore.EnableViewState = true;
            etichettaContatoreSottocontatore.Text = oggettoCustom.DESCRIZIONE;
            etichettaContatoreSottocontatore.CssClass = "weight";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatoreSottocontatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            //Ricerca contatore
            TextBox contatoreDa = new TextBox();
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.Text = oggettoCustom.VALORE_DATABASE;
            contatoreDa.Width = 40;
            contatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox contatoreA = new TextBox();
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.Text = oggettoCustom.VALORE_DATABASE;
            contatoreA.Width = 40;
            contatoreA.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreDa = new TextBox();
            sottocontatoreDa.ID = "da_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreDa.Width = 40;
            sottocontatoreDa.CssClass = "comp_profilazione_anteprima";

            TextBox sottocontatoreA = new TextBox();
            sottocontatoreA.ID = "a_sottocontatore_" + oggettoCustom.SYSTEM_ID.ToString();
            sottocontatoreA.Text = oggettoCustom.VALORE_SOTTOCONTATORE;
            sottocontatoreA.Width = 40;
            sottocontatoreA.CssClass = "comp_profilazione_anteprima";


            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }


            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaContatoreDa.Font.Size = FontUnit.Point(8);
            etichettaContatoreDa.Font.Bold = true;
            etichettaContatoreDa.Font.Name = "Verdana";
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.Text = "&nbsp;a&nbsp;";
            etichettaContatoreA.Font.Size = FontUnit.Point(8);
            etichettaContatoreA.Font.Bold = true;
            etichettaContatoreA.Font.Name = "Verdana";

            Label etichettaSottocontatoreDa = new Label();
            etichettaSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreDa.Font.Bold = true;
            etichettaSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaSottocontatoreA = new Label();
            etichettaSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaSottocontatoreA.Font.Bold = true;
            etichettaSottocontatoreA.Font.Name = "Verdana";

            Label etichettaDataSottocontatoreDa = new Label();
            etichettaDataSottocontatoreDa.Text = "<br/>&nbsp;&nbsp;da&nbsp;";
            etichettaDataSottocontatoreDa.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreDa.Font.Bold = true;
            etichettaDataSottocontatoreDa.Font.Name = "Verdana";
            Label etichettaDataSottocontatoreA = new Label();
            etichettaDataSottocontatoreA.Text = "&nbsp;a&nbsp;";
            etichettaDataSottocontatoreA.Font.Size = FontUnit.Point(8);
            etichettaDataSottocontatoreA.Font.Bold = true;
            etichettaDataSottocontatoreA.Font.Name = "Verdana";

            //TableRow row = new TableRow();
            //TableCell cell_1 = new TableCell();
            //cell_1.Controls.Add(etichettaContatoreSottocontatore);
            //row.Cells.Add(cell_1);

            //TableCell cell_2 = new TableCell();
            //


            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            // aggiunto default vuoto
            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            if (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }

                Ruolo ruoloUtente = RoleManager.GetRoleInSession();
                Registro[] registriRfVisibili = UIManager.RegistryManager.GetListRegistriesAndRF(ruoloUtente.systemId, string.Empty, string.Empty);

                Panel divColDllEti = new Panel();
                divColDllEti.CssClass = "col";
                divColDllEti.EnableViewState = true;

                Panel divColDll = new Panel();
                divColDll.CssClass = "col";
                divColDll.EnableViewState = true;

                switch (oggettoCustom.TIPO_CONTATORE)
                {
                    case "T":
                        break;
                    case "A":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "0" && !((Registro)registriRfVisibili[i]).Sospeso)
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                    case "R":
                        paneldll = true;
                        etichettaDDL.Text = "&nbsp;RF&nbsp;";
                        etichettaDDL.CssClass = "weight";
                        etichettaDDL.Width = 50;
                        ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                        ddl.CssClass = "chzn-select-deselect";
                        ddl.Width = 240;

                        //Distinguo se è un registro o un rf
                        for (int i = 0; i < registriRfVisibili.Length; i++)
                        {
                            ListItem item = new ListItem();
                            if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                            {
                                item.Value = ((Registro)registriRfVisibili[i]).systemId;
                                item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                                ddl.Items.Add(item);
                            }
                        }

                        parDll.Controls.Add(etichettaDDL);
                        divColDllEti.Controls.Add(parDll);
                        divRowEtiDll.Controls.Add(divColDllEti);

                        divColDll.Controls.Add(ddl);
                        divRowDll.Controls.Add(divColDll);
                        break;
                }
            }

            if (!oggettoCustom.VALORE_SOTTOCONTATORE.Equals(""))
            {
                if (oggettoCustom.VALORE_SOTTOCONTATORE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_SOTTOCONTATORE.Split('@');
                    sottocontatoreDa.Text = contatori[0].ToString();
                    sottocontatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    sottocontatoreDa.Text = oggettoCustom.VALORE_SOTTOCONTATORE.ToString();
                    sottocontatoreA.Text = "";
                }
            }
        }

        private void inserisciCorrispondente(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue corrOldOb = new StoricoProfilatiOldValue();

            UserControls.CorrespondentCustom corrispondente = (UserControls.CorrespondentCustom)this.LoadControl("../UserControls/CorrespondentCustom.ascx");
            corrispondente.EnableViewState = true;

            corrispondente.TxtEtiCustomCorrespondent = oggettoCustom.DESCRIZIONE;

            corrispondente.TypeCorrespondentCustom = oggettoCustom.TIPO_RICERCA_CORR;
            corrispondente.ID = oggettoCustom.SYSTEM_ID.ToString();

            //Da amministrazione è stato impostato un ruolo di default per questo campo.
            if (!string.IsNullOrEmpty(oggettoCustom.ID_RUOLO_DEFAULT) && oggettoCustom.ID_RUOLO_DEFAULT != "0")
            {
                DocsPaWR.Ruolo ruolo = RoleManager.getRuoloById(oggettoCustom.ID_RUOLO_DEFAULT);
                if (ruolo != null)
                {
                    corrispondente.IdCorrespondentCustom = ruolo.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = ruolo.codiceRubrica;
                    corrispondente.TxtDescriptionCorrespondentCustom = ruolo.descrizione;
                }
                oggettoCustom.ID_RUOLO_DEFAULT = "0";
            }

            //Il campo è valorizzato.
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                DocsPaWR.Corrispondente corr_1 = AddressBookManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                if (corr_1 != null)
                {
                    corrispondente.IdCorrespondentCustom = corr_1.systemId;
                    corrispondente.TxtCodeCorrespondentCustom = corr_1.codiceRubrica.ToString();
                    corrispondente.TxtDescriptionCorrespondentCustom = corr_1.descrizione.ToString();
                    oggettoCustom.VALORE_DATABASE = corr_1.systemId;
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(corrispondente.TxtEtiCustomCorrespondent, corrispondente, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (corrispondente.Visible)
            {
                this.PnlTypeDocument.Controls.Add(corrispondente);
            }

        }

        private void inserisciData(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            //Per il momento questo tipo di campo è stato implementato con tre semplici textBox
            //Sarebbe opportuno creare un oggetto personalizzato, che espone le stesse funzionalità
            //della textBox, ma che mi permette di gestire la data con i tre campi separati.
            DocsPaWR.StoricoProfilatiOldValue dataOldOb = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaData = new Label();
            etichettaData.EnableViewState = true;


            etichettaData.Text = oggettoCustom.DESCRIZIONE;

            etichettaData.CssClass = "weight";

            UserControls.Calendar data = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data.EnableViewState = true;
            data.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            data.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data.SetEnableTimeMode();

            UserControls.Calendar data2 = (UserControls.Calendar)this.LoadControl("../UserControls/Calendar.ascx");
            data2.EnableViewState = true;
            data2.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            data2.VisibleTimeMode = ProfilerDocManager.getVisibleTimeMode(oggettoCustom);
            data2.SetEnableTimeMode();

            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] date = oggettoCustom.VALORE_DATABASE.Split('@');
                    //dataDa.txt_Data.Text = date[0].ToString();
                    //dataA.txt_Data.Text = date[1].ToString();
                    data.Text = date[0].ToString();
                    data2.Text = date[1].ToString();
                }
                else
                {
                    //dataDa.txt_Data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    //data.txt_Data.Text = "";
                    data.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    data2.Text = "";
                }
            }

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaData, data, oggettoCustom, this.Template, dirittiCampiRuolo);

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaData);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            if (etichettaData.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            Label etichettaDataFrom = new Label();
            etichettaDataFrom.EnableViewState = true;
            etichettaDataFrom.Text = "Da";

            HtmlGenericControl parDescFrom = new HtmlGenericControl("p");
            parDescFrom.Controls.Add(etichettaDataFrom);
            parDescFrom.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divColValueFrom = new Panel();
            divColValueFrom.CssClass = "col";
            divColValueFrom.EnableViewState = true;

            divColValueFrom.Controls.Add(parDescFrom);
            divRowValueFrom.Controls.Add(divColValueFrom);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(data);
            divRowValue.Controls.Add(divColValue);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }

            //////
            Label etichettaDataTo = new Label();
            etichettaDataTo.EnableViewState = true;
            etichettaDataTo.Text = "A";

            Panel divRowValueTo = new Panel();
            divRowValueTo.CssClass = "row";
            divRowValueTo.EnableViewState = true;

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            HtmlGenericControl parDescTo = new HtmlGenericControl("p");
            parDescTo.Controls.Add(etichettaDataTo);
            parDescTo.EnableViewState = true;

            divColValueTo.Controls.Add(parDescTo);
            divRowValueTo.Controls.Add(divColValueTo);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueTo);
            }

            Panel divRowValue2 = new Panel();
            divRowValue2.CssClass = "row";
            divRowValue2.EnableViewState = true;


            Panel divColValue2 = new Panel();
            divColValue2.CssClass = "col";
            divColValue2.EnableViewState = true;

            divColValue2.Controls.Add(data2);
            divRowValue2.Controls.Add(divColValue2);

            if (data.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue2);
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "refreshPicker", "DatePicker('" + UIManager.UserManager.GetLanguageData() + "');", true);
        }

        private void inserisciContatore(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            bool paneldll = false;

            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaContatore = new Label();
            etichettaContatore.EnableViewState = true;


            etichettaContatore.Text = oggettoCustom.DESCRIZIONE;

            etichettaContatore.CssClass = "weight";

            CustomTextArea contatoreDa = new CustomTextArea();
            contatoreDa.EnableViewState = true;
            contatoreDa.ID = "da_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreDa.CssClass = "txt_textdata";

            CustomTextArea contatoreA = new CustomTextArea();
            contatoreA.EnableViewState = true;
            contatoreA.ID = "a_" + oggettoCustom.SYSTEM_ID.ToString();
            contatoreA.CssClass = "txt_textdata";

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaContatore);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);
            divRowDesc.Controls.Add(divColDesc);

            //Ruolo ruoloUtente = RoleManager.GetRoleInSession();
            //Registro[] registriRfVisibili = UserManager.getListaRegistriWithRF(ruoloUtente.systemId, string.Empty, string.Empty);
            Registro[] registriRfVisibili = RegistryManager.GetRegAndRFListInSession();
            //Le dropDownLsit delle AOO o RF e la checkbox per il contaDopo vanno considerati e visualizzati
            //solo nel caso di un contatore non valorizzato, altrimenti deve essere riporato solo il valore 
            //del contatore come da formato prescelto e in readOnly
            Label etichettaDDL = new Label();
            etichettaDDL.EnableViewState = true;
            etichettaDDL.Width = 50;
            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;

            string language = UIManager.UserManager.GetUserLanguage();
            ddl.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));

            //Emanuela 19-05-2014: aggiunto default vuoto
            ddl.Items.Add(new ListItem() { Text = "", Value = "" });

            Panel divRowDll = new Panel();
            divRowDll.CssClass = "row";
            divRowDll.EnableViewState = true;

            Panel divRowEtiDll = new Panel();
            divRowEtiDll.CssClass = "row";
            divRowEtiDll.EnableViewState = true;

            HtmlGenericControl parDll = new HtmlGenericControl("p");
            parDll.EnableViewState = true;

            Panel divColDllEti = new Panel();
            divColDllEti.CssClass = "col";
            divColDllEti.EnableViewState = true;

            Panel divColDll = new Panel();
            divColDll.CssClass = "col";
            divColDll.EnableViewState = true;


            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            switch (oggettoCustom.TIPO_CONTATORE)
            {
                case "T":
                    break;
                case "A":
                    paneldll = true;
                    etichettaDDL.Text = "&nbsp;AOO&nbsp;";
                    etichettaDDL.CssClass = "weight";
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "chzn-select-deselect";
                    ddl.Width = 240;

                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((Registro)registriRfVisibili[i]).chaRF == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    //  ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);
                    break;
                case "R":
                    paneldll = true;
                    etichettaDDL.Text = "&nbsp;RF&nbsp;";
                    etichettaDDL.CssClass = "weight";
                    ddl.ID = oggettoCustom.SYSTEM_ID.ToString() + "_menu";
                    ddl.CssClass = "chzn-select-deselect";
                    ddl.Width = 240;

                    //Distinguo se è un registro o un rf
                    for (int i = 0; i < registriRfVisibili.Length; i++)
                    {
                        ListItem item = new ListItem();
                        if (((Registro)registriRfVisibili[i]).chaRF == "1" && ((Registro)registriRfVisibili[i]).rfDisabled == "0")
                        {
                            item.Value = ((Registro)registriRfVisibili[i]).systemId;
                            item.Text = ((Registro)registriRfVisibili[i]).codRegistro;
                            ddl.Items.Add(item);
                        }
                    }
                    /*
                     * Emanuela 21-05-2014: commento per far si che come RF di default venga mostrato l'item vuoto
                    if (ddl.Items.Count == 1)
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;
                    else
                        ddl.Items.Insert(0, new ListItem(""));
                    */

                    if (!string.IsNullOrEmpty(oggettoCustom.ID_AOO_RF) && !oggettoCustom.ID_AOO_RF.Equals("0"))
                        ddl.SelectedValue = oggettoCustom.ID_AOO_RF;

                    ddl.CssClass = "chzn-select-deselect";

                    parDll.Controls.Add(etichettaDDL);
                    divColDllEti.Controls.Add(parDll);
                    divRowEtiDll.Controls.Add(divColDllEti);

                    divColDll.Controls.Add(ddl);
                    divRowDll.Controls.Add(divColDll);

                    break;
            }

            if (etichettaContatore.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                if (oggettoCustom.VALORE_DATABASE.IndexOf("@") != -1)
                {
                    string[] contatori = oggettoCustom.VALORE_DATABASE.Split('@');
                    contatoreDa.Text = contatori[0].ToString();
                    contatoreA.Text = contatori[1].ToString();
                }
                else
                {
                    contatoreDa.Text = oggettoCustom.VALORE_DATABASE.ToString();
                    contatoreA.Text = "";
                }
            }

            Label etichettaContatoreDa = new Label();
            etichettaContatoreDa.EnableViewState = true;
            etichettaContatoreDa.Text = "Da";

            //////
            Label etichettaContatoreA = new Label();
            etichettaContatoreA.EnableViewState = true;
            etichettaContatoreA.Text = "A";

            Panel divColValueTo = new Panel();
            divColValueTo.CssClass = "col";
            divColValueTo.EnableViewState = true;

            Panel divRowValueFrom = new Panel();
            divRowValueFrom.CssClass = "row";
            divRowValueFrom.EnableViewState = true;

            Panel divCol1 = new Panel();
            divCol1.CssClass = "col";
            divCol1.EnableViewState = true;

            Panel divCol2 = new Panel();
            divCol2.CssClass = "col";
            divCol2.EnableViewState = true;

            Panel divCol3 = new Panel();
            divCol3.CssClass = "col";
            divCol3.EnableViewState = true;

            Panel divCol4 = new Panel();
            divCol4.CssClass = "col";
            divCol4.EnableViewState = true;

            divCol1.Controls.Add(etichettaContatoreDa);
            divCol2.Controls.Add(contatoreDa);
            divCol3.Controls.Add(etichettaContatoreA);
            divCol4.Controls.Add(contatoreA);
            divRowValueFrom.Controls.Add(divCol1);
            divRowValueFrom.Controls.Add(divCol2);
            divRowValueFrom.Controls.Add(divCol3);
            divRowValueFrom.Controls.Add(divCol4);

            impostaDirittiRuoloContatore(etichettaContatore, contatoreDa, contatoreA, etichettaContatoreDa, etichettaContatoreA, etichettaDDL, ddl, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (paneldll)
            {
                this.PnlTypeDocument.Controls.Add(divRowEtiDll);
                this.PnlTypeDocument.Controls.Add(divRowDll);
            }

            if (contatoreDa.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValueFrom);
            }
            #region DATA REPERTORIAZIONE
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_DATA_REPERTORIO.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_ENABLE_DATA_REPERTORIO.ToString()).Equals("1"))
            {
                Label dataRepertorio = new Label();
                dataRepertorio.EnableViewState = true;
                if (oggettoCustom.REPERTORIO.Equals("1"))
                    dataRepertorio.Text = Utils.Languages.GetLabelFromCode("SearhDocumentDataRepertorio", language);
                else
                    dataRepertorio.Text = Utils.Languages.GetLabelFromCode("SearhDocumentDataInserimentoContatore", language);
                dataRepertorio.CssClass = "weight";
                Panel divEtichettaDataRepertorio = new Panel();
                divEtichettaDataRepertorio.CssClass = "row";
                divEtichettaDataRepertorio.EnableViewState = true;
                divEtichettaDataRepertorio.Controls.Add(dataRepertorio);

                Panel divFiltriDataRepertorio = new Panel();
                divFiltriDataRepertorio.CssClass = "row";
                divFiltriDataRepertorio.EnableViewState = true;

                Panel divFiltriDdlIntervalloDataRepertorio = new Panel();
                divFiltriDdlIntervalloDataRepertorio.CssClass = "col";
                divFiltriDdlIntervalloDataRepertorio.EnableViewState = true;
                DropDownList ddlIntervalloDataRepertorio = new DropDownList();
                ddlIntervalloDataRepertorio.EnableViewState = true;
                ddlIntervalloDataRepertorio.ID = "DdlIntervalloDataRepertorio_" + oggettoCustom.SYSTEM_ID.ToString();
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "0", Text = Utils.Languages.GetLabelFromCode("ddl_data0", language), Selected = true });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "1", Text = Utils.Languages.GetLabelFromCode("ddl_data1", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "2", Text = Utils.Languages.GetLabelFromCode("ddl_data2", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "3", Text = Utils.Languages.GetLabelFromCode("ddl_data3", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "4", Text = Utils.Languages.GetLabelFromCode("ddl_data4", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "5", Text = Utils.Languages.GetLabelFromCode("ddl_data5", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "6", Text = Utils.Languages.GetLabelFromCode("ddl_data6", language) });
                ddlIntervalloDataRepertorio.Items.Add(new ListItem() { Value = "7", Text = Utils.Languages.GetLabelFromCode("ddl_data7", language) });
                ddlIntervalloDataRepertorio.AutoPostBack = true;
                ddlIntervalloDataRepertorio.SelectedIndexChanged += DdlIntervalloDataRepertorio_SelectedIndexChanged;
                divFiltriDdlIntervalloDataRepertorio.Controls.Add(ddlIntervalloDataRepertorio);

                Panel divFiltriDataDa = new Panel();
                divFiltriDataDa.CssClass = "col";
                divFiltriDataDa.EnableViewState = true;
                Panel divFiltriLblDataDa = new Panel();
                divFiltriLblDataDa.CssClass = "col-no-margin-top";
                divFiltriLblDataDa.EnableViewState = true;
                Label lblDataDa = new Label();
                lblDataDa.ID = "LblDataRepertorioDa_" + oggettoCustom.SYSTEM_ID;
                lblDataDa.EnableViewState = true;
                lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                lblDataDa.CssClass = "weight";
                divFiltriLblDataDa.Controls.Add(lblDataDa);
                divFiltriDataDa.Controls.Add(divFiltriLblDataDa);
                CustomTextArea dataDa = new CustomTextArea();
                dataDa.EnableViewState = true;
                dataDa.ID = "TxtDataRepertorioDa_" + oggettoCustom.SYSTEM_ID.ToString();
                dataDa.CssClass = "txt_textdata datepicker";
                dataDa.CssClassReadOnly = "txt_textdata_disabled";
                dataDa.Style["width"] = "80px";
                divFiltriDataDa.Controls.Add(dataDa);

                Panel divFiltriDataA = new Panel();
                divFiltriDataA.CssClass = "col";
                divFiltriDataA.EnableViewState = true;
                Panel divFiltriLblDataA = new Panel();
                divFiltriLblDataA.CssClass = "col-no-margin-top";
                divFiltriLblDataA.EnableViewState = true;
                Label lblDataA = new Label();
                lblDataA.ID = "LblDataRepertorioA_" + oggettoCustom.SYSTEM_ID;
                lblDataA.EnableViewState = true;
                lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                lblDataA.CssClass = "weight";
                lblDataA.Visible = false;
                divFiltriLblDataA.Controls.Add(lblDataA);
                divFiltriDataA.Controls.Add(divFiltriLblDataA);
                CustomTextArea dataA = new CustomTextArea();
                dataA.EnableViewState = true;
                dataA.ID = "TxtDataRepertorioA_" + oggettoCustom.SYSTEM_ID.ToString();
                dataA.CssClass = "txt_textdata datepicker";
                dataA.CssClassReadOnly = "txt_textdata_disabled";
                dataA.Style["width"] = "80px";
                dataA.Visible = false;
                divFiltriDataA.Controls.Add(dataA);

                divFiltriDataRepertorio.Controls.Add(divFiltriDdlIntervalloDataRepertorio);
                divFiltriDataRepertorio.Controls.Add(divFiltriDataDa);
                divFiltriDataRepertorio.Controls.Add(divFiltriDataA);

                Panel divRowDataRepertorio = new Panel();
                divRowDataRepertorio.CssClass = "row";
                divRowDataRepertorio.EnableViewState = true;

                divRowDataRepertorio.Controls.Add(divEtichettaDataRepertorio);
                divRowDataRepertorio.Controls.Add(divFiltriDataRepertorio);

                if (contatoreDa.Visible)
                {
                    this.PnlTypeDocument.Controls.Add(divRowDataRepertorio);
                }

                #region BindFilterDataRepertorio

                if (!string.IsNullOrEmpty(oggettoCustom.DATA_INSERIMENTO))
                {
                    if (oggettoCustom.DATA_INSERIMENTO.IndexOf("@") != -1)
                    {
                        ddlIntervalloDataRepertorio.SelectedIndex = 1;
                        dataDa.ReadOnly = false;
                        dataA.ReadOnly = false;
                        lblDataA.Visible = true;
                        lblDataDa.Visible = true;
                        dataA.Visible = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        string[] dataInserimento = oggettoCustom.DATA_INSERIMENTO.Split('@');
                        dataDa.Text = dataInserimento[0].ToString();
                        dataA.Text = dataInserimento[1].ToString();
                    }
                    else
                    {
                        dataDa.Text = oggettoCustom.DATA_INSERIMENTO.ToString();
                        dataA.Text = "";
                    }
                }

                #endregion
            }
            #endregion
        }

        protected void DdlIntervalloDataRepertorio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((DropDownList)sender).ID).Replace("DdlIntervalloDataRepertorio_", "");
                DropDownList dlIntervalloDataRepertorio = (DropDownList)sender;
                CustomTextArea dataDa = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioDa_" + idOggetto);
                Label lblDataDa = (Label)this.PnlTypeDocument.FindControl("LblDataRepertorioDa_" + idOggetto);
                CustomTextArea dataA = (CustomTextArea)this.PnlTypeDocument.FindControl("TxtDataRepertorioA_" + idOggetto);
                Label lblDataA = (Label)this.PnlTypeDocument.FindControl("LblDataRepertorioA_" + idOggetto);
                string language = UIManager.UserManager.GetUserLanguage();
                switch (dlIntervalloDataRepertorio.SelectedIndex)
                {
                    case 0: //Valore singolo
                        dataDa.ReadOnly = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        lblDataA.Visible = false;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 1: //Intervallo
                        dataDa.ReadOnly = false;
                        dataA.ReadOnly = false;
                        lblDataA.Visible = true;
                        lblDataDa.Visible = true;
                        dataA.Visible = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 2: //Oggi
                        lblDataA.Visible = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        dataDa.ReadOnly = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.toDay();
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 3: //Settimana corrente
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfWeek();
                        dataA.Text = NttDataWA.Utils.dateformat.getLastDayOfWeek();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 4: //Mese corrente
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.getFirstDayOfMonth();
                        dataA.Text = NttDataWA.Utils.dateformat.getLastDayOfMonth();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 5: //Ieri
                        lblDataA.Visible = false;
                        dataA.Visible = false;
                        dataA.Text = string.Empty;
                        dataDa.ReadOnly = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetYesterday();
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("VisibilityOneField", language);
                        break;
                    case 6: //Ultimi 7 giorni
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetLastSevenDay();
                        dataA.Text = NttDataWA.Utils.dateformat.toDay();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                    case 7: //Ultimi 31 giorni
                        lblDataA.Visible = true;
                        dataA.Visible = true;
                        dataDa.Text = NttDataWA.Utils.dateformat.GetLastThirtyOneDay();
                        dataA.Text = NttDataWA.Utils.dateformat.toDay();
                        dataA.ReadOnly = true;
                        dataDa.ReadOnly = true;
                        lblDataDa.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlDa", language);
                        lblDataA.Text = Utils.Languages.GetLabelFromCode("SearchDocumentAdvancedLtlA", language);
                        break;
                }
                OggettoCustom oggetto = (from o in this.Template.ELENCO_OGGETTI where o.TIPO.DESCRIZIONE_TIPO.Equals("Contatore") select o).FirstOrDefault();
                if (oggetto != null)
                    this.controllaCampi(oggetto, oggetto.SYSTEM_ID.ToString());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloContatore(System.Object etichettaContatore, System.Object contatoreDa, System.Object contatoreA, System.Object etichettaContatoreDa, System.Object etichettaContatoreA, System.Object etichettaDDL, System.Object ddl, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichettaContatore).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaContatoreDa).Visible = false;
                        ((CustomTextArea)contatoreDa).Visible = false;
                        ((System.Web.UI.WebControls.Label)etichettaContatoreA).Visible = false;
                        ((CustomTextArea)contatoreA).Visible = false;

                        ((System.Web.UI.WebControls.Label)etichettaDDL).Visible = false;
                        ((System.Web.UI.WebControls.DropDownList)ddl).Visible = false;
                    }
                }
            }
        }

        private void inserisciSelezioneEsclusiva(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            DocsPaWR.StoricoProfilatiOldValue selezEsclOldObj = new StoricoProfilatiOldValue();
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            Label etichettaSelezioneEsclusiva = new Label();
            etichettaSelezioneEsclusiva.EnableViewState = true;
            CustomImageButton cancella_selezioneEsclusiva = new CustomImageButton();
            string language = UIManager.UserManager.GetUserLanguage();
            cancella_selezioneEsclusiva.AlternateText = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.ToolTip = Utils.Languages.GetLabelFromCode("LinkDocFascBtn_Reset", language);
            cancella_selezioneEsclusiva.EnableViewState = true;


            etichettaSelezioneEsclusiva.Text = oggettoCustom.DESCRIZIONE;


            cancella_selezioneEsclusiva.ID = "_" + oggettoCustom.SYSTEM_ID.ToString();
            cancella_selezioneEsclusiva.ImageUrl = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOutImage = "../Images/Icons/clean_field_custom.png";
            cancella_selezioneEsclusiva.OnMouseOverImage = "../Images/Icons/clean_field_custom_hover.png";
            cancella_selezioneEsclusiva.ImageUrlDisabled = "../Images/Icons/clean_field_custom_disabled.png";
            cancella_selezioneEsclusiva.CssClass = "clickable";
            cancella_selezioneEsclusiva.Click += cancella_selezioneEsclusiva_Click;
            etichettaSelezioneEsclusiva.CssClass = "weight";

            RadioButtonList selezioneEsclusiva = new RadioButtonList();
            selezioneEsclusiva.EnableViewState = true;
            selezioneEsclusiva.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                {
                    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                        valoreOggetto.ABILITATO = 1;

                    selezioneEsclusiva.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                    //Valore di default
                    //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                    //{
                    //    valoreDiDefault = i;
                    //}
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                selezioneEsclusiva.RepeatDirection = RepeatDirection.Vertical;
            }
            //if (valoreDiDefault != -1)
            //{
            //    selezioneEsclusiva.SelectedIndex = valoreDiDefault;
            //}
            if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
            {
                selezioneEsclusiva.SelectedIndex = impostaSelezioneEsclusiva(oggettoCustom.VALORE_DATABASE, selezioneEsclusiva);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divColImage = new Panel();
            divColImage.CssClass = "col-right-no-margin";
            divColImage.EnableViewState = true;

            divColImage.Controls.Add(cancella_selezioneEsclusiva);

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaSelezioneEsclusiva);
            parDesc.EnableViewState = true;

            divColDesc.Controls.Add(parDesc);

            divRowDesc.Controls.Add(divColDesc);
            divRowDesc.Controls.Add(divColImage);


            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            divColValue.Controls.Add(selezioneEsclusiva);
            divRowValue.Controls.Add(divColValue);



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSelezioneEsclusiva(etichettaSelezioneEsclusiva, selezioneEsclusiva, cancella_selezioneEsclusiva, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaSelezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (selezioneEsclusiva.Visible)
            {
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        protected void cancella_selezioneEsclusiva_Click(object sender, EventArgs e)
        {
            try
            {
                string idOggetto = (((CustomImageButton)sender).ID).Substring(1);
                ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).SelectedIndex = -1;
                ((RadioButtonList)this.PnlTypeDocument.FindControl(idOggetto)).EnableViewState = true;
                for (int i = 0; i < this.Template.ELENCO_OGGETTI.Length; i++)
                {
                    if (((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).SYSTEM_ID.ToString().Equals(idOggetto))
                    {
                        ((DocsPaWR.OggettoCustom)this.Template.ELENCO_OGGETTI[i]).VALORE_DATABASE = "";
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        public void impostaDirittiRuoloSelezioneEsclusiva(System.Object etichetta, System.Object campo, System.Object button, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                    {
                        ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                        ((System.Web.UI.WebControls.RadioButtonList)campo).Visible = false;
                        //((System.Web.UI.HtmlControls.HtmlAnchor)button).Visible = false;
                        oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                    }
                }
            }
        }

        private void inserisciMenuATendina(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            DocsPaWR.StoricoProfilatiOldValue menuOldObj = new StoricoProfilatiOldValue();
            Label etichettaMenuATendina = new Label();
            etichettaMenuATendina.EnableViewState = true;
            etichettaMenuATendina.Text = oggettoCustom.DESCRIZIONE;

            etichettaMenuATendina.CssClass = "weight";

            int maxLenght = 0;
            DropDownList menuATendina = new DropDownList();
            menuATendina.EnableViewState = true;
            menuATendina.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreOggetto = ((DocsPaWR.ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                //Valori disabilitati/abilitati
                //if (valoreOggetto.ABILITATO == 1 || (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE))
                //{
                //    //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                //    if (valoreOggetto.ABILITATO == 0 && valoreOggetto.VALORE == oggettoCustom.VALORE_DATABASE)
                //        valoreOggetto.ABILITATO = 1;

                menuATendina.Items.Add(new ListItem(valoreOggetto.VALORE, valoreOggetto.VALORE));
                //Valore di default
                //if (valoreOggetto.VALORE_DI_DEFAULT.Equals("SI"))
                //{
                //    valoreDiDefault = i;
                //}

                if (maxLenght < valoreOggetto.VALORE.Length)
                {
                    maxLenght = valoreOggetto.VALORE.Length;
                }
                //  }
            }
            menuATendina.CssClass = "chzn-select-deselect";
            string language = UIManager.UserManager.GetUserLanguage();
            menuATendina.Attributes.Add("data-placeholder", Utils.Languages.GetLabelFromCode("SelectProflierMenu", language));
            menuATendina.Width = maxLenght + 250;

            //if (valoreDiDefault != -1)
            //{
            //    menuATendina.SelectedIndex = valoreDiDefault;
            //}
            //if (!(valoreDiDefault != -1 && oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI")))
            //{
            menuATendina.Items.Insert(0, "");
            //}
            if (!oggettoCustom.VALORE_DATABASE.Equals(""))
            {
                menuATendina.SelectedIndex = this.impostaSelezioneMenuATendina(oggettoCustom.VALORE_DATABASE, menuATendina);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaMenuATendina);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;

            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaMenuATendina, menuATendina, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaMenuATendina.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }


            if (menuATendina.Visible)
            {
                divColValue.Controls.Add(menuATendina);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private void inserisciCampoDiTesto(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }

            Label etichettaCampoDiTesto = new Label();
            etichettaCampoDiTesto.EnableViewState = true;

            CustomTextArea txt_CampoDiTesto = new CustomTextArea();
            txt_CampoDiTesto.EnableViewState = true;

            if (oggettoCustom.MULTILINEA.Equals("SI"))
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

                etichettaCampoDiTesto.CssClass = "weight";

                txt_CampoDiTesto.CssClass = "txt_textarea";
                txt_CampoDiTesto.CssClassReadOnly = "txt_textarea_disabled";

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_LINEE))
                {
                    txt_CampoDiTesto.Height = 55;
                }
                else
                {
                    txt_CampoDiTesto.Rows = Convert.ToInt32(oggettoCustom.NUMERO_DI_LINEE);
                }

                if (string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    txt_CampoDiTesto.MaxLength = 150;
                }
                else
                {
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }

                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.TextMode = TextBoxMode.MultiLine;
            }
            else
            {

                etichettaCampoDiTesto.Text = oggettoCustom.DESCRIZIONE;

                etichettaCampoDiTesto.CssClass = "weight";

                if (!string.IsNullOrEmpty(oggettoCustom.NUMERO_DI_CARATTERI))
                {
                    //ATTENZIONE : La lunghezza della textBox non è speculare al numero massimo di
                    //caratteri che l'utente inserisce.
                    if (((Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6) <= 400))
                    {
                        txt_CampoDiTesto.Width = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI) * 6;
                    }
                    txt_CampoDiTesto.MaxLength = Convert.ToInt32(oggettoCustom.NUMERO_DI_CARATTERI);
                }
                txt_CampoDiTesto.ID = oggettoCustom.SYSTEM_ID.ToString();
                txt_CampoDiTesto.Text = oggettoCustom.VALORE_DATABASE;
                txt_CampoDiTesto.CssClass = "txt_input_full";
                txt_CampoDiTesto.CssClassReadOnly = "txt_input_full_disabled";
                txt_CampoDiTesto.TextMode = TextBoxMode.SingleLine;


            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColValue.EnableViewState = true;


            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCampoDiTesto, txt_CampoDiTesto, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaCampoDiTesto.Visible)
            {
                HtmlGenericControl parDesc = new HtmlGenericControl("p");
                parDesc.Controls.Add(etichettaCampoDiTesto);
                parDesc.EnableViewState = true;
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (txt_CampoDiTesto.Visible)
            {
                divColValue.Controls.Add(txt_CampoDiTesto);
                divRowValue.Controls.Add(divColValue);
                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private void inserisciCasellaDiSelezione(DocsPaWR.OggettoCustom oggettoCustom, bool readOnly, int index, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            if (string.IsNullOrEmpty(oggettoCustom.DESCRIZIONE))
            {
                return;
            }
            DocsPaWR.StoricoProfilatiOldValue casellaSelOldObj = new StoricoProfilatiOldValue();
            Label etichettaCasellaSelezione = new Label();
            etichettaCasellaSelezione.EnableViewState = true;

            if (oggettoCustom.CAMPO_OBBLIGATORIO.Equals("SI"))
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE + " *";
            }
            else
            {
                etichettaCasellaSelezione.Text = oggettoCustom.DESCRIZIONE;
            }

            etichettaCasellaSelezione.Width = Unit.Percentage(100);
            etichettaCasellaSelezione.CssClass = "weight";

            CheckBoxList casellaSelezione = new CheckBoxList();
            casellaSelezione.EnableViewState = true;
            casellaSelezione.ID = oggettoCustom.SYSTEM_ID.ToString();
            //int valoreDiDefault = -1;
            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Length; i++)
            {
                DocsPaWR.ValoreOggetto valoreElenco = ((ValoreOggetto)(oggettoCustom.ELENCO_VALORI[i]));
                if (i < oggettoCustom.VALORI_SELEZIONATI.Length)
                {
                    string valoreSelezionato = (string)(oggettoCustom.VALORI_SELEZIONATI[i]);
                    if (valoreElenco.ABILITATO == 1 || (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato)))
                    {
                        //Nel caso il valore è disabilitato ma selezionato lo rendo disponibile solo fino al salvataggio del documento 
                        if (valoreElenco.ABILITATO == 0 && !string.IsNullOrEmpty(valoreSelezionato))
                            valoreElenco.ABILITATO = 1;

                        casellaSelezione.Items.Add(new ListItem(valoreElenco.VALORE, valoreElenco.VALORE));
                        //Valore di default
                        //if (valoreElenco.VALORE_DI_DEFAULT.Equals("SI"))
                        //{
                        //    valoreDiDefault = i;
                        //}
                    }
                }
            }

            if (oggettoCustom.ORIZZONTALE_VERTICALE.Equals("Orizzontale"))
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Horizontal;
            }
            else
            {
                casellaSelezione.RepeatDirection = RepeatDirection.Vertical;
            }
            //if (valoreDiDefault != -1)
            //{
            //    casellaSelezione.SelectedIndex = valoreDiDefault;
            //}

            if (oggettoCustom.VALORI_SELEZIONATI != null)
            {
                this.impostaSelezioneCaselleDiSelezione(oggettoCustom, casellaSelezione);
            }

            Panel divRowDesc = new Panel();
            divRowDesc.CssClass = "row";
            divRowDesc.EnableViewState = true;

            Panel divColDesc = new Panel();
            divColDesc.CssClass = "col";
            divColDesc.EnableViewState = true;

            HtmlGenericControl parDesc = new HtmlGenericControl("p");
            parDesc.Controls.Add(etichettaCasellaSelezione);
            parDesc.EnableViewState = true;



            Panel divRowValue = new Panel();
            divRowValue.CssClass = "row";
            divRowValue.EnableViewState = true;

            Panel divColValue = new Panel();
            divColValue.CssClass = "col_full";
            divColDesc.EnableViewState = true;



            //Verifico i diritti del ruolo sul campo
            this.impostaDirittiRuoloSulCampo(etichettaCasellaSelezione, casellaSelezione, oggettoCustom, this.Template, dirittiCampiRuolo);

            if (etichettaCasellaSelezione.Visible)
            {
                divColDesc.Controls.Add(parDesc);
                divRowDesc.Controls.Add(divColDesc);
                this.PnlTypeDocument.Controls.Add(divRowDesc);
            }

            if (casellaSelezione.Visible)
            {

                divColValue.Controls.Add(casellaSelezione);
                divRowValue.Controls.Add(divColValue);

                this.PnlTypeDocument.Controls.Add(divRowValue);
            }
        }

        private int impostaSelezioneEsclusiva(string valore, RadioButtonList rbl)
        {
            for (int i = 0; i < rbl.Items.Count; i++)
            {
                if (rbl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        private void impostaSelezioneCaselleDiSelezione(DocsPaWR.OggettoCustom objCustom, CheckBoxList cbl)
        {
            for (int i = 0; i < objCustom.VALORI_SELEZIONATI.Length; i++)
            {
                for (int j = 0; j < cbl.Items.Count; j++)
                {
                    if ((string)objCustom.VALORI_SELEZIONATI[i] == cbl.Items[j].Text)
                    {
                        cbl.Items[j].Selected = true;
                    }
                }
            }
        }

        private int impostaSelezioneMenuATendina(string valore, DropDownList ddl)
        {
            for (int i = 0; i < ddl.Items.Count; i++)
            {
                if (ddl.Items[i].Text == valore)
                    return i;
            }
            return 0;
        }

        public void impostaDirittiRuoloSulCampo(System.Object etichetta, System.Object campo, DocsPaWR.OggettoCustom oggettoCustom, DocsPaWR.Templates template, List<AssDocFascRuoli> dirittiCampiRuolo)
        {
            foreach (DocsPaWR.AssDocFascRuoli assDocFascRuoli in dirittiCampiRuolo)
            {
                if (assDocFascRuoli.ID_OGGETTO_CUSTOM == oggettoCustom.SYSTEM_ID.ToString())
                {
                    switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((CustomTextArea)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "CasellaDiSelezione":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.CheckBoxList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "MenuATendina":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((System.Web.UI.WebControls.DropDownList)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "SelezioneEsclusiva":
                            //Per la selezione esclusiva è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Contatore":
                            //Per il contatore è stato implementato un metodo a parte perchè gli oggetti in uso sono più di due
                            break;
                        case "Data":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.Calendar)campo).Visible = false;
                                ((UserControls.Calendar)campo).VisibleTimeMode = UserControls.Calendar.VisibleTimeModeEnum.Nothing;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Corrispondente":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((UserControls.CorrespondentCustom)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "Link":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.LinkDocFasc)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                        case "OggettoEsterno":
                            if (assDocFascRuoli != null && assDocFascRuoli.VIS_OGG_CUSTOM == "0")
                            {
                                ((System.Web.UI.WebControls.Label)etichetta).Visible = false;
                                ((UserControls.IntegrationAdapter)campo).Visible = false;
                                oggettoCustom.CAMPO_OBBLIGATORIO = "NO";
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// In presenza di un solo corrispondente in griglia,
        /// lo seleziona per default
        /// </summary>
        private void SelectDefaultCorrispondente()
        {
            if (this.grdCorr.Rows.Count == 1)
            {
                GridViewRow dgItem = this.grdCorr.Rows[0];

                RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
                if (optCorr != null)
                    optCorr.Checked = true;

                this.grdCorr.SelectedIndex = 0;
                this.SelectedCorrIndex = 0;
                this.grid_corrindex.Value = "0";
            }
            else
            {
                this.grdCorr.SelectedIndex = -1;
                this.SelectedCorrIndex = -1;
                this.grid_corrindex.Value = "-1";
            }
        }

        private void AppendDataRow(DataTable dt, string tipoCorr, string systemId, string descCorr)
        {
            DataRow row = dt.NewRow();
            row["SYSTEM_ID"] = systemId;
            row["TIPO_CORR"] = tipoCorr;
            row["DESC_CORR"] = descCorr;
            dt.Rows.Add(row);
            row = null;
        }

        private string GetImage(string rowType)
        {
            string retValue = string.Empty;

            switch (rowType)
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

            retValue = " <img src=\"../Images/Icons/" + retValue + ".png\" border=\"\" alt=\"\" />";

            return retValue;
        }

        public string GetLabel(string id)
        {
            string language = UIManager.UserManager.GetUserLanguage();
            return Utils.Languages.GetLabelFromCode(id, language);
        }

    }


    public class ProtocolloDataGridItem
    {
        private string data;
        private string oggetto;
        private string mittDest;
        private string registro;
        private int chiave;

        public ProtocolloDataGridItem(string data, string oggetto, string mittDest, string registro, int chiave)
        {
            this.data = data;
            this.oggetto = oggetto;
            this.registro = registro;
            this.mittDest = mittDest;
            this.chiave = chiave;
        }

        public string Data { get { return data; } }
        public string Oggetto { get { return oggetto; } }
        public string Registro { get { return registro; } }
        public string MittDest { get { return mittDest; } }
        public int Chiave { get { return chiave; } }
    }

    /// <summary>
    /// Classe per la gestione dei dati in sessione relativamente
    /// alla dialog "RicercaProtocolliUscita"
    /// </summary>
    public sealed class RicercaDocumentiSessionMng
    {
        private RicercaDocumentiSessionMng()
        {
        }

        public static void SetListaInfoDocumenti(Page page, DocsPaWR.InfoDocumento[] listaDocumenti)
        {
            page.Session["RicercaProtocolliUscita.ListaInfoDoc"] = listaDocumenti;
        }

        public static DocsPaWR.InfoDocumento[] GetListaInfoDocumenti(Page page)
        {
            return page.Session["RicercaProtocolliUscita.ListaInfoDoc"] as DocsPaWR.InfoDocumento[];
        }

        public static void RemoveListaInfoDocumenti(Page page)
        {
            page.Session.Remove("RicercaProtocolliUscita.ListaInfoDoc");
        }

        /// <summary>
        /// Impostazione flag booleano, se true, la dialog è stata caricata almeno una volta
        /// </summary>
        /// <param name="page"></param>
        public static void SetAsLoaded(Page page)
        {
            page.Session["RicercaDocumentiSessionMng.isLoaded"] = true;
        }

        /// <summary>
        /// Impostazione flag relativo al caricamento della dialog
        /// </summary>
        /// <param name="page"></param>
        public static void SetAsNotLoaded(Page page)
        {
            page.Session.Remove("RicercaDocumentiSessionMng.isLoaded");
        }

        /// <summary>
        /// Verifica se la dialog è stata caricata almeno una volta
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static bool IsLoaded(Page page)
        {
            return (page.Session["RicercaDocumentiSessionMng.isLoaded"] != null);
        }

        /// <summary>
        /// Impostazione valore di ritorno
        /// </summary>
        /// <param name="page"></param>
        /// <param name="dialogReturnValue"></param>
        public static void SetDialogReturnValue(Page page, bool dialogReturnValue)
        {
            page.Session["RicercaDocumentiSessionMng.dialogReturnValue"] = dialogReturnValue;
        }

        /// <summary>
        /// Reperimento valore di ritorno
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static bool GetDialogReturnValue(Page page)
        {
            bool retValue = false;

            if (IsLoaded(page))
                retValue = Convert.ToBoolean(page.Session["RicercaDocumentiSessionMng.dialogReturnValue"]);

            page.Session.Remove("RicercaDocumentiSessionMng.isLoaded");

            return retValue;
        }

        /// <summary>
        /// Metodo per il settaggio in sessione del corrispondente selezionato per il protocollo di risposta
        /// </summary>
        /// <param name="page"></param>
        /// <param name="corrispondente"></param>
        public static void setCorrispondenteRispostaUSCITA(Page page, DocsPaWR.Corrispondente corrispondente)
        {
            page.Session["RicercaProtocolliUscita.corrispondenteRisposta"] = corrispondente;
        }

        public static DocsPaWR.Corrispondente getCorrispondenteRispostaUSCITA(Page page)
        {
            return (DocsPaWR.Corrispondente)page.Session["RicercaProtocolliUscita.corrispondenteRisposta"];
        }

        public static void removeCorrispondenteRispostaUSCITA(Page page)
        {
            page.Session.Remove("RicercaProtocolliUscita.corrispondenteRisposta");
        }


        public static void setCorrispondenteRispostaINGRESSO(Page page, DocsPaWR.Corrispondente[] corrispondente)
        {
            page.Session["RicercaProtocolliIngresso.corrispondenteRisposta"] = corrispondente;
        }

        public static DocsPaWR.Corrispondente[] getCorrispondenteRispostaINGRESSO(Page page)
        {
            return (DocsPaWR.Corrispondente[])page.Session["RicercaProtocolliIngresso.corrispondenteRisposta"];
        }

        public static void removeCorrispondenteRispostaINGRESSO(Page page)
        {
            page.Session.Remove("RicercaProtocolliIngresso.corrispondenteRisposta");
        }
    }
        
}