using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;

namespace NttDataWA.UIManager
{
    public class DelegheManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();


        public static DocsPaWR.InfoDelega[] SearchDeleghe(Page page, SearchDelegaInfo searchInfo, ref SearchPagingContext pagingContext)
        {
            DocsPaWR.InfoUtente infoUtente = null;
            if (searchInfo.TipoDelega != "tutte")
                infoUtente = UserManager.GetInfoUser();

            DocsPaWR.InfoDelega[] deleghe = docsPaWS.DelegaSearch(infoUtente, searchInfo, ref pagingContext);
            //if (deleghe == null)
            //{
            //    throw new Exception();
            //}
            return deleghe;
        }

        public static DocsPaWR.Utente getUtenteById(Page page, string idPeople)
        {
            DocsPaWR.Utente utente = docsPaWS.getUtenteById(idPeople);
            return utente;
        }

        public static DocsPaWR.Utente EsercitaDelega(Page page, DocsPaWR.UserLogin userLogin, string id_delega, string id_ruoloDelegante, out DocsPaWR.LoginResult loginResult)
        {
            //verifica che l'utente delegante non sia connesso
            //chiamata al metodo che alla login riempe per la prima volta l'oggetto infoUtente;
            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            DocsPaWR.Utente utente = null;
            utente = docsPaWS.DelegaEsercita(infoUtente, userLogin, page.Session.SessionID, id_delega, id_ruoloDelegante, out loginResult);
            return utente;
        }

        public static bool DismettiDelega(DocsPaWR.InfoUtente infoUtente)
        {
            bool retValue = false;
            retValue = docsPaWS.DelegaDismetti(infoUtente, infoUtente.userId);
            return retValue;
        }

        public static bool DismettiDelega()
        {
            return DismettiDelega(UserManager.GetInfoUser());
        }

        public static bool RevocaDelega(Page page, DocsPaWR.InfoDelega[] listaDeleghe, out string msg)
        {
            msg = string.Empty;
            bool result = false;

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            result = docsPaWS.DelegaRevoca(infoUtente, listaDeleghe, out msg);
            return result;
        }

        public static bool VerificaUnicaDelega(DocsPaWR.InfoDelega delega)
        {
            bool retValue = false;

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            retValue = docsPaWS.DelegaVerificaUnicaAssegnata(infoUtente, delega);

            return retValue;
        }

        public static bool CreaNuovaDelega(DocsPaWR.InfoDelega delega)
        {
            bool retValue = false;

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            retValue = docsPaWS.DelegaCreaNuova(infoUtente, delega);

            return retValue;
        }

        public static OrgRisultatoRicerca[] GetRolesForUser(string codice)
        {
            return docsPaWS.AmmRicercaInOrg("PC", codice, string.Empty, UserManager.GetUserInSession().idAmministrazione, false, false);
        }

        public static bool ModificaDelega(DocsPaWR.InfoDelega delega, string tipoDelega, string idRuoloOld, string idUtenteOld, string dataScadenzaOld, string dataDecorrenzaOld, string idRuoloDeleganteOld)
        {
            bool retValue = false;

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();
            retValue = docsPaWS.DelegaModifica(infoUtente, delega, tipoDelega, idRuoloOld, idUtenteOld, dataScadenzaOld, dataDecorrenzaOld, idRuoloDeleganteOld);

            return retValue;
        }

        public static int VerificaDelega()
        {
            int result = 0;

            DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

            result = docsPaWS.DelegaCheckAttiva(infoUtente);
            return result;
        }

    }
}