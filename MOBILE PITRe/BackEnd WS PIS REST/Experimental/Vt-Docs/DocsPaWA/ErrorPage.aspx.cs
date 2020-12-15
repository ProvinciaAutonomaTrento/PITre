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
using System.Text;
using log4net;

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for ErrorPage.
	/// </summary>
	public class form : System.Web.UI.Page
	{
        private ILog logger = LogManager.GetLogger(typeof(form));
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Button btn_rtnLogin;
		protected System.Web.UI.WebControls.Label lbl_mgserrore;
	
		private void Page_Load(object sender, System.EventArgs e)

		{
            // Solo nel caso in cui non si è in postback bisogna eseguire
            // la verififca dell'errore
            if (!Page.IsPostBack)
                // Se la sessione esiste ancora...
                if (Session != null)
                    // ...allora di procede con l'identificazione della 
                    // tipologia di errore.
                    // Se si è nel caso in cui è stato impostato il ruolo...
                    if (!String.IsNullOrEmpty((string)Session["noruolo"]))
                    {
                        // ...si scrive nella label la descrizione
                        this.lbl_mgserrore.Text = Session["noruolo"].ToString();

                        // ...e si rumuove dalla sessione noruolo
                        Session.Remove("noruolo");
                    }
                    else
                        // ...altrimenti si vede se ErrorManager.error 
                        // è valido...
                        if (!String.IsNullOrEmpty((string)
                            Session["ErrorManager.error"]))
                            // ...si costruisce la descrizione dettagliata
                            // dell'errore e la si stampa nel log
                            logger.Error(this.GetErrorDetails((string) Session["ErrorManager.error"]));
                        else
                            // ...altrimenti se userData è valido...
                            if (Session["userData"] != null)
                                // ...si redirezione l'utente alla pagina di login
                                ErrorManager.redirectToLoginPage(this);
                            else
                                // ...altrimenti si indica che non è stato
                                // possibile identificare l'errore
                                logger.Error(this.GetErrorDetails("Errore non identificato"));
		}

        /// <summary>
        /// Funzione per la creazione del dettaglio dell'errore
        /// </summary>
        /// <returns>Una stringa con la descrizione dell'errore</returns>
        private Exception GetErrorDetails(string error)
        {
            // Prelevamento del messaggio di errore dalla sessione
            StringBuilder msg = new StringBuilder();
            msg.AppendLine((string)Session["ErrorManager.error"]);

            // Rimozione della chiave dalla sessione
            Session.Remove("ErrorManager.error");

            // Si aggiunge alla descrizione dell'errore l'indirizzo ip
            // della macchina cui è collegato il client, il nome utente
            // e data e ora in cui si è verificato l'errore
            msg.AppendLine();
           // msg.AppendLine("Indirizzo IP: " + Request.ServerVariables["LOCAL_ADDR"]);
            msg.AppendLine();
            if(UserManager.getInfoUtente()!=null)
              msg.AppendLine("Nome utente: " + UserManager.getInfoUtente().userId);
            msg.AppendLine();
            msg.AppendLine("Data e ora: " + DateTime.Now);

            return new Exception(msg.ToString());

        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.form_PreRender);

		}
		#endregion

		private void btn_rtnLogin_Click(object sender, System.EventArgs e)
		{
			
			//Session.Abandon();
			
			

		}
	
		private void form_PreRender(object sender, System.EventArgs e)
		{
			
		}
	}
}
