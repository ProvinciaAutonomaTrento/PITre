using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.SmartClient
{
    /// <summary>
    /// Summary description for SaveSignedHashFile.
    /// </summary>
    public class SaveSignedHashFile : Page
    {
        protected System.Web.UI.WebControls.Button Button1;
        protected string hashSigned;
        protected bool isPades;
        protected string idDocument = string.Empty;

        private void Page_Load(object sender, System.EventArgs e)
        {
            string base64content = string.Empty;
            // Put user code to initialize the page here
            try
            {
                this.startUp();
                this.ErrorPage = "";

                if (Request.QueryString["idDocumento"] != null)
                    idDocument = Request.QueryString["idDocumento"].ToString();

                hashSigned = Request.Form["signedDoc"];
                if (hashSigned == null)
                {
                    byte[] ba = Request.BinaryRead(Request.ContentLength);
                    hashSigned = System.Text.ASCIIEncoding.ASCII.GetString(ba);
                }
                isPades = false;
                
                string strPades = Request.QueryString["isPades"];
                bool.TryParse(strPades, out isPades);
                
                DocsPaWR.MassSignature massSignature = UIManager.FileManager.getSelectedMassSignature(idDocument);
                massSignature.base64Signature = hashSigned;
                massSignature.signPades = isPades;

                DocsPaWR.DocsPaWebService DocsPaWS = ProxyManager.GetWS();
                massSignature = DocsPaWS.signDocument(massSignature, UIManager.UserManager.GetInfoUser());

                FileRequest fr = massSignature.fileRequest;
                UIManager.FileManager.setSelectedFile(fr);
                DocsPaWR.SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();
                //List<DocsPaWR.Allegato> attachments = new List<DocsPaWR.Allegato>(schedaDocumento.allegati);

                if (schedaDocumento != null)
                {
                    if (schedaDocumento.allegati != null && UIManager.DocumentManager.getSelectedAttachId() != null)
                    {
                        //attachments.Add((Allegato)fr);
                        //schedaDocumento.allegati = attachments.ToArray();
                        int index = schedaDocumento.allegati.Select((item, i) => new { obj = item, index = i }).First(item => item.obj.versionId.Equals(UIManager.DocumentManager.GetSelectedAttachment().versionId)).index;
                        Allegato a = new Allegato();
                        a.applicazione = fr.applicazione;
                        a.daAggiornareFirmatari = fr.daAggiornareFirmatari;
                        a.dataInserimento = fr.dataInserimento;
                        a.descrizione = fr.descrizione;
                        a.docNumber = fr.docNumber;
                        a.docServerLoc = fr.docServerLoc;
                        a.fileName = fr.fileName;
                        a.fileSize = fr.fileSize;
                        a.firmatari = fr.firmatari;
                        a.firmato = fr.firmato;
                        a.idPeople = fr.idPeople;
                        a.path = fr.path;
                        a.subVersion = fr.version;
                        a.version = fr.version;
                        a.versionId = fr.versionId;
                        a.versionLabel = schedaDocumento.allegati[index].versionLabel;
                        a.cartaceo = fr.cartaceo;
                        a.repositoryContext = fr.repositoryContext;
                        a.TypeAttachment = 1;
                        //a.numeroPagine = (fr as Allegato).numeroPagine;
                        // modifica necessaria per FILENET (A.B.)
                        if ((fr.fNversionId != null) && (fr.fNversionId != ""))
                            a.fNversionId = fr.fNversionId;
                        schedaDocumento.allegati[index] = a;

                        UIManager.DocumentManager.setSelectedAttachId(fr.versionId);
                        UIManager.DocumentManager.setSelectedNumberVersion(a.version);
                        //schedaDocumento.allegati = UIManager.DocumentManager.AddAttachment((Allegato)fr);
                    }
                    else
                    {
                        //fr = UIManager.DocumentManager.AddVersion(fr, false);
                        schedaDocumento.documenti = UIManager.DocumentManager.addVersion(schedaDocumento.documenti, (Documento)fr);
                        UIManager.DocumentManager.setSelectedNumberVersion(fr.version);
                    }

                    UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                }
            }
            catch (Exception es)
            {
                ErrorManager.setError(this, es);
            }
        }

        private void startUp()
        {
            Response.CacheControl = "no-cache";
            Response.AddHeader("Pragma", "no-cache");
            Response.Expires = -1;
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
