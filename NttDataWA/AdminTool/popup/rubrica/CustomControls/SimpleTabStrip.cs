using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace SAAdminTool.popup.RubricaDocsPA
{

	public class SimpleTabStrip : System.Web.UI.Control, IPostBackDataHandler			
	{
		public class Tab 
		{
			public string EnabledImageUrl;
			public string DisabledImageUrl;

			public Tab (string enabled_image_url, string disabled_image_url)
			{
				EnabledImageUrl = enabled_image_url;
				DisabledImageUrl = disabled_image_url;
			}
		}

		public AttributeCollection Attributes 
		{
			get { return _attributes; }
		}

		public Unit Width;
		public Unit Height;
		public Color BackColor;

		public ArrayList Tabs;

		private int _selected_index;

		private AttributeCollection _attributes;

		public event EventHandler SelectedIndexChange;

		public int SelectedIndex 
		{
			get { return _selected_index; }
			set { 
					bool fire = (_selected_index != value); 
					_selected_index = value; 
					if (fire)
						RaisePostDataChangedEvent();
			}
		}

		public SimpleTabStrip()
		{
			Tabs = new ArrayList();
			BackColor = Color.White;
			this.PreRender += new EventHandler(SimpleTabStrip_PreRender);
			_attributes = new AttributeCollection(new StateBag());
		}


		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			string theStyle = String.Format("width:{0};height:{1};background-color:{2}", 
				this.Width.ToString(), this.Height.ToString(),
				String.Format ("#{0:x2}{0:x2}{0:x2}", 
				this.BackColor.R,
				this.BackColor.G,
				this.BackColor.B));
			writer.WriteBeginTag ("table");
			writer.WriteAttribute ("style", theStyle);
			writer.WriteAttribute ("border", "0");
			writer.WriteAttribute ("cellpadding", "0");
			writer.WriteAttribute ("cellspacing", "0");
			writer.Write (HtmlTextWriter.TagRightChar);

			writer.WriteBeginTag("tr");
			writer.Write (HtmlTextWriter.TagRightChar);
			for (int i = 0; i < Tabs.Count; i++)
			{
				Tab tab = (Tab) Tabs[i];
				writer.WriteBeginTag ("td");
				writer.Write (HtmlTextWriter.TagRightChar);

				if (SelectedIndexChange != null) 
				{
					writer.WriteBeginTag ("a");
					string tgt = this.ClientID ;
					writer.WriteAttribute ("href", "javascript:__doPostBack('" + tgt + "','" + i.ToString() + "');");
			
					foreach (string key in this.Attributes.Keys) 
					{
						string val = (string)this.Attributes[key];
						writer.WriteAttribute (key, val);
					}
					writer.Write (HtmlTextWriter.TagRightChar);

					writer.WriteBeginTag ("img");
					writer.WriteAttribute ("src", i == SelectedIndex? tab.EnabledImageUrl : tab.DisabledImageUrl);
					writer.WriteAttribute ("border", "0");
					writer.Write (HtmlTextWriter.TagRightChar);
					writer.WriteEndTag ("a");
				}
				else
				{
					writer.WriteBeginTag ("div");
					string tgt = this.ClientID ;
					//writer.WriteAttribute ("href", "javascript:__doPostBack('" + tgt + "','" + i.ToString() + "');");
			
					foreach (string key in this.Attributes.Keys) 
					{
						string val = (string)this.Attributes[key];
						writer.WriteAttribute (key, val);
					}
					writer.Write (HtmlTextWriter.TagRightChar);

					writer.WriteBeginTag ("img");
					writer.WriteAttribute ("src", i == SelectedIndex? tab.EnabledImageUrl : tab.DisabledImageUrl);
					writer.WriteAttribute ("border", "0");
					writer.Write (HtmlTextWriter.TagRightChar);
					writer.WriteEndTag ("div");
				}
				writer.WriteEndTag ("td");
			}
			writer.WriteEndTag("tr");	
			writer.WriteEndTag("table");	

		}
		#region IPostBackDataHandler Members

		public void RaisePostDataChangedEvent()
		{
			if (SelectedIndexChange != null)
				SelectedIndexChange (this, new EventArgs());
		}

		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			HttpContext ctx = HttpContext.Current;
			string tgt = ctx.Request.Form["__EVENTTARGET"];
			string arg = ctx.Request.Form["__EVENTARGUMENT"];
			if (tgt == null || arg == null)
				return false;

			if (tgt.Equals (this.UniqueID)) 
			{
				// Ok, l'evento è per noi
				SelectedIndex = Convert.ToInt32 (arg);
				return true;
			}
			return false;
		}

		#endregion

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState (savedState);
			if (this.ViewState["_ts_selected_index"] != null) 
			{
				_selected_index = Convert.ToInt32 (this.ViewState["_ts_selected_index"]);
			}
		}

		private void SimpleTabStrip_PreRender(object sender, EventArgs e)
		{
			this.ViewState.Add("_ts_selected_index", SelectedIndex);
		}

		public void ClearEvents()
		{
			SelectedIndexChange = null;
		}
	}
}
