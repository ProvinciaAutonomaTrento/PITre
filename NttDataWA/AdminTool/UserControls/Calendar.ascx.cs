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

namespace SAAdminTool.UserControls
{
    public partial class Calendar : System.Web.UI.UserControl
    {
        protected System.Web.UI.HtmlControls.HtmlTable tbl_Calendario;
        public System.Web.UI.WebControls.ImageButton btn_Cal;
        public string paginaChiamante;
        public string fromUrl = "../UserControls/DialogCalendar.aspx";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                //this.btn_Cal.OnClientClick = "return ShowCalendar('" + this.txt_Data.ClientID + "','" + fromUrl + "');";
                this.btn_Cal.OnClientClick = "return ShowCalendar('" + this.txt_Data.ClientID + "','" + this.txt_hours.ClientID + "','" + this.txt_minutes.ClientID + "','" + this.txt_seconds.ClientID + "','" + fromUrl + "');";
                this.SetUrlControls();
            }
        
            if (this.paginaChiamante == "prospetti")
                fromUrl = "../../UserControls/DialogCalendar.aspx";

            if (paginaChiamante == "NuovoFascicolo")
                this.btn_Cal.OnClientClick = "return ShowCalendar('" + this.txt_Data.ClientID + "','" + this.txt_hours.ClientID + "','" + this.txt_minutes.ClientID + "','" + this.txt_seconds.ClientID + "','" + fromUrl + "');";

