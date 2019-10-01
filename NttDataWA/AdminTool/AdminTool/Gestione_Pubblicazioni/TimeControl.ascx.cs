using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAAdminTool.AdminTool.Gestione_Pubblicazioni
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TimeControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.txtHours.Text = "00";
                this.txtMinutes.Text = "00";
                this.txtSeconds.Text = "00";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.txtHours.Enabled;
            }
            set
            {
                this.txtHours.Enabled = value;
                this.txtMinutes.Enabled = value;
                this.txtSeconds.Enabled = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        public void SetTimeSpan(TimeSpan ts)
        {
            this.txtHours.Text = ts.Hours.ToString();
            this.txtMinutes.Text = ts.Minutes.ToString();
            this.txtSeconds.Text = ts.Seconds.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTimeSpan()
        {
            int hours;
            Int32.TryParse(this.txtHours.Text, out hours);

            int minutes;
            Int32.TryParse(this.txtMinutes.Text, out minutes);

            int seconds;
            Int32.TryParse(this.txtSeconds.Text, out seconds);
            
            return new TimeSpan(0, hours, minutes, seconds, 0);
        }
    }
}