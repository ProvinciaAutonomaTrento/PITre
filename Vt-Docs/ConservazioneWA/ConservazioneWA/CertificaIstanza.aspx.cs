using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA
{
    public partial class CertificaIstanza : System.Web.UI.Page
    {

        protected WSConservazioneLocale.InfoUtente infoUtente;
        protected string idEsibizione = string.Empty;
        protected string idCert = string.Empty;
        protected bool daFirmare = false;

        protected void Page_Load(object sender, EventArgs e)
        {

            Response.Expires = -1;
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            this.idEsibizione = Request.QueryString["idEsibizione"];
            this.idCert = Request.QueryString["idCertificazione"];
            this.daFirmare = (Request.QueryString["firma"].ToString() == "y");
            this.hd_iddocumento.Value = idCert;
            this.hd_idesibizione.Value = idEsibizione;

            if (daFirmare)
            {
                this.btnApplicaFirma.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnApplicaFirma.Attributes.Add("onmouseout", "this.className='cbtn';");
                this.btnApplicaFirma.Attributes.Add("onClick", "ApplySign('" + this.GetQueryStringTipoFirma() + "');");
                this.btnAnnulla.Text = "Annulla";
                ClientScript.RegisterStartupScript(this.GetType(), "fetchListaCert", "FetchListaCertificati();", true);
            }
            else
            {
                this.btnApplicaFirma.Visible = false;
                this.rowCertificati1.Visible = false;
                this.rowCertificati2.Visible = false;
                this.lblListaCertificati.Visible = false;
                this.btnAnnulla.Text = "Chiudi";

                this.iFrameSignedDoc.Attributes.Add("height", "520px");

            }

            this.btnAnnulla.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btnAnnulla.Attributes.Add("onmouseout", "this.className='cbtn';");
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