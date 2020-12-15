using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;

namespace ConservazioneWA.UserControl
{
    [DefaultProperty("Text"),
           ToolboxData("<{0}:CustomButton runat=server></{0}:CustomButton>")]
    public class CustomButton : System.Web.UI.WebControls.Button
    {
        public CustomButton()
        {
            ViewState["Enabled1"] = true;
            ViewState["CssClassDisabled"] = string.Empty;
            ViewState["OnMouseOver"] = string.Empty;
            ViewState["OnMouseOut"] = string.Empty;
        }

        [Bindable(true), Browsable(true),
            Category("Appearance"),
            DefaultValue(true)]
        public override bool Enabled
        {
            get
            {
                return Boolean.Parse(ViewState["Enabled1"].ToString());
            }

            set
            {
                ViewState["Enabled1"] = value.ToString();
            }
        }

        [Bindable(true), Browsable(true),
        Category("Behavior"),
        DefaultValue("")]
        public string CssClassDisabled
        {
            get
            {
                return (string)ViewState["CssClassDisabled"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["CssClassDisabled"] = value.ToString();
            }
        }

        [Bindable(true), Browsable(true),
      Category("Appearance"),
      DefaultValue("")]
        public string OnMouseOver
        {
            get
            {
                return (string)ViewState["OnMouseOver"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["OnMouseOver"] = value.ToString();
            }
        }

        [Bindable(true), Browsable(true),
       Category("Appearance"),
       DefaultValue("")]
        public string OnMouseOut
        {
            get
            {
                return (string)ViewState["OnMouseOut"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["OnMouseOut"] = value.ToString();
            }
        }
        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        /// 

        protected override void Render(HtmlTextWriter output)
        {
            if (this.Enabled)
            {
                base.CssClass = this.CssClass;
                base.Attributes.Add("onmouseover", "this.className='" + this.OnMouseOver + "';");
                base.Attributes.Add("onmouseout", "this.className='"+ this.CssClass+"';");
            }
            else
            {
                base.CssClass = this.CssClassDisabled;
                base.Attributes.Remove("onmouseover");
                base.Attributes.Remove("onmouseout");
            }

            base.Render(output);
        }
    }
}
