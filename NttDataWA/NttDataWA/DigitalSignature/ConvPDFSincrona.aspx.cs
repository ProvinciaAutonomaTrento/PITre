using System;//using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.DigitalSignature
{
    public partial class ConvPDFSincrona : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                NttDataWA.DocsPaWR.FileDocumento result = null;
                bool convert = true;
                Response.Expires = -1;

                //DigitalSignature.DigitalSignManager DigitalSignatureMng = new DigitalSignature.DigitalSignManager();
                //NttDataWA.DocsPaWR.FileDocumento fileFirmato = DigitalSignatureMng.GetSignedDocument(this);
                
                FileRequest fileReq = NttDataWA.UIManager.FileManager.getSelectedFile();
                FileDocumento fileToConvert = DocumentManager.DocumentoGetFile(fileReq);

                DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString())) && Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_REQ_CONV_PDF.ToString()).Equals("1"))
                {

                    if (FileManager.GetSelectedAttachment() == null)
                        fileReq = UIManager.FileManager.getSelectedFile();
                    else
                    {
                        fileReq = FileManager.GetSelectedAttachment();
                    }

                    bool isPdf = (FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToUpper() == "PDF");

                    if (!isPdf)
                    {
                        convert = true;

                        if (UIManager.FileManager.IsEnabledSupportedFileTypes())
                        {
                            // Gabriele Melini 18-03-2014
                            // fix per schianto firma file dopo conversione
                            /*
                            if (this.FileTypes != null && this.FileTypes.Length > 0)
                            {
                                this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));
                            }
                            */
                            if(this.FileTypes == null)
                                this.FileTypes = UIManager.FileManager.GetSupportedFileTypes(Int32.Parse(UIManager.UserManager.GetInfoUser().idAmministrazione));
                            bool retVal = true;
                            

                            
                            int count = this.FileTypes.Count(r => r.FileExtension.ToLowerInvariant() == FileManager.getEstensioneIntoSignedFile(fileReq.fileName).ToLowerInvariant() &&
                                                                    r.FileTypeUsed && r.FileTypeSignature);
                            retVal = (count > 0);

                            convert = !retVal;
                        }
                    }
                }

                if (convert)
                {
                    result = docsPaWS.GeneratePDFInSyncMod(fileToConvert);
                }
                else
                {
                    result = fileToConvert;
                }

    
                //DigitalSignatureMng = null;
                
                /* Se ho un file fisico da poter convertire
                if (fileFirmato != null)
                {
                    
                    DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();
                    result = docsPaWS.GeneratePDFInSyncMod(fileFirmato);

                    /*
                    FileRequest fileReq = NttDataWA.UIManager.FileManager.getSelectedFile();
                    SchedaDocumento sched = NttDataWA.UIManager.DocumentManager.getSelectedRecord();
                    InfoUtente ute = NttDataWA.UIManager.UserManager.GetInfoUser();
                    //labelPdf label = new labelPdf();

                    
                    result = docsPaWS.DocumentoGetFileConSegnaturaUsingLC(fileReq, sched, ute, null, false);
                    //result = FileManager.getInstance(sched.systemId).DocumentoGetFileConSegnaturaUsingLC(this.Page, sched, label, fileReq);
                    //result = new DocsPaWebService().DocumentoGetFileConSegnaturaUsingLC(fileReq, sched, ute, null, false);
                    if ((result == null) || !(result.content.Length > 0)) result = docsPaWS.GeneratePDFInSyncMod(fileFirmato);
                     
                }
                */
                if (result != null)
                {
                    if (Request.QueryString["applet"] != null)
                    {
                        string base64String = System.Convert.ToBase64String(result.content, 0, result.content.Length);
                        Response.Write(base64String);
                    }
                    else
                    {
                        this.Response.ContentType = "application/pdf";
                        this.Response.AddHeader("content-length", result.content.Length.ToString());
                        this.Response.BinaryWrite(result.content);
                        Response.Flush();
                    }
                }
                else
                {
                    this.Response.StatusCode = 500;
                    this.Response.StatusDescription = "Non e' stato possibile convertire il documento in PDF lato server";
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// Tipi di file
        /// </summary>
        private SupportedFileType[] FileTypes
        {
            get
            {
                if (this.ViewState["SupportedFileType"] == null)
                    return null;
                else
                    return this.ViewState["SupportedFileType"] as SupportedFileType[];
            }
            set
            {
                if (this.ViewState["SupportedFileType"] == null)
                    this.ViewState.Add("SupportedFileType", value);
                else
                    this.ViewState["SupportedFileType"] = value;
            }
        }
    }
}
