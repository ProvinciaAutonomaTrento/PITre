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

namespace SAAdminTool.AdminTool.Gestione_RagioniTrasm
{
    public partial class messaggioNotifica : System.Web.UI.Page
    {
        string idRagione = String.Empty;
        string idAmministrazione = String.Empty;
        string codiceRagione = String.Empty;
        string descrRagione = string.Empty;
        protected Utilities.MessageBox msgBoxChiudiNotifica;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Expires = -1;
            //systemId della ragione di trasmissione
            idRagione = Request.QueryString["idRagione"].ToString();
            idAmministrazione = Request.QueryString["idAmm"].ToString();
            this.InitializeBusinessRuleControls();
            if (!IsPostBack)
            {
                // imposta griglie per la composizione del messaggio di notifica per doc e fasc
                this.ImpostaGrigliaNotifica();
               
                //ricavo la ragione di Trasmissione e ne estraggo i dati che mi interessano:
                // - testoMsgNotificaDoc
                // - testoMsgNotificaFasc
                // - idAmministrazione
                // - codiceRagione

                SAAdminTool.DocsPaWR.OrgRagioneTrasmissione ragione = GetRagioneTrasmissione(idRagione);
                this.txtMsgNotificaDoc.Text = ragione.testoMsgNotificaDoc.ToString();
                this.txtMsgNotificaFasc.Text = ragione.testoMsgNotificaFasc.ToString();
                
                codiceRagione = ragione.Codice;
                descrRagione = ragione.Descrizione;
                this.lblNomeRag.Text = codiceRagione.ToUpper();

                this.CurrentCodiceRagioneTrasmissione = codiceRagione;
                this.CurrentTestoNotificaDocumento = this.txtMsgNotificaDoc.Text;
                this.CurrentTestoNotificaFascicolo = this.txtMsgNotificaFasc.Text;

               
            }
            //aggiorno il contatore di caratteri a video
            Page.RegisterStartupScript("calcTest", "<script>calcTestoDoc('1024');calcTestoFasc('1024');</script>");
              
        }

        /// <summary>
        /// carica i valori per la segnatura nel datagrid
        /// </summary>
        private void ImpostaGrigliaNotifica()
        {
            DataTable dt = new DataTable("NOTIFICA_DOC");
            DataColumn dc;
            dc = new DataColumn("codice");
            dt.Columns.Add(dc);
            dc = new DataColumn("descrizione");
            dt.Columns.Add(dc);

            DataRow row;
            row = dt.NewRow();
            row["codice"] = "RAG_TRASM";
            row["descrizione"] = "Ragione di trasmissione";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "DEST_TRASM";
            row["descrizione"] = "Destinatario della trasmissione";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "NOTE_GEN";
            row["descrizione"] = "Note generali";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "NOTE_IND";
            row["descrizione"] = "Note individuali";
            dt.Rows.Add(row);

            
            row = dt.NewRow();
            row["codice"] = "SEGN_ID_DOC";
            row["descrizione"] = "Segnatura/Id del documento";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "OGG_DOC";
            row["descrizione"] = "Oggetto del documento";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "MITT_PROTO";
            row["descrizione"] = "Mittente del protocollo";
            dt.Rows.Add(row);

            dgNotificaDoc.DataSource = dt;
            dgNotificaDoc.DataBind();
            dgNotificaDoc.Visible = true;

            dt.Rows.Clear();

            
            row = dt.NewRow();
            row["codice"] = "RAG_TRASM";
            row["descrizione"] = "Ragione di trasmissione";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "DEST_TRASM";
            row["descrizione"] = "Destinatario della trasmissione";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "NOTE_GEN";
            row["descrizione"] = "Note generali";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "NOTE_IND";
            row["descrizione"] = "Note individuali";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "COD_FASC";
            row["descrizione"] = "Codice del fascicolo";
            dt.Rows.Add(row);

            row = dt.NewRow();
            row["codice"] = "DESC_FASC";
            row["descrizione"] = "Descrizione del fascicolo";
            dt.Rows.Add(row);

            dgNotificaFasc.DataSource = dt;
            dgNotificaFasc.DataBind();
            dgNotificaFasc.Visible = true;
        }

