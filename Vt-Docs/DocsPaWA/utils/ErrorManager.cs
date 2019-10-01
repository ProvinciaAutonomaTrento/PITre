using System;
using System.Web.Services.Protocols;
using System.Xml;
using log4net;

namespace DocsPAWA
{
	/// <summary>
	/// Summary description for ErrorManager.
	/// </summary>
	public class ErrorManager 
	{
        private static ILog logger = LogManager.GetLogger(typeof(ErrorManager));
		public static void redirect(System.Web.UI.Page page, System.Exception e) {
			if (!(page.Session != null && page.Session["userData"] != null))
				redirectToLoginPage(page);
			else
				redirectToErrorPage(page, e);
		}

        public static void redirectCache(System.Web.UI.Page page, System.Exception e, bool cachePiena)
        {
            if (!(page.Session != null && page.Session["userData"] != null))
                redirectToLoginPage(page);
            else
                redirectToErrorPageCache(page, e, cachePiena);
        }

		public static void redirect(System.Web.UI.Page page, System.Exception e, string nomeProcedura) 
		{
			if (!(page.Session != null && page.Session["userData"] != null))
				redirectToLoginPage(page);
			else
				OpenErrorPage(page, e,nomeProcedura);
		}
//		public static string getErrorMsg(string codice)
//		{
//			XmlDocument xmlDoc = new XmlDocument();	
//			string xmldocpath = AppDomain.CurrentDomain.BaseDirectory + @"Utils/ErrorMsg.xml";	
//			xmlDoc.Load(xmldocpath);
//			XmlNode node =xmlDoc.SelectSingleNode(@"//VOCE[CODICE='" + codice + "']");	
//			
//			return node.Value;
//			
//		}
		public static void redirectToErrorPage(System.Web.UI.Page page, System.Exception e) {
			setError(page, e);
			string errPage = getErrorPage(page);
			page.ErrorPage = errPage;			
			page.Server.Transfer(errPage);		
			
		}

        public static void redirectToErrorPageCache(System.Web.UI.Page page, System.Exception e, bool cachePiena)
        {
            setError(page, e);
            string errPage = getErrorPageCache(page, cachePiena);
            page.ErrorPage = errPage;
            page.Server.Transfer(errPage);

        }
		public static void OpenErrorPage(System.Web.UI.Page page, System.Exception e, string nomeProcedura) 
		{
			setError(page, e);
			string errPage = page.Request.ApplicationPath + "/newErrorPage.aspx?nome_proc=" + nomeProcedura;
            page.Response.Write("<script>window.showModalDialog('" + errPage + "','','dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');</script> ");
        }

       

		public static void OpenErrorPage(System.Web.UI.Page page, System.Exception e, string nomeProcedura, bool closeChiamante) 
		{
			setError(page, e);
			string errPage = page.Request.ApplicationPath + "/newErrorPage.aspx?nome_proc=" + nomeProcedura;	
			if(closeChiamante)
				page.Response.Write("<script>window.showModalDialog('" + errPage + "','','dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');window.close()</script> ");
			else
				page.Response.Write("<script>window.showModalDialog('" + errPage + "','','dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');</script> ");
			

		}

        public static void OpenErrorPage(System.Web.UI.Page page, string msgError, string nomeProcedura)
        {
            page.Session["ErrorManager.error"] = msgError;
            string errPage = page.Request.ApplicationPath + "/newErrorPage.aspx?nome_proc=" + nomeProcedura;
            page.Response.Write("<script>window.showModalDialog('" + errPage + "','','dialogWidth:500px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');</script> ");
        }

		public static string getErrorPage(System.Web.UI.Page page) {
			string errPage = page.Request.ApplicationPath + "/ErrorPage.aspx";			
			string pageName = page.GetType().BaseType.Name;
			return errPage;
		}

        public static string getErrorPageCache(System.Web.UI.Page page, bool cachepiena)
        {

            string errPage = string.Empty;
            if(cachepiena)
                errPage = page.Request.ApplicationPath + "/documento/MemoriaPienaCache.aspx";
            else
                errPage= page.Request.ApplicationPath + "/documento/statoDocAcquisitoCaching.aspx";
           
            string pageName = page.GetType().BaseType.Name;
            return errPage;
        }
		public static void setError(System.Web.UI.Page page, System.Exception e) 
		{
			System.Web.HttpContext context = new System.Web.HttpContext(page.Request, page.Response);
			context.ClearError();
			context.AddError(e);
			e = context.AllErrors[0];
			string msg = e.Message;
			try {
//				SoapException es;
//				if (e.GetType().Equals(typeof(SoapException))) {
//					es = (SoapException) e;
//					msg = ((XmlElement)es.Detail).GetElementsByTagName("messaggio")[0].FirstChild.Value;
//					//debug = ((XmlElement)es.Detail).GetElementsByTagName("debug")[0].FirstChild.Value;
//				}
			} catch (Exception) {}
			logger.Error(e);
			page.Session["ErrorManager.error"] = msg;
		}

		public static void checkError(System.Web.UI.Page page) {
			if(getError(page) != null)
				page.Server.Transfer(getErrorPage(page));
		}

		public static Exception getError(System.Web.UI.Page page) {
			if(page.Session["ErrorManager.error"] != null)
				return new Exception((string)page.Session["ErrorManager.error"]);
			return null;
		}

		public static void redirectToLoginPage(System.Web.UI.Page page) {
           // string lgnPage = page.Request.ApplicationPath + "/login.aspx";
            string lgnPage = "~/login.aspx";
			string nameSpace="";
            
            if (page == null)
                return;
            
            nameSpace = page.GetType().BaseType.Namespace;

			string scriptFunct;
            if (!string.IsNullOrEmpty(nameSpace) && nameSpace.EndsWith(".popup"))
            {
				scriptFunct = "window.opener.top.location.href='" + lgnPage + "';";
				scriptFunct += "window.close();";
			} else  {
				//scriptFunct = "window.top.location.href='" + lgnPage + "';";	
				scriptFunct ="window.open('"+lgnPage+"','_blank','fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');";
										
					scriptFunct +=" window.opener=null;";
					scriptFunct +=" window.close();";
				//scriptFunct = "window.top.location.href='" + lgnPage + "';";
			}
			page.Response.Write("<script> " + scriptFunct + " </script>");
		}
	}
}
