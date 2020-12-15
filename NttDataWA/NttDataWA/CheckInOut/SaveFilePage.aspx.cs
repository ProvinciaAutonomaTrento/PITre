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

namespace NttDataWA.CheckInOut
{
    /// <summary>
    /// Pagina necessaria per effetturare il download di un documento
    /// </summary>
    public partial class SaveFilePage : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.DownloadFile();
        }

        /// <summary>
        /// Download del file richiesto
        /// </summary>
        protected virtual void DownloadFile()
        {
            string filePath = Request.QueryString["filePath"];

            byte[] fileContent = null;
            
            // Reperimento contenuto del file, verificando se reperire
            // il file firmato o il file originale
            if (filePath.ToUpper().EndsWith(".ZIP"))
            {
                fileContent = this.GetPackage();
            }
            else if (
                filePath.ToUpper().EndsWith(".P7M") ||
                filePath.ToUpper().EndsWith(".TSD") ||
                filePath.ToUpper().EndsWith(".M7M") ||
                filePath.ToUpper().EndsWith(".TSR")
                )
                fileContent = this.GetSignedFileContent();
            else
                fileContent = this.GetFileContent();

            if (fileContent == null)
            {   
                // Il contenuto non è stato reperito
                Response.StatusCode = -1;
                Response.StatusDescription = "Il file potrebbe non essere stato acquisito";
            }
            else
            {
                Response.BinaryWrite(fileContent);
                Response.End();
            }
        }

        /// <summary>
        /// Download del contenuto del file richiesto (firmato)
        /// </summary>
        /// <returns></returns>
        protected byte[] GetSignedFileContent()
        {
            return SaveFileServices.GetSignedFileContent();
        }

        /// <summary>
        /// Download del contenuto del file richiesto
        /// </summary>
        /// <returns></returns>
        protected byte[] GetFileContent()
        {
            return SaveFileServices.GetFileContent();
        }

        protected byte[] GetPackage()
        {
            return SaveFileServices.GetPackage();
        }
    }
}
