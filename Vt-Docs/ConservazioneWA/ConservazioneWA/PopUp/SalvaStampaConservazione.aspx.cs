using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.PopUp
{
    public partial class SalvaStampaConservazione : System.Web.UI.Page
    {

        protected string idDocumento;

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Expires = -1;
            idDocumento = Request.QueryString["idDocumento"];

            if (!IsPostBack)
            {
                this.btnOk.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnOk.Attributes.Add("onmouseout", "this.className='cbtn';");
                this.btnAnnulla.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnAnnulla.Attributes.Add("onmouseout", "this.className='cbtn';");

                this.txtFileName.Text = idDocumento + ".pdf";
            }

            idDocumento = Request.QueryString["idDocumento"];
            

            this.btnOk.Attributes.Add("onClick", "GetPath();");
            this.btnAnnulla.Attributes.Add("onClick", "CloseWindow('annulla');");
            this.btnBrowseForFolder.Attributes.Add("onClick", "PerformSelectFolder();");

        }
    }
}