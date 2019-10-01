using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SAAdminTool.fascicolo
{
    public partial class AclFascicolo : System.Web.UI.UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnAclRevocata = null;
        string errorMessage;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.VerificaRevocaAcl();

            if (this.AclRevocata)
                this.HandlerAclRevocata();
        }

        /// <summary>
        /// Visualizzazione dialog di errore relativo all'acl revocata sul documento
        /// </summary>
        protected virtual void HandlerAclRevocata()
        {
            if (this.ShowDefaultMessageAclRevocata)
            {
                // Visualizzazione messaggio di default, se richiesto
                //string errorMessage = "Sono stati tolti i diritti di visibilità per questo fascicolo.\\nNon è più possibile visualizzarlo.";

                Response.Write("<script>alert('" + errorMessage + "');</script>");
            }

            if (this.OnAclRevocata != null)
                this.OnAclRevocata(this, new EventArgs());
        }

        /// <summary>
        /// Flag, indica se l'acl sull'oggetto è stata revocata o meno
        /// </summary>
        public bool AclRevocata
        {
            get
            {
                if (this.ViewState["AclRevocata"] != null)
                    return Convert.ToBoolean(this.ViewState["AclRevocata"]);
                else
                    // Se nel viewstate non è presente il dato,
                    // la verifica dell'acl non è stata ancora effettuata
                    throw new ApplicationException("Verifica della revoca dell'ACL non effettuata");
            }
            set
            {
                this.ViewState["AclRevocata"] = value;
            }
        }

        /// <summary>
        /// Indica se visualizzare o meno il messaggio di default
        /// in caso di acl revocata
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public bool ShowDefaultMessageAclRevocata
        {
            get
            {
                if (this.ViewState["ShowDefaultMessageAclRevocata"] != null)
                    return Convert.ToBoolean(this.ViewState["ShowDefaultMessageAclRevocata"]);
                else
                    return true;
            }
            set
            {
                this.ViewState["ShowDefaultMessageAclRevocata"] = value;
            }
        }

        /// <summary>
        /// ID del documento per cui si deve verificare l'acl
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public string IdFascicolo
        {
            get
            {
                if (this.ViewState["IdFascicolo"] != null)
                    return this.ViewState["IdFascicolo"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["IdFascicolo"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void VerificaRevocaAcl()
        {
            if (!string.IsNullOrEmpty(this.IdFascicolo))
            {
                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                int result = DocumentManager.verificaACL("F", this.IdFascicolo, UserManager.getInfoUtente(), out errorMessage);

                // Imposta lo stato della revoca dell'acl
                this.AclRevocata = (result == 0);
            }
            else
                this.AclRevocata = false;
        }
    }
}