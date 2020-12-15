using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VisualizzaDocumento : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            string idDocumento = this.Request.QueryString["idDocumento"];
            
            int indiceAllegato;
            Int32.TryParse(this.Request.QueryString["indiceAllegato"], out indiceAllegato);

            //GM per scaricare stampe firmate del registro di conservazione
            bool downloadAsAttachment;
            bool.TryParse(Request.QueryString["downloadAsAttachment"], out downloadAsAttachment);
            WSConservazioneLocale.InfoUtente infoUtente = (WSConservazioneLocale.InfoUtente) Session["infoutCons"];
            WSConservazioneLocale.FileDocumento fileDocumento = null;

            //GM 22-7-2013
            //gestione diversa se devo visualizzare o scaricare il documento
            if (!downloadAsAttachment)
            {
                fileDocumento = ConservazioneWA.Utils.ConservazioneManager.GetFileDocumentoNotifica(infoUtente, idDocumento, indiceAllegato);
            }
            else
            {
                fileDocumento = ConservazioneWA.Utils.ConservazioneManager.GetFileDocumentoFirmato(infoUtente, idDocumento, indiceAllegato);
            }


            if (fileDocumento != null)
            {
               
                if (!downloadAsAttachment)
                {
                    this.Response.ContentType = "application/pdf";
                    this.Response.AddHeader("Content-Disposition", "inline");
                    this.Response.BinaryWrite(fileDocumento.content);
                    
                } 
                else
                {
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "attachment; filename="+fileDocumento.name);
                    Response.BinaryWrite(fileDocumento.content);

                    Response.Flush();
                    Response.End();
                }

            }
        }
    }
}