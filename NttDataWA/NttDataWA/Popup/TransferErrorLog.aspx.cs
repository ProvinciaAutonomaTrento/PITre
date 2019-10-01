using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NttDataWA.Popup
{
    public partial class TransferErrorLog : System.Web.UI.Page
    {
        protected string ListaVersamentoIDANDPolicyID
        {
            get
            {
                string result = string.Empty;
                if (Session["ListaVersamentoIDANDPolicyID"] != null)
                {
                    result = Session["ListaVersamentoIDANDPolicyID"].ToString();
                }

                return result;
            }
            set
            {
                Session["ListaVersamentoIDANDPolicyID"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.InitializeLanguage();
                    this.InitializePage();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        private void InitializePage()
        {
            InitializeLanguage();
            InitializeLst();
        }

        private void InitializeLst()
        {
            List<DocsPaWR.ARCHIVE_LOG_TransferAndPolicy> _lst = UIManager.ArchiveManager.GetARCHIVE_LOG_TransferAndPolicy(ListaVersamentoIDANDPolicyID);
            //LstLogTransfer = new MultiColumnListBox();

            LstLogTransfer.DataSource = _lst.Where(x => x.ObjectType == 1 && x.ActionType.Equals("ERROR")).Select(p => p.Action);
            LstTransferPolicy.DataSource = _lst.Where(x => x.ObjectType == 2 && x.ActionType.Equals("ERROR")).Select(p => p.Action);

            LstLogTransfer.DataBind();
            LstTransferPolicy.DataBind();

        }

        protected void InitializeLanguage()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.SelectLogTransferLbl.Text = Utils.Languages.GetLabelFromCode("SelectLogTransferLbl", language);
            this.SelectTransferPolicyLbl.Text = Utils.Languages.GetLabelFromCode("SelectTransferPolicyLbl", language);
            this.BtnChiudi.Text = Utils.Languages.GetLabelFromCode("SelectLoTransferBtnChiudi", language);
        }

        protected void BtnChiudi_click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "TransferErrorLog", "closeAjaxModal('TransferErrorLog', 'up', parent);", true);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        
    }
}