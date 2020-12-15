using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;

namespace NttDataWA
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        private static ILog logger = LogManager.GetLogger(typeof(ErrorPage));


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                this.InitializePage();
            }
        }      

        public void InitializePage()
        {
            string l3 = string.Empty;            
            this.TxtBoxError.Rows = 17;
            this.lblTxt.Text = "ATTENZIONE!<br />Si è verificato un errore.";
            this.BtnLogin.Text = "Torna";
            string dataora = DateTime.Now.ToString() + "\n\n";

            string l1 = "Descrizione: \n"  + Server.HtmlDecode(Convert.ToString(Request.QueryString["Message"])) + "\n\n";
            string l2 = "Nel file: \n" + Server.HtmlDecode(Convert.ToString(Request.QueryString["fileName"])) + "\n\n";

            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["lineCol"])))
            l3 = "In: \n" + Server.HtmlDecode(Convert.ToString(Request.QueryString["lineCol"])) + "\n\n";            

            string l4 = "Metodo: \n" + Server.HtmlDecode(Convert.ToString(Request.QueryString["methodName"])) + "\n\n";
            if (Request.QueryString["Message"] == null && Request.QueryString["fileName"] == null && Request.QueryString["methodName"] == null)
            {
                this.TxtBoxError.Text = "Attenzione si è verificato un errore, contattare il gruppo di supporto";
            }
            else
            {
                this.TxtBoxError.Text = dataora + l1 + l2 + l3 + l4;
            }

            // Scrittura nel log
            logger.Error(dataora);
            logger.Error(l1);
            logger.Error(l2);
            logger.Error(l3);
            logger.Error(l4);

            if (Request.QueryString["filename"] != null && Request.QueryString["fileName"].Contains("Popup"))
            {
                this.plcPopup.Visible = true;
                this.plcPopup2.Visible = true;
            }
            else
                this.plcBase.Visible = true;
        }

    }
}