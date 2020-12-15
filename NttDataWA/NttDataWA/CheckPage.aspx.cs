using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

namespace NttDataWA
{
    public partial class CheckPage : System.Web.UI.Page
    {
        private ILog logger = LogManager.GetLogger(typeof(CheckPage));
        //protected System.Web.UI.WebControls.Label lbl_msg;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            bool rtn = false;
            try
            {
                string exMessage = "";

                rtn = NttDataWA.Utils.utils.checkSystem(out exMessage);
                
                if (!rtn)
                {
                    this.lbl_msg.Text = "False";
                    Response.StatusCode = 500;
                    logger.Error("CheckPageError: " + exMessage);
                }
                else
                {
                    logger.Info("System Check OK");
                    this.lbl_msg.Text = "True";
                    Response.StatusCode = 200;
                }
                if (!Page.IsPostBack)
                {
                    this.lbl_esito_refresh_chiavi.Visible = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " " + ex.StackTrace);
            }
        }

        protected void btnRefreshChiaviConfig_Click(object sender, EventArgs e)
        {
            try
            {
                DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                DocsPaWR.InfoAmministrazione[] amministrazioni = ws.AmmGetListAmministrazioni();
                if (amministrazioni.Length > 0)
                {
                    foreach (DocsPaWR.InfoAmministrazione amm in amministrazioni)
                    {
                        NttDataWA.Utils.InitConfigurationKeys.remove(amm.IDAmm);
                        ws.clearHashTableChiaviConfig(amm.IDAmm);
                        NttDataWA.Utils.InitConfigurationKeys.getInstance(amm.IDAmm);
                        this.lbl_esito_refresh_chiavi.Visible = true;
                        this.lbl_esito_refresh_chiavi.Text = "Refresh delle chiavi effettuato con successo.";
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                this.lbl_esito_refresh_chiavi.Visible = true;
                this.lbl_esito_refresh_chiavi.Text = ex.Message;

            }
        }

        protected void btn_RefreshQlistandBEKeys_Click(object sender, EventArgs e)
        {

            try
            {
                DocsPaWR.DocsPaWebService docsPaWS = Utils.ProxyManager.GetWS();

                if (docsPaWS == null)
                    docsPaWS = new DocsPaWR.DocsPaWebService();

                bool retval = docsPaWS.RefreshQueryList("beatoATe");

                if (retval)
                {
                    this.Label2.Visible = true;
                    this.Label2.Text = "Refresh delle QL e CHIAVI e Grids effettuato con successo.";
                }
                else throw new Exception("Refresh delle QL e CHIAVI e Grids non effettuato.");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                this.Label2.Visible = true;
                this.Label2.Text = ex.Message;

            }
       
        }
    }
}