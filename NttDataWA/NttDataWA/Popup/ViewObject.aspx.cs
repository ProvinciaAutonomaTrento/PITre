using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDatalLibrary;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.Popup
{
    public partial class ViewObject : System.Web.UI.Page
    {

        #region property

        private string ReturnValue
        {
            set
            {
                HttpContext.Current.Session["ReturnValuePopup"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (!IsPostBack)
                {
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void InitializePage()
        {

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_OGGETTO.ToString()));
            }
            this.InitializeLabel();
            this.InitializeObjectValue();
            this.VerifyEnableEdit();
            this.VisibleRoleFunctions();
        }

        private void VisibleRoleFunctions()
        {
            if (!UserManager.IsAuthorizedFunctions("DO_PROT_OG_MODIFICA"))
            {
                this.ViewObjectBtnOk.Visible = false;
            }

            if (DocumentManager.getSelectedRecord() != null && !string.IsNullOrEmpty(DocumentManager.getSelectedRecord().systemId) && DocumentManager.getSelectedRecord().tipoProto.Equals("G"))
            {
                this.ViewObjectBtnOk.Visible = true;
            }
        }
        protected void VerifyEnableEdit()
        {
            
            SchedaDocumento doc = UIManager.DocumentManager.getSelectedRecord();
            if (doc != null && doc.tipoProto != null && doc.protocollo!=null && doc.protocollo.protocolloAnnullato != null)
            {
                this.TxtViewObject.ReadOnly = true;
                this.ViewObjectBtnOk.Enabled = false;
            }

            if (doc != null && !string.IsNullOrEmpty(doc.systemId) && !string.IsNullOrEmpty(doc.accessRights) && Convert.ToInt32(doc.accessRights) < Convert.ToInt32(HMdiritti.HMdiritti_Write))
            {
                this.TxtViewObject.ReadOnly = true;
                this.ViewObjectBtnOk.Enabled = false;
            }

            //Se consolidato disabilito l'edit
            if (doc != null && !string.IsNullOrEmpty(doc.systemId) && doc.ConsolidationState!=null)
            {
                if (doc.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                {
                    this.TxtViewObject.ReadOnly = true;
                    this.ViewObjectBtnOk.Enabled = false;
                }
            }
        }

        protected void InitializeObjectValue()
        {
            this.TxtViewObject.Text = TxtObjectViewer;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeLengthCharacters", "charsLeft('TxtViewObject','" + this.MaxLenghtObject + "','Descrizione oggetto');", true);
        }

        protected void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ViewObjectBtnOk.Text = Utils.Languages.GetLabelFromCode("ViewObjectBtnOk", language);
            this.ViewObjectBtnClose.Text = Utils.Languages.GetLabelFromCode("ViewObjectBtnClose", language);
            this.ViewObjectLitObjectChAv.Text = Utils.Languages.GetLabelFromCode("ViewObjectLitObjectChAv", language);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.TxtObjectViewer = this.TxtViewObject.Text;
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('ViewObject', 'up');</script></body></html>");
            Response.End();
        }

        protected void ObjectBtnChiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<html><body><script type=\"text/javascript\">parent.closeAjaxModal('ViewObject', '');</script></body></html>");
            Response.End();
        }

        protected int MaxLenghtObject
        {
            get
            {
                int result = 2000;
                if (HttpContext.Current.Session["maxLenghtObject"] != null)
                {
                    result = int.Parse(HttpContext.Current.Session["maxLenghtObject"].ToString());
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["maxLenghtObject"] = value;
            }
        }

        public string TxtObjectViewer
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["txtObjectViewer"] != null)
                {
                    result = HttpContext.Current.Session["txtObjectViewer"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["txtObjectViewer"] = value;
            }
        }
    }
}