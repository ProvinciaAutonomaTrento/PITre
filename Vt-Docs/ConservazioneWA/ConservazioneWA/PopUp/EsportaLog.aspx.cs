using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPaVO.documento;

namespace ConservazioneWA.PopUp
{
    public partial class EsportaLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            WSConservazioneLocale.FileDocumento fd = (WSConservazioneLocale.FileDocumento)Session["fileReport"];
            string nomeFile = fd.name;
            this.hd_nomeDoc.Value = nomeFile.Substring(0, nomeFile.Length - 4);

            Response.Expires = -1;
            if (!IsPostBack)
            {
                
                this.btnSalva.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnSalva.Attributes.Add("onmouseout", "this.className='cbtn';");
                this.btnSalva.Attributes.Add("onClick", "DialogDownload();");
                this.btnSalva.Visible = true;

                this.btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
                this.btnChiudi.Attributes.Add("onClick", "CloseWindow();");
                this.btnChiudi.Visible = true;

            }

        }
    }
}