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

namespace DocsPAWA.popup
{
	/// <summary>
	/// Maschera per la gestione dell'allegato
	/// </summary>
	public class gestioneAllegato : DocsPAWA.CssPage
	{   
        protected System.Web.UI.WebControls.Label LabelTitolo;
        protected System.Web.UI.WebControls.Label LabelMessage;
        protected System.Web.UI.WebControls.Label LabelCodice;
        protected System.Web.UI.WebControls.TextBox TextCodice;
        protected System.Web.UI.WebControls.Label LabelDescrizione;
        protected System.Web.UI.WebControls.TextBox TextDescrizione;        
        protected System.Web.UI.WebControls.Label LabelNumPag;
        protected System.Web.UI.WebControls.TextBox TextNumPag;
        protected System.Web.UI.WebControls.Button btn_ok;
        protected System.Web.UI.WebControls.Button btn_chiudi;
        protected bool _isCheckedOutDocument = false;
        protected System.Web.UI.HtmlControls.HtmlInputControl clTesto;
        protected  int caratteriDisponibili=2000 ;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, System.EventArgs e)
        {
            this.Response.Expires = -1;

            if (!this.IsPostBack)
            {
                DocsPAWA.DocsPaWR.InfoUtente info = new DocsPAWA.DocsPaWR.InfoUtente();
                info = UserManager.getInfoUtente(this.Page);


                string valoreChiave = utils.InitConfigurationKeys.GetValue(info.idAmministrazione, "FE_MAX_LENGTH_DESC_ALLEGATO");
                if (!string.IsNullOrEmpty(valoreChiave))
                    caratteriDisponibili = int.Parse(valoreChiave);

                TextDescrizione.MaxLength = caratteriDisponibili;
                clTesto.Value = caratteriDisponibili.ToString();
                TextDescrizione.Attributes.Add("onKeyUp", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                TextDescrizione.Attributes.Add("onchange", "calcTesto(this,'" + caratteriDisponibili.ToString() + " ','DESCRIZIONE'," + clTesto.ClientID + ")");
                if(DocumentManager.getBlockAllegati(this) != null)
                    DocumentManager.setBlockAllegati(this, false);
                this.btn_ok.Attributes.Add("onclick", "window.document.body.style.cursor = 'wait'");

                // Reperimento allegato selezionato
                DocsPaWR.Allegato allegato = this.GetSelectedAllegato();

                this._isCheckedOutDocument = (CheckInOut.CheckInOutServices.IsCheckedOutDocument());

                if (!this._isCheckedOutDocument)
                {
                    if (allegato == null)
                    {
                        // Modalità di inserimento
                        this.FetchNew();
                    }
                    else
                    {
                        // Modalità di modifica dati
                        this.Fetch(allegato);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_PreRender(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
                this.EnableControls();

            this.btn_ok.Attributes.Add("ondblclick", "this.disabled = true;"); 
        }

        /// <summary>
        /// 
        /// </summary>
        protected void EnableControls()
        {
            if (this._isCheckedOutDocument)
            {
                this.LabelMessage.Visible = true;
                this.LabelCodice.Visible = false;
                this.LabelDescrizione.Visible = false;
                this.LabelNumPag.Visible = false;
                this.TextCodice.Visible = false;
                this.TextDescrizione.Visible = false;
                this.TextNumPag.Visible = false;
                this.btn_ok.Visible = false;
            }
            else
            {
                bool insertMode = string.IsNullOrEmpty(this.VersionId);
                this.LabelMessage.Visible = false;
                this.LabelCodice.Visible = !insertMode;
                this.TextCodice.Visible = !insertMode;
            }
        }

        /// <summary>
        /// Reperimento scheda documento corrente
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.SchedaDocumento GetSchedaDocumento()
        {
            return DocumentManager.getDocumentoSelezionato(this);
        }

        /// <summary>
        /// 
        /// </summary>
        protected string VersionId
        {
            get
            {
                return Request.QueryString["versionId"];
            }
        }

        /// <summary>
        /// Reperimento allegato selezionato per la modifica dei dati
        /// </summary>
        /// <returns></returns>
        protected DocsPaWR.Allegato GetSelectedAllegato()
        {
            DocsPaWR.Allegato retValue = null;

            string versionId = this.VersionId;

            if (!string.IsNullOrEmpty(versionId))
            {
                DocsPaWR.SchedaDocumento schedaDocumento = this.GetSchedaDocumento();

                if (schedaDocumento != null)
                {
                    foreach (DocsPaWR.Allegato allegato in schedaDocumento.allegati)
                    {
                        if (allegato.versionId.Equals(versionId))
                        {
                            retValue = allegato;
                            break;
                        }
                    }
                }
            }

            return retValue;
        }

        /// <summary>
        /// Associazione dati per l'inserimento
        /// </summary>
        private void FetchNew()
        {
            this.LabelTitolo.Text = "Inserimento allegato";
            this.TextCodice.Text = string.Empty;
            this.TextDescrizione.Text = string.Empty;
            this.TextNumPag.Text = string.Empty;
        }

        /// <summary>
        /// Associazione dati
        /// </summary>
        /// <param name="allegato"></param>
        private void Fetch(DocsPaWR.Allegato allegato)
        {
            this.LabelTitolo.Text = "Modifica allegato";
            this.TextCodice.Text = allegato.versionLabel;
            this.TextDescrizione.Text = allegato.descrizione;
            this.TextNumPag.Text = allegato.numeroPagine.ToString();            
        }

        /// <summary>
        /// Validazione dati maschera
        /// </summary>
        /// <param name="validationMessage"></param>
        /// <returns></returns>
        protected virtual bool IsValidMask(out string validationMessage)
        {
            validationMessage = string.Empty;

            if (this.TextDescrizione.Text.Trim() == string.Empty)
                validationMessage = "Descrizione: campo obbligatorio";

            if (this.TextNumPag.Text != string.Empty)
            {
                int numeroPagine;
                if (!Int32.TryParse(this.TextNumPag.Text, out numeroPagine))
                {
                    if (validationMessage != string.Empty)
                        validationMessage += Environment.NewLine;
                    validationMessage = "Numero pagine: inserire un valore numerico";
                }
            }

            return (string.IsNullOrEmpty(validationMessage));
        }

        /// <summary>
        /// 
        /// </summary>
        private int NumeroPagine
        {
            get
            {
                int numeroPagine;
                Int32.TryParse(this.TextNumPag.Text, out numeroPagine);
                return numeroPagine;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ok_Click(object sender, System.EventArgs e)
        {
            // Se il documento è bloccato, non si può modificare
            if (this.IsSelectedDocumentCheckedOut())
                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "NonModificabile",
                    "alert('Non è possibile salvare le modifiche in quanto il documento principale oppure almeno uno dei suoi allegati risulta bloccato');",
                    true);
            else
                this.ProceedWithSave();

        }

        private bool IsSelectedDocumentCheckedOut()
        {
            // Reperimento del documento selezionato
            DocsPAWA.DocsPaWR.SchedaDocumento doc = DocumentManager.getDocumentoSelezionato(this);

            return CheckInOut.CheckInOutServices.IsCheckedOutDocument(
                doc.systemId,
                doc.docNumber,
                UserManager.getInfoUtente(),
                true);

        }

        /// <summary>
        /// Funzione per il salvataggio dell'allegato
        /// </summary>
        private void ProceedWithSave()
        {
            string validationMessage;

            if (this.IsValidMask(out validationMessage))
            {
                if (DocumentManager.getBlockAllegati(this) != null && !DocumentManager.getBlockAllegati(this))
                {
                    DocumentManager.setBlockAllegati(this, true);
                    DocsPaWR.Allegato allegato = this.SaveAllegato();

                    if (allegato != null)
                    {
                        this.CloseMask(allegato.versionId);
                    }
                    else
                    {
                        this.RenderMessage("Errore nell'inserimento dell'allegato");
                    }
                }
            }
            else
                this.RenderMessage(validationMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionId"></param>
        protected void CloseMask(string versionId)
        {
            DocumentManager.removeBlockAllegati(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseMask", string.Format("<script>CloseMask('{0}');</script>", versionId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected virtual void RenderMessage(string message)
        {
            this.Response.Write(string.Format("<script>alert('{0}')</script>", message.Replace("'", @"\'")));
        }

        /// <summary>
        /// Task di salvataggio dell'allegato
        /// </summary>
        protected virtual DocsPaWR.Allegato SaveAllegato()
        {
            DocsPaWR.Allegato allegato = null;

            try
            {
                DocsPaWR.SchedaDocumento schedaDocumento = this.GetSchedaDocumento();

                // Reperimento oggetto allegato
                allegato = this.GetSelectedAllegato();

                if (allegato == null)
                {
                    // Modalità di inserimento
                    allegato = new DocsPaWR.Allegato();
                    allegato.descrizione = this.TextDescrizione.Text;
                    allegato.numeroPagine = this.NumeroPagine;
                    allegato.docNumber = schedaDocumento.docNumber;
                    allegato.version = "0";

                    string idPeopleDelegato = "0";
                    if (UserManager.getInfoUtente().delegato != null)
                        idPeopleDelegato = UserManager.getInfoUtente().delegato.idPeople;
                    allegato.idPeopleDelegato = idPeopleDelegato;

                    // Impostazione del repositoryContext associato al documento
                    allegato.repositoryContext = schedaDocumento.repositoryContext;
                    allegato.position = (schedaDocumento.allegati.Length + 1);
                    allegato = DocumentManager.aggiungiAllegato(allegato);
                    
                    // Inserimento dell'allegato nella scheda documento
                    List<DocsPaWR.Allegato> allegati = new List<DocsPaWR.Allegato>(schedaDocumento.allegati);
                    allegati.Add(allegato);
                    schedaDocumento.allegati = allegati.ToArray();
                }
                else
                {
                    // Modifica dati
                    allegato.descrizione = this.TextDescrizione.Text;
                    allegato.numeroPagine = this.NumeroPagine;
                    
                    DocumentManager.modificaAllegato(this, allegato,schedaDocumento.docNumber);
                    DocumentManager.setDocumentoSelezionato(this, schedaDocumento);
                }
            }
            catch (Exception ex)
            {
                allegato = null;
            }

            return allegato;
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
            this.PreRender += new System.EventHandler(this.Page_PreRender);
            this.btn_chiudi.Click += new EventHandler(btn_chiudi_Click);
		}

        void btn_chiudi_Click(object sender, EventArgs e)
        {
            DocumentManager.removeBlockAllegati(this);
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "CloseMask", "<script>window.close();</script>");
		}

		#endregion
	}
}
