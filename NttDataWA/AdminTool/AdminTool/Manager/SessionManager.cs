using System;
using System.Web;

namespace SAAdminTool.AdminTool.Manager
{
	public class SessionManager
	{
		private const string SESSION_UTENTE = "AMM_DATI_UTENTE";

        public void setUserAmmSession(SAAdminTool.DocsPaWR.InfoUtenteAmministratore datiAmministratore)
		{
			if (HttpContext.Current.Session[SESSION_UTENTE] == null)
				HttpContext.Current.Session.Add(SESSION_UTENTE, datiAmministratore);
            else
                HttpContext.Current.Session[SESSION_UTENTE] = datiAmministratore;

		}

        public SAAdminTool.DocsPaWR.InfoUtenteAmministratore getUserAmmSession()
		{
            SAAdminTool.DocsPaWR.InfoUtenteAmministratore datiAmministratore = null;

			if (HttpContext.Current.Session[SESSION_UTENTE] != null)
                datiAmministratore = (DocsPaWR.InfoUtenteAmministratore) HttpContext.Current.Session[SESSION_UTENTE];

            return datiAmministratore;
		}		
		
		public void removeUserAmmSession()
		{
            if (HttpContext.Current.Session[SESSION_UTENTE] != null)
			    HttpContext.Current.Session.Remove(SESSION_UTENTE);
		}
	}
}
