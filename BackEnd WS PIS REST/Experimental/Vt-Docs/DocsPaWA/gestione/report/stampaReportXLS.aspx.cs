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

namespace DocsPAWA.gestione.report
{
    public partial class stampaReportXLS : System.Web.UI.Page
    {
        private DocsPAWA.DocsPaWR.FileDocumento _objFileReport = null;

        public class stampaReportXLS_Session
        {
            private const string SESSION_KEY = "stampaReportXLS";

            public void SetSessionReportXLS(DocsPAWA.DocsPaWR.ExportExcelClass objStampaReportXLS)
            {
                if (System.Web.HttpContext.Current.Session[SESSION_KEY] == null)
                {
                    System.Web.HttpContext.Current.Session.Add(SESSION_KEY, objStampaReportXLS);
                }
            }

            public DocsPAWA.DocsPaWR.ExportExcelClass getSessionReportXLS()
            {
                DocsPAWA.DocsPaWR.ExportExcelClass objStampaReportXLS = new DocsPAWA.DocsPaWR.ExportExcelClass();

                if (System.Web.HttpContext.Current.Session[SESSION_KEY] != null)
                {
                    objStampaReportXLS = (DocsPAWA.DocsPaWR.ExportExcelClass)System.Web.HttpContext.Current.Session[SESSION_KEY];
                }
                return objStampaReportXLS;
            }

            public bool ExistSessionReportXLS()
            {
                bool exist = false;
                if (System.Web.HttpContext.Current.Session[SESSION_KEY] != null)
                {
                    exist = true;
                }
                return exist;
            }

            public void ReleaseSessionReportXLS()
            {
                System.Web.HttpContext.Current.Session.Remove(SESSION_KEY);                            
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                DocsPAWA.DocsPaWR.ExportExcelClass obj = new DocsPAWA.DocsPaWR.ExportExcelClass();                                      
                stampaReportXLS_Session sessione = new stampaReportXLS_Session();

                // recupera l'oggetto in sessione
                obj = sessione.getSessionReportXLS();
                // rilascia la sessione
                sessione.ReleaseSessionReportXLS();

                if (obj != null && obj.file != null && obj.file.content.Length > 0)
                {                    
                    Response.ContentType = obj.file.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=" + obj.file.fullName);
                    Response.AddHeader("content-lenght", obj.file.content.Length.ToString());
                    Response.BinaryWrite(obj.file.content);                    
                }
                else
                    this.executeJS("<SCRIPT>alert('Si è verificato un errore nella generazione del report Excel');self.close();</SCRIPT>");
            }
            catch (Exception ex)
            {
                this.executeJS("<SCRIPT>alert('Errore di sistema: " + ex.Message.Replace("'", "\\'") + "');self.close();</SCRIPT>");
            }
        }

        private void executeJS(string key)
        {
            if (!this.Page.IsStartupScriptRegistered("theJS"))
                this.Page.RegisterStartupScript("theJS", key);
        }
    }
}
