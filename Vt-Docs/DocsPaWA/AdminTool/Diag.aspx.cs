using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool
{
    public partial class Diag : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

     
            DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            DocsPaWR.Diagnostics diag = ws.A1GetDiagnostics();
            String InfoBK = String.Empty;
            String InfoDB = String.Empty;
            String InfoDOC = String.Empty;
            String InfoURL = String.Empty;
            String InfoFE = String.Empty;
            InfoBK += String.Format("Titolo Applicativo <b>{0}</b><br/>", diag.TitoloSGD);
            InfoBK += String.Format("Orario del Backend <b>{0}</b><br/>", diag.BackedTime.ToLocalTime().ToString());
            InfoBK += String.Format("Versione Backend <b>{0}</b><br/>", diag.VersioneWS);
            InfoBK += String.Format("Path Backend <b>{0}</b><br/>", diag.PathWS);
            InfoBK += String.Format("AppPool Backend <b>{0}</b><br/>", diag.AppPoolWS);
            InfoBK += String.Format("Server Backend <b>{0}</b><br/>", diag.WSServerName);

            InfoDB += String.Format("Tipo DB <b>{0}</b><br/>", diag.TipoDB);
            InfoDB += String.Format("Nome Utente DB <b>{0}</b><br/>", diag.NomeUtenteDb);
            InfoDB += String.Format("Nome Database <b>{0}</b><br/>", diag.NomeDb);
            InfoDB += String.Format("IL DB è collegato <b>{0}</b><br/>", diag.DBok);

            InfoDOC += String.Format("Tipo Documentale <b>{0}</b><br/>", diag.TipoDocumentale);
            InfoDOC += String.Format("Info Documentale <b>{0}</b><br/>", diag.InfoDocumentale);

            InfoURL += String.Format("Url Convertitore PDF <b>{0}</b><br/>", diag.UrlConvertitorePDF);
            InfoURL += String.Format("Url Centro Notifiche <b>{0}</b><br/>", diag.UrlCentroNotifiche);
            InfoURL += String.Format("Url Rubrica Comune <b>{0}</b><br/>", diag.UrlRubricaComune);


            InfoFE += String.Format("Orario del Frontend <b>{0}</b><br/>", DateTime.Now.ToLocalTime().ToString());
            InfoFE += String.Format("Path Url al BE <b>{0}</b><br/>", ws.Url);
            InfoFE += String.Format("Path Frontend <b>{0}</b><br/>", Server.MapPath("~"));
            InfoFE += String.Format("AppPool Frontend <b>{0}</b><br/>", Request.ServerVariables["APP_POOL_ID"]);
            InfoFE += String.Format("Server Frontend <b>{0}</b><br/>", Server.MachineName);


            InfoBackend.Text = InfoBK;
            ImpostazioniDB.Text = InfoDB;
            ImpostazioniDoc.Text = InfoDOC;
            ImpostazioniUrl.Text = InfoURL;
            InfoFrontend.Text = InfoFE;
            string adminLink = new Uri ( Request.Url,".").ToString()+"login.htm";
            string LoginLink = new Uri(Request.Url, "..").ToString() + "login.htm";
            LinkAdmin.NavigateUrl = adminLink;
            LinkLogin.NavigateUrl = LoginLink;






        }
    }
}