using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace DocsPAWA.AdminTool.Gestione_ProfDinamica
{
    public partial class VisualizzatoreModelli : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            string path = (String)Session["ModelloDaVisualizzare"];
            if (path != null)
                this.ShowDocument();

        }

        private void ShowDocument()
        {
            string path = (String)Session["ModelloDaVisualizzare"];
            //ChiamataWebservice
            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            byte[] file = ws.GetFileFromPath(path);
            //Session.Remove("ModelloDaVisualizzare");
            //WriteResponse
            string filename = System.IO.Path.GetFileName(path);
            
            Response.Buffer = true;
            Response.ClearHeaders();
            Response.ContentType = "application/rtf";

            Response.AppendHeader("content-disposition", "inline;filename=" + filename);
            Response.AddHeader("content-length", file.Length.ToString());

            Response.BinaryWrite(file);
            //Response.Close();
            Response.Flush();
            Response.End();


        }
         
    }
}