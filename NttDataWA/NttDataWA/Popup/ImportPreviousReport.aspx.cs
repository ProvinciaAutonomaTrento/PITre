using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.Popup
{
    public partial class ImportPreviousReport : System.Web.UI.Page
    {

        #region Fields

        public static string componentType = Constans.TYPE_SMARTCLIENT;

        #endregion

        #region Properties

        protected Dictionary<string, DocsPaWR.Registro> Registri
        {
            get
            {
                return HttpContext.Current.Session["registri"] as Dictionary<string, DocsPaWR.Registro>;
            }
            set
            {
                HttpContext.Current.Session["registri"] = value;
            }
        }

        protected Dictionary<string, DocsPaWR.Utente> Utenti
        {
            get
            {
                return HttpContext.Current.Session["utenti"] as Dictionary<string, DocsPaWR.Utente>;
            }
            set
            {
                HttpContext.Current.Session["utenti"] = value;
            }
        }

        protected Dictionary<string, DocsPaWR.Ruolo> Ruoli
        {
            get
            {
                return HttpContext.Current.Session["ruoli"] as Dictionary<string, DocsPaWR.Ruolo>;
            }
            set
            {
                HttpContext.Current.Session["ruoli"] = value;
            }
        }

        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                return HttpContext.Current.Session["infoUtente"] as DocsPaWR.InfoUtente;
            }
            set
            {
                HttpContext.Current.Session["infoUtente"] = value;
            }
        }

        protected DocsPaWR.ReportPregressi ReportSelezionato
        {
            get
            {
                return HttpContext.Current.Session["reportSelezionato"] as DocsPaWR.ReportPregressi;
            }
            set
            {
                HttpContext.Current.Session["reportSelezionato"] = value;
            }
        }

        public DocsPaWR.FileDocumento FileDocumento
        {
            get
            {
                return HttpContext.Current.Session["fileDocumento"] as DocsPaWR.FileDocumento;
            }
            set
            {
                HttpContext.Current.Session["fileDocumento"] = value;
            }
        }

        public static string httpFullPath
        {
            get
            {
                return utils.getHttpFullPath();
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            componentType = UserManager.getComponentType(Request.UserAgent);
            this.InitApplet();

            if (!IsPostBack)
            {
                this.InitializePage();
                this.FetchData();
            }
        }

        protected void BtnClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reallowOp", "reallowOp();", true);
            this.CloseMask(true);
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            this.Export();
        }

        #endregion

        #region Methods

       
        private void InitApplet()
        {
            if (componentType == Constans.TYPE_APPLET)
                this.plcApplet.Visible = true;
            else
            {
                Control ShellWrapper = Page.LoadControl("../ActivexWrappers/ShellWrapper.ascx");
                this.plcActiveX.Controls.Add(ShellWrapper);

                Control AdoStreamWrapper = Page.LoadControl("../ActivexWrappers/AdoStreamWrapper.ascx");
                this.plcActiveX.Controls.Add(AdoStreamWrapper);

                Control FsoWrapper = Page.LoadControl("../ActivexWrappers/FsoWrapper.ascx");
                this.plcActiveX.Controls.Add(FsoWrapper);

                this.plcActiveX.Visible = true;
            }
        }

        protected void InitializePage()
        {
            this.InitializeLanguage();
            this.SetReportPregresso();
            this.PopolaInfoUtente();
            this.Registri = new Dictionary<string, DocsPaWR.Registro>();
            this.Utenti = new Dictionary<string, DocsPaWR.Utente>();
            this.Ruoli = new Dictionary<string, DocsPaWR.Ruolo>();
        }

        private void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.BtnClose.Text = Utils.Languages.GetLabelFromCode("GenericBtnClose", language);
            this.BtnReport.Text = Utils.Languages.GetLabelFromCode("ImportPreviousReportBtnReport", language);
            this.grvItems.Columns[1].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportDate", language);
            this.grvItems.Columns[2].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportOutcome", language);
            this.grvItems.Columns[3].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportError", language);
            this.grvItems.Columns[4].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportIdDoc", language);
            this.grvItems.Columns[5].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportIdOld", language);
            this.grvItems.Columns[6].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportRegistry", language);
            this.grvItems.Columns[7].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportOwner", language);
            this.grvItems.Columns[8].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportTypeOperation", language);
            this.grvItems.Columns[9].HeaderText = Utils.Languages.GetLabelFromCode("ImportPreviousReportNumAttachments", language);
        }

        private void CloseMask(bool withReturnValue)
        {
            string retValue = withReturnValue ? "true" : "false";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeAjaxModal", "parent.closeAjaxModal('ImportPreviousReport', '" + retValue + "');", true);
        }

        protected void FetchData()
        {
            this.grvItems.DataSource = this.ReportSelezionato.itemPregressi;
            this.grvItems.PageIndex = 0;
            this.grvItems.DataBind();
        }

        protected void SetReportPregresso()
        {
            if (Session["ImportPreviousReport_ID"] != null)
            {
                string idReport = Session["ImportPreviousReport_ID"].ToString();
                this.ReportSelezionato = ImportPreviousManager.GetReportPregressi(idReport);
            }
        }

        protected void PopolaInfoUtente()
        {
            this.InfoUtente = UserManager.GetInfoUser();
        }

        protected string GetItemID(DocsPaWR.ItemReportPregressi report)
        {
            return report.systemId;
        }

        protected string GetData(DocsPaWR.ItemReportPregressi report)
        {
            return report.data;
        }

        protected string GetEsito(DocsPaWR.ItemReportPregressi report)
        {
            return report.esito;
        }

        protected string GetErrore(DocsPaWR.ItemReportPregressi report)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(report.errore))
            {
                string[] errori = report.errore.Split('|');
                result = "<ul>";
                foreach (string err in errori)
                {
                    result += "<li>" + err + "</li>";
                }
                result += "</ul>";
            }
            return result;
        }

        protected string GetDocumento(DocsPaWR.ItemReportPregressi report)
        {
            return report.idDocumento;
        }

        protected string GetNumProtoExcel(DocsPaWR.ItemReportPregressi report)
        {
            return report.idNumProtocolloExcel;
        }

        protected string GetRegistro(DocsPaWR.ItemReportPregressi report)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(report.idRegistro))
            {
                if (this.Registri == null)
                {
                    this.Registri = new Dictionary<string, DocsPaWR.Registro>();
                }
                if (this.Registri.ContainsKey(report.idRegistro))
                {
                    result = this.Registri[report.idRegistro].codRegistro;
                }
                else
                {
                    DocsPaWR.Registro reg = RegistryManager.getRegistroBySistemId(report.idRegistro);
                    this.Registri.Add(report.idRegistro, reg);
                    result = reg.codRegistro;
                }
            }
            return result;
        }

        protected string GetUtenteRuolo(DocsPaWR.ItemReportPregressi report)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(report.idUtente))
            {
                DocsPaWR.Utente utente = null;
                if (this.Utenti == null)
                {
                    this.Utenti = new Dictionary<string, DocsPaWR.Utente>();
                }

                if (Utenti.ContainsKey(report.idUtente))
                {
                    utente = Utenti[report.idUtente];
                }
                else
                {
                    utente = UserManager.GetUtenteByIdPeople(report.idUtente);
                    Utenti.Add(report.idUtente, utente);
                }

                if (utente != null)
                {
                    result = utente.nome + " " + utente.cognome;
                }
            }

            if (!string.IsNullOrEmpty(report.idRuolo))
            {
                if (this.Ruoli == null)
                {
                    this.Ruoli = new Dictionary<string, DocsPaWR.Ruolo>();
                }
                DocsPaWR.Ruolo ruolo = null;

                if (Ruoli.ContainsKey(report.idRuolo))
                {
                    ruolo = Ruoli[report.idRuolo];
                }
                else
                {
                    ruolo = RoleManager.getRuoloByIdGruppo(report.idRuolo);
                    Ruoli.Add(report.idRuolo, ruolo);
                }

                if (ruolo != null)
                {
                    result = result + " (" + ruolo.descrizione + ")";
                }
            }

            return result;
        }

        protected string GetTipoOperazione(DocsPaWR.ItemReportPregressi report)
        {
            return report.tipoOperazione;
        }

        protected string GetNumeroAllegati(DocsPaWR.ItemReportPregressi report)
        {
            string result = "0";
            if (report.Allegati != null)
            {
                result = report.Allegati.Length.ToString();
            }
            return result;
        }

        protected void Export()
        {
            DocsPaWR.FileDocumento file = new DocsPaWR.FileDocumento();
            string componentCall = string.Empty;
            try
            {
                file = this.GetFile();
                if (file == null || file.content == null || file.content.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ImportPreviousReportNoDocuments', 'error', '');", true);
                }
                else
                    
                {
                    switch(componentType){
                        case(Constans.TYPE_APPLET):
                           componentCall = "OpenFileApplet('XLS');";
                            break;
                        case (Constans.TYPE_SOCKET):
                            componentCall = "OpenFileSocket('XLS');";
                            break;
                        default:
                            componentCall = "OpenFileActiveX('XLS');";
                            break;
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openFile", componentCall, true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ajaxDialogModal", "ajaxDialogModal('ErrorCustom', 'error', '', '" + utils.FormatJs(ex.Message) + "');", true);
            }
        }

        public DocsPaWR.FileDocumento GetFile()
        {
            DocsPaWR.InfoUtente infoUtente = new DocsPaWR.InfoUtente();
            this.FileDocumento = ImportPreviousManager.ExportPregressiExcel(this.ReportSelezionato, infoUtente);
            return this.FileDocumento;
        }

        #endregion
    }
}