using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DocsPAWA.CheckInOut;

namespace DocsPAWA.popup
{
	/// <summary>
	/// Summary description for rimuoviAllegato.
	/// </summary>
    public class rimuoviAllegato : DocsPAWA.CssPage
	{
		protected System.Web.UI.WebControls.Label lbl_message;
		protected System.Web.UI.WebControls.Button btn_ok;
		protected System.Web.UI.WebControls.Button btn_annulla;
		protected System.Web.UI.WebControls.Label lbl_messageCheckOut;
		protected System.Web.UI.WebControls.Label lbl_result;
        
        /// <summary>
        /// Oggetto per la verifica delle acl sul documenot
        /// </summary>
        protected documento.AclDocumento aclDocumento;

        /// <summary>
        /// Messaggi di alert
        /// </summary>
        private const string MSG_CONFERMA_RIMUOVI = "Rimuovere l'allegato selezionato?";
        private const string MSG_ACL_REVOCATA = "Sono stati tolti i diritti di visibilità per questo documento.<br />Impossibile rimuovere l'allegato selezionato.";
        private const string MSG_REMOVE_ERROR = "Si è verificato un errore nella rimozione dell'allegato";

        /// <summary>
        /// Scheda documento corrente
        /// </summary>
        private DocsPaWR.SchedaDocumento _schedaDocumento = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void Page_Load(object sender, System.EventArgs e)
		{
            this.Response.Expires = -1;

            // Reperimento documento selezionato
            this._schedaDocumento = DocumentManager.getDocumentoSelezionato(this);

            if (!this.IsPostBack)
            {
                this.btn_ok.Attributes.Add("onclick", "window.document.body.style.cursor = 'wait'");
                this.btn_annulla.Attributes.Add("onclick", "CloseMask(false);");
                
                bool canRemove = true;

                string message = string.Empty;

                canRemove = !this.IsAclRevocata(out message);

                if (canRemove)
                    canRemove = !this.IsCheckedOutDocument(out message);

                if (canRemove)
                    message = MSG_CONFERMA_RIMUOVI;

                this.btn_ok.Enabled = canRemove;
                this.lbl_message.Text = message;
            }
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

        /// <summary>
        /// Verifica se la visibilità sul documento è stata revocata
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool IsAclRevocata(out string message)
        {
            message = string.Empty;

            // Verifica revoca acl sul documento
            this.aclDocumento.IdDocumento = this._schedaDocumento.systemId;
            this.aclDocumento.VerificaRevocaAcl();
            this.aclDocumento.ShowDefaultMessageAclRevocata = false;

            if (this.aclDocumento.AclRevocata)
                message = MSG_ACL_REVOCATA;

            return this.aclDocumento.AclRevocata;
        }

        /// <summary>
        /// Verifica se il documento è in stato checkout
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool IsCheckedOutDocument(out string message)
        {
            // Verifica stato documento principale
            bool retValue = CheckInOutServices.IsCheckedOutDocument(out message);

            if (!retValue)
            {
                // Verifica stato allegato selezionato per la rimozione il quale, se in checkout, non può essere elmininato
                string docNumber = Request.QueryString["docNumber"];

                retValue = CheckInOutServices.IsCheckedOutDocument(docNumber, docNumber, UserManager.getInfoUtente(), false);

                if (retValue)
                    message = "L'allegato selezionato risulta bloccato.";
            }

            return retValue;
        }

        /// <summary>
        /// Reperimento allegato da rimuovere
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.Allegato GetAllegatoSelezionato()
        {
            DocsPaWR.Allegato retValue = null;

            
            if (_schedaDocumento.allegati.Length == 0)
            {
                DocsPAWA.DocsPaWR.Allegato[] lista = DocumentManager.getAllegatiPerRimozione(_schedaDocumento.documentoPrincipale.docNumber);
                if (lista != null && lista.Length > 0)
                {
                    foreach (DocsPaWR.Allegato item in lista)
                    {
                        if (item.docNumber == _schedaDocumento.docNumber)
                        {
                            retValue = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                string docNumber = Request.QueryString["docNumber"];
                string versionId = Request.QueryString["versionId"];

                foreach (DocsPaWR.Allegato item in this._schedaDocumento.allegati)
                {
                    if (item.versionId == versionId)
                    {
                        retValue = item;
                        break;
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btn_ok_Click(object sender, System.EventArgs e)
		{
			try 
			{
                DocsPaWR.Allegato allegato = this.GetAllegatoSelezionato();
                
                bool removed = DocumentManager.rimuoviAllegato(allegato, this._schedaDocumento);

                if (!removed)
                {
                    this.RenderMessage(MSG_REMOVE_ERROR);
                }
                else
                {
                    //Se vengo dal dettaglio documento e sto rimuovendo un allegato
                    if (_schedaDocumento.allegati.Length == 0)
                    {
                        _schedaDocumento.inCestino = "1";
                        bool returnValue = removed;
                        Response.Write("<script>window.returnValue = " + returnValue.ToString().ToLower() + ";window.close();</script>");
                   
                    }
                    else
                    {
                        //Rimozione dell'allegato dal tab allegati
                        //pulisco anche la parte di destra 
                        FileManager.removeSelectedFile(this);

                        List<DocsPaWR.Allegato> allegati = new List<DocsPAWA.DocsPaWR.Allegato>(this._schedaDocumento.allegati);
                        allegati.Remove(allegato);
                        this._schedaDocumento.allegati = allegati.ToArray();
                    }
                    this.CloseMask(true);
                }
			} 
			catch (Exception ex) 
			{
				ErrorManager.redirect(this, ex);
			}		
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected virtual void RenderMessage(string message)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "RenderMessage", string.Format("<script>alert('{0}')</script>", message.Replace("'", @"\'")));
        }

        // <summary>
        /// 
        /// </summary>
        /// <param name="retValue"></param>
        protected void CloseMask(bool retValue)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseMask", string.Format("<script>CloseMask({0});</script>", retValue.ToString().ToLower()));
        }

	}
}
