using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Project.ImportExport
{
    public partial class GetXMLProject : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string idProject = Request.QueryString["ID"].ToString();
            
            string requestAsXml = "<ExportFascicoloRequest>";

            InfoUtente userInfo = NttDataWA.UIManager.UserManager.GetInfoUser();
            DocsPaWebService webServices = new DocsPaWebService();

            if (!string.IsNullOrEmpty(idProject)) {
                requestAsXml += "<fascicolo id=\"" + idProject + "\"/>";
                requestAsXml += "<userInfo userId=\"" + userInfo.userId + "\" idCorrGlobali=\"" + userInfo.idCorrGlobali + "\" idPeople=\"" + userInfo.idPeople + "\" idGruppo=\"" + userInfo.idGruppo + "\" idAmministrazione=\"" + userInfo.idAmministrazione + "\" sede=\"" + userInfo.sede + "\" urlWA=\"" + userInfo.urlWA + "\" dst=\"" + userInfo.dst + "\"/>";
            }
            
            requestAsXml += "</ExportFascicoloRequest>";

            String xmlInfoProject = webServices.GetInfoFascicoloAsXml(requestAsXml);
            
            Response.Write(xmlInfoProject);
        }
    }
}