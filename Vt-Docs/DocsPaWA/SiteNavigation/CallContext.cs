using System;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Reflection;

namespace DocsPAWA.SiteNavigation
{
	/// <summary>
	/// 
	/// </summary>
	public delegate void RestoreContextDelegate(RestoreContextEventArgs e);

	/// <summary>
	/// 
	/// </summary>
	public class RestoreContextEventArgs : EventArgs
	{
		private IDictionary _sessionState=null;

		public RestoreContextEventArgs(IDictionary sessionState)
		{
			this._sessionState=sessionState;
		}

		/// <summary>
		/// 
		/// </summary>
		public IDictionary SessionState
		{
			get
			{
				return this._sessionState;
			}
		}
	}

	/// <summary>
	/// Classe per la gestione dei dati di un contesto di chiamata
	/// </summary>
	public class CallContext : IDisposable
	{
		/// <summary>
		/// Nome del contesto
		/// </summary>
		private string _contextName=string.Empty;

		/// <summary>
		/// Descrizione del contesto
		/// </summary>
		private string _contextDescription=string.Empty;

		/// <summary>
		/// Url del contesto
		/// </summary>
		private string _url=string.Empty;

		/// <summary>
		/// Eventuale nome del frame in cui caricare il contesto
		/// </summary>
		private string _contextFrameName=string.Empty;

		/// <summary>
		/// 
		/// </summary>
		private int _pageNumber=1;

		/// <summary>
		/// Parametri querystring aggiuntivi per il contesto
		/// </summary>
		private Hashtable _queryStringParameters=null;

		/// <summary>
		/// Stato della sessione legato al contesto
		/// </summary>
		private Hashtable _sessionState=null;

        /// <summary>
        /// Stato del contesto
        /// </summary>
        private Hashtable _contextState = null;

        /// <summary>
        /// Oggetto per lo scorrimento di elementi
        /// </summary>
        private UserControls.ScrollElementsList.ObjScrollElementsList _objScrollElementsList = null;

		#region Gestione eventi contesto

		/// <summary>
		/// 
		/// </summary>
		private RestoreContextDelegate _restoreContext=null;

