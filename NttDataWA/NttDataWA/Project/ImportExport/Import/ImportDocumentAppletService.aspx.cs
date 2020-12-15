using Newtonsoft.Json;
using NttDataWA.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Project.ImportExport.Import
{
    public partial class ImportDocumentAppletService : System.Web.UI.Page
    {
        private string absolutePath = string.Empty;
        private string codFasc = string.Empty;
        private string foldName = string.Empty;
        private NttDataWA.DocsPaWR.Fascicolo fasc = null;
        private NttDataWA.DocsPaWR.FileDocumento fd = null;
        private string type = string.Empty;

        private bool IsSocket
        {
            get
            {
                return (!String.IsNullOrEmpty(Request.QueryString["issocket"]));
            }
        }

        DocsPaWR.DocsPaWebService DocsPaWS = new NttDataWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            int statusCode = 0;
			NttDataWA.Utils.FileJSON file = null;
            string componentType = UIManager.UserManager.getComponentType(Request.UserAgent);
            if ((Request.QueryString["Absolutepath"] != null) && (Request.QueryString["Absolutepath"] != ""))
            {
                //1. controllo se arriva la path assoluta
                absolutePath = decodeQueryString(Request.QueryString["Absolutepath"].ToString());
                //1.0.1 Controllo della dimensione del file
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[NttDataWA.Utils.WebConfigKeys.FILE_ACQ_SIZE_MAX.ToString()]) &&
                                               (Int32.Parse(System.Configuration.ConfigurationManager.AppSettings[NttDataWA.Utils.WebConfigKeys.FILE_ACQ_SIZE_MAX.ToString()].ToString()) * 1024) < Request.ContentLength
                                               )
                {
                    parseStatusCode(5);
                    if (!String.IsNullOrEmpty(componentType) && componentType.Equals(Constans.TYPE_SOCKET))
                    {
                        ImportDocManager.setSessionMapImportStatus(absolutePath, this.Response.StatusDescription);
                    }
                    else
                    {
                        HttpContext.Current.Session["ImportStatus"] = this.Response.StatusDescription;
                    }
                }
                else
                {
                    fd = new NttDataWA.DocsPaWR.FileDocumento();
                    //1.1 Leggo il content del file 
                    if (!IsSocket)
                    {
                        byte[] ba = Request.BinaryRead(Request.ContentLength);
                        fd.content = ba;
                    }
                    else
                    {
                        //Stream stream=Request.InputStream;
                        string contentFile = Request["contentFile"];
                        if (!String.IsNullOrEmpty(contentFile))
                        {
                            contentFile = contentFile.Replace(' ', '+');
                            contentFile = contentFile.Trim();
                            file = JsonConvert.DeserializeObject<NttDataWA.Utils.FileJSON>(contentFile);
                        }
                        if (file != null && !String.IsNullOrEmpty(file.content))
                            fd.content = Convert.FromBase64String(file.content);
                    }

                    //1.2 leggo il codice fascicolo
                    if (Request.QueryString["codFasc"] != null && Request.QueryString["codFasc"] != "")
                    {
                        codFasc = decodeQueryString(Request.QueryString["codFasc"].ToString());
                        //fasc = FascicoliManager.getFascicoloDaCodice(this.Page, codFasc);
                        fasc = NttDataWA.UIManager.ProjectManager.getFascicoloDaCodice(this.Page, codFasc, Request.QueryString["idTitolario"]);
                    }

                    //1.4. tipo passato
                    if (Request.QueryString["type"] != null && Request.QueryString["type"] != "")
                        type = Request.QueryString["type"].ToString();

                    if (Request.QueryString["foldName"] != null && Request.QueryString["foldName"] != "")
                        foldName = decodeQueryString(Request.QueryString["foldName"].ToString());

                    //1.5. se tutte le condizioni sono ok... procedo
                    if ((codFasc != "") && (absolutePath != ""))
                    {

                        /* prendo la cartella selezionata per l'acquisizione massiva */
                        DocsPaWR.Folder folder = null;
                        if (UIManager.ProjectManager.getProjectInSession().folderSelezionato != null)
                        {
                            folder = UIManager.ProjectManager.getProjectInSession().folderSelezionato;
                        }
                        
                        statusCode = ImportDocManager.checkORCreateDocFolderFasc(this.Page, fasc, absolutePath, fd, foldName, type, folder, componentType);

                        // parsing degli stati
                        parseStatusCode(statusCode);
                        if (!String.IsNullOrEmpty(componentType) && componentType.Equals(Constans.TYPE_SOCKET)) 
                        {
                            ImportDocManager.setSessionMapImportStatus(absolutePath, this.Response.StatusDescription);
                        }
                        else
                        {
                            HttpContext.Current.Session["ImportStatus"] = this.Response.StatusDescription;
                        }
                    }
                }

            }
        }

        private void parseStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 1:
                    this.Response.StatusCode = 1;
                    this.Response.StatusDescription = "File Acquisito";
                    break;
                case 2:
                    this.Response.StatusCode = 2;
                    this.Response.StatusDescription = "Errore: Formato File non supportato";
                    break;
                case 21:
                    this.Response.StatusCode = 2;
                    this.Response.StatusDescription = "Errore: Formato File Firmato non supportato";
                    break;
                case 3:
                    this.Response.StatusCode = 3;
                    this.Response.StatusDescription = "Cartella Acquisita";
                    break;
                case 4:
                    this.Response.StatusCode = 4;
                    this.Response.StatusDescription = "Cartella Esistente";
                    break;
                case 5:
                    this.Response.StatusCode = 5;
                    this.Response.StatusDescription = "Errore: Dimensione file troppo grande";
                    break;
                case 0:
                    this.Response.StatusCode = 0;
                    this.Response.StatusDescription = "Errore in acquisizione";
                    break;
                case 6:
                    this.Response.StatusCode = 6;
                    this.Response.StatusDescription = "Fattura Elettronica non firmata";
                    break;
                case 7:
                    this.Response.StatusCode = 6;
                    this.Response.StatusDescription = "Errore in acquisizione: il nome del file supera il limite massimo consentito.";
                    break;
            }
        }

        private string decodeQueryString(string queryString)
        {
            return System.Web.HttpUtility.UrlDecode(queryString);
        }

    }
}