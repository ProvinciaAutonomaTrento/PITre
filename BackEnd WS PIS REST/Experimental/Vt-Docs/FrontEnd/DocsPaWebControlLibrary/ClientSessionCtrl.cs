using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;


namespace DocsPaWebCtrlLibrary
{
	/// <summary>
	/// Summary description for WebCustomControl2.
	/// </summary>
	[ToolboxData("<{0}:ClientSessionControl runat=server></{0}:ClientSessionControl>")]
	public class ClientSessionControl : System.Web.UI.WebControls.WebControl, IPostBackDataHandler 
	{

		private string _xml;

		public ClientSessionControl()
		{
			_xml = "";
		}
		
		[Bindable(true), 
		Category("Misc"), 
		DefaultValue("")] 
		public virtual string ClientSessionState 
		{
			get
			{
				return _xml;
			}

			set
			{
				_xml = value;
			}
		}

		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			//string name = "__ClientSessionControl";
			string name = this.UniqueID;
			output.Write("<input type=hidden name=" + name + " value=\""+ Page.Server.HtmlEncode(_xml)+"\">");
		}

		public event EventHandler ClientSessionChanged;
	
		protected virtual void OnClientSessionChanged(EventArgs e)
		{
			if (ClientSessionChanged != null)
				ClientSessionChanged(this,e);
		}

		public bool LoadPostData(string postDataKey, NameValueCollection postCollection) 
		{
			string presentValue = _xml;
			string postedValue = postCollection[postDataKey];

			if (!presentValue.Equals(postedValue))
			{

				_xml = postedValue;
				return true;
			}
			return false;
		}

		public virtual void RaisePostDataChangedEvent() 
		{
			OnClientSessionChanged(EventArgs.Empty);      
		}
	}
}
