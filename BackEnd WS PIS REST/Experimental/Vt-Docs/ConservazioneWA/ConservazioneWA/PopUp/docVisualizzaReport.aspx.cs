using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPaVO.documento;


namespace ConservazioneWA.PopUp
{
    public partial class docVisualizzaReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            WSConservazioneLocale.FileDocumento fd = (WSConservazioneLocale.FileDocumento)Session["fileReport"];
            string content = (String)Session["valoreContent"];

            if (fd != null)
            {
                if (content == "PDF")
                {
                    Response.ContentType = "application/pdf";

                    Response.AddHeader("content-disposition", "inline;filename=" + fd.name);

                    Response.AddHeader("content-length", fd.content.Length.ToString());

                    Response.BinaryWrite(fd.content);

                    Response.End();

                }

                else 
                {
                    Response.ContentType = fd.contentType;

                    Response.AddHeader("content-disposition", "inline;filename=" + fd.name);

                    Response.AddHeader("content-length", fd.content.Length.ToString());

                    Response.BinaryWrite(fd.content);

                    Response.End();
                
                }
                


            }

        }
        
    }
}