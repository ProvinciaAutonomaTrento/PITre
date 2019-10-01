using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using SAAdminTool.DocsPaWR;
using System.Collections.Generic;

namespace SAAdminTool
{
    public class DelegheManager
    {
        protected static SAAdminTool.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();

        public static bool CreaNuovaDelegaAmm(Page page, SAAdminTool.DocsPaWR.InfoDelega delega)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.DelegaCreaNuova(null,delega);
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }
            return retValue;
        }


        public static bool CreaNuovaDelega(Page page, SAAdminTool.DocsPaWR.InfoDelega delega)
        {
            bool retValue = false;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue=docsPaWS.DelegaCreaNuova(infoUtente, delega);
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }
            return retValue;
        }

        public static bool VerificaUnicaDelega(Page page, SAAdminTool.DocsPaWR.InfoDelega delega)
        {
            bool retValue = false;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.DelegaVerificaUnicaAssegnata(infoUtente, delega);
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }
            return retValue;
        }

        public static bool VerificaUnicaDelegaAmministrazione(Page page, SAAdminTool.DocsPaWR.InfoDelega delega)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.DelegaVerificaUnicaAssegnataAmm(delega);
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }
            return retValue;
        }

        public static bool ModificaDelega(Page page, SAAdminTool.DocsPaWR.InfoDelega delega, string tipoDelega, string idRuoloOld, string idUtenteOld, string dataScadenzaOld, string dataDecorrenzaOld, string idRuoloDeleganteOld)
        {
            bool retValue = false;

            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                retValue = docsPaWS.DelegaModifica(infoUtente, delega, tipoDelega, idRuoloOld, idUtenteOld, dataScadenzaOld, dataDecorrenzaOld, idRuoloDeleganteOld);
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }
            return retValue;
        }

        public static bool ModificaDelegaAmm(Page page, SAAdminTool.DocsPaWR.InfoDelega delegaOld, SAAdminTool.DocsPaWR.InfoDelega delegaNew, string tipoDelega, string dataScadenzaOld, string dataDecorrenzaOld)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.DelegaModificaAmm(delegaOld, delegaNew, tipoDelega, dataScadenzaOld, dataDecorrenzaOld);
            }
            catch (System.Exception exc)
            {
                ErrorManager.redirect(page, exc);
            }
            return retValue;
        }

        //Restituisce la lista delle deleghe assegnate da un dato utente
        public static  SAAdminTool.DocsPaWR.InfoDelega[] GetListaDeleghe(Page page, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe)
        {
            numDeleghe = 0;
            SAAdminTool.DocsPaWR.InfoDelega[] deleghe = null;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = null;
                if (tipoDelega != "tutte")
                    infoUtente = UserManager.getInfoUtente(page);
                
                deleghe = docsPaWS.DelegaGetLista(infoUtente, tipoDelega, statoDelega, idAmm, out numDeleghe);
                if (deleghe == null)
                {
                    throw new Exception();
                }
                return deleghe;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return deleghe;
        }

        public static SAAdminTool.DocsPaWR.InfoDelega[] SearchDeleghe(Page page, SearchDelegaInfo searchInfo, ref SearchPagingContext pagingContext)
        {
            SAAdminTool.DocsPaWR.InfoDelega[] deleghe = null;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = null;
                if (searchInfo.TipoDelega != "tutte")
                    infoUtente = UserManager.getInfoUtente(page);

                deleghe = docsPaWS.DelegaSearch(infoUtente, searchInfo,ref pagingContext);
                if (deleghe == null)
                {
                    throw new Exception();
                }
                return deleghe;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return deleghe;
        }

        public static SAAdminTool.DocsPaWR.Utente EsercitaDelega(Page page, DocsPaWR.UserLogin userLogin, string id_delega, string id_ruoloDelegante, out DocsPaWR.LoginResult loginResult)
        { 
            //verifica che l'utente delegante non sia connesso
            //chiamata al metodo che alla login riempe per la prima volta l'oggetto infoUtente;
            SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
            SAAdminTool.DocsPaWR.Utente utente = null;
            utente = docsPaWS.DelegaEsercita(infoUtente, userLogin, page.Session.SessionID, id_delega, id_ruoloDelegante, out loginResult);
            return utente;
        }

        public static bool DismettiDelega(DocsPaWR.InfoUtente infoUtente)
        {
            bool retValue = false;
            //retValue = docsPaWS.DelegaDismetti(infoUtente.delegato, infoUtente.userId);
            retValue = docsPaWS.DelegaDismetti(infoUtente, infoUtente.userId);
            return retValue;
        }

        public static bool DismettiDelega()
        {
            return DismettiDelega(UserManager.getInfoUtente());
        }

        public static bool RevocaDelega(Page page, SAAdminTool.DocsPaWR.InfoDelega[] listaDeleghe, out string msg)
        {
            msg = string.Empty;
            bool result = false;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);

                result = docsPaWS.DelegaRevoca(infoUtente, listaDeleghe, out msg);
                //if (!result)
                //{
                //    //throw new Exception();
                //}
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        public static bool RevocaDelegaAmm(Page page, SAAdminTool.DocsPaWR.InfoDelega[] listaDeleghe, out string msg)
        {
            bool result = false;
            msg = string.Empty;
            try
            {
                result = docsPaWS.DelegaRevoca(null, listaDeleghe, out msg);
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }


        public static int VerificaDelega(Page page)
        {
            int result = 0;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);

                result = docsPaWS.DelegaCheckAttiva(infoUtente);
                return result;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return result;
        }

        public static DocsPaWR.Utente getUtenteById(Page page, string idPeople)
        {
            DocsPaWR.Utente utente = null;
            try
            {
                utente = docsPaWS.getUtenteById(idPeople);
                return utente;
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return utente;
        }

        //MODELLI DI DELEGA
        public static SAAdminTool.DocsPaWR.ModelloDelega[] GetModelliDelega(Page page,SearchModelloDelegaInfo searchInfo,ref SearchPagingContext pagingContext){
            SAAdminTool.DocsPaWR.ModelloDelega[] res = null;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                infoUtente = UserManager.getInfoUtente(page);
                res = docsPaWS.SearchModelliDelega(infoUtente,searchInfo,ref pagingContext);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return res;
        }

        public static ModelloDelega[] GetModelliDelegaValidi(Page page)
        {
            SAAdminTool.DocsPaWR.ModelloDelega[] res = null;
            try
            {
                SAAdminTool.DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente(page);
                infoUtente = UserManager.getInfoUtente(page);
                res = docsPaWS.GetModelliDelegaByStato(infoUtente, StatoModelloDelega.VALIDO);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return res;
        }

        public static ModelloDelega GetModelloDelegaById(string idModello,Page page)
        {
            SAAdminTool.DocsPaWR.ModelloDelega res = null;
            try
            {
                res = docsPaWS.GetModelloDelegaById(idModello);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return res;
        }

        public static bool CreaNuovoModelloDelega(ModelloDelega modDel, Page page)
        {
            try
            {
                return docsPaWS.InsertModelloDelega(modDel);
            }
            catch (Exception es)
            {
                ErrorManager.redirect(page, es);
            }
            return false;
        }

        public static bool ModificaModelloDelega(ModelloDelega modDel, Page page)
        {
            try
            {
                return docsPaWS.UpdateModelloDelega(modDel);
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return false;
        }

        public static bool CancellaModelliDelega(List<string> ids, Page page)
        {
            try
            {
                return docsPaWS.DeleteModelliDelega(ids.ToArray());
            }
            catch (Exception e)
            {
                ErrorManager.redirect(page, e);
            }
            return false;
        }
    }
}
