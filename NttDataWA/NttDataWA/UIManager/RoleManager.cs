using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using System.Web.UI;
using NttDataWA.DocsPaWR;
using System.Data;

namespace NttDataWA.UIManager
{
    public class RoleManager
    {        
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static void SetRoleInSession(DocsPaWR.Ruolo role)
        {
            try
            {
                HttpContext.Current.Session["userRole"] = role;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.Ruolo GetRoleInSession()
        {
            try
            {
                return HttpContext.Current.Session["userRole"] as DocsPaWR.Ruolo;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Usato nel caso di protocollo interno per determinare se un ruolo appartenente a una lista
        /// è autorizzato sul  registro del protocollo
        /// </summary>
        /// <param name="idRegistro">systemId del registro della scheda documento corrente</param>
        /// <returns></returns>
        public static bool VerifyAuthenticationRoleOnRegistry(string idRegister, string idRole)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(idRegister))
                {
                    DocsPaWR.Registro[] RegRuoloSel = UIManager.RegistryManager.GetRegistriesByRole(idRole);
                    foreach (DocsPaWR.Registro reg in RegRuoloSel)
                    {
                        if (idRegister == reg.systemId)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static string[] GetUoInternalAOO()
        {
            try
            {
                string id_reg = UIManager.RegistryManager.GetRegistryInSession().systemId;
                return docsPaWS.GetUoInterneAoo(id_reg, UIManager.UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Ruolo getRuoloById(string idCorrGlobali)
        {
            try
            {
                return docsPaWS.getRuoloById(idCorrGlobali);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Ruolo GetRuolo(string idCorrGlobali)
        {
            try
            {
                return docsPaWS.GetRuolo(idCorrGlobali);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static Ruolo getRuoloByIdGruppo(string idGruppo)
        {
            try
            {
                return docsPaWS.getRuoloByIdGruppo(idGruppo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void setRuoloDelegato(Ruolo ruolo)
        {
            HttpContext ctx = HttpContext.Current;
            ctx.Session["userRuoloDelegato"] = ruolo;
        }

        public static Ruolo getRuoloDelegato()
        {
            return (Ruolo)HttpContext.Current.Session["userRuoloDelegato"];
        }

        public static DataSet getUtentiInRuoloSottoposto(string systemId)
        {
            return docsPaWS.getUtentiInRuoloSottoposto(UIManager.UserManager.GetInfoUser(), systemId);
        }

        /// <summary>
        /// Metodo per il recupero della storia di un ruolo
        /// </summary>
        /// <param name="request">Request con le informazioni sul ruolo di cui recuperare la storia</param>
        /// <returns>Storia del ruolo</returns>
        /// <exception cref="Exception">Dettaglio di un errore avvenuto nel backend</exception>
        public static RoleHistoryResponse GetRoleHistory(RoleHistoryRequest request)
        {
            try
            {
                return docsPaWS.GetRoleHistory(request);

            }
            catch (Exception e)
            {
                throw Utils.SoapExceptionParser.GetOriginalException(e);
            }
        }

    }
}