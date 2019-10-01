using System;
using System.Web;
using System.Collections;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Classe per la gestione del contesto chiamante
	/// </summary>
	public class CallerContext
	{
		private const string SESSION_KEY="CallerContext";

		/// <summary>
		/// Creazione di un nuovo contesto chiamante ed inserimento in sessione
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static CallerContext NewContext(string url)
		{
			CallerContext context=new CallerContext(url);
			SetCallerContext(context);
			return context;
		}

		/// <summary>
		/// Reperimento del contesto chiamante
		/// </summary>
		/// <returns></returns>
		public static CallerContext GetCallerContext()
		{
			CallerContext context=null;

			Stack stack=HttpContext.Current.Session[SESSION_KEY] as Stack;

			if (stack!=null)
				context=stack.Pop() as CallerContext;

			return context;
		}

		/// <summary>
		/// Impostazione del contesto chiamante
		/// </summary>
		/// <param name="callerContext"></param>
		private static void SetCallerContext(CallerContext callerContext)
		{
			Stack stack=HttpContext.Current.Session[SESSION_KEY] as Stack;

			if (stack==null)
			{
				stack=new Stack();
				HttpContext.Current.Session.Add(SESSION_KEY,stack);
			}

			stack.Push(callerContext);
		}
	
		private string _url=string.Empty;
		private int _pageNumber=1;
		private Hashtable _additionalParameters=new Hashtable();

		public CallerContext(string url)
		{
			this._url=url;
		}

		/// <summary>
		/// Url del chiamante
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
			}
		}

		/// <summary>
		/// Interpretazione parametri querystring
		/// </summary>
		/// <returns></returns>
		private string ParseUrl()
		{
			string retValue=string.Empty;

			Hashtable queryStringParams=new Hashtable();

			// Reperimento parametri addizionali in querystring
			foreach (DictionaryEntry item in this.AdditionalParameters)
				queryStringParams.Add(item.Key,item.Value);					

			Uri uri=new Uri(this._url);

			if (uri.Query!=string.Empty)
			{
				retValue=uri.AbsoluteUri.Substring(0,uri.AbsoluteUri.IndexOf("?"));

				string[] items=uri.Query.Split('&');

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

						if (!queryStringParams.ContainsKey(paramName))
							queryStringParams.Add(paramName,paramValue);
					}
				}
			}
			else
			{
				retValue=uri.AbsoluteUri;
			}

			string queryString=string.Empty;

			foreach (DictionaryEntry item in queryStringParams)
			{
				if (queryString==string.Empty)
					queryString="?";
				else
					queryString+="&";

				queryString+=item.Key.ToString() + "=" + item.Value.ToString();
			}
		
			return retValue + queryString;
		}

		/// <summary>
		/// Contesto di paginazione del chiamante
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
		public Hashtable AdditionalParameters
		{
			get
			{	
				return this._additionalParameters;
			}
			set
			{
				this._additionalParameters=value;
			}
		}
	}
}