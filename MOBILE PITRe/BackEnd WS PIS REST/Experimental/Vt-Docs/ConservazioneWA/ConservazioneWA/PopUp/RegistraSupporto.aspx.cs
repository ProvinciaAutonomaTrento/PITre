using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.PopUp
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RegistraSupporto : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                btnRegistra.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnRegistra.Attributes.Add("onmouseout", "this.className='cbtn';");

                btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegistra_Click(object sender, EventArgs e)
        {
            ConservazioneWA.Utils.ConservazioneManager.RegistraSupportoRimovibile(
                                    (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                    this.IdIstanza, 
                                    this.IdSupporto, 
                                    this.txtCollocazione.Text,
                                    this.txtNote.Text);

            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "close", "window.retValue = true; window.close();", true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdIstanza
        {
            get
            {
                return this.Request.QueryString["idIstanza"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdSupporto
        {
            get
            {
                return this.Request.QueryString["idSupporto"];
            }
        }
    }
}