		/// <summary>
		/// Evento notificato al ripristino dello stato dell'oggetto "CallContext"
        /// (se la proprietà "IsBack" è true)
        /// 
		/// </summary>
		public event RestoreContextDelegate RestoreContextState
		{
			add
			{
				if (this._restoreContext==null)
				{
					this._restoreContext+=value;
				}
				else
				{
					bool insert=true;

					foreach (Delegate item in this._restoreContext.GetInvocationList())
					{
						if (item.Method.Equals(value.Method))
						{
							insert=false;
							break;
						}
					}

					if (insert)
						this._restoreContext+=value;
				}
			}
			remove
			{
				if (this._restoreContext!=null)
					this._restoreContext-=value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        private EventHandler _contextDisposed = null;

        /// <summary>
        /// Evento notificato alla rimozione dell'oggetto "CallContext"
        /// </summary>
        public event EventHandler ContextDisposed
        {
            add
            {
                if (this._contextDisposed == null)
                {
                    this._contextDisposed += value;
                }
                else
                {
                    bool insert = true;

                    foreach (Delegate item in this._contextDisposed.GetInvocationList())
                    {
                        if (item.Method.Equals(value.Method))
                        {
                            insert = false;
                            break;
                        }
                    }

                    if (insert)
                        this._contextDisposed += value;
                }
            }
            remove
            {
                if (this._contextDisposed != null)
                    this._contextDisposed -= value;
            }
        }


		/// <summary>
		/// Ripristino dello stato della sessione del contesto
		/// </summary>
		/// <param name="context"></param>
		internal static void RestoreContextSessionState(CallContext context)
		{
			foreach (DictionaryEntry entry in context.SessionState)
			{
				if (HttpContext.Current.Session[entry.Key.ToString()]==null)
					HttpContext.Current.Session.Add(entry.Key.ToString(),entry.Value);
				else
					HttpContext.Current.Session[entry.Key.ToString()]=entry.Value;
			}

			if (context._restoreContext!=null)
			{
				context._restoreContext(new RestoreContextEventArgs(context.SessionState));
			}

            // Rilascio risorse di sessione ripristinate dal contesto corrente
            ReleaseSessionState(context);
		}

		#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CallContext)
            {
                return (this.ContextName.Equals(((CallContext)obj).ContextName));
                        // this.Url.Equals(((CallContext)obj).Url));
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ContextName.GetHashCode();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextName"></param>
		public CallContext(string contextName)
		{
			this._contextName=contextName;

			if (HttpContext.Current.Request.UrlReferrer!=null)
				this.Url=HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextName"></param>
		/// <param name="url"></param>
		public CallContext(string contextName,string url)
		{
			this._contextName=contextName;
			
			this.Url=url;
		}

		#region Public methods

		/// <summary>
		/// Nome del contesto di chiamata
		/// </summary>
		public string ContextName
		{
			get
			{
				return this._contextName;
			}
		}

		/// <summary>
		/// Descrizione del contesto di chiamata
		/// </summary>
		public string ContextDescription
		{
			get
			{
				return this._contextDescription;
			}
			set
			{
				this._contextDescription=value;
			}
		}

        /// <summary>
        /// Contesto di back
        /// </summary>
        public bool IsBack
        {
            get
            {   
                bool result = false;

                if (this.QueryStringParameters.ContainsKey("back"))
                    bool.TryParse(this.QueryStringParameters["back"].ToString(), out result);

                return result;
            }
            set
            {
                if (!this.QueryStringParameters.ContainsKey("back"))
                    this.QueryStringParameters.Add("back", value.ToString().ToLower());
                else
                    this.QueryStringParameters["back"] = value.ToString().ToLower();
            }
        }

		/// <summary>
		/// Url del contesto
		/// </summary>
		public string Url
		{
			get
			{
				return this.ParseUrl();
			}
			set
			{
				this._url=value;

				int indexOf=this._url.IndexOf("?");

				if (indexOf>-1)
					this.ParseQueryString(this._url,this.QueryStringParameters);
			}
		}

		/// <summary>
		/// Nome del frame in cui effettuare la redirect dell'url
		/// </summary>
		public string ContextFrameName
		{
			get
			{
				return this._contextFrameName;
			}
			set
			{
				this._contextFrameName=value;
			}
		}

		/// <summary>
		/// Contesto di paginazione del contesto di chiamata
		/// </summary>
		public int PageNumber
		{
			get
			{
				return this._pageNumber;
			}
			set
			{
				this._pageNumber=value;
			}
		}

		/// <summary>
		/// Hashtable contenente eventuali parametri query string aggiuntivi
		/// </summary>
		public Hashtable QueryStringParameters
		{
			get
			{	
				if (this._queryStringParameters==null)
					this._queryStringParameters=new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());

				return this._queryStringParameters;
			}
		}

		/// <summary>
		/// Stato della sessione da ripristinare
		/// </summary>
		public Hashtable SessionState
		{
			get
			{
				if (this._sessionState==null)
					this._sessionState=new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());

				return this._sessionState;
			}
		}

        /// <summary>
        /// Stato del contesto
        /// </summary>
        public Hashtable ContextState
        {
            get
            {
                if (this._contextState == null)
                    this._contextState = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());

                return this._contextState;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (this.ContextDescription==string.Empty)
				return this.ContextName;
			else
				return this.ContextDescription;
        }
        
        /// <summary>
		/// Deallocazione risorse del contesto
		/// </summary>
		public void Dispose()
		{
            ReleaseSessionState(this);

			if (this._queryStringParameters!=null)
			{
				this._queryStringParameters.Clear();
				this._queryStringParameters=null;
			}

            if (this._contextState != null)
            {
                this._contextState.Clear();
                this._contextState = null;
            }

            if (this._contextDisposed != null)
            {
                // Notifica evento "ContextDisposed"
                this._contextDisposed(this, new EventArgs());
                this._contextDisposed = null;
            }
		}

        /// <summary>
        /// Oggetto per lo scorrimento di elementi
        /// </summary>
        public UserControls.ScrollElementsList.ObjScrollElementsList objScrollElementsList
        {
            get
            {
                return _objScrollElementsList;
            }
            set
            {
                this._objScrollElementsList = value;
            }
        }

		#endregion

        #region Protected methods

        /// <summary>
        /// Deallocazione risorse di sessione ripristinate dal contesto
        /// </summary>
        protected static void ReleaseSessionState(CallContext context)
        {
            if (context._sessionState != null)
            {
                context._sessionState.Clear();
                context._sessionState = null;
            }
            
            if (context._restoreContext != null)
            {
                context._restoreContext = null;
            }
        }

        #endregion
        
		#region Private methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="completeUrl"></param>
		/// <param name="queryStringParameters"></param>
		private void ParseQueryString(string completeUrl,Hashtable queryStringParameters)
		{
			Uri uri=new Uri(completeUrl);
			
			string queryString=uri.Query;

			if (queryString!=string.Empty)
			{
				string[] items=queryString.Split('&');

				foreach (string item in items)
				{
					string[] paramPair=item.Split('=');
					if (paramPair.Length>0)
					{
						string paramName=paramPair[0];
						if (paramName.StartsWith("?"))
							paramName=paramName.Substring(1);
							
						string paramValue=string.Empty;
						if (paramPair.Length==2)
							paramValue=paramPair[1];

						if (!queryStringParameters.ContainsKey(paramName))
							queryStringParameters.Add(paramName,paramValue);
					}
				}
			}
		}


		/// <summary>
		/// Interpretazione parametri querystring
		/// </summary>
		/// <returns></returns>
		private string ParseUrl()
		{
			string queryString=string.Empty;

			foreach (DictionaryEntry item in this.QueryStringParameters)
			{
                if (queryString == string.Empty)
                    queryString = "?";
                else
                    queryString += "&";

                queryString += item.Key.ToString() + "=" + item.Value.ToString();
			}
		
			string retValue=this._url;

			int indexOf=retValue.IndexOf("?");
			if (indexOf > -1)
				retValue=retValue.Substring(0,indexOf);

			if (queryString!=string.Empty)
				retValue+=queryString;

			return retValue;
		}

		#endregion
	}
}