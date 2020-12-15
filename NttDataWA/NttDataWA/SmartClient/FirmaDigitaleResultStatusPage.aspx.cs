using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.SmartClient
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FirmaDigitaleResultStatusPage : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                this.Response.Expires = -1;

                FirmaDigitaleResultManager.SetData(
                            new FirmaDigitaleResultStatus
                            {
                                 Status = this.Status,
                                 StatusDescription = this.StatusDescription,
                                 IdDocument = this.IdDocument
                            }
                    );
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool Status
        {
            get
            {
                bool retValue;
                bool.TryParse(this.Request.QueryString["status"], out retValue);
                return retValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string StatusDescription
        {
            get
            {
                return this.Request.QueryString["statusDescription"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdDocument
        {
            get
            {
                return this.Request.QueryString["idDocument"];
            }
        }
    }
}