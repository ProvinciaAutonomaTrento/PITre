using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Document
{
    public partial class AttachmentInlineSaver : System.Web.UI.Page
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (Request.Form["value"] != null && Request.Form["value"].Length>0)
                {
                    DocsPaWR.Allegato attach = DocumentManager.GetSelectedAttachment();
                    foreach (DocsPaWR.Documento d in DocumentManager.getDocumentDetails(this.Page, attach.docNumber, attach.docNumber).documenti)
                    {
                        if (d.version.Equals(DocumentManager.getSelectedNumberVersion()))
                        {
                            d.descrizione = Request.Form["value"];
                            DocumentManager.ModifyVersion(d.descrizione);
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

    }
}