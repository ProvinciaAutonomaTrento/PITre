using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.SiteNavigation;
using System.Drawing;

namespace DocsPAWA.Import.Documenti
{
    public partial class ImportPregressiDettaglio : CssPage
    {
        private DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.startUp(this);
            Response.Expires = -1;
            if (!IsPostBack)
            {
                SetReportPregresso();
                FetchData();
                PopolaInfoUtente();
                GetTheme();
                this.Registri = new Dictionary<string, DocsPaWR.Registro>();
                this.Utenti = new Dictionary<string, DocsPaWR.Utente>();
                this.Ruoli = new Dictionary<string, DocsPaWR.Ruolo>();
            }
        }

        protected void SetReportPregresso()
        {
            if (Request.QueryString["id"] != null)
            {
                string idReport = Request.QueryString["id"].ToString();
                this.ReportSelezionato = WsInstance.GetReportPregressi(idReport, true);
            }
        }

        protected void FetchData()
        {
            this.grvItems.DataSource = this.ReportSelezionato.itemPregressi;
            this.grvItems.CurrentPageIndex = 0;
            this.grvItems.DataBind();
        }

        protected void BtnEsporta_Click(object sender, EventArgs e)
        {
            this.Export();
        }

        protected void Export()
        {
            DocsPaWR.FileDocumento file = new DocsPAWA.DocsPaWR.FileDocumento();
            try
            {
                file = GetFile();
                if (file == null || file.content == null || file.content.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "noRisultatiRicerca", "alert('Nessun documento trovato.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "openFile", "OpenFile();", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "errore", "alert('Errore di sistema: " + ex.Message.Replace("'", "\\'") + "');", true);
            }
        }

        public DocsPAWA.DocsPaWR.FileDocumento GetFile()
        {
            DocsPaWR.InfoUtente infoUtente = new DocsPaWR.InfoUtente();
            this.FileDocumento = WsInstance.ExportPregressiExcel(this.ReportSelezionato, infoUtente);
            return this.FileDocumento;
        }



        protected void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
        {

        }

        protected void ImageCreatedRender(Object sender, DataGridItemEventArgs e)
        {

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
                    DocsPaWR.Registro reg = WsInstance.GetRegistroBySistemId(report.idRegistro);
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
                    ruolo = UserManager.getRuoloByIdGruppo(report.idRuolo, this.Page);
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

        protected void PopolaInfoUtente()
        {
            this.InfoUtente = UserManager.getInfoUtente(this);
        }

        /// <summary>
        /// Reperimento idamministrazione corrente
        /// </summary>
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        protected DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                if (this._wsInstance == null)
                {
                    this._wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    this._wsInstance.Timeout = System.Threading.Timeout.Infinite;
                }
                return this._wsInstance;
            }
        }

        protected void GetTheme()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(UserManager.getInfoUtente().idAmministrazione);

            if (!string.IsNullOrEmpty(Tema))
            {
                string[] colorsplit = Tema.Split('^');
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);
            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
            }
        }

        /// <summary>
        /// Report Selezionato
        /// </summary>
        protected DocsPaWR.ReportPregressi ReportSelezionato
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["reportSelezionato"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["reportSelezionato"] as DocsPaWR.ReportPregressi;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["reportSelezionato"] = value;
            }
        }

        /// <summary>
        /// File Excel
        /// </summary>
        public DocsPaWR.FileDocumento FileDocumento
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["fileDocumento"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["fileDocumento"] as DocsPaWR.FileDocumento;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["fileDocumento"] = value;
            }
        }

        /// <summary>
        /// InfoUtente
        /// </summary>
        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["infoUtente"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["infoUtente"] as DocsPaWR.InfoUtente;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["infoUtente"] = value;
            }
        }

        /// <summary>
        ///  Registri
        /// </summary>
        protected Dictionary<string, DocsPaWR.Registro> Registri
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["registri"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["registri"] as Dictionary<string, DocsPaWR.Registro>;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["registri"] = value;
            }
        }

        /// <summary>
        ///  Utenti
        /// </summary>
        protected Dictionary<string, DocsPaWR.Utente> Utenti
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["utenti"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["utenti"] as Dictionary<string, DocsPaWR.Utente>;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["utenti"] = value;
            }
        }

        /// <summary>
        ///  Ruoli
        /// </summary>
        protected Dictionary<string, DocsPaWR.Ruolo> Ruoli
        {
            get
            {
                if (CallContextStack.CurrentContext != null && CallContextStack.CurrentContext.ContextState["ruoli"] != null)
                {
                    if (CallContextStack.CurrentContext == null)
                    {
                        CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                    }
                    return CallContextStack.CurrentContext.ContextState["ruoli"] as Dictionary<string, DocsPaWR.Ruolo>;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CallContextStack.CurrentContext == null)
                {
                    CallContextStack.CurrentContext = new SiteNavigation.CallContext("CallContextStack.CurrentContext");
                }
                CallContextStack.CurrentContext.ContextState["ruoli"] = value;
            }
        }

    }
}