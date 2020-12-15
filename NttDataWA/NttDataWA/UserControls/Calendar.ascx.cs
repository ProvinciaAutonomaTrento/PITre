using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace NttDataWA.UserControls
{
    public partial class Calendar : System.Web.UI.UserControl
    {

        public bool sReadonly;
        protected void Page_Load(object sender, EventArgs e)
        {
            try {
                if (((ScriptManager)Page.Master.FindControl("ScriptManager1")).IsInAsyncPostBack)
                {
                    this.UpCalendarHours.Update();
                }

                if (ReadOnly)
                {
                    this.TxtDataCalendar.ReadOnly = true;
                    this.TxtHours.ReadOnly = true;
                    this.TxtMinute.ReadOnly = true;
                    this.TxtSeconds.ReadOnly = true;
                    this.UpCalendarHours.Update();
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        protected void TxtDataCalendar_TextChanged(object sender, EventArgs e)
        {
            try {
                if (!string.IsNullOrEmpty(this.TxtDataCalendar.Text))
                {
                    if (this.VisibleTimeMode == VisibleTimeModeEnum.Nothing)
                    {

                    }
                    else if (this.VisibleTimeMode == VisibleTimeModeEnum.Hours)
                    {
                        if (string.IsNullOrEmpty(this.TxtHours.Text))
                        {
                            this.TxtHours.Text = "09";
                        }
                    }
                    else if (this.VisibleTimeMode == VisibleTimeModeEnum.Minutes)
                    {
                        if (string.IsNullOrEmpty(this.TxtHours.Text))
                        {
                            this.TxtHours.Text = "09";
                        }

                        if (string.IsNullOrEmpty(this.TxtMinute.Text))
                        {
                            this.TxtMinute.Text = "00";
                        }

                    }
                    else if (this.VisibleTimeMode == VisibleTimeModeEnum.Seconds)
                    {
                        if (string.IsNullOrEmpty(this.TxtHours.Text))
                        {
                            this.TxtHours.Text = "09";
                        }

                        if (string.IsNullOrEmpty(this.TxtMinute.Text))
                        {
                            this.TxtMinute.Text = "00";
                        }

                        if (string.IsNullOrEmpty(this.TxtSeconds.Text))
                        {
                            this.TxtSeconds.Text = "00";
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

        /// <summary>
        /// 
        /// </summary>
        public void SetEnableTimeMode()
        {
            if (this.VisibleTimeMode == VisibleTimeModeEnum.Nothing)
            {
                //lbl_hours.Visible
                this.lbl_minutes.Visible = false;
                this.lbl_seconds.Visible = false;
                this.TxtHours.Visible = false;
                this.TxtMinute.Visible = false;
                this.TxtSeconds.Visible = false;
            }
            else if (this.VisibleTimeMode == VisibleTimeModeEnum.Hours)
            {
                //lbl_hours.Visible
                this.lbl_minutes.Visible = false;
                this.lbl_seconds.Visible = false;
                this.TxtHours.Visible = true;
                this.TxtMinute.Visible = false;
                this.TxtSeconds.Visible = false;
            }
            else if (this.VisibleTimeMode == VisibleTimeModeEnum.Minutes)
            {
                //lbl_hours.Visible
                this.lbl_minutes.Visible = true;
                this.lbl_seconds.Visible = false;
                this.TxtHours.Visible = true;
                this.TxtMinute.Visible = true;
                this.TxtSeconds.Visible = false;

            }
            else if (this.VisibleTimeMode == VisibleTimeModeEnum.Seconds)
            {
                //lbl_hours.Visible
                this.lbl_minutes.Visible = true;
                this.lbl_seconds.Visible = true;
                this.TxtHours.Visible = true;
                this.TxtMinute.Visible = true;
                this.TxtSeconds.Visible = true;
            }

            //this.UpPnlCalendarHours.Update();
        }

        /// <summary>
        /// Impostazione / reperimento valore della data dello usercontrol
        /// </summary>
        public string Text
        {
            get
            {
                string value = string.Empty;

                if (this.VisibleTimeMode == VisibleTimeModeEnum.Nothing && !string.IsNullOrEmpty(TxtDataCalendar.Text))
                    value = this.TxtDataCalendar.Text;
                else
                {
                    if (!string.IsNullOrEmpty(this.TxtDataCalendar.Text))
                    {
                        if (this.VisibleTimeMode == VisibleTimeModeEnum.Hours)
                        {
                            value = string.Format("{0} {1}",
                                            this.TxtDataCalendar.Text,
                                            this.GetTimePart(this.TxtHours.Text));
                        }

                        if (this.VisibleTimeMode == VisibleTimeModeEnum.Minutes)
                        {
                            value = string.Format("{0} {1}:{2}", this.TxtDataCalendar.Text, this.GetTimePart(this.TxtHours.Text), this.GetTimePart(this.TxtMinute.Text));
                        }

                        if (this.VisibleTimeMode == VisibleTimeModeEnum.Seconds)
                        {
                            value = string.Format("{0} {1}:{2}:{3}", this.TxtDataCalendar.Text, this.GetTimePart(this.TxtHours.Text), this.GetTimePart(this.TxtMinute.Text), this.GetTimePart(this.TxtSeconds.Text));
                        }

                    }
                }

                return value;
            }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                {
                    this.TxtDataCalendar.Text = date.ToString("dd/MM/yyyy");
                    this.TxtHours.Text = date.ToString("HH");
                    this.TxtMinute.Text = date.ToString("mm");
                    this.TxtSeconds.Text = date.ToString("ss");
                }
                else
                {
                    this.TxtDataCalendar.Text = string.Empty;
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
            {
                return "00";
            }
            else
            {
                return timePart;
            }
        }

        #region Session Utility
        //[Browsable(true)]
        //public string IdControl
        //{
        //    get
        //    {
        //        string result = string.Empty;
        //        if (HttpContext.Current.Session["idControl"] != null)
        //        {
        //            result = HttpContext.Current.Session["idControl"].ToString();
        //        }
        //        return result;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session[idControl"] = value;
        //    }
        //}

        public enum VisibleTimeModeEnum
        {
            Nothing,
            Hours,
            Minutes,
            Seconds
        }

      

        public bool ReadOnly
        {
            get
            {
                return sReadonly;
            }
            set
            {
                sReadonly = value;
            }
        }


        [Browsable(true)]
        public VisibleTimeModeEnum VisibleTimeMode
        {
            get
            {
                VisibleTimeModeEnum result = VisibleTimeModeEnum.Nothing;
                if (this.ViewState["visibleTimeMode"] != null)
                {
                    result = (VisibleTimeModeEnum)this.ViewState["visibleTimeMode"];
                }
                return result;
            }
            set
            {
                this.ViewState["visibleTimeMode"] = value;
            }
        }
        #endregion
    }
}