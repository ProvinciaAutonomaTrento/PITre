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

namespace DocsPAWA.popup
{
    public partial class dettaglioCampo : DocsPAWA.CssPage
    {

        protected string appTitle;
        protected int caratteriDisponibili = 2000;
        protected System.Web.UI.WebControls.Button btn_salva;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
                this.appTitle = titolo;
            else
                this.appTitle = "DOCSPA";

            if (!IsPostBack)
            {

                lblDettaglioCampo.Text = Session["SessionDescCampo"].ToString();
                txt_oggetto.Text = Session["SessionDescCampo"].ToString();
                Session.Remove("SessionDescCampo");

                string tipoCampo = Request.QueryString["tipoCampo"];
                if(tipoCampo!=null)
                {
                    switch (tipoCampo)
                    { 
                    
                        case "O":
                            this.Page.Title = appTitle + " > Descrizione campo oggetto";
                            this.nomePagina.Text = "Descrizione campo oggetto";
                            bool valore_o = false;
                            if(Session["Abilitato_modifica"]!=null)
                            {
                               Boolean.TryParse(Session["Abilitato_modifica"].ToString(), out valore_o);
                            }
                            if (valore_o)
                            {
                                this.pnl_no_ogg.Visible = false;
                                this.pnl_ogg.Visible = true;
                                this.nomePagina2.Text = "Descrizione campo oggetto";

                                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                                info = UserManager.getInfoUtente(this.Page);
                                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_OGGETTO");
                                if (!string.IsNullOrEmpty(valoreChiave))
                                    caratteriDisponibili = int.Parse(valoreChiave);
                                txt_oggetto.MaxLength = caratteriDisponibili;
                                clTesto.Value = (caratteriDisponibili - (txt_oggetto.Text.Length)).ToString();
                                txt_oggetto.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Oggetto'," + clTesto.ClientID + ")");
                                txt_oggetto.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','Descrizione Oggetto'," + clTesto.ClientID + ")");
                            }
                            break;

                        case "D":
                          
                            this.Page.Title = appTitle + " > Lista Destinatari in TO";
                            this.nomePagina.Text = "Destinatari principali";
                            break;

                        case "C":

                            this.Page.Title = appTitle + " > Lista Destinatari in CC";
                            this.nomePagina.Text = "Destinatari in conoscenza";
                            break;

                        case "U":

                            this.Page.Title = appTitle + " > Lista Destinatari";
                            this.nomePagina.Text = "Destinatari del protocollo";
                            break;
                    }
                }

            }
            this.btn_salva.Click += new System.EventHandler(this.btn_salva_click);
        }

        private void btn_salva_click(object sender, EventArgs e)
        {
            Session.Remove("oggetto_popup");
            Session["oggetto_popup"] = this.txt_oggetto.Text;
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "chiudi_finestra", "window.close();", true);
        }
    }
}
