using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace ConservazioneWA.PopUp
{
    public partial class Note : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btn_inserisci.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_inserisci.Attributes.Add("onmouseout", "this.className='cbtn';");

            btn_chiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            btn_chiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
        }

        protected void btn_inserisci_Click(object sender, EventArgs e)
        {
            string note_appo = this.TextArea1.Value.ToString();
            string note = note_appo.Replace("\r\n", " ");
            note = note.Replace("'", "\\'");
            ClientScript.RegisterStartupScript(this.GetType(), "close", "window.returnValue='" + note + "'; window.close();",true);
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "close", "window.close();",true);
        }
    }
}
