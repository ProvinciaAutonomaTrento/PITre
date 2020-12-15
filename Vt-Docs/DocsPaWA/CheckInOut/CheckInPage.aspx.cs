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
using DocsPAWA.DocsPaWR;
using MText;
using DocsPAWA.models;

namespace DocsPAWA.CheckInOut
{
    /// <summary>
    /// Pagina utilizzata per effettuare il checkin del documento corrente
    /// </summary>
    public class CheckInPage : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            // Discrimina tra le diverse tipologie di documento
            String file = "";
            if (CheckOutContext.Current != null && CheckOutContext.Current.Status != null)
                file = CheckOutContext.Current.Status.DocumentLocation;

            if (file != String.Empty)
            {
                if (file.StartsWith("mtext://"))
                    this.CheckInMTextDocument(file);
                else
                    this.CheckInDocument();
            }
            else
            {

                this.CheckInDocument();
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

        /// <summary>
        /// CheckIn del documento corrente
        /// </summary>
        private void CheckInDocument()
        {
            byte[] content = Request.BinaryRead(Request.ContentLength);

            ValidationResultInfo result = CheckInOutServices.CheckInDocument(content);

            if (!result.Value)
            {
                // Scrittura dei messaggi di errore nel checkin
                Response.Write(this.GetErrorMessage(result));
            }
        }

        /// <summary>
        /// CheckIn del documento corrente MTEXT
        /// </summary>
        private void CheckInMTextDocument(String file)
        {
            // Salvataggio del docNumber del documento in CheckOut
            String docNumber = CheckOutContext.Current.Status.DocumentNumber;

            // Estrai porzione del path di interesse per M/TEXT
            file = file.Substring(8);

            // Accedi a MTEXT per prelevare il PDF
            MTextModelProvider mtext = ModelProviderFactory<MTextModelProvider>.GetInstance();

            // Preleva contenuto PDF
            Byte[] content = mtext.ExportDocument(file, "application/pdf");

            // Fai sembrare il file M/TEXT un file FDF
            CheckOutContext.Current.Status.DocumentLocation = "mtext.pdf";

            // Effettua il checkin del documento
            ValidationResultInfo result = CheckInOutServices.CheckInDocument(content);

            // Salva l'FQN del file M/TEXT
            DocsPaWebService ws = new DocsPaWebService();
            ws.SetMTextFullQualifiedName(new MTextDocumentInfo()
            {
                DocumentDocNumber = docNumber,
                FullQualifiedName = file
            });

            if (!result.Value)
            {
                // Scrittura dei messaggi di errore nel checkin
                Response.Write(this.GetErrorMessage(result));
            }
        }

        /// <summary>
        /// Reperimento dell'eventuale messaggio di errore proveniente dal checkout
        /// </summary>
        /// <param name="resultInfo"></param>
        /// <returns></returns>
        private string GetErrorMessage(ValidationResultInfo resultInfo)
        {
            string message = string.Empty;

            // Restituzione dei messaggi di validazione
            foreach (BrokenRule brokenRule in resultInfo.BrokenRules)
            {
                if (message != string.Empty)
                    message += Environment.NewLine;

                message += brokenRule.Description;
            }

            return message;
        }
    }
}
