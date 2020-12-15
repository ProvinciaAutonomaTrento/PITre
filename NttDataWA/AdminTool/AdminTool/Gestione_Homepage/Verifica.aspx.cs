using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using System.Configuration;


namespace Amministrazione.Gestione_Homepage
{
    public class Verifica : System.Web.UI.Page
    {
        #region WebControls e variabili
        protected AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
        protected System.Web.UI.WebControls.Label lbl_doc_no_mitt;
        protected System.Web.UI.WebControls.Label lbl_doc_no_dest;
        protected System.Web.UI.WebControls.Label lbl_doc_cc;
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }
        #endregion

        #region Page Load e inizializzazione dati
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            //----- CONTROLLO DELL'UTENTE AMMINISTRATORE CONNESSO -----------

            if (!ws.CheckSession(Session.SessionID))
            {
                Response.Redirect("../Exit.aspx?FROM=ABORT");
            }

            if (!IsPostBack)
            {
                SAAdminTool.DocsPaWR.DocsPaWebService wws = new SAAdminTool.DocsPaWR.DocsPaWebService();
                string idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                string verifica = wws.VerificaDocErrati(idAmm);

                if (!string.IsNullOrEmpty(verifica))
                {
                    if (verifica.Contains("^"))
                    {
                        string[] lista = verifica.Split('^');
                        this.lbl_doc_no_dest.Text = lista[0];
                        this.lbl_doc_no_mitt.Text = lista[1];
                        this.lbl_doc_cc.Text = lista[2];
                    }
                }
            }
        }
        #endregion

    }
}
