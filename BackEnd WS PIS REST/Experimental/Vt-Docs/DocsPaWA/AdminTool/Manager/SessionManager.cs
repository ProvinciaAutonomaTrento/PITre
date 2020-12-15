using System.Web;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.AdminTool.Manager
{
    public class SessionManager
    {
        private const string SessionUtente = "AMM_DATI_UTENTE";

        public void setUserAmmSession(InfoUtenteAmministratore datiAmministratore)
        {
            if (HttpContext.Current.Session[SessionUtente] == null)
                HttpContext.Current.Session.Add(SessionUtente, datiAmministratore);
            else
                HttpContext.Current.Session[SessionUtente] = datiAmministratore;
        }

        public InfoUtenteAmministratore getUserAmmSession()
        {
            InfoUtenteAmministratore datiAmministratore = null;

            if (HttpContext.Current == null || HttpContext.Current.Session == null)
                return null;

            if (HttpContext.Current.Session[SessionUtente] != null)
                datiAmministratore = (InfoUtenteAmministratore) HttpContext.Current.Session[SessionUtente];

            return datiAmministratore;
        }

        public void removeUserAmmSession()
        {
            if (HttpContext.Current.Session[SessionUtente] != null)
                HttpContext.Current.Session.Remove(SessionUtente);
        }
    }
}