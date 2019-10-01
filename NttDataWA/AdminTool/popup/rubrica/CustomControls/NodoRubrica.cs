using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SAAdminTool.DocsPaWR;
using SAAdminTool.popup.RubricaDocsPA.CustomControls;

namespace SAAdminTool.popup.RubricaDocsPA
{
	/// <summary>
	/// Summary description for NodoRubrica.
	/// </summary>
	public class NodoRubrica : Microsoft.Web.UI.WebControls.TreeNode
	{
		private const string SELECT_ALLOWED="SELECT_ALLOWED";

		/// <summary>
		/// 
		/// </summary>
		public bool SelectAllowed
		{
			get
			{
				bool retValue=false;

				try
				{
					retValue=Convert.ToBoolean(this.GetViewStateItem(SELECT_ALLOWED));
				}
				catch
				{
				}
					
				return retValue;
			}
			set
			{
				this.SetViewStateItem(SELECT_ALLOWED,value.ToString());
			}
		}

		private void SetViewStateItem(string key,string value)
		{
			ViewState[key]=value;
		}

		private string GetViewStateItem(string key)
		{
			if (ViewState[key]!=null)
				return ViewState[key].ToString();
			else
				return string.Empty;
		}
	}
}
