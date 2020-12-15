using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.popup
{
    public partial class RemotePdfStamp : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            Session["ResultRemotePdfStamp"] = false;
        }

        protected void doAction(object sender, EventArgs e)
        {
            //this.ClientScript.RegisterClientScriptBlock(this.GetType(), "PB", "__doPostBack();", true);
            //System.Threading.Thread.Sleep(2000);
            btn_ok.Enabled = false;
            btn_close.Enabled = false;
            
            bool result = DoRemotePdfStamp();

            if (result)
            {
                this.lbl_esito_op.Text = "Operazione avvenuta con successo.";
                this.lbl_esito_op.Visible = true;
            }
            else
            {
                this.lbl_esito_op.Text = "Si è verificato un errore. Operazione non effettuata.";
                this.lbl_esito_op.Visible = true;
            }

            btn_ok.Visible = false;
            btn_close.Visible = false;

            
            initLabel.Visible = false;
            btn_chiudi.Visible = true;

            Session["ResultRemotePdfStamp"] = result;

            //string script = "var vReturnValue = new Object();" +
            //                "vReturnValue.Value = " + result + ";" +
            //                "window.returnValue = vReturnValue;" +
            //                "alert('1. RemotePdfStamp');";// +
            ////"window.close();";
            //this.ClientScript.RegisterClientScriptBlock(this.GetType(), "retValPage", script, true);
            
        }

        private bool DoRemotePdfStamp()
        {
            bool retVal;
            //Get Valori dalla sessione
            DocsPaWR.FileRequest frDoc = (DocsPaWR.FileRequest)Session["frDoc_RemotePdfStamp"];
            DocsPaWR.InfoUtente infoUt = (DocsPaWR.InfoUtente)Session["infoUt_RemotePdfStamp"];
            DocsPaWR.SchedaDocumento sd = (DocsPaWR.SchedaDocumento)Session["sd_RemotePdfStamp"];

            // Creazione dell'oggetto per l'invocazione del webService
            DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
            try
            {
                // invocazione metodo per la Stampa Segnatura Firma Automatica External Service
                //retVal = true;
                retVal = docsPaWS.RemotePdfStamp(infoUt, frDoc, sd.protocollo.segnatura);
            }
            catch (Exception e)
            {
                retVal = false;
            }

            return retVal;
        }

        //private void RefreshDocument(DocsPaWR.SchedaDocumento schedaCorrente)
        //{
        //    // Reperimento scheda documento corrente
        //    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDettaglioDocumento(this, schedaCorrente.systemId, schedaCorrente.docNumber);
        //    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);

        //    // Aggiornamento pagina visualizzata
        //    string function = "window.top.principale.iFrame_sx.document.location=window.top.principale.iFrame_sx.document.location.href;";
        //    Response.Write("<script>" + function + "</script>");
        //}

    }
}