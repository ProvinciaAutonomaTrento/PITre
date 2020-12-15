using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;

namespace DocsPAWA.Help
{
    public partial class Manuale : System.Web.UI.Page
    {
        protected string from;

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.from = "Manuale.htm?from=" + this.Request.QueryString["from"];
            //if (System.Configuration.ConfigurationManager.AppSettings["MANUALE"] != null && System.Configuration.ConfigurationManager.AppSettings["MANUALE"] != "")
            //    this.from = System.Configuration.ConfigurationManager.AppSettings["MANUALE"] + "/" + this.from;

            string nomeBookmark = this.Request.QueryString["from"];
            string sLine = string.Empty;

            try
            {
                string valoreChiave = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_ELENCO_BOOKMARKS");

                if (!string.IsNullOrEmpty(valoreChiave))
                {
                    StreamReader objReader = new StreamReader(valoreChiave);
                    while (sLine != null)
                    {
                        sLine = objReader.ReadLine();
                        if (sLine != null && sLine.Contains(nomeBookmark))
                            this.from = sLine.Substring(sLine.IndexOf("="));
                    }
                    objReader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }
    }
}
