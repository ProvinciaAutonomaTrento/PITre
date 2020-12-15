using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.UserControls
{
    public partial class RubricaVeloce : System.Web.UI.UserControl
    {

        protected System.Web.UI.WebControls.Panel pnl_rubr_veloce_ingresso;
        protected System.Web.UI.WebControls.Panel pnl_rubr_veloce_uscita;
        protected System.Web.UI.WebControls.Panel pnl_mittente_semplificato_ingresso;

        protected void Page_Load(object sender, EventArgs e)
        {
            EnableAutoComplete();
        }

        protected void txtAutoComplete_rubr_Changed(object sender, EventArgs e)
        {

        }

        protected void txtAutoComplete_rubr_dest_changed(object sender, EventArgs e)
        {

        }

        private void EnableAutoComplete()
        {
            
            if (System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"] != null)
            {
                autoComplete2.MinimumPrefixLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"].ToString());
                autoComplete3.MinimumPrefixLength = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RUBRICAVELOCE_MINIMUMPREFIXLENGTH"].ToString());
            }
            
            string dataUser = null;
            System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            if (ctx.Session["userRuolo"] != null)
            {
                dataUser = ((DocsPAWA.DocsPaWR.Ruolo)ctx.Session["userRuolo"]).systemId;
            }
         
            if (ctx.Session["userRegistro"] != null)
            {
                dataUser = dataUser + "-" + ((Registro)ctx.Session["userRegistro"]).systemId;
            }

            
            DocsPaWR.Corrispondente cr = (DocsPAWA.DocsPaWR.Corrispondente)this.Session["userData"];
            string idAmm = cr.idAmministrazione;
            string callType = null;
            string javascript = null;

            switch (this.CALLTYPE_RUBRICA_VELOCE)
            {
                // Mittente su protocollo in ingresso
                case "CALLTYPE_PROTO_IN":
                    callType = "CALLTYPE_PROTO_IN";
                    pnl_rubr_veloce_ingresso.Visible = true;
                    txtAutoComplete_rubr.Enabled = true;
                    txtAutoComplete_rubr.Text = "";
                    autoComplete2.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                // Mittente su protocollo in uscita
                case "CALLTYPE_PROTO_OUT_MITT":
                    callType = "CALLTYPE_PROTO_OUT_MITT";
                    pnl_rubr_veloce_ingresso.Visible = true;
                    txtAutoComplete_rubr.Enabled = true;
                    txtAutoComplete_rubr.Text = "";
                    autoComplete2.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                // Mittente su protocollo interno
                case "CALLTYPE_PROTO_INT_MITT":
                    callType = "CALLTYPE_PROTO_INT_MITT";
                    pnl_rubr_veloce_ingresso.Visible = true;
                    txtAutoComplete_rubr.Enabled = true;
                    txtAutoComplete_rubr.Text = "";
                    autoComplete2.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                // Destinatari
                case "CALLTYPE_PROTO_OUT":
                    callType = "CALLTYPE_PROTO_OUT";
                    pnl_rubr_veloce_uscita.Visible = true;
                    txtAutoComplete_rubr_dest.Enabled = true;
                    txtAutoComplete_rubr_dest.Text = "";
                    autoComplete3.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;               
                // Destinatario su protocollo interno
                case "CALLTYPE_PROTO_INT_DEST":
                    callType = "CALLTYPE_PROTO_INT_DEST";
                    pnl_rubr_veloce_uscita.Visible = true;
                    txtAutoComplete_rubr_dest.Enabled = true;
                    txtAutoComplete_rubr_dest.Text = "";
                    autoComplete3.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                //Mittente su protocollo ingresso semplificato
                case "CALLTYPE_PROTO_INGRESSO":
                    callType = "CALLTYPE_PROTO_INGRESSO";
                    pnl_mittente_semplificato_ingresso.Visible = true;
                    txt_autoComplete_rubr_sempl_ingresso.Enabled = true;
                    txt_autoComplete_rubr_sempl_ingresso.Text = "";
                    autocomplete4.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                //Mittente su protocollo uscita semplificato
                case "CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO":
                    callType = "CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO";
                    pnl_autoComplete_rubr_sempl_uscita_mitt.Visible = true;
                    txt_autoComplete_rubr_sempl_uscita_mitt.Enabled = true;
                    txt_autoComplete_rubr_sempl_uscita_mitt.Text = "";
                    autocomplete5.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                //Destinatari protocollo in uscita semplificato
                case "CALLTYPE_PROTO_USCITA_SEMPLIFICATO":
                    callType = "CALLTYPE_PROTO_USCITA_SEMPLIFICATO";
                    pnl_rubrica_veloce_destinatario_sempl.Visible = true;
                    txt_rubrica_veloce_destinatario_sempl.Enabled = true;
                    txt_rubrica_veloce_destinatario_sempl.Text = "";
                    autocomplete6.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                //Mittente ingresso multiplo
                case "CALLTYPE_MITT_MULTIPLI":
                    callType = "CALLTYPE_MITT_MULTIPLI";
                    pnl_rubr_veloce_ingr_multiplo.Visible = true;
                    txtAutoComplete_rubr_mitt_multiplo.Visible = true;
                    txtAutoComplete_rubr_mitt_multiplo.Enabled = true;
                    txtAutoComplete_rubr_mitt_multiplo.Text = "";
                    AutoComplete7.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
                //Mittene ingresso multiplo semplificato
                case "CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO":
                    callType = "CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO";
                    pnl_rubr_veloce_mitt_sempl_multipli.Visible = true;
                    txt_autoComplete_rubr_sempl_ingresso_multi.Visible = true;
                    txt_autoComplete_rubr_sempl_ingresso_multi.Enabled = true;
                    txt_autoComplete_rubr_sempl_ingresso_multi.Text  = "";
                    AutoComplete8.ContextKey = dataUser + "-" + idAmm + "-" + callType;
                    break;
            } 
        }

        public string CALLTYPE_RUBRICA_VELOCE
        {
            get
            {
                return this.GetStateValue("CALLTYPE_RUBRICA_VELOCE");
            }
            set
            {
                this.SetStateValue("CALLTYPE_RUBRICA_VELOCE", value);
            }
        }

        protected string GetStateValue(string key)
        {
            if (this.ViewState[key] != null)
                return this.ViewState[key].ToString();
            else
                return string.Empty;
        }

        protected void SetStateValue(string key, string obj)
        {
            this.ViewState[key] = obj;
        }

        public bool ReadOnly
        {
            set
            {
                this.txtAutoComplete_rubr.ReadOnly = value;
                this.txtAutoComplete_rubr_dest.ReadOnly = value;
                this.txt_autoComplete_rubr_sempl_ingresso.ReadOnly = value;
                this.txt_autoComplete_rubr_sempl_uscita_mitt.ReadOnly = value;
                this.txt_rubrica_veloce_destinatario_sempl.ReadOnly = value;
                this.txtAutoComplete_rubr_mitt_multiplo.ReadOnly = value;
                this.txt_autoComplete_rubr_sempl_ingresso_multi.ReadOnly = value;
            }
        }

        public System.Drawing.Color BackColor
        {
            set
            {
                this.txtAutoComplete_rubr.BackColor = value;
                this.txtAutoComplete_rubr_dest.BackColor = value;
                this.txt_autoComplete_rubr_sempl_ingresso.BackColor = value;
                this.txt_autoComplete_rubr_sempl_uscita_mitt.BackColor = value;
                this.txt_rubrica_veloce_destinatario_sempl.BackColor = value;
                this.txtAutoComplete_rubr_mitt_multiplo.BackColor = value;
                this.txt_autoComplete_rubr_sempl_ingresso_multi.BackColor = value;
            }
        }
    }
}