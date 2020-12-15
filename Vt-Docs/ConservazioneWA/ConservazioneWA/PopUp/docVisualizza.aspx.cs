using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ConservazioneWA.PopUp
{
    public partial class docVisualizza : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!Page.IsPostBack)
            {
                
                DocsPaWR.FileDocumento theDoc = new DocsPaWR.FileDocumento();

                bool localStore = false;
                string locale = this.Request.QueryString["locale"];

                if (String.IsNullOrEmpty(locale))
                    locale = "false";

                Boolean.TryParse(locale, out localStore);


                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                string file = this.Request.QueryString["file"];
                string idConservazione = this.Request.QueryString["idC"];
                string filetype= this.Request.QueryString["ext"];
                string[] uniSincroItems = file.Split('§');

                string path = uniSincroItems[2];
                string formato = uniSincroItems[0];
                if (formato == "image/jpg"||formato=="imagejpg") formato = "image/jpeg";

                Response.Clear();
                string[] formatsToDownload ={   "application/msword",
                                            "applicationmsword",
                                            "application/vnd.ms-excel",
                                            "applicationvnd.ms-excel",
                                            "application/vnd.ms-powerpoint",
                                            "applicationvnd.ms-powerpoint",
                                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                            "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
                                            "application/vnd.openxmlformats-officedocument.presentationml.template",
                                            "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
                                            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                                            "application/vnd.openxmlformats-officedocument.presentationml.slide",
                                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                                            "application/vnd.oasis.opendocument.text",
                                            "application/vnd.oasis.opendocument.spreadsheet",
                                            "application/vnd.oasis.opendocument.presentation"};

                string[] extToDownload = {".doc",
                                         ".docx",
                                         ".ppt",
                                         ".pptx",
                                         ".xls",
                                         ".xlsx",
                                         ".odt",
                                         ".odp",
                                         ".ods",
                                         ".zip"};
                

                byte[] bincontent = null;

                if (formato.Contains("pkcs7-mime"))
                {
                    Response.AddHeader("content-disposition", "inline");
                    WSConservazioneLocale.FileDocumento fd = ConservazioneWA.Utils.ConservazioneManager.sbustaFileFirmato(idConservazione, path, localStore);
                    if (((IList<string>)formatsToDownload).Contains(fd.contentType) || string.IsNullOrEmpty(fd.contentType))
                    {
                        Response.ContentType = "text/html";
                        Response.Write(string.Format("<a href=\"documentDL.aspx?file={0}&idC={1}&ext={2}&locale={3}\">Per visualizzare il file clicca qui.</a>", file, idConservazione, filetype, localStore.ToString().ToLower()));
                        //Response.Write(string.Format("File non apribile nella finestra di visualizzazione. <br /><br /><a href=\"documentDL.aspx?file={0}&idC={1}&ext={2}&locale={3}\">Cliccare qui per scaricare il file.</a>", file, idConservazione, filetype,localStore.ToString().ToLower()));

                    }
                    else
                    {
                        Response.ContentType = fd.contentType;
                        bincontent = fd.content;
                        Response.BinaryWrite(bincontent);
                    }
                    
                }
                else if (((IList<string>)formatsToDownload).Contains(formato)||((IList<string>)extToDownload).Contains(filetype.ToLower()))
                {
                    //Response.ContentType = "application/octet-stream";
                    //Response.AddHeader("content-disposition", "attachment;filename=" + uniSincroItems[1] + filetype);
                    //bincontent = ConservazioneWA.Utils.ConservazioneManager.getFileFromStore(infoUtente, idConservazione, path);
                    //Response.AddHeader("content-length", bincontent.Length.ToString());
                    //Response.BinaryWrite(bincontent);

                    Response.ContentType = "text/html";
                    Response.Write(string.Format("<a href=\"documentDL.aspx?file={0}&idC={1}&ext={2}&locale={3}\">Per visualizzare il file clicca qui.</a>", file, idConservazione, filetype, localStore.ToString().ToLower()));
                    //Response.Write(string.Format("File non apribile nella finestra di visualizzazione. <br /><br /><a href=\"documentDL.aspx?file={0}&idC={1}&ext={2}&locale={3}\">Cliccare qui per scaricare il file.</a>", file, idConservazione, filetype, localStore.ToString().ToLower()));
                    
                    
                }
                else
                {
                    Response.AddHeader("content-disposition", "inline;filename=" + uniSincroItems[1]+filetype);
                    try
                    {
                        bincontent = ConservazioneWA.Utils.ConservazioneManager.getFileFromStore(infoUtente, idConservazione, path, localStore);
                    }
                    catch
                    {
                        bincontent = ConservazioneWA.Utils.ConservazioneManager.getFileFromStore(infoUtente, idConservazione, path, false);
                    }
                    Response.ContentType = formato;
                    Response.AddHeader("content-length", bincontent.Length.ToString());
                    Response.BinaryWrite(bincontent);
                    
                }

                

                //Response.TransmitFile(path);
                Response.Flush();
                Response.End();
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

    }
}