        protected void dgNotificaFasc_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            txtMsgNotificaFasc.Text += e.Item.Cells[1].Text;
           // string label = "<INPUT type='text' readonly value='" + e.Item.Cells[1].Text + "'></INPUT>";
            //txtMsgNotificaFasc.Text += label;
            SetFocus(txtMsgNotificaFasc);
            CalcTest(this.txtMsgNotificaFasc);
        }


        private void SetFocus(System.Web.UI.Control ctrl)
        {
            string s = "<SCRIPT language='javascript'>document.getElementById('" + ctrl.ID + "').focus() </SCRIPT>";
            RegisterStartupScript("focus", s);
        }					
		
        private void CalcTest(System.Web.UI.Control ctrl)
        {
            string s = "";
            if (ctrl.ID.ToUpper().Equals("TXTMSGNOTIFICADOC"))
            {
                s = "<SCRIPT language='javascript'>calcTestoDoc('1024');</SCRIPT>";
            }
            else
            {
                s = "<SCRIPT language='javascript'>calcTestoFasc('1024');</SCRIPT>";
            }
            RegisterStartupScript("calcTest", s);
          
        }					
		
        protected void dgNotificaDoc_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            txtMsgNotificaDoc.Text += e.Item.Cells[1].Text;
            SetFocus(txtMsgNotificaDoc);
            CalcTest(this.txtMsgNotificaDoc);
        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {

            if (this.txtMsgNotificaFasc.Text.Trim() == string.Empty || this.txtMsgNotificaDoc.Text.Trim() == string.Empty)
            {
                Page.RegisterStartupScript("Attenzione", "<script>alert('Attenzione, per il salvataggio dei dati è necessario\\ncompletare tutti i campi obbligatori');</script>");

                if (this.txtMsgNotificaFasc.Text.Trim() == string.Empty)
                    SetFocus(txtMsgNotificaFasc);
                if (this.txtMsgNotificaDoc.Text.Trim() == string.Empty)
                    SetFocus(txtMsgNotificaDoc);
            }
            else
            {
                salvaModifiche();
            }
        }


        private void salvaModifiche()
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            //prendo i dati per il salvataggio dei dati relativi alla ragione corrente

            bool allRagioniDoc = this.ckbDoc.Checked;
            bool allRagioniFasc = this.ckbFasc.Checked;

            SAAdminTool.DocsPaWR.ValidationResultInfo result = ws.UpdateMessageNotificaRagioneTrasmissione(this.CurrentCodiceRagioneTrasmissione, idAmministrazione, this.txtMsgNotificaDoc.Text, this.txtMsgNotificaFasc.Text, allRagioniDoc, allRagioniFasc);

            if (!result.Value)
            {
                ShowValidationMessage(result);
            }
            else
            {
                this.ckbDoc.Checked = false;
                this.ckbFasc.Checked = false;
                this.CurrentTestoNotificaDocumento = this.txtMsgNotificaDoc.Text;
                this.CurrentTestoNotificaFascicolo = this.txtMsgNotificaFasc.Text;
            }
        }

        protected SAAdminTool.DocsPaWR.OrgRagioneTrasmissione GetRagioneTrasmissione(string idRagione)
        {
            AmmUtils.WebServiceLink ws = new AmmUtils.WebServiceLink();
            return ws.GetRagioneTrasmissione(idRagione);
        }

        private string GetValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult,
			out Control firstInvalidControl,
			out bool warningMessage)
		{
			string retValue=string.Empty;
			bool errorMessage=false;
			firstInvalidControl=null;

			foreach (SAAdminTool.DocsPaWR.BrokenRule rule in validationResult.BrokenRules)
			{
                if (!errorMessage && rule.Level == SAAdminTool.DocsPaWR.BrokenRuleLevelEnum.Error)
					errorMessage=true;

				if (retValue!=string.Empty)
					retValue+="\\n";

				retValue += " - " + rule.Description;

				if (firstInvalidControl==null)
					firstInvalidControl=this.GetBusinessRuleControl(rule.ID);
			}

			if (errorMessage)
				retValue="Sono state riscontrate le seguenti anomalie:\\n\\n" + retValue;
			else
				retValue="Attenzione:\\n\\n" + retValue;

			warningMessage=!errorMessage;
			
			return retValue.Replace("'","\\'");
		}

        	/// <summary>
		/// Visualizzazione messaggi di validazione
		/// </summary>
		/// <param name="validationResult"></param>
		private void ShowValidationMessage(SAAdminTool.DocsPaWR.ValidationResultInfo validationResult)
		{
			// Visualizzazione delle regole di business non valide
			bool warningMessage;
			Control firstInvalidControl;

			string validationMessage=this.GetValidationMessage(validationResult,out firstInvalidControl,out warningMessage);

			this.RegisterClientScript("ShowValidationMessage","ShowValidationMessage('" + validationMessage + "'," + warningMessage.ToString().ToLower() + ");");

			if (firstInvalidControl!=null)
				this.SetFocus(firstInvalidControl);
		}

        /// <summary>
        /// Hashtable businessrules
        /// </summary>
        private Hashtable _businessRuleControls = null;

        /// <summary>
        /// Inizializzazione hashtable per le businessrule:
        /// - Key:		ID della regola di business
        /// - Value:	Controllo della UI contenente il 
        ///				dato in conflitto con la regola di business
        /// </summary>
        private void InitializeBusinessRuleControls()
        {
            this._businessRuleControls = new Hashtable();

            this._businessRuleControls.Add("DB_ERROR", this.txtMsgNotificaDoc);
          
        }

        private Control GetBusinessRuleControl(string idBusinessRule)
        {
            return this._businessRuleControls[idBusinessRule] as Control;
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


        /// <summary>
        /// Reperimento e settaggio codice ragione trasmissione corrente
        /// </summary>
        private string CurrentCodiceRagioneTrasmissione
        {
            get
            {
                if (this.ViewState["CurrentCodiceRagioneTrasmissione"] != null)
                    return this.ViewState["CurrentCodiceRagioneTrasmissione"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["CurrentCodiceRagioneTrasmissione"] = value;
            }
        }

        /// <summary>
        /// Reperimento e settaggio del testo per la notifica dei documenti
        /// </summary>
        private string CurrentTestoNotificaDocumento
        {
            get
            {
                if (this.ViewState["CurrentTestoNotificaDocumento"] != null)
                    return this.ViewState["CurrentTestoNotificaDocumento"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["CurrentTestoNotificaDocumento"] = value;
            }
        }

        /// <summary>
        /// Reperimento e settaggio del testo per la notifica dei documenti
        /// </summary>
        private string CurrentTestoNotificaFascicolo
        {
            get
            {
                if (this.ViewState["CurrentTestoNotificaFascicolo"] != null)
                    return this.ViewState["CurrentTestoNotificaFascicolo"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                this.ViewState["CurrentTestoNotificaFascicolo"] = value;
            }
        }

        protected void btnChiudi_Click(object sender, EventArgs e)
        {
            string oldValueDoc = this.CurrentTestoNotificaDocumento.Replace("\r\n", "\n"); ;
            string newValueDoc = this.txtMsgNotificaDoc.Text.Replace("\r\n", "\n");


            string oldValueFasc = this.CurrentTestoNotificaFascicolo.Replace("\r\n", "\n"); ;
            string newValueFasc = this.txtMsgNotificaFasc.Text.Replace("\r\n", "\n"); 
            string messageConfirm = string.Empty;



            //se il testo non è cambiato e chiudo, allora chiudo la pagina, alrim avviso l'utente
            if (((newValueDoc == oldValueDoc) && (newValueFasc == oldValueFasc)) || (newValueDoc.Trim() == string.Empty || newValueFasc.Trim() == string.Empty))
            {
           
                if (newValueDoc == oldValueDoc && newValueFasc == oldValueFasc &&  newValueDoc.Trim() != string.Empty && newValueFasc.Trim() != string.Empty )
                {
                    if (this.ckbDoc.Checked && this.ckbFasc.Checked)
                    {
                        messageConfirm = InitMessageXml.getInstance().getMessage("CLOSE_NOTIFICA_DOCFASC");  
                       // messageConfirm = "Prima di chiudere, si vuole salvare il testo\\nper la notifica dei documenti e la notifica dei fascicoli a tutte le ragioni?";
                    }
                    else
                    {
                        if (this.ckbDoc.Checked)
                        {
                            messageConfirm = InitMessageXml.getInstance().getMessage("CLOSE_NOTIFICA_DOC");  
                      
                            //messageConfirm = "Prima di chiudere, si vuole salvare il testo\\nper la notifica dei documenti a tutte le ragioni?";
                        }
                        else
                        {
                            if (this.ckbFasc.Checked)
                            {
                                messageConfirm = InitMessageXml.getInstance().getMessage("CLOSE_NOTIFICA_FASC"); 
                               // messageConfirm = "Prima di chiudere, si vuole salvare il testo\\nper la notifica dei fascicoli a tutte le ragioni?";
                            }

                            else
                            {
                                Page.RegisterStartupScript("close", "<script>window.close();</script>");
                                return;
                            }
                        }
                    }
                    msgBoxChiudiNotifica.Confirm(messageConfirm);
                }
                else
                {
                    Page.RegisterStartupScript("close", "<script>window.close();</script>");
                }
            }
            else
            {
             
                if ((newValueDoc != oldValueDoc) && (newValueFasc != oldValueFasc))
                {
                    //sono cambiati entrambi i testi
                    messageConfirm = InitMessageXml.getInstance().getMessage("SAVE_NOTIFICA_FASC"); 
                              
                   // messageConfirm = "Le modifiche effettuate per il testo della:\\n- notifica dei documenti\\n- notifica dei fascicoli\\nnon sono state salvate.\\n\\nVuoi salvarle?";
                }
                else
                {
                    if (newValueDoc != oldValueDoc)
                    {
                        //è cambiato solamente il testo per i documenti
                        messageConfirm = InitMessageXml.getInstance().getMessage("SAVE_NOTIFICA_DOC"); 
                    
                        //messageConfirm = "Le modifiche effettuate per il testo della notifica documenti\\nnon sono state salvate.\\n\\nVuoi salvarle?";
                    }
                    else if (newValueFasc != oldValueFasc)
                    {
                        //è cambiato solamente il testo per i fascicoli
                        messageConfirm = InitMessageXml.getInstance().getMessage("SAVE_NOTIFICA_FASC"); 
                      //  messageConfirm = "Le modifiche effettuate per il testo della notifica fascicoli\\nnon sono state salvate.\\n\\nVuoi salvarle?";
                    }
                }

                msgBoxChiudiNotifica.Confirm(messageConfirm);

            }   
        }

        protected void msgBoxChiudiNotifica_GetMessageBoxResponse(object sender, Utilities.MessageBox.MessageBoxEventHandler e)
        {
            if (e.ButtonPressed == Utilities.MessageBox.MessageBoxEventHandler.Button.Ok)
            {
                salvaModifiche();
            }
            else
            { 
                Page.RegisterStartupScript("close", "<script>window.close();</script>");
             
            }
        }

    }
}
