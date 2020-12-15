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
using System.Globalization;


namespace ConservazioneWA.PopUp
{
    public partial class GeneraSupporti : System.Web.UI.Page
    {
        string periodoVerSupp = "";       
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!Page.IsPostBack)
            {
                //if (Request.QueryString["periodoVer"] != null && Request.QueryString["periodoVer"]!=String.Empty)
                //{
                //    periodoVerSupp = Request.QueryString["periodoVer"];
                //    System.DateTime now = System.DateTime.Now;
                //    CultureInfo ci = new CultureInfo("en-US");
                //    string dataProxVer = (now.AddYears(Convert.ToInt32(periodoVerSupp))).ToString("dd/MM/yyyy", ci);
                //    this.data.Text = dataProxVer;
                //    this.hd_data.Value = dataProxVer;
                //}

                System.DateTime now = System.DateTime.Now;
                CultureInfo ci = new CultureInfo("en-US");
                this.data.Text = (now.AddMonths(6)).ToString("dd/MM/yyyy", ci);
                this.hd_data.Value = this.data.Text;

                if (Request.QueryString["stato"] == "V")
                {
                    this.lb_intestazione.Text = "Registra dati verifica";
                    this.lb_verifica.Visible = true;
                    this.ddl_verifica.Visible = true;
                    this.hd_verifica.Value = this.ddl_verifica.SelectedValue.ToString();
                }
            }
        }

        protected void btn_inserisci_Click(object sender, EventArgs e)
        {
            string collFisica = this.txt_collFisica.Text;
            string appo_note = this.TextArea1.Value;
            string note = appo_note.Replace("\r\n", " ");
            Session["noteSupporto"] = note;
            Session["collFisicaSupp"] = collFisica;
            if (this.data.Text == "")
                Session["dataProxVer"] = this.hd_data.Value;
            else
                Session["dataProxVer"] = this.data.Text;
            if (this.hd_verifica.Value!=null && this.hd_verifica.Value!=String.Empty)
                Session["esitoVerifica"] = this.ddl_verifica.SelectedValue.ToString();
            Response.Write("<script>window.returnValue=true; window.close();</script>");
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }
    }
}
