using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using System.Collections;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class TransmissionModelsManager
    {        
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static ArrayList GetTransmissionModelsLite(string idAdm, Registro[] registries, string idPeople, string idCorrGlobal, string idTypeDoc, string idDiagram, string idState, string obj_type, string system_id, string idGroup, bool allReg, string accessrights)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliPerTrasmLite(idAdm, registries, idPeople, idCorrGlobal, idTypeDoc, idDiagram, idState, obj_type, system_id, idGroup, allReg, accessrights));
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList GetTransmissionModelsLiteFasc(string idAdm, Registro[] registries, string idPeople, string idCorrGlobal, string idTypeProj, string idDiagram, string idState, string obj_type, string system_id, string idGroup, bool allReg, string accessrights)
        {
            try
            {
                ArrayList result = new ArrayList(docsPaWS.getModelliPerTrasmLiteFasc(idAdm, registries, idPeople, idCorrGlobal, idTypeProj, idDiagram, idState, obj_type, system_id, idGroup, allReg, accessrights));
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ArrayList GetTransmissionModelsInSession()
        {
            try
            {
                return (ArrayList)GetSessionValue("transmissionModels");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SetTransmissionModelsInSession(ArrayList transmissionModels)
        {
            try
            {
                SetSessionValue("transmissionModels", transmissionModels);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static string GetTemplateSystemId()
        {
            try
            {
                return docsPaWS.getModelloSystemId();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.ModelloTrasmissione GetTemplateById(string idAmministrazione, string id)
        {
            try
            {
                return docsPaWS.getModelloByID(idAmministrazione, id);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SaveTemplate(ModelloTrasmissione template, InfoUtente user)
        {
            try
            {
                docsPaWS.salvaModello(template, user);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static RagioneTrasmissione[] SmistamentoGetRagioniTrasmissione(string idAdm)
        {
            try
            {
                return docsPaWS.SmistamentoGetRagioniTrasmissione(idAdm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.RagioneTrasmissione GetReasonById(string reasonId, InfoUtente infoUser)
        {
            try
            {
                return docsPaWS.getRagioneById(reasonId);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
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
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #region Session functions

        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static System.Object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion
    }
}