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
    public partial class ViewNote : System.Web.UI.Page
    {
        #region Property

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

        public string TxtNoteViewer
        {
            get
            {
                string result = string.Empty;
                if (HttpContext.Current.Session["TxtNoteViewer"] != null)
                {
                    result = HttpContext.Current.Session["TxtNoteViewer"].ToString();
                }
                return result;
            }
            set
            {
                HttpContext.Current.Session["TxtNoteViewer"] = value;
            }
        }

        #endregion

        #region Constat
        private const string NOTE_GENERALI = "g";
        private const string NOTE_INDIVIDUALI = "i";
        private const string READONLY = "true";
        #endregion

        #region Initialize
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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

            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_TRASM.ToString())))
            {
                this.MaxLenghtObject = int.Parse(Utils.InitConfigurationKeys.GetValue(UIManager.UserManager.GetInfoUser().idAmministrazione, DBKeys.FE_MAX_LENGTH_DESC_TRASM.ToString()));
            }

            if (Request.QueryString["tiponota"] != null && Request.QueryString["tiponota"].Equals(NOTE_INDIVIDUALI))
            {
                this.MaxLenghtObject = 250;
            }

            this.InitializeLabel();
            this.InitializeObjectValue();

            if (Request.QueryString["readonly"] != null)
            {
                if(Request.QueryString["readonly"].ToString().Equals(READONLY))
                {
                    this.ViewNoteBtnOk.Visible = false;
                    this.TxtViewNote.ReadOnly = true;
                    this.ViewNoteBtnClose.Focus();
                }
            }
        }

        protected void InitializeObjectValue()
        {
            this.TxtViewNote.Text = TxtNoteViewer;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeLengthCharacters", "charsLeft('TxtViewNote','" + this.MaxLenghtObject + "','Note');", true);
        }

        protected void InitializeLabel()
        {
            string language = UIManager.UserManager.GetUserLanguage();
            this.ViewNoteBtnOk.Text = Utils.Languages.GetLabelFromCode("ViewNoteBtnOk", language);
            this.ViewNoteBtnClose.Text = Utils.Languages.GetLabelFromCode("ViewNoteBtnClose", language);
            this.ViewNoteLitNoteChAv.Text = Utils.Languages.GetLabelFromCode("ViewNoteLitNoteChAv", language);
        }

        #endregion

        #region Event Button
        protected void ViewNoteBtnOk_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "function", "reallowOp();", true);
            this.TxtNoteViewer = this.TxtViewNote.Text;
            CloseMask(true);
        }

        protected void ViewNoteBtnClose_Click(object sender, EventArgs e)
        {
            CloseMask(false);
        }

        private void CloseMask(bool withResult)
        {
            string result = string.Empty;
            if (withResult)
                result = "up";

            string popup = "ViewNoteGen";
            if (Request.QueryString["tiponota"] != null)
            {
                if (Request.QueryString["tiponota"].ToString().Equals(NOTE_INDIVIDUALI))
                {
                    popup = "ViewNoteInd";
                }
            }
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "closeAJM", "parent.closeAjaxModal('" + popup + "','" + result + "');", true);
        }
        #endregion
    }
}