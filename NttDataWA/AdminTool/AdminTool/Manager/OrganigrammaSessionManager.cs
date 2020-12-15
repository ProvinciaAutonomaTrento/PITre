using System;

namespace Amministrazione.Manager
{
	/// <summary>
	/// Summary description for OrganigrammaSessionManager.
	/// </summary>
	public class OrganigrammaSessionManager
	{
		private const string SESSION_KEY="AMM_ORG_MANAGER";

		public static Amministrazione.Manager.OrganigrammaManager OrganigrammaManager()
		{
			if (System.Web.HttpContext.Current.Session[SESSION_KEY]==null)
			{				
				Amministrazione.Manager.OrganigrammaManager retValue = new OrganigrammaManager();
				System.Web.HttpContext.Current.Session.Add(SESSION_KEY,retValue);
			}

			return (Amministrazione.Manager.OrganigrammaManager) System.Web.HttpContext.Current.Session[SESSION_KEY];
		}
		
		public static void ReleaseAmmOrgManager()
		{
			System.Web.HttpContext.Current.Session.Remove(SESSION_KEY);
		}
	}
}
