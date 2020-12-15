using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Specialized;

namespace ConservazioneWA.UserControl
{
     [DefaultProperty("Text"),
           ToolboxData("<{0}:CustomTextArea runat=server></{0}:CustomTextArea>")]
    public class CustomTextArea : System.Web.UI.WebControls.TextBox
    {
         public CustomTextArea()
         {
             ViewState["ReadOnly1"] = false;
             ViewState["CssClassReadOnly"] = string.Empty;
         }


         [Bindable(true), Browsable(true),
         Category("Behavior"),
         DefaultValue("")]
         public string CssClassReadOnly
         {
             get
             {
                 return (string)ViewState["CssClassReadOnly"];
             }

             set
             {
                 if (!string.IsNullOrEmpty(value))
                     ViewState["CssClassReadOnly"] = value.ToString();
             }
         }

         /// <summary> 
         /// Render this control to the output parameter specified.
         /// </summary>
         /// <param name="output"> The HTML writer to write out to </param>
         /// 

         protected override void Render(HtmlTextWriter output)
         {
             if (this.ReadOnly || !this.Enabled)
             {
                 base.CssClass = this.CssClassReadOnly;
                 
             }
             else
             {
                 base.CssClass = this.CssClass;
             }   

             base.Render(output);
         }
    }
}
