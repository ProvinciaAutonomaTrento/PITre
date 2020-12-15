using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace DocsPaWebCtrlLibrary
{
	/// <summary>
	/// Summary description for WebCustomControl1.
	/// </summary>
	[DefaultProperty("Text"),
	ToolboxData("<{0}:DateMask runat=server></{0}:DateMask>")]
	public class DateMask : System.Web.UI.WebControls.TextBox 
	{
		/// <summary>
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{	
			output.AddAttribute("maxlength","10");
			output.AddAttribute("onBlur","FormatDate(this);");
			output.AddAttribute("onKeyPress","ValidateNumKey(); FormatDate(this);");

			base.Render(output);
		}

		protected override void OnPreRender(EventArgs e)
		{
			this.RenderJSFunctions();
		}

		/// <summary>
		/// Funzione javascript di verifica se il valore immesso è un numero
		/// </summary>
		/// <returns></returns>
		private string GetJSFunctionValidateNumKey()
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			sb.Append("<script language='javascript'>");
			sb.Append("function ValidateNumKey()");
			sb.Append("{var inputKey=event.keyCode;");
			sb.Append("var returnCode=true;");
			sb.Append("if(inputKey > 47 && inputKey < 58){return;}else");
			sb.Append("{returnCode=false; event.keyCode=0;}");
			sb.Append("event.returnValue = returnCode;}");
			sb.Append("</script>");

			return sb.ToString();
		}

		/// <summary>
		/// Funzione javascript per la formattazione del valore immesso come data
		/// </summary>
		/// <returns></returns>
		private string GetJSFunctionFormatDate()
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();

			sb.Append("<script language='javascript'>");
			sb.Append("function FormatDate(dateControl){");
			sb.Append("var inputKey=event.keyCode;");
			sb.Append("if(inputKey==37 || inputKey==39 || inputKey==8 || inputKey==46){return;}");
			sb.Append("var textValue=dateControl.value;");
			sb.Append("var contentArray=textValue.split('/');");
			sb.Append("var innerValue=contentArray.join('');");
			sb.Append("if (innerValue.length==7 || parseInt(contentArray.length)>=3) {}");
			sb.Append("else if (innerValue.length > 3)");
			sb.Append("{ dateControl.value=innerValue.substr(0,2) + '/' + innerValue.substr(2,2) + '/' + innerValue.substr(4); }");
			sb.Append("else if (innerValue.length > 1)");
			sb.Append("{ dateControl.value=innerValue.substr(0,2) + '/' + innerValue.substr(2); }");
			sb.Append("}");
			sb.Append("</script>");
			
			return sb.ToString();
		}

		private void RenderJSFunctions()
		{
			if (!this.Page.IsClientScriptBlockRegistered("ValidateNumKey"))
				this.Page.RegisterClientScriptBlock("ValidateNumKey",this.GetJSFunctionValidateNumKey());

			if (!this.Page.IsClientScriptBlockRegistered("FormatDate"))
				this.Page.RegisterClientScriptBlock("FormatDate",this.GetJSFunctionFormatDate());
		}
	}
}

