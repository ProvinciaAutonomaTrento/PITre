using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;


namespace ConservazioneWA.UserControl
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        protected System.Web.UI.HtmlControls.HtmlTable tbl_Calendario;
        public System.Web.UI.WebControls.ImageButton btn_Cal; 
        public string fromUrl = string.Empty;

        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.SetUrlControls();
            }
            if (this.PAGINA_CHIAMANTE == "generaSupporti" || this.PAGINA_CHIAMANTE == "VerificaSupporto" || this.PAGINA_CHIAMANTE == "CampiProfilati" || this.PAGINA_CHIAMANTE == "ISTANZEESIBIZIONE" || this.PAGINA_CHIAMANTE == "RICERCAISTANZECONS_ESIBIZIONE" || this.PAGINA_CHIAMANTE == "RICERCADOCUMENTICONS_ESIBIZIONE" || this.PAGINA_CHIAMANTE == "RICERCAFASCICOLICONS_ESIBIZIONE")
                fromUrl = "../UserControl/DialogCalendar.aspx";
            else
                fromUrl = "UserControl/DialogCalendar.aspx";
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
        }

        private void btn_Cal_Click(object sender, EventArgs e)
        {

            //string jsFunction = "var control=ShowCalendar('";
            //jsFunction += this.txt_Data.ClientID + "');";
            //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SetData", "<script language='javascript'>" + jsFunction + "</script>");
            if (this.PAGINA_CHIAMANTE == "RICERCAISTANZE" || this.PAGINA_CHIAMANTE == "ISTANZEESIBIZIONE" || this.PAGINA_CHIAMANTE == "RICERCAISTANZECONS_ESIBIZIONE" || this.PAGINA_CHIAMANTE == "RICERCADOCUMENTICONS_ESIBIZIONE" || this.PAGINA_CHIAMANTE == "RICERCAFASCICOLICONS_ESIBIZIONE")
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SetData", "ShowCalendar('" + this.txt_Data.ClientID + "','" + fromUrl + "');", true);
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SetData", "ShowCalendar('" + this.txt_Data.ClientID + "','" + fromUrl + "');", true);
            }
           
        }

        /// <summary>
        /// Reperimento clientid del campo text contenente la data 
        /// </summary>
        public string DateTextClientId
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
                //this.txt_Data.Text = value;
                DateTime date;

                if (DateTime.TryParse(value, out date))
                {
                    this.txt_Data.Text = date.ToString("dd/MM/yyyy");
                }
                else
                {
                    this.txt_Data.Text = string.Empty;
                }
            }
        }

        //public string FromUrl
        //{
        //    get
        //    {
        //        return this.fromUrl;
        //    }
        //    set
        //    {
        //        this.fromUrl = value;
        //    }
        //}
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

        public string PAGINA_CHIAMANTE
        {
            get
            {
                return this.GetStateValue("PAGINA_CHIAMANTE");
            }
            set
            {
                this.SetStateValue("PAGINA_CHIAMANTE", value);
            }
        }

        /// <summary>
        /// Impostazione pulsante di default della pagina per lo user control
        /// </summary>
        /// <param name="button"></param>
        public void SetDefaultPageButton(System.Web.UI.WebControls.ImageButton button)
        {
           this.DefaultButton(this.Page, ref this.txt_Data, ref button);
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

        protected string GetStateValue(string key)
        {
            if (this.ViewState[key] != null)
                return this.ViewState[key].ToString();
            else
                return string.Empty;
        }

        protected void SetStateValue(string key, string obj)
        {
            this.ViewState[key] = obj;
        }

        public enum VisibleTimeModeEnum
        {
            Nothing,
            Hours,
            Minutes,
            Seconds
        }

        /// <summary>
        /// 
        /// </summary>
        public VisibleTimeModeEnum VisibleTimeMode
        {
            get
            {
                if (this.ViewState["VisibleTimeMode"] != null)
                    return (VisibleTimeModeEnum)this.ViewState["VisibleTimeMode"];
                else
                    return VisibleTimeModeEnum.Nothing;
            }
            set
            {
                this.ViewState["VisibleTimeMode"] = value;
            }
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

        public void DefaultButton(System.Web.UI.Page Page, ref DocsPaWebCtrlLibrary.DateMask objTextControl, ref ImageButton objDefaultButton)
        {
            try
            {
                System.Text.StringBuilder sScript = new System.Text.StringBuilder();

                sScript.Append(Environment.NewLine + "<SCRIPT language=\"javascript\">" + Environment.NewLine);
                sScript.Append("function fnTrapKD(btn) {" + Environment.NewLine);
                sScript.Append(" if (document.all){" + Environment.NewLine);
                sScript.Append("   if (event.keyCode == 13)" + Environment.NewLine);
                sScript.Append("   { " + Environment.NewLine);
                sScript.Append("     event.returnValue=false;" + Environment.NewLine);
                sScript.Append("     event.cancel = true;" + Environment.NewLine);
                sScript.Append("     btn.click();" + Environment.NewLine);
                sScript.Append("   } " + Environment.NewLine);
                sScript.Append(" } " + Environment.NewLine);
                sScript.Append("}" + Environment.NewLine);
                sScript.Append("</SCRIPT>" + Environment.NewLine);

                objTextControl.Attributes.Add("onkeydown", "try { fnTrapKD(document.all." + objDefaultButton.ClientID + "); } catch (e) {}");

                //Page.RegisterStartupScript("ForceDefaultToScript", sScript.ToString());
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "ForceDefaultToScript", sScript.ToString());
            }
            catch
            {
            }
        }
        #endregion
    }
}
