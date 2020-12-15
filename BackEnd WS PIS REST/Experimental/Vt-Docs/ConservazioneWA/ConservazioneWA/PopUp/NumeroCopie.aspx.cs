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

namespace ConservazioneWA.PopUp
{
    public partial class NumeroCopie : System.Web.UI.Page
    {
        bool policyError = false;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!string.IsNullOrEmpty(this.Request.QueryString["policyInvalid"]))
            {
                string polInvalid = this.Request.QueryString["policyInvalid"];
                Boolean.TryParse(polInvalid, out policyError);
                if (!policyError)
                {
                    pnlPlocyNonValidata.Visible = false;
                }
            }
            
            Response.Expires = -1;

            btn_inserisci.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_inserisci.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_chiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_chiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
        }

        protected void btn_inserisci_Click(object sender, EventArgs e)
        {
            if(policyError)
            {
                if (txtMotivoAccettazione.Value.Length == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert(\"Inserire il motivo della messa in lavorazione\");", true);
                    return;
                }
                else
                {
                    //qui salverò sul DB il motivo
                    Session["MotivoMessaInLavorazione"] = txtMotivoAccettazione.Value;
                }

            }
            string copie = this.txtNumeroSupporti.Text;
            Session["copie"] = copie;
            //Response.Write("<script>parent.location.href='../Ricerca.aspx'; window.close();</script>");
            Response.Write("<script>window.returnValue=true; window.close();</script>");
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }

  
    }
}
