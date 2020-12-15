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
	[DefaultEvent("Navigate"),
	ToolboxData("<{0}:IFrameServerControl runat=server></{0}:IFrameServerControl>")]
	public class IFrameServerControl : System.Web.UI.WebControls.WebControl, IPostBackDataHandler 
	{
		private string _src; 

		public IFrameServerControl()
		{
			_src = "";
		}

		public virtual string Name
		{
			get
			{
				return this.Page.ToString().Replace(".","_") + "_" + this.UniqueID;
			}
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
			output.Write("<input type=hidden name=" + this.UniqueID + " value=\""+ Page.Server.UrlEncode(this.NavigateTo)+"\">");
			output.WriteLine("<iframe name=\"" + this.Name + "\" scrolling=\"auto\"></iframe>");

			if(!this.NavigateTo.Equals(""))
			{
				output.WriteLine("<script language=\"jscript\">");
				output.WriteLine(this.Parent.ClientID + ".target=\""+ this.Name +"\";");
				output.WriteLine(this.Parent.ClientID + ".submit();");
				output.WriteLine("</script>");
			}
		}
	}
}
