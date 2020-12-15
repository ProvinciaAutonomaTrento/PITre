using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DocsPAWA.ActivexWrappers
{
    public partial class CacheWrapper : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {}

        protected string docNumber
        {
            get
            {
                string number = string.Empty;
                DocsPAWA.DocsPaWR.FileRequest filereq =  DocsPAWA.FileManager.getSelectedFile();
                if(filereq != null && !string.IsNullOrEmpty(filereq.docNumber))
                            number = filereq.docNumber;
                return number;
            }
        }

        protected string pathRepository
        {
            get
            {
                DocsPAWA.DocsPaWR.CacheConfig config = caricaConfig();
                string newPath = string.Empty;
                if (config != null
                    && !string.IsNullOrEmpty(config.doc_root_server_locale))
                {
                    string[] pathSplit = config.doc_root_server_locale.Split('\\');
                    for (int i = 0; i < pathSplit.Length - 1; i++)
                        newPath += pathSplit[i] + "/";
                    newPath += pathSplit[pathSplit.Length - 1];
                }
                return newPath;
            }
        }

        protected string versionId
        {
            get
            {
                string version = string.Empty;
                DocsPAWA.DocsPaWR.FileRequest filereq = DocsPAWA.FileManager.getSelectedFile();
                if (filereq != null && !string.IsNullOrEmpty(filereq.docNumber))
                    version = filereq.versionId;
                return version;
            }
        }

 
        protected string webServiceUrl
        {
            get
            {
                string url_ws = string.Empty;
                DocsPAWA.DocsPaWR.CacheConfig config = caricaConfig();
                if(config != null
                    && !string.IsNullOrEmpty(config.url_ws_caching_locale))
                    url_ws = config.url_ws_caching_locale;

                return url_ws;
            }
        }

        protected string statoFileTrasferito
        {
            get
            {
                return "1";
            }
        }

        protected string statoFileNonTraferito
        {
            get
            {
                return "0";
            }
        }


        protected string localita
        {
            get
            {
               string idAmm = UserManager.getInfoUtente().idAmministrazione;

               if (!string.IsNullOrEmpty(idAmm))
                   return idAmm;
               else
                   return string.Empty;


            }
        }

        /// <summary>
        /// funzionr che carica la configurazioen del caching
        /// </summary>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.CacheConfig caricaConfig()
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            DocsPAWA.DocsPaWR.CacheConfig configurazione = null;
            string idAmm = UserManager.getInfoUtente().idAmministrazione;
            if(!string.IsNullOrEmpty(idAmm))
                 configurazione = ws.getConfigurazioneCache(idAmm);

            return configurazione;
        }

        /// <summary>
        /// verifica se è attivo il caching 
        /// </summary>
        protected string isActiveCache
        {
            get
            {
                DocsPAWA.DocsPaWR.CacheConfig config = caricaConfig();
                if (config != null
                    && config.caching)
                    return "TRUE";
                return "FALSE";
            }
        }
    }
}