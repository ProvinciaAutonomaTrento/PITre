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

namespace DocsPAWA.documento
{
	/// <summary>
	/// Summary description for statoDocAcquisito.
	/// </summary>
	public class statoDocAcquisito : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
		}

        /// <summary>
        /// Creazione link per l'accesso diretto al documento dall'esterno
        /// </summary>
        protected string Link
        {
            get
            {
                string toRet;

                if (DocumentManager.getDocumentoSelezionato() != null)
                {
                    StringBuilder link = new StringBuilder(Utils.getHttpFullPath() +
                        "/visualizzaLink.aspx?groupId=" +
                        UserManager.getInfoUtente().idGruppo +
                        "&docNumber=" +
                        DocumentManager.getDocumentoSelezionato().docNumber +
                        "&idProfile=" + DocumentManager.getDocumentoSelezionato().systemId +
                        "&numVersion=");

                    if (FileManager.getSelectedFile() != null)
                        if (!String.IsNullOrEmpty(FileManager.getSelectedFile().version))
                            link.Append("");
                        else
                            link.Append(FileManager.getSelectedFile().version);

                    toRet = link.ToString();
                }
                else
                    toRet = String.Empty;

                return toRet;
            }
        }

        protected string DocName
        {
            get 
            {
                return DocumentManager.getDocumentoSelezionato().docNumber;
 
            }
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
		}
		#endregion
	}
}
