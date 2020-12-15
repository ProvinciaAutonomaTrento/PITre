using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;
using System.Linq;

namespace DocsPAWA.FormatiDocumento
{
    /// <summary>
    /// Oggetto che permette di gestire e verificare i formati di file ammessi dall'amministrazione
    /// </summary>
    public partial class SupportedFilesController : System.Web.UI.UserControl
    {
        private DocsPaWebService _wsInstance = null;

        /// <summary>
        /// Chiave di sessione relativa alla lista dei file supportati
        /// </summary>
        private const string SESSION_KEY = "SupportedFilesController.FileTypeList";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool IsSignEnabledPerExtension(string extension)
        {
            if (this.SupportedFileTypesEnabled)
            {
                int count = this.GetSupportedFileTypes().Count(e => e.FileExtension.ToLowerInvariant() == extension.ToLowerInvariant() &&
                                                        e.FileTypeUsed && e.FileTypeSignature);

                return (count > 0);
            }
            else
                return true;
        }

        /// <summary>
        /// Reperimento instanza webservice
        /// </summary>
        private DocsPaWebService WSInstance
        {
            get
            {
                if (this._wsInstance == null)
                    this._wsInstance = new DocsPaWebService();
                return this._wsInstance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // Caricamento dati formati
                this.Fetch();
            }
        }

        /// <summary>
        /// Formati di file ammessi (separati dal carattere '|')
        /// </summary>
        protected string SupportedFileFormats
        {
            get
            {
                if (this.ViewState["SupportedFileFormats"] != null)
                    return this.ViewState["SupportedFileFormats"].ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Dimensioni massime per ogni formato di file ammesso (separati dal carattere '|')
        /// </summary>
        protected string SupportedFileFormatsMaxSize
        {
            get
            {
                if (this.ViewState["SupportedFileFormatsMaxSize"] != null)
                    return this.ViewState["SupportedFileFormatsMaxSize"].ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Modalità di avviso nel caso di dimensione file
        /// superiore a quella massima supportata dal formato (separati dal carattere '|')
        /// </summary>
        protected string MaxFileSizeAlertMode
        {
            get
            {
                if (this.ViewState["MaxFileSizeAlertMode"] != null)
                    return this.ViewState["MaxFileSizeAlertMode"].ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int FileAcquireSizeMax
        {
            get
            {
                int fileAcquireSizeMax = 0;
                if (this.ViewState["FileAcquireSizeMax"] != null)
                    Int32.TryParse(this.ViewState["FileAcquireSizeMax"].ToString(), out fileAcquireSizeMax);
                return fileAcquireSizeMax;
            }
        }

        /// <summary>
        /// Tipi documento cui viene applicato il formato file (separati dal carattere '|')
        /// </summary>
        protected string DocumentType
        {
            get
            {
                if (this.ViewState["DocumentType"] != null)
                    return this.ViewState["DocumentType"].ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Verifica se risulta abilitata la gestione dei tipi file supportati
        /// </summary>
        protected bool SupportedFileTypesEnabled
        {
            get
            {
                return Configurations.SupportedFileTypesEnabled;
            }
        }

        /// <summary>
        /// Reperimento metadati sui tipi di file supportati dall'amministrazione
        /// </summary>
        /// <returns></returns>
        private SupportedFileType[] GetSupportedFileTypes()
        {
            if (Session[SESSION_KEY] == null)
                Session.Add(SESSION_KEY, this.WSInstance.GetSupportedFileTypes(Convert.ToInt32(UserManager.getInfoUtente().idAmministrazione)));
            return Session[SESSION_KEY] as SupportedFileType[];
        }

        /// <summary>
        /// Reperimento tipologia del documento corrente
        /// </summary>
        protected string TipoDocumento
        {
            get
            {
                SchedaDocumento documentoSelezionato = DocumentManager.getDocumentoSelezionato();
                if (documentoSelezionato != null && documentoSelezionato.tipoProto == "G")
                    return "G";
                else if (documentoSelezionato != null)
                    return "P";
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Caricamento dati relativi ai formati
        /// di file ammessi in amministrazione e 
        /// alla dimensione massima
        /// </summary>
        private void Fetch()
        {
            // Configurazione relativa alla gestione precedente
            // sulla dimensione massima dei file
            this.ViewState["FileAcquireSizeMax"] = FormatiDocumento.Configurations.FileAcquireSizeMax.ToString();

            // Se la gestione tipi file è abilitata,
            // vengono reperiti i dati sui formati ammessi
            if (this.SupportedFileTypesEnabled)
            {
                SupportedFileType[] fileTypes = this.GetSupportedFileTypes();

                List<string> list = new List<string>();

                string fileFormats = string.Empty;
                string fileFormatsMaxSize = string.Empty;
                string maxFileSizeAlertMode = string.Empty;
                string documentType = string.Empty;

                foreach (SupportedFileType fileType in fileTypes)
                {
                    if (fileFormats != string.Empty)
                        fileFormats += "|";
                    fileFormats += fileType.FileExtension + ":" + fileType.FileTypeUsed.ToString();

                    if (fileFormatsMaxSize != string.Empty)
                        fileFormatsMaxSize += "|";
                    fileFormatsMaxSize += fileType.MaxFileSize;

                    if (maxFileSizeAlertMode != string.Empty)
                        maxFileSizeAlertMode += "|";
                    maxFileSizeAlertMode += fileType.MaxFileSizeAlertMode.ToString();

                    if (documentType != string.Empty)
                        documentType += "|";
                    documentType += fileType.DocumentType.ToString();
                }

                this.ViewState["SupportedFileFormats"] = fileFormats;
                this.ViewState["SupportedFileFormatsMaxSize"] = fileFormatsMaxSize;
                this.ViewState["MaxFileSizeAlertMode"] = maxFileSizeAlertMode;
                this.ViewState["DocumentType"] = documentType;
            }
        }


        //modifica
        protected bool useCache
        {
            get
            {
                return DocsPAWA.ActivexWrappers.Configurations.CacheControl;
            }
        }

        protected bool isCache
        {
            get
            {
                DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

                string idAmm = UserManager.getInfoUtente().idAmministrazione;
                if (!string.IsNullOrEmpty(idAmm))
                {
                    DocsPAWA.DocsPaWR.CacheConfig config = ws.getConfigurazioneCache(idAmm);
                    if (config != null)
                        return config.caching;
                    
                }
                return false;
            }
        }
        //fine modifica

    }
}