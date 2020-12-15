using System;

namespace DocsPaVO.utente
{
	/// <summary>
	/// Oggetto per il trasporto dei dati relativi ad un utente durante lo operazioni
	/// di connessione all'applicazione
	/// </summary>
    [Serializable()]
	public class UserLogin
	{
        /// <summary>
        /// Usato per comunicare al front end l'esito di un'operazione di login
        /// </summary>
        public enum LoginResult
        {
            OK,
            UNKNOWN_USER,
            USER_ALREADY_LOGGED_IN,
            APPLICATION_ERROR,
            DISABLED_USER,
            NO_RUOLI,
            NO_AMMIN,
            PASSWORD_EXPIRED,
            UNKNOWN_DTCM_USER,
            DTCM_SERVICE_NO_CONTACT,
            DB_ERROR,
            UNKNOWN_AMMIN
        }

		/// <summary>
		/// Usato per comunicare al front end l'esito di un'operazione di login
		/// </summary>
		public enum ValidationResult
		{
			OK,
			SESSION_EXPIRED,
			SESSION_DROPPED,
			APPLICATION_ERROR
		}

        public enum ResetPasswordResult
        {
            OK,
            INVALID_EMAIL,
            INVALID_USERID,
            INVALID_OTP,
            ERROR_SEND_MAIL,
            DOMAIN_USER,
            KO
        }

        /// <summary>
        /// 
        /// </summary>
        private const bool UPDATE_DEFAULT_VALUE = false;

		private string userName	= null;
		private string password	= null;
		private string idAmm	= null;
		private string dominio	= null;
		private string dst		= null;
		private bool update	= UPDATE_DEFAULT_VALUE;
		private string systemID="";
		private string ipaddress;
        private string token = null;
        private string _sessionId = null;
        private bool _ssoLogin = false;
        private string _modulo = null;
        private BrowserInfo browser = null;

		/// <summary>
		/// Aggiorna le proprieta' dell'oggetto. Passare 'null' ai parametri non utilizzati. Alternativamente,
		/// utilizzare i metodi 'ResetData' ed assegnare le proprieta' direttamente.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		/// <param name="idAmministrazione"></param>
		/// <param name="dominio"></param>
		/// <param name="dst"></param>
		/// <param name="update"></param>
		public void SetData(string name, string password, string idAmministrazione, string dominio, string dst, bool update)
		{
			this.UserName			= name;
			this.Password			= password;
			this.IdAmministrazione	= idAmministrazione;
			this.Dominio			= dominio;
			this.DST				= dst;
			this.Update				= update;
		}

		/// <summary>
		/// Azzera le proprieta' dell'oggetto ai valori di default.
		/// </summary>
		public void ResetData()
		{
			this.UserName			= null;
			this.Password			= null;
			this.IdAmministrazione	= null;
			this.Dominio			= null;
			this.DST				= null;
			this.Update				= UPDATE_DEFAULT_VALUE;
            this.SessionId = null;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool AcquireData(System.Data.DataSet login)
		{
			bool result = true;
			this.ResetData();

			try
			{
				System.Data.DataRow row = login.Tables[0].Rows[0];

				string newDominio = null;
				string newDst	  = null;
				if(row["dominio"] != null) newDominio = row["dominio"].ToString();
				if(row["dst"]     != null) newDst     = row["dst"].ToString();

				this.SetData(row["userName"].ToString(),
							 row["password"].ToString(),
							 row["idAmministrazione"].ToString(),
							 row["dominio"].ToString(),
							 row["dst"].ToString(),
							 UPDATE_DEFAULT_VALUE);
			}
			catch(Exception)
			{
				result = false;
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool AcquireData(string loginXmlStream)
		{
			bool result = true;
			this.ResetData();

			try
			{
				System.Xml.XmlDocument sourceDoc = new System.Xml.XmlDocument();
				sourceDoc.LoadXml(loginXmlStream);

				string newDominio = null;
				string newDst	  = null;
				if(sourceDoc.DocumentElement.SelectSingleNode("dominio") != null) newDominio = sourceDoc.DocumentElement.SelectSingleNode("dominio").InnerText;
				if(sourceDoc.DocumentElement.SelectSingleNode("dst")     != null) newDst     = sourceDoc.DocumentElement.SelectSingleNode("dst").InnerText;

				this.SetData(sourceDoc.DocumentElement.SelectSingleNode("userName").InnerText,
							 sourceDoc.DocumentElement.SelectSingleNode("password").InnerText,
							 sourceDoc.DocumentElement.SelectSingleNode("idAmministrazione").InnerText,
							 newDominio,
							 newDst,
							 UPDATE_DEFAULT_VALUE);
			}
			catch(Exception)
			{
				result = false;
			}

			return result;
		}

		#region Costruttori		
		/// <summary>
		/// 
		/// </summary>
		public UserLogin()
		{
			this.ResetData();
		}		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		/// <param name="idAmministrazione"></param>
		/// <param name="dominio"></param>
		/// <param name="dst"></param>
		/// <param name="update"></param>
		public UserLogin(string name, string password, string idAmministrazione, string dominio, string dst, bool update)
		{
			this.SetData(name, password, idAmministrazione, dominio, dst, update);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		/// <param name="idAmministrazione"></param>
		/// <param name="dominio"></param>
		/// <param name="dst"></param>
		public UserLogin(string name, string password, string idAmministrazione, string dominio, string dst)
		{
			this.SetData(name, password, idAmministrazione, dominio, dst, UPDATE_DEFAULT_VALUE);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		/// <param name="idAmministrazione"></param>
		/// <param name="dominio"></param>
		public UserLogin(string name, string password, string idAmministrazione, string dominio)
		{
			this.SetData(name, password, idAmministrazione, dominio, null, UPDATE_DEFAULT_VALUE);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="password"></param>
		/// <param name="idAmministrazione"></param>
		public UserLogin(string name, string password, string idAmministrazione)
		{
			this.SetData(name, password, idAmministrazione, null, null, UPDATE_DEFAULT_VALUE);
		}
		#endregion

		#region Proprieta'
		/// <summary>
		/// 
		/// </summary>
		public string UserName
		{
			get { return this.userName;  }
			set { this.userName = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Password
		{
			get { return this.password;  }
			set { this.password = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string IdAmministrazione
		{
			get { return this.idAmm;  }
			set { this.idAmm = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Dominio
		{
			get { return this.dominio;  }
			set { this.dominio = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string DST
		{
			get { return this.dst;  }
			set { this.dst = value; }
		}

        /// <summary>
        /// 
        /// </summary>
        public string SessionId
        {
            get { return this._sessionId; }
            set { this._sessionId = value; }
        }

		/// <summary>
		/// 
		/// </summary>
		public bool Update
		{
			get { return this.update;  }
			set { this.update = value; }
		}

		public string SystemID
		{
			get { return this.systemID;  }
			set { this.systemID=value; }
		}

		public string IPAddress
		{
			get { return this.ipaddress;  }
			set { this.ipaddress=value; }
		}

        public string Token
        {
            get { return this.token; }
            set { this.token = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SSOLogin
        {
            get { return this._ssoLogin; }
            set { this._ssoLogin = value; }
        }

        /// <summary>
        /// Indica se valorizzato il nome del modulo per cui si richiede l'accesso
        /// </summary>
        public string Modulo
        {
            get { return this._modulo; }
            set { this._modulo = value; }
        }

        public BrowserInfo BrowserInfo
        {
            get { return this.browser; }
            set { this.browser = value; }
        }

		#endregion
	}
}
