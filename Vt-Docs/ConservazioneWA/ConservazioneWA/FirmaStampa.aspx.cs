using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ConservazioneWA
{
    public partial class FirmaStampa : System.Web.UI.Page
    {

        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected string idDocumento;

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);


            if (!IsPostBack)
            {
                this.btnApplicaFirma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnApplicaFirma.Attributes.Add("onmouseout", "this.className='cbtn';");
                this.btnAnnulla.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnAnnulla.Attributes.Add("onmouseout", "this.className='cbtn';");

                this.btnSalvaDoc.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnSalvaDoc.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

                        
            this.idDocumento = Request.QueryString["idDocumento"];
            bool isFirmato = Convert.ToBoolean(Request.QueryString["firmato"]);
            this.hd_iddocumento.Value = idDocumento;

            if (!isFirmato)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "fetchlistacert", "FetchListaCertificati();", true);
                this.btnApplicaFirma.Attributes.Add("onClick", "ApplySign('" + this.GetQueryStringTipoFirma() + "');");
            }
            else
            {
                //impedisco la visualizzazione della lista certificati
                this.rowListaCertificati.Visible = false;
                this.lblListaCertificati.Visible = false;
                this.lstListaCertificati.Visible = false;

                //ingrandisco il frame con il visualizzatore
                iFrameSignedDoc.Attributes.Add("height", "540px");
                
                //modifico i pulsanti
                this.btnApplicaFirma.Visible = false;
                this.btnSalvaDoc.Visible = true;
                this.btnAnnulla.Text = "Chiudi";

            }
            this.btnAnnulla.Attributes.Add("onClick", "CloseWindow('annulla');");
            this.btnSalvaDoc.Attributes.Add("onClick", "DialogDownload();");
            

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