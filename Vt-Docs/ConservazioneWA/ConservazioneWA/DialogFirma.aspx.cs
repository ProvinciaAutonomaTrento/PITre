using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ConservazioneWA
{
    public partial class DialogFirma : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!IsPostBack)
            {
                this.btnApplicaFirma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnApplicaFirma.Attributes.Add("onmouseout", "this.className='cbtn';");
                this.btnAnnulla.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnAnnulla.Attributes.Add("onmouseout", "this.className='cbtn';");
            }
            string idIstanza = Request.QueryString["idIstanza"];
            this.hd_idIstanza.Value = idIstanza;

            int componentType = (string.IsNullOrEmpty(Request.QueryString["applet"])?0:1) ;
            this.hd_componentType.Value = componentType.ToString();
            if (componentType == 0)
                this.btnApplicaFirma.Attributes.Add("onClick", "ApplySign('" + this.GetQueryStringTipoFirma() + "');");
            else
                this.btnApplicaFirma.Attributes.Add("onClick", "ApplySignApplet('" + this.GetQueryStringTipoFirma() + "');");

            if (componentType==0)
                this.btnAnnulla.Attributes.Add("onClick", "CloseWindow('annulla');");
            else
                this.btnAnnulla.Attributes.Add("onClick", "CloseWindow('annulla');");
        }

        private string GetQueryStringTipoFirma()
        {
            string tipoFirma = Request.QueryString["TipoFirma"];

            if (tipoFirma == null || tipoFirma == string.Empty)
                tipoFirma = "sign";

            return tipoFirma;
        }
    }
}
