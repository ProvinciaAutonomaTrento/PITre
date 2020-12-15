using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ConservazioneWA.Utils;
using Debugger = ConservazioneWA.Utils.Debugger;
using System.Collections.Generic;

namespace ConservazioneWA.Esibizione
{
    public partial class VisualizzaDocFromStore : System.Web.UI.Page
    {
        protected WSConservazioneLocale.DocsPaConservazioneWS wss;
        protected string idIstanza;
        protected string pathFile;
        protected string idDocumento;
        protected string ContentType;
        WSConservazioneLocale.FileDocumento fileDocumento = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            WSConservazioneLocale.InfoUtente infoUt = (WSConservazioneLocale.InfoUtente)Session["infoutCons"];
            // Per adesso Provo lacale
            bool localStore = true;
            //
            // Get isLocalStore
            localStore = ConservazioneWA.Utils.ConservazioneManager.isLocalStore();

            try
            {
                // Parametri necessari, idIstanza di conservazione, pathFile
                // Parametri passati in input dal chiamante
                idIstanza = Request.QueryString["idIstanza"];
                idDocumento = Request.QueryString["idDoc"];

                wss = new ProxyManager().getProxy();

                //
                // Recupero il file di chiusura, in cui sono contenute tutte le informazioni
                Dictionary<String, String> documentiMemorizzati = null;
                documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUt, idIstanza, localStore);

                string info = documentiMemorizzati[idDocumento];
                
                if (string.IsNullOrEmpty(info))
                {
                    // Non è stato possibile recuperare il file di chiusura
                    // prendo file da locale
                    //
                    // Recupero le info sul file
                    //WSConservazioneLocale.FileDocumento fileDocumento = null;
                    fileDocumento = ConservazioneWA.Utils.ConservazioneManager.GetFileDocumentoNotifica(infoUt, idDocumento, 0);

                    if (fileDocumento != null)
                    {
                        pathFile = fileDocumento.path;
                        ContentType = fileDocumento.contentType;

                        Response.ContentType = ContentType;
                        Response.AddHeader("Content-Disposition", "inline");
                        Response.BinaryWrite(fileDocumento.content);
                    }

                }
                else 
                {
                    // Prendo il file dallo storage
                    string formato = info.Split('§')[0];
                    string idDocument = info.Split('§')[1];
                    string path = info.Split('§')[2];
                    string hashSupporto = info.Split('§')[3];

                    pathFile = path;

                    byte[] contentFile = wss.getFileFromStore(infoUt, idIstanza, pathFile, localStore);

                    if (contentFile != null)
                    {
                        Response.ContentType = formato;
                        Response.AddHeader("Content-Disposition", "inline");
                        Response.BinaryWrite(contentFile);
                    }
                }

            }
            catch (Exception ex)
            {
                Debugger.Write("Errore nel download del file: " + ex.Message);
            }
        }
    }
}