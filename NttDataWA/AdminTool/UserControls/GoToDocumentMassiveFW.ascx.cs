using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.SiteNavigation;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.UserControls
{
    public partial class GoToDocumentMassiveFW : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se non si è in postback viene caricata e memorizzata l'autorizzazione all'inoltro massivo
            // e si verifica se sono presenti le condizioni per visualizzare il pulsante
            if (!IsPostBack)
            {
                this.IsRoleAuthorizedMassiveForwarding = UserManager.getRuolo(this.Page).funzioni.Where(
                    role => role.codice.Equals("MASSIVE_INOLTRA")).Count() > 0;
                this.Visible = this.IsRoleAuthorizedMassiveForwarding &&
                    !String.IsNullOrEmpty(this.DocumentId) &&
                    this.DocumentId != "-1" &&
                    this.ShowIfIsAttachment;
            }

        }

        protected void imgGoToDocument_Click(object sender, ImageClickEventArgs e)
        {
            // Reperimento scheda del documento
            SchedaDocumento doc = DocumentManager.getDettaglioDocumento(this.Page, this.DocumentId, String.Empty);

            // Salvataggio del documento selezionato
            DocumentManager.setDocumentoSelezionato(this.Page, doc);

            // Navigazione verso il documento,
            // indicando di forzare la crezione di un nuovo contesto e
            // se il documento è stato inserito in cestino
            this.Page.ClientScript.RegisterStartupScript(
                this.GetType(),
                "GoToDoc",
                "window.open('gestioneDoc.aspx?tab=profilo&forceNewContext=true','principale');",
                true);
        }

        /// <summary>
        /// True se il ruolo è autorizzato a compiere l'inoltro massivo
        /// </summary>
        private bool IsRoleAuthorizedMassiveForwarding
        {
            get
            {
                return Boolean.Parse(CallContextStack.CurrentContext.ContextState["MassiveForwarding"].ToString());
            }

            set
            {
                CallContextStack.CurrentContext.ContextState["MassiveForwarding"] = value;
            }
        }

        /// <summary>
        /// System id del documento da linkare
        /// </summary>
        public String DocumentId { get; set; }

        /// <summary>
        /// Tool tip da visualizzare
        /// </summary>
        public String ToolTip
        {
            get
            {
                return this.imgGoToDocument.ToolTip;
            }

            set 
            {
                this.imgGoToDocument.ToolTip = value;
            }
        }

        /// <summary>
        /// True se il tasto va mostrato anche nel caso di documento allegato
        /// </summary>
        public bool ShowIfIsAttachment { get; set; }

    }
}