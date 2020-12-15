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

namespace DocsPAWA.FascicolazioneCartacea
{
    public partial class ShowFilePage : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!this.IsPostBack)
            {
                this.RenderDocument();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderDocument()
        {
            try
            {
                DocsPaWR.FileDocumento fileDocument = this.GetFileDocument();

                if (fileDocument != null)
                {
                    Response.ContentType = fileDocument.contentType;
                    Response.AddHeader("content-disposition", "inline;filename=" + fileDocument.name);
                    Response.AddHeader("content-length", fileDocument.content.Length.ToString());
                    Response.BinaryWrite(fileDocument.content);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, new Exception("Il file richiesto non è visualizzabile"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual DocsPaWR.FileDocumento GetFileDocument()
        {
            int versionId;
            if (Int32.TryParse(this.Request.QueryString["versionId"], out versionId))
            {
                DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                return ws.FascCartaceaGetFile(UserManager.getInfoUtente(), versionId);
            }
            else
            {
                throw new ApplicationException("Parametro 'VersionId' non valido");
            }
        }
    }
}
