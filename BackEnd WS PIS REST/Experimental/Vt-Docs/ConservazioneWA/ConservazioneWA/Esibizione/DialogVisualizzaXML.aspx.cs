using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.Esibizione
{
    public partial class DialogVisualizzaXML : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;
            if (!IsPostBack)
            {
                this.btnChiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                this.btnChiudi.Attributes.Add("onmouseout", "this.className='cbtn';");
            }

            // Parametri passati dal chiamante:
            string idIstanza = Request.QueryString["idIstanza"];
            this.hd_idIstanza.Value = idIstanza;
            string idDoc = Request.QueryString["idDoc"];
            this.hd_idDoc.Value = idDoc;
            string type = Request.QueryString["type"];
            this.hd_type.Value = type;

            this.btnChiudi.Attributes.Add("onClick", "CloseWindow('Chiudi');");
        }
    }
}