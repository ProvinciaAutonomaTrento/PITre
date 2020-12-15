using System;
using System.Xml;

namespace BusinessLogic.AmministrazioneXml
{
	/// <summary>
	/// Dati relativi all'utente
	/// </summary>
	public struct UserData
	{
		private string userIdAmm;
		private string userDescrizione;
		private string userRegistro;
		private string userIdRegistro;
		private string userRuolo;
		private string userCodice;
		private string userAccessoFunzione;
		private string userUtente;
		private string userPassword;

		/// <summary>
		/// </summary>
		/// <param name="nulled">
		/// true: gli attributi vengono impostati a 'null'
		/// false: gli attributi vengono impostati a stringa vuota ("")
		/// </param>
		public UserData(bool nulled)
		{
			if(nulled)
			{
				userIdAmm			= null;
				userDescrizione		= null;
				userRegistro		= null;
				userIdRegistro		= null;
				userRuolo			= null;
				userCodice			= null;
				userAccessoFunzione = null;
				userUtente			= null;
				userPassword		= null;
			}
			else
			{
				userIdAmm			= "";
				userDescrizione		= "";
				userRegistro		= "";
				userIdRegistro		= "";
				userRuolo			= "";
				userCodice			= "";
				userAccessoFunzione = "";
				userUtente			= "";
				userPassword		= "";
			}
		}

		/// <summary>
		/// </summary>
		public string idAmm
		{
			get
			{
				return userIdAmm;
			}
			set
			{
				if(value != null && value !="") userIdAmm = value;
			}
		}

		/// <summary>
		/// </summary>
		public string descrizione
		{
			get
			{
				return userDescrizione;
			}
			set
			{
				if(value != null && value != "") userDescrizione = value;
			}
		}
	
		/// <summary>
		/// </summary>
		public string registro
		{
			get
			{
				return userRegistro;
			}
			set
			{
				if(value != null && value !="") userRegistro = value;
			}
		}

		/// <summary>
		/// </summary>
		public string idRegistro
		{
			get
			{
				return userIdRegistro;
			}
			set
			{
				if(value != null && value !="") userIdRegistro = value;
			}
		}

		/// <summary>
		/// </summary>
		public string ruolo
		{
			get
			{
				return userRuolo;
			}
			set
			{
				if(value != null && value !="") userRuolo = value;
			}
		}

		/// <summary>
		/// </summary>
		public string codice
		{
			get
			{
				return userCodice;
			}
			set
			{
				if(value != null && value !="") userCodice = value;
			}
		}

		/// <summary>
		/// </summary>
		public string utente
		{
			get
			{
				return userUtente;
			}
			set
			{
				if(value != null && value != "") userUtente = value;
			}
		}

		/// <summary>
		/// </summary>
		public string password
		{
			get
			{
				return userPassword;
			}
			set
			{
				if(value != null && value != "") userPassword = value;				
			}
		}

		/// <summary>
		/// </summary>
		public string accessoFunzione
		{
			get
			{
				return userAccessoFunzione;
			}
			set
			{
				if(value != null && value !="") userAccessoFunzione = value;
			}
		}
	}
}
