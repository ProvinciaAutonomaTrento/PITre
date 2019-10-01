using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class transmissions_print : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["PrintTransm"]) == "D")
            {
                if (!IsPostBack && !DocumentManager.CheckRevocationAcl())
                {
                    try
                    {
                        DocsPaWR.InfoDocumento infoDoc = DocumentManager.getInfoDocumento(DocumentManager.getSelectedRecord());
                        DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
                        oggettoTrasm.infoDocumento = infoDoc;
                        DocsPaWR.FileDocumento fileRep = TrasmManager.getReportTrasm(this, oggettoTrasm);
                        if (fileRep == null) return;

                        Response.ContentType = fileRep.contentType;
                        Response.AddHeader("content-disposition", "inline;filename=" + fileRep.name);
                        Response.AddHeader("content-lenght", fileRep.content.Length.ToString());
                        Response.BinaryWrite(fileRep.content);
                        Response.Flush();
                        //Response.Close();
                        //Response.End();
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                    }
                }
            }

            if (Convert.ToString(Session["PrintTransm"]) == "F")
            {
                if (!IsPostBack && !ProjectManager.CheckRevocationAcl())
                {
                    try
                    {
                        InfoFascicolo infoFasc = ProjectManager.getInfoFascicoloDaFascicolo(ProjectManager.getProjectInSession());
                        DocsPaWR.TrasmissioneOggettoTrasm oggettoTrasm = new DocsPaWR.TrasmissioneOggettoTrasm();
                        
                        oggettoTrasm.infoFascicolo = infoFasc;

                        DocsPaWR.FileDocumento fileRep = TrasmManager.getReportTrasm(this, oggettoTrasm);
                        if (fileRep == null) return;

                        Response.ContentType = fileRep.contentType;
                        Response.AddHeader("content-disposition", "inline;filename=" + fileRep.name);
                        Response.AddHeader("content-lenght", fileRep.content.Length.ToString());
                        Response.BinaryWrite(fileRep.content);
                        Response.Flush();
                        //Response.Close();
                        //Response.End();
                    }
                    catch (System.Exception ex)
                    {
                        UIManager.AdministrationManager.DiagnosticError(ex);
                    }
                }
            }
            if (Session["PrintTransm"] == null)
            {
                string id = Session.SessionID;
                DocsPaWR.FileDocumento theDoc = FileManager.getInstance(id).getReport(this);

                Response.ContentType = theDoc.contentType;
                Response.AddHeader("content-disposition", "inline;filename=" + theDoc.name);
                Response.AddHeader("content-lenght", theDoc.content.Length.ToString());
                Response.BinaryWrite(theDoc.content);
                //Response.Flush();
                //Response.Close();
                //Response.End();
            }
        }
    }
}