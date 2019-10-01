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
using System.Linq;

namespace ConservazioneWA.Esibizione
{
    public partial class VisualizzaXMLFromStore : System.Web.UI.Page
    {
        protected WSConservazioneLocale.DocsPaConservazioneWS wss;
        protected string idIstanza;
        protected string pathFile;
        protected string idDocumento;
        protected string ContentType;
        protected string type;
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
                type = Request.QueryString["type"];

                
                wss = new ProxyManager().getProxy();

                // Visualizzazione xml per documento
                if (type == "D")
                {
                    //
                    // Recupero il file di chiusura, in cui sono contenute tutte le informazioni
                    Dictionary<String, String> documentiMemorizzati = null;
                    documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUt, idIstanza, localStore);

                    string info = documentiMemorizzati[idDocumento];

                    if (string.IsNullOrEmpty(info))
                    {
                        //
                    }
                    else
                    {
                        // Prendo il file dallo storage
                        //string formato = info.Split('§')[0];
                        string formato = "text/plain";
                        string idDocument = info.Split('§')[1];
                        string path = info.Split('§')[2];
                        //string hashSupporto = info.Split('§')[3];

                        pathFile = path + ".xml";

                        byte[] contentFile = wss.getFileFromStore(infoUt, idIstanza, pathFile, localStore);

                        if (contentFile != null)
                        {
                            Response.ContentType = formato;
                            Response.AddHeader("Content-Disposition", "inline");
                            Response.BinaryWrite(contentFile);
                        }
                    }
                }

                // Visualizzazione xml per fascicolo
                if (type == "F") 
                {
                    // Get Info istanze conservazione
                    WSConservazioneLocale.ItemsConservazione[] itemsCons = ConservazioneManager.getItemsConservazione(idIstanza, infoUt);
                    
                    string formato = "text/plain";
                    string idFascicolo = idDocumento;
                    
                    string CodiceFascicolo = string.Empty;
                    if (itemsCons.Length > 0)
                        CodiceFascicolo =itemsCons.FirstOrDefault(x => x.ID_Project.ToString() == idFascicolo).CodFasc.ToString();

                    string path = "\\Fascicoli" + "\\" + CodiceFascicolo + "\\" + idFascicolo;
                    //string hashSupporto = info.Split('§')[3];

                    pathFile = path + ".xml";

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