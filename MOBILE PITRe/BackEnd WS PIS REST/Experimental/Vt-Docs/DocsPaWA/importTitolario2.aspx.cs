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
using System.Data.OleDb;

namespace DocsPAWA
{
    public partial class importTitolario2 : System.Web.UI.Page
    {
        protected DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_errore.Text = "";
            btn_log.Visible = false;
            pnl_log.Visible = false;
        }

        protected void btn_import_Click(object sender, EventArgs e)
        {
            //Verifica dati obbligatori e loro correttezza
            if (txt_Path.Text == "")
            {
                lbl_errore.Text = "Inserire i dati obbligatori contrassegnati da un asterisco.";
                return;
            }

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            //xlsConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + txt_Path.Text +";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + txt_Path.Text + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1'";
            xlsConn.Open();
            
            OleDbCommand xlsCmd = new OleDbCommand("select * from [Sheet 1$]", xlsConn);
            xlsReader = xlsCmd.ExecuteReader();

            while (xlsReader.Read())
            {
                if (get_string(xlsReader, 0) != null && get_string(xlsReader, 0) != "" &&
                    get_string(xlsReader, 1) != null && get_string(xlsReader, 1) != "")
                {
                    string codice = get_string(xlsReader, 0);
                    string descrizione = get_string(xlsReader, 1);
                    wws.Timeout = System.Threading.Timeout.Infinite;

                    DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();

                    bool result = wws.modificaTemplateExcelTitolario(codice, descrizione, "importTitolario.xls", sessionManager.getUserAmmSession());
                    if (!result)
                    {
                        lbl_errore.Text = "Problemi durante l'importazione, consultare il file di log.";
                        btn_log.Visible = true;
                        return;
                    }
                }
            }

            if (xlsReader != null)
                xlsReader.Close();
            if (xlsConn != null)
                xlsConn.Close();

            lbl_errore.Text = "Importazione avvenuta con successo.";
        }

        protected void bnt_log_Click(object sender, EventArgs e)
        {
            txt_log.Text = "";
            ArrayList fileLog = new ArrayList(wws.getLogImportTitolario());
            foreach (string sOutput in fileLog)
                txt_log.Text += sOutput + "\n";
            txt_log.ReadOnly = true;

            pnl_log.Visible = true;
            btn_log.Visible = true;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString();
        }
    }
}
