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
using MText;
using NttDataWA.DocsPaWR;
using Newtonsoft.Json;

namespace NttDataWA.CheckInOutApplet
{
    /// <summary>
    /// Pagina utilizzata per effettuare il checkin del documento corrente
    /// </summary>
    public class CheckInPage : System.Web.UI.Page
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            bool isApplet = (Request.QueryString["type"] != null && Request.QueryString["type"].ToString() == "AP");
			bool isSocket = (Request.QueryString["type"] != null && Request.QueryString["type"].ToString() == "socket");

            // Discrimina tra le diverse tipologie di documento
            String file = "";
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                CheckOutStatus tempStatus = UIManager.DocumentManager.GetCheckOutDocumentStatus(UIManager.DocumentManager.GetSelectedAttachment().docNumber);
                if (tempStatus != null)
                    file = UIManager.DocumentManager.GetCheckOutDocumentStatus(UIManager.DocumentManager.GetSelectedAttachment().docNumber).DocumentLocation;
            } 
            else 
            {
                if (CheckOutAppletContext.Current != null && CheckOutAppletContext.Current.Status != null)
                    file = CheckOutAppletContext.Current.Status.DocumentLocation;
            }

            if (file != String.Empty)
            {
                if (file.StartsWith("mtext://"))
                    this.CheckInMTextDocument(file);
                else
                    this.CheckInDocument(isApplet, isSocket);
            }
            else
            {

                this.CheckInDocument(isApplet, isSocket);
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
        private void CheckInDocument(bool isApplet = false, bool isSocket = false)
        {
            byte[] content = null;




            if(isApplet)
                content = System.Convert.FromBase64String(Request.Params["strFile"]);
            else if (isSocket)
            {
                string JSONfile = Request["strFile"];
                //Stream stream=Request.InputStream;
                JSONfile = JSONfile.Replace(' ', '+');
                JSONfile = JSONfile.Trim();
                NttDataWA.Utils.FileJSON file = JsonConvert.DeserializeObject<NttDataWA.Utils.FileJSON>(JSONfile);
                content = System.Convert.FromBase64String(file.content);
            } 
            else
                content = Request.BinaryRead(Request.ContentLength);

            NttDataWA.DocsPaWR.ValidationResultInfo result = CheckInOutServices.CheckInDocument(content);

            if (!result.Value)
            {
                // Scrittura dei messaggi di errore nel checkin
                Response.Write(this.GetErrorMessage(result));
            }
            else {
                System.Web.HttpContext.Current.Session["isCheckinOrOut"] = result.Value;
                CheckOutAppletContext.Current = null;
            }
        }

        /// <summary>
        /// CheckIn del documento corrente MTEXT
        /// </summary>
        private void CheckInMTextDocument(String file)
        {
            // Salvataggio del docNumber del documento in CheckOut
            CheckOutStatus ckSt = CheckOutAppletContext.Current.Status;
            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                ckSt = UIManager.DocumentManager.GetCheckOutDocumentStatus(UIManager.DocumentManager.GetSelectedAttachment().docNumber);
            }

            String docNumber = ckSt.DocumentNumber;

            // Estrai porzione del path di interesse per M/TEXT
            file = file.Substring(8);
            
            // Accedi a MTEXT per prelevare il PDF
            MTextModelProvider mtext = models.ModelProviderFactory<MTextModelProvider>.GetInstance();

            // Preleva contenuto PDF
            Byte[] content = mtext.ExportDocument(file, "application/pdf");

            // Fai sembrare il file M/TEXT un file FDF
            ckSt.DocumentLocation = "mtext.pdf";

            // Effettua il checkin del documento
            NttDataWA.DocsPaWR.ValidationResultInfo result = CheckInOutServices.CheckInDocument(content);

            // Salva l'FQN del file M/TEXT
            NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
            ws.SetMTextFullQualifiedName(new NttDataWA.DocsPaWR.MTextDocumentInfo()
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
        private string GetErrorMessage(NttDataWA.DocsPaWR.ValidationResultInfo resultInfo)
        {
            string message = string.Empty;

            // Restituzione dei messaggi di validazione
            foreach (NttDataWA.DocsPaWR.BrokenRule brokenRule in resultInfo.BrokenRules)
            {
                if (message != string.Empty)
                    message += Environment.NewLine;

                message += brokenRule.Description;
            }

            return message;
        }
    }
}
