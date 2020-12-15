using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace DocsPaWebCtrlLibrary
{
    [DefaultProperty("Text"),
    ToolboxData("<{0}:TimeMask runat=server></{0}:TimeMask>")]
    public class TimeMask : System.Web.UI.WebControls.TextBox 
    {
        /// <summary>
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            output.AddAttribute("maxlength", "8");
            output.AddAttribute("onBlur", "FormatTime(this);");
            output.AddAttribute("onKeyPress", "ValidateNumKey(); FormatTime(this);");

            base.Render(output);
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.RenderJSFunctions();
        }

        private void RenderJSFunctions()
        {
            if (!this.Page.IsClientScriptBlockRegistered("ValidateNumKey"))
                this.Page.RegisterClientScriptBlock("ValidateNumKey", this.GetJSFunctionValidateNumKey());

            if (!this.Page.IsClientScriptBlockRegistered("FormatTime"))
                this.Page.RegisterClientScriptBlock("FormatTime", this.GetJSFunctionFormatDate());
        }

        /// <summary>
        /// Funzione javascript di verifica se il valore immesso è un numero
        /// </summary>
        /// <returns></returns>
        private string GetJSFunctionValidateNumKey()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

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

        private string GetJSFunctionFormatDate()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<script language='javascript'>");
            sb.Append("function FormatTime(timeControl){");
            sb.Append("var inputKey=event.keyCode;");
            sb.Append("if(inputKey==37 || inputKey==39 || inputKey==8 || inputKey==46){return;}");
            sb.Append("var textValue=timeControl.value;");
            sb.Append("var contentArray=textValue.split('.');");
            sb.Append("var innerValue=contentArray.join('');");
            sb.Append("if (innerValue.length==7 || parseInt(contentArray.length)>=3 || innerValue.length == 3) {}");
            sb.Append("else if (innerValue.length == 4 && innerValue.length<8)");
            sb.Append("{ timeControl.value=innerValue.substr(0,2) + '.' + innerValue.substr(2,2) + '.' + '00'; }");
            sb.Append("else if (innerValue.length > 4 && innerValue.length<8)");
            sb.Append("{ timeControl.value=innerValue.substr(0,2) + '.' + innerValue.substr(2,2) + '.' + innerValue.substr(4); }");
            sb.Append("else if (innerValue.length > 1 && innerValue.length<8)");
            sb.Append("{ timeControl.value=innerValue.substr(0,2) + '.' + innerValue.substr(2); }");
            sb.Append("}");
            sb.Append("</script>");

            return sb.ToString();
        }

    }
}
