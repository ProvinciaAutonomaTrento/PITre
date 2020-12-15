using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;

namespace NttDataWA.Popup
{
    public partial class HierarchyVisibilityTransmission : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.InitializePage();
                this.InitiliazeLanguage();
                if (UIManager.DocumentManager.getSelectedRecord() != null)
                {
                    SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
                    doc.eredita = "0";
                    UIManager.DocumentManager.setSelectedRecord(doc);
                }
            }
        }

        private void InitializePage()
        {

        }

        private void InitiliazeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.HierarchyVisibilityMessageBtnNo.Text = "No";
            this.HierarchyVisibilityMessageBtnYes.Text = "Si";
            this.HierarchyVisibilityMessage.Text = "Attenzione si sta trasmettendo un documento o fascicolo privato con una ragione che prevede l'estensione gerarchica della visibilità. Vuoi bloccare l'estensione gerarchica della visibilità?";
        }

        protected void HierarchyVisibilityMessageBtnNo_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('HierarchyVisibility', '');} else {parent.closeAjaxModal('HierarchyVisibility', '');};", true);
        }

        protected void HierarchyVisibilityMessageBtnYes_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "closeMask", "if (parent.fra_main) {parent.fra_main.closeAjaxModal('HierarchyVisibility', 'up');} else {parent.closeAjaxModal('HierarchyVisibility', 'up');};", true);
        }

    }
}