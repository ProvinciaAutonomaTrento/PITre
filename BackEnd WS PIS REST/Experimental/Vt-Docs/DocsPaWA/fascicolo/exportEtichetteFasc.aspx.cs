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

namespace DocsPAWA.fascicolo
{
    public partial class exportEtichetteFasc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Recupero dalla sessione il file documento
                DocsPaWR.FileDocumento file = (DocsPaWR.FileDocumento)Session["FileManager.selectedReport"];
                // Rimuovo dalla sessione il file documento
                Session.Remove("FileManager.selectedReport");

                if (file != null && file.content.Length > 0)
                {
                    Response.ContentType = file.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=" + file.fullName);
                    Response.AddHeader("content-lenght", file.content.Length.ToString());
                    Response.BinaryWrite(file.content);
                }
                else
                {
                    this.executeJS("<SCRIPT>alert('Impossibile generare il file di esportazione dei dati.\nContattare l'amministratore di sistema.');self.close();</SCRIPT>");
                }
            }
            catch
            {
                this.executeJS("<SCRIPT>alert('Impossibile generare il documento di esportazione dei dati.\nContattare l'amministratore di sistema.');self.close();</SCRIPT>");
            }
        }

        private void executeJS(string key)
        {
            if (!this.Page.IsStartupScriptRegistered("theJS"))
                this.Page.RegisterStartupScript("theJS", key);
        }
    }
}
