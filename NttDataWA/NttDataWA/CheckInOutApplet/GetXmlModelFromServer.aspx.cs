using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOutApplet
{
    public partial class GetXmlModelFromServer : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.DownloadFile();
        }

        /// <summary>
        /// Download del file richiesto
        /// </summary>
        protected virtual void DownloadFile()
        {
            DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

            ModelRequest modelReq = new DocsPaWR.ModelRequest();
            modelReq.documentId = Request.QueryString["documentId"];
            modelReq.modelType = Request.QueryString["modelType"];
            modelReq.userInfo = new InfoUtente(); // UserManager.GetInfoUser();
            //modelReq.userInfo.extApplications = null;
            
            modelReq.userInfo.userId = UserManager.GetInfoUser().userId;
            modelReq.userInfo.dst = UserManager.GetInfoUser().dst;
            modelReq.userInfo.idAmministrazione = UserManager.GetInfoUser().idAmministrazione;
            modelReq.userInfo.idCorrGlobali = UserManager.GetInfoUser().idCorrGlobali;
            modelReq.userInfo.idGruppo = UserManager.GetInfoUser().idGruppo;
            modelReq.userInfo.idPeople = UserManager.GetInfoUser().idPeople;
            modelReq.userInfo.urlWA = UserManager.GetInfoUser().urlWA;

            modelReq.userInfo.urlWA = UserManager.GetInfoUser().urlWA;

            if (!string.IsNullOrEmpty(WebServiceUrl))
                ws.Url = WebServiceUrl;

            string modelres = ws.GetDocumentModelAsXml(modelReq);


            Response.Write(modelres);
        }

        protected string WebServiceUrl
        {
            get
            {
                string webServiceUrl = Utils.InitConfigurationKeys.GetValue("0", "BE_WEBSERVICE_URL");

                if (string.IsNullOrEmpty(webServiceUrl))
                    webServiceUrl = NttDataWA.Properties.Settings.Default.NttDataWA_DocsPaWR_DocsPaWebService;

                return webServiceUrl;
            }
        }
    }
}