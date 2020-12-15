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
using ConservazioneWA.Utils;
using Debugger = ConservazioneWA.Utils.Debugger;


namespace ConservazioneWA
{
    public partial class ShowDocument : System.Web.UI.Page
    {
        protected WSConservazioneLocale.DocsPaConservazioneWS wss;
        protected string idIstanza;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            try
            {
                idIstanza = Request.QueryString["idIstanza"];
                wss = new ProxyManager().getProxy();
                byte[] fileToSign = wss.downloadSignedXml(idIstanza, ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]));

                //va aggiunta la chiamata al servizio con MTOM o in alternativa con DIME
                //System.IO.File fileFirmato = new System.IO.FileStream("timbro_orizzontale.JPG", System.IO.FileAccess.ReadWrite);

                if (fileToSign != null)
                    Response.BinaryWrite(fileToSign);
            }
            catch (Exception ex)
            {
                Debugger.Write("Errore nel download dal servizio del file xml da visualizzare per la firma: " + ex.Message);
            }
        }
    }
}
