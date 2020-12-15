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
using System.Text;
using ConservazioneWA.Utils;
using Debugger = ConservazioneWA.Utils.Debugger;
using Newtonsoft.Json;

namespace ConservazioneWA
{
    public partial class SalvaSignDoc : System.Web.UI.Page
    {
        protected string tipofirma;
        protected bool firmabool;
        protected string idIstanza;
        protected WSConservazioneLocale.DocsPaConservazioneWS wss;
        protected string comcomponentType;

        private class FileJSON
        {
            public string name;
            public string fullPath;
            public string extension;
            public string content;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            try
            {
                idIstanza = Request.QueryString["idIstanza"];
                tipofirma = Request.QueryString["tipofirma"];
                comcomponentType = Request.QueryString["applet"];

                if (string.IsNullOrEmpty(tipofirma))
                {
                    tipofirma = string.Empty;
                }
                if (tipofirma.Equals("cosign"))
                {
                    firmabool = true;
                }
                else
                {
                    firmabool = false;
                }

                byte[] ba = null;
                if (string.IsNullOrEmpty(comcomponentType))
                    ba = Request.BinaryRead(Request.ContentLength);
                else 
                {
                    string b64Content = Request["contentFile"];
                    if (!string.IsNullOrEmpty(b64Content))
                    {
                        b64Content = b64Content.Replace(' ', '+');
                        b64Content = b64Content.Trim();

                        FileJSON file = JsonConvert.DeserializeObject<FileJSON>(b64Content);

                        ba = Convert.FromBase64String(file.content);
                    }
                }

                //Invia i dati al servizio
                wss = new ProxyManager().getProxy();
                string result = wss.uploadSignedXml(idIstanza, ba, (WSConservazioneLocale.InfoUtente)Session["infoutCons"], !string.IsNullOrEmpty(comcomponentType));
                Session["resultFirma"] = result;

                //wss.uploadSignedXml(idIstanza, ba, (WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

                //se l'upload sul servizio avviene con successo restituisco il seguente alert di conferma!!!
                string confirmMsg = "Il documento firmato e' stato archiviato con successo!";
                confirmMsg = confirmMsg.Replace("'", "\\'");
                // Visualizzazione messaggio di errore
                this.RegisterClientScript("UploadSignDoc", "alert('" + confirmMsg + "');");
            }
            catch (Exception es)
            {
                Debugger.Write("Errore nell'upload del file firmato: " + es.Message);
            }
        }


        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }
    }
}
