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

namespace SAAdminTool.ActivexWrappers
{
    public partial class ProjectToFSWrapper : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!this.IsPostBack)
            //{
            //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "InitializeCtrlScript", "InitializeCtrl();", true);
            //}
        }

        /// <summary>
        /// Reperimento Url del WebService di DocsPa
        /// </summary>
        protected string WebServiceUrl
        {
            get
            {
                return SAAdminTool.Properties.Settings.Default.AdminTool_DocsPaWR_DocsPaWebService;
            }
        }

        /// <summary>
        /// Reperimento InfoUtente corrente
        /// </summary>
        protected DocsPaWR.InfoUtente InfoUtente
        {
            get
            {
                return UserManager.getInfoUtente();
            }
        }

        /// <summary>
        /// Id del fascicolo da importare
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string IdFascicolo
        {
            get
            {
                if (this.ViewState["IdFascicolo"] != null)
                    return this.ViewState["IdFascicolo"].ToString();
                else
                    return null;
            }
            set
            {
                this.ViewState["IdFascicolo"] = value;
            }
        }
    }
}