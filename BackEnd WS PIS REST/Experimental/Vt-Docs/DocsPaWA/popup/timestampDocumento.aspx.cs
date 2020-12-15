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
using System.Linq;
namespace DocsPAWA.popup
{
    public partial class timestampDocumento : DocsPAWA.CssPage
    {
        protected DocsPaWR.InfoUtente infoUtente;
        protected DocsPaWR.FileRequest fileRequest;
        protected ArrayList timestampsDoc;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            infoUtente = UserManager.getInfoUtente(this);
            fileRequest = FileManager.getSelectedFile(this);
            timestampsDoc = DocumentManager.getTimestampsDoc(infoUtente, fileRequest);

            if (!IsPostBack)
            {
                caricaDgTimestamp();
            }

            AdminTool.UserControl.ScrollKeeper skTimestamps = new AdminTool.UserControl.ScrollKeeper();
            skTimestamps.WebControl = "div_timestamps";
            this.Form.Controls.Add(skTimestamps);

            if (UserManager.getRuolo(this).funzioni.Where(funz => funz.codice.ToUpper().Equals("DO_TIMESTAMP")).FirstOrDefault() != null)
            {
                btn_associaTimestamp.Enabled = true;
                btn_creaTsd.Enabled = true;
            }
        }

        private void caricaDgTimestamp()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SYSTEM_ID");
            dt.Columns.Add("NUMERO_DI_SERIE");
            dt.Columns.Add("DTA_CREAZIONE");
            dt.Columns.Add("DTA_SCADENZA");

            foreach (DocsPaWR.TimestampDoc timestampDoc in timestampsDoc)
            {
                DataRow rw = dt.NewRow();
                rw[0] = timestampDoc.SYSTEM_ID;
                rw[1] = timestampDoc.NUM_SERIE;
                rw[2] = timestampDoc.DTA_CREAZIONE + "(GMT)";
                rw[3] = timestampDoc.DTA_SCADENZA + "(GMT)";
                dt.Rows.Add(rw);
            }

            dt.AcceptChanges();
            dg_timestamp.DataSource = dt;
            dg_timestamp.DataBind();

            dg_timestamp.SelectedIndex = -1;
        }

        protected void dg_timestamp_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            int elSelezionato = e.Item.ItemIndex;
            DocsPaWR.TimestampDoc timestampDoc = (DocsPaWR.TimestampDoc)timestampsDoc[elSelezionato];
            dg_timestamp.SelectedIndex = elSelezionato;

            switch (e.CommandName)
            {
                case "Dettagli":
                    pnl_dettagli.Visible = true;
                    lbl_soggetto.Text = timestampDoc.SOGGETTO;
                    lbl_paese.Text = timestampDoc.PAESE;
                    lbl_certificato.Text = timestampDoc.S_N_CERTIFICATO;
                    lbl_algoritmo.Text = timestampDoc.ALG_HASH;
                    lbl_numeroSerie.Text = timestampDoc.NUM_SERIE;
                    break;

                case "Salva":
                    string fileName = timestampDoc.DOC_NUMBER + "-" + timestampDoc.NUM_SERIE + ".tsr";
                    Response.AppendHeader("Content-Disposition", "attachment; filename="+fileName);
                    Response.BinaryWrite(Convert.FromBase64String(timestampDoc.TSR_FILE));
                    Response.End();                    
                    break;
            }
        }

        protected void btn_associaTimestamp_Click(object source, EventArgs e)
        {
            FileManager fileManager = new FileManager();
            DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile(this);
            DocsPaWR.FileDocumento fileDocumento = fileManager.getFile(this, fileRequest, false, true);

            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);

            string stringFile = BitConverter.ToString(fileDocumento.content);
            stringFile = stringFile.Replace("-", "");

            DocsPaWR.InputMarca inputMarca = new DocsPaWR.InputMarca();
            inputMarca.applicazione = infoUtente.urlWA;
            inputMarca.file_p7m = stringFile;
            inputMarca.riferimento = infoUtente.userId;

            String message = string.Empty;
            DocumentManager.executeAndSaveTSR(infoUtente, inputMarca, fileRequest, out message);

            if (message == "OK")
            {
                timestampsDoc = DocumentManager.getTimestampsDoc(infoUtente, fileRequest);
                caricaDgTimestamp();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ErroreTimespamp", "alert('"+message+"');", true);
            }

            pnl_dettagli.Visible = false;
        }

        protected void btn_creaTsd_Click(object sender, EventArgs e)
        {
            DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(this);
            DocsPaWR.FileRequest fileRequest = FileManager.getSelectedFile(this);
            DocsPaWR.FileRequest  tsdVer =DocumentManager.CreateTSDVersion(infoUtente, fileRequest);
            if (tsdVer == null)
                ClientScript.RegisterStartupScript(this.GetType(), "ErroreTimespamp", "alert('Non è stato possibile creare il file TSD. Verificare:\\n• che la marca temporale non sia già in formato TSD;\\n• che sia stato associato un timestamp al documento.');", true);
            else
            {
                 DocsPAWA.DocumentManager.setDocumentoSelezionato (DocsPAWA.DocumentManager.getDettaglioDocumento(this.Page , fileRequest.docNumber,""));
                
                string funct = " window.close(); ";
                Response.Write("<script> " + funct + "</script>");
            }
        }
    }
}