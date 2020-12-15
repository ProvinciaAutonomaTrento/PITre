using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

namespace DocsPAWA.AdminTool.Gestione_News
{
    public partial class News : System.Web.UI.Page
    {
        private string idAmm;
        protected System.Web.UI.WebControls.Button btn_refresh;
        protected System.Web.UI.WebControls.CheckBox enable_news;
        protected System.Web.UI.WebControls.TextBox news;
        protected System.Web.UI.WebControls.Label lbl_position;
        protected System.Web.UI.WebControls.Label risultato_mod;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;
            Session["AdminBookmark"] = "GestioneNews";
            idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            if (!IsPostBack)
            {
                this.lbl_position.Text = "&nbsp;&bull;&nbsp;Amministrazione: " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "1");
                this.getDatiAmm(idAmm);
            }
        }
 
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        protected void Aggiorna(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.news.Text) && this.enable_news.Checked)
            {
                Response.Write("<script>alert('Non si può volere l\'apertura di una pagina delle news senza specificarne una URL corretta');</script>");
            }
            else
            {
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
                if (this.setDatiAmm(idAmm))
                {
                    this.risultato_mod.Visible = true;
                    this.risultato_mod.Text = "Modifica effettuata correttamente";
                }
                else
                {
                    this.risultato_mod.Visible = true;
                    this.risultato_mod.Text = "Si sono verificati degli errori nel salvataggio dei dati: riprovare!";
                }
            }
        }

        public void getDatiAmm(string idAmm)
        {
            //AmmUtils.WebServiceLink wws = new AmmUtils.WebServiceLink();
            //DocsPAWA.DocsPaWR.InfoAmministrazione amm = wws.GetInfoAmmCorrente(idAmm);
            //this.enable_news.Checked = amm.enable_news;
            //this.news.Text = amm.news;
            DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            string result = wws.getNews(idAmm);
            if (!string.IsNullOrEmpty(result))
            {
                string[] res = result.Split('^');
                if (res[0].ToString().Equals("1"))
                    this.enable_news.Checked = true;
                else
                    this.enable_news.Checked = false;
                this.news.Text = res[1].ToString();
            }
        }

        public bool setDatiAmm(string idAmm)
        {
            DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            bool result = wws.setNews(idAmm, this.news.Text, this.enable_news.Checked);
            return result;
        }
    }
}
