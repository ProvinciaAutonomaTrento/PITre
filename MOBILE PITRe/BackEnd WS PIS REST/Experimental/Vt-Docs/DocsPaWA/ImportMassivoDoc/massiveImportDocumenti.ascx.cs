using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using DocsPAWA;


namespace DocsPAWA.ImportMassivoDoc
{
    public partial class massiveImportDocumenti : System.Web.UI.UserControl
    {
        private string absolutePath = string.Empty;
        private string codFasc = string.Empty;
        private string foldName = string.Empty;
        private DocsPAWA.DocsPaWR.Fascicolo fasc = null;
        private DocsPAWA.DocsPaWR.FileDocumento fd = null;
        private string type = string.Empty;
        private DataSet ds = null;
        DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.getWS();

        protected void Page_Load(object sender, EventArgs e)
        {
            int statusCode = 0;
            if ((Request.QueryString["Absolutepath"] != null) && (Request.QueryString["Absolutepath"] != ""))
            {
                //1. controllo se arriva la path assoluta
                absolutePath = decodeQueryString (Request.QueryString["Absolutepath"].ToString());

                //1.1 Leggo il content del file 
                byte[] ba = Request.BinaryRead(Request.ContentLength);
                fd = new DocsPAWA.DocsPaWR.FileDocumento();
                fd.content = ba;

                //1.2 leggo il codice fascicolo
                if (Request.QueryString["codFasc"] != null && Request.QueryString["codFasc"] != "")
                {
                    codFasc = decodeQueryString (Request.QueryString["codFasc"].ToString());
                    //fasc = FascicoliManager.getFascicoloDaCodice(this.Page, codFasc);
                    fasc = FascicoliManager.getFascicoloDaCodice2(this.Page, codFasc, Request.QueryString["idTitolario"]);
                }

                //1.4. tipo passato
                if (Request.QueryString["type"] != null && Request.QueryString["type"] != "")
                    type = Request.QueryString["type"].ToString();

                if (Request.QueryString["foldName"] != null && Request.QueryString["foldName"] != "")
                    foldName = decodeQueryString(Request.QueryString["foldName"].ToString());

                //1.5. se tutte le condizioni sono ok... procedo
                if ((codFasc != "") && (absolutePath != ""))
                {

                    /* ABBATANGELI GIANLUIGI
                     * prendo la cartella selezionata per l'acquisizione massiva */
                    DocsPaWR.Folder folder = null;
                    if (Session["fascDocumenti.FolderSel"] != null)
                    {
                        folder = (DocsPaWR.Folder)Session["fascDocumenti.FolderSel"];
                    }

                    statusCode = importDocManager.checkORCreateDocFolderFasc(this.Page, fasc, absolutePath, fd, foldName, type, folder);
                    // parsing degli stati
                    parseStatusCode(statusCode);
                }

            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            if (dgrLog.Items.Count > 0)
                btn_stampa.Enabled = true;
            else btn_stampa.Enabled = false;
        }

        private void parseStatusCode(int statusCode)
        {
                switch (statusCode)
                {
                    case 1:
                        this.Response.StatusCode = 1;
                        this.Response.StatusDescription = "File Acquisito";
                        break;
                    case 2:
                        this.Response.StatusCode = 2;
                        this.Response.StatusDescription = "Formato File non supportato";
                        break;
                    case 3:
                        this.Response.StatusCode = 3;
                        this.Response.StatusDescription = "Cartella Acquisita";
                        break;
                    case 4:
                        this.Response.StatusCode = 4;
                        this.Response.StatusDescription = "Cartella Esistente";
                        break;
                    case 0:
                        this.Response.StatusCode = 0;
                        this.Response.StatusDescription = "Errore in aquisizione";
                        break;
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaFileContent"></param>
        public void SetMetaFileContent(string metaFileContent)
        {
            // Invio metadati dell'elaborazione
            this.dgrLog.DataSource = this.ParseMetaFile(metaFileContent);
            this.dgrLog.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaFileContent"></param>
        /// <returns></returns>
        private DataSet ParseMetaFile(string metaFileContent)
        {
            ds = new DataSet();
            DataTable table = new DataTable();
            table.Columns.Add("TS", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Absolutepath", typeof(string));
            table.Columns.Add("ErrorMessage", typeof(string));
            ds.Tables.Add(table);

            using (StringReader reader = new StringReader(metaFileContent))
            {
                string line = reader.ReadLine();

                while (!string.IsNullOrEmpty(line))
                {
                    DataRow row = table.NewRow();
                    this.FetchRowEntry(line, row);
                    table.Rows.Add(row);

                    line = reader.ReadLine();
                } 
            }
            Session.Add("dsData", ds);

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryLine"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private void FetchRowEntry(string entryLine, DataRow row)
        {
            foreach (string entry in entryLine.Split('|'))
            {
                string[] keyValuePair = entry.Split('=');

                if (keyValuePair.Length == 2)
                {
                        switch (keyValuePair[0])
                        {
                            case "TS":
                                if (keyValuePair[1] != "")
                                {
                                    row[keyValuePair[0]] = System.DateTime.Now.ToShortDateString() + " " + keyValuePair[1].ToString();
                                }
                                break;
                            case "Type":
                                if (keyValuePair[1] == "FILE") row[keyValuePair[0]] = "File";
                                else if (keyValuePair[1] == "DIR") row[keyValuePair[0]] = "Cartella";
                                break;
                            case "Absolutepath":
                                row[keyValuePair[0]] = keyValuePair[1];
                                break;
                            case "ErrorMessage":
                                row[keyValuePair[0]] = keyValuePair[1];
                                break;
                        }
                    }
                }
        }

        private string decodeQueryString(string queryString)
        {
            return System.Web.HttpUtility.UrlDecode(queryString);
        }

        protected void btn_stampa_Click(object sender, ImageClickEventArgs e)
        {

        }

    }
}