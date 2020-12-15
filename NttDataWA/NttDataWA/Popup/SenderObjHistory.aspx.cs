using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class SenderObjHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.SenderObjHistoryBtnClose.Text = "OK";
                DocsPaWR.ElStoricoSpedizioni[] elementi = NttDataWA.UIManager.SenderManager.GetElementiStoricoSpedizione((NttDataWA.UIManager.DocumentManager.getSelectedRecord()).docNumber);

                if (elementi.Length < 1)
                {
                    tabellaStorico.InnerHtml = "Non sono presenti spedizioni per il documento selezionato";
                }
                else
                {
                    tabellaStorico.InnerHtml = "<table class=\"tbl_rounded round_onlyextreme\"><tr><th>Destinatario</th><th>Data/Ora</th><th>Mezzo</th><th>Mail Dest</th><th>Mail Mitt</th><th>Esito</th></tr>";
                    foreach (DocsPaWR.ElStoricoSpedizioni el in elementi)
                    {
                        tabellaStorico.InnerHtml += "<tr><td>" + el.Corrispondente + "</td><td>" + el.DataSpedizione + "</td><td>" + el.Mezzo + "</td><td>" + el.Mail + "</td><td>"+el.Mail_mittente+"</td><td>" + el.Esito + "</td></tr>";

                    }
                    tabellaStorico.InnerHtml += "</table>";

 
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void SenderObjHistoryBtnClose_Click(object sender, EventArgs e)
        {
            //Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('SenderObjHistory','');</script></body></html>");
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "closePopup", "parent.closeAjaxModal('SenderObjHistory','');", true);
            //Response.End();
        }
    }
}