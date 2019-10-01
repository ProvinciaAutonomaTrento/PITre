using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;


namespace DocsPaWebCtrlLibrary
{
	/// <summary>
	/// Summary description for WebCustomControl1.
	/// </summary>
	[DefaultEvent("Navigate"),DefaultProperty("NavigateTo"),
	ToolboxData("<{0}:IFrameWebControl runat=server></{0}:IFrameWebControl>")]
	public class IFrameWebControl : System.Web.UI.WebControls.WebControl, IPostBackDataHandler 
	{
		private string _src; 
		private string _width;
		private string _height;
		private string _marginwidth;
		private string _marginheight;
		private string _align;
		private string _frameborder;
		private string _scrolling;

		public IFrameWebControl()
		{
			_src = "";
			 _width="";
			 _height="";
			 _marginwidth="";
			 _marginheight="";
			 _align="";
			 _frameborder="";
			 _scrolling="";

		}

		[Bindable(true),
		Category("Misc"),
		DefaultValue("")]	
		public virtual string NavigateTo
		{
			get
			{
				return _src;
			}

			set
			{
				_src = value;
			}
		}

		[Bindable(true),
		Category("Misc"),
		DefaultValue("")]	
		public virtual string Frameborder
		{
			get
			{
				return this._frameborder;
			}

			set
			{
				this._frameborder = value;
			}
		}

	
		[Bindable(true),
		Category("Misc"),
		DefaultValue("")]	
		public virtual string Marginwidth
		{
			get
			{
				return this._marginwidth;
			}

			set
			{
				this._marginwidth = value;
			}
		}
		[Bindable(true),
		Category("Misc"),
		DefaultValue("")]	
		public virtual string Marginheight
		{
			get
			{
				return this._marginheight;
			}

			set
			{
				this._marginheight = value;
			}
		}
		[Bindable(true),
		Category("Misc"),
		DefaultValue("")]	
		public virtual string Align
		{
			get
			{
				return this._align;
			}

			set
			{
				this._align = value;
			}
		}
		[Bindable(true),
		Category("Misc"),
		DefaultValue("")]	
		public virtual string Scrolling
		{
			get
			{
				return this._scrolling;
			}

			set
			{
				this._scrolling = value;
			}
		}
		[Bindable(true),
		Category("Layout"),
		DefaultValue("")]	
		public virtual string iHeight
		{
			get
			{
				return this._height;
			}

			set
			{
				this._height = value;
			}
		}

		[Bindable(true),
		Category("Layout"),
		DefaultValue("")]	
		public virtual string iWidth
		{
			get
			{
				return this._width;
			}

			set
			{
				this._width = value;
			}
		}


		

	



		public bool LoadPostData(string postDataKey, NameValueCollection postCollection) 
		{
			string presentValue = _src;
			string postedValue = postCollection[postDataKey];

			if (!presentValue.Equals(postedValue))
			{

				_src = postedValue;
				return true;
			}
			return false;
		}

		public event EventHandler Navigate;
	
		protected virtual void OnNavigate(EventArgs e)
		{
			if (Navigate != null)
				Navigate(this,e);
		}

	

		public virtual void RaisePostDataChangedEvent() 
		{
			OnNavigate(EventArgs.Empty);
		}



		/// <summary>
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
				
			output.WriteLine("<iframe height='"+this._height+"' scrolling='"+this._scrolling+"' marginheight='"+this._marginheight+"' marginwidth='"+this._marginwidth+"' frameborder='"+this._frameborder+"' align='"+this._align+"' width='"+this._width+"' name=\"" + this.UniqueID + "\" src='"+this._src+"'  ></iframe>");
//					output.Write("<input type=hidden name=" + this.UniqueID + " value=\""+ Page.Server.UrlEncode(_src)+"\">");

				
			

		}
	}
}
