using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;


namespace DocsPaWebCtrlLibrary
{
	/// <summary>
	/// Summary description for WebCustomControl3.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ImageButton runat=server></{0}:ImageButton>")]
	public class ImageButton : System.Web.UI.WebControls.ImageButton 
	{
		
	
		public ImageButton()
		{
			ViewState["Enabled1"] = true;
			ViewState["DisabledUrl"] = "";
			ViewState["Tipologia"]="";
            ViewState["Thema"] = "";
		}

		[Bindable(true),Browsable(true), 
			Category("Appearance"), 
			DefaultValue(true)] 
		public override bool Enabled 
		{
			get
			{
				return  Boolean.Parse(ViewState["Enabled1"].ToString());
			}

			set
			{
				ViewState["Enabled1"] = value.ToString();
			}
		}

		[Bindable(true), Browsable(true),
		Category("Appearance"), 
		DefaultValue("")] 
		public string DisabledUrl 
		{
			get
			{
				return (string) ViewState["DisabledUrl"];
			}

			set
			{
                if (value != null && !value.ToString().Equals(""))
                    ViewState["DisabledUrl"] = value.ToString();
			}
		}

        [Bindable(true),Browsable(true), 
		Category("Behavior"), 
		DefaultValue("")] 
		public string Tipologia
		{
			get
			{
				return (string) ViewState["Tipologia"];
			}

			set
			{
				ViewState["Tipologia"] = value.ToString();
			}
		}

        [Bindable(true), Browsable(true),
        Category("Behavior"),
        DefaultValue("")]
        public string Thema
        {
            get
            {
                return (string)ViewState["Thema"];
            }

            set
            {
                ViewState["Thema"] = value.ToString();
            }
        }


		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		/// 
		
	
		
		protected override void Render(HtmlTextWriter output)
		{
            if (!this.Enabled)
            {
                if(this.DisabledUrl == string.Empty)
                    base.ImageUrl = "~/App_Themes/" + this.Page.Theme + "/" + this.Thema + this.SkinID + ".gif";
                else
                    base.ImageUrl = this.DisabledUrl;
                base.Attributes.Add("onclick", "return false;");
                //				base.Style.Add("CURSOR","DEFAULT");  12.12.2006 gadamo: disabilitato per far in modo che i pulsanti del menù abbiano il cursore con la manina
            }
            else
            {
                if (this.Thema != string.Empty)
                    base.ImageUrl = "~/App_Themes/" + this.Page.Theme + "/" + this.Thema + this.SkinID + ".gif";
            }

            base.Render(output);
		}
	}
}
