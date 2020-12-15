using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.CheckInOut;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    /// <summary>
    /// Summary description for rimuoviProfilo.
    /// </summary>
    public class rimuoviProfilo : DocsPAWA.CssPage
    {
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Button btn_annulla;
        protected System.Web.UI.WebControls.Label lbl_result;
        protected System.Web.UI.WebControls.Label lbl_messageCheckOut;
        protected System.Web.UI.WebControls.Label lbl_messageOwnerCheckOut;
        protected System.Web.UI.WebControls.Label lbl_note;
        protected System.Web.UI.WebControls.Label lbl_mess_conf_rimuovi;
        protected System.Web.UI.WebControls.TextBox txt_note;
        //protected Utilities.MessageBox Msg_Rimuovi;
        protected string result;
        private void Page_Load(object sender, System.EventArgs e)
        {
            result = "";
            Response.Expires = -1;
            this.txt_note.Focus();
            if (!this.IsPostBack)
            {
                this.btn_annulla.Attributes["onClick"] = "ClosePage(false);";

                DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

                string ownerUser;

                bool isCheckedOut = CheckInOutServices.IsCheckedOutDocumentWithUser(out ownerUser);

                // Verifica se il documento è in stato checkout
                if (isCheckedOut)
                {
                    this.IsOwnerCheckedOut = (ownerUser.ToUpper() == UserManager.getInfoUtente().userId.ToUpper());

                    this.lbl_messageCheckOut.Visible = !this.IsOwnerCheckedOut;
                    this.lbl_messageOwnerCheckOut.Visible = this.IsOwnerCheckedOut;

                    this.btn_ok.Visible = this.IsOwnerCheckedOut;
                    this.lbl_note.Visible = false;
                    this.txt_note.Visible = false;
                    this.lbl_result.Text = string.Empty;
                }
                else
                {
                    this.lbl_messageCheckOut.Visible = false;
                    this.lbl_messageOwnerCheckOut.Visible = false;

                    this.lbl_mess_conf_rimuovi.Visible = true;

                    result = DocumentManager.verificaDirittiCestinaDocumento(this, schedaDocumento);

                    this.lbl_note.Visible = (result == "Del");
                    //luluciani 17/06/2008
                    this.btn_ok.Visible = (result == "Del");
                    this.lbl_mess_conf_rimuovi.Visible = (result == "Del");


                    this.txt_note.Visible = this.lbl_note.Visible;

                    if (!this.lbl_note.Visible)
                    {
                        this.lbl_result.Text = result;

                    }
                }
            }
            tastoInvio();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // this.Msg_Rimuovi.GetMessageBoxResponse += new Utilities.MessageBox.Message(this.Msg_Rimuovi_GetMessageBoxResponse);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion

        public void tastoInvio()
        {
            Utils.DefaultButton(this, ref txt_note, ref btn_ok);
        }
        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            //Conferma utente per la rimozione del documento
            // string messaggio = InitMessageXml.getInstance().getMessage("RIMUOVI_DOCUMENTO");
            // Msg_Rimuovi.CssClass = "MeggageBox";
            //Msg_Rimuovi.Confirm(messaggio);
            RimuoviDoc();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsOwnerCheckedOut
        {
            get
            {
                if (this.ViewState["IsOwnerCheckedOut"] == null)
                    return false;
                else
                    return Convert.ToBoolean(this.ViewState["IsOwnerCheckedOut"]);
            }
            set
            {
                this.ViewState["IsOwnerCheckedOut"] = value;
            }
        }

        /// <summary>
        /// Registrazione script client
        /// </summary>
        /// <param name="scriptKey"></param>
        /// <param name="scriptValue"></param>
        private void RegisterClientScript(string scriptKey, string scriptValue)
        {
            if (!this.Page.IsStartupScriptRegistered(scriptKey))
            {
                string scriptString = "<SCRIPT>" + scriptValue + "</SCRIPT>";
                this.Page.RegisterStartupScript(scriptKey, scriptString);
            }
        }

        #region Message-Box
        //private void Msg_Rimuovi_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        //{
        //    if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
        //    {
        //        RimuoviDoc();
        //    }
        //}
        private bool checkDati()
        {
            if (txt_note.Text.Length > 256)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "note", "alert('Il campo note può contenere al massimo 256 caratteri.');", true);
                return false;
            }
            return true;
        }

        //Chiamata al metodo per la rimozione del documento grigio o predisposto
        private void RimuoviDoc()
        {
            try
            {
                string prov = Request.QueryString["nomeForm"];

                if (checkDati())
                {
                    DocsPaWR.SchedaDocumento schedaDocumento = DocumentManager.getDocumentoSelezionato(this);
                    //bool cestina = DocumentManager.CestinaDocumento(this, schedaDocumento, "P", txt_note.Text);
                    string errorMsg = string.Empty;
                    bool cestina = DocumentManager.CestinaDocumento(this, schedaDocumento, "P", txt_note.Text, out errorMsg);

                    if (cestina)
                    {
                        schedaDocumento.inCestino = "1";

                        //Si è deciso di rimuovere il documento dalla scheda del documento stesso
                        if (prov == "docProfilo")
                        {
                            //   Response.Write("<script>window.opener.parent.location.href='../documento/tabGestioneDoc.aspx?tab=profilo'; window.close();</script>");
                        }
                        else if (prov == "toDoList")
                        {
                            //Si è deciso di rimuovere il documento dalla todolist
                            //  string script = "<script>window.opener.parent.location.href='../sceltaRuoloNew.aspx'; window.close();</script>";


                            // Response.Write(script);
                        }
                        bool returnValue = cestina;
                        Response.Write("<script>window.returnValue = " + returnValue.ToString().ToLower() + ";window.close();</script>");
                    }
                    else
                    {
                        this.lbl_result.Text = errorMsg;
                        lbl_mess_conf_rimuovi.Visible = false;
                        this.lbl_note.Visible = false;
                        this.btn_ok.Visible = false;
                        this.txt_note.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(this, ex);
            }
        }
        #endregion

    }
}
