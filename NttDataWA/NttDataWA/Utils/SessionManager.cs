using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public class SessionManager
    {

        private const string SESSION_UTENTE = "AMM_DATI_UTENTE";

        public void setUserAmmSession(DocsPaWR.InfoUtenteAmministratore datiAmministratore)
        {
            if (HttpContext.Current.Session[SESSION_UTENTE] == null)
                HttpContext.Current.Session.Add(SESSION_UTENTE, datiAmministratore);
            else
                HttpContext.Current.Session[SESSION_UTENTE] = datiAmministratore;

        }

        public DocsPaWR.InfoUtenteAmministratore getUserAmmSession()
        {
            DocsPaWR.InfoUtenteAmministratore datiAmministratore = null;

            if (HttpContext.Current.Session[SESSION_UTENTE] != null)
                datiAmministratore = (DocsPaWR.InfoUtenteAmministratore)HttpContext.Current.Session[SESSION_UTENTE];

            return datiAmministratore;
        }

        public void removeUserAmmSession()
        {
            if (HttpContext.Current.Session[SESSION_UTENTE] != null)
                HttpContext.Current.Session.Remove(SESSION_UTENTE);
        }

    }
}