            if (this.paginaChiamante == "esportalog")
            {
                fromUrl = "../../UserControls/DialogCalendar.aspx";
                this.txt_Data.AutoPostBack = true;
            }
            
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            this.RefreshControlsEnabled();
        }

        private void btn_Cal_Click(object sender, EventArgs e)
        {

            //if(paginaChiamante == "NuovoFascicolo")
            //    //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SetData", "ShowCalendar('" + this.txt_Data.ClientID + "','" + fromUrl + "');", true);
            //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "SetData", "ShowCalendar('" + this.txt_Data.ClientID + "','" + this.txt_hours.ClientID + "','" + this.txt_minutes.ClientID + "','" + this.txt_seconds.ClientID + "','" + fromUrl + "');", true);
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
                string value = string.Empty;

                if (this.VisibleTimeMode == VisibleTimeModeEnum.Nothing && !string.IsNullOrEmpty(txt_Data.Text))
                    value = this.txt_Data.Text;
                else
                {
                    if (!string.IsNullOrEmpty(this.txt_Data.Text))
                    {
                        if (this.VisibleTimeMode == VisibleTimeModeEnum.Hours)
                        {
                            value = string.Format("{0} {1}", 
                                            this.txt_Data.Text, 
                                            this.GetTimePart(this.txt_hours.Text));
                        }

                        if (this.VisibleTimeMode == VisibleTimeModeEnum.Minutes)
                        {
                            value = string.Format("{0} {1}:{2}", this.txt_Data.Text, this.GetTimePart(this.txt_hours.Text), this.GetTimePart(this.txt_minutes.Text));
                        }

                        if (this.VisibleTimeMode == VisibleTimeModeEnum.Seconds)
                        {
                            value = string.Format("{0} {1}:{2}:{3}", this.txt_Data.Text, this.GetTimePart(this.txt_hours.Text), this.GetTimePart(this.txt_minutes.Text), this.GetTimePart(this.txt_seconds.Text));
                        }
                        
                        //this.GetTimePart(this.txt_minutes.Text);
                        //this.GetTimePart(this.txt_seconds.Text);
                    }
                }


                //else if (this.VisibleTimeMode == VisibleTimeModeEnum.Hours && !string.IsNullOrEmpty(txt_Data.Text) && !string.IsNullOrEmpty(txt_hours.Text))
                //    value = string.Format("{0} {1}", this.txt_Data.Text, this.txt_hours.Text);
                //else if (this.VisibleTimeMode == VisibleTimeModeEnum.Minutes && !string.IsNullOrEmpty(txt_Data.Text) && !string.IsNullOrEmpty(txt_hours.Text) && !string.IsNullOrEmpty(txt_minutes.Text))
                //    value = string.Format("{0} {1}:{2}", this.txt_Data.Text, this.txt_hours.Text, this.txt_minutes.Text);
                //else if (this.VisibleTimeMode == VisibleTimeModeEnum.Seconds && !string.IsNullOrEmpty(txt_Data.Text) && !string.IsNullOrEmpty(txt_hours.Text) && !string.IsNullOrEmpty(txt_minutes.Text) && !string.IsNullOrEmpty(txt_seconds.Text))
                //    value = string.Format("{0} {1}:{2}:{3}", this.txt_Data.Text, this.txt_hours.Text, this.txt_minutes.Text, this.txt_seconds.Text);

                return value;
            }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    //this.txt_Data.Text = string.Format("{0}/{1}/{2}", date.Day, date.Month, date.Year);
                    this.txt_Data.Text = date.ToString("dd/MM/yyyy");
                    this.txt_hours.Text = date.ToString("HH");
                    this.txt_minutes.Text = date.ToString("mm");
                    this.txt_seconds.Text = date.ToString("ss");
                }
                else
                {
                    this.txt_Data.Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timePart"></param>
        /// <returns></returns>
        protected string GetTimePart(string timePart)
        {
            if (string.IsNullOrEmpty(timePart))
                return "00";
            else
                return timePart;
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

        public System.Drawing.Color BackColor
        {
            set
            {
                this.txt_Data.BackColor = value;
                this.txt_Data.BackColor = value;
                this.txt_hours.BackColor = value;
                this.txt_minutes.BackColor = value;
                this.txt_seconds.BackColor = value;
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
                this.txt_hours.CssClass = value;
                this.txt_minutes.CssClass = value;
                this.txt_seconds.CssClass = value;
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


        public string PaginaChiamante
        {
            get
            {
                return this.paginaChiamante;
            }
            set
            {
                this.paginaChiamante = value;
            }
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

        /// <summary>
        /// 
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return this.txt_Data.ReadOnly;
            }
            set
            {
                this.txt_Data.ReadOnly = value;
                this.txt_hours.ReadOnly = value;
                this.txt_minutes.ReadOnly = value;
                this.txt_seconds.ReadOnly = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.txt_Data.Visible;
            }
            set
            {
                this.txt_Data.Visible = value;
                this.btn_Cal.Visible = value;
            }
        }

        /// <summary>
        /// Impostazione pulsante di default della pagina per lo user control
        /// </summary>
        /// <param name="button"></param>
        public void SetDefaultPageButton(System.Web.UI.WebControls.ImageButton button)
        {
             SAAdminTool.Utils.DefaultButton(this.Page, ref this.txt_Data, ref button);
        }

        public bool ValidateData(out string validationMessage)
        {
            ArrayList validationItems = new ArrayList();
            validationMessage = string.Empty;
            return (validationItems.Count == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RefreshControlsEnabled()
        {
            if (this.VisibleTimeMode == VisibleTimeModeEnum.Nothing)
            {
                //lbl_hours.Visible
                lbl_minutes.Visible = false;
                lbl_seconds.Visible = false;
                txt_hours.Visible = false;
                txt_minutes.Visible = false;
                txt_seconds.Visible = false;
            }
            else if (this.VisibleTimeMode == VisibleTimeModeEnum.Hours)
            {
                //lbl_hours.Visible
                lbl_minutes.Visible = false;
                lbl_seconds.Visible = false;
                txt_hours.Visible = true;
                txt_minutes.Visible = false;
                txt_seconds.Visible = false;
            }
            else if (this.VisibleTimeMode == VisibleTimeModeEnum.Minutes)
            {
                //lbl_hours.Visible
                lbl_minutes.Visible = true;
                lbl_seconds.Visible = false;
                txt_hours.Visible = true;
                txt_minutes.Visible = true;
                txt_seconds.Visible = false;
            }
            else if (this.VisibleTimeMode == VisibleTimeModeEnum.Seconds)
            {
                //lbl_hours.Visible
                lbl_minutes.Visible = true;
                lbl_seconds.Visible = true;
                txt_hours.Visible = true;
                txt_minutes.Visible = true;
                txt_seconds.Visible = true;
            }
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

        public enum VisibleTimeModeEnum
        {
            Nothing,
            Hours,
            Minutes,
            Seconds
        }

    }
}
