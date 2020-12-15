using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.documento
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VisualizzaStampaRicevutaPdf : System.Web.UI.Page
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;

            this.PerformActionStampaRicevutaPdf();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void PerformActionStampaRicevutaPdf()
        {
            DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();



            DocsPaWR.FileDocumento content = ws.StampaRicevutaProtocolloPdf(UserManager.getInfoUtente(), this.IdDocument);
            
            if (content != null)
            {

                System.IO.MemoryStream memStream = new System.IO.MemoryStream(content.content);
                //Response.Clear();
                //Response.ContentType = "application/pdf";
                //Response.AddHeader("content-disposition", "inline;filename=Ricevuta.pdf");
                //Response.AddHeader("content-length", content.content.Length.ToString());
                //Response.BinaryWrite(content.content);
                //Response.Flush();
                //Response.End();

                Response.AppendHeader("content-disposition", "inline;filename=Ricevuta.pdf");
               Response.AppendHeader("Content-Length", memStream.Length.ToString()) ; 
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(memStream.ToArray()) ;
                Response.Flush();
                memStream.Close();



               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdDocument
        {
            get
            {
                return this.Request.QueryString["IdDocument"];
            }
        }
    }
}