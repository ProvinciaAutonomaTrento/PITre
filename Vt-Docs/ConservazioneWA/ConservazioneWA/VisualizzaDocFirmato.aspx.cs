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

namespace ConservazioneWA
{
    public partial class VisualizzaDocFirmato : System.Web.UI.Page
    {
        protected WSConservazioneLocale.DocsPaConservazioneWS wss;
        protected string idIstanza;
        protected string comcomponentType;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            try
            {
                idIstanza = Request.QueryString["idIstanza"];
                comcomponentType = Request.QueryString["applet"];

                wss = new ProxyManager().getProxy();
                
                byte[] fileToSign = wss.downloadSignedXml(idIstanza, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]));

                if (fileToSign != null)
                {
                    if (string.IsNullOrEmpty(comcomponentType))
                    {
                        Response.ContentType = "application/xml";
                        Response.AddHeader("Content-Disposition", "inline");
                        Response.BinaryWrite(fileToSign);
                    }
                    else
                    {
                        string base64String = System.Convert.ToBase64String(fileToSign, 0, fileToSign.Length);
                        Response.Write(base64String);
                    }
                }

            }
            catch(Exception ex)
            {
                Debugger.Write("Errore nel download del file xml da firmare: " + ex.Message);
            }
        }
    }
}
