using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.PopUp
{
    public partial class documentDL : System.Web.UI.Page
    {
          protected void Page_Load(object sender, EventArgs e)
        {

            string locale = this.Request.QueryString["locale"];
            bool localStore = false;
   
            if (String.IsNullOrEmpty(locale))
                locale = "false";

            Boolean.TryParse(locale, out localStore);

            WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
            string file = this.Request.QueryString["file"];
            string idConservazione = this.Request.QueryString["idC"];
            string filetype = this.Request.QueryString["ext"];
            string[] uniSincroItems = file.Split('§');

            string path = uniSincroItems[2];
            string formato = uniSincroItems[0];
            Response.Clear();
            byte[] bincontent = null;
            if (formato.Contains("pkcs7-mime"))
            {
                
                WSConservazioneLocale.FileDocumento fd = ConservazioneWA.Utils.ConservazioneManager.sbustaFileFirmato(idConservazione, path,localStore );
                Response.AddHeader("content-disposition", "inline;filename="+fd.name);
                Response.ContentType = !string.IsNullOrEmpty(fd.contentType) ? fd.contentType : "application/octet-stream";
                bincontent = fd.content;
                Response.AddHeader("content-length", bincontent.Length.ToString());
                Response.BinaryWrite(bincontent);
            }
            else
            {

                Response.ContentType = formato;
                Response.AddHeader("content-disposition", "inline;filename=" + uniSincroItems[1] + filetype);
                bincontent = ConservazioneWA.Utils.ConservazioneManager.getFileFromStore(infoUtente, idConservazione, path,localStore );
                Response.AddHeader("content-length", bincontent.Length.ToString());
                Response.BinaryWrite(bincontent);
                Response.Flush();
            }
            //Response.Flush();
            Response.End();
        }
    }
}