using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace DocsPAWA.gestione.report
{
    public partial class CalendarReport : System.Web.UI.UserControl
    {
        protected System.Web.UI.HtmlControls.HtmlTable tbl_Calendario;
        public System.Web.UI.WebControls.ImageButton btn_Cal;
        public string fromUrl = "../../UserControls/DialogCalendar.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.SetUrlControls();
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
        }

        private void btn_Cal_Click(object sender, EventArgs e)
        {
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SetData", "ShowCalendar('"+this.txt_Data.ClientID+"','"+fromUrl+"');", true);
        }

        /// <summary>
        /// Reperimento clientid del campo text contenente la data 
        /// </summary>
        protected string DateTextClientId
        {
            get
            {
                return this.txt_Data.ClientID;
            }
        }

        /// <summary>
        /// Impostazione / reperimento valore della data dello usercontrol
        /// </summary>
        public string Text
        {
            get
            {
                return this.txt_Data.Text;
            }
            set
            {
                this.txt_Data.Text = value;
            }
        }

        public string campoData
        {
            get
            {
                return this.campoData;
            }
            set
            {
                this.campoData = value;
            }
        }

        public string CSS
        {
            get
            {
                return this.txt_Data.CssClass;
            }
            set
            {
                this.txt_Data.CssClass = value;
            }
        }

        public bool EnableBtnCal
        {
            get
            {
                return btn_Cal.Enabled;
            }
            set
            {
                btn_Cal.Enabled = value;
            }
        }

        /// <summary>
        /// Impostazione pulsante di default della pagina per lo user control
        /// </summary>
        /// <param name="button"></param>
        public void SetDefaultPageButton(System.Web.UI.WebControls.ImageButton button)
        {
             DocsPAWA.Utils.DefaultButton(this.Page, ref this.txt_Data, ref button);
        }

        public bool ValidateData(out string validationMessage)
        {
            ArrayList validationItems = new ArrayList();
            validationMessage = string.Empty;
            return (validationItems.Count == 0);
        }

        private void SetUrlControls()
        {
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.btn_Cal.Click += new System.Web.UI.ImageClickEventHandler(this.btn_Cal_Click);
        }
        #endregion

    }
}