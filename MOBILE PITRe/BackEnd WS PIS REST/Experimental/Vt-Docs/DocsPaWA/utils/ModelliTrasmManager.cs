using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using DocsPAWA.DocsPaWR;
using System.Data;
using System.Text.RegularExpressions;
using DocsPAWA.SiteNavigation;


namespace DocsPAWA
{
    public class ModelliTrasmManager
    {
        private static DocsPaWebService docsPaWS = ProxyManager.getWS();

        public static ArrayList getModelliPerTrasm(string idAmm, Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, Page page, string system_id, string idRuoloUtente, bool AllReg, string accessrights)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliPerTrasm(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, cha_tipo_oggetto, system_id, idRuoloUtente, AllReg, accessrights));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static string salvaModello(ModelloTrasmissione modelloTrasmissione, InfoUtente infoUtente, Page page)
        {
            try
            {
                return docsPaWS.salvaModello(modelloTrasmissione, infoUtente);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool isUniqueCodModelloTrasm(ModelloTrasmissione modello, Page page)
        {
            try
            {
                return docsPaWS.isUniqueCodModelloTrasm(modello);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static ArrayList getModelliByAmm(string idAmm, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliByAmm(idAmm));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getModelliByAmmPaging(string idAmm, int nPagina, string ricerca, string codice, out int numTotPag, Page page)
        {
            numTotPag = 0;
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliByAmmPaging(idAmm, nPagina, ricerca, codice, out numTotPag));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static string getModelloSystemId(Page page)
        {
            try
            {
                return docsPaWS.getModelloSystemId();
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getModelliUtente(Utente utente, InfoUtente infoUt, DocsPaWR.FiltroRicerca[] filtriRicerca, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliUtente(utente, infoUt, filtriRicerca));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ModelloTrasmissione getModelloByID(string idAmm, string idModello, Page page)
        {
            try
            {
                return docsPaWS.getModelloByID(idAmm, idModello);

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static void CancellaModello(string idAmm, string idModello, Page page)
        {
            try
            {
                docsPaWS.CancellaModello(idAmm, idModello);

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static void CancellaDestModello(string idRagione, string varCodRubrica, string idModello, Page page)
        {
            try
            {
                docsPaWS.CancellaDestModello(idRagione, varCodRubrica, idModello);

            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
            }
        }

        public static ModelloTrasmissione UtentiConNotificaTrasm(ModelloTrasmissione objModTrasm, string operazione, Page page)
        {
            try
            {
                return docsPaWS.UtentiConNotificaTrasm(objModTrasm, null, null, operazione);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static bool SalvaCessioneDirittiSuModelliTrasm(ModelloTrasmissione objTrasm, Page page)
        {
            try
            {
                return docsPaWS.SalvaCessioneDirittiSuModelliTrasm(objTrasm);
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return false;
            }
        }

        public static ArrayList getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca, Page page)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliByAmmConRicerca(idAmm, codiceRicerca, tipoRicerca));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo, Page page)
        {

            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliAssDiagrammi(idTipo, idDiagramma, stato, idAmm, selezionati, tipo));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ModelloTrasmissione getModelloByCodice(string idAmministrazione, string codiceModello, Page page)
        {
            try
            {
                string[] system_id_modello = Regex.Split(codiceModello, "MT_");
                ModelloTrasmissione modelloResult = null;
                if (system_id_modello.Length == 2)
                {
                    modelloResult = docsPaWS.getModelloByID(idAmministrazione, system_id_modello[1]);
                }

                return modelloResult;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        public static ArrayList getModelliPerTrasmLite(string idAmm, Registro[] registri, string idPeople, string idCorrGlobali, string idTipoDoc, string idDiagramma, string idStato, string cha_tipo_oggetto, Page page, string system_id, string idRuoloUtente, bool AllReg, string accessrights)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliPerTrasmLite(idAmm, registri, idPeople, idCorrGlobali, idTipoDoc, idDiagramma, idStato, cha_tipo_oggetto, system_id, idRuoloUtente, AllReg, accessrights));
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.redirect(page, ex);
                return null;
            }
        }

        /// <summary>
        /// Funzione per ricerca e sostituzione di un ruolo con un altro in determinati ruoli
        /// </summary>
        /// <param name="roleToReplace">Ruolo da restituire</param>
        /// <param name="newRole">Ruolo con cui sostituire</param>
        /// <param name="operation">Operazione da compiere</param>
        /// <param name="searchFilters">Filtri di ricerca</param>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <param name="isAdmin">True se l'utente richiedente è un amministratore</param>
        /// <param name="models">Collection dei modelli</param>
        /// <param name="copyNotes">True se bisogna copiare le note</param>
        /// <returns>Risultato dell'elaborazione</returns>
        public static FindAndReplaceResponse FindAndReplaceRolesInModelliTrasmissione(ElementoRubrica roleToReplace, ElementoRubrica newRole, FindAndReplaceEnum operation, FiltroRicerca[] searchFilters, InfoUtente userInfo, bool isAdmin, ModelloTrasmissioneSearchResult[] models, bool copyNotes)
        {
            // Costruzione della request
            FindAndReplaceRequest request = new FindAndReplaceRequest()
                {
                    NewRole = newRole,
                    Operation = operation,
                    RoleToReplace = roleToReplace,
                    SearchFilters = searchFilters,
                    UserInfo = userInfo,
                    IsAdministrator = isAdmin,
                    Models = models,
                    CopyNotes = copyNotes
                };

            // Esecuzione operazione
            FindAndReplaceResponse response = docsPaWS.FindAndReplaceRoleInModelliTrasmissione(request);

            // Restituzione della response
            return response;

        }

        /// <summary>
        /// Filtro di ricerca da utilizzare per la ricerca dei modelli di trasmissione
        /// </summary>
        public static FiltroRicerca[] SearchFilters
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["SearchFilters"] as FiltroRicerca[];
            }

            set
            {
                if (CallContextStack.CurrentContext == null)
                    CallContextStack.CurrentContext = new CallContext("FindAndReplace");
                CallContextStack.CurrentContext.ContextState["SearchFilters"] = value;
            }
        }
    }
}
