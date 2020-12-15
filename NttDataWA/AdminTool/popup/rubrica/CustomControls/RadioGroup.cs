using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web;
using System.IO;

namespace SAAdminTool.popup.RubricaDocsPA.CustomControls
{
	public struct RadioGroupItem 
	{
		public string Caption;
		public string Value;
		public bool Enabled;

		public RadioGroupItem (string c, string v)
		{
			Caption = c;
			Value = v;
			Enabled = true;
		}
	}

	public class RadioGroup : Panel
	{
		public ArrayList Items;
		public string Name;

		string _selected_value;

		public RadioGroup()
		{
			Name = "";
			Items = new ArrayList();
			EnableViewState = true;
			this.ID = this.ClientID;
			Width = new Unit("100%");
			Height = new Unit("100%");
			this.PreRender += new EventHandler(RadioGroup_PreRender);
		}

		public string SelectedValue 
		{
			get { 
				if (Visible && Page.IsPostBack && (_selected_value != null && _selected_value != "")) 
				{
					object o = HttpContext.Current.Request.Form[Name];
					return (o != null) ? o.ToString() : "";
				}
				else
					return _selected_value;
			}
			set { _selected_value = value; }
		}

		public new bool Visible 
		{
			get { return this.Enabled; }
			set { this.Enabled = value; }
		}

//		protected override void Render(System.Web.UI.HtmlTextWriter writer)
//		{
//			if (Name == null || Name == "")
//				throw new ArgumentException ();
//
//			if (SelectedValue == "")
//				SelectedValue = ((RadioGroupItem) Items[0]).Value;
//
////StreamWriter sw = new StreamWriter (@"c:\test.html");
////writer = new HtmlTextWriter(sw);
//
////			writer.WriteBeginTag ("table");
////			writer.WriteAttribute ("class", CssClass);
////			writer.WriteAttribute ("width", Width.ToString());
////			writer.WriteAttribute ("height", Height.ToString());
////			writer.WriteAttribute ("border", "0");
////			writer.WriteAttribute ("cellspacing", "1");
////			writer.WriteAttribute ("cellpadding", "1");
////			writer.Write (HtmlTextWriter.TagRightChar);
//
//			for (int i = 0; i < Items.Count; i++) 
//			{
////				writer.WriteBeginTag ("tr");
////				writer.Write (HtmlTextWriter.TagRightChar);
////
////				writer.WriteBeginTag ("td");
////				writer.Write (HtmlTextWriter.TagRightChar);
//
//				RadioGroupItem rgi = (RadioGroupItem) Items[i];
//
//				writer.WriteBeginTag ("input");
//				writer.WriteAttribute ("type", "radio");
//				writer.WriteAttribute ("name", Name);
//				writer.WriteAttribute ("value", rgi.Value);
//				writer.WriteAttribute ("id", "ctl_" + Name + "_" + i.ToString());
//				if (rgi.Value == SelectedValue)
//					writer.Write (" checked");
//
//				writer.Write (HtmlTextWriter.TagRightChar);
//
//				writer.WriteBeginTag ("span");
//				writer.WriteAttribute ("class", CssClass);
//				writer.Write (HtmlTextWriter.TagRightChar);
//
//				writer.Write (rgi.Caption);
//				writer.WriteEndTag ("span");
//
////				writer.WriteEndTag ("td");
////				writer.WriteEndTag ("tr");
//			}
////
////			writer.WriteEndTag ("table");
////sw.Close();

//			
//		}
		//

		private void RadioGroup_PreRender(object sender, EventArgs e)
		{
			string script = "<script language=\"javascript\">";

			for (int i = 0; i < Items.Count; i++) 
			{
				RadioGroupItem rgi = (RadioGroupItem) Items[i];
				RadioButton rb = new RadioButton();
				rb.GroupName = Name;
				rb.Text = rgi.Caption;
				rb.Checked = (rgi.Value == SelectedValue);
				rb.Attributes["value"] = rgi.Value;
				Controls.Add (rb);
				

				script += ("document.getElementById(\"" + rb.ClientID + "\").disabled=" + (!this.Visible).ToString().ToLower() + "; ");
			}
			
			script += "</script>";
			this.Page.RegisterStartupScript (Name + "_enabler", script); 
		}
	}
}
