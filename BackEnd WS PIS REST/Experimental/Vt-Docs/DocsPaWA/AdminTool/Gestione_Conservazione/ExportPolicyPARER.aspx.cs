using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class ExportPolicyPARER : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string tipoReport = Request.QueryString["tipo"].ToString();
            if (!string.IsNullOrEmpty(tipoReport))
            {
                if (tipoReport.Equals("DOC"))
                    this.titlePage.Text = "Esporta dettagli Policy documenti";
            }
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            // formato report
            string formato = this.ddl_format.SelectedValue;
            string tipoReport = Request.QueryString["tipo"].ToString();

            string[] listaID = (string[])this.itemsToExport.ToArray(typeof(string));

            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();

            DocsPAWA.DocsPaWR.FileDocumento doc = new FileDocumento();
            doc = ws.ExportPolicyPARER(listaID, formato, tipoReport);

            if (doc != null)
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + doc.name);
                Response.BinaryWrite(doc.content);
                Response.Flush();
                Response.End();
            }
            else
            {
                
            }
        }

        protected ArrayList itemsToExport
        {
            get
            {
                return HttpContext.Current.Session["itemsToExport"] as ArrayList;
            }
            set
            {
                HttpContext.Current.Session["itemsToExport"] = value;
            }
        }
    }
}