using System;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for SessionWebPage.
	/// </summary>
	public class SessionWebPage : WebPage
	{
		private bool sessionValid = false;
		private string sessionMsg = null;

		private const string SESSION_DROPPED_MSG = "Sessione Utente Terminata";
		private const string SESSION_EXPIRED_MSG = "Sessione Utente Scaduta";
		private const string SESSION_ERROR_MSG = "Sessione Utente Non Valida";

		public SessionWebPage()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		protected override void OnLoad(EventArgs e)
		{
			// Controllo validità delle sessione utente
			this.CheckSessionValidity();

			if (!this.IsPostBack && Request.UrlReferrer!=null)
				// Impostazione dell'url della pagina chiamante
				this.BackUrl=Request.UrlReferrer.AbsoluteUri;

			base.OnLoad (e);
		}
		/// <summary>
		/// Reperimento info supplementari da aggiungere al titolo della pagina
		/// </summary>
		/// <returns></returns>
		protected virtual string GetTitleExtraInformation()
		{
			return string.Empty;
		}

		public void CheckSessionValidity()
		{
			try
			{
				DocsPaWR.Utente usr = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
				DocsPaWR.ValidationResult vr = 
					UserManager.ValidateLogin(usr.userId,usr.idAmministrazione,Session.SessionID);
				switch (vr)
				{
					case DocsPAWA.DocsPaWR.ValidationResult.OK:
						sessionValid = true;
						sessionMsg = null;
						break;
					case DocsPAWA.DocsPaWR.ValidationResult.SESSION_DROPPED:
						sessionValid = false;
						sessionMsg = SESSION_DROPPED_MSG;
						break;
					case DocsPAWA.DocsPaWR.ValidationResult.SESSION_EXPIRED:
						sessionValid = false;
						sessionMsg = SESSION_EXPIRED_MSG;
						break;
					default:
						sessionValid = false;
						sessionMsg = SESSION_ERROR_MSG;
						break;
				}
			}
			catch (Exception)
			{
				sessionValid = false;
				sessionMsg = SESSION_ERROR_MSG;
			}

			if (!sessionValid)
			{
				string url = this.GetBaseUrl()+"/Login.aspx?error="+sessionMsg;
				Response.Redirect(url,true);
			}
		}


		public void QuickSearch()
		{
			if (this.Context.Request.Params["txtsearch"]!=null)
			{
				string txtsearch = (string)this.Context.Request.Params["txtsearch"];
				//Esegui ricerca veloce e reindirizza alla pagina di risultato
			}
		}

		public override void Create(System.Web.SessionState.HttpSessionState mySession)
		{
			base.Create(mySession);
		}

		public bool isSessionValid
		{
			get { return sessionValid; }
		}

		public string Message
		{
			get { return sessionMsg; }
		}

		public bool HasFunction(string codFunzione)
		{
			bool found = false;

			DocsPaWR.Ruolo ruolo = GetCurrentRule();
			if (ruolo!=null)
			{
				DocsPaWR.Funzione[] funzioni = ruolo.funzioni;
				if (funzioni!=null)
				{
					for (int i=0; !found && i<funzioni.Length; i++)
					{
						if (funzioni[i].codice == codFunzione)
							found = true;
					}
				}
			}

			return found;
		}

		public DocsPAWA.DocsPaWR.Ruolo GetCurrentRule()
		{
			DocsPaWR.Ruolo ruolo = null;
			DocsPaWR.Utente utente = (DocsPAWA.DocsPaWR.Utente)Session["userData"];
			if (utente!=null && utente.ruoli!=null)
			{
				bool found = false;
				for (int i=0; !found && i<utente.ruoli.Length; i++)
				{
					ruolo = utente.ruoli[i];
					if (ruolo.selezionato)
						found = true;
				}
				if (!found)
					ruolo = null;
			}
			return ruolo;
		}

		public DocsPAWA.DocsPaWR.Utente GetCurrentUser()
		{
			return (DocsPAWA.DocsPaWR.Utente)Session["userData"];
		}

		public DocsPAWA.DocsPaWR.UnitaOrganizzativa GetCurrentUnit()
		{
			DocsPaWR.UnitaOrganizzativa uo = null;
			DocsPaWR.Ruolo ruolo = GetCurrentRule();
			if (ruolo!=null)
			{
				uo = ruolo.uo;
			}
			return uo;
		}

		public DocsPAWA.DocsPaWR.Registro GetCurrentRegistry()
		{
			return UserManager.getRegistroSelezionato(this.Page);
/*
			DocsPaWR.Registro reg = null;
			DocsPaWR.Ruolo ruolo = GetCurrentRule();
			if (ruolo!=null)
			{
				bool found = false;
				for (int i=0; !found && i<ruolo.registri.Length; i++)
				{
					reg = ruolo.registri[i];
					if (reg.systemId==ruolo.idRegistro)
						found = true;
				}
				if (!found)
					reg = null;
			}
			return reg;
*/			
		}

		/// <summary>
		/// Reperimento valore parametro querystring
		/// </summary>
		/// <param name="paramName"></param>
		/// <returns></returns>
		protected string GetQueryStringParameter(string paramName)
		{
			string retValue=string.Empty;
			if (Request.QueryString[paramName]!=null)
				retValue=Request.QueryString[paramName];
			return retValue;
		}

		/// <summary>
		/// Percorso della pagina chiamante
		/// </summary>
		protected string BackUrl
		{
			get
			{
				if (this.ViewState["BACK_URL"]!=null)
					return this.ViewState["BACK_URL"].ToString();
				else
					return string.Empty;
			}
			set
			{
				this.ViewState["BACK_URL"]=value;
			}
		}
	}
}
