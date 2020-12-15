using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace DocsPaWebCtrlLibrary
{
	/// <summary>
	/// Summary description for Panel.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:Panel runat=server></{0}:Panel>")]
	public class Panel : System.Web.UI.WebControls.Panel
	{
		private string tipologia;
	
		[Bindable(true),Browsable(true), 
		Category("Behavior"), 
		DefaultValue("")] 
		public string Tipologia
		{
			get
			{
				if (tipologia==null)
					return tipologia ="";
				else
					return tipologia;
			}

			set
			{
				tipologia = value;
			}
		}
		protected override object SaveViewState()
		{
			object[] State = new object[1];
			State[0] = tipologia;
			return (object) State;

		}
		protected override void LoadViewState(object state)
		{
			object[] SavedState =  (object[]) state;
			tipologia = (string) SavedState[0];
			

		}
		
	
	
		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			base.Render(output);	
		}
	}
}
