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
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.SitoAccessibile.Rubrica
{

	public class DettagliCorrispondente : SessionWebPage
	{
		protected System.Web.UI.HtmlControls.HtmlForm datiCorrispondente;
		protected System.Web.UI.WebControls.TextBox txt_cap;
		protected System.Web.UI.WebControls.Label lbl_user;
		protected System.Web.UI.WebControls.Button btnOccasionale;
		protected System.Web.UI.WebControls.Button btnBack;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_codiceRubrica;

		private string _idCorrispondente=string.Empty;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lbl_Denominazione;
		protected System.Web.UI.WebControls.TextBox txtUser;
		protected System.Web.UI.WebControls.TextBox txtCodiceRubrica;
		protected System.Web.UI.WebControls.TextBox txtIndirizzo;
		protected System.Web.UI.WebControls.TextBox txtCitta;
		protected System.Web.UI.WebControls.TextBox txtProvincia;
		protected System.Web.UI.WebControls.TextBox txtNazione;
		protected System.Web.UI.WebControls.TextBox txtTelprinc;
		protected System.Web.UI.WebControls.TextBox txtTelsecond;
		protected System.Web.UI.WebControls.TextBox txtFax;
		protected System.Web.UI.WebControls.TextBox txtFiscale;
		protected System.Web.UI.WebControls.TextBox txtEmail;
		protected System.Web.UI.WebControls.TextBox txtCodaoo;
		protected System.Web.UI.WebControls.TextBox txtCodadmin;
		protected System.Web.UI.WebControls.TextBox txtNote;
		protected System.Web.UI.WebControls.TextBox txtRicRitDestinatario;
		protected System.Web.UI.WebControls.TextBox txtRicRitCodiceAmm;
		protected System.Web.UI.WebControls.TextBox txtRicRitCodiceAOO;
		protected System.Web.UI.WebControls.TextBox txtRicRitDataProtocollo;
		protected System.Web.UI.WebControls.TextBox txtRicRitNumProtocollo;
		protected System.Web.UI.WebControls.TextBox txtRicRitDataSpedizione;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlRicevutaRitorno;
		protected System.Web.UI.WebControls.TextBox txtCap;

		private string _idProfile=string.Empty;

		private void Page_Load(object sender, System.EventArgs e)
		{	
			try
			{
				if (!this.IsPostBack)
				{
					this._idProfile=this.GetQueryStringParameter("iddoc");

					if (this._idProfile==string.Empty)
						throw new ApplicationException("Parametro 'idProfile' mancante");

					// Reperimento parametri query string
					this._idCorrispondente=this.GetQueryStringParameter("idCorrispondente");
				
					if (this._idCorrispondente!=string.Empty)
						this.Fetch();
					else
						throw new ApplicationException("Parametro 'idCorrispondente' mancante");
				}
			}
			catch(Exception ex)
			{
				ErrorManager.redirect(this,ex);
			}
		}

		/// <summary>
		/// Visualizzazione dati del corrispondente richiesto
		/// </summary>
		private void Fetch()
		{
			Corrispondente corrispondente=this.GetCorrispondente();

			this.txtUser.Text=corrispondente.descrizione;
			this.txtCodiceRubrica.Text=corrispondente.codiceRubrica;

			if (corrispondente.info!=null)
			{
				DocsPaVO.addressbook.DettagliCorrispondente details=new DocsPaVO.addressbook.DettagliCorrispondente();
				DocsPaUtils.Data.TypedDataSetManager.MakeTyped(corrispondente.info,details.Corrispondente.DataSet);

				this.txtIndirizzo.Text=details.Corrispondente[0].indirizzo;
				this.txtCap.Text=details.Corrispondente[0].cap;
				this.txtCitta.Text=details.Corrispondente[0].citta;
				this.txtProvincia.Text=details.Corrispondente[0].provincia;
				this.txtNazione.Text=details.Corrispondente[0].nazione;
				this.txtTelprinc.Text=details.Corrispondente[0].telefono;
				this.txtTelsecond.Text=details.Corrispondente[0].telefono2;
				this.txtFax.Text=details.Corrispondente[0].fax;
				this.txtFiscale.Text=details.Corrispondente[0].codiceFiscale;
			}

			this.txtEmail.Text=corrispondente.email;
			this.txtCodadmin.Text=corrispondente.codiceAmm;
			this.txtCodaoo.Text=corrispondente.codiceAOO;

			// Caricamento dati ricevuta di ritorno
			ProtocolloDestinatario protocolloDestinatario=this.GetProtocolloDestinatario(corrispondente);

			bool isVisible=(protocolloDestinatario!=null && 
							protocolloDestinatario.dta_spedizione!=null &&
							protocolloDestinatario.dta_spedizione!=string.Empty);

			this.SetVisibilityPanelProtocolloDestinatario(isVisible);
			
			if (isVisible)
				this.FetchPanelRicevutaRitorno(protocolloDestinatario);
		}

		/// <summary>
		/// Verifica se il corrispondente è occasionale
		/// </summary>
		/// <param name="corrispondente"></param>
		/// <returns></returns>
		private bool IsCorrispondenteOccasionale(Corrispondente corrispondente)
		{
			return (corrispondente.tipoCorrispondente!=null && corrispondente.tipoCorrispondente.ToUpper()=="O");
		}

		/// <summary>
		/// Reperimento oggetto corrispondente
		/// </summary>
		/// <returns></returns>
		private Corrispondente GetCorrispondente()
		{	
			DocsPaWR.DocsPaWebService ws=new DocsPAWA.DocsPaWR.DocsPaWebService();
			Corrispondente retValue=ws.AddressbookGetCorrispondenteBySystemId(this._idCorrispondente);

			if (!this.IsCorrispondenteOccasionale(retValue))
				// Reperimento dettagli rubrica, solo se non è occasionale
				retValue=UserManager.getCorrispondenteByCodRubrica(this,retValue.codiceRubrica);

			if (retValue.info==null)
				// Reperimento informazioni supplementari del corrispondente
				retValue.info=UserManager.getDettagliCorrispondente(this,this._idCorrispondente);

			return retValue;
		}

		/// <summary>
		/// Impostazione visibilità pannello protocollo destinatario
		/// </summary>
		/// <param name="isVisible"></param>
		private void SetVisibilityPanelProtocolloDestinatario(bool isVisible)
		{
			this.pnlRicevutaRitorno.Visible=isVisible;
		}

		/// <summary>
		/// Reperimento protocollo destinatario
		/// </summary>
		/// <param name="corrispondente"></param>
		/// <returns></returns>
		private ProtocolloDestinatario GetProtocolloDestinatario(Corrispondente corrispondente)
		{
			ProtocolloDestinatario retValue=null;

			if (!this.IsCorrispondenteOccasionale(corrispondente))
			{
				ProtocolloDestinatario[] protocolliDestinatari=DocumentManager.getDestinatariInteropAggConferma(this,this._idProfile,corrispondente);

				if (protocolliDestinatari.Length>0)
					retValue=protocolliDestinatari[0];
			}

			return retValue;
		}

		/// <summary>
		/// Caricamento dati ricevuta di ritorno
		/// </summary>
		/// <param name="protocolloDestinatario"></param>
		private void FetchPanelRicevutaRitorno(ProtocolloDestinatario protocolloDestinatario)
		{
			this.txtRicRitDestinatario.Text=protocolloDestinatario.descrizioneCorr;
			this.txtRicRitCodiceAmm.Text=protocolloDestinatario.codiceAmm;
			this.txtRicRitCodiceAOO.Text=protocolloDestinatario.codiceAOO;
			this.txtRicRitDataProtocollo.Text=protocolloDestinatario.dataProtocolloDestinatario;
			this.txtRicRitNumProtocollo.Text=protocolloDestinatario.protocolloDestinatario;
			this.txtRicRitDataSpedizione.Text=protocolloDestinatario.dta_spedizione;
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
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnBack_Click(object sender, System.EventArgs e)
		{
			Response.Redirect(BackUrl);
		}
	}
}
