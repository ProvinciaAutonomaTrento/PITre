using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;

namespace NttDatalLibrary
{
    [DefaultProperty("Text"),
           ToolboxData("<{0}:CustomImageButton runat=server></{0}:CustomImageButton>")]
    public class CustomImageButton : System.Web.UI.WebControls.ImageButton
    {
        public CustomImageButton()
        {
            ViewState["Enabled2"] = true;
            ViewState["ImageUrlDisabled"] = string.Empty;
            ViewState["OnMouseOverImage"] = string.Empty;
            ViewState["OnMouseOutImage"] = string.Empty;
        }

        [Bindable(true), Browsable(true),
        Category("Appearance"),
        DefaultValue(true)]
        public override bool Enabled
        {
            get
            {
                return Boolean.Parse(ViewState["Enabled2"].ToString());
            }

            set
            {
                ViewState["Enabled2"] = value.ToString();
            }
        }

        [Bindable(true), Browsable(true),
        Category("Appearance"),
        DefaultValue("")]
        public string ImageUrlDisabled
        {
            get
            {
                return (string)ViewState["ImageUrlDisabled"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["ImageUrlDisabled"] = value.ToString();
            }
        }

        [Bindable(true), Browsable(true),
      Category("Behavior"),
      DefaultValue("")]
        public string OnMouseOverImage
        {
            get
            {
                return (string)ViewState["OnMouseOverImage"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["OnMouseOverImage"] = value.ToString();
            }
        }

        [Bindable(true), Browsable(true),
       Category("Behavior"),
       DefaultValue("")]
        public string OnMouseOutImage
        {
            get
            {
                return (string)ViewState["OnMouseOutImage"];
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["OnMouseOutImage"] = value.ToString();
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
                base.ImageUrl = this.ImageUrl;
                base.Attributes.Add("onmouseover", "this.src='" + this.OnMouseOverImage + "';");
                base.Attributes.Add("onmouseout", "this.src='" + this.OnMouseOutImage + "';");
            }
            else
            {
                this.ImageUrl = this.ImageUrlDisabled;
                base.Attributes.Remove("onmouseover");
                base.Attributes.Remove("onmouseout");
               
            }

            base.Render(output);
        }

        protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            // Control coordinates are sent in decimal by IE10
            // Recreating the collection with corrected values
            NameValueCollection modifiedPostCollection = new NameValueCollection();
            for (int i = 0; i < postCollection.Count; i++)
            {
                string actualKey = postCollection.GetKey(i);
                if (!string.IsNullOrEmpty(actualKey))
                {
                    string[] actualValueTab = postCollection.GetValues(i);
                    if (actualKey.EndsWith(".x") || actualKey.EndsWith(".y"))
                    {
                        string value = actualValueTab[0];
                        decimal dec;
                        Decimal.TryParse(value, out dec);
                        if ((long)dec<(long)Int32.MaxValue) 
                            modifiedPostCollection.Add(actualKey, ((long)Math.Round(dec)).ToString());
                        else
                            modifiedPostCollection.Add(actualKey, Int32.MaxValue.ToString());
                    }
                    else
                    {
                        foreach (string actualValue in actualValueTab)
                        {
                            modifiedPostCollection.Add(actualKey, actualValue);
                        }
                    }
                }
            }
            return base.LoadPostData(postDataKey, modifiedPostCollection);
        }

    }
}
