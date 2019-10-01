using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;

namespace NttDatalLibrary
{
    public class CustomImage : System.Web.UI.WebControls.Image
    {
        public CustomImage()
        {
            ViewState["ImageUrlDisalbed"] = string.Empty;
            ViewState["OnMouseOver"] = string.Empty;
            ViewState["OnMouseOut"] = string.Empty;
        }

        [Bindable(true), Browsable(true),
        Category("Behavior"),
        DefaultValue("")]
        public string ImageUrlDisalbed
        {
            get
            {
                return (string)ViewState["ImageUrlDisalbed"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["ImageUrlDisalbed"] = value.ToString();
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
            //if (this.ena)
            //{
            //    base.CssClass = this.CssClass;
            //    base.Attributes.Add("onmouseover", "this.className='" + this.OnMouseOver + "';");
            //    base.Attributes.Add("onmouseout", "this.className='"+ this.CssClass+"';");
            //}
            //else
            //{
            //    base.CssClass = this.CssClassDisabled;
            //    base.Attributes.Remove("onmouseover");
            //    base.Attributes.Remove("onmouseout");
            //}

            //base.Render(output);
        }
    }

}
