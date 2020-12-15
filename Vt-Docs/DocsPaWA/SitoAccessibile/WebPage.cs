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


namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Summary description for WebPage.
	/// </summary>
	public class WebPage : DocsPAWA.XHTMLPage
	{
		protected System.Web.SessionState.HttpSessionState pgSession = null;
		private const string BASE_NAMESPACE = "DocsPAWA.SitoAccessibile";

		public WebPage()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public virtual void Create(System.Web.SessionState.HttpSessionState mySession)
		{
			pgSession = mySession;
		}

		public System.Web.SessionState.HttpSessionState AspxSession
		{
			get { return pgSession; }
		}

		public string PageName
		{
			get { return (GetType().BaseType.Name + ".aspx"); }
		}

		public string BaseNamespace
		{
			get { return BASE_NAMESPACE; }
		}

		public string RelativePageName
		{
			get 
			{ 
				string basepath = CurrentSection.Substring(BASE_NAMESPACE.Length);
				basepath = basepath.Replace(".","/");
				return (basepath + "/" + PageName); 
			}
		}


		public string CurrentPage
		{
			get { return GetType().BaseType.FullName; }
		}

		public string CurrentSection
		{
			get { return GetType().BaseType.Namespace; }
		}

		public virtual void AddToSession()
		{
		}

		public virtual void RemoveFromSession()
		{
		}

		public string RelativeDestinationPage(string pagename)
		{
			char[] sep = {'.'};

			string curr = CurrentPage;
			if (curr.EndsWith(".aspx"))
				curr = curr.Substring(0,curr.Length-".aspx".Length);
			string[] currElements = curr.Split(sep);

			string dest = pagename.Trim();
			if (dest.EndsWith(".aspx"))
				dest = dest.Substring(0,dest.Length-".aspx".Length);
			string[] destElements = dest.Split(sep);

			string outcome = "";
			int lastEqualIdx=0;
			bool stop = false;
			for (int i=0; !stop && i<currElements.Length-1; i++)
			{
				if (currElements[i]==destElements[i])
				{
					if (!stop)
						lastEqualIdx++;
				}
				else
				{
					outcome += "../";
				}
			}
			for (int j=lastEqualIdx; j<destElements.Length-1; j++)
			{
				outcome += destElements[j]+"/";
			}
			if (outcome=="")
				outcome = "./";

			outcome += destElements[destElements.Length-1]+".aspx";

			return outcome;
		}

		public string GetUrl()
		{
			string baseUrl = null;
			try
			{
				System.Uri uri = this.Request.Url;
				baseUrl = uri.Scheme+"://";
				baseUrl += uri.Host;
				baseUrl += (uri.Port!=80) ? (":"+uri.Port) : "";
				baseUrl += "/";
				for (int i=1; i<(uri.Segments.Length); i++)
				{
					baseUrl += uri.Segments[i];
				}
			}
			catch (Exception) {}
			return baseUrl;
		}

		public string GetBaseUrl()
		{
			string baseUrl = null;
			try
			{
				Type t = GetType().BaseType;
				string rPart = t.FullName.Substring(BASE_NAMESPACE.Length);
				rPart = rPart.Replace(".","/");
				rPart += ".aspx";
				System.Uri uri = this.Request.Url;
				string absUri = uri.AbsoluteUri;
				absUri = absUri.Substring(0, (absUri.Length - uri.Query.Length));
				baseUrl = absUri.Substring(0,(absUri.Length - rPart.Length));
			}
			catch (Exception) {}

			return baseUrl;
		}

		public string Escape(string url)
		{
			if (url==null)
				return null;

			string outcome = "";
			foreach (char c in url.ToCharArray())
			{
				switch (c)
				{
					case ' ':
						outcome += System.Uri.HexEscape(c);
						break;
					default:
						outcome += c;
						break;
				}
			}
			return outcome;
		}

		public string BackToBasePath()
		{
			string relPath = "";
			try
			{
				Type t = GetType().BaseType;
				string rPart = t.Namespace.Substring(BASE_NAMESPACE.Length);
				char[] sep = {'.'};
				string[] tokens = rPart.Split(sep);
				for (int i=0; tokens!=null && i<tokens.Length; i++)
				{
					if (tokens[i]!="")
					{
						relPath += "../";
					}
				}
				if (relPath=="")
					relPath = ".";

				while (relPath.EndsWith("/"))
					relPath = relPath.Substring(0,(relPath.Length-1));
									
			}
			catch (Exception) {}

			return relPath;
		}

		public string GetCallerAddress()
		{
			return System.Net.Dns.Resolve(this.Request.Url.Host).AddressList[0].ToString();
		}

	}
}
